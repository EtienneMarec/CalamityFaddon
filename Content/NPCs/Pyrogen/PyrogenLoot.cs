using System;
using CalamityFandom.Content.Projectiles.PyrogenLoot;
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

// Butin de Pyrogen inventé par le wiki (19/11) : il tombe par WikiLoot (infobox de Pyrogen).

namespace CalamityFandom.Content.Items.Accessories
{
    /// <summary>
    /// Wiki : Soul of Pyrogen.txt — +20 % de dégâts de mêlée ; les coups de vraie mêlée font sortir 5 lances du sol (50 % des
    /// dégâts de l'arme tenue, Dragon Fire 2 s). Objet Expert (rare -11). [choix] Au plus une salve de lances toutes les 20 images.
    /// </summary>
    public class SoulofPyrogen : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = Item.height = 34;
            Item.accessory = true;
            Item.expert = true;
            Item.rare = ItemRarityID.Expert;
            Item.value = Item.sellPrice(gold: 5);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Melee) += 0.2f;
            player.GetModPlayer<SoulofPyrogenPlayer>().Equipped = true;
        }
    }

    public class SoulofPyrogenPlayer : ModPlayer
    {
        public bool Equipped;
        private int cooldown;

        public override void ResetEffects() => Equipped = false;

        public override void PostUpdate()
        {
            if (cooldown > 0)
                cooldown--;
        }

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone) => Spears(target);

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (proj.DamageType == ModContent.GetInstance<TrueMeleeDamageClass>() || proj.DamageType == ModContent.GetInstance<TrueMeleeNoSpeedDamageClass>())
                Spears(target);
        }

        private void Spears(NPC target)
        {
            if (!Equipped || cooldown > 0 || Player.whoAmI != Main.myPlayer || target.friendly)
                return;
            cooldown = 20;
            int damage = Math.Max(1, (int)(Player.GetWeaponDamage(Player.HeldItem) * 0.5f));
            for (int i = 0; i < 5; i++)
                Projectile.NewProjectile(Player.GetSource_Accessory(Player.HeldItem), target.Bottom + new Vector2((i - 2) * 24f, 60f), new Vector2(0f, -14f),
                    ModContent.ProjectileType<Pyrospear>(), damage, 2f, Player.whoAmI);
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Seared Shredder.txt — épée qui lance des faux de flammes comme la Death Sickle : elles s'arrêtent et tournent
    /// sur place, traversent les blocs et les ennemis, éclairent ; Brimstone Flames et On Fire! (3 et 5 s au contact, 2 et
    /// 3 s par la faux). Butin de Pyrogen.
    /// </summary>
    public class SearedShredder : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 62;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 19;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 7.5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item71;
            Item.width = 74;
            Item.height = 68;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 16);
            Item.shoot = ModContent.ProjectileType<FlameScythe>();
            Item.shootSpeed = 18f;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 180);
            target.AddBuff(BuffID.OnFire, 300);
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Summon
{
    /// <summary>
    /// Wiki : Hell's Bells.txt — invoque une cloche d'obsidienne qui suit le curseur et lâche des carillons circulaires qui
    /// traversent tout autour d'elle (On Fire! 1 s). Butin de Pyrogen.
    /// </summary>
    public class HellsBells : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.GamepadWholeScreenUseRange[Type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 39;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 10;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 7f;
            Item.UseSound = SoundID.Item44;
            Item.width = Item.height = 42;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(gold: 7, silver: 20);
            Item.buffType = ModContent.BuffType<Buffs.ObsidianBellBuff>();
            Item.shoot = ModContent.ProjectileType<ObsidianBell>();
            Item.shootSpeed = 10f;
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
}

namespace CalamityFandom.Content.Buffs
{
    /// <summary>Buff de la cloche d'obsidienne (Hell's Bells). [technique] Icône : le sprite de l'arme (pas d'image de buff sur le wiki).</summary>
    public class ObsidianBellBuff : ModBuff
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Summon/HellsBells";

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<ObsidianBell>()] > 0)
                player.buffTime[buffIndex] = 18000;
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}

namespace CalamityFandom.Content.Projectiles.PyrogenLoot
{
    /// <summary>Lance de la Soul of Pyrogen : sort du sol sous la cible, Dragon Fire 2 s.</summary>
    public class Pyrospear : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.SharpTears;

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 20;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.85f;
            Projectile.rotation = -MathHelper.PiOver2;
            if (!Main.dedServ && Main.rand.NextBool(2))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, -2f, 100, default, 1.3f).noGravity = true;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 140, 60, 220);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Dragonfire>(), 120);
    }

    /// <summary>Faux de flammes du Seared Shredder : ralentit puis tourne sur place (comme la Death Sickle), traverse tout.</summary>
    public class FlameScythe : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.DeathSickle;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 120;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void AI()
        {
            Projectile.rotation += 0.4f * Projectile.direction;
            if (Projectile.timeLeft < 100)
                Projectile.velocity *= 0.92f;
            Lighting.AddLight(Projectile.Center, 1f, 0.5f, 0.15f);
            if (!Main.dedServ && Main.rand.NextBool(2))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1.4f).noGravity = true;
            if (Projectile.timeLeft < 20)
                Projectile.alpha += 12;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 120, 40, 200) * Projectile.Opacity;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
            target.AddBuff(BuffID.OnFire, 180);
        }
    }

    /// <summary>Cloche d'obsidienne de Hell's Bells : suit le curseur et sonne un carillon toutes les 50 images (moins si plus rapide).</summary>
    public class ObsidianBell : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Summon/ObsidianBell";

        public override void SetStaticDefaults()
        {
            Main.projPet[Type] = true;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 1f;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 18000;
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.dead || !owner.active)
                owner.ClearBuff(ModContent.BuffType<Buffs.ObsidianBellBuff>());
            if (owner.HasBuff(ModContent.BuffType<Buffs.ObsidianBellBuff>()))
                Projectile.timeLeft = 2;
            if (Projectile.owner == Main.myPlayer)
            {
                // Suit le curseur (un peu décalé selon son rang, pour que plusieurs cloches ne se superposent pas)
                Vector2 dest = Main.MouseWorld + new Vector2(0f, -30f * Projectile.minionPos);
                Projectile.velocity = (dest - Projectile.Center) * 0.12f;
                if (Main.GameUpdateCount % 10 == 0)
                    Projectile.netUpdate = true;
            }
            Projectile.rotation = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 6f) * 0.3f;
            Lighting.AddLight(Projectile.Center, 0.6f, 0.25f, 0.05f);
            if (Projectile.owner == Main.myPlayer && ++Projectile.ai[0] >= 50f)
            {
                Projectile.ai[0] = 0f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BellChime>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }
    }

    /// <summary>Carillon de la cloche : anneau qui grandit jusqu'à 160 px et frappe une fois chaque ennemi traversé (On Fire! 1 s).</summary>
    public class BellChime : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private const float MaxRadius = 160f;
        private float Radius => MaxRadius * (1f - Projectile.timeLeft / 30f);

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 30;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void OnSpawn(IEntitySource source) => SoundEngine.PlaySound(SoundID.Item35 with { Volume = 0.6f, Pitch = -0.4f }, Projectile.Center);

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float d = Vector2.Distance(Projectile.Center, targetHitbox.ClosestPointInRect(Projectile.Center));
            return d < Radius + 10f;
        }

        public override void AI()
        {
            if (Main.dedServ)
                return;
            for (int i = 0; i < 8; i++)
            {
                Vector2 dir = Main.rand.NextVector2Unit();
                Dust.NewDustPerfect(Projectile.Center + dir * Radius, DustID.Torch, dir, 100, default, 1.2f).noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.OnFire, 60);
    }
}
