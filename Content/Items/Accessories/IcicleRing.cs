using CalamityMod.Rarities;
using Terraria;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Items.Accessories
{
    /// <summary>
    /// Wiki : Icicle Ring.txt — la vie baisse de 15 PV/s jusqu'au tiers de la vie maximale (arrondi au-dessus) ; en échange,
    /// les sorts ne coûtent plus de mana et la magie fait 50 % de dégâts en plus. Lâché à 1 % par les Frost Imps.
    /// TODO : Frost Imp (ennemi inventé, pas encore créé) : l'anneau n'a pas encore de source.
    /// </summary>
    public class IcicleRing : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 30;
            Item.accessory = true;
            Item.value = Item.sellPrice(platinum: 1, gold: 9, silver: 9, copper: 7);
            Item.rare = ModContent.RarityType<BurnishedAuric>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.manaCost -= 1f;
            player.GetDamage(DamageClass.Magic) += 0.5f;
            player.GetModPlayer<IcicleRingPlayer>().Active = true;
        }
    }

    public class IcicleRingPlayer : ModPlayer
    {
        public bool Active;

        public override void ResetEffects() => Active = false;

        public override void UpdateBadLifeRegen()
        {
            // 15 PV/s tant que la vie dépasse le tiers de la vie maximale
            if (!Active || Player.statLife <= (Player.statLifeMax2 + 2) / 3)
                return;
            if (Player.lifeRegen > 0)
                Player.lifeRegen = 0;
            Player.lifeRegenTime = 0;
            Player.lifeRegen -= 30;
        }
    }
}
