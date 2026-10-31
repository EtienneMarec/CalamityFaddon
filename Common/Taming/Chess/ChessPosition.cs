using System;
using System.Collections.Generic;
using System.Text;

namespace CalamityFandom.Common.Taming.Chess
{
    /// <summary>Coup : cases de départ et d'arrivée (a1 = 0, h8 = 63), promotion éventuelle (type de pièce positif).</summary>
    public readonly record struct ChessMove(int From, int To, int Promotion = 0)
    {
        public string Uci => ChessPosition.SquareName(From) + ChessPosition.SquareName(To)
            + (Promotion > 0 ? "nbrq"[Promotion - 2].ToString() : "");
    }

    /// <summary>
    /// Position d'échecs complète (roque, prise en passant, promotion) : sert aux problèmes du Holographic Chessboard
    /// (Taming du Brain of Cthulhu et de XP-00 Hypnos) et à leur génération (tools/chess_puzzles). Sans dépendance à
    /// Terraria. Pièces : 1 pion, 2 cavalier, 3 fou, 4 tour, 5 dame, 6 roi ; positives pour les Blancs.
    /// </summary>
    public sealed class ChessPosition
    {
        public const int Pawn = 1, Knight = 2, Bishop = 3, Rook = 4, Queen = 5, King = 6;

        public readonly int[] Board = new int[64];
        /// <summary>1 : aux Blancs de jouer, -1 : aux Noirs.</summary>
        public int Side = 1;
        /// <summary>Roques possibles : 1 K, 2 Q, 4 k, 8 q.</summary>
        public int Castling;
        public int EnPassant = -1;

        public static int File(int sq) => sq & 7;
        public static int Rank(int sq) => sq >> 3;
        public static string SquareName(int sq) => $"{(char)('a' + File(sq))}{Rank(sq) + 1}";

        public ChessPosition Clone()
        {
            var p = new ChessPosition { Side = Side, Castling = Castling, EnPassant = EnPassant };
            Array.Copy(Board, p.Board, 64);
            return p;
        }

        public static ChessPosition FromFen(string fen)
        {
            var p = new ChessPosition();
            string[] parts = fen.Trim().Split(' ');
            int rank = 7, file = 0;
            foreach (char c in parts[0])
            {
                if (c == '/')
                {
                    rank--;
                    file = 0;
                }
                else if (char.IsDigit(c))
                    file += c - '0';
                else
                {
                    int type = "pnbrqk".IndexOf(char.ToLowerInvariant(c)) + 1;
                    p.Board[rank * 8 + file++] = char.IsUpper(c) ? type : -type;
                }
            }
            p.Side = parts.Length > 1 && parts[1] == "b" ? -1 : 1;
            if (parts.Length > 2)
                foreach (char c in parts[2])
                    p.Castling |= c switch { 'K' => 1, 'Q' => 2, 'k' => 4, 'q' => 8, _ => 0 };
            if (parts.Length > 3 && parts[3] != "-")
                p.EnPassant = (parts[3][1] - '1') * 8 + parts[3][0] - 'a';
            return p;
        }

        public string ToFen()
        {
            var sb = new StringBuilder();
            for (int rank = 7; rank >= 0; rank--)
            {
                int empty = 0;
                for (int file = 0; file < 8; file++)
                {
                    int piece = Board[rank * 8 + file];
                    if (piece == 0)
                    {
                        empty++;
                        continue;
                    }
                    if (empty > 0)
                        sb.Append(empty);
                    empty = 0;
                    char c = "pnbrqk"[Math.Abs(piece) - 1];
                    sb.Append(piece > 0 ? char.ToUpperInvariant(c) : c);
                }
                if (empty > 0)
                    sb.Append(empty);
                if (rank > 0)
                    sb.Append('/');
            }
            string castling = (Castling & 1) != 0 ? "K" : "";
            castling += (Castling & 2) != 0 ? "Q" : "";
            castling += (Castling & 4) != 0 ? "k" : "";
            castling += (Castling & 8) != 0 ? "q" : "";
            sb.Append(Side > 0 ? " w " : " b ").Append(castling.Length > 0 ? castling : "-").Append(' ')
                .Append(EnPassant >= 0 ? SquareName(EnPassant) : "-").Append(" 0 1");
            return sb.ToString();
        }

        private static readonly (int df, int dr)[] KnightDirs = { (1, 2), (2, 1), (2, -1), (1, -2), (-1, -2), (-2, -1), (-2, 1), (-1, 2) };
        private static readonly (int df, int dr)[] KingDirs = { (1, 0), (1, 1), (0, 1), (-1, 1), (-1, 0), (-1, -1), (0, -1), (1, -1) };
        private static readonly (int df, int dr)[] BishopDirs = { (1, 1), (-1, 1), (-1, -1), (1, -1) };
        private static readonly (int df, int dr)[] RookDirs = { (1, 0), (0, 1), (-1, 0), (0, -1) };

        private static int Offset(int sq, int df, int dr)
        {
            int f = File(sq) + df, r = Rank(sq) + dr;
            return f < 0 || f > 7 || r < 0 || r > 7 ? -1 : r * 8 + f;
        }

        public int KingSquare(int side) => Array.IndexOf(Board, King * side);

        /// <summary>La case est-elle attaquée par le camp <paramref name="by"/> ?</summary>
        public bool Attacked(int sq, int by)
        {
            // Pions : un pion blanc attaque vers le haut, donc il se trouve en dessous de la case
            foreach (int df in new[] { -1, 1 })
            {
                int from = Offset(sq, df, -by);
                if (from >= 0 && Board[from] == Pawn * by)
                    return true;
            }
            foreach (var (df, dr) in KnightDirs)
            {
                int from = Offset(sq, df, dr);
                if (from >= 0 && Board[from] == Knight * by)
                    return true;
            }
            foreach (var (df, dr) in KingDirs)
            {
                int from = Offset(sq, df, dr);
                if (from >= 0 && Board[from] == King * by)
                    return true;
            }
            if (SlideAttack(sq, by, BishopDirs, Bishop) || SlideAttack(sq, by, RookDirs, Rook))
                return true;
            return false;
        }

        private bool SlideAttack(int sq, int by, (int df, int dr)[] dirs, int slider)
        {
            foreach (var (df, dr) in dirs)
            {
                int cur = Offset(sq, df, dr);
                while (cur >= 0)
                {
                    int piece = Board[cur];
                    if (piece != 0)
                    {
                        if (piece == slider * by || piece == Queen * by)
                            return true;
                        break;
                    }
                    cur = Offset(cur, df, dr);
                }
            }
            return false;
        }

        public bool InCheck(int side)
        {
            int king = KingSquare(side);
            return king >= 0 && Attacked(king, -side);
        }

        private void PseudoMoves(List<ChessMove> moves)
        {
            int us = Side;
            for (int sq = 0; sq < 64; sq++)
            {
                int piece = Board[sq] * us;
                if (piece <= 0)
                    continue;
                switch (piece)
                {
                    case Pawn:
                        PawnMoves(sq, moves);
                        break;
                    case Knight:
                        Steps(sq, KnightDirs, moves);
                        break;
                    case King:
                        Steps(sq, KingDirs, moves);
                        Castles(sq, moves);
                        break;
                    default:
                        if (piece == Bishop || piece == Queen)
                            Slides(sq, BishopDirs, moves);
                        if (piece == Rook || piece == Queen)
                            Slides(sq, RookDirs, moves);
                        break;
                }
            }
        }

        private void PawnMoves(int sq, List<ChessMove> moves)
        {
            int us = Side;
            int start = us > 0 ? 1 : 6, last = us > 0 ? 7 : 0;
            int one = Offset(sq, 0, us);
            if (one >= 0 && Board[one] == 0)
            {
                AddPawn(sq, one, last, moves);
                int two = Offset(sq, 0, 2 * us);
                if (Rank(sq) == start && Board[two] == 0)
                    moves.Add(new ChessMove(sq, two));
            }
            foreach (int df in new[] { -1, 1 })
            {
                int to = Offset(sq, df, us);
                if (to < 0)
                    continue;
                if (Board[to] * us < 0 || to == EnPassant)
                    AddPawn(sq, to, last, moves);
            }
        }

        private static void AddPawn(int from, int to, int last, List<ChessMove> moves)
        {
            if (Rank(to) == last)
                for (int promo = Queen; promo >= Knight; promo--)
                    moves.Add(new ChessMove(from, to, promo));
            else
                moves.Add(new ChessMove(from, to));
        }

        private void Steps(int sq, (int df, int dr)[] dirs, List<ChessMove> moves)
        {
            foreach (var (df, dr) in dirs)
            {
                int to = Offset(sq, df, dr);
                if (to >= 0 && Board[to] * Side <= 0)
                    moves.Add(new ChessMove(sq, to));
            }
        }

        private void Slides(int sq, (int df, int dr)[] dirs, List<ChessMove> moves)
        {
            foreach (var (df, dr) in dirs)
            {
                int to = Offset(sq, df, dr);
                while (to >= 0)
                {
                    if (Board[to] * Side > 0)
                        break;
                    moves.Add(new ChessMove(sq, to));
                    if (Board[to] != 0)
                        break;
                    to = Offset(to, df, dr);
                }
            }
        }

        private void Castles(int sq, List<ChessMove> moves)
        {
            int home = Side > 0 ? 4 : 60;
            if (sq != home || InCheck(Side))
                return;
            int kingSide = Side > 0 ? 1 : 4, queenSide = Side > 0 ? 2 : 8;
            if ((Castling & kingSide) != 0 && Board[home + 1] == 0 && Board[home + 2] == 0 && Board[home + 3] == Rook * Side
                && !Attacked(home + 1, -Side) && !Attacked(home + 2, -Side))
                moves.Add(new ChessMove(home, home + 2));
            if ((Castling & queenSide) != 0 && Board[home - 1] == 0 && Board[home - 2] == 0 && Board[home - 3] == 0
                && Board[home - 4] == Rook * Side && !Attacked(home - 1, -Side) && !Attacked(home - 2, -Side))
                moves.Add(new ChessMove(home, home - 2));
        }

        /// <summary>Joue le coup (supposé pseudo-légal) sur une copie.</summary>
        public ChessPosition Play(ChessMove m)
        {
            var p = Clone();
            int piece = p.Board[m.From];
            int type = Math.Abs(piece);
            p.Board[m.To] = m.Promotion > 0 ? m.Promotion * Math.Sign(piece) : piece;
            p.Board[m.From] = 0;
            if (type == Pawn && m.To == EnPassant)
                p.Board[m.To - 8 * Side] = 0;
            if (type == King && Math.Abs(m.To - m.From) == 2)
            {
                bool kingSide = m.To > m.From;
                int rookFrom = kingSide ? m.From + 3 : m.From - 4, rookTo = kingSide ? m.From + 1 : m.From - 1;
                p.Board[rookTo] = p.Board[rookFrom];
                p.Board[rookFrom] = 0;
            }
            p.EnPassant = type == Pawn && Math.Abs(m.To - m.From) == 16 ? (m.From + m.To) / 2 : -1;
            foreach (int sq in new[] { m.From, m.To })
            {
                p.Castling &= sq switch { 4 => ~3, 0 => ~2, 7 => ~1, 60 => ~12, 56 => ~8, 63 => ~4, _ => ~0 };
            }
            p.Side = -Side;
            return p;
        }

        public List<ChessMove> LegalMoves()
        {
            var pseudo = new List<ChessMove>();
            PseudoMoves(pseudo);
            var legal = new List<ChessMove>(pseudo.Count);
            foreach (var m in pseudo)
                if (!Play(m).InCheck(Side))
                    legal.Add(m);
            return legal;
        }

        /// <summary>
        /// Position qu'on peut atteindre dans une vraie partie sans promotion : un roi de chaque côté, non adjacents ; pas
        /// plus de pièces qu'au départ (8 pions, 1 dame, 2 tours, 2 cavaliers, 2 fous, et ces deux fous sur des cases de
        /// couleurs différentes) ; aucun pion sur la première ou la dernière rangée ; le camp qui ne joue pas n'est pas en échec.
        /// </summary>
        public bool IsPlausible(out string why)
        {
            why = null;
            foreach (int side in new[] { 1, -1 })
            {
                var counts = new int[7];
                var bishopColors = new int[2];
                for (int sq = 0; sq < 64; sq++)
                {
                    int piece = Board[sq] * side;
                    if (piece <= 0)
                        continue;
                    counts[piece]++;
                    if (piece == Bishop)
                        bishopColors[(File(sq) + Rank(sq)) & 1]++;
                    if (piece == Pawn && (Rank(sq) == 0 || Rank(sq) == 7))
                        why = "pion sur la première ou la dernière rangée";
                }
                if (counts[King] != 1)
                    why = "rois";
                else if (counts[Pawn] > 8 || counts[Queen] > 1 || counts[Rook] > 2 || counts[Knight] > 2 || counts[Bishop] > 2)
                    why = "plus de pièces qu'au départ";
                else if (bishopColors[0] > 1 || bishopColors[1] > 1)
                    why = "deux fous de la même couleur";
                if (why != null)
                    return false;
            }
            int wk = KingSquare(1), bk = KingSquare(-1);
            if (Math.Abs(File(wk) - File(bk)) <= 1 && Math.Abs(Rank(wk) - Rank(bk)) <= 1)
                why = "rois côte à côte";
            else if (InCheck(-Side))
                why = "le camp qui ne joue pas est en échec";
            return why == null;
        }

        public bool IsCheckmate() => InCheck(Side) && LegalMoves().Count == 0;

        public bool IsStalemate() => !InCheck(Side) && LegalMoves().Count == 0;

        /// <summary>Coups qui matent tout de suite.</summary>
        public List<ChessMove> MatesInOne()
        {
            var result = new List<ChessMove>();
            foreach (var m in LegalMoves())
                if (Play(m).IsCheckmate())
                    result.Add(m);
            return result;
        }

        /// <summary>Le coup force-t-il le mat au coup suivant, quelle que soit la réponse ?</summary>
        public bool ForcesMateInTwo(ChessMove m)
        {
            var after = Play(m);
            var replies = after.LegalMoves();
            if (replies.Count == 0)
                return false;
            foreach (var r in replies)
                if (after.Play(r).MatesInOne().Count == 0)
                    return false;
            return true;
        }

        /// <summary>Coups clés d'un mat en deux (sans compter les mats immédiats).</summary>
        public List<ChessMove> MatesInTwo()
        {
            var result = new List<ChessMove>();
            foreach (var m in LegalMoves())
                if (!Play(m).IsCheckmate() && ForcesMateInTwo(m))
                    result.Add(m);
            return result;
        }

        /// <summary>Position vue de l'autre camp : couleurs échangées et échiquier retourné (même problème pour les Noirs).</summary>
        public ChessPosition ColorFlipped()
        {
            var p = new ChessPosition { Side = -Side };
            for (int sq = 0; sq < 64; sq++)
                p.Board[(7 - Rank(sq)) * 8 + File(sq)] = -Board[sq];
            p.Castling = (Castling & 3) << 2 | (Castling & 12) >> 2;
            p.EnPassant = EnPassant < 0 ? -1 : (7 - Rank(EnPassant)) * 8 + File(EnPassant);
            return p;
        }
    }
}
