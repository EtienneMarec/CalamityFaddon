using System;
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
    /// Wiki : Shortstrike.txt — « clone magique » d'un Shortflurry : l'épée courte (ai[0] = son type) part du joueur vers
    /// le curseur et revient, avec les dégâts boostés et les effets « au coup » de son Shortflurry.
    /// </summary>
    public class ShortflurryClone : ModProjectile
    {
        private const int Lifetime = 14;
        private const float Reach = 56f;

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private Item Weapon => ContentSamples.ItemsByType.TryGetValue((int)Projectile.ai[0], out var item) ? item : null;

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = Lifetime;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1; // un coup par clone et par ennemi
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                Projectile.localAI[1] = Projectile.velocity.ToRotation();
                if (Weapon != null)
                    Projectile.DamageType = Weapon.DamageType;
                Projectile.velocity = Vector2.Zero;
            }
            // Aller-retour de l'estoc
            float progress = 1f - Projectile.timeLeft / (float)Lifetime;
            float distance = Reach * (float)Math.Sin(progress * MathHelper.Pi);
            Vector2 direction = Projectile.localAI[1].ToRotationVector2();
            Projectile.Center = owner.MountedCenter + direction * (16f + distance);
            Projectile.rotation = Projectile.localAI[1] + MathHelper.PiOver4;
            if (Main.rand.NextBool(3))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.OrangeTorch, 0f, 0f, 120, default, 0.9f).noGravity = true;
        }

        private ShortstrikeData.Attack Attack => Weapon != null ? ShortstrikeData.For(Weapon) : ShortstrikeData.Generic;

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            foreach (var effect in Attack.Effects)
            {
                // « Hits drain 3% health » : 3 % de la vie restante, au plus 5 fois les dégâts du coup ([choix])
                if (effect.Type == "drainPercent")
                    modifiers.FlatBonusDamage += Math.Min(target.life * effect.Value, Projectile.damage * 5f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player owner = Main.player[Projectile.owner];
            foreach (var effect in Attack.Effects)
            {
                switch (effect.Type)
                {
                    case "inflict" when effect.BuffType > 0:
                        // Sans durée écrite (« Attacks inflict Hellfire ») : 5 secondes ([choix])
                        target.AddBuff(effect.BuffType, (int)((effect.Seconds ?? 5f) * 60));
                        break;
                    case "healPerHit" when Projectile.owner == Main.myPlayer:
                        owner.Heal((int)effect.Value);
                        break;
                    case "spawnOnHit" when effect.ProjectileType > 0 && Projectile.owner == Main.myPlayer:
                        // Projectile de soutien : la moitié des dégâts du coup ([choix])
                        var spawned = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center,
                            Main.rand.NextVector2CircularEdge(8f, 8f), effect.ProjectileType, Projectile.damage / 2, 2f, Projectile.owner);
                        spawned.DamageType = Projectile.DamageType;
                        break;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Weapon == null)
                return false;
            Main.instance.LoadItem(Weapon.type);
            Texture2D texture = TextureAssets.Item[Weapon.type].Value;
            Rectangle frame = Main.itemAnimations[Weapon.type]?.GetFrame(texture) ?? texture.Frame();
            float fade = (float)Math.Sin((1f - Projectile.timeLeft / (float)Lifetime) * MathHelper.Pi);
            // Clone magique : teinte orange translucide, comme l'aura de la jauge pleine
            Color color = Color.Lerp(Color.White, new Color(255, 160, 60), 0.5f) * (0.75f * fade + 0.25f);
            color.A = 120;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, color, Projectile.rotation,
                frame.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }

    /// <summary>Affiche la durée d'un Shortflurry en cours (ses effets sont appliqués par ShortstrikePlayer).</summary>
    public class ShortflurryBuff : ModBuff
    {
        public override string Texture => $"Terraria/Images/Buff_{BuffID.Sharpened}";

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = false;
        }
    }
}
