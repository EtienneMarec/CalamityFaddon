using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace CalamityFandom.Common.WikiData
{
    /// <summary>
    /// Résolution des noms du wiki vers les types de PNJ en jeu, par nom anglais. Les noms traduits n'existent qu'à
    /// partir de AddRecipeGroups : n'appeler qu'à partir de là (PostAddRecipes en pratique).
    /// </summary>
    public static class WikiNames
    {
        private static Dictionary<string, List<int>> npcsByName;

        public static void Reset() => npcsByName = null;

        private static Dictionary<string, List<int>> NPCs
        {
            get
            {
                if (npcsByName != null)
                    return npcsByName;
                npcsByName = new();
                for (int type = 1; type < NPCLoader.NPCCount; type++)
                {
                    string n = WikiRecipes.Normalize(Lang.GetNPCNameValue(type));
                    if (n.Length == 0)
                        continue;
                    if (!npcsByName.TryGetValue(n, out var list))
                        npcsByName[n] = list = new();
                    list.Add(type);
                }
                return npcsByName;
            }
        }

        /// <summary>Types de PNJ pour un titre de page ; à défaut, par les noms qui y redirigent.</summary>
        public static List<int> FindNPCs(string title, IEnumerable<string> aliases = null)
        {
            var types = Lookup(title).ToList();
            if (types.Count == 0 && aliases != null)
                types = aliases.SelectMany(Lookup).Distinct().ToList();
            return types;
        }

        private static IEnumerable<int> Lookup(string title)
        {
            string n = WikiRecipes.Normalize(title);
            if (NPCs.TryGetValue(n, out var list))
                return list;
            // « Hive (Enemy) » : désambiguïsation du wiki
            int paren = n.LastIndexOf(" (", StringComparison.Ordinal);
            if (paren > 0 && n.EndsWith(')') && NPCs.TryGetValue(n[..paren], out list))
                return list;
            return Array.Empty<int>();
        }
    }

    /// <summary>Vide le cache des noms à chaque déchargement.</summary>
    public class WikiNamesSystem : ModSystem
    {
        public override void Unload() => WikiNames.Reset();
        public override void OnModLoad() => WikiNames.Reset();
    }
}
