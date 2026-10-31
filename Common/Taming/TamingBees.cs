using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityFandom.Common.Taming
{
    /// <summary>
    /// Queen Bee : « Bring 100 bees into a hive biome. Upon doing so, Queen Bee will spawn, and will instantly be tamed ».
    /// [validé 31/10] Comptent les petites abeilles ennemies et celles du joueur (Bee Gun, Beenade, Bee's Knees, Hive
    /// Pack...). [choix] 100 abeilles en même temps à moins de 100 blocs d'un joueur qui est dans une ruche (ZoneHive).
    /// </summary>
    public static class TamingBees
    {
        public const int Goal = 100;
        public const string Key = "Queen Bee";
        private const float Range = 1600f;

        public static bool IsBee(Projectile p) => p.type is ProjectileID.Bee or ProjectileID.GiantBee;

        public static bool InHive(Vector2 position)
        {
            Point tile = position.ToTileCoordinates();
            return WorldGen.InWorld(tile.X, tile.Y) && Main.tile[tile.X, tile.Y].WallType is WallID.HiveUnsafe or WallID.Hive;
        }

        /// <summary>Abeilles autour d'un joueur dans une ruche (0 s'il n'y est pas).</summary>
        public static int CountAround(Player player)
        {
            if (!player.active || player.dead || !player.ZoneHive)
                return 0;
            int count = 0;
            foreach (var n in Main.ActiveNPCs)
                if (n.type is NPCID.Bee or NPCID.BeeSmall && n.WithinRange(player.Center, Range))
                    count++;
            foreach (var p in Main.ActiveProjectiles)
                if (IsBee(p) && p.WithinRange(player.Center, Range))
                    count++;
            return count;
        }
    }

    /// <summary>
    /// [validé 31/10] Dans une ruche (mur de ruche derrière l'abeille, ou propriétaire dans le biome), les abeilles du
    /// joueur rebondissent sur les blocs sans perdre de pénétration (au lieu de mourir au 3e rebond). [choix] Elles y
    /// vivent aussi 15 s au lieu de 2,5 s, pour qu'on puisse en réunir 100.
    /// </summary>
    public class HiveBeeProjectile : GlobalProjectile
    {
        private const int HiveLifetime = 3600;

        public override bool InstancePerEntity => true;
        private bool extended;

        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => TamingBees.IsBee(entity);

        private static bool InHive(Projectile projectile) =>
            TamingBees.InHive(projectile.Center) || Main.player[projectile.owner].ZoneHive;

        public override void AI(Projectile projectile)
        {
            if (!extended && InHive(projectile))
            {
                extended = true;
                projectile.timeLeft = System.Math.Max(projectile.timeLeft, HiveLifetime);
            }
        }

        public override bool OnTileCollide(Projectile projectile, Vector2 oldVelocity)
        {
            if (!InHive(projectile))
                return true;
            if (projectile.velocity.X != oldVelocity.X)
                projectile.velocity.X = -oldVelocity.X;
            if (projectile.velocity.Y != oldVelocity.Y)
                projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }
    }

    /// <summary>Affiche le compte d'abeilles au-dessus du joueur dans une ruche (à partir de 10, chaque seconde).</summary>
    public class TamingBeesPlayer : ModPlayer
    {
        public override void PostUpdate()
        {
            if (Player.whoAmI != Main.myPlayer || Main.GameUpdateCount % 60 != 0 || TamingSystem.Untamable.Contains(TamingBees.Key))
                return;
            int count = TamingBees.CountAround(Player);
            if (count >= 10)
                CombatText.NewText(Player.Hitbox, Color.Gold, Language.GetTextValue("Mods.CalamityFandom.Taming.Bees", count, TamingBees.Goal));
        }
    }
}
