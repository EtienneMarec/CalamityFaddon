using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace CalamityFandom.Common.Taming
{
    /// <summary>
    /// Skeletron (page Taming : « Arm Wrestle the Old Man. The 'Arm Wrestle' option can be accessed by going down 3 options,
    /// permanently revealing it »). [choix] Le bouton « Arm Wrestle » apparaît après 3 conversations avec l'Old Man, pour
    /// toujours (par personnage). Gagner le bras de fer fait apparaître Skeletron, apprivoisé aussitôt (comme la
    /// malédiction, mais sans combat : donjon ouvert, Clothier).
    /// </summary>
    public class OldManWrestle : GlobalNPC
    {
        public const string Key = "Skeletron";
        public const int TalksToReveal = 3;

        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.OldMan;

        private static bool Available()
        {
            var e = TamingSystem.Get(Key);
            return e != null && !TamingSystem.Untamable.Contains(Key) && TamingSystem.FindMain(e) == null
                && Main.LocalPlayer.GetModPlayer<WrestlePlayer>().Talks >= TalksToReveal;
        }

        /// <summary>Second bouton de l'Old Man (TamingChatButtons).</summary>
        public static string SecondButton() => Available() ? Language.GetTextValue("Mods.CalamityFandom.Taming.Wrestle.Button") : null;

        public override void GetChat(NPC npc, ref string chat)
        {
            var player = Main.LocalPlayer.GetModPlayer<WrestlePlayer>();
            if (player.Talks < TalksToReveal && ++player.Talks == TalksToReveal && Available())
                chat = Language.GetTextValue("Mods.CalamityFandom.Taming.Wrestle.Reveal");
        }

        public override bool PreChatButtonClicked(NPC npc, bool firstButton)
        {
            if (firstButton || !Available())
                return true;
            Main.LocalPlayer.SetTalkNPC(-1);
            Main.npcChatText = "";
            WrestleUISystem.Open();
            return false;
        }
    }

    /// <summary>Conversations avec l'Old Man (le bouton se révèle à la troisième), gardées par personnage.</summary>
    public class WrestlePlayer : ModPlayer
    {
        public int Talks;

        public override void SaveData(TagCompound tag) => tag["oldManTalks"] = Talks;

        public override void LoadData(TagCompound tag) => Talks = tag.GetInt("oldManTalks");
    }

    /// <summary>
    /// Bras de fer (idée de l'utilisateur, validée le 31/10) : un cran fait des allers-retours sur une barre ; cliquer (ou
    /// Espace) quand il est dans la zone fait avancer le bras de fer de 5, de 10 dans la zone ×2 (deux fois plus étroite,
    /// au milieu). L'Old Man tire de 1 toutes les demi-secondes. Gagné à +50, perdu à -50.
    /// [choix] Après un coup, 1/3 s sans pouvoir recliquer (sinon cliquer en continu gagne à coup sûr) ; un coup raté ne
    /// coûte rien d'autre ; après chaque coup réussi, la zone change de place et le cran accélère de 5 % (jusqu'à ×2).
    /// </summary>
    public class WrestleSession
    {
        public const int Goal = 50, OldManPull = 1, PullTicks = 30, Hit = 5, DoubleHit = 10, Cooldown = 20;
        public const float ZoneHalf = 0.1f, BaseSpeed = 1f / 60f;

        public int Meter;
        public float Cursor, Speed = BaseSpeed, Zone = 0.5f;
        private int direction = 1, pullTimer, cooldown;
        public int Flash;
        public Color FlashColor;

        /// <summary>1 gagné, -1 perdu, 0 en cours.</summary>
        public int Result => Meter >= Goal ? 1 : Meter <= -Goal ? -1 : 0;

        public void Update(bool click)
        {
            Cursor += Speed * direction;
            if (Cursor > 1f || Cursor < 0f)
            {
                direction = -direction;
                Cursor = Math.Clamp(Cursor, 0f, 1f);
            }
            if (++pullTimer >= PullTicks)
            {
                pullTimer = 0;
                Meter -= OldManPull;
            }
            if (Flash > 0)
                Flash--;
            if (cooldown > 0)
            {
                cooldown--;
                return;
            }
            if (!click)
                return;
            cooldown = Cooldown;
            float distance = Math.Abs(Cursor - Zone);
            int gain = distance <= ZoneHalf / 2f ? DoubleHit : distance <= ZoneHalf ? Hit : 0;
            if (gain == 0)
            {
                Flash = 15;
                FlashColor = Color.Gray;
                return;
            }
            Meter += gain;
            Flash = 15;
            FlashColor = gain == DoubleHit ? Color.Orange : Color.Yellow;
            SoundEngine.PlaySound(gain == DoubleHit ? SoundID.Item37 : SoundID.Item1);
            Zone = Main.rand.NextFloat(ZoneHalf, 1f - ZoneHalf);
            Speed = Math.Min(Speed * 1.05f, BaseSpeed * 2f);
        }
    }

    [Autoload(Side = ModSide.Client)]
    public class WrestleUISystem : ModSystem
    {
        public static WrestleSession Session { get; private set; }
        private static int endTimer;
        private const int PanelWidth = 440, PanelHeight = 150;

        public static void Open()
        {
            Session = new WrestleSession();
            endTimer = 0;
        }

        public override void OnWorldUnload() => Session = null;

        public override void UpdateUI(GameTime gameTime)
        {
            var s = Session;
            if (s == null)
                return;
            if (Main.LocalPlayer.dead || Main.keyState.IsKeyDown(Keys.Escape))
            {
                Session = null;
                return;
            }
            Main.LocalPlayer.mouseInterface = true;
            if (endTimer > 0)
            {
                if (--endTimer == 0)
                    Session = null;
                return;
            }
            bool click = Main.mouseLeft && Main.mouseLeftRelease
                || Main.keyState.IsKeyDown(Keys.Space) && !Main.oldKeyState.IsKeyDown(Keys.Space);
            s.Update(click);
            if (s.Result == 0)
                return;
            endTimer = 90;
            if (s.Result > 0)
            {
                SoundEngine.PlaySound(SoundID.Item4);
                TamingSystem.ReportArmWrestleWon();
            }
            else
                SoundEngine.PlaySound(SoundID.NPCHit2);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(l => l.Name == "Vanilla: Mouse Text");
            if (index < 0 || Session == null)
                return;
            layers.Insert(index, new LegacyGameInterfaceLayer("CalamityFandom: Arm Wrestle", () =>
            {
                Draw(Main.spriteBatch, Session);
                return true;
            }, InterfaceScaleType.UI));
        }

        private static void Draw(SpriteBatch sb, WrestleSession s)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            var panel = new Rectangle(Main.screenWidth / 2 - PanelWidth / 2, Main.screenHeight / 2 - PanelHeight / 2, PanelWidth, PanelHeight);
            sb.Draw(pixel, panel, new Color(40, 25, 15, 230));
            string key = "Mods.CalamityFandom.Taming.Wrestle.";
            string title = s.Result > 0 ? Language.GetTextValue(key + "Won") : s.Result < 0 ? Language.GetTextValue(key + "Lost")
                : Language.GetTextValue(key + "Title");
            Utils.DrawBorderString(sb, title, panel.TopLeft() + new Vector2(10, 6), s.Result < 0 ? Color.OrangeRed : Color.Wheat);

            // Bras de fer : à gauche l'Old Man, à droite le joueur
            var tug = new Rectangle(panel.X + 20, panel.Y + 40, PanelWidth - 40, 18);
            sb.Draw(pixel, tug, new Color(90, 70, 60));
            float ratio = (s.Meter + WrestleSession.Goal) / (2f * WrestleSession.Goal);
            sb.Draw(pixel, new Rectangle(tug.X, tug.Y, (int)(tug.Width * Math.Clamp(ratio, 0f, 1f)), tug.Height), new Color(110, 190, 90));
            sb.Draw(pixel, new Rectangle(tug.Center.X - 1, tug.Y - 3, 2, tug.Height + 6), Color.White);
            Utils.DrawBorderString(sb, Language.GetTextValue(key + "OldMan"), new Vector2(tug.X, tug.Bottom + 2), Color.LightGray, 0.75f);
            string you = Language.GetTextValue(key + "You");
            Utils.DrawBorderString(sb, you, new Vector2(tug.Right - FontAssets.MouseText.Value.MeasureString(you).X * 0.75f, tug.Bottom + 2), Color.LightGray, 0.75f);

            // Barre du cran : zone (jaune), zone x2 (orange), cran (blanc)
            var bar = new Rectangle(panel.X + 20, panel.Y + 90, PanelWidth - 40, 22);
            sb.Draw(pixel, bar, s.Flash > 0 ? Color.Lerp(new Color(30, 30, 30), s.FlashColor, s.Flash / 30f) : new Color(30, 30, 30));
            int zoneX = bar.X + (int)(bar.Width * (s.Zone - WrestleSession.ZoneHalf)), zoneW = (int)(bar.Width * WrestleSession.ZoneHalf * 2f);
            sb.Draw(pixel, new Rectangle(zoneX, bar.Y, zoneW, bar.Height), new Color(220, 200, 60));
            sb.Draw(pixel, new Rectangle(zoneX + zoneW / 4, bar.Y, zoneW / 2, bar.Height), new Color(240, 130, 40));
            sb.Draw(pixel, new Rectangle(bar.X + (int)(bar.Width * s.Cursor) - 2, bar.Y - 4, 4, bar.Height + 8), Color.White);
            Utils.DrawBorderString(sb, Language.GetTextValue(key + "Help"), new Vector2(panel.X + 10, panel.Bottom - 26), Color.Gray, 0.75f);
        }
    }
}
