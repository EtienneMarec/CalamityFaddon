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
    /// [choix] Après un coup, 1/3 s sans pouvoir recliquer (sinon cliquer en continu gagne à coup sûr) ; après chaque coup
    /// réussi, la zone change de place et le cran accélère de 5 % (jusqu'à ×2). [validé 31/10] Un coup raté fait reculer de
    /// 3 ([choix] dans les 3-4 demandés) ; tout est figé pendant le bras de fer, comme aux échecs.
    /// </summary>
    public class WrestleSession : OldManGame
    {
        public const int Goal = 50, OldManPull = 1, PullTicks = 30, Hit = 5, DoubleHit = 10, Miss = 3, Cooldown = 20;
        public const float ZoneHalf = 0.1f, BaseSpeed = 1f / 60f;

        public int Meter;
        public float Cursor, Speed = BaseSpeed, Zone = 0.5f;
        private int direction = 1, pullTimer, cooldown;
        public int Flash;
        public Color FlashColor;

        public override int Result => Meter >= Goal ? 1 : Meter <= -Goal ? -1 : 0;
        public override string Title => Language.GetTextValue("Mods.CalamityFandom.Taming.Wrestle.Title");
        public override string Help => Language.GetTextValue("Mods.CalamityFandom.Taming.Wrestle.Help");

        public override void Update(OldManInput input)
        {
            bool click = input.Click || input.Space;
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
                Meter -= Miss;
                Flash = 15;
                FlashColor = Color.Red;
                SoundEngine.PlaySound(SoundID.Tink);
                return;
            }
            Meter += gain;
            Flash = 15;
            FlashColor = gain == DoubleHit ? Color.Orange : Color.Yellow;
            SoundEngine.PlaySound(gain == DoubleHit ? SoundID.Item37 : SoundID.Item1);
            Zone = Main.rand.NextFloat(ZoneHalf, 1f - ZoneHalf);
            Speed = Math.Min(Speed * 1.05f, BaseSpeed * 2f);
        }

        public override void Draw(SpriteBatch sb, Rectangle panel)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            string key = "Mods.CalamityFandom.Taming.Wrestle.";
            // Bras de fer : à gauche l'Old Man, à droite le joueur
            var tug = new Rectangle(panel.X + 20, panel.Y + 40, panel.Width - 40, 18);
            sb.Draw(pixel, tug, new Color(90, 70, 60));
            float ratio = (Meter + Goal) / (2f * Goal);
            sb.Draw(pixel, new Rectangle(tug.X, tug.Y, (int)(tug.Width * Math.Clamp(ratio, 0f, 1f)), tug.Height), new Color(110, 190, 90));
            sb.Draw(pixel, new Rectangle(tug.Center.X - 1, tug.Y - 3, 2, tug.Height + 6), Color.White);
            Utils.DrawBorderString(sb, Language.GetTextValue(key + "OldMan"), new Vector2(tug.X, tug.Bottom + 2), Color.LightGray, 0.75f);
            string you = Language.GetTextValue(key + "You");
            Utils.DrawBorderString(sb, you, new Vector2(tug.Right - FontAssets.MouseText.Value.MeasureString(you).X * 0.75f, tug.Bottom + 2), Color.LightGray, 0.75f);

            // Barre du cran : zone (jaune), zone x2 (orange), cran (blanc)
            var bar = new Rectangle(panel.X + 20, panel.Y + 90, panel.Width - 40, 22);
            sb.Draw(pixel, bar, Flash > 0 ? Color.Lerp(new Color(30, 30, 30), FlashColor, Flash / 30f) : new Color(30, 30, 30));
            int zoneX = bar.X + (int)(bar.Width * (Zone - ZoneHalf)), zoneW = (int)(bar.Width * ZoneHalf * 2f);
            sb.Draw(pixel, new Rectangle(zoneX, bar.Y, zoneW, bar.Height), new Color(220, 200, 60));
            sb.Draw(pixel, new Rectangle(zoneX + zoneW / 4, bar.Y, zoneW / 2, bar.Height), new Color(240, 130, 40));
            sb.Draw(pixel, new Rectangle(bar.X + (int)(bar.Width * Cursor) - 2, bar.Y - 4, 4, bar.Height + 8), Color.White);
        }
    }

    /// <summary>
    /// Fenêtre des mini-jeux de l'Old Man (bras de fer avant Skeletron ; en Calamity++, bonneteau puis Simon pendant le
    /// combat). Le monde est figé tant qu'un jeu est ouvert (TamingSystem.ReportWrestling).
    /// </summary>
    [Autoload(Side = ModSide.Client)]
    public class WrestleUISystem : ModSystem
    {
        private static OldManGame session;
        /// <summary>Jeu lancé pendant le combat contre Skeletron (le résultat va au combat, pas à l'apparition du boss).</summary>
        private static bool inFight;

        /// <summary>Partie en cours ; l'ouvrir ou la fermer fige ou relance le monde (TamingSystem.ReportWrestling).</summary>
        public static OldManGame Session
        {
            get => session;
            private set
            {
                if ((session == null) != (value == null))
                    TamingSystem.ReportWrestling(value != null);
                session = value;
            }
        }
        private static int endTimer;
        private static bool mouseWasDown;
        private const int PanelWidth = 440, PanelHeight = 170;

        /// <summary>Panneau, en coordonnées d'interface (centré malgré l'échelle de l'interface).</summary>
        public static Rectangle Panel =>
            new((int)(Main.screenWidth / Main.UIScale / 2f) - PanelWidth / 2, (int)(Main.screenHeight / Main.UIScale / 2f) - PanelHeight / 2, PanelWidth, PanelHeight);

        /// <summary>0 : bras de fer, 1 : bonneteau, 2 : Simon ; <paramref name="duringFight"/> : pendant le combat contre Skeletron.</summary>
        public static void Open(int kind = 0, bool duringFight = false)
        {
            if (Session != null)
                return;
            Session = OldManGame.Create(kind);
            inFight = duringFight;
            endTimer = 0;
            // Le clic qui a lancé le jeu (bouton « Arm Wrestle ») ne compte pas comme un premier coup
            mouseWasDown = true;
        }

        private static void Finish(bool won)
        {
            if (inFight)
                TamingSystem.ReportOldManGame(won);
            else if (won)
                TamingSystem.ReportArmWrestleWon();
        }

        public override void OnWorldUnload()
        {
            session = null;
            Main.LocalPlayer.GetModPlayer<Chess.ChessPlayer>().Wrestling = false;
        }

        public override void UpdateUI(GameTime gameTime)
        {
            var s = Session;
            if (s == null)
                return;
            if (Main.LocalPlayer.dead || Main.keyState.IsKeyDown(Keys.Escape))
            {
                // Abandon pendant le combat : compte comme une défaite (le jeu reviendra 30 s plus tard)
                if (endTimer == 0 && inFight)
                    Finish(false);
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
            // [validé 31/10] Clic gauche ou Espace : on repère nous-mêmes le moment où le bouton s'enfonce (mouseLeftRelease
            // est déjà remis à faux quand l'interface se met à jour, le clic ne passait pas)
            bool mouse = Main.mouseLeft;
            var input = new OldManInput
            {
                Click = mouse && !mouseWasDown,
                Mouse = Main.MouseScreen,
                Space = Pressed(Keys.Space),
                Key = Pressed(Keys.D1) || Pressed(Keys.Left) ? 1 : Pressed(Keys.D2) || Pressed(Keys.Up) ? 2
                    : Pressed(Keys.D3) || Pressed(Keys.Down) ? 3 : Pressed(Keys.D4) || Pressed(Keys.Right) ? 4 : 0,
            };
            mouseWasDown = mouse;
            s.Update(input);
            if (s.Result == 0)
                return;
            endTimer = 90;
            // Le résultat s'affiche encore 1,5 s, mais le monde repart tout de suite
            TamingSystem.ReportWrestling(false);
            if (s is WrestleSession)
                SoundEngine.PlaySound(s.Result > 0 ? SoundID.Item4 : SoundID.NPCHit2);
            Finish(s.Result > 0);
        }

        private static bool Pressed(Keys key) => Main.keyState.IsKeyDown(key) && !Main.oldKeyState.IsKeyDown(key);

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(l => l.Name == "Vanilla: Mouse Text");
            if (index < 0 || Session == null)
                return;
            layers.Insert(index, new LegacyGameInterfaceLayer("CalamityFandom: Old Man games", () =>
            {
                Draw(Main.spriteBatch, Session);
                return true;
            }, InterfaceScaleType.UI));
        }

        private static void Draw(SpriteBatch sb, OldManGame s)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            Rectangle panel = Panel;
            sb.Draw(pixel, panel, new Color(40, 25, 15, 230));
            string key = "Mods.CalamityFandom.Taming.Wrestle.";
            string title = s is WrestleSession && s.Result > 0 ? Language.GetTextValue(key + "Won")
                : s is WrestleSession && s.Result < 0 ? Language.GetTextValue(key + "Lost") : s.Title;
            Utils.DrawBorderString(sb, title, panel.TopLeft() + new Vector2(10, 6), s.Result < 0 ? Color.OrangeRed : Color.Wheat);
            s.Draw(sb, panel);
            Utils.DrawBorderString(sb, s.Help, new Vector2(panel.X + 10, panel.Bottom - 26), Color.Gray, 0.75f);
        }
    }
}
