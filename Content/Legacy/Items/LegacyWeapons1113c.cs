// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using System;
using CalamityFandom.Content.Legacy.Buffs.Potions;
using CalamityFandom.Content.Legacy.Projectiles.Magic;
using CalamityFandom.Content.Legacy.Projectiles.Rogue;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

// Lot du 13/11 (fin) : Tesla Potion, Yharim's Stimulants, Divine Retribution, Hell's Sun. Code de 2.0.1.005, valeurs des
// infobox et du texte des pages ; recettes du wiki (WikiRecipes).

namespace CalamityFandom.Content.Legacy.Items.Potions
{
    /// <summary>
    /// Wiki : Tesla Potion.txt — aura électrique (celle du Tesla's Amulet de Calamity) : Electrified 3 s et Galvanic
    /// Corrosion 0,5 s aux ennemis touchés ; réduit la durée d'Electrified sur le joueur.
    /// </summary>
    public class TeslaPotion : LegacyBuffPotion
    {
        protected override int Buff => ModContent.BuffType<TeslaBuff>();
        protected override int Seconds => 480;
        protected override int Rarity => ItemRarityID.Orange;
        protected override (int, int) Size => (24, 26);
    }

    /// <summary>
    /// Wiki : Yharim_s Stimulants.txt — +6 % de dégâts, +2 % de critique, +5 % de vitesse de minage, +100 % de recul des
    /// sbires, +15 % de vitesse de déplacement, +6 % de réduction des dégâts, +8 défense ; 30 minutes.
    /// </summary>
    public class YharimsStimulants : LegacyBuffPotion
    {
        protected override int Buff => ModContent.BuffType<YharimPower>();
        protected override int Seconds => 1800;
        protected override int Rarity => ItemRarityID.Orange;
        protected override (int, int) Size => (40, 40);
    }
}

namespace CalamityFandom.Content.Legacy.Buffs.Potions
{
    public class TeslaBuff : LegacyPotionBuff
    {
        // Le drapeau tesla de Calamity (Tesla's Amulet) fait apparaître l'aura et réduit Static Discharge (l'Electrified du wiki)
        public override void Update(Player player, ref int buffIndex) => player.Calamity().tesla = true;
    }

    public class YharimPower : LegacyPotionBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage<GenericDamageClass>() += 0.06f;
            player.GetCritChance<GenericDamageClass>() += 2;
            player.pickSpeed -= 0.05f;
            player.GetKnockback<SummonDamageClass>() += 1f;
            player.moveSpeed += 0.15f;
            player.endurance += 0.06f;
            player.statDefense += 8;
        }
    }

    /// <summary>
    /// Aura de la Tesla Potion : valeurs de la page quand le joueur a le buff (Electrified = Static Discharge 3 s, Galvanic
    /// Corrosion 0,5 s) ; avec le seul Tesla's Amulet, rien ne change.
    /// </summary>
    public class TeslaPotionAura : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => entity.type == ModContent.ProjectileType<TeslaAura>();

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!Main.player[projectile.owner].HasBuff<TeslaBuff>())
                return;
            target.AddBuff(ModContent.BuffType<StaticDischarge>(), 180);
            target.AddBuff(ModContent.BuffType<GalvanicCorrosion>(), 30);
        }
    }
}

namespace CalamityFandom.Content.Legacy.Items.Weapons.Magic
{
    /// <summary>
    /// Wiki : Divine Retribution.txt — cinq lances divines surgissent d'en dessous vers le curseur, traversent les blocs,
    /// cherchent les ennemis (Holy Flames et Crush Depth 3 s) et reviennent vers le joueur si elles s'en éloignent trop.
    /// </summary>
    public class DivineRetribution : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 39;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 15;
            Item.width = 66;
            Item.height = 88;
            Item.useTime = Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.5f;
            Item.value = Item.sellPrice(gold: 24);
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.Calamity().donorItem = true;
            Item.UseSound = SoundID.Item73;
            Item.autoReuse = true;
            Item.shootSpeed = 19f;
            Item.shoot = ModContent.ProjectileType<DivineRetributionSpear>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 mouse = Main.MouseWorld;
            for (int i = 0; i < 5; i++)
            {
                Vector2 spawn = new(player.Center.X + Main.rand.Next(51) * -player.direction + (mouse.X - player.Center.X), player.MountedCenter.Y + 600f);
                spawn.X = (spawn.X + player.Center.X) / 2f + Main.rand.Next(-50, 51);
                spawn.Y += 100 * i;
                Vector2 toMouse = mouse - spawn;
                toMouse.Y = Math.Max(Math.Abs(toMouse.Y), 20f);
                Vector2 shot = toMouse.SafeNormalize(Vector2.UnitY) * Item.shootSpeed + Main.rand.NextVector2Square(-1.2f, 1.2f);
                shot.Y = -shot.Y;
                Projectile.NewProjectile(source, spawn, shot, type, damage, knockback, player.whoAmI, 0f, Main.rand.NextFloat() + 0.5f);
            }
            return false;
        }
    }
}

namespace CalamityFandom.Content.Legacy.Items.Weapons.Rogue
{
    /// <summary>
    /// Wiki : Hell_s Sun.txt — boule qui défie la gravité et traverse les blocs (autant que la pile, 10 au plus), s'arrête,
    /// puis lâche de petits soleils qui explosent (Holy Flames : 3 s par la boule, 2 s par les soleils) ; 20 coups au
    /// plus ; frappe furtive : soleils deux fois plus fréquents, boule qui dure et perce davantage ; clic droit : supprime
    /// toutes les boules.
    /// </summary>
    public class HellsSun : RogueWeapon
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 10;

        public override void SetDefaults()
        {
            Item.damage = 85;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.width = Item.height = 1;
            Item.useTime = Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5f;
            Item.value = Item.sellPrice(gold: 2, silver: 40);
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.maxStack = 10;
            Item.shootSpeed = 5f;
            Item.shoot = ModContent.ProjectileType<HellsSunProj>();
        }

        public override float StealthDamageMultiplier => 0.12f;

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            int balls = player.ownedProjectileCounts[ModContent.ProjectileType<HellsSunProj>()];
            return player.altFunctionUse == 2 ? balls > 0 : balls < Item.stack;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                foreach (var p in Main.ActiveProjectiles)
                    if (p.owner == player.whoAmI && p.type == type)
                        p.Kill();
                return false;
            }
            int ball = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (player.Calamity().StealthStrikeAvailable() && ball.WithinBounds(Main.maxProjectiles))
            {
                Projectile proj = Main.projectile[ball];
                proj.Calamity().stealthStrike = true;
                proj.penetrate = -1;
                proj.timeLeft = 2400;
            }
            return false;
        }
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.Magic
{
    /// <summary>Lance de la Divine Retribution : cherche les ennemis à 200 px, revient vers le joueur au-delà de 1000 px, explose.</summary>
    public class DivineRetributionSpear : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 7;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 420;
        }

        public override void AI()
        {
            Projectile.spriteDirection = Projectile.velocity.X < 0f ? -1 : 1;
            Projectile.rotation = Projectile.spriteDirection == -1 ? (-Projectile.velocity).ToRotation() : Projectile.velocity.ToRotation();
            Lighting.AddLight(Projectile.Center, 0.7f, 0.3f, 0f);
            Player owner = Main.player[Projectile.owner];
            if (!owner.active || owner.dead)
            {
                if (Projectile.timeLeft > 30)
                    Projectile.timeLeft = 30;
                return;
            }
            if (Projectile.Distance(owner.Center) > 1000f)
            {
                float inertia = 25f * Projectile.ai[1];
                Vector2 toOwner = Projectile.SafeDirectionTo(owner.Center, Vector2.UnitY);
                Projectile.velocity = (Projectile.velocity * (inertia - 1f) + toOwner * 5f * Projectile.ai[1]) / inertia;
                return;
            }
            CalamityUtils.HomeInOnNPC(Projectile, true, 200f, 9f, 20f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 180);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.ExpandHitboxBy(96);
            Projectile.maxPenetrate = Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();
            SoundEngine.PlaySound(SoundID.Item74, Projectile.Center);
            for (int i = 0; i < 6; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 244);
            for (int i = 0; i < 10; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 244);
                d.noGravity = true;
                d.velocity *= 3f;
                d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 244);
                d.noGravity = true;
                d.velocity *= 2f;
            }
        }
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.Rogue
{
    /// <summary>Boule de Hell's Sun : ralentit au bout de 70 images puis lâche un soleil toutes les secondes (demi-seconde en frappe furtive).</summary>
    public class HellsSunProj : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Legacy/Items/Weapons/Rogue/HellsSun";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 15;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 20;
            Projectile.timeLeft = 1200;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 40;
        }

        public override void AI()
        {
            if (++Projectile.ai[0] < 70f)
            {
                Projectile.rotation += 0.3f * Projectile.direction;
                return;
            }
            Projectile.velocity *= 0.96f;
            if (++Projectile.localAI[1] < (Projectile.Calamity().stealthStrike ? 30f : 60f))
                return;
            Projectile.localAI[1] = 0f;
            if (Projectile.owner != Main.myPlayer)
                return;
            Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.Next(10, 41) * 0.1f;
            if (Main.rand.NextBool(3))
                velocity *= 2f;
            velocity = (Projectile.velocity * 0.25f + velocity) * 0.8f;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center - velocity, velocity, ModContent.ProjectileType<ExplosiveSun>(),
                Projectile.damage, Projectile.knockBack, Projectile.owner, Main.rand.Next(3));
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
    }

    /// <summary>Petit soleil de Hell's Sun (trois tailles : ai[0]) : explose au premier coup ou au bout d'une seconde, à moitié des dégâts.</summary>
    public class ExplosiveSun : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.light = 0.5f;
            Projectile.alpha = 50;
            Projectile.scale = 1.2f;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
        }

        public override void AI() => Projectile.velocity *= 0.985f;

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] == 0f)
                return true;
            Texture2D texture = ModContent.Request<Texture2D>(Texture + (Projectile.ai[0] == 1f ? "2" : "3")).Value;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation,
                texture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<HolyFlames>(), 120);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<HolyFlames>(), 120);

        public override void OnKill(int timeLeft)
        {
            Projectile.ExpandHitboxBy(192);
            Projectile.maxPenetrate = Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.damage /= 2;
            Projectile.Damage();
            if (Main.rand.NextBool(3))
                SoundEngine.PlaySound(SoundID.Item14 with { Volume = SoundID.Item14.Volume * 0.5f }, Projectile.Center);
            for (int i = 0; i < 5; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 2f);
                d.velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    d.scale = 0.5f;
                    d.fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            for (int i = 0; i < 10; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 3f);
                d.noGravity = true;
                d.velocity *= 5f;
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 2f).velocity *= 2f;
            }
        }
    }
}
