using System.Collections.Generic;
using System.Linq;
using CalamityMod;
using CalamityMod.NPCs.Abyss;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

// Outils et objets inventés par le wiki (19/11) : sprites et valeurs des pages, recettes du wiki.

namespace CalamityFandom.Content.Items.Tools
{
    /// <summary>Wiki : Anti-Moth Ointment.txt — en favori dans l'inventaire : pas de Prismatic Lacewing autour du joueur.</summary>
    public class AntiMothOintment : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 36;
            Item.rare = ItemRarityID.Lime;
            Item.value = Item.sellPrice(gold: 1);
        }
    }

    /// <summary>
    /// Wiki : Anti-Starspawn Ointment.txt — en favori : les ennemis de l'Astral Infection ne lâchent plus d'armes ni
    /// d'accessoires. [choix] Vérifié sur le joueur le plus proche de l'ennemi tué, s'il est dans l'Astral Infection.
    /// </summary>
    public class AntiStarspawnOintment : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 36;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(silver: 50);
        }
    }

    /// <summary>Ointments en favori (Anti-Moth, Anti-Starspawn).</summary>
    public class OintmentSystem : GlobalNPC
    {
        public static bool Favorited(Player player, int type) => player.inventory.Any(i => i.type == type && i.favorited);

        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            if (Favorited(spawnInfo.Player, ModContent.ItemType<AntiMothOintment>()))
                pool.Remove(NPCID.EmpressButterfly);
        }
    }

    public class StarspawnLootBlock : GlobalItem
    {
        public override void OnSpawn(Item item, IEntitySource source)
        {
            if (source is not EntitySource_Loot { Entity: NPC npc } || (item.damage <= 0 && !item.accessory) || item.ammo > 0 || item.consumable)
                return;
            Player p = Main.player[Player.FindClosest(npc.position, npc.width, npc.height)];
            if (p.active && p.Calamity().ZoneAstral && OintmentSystem.Favorited(p, ModContent.ItemType<AntiStarspawnOintment>()))
                item.TurnToAir();
        }
    }

    /// <summary>Wiki : Catastrophe Claybore.txt — foreuse en Catastrophe Clay, pioche 190.</summary>
    public class CatastropheClaybore : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.IsDrill[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 14;
            Item.DamageType = DamageClass.Melee;
            Item.width = 50;
            Item.height = 22;
            Item.useTime = 4;
            Item.useAnimation = 15;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.pick = 190;
            Item.tileBoost = -1;
            Item.knockBack = 18f;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item23;
            Item.autoReuse = true;
            Item.rare = ModContent.RarityType<CalamityMod.Rarities.BurnishedAuric>();
            Item.value = Item.sellPrice(gold: 1, silver: 22);
            Item.shoot = ModContent.ProjectileType<CatastropheClayboreProj>();
            Item.shootSpeed = 40f;
        }
    }

    public class CatastropheClayboreProj : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Tools/CatastropheClaybore";

        public override void SetStaticDefaults() => ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            Projectile.ownerHitCheck = true;
            Projectile.aiStyle = ProjAIStyleID.Drill;
            Projectile.DamageType = DamageClass.Melee;
        }
    }

    /// <summary>Wiki : Dusty Axe.txt — hache d'avant Hardmode (148 %), 10 % de critique, sans effet.</summary>
    public class DustyAxe : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 50;
            Item.DamageType = DamageClass.Melee;
            Item.width = Item.height = 48;
            Item.useTime = 8;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.axe = 30;
            Item.knockBack = 5f;
            Item.crit = 6;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(silver: 1);
        }
    }

    /// <summary>
    /// Wiki : Rod of Misery.txt — téléporte à l'opposé du curseur : symétrique en X, en Y ou les deux, au hasard ; Chaos State
    /// 15 s, pendant lequel la téléportation ne marche pas.
    /// </summary>
    public class RodofMisery : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 42;
            Item.useTime = Item.useAnimation = 28;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item6;
            Item.rare = ModContent.RarityType<CalamityMod.Rarities.HotPink>();
            Item.value = Item.sellPrice(gold: 32);
        }

        public override bool CanUseItem(Player player) => !player.HasBuff(BuffID.ChaosState);

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI != Main.myPlayer)
                return true;
            Vector2 offset = Main.MouseWorld - player.Center;
            int mode = Main.rand.Next(3);
            if (mode != 1)
                offset.X = -offset.X;
            if (mode != 0)
                offset.Y = -offset.Y;
            Vector2 dest = player.Center + offset - player.Size / 2f;
            if (dest.X > 50f && dest.X < Main.maxTilesX * 16 - 50 && dest.Y > 50f && dest.Y < Main.maxTilesY * 16 - 50
                && !Collision.SolidCollision(dest, player.width, player.height))
            {
                player.Teleport(dest, TeleportationStyleID.RodOfDiscord);
                NetMessage.SendData(MessageID.TeleportEntity, -1, -1, null, 0, player.whoAmI, dest.X, dest.Y, TeleportationStyleID.RodOfDiscord);
                player.AddBuff(BuffID.ChaosState, 900);
            }
            return true;
        }
    }
}

namespace CalamityFandom.Content.Items.Ammo
{
    /// <summary>
    /// Wiki : Jellyfish Cannonball.txt — en la fabriquant, on n'obtient rien : un Cannonball Jellyfish apparaît à la place.
    /// TODO : munition du Cannon vanilla (boulet chercheur qui explose) : le canon placé n'accepte que ses boulets.
    /// </summary>
    public class JellyfishCannonball : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 99;

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 30;
            Item.damage = 500;
            Item.DamageType = DamageClass.Ranged;
            Item.knockBack = 40f;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(silver: 10);
        }

        public override void OnCreated(ItemCreationContext context)
        {
            if (context is not RecipeItemCreationContext)
                return;
            Player player = Main.LocalPlayer;
            SoundEngine.PlaySound(SoundID.Item86, player.Center);
            if (Main.netMode == NetmodeID.SinglePlayer)
                NPC.NewNPC(player.GetSource_FromThis(), (int)player.Center.X + 60 * player.direction, (int)player.Center.Y, ModContent.NPCType<CannonballJellyfish>());
            else
                NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: ModContent.NPCType<CannonballJellyfish>());
            Item.TurnToAir();
        }
    }
}

namespace CalamityFandom.Content.Items.Armor
{
    /// <summary>
    /// Wiki : Wulfrum Mining Helmet.txt — 3 défense, effets du Wulfrum Hardhat (immunité aux pièges) et des Wulfrum Lenses
    /// (un faisceau de lumière bleue devant le joueur qui montre les trésors). [choix] Faisceau = lumière bleue devant le
    /// joueur et effet de la Spelunker Potion.
    /// </summary>
    [AutoloadEquip(EquipType.Head)]
    public class WulfrumMiningHelmet : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 20;
            Item.defense = 3;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 60);
        }

        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<HardhatPlayer>().TrapImmune = true;
            player.findTreasure = true;
            for (int i = 1; i <= 6; i++)
                Lighting.AddLight(player.Center + new Vector2(player.direction * i * 32f, 0f), 0.2f, 0.45f, 0.8f);
        }
    }
}
