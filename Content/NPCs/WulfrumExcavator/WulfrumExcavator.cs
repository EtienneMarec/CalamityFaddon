using System;
using System.Collections.Generic;
using System.IO;
using CalamityFandom.Common.Taming;
using CalamityMod;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Dv = CalamityFandom.Content.NPCs.Inventors.InventorBase;

namespace CalamityFandom.Content.NPCs.WulfrumExcavator
{
    /// <summary>
    /// Wiki : Wulfrum Excavator.txt — premier boss pré-Hardmode (avec The Inventors), ver de Wulfrum. Apparaît quand on frappe
    /// une Wulfrum Pylon avec une pioche (<see cref="ExcavatorSummon"/>) : il surgit du sol et la mange. Ver qui tire des
    /// lasers de son corps ; sous des seuils de vie : cercle de 20 blocs autour du joueur (hexagone de lasers), rectangle au
    /// bord de l'écran (salves aux coins), lemniscate au-dessus du joueur (anneaux de lasers). « Supercharged » près d'une
    /// Wulfrum Pylon : un cran de difficulté de plus. Butin par WikiLoot (infobox).
    /// Taming : « Lead the Wulfrum Excavator to 10 veins of naturally spawning ore, which it will dig up automatically ».
    /// </summary>
    [AutoloadBossHead]
    public class WulfrumExcavatorHead : ModNPC
    {
        public const string TamingKey = "Wulfrum Excavator";
        /// <summary>[choix] 8 segments de corps, d'après l'image entière du wiki (422 px de long).</summary>
        public const int BodyCount = 8;
        public const float SuperchargeRadius = 495f;
        public const int SuperchargeTime = 720;

        private ref float Pattern => ref NPC.ai[0];
        private ref float Timer => ref NPC.ai[2];
        private ref float Angle => ref NPC.ai[3];

        private bool spawned;
        private int supercharge;
        private int corner;
        private int berserkTimer;
        private int lastPattern = -1;
        private readonly HashSet<Point> dugVeins = new();

        public bool Supercharged => supercharge > 0;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][ModContent.BuffType<PearlAura>()] = true;
        }

        public override void SetDefaults()
        {
            NPC.boss = true;
            NPC.width = NPC.height = 44;
            NPC.lifeMax = 2000;
            NPC.damage = 20;
            NPC.defense = 0;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.behindTiles = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.Item14;
            NPC.value = Item.buyPrice(gold: 10) / 5;
            NPC.npcSlots = 5f;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToWater = true;
            if (!Main.dedServ)
                Music = MusicID.Boss1;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(Dv.Dv(2000, 3500, 5000, 5000) * balance);
            NPC.damage = Dv.Dv(20, 28, 34, 40);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) =>
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("Mods.CalamityFandom.NPCs.WulfrumExcavatorHead.Bestiary"),
            });

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(supercharge);
            writer.Write(corner);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            supercharge = reader.ReadInt32();
            corner = reader.ReadInt32();
        }

        /// <summary>Cran de difficulté (0 normal, 1 Expert, 2 Revengeance, 3 Death), +1 quand il est « supercharged ».</summary>
        public int Level => Math.Min(3, (CalamityWorld.death ? 3 : CalamityWorld.revenge ? 2 : Main.expertMode ? 1 : 0) + (Supercharged ? 1 : 0));

        private static readonly float[] Phase1 = { 0.6f, 0.7f, 0.75f, 0.75f };
        private static readonly float[] Phase2 = { 0.4f, 0.45f, 0.5f, 0.5f };
        private static readonly float[] Phase3 = { 0.15f, 0.2f, 0.25f, 0.25f };
        private static readonly int[] RingLasers = { 6, 8, 10, 12 };
        /// <summary>Intervalle entre les lasers (multiplicateur) : plus souvent en Expert, encore plus en Revengeance.</summary>
        public static readonly float[] LaserRate = { 1f, 0.8f, 0.65f, 0.55f };

        private float SpeedMult => CalamityWorld.death ? 1.4f : 1f;
        public static float LaserSpeed => CalamityWorld.death ? 9.6f : 8f;
        public static int LaserDamage => Dv.ProjDamage(20, 30, 40, 56);

        private Player Target => Main.player[NPC.target];
        private bool Server => Main.netMode != NetmodeID.MultiplayerClient;

        public override void AI()
        {
            if (!spawned && Server)
            {
                spawned = true;
                int ahead = NPC.whoAmI;
                for (int i = 0; i <= BodyCount; i++)
                {
                    int type = i < BodyCount ? ModContent.NPCType<WulfrumExcavatorBody>() : ModContent.NPCType<WulfrumExcavatorTail>();
                    int seg = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y + 20 * (i + 1), type, NPC.whoAmI, 0f, ahead, i);
                    Main.npc[seg].realLife = NPC.whoAmI;
                    Main.npc[seg].ai[1] = ahead;
                    Main.npc[seg].netUpdate = true;
                    ahead = seg;
                }
            }
            if (NPC.target < 0 || NPC.target == 255 || Target.dead || !Target.active)
                NPC.TargetClosest();
            if (Target.dead || !Target.active || NPC.Distance(Target.Center) > 6000f)
            {
                NPC.velocity.Y = Math.Min(NPC.velocity.Y + 0.3f, 16f);
                NPC.EncourageDespawn(60);
                NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
                return;
            }

            UpdateSupercharge();
            if (Server)
                DigVeins();

            float life = NPC.life / (float)NPC.lifeMax;
            int level = Level;
            int pattern = life < Phase3[level] ? 3 : life < Phase2[level] ? 2 : life < Phase1[level] ? 1 : 0;
            // Revengeance : à 5 %, il passe d'un mouvement à l'autre toutes les 5 s
            if (level >= 2 && life < 0.05f)
            {
                berserkTimer++;
                pattern = 1 + berserkTimer / 300 % 3;
            }
            if (pattern != lastPattern)
            {
                if (lastPattern >= 0)
                    Transition();
                lastPattern = pattern;
                Timer = 0f;
                Pattern = pattern;
                NPC.netUpdate = true;
            }
            Timer++;
            switch (pattern)
            {
                case 0:
                    Burrow();
                    break;
                case 1:
                    Circle(level);
                    break;
                case 2:
                    Rectangle();
                    break;
                default:
                    Lemniscate(level);
                    break;
            }
            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
        }

        private void UpdateSupercharge()
        {
            if (supercharge > 0)
                supercharge--;
            if (Main.GameUpdateCount % 30 != 0 || !Server)
                return;
            int pylon = ModContent.TryFind<ModNPC>("CalamityFandom", "WulfrumPylon", out var p) ? p.Type : -1;
            int amplifier = ModContent.NPCType<WulfrumAmplifier>();
            foreach (NPC n in Main.ActiveNPCs)
                if ((n.type == pylon || n.type == amplifier) && n.Distance(NPC.Center) < SuperchargeRadius)
                {
                    if (supercharge <= 0)
                        SoundEngine.PlaySound(SoundID.Item93, NPC.Center);
                    supercharge = SuperchargeTime;
                    NPC.netUpdate = true;
                    break;
                }
        }

        /// <summary>Changement de mouvement : en Expert, des ennemis Wulfrum arrivent ({{dv|0|2|3|4}}), Wulfrum Pylon comprise.</summary>
        private void Transition()
        {
            SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
            if (!Server)
                return;
            int count = new[] { 0, 2, 3, 4 }[Level];
            var types = new List<int> { ModContent.NPCType<WulfrumDrone>(), ModContent.NPCType<WulfrumGyrator>(), ModContent.NPCType<WulfrumHovercraft>(), ModContent.NPCType<WulfrumRover>() };
            if (ModContent.TryFind<ModNPC>("CalamityFandom", "WulfrumPylon", out var pylon))
                types.Add(pylon.Type);
            for (int i = 0; i < count; i++)
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)(Target.Center.X + Main.rand.Next(-400, 401)), (int)(Target.Center.Y - 300), types[Main.rand.Next(types.Count)]);
        }

        /// <summary>Mouvement de ver ordinaire : fonce dans le sol vers le joueur, retombe hors des blocs.</summary>
        private void Burrow()
        {
            float max = 10f * SpeedMult, accel = 0.16f * SpeedMult;
            if (InTiles() || NPC.Center.Y > Target.Center.Y + 400f)
            {
                Vector2 desired = NPC.SafeDirectionTo(Target.Center) * max;
                NPC.velocity.X = Approach(NPC.velocity.X, desired.X, accel);
                NPC.velocity.Y = Approach(NPC.velocity.Y, desired.Y, accel);
                if (NPC.soundDelay <= 0)
                {
                    NPC.soundDelay = 20;
                    SoundEngine.PlaySound(SoundID.WormDig, NPC.Center);
                }
            }
            else
            {
                NPC.velocity.Y = Math.Min(NPC.velocity.Y + 0.15f, 14f);
                NPC.velocity.X *= 0.995f;
            }
            if (NPC.velocity.Length() > max * 1.5f)
                NPC.velocity = NPC.velocity.SafeNormalize(Vector2.UnitY) * max * 1.5f;
        }

        private void Steer(Vector2 destination, float speed)
        {
            Vector2 want = destination - NPC.Center;
            float dist = want.Length();
            want = want.SafeNormalize(Vector2.Zero) * Math.Min(speed, dist / 4f + 2f);
            NPC.velocity = Vector2.Lerp(NPC.velocity, want, 0.12f);
        }

        private void Laser(Vector2 from, Vector2 velocity)
        {
            if (Server)
                Projectile.NewProjectile(NPC.GetSource_FromAI(), from, velocity, ModContent.ProjectileType<ExcavatorLaser>(), LaserDamage, 0f, Main.myPlayer);
        }

        /// <summary>Cercle de 20 blocs de diamètre autour du joueur, hexagone de lasers vers l'extérieur de temps en temps.</summary>
        private void Circle(int level)
        {
            float speed = 14f * SpeedMult;
            Angle += speed / 160f;
            Steer(Target.Center + Angle.ToRotationVector2() * 160f, speed * 1.4f);
            if (Timer % (int)(120 * LaserRate[level]) == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item33, NPC.Center);
                for (int i = 0; i < 6; i++)
                    Laser(NPC.Center, (MathHelper.TwoPi * i / 6f + Angle).ToRotationVector2() * LaserSpeed);
            }
        }

        /// <summary>Rectangle au bord de l'écran du joueur ; salve de 5 lasers vers lui à chaque coin.</summary>
        private void Rectangle()
        {
            // [choix] Écran de 1800 x 1000 px (le serveur ne connaît pas l'écran du joueur)
            Vector2[] corners = { new(-820f, -420f), new(820f, -420f), new(820f, 420f), new(-820f, 420f) };
            Vector2 dest = Target.Center + corners[corner % 4];
            Steer(dest, 20f * SpeedMult);
            if (NPC.Distance(dest) < 60f)
            {
                corner++;
                SoundEngine.PlaySound(SoundID.Item33, NPC.Center);
                Vector2 dir = NPC.SafeDirectionTo(Target.Center);
                for (int i = -2; i <= 2; i++)
                    Laser(NPC.Center, dir.RotatedBy(i * 0.12f) * LaserSpeed * 1.2f);
                NPC.netUpdate = true;
            }
        }

        /// <summary>Lemniscate (huit couché) au-dessus du joueur ; anneau de {{dv|6|8|10|12}} lasers à chaque tour.</summary>
        private void Lemniscate(int level)
        {
            float before = Angle;
            Angle += 0.025f * SpeedMult;
            float s = (float)Math.Sin(Angle), c = (float)Math.Cos(Angle);
            float denom = 1f + s * s;
            Vector2 offset = new(360f * c / denom, 360f * s * c / denom - 300f);
            Steer(Target.Center + offset, 22f * SpeedMult);
            if ((int)(before / MathHelper.TwoPi) != (int)(Angle / MathHelper.TwoPi))
            {
                SoundEngine.PlaySound(SoundID.Item33, NPC.Center);
                int n = RingLasers[level];
                for (int i = 0; i < n; i++)
                    Laser(NPC.Center, (MathHelper.TwoPi * i / n).ToRotationVector2() * LaserSpeed);
            }
        }

        private static float Approach(float value, float goal, float step) =>
            value < goal ? Math.Min(value + step, goal) : Math.Max(value - step, goal);

        private bool InTiles()
        {
            Point p = NPC.Center.ToTileCoordinates();
            for (int x = p.X - 2; x <= p.X + 2; x++)
                for (int y = p.Y - 2; y <= p.Y + 2; y++)
                    if (WorldGen.InWorld(x, y) && Main.tile[x, y].HasUnactuatedTile && Main.tileSolid[Main.tile[x, y].TileType])
                        return true;
            return false;
        }

        /// <summary>
        /// Taming : quand la tête touche un filon de minerai, il le creuse en entier (les minerais tombent) et le filon compte
        /// (10 pour l'apprivoiser). [choix] Tout minerai (TileID.Sets.Ore) compte : le jeu ne sait pas si un minerai est naturel ;
        /// un filon = les cases du même minerai qui se touchent (200 au plus).
        /// </summary>
        private void DigVeins()
        {
            if (Main.GameUpdateCount % 6 != 0)
                return;
            Point p = NPC.Center.ToTileCoordinates();
            for (int x = p.X - 2; x <= p.X + 2; x++)
                for (int y = p.Y - 2; y <= p.Y + 2; y++)
                {
                    if (!WorldGen.InWorld(x, y, 10))
                        continue;
                    Tile t = Main.tile[x, y];
                    if (!t.HasTile || !TileID.Sets.Ore[t.TileType] || dugVeins.Contains(new Point(x, y)))
                        continue;
                    int dug = DigVein(x, y, t.TileType);
                    if (dug > 0)
                    {
                        SoundEngine.PlaySound(SoundID.Item52, NPC.Center);
                        TamingSystem.Report(TamingKey, 1f);
                    }
                }
        }

        private int DigVein(int x0, int y0, ushort type)
        {
            var queue = new Queue<Point>();
            queue.Enqueue(new Point(x0, y0));
            dugVeins.Add(new Point(x0, y0));
            int dug = 0;
            while (queue.Count > 0 && dug < 200)
            {
                Point q = queue.Dequeue();
                Tile t = Main.tile[q.X, q.Y];
                if (!t.HasTile || t.TileType != type)
                    continue;
                WorldGen.KillTile(q.X, q.Y);
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, q.X, q.Y);
                dug++;
                foreach (Point d in new[] { new Point(1, 0), new Point(-1, 0), new Point(0, 1), new Point(0, -1) })
                {
                    var n = new Point(q.X + d.X, q.Y + d.Y);
                    if (WorldGen.InWorld(n.X, n.Y, 10) && !dugVeins.Contains(n) && Main.tile[n.X, n.Y].HasTile && Main.tile[n.X, n.Y].TileType == type)
                    {
                        dugVeins.Add(n);
                        queue.Enqueue(n);
                    }
                }
            }
            return dug;
        }

        /// <summary>
        /// Résistances (page) : en Expert, les projectiles qui percent (pas les sbires) font leurs dégâts divisés par le
        /// nombre d'ennemis qu'ils peuvent percer (50 % s'ils percent à l'infini) ; les grenades font 80 % de moins.
        /// </summary>
        public static void Resist(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (IsGrenade(projectile))
                modifiers.FinalDamage *= 0.2f;
            if (!Main.expertMode || projectile.minion || projectile.sentry || ProjectileID.Sets.MinionShot[projectile.type])
                return;
            if (projectile.maxPenetrate < 0)
                modifiers.FinalDamage *= 0.5f;
            else if (projectile.maxPenetrate > 1)
                modifiers.FinalDamage /= projectile.maxPenetrate;
        }

        /// <summary>[choix] Grenades : celles de Terraria (lancées et de lance-grenades) et tout projectile moddé dont le nom contient « Grenade ».</summary>
        private static bool IsGrenade(Projectile p) => p.type is ProjectileID.Grenade or ProjectileID.StickyGrenade or ProjectileID.BouncyGrenade
            or ProjectileID.Beenade or ProjectileID.PartyGirlGrenade or ProjectileID.GrenadeI or ProjectileID.GrenadeII or ProjectileID.GrenadeIII
            or ProjectileID.GrenadeIV or ProjectileID.ClusterGrenadeI or ProjectileID.ClusterGrenadeII or ProjectileID.MiniNukeGrenadeI
            or ProjectileID.MiniNukeGrenadeII or ProjectileID.DryGrenade or ProjectileID.WetGrenade or ProjectileID.LavaGrenade or ProjectileID.HoneyGrenade
            || p.ModProjectile?.Name.Contains("Grenade") == true;

        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers) => Resist(projectile, ref modifiers);

        public override void OnKill()
        {
            if (!TamingSystem.TamingNow)
                ExcavatorSystem.Downed = true;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Récompense du Taming (page Taming : Wulfrum Lenses, « Pacifist Drop »)
            var tamed = new LeadingConditionRule(new Inventors.TamedCondition());
            tamed.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Items.Armor.WulfrumLenses>()));
            npcLoot.Add(tamed);
            // « Wulfrum Lure (While supercharged) »
            if (ModContent.TryFind<ModItem>("CalamityMod", "WulfrumLureItem", out var lure))
                npcLoot.Add(ItemDropRule.ByCondition(new SuperchargedCondition(), lure.Type));
        }

        public override void BossLoot(ref string name, ref int potionType) => potionType = ItemID.None;

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (Main.dedServ)
                return;
            for (int i = 0; i < (NPC.life <= 0 ? 30 : 4); i++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GreenTorch, hit.HitDirection, -1f, 100, default, 1.3f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) => ExcavatorDraw.Segment(NPC, spriteBatch, screenPos, drawColor, Supercharged);
    }

    /// <summary>Corps du Wulfrum Excavator : suit le segment de devant (ai[1]), partage la vie de la tête, tire des lasers.</summary>
    public class WulfrumExcavatorBody : ModNPC
    {
        protected virtual float Spacing => 38f;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new() { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][ModContent.BuffType<PearlAura>()] = true;
            NPCID.Sets.RespawnEnemyID[Type] = ModContent.NPCType<WulfrumExcavatorHead>();
        }

        public override void SetDefaults()
        {
            NPC.width = NPC.height = 40;
            NPC.lifeMax = 2000;
            NPC.damage = 10;
            NPC.defense = 4;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.behindTiles = true;
            NPC.netAlways = true;
            NPC.dontCountMe = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.Item14;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToWater = true;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(Dv.Dv(2000, 3500, 5000, 5000) * balance);
            NPC.damage = Damage;
        }

        protected virtual int Damage => Dv.Dv(10, 16, 20, 25);

        public override bool CheckActive() => false;

        public override void AI()
        {
            NPC head = NPC.realLife >= 0 ? Main.npc[NPC.realLife] : null;
            NPC ahead = Main.npc[(int)NPC.ai[1]];
            if (head == null || !head.active || head.type != ModContent.NPCType<WulfrumExcavatorHead>() || !ahead.active)
            {
                NPC.life = 0;
                NPC.HitEffect();
                NPC.active = false;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                return;
            }
            Vector2 toAhead = ahead.Center - NPC.Center;
            NPC.rotation = toAhead.ToRotation() + MathHelper.PiOver2;
            NPC.Center = ahead.Center - toAhead.SafeNormalize(Vector2.UnitY) * Spacing;
            NPC.velocity = Vector2.Zero;
            NPC.target = head.target;
            Lasers(head);
        }

        /// <summary>Lasers du corps vers le joueur, de temps en temps (plus souvent en Expert et au-delà).</summary>
        protected virtual void Lasers(NPC head)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient || head.ModNPC is not WulfrumExcavatorHead h)
                return;
            NPC.localAI[0]++;
            int interval = (int)(300 * WulfrumExcavatorHead.LaserRate[h.Level]) + (int)NPC.ai[2] * 17;
            if (NPC.localAI[0] < interval)
                return;
            NPC.localAI[0] = 0f;
            Player target = Main.player[head.target];
            if (!Main.rand.NextBool(3) || target.dead || NPC.Distance(target.Center) > 900f)
                return;
            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.SafeDirectionTo(target.Center) * WulfrumExcavatorHead.LaserSpeed,
                ModContent.ProjectileType<ExcavatorLaser>(), WulfrumExcavatorHead.LaserDamage, 0f, Main.myPlayer);
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers) => WulfrumExcavatorHead.Resist(projectile, ref modifiers);

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (!Main.dedServ)
                for (int i = 0; i < 3; i++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GreenTorch, hit.HitDirection, -1f, 100, default, 1.2f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            bool super = NPC.realLife >= 0 && Main.npc[NPC.realLife].ModNPC is WulfrumExcavatorHead h && h.Supercharged;
            return ExcavatorDraw.Segment(NPC, spriteBatch, screenPos, drawColor, super);
        }
    }

    /// <summary>Queue du Wulfrum Excavator (défense 6, DR 5 %), sans laser.</summary>
    public class WulfrumExcavatorTail : WulfrumExcavatorBody
    {
        protected override float Spacing => 44f;
        protected override int Damage => Dv.Dv(5, 8, 16, 20);

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.defense = 6;
            NPC.Calamity().DR = 0.05f;
        }

        protected override void Lasers(NPC head)
        {
        }
    }

    internal static class ExcavatorDraw
    {
        /// <summary>Dessine un segment ; [technique] « supercharged » : reflet cyan (le wiki n'a qu'une image du ver entier).</summary>
        public static bool Segment(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor, bool super)
        {
            Texture2D tex = TextureAssets.Npc[npc.type].Value;
            Vector2 pos = npc.Center - screenPos;
            spriteBatch.Draw(tex, pos, null, npc.GetAlpha(drawColor), npc.rotation, tex.Size() / 2f, npc.scale, SpriteEffects.None, 0f);
            if (super)
                spriteBatch.Draw(tex, pos, null, new Color(80, 220, 255, 0) * (0.5f + 0.2f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 8f)), npc.rotation, tex.Size() / 2f, npc.scale, SpriteEffects.None, 0f);
            return false;
        }
    }

    /// <summary>Vrai si le Wulfrum Excavator est « supercharged » quand il meurt.</summary>
    public class SuperchargedCondition : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info) => info.npc?.ModNPC is WulfrumExcavatorHead h && h.Supercharged;
        public bool CanShowItemDropInUI() => true;
        public string GetConditionDescription() => Language.GetTextValue("Mods.CalamityFandom.NPCs.WulfrumExcavatorHead.SuperchargedCondition");
    }

    /// <summary>Vrai une fois le Wulfrum Excavator vaincu (Energy Orbs des ennemis Wulfrum, Desert Medallions des Cnidrions).</summary>
    public class ExcavatorDownedCondition : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info) => ExcavatorSystem.Downed;
        public bool CanShowItemDropInUI() => ExcavatorSystem.Downed;
        public string GetConditionDescription() => Language.GetTextValue("Mods.CalamityFandom.NPCs.WulfrumExcavatorHead.DownedCondition");
    }

    public class ExcavatorSystem : ModSystem
    {
        public static bool Downed;

        public override void ClearWorld() => Downed = false;

        public override void SaveWorldData(TagCompound tag)
        {
            if (Downed)
                tag["downedWulfrumExcavator"] = true;
        }

        public override void LoadWorldData(TagCompound tag) => Downed = tag.ContainsKey("downedWulfrumExcavator");

        public override void NetSend(BinaryWriter writer) => writer.Write(Downed);

        public override void NetReceive(BinaryReader reader) => Downed = reader.ReadBoolean();
    }

    /// <summary>
    /// Après le Wulfrum Excavator (page, « Aftermath ») : les ennemis Wulfrum lâchent des Energy Orbs (1 %), les Cnidrions des
    /// Desert Medallions. [choix] Desert Medallion à 10 % (la page ne donne pas la chance). La pousse des minerais n'est pas
    /// faite (décision en attente).
    /// </summary>
    public class ExcavatorAftermath : GlobalNPC
    {
        private static bool IsWulfrum(NPC npc) => npc.type == ModContent.NPCType<WulfrumDrone>() || npc.type == ModContent.NPCType<WulfrumGyrator>()
            || npc.type == ModContent.NPCType<WulfrumHovercraft>() || npc.type == ModContent.NPCType<WulfrumRover>() || npc.type == ModContent.NPCType<WulfrumAmplifier>()
            || npc.ModNPC is WikiEnemy w && w.Name.StartsWith("Wulfrum");

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (IsWulfrum(npc) && ModContent.TryFind<ModItem>("CalamityFandom", "EnergyOrb", out var orb))
                npcLoot.Add(ItemDropRule.ByCondition(new ExcavatorDownedCondition(), orb.Type, 100));
            if (npc.type == ModContent.NPCType<Cnidrion>())
                npcLoot.Add(ItemDropRule.ByCondition(new ExcavatorDownedCondition(), ModContent.ItemType<CalamityMod.Items.SummonItems.DesertMedallion>(), 10));
        }
    }

    /// <summary>
    /// Invocation (page, « Spawn ») : frapper une Wulfrum Pylon avec une pioche ; l'Excavator surgit du sol sous elle et la
    /// mange. [choix] La condition de l'Ogscule est ignorée : la page du boss la demande dans l'inventaire, celle de
    /// l'Ogscule dit le contraire (décisions).
    /// </summary>
    public class ExcavatorSummon : GlobalNPC
    {
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) =>
            ModContent.TryFind<ModNPC>("CalamityFandom", "WulfrumPylon", out var pylon) && entity.type == pylon.Type;

        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            if (item.pick <= 0 || player.whoAmI != Main.myPlayer || NPC.AnyNPCs(ModContent.NPCType<WulfrumExcavatorHead>()))
                return;
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                var p = ModContent.GetInstance<CalamityFandom>().GetPacket();
                p.Write((byte)CalamityFandom.ExtraPacket.ExcavatorSummon);
                p.Write((short)npc.whoAmI);
                p.Write((byte)player.whoAmI);
                p.Send();
                return;
            }
            Summon(npc, player);
        }

        public static void Summon(NPC pylon, Player player)
        {
            if (!pylon.active || NPC.AnyNPCs(ModContent.NPCType<WulfrumExcavatorHead>()))
                return;
            int head = NPC.NewNPC(new EntitySource_BossSpawn(player), (int)pylon.Center.X, (int)pylon.Center.Y + 600, ModContent.NPCType<WulfrumExcavatorHead>(), 0, 0f, 0f, 0f, 0f, player.whoAmI);
            if (Main.npc.IndexInRange(head))
                Main.npc[head].velocity = new Vector2(0f, -16f);
            SoundEngine.PlaySound(SoundID.Roar, pylon.Center);
            // Il mange la Wulfrum Pylon
            pylon.life = 0;
            pylon.HitEffect();
            pylon.active = false;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.SyncNPC, number: pylon.whoAmI);
            string text = Language.GetTextValue("Announcement.HasAwoken", Lang.GetNPCNameValue(ModContent.NPCType<WulfrumExcavatorHead>()));
            if (Main.netMode == NetmodeID.SinglePlayer)
                Main.NewText(text, 175, 75, 255);
            else
                Terraria.Chat.ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Announcement.HasAwoken", Lang.GetNPCNameValue(ModContent.NPCType<WulfrumExcavatorHead>())), new Color(175, 75, 255));
        }

        /// <summary>Paquet <see cref="CalamityFandom.ExtraPacket.ExcavatorSummon"/> reçu par le serveur.</summary>
        public static void Receive(BinaryReader reader)
        {
            int npc = reader.ReadInt16();
            int player = reader.ReadByte();
            if (Main.netMode == NetmodeID.Server && Main.npc.IndexInRange(npc))
                Summon(Main.npc[npc], Main.player[player]);
        }
    }

    /// <summary>Laser du Wulfrum Excavator (vert).</summary>
    public class ExcavatorLaser : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.GreenLaser;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 6;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Lighting.AddLight(Projectile.Center, 0.2f, 0.6f, 0.2f);
        }

        public override Color? GetAlpha(Color lightColor) => new Color(140, 255, 140, 0);
    }
}

namespace CalamityFandom.Content.Items.Armor
{
    /// <summary>
    /// Wiki : Wulfrum Lenses.txt — casque (1 défense) : un petit faisceau de lumière bleue devant le joueur et les trésors
    /// montrés dans une grande zone devant lui. Récompense du Taming du Wulfrum Excavator.
    /// [choix] Faisceau = lumière bleue devant le joueur ; trésors = effet de la Spelunker Potion (comme le Wulfrum Mining
    /// Helmet). [technique] Rendu porté provisoire (l'icône réduite sur les yeux) : sprite demandé (docs/SPRITES.md).
    /// </summary>
    [AutoloadEquip(EquipType.Head)]
    public class WulfrumLenses : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.defense = 1;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 36);
        }

        public override void UpdateEquip(Player player)
        {
            player.findTreasure = true;
            for (int i = 1; i <= 6; i++)
                Lighting.AddLight(player.Center + new Vector2(player.direction * 24f * i, -6f), 0.15f, 0.3f, 0.6f);
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Summon
{
    /// <summary>
    /// Wiki : Wulfrum Fusion Array.txt — rafales rapides de projectiles à portée moyenne qui traversent plusieurs ennemis ;
    /// l'ennemi touché devient la cible des sbires et reçoit 4 dégâts de « tag » (comme un fouet) ; 6 de pénétration d'armure.
    /// Butin des Inventors et du Wulfrum Excavator. [choix] Rafale de 3 tirs, portée de 25 blocs, 3 ennemis traversés.
    /// </summary>
    public class WulfrumFusionArray : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 6;
            Item.DamageType = DamageClass.SummonMeleeSpeed;
            Item.useTime = 3;
            Item.useAnimation = 10;
            Item.reuseDelay = 6;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item12;
            Item.width = 38;
            Item.height = 18;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 20);
            Item.shoot = ModContent.ProjectileType<FusionBolt>();
            Item.shootSpeed = 18f;
            Item.ArmorPenetration = 6;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-4f, 0f);
    }

    /// <summary>Projectile du Wulfrum Fusion Array : marque l'ennemi (cible des sbires, 4 dégâts de plus pour eux).</summary>
    public class FusionBolt : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.GreenLaser;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.SummonMeleeSpeed;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 23;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ArmorPenetration = 6;
        }

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        public override Color? GetAlpha(Color lightColor) => new Color(130, 220, 120, 0);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
            target.GetGlobalNPC<FusionTag>().Tagged = 240;
        }
    }

    /// <summary>« 4 summon tag damage » : les sbires et sentinelles font 4 dégâts de plus à l'ennemi marqué (4 s).</summary>
    public class FusionTag : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public int Tagged;

        public override void ResetEffects(NPC npc)
        {
            if (Tagged > 0)
                Tagged--;
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (Tagged > 0 && (projectile.minion || projectile.sentry || ProjectileID.Sets.MinionShot[projectile.type] || ProjectileID.Sets.SentryShot[projectile.type]))
                modifiers.FlatBonusDamage += 4;
        }
    }
}
