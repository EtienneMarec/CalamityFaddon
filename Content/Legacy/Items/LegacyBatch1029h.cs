// Adapté de Calamity Mod (objets retirés en 2022), © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using System;
using CalamityFandom.Content.Legacy.Projectiles.Magic;
using CalamityFandom.Content.Legacy.Projectiles.Rogue;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.Weapons.Magic
{
    /// <summary>
    /// Wiki : Hadopelagic Echo.txt — rafale de 4 ondes qui traversent tout et rebondissent sur les blocs ; l'onde
    /// commence petite à 5 % des dégâts et atteint sa taille et ses dégâts pleins en une seconde ; au contact, deux ondes
    /// à 20 % arrivent de directions au hasard (2 s de recharge) ; Crush Depth 4 s.
    /// </summary>
    public class HadopelagicEcho : ModItem
    {
        private int counter;

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 500;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 15;
            Item.width = Item.height = 60;
            Item.useTime = 8;
            Item.useAnimation = 30;
            Item.reuseDelay = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1.5f;
            Item.value = Item.sellPrice(gold: 50);
            Item.rare = ModContent.RarityType<CalamityMod.Rarities.HotPink>();
            Item.autoReuse = true;
            Item.shootSpeed = 10f;
            Item.shoot = ModContent.ProjectileType<HadopelagicEchoSoundwave>();
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, counter);
            counter = (counter + 1) % 5;
            return false;
        }
    }

    /// <summary>Wiki : Crystal Flare Staff.txt — deux flammes de givre par usage, qui apparaissent peu à peu et explosent au contact (Frostburn 3 s).</summary>
    public class CrystalFlareStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 50;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 15;
            Item.width = 44;
            Item.height = 46;
            Item.useTime = 10;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5.25f;
            Item.value = Item.sellPrice(gold: 12);
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SpiritFlameCurse>();
            Item.shootSpeed = 14f;
        }
    }
}

namespace CalamityFandom.Content.Legacy.Items.Weapons.Rogue
{
    /// <summary>
    /// Wiki : Bouncing Betty.txt — grenade rogue consommable (pile de 9999) vendue 4 argent par le Bandit après un boss
    /// mécanique ; rebondit deux fois sur les ennemis en explosant à chaque fois et rebondit un moment sur les blocs ;
    /// attaque furtive : 12 éclats et étincelles de feu (On Fire! 3 à 6 s) à l'explosion.
    /// </summary>
    public class BouncingBetty : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 99;

        public override void SetDefaults()
        {
            Item.damage = 52;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.noMelee = true;
            Item.consumable = true;
            Item.noUseGraphic = true;
            Item.useTime = Item.useAnimation = 23;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.width = 16;
            Item.height = 22;
            Item.maxStack = 9999;
            Item.value = Item.buyPrice(silver: 4);
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ModContent.ProjectileType<BouncingBettyProjectile>();
            Item.shootSpeed = 16f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var grenade = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (player.Calamity().StealthStrikeAvailable())
                grenade.Calamity().stealthStrike = true;
            return false;
        }
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.Magic
{
    /// <summary>Onde de la Hadopelagic Echo (sprite de l'Eidolic Wail) ; ai[0] = rang dans la rafale (son sur la 1re et la 5e).</summary>
    public class HadopelagicEchoSoundwave : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Magic/EidolicWailSoundwave";

        // [choix] « grows to full size and damage after one second » : de 5 % à 100 % en 120 mises à jour (extraUpdates = 1)
        private const float Growth = 0.95f / 120f;
        private int echoCooldown;
        private bool playedSound;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 36;
            Projectile.scale = 0.05f;
            Projectile.alpha = 100;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 16;
            Projectile.timeLeft = 450;
        }

        public override void AI()
        {
            if ((Projectile.ai[0] == 0f || Projectile.ai[0] == 4f) && !playedSound)
            {
                SoundEngine.PlaySound(Main.rand.NextBool(100) ? Sunskater.DeathSound : CommonCalamitySounds.WyrmScreamSound, Projectile.Center);
                playedSound = true;
            }
            if (Projectile.localAI[0] == 0f)
                Projectile.localAI[0] = 0.05f;
            if (Projectile.localAI[0] < 1f)
            {
                Projectile.localAI[0] = Math.Min(1f, Projectile.localAI[0] + Growth);
                Projectile.scale = Projectile.localAI[0];
                Vector2 center = Projectile.Center;
                Projectile.width = Projectile.height = Math.Max(2, (int)(36f * Projectile.scale));
                Projectile.Center = center;
            }
            if (echoCooldown > 0)
                echoCooldown--;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => modifiers.SourceDamage *= Projectile.localAI[0];

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 240);
            if (echoCooldown > 0)
                return;
            // Recharge de 2 s : 240 mises à jour avec extraUpdates = 1 (l'ancien code comptait 120, soit 1 s)
            echoCooldown = 240;
            if (Projectile.owner != Main.myPlayer)
                return;
            for (int i = 0; i < 2; i++)
            {
                Vector2 dir = Main.rand.NextVector2Unit();
                Vector2 start = target.Center + dir * Main.rand.NextFloat(260f, 270f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), start, dir * -Main.rand.NextFloat(15f, 18f), ModContent.ProjectileType<HadopelagicEcho2>(),
                    (int)(0.2f * Projectile.damage), Projectile.knockBack / 3f, Projectile.owner);
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.timeLeft < 85)
            {
                byte b = (byte)(Projectile.timeLeft * 3);
                return new Color(b, b, b, (byte)(100f * b / 255f));
            }
            return new Color(255, 255, 255, 100);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }
    }

    /// <summary>Onde d'écho (20 % des dégâts), traverse les blocs, Crush Depth 4 s.</summary>
    public class HadopelagicEcho2 : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Magic/EidolicWailSoundwave";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 36;
            Projectile.scale = 0.85f;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
            Projectile.timeLeft = 30;
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Projectile.alpha = Math.Max(100, Projectile.alpha - 25);
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<CrushDepth>(), 240);

        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 100);

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }
    }

    /// <summary>Flamme de givre de la Crystal Flare Staff : ralentit, apparaît peu à peu, explose (160 px) au contact ou à la fin.</summary>
    public class SpiritFlameCurse : ModProjectile
    {
        public override void SetStaticDefaults() => Main.projFrames[Type] = 8;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.alpha = 255;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.coldDamage = true;
        }

        public override void AI()
        {
            Projectile.alpha -= 3;
            if (Projectile.alpha <= 0)
                Projectile.Kill();
            Projectile.velocity *= 0.975f;
            if (++Projectile.frameCounter > 4)
            {
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
                Projectile.frameCounter = 0;
            }
            Lighting.AddLight(Projectile.Center, 0f, 0.25f, 0.5f);
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.Resize(160, 160);
            Projectile.maxPenetrate = Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            for (int i = 0; i < 20; i++)
            {
                var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 160, 0f, 0f, 100, default, 2f);
                dust.velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    dust.scale = 0.5f;
                    dust.fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            for (int i = 0; i < 35; i++)
            {
                var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 160, 0f, 0f, 100, default, 3f);
                dust.noGravity = true;
                dust.velocity *= 5f;
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 160, 0f, 0f, 100, default, 2f).velocity *= 2f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Frostburn, 180);
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.Rogue
{
    /// <summary>
    /// Grenade de la Bouncing Betty : rebondit sur les blocs pendant ~1 s puis explose dès qu'elle touche le sol ; à chaque
    /// ennemi touché (3 au plus), elle explose et repart en arrière. Attaque furtive : 12 éclats ou étincelles par explosion.
    /// </summary>
    public class BouncingBettyProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.timeLeft = 600;
            Projectile.penetrate = 3;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.ignoreWater = true;
        }

        private void Explode()
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            if (Main.myPlayer != Projectile.owner)
                return;
            var source = Projectile.GetSource_FromThis();
            Projectile.NewProjectile(source, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BettyExplosion>(), Projectile.damage, 8f, Projectile.owner);
            if (!Projectile.Calamity().stealthStrike)
                return;
            for (int i = 0; i < 12; i++)
            {
                if (Main.rand.NextBool(2))
                {
                    Vector2 velocity = (Vector2.UnitY * Main.rand.NextFloat(-12f, -4f)).RotatedByRandom(MathHelper.ToRadians(30f));
                    Projectile.NewProjectile(source, Projectile.Center, Projectile.velocity + velocity, ModContent.ProjectileType<BouncingBettyShrapnel>(), (int)(Projectile.damage * 0.5f), 3f, Projectile.owner);
                }
                else
                {
                    Vector2 velocity = (Vector2.UnitY * Main.rand.NextFloat(-12f, -4f)).RotatedByRandom(MathHelper.ToRadians(40f));
                    var fire = Projectile.NewProjectileDirect(source, Projectile.Center, Projectile.velocity + velocity, ModContent.ProjectileType<TotalityFire>(), (int)(Projectile.damage * 0.6f), 1f, Projectile.owner);
                    fire.localNPCHitCooldown = 9;
                    fire.timeLeft = 240;
                    // « On Fire! 3-6 seconds (flame spark) » (infobox) ; l'étincelle de Calamity donne seulement Hellfire
                    LegacyHitDebuff.Give(fire, BuffID.OnFire, Main.rand.Next(180, 361));
                }
            }
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Point below = Projectile.Bottom.ToTileCoordinates();
            Tile tile = Framing.GetTileSafely(below.X, below.Y + 1);
            if (tile.HasUnactuatedTile && WorldGen.SolidTile(tile) && Projectile.timeLeft < 575)
            {
                Explode();
                Projectile.Kill();
                return;
            }
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.4f, 16f);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity *= -1f;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Explode();
            Projectile.velocity *= -1f;
        }
    }

    /// <summary>Explosion de la Bouncing Betty (75 px, invisible).</summary>
    public class BettyExplosion : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 75;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 20;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0f)
            {
                for (int i = 0; i < 15; i++)
                {
                    var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 31, 0f, 0f, 100, default, 2f);
                    dust.velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        dust.scale = 0.5f;
                        dust.fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int i = 0; i < 25; i++)
                {
                    var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 3f);
                    dust.noGravity = true;
                    dust.velocity *= 5f;
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 2f).velocity *= 2f;
                }
                if (!Main.dedServ)
                {
                    Vector2 corner = Projectile.Center - new Vector2(24f);
                    for (int i = 0; i < 3; i++)
                    {
                        float mult = i == 0 ? 0.66f : i == 2 ? 1f : 0.33f;
                        foreach (Vector2 push in new[] { new Vector2(1f, 1f), new Vector2(-1f, 1f), new Vector2(1f, -1f), new Vector2(-1f, -1f) })
                        {
                            Gore gore = Gore.NewGoreDirect(Projectile.GetSource_FromAI(), corner, default, Main.rand.Next(61, 64));
                            gore.velocity = gore.velocity * mult + push;
                        }
                    }
                }
            }
            if (Projectile.ai[0] <= 1f)
                Projectile.ai[0]++;
        }
    }

    /// <summary>Éclat de la Bouncing Betty (attaque furtive) : tombe, traverse les blocs, un ennemi.</summary>
    public class BouncingBettyShrapnel : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override void AI()
        {
            if (Math.Abs(Projectile.position.Y - Projectile.oldPosition.Y) > 4f)
            {
                Projectile.velocity.X = 0f;
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
            else
                Projectile.rotation = MathHelper.Pi;
            Projectile.velocity.Y += 0.2f;
        }
    }
}
