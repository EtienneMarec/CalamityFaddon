using System;
using System.Collections.Generic;
using System.Text.Json;
using CalamityFandom.Common.WikiData;
using CalamityMod;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace CalamityFandom.Content.NPCs
{
    /// <summary>
    /// Ennemi inventé par le wiki, sans classe dédiée : une instance par entrée de WikiData/enemies.json (généré par
    /// tools/gen_enemies.py, sprite dans Assets/WikiEnemies/, nom dans Mods.CalamityFandom.WikiEnemies.hjson). IA vanilla la
    /// plus proche (slime, zombie, Demon Eye, chauve-souris), valeurs de l'infobox, debuff au contact, apparition d'après la
    /// zone de la page. Le butin vient de WikiLoot et les éléments d'ElementData, par le nom. Un ennemi qui doit faire plus
    /// mérite sa propre classe (le retirer alors de la génération).
    /// </summary>
    [Autoload(false)]
    public class WikiEnemy : ModNPC
    {
        public class Data
        {
            public string Name { get; set; }
            public string DisplayName { get; set; }
            public string Page { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public int Frames { get; set; } = 1;
            public string Ai { get; set; }
            public int Life { get; set; }
            public int Damage { get; set; }
            public int Defense { get; set; }
            public float KnockBackResist { get; set; }
            public int Money { get; set; }
            public string Debuff { get; set; }
            public int DebuffSeconds { get; set; }
            public List<string> Zones { get; set; } = new();
            public bool Hardmode { get; set; }
            public bool Postml { get; set; }
            public bool Night { get; set; }
        }

        private class EnemiesFile
        {
            public List<Data> Enemies { get; set; } = new();
        }

        private readonly Data data;
        private int debuffType = -1;

        public WikiEnemy(Data data) => this.data = data;

        public override string Name => data.Name;
        public override string Texture => $"CalamityFandom/Assets/WikiEnemies/{data.Name}";
        public override string LocalizationCategory => "WikiEnemies";
        protected override bool CloneNewInstances => true;

        public static readonly List<Data> All = new();

        public static void LoadAll(Mod mod)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var file = JsonSerializer.Deserialize<EnemiesFile>(mod.GetFileBytes("WikiData/enemies.json"), options);
            All.Clear();
            foreach (var entry in file.Enemies)
            {
                All.Add(entry);
                mod.AddContent(new WikiEnemy(entry));
            }
        }

        public override void SetStaticDefaults() => Main.npcFrameCount[Type] = Math.Max(1, data.Frames);

        public override void SetDefaults()
        {
            NPC.width = Math.Max(8, data.Width - 4);
            NPC.height = Math.Max(8, data.Height - 4);
            NPC.lifeMax = Math.Max(1, data.Life);
            NPC.damage = data.Damage;
            NPC.defense = data.Defense;
            NPC.knockBackResist = data.KnockBackResist;
            NPC.value = data.Money;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            switch (data.Ai)
            {
                case "slime":
                    NPC.aiStyle = NPCAIStyleID.Slime;
                    AIType = NPCID.BlueSlime;
                    break;
                case "bat":
                    NPC.aiStyle = NPCAIStyleID.Bat;
                    AIType = NPCID.CaveBat;
                    NPC.noGravity = true;
                    break;
                case "flier":
                    NPC.aiStyle = NPCAIStyleID.DemonEye;
                    AIType = NPCID.DemonEye;
                    NPC.noGravity = true;
                    break;
                default:
                    NPC.aiStyle = NPCAIStyleID.Fighter;
                    AIType = NPCID.Zombie;
                    break;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (data.Frames <= 1)
                return;
            if (++NPC.frameCounter >= 6)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y = (NPC.frame.Y + frameHeight) % (frameHeight * data.Frames);
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            foreach (string zone in data.Zones)
                if (Biome(zone) is IBestiaryInfoElement biome)
                    bestiaryEntry.Info.Add(biome);
        }

        private static IBestiaryInfoElement Biome(string zone) => zone switch
        {
            "surface" => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
            "underground" => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Underground,
            "cavern" => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,
            "snow" => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Snow,
            "desert" => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Desert,
            "jungle" => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Jungle,
            "ocean" => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Ocean,
            "hell" => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld,
            "dungeon" => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheDungeon,
            "space" => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,
            "hallow" => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheHallow,
            "corruption" => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
            "crimson" => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
            "mushroom" => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.SurfaceMushroom,
            "temple" => BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheTemple,
            _ => null,
        };

        /// <summary>
        /// Apparition : dans une des zones de la page, avec les drapeaux Hardmode / après le Moon Lord, la nuit si la page le
        /// dit. [choix] Taux : 5 % du taux normal (1 % au-delà de 5 000 PV), jamais en zone sûre.
        /// </summary>
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (data.Zones.Count == 0 || spawnInfo.PlayerSafe || spawnInfo.Invasion)
                return 0f;
            if ((data.Hardmode && !Main.hardMode) || (data.Postml && !NPC.downedMoonlord) || (data.Night && Main.dayTime))
                return 0f;
            Player p = spawnInfo.Player;
            var cal = p.Calamity();
            bool inZone = false;
            foreach (string zone in data.Zones)
                inZone |= zone switch
                {
                    "surface" => p.ZoneOverworldHeight && !p.ZoneSnow && !p.ZoneDesert && !p.ZoneJungle && !p.ZoneBeach,
                    "underground" => p.ZoneDirtLayerHeight || p.ZoneRockLayerHeight,
                    "cavern" => p.ZoneRockLayerHeight,
                    "snow" => p.ZoneSnow,
                    "desert" => p.ZoneDesert,
                    "jungle" => p.ZoneJungle,
                    "ocean" => p.ZoneBeach && !cal.ZoneSulphur,
                    "hell" => p.ZoneUnderworldHeight && !cal.ZoneCalamity,
                    "dungeon" => p.ZoneDungeon,
                    "space" => p.ZoneSkyHeight,
                    "hallow" => p.ZoneHallow,
                    "corruption" => p.ZoneCorrupt,
                    "crimson" => p.ZoneCrimson,
                    "mushroom" => p.ZoneGlowshroom,
                    "temple" => p.ZoneLihzhardTemple,
                    "crags" => cal.ZoneCalamity,
                    "sulphur" => cal.ZoneSulphur,
                    "abyss" => cal.ZoneAbyss,
                    "sunkensea" => cal.ZoneSunkenSea,
                    "astral" => cal.ZoneAstral,
                    _ => false,
                };
            if (!inZone)
                return 0f;
            return data.Life > 5000 ? 0.01f : 0.05f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (data.DebuffSeconds <= 0 || string.IsNullOrEmpty(data.Debuff))
                return;
            if (debuffType < 0)
                debuffType = FindBuff(data.Debuff);
            if (debuffType > 0)
                target.AddBuff(debuffType, data.DebuffSeconds * 60);
        }

        private static readonly Dictionary<string, int> buffsByName = new();

        /// <summary>Buff par son nom affiché (vanilla, Calamity, ce mod) ; 0 s'il n'existe pas.</summary>
        public static int FindBuff(string name)
        {
            if (buffsByName.Count == 0)
                for (int i = 1; i < BuffLoader.BuffCount; i++)
                    buffsByName.TryAdd(WikiRecipes.Normalize(Lang.GetBuffName(i)), i);
            return buffsByName.TryGetValue(WikiRecipes.Normalize(name), out int type) ? type : 0;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int i = 0; i < (NPC.life <= 0 ? 15 : 3); i++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f);
        }
    }
}
