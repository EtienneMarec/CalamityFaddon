using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Common.Taming
{
    /// <summary>
    /// [validé 31/10] Départ d'un boss apprivoisé : son butin tombe tout de suite, puis il joue sa propre animation
    /// d'abandon (le Eye of Cthulhu s'envole, les vers s'enfouissent...) au lieu de disparaître d'un coup. Pour ça, pendant
    /// sa mise à jour, il croit tous les joueurs morts (sa propre IA, ou celle que Calamity lui donne, le fait partir).
    /// Il ne blesse plus personne et ne prend plus de dégâts. [choix] Au bout de 6 s, il s'efface en 1,5 s s'il est
    /// encore là. Ses PV sont mis à 0 quand il disparaît : Boss Checklist n'affiche alors pas « ... is no longer after
    /// you » (ce message ne sort que pour un boss qui part en vie).
    /// </summary>
    public class TamingLeave : ModSystem
    {
        public const int MaxTicks = 6 * 60, FadeTicks = 90;

        /// <summary>PNJ qui partent (index) : type, pour ne pas confondre avec un PNJ qui reprendrait la place, et âge.</summary>
        private static readonly Dictionary<int, (int type, int age)> leaving = new();
        private static readonly bool[] wasDead = new bool[Main.maxPlayers];

        public static bool IsLeaving(NPC npc) =>
            leaving.TryGetValue(npc.whoAmI, out var l) && l.type == npc.type && npc.active;

        public static void Start(NPC npc)
        {
            leaving[npc.whoAmI] = (npc.type, 0);
            npc.dontTakeDamage = true;
            npc.netUpdate = true;
        }

        public static IEnumerable<int> Indices => leaving.Keys;

        public override void Load() => On_NPC.UpdateNPC += UpdateLeaving;

        public override void ClearWorld() => leaving.Clear();

        private static void UpdateLeaving(On_NPC.orig_UpdateNPC orig, NPC self, int i)
        {
            if (!leaving.TryGetValue(i, out var l))
            {
                orig(self, i);
                return;
            }
            if (!self.active || self.type != l.type)
            {
                leaving.Remove(i);
                orig(self, i);
                return;
            }
            // Personne n'est en vie pour lui : il part comme quand il perd sa cible
            for (int p = 0; p < Main.maxPlayers; p++)
            {
                wasDead[p] = Main.player[p].dead;
                if (Main.player[p].active)
                    Main.player[p].dead = true;
            }
            try
            {
                orig(self, i);
            }
            finally
            {
                for (int p = 0; p < Main.maxPlayers; p++)
                    Main.player[p].dead = wasDead[p];
            }
            self.dontTakeDamage = true;
            int age = l.age + 1;
            leaving[i] = (l.type, age);
            if (age > MaxTicks - FadeTicks)
                self.alpha = System.Math.Max(self.alpha, (int)(255f * (age - (MaxTicks - FadeTicks)) / FadeTicks));
            if (age >= MaxTicks && Main.netMode != NetmodeID.MultiplayerClient)
            {
                self.active = false;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.SyncNPC, number: i);
            }
            if (!self.active)
            {
                // Parti (par lui-même ou au bout du temps) : PV à 0, pas de message de Boss Checklist
                self.life = 0;
                leaving.Remove(i);
            }
        }

        /// <summary>Clients : les PNJ qui partent, reçus avec le paquet du Taming.</summary>
        public static void Receive(int index, int type)
        {
            if (index >= 0 && index < Main.maxNPCs)
                leaving[index] = (type, 0);
        }
    }

    /// <summary>Un boss qui part ne touche plus personne.</summary>
    public class TamingLeaveNPC : GlobalNPC
    {
        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot) => !TamingLeave.IsLeaving(npc);

        public override bool CanHitNPC(NPC npc, NPC target) => !TamingLeave.IsLeaving(npc);
    }
}
