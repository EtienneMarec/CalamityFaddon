using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.NPCs.Critters
{
    /// <summary>
    /// Wiki : Gullet Worm.txt — bestiole de la Blood Moon : 10 % de chances d'apparaître en cassant un objet de décor qui peut
    /// donner une bestiole ; rampe comme un ver ; s'attrape au filet. [choix] Objets de décor : les tas de cailloux et de
    /// débris (petits et grands), pas ceux posés avec le Rubblemaker (page). Un seul dessin (le wiki n'en montre qu'un).
    /// </summary>
    public class GulletWormNPC : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcCatchable[Type] = true;
            NPCID.Sets.CountsAsCritter[Type] = true;
            NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 14;
            NPC.height = 8;
            NPC.aiStyle = NPCAIStyleID.CritterWorm;
            AIType = NPCID.Worm;
            NPC.damage = 0;
            NPC.defense = 1;
            NPC.lifeMax = 20;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.npcSlots = 0.1f;
            NPC.catchItem = ModContent.ItemType<Items.Critters.GulletWorm>();
            // Le Lifeform Analyzer le repère (page, « Notes »)
            NPC.rarity = 1;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) =>
            bestiaryEntry.Info.Add(BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.BloodMoon);
    }

    /// <summary>Apparition du Gullet Worm : 10 % quand un tas de cailloux ou de débris naturel est cassé pendant une Blood Moon.</summary>
    public class GulletWormSpawn : GlobalTile
    {
        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (fail || effectOnly || Main.netMode == NetmodeID.MultiplayerClient || !Main.bloodMoon)
                return;
            if (type != TileID.SmallPiles && type != TileID.LargePiles && type != TileID.LargePiles2)
                return;
            // Les décors posés avec le Rubblemaker sont des tuiles à part (ne donnent pas de bestiole)
            if (!Main.rand.NextBool(10))
                return;
            int worm = NPC.NewNPC(new EntitySource_TileBreak(i, j), i * 16 + 8, j * 16, ModContent.NPCType<GulletWormNPC>());
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.SyncNPC, number: worm);
        }
    }
}

namespace CalamityFandom.Content.Items.Critters
{
    /// <summary>
    /// Gullet Worm attrapé : appât de puissance 120 ; se relâche. [choix] « Higher chance to catch enemies during a Blood
    /// Moon » : 1 chance sur 6 de plus de remonter un ennemi de la Blood Moon (ceux du jeu, selon la progression).
    /// </summary>
    public class GulletWorm : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 5;

        public override void SetDefaults()
        {
            Item.DefaultToCapturedCritter(ModContent.NPCType<NPCs.Critters.GulletWormNPC>());
            Item.width = 28;
            Item.height = 18;
            Item.bait = 120;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(silver: 10);
            Item.rare = ItemRarityID.Orange;
        }
    }

    public class GulletWormFishing : ModPlayer
    {
        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            if (npcSpawn > 0 || !Main.bloodMoon || Main.dayTime || attempt.playerFishingConditions.BaitItemType != ModContent.ItemType<GulletWorm>()
                || attempt.inLava || attempt.inHoney || !Main.rand.NextBool(6))
                return;
            npcSpawn = Main.hardMode && Main.rand.NextBool(3)
                ? Main.rand.NextBool() ? NPCID.GoblinShark : NPCID.BloodEelHead
                : Main.rand.NextBool() ? NPCID.ZombieMerman : NPCID.EyeballFlyingFish;
            itemDrop = -1;
        }
    }
}
