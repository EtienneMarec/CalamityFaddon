using System;
using System.Collections.Generic;
using System.Linq;
using CalamityFandom.Common.Elements;
using CalamityFandom.Common.WikiData;
using Terraria;
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
        };

        public override void PostSetupRecipes()
        {
            if (!Enabled)
                return;

            CheckNames();
            CheckItemStats();
            CheckNPCStats();
            CheckRecipes();
            CheckElements();
            CheckMaterials();
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
            string reports = Environment.GetEnvironmentVariable("CALFANDOM_REPORTS");
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
            string reports = Environment.GetEnvironmentVariable("CALFANDOM_REPORTS");
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
            Report($"matériaux : {count} objets créés depuis le wiki");
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
