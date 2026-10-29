// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using System;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Projectiles.Rogue
{
    /// <summary>Lance de Night's Gaze : se brise sur un ennemi en étoiles et étincelles ; en furtivité, sème des étoiles à tête chercheuse.</summary>
    public class NightsGazeProjectile : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Legacy/Items/Weapons/Rogue/NightsGaze";

        // Wiki : Moon Burnt 5 secondes pour la lance (l'ancien code utilisait le temps de vie restant)
        private const int SpearDebuffTime = 300;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
        }

        private int SplitProjDamage => (int)(Projectile.damage * 0.6f);

        public override void AI()
        {
            if (Projectile.ai[0] == 0f)
                Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.ToRadians(45);

            if (Projectile.Calamity().stealthStrike && Main.rand.NextBool(8) && Projectile.owner == Main.myPlayer)
            {
                int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity,
                    ModContent.ProjectileType<NightsGazeStar>(), SplitProjDamage, 5f, Projectile.owner, 1f, 0f);
                if (p.WithinBounds(Main.maxProjectiles))
                    Main.projectile[p].penetrate = 1;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Nightwither>(), SpearDebuffTime);
            OnHitEffects();
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<Nightwither>(), SpearDebuffTime);
            OnHitEffects();
        }

        private void OnHitEffects()
        {
            if (Projectile.owner == Main.myPlayer)
            {
                int sparkID = ModContent.ProjectileType<NightsGazeSpark>();
                int starID = ModContent.ProjectileType<NightsGazeStar>();
                for (int i = 0; i < 6; i++)
                {
                    int projID = Main.rand.NextBool(2) ? starID : sparkID;
                    Vector2 velocity = Projectile.oldVelocity.RotatedByRandom(MathHelper.ToRadians(20f)) * Main.rand.NextFloat(1.5f, 2f);
                    float moveDuration = Main.rand.Next(5, 15);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, projID, SplitProjDamage, 5f, Projectile.owner, 0f, moveDuration);
                }
            }

            SoundEngine.PlaySound(SoundID.Item62 with { Volume = SoundID.Item62.Volume * 0.6f }, Projectile.position);
            SoundEngine.PlaySound(SoundID.Item68 with { Volume = SoundID.Item68.Volume * 0.2f }, Projectile.position);
            SoundEngine.PlaySound(SoundID.Item122 with { Volume = SoundID.Item122.Volume * 0.4f }, Projectile.position);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            Projectile.Kill();
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture + "Glow").Value;
            Vector2 origin = texture.Size() / 2f;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin, 1f, SpriteEffects.None, 0);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                int dustType = Utils.SelectRandom(Main.rand, 109, 111, 132);
                int dust = Dust.NewDust(Projectile.Center, 1, 1, dustType, Projectile.velocity.X / 3f, Projectile.velocity.Y / 3f, 0, default, 1.5f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
