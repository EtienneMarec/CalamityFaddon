// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
using System;
using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Projectiles.Magic
{
    /// <summary>
    /// Pulsation du Wrath of the Ancients : flotte sur place 20 s, libère 8 GraniteEnergy toutes les 50 ticks ; une seule
    /// par joueur (la plus ancienne s'efface quand on en pose une autre). Ne fait pas de dégâts elle-même.
    /// </summary>
    public class GranitePulse : ModProjectile
    {
        private bool initialized;

        public override void SetStaticDefaults() => Main.projFrames[Type] = 6;

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 46;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1200;
        }

        public override void AI()
        {
            Projectile.velocity = new Vector2(0f, (float)Math.Sin(MathHelper.TwoPi * Projectile.ai[0] / 300f) * 0.5f);
            if (++Projectile.ai[0] >= 300f)
            {
                Projectile.ai[0] = 0f;
                Projectile.netUpdate = true;
            }

            if (++Projectile.frameCounter > 4)
            {
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
                Projectile.frameCounter = 0;
            }
            Projectile.ai[1] += 1f;
            if (Projectile.ai[1] >= 7200f)
            {
                Projectile.alpha += 5;
                if (Projectile.alpha > 255)
                {
                    Projectile.alpha = 255;
                    Projectile.Kill();
                }
            }

            // Une seule pulsation par joueur : la plus ancienne commence à disparaître
            if (++Projectile.localAI[0] >= 10f)
            {
                Projectile.localAI[0] = 0f;
                int count = 0, oldest = 0;
                float oldestAge = 0f;
                foreach (var proj in Main.ActiveProjectiles)
                {
                    if (proj.owner == Projectile.owner && proj.type == Type && proj.ai[1] < 3600f)
                    {
                        count++;
                        if (proj.ai[1] > oldestAge)
                        {
                            oldest = proj.whoAmI;
                            oldestAge = proj.ai[1];
                        }
                    }
                }
                if (count > 1)
                {
                    Main.projectile[oldest].netUpdate = true;
                    Main.projectile[oldest].ai[1] = 36000f;
                    return;
                }
            }

            if (!initialized)
            {
                Burst();
                initialized = true;
            }
            if (Projectile.timeLeft % 50 == 1 && Projectile.alpha <= 0)
            {
                Burst();
                if (Projectile.owner == Main.myPlayer)
                {
                    float spread = 45f * 0.0174f;
                    double startAngle = Math.Atan2(Projectile.velocity.X, Projectile.velocity.Y) - spread / 2;
                    double deltaAngle = spread / 8f;
                    for (int i = 0; i < 4; i++)
                    {
                        double offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                        Vector2 v = new Vector2((float)Math.Sin(offsetAngle), (float)Math.Cos(offsetAngle)) * 5f;
                        int energy = ModContent.ProjectileType<GraniteEnergy>();
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, v, energy, Projectile.damage, Projectile.knockBack, Projectile.owner);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, -v, energy, Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                }
            }
        }

        /// <summary>Son et poussière d'une pulsation.</summary>
        private void Burst()
        {
            SoundEngine.PlaySound(SoundID.NPCHit53, Projectile.Center);
            Projectile.ExpandHitboxBy(20);
            for (int d = 0; d < 5; d++)
            {
                var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 229, 0f, 0f, 100, default, 0.5f);
                dust.velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    dust.scale = 0.5f;
                    dust.fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            for (int d = 0; d < 10; d++)
            {
                var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 206, 0f, 0f, 100, default, 1f);
                dust.noGravity = true;
                dust.velocity *= 5f;
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 206, 0f, 0f, 100, default, 0.5f).velocity *= 2f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            int height = texture.Height / Main.projFrames[Type];
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                new Rectangle(0, height * Projectile.frame, texture.Width, height), Projectile.GetAlpha(lightColor), Projectile.rotation,
                new Vector2(texture.Width / 2f, height / 2f), Projectile.scale, SpriteEffects.None);
            return false;
        }

        public override bool? CanDamage() => false;
    }

    /// <summary>Esprit du Wrath of the Ancients : file droit 30 ticks puis cherche un ennemi (600 px).</summary>
    public class GraniteEnergy : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 90;
        }

        public override bool? CanHitNPC(NPC target) => Projectile.timeLeft < 60 && target.CanBeChasedBy(Projectile) ? null : false;

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Main.rand.NextBool())
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 229, 0f, 0f, 100, default, 0.6f);
            if (Projectile.timeLeft < 60)
                CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 600f, 12f, 20f);
        }
    }
}
