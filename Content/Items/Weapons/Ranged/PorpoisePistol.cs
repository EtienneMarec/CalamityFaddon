using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Items.Weapons.Ranged
{
    /// <summary>
    /// Wiki : Porpoise Pistol.txt — pistolet lâché par les Dolphins en Hardmode : tire un trait d'énergie bleu qui rebondit
    /// sur les blocs et traverse 2 ennemis, en ignorant une partie de leur temps d'invulnérabilité. [choix] Pas de
    /// munition (l'infobox n'en donne pas) ; 3 rebonds au plus ; touche 3 ennemis (traverse les 2 premiers).
    /// </summary>
    public class PorpoisePistol : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 30;
            Item.damage = 90;
            Item.DamageType = DamageClass.Ranged;
            Item.knockBack = 3.5f;
            Item.useTime = Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item12;
            Item.shoot = ModContent.ProjectileType<Projectiles.Ranged.ProporseBolt>();
            Item.shootSpeed = 20f;
            Item.value = Item.sellPrice(gold: 2, silver: 40);
            Item.rare = ItemRarityID.LightRed;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-2f, 0f);
    }
}

namespace CalamityFandom.Content.Projectiles.Ranged
{
    /// <summary>« Proporse Bolt » (sprite du wiki) : rebondit sur les blocs, traverse 2 ennemis.</summary>
    public class ProporseBolt : ModProjectile
    {
        public const int MaxBounces = 3;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Lighting.AddLight(Projectile.Center, 0.1f, 0.35f, 0.6f);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (++Projectile.ai[0] > MaxBounces)
                return true;
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation,
                texture.Size() / 2f, 1f, SpriteEffects.None);
            return false;
        }
    }
}
