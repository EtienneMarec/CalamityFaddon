using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Items.Weapons.Typeless
{
    /// <summary>
    /// Wiki : Trash of Magnus.txt — arme sans classe inventée par le wiki (le « déchet » de l'Eye of Magnus), lâchée par le
    /// Plaguebringer Goliath (WikiLoot) : rapides éclairs de peste qui infligent la Plague 8 s ; profite de tous les bonus
    /// de dégâts à la fois (AverageDamageClass de Calamity, comme l'Eye of Magnus) ; pas de modificateurs ; critique 0 %.
    /// Valeurs du wiki : 24 dégâts, recul 2, vélocité 12, use 20, rareté 8, vente 6 or.
    /// </summary>
    public class TrashofMagnus : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            // Sprite dessiné en diagonale : tenu comme un bâton, pointé vers la cible (retour du 30/10)
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 56;
            Item.damage = 24;
            Item.crit = -4;
            Item.DamageType = ModContent.GetInstance<AverageDamageClass>();
            Item.useTime = Item.useAnimation = 20;
            Item.knockBack = 2f;
            Item.shoot = ModContent.ProjectileType<TrashofMagnusBolt>();
            Item.shootSpeed = 12f;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.value = Item.sellPrice(gold: 6);
            Item.rare = ItemRarityID.Yellow;
        }

        public override bool AllowPrefix(int pre) => false;

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
            => itemGroup = (ContentSamples.CreativeHelper.ItemGroup)CalamityResearchSorting.ClasslessWeapon;
    }

    /// <summary>Éclair de peste du Trash of Magnus : invisible, traînée verte, Plague 8 s.</summary>
    public class TrashofMagnusBolt : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<AverageDamageClass>();
            Projectile.extraUpdates = 2;
            Projectile.timeLeft = 200;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.2f, 0.5f, 0.1f);
            for (int i = 0; i < 2; i++)
            {
                var dust = Dust.NewDustPerfect(Projectile.Center - Projectile.velocity * (i * 0.5f), DustID.GreenFairy);
                dust.velocity = Vector2.Zero;
                dust.noGravity = true;
                dust.scale = 1.1f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Plague>(), 480);
    }
}
