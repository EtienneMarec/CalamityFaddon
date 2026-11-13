// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using System;
using System.IO;
using CalamityFandom.Content.Buffs;
using CalamityFandom.Content.Legacy.Projectiles.Melee;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

// Elemental Excalibur (13/11) : code de 2.0.1.005, valeurs de l'infobox et du texte de la page ; recette du wiki.

namespace CalamityFandom.Content.Legacy.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Elemental Excalibur.txt — chaque coup lance un rayon d'une couleur différente, dans un ordre fixe (13 couleurs,
    /// chacune avec son comportement) ; coups directs : soigne de 10 à 19 PV ; clic droit : coup de vraie mêlée à dégâts
    /// doublés ; Exo Freeze 1 s, Holy Flames 4 s, Frostburn 5 s, On Fire! 6 s. Rareté arc-en-ciel sans être un objet Expert.
    /// </summary>
    public class ElementalExcalibur : ModItem
    {
        public const int BeamColors = 13;
        private int beamType;

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 2000;
            Item.useTime = Item.useAnimation = 13;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 8f;
            Item.crit = 6; // 10 % (4 de base + 6)
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.width = Item.height = 112;
            Item.value = Item.sellPrice(platinum: 1);
            Item.shoot = ModContent.ProjectileType<ElementalExcaliburBeam>();
            Item.shootSpeed = 6f;
            Item.rare = ItemRarityID.Expert;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            Item.shoot = player.altFunctionUse == 2 ? ProjectileID.None : ModContent.ProjectileType<ElementalExcaliburBeam>();
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, beamType);
            beamType = (beamType + 1) % BeamColors;
            return false;
        }

        public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (player.altFunctionUse == 2)
                modifiers.SourceDamage *= 2f;
        }

        public override void ModifyHitPvp(Player player, Player target, ref Player.HurtModifiers modifiers)
        {
            if (player.altFunctionUse == 2)
                modifiers.SourceDamage *= 2f;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (!Main.rand.NextBool(4))
                return;
            Dust d = Dust.NewDustDirect(hitbox.TopLeft(), hitbox.Width, hitbox.Height, 267, 0f, 0f, ElementalExcaliburBeam.Alpha, ElementalExcaliburBeam.BeamColor(beamType), 1.2f);
            d.noGravity = true;
        }

        public static void ExoDebuffs(NPC target)
        {
            target.AddBuff(ModContent.BuffType<ExoFreeze>(), 60);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 240);
            target.AddBuff(BuffID.Frostburn, 300);
            target.AddBuff(BuffID.OnFire, 360);
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            ExoDebuffs(target);
            if (!target.canGhostHeal || player.moonLeech)
                return;
            int heal = Main.rand.Next(10, 20);
            player.statLife = Math.Min(player.statLife + heal, player.statLifeMax2);
            player.HealEffect(heal);
        }
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.Melee
{
    /// <summary>Rayon de l'Elemental Excalibur : ai[0] = couleur (0 rouge … 12 gris), comportement de la page.</summary>
    public class ElementalExcaliburBeam : ModProjectile
    {
        public const int Alpha = 50;

        private const int Red = 0, Orange = 1, Yellow = 2, Lime = 3, Green = 4, Turquoise = 5, Cyan = 6, LightBlue = 7, Blue = 8,
            Purple = 9, Fuchsia = 10, HotPink = 11, Gray = 12;

        private bool playedSound;

        public static Color BeamColor(int type) => type switch
        {
            Orange => new Color(255, 128, 0, Alpha),
            Yellow => new Color(255, 255, 0, Alpha),
            Lime => new Color(128, 255, 0, Alpha),
            Green => new Color(0, 255, 0, Alpha),
            Turquoise => new Color(0, 255, 128, Alpha),
            Cyan => new Color(0, 255, 255, Alpha),
            LightBlue => new Color(0, 128, 255, Alpha),
            Blue => new Color(0, 0, 255, Alpha),
            Purple => new Color(128, 0, 255, Alpha),
            Fuchsia => new Color(255, 0, 255, Alpha),
            HotPink => new Color(255, 0, 128, Alpha),
            Gray => new Color(168, 168, 168, Alpha),
            _ => new Color(255, 0, 0, Alpha),
        };

        private int Kind => (int)Projectile.ai[0];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
            Projectile.timeLeft = 1200;
            Projectile.extraUpdates = 3;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            if (!playedSound)
            {
                SoundEngine.PlaySound(SoundID.Item60, Projectile.Center);
                playedSound = true;
                // Gris : traverse les blocs et les ennemis (page)
                if (Kind == Gray)
                {
                    Projectile.tileCollide = false;
                    Projectile.penetrate = -1;
                }
            }
            // Pulsation
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.scale -= 0.01f;
                Projectile.alpha += 15;
                if (Projectile.alpha >= 250)
                {
                    Projectile.alpha = 255;
                    Projectile.localAI[0] = 1f;
                }
            }
            else
            {
                Projectile.scale += 0.01f;
                Projectile.alpha -= 15;
                if (Projectile.alpha <= 0)
                {
                    Projectile.alpha = 0;
                    Projectile.localAI[0] = 0f;
                }
            }
            Player owner = Main.player[Projectile.owner];
            switch (Kind)
            {
                case Orange:
                    // Revient vers le joueur
                    Player closest = Main.player[Player.FindClosest(Projectile.Center, 1, 1)];
                    Projectile.ai[1]++;
                    if (Projectile.ai[1] < 220f && Projectile.ai[1] > 60f)
                    {
                        float speed = Projectile.velocity.Length();
                        Vector2 toPlayer = Projectile.SafeDirectionTo(closest.Center) * speed;
                        Projectile.velocity = Vector2.Normalize((Projectile.velocity * 24f + toPlayer) / 25f) * speed;
                    }
                    if (Projectile.velocity.Length() < 24f)
                        Projectile.velocity *= 1.02f;
                    break;
                case Yellow:
                    // Se sépare en trois rayons rouges
                    if (++Projectile.localAI[1] >= 180f)
                    {
                        if (Projectile.owner == Main.myPlayer)
                            for (int i = 0; i < 3; i++)
                                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                                    Projectile.velocity.RotatedBy(MathHelper.ToRadians(-10 + 10 * i)) * 0.5f, Type, Projectile.damage / 2, Projectile.knockBack * 0.5f, Projectile.owner, Red);
                        Projectile.Kill();
                    }
                    break;
                case Lime:
                    // Cherche le joueur
                    if (owner.active && !owner.dead)
                    {
                        if (Projectile.Distance(owner.Center) > 80f)
                            Projectile.velocity = (Projectile.velocity * 74f + Projectile.SafeDirectionTo(owner.Center, Vector2.UnitY) * 7.5f) / 75f;
                    }
                    else if (Projectile.timeLeft > 30)
                        Projectile.timeLeft = 30;
                    break;
                case Green:
                    CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 300f, 6f, 20f);
                    break;
                case Turquoise:
                    Projectile.tileCollide = false;
                    if (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y) < 28f)
                        Projectile.velocity *= 1.035f;
                    break;
                case Purple:
                    Projectile.localAI[1]++;
                    if (Projectile.localAI[1] <= 40f)
                        Projectile.velocity *= 0.95f;
                    else if (Projectile.localAI[1] <= 79f)
                        Projectile.velocity *= 1.05f;
                    else
                        Projectile.localAI[1] = 0f;
                    break;
                case Fuchsia:
                    if (Projectile.localAI[1] == 0f)
                    {
                        Projectile.velocity *= 0.1f;
                        Projectile.localAI[1] = 1f;
                    }
                    Projectile.velocity *= 1.01f;
                    break;
                case HotPink:
                    if (Projectile.localAI[1] == 0f)
                        Projectile.velocity *= 0.33f;
                    if (++Projectile.localAI[1] >= 181f)
                    {
                        Projectile.localAI[1] = 1f;
                        if (Main.myPlayer == Projectile.owner)
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity, Projectile.velocity, Type,
                                Projectile.damage / 2, Projectile.knockBack * 0.5f, Projectile.owner, Fuchsia);
                    }
                    break;
            }
            if (Main.rand.NextBool(2))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 267, 0f, 0f, Alpha, BeamColor(Kind), 1.5f).noGravity = true;
        }

        private void OnAnyHit()
        {
            if (Kind == LightBlue)
                Projectile.velocity *= 0.25f;
            else if (Kind == Blue)
                Projectile.velocity *= -1f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Items.Weapons.Melee.ElementalExcalibur.ExoDebuffs(target);
            OnAnyHit();
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 240);
            target.AddBuff(BuffID.Frostburn, 300);
            target.AddBuff(BuffID.OnFire, 360);
            OnAnyHit();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Kind != Blue)
                return true;
            // Bleu : rebondit sans fin
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }

        public override Color? GetAlpha(Color lightColor) => BeamColor(Kind);

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft <= 1195)
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            Projectile.ExpandHitboxBy(64);
            Projectile.maxPenetrate = Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();
            if (Projectile.owner == Main.myPlayer)
            {
                if (Kind == Cyan)
                {
                    // Cyan : trois rayons turquoise tombent vers le point d'impact
                    for (int x = 0; x < 3; x++)
                    {
                        bool fromRight = x == 1 || (x == 2 && Main.rand.NextBool());
                        CalamityUtils.ProjectileBarrage(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.Center, fromRight, 500f, 500f, 0f, 500f, 5f,
                            Type, (int)(Projectile.damage * 0.2f), Projectile.knockBack * 0.2f, Projectile.owner, false, 0f).ai[0] = Turquoise;
                    }
                }
                else if (Kind == Gray && Main.rand.NextBool(100))
                {
                    // Gris : une chance de lâcher une pièce de platine (page)
                    Item.NewItem(Projectile.GetSource_FromThis(), Projectile.Hitbox, ItemID.PlatinumCoin);
                }
            }
            if (Main.dedServ)
                return;
            Color color = BeamColor(Kind);
            for (int i = 0; i < 3; i++)
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 267, 0f, 0f, Alpha, color, 1.5f).noGravity = true;
            for (int i = 0; i < 30; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 267, 0f, 0f, Alpha, color, 2.5f);
                d.noGravity = true;
                d.velocity *= 3f;
                d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 267, 0f, 0f, Alpha, color, 1.5f);
                d.velocity *= 2f;
                d.noGravity = true;
            }
        }
    }
}
