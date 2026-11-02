using CalamityMod;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Items.Accessories
{
    /// <summary>
    /// Wiki : Ferro Claws.txt — +17 % de dégâts de mêlée, -10 % de vitesse de mêlée, attaques automatiques ; chaque coup
    /// d'arme de vrai corps-à-corps lance 2 éclats de ferrofluide à tête chercheuse quand un ennemi est proche (30 blocs de
    /// portée, 25 % des dégâts du coup ; rien pour les fouets). Obtenue en jetant un Mechanical Glove dans le Shimmer après
    /// le Moon Lord (avant : le Mechanical Glove se défait comme d'habitude).
    /// [choix] « Ennemi proche » : à moins de 30 blocs ; « coup » : le début de chaque coup d'une arme de mêlée qui frappe
    /// elle-même (pas les armes qui lancent un projectile à la place).
    /// </summary>
    public class FerroClaws : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = Item.height = 36;
            Item.accessory = true;
            Item.value = Item.sellPrice(gold: 22, silver: 50);
            Item.rare = ItemRarityID.Red;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Melee) += 0.17f;
            player.GetAttackSpeed(DamageClass.Melee) -= 0.10f;
            player.autoReuseGlove = true;
            player.GetModPlayer<FerroClawsPlayer>().Active = true;
        }
    }

    /// <summary>
    /// Page Shimmer : Mechanical Glove → Ferro Claws « After defeating Moon Lord » (la table de la page n'est pas lue par
    /// gen_recipes.py) ; avant, le gant se défait comme dans le jeu de base.
    /// </summary>
    public class FerroClawsShimmer : ModSystem
    {
        public override void PostUpdateEverything() =>
            ItemID.Sets.ShimmerTransformToItem[ItemID.MechanicalGlove] = NPC.downedMoonlord ? ModContent.ItemType<FerroClaws>() : -1;
    }

    public class FerroClawsPlayer : ModPlayer
    {
        public const float Range = 30 * 16f;
        public bool Active;

        public override void ResetEffects() => Active = false;

        public override void PostItemCheck()
        {
            Item item = Player.HeldItem;
            if (!Active || Player.whoAmI != Main.myPlayer || !Player.ItemAnimationJustStarted || item.damage <= 0 || item.noMelee
                || !item.CountsAsClass(DamageClass.Melee) || ProjectileID.Sets.IsAWhip[item.shoot])
                return;
            NPC target = null;
            foreach (var n in Main.ActiveNPCs)
                if (n.CanBeChasedBy() && n.WithinRange(Player.Center, Range) && (target == null || n.DistanceSQ(Player.Center) < target.DistanceSQ(Player.Center)))
                    target = n;
            if (target == null)
                return;
            int damage = (int)(Player.GetWeaponDamage(item) * 0.25f);
            Vector2 toward = (target.Center - Player.Center).SafeNormalize(Vector2.UnitX * Player.direction);
            for (int i = 0; i < 2; i++)
                Projectile.NewProjectile(Player.GetSource_Accessory(Player.armor[0]), Player.Center, toward.RotatedBy(i == 0 ? -0.4f : 0.4f) * 12f,
                    ModContent.ProjectileType<FerroShrapnel>(), damage, 2f, Player.whoAmI);
        }
    }

    /// <summary>Éclat de ferrofluide des Ferro Claws : cherche un ennemi, disparaît après 30 blocs.</summary>
    public class FerroShrapnel : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.timeLeft = (int)(FerroClawsPlayer.Range / 12f);
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            CalamityUtils.HomeInOnNPC(Projectile, false, FerroClawsPlayer.Range, 12f, 14f);
            for (int i = 0; i < 2; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center - Projectile.velocity * i * 0.5f, DustID.Smoke, Vector2.Zero, 60, new Color(40, 40, 50), 1.1f);
                dust.noGravity = true;
            }
        }
    }
}
