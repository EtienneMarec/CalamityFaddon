// Adapté de Calamity Mod (épées courtes retirées en 2022), © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using CalamityMod;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Projectiles.Melee
{
    /// <summary>
    /// Estoc d'une ancienne épée courte de Calamity : elles frappaient avec la hitbox de l'objet (Thrust) ; portées en estoc
    /// (Rapier + projectile, comme les épées courtes actuelles) pour avoir la jauge Shortstrike et de vrais clones de
    /// Shortflurry. L'image est celle de l'objet ; un coup par estoc et par ennemi.
    /// </summary>
    public abstract class LegacyShortswordProj : BaseShortswordProjectile
    {
        /// <summary>Objet dont c'est l'estoc (image, poussière).</summary>
        protected abstract int ItemType { get; }

        /// <summary>Poussière de l'ancien MeleeEffects (une chance sur DustChance), -1 pour aucune.</summary>
        protected virtual int DustType => -1;
        protected virtual int DustChance => 5;

        // Image de l'objet ; son resprite du wiki s'il en a un (tools/gen_sprites.py), pour que l'estoc ressemble à l'icône
        public override string Texture
        {
            get
            {
                ModItem item = ModContent.GetModItem(ItemType);
                string generated = $"Assets/Overrides/Generated/Items/{item.Name}";
                return Mod.FileExists(generated + ".rawimg") || Mod.FileExists(generated + ".png") ? $"{Mod.Name}/{generated}" : item.Texture;
            }
        }

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(16);
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Projectile.ownerHitCheck = true;
            Projectile.timeLeft = 360;
            Projectile.hide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void SetVisualOffsets()
        {
            // Image centrée sur le projectile, quelle que soit sa taille (32×32, 42×38...)
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            int halfWidth = texture.Width() / 2, halfHeight = texture.Height() / 2;
            DrawOriginOffsetX = 0;
            DrawOffsetX = -(halfWidth - Projectile.width / 2);
            DrawOriginOffsetY = -(halfHeight - Projectile.height / 2);
        }

        public override void ExtraBehavior()
        {
            if (DustType >= 0 && Main.rand.NextBool(DustChance))
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustType);
        }

        /// <summary>Dégâts d'un projectile de soutien : la moitié de ceux de l'estoc (ancien code : moitié des dégâts de base, bonus compris).</summary>
        protected int HalfDamage => (int)(Projectile.damage * 0.5f);
    }

    /// <summary>
    /// Orbe qui cherche un ennemi après un court délai (sangsue de la Leeching Dagger et sang de la Bloody Rupture, qui
    /// soignent de 2 PV : « heal the player for 2 life » ; énergie sombre du Night's Stabber, sans soin). ai[0] = poussière,
    /// ai[1] = soin.
    /// </summary>
    public class LegacyHealingOrb : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override bool? CanHitNPC(NPC target) => Projectile.timeLeft < 150 && target.CanBeChasedBy(Projectile) ? null : false;

        public override void AI()
        {
            Projectile.localAI[0]++;
            if (Projectile.localAI[0] > 4f)
            {
                for (int i = 0; i < 2; i++)
                {
                    var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, (int)Projectile.ai[0], 0f, 0f, 100);
                    dust.noGravity = true;
                    dust.velocity *= 0f;
                }
            }
            if (Projectile.timeLeft < 150)
                CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 600f, 6f, 20f);
        }

        public override void OnKill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, (int)Projectile.ai[0], Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.ai[1] <= 0f || player.moonLeech || Projectile.owner != Main.myPlayer)
                return;
            player.Heal((int)Projectile.ai[1]);
        }
    }
}
