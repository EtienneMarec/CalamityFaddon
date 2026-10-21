using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace CalamityFandom.Common.WikiData
{
    /// <summary>
    /// Butin des infobox de PNJ du wiki (WikiData/loot.json, généré par tools/gen_loot.py). Additif : seuls les objets
    /// que le PNJ ne lâche pas déjà sont ajoutés, avec la chance et la quantité du mode normal. Les entrées avec une
    /// condition en note (« Post-Golem », « Tier 3 ») ne sont pas appliquées. Sur un boss, l'ajout est limité au mode
    /// normal (en Expert, le butin passe par le sac, pas encore géré).
    /// </summary>
    public class WikiLoot : ModSystem
    {
        private class DropData
        {
            public string Item { get; set; }
            public int Min { get; set; } = 1;
            public int Max { get; set; } = 1;
            public double Chance { get; set; }
            public string Note { get; set; }
            public int Group { get; set; }
            public bool GroupAlways { get; set; }
        }

        private class NPCData
        {
            public string Page { get; set; }
            public bool Boss { get; set; }
            public List<DropData> Drops { get; set; } = new();
        }

        private class LootFile
        {
            public Dictionary<string, NPCData> Npcs { get; set; } = new();
            public Dictionary<string, List<string>> Aliases { get; set; } = new();
        }

        private static LootFile data;

        /// <summary>Règles ajoutées (type de PNJ, règle, description), pour l'auto-test et le rapport.</summary>
        public static List<(int npc, IItemDropRule rule, string what)> Added { get; } = new();
        public static int AlreadyDropped, Conditional;
        public static SortedDictionary<string, int> MissingItems { get; } = new();
        public static List<string> MissingNPCs { get; } = new();

        public override void Load()
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            data = JsonSerializer.Deserialize<LootFile>(Mod.GetFileBytes("WikiData/loot.json"), options);
        }

        public override void Unload()
        {
            data = null;
            Added.Clear();
            MissingItems.Clear();
            MissingNPCs.Clear();
            AlreadyDropped = Conditional = 0;
        }

        // Après les recettes : les noms traduits sont disponibles et la base de butin est remplie
        public override void PostAddRecipes()
        {
            foreach (var (title, npcData) in data.Npcs)
            {
                if (!title.Any(char.IsLetter))
                    continue; // « ??? » et autres titres sans nom
                data.Aliases.TryGetValue(title, out var aliases);
                var types = WikiNames.FindNPCs(title, aliases);
                if (types.Count == 0)
                {
                    MissingNPCs.Add(title);
                    continue;
                }
                // Un seul PNJ par page : segments de ver et alias partagent un nom, le butin ne doit pas se multiplier.
                // On prend celui qui a déjà le plus de butin (la tête d'un ver, le vrai boss d'une page).
                int target = types.OrderByDescending(t => Main.ItemDropsDB.GetRulesForNPCID(t, false).Count).ThenBy(t => t).First();
                ApplyToNPC(target, npcData);
            }
        }

        private static HashSet<int> CurrentDrops(int npc)
        {
            var rates = new List<DropRateInfo>();
            var feed = new DropRateInfoChainFeed(1f);
            foreach (var rule in Main.ItemDropsDB.GetRulesForNPCID(npc, false))
                rule.ReportDroprates(rates, feed);
            return rates.Select(r => r.itemId).ToHashSet();
        }

        private static void ApplyToNPC(int npc, NPCData npcData)
        {
            var current = CurrentDrops(npc);
            var groups = new Dictionary<int, List<int>>();
            foreach (var drop in npcData.Drops)
            {
                if (!string.IsNullOrEmpty(drop.Note))
                {
                    Conditional++;
                    continue;
                }
                int item = WikiRecipes.ResolveItem(drop.Item);
                if (item <= 0)
                {
                    MissingItems.TryGetValue(drop.Item, out int n);
                    MissingItems[drop.Item] = n + 1;
                    continue;
                }
                if (drop.Group > 0 && drop.GroupAlways)
                {
                    if (!groups.TryGetValue(drop.Group, out var list))
                        groups[drop.Group] = list = new();
                    list.Add(item);
                    continue;
                }
                if (current.Contains(item))
                {
                    AlreadyDropped++;
                    continue;
                }
                int numerator = Math.Clamp((int)Math.Round(drop.Chance * 10000), 1, 10000);
                var rule = new CommonDrop(item, 10000, Math.Max(1, drop.Min), Math.Max(drop.Min, drop.Max), numerator);
                Register(npc, npcData.Boss, rule, $"{Lang.GetItemNameValue(item)} {drop.Min}-{drop.Max} à {drop.Chance:P1}");
                current.Add(item);
            }

            // « One of the following N items will always drop » : un parmi le groupe, sauf si le jeu en donne déjà un
            foreach (var items in groups.Values)
            {
                if (items.Any(current.Contains))
                {
                    AlreadyDropped += items.Count;
                    continue;
                }
                var rule = items.Count == 1 ? ItemDropRule.Common(items[0]) : ItemDropRule.OneFromOptionsNotScalingWithLuck(1, items.ToArray());
                Register(npc, npcData.Boss, rule, "un parmi : " + string.Join(", ", items.Select(Lang.GetItemNameValue)));
                current.UnionWith(items);
            }
        }

        private static void Register(int npc, bool boss, IItemDropRule rule, string what)
        {
            IItemDropRule final = rule;
            if (boss)
            {
                var notExpert = new LeadingConditionRule(new Conditions.NotExpert());
                notExpert.OnSuccess(rule);
                final = notExpert;
                what += " (mode normal)";
            }
            Main.ItemDropsDB.RegisterToNPC(npc, final);
            Added.Add((npc, final, what));
        }

        public static void WriteReport(string folder)
        {
            var sb = new StringBuilder();
            sb.AppendLine("# Butin du wiki appliqué en jeu").AppendLine();
            sb.AppendLine("_Généré par l'auto-test à partir de `WikiData/loot.json`._").AppendLine();
            sb.AppendLine($"- {Added.Count} drops ajoutés sur {Added.Select(a => a.npc).Distinct().Count()} PNJ");
            sb.AppendLine($"- {AlreadyDropped} drops du wiki déjà présents dans le jeu (inchangés)");
            sb.AppendLine($"- {Conditional} drops à condition écrite en note, non appliqués (voir `reports/loot.md`)");
            sb.AppendLine($"- {MissingNPCs.Count} PNJ du wiki absents du jeu, {MissingItems.Count} objets introuvables").AppendLine();
            sb.AppendLine("## Ajouts").AppendLine();
            foreach (var group in Added.GroupBy(a => a.npc).OrderBy(g => Lang.GetNPCNameValue(g.Key)))
                sb.AppendLine($"- **{Lang.GetNPCNameValue(group.Key)}** : {string.Join(" ; ", group.Select(a => a.what))}");
            sb.AppendLine().AppendLine("## Objets introuvables (contenu à créer)").AppendLine();
            foreach (var (item, count) in MissingItems.OrderByDescending(kv => kv.Value))
                sb.AppendLine($"- {item} — {count}");
            File.WriteAllText(Path.Combine(folder, "loot_applied.md"), sb.ToString());
        }
    }
}
