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
            public int ShownQuarter;
        }

        public static readonly List<Entry> Entries = new();
        private static readonly Dictionary<string, Entry> ByKey = new();
        private static readonly Dictionary<int, Entry> ByType = new();
        public static readonly Dictionary<string, Fight> Fights = new();
        /// <summary>Boss tués (inapprivoisables jusqu'à l'apaisement), sauvegardés avec le monde.</summary>
        public static readonly HashSet<string> Untamable = new();
        public static bool SoothedToday;
        /// <summary>Tombes posées dans le donjon (Polterghast apparaît apprivoisé à 100).</summary>
        public static int DungeonGraves;
        private static readonly HashSet<string> InstantTame = new();
        /// <summary>Vrai pendant le butin d'un boss apprivoisé (ne pas le compter comme tué).</summary>
        public static bool TamingNow;

        public const int GravesGoal = 100;
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
            // Pré-Hardmode. TODO : The Inventors, Wulfrum Excavator (boss inventés) ; Brain of Cthulhu (Holographic
            // Chessboard), Queen Bee (« 100 bees », sens flou), Skeletron (bras de fer avec l'Old Man), The Slime God
            // (absorption des Blight Slimes) : contenu à créer
            Add("King Slime", 300 * 60, 0, NPCID.KingSlime);
            Add("Desert Scourge", 180, 0, ModContent.NPCType<DesertScourgeHead>(), ModContent.NPCType<DesertScourgeBody>(), ModContent.NPCType<DesertScourgeTail>());
            Add("Eye of Cthulhu", 30, 0, NPCID.EyeofCthulhu);
            Add("Crabulon", 1, 0, ModContent.NPCType<Crabulon>());
            Add("Eater of Worlds", 3, 0, NPCID.EaterofWorldsHead, NPCID.EaterofWorldsBody, NPCID.EaterofWorldsTail);
            Add("The Hive Mind", 100, 0, ModContent.NPCType<HiveMind>());
            Add("The Perforators", 50, 0, ModContent.NPCType<PerforatorHive>(),
                ModContent.NPCType<PerforatorHeadSmall>(), ModContent.NPCType<PerforatorBodySmall>(), ModContent.NPCType<PerforatorTailSmall>(),
                ModContent.NPCType<PerforatorHeadMedium>(), ModContent.NPCType<PerforatorBodyMedium>(), ModContent.NPCType<PerforatorTailMedium>(),
                ModContent.NPCType<PerforatorHeadLarge>(), ModContent.NPCType<PerforatorBodyLarge>(), ModContent.NPCType<PerforatorTailLarge>());
            Add("Wall of Flesh", 1, 0, NPCID.WallofFlesh, NPCID.WallofFleshEye);
            // Hardmode. TODO : Cumulomenace, Pyrogen, The Derellect, Polyphemalus (inventés) ; Brimstone Elemental (PNJ qui
            // emménagent), Leviathan and Anahita (poissons rouges), Astrum Aureus (Draedon's Remote absent), Golem (ballon
            // de plage)
            Add("Queen Slime", 100, 1, NPCID.QueenSlimeBoss);
            Add("Cryogen", 180, 1, ModContent.NPCType<Cryogen>());
            Add("The Twins", 60 * 60, 1, NPCID.Retinazer, NPCID.Spazmatism);
            Add("The Destroyer", 5, 1, NPCID.TheDestroyer, NPCID.TheDestroyerBody, NPCID.TheDestroyerTail);
            Add("Skeletron Prime", 60 * 60, 1, NPCID.SkeletronPrime, NPCID.PrimeCannon, NPCID.PrimeSaw, NPCID.PrimeVice, NPCID.PrimeLaser);
            Add("Aquatic Scourge", 180, 1, ModContent.NPCType<AquaticScourgeHead>(), ModContent.NPCType<AquaticScourgeBody>(),
                ModContent.NPCType<AquaticScourgeBodyAlt>(), ModContent.NPCType<AquaticScourgeTail>());
            Add("Plantera", 180 * 60, 1, NPCID.Plantera);
            Add("Calamitas", 60, 1, ModContent.NPCType<CalamitasClone>());
            Add("The Plaguebringer Goliath", 200, 1, ModContent.NPCType<PlaguebringerGoliath>());
            Add("Duke Fishron", 1, 1, NPCID.DukeFishron);
            Add("Empress of Light", 1, 1, NPCID.HallowBoss);
            Add("Ravager", 1, 1, ModContent.NPCType<RavagerBody>(), ModContent.NPCType<RavagerClawLeft>(), ModContent.NPCType<RavagerClawRight>(),
                ModContent.NPCType<RavagerLegLeft>(), ModContent.NPCType<RavagerLegRight>(), ModContent.NPCType<RavagerHead>());
            Add("Astrum Deus", 1, 1, ModContent.NPCType<AstrumDeusHead>(), ModContent.NPCType<AstrumDeusBody>(), ModContent.NPCType<AstrumDeusTail>());
            Add("The Dragonfolly", 1, 1, ModContent.NPCType<Dragonfolly>());
            Add("Lunatic Cultist", 1, 1, NPCID.CultistBoss);
            Add("Moon Lord", 1, 1, NPCID.MoonLordCore, NPCID.MoonLordHead, NPCID.MoonLordHand);
            // Après le Moon Lord. TODO : Yharim Worm, The Wall of Bronze, The Fabricational Quartet, XP-00 Hypnos (inventés) ;
            // Supreme Calamitas (« alt-f4 »)
            Add("Profaned Guardians", 1, 2, ModContent.NPCType<ProfanedGuardianCommander>(), ModContent.NPCType<ProfanedGuardianDefender>(),
                ModContent.NPCType<ProfanedGuardianHealer>());
            Add("Providence, the Profaned Goddess", 4, 2, ModContent.NPCType<Providence>());
            Add("Ceaseless Void", 50, 2, ModContent.NPCType<CeaselessVoid>());
            Add("Storm Weaver", 10, 2, ModContent.NPCType<StormWeaverHead>(), ModContent.NPCType<StormWeaverBody>(), ModContent.NPCType<StormWeaverTail>());
            Add("Signus, Envoy of the Devourer", 8 * 60 * 60, 2, ModContent.NPCType<Signus>());
            Add("Polterghast", 1, 2, ModContent.NPCType<Polterghast>());
            Add("The Devourer of Gods", 150, 2, ModContent.NPCType<DevourerofGodsHead>(), ModContent.NPCType<DevourerofGodsBody>(), ModContent.NPCType<DevourerofGodsTail>());
            Add("Jungle Dragon, Yharon", 5 * 60 * 60, 2, ModContent.NPCType<Yharon>());
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
            foreach (var n in Terraria.Main.ActiveNPCs)
                if (n.type == e.Main)
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
            InstantTame.Clear();
            SoothedToday = false;
            DungeonGraves = 0;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag["tamingUntamable"] = Untamable.ToList();
            tag["tamingSoothedToday"] = SoothedToday;
            tag["tamingGraves"] = DungeonGraves;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            Untamable.Clear();
            foreach (string key in tag.GetList<string>("tamingUntamable"))
                Untamable.Add(key);
            SoothedToday = tag.GetBool("tamingSoothedToday");
            DungeonGraves = tag.GetInt("tamingGraves");
        }

        // La liste sert à la boutique du Zoologist, côté client
        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(SoothedToday);
            writer.Write(Untamable.Count);
            foreach (string key in Untamable)
                writer.Write(key);
        }

        public override void NetReceive(BinaryReader reader)
        {
            SoothedToday = reader.ReadBoolean();
            Untamable.Clear();
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
                Untamable.Add(reader.ReadString());
        }

        private static bool wasDay;

        public override void PostUpdateWorld()
        {
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

        private enum Packet : byte { Progress, Hurt, Soothe, Tamed, Grave }

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
                AddProgress(key, amount);
        }

        public static void AddProgress(string key, float amount)
        {
            if (Active(key) is Fight f)
                f.Progress += amount;
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
                        AddProgress(key, amount);
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
            foreach (var e in Entries)
            {
                NPC npc = FindMain(e);
                if (npc == null)
                {
                    Fights.Remove(e.Key);
                    continue;
                }
                if (!Fights.TryGetValue(e.Key, out var f))
                {
                    Fights[e.Key] = f = new Fight { WasDay = Terraria.Main.dayTime };
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
            || e.Key == "The Perforators" && n.ModNPC is Content.NPCs.Perforators.EstrangedSonSegment;

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
