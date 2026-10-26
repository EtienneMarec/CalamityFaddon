// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Projectiles.Melee
{
    /// <summary>Décharge du Eutrophic Scimitar : arrêtée par les blocs, Eutrophication 0,5 s (infobox du wiki ; 1 s dans le code de 2.0.1.005).</summary>
    public class EutrophicScimitarProj : ModProjectile
    {
        public override void SetStaticDefaults() => Main.projFrames[Type] = 3;

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(127, 255, 253);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Eutrophication>(), 30);

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0f, (255 - Projectile.alpha) * 0.35f / 255f, (255 - Projectile.alpha) * 0.35f / 255f);
            if (++Projectile.frameCounter > 2)
            {
                Projectile.frame = (Projectile.frame + 1) % 3;
                Projectile.frameCounter = 0;
            }
            if (Main.rand.NextBool())
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 226, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 0, Color.White, 0.5f);
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i <= 10; i++)
            {
                var dust = Dust.NewDustDirect(Projectile.position - new Vector2(4), 58, 58, 226, 0f, 0f, 0, Color.White, 0.46f);
                dust.noGravity = true;
                dust.fadeIn = 0.95f;
            }
        }
    }
}
