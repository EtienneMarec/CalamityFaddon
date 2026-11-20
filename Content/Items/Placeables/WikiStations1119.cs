using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

// Lot AI (19/11) : stations d'artisanat du wiki (recettes du wiki). Les recettes qui les demandent les trouvent par le nom
// de l'objet (WikiRecipes : objet → tuile posée).

namespace CalamityFandom.Content.Items.Placeables
{
    /// <summary>Station du wiki : objet posable dont la tuile porte le même nom suivi de « Tile ».</summary>
    public abstract class WikiStation : ModItem
    {
        protected abstract int TileType { get; }
        protected abstract int Rarity { get; }

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(TileType);
            Item.width = Item.height = 32;
            Item.rare = Rarity;
            Item.value = 0;
        }
    }

    /// <summary>Wiki : Bronze Smelter.txt — station 3 × 3 (mobilier Bronze), source de lumière ; « No value ».</summary>
    public class BronzeSmelter : WikiStation
    {
        protected override int TileType => ModContent.TileType<Tiles.BronzeSmelterTile>();
        protected override int Rarity => ModContent.RarityType<CalamityMod.Rarities.BurnishedAuric>();
    }

    /// <summary>
    /// Wiki : Scroll Press.txt — station 3 × 2 pour les parchemins, qui sert aussi de Loom, Sawmill, Dye Vat et de source
    /// d'eau.
    /// </summary>
    public class ScrollPress : WikiStation
    {
        protected override int TileType => ModContent.TileType<Tiles.ScrollPressTile>();
        protected override int Rarity => ItemRarityID.Blue;
    }

    /// <summary>
    /// Wiki : Entropic Refiner.txt — station pour la Quark-Gluon Sludge et les Omnicidal Sludge Bars, qui sert aussi de Static
    /// Refiner, de Crystal Ball et de source d'eau, de lave, de miel et de Shimmer. [technique] 4 × 4 cases comme l'image du
    /// bloc posé (la page dit 3 × 3).
    /// </summary>
    public class EntropicRefiner : WikiStation
    {
        protected override int TileType => ModContent.TileType<Tiles.EntropicRefinerTile>();
        protected override int Rarity => ItemRarityID.Blue;
    }

    /// <summary>Wiki : Advanced Void Condenser.txt — station 3 × 3 (mobilier Skylac), source de lumière ; rareté « C1 » → 0.</summary>
    public class AdvancedVoidCondenser : WikiStation
    {
        protected override int TileType => ModContent.TileType<Tiles.AdvancedVoidCondenserTile>();
        protected override int Rarity => ItemRarityID.White;
    }
}

namespace CalamityFandom.Content.Tiles
{
    /// <summary>Tuile d'une station du wiki (planche faite par tools/import_sprite.py --tile depuis l'image « (placed) »).</summary>
    public abstract class WikiStationTile : ModTile
    {
        protected abstract int Width { get; }
        protected abstract int Height { get; }
        protected abstract Color MapColor { get; }
        protected virtual Vector3 Light => Vector3.Zero;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileLighted[Type] = Light != Vector3.Zero;
            TileObjectData.newTile.CopyFrom(Height == 3 ? TileObjectData.Style3x3 : TileObjectData.Style3x2);
            TileObjectData.newTile.Width = Width;
            TileObjectData.newTile.Height = Height;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.Table, Width, 0);
            TileObjectData.newTile.CoordinateHeights = new int[Height];
            for (int i = 0; i < Height; i++)
                TileObjectData.newTile.CoordinateHeights[i] = 16;
            TileObjectData.newTile.Origin = new Point16(Width / 2, Height - 1);
            TileObjectData.addTile(Type);
            AddMapEntry(MapColor, Language.GetText($"Mods.CalamityFandom.Items.{Name[..^4]}.DisplayName"));
            DustType = DustID.Iron;
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = Light.X;
            g = Light.Y;
            b = Light.Z;
        }
    }

    public class BronzeSmelterTile : WikiStationTile
    {
        protected override int Width => 3;
        protected override int Height => 3;
        protected override Color MapColor => new(0x7A, 0x50, 0x34);
        protected override Vector3 Light => new(0.8f, 0.45f, 0.2f);
    }

    public class ScrollPressTile : WikiStationTile
    {
        protected override int Width => 3;
        protected override int Height => 2;
        protected override Color MapColor => new(0x9B, 0xAB, 0x75);

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            AdjTiles = new int[] { TileID.Loom, TileID.Sawmill, TileID.DyeVat };
            TileID.Sets.CountsAsWaterSource[Type] = true;
            DustType = DustID.WoodFurniture;
        }
    }

    public class EntropicRefinerTile : WikiStationTile
    {
        protected override int Width => 4;
        protected override int Height => 4;
        protected override Color MapColor => new(0x00, 0x6E, 0xC0);
        protected override Vector3 Light => new(0.1f, 0.4f, 0.8f);

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            AdjTiles = new int[] { ModContent.TileType<CalamityMod.Tiles.FurnitureStatigel.StaticRefiner>(), TileID.CrystalBall };
            TileID.Sets.CountsAsWaterSource[Type] = true;
            TileID.Sets.CountsAsLavaSource[Type] = true;
            TileID.Sets.CountsAsHoneySource[Type] = true;
            TileID.Sets.CountsAsShimmerSource[Type] = true;
        }
    }

    public class AdvancedVoidCondenserTile : WikiStationTile
    {
        protected override int Width => 3;
        protected override int Height => 3;
        protected override Color MapColor => new(0x0A, 0x0A, 0xFF);
        protected override Vector3 Light => new(0.15f, 0.15f, 0.9f);
    }
}
