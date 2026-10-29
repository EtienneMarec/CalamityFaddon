// Adapté de Calamity Mod (objets retirés ou remplacés en 2022), © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using System;
using CalamityFandom.Content.Legacy.Buffs.Potions;
using CalamityFandom.Content.Legacy.Projectiles.Ranged;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

// Objets retirés de Calamity (ou remplacés par un objet que le wiki connaît aussi : ils coexistent) : dans les deux
// modes. La Marnite Bayonet suit la règle des remplacements (glossary.json -> replacements). Recettes du wiki.

namespace CalamityFandom.Content.Legacy.Items.Weapons.Melee
{
    /// <summary>Wiki : Wulfrum Blade.txt — épée de Wulfrum (coup automatique). Coexiste avec le Wulfrum Screwdriver.</summary>
    public class WulfrumBlade : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 46;
            Item.damage = 12;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 19;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.knockBack = 3.75f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = Item.sellPrice(silver: 20);
            Item.rare = ItemRarityID.Blue;
        }
    }
}

namespace CalamityFandom.Content.Legacy.Items.Weapons.Ranged
{
    /// <summary>Wiki : Wulfrum Bow.txt — arc de Wulfrum (11 dégâts, page). Coexiste avec le Wulfrum Blunderbuss.</summary>
    public class WulfrumBow : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 11;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 27;
            Item.height = 38;
            Item.useTime = Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.25f;
            Item.value = Item.sellPrice(silver: 20);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.Arrow;
        }
    }

    /// <summary>
    /// Wiki : Marnite Bayonet.txt — fusil dont le canon blesse aussi les ennemis qui le touchent (dégâts à distance, hitbox
    /// fixe). Remplacée par le Marnite Repulsion Shield (sans page) : portage en Full Fandom, bouclier renommé en Calamity++.
    /// </summary>
    public class MarniteBayonet : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 72;
            Item.height = 20;
            Item.useTime = Item.useAnimation = 28;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2.25f;
            Item.value = Item.sellPrice(silver: 20);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item41;
            Item.autoReuse = true;
            Item.shootSpeed = 22f;
            Item.useAmmo = AmmoID.Bullet;
            Item.shoot = ProjectileID.PurificationPowder;
        }
    }

    /// <summary>
    /// Wiki : Charged Dart Blaster.txt — 2 à 4 fléchettes en gerbe (moitié des dégâts) et un trait d'énergie qui se divise
    /// (projectile de Calamity) ; clic droit : trait explosif à double dégâts qui rebondit 3 fois (explosions de Plague 4 s).
    /// </summary>
    public class ChargedDartBlaster : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 100;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 74;
            Item.height = 40;
            Item.useTime = Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7f;
            Item.value = Item.sellPrice(gold: 20);
            Item.rare = ItemRarityID.Red;
            Item.UseSound = CommonCalamitySounds.LaserCannonSound;
            Item.autoReuse = true;
            Item.shootSpeed = 22f;
            Item.shoot = ModContent.ProjectileType<ChargedBlast>();
            Item.useAmmo = AmmoID.Dart;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<ChargedBlast3>(), damage * 2, knockback, player.whoAmI);
                return false;
            }
            int darts = Main.rand.Next(2, 5);
            for (int i = 0; i < darts; i++)
                Projectile.NewProjectile(source, position, velocity + new Vector2(Main.rand.Next(-40, 41), Main.rand.Next(-40, 41)) * 0.05f, type, damage / 2, knockback, player.whoAmI);
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<ChargedBlast>(), damage, knockback, player.whoAmI);
            return false;
        }
    }
}

namespace CalamityFandom.Content.Legacy.Items.Weapons.Magic
{
    /// <summary>Wiki : Wulfrum Staff.txt — bâton : trait de Wulfrum (projectile de Calamity). Coexiste avec la Wulfrum Prosthesis.</summary>
    public class WulfrumStaff : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 13;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 2;
            Item.width = 44;
            Item.height = 46;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.value = Item.sellPrice(silver: 20);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item43;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<WulfrumBolt>();
            Item.shootSpeed = 9f;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-3, 0);
    }

    /// <summary>Wiki : Lazinator.txt — rafale de lasers violets ou verts (vanilla) qui traversent deux ennemis.</summary>
    public class Lazinator : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 55;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 4;
            Item.width = 46;
            Item.height = 22;
            Item.useTime = Item.useAnimation = 7;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = Item.sellPrice(gold: 9, silver: 60);
            Item.rare = ItemRarityID.LightPurple;
            Item.UseSound = SoundID.Item12;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurpleLaser;
            Item.shootSpeed = 20f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool green = Main.rand.NextBool(2);
            // « Green lasers are very slightly shorter and slower »
            var laser = Projectile.NewProjectileDirect(source, position, green ? velocity * 0.95f : velocity, green ? ProjectileID.GreenLaser : ProjectileID.PurpleLaser, damage, knockback, player.whoAmI);
            laser.DamageType = DamageClass.Magic;
            laser.penetrate = 3;
            laser.usesLocalNPCImmunity = true;
            laser.localNPCHitCooldown = 10;
            return false;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);
    }
}

namespace CalamityFandom.Content.Legacy.Items.Ammo
{
    /// <summary>Wiki : Acceleration Round.txt — balle qui accélère avec le temps.</summary>
    public class AccelerationRound : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 99;

        public override void SetDefaults()
        {
            Item.damage = 11;
            Item.DamageType = DamageClass.Ranged;
            Item.width = Item.height = 8;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.knockBack = 1.25f;
            Item.value = Item.sellPrice(copper: 50);
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ModContent.ProjectileType<AccelerationRoundProj>();
            Item.shootSpeed = 1f;
            Item.ammo = AmmoID.Bullet;
        }
    }
}

namespace CalamityFandom.Content.Legacy.Items.Potions
{
    /// <summary>
    /// Wiki : Cadance Potion.txt — buff Cadance 8 min : régénération +5, vie max +25 %, portée de ramassage des cœurs
    /// accrue, apparition des ennemis -56 % (et nombre max -44 %), Regeneration et Lifeforce désactivés.
    /// </summary>
    public class CadancePotion : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 20;

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 38;
            Item.useTurn = true;
            Item.maxStack = 9999;
            Item.useTime = Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.UseSound = SoundID.Item3;
            Item.rare = ItemRarityID.LightRed;
            Item.consumable = true;
            Item.buffType = ModContent.BuffType<Cadance>();
            Item.buffTime = 480 * 60;
            Item.value = Item.sellPrice(silver: 40);
        }
    }

    /// <summary>Wiki : Crumbling Potion.txt — buff Armor Crumbling 8 min : +5 % critique mêlée et spy, Armor Crunch 2, 4 ou 6 s.</summary>
    public class CrumblingPotion : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 20;

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.useTurn = true;
            Item.maxStack = 9999;
            Item.useTime = Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.UseSound = SoundID.Item3;
            Item.value = Item.sellPrice(silver: 40);
            Item.rare = ItemRarityID.LightRed;
            Item.consumable = true;
            Item.buffType = ModContent.BuffType<ArmorCrumbling>();
            Item.buffTime = 480 * 60;
        }
    }

    /// <summary>
    /// Wiki : Shattering Potion.txt — buff Armor Shattering 8 min : +8 % dégâts et critique mêlée et spy, Armor Crunch 2, 4
    /// ou 6 s ; désactive l'Armor Crumbling.
    /// </summary>
    public class ShatteringPotion : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 20;

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.useTurn = true;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.Yellow;
            Item.useTime = Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.UseSound = SoundID.Item3;
            Item.consumable = true;
            Item.buffType = ModContent.BuffType<ArmorShattering>();
            Item.buffTime = 480 * 60;
            Item.value = Item.sellPrice(silver: 40);
        }
    }
}

namespace CalamityFandom.Content.Legacy.Items.Accessories
{
    /// <summary>
    /// Wiki : Frigid Bulwark.txt — covering (Paladin's Shield + Frozen Turtle Shell) : +13 défense, immunité au recul ;
    /// au-dessus de 25 % de vie, absorbe 25 % des dégâts des alliés (Paladin's Shield vanilla) ; sous 50 % de vie, Ice
    /// Barrier (-15 %), sous 15 %, -20 % au total. Recette de 2022 ajoutée à notre wiki (la page n'en montre pas).
    /// </summary>
    public class FrigidBulwark : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 40;
            Item.value = Item.sellPrice(gold: 9, silver: 60);
            Item.rare = ItemRarityID.Lime;
            Item.defense = 13;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.noKnockback = true;
            if (player.statLife > player.statLifeMax2 * 0.25f)
            {
                player.hasPaladinShield = true;
                if (player.whoAmI != Main.myPlayer && player.miscCounter % 10 == 0 && Main.LocalPlayer.team == player.team && player.team != 0)
                {
                    if (Main.LocalPlayer.Distance(player.Center) < 800f)
                        Main.LocalPlayer.AddBuff(BuffID.PaladinsShield, 20);
                }
            }
            if (player.statLife <= player.statLifeMax2 * 0.5f)
                player.AddBuff(BuffID.IceBarrier, 5);
            if (player.statLife <= player.statLifeMax2 * 0.15f)
                player.endurance += 0.05f;
        }
    }
}

namespace CalamityFandom.Content.Legacy.Buffs.Potions
{
    /// <summary>Cadance (Cadance Potion) : voir la potion ; effets dans LegacyPotionBuffPlayer.</summary>
    public class Cadance : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            var p = player.GetModPlayer<LegacyPotionBuffPlayer>();
            p.Cadance = true;
            player.lifeMagnet = true;
            player.buffImmune[BuffID.Regeneration] = true;
            player.buffImmune[BuffID.Lifeforce] = true;
        }
    }

    /// <summary>Armor Crumbling (Crumbling Potion) : +5 % critique mêlée et spy, attaques mêlée et spy : Armor Crunch.</summary>
    public class ArmorCrumbling : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            if (player.HasBuff(ModContent.BuffType<ArmorShattering>()))
                return;
            player.GetModPlayer<LegacyPotionBuffPlayer>().ArmorBreak = true;
            player.GetCritChance<MeleeDamageClass>() += 5;
            player.GetCritChance<RogueDamageClass>() += 5;
        }
    }

    /// <summary>Armor Shattering (Shattering Potion) : +8 % dégâts et critique mêlée et spy, attaques mêlée et spy : Armor Crunch.</summary>
    public class ArmorShattering : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<LegacyPotionBuffPlayer>().ArmorBreak = true;
            player.GetDamage<MeleeDamageClass>() += 0.08f;
            player.GetDamage<RogueDamageClass>() += 0.08f;
            player.GetCritChance<MeleeDamageClass>() += 8;
            player.GetCritChance<RogueDamageClass>() += 8;
        }
    }

    public class LegacyPotionBuffPlayer : ModPlayer
    {
        public bool Cadance;
        public bool ArmorBreak;

        public override void ResetEffects() => Cadance = ArmorBreak = false;

        public override void PostUpdateEquips()
        {
            if (Cadance)
                Player.statLifeMax2 += Player.statLifeMax / 4;
        }

        public override void UpdateLifeRegen()
        {
            if (Cadance)
                Player.lifeRegen += 5;
        }

        private static bool MeleeOrSpy(DamageClass damageClass)
            => damageClass.CountsAsClass<MeleeDamageClass>() || damageClass.CountsAsClass<RogueDamageClass>();

        // « 2, 4 or 6 seconds » (infobox) : une des trois durées au hasard
        private static void Crunch(NPC target) => target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 120 * Main.rand.Next(1, 4));

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (ArmorBreak && MeleeOrSpy(item.DamageType))
                Crunch(target);
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (ArmorBreak && MeleeOrSpy(proj.DamageType))
                Crunch(target);
        }
    }

    /// <summary>Apparition des ennemis réduite sous Cadance (-56 %, nombre max -44 %).</summary>
    public class CadanceSpawnRate : GlobalNPC
    {
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            if (!player.GetModPlayer<LegacyPotionBuffPlayer>().Cadance)
                return;
            spawnRate = (int)(spawnRate / 0.44f);
            maxSpawns = (int)(maxSpawns * 0.56f);
        }
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.Ranged
{
    /// <summary>Balle de l'Acceleration Round : accélère de 1 % par mise à jour ; à 12, elle repart à moitié avec une mise à jour de plus.</summary>
    public class AccelerationRoundProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 3;
            AIType = ProjectileID.Bullet;
        }

        public override void AI()
        {
            Projectile.velocity *= 1.01f;
            if (Projectile.velocity.Length() >= 12f && Projectile.extraUpdates < 20)
            {
                Projectile.velocity *= 0.5f;
                Projectile.extraUpdates++;
                Projectile.netUpdate = true;
            }
            if (++Projectile.localAI[0] > 4f && Main.rand.NextBool(3))
            {
                var dust = Dust.NewDustDirect(Projectile.position, 1, 1, 91, 0f, 0f, Projectile.alpha, default, 0.5f);
                dust.velocity = Vector2.Zero;
                dust.noGravity = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesFromEdge(Projectile, 0, lightColor);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int k = 0; k < 5; k++)
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 91, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
        }
    }

    /// <summary>Trait explosif du Charged Dart Blaster (clic droit) : rebondit 3 fois, explose à chaque rebond, ennemi et à sa fin.</summary>
    public class ChargedBlast3 : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/LaserProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 5;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 4;
            Projectile.extraUpdates = 3;
            Projectile.alpha = 255;
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
        }

        public override void AI()
        {
            Projectile.alpha = Math.Max(0, Projectile.alpha - 25);
            Lighting.AddLight(Projectile.Center, 0f, 0.3f, 0.7f);
            if (Projectile.ai[1] == 0f)
                Projectile.localAI[0] = Math.Min(50f, Projectile.localAI[0] + 1.5f);
            else if ((Projectile.localAI[0] -= 1.5f) <= 0f)
                Projectile.Kill();
        }

        public override Color? GetAlpha(Color lightColor) => new Color(100, 100, 255, 0);

        public override bool PreDraw(ref Color lightColor) => Projectile.DrawBeam(100f, 3f, lightColor);

        private void Boom(float big)
        {
            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ChargedDartExplosion>(), Projectile.damage / 4, Projectile.knockBack, Projectile.owner, 0f, big);
            SoundEngine.PlaySound(SoundID.Item62, Projectile.position);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Boom(0f);
            if (--Projectile.penetrate <= 0)
                return true;
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }

        public override void OnKill(int timeLeft) => Boom(1f);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => Boom(0f);
    }

    /// <summary>Explosion du Charged Dart Blaster (50 px, 100 px à la fin du trait), Plague 4 s.</summary>
    public class ChargedDartExplosion : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 5;
        }

        public override void AI()
        {
            if (Projectile.timeLeft != 5)
                return;
            bool big = Projectile.ai[1] != 0f;
            if (big)
                Projectile.Resize(100, 100);
            for (int i = 0; i < (big ? 10 : 5); i++)
            {
                var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 160, 0f, 0f, 100, default, 2f);
                dust.velocity *= 3f;
            }
            for (int i = 0; i < (big ? 20 : 10); i++)
            {
                var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 160, 0f, 0f, 100, default, 3f);
                dust.noGravity = true;
                dust.velocity *= 5f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Plague>(), 240);
    }
}
