using System;
using CalamityFandom.Content.Projectiles.Tools1119b;
using CalamityMod;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Dart = CalamityFandom.Content.Projectiles.WikiDarts.WikiDartProj;

// Lot AH (19/11) : outils et potions du wiki (recettes du wiki). Puissances de hache : valeur du wiki / 5 (Terraria).

namespace CalamityFandom.Content.Items.Tools
{
    /// <summary>
    /// Wiki : Braxaxe.txt — pioche 299 %, hache 3001 %, marteau 3001 % ; clic gauche : blocs et arbres ; clic droit : arbres
    /// et murs ; +20 de portée. [choix] Marteau ramené à 300 % (au-delà rien ne change) ; cadence 1 comme sur l'infobox.
    /// [technique] « Le Primordial Ore tombe sous sa forme d'objet » : pas encore de Primordial Ore dans ce mod.
    /// </summary>
    public class Braxaxe : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 100;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.knockBack = 0f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = Item.height = 62;
            Item.pick = 299;
            Item.axe = 600;
            Item.tileBoost = 20;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
            Item.value = Item.sellPrice(gold: 30);
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            bool right = player.altFunctionUse == 2;
            Item.pick = right ? 0 : 299;
            Item.hammer = right ? 300 : 0;
            Item.axe = 600;
            return true;
        }
    }

    /// <summary>
    /// Wiki : Destinary.txt — tronçonneuse (hache infinie) qui lance des disques exo vers le curseur ; ils reviennent après
    /// le curseur ou une certaine distance, ignorent l'invulnérabilité des ennemis (10 %) et coupent les arbres d'un coup.
    /// [choix] Hache 600 (« infinie ») ; un disque toutes les 20 images ; retour à 600 px ou au curseur.
    /// </summary>
    public class Destinary : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 5160;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = Item.noUseGraphic = true;
            Item.channel = true;
            Item.knockBack = 36f;
            Item.crit = 22;
            Item.UseSound = SoundID.Item23;
            Item.width = 197;
            Item.height = 108;
            Item.axe = 600;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.value = Item.sellPrice(gold: 30);
            Item.shoot = ModContent.ProjectileType<DestinaryHoldout>();
            Item.shootSpeed = 40f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;
    }

    /// <summary>Wiki : Grarble Obliterator.txt — foreuse-hache d'avant les boss : pioche 50 %, hache 30 %.</summary>
    public class GrarbleObliterator : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 13;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 6;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = Item.noUseGraphic = true;
            Item.channel = true;
            Item.knockBack = 1f;
            Item.UseSound = SoundID.Item23;
            Item.width = 50;
            Item.height = 22;
            Item.pick = 50;
            Item.axe = 6;
            Item.tileBoost = -1;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 20);
            Item.shoot = ModContent.ProjectileType<GrarbleObliteratorProj>();
            Item.shootSpeed = 32f;
        }
    }

    /// <summary>Outils en Twisted Netherite : Wither sur les coups.</summary>
    public abstract class TwistedNetheriteTool : ModItem
    {
        protected abstract int WitherSeconds { get; }

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Melee;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.knockBack = 8f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.value = Item.sellPrice(gold: 12);
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Buffs.Wither>(), WitherSeconds * 60);
    }

    /// <summary>Wiki : Twisted Netherite Axe.txt — hache 260 %, Wither 5 s.</summary>
    public class TwistedNetheriteAxe : TwistedNetheriteTool
    {
        protected override int WitherSeconds => 5;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 90;
            Item.useAnimation = 8;
            Item.useTime = 5;
            Item.axe = 52;
            Item.width = Item.height = 56;
        }
    }

    /// <summary>Wiki : Twisted Netherite Pickaxe.txt — pioche 260 % (jusqu'à l'Auric Ore), Wither 3 s.</summary>
    public class TwistedNetheritePickaxe : TwistedNetheriteTool
    {
        protected override int WitherSeconds => 3;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 45;
            Item.useAnimation = 8;
            Item.useTime = 4;
            Item.pick = 260;
            Item.width = Item.height = 32;
        }
    }

    /// <summary>
    /// Wiki : Twisted Netherite Shovel.txt — creuse un carré de 10 x 10 blocs autour du curseur, avec une pioche de 60 %
    /// seulement (les blocs plus durs restent) ; Wither 3 s. [choix] Portée habituelle d'un outil (le curseur doit être à
    /// portée) ; un carré par coup.
    /// </summary>
    public class TwistedNetheriteShovel : TwistedNetheriteTool
    {
        protected override int WitherSeconds => 3;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 45;
            Item.useAnimation = 60;
            Item.useTime = 5;
            Item.pick = 60;
            Item.width = 50;
            Item.height = 54;
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI != Main.myPlayer || player.itemAnimation != player.itemAnimationMax)
                return null;
            Point c = Main.MouseWorld.ToTileCoordinates();
            if (Vector2.Distance(Main.MouseWorld, player.Center) > (Player.tileRangeX + Item.tileBoost + 4) * 16f)
                return null;
            for (int x = c.X - 5; x < c.X + 5; x++)
                for (int y = c.Y - 5; y < c.Y + 5; y++)
                {
                    if (!WorldGen.InWorld(x, y, 2))
                        continue;
                    Tile tile = Main.tile[x, y];
                    if (!tile.HasTile || !WorldGen.CanKillTile(x, y) || !player.HasEnoughPickPowerToHurtTile(x, y))
                        continue;
                    WorldGen.KillTile(x, y);
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, x, y);
                }
            return true;
        }
    }
}

namespace CalamityFandom.Content.Items.Potions
{
    /// <summary>Wiki : Meld Skin Potion.txt — Meld Skin 8 min : +22 défense.</summary>
    public class MeldSkinPotion : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 20;

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 34;
            Item.useTime = Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.UseSound = SoundID.Item3;
            Item.useTurn = true;
            Item.consumable = true;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 1);
            Item.buffType = ModContent.BuffType<Buffs.MeldSkin>();
            Item.buffTime = 8 * 60 * 60;
        }
    }

    /// <summary>
    /// Wiki : Supreme Strange Brew.txt — soigne 200 à 325 PV et donne 40 à 70 s de Potion Sickness (au hasard) ; lâchée par
    /// The Starborn Moth (butin de la page).
    /// </summary>
    public class SupremeStrangeBrew : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 30;

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 34;
            Item.useTime = Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.UseSound = SoundID.Item3;
            Item.useTurn = true;
            Item.consumable = true;
            Item.maxStack = 9999;
            Item.potion = true;
            Item.healLife = 200;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.value = Item.buyPrice(gold: 3);
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
                player.Heal(Main.rand.Next(0, 126));
            player.GetModPlayer<ShortSicknessPlayer>().Pending = Main.rand.Next(40 * 60, 70 * 60 + 1);
            return null;
        }
    }
}

namespace CalamityFandom.Content.Buffs
{
    public class MeldSkin : ModBuff
    {
        public override string Texture => "CalamityFandom/Content/Buffs/Wiki/MeldSkin";

        public override void Update(Player player, ref int buffIndex) => player.statDefense += 22;
    }
}

namespace CalamityFandom.Content.Projectiles.Tools1119b
{
    /// <summary>Foreuse du Grarble Obliterator (texture de l'objet).</summary>
    public class GrarbleObliteratorProj : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Tools/GrarbleObliterator";

        public override void SetStaticDefaults() => ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.aiStyle = ProjAIStyleID.Drill;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            Projectile.ownerHitCheck = true;
            Projectile.DamageType = DamageClass.Melee;
        }
    }

    /// <summary>Tronçonneuse tenue du Destinary : frappe au contact, lance un disque exo toutes les 20 images.</summary>
    public class DestinaryHoldout : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Tools/Destinary";

        public override void SetStaticDefaults() => ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.aiStyle = ProjAIStyleID.Drill;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            Projectile.ownerHitCheck = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.scale = 0.5f;
        }

        public override void PostAI()
        {
            if (Projectile.owner == Main.myPlayer && ++Projectile.localAI[1] % 20 == 1)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.UnitX) * 18f,
                    ModContent.ProjectileType<ExoDisk>(), Projectile.damage / 10, 0f, Projectile.owner, Main.MouseWorld.X, Main.MouseWorld.Y);
        }
    }

    /// <summary>
    /// Disque exo du Destinary (10 %) : va au curseur (ai[0], ai[1]) ou à 600 px puis revient ; frappe à chaque image ;
    /// abat les arbres qu'il traverse.
    /// </summary>
    public class ExoDisk : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Melee/RefractionRotorDisk";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 240;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 1;
            Projectile.scale = 0.25f;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            Projectile.rotation += 0.6f;
            Vector2 target = new(Projectile.ai[0], Projectile.ai[1]);
            if (Projectile.localAI[0] == 0f && (Vector2.Distance(Projectile.Center, target) < 30f || Vector2.Distance(Projectile.Center, owner.Center) > 600f))
                Projectile.localAI[0] = 1f;
            if (Projectile.localAI[0] == 1f)
            {
                Vector2 back = owner.Center - Projectile.Center;
                if (back.Length() < 40f)
                {
                    Projectile.Kill();
                    return;
                }
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, back.SafeNormalize(Vector2.Zero) * 20f, 0.15f);
            }
            if (Projectile.owner != Main.myPlayer)
                return;
            // Abat les arbres traversés
            Point p = Projectile.Center.ToTileCoordinates();
            if (WorldGen.InWorld(p.X, p.Y, 2) && Main.tile[p.X, p.Y].HasTile && Main.tileAxe[Main.tile[p.X, p.Y].TileType])
            {
                WorldGen.KillTile(p.X, p.Y);
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, p.X, p.Y);
            }
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;
    }
}
