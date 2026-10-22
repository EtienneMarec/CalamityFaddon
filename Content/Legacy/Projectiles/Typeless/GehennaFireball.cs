// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Projectiles.Typeless
{
    /// <summary>Boule de feu du Gehenna (BrimstoneHellfireballFriendly2) : explose en GehennaExplosion à sa mort.</summary>
    public class GehennaFireball : ModProjectile
    {
        /// <summary>Brimstone Flames : 10 secondes (infobox de Gehenna).</summary>
        public const int FlamesTime = 600;

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 50;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.5f, 0.05f, 0.05f);
            if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item20, Projectile.position);
                Projectile.localAI[0] = 1f;
            }
            for (int i = 0; i < 5; i++)
            {
                var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 0.6f);
                dust.noGravity = true;
                dust.velocity = dust.velocity * 0.5f + Projectile.velocity * 0.1f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GehennaExplosion>(),
                    Projectile.damage, Projectile.knockBack, Projectile.owner);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), FlamesTime);
    }

    /// <summary>Explosion persistante de soufre (HellfireExplosionFriendly2), « comme l'Inferno Fork » selon le wiki.</summary>
    public class GehennaExplosion : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 130;
            Projectile.height = 130;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.75f, 0.05f, 0.05f);
            if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item20, Projectile.position);
                Projectile.localAI[0] = 1f;
            }
            float strength = 25f;
            if (Projectile.ai[0] > 180f)
                strength -= (Projectile.ai[0] - 180f) / 2f;
            if (strength <= 0f)
            {
                Projectile.Kill();
                return;
            }
            Projectile.ai[0] += 4f;
            for (int i = 0; i < strength * 0.7f; i++)
            {
                Vector2 velocity = Vector2.Normalize(new Vector2(Main.rand.Next(-15, 16), Main.rand.Next(-15, 16)) + new Vector2(0.01f)) * Main.rand.Next(5, 12);
                var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 0.75f);
                dust.noGravity = true;
                dust.position = Projectile.Center + new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11));
                dust.velocity = velocity;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), GehennaFireball.FlamesTime);
    }
}
