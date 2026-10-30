using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityFandom.Common.Elements
{
    /// <summary>Un projectile porte les éléments de l'arme qui l'a tiré (ou du projectile qui l'a créé).</summary>
    public class ElementGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public Element[] Elements = Array.Empty<Element>();

        // Bomb : « deals more damage the more enemies are hit at one time »
        private uint bombTick;
        private int bombHits;

        public static IReadOnlyList<Element> Of(Projectile projectile) =>
            projectile.TryGetGlobalProjectile<ElementGlobalProjectile>(out var g) ? g.Elements : Array.Empty<Element>();

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            // Projectile ennemi : éléments des attaques du PNJ qui le tire (ElementData.AttackElements)
            if (projectile.hostile && source is EntitySource_Parent { Entity: NPC shooter })
            {
                Elements = ElementData.AttackElements(shooter.type);
                return;
            }
            if (!projectile.friendly || projectile.hostile)
                return;
            Elements = source switch
            {
                EntitySource_ItemUse use => ElementData.Of(use.Item),
                EntitySource_Parent { Entity: Projectile parent } => (Element[])Of(parent),
                _ => Elements,
            };
        }

        /// <summary>+10 % par ennemi déjà touché dans le même tick (explosions), jusqu'à x2.</summary>
        public float BombCrowdBonus()
        {
            if (bombTick != Main.GameUpdateCount)
            {
                bombTick = Main.GameUpdateCount;
                bombHits = 0;
            }
            return Math.Min(2f, 1f + 0.1f * bombHits++);
        }
    }
}
