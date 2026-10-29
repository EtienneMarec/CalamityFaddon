// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using Terraria.ID;

namespace CalamityFandom.Content.Legacy.Items.TreasureBags.MiscGrabBags
{
    /// <summary>
    /// Wiki : Bleached Angling Kit.txt — lâché par l'Aquatic Scourge (butin du wiki, WikiLoot). Chances de la page
    /// ({{dv|normal|expert}}), Angler Tackle Bag en plus du Sandy Angling Kit ; pile 9999, sans valeur, rareté 5.
    /// </summary>
    public class BleachedAnglingKit : AnglingKit
    {
        protected override int Rarity => ItemRarityID.Pink;

        protected override Row[] Table => new Row[]
        {
            new(Gear, 12, 10),
            new(Tools, 9, 8),
            new(new int[] { ItemID.AnglerTackleBag }, 18, 15),
            new(Clothes, 4, 2),
            new(Potions, 4, 2, 2, 3),
            new(new int[] { ItemID.GoldenBugNet }, 15, 12),
        };
    }
}
