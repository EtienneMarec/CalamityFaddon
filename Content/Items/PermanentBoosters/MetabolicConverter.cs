using CalamityMod;
using CalamityMod.World;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityFandom.Content.Items.PermanentBoosters
{
    /// <summary>
    /// Wiki : Metabolic Converter.txt — objet inventé par le wiki, dans le sac de The Inventors (Crabulon) en Revengeance
    /// (page Treasure Bag) : débloque pour toujours la Rage et l'Adrenaline, une seule fois. [validé 31/10] Dans les deux
    /// modes : sans lui, les deux jauges restent verrouillées en Revengeance (sauf Shattered Community / Draedon's Heart,
    /// qui les activent déjà dans Calamity).
    /// </summary>
    public class MetabolicConverter : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = Item.height = 30;
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;
            Item.rare = ItemRarityID.Green;
            Item.value = 0;
        }

        public override bool CanUseItem(Player player) => !player.GetModPlayer<MetabolicPlayer>().Converted;

        public override bool? UseItem(Player player)
        {
            player.GetModPlayer<MetabolicPlayer>().Converted = true;
            if (player.whoAmI == Main.myPlayer)
                SoundEngine.PlaySound(SoundID.Item4, player.Center);
            return true;
        }
    }

    public class MetabolicPlayer : ModPlayer
    {
        public bool Converted;

        public override void SaveData(TagCompound tag) => tag["metabolicConverter"] = Converted;

        public override void LoadData(TagCompound tag) => Converted = tag.GetBool("metabolicConverter");

        // Après les accessoires (Shattered Community, Draedon's Heart) et avant la mise à jour des jauges
        public override void PostUpdateEquips()
        {
            if (Converted || !CalamityWorld.revenge)
                return;
            var cal = Player.Calamity();
            if (!cal.shatteredCommunity)
                cal.externalRageEnabled = false;
            if (!cal.draedonsHeart)
                cal.externalAdrenalineEnabled = false;
        }
    }
}
