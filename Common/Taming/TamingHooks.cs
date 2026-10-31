using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CalamityMod;
using CalamityMod.NPCs.Abyss;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.NPCs.Providence;
using CalamityFandom.Content.NPCs.Perforators;
using Microsoft.Xna.Framework;
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
        /// <summary>Ballon de plage (Golem) : contacts du tick précédent, délai avant le prochain coup compté, immobilité.</summary>
        public bool TouchedPlayer, TouchedGolem;
        public int HitCooldown, StillTicks;

        /// <summary>Golem vivant et apprivoisable (vu aussi des clients : la liste des boss tués est synchronisée).</summary>
        public static bool GolemTamable()
        {
            var e = TamingSystem.Get("Golem");
            return e != null && !TamingSystem.Untamable.Contains(e.Key) && TamingSystem.FindMain(e) != null;
        }

        // [validé 31/10] Le ballon ne doit pas rester bloqué : pendant une tentative, il ne disparaît pas et, immobile 3 s,
        // il est relancé en l'air (côté propriétaire du projectile, qui le simule)
        public override void PostAI(Projectile projectile)
        {
            if (projectile.type != ProjectileID.BeachBall || projectile.owner != Main.myPlayer || !GolemTamable())
                return;
            projectile.timeLeft = System.Math.Max(projectile.timeLeft, 600);
            if (projectile.velocity.Length() > 0.5f)
            {
                StillTicks = 0;
                return;
            }
            if (++StillTicks < 3 * 60)
                return;
            StillTicks = 0;
            projectile.velocity = new Vector2(Main.rand.NextFloat(-4f, 4f), -10f);
            projectile.netUpdate = true;
        }

        // Les combats ne sont suivis que par le serveur : ici, seulement « le boss est apprivoisable » (liste synchronisée)
        public override bool? CanHitNPC(Projectile projectile, NPC target) =>
            TamingSystem.EntryOf(target.type) is { } e && !TamingSystem.Untamable.Contains(e.Key) && TamingMethods.IsMethodProjectile(projectile.type, target.type)
                ? false : null;
    }

    public class TamingGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public const string LeviathanKey = "Leviathan and Anahita";
        public const string SlimeGodKey = "The Slime God";
        /// <summary>
        /// Agrandissement (King Slime, Queen Slime), calculé par le serveur et envoyé aux clients. [technique] Appliqué au
        /// dessin (PreDraw/PostDraw) et à la hitbox après l'IA, sans toucher à NPC.scale : l'IA de King Slime de Calamity
        /// recalcule scale à chaque tick dans son PreAI, qui passe avant le nôtre.
        /// </summary>
        private float grown = 1f;
        private float drawnScale;
        public const float MaxGrowth = 0.5f;

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
            // « Spawning a goldfish during the fight will cause Leviathan to awaken early » : Anahita appelle Leviathan
            // comme à 40 % de vie (projectile LeviathanSpawner de Calamity)
            if ((npc.type == NPCID.Goldfish || npc.type == NPCID.GoldfishWalker) && TamingSystem.Active(LeviathanKey) != null)
                foreach (var ana in Main.ActiveNPCs)
                    if (ana.ModNPC is Anahita anahita && !anahita.HasBegunSummoningLeviathan && !NPC.AnyNPCs(ModContent.NPCType<CalamityMod.NPCs.Leviathan.Leviathan>()))
                    {
                        Projectile.NewProjectile(ana.GetSource_FromAI(), ana.Center, Vector2.Zero, ModContent.ProjectileType<CalamityMod.Projectiles.Boss.LeviathanSpawner>(), 0, 0f);
                        anahita.HasBegunSummoningLeviathan = true;
                        ana.netUpdate = true;
                    }
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
            return true;
        }

        // King Slime « will grow larger as he is sprayed, reaching a maximum size of 100% larger » ; Queen Slime « will grow
        // 1% larger with each balloon ». [validé 31/10] Plafonné à ×1,5 : King Slime est déjà grand (Calamity le dessine
        // jusqu'à ×1,5 selon sa vie), ×2 en plus était bien trop gros à l'écran.
        public override void PostAI(NPC npc)
        {
            if (npc.type != NPCID.KingSlime && npc.type != NPCID.QueenSlimeBoss)
                return;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                var e = TamingSystem.EntryOf(npc.type);
                float target = e != null && TamingSystem.Active(e.Key) is { } f ? 1f + MaxGrowth * System.Math.Clamp(f.Progress / e.Goal, 0f, 1f) : 1f;
                if (System.Math.Abs(target - grown) >= 0.01f || target == 1f && grown != 1f)
                {
                    grown = target;
                    npc.netUpdate = true;
                }
            }
            // Hitbox de King Slime (formule de Calamity : 98 × 92 × scale), agrandie par le bas, comme le fait son IA
            if (npc.type == NPCID.KingSlime)
            {
                int width = (int)(98f * npc.scale * grown), height = (int)(92f * npc.scale * grown);
                if (width != npc.width || height != npc.height)
                {
                    npc.position.X += npc.width / 2 - width / 2;
                    npc.position.Y += npc.height - height;
                    npc.width = width;
                    npc.height = height;
                }
            }
        }

        public override bool PreDraw(NPC npc, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            drawnScale = npc.scale;
            if (grown != 1f)
                npc.scale *= grown;
            return true;
        }

        public override void PostDraw(NPC npc, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) =>
            npc.scale = drawnScale;

        public override void SendExtraAI(NPC npc, Terraria.ModLoader.IO.BitWriter bitWriter, System.IO.BinaryWriter binaryWriter)
        {
            if (npc.type == NPCID.KingSlime || npc.type == NPCID.QueenSlimeBoss)
                binaryWriter.Write(grown);
        }

        public override void ReceiveExtraAI(NPC npc, Terraria.ModLoader.IO.BitReader bitReader, System.IO.BinaryReader binaryReader)
        {
            if (npc.type == NPCID.KingSlime || npc.type == NPCID.QueenSlimeBoss)
                grown = binaryReader.ReadSingle();
        }

        // Colossal Squid (Moon Lord) et wyvernes (Storm Weaver) « will ignore boss zen » : on annule la réduction de Calamity
        private static int ZenException(Player player)
        {
            if (TamingSystem.Active("Moon Lord") != null && (player.Calamity().ZoneAbyssLayer3 || player.Calamity().ZoneAbyssLayer4))
                return ModContent.NPCType<ColossalSquid>();
            if (TamingSystem.Active("Storm Weaver") != null && player.ZoneSkyHeight)
                return 0;
            // « Blight slimes will ignore boss zen while the Slime God is alive »
            if (TamingSystem.Active(SlimeGodKey) != null && (player.ZoneCorrupt || player.ZoneCrimson))
                return player.ZoneCorrupt ? ModContent.NPCType<CalamityMod.NPCs.NormalNPCs.EbonianBlightSlime>()
                    : ModContent.NPCType<CalamityMod.NPCs.NormalNPCs.CrimulanBlightSlime>();
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
            // « Consume 3 Food items throughout the fight » : l'aliment compte quand son Well Fed s'achève (TamingPlayer)
            if (TamingPlayer.IsWellFed(item.buffType) && TamingSystem.Get(TamingPlayer.EaterKey) is { } eow && TamingSystem.FindMain(eow) != null)
            {
                var taming = player.GetModPlayer<TamingPlayer>();
                taming.PendingFood = item.type;
                // [validé 31/10] Calamity++ : prévenu dès qu'on le mange (et encore à la fin du Well Fed)
                if (TamingSystem.EaterDistinctFoods && taming.AlreadyEaten(item.type))
                    TamingPlayer.TellSameFood(item.type);
            }
            // « Consume a Yharic Delight while Crabulon is passive right after spawning » : la fenêtre de 5 s est
            // vérifiée là où le combat est suivi (TamingSystem.AddProgress)
            if (item.type == WikiData.WikiRecipes.ResolveItem("Yharic Delight") && NPC.AnyNPCs(ModContent.NPCType<Crabulon>()))
                TamingSystem.Report("Crabulon", 1f);
        }
    }

    public class TamingPlayer : ModPlayer
    {
        /// <summary>Partie d'échecs contre un boss en cours (Holographic Chessboard), connue de tous : le monde est figé.</summary>
        public bool ChessOpen;
        /// <summary>Début de la partie (Main.GameUpdateCount) et pied de la table (Chess/ChessScene.cs).</summary>
        public uint ChessStart;
        public Vector2 ChessTable;

        // Partie d'échecs : plus aucun contrôle, plus de mouvement, plus de dégâts (Chess/ChessScene.cs)
        public override void SetControls()
        {
            if (!Chess.ChessScene.Frozen)
                return;
            Player.controlLeft = Player.controlRight = Player.controlUp = Player.controlDown = false;
            Player.controlJump = Player.controlUseItem = Player.controlUseTile = Player.controlThrow = false;
            Player.controlHook = Player.controlMount = Player.controlQuickHeal = Player.controlQuickMana = false;
            Player.controlSmart = Player.controlTorch = false;
        }

        public override void PreUpdateMovement()
        {
            if (!Chess.ChessScene.Frozen)
                return;
            Player.velocity = Vector2.Zero;
            Player.SetImmuneTimeForAllTypes(2);
        }
        private bool dashCounted;

        public const string EaterKey = "Eater of Worlds";
        /// <summary>Dernier aliment mangé pendant le combat contre l'Eater of Worlds, en attente de la fin de son Well Fed.</summary>
        public int PendingFood;
        /// <summary>Durée restante du Well Fed au tick précédent (-1 : pas de Well Fed).</summary>
        private int lastWellFedTime = -1;
        /// <summary>Aliments déjà comptés pendant ce combat (Calamity++ : 3 différents).</summary>
        private readonly HashSet<int> eatenFoods = new();

        public static bool IsWellFed(int buff) => buff is BuffID.WellFed or BuffID.WellFed2 or BuffID.WellFed3;

        public bool AlreadyEaten(int food) => eatenFoods.Contains(food);

        public static void TellSameFood(int food) =>
            Main.NewText(Language.GetTextValue("Mods.CalamityFandom.Taming.Food.Same", Lang.GetItemNameValue(food)), Color.LightPink);

        /// <summary>
        /// Eater of Worlds : « Consume 3 Food items throughout the fight. Note that the Well Fed buff must run out before
        /// another food item can be counted as eaten ». [validé 31/10] Un aliment mangé pendant le combat compte quand son
        /// Well Fed s'achève de lui-même : le retirer d'un clic droit l'annule. Calamity++ (Balance/taming.json) : 3
        /// aliments différents. [choix] Si on remange pendant un Well Fed, c'est le dernier aliment qui compte.
        /// </summary>
        public override void PostUpdateBuffs()
        {
            if (Player.whoAmI != Main.myPlayer)
                return;
            var eow = TamingSystem.Get(EaterKey);
            if (eow == null || TamingSystem.FindMain(eow) == null)
            {
                PendingFood = 0;
                eatenFoods.Clear();
            }
            int time = -1;
            for (int i = 0; i < Player.MaxBuffs; i++)
                if (IsWellFed(Player.buffType[i]) && Player.buffTime[i] > 0)
                    time = Player.buffTime[i];
            if (time < 0 && lastWellFedTime >= 0 && PendingFood > 0)
            {
                // Le buff a disparu : fin normale (il restait 1 ou 2 ticks), ou retiré d'un clic droit
                if (lastWellFedTime > 2)
                    Main.NewText(Language.GetTextValue("Mods.CalamityFandom.Taming.Food.Cancelled"), Color.LightPink);
                else if (TamingSystem.EaterDistinctFoods && !eatenFoods.Add(PendingFood))
                    TellSameFood(PendingFood);
                else
                {
                    eatenFoods.Add(PendingFood);
                    CombatText.NewText(Player.Hitbox, Color.HotPink, Language.GetTextValue("Mods.CalamityFandom.Taming.Food.Digested",
                        Lang.GetItemNameValue(PendingFood)));
                    TamingSystem.Report(EaterKey, 1f);
                }
                PendingFood = 0;
            }
            lastWellFedTime = time;
        }

        // « Dash directly into the Destroyers head segment with a dash 5 times. Note that the dash must not deal damage »
        public override void PostUpdate()
        {
            // Poignée de main avec un Creeper au début de la partie d'échecs : le bras avant se tend et monte, descend
            uint age = Main.GameUpdateCount - ChessStart;
            if (ChessOpen && age <= Chess.ChessScene.IntroTicks)
            {
                Vector2 toHand = Chess.ChessScene.HandPosition(Player, age) - Player.MountedCenter;
                Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, toHand.ToRotation() - MathHelper.PiOver2);
            }
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
            // [validé 31/10] « Only one boss can be soothed per day » : toutes les offres disparaissent de la boutique
            // ouverte jusqu'au prochain jour (le serveur confirme ensuite par la synchronisation du monde)
            TamingSystem.SoothedToday = true;
            for (int i = 0; i < shopInventory.Length; i++)
                if (shopInventory[i]?.ModItem is SoothingService)
                    shopInventory[i].TurnToAir();
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
            HungaryDelight(message.Text, clientId);
            const string key = "The Devourer of Gods";
            if (string.IsNullOrWhiteSpace(message.Text) || TamingSystem.Active(key) == null)
                return;
            float points = Providence.IsMatch(message.Text) ? 5f : 1f;
            points += Insults.Matches(message.Text).Count;
            TamingSystem.AddProgress(key, points);
        }

        public const string HungaryLine = "Yo, I'm Hungary !!";
        /// <summary>Yharic Delight déjà donné pendant la visite actuelle du Traveling Merchant.</summary>
        private static bool served;

        /// <summary>
        /// [validé 31/10] Calamity++ (Balance/taming.json) : taper exactement « Yo, I'm Hungary !! » à moins de 10 blocs du
        /// Traveling Merchant ; il répond et donne un Yharic Delight (Taming de Crabulon avant la fin du jeu). [validé 31/10]
        /// Un seul par passage en ville (tous joueurs confondus). [choix] Apostrophe typographique acceptée, espaces autour
        /// ignorés, majuscules exigées.
        /// </summary>
        private static void HungaryDelight(string text, int clientId)
        {
            if (!TamingSystem.HungaryDelight || text == null || text.Replace('’', '\'').Trim() != HungaryLine)
                return;
            int who = Main.netMode == NetmodeID.SinglePlayer ? Main.myPlayer : clientId;
            if (who < 0 || who >= Main.maxPlayers)
                return;
            Player player = Main.player[who];
            NPC merchant = null;
            foreach (var n in Main.ActiveNPCs)
                if (n.type == NPCID.TravellingMerchant && n.WithinRange(player.Center, 160f))
                    merchant = n;
            if (merchant == null || served)
                return;
            int delight = WikiData.WikiRecipes.ResolveItem("Yharic Delight");
            if (delight <= 0)
                return;
            served = true;
            player.QuickSpawnItem(new EntitySource_Gift(merchant), delight);
            var reply = NetworkText.FromKey("Mods.CalamityFandom.Taming.Hungary", player.name);
            if (Main.netMode == NetmodeID.Server)
                ChatHelper.BroadcastChatMessage(reply, Color.LightGreen);
            else
                Main.NewText(reply.ToString(), Color.LightGreen);
        }

        // Le marchand est reparti : le prochain passage redonne droit à un Yharic Delight
        public override void PostUpdateNPCs()
        {
            if (served && !NPC.AnyNPCs(NPCID.TravellingMerchant))
                served = false;
        }

        public override void ClearWorld() => served = false;
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
