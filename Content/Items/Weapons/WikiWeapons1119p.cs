using System;
using CalamityFandom.Content.Projectiles.Weapons1119p;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

// Lot T (19/11) : objets du wiki fabricables aux premières étapes de l'Aquatic Hypercomputer (recettes du wiki).

namespace CalamityFandom.Content.Items.Weapons.Ranged
{
    /// <summary>
    /// Wiki : Microhavoc.txt — lance-flammes : rayon de Brimstone Flames (4 s) qui perce les ennemis et traverse 2 blocs ;
    /// la cadence ne dépend pas des modificateurs. Amélioration du Spark Spreader. [choix] Utilise du gel comme les
    /// lance-flammes de Terraria.
    /// </summary>
    public class Microhavoc : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 40;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 6;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item34;
            Item.width = 56;
            Item.height = 24;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 1);
            Item.shoot = ModContent.ProjectileType<BrimstoneBeam>();
            Item.shootSpeed = 11f;
            Item.useAmmo = AmmoID.Gel;
        }

        public override float UseSpeedMultiplier(Player player) => 1f;

        public override bool CanConsumeAmmo(Item ammo, Player player) => player.itemAnimation >= player.itemAnimationMax - 1;

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) =>
            type = ModContent.ProjectileType<BrimstoneBeam>();
    }

    /// <summary>Wiki : Frightshooter.txt — salves de 3 fléchettes très rapides qui infligent Brimstone Flames (3 s).</summary>
    public class Frightshooter : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 50;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 4;
            Item.useAnimation = 12;
            Item.reuseDelay = 12;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item98;
            Item.width = 60;
            Item.height = 30;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(gold: 5);
            Item.shoot = ProjectileID.PoisonDartBlowgun;
            Item.shootSpeed = 40f;
            Item.useAmmo = AmmoID.Dart;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int p = Projectile.NewProjectile(source, position, velocity * 0.5f, type, damage, knockback, player.whoAmI);
            if (Main.projectile.IndexInRange(p))
            {
                Main.projectile[p].extraUpdates += 1;
                Main.projectile[p].GetGlobalProjectile<BrimstoneShot>().Active = true;
            }
            return false;
        }
    }

    /// <summary>Tirs qui infligent Brimstone Flames 3 s (Frightshooter).</summary>
    public class BrimstoneShot : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public bool Active;

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Active)
                target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 180);
        }
    }

    /// <summary>
    /// Wiki : Crystallic Coin Gun.txt — tire des pièces qui explosent en éclats de cristal (20 dégâts chacun, toujours) ;
    /// table de la page : cuivre 40 / explosion 10 / 0 éclat, argent 60 / 15 / 2, or 100 / 25 / 5, platine 200 / 50 / 10 ;
    /// Frostburn 5 s (argent), 10 s (or, platine).
    /// </summary>
    public class CrystallicCoinGun : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item11;
            Item.width = 72;
            Item.height = 32;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 5);
            Item.shoot = ProjectileID.CopperCoin;
            Item.shootSpeed = 24f;
            Item.useAmmo = AmmoID.Coin;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int tier = type switch
            {
                ProjectileID.SilverCoin => 1,
                ProjectileID.GoldCoin => 2,
                ProjectileID.PlatinumCoin => 3,
                _ => 0,
            };
            int coinDamage = (int)player.GetTotalDamage(DamageClass.Ranged).ApplyTo(new[] { 40, 60, 100, 200 }[tier]);
            int p = Projectile.NewProjectile(source, position, velocity, type, coinDamage, knockback, player.whoAmI);
            if (Main.projectile.IndexInRange(p))
                Main.projectile[p].GetGlobalProjectile<CrystallicCoin>().Tier = tier + 1;
            return false;
        }
    }

    /// <summary>Pièce du Crystallic Coin Gun (Tier 1 à 4) : explosion et éclats en disparaissant, Frostburn selon la pièce.</summary>
    public class CrystallicCoin : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public int Tier;

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Tier >= 2)
                target.AddBuff(BuffID.Frostburn, Tier == 2 ? 300 : 600);
        }

        public override void OnKill(Projectile projectile, int timeLeft)
        {
            if (Tier <= 0 || projectile.owner != Main.myPlayer)
                return;
            Player owner = Main.player[projectile.owner];
            int explosion = (int)owner.GetTotalDamage(DamageClass.Ranged).ApplyTo(new[] { 10, 15, 25, 50 }[Tier - 1]);
            Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Vector2.Zero, ModContent.ProjectileType<CoinBurst>(), explosion, 0f, projectile.owner);
            int shards = new[] { 0, 2, 5, 10 }[Tier - 1];
            for (int i = 0; i < shards; i++)
                Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Main.rand.NextVector2CircularEdge(8f, 8f), ModContent.ProjectileType<CoinCrystal>(), 20, 0f, projectile.owner);
        }
    }
}

namespace CalamityFandom.Content.Items.Accessories
{
    /// <summary>
    /// Wiki : Landscapist Mobilite Fragment.txt — ruée de 25 blocs qui sème des feuilles (Plague 1 s), seulement en touchant un
    /// bloc et hors des liquides. [choix] Double appui gauche/droite ; 1 s d'attente ; 6 feuilles à 30 dégâts.
    /// </summary>
    public class LandscapistMobiliteFragment : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = Item.height = 32;
            Item.accessory = true;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(gold: 5);
        }

        public override void UpdateAccessory(Player player, bool hideVisual) => player.GetModPlayer<MobiliteDashPlayer>().Land = true;
    }

    /// <summary>
    /// Wiki : Seascapist Mobilite Fragment.txt — ruée de 25 blocs qui crée des vagues (Suffocation 2 s), seulement entièrement
    /// sous l'eau. [choix] Comme la Landscapist.
    /// </summary>
    public class SeascapistMobiliteFragment : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = Item.height = 32;
            Item.accessory = true;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(gold: 5);
        }

        public override void UpdateAccessory(Player player, bool hideVisual) => player.GetModPlayer<MobiliteDashPlayer>().Sea = true;
    }

    /// <summary>Ruées des Mobilite Fragments : double appui à gauche ou à droite, 25 blocs en 20 images.</summary>
    public class MobiliteDashPlayer : ModPlayer
    {
        public bool Land, Sea;
        private int cooldown, dashing, dir;

        public override void ResetEffects()
        {
            Land = Sea = false;
        }

        public override void PreUpdateMovement()
        {
            if (cooldown > 0)
                cooldown--;
            if (dashing > 0)
            {
                dashing--;
                Player.velocity.X = dir * 20f;
                Player.velocity.Y *= 0.5f;
                if (Player.whoAmI == Main.myPlayer && dashing % 4 == 0)
                    Projectile.NewProjectile(Player.GetSource_Misc("MobiliteDash"), Player.Center, new Vector2(-dir * 2f, -1f), ModContent.ProjectileType<MobiliteTrail>(), 30, 0f, Player.whoAmI, Sea ? 1f : 0f);
                return;
            }
            if (cooldown > 0 || Player.whoAmI != Main.myPlayer || !(Land || Sea))
                return;
            int tap = Player.controlRight && Player.releaseRight && Player.doubleTapCardinalTimer[2] < 15 ? 1
                : Player.controlLeft && Player.releaseLeft && Player.doubleTapCardinalTimer[3] < 15 ? -1 : 0;
            if (tap == 0)
                return;
            bool underwater = Collision.DrownCollision(Player.position, Player.width, Player.height, Player.gravDir);
            bool onGround = Player.velocity.Y == 0f && !Player.wet;
            if (!(Land && onGround) && !(Sea && underwater))
                return;
            dir = tap;
            dashing = 20;
            cooldown = 80;
            SoundEngine.PlaySound(SoundID.Item24, Player.Center);
        }
    }

    /// <summary>
    /// Wiki : Shatterspectre Tracers.txt — 10 défense, +2 PV/s, +10 % de vitesse, course très rapide (30 mph), toutes les
    /// attaques infligent Crush Depth, une fée tire des pétales chercheurs. Amélioration des Fairy Boots et des Hellfire
    /// Treads. [choix] Crush Depth 2 s ; fée : 25 dégâts toutes les 40 images. « Water and ice debuffs are stronger » n'est
    /// pas fait (pas de règle commune pour ces debuffs).
    /// </summary>
    public class ShatterspectreTracers : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 40;
            Item.accessory = true;
            Item.defense = 10;
            Item.rare = ItemRarityID.Lime;
            Item.value = Item.sellPrice(gold: 20);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.lifeRegen += 4;
            player.moveSpeed += 0.1f;
            player.accRunSpeed = Math.Max(player.accRunSpeed, 6.75f);
            player.GetModPlayer<ShatterspectrePlayer>().Equipped = true;
            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<SpectreFairy>()] < 1)
                Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<SpectreFairy>(), 25, 0f, player.whoAmI);
        }
    }

    public class ShatterspectrePlayer : ModPlayer
    {
        public bool Equipped;

        public override void ResetEffects() => Equipped = false;

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Equipped)
                target.AddBuff(ModContent.BuffType<CrushDepth>(), 120);
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Equipped)
                target.AddBuff(ModContent.BuffType<CrushDepth>(), 120);
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Weapons1119p
{
    /// <summary>Rayon de Brimstone Flames du Microhavoc : perce les ennemis, traverse 2 blocs.</summary>
    public class BrimstoneBeam : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private int tilesCrossed;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Point t = Projectile.Center.ToTileCoordinates();
            if (WorldGen.InWorld(t.X, t.Y) && WorldGen.SolidTile(t.X, t.Y) && Projectile.localAI[0] != t.X * 10000 + t.Y)
            {
                Projectile.localAI[0] = t.X * 10000 + t.Y;
                if (++tilesCrossed > 2)
                {
                    Projectile.Kill();
                    return;
                }
            }
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.RedTorch, Projectile.velocity * 0.1f, 100, default, 1.6f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 240);
    }

    /// <summary>Explosion de pièce du Crystallic Coin Gun.</summary>
    public class CoinBurst : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 4;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (!Main.dedServ)
                for (int i = 0; i < 10; i++)
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.IceTorch, 0f, 0f, 100, default, 1.3f).velocity *= 1.5f;
        }
    }

    /// <summary>Éclat de cristal d'une pièce : 20 dégâts, toujours.</summary>
    public class CoinCrystal : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.CrystalShard;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Default;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 60;
        }

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
    }

    /// <summary>Traînée des Mobilite Fragments : ai[0] = 0 feuilles (Plague 1 s), 1 vague (Suffocation 2 s).</summary>
    public class MobiliteTrail : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 40;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.95f;
            if (!Main.dedServ)
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, Projectile.ai[0] == 1f ? DustID.Water : DustID.GrassBlades, 0f, 0f, 100, default, 1.2f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[0] == 1f)
                target.AddBuff(BuffID.Suffocation, 120);
            else
                target.AddBuff(ModContent.BuffType<Plague>(), 60);
        }
    }

    /// <summary>Fée des Shatterspectre Tracers : suit le joueur, tire un pétale chercheur toutes les 40 images.</summary>
    public class SpectreFairy : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.FairyQueenMagicItemShot;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 2;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!owner.active || owner.dead || !owner.GetModPlayer<Items.Accessories.ShatterspectrePlayer>().Equipped)
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 2;
            Projectile.Center = Vector2.Lerp(Projectile.Center, owner.Center + new Vector2(-owner.direction * 30f, -40f + 6f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 3f)), 0.2f);
            if (!Main.dedServ && Main.rand.NextBool(3))
                Dust.NewDustPerfect(Projectile.Center, DustID.PinkFairy, Vector2.Zero, 100, default, 0.8f).noGravity = true;
            NPC target = Projectile.Center.ClosestNPCAt(600f);
            if (target != null && Projectile.owner == Main.myPlayer && ++Projectile.ai[0] % 40f == 0f)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.SafeDirectionTo(target.Center) * 10f, ModContent.ProjectileType<FairyPetal>(), Projectile.damage, 0f, Projectile.owner);
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }

    /// <summary>Pétale chercheur de la fée.</summary>
    public class FairyPetal : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            CalamityUtils.HomeInOnNPC(Projectile, true, 600f, 10f, 15f);
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.PinkTorch, Vector2.Zero, 100, default, 1.1f).noGravity = true;
        }
    }
}
