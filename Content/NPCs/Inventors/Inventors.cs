using System;
using System.IO;
using CalamityFandom.Common.Taming;
using CalamityMod;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

// The Inventors (wiki : The Inventors.txt) : John et Jane Wulfrum, duo de boss d'avant Hardmode, le premier du jeu.
// Invoqués par une Draedon Power Cell sur un Wulfrum Signal Transmitter (Content/Tiles/WulfrumSignalTransmitter.cs).

namespace CalamityFandom.Content.NPCs.Inventors
{
    /// <summary>
    /// Base commune : vie et dégâts par difficulté (page), seuils de 75/50/25 % qui débloquent les attaques, bouclier
    /// invincible pour celui qui a dépassé un seuil avant l'autre, saut vers l'autre s'ils sont trop loin, départ quand
    /// il n'y a plus de cible.
    /// </summary>
    public abstract class InventorBase : ModNPC
    {
        protected abstract int Defense { get; }

        /// <summary>L'autre inventeur (index dans Main.npc, -1 s'il est mort).</summary>
        public int Partner = -1;
        private int farTimer;
        private bool saidHalf;

        protected Player Target => Main.player[NPC.target];

        /// <summary>Nombre de seuils passés (0 à 3) ; en Death Mode, une série d'attaques d'avance (page).</summary>
        public int Phase
        {
            get
            {
                float ratio = NPC.life / (float)NPC.lifeMax;
                int p = ratio <= 0.25f ? 3 : ratio <= 0.5f ? 2 : ratio <= 0.75f ? 1 : 0;
                return CalamityWorld.death ? Math.Min(p + 1, 3) : p;
            }
        }

        private int RawPhase => NPC.life / (float)NPC.lifeMax <= 0.25f ? 3 : NPC.life / (float)NPC.lifeMax <= 0.5f ? 2 : NPC.life / (float)NPC.lifeMax <= 0.75f ? 1 : 0;

        /// <summary>Bouclier : ce boss a passé plus de seuils que l'autre, encore vivant.</summary>
        public bool Shielded => PartnerNPC is NPC p && RawPhase > ((InventorBase)p.ModNPC).RawPhase;

        public NPC PartnerNPC => Partner >= 0 && Main.npc[Partner].active && Main.npc[Partner].ModNPC is InventorBase ? Main.npc[Partner] : null;

        /// <summary>Valeur selon la difficulté : normal, Expert, Revengeance, Death (modèle {{dv}} du wiki).</summary>
        public static int Dv(int normal, int expert, int rev, int death) =>
            CalamityWorld.death ? death : CalamityWorld.revenge ? rev : Main.expertMode ? expert : normal;

        /// <summary>Dégâts d'un projectile ennemi : le jeu les multiplie en Expert et en Master, on vise la valeur du wiki.</summary>
        public static int ProjDamage(int normal, int expert, int rev, int death) =>
            Math.Max(1, (int)(Dv(normal, expert, rev, death) / Math.Max(1f, Main.GameModeInfo.EnemyDamageMultiplier)));

        public override void SetStaticDefaults()
        {
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Slow] = true;
        }

        public override void SetDefaults()
        {
            NPC.boss = true;
            NPC.lifeMax = 450;
            NPC.damage = 10;
            NPC.defense = Defense;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.Item14;
            NPC.value = Item.buyPrice(gold: 10) / 2;
            NPC.npcSlots = 5f;
            NPC.Calamity().DR = 0.05f;
            NPC.Calamity().VulnerableToHeat = true;
            if (!Main.dedServ)
                Music = MusicID.Boss1;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(Dv(450, 520, 564, 640) * balance);
            NPC.damage = Dv(10, 16, 20, 25);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) =>
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("Mods.CalamityFandom.NPCs." + Name + ".Bestiary"),
            });

        public override void SendExtraAI(BinaryWriter writer) => writer.Write((short)Partner);

        public override void ReceiveExtraAI(BinaryReader reader) => Partner = reader.ReadInt16();

        public static void Say(string key, Color color)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                Main.NewText(Language.GetTextValue("Mods.CalamityFandom.Inventors." + key), color);
            else if (Main.netMode == NetmodeID.Server)
                ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Mods.CalamityFandom.Inventors." + key), color);
        }

        protected abstract void Attack();

        protected abstract string HalfQuote { get; }

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Target.dead || !Target.active)
                NPC.TargetClosest();
            // Plus de cible : ils partent en courant
            if (Target.dead || !Target.active || Vector2.Distance(Target.Center, NPC.Center) > 6000f)
            {
                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, 8f * (NPC.Center.X < Main.maxTilesX * 8f ? -1f : 1f), 0.1f);
                NPC.spriteDirection = NPC.direction = Math.Sign(NPC.velocity.X);
                NPC.EncourageDespawn(10);
                return;
            }
            NPC.dontTakeDamage = Shielded;
            NPC.Calamity().canBreakPlayerDefense = false;
            if (!saidHalf && RawPhase >= 2 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                saidHalf = true;
                Say(HalfQuote, Color.LimeGreen);
            }
            // Trop loin de l'autre trop longtemps : celui qui est le plus loin du joueur saute vers l'autre (page, Notes)
            if (PartnerNPC is NPC partner && Vector2.Distance(partner.Center, NPC.Center) > 900f
                && Vector2.Distance(NPC.Center, Target.Center) > Vector2.Distance(partner.Center, Target.Center))
            {
                if (++farTimer > 180 && NPC.velocity.Y == 0f)
                {
                    farTimer = 0;
                    Vector2 leap = partner.Center - NPC.Center;
                    NPC.velocity = new Vector2(MathHelper.Clamp(leap.X / 45f, -14f, 14f), -12f);
                    NPC.netUpdate = true;
                }
            }
            else
                farTimer = 0;
            Attack();
            if (NPC.velocity.X != 0f)
                NPC.spriteDirection = NPC.direction = Math.Sign(NPC.velocity.X);
        }

        /// <summary>Marche vers une position, saute devant un obstacle ou si la cible est au-dessus.</summary>
        protected void Walk(Vector2 goal, float speed, float accel = 0.15f)
        {
            float dir = Math.Sign(goal.X - NPC.Center.X);
            if (Math.Abs(goal.X - NPC.Center.X) < 12f)
                dir = 0f;
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, dir * speed, accel);
            if (NPC.velocity.Y == 0f && NPC.collideX)
                NPC.velocity.Y = -8f;
            else if (NPC.velocity.Y == 0f && goal.Y < NPC.Top.Y - 64f && Main.rand.NextBool(40))
                NPC.velocity.Y = -9f;
            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
        }

        protected int Shoot(Vector2 from, Vector2 velocity, int type, int damage, float ai0 = 0f)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return -1;
            return Projectile.NewProjectile(NPC.GetSource_FromAI(), from, velocity, type, damage, 0f, Main.myPlayer, ai0);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = TextureAssets.Npc[Type].Value;
            SpriteEffects fx = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 pos = NPC.Bottom - screenPos + new Vector2(0f, NPC.gfxOffY + 2f);
            spriteBatch.Draw(tex, pos, null, NPC.GetAlpha(drawColor), NPC.rotation, new Vector2(tex.Width / 2f, tex.Height), NPC.scale, fx, 0f);
            if (Shielded)
            {
                // Bouclier d'énergie
                float pulse = 0.5f + 0.2f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 6f);
                spriteBatch.Draw(tex, pos + new Vector2(0f, 4f), null, new Color(120, 255, 120, 0) * pulse, NPC.rotation,
                    new Vector2(tex.Width / 2f, tex.Height), NPC.scale * 1.25f, fx, 0f);
                if (Main.rand.NextBool(3))
                    Dust.NewDustDirect(NPC.position - new Vector2(8f), NPC.width + 16, NPC.height + 16, DustID.GreenTorch, 0f, 0f, 100, default, 0.8f).noGravity = true;
            }
            return false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int i = 0; i < (NPC.life <= 0 ? 20 : 3); i++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Electric, hit.HitDirection, -1f, 100, default, 0.8f);
        }

        public override void OnKill()
        {
            if (PartnerNPC == null && !TamingSystem.TamingNow)
            {
                InventorsDowned.Downed = true;
                Say("Defeated", Color.LimeGreen);
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData);
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Le butin n'arrive qu'avec le second inventeur (ou quand ils sont apprivoisés)
            var last = new LeadingConditionRule(new LastInventorCondition());
            last.OnSuccess(ItemDropRule.BossBag(ModContent.ItemType<InventorsBag>()));
            var normal = new LeadingConditionRule(new Conditions.NotExpert());
            InventorsBag.AddContents(normal);
            last.OnSuccess(normal);
            last.OnSuccess(ItemDropRule.Common(ItemID.LesserHealingPotion, 1, 8, 14));
            npcLoot.Add(last);
            // Récompense du Taming (page Taming : Wulfrum Hardhat, « Pacifist Drop »)
            var tamed = new LeadingConditionRule(new TamedCondition());
            tamed.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Items.Armor.WulfrumHardhat>()));
            npcLoot.Add(tamed);
        }

        public override void BossLoot(ref string name, ref int potionType) => potionType = ItemID.None;
    }

    /// <summary>Vrai pour le dernier inventeur en vie, ou pendant un apprivoisement.</summary>
    public class LastInventorCondition : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info) =>
            TamingSystem.TamingNow || info.npc?.ModNPC is not InventorBase b || b.PartnerNPC == null;

        public bool CanShowItemDropInUI() => true;

        public string GetConditionDescription() => Language.GetTextValue("Mods.CalamityFandom.Inventors.LootCondition");
    }

    /// <summary>
    /// Wiki : The Inventors.txt — John Wulfrum : avance et saute vers le joueur ; à 75 %, court et fonce à l'horizontale ;
    /// à 50 %, tire des Wulfrum Bolts au début et à la fin de chaque ruée ; à 25 %, se charge, saute par-dessus le joueur et
    /// retombe lourdement. Revengeance : ruées dans toutes les directions.
    /// </summary>
    [AutoloadBossHead]
    public class JohnWulfrum : InventorBase
    {
        protected override int Defense => 10;
        protected override string HalfQuote => "JohnHalf";

        private ref float Timer => ref NPC.ai[0];
        private ref float State => ref NPC.ai[1];   // 0 marche, 1 ruée, 2 charge du saut, 3 saut, 4 retombée

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 28;
            NPC.height = 54;
        }

        public override void OnSpawn(Terraria.DataStructures.IEntitySource source)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            int jane = NPC.NewNPC(source, (int)NPC.Center.X - 60, (int)NPC.Bottom.Y, ModContent.NPCType<JaneWulfrum>());
            if (Main.npc.IndexInRange(jane))
            {
                Partner = jane;
                ((InventorBase)Main.npc[jane].ModNPC).Partner = NPC.whoAmI;
                Main.npc[jane].netUpdate = true;
            }
            NPC.netUpdate = true;
            Say("Summoned", Color.LimeGreen);
        }

        private float DashGap => CalamityWorld.revenge ? 120f : Main.expertMode ? 160f : 200f;

        protected override void Attack()
        {
            Timer++;
            float walkSpeed = (Phase >= 1 ? 3.6f : 2.4f) * (CalamityWorld.death ? 1.4f : 1f);
            switch (State)
            {
                case 0:
                    Walk(Target.Center, walkSpeed);
                    if (Phase >= 3 && Timer > 420f && NPC.velocity.Y == 0f)
                        Switch(2);
                    else if (Phase >= 1 && Timer > DashGap && NPC.velocity.Y == 0f)
                        Switch(1);
                    break;
                case 1:
                    // Ruée : 30 images, horizontale (toutes directions en Revengeance)
                    NPC.Calamity().canBreakPlayerDefense = true;
                    if (Timer == 1f)
                    {
                        Vector2 dir = CalamityWorld.revenge ? NPC.SafeDirectionTo(Target.Center) : new Vector2(Math.Sign(Target.Center.X - NPC.Center.X), 0f);
                        NPC.velocity = dir * 12f;
                        if (CalamityWorld.revenge)
                            NPC.noGravity = true;
                        SoundEngine.PlaySound(SoundID.Item74 with { Volume = 0.5f }, NPC.Center);
                        if (Phase >= 2)
                            Bolts();
                        for (int i = 0; i < 15; i++)
                            Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Electric).velocity *= 2f;
                    }
                    if (Timer >= 30f)
                    {
                        NPC.noGravity = false;
                        if (Phase >= 2)
                            Bolts();
                        NPC.velocity *= 0.3f;
                        Switch(0);
                    }
                    break;
                case 2:
                    // Se charge
                    NPC.velocity.X *= 0.85f;
                    if (Main.rand.NextBool(2))
                        Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Electric, 0f, -2f).noGravity = true;
                    if (Timer >= 60f)
                    {
                        // Saut par-dessus le joueur
                        float dx = Target.Center.X - NPC.Center.X;
                        NPC.velocity = new Vector2(MathHelper.Clamp(dx / 40f, -16f, 16f), -15f);
                        SoundEngine.PlaySound(SoundID.Item14 with { Volume = 0.5f }, NPC.Center);
                        Switch(3);
                    }
                    break;
                case 3:
                    if (NPC.velocity.Y >= 0f && Timer > 20f)
                    {
                        NPC.velocity = new Vector2(0f, 18f);
                        Switch(4);
                    }
                    break;
                case 4:
                    NPC.Calamity().canBreakPlayerDefense = true;
                    NPC.velocity.Y = 18f;
                    if (NPC.collideY || Timer > 90f)
                    {
                        SoundEngine.PlaySound(SoundID.Item62 with { Volume = 0.6f }, NPC.Bottom);
                        Collision.HitTiles(NPC.BottomLeft, Vector2.UnitY * 6f, NPC.width, 8);
                        for (int i = 0; i < 25; i++)
                            Dust.NewDustDirect(NPC.BottomLeft - new Vector2(30f, 8f), NPC.width + 60, 10, DustID.Smoke, Main.rand.NextFloat(-6f, 6f), -2f);
                        Switch(0);
                    }
                    break;
            }
        }

        private void Bolts()
        {
            int damage = ProjDamage(10, 16, 20, 25);
            Vector2 dir = NPC.SafeDirectionTo(Target.Center);
            for (int i = -1; i <= 1; i++)
                Shoot(NPC.Center, dir.RotatedBy(0.2f * i) * 8f, ModContent.ProjectileType<InventorBolt>(), damage);
        }

        private void Switch(int state)
        {
            State = state;
            Timer = 0f;
            NPC.netUpdate = true;
        }
    }

    /// <summary>
    /// Wiki : The Inventors.txt — Jane Wulfrum : se tient derrière John et tire des flèches (Wulfrum Arrows qui éclatent en
    /// éclats en Expert) ; à 75 %, lance des Wulfrum Knives en éventail ; à 50 %, tire au Wulfrum Blunderbuss et frappe à
    /// l'épée si le joueur est tout près ; à 25 %, les couteaux deviennent des Wulfrum Shock Cutters. Revengeance : bouge
    /// toujours en tirant.
    /// </summary>
    [AutoloadBossHead]
    public class JaneWulfrum : InventorBase
    {
        protected override int Defense => 4;
        protected override string HalfQuote => "JaneHalf";

        private ref float ArrowTimer => ref NPC.ai[0];
        private ref float SpecialTimer => ref NPC.ai[1];
        private ref float SwordTimer => ref NPC.ai[2];

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 26;
            NPC.height = 44;
        }

        protected override void Attack()
        {
            float speed = 2.6f * (CalamityWorld.death ? 1.4f : 1f);
            // Derrière John, à l'opposé du joueur
            Vector2 goal = PartnerNPC is NPC john
                ? john.Center + new Vector2(Math.Sign(john.Center.X - Target.Center.X) * 120f, 0f)
                : Target.Center + new Vector2(Math.Sign(NPC.Center.X - Target.Center.X) * 260f, 0f);
            bool shooting = ArrowTimer > ArrowRate - 20f;
            if (!shooting || CalamityWorld.revenge)
                Walk(goal, speed);
            else
                NPC.velocity.X *= 0.85f;
            NPC.spriteDirection = NPC.direction = Math.Sign(Target.Center.X - NPC.Center.X);

            if (++ArrowTimer >= ArrowRate)
            {
                ArrowTimer = 0f;
                SoundEngine.PlaySound(SoundID.Item5, NPC.Center);
                Vector2 v = CalamityUtils.CalculatePredictiveAimToTarget(NPC.Center, Target, 11f);
                int dmg = ProjDamage(20, 28, 34, 40);
                if (Main.expertMode)
                    Shoot(NPC.Center, v, ModContent.ProjectileType<WulfrumArrowHostile>(), dmg);
                else
                    Shoot(NPC.Center, v, ProjectileID.WoodenArrowHostile, dmg);
            }

            if (Phase >= 1 && ++SpecialTimer >= (CalamityWorld.revenge ? 150f : 210f))
            {
                SpecialTimer = 0f;
                bool blunder = Phase >= 2 && Main.rand.NextBool();
                Vector2 dir = NPC.SafeDirectionTo(Target.Center);
                if (blunder)
                {
                    // Wulfrum Blunderbuss : gerbe de 5 plombs
                    SoundEngine.PlaySound(SoundID.Item36, NPC.Center);
                    for (int i = 0; i < 5; i++)
                        Shoot(NPC.Center, dir.RotatedByRandom(0.3f) * Main.rand.NextFloat(8f, 11f), ModContent.ProjectileType<BlunderPellet>(), ProjDamage(5, 8, 16, 20));
                }
                else
                {
                    // Éventail de Wulfrum Knives (Shock Cutters à 25 %)
                    SoundEngine.PlaySound(SoundID.Item1, NPC.Center);
                    int type = Phase >= 3 ? ModContent.ProjectileType<InventorCutter>() : ModContent.ProjectileType<InventorKnife>();
                    int dmg = Phase >= 3 ? ProjDamage(10, 16, 20, 25) : ProjDamage(5, 8, 16, 20);
                    for (int i = -2; i <= 2; i++)
                        Shoot(NPC.Center, dir.RotatedBy(0.18f * i) * 9f, type, dmg);
                }
            }

            // Épée si le joueur est tout près (à partir de 50 %)
            if (Phase >= 2 && --SwordTimer <= 0f && Vector2.Distance(Target.Center, NPC.Center) < 90f)
            {
                SwordTimer = 60f;
                SoundEngine.PlaySound(SoundID.Item1 with { Pitch = -0.2f }, NPC.Center);
                Shoot(NPC.Center + new Vector2(NPC.direction * 30f, 0f), Vector2.Zero, ModContent.ProjectileType<JaneSwordSwipe>(), ProjDamage(20, 28, 34, 40), NPC.whoAmI);
            }
        }

        private float ArrowRate => (CalamityWorld.revenge ? 45f : Main.expertMode ? 60f : 75f) / (CalamityWorld.death ? 1.3f : 1f);
    }

    /// <summary>« Downed » des Inventors, sauvegardé avec le monde.</summary>
    public class InventorsDowned : ModSystem
    {
        public static bool Downed;

        public override void ClearWorld() => Downed = false;

        public override void SaveWorldData(TagCompound tag)
        {
            if (Downed)
                tag["downedInventors"] = true;
        }

        public override void LoadWorldData(TagCompound tag) => Downed = tag.ContainsKey("downedInventors");

        public override void NetSend(BinaryWriter writer) => writer.Write(Downed);

        public override void NetReceive(BinaryReader reader) => Downed = reader.ReadBoolean();
    }

    /// <summary>Sac des Inventors (page Treasure Bag / butin de la page) : Metabolic Converter en Revengeance.</summary>
    public class InventorsBag : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 3;
            ItemID.Sets.BossBag[Type] = true;
            ItemID.Sets.PreHardmodeLikeBossBag[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.width = Item.height = 24;
            Item.rare = ItemRarityID.Expert;
            Item.expert = true;
        }

        public override bool CanRightClick() => true;

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<JohnWulfrum>()));
            var all = new LeadingConditionRule(new AlwaysCondition());
            AddContents(all);
            itemLoot.Add(all);
            var rev = new LeadingConditionRule(new RevengeanceCondition());
            rev.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Items.PermanentBoosters.MetabolicConverter>()));
            itemLoot.Add(rev);
        }

        /// <summary>
        /// Contenu commun au sac et au butin normal. [choix] Les noms « Egbert » de la page (vandalisme) sont remplacés par
        /// les objets Wulfrum qu'ils déforment : Metal Scrap, Energy Core, Energy Orb, et une arme parmi Blunderbuss,
        /// Prosthesis, Shock Cutter (les Egbert Grappling Gun et Fusion Array n'existent pas en jeu).
        /// </summary>
        public static void AddContents(IItemDropRule rule)
        {
            rule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<CalamityMod.Items.Materials.WulfrumMetalScrap>(), 1, 16, 26));
            rule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<CalamityMod.Items.Materials.EnergyCore>(), 1, 1, 2));
            // Les tables de butin se remplissent avant les recettes : types directs (Energy Orb = matériau généré du wiki,
            // Conquest Fragment = Pearl Shard renommé)
            if (ModContent.TryFind<ModItem>("CalamityFandom", "EnergyOrb", out var orb))
                rule.OnSuccess(ItemDropRule.Common(orb.Type));
            rule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<CalamityMod.Items.Materials.PearlShard>()));
            rule.OnSuccess(ItemDropRule.OneFromOptions(1, ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.WulfrumBlunderbuss>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Magic.WulfrumProsthesis>(), ModContent.ItemType<Items.Weapons.Rogue.WulfrumShockCutter>()));
        }
    }

    public class TamedCondition : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info) => TamingSystem.TamingNow;
        public bool CanShowItemDropInUI() => true;
        public string GetConditionDescription() => Language.GetTextValue("Mods.CalamityFandom.Inventors.TamedCondition");
    }

    public class AlwaysCondition : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info) => true;
        public bool CanShowItemDropInUI() => true;
        public string GetConditionDescription() => null;
    }

    public class RevengeanceCondition : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info) => CalamityWorld.revenge;
        public bool CanShowItemDropInUI() => true;
        public string GetConditionDescription() => Language.GetTextValue("Mods.CalamityFandom.Inventors.RevCondition");
    }

    /// <summary>Wulfrum Bolt de John (texture de l'énergie Wulfrum de Calamity).</summary>
    public class InventorBolt : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Summon/WulfrumEnergyBurst";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Lighting.AddLight(Projectile.Center, 0.3f, 0.6f, 0.2f);
            if (Main.rand.NextBool(3))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, 0f, 0f, 100, default, 0.6f).noGravity = true;
        }
    }

    /// <summary>Wulfrum Arrow de Jane (Expert) : éclate en 3 éclats à l'impact.</summary>
    public class WulfrumArrowHostile : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.WoodenArrowHostile;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.08f, 14f);
        }

        public override Color? GetAlpha(Color lightColor) => new Color(170, 255, 120, 255).MultiplyRGB(lightColor);

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10 with { Volume = 0.5f }, Projectile.Center);
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            for (int i = 0; i < 3; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2CircularEdge(5f, 5f) - Vector2.UnitY * 2f,
                    ModContent.ProjectileType<WulfrumShardHostile>(), Math.Max(1, Projectile.damage / 4), 0f, Main.myPlayer);
        }
    }

    /// <summary>Éclat Wulfrum (des flèches) et plomb du Blunderbuss.</summary>
    public class WulfrumShardHostile : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.CrystalShard;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 60;
            Projectile.scale = 0.7f;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.2f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(150, 220, 90, 220);
    }

    public class BlunderPellet : WulfrumShardHostile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.Bullet;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.timeLeft = 90;
            Projectile.scale = 1f;
        }

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        public override Color? GetAlpha(Color lightColor) => null;
    }

    /// <summary>Wulfrum Knife lancé par Jane (texture du Wulfrum Knife de Calamity).</summary>
    public class InventorKnife : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/WulfrumKnife";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.12f, 14f);
        }
    }

    /// <summary>Wulfrum Shock Cutter lancé par Jane (à 25 %).</summary>
    public class InventorCutter : InventorKnife
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Rogue/WulfrumShockCutter";

        public override void AI()
        {
            Projectile.rotation += 0.4f;
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.1f, 14f);
            if (Main.rand.NextBool(4))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, 0f, 0f, 100, default, 0.6f).noGravity = true;
        }
    }

    /// <summary>Coup d'épée Wulfrum de Jane : zone devant elle pendant 12 images (dégâts de défense).</summary>
    public class JaneSwordSwipe : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 50;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 12;
        }

        public override void AI()
        {
            NPC jane = Main.npc[(int)Projectile.ai[0]];
            if (jane.active)
                Projectile.Center = jane.Center + new Vector2(jane.direction * 30f, 0f);
            if (!Main.dedServ)
                for (int i = 0; i < 3; i++)
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GreenTorch, 0f, 0f, 100, default, 1.2f).noGravity = true;
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers) => modifiers.ScalingArmorPenetration += 1f;
    }

    /// <summary>
    /// Taming (page Taming) : « Give the Inventors a Diamond Ring » — la Diamond Ring s'utilise près de John ou de Jane
    /// (à 15 blocs), elle est donnée (consommée) et l'apprivoisement est fait.
    /// </summary>
    public class InventorsDiamondRing : GlobalItem
    {
        public const string Key = "The Inventors";

        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.DiamondRing;

        public override void SetDefaults(Item item)
        {
            item.useStyle = ItemUseStyleID.HoldUp;
            item.useTime = item.useAnimation = 30;
        }

        private static bool NearInventor(Player player)
        {
            foreach (var n in Main.ActiveNPCs)
                if (n.ModNPC is InventorBase && Vector2.Distance(n.Center, player.Center) < 240f)
                    return true;
            return false;
        }

        public override bool CanUseItem(Item item, Player player) => NearInventor(player);

        public override bool? UseItem(Item item, Player player)
        {
            if (player.whoAmI != Main.myPlayer)
                return null;
            item.stack--;
            if (item.stack <= 0)
                item.TurnToAir();
            SoundEngine.PlaySound(SoundID.Item4, player.Center);
            TamingSystem.Report(Key, 1f);
            return true;
        }
    }
}
