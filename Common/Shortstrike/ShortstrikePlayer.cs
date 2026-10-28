using System;
using System.Linq;
using CalamityFandom.Common.Config;
using CalamityMod;
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
        // Sahara Slicers (Calamity++) : éclats du carquois tirés un par un pendant le Shortflurry
        private ShortstrikeData.Effect quiverEffect;
        private int quiverInterval, quiverTimer;
        private readonly System.Collections.Generic.Dictionary<int, int> immuneTimers = new();

        public bool Full => Points >= ShortstrikeData.Current.MaxPoints;
        public bool FlurryActive => flurryTimer > 0;
        /// <summary>Part du Shortflurry encore à venir (1 → 0) : la jauge se vide au même rythme.</summary>
        public float FlurryRemaining => FlurryActive ? flurryTimer / (float)flurryDuration : 0f;

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
            // Estoc d'une épée courte de Calamity et tout ce qu'il tire (rayons, éclats...) : « damage dealt using a shortsword »
            if (proj.GetGlobalProjectile<ShortstrikeProjectile>().FromShortsword)
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
            SoundEngine.PlaySound(FlurryStartSound, Player.Center);
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
                    case "saharaQuiver" when effect.ProjectileType > 0:
                        // Tout le carquois (éclats dans le dos) part pendant la durée, régulièrement
                        quiverEffect = effect;
                        quiverInterval = Math.Max(1, flurryDuration / Math.Max(1, Player.Calamity().saharaSlicersBolts));
                        quiverTimer = 0;
                        break;
                    case "throwClone" when Player.whoAmI == Main.myPlayer:
                        // Clone lancé vers le curseur, qui éclate en étoiles (Projectile) à chaque ennemi traversé
                        Vector2 aim = (Main.MouseWorld - Player.Center).SafeNormalize(Vector2.UnitX * Player.direction) * effect.Speed;
                        int damage = (int)(Player.GetWeaponDamage(item) * (1f + Flurry.DamageBoost));
                        var thrown = Projectile.NewProjectileDirect(Player.GetSource_ItemUse(item), Player.Center, aim,
                            ModContent.ProjectileType<ShortflurryThrown>(), damage, Player.GetWeaponKnockback(item), Player.whoAmI,
                            item.type, effect.ProjectileType, effect.Count);
                        thrown.DamageType = item.DamageType;
                        break;
                }
            }
        }

        /// <summary>Son de départ choisi dans la config client (Calamity : sons de ses capacités d'armure).</summary>
        private static SoundStyle FlurryStartSound => (FandomClientConfig.Instance?.ShortflurrySound ?? FandomClientConfig.FlurrySound.SilvaActivation) switch
        {
            FandomClientConfig.FlurrySound.ShortswordStab => SoundID.Item71,
            FandomClientConfig.FlurrySound.OmegaBlueAbility => new SoundStyle("CalamityMod/Sounds/Custom/AbilitySounds/OmegaBlueAbility"),
            FandomClientConfig.FlurrySound.PlagueReaperAbility => new SoundStyle("CalamityMod/Sounds/Custom/AbilitySounds/PlagueReaperAbility"),
            FandomClientConfig.FlurrySound.BrimflameAbility => new SoundStyle("CalamityMod/Sounds/Custom/AbilitySounds/BrimflameAbility"),
            FandomClientConfig.FlurrySound.SonicBoomBlade => SoundID.DD2_SonicBoomBladeSlash,
            _ => new SoundStyle("CalamityMod/Sounds/Custom/AbilitySounds/SilvaActivation"),
        };

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
            if (quiverEffect != null && Player.whoAmI == Main.myPlayer && ++quiverTimer >= quiverInterval)
            {
                quiverTimer = 0;
                FireQuiverBolt();
            }
            if (--flurryTimer <= 0)
            {
                Flurry = null;
                quiverEffect = null;
            }
        }

        /// <summary>Tire un éclat du carquois des Sahara Slicers vers le curseur, comme leur clic droit (sans le consommer en plus).</summary>
        private void FireQuiverBolt()
        {
            ref int bolts = ref Player.Calamity().saharaSlicersBolts;
            if (bolts <= 0 || flurryItem == null || flurryItem.IsAir)
                return;
            bolts--; // l'éclat du dos disparaît de lui-même
            Vector2 aim = (Main.MouseWorld - Player.Center).SafeNormalize(Vector2.UnitX * Player.direction).RotatedByRandom(quiverEffect.Spread) * quiverEffect.Speed;
            int damage = (int)(Player.GetWeaponDamage(flurryItem) * (1f + Flurry.DamageBoost));
            SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaivePierce with { Pitch = 1.5f }, Player.Center);
            Projectile.NewProjectile(Player.GetSource_ItemUse(flurryItem), Player.Center, aim, quiverEffect.ProjectileType, damage,
                Player.GetWeaponKnockback(flurryItem), Player.whoAmI, 1f);
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

    /// <summary>
    /// Marque les projectiles tirés par une épée courte (et leurs enfants) : ils remplissent la jauge ([choix] du 28/10 :
    /// les rayons du Cosmic Shiv et les éclats des Sahara Slicers ne comptaient pas). Les clones du Shortflurry et ce
    /// qu'ils créent restent exclus.
    /// </summary>
    public class ShortstrikeProjectile : GlobalProjectile
    {
        public bool FromShortsword;
        /// <summary>Projectile qui tombe du ciel sur l'ennemi touché (croissants du Shortflurry du Galileo Gladius, Full Fandom).</summary>
        public int RainType = -1;

        public override bool InstancePerEntity => true;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (projectile.ModProjectile is ShortflurryClone or ShortflurryThrown)
                return;
            FromShortsword = source switch
            {
                EntitySource_ItemUse use => ShortstrikeData.IsShortsword(use.Item),
                EntitySource_Parent { Entity: Projectile parent } => parent.GetGlobalProjectile<ShortstrikeProjectile>().FromShortsword,
                _ => false,
            };
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            // « crescent moons will drop down planetoids on enemy hits » : même délai que les planètes de l'arme
            if (RainType > 0)
                Content.Legacy.Projectiles.Melee.GalileoPlanetPlayer.TryRain(Main.player[projectile.owner], projectile.GetSource_FromThis(),
                    target.Center, projectile.damage * 2, RainType);
        }
    }

    /// <summary>
    /// Jauge pleine ou Shortflurry en cours : le clic droit appartient au Shortflurry, pas à l'attaque secondaire de l'arme
    /// (lancer du Galileo Gladius, éclats des Sahara Slicers ; décision du 28/10).
    /// </summary>
    public class ShortstrikeAltBlock : GlobalItem
    {
        public override bool CanUseItem(Item item, Player player)
        {
            if (player.altFunctionUse != 2 || !ShortstrikeData.IsShortsword(item))
                return true;
            var modPlayer = player.GetModPlayer<ShortstrikePlayer>();
            return !modPlayer.Full && !modPlayer.FlurryActive;
        }
    }
}
