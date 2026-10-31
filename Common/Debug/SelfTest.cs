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
            ("item", "StarspawnHelixStaff", "Godspawn Helix Staff"),
            // Lore : nom de la page « Lore items », y compris quand Calamity l'a réécrit sous un autre nom
            ("item", "LoreArchmage", "Cryogen"),
            ("item", "LoreDesertScourge", "The Desert Scourge"),
            ("item", "LoreYharon", "Jungle Dragon, Yharon"),
            ("item", "LoreRavager", "Meatmetal Sans"),
            // Chaîne de renommages (Caustic Edge -> Tainted Blade, True Caustic Edge -> Caustic Edge), page de groupe
            // (Draedon's Logs), nom par défaut de tModLoader en 2022 (EutrophicChair -> « Eutrophic Chair »)
            ("item", "TrueCausticEdge", "True Caustic Edge"),
            ("item", "TaintedBlade", "Caustic Edge"),
            ("item", "DraedonsLogHell", "Draedon's Log - Hell"),
            ("item", "NavystoneChair", "Eutrophic Chair"),
            ("item", "SightseerSpitterBanner", "Big Sightseer Banner"),
            ("npc", "CrimulanPaladin", "Crimulan Slime God"),
            ("item", "SanguineTangerine", "Hemagrape"),
            ("item", "TaintedCloudberry", "Sambucus Fruit"),
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
            CheckShortstrike();
            CheckCrabulonTame();
            foreach (var check in ExtraChecks)
                check(this);

            Report($"{passed} OK, {failures.Count} échec(s)");
            foreach (string f in failures)
                Report("ÉCHEC " + f);
            Report(failures.Count == 0 ? "RESULT PASS" : "RESULT FAIL");
            Environment.Exit(failures.Count == 0 ? 0 : 1);
        }

        /// <summary>
        /// Taming de Crabulon joué pour de vrai (31/10) : joueur factice, Crabulon apparu, Yharic Delight mangé par le
        /// chemin du jeu (ItemLoader.OnConsumeItem). Compte dans les 5 s, plus après. Tout est remis en place ensuite.
        /// </summary>
        private void CheckCrabulonTame()
        {
            const string key = "Crabulon";
            int delight = WikiRecipes.ResolveItem("Yharic Delight");
            Expect(delight > 0 && ContentSamples.ItemsByType[delight].consumable, $"Yharic Delight existe et se mange ({delight})");
            if (delight <= 0)
                return;
            Player player = Main.player[0];
            bool wasActive = player.active;
            int oldMyPlayer = Main.myPlayer;
            player.active = true;
            player.dead = false;
            player.position = new Microsoft.Xna.Framework.Vector2(1600f, 1600f);
            int crab = NPC.NewNPC(NPC.GetBossSpawnSource(0), 1800, 1600, ModContent.NPCType<CalamityMod.NPCs.Crabulon.Crabulon>());
            var taming = ModContent.GetInstance<Common.Taming.TamingSystem>();
            bool wasUntamable = Common.Taming.TamingSystem.Untamable.Remove(key);
            try
            {
                taming.PostUpdateNPCs();
                var fight = Common.Taming.TamingSystem.Fights.GetValueOrDefault(key);
                Expect(fight != null && !fight.Harmed, "Taming de Crabulon : combat suivi, pas blessé");
                if (fight == null)
                    return;
                Main.myPlayer = 0;
                ItemLoader.OnConsumeItem(new Item(delight), player);
                Expect(fight.Progress >= 1f, $"Taming de Crabulon : Yharic Delight compté dans les 5 s (progrès {fight.Progress})");
                fight.Progress = 0f;
                fight.Started -= Common.Taming.TamingSystem.CrabulonWindow + 60;
                ItemLoader.OnConsumeItem(new Item(delight), player);
                Expect(fight.Progress == 0f, "Taming de Crabulon : Yharic Delight pas compté après 5 s");
            }
            finally
            {
                Main.myPlayer = oldMyPlayer;
                Main.npc[crab].active = false;
                Common.Taming.TamingSystem.Fights.Remove(key);
                if (wasUntamable)
                    Common.Taming.TamingSystem.Untamable.Add(key);
                player.active = wasActive;
            }
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
            // Lore absent de la page « Lore items » : masqué en Full Fandom ; lore cité sous « X (Lore) » : résolu
            if (ModContent.TryFind<ModItem>("CalamityMod", "LoreSignus", out var signusLore))
                Expect(FullFandomHiding.HiddenItems.Contains(signusLore.Type) == full, "lore de Signus masqué en Full Fandom seulement");
            if (ModContent.TryFind<ModItem>("CalamityMod", "LoreArchmage", out var cryogenLore))
            {
                Expect(!FullFandomHiding.HiddenItems.Contains(cryogenLore.Type), "lore de Cryogen jamais masqué");
                Expect(WikiRecipes.ResolveItem("Cryogen (Lore)") == cryogenLore.Type, "« Cryogen (Lore) » -> LoreArchmage");
            }
            if (ModContent.TryFind<ModItem>("CalamityMod", "LoreOldDuke", out var dukeLore))
                Expect(WikiRecipes.ResolveItem("Old Duke (Lore)") == dukeLore.Type, "« Old Duke (Lore) » -> LoreOldDuke");
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
            // Objets de Terraria (31/10) : recettes du wiki ajoutées ou remplaçantes dans tous les modes, anciennes
            // gardées en Calamity++ au cas par cas (Balance/recipes.json), recettes de base jamais retirées
            var recipes = Main.recipe.Take(Recipe.numRecipes).Where(r => !r.Disabled).ToList();
            bool Has(int result, int ingredient, int stack) =>
                recipes.Any(r => r.createItem.type == result && r.requiredItem.Any(i => i.type == ingredient && i.stack == stack));
            Expect(recipes.Any(r => r.createItem.type == ItemID.FeralClaws && r.requiredItem.Any(i => i.type == ItemID.Silk)),
                "Feral Claws : recette du wiki ajoutée (10 Silk)");
            Expect(recipes.Any(r => r.createItem.type == ItemID.Glass && r.requiredItem.Any(i => i.type == ItemID.SandBlock)),
                "Glass : la recette en sable reste (celle du wiki en Astral Clay s'ajoute)");
            Expect(Has(ItemID.AngelWings, ItemID.SoulofLight, 25) && !Has(ItemID.AngelWings, ItemID.SoulofLight, 15),
                "Angel Wings : 25 Soul of Light (wiki) au lieu de 15");
            Expect(Has(ItemID.CloudinaBottle, ItemID.Cloud, 30) == FandomMode.IsPlus && Has(ItemID.CloudinaBottle, ItemID.FallenStar, 50),
                "Cloud in a Bottle : recette du wiki ; ancienne gardée en Calamity++ seulement");
            Expect(WikiRecipes.VanillaApplied.Count >= 100 && (WikiRecipes.VanillaKeptOld.Count > 0) == FandomMode.IsPlus,
                $"objets de Terraria : {WikiRecipes.VanillaApplied.Count} recettes du wiki, {WikiRecipes.VanillaKeptOld.Count} anciennes gardées");
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
            // Metabolic Converter (31/10) : trouvable, dans le sac de Crabulon (page Treasure Bag) dans tous les modes
            int converter = WikiRecipes.ResolveItem("Metabolic Converter");
            Expect(converter > 0 && ItemLoader.GetItem(converter)?.Mod == Mod, "Metabolic Converter créé");
            if (ModContent.TryFind<ModItem>("CalamityMod", "CrabulonBag", out var crabBag))
            {
                var crabRates = new List<DropRateInfo>();
                foreach (var r in Main.ItemDropsDB.GetRulesForItemID(crabBag.Type))
                    r.ReportDroprates(crabRates, new DropRateInfoChainFeed(1f));
                Expect(crabRates.Any(r => r.itemId == converter), "le sac de Crabulon contient le Metabolic Converter");
            }
            // Taming (31/10) : 42 boss avec leur méthode, PNJ résolus, projectiles des méthodes trouvés
            var tames = Common.Taming.TamingSystem.Entries;
            Expect(Common.Taming.TamingSystem.CrabulonWindow == 300 && Common.Taming.TamingSystem.HungaryDelight == FandomMode.IsPlus,
                "Taming de Crabulon : 5 s ; Yharic Delight du Traveling Merchant en Calamity++ seulement");
            Expect(tames.Count == 42 && tames.All(t => t.Types.All(type => type > 0)), $"Taming : {tames.Count} boss apprivoisables");
            Expect(new[] { ItemID.SlimeGun, ItemID.GelBalloon, ItemID.GreenSolution }.All(i => Common.Taming.TamingMethods.Shoot(i) > 0),
                "Taming : projectiles du Slime Gun, du Sparkle Slime Balloon et de la Green Solution : "
                + string.Join(", ", new[] { ItemID.SlimeGun, ItemID.GelBalloon, ItemID.GreenSolution }.Select(i => Common.Taming.TamingMethods.Shoot(i))));
            Expect(Common.Taming.TamingSystem.EntryOf(NPCID.Spazmatism)?.Key == "The Twins", "Taming : Spazmatism fait partie des Twins");
            // King Slime : 5 min en Full Fandom (page), 3 min en Calamity++ (Balance/taming.json)
            float slimeGoal = Common.Taming.TamingSystem.Get("King Slime").Goal;
            Expect(slimeGoal == (FandomMode.IsPlus ? 3 : 5) * 60 * 60, $"Taming : objectif de King Slime ({slimeGoal / 3600f} min)");
            // Route sans tuer : Slime Gun fabriqué, Yharon Soul Fragment chez les ennemis du Solar Pillar
            Expect(Main.recipe.Take(Recipe.numRecipes).Any(r => r.createItem.type == ItemID.SlimeGun), "Taming : recette du Slime Gun");
            var solarRates = new List<DropRateInfo>();
            foreach (var r in Main.ItemDropsDB.GetRulesForNPCID(NPCID.SolarSolenian, false))
                r.ReportDroprates(solarRates, new DropRateInfoChainFeed(1f));
            Expect(solarRates.Any(r => r.itemId == ModContent.ItemType<CalamityMod.Items.Materials.YharonSoulFragment>()),
                "Taming : Yharon Soul Fragment lâché par les Selenians");
            Expect(ModContent.NPCType<Content.NPCs.Perforators.EstrangedSonHead>() > 0, "Taming : Estranged Son");
            // Portages du 31/10 : leurs recettes du wiki s'appliquent (Terra Edge, Lightning Hawk, Plague Hive)
            foreach (int ported in new[] { ModContent.ItemType<Content.Legacy.Items.Weapons.Melee.TerraEdge>(),
                ModContent.ItemType<Content.Legacy.Items.Weapons.Typeless.LightningHawk>(), ModContent.ItemType<Content.Legacy.Items.Accessories.PlagueHive>(),
                ModContent.ItemType<Content.Legacy.Items.Weapons.Melee.Devastation>(), ModContent.ItemType<Content.Items.Tools.AstralHammer>(),
                ModContent.ItemType<Content.Items.Weapons.Melee.FrostBlade>(), ModContent.ItemType<Content.Items.Weapons.Melee.ElementalClaymore>(),
                ModContent.ItemType<Content.Items.Weapons.Rogue.CatastropheClaycore>(), ModContent.ItemType<Content.Items.Weapons.Magic.ThePrincess>(),
                ModContent.ItemType<Content.Legacy.Items.Weapons.Rogue.AlphaVirus>() })
                Expect(Main.recipe.Take(Recipe.numRecipes).Any(r => r.createItem.type == ported && !r.Disabled),
                    $"recette du wiki de {Lang.GetItemNameValue(ported)}");
            // Ensemble « War » (31/10) : armes de l'armée gobeline, butin des pages d'objets
            var archerRates = new List<DropRateInfo>();
            foreach (var r in Main.ItemDropsDB.GetRulesForNPCID(NPCID.GoblinArcher, false))
                r.ReportDroprates(archerRates, new DropRateInfoChainFeed(1f));
            Expect(archerRates.Any(r => r.itemId == ModContent.ItemType<Content.Items.Weapons.Ranged.Warbow>())
                && archerRates.Any(r => r.itemId == ModContent.ItemType<Content.Items.Ammo.WarArrow>()), "le Goblin Archer lâche le Warbow et des War Arrows");
            int alphaVirus = ModContent.ItemType<Content.Legacy.Items.Weapons.Rogue.AlphaVirus>();
            Expect(Main.recipe.Take(Recipe.numRecipes).Any(r => r.createItem.type == ModContent.ItemType<CalamityMod.Items.Weapons.Rogue.Celestus>()
                && r.HasIngredient(alphaVirus)), "recette du wiki de Celestus (avec l'Alpha Virus)");
            // The Insacred Texts (31/10) : lâché par les Zombies (5 %) ; les infobulles des armes Rogue sont dans Items.Weapons.Rogue
            var zombieRates = new List<DropRateInfo>();
            foreach (var r in Main.ItemDropsDB.GetRulesForNPCID(NPCID.Zombie, false))
                r.ReportDroprates(zombieRates, new DropRateInfoChainFeed(1f));
            Expect(zombieRates.Any(r => r.itemId == ModContent.ItemType<Content.Items.Weapons.Magic.TheInsacredTexts>()), "le Zombie lâche The Insacred Texts");
            var emptyRogue = ModContent.GetContent<ModItem>().Where(i => i.Mod == Mod && i is CalamityMod.Items.Weapons.Rogue.RogueWeapon
                && string.IsNullOrWhiteSpace(i.Tooltip.Value)).Select(i => i.Name).ToList();
            Expect(emptyRogue.Count == 0, $"infobulles des armes Rogue ({string.Join(", ", emptyRogue)})");
            Expect(Language.Exists("Mods.CalamityFandom.InsacredTexts.Fic" + (Content.Items.Weapons.Magic.TheInsacredTexts.FicCount - 1)),
                "The Insacred Texts : phrases du clic droit");
            // Slime Puppet Staff de la page (31/10) : en Full Fandom, il pose des boules de slime
            int puppetStaff = ModContent.ItemType<CalamityMod.Items.Weapons.Summon.SlimePuppetStaff>();
            Expect((ContentSamples.ItemsByType[puppetStaff].shoot == ModContent.ProjectileType<Content.Projectiles.Summon.SlimePuppetBall>()) == FandomMode.IsFull,
                "Slime Puppet Staff : boules de slime en Full Fandom seulement");
            // Matériaux qui se posent (31/10) : bloc plein et barre 1 x 1
            int aeroplate = WikiRecipes.ResolveItem("Aeroplate"), flahium = WikiRecipes.ResolveItem("Flahium Bar");
            Expect(aeroplate > 0 && ContentSamples.ItemsByType[aeroplate].createTile > 0 && Main.tileSolid[ContentSamples.ItemsByType[aeroplate].createTile],
                "Aeroplate se pose (bloc plein)");
            Expect(flahium > 0 && ContentSamples.ItemsByType[flahium].createTile > 0 && Main.tileFrameImportant[ContentSamples.ItemsByType[flahium].createTile],
                "Flahium Bar se pose (barre)");
            // Sources génériques des pages d'objets (« any Jungle enemy », « Any Boss ») : règles globales (Cent Striker absent)
            Expect(WikiLoot.GenericAdded.Count >= 14 && WikiLoot.GenericAdded.Any(g => g.Contains("Any Boss")),
                $"butin : {WikiLoot.GenericAdded.Count} sources génériques (« Any Boss », « any Jungle enemy »...)");
            // Draedon's Remote (Astrum Aureus) : lâché par l'Armored Digger porté ; butin du mode Mayhem gardé en Full Fandom
            int remote = WikiRecipes.ResolveItem("Draedon's Remote");
            Expect(remote == ModContent.ItemType<Content.Legacy.Items.SummonItems.DraedonsRemote>(), "Draedon's Remote porté");
            var diggerRates = new List<DropRateInfo>();
            foreach (var r in Main.ItemDropsDB.GetRulesForNPCID(ModContent.NPCType<Content.Legacy.NPCs.NormalNPCs.ArmoredDiggerHead>(), false))
                r.ReportDroprates(diggerRates, new DropRateInfoChainFeed(1f));
            Expect(diggerRates.Any(r => r.itemId == remote), "l'Armored Digger lâche le Draedon's Remote");
            var primeRates = new List<DropRateInfo>();
            foreach (var r in Main.ItemDropsDB.GetRulesForNPCID(NPCID.SkeletronPrime, false))
                r.ReportDroprates(primeRates, new DropRateInfoChainFeed(1f));
            Expect(primeRates.Any(r => r.itemId == ModContent.ItemType<CalamityMod.Items.Materials.DubiousPlating>()),
                "Skeletron Prime du Draedon's Remote lâche des Dubious Plating");
            // Échecs (Brain of Cthulhu, XP-00 Hypnos) : chaque problème a bien sa solution d'après le moteur, et une seule
            var mate1Bad = Common.Taming.Chess.ChessPuzzles.Mate1.Count(p => Common.Taming.Chess.ChessPosition.FromFen(p.Fen).MatesInOne().Count != 1);
            var mate2Bad = Common.Taming.Chess.ChessPuzzles.Mate2.Count(p =>
            {
                var pos = Common.Taming.Chess.ChessPosition.FromFen(p.Fen);
                return pos.MatesInOne().Count > 0 || pos.MatesInTwo().Count != 1;
            });
            Expect(Common.Taming.Chess.ChessPuzzles.Mate1.Count >= 30 && Common.Taming.Chess.ChessPuzzles.Mate2.Count >= 30 && mate1Bad == 0 && mate2Bad == 0,
                $"Taming : problèmes d'échecs ({Common.Taming.Chess.ChessPuzzles.Mate1.Count} mats en un, {Common.Taming.Chess.ChessPuzzles.Mate2.Count} mats en deux, {mate1Bad + mate2Bad} faux)");
            Expect(Main.recipe.Take(Recipe.numRecipes).Any(r => r.createItem.type == ModContent.ItemType<Content.Items.Taming.HolographicChessboard>()),
                "Taming : recette du Holographic Chessboard");
            // Proficiency (31/10) : tableau de la page chargé, Level Meters vendues par le Goblin Tinkerer
            var levels = Common.Proficiency.ProficiencyData.Levels;
            Expect(levels.Length == 30 && levels[^1].Required == 50000 && Common.Proficiency.ProficiencyData.LevelOf(99) == 0
                && Common.Proficiency.ProficiencyData.LevelOf(100) == 1 && Common.Proficiency.ProficiencyData.LevelOf(500000) == 30,
                $"proficiency : {levels.Length} niveaux");
            if (NPCShopDatabase.TryGetNPCShop(NPCShopDatabase.GetShopName(NPCID.GoblinTinkerer), out var goblinShop))
                Expect(goblinShop.ActiveEntries.Count(e => e.Item.ModItem is Content.Legacy.Items.Accessories.LevelMeter) == 5, "le Goblin Tinkerer vend les 5 Level Meters");
            else
                Expect(false, "boutique du Goblin Tinkerer introuvable");
            // Ensembles d'armure élémentaires (31/10) : préfixe de classe qui désigne de vraies pièces (casque, plastron, jambières)
            var armorSets = ElementData.ArmorAffinity.Where(a => ItemLoader.ItemCount > 0 && Enumerable.Range(ItemID.Count, ItemLoader.ItemCount - ItemID.Count)
                .Select(ItemLoader.GetItem).Where(m => m != null && m.Name.StartsWith(a.prefix, StringComparison.Ordinal))
                .Select(m => m.Item).Count(i => i.headSlot > 0 || i.bodySlot > 0 || i.legSlot > 0) >= 3).Select(a => a.prefix).ToList();
            Expect(armorSets.Count >= 7, $"ensembles d'armure élémentaires trouvés en jeu : {string.Join(", ", armorSets)}");
            Report($"armures élémentaires : {armorSets.Count} ensembles en jeu sur {ElementData.ArmorAffinity.Count} ({string.Join(", ", armorSets)})");
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
            // Aliments du wiki (24/10) : Titan Finger = Plenty Satisfied 4 minutes, lâché par Atlas (butin des pages d'objets)
            if (Mod.TryFind<ModItem>("TitanFinger", out var finger))
            {
                var sample = ContentSamples.ItemsByType[finger.Type];
                Expect(sample.buffType == BuffID.WellFed2 && sample.buffTime == 240 * 60 && sample.consumable, $"Titan Finger : buff {sample.buffType}, {sample.buffTime} ticks");
                Expect(!WikiLoot.MissingItems.ContainsKey("Titan Finger"), "Titan Finger trouvé par le butin");
            }
            else
                Expect(false, "TitanFinger introuvable");
            // Ailes du wiki (24/10) : Red Wings = vitesse 15, accélération 3, 190 de temps de vol
            if (Mod.TryFind<ModItem>("RedWings", out var redWings))
            {
                int slot = ContentSamples.ItemsByType[redWings.Type].wingSlot;
                var stats = slot > 0 ? ArmorIDs.Wing.Sets.Stats[slot] : default;
                Expect(slot > 0 && stats.FlyTime == 190 && stats.AccRunSpeedOverride == 15f, $"Red Wings : emplacement {slot}, vol {stats.FlyTime}, vitesse {stats.AccRunSpeedOverride}");
            }
            else
                Expect(false, "RedWings introuvable");
            // Yharim Bar géant au sol (image « The item in-game » du wiki), ramassage compris : hitbox x16
            if (Mod.TryFind<ModItem>("YharimBar", out var yharim))
                Expect(ContentSamples.ItemsByType[yharim.Type].width == 480, $"largeur de Yharim Bar au sol : {ContentSamples.ItemsByType[yharim.Type].width}");
            else
                Expect(false, "YharimBar introuvable");
            Report($"matériaux : {count} objets créés depuis le wiki");
        }

        private void CheckLegacy()
        {
            // Contenu porté depuis Calamity 2.0.1.005 : trouvable par son nom du wiki, intégré aux recettes du wiki.
            // Portages d'objets que Calamity a seulement renommés ou refaits (décision du 23/10) : notre version en Full
            // Fandom, l'objet actuel de Calamity (sous le nom du wiki) dans les autres modes
            // (Starfall, Seabound Staff, Effluvium Bow : fusionnés le 28/10 par erreur, rétablis le 29/10)
            var replacedByCalamity = new HashSet<string> { "Night's Gaze", "Seabound Staff", "Effluvium Bow", "Regenator", "Storm Spray",
                "Solar Flare", "Corroslime Staff", "Shock Grenade", "Mana Jelly", "Amidias' Spark", "Energy Staff", "Starfall",
                "Marnite Spear", "Cursed Capper", "Luminous Striker", "The Reaper", "Warbanner of the Sun", "Rusty Medallion",
                "Alchemical Flask", "Red Bow", "Marnite Bayonet" };
            string ExpectedOwner(string name) => replacedByCalamity.Contains(name) && !FandomMode.IsFull ? "CalamityMod" : Mod.Name;
            int gaze = WikiRecipes.ResolveItem("Night's Gaze");
            Expect(gaze > 0 && ItemLoader.GetItem(gaze)?.Mod.Name == ExpectedOwner("Night's Gaze"),
                $"Night's Gaze : {ItemLoader.GetItem(gaze)?.FullName} ({ExpectedOwner("Night's Gaze")} attendu)");
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
            // Lot de portages du 22/10 : trouvables par leur nom du wiki, et lâchés là où le wiki le dit
            foreach (string name in new[] { "Bleached Angling Kit", "Seabound Staff", "Effluvium Bow", "Regenator", "Storm Spray", "Aquatic Discharge", "Solar Flare", "Diseased Pike", "Crimslime Staff", "Corroslime Staff", "Gehenna", "Shock Grenade", "Mana Jelly", "Fungal Carapace", "Amidias' Spark", "Energy Staff", "Starfall", "Storm Dragoon", "Soul Edge" })
            {
                int type = WikiRecipes.ResolveItem(name);
                Expect(type > 0 && ItemLoader.GetItem(type)?.Mod.Name == ExpectedOwner(name), $"{name} : {ItemLoader.GetItem(type)?.FullName} ({ExpectedOwner(name)} attendu)");
            }
            // Full Fandom : notre Seabound Staff (butin du wiki) dans la table du Desert Scourge
            if (FandomMode.IsFull && ModContent.TryFind<ModNPC>("CalamityMod", "DesertScourgeHead", out var desertScourge))
            {
                int seabound = WikiRecipes.ResolveItem("Seabound Staff");
                var scourgeRates = new List<DropRateInfo>();
                foreach (var r in Main.ItemDropsDB.GetRulesForNPCID(desertScourge.Type, false))
                    r.ReportDroprates(scourgeRates, new DropRateInfoChainFeed(1f));
                Expect(seabound > 0 && scourgeRates.Any(r => r.itemId == seabound), "le Desert Scourge lâche le Seabound Staff");
            }
            // Lot du 26/10 : objets qui débloquent des recettes ou du butin du wiki
            foreach (string name in new[] { "Psychotic Amulet", "Disseminator", "Wrath of the Ancients", "Astral Bow", "Lazhar", "Carnage Ray", "Phantom Lance", "Eutrophic Scimitar", "Aftershock", "Sandslasher", "Asteroid Staff", "Lead Core", "Surgical Glove", "Sulfuric Scale", "Flask of Plague", "Plague Balloon", "Microxodonta", "Sea Shell", "Mirage Jelly (item)", "Inverted Gar", "Glacial Crusher", "Trash of Magnus", "Blow Pipe", "The Needler", "Disease", "Reemergence", "Phlegm", "Toxic Wisdom", "Poison I-Bee", "Decomposition", "Metal Ball", "Sewer Brewer", "Terra-cotta", "Blood Clot Staff" })
            {
                int type = WikiRecipes.ResolveItem(name);
                Expect(type > 0 && ItemLoader.GetItem(type)?.Mod == Mod, $"{name} porté : {ItemLoader.GetItem(type)?.FullName}");
                Expect(!WikiRecipes.Missing.ContainsKey("objet : " + name) && !WikiLoot.MissingItems.ContainsKey(name), $"{name} n'est plus introuvable");
            }
            // Lot du 29/10 : épées courtes retirées avant 2.0.1.005 (tableau Shortstrike), portées en estoc avec leur Shortflurry
            foreach (string name in new[] { "Leeching Dagger", "Bloody Rupture", "Ancient Shiv", "Eutrophic Shank", "Tomatus", "Night's Stabber", "Caliburn", "True Caliburn", "True Night's Stabber", "Terra Shiv", "Wulfrum Leech Dagger" })
            {
                int type = WikiRecipes.ResolveItem(name);
                Expect(type > 0 && ItemLoader.GetItem(type)?.Mod == Mod, $"{name} porté : {ItemLoader.GetItem(type)?.FullName}");
                Expect(type > 0 && Common.Shortstrike.ShortstrikeData.ByItem.ContainsKey(type), $"{name} a son Shortflurry du tableau");
                Expect(type > 0 && Content.Shortstrike.ShortflurryRealClone.CanClone(ContentSamples.ItemsByType[type].shoot), $"{name} : clone de Shortflurry réel (estoc)");
            }
            // Lot du 29/10 (2) : objets simples retirés avant 2.0.1.005
            foreach (string name in new[] { "Blood Relic", "Wulfrum Axe", "Wulfrum Hammer", "Waraxe", "Warblade", "Corrupted Crusher Blade", "Crimson Crusher Blade", "Mandible Claws", "Mandible Bow", "Raider's Glory", "Bittercold Staff", "Shadowdrop Staff", "Hell Burst" })
            {
                int type = WikiRecipes.ResolveItem(name);
                Expect(type > 0 && ItemLoader.GetItem(type)?.Mod == Mod, $"{name} porté : {ItemLoader.GetItem(type)?.FullName}");
            }
            // Lot du 29/10 (3) : armes retirées en juin-juillet 2022, recettes du wiki ; Sylvan Slasher chez le Bandit
            foreach (string name in new[] { "Crystal Blade", "Temporal Floe Sword", "Marnite Spear", "Sylvan Slasher", "Animus", "Blade of Enmity", "Brimlash Buster", "Phoenix Blade", "Trinity", "Fulguration Halberd", "Mariana", "Aquatic Dissolution", "Impaler", "Spectre Rifle", "Star Cannon EX", "Hadopelagic Echo", "Crystal Flare Staff", "Marked Magnum", "Revivify Potion", "Dualpoon", "Triploon", "Exo Gladius", "Cursed Capper", "Luminous Striker", "The Reaper", "Warbanner of the Sun", "Rusty Medallion", "Alchemical Flask", "Red Bow", "Seabow", "Redtide Sword", "Seashell Boomerang", "Urchin Spear", "Urchin Flail", "Wulfrum Blade", "Wulfrum Bow", "Wulfrum Staff", "Marnite Bayonet", "Charged Dart Blaster", "Lazinator", "Acceleration Round", "Cadance Potion", "Crumbling Potion", "Shattering Potion", "Frigid Bulwark" })
            {
                int type = WikiRecipes.ResolveItem(name);
                Expect(type > 0 && ItemLoader.GetItem(type)?.Mod.Name == ExpectedOwner(name), $"{name} : {ItemLoader.GetItem(type)?.FullName} ({ExpectedOwner(name)} attendu)");
                if (!new[] { "Sylvan Slasher", "The Reaper", "Warbanner of the Sun", "Red Bow", "Urchin Flail" }.Contains(name))
                    Expect(Main.recipe.Take(Recipe.numRecipes).Any(r => !r.Disabled && r.createItem.type == type), $"{name} a une recette");
            }
            Expect(WikiLoot.Added.Any(a => a.what.StartsWith("Crystal Blade")), "le Crystal Crawler lâche la Crystal Blade");
            Expect(WikiLoot.Added.Any(a => a.what.StartsWith("Quasar")), "Astrum Deus lâche la Quasar");
            // 30/10 : Arcanum of the Void (sac du Ceaseless Void), Microcell Cut (créée), Exo Gladius (recette du wiki)
            if (ModContent.TryFind<ModItem>("CalamityMod", "CeaselessVoidBag", out var voidBag))
            {
                var bagRates = new List<DropRateInfo>();
                foreach (var r in Main.ItemDropsDB.GetRulesForItemID(voidBag.Type))
                    r.ReportDroprates(bagRates, new DropRateInfoChainFeed(1f));
                Expect(bagRates.Any(r => r.itemId == ModContent.ItemType<Content.Legacy.Items.Accessories.ArcanumoftheVoid>()), "le sac du Ceaseless Void contient l'Arcanum of the Void");
            }
            int microcell = WikiRecipes.ResolveItem("Microcell Cut");
            Expect(microcell > 0 && ItemLoader.GetItem(microcell)?.Mod == Mod, "Microcell Cut trouvable par son nom");
            int jam = WikiRecipes.ResolveItem("Purified Jam");
            Expect(jam > 0 && ItemLoader.GetItem(jam)?.Mod == Mod, "Purified Jam porté");
            Expect(WikiLoot.Added.Any(a => a.what.StartsWith("Purified Jam")), "The Slime God lâche la Purified Jam");
            Expect(Content.Legacy.Buffs.Potions.Invincible.Immunities.Length >= 37, $"Invincible : {Content.Legacy.Buffs.Potions.Invincible.Immunities.Length} immunités (38 attendues)");
            int flameblaster = WikiRecipes.ResolveItem("Brimstone Flameblaster");
            Expect(flameblaster > 0 && ItemLoader.GetItem(flameblaster)?.Mod == Mod, "Brimstone Flameblaster porté");
            Expect(WikiLoot.Added.Any(a => a.what.StartsWith("Brimstone Flameblaster")), "Cataclysm lâche le Brimstone Flameblaster");
            if (ModContent.TryFind<ModNPC>("CalamityMod", "Bandit", out var bandit) && NPCShopDatabase.TryGetNPCShop(NPCShopDatabase.GetShopName(bandit.Type), out var banditShop))
            {
                int sylvan = ModContent.ItemType<Content.Legacy.Items.Weapons.Rogue.SylvanSlasher>();
                Expect(banditShop.ActiveEntries.Any(e => e.Item.type == sylvan && e.Item.shopCustomPrice == Item.buyPrice(platinum: 5)), "le Bandit vend la Sylvan Slasher (5 platine)");
                Expect(banditShop.ActiveEntries.Any(e => e.Item.type == ModContent.ItemType<Content.Legacy.Items.Weapons.Rogue.BouncingBetty>()), "le Bandit vend la Bouncing Betty");
                Expect(banditShop.ActiveEntries.Any(e => e.Item.type == ModContent.ItemType<Content.Legacy.Items.Weapons.Rogue.LatcherMine>()), "le Bandit vend la Latcher Mine");
            }
            else
                Expect(false, "boutique du Bandit introuvable");
            // Chaque objet du mod a son nom dans la localisation (une classe de base de Calamity, RogueWeapon, change la
            // catégorie : Items.Weapons.Rogue)
            var unnamed = Mod.GetContent<ModItem>().Where(i => !Terraria.Localization.Language.Exists(i.DisplayName.Key)).Select(i => i.DisplayName.Key).ToList();
            Expect(unnamed.Count == 0, $"objets sans nom localisé : {string.Join(", ", unnamed)}");
            int amulet = WikiRecipes.ResolveItem("Psychotic Amulet");
            Expect(Main.recipe.Take(Recipe.numRecipes).Any(r => !r.Disabled && r.requiredItem.Any(i => i.type == amulet)
                && Lang.GetItemNameValue(r.createItem.type).StartsWith("Auric Tesla")), "les casques Auric Tesla demandent le Psychotic Amulet");
            Expect(WikiLoot.Added.Any(a => a.what.StartsWith("Psychotic Amulet")), "la Possessed Armor lâche le Psychotic Amulet");
            // Alternatives abrégées du wiki (« Silver/Tungsten Bar ») et groupes au pluriel (« Any Quivers »)
            Expect(!WikiRecipes.Missing.ContainsKey("objet : Silver") && !WikiRecipes.Missing.ContainsKey("objet : Any Quivers"),
                "« Silver/Tungsten Bar » et « Any Quivers » résolus");
            // Renommages de Calamity depuis 2022 : nom du wiki rendu, plus masqués en Full Fandom
            if (ModContent.TryFind<ModItem>("CalamityMod", "EssenceofHavoc", out var havoc))
            {
                Expect(Lang.GetItemNameValue(havoc.Type) == "Essence of Chaos", $"Essence of Havoc s'appelle {Lang.GetItemNameValue(havoc.Type)}");
                Expect(!Common.Modes.FullFandomHiding.HiddenItems.Contains(havoc.Type), "Essence of Chaos n'est pas masquée");
            }
            if (ModContent.TryFind<ModItem>("CalamityMod", "Vega", out var vega))
                Expect(Common.Modes.FullFandomHiding.HiddenItems.Contains(vega.Type) == FandomMode.IsFull, "Vega masquée en Full Fandom seulement (notre Night's Gaze la remplace)");
            Expect(Common.Modes.FullFandomHiding.HiddenItems.Contains(ModContent.ItemType<Content.Legacy.Items.Weapons.Rogue.NightsGaze>()) == !FandomMode.IsFull,
                "notre Night's Gaze masquée hors Full Fandom");
            Expect(WikiLoot.Added.Any(a => a.what.StartsWith("Bleached Angling Kit")), "l'Aquatic Scourge lâche le Bleached Angling Kit");
            int grenade = WikiRecipes.ResolveItem("Shock Grenade");
            Expect(Main.recipe.Take(Recipe.numRecipes).Any(r => !r.Disabled && r.createItem.type == grenade && r.createItem.stack == 100),
                "Shock Grenade : recette du wiki (100 grenades)");
            Expect(!WikiLoot.MissingItems.ContainsKey("Calamitas Mask"), "Calamitas Mask = Calamitas Clone Mask (alias du butin)");
            // Abyss Bar : « note=(Post-Golem) » invisible sur le wiki, donc sans condition (décision du 22/10)
            Expect(WikiLoot.Added.Any(a => a.what.StartsWith(Lang.GetItemNameValue(WikiRecipes.ResolveItem("Abyss Bar")) + " 3-8")),
                "le Desert Scourge lâche des Abyss Bar sans condition");
        }

        private void CheckShortstrike()
        {
            // Wiki : Shortstrike.txt — jauge (Balance/ en Calamity++) et Shortflurry des épées courtes du tableau
            var rules = Common.Shortstrike.ShortstrikeData.Current;
            Expect(rules.MaxPoints == 100 && rules.PointsPerDamagePercent == (FandomMode.IsPlus ? 10 : 5) && rules.PointsPerDefense == (FandomMode.IsPlus ? 6 : 3),
                $"règles du Shortstrike : {rules.PointsPerDamagePercent} points/% dégâts, {rules.PointsPerDefense} points/défense");
            var byItem = Common.Shortstrike.ShortstrikeData.ByItem;
            Expect(byItem.TryGetValue(ItemID.CopperShortsword, out var copper) && copper.Hits == 3 && copper.Effects.Any(e => e.Type == "defense" && e.Value == 5),
                "Shortflurry de la Copper Shortsword (+10 %, 3 coups, +5 défense)");
            int discharge = WikiRecipes.ResolveItem("Aquatic Discharge");
            Expect(byItem.TryGetValue(discharge, out var aquatic) && aquatic.Effects.Any(e => e.Type == "inflict" && e.BuffType == BuffID.Electrified),
                "Shortflurry de l'Aquatic Discharge : Electrified");
            int galileo = WikiRecipes.ResolveItem("Galileo Gladius");
            Expect(byItem.TryGetValue(galileo, out var gladius) && gladius.Effects.Any(e => e.Type == "immune" && e.BuffType > 0),
                "Shortflurry du Galileo Gladius : immunité à Nightwither (Moon Burnt) résolue");
            Expect(Common.Shortstrike.ShortstrikeData.IsShortsword(ContentSamples.ItemsByType[ItemID.CopperShortsword]), "la Copper Shortsword est une épée courte");
            // Effets écrits à la main (tools/shortstrike_extra.json → overrides) : projectiles et debuffs résolus
            // Galileo Gladius et Cosmic Shiv : effets propres à chaque mode (2.0.1.005 en Full Fandom, armes de Calamity sinon)
            foreach (var (weapon, type) in new[] { ("Galileo Gladius", FandomMode.IsFull ? "spawnOnStab" : "throwClone"), ("Cosmic Shiv", "spawnOnStab"),
                ("Submarine Shocker", "cloud"), ("Spore Knife", "vine"), ("Sahara Slicers", FandomMode.IsFull ? null : "saharaQuiver"),
                ("Caliburn", "verticalBeams"), ("True Caliburn", "flurryBeam"), ("True Night's Stabber", "flurryBeam"), ("Terra Shiv", "flurryBeam") })
            {
                if (type == null)
                    continue;
                int item = WikiRecipes.ResolveItem(weapon);
                var effect = byItem.TryGetValue(item, out var attack) ? attack.Effects.FirstOrDefault(e => e.Type == type) : null;
                Expect(effect != null && (effect.Projectile == null || effect.ProjectileType > 0) && (effect.Name == null || effect.BuffType > 0)
                    && (effect.Rain == null || effect.RainType > 0), $"Shortflurry de {weapon} : effet {type} résolu");
                if (effect?.Projectile != null)
                    Expect(effect.Projectile.StartsWith("CalamityFandom/") == FandomMode.IsFull || weapon is "Submarine Shocker" or "Spore Knife" or "Sahara Slicers" or "Caliburn" or "True Caliburn" or "True Night's Stabber" or "Terra Shiv"
                        || type == "throwClone", $"Shortflurry de {weapon} : projectile du bon mode ({effect.Projectile})");
            }
            // Full Fandom : Galileo Gladius et Cosmic Shiv tirent les projectiles portés de 2.0.1.005, la Spore Knife redevient une épée courte
            foreach (var (weapon, legacy) in new[] { ("Galileo Gladius", ModContent.ProjectileType<Content.Legacy.Projectiles.Melee.GalileoGladiusHeldProj>()),
                ("Cosmic Shiv", ModContent.ProjectileType<Content.Legacy.Projectiles.Melee.CosmicShivProj>()),
                ("Spore Knife", ModContent.ProjectileType<Content.Legacy.Projectiles.Melee.SporeKnifeShortsword>()) })
            {
                var sample = ContentSamples.ItemsByType[WikiRecipes.ResolveItem(weapon)];
                Expect((sample.shoot == legacy) == FandomMode.IsFull, $"{weapon} : estoc de 2.0.1.005 en Full Fandom seulement (shoot {sample.shoot})");
            }
            Expect(!Common.Shortstrike.ShortstrikeData.IsShortsword(ContentSamples.ItemsByType[ItemID.CopperBroadsword]), "la Copper Broadsword n'en est pas une");
            int sahara = WikiRecipes.ResolveItem("Sahara Slicers");
            if (sahara > 0)
                Expect(byItem.ContainsKey(sahara) == !FandomMode.IsFull, "Shortflurry des Sahara Slicers hors Full Fandom (Calamity++ et sa couche Minimal)");
            Report($"shortstrike : {byItem.Count} épées courtes du tableau en jeu, {Common.Shortstrike.ShortstrikeData.MissingWeapons.Count} absentes ("
                + string.Join(", ", Common.Shortstrike.ShortstrikeData.MissingWeapons) + "), noms non résolus : "
                + string.Join(" ; ", Common.Shortstrike.ShortstrikeData.MissingNames));
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
            // Conditions écrites en note (« After defeating Calamitas », « In Hardmode »...) : appliquées
            Expect(WikiLoot.Conditioned > 50, $"drops du wiki avec leur condition : {WikiLoot.Conditioned}");
            Expect(WikiLoot.Added.Any(a => a.what.Contains("[After defeating Calamitas]")), "Lumenyl après Calamitas (ennemis des abysses)");
            // Devil Fish : la condition du wiki remplace celle du jeu (Depth Cells après Calamitas, Abyss Ore après Golem)
            Expect(WikiLoot.Added.Any(a => a.what.StartsWith("Depth Cells") && a.what.Contains("[After defeating Calamitas]")),
                "Devil Fish : Depth Cells après Calamitas (et non après Leviathan)");
            Expect(WikiLoot.Added.Any(a => a.what.Contains("3-9") && a.what.Contains("[After defeating Golem]")),
                "Devil Fish : Abyss Ore 3-9 après Golem, condition affichée");
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
            Report($"butin : {WikiLoot.Added.Count} drops ajoutés, {WikiLoot.AddedToBags.Count} dans les sacs ({WikiLoot.RemovedFromBags} retirés), {WikiLoot.AlreadyDropped} déjà présents, {WikiLoot.Conditioned} avec condition, {WikiLoot.Conditional} conditionnels ignorés, "
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
