// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using CalamityMod;
using CalamityMod.Dusts;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Projectiles.Summon
{
    /// <summary>
    /// Tourelle de l'Energy Staff telle qu'en 2.0.1.005 (ProfanedEnergy) : immobile (« turret » selon le wiki ; la
    /// version actuelle de Calamity, refaite pour la Sanctified Spark, suit le joueur). Tire une Flame Blast ou une Flame
    /// Burst (encore dans Calamity) toutes les 16 frames sur l'ennemi visible le plus proche, ou la cible du joueur.
    /// </summary>
    public class ProfanedEnergyTurret : ModProjectile
    {
        private const float Range = 1000f;
        private const int Cooldown = 16;

        public override string Texture => "CalamityMod/NPCs/NormalNPCs/ImpiousImmolator";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.sentry = true;
            Projectile.timeLeft = Projectile.SentryLifeTime;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            if (++Projectile.frameCounter > 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
            }
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                SpawnEffects();
            }
            if (Projectile.owner != Main.myPlayer)
                return;
            if (Projectile.ai[0] > 0f)
            {
                Projectile.ai[0]--;
                return;
            }
            NPC target = FindTarget(Main.player[Projectile.owner]);
            if (target == null)
                return;
            Projectile.spriteDirection = target.Center.X < Projectile.Center.X ? 1 : -1;
            int type = Main.rand.NextBool() ? ModContent.ProjectileType<FlameBlast>() : ModContent.ProjectileType<FlameBurst>();
            Vector2 velocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitX) * 25f;
            var fire = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, velocity, type,
                Projectile.damage, Projectile.knockBack, Projectile.owner, target.whoAmI);
            fire.originalDamage = Projectile.originalDamage;
            Projectile.ai[0] = Cooldown;
        }

        /// <summary>Cible du joueur si elle est à portée et visible, sinon l'ennemi visible le plus proche (distance de Manhattan, vieux code).</summary>
        private NPC FindTarget(Player player)
        {
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(Projectile) && Distance(npc) < Range && Collision.CanHit(Projectile.Center, Projectile.width, Projectile.height, npc.Center, npc.width, npc.height))
                    return npc;
            }
            NPC best = null;
            float bestDistance = Range;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                float distance = Distance(npc);
                if (npc.CanBeChasedBy(Projectile) && distance < bestDistance && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
                {
                    bestDistance = distance;
                    best = npc;
                }
            }
            return best;
        }

        private float Distance(NPC npc) => System.Math.Abs(Projectile.Center.X - npc.Center.X) + System.Math.Abs(Projectile.Center.Y - npc.Center.Y);

        private void SpawnEffects()
        {
            SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
            for (int i = 0; i < 5; i++)
            {
                var holy = Dust.NewDustDirect(Projectile.Center, Projectile.width, Projectile.height, (int)CalamityDusts.ProfanedFire, 0f, 0f, 100, default, 2f);
                holy.velocity *= 3f;
                holy.position = Projectile.Center;
                if (Main.rand.NextBool())
                {
                    holy.scale = 0.5f;
                    holy.fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            for (int i = 0; i < 10; i++)
            {
                var fire = Dust.NewDustDirect(Projectile.Center, Projectile.width, Projectile.height, 246, 0f, 0f, 100, default, 3f);
                fire.noGravity = true;
                fire.velocity *= 5f;
                fire.position = Projectile.Center;
                fire = Dust.NewDustDirect(Projectile.Center, Projectile.width, Projectile.height, 246, 0f, 0f, 100, default, 2f);
                fire.velocity *= 2f;
                fire.position = Projectile.Center;
            }
        }

        public override bool? CanDamage() => false;
    }
}
