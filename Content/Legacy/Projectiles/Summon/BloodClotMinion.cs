// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using System;
using CalamityFandom.Content.Legacy.Buffs.Summon;
using CalamityMod;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Projectiles.Summon
{
    /// <summary>
    /// Caillot de sang du Blood Clot Staff : vole près du joueur, fonce sur l'ennemi le plus proche (ou la cible désignée)
    /// à 1300 px, recule brièvement après chaque coup. IA de 2.0.1.005, simplifiée.
    /// </summary>
    public class BloodClotMinion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.minionSlots = 1;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.minion = true;
            Projectile.tileCollide = false;
            Projectile.MaxUpdates = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = Projectile.MaxUpdates * 24;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.dead || !player.active)
                player.ClearBuff(ModContent.BuffType<BloodClot>());
            if (player.HasBuff(ModContent.BuffType<BloodClot>()))
                Projectile.timeLeft = 2;

            var blood = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Blood);
            blood.velocity *= 0.1f;
            blood.scale = 0.7f;
            blood.noGravity = true;
            if (++Projectile.frameCounter > 5)
            {
                Projectile.frame = (Projectile.frame + 1) % 4;
                Projectile.frameCounter = 0;
            }
            // Les caillots s'écartent les uns des autres (MinionAntiClump de Calamity, internal)
            foreach (var other in Main.ActiveProjectiles)
            {
                if (other.whoAmI != Projectile.whoAmI && other.owner == Projectile.owner && other.type == Type
                    && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
                    Projectile.velocity += (Projectile.Center - other.Center).SafeNormalize(Vector2.UnitX) * 0.05f;
            }

            // Trop loin : retour au joueur
            if (Math.Abs(Projectile.Center.X - player.Center.X) + Math.Abs(Projectile.Center.Y - player.Center.Y) > (Projectile.ai[1] != 0f ? 1800 : 1100))
                Projectile.ai[0] = 1f;

            NPC target = null;
            if (Projectile.ai[0] == 0f)
            {
                if (player.HasMinionAttackTargetNPC && Main.npc[player.MinionAttackTargetNPC].CanBeChasedBy(Projectile)
                    && Projectile.Distance(Main.npc[player.MinionAttackTargetNPC].Center) < 1300f)
                    target = Main.npc[player.MinionAttackTargetNPC];
                else
                    target = Projectile.Center.ClosestNPCAt(1300f, false);
            }

            if (target == null)
            {
                float speed = Projectile.ai[0] == 1f ? 12f : 8f;
                Vector2 toPlayer = player.Center - new Vector2(0f, 60f) - Projectile.Center;
                float distance = toPlayer.Length();
                if (distance < 100f && Projectile.ai[0] == 1f)
                    Projectile.ai[0] = 0f;
                if (distance > 2000f)
                    Projectile.Center = player.Center;
                if (distance > 70f)
                    Projectile.velocity = (Projectile.velocity * 20f + toPlayer.SafeNormalize(Vector2.Zero) * speed) / 21f;
                else
                {
                    if (Projectile.velocity == Vector2.Zero)
                        Projectile.velocity = new Vector2(-0.15f, -0.05f);
                    Projectile.velocity *= 1.01f;
                }
            }
            else
            {
                if (Projectile.ai[1] == -1f)
                    Projectile.ai[1] = 11f;
                if (Projectile.ai[1] > 0f)
                    Projectile.ai[1] -= 1f;
                if (Projectile.ai[1] == 0f)
                {
                    float speed = Projectile.Distance(target.Center) < 100f ? 10f : 8f;
                    Projectile.velocity = (Projectile.velocity * 40f + (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * speed) / 41f;
                }
                else if (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y) < 10f)
                    Projectile.velocity *= 1.05f;
            }
            Projectile.rotation = Projectile.velocity.X * 0.05f;
            if (Math.Abs(Projectile.velocity.X) > 0.2f)
                Projectile.spriteDirection = -Projectile.direction;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.ai[1] = -1f;
            Projectile.netUpdate = true;
        }
    }
}
