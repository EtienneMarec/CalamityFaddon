// Adapté de Calamity Mod 2.0.1.005 et 2.2.2 (GalileoGladiusProj), © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
using System;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Projectiles.Melee
{
    /// <summary>
    /// Wiki : Galileo Gladius.txt — estoc du Galileo Gladius en Full Fandom (Common/Legacy/FullFandomLegacyWeapons) : animation
    /// de l'arme actuelle de Calamity (épée tenue, GalileoGladiusProj de 2.2.2), lame lumineuse et portée réduites de plus de
    /// moitié (retour de test du 28/10), avec le fonctionnement de 2.0.1.005 : un estoc qui touche fait pleuvoir une planète
    /// et inflige Moon Burnt 5 s ; un estoc dans le vide tire un croissant de lune à tête chercheuse (un tiers des dégâts).
    /// Pas de Stratus Starburst.
    /// </summary>
    public class GalileoGladiusHeldProj : BaseSwordHoldoutProjectile
    {
        /// <summary>Longueur de la ligne qui touche (192 chez Calamity) et de la lame lumineuse (1,55 chez Calamity).</summary>
        private const float CollisionLength = 84f;
        private const float GlowLength = 0.65f;

        // Objet modèle du ModItem, comme Calamity : ContentSamples n'est pas encore rempli quand les projectiles sont préparés
        public override Item BaseItem => ModContent.GetModItem(ModContent.ItemType<GalileoGladius>()).Item;
        public override string Texture => "CalamityMod/Items/Weapons/Melee/GalileoGladius";
        public override int swingWidth => 200;
        public override int AfterImageLength => 0;
        // Arme plus avancée dans la main que chez Calamity (15 → 45) : la garde restait loin du bras
        public override int OffsetDistance { get; set; } = 22;
        public override bool drawSwordTrail => false;
        public override bool AlternateSwings => false;
        public override bool useAttackSpeed => true;
        public override int swingTime { get; set; } = 8;
        public override SoundStyle? UseSound => SoundID.Item71 with { Volume = 0.5f };

        public override void Defaults() => Projectile.extraUpdates = 3;

        public override void Spawn() => angle = angle.RotatedByRandom(0.25f);

        public override float SwingFunction() => 0f; // estoc, pas de balayage

        public override void AdditionalAI()
        {
            OffsetDistance = (int)MathHelper.Lerp(22, 38, SwingCompletion);
            Vector2 forward = angle.RotatedBy(MathHelper.Pi);
            if (Projectile.FinalExtraUpdate())
            {
                for (float i = -1; i <= 1; i += 2f)
                {
                    Vector2 velocity = -forward.RotatedBy(i * -0.3f) * 6f;
                    Vector2 position = Projectile.Center + new Vector2(MathHelper.Lerp(20, 45 * Projectile.scale, SwingCompletion), i * 10).RotatedBy(forward.ToRotation());
                    GeneralParticleHandler.SpawnParticle(new CustomSpark(position, velocity, "CalamityMod/Particles/BloomCircle", false, 3, 0.25f,
                        Color.LightSkyBlue, new Vector2(0.3f, 2f), noShrink: true));
                }
            }
            Lighting.AddLight(Main.player[Projectile.owner].Center, 0.96f, 0.91f, 1f);
            Main.player[Projectile.owner].heldProj = Projectile.whoAmI;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D glow = ModContent.Request<Texture2D>("CalamityMod/Particles/GlowBlade").Value;
            Vector2 forward = angle.RotatedBy(MathHelper.Pi);
            {
                // Lot additif le temps d'un dessin (SpriteBatchScope de Calamity est interne)
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                Main.spriteBatch.Draw(glow, Projectile.Center + forward * -10 - Main.screenPosition, null, Color.SkyBlue * 0.75f, forward.ToRotation() + MathHelper.PiOver2,
                    new Vector2(glow.Width * 0.5f, glow.Height), new Vector2(0.6f, MathHelper.Lerp(0.1f, GlowLength, SwingCompletion)) * Projectile.scale * 0.04f, SpriteEffects.None, 1);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }
            Texture2D sword = TextureAssets.Projectile[Type].Value;
            Main.spriteBatch.Draw(sword, Projectile.Center + new Vector2(0, Projectile.gfxOffY) - Main.screenPosition, null, Color.SkyBlue, Projectile.rotation,
                sword.Size() * 0.5f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 1);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D glow = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/GalileoGladiusGlow").Value;
            {
                // Lot additif le temps d'un dessin (SpriteBatchScope de Calamity est interne)
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                Main.spriteBatch.Draw(glow, Projectile.Center + new Vector2(0, Projectile.gfxOffY) - Main.screenPosition, null, Color.SkyBlue * 0.75f, Projectile.rotation,
                    glow.Size() * 0.5f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 1);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Nightwither>(), 300);
            GalileoPlanetPlayer.TryRain(Main.player[Projectile.owner], Projectile.GetSource_FromThis(), target.Center, Projectile.damage * 2);
        }

        public override void OnKill(int timeLeft)
        {
            // Coup direct : la planète seule (retour de test du 28/10) ; estoc dans le vide : le croissant
            // (clones du Shortflurry : leur croissant vient de l'effet du Shortflurry, avec ses planètes)
            if (Projectile.numHits > 0 || Projectile.owner != Main.myPlayer || Projectile.GetGlobalProjectile<Content.Shortstrike.ShortflurryRealClone>().IsClone)
                return;
            Vector2 forward = angle.RotatedBy(MathHelper.Pi);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Main.player[Projectile.owner].MountedCenter + forward * 30f, forward * 9f,
                ModContent.ProjectileType<GalileosMoon>(), (int)(Projectile.damage * 0.3333f), Projectile.knockBack, Projectile.owner);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Player player = Main.player[Projectile.owner];
            Vector2 armCenter = player.Center - new Vector2(5 * player.direction, 2);
            Vector2 line = new Vector2(CollisionLength / 2f, 0f).RotatedBy(armCenter.DirectionTo(Projectile.Center).ToRotation()) * Projectile.scale;
            if (!float.IsNaN(line.X) && !float.IsNaN(line.Y)
                && Collision.CheckAABBvLineCollision(targetHitbox.Location.ToVector2(), targetHitbox.Size(), Projectile.Center, Projectile.Center + line))
                return true;
            return base.Colliding(projHitbox, targetHitbox);
        }
    }

    /// <summary>Délai entre deux pluies de planètes (galileoCooldown de 2.0.1.005, retiré de Calamity).</summary>
    public class GalileoPlanetPlayer : ModPlayer
    {
        public int Cooldown;

        public override void PostUpdate()
        {
            if (Cooldown > 0)
                Cooldown--;
        }

        /// <summary>Fait tomber une planète sur la cible, au plus une fois toutes les 15 frames.</summary>
        public static void TryRain(Player player, Terraria.DataStructures.IEntitySource source, Vector2 target, int damage, int type = -1)
        {
            var modPlayer = player.GetModPlayer<GalileoPlanetPlayer>();
            if (player.whoAmI != Main.myPlayer || modPlayer.Cooldown > 0)
                return;
            if (type <= 0)
                type = ModContent.ProjectileType<GalileosPlanet>();
            CalamityUtils.ProjectileRain(source, target, 400f, 100f, 500f, 800f, 25f, type, damage, 15f, player.whoAmI);
            modPlayer.Cooldown = 15;
        }
    }

    /// <summary>Croissant de lune à tête chercheuse (version 2.0.1.005 : perce 12 fois, Moon Burnt 3 s comme dit la page).</summary>
    public class GalileosMoon : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/CrescentMoonProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.alpha = 100;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.penetrate = 12;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 135 * Projectile.MaxUpdates;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = Projectile.MaxUpdates * 10;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0f, 0f, 0.6f);
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 20 + Main.rand.Next(40);
                if (Main.rand.NextBool(5))
                    SoundEngine.PlaySound(SoundID.Item9, Projectile.position);
            }
            Projectile.rotation += Projectile.direction * 0.55f;
            CalamityUtils.HomeInOnNPC(Projectile, true, 250f, 10f, 25f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Nightwither>(), 180);
    }

    /// <summary>Planète qui tombe du ciel et explose (au sol ou sur un ennemi) : moitié des dégâts en zone, Moon Burnt 5 s (page).</summary>
    public class GalileosPlanet : ModProjectile
    {
        private float radius = 84f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 88;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            // Traînée de poussière bleue de part et d'autre de la planète
            Vector2 tail = Projectile.position + new Vector2(Projectile.width / 2 - 8, Projectile.height + 16);
            for (float i = -5; i <= 5; i++)
            {
                if (Main.rand.NextFloat() < 0.4f)
                    Dust.NewDustPerfect(tail + new Vector2((i < 0 ? -20 : 20) + i, -2), 180, new Vector2(i * 0.7f, -1.1f)).noGravity = true;
            }
            Dust.NewDust(Projectile.Center - new Vector2(20, 20), 40, 40, 180);
            Lighting.AddLight(Projectile.Center, 0.3f, 0.3f, 0.7f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            // Explosion : zone de 100 px, moitié des dégâts
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 32;
            Projectile.position -= Projectile.Size / 2f;
            radius = 100f;
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.damage /= 2;
            Projectile.Damage();

            for (int i = 0; i < 72; i++)
            {
                var dust = Dust.NewDustDirect(Projectile.Center - new Vector2(30, 30), 60, 60, 187, Main.rand.Next(-5, 5), Main.rand.Next(-5, 5), 0, new Color(0, 167, 255), 5f);
                dust.noGravity = true;
                dust.shader = GameShaders.Armor.GetSecondaryShader(47, Main.LocalPlayer);
            }
            SoundEngine.PlaySound(SoundID.NPCDeath43, Projectile.position);
            for (int i = 0; i < 155; i++)
                Dust.NewDustDirect(Projectile.position, 94, 94, 180, Main.rand.Next(-5, 5), Main.rand.Next(-5, 5), 0, Color.White, Main.rand.NextFloat(1.2f, 2.2f)).noGravity = true;
            // Deux anneaux de poussière
            for (int ring = 1; ring <= 3; ring++)
            {
                const int Points = 36;
                for (int i = 0; i < Points; i++)
                {
                    Vector2 offset = (Vector2.Normalize(Projectile.velocity) * new Vector2(Projectile.width / 2f, Projectile.height) * 0.375f)
                        .RotatedBy((i - (Points / 2 - 1)) * MathHelper.TwoPi / Points);
                    var dust = Dust.NewDustDirect(Projectile.Center + offset * 2f, 0, 0, 180, 0f, 0f, 0, Color.White, 1f);
                    dust.noGravity = true;
                    dust.velocity = Vector2.Normalize(offset) * ring * 2f;
                }
            }
            if (Main.netMode != NetmodeID.Server)
            {
                Vector2 source = Projectile.Center - new Vector2(24f, 24f);
                foreach (var direction in new[] { new Vector2(1f, 1f), new Vector2(-1f, 1f), new Vector2(1f, -1f), new Vector2(-1f, -1f) })
                {
                    var gore = Gore.NewGoreDirect(Projectile.GetSource_Death(), source, default, Main.rand.Next(61, 64));
                    gore.velocity = gore.velocity * 0.66f + direction;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Nightwither>(), 300);

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, radius, targetHitbox);
    }
}
