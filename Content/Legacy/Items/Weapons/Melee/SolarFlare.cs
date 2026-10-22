// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
using CalamityFandom.Content.Legacy.Projectiles.Melee;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Solar Flare.txt — yoyo de Providence, retiré de Calamity : explosions sacrées (75 % des dégâts, Holy Flames
    /// 4 s) à chaque coup. Valeurs de l'infobox (identiques au vieux code ; vente 24 or, rareté 12). Lâché par Providence
    /// (butin du wiki), éléments Holy/Fire par le système d'éléments.
    /// </summary>
    public class SolarFlare : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.Yoyo[Type] = true;
            ItemID.Sets.GamepadExtraRange[Type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 38;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.damage = 71;
            Item.knockBack = 7.5f;
            Item.useTime = Item.useAnimation = 20;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item1;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<SolarFlareYoyo>();
            Item.shootSpeed = 16f;
            Item.value = Item.sellPrice(gold: 24);
            Item.rare = ModContent.RarityType<Turquoise>();
        }
    }
}
