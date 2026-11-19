using System;
using CalamityFandom.Content.Projectiles.Weapons1119k;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

// Lot N (19/11) : objets du wiki lâchés par des ennemis déjà dans le jeu (Plaguebringer Goliath, Clinger, Earth Elemental,
// Wall of Flesh, Clam, Providence, Yharon) : obtenables tout de suite par WikiLoot. Pages d'addon comprises (section 11).

namespace CalamityFandom.Content.Items.Weapons.Rogue
{
    /// <summary>
    /// Wiki : Infected Dagger Misses No Skeptics.txt — dague volante chercheuse qui perce à l'infini et rebondit vers un autre
    /// ennemi proche (Plague 2 s) ; frappe furtive : 3 dagues de plus, toutes plus rapides. Butin du Plaguebringer Goliath.
    /// </summary>
    public class InfectedDaggerMissesNoSkeptics : RogueWeapon
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 64;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.useTime = Item.useAnimation = 8;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = Item.noUseGraphic = true;
            Item.knockBack = 1.15f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = 48;
            Item.height = 20;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 10);
            Item.shoot = ModContent.ProjectileType<InfectedDagger>();
            Item.shootSpeed = 20f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool stealth = player.Calamity().StealthStrikeAvailable();
            int count = stealth ? 4 : 1;
            for (int i = 0; i < count; i++)
            {
                Vector2 v = stealth ? velocity.RotatedBy(MathHelper.Lerp(-0.3f, 0.3f, i / 3f)) * 1.4f : velocity;
                int p = Projectile.NewProjectile(source, position, v, type, damage, knockback, player.whoAmI);
                if (stealth && Main.projectile.IndexInRange(p))
                    Main.projectile[p].Calamity().stealthStrike = true;
            }
            return false;
        }
    }

    /// <summary>
    /// Wiki : Cursed Spear.txt — trident à pointe de flammes maudites qui vole loin avant de tomber, perce un ennemi (Cursed
    /// Inferno 2 s) ; frappe furtive : arrosé de boules de feu maudites. Butin rare des Clingers.
    /// </summary>
    public class CursedSpear : RogueWeapon
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 76;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.useTime = Item.useAnimation = 19;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = Item.noUseGraphic = true;
            Item.knockBack = 6f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = Item.height = 50;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 2, silver: 40);
            Item.shoot = ModContent.ProjectileType<CursedSpearProj>();
            Item.shootSpeed = 20f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (player.Calamity().StealthStrikeAvailable() && Main.projectile.IndexInRange(p))
                Main.projectile[p].Calamity().stealthStrike = true;
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Typeless
{
    /// <summary>
    /// Wiki : Varroa Hack.txt — arme sans classe consommable : s'accroche à l'ennemi touché et le blesse sans arrêt (Plague
    /// et Bleeding), puis explose (150 %) au bout d'un moment ou sur un bloc. Butin du Plaguebringer Goliath.
    /// [choix] Accrochée 3 s, un coup toutes les 15 images ; explosion de 80 px.
    /// </summary>
    public class VarroaHack : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 99;

        public override void SetDefaults()
        {
            Item.damage = 71;
            Item.DamageType = DamageClass.Default;
            Item.useTime = Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = Item.noUseGraphic = true;
            Item.consumable = true;
            Item.maxStack = Item.CommonMaxStack;
            Item.knockBack = 1f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = Item.height = 40;
            Item.rare = ItemRarityID.Lime;
            Item.value = Item.sellPrice(silver: 5);
            Item.shoot = ModContent.ProjectileType<VarroaHackProj>();
            Item.shootSpeed = 19f;
        }
    }

    /// <summary>
    /// Wiki : Earthen Clicker.txt — « clicker » : frappe à la position du curseur, à portée et en ligne de vue ; Armor
    /// Crunch 2 s. Butin de l'Earth Elemental. [technique] Pas de classe « clicker » (mod Clicker Class) : dégâts sans
    /// classe, portée de 30 blocs, un clic par utilisation.
    /// </summary>
    public class EarthenClicker : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 22;
            Item.DamageType = DamageClass.Default;
            Item.useTime = Item.useAnimation = 6;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.knockBack = 1f;
            Item.autoReuse = false;
            Item.UseSound = SoundID.MenuTick;
            Item.width = 16;
            Item.height = 22;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 7, silver: 20);
            Item.shoot = ModContent.ProjectileType<ClickHit>();
            Item.shootSpeed = 1f;
        }

        public override bool CanUseItem(Player player) => player.Distance(Main.MouseWorld) < 480f && Collision.CanHitLine(player.Center, 1, 1, Main.MouseWorld, 1, 1);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI);
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Ranged
{
    /// <summary>
    /// Wiki : Purgatory Blaster.txt — fléchette sacrée et 3 boules de feu qui courbent au hasard (Holy Flames 5 s). Butin de
    /// Providence. [choix] Utilise les fléchettes (« dart gun ») ; boules de feu à 50 %.
    /// </summary>
    public class PurgatoryBlaster : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 110;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item98;
            Item.width = 72;
            Item.height = 36;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.value = Item.sellPrice(platinum: 2);
            Item.shoot = ProjectileID.PoisonDartBlowgun;
            Item.shootSpeed = 32f;
            Item.useAmmo = AmmoID.Dart;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10f, 0f);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int dart = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (Main.projectile.IndexInRange(dart))
                Main.projectile[dart].GetGlobalProjectile<HolyFlamesShot>().Active = true;
            for (int i = 0; i < 3; i++)
                Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.4f) * 0.4f, ModContent.ProjectileType<CurvingFireball>(), damage / 2, knockback, player.whoAmI,
                    Main.rand.NextFloat(-0.04f, 0.04f));
            return false;
        }
    }

    /// <summary>Fléchettes du Purgatory Blaster : Holy Flames 5 s.</summary>
    public class HolyFlamesShot : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public bool Active;

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Active)
                target.AddBuff(ModContent.BuffType<HolyFlames>(), 300);
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Magic
{
    /// <summary>
    /// Wiki : Dragon Flame Barrage.txt — pluie de flammes sacrées en colonne verticale (au curseur), qui explosent après un
    /// court moment ; Holy Flames 5 s. Butin de Yharon. [choix] 8 flammes depuis le haut de l'écran, explosion après 30 images.
    /// </summary>
    public class DragonFlameBarrage : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 150;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 20;
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item88;
            Item.width = 80;
            Item.height = 84;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
            Item.value = Item.sellPrice(gold: 30);
            Item.shoot = ModContent.ProjectileType<HolyBarrageFlame>();
            Item.shootSpeed = 30f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2 spawn = new(Main.MouseWorld.X + Main.rand.Next(-40, 41), Main.screenPosition.Y - 40f - i * 50f);
                Projectile.NewProjectile(source, spawn, new Vector2(0f, 24f), type, damage, knockback, player.whoAmI, Main.MouseWorld.Y);
            }
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Accessories
{
    /// <summary>Wiki : Nightform.txt — +25 % de mana maximum la nuit. Butin du Wall of Flesh.</summary>
    public class Nightform : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = Item.height = 50;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.sellPrice(gold: 2);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!Main.dayTime)
                player.statManaMax2 += player.statManaMax2 / 4;
        }
    }

    /// <summary>
    /// Wiki : Ghast Relic.txt — quand un sbire touche un ennemi, 10 % de chances de faire naître un fantôme qui poursuit les
    /// ennemis et les traverse (20 dégâts par seconde). Butin des Clams. [choix] Fantôme de 5 s, au plus 3 à la fois.
    /// </summary>
    public class GhastRelic : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 22;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override void UpdateAccessory(Player player, bool hideVisual) => player.GetModPlayer<GhastRelicPlayer>().Equipped = true;
    }

    public class GhastRelicPlayer : ModPlayer
    {
        public bool Equipped;

        public override void ResetEffects() => Equipped = false;

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!Equipped || Player.whoAmI != Main.myPlayer || !(proj.minion || ProjectileID.Sets.MinionShot[proj.type]) || !Main.rand.NextBool(10)
                || Player.ownedProjectileCounts[ModContent.ProjectileType<RelicGhast>()] >= 3)
                return;
            Projectile.NewProjectile(Player.GetSource_Accessory(Player.HeldItem), target.Center, Main.rand.NextVector2Circular(3f, 3f), ModContent.ProjectileType<RelicGhast>(), 20, 0f, Player.whoAmI);
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Weapons1119k
{
    /// <summary>Dague infectée : chercheuse, perce à l'infini, repart vers un autre ennemi après chaque coup (Plague 2 s).</summary>
    public class InfectedDagger : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Rogue/InfectedDaggerMissesNoSkeptics";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.scale = 0.7f;
        }

        public override void AI()
        {
            CalamityUtils.HomeInOnNPC(Projectile, true, 600f, Projectile.velocity.Length(), 12f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 120);
            NPC next = null;
            float best = 500f;
            foreach (NPC n in Main.ActiveNPCs)
                if (n.whoAmI != target.whoAmI && n.CanBeChasedBy(Projectile) && n.Distance(Projectile.Center) < best)
                {
                    best = n.Distance(Projectile.Center);
                    next = n;
                }
            if (next != null)
            {
                Projectile.velocity = Projectile.SafeDirectionTo(next.Center) * Projectile.velocity.Length();
                Projectile.netUpdate = true;
            }
        }
    }

    /// <summary>Lance maudite : vole droit 30 images puis tombe, perce un ennemi (Cursed Inferno 2 s) ; furtive : boules de feu maudites.</summary>
    public class CursedSpearProj : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Rogue/CursedSpear";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = 2;
            Projectile.timeLeft = 240;
        }

        public override void AI()
        {
            if (++Projectile.ai[0] > 30f)
                Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.3f, 16f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            if (!Main.dedServ && Main.rand.NextBool(2))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.CursedTorch, 0f, 0f, 100, default, 1.2f).noGravity = true;
            if (Projectile.Calamity().stealthStrike && Projectile.owner == Main.myPlayer && Projectile.ai[0] % 6f == 0f)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Circular(4f, 4f) + Vector2.UnitY * 2f, ProjectileID.CursedFlameFriendly,
                    Projectile.damage / 3, 0f, Projectile.owner);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.CursedInferno, 120);
    }

    /// <summary>Varroa Hack : s'accroche à l'ennemi touché 3 s (un coup toutes les 15 images, Plague et Bleeding), puis explose à 150 %.</summary>
    public class VarroaHackProj : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Typeless/VarroaHack";

        private int stuckTo = -1;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Default;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.scale = 0.6f;
        }

        public override void AI()
        {
            if (stuckTo >= 0)
            {
                NPC n = Main.npc[stuckTo];
                if (!n.active)
                {
                    Projectile.Kill();
                    return;
                }
                Projectile.Center = n.Center + new Vector2(Projectile.localAI[0], Projectile.localAI[1]);
                Projectile.velocity = Vector2.Zero;
                return;
            }
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.2f, 14f);
            Projectile.rotation += 0.25f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 120);
            target.AddBuff(BuffID.Bleeding, 120);
            if (stuckTo >= 0)
                return;
            stuckTo = target.whoAmI;
            Projectile.localAI[0] = Projectile.Center.X - target.Center.X;
            Projectile.localAI[1] = Projectile.Center.Y - target.Center.Y;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 180;
            Projectile.netUpdate = true;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14 with { Volume = 0.5f }, Projectile.Center);
            Projectile.ExpandHitboxBy(80);
            Projectile.maxPenetrate = Projectile.penetrate = -1;
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;
            for (int i = 0; i < Projectile.localNPCImmunity.Length; i++)
                Projectile.localNPCImmunity[i] = 0;
            Projectile.damage = (int)(Projectile.damage * 1.5f);
            Projectile.Damage();
            if (!Main.dedServ)
                for (int i = 0; i < 15; i++)
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GreenFairy, 0f, 0f, 100, default, 1.4f).velocity *= 2f;
        }
    }

    /// <summary>Clic de l'Earthen Clicker : petit coup à la position du curseur (Armor Crunch 2 s).</summary>
    public class ClickHit : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Default;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 2;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (!Main.dedServ)
                for (int i = 0; i < 6; i++)
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Dirt, 0f, 0f, 100, default, 1.1f).velocity *= 1.5f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 120);
    }

    /// <summary>Boule de feu du Purgatory Blaster : courbe d'un côté au hasard (ai[0]), Holy Flames 5 s.</summary>
    public class CurvingFireball : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.ai[0]);
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.SolarFlare, Vector2.Zero, 100, default, 1.4f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<HolyFlames>(), 300);
    }

    /// <summary>Flamme sacrée du Dragon Flame Barrage : tombe jusqu'à la hauteur du curseur (ai[0]), s'arrête, explose 30 images plus tard.</summary>
    public class HolyBarrageFlame : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 180;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f && Projectile.Center.Y >= Projectile.ai[0])
            {
                Projectile.localAI[0] = 1f;
                Projectile.velocity = Vector2.Zero;
                Projectile.timeLeft = 30;
            }
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.SolarFlare, Vector2.Zero, 100, default, 1.6f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<HolyFlames>(), 300);

        public override void OnKill(int timeLeft)
        {
            if (Projectile.localAI[0] == 0f)
                return;
            SoundEngine.PlaySound(SoundID.Item14 with { Volume = 0.5f }, Projectile.Center);
            Projectile.ExpandHitboxBy(90);
            Projectile.localNPCHitCooldown = -1;
            for (int i = 0; i < Projectile.localNPCImmunity.Length; i++)
                Projectile.localNPCImmunity[i] = 0;
            Projectile.Damage();
            if (!Main.dedServ)
                for (int i = 0; i < 20; i++)
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SolarFlare, 0f, 0f, 100, default, 1.6f).velocity *= 2f;
        }
    }

    /// <summary>Fantôme de la Ghast Relic : 5 s, poursuit les ennemis et les traverse (20 dégâts par seconde).</summary>
    public class RelicGhast : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.SpectreWrath;

        public override void SetStaticDefaults() => ProjectileID.Sets.MinionShot[Type] = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
        }

        public override void AI()
        {
            CalamityUtils.HomeInOnNPC(Projectile, true, 600f, 6f, 20f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.DungeonSpirit, Vector2.Zero, 120, default, 1f).noGravity = true;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White * 0.6f;
    }
}
