// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
using CalamityFandom.Content.Legacy.Buffs.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Projectiles.Summon
{
    /// <summary>
    /// Petit slime sbire (IA du Baby Slime : saute, attaque au contact, s'envole avec des ailes s'il est trop loin).
    /// Vit tant que le joueur a le buff ; poussière de couleur à l'apparition.
    /// </summary>
    public abstract class SlimeMinion<TBuff> : ModProjectile where TBuff : ModBuff
    {
        protected abstract int SpawnDust { get; }

        private bool spawned;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 6;
            Main.projPet[Type] = true;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.minionSlots = 1;
            Projectile.alpha = 75;
            Projectile.aiStyle = ProjAIStyleID.Pet;
            AIType = ProjectileID.BabySlime;
            Projectile.timeLeft = 18000 * 5;
            Projectile.penetrate = -1;
            Projectile.minion = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 23;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }

        public override bool MinionContactDamage() => true;

        public override void AI()
        {
            if (!spawned)
            {
                spawned = true;
                for (int i = 0; i < 16; i++)
                {
                    Vector2 offset = (Vector2.UnitY * Projectile.height * 0.75f).RotatedBy(MathHelper.TwoPi * i / 16f);
                    var dust = Dust.NewDustDirect(Projectile.Center + offset * 2f, 0, 0, SpawnDust, offset.X, offset.Y, 100, default, 1.1f);
                    dust.noGravity = true;
                    dust.noLight = true;
                    dust.velocity = offset;
                }
            }
            Player player = Main.player[Projectile.owner];
            if (player.dead || !player.active)
                player.ClearBuff(ModContent.BuffType<TBuff>());
            if (player.HasBuff(ModContent.BuffType<TBuff>()))
                Projectile.timeLeft = 2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            int frameHeight = texture.Height / Main.projFrames[Type];
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                new Rectangle(0, frameHeight * Projectile.frame, texture.Width, frameHeight), Projectile.GetAlpha(lightColor),
                Projectile.rotation, new Vector2(texture.Width / 2f, frameHeight / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;
    }

    public class CrimslimeMinion : SlimeMinion<Crimslime>
    {
        protected override int SpawnDust => DustID.Blood;
    }

    public class CorroslimeMinion : SlimeMinion<Corroslime>
    {
        protected override int SpawnDust => DustID.Corruption;
    }
}
