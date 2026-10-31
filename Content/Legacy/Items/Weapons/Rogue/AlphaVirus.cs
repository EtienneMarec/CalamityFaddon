// Adapté de Calamity Mod (version mensuelle de 2022, legacy_extract.py), © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using CalamityFandom.Content.Legacy.Projectiles.Rogue;
using CalamityMod;
using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.Weapons.Rogue
{
    /// <summary>
    /// Wiki : Alpha Virus.txt — cellule de peste géante à aura mortelle (dégâts à travers les blocs), qui se divise en 6
    /// plague seekers ; frappe furtive : la cellule ralentit et accumule des seekers autour d'elle. Valeurs du wiki
    /// (242 dégâts, use 30, rareté 11, vente 24 or) ; recette du wiki (Epidemic Shredder, 5 Luminite Bar, 25 Plague Cell
    /// Canister) par WikiRecipes : le Bloodstone Core n'était que dans la recette du vieux code.
    /// </summary>
    public class AlphaVirus : RogueWeapon
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = Item.height = 44;
            Item.damage = 242;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 1f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = Item.sellPrice(gold: 24);
            Item.shoot = ModContent.ProjectileType<AlphaVirusProjectile>();
            Item.shootSpeed = 4f;
            Item.rare = ItemRarityID.Purple;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!player.Calamity().StealthStrikeAvailable())
                return true;
            int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, Item.shootSpeed);
            if (p.WithinBounds(Main.maxProjectiles))
                Main.projectile[p].Calamity().stealthStrike = true;
            return false;
        }
    }
}
