// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using CalamityFandom.Content.Legacy.Projectiles.Melee;
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Aftershock.txt — épée qui fait tomber un rocher du ciel à chaque coup (AftershockRock, perce 5 ennemis) et
    /// inflige Armor Crunch 5 s. Lâchée par les Earth Elemental (infobox, WikiLoot). Retirée de Calamity après 2.0.1.005.
    /// Valeurs du wiki : use 27, rareté 5, vente 7 or 20 argent ; élément par les données du wiki.
    /// </summary>
    public class Aftershock : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 65;
            Item.DamageType = DamageClass.Melee;
            Item.width = 54;
            Item.height = 58;
            Item.scale = 1.75f;
            Item.useTime = Item.useAnimation = 27;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 7.5f;
            Item.value = Item.sellPrice(gold: 7, silver: 20);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shootSpeed = 12f;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 300);
            SpawnRock(player, target.Center, hit.Knockback);
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 300);
            SpawnRock(player, target.Center, Item.knockBack);
        }

        /// <summary>
        /// Rocher lancé de 700 px au-dessus du joueur, vers l'ennemi touché (toujours vers le bas). [choix] Retour du 30/10 :
        /// l'ancien code visait le curseur, ce qui ne correspondait pas à l'ennemi frappé ; la page dit seulement « falls from
        /// the sky ».
        /// </summary>
        private void SpawnRock(Player player, Vector2 target, float knockback)
        {
            if (player.whoAmI != Main.myPlayer)
                return;
            Vector2 origin = new Vector2((player.Center.X + Main.rand.Next(201) * -player.direction + target.X) / 2f + Main.rand.Next(-200, 201),
                player.MountedCenter.Y - 700f);
            Vector2 aim = target - origin;
            aim.Y = System.Math.Max(System.Math.Abs(aim.Y), 20f);
            aim = Vector2.Normalize(aim) * Item.shootSpeed + new Vector2(0f, Main.rand.Next(-10, 11) * 0.02f);
            int damage = (int)player.GetTotalDamage(DamageClass.Melee).ApplyTo(Item.damage);
            Projectile.NewProjectile(player.GetSource_ItemUse(Item), origin, aim, ModContent.ProjectileType<AftershockRock>(), damage, knockback,
                player.whoAmI, 0f, Main.rand.Next(10));
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
                Dust.NewDust(hitbox.TopLeft(), hitbox.Width, hitbox.Height, 32);
        }
    }
}
