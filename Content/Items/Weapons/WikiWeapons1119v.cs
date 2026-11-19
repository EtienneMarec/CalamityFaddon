using System;
using CalamityFandom.Content.Projectiles.Weapons1119v;
using CalamityMod;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

// Lot Z (19/11) : armes du wiki fabricables à la Cosmic Anvil (recettes du wiki).

namespace CalamityFandom.Content.Items.Weapons.Magic
{
    /// <summary>
    /// Wiki : Twin Cranes.txt — 4 rayons monochromes en éventail ; en même temps, deux carrés près du joueur tirent des balles
    /// rondes (2 volées, ligne droite, 75 %) et des losanges (4 volées de 5, en vagues, 45 %) vers le curseur.
    /// [choix] Une volée de chaque par utilisation (use 4) ; la barrière (touche non nommée sur la page) n'est pas faite.
    /// </summary>
    public class TwinCranes : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 138;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 4;
            Item.useTime = Item.useAnimation = 4;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item12;
            Item.width = 48;
            Item.height = 60;
            Item.rare = ModContent.RarityType<CosmicPurple>();
            Item.value = Item.sellPrice(gold: 25);
            Item.shoot = ModContent.ProjectileType<CraneRay>();
            Item.shootSpeed = 20f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 4; i++)
                Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.Lerp(-0.25f, 0.25f, i / 3f)), type, damage, knockback, player.whoAmI, i % 2);
            Vector2 aim = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX);
            for (int side = -1; side <= 1; side += 2)
            {
                Vector2 square = player.Center + aim.RotatedBy(MathHelper.PiOver2) * 40f * side;
                Projectile.NewProjectile(source, square, (Main.MouseWorld - square).SafeNormalize(aim) * 16f, ModContent.ProjectileType<CraneBullet>(), (int)(damage * 0.75f), knockback, player.whoAmI, 0f);
                Projectile.NewProjectile(source, square, (Main.MouseWorld - square).SafeNormalize(aim) * 12f, ModContent.ProjectileType<CraneBullet>(), (int)(damage * 0.45f), knockback, player.whoAmI, 1f);
            }
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Whipped Bouquet Blade.txt — épée qui lance 4 perles de crème qui explosent (ennemi ou bloc) et font tomber 6 à 8
    /// pétales. [choix] Perles à 80 %, pétales à 30 % ; la mécanique d'effleurement (« Cream points », charges de dessert)
    /// n'est pas faite.
    /// </summary>
    public class WhippedBouquetBlade : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 620;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = Item.height = 96;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
            Item.value = Item.sellPrice(gold: 25);
            Item.shoot = ModContent.ProjectileType<CreamPearl>();
            Item.shootSpeed = 6f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 4; i++)
                Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.Lerp(-0.3f, 0.3f, i / 3f)) * Main.rand.NextFloat(1f, 1.6f), type, (int)(damage * 0.8f), knockback, player.whoAmI);
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Summon
{
    /// <summary>
    /// Wiki : Cirnogenic Staff.txt — une étoile de glace : en sbire, elle fonce sur les ennemis et tourne parfois sur elle-même
    /// en lançant des pics de glace au hasard ; clic droit : passe en sentinelle (immobile, pics bien plus fréquents, rayon
    /// endo qui perce tout de temps en temps) ; Frostburn 1 s. [choix] Pics toutes les 40 images (sbire) / 10 (sentinelle),
    /// rayon toutes les 3 s ; 1 place de sbire.
    /// </summary>
    public class CirnogenicStaff : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 128;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 12;
            Item.useTime = Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.UseSound = SoundID.Item44;
            Item.width = Item.height = 60;
            Item.rare = ModContent.RarityType<CosmicPurple>();
            Item.value = Item.sellPrice(gold: 22);
            Item.buffType = ModContent.BuffType<Buffs.IceStarBuff>();
            Item.shoot = ModContent.ProjectileType<IceStar>();
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                foreach (Projectile p in Main.ActiveProjectiles)
                    if (p.owner == player.whoAmI && p.type == type)
                    {
                        p.ai[0] = p.ai[0] == 1f ? 0f : 1f;
                        p.netUpdate = true;
                    }
                return false;
            }
            player.AddBuff(Item.buffType, 2);
            int s = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI);
            if (Main.projectile.IndexInRange(s))
                Main.projectile[s].originalDamage = Item.damage;
            return false;
        }
    }
}

namespace CalamityFandom.Content.Buffs
{
    public class IceStarBuff : WikiMinionBuff
    {
        public override string Texture => "Terraria/Images/Buff_" + BuffID.Frozen;
        protected override int MinionType => ModContent.ProjectileType<IceStar>();
    }
}

namespace CalamityFandom.Content.Projectiles.Weapons1119v
{
    /// <summary>Rayon monochrome des Twin Cranes (ai[0] = 0 blanc, 1 noir).</summary>
    public class CraneRay : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 50;
            Projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, Projectile.ai[0] == 0f ? DustID.WhiteTorch : DustID.Shadowflame, Vector2.Zero, 100, Projectile.ai[0] == 0f ? Color.White : Color.Black, 1.1f).noGravity = true;
        }
    }

    /// <summary>Balle des carrés : ai[0] = 0 ronde (droite), 1 losange (vagues).</summary>
    public class CraneBullet : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 90;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 1f)
                Projectile.velocity = Projectile.velocity.RotatedBy(0.08f * (float)Math.Sin(++Projectile.localAI[0] * 0.25f));
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, Projectile.ai[0] == 0f ? DustID.SilverCoin : DustID.Smoke, Vector2.Zero, 100, default, 1f).noGravity = true;
        }
    }

    /// <summary>Perle de crème : explose (ennemi ou bloc) et fait tomber 6 à 8 pétales (30 %).</summary>
    public class CreamPearl : ModProjectile
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.WhitePearl;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.1f;
            Projectile.rotation += 0.15f;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14 with { Volume = 0.4f }, Projectile.Center);
            if (Projectile.owner != Main.myPlayer)
                return;
            for (int i = 0; i < Main.rand.Next(6, 9); i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + new Vector2(Main.rand.Next(-60, 61), -200f), new Vector2(Main.rand.NextFloat(-1f, 1f), 6f),
                    ModContent.ProjectileType<CreamPetal>(), (int)(Projectile.damage * 0.3f / 0.8f), 0f, Projectile.owner);
        }
    }

    /// <summary>Pétale de crème : tombe en flottant.</summary>
    public class CreamPetal : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Projectile.velocity.X = (float)Math.Sin(++Projectile.localAI[0] * 0.1f) * 2f;
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.PinkFairy, Vector2.Zero, 100, default, 1f).noGravity = true;
        }
    }

    /// <summary>
    /// Étoile de glace de la Cirnogenic Staff : ai[0] = 0 sbire (fonce, tourne et lance des pics toutes les 40 images), 1
    /// sentinelle (immobile, pics toutes les 10 images, rayon endo toutes les 3 s).
    /// </summary>
    public class IceStar : Weapons1119m.WikiMinion
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Summon/IceStar";
        protected override int BuffType => ModContent.BuffType<Buffs.IceStarBuff>();

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 40;
            Projectile.scale = 0.7f;
        }

        public override bool? CanDamage() => Projectile.ai[0] == 0f ? null : false;

        public override void AI()
        {
            if (!KeepAlive())
                return;
            NPC target = Projectile.Center.MinionHoming(900f, Owner);
            bool sentry = Projectile.ai[0] == 1f;
            Projectile.ai[1]++;
            if (sentry)
                Projectile.velocity *= 0.8f;
            else if (target == null)
                Idle();
            else
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(target.Center) * 12f, 0.06f);
            Projectile.rotation += sentry || Projectile.ai[1] % 120 < 30 ? 0.4f : 0.08f;
            if (target == null || Projectile.owner != Main.myPlayer)
                return;
            int spikeEvery = sentry ? 10 : 40;
            if (Projectile.ai[1] % spikeEvery == 0)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Unit() * 10f, ModContent.ProjectileType<CirnoIceSpike>(), Projectile.damage / 2, 0f, Projectile.owner);
            if (sentry && Projectile.ai[1] % 180 == 0)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Unit() * 16f, ModContent.ProjectileType<EndoBeam>(), Projectile.damage, 0f, Projectile.owner);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Frostburn, 60);
    }

    /// <summary>Pic de glace (Frostburn 1 s).</summary>
    public class CirnoIceSpike : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.FrostShard;

        public override void SetStaticDefaults() => ProjectileID.Sets.MinionShot[Type] = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 80;
        }

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Frostburn, 60);
    }

    /// <summary>Rayon endo : perce tout.</summary>
    public class EndoBeam : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults() => ProjectileID.Sets.MinionShot[Type] = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.IceTorch, Vector2.Zero, 100, default, 1.5f).noGravity = true;
        }
    }
}
