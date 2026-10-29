// Adapté de Calamity Mod (retiré le 09/04/2022, commit b3837f516b), © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using CalamityMod.Buffs.StatDebuffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.Accessories
{
    /// <summary>
    /// Wiki : Lead Core.txt — covering retiré de Calamity avant 2.0.1.005 : immunité à Irradiated. Lâché par les Irradiated
    /// Slime et Gamma Slime (infobox, WikiLoot). Rareté 5, vente 4 or 80 argent.
    /// </summary>
    public class LeadCore : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(gold: 4, silver: 80);
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual) => player.buffImmune[ModContent.BuffType<Irradiated>()] = true;
    }
}
