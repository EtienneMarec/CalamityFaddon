using System;
using CalamityFandom.Content.Projectiles.Weapons1119i;
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

// Lot L (19/11) : armes du wiki qui sont des ingrédients de recettes (pages d'addon comprises, décisions section 11).
// Sprites et valeurs des pages, recettes du wiki (WikiRecipes).

namespace CalamityFandom.Content.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Onyx Gunblade.txt — épée qui fait aussi des dégâts à distance : avec des balles, chaque coup tire un tir d'onyx
    /// (comme l'Onyx Blaster) et 2 à 7 balles en éventail ; sans balles, rien de plus.
    /// [choix] Les balles ne sont pas consommées (la page ne le dit pas) ; tirs à distance au taux « ranged » du joueur.
    /// </summary>
    public class OnyxGunblade : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 25;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = Item.height = 60;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(silver: 80);
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 6f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Item bullet = player.ChooseAmmo(ContentSamples.ItemsByType[ItemID.Minishark]);
            if (bullet == null || bullet.IsAir)
                return false;
            int ranged = (int)player.GetTotalDamage(DamageClass.Ranged).ApplyTo(Item.OriginalDamage + bullet.damage);
            Vector2 aim = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX * player.direction) * 10f;
            Projectile.NewProjectile(source, player.Center, aim, ProjectileID.BlackBolt, (int)(ranged * 1.45f), knockback * 2f, player.whoAmI);
            int shot = bullet.shoot > ProjectileID.None ? bullet.shoot : ProjectileID.Bullet;
            for (int i = 0; i < Main.rand.Next(2, 8); i++)
                Projectile.NewProjectile(source, player.Center, aim.RotatedByRandom(0.25f) * Main.rand.NextFloat(0.9f, 1.4f), shot, ranged, knockback, player.whoAmI);
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Magic
{
    /// <summary>Wiki : Onyx Staff.txt — trois traits noirs qui font de petites explosions sur les blocs et les ennemis.</summary>
    public class OnyxStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 25;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 3;
            Item.useTime = Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item43;
            Item.width = 42;
            Item.height = 40;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(silver: 80);
            Item.shoot = ModContent.ProjectileType<OnyxBolt>();
            Item.shootSpeed = 14f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = -1; i <= 1; i++)
                Projectile.NewProjectile(source, position, velocity.RotatedBy(0.12f * i), type, damage, knockback, player.whoAmI);
            return false;
        }
    }

    /// <summary>
    /// Wiki : Thunder Bolt.txt — boule d'éclair en ligne droite, sans gravité, faible lumière : 4 ennemis ou 4 rebonds sur les
    /// murs ; Electrified 3 s.
    /// </summary>
    public class ThunderBolt : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 54;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 12;
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5.5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item21;
            Item.width = 34;
            Item.height = 42;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(silver: 90);
            Item.shoot = ModContent.ProjectileType<BoltBall>();
            Item.shootSpeed = 7.5f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, (int)BoltBall.Kind.Thunder);
            return false;
        }
    }

    /// <summary>Wiki : Aero Bolt.txt — boule d'air lente en ligne droite, sans gravité : 3 ennemis ou 3 rebonds sur les murs.</summary>
    public class AeroBolt : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 35;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 12;
            Item.useTime = Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item21;
            Item.width = 42;
            Item.height = 44;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 2, silver: 40);
            Item.shoot = ModContent.ProjectileType<BoltBall>();
            Item.shootSpeed = 5.5f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, (int)BoltBall.Kind.Aero);
            return false;
        }
    }

    /// <summary>
    /// Wiki : Plasmaflash Bolt.txt — boule de plasma rapide en ligne droite, sans gravité, lumineuse, traverse 10 ennemis ;
    /// Electrified 10 s. Amélioration du Thunder Bolt et de l'Aero Bolt.
    /// </summary>
    public class PlasmaflashBolt : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 45;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 13;
            Item.useTime = Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5.5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item21;
            Item.width = 48;
            Item.height = 44;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 2, silver: 40);
            Item.shoot = ModContent.ProjectileType<BoltBall>();
            Item.shootSpeed = 6.5f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity * 2f, type, damage, knockback, player.whoAmI, (int)BoltBall.Kind.Plasma);
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Ranged
{
    /// <summary>
    /// Wiki : Rose Bow.txt — flot rapide de flèches en petit éventail au hasard ; les Wooden Arrows deviennent des flèches
    /// d'énergie qui entaillent l'ennemi touché 3 fois (30 % chacune) ; Venom 1 à 8 s.
    /// </summary>
    public class RoseBow : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 98;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 6;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item5;
            Item.width = 54;
            Item.height = 114;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.value = Item.sellPrice(gold: 28);
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 23f;
            Item.useAmmo = AmmoID.Arrow;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.RotatedByRandom(0.06f);
            if (type == ProjectileID.WoodenArrowFriendly)
                type = ModContent.ProjectileType<RoseArrow>();
        }
    }

    /// <summary>
    /// Wiki : Pumplerfaust.txt — maintenir : une citrouille blanche gonfle au bout du canon (5 s, puis elle s'allume) ;
    /// relâcher : elle part vers le curseur et explose au contact ; pleine, énorme explosion et brume qui blesse ; sinon,
    /// explosion bien plus petite sans brume. [choix] Explosion de 60 à 160 px, brume de 5 s à 30 %.
    /// </summary>
    public class Pumplerfaust : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 102;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 42;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.channel = true;
            Item.knockBack = 5f;
            Item.autoReuse = true;
            Item.width = 100;
            Item.height = 36;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 1);
            Item.shoot = ModContent.ProjectileType<WhitePumpkin>();
            Item.shootSpeed = 17f;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-20f, 0f);

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;
    }

    /// <summary>
    /// Wiki : Gourdler.txt — maintenir : charge jusqu'à 8 grenades-courges (le canon grossit) ; relâcher : les tire toutes,
    /// soumises à la gravité, qui explosent au contact d'un ennemi en une gerbe d'épines. [choix] Une grenade de plus toutes
    /// les 36 images (la valeur « use »), 6 épines à 30 %.
    /// </summary>
    public class Gourdler : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 86;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.channel = true;
            Item.knockBack = 3f;
            Item.autoReuse = true;
            Item.width = 54;
            Item.height = 30;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(silver: 80);
            Item.shoot = ModContent.ProjectileType<GourdlerHoldout>();
            Item.shootSpeed = 15f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;
    }
}

namespace CalamityFandom.Content.Items.Weapons.Rogue
{
    /// <summary>
    /// Wiki : Celestial Nuke.txt — bombe de poche qui explose au contact et laisse une grande aura étoilée (55 %), des braises
    /// de God Slayer (40 %, Embers of Duality 5 s) et des traits célestes (80 %) ; frappe furtive : plus mortelle.
    /// [choix] Aura de 300 px pendant 3 s ; 8 braises, 4 traits chercheurs ; « Embers of Duality » = God Slayer Inferno de
    /// Calamity ; frappe furtive : tout x1,5 et deux fois plus de braises et de traits.
    /// </summary>
    public class CelestialNuke : RogueWeapon
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 175;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.useTime = Item.useAnimation = 65;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = Item.noUseGraphic = true;
            Item.knockBack = 3f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = 44;
            Item.height = 26;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.value = Item.sellPrice(gold: 10);
            Item.shoot = ModContent.ProjectileType<CelestialNukeProj>();
            Item.shootSpeed = 13f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool stealth = player.Calamity().StealthStrikeAvailable();
            int p = Projectile.NewProjectile(source, position, velocity, type, stealth ? (int)(damage * 1.5f) : damage, knockback, player.whoAmI);
            if (stealth && Main.projectile.IndexInRange(p))
                Main.projectile[p].Calamity().stealthStrike = true;
            return false;
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Weapons1119i
{
    /// <summary>Trait d'onyx de l'Onyx Staff : petite explosion (60 px, moitié des dégâts) sur un bloc ou un ennemi.</summary>
    public class OnyxBolt : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.BlackBolt;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 90;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.Shadowflame, Vector2.Zero, 100, default, 1f).noGravity = true;
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14 with { Volume = 0.3f }, Projectile.Center);
            Projectile.ExpandHitboxBy(60);
            Projectile.maxPenetrate = Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.damage /= 2;
            Projectile.Damage();
            if (!Main.dedServ)
                for (int i = 0; i < 10; i++)
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Shadowflame, 0f, 0f, 100, default, 1.3f).velocity *= 1.5f;
        }
    }

    /// <summary>Boule des tomes : ai[0] = Kind. Thunder : 4 ennemis / 4 rebonds, Electrified 3 s ; Aero : 3 / 3 ; Plasma : 10 ennemis, Electrified 10 s.</summary>
    public class BoltBall : ModProjectile
    {
        public enum Kind { Thunder, Aero, Plasma }

        public override string Texture => "CalamityFandom/Content/Projectiles/Magic/PlasmaflashBall";

        private Kind K => (Kind)(int)Projectile.ai[0];
        private int bounces;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 4;
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.penetrate = K switch { Kind.Aero => 3, Kind.Plasma => 10, _ => 4 };
            if (K == Kind.Plasma)
                Projectile.tileCollide = true;
        }

        public override void AI()
        {
            Projectile.rotation += 0.2f;
            Color c = K switch { Kind.Aero => new Color(150, 230, 255), Kind.Plasma => new Color(255, 120, 220), _ => new Color(255, 240, 120) };
            Lighting.AddLight(Projectile.Center, c.ToVector3() * (K == Kind.Plasma ? 0.6f : 0.3f));
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, K == Kind.Aero ? DustID.Cloud : DustID.Electric, Main.rand.NextVector2Circular(1f, 1f), 100, c, 1f).noGravity = true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (K == Kind.Plasma || ++bounces > (K == Kind.Aero ? 3 : 4))
                return true;
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (K != Kind.Aero)
                target.AddBuff(BuffID.Electrified, K == Kind.Plasma ? 600 : 180);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (K != Kind.Plasma)
                return false;
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, tex.Size() / 2f, 0.7f, SpriteEffects.None);
            return false;
        }
    }

    /// <summary>Flèche d'énergie de la Rose Bow : entaille l'ennemi touché 3 fois (30 % chacune), Venom 1 à 8 s.</summary>
    public class RoseArrow : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.ChlorophyteArrow;

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.WoodenArrowFriendly);
            AIType = ProjectileID.WoodenArrowFriendly;
            Projectile.arrow = true;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 120, 170, 200);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Venom, Main.rand.Next(60, 481));
            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<RoseSlash>(), (int)(Projectile.damage * 0.3f), 0f, Projectile.owner, target.whoAmI);
        }
    }

    /// <summary>Entaille de la Rose Bow : 3 coups sur l'ennemi ai[0], à 8 images d'écart.</summary>
    public class RoseSlash : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 24;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
        }

        public override bool? CanHitNPC(NPC target) => target.whoAmI == (int)Projectile.ai[0] ? null : false;

        public override void AI()
        {
            NPC n = Main.npc[(int)Projectile.ai[0]];
            if (!n.active)
            {
                Projectile.Kill();
                return;
            }
            Projectile.Center = n.Center;
            if (!Main.dedServ && Projectile.timeLeft % 8 == 0)
                for (int i = 0; i < 8; i++)
                    Dust.NewDustPerfect(n.Center + new Vector2(i * 6 - 24, i * 6 - 24).RotatedBy(Projectile.timeLeft), DustID.PinkTorch, Vector2.Zero, 100, default, 1.2f).noGravity = true;
        }
    }

    /// <summary>Citrouille blanche du Pumplerfaust : grossit au bout du canon tant qu'on maintient (5 s), puis part vers le curseur.</summary>
    public class WhitePumpkin : ModProjectile
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.Pumpkin;

        private const int FullCharge = 300;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 2;
            Projectile.tileCollide = false;
        }

        public override bool? CanDamage() => Projectile.ai[1] == 1f ? null : false;

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (Projectile.ai[1] == 0f)
            {
                if (owner.channel && !owner.dead && owner.HeldItem.type == ModContent.ItemType<Items.Weapons.Ranged.Pumplerfaust>())
                {
                    Projectile.timeLeft = 2;
                    owner.itemTime = owner.itemAnimation = 2;
                    owner.heldProj = Projectile.whoAmI;
                    Vector2 aim = (Main.MouseWorld - owner.MountedCenter).SafeNormalize(Vector2.UnitX * owner.direction);
                    if (Projectile.owner == Main.myPlayer)
                    {
                        Projectile.velocity = aim;
                        if (Projectile.ai[0] % 10f == 0f)
                            Projectile.netUpdate = true;
                    }
                    owner.ChangeDir(Projectile.velocity.X > 0 ? 1 : -1);
                    owner.itemRotation = (Projectile.velocity * owner.direction).ToRotation();
                    if (Projectile.ai[0] < FullCharge && ++Projectile.ai[0] == FullCharge)
                        SoundEngine.PlaySound(SoundID.Item29, Projectile.Center);
                    Projectile.scale = 0.6f + Projectile.ai[0] / FullCharge;
                    Projectile.Center = owner.MountedCenter + Projectile.velocity * (60f + 12f * Projectile.scale);
                    return;
                }
                // Relâchée : part vers le curseur
                Projectile.ai[1] = 1f;
                Projectile.timeLeft = 240;
                Projectile.tileCollide = true;
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX) * 17f;
                Projectile.netUpdate = true;
            }
            Projectile.velocity.Y += 0.1f;
            Projectile.rotation += 0.1f;
        }

        public override Color? GetAlpha(Color lightColor) => Projectile.ai[0] >= FullCharge ? Color.White : new Color(235, 235, 235) * (lightColor.A / 255f);

        public override void OnKill(int timeLeft)
        {
            if (Projectile.ai[1] != 1f)
                return;
            bool full = Projectile.ai[0] >= FullCharge;
            SoundEngine.PlaySound(full ? SoundID.Item62 : SoundID.Item14, Projectile.Center);
            if (Projectile.owner != Main.myPlayer)
                return;
            float charge = Projectile.ai[0] / FullCharge;
            int size = (int)MathHelper.Lerp(60f, 160f, charge);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<PumpkinBlast>(), (int)(Projectile.damage * (1f + charge)), 6f, Projectile.owner, size);
            if (full)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<PumpkinMist>(), (int)(Projectile.damage * 0.3f), 0f, Projectile.owner);
        }
    }

    /// <summary>Explosion de citrouille (taille ai[0]).</summary>
    public class PumpkinBlast : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 5;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.ExpandHitboxBy((int)Projectile.ai[0]);
            if (!Main.dedServ)
                for (int i = 0; i < Projectile.ai[0] / 3; i++)
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 2f).velocity *= 2.5f;
        }
    }

    /// <summary>Brume blanche après une citrouille pleine : 5 s, 200 px, frappe toutes les 20 images.</summary>
    public class PumpkinMist : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 200;
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
            if (!Main.dedServ)
                for (int i = 0; i < 3; i++)
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, -0.3f, 160, Color.White, 2f).noGravity = true;
        }
    }

    /// <summary>Canon du Gourdler : charge une grenade de plus toutes les 36 images (8 au plus), les tire toutes en relâchant.</summary>
    public class GourdlerHoldout : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Ranged/Gourdler";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 2;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            int loaded = 1 + (int)(Projectile.ai[0] / 36f);
            if (owner.channel && !owner.dead && owner.HeldItem.type == ModContent.ItemType<Items.Weapons.Ranged.Gourdler>())
            {
                Projectile.timeLeft = 2;
                owner.itemTime = owner.itemAnimation = 2;
                owner.heldProj = Projectile.whoAmI;
                if (Projectile.owner == Main.myPlayer)
                {
                    Projectile.velocity = (Main.MouseWorld - owner.MountedCenter).SafeNormalize(Vector2.UnitX * owner.direction);
                    if (Projectile.ai[0] % 10f == 0f)
                        Projectile.netUpdate = true;
                }
                owner.ChangeDir(Projectile.velocity.X > 0 ? 1 : -1);
                owner.itemRotation = (Projectile.velocity * owner.direction).ToRotation();
                Projectile.Center = owner.MountedCenter + Projectile.velocity * 24f;
                Projectile.rotation = Projectile.velocity.ToRotation();
                if (Projectile.ai[0] < 36f * 7f)
                {
                    Projectile.ai[0]++;
                    if (Projectile.ai[0] % 36f == 0f)
                        SoundEngine.PlaySound(SoundID.Item149, Projectile.Center);
                }
                Projectile.scale = 1f + 0.04f * (loaded - 1);
                return;
            }
            if (Projectile.owner == Main.myPlayer)
            {
                SoundEngine.PlaySound(SoundID.Item61, Projectile.Center);
                for (int i = 0; i < loaded; i++)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.RotatedByRandom(0.2f) * 15f * Main.rand.NextFloat(0.85f, 1.1f),
                        ModContent.ProjectileType<GourdGrenade>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
            Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            SpriteEffects fx = Projectile.velocity.X < 0 ? SpriteEffects.FlipVertically : SpriteEffects.None;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, Projectile.scale, fx);
            return false;
        }
    }

    /// <summary>Grenade-courge : gravité, explose au contact d'un ennemi en 6 épines (30 %).</summary>
    public class GourdGrenade : ModProjectile
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.Pumpkin;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 240;
            Projectile.scale = 0.6f;
        }

        public override void AI()
        {
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.25f, 14f);
            Projectile.rotation += 0.15f;
        }

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, new Color(120, 200, 80), 0.5f);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.owner != Main.myPlayer)
                return;
            SoundEngine.PlaySound(SoundID.Item14 with { Volume = 0.4f }, Projectile.Center);
            for (int i = 0; i < 6; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (MathHelper.TwoPi * i / 6f).ToRotationVector2() * 8f, ProjectileID.SharpTears,
                    (int)(Projectile.damage * 0.3f), 0f, Projectile.owner, 0f, 0.5f);
        }
    }

    /// <summary>Bombe céleste : explose au contact en aura étoilée (55 %), braises de God Slayer (40 %) et traits célestes (80 %).</summary>
    public class CelestialNukeProj : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Rogue/CelestialNuke";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = 1;
            Projectile.timeLeft = 240;
        }

        public override void AI()
        {
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.15f, 14f);
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item62, Projectile.Center);
            if (Projectile.owner != Main.myPlayer)
                return;
            bool stealth = Projectile.Calamity().stealthStrike;
            var source = Projectile.GetSource_FromThis();
            Projectile.NewProjectile(source, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<StarlitAura>(), (int)(Projectile.damage * 0.55f), 0f, Projectile.owner);
            for (int i = 0; i < (stealth ? 16 : 8); i++)
                Projectile.NewProjectile(source, Projectile.Center, Main.rand.NextVector2Circular(6f, 6f), ModContent.ProjectileType<GodSlayerEmber>(), (int)(Projectile.damage * 0.4f), 0f, Projectile.owner);
            for (int i = 0; i < (stealth ? 8 : 4); i++)
                Projectile.NewProjectile(source, Projectile.Center, Main.rand.NextVector2CircularEdge(9f, 9f), ModContent.ProjectileType<CelestialBolt>(), (int)(Projectile.damage * 0.8f), 0f, Projectile.owner);
        }
    }

    /// <summary>Aura étoilée de la Celestial Nuke : 300 px, 3 s.</summary>
    public class StarlitAura : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 300;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 180;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) =>
            Vector2.Distance(Projectile.Center, targetHitbox.ClosestPointInRect(Projectile.Center)) < 150f;

        public override void AI()
        {
            if (!Main.dedServ)
                for (int i = 0; i < 4; i++)
                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(150f, 150f), DustID.PurpleTorch, Vector2.Zero, 100, default, 1.3f).noGravity = true;
        }
    }

    /// <summary>Braise de God Slayer : flotte un peu puis tombe, Embers of Duality (God Slayer Inferno) 5 s.</summary>
    public class GodSlayerEmber : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.97f;
            Projectile.velocity.Y += 0.03f;
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.PurpleTorch, Vector2.Zero, 100, default, 1.4f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 300);
    }

    /// <summary>Trait céleste : cherche les ennemis.</summary>
    public class CelestialBolt : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.HallowStar;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            CalamityUtils.HomeInOnNPC(Projectile, true, 700f, 14f, 15f);
            Projectile.rotation += 0.2f;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(200, 160, 255, 200);
    }
}
