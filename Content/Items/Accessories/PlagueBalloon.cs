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
    /// visuelles. Rareté 8, vente 2 or. [choix] Image portée = l'image de l'objet (le wiki n'en a pas d'autre), abeilles =
    /// nuée de poussière verte.
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
            if (!Worn || Main.dedServ)
                return;
            SoundEngine.PlaySound(SoundID.Item16, Player.Center);
            for (int i = 0; i < 30; i++)
            {
                var dust = Dust.NewDustDirect(Player.Top - new Microsoft.Xna.Framework.Vector2(12f, 40f), 24, 30, DustID.GreenFairy);
                dust.velocity = Main.rand.NextVector2Circular(6f, 6f);
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(0.9f, 1.4f);
            }
        }
    }
}
