// Adapté de Calamity Mod (retiré le 11/06/2022, commit 186ddd241f), © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Terra-cotta.txt — épée retirée de Calamity avant 2.0.1.005 : chaque coup fait exploser l'ennemi ; un ennemi
    /// tué libère 3 orbes jaunes qui soignent de 9 PV (attirées seulement avec Heartreach, Notes). Lâchée par les Dune
    /// Splicer après Plantera (WikiLoot). Valeurs du wiki : use 14, rareté 8, vente 16 or ; éléments par les données du wiki.
    /// </summary>
    public class Terracotta : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 62;
            Item.scale = 1.5f;
            Item.damage = 110;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 14;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 7f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = Item.sellPrice(gold: 16);
            Item.rare = ItemRarityID.Yellow;
            Item.shootSpeed = 5f;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            var source = player.GetSource_ItemUse(Item);
            if (target.life <= 0)
            {
                float x = Main.rand.Next(3), y = Main.rand.Next(3, 5);
                foreach (float vx in new[] { -x, x, 0f })
                    Projectile.NewProjectile(source, target.Center, new Vector2(vx, -y), ModContent.ProjectileType<TerracottaProj>(), 0, 0f, player.whoAmI);
            }
            int blast = (int)player.GetTotalDamage(DamageClass.Melee).ApplyTo(Item.damage);
            Projectile.NewProjectile(source, target.Center, Vector2.Zero, ModContent.ProjectileType<TerracottaExplosion>(), blast, hit.Knockback, player.whoAmI);
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
                Dust.NewDust(hitbox.TopLeft(), hitbox.Width, hitbox.Height, 246);
        }
    }

    /// <summary>Orbe de soin du Terra-cotta : 9 PV au contact du joueur ; attirée vers lui seulement avec Heartreach.</summary>
    public class TerracottaProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.timeLeft = 360;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            Projectile.velocity *= 0.99f;
            if (owner.lifeMagnet && Projectile.timeLeft < 300)
                Projectile.velocity = (Projectile.velocity * 14f + (owner.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 8f) / 15f;
            if (owner.active && !owner.dead && Projectile.Hitbox.Intersects(owner.Hitbox))
            {
                if (Projectile.owner == Main.myPlayer)
                    owner.Heal(9);
                Projectile.Kill();
                return;
            }
            var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 246, 0f, 0f, 100);
            dust.noGravity = true;
            dust.position -= Projectile.velocity * 0.2f;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 246, 0f, 0f, 100, default, 2f);
                dust.velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    dust.scale = 0.5f;
                    dust.fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
        }
    }

    /// <summary>Explosion du Terra-cotta : gerbe de poussière dorée, 5 ticks.</summary>
    public class TerracottaExplosion : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 5;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.5f, 0.35f, 0f);
            for (int i = 0; i < 17; i++)
            {
                var dust = Dust.NewDustPerfect(Projectile.Center + new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11)), 246,
                    Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.Next(3, 9), 100, default, 1.5f);
                dust.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.immune[Projectile.owner] = 6;
    }
}
