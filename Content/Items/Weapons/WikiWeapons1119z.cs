using System;
using System.Collections.Generic;
using CalamityFandom.Content.Buffs;
using CalamityFandom.Content.Legacy.Buffs.StatDebuffs;
using CalamityFandom.Content.Projectiles.Weapons1119z;
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
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

// Lot AE (19/11) : armes fabricables du wiki (recettes du wiki). Raretés « C2 » → 0 (règle des raretés inventées).

namespace CalamityFandom.Content.Items.Weapons.Magic
{
    /// <summary>
    /// Wiki : Calamitous Vortex.txt — tenue : un grand vortex arc-en-ciel et 4 tentacules autour du joueur ; les ennemis à
    /// moins de 20 blocs sont aspirés et ne font plus de dégâts de contact ; les tentacules fouettent les ennemis. Disparaît
    /// sans mana ou quand on lâche l'arme. Exo Freeze 0,5 s, Holy Flames 2 s, Frostburn 2,5 s, On Fire! 3 s.
    /// [choix] 78 mana par seconde ; les tentacules frappent les ennemis à moins de 10 blocs toutes les 15 images ; les boss
    /// ne sont pas aspirés.
    /// </summary>
    public class CalamitousVortex : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 1020;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 78;
            Item.useTime = Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.channel = true;
            Item.knockBack = 0f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item84;
            Item.width = 38;
            Item.height = 48;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
            Item.value = Item.sellPrice(gold: 30);
            Item.shoot = ModContent.ProjectileType<CalamitousVortexField>();
            Item.shootSpeed = 1f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;
    }

    /// <summary>
    /// Wiki : Lacelinous Nocturne.txt — un grand triangle lent qui tire des balles derrière lui ; les balles explosent au
    /// contact d'un ennemi ou d'un bloc. Evercold 50 %. [choix] Une balle (50 %) toutes les 8 images, explosion de 64 px ;
    /// triangle de 4 s qui perce. [technique] « Lacelic Web » (sans page) non fait.
    /// </summary>
    public class LacelinousNocturne : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 1220;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 82;
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7.5f;
            Item.UseSound = SoundID.Item20;
            Item.width = Item.height = 96;
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(platinum: 2);
            Item.shoot = ModContent.ProjectileType<LacelinousTriangle>();
            Item.shootSpeed = 2f;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Ranged
{
    /// <summary>
    /// Wiki : Dead Star's Laser Rifle.txt — rayon qui perce à l'infini et change de couleur à chaque tir (pas de tir
    /// automatique) : bleu Glacial State ; orange ignore l'invulnérabilité, enflamme, l'ennemi explose à sa mort ; jaune
    /// explose à l'impact et laisse quelques étincelles (80 %) ; vert +5 de régénération et 33 % de chances d'un orbe qui
    /// soigne 10 PV. [choix] Explosions de 160 px (100 %) ; 3 étincelles ; régénération pendant 3 s. Le butin d'Astrum Deus
    /// (100 % en Death, dernier ver) : <see cref="DeadStarDrop"/> ; la recette du wiki demande une « Spirit of the
    /// Nightmare » qui n'a ni page ni sprite (elle ne s'applique donc pas).
    /// </summary>
    public class DeadStarsLaserRifle : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 250;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0f;
            Item.autoReuse = false;
            Item.UseSound = SoundID.Item12;
            Item.width = 96;
            Item.height = 50;
            Item.rare = ItemRarityID.Cyan;
            Item.value = Item.sellPrice(gold: 50);
            Item.shoot = ModContent.ProjectileType<StarkillerBeam>();
            Item.shootSpeed = 25f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var mp = player.GetModPlayer<Summon.WikiWeaponsZPlayer>();
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, mp.LaserColor);
            mp.LaserColor = (mp.LaserColor + 1) % 4;
            return false;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-14f, 0f);
    }

    /// <summary>Astrum Deus (dernier ver) lâche la Dead Star's Laser Rifle à 100 % en Death.</summary>
    public class DeadStarDrop : GlobalNPC
    {
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == ModContent.NPCType<CalamityMod.NPCs.AstrumDeus.AstrumDeusHead>();

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            var rule = new Terraria.GameContent.ItemDropRules.LeadingConditionRule(CalamityMod.DropHelper.If(info =>
                CalamityMod.World.CalamityWorld.death && !CalamityMod.NPCs.AstrumDeus.AstrumDeusHead.ShouldNotDropThings(info.npc)));
            rule.OnSuccess(Terraria.GameContent.ItemDropRules.ItemDropRule.Common(ModContent.ItemType<DeadStarsLaserRifle>()));
            npcLoot.Add(rule);
            // Butin donné par la page de l'arme, pas par celle d'Astrum Deus : gardé en Full Fandom
            Common.WikiData.WikiLoot.Keep.Add(rule);
        }
    }

    /// <summary>
    /// Wiki : Grarble Bayonet.txt — tire des balles ; l'arme elle-même blesse les ennemis qui la touchent (mêmes dégâts,
    /// distance). [choix] Zone de contact de 40 px au bout du canon pendant le tir.
    /// </summary>
    public class GrarbleBayonet : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 28;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.25f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item11;
            Item.width = 66;
            Item.height = 18;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 20);
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 22f;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<BayonetBlade>()] < 1)
                Projectile.NewProjectile(source, player.Center, velocity.SafeNormalize(Vector2.UnitX), ModContent.ProjectileType<BayonetBlade>(), Item.damage, knockback, player.whoAmI);
            return true;
        }
    }

    /// <summary>
    /// Wiki : Holy Napalm Launcher.txt — lance lentement des roquettes au napalm qui volent 5 s en laissant des flammes sacrées
    /// (25 %), puis explosent sur 20x20 blocs (300 %). Nightclear, Holy Flames, Evercold 5 s. [choix] Utilise des roquettes
    /// (remplacées par le napalm) ; une flamme toutes les 6 images ; la roquette explose aussi au contact.
    /// </summary>
    public class HolyNapalmLauncher : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 4000;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 72;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 15f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item61;
            Item.width = 112;
            Item.height = 50;
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(platinum: 50);
            Item.shoot = ProjectileID.RocketI;
            Item.shootSpeed = 6f;
            Item.useAmmo = AmmoID.Rocket;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
            => type = ModContent.ProjectileType<HolyNapalm>();

        public override Vector2? HoldoutOffset() => new Vector2(-24f, 0f);
    }
}

namespace CalamityFandom.Content.Items.Weapons.Rogue
{
    /// <summary>
    /// Wiki : Diminishing Victory.txt — comme le Paladin's Hammer (marteau qui revient), Brimstone Flames 5 s ; frappe furtive
    /// (toute la jauge) : la faux est lancée comme un boomerang lent qui perce à l'infini et revient bien plus lentement.
    /// [choix] Vitesse 20 (le wiki dit 100) ; normal : perce 3 fois ; furtif : vitesse 8, 4 s avant le retour.
    /// </summary>
    public class DiminishingVictory : RogueWeapon
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = Item.height = 80;
            Item.damage = 3750;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.knockBack = 6.75f;
            Item.crit = 4;
            Item.useTime = Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<DiminishingScythe>();
            Item.shootSpeed = 20f;
            Item.value = Item.sellPrice(gold: 16);
            Item.rare = ModContent.RarityType<BurnishedAuric>();
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool stealth = player.Calamity().StealthStrikeAvailable();
            int p = Projectile.NewProjectile(source, position, stealth ? velocity.SafeNormalize(Vector2.UnitX) * 8f : velocity, type, damage, knockback, player.whoAmI);
            if (stealth && p.WithinBounds(Main.maxProjectiles))
                Main.projectile[p].Calamity().stealthStrike = true;
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Grand Reef.txt — fléau (amélioration du Clam Crusher) : lance un gros bloc de Voidstone qui fait jaillir 30 à 46
    /// éclats de corail mort en touchant un ennemi ou un bloc, puis revient ; s'il retombe après avoir été lancé en l'air :
    /// 7 fois plus de dégâts et 58 à 72 éclats. [choix] Éclats à 5 % qui restent 2 s ; « retombe » = touche en descendant
    /// après avoir monté.
    /// </summary>
    public class GrandReef : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 5160;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 50;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = Item.noUseGraphic = true;
            Item.knockBack = 10f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = 90;
            Item.height = 98;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.value = Item.sellPrice(gold: 30);
            Item.shoot = ModContent.ProjectileType<GrandReefChunk>();
            Item.shootSpeed = 18f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;
    }
}

namespace CalamityFandom.Content.Items.Weapons.Summon
{
    /// <summary>
    /// Wiki : Duet Hit.txt — fouet : 2 fouets à la fois (feu et givre), 10 de dégâts de marquage ; un coup crée un flocon (de
    /// givre ou de feu) qui chasse les ennemis proches (20 dégâts, ignore 30 défense ; Frostbite ou Hellfire, 50 %, 5 s),
    /// 3 s ; un nouveau coup remet le temps à zéro au lieu d'en créer un autre. Le flocon reste immobile sans ennemi.
    /// </summary>
    public class DuetHit : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.DefaultToWhip(ModContent.ProjectileType<WikiWhip>(), 45, 7f, 4f, 25);
            Item.width = 52;
            Item.height = 46;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 5);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, WikiWhip.DuetFire);
            Projectile.NewProjectile(source, position, velocity.RotatedBy(0.15f), type, damage, knockback, player.whoAmI, 0f, WikiWhip.DuetFrost);
            return false;
        }

        public override bool MeleePrefix() => true;
    }

    /// <summary>
    /// Wiki : Miracle Smacker.txt — fouet : 3 fouets, 22 de dégâts de marquage. Rouge : On Fire! 3 s, les coups des sbires
    /// sur la cible font des explosions ; vert : Holy Flames 3 s, vole 4 PV ; bleu : Exo Freeze 0,5 s et Frostburn 3 s, +5
    /// défense et +2 % de réduction des dégâts. [choix] Explosions des sbires : 64 px, 50 % du coup, une toutes les 10 images
    /// au plus ; bonus du bleu pendant 5 s.
    /// </summary>
    public class MiracleSmacker : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.DefaultToWhip(ModContent.ProjectileType<WikiWhip>(), 1000, 4f, 4f, 22);
            Item.width = 42;
            Item.height = 64;
            Item.autoReuse = true;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
            Item.value = Item.sellPrice(gold: 30);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 3; i++)
                Projectile.NewProjectile(source, position, velocity.RotatedBy((i - 1) * 0.15f), type, damage, knockback, player.whoAmI, 0f, WikiWhip.MiracleRed + i);
            return false;
        }

        public override bool MeleePrefix() => true;
    }

    /// <summary>
    /// Wiki : EXO-LOTL Staff.txt — un axolotl blindé avec un canon sur le dos, qui saute sur les ennemis en les visant et leur
    /// tire des lasers ; Exo Freeze 3 s. [choix] Un laser toutes les 30 images ; 1 emplacement de sbire.
    /// </summary>
    public class EXOLOTLStaff : WikiMinionStaff
    {
        protected override int Damage => 1;
        protected override int Rarity => ModContent.RarityType<BurnishedAuric>();
        protected override int SellCopper => 300000;
        protected override int BuffType => ModContent.BuffType<Buffs.EXOLOTLBuff>();
        protected override int MinionType => ModContent.ProjectileType<EXOLOTL>();
        protected override float Knockback => 4f;
        protected override int UseTime => 9;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.mana = 500;
            Item.width = 68;
            Item.height = 74;
        }
    }

    /// <summary>
    /// Wiki : Evercoldance.txt — tourelle : un cristal qui tire des rayons (110 %) sur les ennemis les plus proches ; 25 % de
    /// chances chacun d'Evercold, Moon Burnt (= Nightwither) et Crush Depth, 5 s. [choix] Un rayon toutes les 30 images sur
    /// les 2 ennemis les plus proches (800 px).
    /// </summary>
    public class Evercoldance : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.GamepadWholeScreenUseRange[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 840;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 64;
            Item.useTime = Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 7.5f;
            Item.UseSound = SoundID.Item44;
            Item.width = Item.height = 62;
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(gold: 75);
            Item.sentry = true;
            Item.shoot = ModContent.ProjectileType<EvercoldCrystal>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var p = Projectile.NewProjectileDirect(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI);
            p.originalDamage = Item.damage;
            player.UpdateMaxTurrets();
            return false;
        }
    }

    public class WikiWeaponsZPlayer : ModPlayer
    {
        public int LaserColor, RegenTime, BlueWhipTime;

        public override void ResetEffects()
        {
            if (RegenTime > 0)
                RegenTime--;
            if (BlueWhipTime > 0)
                BlueWhipTime--;
        }

        public override void UpdateLifeRegen()
        {
            if (RegenTime > 0)
                Player.lifeRegen += 5;
        }

        public override void PostUpdateEquips()
        {
            if (BlueWhipTime > 0)
            {
                Player.statDefense += 5;
                Player.endurance += 0.02f;
            }
        }
    }
}

namespace CalamityFandom.Content.Buffs
{
    public class EXOLOTLBuff : WikiMinionBuff
    {
        public override string Texture => "CalamityFandom/Content/Buffs/Wiki/EXOLOTLBuff";

        protected override int MinionType => ModContent.ProjectileType<EXOLOTL>();
    }

    /// <summary>Marquage des fouets du wiki : dégâts de plus pour les sbires (4 s) ; explosions (Miracle Smacker rouge).</summary>
    public class WikiWhipTag : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public int Tag, TagTime, ExplosiveTime, ExplosionCooldown;

        public void Mark(int tag)
        {
            Tag = TagTime > 0 ? Math.Max(Tag, tag) : tag;
            TagTime = 240;
        }

        public override void ResetEffects(NPC npc)
        {
            if (TagTime > 0)
                TagTime--;
            if (ExplosiveTime > 0)
                ExplosiveTime--;
            if (ExplosionCooldown > 0)
                ExplosionCooldown--;
        }

        private static bool Minion(Projectile p) => p.minion || p.sentry || ProjectileID.Sets.MinionShot[p.type] || ProjectileID.Sets.SentryShot[p.type];

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (TagTime > 0 && Minion(projectile))
                modifiers.FlatBonusDamage += Tag * ProjectileID.Sets.SummonTagDamageMultiplier[projectile.type];
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (ExplosiveTime <= 0 || ExplosionCooldown > 0 || !Minion(projectile) || projectile.owner != Main.myPlayer
                || projectile.type == ModContent.ProjectileType<Projectiles.WikiDarts.DartBlast>())
                return;
            ExplosionCooldown = 10;
            var p = Projectile.NewProjectileDirect(projectile.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.WikiDarts.DartBlast>(),
                Math.Max(1, damageDone / 2), 0f, projectile.owner, 64f, DustID.Torch, 0f);
            p.DamageType = DamageClass.Summon;
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Weapons1119z
{
    /// <summary>
    /// Vortex du Calamitous Vortex : suit le joueur ; aspire les ennemis (pas les boss) à moins de 20 blocs et leur retire les
    /// dégâts de contact ; 4 tentacules frappent les ennemis à moins de 10 blocs toutes les 15 images ; 78 mana par seconde.
    /// </summary>
    public class CalamitousVortexField : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Magic/CalamitousVortexSmall";

        private readonly List<Vector2> lashes = new();

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 320;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => Vector2.Distance(targetHitbox.Center.ToVector2(), Projectile.Center) < 160f;

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!owner.channel || owner.dead || owner.noItems || owner.CCed || owner.HeldItem.type != ModContent.ItemType<Items.Weapons.Magic.CalamitousVortex>())
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 2;
            Projectile.Center = owner.Center;
            owner.itemTime = owner.itemAnimation = 2;
            owner.heldProj = Projectile.whoAmI;
            Projectile.rotation += 0.05f;
            if (++Projectile.ai[0] % 60 == 0 && Projectile.owner == Main.myPlayer && !owner.CheckMana(owner.HeldItem, -1, true))
            {
                Projectile.Kill();
                return;
            }
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.friendly || npc.dontTakeDamage || npc.boss || Vector2.Distance(npc.Center, Projectile.Center) > 320f)
                    continue;
                npc.GetGlobalNPC<WikiDebuffsNPC>().NoContact = 2;
                if (npc.knockBackResist > 0f)
                    npc.velocity = Vector2.Lerp(npc.velocity, (Projectile.Center - npc.Center).SafeNormalize(Vector2.Zero) * 6f, 0.1f);
            }
            if (lashes.Count > 0 && Projectile.ai[0] % 8 == 0)
                lashes.Clear();
            if (!Main.dedServ && Main.rand.NextBool(2))
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(150f, 150f), DustID.RainbowMk2, Vector2.Zero, 0,
                    Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.6f), 1.2f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            lashes.Add(target.Center);
            target.AddBuff(ModContent.BuffType<ExoFreeze>(), 30);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 120);
            target.AddBuff(BuffID.Frostburn, 150);
            target.AddBuff(BuffID.OnFire, 180);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Vector2 c = Projectile.Center - Main.screenPosition;
            Main.EntitySpriteDraw(tex, c, null, Color.White * 0.6f, Projectile.rotation, tex.Size() / 2f, 5f, SpriteEffects.None);
            // 4 tentacules (et un coup de fouet vers chaque ennemi touché)
            Texture2D px = TextureAssets.MagicPixel.Value;
            for (int i = 0; i < 4; i++)
            {
                float a = Projectile.rotation * 2f + MathHelper.PiOver2 * i;
                Main.EntitySpriteDraw(px, c, new Rectangle(0, 0, 1, 1), new Color(220, 60, 90), a, new Vector2(0f, 0.5f), new Vector2(110f, 6f), SpriteEffects.None);
            }
            foreach (Vector2 t in lashes)
                Main.EntitySpriteDraw(px, c, new Rectangle(0, 0, 1, 1), new Color(255, 120, 150), (t - Projectile.Center).ToRotation(), new Vector2(0f, 0.5f),
                    new Vector2(Vector2.Distance(t, Projectile.Center), 4f), SpriteEffects.None);
            return false;
        }
    }

    /// <summary>Triangle de la Lacelinous Nocturne : lent, perce, tire une balle vers l'arrière toutes les 8 images.</summary>
    public class LacelinousTriangle : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 240;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.owner == Main.myPlayer && ++Projectile.ai[0] % 8 == 0)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, -Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(0.4f) * 12f,
                    ModContent.ProjectileType<LacelinousBullet>(), Projectile.damage / 2, 2f, Projectile.owner);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(2))
                target.AddBuff(ModContent.BuffType<Evercold>(), 300);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D px = TextureAssets.MagicPixel.Value;
            Vector2 c = Projectile.Center - Main.screenPosition;
            for (int i = 0; i < 3; i++)
            {
                float a = Projectile.rotation + MathHelper.TwoPi * i / 3f;
                Vector2 p1 = c + a.ToRotationVector2() * 30f, p2 = c + (a + MathHelper.TwoPi / 3f).ToRotationVector2() * 30f;
                Main.EntitySpriteDraw(px, p1, new Rectangle(0, 0, 1, 1), new Color(170, 120, 255, 0), (p2 - p1).ToRotation(), new Vector2(0f, 0.5f),
                    new Vector2(Vector2.Distance(p1, p2), 4f), SpriteEffects.None);
            }
            return false;
        }
    }

    /// <summary>Balle de la Lacelinous Nocturne : explose (64 px) sur un ennemi ou un bloc.</summary>
    public class LacelinousBullet : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.BulletHighVelocity;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
        }

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner != Main.myPlayer)
                return;
            var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<WikiDarts.DartBlast>(),
                Projectile.damage, 0f, Projectile.owner, 64f, DustID.PurpleTorch, Main.rand.NextBool(2) ? ModContent.BuffType<Evercold>() : 0f);
            p.DamageType = DamageClass.Magic;
        }
    }

    /// <summary>Rayon de la Dead Star's Laser Rifle : ai[0] 0 bleu, 1 orange, 2 jaune, 3 vert. Perce à l'infini.</summary>
    public class StarkillerBeam : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private static readonly Color[] Colors = { new(80, 150, 255), new(255, 140, 40), new(255, 230, 60), new(80, 255, 120) };

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
            Projectile.extraUpdates = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            // Orange : ignore l'invulnérabilité des ennemis
            if (Projectile.ai[0] == 1f)
                Projectile.localNPCHitCooldown = 1;
        }

        public override void AI()
        {
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.RainbowMk2, Vector2.Zero, 0, Colors[(int)Projectile.ai[0] % 4], 1.1f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player owner = Main.player[Projectile.owner];
            var source = Projectile.GetSource_FromThis();
            switch ((int)Projectile.ai[0])
            {
                case 0:
                    target.AddBuff(ModContent.BuffType<GlacialState>(), 120);
                    break;
                case 1:
                    target.AddBuff(BuffID.OnFire3, 300);
                    target.GetGlobalNPC<DeadStarNPC>().ExplodesOnDeath = Projectile.owner + 1;
                    target.GetGlobalNPC<DeadStarNPC>().ExplosionDamage = Projectile.damage;
                    break;
                case 2:
                    if (Projectile.owner == Main.myPlayer)
                    {
                        var b = Projectile.NewProjectileDirect(source, target.Center, Vector2.Zero, ModContent.ProjectileType<WikiDarts.DartBlast>(), Projectile.damage, 0f,
                            Projectile.owner, 160f, DustID.YellowTorch, 0f);
                        b.DamageType = DamageClass.Ranged;
                        for (int i = 0; i < 3; i++)
                            Projectile.NewProjectile(source, target.Center, Main.rand.NextVector2Circular(4f, 4f), ModContent.ProjectileType<StellarSpark>(),
                                (int)(Projectile.damage * 0.8f), 0f, Projectile.owner);
                    }
                    break;
                case 3:
                    owner.GetModPlayer<Items.Weapons.Summon.WikiWeaponsZPlayer>().RegenTime = 180;
                    if (Main.rand.NextBool(3))
                        owner.Heal(10);
                    break;
            }
        }
    }

    /// <summary>Étincelle stellaire qui reste 2 s (Dead Star's Laser Rifle, rayon jaune).</summary>
    public class StellarSpark : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Melee/FlamingShard";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.95f;
            Projectile.rotation += 0.2f;
            Lighting.AddLight(Projectile.Center, 0.6f, 0.5f, 0.1f);
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 230, 120, 120);
    }

    /// <summary>Ennemi touché par le rayon orange de la Dead Star's Laser Rifle : explose à sa mort (160 px).</summary>
    public class DeadStarNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public int ExplodesOnDeath, ExplosionDamage;

        public override void OnKill(NPC npc)
        {
            if (ExplodesOnDeath <= 0 || ExplodesOnDeath - 1 != Main.myPlayer)
                return;
            var p = Projectile.NewProjectileDirect(npc.GetSource_Death(), npc.Center, Vector2.Zero, ModContent.ProjectileType<WikiDarts.DartBlast>(), ExplosionDamage, 6f,
                Main.myPlayer, 160f, DustID.Torch, BuffID.OnFire3);
            p.DamageType = DamageClass.Ranged;
        }
    }

    /// <summary>Zone de contact de la Grarble Bayonet : au bout du canon pendant le tir.</summary>
    public class BayonetBlade : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.itemAnimation > 0)
                Projectile.timeLeft = 2;
            Projectile.Center = owner.Center + new Vector2(owner.direction * 46f, 0f);
        }
    }

    /// <summary>Napalm sacré : vole 5 s en laissant des flammes (25 %), puis explose sur 320 px (300 %).</summary>
    public class HolyNapalm : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Ranged/Napalm";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.owner == Main.myPlayer && ++Projectile.ai[0] % 6 == 0)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<HolyNapalmFlame>(), Projectile.damage / 4, 0f, Projectile.owner);
        }

        public static void Debuffs(NPC target)
        {
            target.AddBuff(ModContent.BuffType<Nightclear>(), 300);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 300);
            target.AddBuff(ModContent.BuffType<Evercold>(), 300);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => Debuffs(target);

        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner != Main.myPlayer)
                return;
            var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<WikiDarts.DartBlast>(),
                Projectile.damage * 3, 10f, Projectile.owner, 320f, DustID.GoldFlame, ModContent.BuffType<HolyFlames>());
            p.DamageType = DamageClass.Ranged;
        }
    }

    /// <summary>Flamme sacrée laissée par le napalm : 2 s.</summary>
    public class HolyNapalmFlame : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            if (!Main.dedServ && Main.rand.NextBool(2))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GoldFlame, 0f, -1f, 100, default, 1.3f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => HolyNapalm.Debuffs(target);
    }

    /// <summary>Faux de la Diminishing Victory : revient vers le joueur ; furtive : perce tout, 4 s avant le retour.</summary>
    public class DiminishingScythe : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Rogue/DiminishingVictory";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = 3;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Projectile.Calamity().stealthStrike)
                Projectile.penetrate = -1;
        }

        public override void AI()
        {
            bool stealth = Projectile.Calamity().stealthStrike;
            Projectile.rotation += stealth ? 0.25f : 0.5f;
            Player owner = Main.player[Projectile.owner];
            if (++Projectile.ai[0] > (stealth ? 240f : 30f))
            {
                Vector2 back = owner.Center - Projectile.Center;
                if (back.Length() < 40f)
                {
                    Projectile.Kill();
                    return;
                }
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, back.SafeNormalize(Vector2.Zero) * (stealth ? 8f : 20f), 0.08f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 300);
            if (!Projectile.Calamity().stealthStrike && Projectile.ai[0] < 30f)
                Projectile.ai[0] = 30f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, 0.8f, SpriteEffects.None);
            return false;
        }
    }

    /// <summary>
    /// Bloc de Voidstone du Grand Reef : gravité ; touche un ennemi ou un bloc : éclats de corail, puis revient (ai[1] = 1).
    /// localAI[0] = 1 après avoir monté (vitesse vers le haut) : un coup en descendant fait 7 fois plus de dégâts.
    /// </summary>
    public class GrandReefChunk : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.BoulderStaffOfEarth;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 34;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        private bool Falling => Projectile.localAI[0] == 1f && Projectile.velocity.Y > 0f;

        public override bool? CanHitNPC(NPC target) => Projectile.ai[1] == 1f ? false : null;

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            Projectile.rotation += 0.15f * Math.Sign(Projectile.velocity.X);
            if (Projectile.ai[1] == 1f)
            {
                Projectile.tileCollide = false;
                Vector2 back = owner.Center - Projectile.Center;
                if (back.Length() < 40f)
                {
                    Projectile.Kill();
                    return;
                }
                Projectile.velocity = back.SafeNormalize(Vector2.Zero) * 22f;
                return;
            }
            if (Projectile.velocity.Y < -4f)
                Projectile.localAI[0] = 1f;
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.4f, 18f);
            if (Vector2.Distance(owner.Center, Projectile.Center) > 900f)
                Projectile.ai[1] = 1f;
        }

        private void Burst()
        {
            bool big = Falling;
            Projectile.ai[1] = 1f;
            Projectile.netUpdate = true;
            SoundEngine.PlaySound(SoundID.Item70, Projectile.Center);
            if (Projectile.owner != Main.myPlayer)
                return;
            int n = big ? Main.rand.Next(58, 73) : Main.rand.Next(30, 47);
            for (int i = 0; i < n; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Circular(9f, 9f) - Vector2.UnitY * 3f,
                    ModContent.ProjectileType<CoralShard>(), Math.Max(1, Projectile.damage / 20), 1f, Projectile.owner);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Falling)
                modifiers.SourceDamage *= 7f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => Burst();

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = oldVelocity;
            Burst();
            return false;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(60, 40, 90).MultiplyRGB(lightColor) * 2f;

        public override bool PreDraw(ref Color lightColor)
        {
            Player owner = Main.player[Projectile.owner];
            Texture2D chain = TextureAssets.Chain3.Value;
            Vector2 from = owner.MountedCenter, to = Projectile.Center;
            float dist = Vector2.Distance(from, to);
            for (float d = 0f; d < dist; d += chain.Height)
            {
                Vector2 pos = Vector2.Lerp(from, to, d / dist);
                Main.EntitySpriteDraw(chain, pos - Main.screenPosition, null, Lighting.GetColor(pos.ToTileCoordinates()), (to - from).ToRotation() - MathHelper.PiOver2,
                    chain.Size() / 2f, 1f, SpriteEffects.None);
            }
            return true;
        }
    }

    /// <summary>Éclat de corail mort (5 %) : retombe, reste 2 s.</summary>
    public class CoralShard : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.CrystalShard;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 120;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.2f, 10f);
            Projectile.velocity.X *= 0.98f;
            Projectile.rotation += Projectile.velocity.X * 0.1f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity *= 0f;
            return false;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(190, 180, 170).MultiplyRGB(lightColor);
    }

    /// <summary>
    /// Fouets du wiki (dessinés en ligne colorée) : ai[1] = genre. Duet Hit : 0 feu, 1 givre ; Miracle Smacker : 2 rouge,
    /// 3 vert, 4 bleu.
    /// </summary>
    public class WikiWhip : ModProjectile
    {
        public const int DuetFire = 0, DuetFrost = 1, MiracleRed = 2, MiracleGreen = 3, MiracleBlue = 4;

        private static readonly Color[] Colors = { new(255, 120, 40), new(120, 200, 255), new(255, 60, 60), new(80, 255, 120), new(80, 140, 255) };

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults() => ProjectileID.Sets.IsAWhip[Type] = true;

        public override void SetDefaults()
        {
            Projectile.DefaultToWhip();
            Projectile.WhipSettings.Segments = 18;
            Projectile.WhipSettings.RangeMultiplier = 1.1f;
        }

        private int Kind => (int)Projectile.ai[1];

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player owner = Main.player[Projectile.owner];
            owner.MinionAttackTargetNPC = target.whoAmI;
            var tag = target.GetGlobalNPC<WikiWhipTag>();
            switch (Kind)
            {
                case DuetFire:
                case DuetFrost:
                    tag.Mark(10);
                    SpawnFlake(owner, target, Kind == DuetFire);
                    break;
                case MiracleRed:
                    tag.Mark(22);
                    tag.ExplosiveTime = 240;
                    target.AddBuff(BuffID.OnFire, 180);
                    break;
                case MiracleGreen:
                    tag.Mark(22);
                    target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
                    owner.Heal(4);
                    break;
                case MiracleBlue:
                    tag.Mark(22);
                    target.AddBuff(ModContent.BuffType<ExoFreeze>(), 30);
                    target.AddBuff(BuffID.Frostburn, 180);
                    owner.GetModPlayer<Items.Weapons.Summon.WikiWeaponsZPlayer>().BlueWhipTime = 300;
                    break;
            }
            Projectile.damage = (int)(Projectile.damage * 0.8f);
        }

        private void SpawnFlake(Player owner, NPC target, bool fire)
        {
            if (Projectile.owner != Main.myPlayer)
                return;
            int type = ModContent.ProjectileType<DuetFlake>();
            foreach (Projectile p in Main.ActiveProjectiles)
                if (p.type == type && p.owner == Projectile.owner && (p.ai[0] == 1f) == fire)
                {
                    p.timeLeft = 180;
                    return;
                }
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, type, 20, 0f, Projectile.owner, fire ? 1f : 0f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var points = new List<Vector2>();
            Projectile.FillWhipControlPoints(Projectile, points);
            Texture2D px = TextureAssets.MagicPixel.Value;
            Color color = Colors[Kind % Colors.Length];
            for (int i = 0; i < points.Count - 1; i++)
            {
                Vector2 diff = points[i + 1] - points[i];
                Main.EntitySpriteDraw(px, points[i] - Main.screenPosition, new Rectangle(0, 0, 1, 1), color, diff.ToRotation(), new Vector2(0f, 0.5f),
                    new Vector2(diff.Length() + 1f, i == points.Count - 2 ? 5f : 3f), SpriteEffects.None);
            }
            return false;
        }
    }

    /// <summary>Flocon de la Duet Hit : ai[0] 1 feu (Hellfire), 0 givre (Frostbite) ; 20 dégâts, ignore 30 défense, 3 s.</summary>
    public class DuetFlake : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Summon/FireFlake";

        public override void SetStaticDefaults() => ProjectileID.Sets.MinionShot[Type] = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.ArmorPenetration = 30;
            Projectile.scale = 0.6f;
        }

        public override void AI()
        {
            NPC target = WikiDarts.WikiDartProj.Closest(Projectile.Center, 500f);
            if (target != null)
                WikiDarts.WikiDartProj.Home(Projectile, target, 10f, 12f);
            else
                Projectile.velocity *= 0.9f;
            Projectile.rotation += 0.1f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(2))
                target.AddBuff(Projectile.ai[0] == 1f ? BuffID.OnFire3 : BuffID.Frostburn2, 300);
        }

        public override Color? GetAlpha(Color lightColor) => Projectile.ai[0] == 1f ? Color.White : new Color(140, 210, 255);
    }

    /// <summary>EXO-LOTL : saute sur les ennemis et leur tire un laser toutes les 30 images (Exo Freeze 3 s).</summary>
    public class EXOLOTL : Weapons1119m.WikiMinion
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Summon/EXOLOTL";

        protected override int BuffType => ModContent.BuffType<Buffs.EXOLOTLBuff>();

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 46;
            Projectile.height = 34;
            Projectile.tileCollide = true;
        }

        public override bool? CanDamage() => false;

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!KeepAlive())
                return;
            NPC target = Projectile.OwnerMinionAttackTargetNPC ?? WikiDarts.WikiDartProj.Closest(Projectile.Center, 900f);
            Vector2 goal = target?.Center ?? owner.Center + new Vector2(-owner.direction * 50f, 0f);
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.4f, 12f);
            bool grounded = Projectile.velocity.Y == 0f || Collision.SolidCollision(Projectile.BottomLeft, Projectile.width, 4);
            float dx = goal.X - Projectile.Center.X;
            Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, Math.Sign(dx) * Math.Min(8f, Math.Abs(dx) * 0.05f), 0.1f);
            if (grounded && (target != null || goal.Y < Projectile.Center.Y - 60f) && ++Projectile.ai[1] > 40f)
            {
                Projectile.ai[1] = 0f;
                Projectile.velocity.Y = -9f;
            }
            if (Vector2.Distance(owner.Center, Projectile.Center) > 1400f)
                Projectile.Center = owner.Center;
            if (Projectile.velocity.X != 0f)
                Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
            if (target != null && Projectile.owner == Main.myPlayer && ++Projectile.ai[0] >= 30f)
            {
                Projectile.ai[0] = 0f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center - Vector2.UnitY * 10f, (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitX) * 16f,
                    ModContent.ProjectileType<EXOLOTLLaser>(), Projectile.damage, 0f, Projectile.owner);
            }
        }
    }

    public class EXOLOTLLaser : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.GreenLaser;

        public override void SetStaticDefaults() => ProjectileID.Sets.MinionShot[Type] = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.extraUpdates = 1;
        }

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<ExoFreeze>(), 180);
    }

    /// <summary>Cristal de l'Evercoldance (tourelle) : rayon (110 %) sur les 2 ennemis les plus proches toutes les 30 images.</summary>
    public class EvercoldCrystal : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Summon/EvercoldCrystal";

        public override void SetStaticDefaults() => ProjectileID.Sets.MinionTargettingFeature[Type] = true;

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 78;
            Projectile.sentry = true;
            Projectile.friendly = false;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.timeLeft = Projectile.SentryLifeTime;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            Projectile.velocity = Vector2.Zero;
            Lighting.AddLight(Projectile.Center, 0.3f, 0.5f, 0.9f);
            if (Projectile.owner != Main.myPlayer || ++Projectile.ai[0] < 30f)
                return;
            Projectile.ai[0] = 0f;
            NPC first = WikiDarts.WikiDartProj.Closest(Projectile.Center, 800f);
            if (first == null)
                return;
            NPC second = WikiDarts.WikiDartProj.Closest(Projectile.Center, 800f, first.whoAmI);
            foreach (NPC n in new[] { first, second })
                if (n != null)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (n.Center - Projectile.Center).SafeNormalize(Vector2.UnitY) * 20f,
                        ModContent.ProjectileType<EvercoldBeam>(), (int)(Projectile.damage * 1.1f), 0f, Projectile.owner);
        }
    }

    /// <summary>Rayon de l'Evercoldance : 25 % chacun d'Evercold, Nightwither, Crush Depth (5 s).</summary>
    public class EvercoldBeam : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults() => ProjectileID.Sets.SentryShot[Type] = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 60;
            Projectile.extraUpdates = 3;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.IceTorch, Vector2.Zero, 100, default, 1.2f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(4))
                target.AddBuff(ModContent.BuffType<Evercold>(), 300);
            if (Main.rand.NextBool(4))
                target.AddBuff(ModContent.BuffType<Nightwither>(), 300);
            if (Main.rand.NextBool(4))
                target.AddBuff(ModContent.BuffType<CrushDepth>(), 300);
        }
    }
}
