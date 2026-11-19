using System;
using System.Collections.Generic;
using System.IO;
using CalamityFandom.Common.Taming;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
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

namespace CalamityFandom.Content.NPCs.Acidsighter
{
    /// <summary>
    /// Wiki : Acidsighter.txt — boss pré-Hardmode, Demon Eye géant empoisonné par la Sulphurous Sea. Invoqué par le Poisoned
    /// Sclera dans la Sulphurous Sea la nuit. Trois phases (sous {{dv|50|55|60|65}} % puis {{dv|30|40|45|50}} %), chacune avec
    /// sa liste d'attaques jouées à tour de rôle ; furieux hors de la Sulphurous Sea et de l'Océan, et le jour (les deux
    /// s'additionnent). Irradiated 3 s au contact. Butin par WikiLoot (infobox), sac en Expert.
    /// </summary>
    [AutoloadBossHead]
    public class Acidsighter : ModNPC
    {
        public const string TamingKey = "Acidsighter";

        private ref float Attack => ref NPC.ai[1];
        private ref float Timer => ref NPC.ai[2];
        private ref float Counter => ref NPC.ai[3];

        private int phase = 1;
        private int attacksDone;
        private float spinAngle;
        private Vector2 dashVelocity;

        public int Phase => phase;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.TrailCacheLength[Type] = 6;
            NPCID.Sets.TrailingMode[Type] = 1;
            foreach (int buff in new[] { BuffID.Confused, ModContent.BuffType<PearlAura>(), ModContent.BuffType<Irradiated>(), ModContent.BuffType<SulphuricPoisoning>() })
                NPCID.Sets.SpecificDebuffImmunity[Type][buff] = true;
        }

        public override void SetDefaults()
        {
            NPC.boss = true;
            NPC.width = 90;
            NPC.height = 110;
            NPC.lifeMax = 4000;
            NPC.damage = 25;
            NPC.defense = 10;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = Item.buyPrice(gold: 20) / 5;
            NPC.npcSlots = 8f;
            if (!Main.dedServ)
                Music = MusicID.Boss1;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(Dv.Dv(4000, 6200, 8760, 8760) * balance);
            NPC.damage = Dv.Dv(25, 30, 35, 40);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) =>
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
                new FlavorTextBestiaryInfoElement("Mods.CalamityFandom.NPCs.Acidsighter.Bestiary"),
            });

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((byte)phase);
            writer.Write(attacksDone);
            writer.Write(spinAngle);
            writer.WriteVector2(dashVelocity);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            phase = reader.ReadByte();
            attacksDone = reader.ReadInt32();
            spinAngle = reader.ReadSingle();
            dashVelocity = reader.ReadVector2();
        }

        private Player Target => Main.player[NPC.target];
        private bool Server => Main.netMode != NetmodeID.MultiplayerClient;

        /// <summary>Furieux hors de la Sulphurous Sea et de l'Océan, et le jour : les deux s'additionnent. [choix] x1,5 chacun.</summary>
        private float Speed
        {
            get
            {
                float s = 1f;
                if (!Target.Calamity().ZoneSulphur && !Target.ZoneBeach)
                    s *= 1.5f;
                if (Main.dayTime)
                    s *= 1.5f;
                if (CalamityWorld.revenge)
                    s *= 1.1f;
                return s;
            }
        }

        private static float[] Phase2At => new[] { 0.5f, 0.55f, 0.6f, 0.65f };
        private static float[] Phase3At => new[] { 0.3f, 0.4f, 0.45f, 0.5f };
        private static float[] SpinAt => new[] { 0.65f, 0.7f, 0.75f, 0.75f };
        private static int Level => CalamityWorld.death ? 3 : CalamityWorld.revenge ? 2 : Main.expertMode ? 1 : 0;

        public static int AcidBombDamage => Dv.ProjDamage(15, 20, 22, 25);
        public static int AcidShotDamage => Dv.ProjDamage(20, 23, 30, 30);
        public static int TeethDamage => Dv.ProjDamage(30, 33, 36, 40);

        public override void AI()
        {
            NPC.TargetClosest(false);
            if (Target.dead || !Target.active || NPC.Distance(Target.Center) > 5000f)
            {
                NPC.velocity.Y -= 0.3f;
                NPC.EncourageDespawn(60);
                return;
            }
            Lighting.AddLight(NPC.Center, 0.2f, 0.5f * phase, 0.1f);
            float life = NPC.life / (float)NPC.lifeMax;
            if (phase == 1 && life < Phase2At[Level])
                EnterPhase(2, Main.rand.Next(4, 6));
            else if (phase == 2 && life < Phase3At[Level])
                EnterPhase(3, 0);

            Timer++;
            bool done = Run((int)Attack);
            if (done)
                NextAttack();
        }

        private void EnterPhase(int p, int eyes)
        {
            phase = p;
            SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
            if (Server)
                for (int i = 0; i < eyes; i++)
                    SpawnEye(NPC.Center + Main.rand.NextVector2Circular(60f, 60f));
            if (p == 3)
                NPC.damage = (int)(NPC.damage * 1.25f);
            Attack = Timer = Counter = 0f;
            attacksDone = 0;
            NPC.netUpdate = true;
        }

        // Attaques par phase (indices de Run) ; Expert : charge avec souffle d'Acid Venom en plus
        private int[] Attacks => phase switch
        {
            1 => Main.expertMode ? new[] { 0, 1, 2, 3, 9 } : new[] { 0, 1, 2, 3 },
            2 => Main.expertMode ? new[] { 4, 5, 6, 7, 8, 9 } : new[] { 4, 5, 6, 7, 8 },
            _ => Main.expertMode ? new[] { 10, 11, 12, 13, 14, 9 } : new[] { 10, 11, 12, 13, 14 },
        };

        private void NextAttack()
        {
            attacksDone++;
            int[] list = Attacks;
            int next = list[attacksDone % list.Length];
            // Phase 1 sous {{dv|65|70|75}} % : toutes les 2-3 attaques, tour autour du joueur puis charge
            if (phase == 1 && NPC.life < NPC.lifeMax * SpinAt[Level] && attacksDone % 3 == 2)
                next = 15;
            Attack = next;
            Timer = Counter = 0f;
            NPC.netUpdate = true;
        }

        private bool Run(int attack)
        {
            float s = Speed;
            switch (attack)
            {
                case 0: // Plane au-dessus du joueur en appelant des Mutated Eyes
                    Fly(Target.Center + new Vector2(0f, -320f), 9f * s, 15f);
                    LookAtTarget();
                    if (Timer % (int)(60 / s) == 0f && Server)
                        SpawnEye(NPC.Center);
                    return Timer > 180f;
                case 1: // 3 charges
                    return Charges(3, 0, s);
                case 2: // Acid Bombs (6 Acid Shots chacune)
                    Fly(Target.Center + new Vector2(Math.Sign(NPC.Center.X - Target.Center.X) * 300f, -250f), 9f * s, 15f);
                    LookAtTarget();
                    if (Timer % (int)(45 / s) == 0f)
                        Fire(ModContent.ProjectileType<AcidBomb>(), 9f, AcidBombDamage, 6);
                    return Timer > 160f;
                case 3: // Teeth Bombs (3 dents chercheuses chacune)
                    Fly(Target.Center + new Vector2(Math.Sign(NPC.Center.X - Target.Center.X) * 320f, -200f), 9f * s, 15f);
                    LookAtTarget();
                    if (Timer % (int)(60 / s) == 0f)
                        Fire(ModContent.ProjectileType<TeethBomb>(), 8f, TeethDamage, 3);
                    return Timer > 150f;
                case 4: // Phase 2 : au-dessus, Acid Bombs
                    Fly(Target.Center + new Vector2(0f, -300f), 10f * s, 12f);
                    LookAtTarget();
                    if (Timer % (int)(40 / s) == 0f)
                        Fire(ModContent.ProjectileType<AcidBomb>(), 9f, AcidBombDamage, 6);
                    return Timer > 160f;
                case 5: // 3 charges, 8 Acid Shots au départ de chacune
                    return Charges(3, 8, s);
                case 6: // Sous le joueur, Teeth Bombs
                    Fly(Target.Center + new Vector2(0f, 300f), 10f * s, 12f);
                    LookAtTarget();
                    if (Timer % (int)(50 / s) == 0f)
                        Fire(ModContent.ProjectileType<TeethBomb>(), 9f, TeethDamage, 3);
                    return Timer > 150f;
                case 7: // Tour autour du joueur puis charge qui sème des Acid Bombs (2 Acid Shots)
                    return SpinCharge(s, 2);
                case 8: // Poursuit en appelant des Mutated Eyes
                    Fly(Target.Center, 8f * s, 25f);
                    LookAtTarget();
                    if (Timer % (int)(70 / s) == 0f && Server)
                        SpawnEye(NPC.Center);
                    return Timer > 200f;
                case 9: // Expert : charge en soufflant de l'Acid Venom
                    if (Timer < 30f)
                    {
                        NPC.velocity *= 0.9f;
                        LookAtTarget();
                        return false;
                    }
                    if (Timer == 30f)
                        NPC.velocity = NPC.SafeDirectionTo(Target.Center) * 11f * s;
                    NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;
                    if (Timer % 4 == 0f && Server)
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + NPC.velocity.SafeNormalize(Vector2.UnitY) * 50f,
                            NPC.velocity.SafeNormalize(Vector2.UnitY).RotatedByRandom(0.2f) * 9f, ModContent.ProjectileType<AcidVenomBreath>(), 1, 0f, Main.myPlayer);
                    return Timer > 100f;
                case 10: // Phase 3 : tourne autour du joueur en lâchant des Acid Bombs
                    spinAngle += 0.04f * s;
                    Fly(Target.Center + spinAngle.ToRotationVector2() * 380f, 16f * s, 8f);
                    LookAtTarget();
                    if (Timer % (int)(30 / s) == 0f)
                        Fire(ModContent.ProjectileType<AcidBomb>(), 8f, AcidBombDamage, 6);
                    return Timer > 200f;
                case 11: // 3 charges, 12 Acid Shots au départ de chacune
                    return Charges(3, 12, s);
                case 12: // Chaîne de ruées rapides qui sème des Acid Bombs (4 Acid Shots)
                    return ChainDash(s);
                case 13: // Poursuit vite en lançant des Teeth Bombs
                    Fly(Target.Center, 13f * s, 15f);
                    LookAtTarget();
                    if (Timer % (int)(45 / s) == 0f)
                        Fire(ModContent.ProjectileType<TeethBomb>(), 9f, TeethDamage, 3);
                    return Timer > 180f;
                case 14: // Mutated Clusters qui ralentissent et lâchent 2-3 Mutated Eyes
                    Fly(Target.Center + new Vector2(0f, -280f), 10f * s, 12f);
                    LookAtTarget();
                    if (Timer % (int)(60 / s) == 0f)
                        Fire(ModContent.ProjectileType<MutatedCluster>(), 10f, AcidBombDamage, 0);
                    return Timer > 180f;
                default: // 15 : tour autour du joueur puis charge (phase 1)
                    return SpinCharge(s, 0);
            }
        }

        private void LookAtTarget() => NPC.rotation = NPC.rotation.AngleLerp(NPC.SafeDirectionTo(Target.Center).ToRotation() - MathHelper.PiOver2, 0.15f);

        private void Fly(Vector2 dest, float speed, float inertia)
        {
            Vector2 want = (dest - NPC.Center).SafeNormalize(Vector2.Zero) * Math.Min(speed, NPC.Distance(dest) / 10f + 1f);
            NPC.velocity = (NPC.velocity * (inertia - 1f) + want) / inertia;
        }

        private void Fire(int type, float speed, int damage, int splits)
        {
            SoundEngine.PlaySound(SoundID.Item17, NPC.Center);
            if (Server)
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + NPC.SafeDirectionTo(Target.Center) * 50f, NPC.SafeDirectionTo(Target.Center) * speed,
                    type, damage, 0f, Main.myPlayer, splits);
        }

        /// <summary>Charges : 30 images de visée, 40 de ruée, 20 de freinage ; « burst » Acid Shots en partant.</summary>
        private bool Charges(int count, int burst, float s)
        {
            float t = Timer % 90f;
            if (t < 30f)
            {
                NPC.velocity *= 0.9f;
                LookAtTarget();
            }
            else if (t == 30f)
            {
                float accel = CalamityWorld.death ? 1.3f : 1f;
                NPC.velocity = NPC.SafeDirectionTo(Target.Center) * 16f * s * accel;
                NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;
                SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                if (burst > 0 && Server)
                    for (int i = 0; i < burst; i++)
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, (MathHelper.TwoPi * i / burst).ToRotationVector2() * 6f,
                            ModContent.ProjectileType<AcidShot>(), AcidShotDamage, 0f, Main.myPlayer);
            }
            else if (t > 70f)
                NPC.velocity *= 0.92f;
            return Timer >= 90f * count;
        }

        /// <summary>Tour autour du joueur (1,5 s) puis charge ; « bombs » &gt; 0 : sème des Acid Bombs (« bombs » Acid Shots chacune) en chargeant.</summary>
        private bool SpinCharge(float s, int bombs)
        {
            if (Timer < 90f)
            {
                spinAngle += 0.07f * s;
                Fly(Target.Center + spinAngle.ToRotationVector2() * 420f, 20f * s, 6f);
                LookAtTarget();
                return false;
            }
            if (Timer == 90f)
            {
                NPC.velocity = NPC.SafeDirectionTo(Target.Center) * 18f * s;
                NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;
                SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
            }
            if (bombs > 0 && Timer % 8 == 0f && Server)
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<AcidBomb>(), AcidBombDamage, 0f, Main.myPlayer, bombs);
            if (Timer > 130f)
                NPC.velocity *= 0.93f;
            return Timer > 150f;
        }

        /// <summary>Phase 3 : 5 ruées rapides enchaînées (25 images), Acid Bombs semées (4 Acid Shots).</summary>
        private bool ChainDash(float s)
        {
            float t = Timer % 25f;
            if (t == 1f)
            {
                NPC.velocity = NPC.SafeDirectionTo(Target.Center + Target.velocity * 15f) * 20f * s;
                NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;
                SoundEngine.PlaySound(SoundID.Item74, NPC.Center);
            }
            else if (t > 18f)
                NPC.velocity *= 0.85f;
            if (Timer % 6 == 0f && Server)
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<AcidBomb>(), AcidBombDamage, 0f, Main.myPlayer, 4);
            return Timer >= 125f;
        }

        public void SpawnEye(Vector2 at)
        {
            int n = NPC.NewNPC(NPC.GetSource_FromAI(), (int)at.X, (int)at.Y, ModContent.NPCType<MutatedEye>(), 0, NPC.whoAmI);
            if (Main.npc.IndexInRange(n))
                Main.npc[n].velocity = Main.rand.NextVector2Circular(5f, 5f);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo) => target.AddBuff(ModContent.BuffType<Irradiated>(), 180);

        public override void OnKill()
        {
            if (!TamingSystem.TamingNow)
                AcidsighterSystem.Downed = true;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) => npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<Items.TreasureBags.AcidsighterBag>()));

        public override void BossLoot(ref string name, ref int potionType) => potionType = ItemID.LesserHealingPotion;

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (Main.dedServ)
                return;
            for (int i = 0; i < (NPC.life <= 0 ? 50 : 4); i++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GreenBlood, hit.HitDirection * 2f, -1f, 100, default, 1.4f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = TextureAssets.Npc[Type].Value;
            Vector2 origin = tex.Size() / 2f;
            // Lueur verte en phases 2 et 3 (plus forte en 3)
            if (phase >= 2)
            {
                float glow = (phase == 2 ? 0.35f : 0.65f) * (0.8f + 0.2f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 5f));
                for (int i = 0; i < 4; i++)
                    spriteBatch.Draw(tex, NPC.Center - screenPos + (MathHelper.PiOver2 * i).ToRotationVector2() * 4f, null, new Color(120, 255, 80, 0) * glow,
                        NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(tex, NPC.Center - screenPos, null, NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
            return false;
        }
    }

    /// <summary>
    /// Mutated Eye (page, « Summons ») : œil volant appelé par l'Acidsighter. Vie {{dv|30|45|55}}, dégâts {{dv|14|28|36|44}},
    /// défense 3, recul {{dv|10|20|30}} % ; plus rapide en Expert ; en Revengeance, tire des Acid Shots quand l'Acidsighter
    /// est sous 55 %.
    /// </summary>
    public class MutatedEye : ModNPC
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][ModContent.BuffType<Irradiated>()] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][ModContent.BuffType<SulphuricPoisoning>()] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 26;
            NPC.height = 18;
            NPC.lifeMax = 30;
            NPC.damage = 14;
            NPC.defense = 3;
            NPC.knockBackResist = 0.9f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(Dv.Dv(30, 45, 55, 55) * balance);
            NPC.damage = Dv.Dv(14, 28, 36, 44);
            NPC.knockBackResist = 1f - Dv.Dv(10, 20, 30, 30) / 100f;
        }

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player target = Main.player[NPC.target];
            float speed = Main.expertMode ? 8f : 6f;
            NPC.velocity = (NPC.velocity * 30f + NPC.SafeDirectionTo(target.Center) * speed) / 31f;
            NPC.rotation = NPC.velocity.ToRotation();
            NPC.spriteDirection = 1;
            NPC boss = Main.npc.IndexInRange((int)NPC.ai[0]) ? Main.npc[(int)NPC.ai[0]] : null;
            if (CalamityWorld.revenge && boss != null && boss.active && boss.ModNPC is Acidsighter && boss.life < boss.lifeMax * 0.55f
                && ++NPC.ai[1] % 150 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.SafeDirectionTo(target.Center) * 7f, ModContent.ProjectileType<AcidShot>(),
                    Acidsighter.AcidShotDamage, 0f, Main.myPlayer);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo) => target.AddBuff(ModContent.BuffType<Irradiated>(), 120);

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (!Main.dedServ)
                for (int i = 0; i < (NPC.life <= 0 ? 10 : 2); i++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GreenBlood, hit.HitDirection, -1f);
        }
    }

    public class AcidsighterSystem : ModSystem
    {
        public static bool Downed;

        public override void ClearWorld() => Downed = false;

        public override void SaveWorldData(TagCompound tag)
        {
            if (Downed)
                tag["downedAcidsighter"] = true;
        }

        public override void LoadWorldData(TagCompound tag) => Downed = tag.ContainsKey("downedAcidsighter");

        public override void NetSend(BinaryWriter writer) => writer.Write(Downed);

        public override void NetReceive(BinaryReader reader) => Downed = reader.ReadBoolean();
    }

    /// <summary>Acid Bomb : éclate en ai[0] Acid Shots (6 par défaut) après 1 s ou au contact d'un bloc.</summary>
    public class AcidBomb : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/NPCs/Acidsighter/AcidBomb";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            Projectile.rotation += 0.15f;
            Projectile.velocity *= 0.98f;
            Lighting.AddLight(Projectile.Center, 0.2f, 0.5f, 0.1f);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<Irradiated>(), 120);

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14 with { Volume = 0.4f }, Projectile.Center);
            int count = Projectile.ai[0] > 0 ? (int)Projectile.ai[0] : 6;
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            for (int i = 0; i < count; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (MathHelper.TwoPi * i / count).ToRotationVector2() * 6f,
                    ModContent.ProjectileType<AcidShot>(), Acidsighter.AcidShotDamage, 0f, Main.myPlayer);
        }
    }

    /// <summary>Acid Shot : goutte acide soumise à la gravité (Irradiated 2 s).</summary>
    public class AcidShot : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/NPCs/Acidsighter/AcidShot";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 240;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.12f, 10f);
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<Irradiated>(), 120);
    }

    /// <summary>Teeth Bomb : éclate après 1 s en ai[0] dents chercheuses (3).</summary>
    public class TeethBomb : AcidBomb
    {
        public override string Texture => "CalamityFandom/Content/NPCs/Acidsighter/TeethBomb";

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath13, Projectile.Center);
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            int count = Projectile.ai[0] > 0 ? (int)Projectile.ai[0] : 3;
            for (int i = 0; i < count; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (MathHelper.TwoPi * i / count).ToRotationVector2() * 5f,
                    ModContent.ProjectileType<HomingTooth>(), Acidsighter.TeethDamage, 0f, Main.myPlayer);
        }
    }

    /// <summary>Dent chercheuse : suit le joueur le plus proche pendant 2 s.</summary>
    public class HomingTooth : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.Bone;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.scale = 0.8f;
        }

        public override void AI()
        {
            if (Projectile.timeLeft > 60)
            {
                Player p = Main.player[Player.FindClosest(Projectile.position, Projectile.width, Projectile.height)];
                Projectile.velocity = (Projectile.velocity * 20f + Projectile.SafeDirectionTo(p.Center) * 8f) / 21f;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, new Color(200, 230, 120), 0.3f);
    }

    /// <summary>Mutated Cluster : ralentit puis éclate en 2-3 Mutated Eyes.</summary>
    public class MutatedCluster : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/NPCs/Acidsighter/MutatedEye";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 70;
            Projectile.tileCollide = false;
            Projectile.scale = 1.6f;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.95f;
            Projectile.rotation += 0.2f;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath13, Projectile.Center);
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            NPC boss = null;
            foreach (NPC n in Main.ActiveNPCs)
                if (n.ModNPC is Acidsighter)
                    boss = n;
            for (int i = 0; i < Main.rand.Next(2, 4); i++)
            {
                int e = NPC.NewNPC(Projectile.GetSource_FromThis(), (int)Projectile.Center.X, (int)Projectile.Center.Y, ModContent.NPCType<MutatedEye>(), 0, boss?.whoAmI ?? -1);
                if (Main.npc.IndexInRange(e))
                    Main.npc[e].velocity = Main.rand.NextVector2Circular(6f, 6f);
            }
        }
    }

    /// <summary>Souffle d'Acid Venom (Expert) : 1 dégât, Venom {{dv||3|4}} s (« Acid Venom »).</summary>
    public class AcidVenomBreath : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 40;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (!Main.dedServ)
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Venom, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, default, 1.5f).noGravity = true;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.Venom, CalamityWorld.revenge ? 240 : 180);
    }
}

namespace CalamityFandom.Content.Items.SummonItems
{
    /// <summary>Wiki : Poisoned Sclera.txt — invoque l'Acidsighter dans la Sulphurous Sea la nuit ; pas consommé.</summary>
    public class PoisonedSclera : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 2;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 20;
            Item.useTime = Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
            Item.rare = ItemRarityID.Green;
        }

        public override bool CanUseItem(Player player) => player.Calamity().ZoneSulphur && !Main.dayTime && !NPC.AnyNPCs(ModContent.NPCType<NPCs.Acidsighter.Acidsighter>());

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI != Main.myPlayer)
                return true;
            SoundEngine.PlaySound(SoundID.Roar, player.Center);
            int type = ModContent.NPCType<NPCs.Acidsighter.Acidsighter>();
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.SpawnOnPlayer(player.whoAmI, type);
            else
                NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
            return true;
        }
    }
}

namespace CalamityFandom.Content.Items.TreasureBags
{
    /// <summary>Wiki : Treasure Bag (Acidsighter).txt — sac Expert ; son contenu vient de WikiLoot (Calamity++ : butin de la page).</summary>
    public class AcidsighterBag : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 3;
            ItemID.Sets.BossBag[Type] = true;
            ItemID.Sets.PreHardmodeLikeBossBag[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.width = Item.height = 32;
            Item.rare = ItemRarityID.Expert;
            Item.expert = true;
        }

        public override bool CanRightClick() => true;

        public override void ModifyItemLoot(ItemLoot itemLoot) => itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<NPCs.Acidsighter.Acidsighter>()));
    }
}

namespace CalamityFandom.Content.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Pungent Bomber.txt — chaque coup pose une Acid Bomb immobile qui éclate en 4 Acid Shots soumis à la gravité
    /// (50 % chacun) au contact d'un ennemi ou après 4 s ; Irradiated 2 s. Butin de l'Acidsighter.
    /// </summary>
    public class PungentBomber : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 27;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 33;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5.5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = 48;
            Item.height = 54;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(silver: 80);
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 120);
            if (player.whoAmI == Main.myPlayer)
                Projectile.NewProjectile(player.GetSource_ItemUse(Item), target.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.Acidsighter.FriendlyAcidBomb>(),
                    player.GetWeaponDamage(Item), 2f, player.whoAmI, 4, 0);
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Summon
{
    /// <summary>Wiki : Corrosive Eye Staff.txt — des yeux corrodés tournent autour du joueur et tirent des Acid Shots sur les ennemis proches.</summary>
    public class CorrosiveEyeStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.StaffMinionSlotsRequired[Type] = 1f;
        }

        public override void SetDefaults()
        {
            Item.damage = 21;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 10;
            Item.useTime = Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item44;
            Item.width = Item.height = 42;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 2, silver: 40);
            Item.buffType = ModContent.BuffType<Buffs.CorrosiveEyeBuff>();
            Item.shoot = ModContent.ProjectileType<Projectiles.Acidsighter.CorrosiveEye>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            int p = Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI);
            if (Main.projectile.IndexInRange(p))
                Main.projectile[p].originalDamage = Item.damage;
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Rogue
{
    /// <summary>
    /// Wiki : Razor Teeth.txt — dent soumise à la gravité qui perce deux fois et éclate en Acid Shots au contact (Irradiated
    /// 3 s) ; frappe furtive : trois dents chercheuses qui se plantent dans les ennemis (Acid Venom 2 s). Butin de l'Acidsighter.
    /// [choix] Rareté verte (la page n'en donne pas) ; 2 Acid Shots par coup (50 %) ; dents plantées 2 s, un coup toutes les 20 images.
    /// </summary>
    public class RazorTeeth : CalamityMod.Items.Weapons.Rogue.RogueWeapon
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.useTime = Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = Item.noUseGraphic = true;
            Item.knockBack = 2f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = 18;
            Item.height = 30;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(silver: 80);
            Item.shoot = ModContent.ProjectileType<Projectiles.Acidsighter.RazorTooth>();
            Item.shootSpeed = 11f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!player.Calamity().StealthStrikeAvailable())
            {
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                return false;
            }
            for (int i = -1; i <= 1; i++)
            {
                int p = Projectile.NewProjectile(source, position, velocity.RotatedBy(0.2f * i), type, damage, knockback, player.whoAmI, 1f);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].Calamity().stealthStrike = true;
            }
            return false;
        }
    }
}

namespace CalamityFandom.Content.Buffs
{
    public class CorrosiveEyeBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Acidsighter.CorrosiveEye>()] > 0)
                player.buffTime[buffIndex] = 18000;
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Acidsighter
{
    /// <summary>Acid Bomb du Pungent Bomber : immobile, éclate en ai[0] Acid Shots (50 %) au contact d'un ennemi ou après 4 s.</summary>
    public class FriendlyAcidBomb : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/NPCs/Acidsighter/AcidBomb";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 240;
            Projectile.tileCollide = false;
            Projectile.scale = 0.8f;
        }

        public override void OnSpawn(IEntitySource source)
        {
            // Pas de coup sur l'ennemi qui vient d'être frappé par l'épée : il explose au prochain contact
            Projectile.localAI[0] = 20f;
        }

        public override bool? CanDamage() => Projectile.localAI[0] > 0f ? false : null;

        public override void AI()
        {
            if (Projectile.localAI[0] > 0f)
                Projectile.localAI[0]--;
            Projectile.velocity = Vector2.Zero;
            Projectile.rotation += 0.05f;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14 with { Volume = 0.4f }, Projectile.Center);
            if (Projectile.owner != Main.myPlayer)
                return;
            int count = Math.Max(1, (int)Projectile.ai[0]);
            for (int i = 0; i < count; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-6f, -2f)),
                    ModContent.ProjectileType<FriendlyAcidShot>(), Projectile.damage / 2, 0f, Projectile.owner, (int)Projectile.ai[1]);
        }
    }

    /// <summary>Acid Shot des armes : gravité, Irradiated 2 s. ai[0] : 0 mêlée, 1 invocation, 2 voleur.</summary>
    public class FriendlyAcidShot : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/NPCs/Acidsighter/AcidShot";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.DamageType = (int)Projectile.ai[0] switch
            {
                1 => DamageClass.Summon,
                2 => ModContent.GetInstance<RogueDamageClass>(),
                _ => DamageClass.Melee,
            };
            if (Projectile.ai[0] == 1f)
                ProjectileID.Sets.MinionShot[Type] = true;
        }

        public override void AI()
        {
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.15f, 10f);
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Irradiated>(), 120);
    }

    /// <summary>Œil corrodé du Corrosive Eye Staff : tourne autour du joueur, tire un Acid Shot vers l'ennemi proche toutes les 50 images.</summary>
    public class CorrosiveEye : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Summon/CorrosiveEye";

        public override void SetStaticDefaults()
        {
            Main.projPet[Type] = true;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 1f;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 18000;
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.dead || !owner.active)
                owner.ClearBuff(ModContent.BuffType<Buffs.CorrosiveEyeBuff>());
            if (owner.HasBuff(ModContent.BuffType<Buffs.CorrosiveEyeBuff>()))
                Projectile.timeLeft = 2;
            int count = Math.Max(1, owner.ownedProjectileCounts[Type]);
            float angle = Main.GlobalTimeWrappedHourly * 2f + MathHelper.TwoPi * Projectile.minionPos / count;
            Projectile.Center = Vector2.Lerp(Projectile.Center, owner.Center + angle.ToRotationVector2() * 70f, 0.3f);
            NPC target = Projectile.Center.MinionHoming(500f, owner);
            Projectile.rotation = target != null ? Projectile.SafeDirectionTo(target.Center).ToRotation() - MathHelper.PiOver2 : 0f;
            if (target != null && Projectile.owner == Main.myPlayer && ++Projectile.ai[0] >= 50f)
            {
                Projectile.ai[0] = 0f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.SafeDirectionTo(target.Center - Vector2.UnitY * 20f) * 10f,
                    ModContent.ProjectileType<FriendlyAcidShot>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 1f);
            }
        }
    }

    /// <summary>
    /// Dent du Razor Teeth : gravité, perce 2 fois, 2 Acid Shots à chaque coup (Irradiated 3 s). ai[0] = 1 (furtif) :
    /// chercheuse, se plante dans l'ennemi touché 2 s (un coup toutes les 20 images, Acid Venom 2 s).
    /// </summary>
    public class RazorTooth : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Rogue/RazorTeeth";

        private bool Stealth => Projectile.ai[0] == 1f;
        private int stuckTo = -1;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = 3;
            Projectile.timeLeft = 240;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            if (stuckTo >= 0)
            {
                NPC n = Main.npc[stuckTo];
                if (!n.active || Projectile.timeLeft > 120)
                {
                    Projectile.Kill();
                    return;
                }
                Projectile.Center = n.Center + new Vector2(Projectile.localAI[0], Projectile.localAI[1]);
                Projectile.velocity = Vector2.Zero;
                return;
            }
            if (Stealth)
                CalamityUtils.HomeInOnNPC(Projectile, true, 600f, 12f, 15f);
            else
                Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.2f, 14f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
            if (Stealth)
            {
                target.AddBuff(BuffID.Venom, 120);
                if (stuckTo < 0)
                {
                    stuckTo = target.whoAmI;
                    Projectile.localAI[0] = Projectile.Center.X - target.Center.X;
                    Projectile.localAI[1] = Projectile.Center.Y - target.Center.Y;
                    Projectile.penetrate = -1;
                    Projectile.tileCollide = false;
                    Projectile.timeLeft = 120;
                    Projectile.netUpdate = true;
                }
                return;
            }
            if (Projectile.owner == Main.myPlayer)
                for (int i = 0; i < 2; i++)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-5f, -2f)),
                        ModContent.ProjectileType<FriendlyAcidShot>(), Projectile.damage / 2, 0f, Projectile.owner, 2f);
        }
    }
}
