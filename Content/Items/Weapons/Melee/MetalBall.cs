using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Metal Ball.txt — fléau inventé par le wiki, lâché par le Plaguebringer Goliath (WikiLoot) : une boule de
    /// nanométal pesante, retenue par une chaîne, qui suit les mouvements du joueur (Samson's Chains de The Binding of
    /// Isaac, Trivia) ; immobile, elle finit sous le joueur. Plague 3 s. Valeurs du wiki : 162 dégâts, recul 6, use 25,
    /// vélocité 3, rareté 8, vente 6 or. [choix] Boule maintenue tant que le bouton est tenu ; chaîne de 160 px ; pendule
    /// simple (gravité 0,35, amortissement 0,99).
    /// </summary>
    public class MetalBall : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 32;
            Item.damage = 162;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.knockBack = 6f;
            Item.value = Item.sellPrice(gold: 6);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<MetalBallProj>();
            Item.shootSpeed = 3f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] == 0;
    }

    public class MetalBallProj : ModProjectile
    {
        private const float ChainLength = 160f;

        public override string Texture => "CalamityFandom/Content/Items/Weapons/Melee/MetalBall";

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!owner.active || owner.dead || !owner.channel || owner.HeldItem.type != ModContent.ItemType<MetalBall>())
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 2;
            owner.itemTime = owner.itemAnimation = 2;
            owner.heldProj = Projectile.whoAmI;

            // Pendule : gravité, amortissement, chaîne qui ne s'allonge pas
            Projectile.velocity.Y += 0.35f;
            Projectile.velocity *= 0.99f;
            Vector2 anchor = owner.MountedCenter;
            Vector2 offset = Projectile.Center - anchor;
            if (offset.Length() > ChainLength)
            {
                Vector2 dir = offset.SafeNormalize(Vector2.UnitY);
                Projectile.Center = anchor + dir * ChainLength;
                float outward = Vector2.Dot(Projectile.velocity - owner.velocity, dir);
                if (outward > 0f)
                    Projectile.velocity -= dir * outward;
            }
            Projectile.rotation += Projectile.velocity.X * 0.05f;
            owner.ChangeDir(Projectile.Center.X > owner.Center.X ? 1 : -1);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X * 0.5f;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y * 0.3f;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Plague>(), 180);

        public override bool PreDraw(ref Color lightColor)
        {
            // Chaîne : maillons de la chaîne de vanilla entre le joueur et la boule
            Texture2D chain = Terraria.GameContent.TextureAssets.Chain.Value;
            Vector2 start = Main.player[Projectile.owner].MountedCenter;
            Vector2 toBall = Projectile.Center - start;
            float rotation = toBall.ToRotation() - MathHelper.PiOver2;
            for (float d = 0f; d < toBall.Length(); d += chain.Height)
            {
                Vector2 pos = start + toBall.SafeNormalize(Vector2.Zero) * d;
                Main.EntitySpriteDraw(chain, pos - Main.screenPosition, null, Lighting.GetColor(pos.ToTileCoordinates()), rotation,
                    chain.Size() / 2f, 1f, SpriteEffects.None);
            }
            return true;
        }
    }
}
