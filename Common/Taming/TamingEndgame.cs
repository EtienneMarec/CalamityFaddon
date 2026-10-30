using System;
using System.IO;
using CalamityMod.NPCs.SupremeCalamitas;
using Microsoft.Xna.Framework;
using MonoMod.RuntimeDetour;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Fight = CalamityFandom.Common.Taming.TamingSystem.Fight;

namespace CalamityFandom.Common.Taming
{
    /// <summary>
    /// Supreme Calamitas (page Taming : « Tame all previous bosses, and then press alt-f4 once Calamitas is at 5% HP »).
    /// [validé 31/10] Si tous les autres boss apprivoisables sont apprivoisés dans ce monde, sa vie baisse d'elle-même tant
    /// qu'on ne la touche pas, jusqu'à 5 %. Quitter le jeu à ce moment (alt-f4, ou Save &amp; Exit) l'apprivoise : au
    /// chargement suivant du monde, elle apparaît et est apprivoisée aussitôt (butin, Brimstone Witch).
    /// Le « quitter » est noté dans un fichier à part, écrit dès les 5 % et effacé si le combat finit autrement.
    /// </summary>
    public class SupremeCalamitasQuit : ModSystem
    {
        /// <summary>[choix] De 100 % à 5 % en 5 minutes.</summary>
        public const int DrainTicks = 5 * 60 * 60;
        public const float Floor = 0.05f;
        private static bool pending;

        private static string MarkerPath => Path.Combine(Main.SavePath, "CalamityFandom_SupremeCalamitasTamed.txt");

        private static string WorldId => Main.ActiveWorldFileData?.UniqueId.ToString() ?? "";

        public static void Remember()
        {
            try
            {
                File.WriteAllText(MarkerPath, WorldId);
            }
            catch (IOException) { }
        }

        public static void Forget()
        {
            try
            {
                if (File.Exists(MarkerPath))
                    File.Delete(MarkerPath);
            }
            catch (IOException) { }
        }

        public override void OnWorldLoad()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            try
            {
                pending = File.Exists(MarkerPath) && File.ReadAllText(MarkerPath).Trim() == WorldId;
            }
            catch (IOException)
            {
                pending = false;
            }
            if (pending)
                Forget();
        }

        public override void PostUpdateWorld()
        {
            if (!pending)
                return;
            foreach (var p in Main.ActivePlayers)
            {
                if (p.dead)
                    continue;
                pending = false;
                TamingSystem.TameOnSpawn(TamingSystem.SupremeCalamitasKey);
                NPC.SpawnOnPlayer(p.whoAmI, ModContent.NPCType<SupremeCalamitas>());
                return;
            }
        }

        /// <summary>Appelé à chaque tick du combat (serveur ou solo).</summary>
        public static void Tick(Fight f, NPC scal)
        {
            if (!TamingSystem.AllOthersTamed())
                return;
            int floor = (int)(scal.lifeMax * Floor);
            if (scal.life > floor)
            {
                // Perte fractionnaire cumulée dans f.A
                f.A += scal.lifeMax * (1f - Floor) / DrainTicks;
                int loss = (int)f.A;
                f.A -= loss;
                scal.life = Math.Max(floor, scal.life - loss);
                // Ce n'est pas le joueur qui la blesse
                f.LastLife[scal.whoAmI] = scal.life;
                if (Main.netMode == NetmodeID.Server && Main.GameUpdateCount % 30 == 0)
                    scal.netUpdate = true;
            }
            f.Progress = Math.Min(99f, (1f - scal.life / (float)scal.lifeMax) * 100f);
            if (scal.life <= floor && !f.Flag)
            {
                f.Flag = true;
                Remember();
                const string key = "Mods.CalamityFandom.Taming.SupremeCalamitasQuit";
                if (Main.netMode == NetmodeID.Server)
                    ChatHelper.BroadcastChatMessage(NetworkText.FromKey(key), Color.Orange);
                else
                    Main.NewText(Language.GetTextValue(key), Color.Orange);
            }
        }
    }

    /// <summary>
    /// The Fabricational Quartet (Exo Mechs) : « File Draedons Taxes ». [validé 31/10] Pendant le combat seulement, le Tax
    /// Collector a un second bouton qui fait payer 1 platine : les impôts de Draedon sont déclarés, les mechs apprivoisés.
    /// </summary>
    public class TamingTaxCollector : GlobalNPC
    {
        public const string Key = "The Fabricational Quartet";
        public static readonly int Price = Item.buyPrice(platinum: 1);

        private delegate void SetChatButtonsOrig(ref string button, ref string button2);
        private delegate void SetChatButtonsHook(SetChatButtonsOrig orig, ref string button, ref string button2);
        private static Hook hook;

        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.TaxCollector;

        public override void Load()
        {
            // tModLoader ne laisse changer les boutons que des PNJ du mod (ModNPC.SetChatButtons) : détour sur l'appel commun
            hook = new Hook(typeof(NPCLoader).GetMethod(nameof(NPCLoader.SetChatButtons)), (SetChatButtonsHook)OnSetChatButtons);
        }

        public override void Unload()
        {
            hook?.Dispose();
            hook = null;
        }

        /// <summary>Un Exo Mech apprivoisable se bat (vu du client : la liste des boss tués est synchronisée).</summary>
        private static bool FightOn()
        {
            var e = TamingSystem.Get(Key);
            return e != null && !TamingSystem.Untamable.Contains(Key) && TamingSystem.FindMain(e) != null;
        }

        private static void OnSetChatButtons(SetChatButtonsOrig orig, ref string button, ref string button2)
        {
            orig(ref button, ref button2);
            if (Main.LocalPlayer.TalkNPC?.type == NPCID.TaxCollector && FightOn())
                button2 = Language.GetTextValue("Mods.CalamityFandom.Taming.Taxes.Button");
        }

        public override void GetChat(NPC npc, ref string chat)
        {
            if (FightOn())
                chat = Language.GetTextValue("Mods.CalamityFandom.Taming.Taxes.Chat");
        }

        public override bool PreChatButtonClicked(NPC npc, bool firstButton)
        {
            if (firstButton || !FightOn())
                return true;
            var player = Main.LocalPlayer;
            if (player.BuyItem(Price))
            {
                TamingSystem.Report(Key, 1f);
                Main.npcChatText = Language.GetTextValue("Mods.CalamityFandom.Taming.Taxes.Paid");
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Coins);
            }
            else
                Main.npcChatText = Language.GetTextValue("Mods.CalamityFandom.Taming.Taxes.Poor");
            return false;
        }
    }
}
