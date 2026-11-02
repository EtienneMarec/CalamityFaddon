using CalamityFandom.Content.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Tiles
{
    /// <summary>
    /// Mur d'un matériau du wiki (« Wall » dans son infobox, image « (placed) » sur le wiki) : une instance par matériau de
    /// WikiData/items.json qui a « wall » (texture dans Assets/WikiWalls/, tools/gen_items.py). [choix] La planche reprend
    /// le carré de 32 px du milieu de l'image du wiki dans toutes ses cases (pas de variantes de bord) ; le mur posé par le
    /// joueur rend l'objet quand on le casse.
    /// </summary>
    [Autoload(false)]
    public class WikiWall : ModWall
    {
        private readonly WikiMaterial.Data data;

        public WikiWall(WikiMaterial.Data data) => this.data = data;

        public override string Name => data.Name + "Wall";
        public override string Texture => $"CalamityFandom/Assets/WikiWalls/{data.Name}";

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            var color = data.MapColor is { Length: 3 } c ? new Color(c[0], c[1], c[2]) : Color.Gray;
            AddMapEntry(color);
            DustType = DustID.Stone;
        }
    }
}
