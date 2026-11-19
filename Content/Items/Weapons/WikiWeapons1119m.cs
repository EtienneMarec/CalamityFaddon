using System;
using CalamityFandom.Content.Projectiles.Weapons1119m;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

// Lot Q (19/11) : armes du wiki fabricables à l'enclume (recettes du wiki), pages d'addon comprises (section 11).

namespace CalamityFandom.Content.Items.Weapons.Summon
{
    /// <summary>Bâton d'invocation du lot Q : buff, sbire posé au curseur.</summary>
    public abstract class WikiMinionStaff : ModItem
    {
        protected abstract int Damage { get; }
        protected abstract int Rarity { get; }
        protected abstract int SellCopper { get; }
        protected abstract int BuffType { get; }
        protected abstract int MinionType { get; }
        protected virtual float Knockback => 2f;
        protected virtual int UseTime => 20;

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.GamepadWholeScreenUseRange[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = Damage;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 10;
            Item.useTime = Item.useAnimation = UseTime;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = Knockback;
            Item.UseSound = SoundID.Item44;
            Item.width = Item.height = 46;
            Item.rare = Rarity;
            Item.value = SellCopper * 5;
            Item.buffType = BuffType;
            Item.shoot = MinionType;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            int p = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI);
            if (Main.projectile.IndexInRange(p))
                Main.projectile[p].originalDamage = Item.damage;
            return false;
        }
    }

    /// <summary>Wiki : Fracture Staff.txt — un Fractal Crawler qui saute sur les ennemis et fait tomber des cristaux (20 %) à chaque coup.</summary>
    public class FractureStaff : WikiMinionStaff
    {
        protected override int Damage => 43;
        protected override int Rarity => ItemRarityID.Pink;
        protected override int SellCopper => 24000;
        protected override int BuffType => ModContent.BuffType<Buffs.FractalCrawlerBuff>();
        protected override int MinionType => ModContent.ProjectileType<FractalCrawler>();
        protected override float Knockback => 5f;
        protected override int UseTime => 18;
    }

    /// <summary>
    /// Wiki : Holy Mackerel Staff.txt — un Holy Mackerel qui fonce sur les ennemis et les frappe vite (Holy Flames) ; une
    /// demi-place de sbire. Amélioration du Herring Staff. [choix] Holy Flames 2 s ; ne vise pas hors de l'écran (page).
    /// </summary>
    public class HolyMackerelStaff : WikiMinionStaff
    {
        protected override int Damage => 46;
        protected override int Rarity => ItemRarityID.Pink;
        protected override int SellCopper => 10000;
        protected override int BuffType => ModContent.BuffType<Buffs.HolyMackerelBuff>();
        protected override int MinionType => ModContent.ProjectileType<HolyMackerel>();
        protected override float Knockback => 1.2f;
        protected override int UseTime => 16;
    }

    /// <summary>
    /// Wiki : Infernum Kris.txt — un humain déchu armé de couteaux, surtout immobile, qui lance vite des couteaux ; si sa cible
    /// s'éloigne trop, il se téléporte au-dessus d'elle et frappe vers le bas en lâchant 8 couteaux en cercle ; 2 places de
    /// sbire, un seul à la fois. [choix] Couteau toutes les 12 images à 600 px ; téléportation au-delà de 600 px.
    /// </summary>
    public class InfernumKris : WikiMinionStaff
    {
        protected override int Damage => 62;
        protected override int Rarity => ItemRarityID.Pink;
        protected override int SellCopper => 20000;
        protected override int BuffType => ModContent.BuffType<Buffs.InfernumKrisBuff>();
        protected override int MinionType => ModContent.ProjectileType<FallenHuman>();
        protected override float Knockback => 2.5f;

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;
    }
}

namespace CalamityFandom.Content.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : The First Fracturer.txt — épée courte qui se répète, tire des rayons bleu clair ; un rayon qui touche fait venir
    /// 3 à 5 rayons de plus de hors de l'écran sur l'ennemi. [technique] Estoc à l'ancienne (Thrust).
    /// </summary>
    public class TheFirstFracturer : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 42;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Thrust;
            Item.knockBack = 3f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = 70;
            Item.height = 78;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 2);
            Item.shoot = ModContent.ProjectileType<FractureBeam>();
            Item.shootSpeed = 12f;
        }
    }
}

namespace CalamityFandom.Content.Buffs
{
    /// <summary>Buff de sbire du lot Q : reste tant que le sbire existe.</summary>
    public abstract class WikiMinionBuff : ModBuff
    {
        protected abstract int MinionType { get; }

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[MinionType] > 0)
                player.buffTime[buffIndex] = 18000;
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }

    public class FractalCrawlerBuff : WikiMinionBuff
    {
        protected override int MinionType => ModContent.ProjectileType<FractalCrawler>();
    }

    public class HolyMackerelBuff : WikiMinionBuff
    {
        protected override int MinionType => ModContent.ProjectileType<HolyMackerel>();
    }

    public class InfernumKrisBuff : WikiMinionBuff
    {
        protected override int MinionType => ModContent.ProjectileType<FallenHuman>();
    }
}

namespace CalamityFandom.Content.Projectiles.Weapons1119m
{
    /// <summary>Sbire du lot Q : garde le buff, suit le joueur sans cible, trouve sa cible.</summary>
    public abstract class WikiMinion : ModProjectile
    {
        protected abstract int BuffType { get; }
        protected virtual float Slots => 1f;

        public override void SetStaticDefaults()
        {
            Main.projPet[Type] = true;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = Slots;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 18000;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        protected Player Owner => Main.player[Projectile.owner];

        /// <summary>Faux si le sbire doit disparaître.</summary>
        protected bool KeepAlive()
        {
            if (Owner.dead || !Owner.active)
                Owner.ClearBuff(BuffType);
            if (Owner.HasBuff(BuffType))
                Projectile.timeLeft = 2;
            return Projectile.timeLeft > 1;
        }

        protected void Idle()
        {
            Vector2 home = Owner.Center + new Vector2(-Owner.direction * (40f + 30f * Projectile.minionPos), -40f);
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, (home - Projectile.Center) * 0.1f, 0.2f);
            if (Projectile.Distance(Owner.Center) > 2000f)
                Projectile.Center = Owner.Center;
        }

        protected void DrawFlipped(Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            SpriteEffects fx = Projectile.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, fx);
        }
    }

    /// <summary>Fractal Crawler : bondit sur la cible (gravité), fait tomber 3 cristaux (20 %) à chaque coup.</summary>
    public class FractalCrawler : WikiMinion
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Summon/FractalCrawler";
        protected override int BuffType => ModContent.BuffType<Buffs.FractalCrawlerBuff>();

        public override void AI()
        {
            if (!KeepAlive())
                return;
            NPC target = Projectile.Center.MinionHoming(900f, Owner);
            if (target == null)
            {
                Idle();
                Projectile.rotation = 0f;
            }
            else if (++Projectile.ai[0] % 35f == 0f)
            {
                // Bond vers la cible
                Projectile.velocity = Projectile.SafeDirectionTo(target.Center) * 13f + new Vector2(0f, -4f);
                Projectile.netUpdate = true;
            }
            else
                Projectile.velocity.Y += 0.3f;
            Projectile.spriteDirection = Projectile.velocity.X > 0 ? 1 : -1;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.owner != Main.myPlayer)
                return;
            for (int i = 0; i < 3; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Top, new Vector2(Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-6f, -3f)), ModContent.ProjectileType<FractalCrystal>(),
                    Math.Max(1, Projectile.damage / 5), 0f, Projectile.owner);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawFlipped(lightColor);
            return false;
        }
    }

    /// <summary>Cristal du Fractal Crawler : gravité.</summary>
    public class FractalCrystal : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.CrystalShard;

        public override void SetStaticDefaults() => ProjectileID.Sets.MinionShot[Type] = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 90;
        }

        public override void AI()
        {
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.25f, 12f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }
    }

    /// <summary>Holy Mackerel : demi-place ; fonce sur une cible à l'écran et frappe vite (Holy Flames 2 s).</summary>
    public class HolyMackerel : WikiMinion
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Summon/HolyMackerel";
        protected override int BuffType => ModContent.BuffType<Buffs.HolyMackerelBuff>();
        protected override float Slots => 0.5f;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (!KeepAlive())
                return;
            NPC target = Projectile.Center.MinionHoming(900f, Owner);
            if (target != null && !new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight).Intersects(target.Hitbox) && Main.netMode == NetmodeID.SinglePlayer)
                target = null;
            if (target == null)
                Idle();
            else
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(target.Center) * 16f, 0.12f);
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.spriteDirection = 1;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<HolyFlames>(), 120);
    }

    /// <summary>Humain déchu de l'Infernum Kris : presque immobile, lance des couteaux ; se téléporte au-dessus d'une cible lointaine (8 couteaux en cercle).</summary>
    public class FallenHuman : WikiMinion
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Summon/FallenHuman";
        protected override int BuffType => ModContent.BuffType<Buffs.InfernumKrisBuff>();
        protected override float Slots => 2f;

        public override bool? CanDamage() => false;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.scale = 0.6f;
        }

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
            Projectile.velocity *= 0.85f;
            Projectile.spriteDirection = target.Center.X > Projectile.Center.X ? 1 : -1;
            if (Projectile.owner != Main.myPlayer)
                return;
            if (Projectile.Distance(target.Center) > 600f)
            {
                Projectile.Center = target.Center - new Vector2(0f, 160f);
                SoundEngine.PlaySound(SoundID.Item8, Projectile.Center);
                for (int i = 0; i < 8; i++)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (MathHelper.TwoPi * i / 8f).ToRotationVector2() * 10f, ModContent.ProjectileType<KrisKnife>(),
                        Projectile.damage, Projectile.knockBack, Projectile.owner);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.UnitY * 16f, ModContent.ProjectileType<KrisKnife>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
                Projectile.netUpdate = true;
                return;
            }
            if (++Projectile.ai[0] % 12f == 0f)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.SafeDirectionTo(target.Center) * 14f, ModContent.ProjectileType<KrisKnife>(),
                    Projectile.damage, Projectile.knockBack, Projectile.owner);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawFlipped(lightColor);
            return false;
        }
    }

    /// <summary>Couteau de l'humain déchu.</summary>
    public class KrisKnife : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.ThrowingKnife;

        public override void SetStaticDefaults() => ProjectileID.Sets.MinionShot[Type] = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 90;
            Projectile.tileCollide = false;
        }

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        public override Color? GetAlpha(Color lightColor) => new Color(255, 80, 80, 200);
    }

    /// <summary>Rayon de The First Fracturer : au contact, 3 à 5 rayons viennent de hors de l'écran sur l'ennemi.</summary>
    public class FractureBeam : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 60;
        }

        public override void AI()
        {
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.IceTorch, Vector2.Zero, 100, new Color(150, 220, 255), 1.3f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.owner != Main.myPlayer || Projectile.ai[0] == 1f)
                return;
            for (int i = 0; i < Main.rand.Next(3, 6); i++)
            {
                Vector2 from = target.Center + Main.rand.NextVector2CircularEdge(1f, 1f) * 900f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), from, (target.Center - from).SafeNormalize(Vector2.UnitY) * 20f, Type, Projectile.damage / 2, 0f, Projectile.owner, 1f);
            }
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Projectile.ai[0] == 1f)
            {
                Projectile.tileCollide = false;
                Projectile.timeLeft = 70;
            }
        }
    }
}
