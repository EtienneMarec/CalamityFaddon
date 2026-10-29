// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Projectiles.Magic
{
    /// <summary>Jet d'eau du Storm Spray : retombe et laisse goutter une traînée (WaterStreamDrip) toutes les 20 ticks.</summary>
    public class WaterStream : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 3;
            Projectile.extraUpdates = 2;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Projectile.ai[1]++;
            if (Projectile.ai[1] >= 4f)
                Projectile.tileCollide = true;
            if (Projectile.timeLeft % 20 == 0 && Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<WaterStreamDrip>(),
                    Projectile.damage, Projectile.knockBack, Projectile.owner);
            Flow(Projectile);
        }

        /// <summary>Mouvement et poussière communs au jet et à ses gouttes.</summary>
        public static void Flow(Projectile projectile)
        {
            Lighting.AddLight(projectile.Center, 0f, 0.15f * (255 - projectile.alpha) / 255f, 0.25f * (255 - projectile.alpha) / 255f);
            projectile.scale -= 0.002f;
            if (projectile.scale <= 0f)
            {
                projectile.Kill();
                return;
            }
            if (projectile.ai[0] <= 3f)
            {
                projectile.ai[0]++;
                return;
            }
            projectile.velocity.Y += 0.075f;
            for (int i = 0; i < 3; i++)
            {
                Vector2 back = projectile.velocity / 3f * i;
                var dust = Dust.NewDustDirect(projectile.position + new Vector2(14), projectile.width - 28, projectile.height - 28, 14, 0f, 0f, 100, new Color(0, 255, 255));
                dust.noGravity = true;
                dust.velocity = dust.velocity * 0.1f + projectile.velocity * 0.5f;
                dust.position -= back;
            }
            if (Main.rand.NextBool(8))
            {
                var dust = Dust.NewDustDirect(projectile.position + new Vector2(16), projectile.width - 32, projectile.height - 32, 14, 0f, 0f, 100, new Color(0, 255, 255), 0.5f);
                dust.velocity = dust.velocity * 0.25f + projectile.velocity * 0.5f;
            }
        }
    }

    /// <summary>Goutte laissée par le jet (WaterStream2 dans le vieux code).</summary>
    public class WaterStreamDrip : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 2;
            Projectile.timeLeft = 80;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI() => WaterStream.Flow(Projectile);
    }
}
