// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.Weapons.Magic
{
    /// <summary>
    /// Wiki : Carnage Ray.txt — bâton qui tire un rayon de sang (BloodRay, encore dans Calamity) ; plus l'ennemi est loin
    /// sur le rayon, plus il prend de dégâts. Retiré de Calamity après 2.0.1.005. Valeurs du wiki : rareté 4, vente 2 or
    /// 40 argent ; recette (Blood Sample, matériau généré) et élément par les données du wiki.
    /// </summary>
    public class CarnageRay : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 24;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.width = 46;
            Item.height = 46;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.25f;
            Item.value = Item.sellPrice(gold: 2, silver: 40);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item72;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BloodRay>();
            Item.shootSpeed = 6f;
        }
    }
}
