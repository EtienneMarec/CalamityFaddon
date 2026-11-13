// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using System;
using CalamityFandom.Content.Legacy.Projectiles.DraedonsArsenal;
using CalamityMod;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

// Lot du 13/11 (fin) : Gatling Laser (arsenal de Draedon, charge de Calamity). Code de 2.0.1.005, valeurs de l'infobox.

namespace CalamityFandom.Content.Legacy.Items.Weapons.DraedonsArsenal
{
    /// <summary>
    /// Wiki : Gatling Laser.txt — canon tenu : un temps de chargement, puis une rafale de lasers rouges rapides et imprécis
    /// (un toutes les deux images), petite explosion, On Fire! 1,5 s ; 0,0075 de charge par image tenue, charge max 135.
    /// </summary>
    public class GatlingLaser : ModItem
    {
        public const float HoldoutChargeUse = 0.0075f;

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 43;
            Item.height = 24;
            Item.DamageType = DamageClass.Magic;
            Item.damage = 43;
            Item.knockBack = 1f;
            Item.useTime = Item.useAnimation = 2;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.mana = 6;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item157;
            Item.noMelee = true;
            Item.value = Item.sellPrice(gold: 12);
            Item.rare = ModContent.RarityType<DarkOrange>();
            Item.shoot = ModContent.ProjectileType<GatlingLaserProj>();
            Item.shootSpeed = 24f;
            var cal = Item.Calamity();
            cal.UsesCharge = true;
            cal.MaxCharge = 135f;
            cal.ChargePerUse = 0f; // la charge est prise par le canon tenu, à chaque laser
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override Vector2? HoldoutOffset() => new Vector2(-20, 0);
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.DraedonsArsenal
{
    /// <summary>Canon tenu du Gatling Laser : tire tant que le bouton est tenu, qu'il reste du mana et de la charge.</summary>
    public class GatlingLaserProj : ModProjectile
    {
        private bool fireLasers;

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 58;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Vector2 center = player.RotatedRelativePoint(player.MountedCenter, true);
            if (Projectile.ai[0] < 5f)
                Projectile.ai[0]++;
            if (Projectile.ai[0] > 4f)
                Projectile.ai[0] = 2f;
            bool shootThisFrame = false;
            if (++Projectile.ai[1] >= 2f)
            {
                Projectile.ai[1] = 0f;
                shootThisFrame = true;
            }
            // Temps de chargement : les lasers partent au premier son de boucle
            if (Projectile.soundDelay <= 0)
            {
                Projectile.soundDelay = 12;
                if (Projectile.ai[0] != 1f)
                {
                    fireLasers = true;
                    Projectile.soundDelay *= 6;
                    SoundEngine.PlaySound(SoundID.Item157 with { Volume = 0.6f }, Projectile.Center);
                }
            }
            if (fireLasers && Projectile.soundDelay % 6 == 0)
                SoundEngine.PlaySound(SoundID.Item91 with { Volume = 0.25f, PitchVariance = 0.2f }, Projectile.Center);

            if (shootThisFrame && Main.myPlayer == Projectile.owner && fireLasers)
            {
                Item gatling = player.HeldItem;
                var cal = gatling.Calamity();
                bool stillInUse = player.channel && !player.noItems && !player.CCed;
                if (stillInUse && cal.Charge >= Items.Weapons.DraedonsArsenal.GatlingLaser.HoldoutChargeUse && player.CheckMana(gatling, -1, true))
                {
                    Vector2 aim = (Main.MouseWorld - center).SafeNormalize(-Vector2.UnitY) * gatling.shootSpeed * Projectile.scale;
                    if (aim != Projectile.velocity)
                        Projectile.netUpdate = true;
                    Projectile.velocity = aim;
                    Vector2 shot = Vector2.Normalize(Projectile.velocity).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi / 32f, MathHelper.Pi / 32f)) * 3f;
                    shot += new Vector2(Main.rand.Next(-1, 2), Main.rand.Next(-1, 2)) * 0.005f;
                    cal.Charge -= Items.Weapons.DraedonsArsenal.GatlingLaser.HoldoutChargeUse;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shot, ModContent.ProjectileType<GatlingLaserShot>(),
                        player.GetWeaponDamage(gatling), Projectile.knockBack, Projectile.owner, Projectile.ai[0] - 2f);
                }
                else
                {
                    SoundEngine.PlaySound(SoundID.Item158 with { Volume = 0.6f }, Projectile.Center);
                    Projectile.Kill();
                    return;
                }
            }
            Projectile.position = center - Projectile.Size / 2f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);
        }

        public override bool? CanDamage() => false;
    }

    /// <summary>Laser du Gatling Laser : trait de poussière rouge instantané, petite explosion, On Fire! 1,5 s.</summary>
    public class GatlingLaserShot : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private const int DustType = 182;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 100;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.2f, 0f, 0f);
            if (++Projectile.localAI[0] == 10f)
                LaserBurst(1.8f, 3f);
            if (Projectile.localAI[0] <= 10f || Main.dedServ)
                return;
            // Trois traits côte à côte selon le canon (ai[0] : 0, 1 ou 2)
            Vector2 offset = Projectile.ai[0] switch
            {
                1f => -Vector2.UnitY.RotatedBy(24f * 0.1308997f) * new Vector2(5f, 10f) * 0.5f,
                2f => -Vector2.UnitY.RotatedBy(24f * 0.1308997f + MathHelper.Pi) * new Vector2(5f, 10f) * 0.5f,
                _ => Vector2.Zero,
            };
            Dust d = Dust.NewDustDirect(Projectile.Center, 0, 0, DustType, 0f, 0f, 160, default, 0.8f);
            d.noGravity = true;
            d.position = Projectile.Center + offset;
            d.velocity = Projectile.velocity;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.OnFire, 90);

        public override void OnKill(int timeLeft)
        {
            Projectile.Resize(20, 20);
            LaserBurst(2.4f, 4.2f);
            if (Main.dedServ)
                return;
            for (int i = 0; i < 100; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.Center, 6, 6, DustType, 0f, 0f, 100);
                Vector2 dir = d.velocity == Vector2.Zero ? Vector2.UnitX : d.velocity.SafeNormalize(Vector2.UnitX);
                d.velocity = d.velocity * 0.5f + dir * 4f;
                d.scale = 0.66f;
                d.noGravity = true;
            }
        }

        private void LaserBurst(float speed1, float speed2)
        {
            if (Main.dedServ)
                return;
            const float angleRandom = 0.05f;
            for (int i = 0; i < 4; i++)
            {
                Vector2 vel = new Vector2(Main.rand.NextFloat(speed1, speed2), 0f).RotatedBy(Projectile.velocity.ToRotation() - angleRandom).RotatedByRandom(2f * angleRandom);
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustType, vel.X, vel.Y, 200, default, 0.9f);
                d.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * Main.rand.NextFloat() * Projectile.width / 2f;
                d.noGravity = true;
                d.velocity *= 3f;
                d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustType, vel.X, vel.Y, 100, default, 0.66f);
                d.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * Main.rand.NextFloat() * Projectile.width / 2f;
                d.velocity *= 2f;
                d.noGravity = true;
                d.fadeIn = 1f;
                d.color = Color.Crimson * 0.5f;
            }
            for (int i = 0; i < 2; i++)
            {
                Vector2 vel = new Vector2(Main.rand.NextFloat(speed1, speed2), 0f).RotatedBy(Projectile.velocity.ToRotation() - angleRandom).RotatedByRandom(2f * angleRandom);
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustType, vel.X, vel.Y, 0, default, 1.2f);
                d.position = Projectile.Center + Vector2.UnitX.RotatedByRandom(MathHelper.Pi).RotatedBy(Projectile.velocity.ToRotation()) * Projectile.width / 3f;
                d.noGravity = true;
                d.velocity *= 0.5f;
            }
        }
    }
}
