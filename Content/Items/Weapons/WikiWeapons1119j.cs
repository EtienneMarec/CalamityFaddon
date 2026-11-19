using System;
using CalamityFandom.Content.Projectiles.Weapons1119j;
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

// Lot M (19/11) : armes du wiki qui sont des ingrédients de recettes (pages d'addon comprises, décisions section 11).

namespace CalamityFandom.Content.Items.Weapons.Summon
{
    /// <summary>
    /// Wiki : Dark Energy Staff.txt — tourelle immobile qui lance vite des salves de matière noire chercheuse ; deux à la fois.
    /// [choix] Une sentinelle (place de tourelle) ; salve de 3 toutes les 20 images à 700 px ; « deux à la fois » = deux au
    /// plus, quelle que soit la limite de tourelles du joueur.
    /// </summary>
    public class DarkEnergyStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.GamepadWholeScreenUseRange[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 76;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 10;
            Item.useTime = Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.UseSound = SoundID.Item78;
            Item.width = 82;
            Item.height = 88;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.value = Item.sellPrice(gold: 28);
            Item.shoot = ModContent.ProjectileType<DarkEnergyTurret>();
            Item.sentry = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Deux au plus : la plus ancienne disparaît
            int count = 0, oldest = -1, oldestTime = int.MaxValue;
            foreach (Projectile p in Main.ActiveProjectiles)
                if (p.owner == player.whoAmI && p.type == type)
                {
                    count++;
                    if (p.timeLeft < oldestTime)
                    {
                        oldestTime = p.timeLeft;
                        oldest = p.whoAmI;
                    }
                }
            if (count >= 2 && oldest >= 0)
                Main.projectile[oldest].Kill();
            int s = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI);
            if (Main.projectile.IndexInRange(s))
                Main.projectile[s].originalDamage = Item.damage;
            player.UpdateMaxTurrets();
            return false;
        }
    }

    /// <summary>
    /// Wiki : Jet Engine.txt — un grand poing d'onyx flottant fonce sur la cible et rebondit en la touchant ; à plus de 75
    /// blocs, il se téléporte vers elle.
    /// </summary>
    public class JetEngine : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 45;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 10;
            Item.useTime = Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.UseSound = SoundID.Item44;
            Item.width = 38;
            Item.height = 32;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(gold: 6);
            Item.buffType = ModContent.BuffType<Buffs.OnyxFistBuff>();
            Item.shoot = ModContent.ProjectileType<OnyxFist>();
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

namespace CalamityFandom.Content.Items.Weapons.Rogue
{
    /// <summary>
    /// Wiki : Profaned Nucleus.txt — morceau de roche profanée en fusion soumis à la gravité qui perce à l'infini et explose
    /// sur les ennemis touchés ; après 10 s ou sur un bloc, éclate en cristaux chercheurs ; frappe furtive : laisse des flaques
    /// de magma. Butin de Providence. [choix] 6 cristaux à 30 % ; explosions de 80 px à 50 % ; flaques de 3 s.
    /// </summary>
    public class ProfanedNucleus : RogueWeapon
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 367;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.useTime = Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = Item.noUseGraphic = true;
            Item.knockBack = 10f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = 59;
            Item.height = 81;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.value = Item.sellPrice(gold: 16);
            Item.shoot = ModContent.ProjectileType<ProfanedRock>();
            Item.shootSpeed = 20f;
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
    /// Wiki : Unsealed Singularity.txt — volée de 5 fioles de vide qui s'arrêtent vite et éclatent (après un moment, sur un
    /// bloc ou un ennemi) : trou noir de 5 s qui attire les ennemis (plus fort s'il y en a plusieurs) et appelle de la matière
    /// violette, et 3 éclats soumis à la gravité (25 %) ; frappe furtive : une fiole, un trou noir géant.
    /// </summary>
    public class UnsealedSingularity : RogueWeapon
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 520;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.useTime = Item.useAnimation = 48;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = Item.noUseGraphic = true;
            Item.knockBack = 10f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item106;
            Item.width = 78;
            Item.height = 70;
            Item.rare = ModContent.RarityType<CosmicPurple>();
            Item.value = Item.sellPrice(gold: 40);
            Item.shoot = ModContent.ProjectileType<VoidFlask>();
            Item.shootSpeed = 12f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int p = Projectile.NewProjectile(source, position, velocity, type, damage * 2, knockback, player.whoAmI);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].Calamity().stealthStrike = true;
                return false;
            }
            for (int i = -2; i <= 2; i++)
                Projectile.NewProjectile(source, position, velocity.RotatedBy(0.15f * i) * Main.rand.NextFloat(0.9f, 1.1f), type, damage, knockback, player.whoAmI);
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Typeless
{
    /// <summary>Wiki : Twisted Netherite Scythe.txt — se balance comme une épée ; toucher un ennemi donne 10 images d'invincibilité.</summary>
    public class TwistedNetheriteScythe : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 1;
            Item.DamageType = DamageClass.Default;
            Item.useTime = Item.useAnimation = 6;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 1f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = 58;
            Item.height = 52;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.value = Item.sellPrice(gold: 3);
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (player.immuneTime < 10)
            {
                player.immune = true;
                player.immuneNoBlink = false;
                player.immuneTime = 10;
            }
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Magic
{
    /// <summary>
    /// Wiki : Wrath of the Eldritch.txt — une pulsation à la position du curseur lance régulièrement deux anneaux de 16 esprits
    /// chercheurs et parfois un rayon de la mort. Amélioration du Wrath of the Ancients.
    /// [choix] Pulsation de 6 s, anneaux toutes les 90 images, rayon toutes les 3 salves vers l'ennemi le plus proche.
    /// </summary>
    public class WrathoftheEldritch : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 98;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 30;
            Item.useTime = Item.useAnimation = 38;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.knockBack = 6f;
            Item.UseSound = SoundID.Item117;
            Item.width = 38;
            Item.height = 44;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.value = Item.sellPrice(gold: 26);
            Item.shoot = ModContent.ProjectileType<WrathPulse>();
            Item.shootSpeed = 11f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI, 0f);
            return false;
        }
    }

    /// <summary>
    /// Wiki : Wrath of the Cosmos.txt — une pulsation galactique au curseur lance régulièrement deux anneaux de 24 boules de
    /// feu de God Slayer chercheuses (God Slayer Inferno 2 s) et parfois deux Cosmic Guardians (3 s). Amélioration du Wrath of
    /// the Eldritch. [choix] Comme lui ; Cosmic Guardians = deux grosses boules chercheuses (x2 dégâts, 3 s de God Slayer Inferno).
    /// </summary>
    public class WrathoftheCosmos : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 220;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 40;
            Item.useTime = Item.useAnimation = 34;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.knockBack = 6f;
            Item.UseSound = SoundID.Item117;
            Item.width = 38;
            Item.height = 44;
            Item.rare = ModContent.RarityType<CosmicPurple>();
            Item.value = Item.sellPrice(gold: 28);
            Item.shoot = ModContent.ProjectileType<WrathPulse>();
            Item.shootSpeed = 12f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI, 1f);
            return false;
        }
    }
}

namespace CalamityFandom.Content.Buffs
{
    public class OnyxFistBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<OnyxFist>()] > 0)
                player.buffTime[buffIndex] = 18000;
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Weapons1119j
{
    /// <summary>Tourelle d'énergie noire : salve de 3 boules de matière noire chercheuses toutes les 20 images.</summary>
    public class DarkEnergyTurret : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Summon/VoidConcentrationDarkEnergy";

        public override void SetStaticDefaults() => Main.projFrames[Type] = 1;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.sentry = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = Projectile.SentryLifeTime;
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            Projectile.rotation += 0.05f;
            Lighting.AddLight(Projectile.Center, 0.4f, 0.1f, 0.5f);
            NPC target = Projectile.Center.MinionHoming(700f, Main.player[Projectile.owner]);
            if (target == null || Projectile.owner != Main.myPlayer || ++Projectile.ai[0] < 20f)
                return;
            Projectile.ai[0] = 0f;
            for (int i = 0; i < 3; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.SafeDirectionTo(target.Center).RotatedByRandom(0.6f) * 9f,
                    ModContent.ProjectileType<DarkMatterBolt>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            // [technique] Première image seulement de la planche de Calamity (son découpage n'est pas régulier)
            int frameHeight = Math.Min(tex.Width, tex.Height);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, tex.Width, frameHeight), Color.White, Projectile.rotation,
                new Vector2(tex.Width / 2f, frameHeight / 2f), 1.5f, SpriteEffects.None);
            return false;
        }
    }

    /// <summary>Matière noire de la tourelle : cherche les ennemis.</summary>
    public class DarkMatterBolt : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults() => ProjectileID.Sets.SentryShot[Type] = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            CalamityUtils.HomeInOnNPC(Projectile, true, 800f, 12f, 15f);
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.Shadowflame, Vector2.Zero, 100, default, 1.3f).noGravity = true;
        }
    }

    /// <summary>Poing d'onyx du Jet Engine : fonce sur la cible, rebondit au contact ; se téléporte si la cible est à plus de 75 blocs.</summary>
    public class OnyxFist : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Summon/OnyxFist";

        public override void SetStaticDefaults()
        {
            Main.projPet[Type] = true;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 1f;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 18000;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.scale = 0.7f;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.dead || !owner.active)
                owner.ClearBuff(ModContent.BuffType<Buffs.OnyxFistBuff>());
            if (owner.HasBuff(ModContent.BuffType<Buffs.OnyxFistBuff>()))
                Projectile.timeLeft = 2;
            NPC target = Projectile.Center.MinionHoming(2000f, owner);
            if (target == null)
            {
                Vector2 home = owner.Center + new Vector2(-owner.direction * (50f + 30f * Projectile.minionPos), -40f);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, (home - Projectile.Center) * 0.1f, 0.2f);
                if (Projectile.Distance(owner.Center) > 2000f)
                    Projectile.Center = owner.Center;
            }
            else
            {
                if (Projectile.Distance(target.Center) > 1200f)
                {
                    Projectile.Center = target.Center + Main.rand.NextVector2CircularEdge(200f, 200f);
                    Projectile.netUpdate = true;
                }
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(target.Center) * 18f, 0.08f);
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.spriteDirection = 1;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => Projectile.velocity = -Projectile.velocity * 0.8f;
    }

    /// <summary>
    /// Roche profanée : gravité, perce à l'infini, explosion à chaque ennemi touché ; après 10 s ou sur un bloc, éclate en 6
    /// cristaux chercheurs ; furtive : flaques de magma.
    /// </summary>
    public class ProfanedRock : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Rogue/ProfanedNucleus";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.scale = 0.5f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.3f, 16f);
            Projectile.rotation += 0.2f;
            Lighting.AddLight(Projectile.Center, 1f, 0.6f, 0.2f);
            if (Projectile.Calamity().stealthStrike && Projectile.owner == Main.myPlayer && Projectile.timeLeft % 20 == 0)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<MagmaPatch>(), Projectile.damage / 4, 0f, Projectile.owner);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<ProfanedBlast>(), Projectile.damage / 2, 0f, Projectile.owner);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item62 with { Volume = 0.6f }, Projectile.Center);
            if (Projectile.owner != Main.myPlayer)
                return;
            for (int i = 0; i < 6; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (MathHelper.TwoPi * i / 6f).ToRotationVector2() * 9f, ModContent.ProjectileType<ProfanedCrystal>(),
                    (int)(Projectile.damage * 0.3f), 0f, Projectile.owner);
        }
    }

    /// <summary>Explosion de la roche profanée (80 px).</summary>
    public class ProfanedBlast : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 80;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 5;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (!Main.dedServ)
                for (int i = 0; i < 15; i++)
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SolarFlare, 0f, 0f, 100, default, 1.5f).velocity *= 2f;
        }
    }

    /// <summary>Cristal profané : cherche les ennemis (Holy Flames 2 s).</summary>
    public class ProfanedCrystal : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.CrystalShard;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = 1;
            Projectile.timeLeft = 150;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (++Projectile.ai[0] > 15f)
                CalamityUtils.HomeInOnNPC(Projectile, true, 700f, 14f, 12f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 200, 100, 200);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<HolyFlames>(), 120);
    }

    /// <summary>Flaque de magma (frappe furtive du Profaned Nucleus) : 3 s, tombe et reste au sol.</summary>
    public class MagmaPatch : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void AI()
        {
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.4f, 12f);
            Projectile.velocity.X = 0f;
            if (!Main.dedServ && Main.rand.NextBool(2))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, -1f, 100, default, 1.5f).noGravity = true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = Vector2.Zero;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.OnFire3, 120);
    }

    /// <summary>
    /// Fiole de vide : ralentit vite, éclate au bout de 40 images, sur un bloc ou un ennemi : trou noir (5 s) et 3 éclats (25 %).
    /// Furtive : trou noir géant.
    /// </summary>
    public class VoidFlask : ModProjectile
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.BottledWater;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = 1;
            Projectile.timeLeft = 40;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.95f;
            Projectile.rotation += 0.25f;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(150, 80, 220);

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Shatter, Projectile.Center);
            if (Projectile.owner != Main.myPlayer)
                return;
            bool stealth = Projectile.Calamity().stealthStrike;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SingularityHole>(), Projectile.damage / 2, 0f, Projectile.owner, stealth ? 1f : 0f);
            for (int i = 0; i < 3; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-6f, -2f)),
                    ModContent.ProjectileType<VoidShard>(), Projectile.damage / 4, 0f, Projectile.owner);
        }
    }

    /// <summary>
    /// Trou noir de l'Unsealed Singularity : 5 s, attire les ennemis (non-boss) à 300 px (plus fort avec chaque autre trou noir
    /// du joueur), une matière violette chercheuse toutes les 30 images ; ai[0] = 1 : géant (x2,5, matière plus grosse).
    /// </summary>
    public class SingularityHole : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Magic/EventHorizonBlackhole";

        private bool Giant => Projectile.ai[0] == 1f;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Giant)
            {
                Projectile.scale = 2.5f;
                Projectile.Resize(100, 100);
            }
        }

        public override void AI()
        {
            Projectile.rotation -= 0.15f;
            int holes = 0;
            foreach (Projectile p in Main.ActiveProjectiles)
                if (p.type == Type && p.owner == Projectile.owner)
                    holes++;
            float pull = 0.15f * (1f + 0.25f * (holes - 1)) * (Giant ? 2f : 1f);
            float range = Giant ? 600f : 300f;
            foreach (NPC npc in Main.ActiveNPCs)
                if (npc.CanBeChasedBy() && !npc.boss && npc.knockBackResist > 0f && npc.Distance(Projectile.Center) < range)
                    npc.velocity += npc.SafeDirectionTo(Projectile.Center) * pull;
            if (Projectile.owner == Main.myPlayer && Projectile.timeLeft % 30 == 0)
            {
                int m = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2CircularEdge(6f, 6f), ModContent.ProjectileType<VoidMatter>(),
                    Projectile.damage, 0f, Projectile.owner);
                if (Giant && Main.projectile.IndexInRange(m))
                    Main.projectile[m].scale = 2f;
            }
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;
    }

    /// <summary>Éclat de fiole : gravité.</summary>
    public class VoidShard : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.CrystalShard;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.25f, 12f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(160, 90, 230, 200);
    }

    /// <summary>Matière violette : cherche les ennemis.</summary>
    public class VoidMatter : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            CalamityUtils.HomeInOnNPC(Projectile, true, 700f, 12f, 15f);
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.PurpleTorch, Vector2.Zero, 100, default, 1.4f * Projectile.scale).noGravity = true;
        }
    }

    /// <summary>
    /// Pulsation des Wrath : ai[0] = 0 eldritch (2 anneaux de 16 esprits, rayon toutes les 3 salves), 1 cosmos (2 anneaux
    /// de 24 boules de God Slayer, 2 Cosmic Guardians toutes les 3 salves). 6 s, une salve toutes les 90 images.
    /// </summary>
    public class WrathPulse : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Magic/GalacticPulse";

        public override void SetStaticDefaults() => Main.projFrames[Type] = 6;

        private bool Cosmos => Projectile.ai[0] == 1f;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 38;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 360;
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            if (++Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
            }
            Lighting.AddLight(Projectile.Center, Cosmos ? 0.6f : 0.3f, 0.2f, 0.6f);
            if (Projectile.owner != Main.myPlayer || ++Projectile.ai[1] % 90 != 0)
                return;
            int count = Cosmos ? 24 : 16;
            for (int ring = 0; ring < 2; ring++)
                for (int i = 0; i < count; i++)
                {
                    Vector2 dir = (MathHelper.TwoPi * (i + ring * 0.5f) / count).ToRotationVector2();
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, dir * (5f + ring * 2f), ModContent.ProjectileType<WrathSpirit>(),
                        Projectile.damage / 3, 0f, Projectile.owner, Projectile.ai[0]);
                }
            if (Projectile.ai[1] % 270 != 0)
                return;
            NPC target = Projectile.Center.ClosestNPCAt(1200f);
            if (Cosmos)
                for (int i = -1; i <= 1; i += 2)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(i * 6f, -4f), ModContent.ProjectileType<WrathSpirit>(),
                        Projectile.damage * 2, 0f, Projectile.owner, 2f);
            else if (target != null)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.SafeDirectionTo(target.Center) * 30f, ProjectileID.LastPrismLaser,
                    Projectile.damage, 0f, Projectile.owner);
        }
    }

    /// <summary>Esprit (ai[0] = 0), boule de God Slayer (1 : God Slayer Inferno 2 s) ou Cosmic Guardian (2 : grosse, 3 s) : cherche les ennemis.</summary>
    public class WrathSpirit : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (++Projectile.localAI[0] > 20f)
                CalamityUtils.HomeInOnNPC(Projectile, true, 900f, Projectile.ai[0] == 2f ? 10f : 13f, 18f);
            if (Main.dedServ)
                return;
            int dust = Projectile.ai[0] == 0f ? DustID.DungeonSpirit : DustID.PurpleTorch;
            Dust.NewDustPerfect(Projectile.Center, dust, Vector2.Zero, 100, default, Projectile.ai[0] == 2f ? 2.5f : 1.2f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[0] >= 1f)
                target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), Projectile.ai[0] == 2f ? 180 : 120);
        }
    }
}
