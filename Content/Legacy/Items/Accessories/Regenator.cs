// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.Accessories
{
    /// <summary>
    /// Wiki : Regenator.txt — covering retiré de Calamity : vie max divisée par deux, régénération bien plus forte.
    /// L'effet a disparu du CalamityPlayer actuel : repris ici tel qu'en 2.0.1.005 (vie max x0,5, lifeRegen +12,
    /// lifeRegenTime +6). Défense 10, rareté 7, vente 9 or 60 argent (infobox). Lâché par Calamitas (wiki).
    /// </summary>
    public class Regenator : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 56;
            Item.value = Item.sellPrice(gold: 9, silver: 60);
            Item.rare = ItemRarityID.Lime;
            Item.defense = 10;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual) => player.GetModPlayer<RegenatorPlayer>().Active = true;
    }

    public class RegenatorPlayer : ModPlayer
    {
        public bool Active;

        public override void ResetEffects() => Active = false;

        public override void PostUpdateEquips()
        {
            if (Active)
                Player.statLifeMax2 = (int)(Player.statLifeMax2 * 0.5);
        }

        public override void UpdateLifeRegen()
        {
            if (!Active)
                return;
            Player.lifeRegenTime += 6;
            Player.lifeRegen += 12;
        }
    }
}
