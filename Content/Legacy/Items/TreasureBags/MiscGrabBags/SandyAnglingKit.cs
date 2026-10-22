// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.TreasureBags.MiscGrabBags
{
    /// <summary>
    /// Wiki : Sandy Angling Kit.txt — sac d'objets de pêche lâché par le Desert Scourge (butin du wiki, WikiLoot),
    /// retiré de Calamity après 2.0.1.005. Chances identiques au vieux code et à la page ({{dv|normal|expert}}) ;
    /// pile de 9999 et sans valeur, comme l'infobox.
    /// </summary>
    public class SandyAnglingKit : ModItem
    {
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
            Item.rare = ItemRarityID.Blue;
            Item.value = 0;
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.GoodieBags;
        }

        public override bool CanRightClick() => true;

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            // Chances différentes en normal et en Expert : 1 sur 15/12, 10/9, 5/4, 20/18
            itemLoot.Add(AddAll(new LeadingConditionRule(new Conditions.NotExpert()), 15, 10, 5, 20));
            itemLoot.Add(AddAll(new LeadingConditionRule(new Conditions.IsExpert()), 12, 9, 4, 18));
        }

        private static LeadingConditionRule AddAll(LeadingConditionRule set, int gear, int tools, int clothes, int bugNet)
        {
            foreach (int item in new[] { ItemID.HighTestFishingLine, ItemID.TackleBox, ItemID.AnglerEarring })
                set.OnSuccess(ItemDropRule.Common(item, gear));
            foreach (int item in new[] { ItemID.FishermansGuide, ItemID.WeatherRadio, ItemID.Sextant })
                set.OnSuccess(ItemDropRule.Common(item, tools));
            foreach (int item in new[] { ItemID.AnglerHat, ItemID.AnglerVest, ItemID.AnglerPants })
                set.OnSuccess(ItemDropRule.Common(item, clothes));
            foreach (int item in new[] { ItemID.FishingPotion, ItemID.SonarPotion, ItemID.CratePotion })
                set.OnSuccess(ItemDropRule.Common(item, clothes, 2, 3));
            // « If Skeletron has been defeated »
            set.OnSuccess(ItemDropRule.ByCondition(new SkeletronDefeated(), ItemID.GoldenBugNet, bugNet));
            return set;
        }

        private class SkeletronDefeated : IItemDropRuleCondition
        {
            public bool CanDrop(DropAttemptInfo info) => NPC.downedBoss3;
            public bool CanShowItemDropInUI() => true;
            public string GetConditionDescription() => "If Skeletron has been defeated";
        }
    }
}
