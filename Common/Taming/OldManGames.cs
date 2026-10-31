using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;

namespace CalamityFandom.Common.Taming
{
    /// <summary>Ce que le joueur fait pendant un mini-jeu de l'Old Man (clic, souris en coordonnées d'interface, touche 1 à 4).</summary>
    public struct OldManInput
    {
        public bool Click;
        public Vector2 Mouse;
        /// <summary>1 à 4 (touches 1-4 ou flèches), 0 sinon.</summary>
        public int Key;
        public bool Space;
    }

    /// <summary>
    /// Mini-jeu de l'Old Man (Taming de Skeletron). [validé 31/10] Calamity++ : trois mini-jeux différents, avec 30 s de
    /// combat contre Skeletron entre chacun (bras de fer, bonneteau, Simon) ; le monde est figé pendant chaque jeu.
    /// </summary>
    public abstract class OldManGame
    {
        /// <summary>1 gagné, -1 perdu, 0 en cours.</summary>
        public abstract int Result { get; }
        public abstract string Title { get; }
        public abstract string Help { get; }
        public abstract void Update(OldManInput input);
        public abstract void Draw(SpriteBatch sb, Rectangle panel);

        protected static string Text(string key, params object[] args) =>
            Language.GetTextValue("Mods.CalamityFandom.Taming.OldManGames." + key, args);

        public static OldManGame Create(int kind) => kind switch
        {
            1 => new ShellGame(),
            2 => new SimonGame(),
            _ => new WrestleSession(),
        };
    }

    /// <summary>
    /// Bonneteau : l'Old Man montre la clé sous un des trois crânes, les mélange (10 échanges, de plus en plus vite), puis
    /// il faut cliquer le bon crâne (ou 1, 2, 3). [choix] Un seul essai.
    /// </summary>
    public class ShellGame : OldManGame
    {
        private const int ShowTicks = 90, Swaps = 10, RevealTicks = 75;
        private enum Phase { Show, Shuffle, Choose, Reveal }

        private Phase phase = Phase.Show;
        private int timer, swapsDone, swapDuration = 26, swapA, swapB, picked = -1;
        /// <summary>Crâne posé sur chaque emplacement ; la clé est sous le crâne 0.</summary>
        private readonly int[] slotCup = { 0, 1, 2 };
        private bool won;

        public ShellGame()
        {
            // La clé commence sous un emplacement au hasard
            int start = Main.rand.Next(3);
            (slotCup[0], slotCup[start]) = (slotCup[start], slotCup[0]);
        }

        public override int Result => phase == Phase.Reveal && timer >= RevealTicks ? (won ? 1 : -1) : 0;
        public override string Title => Text("ShellTitle");
        public override string Help => Text(phase == Phase.Choose ? "ShellChoose" : "ShellWatch");

        private static Vector2 SlotCenter(Rectangle panel, int slot) =>
            new Vector2(panel.X + panel.Width * (slot + 1) / 4f, panel.Y + 100f);

        public override void Update(OldManInput input)
        {
            timer++;
            switch (phase)
            {
                case Phase.Show:
                    if (timer >= ShowTicks)
                        NextSwap();
                    break;
                case Phase.Shuffle:
                    if (timer >= swapDuration)
                    {
                        (slotCup[swapA], slotCup[swapB]) = (slotCup[swapB], slotCup[swapA]);
                        if (++swapsDone >= Swaps)
                        {
                            phase = Phase.Choose;
                            timer = 0;
                        }
                        else
                            NextSwap();
                    }
                    break;
                case Phase.Choose:
                    int choice = input.Key is >= 1 and <= 3 ? input.Key - 1 : -1;
                    if (choice < 0 && input.Click)
                    {
                        Rectangle panel = WrestleUISystem.Panel;
                        for (int slot = 0; slot < 3; slot++)
                            if (Vector2.Distance(input.Mouse, SlotCenter(panel, slot)) < 34f)
                                choice = slot;
                    }
                    if (choice >= 0)
                    {
                        picked = choice;
                        won = slotCup[choice] == 0;
                        phase = Phase.Reveal;
                        timer = 0;
                        SoundEngine.PlaySound(won ? SoundID.Item4 : SoundID.NPCHit2);
                    }
                    break;
            }
        }

        private void NextSwap()
        {
            phase = Phase.Shuffle;
            timer = 0;
            swapA = Main.rand.Next(3);
            swapB = (swapA + 1 + Main.rand.Next(2)) % 3;
            swapDuration = Math.Max(10, 26 - swapsDone * 2);
            SoundEngine.PlaySound(SoundID.MenuTick);
        }

        public override void Draw(SpriteBatch sb, Rectangle panel)
        {
            Main.instance.LoadItem(ItemID.Skull);
            Main.instance.LoadItem(ItemID.GoldenKey);
            Texture2D skull = TextureAssets.Item[ItemID.Skull].Value, key = TextureAssets.Item[ItemID.GoldenKey].Value;
            for (int slot = 0; slot < 3; slot++)
            {
                Vector2 pos = SlotCenter(panel, slot);
                // Pendant un échange, les deux crânes glissent l'un vers l'autre en arc (l'un dessus, l'autre dessous)
                if (phase == Phase.Shuffle && (slot == swapA || slot == swapB))
                {
                    int other = slot == swapA ? swapB : swapA;
                    float t = MathHelper.Clamp(timer / (float)swapDuration, 0f, 1f);
                    Vector2 target = SlotCenter(panel, other);
                    pos = Vector2.Lerp(pos, target, t) + new Vector2(0f, (slot == swapA ? -1 : 1) * 22f * MathF.Sin(t * MathHelper.Pi));
                }
                bool lifted = phase == Phase.Show || phase == Phase.Reveal && (slot == picked || slotCup[slot] == 0);
                if (lifted && slotCup[slot] == 0)
                    sb.Draw(key, pos + new Vector2(0f, 14f), null, Color.White, 0f, key.Size() / 2f, 1.5f, SpriteEffects.None, 0f);
                Color tint = phase == Phase.Reveal && slot == picked ? (won ? Color.LightGreen : Color.OrangeRed) : Color.White;
                sb.Draw(skull, pos - new Vector2(0f, lifted ? 26f : 0f), null, tint, 0f, skull.Size() / 2f, 2f, SpriteEffects.None, 0f);
                if (phase == Phase.Choose)
                    Utils.DrawBorderString(sb, (slot + 1).ToString(), pos + new Vector2(-4f, 30f), Color.Gray, 0.8f);
            }
            if (phase == Phase.Reveal)
                Utils.DrawBorderString(sb, Text(won ? "ShellWon" : "ShellLost"), new Vector2(panel.X + 10, panel.Y + 30),
                    won ? Color.LightGreen : Color.OrangeRed);
        }
    }

    /// <summary>
    /// Simon : quatre dalles de couleur s'allument dans un ordre (6 fois), puis il faut le refaire en cliquant les dalles
    /// (ou 1 à 4, ou les flèches gauche, haut, bas, droite). Une erreur et c'est perdu. [choix] 6 éléments, 0,5 s chacun.
    /// </summary>
    public class SimonGame : OldManGame
    {
        private const int Length = 6, LitTicks = 30, GapTicks = 12, EndTicks = 75;
        private static readonly Color[] Colors = { new(220, 60, 60), new(70, 130, 230), new(80, 190, 90), new(230, 200, 60) };

        private readonly int[] sequence = new int[Length];
        private int shown, entered, timer, flashPad = -1, flashTimer;
        private bool showing = true, ended, won;

        public SimonGame()
        {
            for (int i = 0; i < Length; i++)
                sequence[i] = Main.rand.Next(4);
        }

        public override int Result => ended && timer >= EndTicks ? (won ? 1 : -1) : 0;
        public override string Title => Text("SimonTitle");
        public override string Help => Text(showing ? "SimonWatch" : "SimonRepeat", entered, Length);

        private static Rectangle Pad(Rectangle panel, int i) =>
            new Rectangle(panel.X + 40 + i * ((panel.Width - 80) / 4), panel.Y + 60, (panel.Width - 80) / 4 - 12, 60);

        public override void Update(OldManInput input)
        {
            timer++;
            if (flashTimer > 0 && --flashTimer == 0)
                flashPad = -1;
            if (ended)
                return;
            if (showing)
            {
                // Chaque élément : allumé LitTicks, puis éteint GapTicks
                if (timer == 1)
                    Light(sequence[shown], LitTicks);
                if (timer >= LitTicks + GapTicks)
                {
                    timer = 0;
                    if (++shown >= Length)
                        showing = false;
                }
                return;
            }
            int pad = input.Key is >= 1 and <= 4 ? input.Key - 1 : -1;
            if (pad < 0 && input.Click)
                for (int i = 0; i < 4; i++)
                    if (Pad(WrestleUISystem.Panel, i).Contains(input.Mouse.ToPoint()))
                        pad = i;
            if (pad < 0)
                return;
            Light(pad, 12);
            if (pad != sequence[entered])
                End(false);
            else if (++entered >= Length)
                End(true);
        }

        private void Light(int pad, int ticks)
        {
            flashPad = pad;
            flashTimer = ticks;
            SoundEngine.PlaySound(SoundID.Item4 with { Pitch = -0.4f + pad * 0.25f, Volume = 0.6f });
        }

        private void End(bool success)
        {
            ended = true;
            won = success;
            timer = 0;
            SoundEngine.PlaySound(success ? SoundID.Item4 : SoundID.NPCHit2);
        }

        public override void Draw(SpriteBatch sb, Rectangle panel)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            for (int i = 0; i < 4; i++)
            {
                Rectangle r = Pad(panel, i);
                Color c = Colors[i] * (flashPad == i ? 1f : 0.35f);
                sb.Draw(pixel, r, c);
                Utils.DrawBorderString(sb, (i + 1).ToString(), new Vector2(r.Center.X - 4, r.Bottom + 2), Color.Gray, 0.8f);
            }
            if (ended)
                Utils.DrawBorderString(sb, Text(won ? "SimonWon" : "SimonLost"), new Vector2(panel.X + 10, panel.Y + 30),
                    won ? Color.LightGreen : Color.OrangeRed);
        }
    }
}
