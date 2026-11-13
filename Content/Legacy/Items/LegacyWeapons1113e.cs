// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using System;
using System.Linq;
using CalamityFandom.Content.Legacy.Buffs.StatBuffs;
using CalamityFandom.Content.Legacy.Buffs.StatDebuffs;
using CalamityFandom.Content.Legacy.Projectiles.Ranged;
using CalamityFandom.Content.Legacy.Projectiles.Typeless;
using CalamityMod;
using CalamityMod.CalPlayer;
using CalamityMod.Events;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.Polterghast;
using CalamityMod.NPCs.Providence;
using CalamityMod.Rarities;
using CalamityMod.Sounds;
using CalamityMod.UI;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

// Lot du 13/11 (suite) : Dodu's Handcannon, Yanmei's Knife. Code de 2.0.1.005, valeurs des infobox et du texte des pages.
// Les effets Kami (buff, debuff, délai, listes de boss) ont disparu de Calamity : réécrits ici d'après la page.

namespace CalamityFandom.Content.Legacy.Items.Weapons.Ranged
{
    /// <summary>
    /// Wiki : Dodu_s Handcannon.txt — tire des coques de cacahuète explosives très rapides, quelle que soit la balle
    /// (explosion à la moitié des dégâts).
    /// </summary>
    public class DodusHandcannon : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 34;
            Item.damage = 857;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 30;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6f;
            Item.UseSound = CommonCalamitySounds.LargeWeaponFireSound with { Volume = 0.3f };
            Item.shoot = ModContent.ProjectileType<HighExplosivePeanutShell>();
            Item.shootSpeed = 24f;
            Item.useAmmo = AmmoID.Bullet;
            Item.value = Item.sellPrice(gold: 26);
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.Calamity().donorItem = true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) =>
            type = Item.shoot;

        public override Vector2? HoldoutOffset() => new Vector2(-17, 5);
    }
}

namespace CalamityFandom.Content.Legacy.Items.Weapons.Typeless
{
    /// <summary>
    /// Wiki : Yanmei_s Knife.txt — entaille verte devant le joueur, puis 3 s sans pouvoir s'en servir ; toucher un boss, un
    /// mini-boss ou leurs serviteurs donne Kami Injection au joueur (+15 % de vitesse et de dégâts, 10 s) et Kami Flu à
    /// la cible (réduction des dégâts -20 %, 125 PV/s, 10 s).
    /// </summary>
    public class YanmeisKnife : ModItem
    {
        public const int UseDelay = 180;

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 44;
            Item.damage = 8;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useTime = Item.useAnimation = 32;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4.5f;
            Item.value = Item.sellPrice(gold: 12);
            Item.rare = ItemRarityID.Yellow;
            Item.Calamity().donorItem = true;
            Item.UseSound = SoundID.Item71;
            Item.shoot = ModContent.ProjectileType<YanmeisKnifeSlash>();
            Item.shootSpeed = 24f;
        }

        // 10 % de critique (4 de base + 6, vieux code)
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 6;

        public override bool CanUseItem(Player player) => player.GetModPlayer<KamiPlayer>().KnifeDelay <= 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.GetModPlayer<KamiPlayer>().KnifeDelay = UseDelay;
            return true;
        }
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.Ranged
{
    /// <summary>Coque de cacahuète de Dodu's Handcannon : explose au contact (140 px, moitié des dégâts).</summary>
    public class HighExplosivePeanutShell : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 9;
            ProjectileID.Sets.TrailingMode[Type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.extraUpdates = 4;
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.spriteDirection = 1;
            Lighting.AddLight(Projectile.Center, 0.75f, 0.65f, 0.08f);
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.WoodFurniture);
            d.noGravity = true;
            d.scale = Main.rand.NextFloat(1f, 1.4f);
            d.velocity = d.velocity * 0.2f + Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.17f, 0.17f)) * 0.6f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item62, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Item88, Projectile.Center);
            Projectile.ExpandHitboxBy(140);
            Projectile.maxPenetrate = Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.LargeFieryExplosion();
            Projectile.damage /= 2;
            Projectile.Damage();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            return true;
        }
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.Typeless
{
    /// <summary>Entaille de Yanmei's Knife : tenue devant le bras le temps de son animation (5 images de 7 ticks).</summary>
    public class YanmeisKnifeSlash : ModProjectile
    {
        /// <summary>
        /// Cibles des effets Kami : boss (pas les segments de vers), mini-boss de Calamity et serviteurs d'un boss vivant.
        /// [choix] « Serviteur » = tout ennemi hostile pendant qu'un boss est là (Calamity n'a plus sa liste).
        /// </summary>
        public static bool CanReceiveKamiEffects(NPC npc)
        {
            if (npc.friendly || npc.townNPC || npc.lifeMax <= 5 || npc.type == NPCID.TargetDummy)
                return false;
            if (npc.boss)
                return npc.realLife < 0 || npc.realLife == npc.whoAmI;
            if (BossHealthBarManager.MinibossHPBarList?.Contains(npc.type) == true || AcidRainEvent.AllMinibosses.Contains(npc.type))
                return true;
            return Main.npc.Any(n => n.active && n.boss);
        }

        public override void SetStaticDefaults() => Main.projFrames[Type] = 5;

        public override void SetDefaults()
        {
            Projectile.width = 180;
            Projectile.height = 96;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (++Projectile.frameCounter % 7 == 0 && ++Projectile.frame >= Main.projFrames[Type])
            {
                Projectile.Kill();
                return;
            }
            Lighting.AddLight(Projectile.Center + Projectile.velocity * 3f, 0f, 1.5f, 0.1f);
            Vector2 armPoint = player.RotatedRelativePoint(player.MountedCenter, true);
            float angle = Projectile.velocity.ToRotation();
            Projectile.direction = Projectile.spriteDirection = (Math.Cos(angle) > 0).ToDirectionInt();
            Projectile.rotation = angle + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f);
            Projectile.position = armPoint - Projectile.Size * 0.5f + angle.ToRotationVector2() * 80f;
            player.ChangeDir(Projectile.direction);
            Projectile.timeLeft = 2;
            player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
            player.heldProj = Projectile.whoAmI;
            player.itemTime = player.itemAnimation = 2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!CanReceiveKamiEffects(target))
                return;
            Player player = Main.player[Projectile.owner];
            target.AddBuff(ModContent.BuffType<KamiFlu>(), 600);
            player.AddBuff(ModContent.BuffType<KamiBuff>(), 600);
            if (Projectile.ai[0] == 0f)
            {
                // Le son de l'Adrenaline Mode (page, Notes)
                SoundEngine.PlaySound(CalamityPlayer.AdrenalineActivationSound, Projectile.Center);
                Projectile.ai[0] = 1f;
            }
            if (Main.dedServ)
                return;
            for (int i = 0; i < 60; i++)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, 267);
                dust.position.X += Main.rand.NextFloat(-16f, 16f);
                dust.color = Main.hslToRgb(Main.rand.NextFloat(0.26f, 0.37f), 1f, 0.75f);
                dust.velocity = Main.rand.NextVector2Circular(24f, 24f);
                dust.scale = Main.rand.NextFloat(1.4f, 1.8f);
                dust.noGravity = true;
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(0, 215, 0, 0);
    }
}

namespace CalamityFandom.Content.Legacy.Buffs.StatBuffs
{
    /// <summary>Kami Injection : +15 % de vitesse de déplacement et de dégâts ; traînée verte.</summary>
    public class KamiBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.moveSpeed += 0.15f;
            player.maxRunSpeed *= 1.15f;
            player.GetDamage<GenericDamageClass>() += 0.15f;
            if (Main.rand.NextBool(3) && player.velocity.Length() > 2f && !Main.dedServ)
            {
                Dust d = Dust.NewDustDirect(player.position, player.width, player.height, 267, 0f, 0f, 100, new Color(80, 255, 80));
                d.noGravity = true;
                d.velocity = -player.velocity * 0.2f;
            }
        }
    }

    /// <summary>Délai de 3 s de Yanmei's Knife.</summary>
    public class KamiPlayer : ModPlayer
    {
        public int KnifeDelay;

        public override void PostUpdate()
        {
            if (KnifeDelay > 0)
                KnifeDelay--;
        }
    }
}

namespace CalamityFandom.Content.Legacy.Buffs.StatDebuffs
{
    /// <summary>Kami Flu : réduction des dégâts x0,8 (sauf Dragonfolly, Polterghast, Providence), 125 PV/s, lueur verte.</summary>
    public class KamiFlu : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex) => npc.GetGlobalNPC<KamiFluNPC>().Infected = true;
    }

    public class KamiFluNPC : GlobalNPC
    {
        public bool Infected;

        public override bool InstancePerEntity => true;

        public override void ResetEffects(NPC npc) => Infected = false;

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (!Infected)
                return;
            if (npc.lifeRegen > 0)
                npc.lifeRegen = 0;
            npc.lifeRegen -= 250;
            if (damage < 25)
                damage = 25;
        }

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            float dr = npc.Calamity().DR;
            if (!Infected || dr <= 0f || dr >= 1f || npc.type == ModContent.NPCType<Dragonfolly>()
                || npc.type == ModContent.NPCType<Polterghast>() || npc.type == ModContent.NPCType<Providence>())
                return;
            // Calamity multiplie les dégâts par (1 - réduction) : on compense pour une réduction x0,8
            modifiers.FinalDamage *= (1f - dr * 0.8f) / (1f - dr);
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (!Infected)
                return;
            drawColor = Color.Lerp(drawColor, new Color(80, 255, 80), 0.4f);
            Lighting.AddLight(npc.Center, 0.1f, 0.6f, 0.1f);
        }
    }
}
