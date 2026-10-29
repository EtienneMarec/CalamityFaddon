// Nom et comète repris de Calamity Mod (objet retiré en 2022), © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// L'effet est celui de la page du wiki (décision du 30/10), écrit pour ce mod.
using CalamityFandom.Common.Shortstrike;
using CalamityFandom.Content.Legacy.Projectiles.Melee;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Exo Gladius.txt — épée courte finale (Draedon's Forge, recette du wiki) : estoc omnidirectionnel qui fait
    /// exploser l'ennemi touché (Miracle Blight 0,5 s). Valeurs du wiki : 690 dégâts, use 8, recul 9,9, rareté 15, vente
    /// 30 or. Sprite du wiki, à sa taille (64×68).
    /// [validé 30/10] Pas d'attaque au clic droit (la parade de la page) : le renvoi des projectiles ennemis vers le
    /// curseur n'a lieu que pendant le Shortflurry. Shortflurry : « Every 5th strike sends out a shotgun blast of 8
    /// healing exo comets » (données : spawnOnStab, every = 5, tools/shortstrike_extra.json).
    /// [choix] Explosion : dégâts de l'estoc, 96 px. TODO : en multijoueur, les projectiles ennemis (gérés par le serveur)
    /// ne sont pas encore renvoyés.
    /// </summary>
    public class ExoGladius : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Item.useAnimation = Item.useTime = 8;
            Item.width = 64;
            Item.height = 68;
            Item.damage = 690;
            Item.knockBack = 9.9f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<ExoGladiusProj>();
            Item.shootSpeed = 2f;
            Item.value = Item.sellPrice(gold: 30);
            Item.rare = ModContent.RarityType<CalamityMod.Rarities.BurnishedAuric>();
        }
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.Melee
{
    /// <summary>Estoc de l'Exo Gladius (et de ses clones de Shortflurry) ; pendant le Shortflurry, renvoie les projectiles ennemis proches.</summary>
    public class ExoGladiusProj : LegacyShortswordProj
    {
        protected override int ItemType => ModContent.ItemType<Items.Weapons.Melee.ExoGladius>();
        protected override int DustType => 107;

        public override void ExtraBehavior()
        {
            base.ExtraBehavior();
            Player player = Main.player[Projectile.owner];
            if (Main.netMode != NetmodeID.SinglePlayer || player.whoAmI != Main.myPlayer || !player.GetModPlayer<ShortstrikePlayer>().FlurryActive)
                return;
            Rectangle guard = player.Hitbox;
            guard.Inflate(64, 64);
            foreach (Projectile other in Main.ActiveProjectiles)
            {
                if (!other.hostile || other.friendly || other.damage <= 0 || !other.Hitbox.Intersects(guard))
                    continue;
                other.hostile = false;
                other.friendly = true;
                other.owner = player.whoAmI;
                other.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
                other.velocity = (Main.MouseWorld - other.Center).SafeNormalize(-other.velocity) * System.Math.Max(other.velocity.Length(), 12f);
                SoundEngine.PlaySound(SoundID.Item150, other.Center);
                for (int i = 0; i < 8; i++)
                    Dust.NewDustPerfect(other.Center, 107, Main.rand.NextVector2Circular(4f, 4f), 100, default, 1.3f).noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<MiracleBlight>(), 30);
            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<ExoGladiusExplosion>(),
                    Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }

    /// <summary>Explosion de l'Exo Gladius (image animée du wiki), Miracle Blight 0,5 s.</summary>
    public class ExoGladiusExplosion : ModProjectile
    {
        public override void SetStaticDefaults() => Main.projFrames[Type] = 5;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 96;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 15;
            Projectile.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.scale = 1.5f;
        }

        public override void AI()
        {
            if (Projectile.localAI[0]++ == 0f)
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            Projectile.frame = System.Math.Min(Main.projFrames[Type] - 1, (int)(Projectile.localAI[0] / 3f));
            Lighting.AddLight(Projectile.Center, 0.3f, 0.8f, 0.8f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<MiracleBlight>(), 30);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            int frameHeight = tex.Height / Main.projFrames[Type];
            var frame = new Rectangle(0, frameHeight * Projectile.frame, tex.Width, frameHeight);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, frame, Color.White, 0f, frame.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }

    /// <summary>
    /// Comète exo du Shortflurry de l'Exo Gladius (ExoGladComet de 2022, sprite Exocomet) : file en accélérant, explose au
    /// contact ; ai[0] = soin à chaque ennemi touché. [choix] 2 PV par comète (huit par salve).
    /// </summary>
    public class ExoGladComet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 5;
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Projectile.penetrate = 1;
            Projectile.alpha = 50;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            if (++Projectile.frameCounter > 5)
            {
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
                Projectile.frameCounter = 0;
            }
            if (Main.rand.NextBool(2))
                Dust.NewDustPerfect(Projectile.Center - Projectile.velocity * 2f, 107, -Projectile.velocity * 0.25f, 100, new Color(0, 255, 255), 0.6f).noGravity = true;
            Lighting.AddLight(Projectile.Center, 0f, 0.5f, 0.5f);
            Projectile.velocity *= 1.005f;
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi) + MathHelper.PiOver2 * Projectile.direction;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<MiracleBlight>(), 30);
            Player player = Main.player[Projectile.owner];
            int heal = (int)Projectile.ai[0];
            if (heal > 0 && Projectile.owner == Main.myPlayer && !player.moonLeech)
                player.Heal(heal);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(0, 255, 255, Projectile.alpha);

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int i = 0; i < 12; i++)
                Dust.NewDustPerfect(Projectile.Center, 107, Main.rand.NextVector2Circular(3f, 3f), 0, new Color(0, 255, 255), 1.5f).noGravity = true;
        }
    }
}
