// Adapté de Calamity Mod (version mensuelle de 2022, legacy_extract.py), © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.NPCs.StormWeaver;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Projectiles.Rogue
{
    /// <summary>
    /// Cellule de l'Alpha Virus : aura de 100 px qui touche à travers les blocs (1/5 des dégâts, sans recul), cellule au
    /// contact (Plague 2 s) ; 6 seekers à la mort. Frappe furtive : ralentit jusqu'à 2 et sème un seeker toutes les 15
    /// mises à jour, accroché à la cellule.
    /// </summary>
    public class AlphaVirusProjectile : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Legacy/Items/Weapons/Rogue/AlphaVirus";

        public const int Lifetime = 600;
        private const float FinalVelocity = 2f;
        private const float DecelerationRate = 0.07f;
        private const float Radius = 100f;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 5;
            Projectile.timeLeft = Lifetime;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Projectile.rotation += 0.05f * Projectile.direction;
            if (!Projectile.Calamity().stealthStrike)
            {
                Projectile.timeLeft--;
                return;
            }
            if (Projectile.ai[0] > FinalVelocity)
            {
                Projectile.ai[0] = MathHelper.Max(Projectile.ai[0] - DecelerationRate, FinalVelocity);
                Projectile.velocity = Vector2.Normalize(Projectile.velocity) * Projectile.ai[0];
            }
            if (Projectile.owner == Main.myPlayer && Projectile.timeLeft < Lifetime - 30 && Projectile.timeLeft % 15 == 0 && Projectile.ai[0] <= FinalVelocity)
            {
                // ai[0] = 1 : le seeker reste accroché à la cellule (ai[1] = son identité)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2CircularEdge(5f, 5f),
                    ModContent.ProjectileType<AlphaSeeker>(), (int)(Projectile.damage * 0.125f), 1f, Projectile.owner, 1f, Projectile.identity);
            }
        }

        /// <summary>Distance du centre de la cellule au coin le plus proche de la cible (contact si ≤ largeur).</summary>
        private float CornerDistance(Rectangle box)
        {
            float d = Vector2.Distance(Projectile.Center, box.TopLeft());
            d = MathHelper.Min(d, Vector2.Distance(Projectile.Center, box.TopRight()));
            d = MathHelper.Min(d, Vector2.Distance(Projectile.Center, box.BottomLeft()));
            return MathHelper.Min(d, Vector2.Distance(Projectile.Center, box.BottomRight()));
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.type == ModContent.NPCType<StormWeaverHead>() || target.type == ModContent.NPCType<StormWeaverBody>() || target.type == ModContent.NPCType<StormWeaverTail>())
                modifiers.SourceDamage /= 5f;
            if (CornerDistance(target.Hitbox) > Projectile.width)
            {
                modifiers.SourceDamage /= 5f;
                modifiers.Knockback *= 0f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (CornerDistance(target.Hitbox) <= Projectile.width)
                target.AddBuff(ModContent.BuffType<Plague>(), 120);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (CornerDistance(target.Hitbox) <= Projectile.width)
                target.AddBuff(ModContent.BuffType<Plague>(), 120);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D aura = ModContent.Request<Texture2D>("CalamityFandom/Content/Legacy/Projectiles/Rogue/AlphaVirusAura").Value;
            float transparency = 0.1f;
            if (Projectile.timeLeft > Lifetime - 10)
                transparency = (Lifetime - Projectile.timeLeft) * (0.1f / 10);
            else if (Projectile.timeLeft < 25)
                transparency = Projectile.timeLeft * (0.1f / 25);
            for (int i = 0; i < 10; i++)
                Main.EntitySpriteDraw(aura, Projectile.Center - Main.screenPosition, null, Color.White * transparency,
                    -(Projectile.rotation * 0.2f) + 0.03f * i * i, aura.Size() / 2f, Projectile.scale - i * 0.03f, SpriteEffects.None, 0);

            for (int i = 0; i < (Lifetime - Projectile.timeLeft) / 30; i++)
            {
                Vector2 pos = new Vector2(0f, -Main.rand.NextFloat(Projectile.width / 2, Radius)).RotatedByRandom(MathHelper.TwoPi);
                Dust dust = Dust.NewDustDirect(Projectile.Center + pos, 1, 1, 89, 0f, 0f, 100);
                dust.noGravity = true;
                dust.velocity = -pos * 0.02f + Projectile.velocity;
            }

            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation,
                tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            return true;
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer)
                for (int i = 0; i < 6; i++)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (MathHelper.Pi * i / 3f).ToRotationVector2() * 10f,
                        ModContent.ProjectileType<AlphaSeeker>(), Projectile.damage, 1f, Projectile.owner);
            for (int i = 0; i < 20; i++)
            {
                Vector2 velocity = new Vector2(0f, -Main.rand.NextFloat(Projectile.width / 2, Radius)).RotatedByRandom(MathHelper.TwoPi) * 0.1f;
                Dust dust = Dust.NewDustDirect(Projectile.Center, 1, 1, 89, 0f, 0f, 100);
                dust.noGravity = true;
                dust.velocity = velocity + Projectile.velocity;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) =>
            CalamityUtils.CircularHitboxCollision(Projectile.Center, Radius, targetHitbox);
    }

    /// <summary>Plague seeker de l'Alpha Virus : tête chercheuse (400 px), Plague 3 s ; accroché à la cellule si ai[0] = 1.</summary>
    public class AlphaSeeker : ModProjectile
    {
        private const float MaxSpeed = 15f;
        private const float HomingStartRange = 400f;
        private const float MoveSpeed = 2f;
        private const float RotateSpeed = 0.04f;
        private const int ReturnTime = 60;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.extraUpdates = 1;
        }

        public override void OnSpawn(Terraria.DataStructures.IEntitySource source)
        {
            Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            Projectile.localAI[1] = Main.rand.NextFloat(-RotateSpeed, RotateSpeed);
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 1f)
            {
                // localAI[0] : distance à la cellule, qui grandit puis revient
                Projectile.tileCollide = false;
                Projectile parent = null;
                foreach (var p in Main.ActiveProjectiles)
                    if (p.identity == Projectile.ai[1])
                        parent = p;
                if (parent == null)
                {
                    Projectile.Kill();
                    return;
                }
                Projectile.Center = parent.Center + new Vector2(0f, Projectile.localAI[0]).RotatedBy(Projectile.rotation);
                Projectile.rotation += Projectile.localAI[1];
                Projectile.localAI[0] += Projectile.timeLeft > ReturnTime ? MoveSpeed : -MoveSpeed;
                if (Projectile.timeLeft % 6 == 0 && AcquireTarget() is int found and >= 0)
                {
                    Projectile.ai[0] = 0f;
                    Projectile.ai[1] = found + 1f;
                }
            }
            else
                HomingAI();

            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 89, 0f, 0f, 100, default, 2f);
            dust.noGravity = true;
            dust.velocity.Y = -0.15f;
        }

        private void HomingAI()
        {
            int targetID = (int)Projectile.ai[1] - 1;
            if (targetID < 0 || !Main.npc[targetID].active)
                targetID = AcquireTarget();
            Projectile.ai[1] = targetID + 1f;
            if (targetID < 0)
                return;
            Vector2 velocity = Projectile.velocity + (Main.npc[targetID].Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 2.5f;
            if (velocity.Length() >= MaxSpeed)
                velocity = velocity.SafeNormalize(Vector2.Zero) * MaxSpeed;
            Projectile.velocity = velocity;
        }

        private int AcquireTarget()
        {
            int target = -1;
            float minDist = HomingStartRange;
            foreach (var npc in Main.ActiveNPCs)
            {
                if (npc.type == NPCID.TargetDummy || !npc.CanBeChasedBy(Projectile))
                    continue;
                float dist = Vector2.Distance(Projectile.Center, npc.Center);
                if (dist < minDist)
                {
                    minDist = dist;
                    target = npc.whoAmI;
                }
            }
            return target;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Plague>(), 180);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<Plague>(), 180);
    }
}
