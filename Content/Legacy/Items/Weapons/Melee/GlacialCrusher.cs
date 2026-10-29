// Adapté de Calamity Mod (retiré le 11/06/2022, commit b8296cc94a), © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using CalamityFandom.Content.Legacy.Buffs.StatDebuffs;
using CalamityFandom.Content.Legacy.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Glacial Crusher.txt — épée retirée de Calamity avant 2.0.1.005 : tire un iceberg lent qui gèle (Glacial State
    /// 0,5 s) ; la lame fait deux fois plus de dégâts (et trois fois plus de recul) aux ennemis gelés. Lâchée par Cryogen
    /// (WikiLoot). Valeurs du wiki : use 26, rareté 5, vente 7 or 20 argent ; éléments par les données du wiki.
    /// </summary>
    public class GlacialCrusher : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 60;
            Item.height = 58;
            Item.damage = 120;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 26;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.scale = 2f;
            Item.value = Item.sellPrice(gold: 7, silver: 20);
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ModContent.ProjectileType<Iceberg>();
            Item.shootSpeed = 10f;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
                Dust.NewDust(hitbox.TopLeft(), hitbox.Width, hitbox.Height, 67);
        }

        public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.HasBuff<GlacialState>())
            {
                modifiers.SourceDamage *= 2f;
                modifiers.Knockback *= 3f;
            }
        }
    }
}
