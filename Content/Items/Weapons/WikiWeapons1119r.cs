using System;
using CalamityFandom.Content.Projectiles.Weapons1119r;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

// Lot V (19/11) : objets du wiki fabricables à l'étape « ERM » de l'Aquatic Hypercomputer (recettes du wiki). Raretés
// « C1 », « C2 » inventées : 0 (règle des stats, décisions section 3). Evercold : debuff de ce mod (Aquatic Hypercomputer).

namespace CalamityFandom.Content.Items.Weapons.Ranged
{
    /// <summary>Wiki : Coldetecht.txt — balles très rapides, dispersion de 10°, Evercold 5 s.</summary>
    public class Coldetecht : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 160;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 2;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item11;
            Item.width = 92;
            Item.height = 34;
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(platinum: 1);
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 30f;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-16f, 0f);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int p = Projectile.NewProjectile(source, position, velocity.RotatedByRandom(MathHelper.ToRadians(5f)), type, damage, knockback, player.whoAmI);
            if (Main.projectile.IndexInRange(p))
                Main.projectile[p].GetGlobalProjectile<EvercoldShot>().Active = true;
            return false;
        }
    }

    /// <summary>Wiki : Gluehunter.txt — tire lentement des balles très rapides qui cherchent les ennemis ; Evercold 5 s.</summary>
    public class Gluehunter : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 1560;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item40;
            Item.width = 82;
            Item.height = 30;
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(platinum: 1);
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 30f;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-14f, 0f);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (Main.projectile.IndexInRange(p))
            {
                Main.projectile[p].GetGlobalProjectile<EvercoldShot>().Active = true;
                Main.projectile[p].GetGlobalProjectile<EvercoldShot>().Homing = true;
            }
            return false;
        }
    }

    /// <summary>Tirs qui infligent Evercold 5 s (et cherchent les ennemis pour le Gluehunter).</summary>
    public class EvercoldShot : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public bool Active, Homing;

        public override void AI(Projectile projectile)
        {
            if (Homing)
                CalamityUtils.HomeInOnNPC(projectile, !projectile.tileCollide, 900f, Math.Max(projectile.velocity.Length(), 12f), 10f);
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Active)
                target.AddBuff(ModContent.BuffType<Buffs.Evercold>(), 300);
        }
    }

    /// <summary>
    /// Wiki : Hyperhavoc.txt — lance-flammes : rayon de flammes abyssales et sacrées qui perce les ennemis et traverse 3 blocs ;
    /// Holy Flames et Abyssal Flames 4 s ; cadence fixe. Grande version du Microhavoc.
    /// </summary>
    public class Hyperhavoc : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 360;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 6;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item34;
            Item.width = 102;
            Item.height = 40;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
            Item.value = Item.sellPrice(platinum: 1);
            Item.shoot = ModContent.ProjectileType<HavocBeam>();
            Item.shootSpeed = 11f;
            Item.useAmmo = AmmoID.Gel;
        }

        public override float UseSpeedMultiplier(Player player) => 1f;

        public override bool CanConsumeAmmo(Item ammo, Player player) => player.itemAnimation >= player.itemAnimationMax - 1;

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) =>
            type = ModContent.ProjectileType<HavocBeam>();
    }

    /// <summary>
    /// Wiki : Coldant Firestarter.txt — lance-flammes de flammes bleu clair très rapide (Evercold 5 s), 90 % de ne pas utiliser
    /// de gel ; clic droit maintenu : les flammes tournent autour du joueur et lui donnent de la défense.
    /// [choix] Défense +20 tant que des flammes tournent ; cercle de 80 px.
    /// </summary>
    public class ColdantFirestarter : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 240;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 6;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item34;
            Item.width = 68;
            Item.height = 28;
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(platinum: 1);
            Item.shoot = ModContent.ProjectileType<ColdantFlame>();
            Item.shootSpeed = 14f;
            Item.useAmmo = AmmoID.Gel;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.Next(100) >= 90;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<ColdantFlame>(), damage, knockback, player.whoAmI, player.altFunctionUse == 2 ? 1f : 0f,
                Main.rand.NextFloat(MathHelper.TwoPi));
            return false;
        }
    }

    /// <summary>Défense du Coldant Firestarter : +20 tant qu'au moins une flamme tourne (une seule fois, quel que soit leur nombre).</summary>
    public class ColdantShieldPlayer : ModPlayer
    {
        public int Timer;

        public override void PostUpdateEquips()
        {
            if (Timer <= 0)
                return;
            Timer--;
            Player.statDefense += 20;
        }
    }

    /// <summary>
    /// Wiki : Abysscent Derailer.txt — les roquettes deviennent des bombes très rapides qui explosent (ennemi ou mur) en 5 étoiles
    /// à 4 branches lentement chercheuses (150 %, Evercold 5 s). Recette du wiki, butin de l'Esserwyrm.
    /// </summary>
    public class AbysscentDerailer : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 1740;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6.5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item61;
            Item.width = 76;
            Item.height = 30;
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(gold: 75);
            Item.shoot = ProjectileID.RocketI;
            Item.shootSpeed = 20f;
            Item.useAmmo = AmmoID.Rocket;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<DerailerBomb>(), damage, knockback, player.whoAmI);
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Magic
{
    /// <summary>
    /// Wiki : Noctic Weaver.txt — toiles qui collent aux ennemis touchés, les ralentissent et font 50 % des dégâts chaque
    /// seconde par toile collée ; Evercold 5 s. [choix] Toile collée 5 s.
    /// </summary>
    public class NocticWeaver : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 520;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 40;
            Item.useTime = Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item17;
            Item.width = 63;
            Item.height = 64;
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(gold: 40);
            Item.shoot = ModContent.ProjectileType<LacelicWeb>();
            Item.shootSpeed = 12f;
        }
    }

    /// <summary>
    /// Wiki : Waterfrosia.txt — une pluie tombe du ciel ; juste avant le sol, les gouttes repartent vers le haut et
    /// disparaissent au plafond ; Evercold 5 s. [choix] 2 gouttes par utilisation au-dessus du curseur.
    /// </summary>
    public class Waterfrosia : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 240;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 5;
            Item.useTime = Item.useAnimation = 2;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.knockBack = 0f;
            Item.autoReuse = true;
            Item.width = 40;
            Item.height = 42;
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(platinum: 1);
            Item.shoot = ModContent.ProjectileType<FrosiaRain>();
            Item.shootSpeed = 12f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 2; i++)
                Projectile.NewProjectile(source, new Vector2(Main.MouseWorld.X + Main.rand.Next(-160, 161), Main.screenPosition.Y - 20f), new Vector2(0f, 14f), type, damage, knockback, player.whoAmI);
            return false;
        }
    }

    /// <summary>
    /// Wiki : Nocturnal Odyssey.txt — 5 orbes tournent autour de l'ennemi ; un orbe explose chaque seconde et le cercle
    /// rétrécit ; si l'ennemi est plus gros que le cercle, tous explosent d'un coup à 50 %. [choix] Cercle de 160 px.
    /// </summary>
    public class NocturnalOdyssey : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 460;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 55;
            Item.useTime = Item.useAnimation = 120;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.knockBack = 0f;
            Item.UseSound = SoundID.Item8;
            Item.width = 38;
            Item.height = 36;
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(gold: 50);
            Item.shoot = ModContent.ProjectileType<OdysseyOrb>();
            Item.shootSpeed = 1f;
        }

        public override bool CanUseItem(Player player) => Main.MouseWorld.ClosestNPCAt(400f) != null;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            NPC target = Main.MouseWorld.ClosestNPCAt(400f);
            if (target == null)
                return false;
            for (int i = 0; i < 5; i++)
                Projectile.NewProjectile(source, target.Center, Vector2.Zero, type, damage, knockback, player.whoAmI, target.whoAmI, i);
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Accessories
{
    /// <summary>Wiki : Silva Module.txt — 15 défense, +20 % de dégâts, +20 % de réduction des dégâts.</summary>
    public class SilvaModule : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = Item.height = 38;
            Item.accessory = true;
            Item.defense = 15;
            Item.rare = ModContent.RarityType<CosmicPurple>();
            Item.value = Item.sellPrice(platinum: 1);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Generic) += 0.2f;
            player.endurance += 0.2f;
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Weapons1119r
{
    /// <summary>Rayon du Hyperhavoc : perce les ennemis, traverse 3 blocs, Holy Flames 4 s.</summary>
    public class HavocBeam : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private int tilesCrossed;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 35;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Point t = Projectile.Center.ToTileCoordinates();
            if (WorldGen.InWorld(t.X, t.Y) && WorldGen.SolidTile(t.X, t.Y) && Projectile.localAI[0] != t.X * 10000 + t.Y)
            {
                Projectile.localAI[0] = t.X * 10000 + t.Y;
                if (++tilesCrossed > 3)
                {
                    Projectile.Kill();
                    return;
                }
            }
            if (!Main.dedServ)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.GoldFlame, Projectile.velocity * 0.1f, 100, default, 1.7f).noGravity = true;
                Dust.NewDustPerfect(Projectile.Center, DustID.Shadowflame, Projectile.velocity * 0.1f, 100, default, 1.2f).noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 240);
            target.AddBuff(ModContent.BuffType<Buffs.AbyssalFlames>(), 240);
        }
    }

    /// <summary>Flamme du Coldant Firestarter : ai[0] = 1 : tourne autour du joueur (angle de départ ai[1]) et lui donne de la défense.</summary>
    public class ColdantFlame : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 40;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 1f)
            {
                Player owner = Main.player[Projectile.owner];
                Projectile.ai[1] += 0.15f;
                Projectile.Center = owner.Center + Projectile.ai[1].ToRotationVector2() * 80f;
                owner.GetModPlayer<Items.Weapons.Ranged.ColdantShieldPlayer>().Timer = 2;
            }
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.IceTorch, Vector2.Zero, 100, default, 1.6f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Buffs.Evercold>(), 300);
    }

    /// <summary>Bombe de l'Abysscent Derailer : très rapide, explose en 5 étoiles chercheuses (150 %).</summary>
    public class DerailerBomb : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.RocketI;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.extraUpdates = 1;
        }

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        public override Color? GetAlpha(Color lightColor) => new Color(120, 200, 255);

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            if (Projectile.owner != Main.myPlayer)
                return;
            for (int i = 0; i < 5; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (MathHelper.TwoPi * i / 5f).ToRotationVector2() * 6f, ModContent.ProjectileType<AbyssStar>(),
                    (int)(Projectile.damage * 1.5f), 0f, Projectile.owner);
        }
    }

    /// <summary>Étoile à 4 branches : cherche lentement, Evercold 5 s.</summary>
    public class AbyssStar : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.HallowStar;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            CalamityUtils.HomeInOnNPC(Projectile, true, 800f, 8f, 25f);
            Projectile.rotation += 0.15f;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(120, 200, 255, 200);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Buffs.Evercold>(), 300);
    }

    /// <summary>Toile du Noctic Weaver : colle à l'ennemi touché 5 s, le ralentit, 50 % des dégâts chaque seconde.</summary>
    public class LacelicWeb : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.WebSpit;

        private int stuckTo = -1;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
        }

        public override void AI()
        {
            if (stuckTo < 0)
                return;
            NPC n = Main.npc[stuckTo];
            if (!n.active)
            {
                Projectile.Kill();
                return;
            }
            Projectile.Center = n.Center + new Vector2(Projectile.localAI[0], Projectile.localAI[1]);
            Projectile.velocity = Vector2.Zero;
            if (!n.boss)
                n.velocity *= 0.9f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Buffs.Evercold>(), 300);
            if (stuckTo >= 0)
                return;
            stuckTo = target.whoAmI;
            Projectile.localAI[0] = Projectile.Center.X - target.Center.X;
            Projectile.localAI[1] = Projectile.Center.Y - target.Center.Y;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.damage /= 2;
            Projectile.netUpdate = true;
        }
    }

    /// <summary>Goutte de la Waterfrosia : tombe, et juste avant un bloc repart vers le haut ; disparaît au plafond ; Evercold 5 s.</summary>
    public class FrosiaRain : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.RainFriendly;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0f)
            {
                // Cherche un bloc juste en dessous : demi-tour
                Point below = (Projectile.Center + new Vector2(0f, 24f)).ToTileCoordinates();
                if (Projectile.timeLeft < 290 && WorldGen.InWorld(below.X, below.Y) && WorldGen.SolidTile(below.X, below.Y))
                {
                    Projectile.ai[0] = 1f;
                    Projectile.velocity.Y = -Math.Abs(Projectile.velocity.Y);
                    Projectile.netUpdate = true;
                }
            }
            else
            {
                Point above = (Projectile.Center - new Vector2(0f, 8f)).ToTileCoordinates();
                if (WorldGen.InWorld(above.X, above.Y) && WorldGen.SolidTile(above.X, above.Y))
                    Projectile.Kill();
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(150, 220, 255, 200);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Buffs.Evercold>(), 300);
    }

    /// <summary>
    /// Orbe de la Nocturnal Odyssey (ai[0] = cible, ai[1] = rang 0-4) : tourne autour de la cible ; l'orbe de rang k explose
    /// à k+1 secondes et le cercle rétrécit ; si la cible est plus grosse que le cercle, tous explosent à 50 %.
    /// </summary>
    public class OdysseyOrb : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 400;
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            NPC target = Main.npc[(int)Projectile.ai[0]];
            if (!target.active)
            {
                Projectile.Kill();
                return;
            }
            Projectile.localAI[0]++;
            float radius = Math.Max(20f, 160f - 28f * (Projectile.localAI[0] / 60f));
            float angle = Projectile.localAI[0] * 0.08f + MathHelper.TwoPi * Projectile.ai[1] / 5f;
            Projectile.Center = target.Center + angle.ToRotationVector2() * radius;
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.PurpleTorch, Vector2.Zero, 100, default, 1.4f).noGravity = true;
            bool tooBig = Math.Max(target.width, target.height) / 2f > radius;
            if (Projectile.owner == Main.myPlayer && (Projectile.localAI[0] >= 60f * (Projectile.ai[1] + 1f) || tooBig))
            {
                SoundEngine.PlaySound(SoundID.Item14 with { Volume = 0.4f }, Projectile.Center);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<OdysseyBlast>(),
                    tooBig ? Projectile.damage / 2 : Projectile.damage, 0f, Projectile.owner);
                Projectile.Kill();
            }
        }
    }

    /// <summary>Explosion d'un orbe de la Nocturnal Odyssey (60 px).</summary>
    public class OdysseyBlast : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 60;
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
                for (int i = 0; i < 12; i++)
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleTorch, 0f, 0f, 100, default, 1.5f).velocity *= 2f;
        }
    }
}
