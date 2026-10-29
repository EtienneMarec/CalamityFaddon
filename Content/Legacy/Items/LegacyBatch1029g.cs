// Adapté de Calamity Mod (objets retirés en 2022), © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using System;
using CalamityFandom.Content.Legacy.Projectiles.Ranged;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy
{
    /// <summary>
    /// Debuff ajouté à un projectile de Calamity ou vanilla réutilisé par un portage, quand le wiki lui en donne un que le
    /// projectile d'origine n'inflige pas (explosion FuckYou, étoile astrale tirée en dégâts à distance...).
    /// </summary>
    public class LegacyHitDebuff : GlobalProjectile
    {
        public int Buff;
        public int Time;

        public override bool InstancePerEntity => true;

        public static void Give(Projectile projectile, int buff, int time)
        {
            var data = projectile.GetGlobalProjectile<LegacyHitDebuff>();
            data.Buff = buff;
            data.Time = time;
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Buff > 0)
                target.AddBuff(Buff, Time);
        }
    }
}

namespace CalamityFandom.Content.Legacy.Items.Weapons.Ranged
{
    /// <summary>
    /// Wiki : Impaler.txt — pieux enflammés (traversent 9 ennemis, On Fire! 4 s), 1 chance sur 3 d'un pieu explosif
    /// (7 ennemis, explosion On Fire! 3 s) ; tue net les Vampires et Vampire Bats. Critique 18 %.
    /// </summary>
    public class Impaler : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 120;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 26;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7f;
            Item.value = Item.sellPrice(gold: 16);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<FlamingStake>();
            Item.shootSpeed = 10f;
            Item.useAmmo = AmmoID.Stake;
        }

        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 14;

        public override Vector2? HoldoutOffset() => new Vector2(0, -10);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 speed = velocity + new Vector2(Main.rand.Next(-2, 3), Main.rand.Next(-2, 3)) * 0.05f;
            int stake = Main.rand.NextBool(3) ? ModContent.ProjectileType<ExplodingStake>() : ModContent.ProjectileType<FlamingStake>();
            Projectile.NewProjectile(source, position, speed, stake, damage, knockback, player.whoAmI);
            return false;
        }
    }

    /// <summary>
    /// Wiki : Spectre Rifle.txt — les balles de mousquet deviennent des âmes rapides à tête chercheuse ; critique 26 % ;
    /// clic droit maintenu : zoom (comme la lunette de visée).
    /// </summary>
    public class SpectreRifle : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 150;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 88;
            Item.height = 30;
            Item.useTime = Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7f;
            Item.value = Item.sellPrice(gold: 16);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item40;
            Item.shoot = ProjectileID.LostSoulFriendly;
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 22;

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        // « Holding right click will use its "zoom" ability »
        public override void HoldItem(Player player) => player.scope = true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (type != ProjectileID.Bullet)
                return true;
            var soul = Projectile.NewProjectileDirect(source, position, velocity, ProjectileID.LostSoulFriendly, damage, knockback, player.whoAmI, 2f);
            soul.DamageType = DamageClass.Ranged;
            soul.extraUpdates += 2;
            return false;
        }
    }

    /// <summary>
    /// Wiki : Star Cannon EX.txt — consomme des Fallen Stars et tire une ou deux étoiles au hasard parmi trois : étoile
    /// astrale (rose, traverse les blocs, Astral Infection 2 s), étoile de Starfury (rouge) et étoile tombée (jaune, arrêtée
    /// par les blocs).
    /// </summary>
    public class StarCannonEX : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 95;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 74;
            Item.height = 24;
            Item.useTime = Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemRarityID.Lime;
            Item.noMelee = true;
            Item.knockBack = 8f;
            Item.value = Item.sellPrice(gold: 12);
            Item.UseSound = SoundID.Item9;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<FallenStarProj>();
            Item.shootSpeed = 15f;
            Item.useAmmo = AmmoID.FallenStar;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int count = Main.rand.Next(1, 3);
            for (int i = 0; i < count; i++)
            {
                Vector2 speed = velocity + new Vector2(Main.rand.Next(-15, 16), Main.rand.Next(-15, 16)) * 0.05f;
                int star = Main.rand.Next(new[] { ModContent.ProjectileType<AstralStar>(), ProjectileID.Starfury, ModContent.ProjectileType<FallenStarProj>() });
                var proj = Projectile.NewProjectileDirect(source, position, speed, star, damage, knockback, player.whoAmI);
                proj.DamageType = DamageClass.Ranged;
                if (star == ModContent.ProjectileType<AstralStar>())
                    LegacyHitDebuff.Give(proj, ModContent.BuffType<CalamityMod.Buffs.DamageOverTime.AstralInfectionDebuff>(), 120);
            }
            return false;
        }
    }

    /// <summary>
    /// Wiki : Brimstone Flameblaster.txt — lance-flammes à gel : boule de soufre qui rebondit jusqu'à 5 fois sur les blocs,
    /// Brimstone Flames 3 s ; lâché par Cataclysm (butin du wiki).
    /// </summary>
    public class BrimstoneFlameblaster : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 64;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 50;
            Item.height = 18;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.5f;
            Item.UseSound = SoundID.Item34;
            Item.value = Item.sellPrice(gold: 12);
            Item.rare = ItemRarityID.Lime;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BrimstoneBallFriendly>();
            Item.shootSpeed = 10f;
            Item.useAmmo = AmmoID.Gel;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.Ranged
{
    /// <summary>Pieu enflammé de l'Impaler : 9 ennemis, On Fire! 4 s ; tue net les vampires ; 1 chance sur 2 de redevenir un pieu.</summary>
    public class FlamingStake : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Legacy/Projectiles/Ranged/ExplodingStake";

        protected virtual int Pierce => 10;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = Pierce;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 3;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            AIType = ProjectileID.Stake;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(5))
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Torch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.type == NPCID.Vampire || target.type == NPCID.VampireBat)
                modifiers.SetInstantKill();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.OnFire, 240);

        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer && Main.rand.NextBool(2))
                Item.NewItem(Projectile.GetSource_DropAsItem(), Projectile.Hitbox, ItemID.Stake);
        }
    }

    /// <summary>Pieu explosif de l'Impaler : 7 ennemis, explosion de Calamity (On Fire! 3 s, infobox) à chaque ennemi touché.</summary>
    public class ExplodingStake : FlamingStake
    {
        public override string Texture => "CalamityFandom/Content/Legacy/Projectiles/Ranged/ExplodingStake";

        protected override int Pierce => 8;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.scale = 0.8f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.owner != Main.myPlayer)
                return;
            var boom = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FuckYou>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
            boom.DamageType = DamageClass.Ranged;
            LegacyHitDebuff.Give(boom, BuffID.OnFire, 180);
        }
    }

    /// <summary>Étoile tombée de la Star Cannon EX : traverse tous les ennemis (dégâts x0,9375 à chaque fois), arrêtée par les blocs.</summary>
    public class FallenStarProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.alpha = 50;
            Projectile.light = 1f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 20 + Main.rand.Next(40);
                SoundEngine.PlaySound(SoundID.Item9, Projectile.Center);
            }
            if (Projectile.localAI[0] == 0f)
                Projectile.localAI[0] = 1f;
            Projectile.alpha += (int)(25f * Projectile.localAI[0]);
            if (Projectile.alpha > 200)
            {
                Projectile.alpha = 200;
                Projectile.localAI[0] = -1f;
            }
            if (Projectile.alpha < 50)
            {
                Projectile.alpha = 50;
                Projectile.localAI[0] = 1f;
            }
            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * Projectile.direction;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => Projectile.damage = Math.Max(1, (int)(Projectile.damage * 0.9375f));

        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, lightColor.A - Projectile.alpha);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 58, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 150, default, 1.2f);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 57, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 150, default, 1.2f);
            }
            if (!Main.dedServ)
                for (int i = 0; i < 6; i++)
                    Gore.NewGore(Projectile.GetSource_Death(), Projectile.position, Projectile.velocity * 0.05f, Main.rand.Next(16, 18));
        }
    }

    /// <summary>Boule de soufre du Brimstone Flameblaster : 5 rebonds puis disparaît, 6 contacts en tout, Brimstone Flames 3 s.</summary>
    public class BrimstoneBallFriendly : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.scale = 1.2f;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 6;
            Projectile.aiStyle = 14; // boule rebondissante vanilla
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            Projectile.rotation += 0.12f * Projectile.direction;
            Lighting.AddLight(Projectile.Center, 0.25f, 0f, 0f);
            if (++Projectile.localAI[0] > 4f)
            {
                var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, (int)CalamityDusts.Brimstone, 0f, 0f, 150, default, 1.5f);
                dust.noGravity = true;
                dust.velocity = Vector2.Zero;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (--Projectile.penetrate <= 0)
                return true;
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            Projectile.velocity *= 0.98f;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 180);
    }
}
