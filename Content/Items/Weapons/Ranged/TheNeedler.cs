using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Items.Weapons.Ranged
{
    /// <summary>
    /// Wiki : The Needler.txt — arme inventée par le wiki, lâchée par le Plaguebringer Goliath (WikiLoot) : « identique à
    /// la Needler » de Calamity, mais inflige la Plague. Les balles normales deviennent des aiguilles (TheNeedlerNeedle,
    /// code de la NeedlerProj de Calamity). Valeurs du wiki : 45 dégâts, recul 5,5, vélocité 9, use 18, rareté 4, vente
    /// 2 or 40 argent ; classe ranged (infobox). [choix] Plague 1,5 s, la durée du Venom de la Needler.
    /// </summary>
    public class TheNeedler : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 46;
            Item.damage = 45;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5.5f;
            Item.value = Item.sellPrice(gold: 2, silver: 40);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item108;
            Item.autoReuse = true;
            Item.shootSpeed = 9f;
            Item.shoot = ModContent.ProjectileType<TheNeedlerNeedle>();
            Item.useAmmo = AmmoID.Bullet;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type == ProjectileID.Bullet ? ModContent.ProjectileType<TheNeedlerNeedle>() : type,
                damage, knockback, player.whoAmI);
            return false;
        }
    }

    public class TheNeedlerNeedle : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.extraUpdates = 2;
            Projectile.aiStyle = ProjAIStyleID.Nail;
            AIType = ProjectileID.NailFriendly;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 1;
        }

        public override void AI()
        {
            Projectile.alpha = System.Math.Max(Projectile.alpha - 10, 0);
            Projectile.localAI[1] += 1f;
            if (Projectile.localAI[1] > 6f && Projectile.numHits < 1)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(3) ? 207 : 256, -Projectile.velocity * Main.rand.NextFloat(0.2f, 0.8f));
                dust.scale = Main.rand.NextFloat(0.6f, 0.9f);
                dust.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Plague>(), 90);

        public override void OnKill(int timeLeft)
        {
            for (int k = 0; k < 9; k++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(6) ? 44 : 39, new Vector2(3, 3).RotatedByRandom(100) * Main.rand.NextFloat(0.3f, 1.3f));
                dust.scale = Main.rand.NextFloat(0.8f, 1.8f);
                dust.fadeIn = 0.5f;
            }
        }
    }
}
