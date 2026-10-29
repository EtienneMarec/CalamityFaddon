// Adapté de Calamity Mod (épées courtes retirées en 2022), © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using CalamityFandom.Content.Legacy.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.Weapons.Melee
{
    /// <summary>Wiki : Caliburn.txt — l'ancienne Excalibur Shortsword de Calamity, renommée par le Fandom ; estoc qui scintille.</summary>
    public class Caliburn : LegacyShortsword
    {
        protected override int Damage => 70;
        protected override int UseTime => 10;
        protected override float Knockback => 5.25f;
        protected override int Rarity => ItemRarityID.Pink;
        protected override int SellSilver => 720;
        protected override int StabProjectile => ModContent.ProjectileType<CaliburnProj>();
    }

    /// <summary>
    /// Wiki : True Caliburn.txt — l'ancienne True Excalibur Shortsword : chaque estoc tire un rayon horizontal (gauche ou
    /// droite) qui perce deux fois.
    /// </summary>
    public class TrueCaliburn : LegacyShortsword
    {
        protected override int Damage => 130;
        protected override int UseTime => 12;
        protected override float Knockback => 5.75f;
        protected override int Rarity => ItemRarityID.Yellow;
        protected override int SellSilver => 1600;
        protected override int StabProjectile => ModContent.ProjectileType<TrueCaliburnProj>();
    }

    /// <summary>Wiki : True Night_s Stabber.txt — chaque estoc tire un rayon horizontal de la nuit qui perce deux fois.</summary>
    public class TrueNightsStabber : LegacyShortsword
    {
        protected override int Damage => 310;
        protected override int UseTime => 16;
        protected override float Knockback => 6.25f;
        protected override int Rarity => ItemRarityID.Yellow;
        protected override int SellSilver => 1600;
        protected override int StabProjectile => ModContent.ProjectileType<TrueNightsStabberProj>();
    }

    /// <summary>Wiki : Terra Shiv.txt — chaque estoc tire une lame verte horizontale qui perce trois fois.</summary>
    public class TerraShiv : LegacyShortsword
    {
        protected override int Damage => 140;
        protected override int UseTime => 13;
        protected override float Knockback => 6.5f;
        protected override int Rarity => ItemRarityID.Yellow;
        protected override int SellSilver => 1600;
        protected override int StabProjectile => ModContent.ProjectileType<TerraShivProj>();
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.Melee
{
    using global::CalamityFandom.Content.Legacy.Items.Weapons.Melee;

    public class CaliburnProj : LegacyShortswordProj
    {
        protected override int ItemType => ModContent.ItemType<Caliburn>();
        protected override int DustType => DustID.Enchanted_Gold;
    }

    /// <summary>Estoc qui tire un rayon horizontal à chaque coup (vitesse de l'ancien shootSpeed, dans la direction du joueur).</summary>
    public abstract class LegacyBeamShortswordProj : LegacyShortswordProj
    {
        protected abstract int BeamType { get; }
        protected abstract float BeamSpeed { get; }

        public override System.Action<Projectile> EffectBeforePullback => proj =>
        {
            Player owner = Main.player[Projectile.owner];
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), owner.MountedCenter, new Vector2(BeamSpeed * owner.direction, 0f), BeamType,
                Projectile.damage, Projectile.knockBack, Projectile.owner);
        };
    }

    public class TrueCaliburnProj : LegacyBeamShortswordProj
    {
        protected override int ItemType => ModContent.ItemType<TrueCaliburn>();
        protected override int DustType => DustID.Enchanted_Gold;
        protected override int BeamType => ModContent.ProjectileType<ShortBeam>();
        protected override float BeamSpeed => 16f;
    }

    public class TrueNightsStabberProj : LegacyBeamShortswordProj
    {
        protected override int ItemType => ModContent.ItemType<TrueNightsStabber>();
        protected override int DustType => DustID.Shadowflame;
        protected override int BeamType => ModContent.ProjectileType<ShortNightBeam>();
        protected override float BeamSpeed => 25f;
    }

    public class TerraShivProj : LegacyBeamShortswordProj
    {
        protected override int ItemType => ModContent.ItemType<TerraShiv>();
        protected override int DustType => DustID.TerraBlade;
        protected override int BeamType => ModContent.ProjectileType<ShortTerraBeam>();
        protected override float BeamSpeed => 12f;
    }

    /// <summary>
    /// Rayon d'épée courte (IA des rayons de lame vanilla : Light Beam, Terra Beam), dessiné après quelques frames, avec une
    /// lumière et une poussière propres ; 7 frames entre deux coups sur le même ennemi (ancien « immune = 7 »).
    /// </summary>
    public abstract class LegacyShortBeam : ModProjectile
    {
        protected abstract int Pierce { get; }
        protected abstract int Lifetime { get; }
        protected abstract int BeamAI { get; }
        protected abstract int DustType { get; }
        protected abstract Vector3 Light { get; }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = ProjAIStyleID.Beam;
            AIType = BeamAI;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = Pierce;
            Projectile.timeLeft = Lifetime;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 7;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Light * ((255 - Projectile.alpha) / 255f));
            if (Main.rand.NextBool(5))
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustType, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > Lifetime - 5)
                return false;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation,
                texture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustType, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
        }
    }

    /// <summary>Rayon de la True Caliburn (ancienne True Excalibur Shortsword) : perce deux fois, s'arrête sur le troisième ennemi.</summary>
    public class ShortBeam : LegacyShortBeam
    {
        protected override int Pierce => 3;
        protected override int Lifetime => 60;
        protected override int BeamAI => ProjectileID.LightBeam;
        protected override int DustType => 73;
        protected override Vector3 Light => new(0.2f, 0f, 0.2f);
    }

    /// <summary>Rayon du True Night's Stabber : perce deux fois.</summary>
    public class ShortNightBeam : LegacyShortBeam
    {
        protected override int Pierce => 3;
        protected override int Lifetime => 60;
        protected override int BeamAI => ProjectileID.TerraBeam;
        protected override int DustType => 21;
        protected override Vector3 Light => new(0f, 0.5f, 0.1f);
    }

    /// <summary>Lame de la Terra Shiv : perce trois fois, gerbe de poussière verte à la fin.</summary>
    public class ShortTerraBeam : LegacyShortBeam
    {
        protected override int Pierce => 4;
        protected override int Lifetime => 120;
        protected override int BeamAI => ProjectileID.TerraBeam;
        protected override int DustType => 107;
        protected override Vector3 Light => new(0f, 0.75f, 0f);

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int i = 4; i < 31; i++)
            {
                Vector2 back = Projectile.oldVelocity * (30f / i);
                var dust = Dust.NewDustDirect(Projectile.oldPosition - back, 8, 8, 107, Projectile.oldVelocity.X, Projectile.oldVelocity.Y, 100, default, 1.8f);
                dust.noGravity = true;
                dust.velocity *= 0.5f;
                dust = Dust.NewDustDirect(Projectile.oldPosition - back, 8, 8, 107, Projectile.oldVelocity.X, Projectile.oldVelocity.Y, 100, default, 1.4f);
                dust.velocity *= 0.05f;
            }
        }
    }
}
