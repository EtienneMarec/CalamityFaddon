// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
using CalamityFandom.Content.Legacy.Buffs.Summon;
using CalamityFandom.Content.Legacy.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.Weapons.Summon
{
    /// <summary>
    /// Bâtons de The Slime God retirés de Calamity (Crimslime Staff, Corroslime Staff) : un petit slime sbire au curseur.
    /// Les deux ne diffèrent que par leur slime. Valeurs du wiki : use 29 (30 dans le vieux code), vente 2 or 40 argent,
    /// rareté 4. Lâchés par The Slime God (butin du wiki), éléments Unholy/Poison/Smash par le système d'éléments.
    /// </summary>
    public abstract class SlimeStaff<TMinion, TBuff> : ModItem where TMinion : ModProjectile where TBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.GamepadWholeScreenUseRange[Type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 24;
            Item.mana = 10;
            Item.width = 52;
            Item.height = 52;
            Item.useTime = Item.useAnimation = 29;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = Item.sellPrice(gold: 2, silver: 40);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item44;
            Item.autoReuse = true;
            Item.buffType = ModContent.BuffType<TBuff>();
            Item.shoot = ModContent.ProjectileType<TMinion>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            var minion = Projectile.NewProjectileDirect(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI);
            minion.originalDamage = Item.damage;
            return false;
        }
    }

    /// <summary>Wiki : Crimslime Staff.txt</summary>
    public class CrimslimeStaff : SlimeStaff<CrimslimeMinion, Crimslime>
    {
    }

    /// <summary>Wiki : Corroslime Staff.txt</summary>
    public class CorroslimeStaff : SlimeStaff<CorroslimeMinion, Corroslime>
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.height = 54;
        }
    }
}
