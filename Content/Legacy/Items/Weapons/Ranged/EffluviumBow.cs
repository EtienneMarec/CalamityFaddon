// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using CalamityMod;
using CalamityFandom.Content.Legacy.Buffs.StatDebuffs;
using CalamityFandom.Content.Legacy.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.Weapons.Ranged
{
    /// <summary>
    /// Wiki : Effluvium Bow.txt — arc qui tire deux flèches ; les flèches en bois deviennent des Mist Arrow (celles de
    /// 2.0.1.005 : Frostburn 3 s, Glacial State 0,5 s ; celles de Calamity 2.2.2 se divisent en brumes). Retiré de Calamity
    /// après 2.0.1.005. Valeurs du wiki : rareté 5, vente 7 or 20 argent.
    /// Recette et éléments (Cold/Water) par les données du wiki.
    /// </summary>
    public class EffluviumBow : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 56;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 26;
            Item.height = 70;
            Item.useTime = Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4f;
            Item.value = Item.sellPrice(gold: 7, silver: 20);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<LegacyMistArrow>();
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.Arrow;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 2; i++)
            {
                Vector2 spread = velocity + new Vector2(Main.rand.Next(-30, 31), Main.rand.Next(-30, 31)) * 0.05f;
                if (CalamityUtils.CheckWoodenAmmo(type, player))
                    Projectile.NewProjectile(source, position, spread, ModContent.ProjectileType<LegacyMistArrow>(), damage, knockback, player.whoAmI);
                else
                    Projectile.NewProjectileDirect(source, position, spread, type, damage, knockback, player.whoAmI).noDropItem = true;
            }
            return false;
        }
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.Ranged
{
    /// <summary>Mist Arrow de 2.0.1.005 (Effluvium Bow) : flèche de brume animée, Frostburn 3 s et Glacial State 0,5 s.</summary>
    public class LegacyMistArrow : ModProjectile
    {
        public override void SetStaticDefaults() => Main.projFrames[Type] = 3;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.timeLeft = 300;
            AIType = ProjectileID.WoodenArrowFriendly;
            Projectile.arrow = true;
            Projectile.coldDamage = true;
        }

        public override void AI()
        {
            if (++Projectile.frameCounter > 9)
            {
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
                Projectile.frameCounter = 0;
            }
            Projectile.alpha = System.Math.Max(5, Projectile.alpha - 15);
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi) + MathHelper.PiOver2 * Projectile.direction;
            if (++Projectile.localAI[0] > 4f)
            {
                var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 31, 0f, 0f, 100, new Color(237, 242, 242, 200), 1.2f);
                dust.noGravity = true;
                dust.velocity = -Projectile.velocity * Main.rand.NextFloat(0.3f, 0.6f);
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 31, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f, 100, new Color(237, 242, 242, 200), 1f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Frostburn, 180);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 30);
        }
    }
}
