using System;
using System.Linq;
using CalamityFandom.Common.Debug;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Accessories;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

// Lot AQ (20/11) : coverings qui cumulent les effets d'objets existants (les UpdateAccessory de Calamity sont appelés tels
// quels) et ajoutent les leurs.

namespace CalamityFandom.Content.Items.Accessories
{
    /// <summary>
    /// Wiki : Quantum Flahium Shock Device.txt — effets du Fungal Symbiote, du Warbanner of the Sun et du Yoyo Bag ; +40 %
    /// de dégâts et de vitesse de mêlée, +35 % de dégâts de vraie mêlée, +15 % de critique de mêlée, +10 de pénétration
    /// d'armure ; les coups de mêlée infligent Flahmes (3 s) ; les coups de vraie mêlée lancent de la flah dans plusieurs
    /// directions. [choix] Le Warbanner de Calamity (Warbanner of the Righteous, que Calamity++ appelle Warbanner of the Sun) ;
    /// contrepoids vanilla ; 4 éclats de flah (25 % des dégâts) au plus toutes les 10 images.
    /// </summary>
    public class QuantumFlahiumShockDevice : WikiCovering
    {
        protected override int Rarity => ModContent.RarityType<HotPink>();
        protected override int SellCopper => 1000000;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            ModContent.GetInstance<FungalSymbiote>().UpdateAccessory(player, hideVisual);
            ModContent.GetInstance<WarbanneroftheRighteous>().UpdateAccessory(player, hideVisual);
            player.yoyoGlove = true;
            player.yoyoString = true;
            player.counterWeight = ProjectileID.BlackCounterweight;
            player.GetDamage<MeleeDamageClass>() += 0.4f;
            player.GetAttackSpeed<MeleeDamageClass>() += 0.4f;
            player.GetDamage(ModContent.GetInstance<TrueMeleeDamageClass>()) += 0.35f;
            player.GetCritChance<MeleeDamageClass>() += 15;
            player.GetArmorPenetration<GenericDamageClass>() += 10;
            player.GetModPlayer<Coverings1120Player>().Shock = true;
        }
    }

    /// <summary>
    /// Wiki : Quantum Flahium Ejection Device.txt — effets des Dragon Scales et de l'Ethereal Extorter ; +50 % de dégâts et
    /// +25 % de vitesse de spy, +15 % de critique de spy, +50 de furtivité max, furtivité 15 % plus rapide ; frappes furtives :
    /// toujours critiques, +50 de pénétration, +10 % de dégâts, la moitié de la furtivité seulement, et trois flares de
    /// flahium (300) qui tombent autour du tir ; les tirs de spy lâchent une flahblade (80, chercheuse) toutes les 0,5 s,
    /// sept au plus ; Flahmes 3 s. [technique] La ruée « Booster Dash » (description coupée sur la page) n'est pas faite.
    /// </summary>
    public class QuantumFlahiumEjectionDevice : WikiCovering
    {
        protected override int Rarity => ModContent.RarityType<HotPink>();
        protected override int SellCopper => 1000000;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            ModContent.GetInstance<DragonScales>().UpdateAccessory(player, hideVisual);
            ModContent.GetInstance<EtherealExtorter>().UpdateAccessory(player, hideVisual);
            var cal = player.Calamity();
            var rogue = ModContent.GetInstance<RogueDamageClass>();
            player.GetDamage(rogue) += 0.5f;
            player.GetCritChance(rogue) += 15;
            cal.rogueVelocity += 0.25f;
            cal.rogueStealthMax += 0.5f;
            cal.stealthGenStandstill += 0.15f;
            cal.stealthGenMoving += 0.15f;
            cal.stealthStrikeHalfCost = true;
            cal.wearingRogueArmor = true;
            player.GetModPlayer<Coverings1120Player>().Ejection = true;
        }
    }

    /// <summary>
    /// Wiki : Zenith Arcanum.txt — +20 % de dégâts d'invocation et de défense, +15 % de vitesse de minage, +4 régénération,
    /// +8 sbires, plus de recul des sbires ; invoque les esprits du monde (élémentaires, gladiateurs, trio de Howl : Heart of
    /// the Elements, Gladiator's Locket et Howl's Heart de Calamity), dont la Rare Sand Elemental qui soigne ; les sbires
    /// infligent huit debuffs 3 s et font apparaître des membres squelettiques (660, Vulnerability Hex) au contact.
    /// [technique] Plague Elemental, Sulphur Elemental et Young Duke n'existent pas dans Calamity : absents.
    /// [choix] « Demon Flames » = Brimstone Flames ; un membre squelettique au plus toutes les 30 images.
    /// </summary>
    public class ZenithArcanum : WikiCovering
    {
        protected override int Rarity => ModContent.RarityType<BurnishedAuric>();
        protected override int SellCopper => 160000;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            ModContent.GetInstance<HeartoftheElements>().UpdateAccessory(player, hideVisual);
            ModContent.GetInstance<GladiatorsLocket>().UpdateAccessory(player, hideVisual);
            ModContent.GetInstance<HowlsHeart>().UpdateAccessory(player, hideVisual);
            player.GetDamage<SummonDamageClass>() += 0.2f;
            player.GetKnockback<SummonDamageClass>() += 1f;
            player.pickSpeed -= 0.15f;
            player.lifeRegen += 4;
            player.maxMinions += 8;
            player.GetModPlayer<Coverings1120Player>().Zenith = true;
        }
    }

    public class Coverings1120Player : ModPlayer
    {
        public bool Shock, Ejection, Zenith;
        public int FlahCooldown, LimbCooldown;

        public override void ResetEffects() => Shock = Ejection = Zenith = false;

        public override void PostUpdateEquips()
        {
            if (Zenith)
                Player.statDefense += Player.statDefense.Positive / 5;
            if (FlahCooldown > 0)
                FlahCooldown--;
            if (LimbCooldown > 0)
                LimbCooldown--;
        }

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Shock && item.CountsAsClass<MeleeDamageClass>())
            {
                target.AddBuff(ModContent.BuffType<Buffs.Flahmes>(), 180);
                EmitFlah(target, damageDone);
            }
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Shock && proj.CountsAsClass<MeleeDamageClass>())
            {
                target.AddBuff(ModContent.BuffType<Buffs.Flahmes>(), 180);
                if (proj.CountsAsClass(ModContent.GetInstance<TrueMeleeDamageClass>()))
                    EmitFlah(target, damageDone);
            }
            if (Ejection && proj.CountsAsClass(ModContent.GetInstance<RogueDamageClass>()))
                target.AddBuff(ModContent.BuffType<Buffs.Flahmes>(), 180);
        }

        /// <summary>Vraie mêlée (Shock Device) : 4 éclats de flah dans des directions au hasard.</summary>
        private void EmitFlah(NPC target, int damage)
        {
            if (FlahCooldown > 0 || Player.whoAmI != Main.myPlayer)
                return;
            FlahCooldown = 10;
            for (int i = 0; i < 4; i++)
                Projectile.NewProjectile(Player.GetSource_OnHit(target), target.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(9f, 14f),
                    ModContent.ProjectileType<Projectiles.Typeless.FlahShard>(), Math.Max(1, damage / 4), 2f, Player.whoAmI);
        }
    }

    /// <summary>Ejection Device (tirs de spy) et Zenith Arcanum (coups des sbires).</summary>
    public class Coverings1120Projectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        private int age;

        private static Coverings1120Player Owner(Projectile p)
            => p.friendly && p.owner >= 0 && p.owner < Main.maxPlayers && Main.player[p.owner].active ? Main.player[p.owner].GetModPlayer<Coverings1120Player>() : null;

        private static bool IsRogue(Projectile p) => p.CountsAsClass(ModContent.GetInstance<RogueDamageClass>()) && p.type != ModContent.ProjectileType<Projectiles.Rogue.Flahblade>()
            && p.type != ModContent.ProjectileType<Projectiles.Rogue.FlahiumFlare>();

        public override void PostAI(Projectile projectile)
        {
            var cp = Owner(projectile);
            if (cp == null || !cp.Ejection || projectile.owner != Main.myPlayer || !IsRogue(projectile))
                return;
            var source = projectile.GetSource_FromThis();
            // Frappe furtive : trois flares de flahium tombent autour du tir
            if (age == 0 && projectile.Calamity().stealthStrike)
                for (int i = -1; i <= 1; i++)
                    Projectile.NewProjectile(source, projectile.Center + new Vector2(i * 120f, -700f), new Vector2(-i * 1.5f, 18f),
                        ModContent.ProjectileType<Projectiles.Rogue.FlahiumFlare>(), (int)cp.Player.GetTotalDamage(ModContent.GetInstance<RogueDamageClass>()).ApplyTo(300), 3f, projectile.owner);
            // Une flahblade toutes les 0,5 s, sept au plus
            if (++age % 30 == 0 && cp.Player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Rogue.Flahblade>()] < 7)
                Projectile.NewProjectile(source, projectile.Center, Main.rand.NextVector2Unit() * 6f, ModContent.ProjectileType<Projectiles.Rogue.Flahblade>(),
                    (int)cp.Player.GetTotalDamage(ModContent.GetInstance<RogueDamageClass>()).ApplyTo(80), 2f, projectile.owner);
        }

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            var cp = Owner(projectile);
            if (cp != null && cp.Ejection && IsRogue(projectile) && projectile.Calamity().stealthStrike)
            {
                modifiers.SetCrit();
                modifiers.ArmorPenetration += 50;
                modifiers.SourceDamage *= 1.1f;
            }
        }

        private static readonly Func<int>[] ZenithDebuffs =
        {
            ModContent.BuffType<BrimstoneFlames>, () => BuffID.Electrified, ModContent.BuffType<TemporalSadness>, ModContent.BuffType<AstralInfectionDebuff>,
            ModContent.BuffType<Irradiated>, () => BuffID.ShadowFlame, ModContent.BuffType<SulphuricPoisoning>, ModContent.BuffType<HolyFlames>,
        };

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            var cp = Owner(projectile);
            if (cp == null || !cp.Zenith || !(projectile.minion || ProjectileID.Sets.MinionShot[projectile.type]) || projectile.type == ModContent.ProjectileType<Projectiles.Summon.SkeletalLimb>())
                return;
            foreach (var debuff in ZenithDebuffs)
                target.AddBuff(debuff(), 180);
            if (cp.LimbCooldown <= 0 && projectile.owner == Main.myPlayer)
            {
                cp.LimbCooldown = 30;
                Vector2 from = target.Center + new Vector2(Main.rand.NextFloat(-80f, 80f), 260f);
                Projectile.NewProjectile(projectile.GetSource_FromThis(), from, (target.Center - from).SafeNormalize(-Vector2.UnitY) * 16f,
                    ModContent.ProjectileType<Projectiles.Summon.SkeletalLimb>(), (int)cp.Player.GetTotalDamage<SummonDamageClass>().ApplyTo(660), 3f, projectile.owner);
            }
        }
    }

    public class Coverings1120Checks : ModSystem
    {
        public override void Load()
        {
            SelfTest.ExtraChecks.Add(t =>
            {
                foreach (int type in new[] { ModContent.ItemType<QuantumFlahiumShockDevice>(), ModContent.ItemType<QuantumFlahiumEjectionDevice>(), ModContent.ItemType<ZenithArcanum>() })
                    t.Expect(Main.recipe.Take(Recipe.numRecipes).Any(r => r.createItem.type == type && !r.Disabled), $"recette du wiki de {Lang.GetItemNameValue(type)}");
            });
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Typeless
{
    /// <summary>Éclat de flah (Shock Device) : Flahmes 3 s.</summary>
    public class FlahShard : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Typeless/FlahShard";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 50;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Lighting.AddLight(Projectile.Center, 0.9f, 0.3f, 0.6f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Buffs.Flahmes>(), 180);

        public override Color? GetAlpha(Color lightColor) => Color.White;
    }
}

namespace CalamityFandom.Content.Projectiles.Rogue
{
    /// <summary>Flahblade (Ejection Device) : 80 dégâts de spy, fonce sur les ennemis ; Flahmes 3 s.</summary>
    public class Flahblade : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Rogue/Flahblade";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = 2;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void AI()
        {
            NPC t = Projectile.FindTargetWithinRange(900f);
            if (t != null)
                Projectile.velocity = (Projectile.velocity * 9f + Projectile.SafeDirectionTo(t.Center) * 20f) / 10f;
            Projectile.rotation += 0.4f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Buffs.Flahmes>(), 180);

        public override Color? GetAlpha(Color lightColor) => Color.White;
    }

    /// <summary>Flare de flahium (frappe furtive de l'Ejection Device) : tombe du ciel, explose.</summary>
    public class FlahiumFlare : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Typeless/FlahShard";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
            Projectile.scale = 1.6f;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Dust.NewDustPerfect(Projectile.Center, DustID.PinkTorch, Vector2.Zero, 0, default, 1.4f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Buffs.Flahmes>(), 180);

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14 with { Volume = 0.5f }, Projectile.Center);
            for (int i = 0; i < 15; i++)
                Dust.NewDustPerfect(Projectile.Center, DustID.PinkTorch, Main.rand.NextVector2Circular(6f, 6f), 0, default, 1.6f).noGravity = true;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;
    }
}

namespace CalamityFandom.Content.Projectiles.Summon
{
    /// <summary>Membre squelettique (Zenith Arcanum) : jaillit vers l'ennemi, 660, Vulnerability Hex 3 s.</summary>
    public class SkeletalLimb : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Summon/SkeletalLimb";

        public override void SetStaticDefaults() => ProjectileID.Sets.MinionShot[Type] = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 40;
            Projectile.tileCollide = false;
        }

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<VulnerabilityHex>(), 180);
    }
}
