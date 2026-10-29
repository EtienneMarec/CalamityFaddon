// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using CalamityFandom.Content.Legacy.Projectiles.Summon;
using Terraria;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Buffs.Summon
{
    /// <summary>Buff de sbire : reste tant qu'un slime du joueur existe.</summary>
    public abstract class SlimeBuff<TMinion> : ModBuff where TMinion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<TMinion>()] > 0)
                player.buffTime[buffIndex] = 18000;
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }

    public class Crimslime : SlimeBuff<CrimslimeMinion>
    {
    }

    public class Corroslime : SlimeBuff<CorroslimeMinion>
    {
    }

    /// <summary>Buff du Blood Clot Staff (même mécanique que les slimes).</summary>
    public class BloodClot : SlimeBuff<BloodClotMinion>
    {
    }
}
