// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using System;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Projectiles.Melee
{
    /// <summary>
    /// Wiki : Cosmic Shiv.txt — estoc du Cosmic Shiv de 2.0.1.005 (Full Fandom, Common/Legacy/FullFandomLegacyWeapons) : tire un
    /// orbe cosmique à tête chercheuse (moitié des dégâts) ; contact : Brimstone Flames, Frostburn, Holy Flames et God Slayer
    /// Inferno 2 s (page).
    /// </summary>
    public class CosmicShivProj : BaseShortswordProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Melee/CosmicShiv";

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(24);
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Projectile.ownerHitCheck = true;
            Projectile.timeLeft = 360;
            Projectile.extraUpdates = 1;
            Projectile.hide = true;
            // Un coup par estoc (le vieux code touchait à l'aller et au retour)
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override Action<Projectile> EffectBeforePullback => proj =>
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 14f,
                ModContent.ProjectileType<CosmicShivBall>(), (int)(Projectile.damage * 0.5f), Projectile.knockBack, Projectile.owner);
        };

        public override void SetVisualOffsets()
        {
            const int HalfSpriteWidth = 48 / 2;
            const int HalfSpriteHeight = 48 / 2;
            DrawOriginOffsetX = 0;
            DrawOffsetX = -(HalfSpriteWidth - Projectile.width / 2);
            DrawOriginOffsetY = -(HalfSpriteHeight - Projectile.height / 2);
        }

        public override void ExtraBehavior() => Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 173);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int k = 0; k < 36; k++)
            {
                var dust = Dust.NewDustDirect(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height - 16, 173);
                dust.velocity *= 3f;
                dust.scale *= 2f;
            }
            CosmicShivBall.Inflict(target, 120);
        }
    }

    /// <summary>
    /// Orbe cosmique : ondule tant qu'il n'a pas de cible, puis fonce sur l'ennemi le plus proche. Au contact : 3 lames
    /// arrivent du bord de l'écran et 6 à 10 étoiles tombent sur l'ennemi ; à sa disparition, explosion de 80 px.
    /// </summary>
    public class CosmicShivBall : ModProjectile
    {
        private const float MaxDistanceToTarget = 540f;
        private const float ExplosionDamageMultiplier = 1.8f;
        private const float ExplosionRadius = 80f;

        private NPC target;
        private bool initialized;
        private float startingVelocityY, randomAngleDelta;

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        /// <summary>Les quatre debuffs de l'arme (page : 2 s au contact, 1 s pour l'orbe et les lames).</summary>
        public static void Inflict(NPC target, int ticks)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), ticks);
            target.AddBuff(BuffID.Frostburn, ticks);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), ticks);
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), ticks);
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 220;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
        }

        public override void AI()
        {
            if (!initialized)
            {
                target = Projectile.Center.ClosestNPCAt(MaxDistanceToTarget);
                startingVelocityY = Projectile.velocity.Y;
                randomAngleDelta = Main.rand.NextFloat(MathHelper.TwoPi);
                initialized = true;
            }
            Projectile.localAI[0]++;
            if (Projectile.localAI[0] > 4f)
            {
                for (int i = 0; i < 3; i++)
                {
                    var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 173, Projectile.direction * 2f, 0f, 115, Color.White, 1.3f);
                    dust.noGravity = true;
                    dust.velocity *= 0f;
                }
                Projectile.velocity.Y *= 0.965f;
            }
            if (Projectile.localAI[0] % 30 == 0)
                target = Projectile.Center.ClosestNPCAt(MaxDistanceToTarget);
            if (target != null && target.active)
            {
                const float Inertia = 35f, HomingSpeed = 33.5f;
                Vector2 ideal = Projectile.SafeDirectionTo(target.Center, Vector2.UnitX) * HomingSpeed;
                Projectile.velocity = (Projectile.velocity * (Inertia - 1f) + ideal) / Inertia;
            }
            else
            {
                // Ondulation (« travel in a sine-wave pattern »)
                Projectile.ai[0]++;
                Projectile.velocity.Y = startingVelocityY + (float)(Math.Cos(Projectile.ai[0] / 12.0 + randomAngleDelta) * 7.0);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                // Lames venues du bord de l'écran
                for (int i = 0; i < 3; i++)
                {
                    int side = Main.rand.NextBool().ToDirectionInt();
                    Vector2 spawn = new(target.Center.X + side * 650, Projectile.Center.Y + Main.rand.Next(-500, 501));
                    Vector2 velocity = Vector2.Normalize(target.Center - spawn) * 30f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawn, velocity, ModContent.ProjectileType<CosmicShivBlade>(),
                        Projectile.damage, Projectile.knockBack * 0.1f, Projectile.owner);
                }
                // Pluie d'étoiles
                int starMax = Main.rand.Next(6, 11);
                for (int i = -starMax / 2; i < starMax / 2; i++)
                {
                    Vector2 spawn = target.Center - new Vector2(0f, 800f + Main.rand.Next(-40, 41)).RotatedBy(MathHelper.ToRadians(i * 11f / starMax));
                    Vector2 velocity = Vector2.Normalize(target.Center - spawn) * 35f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawn, velocity, ModContent.ProjectileType<GalaxyStar>(),
                        Projectile.damage / 2, Projectile.knockBack * 0.5f, Projectile.owner);
                }
            }
            Inflict(target, 60);
        }

        public override void OnKill(int timeLeft)
        {
            // Étoile de poussière, puis explosion (le vieux code frappait les ennemis à la main : ici, la hitbox grandit)
            float rand2PI = Main.rand.NextFloat(MathHelper.TwoPi);
            const int PetalCount = 5;
            const float Speed = 12f;
            float scale = Main.rand.NextFloat(1f, 1.35f);
            for (float k = 0f; k < MathHelper.TwoPi; k += 0.05f)
            {
                Vector2 velocity = k.ToRotationVector2() * (2f + (float)(Math.Sin(rand2PI + k * PetalCount) + 1.0) * Speed) * Main.rand.NextFloat(0.95f, 1.05f);
                Dust.NewDustPerfect(Projectile.Center, 173, velocity, 0, default, scale).customData = 0.025f;
            }
            if (Projectile.owner != Main.myPlayer)
                return;
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = (int)(ExplosionRadius * 2f);
            Projectile.position -= Projectile.Size / 2f;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            for (int i = 0; i < Projectile.localNPCImmunity.Length; i++)
                Projectile.localNPCImmunity[i] = 0;
            Projectile.damage = (int)(Projectile.damage * ExplosionDamageMultiplier);
            Projectile.Damage();
        }
    }

    /// <summary>Lame cosmique : arrive en arc, rebondit sur chaque ennemi touché en grandissant (12 coups), 1/15 des dégâts après le premier.</summary>
    public class CosmicShivBlade : ModProjectile
    {
        private const int PenetrateMax = 12;
        private const float MaxScale = 1.8f;

        private bool initialized;
        private float startYVelSign;

        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.friendly = true;
            Projectile.penetrate = PenetrateMax;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 290;
            Projectile.DamageType = DamageClass.Melee;
            // Toutes les lames peuvent toucher le même ennemi (le vieux code remettait son immunité à 0)
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (!initialized)
            {
                startYVelSign = Math.Sign(Projectile.velocity.Y) * 0.35f;
                initialized = true;
            }
            if (Projectile.penetrate == PenetrateMax && Projectile.timeLeft < 245)
            {
                Projectile.velocity.Y -= startYVelSign;
                if (Math.Abs(Projectile.velocity.X) < 30f)
                    Projectile.velocity.X *= 1.04f;
            }
            if (Projectile.velocity != Vector2.Zero)
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            else
                Projectile.rotation += MathHelper.Pi / 6f / (Projectile.scale * 2.5f);
            Projectile.ai[0]++;
            if (Projectile.ai[1] >= 0)
            {
                Projectile.ai[1]--;
                if (Projectile.ai[1] == 0)
                    Projectile.velocity *= -0.9f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.penetrate == PenetrateMax)
                Projectile.damage /= 15;
            else
                Projectile.scale += (MaxScale - 1f) / PenetrateMax;
            Projectile.ai[1] = 5 + Main.rand.Next(-2, 3);
            CosmicShivBall.Inflict(target, 60);
        }

        public override void OnKill(int timeLeft)
        {
            Vector2 point = Vector2.One.RotatedByRandom(MathHelper.ToRadians(32f));
            float lerpStart = Main.rand.Next(12, 17);
            float lerpEnd = Main.rand.Next(3, 7);
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    Vector2 rotated = point.RotatedBy((j == 0 ? 1 : -1) * MathHelper.TwoPi / 18);
                    for (float k = 0f; k < 20f; k++)
                    {
                        var dust = Dust.NewDustDirect(Projectile.Center, 0, 0, 173, 0f, 0f, 100, default, 1.1f);
                        dust.velocity = dust.velocity * 0.1f + Vector2.Lerp(point, rotated, k / 20f) * MathHelper.Lerp(lerpStart, lerpEnd, k / 20f) * 4f;
                        dust.noGravity = true;
                    }
                }
                point = point.RotatedBy(MathHelper.TwoPi / 9);
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(124, Main.DiscoG, 255, Projectile.alpha);
    }

    /// <summary>Étoile qui tombe sur l'ennemi touché par l'orbe (son de la Starfury).</summary>
    public class GalaxyStar : ModProjectile
    {
        private bool playedSound;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 160;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 1f, 1f, 1f);
            if (!playedSound)
            {
                SoundEngine.PlaySound(SoundID.Item9, Projectile.position);
                playedSound = true;
            }
            Projectile.ai[0]++;
            if (Projectile.ai[0] % 5 == 0)
            {
                for (int i = 0; i < Main.rand.Next(2, 4); i++)
                {
                    Vector2 velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * 0.7f;
                    Dust.NewDust(Projectile.Center, 4, 4, 58, velocity.X, velocity.Y);
                }
            }
            Projectile.rotation += Projectile.velocity.Length() / 19f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 2);
            return false;
        }
    }
}
