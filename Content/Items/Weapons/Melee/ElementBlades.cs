using CalamityMod;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Items.Weapons.Melee
{
    /// <summary>
    /// Épées du wiki qui forment la Flarefrost Blade (31/10) : chacune tire une petite orbe à tête chercheuse qui inflige
    /// son debuff 2 s ; la lame l'inflige 3 s. Orbe inspirée du projectile Flarefrost de Calamity, réduite à un élément.
    /// </summary>
    public abstract class ElementBlade : ModItem
    {
        protected abstract int Debuff { get; }
        protected abstract int Orb { get; }
        protected abstract int SwingDust { get; }

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 64;
            Item.height = 66;
            Item.damage = 95;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 23;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.knockBack = 6.25f;
            Item.UseSound = SoundID.Item1;
            Item.shoot = Orb;
            Item.shootSpeed = 11f;
            Item.value = Item.sellPrice(gold: 7, silver: 20);
            Item.rare = ItemRarityID.Pink;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
                Dust.NewDust(hitbox.TopLeft(), hitbox.Width, hitbox.Height, SwingDust);
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(Debuff, 3 * 60);
    }

    /// <summary>Wiki : Frost Blade.txt — « Fires a homing frost orb » (Frostburn).</summary>
    public class FrostBlade : ElementBlade
    {
        protected override int Debuff => BuffID.Frostburn;
        protected override int Orb => ModContent.ProjectileType<Projectiles.Melee.FrostOrb>();
        protected override int SwingDust => 67;
    }

    /// <summary>Wiki : Flare Blade.txt — « Fires a homing flare orb » (On Fire!).</summary>
    public class FlareBlade : ElementBlade
    {
        protected override int Debuff => BuffID.OnFire;
        protected override int Orb => ModContent.ProjectileType<Projectiles.Melee.FlareOrb>();
        protected override int SwingDust => 174;
    }
}

namespace CalamityFandom.Content.Projectiles.Melee
{
    /// <summary>Orbe de poussière à tête chercheuse (3 s), debuff 2 s au contact.</summary>
    public abstract class ElementOrb : ModProjectile
    {
        protected abstract int DustType { get; }
        protected abstract int Debuff { get; }
        protected abstract Vector3 Light { get; }

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Light);
            Projectile.localAI[0] = (Projectile.localAI[0] + 1f) % 48f;
            Vector2 spin = -Vector2.UnitY.RotatedBy(Projectile.localAI[0] * 0.1309f) * new Vector2(5f, 10f);
            Dust dust = Dust.NewDustPerfect(Projectile.Center + spin, DustType, Projectile.velocity, 160, default, 1.3f);
            dust.noGravity = true;
            Dust core = Dust.NewDustPerfect(Projectile.Center, DustType, Vector2.Zero, 100, default, 0.8f);
            core.noGravity = true;
            CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 250f, 12f, 20f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(Debuff, 2 * 60);

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);
            for (int i = 0; i < 4; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
        }
    }

    public class FrostOrb : ElementOrb
    {
        protected override int DustType => 67;
        protected override int Debuff => BuffID.Frostburn;
        protected override Vector3 Light => new(0f, 0.15f, 0.3f);
    }

    public class FlareOrb : ElementOrb
    {
        protected override int DustType => 174;
        protected override int Debuff => BuffID.OnFire;
        protected override Vector3 Light => new(0.3f, 0.1f, 0f);
    }
}
