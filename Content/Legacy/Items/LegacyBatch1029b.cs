// Adapté de Calamity Mod (objets retirés en 2022), © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
using System;
using CalamityFandom.Content.Legacy.Projectiles.Magic;
using CalamityMod;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.Weapons.Ranged
{
    /// <summary>
    /// Wiki : Raider_s Glory.txt — tire des flèches d'Ichor plus rapides (une mise à jour de plus) ; « These arrows also cause
    /// enemies to drop more money » : elles infligent Midas (l'ancien code ne le faisait pas, le wiki fait foi).
    /// </summary>
    public class RaidersGlory : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 44;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 50;
            Item.height = 22;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.25f;
            Item.value = Item.sellPrice(gold: 7, silver: 20);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 15f;
            Item.useAmmo = AmmoID.Arrow;
        }

        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 10;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var arrow = Projectile.NewProjectileDirect(source, position, velocity, ProjectileID.IchorArrow, damage, knockback, player.whoAmI);
            arrow.extraUpdates++;
            arrow.GetGlobalProjectile<RaidersGloryArrow>().Midas = true;
            return false;
        }
    }

    /// <summary>Flèches de la Raider's Glory : Midas au contact (« drop more money »).</summary>
    public class RaidersGloryArrow : GlobalProjectile
    {
        public bool Midas;

        public override bool InstancePerEntity => true;

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Midas)
                target.AddBuff(BuffID.Midas, 600);
        }
    }
}

namespace CalamityFandom.Content.Legacy.Items.Weapons.Magic
{
    /// <summary>Wiki : Bittercold Staff.txt — gerbe de 2 à 5 pics de glace à tête chercheuse (Frostburn 3 s).</summary>
    public class BittercoldStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 40;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 17;
            Item.width = Item.height = 52;
            Item.useTime = Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.value = Item.sellPrice(gold: 4, silver: 80);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item46;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<IceSpike>();
            Item.shootSpeed = 10f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // 2 pics, +1 à 1/3, +1 à 1/4, +1 à 1/5 ; chacun plus dispersé que le précédent (ancien code)
            int count = 2 + (Main.rand.NextBool(3) ? 1 : 0) + (Main.rand.NextBool(4) ? 1 : 0) + (Main.rand.NextBool(5) ? 1 : 0);
            Vector2 aim = Main.MouseWorld - player.MountedCenter;
            for (int i = 0; i < count; i++)
            {
                Vector2 target = aim + new Vector2(Main.rand.Next(-100, 100), Main.rand.Next(-100, 100)) * 0.05f * i;
                Projectile.NewProjectile(source, player.MountedCenter, target.SafeNormalize(Vector2.UnitX) * Item.shootSpeed, type, damage, knockback, player.whoAmI);
            }
            return false;
        }
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.Magic
{
    /// <summary>Pic de glace de la Bittercold Staff : cherche les ennemis proches, Frostburn 3 s, se brise au contact.</summary>
    public class IceSpike : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Boss/IceRain";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 240;
        }

        public override void AI()
        {
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi) + MathHelper.PiOver2 * Projectile.direction;
            Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.IceTorch, 0f, 0f, 100).noGravity = true;
            CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 200f, 10f, 20f);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
            for (int k = 0; k < 5; k++)
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.IceTorch);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Frostburn, 180);
    }
}
