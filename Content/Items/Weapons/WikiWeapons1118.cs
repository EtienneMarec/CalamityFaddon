using System;
using CalamityFandom.Content.Legacy.Buffs.StatDebuffs;
using CalamityFandom.Content.Projectiles.Weapons1118;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

// Armes inventées par le wiki (18/11) : sprites et valeurs des pages, recettes du wiki (WikiRecipes).

namespace CalamityFandom.Content.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Terriblade.txt — chaque coup tire au hasard un effet parmi neuf : coup normal, On Fire! 3 s, glaçon (Glacial
    /// State 3 s), éclair à un endroit de l'écran (Electrified 3 s), Bleeding 3 s, lancer en boomerang qui se plante 5 s dans
    /// l'ennemi, éclats autour du joueur (20 % des dégâts), lancer qui ricoche au sol, coup vers le haut qui projette le
    /// joueur. Vaporfied 1,5 s à chaque coup.
    /// </summary>
    public class Terriblade : ModItem
    {
        public const int Normal = 0, Fire = 1, Ice = 2, Lightning = 3, Bleed = 4, Boomerang = 5, Shrapnel = 6, Skip = 7, Launch = 8;

        private int effect;

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 200;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 11;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 9f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = Item.height = 60;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(gold: 30);
            Item.shoot = ProjectileID.PurificationPowder; // remplacé dans Shoot (aucun projectile pour la plupart des effets)
            Item.shootSpeed = 15f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            effect = Main.rand.Next(9);
            switch (effect)
            {
                case Ice:
                    Projectile.NewProjectile(source, position, velocity * 0.8f, ModContent.ProjectileType<TerribladeIcicle>(), damage / 2, knockback, player.whoAmI);
                    break;
                case Lightning:
                    Vector2 spot = Main.screenPosition + new Vector2(Main.rand.Next(Main.screenWidth), Main.rand.Next(Main.screenHeight / 2, Main.screenHeight));
                    Projectile.NewProjectile(source, spot, Vector2.Zero, ModContent.ProjectileType<TerribladeLightning>(), damage, knockback, player.whoAmI);
                    SoundEngine.PlaySound(SoundID.Thunder with { Volume = 0.5f }, spot);
                    break;
                case Boomerang:
                case Skip:
                    // L'épée part (elle ne se voit plus dans la main pendant ce temps)
                    if (player.ownedProjectileCounts[ModContent.ProjectileType<TerribladeThrown>()] < 1)
                        Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<TerribladeThrown>(), damage, knockback, player.whoAmI, effect);
                    break;
                case Shrapnel:
                    SoundEngine.PlaySound(SoundID.Item14 with { Volume = 0.6f }, player.Center);
                    for (int i = 0; i < 8; i++)
                        Projectile.NewProjectile(source, player.Center, (MathHelper.TwoPi * i / 8f).ToRotationVector2() * 9f, ModContent.ProjectileType<TerribladeShrapnel>(),
                            (int)(damage * 0.2f), knockback * 0.2f, player.whoAmI);
                    break;
                case Launch:
                    player.velocity.Y = -14f * player.gravDir;
                    player.fallStart = (int)(player.position.Y / 16f);
                    break;
            }
            return false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            int dust = effect switch { Fire => DustID.Torch, Ice => DustID.IceTorch, Lightning => DustID.Electric, Bleed => DustID.Blood, _ => DustID.Obsidian };
            if (Main.rand.NextBool(3))
                Dust.NewDustDirect(hitbox.TopLeft(), hitbox.Width, hitbox.Height, dust).noGravity = true;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Vaporfied>(), 90);
            switch (effect)
            {
                case Fire: target.AddBuff(BuffID.OnFire, 180); break;
                case Ice: target.AddBuff(ModContent.BuffType<GlacialState>(), 180); break;
                case Lightning: target.AddBuff(ModContent.BuffType<StaticDischarge>(), 180); break;
                case Bleed: target.AddBuff(BuffID.Bleeding, 180); break;
            }
        }
    }

    /// <summary>Wiki : Sea Stone Sickle.txt — faucille en Sea Stone, sans effet.</summary>
    public class SeaStoneSickle : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 14;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4f;
            Item.UseSound = SoundID.Item1;
            Item.width = Item.height = 40;
            Item.rare = ItemRarityID.Blue;
            Item.value = 2 * 5;
        }
    }

    /// <summary>Wiki : Auric Sword.txt — épée d'avant Hardmode sans effet, frappe automatique.</summary>
    public class AuricSword : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 50;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = Item.height = 60;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(silver: 2, copper: 30);
        }
    }

    /// <summary>Wiki : Raito.txt — épée de vraie mêlée, amélioration de l'Excalibur ; 10 % de critique.</summary>
    public class Raito : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 200;
            Item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Item.useTime = Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4f;
            Item.crit = 6;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = Item.height = 44;
            Item.scale = 1.4f;
            Item.rare = ItemRarityID.Cyan;
            Item.value = Item.sellPrice(gold: 30);
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
                Dust.NewDustDirect(hitbox.TopLeft(), hitbox.Width, hitbox.Height, DustID.Enchanted_Gold).noGravity = true;
        }
    }

    /// <summary>
    /// Wiki : Majestic Gar.txt — grande portée, chaque coup baisse la défense de l'ennemi de 1 ; si elle est à 0 ou moins,
    /// le joueur récupère 3 PV. [choix] Grande portée : taille x1,5.
    /// </summary>
    public class MajesticGar : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 70;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 21;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 7.5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = Item.height = 80;
            Item.scale = 1.5f;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(gold: 7, silver: 20);
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.defense > 0)
            {
                target.defense--;
                return;
            }
            if (player.moonLeech)
                return;
            player.statLife = Math.Min(player.statLife + 3, player.statLifeMax2);
            player.HealEffect(3);
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Rogue
{
    /// <summary>Wiki : Sea Stone Shuriken.txt — shurikens consommables qui traversent jusqu'à 2 ennemis.</summary>
    public class SeaStoneShuriken : RogueWeapon
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 99;

        public override void SetDefaults()
        {
            Item.damage = 11;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.knockBack = 3f;
            Item.useTime = Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = Item.noUseGraphic = true;
            Item.consumable = true;
            Item.maxStack = 9999;
            Item.UseSound = SoundID.Item1;
            Item.width = Item.height = 22;
            Item.rare = ItemRarityID.Blue;
            Item.value = 2 * 5;
            Item.shoot = ModContent.ProjectileType<SeaStoneShurikenProj>();
            Item.shootSpeed = 12f;
            Item.autoReuse = true;
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
    /// Wiki : Dirt Clump.txt — boule de terre consommable (5 par bloc de terre) ; frappe furtive : +50 % de dégâts et se
    /// brise en fragments qui frappent les ennemis proches (4 dégâts).
    /// </summary>
    public class DirtClump : RogueWeapon
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 99;

        public override void SetDefaults()
        {
            Item.damage = 6;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.knockBack = 2f;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = Item.noUseGraphic = true;
            Item.consumable = true;
            Item.maxStack = 9999;
            Item.UseSound = SoundID.Item1;
            Item.width = Item.height = 16;
            Item.rare = ItemRarityID.White;
            Item.shoot = ModContent.ProjectileType<DirtClumpProj>();
            Item.shootSpeed = 7f;
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

namespace CalamityFandom.Content.Items.Weapons.Ranged
{
    /// <summary>
    /// Wiki : Sillyshark.txt — pistolet rapide (33 % de chance de ne pas consommer de munition) ; une balle sur dix est
    /// remplacée par une toux de crottes de nez inoffensives ; Vaporfied 1,5 s par les balles.
    /// </summary>
    public class Sillyshark : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 15;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.75f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item11;
            Item.width = 55;
            Item.height = 32;
            Item.scale = 0.75f;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(silver: 16);
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 20f;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-8f, 0f);

        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.NextFloat() >= 0.33f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.rand.NextBool(10))
            {
                // Toux : quelques crottes de nez qui tombent, sans dégâts
                SoundEngine.PlaySound(SoundID.NPCHit18 with { Volume = 0.5f, Pitch = 0.5f }, position);
                for (int i = 0; i < Main.rand.Next(3, 6); i++)
                    Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.2f, 0.4f), ModContent.ProjectileType<SillysharkBooger>(), 0, 0f, player.whoAmI);
                return false;
            }
            int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (Main.projectile.IndexInRange(p))
                Main.projectile[p].GetGlobalProjectile<VaporfiedShot>().Active = true;
            return false;
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Weapons1118
{
    /// <summary>Balles du Sillyshark : Vaporfied 1,5 s.</summary>
    public class VaporfiedShot : GlobalProjectile
    {
        public bool Active;

        public override bool InstancePerEntity => true;

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Active)
                target.AddBuff(ModContent.BuffType<Vaporfied>(), 90);
        }
    }

    /// <summary>Crotte de nez du Sillyshark : tombe, inoffensive.</summary>
    public class SillysharkBooger : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = false;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.25f;
            if (!Main.dedServ)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.t_Slime, Vector2.Zero, 100, new Color(120, 200, 80), 0.9f);
                d.noGravity = true;
            }
        }
    }

    /// <summary>Shuriken en Sea Stone : traverse 2 ennemis.</summary>
    public class SeaStoneShurikenProj : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Rogue/SeaStoneShuriken";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = 3;
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.rotation += 0.35f * Math.Sign(Projectile.velocity.X);
            if (++Projectile.ai[0] > 25f)
                Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.25f, 16f);
        }

        public override void OnKill(int timeLeft)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            if (Main.rand.NextBool(4))
                Item.NewItem(Projectile.GetSource_DropAsItem(), Projectile.Hitbox, ModContent.ItemType<Items.Weapons.Rogue.SeaStoneShuriken>());
        }
    }

    /// <summary>Boule de terre : tombe ; en frappe furtive, se brise en fragments de 4 dégâts qui frappent les ennemis proches.</summary>
    public class DirtClumpProj : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Rogue/DirtClump";

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
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.25f, 16f);
            Projectile.rotation += 0.15f * Math.Sign(Projectile.velocity.X);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            for (int i = 0; i < 8; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Dirt);
            if (!Projectile.Calamity().stealthStrike || Projectile.owner != Main.myPlayer)
                return;
            for (int i = 0; i < 5; i++)
            {
                int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Circular(5f, 5f) - Vector2.UnitY * 2f,
                    ModContent.ProjectileType<DirtFragment>(), 4, 0f, Projectile.owner);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].DamageType = ModContent.GetInstance<RogueDamageClass>();
            }
        }
    }

    /// <summary>Fragment de terre (Dirt Clump en frappe furtive) : cherche l'ennemi le plus proche à 200 px.</summary>
    public class DirtFragment : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Rogue/DirtClump";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 90;
            Projectile.scale = 0.5f;
        }

        public override void AI()
        {
            CalamityUtils.HomeInOnNPC(Projectile, false, 200f, 7f, 15f);
            Projectile.velocity.Y += 0.1f;
        }
    }

    /// <summary>Glaçon de la Terriblade : Glacial State 3 s.</summary>
    public class TerribladeIcicle : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.FrostBoltSword;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Main.rand.NextBool(2))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.IceTorch).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<GlacialState>(), 180);
    }

    /// <summary>Éclair de la Terriblade : colonne qui frappe à un endroit de l'écran (Electrified 3 s).</summary>
    public class TerribladeLightning : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 600;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 12;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void OnSpawn(IEntitySource source) => Projectile.position.Y -= Projectile.height - 30;

        public override void AI()
        {
            Lighting.AddLight(Projectile.Bottom, 0.6f, 0.8f, 1f);
            if (Main.dedServ)
                return;
            for (int i = 0; i < 8; i++)
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, 0f, 4f, 100, default, 1.2f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<StaticDischarge>(), 180);
    }

    /// <summary>Éclat de la Terriblade (20 % des dégâts).</summary>
    public class TerribladeShrapnel : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.CrystalShard;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 40;
        }

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        public override Color? GetAlpha(Color lightColor) => new Color(90, 60, 120, 200);
    }

    /// <summary>
    /// Terriblade lancée : ai[0] = 5 en boomerang (se plante 5 s dans le premier ennemi touché, puis revient), 7 en ricochet
    /// (tombe et rebondit sur le sol et l'eau, 4 rebonds).
    /// </summary>
    public class TerribladeThrown : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Melee/Terriblade";

        private int stuckTo = -1;
        private Vector2 offset;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.scale = 0.6f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (Projectile.ai[0] == Items.Weapons.Melee.Terriblade.Skip)
            {
                Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.35f, 14f);
                if (Projectile.wet && Projectile.velocity.Y > 0f)
                    Projectile.velocity.Y = -Math.Abs(Projectile.velocity.Y) * 0.8f;
                Projectile.rotation += 0.4f * Math.Sign(Projectile.velocity.X);
                return;
            }
            // Boomerang
            if (stuckTo >= 0)
            {
                NPC npc = Main.npc[stuckTo];
                if (!npc.active || ++Projectile.ai[1] > 300f)
                {
                    stuckTo = -1;
                    Projectile.localAI[0] = 1f;
                }
                else
                {
                    Projectile.Center = npc.Center + offset;
                    Projectile.velocity = Vector2.Zero;
                    return;
                }
            }
            Projectile.rotation += 0.4f;
            if (++Projectile.localAI[1] > 30f)
                Projectile.localAI[0] = 1f;
            if (Projectile.localAI[0] == 1f)
            {
                Projectile.tileCollide = false;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(owner.Center) * 16f, 0.1f);
                if (Projectile.Hitbox.Intersects(owner.Hitbox))
                    Projectile.Kill();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Vaporfied>(), 90);
            if (Projectile.ai[0] == Items.Weapons.Melee.Terriblade.Boomerang && stuckTo < 0 && Projectile.localAI[0] == 0f)
            {
                stuckTo = target.whoAmI;
                offset = Projectile.Center - target.Center;
                Projectile.netUpdate = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.ai[0] != Items.Weapons.Melee.Terriblade.Skip)
            {
                Projectile.localAI[0] = 1f;
                return false;
            }
            // Ricochet : rebondit en perdant de la vitesse, 4 fois
            if (++Projectile.ai[1] > 4)
                return true;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y * 0.7f;
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X * 0.7f;
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None);
            return false;
        }
    }
}
