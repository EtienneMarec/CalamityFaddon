using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace CalamityFandom.Common.Taming.Chess
{
    /// <summary>
    /// Séance d'échecs du Holographic Chessboard : 3 problèmes réussis apprivoisent le boss (« Beat the Brain of Cthulhu at
    /// chess using the Holographic Chessboard », « Defeat Hypnos at chess »). [choix] Un mauvais coup fait perdre le
    /// problème (un autre est tiré, sans compter) ; sans boss, la séance sert d'entraînement.
    /// </summary>
    public class ChessSession
    {
        public const int Needed = 3;

        /// <summary>Boss à apprivoiser (clé de la page Taming), ou null à l'entraînement.</summary>
        public readonly string Boss;
        public readonly bool MateInTwo;
        public int Solved;
        public ChessPosition Position;
        /// <summary>Camp du joueur dans le problème en cours (1 Blancs, -1 Noirs).</summary>
        public int Player;
        /// <summary>0 : premier coup du joueur ; 1 : réponse de l'adversaire en attente ; 2 : coup du mat.</summary>
        public int Phase;
        public int Selected = -1;
        public int ReplyTimer;
        public string Status = "";
        public Color StatusColor = Color.White;
        private readonly HashSet<int> used = new();
        private int nextTimer;

        public ChessSession(string boss, bool mateInTwo)
        {
            Boss = boss;
            MateInTwo = mateInTwo;
            Next();
        }

        private List<ChessPuzzles.Puzzle> Pool => MateInTwo ? ChessPuzzles.Mate2 : ChessPuzzles.Mate1;

        private void Next()
        {
            var puzzle = Pool[ChessPuzzles.Pick(Pool, used)];
            Position = ChessPosition.FromFen(puzzle.Fen);
            Player = Position.Side;
            Phase = 0;
            Selected = -1;
        }

        public bool Waiting => nextTimer > 0 || ReplyTimer > 0;

        /// <summary>Le joueur joue de <paramref name="from"/> vers <paramref name="to"/> (promotion en dame).</summary>
        public void Play(int from, int to)
        {
            ChessMove? found = null;
            foreach (var m in Position.LegalMoves())
                if (m.From == from && m.To == to && (m.Promotion == 0 || m.Promotion == ChessPosition.Queen))
                    found = m;
            if (found is not ChessMove move)
                return;
            bool good = MateInTwo && Phase == 0 ? Position.ForcesMateInTwo(move) : Position.Play(move).IsCheckmate();
            Position = Position.Play(move);
            Selected = -1;
            SoundEngine.PlaySound(SoundID.MenuTick);
            if (!good)
            {
                Say("Failed", Color.OrangeRed);
                nextTimer = 90;
                return;
            }
            if (MateInTwo && Phase == 0)
            {
                Phase = 1;
                ReplyTimer = 40;
                return;
            }
            Solved++;
            if (Boss != null)
                TamingSystem.Report(Boss, 1f);
            SoundEngine.PlaySound(SoundID.Item4);
            Say(Solved >= Needed ? "Won" : "Solved", Color.LightGreen, Solved, Needed);
            nextTimer = 90;
        }

        private void Say(string key, Color color, params object[] args)
        {
            Status = Language.GetTextValue("Mods.CalamityFandom.Taming.Chess." + key, args);
            StatusColor = color;
        }

        /// <summary>Renvoie faux quand la séance est finie.</summary>
        public bool Update()
        {
            if (ReplyTimer > 0 && --ReplyTimer == 0)
            {
                var replies = Position.LegalMoves();
                Position = Position.Play(replies[Main.rand.Next(replies.Count)]);
                Phase = 2;
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
            if (nextTimer > 0 && --nextTimer == 0)
            {
                if (Solved >= Needed && Boss != null)
                    return false;
                Status = "";
                Next();
            }
            return true;
        }
    }

    /// <summary>Échiquier affiché au centre de l'écran.</summary>
    public class ChessBoardElement : UIElement
    {
        public const int Square = 44;
        private const int Header = 54, Footer = 30;

        public ChessBoardElement()
        {
            Width.Set(Square * 8 + 20, 0f);
            Height.Set(Square * 8 + Header + Footer, 0f);
            HAlign = VAlign = 0.5f;
        }

        private static ChessSession Session => ChessUISystem.Session;

        private Vector2 BoardOrigin => GetDimensions().Position() + new Vector2(10, Header);

        /// <summary>Case sous la souris (échiquier vu du côté du joueur), ou -1.</summary>
        private int SquareAt(Vector2 mouse)
        {
            Vector2 local = mouse - BoardOrigin;
            if (local.X < 0 || local.Y < 0 || local.X >= Square * 8 || local.Y >= Square * 8)
                return -1;
            int col = (int)(local.X / Square), row = (int)(local.Y / Square);
            return Session.Player > 0 ? (7 - row) * 8 + col : row * 8 + (7 - col);
        }

        private Vector2 ScreenOf(int sq)
        {
            int file = ChessPosition.File(sq), rank = ChessPosition.Rank(sq);
            int col = Session.Player > 0 ? file : 7 - file, row = Session.Player > 0 ? 7 - rank : rank;
            return BoardOrigin + new Vector2(col * Square, row * Square);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            var s = Session;
            if (s == null || s.Waiting || s.Position.Side != s.Player)
                return;
            int sq = SquareAt(evt.MousePosition);
            if (sq < 0)
                return;
            if (s.Position.Board[sq] * s.Player > 0)
                s.Selected = sq;
            else if (s.Selected >= 0)
                s.Play(s.Selected, sq);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (IsMouseHovering)
                Main.LocalPlayer.mouseInterface = true;
        }

        protected override void DrawSelf(SpriteBatch sb)
        {
            var s = Session;
            if (s == null)
                return;
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            Rectangle panel = GetDimensions().ToRectangle();
            sb.Draw(pixel, panel, new Color(10, 20, 45, 230));

            string title = s.Boss != null
                ? Language.GetTextValue("Mods.CalamityFandom.Taming.Chess.Title", s.Boss, s.Solved, ChessSession.Needed)
                : Language.GetTextValue("Mods.CalamityFandom.Taming.Chess.Practice");
            Utils.DrawBorderString(sb, title, panel.TopLeft() + new Vector2(10, 6), Color.Cyan);
            string side = Language.GetTextValue("Mods.CalamityFandom.Taming.Chess." + (s.Player > 0 ? "White" : "Black"));
            string goal = Language.GetTextValue("Mods.CalamityFandom.Taming.Chess." + (s.MateInTwo ? "MateIn2" : "MateIn1"), side);
            Utils.DrawBorderString(sb, goal, panel.TopLeft() + new Vector2(10, 28), Color.White, 0.9f);

            var legalTargets = new HashSet<int>();
            if (s.Selected >= 0)
                foreach (var m in s.Position.LegalMoves())
                    if (m.From == s.Selected)
                        legalTargets.Add(m.To);
            int hovered = SquareAt(Main.MouseScreen);
            for (int sq = 0; sq < 64; sq++)
            {
                Vector2 pos = ScreenOf(sq);
                var rect = new Rectangle((int)pos.X, (int)pos.Y, Square, Square);
                bool light = (ChessPosition.File(sq) + ChessPosition.Rank(sq)) % 2 == 1;
                Color color = light ? new Color(120, 200, 255, 210) : new Color(35, 85, 160, 210);
                if (sq == s.Selected)
                    color = new Color(255, 220, 90, 230);
                else if (sq == hovered)
                    color = Color.Lerp(color, Color.White, 0.25f);
                sb.Draw(pixel, rect, color);
                if (legalTargets.Contains(sq))
                    sb.Draw(pixel, new Rectangle(rect.X + Square / 2 - 5, rect.Y + Square / 2 - 5, 10, 10), new Color(255, 230, 120, 220));
                DrawPiece(sb, s.Position.Board[sq], rect);
            }
            if (s.Status.Length > 0)
                Utils.DrawBorderString(sb, s.Status, new Vector2(panel.X + 10, panel.Bottom - Footer + 4), s.StatusColor);
            else
                Utils.DrawBorderString(sb, Language.GetTextValue("Mods.CalamityFandom.Taming.Chess.Close"),
                    new Vector2(panel.X + 10, panel.Bottom - Footer + 4), Color.Gray, 0.8f);
        }

        /// <summary>
        /// Pièce provisoire : lettre (K Q R B N P) cerclée, blanche ou noire. TODO : sprites des pièces (docs/SPRITES.md).
        /// </summary>
        private static void DrawPiece(SpriteBatch sb, int piece, Rectangle rect)
        {
            if (piece == 0)
                return;
            string letter = "PNBRQK"[System.Math.Abs(piece) - 1].ToString();
            bool white = piece > 0;
            var font = FontAssets.DeathText.Value;
            Vector2 size = font.MeasureString(letter) * 0.55f;
            Vector2 at = rect.Center.ToVector2() - size / 2f + new Vector2(0, 2);
            Utils.DrawBorderStringFourWay(sb, font, letter, at.X, at.Y, white ? Color.White : new Color(25, 25, 35),
                white ? new Color(25, 25, 35) : new Color(220, 220, 230), Vector2.Zero, 0.55f);
        }
    }

    /// <summary>Ouvre et ferme l'échiquier ; signale au serveur que le joueur joue (le boss attend la fin de la partie).</summary>
    [Autoload(Side = ModSide.Client)]
    public class ChessUISystem : ModSystem
    {
        public static ChessSession Session { get; private set; }
        private static UserInterface ui;
        private static UIState state;

        public override void Load()
        {
            ui = new UserInterface();
            state = new UIState();
            state.Append(new ChessBoardElement());
        }

        public override void Unload()
        {
            ui = null;
            state = null;
            Session = null;
        }

        /// <summary>Partie face à un PNJ (mise en scène et monde figé), pas un simple entraînement.</summary>
        private static bool scene;

        /// <summary>
        /// Ouvre le plateau. Face à un adversaire (<paramref name="opponent"/>), la mise en scène de ChessScene se joue
        /// d'abord et le plateau s'ouvre après.
        /// </summary>
        public static void Open(string boss, bool mateInTwo, NPC opponent = null)
        {
            Session = new ChessSession(boss, mateInTwo);
            ui.SetState(state);
            scene = opponent != null;
            if (scene)
                ChessScene.Begin(opponent);
        }

        public static void Close()
        {
            if (scene)
                TamingSystem.ReportChess(false);
            scene = false;
            Session = null;
            ui?.SetState(null);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (Session == null)
                return;
            if (Main.LocalPlayer.dead || Main.keyState.IsKeyDown(Keys.Escape))
            {
                Close();
                return;
            }
            // Le plateau s'ouvre après la mise en scène
            if (!ChessScene.IntroDone(Main.LocalPlayer))
                return;
            if (!Session.Update())
            {
                Close();
                return;
            }
            ui.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(l => l.Name == "Vanilla: Mouse Text");
            if (index < 0 || Session == null || !ChessScene.IntroDone(Main.LocalPlayer))
                return;
            layers.Insert(index, new LegacyGameInterfaceLayer("CalamityFandom: Holographic Chessboard", () =>
            {
                ui.Draw(Main.spriteBatch, new GameTime());
                return true;
            }, InterfaceScaleType.UI));
        }

        public override void OnWorldUnload()
        {
            Session = null;
            scene = false;
        }
    }
}
