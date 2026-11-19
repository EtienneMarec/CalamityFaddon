using System;
using CalamityFandom.Content.Projectiles.Weapons1119s;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

// Lot W (19/11) : objets du wiki fabricables à la dernière étape de l'Aquatic Hypercomputer (recettes du wiki). Raretés
// « C3 », « C4 » inventées : 0 (règle de la section 3). « Ancient Astral Blight » (debuff sans effet défini) = Astral
// Infection de Calamity.

namespace CalamityFandom.Content.Items.Weapons.Melee
{
    /// <summary>Wiki : Astralence.txt — épée qui tire des tranchants (200 %, Ancient Astral Blight 4 s).</summary>
    public class Astralence : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 1322;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5.5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = Item.height = 72;
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(platinum: 5);
            Item.shoot = ModContent.ProjectileType<AstralEdge>();
            Item.shootSpeed = 9f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage * 2, knockback, player.whoAmI);
            return false;
        }
    }

    /// <summary>Wiki : Magnic Circularity.txt — yoyo qui attire les ennemis vers lui (Ancient Astral Blight 4 s). [choix] Non-boss à 300 px.</summary>
    public class MagnicCircularity : ModItem
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
            Item.damage = 900;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.useTime = Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = Item.noUseGraphic = true;
            Item.channel = true;
            Item.knockBack = 5.5f;
            Item.UseSound = SoundID.Item1;
            Item.width = Item.height = 72;
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(platinum: 5);
            Item.shoot = ModContent.ProjectileType<MagnicYoyo>();
            Item.shootSpeed = 16f;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Ranged
{
    /// <summary>Wiki : Decreator.txt — balles très rapides, gros dégâts et recul ; 1 % de chances qu'une balle fasse x10.</summary>
    public class Decreator : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 3135;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 37;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 15f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item40;
            Item.width = Item.height = 72;
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(platinum: 5);
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 42f;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (Main.rand.NextBool(100))
                damage *= 10;
        }
    }

    /// <summary>Wiki : Technicate.txt — 75 % de ne pas consommer ; chaque balle fait naître 2 balles à côté d'elle toutes les 0,5 s.</summary>
    public class Technicate : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 120;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 3;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item11;
            Item.width = 114;
            Item.height = 64;
            Item.scale = 0.6f;
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(platinum: 2);
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 18f;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.Next(100) >= 75;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (Main.projectile.IndexInRange(p))
                Main.projectile[p].GetGlobalProjectile<CloningBullet>().Active = true;
            return false;
        }
    }

    /// <summary>
    /// Balle du Technicate : toutes les 30 images, crée 2 balles du même type à côté d'elle. [choix] Les copies ne se copient
    /// pas (sinon des milliers de balles).
    /// </summary>
    public class CloningBullet : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public bool Active;
        private int timer;

        public override void AI(Projectile projectile)
        {
            if (!Active || projectile.owner != Main.myPlayer || ++timer % 30 != 0)
                return;
            Vector2 side = projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2) * 10f;
            for (int i = -1; i <= 1; i += 2)
                Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center + side * i, projectile.velocity, projectile.type, projectile.damage, projectile.knockBack, projectile.owner);
        }
    }

    /// <summary>
    /// Wiki : Ifflamator.txt — lance-flammes jaune dont les flammes tournent vite autour du curseur et disparaissent à 10 blocs
    /// de lui ; Ancient Astral Blight 5 s ; consomme toujours du gel. [choix] Flammes de 3 s, rayon qui grandit de 3 px par image.
    /// </summary>
    public class Ifflamator : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 120;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 2;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item34;
            Item.width = Item.height = 72;
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(platinum: 5);
            Item.shoot = ModContent.ProjectileType<OrbitingFlame>();
            Item.shootSpeed = 30f;
            Item.useAmmo = AmmoID.Gel;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<OrbitingFlame>(), damage, knockback, player.whoAmI, Main.rand.NextFloat(MathHelper.TwoPi));
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Magic
{
    /// <summary>Wiki : Technomagica.txt — arcs électriques rapides (Electrified 5 s). [choix] 3 arcs par utilisation vers le curseur.</summary>
    public class Technomagica : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 840;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 50;
            Item.useTime = Item.useAnimation = 32;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item94;
            Item.width = 40;
            Item.height = 46;
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(platinum: 2);
            Item.shoot = ModContent.ProjectileType<ElectricArc>();
            Item.shootSpeed = 18f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = -1; i <= 1; i++)
                Projectile.NewProjectile(source, position, velocity.RotatedBy(0.15f * i), type, damage, knockback, player.whoAmI);
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Tools
{
    /// <summary>Wiki : Reactive Pickaxe.txt — pioche 350 %, très rapide. [choix] « Reactive Flames » (debuff sans page) : non fait.</summary>
    public class ReactivePickaxe : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 200;
            Item.DamageType = DamageClass.Melee;
            Item.width = 50;
            Item.height = 48;
            Item.useTime = Item.useAnimation = 4;
            Item.pick = 350;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.tileBoost = 3;
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(platinum: 10);
        }
    }

    /// <summary>Wiki : Supreme Onyx Drill.txt — foreuse 400 %, très rapide.</summary>
    public class SupremeOnyxDrill : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.IsDrill[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 500;
            Item.DamageType = DamageClass.Melee;
            Item.width = 88;
            Item.height = 42;
            Item.useTime = 1;
            Item.useAnimation = 10;
            Item.channel = true;
            Item.noUseGraphic = Item.noMelee = true;
            Item.pick = 400;
            Item.tileBoost = 3;
            Item.knockBack = 2f;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item23;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(platinum: 5);
            Item.shoot = ModContent.ProjectileType<SupremeDrillProj>();
            Item.shootSpeed = 40f;
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Weapons1119s
{
    /// <summary>Tranchant de l'Astralence : Ancient Astral Blight (Astral Infection) 4 s.</summary>
    public class AstralEdge : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 80;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            if (!Main.dedServ)
                for (int i = 0; i < 2; i++)
                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8f, 8f), Main.rand.NextBool() ? DustID.PinkTorch : DustID.BlueTorch, Vector2.Zero, 100, default, 1.4f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 240);
    }

    /// <summary>Yoyo de la Magnic Circularity : attire les ennemis (non-boss) à 300 px.</summary>
    public class MagnicYoyo : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.Terrarian;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Type] = -1f;
            ProjectileID.Sets.YoyosMaximumRange[Type] = 360f;
            ProjectileID.Sets.YoyosTopSpeed[Type] = 18f;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.aiStyle = ProjAIStyleID.Yoyo;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = -1;
        }

        public override void PostAI()
        {
            foreach (NPC npc in Main.ActiveNPCs)
                if (npc.CanBeChasedBy() && !npc.boss && npc.knockBackResist > 0f && npc.Distance(Projectile.Center) < 300f)
                    npc.velocity = Vector2.Lerp(npc.velocity, npc.SafeDirectionTo(Projectile.Center) * 6f, 0.08f);
        }

        public override Color? GetAlpha(Color lightColor) => new Color(180, 120, 255, 220);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 240);
    }

    /// <summary>Flamme de l'Ifflamator : tourne autour du curseur en s'éloignant ; disparaît à 10 blocs (Astral Infection 5 s).</summary>
    public class OrbitingFlame : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Projectile.localAI[0]++;
            float radius = 20f + 3f * Projectile.localAI[0];
            if (radius > 160f)
            {
                Projectile.Kill();
                return;
            }
            Vector2 center = Projectile.owner == Main.myPlayer ? Main.MouseWorld : Projectile.Center;
            Projectile.ai[0] += 0.25f;
            Projectile.Center = center + Projectile.ai[0].ToRotationVector2() * radius;
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.YellowTorch, Vector2.Zero, 100, default, 1.4f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 300);
    }

    /// <summary>Arc électrique de la Technomagica : zigzague, Electrified 5 s.</summary>
    public class ElectricArc : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            if (++Projectile.localAI[0] % 6 == 0)
                Projectile.velocity = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f));
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.Electric, Vector2.Zero, 100, default, 1f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Electrified, 300);
    }

    /// <summary>Foreuse tenue du Supreme Onyx Drill.</summary>
    public class SupremeDrillProj : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Tools/SupremeOnyxDrill";

        public override void SetStaticDefaults() => ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            Projectile.ownerHitCheck = true;
            Projectile.aiStyle = ProjAIStyleID.Drill;
            Projectile.DamageType = DamageClass.Melee;
        }
    }
}
