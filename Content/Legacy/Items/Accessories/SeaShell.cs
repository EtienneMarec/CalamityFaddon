// Adapté de Calamity Mod (retiré le 11/06/2022, commit 538b602700), © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.Accessories
{
    /// <summary>
    /// Wiki : Sea Shell.txt — covering retiré de Calamity avant 2.0.1.005 : 3 de défense ; dans un liquide, +3 de défense,
    /// +5 % de réduction des dégâts, +15 % de vitesse (wiki ; 10 % dans le vieux code) et déplacement libre. Lâché par les
    /// Sea Snail (WikiLoot). Rareté 2, vente 40 argent.
    /// </summary>
    public class SeaShell : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.defense = 3;
            Item.width = 20;
            Item.height = 24;
            Item.value = Item.sellPrice(silver: 40);
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!player.wet)
                return;
            player.statDefense += 3;
            player.endurance += 0.05f;
            player.moveSpeed += 0.15f;
            player.ignoreWater = true;
        }
    }
}
