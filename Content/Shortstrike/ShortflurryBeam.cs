using System;
using System.Linq;
using CalamityFandom.Common.Shortstrike;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Shortstrike
{
    /// <summary>
    /// Rayon d'un Shortflurry (effet « flurryBeam » de la page Shortstrike : True Caliburn, True Night's Stabber, Terra Shiv,
    /// [choix] du 29/10). Part de l'ennemi touché dans la direction du clone et parcourt Value blocs en perçant sans fin ;
    /// en route, tous les Spread blocs, il lâche Rain (colonnes de lumière, boules de feu maudites) ; au bout, il se divise
    /// en Count rayons (DamageMult des dégâts) ; chaque coup soigne de Ai0 PV et inflige Name. ai[0] = type de l'arme,
    /// ai[1] = 1 pour un rayon issu d'une division.
    /// </summary>
    public class ShortflurryBeam : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private Vector2 start;
        private float nextRain;

        private ShortstrikeData.Effect Effect =>
            ContentSamples.ItemsByType.TryGetValue((int)Projectile.ai[0], out var item)
                ? ShortstrikeData.For(item).Effects.FirstOrDefault(e => e.Type == "flurryBeam") : null;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void AI()
        {
            var effect = Effect;
            if (effect == null)
            {
                Projectile.Kill();
                return;
            }
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                start = Projectile.Center;
                nextRain = Math.Max(1f, effect.Spread) * 16f;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            Lighting.AddLight(Projectile.Center, 0.4f, 0.4f, 0.3f);
            float travelled = Vector2.Distance(start, Projectile.Center);
            float length = (Projectile.ai[1] == 1f ? 0.5f : 1f) * effect.Value * 16f;
            if (Projectile.owner == Main.myPlayer && effect.RainType > 0 && travelled >= nextRain)
            {
                nextRain += Math.Max(1f, effect.Spread) * 16f;
                Rain(effect);
            }
            if (travelled >= length)
            {
                if (Projectile.owner == Main.myPlayer && Projectile.ai[1] == 0f)
                    Split(effect);
                Projectile.Kill();
            }
        }

        /// <summary>Colonnes de lumière (une vers le haut, une vers le bas) ou boule de feu dans une direction au hasard.</summary>
        private void Rain(ShortstrikeData.Effect effect)
        {
            bool beam = ContentSamples.ProjectilesByType.TryGetValue(effect.RainType, out var sample) && sample.aiStyle == ProjAIStyleID.Beam;
            var directions = beam ? new[] { -Vector2.UnitY, Vector2.UnitY } : new[] { Main.rand.NextVector2CircularEdge(1f, 1f) };
            foreach (var direction in directions)
            {
                var shot = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, direction * 12f, effect.RainType,
                    Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
                shot.DamageType = Projectile.DamageType;
            }
        }

        /// <summary>Division en Count rayons écartés de 25°, moitié de la longueur, DamageMult des dégâts.</summary>
        private void Split(ShortstrikeData.Effect effect)
        {
            for (int i = 0; i < effect.Count; i++)
            {
                float angle = MathHelper.ToRadians(25f) * (i - (effect.Count - 1) / 2f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.RotatedBy(angle),
                    Type, (int)(Projectile.damage * effect.DamageMult), Projectile.knockBack, Projectile.owner, Projectile.ai[0], 1f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            var effect = Effect;
            if (effect == null)
                return;
            if (effect.BuffType > 0)
                target.AddBuff(effect.BuffType, (int)((effect.Seconds ?? 5f) * 60));
            if (effect.Ai0 > 0f && Projectile.owner == Main.myPlayer)
                Main.player[Projectile.owner].Heal((int)effect.Ai0);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var effect = Effect;
            if (effect == null || effect.ProjectileType <= 0)
                return false;
            Main.instance.LoadProjectile(effect.ProjectileType);
            Texture2D texture = TextureAssets.Projectile[effect.ProjectileType].Value;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White * 0.9f, Projectile.rotation,
                texture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
