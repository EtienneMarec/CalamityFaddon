using System;
using CalamityFandom.Content.Projectiles.Summon;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

// Les bâtons écureuils du wiki (pages Squirrel <Rang> Staff) : une tourelle par rang, qui améliore le Squirrel Squire Staff
// de Calamity. Toutes tombent doucement jusqu'au sol si on les pose en l'air et peuvent tirer en tombant.

namespace CalamityFandom.Content.Items.Weapons.Summon
{
    public abstract class SquirrelStaff : ModItem
    {
        protected abstract int Damage { get; }
        protected abstract float Knockback { get; }
        protected abstract int Rarity { get; }
        protected abstract int Sentry { get; }

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = Damage;
            Item.DamageType = DamageClass.Summon;
            Item.knockBack = Knockback;
            Item.mana = 12;
            Item.useTime = Item.useAnimation = 34;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.sentry = true;
            Item.autoReuse = true;
            Item.rare = Rarity;
            Item.value = Item.sellPrice(silver: 10);
            Item.UseSound = SoundID.Item44;
            Item.shoot = Sentry;
            Item.shootSpeed = 1f;
            Item.width = Item.height = 40;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Posée au curseur (à portée), sans entrer dans les blocs
            Vector2 spawn = Main.MouseWorld;
            if (Vector2.Distance(spawn, player.Center) > 600f)
                spawn = player.Center + (spawn - player.Center).SafeNormalize(Vector2.UnitX) * 600f;
            int guard = 0;
            while (Collision.SolidCollision(spawn - new Vector2(16f), 32, 32) && guard++ < 100)
                spawn = Vector2.Lerp(spawn, player.Center, 0.05f);
            int p = Projectile.NewProjectile(source, spawn, Vector2.Zero, type, damage, knockback, player.whoAmI);
            if (Main.projectile.IndexInRange(p))
                Main.projectile[p].originalDamage = Item.damage;
            player.UpdateMaxTurrets();
            return false;
        }
    }

    /// <summary>Wiki : Squirrel Knight Staff.txt — un écureuil chevalier qui lance des couteaux (percent une fois).</summary>
    public class SquirrelKnightStaff : SquirrelStaff
    {
        protected override int Damage => 17;
        protected override float Knockback => 1f;
        protected override int Rarity => ItemRarityID.Green;
        protected override int Sentry => ModContent.ProjectileType<SquirrelKnightSentry>();
    }

    /// <summary>Wiki : Squirrel Baron Staff.txt — lance des Prism Shards qui percent une fois puis éclatent en 2 ou 3 éclats.</summary>
    public class SquirrelBaronStaff : SquirrelStaff
    {
        protected override int Damage => 25;
        protected override float Knockback => 1f;
        protected override int Rarity => ItemRarityID.White;
        protected override int Sentry => ModContent.ProjectileType<SquirrelBaronSentry>();
    }

    /// <summary>Wiki : Squirrel Viscount Staff.txt — lance des pieux qui percent sans fin.</summary>
    public class SquirrelViscountStaff : SquirrelStaff
    {
        protected override int Damage => 42;
        protected override float Knockback => 1f;
        protected override int Rarity => ItemRarityID.LightPurple;
        protected override int Sentry => ModContent.ProjectileType<SquirrelViscountSentry>();
    }

    /// <summary>Wiki : Squirrel Count Staff.txt — boules de feu à tête chercheuse qui explosent.</summary>
    public class SquirrelCountStaff : SquirrelStaff
    {
        protected override int Damage => 68;
        protected override float Knockback => 3f;
        protected override int Rarity => ItemRarityID.Cyan;
        protected override int Sentry => ModContent.ProjectileType<SquirrelCountSentry>();
    }

    /// <summary>Wiki : Squirrel Duke Staff.txt — flots continus de bulles à tête chercheuse.</summary>
    public class SquirrelDukeStaff : SquirrelStaff
    {
        protected override int Damage => 168;
        protected override float Knockback => 1f;
        protected override int Rarity => ModContent.RarityType<PureGreen>();
        protected override int Sentry => ModContent.ProjectileType<SquirrelDukeSentry>();
    }

    /// <summary>Wiki : Squirrel Prince Staff.txt — boules de feu qui explosent fort et se séparent en boules chercheuses.</summary>
    public class SquirrelPrinceStaff : SquirrelStaff
    {
        protected override int Damage => 325;
        protected override float Knockback => 4f;
        protected override int Rarity => ModContent.RarityType<BurnishedAuric>();
        protected override int Sentry => ModContent.ProjectileType<SquirrelPrinceSentry>();
    }

    /// <summary>Wiki : Squirrel King Staff.txt — fait tomber des rayons de lumière sacrée sur les ennemis.</summary>
    public class SquirrelKingStaff : SquirrelStaff
    {
        protected override int Damage => 604;
        protected override float Knockback => 10f;
        protected override int Rarity => ModContent.RarityType<HotPink>();
        protected override int Sentry => ModContent.ProjectileType<SquirrelKingSentry>();
    }
}

namespace CalamityFandom.Content.Projectiles.Summon
{
    /// <summary>
    /// Tourelle écureuil : tombe doucement jusqu'au sol, se tourne vers sa cible (960 px, cible du joueur prioritaire) et
    /// tire toutes les <see cref="FireRate"/> images. [choix] Cadences : 40 images (Duke : 8, « constamment »).
    /// </summary>
    public abstract class SquirrelSentry : ModProjectile
    {
        protected virtual int FireRate => 40;
        protected virtual float ShotSpeed => 12f;

        protected abstract void Fire(NPC target, Vector2 from);

        public override void SetStaticDefaults() => ProjectileID.Sets.MinionTargettingFeature[Type] = true;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.timeLeft = Projectile.SentryLifeTime;
            Projectile.penetrate = -1;
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.sentry = true;
            Projectile.netImportant = true;
        }

        public override void AI()
        {
            // Chute lente (page)
            Projectile.velocity.X = 0f;
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.1f, 2.5f);
            Player owner = Main.player[Projectile.owner];
            NPC target = Projectile.Center.MinionHoming(960f, owner, false);
            if (target == null)
                return;
            Projectile.spriteDirection = Math.Sign(target.Center.X - Projectile.Center.X);
            if (++Projectile.ai[0] < FireRate)
                return;
            Projectile.ai[0] = 0f;
            if (Main.myPlayer == Projectile.owner)
                Fire(target, Projectile.Center + new Vector2(Projectile.spriteDirection * Projectile.width / 2f, -6f));
            Projectile.netUpdate = true;
        }

        protected Vector2 Aim(Vector2 from, NPC target, float speed) => CalamityUtils.CalculatePredictiveAimToTarget(from, target, speed);

        protected int Shoot<T>(Vector2 from, Vector2 velocity, float damageMult = 1f) where T : ModProjectile =>
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), from, velocity, ModContent.ProjectileType<T>(), (int)(Projectile.damage * damageMult), Projectile.knockBack, Projectile.owner);

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }

        public override bool? CanDamage() => false;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 pos = Projectile.Bottom - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
            SpriteEffects flip = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.EntitySpriteDraw(texture, pos, null, Projectile.GetAlpha(lightColor), 0f, new Vector2(texture.Width / 2f, texture.Height), Projectile.scale, flip);
            return false;
        }
    }

    public class SquirrelKnightSentry : SquirrelSentry
    {
        protected override void Fire(NPC target, Vector2 from)
        {
            SoundEngine.PlaySound(SoundID.Item1 with { Volume = 0.5f, Pitch = 0.6f }, from);
            Shoot<SquirrelKnife>(from, Aim(from, target, ShotSpeed));
        }
    }

    public class SquirrelBaronSentry : SquirrelSentry
    {
        protected override void Fire(NPC target, Vector2 from)
        {
            SoundEngine.PlaySound(SoundID.Item1 with { Volume = 0.5f, Pitch = 0.4f }, from);
            Shoot<SquirrelPrismShard>(from, Aim(from, target, ShotSpeed));
        }
    }

    public class SquirrelViscountSentry : SquirrelSentry
    {
        protected override void Fire(NPC target, Vector2 from)
        {
            SoundEngine.PlaySound(SoundID.Item1 with { Volume = 0.5f, Pitch = 0.2f }, from);
            Shoot<SquirrelStake>(from, Aim(from, target, 16f));
        }
    }

    public class SquirrelCountSentry : SquirrelSentry
    {
        protected override void Fire(NPC target, Vector2 from)
        {
            SoundEngine.PlaySound(SoundID.Item20 with { Volume = 0.5f }, from);
            Shoot<SquirrelFireball>(from, Aim(from, target, 9f));
        }
    }

    public class SquirrelDukeSentry : SquirrelSentry
    {
        protected override int FireRate => 8;

        protected override void Fire(NPC target, Vector2 from)
        {
            if (Main.rand.NextBool(3))
                SoundEngine.PlaySound(SoundID.Item85 with { Volume = 0.4f }, from);
            Shoot<SquirrelBubble>(from, Aim(from, target, 8f).RotatedByRandom(0.15f), 0.5f);
        }
    }

    public class SquirrelPrinceSentry : SquirrelSentry
    {
        protected override void Fire(NPC target, Vector2 from)
        {
            SoundEngine.PlaySound(SoundID.Item73, from);
            Shoot<SquirrelPrinceFireball>(from, Aim(from, target, 11f));
        }
    }

    public class SquirrelKingSentry : SquirrelSentry
    {
        protected override int FireRate => 50;

        protected override void Fire(NPC target, Vector2 from)
        {
            SoundEngine.PlaySound(SoundID.Item122 with { Volume = 0.6f }, target.Center);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<SquirrelHolyBeam>(),
                Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }

    /// <summary>Couteau du Squirrel Knight (texture du couteau de lancer vanilla) : perce une fois.</summary>
    public class SquirrelKnife : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.ThrowingKnife;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 180;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (++Projectile.ai[0] > 20f)
                Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.2f, 16f);
        }
    }

    /// <summary>Prism Shard du Squirrel Baron : perce une fois, puis éclate en 2 ou 3 éclats qui ne percent pas.</summary>
    public class SquirrelPrismShard : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Placeables/SunkenSea/PrismShard";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 180;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.rotation += 0.3f * Math.Sign(Projectile.velocity.X);
            if (Projectile.ai[0] == 0f && Main.rand.NextBool(3))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GemSapphire).noGravity = true;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27 with { Volume = 0.5f }, Projectile.Center);
            // Les éclats (ai[0] = 1) ne se séparent plus
            if (Projectile.ai[0] != 0f || Projectile.owner != Main.myPlayer)
                return;
            int count = Main.rand.Next(2, 4);
            for (int i = 0; i < count; i++)
            {
                int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2CircularEdge(7f, 7f), Type,
                    Projectile.damage / 2, Projectile.knockBack, Projectile.owner, 1f);
                if (Main.projectile.IndexInRange(p))
                {
                    Main.projectile[p].penetrate = 1;
                    Main.projectile[p].scale = 0.6f;
                    Main.projectile[p].timeLeft = 40;
                }
            }
        }
    }

    /// <summary>Pieu du Squirrel Viscount (texture du pieu vanilla) : perce sans fin.</summary>
    public class SquirrelStake : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.Stake;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
    }

    /// <summary>Boule de feu du Squirrel Count : cherche les ennemis, explose (On Fire! 3 s).</summary>
    public class SquirrelFireball : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.BallofFire;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 240;
        }

        public override void AI()
        {
            Projectile.rotation += 0.2f;
            if (++Projectile.ai[1] > 10f)
                CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 500f, 10f, 20f);
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1.5f);
            d.noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.OnFire, 180);

        public override void OnKill(int timeLeft) => Explode(Projectile, 64, DustID.Torch);

        public static void Explode(Projectile projectile, int size, int dust)
        {
            SoundEngine.PlaySound(SoundID.Item14 with { Volume = 0.6f }, projectile.Center);
            projectile.ExpandHitboxBy(size);
            projectile.maxPenetrate = projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Damage();
            for (int i = 0; i < size / 3; i++)
            {
                Dust d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, dust, 0f, 0f, 100, default, 2f);
                d.noGravity = true;
                d.velocity *= 3f;
            }
        }
    }

    /// <summary>Bulle du Squirrel Duke : cherche les ennemis.</summary>
    public class SquirrelBubble : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.Bubble;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 200;
            Projectile.alpha = 80;
        }

        public override void AI() => CalamityUtils.HomeInOnNPC(Projectile, true, 500f, 9f, 25f);

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item54 with { Volume = 0.4f }, Projectile.Center);
            for (int i = 0; i < 6; i++)
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.BubbleBurst_Blue).velocity *= 1.5f;
        }
    }

    /// <summary>Boule de feu du Squirrel Prince : grosse explosion, puis trois boules de feu chercheuses (moitié des dégâts).</summary>
    public class SquirrelPrinceFireball : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.InfernoFriendlyBolt;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 240;
            Projectile.scale = 1.3f;
        }

        public override void AI()
        {
            Projectile.rotation += 0.25f;
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SolarFlare, 0f, 0f, 100, default, 1.6f);
            d.noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);

        public override void OnKill(int timeLeft)
        {
            int damage = Projectile.damage;
            SquirrelFireball.Explode(Projectile, 160, DustID.SolarFlare);
            if (Projectile.owner != Main.myPlayer)
                return;
            for (int i = 0; i < 3; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2CircularEdge(8f, 8f),
                    ModContent.ProjectileType<SquirrelFireball>(), damage / 2, Projectile.knockBack, Projectile.owner);
        }
    }

    /// <summary>Rayon de lumière sacrée du Squirrel King : colonne qui tombe sur l'ennemi (frappe une fois chaque ennemi).</summary>
    public class SquirrelHolyBeam : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private const int Height = 480;

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = Height;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 24;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            // La colonne descend du haut de l'écran jusqu'au bas de la cible
            Projectile.position = new Vector2(Projectile.Center.X - Projectile.width / 2f, Projectile.Center.Y - Height + 24f);
        }

        public override bool? CanDamage() => Projectile.timeLeft > 12 ? null : false;

        public override void AI()
        {
            Lighting.AddLight(Projectile.Bottom, 1f, 0.95f, 0.6f);
            if (Main.dedServ)
                return;
            for (int i = 0; i < 6; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GoldFlame, 0f, 6f, 100, default, 1.8f);
                d.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
    }
}
