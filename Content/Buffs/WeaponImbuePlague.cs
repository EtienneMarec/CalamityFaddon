using CalamityMod;
using CalamityMod.Buffs;
using CalamityMod.Buffs.DamageOverTime;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Buffs
{
    /// <summary>
    /// Buff de la Flask of Plague (comme les flasks de Calamity : un seul buff de flask à la fois, IsAFlaskBuff) : les
    /// attaques de mêlée, de fouet et spy infligent la Plague 3 s.
    /// </summary>
    public class WeaponImbuePlague : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.pvpBuff[Type] = true;
            Main.meleeBuff[Type] = true;
            Main.persistentBuff[Type] = true;
            BuffID.Sets.IsAFlaskBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FlaskOfPlaguePlayer>().Active = true;
            // Valeur de Calamity pour « flask moddée » (pas d'effet de flask de vanilla)
            player.meleeEnchant = CalamityGlobalBuff.ModdedFlaskEnchant;
        }
    }

    public class FlaskOfPlaguePlayer : ModPlayer
    {
        public bool Active;

        public override void ResetEffects() => Active = false;

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone) => Infect(item.DamageType, false, target);

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone) => Infect(proj.DamageType, proj.noEnchantments, target);

        private void Infect(DamageClass type, bool noEnchantments, NPC target)
        {
            if (!Active || noEnchantments)
                return;
            if (type.CountsAsClass(DamageClass.Melee) || type.CountsAsClass(DamageClass.SummonMeleeSpeed)
                || type.CountsAsClass(ModContent.GetInstance<RogueDamageClass>()))
                target.AddBuff(ModContent.BuffType<Plague>(), 180);
        }

        public override void MeleeEffects(Item item, Microsoft.Xna.Framework.Rectangle hitbox)
        {
            if (Active && Main.rand.NextBool(3))
                Dust.NewDust(hitbox.TopLeft(), hitbox.Width, hitbox.Height, DustID.GreenFairy, Player.velocity.X * 0.2f + Player.direction * 3f, Player.velocity.Y * 0.2f, 100);
        }
    }
}
