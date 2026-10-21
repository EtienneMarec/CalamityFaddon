using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityFandom.Common.Config
{
    /// <summary>
    /// Mémorise dans le monde le mode avec lequel il a été créé, et prévient si on le charge dans un autre mode
    /// (le contenu se fixe au chargement du mod : impossible de basculer automatiquement).
    /// </summary>
    public class WorldModeLock : ModSystem
    {
        public static FandomAccuracy? WorldMode { get; private set; }

        public override void OnWorldLoad() => WorldMode = null;
        public override void OnWorldUnload() => WorldMode = null;

        public override void SaveWorldData(TagCompound tag) => tag["mode"] = (int)(WorldMode ?? FandomMode.Current);

        public override void LoadWorldData(TagCompound tag)
        {
            if (tag.ContainsKey("mode"))
                WorldMode = (FandomAccuracy)tag.GetInt("mode");
        }

        public override void PostWorldGen() => WorldMode = FandomMode.Current;

        // Seul le passage Full Fandom <-> le reste change le contenu (objets masqués) ; l'équilibrage ne change que des nombres
        public static bool Mismatch => WorldMode is { } mode && (mode == FandomAccuracy.FullFandom) != FandomMode.IsFull;
    }

    /// <summary>Avertissement côté joueur à l'entrée dans un monde créé avec un autre mode.</summary>
    public class WorldModeLockPlayer : ModPlayer
    {
        public override void OnEnterWorld()
        {
            if (!WorldModeLock.Mismatch || !ModContent.GetInstance<FandomConfig>().WarnOnWorldModeMismatch)
                return;
            string world = Language.GetTextValue($"Mods.CalamityFandom.Modes.{WorldModeLock.WorldMode}");
            string current = Language.GetTextValue($"Mods.CalamityFandom.Modes.{FandomMode.Current}");
            Main.NewText(Language.GetTextValue("Mods.CalamityFandom.Modes.Mismatch", world, current), Color.OrangeRed);
        }
    }
}
