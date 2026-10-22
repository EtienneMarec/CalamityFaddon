// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
using CalamityFandom.Content.Legacy.Projectiles.Typeless;
using CalamityMod;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.Accessories
{
    /// <summary>
    /// Wiki : Gehenna.txt — covering de la Brimstone Elemental, retiré de Calamity : toutes les 10 secondes, deux boules de
    /// feu de soufre tombent du ciel vers le joueur et laissent une explosion persistante. Valeurs du wiki : Brimstone
    /// Flames 10 s (4 s dans le vieux code), rareté 5, vente 4 or 80 argent. Dégâts 54, mis à l'échelle de la meilleure
    /// classe du joueur (vieux code). Le compte à rebours passe dans un ModPlayer (il était sur l'objet).
    /// </summary>
    public class Gehenna : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.value = Item.sellPrice(gold: 4, silver: 80);
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual) => player.GetModPlayer<GehennaPlayer>().Equipped = Item;
    }

    public class GehennaPlayer : ModPlayer
    {
        private const int Cooldown = 600;
        private const int Fireballs = 2;
        private const float Spread = 120f;

        public Item Equipped;
        private int countdown = Cooldown;

        public override void ResetEffects() => Equipped = null;

        public override void PostUpdateEquips()
        {
            if (Equipped == null || --countdown > 0)
                return;
            countdown = Cooldown;
            if (Player.whoAmI != Main.myPlayer)
                return;
            Vector2 baseSpawn = Player.Center + new Vector2(Main.rand.Next(1000) - 500, -1000);
            Vector2 baseVelocity = Vector2.Normalize(Player.Center - baseSpawn) * 25f;
            int damage = (int)Player.GetBestClassDamage().ApplyTo(54);
            for (int i = 0; i < Fireballs; i++)
            {
                Vector2 spawn = baseSpawn + new Vector2(i * 30 - Fireballs * 15, 0f);
                Vector2 velocity = baseVelocity.RotatedBy(MathHelper.ToRadians(-Spread / 2 + Spread * i / Fireballs));
                velocity.X += 3 * Main.rand.NextFloat() - 1.5f;
                Projectile.NewProjectile(Player.GetSource_Accessory(Equipped), spawn, velocity, ModContent.ProjectileType<GehennaFireball>(), damage, 5f, Player.whoAmI);
            }
        }
    }
}
