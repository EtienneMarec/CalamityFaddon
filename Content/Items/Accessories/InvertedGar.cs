using CalamityFandom.Content.Buffs;
using CalamityMod;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Items.Accessories
{
    /// <summary>
    /// Wiki : Inverted Gar.txt — covering inventé par le wiki, pêché dans l'Abysse (5 %, toute puissance de pêche) : être
    /// touché par un ennemi donne Gar Mana Boost 1 s (pas de mana consommé ; pas avec les pics, Notes). Rareté 3, vente
    /// 80 argent.
    /// </summary>
    public class InvertedGar : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 48;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(silver: 80);
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual) => player.GetModPlayer<InvertedGarPlayer>().Equipped = true;
    }

    public class InvertedGarPlayer : ModPlayer
    {
        public bool Equipped;

        public override void ResetEffects() => Equipped = false;

        public override void OnHurt(Player.HurtInfo info)
        {
            // Seulement les coups d'ennemis (PNJ ou projectile hostile) : pas les pics ni les chutes
            if (Equipped && (info.DamageSource.SourceNPCIndex >= 0 || info.DamageSource.SourceProjectileType > 0))
                Player.AddBuff(ModContent.BuffType<GarManaBoost>(), 60);
        }

        public override void ModifyManaCost(Item item, ref float reduce, ref float mult)
        {
            if (Player.HasBuff<GarManaBoost>())
                mult = 0f;
        }

        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Microsoft.Xna.Framework.Vector2 sonarPosition)
        {
            if (!attempt.inLava && !attempt.inHoney && Player.InAbyss() && attempt.rolledItemDrop > 0 && Main.rand.NextFloat() < 0.05f)
                itemDrop = ModContent.ItemType<InvertedGar>();
        }
    }
}
