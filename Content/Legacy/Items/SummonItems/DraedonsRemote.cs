// Adapté de Calamity Mod (version d'avril 2022, 187bad450b), © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using System.Collections.Generic;
using CalamityFandom.Common.Taming;
using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.AstrumAureus;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.SummonItems
{
    /// <summary>
    /// Wiki : Draedon_s Remote.txt — non consommé ; la nuit avant Plantera, fait apparaître les trois boss mécaniques à la
    /// fois, renforcés (DraedonMayhem). Lâché par l'Armored Digger (1 sur 3, infobox). Utilisé pendant qu'Astrum Aureus est
    /// en vie, il l'apprivoise (page Taming). TODO : de jour, la « Mechanical Horde » (événement inventé, à créer).
    /// </summary>
    public class DraedonsRemote : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.rare = ItemRarityID.Orange;
            Item.useAnimation = Item.useTime = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
            Item.value = 0;
        }

        private static bool AureusTamable()
        {
            var e = TamingSystem.Get("Astrum Aureus");
            return e != null && !TamingSystem.Untamable.Contains(e.Key) && TamingSystem.FindMain(e) != null;
        }

        // « Attempting to use the item while any of the three Mechanical Bosses are still alive will do nothing »
        public override bool CanUseItem(Player player) =>
            AureusTamable() || !Main.dayTime && !NPC.downedPlantBoss && !DraedonMayhem.AnyMech() && !BossRushEvent.BossRushActive;

        public override bool? UseItem(Player player)
        {
            // « Use Draedon's Remote while Astrum Aureus is alive » (Taming)
            if (AureusTamable())
            {
                if (player.whoAmI == Main.myPlayer)
                    TamingSystem.Report("Astrum Aureus", 1f);
                return true;
            }
            SoundEngine.PlaySound(SoundID.Roar, player.Center);
            if (player.whoAmI != Main.myPlayer)
                return true;
            foreach (int boss in new[] { NPCID.TheDestroyer, NPCID.SkeletronPrime, NPCID.Spazmatism, NPCID.Retinazer })
            {
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: boss);
                else
                    NPC.SpawnOnPlayer(player.whoAmI, boss);
            }
            return true;
        }
    }

    /// <summary>
    /// Boss mécaniques appelés par le Draedon's Remote (page Draedon's Remote) : Twins et Destroyer x1,8 PV (Destroyer
    /// x1,5 en taille), Probes x1,6 PV et x1,2 en taille, tête de Skeletron Prime x1,45 PV, ses bras x0,8 ; tant qu'un
    /// d'eux vit, le joueur subit 20 % de dégâts en plus ; chacun lâche 8-16 Mysterious Circuitry et Dubious Plating.
    /// [technique] Le « mode Mayhem » de 2022 (CalamityGlobalNPC.DraedonMayhem) n'existe plus : refait ici. Un boss
    /// mécanique qui apparaît pendant que d'autres boss mécaniques du remote vivent en fait partie.
    /// </summary>
    public class DraedonMayhem : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public bool Mayhem;

        private static readonly Dictionary<int, (float life, float size)> Buffs = new()
        {
            [NPCID.Retinazer] = (1.8f, 1f),
            [NPCID.Spazmatism] = (1.8f, 1f),
            [NPCID.TheDestroyer] = (1.8f, 1.5f),
            [NPCID.TheDestroyerBody] = (1.8f, 1.5f),
            [NPCID.TheDestroyerTail] = (1.8f, 1.5f),
            [NPCID.Probe] = (1.6f, 1.2f),
            [NPCID.SkeletronPrime] = (1.45f, 1f),
            [NPCID.PrimeCannon] = (0.8f, 1f),
            [NPCID.PrimeLaser] = (0.8f, 1f),
            [NPCID.PrimeSaw] = (0.8f, 1f),
            [NPCID.PrimeVice] = (0.8f, 1f),
        };

        private static readonly int[] Leaders = { NPCID.Retinazer, NPCID.Spazmatism, NPCID.TheDestroyer, NPCID.SkeletronPrime };

        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => Buffs.ContainsKey(entity.type);

        public static bool AnyMech()
        {
            foreach (var n in Main.ActiveNPCs)
                if (System.Array.IndexOf(Leaders, n.type) >= 0)
                    return true;
            return false;
        }

        /// <summary>Un boss mécanique du remote est en vie (dégâts subis +20 %).</summary>
        public static bool Active()
        {
            foreach (var n in Main.ActiveNPCs)
                if (n.TryGetGlobalNPC<DraedonMayhem>(out var m) && m.Mayhem)
                    return true;
            return false;
        }

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            // Appelé par le remote (objet tenu au moment de l'appel), ou né pendant un combat du remote (segments, sondes, bras)
            bool fromRemote = source is EntitySource_BossSpawn spawn && spawn.Target is Player p && p.HeldItem.type == ModContent.ItemType<DraedonsRemote>();
            if (!fromRemote && !Active())
                return;
            Mayhem = true;
            var (life, size) = Buffs[npc.type];
            npc.lifeMax = (int)(npc.lifeMax * life);
            npc.life = npc.lifeMax;
            if (size != 1f)
            {
                npc.scale *= size;
                npc.width = (int)(npc.width * size);
                npc.height = (int)(npc.height * size);
            }
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (System.Array.IndexOf(Leaders, npc.type) < 0)
                return;
            var root = new LeadingConditionRule(new MayhemCondition());
            // Les Twins : seulement le dernier des deux
            IItemDropRule drops = npc.type == NPCID.Retinazer || npc.type == NPCID.Spazmatism
                ? root.OnSuccess(new LeadingConditionRule(new Conditions.MissingTwin()))
                : root;
            drops.OnSuccess(ItemDropRule.Common(ModContent.ItemType<MysteriousCircuitry>(), 1, 8, 16));
            drops.OnSuccess(ItemDropRule.Common(ModContent.ItemType<DubiousPlating>(), 1, 8, 16));
            npcLoot.Add(root);
            // Butin donné par la page du Draedon's Remote, pas par celle du boss : gardé en Full Fandom
            Common.WikiData.WikiLoot.Keep.Add(root);
        }

        private class MayhemCondition : IItemDropRuleCondition
        {
            public bool CanDrop(DropAttemptInfo info) => info.npc != null && info.npc.TryGetGlobalNPC<DraedonMayhem>(out var m) && m.Mayhem;
            public bool CanShowItemDropInUI() => true;
            public string GetConditionDescription() => Language.GetTextValue("Mods.CalamityFandom.Conditions.DraedonsRemote");
        }
    }

    public class DraedonMayhemPlayer : ModPlayer
    {
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (DraedonMayhem.Active())
                modifiers.FinalDamage *= 1.2f;
        }
    }
}
