// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using System.Linq;
using CalamityMod.CalPlayer;
using CalamityMod.Events;
using CalamityMod.Systems;
using CalamityMod.UI.ModeIndicator;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

// Objets de difficulté retirés avant 2.0.1.005 (versions mensuelles de 2022) : ils activent ou désactivent la difficulté
// par le sélecteur de Calamity (ModeIndicatorUI.SwitchToDifficulty), qui les a remplacés.

namespace CalamityFandom.Content.Legacy.Items.DifficultyItems
{
    public abstract class DifficultyItem : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = Item.height = 28;
            Item.useTime = Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ItemRarityID.Expert;
            Item.expert = true;
            Item.UseSound = SoundID.Item119;
            Item.consumable = false;
        }

        protected static T Mode<T>() where T : DifficultyMode => DifficultyModeSystem.Difficulties.OfType<T>().FirstOrDefault();

        /// <summary>Change de difficulté, sauf pendant un boss ou le Boss Rush (message de Calamity).</summary>
        protected static void Switch(DifficultyMode mode)
        {
            if (mode == null || Main.netMode == NetmodeID.Server)
                return;
            if (CalamityPlayer.areThereAnyDamnBosses || BossRushEvent.BossRushActive)
            {
                Main.NewText(Language.GetTextValue("Mods.CalamityMod.UI.ChangingTheRules"), Color.Crimson);
                return;
            }
            ModeIndicatorUI.SwitchToDifficulty(mode, true);
        }

        // Fabriqué à un autel, sans ingrédient (page) ; la recette du wiki n'a pas d'ingrédient à lire pour WikiRecipes
        public override void AddRecipes() => CreateRecipe().AddTile(TileID.DemonAltar).Register();
    }

    /// <summary>Wiki : Revengeance.txt — active ou désactive le mode Revengeance, seulement en Expert ; dans le Starter Bag en Expert.</summary>
    public class Revenge : DifficultyItem
    {
        public override bool CanUseItem(Player player) => Main.expertMode || CalamityWorld.revenge;

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
                Switch(CalamityWorld.revenge ? Mode<ExpertDifficulty>() : Mode<RevengeanceDifficulty>());
            return true;
        }
    }

    /// <summary>Wiki : Death.txt — active ou désactive le mode Death, seulement en Revengeance ; dans le Starter Bag en Expert.</summary>
    public class Death : DifficultyItem
    {
        public override bool CanUseItem(Player player) => CalamityWorld.revenge || CalamityWorld.death;

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
                Switch(CalamityWorld.death ? Mode<RevengeanceDifficulty>() : Mode<DeathDifficulty>());
            return true;
        }
    }

    /// <summary>Le Starter Bag de Calamity donne les deux objets en Expert (pages).</summary>
    public class StarterBagDifficultyItems : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) =>
            entity.type == ModContent.ItemType<CalamityMod.Items.TreasureBags.MiscGrabBags.StarterBag>();

        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            var expert = new LeadingConditionRule(new Conditions.IsExpert());
            expert.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Revenge>()));
            expert.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Death>()));
            itemLoot.Add(expert);
        }
    }
}
