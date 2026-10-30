using MonoMod.RuntimeDetour;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Common.Taming
{
    /// <summary>
    /// Seconds boutons de dialogue ajoutés aux PNJ du jeu de base pour le Taming : Tax Collector (impôts de Draedon), Old
    /// Man (bras de fer). [technique] tModLoader ne laisse changer les boutons que des PNJ du mod (ModNPC.SetChatButtons) :
    /// détour sur l'appel commun NPCLoader.SetChatButtons ; le clic passe par GlobalNPC.PreChatButtonClicked.
    /// </summary>
    public class TamingChatButtons : ModSystem
    {
        private delegate void SetChatButtonsOrig(ref string button, ref string button2);
        private delegate void SetChatButtonsHook(SetChatButtonsOrig orig, ref string button, ref string button2);
        private static Hook hook;

        public override void Load() =>
            hook = new Hook(typeof(NPCLoader).GetMethod(nameof(NPCLoader.SetChatButtons)), (SetChatButtonsHook)OnSetChatButtons);

        public override void Unload()
        {
            hook?.Dispose();
            hook = null;
        }

        private static void OnSetChatButtons(SetChatButtonsOrig orig, ref string button, ref string button2)
        {
            orig(ref button, ref button2);
            string extra = Main.LocalPlayer.TalkNPC?.type switch
            {
                NPCID.TaxCollector => TamingTaxCollector.SecondButton(),
                NPCID.OldMan => OldManWrestle.SecondButton(),
                _ => null,
            };
            if (extra != null)
                button2 = extra;
        }
    }
}
