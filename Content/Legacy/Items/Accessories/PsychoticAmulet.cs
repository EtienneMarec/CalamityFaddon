// Adapté de Calamity Mod (retiré le 11/06/2022, commit 7124134610), © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using CalamityMod;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.Accessories
{
    /// <summary>
    /// Wiki : Psychotic Amulet.txt — covering retiré de Calamity avant 2.0.1.005 : +5 % de dégâts et de critique spy et
    /// ranged ; immobile, le joueur devient invisible comme avec la Shroomite armor (furtivité de vanilla, qui donne la
    /// partie ranged : +60 % de dégâts, +10 % de critique, +50 % de recul, -750 d'aggro) et la partie spy s'y ajoute
    /// (+20 % de dégâts, +10 % de critique, +50 % de recul : les totaux de la page). Rareté 5, vente 4 or 80 argent.
    /// Sert aux casques Auric Tesla ; lâché par la Possessed Armor (infobox, WikiLoot).
    /// </summary>
    public class PsychoticAmulet : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.value = Item.sellPrice(gold: 4, silver: 80);
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var rogue = ModContent.GetInstance<RogueDamageClass>();
            player.GetDamage(rogue) += 0.05f;
            player.GetCritChance(rogue) += 5;
            player.GetDamage(DamageClass.Ranged) += 0.05f;
            player.GetCritChance(DamageClass.Ranged) += 5;

            // Furtivité de vanilla : ne se cumule pas avec la Shroomite armor (même drapeau)
            player.shroomiteStealth = true;
            float hidden = 1f - player.stealth;
            player.GetDamage(rogue) += hidden * 0.2f;
            player.GetCritChance(rogue) += (int)(hidden * 10f);
            player.GetKnockback(rogue) += hidden * 0.5f;
        }
    }
}
