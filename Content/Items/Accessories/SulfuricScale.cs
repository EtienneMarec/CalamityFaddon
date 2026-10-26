using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Items.Accessories
{
    /// <summary>
    /// Wiki : Sulfuric Scale.txt — covering inventé par le wiki (distinct du matériau Sulphuric Scale), lâché par le
    /// Nuclear Terror (WikiLoot) : immunité à Poisoned, Venom et Sulphuric Poisoning ; sous un debuff qui fait perdre de
    /// la vie : +10 % de dégâts et +5 % de critique ; par tranche de 25 % de vie perdue : +6 % et +3 % (au plus 18 % et
    /// 9 %, sous 25 % de vie). Rareté 13, vente 26 or.
    /// </summary>
    public class SulfuricScale : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 26;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.value = Item.sellPrice(gold: 26);
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[BuffID.Poisoned] = true;
            player.buffImmune[BuffID.Venom] = true;
            player.buffImmune[ModContent.BuffType<SulphuricPoisoning>()] = true;

            float bonus = 0f;
            int crit = 0;
            if (player.GetModPlayer<SulfuricScalePlayer>().LosingLife)
            {
                bonus += 0.1f;
                crit += 5;
            }
            int tiers = Math.Clamp((int)((1f - (float)player.statLife / player.statLifeMax2) / 0.25f), 0, 3);
            bonus += tiers * 0.06f;
            crit += tiers * 3;
            player.GetDamage(DamageClass.Generic) += bonus;
            player.GetCritChance(DamageClass.Generic) += crit;
        }
    }

    public class SulfuricScalePlayer : ModPlayer
    {
        /// <summary>[technique] Un debuff fait perdre de la vie (régénération négative au tick précédent).</summary>
        public bool LosingLife;

        public override void UpdateBadLifeRegen() => LosingLife = Player.lifeRegen < 0;
    }
}
