using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using CalamityFandom.Common.Config;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Common.WikiData
{
    /// <summary>
    /// Recettes du wiki (WikiData/recipes.json, généré par tools/gen_recipes.py) appliquées aux objets de Calamity et
    /// de ce mod. Les noms sont résolus par leur nom anglais en jeu (donc après nos renommages).
    /// Une recette existante est modifiée sur place (ses conditions et callbacks sont conservés) ; rien n'est touché si
    /// elle est déjà identique ou si un ingrédient / une station du wiki n'existe pas encore.
    /// </summary>
    public class WikiRecipes : ModSystem
    {
        private class IngredientData
        {
            public List<string> Items { get; set; } = new();
            public int Stack { get; set; }
        }

        private class RecipeData
        {
            public string Result { get; set; }
            public int Amount { get; set; } = 1;
            public List<string> Stations { get; set; } = new();
            public List<IngredientData> Ingredients { get; set; } = new();
            public string Page { get; set; }
        }

        private class ShimmerData
        {
            public string From { get; set; }
            public string To { get; set; }
            public string Station { get; set; }
        }

        private class RecipesFile
        {
            public List<RecipeData> Recipes { get; set; } = new();
            public List<ShimmerData> Shimmer { get; set; } = new();
            public Dictionary<string, string> Aliases { get; set; } = new();
        }

        /// <summary>Recette résolue : types d'objets, groupes, tuiles et conditions prêts à appliquer.</summary>
        public class Resolved
        {
            public int Result;
            public int Amount;
            public List<(HashSet<int> accepted, int groupId, int stack)> Ingredients = new();
            public List<int> Tiles = new();
            public bool NeedsWater;
            public string Page;
        }

        private static RecipesFile data;
        private static Dictionary<string, int> itemsByName;
        private static Dictionary<string, int> groupsByText;
        private static Dictionary<string, string> aliases;

        /// <summary>Résultats modifiés et recettes attendues, pour l'auto-test.</summary>
        public static Dictionary<int, List<Resolved>> Applied { get; } = new();
        /// <summary>Noms introuvables (objets, stations) → nombre de recettes bloquées.</summary>
        public static Dictionary<string, int> Missing { get; } = new();
        public static int Unchanged, Edited, Created, Kept, DisabledNotOnWiki, ShimmerSet;
        /// <summary>Détail lisible des changements (objet, avant, après) pour le rapport.</summary>
        public static List<(int result, string before, string after)> Changes { get; } = new();

        // Stations vanilla dont le nom du wiki ne correspond pas à un objet qui pose la bonne tuile
        private static readonly Dictionary<string, int> SpecialStations = new()
        {
            ["iron anvil"] = TileID.Anvils,
            ["mythril anvil"] = TileID.MythrilAnvil,
            ["adamantite forge"] = TileID.AdamantiteForge,
            ["furnace"] = TileID.Furnaces,
            ["work bench"] = TileID.WorkBenches,
            ["cooking pot"] = TileID.CookingPots,
            ["bookcase"] = TileID.Bookcases,
            ["placed bottle"] = TileID.Bottles,
            ["alchemy table only"] = TileID.AlchemyTable,
            ["demon altar"] = TileID.DemonAltar,
            ["ancient manipulator"] = TileID.LunarCraftingStation,
            ["tinkerer's workshop"] = TileID.TinkerersWorkbench,
            ["hellforge"] = TileID.Hellforge,
        };

        public static string Normalize(string name) =>
            string.Join(' ', name.Replace('’', '\'').Trim().ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries));

        public override void Load()
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            data = JsonSerializer.Deserialize<RecipesFile>(Mod.GetFileBytes("WikiData/recipes.json"), options);
            aliases = data.Aliases.ToDictionary(kv => Normalize(kv.Key), kv => kv.Value);
        }

        public override void Unload()
        {
            data = null;
            itemsByName = null;
            groupsByText = null;
            Applied.Clear();
            Missing.Clear();
            Changes.Clear();
            Unchanged = Edited = Created = Kept = DisabledNotOnWiki = ShimmerSet = 0;
        }

        public override void AddRecipeGroups()
        {
            BuildItemNames();
            // Groupes « A/B » du wiki : réutilise un groupe existant qui contient exactement ces objets, sinon en crée un
            groupsByText = new();
            foreach (var group in RecipeGroup.recipeGroups.Values)
                groupsByText.TryAdd(Normalize(group.GetText()), group.RegisteredId);

            foreach (var ingredient in data.Recipes.SelectMany(r => r.Ingredients).Where(i => i.Items.Count > 1))
            {
                string key = string.Join("/", ingredient.Items.Select(Normalize));
                if (groupsByText.ContainsKey(key))
                    continue;
                var types = ingredient.Items.Select(ResolveItem).ToList();
                if (types.Any(t => t <= 0))
                    continue;
                var set = types.ToHashSet();
                var existing = RecipeGroup.recipeGroups.Values.FirstOrDefault(g => g.ValidItems.SetEquals(set));
                int id = existing?.RegisteredId ?? RecipeGroup.RegisterGroup($"CalamityFandom:{key}",
                    new RecipeGroup(() => string.Join(" / ", types.Select(Lang.GetItemNameValue)), types.ToArray()));
                groupsByText[key] = id;
            }
        }

        public override void PostAddRecipes()
        {
            var byResult = new Dictionary<int, List<Resolved>>();
            foreach (var recipe in data.Recipes)
            {
                var resolved = Resolve(recipe);
                if (resolved == null)
                    continue;
                if (!byResult.TryGetValue(resolved.Result, out var list))
                    byResult[resolved.Result] = list = new();
                list.Add(resolved);
            }

            foreach (var (result, wanted) in byResult)
            {
                // Une recette du wiki produit cet objet : le wiki le connaît, il ne doit pas être masqué en Full Fandom
                // (cas des noms que tools/gen_baseline.py ne sait pas lire hors du jeu, ex. objets de lore)
                Modes.FullFandomHiding.HiddenItems.Remove(result);
                ApplyToResult(result, wanted);
            }

            ApplyShimmer();
        }

        private static void BuildItemNames()
        {
            // Priorité en cas d'homonymes : ce mod, puis Calamity, puis vanilla, puis le reste
            static int Rank(int type) => ItemLoader.GetItem(type)?.Mod.Name switch
            {
                "CalamityFandom" => 0,
                "CalamityMod" => 1,
                null => 2,
                _ => 3,
            };

            itemsByName = new();
            var ranks = new Dictionary<string, int>();
            for (int type = 1; type < ItemLoader.ItemCount; type++)
            {
                string name = Normalize(Lang.GetItemNameValue(type));
                if (name.Length == 0)
                    continue;
                int rank = Rank(type);
                if (!ranks.TryGetValue(name, out int best) || rank < best)
                {
                    ranks[name] = rank;
                    itemsByName[name] = type;
                }
            }
        }

        public static int ResolveItem(string name)
        {
            string n = Normalize(name);
            if (itemsByName.TryGetValue(n, out int type))
                return type;
            // « Elemental Disk (Rogue) » : désambiguïsation du wiki
            int paren = n.LastIndexOf(" (", StringComparison.Ordinal);
            if (paren > 0 && n.EndsWith(')') && itemsByName.TryGetValue(n[..paren], out type))
                return type;
            // Ancien nom encore utilisé par le wiki : on suit sa redirection
            if (aliases.TryGetValue(n, out string target) && Normalize(target) != n)
                return ResolveItem(target);
            return -1;
        }

        private static void NoteMissing(string what)
        {
            Missing.TryGetValue(what, out int count);
            Missing[what] = count + 1;
        }

        private static Resolved Resolve(RecipeData recipe)
        {
            int result = ResolveItem(recipe.Result);
            if (result <= 0)
                return null; // contenu pas encore créé : rien à faire
            string owner = ItemLoader.GetItem(result)?.Mod.Name;
            if (owner != "CalamityMod" && owner != "CalamityFandom")
                return null; // recettes vanilla laissées telles quelles pour l'instant

            var resolved = new Resolved { Result = result, Amount = Math.Max(1, recipe.Amount), Page = recipe.Page };
            bool ok = true;
            foreach (var ingredient in recipe.Ingredients)
            {
                if (ingredient.Items.Count == 1)
                {
                    string n = Normalize(ingredient.Items[0]);
                    int type = ResolveItem(n);
                    if (type > 0)
                    {
                        resolved.Ingredients.Add((new HashSet<int> { type }, -1, ingredient.Stack));
                        continue;
                    }
                    if (groupsByText.TryGetValue(n, out int groupId)
                        || aliases.TryGetValue(n, out string groupAlias) && groupsByText.TryGetValue(Normalize(groupAlias), out groupId))
                    {
                        resolved.Ingredients.Add((RecipeGroup.recipeGroups[groupId].ValidItems.ToHashSet(), groupId, ingredient.Stack));
                        continue;
                    }
                    NoteMissing("objet : " + ingredient.Items[0]);
                    ok = false;
                }
                else
                {
                    string key = string.Join("/", ingredient.Items.Select(Normalize));
                    if (groupsByText.TryGetValue(key, out int groupId))
                    {
                        resolved.Ingredients.Add((RecipeGroup.recipeGroups[groupId].ValidItems.ToHashSet(), groupId, ingredient.Stack));
                        continue;
                    }
                    foreach (string alt in ingredient.Items.Where(a => ResolveItem(a) <= 0))
                        NoteMissing("objet : " + alt);
                    ok = false;
                }
            }

            foreach (string station in recipe.Stations)
            {
                string n = Normalize(station);
                if (n == "by hand")
                    continue;
                if (n == "water")
                {
                    resolved.NeedsWater = true;
                    continue;
                }
                if (SpecialStations.TryGetValue(n, out int tile))
                {
                    resolved.Tiles.Add(tile);
                    continue;
                }
                int stationItem = ResolveItem(n);
                if (stationItem > 0 && ContentSamples.ItemsByType[stationItem].createTile >= 0)
                {
                    resolved.Tiles.Add(ContentSamples.ItemsByType[stationItem].createTile);
                    continue;
                }
                NoteMissing("station : " + station);
                ok = false;
            }
            return ok ? resolved : null;
        }

        /// <summary>Signature comparable d'une recette : (objets acceptés, quantité) + tuiles + eau.</summary>
        public static string Signature(int amount, IEnumerable<(HashSet<int> accepted, int stack)> ingredients, IEnumerable<int> tiles, bool water)
        {
            var sb = new StringBuilder().Append(amount).Append('|');
            foreach (string part in ingredients.Select(i => string.Join(",", i.accepted.OrderBy(t => t)) + "x" + i.stack).OrderBy(s => s))
                sb.Append(part).Append(';');
            sb.Append('|').Append(string.Join(",", tiles.Distinct().OrderBy(t => t))).Append('|').Append(water);
            return sb.ToString();
        }

        public static string Signature(Resolved r) =>
            Signature(r.Amount, r.Ingredients.Select(i => (i.accepted, i.stack)), r.Tiles, r.NeedsWater);

        public static string Signature(Recipe recipe)
        {
            var ingredients = recipe.requiredItem.Select(item =>
            {
                // Un ingrédient de groupe est représenté par l'objet iconique du groupe
                foreach (int groupId in recipe.acceptedGroups)
                {
                    var group = RecipeGroup.recipeGroups[groupId];
                    if (group.ContainsItem(item.type))
                        return (group.ValidItems.ToHashSet(), item.stack);
                }
                return (new HashSet<int> { item.type }, item.stack);
            });
            bool water = recipe.Conditions.Contains(Condition.NearWater);
            return Signature(recipe.createItem.stack, ingredients, recipe.requiredTile, water);
        }

        private static void ApplyToResult(int result, List<Resolved> wanted)
        {
            // Le wiki liste parfois la même recette sur deux pages
            wanted = wanted.GroupBy(Signature).Select(g => g.First()).ToList();
            var existing = Main.recipe.Take(Recipe.numRecipes).Where(r => r.createItem.type == result && !r.Disabled).ToList();

            // Recettes déjà identiques : gardées telles quelles
            var remainingWanted = new List<Resolved>();
            var unmatched = existing.ToList();
            foreach (var target in wanted)
            {
                string sig = Signature(target);
                var same = unmatched.FirstOrDefault(r => Signature(r) == sig);
                if (same != null)
                {
                    unmatched.Remove(same);
                    Unchanged++;
                }
                else
                    remainingWanted.Add(target);
            }
            if (remainingWanted.Count == 0 && (unmatched.Count == 0 || !FandomMode.IsFull))
                return;

            Applied[result] = wanted;
            var template = existing.FirstOrDefault();
            foreach (var target in remainingWanted)
            {
                // On réécrit la recette existante la plus proche (ingrédients ou station en commun) ; s'il n'y en a
                // qu'une de chaque côté, c'est forcément la même recette remaniée. Sinon on en crée une nouvelle.
                var best = unmatched.Select(r => (recipe: r, score: Similarity(r, target)))
                    .OrderByDescending(x => x.score).FirstOrDefault();
                bool reuse = best.recipe != null && (best.score > 0 || (unmatched.Count == 1 && remainingWanted.Count == 1));

                Recipe recipe;
                string before;
                if (reuse)
                {
                    recipe = best.recipe;
                    unmatched.Remove(recipe);
                    before = Describe(recipe);
                    Edited++;
                }
                else
                {
                    recipe = template != null ? template.Clone() : Recipe.Create(result);
                    recipe.Register();
                    before = "(nouvelle)";
                    Created++;
                }
                Rewrite(recipe, target);
                Changes.Add((result, before, Describe(recipe)));
            }
            // Recettes du jeu absentes du wiki (souvent des recettes inverses mur -> bloc) : conservées, sauf en Full
            // Fandom où seul ce qui est écrit sur le wiki existe
            if (FandomMode.IsFull)
            {
                foreach (var extra in unmatched)
                {
                    Changes.Add((result, Describe(extra), "(désactivée : absente du wiki)"));
                    extra.DisableRecipe();
                    DisabledNotOnWiki++;
                }
            }
            else
                Kept += unmatched.Count;
        }

        private static int Similarity(Recipe recipe, Resolved target)
        {
            var types = target.Ingredients.SelectMany(i => i.accepted).ToHashSet();
            int score = recipe.requiredItem.Count(item => types.Contains(item.type));
            score += recipe.requiredTile.Count(target.Tiles.Contains);
            return score;
        }

        public static string Describe(Recipe recipe)
        {
            var parts = recipe.requiredItem.Select(item =>
            {
                int groupId = recipe.acceptedGroups.FirstOrDefault(g => RecipeGroup.recipeGroups[g].ContainsItem(item.type), -1);
                string name = groupId >= 0 ? RecipeGroup.recipeGroups[groupId].GetText() : Lang.GetItemNameValue(item.type);
                return $"{item.stack} {name}";
            });
            var tiles = recipe.requiredTile.Select(t => TileLoader.GetTile(t)?.Name ?? TileID.Search.GetName(t));
            string water = recipe.Conditions.Contains(Condition.NearWater) ? " + eau" : "";
            return $"{string.Join(", ", parts)} @ {string.Join(", ", tiles.DefaultIfEmpty("main"))}{water} → {recipe.createItem.stack}";
        }

        private static void Rewrite(Recipe recipe, Resolved target)
        {
            recipe.createItem.stack = target.Amount;
            recipe.requiredItem.Clear();
            recipe.acceptedGroups.Clear();
            foreach (var (accepted, groupId, stack) in target.Ingredients)
            {
                if (groupId >= 0)
                    recipe.AddRecipeGroup(groupId, stack);
                else
                    recipe.AddIngredient(accepted.First(), stack);
            }
            recipe.requiredTile.Clear();
            foreach (int tile in target.Tiles.Distinct())
                recipe.AddTile(tile);
            recipe.Conditions.RemoveAll(c => c == Condition.NearWater);
            if (target.NeedsWater)
                recipe.AddCondition(Condition.NearWater);
        }

        private static void ApplyShimmer()
        {
            foreach (var s in data.Shimmer)
            {
                string station = Normalize(s.Station ?? "");
                if (station != "shimmer" && station != "shimmer transmutation")
                    continue; // « Sanguine Shimmer » est une mécanique inventée : pas le Shimmer vanilla
                int from = ResolveItem(s.From), to = ResolveItem(s.To);
                if (from <= 0 || to <= 0)
                    continue;
                bool ours = ItemLoader.GetItem(from)?.Mod.Name is "CalamityMod" or "CalamityFandom"
                    || ItemLoader.GetItem(to)?.Mod.Name is "CalamityMod" or "CalamityFandom";
                if (!ours || ItemID.Sets.ShimmerTransformToItem[from] == to)
                    continue;
                ItemID.Sets.ShimmerTransformToItem[from] = to;
                ShimmerSet++;
            }
        }

        /// <summary>Écrit le bilan (appelé par l'auto-test) dans le dossier indiqué par CALFANDOM_REPORTS.</summary>
        public static void WriteReport(string folder)
        {
            var sb = new StringBuilder();
            sb.AppendLine("# Recettes du wiki appliquées en jeu").AppendLine();
            sb.AppendLine("_Généré par l'auto-test (`tools/selftest.py`) à partir de `WikiData/recipes.json`._").AppendLine();
            sb.AppendLine($"- {Applied.Count} objets dont les recettes ont changé : {Edited} recettes modifiées, {Created} créées");
            sb.AppendLine($"- {Kept} recettes du jeu absentes du wiki conservées (ex. recettes inverses mur → bloc) ; désactivées en Full Fandom");
            sb.AppendLine($"- {Unchanged} recettes du wiki déjà identiques au jeu");
            sb.AppendLine($"- {ShimmerSet} transmutations Shimmer ajoutées");
            sb.AppendLine($"- {Missing.Count} noms introuvables (contenu à créer ou nom à corriger), qui bloquent {Missing.Values.Sum()} recettes").AppendLine();
            sb.AppendLine("## Recettes changées").AppendLine();
            sb.AppendLine("| Objet | Avant | Après |").AppendLine("|---|---|---|");
            foreach (var (result, before, after) in Changes.OrderBy(c => Lang.GetItemNameValue(c.result)))
                sb.AppendLine($"| {Lang.GetItemNameValue(result)} | {before} | {after} |");
            sb.AppendLine().AppendLine("## Noms introuvables (par nombre de recettes bloquées)").AppendLine();
            foreach (var (name, count) in Missing.OrderByDescending(kv => kv.Value).ThenBy(kv => kv.Key))
                sb.AppendLine($"- {name} — {count}");
            File.WriteAllText(Path.Combine(folder, "recipes.md"), sb.ToString());
        }
    }
}
