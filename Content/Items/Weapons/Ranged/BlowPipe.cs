using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Items.Weapons.Ranged
{
    /// <summary>
    /// Wiki : Blow Pipe.txt — sarbacane inventée par le wiki, lâchée par le Plaguebringer Goliath (WikiLoot) : tire des
    /// fléchettes ou des graines (munition Dart, comme le Blowgun de vanilla), 70 % de chances de ne pas consommer de
    /// munition ; clic droit : dard de peste (Plague 10 s), vitesse doublée (28). Valeurs du wiki : 36 dégâts, recul 1,5,
    /// vélocité 10, use 14, rareté 8, vente 16 or.
    /// </summary>
    public class BlowPipe : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 28;
            Item.damage = 36;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1.5f;
            Item.UseSound = SoundID.Item63;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.Seed;
            Item.shootSpeed = 10f;
            Item.useAmmo = AmmoID.Dart;
            Item.value = Item.sellPrice(gold: 16);
            Item.rare = ItemRarityID.Yellow;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override float UseSpeedMultiplier(Player player) => player.altFunctionUse == 2 ? 0.5f : 1f;

        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.NextFloat() >= 0.7f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<BlowPipeStinger>(), damage, knockback, player.whoAmI);
                return false;
            }
            return true;
        }
    }

    /// <summary>Dard de peste de la Blow Pipe : image du dard de vanilla, Plague 10 s.</summary>
    public class BlowPipeStinger : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.Stinger}";

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 300;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(3))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GreenFairy, 0f, 0f, 100, default, 0.8f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Plague>(), 600);
    }
}
