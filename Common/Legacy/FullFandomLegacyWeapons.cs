using CalamityFandom.Common.Config;
using CalamityFandom.Content.Legacy.Projectiles.Melee;
using CalamityMod;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Rogue;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Common.Legacy
{
    /// <summary>
    /// Armes officielles refaites par Calamity après l'abandon du wiki : en Full Fandom, elles retrouvent le fonctionnement de
    /// 2.0.1.005 décrit par leur page (projectiles portés dans Content/Legacy). Calamity++ garde la version de Calamity.
    /// - Galileo Gladius : animation de Calamity réduite ; croissant de lune si l'estoc rate, planète sur un coup direct ;
    ///   plus de lancer au clic droit.
    /// - Cosmic Shiv : orbe cosmique qui appelle des lames et une pluie d'étoiles.
    /// - Spore Knife (retirée avant 2.0.1.005 puis refaite en arme rogue) : épée courte de mêlée, nuage de spores au coup.
    /// Stats (dégâts, use...) : WikiStats, comme pour toute arme officielle.
    /// </summary>
    public class FullFandomLegacyWeapons : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) =>
            FandomMode.IsFull && (entity.type == ModContent.ItemType<GalileoGladius>() || entity.type == ModContent.ItemType<CosmicShiv>()
                || entity.type == ModContent.ItemType<SporeKnife>());

        public override void SetDefaults(Item item)
        {
            if (item.type == ModContent.ItemType<GalileoGladius>())
            {
                // Animation de Calamity (épée tenue), réduite, avec le fonctionnement de 2.0.1.005
                item.shoot = ModContent.ProjectileType<GalileoGladiusHeldProj>();
            }
            else if (item.type == ModContent.ItemType<SporeKnife>())
            {
                // Couteau de lancer rogue chez Calamity : redevient l'épée courte de mêlée de la page
                item.useStyle = ItemUseStyleID.Rapier;
                item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
                item.shoot = ModContent.ProjectileType<SporeKnifeShortsword>();
                item.shootSpeed = 2f;
                item.useTurn = false;
                item.UseSound = SoundID.Item1;
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

        /// <summary>
        /// Calamity relance le minuteur de Stratus Starburst tant qu'on tient le Galileo Gladius, ce qui affiche le compteur
        /// en haut à gauche ; le Starburst n'existe pas sur le wiki : on remet la valeur d'avant (retour de test du 28/10).
        /// </summary>
        public override void HoldItem(Item item, Player player)
        {
            if (item.type == ModContent.ItemType<GalileoGladius>())
                player.Calamity().StratusStarburstResetTimer = player.GetModPlayer<StarburstTimerPlayer>().TimerBeforeItemCheck;
        }

        // Le lancer du Galileo Gladius (clic droit, Stratus) n'existe pas sur le wiki
        public override bool CanUseItem(Item item, Player player)
        {
            if (player.altFunctionUse == 2)
                return false;
            // Un estoc à la fois (Calamity le vérifie pour son propre projectile seulement)
            return item.type != ModContent.ItemType<GalileoGladius>() || player.ownedProjectileCounts[ModContent.ProjectileType<GalileoGladiusHeldProj>()] == 0;
        }
    }

    /// <summary>Minuteur de Stratus Starburst avant l'utilisation des objets (voir FullFandomLegacyWeapons.HoldItem).</summary>
    public class StarburstTimerPlayer : ModPlayer
    {
        public int TimerBeforeItemCheck;

        public override bool PreItemCheck()
        {
            TimerBeforeItemCheck = Player.Calamity().StratusStarburstResetTimer;
            return true;
        }
    }
}
