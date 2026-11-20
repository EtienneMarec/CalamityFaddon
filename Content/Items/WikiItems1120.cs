using System.Linq;
using CalamityFandom.Common.Debug;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

// Lot AR (20/11) : objets simples du wiki dont la recette est faisable.

namespace CalamityFandom.Content.Items.Ammo
{
    /// <summary>
    /// Wiki : Klepticoin.txt — pièce qui vaut 100 Cosmilite Coins ; munition du Coin Gun (6000 dégâts de base).
    /// [technique] Valeur plafonnée au maximum du jeu (10 000 platine dépasse un entier).
    /// </summary>
    public class Klepticoin : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 100;

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 34;
            Item.maxStack = 999;
            Item.consumable = true;
            Item.damage = 6000;
            Item.DamageType = DamageClass.Ranged;
            Item.ammo = AmmoID.Coin;
            Item.shoot = ModContent.ProjectileType<Projectiles.Ranged.KlepticoinProj>();
            Item.rare = ItemRarityID.White;
            Item.value = int.MaxValue;
        }

        public override void PickAmmo(Item weapon, Player player, ref int type, ref float speed, ref StatModifier damage, ref float knockback)
            => type = ModContent.ProjectileType<Projectiles.Ranged.KlepticoinProj>();
    }
}

namespace CalamityFandom.Content.Items.Materials
{
    /// <summary>
    /// Wiki : Exo Avalon.txt — objet sans usage (« It has no use »), vendu {{value|2147|48|36|47}} = le maximum du jeu.
    /// [technique] Valeur d'achat au maximum (prix de vente : un cinquième).
    /// </summary>
    public class ExoAvalon : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 54;
            Item.maxStack = 1;
            Item.rare = ItemRarityID.Purple;
            Item.value = int.MaxValue;
        }
    }
}

namespace CalamityFandom.Content.Items.Tools
{
    /// <summary>
    /// Wiki : Dialup.txt — « The dialup is a hammer i think », sans chiffres. [choix] Marteau de fin de jeu : 250 % de
    /// puissance, 300 dégâts de mêlée, 8 images, recul 8 ; rareté et prix de Calamity Red.
    /// </summary>
    public class Dialup : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 300;
            Item.DamageType = DamageClass.Melee;
            Item.hammer = 250;
            Item.useTime = Item.useAnimation = 8;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 8f;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.UseSound = SoundID.Item1;
            Item.width = Item.height = 98;
            Item.scale = 0.6f;
            Item.rare = ModContent.RarityType<CalamityRed>();
            Item.value = CalamityMod.Items.CalamityGlobalItem.RarityCalamityRedBuyPrice;
        }
    }

    public class WikiItems1120Checks : ModSystem
    {
        public override void Load()
        {
            SelfTest.ExtraChecks.Add(t =>
            {
                foreach (int type in new[] { ModContent.ItemType<Ammo.Klepticoin>(), ModContent.ItemType<Materials.ExoAvalon>(), ModContent.ItemType<Dialup>() })
                    t.Expect(Main.recipe.Take(Recipe.numRecipes).Any(r => r.createItem.type == type && !r.Disabled), $"recette du wiki de {Lang.GetItemNameValue(type)}");
            });
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Ranged
{
    /// <summary>Klepticoin tirée par le Coin Gun : traverse 3 ennemis, lumière dorée.</summary>
    public class KlepticoinProj : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Ammo/Klepticoin";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 1;
            Projectile.scale = 0.6f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.rotation += 0.3f;
            Lighting.AddLight(Projectile.Center, 0.9f, 0.7f, 0.2f);
        }
    }
}
