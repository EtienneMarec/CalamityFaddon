using CalamityMod.Items.Materials;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Common.Taming
{
    /// <summary>
    /// Route sans tuer (validé 31/10 : « le joueur doit pouvoir finir le jeu sans tuer un seul boss ») : sources des objets
    /// que demandent les méthodes de Taming quand le seul moyen de les avoir était de tuer un boss.
    /// </summary>
    public class TamingProgression : ModSystem
    {
        // King Slime : le Slime Gun ne tombe que de lui. [choix] Fabriqué à l'enclume.
        public override void AddRecipes()
        {
            Recipe.Create(ItemID.SlimeGun)
                .AddIngredient(ItemID.Gel, 30)
                .AddRecipeGroup(RecipeGroupID.IronBar, 4)
                .AddTile(TileID.Anvils)
                .Register();
        }

        // Wall of Flesh : le Guide meurt en l'invoquant. Le jeu le fait revenir pendant les combats de boss (seulement les
        // invasions et les éclipses bloquent les arrivées), mais au contrôle suivant (jusqu'à 2 min) : pendant une
        // tentative, contrôle toutes les 5 s. Même chose pour la Brimstone Elemental (PNJ qui emménagent).
        public override void PostUpdateWorld()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            if (TamingSystem.Active("Wall of Flesh") != null || TamingSystem.Active("Brimstone Elemental") != null)
            {
                double rate = WorldGen.GetWorldUpdateRate();
                if (rate > 0)
                    Main.checkForSpawns = System.Math.Max(Main.checkForSpawns, (int)(7200 / rate) - 300);
            }
        }
    }

    /// <summary>
    /// The Dragonfolly demande un Yharon Soul Fragment, qui ne tombe que de Yharon, qu'on n'invoque qu'avec les plumes du
    /// Dragonfolly. [choix] Comme les « Hellcaster Fragments » d'avant (Trivia de la page Yharon Soul Fragment : « dropped by
    /// Solar Pillar enemies »), les ennemis du Solar Pillar en lâchent un sur 20.
    /// </summary>
    public class SolarSoulFragments : GlobalNPC
    {
        private static readonly int[] SolarEnemies =
        {
            NPCID.SolarCorite, NPCID.SolarCrawltipedeHead, NPCID.SolarDrakomire, NPCID.SolarDrakomireRider,
            NPCID.SolarSroller, NPCID.SolarSolenian, NPCID.SolarSpearman,
        };

        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => System.Array.IndexOf(SolarEnemies, entity.type) >= 0;

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot) =>
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<YharonSoulFragment>(), 20));
    }
}
