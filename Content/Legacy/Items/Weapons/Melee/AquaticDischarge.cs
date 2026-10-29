// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using CalamityFandom.Content.Legacy.Projectiles.Melee;
using CalamityMod;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Aquatic Discharge.txt — épée courte du Desert Scourge, retirée de Calamity. Valeurs du wiki : use 12 (18 dans
    /// le vieux code), vente 40 argent. Comportement de la page : étincelles électriques quand un ennemi meurt (le vieux
    /// code en lançait à chaque coup). Shortflurry (Electrified 5 s) : Common/Shortstrike, données de la page Shortstrike.
    /// Lâchée par le Desert Scourge (butin du wiki), éléments Water/Stab par le système d'éléments.
    /// </summary>
    public class AquaticDischarge : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Item.useTurn = false;
            Item.useAnimation = Item.useTime = 12;
            Item.width = 32;
            Item.height = 32;
            Item.damage = 23;
            Item.knockBack = 5.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<AquaticDischargeProj>();
            Item.shootSpeed = 2f;
            Item.value = Item.sellPrice(silver: 40);
            Item.rare = ItemRarityID.Green;
        }
    }
}
