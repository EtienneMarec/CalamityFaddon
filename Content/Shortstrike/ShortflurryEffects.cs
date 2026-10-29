using System;
using System.IO;
using CalamityFandom.Common.Shortstrike;
using CalamityMod;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityFandom.Content.Shortstrike
{
    /// <summary>
    /// Effets « au coup » et « à l'estoc » d'un Shortflurry (données de la page Shortstrike), communs aux deux sortes de
    /// clones : le vrai projectile d'attaque de l'arme (ShortflurryRealClone) ou le clone dessiné (ShortflurryClone).
    /// </summary>
    public static class ShortflurryEffects
    {
        /// <summary>
        /// Projectiles tirés à chaque estoc d'un clone, dans sa direction (Value = chance) : « second beam » du Cosmic Shiv
        /// à 20 %, croissant du Galileo Gladius (Full Fandom)...
        /// </summary>
        public static void FireOnStab(Projectile clone, ShortstrikeData.Attack attack, Vector2 direction)
        {
            if (clone.owner != Main.myPlayer)
                return;
            Player owner = Main.player[clone.owner];
            int stab = ++owner.GetModPlayer<ShortstrikePlayer>().FlurryStabs;
            foreach (var effect in attack.Effects)
            {
                if (effect.Type != "spawnOnStab" || effect.ProjectileType <= 0 || Main.rand.NextFloat() >= effect.Value)
                    continue;
                if (effect.Every > 1 && stab % effect.Every != 0)
                    continue;
                for (int i = 0; i < effect.Count; i++)
                {
                    // Dispersion (Spread, en radians de part et d'autre) : un second tir ne se superpose pas au premier
                    Vector2 velocity = direction.RotatedByRandom(effect.Spread) * effect.Speed;
                    var shot = Projectile.NewProjectileDirect(clone.GetSource_FromThis(), owner.MountedCenter + direction * 24f, velocity,
                        effect.ProjectileType, (int)(clone.damage * effect.DamageMult), clone.knockBack, clone.owner, effect.Ai0);
                    shot.DamageType = clone.DamageType;
                    if (effect.RainType > 0)
                        shot.GetGlobalProjectile<ShortstrikeProjectile>().RainType = effect.RainType;
                }
            }
        }

        /// <summary>Type de l'arme d'un clone (clone dessiné : ai[0] ; clone réel : ShortflurryRealClone).</summary>
        private static int WeaponOf(Projectile clone) =>
            clone.ModProjectile is ShortflurryClone ? (int)clone.ai[0] : clone.GetGlobalProjectile<ShortflurryRealClone>().WeaponType;

        public static void ModifyHit(Projectile clone, ShortstrikeData.Attack attack, NPC target, ref NPC.HitModifiers modifiers)
        {
            foreach (var effect in attack.Effects)
            {
                // « Hits drain 3% health » : 3 % de la vie restante, au plus 5 fois les dégâts du coup ([choix])
                if (effect.Type == "drainPercent")
                    modifiers.FlatBonusDamage += Math.Min(target.life * effect.Value, clone.damage * 5f);
            }
        }

        public static void OnHit(Projectile clone, ShortstrikeData.Attack attack, NPC target)
        {
            Player owner = Main.player[clone.owner];
            bool local = clone.owner == Main.myPlayer;
            foreach (var effect in attack.Effects)
            {
                switch (effect.Type)
                {
                    case "inflict" when effect.BuffType > 0:
                        // Sans durée écrite (« Attacks inflict Hellfire ») : 5 secondes ([choix])
                        target.AddBuff(effect.BuffType, (int)((effect.Seconds ?? 5f) * 60));
                        break;
                    case "healPerHit" when local:
                        owner.Heal((int)effect.Value);
                        break;
                    case "spawnOnHit" when effect.ProjectileType > 0 && local:
                        // Projectile de soutien : la moitié des dégâts du coup ([choix])
                        var spawned = Projectile.NewProjectileDirect(clone.GetSource_FromThis(), target.Center,
                            Main.rand.NextVector2CircularEdge(8f, 8f), effect.ProjectileType, clone.damage / 2, 2f, clone.owner);
                        spawned.DamageType = clone.DamageType;
                        break;
                    case "cloud" when local:
                        Projectile.NewProjectile(clone.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<ShortflurryCloud>(),
                            clone.damage / 4, 0f, clone.owner, effect.BuffType, (effect.Seconds ?? 3f) * 60);
                        break;
                    case "flurryBeam" when local:
                        // Rayon qui part de l'ennemi touché, dans le prolongement du coup (ShortflurryBeam)
                        Vector2 forward = (target.Center - owner.Center).SafeNormalize(Vector2.UnitX * owner.direction) * effect.Speed;
                        Projectile.NewProjectile(clone.GetSource_FromThis(), target.Center, forward, ModContent.ProjectileType<ShortflurryBeam>(),
                            clone.damage, clone.knockBack, clone.owner, WeaponOf(clone));
                        break;
                    case "verticalBeams" when effect.ProjectileType > 0 && local:
                        // « vertical beams of holy light » : un rayon vers le haut et un vers le bas, depuis l'ennemi
                        foreach (float sign in new[] { -1f, 1f })
                        {
                            var pillar = Projectile.NewProjectileDirect(clone.GetSource_FromThis(), target.Center, new Vector2(0f, 12f * sign),
                                effect.ProjectileType, (int)(clone.damage * effect.DamageMult), 2f, clone.owner);
                            pillar.DamageType = clone.DamageType;
                        }
                        break;
                    case "vine" when local:
                        // 1 à 3 épines par coup, chacune dans une direction au hasard ([choix] du 28/10)
                        for (int i = Main.rand.Next(1, 4); i > 0; i--)
                            Projectile.NewProjectile(clone.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<ShortflurryVine>(),
                                clone.damage / 4, 0f, clone.owner, effect.Value, Main.rand.NextFloat(MathHelper.TwoPi));
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Clone « réel » d'un Shortflurry (retour de test du 28/10 : un clone doit être l'attaque elle-même) : le projectile
    /// d'attaque de l'arme (estoc de Calamity ou de ce mod, épée tenue du Galileo Gladius), avec ses images, ses particules
    /// et ses effets au coup, mais animé ici : son propre AI s'occupe du joueur (arme tenue, direction, temps d'utilisation)
    /// et ne doit pas tourner pour un clone qui frappe à côté de l'attaque normale.
    /// </summary>
    public class ShortflurryRealClone : GlobalProjectile
    {
        /// <summary>Type de l'arme du Shortflurry ; 0 = pas un clone.</summary>
        public int WeaponType;
        private Vector2 direction;
        /// <summary>
        /// Écart entre le joueur et le clone, dans sa direction : la portée de l'attaque normale le laissait collé au joueur
        /// (retour de test du 28/10) ; le clone dessiné allait de 16 à 72 px.
        /// </summary>
        private const float CloneDistance = 40f;
        private int timer, previousHeldProj = -1;

        public override bool InstancePerEntity => true;

        public bool IsClone => WeaponType > 0;

        private ShortstrikeData.Attack Attack => ContentSamples.ItemsByType.TryGetValue(WeaponType, out var item) ? ShortstrikeData.For(item) : ShortstrikeData.Generic;

        /// <summary>Le projectile d'attaque de l'arme peut-il servir de clone ? (estoc ou épée tenue de Calamity)</summary>
        public static bool CanClone(int type) =>
            type > 0 && ContentSamples.ProjectilesByType.TryGetValue(type, out var sample)
            && sample.ModProjectile is BaseShortswordProjectile or BaseSwordHoldoutProjectile;

        /// <summary>Lance le clone : projectile <paramref name="type"/> vers <paramref name="aim"/>, dégâts du Shortflurry.</summary>
        public static void Spawn(Player player, Item weapon, int type, Vector2 aim, int damage)
        {
            var proj = Projectile.NewProjectileDirect(player.GetSource_ItemUse(weapon), player.MountedCenter, aim * Math.Max(1f, weapon.shootSpeed), type,
                damage, player.GetWeaponKnockback(weapon), player.whoAmI);
            var clone = proj.GetGlobalProjectile<ShortflurryRealClone>();
            clone.WeaponType = weapon.type;
            clone.direction = aim;
            clone.Setup(proj);
            proj.GetGlobalProjectile<ShortstrikeProjectile>().FromShortsword = false; // les clones ne remplissent pas la jauge
            proj.netUpdate = true;
            ShortflurryEffects.FireOnStab(proj, clone.Attack, aim);
        }

        private void Setup(Projectile proj)
        {
            // Dessiné avec les autres projectiles (un estoc est caché et dessiné « dans la main » du joueur), un coup par ennemi,
            // dégâts fixés (une épée tenue recopie sinon ceux de l'arme en main à chaque frame)
            proj.hide = false;
            proj.usesLocalNPCImmunity = true;
            proj.localNPCHitCooldown = -1;
            proj.ContinuouslyUpdateDamageStats = false;
            if (proj.ModProjectile is BaseSwordHoldoutProjectile holdout)
            {
                Player player = Main.player[proj.owner];
                holdout.angle = -direction; // Calamity : du curseur vers le joueur
                proj.spriteDirection = direction.X >= 0f ? 1 : -1;
                holdout.swingTime = Math.Max(1, (ContentSamples.ItemsByType[WeaponType].useTime) * proj.MaxUpdates);
                proj.scale *= player.GetMeleeScale();
                holdout.baseScale = proj.scale;
            }
        }

        public override bool PreAI(Projectile projectile)
        {
            if (!IsClone)
                return true;
            if (projectile.ModProjectile is BaseShortswordProjectile stab)
                AnimateStab(projectile, stab);
            else if (projectile.ModProjectile is BaseSwordHoldoutProjectile holdout)
                AnimateHoldout(projectile, holdout);
            return false;
        }

        /// <summary>Estoc (BaseShortswordProjectile.Behavior sans ce qui touche au joueur) : aller-retour, fondu, particules.</summary>
        private void AnimateStab(Projectile projectile, BaseShortswordProjectile stab)
        {
            Player owner = Main.player[projectile.owner];
            stab.Timer++;
            if (stab.Timer >= stab.TotalDuration)
            {
                projectile.Kill();
                return;
            }
            projectile.Opacity = Utils.GetLerpValue(0f, stab.FadeInDuration, stab.Timer, true) * Utils.GetLerpValue(stab.TotalDuration, stab.TotalDuration - stab.FadeOutDuration, stab.Timer, true);
            projectile.Center = owner.MountedCenter + direction * CloneDistance + stab.OffsetFromPlayer;
            // Effet « à l'estoc » de l'arme (rayon du Cosmic Shiv...), tiré depuis le clone déjà placé
            if (projectile.localAI[0] == 0f && stab.EffectBeforePullback != null && Main.myPlayer == projectile.owner)
            {
                projectile.localAI[0] = 1f;
                stab.EffectBeforePullback.Invoke(projectile);
            }
            projectile.scale = 1f + (float)Math.Sin(stab.FullUse * MathHelper.Pi) * 0.2f;
            projectile.spriteDirection = (Vector2.Dot(projectile.velocity, Vector2.UnitX) >= 0f).ToDirectionInt();
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2 - MathHelper.PiOver4 * projectile.spriteDirection;
            stab.SetVisualOffsets();
            stab.ExtraBehavior();
        }

        /// <summary>Épée tenue (BaseSwordHoldoutProjectile.AI sans ce qui touche au joueur) : estoc dans la direction du clone.</summary>
        private void AnimateHoldout(Projectile projectile, BaseSwordHoldoutProjectile holdout)
        {
            Player owner = Main.player[projectile.owner];
            projectile.gfxOffY = owner.gfxOffY;
            float adjust = MathHelper.ToRadians(projectile.spriteDirection == -1 ? -45 : 225);
            int side = direction.X >= 0f ? 1 : -1;
            Vector2 armCenter = owner.MountedCenter - new Vector2(5 * side, 2);
            float swing = holdout.SwingFunction();
            projectile.Center = armCenter + direction * CloneDistance
                - (holdout.angle * holdout.OffsetDistance * (1 + (projectile.scale - 1) * 0.75f)).RotatedBy(projectile.spriteDirection * swing);
            projectile.rotation = holdout.angle.RotatedBy(projectile.spriteDirection * swing).ToRotation() + adjust;
            // Ses effets visuels (lame lumineuse, étincelles) ; il se déclare arme tenue du joueur : on rend la main à l'attaque normale
            previousHeldProj = owner.heldProj;
            holdout.AdditionalAI();
            owner.heldProj = previousHeldProj;
            if (!projectile.active)
                return;
            if (holdout.timer > holdout.swingTime)
            {
                projectile.Kill();
                return;
            }
            holdout.timer++;
            if (holdout.swingTimer < holdout.swingTime)
                holdout.swingTimer++;
        }

        /// <summary>
        /// Zone de coup d'un estoc décalé : celle de BaseShortswordProjectile part du joueur, on la déplace avec le clone (une
        /// épée tenue mesure la sienne depuis son centre : rien à faire).
        /// </summary>
        public override bool? Colliding(Projectile projectile, Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (!IsClone || projectile.ModProjectile is not BaseShortswordProjectile stab)
                return null;
            float collisionPoint = 0f;
            Vector2 start = Main.player[projectile.owner].MountedCenter + direction * CloneDistance + stab.OffsetFromPlayer;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, start + projectile.velocity * 12f * projectile.scale, 24, ref collisionPoint);
        }

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (IsClone)
                ShortflurryEffects.ModifyHit(projectile, Attack, target, ref modifiers);
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (IsClone)
                ShortflurryEffects.OnHit(projectile, Attack, target);
        }

        public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            bitWriter.WriteBit(IsClone);
            if (!IsClone)
                return;
            binaryWriter.Write(WeaponType);
            binaryWriter.WriteVector2(direction);
        }

        public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader)
        {
            if (!bitReader.ReadBit())
                return;
            bool wasClone = IsClone;
            WeaponType = binaryReader.ReadInt32();
            direction = binaryReader.ReadVector2();
            if (!wasClone)
                Setup(projectile);
        }
    }
}
