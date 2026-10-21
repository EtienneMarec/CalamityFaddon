// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Projectiles.Rogue
{
    /// <summary>Étincelle de Night's Gaze : file puis ralentit jusqu'à disparaître (ai[1] = ticks avant de ralentir).</summary>
    public class NightsGazeSpark : ModProjectile
    {
        public const int Lifetime = 150;

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 5;
            Projectile.timeLeft = Lifetime;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.localAI[0] = 10f;
        }

        public override void AI()
        {
            Projectile.rotation += Projectile.direction * Projectile.ai[0];

            if (Projectile.timeLeft < Lifetime - Projectile.ai[1] && Projectile.localAI[0] >= 0)
            {
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * Projectile.localAI[0];
                Projectile.localAI[0]--;
            }
            if (Projectile.localAI[0] == 0)
                Projectile.Kill();

            for (int i = 0; i < 3; i++)
            {
                int dust = Dust.NewDust(Projectile.Center, 1, 1, Utils.SelectRandom(Main.rand, 109, 111, 132),
                    Projectile.velocity.X, Projectile.velocity.Y, 0, default, 1.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= Main.rand.NextFloat(0f, 0.5f);
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

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                int dust = Dust.NewDust(Projectile.Center, 1, 1, Utils.SelectRandom(Main.rand, 109, 111, 132),
                    Projectile.velocity.X, Projectile.velocity.Y, 0, default, 1.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0.75f;
            }
        }
    }
}
