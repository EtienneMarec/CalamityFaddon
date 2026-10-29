// Adapté de Calamity Mod (objets retirés en 2022), © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using System;
using CalamityFandom.Content.Legacy.Projectiles.Rogue;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.Dusts;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.Weapons.Rogue
{
    /// <summary>
    /// Wiki : Latcher Mine.txt — mines rogue consommables (pile de 9999) vendues 3 argent par le Bandit après un boss
    /// mécanique ; collent aux ennemis et explosent 3 s plus tard, se brisent sur les blocs ; l'explosion projette 6 flammes
    /// (On Fire! 10 s). Attaque furtive : la mine colle au sol (14 s), s'accroche aux ennemis qui la touchent et lâche aussi
    /// des éclats.
    /// </summary>
    public class LatcherMine : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 99;

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 32;
            Item.damage = 80;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.noMelee = true;
            Item.consumable = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 14;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.maxStack = 9999;
            Item.value = Item.buyPrice(silver: 3);
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ModContent.ProjectileType<LatcherMineProjectile>();
            Item.shootSpeed = 10f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var mine = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            mine.Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }
    }

    /// <summary>
    /// Wiki : Quasar.txt — dague rogue lâchée par Astrum Deus (10 %, butin du wiki), comme la Radiant Star : le couteau
    /// attire les ennemis (pas les boss, ni quand un boss est en vie), puis éclate en trois couteaux et une explosion
    /// astrale (pluie d'étoiles sur les ennemis touchés). Les couteaux éclatés ne cherchent pas les ennemis. Attaque
    /// furtive : chaque couteau éclaté explose à son tour. Critique 16 %.
    /// </summary>
    public class Quasar : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 48;
            Item.damage = 80;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useTime = Item.useAnimation = 11;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 0f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = Item.sellPrice(gold: 16);
            Item.rare = ItemRarityID.Cyan;
            Item.shoot = ModContent.ProjectileType<QuasarKnife>();
            Item.shootSpeed = 20f;
        }

        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 12;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var knife = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            knife.Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.Rogue
{
    /// <summary>
    /// Mine de la Latcher Mine. ai[0] : 0 en vol, 1 collée à l'ennemi ai[1], 2 posée au sol (attaque furtive).
    /// Explosion de 100 px (240 en attaque furtive) ; 6 projectiles projetés vers le haut.
    /// </summary>
    public class LatcherMineProjectile : ModProjectile
    {
        private bool Stealth => Projectile.Calamity().stealthStrike;

        public override void SetStaticDefaults() => Main.projFrames[Type] = 4;

        public override void SetDefaults()
        {
            Projectile.width = 27;
            Projectile.height = 15;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.scale = 1.5f;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            int mult = Stealth ? 2 : 1;
            switch (Projectile.ai[0])
            {
                case 0f:
                    Projectile.rotation += Projectile.velocity.X > 0 ? 0.3f : -0.3f;
                    if (Projectile.velocity.Y < 7f)
                        Projectile.velocity.Y += 0.24f;
                    break;
                case 1f:
                    Projectile.tileCollide = false;
                    int target = (int)Projectile.ai[1];
                    if (target < 0 || target >= Main.maxNPCs || !Main.npc[target].active || Main.npc[target].dontTakeDamage)
                    {
                        Projectile.Kill();
                        return;
                    }
                    NPC npc = Main.npc[target];
                    Projectile.Center = npc.Center - Projectile.velocity * 2f;
                    Projectile.gfxOffY = npc.gfxOffY;
                    Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
                    // « detonates after 3 seconds » (l'ancien code : 2 s)
                    Projectile.timeLeft = Math.Min(Projectile.timeLeft, 180);
                    break;
                case 2f:
                    Projectile.velocity = Vector2.UnitY * 3f;
                    Projectile.rotation = 0f;
                    break;
            }
            // Clignote de plus en plus vite avant d'exploser
            if (Projectile.timeLeft == 110 * mult || Projectile.timeLeft == 60 * mult || Projectile.timeLeft == 24 * mult)
                Projectile.frame = Math.Min(Projectile.frame + 1, 3);
            if (Projectile.timeLeft < 24 * mult && ++Projectile.frameCounter % 2 == 1)
                Projectile.frame = (Projectile.frame + 1) % 4;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!Stealth)
                return true;
            Projectile.height = 10;
            if (Projectile.localAI[1] == 0f)
            {
                Projectile.timeLeft = 14 * 60;
                Projectile.localAI[1] = 1f;
            }
            Projectile.ai[0] = 2f;
            return false;
        }

        public override bool? CanHitNPC(NPC target) => Projectile.ai[0] == 1f ? false : null;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[0] == 1f)
                return;
            // Colle à l'ennemi : position relative gardée dans la vitesse
            Projectile.ai[0] = 1f;
            Projectile.ai[1] = target.whoAmI;
            Projectile.velocity = (target.Center - Projectile.Center) * 0.75f;
            Projectile.netUpdate = true;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
                targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
            return projHitbox.Intersects(targetHitbox);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            Projectile.Resize(Stealth ? 240 : 100, Stealth ? 240 : 100);
            Projectile.ai[0] = 0f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.Damage();
            if (Projectile.owner != Main.myPlayer)
                return;
            // [choix] « a burst of 6 gravity-affected flame projectiles » ; en attaque furtive, moitié d'éclats
            for (int i = 0; i < 6; i++)
            {
                Vector2 velocity = (Vector2.UnitY * (-16f + Main.rand.NextFloat(-3f, 12f))).RotatedByRandom(MathHelper.ToRadians(40f));
                if (Stealth && Main.rand.NextBool(2))
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Top, velocity, ModContent.ProjectileType<BarrelShrapnel>(), Projectile.damage, 3f, Projectile.owner);
                else
                {
                    var fire = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Top, velocity, ModContent.ProjectileType<TotalityFire>(), Projectile.damage / 3, 1f, Projectile.owner);
                    fire.localNPCHitCooldown = -2;
                    LegacyHitDebuff.Give(fire, BuffID.OnFire, 600);
                }
            }
        }
    }

    /// <summary>Couteau de la Quasar : attire les ennemis proches, éclate au bout de 25 frames (3 couteaux + explosion de la Radiant Star).</summary>
    public class QuasarKnife : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Legacy/Items/Weapons/Rogue/Quasar";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 4;
            Projectile.timeLeft = 300;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
        }

        /// <summary>« Succ » : attire les ennemis non-boss à moins de 600 px (distance de Manhattan), sauf pendant un combat de boss.</summary>
        public static void Pull(Projectile projectile)
        {
            if (CalamityPlayer.areThereAnyDamnBosses)
                return;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (!npc.CanBeChasedBy(projectile) || npc.boss || !Collision.CanHit(projectile.Center, 1, 1, npc.Center, 1, 1))
                    continue;
                if (Math.Abs(projectile.Center.X - npc.Center.X) + Math.Abs(projectile.Center.Y - npc.Center.Y) >= 600f)
                    continue;
                npc.velocity.X += npc.position.X < projectile.Center.X ? 0.25f : -0.25f;
                npc.velocity.Y += npc.position.Y < projectile.Center.Y ? 0.25f : -0.25f;
            }
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            Pull(Projectile);
            if (++Projectile.ai[1] != 25f || Projectile.owner != Main.myPlayer)
                return;
            bool stealth = Projectile.Calamity().stealthStrike;
            for (int i = 0; i < 3; i++)
            {
                Vector2 speed = Main.rand.NextVector2Unit() * Main.rand.Next(30, 61) * 0.25f;
                var knife = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, speed, ModContent.ProjectileType<Quasar2>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                knife.Calamity().stealthStrike = stealth;
                if (stealth)
                    knife.timeLeft = 120;
            }
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<RadiantExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            Projectile.Kill();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 180);

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, ModContent.DustType<AstralBlue>(), Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
        }
    }

    /// <summary>Couteau éclaté de la Quasar : attire aussi, sans tête chercheuse (page) ; en attaque furtive, explose à sa mort.</summary>
    public class Quasar2 : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Legacy/Items/Weapons/Rogue/Quasar";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            QuasarKnife.Pull(Projectile);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 180);

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, ModContent.DustType<AstralBlue>(), Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            if (!Projectile.Calamity().stealthStrike || Projectile.owner != Main.myPlayer)
                return;
            var boom = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<RadiantExplosion>(), (int)(Projectile.damage * 0.5f), Projectile.knockBack, Projectile.owner, 0f, 1f);
            boom.Calamity().stealthStrike = true;
            boom.Resize(280, 280);
        }
    }
}
