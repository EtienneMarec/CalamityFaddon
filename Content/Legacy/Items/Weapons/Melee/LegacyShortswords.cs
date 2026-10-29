// Adapté de Calamity Mod (épées courtes retirées en 2022), © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
using CalamityFandom.Content.Legacy.Projectiles.Melee;
using CalamityMod;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.Weapons.Melee
{
    /// <summary>
    /// Ancienne épée courte de Calamity (retirée en 2022, avant 2.0.1.005), portée en estoc : valeurs du wiki, recette et butin
    /// par les systèmes du wiki, Shortflurry par la page Shortstrike.
    /// </summary>
    public abstract class LegacyShortsword : ModItem
    {
        protected abstract int Damage { get; }
        protected abstract int UseTime { get; }
        protected abstract float Knockback { get; }
        protected abstract int Rarity { get; }
        protected abstract int SellSilver { get; }
        protected abstract int StabProjectile { get; }

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Item.useAnimation = Item.useTime = UseTime;
            Item.width = Item.height = 32;
            Item.damage = Damage;
            Item.knockBack = Knockback;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.shoot = StabProjectile;
            Item.shootSpeed = 2f;
            Item.value = Item.sellPrice(silver: SellSilver);
            Item.rare = Rarity;
        }
    }

    /// <summary>Wiki : Leeching Dagger.txt — un ennemi touché libère une sangsue qui cherche un ennemi et soigne de 2 PV.</summary>
    public class LeechingDagger : LegacyShortsword
    {
        protected override int Damage => 33;
        protected override int UseTime => 15;
        protected override float Knockback => 5.25f;
        protected override int Rarity => ItemRarityID.Orange;
        protected override int SellSilver => 80;
        protected override int StabProjectile => ModContent.ProjectileType<LeechingDaggerProj>();
    }

    /// <summary>Wiki : Bloody Rupture.txt — un ennemi tué libère un orbe de sang qui cherche un ennemi et soigne de 2 PV.</summary>
    public class BloodyRupture : LegacyShortsword
    {
        protected override int Damage => 33;
        protected override int UseTime => 15;
        protected override float Knockback => 5.5f;
        protected override int Rarity => ItemRarityID.Orange;
        protected override int SellSilver => 80;
        protected override int StabProjectile => ModContent.ProjectileType<BloodyRuptureProj>();
    }

    /// <summary>Wiki : Ancient Shiv.txt — un ennemi tué libère un nuage d'aura bleue (5 s, moitié des dégâts, perce sans fin).</summary>
    public class AncientShiv : LegacyShortsword
    {
        protected override int Damage => 35;
        protected override int UseTime => 12;
        protected override float Knockback => 6f;
        protected override int Rarity => ItemRarityID.Orange;
        protected override int SellSilver => 80;
        protected override int StabProjectile => ModContent.ProjectileType<AncientShivProj>();
    }

    /// <summary>
    /// Wiki : Tomatus.txt — ancienne Flameburst Shortsword de Calamity (renommée Tomutus puis Tomatus par le Fandom) : un ennemi
    /// touché explose (explosion de Calamity, 75 % des dégâts, On Fire! 3 s).
    /// </summary>
    public class Tomatus : LegacyShortsword
    {
        protected override int Damage => 35;
        protected override int UseTime => 15;
        protected override float Knockback => 5.5f;
        protected override int Rarity => ItemRarityID.Orange;
        protected override int SellSilver => 80;
        protected override int StabProjectile => ModContent.ProjectileType<TomatusProj>();
    }

    /// <summary>Wiki : Night_s Stabber.txt — un ennemi touché libère une énergie sombre qui cherche un autre ennemi (pleins dégâts).</summary>
    public class NightsStabber : LegacyShortsword
    {
        protected override int Damage => 60;
        protected override int UseTime => 15;
        protected override float Knockback => 6.5f;
        protected override int Rarity => ItemRarityID.Orange;
        protected override int SellSilver => 80;
        protected override int StabProjectile => ModContent.ProjectileType<NightsStabberProj>();
    }

    /// <summary>Wiki : Eutrophic Shank.txt — chaque estoc projette une étincelle électrique qui reste au sol (40 % des dégâts).</summary>
    public class EutrophicShank : LegacyShortsword
    {
        protected override int Damage => 35;
        protected override int UseTime => 14;
        protected override float Knockback => 4f;
        protected override int Rarity => ItemRarityID.Green;
        protected override int SellSilver => 40;
        protected override int StabProjectile => ModContent.ProjectileType<EutrophicShankProj>();
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.Melee
{
    using global::CalamityFandom.Content.Legacy.Items.Weapons.Melee;

    public class LeechingDaggerProj : LegacyShortswordProj
    {
        protected override int ItemType => ModContent.ItemType<LeechingDagger>();
        protected override int DustType => DustID.Demonite;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // « Upon hitting an enemy with it, it will release leech orbs »
            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<LegacyHealingOrb>(),
                    HalfDamage, hit.Knockback, Projectile.owner, DustID.Demonite, 2f);
        }
    }

    public class BloodyRuptureProj : LegacyShortswordProj
    {
        protected override int ItemType => ModContent.ItemType<BloodyRupture>();
        protected override int DustType => DustID.Blood;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // « Upon killing an enemy with it » (le vieux code le faisait à chaque coup ; le wiki fait foi)
            if (target.life <= 0 && Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<LegacyHealingOrb>(),
                    HalfDamage, hit.Knockback, Projectile.owner, DustID.Blood, 2f);
        }
    }

    public class AncientShivProj : LegacyShortswordProj
    {
        protected override int ItemType => ModContent.ItemType<AncientShiv>();
        protected override int DustType => DustID.MagicMirror;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // « Upon killing an enemy with this sword, they will release a small blue aura cloud »
            if (target.life <= 0 && Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<BlueAura>(),
                    HalfDamage, hit.Knockback, Projectile.owner);
        }
    }

    public class EutrophicShankProj : LegacyShortswordProj
    {
        protected override int ItemType => ModContent.ItemType<EutrophicShank>();
        protected override int DustType => 206;
        protected override int DustChance => 3;

        // L'étincelle part à chaque estoc (Shoot du vieux code : vitesse 10, légère dispersion, 40 % des dégâts)
        public override System.Action<Projectile> EffectBeforePullback => proj =>
        {
            Vector2 velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX) * 10f + new Vector2(Main.rand.Next(-60, 61), Main.rand.Next(-60, 61)) * 0.05f;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<AquaticDischargeSpark>(),
                (int)(Projectile.damage * 0.4f), Projectile.knockBack, Projectile.owner);
        };
    }

    public class TomatusProj : LegacyShortswordProj
    {
        protected override int ItemType => ModContent.ItemType<Tomatus>();
        protected override int DustType => DustID.Torch;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 180);
            if (Projectile.owner != Main.myPlayer)
                return;
            // « Hitting an enemy will make a small explosion » : explosion de Calamity (FuckYou), taille au hasard comme avant
            var boom = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero,
                ModContent.ProjectileType<CalamityMod.Projectiles.Typeless.FuckYou>(), (int)(Projectile.damage * 0.75f), hit.Knockback,
                Projectile.owner, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
            boom.DamageType = DamageClass.Melee;
        }
    }

    public class NightsStabberProj : LegacyShortswordProj
    {
        protected override int ItemType => ModContent.ItemType<NightsStabber>();
        protected override int DustType => DustID.Demonite;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<LegacyHealingOrb>(),
                    Projectile.damage, hit.Knockback, Projectile.owner, DustID.Demonite, 0f);
        }
    }

    /// <summary>Nuage d'aura bleue de l'Ancient Shiv : immobile 5 s, perce sans fin, touche toutes les 10 frames.</summary>
    public class BlueAura : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            for (int i = 0; i < 3; i++)
            {
                var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.MagicMirror, 0f, 0f, 100, default, 1.3f);
                dust.noGravity = true;
                dust.velocity *= 0f;
            }
        }
    }
}
