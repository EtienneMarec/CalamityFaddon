// Sprites repris de Calamity Mod (Level Meters retirées en septembre 2022), © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
using System.Collections.Generic;
using CalamityFandom.Common.Proficiency;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.Accessories
{
    /// <summary>
    /// Wiki : Level Meters.txt — coverings vendus 5 or par le Goblin Tinkerer : montrent le niveau de proficiency de leur
    /// classe et ses bonus ; équipés, recharge du gain divisée par deux et 10 % de chances de double gain.
    /// </summary>
    public abstract class LevelMeter : ModItem
    {
        public abstract ProficiencyClass Class { get; }

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 24;
            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual) => player.GetModPlayer<ProficiencyPlayer>().Meter[(int)Class] = true;

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var prof = Main.LocalPlayer.GetModPlayer<ProficiencyPlayer>();
            int level = prof.Level[(int)Class], value = prof.Value[(int)Class];
            int next = level < ProficiencyData.Levels.Length ? ProficiencyData.Levels[level].Required : -1;
            string key = "Mods.CalamityFandom.Proficiency.Meter.";
            tooltips.Add(new TooltipLine(Mod, "ProficiencyLevel", Language.GetTextValue(key + "Level", level, value)
                + (next > 0 ? Language.GetTextValue(key + "Next", next) : Language.GetTextValue(key + "Maxed"))));
            var bonus = ProficiencyData.BonusOf(level);
            if (bonus == null)
                return;
            var parts = new List<string> { Language.GetTextValue(key + "Damage", (int)(bonus.Damage * 100)) };
            if (Class != ProficiencyClass.Summon)
                parts.Add(Language.GetTextValue(key + "Crit", (int)(bonus.Crit * 100)));
            parts.Add(Class switch
            {
                ProficiencyClass.Melee => Language.GetTextValue(key + "MeleeSpeed", (int)(bonus.MeleeSpeed * 100)),
                ProficiencyClass.Ranged => Language.GetTextValue(key + "Ammo", (int)(bonus.AmmoConsumption * 100)),
                ProficiencyClass.Magic => Language.GetTextValue(key + "Mana", (int)(bonus.ManaCost * 100)),
                ProficiencyClass.Summon => Language.GetTextValue(key + "Minions", bonus.MinionKnockback, bonus.MinionSlots),
                _ => Language.GetTextValue(key + "Velocity", (int)(bonus.ProjectileSpeed * 100)),
            });
            tooltips.Add(new TooltipLine(Mod, "ProficiencyBonus", string.Join(", ", parts)));
        }
    }

    /// <summary>Wiki : Melee Level Meter.txt</summary>
    public class MeleeLevelMeter : LevelMeter { public override ProficiencyClass Class => ProficiencyClass.Melee; }

    /// <summary>Wiki : Ranged Level Meter.txt</summary>
    public class RangedLevelMeter : LevelMeter { public override ProficiencyClass Class => ProficiencyClass.Ranged; }

    /// <summary>Wiki : Magic Level Meter.txt</summary>
    public class MagicLevelMeter : LevelMeter { public override ProficiencyClass Class => ProficiencyClass.Magic; }

    /// <summary>Wiki : Summon Level Meter.txt</summary>
    public class SummonLevelMeter : LevelMeter { public override ProficiencyClass Class => ProficiencyClass.Summon; }

    /// <summary>Wiki : Spy Level Meter.txt</summary>
    public class SpyLevelMeter : LevelMeter { public override ProficiencyClass Class => ProficiencyClass.Spy; }

    /// <summary>Le Goblin Tinkerer vend les Level Meters (page Level Meters).</summary>
    public class LevelMeterShop : GlobalNPC
    {
        public override void ModifyShop(NPCShop shop)
        {
            if (shop.NpcType != NPCID.GoblinTinkerer)
                return;
            shop.Add<MeleeLevelMeter>();
            shop.Add<RangedLevelMeter>();
            shop.Add<MagicLevelMeter>();
            shop.Add<SummonLevelMeter>();
            shop.Add<SpyLevelMeter>();
        }
    }
}
