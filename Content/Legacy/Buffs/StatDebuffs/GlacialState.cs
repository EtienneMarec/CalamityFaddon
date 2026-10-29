// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Buffs.StatDebuffs
{
    /// <summary>
    /// Wiki : Glacial State.txt — debuff retiré de Calamity : « ne peut plus bouger et la défense est brisée ». Repris du
    /// code de 2.0.1.005 pour le mouvement (vitesse x0,9 par tick, chute lente) ; [choix] défense à 0 pendant l'effet (le
    /// wiki ne donne pas de chiffre) ; sans effet sur les boss (règle actuelle de Calamity pour les ralentissements).
    /// Joueur : vitesse horizontale divisée par deux.
    /// </summary>
    public class GlacialState : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.velocity.X *= 0.5f;
            player.velocity.Y = MathHelper.Min(player.velocity.Y + 0.05f, 15f);
        }
    }

    public class GlacialStateNPC : GlobalNPC
    {
        public static bool Affects(NPC npc) => npc.HasBuff<GlacialState>() && !npc.boss;

        public override void PostAI(NPC npc)
        {
            if (!Affects(npc))
                return;
            npc.velocity.X *= 0.9f;
            if (!npc.noGravity)
                npc.velocity.Y = MathHelper.Min(npc.velocity.Y + 0.025f, 15f);
            else
                npc.velocity.Y *= 0.9f;
        }

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (Affects(npc))
                modifiers.DefenseEffectiveness *= 0f;
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (npc.HasBuff<GlacialState>())
                drawColor = Color.Cyan;
        }
    }
}
