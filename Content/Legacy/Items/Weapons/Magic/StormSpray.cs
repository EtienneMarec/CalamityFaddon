// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using CalamityFandom.Content.Legacy.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.Weapons.Magic
{
    /// <summary>
    /// Wiki : Storm Spray.txt — bâton du Desert Scourge (retiré de Calamity) : un jet d'eau qui laisse goutter des
    /// traînées d'eau. Valeurs de l'infobox (identiques au vieux code, vente 40 argent). Lâché par le Desert Scourge
    /// (butin du wiki), élément Water par le système d'éléments.
    /// </summary>
    public class StormSpray : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 14;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 8;
            Item.width = 42;
            Item.height = 42;
            Item.useTime = Item.useAnimation = 33;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = Item.sellPrice(silver: 40);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item13;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<WaterStream>();
            Item.shootSpeed = 10f;
        }
    }
}
