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
    /// Wiki : Diseased Pike.txt — lance de la Plaguebringer Goliath, retirée de Calamity : 4 Plague Seekers (75 % des
    /// dégâts) à chaque coup, Plague 5 s. Valeurs de l'infobox (identiques au vieux code ; vente 16 or, rareté 8).
    /// Lâchée par la Plaguebringer Goliath (butin du wiki), éléments Poison/Stab/Machine par le système d'éléments.
    /// </summary>
    public class DiseasedPike : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.Spears[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 58;
            Item.damage = 65;
            Item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Item.noMelee = true;
            Item.useTurn = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 8.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = Item.sellPrice(gold: 16);
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = ModContent.ProjectileType<DiseasedPikeSpear>();
            Item.shootSpeed = 10f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;
    }
}
