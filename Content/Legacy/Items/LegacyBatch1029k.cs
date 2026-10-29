// Adapté de Calamity Mod (objets retirés en 2022), © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using CalamityFandom.Content.Legacy.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.Weapons.Ranged
{
    /// <summary>
    /// Harpons multiples (Dualpoon, Triploon) : N harpons en éventail (18° d'écart) qui vont un peu loin puis reviennent
    /// au joueur, sans gravité ni chaîne visible (Trivia). TODO : l'Agent (PNJ inventé du wiki) les donne aussi.
    /// </summary>
    public abstract class MultiHarpoon : ModItem
    {
        protected abstract int Count { get; }
        protected abstract float Reach { get; }

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 center = player.RotatedRelativePoint(player.MountedCenter, true);
            Vector2 dir = velocity.SafeNormalize(Vector2.UnitX * player.direction) * Reach;
            bool canHit = Collision.CanHit(center, 0, 0, center + dir, 0, 0);
            for (int i = 0; i < Count; i++)
            {
                Vector2 offset = dir.RotatedBy(MathHelper.Pi * 0.1f * (i - (Count - 1f) / 2f));
                if (!canHit)
                    offset -= dir;
                Projectile.NewProjectile(source, center + offset, dir, type, damage, knockback, player.whoAmI);
            }
            return false;
        }
    }

    /// <summary>Wiki : Dualpoon.txt — deux harpons.</summary>
    public class Dualpoon : MultiHarpoon
    {
        protected override int Count => 2;
        protected override float Reach => 40f;

        public override void SetDefaults()
        {
            Item.damage = 70;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 56;
            Item.height = 28;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6.5f;
            Item.value = Item.sellPrice(gold: 7, silver: 20);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item10;
            Item.autoReuse = true;
            Item.shootSpeed = 20f;
            Item.shoot = ModContent.ProjectileType<DualpoonProj>();
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);
    }

    /// <summary>Wiki : Triploon.txt — trois harpons.</summary>
    public class Triploon : MultiHarpoon
    {
        protected override int Count => 3;
        protected override float Reach => 30f;

        public override void SetDefaults()
        {
            Item.damage = 70;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 46;
            Item.height = 24;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7.5f;
            Item.value = Item.sellPrice(gold: 12);
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = SoundID.Item10;
            Item.autoReuse = true;
            Item.shootSpeed = 20f;
            Item.shoot = ModContent.ProjectileType<TriploonProj>();
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.Ranged
{
    /// <summary>Harpon : file tout droit jusqu'à 350 px du joueur (ou au premier ennemi), puis revient à travers les blocs.</summary>
    public abstract class MultiHarpoonProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.alpha = 255;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.dead)
            {
                Projectile.Kill();
                return;
            }
            if (Projectile.alpha == 0)
                player.ChangeDir(Projectile.Center.X > player.Center.X ? 1 : -1);
            Projectile.extraUpdates = Projectile.ai[0] == 0f ? 0 : 1;
            Vector2 toPlayer = player.Center - Projectile.Center;
            float dist = toPlayer.Length();
            if (Projectile.ai[0] == 0f)
            {
                if (dist > 350f)
                    Projectile.ai[0] = 1f;
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                if (++Projectile.ai[1] > 5f)
                    Projectile.alpha = 0;
                if (Projectile.ai[1] > 8f)
                    Projectile.ai[1] = 8f;
            }
            else
            {
                Projectile.tileCollide = false;
                Projectile.rotation = toPlayer.ToRotation() - MathHelper.PiOver2;
                if (dist < 50f)
                    Projectile.Kill();
                Projectile.velocity = toPlayer.SafeNormalize(Vector2.Zero) * 20f;
            }
        }
    }

    public class DualpoonProj : MultiHarpoonProj { }

    public class TriploonProj : MultiHarpoonProj { }
}
