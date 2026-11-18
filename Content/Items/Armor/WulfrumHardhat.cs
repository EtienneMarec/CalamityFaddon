using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Items.Armor
{
    /// <summary>
    /// Wiki : Wulfrum Hardhat.txt — casque d'avant Hardmode, 2 défense, immunité aux pièges ; obtenu en apprivoisant The
    /// Inventors (récompense de la page Taming). [choix] Pièges = projectiles marqués « trap » par le jeu (fléchettes,
    /// lances, boules à pointes, flammes, explosifs des pièges) et rochers.
    /// [technique] Planche portée fabriquée à partir de l'image « Equipped » du wiki (chapeau recopié sur chaque image).
    /// </summary>
    [AutoloadEquip(EquipType.Head)]
    public class WulfrumHardhat : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 16;
            Item.defense = 2;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 32);
        }

        public override void UpdateEquip(Player player) => player.GetModPlayer<HardhatPlayer>().TrapImmune = true;
    }

    public class HardhatPlayer : ModPlayer
    {
        public bool TrapImmune;

        public override void ResetEffects() => TrapImmune = false;

        public override bool CanBeHitByProjectile(Projectile proj) =>
            !(TrapImmune && (proj.trap || proj.type == ProjectileID.Boulder || proj.type == ProjectileID.RollingCactus));
    }
}
