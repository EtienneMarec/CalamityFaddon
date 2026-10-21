using System.Collections.Generic;
using CalamityFandom.Common.Elements;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Debug
{
    /// <summary>
    /// Objet de test (sans recette, à prendre via Cheat Sheet) : applique un debuff choisi au joueur et à l'ennemi le plus
    /// proche, pour vérifier en jeu son nom, son icône et sa description (barre de buffs du joueur, affichage de Calamity
    /// au-dessus des boss). Clic droit : debuff suivant.
    /// </summary>
    public class FandomTestVial : ModItem
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.BottledWater}";

        private int selected;

        /// <summary>Debuffs testables : ceux que le wiki renomme ou dessine autrement, puis les statuts d'éléments.</summary>
        private static List<int> Debuffs
        {
            get
            {
                var list = new List<int>();
                foreach (string name in new[] { "Nightwither", "MarkedforDeath" })
                    if (ModContent.TryFind<ModBuff>("CalamityMod", name, out var buff))
                        list.Add(buff.Type);
                foreach (int status in ElementData.StatusBuffs)
                    if (status >= 0)
                        list.Add(status);
                return list;
            }
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 26;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useAnimation = Item.useTime = 20;
            Item.UseSound = SoundID.Item3;
            Item.rare = ItemRarityID.Quest;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool? UseItem(Player player)
        {
            var debuffs = Debuffs;
            if (debuffs.Count == 0 || player.whoAmI != Main.myPlayer)
                return true;
            selected %= debuffs.Count;
            if (player.altFunctionUse == 2)
            {
                selected = (selected + 1) % debuffs.Count;
                CombatText.NewText(player.Hitbox, Color.LightSkyBlue, Lang.GetBuffName(debuffs[selected]));
                return true;
            }
            int buff = debuffs[selected];
            player.AddBuff(buff, 600);
            NPC target = null;
            float best = 800f;
            foreach (var npc in Main.ActiveNPCs)
            {
                float dist = npc.Distance(player.Center);
                if (!npc.friendly && npc.lifeMax > 5 && dist < best)
                {
                    best = dist;
                    target = npc;
                }
            }
            target?.AddBuff(buff, 600);
            Main.NewText(Language.GetTextValue("Mods.CalamityFandom.Items.FandomTestVial.Applied", Lang.GetBuffName(buff),
                target != null ? target.FullName : "-"), Color.LightSkyBlue);
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var debuffs = Debuffs;
            if (debuffs.Count > 0)
                tooltips.Add(new TooltipLine(Mod, "Selected", Language.GetTextValue("Mods.CalamityFandom.Items.FandomTestVial.Selected",
                    Lang.GetBuffName(debuffs[selected % debuffs.Count]))));
        }
    }
}
