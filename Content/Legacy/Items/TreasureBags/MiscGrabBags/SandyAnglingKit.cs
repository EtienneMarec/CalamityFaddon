// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using Terraria.ID;

namespace CalamityFandom.Content.Legacy.Items.TreasureBags.MiscGrabBags
{
    /// <summary>
    /// Wiki : Sandy Angling Kit.txt — lâché par le Desert Scourge (butin du wiki, WikiLoot). Chances identiques au vieux
    /// code et à la page ; pile de 9999 et sans valeur, comme l'infobox.
    /// </summary>
    public class SandyAnglingKit : AnglingKit
    {
        protected override int Rarity => ItemRarityID.Blue;

        protected override Row[] Table => new Row[]
        {
            new(Gear, 15, 12),
            new(Tools, 10, 9),
            new(Clothes, 5, 4),
            new(Potions, 5, 4, 2, 3),
            new(new int[] { ItemID.GoldenBugNet }, 20, 18, Condition: new SkeletronDefeated()), // « If Skeletron has been defeated »
        };
    }
}
