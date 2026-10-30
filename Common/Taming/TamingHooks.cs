using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CalamityMod;
using CalamityMod.NPCs.Abyss;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.Providence;
using CalamityFandom.Content.NPCs.Perforators;
using Terraria;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityFandom.Common.Taming
{
    /// <summary>Projectiles des méthodes (Taming) : comptés une seule fois, sans blesser le boss.</summary>
    public class TamingGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public bool Counted;

        // Les combats ne sont suivis que par le serveur : ici, seulement « le boss est apprivoisable » (liste synchronisée)
        public override bool? CanHitNPC(Projectile projectile, NPC target) =>
            TamingSystem.EntryOf(target.type) is { } e && !TamingSystem.Untamable.Contains(e.Key) && TamingMethods.IsMethodProjectile(projectile.type, target.type)
                ? false : null;
    }

    public class TamingGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        /// <summary>Agrandissement appliqué (King Slime, Queen Slime), retiré avant l'IA du tick suivant.</summary>
        private float grown = 1f;

        // « Killing a boss will make it untamable »
        public override void OnKill(NPC npc)
        {
            // « The Estranged Sons cannot be killed for the tame to work »
            if (npc.type == ModContent.NPCType<EstrangedSonHead>() && TamingSystem.Fights.TryGetValue("The Perforators", out var perf))
                perf.Harmed = true;
            if (TamingSystem.TamingNow || TamingSystem.EntryOf(npc.type) is not { } e || !TamingSystem.Untamable.Add(e.Key))
                return;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);
        }

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            // « Summon Providence, the Profaned Goddess while the Profaned Guardians are alive »
            if (npc.type == ModContent.NPCType<Providence>())
                TamingSystem.AddProgress("Profaned Guardians", 1f);
            // « Spawn 50 worms during the fight. Each worm will transform into an Estranged Son after around 5 seconds »
            // (ver relâché, ou sorti d'une Worm Statue)
            if (npc.type == NPCID.Worm && TamingSystem.Active("The Perforators") != null)
                wormTransform = 5 * 60;
            // Colossal Squid apparu pendant le combat du Moon Lord
            if (npc.type == ModContent.NPCType<ColossalSquid>() && TamingSystem.Active("Moon Lord") is { } ml)
                ml.Flag = true;
        }

        private int wormTransform;

        public override bool PreAI(NPC npc)
        {
            if (wormTransform > 0 && --wormTransform == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (TamingSystem.Active("The Perforators") != null)
                {
                    int son = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<EstrangedSonHead>());
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.SyncNPC, number: son);
                    TamingSystem.AddProgress("The Perforators", 1f);
                }
                npc.active = false;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.SyncNPC, number: npc.whoAmI);
                return false;
            }
            if (grown != 1f)
            {
                npc.scale /= grown;
                grown = 1f;
            }
            return true;
        }

        // King Slime « will grow larger as he is sprayed, reaching a maximum size of 100% larger » ; Queen Slime « will grow
        // 1% larger with each balloon » (jusqu'à +100 %)
        public override void PostAI(NPC npc)
        {
            if (npc.type != NPCID.KingSlime && npc.type != NPCID.QueenSlimeBoss)
                return;
            var e = TamingSystem.EntryOf(npc.type);
            if (e == null || TamingSystem.Active(e.Key) is not { } f)
                return;
            grown = 1f + System.Math.Clamp(f.Progress / e.Goal, 0f, 1f);
            npc.scale *= grown;
        }

        // Colossal Squid (Moon Lord) et wyvernes (Storm Weaver) « will ignore boss zen » : on annule la réduction de Calamity
        private static int ZenException(Player player)
        {
            if (TamingSystem.Active("Moon Lord") != null && (player.Calamity().ZoneAbyssLayer3 || player.Calamity().ZoneAbyssLayer4))
                return ModContent.NPCType<ColossalSquid>();
            if (TamingSystem.Active("Storm Weaver") != null && player.ZoneSkyHeight)
                return 0;
            return -1;
        }

        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            if (ZenException(player) < 0 || !player.Calamity().isNearbyBoss)
                return;
            spawnRate = System.Math.Max(1, spawnRate / 5);
            maxSpawns = System.Math.Max(maxSpawns, 3);
        }

        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            int only = ZenException(spawnInfo.Player);
            if (only < 0 || !spawnInfo.Player.Calamity().isNearbyBoss)
                return;
            foreach (int key in pool.Keys.ToList())
                if (key != only)
                    pool.Remove(key);
        }

        // Zoologist : « Soothing » (un boss par jour), à la suite de sa boutique
        public override void ModifyActiveShop(NPC npc, string shopName, Item[] items)
        {
            if (npc.type != NPCID.BestiaryGirl || TamingSystem.SoothedToday)
                return;
            int slot = 0;
            foreach (var e in TamingSystem.Entries.Where(e => TamingSystem.Untamable.Contains(e.Key)))
            {
                while (slot < items.Length && items[slot] != null && !items[slot].IsAir)
                    slot++;
                if (slot >= items.Length)
                    break;
                items[slot] = SoothingService.Create(e);
            }
        }
    }

    /// <summary>« Place 100 books during the fight » (Hive Mind) ; « Place 100 graves in the dungeon » (Polterghast).</summary>
    public class TamingGlobalTile : GlobalTile
    {
        public override void PlaceInWorld(int i, int j, int type, Item item)
        {
            if (type == TileID.Books && TamingSystem.Get("The Hive Mind") is { } hive && TamingSystem.FindMain(hive) != null)
                TamingSystem.Report(hive.Key, 1f);
            else if (type == TileID.Tombstones && Main.wallDungeon[Main.tile[i, j].WallType])
                TamingSystem.ReportGrave(Main.myPlayer);
        }
    }

    /// <summary>Nourriture (Eater of Worlds) et Yharic Delight (Crabulon).</summary>
    public class TamingGlobalItem : GlobalItem
    {
        public override void OnConsumeItem(Item item, Player player)
        {
            if (player.whoAmI != Main.myPlayer)
                return;
            // « Consume 3 Food items throughout the fight. Note that the Well Fed buff must run out before another food item
            // can be counted as eaten »
            bool food = item.buffType == BuffID.WellFed || item.buffType == BuffID.WellFed2 || item.buffType == BuffID.WellFed3;
            if (food && !player.GetModPlayer<TamingPlayer>().HadWellFed && TamingSystem.Get("Eater of Worlds") is { } eow && TamingSystem.FindMain(eow) != null)
                TamingSystem.Report(eow.Key, 1f);
            // « Consume a Yharic Delight while Crabulon is passive right after spawning »
            if (item.type == WikiData.WikiRecipes.ResolveItem("Yharic Delight"))
                foreach (var crab in Main.ActiveNPCs)
                    if (crab.type == ModContent.NPCType<Crabulon>() && crab.ai[0] == 0f)
                        TamingSystem.Report("Crabulon", 1f);
        }
    }

    public class TamingPlayer : ModPlayer
    {
        public bool HadWellFed;
        private bool dashCounted;

        public override void PreUpdate() =>
            HadWellFed = Player.HasBuff(BuffID.WellFed) || Player.HasBuff(BuffID.WellFed2) || Player.HasBuff(BuffID.WellFed3);

        // « Dash directly into the Destroyers head segment with a dash 5 times. Note that the dash must not deal damage »
        public override void PostUpdate()
        {
            if (Player.whoAmI != Main.myPlayer)
                return;
            if (Player.dashDelay >= 0)
            {
                dashCounted = false;
                return;
            }
            if (dashCounted)
                return;
            foreach (var head in Main.ActiveNPCs)
            {
                if (head.type == NPCID.TheDestroyer && head.Hitbox.Intersects(Player.Hitbox))
                {
                    dashCounted = true;
                    TamingSystem.Report("The Destroyer", 1f);
                    break;
                }
            }
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            if (Player.whoAmI == Main.myPlayer)
                TamingSystem.ReportHurt();
        }

        public override bool CanBuyItem(NPC vendor, Item[] shopInventory, Item item) =>
            item.ModItem is not SoothingService s || !TamingSystem.SoothedToday && TamingSystem.Untamable.Contains(s.Key);

        public override void PostBuyItem(NPC vendor, Item[] shopInventory, Item item)
        {
            if (item.ModItem is not SoothingService s)
                return;
            TamingSystem.ReportSoothe(s.Key);
            item.TurnToAir();
            if (Main.mouseItem.ModItem is SoothingService)
                Main.mouseItem.TurnToAir();
        }
    }

    /// <summary>
    /// « Send 150 messages in chat » (The Devourer of Gods) : les mots « DoG », « sucks », « stupid », « bad boss » et
    /// « worm boss » comptent un point de plus chacun, « providence » compte 5 points. Lu là où les messages sont traités
    /// (serveur, ou solo). TODO : « providence » doit aussi enrager le Devourer of Gods.
    /// </summary>
    public class TamingChat : ModSystem
    {
        private static readonly Regex Insults = new(@"\b(dog|sucks|stupid|bad boss|worm boss)\b", RegexOptions.IgnoreCase);
        private static readonly Regex Providence = new(@"\bprovidence\b", RegexOptions.IgnoreCase);

        public override void Load() => On_ChatCommandProcessor.ProcessIncomingMessage += OnMessage;

        private static void OnMessage(On_ChatCommandProcessor.orig_ProcessIncomingMessage orig, ChatCommandProcessor self, ChatMessage message, int clientId)
        {
            orig(self, message, clientId);
            const string key = "The Devourer of Gods";
            if (string.IsNullOrWhiteSpace(message.Text) || TamingSystem.Active(key) == null)
                return;
            float points = Providence.IsMatch(message.Text) ? 5f : 1f;
            points += Insults.Matches(message.Text).Count;
            TamingSystem.AddProgress(key, points);
        }
    }

    /// <summary>Apaisement d'un boss vendu par le Zoologist (page Taming, section Soothing) : rien ne reste dans l'inventaire.</summary>
    public class SoothingService : ModItem
    {
        public string Key = "";

        public override string Texture => "Terraria/Images/Item_" + ItemID.LovePotion;

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Pink;
        }

        // Acheté en clic droit ou sans passer par la souris : ne reste jamais dans l'inventaire
        public override void UpdateInventory(Player player) => Item.TurnToAir();

        /// <summary>Prix choisis (la page n'en donne pas) : 2 or avant le Hardmode, 10 or en Hardmode, 50 or après le Moon Lord.</summary>
        public static readonly int[] Prices = { Item.buyPrice(gold: 2), Item.buyPrice(gold: 10), Item.buyPrice(gold: 50) };

        public static Item Create(TamingSystem.Entry e)
        {
            var item = new Item(ModContent.ItemType<SoothingService>());
            ((SoothingService)item.ModItem).Key = e.Key;
            item.shopCustomPrice = Prices[e.Tier];
            item.SetNameOverride(Language.GetTextValue("Mods.CalamityFandom.Taming.SootheName", e.Key));
            return item;
        }
    }
}
