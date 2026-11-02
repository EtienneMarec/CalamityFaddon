// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using System.Collections.Generic;
using CalamityFandom.Content.Legacy.Projectiles.Melee;
using CalamityFandom.Content.Legacy.Projectiles.Rogue;
using CalamityFandom.Content.Legacy.Projectiles.Typeless;
using CalamityMod;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

// Armes retirées de Calamity après 2.0.1.005 et documentées sur le wiki (02/11) : code de 2.0.1.005, valeurs des infobox
// (dégâts, critique, vitesse, rareté, prix, piles), recettes du wiki (WikiRecipes). Écarts au vieux code, d'après la page :
// Sticky Spiky Ball (Slimed 5 s, sept balles en frappe furtive), Bouncy Spiky Ball (frappe furtive : un seul ballon, mais un
// plus gros bonus à chaque ricochet ; [choix] +25 % au lieu de +10 %), Golden Gun (Ichor 8 s, absent du vieux code).

namespace CalamityFandom.Content.Legacy.Items.Weapons.Rogue
{
    /// <summary>
    /// Wiki : Monkey Darts.txt — fléchettes (Venom 3 s) ; frappe furtive : 3 fléchettes rapides qui rebondissent une fois.
    /// Vendues 4 argent par le Bandit après Plantera ; une chance sur deux de casser (vieux code : trois sur quatre).
    /// </summary>
    public class MonkeyDarts : RogueWeapon
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 99;

        public override void SetDefaults()
        {
            Item.damage = 150;
            Item.knockBack = 4f;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.consumable = true;
            Item.maxStack = 9999;
            Item.width = Item.height = 27;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.useTime = Item.useAnimation = 19;
            Item.value = Item.sellPrice(copper: 80);
            Item.shopCustomPrice = Item.buyPrice(silver: 4);
            Item.rare = ItemRarityID.Lime;
            Item.shootSpeed = 8f;
            Item.shoot = ModContent.ProjectileType<MonkeyDart>();
            Item.autoReuse = true;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
        }

        // 22 % de critique (4 de base + 18)
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 18;

        public override float StealthDamageMultiplier => 0.3333f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!player.Calamity().StealthStrikeAvailable())
                return true;
            for (int i = 0; i < 3; i++)
            {
                int p = Projectile.NewProjectile(source, position, (velocity * 1.25f).RotatedBy(MathHelper.ToRadians(7 - 7 * i)), type, damage, knockback, player.whoAmI, 1f);
                if (p.WithinBounds(Main.maxProjectiles))
                    Main.projectile[p].Calamity().stealthStrike = true;
            }
            return false;
        }
    }

    /// <summary>Le Bandit vend les Monkey Darts après Plantera (page).</summary>
    public class MonkeyDartsShop : GlobalNPC
    {
        public override void ModifyShop(NPCShop shop)
        {
            if (shop.NpcType == ModContent.NPCType<CalamityMod.NPCs.TownNPCs.Bandit>())
                shop.Add<MonkeyDarts>(Condition.DownedPlantera);
        }
    }

    /// <summary>
    /// Wiki : Sky Stabber.txt — boule à pointes qui défie la gravité ; 4 au plus (pile de 4) ; frappe furtive : pluie de
    /// plumes sur les ennemis touchés ; clic droit : supprime toutes les boules.
    /// </summary>
    public class SkyStabber : RogueWeapon
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 50;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.width = Item.height = 20;
            Item.useTime = Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 2f;
            Item.value = Item.sellPrice(silver: 20);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item1;
            Item.maxStack = 4;
            Item.shootSpeed = 2f;
            Item.shoot = ModContent.ProjectileType<SkyStabberProj>();
        }

        // 8 % de critique (4 de base + 4)
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 4;

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            int balls = player.ownedProjectileCounts[ModContent.ProjectileType<SkyStabberProj>()];
            return player.altFunctionUse == 2 ? balls > 0 : balls < Item.stack;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                // Clic droit : toutes les boules disparaissent
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

    /// <summary>Wiki : Sticky Spiky Ball.txt — boule qui colle à tout (Slimed 5 s) ; frappe furtive : sept à la fois, qui durent bien plus.</summary>
    public class StickySpikyBall : RogueWeapon
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 99;

        public override void SetDefaults()
        {
            Item.width = Item.height = 14;
            Item.damage = 10;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.useTime = Item.useAnimation = 19;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3f;
            Item.UseSound = SoundID.Item1;
            Item.value = Item.sellPrice(copper: 20);
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ModContent.ProjectileType<StickyBol>();
            Item.shootSpeed = 8f;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!player.Calamity().StealthStrikeAvailable())
                return true;
            int spread = 3;
            for (int i = 0; i < 7; i++)
            {
                Vector2 perturbed = (velocity + new Vector2(Main.rand.Next(-3, 4), Main.rand.Next(-3, 4))).RotatedBy(MathHelper.ToRadians(spread));
                int p = Projectile.NewProjectile(source, position, perturbed, type, damage, knockback, player.whoAmI);
                if (p.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[p].Calamity().stealthStrike = true;
                    Main.projectile[p].timeLeft *= 4;
                    Main.projectile[p].localNPCHitCooldown += 15;
                }
                spread -= Main.rand.Next(1, 4);
            }
            return false;
        }
    }

    /// <summary>
    /// Wiki : Bouncy Spiky Ball.txt — ballon qui ricoche sur les murs et les ennemis (vers l'ennemi le plus proche), un peu plus
    /// fort et rapide après chaque ennemi ; frappe furtive : un plus gros bonus.
    /// </summary>
    public class BouncySpikyBall : RogueWeapon
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 99;

        public override void SetDefaults()
        {
            Item.width = Item.height = 14;
            Item.damage = 10;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.useTime = Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 1f;
            Item.UseSound = SoundID.Item1;
            Item.value = Item.sellPrice(copper: 20);
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ModContent.ProjectileType<BouncyBol>();
            Item.shootSpeed = 8f;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (player.Calamity().StealthStrikeAvailable() && p.WithinBounds(Main.maxProjectiles))
                Main.projectile[p].Calamity().stealthStrike = true;
            return false;
        }
    }
}

namespace CalamityFandom.Content.Legacy.Items.Weapons.Melee
{
    /// <summary>Wiki : Verdant.txt — yoyo très agile qui lance des feuilles de cristal (80 % des dégâts) quand des ennemis sont proches.</summary>
    public class Verdant : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.Yoyo[Type] = true;
            ItemID.Sets.GamepadExtraRange[Type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[Type] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 30;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.damage = 86;
            Item.knockBack = 6f;
            Item.useTime = Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item1;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<VerdantYoyo>();
            Item.shootSpeed = 16f;
            Item.autoReuse = true;
            Item.value = Item.sellPrice(gold: 24);
            Item.rare = ModContent.RarityType<Turquoise>();
        }
    }
}

namespace CalamityFandom.Content.Legacy.Items.Weapons.Typeless
{
    /// <summary>Wiki : Golden Gun.txt — tire une ligne droite d'ichor (Ichor 8 s) ; sans classe de dégâts.</summary>
    public class GoldenGun : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 5;
            Item.width = 78;
            Item.height = 36;
            Item.useTime = Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = Item.sellPrice(silver: 80);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<GoldenGunProj>();
            Item.shootSpeed = 12f;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5f, 0f);
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.Rogue
{
    public class MonkeyDart : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.penetrate = 6;
            DrawOffsetX = -10;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 40;
            Projectile.extraUpdates = 2;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
        }

        private bool Stealth => Projectile.ai[0] == 1f;

        public override void AI()
        {
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);
            Projectile.velocity.Y = MathHelper.Min(Projectile.velocity.Y + 0.04f, 16f);
            if (Main.rand.NextBool(25))
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Venom, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 150, default, 0.9f);
                dust.position = Projectile.Center;
                dust.noLight = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Venom, 180);

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            for (int i = 0; i < 4; i++)
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Stone, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 100, default, 0.9f).position = Projectile.Center;
            // Une fléchette normale sur deux se ramasse (« 50% chance of breaking »)
            if (!Stealth && Projectile.owner == Main.myPlayer && Main.rand.NextBool(2))
                Item.NewItem(Projectile.GetSource_DropAsItem(), Projectile.Hitbox, ModContent.ItemType<Items.Weapons.Rogue.MonkeyDarts>());
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // Les fléchettes de frappe furtive rebondissent une fois
            if (!Stealth || Projectile.ai[1] >= 1f)
                return true;
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            Projectile.ai[1]++;
            return false;
        }
    }

    public class SkyStabberProj : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Legacy/Items/Weapons/Rogue/SkyStabber";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 15;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = 20;
            Projectile.timeLeft = 1200;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        public override void AI()
        {
            if (++Projectile.ai[0] >= 90f)
                Projectile.velocity *= 0.98f;
            else
                Projectile.rotation += 0.3f * Projectile.direction;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X * 0.6f;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y * 0.6f;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!Projectile.Calamity().stealthStrike || Projectile.owner != Main.myPlayer)
                return;
            for (int n = 0; n < 4; n++)
            {
                Projectile feather = CalamityUtils.ProjectileRain(Projectile.GetSource_FromThis(), target.Center, 400f, 100f, 500f, 800f, 20f,
                    ModContent.ProjectileType<StickyFeatherAero>(), (int)(Projectile.damage * 0.25f), Projectile.knockBack * 0.25f, Projectile.owner);
                if (feather.whoAmI.WithinBounds(Main.maxProjectiles))
                    feather.DamageType = ModContent.GetInstance<RogueDamageClass>();
            }
        }
    }

    /// <summary>
    /// Boule collante : se fixe à l'ennemi touché (et le touche à nouveau toutes les secondes) ou s'arrête contre un bloc.
    /// [technique] Les fonctions « collantes » de Calamity sont internes : comportement réécrit ici. ai[0] = 1 : fixée à
    /// l'ennemi ai[1] ; ai[0] = 2 : fixée à un bloc.
    /// </summary>
    public class StickyBol : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Legacy/Items/Weapons/Rogue/StickySpikyBall";

        private Vector2 offset;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.penetrate = 5;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(12))
                Dust.NewDust(Projectile.position + Vector2.One * 6f, Projectile.width - 12, Projectile.height - 12, DustID.TintableDust,
                    Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 175, new Color(0, 80, 255, 100), 1.2f);
            if (Projectile.ai[0] == 1f)
            {
                NPC target = Main.npc[(int)Projectile.ai[1]];
                if (!target.active)
                {
                    Projectile.ai[0] = 0f;
                    return;
                }
                Projectile.tileCollide = false;
                Projectile.Center = target.Center + offset;
                Projectile.velocity = Vector2.Zero;
                return;
            }
            if (Projectile.ai[0] == 2f)
            {
                Projectile.velocity = Vector2.Zero;
                return;
            }
            Projectile.velocity.Y = MathHelper.Min(Projectile.velocity.Y + 0.2f, 16f);
            Projectile.rotation += Projectile.velocity.X * 0.1f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.ai[0] = 2f;
            Projectile.velocity = Vector2.Zero;
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
                targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
            return projHitbox.Intersects(targetHitbox);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Slimed, 300);
            if (Projectile.ai[0] != 1f)
            {
                Projectile.ai[0] = 1f;
                Projectile.ai[1] = target.whoAmI;
                offset = Projectile.Center - target.Center;
                Projectile.netUpdate = true;
            }
        }
    }

    public class BouncyBol : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Legacy/Items/Weapons/Rogue/BouncySpikyBall";

        private readonly List<int> previousNPCs = new() { -1 };
        private bool hasHitEnemy;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.penetrate = 8;
            Projectile.timeLeft = 600;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.aiStyle = ProjAIStyleID.GroundProjectile;
        }

        public override void AI()
        {
            // Après un ennemi, il accélère doucement
            if (hasHitEnemy && Projectile.timeLeft < 575)
                Projectile.velocity = Vector2.Clamp(Projectile.velocity * 1.005f, new Vector2(-16f), new Vector2(16f));
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            previousNPCs.Add(target.whoAmI);
            NPC next = null;
            float minDist = 999f;
            foreach (var npc in Main.ActiveNPCs)
            {
                if (npc == target || previousNPCs.Contains(npc.whoAmI) || !npc.CanBeChasedBy(Projectile))
                    continue;
                float dist = Vector2.Distance(Projectile.Center, npc.Center);
                if (dist < minDist)
                {
                    minDist = dist;
                    next = npc;
                }
            }
            if (next == null)
                return;
            // Ricochet vers l'ennemi le plus proche : +10 % de dégâts (+25 % en frappe furtive)
            Projectile.damage = (int)(Projectile.damage * (Projectile.Calamity().stealthStrike ? 1.25f : 1.1f));
            hasHitEnemy = true;
            Projectile.velocity = (next.Center - Projectile.Center).SafeNormalize(Vector2.UnitX) * 15f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X * 1.001f;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y * 1.001f;
            return false;
        }
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.Melee
{
    public class VerdantYoyo : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Type] = -1f;
            ProjectileID.Sets.YoyosMaximumRange[Type] = 560f;
            ProjectileID.Sets.YoyosTopSpeed[Type] = 17f;
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = ProjAIStyleID.Yoyo;
            Projectile.width = Projectile.height = 16;
            Projectile.scale = 1.1f;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = -1;
            Projectile.MaxUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 9;
        }

        public override void AI()
        {
            Projectile.velocity *= 1.01f;
            if (Main.rand.NextBool(5))
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.CursedTorch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            CalamityUtils.MagnetSphereHitscan(Projectile, 300f, 12f, 54f, 5, ProjectileID.CrystalLeafShot, 0.8);
            if ((Projectile.position - Main.player[Projectile.owner].position).Length() > 3200f)
                Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.Typeless
{
    /// <summary>Trait d'ichor du Golden Gun (poussière, pas de sprite) : Ichor 8 s.</summary>
    public class GoldenGunProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.penetrate = 1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] < 5f)
            {
                Projectile.localAI[0]++;
                return;
            }
            for (int i = 0; i < 6; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position + new Vector2(6f), Projectile.width - 12, Projectile.height - 12, DustID.Ichor, 0f, 0f, 75, default, 1.2f);
                dust.noGravity = true;
                dust.position = Projectile.Center - Projectile.velocity / 6f * i;
                dust.velocity = Projectile.velocity * 0.1f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Ichor, 480);

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 30; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Ichor, 0f, 0f, 75, default, 1.2f);
                dust.velocity *= 2f;
                dust.noGravity = true;
            }
        }
    }
}
