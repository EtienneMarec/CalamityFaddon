using System.Collections.Generic;
using System.Text.Json;
using CalamityFandom.Common.WikiData;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Items
{
    /// <summary>
    /// Matériau d'artisanat du wiki, sans comportement propre : une instance par entrée de WikiData/items.json
    /// (généré par tools/gen_items.py, sprite dans Assets/WikiItems/, textes dans Mods.CalamityFandom.WikiItems.hjson).
    /// Un matériau qui doit faire plus (se poser, avoir un effet) mérite sa propre classe : le retirer alors de la
    /// génération via tools/gen_items.py.
    /// </summary>
    [Autoload(false)]
    public class WikiMaterial : ModItem
    {
        public class Data
        {
            public string Name { get; set; }
            public string DisplayName { get; set; }
            public string Page { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public int Frames { get; set; } = 1;
            public int TicksPerFrame { get; set; } = 5;
            public int Rare { get; set; }
            public int MaxStack { get; set; } = 9999;
            public int Value { get; set; }
        }

        private class ItemsFile
        {
            public List<Data> Materials { get; set; } = new();
        }

        private readonly Data data;

        public WikiMaterial(Data data) => this.data = data;

        public override string Name => data.Name;
        public override string Texture => $"CalamityFandom/Assets/WikiItems/{data.Name}";
        public override string LocalizationCategory => "WikiItems";
        protected override bool CloneNewInstances => true;

        /// <summary>Crée un objet par matériau du JSON, sauf si un objet vanilla porte déjà ce nom.</summary>
        public static void LoadAll(Mod mod)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var file = JsonSerializer.Deserialize<ItemsFile>(mod.GetFileBytes("WikiData/items.json"), options);
            var vanillaNames = new HashSet<string>();
            for (int type = 1; type < ItemID.Count; type++)
                vanillaNames.Add(WikiRecipes.Normalize(Lang.GetItemNameValue(type)));
            foreach (var entry in file.Materials)
            {
                if (vanillaNames.Contains(WikiRecipes.Normalize(entry.DisplayName)))
                    continue;
                mod.AddContent(new WikiMaterial(entry));
            }
        }

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
            if (data.Frames > 1)
            {
                Main.RegisterItemAnimation(Type, new DrawAnimationVertical(data.TicksPerFrame, data.Frames));
                ItemID.Sets.AnimatesAsSoul[Type] = true;
                ItemID.Sets.ItemNoGravity[Type] = true;
            }
        }

        public override void SetDefaults()
        {
            Item.width = data.Width;
            Item.height = data.Height;
            Item.maxStack = data.MaxStack;
            Item.value = data.Value;
            Item.rare = WikiStats.ResolveRarity(data.Rare);
        }
    }
}
