using CalamityFandom.Content.Buffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Items.Potions
{
    /// <summary>
    /// Wiki : Flask of Plague.txt — flask inventée par le wiki, sur le modèle de celles de Calamity (Flask of Brimstone...) :
    /// Weapon Imbue: Plague 20 min, les attaques de mêlée, de fouet et spy infligent la Plague 3 s. Lâchée par le
    /// Plaguebringer Goliath (WikiLoot). Valeurs du wiki : use 17, pile 9999, recherche 20, rareté 7, vente 40 argent.
    /// [choix] Les caisses de jungle (Notes) ne sont pas encore faites.
    /// </summary>
    public class FlaskOfPlague : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
            ItemID.Sets.DrinkParticleColors[Type] = new[] { new Color(150, 200, 40), new Color(90, 160, 30), new Color(60, 110, 40) };
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(18, 40, ModContent.BuffType<WeaponImbuePlague>(), 60 * 60 * 20, true);
            Item.useTime = Item.useAnimation = 17;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(silver: 40);
            Item.rare = ItemRarityID.Lime;
        }
    }
}
