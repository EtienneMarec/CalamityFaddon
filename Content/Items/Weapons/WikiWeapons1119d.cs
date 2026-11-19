using System;
using CalamityFandom.Content.Projectiles.Weapons1119d;
using CalamityMod;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

// Butin des ennemis générés du wiki (19/11) : sprites et valeurs des pages ; ils tombent par WikiLoot.

namespace CalamityFandom.Content.Items.Weapons.Rogue
{
    /// <summary>
    /// Wiki : Creature Capsule.txt — capsule qui explose et libère un fantôme qui tire sur les ennemis proches ; un coup
    /// critique de la capsule tue net un ennemi qui n'est pas un boss ; frappe furtive : 5 fantômes, +25 % de critique.
    /// Butin du Glitch.
    /// </summary>
    public class CreatureCapsule : RogueWeapon
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 350;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = Item.noUseGraphic = true;
            Item.knockBack = 4f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = Item.height = 34;
            Item.rare = ModContent.RarityType<CosmicPurple>();
            Item.value = Item.sellPrice(gold: 15, silver: 30);
            Item.shoot = ModContent.ProjectileType<CapsuleProj>();
            Item.shootSpeed = 12f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (player.Calamity().StealthStrikeAvailable() && Main.projectile.IndexInRange(p))
            {
                Main.projectile[p].Calamity().stealthStrike = true;
                Main.projectile[p].CritChance += 25;
            }
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Guardians Wrath.txt — lance ; clic droit : lancer le trident, qui se plante dans un bloc ou un ennemi ;
    /// clic droit encore : un éclair le frappe (dégâts énormes autour) et il revient. Butin du Cuboid Curse.
    /// [choix] Éclair = x5 dégâts sur 160 px ; le trident ne quitte pas l'inventaire (il ne s'utilise plus en attendant).
    /// </summary>
    public class GuardiansWrath : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.Spears[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 346;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = Item.noUseGraphic = true;
            Item.knockBack = 10f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = Item.height = 50;
            Item.rare = ModContent.RarityType<CosmicPurple>();
            Item.value = Item.sellPrice(gold: 20);
            Item.shoot = ModContent.ProjectileType<GuardiansWrathSpear>();
            Item.shootSpeed = 20f;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            int thrown = ModContent.ProjectileType<GuardiansTrident>();
            foreach (var p in Main.ActiveProjectiles)
                if (p.owner == player.whoAmI && p.type == thrown)
                {
                    // Trident lancé : clic droit = éclair, sinon rien
                    if (player.altFunctionUse == 2 && player.whoAmI == Main.myPlayer && p.ai[0] != 0f)
                        ((GuardiansTrident)p.ModProjectile).Strike();
                    return false;
                }
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<GuardiansTrident>(), damage, knockback, player.whoAmI);
                return false;
            }
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Magic
{
    /// <summary>
    /// Wiki : Hailberd.txt — 4 ou 5 hallebardes en cercle autour du curseur, pointées vers lui, qui accélèrent et traversent
    /// les blocs. Butin du Frigidemon.
    /// </summary>
    public class Hailberd : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 980;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 30;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 10f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item28;
            Item.width = 56;
            Item.height = 50;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
            Item.value = Item.sellPrice(gold: 20);
            Item.shoot = ModContent.ProjectileType<HailberdProjectile>();
            Item.shootSpeed = 15f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 mouse = Main.MouseWorld;
            int count = Main.rand.Next(4, 6);
            float start = Main.rand.NextFloat(MathHelper.TwoPi);
            for (int i = 0; i < count; i++)
            {
                Vector2 spawn = mouse + (start + MathHelper.TwoPi * i / count).ToRotationVector2() * 220f;
                Projectile.NewProjectile(source, spawn, (mouse - spawn).SafeNormalize(Vector2.UnitY) * 2f, type, damage, knockback, player.whoAmI);
            }
            return false;
        }
    }

    /// <summary>
    /// Wiki : Pizza Wheel.txt — des pizzas roulent sur l'écran depuis la gauche et la droite, en ligne droite, sans gravité
    /// et à travers les blocs. Butin de l'Evil Animatronic. [choix] Une pizza de chaque côté, à la hauteur du curseur.
    /// </summary>
    public class PizzaWheel : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 300;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 9;
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.knockBack = 0.5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item8;
            Item.width = Item.height = 34;
            Item.rare = ModContent.RarityType<CosmicPurple>();
            Item.value = Item.sellPrice(gold: 20);
            Item.shoot = ModContent.ProjectileType<RollingPizza>();
            Item.shootSpeed = 30f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float y = Main.MouseWorld.Y + Main.rand.Next(-40, 41);
            for (int side = -1; side <= 1; side += 2)
            {
                Vector2 spawn = new(player.Center.X + side * (Main.screenWidth / 2f + 40f), y);
                Projectile.NewProjectile(source, spawn, new Vector2(-side * 14f, 0f), type, damage, knockback, player.whoAmI);
            }
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Summon
{
    /// <summary>
    /// Wiki : Sign of the Operator.txt — fouet unique : une page flottante devant le joueur lâche sans arrêt de courtes
    /// tentacules-fouets ; Operator Sickness 3 s. Butin du Tall Man.
    /// [choix] La page dure le temps de l'utilisation et lâche une tentacule toutes les 6 images vers le curseur ;
    /// « Operator Sickness » n'existe pas encore : Ichor 3 s à la place.
    /// </summary>
    public class SignoftheOperator : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 140;
            Item.DamageType = DamageClass.SummonMeleeSpeed;
            Item.useTime = Item.useAnimation = 28;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.knockBack = 12f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item8;
            Item.width = 28;
            Item.height = 34;
            Item.rare = ModContent.RarityType<CosmicPurple>();
            Item.value = Item.sellPrice(gold: 18);
            Item.shoot = ModContent.ProjectileType<OperatorPage>();
            Item.shootSpeed = 1f;
            Item.channel = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;
    }
}

namespace CalamityFandom.Content.Items.PermanentBoosters
{
    /// <summary>
    /// Wiki : Elder Berry.txt — donne pour toujours un dash, lancé par une touche (« Elder Berry Dash »). Butin en
    /// Revengeance (Yggdrasil Ent...). [choix] Dash de 14 px par image dans la direction du joueur, 1 s d'attente.
    /// </summary>
    public class ElderBerry : WikiBooster
    {
        protected override int Rarity => ModContent.RarityType<Turquoise>();
        protected override int SellCopper => 200000;
    }

    public class ElderBerryDash : ModPlayer
    {
        public static ModKeybind Key;
        private int cooldown;

        public override void Load() => Key = KeybindLoader.RegisterKeybind(Mod, "ElderBerryDash", "F");

        public override void Unload() => Key = null;

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (Key?.JustPressed != true || cooldown > 0 || !Player.GetModPlayer<WikiBoosterPlayer>().Used.Contains(nameof(ElderBerry)))
                return;
            int dir = Player.controlLeft ? -1 : Player.controlRight ? 1 : Player.direction;
            Player.velocity.X = dir * 14f;
            cooldown = 60;
            SoundEngine.PlaySound(SoundID.Item24, Player.Center);
            for (int i = 0; i < 15; i++)
                Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.PurpleTorch, -dir * 3f, 0f, 100, default, 1.3f).noGravity = true;
        }

        public override void PostUpdate()
        {
            if (cooldown > 0)
                cooldown--;
        }
    }
}

namespace CalamityFandom.Content.Items.Accessories
{
    /// <summary>
    /// Wiki : Metamorphic Stone.txt — accessoire qui invoque un Earth Elemental (126 dégâts d'invocation) : ruées, rafales
    /// de pierres terminées par une grosse pierre qui éclate, coups de bras à 600 % (War Cleave et Armor Crunch).
    /// [choix] Une attaque toutes les 2 s qui tourne entre les trois ; War Cleave porté, Armor Crunch de Calamity.
    /// </summary>
    public class MetamorphicStone : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 32;
            Item.accessory = true;
            Item.damage = 126;
            Item.DamageType = DamageClass.Summon;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(gold: 4, silver: 80);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddBuff(ModContent.BuffType<Buffs.EarthElementalBuff>(), 2);
            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<EarthElementalMinion>()] < 1)
            {
                int damage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(Item.damage);
                int p = Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<EarthElementalMinion>(), damage, 4f, player.whoAmI);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].originalDamage = Item.damage;
            }
        }
    }
}

namespace CalamityFandom.Content.Buffs
{
    public class EarthElementalBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Weapons1119d
{
    /// <summary>Capsule : explose (moitié des dégâts, 80 px) et libère 1 fantôme (5 en frappe furtive) ; un critique tue net un non-boss.</summary>
    public class CapsuleProj : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Rogue/CreatureCapsule";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.scale = 0.7f;
        }

        public override void AI()
        {
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.25f, 14f);
            Projectile.rotation += 0.2f * Math.Sign(Projectile.velocity.X);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // « Capturé » : un critique sur un ennemi normal le tue
            if (!target.boss && target.realLife < 0)
                modifiers.SetInstantKill();
            modifiers.DisableCrit();
        }

        public override bool? CanHitNPC(NPC target) => null;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14 with { Volume = 0.5f }, Projectile.Center);
            for (int i = 0; i < 15; i++)
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Clentaminator_Purple, 0f, 0f, 100, default, 1.3f).velocity *= 2f;
            if (Projectile.owner != Main.myPlayer)
                return;
            int ghosts = Projectile.Calamity().stealthStrike ? 5 : 1;
            for (int i = 0; i < ghosts; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Circular(4f, 4f), ModContent.ProjectileType<CapsuleGhost>(),
                    Projectile.damage / 4, 0f, Projectile.owner);
        }
    }

    /// <summary>Fantôme de la capsule : vole au hasard 6 s et tire sur l'ennemi le plus proche toutes les 30 images.</summary>
    public class CapsuleGhost : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.SpectreWrath;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 360;
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            if (Main.rand.NextBool(20))
                Projectile.velocity = Main.rand.NextVector2Circular(4f, 4f);
            Dust.NewDustPerfect(Projectile.Center, DustID.Clentaminator_Purple, Vector2.Zero, 100, default, 1f).noGravity = true;
            if (Projectile.owner != Main.myPlayer || ++Projectile.ai[0] < 30f)
                return;
            NPC target = Projectile.Center.ClosestNPCAt(600f);
            if (target == null)
                return;
            Projectile.ai[0] = 0f;
            int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.SafeDirectionTo(target.Center) * 12f, ProjectileID.LostSoulFriendly,
                Projectile.damage, 0f, Projectile.owner);
            if (Main.projectile.IndexInRange(p))
                Main.projectile[p].DamageType = ModContent.GetInstance<RogueDamageClass>();
        }
    }

    /// <summary>Lance tenue de Guardians Wrath.</summary>
    public class GuardiansWrathSpear : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Melee/GuardiansWrathSpear";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.aiStyle = ProjAIStyleID.Spear;
            AIType = ProjectileID.Trident;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.hide = true;
        }
    }

    /// <summary>Trident lancé : se plante (ai[0] = 1 bloc, 2 + index ennemi) ; <see cref="Strike"/> : éclair x5 sur 160 px.</summary>
    public class GuardiansTrident : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Melee/GuardiansWrathSpear";

        private Vector2 offset;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 3600;
            Projectile.scale = 0.8f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0f)
            {
                Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.12f, 16f);
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
                return;
            }
            Projectile.velocity = Vector2.Zero;
            if (Projectile.ai[0] >= 2f)
            {
                NPC npc = Main.npc[(int)Projectile.ai[0] - 2];
                if (!npc.active)
                    Projectile.ai[0] = 1f;
                else
                    Projectile.Center = npc.Center + offset;
            }
        }

        public override bool? CanDamage() => Projectile.ai[0] == 0f ? null : false;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[0] != 0f)
                return;
            Projectile.ai[0] = 2f + target.whoAmI;
            offset = Projectile.Center - target.Center;
            Projectile.netUpdate = true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.ai[0] = 1f;
            return false;
        }

        public void Strike()
        {
            SoundEngine.PlaySound(SoundID.Thunder, Projectile.Center);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GuardiansLightning>(), Projectile.damage * 5, 10f, Projectile.owner);
            Projectile.Kill();
        }
    }

    /// <summary>Éclair de Guardians Wrath : colonne qui frappe une fois tout autour (160 px).</summary>
    public class GuardiansLightning : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 160;
            Projectile.height = 160;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 8;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            if (Main.dedServ)
                return;
            for (int i = 0; i < 10; i++)
                Dust.NewDustPerfect(Projectile.Center + new Vector2(Main.rand.Next(-15, 16), -Main.rand.Next(0, 600)), DustID.Electric, Vector2.Zero, 100, default, 1.6f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Electrified, 120);
    }

    /// <summary>Hallebarde de glace : vise le curseur, accélère, traverse les blocs.</summary>
    public class HailberdProjectile : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Magic/HailberdProjectile";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 3;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 120;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void AI()
        {
            if (Projectile.velocity.Length() < 26f)
                Projectile.velocity *= 1.06f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            if (Main.rand.NextBool(3))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.IceTorch, 0f, 0f, 100, default, 1.1f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Frostburn, 180);
    }

    /// <summary>Pizza qui roule en ligne droite (texture de la Pizza vanilla), traverse tout, frappe chaque ennemi une fois.</summary>
    public class RollingPizza : ModProjectile
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.Pizza;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 180;
            Projectile.scale = 1.4f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI() => Projectile.rotation += Projectile.velocity.X * 0.03f;
    }

    /// <summary>Page de l'Operator : flotte devant le joueur tant que l'arme est utilisée et lâche des tentacules vers le curseur.</summary>
    public class OperatorPage : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Summon/SignoftheOperator";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.SummonMeleeSpeed;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 2;
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!owner.channel || owner.dead || owner.HeldItem.type != ModContent.ItemType<Items.Weapons.Summon.SignoftheOperator>())
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 2;
            owner.itemTime = owner.itemAnimation = 2;
            Vector2 aim = (Main.MouseWorld - owner.Center).SafeNormalize(Vector2.UnitX * owner.direction);
            Projectile.Center = owner.Center + aim * 50f;
            Projectile.rotation = (float)Math.Sin(Main.GameUpdateCount * 0.1f) * 0.2f;
            if (Projectile.owner == Main.myPlayer && ++Projectile.ai[0] % 6f == 0f)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, aim.RotatedByRandom(0.5f) * 10f, ModContent.ProjectileType<OperatorTendril>(),
                    Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }

    /// <summary>Tentacule de l'Operator : courte portée, marque les ennemis pour les sbires (fouet).</summary>
    public class OperatorTendril : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.SummonMeleeSpeed;
            Projectile.penetrate = 2;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 18;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.Shadowflame, Vector2.Zero, 100, default, 1.3f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Ichor, 180);
            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
        }
    }

    /// <summary>
    /// Earth Elemental du Metamorphic Stone : suit le joueur ; toutes les 2 s, une attaque à tour de rôle : ruée, rafale de
    /// pierres (la dernière, grosse, éclate), coup de bras à x6 (War Cleave, Armor Crunch).
    /// </summary>
    public class EarthElementalMinion : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Summon/EarthElementalMinion";

        public override void SetStaticDefaults()
        {
            Main.projPet[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 2;
            Projectile.scale = 0.35f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        private int attack;

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!owner.active || owner.dead || !owner.HasBuff<Buffs.EarthElementalBuff>())
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 2;
            NPC target = Projectile.Center.MinionHoming(800f, owner);
            Projectile.ai[0]++;
            if (target == null)
            {
                Vector2 home = owner.Center + new Vector2(-owner.direction * 60f, -50f);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, (home - Projectile.Center) * 0.08f, 0.2f);
                Projectile.spriteDirection = owner.direction;
                return;
            }
            Projectile.spriteDirection = Math.Sign(target.Center.X - Projectile.Center.X);
            if (Projectile.ai[0] < 120f)
            {
                Vector2 near = target.Center + new Vector2(-Projectile.spriteDirection * 120f, -40f);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, (near - Projectile.Center) * 0.06f, 0.15f);
                return;
            }
            Projectile.ai[0] = 0f;
            if (Projectile.owner != Main.myPlayer)
                return;
            var source = Projectile.GetSource_FromThis();
            switch (attack++ % 3)
            {
                case 0:
                    Projectile.velocity = Projectile.SafeDirectionTo(target.Center) * 18f;
                    break;
                case 1:
                    for (int i = 0; i < 4; i++)
                        Projectile.NewProjectile(source, Projectile.Center, Projectile.SafeDirectionTo(target.Center).RotatedByRandom(0.2f) * 12f, ProjectileID.BoulderStaffOfEarth,
                            Projectile.damage / 3, 4f, Projectile.owner);
                    break;
                default:
                    Projectile.NewProjectile(source, target.Center, Vector2.Zero, ModContent.ProjectileType<ElementalArmSwing>(), Projectile.damage * 6, 8f, Projectile.owner);
                    break;
            }
            Projectile.netUpdate = true;
        }

        public override bool? CanDamage() => Projectile.velocity.Length() > 10f ? null : false;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            SpriteEffects fx = Projectile.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), 0f, tex.Size() / 2f, Projectile.scale, fx);
            return false;
        }
    }

    /// <summary>Coup de bras de l'Earth Elemental : x6, War Cleave et Armor Crunch.</summary>
    public class ElementalArmSwing : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 90;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 6;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 180);
            if (ModContent.TryFind<ModBuff>("CalamityFandom", "WarCleave", out var cleave))
                target.AddBuff(cleave.Type, 180);
            if (!Main.dedServ)
                for (int i = 0; i < 15; i++)
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Stone, 0f, 0f, 100, default, 1.5f).velocity *= 2f;
        }
    }
}
