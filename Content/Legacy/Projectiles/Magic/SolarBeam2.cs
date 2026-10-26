// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
using CalamityMod;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Projectiles.Magic
{
    /// <summary>
    /// Rayon du Lazhar : invisible (traînée de poussière), rebondit 5 fois sur les blocs, explose sur l'ennemi touché
    /// (explosion FuckYou de Calamity, rendue magique). [choix] Le wiki dit que l'explosion inflige On Fire! 3 s ;
    /// l'explosion de Calamity ne le fait pas : le debuff est donné par le rayon à l'ennemi touché.
    /// </summary>
    public class SolarBeam2 : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 6;
            Projectile.extraUpdates = 100;
            Projectile.timeLeft = 300;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (--Projectile.penetrate <= 0)
                Projectile.Kill();
            else
            {
                if (Projectile.velocity.X != oldVelocity.X)
                    Projectile.velocity.X = -oldVelocity.X;
                if (Projectile.velocity.Y != oldVelocity.Y)
                    Projectile.velocity.Y = -oldVelocity.Y;
            }
            return false;
        }

        public override void AI()
        {
            if (++Projectile.localAI[0] <= 4f)
                return;
            Projectile.alpha = 255;
            for (int i = 0; i < 4; i++)
            {
                var dust = Dust.NewDustDirect(Projectile.position, 1, 1, 246);
                dust.position = Projectile.position - Projectile.velocity * (i * 0.25f);
                dust.scale = Main.rand.Next(70, 110) * 0.013f;
                dust.velocity *= 0.2f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.owner] = 4;
            target.AddBuff(BuffID.OnFire, 180);
            if (Projectile.owner == Main.myPlayer)
            {
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FuckYou>(),
                    Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
                if (proj.WithinBounds(Main.maxProjectiles))
                    Main.projectile[proj].DamageType = DamageClass.Magic;
            }
        }
    }
}
