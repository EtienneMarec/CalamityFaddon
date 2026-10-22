using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace CalamityFandom.Common.Shortstrike
{
    /// <summary>
    /// Jauge de Shortstrike sous le joueur, seulement en tenant une épée courte (le wiki parle du « Shortstrike counter »
    /// sans dire comment il s'affiche : [choix]). Orange et pulsante quand elle est pleine ; survol = texte d'aide.
    /// </summary>
    public class ShortstrikeUI : ModSystem
    {
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(l => l.Name == "Vanilla: Resource Bars");
            if (index < 0)
                return;
            layers.Insert(index, new LegacyGameInterfaceLayer("CalamityFandom: Shortstrike", Draw, InterfaceScaleType.Game));
        }

        private static bool Draw()
        {
            Player player = Main.LocalPlayer;
            if (player.dead || !ShortstrikeData.IsShortsword(player.HeldItem))
                return true;
            var modPlayer = player.GetModPlayer<ShortstrikePlayer>();
            float fill = modPlayer.Points / (float)ShortstrikeData.Current.MaxPoints;
            const int Width = 48, Height = 6;
            Vector2 position = player.Bottom - Main.screenPosition + new Vector2(-Width / 2f, 10f + player.gfxOffY);
            var background = new Rectangle((int)position.X - 1, (int)position.Y - 1, Width + 2, Height + 2);
            var bar = new Rectangle((int)position.X, (int)position.Y, (int)(Width * fill), Height);
            float pulse = modPlayer.Full ? 0.75f + 0.25f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 6f) : 1f;
            Color color = modPlayer.Full ? new Color(255, 150, 40) * pulse : new Color(230, 200, 120);
            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, background, Color.Black * 0.6f);
            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, bar, color);
            if (background.Contains(Main.MouseScreen.ToPoint()))
                Main.instance.MouseText(Language.GetTextValue("Mods.CalamityFandom.Shortstrike.Gauge", modPlayer.Points, ShortstrikeData.Current.MaxPoints));
            return true;
        }
    }
}
