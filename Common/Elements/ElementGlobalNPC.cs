using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Common.Elements
{
    /// <summary>
    /// Côté PNJ du système d'éléments : multiplicateur résistant/vulnérable sur chaque coup, application des statuts,
    /// et effets des statuts actifs (dégâts sur la durée, ralentissement, affaiblissement, défense, ratés).
    /// </summary>
    public class ElementGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        // Effets cumulés des statuts actifs, recalculés à chaque tick
        private float slow, damageDown, defenseDown, missChance;

        public override void ResetEffects(NPC npc)
        {
            slow = damageDown = defenseDown = missChance = 0f;
        }

        private static int Tier => NPC.downedMoonlord ? 2 : Main.hardMode ? 1 : 0;

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            bool noRegen = false;
            int dps = 0;
            for (int i = 0; i < NPC.maxBuffs; i++)
            {
                if (npc.buffTime[i] <= 0 || ModContent.GetModBuff(npc.buffType[i]) is not ElementStatus status)
                    continue;
                var effect = ElementInfo.Effects[(int)status.Element];
                if (effect.Dps != null)
                    dps += effect.Dps[Tier];
                slow += effect.Slow;
                damageDown += effect.DamageDown;
                defenseDown += effect.DefenseDown;
                missChance += effect.MissChance;
                noRegen |= effect.NoRegen;
            }
            slow = Math.Min(slow, 0.5f) * (npc.boss ? 0.5f : 1f);
            damageDown = Math.Min(damageDown, 0.5f);
            defenseDown = Math.Min(defenseDown, 0.5f);
            missChance = Math.Min(missChance, 0.5f);

            if (noRegen && npc.lifeRegen > 0)
                npc.lifeRegen = 0;
            if (dps > 0)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;
                npc.lifeRegen -= dps * 2;
                damage = Math.Max(damage, Math.Max(1, dps / 4));
            }
        }

        public override void PostAI(NPC npc)
        {
            // Ralentissement : on annule une partie du déplacement du tick (ne se cumule pas d'un tick à l'autre)
            if (slow > 0f && !npc.townNPC)
                npc.position -= npc.velocity * slow;
        }

        /// <summary>
        /// Retour visuel des statuts, sans icône (décision du 21/10) : particules propres à chaque élément et légère teinte
        /// de l'ennemi (couleurs mélangées si plusieurs éléments).
        /// </summary>
        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            Vector3 tint = Vector3.Zero;
            int active = 0;
            for (int i = 0; i < NPC.maxBuffs; i++)
            {
                if (npc.buffTime[i] <= 0 || ModContent.GetModBuff(npc.buffType[i]) is not ElementStatus status)
                    continue;
                tint += ElementInfo.Colors[(int)status.Element].ToVector3();
                active++;
                if (Main.rand.NextBool(5))
                {
                    var dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, ElementInfo.Dusts[(int)status.Element], 0f, -1f, 100, default, 1.1f);
                    dust.noGravity = status.Element is not (Element.Stab or Element.Slash); // le sang tombe
                    dust.velocity *= 0.4f;
                }
            }
            if (active > 0)
                drawColor = Color.Lerp(drawColor, new Color(tint / active).MultiplyRGBA(drawColor), 0.35f);
        }

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (defenseDown > 0f)
                modifiers.Defense *= 1f - defenseDown;
        }

        public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
        {
            if (damageDown > 0f)
                modifiers.SourceDamage *= 1f - damageDown;
        }

        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            // Dark : rate ses attaques. Tirage fixe par fenêtre de 20 ticks, sinon le contact retenterait chaque tick.
            if (missChance <= 0f)
                return true;
            uint window = Main.GameUpdateCount / 20;
            uint hash = (uint)(npc.whoAmI * 7919) ^ (window * 2654435761u);
            return hash % 1000 >= missChance * 1000;
        }

        // ------------------------------------------------------------------ coups reçus

        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers) =>
            ModifyElementalHit(npc, ElementData.Of(item), null, ref modifiers);

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers) =>
            ModifyElementalHit(npc, ElementGlobalProjectile.Of(projectile), projectile, ref modifiers);

        private static void ModifyElementalHit(NPC npc, IReadOnlyList<Element> elements, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (elements.Count == 0)
                return;
            modifiers.FinalDamage *= ElementData.Multiplier(elements, npc.type);
            foreach (var e in elements)
            {
                switch (e)
                {
                    case Element.Wind:
                        modifiers.Knockback *= 1.5f; // « send enemies flying away »
                        break;
                    case Element.Bomb:
                        modifiers.CritDamage *= 2f; // « 2x more critical damage »
                        if (projectile != null)
                            modifiers.SourceDamage *= projectile.GetGlobalProjectile<ElementGlobalProjectile>().BombCrowdBonus();
                        break;
                }
            }
        }

        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone) =>
            OnElementalHit(npc, ElementData.Of(item), hit);

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone) =>
            OnElementalHit(npc, ElementGlobalProjectile.Of(projectile), hit);

        private static void OnElementalHit(NPC npc, IReadOnlyList<Element> elements, NPC.HitInfo hit)
        {
            if (elements.Count == 0)
                return;
            foreach (var e in elements)
            {
                int buff = ElementData.StatusBuffs[(int)e];
                if (buff >= 0 && Main.rand.NextFloat() < ElementData.ProcChance[(int)e])
                    npc.AddBuff(buff, ElementInfo.StatusTime); // ignoré si le PNJ est résistant (immunité)
            }

            // Retour visuel pour le joueur qui frappe, sans texte en plus (décision du 22/10) : le chiffre de dégâts change de
            // couleur
            float mult = ElementData.Multiplier(elements, npc.type);
            if (mult != 1f && Main.netMode != NetmodeID.Server)
                TintDamageText(npc, hit.Damage, mult > 1f ? VulnerableColor : ResistedColor);
        }

        public static readonly Color VulnerableColor = new(255, 235, 70);
        public static readonly Color ResistedColor = new(150, 150, 160);

        /// <summary>
        /// Recolore le chiffre de dégâts que le jeu vient d'afficher pour ce coup (créé par StrikeNPC juste avant les
        /// OnHit) : un texte de combat tout neuf a encore scale = 0 et alpha = 1, le même nombre et se trouve sur l'ennemi.
        /// </summary>
        private static void TintDamageText(NPC npc, int damage, Color color)
        {
            string text = damage.ToString();
            Rectangle area = npc.Hitbox;
            area.Inflate(64, 64);
            for (int i = Main.maxCombatText - 1; i >= 0; i--)
            {
                CombatText t = Main.combatText[i];
                if (t.active && t.scale == 0f && t.alpha == 1f && t.text == text && area.Contains(t.position.ToPoint()))
                {
                    t.color = color;
                    return;
                }
            }
        }
    }
}
