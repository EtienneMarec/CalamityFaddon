using CalamityMod.Events;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Buffs
{
    /// <summary>
    /// Wiki : Exo Freeze.txt — debuff retiré de Calamity (il n'en reste que le texte) : « makes the target completely unable
    /// to move for its duration », sans effet pendant la Boss Rush. [choix] Les boss n'y sont pas sensibles (la page dit
    /// « some bosses » sans les nommer) ; la cible garde son IA, seule sa vitesse est annulée.
    /// </summary>
    public class ExoFreeze : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex) => npc.GetGlobalNPC<ExoFreezeNPC>().Frozen = true;
    }

    public class ExoFreezeNPC : GlobalNPC
    {
        public bool Frozen;

        public override bool InstancePerEntity => true;

        public override void ResetEffects(NPC npc) => Frozen = false;

        // Après l'IA, avant que la vitesse ne déplace le PNJ
        public override void PostAI(NPC npc)
        {
            if (Frozen && !npc.boss && !BossRushEvent.BossRushActive)
                npc.velocity = Vector2.Zero;
        }
    }
}
