// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
using CalamityFandom.Content.Legacy.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.Weapons.Magic
{
    /// <summary>
    /// Wiki : Lazhar.txt — pistolet magique, amélioration du Heat Ray : rayons rapides qui rebondissent sur les blocs et
    /// explosent sur les ennemis (SolarBeam2). Retiré de Calamity après 2.0.1.005. Valeurs du wiki : rareté 10, vente
    /// 20 or ; recette et éléments par les données du wiki.
    /// </summary>
    public class Lazhar : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 100;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 4;
            Item.width = 42;
            Item.height = 20;
            Item.useTime = Item.useAnimation = 7;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.value = Item.sellPrice(gold: 20);
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item12;
            Item.autoReuse = true;
            Item.shootSpeed = 15f;
            Item.shoot = ModContent.ProjectileType<SolarBeam2>();
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
            => velocity += new Vector2(Main.rand.Next(-15, 16), Main.rand.Next(-15, 16)) * 0.05f;
    }
}
