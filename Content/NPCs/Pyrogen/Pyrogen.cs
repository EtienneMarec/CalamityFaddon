using System;
using System.IO;
using CalamityFandom.Common.Taming;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.SummonItems;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Dv = CalamityFandom.Content.NPCs.Inventors.InventorBase;

namespace CalamityFandom.Content.NPCs.Pyrogen
{
    /// <summary>
    /// Wiki : Pyrogen.txt — boss de fin de Hardmode, jumeau de feu de Cryogen, dans l'Underworld ou le Brimstone Crag. Apparaît
    /// quand une Cryo Key est jetée dans la lave (<see cref="PyrogenCryoKey"/>). Bouclier d'obsidienne qui détruit les projectiles
    /// (pas les coups de mêlée) ; quatre phases qui tournent avec le temps, plus vite quand il perd sa vie ; Hellstorm une fois
    /// sous 44 % (55 % en Revengeance). Furieux hors de l'Underworld. Butin par WikiLoot (infobox).
    /// Taming : « Lead Pyrogen into the Surface Snow biome » (TamingMethods).
    /// </summary>
    [AutoloadBossHead]
    public class Pyrogen : ModNPC
    {
        // ai[0] : phase (1 à 4, 5 = Hellstorm) ; ai[1] : minuteur ; ai[2] : sous-état ; ai[3] : compteur
        private ref float Phase => ref NPC.ai[0];
        private ref float Timer => ref NPC.ai[1];
        private ref float SubState => ref NPC.ai[2];
        private ref float Counter => ref NPC.ai[3];

        /// <summary>Images restantes sans bouclier (le boss est séparé de son bouclier).</summary>
        private int shieldOff;
        private bool hellstormDone;
        private float orbitAngle;
        private Vector2 teleportOffset;
        private int teleportTimer;

        public const float ShieldRadius = 110f;
        public const float ExtendedShieldRadius = 180f;
        public const int ShieldPieces = 8;

        public bool Shielded => shieldOff <= 0 && Phase != 3f && Phase != 5f;
        public bool Extended => Phase == 4f && shieldOff <= 0;

        private Player Target => Main.player[NPC.target];

        public override void SetStaticDefaults()
        {
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.TrailCacheLength[Type] = 6;
            NPCID.Sets.TrailingMode[Type] = 1;
            foreach (int buff in new[] { BuffID.Confused, BuffID.Frostburn, BuffID.Frostburn2, BuffID.OnFire, BuffID.OnFire3, ModContent.BuffType<PearlAura>(),
                ModContent.BuffType<Legacy.Buffs.StatDebuffs.GlacialState>(), ModContent.BuffType<Buffs.ExoFreeze>(), ModContent.BuffType<Dragonfire>(), ModContent.BuffType<BrimstoneFlames>() })
                NPCID.Sets.SpecificDebuffImmunity[Type][buff] = true;
        }

        public override void SetDefaults()
        {
            NPC.boss = true;
            NPC.width = NPC.height = 120;
            NPC.lifeMax = 44000;
            NPC.damage = 70;
            NPC.defense = 24;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.lavaImmune = true;
            NPC.HitSound = SoundID.NPCHit5;
            NPC.DeathSound = SoundID.NPCDeath15;
            NPC.value = Item.buyPrice(gold: 20) / 5;
            NPC.npcSlots = 10f;
            NPC.Calamity().DR = 0.2f;
            NPC.Calamity().VulnerableToWater = true;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToCold = false;
            if (!Main.dedServ)
                Music = MusicID.PumpkinMoon;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(Dv.Dv(44000, 52000, 66600, 66600) * balance);
            NPC.damage = Dv.Dv(70, 130, 145, 172);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) =>
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld,
                new FlavorTextBestiaryInfoElement("Mods.CalamityFandom.NPCs.Pyrogen.Bestiary"),
            });

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(shieldOff);
            writer.Write(hellstormDone);
            writer.Write(orbitAngle);
            writer.WriteVector2(teleportOffset);
            writer.Write(teleportTimer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            shieldOff = reader.ReadInt32();
            hellstormDone = reader.ReadBoolean();
            orbitAngle = reader.ReadSingle();
            teleportOffset = reader.ReadVector2();
            teleportTimer = reader.ReadInt32();
        }

        /// <summary>Vitesse des attaques : plus vite quand la vie baisse, en Revengeance, et beaucoup plus hors de l'Underworld.</summary>
        private float Speed
        {
            get
            {
                float s = 1f + 0.5f * (1f - NPC.life / (float)NPC.lifeMax);
                if (Main.expertMode)
                    s *= 1.1f;
                if (CalamityWorld.revenge)
                    s *= 1.15f;
                if (Enraged)
                    s *= 2f;
                return s;
            }
        }

        private bool Enraged => !Target.ZoneUnderworldHeight && !Target.Calamity().ZoneCalamity;

        private float HellstormThreshold => CalamityWorld.revenge ? 0.55f : 0.44f;

        public override void AI()
        {
            if (Phase == 0f)
            {
                Phase = 1f;
                NPC.netUpdate = true;
            }
            NPC.TargetClosest(false);
            if (!Target.active || Target.dead || NPC.Distance(Target.Center) > 6000f)
            {
                // Plus de cible : s'enfonce dans la lave et disparaît
                NPC.velocity.Y += 0.3f;
                NPC.velocity.X *= 0.95f;
                if (NPC.timeLeft > 120)
                    NPC.timeLeft = 120;
                return;
            }
            NPC.timeLeft = 1800;
            Lighting.AddLight(NPC.Center, 1f, 0.5f, 0.1f);
            if (shieldOff > 0)
                shieldOff--;
            if (Shielded)
                DestroyProjectiles(ShieldRadius);

            // Hellstorm, une seule fois
            if (!hellstormDone && Phase != 5f && NPC.life < NPC.lifeMax * HellstormThreshold)
            {
                StartHellstorm();
                return;
            }

            float speed = Speed;
            Timer += 1f;
            switch ((int)Phase)
            {
                case 1:
                    PhaseOne(speed);
                    break;
                case 2:
                    PhaseTwo(speed);
                    break;
                case 3:
                    PhaseThree(speed);
                    break;
                case 4:
                    PhaseFour(speed);
                    break;
                case 5:
                    Hellstorm(speed);
                    return;
            }
            // Death Mode : se téléporte quelle que soit la phase (la phase 4 le fait déjà)
            if (CalamityWorld.death && Phase != 4f)
                Teleports();
        }

        private void NextPhase()
        {
            Phase = Phase >= 4f ? 1f : Phase + 1f;
            Timer = SubState = Counter = 0f;
            NPC.netUpdate = true;
        }

        private void Fly(Vector2 destination, float speed, float inertia)
        {
            Vector2 want = (destination - NPC.Center).SafeNormalize(Vector2.Zero) * Math.Min(speed, NPC.Distance(destination) / 8f + 1f);
            NPC.velocity = (NPC.velocity * (inertia - 1f) + want) / inertia;
        }

        private bool Server => Main.netMode != NetmodeID.MultiplayerClient;

        private void Shoot(Vector2 position, Vector2 velocity, int type, int damage, float ai0 = 0f, float ai1 = 0f)
        {
            if (Server)
                Projectile.NewProjectile(NPC.GetSource_FromAI(), position, velocity, type, damage, 0f, Main.myPlayer, ai0, ai1);
        }

        private static int ShardDamage => Dv.ProjDamage(42, 54, 78, 112);
        private static int MagmaDamage => Dv.ProjDamage(46, 80, 96, 108);

        /// <summary>Phase 1 : tourne autour du joueur, rafale serrée de 13 éclats toutes les 4 s ; après 4 tours, ruée vers le bord de l'écran puis ruée prédictive (bouclier séparé 4 s).</summary>
        private void PhaseOne(float speed)
        {
            if (SubState == 0f)
            {
                float step = MathHelper.TwoPi / 180f * speed;
                orbitAngle += step;
                Counter += step;
                Fly(Target.Center + orbitAngle.ToRotationVector2() * 450f, 14f * speed, 12f);
                if (Timer % (int)(240 / speed) == 0f)
                {
                    SoundEngine.PlaySound(SoundID.Item28, NPC.Center);
                    Vector2 dir = NPC.SafeDirectionTo(Target.Center);
                    for (int i = 0; i < 13; i++)
                        Shoot(NPC.Center, dir.RotatedBy(MathHelper.Lerp(-0.3f, 0.3f, i / 12f)) * 9f * Math.Min(speed, 1.6f), ModContent.ProjectileType<PyrogenShard>(), ShardDamage, 1f);
                }
                if (Counter >= MathHelper.TwoPi * 4f)
                {
                    SubState = 1f;
                    Timer = 0f;
                    NPC.netUpdate = true;
                }
                return;
            }
            if (SubState == 1f)
            {
                // Vers le bord de l'écran, à l'opposé du joueur
                float side = NPC.Center.X < Target.Center.X ? -1f : 1f;
                Fly(Target.Center + new Vector2(side * 900f, -100f), 26f, 6f);
                if (NPC.Distance(Target.Center) > 850f || Timer > 120f)
                {
                    SubState = 2f;
                    Timer = 0f;
                    shieldOff = 240;
                    Vector2 aim = Target.Center + Target.velocity * 30f;
                    NPC.velocity = (aim - NPC.Center).SafeNormalize(Vector2.UnitX) * 26f * Math.Min(speed, 1.5f);
                    SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                    NPC.netUpdate = true;
                }
                return;
            }
            NPC.velocity *= Timer > 50f ? 0.95f : 1f;
            if (Timer > 120f)
                NextPhase();
        }

        /// <summary>
        /// Phase 2 : au-dessus du joueur, 2 salves d'éclats lancés vers le haut qui retombent, se plantent puis explosent ;
        /// sous le joueur, ruées rapides ; arrêt : bouclier qui glisse, anneaux de Magma Blasts.
        /// </summary>
        private void PhaseTwo(float speed)
        {
            switch ((int)SubState)
            {
                case 0:
                    Fly(Target.Center + new Vector2(0f, -320f), 16f * speed, 10f);
                    if (Timer % (int)(100 / speed) == 0f)
                    {
                        SoundEngine.PlaySound(SoundID.Item28, NPC.Center);
                        for (int i = 0; i < 10; i++)
                            Shoot(NPC.Center, new Vector2(Main.rand.NextFloat(-7f, 7f), Main.rand.NextFloat(-12f, -6f)), ModContent.ProjectileType<PyrogenFallingShard>(), ShardDamage);
                        if (++Counter >= 2f)
                        {
                            SubState = 1f;
                            Timer = Counter = 0f;
                            NPC.netUpdate = true;
                        }
                    }
                    break;
                case 1:
                    // Se place sous le joueur
                    Fly(Target.Center + new Vector2(0f, 360f), 22f, 6f);
                    if (Timer > 40f)
                    {
                        SubState = 2f;
                        Timer = 0f;
                        NPC.netUpdate = true;
                    }
                    break;
                case 2:
                    // Ruées : 3 (4 en Revengeance)
                    if (Timer == 1f)
                    {
                        NPC.velocity = NPC.SafeDirectionTo(Target.Center + Target.velocity * 10f) * 22f * Math.Min(speed, 1.5f);
                        SoundEngine.PlaySound(SoundID.Item74, NPC.Center);
                    }
                    else
                        NPC.velocity *= 0.97f;
                    if (Timer >= 35f / Math.Min(speed, 1.5f))
                    {
                        Timer = 0f;
                        if (++Counter >= (CalamityWorld.revenge ? 4f : 3f))
                        {
                            SubState = 3f;
                            Counter = 0f;
                            shieldOff = 180;
                        }
                        NPC.netUpdate = true;
                    }
                    break;
                default:
                    NPC.velocity *= 0.9f;
                    if (Timer % (int)(45 / speed) == 0f)
                    {
                        SoundEngine.PlaySound(SoundID.Item20, NPC.Center);
                        float start = Main.rand.NextFloat(MathHelper.TwoPi);
                        for (int i = 0; i < 12; i++)
                            Shoot(NPC.Center, (start + MathHelper.TwoPi * i / 12f).ToRotationVector2() * 6f, ModContent.ProjectileType<MagmaBlast>(), MagmaDamage);
                    }
                    if (Timer > 180f)
                        NextPhase();
                    break;
            }
        }

        /// <summary>Phase 3 : poursuit lentement à distance ; les morceaux du bouclier montent sous le joueur, avec des trous réguliers.</summary>
        private void PhaseThree(float speed)
        {
            float distance = NPC.Distance(Target.Center);
            Vector2 away = NPC.SafeDirectionTo(Target.Center);
            if (distance > 420f)
                Fly(Target.Center - away * 360f, 6f * speed, 20f);
            else if (distance < 300f)
                NPC.velocity = Vector2.Lerp(NPC.velocity, -away * 6f, 0.05f);
            else
                NPC.velocity *= 0.97f;
            if (Timer % (int)(80 / speed) == 1f)
            {
                SoundEngine.PlaySound(SoundID.Item69, Target.Center);
                float offset = Main.rand.NextFloat(170f);
                for (int i = -4; i <= 4; i++)
                    Shoot(Target.Center + new Vector2(i * 170f + offset, 700f), new Vector2(0f, -9f * Math.Min(speed, 1.6f)), ModContent.ProjectileType<PyrogenShieldPiece>(), ShardDamage);
            }
            if (Timer > 480f)
                NextPhase();
        }

        /// <summary>Phase 4 : poursuit vite ; bouclier écarté (les morceaux blessent, on tire entre eux) ; Magma Blasts deux fois plus souvent ; téléportations.</summary>
        private void PhaseFour(float speed)
        {
            Fly(Target.Center, 11f * speed, 18f);
            if (Timer == 1f)
                for (int i = 0; i < ShieldPieces; i++)
                    Shoot(NPC.Center, Vector2.Zero, ModContent.ProjectileType<PyrogenOrbitPiece>(), ShardDamage, NPC.whoAmI, MathHelper.TwoPi * i / ShieldPieces);
            if (Timer % (int)(30 / speed) == 0f)
                Shoot(NPC.Center, NPC.SafeDirectionTo(Target.Center) * 9f, ModContent.ProjectileType<MagmaBlast>(), MagmaDamage);
            Teleports();
            if (Timer > 600f)
                NextPhase();
        }

        /// <summary>Toutes les 5 s, téléportation près du joueur, devant sa trajectoire, annoncée 2 s avant par de la poussière orange (qui suit le joueur).</summary>
        private void Teleports()
        {
            teleportTimer++;
            if (teleportTimer == 180 && Server)
            {
                teleportOffset = Target.velocity * 40f + Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(300f, 450f);
                NPC.netUpdate = true;
            }
            if (teleportTimer > 180 && !Main.dedServ)
                for (int i = 0; i < 3; i++)
                    Dust.NewDustPerfect(Target.Center + teleportOffset + Main.rand.NextVector2Circular(60f, 60f), DustID.Torch, Vector2.Zero, 100, default, 2f).noGravity = true;
            if (teleportTimer >= 300)
            {
                teleportTimer = 0;
                NPC.Center = Target.Center + teleportOffset;
                NPC.velocity = Vector2.Zero;
                SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                NPC.netUpdate = true;
            }
        }

        private void StartHellstorm()
        {
            hellstormDone = true;
            Phase = 5f;
            Timer = SubState = Counter = 0f;
            SoundEngine.PlaySound(SoundID.Item14, NPC.Center);
            // Le bouclier explose
            for (int i = 0; i < 16; i++)
                Shoot(NPC.Center, (MathHelper.TwoPi * i / 16f).ToRotationVector2() * 10f, ModContent.ProjectileType<PyrogenShard>(), ShardDamage);
            PyrogenSay("Hellstorm", new Color(255, 120, 40));
            NPC.netUpdate = true;
        }

        /// <summary>Phase 5 : Hellstorm (8 s) — tourne sur lui-même, attire le joueur et des anneaux d'éclats ; dégâts reçus divisés par 2 ; finit par un amas d'obsidienne qui éclate.</summary>
        private void Hellstorm(float speed)
        {
            NPC.velocity *= 0.9f;
            NPC.rotation += 0.4f;
            Player local = Main.LocalPlayer;
            if (local.active && !local.dead && local.Distance(NPC.Center) < 1600f)
                local.velocity += local.SafeDirectionTo(NPC.Center) * 0.3f;
            if (!Main.dedServ)
                for (int i = 0; i < 4; i++)
                {
                    Vector2 pos = NPC.Center + Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(100f, 500f);
                    Dust.NewDustPerfect(pos, DustID.Torch, (NPC.Center - pos) * 0.05f, 100, default, 2f).noGravity = true;
                }
            if (Timer % (int)(40 / Math.Min(speed, 1.5f)) == 0f)
            {
                float start = Main.rand.NextFloat(MathHelper.TwoPi);
                for (int i = 0; i < 16; i++)
                {
                    Vector2 pos = NPC.Center + (start + MathHelper.TwoPi * i / 16f).ToRotationVector2() * 750f;
                    Shoot(pos, (NPC.Center - pos).SafeNormalize(Vector2.Zero) * 6f, ModContent.ProjectileType<PyrogenShard>(), ShardDamage);
                }
            }
            if (Timer < 480f)
                return;
            NPC.rotation = 0f;
            SoundEngine.PlaySound(SoundID.Item62, NPC.Center);
            Shoot(NPC.Center, NPC.SafeDirectionTo(Target.Center) * 10f, ModContent.ProjectileType<ObsidianCluster>(), ShardDamage * 2);
            Phase = 1f;
            Timer = SubState = Counter = 0f;
            NPC.netUpdate = true;
        }

        public bool InHellstorm => Phase == 5f;

        /// <summary>Le bouclier détruit les projectiles des joueurs qui l'atteignent (pas les sbires, ni les coups de mêlée).</summary>
        private void DestroyProjectiles(float radius)
        {
            foreach (Projectile p in Main.ActiveProjectiles)
                if (p.friendly && !p.hostile && p.damage > 0 && !p.minion && !p.sentry && !Main.projPet[p.type] && p.owner == Main.myPlayer
                    && Main.player[p.owner].heldProj != p.whoAmI && p.aiStyle != ProjAIStyleID.Spear && p.Distance(NPC.Center) < radius)
                {
                    for (int i = 0; i < 4; i++)
                        Dust.NewDustPerfect(p.Center, DustID.Obsidian, Main.rand.NextVector2Circular(2f, 2f), 100, default, 1.2f);
                    p.Kill();
                }
        }

        public static void PyrogenSay(string key, Color color)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                Main.NewText(Language.GetTextValue("Mods.CalamityFandom.NPCs.Pyrogen." + key), color);
            else if (Main.netMode == NetmodeID.Server)
                ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Mods.CalamityFandom.NPCs.Pyrogen." + key), color);
        }

        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            if (InHellstorm)
                modifiers.FinalDamage *= 0.5f;
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            // Contact pendant le Hellstorm : {{dv||300|500|6666}} (Expert et au-delà)
            if (InHellstorm && Main.expertMode)
                modifiers.SourceDamage *= Dv.Dv(1, 300, 500, 6666) / (float)Math.Max(1, NPC.damage);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(Main.expertMode ? BuffID.OnFire3 : BuffID.OnFire, 20 * 60);
            target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 5 * 60);
            // Dragon Fire : 1 s au contact, 30 min pendant le Hellstorm (page)
            target.AddBuff(ModContent.BuffType<Dragonfire>(), InHellstorm ? 30 * 60 * 60 : 60);
        }

        public override void OnKill()
        {
            if (!TamingSystem.TamingNow)
                PyrogenSystem.Downed = true;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);
        }

        public override void BossLoot(ref string name, ref int potionType) => potionType = ItemID.None;

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (Main.dedServ)
                return;
            for (int i = 0; i < (NPC.life <= 0 ? 60 : 5); i++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, NPC.life <= 0 ? DustID.Torch : DustID.Obsidian, hit.HitDirection * 2f, -2f, 100, default, 1.5f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = TextureAssets.Npc[Type].Value;
            Vector2 origin = tex.Size() / 2f;
            // Bouclier : morceaux d'obsidienne autour du boss (écartés en phase 4, absents séparé)
            if (Shielded && !Extended)
            {
                Texture2D shield = ModContent.Request<Texture2D>("CalamityMod/NPCs/Cryogen/CryogenShield").Value;
                float pulse = 0.85f + 0.15f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 4f);
                spriteBatch.Draw(shield, NPC.Center - screenPos, null, new Color(90, 40, 30) * pulse, Main.GlobalTimeWrappedHourly, shield.Size() / 2f, 1.15f, SpriteEffects.None, 0f);
            }
            Color glow = Color.Lerp(drawColor, Color.White, 0.6f);
            spriteBatch.Draw(tex, NPC.Center - screenPos, null, glow, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
            return false;
        }
    }

    /// <summary>Pyrogen est-il vaincu dans ce monde (sauvegardé).</summary>
    public class PyrogenSystem : ModSystem
    {
        public static bool Downed;

        public override void ClearWorld() => Downed = false;

        public override void SaveWorldData(TagCompound tag)
        {
            if (Downed)
                tag["downedPyrogen"] = true;
        }

        public override void LoadWorldData(TagCompound tag) => Downed = tag.ContainsKey("downedPyrogen");

        public override void NetSend(BinaryWriter writer) => writer.Write(Downed);

        public override void NetReceive(BinaryReader reader) => Downed = reader.ReadBoolean();
    }

    /// <summary>
    /// Invocation (page Pyrogen, « Spawn ») : une Cryo Key jetée dans la lave en Hardmode est consumée et Pyrogen apparaît près
    /// du joueur le plus proche. [choix] Pas de condition de zone : la lave suffit (Underworld ou Brimstone Crag d'habitude).
    /// </summary>
    public class PyrogenCryoKey : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ModContent.ItemType<CryoKey>();

        public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
        {
            if (!item.lavaWet || !Main.hardMode || Main.netMode == NetmodeID.MultiplayerClient || NPC.AnyNPCs(ModContent.NPCType<Pyrogen>()))
                return;
            Player closest = Main.player[Player.FindClosest(item.position, item.width, item.height)];
            if (!closest.active || closest.dead)
                return;
            item.stack--;
            if (item.stack <= 0)
                item.TurnToAir();
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, item.whoAmI);
            SoundEngine.PlaySound(SoundID.Roar, closest.Center);
            NPC.SpawnOnPlayer(closest.whoAmI, ModContent.NPCType<Pyrogen>());
        }
    }

    /// <summary>Éclat d'obsidienne de Pyrogen ; ai[0] = 1 : ceux de la phase 1, sans debuff ; sinon Armor Crunch 20 s.</summary>
    public class PyrogenShard : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Ranged/ObsidianShard";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.scale = 2.5f;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Main.rand.NextBool(4))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1f).noGravity = true;
        }

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, Color.OrangeRed, 0.4f);

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Projectile.ai[0] != 1f)
                target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 20 * 60);
        }
    }

    /// <summary>Éclat de la phase 2 : soumis à la gravité, se plante dans le sol, explose 1,5 s plus tard en 6 éclats.</summary>
    public class PyrogenFallingShard : PyrogenShard
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.tileCollide = true;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            if (Projectile.ai[1] > 0f)
            {
                Projectile.velocity = Vector2.Zero;
                if (++Projectile.ai[1] > 90f)
                    Projectile.Kill();
                return;
            }
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.25f, 14f);
            base.AI();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.ai[1] = 1f;
            Projectile.tileCollide = false;
            Projectile.position += oldVelocity;
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.ai[1] <= 0f)
                return;
            SoundEngine.PlaySound(SoundID.Item14 with { Volume = 0.4f }, Projectile.Center);
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            for (int i = 0; i < 6; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center - Vector2.UnitY * 8f, new Vector2(0f, -7f).RotatedBy(MathHelper.Lerp(-1.3f, 1.3f, i / 5f)),
                    ModContent.ProjectileType<PyrogenShard>(), Projectile.damage, 0f, Main.myPlayer);
        }
    }

    /// <summary>Magma Blast : boule de feu (On Fire! / Hellfire 3 s).</summary>
    public class MagmaBlast : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.Fireball;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 360;
            Projectile.scale = 1.3f;
        }

        public override void AI()
        {
            Projectile.rotation += 0.2f;
            Lighting.AddLight(Projectile.Center, 0.8f, 0.4f, 0.1f);
            if (Main.rand.NextBool(2))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1.4f).noGravity = true;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 200, 120, 200);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(Main.expertMode ? BuffID.OnFire3 : BuffID.OnFire, 180);
    }

    /// <summary>Morceau de bouclier qui monte depuis le bas (phase 3).</summary>
    public class PyrogenShieldPiece : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Ranged/ObsidianShard";

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 60;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 200;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(3))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1.2f).noGravity = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.Lerp(lightColor, new Color(255, 120, 60), 0.5f), 0f, tex.Size() / 2f, new Vector2(4f, 10f), SpriteEffects.None);
            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 5 * 60);
    }

    /// <summary>Morceau du bouclier écarté (phase 4) : tourne autour de Pyrogen (ai[0] = index du boss, ai[1] = angle), blesse et arrête les projectiles.</summary>
    public class PyrogenOrbitPiece : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Ranged/ObsidianShard";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 34;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 700;
        }

        public override void AI()
        {
            NPC boss = Main.npc[(int)Projectile.ai[0]];
            if (!boss.active || boss.ModNPC is not Pyrogen pyro || !pyro.Extended)
            {
                Projectile.Kill();
                return;
            }
            Projectile.ai[1] += 0.03f;
            Projectile.Center = boss.Center + Projectile.ai[1].ToRotationVector2() * Pyrogen.ExtendedShieldRadius;
            Projectile.rotation = Projectile.ai[1];
            // Arrête les projectiles des joueurs qui le touchent
            foreach (Projectile p in Main.ActiveProjectiles)
                if (p.friendly && p.damage > 0 && !p.minion && p.owner == Main.myPlayer && p.Hitbox.Intersects(Projectile.Hitbox))
                    p.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.Lerp(lightColor, new Color(255, 120, 60), 0.4f), Projectile.rotation, tex.Size() / 2f, 6f, SpriteEffects.None);
            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 5 * 60);
    }

    /// <summary>Amas d'obsidienne de fin de Hellstorm : vole vers le joueur et éclate au bout d'1 s en 30 éclats.</summary>
    public class ObsidianCluster : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Ranged/ObsidianShard";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 60;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
        }

        public override void AI() => Projectile.rotation += 0.3f;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            for (int i = 0; i < 6; i++)
                Main.EntitySpriteDraw(tex, Projectile.Center + (Projectile.rotation + i).ToRotationVector2() * 14f - Main.screenPosition, null,
                    Color.Lerp(lightColor, Color.OrangeRed, 0.5f), Projectile.rotation + i, tex.Size() / 2f, 7f, SpriteEffects.None);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item62, Projectile.Center);
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            for (int i = 0; i < 30; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(5f, 11f),
                    ModContent.ProjectileType<PyrogenShard>(), Projectile.damage / 2, 0f, Main.myPlayer);
        }
    }
}
