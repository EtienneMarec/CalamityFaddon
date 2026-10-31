using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Common.Taming.Chess
{
    /// <summary>
    /// Partie d'échecs contre un boss (Holographic Chessboard) : [validé 31/10] tout s'arrête pendant la partie (joueurs,
    /// ennemis, boss et ses attaques, projectiles), et une table et deux chaises en bois apparaissent là où se tient le
    /// joueur (dessinées par-dessus le décor, sans être posées) ; le joueur et le boss y sont placés, puis le joueur serre
    /// un Creeper (un « œil ») avant que le plateau s'ouvre. Table et chaises restent jusqu'à la fin du combat.
    /// [choix] Arrêt de tout le monde, aussi en multijoueur ; poignée de main de 1 s.
    /// </summary>
    public class ChessScene : ModSystem
    {
        /// <summary>Durée de la poignée de main avant l'ouverture du plateau.</summary>
        public const int IntroTicks = 60;
        /// <summary>Écart entre le centre de la table et chaque chaise.</summary>
        public const float ChairOffset = 40f;

        /// <summary>Un joueur joue aux échecs contre un boss : le monde est figé.</summary>
        public static bool Frozen
        {
            get
            {
                foreach (var p in Main.ActivePlayers)
                    if (!p.dead && p.GetModPlayer<TamingPlayer>().ChessOpen)
                        return true;
                return false;
            }
        }

        public override void Load()
        {
            On_NPC.UpdateNPC += (orig, self, i) =>
            {
                if (!Frozen)
                    orig(self, i);
            };
            On_Projectile.Update += (orig, self, i) =>
            {
                if (!Frozen || self.type == ModContent.ProjectileType<ChessTable>())
                    orig(self, i);
            };
        }

        /// <summary>Où se tient la main du joueur pendant la poignée de main (même calcul partout).</summary>
        public static Vector2 HandPosition(Player player, uint age) =>
            player.MountedCenter + new Vector2(player.direction * 24f, 4f + 5f * System.MathF.Sin(age * 0.5f));

        /// <summary>
        /// Place le boss sur sa chaise (là où les PNJ sont gérés : solo ou serveur). Le joueur se place lui-même
        /// (<see cref="TamingPlayer"/>).
        /// </summary>
        public static void Arrange(Player player, Vector2 table)
        {
            NPC boss = BossFor(player);
            if (boss == null)
                return;
            boss.Center = table + new Vector2(ChairOffset + 20f, -boss.height / 2f - 4f);
            boss.velocity = Vector2.Zero;
            boss.netUpdate = true;
        }

        private static NPC BossFor(Player player)
        {
            foreach (var n in Main.ActiveNPCs)
                if (n.type == NPCID.BrainofCthulhu && n.WithinRange(player.Center, 3000f))
                    return n;
            return null;
        }

        // Poignée de main : le Creeper le plus proche vient serrer la main du joueur (les PNJ sont figés, on les place)
        public override void PostUpdateNPCs()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            foreach (var p in Main.ActivePlayers)
            {
                var taming = p.GetModPlayer<TamingPlayer>();
                uint age = Main.GameUpdateCount - taming.ChessStart;
                if (!taming.ChessOpen || age > IntroTicks)
                    continue;
                NPC eye = null;
                foreach (var n in Main.ActiveNPCs)
                    if (n.type == NPCID.Creeper && (eye == null || n.DistanceSQ(p.Center) < eye.DistanceSQ(p.Center)))
                        eye = n;
                if (eye == null)
                    continue;
                eye.Center = HandPosition(p, age) + new Vector2(p.direction * eye.width / 2f, 0f);
                eye.velocity = Vector2.Zero;
                if (age % 6 == 0 || age == IntroTicks)
                    eye.netUpdate = true;
            }
        }
    }

    /// <summary>Table et chaises en bois de la partie d'échecs : dessin seulement, retirées quand le boss n'est plus là.</summary>
    public class ChessTable : ModProjectile
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.WoodenTable;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hide = true;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Projectile.velocity = Vector2.Zero;
            bool fight = false;
            foreach (var n in Main.ActiveNPCs)
                if (n.type == NPCID.BrainofCthulhu)
                    fight = true;
            if (fight)
                Projectile.timeLeft = 2;
        }

        public override bool? CanDamage() => false;

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles,
            List<int> overPlayers, List<int> overWiresUI) => behindNPCs.Add(index);

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadTiles(TileID.Tables);
            Main.instance.LoadTiles(TileID.Chairs);
            Texture2D table = TextureAssets.Tile[TileID.Tables].Value;
            Texture2D chair = TextureAssets.Tile[TileID.Chairs].Value;
            // Projectile.Center = milieu du bas de la table (le sol sous les pieds du joueur au départ)
            Vector2 floor = Projectile.Center;
            // Table en bois : 3 × 2 cases (rangées de 16 et 18 px, cadres espacés de 18 px)
            for (int x = 0; x < 3; x++)
                for (int y = 0; y < 2; y++)
                    DrawPiece(table, new Rectangle(x * 18, y * 18, 16, y == 0 ? 16 : 18), floor + new Vector2(-24 + x * 16, -32 + y * 16));
            // Chaises en bois : 1 × 2 cases (16 et 18 px) ; cadre 0 tourné vers la gauche, 18 vers la droite
            for (int y = 0; y < 2; y++)
            {
                DrawPiece(chair, new Rectangle(18, y * 18, 16, y == 0 ? 16 : 18), floor + new Vector2(-ChessScene.ChairOffset - 8, -32 + y * 16));
                DrawPiece(chair, new Rectangle(0, y * 18, 16, y == 0 ? 16 : 18), floor + new Vector2(ChessScene.ChairOffset - 8, -32 + y * 16));
            }
            return false;
        }

        private static void DrawPiece(Texture2D texture, Rectangle source, Vector2 topLeft)
        {
            Color light = Lighting.GetColor((topLeft + new Vector2(8f)).ToTileCoordinates());
            Main.EntitySpriteDraw(texture, topLeft - Main.screenPosition, source, light, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }
    }
}
