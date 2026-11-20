using System;
using System.Linq;
using CalamityFandom.Common.Debug;
using CalamityFandom.Common.WikiData;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

// Lot AV (20/11) : armes dont la recette du wiki est devenue faisable (Entropic Refiner, Klepticoin, Miracle Coin...).

namespace CalamityFandom.Content.Items.Weapons.Ranged
{
    /// <summary>
    /// Wiki : Charity.txt — lance-pièces « médiocre » : dégâts des pièces du tableau (cuivre 15, argent 60...), Midas 10 s
    /// (argent) et 20 s (or).
    /// </summary>
    public class Charity : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 0;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 8;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item11;
            Item.width = 58;
            Item.height = 26;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 2);
            Item.shoot = ProjectileID.CopperCoin;
            Item.shootSpeed = 24f;
            Item.useAmmo = AmmoID.Coin;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            // [choix] Dégâts du tableau ajoutés à ceux de la pièce (cuivre 15, argent 60 ; or et platine : x4 à chaque palier)
            int bonus = type switch { ProjectileID.CopperCoin => 15, ProjectileID.SilverCoin => 60, ProjectileID.GoldCoin => 240, ProjectileID.PlatinumCoin => 960, _ => 0 };
            damage += (int)player.GetTotalDamage(DamageClass.Ranged).ApplyTo(bonus);
        }
    }

    /// <summary>Midas de la Charity : 10 s (argent), 20 s (or).</summary>
    public class CharityCoins : GlobalProjectile
    {
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (projectile.owner < 0 || projectile.owner >= Main.maxPlayers || Main.player[projectile.owner].HeldItem.type != ModContent.ItemType<Charity>())
                return;
            if (projectile.type == ProjectileID.SilverCoin)
                target.AddBuff(BuffID.Midas, 600);
            else if (projectile.type == ProjectileID.GoldCoin)
                target.AddBuff(BuffID.Midas, 1200);
        }
    }

    /// <summary>
    /// Wiki : Cobadium Rifle.txt — la « Palladium Dartshooter » (nom de l'infobox, du texte et des recettes) : deux fléchettes
    /// rapides à la fois. [choix] Nom affiché « Palladium Dartshooter », celui que les recettes demandent ; écart de 4°.
    /// </summary>
    public class PalladiumDartshooter : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 40;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item98;
            Item.width = 58;
            Item.height = 26;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(gold: 5);
            Item.shoot = ProjectileID.PoisonDartBlowgun;
            Item.shootSpeed = 32f;
            Item.useAmmo = AmmoID.Dart;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = -1; i <= 1; i += 2)
                Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(2f * i)), type, damage, knockback, player.whoAmI);
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Frostflow Edge.txt — 1 dégât toujours, jamais de critique ; chaque coup retire 4 % de la vie max de l'ennemi
    /// (sans défense ni réduction ; seulement la tête des vers). Clic droit (mana plein) : perd 50 % de sa vie max et tout son
    /// mana, la lame est renforcée 1 minute : les coups lancent des vagues glaciales qui retirent 2 %. Dans l'eau avec moins
    /// de 40 défense : -20 PV par seconde.
    /// [choix] Une vague par coup pendant le renfort ; la perte de vie ne tue pas (1 PV au moins).
    /// </summary>
    public class FrostflowEdge : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 1;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3f;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.UseSound = SoundID.Item1;
            Item.width = 64;
            Item.height = 70;
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(platinum: 1);
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
                return player.statMana >= player.statManaMax2 && player.GetModPlayer<FrostflowPlayer>().Empowered <= 0;
            return true;
        }

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2 && player.whoAmI == Main.myPlayer)
            {
                player.statMana = 0;
                int loss = Math.Min(player.statLifeMax2 / 2, player.statLife - 1);
                player.statLife -= loss;
                CombatText.NewText(player.Hitbox, CombatText.DamagedFriendly, loss);
                player.GetModPlayer<FrostflowPlayer>().Empowered = 3600;
                SoundEngine.PlaySound(SoundID.Item30, player.Center);
            }
            return true;
        }

        public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.DisableCrit();
            modifiers.SetMaxDamage(1);
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage) => damage = StatModifier.Default;

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) => FrostflowPlayer.Drain(target, 0.04f);

        public override void UseItemFrame(Player player)
        {
            var fp = player.GetModPlayer<FrostflowPlayer>();
            if (player.altFunctionUse != 2 && fp.Empowered > 0 && player.itemAnimation == player.itemAnimationMax - 1 && player.whoAmI == Main.myPlayer)
                Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX) * 14f,
                    ModContent.ProjectileType<Projectiles.Melee.GlacialWave>(), 1, 0f, player.whoAmI);
        }
    }

    public class FrostflowPlayer : ModPlayer
    {
        public int Empowered;

        /// <summary>Retire une part de la vie max (hors défense) ; les corps et queues de vers ne comptent pas.</summary>
        public static void Drain(NPC target, float part)
        {
            if (target.dontTakeDamage || target.realLife >= 0 && target.realLife != target.whoAmI)
                return;
            // Frappe directe : ni défense ni réduction des dégâts (SimpleStrikeNPC appliquerait la défense)
            var hit = new NPC.HitInfo { Damage = Math.Max(1, (int)(target.lifeMax * part)), HitDirection = 0 };
            target.StrikeNPC(hit);
            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendStrikeNPC(target, hit);
        }

        public override void PostUpdate()
        {
            if (Empowered > 0)
                Empowered--;
        }

        public override void UpdateBadLifeRegen()
        {
            if (Player.HeldItem.type == ModContent.ItemType<FrostflowEdge>() && Player.statDefense < 40
                && Collision.DrownCollision(Player.position, Player.width, Player.height, Player.gravDir))
            {
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;
                Player.lifeRegenTime = 0;
                Player.lifeRegen -= 40;
            }
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Magic
{
    /// <summary>
    /// Wiki : Omega Ray.txt — clic gauche : cinq rayons parallèles (80 %) et des lasers rapides (40 %) ; clic droit : un rayon
    /// (166,25 %) qui se divise en cinq lasers qui tournent au hasard. Tout traverse les blocs, les rayons traversent tout.
    /// [choix] Un laser par tir à gauche ; division au bout de 20 images.
    /// </summary>
    public class OmegaRay : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 210;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 4;
            Item.useTime = Item.useAnimation = 4;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item12 with { Volume = 0.4f };
            Item.width = 106;
            Item.height = 130;
            Item.scale = 0.6f;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
            Item.value = Item.sellPrice(gold: 40);
            Item.shoot = ModContent.ProjectileType<Projectiles.Magic.OmegaBeam>();
            Item.shootSpeed = 11f;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override Vector2? HoldoutOffset() => new Vector2(-20f, 0f);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(source, position, velocity, type, (int)(damage * 1.6625f), knockback, player.whoAmI, 1f);
                return false;
            }
            Vector2 normal = velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2);
            for (int i = -2; i <= 2; i++)
                Projectile.NewProjectile(source, position + normal * i * 8f, velocity, type, (int)(damage * 0.8f), knockback, player.whoAmI);
            Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.1f) * 1.5f, ModContent.ProjectileType<Projectiles.Magic.OmegaLaser>(), (int)(damage * 0.4f), knockback * 0.4f, player.whoAmI);
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Rogue
{
    /// <summary>
    /// Wiki : Flame Boy Popsicle.txt — un esquimau soumis à la gravité qui rebondit sur l'ennemi touché et se ramasse ; le
    /// ramasser fait monter le rang (D, C, B, A, S, S+, U : dégâts, critique et durée avant remise à zéro du tableau). B et
    /// plus : explosion rouge (Vulnerability Hex) ; S et plus : tourbillon qui détruit les tirs ennemis ; U : éclats de glace
    /// chercheurs (500 %) puis remise à zéro. Frappe furtive : lancer plus fort, dégâts doublés, ramassage plus large ;
    /// Miracle Blight 2 s.
    /// [choix] Un seul esquimau à la fois ; tourbillon de 200 px (300 en S+) pendant 1 s ; 6 éclats.
    /// </summary>
    public class FlameBoyPopsicle : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 1050;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.crit = 6;
            Item.useTime = Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = 42;
            Item.height = 62;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.value = Item.sellPrice(platinum: 1);
            Item.shoot = ModContent.ProjectileType<Projectiles.Rogue.PopsicleProj>();
            Item.shootSpeed = 40f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            var pp = player.GetModPlayer<PopsiclePlayer>();
            damage *= PopsiclePlayer.Damage[pp.Rank] / 1050f;
        }

        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += PopsiclePlayer.Crit[player.GetModPlayer<PopsiclePlayer>().Rank] - 10;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool stealth = player.Calamity().StealthStrikeAvailable();
            int p = Projectile.NewProjectile(source, position, stealth ? velocity * 1.3f : velocity * 0.4f, type, stealth ? damage * 2 : damage, knockback, player.whoAmI);
            if (stealth && Main.projectile.IndexInRange(p))
            {
                Main.projectile[p].Calamity().stealthStrike = true;
                player.Calamity().ConsumeStealthByAttacking();
            }
            return false;
        }
    }

    /// <summary>Rang de l'esquimau (0 = D … 6 = U), ramassages vers le rang suivant, temps avant remise à zéro.</summary>
    public class PopsiclePlayer : ModPlayer
    {
        public static readonly string[] Names = { "D", "C", "B", "A", "S", "S+", "U" };
        public static readonly int[] Damage = { 1575, 2363, 3545, 5318, 7977, 11966, 17949 };
        public static readonly int[] Crit = { 20, 30, 40, 50, 60, 75, 90 };
        private static readonly float[] Seconds = { 10f, 9f, 8f, 6f, 4f, 3.5f, 3f };
        private static readonly int[] PickupsToNext = { 1, 1, 1, 2, 3, 5, 0 };

        public int Rank, Pickups, Timer;

        public void PickUp()
        {
            if (Rank < 6 && ++Pickups >= PickupsToNext[Rank])
            {
                Rank++;
                Pickups = 0;
                CombatText.NewText(Player.Hitbox, new Color(254, 88, 28), Names[Rank], true);
            }
            Timer = (int)(Seconds[Rank] * 60);
        }

        public void Reset()
        {
            Rank = Pickups = Timer = 0;
        }

        public override void PostUpdate()
        {
            if (Timer > 0 && --Timer == 0)
                Reset();
            // A et plus : la furtivité monte 50 % plus vite
            if (Rank >= 3 && Player.HeldItem.type == ModContent.ItemType<FlameBoyPopsicle>())
            {
                Player.Calamity().stealthGenStandstill += 0.5f;
                Player.Calamity().stealthGenMoving += 0.5f;
            }
        }
    }

    public class WikiWeapons1120cChecks : ModSystem
    {
        public override void Load()
        {
            SelfTest.ExtraChecks.Add(t =>
            {
                foreach (string name in new[] { "Charity", "Palladium Dartshooter", "Frostflow Edge", "Omega Ray", "Flame Boy Popsicle" })
                {
                    int type = WikiRecipes.ResolveItem(name);
                    t.Expect(type > 0 && Main.recipe.Take(Recipe.numRecipes).Any(r => r.createItem.type == type && !r.Disabled), $"recette du wiki de {name}");
                }
            });
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Melee
{
    /// <summary>Vague glaciale (Frostflow Edge renforcée) : traverse, retire 2 % de la vie max.</summary>
    public class GlacialWave : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Melee/GlacialWave";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 45;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            Projectile.alpha = Math.Max(0, 255 - Projectile.timeLeft * 12);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.DisableCrit();
            modifiers.SetMaxDamage(1);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => Items.Weapons.Melee.FrostflowPlayer.Drain(target, 0.02f);

        public override Color? GetAlpha(Color lightColor) => Color.White * (1f - Projectile.alpha / 255f);
    }
}

namespace CalamityFandom.Content.Projectiles.Magic
{
    /// <summary>Rayon de l'Omega Ray : traverse tout ; ai[0] = 1 : rayon du clic droit, qui se divise en 5 lasers tournoyants.</summary>
    public class OmegaBeam : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 90;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Color c = Projectile.ai[0] == 1f ? new Color(255, 80, 200) : new Color(120, 255, 200);
            Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.RainbowMk2, Vector2.Zero, 0, c, Projectile.ai[0] == 1f ? 1.6f : 1f);
            d.noGravity = true;
            if (Projectile.ai[0] == 1f && ++Projectile.localAI[0] == 80 && Projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < 5; i++)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.RotatedBy(MathHelper.ToRadians(-30f + 15f * i)),
                        ModContent.ProjectileType<OmegaLaser>(), Projectile.damage / 3, Projectile.knockBack, Projectile.owner, 1f);
                Projectile.Kill();
            }
        }
    }

    /// <summary>Laser de l'Omega Ray : traverse les blocs ; ai[0] = 1 : tourne au hasard (division du clic droit).</summary>
    public class OmegaLaser : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.GreenLaser;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 1f)
                Projectile.velocity = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.12f, 0.12f));
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;
    }
}

namespace CalamityFandom.Content.Projectiles.Rogue
{
    /// <summary>
    /// Esquimau (Flame Boy Popsicle) : soumis à la gravité, rebondit sur l'ennemi touché, se ramasse (rang suivant) ; il
    /// disparaît au bout de 5 s sans être ramassé (rang remis à zéro).
    /// </summary>
    public class PopsicleProj : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Rogue/FlameBoyPopsicle";

        private bool Bounced => Projectile.ai[0] == 1f;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.scale = 0.7f;
        }

        public override void AI()
        {
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.35f, 16f);
            Projectile.rotation += 0.3f * Math.Sign(Projectile.velocity.X == 0 ? 1 : Projectile.velocity.X);
            Lighting.AddLight(Projectile.Center, 1f, 0.4f, 0.1f);
            Player owner = Main.player[Projectile.owner];
            float pickup = Projectile.Calamity().stealthStrike ? 120f : 50f;
            if (Bounced && Projectile.owner == Main.myPlayer && owner.Distance(Projectile.Center) < pickup)
            {
                var pp = owner.GetModPlayer<Items.Weapons.Rogue.PopsiclePlayer>();
                pp.PickUp();
                if (pp.Rank >= 4)
                    Whirlwind(owner, pp.Rank >= 5 ? 300f : 200f);
                SoundEngine.PlaySound(SoundID.Grab, owner.Center);
                Projectile.Kill();
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (timeLeft <= 0 && Projectile.owner == Main.myPlayer)
                Main.player[Projectile.owner].GetModPlayer<Items.Weapons.Rogue.PopsiclePlayer>().Reset();
        }

        /// <summary>S et plus : tourbillon qui détruit les tirs ennemis autour du joueur.</summary>
        private static void Whirlwind(Player owner, float radius)
        {
            foreach (Projectile p in Main.projectile)
                if (p.active && p.hostile && p.Distance(owner.Center) < radius)
                    p.Kill();
            for (int i = 0; i < 40; i++)
                Dust.NewDustPerfect(owner.Center + (MathHelper.TwoPi * i / 40f).ToRotationVector2() * radius, DustID.Torch, Vector2.Zero, 0, default, 1.8f).noGravity = true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.ai[0] = 1f;
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X * 0.5f;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y * 0.4f;
            Projectile.velocity.X *= 0.9f;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<MiracleBlight>(), 120);
            Player owner = Main.player[Projectile.owner];
            var pp = owner.GetModPlayer<Items.Weapons.Rogue.PopsiclePlayer>();
            // Rebondit vers le joueur
            Projectile.ai[0] = 1f;
            Projectile.velocity = new Vector2(Math.Sign(owner.Center.X - target.Center.X) * 5f, -8f);
            if (Projectile.owner != Main.myPlayer)
                return;
            var source = Projectile.GetSource_FromThis();
            if (pp.Rank >= 2)
            {
                float part = pp.Rank >= 5 ? 3f : pp.Rank >= 4 ? 1.5f : pp.Rank >= 3 ? 0.75f : 0.5f;
                Projectile.NewProjectile(source, target.Center, Vector2.Zero, ModContent.ProjectileType<WikiDarts.DartBlast>(), (int)(Projectile.damage * part), 0f,
                    Projectile.owner, 90f, DustID.Torch, ModContent.BuffType<VulnerabilityHex>());
            }
            if (pp.Rank >= 6)
            {
                for (int i = 0; i < 6; i++)
                    Projectile.NewProjectile(source, target.Center, Main.rand.NextVector2Unit() * 10f, ModContent.ProjectileType<PopsicleShard>(), Projectile.damage * 5, 0f, Projectile.owner, target.whoAmI);
                pp.Reset();
            }
        }
    }

    /// <summary>Éclat de glace (rang U) : fonce sur l'ennemi visé.</summary>
    public class PopsicleShard : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.FrostShard;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            NPC t = Main.npc[(int)Projectile.ai[0]];
            if (t.active && !t.friendly)
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(t.Center) * 18f, 0.12f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }
    }
}
