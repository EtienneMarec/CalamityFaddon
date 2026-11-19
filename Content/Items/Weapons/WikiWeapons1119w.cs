using System;
using CalamityFandom.Content.Projectiles.Weapons1119w;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

// Lot AA (19/11) : armes du wiki déjà obtenables (recette du wiki ou butin d'un ennemi du jeu).

namespace CalamityFandom.Content.Items.Weapons.Ranged
{
    /// <summary>
    /// Wiki : Phototerminator.txt — lance-flammes (gel, 95 % de ne pas en utiliser) : rayon qui traverse 5 blocs (Nightclear
    /// 5 s) ; clic droit : étincelles chercheuses tirées en tous sens (90°, 10 %) ; cadence fixe.
    /// </summary>
    public class Phototerminator : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 888;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 6;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item34;
            Item.width = 96;
            Item.height = 36;
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(platinum: 50);
            Item.shoot = ModContent.ProjectileType<PhotoBeam>();
            Item.shootSpeed = 18f;
            Item.useAmmo = AmmoID.Gel;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override float UseSpeedMultiplier(Player player) => 1f;

        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.Next(100) >= 95;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
                Projectile.NewProjectile(source, position, velocity.RotatedByRandom(MathHelper.PiOver4) * 0.6f, ModContent.ProjectileType<PhotoSpark>(), damage / 10, 0f, player.whoAmI);
            else
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<PhotoBeam>(), damage, knockback, player.whoAmI);
            return false;
        }
    }

    /// <summary>
    /// Wiki : Sandstorm (weapon).txt — utilise des blocs de sable (50 % de ne pas en consommer) ; tirs de sable qui éclatent et
    /// ralentissent ; ne pose pas de sable. Amélioration du Sandgun. [choix] Explosion de 50 px à 50 %, Slow 2 s.
    /// </summary>
    public class Sandstorm : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 80;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item11;
            Item.width = 62;
            Item.height = 26;
            Item.rare = ItemRarityID.Lime;
            Item.value = Item.sellPrice(gold: 12);
            Item.shoot = ModContent.ProjectileType<SandBlast>();
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.Sand;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.NextBool();

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) =>
            type = ModContent.ProjectileType<SandBlast>();
    }
}

namespace CalamityFandom.Content.Items.Weapons.Summon
{
    /// <summary>
    /// Wiki : Morpho Doomblade.txt — épée d'invocation qui lance des entailles en croissant ; clic droit (100 mana) : un Arbiter
    /// of Judgement (aucune place) qui frappe et tire des croissants puis revient ; tout inflige Whispering Death et Valfrey
    /// Burn 2 s (200 PV/s, -5 % de réduction des dégâts). Amélioration du Wooden Butterfly Staff. [choix] Un arbitre à la fois, 20 s.
    /// </summary>
    public class MorphoDoomblade : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 1430;
            Item.DamageType = DamageClass.Summon;
            Item.useTime = Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 2.5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item71;
            Item.width = Item.height = 82;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
            Item.value = Item.sellPrice(gold: 30);
            Item.shoot = ModContent.ProjectileType<MorphoCrescent>();
            Item.shootSpeed = 12f;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse != 2)
            {
                Item.mana = 0;
                return true;
            }
            Item.mana = 100;
            return player.ownedProjectileCounts[ModContent.ProjectileType<ArbiterofJudgement>()] < 1;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<ArbiterofJudgement>(), damage, knockback, player.whoAmI);
                return false;
            }
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) => MorphoDebuffs.Apply(target);
    }

    /// <summary>
    /// Wiki : Prism Destroyer.txt — une épée prismatique (sbire) reste à distance et tire des étoiles chercheuses en dépensant
    /// du mana (rien sans mana) ; clic droit : l'épée revient en main et part en lancer (mêlée/voleur) ; frappe furtive au lancer :
    /// 20 flèches du joueur foncent vers l'ennemi touché (200 %) ; Daybroken 5 s au contact, 2,5 s pour les tirs.
    /// [choix] 1 place ; 3 étoiles toutes les 40 images pour 4 mana ; lancer : épée qui revient ; flèches = Wooden Arrows (pas
    /// prises dans l'inventaire).
    /// </summary>
    public class PrismDestroyer : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 528;
            Item.DamageType = DamageClass.Summon;
            Item.useTime = Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = Item.noUseGraphic = true;
            Item.knockBack = 8f;
            Item.UseSound = SoundID.Item44;
            Item.width = Item.height = 52;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
            Item.value = Item.sellPrice(gold: 32);
            Item.buffType = ModContent.BuffType<Buffs.PrismSwordBuff>();
            Item.shoot = ModContent.ProjectileType<PrismSword>();
            Item.shootSpeed = 16f;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player) => player.altFunctionUse == 2 || player.ownedProjectileCounts[Item.shoot] < 1;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                foreach (Projectile p in Main.ActiveProjectiles)
                    if (p.owner == player.whoAmI && p.type == type)
                        p.Kill();
                bool stealth = player.Calamity().StealthStrikeAvailable();
                int t = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<ThrownPrismSword>(), damage, knockback, player.whoAmI);
                if (stealth && Main.projectile.IndexInRange(t))
                    Main.projectile[t].Calamity().stealthStrike = true;
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
    public class PrismSwordBuff : WikiMinionBuff
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Summon/PrismDestroyer";
        protected override int MinionType => ModContent.ProjectileType<PrismSword>();
    }

    /// <summary>Valfrey Burn (Morpho Doomblade) : 200 PV/s, -5 % de réduction des dégâts (coups +5 %).</summary>
    public class ValfreyBurn : ModBuff
    {
        public override void SetStaticDefaults() => Main.debuff[Type] = true;

        public override void Update(NPC npc, ref int buffIndex) => npc.GetGlobalNPC<ValfreyBurnNPC>().Burning = true;
    }

    public class ValfreyBurnNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public bool Burning;

        public override void ResetEffects(NPC npc) => Burning = false;

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (!Burning)
                return;
            if (npc.lifeRegen > 0)
                npc.lifeRegen = 0;
            npc.lifeRegen -= 400;
            damage = Math.Max(damage, 40);
        }

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (Burning)
                modifiers.FinalDamage *= 1.05f;
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Weapons1119w
{
    internal static class MorphoDebuffs
    {
        public static void Apply(NPC target)
        {
            target.AddBuff(ModContent.BuffType<WhisperingDeath>(), 120);
            target.AddBuff(ModContent.BuffType<Buffs.ValfreyBurn>(), 120);
        }
    }

    /// <summary>Rayon du Phototerminator : traverse 5 blocs, perce les ennemis, Holy Flames 5 s.</summary>
    public class PhotoBeam : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private int tiles;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 40;
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
                if (++tiles > 5)
                {
                    Projectile.Kill();
                    return;
                }
            }
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.WhiteTorch, Projectile.velocity * 0.1f, 100, new Color(255, 250, 220), 1.6f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Buffs.Nightclear>(), 300);
    }

    /// <summary>Étincelle du Phototerminator : chercheuse.</summary>
    public class PhotoSpark : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 90;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            CalamityUtils.HomeInOnNPC(Projectile, true, 600f, 12f, 15f);
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.YellowTorch, Vector2.Zero, 100, default, 1.1f).noGravity = true;
        }
    }

    /// <summary>Tir de sable du Sandstorm : éclate en 50 px (50 %), Slow 2 s ; ne pose pas de sable.</summary>
    public class SandBlast : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.SandBallGun;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.1f, 12f);
            Projectile.rotation += 0.2f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Slow, 120);

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            Projectile.ExpandHitboxBy(50);
            Projectile.maxPenetrate = Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            for (int i = 0; i < Projectile.localNPCImmunity.Length; i++)
                Projectile.localNPCImmunity[i] = 0;
            Projectile.damage /= 2;
            Projectile.Damage();
            if (!Main.dedServ)
                for (int i = 0; i < 12; i++)
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Sand, 0f, 0f, 100, default, 1.3f).velocity *= 1.5f;
        }
    }

    /// <summary>Croissant de la Morpho Doomblade : 3 ennemis, debuffs Morpho.</summary>
    public class MorphoCrescent : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            if (!Main.dedServ)
                for (int i = 0; i < 2; i++)
                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(14f, 14f), DustID.BlueTorch, Vector2.Zero, 100, default, 1.3f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => MorphoDebuffs.Apply(target);
    }

    /// <summary>Arbiter of Judgement : 20 s, aucune place ; fonce sur l'ennemi le plus proche, tire un croissant toutes les 30 images, revient près du joueur.</summary>
    public class ArbiterofJudgement : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Summon/ArbiterofJudgement";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 1200;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.scale = 0.6f;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            NPC target = Projectile.Center.MinionHoming(900f, owner);
            if (target == null)
            {
                Vector2 home = owner.Center + new Vector2(owner.direction * -60f, -60f);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, (home - Projectile.Center) * 0.1f, 0.2f);
            }
            else
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(target.Center) * 16f, 0.08f);
                if (Projectile.owner == Main.myPlayer && ++Projectile.ai[0] % 30 == 0)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.SafeDirectionTo(target.Center) * 14f, ModContent.ProjectileType<MorphoCrescent>(),
                        Projectile.damage / 2, 2f, Projectile.owner);
            }
            Projectile.spriteDirection = Projectile.velocity.X > 0 ? 1 : -1;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => MorphoDebuffs.Apply(target);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.White * 0.9f, 0f, tex.Size() / 2f, Projectile.scale,
                Projectile.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            return false;
        }
    }

    /// <summary>Épée prismatique (sbire) : reste à distance, 3 étoiles chercheuses toutes les 40 images pour 4 mana.</summary>
    public class PrismSword : Weapons1119m.WikiMinion
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Summon/PrismDestroyer";
        protected override int BuffType => ModContent.BuffType<Buffs.PrismSwordBuff>();

        public override bool? CanDamage() => false;

        public override void AI()
        {
            if (!KeepAlive())
                return;
            NPC target = Projectile.Center.MinionHoming(1000f, Owner);
            if (target == null)
            {
                Idle();
                Projectile.rotation = 0f;
                return;
            }
            Vector2 keep = target.Center + target.SafeDirectionTo(Owner.Center) * 260f;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, (keep - Projectile.Center) * 0.08f, 0.2f);
            Projectile.rotation = Projectile.SafeDirectionTo(target.Center).ToRotation() + MathHelper.PiOver4;
            if (Projectile.owner == Main.myPlayer && ++Projectile.ai[0] % 40 == 0 && Owner.CheckMana(4, true))
                for (int i = -1; i <= 1; i++)
                {
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.SafeDirectionTo(target.Center).RotatedBy(0.3f * i) * 12f,
                        ModContent.ProjectileType<PrismStar>(), Projectile.damage / 2, 0f, Projectile.owner);
                }
        }
    }

    /// <summary>Étoile prismatique : chercheuse, Daybroken 2,5 s.</summary>
    public class PrismStar : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.HallowStar;

        public override void SetStaticDefaults() => ProjectileID.Sets.MinionShot[Type] = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            CalamityUtils.HomeInOnNPC(Projectile, true, 900f, 14f, 15f);
            Projectile.rotation += 0.2f;
        }

        public override Color? GetAlpha(Color lightColor) => Main.DiscoColor with { A = 200 };

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Daybroken>(), 150);
    }

    /// <summary>Épée lancée : revient ; Daybroken 5 s ; furtive : 20 flèches foncent vers l'ennemi touché (200 %).</summary>
    public class ThrownPrismSword : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Summon/PrismDestroyer";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void AI()
        {
            Projectile.rotation += 0.4f;
            if (++Projectile.ai[0] > 30f)
            {
                Player owner = Main.player[Projectile.owner];
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(owner.Center) * 20f, 0.12f);
                if (Projectile.Distance(owner.Center) < 40f)
                    Projectile.Kill();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Daybroken>(), 300);
            if (!Projectile.Calamity().stealthStrike || Projectile.owner != Main.myPlayer || Projectile.localAI[0] == 1f)
                return;
            Projectile.localAI[0] = 1f;
            for (int i = 0; i < 20; i++)
            {
                Vector2 from = target.Center + Main.rand.NextVector2CircularEdge(1f, 1f) * 500f;
                int a = Projectile.NewProjectile(Projectile.GetSource_FromThis(), from, (target.Center - from).SafeNormalize(Vector2.UnitY) * 20f, ProjectileID.WoodenArrowFriendly,
                    Projectile.damage * 2, 0f, Projectile.owner);
                if (Main.projectile.IndexInRange(a))
                {
                    Main.projectile[a].tileCollide = false;
                    Main.projectile[a].DamageType = DamageClass.Ranged;
                }
            }
        }
    }
}
