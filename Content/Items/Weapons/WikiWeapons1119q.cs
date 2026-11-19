using System;
using CalamityFandom.Content.Projectiles.Weapons1119q;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

// Lot U (19/11) : objets du wiki fabricables à l'étape « Capsules » de l'Aquatic Hypercomputer (recettes du wiki).

namespace CalamityFandom.Content.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Doctrine Spear.txt — lance qui crée 4 orbes chercheurs aux debuffs différents (3 s chacun) : Plague, Brimstone
    /// Flames, Frostburn et un 4e (coupé sur la page). [choix] 4e orbe : Ichor ; orbes à 50 %, créés à chaque coup d'estoc.
    /// </summary>
    public class DoctrineSpear : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.Spears[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 160;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = Item.noUseGraphic = true;
            Item.knockBack = 12f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = 76;
            Item.height = 82;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.sellPrice(gold: 30);
            Item.shoot = ModContent.ProjectileType<DoctrineSpearProj>();
            Item.shootSpeed = 12f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            for (int i = 0; i < 4; i++)
                Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.Lerp(-0.4f, 0.4f, i / 3f)) * 0.8f, ModContent.ProjectileType<DoctrineOrb>(), damage / 2, 2f, player.whoAmI, i);
            return false;
        }
    }

    /// <summary>
    /// Wiki : Chemicircle.txt — yoyo qui lâche sans arrêt des gaz (120 % des dégâts) qui infligent « Cancer ».
    /// [choix] Un nuage de gaz toutes les 20 images ; « Cancer » (moitié d'une statistique au hasard, 550 à 900 dégâts au
    /// hasard) n'existe pas : Poisoned 5 s à la place. Les règles de chance et de critique de l'infobulle ne sont pas faites.
    /// </summary>
    public class Chemicircle : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.Yoyo[Type] = true;
            ItemID.Sets.GamepadExtraRange[Type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 80;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.useTime = Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = Item.noUseGraphic = true;
            Item.channel = true;
            Item.knockBack = 2.5f;
            Item.UseSound = SoundID.Item1;
            Item.width = 30;
            Item.height = 36;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 20);
            Item.shoot = ModContent.ProjectileType<ChemicircleYoyo>();
            Item.shootSpeed = 14f;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Ranged
{
    /// <summary>
    /// Wiki : Nightic Blaster.txt — fléchette qui fait tomber des étoiles du ciel (200 %), qui se brisent au premier contact ;
    /// la nuit, elles traversent les blocs. [choix] 2 étoiles par tir sur le curseur.
    /// </summary>
    public class NighticBlaster : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 100;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item98;
            Item.width = 68;
            Item.height = 34;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.value = Item.sellPrice(gold: 50);
            Item.shoot = ProjectileID.PoisonDartBlowgun;
            Item.shootSpeed = 22f;
            Item.useAmmo = AmmoID.Dart;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            for (int i = 0; i < 2; i++)
            {
                Vector2 from = new(Main.MouseWorld.X + Main.rand.Next(-200, 201), Main.screenPosition.Y - 40f);
                Projectile.NewProjectile(source, from, (Main.MouseWorld - from).SafeNormalize(Vector2.UnitY) * 18f, ModContent.ProjectileType<NighticStar>(), damage * 2, knockback, player.whoAmI,
                    Main.dayTime ? 0f : 1f);
            }
            return false;
        }
    }

    /// <summary>
    /// Wiki : Cyberlight Striker.txt — fléchette très rapide qui tire une balle dans un sens au hasard toutes les 0,1 s ; clic
    /// droit : une cible ; une balle qui touche la cible devient 5 fléchettes chercheuses (80 %). [choix] Balles à 30 %,
    /// cible de 5 s au curseur (une à la fois).
    /// </summary>
    public class CyberlightStriker : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 60;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item98;
            Item.width = 70;
            Item.height = 22;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(platinum: 1);
            Item.shoot = ModContent.ProjectileType<CyberlightDart>();
            Item.shootSpeed = 32f;
            Item.useAmmo = AmmoID.Dart;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            Item.useAmmo = player.altFunctionUse == 2 ? AmmoID.None : AmmoID.Dart;
            Item.useTime = Item.useAnimation = player.altFunctionUse == 2 ? 20 : 60;
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                foreach (Projectile p in Main.ActiveProjectiles)
                    if (p.owner == player.whoAmI && p.type == ModContent.ProjectileType<Bullseye>())
                        p.Kill();
                Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<Bullseye>(), damage, 0f, player.whoAmI);
                return false;
            }
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<CyberlightDart>(), damage, knockback, player.whoAmI);
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Accessories
{
    /// <summary>
    /// Wiki : Phantasmal Rune.txt — à chaque coup de sbire, 5 % de chances : un fantôme qui poursuit et traverse les ennemis
    /// (100 dégâts par seconde), ou un feu (15 dégâts par seconde) qui lance des orbes qui propagent le feu. Amélioration de
    /// la Ghast Relic. [choix] Feu = debuff « Phantasmal Fire » de ce mod, 10 s (infobox) ; un orbe toutes les 2 s vers un ennemi proche à 25 blocs.
    /// </summary>
    public class PhantasmalRune : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 30;
            Item.accessory = true;
            Item.rare = ItemRarityID.Cyan;
            Item.value = Item.sellPrice(gold: 5);
        }

        public override void UpdateAccessory(Player player, bool hideVisual) => player.GetModPlayer<PhantasmalRunePlayer>().Equipped = true;
    }

    public class PhantasmalRunePlayer : ModPlayer
    {
        public bool Equipped;

        public override void ResetEffects() => Equipped = false;

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!Equipped || Player.whoAmI != Main.myPlayer || !(proj.minion || ProjectileID.Sets.MinionShot[proj.type]) || !Main.rand.NextBool(20))
                return;
            if (Main.rand.NextBool())
                Projectile.NewProjectile(Player.GetSource_Accessory(Player.HeldItem), target.Center, Main.rand.NextVector2Circular(3f, 3f), ModContent.ProjectileType<Projectiles.Weapons1119k.RelicGhast>(),
                    100, 0f, Player.whoAmI);
            else
                target.AddBuff(ModContent.BuffType<PhantasmalSpreadFire>(), 600);
        }
    }

    /// <summary>Feu de la Phantasmal Rune : 15 dégâts par seconde ; toutes les 2 s, un orbe qui porte le feu à un ennemi proche.</summary>
    public class PhantasmalSpreadFire : ModBuff
    {
        public override string Texture => "Terraria/Images/Buff_" + BuffID.ShadowFlame;

        public override void SetStaticDefaults() => Main.debuff[Type] = true;

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<PhantasmalFireNPC>().Burning = true;
            if (Main.netMode != NetmodeID.MultiplayerClient && npc.buffTime[buffIndex] % 120 == 0)
                foreach (NPC other in Main.ActiveNPCs)
                    if (other.whoAmI != npc.whoAmI && other.CanBeChasedBy() && other.Distance(npc.Center) < 400f && !other.HasBuff(Type))
                    {
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, npc.SafeDirectionTo(other.Center) * 8f, ModContent.ProjectileType<PhantasmalFireOrb>(), 0, 0f, Main.myPlayer, other.whoAmI);
                        break;
                    }
        }
    }

    public class PhantasmalFireNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public bool Burning;

        public override void ResetEffects(NPC npc) => Burning = false;

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (!Burning)
                return;
            if (npc.lifeRegen > 0)
                npc.lifeRegen = 0;
            npc.lifeRegen -= 30;
            damage = Math.Max(damage, 5);
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (Burning && Main.rand.NextBool(4))
                Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.DungeonSpirit, 0f, -1f, 100, default, 1.2f).noGravity = true;
        }
    }

    /// <summary>Orbe qui porte le feu de la Phantasmal Rune à l'ennemi ai[0].</summary>
    public class PhantasmalFireOrb : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            NPC target = Main.npc[(int)Projectile.ai[0]];
            if (!target.active)
            {
                Projectile.Kill();
                return;
            }
            Projectile.velocity = Projectile.SafeDirectionTo(target.Center) * 8f;
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.DungeonSpirit, Vector2.Zero, 100, default, 1.2f).noGravity = true;
            if (Projectile.Hitbox.Intersects(target.Hitbox))
            {
                target.AddBuff(ModContent.BuffType<PhantasmalSpreadFire>(), 600);
                Projectile.Kill();
            }
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Weapons1119q
{
    /// <summary>Lance tenue de la Doctrine Spear.</summary>
    public class DoctrineSpearProj : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Melee/DoctrineSpear";

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
            Projectile.scale = 0.8f;
        }
    }

    /// <summary>Orbe de la Doctrine Spear (ai[0] = 0 Plague, 1 Brimstone Flames, 2 Frostburn, 3 Ichor ; 3 s) : 3 s, chercheur.</summary>
    public class DoctrineOrb : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (++Projectile.localAI[0] > 10f)
                CalamityUtils.HomeInOnNPC(Projectile, true, 700f, 12f, 15f);
            if (Main.dedServ)
                return;
            int dust = (int)Projectile.ai[0] switch { 0 => DustID.GreenTorch, 1 => DustID.RedTorch, 2 => DustID.IceTorch, _ => DustID.YellowTorch };
            Dust.NewDustPerfect(Projectile.Center, dust, Vector2.Zero, 100, default, 1.5f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            switch ((int)Projectile.ai[0])
            {
                case 0: target.AddBuff(ModContent.BuffType<Plague>(), 180); break;
                case 1: target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 180); break;
                case 2: target.AddBuff(BuffID.Frostburn, 180); break;
                default: target.AddBuff(BuffID.Ichor, 180); break;
            }
        }
    }

    /// <summary>Yoyo de la Chemicircle : lâche un nuage de gaz (120 %) toutes les 20 images.</summary>
    public class ChemicircleYoyo : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.Yelets;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Type] = 12f;
            ProjectileID.Sets.YoyosMaximumRange[Type] = 280f;
            ProjectileID.Sets.YoyosTopSpeed[Type] = 14f;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.aiStyle = ProjAIStyleID.Yoyo;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = -1;
        }

        public override void PostAI()
        {
            if (Projectile.owner == Main.myPlayer && ++Projectile.localAI[1] % 20 == 0)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Circular(2f, 2f), ModContent.ProjectileType<CarcinogenicGas>(),
                    (int)(Projectile.damage * 1.2f), 0f, Projectile.owner);
        }

        public override Color? GetAlpha(Color lightColor) => new Color(200, 160, 90, 220);
    }

    /// <summary>Gaz cancérigène : 1,5 s, frappe une fois chaque ennemi (Poisoned 5 s).</summary>
    public class CarcinogenicGas : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 90;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.95f;
            if (!Main.dedServ)
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, -0.5f, 150, new Color(160, 150, 90), 1.5f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Poisoned, 300);
    }

    /// <summary>Étoile du Nightic Blaster : se brise au premier contact ; ai[0] = 1 (nuit) : traverse les blocs.</summary>
    public class NighticStar : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.Starfury;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
        }

        public override void OnSpawn(IEntitySource source) => Projectile.tileCollide = Projectile.ai[0] != 1f;

        public override void AI()
        {
            Projectile.rotation += 0.2f;
            if (!Main.dedServ && Main.rand.NextBool(2))
                Dust.NewDustPerfect(Projectile.Center, DustID.YellowStarDust, Vector2.Zero, 100, default, 1f).noGravity = true;
        }
    }

    /// <summary>Fléchette de la Cyberlight Striker : très rapide, tire une balle dans un sens au hasard toutes les 6 images (30 %).</summary>
    public class CyberlightDart : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.CursedDartFlame;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.Electric, Vector2.Zero, 100, default, 0.8f).noGravity = true;
            if (Projectile.owner == Main.myPlayer && ++Projectile.localAI[0] % 12 == 0)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Unit() * 14f, ModContent.ProjectileType<CyberBullet>(), (int)(Projectile.damage * 0.3f), 0f, Projectile.owner);
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }

    /// <summary>Balle de la Cyberlight Striker : sur une cible (Bullseye), devient 5 fléchettes chercheuses (80 % de l'arme).</summary>
    public class CyberBullet : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 60;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.Electric, Vector2.Zero, 100, default, 0.6f).noGravity = true;
            if (Projectile.owner != Main.myPlayer)
                return;
            foreach (Projectile p in Main.ActiveProjectiles)
                if (p.owner == Projectile.owner && p.type == ModContent.ProjectileType<Bullseye>() && p.Hitbox.Intersects(Projectile.Hitbox))
                {
                    for (int i = 0; i < 5; i++)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), p.Center, Main.rand.NextVector2CircularEdge(10f, 10f), ModContent.ProjectileType<HomingCyberDart>(),
                            (int)(p.damage * 0.8f), 0f, Projectile.owner);
                    SoundEngine.PlaySound(SoundID.Item93, p.Center);
                    Projectile.Kill();
                    break;
                }
        }
    }

    /// <summary>Cible de la Cyberlight Striker : 5 s au curseur.</summary>
    public class Bullseye : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            if (!Main.dedServ)
                for (int i = 0; i < 2; i++)
                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(20f, 20f), DustID.RedTorch, Vector2.Zero, 100, default, 1f).noGravity = true;
        }
    }

    /// <summary>Fléchette chercheuse de la cible.</summary>
    public class HomingCyberDart : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.CursedDartFlame;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            CalamityUtils.HomeInOnNPC(Projectile, true, 900f, 16f, 10f);
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.RedTorch, Vector2.Zero, 100, default, 1f).noGravity = true;
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }
}
