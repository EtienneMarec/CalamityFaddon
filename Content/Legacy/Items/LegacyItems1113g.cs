// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using CalamityMod.Events;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

// Lot du 13/11 (fin) : Old Power Cell, Shroomite Visage. Code de 2.0.1.005, valeurs des infobox ; recettes du wiki.

namespace CalamityFandom.Content.Legacy.Items.SummonItems
{
    /// <summary>
    /// Wiki : Old Power Cell.txt — invoque Golem quand on l'utilise dans le temple de la jungle (pas à l'autel), même avant
    /// Plantera ; non consommé.
    /// </summary>
    public class OldPowerCell : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            NPCID.Sets.MPAllowedEnemies[NPCID.Golem] = true;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 15; // comme la Lihzahrd Power Cell
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.rare = ItemRarityID.Lime;
            Item.useTime = Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup) =>
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossItem;

        public override bool CanUseItem(Player player)
        {
            // Dans le temple : sous la surface, devant un mur de briques lihzahrd naturel
            if (player.Center.Y <= Main.worldSurface * 16.0)
                return false;
            Tile tile = Framing.GetTileSafely(player.Center.ToTileCoordinates());
            return tile.WallType == WallID.LihzahrdBrickUnsafe && !NPC.AnyNPCs(NPCID.Golem) && !BossRushEvent.BossRushActive;
        }

        public override bool? UseItem(Player player)
        {
            SoundEngine.PlaySound(SoundID.Roar, player.Center);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.SpawnOnPlayer(player.whoAmI, NPCID.Golem);
            else
                NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: NPCID.Golem);
            return true;
        }
    }
}

namespace CalamityFandom.Content.Legacy.Items.Armor
{
    /// <summary>
    /// Wiki : Shroomite Visage.txt — casque en shroomite : 11 défense, +25 % de dégâts à distance pour les lance-flammes
    /// (armes qui utilisent du gel) ; ensemble avec le plastron et les jambières en shroomite : furtivité à l'arrêt.
    /// </summary>
    [AutoloadEquip(EquipType.Head)]
    public class ShroomiteVisage : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 20;
            Item.value = Item.sellPrice(gold: 6);
            Item.rare = ItemRarityID.Yellow;
            Item.defense = 11;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) =>
            body.type == ItemID.ShroomiteBreastplate && legs.type == ItemID.ShroomiteLeggings;

        public override void UpdateArmorSet(Player player)
        {
            player.shroomiteStealth = true;
            player.setBonus = Language.GetTextValue("ArmorSetBonus.Shroomite");
        }

        public override void UpdateEquip(Player player) => player.GetModPlayer<FlamethrowerBoostPlayer>().Boost = true;
    }

    /// <summary>+25 % de dégâts pour les armes à distance qui utilisent du gel (Shroomite Visage).</summary>
    public class FlamethrowerBoostPlayer : ModPlayer
    {
        public bool Boost;

        public override void ResetEffects() => Boost = false;

        public override void ModifyWeaponDamage(Item item, ref StatModifier damage)
        {
            if (Boost && item.useAmmo == AmmoID.Gel && item.CountsAsClass(DamageClass.Ranged))
                damage += 0.25f;
        }
    }
}
