using CalamityMod;
using Terraria;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Buffs
{
    /// <summary>
    /// Wiki : Flahmes.txt — debuff des objets en Flahium : la cible perd 500 PV/s et sa réduction des dégâts baisse de 35 %.
    /// [choix] 35 points de réduction en moins (pas 35 % de sa valeur), jamais en dessous de 0, appliqués par-dessus le calcul
    /// de Calamity.
    /// </summary>
    public class Flahmes : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex) => npc.GetGlobalNPC<FlahmesNPC>().Burning = true;
    }

    public class FlahmesNPC : GlobalNPC
    {
        public const float DRReduction = 0.35f;
        public bool Burning;

        public override bool InstancePerEntity => true;

        public override void ResetEffects(NPC npc) => Burning = false;

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (!Burning)
                return;
            if (npc.lifeRegen > 0)
                npc.lifeRegen = 0;
            npc.lifeRegen -= 1000;
            if (damage < 100)
                damage = 100;
        }

        // Calamity multiplie les dégâts par (1 - réduction) : on compense pour une réduction de 35 points de moins
        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            float dr = npc.Calamity().DR;
            if (!Burning || dr <= 0f || dr >= 1f)
                return;
            modifiers.FinalDamage *= (1f - System.Math.Max(0f, dr - DRReduction)) / (1f - dr);
        }
    }
}
