// Adapté de Calamity Mod (objets retirés en 2022), © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using System;
using CalamityFandom.Content.Legacy.Buffs.StatDebuffs;
using CalamityFandom.Content.Legacy.Projectiles.Melee;
using CalamityFandom.Content.Legacy.Projectiles.Rogue;
using CalamityMod;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Crystal Blade.txt — épée qui lance un lent nuage de poussière de cristal (use 24 selon le wiki, 25 dans
    /// l'ancien code) ; lâchée par le Crystal Crawler (butin du wiki).
    /// </summary>
    public class CrystalBlade : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = Item.height = 66;
            Item.damage = 45;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.knockBack = 4.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = Item.sellPrice(gold: 2, silver: 40);
            Item.rare = ItemRarityID.LightRed;
            Item.shoot = ModContent.ProjectileType<CrystalDust>();
            Item.shootSpeed = 3f;
        }
    }

    /// <summary>
    /// Wiki : Temporal Floe Sword.txt — rayon de glace à 65 % des dégâts qui se divise, au contact d'un ennemi ou d'un
    /// bloc, en deux rayons à 32,5 % orientés comme les aiguilles d'une horloge à l'heure du jeu. Contact : Glacial State
    /// 1 s et Frostburn 5 s ; rayon principal 0,5 s / 3 s ; rayons divisés 0,5 s / 2 s (infobox).
    /// </summary>
    public class TemporalFloeSword : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 60;
            Item.damage = 95;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.knockBack = 6f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = Item.sellPrice(gold: 16);
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = ModContent.ProjectileType<TemporalFloeSwordProjectile>();
            Item.shootSpeed = 16f;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
            => damage = (int)(damage * 0.65f);

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
                Dust.NewDust(hitbox.TopLeft(), hitbox.Width, hitbox.Height, 34);
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<GlacialState>(), 60);
            target.AddBuff(BuffID.Frostburn, 300);
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(ModContent.BuffType<GlacialState>(), 60);
            target.AddBuff(BuffID.Frostburn, 300);
        }
    }

    /// <summary>Wiki : Marnite Spear.txt — lance de vrai corps à corps, pré-Hardmode.</summary>
    public class MarniteSpear : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = Item.height = 50;
            Item.damage = 26;
            Item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Item.noMelee = true;
            Item.useTurn = true;
            Item.noUseGraphic = true;
            Item.useTime = Item.useAnimation = 21;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 5.25f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = Item.sellPrice(silver: 20);
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ModContent.ProjectileType<MarniteSpearProjectile>();
            Item.shootSpeed = 5f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;
    }
}

namespace CalamityFandom.Content.Legacy.Items.Weapons.Rogue
{
    /// <summary>
    /// Wiki : Sylvan Slasher.txt — entailles en rafale au curseur (comme l'Arkhalis) ; chaque coup sur un ennemi (dégâts
    /// de contact ≥ 5 ou boss, pas de statue) donne 5 points de discrétion, au plus 10 fois par seconde ; 1 chance sur 8
    /// de lancer une vague d'épée depuis le joueur, à l'opposé du curseur. Ne consomme pas la discrétion, pas d'attaque
    /// furtive. Vendue 5 platine par le Bandit après Providence.
    /// </summary>
    public class SylvanSlasher : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 72;
            Item.height = 78;
            Item.damage = 52;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.useAnimation = 25;
            Item.useTime = 5;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 3f;
            Item.value = Item.sellPrice(gold: 24);
            Item.rare = ModContent.RarityType<CalamityMod.Rarities.Turquoise>();
            Item.shoot = ModContent.ProjectileType<SylvanSlashAttack>();
            Item.shootSpeed = 24f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;
    }

    /// <summary>
    /// Objets portés vendus par le Bandit (pages des objets) : Sylvan Slasher après Providence (5 platine), Bouncing Betty
    /// et Latcher Mine après un boss mécanique (4 et 3 argent, leur prix).
    /// </summary>
    public class LegacyBanditShop : GlobalNPC
    {
        public override void ModifyShop(NPCShop shop)
        {
            if (!ModContent.TryFind<ModNPC>("CalamityMod", "Bandit", out var bandit) || shop.NpcType != bandit.Type)
                return;
            shop.Add(new Item(ModContent.ItemType<SylvanSlasher>()) { shopCustomPrice = Item.buyPrice(platinum: 5) },
                CalamityMod.CalamityConditions.DownedProvidence);
            shop.Add(ModContent.ItemType<BouncingBetty>(), Condition.DownedMechBossAny);
            shop.Add(ModContent.ItemType<LatcherMine>(), Condition.DownedMechBossAny);
        }
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.Melee
{
    /// <summary>Nuage de poussière de cristal (invisible, poussières), traverse 3 ennemis.</summary>
    public class CrystalDust : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 46;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 100;
        }

        public override void AI()
        {
            if (++Projectile.localAI[0] <= 4f)
                return;
            int dustType = Main.rand.Next(3) switch { 0 => 173, 1 => 57, _ => 58 };
            for (int i = 0; i < 5; i++)
            {
                var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, 2f);
                dust.noGravity = true;
                dust.velocity = Vector2.Zero;
            }
        }
    }

    /// <summary>
    /// Rayon de la Temporal Floe Sword : à sa fin (ennemi ou bloc), deux rayons divisés. [choix] Horloge du wiki : la
    /// minute tourne de 6° par minute, l'heure de 15° par heure (cadran de 24 h, le code de 2022 utilisait 12 h), dans le
    /// sens horaire, aiguilles vers le haut à midi.
    /// </summary>
    public class TemporalFloeSwordProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.aiStyle = ProjAIStyleID.Beam;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.coldDamage = true;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0f, (255 - Projectile.alpha) * 0.05f / 255f, (255 - Projectile.alpha) * 0.35f / 255f);
            if (Main.rand.NextBool(2))
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 56);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > 595)
                return false;
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(50, 50, 255, Projectile.alpha);

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            if (Projectile.owner != Main.myPlayer)
                return;
            // Heure du jeu sur 24 h (Main.time compte depuis 4 h 30 le jour, 19 h 30 la nuit)
            double hours = Main.time / 3600.0 + (Main.dayTime ? 4.5 : 19.5);
            hours %= 24.0;
            double minutes = (hours - Math.Floor(hours)) * 60.0;
            double sinceNoon = (hours - 12.0 + 24.0) % 24.0;
            int type = ModContent.ProjectileType<TemporalFloeNumberTwo>();
            foreach (double degrees in new[] { Math.Floor(minutes) * 6.0, Math.Floor(sinceNoon) * 15.0 })
            {
                // Sens horaire depuis le haut de l'écran (y vers le bas)
                float angle = MathHelper.ToRadians((float)degrees);
                Vector2 velocity = new Vector2(MathF.Sin(angle), -MathF.Cos(angle)) * 8f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, type, Projectile.damage / 2, Projectile.knockBack * 0.5f, Projectile.owner);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<GlacialState>(), 30);
            target.AddBuff(BuffID.Frostburn, 180);
        }
    }

    /// <summary>Rayon divisé de la Temporal Floe Sword (32,5 % des dégâts de l'épée), traverse tout pendant 1 s.</summary>
    public class TemporalFloeNumberTwo : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Legacy/Projectiles/Melee/TemporalFloeSwordProjectile";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.aiStyle = ProjAIStyleID.Beam;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.coldDamage = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0f, (255 - Projectile.alpha) * 0.05f / 255f, (255 - Projectile.alpha) * 0.35f / 255f);
            if (Main.rand.NextBool(2))
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 56);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(50, 50, 255, Projectile.alpha);

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int i = 1; i < 27; i++)
            {
                Vector2 back = Projectile.oldVelocity * (30f / i);
                var dust = Dust.NewDustDirect(Projectile.oldPosition - back, 8, 8, 56, Projectile.oldVelocity.X, Projectile.oldVelocity.Y, 100, default, 1.8f);
                dust.noGravity = true;
                dust.velocity *= 0.5f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<GlacialState>(), 30);
            target.AddBuff(BuffID.Frostburn, 120);
        }
    }

    /// <summary>Lance de la Marnite Spear (IA de lance de Calamity).</summary>
    public class MarniteSpearProjectile : BaseSpearProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 50;
            Projectile.aiStyle = ProjAIStyleID.Spear;
            Projectile.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Projectile.timeLeft = 90;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
        }

        public override float InitialSpeed => 3f;
        public override float ReelbackSpeed => 1.1f;
        public override float ForwardSpeed => 0.95f;
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.Rogue
{
    /// <summary>
    /// Entailles de la Sylvan Slasher : suivent le curseur tant que l'arme est tenue. Discrétion : +5 points par coup (un
    /// coup compté toutes les 3 frames au plus), jamais sur un ennemi à moins de 5 dégâts de contact (hors boss) ni sur
    /// un ennemi de statue ; 1 chance sur 8 par coup de lancer une vague (50 % des dégâts).
    /// </summary>
    public class SylvanSlashAttack : ModProjectile
    {
        public override void SetStaticDefaults() => Main.projFrames[Type] = 28;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 68;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (++Projectile.frame >= Main.projFrames[Type])
                Projectile.frame = 0;
            if (--Projectile.soundDelay <= 0)
            {
                SoundEngine.PlaySound(SoundID.Item15, Projectile.Center);
                Projectile.soundDelay = 24;
            }
            if (Projectile.ai[0] > 0)
                Projectile.ai[0]--;
            if (Main.myPlayer != Projectile.owner)
                return;

            Vector2 center = player.RotatedRelativePoint(player.MountedCenter, true);
            if (!player.channel || player.noItems || player.CCed)
            {
                Projectile.Kill();
                return;
            }
            Vector2 aim = (Main.MouseWorld - center).SafeNormalize(Vector2.UnitX * player.direction) * player.HeldItem.shootSpeed * Projectile.scale;
            if (aim != Projectile.velocity)
                Projectile.netUpdate = true;
            Projectile.velocity = aim;

            Vector2 lightPos = Projectile.Center + Projectile.velocity * 3f;
            Lighting.AddLight(lightPos, 0.2f, 2f, 3f);
            if (Main.rand.NextBool(3))
            {
                var dust = Dust.NewDustDirect(lightPos - Projectile.Size / 2f, Projectile.width, Projectile.height, 111, Projectile.velocity.X, Projectile.velocity.Y, 100, default, 2f);
                dust.noGravity = true;
                dust.position -= Projectile.velocity;
            }
            Projectile.Center = Main.MouseWorld;
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f);
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = player.itemAnimation = 2;
            player.itemRotation = MathF.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.owner != Main.myPlayer)
                return;
            Player player = Main.player[Projectile.owner];
            var calPlayer = player.Calamity();
            if (Projectile.ai[0] <= 0f && (target.damage > 5 || target.boss) && !target.SpawnedFromStatue
                && calPlayer.wearingRogueArmor && calPlayer.rogueStealthMax != 0 && calPlayer.rogueStealth < calPlayer.rogueStealthMax)
            {
                calPlayer.rogueStealth = Math.Min(calPlayer.rogueStealth + 0.05f, calPlayer.rogueStealthMax);
                Projectile.ai[0] = 3f;
            }
            if (Main.rand.NextBool(8))
            {
                Vector2 away = -(Projectile.Center - player.Center).SafeNormalize(Vector2.UnitX);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center, away * Main.rand.NextFloat(3f, 6f), ModContent.ProjectileType<SylvanSlash>(),
                    (int)(Projectile.damage * 0.5f), Projectile.knockBack, Projectile.owner);
            }
        }
    }

    /// <summary>Vague d'épée de la Sylvan Slasher (« Sylvan Backslash »), ralentit à chaque ennemi touché, 2 ennemis.</summary>
    public class SylvanSlash : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = 2;
            Projectile.alpha = 120;
            Projectile.timeLeft = 200;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item92, Projectile.position);
                Projectile.localAI[0] = 1f;
            }
            Lighting.AddLight(Projectile.Center, 0.1f, 1f, 2f);
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi) + MathHelper.PiOver2 * Projectile.direction;
            if (Main.rand.NextBool(2))
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 111, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > 195)
                return false;
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 2);
            return false;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, Projectile.alpha);

        public override void OnKill(int timeLeft)
        {
            for (int k = 0; k < 3; k++)
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 111, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => Projectile.velocity *= 0.5f;
    }
}
