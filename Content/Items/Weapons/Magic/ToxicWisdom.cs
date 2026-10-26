using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Items.Weapons.Magic
{
    /// <summary>
    /// Wiki : Toxic Wisdom.txt — tome inventé par le wiki, lâché par le Plaguebringer Goliath (WikiLoot) : tire des
    /// hiéroglyphes empoisonnés et des volutes de Pigron qui infligent la Plague 5 s. Valeurs du wiki : 64 dégâts, recul
    /// 5,5, mana 18, vitesse 15, vélocité 5,5, rareté 8, vente 16 or. [choix] Les deux tirs alternent : hiéroglyphe vert
    /// qui file droit, volute rose qui ondule ; ni l'un ni l'autre n'a d'image sur le wiki (poussière).
    /// </summary>
    public class ToxicWisdom : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 33;
            Item.height = 34;
            Item.damage = 64;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 18;
            Item.useTime = Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5.5f;
            Item.value = Item.sellPrice(gold: 16);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ToxicWisdomBolt>();
            Item.shootSpeed = 5.5f;
        }

        private bool wisp;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            wisp = !wisp;
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, wisp ? 1f : 0f);
            return false;
        }
    }

    /// <summary>Tir du Toxic Wisdom : ai[0] = 0 hiéroglyphe (droit, vert), 1 volute de Pigron (ondule, rose).</summary>
    public class ToxicWisdomBolt : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.extraUpdates = 2;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            bool wisp = Projectile.ai[0] == 1f;
            if (wisp)
                Projectile.velocity = Projectile.velocity.RotatedBy(System.Math.Sin(++Projectile.localAI[0] * 0.1f) * 0.03f);
            var dust = Dust.NewDustPerfect(Projectile.Center, wisp ? DustID.PinkTorch : DustID.GreenTorch, Vector2.Zero, 100, default, 1.4f);
            dust.noGravity = true;
            Lighting.AddLight(Projectile.Center, wisp ? new Vector3(0.5f, 0.2f, 0.4f) : new Vector3(0.2f, 0.5f, 0.1f));
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Plague>(), 300);
    }
}
