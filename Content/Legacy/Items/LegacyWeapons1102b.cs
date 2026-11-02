// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using CalamityFandom.Content.Legacy.Projectiles.Ranged;
using CalamityFandom.Content.Legacy.Projectiles.DraedonsArsenal;
using CalamityMod;
using CalamityMod.Projectiles.DraedonsArsenal;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

// Lot du 02/11 (suite) : deux armes de l'arsenal de Draedon (charge de Calamity) et le Magna Striker. Code de 2.0.1.005,
// valeurs des infobox (Gauss Dagger use 23 ; Gauss Pistol 110 dégâts au lieu de 150), recettes du wiki.

namespace CalamityFandom.Content.Legacy.Items.Weapons.DraedonsArsenal
{
    /// <summary>
    /// Wiki : Gauss Dagger.txt — dague électrique (charge) : frapper deux fois le même ennemi l'enveloppe d'un flux magnétique
    /// (GaussFlux de Calamity). [technique] Le compteur de flux de Calamity n'existe plus : remplacé par GaussFluxCounter.
    /// </summary>
    public class GaussDagger : ModItem
    {
        public const int HitsRequiredForFlux = 2;

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 25;
            Item.DamageType = DamageClass.Melee;
            Item.width = Item.height = 26;
            Item.scale = 1.5f;
            Item.useTime = Item.useAnimation = 23;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.knockBack = 7f;
            Item.value = Item.sellPrice(silver: 80);
            Item.rare = ModContent.RarityType<DarkOrange>();
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            var cal = Item.Calamity();
            cal.UsesCharge = true;
            cal.MaxCharge = 50f;
            cal.ChargePerUse = 0.05f;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            var counter = target.GetGlobalNPC<GaussFluxCounter>();
            counter.Timer += 50;
            if (counter.Timer < 30 * HitsRequiredForFlux)
                return;
            counter.Timer = 0;
            if (player.whoAmI == Main.myPlayer)
                Projectile.NewProjectile(player.GetSource_ItemUse(Item), target.Center, Vector2.Zero, ModContent.ProjectileType<GaussFlux>(),
                    hit.Crit ? hit.Damage / 2 : hit.Damage, 0f, player.whoAmI, 0f, target.whoAmI);
        }
    }

    /// <summary>Compteur de coups de la Gauss Dagger sur un ennemi (diminue avec le temps).</summary>
    public class GaussFluxCounter : GlobalNPC
    {
        public int Timer;

        public override bool InstancePerEntity => true;

        public override void ResetEffects(NPC npc)
        {
            if (Timer > 0)
                Timer--;
        }
    }

    /// <summary>
    /// Wiki : Gauss Pistol.txt — pistolet magique (charge) : un tir chargé d'une demi-seconde, très rapide et au recul énorme.
    /// </summary>
    public class GaussPistol : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 22;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 6;
            Item.damage = 110;
            Item.knockBack = 11f;
            Item.useTime = Item.useAnimation = 20;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = CommonCalamitySounds.PlasmaBoltSound; // [technique] le son Gauss de Calamity n'existe plus
            Item.noMelee = true;
            Item.value = Item.sellPrice(gold: 4, silver: 80);
            Item.rare = ModContent.RarityType<DarkOrange>();
            Item.shoot = ModContent.ProjectileType<GaussPistolShot>();
            Item.shootSpeed = 14f;
            var cal = Item.Calamity();
            cal.UsesCharge = true;
            cal.MaxCharge = 85f;
            cal.ChargePerUse = 0.05f;
        }
    }
}

namespace CalamityFandom.Content.Legacy.Items.Weapons.Ranged
{
    /// <summary>Wiki : Magna Striker.txt — rafales d'éclats d'opale (75 % des dégâts) et de frappes magna, au hasard.</summary>
    public class MagnaStriker : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 35;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 72;
            Item.height = 38;
            Item.useTime = 5;
            Item.reuseDelay = 6;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.25f;
            Item.value = Item.sellPrice(gold: 16);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = CalamityMod.Items.Weapons.Ranged.OpalStriker.Fire;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<OpalStrike>();
            Item.shootSpeed = 15f;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5f, 0f);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.rand.NextBool())
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<OpalStrike>(), (int)(damage * 0.75f), knockback, player.whoAmI);
            else
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<MagnaStrike>(), damage, knockback, player.whoAmI);
            return false;
        }
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.Ranged
{
    public class MagnaStrike : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.scale = 0.5f;
            Projectile.alpha = 200;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void AI()
        {
            if (Projectile.scale <= 1.5f)
                Projectile.scale *= 1.01f;
            Projectile.alpha -= 2;
            if (Projectile.alpha <= 0)
                Projectile.alpha = 200;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.DraedonsArsenal
{
    /// <summary>Tir du Gauss Pistol : reste en place une demi-seconde en se chargeant (sphère de poussière), puis part.</summary>
    public class GaussPistolShot : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private const float MaxChargeRadius = 7f, MinEnergyParticleSpeed = 0.25f, MaxEnergyParticleSpeed = 4f, ChargeTime = 30f;

        private ref float Time => ref Projectile.ai[0];
        private Vector2 heldVelocity;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 240;
        }

        public override void SendExtraAI(System.IO.BinaryWriter writer) => writer.WriteVector2(heldVelocity);

        public override void ReceiveExtraAI(System.IO.BinaryReader reader) => heldVelocity = reader.ReadVector2();

        public override void AI()
        {
            Time++;
            if (Time < ChargeTime && heldVelocity == Vector2.Zero)
            {
                heldVelocity = Projectile.velocity;
                Projectile.velocity = Vector2.Zero;
                Projectile.netUpdate = true;
            }
            if (Time >= ChargeTime && heldVelocity != Vector2.Zero)
            {
                Projectile.velocity = heldVelocity;
                heldVelocity = Vector2.Zero;
                Projectile.netUpdate = true;
            }
            if (Main.dedServ)
                return;
            float progress = Utils.GetLerpValue(0f, ChargeTime, Time, true);
            float energyRadius = MathHelper.Lerp(MaxChargeRadius * 0.333f, MaxChargeRadius, progress);
            float sphereSpeed = MathHelper.Lerp(MaxEnergyParticleSpeed, MinEnergyParticleSpeed, progress);
            if (Projectile.velocity == Vector2.Zero)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 offset = Main.rand.NextVector2CircularEdge(energyRadius, energyRadius) * 3f;
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + offset, 261);
                    dust.velocity = Projectile.DirectionFrom(dust.position) * energyRadius / 3f;
                    dust.color = Main.hslToRgb(Main.rand.NextFloat(0.15f, 0.3f), Main.rand.NextFloat(0.9f, 1f), Main.rand.NextFloat(0.5f, 0.66f));
                    dust.noGravity = true;
                }
            }
            for (int i = 0; i < 10; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(energyRadius, energyRadius), 261);
                dust.velocity = Main.rand.NextVector2Circular(sphereSpeed, sphereSpeed) + Projectile.velocity;
                dust.color = Main.hslToRgb(Main.rand.NextFloat(0.15f, 0.3f), Main.rand.NextFloat(0.9f, 1f), Main.rand.NextFloat(0.5f, 0.66f));
                dust.scale = 0.8f;
                dust.noGravity = true;
            }
        }
    }
}
