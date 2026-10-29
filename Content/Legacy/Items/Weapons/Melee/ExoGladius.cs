// Sprite et nom repris de Calamity Mod (objet retiré en 2022), © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// L'effet est celui de la page du wiki (décision du 30/10), écrit pour ce mod.
using CalamityFandom.Content.Legacy.Projectiles.Melee;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Exo Gladius.txt — épée courte finale (Draedon's Forge, recette du wiki) : estoc omnidirectionnel qui fait
    /// exploser l'ennemi touché (Miracle Blight 0,5 s). Clic droit : parade, 4 fois les dégâts, le recul et le critique
    /// de l'estoc, use 40 au lieu de 8 ; elle renvoie les projectiles ennemis vers le curseur et rend le joueur
    /// invulnérable pendant l'estoc. Jauge Shortstrike pleine : le clic droit lance le Shortflurry (règle commune), la
    /// parade n'est donc possible que jauge non pleine. Valeurs du wiki : 690 dégâts, recul 9,9, rareté 15, vente 30 or.
    /// [choix] Explosion : dégâts de l'estoc, 96 px. Invulnérabilité de la parade : 30 frames. Sprite du wiki.
    /// TODO : en multijoueur, les projectiles ennemis (gérés par le serveur) ne sont pas encore renvoyés.
    /// </summary>
    public class ExoGladius : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Item.useAnimation = Item.useTime = 8;
            Item.width = 32;
            Item.height = 34;
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

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            // « alternate fire use = 40 »
            Item.useAnimation = Item.useTime = player.altFunctionUse == 2 ? 40 : 8;
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool parry = player.altFunctionUse == 2;
            // ai[0] est le minuteur de l'estoc (BaseShortswordProjectile) : la parade est notée dans ai[2]
            var stab = Projectile.NewProjectileDirect(source, position, velocity, type, parry ? damage * 4 : damage, parry ? knockback * 4f : knockback, player.whoAmI, 0f, 0f, parry ? 1f : 0f);
            if (parry)
            {
                stab.CritChance *= 4;
                player.SetImmuneTimeForAllTypes(30);
                SoundEngine.PlaySound(SoundID.Item71, player.Center);
            }
            return false;
        }
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.Melee
{
    /// <summary>Estoc de l'Exo Gladius ; ai[2] = 1 pour la parade (renvoie les projectiles ennemis proches).</summary>
    public class ExoGladiusProj : LegacyShortswordProj
    {
        protected override int ItemType => ModContent.ItemType<Items.Weapons.Melee.ExoGladius>();
        protected override int DustType => 107;

        private bool Parry => Projectile.ai[2] == 1f;

        public override void ExtraBehavior()
        {
            base.ExtraBehavior();
            if (!Parry)
                return;
            Player player = Main.player[Projectile.owner];
            if (Main.netMode != NetmodeID.SinglePlayer || player.whoAmI != Main.myPlayer)
                return;
            Rectangle guard = player.Hitbox;
            guard.Inflate(64, 64);
            foreach (Projectile other in Main.ActiveProjectiles)
            {
                if (!other.hostile || other.friendly || other.damage <= 0 || !other.Hitbox.Intersects(guard))
                    continue;
                // Renvoyé vers le curseur, au profit du joueur
                other.hostile = false;
                other.friendly = true;
                other.owner = player.whoAmI;
                other.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
                other.velocity = (Main.MouseWorld - other.Center).SafeNormalize(-other.velocity) * System.Math.Max(other.velocity.Length(), 12f);
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

        public override Color? GetAlpha(Color lightColor) => Color.White;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            int frameHeight = tex.Height / Main.projFrames[Type];
            var frame = new Rectangle(0, frameHeight * Projectile.frame, tex.Width, frameHeight);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, frame, Color.White, 0f, frame.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
