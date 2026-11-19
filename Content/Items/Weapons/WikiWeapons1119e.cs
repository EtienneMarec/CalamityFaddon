using System;
using CalamityFandom.Content.Projectiles.Weapons1119e;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
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

// Armes et objets inventés par le wiki (19/11, lot H) : sprites et valeurs des pages, recettes du wiki (WikiRecipes),
// butin par WikiLoot. Molten Piercer, Obsidigun, The Generator : Pyrogen. Large Father (The Wall of Bronze) attend son
// boss : sa recette du wiki remplacerait celle de la Crystyl Crusher de Calamity, qui ne serait plus fabricable.

namespace CalamityFandom.Content.Items.Weapons.Ranged
{
    /// <summary>
    /// Wiki : Axisdriver.txt — rafale de 3 balles exo qui traversent les blocs et percent 10 fois ; clic droit : une balle exo
    /// à tête chercheuse (x5) qui explose ; un critique fait pleuvoir 5 éclairs élémentaires ; 50 % de ne pas consommer.
    /// Debuffs : Exo Freeze 0,5 s, Holy Flames 2 s, Frostburn 2,5 s, On Fire! 3 s ; éclairs : Brimstone Flames 2 s en plus.
    /// </summary>
    public class Axisdriver : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 655;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 4;
            Item.useAnimation = 12;
            Item.reuseDelay = 27;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 8f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item41;
            Item.width = 134;
            Item.height = 50;
            Item.crit = 24;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
            Item.value = Item.sellPrice(gold: 30);
            Item.shoot = ModContent.ProjectileType<ExoBullet>();
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-30f, 0f);

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            // Clic droit : un seul tir lent
            bool alt = player.altFunctionUse == 2;
            Item.useTime = alt ? 39 : 4;
            Item.useAnimation = alt ? 39 : 12;
            Item.reuseDelay = alt ? 0 : 27;
            return true;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.NextBool();

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool alt = player.altFunctionUse == 2;
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<ExoBullet>(), alt ? damage * 5 : damage, knockback, player.whoAmI, alt ? 1f : 0f);
            return false;
        }
    }

    /// <summary>
    /// Wiki : Elemental Saw Launcher.txt — maintenir : charge une scie qui tourne (dégâts au contact, rafales de souffles
    /// arc-en-ciel un peu chercheurs, Elemental Mix 1,5 s) ; relâcher : la lance, elle revient seule ; clic droit pendant
    /// qu'elle est lancée : rappel rapide, explosion et 6 souffles au retour (10 s d'attente). Ne consomme pas de munitions.
    /// [technique] Texture de la scie : Light Disc vanilla aux couleurs de l'arc-en-ciel (pas de sprite sur le wiki).
    /// </summary>
    public class ElementalSawLauncher : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 50;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 6;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.channel = true;
            Item.knockBack = 1.75f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item22;
            Item.width = 84;
            Item.height = 46;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.sellPrice(gold: 22);
            Item.shoot = ModContent.ProjectileType<ElementalSaw>();
            Item.shootSpeed = 18f;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-14f, 0f);

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            int saw = ModContent.ProjectileType<ElementalSaw>();
            if (player.altFunctionUse == 2)
            {
                var sawPlayer = player.GetModPlayer<ElementalSawPlayer>();
                if (player.whoAmI == Main.myPlayer && sawPlayer.Cooldown <= 0)
                    foreach (var p in Main.ActiveProjectiles)
                        if (p.owner == player.whoAmI && p.type == saw && p.ai[0] == 1f)
                        {
                            p.ai[0] = 2f;
                            p.netUpdate = true;
                            sawPlayer.Cooldown = 600;
                        }
                return false;
            }
            return player.ownedProjectileCounts[saw] < 1;
        }
    }

    /// <summary>Attente du rappel de l'Elemental Saw Launcher (10 s).</summary>
    public class ElementalSawPlayer : ModPlayer
    {
        public int Cooldown;

        public override void PostUpdate()
        {
            if (Cooldown > 0)
                Cooldown--;
        }
    }

    /// <summary>
    /// Wiki : Obsidigun.txt — tir lent et très rapide comme le Sniper Rifle ; les Musket Balls deviennent des balles
    /// d'obsidienne qui éclatent en éclats au contact (High Velocity + Crystal Bullet). Butin de Pyrogen.
    /// </summary>
    public class Obsidigun : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 48;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item40;
            Item.width = 74;
            Item.height = 32;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(gold: 7, silver: 20);
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 28f;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-12f, 0f);

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (type == ProjectileID.Bullet)
                type = ModContent.ProjectileType<ObsidianBullet>();
        }
    }

    /// <summary>
    /// Wiki : Onyx Minigun.txt — rafales de 8 balles en 4 paires symétriques autour d'un tir d'onyx (170 %) ;
    /// 70 % de ne pas consommer. Amélioration de l'Onyxia.
    /// </summary>
    public class OnyxMinigun : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 900;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 7;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item36;
            Item.width = 92;
            Item.height = 42;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.value = Item.sellPrice(gold: 28);
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 34f;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-16f, 2f);

        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.Next(100) >= 70;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile shard = Projectile.NewProjectileDirect(source, position, velocity * 0.85f, ProjectileID.BlackBolt, (int)(damage * 1.7f), knockback * 2f, player.whoAmI);
            shard.timeLeft = (int)(shard.timeLeft * 1.4f);
            for (int i = 0; i < 4; i++)
            {
                float angle = Main.rand.NextFloat(0.01f, 0.05f) * (i + 1) * 0.6f;
                float speed = Main.rand.NextFloat(0.92f, 1.08f);
                Projectile.NewProjectile(source, position, velocity.RotatedBy(-angle) * speed, type, damage, knockback, player.whoAmI);
                Projectile.NewProjectile(source, position, velocity.RotatedBy(angle) * speed, type, damage, knockback, player.whoAmI);
            }
            return false;
        }
    }

    /// <summary>
    /// Wiki : SDOMG.txt — rafale de balles dispersées qui se resserrent en 10 s de tir continu ; 75 % de ne pas consommer ;
    /// clic droit immobile : consomme une Plasma Grenade (une toutes les 5 s) pour un grand rayon, pendant lequel le joueur
    /// a Slow, On Fire! et Oiled. Amélioration du SDFMG.
    /// </summary>
    public class SDOMG : ModItem
    {
        private int heldShots;
        private uint lastShot;

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 443;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 5;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item11;
            Item.width = 72;
            Item.height = 28;
            Item.rare = ModContent.RarityType<CosmicPurple>();
            Item.value = Item.sellPrice(gold: 20);
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 18f;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10f, 0f);

        public override bool AltFunctionUse(Player player) => true;

        private static int Grenade => ModContent.ItemType<CalamityMod.Items.Weapons.DraedonsArsenal.PlasmaGrenade>();

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse != 2)
            {
                Item.useAmmo = AmmoID.Bullet;
                Item.channel = false;
                Item.UseSound = SoundID.Item11;
                return true;
            }
            Item.useAmmo = AmmoID.None;
            Item.channel = true;
            Item.UseSound = null;
            return Math.Abs(player.velocity.X) < 0.1f && Math.Abs(player.velocity.Y) < 0.1f && player.HasItem(Grenade)
                && player.ownedProjectileCounts[ModContent.ProjectileType<SDOMGRay>()] < 1;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.Next(100) >= 75;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                player.ConsumeItem(Grenade);
                Projectile.NewProjectile(source, player.Center, velocity.SafeNormalize(Vector2.UnitX), ModContent.ProjectileType<SDOMGRay>(), damage * 3, knockback, player.whoAmI);
                return false;
            }
            // Dispersion qui se resserre en 10 s (600 images) de tir continu
            if (Main.GameUpdateCount - lastShot > 30)
                heldShots = 0;
            lastShot = Main.GameUpdateCount;
            heldShots = Math.Min(heldShots + Item.useTime, 600);
            float spread = MathHelper.Lerp(0.3f, 0.02f, heldShots / 600f);
            for (int i = 0; i < 2; i++)
                Projectile.NewProjectile(source, position, velocity.RotatedByRandom(spread), type, damage, knockback, player.whoAmI);
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Magic
{
    /// <summary>
    /// Wiki : The Generator.txt — sceau couronné qui traverse tout en ligne droite, attire les ennemis proches et les blesse
    /// sans arrêt ; Shadowflame 2 s ; éclaire. Butin de Pyrogen.
    /// </summary>
    public class TheGenerator : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 67;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 17;
            Item.useTime = Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item117;
            Item.width = 40;
            Item.height = 46;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(gold: 7, silver: 20);
            Item.shoot = ModContent.ProjectileType<CrownedSigil>();
            Item.shootSpeed = 8f;
        }
    }

    /// <summary>
    /// Wiki : Worst Prism.txt — absorbe l'énergie 5 s puis tire un éventail irrégulier de rayons de 100 blocs dont la
    /// longueur pulse au hasard (jusqu'à 100 blocs de plus ou de moins) ; chaque couleur a son debuff, Vaporfied 1,5 s pour
    /// tous ; le prisme blesse au contact ; les rayons s'arrêtent après 1 minute d'utilisation continue.
    /// </summary>
    public class WorstPrism : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 16;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.useTime = Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = Item.noUseGraphic = true;
            Item.channel = true;
            Item.knockBack = 0f;
            Item.autoReuse = true;
            Item.width = Item.height = 45;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(gold: 26);
            Item.shoot = ModContent.ProjectileType<WorstPrismHoldout>();
            Item.shootSpeed = 24f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;
    }

    /// <summary>
    /// Wiki : Unending Void.txt — anneau d'orbes d'énergie noire qui cherchent les ennemis proches (dérivent sinon) ; au
    /// contact, trou noir immobile (75 %, perce tout, 2,5 s). Chaque coup d'orbe ou de trou noir donne 1 de charge ; à 150,
    /// clic droit : trou noir supermassif qui attire des orbes (200 %), puis explose (1000 %) et lâche des orbes (300 %).
    /// </summary>
    public class UnendingVoid : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            Item.staff[Type] = false;
        }

        public override void SetDefaults()
        {
            Item.damage = 648;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 25;
            Item.useTime = Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item84;
            Item.width = 40;
            Item.height = 46;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.value = Item.sellPrice(gold: 40);
            Item.shoot = ModContent.ProjectileType<DarkEnergyOrb>();
            Item.shootSpeed = 12f;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player) => player.altFunctionUse != 2
            || (player.GetModPlayer<UnendingVoidPlayer>().Charge >= UnendingVoidPlayer.Max && player.ownedProjectileCounts[ModContent.ProjectileType<SupermassiveBlackHole>()] < 1);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                player.GetModPlayer<UnendingVoidPlayer>().Charge = 0;
                SoundEngine.PlaySound(SoundID.Item119, Main.MouseWorld);
                Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<SupermassiveBlackHole>(), damage, knockback, player.whoAmI);
                return false;
            }
            for (int i = 0; i < 8; i++)
            {
                Vector2 dir = (MathHelper.TwoPi * i / 8f).ToRotationVector2();
                Projectile.NewProjectile(source, player.Center + dir * 40f, dir * velocity.Length(), type, damage, knockback, player.whoAmI);
            }
            return false;
        }
    }

    /// <summary>Charge de l'Unending Void (coups d'orbes et de trous noirs), affichée au-dessus du joueur quand l'arme est tenue.</summary>
    public class UnendingVoidPlayer : ModPlayer
    {
        public const int Max = 150;
        public int Charge;

        public void Add()
        {
            if (Charge < Max && ++Charge == Max && Player.whoAmI == Main.myPlayer)
                CombatText.NewText(Player.Hitbox, new Color(160, 80, 220), "Void charged!");
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Garth.txt — chaque coup baisse la défense de 1 et soigne (1 à 68 PV) ; fait pleuvoir 3 météores RGB vers le
    /// curseur (On Fire! 5 s, Cursed Inferno 3 s, Frostburn 4 s) qui éclatent en 8 météores rouges (explosion, Holy Flames
    /// 10 s), verts (orbes de soin : 7,5 % des dégâts) ou bleus (Frostburn 3 s). Dédiée à Garth.
    /// </summary>
    public class Garth : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 170;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 9.5f;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.UseSound = SoundID.Item1;
            Item.width = 88;
            Item.height = 102;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.value = Item.sellPrice(gold: 40);
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.defense = Math.Max(0, target.defense - 1);
            if (!player.moonLeech && target.lifeMax > 5 && !target.friendly)
            {
                // [choix] 5 % des dégâts, entre 1 et 68
                int heal = Math.Clamp(damageDone / 20, 1, 68);
                player.Heal(heal);
            }
            if (player.whoAmI != Main.myPlayer)
                return;
            for (int i = 0; i < 3; i++)
            {
                Vector2 spawn = new(Main.MouseWorld.X + Main.rand.Next(-200, 201), Main.screenPosition.Y - 60f);
                Vector2 vel = (Main.MouseWorld - spawn).SafeNormalize(Vector2.UnitY) * 18f;
                Projectile.NewProjectile(player.GetSource_ItemUse(Item), spawn, vel, ModContent.ProjectileType<RGBMeteor>(), Math.Max(1, hit.SourceDamage / 2),
                    4f, player.whoAmI);
            }
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Rogue
{
    /// <summary>
    /// Wiki : Molten Piercer.txt — couteaux enflammés soumis à la gravité qui éclatent en éclats d'obsidienne (On Fire! 2 s) ;
    /// frappe furtive : 200 %, le couteau tourne en tempête de feu, ralentit jusqu'à l'arrêt et tire des éclats tout autour.
    /// Butin de Pyrogen.
    /// </summary>
    public class MoltenPiercer : RogueWeapon
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 57;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.useTime = Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = Item.noUseGraphic = true;
            Item.knockBack = 5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = Item.height = 32;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(gold: 7, silver: 20);
            Item.shoot = ModContent.ProjectileType<MoltenPiercerThrown>();
            Item.shootSpeed = 16f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool stealth = player.Calamity().StealthStrikeAvailable();
            int p = Projectile.NewProjectile(source, position, velocity, type, stealth ? damage * 2 : damage, knockback, player.whoAmI);
            if (stealth && Main.projectile.IndexInRange(p))
                Main.projectile[p].Calamity().stealthStrike = true;
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Accessories
{
    /// <summary>
    /// Wiki : Prehistoric Tablet.txt — invoque un Sulphur Elemental (250 dégâts) qui alterne : 3 nuages de pluie sulfurique
    /// au-dessus de la cible (6 s, Sulphuric Poisoning 12 s), puis 5 ruées. Butin du Mauler (8 %).
    /// </summary>
    public class PrehistoricTablet : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 24;
            Item.accessory = true;
            Item.damage = 250;
            Item.DamageType = DamageClass.Summon;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.value = Item.sellPrice(platinum: 1, gold: 25);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddBuff(ModContent.BuffType<Buffs.SulphurElementalBuff>(), 2);
            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<SulphurElementalMinion>()] < 1)
            {
                int damage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(Item.damage);
                int p = Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<SulphurElementalMinion>(), damage, 4f, player.whoAmI);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].originalDamage = Item.damage;
            }
        }
    }
}

namespace CalamityFandom.Content.Buffs
{
    public class SulphurElementalBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Weapons1119e
{
    internal static class ExoDebuffs
    {
        public static void Apply(NPC target)
        {
            target.AddBuff(ModContent.BuffType<Buffs.ExoFreeze>(), 30);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 120);
            target.AddBuff(BuffID.Frostburn, 150);
            target.AddBuff(BuffID.OnFire, 180);
        }
    }

    /// <summary>Balle exo : traverse les blocs, perce 10 fois ; ai[0] = 1 : balle chercheuse qui explose. Critique : 5 éclairs élémentaires.</summary>
    public class ExoBullet : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Ranged/ExoBullet";

        private bool Homing => Projectile.ai[0] == 1f;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 10;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Homing)
                Projectile.penetrate = 1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Lighting.AddLight(Projectile.Center, Main.DiscoColor.ToVector3() * 0.4f);
            if (Homing)
                CalamityUtils.HomeInOnNPC(Projectile, true, 800f, 14f, 20f);
        }

        public override Color? GetAlpha(Color lightColor) => Main.DiscoColor with { A = 200 };

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ExoDebuffs.Apply(target);
            if (hit.Crit && Projectile.owner == Main.myPlayer)
                for (int i = 0; i < 5; i++)
                {
                    Vector2 spawn = target.Center + new Vector2(Main.rand.Next(-300, 301), -700f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawn, (target.Center - spawn).SafeNormalize(Vector2.UnitY) * 16f,
                        ModContent.ProjectileType<ExoElementalBolt>(), Projectile.damage / 3, 0f, Projectile.owner);
                }
        }

        public override void OnKill(int timeLeft)
        {
            if (!Homing)
                return;
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ExoBlast>(), Projectile.damage / 2, 0f, Projectile.owner);
        }
    }

    /// <summary>Explosion de la balle exo : 130 px, une fois par ennemi, debuffs exo ; image de l'explosion de prisme de Calamity.</summary>
    public class ExoBlast : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Ranged/PrismExplosionSmall";

        public override void SetStaticDefaults() => Main.projFrames[Type] = 1;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 130;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 20;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.Opacity = Projectile.timeLeft / 20f;
            Projectile.scale = 1.4f - Projectile.timeLeft / 40f;
        }

        public override Color? GetAlpha(Color lightColor) => Main.DiscoColor * Projectile.Opacity;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => ExoDebuffs.Apply(target);
    }

    /// <summary>Éclair élémentaire de l'Axisdriver : tête chercheuse, debuffs exo et Brimstone Flames 2 s.</summary>
    public class ExoElementalBolt : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 120;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            CalamityUtils.HomeInOnNPC(Projectile, true, 900f, 16f, 15f);
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.RainbowTorch, Vector2.Zero, 100, Main.DiscoColor, 1.3f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ExoDebuffs.Apply(target);
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
        }
    }

    /// <summary>
    /// Scie de l'Elemental Saw Launcher. ai[0] : 0 = tenue (charge dans localAI[0]), 1 = lancée, 2 = rappel rapide (explosion
    /// et 6 souffles au retour). Lance un souffle arc-en-ciel toutes les 20 images (15 une fois lancée).
    /// </summary>
    public class ElementalSaw : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.LightDisc;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!owner.active || owner.dead)
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 2;
            Projectile.localAI[1]++;
            if (Projectile.ai[0] == 0f)
            {
                if (owner.channel && owner.HeldItem.type == ModContent.ItemType<Items.Weapons.Ranged.ElementalSawLauncher>())
                {
                    owner.itemTime = owner.itemAnimation = 2;
                    Projectile.localAI[0] = Math.Min(Projectile.localAI[0] + 1f, 120f);
                    if (Projectile.owner == Main.myPlayer)
                    {
                        Projectile.velocity = (Main.MouseWorld - owner.MountedCenter).SafeNormalize(Vector2.UnitX * owner.direction);
                        if (Projectile.localAI[1] % 10f == 0f)
                            Projectile.netUpdate = true;
                    }
                    owner.ChangeDir(Projectile.velocity.X > 0 ? 1 : -1);
                    owner.itemRotation = (Projectile.velocity * owner.direction).ToRotation();
                    Projectile.Center = owner.MountedCenter + Projectile.velocity * 60f;
                    Projectile.rotation += 0.1f + Projectile.localAI[0] * 0.004f;
                    Shoot(20);
                    return;
                }
                // Relâchée : lancée, plus vite si chargée
                Projectile.ai[0] = 1f;
                Projectile.ai[1] = 0f;
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX) * (18f + Projectile.localAI[0] / 10f);
                Projectile.netUpdate = true;
            }
            Projectile.rotation += 0.6f;
            Projectile.ai[1]++;
            Shoot(15);
            if (Projectile.ai[0] == 1f && Projectile.ai[1] < 45f)
                return;
            // Retour (normal, ou rapide au rappel)
            float speed = Projectile.ai[0] == 2f ? 34f : 20f;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(owner.Center) * speed, 0.15f);
            if (Projectile.Distance(owner.Center) >= 40f)
                return;
            if (Projectile.ai[0] == 2f && Projectile.owner == Main.myPlayer)
            {
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ExoBlast>(), Projectile.damage * 3, 0f, Projectile.owner);
                for (int i = 0; i < 6; i++)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (MathHelper.TwoPi * i / 6f).ToRotationVector2() * 10f,
                        ModContent.ProjectileType<ElementalSawBlast>(), Projectile.damage, 0f, Projectile.owner);
            }
            Projectile.Kill();
        }

        private void Shoot(int every)
        {
            if (Projectile.owner != Main.myPlayer || Projectile.localAI[1] % every != 0f)
                return;
            NPC target = Projectile.Center.ClosestNPCAt(700f);
            Vector2 dir = target != null ? Projectile.SafeDirectionTo(target.Center) : Main.rand.NextVector2Unit();
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, dir * 12f, ModContent.ProjectileType<ElementalSawBlast>(), Projectile.damage / 2, 0f, Projectile.owner);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<ElementalMix>(), 90);

        public override Color? GetAlpha(Color lightColor) => Main.DiscoColor with { A = 200 };
    }

    /// <summary>Souffle arc-en-ciel de l'Elemental Saw Launcher : un peu chercheur, Elemental Mix 1,5 s.</summary>
    public class ElementalSawBlast : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 90;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            CalamityUtils.HomeInOnNPC(Projectile, true, 400f, 12f, 30f);
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.RainbowTorch, Vector2.Zero, 100,
                    Main.hslToRgb((Projectile.identity * 0.13f + Main.GlobalTimeWrappedHourly) % 1f, 1f, 0.6f), 1.4f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<ElementalMix>(), 90);
    }

    /// <summary>Balle d'obsidienne de l'Obsidigun : rapide, éclate en 4 éclats au contact d'un ennemi ou d'un bloc.</summary>
    public class ObsidianBullet : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Ranged/ObsidianBullet";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 2;
        }

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation();

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27 with { Volume = 0.5f }, Projectile.Center);
            if (Projectile.owner != Main.myPlayer)
                return;
            for (int i = 0; i < 4; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center - Projectile.velocity, Main.rand.NextVector2CircularEdge(6f, 6f),
                    ModContent.ProjectileType<ObsidianShardProj>(), Projectile.damage / 3, 0f, Projectile.owner, 0f);
        }
    }

    /// <summary>Éclat d'obsidienne (Obsidigun, Molten Piercer) : soumis à la gravité ; ai[0] = 1 : dégâts de lanceur (On Fire! 2 s).</summary>
    public class ObsidianShardProj : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Ranged/ObsidianShard";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 90;
            Projectile.scale = 1.4f;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Projectile.ai[0] == 1f)
                Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
        }

        public override void AI()
        {
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.25f, 12f);
            Projectile.rotation += 0.3f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[0] == 1f)
                target.AddBuff(BuffID.OnFire, 120);
        }
    }

    /// <summary>Couteau du Molten Piercer : gravité, éclate en 3 éclats ; furtif : tourne, ralentit et tire des éclats autour (3 s).</summary>
    public class MoltenPiercerThrown : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Rogue/MoltenPiercerThrown";

        private bool Stealth => Projectile.Calamity().stealthStrike;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (!Main.dedServ && Main.rand.NextBool(2))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1.3f).noGravity = true;
            if (!Stealth)
            {
                Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.3f, 16f);
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                return;
            }
            // Tempête de feu : ralentit, tourne, tire des éclats dans toutes les directions
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.velocity *= 0.95f;
            Projectile.rotation += 0.5f;
            if (Projectile.owner == Main.myPlayer && Projectile.timeLeft % 5 == 0)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (Projectile.rotation).ToRotationVector2() * 8f,
                    ModContent.ProjectileType<ObsidianShardProj>(), Projectile.damage / 4, 0f, Projectile.owner, 1f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.OnFire, 120);

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27 with { Volume = 0.5f }, Projectile.Center);
            if (Projectile.owner != Main.myPlayer || Stealth)
                return;
            for (int i = 0; i < 3; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center - Projectile.velocity, new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-6f, -2f)),
                    ModContent.ProjectileType<ObsidianShardProj>(), Projectile.damage / 3, 0f, Projectile.owner, 1f);
        }
    }

    /// <summary>Sceau couronné de The Generator : traverse tout, attire les ennemis (pas les boss) à 240 px, Shadowflame 2 s.</summary>
    public class CrownedSigil : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Magic/CrownedSigil";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 36;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 150;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Projectile.rotation += 0.08f;
            Lighting.AddLight(Projectile.Center, 0.6f, 0.2f, 0.8f);
            foreach (NPC npc in Main.ActiveNPCs)
                if (npc.CanBeChasedBy() && !npc.boss && npc.knockBackResist > 0f && npc.Distance(Projectile.Center) < 240f)
                    npc.velocity = Vector2.Lerp(npc.velocity, npc.SafeDirectionTo(Projectile.Center) * 6f, 0.1f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.ShadowFlame, 120);
    }

    /// <summary>
    /// Prisme tenu du Worst Prism : charge 5 s puis tire 7 rayons de couleur ; blesse au contact ; les rayons s'arrêtent
    /// après 1 minute d'utilisation continue (le prisme reste).
    /// </summary>
    public class WorstPrismHoldout : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Magic/WorstPrism";

        public const int Charge = 300;
        public const int Beams = 7;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.scale = 0.6f;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!owner.channel || owner.dead || owner.noItems || owner.CCed)
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 2;
            owner.heldProj = Projectile.whoAmI;
            owner.itemTime = owner.itemAnimation = 2;
            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 aim = (Main.MouseWorld - owner.MountedCenter).SafeNormalize(Vector2.UnitX * owner.direction);
                if (aim != Projectile.velocity)
                    Projectile.netUpdate = true;
                Projectile.velocity = aim;
            }
            owner.ChangeDir(Projectile.velocity.X > 0 ? 1 : -1);
            Projectile.Center = owner.MountedCenter + Projectile.velocity * 30f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            owner.itemRotation = (Projectile.velocity * owner.direction).ToRotation();
            Projectile.ai[0]++;
            // Mana : une fois par utilisation de l'objet (12 images)
            if (Projectile.ai[0] % 12f == 0f && !owner.CheckMana(owner.HeldItem, -1, true))
            {
                Projectile.Kill();
                return;
            }
            if (Projectile.ai[0] < Charge)
            {
                if (!Main.dedServ)
                {
                    Vector2 from = Projectile.Center + Main.rand.NextVector2CircularEdge(60f, 60f);
                    Dust.NewDustPerfect(from, DustID.RainbowTorch, (Projectile.Center - from) * 0.08f, 100, Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.6f), 1.2f).noGravity = true;
                }
                return;
            }
            if (Projectile.ai[0] == Charge && Projectile.owner == Main.myPlayer)
            {
                SoundEngine.PlaySound(SoundID.Item122, Projectile.Center);
                for (int i = 0; i < Beams; i++)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<WorstPrismBeam>(),
                        Projectile.damage, 0f, Projectile.owner, Projectile.whoAmI, i);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Color c = Projectile.ai[0] < Charge ? Color.White : Main.hslToRgb(Main.GlobalTimeWrappedHourly % 1f, 1f, 0.75f);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, c, Projectile.rotation + MathHelper.PiOver4, tex.Size() / 2f, Projectile.scale, SpriteEffects.None);
            return false;
        }
    }

    /// <summary>
    /// Rayon du Worst Prism : éventail irrégulier, longueur de 100 blocs qui pulse au hasard entre 0 et 200 blocs. Debuff selon
    /// la couleur (rouge On Fire!, orange Hellfire, jaune Ichor, vert Poisoned, cyan Frostburn, bleu Confused, violet
    /// Shadowflame) et Vaporfied 1,5 s pour tous. Disparaît après 1 minute.
    /// </summary>
    public class WorstPrismBeam : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private static readonly Color[] colors = { Color.Red, Color.Orange, Color.Yellow, Color.Lime, Color.Cyan, Color.Blue, Color.Purple };
        private static readonly int[] debuffs = { BuffID.OnFire, BuffID.OnFire3, BuffID.Ichor, BuffID.Poisoned, BuffID.Frostburn, BuffID.Confused, BuffID.ShadowFlame };
        private float length = 1600f;
        private float offset;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 3600;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void OnSpawn(IEntitySource source) => offset = Main.rand.NextFloat(-0.35f, 0.35f);

        public override void AI()
        {
            Projectile prism = Main.projectile[(int)Projectile.ai[0]];
            if (!prism.active || prism.type != ModContent.ProjectileType<WorstPrismHoldout>() || prism.owner != Projectile.owner)
            {
                Projectile.Kill();
                return;
            }
            // Éventail irrégulier qui tremble, longueur qui pulse au hasard
            if (Main.rand.NextBool(6))
                offset = MathHelper.Clamp(offset + Main.rand.NextFloat(-0.08f, 0.08f), -0.45f, 0.45f);
            length = MathHelper.Clamp(length + Main.rand.NextFloat(-240f, 240f), 0f, 3200f);
            Projectile.velocity = prism.velocity.RotatedBy(offset);
            Projectile.Center = prism.Center;
            Lighting.AddLight(Projectile.Center + Projectile.velocity * length * 0.5f, colors[(int)Projectile.ai[1] % colors.Length].ToVector3() * 0.5f);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * length, 14f, ref _);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(debuffs[(int)Projectile.ai[1] % debuffs.Length], 180);
            target.AddBuff(ModContent.BuffType<Vaporfied>(), 90);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D px = TextureAssets.MagicPixel.Value;
            Color c = colors[(int)Projectile.ai[1] % colors.Length] with { A = 0 };
            Vector2 start = Projectile.Center - Main.screenPosition;
            float rot = Projectile.velocity.ToRotation();
            Main.EntitySpriteDraw(px, start, new Rectangle(0, 0, 1, 1), c * 0.8f, rot, new Vector2(0f, 0.5f), new Vector2(length, 10f), SpriteEffects.None);
            Main.EntitySpriteDraw(px, start, new Rectangle(0, 0, 1, 1), Color.White with { A = 0 } * 0.6f, rot, new Vector2(0f, 0.5f), new Vector2(length, 4f), SpriteEffects.None);
            return false;
        }
    }

    /// <summary>Rayon du SDOMG : tenu tant que la grenade dure (5 s chacune), joueur immobile ; Slow, On Fire! et Oiled au joueur.</summary>
    public class SDOMGRay : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private const float Length = 2000f;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!owner.channel || owner.dead || owner.HeldItem.type != ModContent.ItemType<Items.Weapons.Ranged.SDOMG>())
            {
                Projectile.Kill();
                return;
            }
            owner.itemTime = owner.itemAnimation = 2;
            owner.heldProj = Projectile.whoAmI;
            owner.AddBuff(BuffID.Slow, 2);
            owner.AddBuff(BuffID.OnFire, 2);
            owner.AddBuff(BuffID.Oiled, 2);
            owner.velocity.X *= 0.5f;
            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 aim = (Main.MouseWorld - owner.MountedCenter).SafeNormalize(Vector2.UnitX * owner.direction);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, aim, 0.08f).SafeNormalize(aim);
                Projectile.netUpdate = true;
                // Une grenade toutes les 5 s
                if (Projectile.timeLeft == 1)
                {
                    int grenade = ModContent.ItemType<CalamityMod.Items.Weapons.DraedonsArsenal.PlasmaGrenade>();
                    if (owner.ConsumeItem(grenade))
                        Projectile.timeLeft = 300;
                }
            }
            owner.ChangeDir(Projectile.velocity.X > 0 ? 1 : -1);
            owner.itemRotation = (Projectile.velocity * owner.direction).ToRotation();
            Projectile.Center = owner.MountedCenter + Projectile.velocity * 40f;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * Length, 40f, ref _);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D px = TextureAssets.MagicPixel.Value;
            Vector2 start = Projectile.Center - Main.screenPosition;
            float rot = Projectile.velocity.ToRotation();
            float pulse = 1f + 0.15f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 30f);
            Main.EntitySpriteDraw(px, start, new Rectangle(0, 0, 1, 1), new Color(80, 200, 255, 0) * 0.7f, rot, new Vector2(0f, 0.5f), new Vector2(Length, 40f * pulse), SpriteEffects.None);
            Main.EntitySpriteDraw(px, start, new Rectangle(0, 0, 1, 1), new Color(255, 255, 255, 0) * 0.8f, rot, new Vector2(0f, 0.5f), new Vector2(Length, 16f * pulse), SpriteEffects.None);
            return false;
        }
    }

    /// <summary>
    /// Orbe d'énergie noire de l'Unending Void. ai[0] : 0 = cherche les ennemis (dérive sinon), 1 = vole vers le trou noir
    /// supermassif ai[1] (200 %, déjà réglé à la création), 2 = orbe lâché par l'explosion (300 %). Au contact : trou noir 75 %.
    /// </summary>
    public class DarkEnergyOrb : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Magic/DarkEnergyOrb";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 240;
        }

        public override void AI()
        {
            Projectile.rotation += 0.1f;
            Lighting.AddLight(Projectile.Center, 0.4f, 0.1f, 0.5f);
            if (Projectile.ai[0] == 1f)
            {
                Projectile hole = Main.projectile[(int)Projectile.ai[1]];
                if (!hole.active || hole.type != ModContent.ProjectileType<SupermassiveBlackHole>())
                {
                    Projectile.Kill();
                    return;
                }
                Projectile.velocity = Projectile.SafeDirectionTo(hole.Center) * 14f;
                if (Projectile.Distance(hole.Center) < 20f)
                    Projectile.Kill();
                return;
            }
            Projectile.velocity *= 0.985f;
            CalamityUtils.HomeInOnNPC(Projectile, true, 700f, 14f, 20f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Main.player[Projectile.owner].GetModPlayer<Items.Weapons.Magic.UnendingVoidPlayer>().Add();
            if (Projectile.owner == Main.myPlayer && Projectile.ai[0] != 1f)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<VoidHole>(), (int)(Projectile.damage * 0.75f), 0f, Projectile.owner);
        }
    }

    /// <summary>Trou noir de l'Unending Void : immobile 2,5 s, perce tout, frappe souvent, 1 de charge par coup.</summary>
    public class VoidHole : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Magic/EventHorizonBlackhole";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 150;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Projectile.rotation -= 0.2f;
            Projectile.scale = Math.Min(1f, (150 - Projectile.timeLeft) / 15f) * Math.Min(1f, Projectile.timeLeft / 15f);
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => Main.player[Projectile.owner].GetModPlayer<Items.Weapons.Magic.UnendingVoidPlayer>().Add();
    }

    /// <summary>
    /// Trou noir supermassif de l'Unending Void : grossit 3 s en attirant des orbes (200 %) depuis 600 px, puis explose
    /// (1000 % sur 400 px) et lâche 12 orbes chercheurs (300 %).
    /// </summary>
    public class SupermassiveBlackHole : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Magic/SupermassiveBlackHole";

        private const int Build = 180;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = Build;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool? CanDamage() => Projectile.timeLeft <= 1 ? null : false;

        public override void AI()
        {
            Projectile.rotation += 0.15f;
            Projectile.scale = 1f + 3f * (1f - Projectile.timeLeft / (float)Build);
            Lighting.AddLight(Projectile.Center, 0.5f, 0.1f, 0.7f);
            if (Projectile.owner == Main.myPlayer && Projectile.timeLeft % 8 == 0)
            {
                Vector2 from = Projectile.Center + Main.rand.NextVector2CircularEdge(600f, 600f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), from, Vector2.Zero, ModContent.ProjectileType<DarkEnergyOrb>(), Projectile.damage * 2, 0f, Projectile.owner, 1f, Projectile.whoAmI);
            }
            if (Projectile.timeLeft == 1)
            {
                // Explosion : agrandit la zone pour la dernière image
                Projectile.ExpandHitboxBy(400);
                Projectile.damage *= 10;
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item62, Projectile.Center);
            if (!Main.dedServ)
                for (int i = 0; i < 60; i++)
                    Dust.NewDustPerfect(Projectile.Center, DustID.Shadowflame, Main.rand.NextVector2Circular(14f, 14f), 100, default, 2f).noGravity = true;
            if (Projectile.owner != Main.myPlayer)
                return;
            int baseDamage = Projectile.damage / 10;
            for (int i = 0; i < 12; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (MathHelper.TwoPi * i / 12f).ToRotationVector2() * 10f,
                    ModContent.ProjectileType<DarkEnergyOrb>(), baseDamage * 3, 0f, Projectile.owner, 2f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None);
            return false;
        }
    }

    /// <summary>Météore RGB de Garth : tombe vers le curseur à travers les blocs ; au contact ou après 40 images, éclate en 8 météores colorés.</summary>
    public class RGBMeteor : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Melee/RGBMeteor";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.RainbowTorch, Vector2.Zero, 100, Main.DiscoColor, 1.2f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 300);
            target.AddBuff(BuffID.CursedInferno, 180);
            target.AddBuff(BuffID.Frostburn, 240);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item89 with { Volume = 0.5f }, Projectile.Center);
            if (Projectile.owner != Main.myPlayer)
                return;
            for (int i = 0; i < 8; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (MathHelper.TwoPi * i / 8f).ToRotationVector2() * 10f,
                    ModContent.ProjectileType<ColorMeteor>(), Projectile.damage / 2 + 1, 2f, Projectile.owner, Main.rand.Next(3));
        }
    }

    /// <summary>Météore coloré de Garth : ai[0] = 0 rouge (explosion, Holy Flames 10 s), 1 vert (orbe de soin 7,5 %), 2 bleu (Frostburn 3 s).</summary>
    public class ColorMeteor : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Melee/RedMeteor";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 50;
            Projectile.scale = 0.8f;
        }

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            switch ((int)Projectile.ai[0])
            {
                case 0:
                    target.AddBuff(ModContent.BuffType<HolyFlames>(), 600);
                    if (Projectile.owner == Main.myPlayer)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GarthBlast>(), Projectile.damage / 2 + 1, 0f, Projectile.owner);
                    break;
                case 1:
                    if (Projectile.owner == Main.myPlayer)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Circular(3f, 3f), ModContent.ProjectileType<GarthHealOrb>(), 0, 0f,
                            Projectile.owner, Math.Max(1, (int)(damageDone * 0.075f)));
                    break;
                default:
                    target.AddBuff(BuffID.Frostburn, 180);
                    break;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            string tex = (int)Projectile.ai[0] switch { 0 => "RedMeteor", 1 => "GreenMeteor", _ => "BlueMeteor" };
            Texture2D t = ModContent.Request<Texture2D>("CalamityFandom/Content/Projectiles/Melee/" + tex).Value;
            Main.EntitySpriteDraw(t, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, t.Size() / 2f, Projectile.scale, SpriteEffects.None);
            return false;
        }
    }

    /// <summary>Explosion du météore rouge de Garth (100 px, Holy Flames 10 s).</summary>
    public class GarthBlast : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 5;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.Item14 with { Volume = 0.5f }, Projectile.Center);
            if (!Main.dedServ)
                for (int i = 0; i < 20; i++)
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.RedTorch, 0f, 0f, 100, default, 1.6f).velocity *= 2f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<HolyFlames>(), 600);
    }

    /// <summary>Orbe de soin du météore vert de Garth : revient au joueur et rend ai[0] PV.</summary>
    public class GarthHealOrb : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(owner.Center) * 12f, 0.08f);
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.GreenTorch, Vector2.Zero, 100, default, 1.3f).noGravity = true;
            if (!Projectile.Hitbox.Intersects(owner.Hitbox))
                return;
            if (Projectile.owner == Main.myPlayer && !owner.moonLeech)
                owner.Heal((int)Projectile.ai[0]);
            Projectile.Kill();
        }
    }

    /// <summary>
    /// Sulphur Elemental de la Prehistoric Tablet : suit le joueur ; près d'une cible, alterne 3 nuages sulfuriques au-dessus
    /// d'elle (6 s) et 5 ruées.
    /// </summary>
    public class SulphurElementalMinion : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Summon/SulphurElementalMinion";

        public override void SetStaticDefaults()
        {
            Main.projPet[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 2;
            Projectile.scale = 0.5f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        // ai[0] : minuteur ; ai[1] : 0 nuages, 1 ruées ; localAI[0] : ruées restantes
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!owner.active || owner.dead || !owner.HasBuff<Buffs.SulphurElementalBuff>())
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 2;
            NPC target = Projectile.Center.MinionHoming(900f, owner);
            if (target == null)
            {
                Vector2 home = owner.Center + new Vector2(owner.direction * 60f, -60f);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, (home - Projectile.Center) * 0.08f, 0.2f);
                Projectile.spriteDirection = owner.direction;
                return;
            }
            Projectile.spriteDirection = Math.Sign(target.Center.X - Projectile.Center.X);
            Projectile.ai[0]++;
            if (Projectile.ai[1] == 0f)
            {
                Vector2 near = target.Center + new Vector2(-Projectile.spriteDirection * 160f, -80f);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, (near - Projectile.Center) * 0.06f, 0.15f);
                if (Projectile.ai[0] == 30f && Projectile.owner == Main.myPlayer)
                    for (int i = -1; i <= 1; i++)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center + new Vector2(i * 90f, -220f), Vector2.Zero,
                            ModContent.ProjectileType<SulphuricRaincloud>(), Projectile.damage / 3, 0f, Projectile.owner);
                if (Projectile.ai[0] >= 120f)
                {
                    Projectile.ai[0] = 0f;
                    Projectile.ai[1] = 1f;
                    Projectile.localAI[0] = 5f;
                    Projectile.netUpdate = true;
                }
                return;
            }
            // Ruées : une toutes les 30 images
            if (Projectile.ai[0] % 30f == 1f)
            {
                Projectile.velocity = Projectile.SafeDirectionTo(target.Center) * 20f;
                Projectile.localAI[0]--;
            }
            else
                Projectile.velocity *= 0.94f;
            if (Projectile.localAI[0] <= 0f && Projectile.ai[0] % 30f == 29f)
            {
                Projectile.ai[0] = 0f;
                Projectile.ai[1] = 0f;
                Projectile.netUpdate = true;
            }
        }

        public override bool? CanDamage() => Projectile.ai[1] == 1f && Projectile.velocity.Length() > 8f ? null : false;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 720);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            SpriteEffects fx = Projectile.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), 0f, tex.Size() / 2f, Projectile.scale, fx);
            return false;
        }
    }

    /// <summary>Nuage sulfurique : 6 s, gouttes acides toutes les 10 images (Sulphuric Poisoning 12 s).</summary>
    public class SulphuricRaincloud : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Summon/SulphuricRaincloud";

        public override void SetStaticDefaults() => Main.projFrames[Type] = 3;

        public override void SetDefaults()
        {
            Projectile.width = 58;
            Projectile.height = 32;
            Projectile.friendly = false;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 360;
        }

        public override void AI()
        {
            if (++Projectile.frameCounter >= 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
            }
            if (Projectile.owner == Main.myPlayer && Projectile.timeLeft % 10 == 0)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + new Vector2(Main.rand.Next(-24, 25), 12f), new Vector2(0f, 10f),
                    ModContent.ProjectileType<SulphuricDrop>(), Projectile.damage, 0f, Projectile.owner);
        }
    }

    /// <summary>Goutte acide du nuage sulfurique.</summary>
    public class SulphuricDrop : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.RainFriendly;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 90;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(170, 230, 60, 200);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 720);
    }
}
