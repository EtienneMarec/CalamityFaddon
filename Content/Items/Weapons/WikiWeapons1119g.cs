using System;
using CalamityFandom.Content.Projectiles.Weapons1119g;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
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

// Lot J (19/11) : armes inventées par le wiki, sprites et valeurs des pages. Plusieurs tombent de boss inventés pas encore
// faits (Acidsighter, Ceasing Void, Sigmund, Grand Thunder Cnidrion, XB-10 Nortazzo) ou s'achètent au Collector (PNJ pas
// encore fait) : TODO avec eux.

namespace CalamityFandom.Content.Items.Weapons.Ranged
{
    /// <summary>Wiki : Acid Bow.txt — deux flèches ; les Wooden Arrows deviennent des flèches sulfuriques (Irradiated 4 s). Butin de l'Acidsighter.</summary>
    public class AcidBow : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 12;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 29;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item5;
            Item.width = 20;
            Item.height = 34;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 20);
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 17f;
            Item.useAmmo = AmmoID.Arrow;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (type == ProjectileID.WoodenArrowFriendly)
                type = ModContent.ProjectileType<SulphuricArrow>();
            for (int i = -1; i <= 1; i += 2)
                Projectile.NewProjectile(source, position, velocity.RotatedBy(0.04f * i), type, damage, knockback, player.whoAmI);
            return false;
        }
    }

    /// <summary>
    /// Wiki : Butcher (weapon).txt — tire lentement et mal au début ; la cadence et la précision montent tant qu'on tient la
    /// gâchette, jusqu'à 600 % de la cadence de départ. [choix] Pleine cadence en 4 s de tir continu, dispersion de 0,25 à 0 rad.
    /// </summary>
    public class Butcher : ModItem
    {
        private int held;

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 18;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item11;
            Item.width = 72;
            Item.height = 30;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 2, silver: 40);
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10f, 0f);

        public override void HoldItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer && !player.controlUseItem)
                held = 0;
        }

        private float Progress => Math.Min(held / 240f, 1f);

        public override float UseSpeedMultiplier(Player player) => 1f + 5f * Progress;

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            held += (int)Math.Max(1, Item.useTime / UseSpeedMultiplier(player));
            velocity = velocity.RotatedByRandom(0.25f * (1f - Progress));
        }
    }

    /// <summary>Wiki : Talon Burst.txt — les Wooden Arrows deviennent des Storm Bolts très rapides qui touchent deux fois la même cible. Butin du Grand Thunder Cnidrion.</summary>
    public class TalonBurst : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 5;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item5;
            Item.width = 28;
            Item.height = 54;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 20);
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 10f;
            Item.useAmmo = AmmoID.Arrow;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (type == ProjectileID.WoodenArrowFriendly)
            {
                type = ModContent.ProjectileType<StormBolt>();
                velocity *= 2f;
            }
        }
    }

    /// <summary>
    /// Wiki : Titanslayer.txt — flot de 8 traits spectraux chercheurs ; une utilisation sur 3 les surcharge : dégâts x6,
    /// taille x2, +20 % de critique, et ils éclatent en âmes (25 %). Butin de XB-10 Nortazzo.
    /// [choix] « +500 % de dégâts » = x6 ; 4 âmes par trait.
    /// </summary>
    public class Titanslayer : ModItem
    {
        private int uses;

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 2223;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 5;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item5;
            Item.width = 46;
            Item.height = 98;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
            Item.value = Item.sellPrice(gold: 30);
            Item.shoot = ModContent.ProjectileType<SpectralBolt>();
            Item.shootSpeed = 14f;
            Item.useAmmo = AmmoID.Arrow;
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.itemAnimation == player.itemAnimationMax - 1)
                uses++;
            return null;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool super = uses % 3 == 0;
            int p = Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.1f), ModContent.ProjectileType<SpectralBolt>(), super ? damage * 6 : damage,
                knockback, player.whoAmI, super ? 1f : 0f);
            if (super && Main.projectile.IndexInRange(p))
                Main.projectile[p].CritChance += 20;
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Magic
{
    /// <summary>Wiki : Aspid Blaster.txt — trois orbes infectés orange qui s'écartent et tombent. Butin des Primal Aspids.</summary>
    public class AspidBlaster : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 43;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 14;
            Item.useTime = Item.useAnimation = 11;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item95;
            Item.width = 56;
            Item.height = 30;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(silver: 80);
            Item.shoot = ModContent.ProjectileType<InfectedOrb>();
            Item.shootSpeed = 12f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = -1; i <= 1; i++)
                Projectile.NewProjectile(source, position, velocity.RotatedBy(0.18f * i), type, damage, knockback, player.whoAmI);
            return false;
        }
    }

    /// <summary>Wiki : Caustic Claw.txt — deux vrilles vertes qui tournent au hasard, comme la Shadowflame Hex Doll (Irradiated). Butin de l'Acidsighter.</summary>
    public class CausticClaw : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 22;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 6;
            Item.useTime = Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item103;
            Item.width = 36;
            Item.height = 40;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 2, silver: 40);
            Item.shoot = ProjectileID.ShadowFlame;
            Item.shootSpeed = 10f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Vrilles de la Shadowflame Hex Doll (ai[0], ai[1] : courbure au hasard), en vert et avec Irradiated
            for (int i = 0; i < 2; i++)
            {
                Vector2 v = velocity.RotatedByRandom(0.3f);
                int p = Projectile.NewProjectile(source, position, v, type, damage, knockback, player.whoAmI, Main.rand.NextFloat(-0.5f, 0.5f) * 0.05f, Main.rand.NextFloat(-0.5f, 0.5f) * 0.05f);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].GetGlobalProjectile<CausticTendril>().Caustic = true;
            }
            return false;
        }
    }

    /// <summary>Vrilles de la Caustic Claw : Irradiated 3 s, poussière verte.</summary>
    public class CausticTendril : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public bool Caustic;

        public override void AI(Projectile projectile)
        {
            if (Caustic && !Main.dedServ && Main.rand.NextBool(2))
                Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.GreenTorch, 0f, 0f, 100, default, 1.2f).noGravity = true;
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Caustic)
                target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Steel Blade.txt — les dégâts de contact du dernier ennemi touché par la lame s'ajoutent à ceux de l'arme ; tire
    /// des projectiles d'acier qui suivent les mouvements du joueur. Butin du Ceasing Void.
    /// [choix] Projectile : couteau vanilla gris qui reçoit la vitesse du joueur.
    /// </summary>
    public class SteelBlade : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 34;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 7f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = Item.height = 72;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(silver: 2);
            Item.shoot = ModContent.ProjectileType<SteelShard>();
            Item.shootSpeed = 5f;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage) => damage.Flat += player.GetModPlayer<SteelBladePlayer>().Stolen;

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) => player.GetModPlayer<SteelBladePlayer>().Stolen = target.damage;
    }

    public class SteelBladePlayer : ModPlayer
    {
        /// <summary>Dégâts de contact du dernier ennemi touché par la Steel Blade.</summary>
        public int Stolen;
    }

    /// <summary>
    /// Wiki : Swift Sword.txt — plus de dégâts quand on va vite (jusqu'à +80 %, et +30 % de critique) ; Swiftness 10 s quand
    /// il tue. S'achète au Collector avec son Blueprint. [choix] Maximum atteint à 40 mph (vitesse 20).
    /// </summary>
    public class SwiftSword : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 32;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = Item.height = 48;
            Item.rare = ItemRarityID.Cyan;
            Item.value = Item.sellPrice(silver: 10);
        }

        private static float SpeedRatio(Player player) => Math.Min(player.velocity.Length() / 20f, 1f);

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage) => damage *= 1f + 0.8f * SpeedRatio(player);

        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 30f * SpeedRatio(player);

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.life <= 0)
                player.AddBuff(BuffID.Swiftness, 600);
        }
    }

    /// <summary>
    /// Wiki : Calamity Sword.txt — rayon d'épée ; s'il touche, il éclate en fragments qui cherchent les ennemis après un
    /// moment (25 % des dégâts) ; On Fire! et Brimstone Flames 5 s ; jusqu'à x3 dans le Brimstone Crag. Amélioration du Swordsplosion.
    /// [choix] x3 dans tout le Brimstone Crag ; 5 fragments, chercheurs après 40 images.
    /// </summary>
    public class CalamitySword : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 582;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 27;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 10f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item60;
            Item.width = 92;
            Item.height = 90;
            Item.rare = ModContent.RarityType<CosmicPurple>();
            Item.value = Item.sellPrice(platinum: 1, gold: 1, silver: 20);
            Item.shoot = ModContent.ProjectileType<CalamitySwordBeam>();
            Item.shootSpeed = 12f;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            if (player.Calamity().ZoneCalamity)
                damage *= 3f;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 300);
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 300);
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Rogue
{
    /// <summary>
    /// Wiki : Steel Kunai.txt — flot rapide de kunais à courte portée, qui traversent les blocs, sans gravité, deviennent
    /// transparents et tournent un peu vers les ennemis proches ; frappe furtive : chercheurs, ils explosent. Butin de Sigmund.
    /// </summary>
    public class SteelKunai : RogueWeapon
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 12;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.useTime = Item.useAnimation = 9;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = Item.noUseGraphic = true;
            Item.knockBack = 5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = 24;
            Item.height = 54;
            Item.rare = ItemRarityID.Lime;
            Item.value = Item.sellPrice(silver: 24);
            Item.shoot = ModContent.ProjectileType<SteelKunaiProj>();
            Item.shootSpeed = 28f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (player.Calamity().StealthStrikeAvailable() && Main.projectile.IndexInRange(p))
            {
                Main.projectile[p].Calamity().stealthStrike = true;
                Main.projectile[p].timeLeft = 120;
            }
            return false;
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Weapons1119g
{
    /// <summary>Flèche sulfurique de l'Acid Bow : Irradiated 4 s.</summary>
    public class SulphuricArrow : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.VenomArrow;

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.WoodenArrowFriendly);
            AIType = ProjectileID.WoodenArrowFriendly;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(170, 230, 80, 220);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Irradiated>(), 240);
    }

    /// <summary>Storm Bolt du Talon Burst : très rapide, touche deux fois la même cible.</summary>
    public class StormBolt : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Ranged/StormBolt";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 4;
            Projectile.scale = 0.6f;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Lighting.AddLight(Projectile.Center, 0.3f, 0.5f, 0.9f);
        }
    }

    /// <summary>Trait spectral du Titanslayer : chercheur ; ai[0] = 1 surchargé (taille x2, éclate en 4 âmes à 25 %).</summary>
    public class SpectralBolt : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.SpectreWrath;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 240;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 1f && Projectile.scale < 2f)
            {
                Projectile.scale = 2f;
                Projectile.Resize(28, 28);
            }
            CalamityUtils.HomeInOnNPC(Projectile, true, 800f, 16f, 15f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.DungeonSpirit, Vector2.Zero, 100, default, Projectile.scale).noGravity = true;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(150, 220, 255, 150);

        public override void OnKill(int timeLeft)
        {
            if (Projectile.ai[0] != 1f || Projectile.owner != Main.myPlayer)
                return;
            for (int i = 0; i < 4; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2CircularEdge(6f, 6f), ProjectileID.SpectreWrath,
                    Projectile.damage / 4, 0f, Projectile.owner);
        }
    }

    /// <summary>Orbe infecté de l'Aspid Blaster : orange, soumis à la gravité.</summary>
    public class InfectedOrb : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.BallofFire;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.15f, 12f);
            Projectile.rotation += 0.2f;
            if (!Main.dedServ && Main.rand.NextBool(2))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.OrangeTorch, 0f, 0f, 100, default, 1.1f).noGravity = true;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 170, 60, 200);
    }

    /// <summary>Éclat d'acier de la Steel Blade : reçoit la vitesse du joueur à chaque image (il suit ses mouvements).</summary>
    public class SteelShard : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.ThrowingKnife;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 60;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            Projectile.position += owner.velocity;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, Color.LightSteelBlue, 0.5f);
    }

    /// <summary>Rayon de la Calamity Sword : éclate en 5 fragments au contact d'un ennemi.</summary>
    public class CalamitySwordBeam : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Melee/CalamitySwordBeam";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            Lighting.AddLight(Projectile.Center, 0.9f, 0.2f, 0.2f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 300);
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 300);
            if (Projectile.owner != Main.myPlayer)
                return;
            SoundEngine.PlaySound(SoundID.Item14 with { Volume = 0.5f }, Projectile.Center);
            for (int i = 0; i < 5; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2CircularEdge(7f, 7f), ModContent.ProjectileType<CalamitySwordFragment>(),
                    Projectile.damage / 4, 2f, Projectile.owner);
        }
    }

    /// <summary>Fragment de la Calamity Sword : se disperse, puis cherche les ennemis après 40 images (Brimstone Flames 5 s).</summary>
    public class CalamitySwordFragment : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Melee/CalamitySwordBeam";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.scale = 0.4f;
        }

        public override void AI()
        {
            if (++Projectile.ai[0] < 40f)
                Projectile.velocity *= 0.95f;
            else
                CalamityUtils.HomeInOnNPC(Projectile, true, 700f, 14f, 15f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 300);
    }

    /// <summary>Kunai d'acier : court (15 images), traverse les blocs, s'efface, tourne un peu vers les ennemis ; furtif : chercheur, explose.</summary>
    public class SteelKunaiProj : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Rogue/SteelKunai";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = 1;
            Projectile.timeLeft = 15;
            Projectile.tileCollide = false;
            Projectile.scale = 0.6f;
        }

        public override void AI()
        {
            bool stealth = Projectile.Calamity().stealthStrike;
            CalamityUtils.HomeInOnNPC(Projectile, true, stealth ? 700f : 200f, Projectile.velocity.Length(), stealth ? 10f : 40f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (!stealth)
                Projectile.Opacity = Projectile.timeLeft / 15f;
        }

        public override void OnKill(int timeLeft)
        {
            if (!Projectile.Calamity().stealthStrike)
                return;
            SoundEngine.PlaySound(SoundID.Item14 with { Volume = 0.5f }, Projectile.Center);
            Projectile.ExpandHitboxBy(70);
            Projectile.maxPenetrate = Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();
            if (!Main.dedServ)
                for (int i = 0; i < 15; i++)
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 1.5f).velocity *= 2f;
        }
    }
}
