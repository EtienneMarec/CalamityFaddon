// Adapté de Calamity Mod (debuff retiré en 2022), © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
using CalamityMod;
using Terraria;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Buffs.StatDebuffs
{
    /// <summary>
    /// Wiki : War Cleave.txt — « Targets defense is reduced by 15 and damage reduction by 25% » (Waraxe, Warblade sur un coup
    /// critique). Ennemis : -15 de défense, réduction de dégâts de Calamity multipliée par 0,75 (comme l'ancien code) ;
    /// joueurs : -15 de défense, réduction de dégâts x0,75.
    /// </summary>
    public class WarCleave : ModBuff
    {
        public const int DefenseReduction = 15;

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.statDefense -= DefenseReduction;
            player.endurance *= 0.75f;
        }
    }

    public class WarCleaveNPC : GlobalNPC
    {
        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (!npc.HasBuff(ModContent.BuffType<WarCleave>()))
                return;
            modifiers.Defense.Flat -= WarCleave.DefenseReduction;
            // Réduction de dégâts de Calamity (appliquée par elle) ramenée à 75 % : on compense sur les dégâts finaux
            float dr = npc.Calamity().DR;
            if (dr > 0f && dr < 1f)
                modifiers.FinalDamage *= (1f - 0.75f * dr) / (1f - dr);
        }
    }
}
