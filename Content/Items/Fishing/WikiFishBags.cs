using CalamityMod;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

// Poissons-sacs inventés par le wiki (19/11) : pêchés dans une zone, ouverts d'un clic droit. Valeurs des pages.

namespace CalamityFandom.Content.Items.Fishing
{
    public abstract class WikiFishBag : ModItem
    {
        protected abstract int Rarity { get; }
        protected abstract int SellCopper { get; }

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 10;
            ItemID.Sets.CanBePlacedOnWeaponRacks[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.width = Item.height = 40;
            Item.rare = Rarity;
            Item.value = SellCopper * 5;
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup) =>
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.GoodieBags;

        public override bool CanRightClick() => true;
    }

    /// <summary>Wiki : Front Gar.txt — Sulphurous Sea après Polterghast, 10 % ; 5 à 15 Reaper Teeth.</summary>
    public class FrontGar : WikiFishBag
    {
        protected override int Rarity => ModContent.RarityType<CalamityMod.Rarities.PureGreen>();
        protected override int SellCopper => 50000;

        public override void ModifyItemLoot(ItemLoot itemLoot) => itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<ReaperTooth>(), 1, 5, 15));
    }

    /// <summary>Wiki : Rear Gar.txt — Jungle après Providence, 10 % ; 5 à 15 Tarragon Ore (Uelibloom Ore).</summary>
    public class RearGar : WikiFishBag
    {
        protected override int Rarity => ModContent.RarityType<CalamityMod.Rarities.Turquoise>();
        protected override int SellCopper => 10000;

        public override void ModifyItemLoot(ItemLoot itemLoot) => itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<UelibloomOre>(), 1, 5, 15));
    }

    /// <summary>Wiki : Side Gar.txt — Planetoids et îles du ciel après le Moon Lord, 10 % ; 5 à 15 Galactica Singularities.</summary>
    public class SideGar : WikiFishBag
    {
        protected override int Rarity => ItemRarityID.Red;
        protected override int SellCopper => 10000;

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            if (ModContent.TryFind<ModItem>("CalamityFandom", "GalacticaSingularity", out var singularity))
                itemLoot.Add(ItemDropRule.Common(singularity.Type, 1, 5, 15));
        }
    }

    /// <summary>
    /// Wiki : Flahr.txt — eau du Brimstone Crag après Calamitas et Plantera, pendant une Blood Moon ou une éclipse, 10 % ;
    /// 1 à 5 Flah (TODO : 5 à 15 Flah Seeds, pas encore dans le mod).
    /// </summary>
    public class Flahr : WikiFishBag
    {
        protected override int Rarity => ItemRarityID.Lime;
        protected override int SellCopper => 50000;

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            if (ModContent.TryFind<ModItem>("CalamityFandom", "Flah", out var flah))
                itemLoot.Add(ItemDropRule.Common(flah.Type, 1, 1, 5));
        }
    }

    /// <summary>Wiki : Tar Gar.txt — pêché souvent dans la Corruption ou le Crimson la nuit ; clic droit : lance une Slime Rain.</summary>
    public class TarGar : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 3;

        public override void SetDefaults()
        {
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.width = 46;
            Item.height = 42;
            Item.rare = ModContent.RarityType<CalamityMod.Rarities.BurnishedAuric>();
            Item.value = 0;
        }

        public override bool CanRightClick() => !Main.slimeRain;

        public override void RightClick(Player player)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                // Le serveur lance la pluie
                var p = ModContent.GetInstance<CalamityFandom>().GetPacket();
                p.Write((byte)CalamityFandom.ExtraPacket.SlimeRain);
                p.Send();
                return;
            }
            Main.StartSlimeRain(true);
        }
    }

    /// <summary>Pêche des poissons-sacs (pages) : 10 % des prises peu communes dans leur zone ; Tar Gar commun (1 sur 4).</summary>
    public class WikiFishCatch : ModPlayer
    {
        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            if (attempt.inLava || attempt.inHoney || attempt.crate || npcSpawn > 0 || itemDrop == attempt.questFish)
                return;
            var cal = Player.Calamity();
            if (attempt.common && !Main.dayTime && (Player.ZoneCorrupt || Player.ZoneCrimson) && Main.rand.NextBool(4))
            {
                itemDrop = ModContent.ItemType<TarGar>();
                return;
            }
            if (!attempt.uncommon || !Main.rand.NextBool(10))
                return;
            if (cal.ZoneSulphur && DownedBossSystem.downedPolterghast)
                itemDrop = ModContent.ItemType<FrontGar>();
            else if (Player.ZoneJungle && DownedBossSystem.downedProvidence)
                itemDrop = ModContent.ItemType<RearGar>();
            else if (Player.ZoneSkyHeight && NPC.downedMoonlord)
                itemDrop = ModContent.ItemType<SideGar>();
            else if (cal.ZoneCalamity && DownedBossSystem.downedCalamitasClone && NPC.downedPlantBoss && (Main.bloodMoon || Main.eclipse))
                itemDrop = ModContent.ItemType<Flahr>();
        }
    }
}
