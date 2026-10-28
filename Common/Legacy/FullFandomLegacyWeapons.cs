using CalamityFandom.Common.Config;
using CalamityFandom.Content.Legacy.Projectiles.Melee;
using CalamityMod.Items.Weapons.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Common.Legacy
{
    /// <summary>
    /// Armes officielles refaites par Calamity après l'abandon du wiki : en Full Fandom, elles retrouvent le fonctionnement de
    /// 2.0.1.005 décrit par leur page (projectiles portés dans Content/Legacy). Calamity++ garde la version de Calamity.
    /// - Galileo Gladius : estoc qui tire un croissant de lune, planètes sur un coup direct ; plus de lancer au clic droit.
    /// - Cosmic Shiv : orbe cosmique qui appelle des lames et une pluie d'étoiles.
    /// Stats (dégâts, use...) : WikiStats, comme pour toute arme officielle.
    /// </summary>
    public class FullFandomLegacyWeapons : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) =>
            FandomMode.IsFull && (entity.type == ModContent.ItemType<GalileoGladius>() || entity.type == ModContent.ItemType<CosmicShiv>());

        public override void SetDefaults(Item item)
        {
            if (item.type == ModContent.ItemType<GalileoGladius>())
            {
                // Calamity en fait une épée tenue (useStyle Shoot) : retour à l'estoc
                item.useStyle = ItemUseStyleID.Rapier;
                item.shoot = ModContent.ProjectileType<GalileoGladiusProj>();
                item.shootSpeed = 0.9f;
            }
            else
            {
                item.shoot = ModContent.ProjectileType<CosmicShivProj>();
                item.shootSpeed = 2.4f;
            }
            item.noUseGraphic = true;
            item.noMelee = true;
            item.autoReuse = true;
        }

        // Le lancer du Galileo Gladius (clic droit, Stratus) n'existe pas sur le wiki
        public override bool CanUseItem(Item item, Player player) => player.altFunctionUse != 2;
    }
}
