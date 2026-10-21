using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using CalamityFandom.Common.WikiData;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Common.Elements
{
    /// <summary>
    /// Éléments des armes et affinités des PNJ (WikiData/elements.json, généré par tools/gen_elements.py), résolus par
    /// nom anglais en jeu une fois tout le contenu chargé.
    /// </summary>
    public class ElementData : ModSystem
    {
        private class ElementsFile
        {
            public Dictionary<string, List<string>> Items { get; set; } = new();
            public Dictionary<string, AffinityData> Npcs { get; set; } = new();
            /// <summary>Titre de page -> autres noms qui y redirigent (ou alias manuels du glossaire).</summary>
            public Dictionary<string, List<string>> Aliases { get; set; } = new();
            /// <summary>Pages dont les éléments diffèrent en Full Fandom ({{mode}} sur la page) : remplacent Items / Npcs.</summary>
            public Dictionary<string, List<string>> FullItems { get; set; } = new();
            public Dictionary<string, AffinityData> FullNpcs { get; set; } = new();
        }

        private class AffinityData
        {
            public List<string> Resistant { get; set; } = new();
            public List<string> Vulnerable { get; set; } = new();
        }

        /// <summary>Résistant : 50 % des dégâts. Vulnérable : 200 %.</summary>
        public const float ResistMultiplier = 0.5f, VulnerableMultiplier = 2f;
        /// <summary>Bornes du multiplicateur quand une arme a plusieurs éléments (choix : docs/decisions.md).</summary>
        public const float MinMultiplier = 0.25f, MaxMultiplier = 4f;

        /// <summary>Type de debuff de statut par élément (-1 si l'élément n'a pas de statut durable).</summary>
        public static readonly int[] StatusBuffs = Enumerable.Repeat(-1, ElementInfo.Count).ToArray();

        public static Dictionary<int, Element[]> ItemElements { get; } = new();
        /// <summary>Affinité par type de PNJ : -1 résistant, +1 vulnérable, 0 neutre, pour chaque élément.</summary>
        public static Dictionary<int, sbyte[]> NPCAffinity { get; } = new();

        private static ElementsFile data;
        /// <summary>Titres du wiki sans correspondance en jeu (contenu pas encore créé), pour l'auto-test.</summary>
        public static List<string> UnresolvedItems { get; } = new();
        public static List<string> UnresolvedNPCs { get; } = new();

        private class BalanceFile
        {
            public Dictionary<string, float> ProcChance { get; set; } = new();
        }

        /// <summary>
        /// Chance qu'un coup inflige l'effet d'un élément : 100 % (le wiki : « weapons inflict effects characteristic of
        /// their element »), sauf en Calamity++ où Balance/elements.json la fixe par élément.
        /// </summary>
        public static readonly float[] ProcChance = Enumerable.Repeat(1f, ElementInfo.Count).ToArray();

        public override void Load()
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            data = JsonSerializer.Deserialize<ElementsFile>(Mod.GetFileBytes("WikiData/elements.json"), options);
            Array.Fill(ProcChance, 1f);
            if (Config.FandomMode.IsPlus && Mod.FileExists("Balance/elements.json"))
            {
                var balance = JsonSerializer.Deserialize<BalanceFile>(Mod.GetFileBytes("Balance/elements.json"), options);
                foreach (var (name, chance) in balance.ProcChance)
                    if (Enum.TryParse<Element>(name, true, out var e))
                        ProcChance[(int)e] = Math.Clamp(chance, 0f, 1f);
            }
        }

        public override void Unload()
        {
            data = null;
            ItemElements.Clear();
            NPCAffinity.Clear();
            UnresolvedItems.Clear();
            UnresolvedNPCs.Clear();
            Array.Fill(StatusBuffs, -1);
        }

        // Les noms traduits ne sont pas encore chargés en PostSetupContent : on résout après les recettes
        public override void PostAddRecipes()
        {
            var itemsByName = new Dictionary<string, List<int>>();
            for (int type = 1; type < ItemLoader.ItemCount; type++)
                AddName(itemsByName, Lang.GetItemNameValue(type), type);

            if (Config.FandomMode.IsFull)
            {
                foreach (var (title, names) in data.FullItems)
                    data.Items[title] = names;
                foreach (var (title, affinity) in data.FullNpcs)
                    data.Npcs[title] = affinity;
            }

            foreach (var (title, names) in data.Items)
            {
                var elements = Parse(names);
                var types = LookupWithAliases(itemsByName, title);
                if (types.Count == 0)
                    UnresolvedItems.Add(title);
                foreach (int type in types)
                    ItemElements[type] = elements;
            }

            foreach (var (title, affinity) in data.Npcs)
            {
                var row = new sbyte[ElementInfo.Count];
                foreach (var e in Parse(affinity.Resistant))
                    row[(int)e] = -1;
                foreach (var e in Parse(affinity.Vulnerable))
                    row[(int)e] = 1;
                data.Aliases.TryGetValue(title, out var npcAliases);
                var npcTypes = WikiNames.FindNPCs(title, npcAliases);
                if (npcTypes.Count == 0)
                    UnresolvedNPCs.Add(title);
                foreach (int type in npcTypes)
                {
                    NPCAffinity[type] = row;
                    // Résistant = immunisé contre l'effet de l'élément
                    for (int e = 0; e < ElementInfo.Count; e++)
                        if (row[e] < 0 && StatusBuffs[e] >= 0)
                            NPCID.Sets.SpecificDebuffImmunity[type][StatusBuffs[e]] = true;
                }
            }
        }

        private static void AddName(Dictionary<string, List<int>> map, string name, int type)
        {
            string n = WikiRecipes.Normalize(name);
            if (n.Length == 0)
                return;
            if (!map.TryGetValue(n, out var list))
                map[n] = list = new();
            list.Add(type);
        }

        /// <summary>Par le titre de la page ; à défaut, par les noms qui redirigent vers elle (« Signus » pour
        /// « Signus, Envoy of the Devourer »).</summary>
        private static List<int> LookupWithAliases(Dictionary<string, List<int>> map, string title)
        {
            var types = Lookup(map, title).ToList();
            if (types.Count == 0 && data.Aliases.TryGetValue(title, out var aliases))
                types = aliases.SelectMany(a => Lookup(map, a)).Distinct().ToList();
            return types;
        }

        private static IEnumerable<int> Lookup(Dictionary<string, List<int>> map, string title)
        {
            string n = WikiRecipes.Normalize(title);
            if (map.TryGetValue(n, out var list))
                return list;
            // « Elemental Disk (Rogue) », « Hive (Enemy) » : désambiguïsation du wiki
            int paren = n.LastIndexOf(" (", StringComparison.Ordinal);
            if (paren > 0 && n.EndsWith(')') && map.TryGetValue(n[..paren], out list))
                return list;
            return Array.Empty<int>();
        }

        private static Element[] Parse(IEnumerable<string> names) =>
            names.Select(n => Enum.TryParse<Element>(n, true, out var e) ? (Element?)e : null)
                 .Where(e => e.HasValue).Select(e => e.Value).Distinct().ToArray();

        public static Element[] Of(Item item) =>
            item != null && !item.IsAir && ItemElements.TryGetValue(item.type, out var e) ? e : Array.Empty<Element>();

        /// <summary>Multiplicateur de dégâts d'une attaque élémentaire contre un type de PNJ (produit par élément, borné).</summary>
        public static float Multiplier(IReadOnlyList<Element> elements, int npcType)
        {
            if (elements.Count == 0 || !NPCAffinity.TryGetValue(npcType, out var row))
                return 1f;
            float mult = 1f;
            foreach (var e in elements)
                mult *= row[(int)e] switch { < 0 => ResistMultiplier, > 0 => VulnerableMultiplier, _ => 1f };
            return Math.Clamp(mult, MinMultiplier, MaxMultiplier);
        }
    }
}
