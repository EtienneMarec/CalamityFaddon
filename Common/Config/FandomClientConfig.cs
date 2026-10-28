using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace CalamityFandom.Common.Config
{
    /// <summary>Réglages propres à chaque joueur (affichage). Les positions sont en % de l'écran, comme les jauges de Calamity.</summary>
    public class FandomClientConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        public static FandomClientConfig Instance;

        /// <summary>
        /// Par défaut à droite de la jauge de vol de Calamity (40,94 % / 7,22 %, 34×70 px), à la même hauteur : sous
        /// l'adrénaline, elle passait sur le texte des PNJ (retour de test du 22/10).
        /// </summary>
        public const float DefaultShortstrikePosX = 46.25f;
        public const float DefaultShortstrikePosY = 7.2222223f;

        /// <summary>Première position par défaut (sous l'adrénaline) : remplacée si le joueur ne l'avait pas changée.</summary>
        private const float OldDefaultPosX = 35.77406f, OldDefaultPosY = 13.117703f;

        [Header("Meters")]
        [Range(0f, 100f)]
        [DefaultValue(DefaultShortstrikePosX)]
        public float ShortstrikeMeterPosX { get; set; } = DefaultShortstrikePosX;

        [Range(0f, 100f)]
        [DefaultValue(DefaultShortstrikePosY)]
        public float ShortstrikeMeterPosY { get; set; } = DefaultShortstrikePosY;

        /// <summary>Son joué au début d'un Shortflurry : choix à l'essai en jeu (retour de test du 28/10, [choix]).</summary>
        public enum FlurrySound
        {
            ShortswordStab,
            BrimflameAbility,
            OmegaBlueAbility,
            PlagueReaperAbility,
            SilvaActivation,
            SonicBoomBlade,
        }

        [DefaultValue(FlurrySound.BrimflameAbility)]
        [DrawTicks]
        public FlurrySound ShortflurrySound { get; set; } = FlurrySound.BrimflameAbility;

        public override void OnLoaded()
        {
            Instance = this;
            if (System.Math.Abs(ShortstrikeMeterPosX - OldDefaultPosX) < 0.001f && System.Math.Abs(ShortstrikeMeterPosY - OldDefaultPosY) < 0.001f)
            {
                ShortstrikeMeterPosX = DefaultShortstrikePosX;
                ShortstrikeMeterPosY = DefaultShortstrikePosY;
            }
        }
    }
}
