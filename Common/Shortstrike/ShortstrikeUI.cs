using System;
using System.Collections.Generic;
using CalamityFandom.Common.Config;
using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace CalamityFandom.Common.Shortstrike
{
    /// <summary>
    /// Jauge de Shortstrike, dessinée par l'utilisateur (Assets/UI/ShortstrikeMeter.png, 50 images de 77×34 :
    /// 0-36 remplissage, 37-49 les épées sortent quand elle devient pleine). Affichée comme la rage et l'adrénaline de
    /// Calamity : position en % de l'écran (sous l'adrénaline par défaut), déplaçable à la souris sauf si Calamity
    /// verrouille ses jauges (« MeterPosLock »), texte au survol. Visible en tenant une épée courte ([choix]).
    /// </summary>
    public class ShortstrikeUI : ModSystem
    {
        private const int Frames = 50;
        private const int LastFillFrame = 36;
        private const int FirstFullFrame = 37;
        private const int TicksPerFullFrame = 4;
        private const float DragEpsilon = 0.05f;
        /// <summary>Dessin à 2× (pixel art agrandi comme les jauges de Calamity ; retour de test du 22/10).</summary>
        private const float Scale = 2f;

        private static Asset<Texture2D> meter;
        private static Vector2? dragOffset;
        private static int fullTimer;

        public override void Load()
        {
            if (!Main.dedServ)
                meter = ModContent.Request<Texture2D>("CalamityFandom/Assets/UI/ShortstrikeMeter");
        }

        public override void Unload() => meter = null;

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(l => l.Name == "Vanilla: Resource Bars");
            if (index < 0)
                return;
            layers.Insert(index, new LegacyGameInterfaceLayer("CalamityFandom: Shortstrike", Draw, InterfaceScaleType.UI));
        }

        private static bool Draw()
        {
            Player player = Main.LocalPlayer;
            var config = FandomClientConfig.Instance;
            if (player.dead || meter == null || config == null || !ShortstrikeData.IsShortsword(player.HeldItem))
            {
                dragOffset = null;
                return true;
            }
            var modPlayer = player.GetModPlayer<ShortstrikePlayer>();
            Texture2D texture = meter.Value;
            int frameHeight = texture.Height / Frames;

            // Image : remplissage, puis sortie des épées une fois pleine (et on garde la dernière) ; pendant un Shortflurry,
            // la jauge se vide sur la durée de l'attaque en tremblant, comme la rage et l'adrénaline de Calamity
            int frame;
            Vector2 shake = Vector2.Zero;
            if (modPlayer.FlurryActive)
            {
                fullTimer = 0;
                frame = (int)Math.Round(modPlayer.FlurryRemaining * LastFillFrame);
                float amount = CalamityClientConfig.Instance?.RipperMeterShake ?? 2f;
                shake = new Vector2(Main.rand.NextFloat(-amount, amount), Main.rand.NextFloat(-amount, amount));
            }
            else if (modPlayer.Full)
            {
                fullTimer++;
                frame = Math.Min(Frames - 1, FirstFullFrame + fullTimer / TicksPerFullFrame);
            }
            else
            {
                fullTimer = 0;
                frame = (int)Math.Round(modPlayer.Points / (float)ShortstrikeData.Current.MaxPoints * LastFillFrame);
            }

            Vector2 ratio = new(config.ShortstrikeMeterPosX, config.ShortstrikeMeterPosY);
            if (ratio.X is < 0f or > 100f)
                ratio.X = FandomClientConfig.DefaultShortstrikePosX;
            if (ratio.Y is < 0f or > 100f)
                ratio.Y = FandomClientConfig.DefaultShortstrikePosY;
            Vector2 screenPos = new((int)(ratio.X * 0.01f * Main.screenWidth), (int)(ratio.Y * 0.01f * Main.screenHeight));

            float pulse = modPlayer.Full && frame == Frames - 1 ? 0.85f + 0.15f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 6f) : 1f;
            var source = new Rectangle(0, frame * frameHeight, texture.Width, frameHeight);
            Main.spriteBatch.Draw(texture, screenPos + shake, source, Color.White * pulse, 0f, source.Size() / 2f, Scale, SpriteEffects.None, 0f);

            // Survol : texte ; glisser pour déplacer (comme les jauges de Calamity)
            Rectangle area = Utils.CenteredRectangle(screenPos, new Vector2(texture.Width, frameHeight) * Scale);
            bool locked = CalamityClientConfig.Instance?.MeterPosLock ?? true;
            if (!area.Contains(Main.MouseScreen.ToPoint()) && !dragOffset.HasValue)
                return true;
            if (!locked)
                player.mouseInterface = true;
            Main.instance.MouseText(Language.GetTextValue("Mods.CalamityFandom.Shortstrike.Gauge", modPlayer.Points, ShortstrikeData.Current.MaxPoints));
            if (locked)
                return true;
            MouseState mouse = Mouse.GetState();
            if (mouse.LeftButton == ButtonState.Pressed)
            {
                dragOffset ??= Main.MouseScreen - screenPos;
                Vector2 corner = Main.MouseScreen - dragOffset.Value;
                Vector2 newRatio = new(100f * corner.X / Main.screenWidth, 100f * corner.Y / Main.screenHeight);
                if (Math.Abs(newRatio.X - ratio.X) >= DragEpsilon || Math.Abs(newRatio.Y - ratio.Y) >= DragEpsilon)
                {
                    config.ShortstrikeMeterPosX = newRatio.X;
                    config.ShortstrikeMeterPosY = newRatio.Y;
                }
            }
            else if (dragOffset.HasValue)
            {
                dragOffset = null;
                config.SaveChanges();
            }
            return true;
        }
    }
}
