using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Items.Weapons.Ranged
{
    /// <summary>
    /// Wiki : Reemergence.txt — arbalète à répétition inventée par le wiki, lâchée par le Plaguebringer Goliath (WikiLoot) :
    /// les flèches en bois deviennent des flèches de peste qui éclatent en abeilles (PlagueArrow de Calamity, celle de la
    /// Malevolence : explosion de peste + abeilles). Valeurs du wiki : 48 dégâts, recul 3, vélocité 18, use 12, rareté 8,
    /// vente 16 or.
    /// </summary>
    public class Reemergence : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 76;
            Item.height = 34;
            Item.damage = 48;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.value = Item.sellPrice(gold: 16);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 18f;
            Item.useAmmo = AmmoID.Arrow;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (type == ProjectileID.WoodenArrowFriendly)
                type = ModContent.ProjectileType<PlagueArrow>();
        }
    }
}
