using System;
using System.Linq;
using CalamityFandom.Common.Shortstrike;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
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
                FireOnStab(owner);
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

        /// <summary>
        /// Projectiles tirés à chaque estoc du clone, dans sa direction, comme l'arme le ferait (Value = chance) : rayon du
        /// Cosmic Shiv et son « second beam » à 20 %, croissant du Galileo Gladius, éclat des Sahara Slicers...
        /// </summary>
        private void FireOnStab(Player owner)
        {
            if (Projectile.owner != Main.myPlayer)
                return;
            Vector2 direction = Projectile.localAI[1].ToRotationVector2();
            foreach (var effect in Attack.Effects)
            {
                if (effect.Type != "spawnOnStab" || effect.ProjectileType <= 0 || Main.rand.NextFloat() >= effect.Value)
                    continue;
                for (int i = 0; i < effect.Count; i++)
                {
                    // Dispersion (Spread, en radians de part et d'autre) : un second tir ne se superpose pas au premier
                    Vector2 velocity = direction.RotatedByRandom(effect.Spread) * effect.Speed;
                    var shot = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), owner.MountedCenter + direction * 24f, velocity,
                        effect.ProjectileType, (int)(Projectile.damage * effect.DamageMult), Projectile.knockBack, Projectile.owner, effect.Ai0);
                    shot.DamageType = Projectile.DamageType;
                    if (effect.RainType > 0)
                        shot.GetGlobalProjectile<ShortstrikeProjectile>().RainType = effect.RainType;
                }
            }
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
                    case "cloud" when Projectile.owner == Main.myPlayer:
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<ShortflurryCloud>(),
                            Projectile.damage / 4, 0f, Projectile.owner, effect.BuffType, (effect.Seconds ?? 3f) * 60);
                        break;
                    case "vine" when Projectile.owner == Main.myPlayer:
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Bottom, Vector2.Zero, ModContent.ProjectileType<ShortflurryVine>(),
                            Projectile.damage / 4, 0f, Projectile.owner, effect.Value);
                        break;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Weapon == null)
                return false;
            // Image : celle de l'attaque si l'arme en a une autre que son icône (« cloneTexture » : Sahara Slicers), sinon l'objet
            Texture2D texture;
            Rectangle frame;
            var attackTextures = Attack.Effects.Where(e => e.Type == "cloneTexture" && e.ProjectileType > 0).ToList();
            if (attackTextures.Count > 0)
            {
                texture = TextureAssets.Projectile[attackTextures[Projectile.identity % attackTextures.Count].ProjectileType].Value;
                frame = texture.Frame();
            }
            else
            {
                Main.instance.LoadItem(Weapon.type);
                texture = TextureAssets.Item[Weapon.type].Value;
                frame = Main.itemAnimations[Weapon.type]?.GetFrame(texture) ?? texture.Frame();
            }
            float fade = (float)Math.Sin((1f - Projectile.timeLeft / (float)Lifetime) * MathHelper.Pi);
            // Clone magique : teinte orange translucide, comme l'aura de la jauge pleine
            Color color = Color.Lerp(Color.White, new Color(255, 160, 60), 0.5f) * (0.75f * fade + 0.25f);
            color.A = 120;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, color, Projectile.rotation,
                frame.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }

    /// <summary>
    /// Grappin d'un Shortflurry (Calamity++, Galileo Gladius : son lancer et sa traction, gratuits, sans Stratus Starburst ;
    /// retours de test du 28/10). Un clone de l'arme (ai[0] = son type) file vers le curseur et se plante dans le premier
    /// ennemi touché (éclat d'étoiles, ai[1] = type, ai[2] = nombre) ; il tire alors le joueur jusqu'à lui, frappe une
    /// dernière fois à dégâts x5 avec un second éclat, et le joueur rebondit en arrière. Sans ennemi, il revient seul.
    /// </summary>
    public class ShortflurryThrown : ModProjectile
    {
        private const float MaxDistance = 700f;
        private const int MaxPullTicks = 40;
        private const float PullSpeed = 18f;
        private const int FinalDamageMultiplier = 5;

        private enum Phase { Flying, Pulling, Returning }

        private Phase phase = Phase.Flying;
        private int stuckNPC = -1;
        private Vector2 stuckOffset;
        private int pullTicks;

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private Item Weapon => ContentSamples.ItemsByType.TryGetValue((int)Projectile.ai[0], out var item) ? item : null;
        private Player Owner => Main.player[Projectile.owner];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 36;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 600;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = DamageClass.Melee;
        }

        // Une fois planté, il ne frappe plus qu'au coup final
        public override bool? CanDamage() => phase == Phase.Flying ? null : false;

        public override void AI()
        {
            if (Main.rand.NextBool(2))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.OrangeTorch, 0f, 0f, 120, default, 1.1f).noGravity = true;
            switch (phase)
            {
                case Phase.Flying:
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
                    if (Projectile.Distance(Owner.Center) > MaxDistance)
                        phase = Phase.Returning;
                    break;
                case Phase.Pulling:
                    Pull();
                    break;
                case Phase.Returning:
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Owner.Center) * 16f, 0.1f);
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 + MathHelper.Pi;
                    if (Projectile.Distance(Owner.Center) < 24f)
                        Projectile.Kill();
                    break;
            }
        }

        private void Pull()
        {
            NPC target = stuckNPC >= 0 ? Main.npc[stuckNPC] : null;
            if (target == null || !target.active || Owner.dead)
            {
                phase = Phase.Returning;
                return;
            }
            Projectile.Center = target.Center + stuckOffset;
            Projectile.velocity = Vector2.Zero;
            if (Projectile.owner != Main.myPlayer)
                return;
            // Le joueur est tiré vers l'arme plantée, invulnérable le temps du trajet
            Owner.mount.Dismount(Owner);
            Owner.RemoveAllGrapplingHooks();
            Owner.SetImmuneTimeForAllTypes(6);
            Owner.velocity = Owner.DirectionTo(Projectile.Center) * PullSpeed;
            Owner.fallStart = (int)(Owner.position.Y / 16f);
            bool blocked = Collision.SolidCollision(Owner.position + Owner.velocity, Owner.width, Owner.height);
            if (Projectile.Distance(Owner.Center) < 56f || ++pullTicks >= MaxPullTicks || blocked)
                FinalStrike(target);
        }

        private void FinalStrike(NPC target)
        {
            // Coup final : dégâts x5 sur l'ennemi planté, éclat d'étoiles, rebond du joueur en arrière
            SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, Owner.Center);
            SoundEngine.PlaySound(SoundID.DD2_SonicBoomBladeSlash, Owner.Center);
            Burst();
            Owner.ApplyDamageToNPC(target, Projectile.damage * FinalDamageMultiplier, Projectile.knockBack, Owner.direction, false, Projectile.DamageType);
            Owner.velocity = target.DirectionTo(Owner.Center).SafeNormalize(-Vector2.UnitX * Owner.direction) * 10f;
            Owner.SetImmuneTimeForAllTypes(Owner.longInvince ? 40 : 20);
            Projectile.Kill();
        }

        /// <summary>Éclat d'étoiles comme celui du lancer de Calamity : vers l'arrière, dispersées, puis à tête chercheuse.</summary>
        private void Burst()
        {
            int starType = (int)Projectile.ai[1];
            if (Projectile.owner != Main.myPlayer || starType <= 0)
                return;
            Vector2 back = -(Projectile.rotation - MathHelper.PiOver4).ToRotationVector2();
            for (int i = 0; i < (int)Projectile.ai[2]; i++)
            {
                var star = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, back.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.75f, 1.25f),
                    starType, Projectile.damage / 2, Projectile.knockBack, Projectile.owner, 0f, Main.rand.Next(5, 15));
                star.DamageType = Projectile.DamageType;
                star.usesIDStaticNPCImmunity = false;
                star.usesLocalNPCImmunity = true;
                star.localNPCHitCooldown = 20;
                star.timeLeft = star.MaxUpdates * 600;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (phase != Phase.Flying)
                return;
            // Planté dans l'ennemi : premier éclat, puis la traction
            SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, Projectile.Center);
            Burst();
            phase = Phase.Pulling;
            stuckNPC = target.whoAmI;
            stuckOffset = Projectile.Center - target.Center;
            Projectile.netUpdate = true;
        }

        public override void SendExtraAI(System.IO.BinaryWriter writer)
        {
            writer.Write((byte)phase);
            writer.Write((short)stuckNPC);
            writer.WriteVector2(stuckOffset);
        }

        public override void ReceiveExtraAI(System.IO.BinaryReader reader)
        {
            phase = (Phase)reader.ReadByte();
            stuckNPC = reader.ReadInt16();
            stuckOffset = reader.ReadVector2();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Weapon == null)
                return false;
            Main.instance.LoadItem(Weapon.type);
            Texture2D texture = TextureAssets.Item[Weapon.type].Value;
            Rectangle frame = Main.itemAnimations[Weapon.type]?.GetFrame(texture) ?? texture.Frame();
            // Fil lumineux entre le joueur et l'arme plantée pendant la traction
            if (phase == Phase.Pulling)
            {
                Vector2 start = Owner.MountedCenter, end = Projectile.Center;
                for (float t = 0f; t < 1f; t += 12f / Math.Max(12f, Vector2.Distance(start, end)))
                    Main.EntitySpriteDraw(TextureAssets.MagicPixel.Value, Vector2.Lerp(start, end, t) - Main.screenPosition, new Rectangle(0, 0, 1, 1),
                        new Color(255, 170, 80, 0) * 0.8f, 0f, new Vector2(0.5f), 3f, SpriteEffects.None, 0);
            }
            Color color = Color.Lerp(Color.White, new Color(255, 160, 60), 0.5f) * 0.85f;
            color.A = 120;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, color, Projectile.rotation,
                frame.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }

    /// <summary>
    /// Nuage immobile d'un Shortflurry (Submarine Shocker : « stationary clouds » qui infligent Sulphuric Poisoning) :
    /// ai[0] = debuff, ai[1] = durée en ticks ; touche toutes les demi-secondes ([choix]). Dessiné avec les nuages de la
    /// Toxic Flask vanilla (retour de test du 28/10 : les étincelles ne faisaient pas nuage).
    /// </summary>
    public class ShortflurryCloud : ModProjectile
    {
        private const int Puffs = 4;
        private const int FadeTicks = 20;
        private static readonly int[] CloudTypes = { ProjectileID.ToxicCloud, ProjectileID.ToxicCloud2, ProjectileID.ToxicCloud3 };

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                if (Projectile.ai[1] > 0f)
                    Projectile.timeLeft = (int)Projectile.ai[1];
                Projectile.localAI[1] = Projectile.timeLeft;
            }
            Projectile.rotation += 0.01f;
            Lighting.AddLight(Projectile.Center, 0.1f * Projectile.Opacity, 0.4f * Projectile.Opacity, 0.05f);
            if (Main.rand.NextBool(6))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GreenTorch, 0f, -0.5f, 150, default, 0.9f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[0] > 0f)
                target.AddBuff((int)Projectile.ai[0], 180);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Apparition et disparition en fondu ; quelques volutes qui tournent lentement autour du centre
            float age = Projectile.localAI[1] - Projectile.timeLeft;
            float opacity = MathHelper.Clamp(age / FadeTicks, 0f, 1f) * MathHelper.Clamp(Projectile.timeLeft / (float)FadeTicks, 0f, 1f);
            var random = new Terraria.Utilities.UnifiedRandom(Projectile.identity);
            for (int i = 0; i < Puffs; i++)
            {
                int type = CloudTypes[random.Next(CloudTypes.Length)];
                Main.instance.LoadProjectile(type);
                Texture2D texture = TextureAssets.Projectile[type].Value;
                Rectangle frame = texture.Frame(1, Main.projFrames[type] > 0 ? Main.projFrames[type] : 1);
                float angle = MathHelper.TwoPi * i / Puffs + Projectile.rotation * (i % 2 == 0 ? 1f : -1f);
                Vector2 offset = angle.ToRotationVector2() * random.NextFloat(6f, 16f);
                float scale = Projectile.width / (float)frame.Width * random.NextFloat(0.8f, 1.1f);
                Color color = Lighting.GetColor((Projectile.Center + offset).ToTileCoordinates(), new Color(150, 255, 120)) * (0.3f * opacity);
                Main.EntitySpriteDraw(texture, Projectile.Center + offset - Main.screenPosition, frame, color, Projectile.rotation + angle,
                    frame.Size() / 2f, scale, SpriteEffects.None, 0);
            }
            return false;
        }
    }

    /// <summary>
    /// Liane d'un Shortflurry (Spore Knife : « 4 tile long vines ») : épine de jungle de ai[0] blocs qui sort du sol sous
    /// l'ennemi (images ThornBase/ThornTip de Calamity), 2 s, Poisoned. Verticale ([choix] du 28/10 : posée à plat sous
    /// l'ennemi, elle ne se voyait pas).
    /// </summary>
    public class ShortflurryVine : ModProjectile
    {
        private const int GrowTicks = 8;
        private const int FadeTicks = 15;
        private const int SegmentHeight = 32;

        public override string Texture => "CalamityMod/Projectiles/Melee/ThornBase";

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                // Position de départ = pied de l'ennemi : la liane monte de là
                Vector2 bottom = Projectile.Center;
                Projectile.height = (int)(16 * System.Math.Max(1f, Projectile.ai[0]));
                Projectile.Bottom = bottom;
                for (int i = 0; i < 10; i++)
                    Dust.NewDustDirect(Projectile.BottomLeft - new Vector2(0f, 8f), Projectile.width, 8, DustID.JunglePlants, 0f, -2f).noGravity = true;
            }
            Projectile.localAI[0]++;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Poisoned, 120);

        public override bool PreDraw(ref Color lightColor)
        {
            float grown = MathHelper.Clamp(Projectile.localAI[0] / GrowTicks, 0f, 1f);
            float opacity = MathHelper.Clamp(Projectile.timeLeft / (float)FadeTicks, 0f, 1f);
            int visible = (int)(Projectile.height * grown);
            if (visible <= 0)
                return false;
            Texture2D stem = TextureAssets.Projectile[Type].Value;
            Texture2D tip = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/ThornTip").Value;
            // Segments empilés depuis le sol, la pointe en haut
            float y = Projectile.Bottom.Y;
            float top = Projectile.Bottom.Y - visible;
            bool first = true;
            while (y > top)
            {
                float segment = System.Math.Min(SegmentHeight, y - top);
                Texture2D texture = y - segment <= top ? tip : stem;
                var source = new Rectangle(0, 0, texture.Width, (int)segment);
                Vector2 position = new Vector2(Projectile.Center.X, y) - Main.screenPosition;
                Color color = Lighting.GetColor(new Vector2(Projectile.Center.X, y - segment / 2f).ToTileCoordinates()) * opacity;
                Main.EntitySpriteDraw(texture, position, source, color, 0f, new Vector2(texture.Width / 2f, segment), 1f,
                    first ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
                y -= segment;
                first = !first;
            }
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
