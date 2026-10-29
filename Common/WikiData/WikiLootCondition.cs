using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod;
using CalamityMod.CalPlayer;
using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace CalamityFandom.Common.WikiData
{
    /// <summary>
    /// Condition d'un drop du wiki écrite en note (« (After defeating Golem) », « (When defeated in Space during
    /// Hardmode) », « (Always on first kill) »), lue par tools/gen_loot.py. Une note dont un boss n'est pas connu ici
    /// (« The Starborn Moth ») ne donne pas de condition : le drop reste non appliqué.
    /// </summary>
    public class WikiLootCondition : IItemDropRuleCondition
    {
        public class Data
        {
            /// <summary>Boss à avoir vaincus, noms du wiki en minuscules (« wall of flesh » = Hardmode).</summary>
            public List<string> Downed { get; set; } = new();
            public bool FirstKill { get; set; }
            /// <summary>« Once per player and world » (Purified Jam) : une fois par personnage et par monde.</summary>
            public bool OncePerPlayerWorld { get; set; }
            public string Biome { get; set; }
        }

        private static readonly Dictionary<string, Func<bool>> Bosses = new()
        {
            ["wall of flesh"] = () => Main.hardMode,
            ["skeletron"] = () => NPC.downedBoss3,
            ["plantera"] = () => NPC.downedPlantBoss,
            ["golem"] = () => NPC.downedGolemBoss,
            ["moon lord"] = () => NPC.downedMoonlord,
            ["desert scourge"] = () => DownedBossSystem.downedDesertScourge,
            ["cryogen"] = () => DownedBossSystem.downedCryogen,
            // « Calamitas » du wiki (2022) = le clone de Calamity actuel ; Supreme Calamitas a son propre nom
            ["calamitas"] = () => DownedBossSystem.downedCalamitasClone,
            ["astrum aureus"] = () => DownedBossSystem.downedAstrumAureus,
            ["astrum deus"] = () => DownedBossSystem.downedAstrumDeus,
            ["astral deus"] = () => DownedBossSystem.downedAstrumDeus,
            ["providence"] = () => DownedBossSystem.downedProvidence,
            ["polterghast"] = () => DownedBossSystem.downedPolterghast,
            ["storm weaver"] = () => DownedBossSystem.downedStormWeaver,
            ["ceaseless void"] = () => DownedBossSystem.downedCeaselessVoid,
            ["signus"] = () => DownedBossSystem.downedSignus,
            ["devourer of gods"] = () => DownedBossSystem.downedDoG,
        };

        private static readonly Dictionary<string, Func<Player, bool>> Biomes = new()
        {
            ["sunkensea"] = p => p.GetModPlayer<CalamityPlayer>().ZoneSunkenSea,
            ["space"] = p => p.ZoneSkyHeight,
            ["jungle"] = p => p.ZoneJungle,
            ["underworld"] = p => p.ZoneUnderworldHeight,
            ["snow"] = p => p.ZoneSnow,
        };

        private readonly Func<bool>[] downed;
        private readonly bool firstKill;
        private readonly bool oncePerPlayerWorld;
        private readonly Func<Player, bool> biome;
        private readonly string description;

        private WikiLootCondition(Func<bool>[] downed, bool firstKill, bool oncePerPlayerWorld, Func<Player, bool> biome, string description)
        {
            this.downed = downed;
            this.firstKill = firstKill;
            this.oncePerPlayerWorld = oncePerPlayerWorld;
            this.biome = biome;
            this.description = description;
        }

        /// <summary>Condition vérifiable sans PNJ ni joueur précis : utilisable dans un sac au trésor.</summary>
        public bool WorksInBag => !firstKill && biome == null;

        /// <summary>Condition du jeu pour ces données, ou null si un boss ou un biome n'est pas connu.</summary>
        public static WikiLootCondition TryCreate(Data data, string text)
        {
            if (data == null)
                return null;
            var checks = new List<Func<bool>>();
            foreach (string name in data.Downed ?? new())
            {
                if (!Bosses.TryGetValue(name, out var check))
                    return null;
                checks.Add(check);
            }
            Func<Player, bool> biome = null;
            if (data.Biome != null && !Biomes.TryGetValue(data.Biome, out biome))
                return null;
            return new WikiLootCondition(checks.ToArray(), data.FirstKill, data.OncePerPlayerWorld, biome, text);
        }

        public bool CanDrop(DropAttemptInfo info)
        {
            if (downed.Any(check => !check()))
                return false;
            // Le kill est compté dans le bestiaire avant le butin (NPC.NPCLoot) : premier kill = 1
            if (firstKill && (info.npc == null || Main.BestiaryTracker.Kills.GetKillCount(info.npc) > 1))
                return false;
            if (biome != null && (info.player == null || !biome(info.player)))
                return false;
            // Une seule fois par personnage dans ce monde : noté dans le monde au moment où le drop a lieu
            if (oncePerPlayerWorld)
                return info.player != null && WikiLootOnceSystem.TryTake($"{info.npc?.type ?? -1}|{description}|{info.player.name}");
            return true;
        }

        public bool CanShowItemDropInUI() => true;

        public string GetConditionDescription() => description;
    }

    /// <summary>Drops « une fois par personnage et par monde » déjà donnés (sauvegardés avec le monde).</summary>
    public class WikiLootOnceSystem : Terraria.ModLoader.ModSystem
    {
        private static readonly HashSet<string> Taken = new();

        public static bool TryTake(string key) => Taken.Add(key);

        public override void ClearWorld() => Taken.Clear();

        public override void SaveWorldData(Terraria.ModLoader.IO.TagCompound tag) => tag["wikiLootOnce"] = Taken.ToList();

        public override void LoadWorldData(Terraria.ModLoader.IO.TagCompound tag)
        {
            Taken.Clear();
            foreach (string key in tag.GetList<string>("wikiLootOnce"))
                Taken.Add(key);
        }
    }
}
