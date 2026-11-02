using CalamityFandom.Content.Buffs;
using CalamityMod;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

// Munitions en Flahium du wiki (02/11), fabriquées à la Draedon's Forge (recettes du wiki : 4999 par Flahium Bar).

namespace CalamityFandom.Content.Items.Ammo
{
    /// <summary>
    /// Wiki : Flah Round.txt — balle lourde qui explose et rebondit sur un ennemi ou un obstacle (9 fois), petites explosions
    /// à 33 % des dégâts, puis grande explosion à 150 % (Flahmes 3 s). [choix] Rayons : 3 et 8 blocs.
    /// </summary>
    public class FlahRound : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 99;

        public override void SetDefaults()
        {
            Item.damage = 40;
            Item.DamageType = DamageClass.Ranged;
            Item.width = Item.height = 8;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.knockBack = 7f;
            Item.value = Item.sellPrice(silver: 5);
            Item.rare = ModContent.RarityType<HotPink>();
            Item.shoot = ModContent.ProjectileType<Projectiles.Ranged.FlahRoundProj>();
            Item.shootSpeed = 6f;
            Item.ammo = AmmoID.Bullet;
        }
    }

    /// <summary>
    /// Wiki : Flarrow.txt — flèche qui cherche lentement les ennemis pendant 2 s puis fonce sur eux à grande vitesse, à travers
    /// les blocs. [choix] Vitesse de la charge : 30 ; sans cible au bout de 2 s, elle continue tout droit.
    /// </summary>
    public class Flarrow : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 99;

        public override void SetDefaults()
        {
            Item.damage = 16;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 18;
            Item.height = 38;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.knockBack = 2f;
            Item.value = Item.sellPrice(silver: 5);
            Item.rare = ModContent.RarityType<HotPink>();
            Item.shoot = ModContent.ProjectileType<Projectiles.Ranged.FlarrowProj>();
            Item.shootSpeed = 6f;
            Item.ammo = AmmoID.Arrow;
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Ranged
{
    public class FlahRoundProj : ModProjectile
    {
        private const int Bounces = 9;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Lighting.AddLight(Projectile.Center, 0.6f, 0.2f, 0.6f);
        }

        /// <summary>Un rebond : petite explosion ; au 9e, la grande explosion finale.</summary>
        private void Bounce()
        {
            Projectile.ai[0]++;
            bool last = Projectile.ai[0] >= Bounces;
            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FlahBlast>(),
                    (int)(Projectile.damage * (last ? 1.5f : 0.33f)), Projectile.knockBack, Projectile.owner, last ? 1f : 0f);
            if (last)
                Projectile.Kill();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.velocity = -Projectile.velocity.RotatedByRandom(0.6f);
            Bounce();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            Bounce();
            return false;
        }
    }

    /// <summary>Explosion de Flahium (ai[0] = 1 : la grande, avec Flahmes 3 s).</summary>
    public class FlahBlast : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private bool Big => Projectile.ai[0] == 1f;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 5;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void OnSpawn(Terraria.DataStructures.IEntitySource source)
        {
            if (Big)
                Projectile.Resize(128, 128);
            SoundEngine.PlaySound(Big ? SoundID.Item14 : SoundID.Item10, Projectile.Center);
            for (int i = 0; i < (Big ? 40 : 10); i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.PinkTorch, 0f, 0f, 100, default, Big ? 2.2f : 1.4f);
                dust.velocity *= Big ? 4f : 2f;
                dust.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Big)
                target.AddBuff(ModContent.BuffType<Flahmes>(), 180);
        }
    }

    /// <summary>Flarrow : cherche lentement 2 s, puis fonce à 30 vers sa cible, à travers les blocs.</summary>
    public class FlarrowProj : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Ammo/Flarrow";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.arrow = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 400;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Lighting.AddLight(Projectile.Center, 0.5f, 0.15f, 0.5f);
            if (Main.rand.NextBool(3))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.PinkTorch, 0f, 0f, 100).noGravity = true;
            Projectile.ai[0]++;
            NPC target = Projectile.FindTargetWithinRange(900f, checkCanHit: false);
            if (Projectile.ai[0] < 120f)
            {
                // Recherche lente
                if (target != null)
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 6f, 0.04f);
            }
            else if (Projectile.ai[0] == 120f)
            {
                Vector2 dir = target != null ? (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) : Projectile.velocity.SafeNormalize(Vector2.UnitX);
                Projectile.velocity = dir * 30f;
                SoundEngine.PlaySound(SoundID.Item74 with { Volume = 0.5f }, Projectile.Center);
            }
        }
    }
}
