// Adapté de Calamity Mod (objets retirés en 2022), © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
using System;
using CalamityFandom.Content.Legacy.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.Weapons.Magic
{
    /// <summary>
    /// Wiki : Shadowdrop Staff.txt — fait tomber du ciel des groupes de 3 gouttes d'aura sombre vers le curseur ; elles ne
    /// traversent ni les blocs ni les ennemis (page ; le vieux code les faisait percer).
    /// </summary>
    public class ShadowdropStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 25;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 6;
            Item.width = Item.height = 44;
            Item.useTime = 9;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.25f;
            Item.value = Item.sellPrice(silver: 80);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item66;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<AuraRain>();
            Item.shootSpeed = 14f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Comme la Blood Bath / la Nimbus Rod : départ au-dessus de l'écran, entre le joueur et le curseur, vers le curseur
            Vector2 mouse = Main.MouseWorld;
            for (int i = 0; i < 3; i++)
            {
                Vector2 spawn = new((player.Center.X + mouse.X) / 2f + Main.rand.Next(-200, 201), player.Center.Y - 600f - 100f * i);
                Vector2 toMouse = mouse - spawn;
                if (toMouse.Y < 20f)
                    toMouse.Y = 20f;
                Vector2 speed = toMouse.SafeNormalize(Vector2.UnitY) * Item.shootSpeed + new Vector2(Main.rand.Next(-30, 31), Main.rand.Next(-30, 31)) * 0.02f;
                Projectile.NewProjectile(source, spawn, speed, type, damage, knockback, player.whoAmI);
            }
            return false;
        }
    }

    /// <summary>
    /// Wiki : Hell Burst.txt — chaîne de grandes pointes de feu (comme la Crystal Vile Shard) qui restent un instant, On Fire!
    /// 4 s ; la dernière pointe est plus grosse.
    /// </summary>
    public class HellBurst : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 30;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 14;
            Item.width = Item.height = 52;
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 7f;
            Item.value = Item.sellPrice(gold: 7, silver: 20);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item34;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<FlameBeamTip>();
            Item.shootSpeed = 32f;
        }
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.Magic
{
    /// <summary>Goutte d'aura sombre (pluie vanilla), un seul ennemi.</summary>
    public class AuraRain : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.aiStyle = 45; // pluie vanilla (RainFriendly)
            AIType = ProjectileID.RainFriendly;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300;
            Projectile.alpha = 255;
            Projectile.scale = 1.1f;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Demonite, Projectile.velocity.X, Projectile.velocity.Y, 100);
            dust.velocity = Vector2.Zero;
            dust.position -= Projectile.velocity / 5f;
            dust.noGravity = true;
            dust.scale = 0.8f;
            dust.noLight = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation,
                texture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }

    /// <summary>
    /// Pointe de feu de la Hell Burst (IA de la Crystal Vile Shard) : chaque pointe en crée une autre plus loin, jusqu'à 15 à 17,
    /// la dernière étant FlameBeamTip2 ; On Fire! 4 s, 8 frames entre deux coups sur un ennemi.
    /// </summary>
    public class FlameBeamTip : ModProjectile
    {
        protected virtual bool Spreads => true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.aiStyle = ProjAIStyleID.Vilethorn;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 8;
        }

        public override bool PreAI()
        {
            if (Projectile.ai[0] != 0f && Projectile.alpha < 170 && Projectile.alpha + 5 >= 170)
            {
                for (int i = 0; i < 10; i++)
                {
                    var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.RedTorch, Projectile.velocity.X * 0.01f,
                        Projectile.velocity.Y * 0.01f, 200, default, 2f);
                    dust.noGravity = true;
                }
            }
            return true;
        }

        public override void AI()
        {
            if (Projectile.ai[0] != 0f)
                return;
            Projectile.alpha -= 50;
            if (Projectile.alpha > 0)
                return;
            Projectile.alpha = 0;
            Projectile.ai[0] = 1f;
            if (Projectile.ai[1] == 0f)
            {
                Projectile.ai[1] += 1f;
                Projectile.position += Projectile.velocity;
            }
            if (Spreads && Main.myPlayer == Projectile.owner)
            {
                int next = Projectile.ai[1] >= 15 + Main.rand.Next(3) ? ModContent.ProjectileType<FlameBeamTip2>() : Type;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity, Projectile.velocity, next,
                    Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, Projectile.ai[1] + 1f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.OnFire, 240);
    }

    /// <summary>Dernière pointe, plus grosse, qui ne se prolonge pas.</summary>
    public class FlameBeamTip2 : FlameBeamTip
    {
        protected override bool Spreads => false;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 40;
        }
    }
}
