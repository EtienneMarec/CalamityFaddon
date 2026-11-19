using System;
using CalamityFandom.Content.Projectiles.Weapons1119n;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

// Lot R (19/11) : armes du wiki fabricables à l'Ancient Manipulator (recettes du wiki), pages d'addon comprises
// (section 11). « Embers of Duality » = God Slayer Inferno de Calamity, comme pour la Celestial Nuke.

namespace CalamityFandom.Content.Items.Weapons.Melee
{
    /// <summary>Wiki : Unloxcalibur.txt — une épée ordinaire, sans projectile.</summary>
    public class Unloxcalibur : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 100;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 13f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = 88;
            Item.height = 92;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 40);
        }
    }

    /// <summary>Wiki : Aspect of the End.txt — épée (infobulle « hi pixel », pas d'autre effet décrit).</summary>
    public class AspectoftheEnd : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 175;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = Item.height = 44;
            Item.rare = ItemRarityID.Cyan;
            Item.value = Item.sellPrice(gold: 30);
        }
    }

    /// <summary>
    /// Wiki : Gamma Ray Burst.txt — yoyo qui ignore l'invincibilité des ennemis : il rebondit vite et fait bien plus que ses
    /// dégâts affichés ; Embers of Duality 3 s. [choix] 4 images entre deux coups sur un même ennemi.
    /// </summary>
    public class GammaRayBurst : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.Yoyo[Type] = true;
            ItemID.Sets.GamepadExtraRange[Type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 75;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = Item.noUseGraphic = true;
            Item.channel = true;
            Item.knockBack = 7.5f;
            Item.UseSound = SoundID.Item1;
            Item.width = 30;
            Item.height = 26;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.value = Item.sellPrice(gold: 24);
            Item.shoot = ModContent.ProjectileType<GammaRayYoyo>();
            Item.shootSpeed = 16f;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Magic
{
    /// <summary>
    /// Wiki : Dark Harvest Staff.txt — citrouilles sombres qui rebondissent sur les blocs, percent un ennemi et se brisent au
    /// suivant en 2 à 4 morceaux soumis à la gravité (20 %). Amélioration du Harvest Staff.
    /// </summary>
    public class DarkHarvestStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 96;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item20;
            Item.width = 60;
            Item.height = 48;
            Item.rare = ItemRarityID.Cyan;
            Item.value = Item.sellPrice(gold: 2);
            Item.shoot = ModContent.ProjectileType<DarkPumpkin>();
            Item.shootSpeed = 9f;
        }
    }

    /// <summary>
    /// Wiki : Prominence.txt — une tempête céleste autour du curseur : lasers de nébuleuse et lances divines chercheuses
    /// (Embers of Duality 1 s). Butin de The Starborn Moth, recette du wiki. [choix] 4 lasers et 2 lances depuis 500 px autour.
    /// </summary>
    public class Prominence : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 64;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 16;
            Item.useTime = Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item105;
            Item.width = 80;
            Item.height = 74;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.value = Item.sellPrice(gold: 24);
            Item.shoot = ModContent.ProjectileType<CelestialStormShot>();
            Item.shootSpeed = 20f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 mouse = Main.MouseWorld;
            for (int i = 0; i < 6; i++)
            {
                Vector2 from = mouse + Main.rand.NextVector2CircularEdge(1f, 1f) * 500f;
                Projectile.NewProjectile(source, from, (mouse - from).SafeNormalize(Vector2.UnitY) * 20f, type, damage, knockback, player.whoAmI, i < 4 ? 0f : 1f);
            }
            return false;
        }
    }

    /// <summary>
    /// Wiki : Chrononimbus.txt — 5 lames célestes ; chaque coup appelle de petites flammes de hors de l'écran (comme Fate's
    /// Reveal). Lames : Astral Infection 5 s et Embers of Duality. Butin de The Starborn Moth, recette du wiki.
    /// [choix] 2 flammes par coup, à 30 %.
    /// </summary>
    public class Chrononimbus : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 57;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 30;
            Item.useTime = Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4.5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item60;
            Item.width = 62;
            Item.height = 76;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.value = Item.sellPrice(gold: 24);
            Item.shoot = ModContent.ProjectileType<CelestialBlade>();
            Item.shootSpeed = 16f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = -2; i <= 2; i++)
                Projectile.NewProjectile(source, position, velocity.RotatedBy(0.1f * i), type, damage, knockback, player.whoAmI);
            return false;
        }
    }

    /// <summary>
    /// Wiki : Crius Prime.txt — clic gauche : petit laser ; clic droit maintenu : charge un gros laser rouge (mana dépensé
    /// pendant la charge) qui part en relâchant et rebondit sur les blocs ; Holy Flames 1,5 s. Amélioration de l'Abyss
    /// Shocker et du Plasma Rifle. [choix] Charge de 2 s maximum (x1 à x5 dégâts), 2 mana par 10 images, 5 rebonds.
    /// </summary>
    public class CriusPrime : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 255;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 15;
            Item.useTime = Item.useAnimation = 32;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6.5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item12;
            Item.width = 74;
            Item.height = 30;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.value = Item.sellPrice(gold: 28);
            Item.shoot = ModContent.ProjectileType<CriusLaser>();
            Item.shootSpeed = 15f;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10f, 0f);

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            bool alt = player.altFunctionUse == 2;
            Item.channel = alt;
            Item.mana = alt ? 0 : 15;
            Item.shoot = alt ? ModContent.ProjectileType<CriusCharge>() : ModContent.ProjectileType<CriusLaser>();
            Item.UseSound = alt ? null : SoundID.Item12;
            return !alt || player.ownedProjectileCounts[ModContent.ProjectileType<CriusCharge>()] < 1;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Ranged
{
    /// <summary>
    /// Wiki : Bolide Launcher.txt — les Wooden Arrows deviennent des bolides qui percent 4 ennemis et traversent les blocs ;
    /// tire aussi deux Galactica Comets (God Slayer Inferno). Butin de The Starborn Moth, recette du wiki.
    /// [choix] Comètes à 250 % (la page est coupée là), bolides : Embers of Duality 2 s.
    /// </summary>
    public class BolideLauncher : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 22;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item5;
            Item.width = 40;
            Item.height = 78;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.value = Item.sellPrice(gold: 24);
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 32f;
            Item.useAmmo = AmmoID.Arrow;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (type == ProjectileID.WoodenArrowFriendly)
                type = ModContent.ProjectileType<Bolide>();
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            for (int i = -1; i <= 1; i += 2)
                Projectile.NewProjectile(source, position, velocity.RotatedBy(0.12f * i) * 0.6f, ModContent.ProjectileType<GalacticaComet>(), (int)(damage * 2.5f), knockback, player.whoAmI);
            return false;
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Weapons1119n
{
    /// <summary>Yoyo du Gamma Ray Burst : ignore l'invincibilité (immunité locale courte), Embers of Duality 3 s.</summary>
    public class GammaRayYoyo : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.Kraken;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Type] = -1f;
            ProjectileID.Sets.YoyosMaximumRange[Type] = 320f;
            ProjectileID.Sets.YoyosTopSpeed[Type] = 16f;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.aiStyle = ProjAIStyleID.Yoyo;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 4;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(220, 160, 255, 220);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 180);
    }

    /// <summary>Citrouille sombre : rebondit sur les blocs, perce un ennemi, se brise au second en 2 à 4 morceaux (20 %).</summary>
    public class DarkPumpkin : ModProjectile
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.Pumpkin;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 240;
        }

        public override void AI()
        {
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.15f, 12f);
            Projectile.rotation += 0.15f;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(90, 50, 120);

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X * 0.8f;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y * 0.8f;
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);
            if (Projectile.owner != Main.myPlayer)
                return;
            for (int i = 0; i < Main.rand.Next(2, 5); i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-6f, -2f)), ModContent.ProjectileType<PumpkinChunk>(),
                    Math.Max(1, Projectile.damage / 5), 0f, Projectile.owner);
        }
    }

    /// <summary>Morceau de citrouille : gravité.</summary>
    public class PumpkinChunk : ModProjectile
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.Pumpkin;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 90;
            Projectile.scale = 0.4f;
        }

        public override void AI()
        {
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.25f, 12f);
            Projectile.rotation += 0.2f;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(90, 50, 120);
    }

    /// <summary>Tir de la Prominence : ai[0] = 0 laser de nébuleuse (droit), 1 lance divine chercheuse ; Embers of Duality 1 s.</summary>
    public class CelestialStormShot : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 90;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 1f && ++Projectile.localAI[0] > 15f)
                CalamityUtils.HomeInOnNPC(Projectile, true, 700f, 18f, 12f);
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, Projectile.ai[0] == 1f ? DustID.GoldFlame : DustID.PinkTorch, Vector2.Zero, 100, default, 1.3f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 60);
    }

    /// <summary>Lame céleste de la Chrononimbus : Astral Infection 5 s, Embers of Duality ; un coup appelle 2 flammes de hors de l'écran (30 %).</summary>
    public class CelestialBlade : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 90;
        }

        public override void AI()
        {
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.PurpleTorch, Vector2.Zero, 100, default, 1.4f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 300);
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 120);
            if (Projectile.owner != Main.myPlayer)
                return;
            for (int i = 0; i < 2; i++)
            {
                Vector2 from = target.Center + Main.rand.NextVector2CircularEdge(1f, 1f) * 800f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), from, (target.Center - from).SafeNormalize(Vector2.UnitY) * 16f, ModContent.ProjectileType<CelestialStormShot>(),
                    (int)(Projectile.damage * 0.3f), 0f, Projectile.owner, 1f);
            }
        }
    }

    /// <summary>Petit laser de la Crius Prime (Holy Flames 1,5 s).</summary>
    public class CriusLaser : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.PurpleLaser;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.extraUpdates = 2;
        }

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        public override Color? GetAlpha(Color lightColor) => new Color(255, 120, 120, 0);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<HolyFlames>(), 90);
    }

    /// <summary>Charge de la Crius Prime : tenue au canon ; dépense du mana ; en relâchant, gros laser rouge qui rebondit (5 fois).</summary>
    public class CriusCharge : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 2;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            Vector2 aim = (Main.MouseWorld - owner.MountedCenter).SafeNormalize(Vector2.UnitX * owner.direction);
            if (owner.channel && !owner.dead && owner.HeldItem.type == ModContent.ItemType<Items.Weapons.Magic.CriusPrime>())
            {
                Projectile.timeLeft = 2;
                owner.itemTime = owner.itemAnimation = 2;
                owner.ChangeDir(aim.X > 0 ? 1 : -1);
                owner.itemRotation = (aim * owner.direction).ToRotation();
                Projectile.Center = owner.MountedCenter + aim * 50f;
                if (Projectile.ai[0] < 120f)
                {
                    Projectile.ai[0]++;
                    if (Projectile.ai[0] % 10f == 0f && Projectile.owner == Main.myPlayer && !owner.CheckMana(2, true))
                        owner.channel = false;
                }
                if (!Main.dedServ)
                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(10f, 10f), DustID.RedTorch, Vector2.Zero, 100, default, 1f + Projectile.ai[0] / 60f).noGravity = true;
                return;
            }
            if (Projectile.owner == Main.myPlayer)
            {
                SoundEngine.PlaySound(SoundID.Item72, Projectile.Center);
                float mult = 1f + 4f * Projectile.ai[0] / 120f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), owner.MountedCenter, aim * 16f, ModContent.ProjectileType<CriusBigLaser>(), (int)(Projectile.damage * mult), Projectile.knockBack, Projectile.owner);
            }
            Projectile.Kill();
        }
    }

    /// <summary>Gros laser rouge de la Crius Prime : rebondit 5 fois sur les blocs, perce, Holy Flames 1,5 s.</summary>
    public class CriusBigLaser : CriusLaser
    {
        private int bounces;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 14;
            Projectile.penetrate = 5;
            Projectile.scale = 2f;
            Projectile.timeLeft = 240;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (++bounces > 5)
                return true;
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }
    }

    /// <summary>Bolide du Bolide Launcher : traverse les blocs, perce 4 ennemis, Embers of Duality 2 s.</summary>
    public class Bolide : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 4;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.arrow = true;
        }

        public override void AI()
        {
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.SolarFlare, -Projectile.velocity * 0.1f, 100, default, 1.3f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 120);
    }

    /// <summary>Galactica Comet : God Slayer Inferno 3 s.</summary>
    public class GalacticaComet : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Ranged/GalacticaComet";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.scale = 0.6f;
        }

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 180);
    }
}
