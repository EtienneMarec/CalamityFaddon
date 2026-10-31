using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Debug
{
    /// <summary>
    /// Objet de test (sans recette, à prendre via Cheat Sheet) : clic gauche, passe au lever du jour (4 h 30) ou à la
    /// tombée de la nuit (19 h 30) ; clic droit, avance d'une heure (le jeu gère le passage jour/nuit). Sert aux tests
    /// qui dépendent de l'heure (Empress of Light apprivoisée de l'aube au crépuscule, Skeletron la nuit...).
    /// [technique] L'heure se règle là où elle fait foi (solo ou serveur) ; le serveur la renvoie aux clients.
    /// </summary>
    public class FandomTimeDial : ModItem
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.Sundial}";

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useAnimation = Item.useTime = 20;
            Item.UseSound = SoundID.Item4;
            Item.rare = ItemRarityID.Quest;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool? UseItem(Player player)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return true;
            if (player.altFunctionUse == 2)
                Main.time += 3600;
            else
            {
                Main.dayTime = !Main.dayTime;
                Main.time = 0;
            }
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);
            else
                Main.NewText(Language.GetTextValue("Mods.CalamityFandom.Items.FandomTimeDial.Now", Hour()), Color.LightSkyBlue);
            return true;
        }

        /// <summary>Heure affichée comme dans le jeu (le jour commence à 4 h 30, la nuit à 19 h 30).</summary>
        private static string Hour()
        {
            double hours = Main.time / 3600.0 + (Main.dayTime ? 4.5 : 19.5);
            hours %= 24;
            return $"{(int)hours}:{(int)(hours % 1 * 60):00}";
        }
    }
}
