// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using CalamityFandom.Content.Legacy.Projectiles.Melee;
using CalamityFandom.Content.Legacy.Projectiles.Rogue;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

// Lot du 13/11 (suite) : Plague Keeper, Poison Pack, The Enforcer. Code de 2.0.1.005, valeurs des infobox et du texte
// des pages ; recettes du wiki (WikiRecipes).

namespace CalamityFandom.Content.Legacy.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Plague Keeper.txt — chaque coup lance un nuage de peste qui traverse tout et sème 25 abeilles, puis ralentit
    /// et s'efface au bout d'une seconde (dégâts -40 %) ; trois abeilles au contact ; abeilles à un tiers des dégâts ;
    /// Plague 5 s au contact, 3 s par le nuage.
    /// </summary>
    public class PlagueKeeper : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 74;
            Item.height = 90;
            Item.damage = 80;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 19;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.knockBack = 6f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = Item.sellPrice(gold: 20);
            Item.rare = ItemRarityID.Red;
            Item.shoot = ModContent.ProjectileType<VirulentBeeWave>();
            Item.shootSpeed = 9f;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 300);
            if (player.whoAmI == Main.myPlayer)
                VirulentBeeWave.SpawnBees(player, player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, hit.SourceDamage, 3);
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo) =>
            target.AddBuff(ModContent.BuffType<Plague>(), 300);
    }

    /// <summary>
    /// Wiki : The Enforcer.txt — chaque coup fait apparaître des flammes d'essence près de l'ennemi, qui foncent sur les
    /// ennemis proches (25 % des dégâts, God Slayer Inferno 5 s).
    /// [choix] Cinq flammes (vieux code), autour de l'ennemi touché plutôt qu'au-dessus du curseur (« near the enemy »).
    /// </summary>
    public class TheEnforcer : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = Item.height = 100;
            Item.scale = 1.5f;
            Item.damage = 890;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.knockBack = 9f;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.value = Item.sellPrice(gold: 36);
            Item.rare = ModContent.RarityType<CosmicPurple>();
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) =>
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>(Texture + "Glow").Value);

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
                Dust.NewDust(hitbox.TopLeft(), hitbox.Width, hitbox.Height, 173);
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) => SpawnFlames(player, target.Center);

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo) => SpawnFlames(player, target.Center);

        private void SpawnFlames(Player player, Vector2 center)
        {
            SoundEngine.PlaySound(SoundID.Item73, player.Center);
            if (player.whoAmI != Main.myPlayer)
                return;
            int damage = player.CalcIntDamage<MeleeDamageClass>(0.25f * Item.damage);
            for (int i = 0; i < 5; i++)
            {
                Vector2 spawn = center + new Vector2(Main.rand.Next(-200, 201), -60f - 60f * i);
                Projectile.NewProjectile(player.GetSource_ItemUse(Item), spawn, Vector2.Zero, ModContent.ProjectileType<EssenceFlame2>(), damage, 0f, player.whoAmI);
            }
        }
    }
}

namespace CalamityFandom.Content.Legacy.Items.Weapons.Rogue
{
    /// <summary>
    /// Wiki : Poison Pack.txt — boules à pointes empoisonnées (Poisoned 4 s), autant que la pile (3 au plus) ; frappe furtive :
    /// la boule relâche des nuages de spores ; clic droit : supprime toutes les boules.
    /// </summary>
    public class PoisonPack : RogueWeapon
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 3;

        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.width = Item.height = 14;
            Item.useTime = Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 1.8f;
            Item.value = Item.sellPrice(silver: 6, copper: 60);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item1;
            Item.maxStack = 3;
            Item.shootSpeed = 7f;
            Item.shoot = ModContent.ProjectileType<PoisonBol>();
        }

        // 8 % de critique (4 de base + 4)
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 4;

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            int balls = player.ownedProjectileCounts[ModContent.ProjectileType<PoisonBol>()];
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
                Main.projectile[ball].Calamity().stealthStrike = true;
            return false;
        }
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.Melee
{
    /// <summary>Nuage de peste de la Plague Keeper (texture de la Virulence de Calamity).</summary>
    public class VirulentBeeWave : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/VirulentWave";

        private bool faded;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 70;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 3;
            Projectile.timeLeft = 600;
            Projectile.alpha = 100;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 8;
        }

        /// <summary>Abeilles du joueur (types et dégâts vanilla), au tiers des dégâts, en mêlée, sans percer.</summary>
        public static void SpawnBees(Player player, IEntitySource source, Vector2 center, Vector2 velocity, int baseDamage, int count, int spread = 0)
        {
            for (int i = 0; i < count; i++)
            {
                Vector2 pos = center + (spread > 0 ? new Vector2(Main.rand.Next(-spread, spread), Main.rand.Next(-spread, spread)) : Vector2.Zero);
                int bee = Projectile.NewProjectile(source, pos, velocity, player.beeType(), player.beeDamage(baseDamage / 3), player.beeKB(0f), player.whoAmI);
                if (bee.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[bee].DamageType = DamageClass.Melee;
                    Main.projectile[bee].penetrate = 1;
                }
            }
        }

        public override void AI()
        {
            Projectile.ai[1]++;
            Lighting.AddLight(Projectile.Center, 0.05f, 0.4f, 0f);
            if (Projectile.ai[1] < 60f)
            {
                Projectile.ai[0]++;
                if (Projectile.ai[0] > 8f)
                {
                    // Une abeille toutes les deux mises à jour : environ 25 (page)
                    if (Projectile.ai[0] % 2f == 0f && Projectile.owner == Main.myPlayer)
                        SpawnBees(Main.player[Projectile.owner], Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, Projectile.damage, 1, Projectile.width / 2);
                    float scalar = Projectile.ai[0] switch { 9f => 0.25f, 10f => 0.5f, 11f => 0.75f, _ => 1f };
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 89, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100);
                    if (Main.rand.NextBool(3))
                    {
                        dust.noGravity = true;
                        dust.scale *= 1.8f;
                        dust.velocity *= 2f;
                    }
                    else
                        dust.scale *= 1.3f;
                    dust.velocity *= 1.2f;
                    dust.scale *= scalar;
                }
            }
            else
            {
                // Au bout d'une seconde (60 mises à jour × 4) : ralentit, s'efface, dégâts -40 % (une fois, page)
                if (!faded)
                {
                    faded = true;
                    Projectile.damage = (int)(Projectile.damage * 0.6f);
                }
                Projectile.velocity *= 0.85f;
                Projectile.alpha += 5;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);
            if (++Projectile.frameCounter > 8)
            {
                Projectile.frame = (Projectile.frame + 1) % 4;
                Projectile.frameCounter = 0;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Plague>(), 180);

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }
    }

    /// <summary>Flamme d'essence de The Enforcer : apparaît, puis fonce sur l'ennemi le plus proche (texture de Calamity).</summary>
    public class EssenceFlame2 : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Healing/EssenceFlame";

        public override void SetStaticDefaults() => Main.projFrames[Type] = 4;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 180;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 3;
        }

        public override bool? CanHitNPC(NPC target) => Projectile.timeLeft < 150 && target.CanBeChasedBy(Projectile) ? null : false;

        public override void AI()
        {
            if (Projectile.alpha > 0)
                Projectile.alpha -= 5;
            if (++Projectile.frameCounter > 16)
            {
                Projectile.frame = (Projectile.frame + 1) % 4;
                Projectile.frameCounter = 0;
            }
            if (Projectile.timeLeft < 150)
                CalamityUtils.HomeInOnNPC(Projectile, true, 600f, 10f, 20f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 300);

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item74, Projectile.Center);
            for (int i = 0; i < 5; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.Center - new Vector2(25f), 50, 50, 173, 0f, 0f, 100, default, 2f);
                d.velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    d.scale = 0.5f;
                    d.fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            for (int i = 0; i < 8; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.Center - new Vector2(25f), 50, 50, 173, 0f, 0f, 100, default, 3f);
                d.noGravity = true;
                d.velocity *= 5f;
                Dust.NewDustDirect(Projectile.Center - new Vector2(25f), 50, 50, 173, 0f, 0f, 100, default, 2f).velocity *= 2f;
            }
        }
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.Rogue
{
    /// <summary>Boule du Poison Pack : roule et glisse au sol (comme une boule à pointes), Poisoned 4 s ; nuages de spores en frappe furtive.</summary>
    public class PoisonBol : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Legacy/Items/Weapons/Rogue/PoisonPack";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.penetrate = 5;
            Projectile.timeLeft = 1200;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.aiStyle = ProjAIStyleID.GroundProjectile;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 45;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(20))
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 44, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            if (Projectile.timeLeft < 20)
                Projectile.alpha += 13;
            if (!Projectile.Calamity().stealthStrike || Projectile.owner != Main.myPlayer || ++Projectile.localAI[1] % 35f != 0f)
                return;
            Vector2 velocity = CalamityUtils.RandomVelocity(100f, 10f, 20f);
            if (Main.rand.NextBool(3))
                velocity *= 2f;
            velocity = (velocity + Projectile.velocity * 0.25f) * 0.8f;
            int spore = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center - velocity, velocity, Main.rand.Next(ProjectileID.SporeGas, ProjectileID.SporeGas3 + 1),
                (int)(Projectile.damage * 0.75f), Projectile.knockBack, Projectile.owner);
            if (spore.WithinBounds(Main.maxProjectiles))
            {
                Main.projectile[spore].DamageType = ModContent.GetInstance<RogueDamageClass>();
                Main.projectile[spore].usesLocalNPCImmunity = true;
                Main.projectile[spore].localNPCHitCooldown = 45;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Poisoned, 240);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.Poisoned, 240);
    }
}
