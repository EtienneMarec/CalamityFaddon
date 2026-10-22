// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Soul Edge.txt — épée retirée de Calamity : tire deux âmes fantomatiques (petites, moyennes ou grandes,
    /// projectiles encore dans Calamity), Crush Depth 5 s au contact. Valeurs du wiki : use 22 (23 dans le vieux code),
    /// rareté 13, vente 26 or.
    /// </summary>
    public class SoulEdge : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 88;
            Item.height = 88;
            Item.damage = 174;
            Item.DamageType = DamageClass.Melee;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = Item.useAnimation = 22;
            Item.useTurn = true;
            Item.knockBack = 5.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<GhastlySoulLarge>();
            Item.shootSpeed = 12f;
            Item.value = Item.sellPrice(gold: 26);
            Item.rare = ModContent.RarityType<PureGreen>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Le vieux code supposait les trois âmes consécutives (type + 0..2) : on les nomme
            int[] souls = { ModContent.ProjectileType<GhastlySoulLarge>(), ModContent.ProjectileType<GhastlySoulMedium>(), ModContent.ProjectileType<GhastlySoulSmall>() };
            for (int i = 0; i < 2; i++)
            {
                Vector2 spread = velocity + new Vector2(Main.rand.Next(-40, 41), Main.rand.Next(-40, 41)) * 0.05f;
                Projectile.NewProjectile(source, position, spread, Main.rand.Next(souls), damage, knockback, player.whoAmI, 0f, Main.rand.NextFloat() + 0.5f);
            }
            return false;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<CrushDepth>(), 300);

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo) => target.AddBuff(ModContent.BuffType<CrushDepth>(), 300);
    }
}
