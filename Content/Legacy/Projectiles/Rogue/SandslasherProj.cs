// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using CalamityMod;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Projectiles.Rogue
{
    /// <summary>
    /// Shuriken du Sandslasher : +0,07 de vitesse horizontale par tick, dégâts augmentés de 2x la vitesse toutes les
    /// 30 ticks ; attaque furtive : un nuage de sable toutes les 5 ticks.
    /// </summary>
    public class SandslasherProj : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Legacy/Items/Weapons/Rogue/Sandslasher";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 16;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            Projectile.ai[0] += 1f;
            Projectile.ai[1] += 1f;
            if (Projectile.ai[0] == 3f)
                Projectile.tileCollide = true;
            if (Projectile.velocity.X != 0f)
            {
                Projectile.velocity.X += 0.07f * System.Math.Sign(Projectile.velocity.X);
                if ((Projectile.ai[0] %= 30f) == 0f)
                    Projectile.damage += (int)(System.Math.Abs(Projectile.velocity.X) * 2f);
            }
            Projectile.rotation += 0.1f * Projectile.direction + Projectile.velocity.X / 85;
            if (Projectile.Calamity().stealthStrike && Projectile.ai[1] >= 5f)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    Vector2 speed = new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f));
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, speed, ModContent.ProjectileType<DuststormCloud>(),
                        (int)(Projectile.damage * 0.4), 0f, Projectile.owner);
                }
                Projectile.ai[1] = 0;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 85, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }
    }
}
