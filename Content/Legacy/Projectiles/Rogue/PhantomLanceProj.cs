// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using CalamityMod;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Projectiles.Rogue
{
    /// <summary>Javelot du Phantom Lance : une âme (Phantom) toutes les 18 ticks, dégâts selon le temps restant.</summary>
    public class PhantomLanceProj : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Legacy/Items/Weapons/Rogue/PhantomLance";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 1;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }

        public override void AI()
        {
            bool stealth = Projectile.Calamity().stealthStrike;
            if (!stealth)
            {
                if (Projectile.timeLeft <= 255)
                    Projectile.alpha += 1;
                if (Projectile.timeLeft >= 75)
                    Projectile.velocity *= 0.995f;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 175, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 0, default, 0.85f);

            if (++Projectile.ai[0] % 18f == 0f && Projectile.owner == Main.myPlayer)
            {
                float damageMult = stealth ? 0.7f : Projectile.timeLeft * 0.7f / 300f;
                int soul = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<Phantom>(),
                    (int)(Projectile.damage * damageMult), Projectile.knockBack, Projectile.owner);
                if (soul.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[soul].DamageType = ModContent.GetInstance<RogueDamageClass>();
                    Main.projectile[soul].usesLocalNPCImmunity = true;
                    Main.projectile[soul].localNPCHitCooldown = -2;
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i <= 10; i++)
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 175, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
        }
    }
}
