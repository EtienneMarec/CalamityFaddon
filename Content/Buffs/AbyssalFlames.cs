using CalamityMod;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Buffs
{
    /// <summary>
    /// Wiki : Abyssal Flames.txt — distinct des Brimstone Flames : pas de régénération, -6 défense ; joueur : -8 PV/s et
    /// particules rouges ; ennemi : -62,5 PV/s (78,5 s'il est Oiled), teinté de noir, crache des particules rouge sang.
    /// TODO : la page le fait aussi infliger au joueur par Supreme Calamitas, ses frères, Sepulcher et les Soul Seekers.
    /// </summary>
    public class AbyssalFlames : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<AbyssalFlamesPlayer>().Burning = true;
            player.statDefense -= 6;
        }

        public override void Update(NPC npc, ref int buffIndex) => npc.GetGlobalNPC<AbyssalFlamesNPC>().Burning = true;
    }

    public class AbyssalFlamesPlayer : ModPlayer
    {
        public bool Burning;

        public override void ResetEffects() => Burning = false;

        public override void UpdateBadLifeRegen()
        {
            if (!Burning)
                return;
            if (Player.lifeRegen > 0)
                Player.lifeRegen = 0;
            Player.lifeRegenTime = 0;
            Player.lifeRegen -= 16;
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (Burning && Main.rand.NextBool(4) && drawInfo.shadow == 0f)
            {
                Dust d = Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.RedTorch, 0f, -1.5f, 100, default, 1.5f);
                d.noGravity = true;
                drawInfo.DustCache.Add(d.dustIndex);
            }
        }
    }

    public class AbyssalFlamesNPC : GlobalNPC
    {
        public bool Burning;

        public override bool InstancePerEntity => true;

        public override void ResetEffects(NPC npc) => Burning = false;

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (!Burning)
                return;
            if (npc.lifeRegen > 0)
                npc.lifeRegen = 0;
            // 62,5 PV/s, 78,5 avec Oiled (page)
            npc.lifeRegen -= npc.HasBuff(BuffID.Oiled) ? 157 : 125;
            if (damage < 13)
                damage = 13;
        }

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (Burning)
                modifiers.Defense.Flat -= 6;
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (!Burning)
                return;
            drawColor = Color.Lerp(drawColor, Color.Black, 0.6f);
            if (Main.rand.NextBool(3))
            {
                Dust d = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.Blood, 0f, -2f, 100, new Color(160, 0, 0), 1.3f);
                d.noGravity = true;
                d.velocity *= 1.5f;
            }
        }
    }
}
