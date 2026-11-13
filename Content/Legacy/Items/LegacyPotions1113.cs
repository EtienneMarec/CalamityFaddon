// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using CalamityFandom.Content.Buffs;
using CalamityFandom.Content.Legacy.Buffs.Potions;
using CalamityMod;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

// Calamitas' Brew : retirée avant 2.0.1.005 (dernière version mensuelle 2022-10), recettes du wiki.

namespace CalamityFandom.Content.Legacy.Items.Potions
{
    /// <summary>
    /// Wiki : Calamitas_ Brew.txt — buff Abyssal Weapon 15 min : +5 % de vitesse de déplacement, les coups de mêlée et de spy
    /// infligent Abyssal Flames (6 s une fois sur quatre, sinon 4 s ou 2 s à chances égales).
    /// </summary>
    public class CalamitasBrew : LegacyBuffPotion
    {
        protected override int Buff => ModContent.BuffType<AbyssalWeapon>();
        protected override int Seconds => 900;
        protected override int Rarity => ItemRarityID.Lime;
        protected override (int, int) Size => (30, 34);
    }
}

namespace CalamityFandom.Content.Legacy.Buffs.Potions
{
    public class AbyssalWeapon : LegacyPotionBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.moveSpeed += 0.05f;
            player.GetModPlayer<AbyssalWeaponPlayer>().Active = true;
        }
    }

    /// <summary>Abyssal Flames sur les coups de mêlée et de spy (armes et projectiles) ; flammes autour de la lame.</summary>
    public class AbyssalWeaponPlayer : ModPlayer
    {
        public bool Active;

        public override void ResetEffects() => Active = false;

        private void Inflict(NPC target)
        {
            if (!Active)
                return;
            float roll = Main.rand.NextFloat();
            int seconds = roll < 0.25f ? 6 : roll < 0.625f ? 4 : 2;
            target.AddBuff(ModContent.BuffType<AbyssalFlames>(), seconds * 60);
        }

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (item.CountsAsClass(DamageClass.Melee) || item.CountsAsClass<RogueDamageClass>())
                Inflict(target);
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (proj.CountsAsClass(DamageClass.Melee) || proj.CountsAsClass<RogueDamageClass>())
                Inflict(target);
        }

        public override void MeleeEffects(Item item, Microsoft.Xna.Framework.Rectangle hitbox)
        {
            if (Active && Main.rand.NextBool(3))
                Dust.NewDust(hitbox.TopLeft(), hitbox.Width, hitbox.Height, ModContent.DustType<CalamityMod.Dusts.BrimstoneFlame>(),
                    Player.velocity.X * 0.2f + Player.direction * 3f, Player.velocity.Y * 0.2f, 100, default, 0.75f);
        }
    }
}
