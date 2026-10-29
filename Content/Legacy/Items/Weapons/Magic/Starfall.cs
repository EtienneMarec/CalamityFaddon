// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.Weapons.Magic
{
    /// <summary>
    /// Wiki : Starfall.txt — tome retiré de Calamity : 5 étoiles astrales tombent sur le curseur (projectile encore dans
    /// Calamity). Valeurs du wiki : 50 dégâts (57 dans le vieux code), critique 29 % (4 + 25), vente 19 or, rareté 9.
    /// </summary>
    public class Starfall : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 50;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 15;
            Item.rare = ItemRarityID.Cyan;
            Item.width = 38;
            Item.height = 40;
            Item.useTime = Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.25f;
            Item.value = Item.sellPrice(gold: 19);
            Item.UseSound = SoundID.Item105;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<AstralStarMagic>();
            Item.shootSpeed = 12f;
        }

        // Un critique élevé dans SetDefaults pose problème à Terraria (vieux code) : ajouté ici
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 25;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Les étoiles partent du curseur (légèrement décalé) et tombent en éventail
            Vector2 origin = Main.MouseWorld + new Vector2(Main.rand.Next(-1, 2), Main.rand.Next(-1, 2)) * 0.5f;
            for (int i = 0; i < 5; i++)
                Projectile.NewProjectile(source, origin, new Vector2(2f + Main.rand.Next(-8, 5), 15f + Main.rand.Next(1, 6)), type, damage, knockback, player.whoAmI);
            return false;
        }
    }
}
