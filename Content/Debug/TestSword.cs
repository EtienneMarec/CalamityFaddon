using System;
using CalamityFandom.Common.Elements;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Debug
{
    /// <summary>
    /// Épées de test (objets de debug, sans recette) : une par élément, plus une qui inflige Moon Burnt. Faibles dégâts
    /// pour voir les effets durer sur FandomTestDummy. Les éléments suivent les règles du mode (chance en Calamity++).
    /// </summary>
    [Autoload(false)]
    public class TestSword : ModItem
    {
        private readonly Element? element;
        private readonly string calamityBuff;
        private readonly string name;

        private TestSword(Element? element, string calamityBuff, string name)
        {
            this.element = element;
            this.calamityBuff = calamityBuff;
            this.name = name;
        }

        public override string Name => name;
        public override string Texture => $"Terraria/Images/Item_{ItemID.CopperBroadsword}";
        protected override bool CloneNewInstances => true;

        private string EffectName => element is { } e
            ? Language.GetTextValue($"Mods.CalamityFandom.Elements.{e}")
            : ModContent.TryFind<ModBuff>("CalamityMod", calamityBuff, out var buff) ? Lang.GetBuffName(buff.Type) : calamityBuff;

        public override LocalizedText DisplayName => Language.GetText("Mods.CalamityFandom.Items.TestSword.DisplayName").WithFormatArgs(EffectName);
        public override LocalizedText Tooltip => Language.GetText("Mods.CalamityFandom.Items.TestSword.Tooltip").WithFormatArgs(EffectName);

        public static void LoadAll(Mod mod)
        {
            foreach (Element e in Enum.GetValues<Element>())
                mod.AddContent(new TestSword(e, null, $"TestSword{e}"));
            mod.AddContent(new TestSword(null, "Nightwither", "TestSwordMoonBurnt"));
        }

        public override void SetStaticDefaults()
        {
            // Un seul élément par épée, sans passer par les données du wiki
            if (element is { } e)
                ElementData.ItemElements[Type] = new[] { e };
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.damage = 10;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 2f;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = Item.useTime = 15;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemRarityID.Quest;
            Item.color = element is { } e ? ElementInfo.Colors[(int)e] : new Color(90, 140, 255);
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (calamityBuff != null && ModContent.TryFind<ModBuff>("CalamityMod", calamityBuff, out var buff))
                target.AddBuff(buff.Type, 180);
        }
    }
}
