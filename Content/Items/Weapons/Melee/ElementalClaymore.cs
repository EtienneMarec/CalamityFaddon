using CalamityFandom.Content.Legacy;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Elemental Claymore.txt — tire un Eon Beam d'une couleur au hasard (rose, vert, bleu, orange) qui traverse 2
    /// ennemis et ignore les blocs, avec une invulnérabilité de 6, 5, 4 ou 3 ticks selon la couleur, et une colonne de 4
    /// orbes élémentaires lancées sans grande précision vers le curseur (traversent les blocs, tête chercheuse, 50 % des
    /// dégâts). La lame, les rayons et les orbes infligent Elemental Mix 2 s. [choix] Les orbes sont les Eon Bolts de
    /// Calamity (même effet : traînée colorée, tête chercheuse, ignorent les blocs).
    /// </summary>
    public class ElementalClaymore : ModItem
    {
        public static readonly float[] Hues = { 0.9f, 0.33f, 0.6f, 0.08f };

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = Item.height = 54;
            Item.damage = 115;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 19;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.knockBack = 8.5f;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<Projectiles.Melee.EonBeam>();
            Item.shootSpeed = 16f;
            Item.value = Item.sellPrice(gold: 22);
            Item.rare = ItemRarityID.Purple;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, Main.rand.Next(4));
            // Colonne de 4 orbes vers le curseur, peu précises
            Vector2 side = Vector2.Normalize(velocity).RotatedBy(MathHelper.PiOver2) * 18f;
            for (int i = 0; i < 4; i++)
            {
                Vector2 dir = velocity.RotatedByRandom(0.25f) * Main.rand.NextFloat(0.6f, 0.8f);
                int p = Projectile.NewProjectile(source, position + side * (i - 1.5f), dir, ModContent.ProjectileType<EonBolt>(),
                    damage / 2, knockback * 0.5f, player.whoAmI, Hues[i], 0.15f);
                if (p.WithinBounds(Main.maxProjectiles))
                    LegacyHitDebuff.Give(Main.projectile[p], ModContent.BuffType<ElementalMix>(), 120);
            }
            return false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
                Dust.NewDust(hitbox.TopLeft(), hitbox.Width, hitbox.Height, DustID.RainbowTorch, 0f, 0f, 0, Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.6f));
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) =>
            target.AddBuff(ModContent.BuffType<ElementalMix>(), 120);
    }
}

namespace CalamityFandom.Content.Projectiles.Melee
{
    /// <summary>
    /// Eon Beam de l'Elemental Claymore (ai[0] = couleur : 0 rose, 1 vert, 2 bleu, 3 orange) : rayon d'épée teinté, traverse
    /// 2 ennemis, ignore les blocs, invulnérabilité locale de 6 / 5 / 4 / 3 ticks ; Elemental Mix 2 s.
    /// </summary>
    public class EonBeam : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.TerraBeam;

        private int Shade => (int)Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.aiStyle = ProjAIStyleID.Beam;
            AIType = ProjectileID.TerraBeam;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 3;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 120;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override void OnSpawn(IEntitySource source) => Projectile.localNPCHitCooldown = 6 - Shade;

        public override void AI()
        {
            Projectile.localNPCHitCooldown = 6 - Shade;
            Vector3 light = Main.hslToRgb(Items.Weapons.Melee.ElementalClaymore.Hues[Shade], 1f, 0.5f).ToVector3() * 0.6f;
            Lighting.AddLight(Projectile.Center, light);
        }

        public override Color? GetAlpha(Color lightColor) =>
            Main.hslToRgb(Items.Weapons.Melee.ElementalClaymore.Hues[Shade], 1f, 0.65f) * ((255 - Projectile.alpha) / 255f);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<ElementalMix>(), 120);
    }
}
