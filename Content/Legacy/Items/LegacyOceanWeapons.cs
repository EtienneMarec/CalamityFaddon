// Adapté de Calamity Mod (objets refaits en mai 2022 : Reed Blowgun, Urchin Mace, Fishbone Boomerang...), © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using System;
using CalamityFandom.Content.Legacy.Projectiles.Melee;
using CalamityFandom.Content.Legacy.Projectiles.Ranged;
using CalamityFandom.Content.Legacy.Projectiles.Rogue;
using CalamityMod;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

// Armes de l'océan d'avant la refonte de mai 2022 (décision du 30/10) : nos portages en Full Fandom, l'arme actuelle de
// Calamity sous le nom du wiki en Calamity++ (masquage automatique par tools/gen_baseline.py). Recettes du wiki.

namespace CalamityFandom.Content.Legacy.Items.Weapons.Ranged
{
    /// <summary>Wiki : Seabow.txt — arc : chaque tir ajoute une gerbe d'eau lente et courte (40 % des dégâts).</summary>
    public class Seabow : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 12;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 22;
            Item.height = 46;
            Item.useTime = Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.5f;
            Item.value = Item.sellPrice(silver: 40);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 16f;
            Item.useAmmo = AmmoID.Arrow;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // « The water blast's velocity varies with the velocity of arrow type » : moitié de la vitesse de la flèche
            Vector2 speed = (velocity + new Vector2(Main.rand.Next(-30, 31), Main.rand.Next(-30, 31)) * 0.05f) * 0.5f;
            Projectile.NewProjectile(source, position, speed, ModContent.ProjectileType<SandWater>(), (int)(damage * 0.4f), 0f, player.whoAmI);
            return true;
        }
    }
}

namespace CalamityFandom.Content.Legacy.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Redtide Sword.txt — tourbillon lumineux qui grossit (hitbox fixe), traverse un ennemi, fait moitié des
    /// dégâts de l'épée, va un peu loin puis éclate en gouttes d'eau lumineuses.
    /// </summary>
    public class RedtideSword : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = Item.height = 42;
            Item.damage = 35;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.knockBack = 4f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = Item.sellPrice(silver: 40);
            Item.rare = ItemRarityID.Green;
            Item.shoot = ModContent.ProjectileType<LegacyRedtideWhirlpool>();
            Item.shootSpeed = 9f;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
            => damage /= 2;
    }

    /// <summary>
    /// Wiki : Urchin Spear.txt — lance ; à chaque coup, un dard à tête chercheuse lente (dard de Calamity) ;
    /// Poisoned 10 s par la lance (3 s dans l'ancien code), Venom 3 s par le dard (infobox).
    /// </summary>
    public class UrchinSpear : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = Item.height = 56;
            Item.damage = 28;
            Item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Item.noMelee = true;
            Item.useTurn = true;
            Item.noUseGraphic = true;
            Item.useTime = Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = Item.sellPrice(silver: 40);
            Item.rare = ItemRarityID.Green;
            Item.shoot = ModContent.ProjectileType<UrchinSpearProjectile>();
            Item.shootSpeed = 4f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;
    }

    /// <summary>
    /// Wiki : Urchin Flail.txt — fléau vendu 2 or par le Sea King : la boule d'oursin, sans gravité, revient dès qu'elle
    /// touche un bloc ou un ennemi et lance alors une épine (Venom 3 s par la boule, 2 s par l'épine). Pas de nouveau
    /// lancer tant qu'elle est sortie.
    /// </summary>
    public class UrchinFlail : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 33;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.width = 44;
            Item.height = 36;
            Item.useTime = Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 6f;
            Item.value = Item.buyPrice(gold: 2);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.channel = true;
            Item.shoot = ModContent.ProjectileType<UrchinBall>();
            Item.shootSpeed = 12f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;
    }

    /// <summary>Le Sea King vend l'Urchin Flail (page de l'arme : « purchased from the Sea King at any time »).</summary>
    public class UrchinFlailShop : GlobalNPC
    {
        public override void ModifyShop(NPCShop shop)
        {
            if (ModContent.TryFind<ModNPC>("CalamityMod", "SeaKing", out var seaKing) && shop.NpcType == seaKing.Type)
                shop.Add(ModContent.ItemType<UrchinFlail>());
        }
    }
}

namespace CalamityFandom.Content.Legacy.Items.Weapons.Rogue
{
    /// <summary>
    /// Wiki : Seashell Boomerang.txt — boomerang rogue lancé sans limite de nombre ; en attaque furtive, il tire
    /// régulièrement des coquillages (projectile de Calamity) sur les ennemis proches.
    /// </summary>
    public class SeashellBoomerang : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 34;
            Item.damage = 15;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useTime = Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5.5f;
            Item.UseSound = SoundID.Item1;
            Item.value = Item.sellPrice(silver: 40);
            Item.rare = ItemRarityID.Green;
            Item.shoot = ModContent.ProjectileType<SeashellBoomerangProjectile>();
            Item.shootSpeed = 11.5f;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            proj.Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.Ranged
{
    /// <summary>Gerbe d'eau du Seabow : invisible (poussière d'eau), 1 s, un ennemi.</summary>
    public class SandWater : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 60;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0f, 0f, 0.5f);
            for (int i = 0; i < 10; i++)
            {
                var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 33, 0f, 0f, 100, default, 0.9f);
                dust.noGravity = true;
                dust.velocity = dust.velocity * 0.5f + Projectile.velocity * 0.1f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item21, Projectile.position);
            for (int i = 0; i <= 40; i++)
            {
                var dust = Dust.NewDustPerfect(Projectile.Center + new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11)), 33,
                    Main.rand.NextVector2Unit() * Main.rand.Next(3, 9), 100, default, 0.6f);
                dust.noGravity = true;
            }
        }
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.Melee
{
    /// <summary>Tourbillon de la Redtide Sword (code de mai 2022) : grossit de 0,5 à 1, traverse un ennemi, éclate en gouttes.</summary>
    public class LegacyRedtideWhirlpool : ModProjectile
    {
        public override void SetStaticDefaults() => Main.projFrames[Type] = 3;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 60;
            Projectile.scale = 0.5f;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 2;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0f, 0f, Projectile.scale * 1.5f);
            for (int i = 0; i < 6; i++)
            {
                Vector2 edge = (Vector2.Normalize(Projectile.velocity) * new Vector2(Projectile.width / 2f, Projectile.height) * 0.75f)
                    .RotatedBy((i - 2) * MathHelper.Pi / 6f) + Projectile.Center;
                Vector2 jitter = (Main.rand.NextFloat(MathHelper.Pi) - MathHelper.PiOver2).ToRotationVector2() * Main.rand.Next(3, 8);
                var dust = Dust.NewDustDirect(edge + jitter, 0, 0, 172, jitter.X * 2f, jitter.Y * 2f, 100, default, Projectile.scale);
                dust.noGravity = true;
                dust.noLight = true;
                dust.velocity = dust.velocity / 4f - Projectile.velocity;
            }
            if (Projectile.scale <= 1f)
                Projectile.scale *= 1.015f;
            if (Projectile.scale >= 1f)
                Projectile.Kill();
            if (++Projectile.frameCounter > 3)
            {
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
                Projectile.frameCounter = 0;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            int frameHeight = tex.Height / Main.projFrames[Type];
            var frame = new Rectangle(0, frameHeight * Projectile.frame, tex.Width, frameHeight);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), frame, Projectile.GetAlpha(lightColor),
                Projectile.rotation, frame.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath19, Projectile.position);
            for (int i = 0; i < 36; i++)
            {
                Vector2 point = (Vector2.Normalize(Projectile.oldVelocity) * new Vector2(Projectile.width / 2f, Projectile.height) * 0.75f)
                    .RotatedBy((i - 17) * MathHelper.TwoPi / 36f) + Projectile.Center;
                Vector2 away = point - Projectile.Center;
                var dust = Dust.NewDustDirect(point + away, 0, 0, 172, away.X * 2f, away.Y * 2f, 100, default, 1.4f);
                dust.noGravity = true;
                dust.noLight = true;
                dust.velocity = away;
            }
        }
    }

    /// <summary>Lance de l'Urchin Spear : lance un dard de Calamity avant de revenir ; Poisoned 10 s.</summary>
    public class UrchinSpearProjectile : BaseSpearProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 56;
            Projectile.aiStyle = ProjAIStyleID.Spear;
            Projectile.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Projectile.timeLeft = 90;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
        }

        public override float InitialSpeed => 3f;
        public override float ReelbackSpeed => 2.4f;
        public override float ForwardSpeed => 0.95f;

        public override Action<Projectile> EffectBeforeReelback => proj =>
        {
            var stinger = Projectile.NewProjectileDirect(proj.GetSource_FromThis(), proj.Center + proj.velocity, proj.velocity * 1.6f,
                ModContent.ProjectileType<UrchinSpikeFugu>(), proj.damage, proj.knockBack * 0.5f, proj.owner);
            stinger.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            LegacyHitDebuff.Give(stinger, BuffID.Venom, 180);
        };

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Poisoned, 600);
    }

    /// <summary>Boule de l'Urchin Flail : part tout droit, revient au premier bloc ou ennemi (en lançant une épine), chaîne de 2022.</summary>
    public class UrchinBall : ModProjectile
    {
        private bool spike;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 34;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 3;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Vector2 toPlayer = player.Center - Projectile.Center;
            Projectile.rotation = toPlayer.ToRotation() - MathHelper.PiOver2;
            if (player.dead)
            {
                Projectile.Kill();
                return;
            }
            player.itemAnimation = player.itemTime = 10;
            Projectile.direction = toPlayer.X < 0f ? 1 : -1;
            player.ChangeDir(Projectile.direction);
            player.itemRotation = (toPlayer * -Projectile.direction).ToRotation();
            Projectile.spriteDirection = toPlayer.X > 0f ? -1 : 1;
            if (Projectile.ai[0] == 0f && toPlayer.Length() > 400f)
                Projectile.ai[0] = 1f;
            if (Projectile.ai[0] >= 1f)
            {
                if (spike)
                {
                    spike = false;
                    if (Projectile.owner == Main.myPlayer)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<UrchinBallSpike>(),
                            Projectile.damage / 2, 0f, Projectile.owner);
                }
                float dist = toPlayer.Length();
                if (dist > 1500f)
                {
                    Projectile.Kill();
                    return;
                }
                if (dist > 600f)
                    Projectile.ai[0] = 2f;
                Projectile.tileCollide = false;
                float speed = Projectile.ai[0] == 2f ? 40f : 20f;
                Projectile.velocity = Vector2.Normalize(toPlayer) * speed;
                if (dist < speed)
                {
                    Projectile.Kill();
                    return;
                }
            }
            if (++Projectile.ai[1] > 5f)
                Projectile.alpha = 0;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            Projectile.ai[0] = 1f;
            Projectile.netUpdate = true;
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 mounted = Main.player[Projectile.owner].MountedCenter;
            Texture2D chain = ModContent.Request<Texture2D>("CalamityFandom/Content/Legacy/Projectiles/Melee/UrchinFlailChain").Value;
            Vector2 pos = Projectile.Center;
            Vector2 toPlayer = mounted - pos;
            float rotation = toPlayer.ToRotation() - MathHelper.PiOver2;
            while (!float.IsNaN(toPlayer.X) && toPlayer.Length() >= chain.Height + 1f)
            {
                pos += Vector2.Normalize(toPlayer) * chain.Height;
                toPlayer = mounted - pos;
                Main.EntitySpriteDraw(chain, pos - Main.screenPosition, null, Lighting.GetColor(pos.ToTileCoordinates()), rotation, chain.Size() / 2f, 1f, SpriteEffects.None, 0);
            }
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            spike = true;
            Projectile.ai[0] = 1f;
            Projectile.netUpdate = true;
            target.AddBuff(BuffID.Venom, 180);
        }
    }

    /// <summary>Épine de l'Urchin Flail : tombe en tournant, Venom 2 s. [choix] Ne colle plus aux ennemis (fonction de Calamity interne).</summary>
    public class UrchinBallSpike : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            Projectile.velocity.X *= 0.99f;
            Projectile.velocity.Y += 0.15f;
            Projectile.rotation += 0.4f * Projectile.direction;
            Projectile.spriteDirection = Projectile.direction;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int i = 0; i < 20; i++)
            {
                var dust = Dust.NewDustDirect(Projectile.position, 13, 20, 14, 0f, 0f, 0, new Color(0, 255, 255), 1f);
                dust.noGravity = true;
                dust.velocity *= 2f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Venom, 120);
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.Rogue
{
    /// <summary>Boomerang du Seashell Boomerang (IA de boomerang vanilla) ; en attaque furtive, tire un coquillage toutes les 20 frames.</summary>
    public class SeashellBoomerangProjectile : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Legacy/Items/Weapons/Rogue/SeashellBoomerang";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.aiStyle = ProjAIStyleID.Boomerang;
            Projectile.timeLeft = 240;
            AIType = ProjectileID.WoodenBoomerang;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
        }

        public override void AI()
        {
            if (!Projectile.Calamity().stealthStrike || Projectile.owner != Main.myPlayer || ++Projectile.localAI[1] < 20f)
                return;
            Projectile.localAI[1] = 0f;
            NPC target = Projectile.Center.ClosestNPCAt(300f);
            if (target == null)
                return;
            var shell = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitY) * 10f,
                ModContent.ProjectileType<Seashell>(), Projectile.damage / 2, Projectile.knockBack * 0.5f, Projectile.owner);
            shell.DamageType = ModContent.GetInstance<RogueDamageClass>();
        }
    }
}
