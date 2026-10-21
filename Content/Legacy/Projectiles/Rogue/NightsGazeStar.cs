// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Projectiles.Rogue
{
    /// <summary>Étoile de Night's Gaze. ai[0] = 1 : à tête chercheuse (furtivité) ; sinon ralentit après ai[1] ticks.</summary>
    public class NightsGazeStar : ModProjectile
    {
        public const int Lifetime = 150;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 5;
            Projectile.timeLeft = Lifetime;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.localAI[0] = 10f;
        }

        public override void AI()
        {
            Projectile.rotation += Projectile.direction * 0.05f;

            if (Projectile.ai[0] == 0f)
            {
                if (Projectile.timeLeft < Lifetime - Projectile.ai[1] && Projectile.localAI[0] >= 0)
                {
                    Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * Projectile.localAI[0];
                    Projectile.localAI[0]--;
                }
                else if (Projectile.timeLeft >= Lifetime - Projectile.ai[1])
                {
                    for (int i = 0; i < 3; i++)
                    {
                        int dust = Dust.NewDust(Projectile.Center, 1, 1, Utils.SelectRandom(Main.rand, 109, 111, 132),
                            Projectile.velocity.X, Projectile.velocity.Y, 0, default, 1.5f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= Main.rand.NextFloat(0f, 0.5f);
                    }
                }
            }
            else if (Projectile.ai[0] == 1f)
            {
                NPC target = Projectile.Center.ClosestNPCAt(999f);
                if (target != null)
                    Projectile.velocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitY) * 10f;
            }

            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 20 + Main.rand.Next(40);
                if (Main.rand.NextBool(5))
                    SoundEngine.PlaySound(SoundID.Item9, Projectile.position);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Nightwither>(), 120);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<Nightwither>(), 120);

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            Projectile.Kill();
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], Color.White, 1);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                int dust = Dust.NewDust(Projectile.Center, 1, 1, Utils.SelectRandom(Main.rand, 109, 111, 132),
                    Projectile.velocity.X, Projectile.velocity.Y, 0, default, 1.5f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
