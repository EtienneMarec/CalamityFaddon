using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Sewer Brewer.txt — perceuse inventée par le wiki, lâchée par le Plaguebringer Goliath (WikiLoot) : on la
    /// charge en tenant le bouton, au relâchement elle tire un foret dont la taille, la vitesse et les dégâts grandissent
    /// avec la charge (600 % des dégâts à pleine charge) ; chargé à fond, il cherche les ennemis ; il ralentit en touchant
    /// (plus de coups) et inflige la Plague 3 s. Pas de modificateur de taille. Valeurs du wiki : 65 dégâts, recul 6,
    /// use 36, rareté 8, vente 16 or. [choix] Charge complète en 90 ticks ; vitesse 8 à 20 ; taille x1 à x2.
    /// </summary>
    public class SewerBrewer : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 24;
            Item.damage = 65;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.knockBack = 6f;
            Item.value = Item.sellPrice(gold: 16);
            Item.rare = ItemRarityID.Yellow;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SewerBrewerHeld>();
            Item.shootSpeed = 1f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] == 0;

        // Pas de modificateur de taille (Notes du wiki)
        public override bool AllowPrefix(int pre) => pre != PrefixID.Large && pre != PrefixID.Massive && pre != PrefixID.Dangerous
            && pre != PrefixID.Bulky && pre != PrefixID.Tiny && pre != PrefixID.Small && pre != PrefixID.Terrible && pre != PrefixID.Shameful;
    }

    /// <summary>Perceuse tenue : se charge pendant que le bouton est tenu, tire le foret au relâchement.</summary>
    public class SewerBrewerHeld : ModProjectile
    {
        public const float FullCharge = 90f;

        public override string Texture => "CalamityFandom/Content/Items/Weapons/Melee/SewerBrewer";

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!owner.active || owner.dead)
            {
                Projectile.Kill();
                return;
            }
            float charge = MathHelper.Min(++Projectile.ai[0] / FullCharge, 1f);
            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 aim = (Main.MouseWorld - owner.MountedCenter).SafeNormalize(Vector2.UnitX * owner.direction);
                if (aim != Projectile.velocity)
                    Projectile.netUpdate = true;
                Projectile.velocity = aim;
            }
            Projectile.Center = owner.MountedCenter + Projectile.velocity * 24f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            owner.ChangeDir(Projectile.velocity.X > 0f ? 1 : -1);
            owner.heldProj = Projectile.whoAmI;
            owner.itemTime = owner.itemAnimation = 2;
            owner.itemRotation = (Projectile.velocity * owner.direction).ToRotation();
            if (charge >= 1f && Projectile.ai[0] == FullCharge)
                SoundEngine.PlaySound(SoundID.Item29, Projectile.Center);
            if (Main.rand.NextFloat() < charge)
                Dust.NewDustPerfect(Projectile.Center + Projectile.velocity * 20f, DustID.GreenTorch, Main.rand.NextVector2Circular(2f, 2f)).noGravity = true;

            if (owner.channel)
            {
                Projectile.timeLeft = 2;
                return;
            }
            // Relâché : foret de 100 % à 600 % des dégâts
            if (Projectile.owner == Main.myPlayer)
            {
                SoundEngine.PlaySound(SoundID.Item22, Projectile.Center);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * MathHelper.Lerp(8f, 20f, charge),
                    ModContent.ProjectileType<SewerDrillBit>(), (int)(Projectile.damage * MathHelper.Lerp(1f, 6f, charge)), Projectile.knockBack,
                    Projectile.owner, charge);
            }
            Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            SpriteEffects flip = Projectile.velocity.X < 0f ? SpriteEffects.FlipVertically : SpriteEffects.None;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, 1f, flip);
            return false;
        }
    }

    /// <summary>Foret de la Sewer Brewer : ai[0] = charge (0 à 1) ; ralentit en touchant ; cherche les ennemis à pleine charge.</summary>
    public class SewerDrillBit : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Melee/SewerDrillBit";

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.penetrate = 6;
            Projectile.timeLeft = 180;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                Projectile.scale = MathHelper.Lerp(1f, 2f, Projectile.ai[0]);
                Projectile.ExpandHitboxBy((int)(20 * Projectile.scale));
            }
            if (Projectile.ai[0] >= 1f)
                CalamityUtils.HomeInOnNPC(Projectile, false, 600f, 20f, 15f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Main.rand.NextBool(3))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GreenTorch).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 180);
            Projectile.velocity *= 0.6f;
        }
    }
}
