// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
using CalamityFandom.Content.Legacy.Projectiles.Rogue;
using CalamityMod;
using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.Weapons.Rogue
{
    /// <summary>
    /// Wiki : Phantom Lance.txt — javelots consommables qui ralentissent et s'effacent en semant des âmes à tête chercheuse
    /// (Phantom de Calamity), de moins en moins fortes (70 % des dégâts, jusqu'à 0 en 5 s). Attaque furtive : vitesse
    /// constante et âmes toujours à 70 %. Retiré de Calamity après 2.0.1.005. Valeurs du wiki : use 22, pile de 9999,
    /// rareté 8, vente 10 cuivre ; recette (100 par fabrication) et éléments par les données du wiki.
    /// </summary>
    public class PhantomLance : RogueWeapon
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 99;

        public override void SetDefaults()
        {
            Item.damage = 70;
            Item.knockBack = 5f;
            Item.width = 62;
            Item.height = 68;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.value = Item.sellPrice(copper: 10);
            Item.rare = ItemRarityID.Yellow;
            Item.useTime = Item.useAnimation = 22;
            Item.maxStack = 9999;
            Item.UseSound = SoundID.Item1;
            Item.consumable = true;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.autoReuse = true;
            Item.shootSpeed = 10f;
            Item.shoot = ModContent.ProjectileType<PhantomLanceProj>();
        }

        public override float StealthDamageMultiplier => 1.75f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                if (p.WithinBounds(Main.maxProjectiles))
                    Main.projectile[p].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }
    }
}
