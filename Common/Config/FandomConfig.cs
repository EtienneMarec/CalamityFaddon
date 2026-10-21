using System;
using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace CalamityFandom.Common.Config
{
    /// <summary>Niveau de fidélité au wiki Fandom. Voir docs/MODES.md.</summary>
    public enum FandomAccuracy
    {
        /// <summary>Uniquement ce qui est écrit sur le wiki : nouveautés de Calamity d'après 2.0.1.005 masquées, valeurs du wiki sans garde-fou.</summary>
        FullFandom,
        /// <summary>Calamity actuel + ajouts du wiki + correctifs quand le wiki est impossible ou muet.</summary>
        Minimal,
        /// <summary>Minimal + vrai travail d'équilibrage pour que tout soit jouable.</summary>
        CalamityPlus,
    }

    public class FandomConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        // Tout se décide au chargement (noms, recettes, existence des objets) : changer de mode recharge le mod
        [ReloadRequired]
        [DefaultValue(FandomAccuracy.Minimal)]
        public FandomAccuracy Accuracy { get; set; } = FandomAccuracy.Minimal;

        /// <summary>Un monde se souvient du mode avec lequel il a été créé.</summary>
        [DefaultValue(true)]
        public bool WarnOnWorldModeMismatch { get; set; } = true;
    }

    /// <summary>Mode actif pour toute la session. CALFANDOM_MODE (FullFandom/Minimal/CalamityPlus) le force, pour l'auto-test.</summary>
    public static class FandomMode
    {
        public static FandomAccuracy Current
        {
            get
            {
                string forced = Environment.GetEnvironmentVariable("CALFANDOM_MODE");
                if (!string.IsNullOrEmpty(forced) && Enum.TryParse<FandomAccuracy>(forced, true, out var mode))
                    return mode;
                return ModContent.GetInstance<FandomConfig>()?.Accuracy ?? FandomAccuracy.Minimal;
            }
        }

        public static bool IsFull => Current == FandomAccuracy.FullFandom;
        public static bool IsPlus => Current == FandomAccuracy.CalamityPlus;
    }
}
