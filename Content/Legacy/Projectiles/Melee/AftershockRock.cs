// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Projectiles.Melee
{
    /// <summary>Rocher de l'Aftershock : roule comme celui du Staff of Earth, 6 coups (perce 5 ennemis).</summary>
    public class AftershockRock : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 34;
            Projectile.aiStyle = ProjAIStyleID.GroundProjectile;
            AIType = ProjectileID.BoulderStaffOfEarth;
            Projectile.friendly = true;
            Projectile.penetrate = 6;
            Projectile.extraUpdates = 1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }
    }
}
