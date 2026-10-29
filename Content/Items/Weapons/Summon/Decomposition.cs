using CalamityMod;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Items.Weapons.Summon
{
    /// <summary>
    /// Wiki : Decomposition.txt — bâton de tourelle inventé par le wiki, lâché par le Plaguebringer Goliath (WikiLoot) :
    /// pose une ruche malade qui lâche des abeilles comme la Plaguenade (PlaguenadeBee de Calamity : dégâts spy, Notes ;
    /// Plague). Valeurs du wiki : 75 dégâts, recul 4, mana 10, use 19, rareté 7, vente 12 or. [choix] Une abeille toutes
    /// les 40 ticks quand un ennemi est à moins de 600 px ; la ruche se pose au curseur et tombe au sol ; Plague des
    /// abeilles = celle de la Plaguenade (3 s, le wiki dit 1 s).
    /// </summary>
    public class Decomposition : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.GamepadWholeScreenUseRange[Type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 44;
            Item.damage = 75;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 10;
            Item.useTime = Item.useAnimation = 19;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 4f;
            Item.value = Item.sellPrice(gold: 12);
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = SoundID.Item46;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<DecompositionHive>();
            Item.sentry = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var hive = Projectile.NewProjectileDirect(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI);
            hive.originalDamage = Item.damage;
            player.UpdateMaxTurrets();
            return false;
        }
    }

    /// <summary>Ruche de Decomposition : tourelle qui tombe au sol et lâche des PlaguenadeBee vers les ennemis proches.</summary>
    public class DecompositionHive : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Summon/DecompositionHive";

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 80;
            Projectile.friendly = true;
            Projectile.sentry = true;
            Projectile.timeLeft = Projectile.SentryLifeTime;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override bool? CanDamage() => false;

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        // Se pose aussi sur les plateformes (retour du 30/10)
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }

        public override void AI()
        {
            Projectile.velocity.X = 0f;
            Projectile.velocity.Y = MathHelper.Min(Projectile.velocity.Y + 0.4f, 12f);
            if (++Projectile.ai[0] < 40f || Projectile.owner != Main.myPlayer)
                return;
            NPC target = Projectile.Center.ClosestNPCAt(600f);
            if (target == null)
                return;
            Projectile.ai[0] = 0f;
            Vector2 velocity = (target.Center - Projectile.Center).SafeNormalize(-Vector2.UnitY) * 6f;
            var bee = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Top + new Vector2(0f, 16f), velocity,
                ModContent.ProjectileType<PlaguenadeBee>(), Projectile.damage, 0f, Projectile.owner);
            bee.originalDamage = Projectile.originalDamage;
        }
    }
}
