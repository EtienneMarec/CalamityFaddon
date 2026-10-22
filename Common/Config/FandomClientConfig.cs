using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace CalamityFandom.Common.Config
{
    /// <summary>Réglages propres à chaque joueur (affichage). Les positions sont en % de l'écran, comme les jauges de Calamity.</summary>
    public class FandomClientConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        public static FandomClientConfig Instance;

        /// <summary>Par défaut juste sous la jauge d'adrénaline de Calamity (même écart que rage → adrénaline).</summary>
        public const float DefaultShortstrikePosX = 35.77406f;
        public const float DefaultShortstrikePosY = 13.117703f;

        [Header("Meters")]
        [Range(0f, 100f)]
        [DefaultValue(DefaultShortstrikePosX)]
        public float ShortstrikeMeterPosX { get; set; } = DefaultShortstrikePosX;

        [Range(0f, 100f)]
        [DefaultValue(DefaultShortstrikePosY)]
        public float ShortstrikeMeterPosY { get; set; } = DefaultShortstrikePosY;
    }
}
