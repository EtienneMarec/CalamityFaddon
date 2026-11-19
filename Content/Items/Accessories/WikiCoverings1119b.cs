using System;
using CalamityFandom.Content.Projectiles.Coverings1119b;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer.Dashes;
using CalamityMod.Enums;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Dart = CalamityFandom.Content.Projectiles.WikiDarts.WikiDartProj;

// Lot AG (19/11) : « coverings » (accessoires) du wiki, fabricables (recettes du wiki). Rareté « C4 » → 0.

namespace CalamityFandom.Content.Items.Accessories
{
    /// <summary>
    /// Wiki : Fathomless Soul Artifact.txt — porté : +15 % de dégâts de plus chaque seconde, mais -20 PV max chaque seconde,
    /// jusqu'à 1 PV max. [choix] Tout repart de zéro quand on l'enlève ou qu'on meurt.
    /// </summary>
    public class FathomlessSoulArtifact : WikiCovering
    {
        protected override int Rarity => ModContent.RarityType<CosmicPurple>();
        protected override int SellCopper => 280000;

        public override void UpdateAccessory(Player player, bool hideVisual) => player.GetModPlayer<CoveringsBPlayer>().Fathomless = true;
    }

    /// <summary>
    /// Wiki : Mobilite Core.txt — une ruée d'environ 25 blocs qui inflige de gros dégâts (100). [choix] Coup de bouclier de
    /// Calamity, vitesse 26.
    /// </summary>
    public class MobiliteCore : WikiCovering
    {
        protected override int Rarity => ItemRarityID.Red;
        protected override int SellCopper => 100000;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().DashID = MobiliteDash.ID;
            player.dashType = 0;
        }
    }

    /// <summary>
    /// Wiki : Ravenous Soul Artifact.txt — +500 % d'aggro ; une ruée qui va vers l'ennemi le plus proche et rebondit dessus
    /// (500 dégâts), au temps de recharge court ; pendant la ruée, les projectiles font 90 % de dégâts en moins.
    /// [choix] « +500 % » = +500 d'aggro (la Beetle armor donne +400 pour comparaison) ; la ruée vise l'ennemi le plus proche
    /// à l'horizontale, à moins de 40 blocs.
    /// </summary>
    public class RavenousSoulArtifact : WikiCovering
    {
        protected override int Rarity => ModContent.RarityType<CosmicPurple>();
        protected override int SellCopper => 280000;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.aggro += 500;
            player.Calamity().DashID = RavenousDash.ID;
            player.dashType = 0;
        }
    }

    /// <summary>
    /// Wiki : Night Cloak.txt — PV et mana max x3, immunité aux Holy Flames (et aux « Umbrus Fumes »), dégâts x2 ; si un
    /// Élémentaire est en vie : Obstructed, dégâts x4 et toutes les attaques infligent Holy Flames 5 s.
    /// [choix] « Élémentaire » = un boss dont le nom contient « Elemental » ; « +200 % » = x3 (les PV), « double » et
    /// « quadruple » = +100 % et +300 % de dégâts. [technique] Umbrus Fumes n'existe pas encore dans ce mod.
    /// </summary>
    public class NightCloak : WikiCovering
    {
        protected override int Rarity => ItemRarityID.White;
        protected override int SellCopper => 0;

        public override void UpdateAccessory(Player player, bool hideVisual) => player.GetModPlayer<CoveringsBPlayer>().NightCloak = true;
    }

    /// <summary>
    /// Wiki : Exotic Beacon.txt — porté : un Élémentaire Exo (500 dégâts d'invocation) qui alterne deux attaques près d'un
    /// ennemi : 3 cristaux Exo chercheurs qui se divisent au contact, et des coups de lianes en avançant ; Elemental Mix.
    /// [choix] Alternance toutes les 3 s ; cristaux toutes les 40 images, 3 éclats à 33 % ; lianes = l'élémentaire fonce et
    /// frappe au contact ; ne prend pas d'emplacement de sbire (comme les élémentaires des accessoires de Calamity).
    /// </summary>
    public class ExoticBeacon : WikiCovering
    {
        protected override int Rarity => ModContent.RarityType<BurnishedAuric>();
        protected override int SellCopper => 22110100;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 500;
            Item.DamageType = DamageClass.Summon;
            Item.width = 22;
            Item.height = 30;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var mp = player.GetModPlayer<CoveringsBPlayer>();
            mp.ExoticBeacon = true;
            mp.BeaconItem = Item;
        }
    }

    /// <summary>
    /// Wiki : Lunarspectre Tracers.txt — +12 défense, +5 de régénération, +12 % de vitesse, +32 % de vitesse de saut, course
    /// jusqu'à ~22 blocs/s, +80 images de vol, saut automatique, pas de dégâts de chute ; en courant, une fée toutes les 2 s
    /// qui fonce sur l'ennemi le plus proche et explose (120, de la meilleure classe, sans critique) ; les attaques
    /// infligent Moon Burnt (Nightwither). [technique] « Les debuffs font 33 % de dégâts en plus et durent 50 % de plus » :
    /// non fait (aucun crochet pour les debuffs posés par les autres objets). [choix] Moon Burnt 3 s.
    /// </summary>
    public class LunarspectreTracers : WikiCovering
    {
        protected override int Rarity => ItemRarityID.Red;
        protected override int SellCopper => 500000;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.defense = 12;
            Item.width = 52;
            Item.height = 40;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.lifeRegen += 5;
            player.moveSpeed += 0.12f;
            player.jumpSpeedBoost += 1.6f;
            player.accRunSpeed = Math.Max(player.accRunSpeed, 10.3f);
            player.wingTimeMax += 80;
            player.autoJump = true;
            player.noFallDmg = true;
            player.GetModPlayer<CoveringsBPlayer>().Lunarspectre = true;
        }
    }

    /// <summary>
    /// Wiki : Dreamy Belt.txt — +15 % de dégâts de mêlée et de magie, +20 % de vraie mêlée, +35 % de PV et de mana max, +15
    /// défense et pénétration d'armure, +0,1 de chance ; un papillon cauchemar toutes les 3 s, qui tourne autour du joueur et
    /// tire des balles chercheuses (150 dégâts magiques) ; touche « Butterfly Release » : tous les papillons foncent vers le
    /// curseur ; quand tous les papillons sont là, la vraie mêlée lance une onde en croissant (perce à l'infini, Moon Burnt
    /// 4 s), recharge de 5 s. [choix] 5 papillons au plus ; balles toutes les 40 images ; onde à 100 % du coup.
    /// </summary>
    public class DreamyBelt : WikiCovering
    {
        protected override int Rarity => ModContent.RarityType<Turquoise>();
        protected override int SellCopper => 300000;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 46;
            Item.height = 38;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Melee) += 0.15f;
            player.GetDamage(DamageClass.Magic) += 0.15f;
            player.GetDamage(ModContent.GetInstance<TrueMeleeDamageClass>()) += 0.2f;
            player.statDefense += 15;
            player.GetArmorPenetration(DamageClass.Generic) += 15;
            player.luck += 0.1f;
            player.GetModPlayer<CoveringsBPlayer>().DreamyBelt = true;
        }
    }

    public class CoveringsBPlayer : ModPlayer
    {
        public const int MaxButterflies = 5;
        public static ModKeybind ButterflyKey;

        public bool Fathomless, NightCloak, ExoticBeacon, Lunarspectre, DreamyBelt;
        public Item BeaconItem;
        public int FathomlessTime, FairyTimer, ButterflyTimer, WaveCooldown;

        public override void Load() => ButterflyKey = KeybindLoader.RegisterKeybind(Mod, "ButterflyRelease", "G");

        public override void Unload() => ButterflyKey = null;

        public override void ResetEffects()
        {
            if (!Fathomless)
                FathomlessTime = 0;
            Fathomless = NightCloak = ExoticBeacon = Lunarspectre = DreamyBelt = false;
            BeaconItem = null;
            if (WaveCooldown > 0)
                WaveCooldown--;
        }

        public override void UpdateDead() => FathomlessTime = 0;

        private static bool ElementalAlive()
        {
            foreach (NPC n in Main.ActiveNPCs)
                if (n.boss && n.TypeName.Contains("Elemental"))
                    return true;
            return false;
        }

        public override void ModifyMaxStats(out StatModifier health, out StatModifier mana)
        {
            health = StatModifier.Default;
            mana = StatModifier.Default;
            if (DreamyBelt)
            {
                health *= 1.35f;
                mana *= 1.35f;
            }
            if (NightCloak)
            {
                health *= 3f;
                mana *= 3f;
            }
        }

        public override void PostUpdateEquips()
        {
            if (Fathomless)
            {
                FathomlessTime++;
                int seconds = FathomlessTime / 60;
                Player.GetDamage(DamageClass.Generic) += 0.15f * seconds;
                Player.statLifeMax2 = Math.Max(1, Player.statLifeMax2 - 20 * seconds);
                if (Player.statLife > Player.statLifeMax2)
                    Player.statLife = Player.statLifeMax2;
            }
            if (NightCloak)
            {
                Player.buffImmune[ModContent.BuffType<HolyFlames>()] = true;
                bool elemental = ElementalAlive();
                Player.GetDamage(DamageClass.Generic) += elemental ? 3f : 1f;
                if (elemental)
                    Player.AddBuff(BuffID.Obstructed, 2);
            }
            if (Player.whoAmI != Main.myPlayer)
                return;
            if (ExoticBeacon && BeaconItem != null && Player.ownedProjectileCounts[ModContent.ProjectileType<ExoElemental>()] < 1)
            {
                int damage = (int)Player.GetTotalDamage(DamageClass.Summon).ApplyTo(BeaconItem.damage);
                var p = Projectile.NewProjectileDirect(Player.GetSource_Accessory(BeaconItem), Player.Center, Vector2.Zero, ModContent.ProjectileType<ExoElemental>(), damage, 2f, Player.whoAmI);
                p.originalDamage = BeaconItem.damage;
            }
            if (Lunarspectre && Math.Abs(Player.velocity.X) > 3f && Player.velocity.Y == 0f && ++FairyTimer >= 120)
            {
                FairyTimer = 0;
                int damage = (int)Player.GetBestClassDamage().ApplyTo(120);
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, new Vector2(0f, -4f), ModContent.ProjectileType<LunarFairy>(), damage, 1f, Player.whoAmI);
            }
            if (DreamyBelt && Player.ownedProjectileCounts[ModContent.ProjectileType<NightmareButterfly>()] < MaxButterflies && ++ButterflyTimer >= 180)
            {
                ButterflyTimer = 0;
                int damage = (int)Player.GetTotalDamage(DamageClass.Magic).ApplyTo(150);
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<NightmareButterfly>(), damage, 1f, Player.whoAmI,
                    Player.ownedProjectileCounts[ModContent.ProjectileType<NightmareButterfly>()]);
            }
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (ButterflyKey?.JustPressed != true || !DreamyBelt)
                return;
            foreach (Projectile p in Main.ActiveProjectiles)
                if (p.type == ModContent.ProjectileType<NightmareButterfly>() && p.owner == Player.whoAmI)
                {
                    p.ai[1] = 1f;
                    p.netUpdate = true;
                }
        }

        private void OnAnyHit(NPC target, bool trueMelee, Vector2 from)
        {
            if (NightCloak && ElementalAlive())
                target.AddBuff(ModContent.BuffType<HolyFlames>(), 300);
            if (Lunarspectre)
                target.AddBuff(ModContent.BuffType<Nightwither>(), 180);
            if (DreamyBelt && trueMelee && WaveCooldown <= 0 && Player.whoAmI == Main.myPlayer
                && Player.ownedProjectileCounts[ModContent.ProjectileType<NightmareButterfly>()] >= MaxButterflies)
            {
                WaveCooldown = 300;
                Vector2 dir = (Main.MouseWorld - Player.Center).SafeNormalize(Vector2.UnitX * Player.direction);
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, dir * 14f, ModContent.ProjectileType<DreamyCrescent>(),
                    Player.GetWeaponDamage(Player.HeldItem), 4f, Player.whoAmI);
            }
        }

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone) => OnAnyHit(target, item.CountsAsClass<MeleeDamageClass>(), Player.Center);

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (proj.type == ModContent.ProjectileType<DreamyCrescent>())
                return;
            OnAnyHit(target, proj.CountsAsClass<TrueMeleeDamageClass>(), proj.Center);
        }
    }

    /// <summary>Ruée du Mobilite Core : coup de bouclier, 100 dégâts (mêlée).</summary>
    public class MobiliteDash : PlayerDashEffect
    {
        public static new string ID { get; private set; }

        public override DashCollisionType CollisionType => DashCollisionType.ShieldSlam;

        public override bool IsOmnidirectional => false;

        public override void Load() => ID = DashID;

        public override float CalculateDashSpeed(Player player) => 26f;

        public override void MidDashEffects(Player player, ref float dashSpeed, ref float dashSpeedDecelerationFactor, ref float runSpeedDecelerationFactor)
        {
            dashSpeed = 20f;
            if (!Main.dedServ)
                Dust.NewDustDirect(player.position, player.width, player.height, DustID.Electric, -player.velocity.X * 0.2f, 0f, 100, default, 1.2f).noGravity = true;
        }

        public override void OnHitEffects(Player player, NPC npc, IEntitySource source, ref DashHitContext hitContext)
        {
            hitContext.HitDirection = player.velocity.X >= 0f ? 1 : -1;
            hitContext.PlayerImmunityFrames = 15;
            hitContext.damageClass = DamageClass.Melee;
            hitContext.BaseDamage = 100;
            hitContext.BaseKnockback = 8f;
        }
    }

    /// <summary>Ruée du Ravenous Soul Artifact : part vers l'ennemi le plus proche à l'horizontale, rebondit dessus (500).</summary>
    public class RavenousDash : PlayerDashEffect
    {
        public static new string ID { get; private set; }

        public override DashCollisionType CollisionType => DashCollisionType.ShieldBonk;

        public override bool IsOmnidirectional => true;

        public override void Load() => ID = DashID;

        public override float CalculateDashSpeed(Player player) => 18f;

        public override void OnDashEffects(Player player)
        {
            NPC best = null;
            float bestDx = 640f;
            foreach (NPC n in Main.ActiveNPCs)
                if (n.CanBeChasedBy() && Math.Abs(n.Center.X - player.Center.X) < bestDx && Math.Abs(n.Center.Y - player.Center.Y) < 400f)
                {
                    bestDx = Math.Abs(n.Center.X - player.Center.X);
                    best = n;
                }
            if (best != null)
                player.velocity = (best.Center - player.Center).SafeNormalize(Vector2.UnitX * player.direction) * 18f;
            player.GetModPlayer<RavenousPlayer>().Dashing = 20;
        }

        public override void MidDashEffects(Player player, ref float dashSpeed, ref float dashSpeedDecelerationFactor, ref float runSpeedDecelerationFactor)
        {
            if (!Main.dedServ)
                Dust.NewDustDirect(player.position, player.width, player.height, DustID.RedTorch, 0f, 0f, 100, default, 1.4f).noGravity = true;
        }

        public override void OnHitEffects(Player player, NPC npc, IEntitySource source, ref DashHitContext hitContext)
        {
            hitContext.HitDirection = npc.Center.X > player.Center.X ? 1 : -1;
            hitContext.PlayerImmunityFrames = 20;
            hitContext.damageClass = DamageClass.Generic;
            hitContext.BaseDamage = 500;
            hitContext.BaseKnockback = 6f;
        }
    }

    /// <summary>Pendant la ruée du Ravenous Soul Artifact, les projectiles font 90 % de dégâts en moins.</summary>
    public class RavenousPlayer : ModPlayer
    {
        public int Dashing;

        public override void PostUpdate()
        {
            if (Dashing > 0)
                Dashing--;
        }

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            if (Dashing > 0)
                modifiers.FinalDamage *= 0.1f;
        }
    }
}

namespace CalamityFandom.Content.Buffs
{
    public class ExoElementalBuff : ModBuff
    {
        public override string Texture => "CalamityFandom/Content/Buffs/Wiki/ExoElementalBuff";

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Coverings1119b
{
    /// <summary>
    /// Élémentaire Exo (Exotic Beacon) : suit le joueur ; près d'un ennemi, alterne toutes les 3 s cristaux chercheurs
    /// (toutes les 40 images) et charge (coups de lianes au contact). Disparaît sans l'accessoire.
    /// </summary>
    public class ExoElemental : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Summon/ExoElemental";

        public override void SetStaticDefaults()
        {
            Main.projPet[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 18000;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.scale = 0.7f;
        }

        private bool Charging => Projectile.ai[0] % 360 >= 180;

        public override bool? CanDamage() => Charging ? null : false;

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            var mp = owner.GetModPlayer<Items.Accessories.CoveringsBPlayer>();
            if (owner.dead || !mp.ExoticBeacon)
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 2;
            owner.AddBuff(ModContent.BuffType<Buffs.ExoElementalBuff>(), 2);
            NPC target = Projectile.OwnerMinionAttackTargetNPC ?? Dart.Closest(owner.Center, 900f);
            Projectile.ai[0]++;
            if (target == null)
            {
                Vector2 home = owner.Center + new Vector2(-owner.direction * 60f, -60f);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, (home - Projectile.Center) * 0.1f, 0.2f);
            }
            else if (Charging)
                Dart.Home(Projectile, target, 16f, 10f);
            else
            {
                Vector2 hover = target.Center + new Vector2(0f, -200f);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, (hover - Projectile.Center) * 0.08f, 0.2f);
                if (Projectile.owner == Main.myPlayer && Projectile.ai[0] % 40 == 0)
                    for (int i = -1; i <= 1; i++)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitY).RotatedBy(i * 0.4f) * 10f,
                            ModContent.ProjectileType<ExoCrystal>(), Projectile.damage / 2, 1f, Projectile.owner);
            }
            if (Projectile.Distance(owner.Center) > 2000f)
                Projectile.Center = owner.Center;
            Projectile.spriteDirection = Projectile.velocity.X > 0 ? 1 : -1;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<ElementalMix>(), 120);
    }

    /// <summary>Cristal Exo : cherche l'ennemi, se divise en 3 éclats (33 %) au contact ; Elemental Mix 2 s.</summary>
    public class ExoCrystal : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.CrystalShard;

        public override void SetStaticDefaults() => ProjectileID.Sets.MinionShot[Type] = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 150;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0f)
                Dart.Home(Projectile, Dart.Closest(Projectile.Center, 700f), 12f, 15f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override Color? GetAlpha(Color lightColor) => Main.hslToRgb((Main.GlobalTimeWrappedHourly + Projectile.whoAmI * 0.1f) % 1f, 1f, 0.7f) with { A = 150 };

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ElementalMix>(), 120);
            if (Projectile.ai[0] != 0f || Projectile.owner != Main.myPlayer)
                return;
            for (int i = 0; i < 3; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2CircularEdge(7f, 7f), Type, Projectile.damage / 3, 0f, Projectile.owner, 1f);
        }
    }

    /// <summary>Fée des Lunarspectre Tracers : fonce sur l'ennemi le plus proche et explose (sans critique).</summary>
    public class LunarFairy : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.FairyQueenMagicItemShot;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 240;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Dart.Home(Projectile, Dart.Closest(Projectile.Center, 800f), 14f, 14f);
            Lighting.AddLight(Projectile.Center, 0.4f, 0.7f, 0.9f);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => modifiers.DisableCrit();

        public override Color? GetAlpha(Color lightColor) => new Color(150, 230, 255, 0);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Nightwither>(), 180);

        public override void OnKill(int timeLeft)
        {
            if (!Main.dedServ)
                for (int i = 0; i < 15; i++)
                    Dust.NewDustPerfect(Projectile.Center, DustID.IceTorch, Main.rand.NextVector2Circular(4f, 4f), 100, default, 1.4f).noGravity = true;
        }
    }

    /// <summary>
    /// Papillon cauchemar (Dreamy Belt) : ai[0] = place autour du joueur ; tire une balle chercheuse toutes les 40 images ;
    /// ai[1] = 1 : lâché, fonce vers le curseur et frappe, puis disparaît.
    /// </summary>
    public class NightmareButterfly : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.EmpressBlade;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 18000;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.scale = 0.6f;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.dead || !owner.GetModPlayer<Items.Accessories.CoveringsBPlayer>().DreamyBelt)
            {
                Projectile.Kill();
                return;
            }
            if (Projectile.ai[1] == 1f)
            {
                Projectile.friendly = true;
                if (Projectile.localAI[1] == 0f)
                {
                    Projectile.localAI[1] = 1f;
                    Projectile.timeLeft = 90;
                    if (Projectile.owner == Main.myPlayer)
                    {
                        Projectile.velocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.UnitY) * 20f;
                        Projectile.netUpdate = true;
                    }
                }
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                return;
            }
            float angle = MathHelper.TwoPi * Projectile.ai[0] / Items.Accessories.CoveringsBPlayer.MaxButterflies + Main.GlobalTimeWrappedHourly * 2f;
            Projectile.Center = owner.Center + angle.ToRotationVector2() * 70f;
            Projectile.rotation = angle + MathHelper.PiOver2;
            if (Projectile.owner == Main.myPlayer && ++Projectile.localAI[0] % 40 == 0)
            {
                NPC target = Dart.Closest(Projectile.Center, 800f);
                if (target != null)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitX) * 12f,
                        ModContent.ProjectileType<NightmareBullet>(), Projectile.damage, 1f, Projectile.owner);
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(120, 60, 180, 150);
    }

    /// <summary>Balle des papillons cauchemar (150 dégâts magiques) : cherche les ennemis.</summary>
    public class NightmareBullet : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Dart.Home(Projectile, Dart.Closest(Projectile.Center, 800f), 14f, 12f);
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.PurpleTorch, Vector2.Zero, 100, default, 1f).noGravity = true;
        }
    }

    /// <summary>Onde en croissant du Dreamy Belt : perce à l'infini 1,5 s, Moon Burnt (Nightwither) 4 s.</summary>
    public class DreamyCrescent : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.NightBeam;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Projectile.penetrate = -1;
            Projectile.timeLeft = 90;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation();

        public override Color? GetAlpha(Color lightColor) => new Color(170, 110, 255, 100);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Nightwither>(), 240);
    }
}
