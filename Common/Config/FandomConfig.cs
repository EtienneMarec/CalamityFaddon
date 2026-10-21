using System;
using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace CalamityFandom.Common.Config
{
    /// <summary>Mode effectif, en interne. Voir docs/MODES.md.</summary>
    public enum FandomAccuracy
    {
        /// <summary>Uniquement ce qui est écrit sur le wiki : nouveautés de Calamity d'après 2.0.1.005 masquées, valeurs de 2022 comprises.</summary>
        FullFandom,
        /// <summary>Couche interne (debug) : Calamity actuel + ce qui est propre au Fandom, sans l'équilibrage de Calamity++.</summary>
        Minimal,
        /// <summary>Calamity actuel + ce qui est propre au Fandom + équilibrage (Balance/).</summary>
        CalamityPlus,
    }

    /// <summary>Les deux modes proposés aux joueurs.</summary>
    public enum PlayerAccuracy
    {
        FullFandom,
        CalamityPlus,
    }

    public class FandomConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        // Tout se décide au chargement (noms, recettes, existence des objets) : changer de mode recharge le mod
        [ReloadRequired]
        [DefaultValue(PlayerAccuracy.CalamityPlus)]
        public PlayerAccuracy Accuracy { get; set; } = PlayerAccuracy.CalamityPlus;

        /// <summary>Un monde se souvient du mode avec lequel il a été créé.</summary>
        [DefaultValue(true)]
        public bool WarnOnWorldModeMismatch { get; set; } = true;

        /// <summary>Debug : Calamity++ sans sa couche d'équilibrage (= mode interne Minimal), pour comparer.</summary>
        [Header("Debug")]
        [ReloadRequired]
        [DefaultValue(false)]
        public bool DisableBalanceLayer { get; set; }
    }

    /// <summary>
    /// Mode actif pour toute la session. CALFANDOM_MODE (FullFandom/Minimal/CalamityPlus) le force, pour l'auto-test.
    /// </summary>
    public static class FandomMode
    {
        public static FandomAccuracy Current
        {
            get
            {
                string forced = Environment.GetEnvironmentVariable("CALFANDOM_MODE");
                if (!string.IsNullOrEmpty(forced) && Enum.TryParse<FandomAccuracy>(forced, true, out var mode))
                    return mode;
                var config = ModContent.GetInstance<FandomConfig>();
                if (config == null)
                    return FandomAccuracy.CalamityPlus;
                if (config.Accuracy == PlayerAccuracy.FullFandom)
                    return FandomAccuracy.FullFandom;
                return config.DisableBalanceLayer ? FandomAccuracy.Minimal : FandomAccuracy.CalamityPlus;
            }
        }

        public static bool IsFull => Current == FandomAccuracy.FullFandom;
        public static bool IsPlus => Current == FandomAccuracy.CalamityPlus;

        /// <summary>Vue du wiki à lire : « fandom » (1re valeur de {{mode}}) en Full Fandom, « plus » sinon.</summary>
        public static string WikiView => IsFull ? "fandom" : "plus";

        /// <summary>Une entrée de données marquée « modes » (générée depuis un {{mode}}) vaut-elle pour le mode courant ?</summary>
        public static bool Applies(System.Collections.Generic.ICollection<string> modes) =>
            modes == null || modes.Count == 0 || modes.Contains(WikiView);
    }
}
