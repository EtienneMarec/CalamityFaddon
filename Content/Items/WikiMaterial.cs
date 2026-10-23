using System.Collections.Generic;
using System.Text.Json;
using CalamityFandom.Common.WikiData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Items
{
    /// <summary>
    /// Matériau d'artisanat (ou aliment, s'il a un buff de nourriture) du wiki, sans comportement propre : une instance par entrée de WikiData/items.json
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
            /// <summary>Taille au sol (dessin et ramassage), d'après une image du wiki ; 1 = normale.</summary>
            public float WorldScale { get; set; } = 1f;
            /// <summary>Version détaillée pour le sol (Assets/WikiItems/&lt;Nom&gt;_World.png), à la taille de la hitbox.</summary>
            public bool WorldTexture { get; set; }
            /// <summary>Aliment (24/10) : buff de nourriture vanilla (WellFed, WellFed2, WellFed3), durée en secondes, use.</summary>
            public string FoodBuff { get; set; }
            public int FoodSeconds { get; set; }
            public int UseTime { get; set; } = 17;
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
            Item.width = (int)(data.Width * data.WorldScale);
            Item.height = (int)(data.Height * data.WorldScale);
            Item.maxStack = data.MaxStack;
            Item.value = data.Value;
            Item.rare = WikiStats.ResolveRarity(data.Rare);
            if (data.FoodBuff != null && BuffID.Search.TryGetId(data.FoodBuff, out int buff))
            {
                // DefaultToFood remet taille, pile et prix : on les reprend du wiki
                Item.DefaultToFood(Item.width, Item.height, buff, data.FoodSeconds * 60);
                Item.useTime = Item.useAnimation = data.UseTime;
                Item.maxStack = data.MaxStack;
                Item.value = data.Value;
                Item.rare = WikiStats.ResolveRarity(data.Rare);
            }
        }

        /// <summary>Objet géant au sol (Yharim Bar) : dessiné à la taille de sa hitbox, posé sur sa base ; version détaillée si fournie.</summary>
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            if (data.WorldScale == 1f)
                return true;
            Texture2D texture = data.WorldTexture
                ? ModContent.Request<Texture2D>(Texture + "_World").Value
                : TextureAssets.Item[Type].Value;
            Rectangle frame = data.WorldTexture ? texture.Frame() : Main.itemAnimations[Type]?.GetFrame(texture) ?? texture.Frame();
            float drawScale = scale * Item.width / frame.Width;
            spriteBatch.Draw(texture, Item.Bottom - Main.screenPosition, frame, alphaColor, rotation,
                new Vector2(frame.Width / 2f, frame.Height), drawScale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
