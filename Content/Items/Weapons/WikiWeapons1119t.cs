using System;
using CalamityFandom.Content.Projectiles.Weapons1119t;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

// Lot X (19/11) : armes du wiki fabricables à Draedon's Forge (ou à l'Aquatic Hypercomputer complet, qui en fait office),
// recettes du wiki. Raretés « C1 » inventées : 0. « Reactive Flames » (sans page) : non fait.

namespace CalamityFandom.Content.Items.Weapons.Rogue
{
    /// <summary>
    /// Wiki : Digital Cheese Spear.txt — javelot très rapide ; à l'impact, 12 mini-lances en barrage (65 %) ; frappe furtive :
    /// en plus, 12 lances identiques très chercheuses, qui appellent chacune 6 mini-lances de plus (56 %). [choix] Mini-lances
    /// depuis 600 px autour de l'ennemi.
    /// </summary>
    public class DigitalCheeseSpear : RogueWeapon
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 5656;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.useTime = Item.useAnimation = 70;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = Item.noUseGraphic = true;
            Item.knockBack = 8f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = Item.height = 90;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.value = Item.sellPrice(platinum: 10, gold: 56, silver: 56, copper: 56);
            Item.shoot = ModContent.ProjectileType<CheeseSpear>();
            Item.shootSpeed = 35f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (player.Calamity().StealthStrikeAvailable() && Main.projectile.IndexInRange(p))
                Main.projectile[p].Calamity().stealthStrike = true;
            return false;
        }
    }

    /// <summary>
    /// Wiki : Exodimus.txt — 5 dagues en éventail qui percent à l'infini ; après 0,5 s elles foncent vers les ennemis (ou le
    /// curseur) et s'y plantent (dégâts continus, 20 au plus par ennemi) ; au bout de 10 s, chacune explose en croix de
    /// lasers (4) ; frappe furtive : étoile de 8 lasers. Amélioration du Seraphim. [choix] Vulnerability Hex (après Supreme
    /// Calamitas) non fait ; un coup toutes les 20 images plantée ; lasers à 50 %.
    /// </summary>
    public class Exodimus : RogueWeapon
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 426;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.useTime = Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = Item.noUseGraphic = true;
            Item.knockBack = 8f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = Item.height = 65;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
            Item.value = Item.sellPrice(gold: 40);
            Item.shoot = ModContent.ProjectileType<ExoDagger>();
            Item.shootSpeed = 16f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool stealth = player.Calamity().StealthStrikeAvailable();
            for (int i = -2; i <= 2; i++)
            {
                int p = Projectile.NewProjectile(source, position, velocity.RotatedBy(0.15f * i), type, damage, knockback, player.whoAmI);
                if (stealth && Main.projectile.IndexInRange(p))
                    Main.projectile[p].Calamity().stealthStrike = true;
            }
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Ranged
{
    /// <summary>
    /// Wiki : Endentant Bow.txt — la flèche qui touche fait venir 3 flèches de derrière l'ennemi, qui en font venir 3 autres
    /// en touchant, etc., jusqu'à ce qu'aucune ne touche ; les copies disparaissent au tir suivant. [choix] 4 générations au plus.
    /// </summary>
    public class EndentantBow : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 800;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item5;
            Item.width = 41;
            Item.height = 71;
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(gold: 50);
            Item.shoot = ModContent.ProjectileType<EndentantArrow>();
            Item.shootSpeed = 25f;
            Item.useAmmo = AmmoID.Arrow;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Les copies du tir précédent disparaissent
            foreach (Projectile p in Main.ActiveProjectiles)
                if (p.owner == player.whoAmI && p.type == ModContent.ProjectileType<EndentantArrow>() && p.ai[0] > 0f)
                    p.Kill();
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<EndentantArrow>(), damage, knockback, player.whoAmI);
            return false;
        }
    }

    /// <summary>
    /// Wiki : Dethermination.txt — fléchette extrêmement rapide (recul énorme) qui laisse des entailles sur son chemin (200 %,
    /// à travers les blocs, 5 s). [choix] Une entaille toutes les 8 images de vol.
    /// </summary>
    public class Dethermination : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 800;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 72;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 60f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item98;
            Item.width = 74;
            Item.height = 26;
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(platinum: 1);
            Item.shoot = ModContent.ProjectileType<WaterfreezeDart>();
            Item.shootSpeed = 30f;
            Item.useAmmo = AmmoID.Dart;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) =>
            type = ModContent.ProjectileType<WaterfreezeDart>();
    }
}

namespace CalamityFandom.Content.Items.Weapons.Magic
{
    /// <summary>
    /// Wiki : Clamor Noxus.txt — 3 projectiles tombent du ciel : des étoiles (25 %, Moon Burnt) qui se plantent 2 s puis
    /// explosent (135 %), ou des Noxlings qui foncent sur les ennemis puis explosent en traits d'énergie Exo et un trou noir
    /// qui attire. Amélioration de la Clamor Noctus. [choix] Une chance sur deux ; Noxling : 4 traits à 50 %, trou noir de 2 s.
    /// </summary>
    public class ClamorNoxus : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 270;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 12;
            Item.useTime = Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item105;
            Item.width = Item.height = 112;
            Item.scale = 0.6f;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
            Item.value = Item.sellPrice(gold: 32);
            Item.shoot = ModContent.ProjectileType<NoxStar>();
            Item.shootSpeed = 21f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 from = new(Main.MouseWorld.X + Main.rand.Next(-250, 251), Main.screenPosition.Y - 40f);
                Vector2 v = (Main.MouseWorld - from).SafeNormalize(Vector2.UnitY) * 21f;
                if (Main.rand.NextBool())
                    Projectile.NewProjectile(source, from, v, ModContent.ProjectileType<NoxStar>(), damage / 4, knockback, player.whoAmI);
                else
                    Projectile.NewProjectile(source, from, v, ModContent.ProjectileType<Noxling>(), damage, knockback, player.whoAmI);
            }
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Lunatic Beats.txt — boomerang qui perce 10 fois avant de revenir ; en vol, il fait naître des pouvoirs célestes
    /// (boules de feu solaires chercheuses qui explosent, portails Vortex qui tirent des éclairs, 3 nébuleuses, lumières
    /// Stardust qui tombent sur les ennemis touchés) ; plus de dégâts au retour. [choix] Un pouvoir au hasard toutes les 12
    /// images (40 %) ; x1,5 au retour ; projectiles vanilla des armes lunaires.
    /// </summary>
    public class LunaticBeats : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 610;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = Item.noUseGraphic = true;
            Item.knockBack = 3f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = Item.height = 42;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.value = Item.sellPrice(platinum: 10);
            Item.shoot = ModContent.ProjectileType<LunaticDisc>();
            Item.shootSpeed = 30f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;
    }
}

namespace CalamityFandom.Content.Projectiles.Weapons1119t
{
    /// <summary>Lance au fromage : à l'impact, 12 mini-lances (65 %) ; furtive : 12 lances chercheuses en plus.</summary>
    public class CheeseSpear : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Rogue/DigitalCheeseSpear";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.extraUpdates = 1;
            Projectile.scale = 0.6f;
        }

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.owner != Main.myPlayer)
                return;
            Barrage(target, 12, (int)(Projectile.damage * 0.65f), 0f);
            if (Projectile.Calamity().stealthStrike)
                Barrage(target, 12, Projectile.damage, 1f);
        }

        private void Barrage(NPC target, int count, int damage, float kind)
        {
            for (int i = 0; i < count; i++)
            {
                Vector2 from = target.Center + Main.rand.NextVector2CircularEdge(1f, 1f) * 600f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), from, (target.Center - from).SafeNormalize(Vector2.UnitY) * 24f, ModContent.ProjectileType<MiniCheeseSpear>(),
                    damage, 0f, Projectile.owner, kind, target.whoAmI);
            }
        }
    }

    /// <summary>Mini-lance au fromage : ai[0] = 1 chercheuse vers l'ennemi ai[1], qui appelle 6 mini-lances (56 %) en touchant.</summary>
    public class MiniCheeseSpear : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Rogue/DigitalCheeseSpear";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = 1;
            Projectile.timeLeft = 90;
            Projectile.tileCollide = false;
            Projectile.scale = 0.3f;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 1f)
            {
                NPC target = Main.npc[(int)Projectile.ai[1]];
                if (target.active)
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(target.Center) * 24f, 0.2f);
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 230, 120, 220);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[0] != 1f || Projectile.owner != Main.myPlayer)
                return;
            for (int i = 0; i < 6; i++)
            {
                Vector2 from = target.Center + Main.rand.NextVector2CircularEdge(1f, 1f) * 500f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), from, (target.Center - from).SafeNormalize(Vector2.UnitY) * 22f, Type,
                    (int)(Projectile.damage * 0.56f), 0f, Projectile.owner, 0f, target.whoAmI);
            }
        }
    }

    /// <summary>Dague Exo : vole 0,5 s, fonce vers un ennemi (ou le curseur) et s'y plante 10 s, puis explose en croix (étoile en furtif).</summary>
    public class ExoDagger : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Rogue/Exodimus";

        private int stuckTo = -1;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = -1;
            Projectile.timeLeft = 900;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.scale = 0.4f;
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
            if (++Projectile.ai[0] == 30f && Projectile.owner == Main.myPlayer)
            {
                NPC target = Projectile.Center.ClosestNPCAt(1000f);
                Vector2 dest = target?.Center ?? Main.MouseWorld;
                Projectile.velocity = Projectile.SafeDirectionTo(dest) * 26f;
                Projectile.netUpdate = true;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (stuckTo >= 0 || Projectile.ai[0] < 30f)
                return;
            int stuck = 0;
            foreach (Projectile p in Main.ActiveProjectiles)
                if (p.type == Type && p.ModProjectile is ExoDagger d && d.stuckTo == target.whoAmI)
                    stuck++;
            if (stuck >= 20)
                return;
            stuckTo = target.whoAmI;
            Projectile.localAI[0] = Projectile.Center.X - target.Center.X;
            Projectile.localAI[1] = Projectile.Center.Y - target.Center.Y;
            Projectile.timeLeft = 600;
            Projectile.netUpdate = true;
        }

        public override void OnKill(int timeLeft)
        {
            if (stuckTo < 0 || Projectile.owner != Main.myPlayer)
                return;
            int beams = Projectile.Calamity().stealthStrike ? 8 : 4;
            for (int i = 0; i < beams; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (MathHelper.TwoPi * i / beams).ToRotationVector2() * 16f, ProjectileID.LastPrismLaser,
                    Projectile.damage / 2, 0f, Projectile.owner);
        }
    }

    /// <summary>Flèche de l'Endentant Bow : ai[0] = génération (0 tirée) ; en touchant, 3 flèches arrivent de derrière l'ennemi (4 générations au plus).</summary>
    public class EndentantArrow : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.FrostburnArrow;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.arrow = true;
            Projectile.extraUpdates = 1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Projectile.ai[0] > 0f)
                Projectile.tileCollide = false;
        }

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.owner != Main.myPlayer || Projectile.ai[0] >= 4f)
                return;
            Vector2 behind = target.Center + Projectile.velocity.SafeNormalize(Vector2.UnitX) * 300f;
            for (int i = -1; i <= 1; i++)
            {
                Vector2 from = behind + Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2) * 60f * i;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), from, (target.Center - from).SafeNormalize(Vector2.UnitX) * 25f, Type, Projectile.damage, 0f, Projectile.owner,
                    Projectile.ai[0] + 1f);
            }
        }
    }

    /// <summary>Fléchette de la Dethermination : extrêmement rapide, laisse une entaille (200 %, 5 s, traverse les blocs) toutes les 8 images.</summary>
    public class WaterfreezeDart : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.IchorDart;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.extraUpdates = 3;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.owner == Main.myPlayer && ++Projectile.localAI[0] % 32 == 0)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<WaterfreezeSlash>(), Projectile.damage * 2, 0f, Projectile.owner);
        }

        public override Color? GetAlpha(Color lightColor) => new Color(150, 220, 255, 200);
    }

    /// <summary>Entaille de la Dethermination : 5 s, frappe toutes les 20 images.</summary>
    public class WaterfreezeSlash : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            if (!Main.dedServ && Main.rand.NextBool(2))
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(20f, 4f).RotatedBy(Projectile.whoAmI), DustID.IceTorch, Vector2.Zero, 100, default, 1.2f).noGravity = true;
        }
    }

    /// <summary>Étoile de la Clamor Noxus : se plante 2 s sur un ennemi ou un bloc puis explose (135 % de l'arme ≈ x5,4 des 25 %) ; Moon Burnt.</summary>
    public class NoxStar : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.FallingStar;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 240;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.scale = 0.7f;
        }

        public override void AI()
        {
            Projectile.rotation += 0.2f;
            if (Projectile.ai[0] == 1f)
                Projectile.velocity = Vector2.Zero;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Stick();
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Nightwither>(), 180);
            Stick();
        }

        private void Stick()
        {
            if (Projectile.ai[0] == 1f)
                return;
            Projectile.ai[0] = 1f;
            Projectile.timeLeft = 120;
            Projectile.netUpdate = true;
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.ai[0] != 1f || Projectile.owner != Main.myPlayer)
                return;
            SoundEngine.PlaySound(SoundID.Item14 with { Volume = 0.4f }, Projectile.Center);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<NoxBlast>(), (int)(Projectile.damage * 5.4f), 0f, Projectile.owner);
        }
    }

    /// <summary>Explosion d'étoile ou de Noxling (80 px).</summary>
    public class NoxBlast : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 80;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 4;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (!Main.dedServ)
                for (int i = 0; i < 15; i++)
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleTorch, 0f, 0f, 100, default, 1.6f).velocity *= 2f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Nightwither>(), 180);
    }

    /// <summary>Noxling : tombe, fonce sur l'ennemi le plus proche, explose en 4 traits Exo (50 %) et un trou noir de 2 s qui attire.</summary>
    public class Noxling : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (++Projectile.localAI[0] > 20f)
                CalamityUtils.HomeInOnNPC(Projectile, true, 900f, 20f, 8f);
            if (!Main.dedServ)
                for (int i = 0; i < 2; i++)
                    Dust.NewDustPerfect(Projectile.Center, DustID.Shadowflame, Vector2.Zero, 100, default, 1.5f).noGravity = true;
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner != Main.myPlayer)
                return;
            for (int i = 0; i < 4; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (MathHelper.PiOver2 * i + MathHelper.PiOver4).ToRotationVector2() * 12f,
                    ModContent.ProjectileType<Weapons1119e.ExoElementalBolt>(), Projectile.damage / 2, 0f, Projectile.owner);
            int hole = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<Weapons1119j.SingularityHole>(),
                Projectile.damage / 4, 0f, Projectile.owner);
            if (Main.projectile.IndexInRange(hole))
            {
                Main.projectile[hole].timeLeft = 120;
                Main.projectile[hole].DamageType = DamageClass.Magic;
            }
        }
    }

    /// <summary>
    /// Disque de Lunatic Beats : boomerang qui perce 10 fois ; en vol, un pouvoir céleste au hasard toutes les 12 images ;
    /// dégâts x1,5 au retour.
    /// </summary>
    public class LunaticDisc : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Melee/LunaticBeats";

        private bool Returning => Projectile.ai[0] == 1f;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 34;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            Projectile.rotation += 0.4f;
            Projectile.ai[1]++;
            if (!Returning && (Projectile.ai[1] > 40f || Projectile.localAI[0] >= 10f))
            {
                Projectile.ai[0] = 1f;
                Projectile.netUpdate = true;
            }
            if (Returning)
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(owner.Center) * 30f, 0.15f);
                if (Projectile.Distance(owner.Center) < 40f)
                    Projectile.Kill();
            }
            if (Projectile.owner != Main.myPlayer || Projectile.ai[1] % 12 != 0)
                return;
            var source = Projectile.GetSource_FromThis();
            int dmg = (int)(Projectile.damage * 0.4f);
            switch (Main.rand.Next(4))
            {
                case 0:
                    Projectile.NewProjectile(source, Projectile.Center, Main.rand.NextVector2CircularEdge(10f, 10f), ProjectileID.SolarWhipSwordExplosion, dmg, 0f, Projectile.owner);
                    break;
                case 1:
                    NPC target = Projectile.Center.ClosestNPCAt(600f);
                    if (target != null)
                        Projectile.NewProjectile(source, Projectile.Center, Projectile.SafeDirectionTo(target.Center) * 10f, ProjectileID.VortexBeaterRocket, dmg, 0f, Projectile.owner);
                    break;
                case 2:
                    Projectile.NewProjectile(source, Projectile.Center, Main.rand.NextVector2CircularEdge(6f, 6f), ProjectileID.NebulaArcanumSubshot, dmg, 0f, Projectile.owner);
                    break;
                default:
                    NPC above = Projectile.Center.ClosestNPCAt(600f);
                    if (above != null)
                        Projectile.NewProjectile(source, above.Center - new Vector2(0f, 400f), new Vector2(0f, 16f), ProjectileID.StarWrath,
                            dmg, 0f, Projectile.owner);
                    break;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Returning)
                modifiers.SourceDamage *= 1.5f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => Projectile.localAI[0]++;
    }
}
