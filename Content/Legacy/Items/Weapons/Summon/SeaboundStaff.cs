// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using CalamityMod;
using CalamityMod.Buffs.Summon;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.Weapons.Summon
{
    /// <summary>
    /// Wiki : Seabound Staff.txt — bâton d'invocation du Desert Scourge, retiré de Calamity (remplacé par le Brittle Star
    /// Staff, dont il réutilise le sbire et le buff). Valeurs du wiki : use 34 (35 dans le vieux code), vente 40 argent.
    /// Lâché par le Desert Scourge (butin du wiki), éléments Water/Smash par le système d'éléments.
    /// </summary>
    public class SeaboundStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.GamepadWholeScreenUseRange[Type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 11;
            Item.mana = 10;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = Item.useAnimation = 34;
            Item.scale = 0.75f;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = Item.sellPrice(silver: 40);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item44;
            Item.autoReuse = true;
            Item.buffType = ModContent.BuffType<BrittleStar>();
            Item.shoot = ModContent.ProjectileType<BrittleStarMinion>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Comme le Brittle Star Staff actuel : le sbire apparaît au curseur, numéroté pour se répartir
            int summonNumber = player.ownedProjectileCounts[type];
            var minion = Projectile.NewProjectileDirect(source, player.ClampedMouseWorld(), Vector2.Zero, type, damage, knockback, player.whoAmI, 0, 0, summonNumber);
            minion.originalDamage = Item.damage;
            player.AddBuff(Item.buffType, 2);
            return false;
        }
    }
}
