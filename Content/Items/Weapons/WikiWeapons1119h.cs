using System;
using CalamityFandom.Content.Projectiles.Weapons1119h;
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

// Lot K (19/11) : armes du wiki qui sont des ingrédients de recettes déjà faites (pages d'addon comprises, décisions
// section 11). Sprites et valeurs des pages, recettes du wiki (WikiRecipes), butin par WikiLoot.

namespace CalamityFandom.Content.Items.Weapons.Ranged
{
    /// <summary>Wiki : Arngren.txt — cadence très élevée, dispersion de 45° au hasard ; 80 % de ne pas consommer. Amélioration du Clock Gatlignum.</summary>
    public class Arngren : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 50;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 3;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item11;
            Item.width = 64;
            Item.height = 34;
            Item.rare = ItemRarityID.Cyan;
            Item.value = Item.sellPrice(gold: 8);
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 26f;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-8f, 0f);

        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.Next(100) >= 80;

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) =>
            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(22.5f));
    }

    /// <summary>
    /// Wiki : Flounder Mortar.txt — les Musket Balls deviennent un jet d'acide rapide qui traverse un ennemi et disparaît au
    /// second (Sulphuric Poisoning 12 s). Butin des Flounders.
    /// </summary>
    public class FlounderMortar : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 43;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 11;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item13;
            Item.width = 60;
            Item.height = 38;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 1);
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 11f;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (type == ProjectileID.Bullet)
                type = ModContent.ProjectileType<AcidStream>();
        }
    }

    /// <summary>
    /// Wiki : Megaskeet.txt — tir rapide ; un tir sur deux devient un jet de lumière solaire qui traverse tout et ne consomme
    /// rien ; tous les 17 tirs, un Sunskater chercheur (prédictif) qui éclate en 2-3 éclats de braise (Holy Flames 3 à 6 s) ;
    /// 50 % de ne pas consommer. Amélioration du SDOMG.
    /// </summary>
    public class Megaskeet : ModItem
    {
        private int shots;

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 129;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 3;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item11;
            Item.width = 78;
            Item.height = 20;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
            Item.value = Item.sellPrice(gold: 90);
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 22f;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10f, 0f);

        public override bool CanConsumeAmmo(Item ammo, Player player) => shots % 2 == 0 && shots % 17 != 0 && Main.rand.NextBool();

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            shots++;
            if (shots % 17 == 0)
                Projectile.NewProjectile(source, position, velocity * 0.6f, ModContent.ProjectileType<SunskaterProj>(), damage * 2, knockback, player.whoAmI);
            else if (shots % 2 == 0)
                Projectile.NewProjectile(source, position, velocity * 1.3f, ModContent.ProjectileType<HardSunlight>(), damage, knockback, player.whoAmI);
            else
                Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.04f), type, damage, knockback, player.whoAmI);
            return false;
        }
    }

    /// <summary>
    /// Wiki : Confection.txt — lentement, des morceaux de gâteau qui éclatent en globes de glaçage soumis à la gravité ; les
    /// globes percent à l'infini, durent 10 s, 40 au plus à la fois.
    /// </summary>
    public class Confection : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 220;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 150;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item5;
            Item.width = 46;
            Item.height = 98;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.value = Item.sellPrice(gold: 10);
            Item.shoot = ModContent.ProjectileType<CakeArrow>();
            Item.shootSpeed = 10f;
            Item.useAmmo = AmmoID.Arrow;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) =>
            type = ModContent.ProjectileType<CakeArrow>();
    }
}

namespace CalamityFandom.Content.Items.Weapons.Magic
{
    /// <summary>Wiki : Dual Cane.txt — un orbe noir et un orbe blanc qui avancent en hélice (assez large).</summary>
    public class DualCane : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 47;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 12;
            Item.useTime = Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item43;
            Item.width = Item.height = 56;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 2);
            Item.shoot = ModContent.ProjectileType<HelixOrb>();
            Item.shootSpeed = 13f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 2; i++)
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, i);
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Rogue
{
    /// <summary>
    /// Wiki : Ichor Dagger.txt — dague non consommable qui rebondit : traverse 2 ennemis ou rebondit 2 fois, disparaît au
    /// troisième impact ; très soumise à la gravité ; Ichor 2 s ; frappe furtive : arrosée de jets d'ichor. Butin des Ichor Stickers.
    /// </summary>
    public class IchorDagger : RogueWeapon
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 34;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.useTime = Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = Item.noUseGraphic = true;
            Item.knockBack = 4.5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = 14;
            Item.height = 40;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(gold: 7, silver: 20);
            Item.shoot = ModContent.ProjectileType<IchorDaggerProj>();
            Item.shootSpeed = 12f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (player.Calamity().StealthStrikeAvailable() && Main.projectile.IndexInRange(p))
                Main.projectile[p].Calamity().stealthStrike = true;
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Tools
{
    /// <summary>
    /// Wiki : Heaven Piercer.txt — foreuse-hache (280 dégâts, pioche 275 %, hache 180 %) qui casse presque tous les blocs
    /// touchés ; clic droit : laser en hélice qui casse aussitôt les blocs qu'il traverse.
    /// [choix] Le laser casse ce que la pioche peut casser (pas les coffres), 40 blocs de portée.
    /// </summary>
    public class HeavenPiercer : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.IsDrill[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 280;
            Item.DamageType = DamageClass.Melee;
            Item.width = 100;
            Item.height = 60;
            Item.useTime = 2;
            Item.useAnimation = 24;
            Item.channel = true;
            Item.noUseGraphic = Item.noMelee = true;
            Item.pick = 275;
            Item.axe = 36;
            Item.tileBoost = 2;
            Item.knockBack = 1f;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item23;
            Item.autoReuse = true;
            Item.rare = ModContent.RarityType<CosmicPurple>();
            Item.value = Item.sellPrice(gold: 20);
            Item.shoot = ModContent.ProjectileType<HeavenPiercerDrill>();
            Item.shootSpeed = 40f;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            bool alt = player.altFunctionUse == 2;
            Item.channel = !alt;
            Item.useTime = alt ? 24 : 2;
            Item.shoot = alt ? ModContent.ProjectileType<HelixLaser>() : ModContent.ProjectileType<HeavenPiercerDrill>();
            Item.shootSpeed = alt ? 16f : 40f;
            Item.UseSound = alt ? SoundID.Item33 : SoundID.Item23;
            return true;
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Weapons1119h
{
    /// <summary>Jet d'acide du Flounder Mortar : rapide, traverse un ennemi (Sulphuric Poisoning 12 s).</summary>
    public class AcidStream : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 60;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            if (!Main.dedServ)
                for (int i = 0; i < 2; i++)
                    Dust.NewDustPerfect(Projectile.Center - Projectile.velocity * i * 0.5f, DustID.GreenTorch, Vector2.Zero, 100, new Color(170, 230, 60), 1.3f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 720);
    }

    /// <summary>Jet de lumière solaire du Megaskeet : traverse tout.</summary>
    public class HardSunlight : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 90;
            Projectile.extraUpdates = 2;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 1f, 0.8f, 0.3f);
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.SolarFlare, Vector2.Zero, 100, default, 1.1f).noGravity = true;
        }
    }

    /// <summary>Sunskater du Megaskeet : vise la position future de l'ennemi, éclate en 2-3 éclats de braise (Holy Flames 3 à 6 s).</summary>
    public class SunskaterProj : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Ranged/SunskaterProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 240;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            NPC target = Projectile.Center.ClosestNPCAt(1000f);
            if (target != null)
            {
                Vector2 aim = target.Center + target.velocity * (Projectile.Distance(target.Center) / 16f);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(aim) * 16f, 0.08f);
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.spriteDirection = Projectile.velocity.X > 0 ? 1 : -1;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14 with { Volume = 0.5f }, Projectile.Center);
            if (Projectile.owner != Main.myPlayer)
                return;
            for (int i = 0; i < Main.rand.Next(2, 4); i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2CircularEdge(8f, 8f), ModContent.ProjectileType<CinderShard>(),
                    Projectile.damage / 3, 0f, Projectile.owner);
        }
    }

    /// <summary>Éclat de braise : Holy Flames 3 à 6 s.</summary>
    public class CinderShard : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.CrystalShard;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 90;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.1f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 180, 80, 200);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<HolyFlames>(), Main.rand.Next(180, 361));
    }

    /// <summary>Morceau de gâteau de la Confection : éclate en 5 globes de glaçage au contact.</summary>
    public class CakeArrow : ModProjectile
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.SliceOfCake;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.1f, 14f);
            Projectile.rotation += 0.1f;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);
            if (Projectile.owner != Main.myPlayer)
                return;
            for (int i = 0; i < 5; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-6f, -2f)),
                    ModContent.ProjectileType<FrostingGlob>(), Projectile.damage / 3, 0f, Projectile.owner);
        }
    }

    /// <summary>Globe de glaçage : gravité, perce à l'infini, 10 s ; 40 au plus par joueur (le plus ancien disparaît).</summary>
    public class FrostingGlob : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.SlimeGun;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void OnSpawn(IEntitySource source)
        {
            // 40 au plus : le plus ancien disparaît
            int count = 0, oldest = -1, oldestTime = int.MaxValue;
            foreach (Projectile p in Main.ActiveProjectiles)
                if (p.type == Type && p.owner == Projectile.owner)
                {
                    count++;
                    if (p.timeLeft < oldestTime)
                    {
                        oldestTime = p.timeLeft;
                        oldest = p.whoAmI;
                    }
                }
            if (count > 40 && oldest >= 0)
                Main.projectile[oldest].Kill();
        }

        public override void AI()
        {
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.2f, 12f);
            Projectile.velocity.X *= 0.98f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = Vector2.Zero;
            return false;
        }
    }

    /// <summary>Orbe de la Dual Cane : ai[0] = 0 noir, 1 blanc ; les deux s'enroulent en hélice autour de la ligne de tir.</summary>
    public class HelixOrb : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.ShadowBeamFriendly;

        private Vector2 origin;
        private Vector2 direction;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            origin = Projectile.Center;
            direction = Projectile.velocity;
        }

        public override void AI()
        {
            Projectile.localAI[0]++;
            float side = Projectile.ai[0] == 0f ? 1f : -1f;
            Vector2 along = origin + direction * Projectile.localAI[0];
            Vector2 normal = direction.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2);
            Projectile.Center = along + normal * side * 36f * (float)Math.Sin(Projectile.localAI[0] * 0.15f);
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, Projectile.ai[0] == 0f ? DustID.Shadowflame : DustID.WhiteTorch, Vector2.Zero, 100,
                    Projectile.ai[0] == 0f ? Color.Black : Color.White, 1.4f).noGravity = true;
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }

    /// <summary>Dague d'ichor : gravité forte, traverse 2 ennemis ou rebondit 2 fois, Ichor 2 s ; furtive : jets d'ichor autour d'elle.</summary>
    public class IchorDaggerProj : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Rogue/IchorDagger";

        private int bounces;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = 3;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.35f, 16f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.Calamity().stealthStrike && Projectile.owner == Main.myPlayer && Projectile.timeLeft % 8 == 0)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Circular(4f, 4f) - Vector2.UnitY * 3f,
                    ProjectileID.GoldenShowerFriendly, Projectile.damage / 3, 0f, Projectile.owner);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (++bounces > 2)
                return true;
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X * 0.8f;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y * 0.8f;
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Ichor, 120);
    }

    /// <summary>Foreuse tenue du Heaven Piercer.</summary>
    public class HeavenPiercerDrill : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Tools/HeavenPiercer";

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

    /// <summary>Laser en hélice du Heaven Piercer : casse les blocs qu'il traverse (ceux que la pioche 275 % peut casser).</summary>
    public class HelixLaser : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 40;
            Projectile.extraUpdates = 3;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.localAI[0]++;
            if (!Main.dedServ)
                for (int s = -1; s <= 1; s += 2)
                {
                    Vector2 n = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2);
                    Dust.NewDustPerfect(Projectile.Center + n * s * 8f * (float)Math.Sin(Projectile.localAI[0] * 0.4f), DustID.Electric, Vector2.Zero, 100, default, 0.9f).noGravity = true;
                }
            if (Projectile.owner != Main.myPlayer)
                return;
            Point t = Projectile.Center.ToTileCoordinates();
            Tile tile = Main.tile[t.X, t.Y];
            if (WorldGen.InWorld(t.X, t.Y, 5) && tile.HasTile && !Main.tileContainer[tile.TileType])
                Main.player[Projectile.owner].PickTile(t.X, t.Y, 275);
        }
    }
}
