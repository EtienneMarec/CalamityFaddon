using System;
using System.Linq;
using CalamityFandom.Content.Shortstrike;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityFandom.Common.Shortstrike
{
    /// <summary>
    /// Wiki : Shortstrike.txt — la jauge du joueur et le Shortflurry en cours.
    /// - 1 point tous les 10 dégâts d'épée courte, 1 point par coup d'estoc qui touche ; 100 au maximum, gardés en changeant
    ///   d'arme (et sauvegardés avec le personnage).
    /// - En tenant une épée courte : +1 % de dégâts tous les 5 points, +1 défense tous les 3 (Calamity++ : Balance/).
    /// - Jauge pleine : son, aura orange ; clic droit avec une épée courte = Shortflurry (la jauge retombe à 0).
    /// </summary>
    public class ShortstrikePlayer : ModPlayer
    {
        public int Points;
        private int damageCarry;
        private bool swingHit;

        // Shortflurry en cours
        public ShortstrikeData.Attack Flurry;
        private Item flurryItem;
        private int flurryTimer, flurryDuration, flurryHitsLeft, flurryNextHit;
        private int regenTimer, regenValue, immuneAllTimer;
        private readonly System.Collections.Generic.Dictionary<int, int> immuneTimers = new();

        public bool Full => Points >= ShortstrikeData.Current.MaxPoints;
        public bool FlurryActive => flurryTimer > 0;

        private static ShortstrikeData.Rules Rules => ShortstrikeData.Current;

        // ------------------------------------------------------------------ points

        /// <summary>Points gagnés par un coup d'épée courte (pas par les clones du Shortflurry).</summary>
        public void AddHit(int damageDone)
        {
            if (FlurryActive)
                return;
            bool wasFull = Full;
            damageCarry += damageDone;
            int gained = damageCarry / Rules.DamagePerPoint;
            damageCarry %= Rules.DamagePerPoint;
            if (!swingHit)
            {
                swingHit = true;
                gained += Rules.PointPerHitSwing;
            }
            Points = Math.Min(Rules.MaxPoints, Points + gained);
            if (Full && !wasFull && Player.whoAmI == Main.myPlayer)
                SoundEngine.PlaySound(SoundID.Item29 with { Volume = 0.6f, Pitch = 0.3f }, Player.Center);
        }

        public override void PostItemCheck()
        {
            // Nouveau coup : il pourra de nouveau rapporter son point de « coup qui touche »
            if (Player.itemAnimation == Player.itemAnimationMax)
                swingHit = false;
        }

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (ShortstrikeData.IsShortsword(item))
                AddHit(damageDone);
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Item held = Player.HeldItem;
            if (proj.ModProjectile is ShortflurryClone || !ShortstrikeData.IsShortsword(held))
                return;
            // Estoc d'une épée courte de Calamity (projectile de l'arme tenue)
            if (proj.type == held.shoot)
                AddHit(damageDone);
        }

        // ------------------------------------------------------------------ bonus passifs

        public override void PostUpdateEquips()
        {
            if (ShortstrikeData.IsShortsword(Player.HeldItem))
            {
                Player.GetDamage(DamageClass.Generic) += Points / Rules.PointsPerDamagePercent / 100f;
                Player.statDefense += Points / Rules.PointsPerDefense;
            }
            if (FlurryActive)
                ApplyFlurryStats();
            if (regenTimer > 0)
            {
                regenTimer--;
                Player.lifeRegen += regenValue * 2; // lifeRegen compte en demi-PV par seconde
            }
            if (immuneAllTimer > 0)
            {
                immuneAllTimer--;
                for (int buff = 0; buff < BuffLoader.BuffCount; buff++)
                    if (Main.debuff[buff] && !BuffID.Sets.NurseCannotRemoveDebuff[buff])
                        Player.buffImmune[buff] = true;
            }
            foreach (int buff in immuneTimers.Keys.ToList())
            {
                Player.buffImmune[buff] = true;
                if (--immuneTimers[buff] <= 0)
                    immuneTimers.Remove(buff);
            }
        }

        private void ApplyFlurryStats()
        {
            foreach (var effect in Flurry.Effects)
            {
                switch (effect.Type)
                {
                    case "defense":
                        Player.statDefense += (int)effect.Value;
                        break;
                    case "defensePercent":
                        Player.statDefense += (int)(Player.statDefense * effect.Value);
                        break;
                    case "damageReduction":
                        Player.endurance += effect.Value;
                        break;
                    case "moveSpeed":
                        Player.moveSpeed += effect.Value;
                        break;
                    case "meleeSpeed":
                        Player.GetAttackSpeed(DamageClass.Melee) += effect.Value;
                        break;
                    case "meleeCrit":
                        Player.GetCritChance(DamageClass.Melee) += effect.Value;
                        break;
                }
            }
        }

        // ------------------------------------------------------------------ Shortflurry

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (!Full || FlurryActive || Player.mouseInterface || !PlayerInput.Triggers.JustPressed.MouseRight)
                return;
            if (Player.dead || Player.CCed || !ShortstrikeData.IsShortsword(Player.HeldItem))
                return;
            StartFlurry(Player.HeldItem);
        }

        public void StartFlurry(Item item)
        {
            Flurry = ShortstrikeData.For(item);
            flurryItem = item;
            Points = 0;
            damageCarry = 0;
            flurryDuration = flurryTimer = Math.Max(1, (int)(Flurry.Seconds * 60));
            flurryHitsLeft = Flurry.Hits;
            flurryNextHit = 0;
            SoundEngine.PlaySound(SoundID.Item71, Player.Center);
            Player.AddBuff(ModContent.BuffType<ShortflurryBuff>(), flurryDuration);
            foreach (var effect in Flurry.Effects)
            {
                int ticks = (int)((effect.Seconds ?? 0f) * 60);
                switch (effect.Type)
                {
                    case "gainBuff" when effect.BuffType > 0:
                        Player.AddBuff(effect.BuffType, ticks);
                        break;
                    case "immune" when effect.BuffType > 0:
                        immuneTimers[effect.BuffType] = ticks;
                        Player.ClearBuff(effect.BuffType);
                        break;
                    case "immuneAll":
                        immuneAllTimer = ticks;
                        break;
                    case "lifeRegen":
                        regenTimer = ticks;
                        regenValue = (int)effect.Value;
                        break;
                }
            }
        }

        public override void PostUpdate()
        {
            if (!FlurryActive)
                return;
            // Les coups des clones, répartis sur la durée
            if (Player.whoAmI == Main.myPlayer && flurryHitsLeft > 0 && flurryDuration - flurryTimer >= flurryNextHit)
            {
                SpawnClone();
                flurryHitsLeft--;
                flurryNextHit = (Flurry.Hits - flurryHitsLeft) * flurryDuration / Math.Max(1, Flurry.Hits);
            }
            if (--flurryTimer <= 0)
                Flurry = null;
        }

        private void SpawnClone()
        {
            Item item = flurryItem;
            if (item == null || item.IsAir)
                return;
            Vector2 direction = (Main.MouseWorld - Player.Center).SafeNormalize(Vector2.UnitX * Player.direction).RotatedByRandom(0.35f);
            int damage = (int)(Player.GetWeaponDamage(item) * (1f + Flurry.DamageBoost));
            Projectile.NewProjectile(Player.GetSource_ItemUse(item), Player.Center, direction, ModContent.ProjectileType<ShortflurryClone>(),
                damage, Player.GetWeaponKnockback(item), Player.whoAmI, item.type);
        }

        // ------------------------------------------------------------------ aura et sauvegarde

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (!Full || drawInfo.shadow != 0f)
                return;
            // « an orange pulsing aura around themselves »
            float pulse = 0.5f + 0.5f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 6f);
            Lighting.AddLight(Player.Center, 0.9f * pulse, 0.45f * pulse, 0.05f);
            if (Main.rand.NextFloat() < 0.35f * pulse)
            {
                var dust = Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.OrangeTorch, 0f, -1f, 100, default, 1.2f);
                dust.noGravity = true;
                dust.velocity *= 0.5f;
            }
        }

        public override void SaveData(TagCompound tag) => tag["shortstrike"] = Points;

        public override void LoadData(TagCompound tag) => Points = tag.GetInt("shortstrike");
    }
}
