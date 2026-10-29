// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using CalamityMod;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Projectiles.Melee
{
    /// <summary>Coup d'estoc de l'Aquatic Discharge : particules électriques, étincelles quand le coup tue (page du wiki).</summary>
    public class AquaticDischargeProj : BaseShortswordProjectile
    {
        public override string Texture => "CalamityFandom/Content/Legacy/Items/Weapons/Melee/AquaticDischarge";

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(16);
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.scale = 1f;
            Projectile.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Projectile.ownerHitCheck = true;
            Projectile.timeLeft = 360;
            Projectile.hide = true;
            // Un coup par estoc et par ennemi, comme les épées courtes de Calamity (sans ça : aller et retour)
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void SetVisualOffsets()
        {
            const int HalfSpriteWidth = 32 / 2;
            const int HalfSpriteHeight = 32 / 2;
            DrawOriginOffsetX = 0;
            DrawOffsetX = -(HalfSpriteWidth - Projectile.width / 2);
            DrawOriginOffsetY = -(HalfSpriteHeight - Projectile.height / 2);
        }

        public override void ExtraBehavior()
        {
            if (Main.rand.NextBool(5))
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Electric);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // « Upon killing an enemy with it, it will release small electric sparks which linger on the ground »
            if (target.life > 0 || Projectile.owner != Main.myPlayer)
                return;
            int sparkDamage = Main.player[Projectile.owner].CalcIntDamage<MeleeDamageClass>(0.5f * Projectile.damage);
            for (int i = 0; i < 3; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, new Vector2(Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-4f, -1f)),
                    ModContent.ProjectileType<AquaticDischargeSpark>(), sparkDamage, hit.Knockback, Projectile.owner);
        }
    }

    /// <summary>Étincelle qui retombe et reste un instant au sol (Spark dans le vieux code).</summary>
    public class AquaticDischargeSpark : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void AI()
        {
            Projectile.ai[0]++;
            if (Projectile.ai[0] > 5f)
            {
                Projectile.ai[0] = 5f;
                if (Projectile.velocity.Y == 0f && Projectile.velocity.X != 0f)
                {
                    Projectile.velocity.X *= 0.97f;
                    if (System.Math.Abs(Projectile.velocity.X) < 0.01f)
                    {
                        Projectile.velocity.X = 0f;
                        Projectile.netUpdate = true;
                    }
                }
                Projectile.velocity.Y = System.Math.Min(Projectile.velocity.Y + 0.2f, 16f);
            }
            var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 206, 0f, 0f, 100);
            dust.position += new Vector2(-2f, 2f);
            dust.scale += Main.rand.Next(50) * 0.01f;
            dust.noGravity = true;
            dust.velocity.Y -= 2f;
            if (Main.rand.NextBool(2))
            {
                var small = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 206, 0f, 0f, 100);
                small.position += new Vector2(-2f, 2f);
                small.scale += 0.3f + Main.rand.Next(50) * 0.01f;
                small.noGravity = true;
                small.velocity *= 0.1f;
            }
            if (Projectile.velocity.Y < 0.25f && Projectile.velocity.Y > 0.15f)
                Projectile.velocity.X *= 0.8f;
            Projectile.rotation = -Projectile.velocity.X * 0.05f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;
    }
}
