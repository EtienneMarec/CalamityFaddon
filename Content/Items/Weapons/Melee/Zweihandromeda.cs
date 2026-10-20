using CalamityFandom.Content.Projectiles.Melee;
using CalamityMod;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Typeless;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Zweihandromeda.txt — épée true melee géante liée à Stratus.
    /// Coups alternés vers le bas puis vers le haut (« back and forth »), portés par le projectile tenu
    /// <see cref="ZweihandromedaSwing"/>. Invoque des étoiles depuis le ciel quand elle touche et inflige
    /// Moon Burnt (= Nightwither) 3 s.
    /// </summary>
    public class Zweihandromeda : ModItem
    {
        public const int StarsPerHit = 2;

        // Sens du prochain coup : 1 = vers le bas, -1 = vers le haut
        private float nextSwingSide = 1f;

        public override void SetDefaults()
        {
            Item.width = 232;
            Item.height = 230;
            Item.damage = 400;
            Item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Item.knockBack = 1f;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = Item.useTime = 19;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<ZweihandromedaSwing>();
            Item.shootSpeed = 1f;
            Item.UseSound = SoundID.Item1;
            Item.value = Item.sellPrice(gold: 26);
            Item.rare = ModContent.RarityType<PureGreen>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, player.MountedCenter, velocity, type, damage, knockback, player.whoAmI, nextSwingSide);
            nextSwingSide = -nextSwingSide;
            return false;
        }

        public override void AddRecipes()
        {
            // TODO : Night's Gaze (javelot retiré de Calamity, présent sur le wiki) quand il sera recréé.
            CreateRecipe()
                .AddIngredient<StratusSphere>()
                .AddIngredient<GalileoGladius>()
                .AddIngredient<CrescentMoon>()
                .AddIngredient<Lumenyl>(25)
                .AddIngredient<DepthCells>(37)
                .AddIngredient<AscendantSpiritEssence>(4)
                .AddIngredient<AuricBar>(8)
                .AddTile(ModContent.TileType<CalamityMod.Tiles.Furniture.CraftingStations.CosmicAnvil>())
                .Register();
        }
    }
}
