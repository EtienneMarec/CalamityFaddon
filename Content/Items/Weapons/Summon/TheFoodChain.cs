using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Items.Weapons.Summon
{
    /// <summary>
    /// Wiki : The Food Chain.txt — fouet lâché par les Colossal Squids : frapper un ennemi donne Well Fed 10 s au joueur.
    /// Le fouet fait de la nourriture (« raw squid rings ») : planche des segments provisoire (docs/SPRITES.md).
    /// </summary>
    public class TheFoodChain : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.DefaultToWhip(ModContent.ProjectileType<Projectiles.Summon.TheFoodChainProj>(), 95, 7f, 4f, 15);
            Item.width = 26;
            Item.height = 40;
            Item.autoReuse = true;
            Item.value = Item.sellPrice(gold: 12);
            Item.rare = ModContent.RarityType<CalamityMod.Rarities.CosmicPurple>();
        }

        public override bool MeleePrefix() => true;
    }
}

namespace CalamityFandom.Content.Projectiles.Summon
{
    /// <summary>Fouet de The Food Chain (dessin des segments repris de l'ExampleWhip de tModLoader).</summary>
    public class TheFoodChainProj : ModProjectile
    {
        public override void SetStaticDefaults() => ProjectileID.Sets.IsAWhip[Type] = true;

        public override void SetDefaults()
        {
            Projectile.DefaultToWhip();
            Projectile.WhipSettings.Segments = 18;
            Projectile.WhipSettings.RangeMultiplier = 1.1f;
        }

        private float Timer => Projectile.ai[0];

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player owner = Main.player[Projectile.owner];
            owner.MinionAttackTargetNPC = target.whoAmI;
            // « Striking enemies will apply the Well Fed buff to the player » (10 s)
            owner.AddBuff(BuffID.WellFed, 10 * 60);
            Projectile.damage = (int)(Projectile.damage * 0.8f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var points = new List<Vector2>();
            Projectile.FillWhipControlPoints(Projectile, points);
            SpriteEffects flip = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 pos = points[0];
            for (int i = 0; i < points.Count - 1; i++)
            {
                // Planche : manche (0, 26 px), maillons (26, 42, 58 ; 16 px), pointe (74, 18 px)
                var frame = new Rectangle(0, 0, 10, 26);
                var origin = new Vector2(5, 8);
                float scale = 1f;
                if (i == points.Count - 2)
                {
                    frame.Y = 74;
                    frame.Height = 18;
                    Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out int _, out float _);
                    float t = Timer / timeToFlyOut;
                    scale = MathHelper.Lerp(0.5f, 1.5f, Utils.GetLerpValue(0.1f, 0.7f, t, true) * Utils.GetLerpValue(0.9f, 0.7f, t, true));
                }
                else if (i > 10)
                {
                    frame.Y = 58;
                    frame.Height = 16;
                }
                else if (i > 5)
                {
                    frame.Y = 42;
                    frame.Height = 16;
                }
                else if (i > 0)
                {
                    frame.Y = 26;
                    frame.Height = 16;
                }
                Vector2 diff = points[i + 1] - points[i];
                Color color = Lighting.GetColor(points[i].ToTileCoordinates());
                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, diff.ToRotation() - MathHelper.PiOver2, origin, scale, flip);
                pos += diff;
            }
            return false;
        }
    }
}
