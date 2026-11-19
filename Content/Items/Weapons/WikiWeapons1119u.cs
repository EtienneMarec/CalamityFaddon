using System;
using CalamityFandom.Content.Projectiles.Weapons1119u;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

// Lot Y (19/11) : armes du wiki fabricables à l'Ancient Manipulator et à la Cosmic Anvil (recettes du wiki).
// « Moon Burnt » = Nightwither.

namespace CalamityFandom.Content.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Rox.txt — clic gauche : une volée irrégulière d'éclats de roche (33 % rebondissent 5 fois, 10 % éclatent en 8) ;
    /// clic droit : le joueur plonge vers le bas, invincible, et une explosion de rochers part du point d'impact (10 % chacun).
    /// [choix] 3 éclats par coup ; 12 rochers à l'impact ; invincible pendant la plongée.
    /// </summary>
    public class Rox : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 300;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 8;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 13f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = 60;
            Item.height = 72;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.value = 1;
            Item.shoot = ModContent.ProjectileType<RoxShard>();
            Item.shootSpeed = 20f;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                var mp = player.GetModPlayer<RoxPlayer>();
                if (mp.Pounding || player.velocity.Y == 0f)
                    return false;
                mp.Pounding = true;
                Item.useTime = Item.useAnimation = 20;
                Item.shoot = ProjectileID.None;
                return true;
            }
            Item.useTime = Item.useAnimation = 8;
            Item.shoot = ModContent.ProjectileType<RoxShard>();
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 3; i++)
                Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.2f) * Main.rand.NextFloat(0.85f, 1.1f), type, damage, knockback, player.whoAmI,
                    Main.rand.NextBool(3) ? 5f : 0f, Main.rand.NextBool(10) ? 1f : 0f);
            return false;
        }
    }

    /// <summary>Plongée de Rox : vers le bas, invincible ; à l'impact, 12 rochers (10 %).</summary>
    public class RoxPlayer : ModPlayer
    {
        public bool Pounding;

        public override void PreUpdateMovement()
        {
            if (!Pounding)
                return;
            Player.velocity.Y = 24f;
            Player.velocity.X *= 0.9f;
            Player.immune = true;
            Player.immuneTime = Math.Max(Player.immuneTime, 2);
            if (Player.velocity.Y != 0f && !Collision.SolidCollision(Player.BottomLeft, Player.width, 8))
                return;
            Pounding = false;
            SoundEngine.PlaySound(SoundID.Item62, Player.Bottom);
            if (Player.whoAmI != Main.myPlayer)
                return;
            int damage = (int)Player.GetTotalDamage(DamageClass.Melee).ApplyTo(300 * 0.1f);
            for (int i = 0; i < 12; i++)
                Projectile.NewProjectile(Player.GetSource_Misc("RoxPound"), Player.Bottom - Vector2.UnitY * 8f, new Vector2(Main.rand.NextFloat(-9f, 9f), Main.rand.NextFloat(-12f, -4f)),
                    ModContent.ProjectileType<RoxShard>(), damage, 4f, Player.whoAmI);
        }
    }

    /// <summary>
    /// Wiki : The Archalis.txt — en frappant, des entailles floues apparaissent vers le curseur (Moon Burnt 3 s) ; 10 % de
    /// chances qu'un caracal tombe du ciel sur l'ennemi touché, saute sur les ennemis puis s'évanouit. [choix] Une entaille par
    /// coup (100 %) ; caracal de 4 s (30 %).
    /// </summary>
    public class TheArchalis : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 512;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = 84;
            Item.height = 86;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.value = Item.sellPrice(gold: 24);
            Item.shoot = ModContent.ProjectileType<ArchalisSlash>();
            Item.shootSpeed = 24f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 dir = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX * player.direction);
            Projectile.NewProjectile(source, player.Center + dir * 60f, dir * 4f, type, damage, knockback, player.whoAmI);
            return false;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Nightwither>(), 180);
    }

    /// <summary>
    /// Wiki : Tenebris Tides.txt — bâton tenu qui tourne de plus en plus vite au-dessus du joueur ; toutes les 20 images une
    /// lance part de 100 blocs sous le curseur et cherche les ennemis ; chaque coup du bâton fait venir 3 lances de tous côtés
    /// sur l'ennemi (33 %) ; Crush Depth 5 s. [choix] Lances depuis 600 px sous le curseur (100 blocs : hors de vue et trop loin).
    /// </summary>
    public class TenebrisTides : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 160;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 23;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = Item.noUseGraphic = true;
            Item.channel = true;
            Item.knockBack = 6f;
            Item.UseSound = SoundID.Item1;
            Item.width = 110;
            Item.height = 108;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.sellPrice(gold: 32);
            Item.shoot = ModContent.ProjectileType<TenebrisBaton>();
            Item.shootSpeed = 8f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;
    }
}

namespace CalamityFandom.Content.Items.Weapons.Ranged
{
    /// <summary>
    /// Wiki : Ribbon Flintlock Rifle Cannon.txt — balles dorées de sniper, 50 % de ne pas consommer ; clic droit : toutes les
    /// balles en vol s'arrêtent, lancent un ruban (200 %) vers l'ennemi le plus proche et foncent vers lui.
    /// </summary>
    public class RibbonFlintlockRifleCannon : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 134;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item40;
            Item.width = 92;
            Item.height = 40;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.value = Item.sellPrice(gold: 12);
            Item.shoot = ModContent.ProjectileType<RibbonBullet>();
            Item.shootSpeed = 9f;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-14f, 0f);

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanConsumeAmmo(Item ammo, Player player) => player.altFunctionUse != 2 && Main.rand.NextBool();

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                foreach (Projectile p in Main.ActiveProjectiles)
                    if (p.owner == player.whoAmI && p.type == ModContent.ProjectileType<RibbonBullet>() && p.ai[0] == 0f)
                    {
                        p.ai[0] = 1f;
                        p.netUpdate = true;
                    }
                return false;
            }
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<RibbonBullet>(), damage, knockback, player.whoAmI);
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Summon
{
    /// <summary>
    /// Wiki : Planterror Staff.txt — un mini Guntera (4 places, un seul) qui change de mode toutes les 10 s : poursuite et
    /// gerbes de 8 balles chaque seconde ; tourne autour de la cible avec un flot de balles faibles et rapides ; au-dessus du
    /// joueur, tir de sniper toutes les 2 s. Il utilise la première munition du joueur à l'invocation, sans la consommer.
    /// [choix] Flot : 25 %, une balle toutes les 6 images ; sniper : x3.
    /// </summary>
    public class PlanterrorStaff : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 325;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 12;
            Item.useTime = Item.useAnimation = 19;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.UseSound = SoundID.Item44;
            Item.width = 48;
            Item.height = 50;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(platinum: 1, gold: 32);
            Item.buffType = ModContent.BuffType<Buffs.GunteraBuff>();
            Item.shoot = ModContent.ProjectileType<MiniGuntera>();
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            Item bullet = player.ChooseAmmo(ContentSamples.ItemsByType[ItemID.Minishark]);
            int shot = bullet != null && !bullet.IsAir && bullet.shoot > ProjectileID.None ? bullet.shoot : ProjectileID.Bullet;
            int p = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI, 0f, shot);
            if (Main.projectile.IndexInRange(p))
                Main.projectile[p].originalDamage = Item.damage;
            return false;
        }
    }
}

namespace CalamityFandom.Content.Buffs
{
    public class GunteraBuff : WikiMinionBuff
    {
        protected override int MinionType => ModContent.ProjectileType<MiniGuntera>();
    }
}

namespace CalamityFandom.Content.Projectiles.Weapons1119u
{
    /// <summary>Éclat de Rox : gravité ; ai[0] = rebonds restants ; ai[1] = 1 : éclate en 8 au contact.</summary>
    public class RoxShard : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Melee/RoxShard";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.scale = 0.6f;
        }

        public override void AI()
        {
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.2f, 14f);
            Projectile.rotation += 0.2f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.ai[0] <= 0f)
                return true;
            Projectile.ai[0]--;
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X * 0.8f;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y * 0.8f;
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.ai[1] != 1f || Projectile.owner != Main.myPlayer)
                return;
            for (int i = 0; i < 8; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (MathHelper.TwoPi * i / 8f).ToRotationVector2() * 7f, Type, Projectile.damage / 2, 0f, Projectile.owner);
        }
    }

    /// <summary>Entaille floue de The Archalis : courte, 3 ennemis ; 10 % de chances d'appeler un caracal sur l'ennemi touché.</summary>
    public class ArchalisSlash : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 70;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 12;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            if (!Main.dedServ)
                for (int i = 0; i < 4; i++)
                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(30f, 30f), DustID.WhiteTorch, Projectile.velocity, 120, default, 1.2f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Nightwither>(), 180);
            if (Projectile.owner == Main.myPlayer && Main.rand.NextBool(10))
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center - new Vector2(0f, 500f), new Vector2(0f, 14f), ModContent.ProjectileType<Caracal>(),
                    (int)(Projectile.damage * 0.3f), 2f, Projectile.owner);
        }
    }

    /// <summary>Caracal : tombe du ciel, rebondit sur les ennemis proches 4 s (comme le Slime Staff) puis s'évanouit.</summary>
    public class Caracal : ModProjectile
    {
        public override string Texture => "Terraria/Images/NPC_" + NPCID.Bunny;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 240;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.4f, 14f);
            Projectile.spriteDirection = Projectile.velocity.X > 0 ? 1 : -1;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            NPC target = Projectile.Center.ClosestNPCAt(400f);
            float dx = target != null ? Math.Clamp((target.Center.X - Projectile.Center.X) / 30f, -8f, 8f) : Main.rand.NextFloat(-4f, 4f);
            Projectile.velocity = new Vector2(dx, -9f);
            return false;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(210, 160, 90).MultiplyRGB(lightColor);

        public override bool PreDraw(ref Color lightColor)
        {
            var tex = Terraria.GameContent.TextureAssets.Npc[NPCID.Bunny].Value;
            int frameHeight = tex.Height / Main.npcFrameCount[NPCID.Bunny];
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, tex.Width, frameHeight), Projectile.GetAlpha(lightColor), 0f,
                new Vector2(tex.Width / 2f, frameHeight / 2f), 1f, Projectile.spriteDirection == 1 ? Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally : Microsoft.Xna.Framework.Graphics.SpriteEffects.None);
            return false;
        }
    }

    /// <summary>Bâton de Tenebris Tides : tenu, tourne au-dessus du joueur ; une lance sous le curseur toutes les 20 images ; 3 lances sur l'ennemi touché.</summary>
    public class TenebrisBaton : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Melee/TenebrisTides";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.scale = 0.8f;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!owner.channel || owner.dead || owner.HeldItem.type != ModContent.ItemType<Items.Weapons.Melee.TenebrisTides>())
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 2;
            owner.itemTime = owner.itemAnimation = 2;
            owner.heldProj = Projectile.whoAmI;
            Projectile.ai[0] = Math.Min(Projectile.ai[0] + 0.005f, 0.6f);
            Projectile.rotation += 0.15f + Projectile.ai[0];
            Projectile.Center = owner.MountedCenter + new Vector2(0f, -40f);
            if (Projectile.owner == Main.myPlayer && ++Projectile.ai[1] % 20 == 0)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Main.MouseWorld + new Vector2(Main.rand.Next(-60, 61), 600f), new Vector2(0f, -16f),
                    ModContent.ProjectileType<TenebrisSpear>(), (int)(Projectile.damage * 0.33f), 2f, Projectile.owner);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 300);
            if (Projectile.owner != Main.myPlayer)
                return;
            for (int i = 0; i < 3; i++)
            {
                Vector2 from = target.Center + Main.rand.NextVector2CircularEdge(1f, 1f) * 400f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), from, (target.Center - from).SafeNormalize(Vector2.UnitY) * 18f, ModContent.ProjectileType<TenebrisSpear>(),
                    (int)(Projectile.damage * 0.33f), 2f, Projectile.owner);
            }
        }
    }

    /// <summary>Lance ténébreuse : cherche les ennemis, Crush Depth 5 s.</summary>
    public class TenebrisSpear : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.UnholyTridentFriendly;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (++Projectile.localAI[0] > 10f)
                CalamityUtils.HomeInOnNPC(Projectile, true, 800f, 16f, 15f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<CrushDepth>(), 300);
    }

    /// <summary>Balle du Ribbon Flintlock : ai[0] = 1 : s'arrête, lance un ruban (200 %) vers l'ennemi le plus proche et fonce vers lui.</summary>
    public class RibbonBullet : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 3;
        }

        public override void AI()
        {
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.GoldFlame, Vector2.Zero, 100, default, 1f).noGravity = true;
            if (Projectile.ai[0] != 1f)
                return;
            Projectile.ai[0] = 2f;
            NPC target = Projectile.Center.ClosestNPCAt(1200f);
            if (target == null)
                return;
            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.SafeDirectionTo(target.Center) * 20f, ModContent.ProjectileType<Ribbon>(), Projectile.damage * 2, 0f, Projectile.owner);
            Projectile.velocity = Projectile.SafeDirectionTo(target.Center) * 12f;
            Projectile.netUpdate = true;
        }
    }

    /// <summary>Ruban doré : rapide, perce 3 ennemis.</summary>
    public class Ribbon : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 90;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.YellowTorch, Vector2.Zero, 100, default, 1.4f).noGravity = true;
        }
    }

    /// <summary>
    /// Mini Guntera : 4 places ; change de mode toutes les 10 s : poursuite + gerbe de 8 balles par seconde ; tour autour de
    /// la cible + flot (25 %, toutes les 6 images) ; au-dessus du joueur, sniper (x3) toutes les 2 s. ai[1] = projectile de la munition.
    /// </summary>
    public class MiniGuntera : WikiMinionBase
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Summon/MiniGuntera";
        protected override int BuffType => ModContent.BuffType<Buffs.GunteraBuff>();
        protected override float Slots => 4f;

        public override bool? CanDamage() => false;

        public override void AI()
        {
            if (!KeepAlive())
                return;
            NPC target = Projectile.Center.MinionHoming(1200f, Owner);
            if (target == null)
            {
                Idle();
                return;
            }
            Projectile.localAI[0]++;
            int mode = (int)(Projectile.localAI[0] / 600) % 3;
            int shot = Projectile.ai[1] > 0 ? (int)Projectile.ai[1] : ProjectileID.Bullet;
            Projectile.rotation = Projectile.SafeDirectionTo(target.Center).ToRotation();
            var source = Projectile.GetSource_FromThis();
            switch (mode)
            {
                case 0:
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(target.Center) * 10f, 0.05f);
                    if (Projectile.owner == Main.myPlayer && Projectile.localAI[0] % 60 == 0)
                        for (int i = 0; i < 8; i++)
                            Fire(source, Projectile.SafeDirectionTo(target.Center).RotatedByRandom(0.15f) * 14f, shot, Projectile.damage);
                    break;
                case 1:
                    float angle = Projectile.localAI[0] * 0.05f;
                    Projectile.Center = Vector2.Lerp(Projectile.Center, target.Center + angle.ToRotationVector2() * 220f, 0.15f);
                    Projectile.velocity = Vector2.Zero;
                    if (Projectile.owner == Main.myPlayer && Projectile.localAI[0] % 6 == 0)
                        Fire(source, Projectile.SafeDirectionTo(target.Center) * 20f, shot, Projectile.damage / 4);
                    break;
                default:
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, (Owner.Center + new Vector2(0f, -90f) - Projectile.Center) * 0.1f, 0.2f);
                    if (Projectile.owner == Main.myPlayer && Projectile.localAI[0] % 120 == 0)
                        Fire(source, Projectile.SafeDirectionTo(target.Center) * 24f, shot, Projectile.damage * 3);
                    break;
            }
        }

        private void Fire(IEntitySource source, Vector2 velocity, int type, int damage)
        {
            int p = Projectile.NewProjectile(source, Projectile.Center, velocity, type, damage, Projectile.knockBack, Projectile.owner);
            if (Main.projectile.IndexInRange(p))
                Main.projectile[p].DamageType = DamageClass.Summon;
        }
    }

    /// <summary>Base de sbire (copie de celle du lot Q, dans ce namespace).</summary>
    public abstract class WikiMinionBase : Weapons1119m.WikiMinion
    {
    }
}
