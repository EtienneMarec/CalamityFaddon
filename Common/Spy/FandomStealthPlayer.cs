using CalamityFandom.Common.Config;
using CalamityMod;
using CalamityMod.Items.Weapons.Rogue;
using Terraria;
using Terraria.ModLoader;

namespace CalamityFandom.Common.Spy
{
    /// <summary>
    /// Discrétion du Spy telle que la décrit la page « Spy » (section Stealth), en Full Fandom : chaque point de discrétion
    /// (1 % de la jauge) donne +0,5 % de dégâts Spy, +0,67 % par point une fois la jauge pleine, et -4 d'aggro.
    /// Remplace le bonus de Calamity 2.2.2 (fonction de la vitesse de l'arme et du temps de remplissage), appliqué dans son
    /// PostUpdateMiscEffects, qui passe avant le nôtre (ce mod se charge après Calamity). Calamity++ garde le système actuel.
    /// </summary>
    public class FandomStealthPlayer : ModPlayer
    {
        public const float DamagePerPoint = 0.005f, DamagePerPointFull = 0.0067f;
        public const int AggroPerPoint = 4;

        public override void PostUpdateMiscEffects()
        {
            if (!FandomMode.IsFull)
                return;
            var cal = Player.Calamity();
            if (!cal.wearingRogueArmor || cal.rogueStealthMax <= 0f)
                return;
            float points = cal.rogueStealth * 100f;
            float bonus = points * (cal.rogueStealth >= cal.rogueStealthMax ? DamagePerPointFull : DamagePerPoint);
            // Calamity n'ajoute pas son bonus quand on tient l'Elephant Killer
            if (Player.HeldItem.type != ModContent.ItemType<ElephantKiller>())
                Player.GetDamage<RogueDamageClass>() += bonus - cal.stealthDamage;
            cal.stealthDamage = bonus;
            // Calamity retire déjà 300 d'aggro par jauge pleine (3 par point) : complément jusqu'à 4 par point
            Player.aggro -= (int)(points * (AggroPerPoint - 3));
        }
    }
}
