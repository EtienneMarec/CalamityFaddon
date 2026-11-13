// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using System;
using CalamityFandom.Content.Legacy.Projectiles.Rogue;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

// Lot du 13/11 (suite) : Acidic Rain Barrel, Nychthemeron. Code de 2.0.1.005, valeurs des infobox et du texte des pages ;
// recettes du wiki.

namespace CalamityFandom.Content.Legacy.Items.Weapons.Rogue
{
    /// <summary>
    /// Wiki : Acidic Rain Barrel.txt — tonneau qui roule en traversant les ennemis et éclate contre un mur en éclats
    /// (Irradiated 3 s) et gouttes acides (Irradiated 3-5 s, Poisoned 1-3 s une fois sur deux, Sulphuric Poisoning 1-2 s une
    /// fois sur huit) ; frappe furtive : rebondit jusqu'à trois fois et lâche davantage.
    /// </summary>
    public class AcidicRainBarrel : RogueWeapon
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = Item.height = 48;
            Item.damage = 43;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = Item.useAnimation = 21;
            Item.knockBack = 8f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = Item.sellPrice(gold: 7, silver: 20);
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ModContent.ProjectileType<GreenDonkeyKongReference>();
            Item.shootSpeed = 14f;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
        }

        public override void ModifyStatsExtra(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position -= Vector2.UnitY * 12f;
            velocity.Y *= 0.667f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (p.WithinBounds(Main.maxProjectiles))
                Main.projectile[p].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }
    }

    /// <summary>
    /// Wiki : Nychthemeron.txt — boule qui ignore la gravité et ralentit (10 au plus, selon la pile) ; chacune fait tourner
    /// une orbe de lumière et une de ténèbres autour du joueur, qui foncent sur les ennemis quand la boule casse (4 ennemis,
    /// un bloc, rappel ou arrêt) ; frappe furtive : toutes les boules d'un coup ; clic droit : rappelle les boules.
    /// </summary>
    public class Nychthemeron : RogueWeapon
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 10;

        public override void SetDefaults()
        {
            Item.width = Item.height = 18;
            Item.damage = 60;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useTime = Item.useAnimation = 19;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 1f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.maxStack = 10;
            Item.value = Item.sellPrice(silver: 72);
            Item.rare = ItemRarityID.LightPurple;
            Item.shoot = ModContent.ProjectileType<NychthemeronProjectile>();
            Item.shootSpeed = 6f;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
        }

        public override float StealthDamageMultiplier => 0.3333f;

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            int balls = player.ownedProjectileCounts[ModContent.ProjectileType<NychthemeronProjectile>()];
            return player.altFunctionUse == 2 ? balls > 0 : balls < Item.stack;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                // Rappel : les boules reviennent de plus en plus vite
                foreach (var p in Main.ActiveProjectiles)
                    if (p.owner == player.whoAmI && p.type == type && p.ai[0] == 0f)
                    {
                        p.ai[0] = 1f;
                        p.netUpdate = true;
                    }
                return false;
            }
            int orbDamage = (int)(damage * 0.75f);
            if (player.Calamity().StealthStrikeAvailable())
            {
                int count = Item.stack - player.ownedProjectileCounts[type];
                for (int i = 0; i < count; i++)
                {
                    Vector2 spread = velocity + Main.rand.NextVector2Square(-2f, 2f);
                    int ball = Projectile.NewProjectile(source, position, spread, type, damage, knockback, player.whoAmI);
                    if (!ball.WithinBounds(Main.maxProjectiles))
                        continue;
                    Main.projectile[ball].Calamity().stealthStrike = true;
                    CreateOrbs(source, position, (int)(orbDamage * 0.675f), knockback, ball, player, true);
                }
                return false;
            }
            int single = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (single.WithinBounds(Main.maxProjectiles))
                CreateOrbs(source, position, orbDamage, knockback, single, player, false);
            return false;
        }

        /// <summary>Deux orbes opposées sur le premier cercle libre autour du joueur (10 places, 3 cercles).</summary>
        private static void CreateOrbs(IEntitySource source, Vector2 position, int damage, float knockback, int ball, Player player, bool stealth)
        {
            int orbType = ModContent.ProjectileType<NychthemeronOrb>();
            int[] slots = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
            foreach (var p in Main.ActiveProjectiles)
                if (p.type == orbType && p.owner == player.whoAmI && p.localAI[0] == 0f && slots[(int)p.localAI[1]] == -1)
                    slots[(int)p.localAI[1]] = p.whoAmI;
            float rotation = 0f;
            int pos = 0;
            bool assigned = false;
            for (int i = 0; i < 10; i++)
            {
                if (slots[i] != -1)
                {
                    rotation = Main.projectile[slots[i]].rotation;
                    assigned = true;
                }
                else if (assigned)
                {
                    pos = i;
                    break;
                }
            }
            float color1 = 0f, color2 = 1f;
            if (pos > 0 && pos < 5)
            {
                rotation += MathHelper.ToRadians(45f);
                color1 = color2 = pos % 2;
            }
            else if (pos >= 5)
                rotation += MathHelper.ToRadians(72f);
            for (int k = 0; k < 2; k++)
            {
                int orb = Projectile.NewProjectile(source, position, Vector2.Zero, orbType, damage, knockback, player.whoAmI, k == 0 ? color1 : color2, ball, stealth ? 1f : 0f);
                if (!orb.WithinBounds(Main.maxProjectiles))
                    continue;
                Main.projectile[orb].localAI[1] = pos;
                Main.projectile[orb].rotation = rotation + (k == 0 ? 0f : MathHelper.Pi);
            }
        }
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.Rogue
{
    /// <summary>Tonneau de l'Acidic Rain Barrel : roule, traverse les ennemis, éclate contre un mur (1 fois, 3 en frappe furtive).</summary>
    public class GreenDonkeyKongReference : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Legacy/Items/Weapons/Rogue/AcidicRainBarrel";

        private float cooldown, oldVelocityX;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 480;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
        }

        public override void AI()
        {
            bool stealth = Projectile.Calamity().stealthStrike;
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.ai[1] = stealth ? 3 : 1;
                Projectile.localAI[0] = 1f;
            }
            Projectile.rotation += Math.Sign(Projectile.velocity.X) * MathHelper.ToRadians(8f);
            if (Projectile.velocity.Y < 15f)
                Projectile.velocity.Y += 0.3f;
            if (Projectile.oldPosition.X == Projectile.position.X && cooldown == 0)
            {
                BounceEffects(stealth);
                Projectile.velocity.X = -oldVelocityX;
            }
            else if (cooldown > 0)
                cooldown--;
            if (Projectile.velocity.X != 0f)
                oldVelocityX = Math.Sign(Projectile.velocity.X) * 12f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        private void BounceEffects(bool stealth)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            if (Projectile.owner == Main.myPlayer)
            {
                var source = Projectile.GetSource_FromThis();
                int count = stealth ? 13 : 8;
                for (int i = 0; i < count; i++)
                {
                    if (Main.rand.NextBool(3))
                    {
                        Vector2 v = (Vector2.UnitY * (-16f + Main.rand.NextFloat(-3, 12f))).RotatedByRandom(MathHelper.ToRadians(30f));
                        Projectile.NewProjectile(source, Projectile.Center, Projectile.velocity + v, ModContent.ProjectileType<AcidShrapnel>(), Projectile.damage, 3f, Projectile.owner);
                    }
                    else
                        SpawnDrop(source, 0.75f, 300, true);
                }
                if (stealth)
                    for (int i = 0; i < 6; i++)
                        SpawnDrop(source, 0.667f, 420, false);
            }
            Projectile.ai[1]--;
            cooldown = 15;
            if (Projectile.ai[1] <= 0)
                Projectile.Kill();
        }

        private void SpawnDrop(IEntitySource source, float damageMult, int timeLeft, bool localImmunity)
        {
            Vector2 v = (Vector2.UnitY * (-16f + Main.rand.NextFloat(-3, 12f))).RotatedByRandom(MathHelper.ToRadians(40f));
            int drop = Projectile.NewProjectile(source, Projectile.Center, Projectile.velocity + v, ModContent.ProjectileType<AcidBarrelDrop>(),
                (int)(Projectile.damage * damageMult), 1f, Projectile.owner);
            if (!drop.WithinBounds(Main.maxProjectiles))
                return;
            Main.projectile[drop].timeLeft = timeLeft;
            if (localImmunity)
            {
                Main.projectile[drop].usesLocalNPCImmunity = true;
                Main.projectile[drop].localNPCHitCooldown = -1;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 2);
            return false;
        }
    }

    /// <summary>Goutte acide de l'Acidic Rain Barrel (texture des gouttes de l'Acid Rain de Calamity).</summary>
    public class AcidBarrelDrop : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Environment/AcidDrop";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.timeLeft = 420;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            if (Projectile.velocity.Y <= 8f)
                Projectile.velocity.Y += 0.15f;
            Projectile.tileCollide = Projectile.timeLeft <= 300;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (int i = 0; i < 4; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.position - Projectile.velocity, 2, 2, 154, 0f, 0f, 0, new Color(112, 150, 42, 127));
                d.position.X -= 2f;
                d.alpha = 38;
                d.velocity = d.velocity * 0.1f - Projectile.velocity * 0.025f;
                d.scale = 0.75f;
            }
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), Main.rand.Next(180, 301));
            if (Main.rand.NextBool(2))
                target.AddBuff(BuffID.Poisoned, Main.rand.Next(60, 181));
            if (Main.rand.NextBool(8))
                target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), Main.rand.Next(60, 121));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], new Color(255, 255, 255, 127), 2);
            return false;
        }
    }

    /// <summary>Éclat de l'Acidic Rain Barrel (texture des éclats du Blast Barrel) : tombe, s'arrête au sol, Irradiated 3 s.</summary>
    public class AcidShrapnel : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Rogue/BarrelShrapnel";

        private bool hitTile;

        public override void SetStaticDefaults() => Main.projFrames[Type] = 3;

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 160;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (hitTile)
            {
                Projectile.velocity.X = 0f;
                Projectile.rotation = MathHelper.Pi;
            }
            else
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.velocity.Y += 0.2f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            hitTile = true;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Irradiated>(), 180);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<Irradiated>(), 180);

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, (int)CalamityDusts.SulphurousSeaAcid);
                dust.velocity = Vector2.UnitY.RotatedBy(i / 15f * MathHelper.TwoPi) * 3f * (float)Math.Cos(i / 15f * MathHelper.TwoPi);
                dust.scale = 2.5f;
                dust.noGravity = true;
            }
        }
    }

    /// <summary>Boule du Nychthemeron : ignore la gravité, ralentit ; 4 ennemis au plus ; casse au contact d'un bloc ; rappel (ai[0] > 0).</summary>
    public class NychthemeronProjectile : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Legacy/Items/Weapons/Rogue/Nychthemeron";

        private const int Lifetime = 300;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 4;
            Projectile.timeLeft = Lifetime;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0f)
            {
                Projectile.velocity *= 0.99f;
                // Casse quand elle s'est arrêtée (page)
                if (Projectile.velocity.LengthSquared() < 0.01f && Projectile.owner == Main.myPlayer)
                    Projectile.Kill();
            }
            else
            {
                Player owner = Main.player[Projectile.owner];
                Projectile.tileCollide = false;
                Projectile.velocity = Projectile.SafeDirectionTo(owner.Center) * Projectile.ai[0];
                Projectile.ai[0] = Math.Min(Projectile.ai[0] + 0.5f, 20f);
                Projectile.extraUpdates = 1;
                if (Main.myPlayer == Projectile.owner && Projectile.Hitbox.Intersects(owner.Hitbox))
                    Projectile.Kill();
            }
            Projectile.rotation += 0.4f * Projectile.direction * (Projectile.timeLeft / (float)Lifetime);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            BreakDust();
            Projectile.Kill();
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            if (timeLeft <= 0)
                BreakDust();
        }

        private void BreakDust()
        {
            for (int i = 0; i < 2; i++)
            {
                Dust.NewDust(Projectile.position, 4, 4, 236, Projectile.velocity.X, Projectile.velocity.Y, 0, default, Main.rand.NextFloat(0.9f, 1.1f));
                Dust.NewDust(Projectile.position, 4, 4, 240, Projectile.velocity.X, Projectile.velocity.Y, 0, default, Main.rand.NextFloat(0.9f, 1.1f));
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }

    /// <summary>
    /// Orbe du Nychthemeron : tourne autour du joueur tant que sa boule (ai[1]) existe, puis fonce sur l'ennemi le plus
    /// proche. ai[0] = 0 lumière, 1 ténèbres ; ai[2] = 1 en frappe furtive (vit moins longtemps) ; localAI[1] = place.
    /// </summary>
    public class NychthemeronOrb : ModProjectile
    {
        private static readonly float[] PlayerDists = { 48f, 80f, 80f, 80f, 80f, 112f, 112f, 112f, 112f, 112f };
        private static readonly float[] RotationSpeeds = { 0.1f, -0.075f, -0.075f, -0.075f, -0.075f, 0.05f, 0.05f, 0.05f, 0.05f, 0.05f };

        private Vector2 lastVelocity;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Projectile ball = Main.projectile[(int)Projectile.ai[1]];
            bool ballAlive = ball.active && ball.owner == Projectile.owner && ball.type == ModContent.ProjectileType<NychthemeronProjectile>();
            if (!ballAlive && Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                Projectile.penetrate = 1;
            }
            Player player = Main.player[Projectile.owner];
            if (Projectile.localAI[0] == 0f)
            {
                int slot = (int)Projectile.localAI[1];
                Projectile.rotation += RotationSpeeds[slot];
                Projectile.Center = player.Center + new Vector2(0f, -PlayerDists[slot]).RotatedBy(Projectile.rotation);
                lastVelocity = Projectile.position - Projectile.oldPosition;
                return;
            }
            if (Projectile.timeLeft > 240 && Projectile.ai[2] == 1f)
                Projectile.timeLeft = 240;
            NPC target = Projectile.Center.ClosestNPCAt(999f);
            if (target != null)
            {
                Projectile.velocity += Projectile.SafeDirectionTo(target.Center);
                if (Projectile.velocity.Length() > 10f)
                    Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 10f;
                lastVelocity = Projectile.velocity;
            }
            else
                Projectile.velocity = lastVelocity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Projectile.ai[0] == 0f ? Texture : Texture + "2").Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
