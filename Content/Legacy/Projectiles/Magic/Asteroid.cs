// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using CalamityMod;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Projectiles.Magic
{
    /// <summary>
    /// Astéroïde de l'Asteroid Staff : traverse les blocs jusqu'à 300 px au-dessus du lanceur, explose au contact (zone de
    /// 128 px x taille). ai[1] = taille.
    /// </summary>
    public class Asteroid : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            if (Projectile.position.Y > Main.player[Projectile.owner].position.Y - 300f || Projectile.position.Y < Main.worldSurface * 16.0)
                Projectile.tileCollide = true;
            Projectile.scale = Projectile.ai[1];
            Projectile.rotation += Projectile.velocity.X * 2f;

            // Traînée verte des deux côtés
            Vector2 front = Projectile.Center + Vector2.Normalize(Projectile.velocity) * 10f;
            foreach (float side in new[] { MathHelper.PiOver2, -MathHelper.PiOver2 })
            {
                var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 107);
                dust.position = front + Projectile.velocity.RotatedBy(side);
                dust.velocity = Projectile.velocity.RotatedBy(side) * 0.33f + Projectile.velocity / 4f;
                dust.fadeIn = 0.5f;
                dust.noGravity = true;
            }
            var core = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 107);
            core.velocity *= 0.5f;
            core.scale *= 1.3f;
            core.fadeIn = 1f;
            core.noGravity = true;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item89, Projectile.position);
            Projectile.ExpandHitboxBy((int)(128f * Projectile.scale));
            for (int i = 0; i < 4; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 31, 0f, 0f, 100, default, 1.5f);
            for (int i = 0; i < 16; i++)
            {
                var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 107, 0f, 0f, 100, default, 2.5f);
                dust.noGravity = true;
                dust.velocity *= 3f;
                dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 107, 0f, 0f, 100, default, 1.5f);
                dust.velocity *= 2f;
                dust.noGravity = true;
            }
            if (Main.netMode != NetmodeID.Server)
            {
                for (int i = 0; i < 2; i++)
                {
                    var gore = Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.position
                        + new Vector2(Projectile.width * Main.rand.Next(100) / 100f, Projectile.height * Main.rand.Next(100) / 100f) - Vector2.One * 10f,
                        default, Main.rand.Next(61, 64));
                    gore.velocity = gore.velocity * 0.3f + new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11)) * 0.05f;
                }
            }
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.localAI[1] = -1f;
                Projectile.maxPenetrate = 0;
                Projectile.Damage();
            }
            for (int i = 0; i < 5; i++)
            {
                var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, Utils.SelectRandom(Main.rand, 107, 259, 158),
                    2.5f * Projectile.direction, -2.5f);
                dust.alpha = 200;
                dust.velocity *= 2.4f;
                dust.scale += Main.rand.NextFloat();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.immune[Projectile.owner] = 2;

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }
    }
}
