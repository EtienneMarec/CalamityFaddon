using System.Collections.Generic;
using System.Text.Json;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Items
{
    /// <summary>
    /// Ailes inventées par le wiki (24/10), une instance par entrée « wings » de WikiData/items.json (tools/gen_items.py) :
    /// vitesse horizontale, multiplicateur d'accélération et temps de vol de l'infobulle, vitesse verticale traduite des mots
    /// du wiki ([choix]), quelques bonus simples en plus. Image portée : la pose unique du wiki répétée sur 4 images
    /// (elles ne battent pas ; Assets/WikiItems/&lt;Nom&gt;_Wings.png).
    /// </summary>
    [Autoload(false)]
    public class WikiWings : ModItem
    {
        public class Extra
        {
            public string Type { get; set; }
            public int Value { get; set; }
        }

        public class Data
        {
            public string Name { get; set; }
            public string DisplayName { get; set; }
            public string Page { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public int Rare { get; set; }
            public int Value { get; set; }
            public float Horizontal { get; set; }
            public float Acceleration { get; set; }
            /// <summary>Temps de vol en ticks ; -1 = infini (« Flight time: ∞ », Holy Mantle).</summary>
            public int FlightTime { get; set; }
            public string Vertical { get; set; } = "average";
            public List<Extra> Extras { get; set; } = new();
        }

        private class ItemsFile
        {
            public List<Data> Wings { get; set; } = new();
        }

        /// <summary>Vitesse verticale du wiki -> (ascentWhenFalling, ascentWhenRising, maxCanAscendMultiplier, maxAscentMultiplier).</summary>
        private static readonly Dictionary<string, (float falling, float rising, float canAscend, float ascent)> VerticalSpeeds = new()
        {
            ["average"] = (0.85f, 0.15f, 1f, 2f),
            ["good"] = (0.85f, 0.15f, 1f, 2.5f),
            ["great"] = (0.95f, 0.16f, 1.1f, 3f),
            ["excellent"] = (1f, 0.17f, 1.2f, 3.25f),
        };

        private readonly Data data;

        public WikiWings(Data data) => this.data = data;

        public override string Name => data.Name;
        public override string Texture => $"CalamityFandom/Assets/WikiItems/{data.Name}";
        public override string LocalizationCategory => "WikiItems";
        protected override bool CloneNewInstances => true;

        public static void LoadAll(Mod mod)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var file = JsonSerializer.Deserialize<ItemsFile>(mod.GetFileBytes("WikiData/items.json"), options);
            foreach (var entry in file.Wings)
                mod.AddContent(new WikiWings(entry));
        }

        public override void Load() => EquipLoader.AddEquipTexture(Mod, $"{Texture}_Wings", EquipType.Wings, this);

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            int flight = data.FlightTime < 0 ? 300 : data.FlightTime;
            // Objet instancié : Item.wingSlot n'est pas encore rempli ici, on demande l'emplacement à tModLoader
            ArmorIDs.Wing.Sets.Stats[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Wings)] = new WingStats(flight, data.Horizontal, data.Acceleration);
        }

        public override void SetDefaults()
        {
            Item.width = data.Width;
            Item.height = data.Height;
            Item.value = data.Value;
            Item.rare = Common.WikiData.WikiStats.ResolveRarity(data.Rare);
            Item.accessory = true;
            Item.wingSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Wings);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (data.FlightTime < 0)
                player.wingTime = player.wingTimeMax; // vol infini
            foreach (var extra in data.Extras)
            {
                switch (extra.Type)
                {
                    case "moveSpeed":
                        player.moveSpeed += extra.Value / 100f;
                        break;
                    case "jumpSpeed":
                        player.jumpSpeedBoost += extra.Value / 100f * 5f; // jumpSpeedBoost : 1 ≈ +20 % de saut
                        break;
                    case "damage":
                        player.GetDamage(DamageClass.Generic) += extra.Value / 100f;
                        break;
                    case "defense":
                        player.statDefense += extra.Value;
                        break;
                }
            }
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising,
            ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            var speeds = VerticalSpeeds.TryGetValue(data.Vertical, out var s) ? s : VerticalSpeeds["average"];
            ascentWhenFalling = speeds.falling;
            ascentWhenRising = speeds.rising;
            maxCanAscendMultiplier = speeds.canAscend;
            maxAscentMultiplier = speeds.ascent;
            constantAscend = 0.135f;
        }
    }
}
