// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using CalamityFandom.Content.Legacy.Projectiles.Magic;
using CalamityMod;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

// Alpha Ray (13/11) : code de 2.0.1.005, valeurs de l'infobox et du texte de la page ; recette du wiki (avec la Genesis
// de Calamity, la « Genisis » du wiki).

namespace CalamityFandom.Content.Legacy.Items.Weapons.Magic
{
    /// <summary>
    /// Wiki : Alpha Ray.txt — clic gauche : trois rayons de particules parallèles (80 % des dégâts, perçants) et des lasers
    /// (40 % des dégâts et du recul) ; clic droit : un rayon qui se sépare en deux comme la Genisis (166,25 %) et trois
    /// lasers. Tout traverse les blocs.
    /// </summary>
    public class AlphaRay : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 130;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 5;
            Item.width = 84;
            Item.height = 74;
            Item.useTime = Item.useAnimation = 4;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1.5f;
            Item.value = Item.sellPrice(gold: 28);
            Item.UseSound = SoundID.Item33;
            Item.autoReuse = true;
            Item.shootSpeed = 6f;
            Item.shoot = ModContent.ProjectileType<ParticleBeamofDoom>();
            Item.rare = ModContent.RarityType<CosmicPurple>();
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);

        public override bool AltFunctionUse(Player player) => true;

        private static void Laser(EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int damage, float knockback, Player player)
        {
            int laser = Projectile.NewProjectile(source, position, velocity, ProjectileID.LaserMachinegunLaser, (int)(damage * 0.4f), knockback * 0.4f, player.whoAmI);
            if (!laser.WithinBounds(Main.maxProjectiles))
                return;
            Main.projectile[laser].timeLeft = 120;
            Main.projectile[laser].tileCollide = false;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(source, position, velocity * 1.35f, ModContent.ProjectileType<BigBeamofDeath>(), (int)(damage * 1.6625f), knockback, player.whoAmI);
                Vector2 spread = velocity + Main.rand.NextVector2Square(-1f, 1f);
                for (int i = 0; i < 3; i++)
                    Laser(source, position, spread * 1.15f, damage, knockback, player);
                return false;
            }
            Vector2 center = player.RotatedRelativePoint(player.MountedCenter, true);
            Vector2 offset = Vector2.Normalize(velocity) * 80f;
            bool clear = Collision.CanHit(center, 0, 0, center + offset, 0, 0);
            for (int i = 0; i < 3; i++)
            {
                Vector2 o = offset.RotatedBy(0.314159274f * (i - 1));
                if (!clear)
                    o -= offset;
                Projectile.NewProjectile(source, center + o, velocity * 1.5f, type, (int)(damage * 0.8f), knockback, player.whoAmI);
                Laser(source, center + o, velocity * 2f, damage, knockback, player);
            }
            return false;
        }
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.Magic
{
    /// <summary>Rayon de particules de l'Alpha Ray (clic gauche) : trait de poussière, perce sans fin.</summary>
    public class ParticleBeamofDoom : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 100;
            Projectile.timeLeft = 100;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        public override void AI()
        {
            if (++Projectile.localAI[0] > 9f)
                BeamDust(Projectile, 2.5f);
        }

        public static void BeamDust(Projectile projectile, float scale)
        {
            if (Main.dedServ)
                return;
            Vector2 pos = projectile.position - projectile.velocity * 0.25f;
            Dust d = Dust.NewDustDirect(pos, 1, 1, 206, 0f, 0f, 0, default, scale);
            d.position = pos;
            d.velocity *= 0.1f;
        }
    }

    /// <summary>Gros rayon de l'Alpha Ray (clic droit) : se sépare en deux rayons à sa portée maximale.</summary>
    public class BigBeamofDeath : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 100;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 100;
            Projectile.timeLeft = 80;
        }

        public override void AI()
        {
            if (++Projectile.localAI[0] > 9f || Projectile.ai[0] == 1f)
                ParticleBeamofDoom.BeamDust(Projectile, 3f);
        }

        public override void OnKill(int timeLeft)
        {
            // ai[0] = 1 : rayon issu de la séparation (ne se sépare plus)
            if (Projectile.ai[0] == 1f || Projectile.owner != Main.myPlayer)
                return;
            float rotation = MathHelper.ToRadians(20);
            for (int i = 0; i < 2; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.RotatedBy(i == 0 ? -rotation : rotation),
                    ModContent.ProjectileType<BigBeamofDeath2>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 1f);
        }
    }

    /// <summary>Les deux rayons nés de la séparation (type à part : leurs propres temps d'invulnérabilité, comme le vieux code).</summary>
    public class BigBeamofDeath2 : BigBeamofDeath
    {
    }
}
