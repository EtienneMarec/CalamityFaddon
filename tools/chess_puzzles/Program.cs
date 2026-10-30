using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using CalamityFandom.Common.Taming.Chess;

// Génère les problèmes d'échecs du Holographic Chessboard (Taming du Brain of Cthulhu et de XP-00 Hypnos) :
//   dotnet run --project tools/chess_puzzles -- [graine]
// Positions tirées au hasard (roi en défense sur ses deux dernières rangées, pions devant lui) puis gardées si :
//   mate1 : un seul coup mate ;
//   mate2 : pas de mat en un, un seul coup clé, silencieux (pas un échec), au moins deux réponses possibles ;
//   pas de sous-promotion (l'interface promeut en dame), pas plus de pièces de chaque sorte qu'au départ.
// La moitié des problèmes est retournée pour que les Noirs jouent. Le jeu revérifie tout avec le même moteur.
if (args.Length > 0 && args[0] == "perft")
{
    // Valeurs de référence (chessprogramming.org) : position initiale, « Kiwipete », position 3
    (string fen, int depth, long nodes)[] cases =
    {
        ("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 4, 197281),
        ("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1", 3, 97862),
        ("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1", 5, 674624),
    };
    foreach (var (fen, depth, nodes) in cases)
    {
        long got = Perft(ChessPosition.FromFen(fen), depth);
        Console.WriteLine($"{(got == nodes ? "OK " : "FAUX")} perft({depth}) = {got} (attendu {nodes}) {fen}");
    }
    return;
}

static long Perft(ChessPosition pos, int depth)
{
    if (depth == 0)
        return 1;
    long n = 0;
    foreach (var m in pos.LegalMoves())
        n += Perft(pos.Play(m), depth - 1);
    return n;
}

const int PerPool = 45;
int seed = args.Length > 0 ? int.Parse(args[0]) : 2026;
var rng = new Random(seed);
string root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
string output = Path.Combine(root, "Common", "Taming", "Chess", "puzzles.json");

var mate1 = new List<Dictionary<string, object>>();
var mate2 = new List<Dictionary<string, object>>();
var seen = new HashSet<string>();
long tries = 0;
while (mate1.Count < PerPool || mate2.Count < PerPool)
{
    tries++;
    var pos = Random(rng, mate2.Count < PerPool && (mate1.Count >= PerPool || rng.Next(3) > 0));
    if (pos == null)
        continue;
    var moves = pos.LegalMoves();
    if (moves.Count < 12)
        continue;
    var ones = pos.MatesInOne();
    if (ones.Count == 1 && mate1.Count < PerPool && Add(mate1, pos, ones[0]))
        continue;
    if (ones.Count > 0 || mate2.Count >= PerPool)
        continue;
    var keys = new List<ChessMove>();
    foreach (var m in moves)
    {
        if (!pos.ForcesMateInTwo(m))
            continue;
        keys.Add(m);
        if (keys.Count > 1)
            break;
    }
    // Un seul coup gagnant (échecs compris), et c'est un coup silencieux qui laisse au moins deux réponses
    if (keys.Count == 1)
    {
        var after = pos.Play(keys[0]);
        if (!after.InCheck(after.Side) && after.LegalMoves().Count >= 2)
            Add(mate2, pos, keys[0]);
    }
}

bool Add(List<Dictionary<string, object>> pool, ChessPosition pos, ChessMove solution)
{
    // L'interface promeut toujours en dame
    if (solution.Promotion != 0 && solution.Promotion != ChessPosition.Queen)
        return false;
    bool flip = pool.Count % 2 == 1;
    var p = flip ? pos.ColorFlipped() : pos;
    string fen = p.ToFen();
    if (!seen.Add(fen))
        return false;
    var s = flip ? new ChessMove(Flip(solution.From), Flip(solution.To), solution.Promotion) : solution;
    pool.Add(new Dictionary<string, object> { ["fen"] = fen, ["solution"] = s.Uci });
    Console.WriteLine($"{(pool == mate1 ? "mate1" : "mate2")} {pool.Count,2} {fen}  {s.Uci}");
    return true;
}

static int Flip(int sq) => (7 - ChessPosition.Rank(sq)) * 8 + ChessPosition.File(sq);

// Blancs : roi + 2 à 5 pièces (pas plus qu'au départ) ; Noirs : roi sur les rangées 7-8, pions devant lui, 1 à 3 pièces.
static ChessPosition Random(Random rng, bool rich)
{
    var p = new ChessPosition { Side = 1 };
    int[] board = p.Board;
    int Free(int minRank, int maxRank)
    {
        for (int i = 0; i < 50; i++)
        {
            int sq = rng.Next(minRank, maxRank + 1) * 8 + rng.Next(8);
            if (board[sq] == 0)
                return sq;
        }
        return -1;
    }
    int bk = Free(6, 7);
    board[bk] = -ChessPosition.King;
    int wk = Free(0, 3);
    if (wk < 0 || Math.Abs(ChessPosition.File(wk) - ChessPosition.File(bk)) <= 1 && Math.Abs(ChessPosition.Rank(wk) - ChessPosition.Rank(bk)) <= 1)
        return null;
    board[wk] = ChessPosition.King;
    // Abri du roi noir : 1 à 3 pions devant lui
    int shelter = rng.Next(1, 4);
    for (int i = 0; i < shelter; i++)
    {
        int sq = (ChessPosition.Rank(bk) - 1) * 8 + Math.Clamp(ChessPosition.File(bk) + rng.Next(-1, 2), 0, 7);
        if (ChessPosition.Rank(sq) >= 1 && rng.Next(3) == 0 && ChessPosition.Rank(sq) > 2)
            sq -= 8;
        if (ChessPosition.Rank(sq) >= 1 && ChessPosition.Rank(sq) <= 6 && board[sq] == 0)
            board[sq] = -ChessPosition.Pawn;
    }
    int[] whiteSet = rich ? new[] { 5, 4, 4, 3, 3, 2, 2, 1, 1 } : new[] { 5, 4, 4, 3, 2, 1 };
    int whiteCount = rng.Next(2, rich ? 6 : 5);
    for (int i = 0; i < whiteCount; i++)
    {
        int type = whiteSet[rng.Next(whiteSet.Length)];
        // Pas plus de pièces qu'au départ (une dame, deux tours/fous/cavaliers) : sinon on prend un pion
        if (board.Count(x => x == type) >= (type == ChessPosition.Queen ? 1 : 2))
            type = ChessPosition.Pawn;
        int sq = type == ChessPosition.Pawn ? Free(1, 6) : Free(0, 7);
        if (sq < 0)
            return null;
        board[sq] = type;
    }
    int[] blackSet = { 4, 3, 2, 1, 1, 5 };
    int blackCount = rng.Next(1, 4);
    for (int i = 0; i < blackCount; i++)
    {
        int type = blackSet[rng.Next(blackSet.Length)];
        if (board.Count(x => x == -type) >= (type == ChessPosition.Queen ? 1 : 2))
            type = ChessPosition.Pawn;
        int sq = type == ChessPosition.Pawn ? Free(1, 6) : Free(2, 7);
        if (sq < 0)
            return null;
        board[sq] = -type;
    }
    // Position légale : les Noirs (qui ne jouent pas) ne sont pas en échec, les Blancs ont du matériel d'attaque
    if (p.InCheck(-1) || p.InCheck(1))
        return null;
    return p;
}

var json = JsonSerializer.Serialize(new { seed, mate1, mate2 }, new JsonSerializerOptions { WriteIndented = true });
File.WriteAllText(output, json + "\n");
Console.WriteLine($"{mate1.Count} + {mate2.Count} problèmes ({tries} positions essayées) -> {output}");
