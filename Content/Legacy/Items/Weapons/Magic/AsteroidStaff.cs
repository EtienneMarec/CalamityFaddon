// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
using System;
using CalamityFandom.Content.Legacy.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.Weapons.Magic
{
    /// <summary>
    /// Wiki : Asteroid Staff.txt — amélioration du Meteor Staff : fait pleuvoir 3 astéroïdes vers le curseur, qui explosent
    /// au contact. Retiré de Calamity après 2.0.1.005. Valeurs du wiki : rareté 10, vente 20 or ; recette et éléments par
    /// les données du wiki.
    /// </summary>
    public class AsteroidStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 145;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 18;
            Item.width = 50;
            Item.height = 50;
            Item.useTime = Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6.75f;
            Item.UseSound = SoundID.Item88;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Asteroid>();
            Item.shootSpeed = 20f;
            Item.value = Item.sellPrice(gold: 20);
            Item.rare = ItemRarityID.Red;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 cursor = Main.MouseWorld;
            for (int i = 0; i < 3; i++)
            {
                Vector2 origin = new Vector2((player.Center.X + Main.rand.Next(201) * -player.direction + cursor.X - player.position.X + player.Center.X) / 2f
                    + Main.rand.Next(-200, 201), player.MountedCenter.Y - 600f - 100f * i);
                Vector2 aim = cursor - origin + new Vector2(Main.rand.Next(-40, 41) * 0.03f, 0f);
                aim.Y = Math.Max(Math.Abs(aim.Y), 20f);
                aim = Vector2.Normalize(aim) * Item.shootSpeed + new Vector2(0f, Main.rand.Next(-40, 41) * 0.02f);
                Projectile.NewProjectile(source, origin, aim * 0.75f, type, damage, knockback, player.whoAmI, 0f, 0.5f + Main.rand.NextFloat() * 0.3f);
            }
            return false;
        }
    }
}
