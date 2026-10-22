using System;
using System.Collections.Generic;
using System.Linq;
using CalamityFandom.Common.Config;
using CalamityFandom.Common.Elements;
using CalamityFandom.Common.Modes;
using CalamityFandom.Common.WikiData;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityFandom.Common.Debug
{
    /// <summary>
    /// Auto-test lancé par tools/selftest.py (serveur dédié, variable CALFANDOM_SELFTEST=1) : vérifie après le
    /// chargement complet (recettes comprises) que les fondations s'appliquent, écrit « [SELFTEST] ... » dans la console puis quitte.
    /// Sans la variable, ne fait rien.
    /// </summary>
    public class SelfTest : ModSystem
    {
        private static bool Enabled => Environment.GetEnvironmentVariable("CALFANDOM_SELFTEST") == "1";

        private readonly List<string> failures = new();
        private int passed;

        // Échantillon de renommages attendus (tools/glossary.json) : (type de contenu, nom interne, nom attendu)
        private static readonly (string kind, string name, string expected)[] NameChecks =
        {
            ("item", "Murasama", "Graydron"),
            ("item", "UelibloomBar", "Tarragon Bar"),
            ("item", "UelibloomBrick", "Uelibloom Brick"),
            ("item", "RogueEmblem", "Spy Emblem"),
            ("item", "PearlShard", "Conquest Fragment"),
            ("buff", "Nightwither", "Moon Burnt"),
            ("npc", "AstrumDeusHead", "Astral Deus"),
            ("npc", "DraconicSwarmer", "Tempest Toddler"),
            ("npc", "RavagerBody", "Meatmetal Sans"),
            ("item", "TheEtomer", "Triboluminescent Etomer"),
            ("item", "MartianDistressRemote", "Martian Distress Beacon"),
        };

        public override void PostSetupRecipes()
        {
            if (!Enabled)
                return;

            Report($"mode : {FandomMode.Current}");
            CheckNames();
            CheckModes();
            CheckItemStats();
            CheckNPCStats();
            CheckRecipes();
            CheckElements();
            CheckMaterials();
            CheckLoot();
            CheckLegacy();
            CheckDebugItems();
            CheckRecipeConditions();
            foreach (var check in ExtraChecks)
                check(this);

            Report($"{passed} OK, {failures.Count} échec(s)");
            foreach (string f in failures)
                Report("ÉCHEC " + f);
            Report(failures.Count == 0 ? "RESULT PASS" : "RESULT FAIL");
            Environment.Exit(failures.Count == 0 ? 0 : 1);
        }

        /// <summary>Vérifications ajoutées par d'autres systèmes (recettes, éléments...).</summary>
        public static readonly List<Action<SelfTest>> ExtraChecks = new();

        /// <summary>Les rapports de reports/ décrivent le mode par défaut (Calamity++) : rien n'est écrit dans les autres modes.</summary>
        private static string ReportsFolder => FandomMode.Current == FandomAccuracy.CalamityPlus
            ? Environment.GetEnvironmentVariable("CALFANDOM_REPORTS") : null;

        private void CheckModes()
        {
            bool full = FandomMode.IsFull;
            // Écart extrême validé par l'utilisateur : dans tous les modes
            WikiStats.NPCs.TryGetValue("DesertScourgeHead", out var scourge);
            Expect(scourge?.LifeRatio == 87.5f, $"vie du Desert Scourge (x87,5 dans tous les modes) : {scourge?.LifeRatio}");
            // Valeur qui n'est que celle de Calamity 2.0.1.005 : Full Fandom seulement
            WikiStats.Items.TryGetValue("Starfleet", out var starfleet);
            Expect((starfleet?.Damage == 68) == full, $"Starfleet à 68 dégâts (valeur 2022, Full seulement) : {starfleet?.Damage}");
            // Éléments : tous les effets à chaque coup en Full (et Minimal, sans équilibrage), chance en Calamity++
            float coldChance = ElementData.ProcChance[(int)Element.Cold];
            Expect(FandomMode.IsPlus ? coldChance == 0.15f : coldChance == 1f, $"chance de l'effet Cold : {coldChance}");
            // Ravager = Meatmetal Sans : 23 coups pour le tuer, dans tous les modes
            if (ModContent.TryFind<ModNPC>("CalamityMod", "RavagerBody", out var ravager))
                Expect(ContentSamples.NpcsByNetId[ravager.Type].lifeMax == Common.NPCs.MeatmetalSans.HitsToKill,
                    $"vie de Meatmetal Sans : {ContentSamples.NpcsByNetId[ravager.Type].lifeMax}");
            Expect((FullFandomHiding.HiddenItems.Count > 400) == full, $"objets masqués : {FullFandomHiding.HiddenItems.Count}");
            // Contenu ancien : masqué s'il n'est cité nulle part (The Old Reaper), gardé s'il est cité en texte (Acidwood Wall)
            if (ModContent.TryFind<ModItem>("CalamityMod", "TheOldReaper", out var reaper))
                Expect(FullFandomHiding.HiddenItems.Contains(reaper.Type) == full, "The Old Reaper masqué en Full Fandom seulement");
            if (ModContent.TryFind<ModItem>("CalamityMod", "AcidwoodWallItem", out var acidWall))
                Expect(!FullFandomHiding.HiddenItems.Contains(acidWall.Type), "Acidwood Wall jamais masqué (cité dans un changelog)");
            if (full)
            {
                var leaks = Main.recipe.Take(Recipe.numRecipes)
                    .Where(r => !r.Disabled && FullFandomHiding.HiddenItems.Contains(r.createItem.type))
                    .Select(r => Lang.GetItemNameValue(r.createItem.type)).Distinct().ToList();
                Expect(leaks.Count == 0, $"recettes actives d'objets masqués : {string.Join(", ", leaks)}");
                Expect(WikiRecipes.DisabledNotOnWiki > 0 && WikiRecipes.Kept == 0, $"recettes hors wiki désactivées : {WikiRecipes.DisabledNotOnWiki}");
                Report($"full fandom : {FullFandomHiding.HiddenItems.Count} objets et {FullFandomHiding.HiddenNPCs.Count} PNJ masqués, "
                    + $"{FullFandomHiding.DisabledRecipes} recettes et {FullFandomHiding.RemovedDrops} règles de butin retirées ; "
                    + $"{WikiRecipes.DisabledNotOnWiki} recettes hors wiki désactivées, {WikiLoot.RemovedNotOnWiki} drops hors wiki retirés");
            }
            else
                Expect(WikiRecipes.DisabledNotOnWiki == 0 && WikiLoot.RemovedNotOnWiki == 0, "rien n'est retiré hors du mode Full");
        }

        public void Expect(bool condition, string description)
        {
            if (condition)
                passed++;
            else
                failures.Add(description);
        }

        private static void Report(string line)
        {
            Console.WriteLine("[SELFTEST] " + line);
            ModContent.GetInstance<CalamityFandom>().Logger.Info("[SELFTEST] " + line);
        }

        private void CheckNames()
        {
            foreach (var (kind, name, expected) in NameChecks)
            {
                string actual = kind switch
                {
                    "item" => ModContent.TryFind<ModItem>("CalamityMod", name, out var i) ? Lang.GetItemNameValue(i.Type) : null,
                    "buff" => ModContent.TryFind<ModBuff>("CalamityMod", name, out var b) ? Lang.GetBuffName(b.Type) : null,
                    "npc" => ModContent.TryFind<ModNPC>("CalamityMod", name, out var n) ? Lang.GetNPCNameValue(n.Type) : null,
                    _ => null,
                };
                Expect(actual == expected, $"nom {kind} {name} : attendu « {expected} », obtenu « {actual} »");
            }
            string rogue = Language.GetTextValue("Mods.CalamityMod.DamageClasses.RogueDamageClass.DisplayName");
            Expect(rogue == "spy damage", $"classe rogue : « {rogue} »");
        }

        private void CheckItemStats()
        {
            foreach (var (name, stats) in WikiStats.Items)
            {
                if (!ModContent.TryFind<ModItem>("CalamityMod", name, out var modItem))
                {
                    failures.Add($"objet {name} introuvable dans Calamity");
                    continue;
                }
                Item item = ContentSamples.ItemsByType[modItem.Type];
                string where = $"objet {name}";
                if (stats.Damage is int d) Expect(item.damage == d, $"{where} damage {item.damage} != {d}");
                if (stats.UseAnimation is int u) Expect(item.useAnimation == u, $"{where} useAnimation {item.useAnimation} != {u}");
                if (stats.KnockBack is float k) Expect(Math.Abs(item.knockBack - k) < 0.001f, $"{where} knockBack {item.knockBack} != {k}");
                if (stats.Crit is int c) Expect(item.crit == c, $"{where} crit {item.crit} != {c}");
                if (stats.ShootSpeed is float s) Expect(Math.Abs(item.shootSpeed - s) < 0.001f, $"{where} shootSpeed {item.shootSpeed} != {s}");
                if (stats.Mana is int m) Expect(item.mana == m, $"{where} mana {item.mana} != {m}");
                if (stats.Defense is int def) Expect(item.defense == def, $"{where} defense {item.defense} != {def}");
                if (stats.Rare is int r) Expect(item.rare == WikiStats.ResolveRarity(r), $"{where} rare {item.rare} != {r}");
                if (stats.Value is int v) Expect(item.value == v, $"{where} value {item.value} != {v}");
            }
        }

        private void CheckRecipes()
        {
            foreach (var (result, wanted) in WikiRecipes.Applied)
            {
                // Chaque recette du wiki doit exister telle quelle (les recettes absentes du wiki sont conservées)
                var actual = Main.recipe.Take(Recipe.numRecipes)
                    .Where(r => r.createItem.type == result && !r.Disabled)
                    .Select(WikiRecipes.Signature).ToHashSet();
                foreach (var w in wanted)
                    Expect(actual.Contains(WikiRecipes.Signature(w)), $"recette de {Lang.GetItemNameValue(result)} absente ({w.Page})");
            }
            string reports = ReportsFolder;
            if (!string.IsNullOrEmpty(reports))
                WikiRecipes.WriteReport(reports);
            Report($"recettes : {WikiRecipes.Applied.Count} objets changés, {WikiRecipes.Edited} modifiées, {WikiRecipes.Created} créées, "
                + $"{WikiRecipes.Kept} conservées hors wiki, {WikiRecipes.Unchanged} identiques, {WikiRecipes.ShimmerSet} shimmer, {WikiRecipes.Missing.Count} noms introuvables");
        }

        private void CheckElements()
        {
            Element[] ElementsOf(string mod, string name) =>
                ModContent.TryFind<ModItem>(mod, name, out var i) && ElementData.ItemElements.TryGetValue(i.Type, out var e) ? e : Array.Empty<Element>();

            var graydron = ElementsOf("CalamityMod", "Murasama");
            Expect(graydron.SequenceEqual(new[] { Element.Slash, Element.Machine }), $"éléments de Graydron : {string.Join(",", graydron)}");
            var zwei = ElementsOf("CalamityFandom", "Zweihandromeda");
            Expect(zwei.SequenceEqual(new[] { Element.Dark, Element.Magic, Element.Stab }), $"éléments de Zweihandromeda : {string.Join(",", zwei)}");

            for (int e = 0; e < ElementInfo.Count; e++)
            {
                bool hasStatus = ElementInfo.Effects[e] != null;
                Expect((ElementData.StatusBuffs[e] >= 0) == hasStatus, $"statut de l'élément {(Element)e} : buff {ElementData.StatusBuffs[e]}");
            }

            // Aero Slime : résistant Poison/Holy, vulnérable Fire/Dark
            if (ModContent.TryFind<ModNPC>("CalamityMod", "AeroSlime", out var slime))
            {
                Expect(ElementData.Multiplier(new[] { Element.Fire }, slime.Type) == 2f, "Aero Slime vulnérable au feu");
                Expect(ElementData.Multiplier(new[] { Element.Poison }, slime.Type) == 0.5f, "Aero Slime résistant au poison");
                Expect(ElementData.Multiplier(new[] { Element.Fire, Element.Poison }, slime.Type) == 1f, "Aero Slime feu + poison = neutre");
                Expect(NPCID.Sets.SpecificDebuffImmunity[slime.Type][ModContent.BuffType<Envenomed>()] == true, "Aero Slime immunisé contre Envenomed");
                Expect(NPCID.Sets.SpecificDebuffImmunity[slime.Type][ModContent.BuffType<Ablaze>()] != true, "Aero Slime sensible à Ablaze");
            }
            else
                Expect(false, "Aero Slime introuvable");

            Expect(ElementData.ItemElements.Count > 600, $"objets avec éléments : {ElementData.ItemElements.Count}");
            Report($"éléments : {ElementData.ItemElements.Count} objets et {ElementData.NPCAffinity.Count} PNJ en jeu ; "
                + $"{ElementData.UnresolvedItems.Count} objets et {ElementData.UnresolvedNPCs.Count} PNJ du wiki pas encore dans le jeu");
            string reports = ReportsFolder;
            if (!string.IsNullOrEmpty(reports))
            {
                var sb = new System.Text.StringBuilder();
                sb.AppendLine("# Éléments du wiki sans objet/PNJ en jeu").AppendLine();
                sb.AppendLine("_Généré par l'auto-test._ Contenu pas encore créé, ou nom différent.").AppendLine();
                sb.AppendLine($"## PNJ ({ElementData.UnresolvedNPCs.Count})").AppendLine();
                foreach (string name in ElementData.UnresolvedNPCs.OrderBy(x => x))
                    sb.AppendLine($"- {name}");
                sb.AppendLine().AppendLine($"## Objets ({ElementData.UnresolvedItems.Count})").AppendLine();
                foreach (string name in ElementData.UnresolvedItems.OrderBy(x => x))
                    sb.AppendLine($"- {name}");
                System.IO.File.WriteAllText(System.IO.Path.Combine(reports, "elements_missing.md"), sb.ToString());
            }
        }

        private void CheckMaterials()
        {
            int count = Mod.GetContent<Content.Items.WikiMaterial>().Count();
            Expect(count > 200, $"matériaux du wiki créés : {count}");
            if (Mod.TryFind<ModItem>("AlloyBar", out var alloy))
            {
                Expect(Lang.GetItemNameValue(alloy.Type) == "Alloy Bar", $"nom de AlloyBar : {Lang.GetItemNameValue(alloy.Type)}");
                Expect(Main.recipe.Take(Recipe.numRecipes).Any(r => r.createItem.type == alloy.Type && !r.Disabled), "Alloy Bar a une recette");
                Expect(Main.recipe.Take(Recipe.numRecipes).Any(r => !r.Disabled && r.requiredItem.Any(i => i.type == alloy.Type)),
                    "Alloy Bar sert d'ingrédient");
                // Le serveur ne charge pas les textures : on vérifie le fichier dans le .tmod (PNG converti en .rawimg)
                Expect(Mod.FileExists($"Assets/WikiItems/{alloy.Name}.rawimg") || Mod.FileExists($"Assets/WikiItems/{alloy.Name}.png"),
                    "sprite de Alloy Bar");
            }
            else
                Expect(false, "AlloyBar introuvable");
            // Yharim Bar géant au sol (image « The item in-game » du wiki), ramassage compris : hitbox x16
            if (Mod.TryFind<ModItem>("YharimBar", out var yharim))
                Expect(ContentSamples.ItemsByType[yharim.Type].width == 480, $"largeur de Yharim Bar au sol : {ContentSamples.ItemsByType[yharim.Type].width}");
            else
                Expect(false, "YharimBar introuvable");
            Report($"matériaux : {count} objets créés depuis le wiki");
        }

        private void CheckLegacy()
        {
            // Contenu porté depuis Calamity 2.0.1.005 : trouvable par son nom du wiki, intégré aux recettes du wiki
            int gaze = WikiRecipes.ResolveItem("Night's Gaze");
            Expect(gaze > 0 && ItemLoader.GetItem(gaze)?.Mod == Mod, "Night's Gaze porté et trouvable par son nom");
            int zwei = ModContent.ItemType<Content.Items.Weapons.Melee.Zweihandromeda>();
            Expect(Main.recipe.Take(Recipe.numRecipes).Any(r => !r.Disabled && r.createItem.type == zwei && r.requiredItem.Any(i => i.type == gaze)),
                "la recette de Zweihandromeda demande Night's Gaze");
            Expect(Main.recipe.Take(Recipe.numRecipes).Any(r => !r.Disabled && r.createItem.type == gaze), "Night's Gaze a une recette");
            // Sprites du wiki (tools/gen_sprites.py) : présents dans le .tmod (le serveur ne les applique pas)
            Expect(Mod.FileExists("Assets/Overrides/Generated/Items/PearlShard.rawimg"), "sprite du wiki de Conquest Fragment (tous les modes)");
            Expect(Mod.GetFileNames().Count(f => f.StartsWith("Assets/Overrides/Generated/Full/Items/")) > 200, "sprites du wiki réservés au Full Fandom");
            // Sandy Angling Kit : porté, lâché par le Desert Scourge (butin du wiki), se vide d'objets de pêche
            int kit = WikiRecipes.ResolveItem("Sandy Angling Kit");
            Expect(kit > 0 && ItemLoader.GetItem(kit)?.Mod == Mod, "Sandy Angling Kit porté et trouvable par son nom");
            Expect(WikiLoot.Added.Any(a => a.what.StartsWith("Sandy Angling Kit")), "le Desert Scourge lâche le Sandy Angling Kit");
            Expect(kit > 0 && Main.ItemDropsDB.GetRulesForItemID(kit).Count > 0, "le Sandy Angling Kit a un contenu");
            // Abyss Bar : « note=(Post-Golem) » invisible sur le wiki, donc sans condition (décision du 22/10)
            Expect(WikiLoot.Added.Any(a => a.what.StartsWith(Lang.GetItemNameValue(WikiRecipes.ResolveItem("Abyss Bar")) + " 3-8")),
                "le Desert Scourge lâche des Abyss Bar sans condition");
        }

        private void CheckRecipeConditions()
        {
            // Recette de Calamity verrouillée (palier 1 de l'arsenal de Draedon) et modifiée par le wiki : la condition reste
            if (ModContent.TryFind<ModItem>("CalamityMod", "DecryptionComputer", out var computer))
            {
                var recipes = Main.recipe.Take(Recipe.numRecipes).Where(r => !r.Disabled && r.createItem.type == computer.Type).ToList();
                Expect(recipes.Count > 0 && recipes.All(r => r.Conditions.Count > 0),
                    $"Decryption Computer garde sa condition de palier : {string.Join(", ", recipes.Select(r => r.Conditions.Count))}");
            }
        }

        private void CheckDebugItems()
        {
            // Une épée de test par élément (élément unique) + Moon Burnt, et le mannequin
            foreach (Element e in Enum.GetValues<Element>())
            {
                bool ok = Mod.TryFind<ModItem>($"TestSword{e}", out var sword)
                    && ElementData.ItemElements.TryGetValue(sword.Type, out var els) && els.Length == 1 && els[0] == e;
                Expect(ok, $"épée de test {e}");
            }
            Expect(Mod.TryFind<ModItem>("TestSwordMoonBurnt", out _), "épée de test Moon Burnt");
            Expect(ContentSamples.NpcsByNetId[ModContent.NPCType<Content.Debug.FandomTestDummy>()].lifeMax == Content.Debug.FandomTestDummy.Life,
                "vie du mannequin de test");
        }

        private void CheckLoot()
        {
            foreach (var (npc, rule, what) in WikiLoot.Added)
                Expect(Main.ItemDropsDB.GetRulesForNPCID(npc, false).Contains(rule), $"butin de {Lang.GetNPCNameValue(npc)} absent : {what}");
            Expect(WikiLoot.Added.Count > 50, $"drops ajoutés : {WikiLoot.Added.Count}");
            // Bestiaire recopié après nos changements (Calamity ajoute ensuite un drop « God Seed » en PostSetupRecipes,
            // absent du bestiaire comme en jeu normal) : les PNJ modifiés qui ont leur entrée ont bien été recopiés
            foreach (int npc in WikiLoot.Added.Select(a => a.npc).Distinct())
            {
                var entry = Main.BestiaryDB.FindEntryByNPCID(npc);
                if (!entry.Info.OfType<Terraria.GameContent.Bestiary.NPCNetIdBestiaryInfoElement>().Any(e => e.NetId == npc))
                    continue;
                int shown = entry.Info.Count(e => e is Terraria.GameContent.Bestiary.ItemDropBestiaryInfoElement);
                Expect(WikiLoot.BestiaryLines.TryGetValue(npc, out int lines) && shown == lines,
                    $"bestiaire de {Lang.GetNPCNameValue(npc)} : {shown} lignes pour {lines} drops recopiés");
            }
            // Sac au trésor jamais retiré (documenté par la page Treasure Bag), même en Full Fandom
            if (ModContent.TryFind<ModNPC>("CalamityMod", "DesertScourgeHead", out var scourge) && ModContent.TryFind<ModItem>("CalamityMod", "DesertScourgeBag", out var bag))
            {
                var rates = new List<DropRateInfo>();
                foreach (var r in Main.ItemDropsDB.GetRulesForNPCID(scourge.Type, false))
                    r.ReportDroprates(rates, new DropRateInfoChainFeed(1f));
                Expect(rates.Any(r => r.itemId == bag.Type), "Desert Scourge lâche toujours son sac en Expert");
                // Expert (décision du 22/10) : Parched Scale dans le sac en Calamity++, directement en Full Fandom
                int scale = WikiRecipes.ResolveItem("Parched Scale");
                var bagRates = new List<DropRateInfo>();
                foreach (var r in Main.ItemDropsDB.GetRulesForItemID(bag.Type))
                    r.ReportDroprates(bagRates, new DropRateInfoChainFeed(1f));
                bool direct = rates.Any(r => r.itemId == scale && r.conditions?.Any(c => c is Conditions.IsExpert) == true);
                bool inBag = bagRates.Any(r => r.itemId == scale);
                Expect(FandomMode.IsFull ? direct : inBag, $"Parched Scale en Expert (direct : {direct}, dans le sac : {inBag})");
            }
            foreach (var (bagType, rule, what) in WikiLoot.AddedToBags)
                Expect(Main.ItemDropsDB.GetRulesForItemID(bagType).Contains(rule), $"contenu de {Lang.GetItemNameValue(bagType)} absent : {what}");
            // Segments de ver : même ratio de vie que la tête (sinon la tête s'aligne sur la vie du corps)
            foreach (string segment in new[] { "DesertScourgeBody", "DesertScourgeTail" })
                Expect(WikiStats.NPCs.TryGetValue(segment, out var st) && st.LifeRatio == 87.5f, $"ratio de vie de {segment} : {st?.LifeRatio}");
            string reports = ReportsFolder;
            if (!string.IsNullOrEmpty(reports))
                WikiLoot.WriteReport(reports);
            Report($"butin : {WikiLoot.Added.Count} drops ajoutés, {WikiLoot.AddedToBags.Count} dans les sacs ({WikiLoot.RemovedFromBags} retirés), {WikiLoot.AlreadyDropped} déjà présents, {WikiLoot.Conditional} conditionnels ignorés, "
                + $"{WikiLoot.MissingNPCs.Count} PNJ et {WikiLoot.MissingItems.Count} objets introuvables");
        }

        private void CheckNPCStats()
        {
            foreach (var (name, stats) in WikiStats.NPCs)
            {
                if (!ModContent.TryFind<ModNPC>("CalamityMod", name, out var modNPC))
                {
                    failures.Add($"PNJ {name} introuvable dans Calamity");
                    continue;
                }
                NPC npc = ContentSamples.NpcsByNetId[modNPC.Type];
                if (stats.Defense is int def)
                    Expect(npc.defense == def, $"PNJ {name} defense {npc.defense} != {def}");
                Expect(npc.lifeMax > 0, $"PNJ {name} lifeMax {npc.lifeMax}");
            }
        }
    }
}
