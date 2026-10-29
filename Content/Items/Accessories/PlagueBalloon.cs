using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Items.Accessories
{
    /// <summary>
    /// Wiki : Plague Balloon.txt — ballon cosmétique inventé par le wiki, lâché par le Plaguebringer Goliath (WikiLoot) :
    /// se porte dans n'importe quel emplacement de covering ; à la mort, il éclate en abeilles de la peste purement
    /// visuelles. Rareté 8, vente 2 or. [choix] Image portée = l'image de l'objet (le wiki n'en a pas d'autre) ; abeilles =
    /// abeilles vanilla teintées en vert, qui ne touchent rien (retour du 30/10 : la poussière seule ne se voyait pas).
    /// </summary>
    [AutoloadEquip(EquipType.Balloon)]
    public class PlagueBalloon : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 44;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 2);
            Item.accessory = true;
            Item.vanity = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual) => player.GetModPlayer<PlagueBalloonPlayer>().Worn = true;

        public override void UpdateVanity(Player player) => player.GetModPlayer<PlagueBalloonPlayer>().Worn = true;
    }

    public class PlagueBalloonPlayer : ModPlayer
    {
        public bool Worn;

        public override void ResetEffects() => Worn = false;

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (!Worn)
                return;
            Vector2 balloon = Player.Top - new Vector2(0f, 26f);
            if (!Main.dedServ)
            {
                SoundEngine.PlaySound(SoundID.Item16, balloon);
                for (int i = 0; i < 20; i++)
                {
                    var dust = Dust.NewDustDirect(balloon - new Vector2(12f, 15f), 24, 30, DustID.GreenFairy);
                    dust.velocity = Main.rand.NextVector2Circular(6f, 6f);
                    dust.noGravity = true;
                    dust.scale = Main.rand.NextFloat(0.9f, 1.4f);
                }
            }
            // Nuée d'abeilles visuelles, lancée par le joueur mort (synchronisée comme tout projectile)
            if (Player.whoAmI == Main.myPlayer)
                for (int i = 0; i < 12; i++)
                    Projectile.NewProjectile(Player.GetSource_Death(), balloon, Main.rand.NextVector2Circular(5f, 5f), ModContent.ProjectileType<PlagueBalloonBee>(), 0, 0f, Player.whoAmI);
        }
    }

    /// <summary>Abeille de la peste du Plague Balloon : purement visuelle, vole au hasard puis disparaît.</summary>
    public class PlagueBalloonBee : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.Bee;

        public override void SetStaticDefaults() => Main.projFrames[Type] = 3;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            Vector2 v = Projectile.velocity + Main.rand.NextVector2Circular(0.6f, 0.6f);
            Projectile.velocity = v.SafeNormalize(Vector2.UnitX) * MathHelper.Clamp(v.Length(), 1.5f, 6f);
            Projectile.spriteDirection = Projectile.velocity.X > 0 ? 1 : -1;
            Projectile.rotation = Projectile.velocity.X * 0.1f;
            if (++Projectile.frameCounter > 3)
            {
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
                Projectile.frameCounter = 0;
            }
            if (Projectile.timeLeft < 30)
                Projectile.alpha = (int)(255 * (1f - Projectile.timeLeft / 30f));
        }

        // Teinte verte de la peste
        public override Color? GetAlpha(Color lightColor) => new Color(140, 255, 110) * ((255 - Projectile.alpha) / 255f) * (lightColor.A / 255f);
    }
}
