using System;
using System.Collections.Generic;
using System.Linq;
using CalamityFandom.Common.Debug;
using CalamityFandom.Common.WikiData;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using CalamityFandom.Content.Items.Materials;
using Terraria.ID;
using Terraria.ModLoader;

// Lot BD (20/11) : Demon Potion (slimes), Expired Medication (Plaguebringer Goliath, Death), Desolate Flah (lave).

namespace CalamityFandom.Content.Items.Potions
{
    /// <summary>
    /// Wiki : Demon Potion.txt — buff Evil 100 min : apparitions et nombre maximal d'ennemis x11 (+1000 %) ; lâchée souvent
    /// par les slimes. [choix] « Souvent » = 1 chance sur 10, les slimes qui ne sont pas des boss.
    /// </summary>
    public class DemonPotion : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 20;

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 36;
            Item.useTime = Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.UseSound = SoundID.Item3;
            Item.useTurn = true;
            Item.consumable = true;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 1);
            Item.buffType = ModContent.BuffType<Buffs.Evil>();
            Item.buffTime = 100 * 3600;
        }
    }

    public class DemonPotionDrop : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.aiStyle != NPCAIStyleID.Slime || npc.boss)
                return;
            var rule = ItemDropRule.Common(ModContent.ItemType<DemonPotion>(), 10);
            npcLoot.Add(rule);
            WikiLoot.Keep.Add(rule);
        }

        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            if (player.HasBuff(ModContent.BuffType<Buffs.Evil>()))
            {
                spawnRate = Math.Max(1, spawnRate / 11);
                maxSpawns *= 11;
            }
        }
    }
}

namespace CalamityFandom.Content.Buffs
{
    public class Evil : ModBuff
    {
        public override string Texture => "Terraria/Images/Buff_" + BuffID.Battle;
    }
}

namespace CalamityFandom.Content.Items.PermanentBoosters
{
    /// <summary>
    /// Wiki : Expired Medication.txt — amélioration permanente (Plaguebringer Goliath, Death) : +100 PV max, -3 de
    /// régénération, +25 % de temps de recharge des potions de soin ; seulement après toutes les Life Fruits.
    /// </summary>
    public class ExpiredMedication : WikiBooster
    {
        protected override int Rarity => ItemRarityID.Lime;
        protected override int SellCopper => 0;

        public override bool CanUseItem(Player player) => base.CanUseItem(player) && player.ConsumedLifeFruit >= Player.LifeFruitMax;
    }

    /// <summary>
    /// Butin : la page de la Plaguebringer Goliath cite l'objet sans chance, la sienne dit « Death Mode-exclusive ».
    /// [choix] 100 % en Death, sur le boss (pas dans le sac), gardé en Full Fandom.
    /// </summary>
    public class ExpiredMedicationDrop : GlobalNPC
    {
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
            => entity.type == ModContent.NPCType<CalamityMod.NPCs.PlaguebringerGoliath.PlaguebringerGoliath>();

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            var rule = new LeadingConditionRule(CalamityMod.DropHelper.If(_ => CalamityMod.World.CalamityWorld.death));
            rule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<ExpiredMedication>()));
            npcLoot.Add(rule);
            WikiLoot.Keep.Add(rule);
        }
    }

    public class ExpiredMedicationPlayer : ModPlayer
    {
        private bool Has => Player.GetModPlayer<WikiBoosterPlayer>().Used.Contains(nameof(ExpiredMedication));

        public override void ModifyMaxStats(out StatModifier health, out StatModifier mana)
        {
            health = StatModifier.Default;
            mana = StatModifier.Default;
            if (Has)
                health.Base += 100;
        }

        public override void PostUpdateEquips()
        {
            if (!Has)
                return;
            Player.potionDelayTime = (int)(Player.potionDelayTime * 1.25f);
        }

        public override void UpdateLifeRegen()
        {
            if (Has)
                Player.lifeRegen -= 6;
        }
    }
}

namespace CalamityFandom.Content.Items.Materials
{
    /// <summary>
    /// Wiki : Desolate Flah.txt — seulement en jetant ensemble dans la lave une Merge Flah, un Energy Core, un Yharon Soul
    /// Fragment, une Alloy Bar et une Meld Bar ; à la place de la Merge Flah, une Flah, Washed, Blessed ou Cosmic Flah
    /// détruit tout ce qui a été jeté. [choix] Objets à moins de 4 blocs les uns des autres, dans la lave en même temps.
    /// </summary>
    public class DesolateFlah : WikiPlainItem
    {
        protected override int Rarity => ModContent.RarityType<BurnishedAuric>();
        protected override int SellCopper => 0;
    }

    public class DesolateFlahLava : GlobalItem
    {
        private static int[] others, wrongFlah;

        private static void Build()
        {
            if (others != null)
                return;
            others = new[] { "Energy Core", "Yharon Soul Fragment", "Alloy Bar", "Meld Bar" }.Select(WikiRecipes.ResolveItem).ToArray();
            wrongFlah = new[] { "Flah", "Washed Flah", "Blessed Flah", "Cosmic Flah" }.Select(WikiRecipes.ResolveItem).Where(t => t > 0).ToArray();
        }

        public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
        {
            if (!item.lavaWet || Main.netMode == NetmodeID.MultiplayerClient || Main.GameUpdateCount % 20 != 0)
                return;
            Build();
            bool merge = item.type == ModContent.ItemType<MergeFlah>();
            if ((!merge && !wrongFlah.Contains(item.type)) || others.Any(t => t <= 0))
                return;
            var found = new List<Item>();
            foreach (int type in others)
            {
                Item other = Main.item.FirstOrDefault(i => i.active && i.type == type && i.lavaWet && i.Distance(item.Center) < 64f);
                if (other == null)
                    return;
                found.Add(other);
            }
            found.Add(item);
            foreach (Item i in found)
            {
                if (--i.stack <= 0)
                    i.TurnToAir();
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, i.whoAmI);
            }
            SoundEngine.PlaySound(merge ? SoundID.Item4 : SoundID.Item14, item.Center);
            for (int d = 0; d < 20; d++)
                Dust.NewDustPerfect(item.Center, merge ? DustID.PinkTorch : DustID.Smoke, Main.rand.NextVector2Circular(4f, 4f), 0, default, 1.5f).noGravity = true;
            if (merge)
            {
                int n = Item.NewItem(item.GetSource_Misc("DesolateFlah"), item.Center, ModContent.ItemType<DesolateFlah>());
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, n, 1f);
            }
        }
    }

    public class WikiItems1120dChecks : ModSystem
    {
        public override void Load()
        {
            SelfTest.ExtraChecks.Add(t =>
            {
                var rates = new List<DropRateInfo>();
                foreach (var r in Main.ItemDropsDB.GetRulesForNPCID(ModContent.NPCType<CalamityMod.NPCs.PlaguebringerGoliath.PlaguebringerGoliath>(), false))
                    r.ReportDroprates(rates, new DropRateInfoChainFeed(1f));
                t.Expect(rates.Any(r => r.itemId == ModContent.ItemType<PermanentBoosters.ExpiredMedication>()), "la Plaguebringer Goliath lâche l'Expired Medication (Death)");
                rates.Clear();
                foreach (var r in Main.ItemDropsDB.GetRulesForNPCID(NPCID.BlueSlime, false))
                    r.ReportDroprates(rates, new DropRateInfoChainFeed(1f));
                t.Expect(rates.Any(r => r.itemId == ModContent.ItemType<Potions.DemonPotion>()), "les slimes lâchent la Demon Potion");
                t.Expect(new[] { "Energy Core", "Yharon Soul Fragment", "Alloy Bar", "Meld Bar" }.All(n => WikiRecipes.ResolveItem(n) > 0), "ingrédients de la Desolate Flah");
            });
        }
    }
}
