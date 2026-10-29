// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.Weapons.Ranged
{
    /// <summary>
    /// Wiki : Storm Dragoon.txt — mitrailleuse retirée de Calamity : rafale de balles dispersées, 90 % de chance de ne
    /// pas consommer de munition. Valeurs de l'infobox (identiques au vieux code ; rareté 12, vente 24 or).
    /// </summary>
    public class StormDragoon : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 62;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 74;
            Item.height = 34;
            Item.useTime = 2;
            Item.reuseDelay = 10;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.25f;
            Item.value = Item.sellPrice(gold: 24);
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.UseSound = SoundID.Item31;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 18f;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-25, 0);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 spread = velocity + new Vector2(Main.rand.Next(-40, 41), Main.rand.Next(-40, 41)) * 0.05f;
            Projectile.NewProjectile(source, position, spread, type, damage, knockback, player.whoAmI);
            return false;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.Next(100) >= 90;
    }
}
