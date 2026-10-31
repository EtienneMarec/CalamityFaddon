using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CalamityMod;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.CalClone;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.Cryogen;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.HiveMind;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs.PlaguebringerGoliath;
using CalamityMod.NPCs.Polterghast;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.Signus;
using CalamityMod.NPCs.StormWeaver;
using CalamityMod.NPCs.Yharon;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Chat;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityFandom.Common.Taming
{
    /// <summary>
    /// Wiki : Taming.txt — apprivoiser les boss sans les blesser : chaque boss a sa méthode ; réussie, il « paraît
    /// heureux », disparaît et laisse son butin. Blessé, il faut le laisser partir pour réessayer ; tué, il devient
    /// inapprivoisable jusqu'à ce que le Zoologist l'apaise (Soothing, un boss par jour). Superboss exclus.
    /// Méthodes des boss qui n'existent pas encore ou qui demandent du contenu absent : TODO dans <see cref="Entries"/>.
    /// L'état des combats vit sur le serveur (ou en solo) ; les événements vus seulement par un client arrivent par paquet.
    /// </summary>
    public class TamingSystem : ModSystem
    {
        /// <summary>Un boss apprivoisable : nom de la page, types de PNJ du combat (le premier porte le butin), objectif.</summary>
        public class Entry
        {
            public string Key;
            public int[] Types;
            public float Goal;
            /// <summary>0 avant le Hardmode, 1 Hardmode, 2 après le Moon Lord (prix de l'apaisement).</summary>
            public int Tier;
            /// <summary>PNJ qui peuvent porter le combat et le butin, par ordre de préférence (le premier type par défaut).</summary>
            public int[] Leads;
            public int Main => Types[0];
        }

        /// <summary>Combat en cours (oublié quand le boss disparaît).</summary>
        public class Fight
        {
            public bool Harmed;
            public float Progress;
            public float A, B, C;
            public bool Flag, WasDay;
            public readonly int[] Ints = new int[4];
            public readonly Dictionary<int, int> LastLife = new();
            public readonly HashSet<int> Seen = new();
            /// <summary>Dernière maison vue de chaque PNJ de ville (Brimstone Elemental).</summary>
            public readonly Dictionary<int, Point> Homes = new();
            public int ShownQuarter;
            /// <summary>Tick de début du combat (Main.GameUpdateCount).</summary>
            public uint Started;
        }

        public static readonly List<Entry> Entries = new();
        private static readonly Dictionary<string, Entry> ByKey = new();
        private static readonly Dictionary<int, Entry> ByType = new();
        public static readonly Dictionary<string, Fight> Fights = new();
        /// <summary>Boss tués (inapprivoisables jusqu'à l'apaisement), sauvegardés avec le monde.</summary>
        public static readonly HashSet<string> Untamable = new();
        /// <summary>Boss déjà apprivoisés dans ce monde (Supreme Calamitas demande tous les autres).</summary>
        public static readonly HashSet<string> Tamed = new();
        public static bool SoothedToday;
        /// <summary>Tombes posées dans le donjon (Polterghast apparaît apprivoisé à 100).</summary>
        public static int DungeonGraves;
        private static readonly HashSet<string> InstantTame = new();
        /// <summary>Vrai pendant le butin d'un boss apprivoisé (ne pas le compter comme tué).</summary>
        public static bool TamingNow;

        public const int GravesGoal = 100;
        /// <summary>
        /// [validé 31/10] Après l'avoir apprivoisée, la Queen Bee ne revient pas avant une demi-journée (43 200 ticks,
        /// 12 minutes) : les abeilles encore là la refaisaient apparaître en boucle. Sauvegardé avec le monde.
        /// </summary>
        public const int QueenBeeRestTicks = 43200;
        public static int QueenBeeRest;
        public const string SupremeCalamitasKey = "Supreme Calamitas";

        /// <summary>Tous les boss apprivoisables sauf Supreme Calamitas sont apprivoisés dans ce monde.</summary>
        public static bool AllOthersTamed() => Entries.All(e => e.Key == SupremeCalamitasKey || Tamed.Contains(e.Key));

        /// <summary>Demande d'apprivoisement immédiat au prochain combat (Supreme Calamitas après un « alt-f4 »).</summary>
        public static void TameOnSpawn(string key) => InstantTame.Add(key);
        private const float NearDistance = 1600f;

        private static void Add(string key, float goal, int tier, params int[] types)
        {
            var e = new Entry { Key = key, Types = types, Goal = goal, Tier = tier };
            Entries.Add(e);
            ByKey[key] = e;
            foreach (int type in types)
                ByType[type] = e;
        }

        public override void PostSetupContent()
        {
            // Pré-Hardmode. TODO : The Inventors, Wulfrum Excavator (boss inventés)
            Add("King Slime", 300 * 60, 0, NPCID.KingSlime);
            Add("Desert Scourge", 180, 0, ModContent.NPCType<DesertScourgeHead>(), ModContent.NPCType<DesertScourgeBody>(), ModContent.NPCType<DesertScourgeTail>());
            Add("Eye of Cthulhu", 30, 0, NPCID.EyeofCthulhu);
            Add("Crabulon", 1, 0, ModContent.NPCType<Crabulon>());
            // « Beat the Brain of Cthulhu at chess using the Holographic Chessboard » : 3 problèmes (Chess/ChessUI.cs)
            Add("Brain of Cthulhu", Chess.ChessSession.Needed, 0, NPCID.BrainofCthulhu, NPCID.Creeper);
            Add("Eater of Worlds", 3, 0, NPCID.EaterofWorldsHead, NPCID.EaterofWorldsBody, NPCID.EaterofWorldsTail);
            Add("The Hive Mind", 100, 0, ModContent.NPCType<HiveMind>());
            Add("The Perforators", 50, 0, ModContent.NPCType<PerforatorHive>(),
                ModContent.NPCType<PerforatorHeadSmall>(), ModContent.NPCType<PerforatorBodySmall>(), ModContent.NPCType<PerforatorTailSmall>(),
                ModContent.NPCType<PerforatorHeadMedium>(), ModContent.NPCType<PerforatorBodyMedium>(), ModContent.NPCType<PerforatorTailMedium>(),
                ModContent.NPCType<PerforatorHeadLarge>(), ModContent.NPCType<PerforatorBodyLarge>(), ModContent.NPCType<PerforatorTailLarge>());
            // « Bring 100 bees into a hive biome » : elle apparaît apprivoisée (TamingBees)
            Add(TamingBees.Key, 1, 0, NPCID.QueenBee);
            // « Arm Wrestle the Old Man » : bras de fer (OldManWrestle), Skeletron apparaît apprivoisé
            Add(OldManWrestle.Key, 1, 0, NPCID.SkeletronHead, NPCID.SkeletronHand);
            // « Have both slime gods absorb 20 of their respective Blight Slime » ; le butin est sur le noyau
            Add(TamingGlobalNPC.SlimeGodKey, 20, 0, ModContent.NPCType<CalamityMod.NPCs.SlimeGod.SlimeGodCore>(),
                ModContent.NPCType<CalamityMod.NPCs.SlimeGod.EbonianPaladin>(), ModContent.NPCType<CalamityMod.NPCs.SlimeGod.CrimulanPaladin>(),
                ModContent.NPCType<CalamityMod.NPCs.SlimeGod.SplitEbonianPaladin>(), ModContent.NPCType<CalamityMod.NPCs.SlimeGod.SplitCrimulanPaladin>());
            Add("Wall of Flesh", 1, 0, NPCID.WallofFlesh, NPCID.WallofFleshEye);
            // Hardmode. TODO : Cumulomenace, Pyrogen, The Derellect, Polyphemalus (inventés)
            Add("Queen Slime", 100, 1, NPCID.QueenSlimeBoss);
            Add("Cryogen", 180, 1, ModContent.NPCType<Cryogen>());
            // « Have five new Town NPCs move into the brimstone crag while the Brimstone Elemental is alive »
            Add("Brimstone Elemental", 5, 1, ModContent.NPCType<CalamityMod.NPCs.BrimstoneElemental.BrimstoneElemental>());
            Add("The Twins", 60 * 60, 1, NPCID.Retinazer, NPCID.Spazmatism);
            Add("The Destroyer", 5, 1, NPCID.TheDestroyer, NPCID.TheDestroyerBody, NPCID.TheDestroyerTail);
            Add("Skeletron Prime", 60 * 60, 1, NPCID.SkeletronPrime, NPCID.PrimeCannon, NPCID.PrimeSaw, NPCID.PrimeVice, NPCID.PrimeLaser);
            // « Spawn 100 goldfish and allow Leviathan to kill them » (Goldfish appelant Leviathan : TamingGlobalNPC.OnSpawn)
            Add(TamingGlobalNPC.LeviathanKey, 100, 1, ModContent.NPCType<CalamityMod.NPCs.Leviathan.Leviathan>(),
                ModContent.NPCType<CalamityMod.NPCs.Leviathan.Anahita>());
            Get(TamingGlobalNPC.LeviathanKey).Leads = new[] { ModContent.NPCType<CalamityMod.NPCs.Leviathan.Leviathan>(),
                ModContent.NPCType<CalamityMod.NPCs.Leviathan.Anahita>() };
            Add("Aquatic Scourge", 180, 1, ModContent.NPCType<AquaticScourgeHead>(), ModContent.NPCType<AquaticScourgeBody>(),
                ModContent.NPCType<AquaticScourgeBodyAlt>(), ModContent.NPCType<AquaticScourgeTail>());
            Add("Plantera", 180 * 60, 1, NPCID.Plantera);
            // « Hit a beach ball 50 times while Golem is alive. Beachball hits by Golem will count towards this total »
            Add("Golem", 50, 1, NPCID.Golem, NPCID.GolemHead, NPCID.GolemHeadFree, NPCID.GolemFistLeft, NPCID.GolemFistRight);
            Add("Calamitas", 60, 1, ModContent.NPCType<CalamitasClone>());
            Add("The Plaguebringer Goliath", 200, 1, ModContent.NPCType<PlaguebringerGoliath>());
            Add("Duke Fishron", 1, 1, NPCID.DukeFishron);
            Add("Empress of Light", 1, 1, NPCID.HallowBoss);
            Add("Ravager", 1, 1, ModContent.NPCType<RavagerBody>(), ModContent.NPCType<RavagerClawLeft>(), ModContent.NPCType<RavagerClawRight>(),
                ModContent.NPCType<RavagerLegLeft>(), ModContent.NPCType<RavagerLegRight>(), ModContent.NPCType<RavagerHead>());
            // « Use Draedon's Remote while Astrum Aureus is alive » (Content/Legacy/Items/SummonItems/DraedonsRemote.cs)
            Add("Astrum Aureus", 1, 1, ModContent.NPCType<CalamityMod.NPCs.AstrumAureus.AstrumAureus>());
            Add("Astrum Deus", 1, 1, ModContent.NPCType<AstrumDeusHead>(), ModContent.NPCType<AstrumDeusBody>(), ModContent.NPCType<AstrumDeusTail>());
            Add("The Dragonfolly", 1, 1, ModContent.NPCType<Dragonfolly>());
            Add("Lunatic Cultist", 1, 1, NPCID.CultistBoss);
            Add("Moon Lord", 1, 1, NPCID.MoonLordCore, NPCID.MoonLordHead, NPCID.MoonLordHand);
            // Après le Moon Lord. TODO : Yharim Worm, The Wall of Bronze, XP-00 Hypnos (inventés)
            Add("Profaned Guardians", 1, 2, ModContent.NPCType<ProfanedGuardianCommander>(), ModContent.NPCType<ProfanedGuardianDefender>(),
                ModContent.NPCType<ProfanedGuardianHealer>());
            Add("Providence, the Profaned Goddess", 4, 2, ModContent.NPCType<Providence>());
            Add("Ceaseless Void", 50, 2, ModContent.NPCType<CeaselessVoid>());
            Add("Storm Weaver", 10, 2, ModContent.NPCType<StormWeaverHead>(), ModContent.NPCType<StormWeaverBody>(), ModContent.NPCType<StormWeaverTail>());
            Add("Signus, Envoy of the Devourer", 8 * 60 * 60, 2, ModContent.NPCType<Signus>());
            Add("Polterghast", 1, 2, ModContent.NPCType<Polterghast>());
            Add("The Devourer of Gods", 150, 2, ModContent.NPCType<DevourerofGodsHead>(), ModContent.NPCType<DevourerofGodsBody>(), ModContent.NPCType<DevourerofGodsTail>());
            Add("Jungle Dragon, Yharon", 5 * 60 * 60, 2, ModContent.NPCType<Yharon>());
            // « File Draedons Taxes » : bouton du Tax Collector pendant le combat (TamingTaxCollector)
            Add("The Fabricational Quartet", 1, 2, ModContent.NPCType<ThanatosHead>(), ModContent.NPCType<AresBody>(), ModContent.NPCType<Apollo>(),
                ModContent.NPCType<Artemis>(), ModContent.NPCType<ThanatosBody1>(), ModContent.NPCType<ThanatosBody2>(), ModContent.NPCType<ThanatosTail>(),
                ModContent.NPCType<AresLaserCannon>(), ModContent.NPCType<AresTeslaCannon>(), ModContent.NPCType<AresPlasmaFlamethrower>(),
                ModContent.NPCType<AresGaussNuke>());
            Get("The Fabricational Quartet").Leads = new[] { ModContent.NPCType<ThanatosHead>(), ModContent.NPCType<AresBody>(), ModContent.NPCType<Apollo>() };
            // « Tame all previous bosses, and then press alt-f4 once Calamitas is at 5% HP » : progrès = vie perdue (en %)
            Add(SupremeCalamitasKey, 100, 2, ModContent.NPCType<SupremeCalamitas>(), ModContent.NPCType<SupremeCataclysm>(),
                ModContent.NPCType<SupremeCatastrophe>(), ModContent.NPCType<SepulcherHead>(), ModContent.NPCType<SepulcherBody>(),
                ModContent.NPCType<SepulcherTail>(), ModContent.NPCType<SepulcherArm>(), ModContent.NPCType<SoulSeekerSupreme>(),
                ModContent.NPCType<BrimstoneHeart>());
            ApplyBalance();
        }

        private class BalanceFile
        {
            public Dictionary<string, float> Goals { get; set; } = new();
            public bool HungaryDelight { get; set; }
            public bool EaterDistinctFoods { get; set; }
        }

        /// <summary>Calamity++ : objectifs de Balance/taming.json (King Slime : 3 min au lieu de 5).</summary>
        private void ApplyBalance()
        {
            HungaryDelight = false;
            EaterDistinctFoods = false;
            if (!Config.FandomMode.IsPlus || !Mod.FileExists("Balance/taming.json"))
                return;
            var balance = System.Text.Json.JsonSerializer.Deserialize<BalanceFile>(Mod.GetFileBytes("Balance/taming.json"),
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            foreach (var (key, goal) in balance.Goals)
                if (Get(key) is { } e)
                    e.Goal = goal;
            HungaryDelight = balance.HungaryDelight;
            EaterDistinctFoods = balance.EaterDistinctFoods;
        }

        public override void Unload()
        {
            Entries.Clear();
            ByKey.Clear();
            ByType.Clear();
            Fights.Clear();
        }

        public static Entry Get(string key) => ByKey.TryGetValue(key, out var e) ? e : null;

        public static Entry EntryOf(int npcType) => ByType.TryGetValue(npcType, out var e) ? e : null;

        public static NPC FindMain(Entry e)
        {
            foreach (int lead in e.Leads ?? new[] { e.Main })
                foreach (var n in Terraria.Main.ActiveNPCs)
                    if (n.type == lead)
                        return n;
            return null;
        }

        /// <summary>Combat apprivoisable en cours pour ce boss (null sinon).</summary>
        public static Fight Active(string key) =>
            Fights.TryGetValue(key, out var f) && !f.Harmed && !Untamable.Contains(key) ? f : null;

        public override void ClearWorld()
        {
            Fights.Clear();
            Untamable.Clear();
            Tamed.Clear();
            InstantTame.Clear();
            SoothedToday = false;
            DungeonGraves = 0;
            QueenBeeRest = 0;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag["tamingQueenBeeRest"] = QueenBeeRest;
            tag["tamingUntamable"] = Untamable.ToList();
            tag["tamingSoothedToday"] = SoothedToday;
            tag["tamingGraves"] = DungeonGraves;
            tag["tamingTamed"] = Tamed.ToList();
        }

        public override void LoadWorldData(TagCompound tag)
        {
            Untamable.Clear();
            foreach (string key in tag.GetList<string>("tamingUntamable"))
                Untamable.Add(key);
            SoothedToday = tag.GetBool("tamingSoothedToday");
            DungeonGraves = tag.GetInt("tamingGraves");
            QueenBeeRest = tag.GetInt("tamingQueenBeeRest");
            Tamed.Clear();
            foreach (string key in tag.GetList<string>("tamingTamed"))
                Tamed.Add(key);
        }

        // La liste sert à la boutique du Zoologist, côté client
        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(SoothedToday);
            writer.Write(QueenBeeRest);
            writer.Write(Untamable.Count);
            foreach (string key in Untamable)
                writer.Write(key);
        }

        public override void NetReceive(BinaryReader reader)
        {
            SoothedToday = reader.ReadBoolean();
            QueenBeeRest = reader.ReadInt32();
            Untamable.Clear();
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
                Untamable.Add(reader.ReadString());
        }

        private static bool wasDay;

        public override void PostUpdateWorld()
        {
            // Repos de la Queen Bee (serveur ou solo ; les clients reçoivent la valeur avec les données du monde)
            if (QueenBeeRest > 0)
                QueenBeeRest--;
            // « Only one boss can be soothed per day » : remis à zéro à l'aube
            if (Terraria.Main.dayTime && !wasDay && SoothedToday)
            {
                SoothedToday = false;
                SyncWorld();
            }
            wasDay = Terraria.Main.dayTime;
        }

        private static void SyncWorld()
        {
            if (Terraria.Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);
        }

        // ---------- événements ----------

        private enum Packet : byte { Progress, Hurt, Soothe, Tamed, Grave, Chess, Wrestle }

        /// <summary>Progrès vu par ce client (nourriture, chat, ruée...) : appliqué ici en solo, envoyé au serveur sinon.</summary>
        public static void Report(string key, float amount)
        {
            if (Terraria.Main.netMode == NetmodeID.MultiplayerClient)
            {
                var p = ModContent.GetInstance<CalamityFandom>().GetPacket();
                p.Write((byte)Packet.Progress);
                p.Write(key);
                p.Write(amount);
                p.Send();
            }
            else
                AddProgress(key, amount, Terraria.Main.myPlayer);
        }

        /// <summary>
        /// Crabulon : « Consume a Yharic Delight while Crabulon is passive right after spawning ». [validé 31/10] Pendant
        /// les 5 premières secondes du combat (sa phase calme de Calamity ne dure que 1 à 2 s, trop court).
        /// </summary>
        public const int CrabulonWindow = 5 * 60;

        /// <summary>Calamity++ (Balance/taming.json) : le Traveling Merchant donne un Yharic Delight à « Yo, I'm Hungary !! ».</summary>
        public static bool HungaryDelight;

        /// <summary>Calamity++ (Balance/taming.json) : l'Eater of Worlds demande 3 aliments différents.</summary>
        public static bool EaterDistinctFoods;

        /// <param name="player">Joueur à prévenir quand le geste ne compte pas (Crabulon), -1 sinon.</param>
        public static void AddProgress(string key, float amount, int player = -1)
        {
            if (Active(key) is not Fight f)
            {
                if (key == "Crabulon")
                    Tell(player, Untamable.Contains(key) ? "Untamable" : Fights.TryGetValue(key, out var hurt) && hurt.Harmed ? "Harmed" : "NoFight", key);
                return;
            }
            uint age = Terraria.Main.GameUpdateCount - f.Started;
            if (key == "Crabulon" && age > CrabulonWindow)
            {
                Tell(player, "TooLate", key, age / 60f, CrabulonWindow / 60);
                return;
            }
            f.Progress += amount;
        }

        /// <summary>Explique à un joueur pourquoi son geste n'a pas compté (Mods.CalamityFandom.Taming.Why.*).</summary>
        private static void Tell(int player, string why, params object[] args)
        {
            if (player < 0 || player >= Terraria.Main.maxPlayers)
                return;
            string key = "Mods.CalamityFandom.Taming.Why." + why;
            if (Terraria.Main.netMode == NetmodeID.Server)
                ChatHelper.SendChatMessageToClient(NetworkText.FromKey(key, args), Color.LightPink, player);
            else
                Terraria.Main.NewText(Language.GetTextValue(key, args), Color.LightPink);
        }

        /// <summary>Le joueur a été touché : remet à zéro les méthodes « sans prendre de dégâts » (Signus, Yharon).</summary>
        public static void ReportHurt()
        {
            if (Terraria.Main.netMode == NetmodeID.MultiplayerClient)
            {
                var p = ModContent.GetInstance<CalamityFandom>().GetPacket();
                p.Write((byte)Packet.Hurt);
                p.Send();
            }
            else
                Hurt();
        }

        private static void Hurt()
        {
            foreach (string key in new[] { "Signus, Envoy of the Devourer", "Jungle Dragon, Yharon" })
                if (Active(key) is Fight f)
                    f.Progress = 0;
        }

        public static void ReportGrave(int player)
        {
            if (Terraria.Main.netMode == NetmodeID.MultiplayerClient)
            {
                var p = ModContent.GetInstance<CalamityFandom>().GetPacket();
                p.Write((byte)Packet.Grave);
                p.Send();
            }
            else
                Grave(player);
        }

        private static void Grave(int player)
        {
            const string key = "Polterghast";
            var e = Get(key);
            if (e == null || Untamable.Contains(key) || FindMain(e) != null)
                return;
            if (++DungeonGraves < GravesGoal)
                return;
            DungeonGraves = 0;
            InstantTame.Add(key);
            NPC.SpawnOnPlayer(player, e.Main);
        }

        /// <summary>Bras de fer gagné contre l'Old Man : Skeletron apparaît et est apprivoisé aussitôt.</summary>
        public static void ReportArmWrestleWon()
        {
            if (Terraria.Main.netMode == NetmodeID.MultiplayerClient)
            {
                var p = ModContent.GetInstance<CalamityFandom>().GetPacket();
                p.Write((byte)Packet.Wrestle);
                p.Send();
            }
            else
                ArmWrestleWon(Terraria.Main.myPlayer);
        }

        private static void ArmWrestleWon(int player)
        {
            var e = Get(OldManWrestle.Key);
            if (e == null || Untamable.Contains(e.Key) || FindMain(e) != null)
                return;
            InstantTame.Add(e.Key);
            // Comme la malédiction de l'Old Man : il disparaît, la tête de Skeletron apparaît à sa place
            NPC.SpawnSkeletron(player);
        }

        /// <summary>
        /// Le joueur commence ou finit une partie d'échecs face à un PNJ, à la table posée en <paramref name="table"/> :
        /// le monde est figé pendant la partie, et tous les joueurs doivent le savoir (Chess/ChessScene.cs).
        /// </summary>
        public static void ReportChess(bool open, Vector2 table = default, int preAge = 0, int opponent = -1, float reach = 0f)
        {
            Chess.ChessScene.Set(Terraria.Main.myPlayer, open, table, preAge, opponent, reach);
            if (Terraria.Main.netMode != NetmodeID.MultiplayerClient)
                return;
            var p = ModContent.GetInstance<CalamityFandom>().GetPacket();
            p.Write((byte)Packet.Chess);
            WriteChess(p, open, table, preAge, opponent, reach);
            p.Send();
        }

        private static void WriteChess(ModPacket p, bool open, Vector2 table, int preAge, int opponent, float reach)
        {
            p.Write(open);
            p.WriteVector2(table);
            p.Write((short)preAge);
            p.Write((short)opponent);
            p.Write(reach);
        }

        public static void ReportSoothe(string key)
        {
            if (Terraria.Main.netMode == NetmodeID.MultiplayerClient)
            {
                var p = ModContent.GetInstance<CalamityFandom>().GetPacket();
                p.Write((byte)Packet.Soothe);
                p.Write(key);
                p.Send();
            }
            else
                Soothe(key);
        }

        private static void Soothe(string key)
        {
            if (SoothedToday || !Untamable.Remove(key))
                return;
            SoothedToday = true;
            Announce("Mods.CalamityFandom.Taming.Soothed", key);
            SyncWorld();
        }

        public static void HandlePacket(BinaryReader reader, int whoAmI)
        {
            var type = (Packet)reader.ReadByte();
            switch (type)
            {
                case Packet.Progress:
                    string key = reader.ReadString();
                    float amount = reader.ReadSingle();
                    if (Terraria.Main.netMode == NetmodeID.Server)
                        AddProgress(key, amount, whoAmI);
                    break;
                case Packet.Hurt:
                    if (Terraria.Main.netMode == NetmodeID.Server)
                        Hurt();
                    break;
                case Packet.Soothe:
                    string soothed = reader.ReadString();
                    if (Terraria.Main.netMode == NetmodeID.Server)
                        Soothe(soothed);
                    break;
                case Packet.Grave:
                    if (Terraria.Main.netMode == NetmodeID.Server)
                        Grave(whoAmI);
                    break;
                case Packet.Wrestle:
                    if (Terraria.Main.netMode == NetmodeID.Server)
                        ArmWrestleWon(whoAmI);
                    break;
                case Packet.Chess:
                    // Client -> serveur : (ouvert, table, avance, adversaire, recul) ; serveur -> autres clients : + joueur
                    bool open = reader.ReadBoolean();
                    Vector2 table = reader.ReadVector2();
                    int preAge = reader.ReadInt16(), opponent = reader.ReadInt16();
                    float reach = reader.ReadSingle();
                    int who = Terraria.Main.netMode == NetmodeID.Server ? whoAmI : reader.ReadByte();
                    Chess.ChessScene.Set(who, open, table, preAge, opponent, reach);
                    if (Terraria.Main.netMode == NetmodeID.Server)
                    {
                        var forward = ModContent.GetInstance<CalamityFandom>().GetPacket();
                        forward.Write((byte)Packet.Chess);
                        WriteChess(forward, open, table, preAge, opponent, reach);
                        forward.Write((byte)who);
                        forward.Send(-1, who);
                    }
                    break;
                case Packet.Tamed:
                    HappyEffects(reader.ReadVector2());
                    break;
            }
        }

        private static void Announce(string textKey, string boss)
        {
            if (Terraria.Main.netMode == NetmodeID.Server)
                ChatHelper.BroadcastChatMessage(NetworkText.FromKey(textKey, boss), Color.HotPink);
            else
                Terraria.Main.NewText(Language.GetTextValue(textKey, boss), Color.HotPink);
        }

        // ---------- combats ----------

        public override void PostUpdateNPCs()
        {
            if (Terraria.Main.netMode == NetmodeID.MultiplayerClient)
                return;
            CultistRitual();
            HiveBees();
            foreach (var e in Entries)
            {
                NPC npc = FindMain(e);
                if (npc == null)
                {
                    if (Fights.Remove(e.Key) && e.Key == SupremeCalamitasKey)
                        SupremeCalamitasQuit.Forget();
                    continue;
                }
                if (!Fights.TryGetValue(e.Key, out var f))
                {
                    Fights[e.Key] = f = new Fight { WasDay = Terraria.Main.dayTime, Started = Terraria.Main.GameUpdateCount };
                    // Empress of Light : invoquée à l'aube (première heure du jour)
                    f.Flag = e.Key == "Empress of Light" && Terraria.Main.dayTime && Terraria.Main.time < 3600;
                    if (InstantTame.Remove(e.Key))
                        f.Progress = e.Goal;
                }
                if (Untamable.Contains(e.Key))
                    continue;
                CheckHarm(e, f);
                if (f.Harmed)
                    continue;
                TamingMethods.Tick(e, f, npc);
                ShowProgress(e, f, npc);
                if (f.Progress >= e.Goal)
                    Tame(e, npc);
            }
        }

        /// <summary>
        /// Queen Bee : 100 abeilles autour d'un joueur dans une ruche. Sans combat, elle apparaît apprivoisée ; pendant un
        /// combat où elle n'a pas été blessée (ses abeilles comptent), elle est apprivoisée aussitôt.
        /// </summary>
        private static void HiveBees()
        {
            var e = Get(TamingBees.Key);
            if (e == null || Terraria.Main.GameUpdateCount % 30 != 0 || Untamable.Contains(e.Key) || InstantTame.Contains(e.Key)
                || QueenBeeRest > 0)
                return;
            NPC queen = FindMain(e);
            if (queen != null && Active(e.Key) == null)
                return;
            foreach (var p in Terraria.Main.ActivePlayers)
            {
                if (TamingBees.CountAround(p) < TamingBees.Goal)
                    continue;
                if (queen != null)
                    Fights[e.Key].Progress = e.Goal;
                else
                {
                    // [validé 31/10] Elle apparaît sur le joueur, au milieu de la ruche et de ses abeilles : son butin y tombe
                    InstantTame.Add(e.Key);
                    int queenIndex = NPC.NewNPC(NPC.GetBossSpawnSource(p.whoAmI), (int)p.Center.X, (int)p.Center.Y, e.Main);
                    if (Terraria.Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.SyncNPC, number: queenIndex);
                }
                return;
            }
        }

        /// <summary>
        /// « Wear the Cultist set, and walk in between the two Lunatic Devotees. This will cause the Lunatic Cultist to
        /// appear, and instantly be tamed » : Lunar Cultist Hood et Robe (armure ou apparence) et passer entre les deux
        /// dévots du rituel, à l'entrée du donjon.
        /// </summary>
        private static void CultistRitual()
        {
            const string key = "Lunatic Cultist";
            var e = Get(key);
            if (e == null || Untamable.Contains(key) || InstantTame.Contains(key) || FindMain(e) != null)
                return;
            NPC left = null, right = null;
            foreach (var n in Terraria.Main.ActiveNPCs)
            {
                if (n.type != NPCID.CultistDevote)
                    continue;
                if (left == null || n.Center.X < left.Center.X)
                    left = n;
                if (right == null || n.Center.X > right.Center.X)
                    right = n;
            }
            if (left == null || left == right)
                return;
            foreach (var p in Terraria.Main.ActivePlayers)
            {
                bool hood = p.armor[0].type == ItemID.BlueLunaticHood || p.armor[10].type == ItemID.BlueLunaticHood;
                bool robe = p.armor[1].type == ItemID.BlueLunaticRobe || p.armor[11].type == ItemID.BlueLunaticRobe;
                if (p.dead || !hood || !robe || p.Center.X < left.Center.X || p.Center.X > right.Center.X
                    || System.Math.Abs(p.Center.Y - left.Center.Y) > 160f)
                    continue;
                InstantTame.Add(key);
                int boss = NPC.NewNPC(NPC.GetBossSpawnSource(p.whoAmI), (int)(left.Center.X + right.Center.X) / 2, (int)left.Center.Y - 120, e.Main);
                if (Terraria.Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.SyncNPC, number: boss);
                return;
            }
        }

        /// <summary>
        /// « The player cannot harm the boss » : toute perte de PV d'un PNJ du combat (coup, debuff) compte. Les étoiles
        /// filantes (Astrum Deus) et les rochers (Ceaseless Void) des méthodes ne touchent pas le boss (TamingGlobalProjectile).
        /// </summary>
        private static void CheckHarm(Entry e, Fight f)
        {
            foreach (var n in Terraria.Main.ActiveNPCs)
            {
                if (!e.Types.Contains(n.type))
                    continue;
                if (f.LastLife.TryGetValue(n.whoAmI, out int last) && n.life < last)
                {
                    f.Harmed = true;
                    if (e.Key == SupremeCalamitasKey)
                        SupremeCalamitasQuit.Forget();
                    return;
                }
                f.LastLife[n.whoAmI] = n.life;
            }
        }

        private static void ShowProgress(Entry e, Fight f, NPC npc)
        {
            int quarter = (int)(Math.Clamp(f.Progress / e.Goal, 0f, 1f) * 4f);
            if (quarter <= f.ShownQuarter || quarter >= 4)
                return;
            f.ShownQuarter = quarter;
            CombatText.NewText(npc.Hitbox, Color.HotPink, $"♥ {quarter * 25}%", true);
        }

        /// <summary>Le boss paraît heureux, disparaît et laisse son butin (compte comme vaincu pour la progression).</summary>
        public static void Tame(Entry e, NPC npc)
        {
            Fights.Remove(e.Key);
            Tamed.Add(e.Key);
            if (e.Key == TamingBees.Key)
            {
                QueenBeeRest = QueenBeeRestTicks;
                SyncWorld();
            }
            Vector2 pos = npc.Center;
            foreach (var n in Terraria.Main.ActiveNPCs)
            {
                if (n.whoAmI == npc.whoAmI || !Belongs(e, n, npc))
                    continue;
                Vanish(n);
            }
            if (e.Key == "Eater of Worlds")
                npc.boss = true;
            TamingNow = true;
            try
            {
                npc.NPCLoot();
            }
            finally
            {
                TamingNow = false;
            }
            Vanish(npc);
            Announce("Mods.CalamityFandom.Taming.Tamed", e.Key);
            if (Terraria.Main.netMode == NetmodeID.Server)
            {
                var p = ModContent.GetInstance<CalamityFandom>().GetPacket();
                p.Write((byte)Packet.Tamed);
                p.WriteVector2(pos);
                p.Send();
            }
            else
                HappyEffects(pos);
        }

        // Tout ce qui appartient au combat : types de l'entrée, segments, membres du Moon Lord, Estranged Sons
        private static bool Belongs(Entry e, NPC n, NPC main) =>
            e.Types.Contains(n.type) || n.realLife == main.whoAmI
            || e.Key == "Moon Lord" && (n.type == NPCID.MoonLordFreeEye || n.type == NPCID.MoonLordLeechBlob)
            // [choix] Les Estranged Sons repartent avec les Perforators apprivoisés
            || e.Key == "The Perforators" && n.ModNPC is Content.NPCs.Perforators.EstrangedSonHead;

        private static void Vanish(NPC n)
        {
            n.life = 0;
            n.active = false;
            if (Terraria.Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.SyncNPC, number: n.whoAmI);
        }

        private static void HappyEffects(Vector2 pos)
        {
            if (Terraria.Main.dedServ)
                return;
            EmoteBubble.NewBubble(EmoteID.EmotionLove, new WorldUIAnchor(pos), 180);
            for (int i = 0; i < 12; i++)
                Gore.NewGore(null, pos + Terraria.Main.rand.NextVector2Circular(60f, 60f), Terraria.Main.rand.NextVector2Circular(3f, 3f) - Vector2.UnitY * 2f, 331); // cœur (Lovestruck)
        }

        /// <summary>Joueur le plus proche du boss (cible des méthodes « mener le boss dans un biome »).</summary>
        public static Player Nearest(NPC npc)
        {
            Player best = null;
            float bestDist = NearDistance;
            foreach (var p in Terraria.Main.ActivePlayers)
            {
                if (p.dead)
                    continue;
                float d = p.Distance(npc.Center);
                if (d < bestDist)
                {
                    best = p;
                    bestDist = d;
                }
            }
            return best;
        }
    }
}
