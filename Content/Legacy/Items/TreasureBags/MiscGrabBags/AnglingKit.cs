// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.TreasureBags.MiscGrabBags
{
    /// <summary>
    /// Sacs d'objets de pêche des boss aquatiques (Sandy Angling Kit, Bleached Angling Kit), retirés de Calamity après
    /// 2.0.1.005. Chaque kit donne sa table de la page du wiki : lignes (objets, 1 sur N en normal, 1 sur N en Expert,
    /// quantité).
    /// </summary>
    public abstract class AnglingKit : ModItem
    {
        protected record Row(int[] Items, int Normal, int Expert, int Min = 1, int Max = 1, IItemDropRuleCondition Condition = null);

        protected abstract Row[] Table { get; }

        protected abstract int Rarity { get; }

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 10;
        }

        public override void SetDefaults()
        {
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.width = 24;
            Item.height = 24;
            Item.rare = Rarity;
            Item.value = 0;
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.GoodieBags;
        }

        public override bool CanRightClick() => true;

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            var normal = new LeadingConditionRule(new Conditions.NotExpert());
            var expert = new LeadingConditionRule(new Conditions.IsExpert());
            foreach (var row in Table)
            {
                foreach (int item in row.Items)
                {
                    normal.OnSuccess(Drop(row, item, row.Normal));
                    expert.OnSuccess(Drop(row, item, row.Expert));
                }
            }
            itemLoot.Add(normal);
            itemLoot.Add(expert);
        }

        private static IItemDropRule Drop(Row row, int item, int denominator) => row.Condition == null
            ? ItemDropRule.Common(item, denominator, row.Min, row.Max)
            : ItemDropRule.ByCondition(row.Condition, item, denominator, row.Min, row.Max);

        protected static readonly int[] Gear = { ItemID.HighTestFishingLine, ItemID.TackleBox, ItemID.AnglerEarring };
        protected static readonly int[] Tools = { ItemID.FishermansGuide, ItemID.WeatherRadio, ItemID.Sextant };
        protected static readonly int[] Clothes = { ItemID.AnglerHat, ItemID.AnglerVest, ItemID.AnglerPants };
        protected static readonly int[] Potions = { ItemID.FishingPotion, ItemID.SonarPotion, ItemID.CratePotion };

        protected class SkeletronDefeated : IItemDropRuleCondition
        {
            public bool CanDrop(DropAttemptInfo info) => NPC.downedBoss3;
            public bool CanShowItemDropInUI() => true;
            public string GetConditionDescription() => "If Skeletron has been defeated";
        }
    }
}
