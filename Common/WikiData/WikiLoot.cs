using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using CalamityFandom.Common.Config;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Common.WikiData
{
    /// <summary>
    /// Butin des infobox de PNJ du wiki et contenu des sacs de la page « Treasure Bag » (WikiData/loot.json, généré par
    /// tools/gen_loot.py). Additif : seuls les objets que le PNJ (ou le sac) ne donne pas déjà sont ajoutés. Les entrées
    /// avec une condition en note (« Post-Golem », « Tier 3 ») ne sont pas appliquées.
    /// Boss (décision du 22/10) : en mode normal, les drops de la page tombent directement. En Expert :
    /// - Calamity++ : ils vont dans le sac (quantités Expert du {{dv}}), avec ce que liste la page Treasure Bag ;
    /// - Full Fandom : ils tombent aussi directement (quantités Expert), et le sac contient ce que liste la page
    ///   Treasure Bag, rien d'autre.
    /// </summary>
    public class WikiLoot : ModSystem
    {
        private class DropData
        {
            public string Item { get; set; }
            public int Min { get; set; } = 1;
            public int Max { get; set; } = 1;
            public double Chance { get; set; }
            /// <summary>Valeurs Expert du {{dv}}, quand elles diffèrent du mode normal.</summary>
            public int? MinExpert { get; set; }
            public int? MaxExpert { get; set; }
            public double? ChanceExpert { get; set; }
            public string Note { get; set; }
            /// <summary>Condition de la note comprise par gen_loot.py (boss vaincus, biome, premier kill).</summary>
            public WikiLootCondition.Data Condition { get; set; }
            public string ConditionText { get; set; }
            public int Group { get; set; }
            public bool GroupAlways { get; set; }
            /// <summary>Objet seulement cité (ligne « custom » sans chance : sac, relique) : pas ajouté, pas retiré.</summary>
            public bool Listed { get; set; }
            /// <summary>Drop propre à un mode (« fandom » / « plus »), tiré d'un {{mode}} ; vide = les deux.</summary>
            public List<string> Modes { get; set; }
        }

        private class NPCData
        {
            public string Page { get; set; }
            public bool Boss { get; set; }
            /// <summary>PNJ connu seulement par les infobox « item drops » des pages d'objets (Wall of Flesh...) : sa
            /// liste de butin est incomplète, le Full Fandom ne lui retire rien.</summary>
            public bool ItemPagesOnly { get; set; }
            public List<DropData> Drops { get; set; } = new();
        }

        private class BagData
        {
            public string Bag { get; set; }
            public List<DropData> Drops { get; set; } = new();
        }

        /// <summary>Source générique d'une page d'objet (« any Jungle enemy », « Any Boss ») : règle globale filtrée.</summary>
        private class GenericData
        {
            public string Item { get; set; }
            public int Min { get; set; } = 1;
            public int Max { get; set; } = 1;
            public double Chance { get; set; }
            /// <summary>« enemy » ou « boss ».</summary>
            public string Target { get; set; }
            public bool Revengeance { get; set; }
            public WikiLootCondition.Data Condition { get; set; }
            public string Source { get; set; }
        }

        /// <summary>
        /// « Any X enemy » : PNJ hostile qui n'est ni un boss, ni un PNJ de statue, ni une bestiole ; « Any Boss » : boss.
        /// Puis la condition de la page (biome du joueur, boss vaincus) et le mode Revengeance.
        /// </summary>
        private class GenericCondition : IItemDropRuleCondition
        {
            private readonly bool boss, revengeance;
            private readonly WikiLootCondition inner;
            private readonly string description;

            public GenericCondition(bool boss, bool revengeance, WikiLootCondition inner, string description)
            {
                this.boss = boss;
                this.revengeance = revengeance;
                this.inner = inner;
                this.description = description;
            }

            public bool CanDrop(DropAttemptInfo info)
            {
                NPC npc = info.npc;
                if (npc == null || revengeance && !CalamityMod.World.CalamityWorld.revenge)
                    return false;
                bool target = boss ? npc.boss
                    : !npc.boss && !npc.friendly && !npc.townNPC && npc.lifeMax > 5 && !npc.SpawnedFromStatue && !npc.CountsAsACritter;
                return target && (inner == null || inner.CanDrop(info));
            }

            // Pas dans le bestiaire : la règle vaut pour tous les PNJ, chaque fiche l'afficherait
            public bool CanShowItemDropInUI() => false;

            public string GetConditionDescription() => description;
        }

        public static List<string> GenericAdded { get; } = new();

        private static void ApplyGeneric()
        {
            foreach (var g in data.Generic)
            {
                int item = WikiRecipes.ResolveItem(data.ItemAliases.GetValueOrDefault(g.Item, g.Item));
                if (item <= 0)
                    item = WikiRecipes.ResolveItem(g.Item);
                if (item <= 0)
                {
                    MissingItems[g.Item] = MissingItems.GetValueOrDefault(g.Item) + 1;
                    continue;
                }
                WikiLootCondition inner = null;
                if (g.Condition != null && (inner = WikiLootCondition.TryCreate(g.Condition, g.Source)) == null)
                    continue;
                int denominator = 10000, numerator = Math.Max(1, (int)Math.Round(g.Chance * denominator));
                var condition = new GenericCondition(g.Target == "boss", g.Revengeance, inner, g.Source);
                Main.ItemDropsDB.RegisterToGlobal(ItemDropRule.ByCondition(condition, item, g.Min, g.Max, numerator, denominator));
                GenericAdded.Add($"{Lang.GetItemNameValue(item)} : {g.Source} ({g.Chance:P1})");
            }
        }

        private class LootFile
        {
            public Dictionary<string, NPCData> Npcs { get; set; } = new();
            /// <summary>Page « Treasure Bag » : titre du boss -> sac et contenu.</summary>
            public Dictionary<string, BagData> Bags { get; set; } = new();
            public Dictionary<string, List<string>> Aliases { get; set; } = new();
            /// <summary>Anciens noms d'objets (redirections du wiki, renommages) : « Victory Shard » -> Conquest Fragment.</summary>
            public Dictionary<string, string> ItemAliases { get; set; } = new();
            /// <summary>Sources génériques des pages d'objets (« any Jungle enemy »).</summary>
            public List<GenericData> Generic { get; set; } = new();
        }

        private static LootFile data;

        /// <summary>Règles ajoutées (type de PNJ, règle, description), pour l'auto-test et le rapport.</summary>
        public static List<(int npc, IItemDropRule rule, string what)> Added { get; } = new();
        /// <summary>Règles ajoutées aux sacs (type du sac, règle, description).</summary>
        public static List<(int bag, IItemDropRule rule, string what)> AddedToBags { get; } = new();
        public static int AlreadyDropped, Conditional, Conditioned, RemovedNotOnWiki, RemovedFromBags, ReplacedByCondition;
        /// <summary>Lignes de butin écrites dans le bestiaire, par PNJ qui a sa propre entrée (pour l'auto-test).</summary>
        public static Dictionary<int, int> BestiaryLines { get; } = new();
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
            Keep.Clear();
            Added.Clear();
            AddedToBags.Clear();
            BestiaryLines.Clear();
            MissingItems.Clear();
            MissingNPCs.Clear();
            GenericAdded.Clear();
            AlreadyDropped = Conditional = Conditioned = RemovedNotOnWiki = RemovedFromBags = ReplacedByCondition = 0;
        }

        // Après les recettes : les noms traduits sont disponibles et la base de butin est remplie
        public override void PostAddRecipes()
        {
            ApplyBags();
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
                // On prend celui qui a déjà le plus de butin (la tête d'un ver, le vrai boss d'une page) ; à égalité, pas un
                // segment (dontCountMe : corps et queue d'un ver porté sans butin, comme l'Armored Digger)
                int target = types.OrderByDescending(t => Main.ItemDropsDB.GetRulesForNPCID(t, false).Count)
                    .ThenBy(t => ContentSamples.NpcsByNetId.TryGetValue(t, out var n) && n.dontCountMe)
                    .ThenBy(t => t).First();
                ApplyToNPC(target, npcData);
            }
            ApplyGeneric();
            RefreshBestiary();
        }

        // ------------------------------------------------------------------ outils

        private static List<DropRateInfo> Rates(IEnumerable<IItemDropRule> rules)
        {
            var rates = new List<DropRateInfo>();
            var feed = new DropRateInfoChainFeed(1f);
            foreach (var rule in rules)
                rule.ReportDroprates(rates, feed);
            return rates;
        }

        /// <summary>Pour chaque objet donné par ces règles : tombe-t-il en mode normal, en Expert ?</summary>
        private static Dictionary<int, (bool normal, bool expert)> Availability(IEnumerable<IItemDropRule> rules)
        {
            var result = new Dictionary<int, (bool normal, bool expert)>();
            foreach (var rate in Rates(rules))
            {
                bool normalOnly = rate.conditions?.Any(c => c is Conditions.NotExpert) == true;
                bool expertOnly = rate.conditions?.Any(c => c is Conditions.IsExpert) == true;
                result.TryGetValue(rate.itemId, out var prev);
                result[rate.itemId] = (prev.normal || !expertOnly, prev.expert || !normalOnly);
            }
            return result;
        }

        /// <summary>Sac au trésor qu'un boss lâche en Expert (ou -1).</summary>
        private static int BossBag(int npc) =>
            Rates(Main.ItemDropsDB.GetRulesForNPCID(npc, false)).Select(r => r.itemId).FirstOrDefault(i => ItemID.Sets.BossBag[i], -1);

        private static int Resolve(DropData drop)
        {
            int item = WikiRecipes.ResolveItem(drop.Item);
            if (item <= 0 && data.ItemAliases.TryGetValue(drop.Item, out string alias))
                item = WikiRecipes.ResolveItem(alias);
            if (item <= 0)
            {
                MissingItems.TryGetValue(drop.Item, out int n);
                MissingItems[drop.Item] = n + 1;
            }
            return item;
        }

        private static IItemDropRule Drop(int item, int min, int max, double chance)
        {
            int numerator = Math.Clamp((int)Math.Round(chance * 10000), 1, 10000);
            return new CommonDrop(item, 10000, Math.Max(1, min), Math.Max(min, max), numerator);
        }

        private static IItemDropRule NormalDrop(int item, DropData d) => Drop(item, d.Min, d.Max, d.Chance);

        private static IItemDropRule ExpertDrop(int item, DropData d) =>
            Drop(item, d.MinExpert ?? d.Min, d.MaxExpert ?? d.Max, d.ChanceExpert ?? d.Chance);

        private static IItemDropRule When(IItemDropRuleCondition condition, IItemDropRule rule)
        {
            var leading = new LeadingConditionRule(condition);
            leading.OnSuccess(rule);
            return leading;
        }

        private static void RegisterNPC(int npc, IItemDropRule rule, string what)
        {
            Main.ItemDropsDB.RegisterToNPC(npc, rule);
            Added.Add((npc, rule, what));
        }

        private static void RegisterBag(int bag, IItemDropRule rule, string what)
        {
            Main.ItemDropsDB.RegisterToItem(bag, rule);
            AddedToBags.Add((bag, rule, what));
        }

        private static string Describe(int item, int min, int max, double chance) =>
            $"{Lang.GetItemNameValue(item)} {min}-{max} à {chance:P1}";

        // ------------------------------------------------------------------ PNJ

        private static void ApplyToNPC(int npc, NPCData npcData)
        {
            if (FandomMode.IsFull && !npcData.ItemPagesOnly)
                RemoveNotOnWiki(npc, npcData);
            var avail = Availability(Main.ItemDropsDB.GetRulesForNPCID(npc, false));
            // Boss vanilla sans page sur ce wiki (Wall of Flesh, cité par des pages d'objets) : reconnu par le jeu
            bool boss = npcData.Boss || ContentSamples.NpcsByNetId.TryGetValue(npc, out var sample) && sample.boss;
            int bag = boss ? BossBag(npc) : -1;
            var bagAvail = bag > 0 ? Availability(Main.ItemDropsDB.GetRulesForItemID(bag)) : new();
            // En Expert, le drop tombe directement en Full Fandom, ou faute de sac ; sinon il va dans le sac
            bool expertDirect = FandomMode.IsFull || bag <= 0;
            var groups = new Dictionary<int, List<(int item, DropData drop)>>();

            foreach (var drop in npcData.Drops)
            {
                if (!FandomMode.Applies(drop.Modes) || drop.Listed)
                    continue;
                // Condition en note : appliquée si le jeu sait la vérifier, sinon le drop reste de côté (rapport)
                WikiLootCondition condition = null;
                if (!string.IsNullOrEmpty(drop.Note))
                {
                    condition = WikiLootCondition.TryCreate(drop.Condition, drop.ConditionText);
                    if (condition == null || drop.Group > 0 && drop.GroupAlways)
                    {
                        Conditional++;
                        continue;
                    }
                }
                int item = Resolve(drop);
                if (item <= 0)
                    continue;
                if (condition != null)
                    Conditioned++;
                if (drop.Group > 0 && drop.GroupAlways)
                {
                    if (!groups.TryGetValue(drop.Group, out var list))
                        groups[drop.Group] = list = new();
                    list.Add((item, drop));
                    continue;
                }
                AddOne(item, drop, condition);
            }

            void AddOne(int item, DropData drop, WikiLootCondition condition)
            {
                // Condition du wiki pour un objet que le jeu donne déjà autrement (Depth Cells du Devil Fish : après
                // Calamitas sur le wiki, après Leviathan dans Calamity) : la version du wiki remplace la règle du jeu,
                // si cette règle ne donne que cet objet ([choix] du 22/10)
                if (condition != null && avail.ContainsKey(item) && RemoveSingleItemRules(npc, item) > 0)
                {
                    avail.Remove(item);
                    ReplacedByCondition++;
                }
                IItemDropRule If(IItemDropRule rule) => condition == null ? rule : When(condition, rule);
                string when = condition == null ? "" : $" [{drop.ConditionText}]";
                string what = Describe(item, drop.Min, drop.Max, drop.Chance) + when;
                if (!boss)
                {
                    if (avail.ContainsKey(item))
                        AlreadyDropped++;
                    else
                        RegisterNPC(npc, If(NormalDrop(item, drop)), what);
                    return;
                }
                if (avail.TryGetValue(item, out var a) && a.normal)
                    AlreadyDropped++;
                else
                    RegisterNPC(npc, When(new Conditions.NotExpert(), If(NormalDrop(item, drop))), what + " (mode normal)");

                string expertWhat = Describe(item, drop.MinExpert ?? drop.Min, drop.MaxExpert ?? drop.Max, drop.ChanceExpert ?? drop.Chance) + when;
                // Premier kill, biome : à vérifier sur le boss lui-même, pas à l'ouverture du sac
                if (expertDirect || condition is { WorksInBag: false })
                {
                    if (!(avail.TryGetValue(item, out a) && a.expert))
                        RegisterNPC(npc, When(new Conditions.IsExpert(), If(ExpertDrop(item, drop))), expertWhat + " (Expert, direct)");
                }
                else if (!bagAvail.ContainsKey(item))
                {
                    RegisterBag(bag, If(ExpertDrop(item, drop)), expertWhat);
                    bagAvail[item] = (true, true);
                }
            }

            // « One of the following N items will always drop » : un parmi le groupe, sauf si le jeu en donne déjà un. Si le
            // jeu n'en donne qu'une partie (Calamity a remplacé des armes du Desert Scourge), les objets absents sont
            // ajoutés un par un avec leur chance de la page.
            foreach (var entries in groups.Values)
            {
                var missing = entries.Where(e => !avail.ContainsKey(e.item)).ToList();
                if (missing.Count > 0 && missing.Count < entries.Count)
                {
                    AlreadyDropped += entries.Count - missing.Count;
                    foreach (var (item, drop) in missing)
                        AddOne(item, drop, null);
                    continue;
                }
                int[] items = entries.Select(e => e.item).ToArray();
                IItemDropRule OneOf() => items.Length == 1 ? ItemDropRule.Common(items[0]) : ItemDropRule.OneFromOptionsNotScalingWithLuck(1, items);
                string what = "un parmi : " + string.Join(", ", items.Select(Lang.GetItemNameValue));
                bool InNormal() => items.Any(i => avail.TryGetValue(i, out var a) && a.normal);
                bool InExpert() => items.Any(i => avail.TryGetValue(i, out var a) && a.expert);

                if (!boss)
                {
                    if (items.Any(avail.ContainsKey))
                        AlreadyDropped += items.Length;
                    else
                        RegisterNPC(npc, OneOf(), what);
                    continue;
                }
                if (InNormal())
                    AlreadyDropped += items.Length;
                else
                    RegisterNPC(npc, When(new Conditions.NotExpert(), OneOf()), what + " (mode normal)");
                if (expertDirect)
                {
                    if (!InExpert())
                        RegisterNPC(npc, When(new Conditions.IsExpert(), OneOf()), what + " (Expert, direct)");
                }
                else if (!items.Any(bagAvail.ContainsKey))
                {
                    RegisterBag(bag, OneOf(), what);
                    foreach (int i in items)
                        bagAvail[i] = (true, true);
                }
            }
        }

        /// <summary>Retire les règles du PNJ qui ne donnent que cet objet ; renvoie leur nombre.</summary>
        private static int RemoveSingleItemRules(int npc, int item)
        {
            int removed = 0;
            foreach (var rule in Main.ItemDropsDB.GetRulesForNPCID(npc, false).ToList())
            {
                var rates = Rates(new[] { rule });
                // Nos propres règles restent (un objet cité deux fois, sur la page du PNJ et sur la sienne)
                if (rates.Count > 0 && rates.All(r => r.itemId == item) && !Added.Any(a => a.npc == npc && a.rule == rule))
                {
                    Main.ItemDropsDB.RemoveFromNPC(npc, rule);
                    removed++;
                }
            }
            return removed;
        }

        /// <summary>
        /// Full Fandom : retire les règles de butin du jeu dont aucun objet n'est sur la page (conditions comprises).
        /// Rien n'est retiré si aucun objet de la page n'est reconnu, pour ne pas vider un PNJ mal résolu.
        /// Les sacs au trésor restent : la page « Treasure Bag » du wiki documente celui de chaque boss (les sacs des
        /// boss récents sont masqués par FullFandomHiding).
        /// </summary>
        /// <summary>
        /// Règles ajoutées par ce mod d'après une autre page que celle du PNJ (butin des boss mécaniques du Draedon's
        /// Remote...) : jamais retirées en Full Fandom.
        /// </summary>
        public static readonly HashSet<IItemDropRule> Keep = new();

        private static void RemoveNotOnWiki(int npc, NPCData npcData)
        {
            var onWiki = npcData.Drops.Select(d => WikiRecipes.ResolveItem(data.ItemAliases.GetValueOrDefault(d.Item, d.Item)))
                .Concat(npcData.Drops.Select(d => WikiRecipes.ResolveItem(d.Item))).Where(t => t > 0).ToHashSet();
            if (onWiki.Count == 0)
                return;
            foreach (var rule in Main.ItemDropsDB.GetRulesForNPCID(npc, false).ToList())
            {
                var rates = Rates(new[] { rule });
                if (rates.Count > 0 && !Keep.Contains(rule) && !rates.Any(r => onWiki.Contains(r.itemId) || ItemID.Sets.BossBag[r.itemId]))
                {
                    Main.ItemDropsDB.RemoveFromNPC(npc, rule);
                    RemovedNotOnWiki++;
                }
            }
        }

        // ------------------------------------------------------------------ sacs (page « Treasure Bag »)

        /// <summary>Sac d'une section de la page : par son nom, sinon celui que lâche le boss de la section.</summary>
        private static int ResolveBag(string boss, BagData bagData)
        {
            int bag = WikiRecipes.ResolveItem(bagData.Bag);
            // « Treasure Bag (Astrum Deus) » : boss renommé par le glossaire (alias de gen_loot.py)
            if (bag <= 0 && data.ItemAliases.TryGetValue(bagData.Bag, out string alias))
                bag = WikiRecipes.ResolveItem(alias);
            if (bag > 0 && ItemID.Sets.BossBag[bag])
                return bag;
            data.Aliases.TryGetValue(boss, out var aliases);
            foreach (int npc in WikiNames.FindNPCs(boss, aliases))
                if (BossBag(npc) is int found && found > 0)
                    return found;
            return -1;
        }

        /// <summary>
        /// Contenu des sacs listé par la page : ajouté s'il manque. En Full Fandom, le reste du sac est retiré (même
        /// règle que le butin des PNJ : rien n'est retiré si aucun objet de la section n'est reconnu).
        /// </summary>
        private static void ApplyBags()
        {
            foreach (var (boss, bagData) in data.Bags)
            {
                int bag = ResolveBag(boss, bagData);
                if (bag <= 0)
                {
                    MissingItems.TryGetValue(bagData.Bag, out int n);
                    MissingItems[bagData.Bag] = n + 1;
                    continue;
                }
                var drops = bagData.Drops.Where(d => FandomMode.Applies(d.Modes)).Select(d => (item: Resolve(d), drop: d)).Where(e => e.item > 0).ToList();
                if (FandomMode.IsFull && drops.Count > 0)
                {
                    var onPage = drops.Select(e => e.item).ToHashSet();
                    foreach (var rule in Main.ItemDropsDB.GetRulesForItemID(bag).ToList())
                    {
                        var rates = Rates(new[] { rule });
                        if (rates.Count > 0 && !rates.Any(r => onPage.Contains(r.itemId)))
                        {
                            Main.ItemDropsDB.RemoveFromItem(bag, rule);
                            RemovedFromBags++;
                        }
                    }
                }
                var avail = Availability(Main.ItemDropsDB.GetRulesForItemID(bag));
                foreach (var (item, drop) in drops)
                {
                    if (avail.ContainsKey(item))
                    {
                        AlreadyDropped++;
                        continue;
                    }
                    RegisterBag(bag, NormalDrop(item, drop), Describe(item, drop.Min, drop.Max, drop.Chance) + " (page Treasure Bag)");
                    avail[item] = (true, true);
                }
            }
        }

        /// <summary>
        /// Le bestiaire copie le butin une seule fois, avant les recettes (ModContent.SetupBestiary) : nos ajouts et
        /// retraits n'y apparaîtraient pas. On recopie le butin de chaque PNJ une fois tout appliqué, comme
        /// BestiaryDatabase.ExtractDropsForNPC. Appelé à la fin de notre PostAddRecipes : tModLoader appelle les systèmes
        /// dans l'ordre alphabétique de leur nom complet, donc après FullFandomHiding (Common.Modes) et avant tout
        /// PostSetupRecipes (dont l'auto-test).
        /// </summary>
        private static void RefreshBestiary()
        {
            for (int npc = -65; npc < NPCLoader.NPCCount; npc++)
            {
                var entry = Main.BestiaryDB.FindEntryByNPCID(npc);
                // Sans entrée propre (variantes cachées), FindEntryByNPCID rend une entrée « poubelle » partagée
                if (entry == null || !entry.Info.OfType<NPCNetIdBestiaryInfoElement>().Any(e => e.NetId == npc))
                    continue;
                entry.Info.RemoveAll(e => e is ItemDropBestiaryInfoElement);
                var rates = Rates(Main.ItemDropsDB.GetRulesForNPCID(npc, false));
                foreach (var rate in rates)
                    entry.Info.Add(new ItemDropBestiaryInfoElement(rate));
                BestiaryLines[npc] = rates.Count;
            }
        }

        public static void WriteReport(string folder)
        {
            var sb = new StringBuilder();
            sb.AppendLine("# Butin du wiki appliqué en jeu").AppendLine();
            sb.AppendLine("_Généré par l'auto-test à partir de `WikiData/loot.json`._").AppendLine();
            sb.AppendLine($"- {Added.Count} drops ajoutés sur {Added.Select(a => a.npc).Distinct().Count()} PNJ");
            sb.AppendLine($"- {AddedToBags.Count} ajouts dans {AddedToBags.Select(a => a.bag).Distinct().Count()} sacs");
            sb.AppendLine($"- {AlreadyDropped} drops du wiki déjà présents dans le jeu (inchangés)");
            sb.AppendLine($"- {Conditioned} drops avec leur condition (boss vaincu, biome, premier kill), dont {ReplacedByCondition} qui remplacent la règle du jeu");
            sb.AppendLine($"- {Conditional} drops à condition écrite en note que le jeu ne sait pas vérifier, non appliqués (voir `reports/loot.md`)");
            sb.AppendLine($"- {MissingNPCs.Count} PNJ du wiki absents du jeu, {MissingItems.Count} objets introuvables").AppendLine();
            sb.AppendLine("## Ajouts").AppendLine();
            foreach (var group in Added.GroupBy(a => a.npc).OrderBy(g => Lang.GetNPCNameValue(g.Key)))
                sb.AppendLine($"- **{Lang.GetNPCNameValue(group.Key)}** : {string.Join(" ; ", group.Select(a => a.what))}");
            sb.AppendLine().AppendLine("## Ajouts dans les sacs").AppendLine();
            foreach (var group in AddedToBags.GroupBy(a => a.bag).OrderBy(g => Lang.GetItemNameValue(g.Key)))
                sb.AppendLine($"- **{Lang.GetItemNameValue(group.Key)}** : {string.Join(" ; ", group.Select(a => a.what))}");
            sb.AppendLine().AppendLine($"## Sources génériques ({GenericAdded.Count} règles globales, pages d'objets)").AppendLine();
            foreach (string line in GenericAdded)
                sb.AppendLine($"- {line}");
            sb.AppendLine().AppendLine("## Objets introuvables (contenu à créer)").AppendLine();
            foreach (var (item, count) in MissingItems.OrderByDescending(kv => kv.Value))
                sb.AppendLine($"- {item} — {count}");
            File.WriteAllText(Path.Combine(folder, "loot_applied.md"), sb.ToString());
        }
    }
}
