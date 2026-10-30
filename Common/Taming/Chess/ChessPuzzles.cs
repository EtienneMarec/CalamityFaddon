using System;
using System.Collections.Generic;
using System.Text.Json;
using Terraria;
using Terraria.ModLoader;

namespace CalamityFandom.Common.Taming.Chess
{
    /// <summary>
    /// Problèmes du Holographic Chessboard (Common/Taming/Chess/puzzles.json, généré par tools/chess_puzzles) :
    /// mats en un pour le Brain of Cthulhu, mats en deux pour XP-00 Hypnos. Le jeu revérifie chaque coup avec le moteur.
    /// </summary>
    public class ChessPuzzles : ModSystem
    {
        public record Puzzle(string Fen, string Solution);

        private class DataFile
        {
            public List<Puzzle> Mate1 { get; set; } = new();
            public List<Puzzle> Mate2 { get; set; } = new();
        }

        public static List<Puzzle> Mate1 { get; private set; } = new();
        public static List<Puzzle> Mate2 { get; private set; } = new();

        public override void Load()
        {
            var data = JsonSerializer.Deserialize<DataFile>(Mod.GetFileBytes("Common/Taming/Chess/puzzles.json"),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Mate1 = data.Mate1;
            Mate2 = data.Mate2;
        }

        public override void Unload()
        {
            Mate1 = new();
            Mate2 = new();
        }

        /// <summary>Tire un problème au hasard, en évitant ceux déjà vus pendant la séance.</summary>
        public static int Pick(List<Puzzle> pool, HashSet<int> used)
        {
            if (used.Count >= pool.Count)
                used.Clear();
            int index;
            do
                index = Main.rand.Next(pool.Count);
            while (used.Contains(index));
            used.Add(index);
            return index;
        }
    }
}
