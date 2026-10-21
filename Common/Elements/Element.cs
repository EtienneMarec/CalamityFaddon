using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityFandom.Common.Elements
{
    /// <summary>Wiki : Elemental System.txt — les 17 éléments, dans l'ordre de la page.</summary>
    public enum Element
    {
        Cold, Fire, Wind, Water, Poison, Acid, Radioactive, Electricity,
        Stab, Slash, Smash, Bomb, Magic, Dark, Holy, Unholy, Machine,
    }

    /// <summary>
    /// Effet durable d'un élément sur un PNJ touché (debuff de statut), d'après la phrase de la page « Elemental System ».
    /// Le wiki ne donne aucun chiffre : valeurs choisies dans docs/decisions.md.
    /// </summary>
    /// <param name="Dps">Dégâts par seconde selon la progression : pré-Hardmode, Hardmode, après Moon Lord.</param>
    /// <param name="Slow">Fraction de vitesse retirée (moitié moins sur les boss).</param>
    /// <param name="DamageDown">Fraction des dégâts de contact retirée.</param>
    /// <param name="DefenseDown">Fraction de défense retirée face aux coups.</param>
    /// <param name="Confuse">Confusion vanilla (« discombobulate »).</param>
    /// <param name="MissChance">Chance de rater un coup de contact.</param>
    /// <param name="NoRegen">Empêche la régénération.</param>
    public record ElementEffect(int[] Dps = null, float Slow = 0f, float DamageDown = 0f, float DefenseDown = 0f,
        bool Confuse = false, float MissChance = 0f, bool NoRegen = false);

    public static class ElementInfo
    {
        public const int Count = 17;

        /// <summary>Durée du statut appliqué à chaque coup (ticks).</summary>
        public const int StatusTime = 120;

        public static readonly Color[] Colors =
        {
            new(140, 220, 255), // Cold
            new(255, 120, 40),  // Fire
            new(200, 240, 220), // Wind
            new(60, 140, 255),  // Water
            new(120, 200, 60),  // Poison
            new(170, 230, 40),  // Acid
            new(120, 255, 90),  // Radioactive
            new(255, 230, 80),  // Electricity
            new(200, 200, 210), // Stab
            new(220, 60, 60),   // Slash
            new(170, 130, 90),  // Smash
            new(255, 160, 60),  // Bomb
            new(220, 110, 255), // Magic
            new(120, 90, 160),  // Dark
            new(255, 245, 170), // Holy
            new(170, 40, 70),   // Unholy
            new(150, 180, 200), // Machine
        };

        // Wind (recul, géré au coup) et Bomb (critiques, géré au coup) n'ont pas de statut durable
        public static readonly ElementEffect[] Effects =
        {
            new(Slow: 0.20f, DamageDown: 0.10f),                                         // Cold : ralentit, affaiblit
            new(Dps: new[] { 6, 20, 60 }, DefenseDown: 0.10f),                           // Fire : brûle, -défense
            null,                                                                        // Wind
            new(Dps: new[] { 3, 10, 30 }, Slow: 0.10f),                                  // Water : ralentit, noie
            new(Dps: new[] { 5, 15, 45 }, DamageDown: 0.10f),                            // Poison : empoisonne, affaiblit
            new(Dps: new[] { 3, 10, 30 }, Slow: 0.10f, DamageDown: 0.10f, DefenseDown: 0.10f), // Acid : corrode
            new(Dps: new[] { 8, 25, 80 }, DefenseDown: 0.15f, NoRegen: true),            // Radioactive : irradie
            new(Dps: new[] { 6, 20, 60 }, Slow: 0.15f),                                  // Electricity : électrocute
            new(Dps: new[] { 4, 12, 40 }, Slow: 0.10f),                                  // Stab : perfore
            new(Dps: new[] { 8, 25, 80 }),                                               // Slash : saignement
            new(DefenseDown: 0.15f, Confuse: true),                                      // Smash : désoriente
            null,                                                                        // Bomb
            new(Dps: new[] { 4, 12, 40 }, DamageDown: 0.10f),                            // Magic : maléfice
            new(MissChance: 0.20f),                                                      // Dark : fait rater
            new(Dps: new[] { 10, 30, 100 }),                                             // Holy : châtiment
            new(Dps: new[] { 10, 30, 100 }),                                             // Unholy : malédiction
            new(DamageDown: 0.10f),                                                      // Machine : enraye
        };

        public static string Hex(Element e) => Colors[(int)e].Hex3();
    }
}
