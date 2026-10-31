using CalamityFandom.Content.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityFandom.Content.Tiles
{
    /// <summary>
    /// Tuile d'un matériau du wiki qui se pose (« Block » dans son infobox, image « (placed) » sur le wiki) : une instance
    /// par matériau de WikiData/items.json qui a un champ « tile » (texture dans Assets/WikiTiles/, tools/gen_items.py).
    /// Bloc : plein, la planche reprend le bloc du milieu de l'image du wiki (sans bordures). Barre : tuile 1 × 1 posée sur
    /// un sol ou une autre barre, comme les barres de métal du jeu. Le matériau tombe quand on la casse (tModLoader).
    /// [choix] Minerais (« Ore », « Slag », « Cluster ») brillants au Spelunker et repérés par le détecteur de métaux ;
    /// aucune pioche minimale (ces blocs ne sont pas générés dans le monde, seulement posés par le joueur).
    /// </summary>
    [Autoload(false)]
    public class WikiTile : ModTile
    {
        private readonly WikiMaterial.Data data;

        public WikiTile(WikiMaterial.Data data) => this.data = data;

        public override string Name => data.Name + "Tile";
        public override string Texture => $"CalamityFandom/Assets/WikiTiles/{data.Name}";

        private bool IsBar => data.Tile == "bar";

        public override void SetStaticDefaults()
        {
            if (IsBar)
            {
                Main.tileShine[Type] = 1100;
                Main.tileSolidTop[Type] = true;
                Main.tileFrameImportant[Type] = true;
                TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
                TileObjectData.newTile.LavaDeath = false;
                TileObjectData.addTile(Type);
            }
            else
            {
                Main.tileSolid[Type] = true;
                Main.tileBlockLight[Type] = true;
                string lower = data.DisplayName.ToLowerInvariant();
                if (lower.EndsWith(" ore") || lower.EndsWith(" slag") || lower.EndsWith(" cluster"))
                {
                    TileID.Sets.Ore[Type] = true;
                    Main.tileSpelunker[Type] = true;
                    Main.tileOreFinderPriority[Type] = 500;
                }
            }
            var color = data.MapColor is { Length: 3 } c ? new Color(c[0], c[1], c[2]) : Color.Gray;
            AddMapEntry(color);
            DustType = DustID.Stone;
        }
    }
}
