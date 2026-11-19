// Adapté de Calamity Mod (version de 2022-07, 14d4b448ea), © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using CalamityMod;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

// Ancienne Wulfrum armor (retirée avant 2.0.1.005) : ses pièces redirigent vers « Wulfrum armor » sur le wiki (qui décrit
// l'armure actuelle de Calamity) mais ont toujours leurs recettes dans les registres du wiki. Valeurs de la version de
// 2022-07 ; recettes du wiki (Wulfrum Bar, Energy Core).

namespace CalamityFandom.Content.Legacy.Items.Armor.Wulfrum
{
    /// <summary>Casque de l'ancienne Wulfrum armor : bonus d'ensemble commun (+3 défense, +5 sous 50 % de vie) et le sien.</summary>
    public abstract class WulfrumLegacyHead : ModItem
    {
        protected abstract int Defense { get; }

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = Item.height = 18;
            Item.value = Item.buyPrice(silver: 75);
            Item.rare = ItemRarityID.Blue;
            Item.defense = Defense;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
            => body.type == ModContent.ItemType<WulfrumArmor>() && legs.type == ModContent.ItemType<WulfrumLeggings>();

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = Language.GetTextValue("Mods.CalamityFandom.WulfrumLegacy.SetBonus") + ExtraSetBonus();
            player.statDefense += 3;
            if (player.statLife <= (int)(player.statLifeMax2 * 0.5))
                player.statDefense += 5;
            SetBonus(player);
        }

        protected virtual string ExtraSetBonus() => "";

        protected virtual void SetBonus(Player player)
        {
        }
    }

    /// <summary>Wiki : Wulfrum armor.txt — Wulfrum Helm (mêlée) : 3 défense, +10 % de dégâts de mêlée ; ensemble : +100 d'aggro.</summary>
    [AutoloadEquip(EquipType.Head)]
    public class WulfrumHeadMelee : WulfrumLegacyHead
    {
        protected override int Defense => 3;

        public override void UpdateEquip(Player player) => player.GetDamage<MeleeDamageClass>() += 0.1f;

        protected override string ExtraSetBonus() => "\n" + Language.GetTextValue("Mods.CalamityFandom.WulfrumLegacy.MeleeSetBonus");

        protected override void SetBonus(Player player) => player.aggro += 100;
    }

    /// <summary>Wiki : Wulfrum armor.txt — Wulfrum Headgear (distance) : 2 défense, +10 % de dégâts à distance.</summary>
    [AutoloadEquip(EquipType.Head)]
    public class WulfrumHeadRanged : WulfrumLegacyHead
    {
        protected override int Defense => 2;

        public override void UpdateEquip(Player player) => player.GetDamage<RangedDamageClass>() += 0.1f;
    }

    /// <summary>Wiki : Wulfrum armor.txt — Wulfrum Visage (magie) : 1 défense, +10 % de dégâts magiques.</summary>
    [AutoloadEquip(EquipType.Head)]
    public class WulfrumHeadMagic : WulfrumLegacyHead
    {
        protected override int Defense => 1;

        public override void UpdateEquip(Player player) => player.GetDamage<MagicDamageClass>() += 0.1f;
    }

    /// <summary>
    /// Wiki : Wulfrum armor.txt — Wulfrum Mask (spy) : 1 défense, +10 % de dégâts de spy ; ensemble : furtivité (50 au plus).
    /// </summary>
    [AutoloadEquip(EquipType.Head)]
    public class WulfrumHeadRogue : WulfrumLegacyHead
    {
        protected override int Defense => 1;

        public override void UpdateEquip(Player player) => player.GetDamage<ThrowingDamageClass>() += 0.1f;

        protected override string ExtraSetBonus() => "\n" + Language.GetTextValue("Mods.CalamityFandom.WulfrumLegacy.RogueSetBonus");

        protected override void SetBonus(Player player)
        {
            var cal = player.Calamity();
            cal.rogueStealthMax += 0.5f;
            cal.wearingRogueArmor = true;
        }
    }

    /// <summary>Wiki : Wulfrum armor.txt — Wulfrum Helmet (invocation) : 0 défense, +10 % de dégâts des sbires ; ensemble : +1 sbire.</summary>
    [AutoloadEquip(EquipType.Head)]
    public class WulfrumHeadSummon : WulfrumLegacyHead
    {
        protected override int Defense => 0;

        public override void UpdateEquip(Player player) => player.GetDamage<SummonDamageClass>() += 0.1f;

        protected override string ExtraSetBonus() => "\n" + Language.GetTextValue("Mods.CalamityFandom.WulfrumLegacy.SummonSetBonus");

        protected override void SetBonus(Player player) => player.maxMinions++;
    }

    /// <summary>Wiki : Wulfrum armor.txt — Wulfrum Armor : 2 défense, +3 % de chances de critique.</summary>
    [AutoloadEquip(EquipType.Body)]
    public class WulfrumArmor : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = Item.height = 18;
            Item.value = Item.buyPrice(silver: 75);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 2;
        }

        public override void UpdateEquip(Player player) => player.GetCritChance<GenericDamageClass>() += 3;
    }

    /// <summary>Wiki : Wulfrum armor.txt — Wulfrum Leggings : 1 défense, +5 % de vitesse.</summary>
    [AutoloadEquip(EquipType.Legs)]
    public class WulfrumLeggings : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = Item.height = 18;
            Item.value = Item.buyPrice(silver: 75);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 1;
        }

        public override void UpdateEquip(Player player) => player.moveSpeed += 0.05f;
    }
}
