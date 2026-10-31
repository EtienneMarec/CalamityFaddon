using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityFandom.Common.Taming
{
    /// <summary>
    /// Queen Bee : « Bring 100 bees into a hive biome. Upon doing so, Queen Bee will spawn, and will instantly be tamed ».
    /// [validé 31/10] Comptent les petites abeilles ennemies et celles du joueur (Bee Gun, Beenade, Bee's Knees, Hive
    /// Pack...). [choix] 100 abeilles en même temps à moins de 100 blocs d'un joueur qui est dans une ruche (ZoneHive).
    /// [validé 31/10] Calamity++ (Balance/taming.json) : 40 abeilles, et seulement les petites abeilles libérées par les blocs
    /// de ruche cassés et les ruches suspendues qui tombent (ni celles du joueur, ni celles de la Queen Bee) ; les outils
    /// (pioches, haches, marteaux, foreuses) ne les blessent pas, sinon on les tue en creusant.
    /// </summary>
    public static class TamingBees
    {
        /// <summary>Abeilles à réunir : 100 d'après la page ; Calamity++ : 40.</summary>
        public static int Goal = 100;
        /// <summary>Calamity++ : seulement les abeilles des ruches (blocs et ruches suspendues).</summary>
        public static bool HiveOnly;
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
                if (n.type is NPCID.Bee or NPCID.BeeSmall && n.WithinRange(player.Center, Range) && (!HiveOnly || n.GetGlobalNPC<HiveBee>().FromHive))
                    count++;
            if (!HiveOnly)
                foreach (var p in Main.ActiveProjectiles)
                    if (IsBee(p) && p.WithinRange(player.Center, Range))
                        count++;
            return count;
        }
    }

    /// <summary>
    /// Petite abeille ennemie sortie d'une ruche : d'un bloc de ruche cassé (le jeu en libère 1 ou 2 une fois sur deux, quand
    /// le bloc ne se change pas en miel) ou d'une ruche suspendue qui tombe (2 à 5). Les outils ne la blessent pas.
    /// </summary>
    public class HiveBee : GlobalNPC
    {
        public bool FromHive;

        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type is NPCID.Bee or NPCID.BeeSmall;

        public override void OnSpawn(NPC npc, IEntitySource source) =>
            FromHive = source is EntitySource_TileBreak || source is EntitySource_Parent { Entity: Projectile { type: ProjectileID.BeeHive } };

        public override void SendExtraAI(NPC npc, Terraria.ModLoader.IO.BitWriter bitWriter, System.IO.BinaryWriter binaryWriter) =>
            bitWriter.WriteBit(FromHive);

        public override void ReceiveExtraAI(NPC npc, Terraria.ModLoader.IO.BitReader bitReader, System.IO.BinaryReader binaryReader) =>
            FromHive = bitReader.ReadBit();

        private static bool IsTool(Item item) => item.pick > 0 || item.axe > 0 || item.hammer > 0;

        public override bool? CanBeHitByItem(NPC npc, Player player, Item item) =>
            TamingBees.HiveOnly && FromHive && IsTool(item) ? false : null;

        // Foreuses et tronçonneuses : projectiles tenus de l'outil
        public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile) =>
            TamingBees.HiveOnly && FromHive && projectile.aiStyle == ProjAIStyleID.Drill ? false : null;
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

    /// <summary>
    /// Compte d'abeilles au-dessus du joueur dans une ruche : [validé 31/10] à chaque palier de 5 atteint (5, 10, 15...),
    /// au lieu de chaque seconde. Pendant le repos de la Queen Bee, le palier affiche le temps restant.
    /// </summary>
    public class TamingBeesPlayer : ModPlayer
    {
        private int shownStep;

        public override void PostUpdate()
        {
            if (Player.whoAmI != Main.myPlayer || Main.GameUpdateCount % 10 != 0 || TamingSystem.Untamable.Contains(TamingBees.Key))
                return;
            int count = TamingBees.CountAround(Player);
            int step = System.Math.Min(count, TamingBees.Goal) / 5;
            if (step > shownStep)
            {
                string text = TamingSystem.QueenBeeRest > 0
                    ? Language.GetTextValue("Mods.CalamityFandom.Taming.BeesResting", (TamingSystem.QueenBeeRest + 3599) / 3600)
                    : Language.GetTextValue("Mods.CalamityFandom.Taming.Bees", step * 5, TamingBees.Goal);
                CombatText.NewText(Player.Hitbox, Color.Gold, text);
            }
            shownStep = step;
        }
    }
}
