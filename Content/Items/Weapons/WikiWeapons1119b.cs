using System;
using System.Collections.Generic;
using CalamityFandom.Content.Projectiles.Weapons1119b;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

// Armes inventées par le wiki (19/11, suite) : sprites et valeurs des pages, recettes du wiki (WikiRecipes).

namespace CalamityFandom.Content.Items.Weapons.Ranged
{
    /// <summary>
    /// Wiki : Twentloon.txt — tire très vite vingt harpons sans gravité qui vont un peu loin puis reviennent (amélioration
    /// du Triploon porté). [choix] Les vingt harpons partent d'affilée (une utilisation de 20 images, un par image).
    /// </summary>
    public class Twentloon : ModItem
    {
        protected virtual int Harpoons => 20;

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 200;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 58;
            Item.height = 60;
            Item.scale = 0.6f;
            Item.useAnimation = Harpoons;
            Item.useTime = 1;
            Item.reuseDelay = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7.5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item10;
            Item.rare = ModContent.RarityType<CosmicPurple>();
            Item.value = Item.sellPrice(gold: 12);
            Item.shoot = ModContent.ProjectileType<WikiHarpoon>();
            Item.shootSpeed = 20f;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10f, 0f);

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) =>
            velocity = velocity.RotatedByRandom(0.15f);
    }

    /// <summary>Wiki : Twenty-Twloon.txt — vingt-deux harpons (amélioration du Twentloon).</summary>
    public class TwentyTwloon : Twentloon
    {
        protected override int Harpoons => 22;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 220;
            Item.width = 74;
            Item.height = 31;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
        }
    }

    /// <summary>
    /// Wiki : Terra Bow.txt — trois flèches à la fois ; les Wooden Arrows deviennent trois traits d'énergie parallèles sans
    /// gravité qui glissent sur les blocs (les Lunar Bolts du Lunarian Bow de Calamity).
    /// </summary>
    public class TerraBow : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 39;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 34;
            Item.height = 74;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.75f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item5;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 16);
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 10f;
            Item.useAmmo = AmmoID.Arrow;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool wooden = type == ProjectileID.WoodenArrowFriendly;
            Vector2 side = velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2) * 8f;
            for (int i = -1; i <= 1; i++)
            {
                if (wooden)
                    Projectile.NewProjectile(source, position + side * i, velocity, ModContent.ProjectileType<CalamityMod.Projectiles.Ranged.LunarBolt>(), damage, knockback, player.whoAmI);
                else
                    Projectile.NewProjectile(source, position, velocity.RotatedBy(0.08f * i), type, damage, knockback, player.whoAmI);
            }
            return false;
        }
    }

    /// <summary>
    /// Wiki : Elemental Bow.txt — fait pleuvoir 7 flèches du ciel vers le curseur ; les Wooden Arrows deviennent des traits
    /// arc-en-ciel chercheurs (rouge, orange, jaune, vert, bleu, indigo, violet ; Elemental Mix 2 s).
    /// </summary>
    public class ElementalBow : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 58;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 30;
            Item.height = 50;
            Item.useTime = Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5.5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item5;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.sellPrice(gold: 22);
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.Arrow;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool wooden = type == ProjectileID.WoodenArrowFriendly;
            Vector2 mouse = Main.MouseWorld;
            var colors = new List<int> { 0, 1, 2, 3, 4, 5, 6 };
            for (int i = 0; i < 7; i++)
            {
                Vector2 spawn = new(mouse.X + Main.rand.Next(-200, 201), player.Center.Y - 600f - Main.rand.Next(100));
                Vector2 v = (mouse - spawn).SafeNormalize(Vector2.UnitY) * Item.shootSpeed * 1.5f;
                int c = colors[Main.rand.Next(colors.Count)];
                colors.Remove(c);
                if (wooden)
                    Projectile.NewProjectile(source, spawn, v, ModContent.ProjectileType<RainbowBolt>(), damage, knockback, player.whoAmI, c);
                else
                {
                    int p = Projectile.NewProjectile(source, spawn, v, type, damage, knockback, player.whoAmI);
                    if (Main.projectile.IndexInRange(p))
                        Main.projectile[p].tileCollide = false;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// Wiki : Rods from God.txt — fait pleuvoir des fléchettes solaires vers le curseur (traversent 3 ennemis) ; clic droit
    /// maintenu : un soleil grandit au bout de l'arme ; au bout de 10 s, en relâchant, il part et explose sur tout l'écran
    /// (On Fire!). [choix] Relâché avant 10 s, le soleil disparaît.
    /// </summary>
    public class RodsfromGod : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 323;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 70;
            Item.height = 64;
            Item.scale = 0.7f;
            Item.useTime = 5;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item92;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
            Item.value = Item.sellPrice(gold: 50);
            Item.shoot = ModContent.ProjectileType<SunDart>();
            Item.shootSpeed = 22f;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.channel = true;
                Item.useTime = Item.useAnimation = 20;
                return player.ownedProjectileCounts[ModContent.ProjectileType<ChargingSun>()] == 0;
            }
            Item.channel = false;
            Item.useTime = 5;
            Item.useAnimation = 25;
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<ChargingSun>(), damage * 5, knockback, player.whoAmI);
                return false;
            }
            Vector2 mouse = Main.MouseWorld;
            Vector2 spawn = new(mouse.X + Main.rand.Next(-150, 151), player.Center.Y - 650f);
            Projectile.NewProjectile(source, spawn, (mouse - spawn).SafeNormalize(Vector2.UnitY) * Item.shootSpeed, type, damage, knockback, player.whoAmI);
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Grand Gar.txt — comme la Majestic Gar (défense -1 par coup, soigne de 4 PV quand elle est à 0), avec en plus :
    /// une grosse explosion à chaque coup (50 % des dégâts de base, 10 % de moins par coup), des traits arc-en-ciel sur les
    /// ennemis à moins de la moitié de leur vie, et des orbes de soin à la mort des ennemis.
    /// </summary>
    public class GrandGar : ModItem
    {
        private int explosionsThisSwing;

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 150;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 21;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 8.5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = Item.height = 90;
            Item.scale = 1.4f;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(gold: 20);
        }

        public override bool? UseItem(Player player)
        {
            explosionsThisSwing = 0;
            return null;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.defense > 0)
                target.defense--;
            else if (!player.moonLeech)
            {
                player.statLife = Math.Min(player.statLife + 4, player.statLifeMax2);
                player.HealEffect(4);
            }
            if (player.whoAmI != Main.myPlayer)
                return;
            var source = player.GetSource_ItemUse(Item);
            float mult = Math.Max(0.1f, 0.5f - 0.1f * explosionsThisSwing++);
            Projectile.NewProjectile(source, target.Center, Vector2.Zero, ModContent.ProjectileType<GarExplosion>(), (int)(Item.damage * mult), 0f, player.whoAmI);
            if (target.life < target.lifeMax / 2)
                for (int i = 0; i < 3; i++)
                    Projectile.NewProjectile(source, target.Center, Main.rand.NextVector2CircularEdge(9f, 9f), ModContent.ProjectileType<RainbowBolt>(),
                        Item.damage / 3, 0f, player.whoAmI, Main.rand.Next(7), 1f);
            if (target.life <= 0)
                Projectile.NewProjectile(source, target.Center, -Vector2.UnitY * 3f, ModContent.ProjectileType<GarHealOrb>(), 0, 0f, player.whoAmI);
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Typeless
{
    /// <summary>
    /// Wiki : Deeepscale Bomb.txt — explosif qui flotte, explose au bout de 5 s (blesse aussi le joueur) et ne casse que
    /// quelques blocs naturels de l'Abyss.
    /// </summary>
    public class DeeepscaleBomb : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 99;

        public override void SetDefaults()
        {
            Item.damage = 500;
            Item.useTime = Item.useAnimation = 55;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = Item.noUseGraphic = true;
            Item.consumable = true;
            Item.maxStack = 9999;
            Item.UseSound = SoundID.Item1;
            Item.width = 26;
            Item.height = 30;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(platinum: 3, silver: 7);
            Item.shoot = ModContent.ProjectileType<DeeepscaleBombProj>();
            Item.shootSpeed = 5f;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Rogue
{
    /// <summary>
    /// Wiki : Stellar Kunai.txt — trois kunaïs sans gravité qui cherchent un peu les ennemis ; au bout d'une seconde, ils se
    /// chargent (plus rapides, recherche agressive, explosion à l'impact) ; frappe furtive : huit kunaïs déjà chargés.
    /// </summary>
    public class StellarKunai : RogueWeapon
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 99;

        public override void SetDefaults()
        {
            Item.damage = 120;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.useTime = Item.useAnimation = 9;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = Item.noUseGraphic = true;
            Item.consumable = true;
            Item.maxStack = 9999;
            Item.knockBack = 2f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = Item.height = 38;
            Item.rare = ItemRarityID.Red;
            Item.value = 24 * 5;
            Item.shoot = ModContent.ProjectileType<StellarKunaiProj>();
            Item.shootSpeed = 22f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool stealth = player.Calamity().StealthStrikeAvailable();
            int count = stealth ? 8 : 3;
            for (int i = 0; i < count; i++)
            {
                Vector2 v = velocity.RotatedBy(MathHelper.Lerp(-0.25f, 0.25f, count == 1 ? 0.5f : i / (count - 1f)));
                int p = Projectile.NewProjectile(source, position, v, type, damage, knockback, player.whoAmI, stealth ? 60f : 0f);
                if (stealth && Main.projectile.IndexInRange(p))
                    Main.projectile[p].Calamity().stealthStrike = true;
            }
            return false;
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Weapons1119b
{
    /// <summary>Harpon du Twentloon (celui du Triploon porté).</summary>
    public class WikiHarpoon : Legacy.Projectiles.Ranged.MultiHarpoonProj
    {
        public override string Texture => "CalamityFandom/Content/Legacy/Projectiles/Ranged/TriploonProj";
    }

    /// <summary>Trait arc-en-ciel : ai[0] = couleur (0 rouge … 6 violet), cherche les ennemis ; Elemental Mix 2 s.</summary>
    public class RainbowBolt : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private static readonly Color[] Colors =
        {
            new(255, 40, 40), new(255, 140, 0), new(255, 230, 40), new(60, 230, 60), new(40, 120, 255), new(75, 0, 180), new(170, 60, 255),
        };

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            // ai[1] = 1 : tiré par la Grand Gar (mêlée)
            if (Projectile.ai[1] == 1f)
                Projectile.DamageType = DamageClass.Melee;
            if (++Projectile.localAI[0] > 15f)
                CalamityUtils.HomeInOnNPC(Projectile, true, 600f, 14f, 15f);
            if (!Main.dedServ)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.RainbowMk2, Vector2.Zero, 100, Colors[(int)Projectile.ai[0] % 7], 1.2f);
                d.noGravity = true;
            }
            Lighting.AddLight(Projectile.Center, Colors[(int)Projectile.ai[0] % 7].ToVector3() * 0.5f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<ElementalMix>(), 120);
    }

    /// <summary>Fléchette solaire des Rods from God : traverse 3 ennemis et les blocs.</summary>
    public class SunDart : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.PoisonDart;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Lighting.AddLight(Projectile.Center, 1f, 0.7f, 0.2f);
            if (Main.rand.NextBool(2))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SolarFlare, 0f, 0f, 100, default, 0.8f).noGravity = true;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 200, 80, 200);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.OnFire, 180);
    }

    /// <summary>Soleil des Rods from God : grandit tant que le clic droit est tenu ; chargé au bout de 10 s, part au relâchement.</summary>
    public class ChargingSun : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private const int ChargeTime = 600;

        private ref float Charge => ref Projectile.ai[0];
        private bool Launched => Projectile.ai[1] == 1f;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        public override bool? CanDamage() => Launched ? null : false;

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!Launched)
            {
                if (!owner.channel || owner.dead)
                {
                    if (Charge >= ChargeTime && Projectile.owner == Main.myPlayer)
                    {
                        // Lancé vers le curseur
                        Projectile.ai[1] = 1f;
                        Projectile.velocity = Projectile.SafeDirectionTo(Main.MouseWorld) * 10f;
                        Projectile.timeLeft = 180;
                        Projectile.tileCollide = true;
                        Projectile.netUpdate = true;
                        SoundEngine.PlaySound(SoundID.Item73, Projectile.Center);
                    }
                    else
                        Projectile.Kill();
                    return;
                }
                Charge = Math.Min(Charge + 1, ChargeTime);
                Projectile.timeLeft = 2;
                Vector2 tip = owner.RotatedRelativePoint(owner.MountedCenter) + (Main.MouseWorld - owner.Center).SafeNormalize(Vector2.UnitX) * 50f;
                Projectile.Center = tip;
                owner.itemTime = owner.itemAnimation = 2;
                owner.heldProj = Projectile.whoAmI;
                if (Charge == ChargeTime && !Main.dedServ)
                    SoundEngine.PlaySound(SoundID.Item29, Projectile.Center);
            }
            float size = 20f + Charge / ChargeTime * 140f;
            Projectile.Resize((int)size, (int)size);
            if (!Main.dedServ)
                for (int i = 0; i < 1 + (int)(Charge / 120); i++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(size / 2f, size / 2f), DustID.SolarFlare, Vector2.Zero, 100, default, 1.5f);
                    d.noGravity = true;
                }
            Lighting.AddLight(Projectile.Center, 1f, 0.8f, 0.3f);
        }

        public override void OnKill(int timeLeft)
        {
            if (!Launched)
                return;
            // Explosion de la taille de l'écran (On Fire! 5 s)
            SoundEngine.PlaySound(SoundID.Item62, Projectile.Center);
            Projectile.ExpandHitboxBy(1600);
            Projectile.maxPenetrate = Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            for (int i = 0; i < Main.maxNPCs; i++)
                Projectile.localNPCImmunity[i] = 0;
            Projectile.Damage();
            for (int i = 0; i < 100; i++)
                Dust.NewDustPerfect(Projectile.Center, DustID.SolarFlare, Main.rand.NextVector2Circular(40f, 40f), 100, default, 3f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.OnFire, 300);
    }

    /// <summary>Explosion de la Grand Gar (comme l'Inferno Fork) : 120 px.</summary>
    public class GarExplosion : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 120;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 5;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.Item74 with { Volume = 0.6f }, Projectile.Center);
            if (Main.dedServ)
                return;
            for (int i = 0; i < 30; i++)
                Dust.NewDustPerfect(Projectile.Center, DustID.InfernoFork, Main.rand.NextVector2Circular(8f, 8f), 100, default, 2f).noGravity = true;
        }
    }

    /// <summary>Orbe de soin de la Grand Gar : flotte vers le joueur et le soigne de 5 PV.</summary>
    public class GarHealOrb : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(owner.Center) * 10f, 0.06f);
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.HealingPlus, Vector2.Zero, 100, default, 1.1f).noGravity = true;
            if (Projectile.Hitbox.Intersects(owner.Hitbox))
            {
                if (Projectile.owner == Main.myPlayer && !owner.moonLeech)
                {
                    owner.statLife = Math.Min(owner.statLife + 5, owner.statLifeMax2);
                    owner.HealEffect(5);
                }
                Projectile.Kill();
            }
        }
    }

    /// <summary>
    /// Bombe Deeepscale : flotte dans l'eau, explose au bout de 5 s (blesse aussi le joueur), ne casse que les blocs naturels
    /// de l'Abyss dans un petit rayon (3 blocs).
    /// </summary>
    public class DeeepscaleBombProj : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Typeless/DeeepscaleBomb";

        private static readonly string[] AbyssTiles = { "AbyssGravel", "Voidstone", "PyreMantle", "PyreMantleMolten", "PlantyMush" };

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
        }

        public override bool? CanDamage() => Projectile.timeLeft <= 3 ? null : false;

        public override void AI()
        {
            // Flotte : remonte dans l'eau, tombe à l'air libre
            Projectile.velocity.Y = Projectile.wet ? Math.Max(Projectile.velocity.Y - 0.15f, -2f) : Math.Min(Projectile.velocity.Y + 0.2f, 10f);
            Projectile.velocity.X *= 0.98f;
            Projectile.rotation += Projectile.velocity.X * 0.05f;
            if (Main.rand.NextBool(3))
                Dust.NewDustDirect(Projectile.Top, 4, 4, DustID.Torch, 0f, -1f).noGravity = true;
            if (Projectile.timeLeft == 3)
            {
                Projectile.ExpandHitboxBy(200);
                Projectile.hostile = true;
                Projectile.velocity = Vector2.Zero;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X * 0.5f;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y * 0.3f;
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            for (int i = 0; i < 40; i++)
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 2f).velocity *= 2.5f;
            if (Projectile.owner != Main.myPlayer)
                return;
            var types = new HashSet<int>();
            foreach (string name in AbyssTiles)
                if (ModContent.TryFind<ModTile>("CalamityMod", name, out var tile))
                    types.Add(tile.Type);
            Point center = Projectile.Center.ToTileCoordinates();
            for (int x = -3; x <= 3; x++)
                for (int y = -3; y <= 3; y++)
                {
                    int i = center.X + x, j = center.Y + y;
                    if (x * x + y * y > 9 || !WorldGen.InWorld(i, j))
                        continue;
                    Tile t = Main.tile[i, j];
                    if (t.HasTile && types.Contains(t.TileType))
                    {
                        WorldGen.KillTile(i, j);
                        if (Main.netMode != NetmodeID.SinglePlayer)
                            NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, i, j);
                    }
                }
        }
    }

    /// <summary>Kunaï stellaire : cherche un peu, se charge au bout de 60 images (ai[0]) puis cherche fort et explose.</summary>
    public class StellarKunaiProj : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Rogue/StellarKunaiProj";

        private bool Charged => Projectile.ai[0] >= 60f;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = 1;
            Projectile.timeLeft = 240;
        }

        public override void AI()
        {
            if (++Projectile.ai[0] == 60f)
            {
                Projectile.velocity *= 1.4f;
                SoundEngine.PlaySound(SoundID.Item9 with { Volume = 0.4f }, Projectile.Center);
            }
            CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, Charged ? 800f : 300f, Charged ? 20f : 12f, Charged ? 10f : 30f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Charged && Main.rand.NextBool(2))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Clentaminator_Cyan, 0f, 0f, 100, default, 0.9f).noGravity = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Charged ? ModContent.Request<Texture2D>("CalamityFandom/Content/Projectiles/Rogue/StellarKunaiCharged").Value : TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            if (!Charged)
                return;
            SoundEngine.PlaySound(SoundID.Item14 with { Volume = 0.4f }, Projectile.Center);
            Projectile.ExpandHitboxBy(64);
            Projectile.maxPenetrate = Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.damage /= 2;
            Projectile.Damage();
            for (int i = 0; i < 12; i++)
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Clentaminator_Cyan, 0f, 0f, 100, default, 1.3f).velocity *= 2f;
        }
    }
}
