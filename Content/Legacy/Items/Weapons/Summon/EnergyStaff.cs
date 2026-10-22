// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
using CalamityFandom.Content.Legacy.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.Weapons.Summon
{
    /// <summary>
    /// Wiki : Energy Staff.txt — bâton de tourelle (lâché par l'Impious Immolator), remplacé dans Calamity par la
    /// Sanctified Spark (2023). Sa tourelle immobile de 2.0.1.005 est portée (ProfanedEnergyTurret) : celle de la Sanctified
    /// Spark suit le joueur. Valeurs de l'infobox (identiques au vieux code ; rareté 11, vente 22 or).
    /// </summary>
    public class EnergyStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 128;
            Item.DamageType = DamageClass.Summon;
            Item.sentry = true;
            Item.mana = 10;
            Item.width = 66;
            Item.height = 68;
            Item.useTime = Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.value = Item.sellPrice(gold: 22);
            Item.rare = ItemRarityID.Purple;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ProfanedEnergyTurret>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var turret = Projectile.NewProjectileDirect(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI, 16f);
            turret.originalDamage = Item.damage;
            player.UpdateMaxTurrets();
            return false;
        }
    }
}
