using System;
using System.Collections.Generic;
using System.Linq;
using CalamityFandom.Common.Debug;
using CalamityFandom.Common.Elements;
using CalamityFandom.Common.WikiData;
using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

// Lot AO (20/11) : les 38 Tainted Potions de la page qui ont une recette (Placed Bottle : la potion vanilla + Disgusting
// Meat). Une seule classe d'objet et une de buff, une instance par potion ; effets dans TaintedPlayer et les Global*.
// Les 4 lignes « DESCRIPTION / DURATION » de la page (Anechoic Coating, Aureus Cell, Bounding, Cadence) n'ont pas de
// recette : pas faites.

namespace CalamityFandom.Content.Items.Potions
{
    public enum Tainted
    {
        AmmoReservation, Archery, Battle, Builder, Calming, Crate, Dangersense, Endurance, Featherfall, Fishing, Gills,
        Gravitation, Heartreach, Hunter, Inferno, Invisibility, Ironskin, Lifeforce, Love, Luck, MagicPower,
        ManaRegeneration, Mining, NightOwl, ObsidianSkin, Rage, Regeneration, Shine, Sonar, Spelunker, Stink, Summoning,
        Swiftness, Thorns, Titan, Warmth, WaterWalking, Wrath,
    }

    /// <summary>Potion vanilla d'origine, son buff (icône reprise par le tableau de la page) et la durée de la page.</summary>
    public static class TaintedInfo
    {
        public static readonly int Count = Enum.GetValues<Tainted>().Length;

        public static readonly (int item, int buff, float minutes)[] Data =
        {
            (ItemID.AmmoReservationPotion, BuffID.AmmoReservation, 8), (ItemID.ArcheryPotion, BuffID.Archery, 8),
            (ItemID.BattlePotion, BuffID.Battle, 7), (ItemID.BuilderPotion, BuffID.Builder, 45),
            (ItemID.CalmingPotion, BuffID.Calm, 8), (ItemID.CratePotion, BuffID.Crate, 3),
            (ItemID.TrapsightPotion, BuffID.Dangersense, 10), (ItemID.EndurancePotion, BuffID.Endurance, 4),
            (ItemID.FeatherfallPotion, BuffID.Featherfall, 10), (ItemID.FishingPotion, BuffID.Fishing, 8),
            (ItemID.GillsPotion, BuffID.Gills, 4), (ItemID.GravitationPotion, BuffID.Gravitation, 3),
            (ItemID.HeartreachPotion, BuffID.Heartreach, 8), (ItemID.HunterPotion, BuffID.Hunter, 8),
            (ItemID.InfernoPotion, BuffID.Inferno, 4), (ItemID.InvisibilityPotion, BuffID.Invisibility, 3),
            (ItemID.IronskinPotion, BuffID.Ironskin, 8), (ItemID.LifeforcePotion, BuffID.Lifeforce, 8),
            (ItemID.LovePotion, BuffID.Lovestruck, 0.5f), (ItemID.LuckPotion, BuffID.Lucky, 5),
            (ItemID.MagicPowerPotion, BuffID.MagicPower, 4), (ItemID.ManaRegenerationPotion, BuffID.ManaRegeneration, 8),
            (ItemID.MiningPotion, BuffID.Mining, 10), (ItemID.NightOwlPotion, BuffID.NightOwl, 10),
            (ItemID.ObsidianSkinPotion, BuffID.ObsidianSkin, 6), (ItemID.RagePotion, BuffID.Rage, 4),
            (ItemID.RegenerationPotion, BuffID.Regeneration, 8), (ItemID.ShinePotion, BuffID.Shine, 10),
            (ItemID.SonarPotion, BuffID.Sonar, 8), (ItemID.SpelunkerPotion, BuffID.Spelunker, 5),
            (ItemID.StinkPotion, BuffID.Stinky, 0.5f), (ItemID.SummoningPotion, BuffID.Summoning, 8),
            (ItemID.SwiftnessPotion, BuffID.Swiftness, 8), (ItemID.ThornsPotion, BuffID.Thorns, 8),
            (ItemID.TitanPotion, BuffID.Titan, 8), (ItemID.WarmthPotion, BuffID.Warmth, 15),
            (ItemID.WaterWalkingPotion, BuffID.WaterWalking, 10), (ItemID.WrathPotion, BuffID.Wrath, 4),
        };

        public static readonly int[] ItemTypes = new int[Count];
        public static readonly int[] BuffTypes = new int[Count];

        public static int Frames(Tainted k) => (int)(Data[(int)k].minutes * 3600);
    }

    /// <summary>Crée les 38 potions et leurs buffs.</summary>
    public class TaintedLoader : ILoadable
    {
        public void Load(Mod mod)
        {
            foreach (Tainted k in Enum.GetValues<Tainted>())
            {
                mod.AddContent(new TaintedPotion(k));
                mod.AddContent(new TaintedBuff(k));
            }
        }

        public void Unload()
        {
        }
    }

    /// <summary>
    /// Wiki : Tainted Potions.txt — potion « corrompue » : un effet inverse ou détourné de la potion vanilla, durée du
    /// tableau. Ne se boit pas si la potion normale est active, et inversement (Notes de la page).
    /// [choix] Rareté, prix et animation de la potion vanilla.
    /// </summary>
    [Autoload(false)]
    public class TaintedPotion : ModItem
    {
        public readonly Tainted Kind;

        public TaintedPotion(Tainted kind) => Kind = kind;

        public override string Name => $"Tainted{Kind}Potion";
        public override string Texture => $"CalamityFandom/Content/Items/Potions/Tainted/{Name}";
        public override string LocalizationCategory => "TaintedPotions";
        protected override bool CloneNewInstances => true;

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
            TaintedInfo.ItemTypes[(int)Kind] = Type;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(TaintedInfo.Data[(int)Kind].item);
            Item.buffType = TaintedInfo.BuffTypes[(int)Kind];
            Item.buffTime = TaintedInfo.Frames(Kind);
        }

        public override bool CanUseItem(Player player) => !player.HasBuff(TaintedInfo.Data[(int)Kind].buff);
    }

    /// <summary>Buff d'une Tainted Potion : icône du buff vanilla (celle du tableau de la page).</summary>
    [Autoload(false)]
    public class TaintedBuff : ModBuff
    {
        public readonly Tainted Kind;

        public TaintedBuff(Tainted kind) => Kind = kind;

        public override string Name => $"Tainted{Kind}";
        public override string Texture => "Terraria/Images/Buff_" + TaintedInfo.Data[(int)Kind].buff;
        public override string LocalizationCategory => "TaintedPotions";

        public override void SetStaticDefaults() => TaintedInfo.BuffTypes[(int)Kind] = Type;

        public override void Update(Player player, ref int buffIndex)
        {
            var tp = player.GetModPlayer<TaintedPlayer>();
            tp.Active[(int)Kind] = true;
            if (Kind == Tainted.WaterWalking)
                tp.GroundGrace = player.buffTime[buffIndex] > TaintedInfo.Frames(Kind) - 180;
            switch (Kind)
            {
                case Tainted.Archery:
                    player.arrowDamage *= 0.9f;
                    break;
                case Tainted.Builder:
                    player.tileSpeed -= 0.25f;
                    player.wallSpeed -= 0.25f;
                    player.blockRange -= 2;
                    break;
                case Tainted.Fishing:
                    player.fishingSkill -= 20;
                    break;
                case Tainted.Hunter:
                    player.aggro += 800;
                    break;
                case Tainted.Inferno:
                    player.AddBuff(BuffID.OnFire, 2);
                    break;
                case Tainted.MagicPower:
                    player.GetDamage<MagicDamageClass>() -= 0.1f;
                    player.GetKnockback<MagicDamageClass>() += 1f;
                    break;
                case Tainted.ManaRegeneration:
                    player.manaCost += 1f;
                    break;
                case Tainted.Mining:
                    player.pickSpeed += 0.25f;
                    break;
                case Tainted.Rage:
                    player.GetDamage<GenericDamageClass>() += 0.3f;
                    break;
                case Tainted.Swiftness:
                    player.moveSpeed -= 0.25f;
                    break;
                case Tainted.Titan:
                    player.GetKnockback<GenericDamageClass>() *= 0f;
                    break;
                case Tainted.Wrath:
                    player.GetDamage<GenericDamageClass>() *= 0f;
                    break;
            }
        }
    }

    /// <summary>Effets des Tainted Potions sur le joueur. Holders = joueurs qui ont chaque effet (recalculé à chaque image).</summary>
    public class TaintedPlayer : ModPlayer
    {
        public readonly bool[] Active = new bool[TaintedInfo.Count];
        public bool GroundGrace;
        private int pickBefore = -1;
        private int gravityTimer;
        private Item pickItem;

        public static readonly List<int>[] Holders = Enumerable.Range(0, TaintedInfo.Count).Select(_ => new List<int>()).ToArray();

        public bool Has(Tainted k) => Active[(int)k];

        /// <summary>Joueur le plus proche qui a l'effet, dans le rayon (null sinon).</summary>
        public static Player Near(Vector2 pos, Tainted k, float range)
        {
            Player best = null;
            float bestDist = range * range;
            foreach (int i in Holders[(int)k])
            {
                float d = Vector2.DistanceSquared(Main.player[i].Center, pos);
                if (d < bestDist)
                {
                    bestDist = d;
                    best = Main.player[i];
                }
            }
            return best;
        }

        public override void ResetEffects() => Array.Clear(Active);

        public override void PostUpdateEquips()
        {
            if (Has(Tainted.Ironskin))
            {
                Player.statDefense -= 8;
                Player.Calamity().defenseDamageRatio *= 0.5;
            }
            if (Has(Tainted.Lifeforce))
            {
                int lost = Player.statLifeMax2 / 5;
                Player.statLifeMax2 -= lost;
                Player.statManaMax2 += lost;
            }
            if (Has(Tainted.Summoning))
                Player.maxMinions = Math.Max(Player.maxMinions, 999);
        }

        public override void PostUpdateRunSpeeds()
        {
            // Plus d'inertie : on atteint et on quitte la vitesse maximale tout de suite
            if (Has(Tainted.Swiftness))
                Player.runAcceleration = Player.runSlowdown = Player.maxRunSpeed;
        }

        public override void NaturalLifeRegen(ref float regen)
        {
            if (Has(Tainted.Regeneration))
                regen = 0f;
        }

        public override void UpdateLifeRegen()
        {
            if (Has(Tainted.Regeneration))
            {
                Player.lifeRegen = Math.Min(Player.lifeRegen, 0);
                Player.lifeRegenTime = 0;
            }
        }

        public override void UpdateBadLifeRegen()
        {
            // Warmth : les brûlures font deux fois moins mal
            if (Has(Tainted.Warmth) && Player.lifeRegen < 0 && (Player.onFire || Player.onFire2 || Player.onFire3 || Player.burned))
                Player.lifeRegen /= 2;
        }

        public override void PreUpdateMovement()
        {
            if (!Has(Tainted.WaterWalking) || Player.mount.Active)
                return;
            // On marche sur l'air (bas pour descendre) ; toucher le sol tue
            if (Player.velocity.Y > 0f && !Player.controlDown)
            {
                Player.velocity.Y = 0f;
                Player.fallStart = (int)(Player.position.Y / 16f);
            }
            bool onGround = Collision.SolidCollision(Player.BottomLeft, Player.width, 2, true);
            if (onGround && !GroundGrace && Player.whoAmI == Main.myPlayer && !Player.dead)
                Player.KillMe(PlayerDeathReason.ByCustomReason(Language.GetText("Mods.CalamityFandom.TaintedPotions.GroundDeath").ToNetworkText(Player.name)), 9999, 0);
        }

        public override void PostUpdate()
        {
            bool server = Main.netMode != NetmodeID.MultiplayerClient;
            if (Has(Tainted.Gills) && Collision.DrownCollision(Player.position, Player.width, Player.height, Player.gravDir)
                && Main.GameUpdateCount % 7 == 0 && Player.breath > 0)
                Player.breath--;
            if (Has(Tainted.ObsidianSkin) && Player.lavaWet && !Player.dead && Player.whoAmI == Main.myPlayer)
                Player.KillMe(PlayerDeathReason.ByOther(2), 9999, 0);
            if (Has(Tainted.Gravitation) && Player.whoAmI == Main.myPlayer)
            {
                Player.gravControl2 = true;
                if (--gravityTimer <= 0)
                {
                    gravityTimer = Main.rand.Next(120, 361);
                    if (Main.rand.NextBool())
                    {
                        Player.gravDir *= -1f;
                        SoundEngine.PlaySound(SoundID.Item8, Player.Center);
                    }
                }
            }
            if (Has(Tainted.Dangersense) && server && Main.GameUpdateCount % 120 == 0)
                TriggerTraps();
            if (Has(Tainted.Spelunker) && Player.whoAmI == Main.myPlayer && Main.GameUpdateCount % 6 == 0)
                CollectOres();
        }

        /// <summary>Dangersense : les pièges proches (plaques de pression, pièges reliés) se déclenchent sans cesse.</summary>
        private void TriggerTraps()
        {
            Point c = Player.Center.ToTileCoordinates();
            Wiring.SetCurrentUser(Player.whoAmI);
            for (int x = c.X - 30; x <= c.X + 30; x++)
                for (int y = c.Y - 20; y <= c.Y + 20; y++)
                {
                    if (!WorldGen.InWorld(x, y, 5))
                        continue;
                    Tile t = Main.tile[x, y];
                    if (t.HasTile && (t.TileType == TileID.PressurePlates || t.TileType == TileID.WeightedPressurePlate || t.TileType == TileID.Traps))
                        Wiring.TripWire(x, y, 1, 1);
                }
            Wiring.SetCurrentUser();
        }

        /// <summary>Spelunker : un minerai à moins de 4 blocs est ramassé aussitôt.</summary>
        private void CollectOres()
        {
            Point c = Player.Center.ToTileCoordinates();
            for (int x = c.X - 4; x <= c.X + 4; x++)
                for (int y = c.Y - 4; y <= c.Y + 4; y++)
                {
                    if (!WorldGen.InWorld(x, y, 5) || (x - c.X) * (x - c.X) + (y - c.Y) * (y - c.Y) > 16)
                        continue;
                    Tile t = Main.tile[x, y];
                    if (!t.HasTile || !TileID.Sets.Ore[t.TileType])
                        continue;
                    WorldGen.KillTile(x, y);
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, x, y);
                }
        }

        public override bool PreItemCheck()
        {
            // Mining : pioche plus puissante (+50) le temps de l'utilisation
            if (Has(Tainted.Mining) && Player.HeldItem.pick > 0)
            {
                pickItem = Player.HeldItem;
                pickBefore = pickItem.pick;
                pickItem.pick += 50;
            }
            return true;
        }

        public override void PostItemCheck()
        {
            if (pickItem != null)
            {
                pickItem.pick = pickBefore;
                pickItem = null;
            }
        }

        public override bool FreeDodge(Player.HurtInfo info)
            => Has(Tainted.Gills) && Player.breath > 0 && Collision.DrownCollision(Player.position, Player.width, Player.height, Player.gravDir);

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (!Has(Tainted.Warmth))
                return;
            if (modifiers.DamageSource.SourceOtherIndex == 2)
                modifiers.FinalDamage *= 0.5f;
            int npc = modifiers.DamageSource.SourceNPCIndex;
            if (npc >= 0 && ElementData.AttackElements(Main.npc[npc].type).Contains(Element.Fire))
                modifiers.FinalDamage *= 0.75f;
        }

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            if (Has(Tainted.Calming))
                modifiers.FinalDamage *= 0.75f;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Has(Tainted.Rage))
                modifiers.DisableCrit();
            if (Has(Tainted.Calming))
                modifiers.FinalDamage *= 1.5f;
            if (Has(Tainted.Endurance))
            {
                float dr = target.Calamity().DR;
                if (dr > 0f && dr < 1f)
                    modifiers.FinalDamage *= (1f - Math.Max(0f, dr - 0.1f)) / (1f - dr);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Has(Tainted.NightOwl))
                target.AddBuff(BuffID.Confused, 120);
            if (target.friendly || target.lifeMax <= 5 || target.SpawnedFromStatue)
                return;
            if (Has(Tainted.Regeneration) && Main.rand.NextBool(20))
                Item.NewItem(Player.GetSource_OnHit(target), target.Hitbox, ItemID.Heart);
            if (Has(Tainted.ManaRegeneration) && hit.DamageType.CountsAsClass<MagicDamageClass>() && Main.rand.NextBool(10)
                && Player.whoAmI == Main.myPlayer)
            {
                Vector2 from = target.Center + new Vector2(Main.rand.NextFloat(-150f, 150f), -700f);
                int p = Projectile.NewProjectile(Player.GetSource_OnHit(target), from, (target.Center - from).SafeNormalize(Vector2.UnitY) * 18f,
                    ProjectileID.Starfury, Math.Max(1, damageDone / 2), 2f, Player.whoAmI);
                Main.projectile[p].DamageType = DamageClass.Magic;
            }
        }

        public override float UseSpeedMultiplier(Item item) => Has(Tainted.Archery) && item.useAmmo == AmmoID.Arrow ? 1.3f : 1f;

        public override void ModifyShootStats(Item item, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (Has(Tainted.Archery) && item.useAmmo == AmmoID.Arrow)
                velocity *= 0.8f;
        }

        public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Ammo Reservation : 20 % de chances de tirer deux fois et de consommer deux munitions
            if (Has(Tainted.AmmoReservation) && item.useAmmo > 0 && Main.rand.NextFloat() < 0.2f
                && Player.PickAmmo(item, out _, out _, out _, out _, out _))
                Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.08f), type, damage, knockback, Player.whoAmI);
            return true;
        }

        public override void ModifyFishingAttempt(ref FishingAttempt attempt)
        {
            if (Has(Tainted.Crate) && attempt.crate && Main.rand.NextBool())
                attempt.crate = false;
        }

        private static readonly int[] SonarPool =
        {
            ItemID.Bass, ItemID.Trout, ItemID.Salmon, ItemID.AtlanticCod, ItemID.Tuna, ItemID.RedSnapper, ItemID.NeonTetra,
            ItemID.ArmoredCavefish, ItemID.Damselfish, ItemID.CrimsonTigerfish, ItemID.FrostMinnow, ItemID.PrincessFish,
            ItemID.GoldenCarp, ItemID.SpecularFish, ItemID.Prismite, ItemID.VariegatedLardfish, ItemID.FlarefinKoi,
            ItemID.DoubleCod, ItemID.Obsidifish, ItemID.Ebonkoi, ItemID.Hemopiranha, ItemID.ChaosFish, ItemID.Stinkfish,
            ItemID.Shrimp, ItemID.Honeyfin, ItemID.WoodenCrate, ItemID.IronCrate, ItemID.GoldenCrate, ItemID.OldShoe, ItemID.TinCan,
        };

        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            // Fishing : ennemis de la Blood Moon à toute heure
            if (Has(Tainted.Fishing) && !Main.bloodMoon && !attempt.inLava && !attempt.inHoney && Main.rand.NextBool(6))
            {
                int[] pool = Main.hardMode
                    ? new int[] { NPCID.EyeballFlyingFish, NPCID.ZombieMerman, NPCID.GoblinShark, NPCID.BloodEelHead, NPCID.BloodNautilus }
                    : new int[] { NPCID.EyeballFlyingFish, NPCID.ZombieMerman };
                npcSpawn = Main.rand.Next(pool);
                itemDrop = -1;
                return;
            }
            // Sonar : le poisson accroché change sans cesse ; c'est celui du moment où on tire qui sort
            if (Has(Tainted.Sonar) && itemDrop > 0 && attempt.questFish != itemDrop)
                itemDrop = Main.rand.Next(SonarPool);
        }

        public override void ModifyCaughtFish(Item fish)
        {
            if (Has(Tainted.Crate) && ItemID.Sets.IsFishingCrate[fish.type])
                fish.stack *= 2;
        }
    }

    /// <summary>Effets des Tainted Potions sur les ennemis.</summary>
    public class TaintedGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        private bool battle, revealed, falling;
        private float fallStart;
        private int loveCooldown;

        private static bool Hostile(NPC npc) => npc.active && !npc.friendly && !npc.townNPC && npc.lifeMax > 5 && !npc.dontTakeDamage;

        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            var tp = player.GetModPlayer<TaintedPlayer>();
            if (tp.Has(Tainted.Battle))
            {
                spawnRate *= 2;
                maxSpawns = Math.Max(1, maxSpawns / 2);
            }
            if (tp.Has(Tainted.Calming))
            {
                spawnRate = Math.Max(1, spawnRate / 2);
                maxSpawns = (int)(maxSpawns * 1.5f);
            }
        }

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (!npc.boss && Hostile(npc) && TaintedPlayer.Near(npc.Center, Tainted.Battle, 3000f) != null)
            {
                battle = true;
                npc.lifeMax = npc.life = (int)(npc.lifeMax * 1.3f);
                npc.damage = (int)(npc.damage * 1.3f);
            }
        }

        public override void OnKill(NPC npc)
        {
            if (!Hostile(npc) || npc.SpawnedFromStatue)
                return;
            Player closest = Main.player[Player.FindClosest(npc.position, npc.width, npc.height)];
            // Battle : butin en double (une seconde résolution des règles du PNJ)
            if (battle && !npc.boss)
                Main.ItemDropSolver.TryDropping(new DropAttemptInfo
                {
                    player = closest, npc = npc, IsExpertMode = Main.expertMode, IsMasterMode = Main.masterMode, IsInSimulation = false, rng = Main.rand,
                });
            // Luck : parfois une Astral Pearl
            if (closest.GetModPlayer<TaintedPlayer>().Has(Tainted.Luck) && Main.rand.NextBool(50))
            {
                int pearl = WikiRecipes.ResolveItem("Astral Pearl");
                if (pearl > 0)
                    Item.NewItem(npc.GetSource_Loot(), npc.Hitbox, pearl);
            }
        }

        public override void PostAI(NPC npc)
        {
            if (!Hostile(npc) || Main.netMode == NetmodeID.MultiplayerClient)
                return;
            if (loveCooldown > 0)
                loveCooldown--;
            // Featherfall : dégâts de chute des ennemis, comme ceux du joueur (au-delà de 25 blocs, 10 par bloc)
            if (!npc.noGravity && !npc.noTileCollide && !npc.boss)
            {
                if (npc.velocity.Y > 0.1f && !falling)
                {
                    falling = true;
                    fallStart = npc.position.Y;
                }
                else if (npc.velocity.Y < 0f)
                    falling = false;
                else if (falling && npc.velocity.Y == 0f)
                {
                    falling = false;
                    float tiles = (npc.position.Y - fallStart) / 16f;
                    if (tiles > 25f && TaintedPlayer.Near(npc.Center, Tainted.Featherfall, 3000f) != null)
                        npc.SimpleStrikeNPC((int)((tiles - 25f) * 10f), 0);
                }
            }
            // Obsidian Skin : la lave enlève 5 % de la vie des ennemis (toutes les demi-secondes)
            if (npc.lavaWet && Main.GameUpdateCount % 30 == 0 && TaintedPlayer.Near(npc.Center, Tainted.ObsidianSkin, 3000f) != null)
                npc.SimpleStrikeNPC(Math.Max(1, npc.lifeMax / 20), 0);
            // Stink : les ennemis proches fuient
            Player stink = npc.boss ? null : TaintedPlayer.Near(npc.Center, Tainted.Stink, 320f);
            if (stink != null)
            {
                int dir = npc.Center.X < stink.Center.X ? -1 : 1;
                if (npc.noGravity)
                {
                    npc.velocity += (npc.Center - stink.Center).SafeNormalize(Vector2.UnitX) * 0.6f;
                    if (npc.velocity.Length() > 8f)
                        npc.velocity = npc.velocity.SafeNormalize(Vector2.UnitX) * 8f;
                }
                else
                    npc.velocity.X = dir * Math.Max(Math.Abs(npc.velocity.X), 3f);
                npc.direction = npc.spriteDirection = dir;
            }
        }

        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone) => revealed = true;

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone) => revealed = true;

        private bool Hidden(NPC npc) => !revealed && Hostile(npc) && Main.LocalPlayer.GetModPlayer<TaintedPlayer>().Has(Tainted.Invisibility);

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) => !Hidden(npc);

        public override bool? DrawHealthBar(NPC npc, byte hbPosition, ref float scale, ref Vector2 position) => Hidden(npc) ? false : null;

        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
            => revealed || !target.GetModPlayer<TaintedPlayer>().Has(Tainted.Invisibility);

        /// <summary>Love, Inferno, Wrath : effets qui touchent tous les ennemis autour d'un joueur (une fois par image).</summary>
        public static void UpdateAll()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            bool love = TaintedPlayer.Holders[(int)Tainted.Love].Count > 0, inferno = TaintedPlayer.Holders[(int)Tainted.Inferno].Count > 0;
            if ((love && Main.GameUpdateCount % 10 == 0) || (inferno && Main.GameUpdateCount % 30 == 0))
                for (int a = 0; a < Main.maxNPCs; a++)
                {
                    NPC npc = Main.npc[a];
                    if (!Hostile(npc))
                        continue;
                    if (inferno && Main.GameUpdateCount % 30 == 0 && TaintedPlayer.Near(npc.Center, Tainted.Inferno, 2500f) != null)
                        npc.AddBuff(BuffID.OnFire, 60);
                    // Love : les ennemis se blessent entre eux au contact
                    if (love && Main.GameUpdateCount % 10 == 0 && npc.damage > 0 && TaintedPlayer.Near(npc.Center, Tainted.Love, 2500f) != null)
                        for (int b = 0; b < Main.maxNPCs; b++)
                        {
                            NPC other = Main.npc[b];
                            if (b == a || !Hostile(other) || other.GetGlobalNPC<TaintedGlobalNPC>().loveCooldown > 0 || !npc.Hitbox.Intersects(other.Hitbox))
                                continue;
                            other.GetGlobalNPC<TaintedGlobalNPC>().loveCooldown = 30;
                            other.SimpleStrikeNPC(npc.damage, npc.Center.X < other.Center.X ? 1 : -1, false, 4f);
                        }
                }
            // Wrath : chaque seconde, un ennemi proche (pas un boss) disparaît en lâchant son butin
            if (Main.GameUpdateCount % 60 == 0)
                foreach (int i in TaintedPlayer.Holders[(int)Tainted.Wrath])
                {
                    var targets = Main.npc.Where(n => Hostile(n) && !n.boss && n.CanBeChasedBy() && n.Distance(Main.player[i].Center) < 1500f).ToList();
                    if (targets.Count > 0)
                        Main.rand.Next(targets).StrikeInstantKill();
                }
        }
    }

    /// <summary>Heartreach : les cœurs ne se ramassent plus et foncent sur les ennemis.</summary>
    public class TaintedGlobalItem : GlobalItem
    {
        private static bool IsHeart(int type) => type == ItemID.Heart || type == ItemID.CandyApple || type == ItemID.CandyCane;

        public override bool CanPickup(Item item, Player player)
            => !IsHeart(item.type) || !player.GetModPlayer<TaintedPlayer>().Has(Tainted.Heartreach);

        public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
        {
            if (!IsHeart(item.type) || Main.netMode == NetmodeID.MultiplayerClient || TaintedPlayer.Near(item.Center, Tainted.Heartreach, 1000f) == null)
                return;
            NPC target = null;
            float best = 500f;
            foreach (NPC n in Main.npc)
                if (n.CanBeChasedBy() && n.Distance(item.Center) < best)
                {
                    best = n.Distance(item.Center);
                    target = n;
                }
            if (target == null)
                return;
            gravity = 0f;
            item.velocity = Vector2.Lerp(item.velocity, (target.Center - item.Center).SafeNormalize(Vector2.Zero) * 9f, 0.2f);
            if (item.Hitbox.Intersects(target.Hitbox))
            {
                // [choix] 40 dégâts par cœur
                target.SimpleStrikeNPC(40, item.Center.X < target.Center.X ? 1 : -1);
                item.TurnToAir();
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, item.whoAmI);
            }
        }

        public override bool CanUseItem(Item item, Player player)
        {
            // Une potion vanilla ne se boit pas pendant sa version Tainted
            for (int k = 0; k < TaintedInfo.Count; k++)
                if (TaintedInfo.Data[k].item == item.type && player.HasBuff(TaintedInfo.BuffTypes[k]))
                    return false;
            return true;
        }

        public override bool ConsumeItem(Item item, Player player)
        {
            // Builder : 20 % de chances qu'un bloc posé ne soit pas consommé (il ne lâchera rien une fois cassé)
            if (item.createTile < TileID.Dirt || Main.tileFrameImportant[item.createTile] || !player.GetModPlayer<TaintedPlayer>().Has(Tainted.Builder)
                || player.itemAnimation <= 0 || Main.rand.NextFloat() >= 0.2f)
                return true;
            TaintedGlobalTile.MarkFree();
            return false;
        }
    }

    /// <summary>Builder (blocs posés sans être consommés : pas de butin) et Spelunker (minerais déguisés).</summary>
    public class TaintedGlobalTile : GlobalTile
    {
        // [technique] Positions non sauvegardées : après un rechargement du monde, ces blocs relâchent leur objet
        private static readonly HashSet<Point> Free = new();
        private static (Point pos, uint frame) lastPlaced = (Point.Zero, uint.MaxValue);
        private static uint pendingFrame = uint.MaxValue;

        public static void MarkFree()
        {
            if (lastPlaced.frame == Main.GameUpdateCount)
                Free.Add(lastPlaced.pos);
            else
                pendingFrame = Main.GameUpdateCount;
        }

        public override void PlaceInWorld(int i, int j, int type, Item item)
        {
            lastPlaced = (new Point(i, j), Main.GameUpdateCount);
            if (pendingFrame == Main.GameUpdateCount)
            {
                Free.Add(new Point(i, j));
                pendingFrame = uint.MaxValue;
            }
        }

        public override bool CanDrop(int i, int j, int type) => !Free.Remove(new Point(i, j));

        public override bool PreDraw(int i, int j, int type, SpriteBatch spriteBatch)
        {
            if (!TileID.Sets.Ore[type] || Main.gameMenu || !Main.LocalPlayer.GetModPlayer<TaintedPlayer>().Has(Tainted.Spelunker))
                return true;
            // Le minerai prend l'apparence d'un bloc voisin (pierre par défaut)
            int look = TileID.Stone;
            for (int r = 1; r <= 3 && look == TileID.Stone; r++)
                for (int dx = -r; dx <= r; dx++)
                    for (int dy = -r; dy <= r; dy++)
                    {
                        if (!WorldGen.InWorld(i + dx, j + dy))
                            continue;
                        Tile n = Main.tile[i + dx, j + dy];
                        if (n.HasTile && Main.tileSolid[n.TileType] && !Main.tileFrameImportant[n.TileType] && !TileID.Sets.Ore[n.TileType])
                        {
                            look = n.TileType;
                            goto found;
                        }
                    }
            found:
            Main.instance.LoadTiles(look);
            Tile t = Main.tile[i, j];
            Vector2 offset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            spriteBatch.Draw(TextureAssets.Tile[look].Value, new Vector2(i * 16, j * 16) - Main.screenPosition + offset,
                new Rectangle(t.TileFrameX, t.TileFrameY, 16, 16), Lighting.GetColor(i, j));
            return false;
        }
    }

    /// <summary>Shine (tirs à tête chercheuse), Summoning (sbires de 5 s), Thorns (ennemis qui renvoient les tirs).</summary>
    public class TaintedGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        private int age;
        private HashSet<int> rolled;

        private static TaintedPlayer Owner(Projectile p)
            => p.friendly && !p.hostile && p.owner >= 0 && p.owner < Main.maxPlayers && Main.player[p.owner].active ? Main.player[p.owner].GetModPlayer<TaintedPlayer>() : null;

        public override void PostAI(Projectile projectile)
        {
            var tp = Owner(projectile);
            if (tp == null || projectile.owner != Main.myPlayer)
                return;
            if (tp.Has(Tainted.Summoning) && projectile.minion && projectile.minionSlots > 0f && ++age >= 300)
            {
                projectile.Kill();
                return;
            }
            if (tp.Has(Tainted.Shine) && !projectile.minion && !projectile.sentry && projectile.damage > 0 && projectile.aiStyle != ProjAIStyleID.Spear
                && !projectile.ownerHitCheck && tp.Player.heldProj != projectile.whoAmI && projectile.velocity.LengthSquared() > 4f)
            {
                NPC target = projectile.FindTargetWithinRange(500f);
                if (target != null)
                {
                    float speed = projectile.velocity.Length();
                    projectile.velocity = Vector2.Lerp(projectile.velocity, (target.Center - projectile.Center).SafeNormalize(Vector2.Zero) * speed, 0.08f);
                    if (Main.GameUpdateCount % 20 == 0)
                        projectile.netUpdate = true;
                }
            }
        }

        public override bool? CanHitNPC(Projectile projectile, NPC target)
        {
            var tp = Owner(projectile);
            if (tp == null || !tp.Has(Tainted.Thorns) || projectile.minion || target.friendly || tp.Player.heldProj == projectile.whoAmI
                || projectile.velocity.LengthSquared() < 1f || !projectile.Colliding(projectile.Hitbox, target.Hitbox))
                return null;
            rolled ??= new HashSet<int>();
            if (!rolled.Add(target.whoAmI))
                return null;
            // [choix] 25 % des tirs renvoyés ; le tir renvoyé reste amical et peut toucher les autres ennemis
            if (Main.rand.NextFloat() >= 0.25f)
                return null;
            projectile.velocity *= -1f;
            projectile.netUpdate = true;
            return false;
        }
    }

    /// <summary>Night Owl (jour assombri), Shine (écran noirci), mise à jour des porteurs d'effets et contrôles de l'auto-test.</summary>
    public class TaintedSystem : ModSystem
    {
        public override void PreUpdateNPCs()
        {
            foreach (var list in TaintedPlayer.Holders)
                list.Clear();
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player p = Main.player[i];
                if (!p.active || p.dead)
                    continue;
                var tp = p.GetModPlayer<TaintedPlayer>();
                for (int k = 0; k < TaintedInfo.Count; k++)
                    if (tp.Active[k])
                        TaintedPlayer.Holders[k].Add(i);
            }
        }

        public override void PostUpdateNPCs() => TaintedGlobalNPC.UpdateAll();

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            layers.Insert(0, new LegacyGameInterfaceLayer("CalamityFandom: Tainted darkness", () =>
            {
                var tp = Main.LocalPlayer.GetModPlayer<TaintedPlayer>();
                // [choix] Assombri à 50 % (Night Owl, le jour) ; « noirci » à 85 % (Shine) pour rester jouable
                float alpha = tp.Has(Tainted.Shine) ? 0.85f : tp.Has(Tainted.NightOwl) && Main.dayTime ? 0.5f : 0f;
                if (alpha > 0f)
                    Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * alpha);
                return true;
            }, InterfaceScaleType.None));
        }

        public override void Load()
        {
            SelfTest.ExtraChecks.Add(t =>
            {
                int meat = WikiRecipes.ResolveItem("Disgusting Meat");
                int done = 0;
                for (int k = 0; k < TaintedInfo.Count; k++)
                {
                    int type = TaintedInfo.ItemTypes[k];
                    bool recipe = Main.recipe.Take(Recipe.numRecipes).Any(r => !r.Disabled && r.createItem.type == type
                        && r.requiredItem.Any(i => i.type == TaintedInfo.Data[k].item || (k == (int)Tainted.Luck && (i.type == ItemID.LuckPotionLesser || i.type == ItemID.LuckPotionGreater)))
                        && r.requiredItem.Any(i => i.type == meat));
                    var sample = ContentSamples.ItemsByType[type];
                    if (recipe && sample.buffType == TaintedInfo.BuffTypes[k] && sample.buffTime == TaintedInfo.Frames((Tainted)k))
                        done++;
                    else
                        t.Expect(false, $"Tainted Potion {Lang.GetItemNameValue(type)} : recette du wiki ({recipe}), buff {sample.buffType}, durée {sample.buffTime}");
                }
                t.Expect(done == TaintedInfo.Count, $"Tainted Potions : {done}/{TaintedInfo.Count} avec recette, buff et durée");
                t.Expect(Lang.GetItemNameValue(TaintedInfo.ItemTypes[(int)Tainted.Archery]) == "Tainted Archery Potion", "nom de la Tainted Archery Potion");
            });
        }
    }
}
