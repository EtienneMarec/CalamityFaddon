using CalamityFandom.Common.Taming;
using CalamityFandom.Common.Taming.Chess;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Items.Taming
{
    /// <summary>
    /// Holographic Chessboard : cité par la page Taming (« Beat the Brain of Cthulhu at chess using the Holographic
    /// Chessboard »), sans page. [validé 31/10] Lance des problèmes d'échecs : 3 mats en un tirés parmi 45 contre le Brain
    /// of Cthulhu, mats en deux contre XP-00 Hypnos (plus dur). Sans boss : entraînement. [choix] Fabriqué à l'établi
    /// (verre, bois, étoile filante) pour que la méthode soit faisable sans tuer de boss ; sprite provisoire.
    /// [validé 31/10] Près d'un autre PNJ (10 blocs, hors boss) : même mise en scène, partie pour le plaisir.
    /// </summary>
    public class HolographicChessboard : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = Item.height = 32;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 20);
        }

        public override bool CanUseItem(Player player) => ChessUISystem.Session == null;

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI != Main.myPlayer)
                return true;
            // TODO : XP-00 Hypnos (boss inventé, à créer) -> mats en deux
            NPC mob = null;
            foreach (var npc in Main.ActiveNPCs)
            {
                if (npc.type == NPCID.BrainofCthulhu && npc.Distance(player.Center) < 3000f && !TamingSystem.Untamable.Contains("Brain of Cthulhu"))
                {
                    ChessUISystem.Open("Brain of Cthulhu", false, npc);
                    return true;
                }
                // Autre PNJ à moins de 10 blocs : partie pour le plaisir (le plus proche)
                if (ChessScene.CanPlay(npc) && npc.WithinRange(player.Center, ChessScene.MobRange)
                    && (mob == null || npc.DistanceSQ(player.Center) < mob.DistanceSQ(player.Center)))
                    mob = npc;
            }
            ChessUISystem.Open(null, NPC.downedMoonlord, mob);
            return true;
        }

        public override void AddRecipes() =>
            CreateRecipe()
                .AddIngredient(ItemID.Glass, 10)
                .AddRecipeGroup(RecipeGroupID.Wood, 10)
                .AddIngredient(ItemID.FallenStar)
                .AddTile(TileID.WorkBenches)
                .Register();
    }
}
