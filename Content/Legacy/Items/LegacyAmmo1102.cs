// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using CalamityFandom.Content.Legacy.Buffs.StatDebuffs;
using CalamityFandom.Content.Legacy.Projectiles.Ranged;
using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

// Munitions retirées de Calamity après 2.0.1.005 et documentées sur le wiki (02/11). Valeurs des infobox (dégâts, pile de
// 9999, prix, rareté) ; recettes du wiki (WikiRecipes). L'Explosive Shotgun Shell n'est pas portée : elle ne servait qu'à
// l'ancienne Hydra, qui n'utilise plus de munitions spéciales.

namespace CalamityFandom.Content.Legacy.Items.Ammo
{
    /// <summary>Wiki : Arctic Arrow.txt — flèche qui gèle (Glacial State 0,5 s).</summary>
    public class ArcticArrow : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 99;

        public override void SetDefaults()
        {
            Item.damage = 18;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 22;
            Item.height = 36;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.knockBack = 1.5f;
            Item.value = Item.sellPrice(copper: 12);
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ModContent.ProjectileType<ArcticArrowProj>();
            Item.shootSpeed = 13f;
            Item.ammo = AmmoID.Arrow;
        }
    }

    /// <summary>Wiki : Enhanced Nano Round.txt — balle qui rend Confused 5 s ; un ennemi tué libère 2 nanomachines à tête chercheuse.</summary>
    public class EnhancedNanoRound : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 99;

        public override void SetDefaults()
        {
            Item.damage = 12;
            Item.DamageType = DamageClass.Ranged;
            Item.width = Item.height = 8;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.knockBack = 5.5f;
            Item.value = Item.sellPrice(silver: 1);
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ModContent.ProjectileType<EnhancedNanoRoundProj>();
            Item.shootSpeed = 8f;
            Item.ammo = AmmoID.Bullet;
        }
    }

    /// <summary>Wiki : Superball Bullet.txt — balle lente qui accélère à chaque rebond ou ennemi traversé (5 fois au plus).</summary>
    public class SuperballBullet : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 99;

        public override void SetDefaults()
        {
            Item.damage = 6;
            Item.DamageType = DamageClass.Ranged;
            Item.width = Item.height = 8;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.knockBack = 1.5f;
            Item.value = Item.sellPrice(copper: 50);
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ModContent.ProjectileType<SuperballBulletProj>();
            Item.shootSpeed = 1f;
            Item.ammo = AmmoID.Bullet;
        }
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.Ranged
{
    public class ArcticArrowProj : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Legacy/Items/Ammo/ArcticArrow";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.arrow = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.coldDamage = true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            if (Main.rand.NextBool(5))
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.IceTorch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            return true;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
            for (int i = 0; i < 10; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(10f), 20, 20, DustID.IceTorch, 0f, 0f, 100, default, 2f);
                dust.velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    dust.scale = 0.5f;
                    dust.fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<GlacialState>(), 30);
    }

    public class EnhancedNanoRoundProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            AIType = ProjectileID.Bullet;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 3;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0f, 0.25f, 0.25f);
            if (++Projectile.localAI[0] > 4f && Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, 1, 1, DustID.Electric, 0f, 0f, 0, default, 0.5f);
                dust.velocity *= 0f;
                dust.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesFromEdge(Projectile, 0, lightColor);
            return false;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(1f, 1f, 1f, 0f);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Confused, 300);
            if (target.life > 0 || Projectile.owner != Main.myPlayer)
                return;
            for (int i = 0; i < 2; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, CalamityUtils.RandomVelocity(100f, 70f, 100f),
                    ModContent.ProjectileType<Nanomachine>(), (int)(Projectile.damage * 0.3f), 0f, Projectile.owner);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item93, Projectile.position);
            for (int i = 0; i < Main.rand.Next(5, 10); i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center - Projectile.velocity / 2f, 0, 0, DustID.Electric, 0f, 0f, 100, default, 2f);
                dust.velocity *= 2f;
                dust.noGravity = true;
            }
        }
    }

    /// <summary>Nanomachine de l'Enhanced Nano Round : cherche les ennemis (800 px), 30 % des dégâts ; deux dessins au hasard.</summary>
    public class Nanomachine : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 3;
            ProjectileID.Sets.TrailCacheLength[Type] = 3;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            if (++Projectile.frameCounter > 4)
            {
                Projectile.frame = (Projectile.frame + 1) % 3;
                Projectile.frameCounter = 0;
            }
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi) + MathHelper.PiOver2 * Projectile.direction;
            Dust dust = Dust.NewDustPerfect(Projectile.Center - Projectile.velocity / 10f, DustID.Electric, -Projectile.velocity * Main.rand.NextFloat(0.5f, 0.8f), 0, default, 0.8f);
            dust.noGravity = true;
            if (Projectile.localAI[0] == 0f)
                Projectile.localAI[0] = Projectile.velocity.Length();
            CalamityUtils.HomeInOnNPC(Projectile, false, 800f, Projectile.localAI[0], 12f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.localAI[1] == 0f)
                Projectile.localAI[1] = Main.rand.Next(1, 3);
            string path = "CalamityFandom/Content/Legacy/Projectiles/Ranged/Nanomachine" + (Projectile.localAI[1] == 2f ? "2" : "");
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1, ModContent.Request<Texture2D>(path).Value);
            return false;
        }
    }

    public class SuperballBulletProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            AIType = ProjectileID.Bullet;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 5;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            if (++Projectile.localAI[0] > 4f && Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, 1, 1, DustID.GemTopaz, 0f, 0f, 0, default, 0.5f);
                dust.velocity *= 0f;
                dust.noGravity = true;
            }
        }

        // « Increases each time they bounce on a block or pierce an enemy, to a maximum of 5 times »
        private void Accelerate()
        {
            if (Projectile.extraUpdates < 5)
                Projectile.extraUpdates++;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => Accelerate();

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (--Projectile.penetrate <= 0)
                return true;
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            Accelerate();
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesFromEdge(Projectile, 0, lightColor);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int k = 0; k < 5; k++)
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.GemTopaz, Projectile.oldVelocity.X * 0.05f, Projectile.oldVelocity.Y * 0.05f);
        }
    }
}
