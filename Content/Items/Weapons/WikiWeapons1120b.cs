using System;
using System.Collections.Generic;
using System.Linq;
using CalamityFandom.Common.Debug;
using CalamityFandom.Content.Legacy.Buffs.StatDebuffs;
using CalamityFandom.Content.Projectiles.Weapons1119m;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

// Lot AS (20/11) : objets que le butin du wiki donne à des PNJ existants (Plaguebringer Goliath, Providence, Crabulon,
// Present Mimic, Snow Balla, Cosmic Elemental, ennemis mécaniques...) : le butin s'ajoute tout seul (WikiLoot) dès que
// l'objet existe.

namespace CalamityFandom.Content.Items.Weapons.Ranged
{
    /// <summary>Wiki : Cent Striker.txt — « juste un lance-pièces normal », le plus faible (ennemis de surface, 0,5 %).</summary>
    public class CentStriker : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 0;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item11;
            Item.width = 50;
            Item.height = 22;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(gold: 2);
            Item.shoot = ProjectileID.CopperCoin;
            Item.shootSpeed = 24f;
            Item.useAmmo = AmmoID.Coin;
        }
    }

    /// <summary>
    /// Wiki : Holy Purge.txt — lance-flammes : long jet de flammes orange et jaunes qui traversent tout mais pas les blocs,
    /// Holy Flames 3 s ; 90 % de chances de ne pas consommer le gel (Providence).
    /// </summary>
    public class HolyPurge : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 175;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item34;
            Item.width = 102;
            Item.height = 56;
            Item.scale = 0.5f;
            Item.rare = ItemRarityID.Cyan;
            Item.value = Item.sellPrice(gold: 50);
            Item.shoot = ModContent.ProjectileType<Projectiles.Ranged.HolyPurgeFlame>();
            Item.shootSpeed = 20f;
            Item.useAmmo = AmmoID.Gel;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.NextFloat() >= 0.9f;

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
            => type = ModContent.ProjectileType<Projectiles.Ranged.HolyPurgeFlame>();

        public override Vector2? HoldoutOffset() => new Vector2(-10f, 0f);
    }
}

namespace CalamityFandom.Content.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Holiday Halberd.txt — chaque coup lance un sac cadeau qui rebondit (rouge 2 fois sur 3 : On Fire! 5 s ; vert :
    /// Cursed Inferno 2 s) et disparaît après deux rebonds ou deux ennemis ; les sacs laissent des nuages de spores (Present
    /// Mimic). [choix] Nuage toutes les 10 images, 25 % des dégâts, 1 s.
    /// </summary>
    public class HolidayHalberd : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 98;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 7.5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = 70;
            Item.height = 72;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 12);
            Item.shoot = ModContent.ProjectileType<Projectiles.Melee.GiftBag>();
            Item.shootSpeed = 12f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, Main.rand.NextBool(3) ? 1f : 0f);
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Rogue
{
    /// <summary>
    /// Wiki : The Last Lightice.txt — arme de spy non consommable : flot rapide de glace magique qui se pose en bloc de glace
    /// temporaire sur un bloc ou un ennemi, Glacial State 3 s ; frappe furtive : 20 blocs en éventail de 25° (Snow Balla).
    /// [choix] Pas de vraie tuile posée : le morceau de glace s'arrête et reste 3 s (pour ne pas abîmer le monde).
    /// </summary>
    public class TheLastLightice : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 45;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.useTime = Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 0f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item28;
            Item.width = 32;
            Item.height = 34;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(gold: 7, silver: 20);
            Item.shoot = ModContent.ProjectileType<Projectiles.Rogue.LightIce>();
            Item.shootSpeed = 8f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                for (int i = 0; i < 20; i++)
                {
                    int p = Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(-12.5f + 25f * i / 19f)) * Main.rand.NextFloat(0.9f, 1.3f), type, damage, knockback, player.whoAmI);
                    if (Main.projectile.IndexInRange(p))
                        Main.projectile[p].Calamity().stealthStrike = true;
                }
                player.Calamity().ConsumeStealthByAttacking();
                return false;
            }
            Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.05f), type, damage, knockback, player.whoAmI);
            return false;
        }
    }

    /// <summary>
    /// Wiki : Death-Needle Stinger.txt — lance une aiguille enchaînée qui se plante dans l'ennemi (dégâts continus) puis
    /// revient, trois au plus ; clic droit : un dard à courte portée qui revient au contact (Whispering Death 3 s, recharge
    /// 10 s) ; frappe furtive : les trois aiguilles d'un coup (200 %, Marked 5 s), l'ennemi touché est tiré vers le joueur
    /// (le joueur vers un boss). Plague 2 s.
    /// [choix] Aiguille plantée 2 s ; portée 600 px ; dard 250 px ; traction de 8 px par image.
    /// </summary>
    public class DeathNeedleStinger : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 75;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.useTime = Item.useAnimation = 8;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = Item.height = 36;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 10);
            Item.shoot = ModContent.ProjectileType<Projectiles.Rogue.DeathNeedle>();
            Item.shootSpeed = 17f;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
                return player.GetModPlayer<Summon.WikiWeapons1120bPlayer>().StingerCooldown <= 0;
            return player.ownedProjectileCounts[Item.shoot] < 3;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                player.GetModPlayer<Summon.WikiWeapons1120bPlayer>().StingerCooldown = 600;
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 1f);
                return false;
            }
            if (player.Calamity().StealthStrikeAvailable())
            {
                for (int i = -1; i <= 1; i++)
                {
                    int p = Projectile.NewProjectile(source, position, velocity.RotatedBy(i * 0.06f), type, damage * 2, knockback, player.whoAmI);
                    if (Main.projectile.IndexInRange(p))
                        Main.projectile[p].Calamity().stealthStrike = true;
                }
                player.Calamity().ConsumeStealthByAttacking();
                return false;
            }
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Potions
{
    /// <summary>
    /// Wiki : Leaves of Crabulon.txt — Leaves Power 4 h 20 : +6 % dégâts, +2 % critique, +5 % vitesse de minage, +100 % de
    /// recul des sbires, +15 % de vitesse, +6 % de réduction des dégâts, +8 défense (Crabulon).
    /// </summary>
    public class LeavesofCrabulon : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 20;

        public override void SetDefaults()
        {
            Item.width = Item.height = 32;
            Item.useTime = Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.UseSound = SoundID.Item2;
            Item.useTurn = true;
            Item.consumable = true;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(gold: 4, silver: 20);
            Item.buffType = ModContent.BuffType<Buffs.LeavesPower>();
            Item.buffTime = (4 * 60 + 20) * 3600;
        }
    }
}

namespace CalamityFandom.Content.Items.Accessories
{
    /// <summary>Wiki : Cosmic Stone.txt — invoque un Cosmic Elemental (20 dégâts) qui fonce sur les ennemis et tire des étoiles chercheuses.</summary>
    public class CosmicStone : WikiCovering
    {
        protected override int Rarity => ItemRarityID.Green;
        protected override int SellCopper => 4000;

        public override void UpdateAccessory(Player player, bool hideVisual)
            => Weapons.Summon.WikiWeapons1120bPlayer.KeepMinion(player, Item, ModContent.ProjectileType<Weapons.Summon.CosmicElementalMinion>(),
                ModContent.BuffType<Buffs.CosmicElementalBuff>(), 20);
    }

    /// <summary>
    /// Wiki : Oil Bottle.txt — invoque Cyn, qui repousse tous les ennemis proches (pas les boss) : 5 blocs toutes les 5 s,
    /// 10 blocs toutes les 4 s après le Moon Lord, 15 toutes les 3 s après The Devourer of Gods.
    /// </summary>
    public class OilBottle : WikiCovering
    {
        protected override int Rarity => ItemRarityID.Yellow;
        protected override int SellCopper => 90000;

        public override void UpdateAccessory(Player player, bool hideVisual)
            => Weapons.Summon.WikiWeapons1120bPlayer.KeepMinion(player, Item, ModContent.ProjectileType<Weapons.Summon.Cyn>(), ModContent.BuffType<Buffs.CynBuff>(), 0);
    }
}

namespace CalamityFandom.Content.Items.Weapons.Summon
{
    public class WikiWeapons1120bPlayer : ModPlayer
    {
        public int StingerCooldown;

        public override void PostUpdate()
        {
            if (StingerCooldown > 0)
                StingerCooldown--;
        }

        /// <summary>Sbire d'un accessoire : le fait apparaître s'il manque, garde son buff.</summary>
        public static void KeepMinion(Player player, Item item, int type, int buff, int baseDamage)
        {
            if (player.whoAmI != Main.myPlayer)
                return;
            player.AddBuff(buff, 2);
            if (player.ownedProjectileCounts[type] < 1)
            {
                int damage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(baseDamage);
                int p = Projectile.NewProjectile(player.GetSource_Accessory(item), player.Center, Vector2.Zero, type, damage, 2f, player.whoAmI);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].originalDamage = baseDamage;
            }
        }
    }

    /// <summary>
    /// Wiki : Profaned Whiplash.txt — fouet : 12 de dégâts de marque, explosion de Holy Flames à chaque coup (Holy Flames 5 s),
    /// un ennemi tué lâche 3 à 5 éclats de cristal sacré chercheurs (110 %) (Providence).
    /// </summary>
    public class ProfanedWhiplash : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.DefaultToWhip(ModContent.ProjectileType<Projectiles.Summon.WikiWhip1120>(), 200, 2f, 4f, 12);
            Item.rare = ItemRarityID.Cyan;
            Item.value = Item.sellPrice(gold: 15);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, Projectiles.Summon.WikiWhip1120.Profaned);
            return false;
        }
    }

    /// <summary>
    /// Wiki : Cirrhotipede.txt — fouet : 30 de dégâts de marque, Plague 10 s et Oiled 5 s ; laisse des nanites de peste qui
    /// foncent sur le dernier ennemi frappé et explosent (Plaguebringer Goliath). [choix] 3 nanites par coup de fouet, 25 %
    /// des dégâts, 3 s.
    /// </summary>
    public class Cirrhotipede : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.DefaultToWhip(ModContent.ProjectileType<Projectiles.Summon.WikiWhip1120>(), 200, 2f, 13f, 24);
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 10);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, Projectiles.Summon.WikiWhip1120.Cirrho);
            return false;
        }
    }

    /// <summary>
    /// Wiki : Plague Caller MK III.txt — prend toutes les places de sbire restantes pour invoquer le Plague Emperor, qui
    /// inonde les ennemis de missiles de peste, d'abeilles et de mini-Plaguebringers (Plague 3 s).
    /// [choix] Dégâts x places prises ; un missile toutes les 20 images, une abeille toutes les 10, un mini-Plaguebringer
    /// (qui fonce 3 fois) toutes les 90.
    /// </summary>
    public class PlagueCallerMKIII : WikiMinionStaff
    {
        protected override int Damage => 302;
        protected override int Rarity => ItemRarityID.Purple;
        protected override int SellCopper => 180000;
        protected override int BuffType => ModContent.BuffType<Buffs.PlagueEmperorBuff>();
        protected override int MinionType => ModContent.ProjectileType<PlagueEmperor>();
        protected override float Knockback => 5f;
        protected override int UseTime => 19;

        public override bool CanUseItem(Player player) => player.maxMinions - player.slotsMinions >= 1f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            foreach (Projectile old in Main.projectile)
                if (old.active && old.owner == player.whoAmI && old.type == type)
                    old.Kill();
            float slots = Math.Max(1f, (float)Math.Floor(player.maxMinions - player.slotsMinions));
            player.AddBuff(Item.buffType, 2);
            int p = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, (int)(damage * slots), knockback, player.whoAmI);
            if (Main.projectile.IndexInRange(p))
            {
                Main.projectile[p].minionSlots = slots;
                Main.projectile[p].originalDamage = (int)(Item.damage * slots);
            }
            return false;
        }
    }

    /// <summary>
    /// Wiki : Bad Apple.txt — 5 places, une seule fois : Utsuho Reiuji, qui répète un cycle : 2 salves de 16 orbes chercheuses
    /// (400 %), 3 grandes étoiles qui explosent (1500 %) en flammèches chercheuses (500 %), un plongeon explosif sur l'ennemi le
    /// plus proche (2000 %), 6 rayons chercheurs (500 %) ; Plague 4 s et Daybroken 2 s (Plaguebringer Goliath).
    /// [choix] Une attaque toutes les 2 s ; 8 flammèches par étoile.
    /// </summary>
    public class BadApple : WikiMinionStaff
    {
        protected override int Damage => 11;
        protected override int Rarity => ItemRarityID.Yellow;
        protected override int SellCopper => 160000;
        protected override int BuffType => ModContent.BuffType<Buffs.UtsuhoBuff>();
        protected override int MinionType => ModContent.ProjectileType<UtsuhoReiuji>();
        protected override float Knockback => 5f;
        protected override int UseTime => 15;

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[MinionType] <= 0 && player.maxMinions - player.slotsMinions >= 5f;
    }

    /// <summary>Cosmic Elemental (Cosmic Stone) : fonce sur les ennemis et tire des étoiles chercheuses.</summary>
    public class CosmicElementalMinion : WikiMinion
    {
        public override string Texture => "CalamityFandom/Content/Legacy/NPCs/NormalNPCs/CosmicElemental";
        protected override int BuffType => ModContent.BuffType<Buffs.CosmicElementalBuff>();
        protected override float Slots => 0f;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = 11;
        }

        public override void AI()
        {
            if (!KeepAlive())
                return;
            if (++Projectile.frameCounter % 6 == 0)
                Projectile.frame = (Projectile.frame + 1) % 4;
            NPC target = Projectile.Center.MinionHoming(800f, Owner);
            if (target == null)
            {
                Idle();
                return;
            }
            if (++Projectile.ai[0] % 45 == 0)
                Projectile.velocity = Projectile.SafeDirectionTo(target.Center) * 12f;
            else
                Projectile.velocity *= 0.97f;
            if (Projectile.owner == Main.myPlayer && Projectile.ai[0] % 60 == 30)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.SafeDirectionTo(target.Center) * 9f, ModContent.ProjectileType<CosmicStar>(),
                    Projectile.damage, 1f, Projectile.owner);
            Projectile.spriteDirection = Projectile.velocity.X > 0 ? 1 : -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Rectangle frame = tex.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, frame, lightColor, 0f, frame.Size() / 2f, 0.8f,
                Projectile.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            return false;
        }
    }

    /// <summary>Étoile chercheuse du Cosmic Elemental.</summary>
    public class CosmicStar : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.HallowStar;

        public override void SetStaticDefaults() => ProjectileID.Sets.MinionShot[Type] = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.scale = 0.6f;
        }

        public override void AI()
        {
            NPC t = Projectile.FindTargetWithinRange(500f);
            if (t != null)
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(t.Center) * 10f, 0.06f);
            Projectile.rotation += 0.2f;
        }
    }

    /// <summary>Cyn (Oil Bottle) : reste près du joueur et repousse les ennemis proches (pas les boss).</summary>
    public class Cyn : WikiMinion
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Summon/Cyn";
        protected override int BuffType => ModContent.BuffType<Buffs.CynBuff>();
        protected override float Slots => 0f;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.friendly = false;
            Projectile.scale = 0.6f;
        }

        public override void AI()
        {
            if (!KeepAlive())
                return;
            Idle();
            Projectile.spriteDirection = Owner.direction;
            bool dog = DownedBossSystem.downedDoG, ml = NPC.downedMoonlord;
            float radius = (dog ? 15 : ml ? 10 : 5) * 16f;
            int every = (dog ? 3 : ml ? 4 : 5) * 60;
            if (++Projectile.ai[0] % every != 0 || Main.netMode == NetmodeID.MultiplayerClient)
                return;
            // « Absolute translate » : tous les ennemis du rayon sont projetés au loin
            foreach (NPC n in Main.npc)
                if (n.active && !n.friendly && !n.boss && n.lifeMax > 5 && n.Distance(Owner.Center) < radius + Math.Max(n.width, n.height) / 2f && n.knockBackResist > 0f)
                {
                    n.velocity = (n.Center - Owner.Center).SafeNormalize(Vector2.UnitX) * 16f;
                    n.netUpdate = true;
                }
            for (int i = 0; i < 40; i++)
            {
                Dust d = Dust.NewDustPerfect(Owner.Center + (MathHelper.TwoPi * i / 40f).ToRotationVector2() * radius, DustID.PurpleTorch, Vector2.Zero, 0, default, 1.5f);
                d.noGravity = true;
            }
            SoundEngine.PlaySound(SoundID.Item8, Owner.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawFlipped(lightColor);
            return false;
        }
    }

    /// <summary>Plague Emperor (Plague Caller MK III) : plane près de la cible et l'inonde de missiles, d'abeilles et de mini-Plaguebringers.</summary>
    public class PlagueEmperor : WikiMinion
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Summon/PlagueEmperor";
        protected override int BuffType => ModContent.BuffType<Buffs.PlagueEmperorBuff>();

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 80;
            Projectile.scale = 0.5f;
        }

        public override void AI()
        {
            if (!KeepAlive())
                return;
            NPC target = Projectile.Center.MinionHoming(1400f, Owner);
            if (target == null)
            {
                Idle();
                return;
            }
            Vector2 spot = target.Center + new Vector2(Projectile.Center.X < target.Center.X ? -300f : 300f, -150f);
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, (spot - Projectile.Center) * 0.06f, 0.15f);
            Projectile.spriteDirection = target.Center.X > Projectile.Center.X ? 1 : -1;
            if (Projectile.owner != Main.myPlayer)
                return;
            var source = Projectile.GetSource_FromThis();
            Projectile.ai[0]++;
            if (Projectile.ai[0] % 20 == 0)
                Projectile.NewProjectile(source, Projectile.Center, Projectile.SafeDirectionTo(target.Center).RotatedByRandom(0.4f) * 10f, ModContent.ProjectileType<PlagueMissile>(), Projectile.damage / 2, 2f, Projectile.owner);
            if (Projectile.ai[0] % 10 == 5)
            {
                int bee = Projectile.NewProjectile(source, Projectile.Center, Main.rand.NextVector2Circular(5f, 5f), ProjectileID.Bee, Projectile.damage / 6, 0f, Projectile.owner);
                if (Main.projectile.IndexInRange(bee))
                    Main.projectile[bee].DamageType = DamageClass.Summon;
            }
            if (Projectile.ai[0] % 90 == 45)
                Projectile.NewProjectile(source, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<MiniPlaguebringer>(), Projectile.damage / 2, 2f, Projectile.owner);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawFlipped(lightColor);
            return false;
        }
    }

    /// <summary>Missile de peste : chercheur, explose (Plague 3 s).</summary>
    public class PlagueMissile : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.RocketI;

        public override void SetStaticDefaults() => ProjectileID.Sets.MinionShot[Type] = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 150;
        }

        public override void AI()
        {
            NPC t = Projectile.FindTargetWithinRange(900f);
            if (t != null)
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(t.Center) * 14f, 0.06f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Dust.NewDustPerfect(Projectile.Center, DustID.JungleSpore, Vector2.Zero, 0, default, 1f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Plague>(), 180);

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14 with { Volume = 0.4f }, Projectile.Center);
            for (int i = 0; i < 10; i++)
                Dust.NewDustPerfect(Projectile.Center, DustID.JungleSpore, Main.rand.NextVector2Circular(4f, 4f), 0, default, 1.4f).noGravity = true;
        }
    }

    /// <summary>Mini-Plaguebringer : fonce trois fois sur les ennemis (Plague 3 s), puis disparaît.</summary>
    public class MiniPlaguebringer : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Summon/PlaguebringerBab";

        public override void SetStaticDefaults() => ProjectileID.Sets.MinionShot[Type] = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 200;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            if (++Projectile.ai[0] % 50 == 1)
            {
                NPC t = Projectile.FindTargetWithinRange(900f);
                if (t != null)
                    Projectile.velocity = Projectile.SafeDirectionTo(t.Center) * 16f;
            }
            else
                Projectile.velocity *= 0.97f;
            Projectile.spriteDirection = Projectile.velocity.X > 0 ? 1 : -1;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Plague>(), 180);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor, 0f, tex.Size() / 2f, 0.8f,
                Projectile.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            return false;
        }
    }

    /// <summary>Utsuho Reiuji (Bad Apple) : cycle d'attaques (orbes, étoiles, plongeon, rayons) toutes les 2 s.</summary>
    public class UtsuhoReiuji : WikiMinion
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Summon/UtsuhoReiuji";
        protected override int BuffType => ModContent.BuffType<Buffs.UtsuhoBuff>();
        protected override float Slots => 5f;

        private int Damage(float percent) => Math.Max(1, (int)(Projectile.damage * percent));

        public override void AI()
        {
            if (!KeepAlive())
                return;
            Lighting.AddLight(Projectile.Center, 1f, 0.7f, 0.3f);
            NPC target = Projectile.Center.MinionHoming(1200f, Owner);
            // Plongeon en cours : fonce, explose au contact
            if (Projectile.ai[1] > 0f)
            {
                if (--Projectile.ai[1] <= 0f || target == null)
                    Projectile.ai[1] = 0f;
                else
                    Projectile.velocity = Projectile.SafeDirectionTo(target.Center) * 24f;
                return;
            }
            if (target == null)
            {
                Idle();
                return;
            }
            Vector2 spot = target.Center + new Vector2(0f, -250f);
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, (spot - Projectile.Center) * 0.07f, 0.15f);
            Projectile.spriteDirection = target.Center.X > Projectile.Center.X ? 1 : -1;
            if (Projectile.owner != Main.myPlayer || ++Projectile.ai[0] % 120 != 0)
                return;
            var source = Projectile.GetSource_FromThis();
            int step = (int)(Projectile.ai[0] / 120) % 4;
            switch (step)
            {
                case 0:
                    for (int wave = 0; wave < 2; wave++)
                        for (int i = 0; i < 16; i++)
                            Projectile.NewProjectile(source, Projectile.Center, (MathHelper.TwoPi * (i + wave * 0.5f) / 16f).ToRotationVector2() * (6f + wave * 2f),
                                ModContent.ProjectileType<NuclearOrb>(), Damage(4f), 1f, Projectile.owner);
                    break;
                case 1:
                    for (int i = -1; i <= 1; i++)
                        Projectile.NewProjectile(source, Projectile.Center + new Vector2(i * 120f, -60f), Projectile.SafeDirectionTo(target.Center) * 6f,
                            ModContent.ProjectileType<NuclearStar>(), Damage(15f), 3f, Projectile.owner, Damage(5f));
                    break;
                case 2:
                    Projectile.ai[1] = 40f;
                    break;
                default:
                    for (int i = 0; i < 6; i++)
                        Projectile.NewProjectile(source, Projectile.Center, (Projectile.SafeDirectionTo(target.Center)).RotatedBy(MathHelper.ToRadians(-50f + 20f * i)) * 14f,
                            ModContent.ProjectileType<NuclearRay>(), Damage(5f), 1f, Projectile.owner);
                    break;
            }
            SoundEngine.PlaySound(SoundID.Item117 with { Volume = 0.5f }, Projectile.Center);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.ai[1] > 0f)
                modifiers.SourceDamage *= 20f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 240);
            target.AddBuff(BuffID.Daybreak, 120);
            if (Projectile.ai[1] > 0f)
            {
                Projectile.ai[1] = 0f;
                for (int i = 0; i < 30; i++)
                    Dust.NewDustPerfect(Projectile.Center, DustID.SolarFlare, Main.rand.NextVector2Circular(8f, 8f), 0, default, 1.6f).noGravity = true;
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawFlipped(Color.White);
            return false;
        }
    }

    /// <summary>Tirs d'Utsuho : base commune (sbire, Plague 4 s, Daybroken 2 s).</summary>
    public abstract class NuclearShot : ModProjectile
    {
        public override void SetStaticDefaults() => ProjectileID.Sets.MinionShot[Type] = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
        }

        protected void Home(float speed, float lerp, int delay)
        {
            if (++Projectile.localAI[0] < delay)
                return;
            NPC t = Projectile.FindTargetWithinRange(1000f);
            if (t != null)
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(t.Center) * speed, lerp);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 240);
            target.AddBuff(BuffID.Daybreak, 120);
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 200, 120, 100);
    }

    /// <summary>Orbe nucléaire : cherche fort après une demi-seconde.</summary>
    public class NuclearOrb : NuclearShot
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.DD2PhoenixBowShot;

        public override void AI()
        {
            Projectile.velocity *= Projectile.localAI[0] < 30 ? 0.96f : 1f;
            Home(16f, 0.12f, 30);
        }
    }

    /// <summary>Grande étoile : explose au bout d'1 s ou au contact en 8 flammèches chercheuses (ai[0] = leurs dégâts).</summary>
    public class NuclearStar : NuclearShot
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.StarWrath;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 40;
            Projectile.timeLeft = 60;
            Projectile.scale = 1.6f;
        }

        public override void AI() => Projectile.rotation += 0.2f;

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            if (Projectile.owner != Main.myPlayer)
                return;
            for (int i = 0; i < 8; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (MathHelper.TwoPi * i / 8f).ToRotationVector2() * 8f, ModContent.ProjectileType<NuclearFlare>(),
                    (int)Projectile.ai[0], 1f, Projectile.owner);
        }
    }

    public class NuclearFlare : NuclearShot
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.DD2PhoenixBowShot;

        public override void AI() => Home(13f, 0.08f, 15);
    }

    /// <summary>Rayon d'énergie : rapide, chercheur, traverse 2 ennemis.</summary>
    public class NuclearRay : NuclearShot
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.PurpleLaser;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.penetrate = 2;
            Projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            Home(14f, 0.05f, 0);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }
    }

    public class WikiWeapons1120bChecks : ModSystem
    {
        public override void Load()
        {
            SelfTest.ExtraChecks.Add(t =>
            {
                // Butin du wiki : ces objets existent, donc les PNJ qui les lâchent sur le wiki les lâchent en jeu
                foreach (string name in new[] { "Bad Apple", "Cirrhotipede", "Plague Caller MK III", "Profaned Whiplash", "Holy Purge", "Holiday Halberd",
                    "Leaves of Crabulon", "Cosmic Stone", "The Last Lightice", "Death-Needle Stinger" })
                    t.Expect(Common.WikiData.WikiLoot.Added.Any(a => a.what.StartsWith(name)), $"butin du wiki : {name}");
            });
        }
    }
}

namespace CalamityFandom.Content.Buffs
{
    public class LeavesPower : ModBuff
    {
        public override string Texture => "CalamityFandom/Content/Buffs/Wiki/LeavesPower";

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage<GenericDamageClass>() += 0.06f;
            player.GetCritChance<GenericDamageClass>() += 2;
            player.pickSpeed -= 0.05f;
            player.GetKnockback<SummonDamageClass>() += 1f;
            player.moveSpeed += 0.15f;
            player.endurance += 0.06f;
            player.statDefense += 8;
        }
    }

    public class CosmicElementalBuff : WikiMinionBuff
    {
        public override string Texture => "CalamityFandom/Content/Buffs/Wiki/CosmicElementalBuff";
        protected override int MinionType => ModContent.ProjectileType<Items.Weapons.Summon.CosmicElementalMinion>();
    }

    public class CynBuff : WikiMinionBuff
    {
        public override string Texture => "CalamityFandom/Content/Buffs/Wiki/CynBuff";
        protected override int MinionType => ModContent.ProjectileType<Items.Weapons.Summon.Cyn>();
    }

    public class PlagueEmperorBuff : WikiMinionBuff
    {
        public override string Texture => "CalamityFandom/Content/Buffs/Wiki/PlagueEmperorBuff";
        protected override int MinionType => ModContent.ProjectileType<Items.Weapons.Summon.PlagueEmperor>();
    }

    /// <summary>[technique] Pas d'icône sur le wiki : celle du buff du Plague Emperor en attendant (docs/SPRITES.md).</summary>
    public class UtsuhoBuff : WikiMinionBuff
    {
        public override string Texture => "CalamityFandom/Content/Buffs/Wiki/PlagueEmperorBuff";
        protected override int MinionType => ModContent.ProjectileType<Items.Weapons.Summon.UtsuhoReiuji>();
    }
}

namespace CalamityFandom.Content.Projectiles.Ranged
{
    /// <summary>Flamme du Holy Purge : traverse tous les ennemis, s'arrête sur les blocs, Holy Flames 3 s.</summary>
    public class HolyPurgeFlame : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 50;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Projectile.scale = 1f + (50 - Projectile.timeLeft) / 25f;
            if (Main.rand.NextBool(2))
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8f, 8f) * Projectile.scale, Main.rand.NextBool() ? DustID.SolarFlare : DustID.GoldFlame,
                    Projectile.velocity * 0.3f, 0, default, 1.5f);
                d.noGravity = true;
            }
            Lighting.AddLight(Projectile.Center, 1f, 0.7f, 0.2f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
    }
}

namespace CalamityFandom.Content.Projectiles.Melee
{
    /// <summary>Sac cadeau (Holiday Halberd) : ai[0] = 1 vert (Cursed Inferno 2 s), 0 rouge (On Fire! 5 s) ; deux rebonds ou deux ennemis.</summary>
    public class GiftBag : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Melee/RedBag";

        private bool Green => Projectile.ai[0] == 1f;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.3f, 14f);
            Projectile.rotation += Projectile.velocity.X * 0.03f;
            if (Projectile.owner == Main.myPlayer && ++Projectile.localAI[0] % 10 == 0)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GiftCloud>(), Projectile.damage / 4, 0f, Projectile.owner, Projectile.ai[0]);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (++Projectile.ai[1] >= 2f)
                return true;
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y * 0.7f;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
            => target.AddBuff(Green ? BuffID.CursedInferno : BuffID.OnFire, Green ? 120 : 300);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Green ? "CalamityFandom/Content/Projectiles/Melee/GreenBag" : Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, 1f, SpriteEffects.None);
            return false;
        }
    }

    /// <summary>Nuage de spores des sacs : 1 s, même debuff que le sac.</summary>
    public class GiftCloud : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(3))
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(18f, 18f), Projectile.ai[0] == 1f ? DustID.CursedTorch : DustID.Torch, Vector2.Zero, 100, default, 1.2f);
                d.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
            => target.AddBuff(Projectile.ai[0] == 1f ? BuffID.CursedInferno : BuffID.OnFire, Projectile.ai[0] == 1f ? 120 : 300);
    }
}

namespace CalamityFandom.Content.Projectiles.Rogue
{
    /// <summary>Glace magique (The Last Lightice) : s'arrête et reste 3 s sur un bloc ou un ennemi ; Glacial State 3 s.</summary>
    public class LightIce : ModProjectile
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.IceBlock;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = 2;
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        private void Settle()
        {
            if (Projectile.ai[0] == 1f)
                return;
            Projectile.ai[0] = 1f;
            Projectile.velocity = Vector2.Zero;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            SoundEngine.PlaySound(SoundID.Item50 with { Volume = 0.4f }, Projectile.Center);
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0f)
            {
                Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.15f, 12f);
                Projectile.rotation += 0.2f;
            }
            Lighting.AddLight(Projectile.Center, 0.3f, 0.5f, 0.8f);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Settle();
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<GlacialState>(), 180);
            Settle();
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 6; i++)
                Dust.NewDustPerfect(Projectile.Center, DustID.Ice, Main.rand.NextVector2Circular(3f, 3f));
        }
    }

    /// <summary>
    /// Aiguille (ai[0] = 0) ou dard (ai[0] = 1) de la Death-Needle Stinger, reliés au joueur par une chaîne. ai[1] = PNJ où
    /// l'aiguille est plantée + 1 ; localAI[0] = temps planté ; localAI[1] = 1 au retour.
    /// </summary>
    public class DeathNeedle : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Rogue/DeathNeedleStinger";

        private bool Stinger => Projectile.ai[0] == 1f;
        private Player Owner => Main.player[Projectile.owner];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            float range = Stinger ? 250f : 600f;
            if (Projectile.ai[1] > 0f)
            {
                NPC n = Main.npc[(int)Projectile.ai[1] - 1];
                if (!n.active || ++Projectile.localAI[0] > 120 || Projectile.Distance(Owner.Center) > 900f)
                {
                    Projectile.ai[1] = 0f;
                    Projectile.localAI[1] = 1f;
                }
                else
                {
                    Projectile.Center = n.Center;
                    // Frappe furtive : l'ennemi est tiré vers le joueur (le joueur vers un boss)
                    if (Projectile.Calamity().stealthStrike)
                    {
                        if (n.boss)
                            Owner.velocity = Vector2.Lerp(Owner.velocity, Owner.SafeDirectionTo(n.Center) * 12f, 0.1f);
                        else if (n.Distance(Owner.Center) > 60f)
                            n.position += n.SafeDirectionTo(Owner.Center) * 8f;
                    }
                    return;
                }
            }
            if (Projectile.localAI[1] == 0f && Projectile.Distance(Owner.Center) > range)
                Projectile.localAI[1] = 1f;
            if (Projectile.localAI[1] == 1f)
            {
                Projectile.velocity = Projectile.SafeDirectionTo(Owner.Center) * 24f;
                if (Projectile.Distance(Owner.Center) < 30f)
                    Projectile.Kill();
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Stinger && Projectile.Calamity().stealthStrike)
                modifiers.SourceDamage *= 3f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Stinger)
            {
                target.AddBuff(ModContent.BuffType<WhisperingDeath>(), 180);
                Projectile.localAI[1] = 1f;
                return;
            }
            target.AddBuff(ModContent.BuffType<Plague>(), 120);
            if (Projectile.Calamity().stealthStrike)
                target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 300);
            if (Projectile.ai[1] == 0f && Projectile.localAI[1] == 0f)
            {
                Projectile.ai[1] = target.whoAmI + 1;
                Projectile.netUpdate = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Chaîne
            Vector2 from = Owner.MountedCenter, to = Projectile.Center;
            float length = Vector2.Distance(from, to);
            Main.EntitySpriteDraw(TextureAssets.MagicPixel.Value, from - Main.screenPosition, new Rectangle(0, 0, 1, 1), new Color(120, 160, 60) * 0.8f,
                (to - from).ToRotation(), new Vector2(0f, 0.5f), new Vector2(length, 2f), SpriteEffects.None);
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(tex, to - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, Stinger ? 0.8f : 0.6f, SpriteEffects.None);
            return false;
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Summon
{
    /// <summary>Fouets du lot AS : ai[1] = Profaned (Profaned Whiplash) ou Cirrho (Cirrhotipede).</summary>
    public class WikiWhip1120 : ModProjectile
    {
        public const float Profaned = 0f, Cirrho = 1f;

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults() => ProjectileID.Sets.IsAWhip[Type] = true;

        public override void SetDefaults()
        {
            Projectile.DefaultToWhip();
            Projectile.WhipSettings.Segments = 20;
            Projectile.WhipSettings.RangeMultiplier = 1.2f;
        }

        public override void AI()
        {
            // Cirrhotipede : 3 nanites de peste au bout du fouet, au milieu du coup
            if (Projectile.ai[1] == Cirrho && Projectile.owner == Main.myPlayer && Projectile.localAI[0] == 0f)
            {
                float time = Main.player[Projectile.owner].itemAnimationMax * Projectile.MaxUpdates;
                if (Projectile.ai[0] >= time / 2f)
                {
                    Projectile.localAI[0] = 1f;
                    var points = new List<Vector2>();
                    Projectile.FillWhipControlPoints(Projectile, points);
                    for (int i = 0; i < 3; i++)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), points[^1] + Main.rand.NextVector2Circular(20f, 20f), Main.rand.NextVector2Circular(2f, 2f),
                            ModContent.ProjectileType<PlagueNanite>(), Projectile.damage / 4, 0f, Projectile.owner);
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player owner = Main.player[Projectile.owner];
            owner.MinionAttackTargetNPC = target.whoAmI;
            var tag = target.GetGlobalNPC<Buffs.WikiWhipTag>();
            if (Projectile.ai[1] == Cirrho)
            {
                tag.Mark(30);
                target.AddBuff(ModContent.BuffType<Plague>(), 600);
                target.AddBuff(BuffID.Oiled, 300);
                return;
            }
            tag.Mark(12);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 300);
            if (Projectile.owner != Main.myPlayer)
                return;
            var blast = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<WikiDarts.DartBlast>(),
                Math.Max(1, damageDone / 3), 0f, Projectile.owner, 60f, DustID.GoldFlame, ModContent.BuffType<HolyFlames>());
            blast.DamageType = DamageClass.Summon;
            if (target.life <= 0)
                for (int i = 0, n = Main.rand.Next(3, 6); i < n; i++)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Main.rand.NextVector2Unit() * 8f, ModContent.ProjectileType<HolyCrystalShard>(),
                        (int)(Projectile.damage * 1.1f), 1f, Projectile.owner);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var points = new List<Vector2>();
            Projectile.FillWhipControlPoints(Projectile, points);
            Color color = Projectile.ai[1] == Cirrho ? new Color(140, 200, 60) : new Color(255, 200, 90);
            for (int i = 0; i < points.Count - 1; i++)
            {
                Vector2 diff = points[i + 1] - points[i];
                Main.EntitySpriteDraw(TextureAssets.MagicPixel.Value, points[i] - Main.screenPosition, new Rectangle(0, 0, 1, 1), color,
                    diff.ToRotation(), new Vector2(0f, 0.5f), new Vector2(diff.Length() + 1f, i == points.Count - 2 ? 6f : 3f), SpriteEffects.None);
            }
            return false;
        }
    }

    /// <summary>Nanite de peste (Cirrhotipede) : mine chercheuse vers l'ennemi frappé par le fouet, explose au contact ou après 3 s.</summary>
    public class PlagueNanite : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults() => ProjectileID.Sets.MinionShot[Type] = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            int target = Main.player[Projectile.owner].MinionAttackTargetNPC;
            if (target >= 0 && Main.npc[target].active && Projectile.timeLeft < 160)
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(Main.npc[target].Center) * 10f, 0.06f);
            else
                Projectile.velocity *= 0.95f;
            Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.JungleSpore, Vector2.Zero, 0, default, 1.1f);
            d.noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Plague>(), 300);

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
                Dust.NewDustPerfect(Projectile.Center, DustID.JungleSpore, Main.rand.NextVector2Circular(3f, 3f), 0, default, 1.3f).noGravity = true;
        }
    }

    /// <summary>Éclat de cristal sacré (Profaned Whiplash) : chercheur.</summary>
    public class HolyCrystalShard : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Summon/HolyCrystalShard";

        public override void SetStaticDefaults() => ProjectileID.Sets.MinionShot[Type] = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 150;
            Projectile.scale = 0.6f;
        }

        public override void AI()
        {
            NPC t = Projectile.FindTargetWithinRange(700f);
            if (t != null)
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(t.Center) * 13f, 0.07f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
    }
}
