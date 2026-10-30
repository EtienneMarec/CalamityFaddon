using Terraria;
using Terraria.ModLoader;

namespace CalamityFandom.Common.Elements
{
    /// <summary>
    /// Affinités élémentaires du joueur (WikiData/elements.json -> armors) : l'ensemble d'armure complet (casque, plastron
    /// et jambières dont la classe commence par le préfixe de l'ensemble) donne ses résistances (50 % des dégâts) et
    /// vulnérabilités (200 %) contre les attaques élémentaires des ennemis (ElementData.AttackElements).
    /// </summary>
    public class ElementArmorPlayer : ModPlayer
    {
        /// <summary>Affinité courante (-1 résistant, +1 vulnérable), null sans ensemble élémentaire.</summary>
        public sbyte[] Affinity;

        public override void PostUpdateEquips()
        {
            Affinity = null;
            string head = Player.armor[0].ModItem?.Name, body = Player.armor[1].ModItem?.Name, legs = Player.armor[2].ModItem?.Name;
            if (head == null || body == null || legs == null)
                return;
            foreach (var (prefix, row) in ElementData.ArmorAffinity)
            {
                if (head.StartsWith(prefix, System.StringComparison.Ordinal) && body.StartsWith(prefix, System.StringComparison.Ordinal)
                    && legs.StartsWith(prefix, System.StringComparison.Ordinal))
                {
                    Affinity = row;
                    return;
                }
            }
        }

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            if (Affinity != null)
                modifiers.FinalDamage *= ElementData.Multiplier(ElementData.AttackElements(npc.type), Affinity);
        }

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            if (Affinity != null)
                modifiers.FinalDamage *= ElementData.Multiplier(ElementGlobalProjectile.Of(proj), Affinity);
        }
    }
}
