using CalamityFandom.Content.Items.Weapons.Melee;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Projectiles.Melee
{
    /// <summary>
    /// Épée tenue de la Zweihandromeda : balaie un arc vers le curseur, alternativement de haut en bas
    /// puis de bas en haut (« swings in a back and forth pattern »). ai[0] = sens du coup (1 ou -1).
    /// </summary>
    public class ZweihandromedaSwing : ModProjectile
    {
        public override string Texture => ModContent.GetInstance<Zweihandromeda>().Texture;

        // Point de prise sur le sprite (bas de la poignée, côté pommeau) et longueur de lame depuis ce point
        private static readonly Vector2 GripOrigin = new(14f, 216f);
        private const float BladeLength = 306f;
        // Demi-angle balayé de part et d'autre de la direction visée
        private const float ArcHalf = 1.9f;

        private Player Owner => Main.player[Projectile.owner];
        private float SwingSide => Projectile.ai[0] == 0f ? 1f : Projectile.ai[0];
        private float Timer
        {
            get => Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.ownerHitCheck = true;
            // Chaque ennemi n'est touché qu'une fois par coup
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 2;
        }

        public override void AI()
        {
            Player owner = Owner;
            if (!owner.active || owner.dead || owner.HeldItem.type != ModContent.ItemType<Zweihandromeda>())
            {
                Projectile.Kill();
                return;
            }

            int swingTime = owner.itemAnimationMax;
            Timer++;
            if (Timer > swingTime)
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 2;

            // La vitesse initiale ne sert qu'à mémoriser la direction visée
            float aim = Projectile.velocity.ToRotation();
            float direction = Projectile.velocity.X >= 0f ? 1f : -1f;
            float progress = MathHelper.SmoothStep(0f, 1f, Timer / swingTime);
            float start = aim - ArcHalf * SwingSide * direction;
            float end = aim + ArcHalf * SwingSide * direction;
            Projectile.rotation = MathHelper.Lerp(start, end, progress);

            Projectile.scale = owner.GetAdjustedItemScale(owner.HeldItem);
            Projectile.direction = Projectile.spriteDirection = (int)direction;

            owner.ChangeDir((int)direction);
            owner.heldProj = Projectile.whoAmI;
            float armRotation = Projectile.rotation - MathHelper.PiOver2;
            owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRotation);
            // L'épée est ancrée dans la main (bout du bras tendu), pas au centre du joueur
            Projectile.Center = owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, armRotation);

            Lighting.AddLight(Projectile.Center + Projectile.rotation.ToRotationVector2() * BladeLength * 0.6f, 0.3f, 0.5f, 0.9f);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Projectile.Center;
            Vector2 end = start + Projectile.rotation.ToRotationVector2() * BladeLength * Projectile.scale;
            float _ = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 40f * Projectile.scale, ref _);
        }

        public override void CutTiles()
        {
            Vector2 end = Projectile.Center + Projectile.rotation.ToRotationVector2() * BladeLength * Projectile.scale;
            Utils.PlotTileLine(Projectile.Center, end, 40f * Projectile.scale, DelegateMethods.CutTiles);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Nightwither>(), 180);

            // Étoiles de Stratus qui tombent du ciel sur la cible
            for (int i = 0; i < Zweihandromeda.StarsPerHit; i++)
            {
                Vector2 spawn = target.Center + new Vector2(Main.rand.NextFloat(-200f, 200f), -800f);
                Vector2 velocity = (target.Center - spawn).SafeNormalize(Vector2.UnitY) * 22f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawn, velocity, ModContent.ProjectileType<StratusStar>(),
                    Projectile.damage / 2, 0f, Projectile.owner);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            bool flip = Projectile.spriteDirection < 0;
            // Le sprite pointe en haut à droite (-45°) ; retourné, il pointe en haut à gauche (-135°)
            SpriteEffects effects = flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 origin = flip ? new Vector2(texture.Width - GripOrigin.X, GripOrigin.Y) : GripOrigin;
            float spriteAngle = flip ? -3f * MathHelper.PiOver4 : -MathHelper.PiOver4;
            Vector2 position = Projectile.Center - Main.screenPosition;

            // Traînée : anciennes rotations, de plus en plus transparentes
            for (int i = Projectile.oldRot.Length - 1; i > 0; i--)
            {
                if (Projectile.oldRot[i] == 0f)
                    continue;
                Color trail = new Color(80, 140, 255, 0) * (0.35f * (1f - i / (float)Projectile.oldRot.Length));
                Main.EntitySpriteDraw(texture, position, null, trail, Projectile.oldRot[i] - spriteAngle, origin, Projectile.scale, effects);
            }

            Main.EntitySpriteDraw(texture, position, null, Projectile.GetAlpha(lightColor), Projectile.rotation - spriteAngle, origin, Projectile.scale, effects);
            return false;
        }

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, Color.White, 0.6f);
    }
}
