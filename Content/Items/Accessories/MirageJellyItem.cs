using CalamityMod;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Items.Accessories
{
    /// <summary>
    /// Wiki : Mirage Jelly _item_.txt — covering inventé par le wiki, lâché par les Mirage Jelly (WikiLoot) : les attaques
    /// furtives coûtent 5 % de furtivité en moins, cumulable avec la Coin of Deceit. Rareté 7, vente 1 or. Sert à la
    /// Grand Gelatin. [choix] Fait par la réduction de perte de furtivité de Calamity (flatStealthLossReduction) : le seuil
    /// pour déclencher l'attaque furtive ne change pas, seule la furtivité consommée baisse.
    /// </summary>
    public class MirageJellyItem : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 32;
            Item.rare = ItemRarityID.Lime;
            Item.value = Item.sellPrice(gold: 1);
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual) => player.GetModPlayer<MirageJellyPlayer>().Equipped = true;
    }

    public class MirageJellyPlayer : ModPlayer
    {
        public bool Equipped;

        public override void ResetEffects() => Equipped = false;

        // Après les armures (qui fixent la furtivité max) : 5 % de la furtivité max, en points (x100)
        public override void PostUpdateEquips()
        {
            if (Equipped)
                Player.Calamity().flatStealthLossReduction += (int)(Player.Calamity().rogueStealthMax * 100f * 0.05f);
        }
    }
}
