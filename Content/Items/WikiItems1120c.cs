using System;
using System.Linq;
using CalamityFandom.Common.Debug;
using CalamityFandom.Common.WikiData;
using CalamityMod;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

// Lot AU (20/11) : objets qui débloquent une recette du wiki (Miracle Coin, Salt Booklet → Salt Water Bolt, Demon Core,
// World Breaker).

namespace CalamityFandom.Content.Items.Ammo
{
    /// <summary>
    /// Wiki : Miracle Coin.txt — pièce qui vaut 100 Klepticoins ; seulement munition de The Ones Striker (9999 dégâts,
    /// 14 pièces) ; aucun marchand ne l'accepte. [technique] Valeur au maximum du jeu.
    /// </summary>
    public class MiracleCoin : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 100;

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 48;
            Item.maxStack = 999;
            Item.consumable = true;
            Item.ammo = Type;
            Item.rare = ItemRarityID.White;
            Item.value = int.MaxValue;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Magic
{
    /// <summary>
    /// Wiki : Salt Booklet.txt — livre : un cube de sel qui va un peu tout droit, retombe, disparaît au contact d'un bloc ou
    /// d'un ennemi ; trouvé en passant du Sulphurous Sand à l'Extractinator. [choix] 1 chance sur 50 par sable.
    /// </summary>
    public class SaltBooklet : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 10;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 12;
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5.5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item21;
            Item.width = 28;
            Item.height = 32;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(copper: 2);
            Item.shoot = ModContent.ProjectileType<Projectiles.Magic.SaltCube>();
            Item.shootSpeed = 7.5f;
        }
    }

    /// <summary>
    /// Wiki : Salt Water Bolt.txt — Water Bolt abîmée : boule d'eau salée qui retombe au bout de 2 s et ne rebondit pas,
    /// Electrified 1 s.
    /// </summary>
    public class SaltWaterBolt : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 21;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 12;
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5.5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item21;
            Item.width = 30;
            Item.height = 38;
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(copper: 1);
            Item.shoot = ModContent.ProjectileType<Projectiles.Magic.SaltWaterBoltProj>();
            Item.shootSpeed = 7.5f;
        }
    }

    /// <summary>Le Sulphurous Sand passe à l'Extractinator ; il donne parfois une Salt Booklet.</summary>
    public class SaltBookletExtract : GlobalItem
    {
        private static int Sand => ModContent.ItemType<CalamityMod.Items.Placeables.Abyss.SulphurousSand>();

        public override void SetStaticDefaults() => ItemID.Sets.ExtractinatorMode[Sand] = Sand;

        public override void ExtractinatorUse(int extractType, int extractinatorBlockType, ref int resultType, ref int resultStack)
        {
            if (extractType == Sand && Main.rand.NextBool(50))
            {
                resultType = ModContent.ItemType<SaltBooklet>();
                resultStack = 1;
            }
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Rogue
{
    /// <summary>
    /// Wiki : Demon Core.txt — arme de spy consommable : un cœur sombre qui inflige Shadowflame (3 s) à tout ennemi à moins de
    /// 4 blocs ; frappe furtive : le cœur laisse un nuage de gaz qui inflige aussi Shadowflame.
    /// [choix] Le cœur traverse 2 ennemis et vit 2 s ; aura toutes les 10 images ; nuage de 3 s.
    /// </summary>
    public class DemonCore : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 99;

        public override void SetDefaults()
        {
            Item.damage = 32;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.useTime = Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 1f;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.maxStack = 9999;
            Item.UseSound = SoundID.Item1;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(copper: 80);
            Item.shoot = ModContent.ProjectileType<Projectiles.Rogue.DemonCoreProj>();
            Item.shootSpeed = 7f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (player.Calamity().StealthStrikeAvailable() && Main.projectile.IndexInRange(p))
            {
                Main.projectile[p].Calamity().stealthStrike = true;
                player.Calamity().ConsumeStealthByAttacking();
            }
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Tools
{
    /// <summary>
    /// Wiki : World Breaker.txt — pioche 280 % et hache 185 %, +3 de portée ; clic droit : une lente boule « Armageddon »
    /// qui traverse les blocs en ralentissant jusqu'à s'arrêter et casse aussitôt tout bloc qu'elle touche.
    /// [choix] « Tout bloc » = ce qu'une pioche peut casser (pas les meubles, coffres, ni ce que le jeu protège) ; la boule
    /// vit 5 s.
    /// </summary>
    public class WorldBreaker : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 200;
            Item.DamageType = DamageClass.Melee;
            Item.pick = 280;
            Item.axe = 185 / 5;
            Item.tileBoost = 3;
            Item.useTime = 4;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3f;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.UseSound = SoundID.Item1;
            Item.width = Item.height = 70;
            Item.rare = ModContent.RarityType<CosmicPurple>();
            Item.value = Item.sellPrice(gold: 20);
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.pick = 0;
                Item.axe = 0;
                Item.shoot = ModContent.ProjectileType<Projectiles.Typeless.ArmageddonBall>();
                Item.shootSpeed = 6f;
                Item.noMelee = true;
                Item.UseSound = SoundID.Item33;
                return player.ownedProjectileCounts[Item.shoot] < 1;
            }
            Item.pick = 280;
            Item.axe = 185 / 5;
            Item.shoot = ProjectileID.None;
            Item.noMelee = false;
            Item.UseSound = SoundID.Item1;
            return true;
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Magic
{
    /// <summary>Cube de sel (Salt Booklet) : va tout droit 20 images puis retombe ; disparaît au premier contact.</summary>
    public class SaltCube : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Magic/SaltCube";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            if (++Projectile.ai[0] > 20)
                Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.3f, 12f);
            Projectile.rotation += 0.2f;
        }
    }

    /// <summary>Boule d'eau salée (Salt Water Bolt) : retombe après 2 s, ne rebondit pas, Electrified 1 s.</summary>
    public class SaltWaterBoltProj : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Magic/SaltWaterBoltProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            if (++Projectile.ai[0] > 120)
                Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.25f, 12f);
            if (Main.rand.NextBool(3))
                Dust.NewDustPerfect(Projectile.Center, DustID.Water, Vector2.Zero).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Electrified, 60);
    }
}

namespace CalamityFandom.Content.Projectiles.Rogue
{
    /// <summary>Cœur démoniaque : aura de Shadowflame (4 blocs) ; furtif : nuage de gaz à sa disparition.</summary>
    public class DemonCoreProj : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Rogue/DemonCore";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = 2;
            Projectile.timeLeft = 120;
            Projectile.scale = 0.6f;
        }

        public override void AI()
        {
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.15f, 10f);
            Projectile.rotation += 0.15f;
            Lighting.AddLight(Projectile.Center, 0.5f, 0.1f, 0.6f);
            if (++Projectile.ai[0] % 10 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                foreach (NPC n in Main.npc)
                    if (n.active && !n.friendly && n.Distance(Projectile.Center) < 64f)
                        n.AddBuff(BuffID.ShadowFlame, 180);
            if (Main.rand.NextBool(2))
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(64f, 64f), DustID.Shadowflame, Vector2.Zero, 100, default, 0.9f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.ShadowFlame, 180);

        public override void OnKill(int timeLeft)
        {
            if (Projectile.Calamity().stealthStrike && Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<DemonGas>(), Projectile.damage / 2, 0f, Projectile.owner);
        }
    }

    /// <summary>Nuage de gaz du Demon Core (frappe furtive) : 3 s, Shadowflame.</summary>
    public class DemonGas : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 120;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(55f, 55f), DustID.Shadowflame, Main.rand.NextVector2Circular(1f, 1f), 120, default, 1.4f);
            d.noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.ShadowFlame, 180);
    }
}

namespace CalamityFandom.Content.Projectiles.Typeless
{
    /// <summary>Boule Armageddon (World Breaker) : traverse les blocs en ralentissant et casse ceux qu'elle touche.</summary>
    public class ArmageddonBall : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Typeless/ArmageddonBall";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.985f;
            Projectile.rotation += 0.1f;
            Lighting.AddLight(Projectile.Center, 0.9f, 0.4f, 0.1f);
            if (Projectile.owner != Main.myPlayer)
                return;
            Point min = Projectile.TopLeft.ToTileCoordinates(), max = Projectile.BottomRight.ToTileCoordinates();
            for (int x = min.X; x <= max.X; x++)
                for (int y = min.Y; y <= max.Y; y++)
                {
                    if (!WorldGen.InWorld(x, y, 10))
                        continue;
                    Tile t = Main.tile[x, y];
                    if (!t.HasTile || Main.tileFrameImportant[t.TileType] || TileID.Sets.BasicChest[t.TileType] || !WorldGen.CanKillTile(x, y))
                        continue;
                    WorldGen.KillTile(x, y);
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, x, y);
                }
        }
    }
}

namespace CalamityFandom.Content.Items
{
    public class WikiItems1120cChecks : ModSystem
    {
        public override void Load()
        {
            SelfTest.ExtraChecks.Add(t =>
            {
                foreach (string name in new[] { "Miracle Coin", "Salt Water Bolt", "Demon Core", "World Breaker", "Klepticoin" })
                {
                    int type = WikiRecipes.ResolveItem(name);
                    t.Expect(type > 0 && Main.recipe.Take(Recipe.numRecipes).Any(r => r.createItem.type == type && !r.Disabled), $"recette du wiki de {name}");
                }
                t.Expect(ItemID.Sets.ExtractinatorMode[ModContent.ItemType<CalamityMod.Items.Placeables.Abyss.SulphurousSand>()] >= 0, "le Sulphurous Sand passe à l'Extractinator");
            });
        }
    }
}
