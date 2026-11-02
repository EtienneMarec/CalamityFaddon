using CalamityFandom.Content.Buffs;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Gravitonomy Pike.txt — lance « Exo » : chaque coup tire un rayon (« exo pike beam ») qui traverse les murs et
    /// cherche un peu les ennemis ; près d'un ennemi ou en le touchant, il se divise en rayons qui cherchent les ennemis
    /// proches. Frapper avec la lance attire les ennemis proches. Lance et rayons ignorent l'invulnérabilité des PNJ et
    /// infligent Exo Freeze 0,5 s, Holy Flames 2 s, Frostburn 2,5 s et On Fire! 3 s. Valeurs de l'infobox.
    /// [choix] Division : 3 rayons à 50 % des dégâts, une seule fois par rayon ; attraction : 25 blocs, pas les boss.
    /// </summary>
    public class GravitonomyPike : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.Spears[Type] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 82;
            Item.height = 88;
            Item.damage = 257;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useTurn = true;
            Item.useAnimation = Item.useTime = 18;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 9.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.Melee.GravitonomyPikeProj>();
            Item.shootSpeed = 12f;
            Item.value = Item.sellPrice(gold: 30);
            Item.rare = ModContent.RarityType<BurnishedAuric>();
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            Projectile.NewProjectile(source, position, velocity * 1.4f, ModContent.ProjectileType<Projectiles.Melee.GravitonomyBeam>(),
                damage, knockback * 0.5f, player.whoAmI);
            return false;
        }

        /// <summary>Les quatre debuffs de la page (lance et rayons).</summary>
        public static void Inflict(NPC target)
        {
            target.AddBuff(ModContent.BuffType<ExoFreeze>(), 30);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 120);
            target.AddBuff(BuffID.Frostburn, 150);
            target.AddBuff(BuffID.OnFire, 180);
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Melee
{
    /// <summary>Lance de la Gravitonomy Pike : attire les ennemis proches vers celui qu'elle touche.</summary>
    public class GravitonomyPikeProj : BaseSpearProjectile
    {
        private const float PullRange = 25 * 16f;

        public override string Texture => "CalamityFandom/Content/Items/Weapons/Melee/GravitonomyPike";

        public override float InitialSpeed => 3f;
        public override float ReelbackSpeed => 2.4f;
        public override float ForwardSpeed => 0.8f;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 90;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            // « Ignores immunity frames » : invulnérabilité propre à ce projectile
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
        }

        public override void ExtraBehavior()
        {
            if (Main.rand.NextBool(3))
                Dust.NewDustDirect(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.RainbowTorch, 0f, 0f, 100,
                    Main.hslToRgb(Main.rand.NextFloat(), 0.9f, 0.6f), 1.1f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Items.Weapons.Melee.GravitonomyPike.Inflict(target);
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            // « Hitting enemies with the pike sucks in all nearby enemies »
            foreach (var n in Main.ActiveNPCs)
            {
                if (n.whoAmI == target.whoAmI || n.boss || n.friendly || n.knockBackResist <= 0f || !n.WithinRange(target.Center, PullRange))
                    continue;
                n.velocity = (target.Center - n.Center).SafeNormalize(Vector2.Zero) * 10f * n.knockBackResist;
                n.netUpdate = true;
            }
        }
    }

    /// <summary>
    /// Rayon de la Gravitonomy Pike : traverse les murs, cherche un peu les ennemis ; à moins de 8 blocs d'un ennemi (ou en le
    /// touchant), se divise en 3 rayons qui cherchent les ennemis proches (ai[0] = 1 : rayon issu d'une division).
    /// </summary>
    public class GravitonomyBeam : ModProjectile
    {
        private const float SplitRange = 8 * 16f;

        private bool Child => Projectile.ai[0] == 1f;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            Lighting.AddLight(Projectile.Center, Main.hslToRgb((Main.GlobalTimeWrappedHourly * 0.5f) % 1f, 0.8f, 0.5f).ToVector3() * 0.5f);
            // Cherche faiblement (fort pour les rayons issus d'une division)
            CalamityUtils.HomeInOnNPC(Projectile, true, Child ? 500f : 300f, Child ? 14f : 12f, Child ? 12f : 40f);
            if (!Child && Projectile.owner == Main.myPlayer && Projectile.localAI[0] == 0f)
                foreach (var n in Main.ActiveNPCs)
                    if (n.CanBeChasedBy(Projectile) && n.WithinRange(Projectile.Center, SplitRange))
                    {
                        Split();
                        break;
                    }
        }

        private void Split()
        {
            Projectile.localAI[0] = 1f;
            for (int i = 0; i < 3; i++)
            {
                Vector2 velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(-30f + 30f * i));
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, Type, Projectile.damage / 2,
                    Projectile.knockBack, Projectile.owner, 1f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Items.Weapons.Melee.GravitonomyPike.Inflict(target);
            if (!Child && Projectile.owner == Main.myPlayer && Projectile.localAI[0] == 0f)
                Split();
        }

        public override Color? GetAlpha(Color lightColor) => Color.White * Projectile.Opacity;
    }
}
