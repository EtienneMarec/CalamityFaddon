using System;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

// Lot P (19/11) : « coverings » (accessoires) du wiki fabricables avec ce que le jeu a déjà (pages d'addon comprises,
// décisions section 11). Valeurs et effets des pages ; recettes du wiki (WikiRecipes).

namespace CalamityFandom.Content.Items.Accessories
{
    /// <summary>Accessoire du lot P : effets portés par <see cref="WikiCoveringPlayer"/>.</summary>
    public abstract class WikiCovering : ModItem
    {
        protected abstract int Rarity { get; }
        protected abstract int SellCopper { get; }

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = Item.height = 32;
            Item.accessory = true;
            Item.rare = Rarity;
            Item.value = SellCopper * 5;
        }
    }

    /// <summary>
    /// Wiki : Blazing Powder.txt — une attaque à distance sur deux crée une boule de feu astrale (25 dégâts, Hellfire 5 s).
    /// [choix] Les 25 dégâts suivent les bonus de dégâts à distance ; la boule part vers le curseur.
    /// </summary>
    public class BlazingPowder : WikiCovering
    {
        protected override int Rarity => ItemRarityID.Pink;
        protected override int SellCopper => 33200;

        public override void UpdateAccessory(Player player, bool hideVisual) => player.GetModPlayer<WikiCoveringPlayer>().BlazingPowder = true;
    }

    /// <summary>
    /// Wiki : BFB.txt — « The Brutal Floating Behemoth » : quadruple saut, puis 5 s de vol une fois les sauts épuisés ; pas de
    /// dégâts de chute, sauts plus hauts et plus rapides, saut automatique ; compte comme des ailes.
    /// [technique] Sans image d'ailes portée : le vol est une poussée vers le haut tant qu'on tient Saut (5 s, rendus au sol).
    /// </summary>
    public class BFB : WikiCovering
    {
        protected override int Rarity => ItemRarityID.Lime;
        protected override int SellCopper => 288000;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetJumpState(ExtraJump.CloudInABottle).Enable();
            player.GetJumpState(ExtraJump.BlizzardInABottle).Enable();
            player.GetJumpState(ExtraJump.SandstormInABottle).Enable();
            player.noFallDmg = true;
            player.jumpBoost = true;
            player.jumpSpeedBoost += 1.5f;
            player.autoJump = true;
            player.GetModPlayer<WikiCoveringPlayer>().BFB = true;
        }
    }

    /// <summary>
    /// Wiki : Magna Core.txt — +30 % de dégâts et +25 % de critique magiques, -100 % de mana dépensé, +250 de mana max, ramasse
    /// les étoiles de mana de plus loin ; boire une potion de mana lance 8 sphères de granite chercheuses (20 000 dégâts,
    /// 5 ennemis chacune), 20 s d'attente.
    /// </summary>
    public class MagnaCore : WikiCovering
    {
        protected override int Rarity => ModContent.RarityType<CalamityMod.Rarities.BurnishedAuric>();
        protected override int SellCopper => 350000;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Magic) += 0.3f;
            player.GetCritChance(DamageClass.Magic) += 25f;
            player.manaCost -= 1f;
            player.statManaMax2 += 250;
            player.manaMagnet = true;
            player.GetModPlayer<WikiCoveringPlayer>().MagnaCore = true;
        }
    }

    /// <summary>
    /// Wiki : Quiver of Madness.txt — +30 % de dégâts et +25 % de critique à distance, effets de la Deadshot Brooch (munitions
    /// non consommées, 15 défense, +2 de régénération, +35 % de vitesse de minage) ; 4 portails de soufre tournent autour du
    /// joueur : un projectile à distance qui en traverse un fait x3 et cherche les ennemis.
    /// [choix] Portails à 90 px ; les cibles en forme de crâne (+500 %) ne sont pas faites.
    /// </summary>
    public class QuiverofMadness : WikiCovering
    {
        protected override int Rarity => ModContent.RarityType<CalamityMod.Rarities.BurnishedAuric>();
        protected override int SellCopper => 350000;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Ranged) += 0.3f;
            player.GetCritChance(DamageClass.Ranged) += 25f;
            player.statDefense += 15;
            player.lifeRegen += 4;
            player.pickSpeed -= 0.35f;
            player.GetModPlayer<WikiCoveringPlayer>().QuiverOfMadness = true;
        }
    }

    /// <summary>
    /// Wiki : Socratic Sapphire.txt — chance de critique et réduction des dégâts divisées par deux ; 10 % de chances qu'un
    /// critique devienne un « coup sauvage » (x5), jusqu'à 75 % avec les bonus de critique et de réduction des dégâts ; durée
    /// des debuffs -70 %. [choix] Chance = 10 % + (bonus de critique + réduction des dégâts x 100) / 2, au plus 75 %.
    /// </summary>
    public class SocraticSapphire : WikiCovering
    {
        protected override int Rarity => ItemRarityID.Purple;
        protected override int SellCopper => 250000;

        public override void UpdateAccessory(Player player, bool hideVisual) => player.GetModPlayer<WikiCoveringPlayer>().SocraticSapphire = true;
    }

    /// <summary>
    /// Wiki : Cobalt Night Circuit.txt — « mech de nuit » : +40 PV max, +10 défense, flotte jusqu'à 5 blocs au-dessus du sol,
    /// +50 % de vitesse, aura de 5 blocs (120 dégâts par seconde). [choix] Flotter = tenir Saut en l'air à moins de 5 blocs
    /// du sol ; aura : 60 dégâts toutes les 30 images. [technique] Pas d'apparence de mech (pas d'image).
    /// </summary>
    public class CobaltNightCircuit : WikiCovering
    {
        protected override int Rarity => ItemRarityID.Pink;
        protected override int SellCopper => 300000;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statLifeMax2 += 40;
            player.statDefense += 10;
            player.moveSpeed += 0.5f;
            player.GetModPlayer<WikiCoveringPlayer>().NightCircuit = true;
        }
    }

    /// <summary>Wiki : Palladium Light Circuit.txt — « mech de lumière » : +100 PV max, +30 défense, +25 % de hauteur de saut, -50 % de vitesse.</summary>
    public class PalladiumLightCircuit : WikiCovering
    {
        protected override int Rarity => ItemRarityID.Pink;
        protected override int SellCopper => 300000;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statLifeMax2 += 100;
            player.statDefense += 30;
            player.jumpSpeedBoost += 1.25f;
            player.moveSpeed -= 0.5f;
        }
    }

    /// <summary>Wiki : Terraformance.txt — +25 % de mana maximum (de jour comme de nuit : amélioration de la Nightform).</summary>
    public class Terraformance : WikiCovering
    {
        protected override int Rarity => ItemRarityID.LightPurple;
        protected override int SellCopper => 50000;

        public override void UpdateAccessory(Player player, bool hideVisual) => player.statManaMax2 += player.statManaMax2 / 4;
    }

    /// <summary>
    /// Wiki : Earth Destruction Core.txt — après chaque coup reçu, pendant 5 s : +150 % de vitesse de minage, +50 % de vitesse
    /// de déplacement, -25 % de réduction des dégâts. Amélioration de la Babil Destruction Core.
    /// </summary>
    public class EarthDestructionCore : WikiCovering
    {
        protected override int Rarity => ItemRarityID.Yellow;
        protected override int SellCopper => 400000;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var mp = player.GetModPlayer<BabilCorePlayer>();
            mp.Equipped = true;
            if (mp.Timer > 0)
            {
                player.pickSpeed -= 0.6f;
                player.moveSpeed += 0.5f;
                player.endurance -= 0.25f;
            }
        }
    }

    /// <summary>Effets du lot P.</summary>
    public class WikiCoveringPlayer : ModPlayer
    {
        public bool BlazingPowder, BFB, MagnaCore, QuiverOfMadness, SocraticSapphire, NightCircuit;
        private int rangedShots, bfbFlight, magnaCooldown;

        public override void ResetEffects() => BlazingPowder = BFB = MagnaCore = QuiverOfMadness = SocraticSapphire = NightCircuit = false;

        public override bool CanConsumeAmmo(Item weapon, Item ammo) => !QuiverOfMadness;

        public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (BlazingPowder && item.CountsAsClass(DamageClass.Ranged) && ++rangedShots % 2 == 0)
            {
                int dmg = (int)Player.GetTotalDamage(DamageClass.Ranged).ApplyTo(25);
                Projectile.NewProjectile(source, position, (Main.MouseWorld - position).SafeNormalize(Vector2.UnitX) * 12f, ModContent.ProjectileType<AstralFireball>(), dmg, 2f, Player.whoAmI);
            }
            return true;
        }

        public override void PostUpdateEquips()
        {
            if (SocraticSapphire)
            {
                // Critique et réduction des dégâts divisés par deux
                Player.GetCritChance(DamageClass.Generic) -= Player.GetTotalCritChance(DamageClass.Generic) / 2f;
                Player.endurance /= 2f;
            }
            if (QuiverOfMadness && Player.whoAmI == Main.myPlayer && Player.ownedProjectileCounts[ModContent.ProjectileType<MadnessPortal>()] < 4)
                for (int i = 0; i < 4; i++)
                    Projectile.NewProjectile(Player.GetSource_Accessory(Player.HeldItem), Player.Center, Vector2.Zero, ModContent.ProjectileType<MadnessPortal>(), 0, 0f, Player.whoAmI, i);
        }

        public override void PostUpdateBuffs()
        {
            if (!SocraticSapphire)
                return;
            // Durée des debuffs -70 % : le temps passe 3,33 fois plus vite
            for (int i = 0; i < Player.MaxBuffs; i++)
            {
                int type = Player.buffType[i];
                if (type > 0 && Main.debuff[type] && !BuffID.Sets.NurseCannotRemoveDebuff[type] && Player.buffTime[i] > 3)
                    Player.buffTime[i] -= Main.GameUpdateCount % 3 == 0 ? 3 : 2;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (!SocraticSapphire)
                return;
            float chance = 0.1f + (Player.GetTotalCritChance(DamageClass.Generic) + Player.endurance * 100f) / 200f;
            if (Main.rand.NextFloat() < Math.Min(chance, 0.75f))
                modifiers.CritDamage *= 5f / 2f;
        }

        public override void PostUpdate()
        {
            if (magnaCooldown > 0)
                magnaCooldown--;
            if (BFB)
            {
                bool grounded = Player.velocity.Y == 0f;
                if (grounded)
                    bfbFlight = 300;
                else if (Player.controlJump && Player.jump == 0 && bfbFlight > 0 && !Player.GetJumpState(ExtraJump.CloudInABottle).Available && !Player.GetJumpState(ExtraJump.BlizzardInABottle).Available && !Player.GetJumpState(ExtraJump.SandstormInABottle).Available)
                {
                    bfbFlight--;
                    Player.velocity.Y = Math.Max(Player.velocity.Y - 0.6f, -6f);
                }
            }
            if (NightCircuit)
            {
                // Flotte : tenir Saut en l'air à moins de 5 blocs du sol
                if (Player.controlJump && Player.velocity.Y > 0f && GroundWithin(5))
                    Player.velocity.Y = 0f;
                if (Player.whoAmI == Main.myPlayer && Main.GameUpdateCount % 30 == 0)
                    Projectile.NewProjectile(Player.GetSource_Accessory(Player.HeldItem), Player.Center, Vector2.Zero, ModContent.ProjectileType<CircuitAura>(), 60, 0f, Player.whoAmI);
            }
        }

        private bool GroundWithin(int blocks)
        {
            Point feet = Player.Bottom.ToTileCoordinates();
            for (int y = feet.Y; y <= feet.Y + blocks; y++)
                if (WorldGen.InWorld(feet.X, y) && WorldGen.SolidTile(feet.X, y))
                    return true;
            return false;
        }

        /// <summary>Magna Core : boire une potion de mana (soin de mana) lance 8 sphères de granite (20 s d'attente).</summary>
        public void ManaPotionUsed()
        {
            if (!MagnaCore || magnaCooldown > 0 || Player.whoAmI != Main.myPlayer)
                return;
            magnaCooldown = 1200;
            SoundEngine.PlaySound(SoundID.Item92, Player.Center);
            int damage = (int)Player.GetTotalDamage(DamageClass.Magic).ApplyTo(20000);
            for (int i = 0; i < 8; i++)
                Projectile.NewProjectile(Player.GetSource_Accessory(Player.HeldItem), Player.Center, (MathHelper.TwoPi * i / 8f).ToRotationVector2() * 10f,
                    ModContent.ProjectileType<GraniteSphere>(), damage, 4f, Player.whoAmI);
        }
    }

    /// <summary>Potions de mana bues avec la Magna Core.</summary>
    public class MagnaCoreManaPotion : GlobalItem
    {
        public override void OnConsumeItem(Item item, Player player)
        {
            if (item.healMana > 0)
                player.GetModPlayer<WikiCoveringPlayer>().ManaPotionUsed();
        }
    }

    /// <summary>Boule de feu astrale de la Blazing Powder : Hellfire 5 s.</summary>
    public class AstralFireball : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool() ? DustID.PinkTorch : DustID.BlueTorch, Vector2.Zero, 100, default, 1.4f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.OnFire3, 300);
    }

    /// <summary>Sphère de granite de la Magna Core : cherche les ennemis, 5 ennemis.</summary>
    public class GraniteSphere : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.ShadowBeamFriendly;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 5;
            Projectile.timeLeft = 240;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            if (++Projectile.ai[0] > 15f)
                CalamityUtils.HomeInOnNPC(Projectile, true, 1200f, 18f, 10f);
            if (!Main.dedServ)
                for (int i = 0; i < 2; i++)
                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8f, 8f), DustID.Granite, Vector2.Zero, 100, default, 1.6f).noGravity = true;
        }

        public override bool PreDraw(ref Microsoft.Xna.Framework.Color lightColor) => false;
    }

    /// <summary>Portail de soufre de la Quiver of Madness : tourne autour du joueur ; un projectile à distance qui le traverse fait x3 et cherche les ennemis.</summary>
    public class MadnessPortal : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 36;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 2;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!owner.active || owner.dead || !owner.GetModPlayer<WikiCoveringPlayer>().QuiverOfMadness)
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 2;
            float angle = Main.GlobalTimeWrappedHourly * 1.5f + MathHelper.PiOver2 * Projectile.ai[0];
            Projectile.Center = owner.Center + angle.ToRotationVector2() * 90f;
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(16f, 16f), DustID.RedTorch, Vector2.Zero, 100, default, 1.2f).noGravity = true;
            if (Projectile.owner != Main.myPlayer)
                return;
            foreach (Projectile p in Main.ActiveProjectiles)
                if (p.owner == Projectile.owner && p.friendly && p.CountsAsClass(DamageClass.Ranged) && p.Hitbox.Intersects(Projectile.Hitbox))
                {
                    var g = p.GetGlobalProjectile<MadnessEmpowered>();
                    if (g.Empowered)
                        continue;
                    g.Empowered = true;
                    p.damage *= 3;
                    p.netUpdate = true;
                }
        }
    }

    /// <summary>Projectile passé par un portail de la Quiver of Madness : cherche fortement les ennemis.</summary>
    public class MadnessEmpowered : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public bool Empowered;

        public override void AI(Projectile projectile)
        {
            if (Empowered && projectile.velocity != Vector2.Zero)
                CalamityUtils.HomeInOnNPC(projectile, !projectile.tileCollide, 900f, Math.Max(projectile.velocity.Length(), 10f), 8f);
        }
    }

    /// <summary>Aura du Cobalt Night Circuit : 5 blocs, frappe une fois.</summary>
    public class CircuitAura : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 160;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.Center = Main.player[Projectile.owner].Center;
            if (!Main.dedServ && Projectile.timeLeft == 3)
                for (int i = 0; i < 12; i++)
                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(80f, 80f), DustID.Electric, Vector2.Zero, 100, default, 0.8f).noGravity = true;
        }
    }
}
