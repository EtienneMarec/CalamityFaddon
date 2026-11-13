// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using CalamityFandom.Content.Legacy.Buffs.StatDebuffs;
using CalamityMod;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.NPCs.NormalNPCs;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace CalamityFandom.Content.Legacy.NPCs.NormalNPCs
{
    /// <summary>
    /// Wiki : Sea Urchin.txt — ennemi de l'océan avant Hardmode qui bondit vers le joueur, dans l'eau comme hors de l'eau ;
    /// 15 à 25 Urchin Stingers ; Venom 2 s au contact (vieux code) ; insensible à Kami Flu.
    /// [technique] Pas de bannière : celle de 2.0.1.005 utilisait une tuile de bannières de Calamity qui a changé.
    /// </summary>
    public class SeaUrchin : ModNPC
    {
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.damage = 20;
            NPC.width = NPC.height = 40;
            NPC.defense = 10;
            NPC.lifeMax = 30;
            NPC.knockBackResist = 0.8f;
            NPC.value = Item.buyPrice(copper: 80);
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath15;
            NPC.behindTiles = true;
            NPC.buffImmune[ModContent.BuffType<KamiFlu>()] = true;
            var cal = NPC.Calamity();
            cal.VulnerableToHeat = false;
            cal.VulnerableToSickness = false;
            cal.VulnerableToElectricity = true;
            cal.VulnerableToWater = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Ocean,
                new FlavorTextBestiaryInfoElement("Mods.CalamityFandom.NPCs.SeaUrchin.Bestiary"),
            });
        }

        public override void AI()
        {
            bool wet = NPC.wet;
            CalamityRegularEnemyAI.UnicornAI(NPC, Mod, true, wet ? 9f : 4f, wet ? 5.5f : 2.2f, wet ? 0.09f : 0.04f,
                wet ? -14f : -6.5f, wet ? -12f : -6f, wet ? -11f : -5f, wet ? -10f : -4f, wet ? -13f : -6f);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(BuffID.Venom, 120, true);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.PlayerSafe || spawnInfo.Player.Calamity().ZoneSulphur)
                return 0f;
            return SpawnCondition.OceanMonster.Chance * 0.2f;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) => npcLoot.Add(Terraria.GameContent.ItemDropRules.ItemDropRule.Common(ModContent.ItemType<UrchinStinger>(), 1, 15, 25));

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < (NPC.life <= 0 ? 25 : 5); k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f);
        }
    }
}
