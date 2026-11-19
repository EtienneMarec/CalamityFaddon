using System;
using CalamityFandom.Content.Projectiles.Weapons1119f;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
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

// Lot I (19/11) : pages d'addon (Community Remix selon le Fandom), traitées comme les autres pages (décisions, section 11).
// Ce sont des ingrédients de recettes du wiki déjà faites (Interfacer Staff, Exif Fist, Twenty-Twloon, Elemental Bow,
// Pastranion). Code écrit pour ce mod d'après les pages (rien de CalRemix).

namespace CalamityFandom.Content.Items.Weapons.Rogue
{
    /// <summary>
    /// Wiki : Bucket of Coal.txt — seau soumis à la gravité qui tourne et lance un morceau de charbon toutes les 20 images
    /// (10 % des dégâts) ; petite explosion noire au contact d'un ennemi ; frappe furtive : les morceaux grossissent (x1,05)
    /// et gagnent 1 dégât par bloc parcouru.
    /// </summary>
    public class BucketofCoal : RogueWeapon
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 90;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.useTime = Item.useAnimation = 62;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = Item.noUseGraphic = true;
            Item.knockBack = 6.5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = 24;
            Item.height = 28;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 6);
            Item.shoot = ModContent.ProjectileType<CoalBucket>();
            Item.shootSpeed = 12f;
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
    /// Wiki : Fracturing Fist.txt — poing rapide (traînée de particules, un peu de gravité) qui explose au contact d'un ennemi
    /// ou d'un bloc en deux moitiés chercheuses (40 %) ; l'explosion ignore l'invincibilité des ennemis ; frappe furtive : le
    /// poing rebondit 4 fois entre les ennemis en lâchant des éclats de cristal (10 %) avant d'exploser.
    /// </summary>
    public class FracturingFist : RogueWeapon
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 63;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.useTime = Item.useAnimation = 28;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = Item.noUseGraphic = true;
            Item.knockBack = 6f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = 28;
            Item.height = 36;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 2);
            Item.shoot = ModContent.ProjectileType<FracturingFistProj>();
            Item.shootSpeed = 5f;
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
    /// Wiki : Contaminated Dish.txt — vieille boîte de Petri qui explose sur un ennemi ou un bloc : éclats de verre sans
    /// dégâts et nuages de nanorobots (65 %, invincibilité de 5 images au lieu de 10, Plague 5 s) ; frappe furtive : lancée
    /// plus fort, les éclats de verre se plantent (Bleeding 5 s). Butin du Plaguebringer Goliath.
    /// </summary>
    public class ContaminatedDish : RogueWeapon
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 80;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = Item.noUseGraphic = true;
            Item.knockBack = 2f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = 44;
            Item.height = 32;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 10);
            Item.shoot = ModContent.ProjectileType<PetriDish>();
            Item.shootSpeed = 13f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool stealth = player.Calamity().StealthStrikeAvailable();
            int p = Projectile.NewProjectile(source, position, stealth ? velocity * 1.6f : velocity, type, damage, knockback, player.whoAmI);
            if (stealth && Main.projectile.IndexInRange(p))
                Main.projectile[p].Calamity().stealthStrike = true;
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Ranged
{
    /// <summary>
    /// Wiki : Flaming Ice Bow.txt — toutes les flèches infligent Frostburn 3 s ; le joueur perd 1 PV par image tant qu'il le
    /// tient (ses mains touchent le feu), sans protection possible.
    /// </summary>
    public class FlamingIceBow : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 23;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item5;
            Item.width = 26;
            Item.height = 36;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 2, silver: 40);
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 18f;
            Item.useAmmo = AmmoID.Arrow;
        }

        public override void HoldItem(Player player)
        {
            if (player.whoAmI != Main.myPlayer || player.dead || player.immune && player.immuneNoBlink)
                return;
            player.statLife--;
            if (Main.rand.NextBool(4))
                Dust.NewDustDirect(player.itemLocation - new Vector2(4f), 8, 8, DustID.Torch, 0f, -1f, 100, default, 1.2f).noGravity = true;
            if (player.statLife <= 0)
                player.KillMe(PlayerDeathReason.ByCustomReason(Terraria.Localization.NetworkText.FromKey("Mods.CalamityFandom.Items.FlamingIceBow.Death", player.name)), 1.0, 0);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (Main.projectile.IndexInRange(p))
                Main.projectile[p].GetGlobalProjectile<WikiArrowDebuff>().Frostburn = true;
            return false;
        }
    }

    /// <summary>Flèches du Flaming Ice Bow : Frostburn 3 s.</summary>
    public class WikiArrowDebuff : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public bool Frostburn;

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Frostburn)
                target.AddBuff(BuffID.Frostburn, 180);
        }
    }

    /// <summary>
    /// Wiki : Baow.txt — deux flèches : l'une part de l'arc, l'autre du côté opposé du curseur, et elles se rejoignent au
    /// curseur (yin et yang). [choix] La seconde part du point symétrique du joueur par rapport au curseur.
    /// </summary>
    public class Baow : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 27;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item5;
            Item.width = 36;
            Item.height = 64;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 5, silver: 20);
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 14f;
            Item.useAmmo = AmmoID.Arrow;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            Vector2 mouse = Main.MouseWorld;
            Vector2 mirror = mouse + (mouse - position);
            Projectile.NewProjectile(source, mirror, (mouse - mirror).SafeNormalize(-velocity) * velocity.Length(), type, damage, knockback, player.whoAmI);
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Ammo
{
    /// <summary>
    /// Wiki : 22 Hornet Round.txt — balle qui, à chaque ennemi touché, repart vers l'ennemi le plus proche, jusqu'au 22e
    /// (21 ricochets) ; sinon continue tout droit.
    /// </summary>
    public class TwentyTwoHornetRound : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 99;

        public override void SetDefaults()
        {
            Item.damage = 22;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 14;
            Item.height = 30;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.knockBack = 11f;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
            Item.value = Item.sellPrice(silver: 1);
            Item.shoot = ModContent.ProjectileType<HornetRoundBullet>();
            Item.shootSpeed = 11f;
            Item.ammo = AmmoID.Bullet;
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Weapons1119f
{
    /// <summary>Seau du Bucket of Coal : gravité, tourne, lance un charbon toutes les 20 images ; explosion noire au contact d'un ennemi.</summary>
    public class CoalBucket : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Rogue/BucketofCoal";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = 1;
            Projectile.timeLeft = 240;
        }

        public override void AI()
        {
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.2f, 14f);
            Projectile.rotation += 0.25f * Math.Sign(Projectile.velocity.X == 0 ? 1 : Projectile.velocity.X);
            if (Projectile.owner == Main.myPlayer && ++Projectile.ai[0] % 20f == 0f)
            {
                int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * 7f,
                    ModContent.ProjectileType<CoalChunk>(), Math.Max(1, Projectile.damage / 10), 1f, Projectile.owner);
                if (Projectile.Calamity().stealthStrike && Main.projectile.IndexInRange(p))
                    Main.projectile[p].Calamity().stealthStrike = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<CoalBlast>(), Projectile.damage / 2, 0f, Projectile.owner);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            for (int i = 0; i < 10; i++)
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Ash, 0f, 0f, 100, default, 1.3f);
        }
    }

    /// <summary>Morceau de charbon : gravité ; frappe furtive : grossit x1,05 et gagne 1 dégât par bloc parcouru.</summary>
    public class CoalChunk : ModProjectile
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.Coal;

        private float travelled;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.scale = 0.6f;
        }

        public override void AI()
        {
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.15f, 12f);
            Projectile.rotation += 0.2f;
            if (!Projectile.Calamity().stealthStrike)
                return;
            travelled += Projectile.velocity.Length();
            while (travelled >= 16f)
            {
                travelled -= 16f;
                Projectile.damage++;
                Projectile.scale = Math.Min(Projectile.scale * 1.05f, 4f);
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            int size = (int)(12 * Projectile.scale / 0.6f);
            var box = new Rectangle((int)Projectile.Center.X - size / 2, (int)Projectile.Center.Y - size / 2, size, size);
            return box.Intersects(targetHitbox);
        }
    }

    /// <summary>Petite explosion noire du seau (60 px).</summary>
    public class CoalBlast : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 5;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.Item14 with { Volume = 0.4f }, Projectile.Center);
            if (!Main.dedServ)
                for (int i = 0; i < 20; i++)
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, Color.Black, 1.8f).velocity *= 1.5f;
        }
    }

    /// <summary>
    /// Poing du Fracturing Fist : un peu de gravité, traînée de particules qui restent ; explose en deux moitiés chercheuses.
    /// Furtif : rebondit vers l'ennemi le plus proche 4 fois (éclats de cristal à chaque rebond) avant d'exploser.
    /// </summary>
    public class FracturingFistProj : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Rogue/FracturingFist";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Projectile.Calamity().stealthStrike)
                Projectile.penetrate = 5;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.03f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            if (!Main.dedServ)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.PinkCrystalShard, Vector2.Zero, 100, default, 1.1f);
                d.noGravity = true;
                d.fadeIn = 1.4f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!Projectile.Calamity().stealthStrike || Projectile.penetrate <= 1 || Projectile.owner != Main.myPlayer)
                return;
            // Rebond vers un autre ennemi, éclats de cristal
            for (int i = 0; i < 3; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Circular(5f, 5f), ProjectileID.CrystalShard,
                    Math.Max(1, Projectile.damage / 10), 0f, Projectile.owner);
            NPC next = null;
            float best = 600f;
            foreach (NPC n in Main.ActiveNPCs)
                if (n.whoAmI != target.whoAmI && n.CanBeChasedBy(Projectile) && n.Distance(Projectile.Center) < best)
                {
                    best = n.Distance(Projectile.Center);
                    next = n;
                }
            if (next != null)
                Projectile.velocity = Projectile.SafeDirectionTo(next.Center) * Projectile.velocity.Length();
            else
                Projectile.penetrate = 1;
            Projectile.netUpdate = true;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);
            if (!Main.dedServ)
                for (int i = 0; i < 15; i++)
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.PinkCrystalShard, 0f, 0f, 100, default, 1.4f).velocity *= 2f;
            if (Projectile.owner != Main.myPlayer)
                return;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FistBlast>(), Projectile.damage, 0f, Projectile.owner);
            for (int i = -1; i <= 1; i += 2)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, -Projectile.velocity.RotatedBy(i * 0.8f) * 0.8f, ModContent.ProjectileType<FistHalf>(),
                    (int)(Projectile.damage * 0.4f), 0f, Projectile.owner);
        }
    }

    /// <summary>
    /// Explosion du Fracturing Fist (80 px). Elle ignore l'invincibilité des ennemis : son immunité est locale (elle frappe
    /// même un ennemi que le poing vient de toucher).
    /// </summary>
    public class FistBlast : CoalBlast
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 80;
        }

        public override void OnSpawn(IEntitySource source)
        {
        }
    }

    /// <summary>Moitié du Fracturing Fist : cherche vite les ennemis (40 %).</summary>
    public class FistHalf : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Rogue/FracturingFist";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.scale = 0.6f;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            CalamityUtils.HomeInOnNPC(Projectile, true, 600f, 12f, 10f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
        }
    }

    /// <summary>Boîte de Petri : explose sur un ennemi ou un bloc ; 3 nuages de nanorobots, éclats de verre (Bleeding en furtif).</summary>
    public class PetriDish : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Rogue/PetriDish";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.2f, 14f);
            Projectile.rotation += 0.2f;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Shatter, Projectile.Center);
            if (Projectile.owner != Main.myPlayer)
                return;
            bool stealth = Projectile.Calamity().stealthStrike;
            for (int i = 0; i < 3; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Circular(1.5f, 1.5f), ModContent.ProjectileType<NanobotCloud>(),
                    (int)(Projectile.damage * 0.65f), 0f, Projectile.owner);
            for (int i = 0; i < 5; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2CircularEdge(6f, 6f), ModContent.ProjectileType<GlassShard>(),
                    stealth ? Math.Max(1, Projectile.damage / 10) : 0, 0f, Projectile.owner, stealth ? 1f : 0f);
        }
    }

    /// <summary>Nuage de nanorobots : 1 s, invincibilité de 5 images, Plague 5 s.</summary>
    public class NanobotCloud : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.97f;
            if (!Main.dedServ)
                for (int i = 0; i < 2; i++)
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GreenFairy, 0f, 0f, 120, default, 1f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Plague>(), 300);
    }

    /// <summary>Éclat de verre de la boîte de Petri : sans dégâts, sauf en frappe furtive (ai[0] = 1 : Bleeding 5 s).</summary>
    public class GlassShard : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Rogue/GlassShards";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = 1;
            Projectile.timeLeft = 60;
            Projectile.scale = 0.6f;
        }

        public override bool? CanDamage() => Projectile.ai[0] == 1f && Projectile.damage > 0 ? null : false;

        public override void AI()
        {
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.25f, 12f);
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Bleeding, 300);
    }

    /// <summary>Balle .22 Hornet : à chaque ennemi touché, repart vers l'ennemi le plus proche (22 au plus).</summary>
    public class HornetRoundBullet : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Ranged/HornetRoundBullet";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 22;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            NPC next = null;
            float best = 1000f;
            foreach (NPC n in Main.ActiveNPCs)
                if (n.whoAmI != target.whoAmI && Projectile.localNPCImmunity[n.whoAmI] == 0 && n.CanBeChasedBy(Projectile) && n.Distance(Projectile.Center) < best)
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

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, new Color(255, 230, 120, 0), Projectile.rotation, new Vector2(tex.Width / 2f, 0f), new Vector2(1f, 0.3f), SpriteEffects.None);
            return false;
        }
    }
}
