// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.Weapons.Ranged
{
    /// <summary>
    /// Wiki : Astral Bow.txt — arc qui tire trois flèches à la suite (5 ticks chacune, rafale de 15). Classe AstralRepeater
    /// comme dans Calamity (ancien nom « Astral Repeater », Trivia). Retiré après 2.0.1.005. Valeurs du wiki : 99 dégâts,
    /// critique 29 % (4 + 25), rareté 9, vente 19 or. [technique] Les tirs à bout portant de Calamity n'existent plus.
    /// </summary>
    public class AstralRepeater : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 99;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 38;
            Item.height = 78;
            Item.useTime = 5;
            Item.reuseDelay = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.5f;
            Item.value = Item.sellPrice(gold: 19);
            Item.rare = ItemRarityID.Cyan;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 16f;
            Item.useAmmo = AmmoID.Arrow;
        }

        // Critique élevé : ajouté ici plutôt que dans SetDefaults (comme Calamity)
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 25;
    }
}
