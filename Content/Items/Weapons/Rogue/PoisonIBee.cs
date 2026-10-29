using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Items.Weapons.Rogue
{
    /// <summary>
    /// Wiki : Poison I-Bee.txt — boomerang spy inventé par le wiki, lâché par le Plaguebringer Goliath (WikiLoot) : disques
    /// rapides qui libèrent des épines en touchant (35 % des dégâts, ignorent l'invulnérabilité) ; attaque furtive : le
    /// disque projette des graines dans tous les sens. Plague : 4 s (disque), 3 s (épine), 2 s (graine). Valeurs du wiki :
    /// 70 dégâts, recul 4, use 15, rareté 8, vente 16 or. [choix] 3 épines par coup, une graine toutes les 8 ticks ;
    /// vélocité 14 (absente de l'infobox).
    /// </summary>
    public class PoisonIBee : RogueWeapon
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 56;
            Item.height = 54;
            Item.damage = 70;
            Item.knockBack = 4f;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = Item.useAnimation = 15;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<PoisonIBeeDisk>();
            Item.shootSpeed = 14f;
            Item.value = Item.sellPrice(gold: 16);
            Item.rare = ItemRarityID.Yellow;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
        }

        // [choix] « rapidly throws disks » : plusieurs disques à la fois, mais 3 au plus (sans limite, ils s'empilaient à l'infini)
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 3;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (player.Calamity().StealthStrikeAvailable() && p.WithinBounds(Main.maxProjectiles))
                Main.projectile[p].Calamity().stealthStrike = true;
            return false;
        }
    }

    /// <summary>Disque du Poison I-Bee : boomerang (aiStyle de vanilla), épines au contact, graines en furtif.</summary>
    public class PoisonIBeeDisk : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Rogue/PoisonIBeeDisk";

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.aiStyle = ProjAIStyleID.Boomerang;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
        }

        public override void AI()
        {
            if (Projectile.Calamity().stealthStrike && ++Projectile.localAI[0] % 8f == 0f && Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2CircularEdge(9f, 9f),
                    ModContent.ProjectileType<IBeeSeed>(), (int)(Projectile.damage * 0.35f), 0f, Projectile.owner);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 240);
            if (Projectile.owner != Main.myPlayer)
                return;
            for (int i = 0; i < 3; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Main.rand.NextVector2CircularEdge(7f, 7f),
                    ModContent.ProjectileType<IBeeThorn>(), (int)(Projectile.damage * 0.35f), 0f, Projectile.owner);
        }
    }

    /// <summary>Épine du Poison I-Bee : ignore l'invulnérabilité des ennemis, Plague 3 s.</summary>
    public class IBeeThorn : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Rogue/IBeeThorn";

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.timeLeft = 90;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
        }

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 180);
            target.immune[Projectile.owner] = 0;
        }
    }

    /// <summary>Graine du Poison I-Bee (attaque furtive) : imprécise, tombe, Plague 2 s.</summary>
    public class IBeeSeed : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Rogue/IBeeSeed";

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.timeLeft = 120;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.15f;
            Projectile.rotation += 0.2f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Plague>(), 120);
    }
}
