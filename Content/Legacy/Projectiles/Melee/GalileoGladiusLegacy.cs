// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
using System;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Projectiles.Melee
{
    /// <summary>
    /// Wiki : Galileo Gladius.txt — estoc du Galileo Gladius de 2.0.1.005 (Full Fandom, Common/Legacy/FullFandomLegacyWeapons) :
    /// tire un croissant de lune à tête chercheuse (un tiers des dégâts) ; un coup direct fait pleuvoir des planètes
    /// (toutes les 15 frames au plus) et inflige Moon Burnt 5 s.
    /// </summary>
    public class GalileoGladiusProj : BaseShortswordProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Melee/GalileoGladius";

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(24);
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Projectile.ownerHitCheck = true;
            Projectile.timeLeft = 360;
            Projectile.extraUpdates = 1;
            Projectile.hide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override Action<Projectile> EffectBeforePullback => proj =>
        {
            int moonDamage = (int)(Projectile.damage * 0.3333f);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 10f,
                ModContent.ProjectileType<GalileosMoon>(), moonDamage, Projectile.knockBack, Projectile.owner);
        };

        public override void SetVisualOffsets()
        {
            const int HalfSpriteWidth = 42 / 2;
            const int HalfSpriteHeight = 46 / 2;
            DrawOriginOffsetX = 0;
            DrawOffsetX = -(HalfSpriteWidth - Projectile.width / 2);
            DrawOriginOffsetY = -(HalfSpriteHeight - Projectile.height / 2);
        }

        public override void ExtraBehavior()
        {
            if (Main.rand.NextBool(5))
            {
                var dust = Dust.NewDustDirect(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, Main.rand.NextBool() ? 20 : 176,
                    Main.player[Projectile.owner].direction * 2f, 0f, 150, default, 1.3f);
                dust.velocity *= 0.2f;
                dust.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Nightwither>(), 300);
            GalileoPlanetPlayer.TryRain(Main.player[Projectile.owner], Projectile.GetSource_FromThis(), target.Center, Projectile.damage * 2);
        }
    }

    /// <summary>Délai entre deux pluies de planètes (galileoCooldown de 2.0.1.005, retiré de Calamity).</summary>
    public class GalileoPlanetPlayer : ModPlayer
    {
        public int Cooldown;

        public override void PostUpdate()
        {
            if (Cooldown > 0)
                Cooldown--;
        }

        /// <summary>Fait tomber une planète sur la cible, au plus une fois toutes les 15 frames.</summary>
        public static void TryRain(Player player, Terraria.DataStructures.IEntitySource source, Vector2 target, int damage, int type = -1)
        {
            var modPlayer = player.GetModPlayer<GalileoPlanetPlayer>();
            if (player.whoAmI != Main.myPlayer || modPlayer.Cooldown > 0)
                return;
            if (type <= 0)
                type = ModContent.ProjectileType<GalileosPlanet>();
            CalamityUtils.ProjectileRain(source, target, 400f, 100f, 500f, 800f, 25f, type, damage, 15f, player.whoAmI);
            modPlayer.Cooldown = 15;
        }
    }

    /// <summary>Croissant de lune à tête chercheuse (version 2.0.1.005 : perce 12 fois, Moon Burnt 3 s comme dit la page).</summary>
    public class GalileosMoon : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/CrescentMoonProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.alpha = 100;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.penetrate = 12;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 135 * Projectile.MaxUpdates;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = Projectile.MaxUpdates * 10;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0f, 0f, 0.6f);
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 20 + Main.rand.Next(40);
                if (Main.rand.NextBool(5))
                    SoundEngine.PlaySound(SoundID.Item9, Projectile.position);
            }
            Projectile.rotation += Projectile.direction * 0.55f;
            CalamityUtils.HomeInOnNPC(Projectile, true, 250f, 10f, 25f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Nightwither>(), 180);
    }

    /// <summary>Planète qui tombe du ciel et explose (au sol ou sur un ennemi) : moitié des dégâts en zone, Moon Burnt 5 s (page).</summary>
    public class GalileosPlanet : ModProjectile
    {
        private float radius = 84f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 88;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            // Traînée de poussière bleue de part et d'autre de la planète
            Vector2 tail = Projectile.position + new Vector2(Projectile.width / 2 - 8, Projectile.height + 16);
            for (float i = -5; i <= 5; i++)
            {
                if (Main.rand.NextFloat() < 0.4f)
                    Dust.NewDustPerfect(tail + new Vector2((i < 0 ? -20 : 20) + i, -2), 180, new Vector2(i * 0.7f, -1.1f)).noGravity = true;
            }
            Dust.NewDust(Projectile.Center - new Vector2(20, 20), 40, 40, 180);
            Lighting.AddLight(Projectile.Center, 0.3f, 0.3f, 0.7f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            // Explosion : zone de 100 px, moitié des dégâts
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 32;
            Projectile.position -= Projectile.Size / 2f;
            radius = 100f;
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.damage /= 2;
            Projectile.Damage();

            for (int i = 0; i < 72; i++)
            {
                var dust = Dust.NewDustDirect(Projectile.Center - new Vector2(30, 30), 60, 60, 187, Main.rand.Next(-5, 5), Main.rand.Next(-5, 5), 0, new Color(0, 167, 255), 5f);
                dust.noGravity = true;
                dust.shader = GameShaders.Armor.GetSecondaryShader(47, Main.LocalPlayer);
            }
            SoundEngine.PlaySound(SoundID.NPCDeath43, Projectile.position);
            for (int i = 0; i < 155; i++)
                Dust.NewDustDirect(Projectile.position, 94, 94, 180, Main.rand.Next(-5, 5), Main.rand.Next(-5, 5), 0, Color.White, Main.rand.NextFloat(1.2f, 2.2f)).noGravity = true;
            // Deux anneaux de poussière
            for (int ring = 1; ring <= 3; ring++)
            {
                const int Points = 36;
                for (int i = 0; i < Points; i++)
                {
                    Vector2 offset = (Vector2.Normalize(Projectile.velocity) * new Vector2(Projectile.width / 2f, Projectile.height) * 0.375f)
                        .RotatedBy((i - (Points / 2 - 1)) * MathHelper.TwoPi / Points);
                    var dust = Dust.NewDustDirect(Projectile.Center + offset * 2f, 0, 0, 180, 0f, 0f, 0, Color.White, 1f);
                    dust.noGravity = true;
                    dust.velocity = Vector2.Normalize(offset) * ring * 2f;
                }
            }
            if (Main.netMode != NetmodeID.Server)
            {
                Vector2 source = Projectile.Center - new Vector2(24f, 24f);
                foreach (var direction in new[] { new Vector2(1f, 1f), new Vector2(-1f, 1f), new Vector2(1f, -1f), new Vector2(-1f, -1f) })
                {
                    var gore = Gore.NewGoreDirect(Projectile.GetSource_Death(), source, default, Main.rand.Next(61, 64));
                    gore.velocity = gore.velocity * 0.66f + direction;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Nightwither>(), 300);

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, radius, targetHitbox);
    }
}
