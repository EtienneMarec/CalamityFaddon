// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
using CalamityFandom.Content.Legacy.Projectiles.Rogue;
using CalamityMod;
using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.Weapons.Rogue
{
    /// <summary>
    /// Wiki : Shock Grenade.txt — grenade de spy retirée de Calamity : explose en éclairs (Electrified 2 s) et en onde de
    /// choc (Electrified 3 s) ; attaque furtive : aura de Tesla (projectile encore dans Calamity) et éclairs à tête
    /// chercheuse. Valeurs du wiki : use 17 (18 dans le vieux code), pile 9999, rareté 8, vente 20 argent. Recette du
    /// wiki (WikiRecipes : 2 Grenade, 1 Bomb, 5 Stratus Brick → 100), éléments Magic/Electricity par le système d'éléments.
    /// </summary>
    public class ShockGrenade : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 30;
            Item.damage = 90;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.consumable = true;
            Item.useAnimation = Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 1f;
            Item.autoReuse = true;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(silver: 20);
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = ModContent.ProjectileType<ShockGrenadeProjectile>();
            Item.shootSpeed = 12.5f;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!player.Calamity().StealthStrikeAvailable())
                return true;
            var grenade = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            grenade.Calamity().stealthStrike = true;
            return false;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>(Texture + "Glow").Value);
        }
    }
}
