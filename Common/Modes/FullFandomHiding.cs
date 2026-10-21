using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using CalamityFandom.Common.Config;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace CalamityFandom.Common.Modes
{
    /// <summary>
    /// Mode Full Fandom : masque le contenu ajouté à Calamity après l'abandon du wiki (2.0.1.005) et qui n'a pas de page
    /// (WikiData/baseline.json, généré par tools/gen_baseline.py). Masquer = plus de recette, plus de butin, plus
    /// d'apparition naturelle, plus de vente. Les PNJ créés en combat ne sont pas touchés : un boss retravaillé peut
    /// dépendre d'un sbire récent.
    /// </summary>
    public class FullFandomHiding : ModSystem
    {
        private class BaselineFile
        {
            public HiddenData Hidden { get; set; } = new();
        }

        private class HiddenData
        {
            public List<string> Items { get; set; } = new();
            public List<string> Npcs { get; set; } = new();
        }

        public static HashSet<int> HiddenItems { get; } = new();
        public static HashSet<int> HiddenNPCs { get; } = new();
        public static int DisabledRecipes, RemovedDrops;

        public override void Unload()
        {
            HiddenItems.Clear();
            HiddenNPCs.Clear();
            DisabledRecipes = RemovedDrops = 0;
        }

        public override void PostSetupContent()
        {
            if (!FandomMode.IsFull)
                return;
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var file = JsonSerializer.Deserialize<BaselineFile>(Mod.GetFileBytes("WikiData/baseline.json"), options);
            foreach (string name in file.Hidden.Items)
                if (ModContent.TryFind<ModItem>("CalamityMod", name, out var item))
                    HiddenItems.Add(item.Type);
            foreach (string name in file.Hidden.Npcs)
                if (ModContent.TryFind<ModNPC>("CalamityMod", name, out var npc))
                    HiddenNPCs.Add(npc.Type);
        }

        public override void PostAddRecipes()
        {
            if (!FandomMode.IsFull)
                return;
            foreach (var recipe in Main.recipe.Take(Recipe.numRecipes))
            {
                if (!recipe.Disabled && HiddenItems.Contains(recipe.createItem.type))
                {
                    recipe.DisableRecipe();
                    DisabledRecipes++;
                }
            }

            // Butin : on retire une règle seulement si tout ce qu'elle peut donner est masqué
            for (int npc = 1; npc < NPCLoader.NPCCount; npc++)
                foreach (var rule in Main.ItemDropsDB.GetRulesForNPCID(npc, false).ToList())
                    if (OnlyHidden(rule))
                    {
                        Main.ItemDropsDB.RemoveFromNPC(npc, rule);
                        RemovedDrops++;
                    }
            for (int item = 1; item < ItemLoader.ItemCount; item++)
                foreach (var rule in Main.ItemDropsDB.GetRulesForItemID(item).ToList())
                    if (OnlyHidden(rule))
                    {
                        Main.ItemDropsDB.RemoveFromItem(item, rule);
                        RemovedDrops++;
                    }
        }

        public static bool OnlyHidden(IItemDropRule rule)
        {
            var rates = new List<DropRateInfo>();
            rule.ReportDroprates(rates, new DropRateInfoChainFeed(1f));
            return rates.Count > 0 && rates.All(r => HiddenItems.Contains(r.itemId));
        }
    }

    public class FullFandomHidingNPC : GlobalNPC
    {
        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            foreach (int type in FullFandomHiding.HiddenNPCs)
                pool.Remove(type);
        }

        public override void ModifyActiveShop(NPC npc, string shopName, Item[] items)
        {
            foreach (var item in items)
                if (item != null && FullFandomHiding.HiddenItems.Contains(item.type))
                    item.TurnToAir();
        }
    }
}
