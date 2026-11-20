using System;
using CalamityMod;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

// Lot BC (20/11) : debuffs du wiki qui n'existaient pas (effets de l'infobox, sur le joueur et sur les ennemis). Demon
// Flames (Zenith Arcanum, Demonshade armor) et Umbric Fumes (Shadowspectre Tracers) remplacent les debuffs provisoires.

namespace CalamityFandom.Content.Buffs
{
    /// <summary>Debuff du wiki : icône de la page, effets dans UpdatePlayer / UpdateNPC ; WikiDebuffNPC lit les autres.</summary>
    public abstract class WikiDebuff : ModBuff
    {
        public override string Texture => "CalamityFandom/Content/Buffs/Wiki/" + Name;

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = false;
        }

        /// <summary>Perte de vie par seconde (joueur, ennemi).</summary>
        protected virtual int PlayerLoss => 0;
        protected virtual int NPCLoss => 0;
        public virtual int NPCDefenseLoss(NPC npc, int buffTime) => 0;
        public virtual float NPCDamageTaken => 1f;
        public virtual float NPCDamageDealt => 1f;

        public override void Update(Player player, ref int buffIndex)
        {
            if (PlayerLoss > 0)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;
                player.lifeRegenTime = 0;
                player.lifeRegen -= PlayerLoss * 2;
            }
            UpdatePlayer(player);
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (NPCLoss > 0)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;
                npc.lifeRegen -= NPCLoss * 2;
            }
            UpdateNPC(npc, npc.buffTime[buffIndex]);
        }

        protected virtual void UpdatePlayer(Player player)
        {
        }

        protected virtual void UpdateNPC(NPC npc, int buffTime)
        {
        }
    }

    /// <summary>Wiki : Demon Flames.txt — -1250 PV par seconde.</summary>
    public class DemonFlames : WikiDebuff
    {
        protected override int PlayerLoss => 1250;
        protected override int NPCLoss => 1250;

        protected override void UpdateNPC(NPC npc, int buffTime)
        {
            if (Main.rand.NextBool(3))
                Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.Shadowflame, 0f, -2f, 100, default, 1.4f).noGravity = true;
        }
    }

    /// <summary>
    /// Wiki : Embers of Civilization.txt — joueur : -15 PV/s, -15 % de dégâts, +30 % de coût en mana, -3 sbires ; ennemi :
    /// -50 PV/s, -15 % de réduction des dégâts. [choix] La réduction en moins = +15 % de dégâts reçus.
    /// </summary>
    public class EmbersofCivilization : WikiDebuff
    {
        protected override int PlayerLoss => 15;
        protected override int NPCLoss => 50;
        public override float NPCDamageTaken => 1.15f;

        protected override void UpdatePlayer(Player player)
        {
            player.GetDamage<GenericDamageClass>() *= 0.85f;
            player.manaCost += 0.3f;
            player.maxMinions = Math.Max(0, player.maxMinions - 3);
        }
    }

    /// <summary>
    /// Wiki : Hemophilia.txt — -10 PV/s ; la défense de la cible baisse d'un point par seconde restante sur le debuff.
    /// [technique] Le gain de défense du joueur qui l'a infligé n'est pas fait (le jeu ne garde pas qui a posé le debuff).
    /// </summary>
    public class Hemophilia : WikiDebuff
    {
        protected override int PlayerLoss => 10;
        protected override int NPCLoss => 10;

        public override int NPCDefenseLoss(NPC npc, int buffTime) => buffTime / 60;

        protected override void UpdatePlayer(Player player) => player.statDefense -= 10;
    }

    /// <summary>
    /// Wiki : Umbric Fumes.txt — plus de régénération, -21 PV/s, -35 défense, -5 % de réduction des dégâts, -20 % de vitesse ;
    /// vision très déformée. [technique] La déformation de la vision n'est pas faite.
    /// </summary>
    public class UmbricFumes : WikiDebuff
    {
        protected override int PlayerLoss => 21;
        protected override int NPCLoss => 21;

        public override int NPCDefenseLoss(NPC npc, int buffTime) => 35;

        protected override void UpdatePlayer(Player player)
        {
            player.statDefense -= 35;
            player.endurance -= 0.05f;
            player.moveSpeed -= 0.2f;
        }
    }

    /// <summary>Wiki : Lunar Aura.txt — -60 PV/s, -20 % de dégâts (le joueur, ou les coups de l'ennemi).</summary>
    public class LunarAura : WikiDebuff
    {
        protected override int PlayerLoss => 60;
        protected override int NPCLoss => 60;
        public override float NPCDamageDealt => 0.8f;

        protected override void UpdatePlayer(Player player) => player.GetDamage<GenericDamageClass>() *= 0.8f;
    }

    /// <summary>Wiki : Accelerated Photosynthesis.txt — -90 % de vitesse.</summary>
    public class AcceleratedPhotosynthesis : WikiDebuff
    {
        protected override void UpdatePlayer(Player player)
        {
            player.moveSpeed *= 0.1f;
            player.maxRunSpeed *= 0.1f;
        }

        protected override void UpdateNPC(NPC npc, int buffTime)
        {
            if (!npc.boss)
                npc.position -= npc.velocity * 0.9f;
        }
    }

    /// <summary>
    /// Wiki : Hemorrhaging.txt — joueur : plus de régénération, -25 % de vitesse et d'accélération, -1 % de la vie max par
    /// seconde (-2,5 % en se déplaçant) ; ennemi : -0,3 % de la vie max par seconde.
    /// </summary>
    public class Hemorrhaging : WikiDebuff
    {
        protected override void UpdatePlayer(Player player)
        {
            float part = Math.Abs(player.velocity.X) > 0.5f ? 0.025f : 0.01f;
            if (player.lifeRegen > 0)
                player.lifeRegen = 0;
            player.lifeRegenTime = 0;
            player.lifeRegen -= (int)(player.statLifeMax2 * part * 2);
            player.moveSpeed -= 0.25f;
            player.runAcceleration *= 0.75f;
        }

        protected override void UpdateNPC(NPC npc, int buffTime)
        {
            if (npc.lifeRegen > 0)
                npc.lifeRegen = 0;
            npc.lifeRegen -= Math.Max(2, (int)(npc.lifeMax * 0.003f * 2));
        }
    }

    /// <summary>Wiki : Horror.txt — -15 défense, -15 % de vitesse et d'accélération, vision assombrie (comme Darkness).</summary>
    public class Horror : WikiDebuff
    {
        public override int NPCDefenseLoss(NPC npc, int buffTime) => 15;

        protected override void UpdatePlayer(Player player)
        {
            player.statDefense -= 15;
            player.moveSpeed -= 0.15f;
            player.runAcceleration *= 0.85f;
            player.blind = true;
        }
    }

    /// <summary>Wiki : Silva Stun.txt — la cible ne peut plus bouger (pas les boss).</summary>
    public class SilvaStun : WikiDebuff
    {
        protected override void UpdatePlayer(Player player)
        {
            player.velocity = Vector2.Zero;
            player.controlLeft = player.controlRight = player.controlJump = player.controlUp = player.controlDown = false;
        }

        protected override void UpdateNPC(NPC npc, int buffTime)
        {
            if (!npc.boss)
                npc.velocity = Vector2.Zero;
        }
    }

    /// <summary>Défense, dégâts reçus et dégâts infligés des ennemis sous un debuff du wiki ; Demon Flames de la Demonshade armor.</summary>
    public class WikiDebuffNPC1120 : GlobalNPC
    {
        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            for (int i = 0; i < NPC.maxBuffs; i++)
            {
                if (npc.buffTime[i] <= 0 || BuffLoader.GetBuff(npc.buffType[i]) is not WikiDebuff d)
                    continue;
                int loss = d.NPCDefenseLoss(npc, npc.buffTime[i]);
                if (loss > 0)
                    modifiers.Defense.Flat -= loss;
                modifiers.FinalDamage *= d.NPCDamageTaken;
            }
        }

        public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
        {
            for (int i = 0; i < NPC.maxBuffs; i++)
                if (npc.buffTime[i] > 0 && BuffLoader.GetBuff(npc.buffType[i]) is WikiDebuff d)
                    modifiers.FinalDamage *= d.NPCDamageDealt;
        }
    }

    /// <summary>
    /// Demonshade armor (page Demon Flames : « inflicted by all weapons while Demonshade armor is equipped ») : 5 s à chaque
    /// coup. [choix] Durée de 5 s.
    /// </summary>
    public class DemonFlamesPlayer : ModPlayer
    {
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Player.Calamity().demonshadeSet)
                target.AddBuff(ModContent.BuffType<DemonFlames>(), 300);
        }
    }
}
