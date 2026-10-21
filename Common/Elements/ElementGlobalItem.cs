using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityFandom.Common.Elements
{
    /// <summary>Côté arme : infobulle des éléments, bonus Bomb (critique) et Machine (statistiques pendant l'usage).</summary>
    public class ElementGlobalItem : GlobalItem
    {
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            var elements = ElementData.Of(item);
            if (elements.Length == 0)
                return;
            string list = string.Join(", ", elements.Select(e =>
                $"[c/{ElementInfo.Hex(e)}:{Language.GetTextValue($"Mods.CalamityFandom.Elements.{e}")}]"));
            var line = new TooltipLine(Mod, "Elements", Language.GetTextValue("Mods.CalamityFandom.Elements.Tooltip", list));
            // Juste après les dégâts si la ligne existe, sinon à la fin
            int index = tooltips.FindIndex(t => t.Mod == "Terraria" && t.Name == "Damage");
            if (index >= 0)
                tooltips.Insert(index + 1, line);
            else
                tooltips.Add(line);
        }

        public override void ModifyWeaponCrit(Item item, Player player, ref float crit)
        {
            if (ElementData.Of(item).Contains(Element.Bomb))
                crit *= 1.5f; // « gains 1.5x more critical chance »
        }

        public override void HoldItem(Item item, Player player)
        {
            // Machine : « empowers the player's offensive stats while being used »
            if (player.itemAnimation > 0 && ElementData.Of(item).Contains(Element.Machine))
            {
                player.GetDamage(DamageClass.Generic) += 0.05f;
                player.GetCritChance(DamageClass.Generic) += 5f;
            }
        }
    }
}
