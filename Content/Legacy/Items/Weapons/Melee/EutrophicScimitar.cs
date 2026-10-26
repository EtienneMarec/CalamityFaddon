// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
using CalamityFandom.Content.Legacy.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Eutrophic Scimitar.txt — épée qui tire deux décharges électriques (70 % des dégâts) qui étourdissent
    /// (Eutrophication 0,5 s). Lâchée par le Blinded Angler (infobox, WikiLoot). Retirée de Calamity après 2.0.1.005.
    /// Valeurs du wiki : use 39, rareté 4, vente 2 or 40 argent ; éléments par les données du wiki.
    /// </summary>
    public class EutrophicScimitar : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 110;
            Item.DamageType = DamageClass.Melee;
            Item.width = 64;
            Item.height = 78;
            Item.useTime = Item.useAnimation = 39;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 2;
            Item.shoot = ModContent.ProjectileType<EutrophicScimitarProj>();
            Item.shootSpeed = 17;
            Item.value = Item.sellPrice(gold: 2, silver: 40);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.useTurn = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i <= 21; i++)
            {
                var dust = Dust.NewDustDirect(position - new Vector2(29), 58, 58, 226, 0f, 0f, 0, Color.White, 0.46f);
                dust.noGravity = true;
                dust.fadeIn = 0.95f;
            }
            for (int i = 0; i < 2; i++)
            {
                Vector2 spread = velocity + new Vector2(Main.rand.Next(-15, 16), Main.rand.Next(-15, 16)) * 0.05f;
                Projectile.NewProjectile(source, position, spread, type, (int)(damage * 0.7), knockback, player.whoAmI);
            }
            return false;
        }
    }
}
