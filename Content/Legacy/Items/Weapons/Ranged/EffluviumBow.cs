// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
using CalamityMod;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.Weapons.Ranged
{
    /// <summary>
    /// Wiki : Effluvium Bow.txt — arc qui tire deux flèches ; les flèches en bois deviennent des Mist Arrow (projectile
    /// encore dans Calamity). Retiré de Calamity après 2.0.1.005. Valeurs du wiki : rareté 5, vente 7 or 20 argent.
    /// Recette et éléments (Cold/Water) par les données du wiki.
    /// </summary>
    public class EffluviumBow : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 56;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 26;
            Item.height = 70;
            Item.useTime = Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4f;
            Item.value = Item.sellPrice(gold: 7, silver: 20);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<MistArrow>();
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.Arrow;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 2; i++)
            {
                Vector2 spread = velocity + new Vector2(Main.rand.Next(-30, 31), Main.rand.Next(-30, 31)) * 0.05f;
                if (CalamityUtils.CheckWoodenAmmo(type, player))
                    Projectile.NewProjectile(source, position, spread, ModContent.ProjectileType<MistArrow>(), damage, knockback, player.whoAmI);
                else
                    Projectile.NewProjectileDirect(source, position, spread, type, damage, knockback, player.whoAmI).noDropItem = true;
            }
            return false;
        }
    }
}
