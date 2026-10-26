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
    /// Wiki : Sandslasher.txt — grand shuriken de sable sans gravité qui accélère à l'horizontale et frappe de plus en plus
    /// fort ; attaque furtive : sème des nuages de sable (DuststormCloud de Calamity, 40 % des dégâts). Retiré de Calamity
    /// après 2.0.1.005. Valeurs du wiki : use 19, rareté 7, vente 12 or ; recette et éléments par les données du wiki.
    /// </summary>
    public class Sandslasher : RogueWeapon
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.damage = 115;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useTime = Item.useAnimation = 19;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = Item.sellPrice(gold: 12);
            Item.rare = ItemRarityID.Lime;
            Item.shoot = ModContent.ProjectileType<SandslasherProj>();
            Item.shootSpeed = 7f;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
        }

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
