// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using CalamityMod;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Projectiles.Rogue
{
    /// <summary>Grenade lancée : explose au contact (sol ou ennemi) en 5 à 10 éclairs, onde de choc, aura si furtive.</summary>
    public class ShockGrenadeProjectile : ModProjectile
    {
        private const string ItemTexture = "CalamityFandom/Content/Legacy/Items/Weapons/Rogue/ShockGrenade";

        public override string Texture => ItemTexture;

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
        }

        public override void AI()
        {
            Projectile.velocity.Y = System.Math.Min(Projectile.velocity.Y + 0.1f, 16f);
            Projectile.rotation += Projectile.direction * 0.2f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            // Côté touché : les éclairs repartent dans l'autre sens
            Projectile.localAI[0] = Projectile.velocity.X != oldVelocity.X ? -System.Math.Sign(oldVelocity.X) : 0;
            Projectile.localAI[1] = Projectile.velocity.Y != oldVelocity.Y ? -System.Math.Sign(oldVelocity.Y) : 0;
            Projectile.Kill();
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, texture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D glow = ModContent.Request<Texture2D>(ItemTexture + "Glow").Value;
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, glow.Size() / 2f, 1f, SpriteEffects.None, 0);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item94 with { Volume = SoundID.Item94.Volume * 0.75f }, Projectile.position);
            if (Projectile.owner != Main.myPlayer)
                return;
            bool stealth = Projectile.Calamity().stealthStrike;
            int bolts = Main.rand.Next(5, 11);
            for (int i = 0; i < bolts; i++)
            {
                var velocity = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-2f, 2f));
                if (Projectile.localAI[0] != 0)
                    velocity.X *= -1;
                if (Projectile.localAI[1] != 0)
                    velocity.Y *= -1;
                velocity += new Vector2(Projectile.localAI[0], Projectile.localAI[1] * 2);
                velocity = Vector2.Normalize(velocity) * 10f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<ShockGrenadeBolt>(),
                    Projectile.damage / 2, 0, Projectile.owner, Main.rand.Next(0, 2), stealth ? 1 : 0);
            }
            if (stealth)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ShockTeslaAura>(), Projectile.damage / 4, 1, Projectile.owner);
                SoundEngine.PlaySound(SoundID.Item93 with { Volume = SoundID.Item93.Volume * 0.5f }, Projectile.position);
            }
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ShockGrenadeExplosion>(), Projectile.damage, 3, Projectile.owner);
        }
    }

    /// <summary>Éclair de la grenade (deux apparences selon ai[0]) ; à tête chercheuse si l'attaque était furtive (ai[1]).</summary>
    public class ShockGrenadeBolt : ModProjectile
    {
        private const int FrameWidth = 12;
        private const int FrameHeight = 26;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 120;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
        }

        public override void AI()
        {
            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.timeLeft < 55)
                Projectile.tileCollide = true;
            if (Projectile.ai[1] != 1f)
                return;
            NPC target = Projectile.Center.ClosestNPCAt(999f, false);
            if (target == null)
                return;
            Projectile.velocity += Vector2.Normalize(target.Center - Projectile.Center) * 2f;
            if (Projectile.velocity.Length() > 10f)
                Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 10f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Electrified, 120);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.Electrified, 120);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D sprite = ModContent.Request<Texture2D>(Projectile.ai[0] == 0f ? Texture : Texture + "2").Value;
            Main.EntitySpriteDraw(sprite, Projectile.Center - Main.screenPosition, new Rectangle(0, FrameHeight * Projectile.frame, FrameWidth, FrameHeight),
                Color.White, Projectile.rotation, new Vector2(FrameWidth / 2, FrameHeight / 2), 1f, SpriteEffects.None, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item93 with { Volume = SoundID.Item93.Volume * 0.25f }, Projectile.position);
            for (int i = 0; i < 5; i++)
                Dust.NewDustDirect(Projectile.Center, 1, 1, 132, Projectile.velocity.X, Projectile.velocity.Y, 0, default, 0.5f).noGravity = true;
        }
    }

    /// <summary>Onde de choc circulaire (rayon 100) : Electrified 3 s.</summary>
    public class ShockGrenadeExplosion : ModProjectile
    {
        private const float Radius = 100f;

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 320;
            Projectile.height = 320;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 10;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
        }

        public override void AI()
        {
            if (Projectile.timeLeft < 8)
                return;
            for (int i = 0; i < 50; i++)
            {
                Vector2 velocity = Vector2.Normalize(new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1)) + new Vector2(0.001f)) * 8f;
                Dust.NewDustDirect(Projectile.Center, 1, 1, Main.rand.NextBool() ? 264 : 132, velocity.X, velocity.Y, 0, default, 0.75f).noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Electrified, 180);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.Electrified, 180);

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, Radius, targetHitbox);
    }
}
