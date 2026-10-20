using System;
using System.Collections.Generic;
using CalamityFandom.Common.WikiData;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityFandom.Common.Debug
{
    /// <summary>
    /// Auto-test lancé par tools/selftest.py (serveur dédié, variable CALFANDOM_SELFTEST=1) : vérifie après le
    /// chargement complet que les fondations s'appliquent, écrit « [SELFTEST] ... » dans la console puis quitte.
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

        public override void PostAddRecipes()
        {
            if (!Enabled)
                return;

            CheckNames();
            CheckItemStats();
            CheckNPCStats();
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
