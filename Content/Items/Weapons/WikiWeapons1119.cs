using System;
using CalamityFandom.Content.Projectiles.Weapons1119;
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

// Armes inventées par le wiki (19/11) : sprites et valeurs des pages, recettes du wiki (WikiRecipes).

namespace CalamityFandom.Content.Items.Weapons.Summon
{
    /// <summary>
    /// Wiki : Fakenado Staff.txt — invoque une fausse tornade de sable qui tire de petits requins en peluche très rapides sur
    /// les ennemis proches (Vaporfied 1 s).
    /// </summary>
    public class FakenadoStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.GamepadWholeScreenUseRange[Type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 25;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 10;
            Item.useTime = Item.useAnimation = 19;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.UseSound = SoundID.Item44;
            Item.width = 36;
            Item.height = 52;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(gold: 26);
            Item.buffType = ModContent.BuffType<Buffs.FakenadoBuff>();
            Item.shoot = ModContent.ProjectileType<FakenadoMinion>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            int p = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI);
            if (Main.projectile.IndexInRange(p))
                Main.projectile[p].originalDamage = Item.damage;
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Magic
{
    /// <summary>
    /// Wiki : Onyx Purge.txt — comme le Nano Purge : une rafale de lasers, et des éclats d'onyx qui explosent sur les
    /// ennemis. [choix] Trois lasers et un éclat par tir.
    /// </summary>
    public class OnyxPurge : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 90;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 6;
            Item.useTime = Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4.5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item12;
            Item.width = 62;
            Item.height = 34;
            Item.rare = ModContent.RarityType<CosmicPurple>();
            Item.value = Item.sellPrice(gold: 28);
            Item.shoot = ProjectileID.PurpleLaser;
            Item.shootSpeed = 28f;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-6f, 0f);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 3; i++)
            {
                int l = Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.08f), type, damage, knockback, player.whoAmI);
                if (Main.projectile.IndexInRange(l))
                {
                    Main.projectile[l].DamageType = DamageClass.Magic;
                    Main.projectile[l].hostile = false;
                    Main.projectile[l].friendly = true;
                }
            }
            Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.15f) * 0.6f, ModContent.ProjectileType<OnyxShard>(), damage, knockback, player.whoAmI);
            return false;
        }
    }

    /// <summary>
    /// Wiki : Interfacer Staff.txt — rayon rose et bleu qui rebondit sur les blocs et tire de petits rayons vers les ennemis
    /// proches.
    /// </summary>
    public class InterfacerStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 125;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 50;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item72;
            Item.width = 62;
            Item.height = 60;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.value = Item.sellPrice(gold: 40);
            Item.shoot = ModContent.ProjectileType<InterfacerBeam>();
            Item.shootSpeed = 13.5f;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Rogue
{
    /// <summary>
    /// Wiki : Exif Fist.txt — poing rapide légèrement soumis à la gravité qui laisse une traînée et explose en 4 à 6 étoiles
    /// (Astral Infection 2 s) ; frappe furtive : ricoche entre 5 ennemis en faisant pleuvoir des étoiles.
    /// </summary>
    public class ExifFist : RogueWeapon
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 85;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.useTime = Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = Item.noUseGraphic = true;
            Item.knockBack = 7f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = 32;
            Item.height = 34;
            Item.rare = ItemRarityID.Lime;
            Item.value = Item.sellPrice(gold: 4);
            Item.shoot = ModContent.ProjectileType<ExifFistProj>();
            Item.shootSpeed = 5f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (player.Calamity().StealthStrikeAvailable() && Main.projectile.IndexInRange(p))
                Main.projectile[p].Calamity().stealthStrike = true;
            return false;
        }
    }

    /// <summary>
    /// Wiki : Hand of God.txt — comme l'Exif Fist, en plus fort : explose en 4 à 6 planètes (Moon Burnt 2 s) ; frappe
    /// furtive : ricoche entre 7 ennemis puis une explosion colossale.
    /// </summary>
    public class HandofGod : ExifFist
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 126;
            Item.useTime = Item.useAnimation = 26;
            Item.knockBack = 10f;
            Item.width = 30;
            Item.height = 60;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.value = Item.sellPrice(gold: 8);
            Item.shoot = ModContent.ProjectileType<HandofGodProj>();
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Typeless
{
    /// <summary>
    /// Wiki : Celesticus.txt — arme sans classe qui profite de tous les bonus de dégâts : lame qui file en ligne droite à
    /// travers les blocs ; en touchant un ennemi, elle lâche 8 cristaux chercheurs (25 % des dégâts de base, Vaporfied 2,5 s).
    /// Ne peut pas être reforgée.
    /// </summary>
    public class Celesticus : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 80;
            Item.DamageType = ModContent.GetInstance<AverageDamageClass>();
            Item.useTime = Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = Item.noUseGraphic = true;
            Item.knockBack = 5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = 60;
            Item.height = 58;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(gold: 20);
            Item.shoot = ModContent.ProjectileType<CelesticusBlade>();
            Item.shootSpeed = 32f;
        }

        public override bool CanReforge() => false;
    }
}

namespace CalamityFandom.Content.Items.Weapons.Ranged
{
    /// <summary>
    /// Wiki : Supercharged Dart Blaster.txt — flot rapide de fléchettes de plasma qui explosent ; clic droit : onde électrique
    /// qui grandit et se sépare en 9 autres. [choix] Pas de munition (les fléchettes sont de plasma).
    /// </summary>
    public class SuperchargedDartBlaster : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 100;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 5;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item92;
            Item.width = 102;
            Item.height = 24;
            Item.scale = 0.75f;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.value = Item.sellPrice(gold: 30);
            Item.shoot = ModContent.ProjectileType<PlasmaDart>();
            Item.shootSpeed = 22f;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override Vector2? HoldoutOffset() => new Vector2(-12f, 0f);

        public override bool CanUseItem(Player player)
        {
            Item.useTime = player.altFunctionUse == 2 ? 25 : 5;
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(source, position, velocity * 0.5f, ModContent.ProjectileType<ElectricWave>(), damage, knockback, player.whoAmI);
                return false;
            }
            Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.05f), type, damage, knockback, player.whoAmI);
            return false;
        }
    }
}

namespace CalamityFandom.Content.Buffs
{
    public class FakenadoBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<FakenadoMinion>()] > 0)
                player.buffTime[buffIndex] = 18000;
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Weapons1119
{
    /// <summary>Fakenado : reste près du joueur et tire un requin en peluche toutes les 30 images sur l'ennemi le plus proche.</summary>
    public class FakenadoMinion : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Summon/FakenadoMinion";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            Main.projPet[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 1f;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 18000;
            Projectile.scale = 0.7f;
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!owner.active || owner.dead || !owner.HasBuff<Buffs.FakenadoBuff>())
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 2;
            Vector2 home = owner.Center + new Vector2(-owner.direction * (50f + 40f * Projectile.minionPos), -50f);
            Projectile.Center = Vector2.Lerp(Projectile.Center, home, 0.08f);
            Projectile.rotation = (float)Math.Sin(Main.GameUpdateCount * 0.2f) * 0.1f;
            if (Main.rand.NextBool(3))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Sand, 0f, -1f, 100, default, 0.9f).noGravity = true;
            NPC target = Projectile.Center.MinionHoming(700f, owner);
            if (target == null || Projectile.owner != Main.myPlayer || ++Projectile.ai[0] < 30f)
                return;
            Projectile.ai[0] = 0f;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.SafeDirectionTo(target.Center) * 18f,
                ModContent.ProjectileType<SharkPlush>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }

    /// <summary>Requin en peluche : très rapide, Vaporfied 1 s.</summary>
    public class SharkPlush : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Summon/SharkPlush";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 90;
            Projectile.scale = 0.3f;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Projectile.spriteDirection = Projectile.velocity.X > 0 ? -1 : 1;
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? MathHelper.Pi : 0f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Vaporfied>(), 60);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            SpriteEffects fx = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, fx);
            return false;
        }
    }

    /// <summary>Éclat d'onyx de l'Onyx Purge : explose au contact (80 px, moitié des dégâts).</summary>
    public class OnyxShard : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.CrystalShard;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
        }

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        public override Color? GetAlpha(Color lightColor) => new Color(70, 40, 90, 230);

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14 with { Volume = 0.4f }, Projectile.Center);
            Projectile.ExpandHitboxBy(80);
            Projectile.maxPenetrate = Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.damage /= 2;
            Projectile.Damage();
            for (int i = 0; i < 15; i++)
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Shadowflame, 0f, 0f, 100, default, 1.4f).velocity *= 2f;
        }
    }

    /// <summary>Rayon de l'Interfacer Staff : rebondit sur les blocs (5 fois) et tire un petit rayon vers un ennemi proche toutes les 12 images.</summary>
    public class InterfacerBeam : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 240;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            bool small = Projectile.ai[1] == 1f;
            int dust = Projectile.ai[0] % 2 == 0 ? DustID.PinkTorch : DustID.BlueTorch;
            Projectile.ai[0]++;
            if (!Main.dedServ)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, dust, Vector2.Zero, 100, default, small ? 1f : 1.6f);
                d.noGravity = true;
            }
            if (small || Projectile.owner != Main.myPlayer || Projectile.ai[0] % 36f != 0f)
                return;
            NPC target = Projectile.Center.ClosestNPCAt(300f);
            if (target != null)
            {
                int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.SafeDirectionTo(target.Center) * 10f, Type,
                    Projectile.damage / 3, 0f, Projectile.owner, 0f, 1f);
                if (Main.projectile.IndexInRange(p))
                {
                    Main.projectile[p].timeLeft = 60;
                    Main.projectile[p].penetrate = 1;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.ai[1] == 1f || ++Projectile.localAI[0] > 5)
                return true;
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            SoundEngine.PlaySound(SoundID.Item10 with { Volume = 0.4f }, Projectile.Center);
            return false;
        }
    }

    /// <summary>
    /// Poing de l'Exif Fist : traînée, légère gravité, explose en 4 à 6 étoiles qui ignorent l'invulnérabilité ; en frappe
    /// furtive, ricoche d'ennemi en ennemi (<see cref="Bounces"/> fois) en faisant tomber des étoiles.
    /// </summary>
    public class ExifFistProj : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Rogue/ExifFist";

        protected virtual int Bounces => 5;
        protected virtual int Debuff => ModContent.BuffType<AstralInfectionDebuff>();
        protected virtual int TrailDust => DustID.BlueFairy;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Projectile.Calamity().stealthStrike)
                Projectile.penetrate = Bounces + 1;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.03f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Dust.NewDustPerfect(Projectile.Center, TrailDust, Vector2.Zero, 100, default, 1.1f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(Debuff, 120);
            if (!Projectile.Calamity().stealthStrike || Projectile.owner != Main.myPlayer)
                return;
            // Ricochet vers un autre ennemi, avec une pluie d'étoiles
            Stars(target.Center - new Vector2(0f, 500f), true);
            NPC next = null;
            float best = 600f;
            foreach (var n in Main.ActiveNPCs)
                if (n.whoAmI != target.whoAmI && n.CanBeChasedBy(Projectile) && Projectile.Distance(n.Center) < best && Projectile.localNPCImmunity[n.whoAmI] == 0)
                {
                    best = Projectile.Distance(n.Center);
                    next = n;
                }
            if (next != null)
                Projectile.velocity = Projectile.SafeDirectionTo(next.Center) * Projectile.velocity.Length();
        }

        protected void Stars(Vector2 from, bool falling)
        {
            int count = Main.rand.Next(4, 7);
            for (int i = 0; i < count; i++)
            {
                Vector2 v = falling ? new Vector2(Main.rand.NextFloat(-3f, 3f), 14f) : Main.rand.NextVector2CircularEdge(8f, 8f);
                Vector2 pos = falling ? from + new Vector2(Main.rand.Next(-150, 151), 0f) : from;
                int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, v, ModContent.ProjectileType<FistStar>(), Projectile.damage / 2,
                    0f, Projectile.owner, Debuff);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].DamageType = Projectile.DamageType;
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14 with { Volume = 0.6f }, Projectile.Center);
            if (Projectile.owner == Main.myPlayer)
                Stars(Projectile.Center, false);
            for (int i = 0; i < 20; i++)
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, TrailDust, 0f, 0f, 100, default, 1.5f).velocity *= 2.5f;
        }
    }

    /// <summary>Poing du Hand of God : planètes (Moon Burnt), 7 ricochets, puis une explosion colossale en frappe furtive.</summary>
    public class HandofGodProj : ExifFistProj
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Rogue/HandofGod";

        protected override int Bounces => 7;
        protected override int Debuff => ModContent.BuffType<Nightwither>();
        protected override int TrailDust => DustID.PurpleTorch;

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            if (!Projectile.Calamity().stealthStrike)
                return;
            // Explosion colossale (la page parle de 300 blocs de diamètre ; [choix] 100 blocs, l'écran entier)
            SoundEngine.PlaySound(SoundID.Item62, Projectile.Center);
            Projectile.ExpandHitboxBy(1600);
            Projectile.maxPenetrate = Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            for (int i = 0; i < Main.maxNPCs; i++)
                Projectile.localNPCImmunity[i] = 0;
            Projectile.damage *= 3;
            Projectile.Damage();
            for (int i = 0; i < 80; i++)
                Dust.NewDustPerfect(Projectile.Center, DustID.PurpleTorch, Main.rand.NextVector2Circular(30f, 30f), 100, default, 2.5f).noGravity = true;
        }
    }

    /// <summary>Étoile ou planète des poings : frappe une fois, ignore l'invulnérabilité ; ai[0] = debuff.</summary>
    public class FistStar : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.FallingStar;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 90;
            Projectile.tileCollide = false;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 0;
            Projectile.scale = 0.7f;
        }

        public override void AI()
        {
            Projectile.rotation += 0.25f;
            Projectile.velocity *= 0.99f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[0] > 0f)
                target.AddBuff((int)Projectile.ai[0], 120);
        }
    }

    /// <summary>Lame du Celesticus : traverse les blocs ; au premier ennemi, 8 cristaux chercheurs (25 %, Vaporfied 2,5 s).</summary>
    public class CelesticusBlade : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Typeless/Celesticus";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<AverageDamageClass>();
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
            Projectile.scale = 0.6f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Vaporfied>(), 150);
            if (Projectile.ai[0] != 0f || Projectile.owner != Main.myPlayer)
                return;
            Projectile.ai[0] = 1f;
            Player owner = Main.player[Projectile.owner];
            int baseDamage = (int)(owner.HeldItem.damage * 0.25f);
            for (int i = 0; i < 8; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, (MathHelper.TwoPi * i / 8f).ToRotationVector2() * 7f,
                    ModContent.ProjectileType<AestheticusCrystal>(), Math.Max(1, baseDamage), 0f, Projectile.owner);
        }
    }

    public class AestheticusCrystal : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Typeless/AestheticusCrystal";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<AverageDamageClass>();
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (++Projectile.ai[0] > 10f)
                CalamityUtils.HomeInOnNPC(Projectile, true, 500f, 12f, 15f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Vaporfied>(), 150);
    }

    /// <summary>Fléchette de plasma : petite explosion à l'impact (48 px).</summary>
    public class PlasmaDart : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.PoisonDart;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Lighting.AddLight(Projectile.Center, 0.2f, 0.4f, 1f);
        }

        public override Color? GetAlpha(Color lightColor) => new Color(120, 180, 255, 200);

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item93 with { Volume = 0.3f }, Projectile.Center);
            Projectile.ExpandHitboxBy(48);
            Projectile.maxPenetrate = Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.damage /= 2;
            Projectile.Damage();
            for (int i = 0; i < 8; i++)
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, 0f, 0f, 100, default, 0.8f).velocity *= 2f;
        }
    }

    /// <summary>Onde électrique (clic droit) : grandit, puis se sépare en 9 ondes plus petites.</summary>
    public class ElectricWave : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 90;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void AI()
        {
            float size = 20f + Projectile.ai[1] * 0f + (90 - Projectile.timeLeft) * 1.5f;
            if (Projectile.ai[0] == 0f)
                Projectile.Resize((int)size, (int)size);
            if (!Main.dedServ)
                for (int i = 0; i < 4; i++)
                {
                    Vector2 edge = Projectile.Center + Main.rand.NextVector2CircularEdge(Projectile.width / 2f, Projectile.height / 2f);
                    Dust.NewDustPerfect(edge, DustID.Electric, Vector2.Zero, 100, default, 0.9f).noGravity = true;
                }
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.ai[0] != 0f || Projectile.owner != Main.myPlayer)
                return;
            for (int i = 0; i < 9; i++)
            {
                int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (MathHelper.TwoPi * i / 9f).ToRotationVector2() * 8f, Type,
                    Projectile.damage / 2, 0f, Projectile.owner, 1f);
                if (Main.projectile.IndexInRange(p))
                {
                    Main.projectile[p].timeLeft = 40;
                    Main.projectile[p].Resize(40, 40);
                }
            }
        }
    }
}
