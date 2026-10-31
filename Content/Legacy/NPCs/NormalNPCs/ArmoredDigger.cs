// Adapté de Calamity Mod (version de 2022, 3ec8f553ed), © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using System;
using CalamityMod;
using CalamityMod.CalPlayer;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.NPCs.NormalNPCs
{
    /// <summary>
    /// Wiki : Armored Digger.txt — ver mécanique, mini-boss des cavernes après Plantera (retiré de Calamity en 2022).
    /// Stats des infobox du wiki (défense et réduction de dégâts de chaque segment) ; IA de 2022. Butin : infobox du wiki
    /// (WikiLoot : Draedon's Remote, Demonic Bone Ash, Mysterious Circuitry, Dubious Plating ; les 3 armes du groupe n'ont
    /// pas de page). Pas de bannière (celle de 2022 dépend de la table des bannières de Calamity).
    /// </summary>
    public class ArmoredDiggerHead : ModNPC
    {
        public const int BodyCount = 30;
        private bool tailSpawned;

        public override void SetStaticDefaults() =>
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = new NPCID.Sets.NPCBestiaryDrawModifiers { Hide = true };

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.damage = 90;
            NPC.npcSlots = 10f;
            NPC.width = 54;
            NPC.height = 54;
            NPC.defense = 15;
            NPC.DR_NERD(0.15f);
            NPC.lifeMax = 20000;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.value = Item.buyPrice(gold: 5);
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.netAlways = true;
            NPC.Calamity().VulnerableToElectricity = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) =>
            bestiaryEntry.Info.Add(BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns);

        public override void AI()
        {
            bool death = CalamityWorld.death;
            if (NPC.ai[3] > 0f)
                NPC.realLife = (int)NPC.ai[3];
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();
            if (Main.netMode != NetmodeID.MultiplayerClient && !tailSpawned)
            {
                NPC.ai[3] = NPC.whoAmI;
                NPC.realLife = NPC.whoAmI;
                int previous = NPC.whoAmI;
                for (int i = 0; i <= BodyCount; i++)
                {
                    int type = i == BodyCount ? ModContent.NPCType<ArmoredDiggerTail>() : ModContent.NPCType<ArmoredDiggerBody>();
                    int seg = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)(NPC.position.Y + NPC.height), type, NPC.whoAmI);
                    Main.npc[seg].ai[3] = NPC.whoAmI;
                    Main.npc[seg].realLife = NPC.whoAmI;
                    Main.npc[seg].ai[1] = previous;
                    Main.npc[previous].ai[0] = seg;
                    NetMessage.SendData(MessageID.SyncNPC, number: seg);
                    previous = seg;
                }
                tailSpawned = true;
            }

            // Dans les blocs ?
            int x0 = Math.Max(0, (int)(NPC.position.X / 16f) - 1), x1 = Math.Min(Main.maxTilesX, (int)((NPC.position.X + NPC.width) / 16f) + 2);
            int y0 = Math.Max(0, (int)(NPC.position.Y / 16f) - 1), y1 = Math.Min(Main.maxTilesY, (int)((NPC.position.Y + NPC.height) / 16f) + 2);
            bool inTiles = false;
            for (int x = x0; x < x1 && !inTiles; x++)
            {
                for (int y = y0; y < y1; y++)
                {
                    Tile tile = Main.tile[x, y];
                    if (tile.HasUnactuatedTile && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType] && tile.TileFrameY == 0) || tile.LiquidAmount > 64)
                    {
                        var box = new Rectangle(x * 16, y * 16, 16, 16);
                        if (NPC.Hitbox.Intersects(box))
                        {
                            inTiles = true;
                            break;
                        }
                    }
                }
            }
            Player target = Main.player[NPC.target];
            if (!inTiles)
            {
                NPC.localAI[1] = 1f;
                // Sous le joueur et loin de tout joueur : il « creuse » quand même pour remonter
                if (NPC.position.Y > target.position.Y)
                {
                    bool near = false;
                    foreach (var p in Main.ActivePlayers)
                        if (NPC.Hitbox.Intersects(new Rectangle((int)p.position.X - 1000, (int)p.position.Y - 1000, 2000, 2000)))
                            near = true;
                    if (!near)
                        inTiles = true;
                }
            }
            else
                NPC.localAI[1] = 0f;

            float maxSpeed = death ? 13.5f : 10f;
            // Le joueur est mort ou remonté au-dessus des cavernes : il plonge et disparaît dans l'enfer
            if (target.dead || target.position.Y < Main.rockLayer * 16.0)
            {
                inTiles = false;
                NPC.velocity.Y += 1f;
                if (NPC.position.Y > Main.rockLayer * 16.0)
                {
                    NPC.velocity.Y += 1f;
                    maxSpeed = death ? 27f : 20f;
                }
                if (NPC.position.Y > (Main.maxTilesY - 200) * 16)
                    foreach (var n in Main.ActiveNPCs)
                        if (n.type == Type || n.type == ModContent.NPCType<ArmoredDiggerBody>() || n.type == ModContent.NPCType<ArmoredDiggerTail>())
                            n.active = false;
            }
            float accel = death ? 0.0675f : 0.05f;
            float turn = death ? 0.10125f : 0.075f;
            Vector2 center = new Vector2((int)(NPC.Center.X / 16f) * 16, (int)(NPC.Center.Y / 16f) * 16);
            float dx = (int)(target.Center.X / 16f) * 16 - center.X;
            float dy = (int)(target.Center.Y / 16f) * 16 - center.Y;
            float dist = (float)Math.Sqrt(dx * dx + dy * dy);
            if (!inTiles)
            {
                NPC.TargetClosest();
                NPC.velocity.Y += 0.15f;
                if (NPC.velocity.Y > maxSpeed)
                    NPC.velocity.Y = maxSpeed;
                if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < maxSpeed * 0.4f)
                    NPC.velocity.X += NPC.velocity.X < 0f ? -accel * 1.1f : accel * 1.1f;
                else if (NPC.velocity.Y == maxSpeed)
                    NPC.velocity.X += NPC.velocity.X < dx ? accel : NPC.velocity.X > dx ? -accel : 0f;
                else if (NPC.velocity.Y > 4f)
                    NPC.velocity.X += NPC.velocity.X < 0f ? accel * 0.9f : -accel * 0.9f;
            }
            else
            {
                if (NPC.soundDelay == 0)
                {
                    NPC.soundDelay = (int)MathHelper.Clamp(dist / 40f, 10f, 20f);
                    SoundEngine.PlaySound(SoundID.WormDig, NPC.position);
                }
                float absX = Math.Abs(dx), absY = Math.Abs(dy);
                float k = maxSpeed / dist;
                dx *= k;
                dy *= k;
                if ((NPC.velocity.X > 0f && dx > 0f || NPC.velocity.X < 0f && dx < 0f) && (NPC.velocity.Y > 0f && dy > 0f || NPC.velocity.Y < 0f && dy < 0f))
                {
                    NPC.velocity.X += NPC.velocity.X < dx ? turn : NPC.velocity.X > dx ? -turn : 0f;
                    NPC.velocity.Y += NPC.velocity.Y < dy ? turn : NPC.velocity.Y > dy ? -turn : 0f;
                }
                if (NPC.velocity.X > 0f && dx > 0f || NPC.velocity.X < 0f && dx < 0f || NPC.velocity.Y > 0f && dy > 0f || NPC.velocity.Y < 0f && dy < 0f)
                {
                    NPC.velocity.X += NPC.velocity.X < dx ? accel : NPC.velocity.X > dx ? -accel : 0f;
                    NPC.velocity.Y += NPC.velocity.Y < dy ? accel : NPC.velocity.Y > dy ? -accel : 0f;
                    if (Math.Abs(dy) < maxSpeed * 0.2f && (NPC.velocity.X > 0f && dx < 0f || NPC.velocity.X < 0f && dx > 0f))
                        NPC.velocity.Y += NPC.velocity.Y > 0f ? accel * 2f : -accel * 2f;
                    if (Math.Abs(dx) < maxSpeed * 0.2f && (NPC.velocity.Y > 0f && dy < 0f || NPC.velocity.Y < 0f && dy > 0f))
                        NPC.velocity.X += NPC.velocity.X > 0f ? accel * 2f : -accel * 2f;
                }
                else if (absX > absY)
                {
                    NPC.velocity.X += NPC.velocity.X < dx ? accel * 1.1f : NPC.velocity.X > dx ? -accel * 1.1f : 0f;
                    if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < maxSpeed * 0.5f)
                        NPC.velocity.Y += NPC.velocity.Y > 0f ? accel : -accel;
                }
                else
                {
                    NPC.velocity.Y += NPC.velocity.Y < dy ? accel * 1.1f : NPC.velocity.Y > dy ? -accel * 1.1f : 0f;
                    if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < maxSpeed * 0.5f)
                        NPC.velocity.X += NPC.velocity.X > 0f ? accel : -accel;
                }
            }
            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + MathHelper.PiOver2;
            float wasInTiles = NPC.localAI[0];
            NPC.localAI[0] = inTiles ? 1f : 0f;
            if (wasInTiles != NPC.localAI[0])
                NPC.netUpdate = true;
            if ((NPC.velocity.X > 0f && NPC.oldVelocity.X < 0f || NPC.velocity.X < 0f && NPC.oldVelocity.X > 0f
                || NPC.velocity.Y > 0f && NPC.oldVelocity.Y < 0f || NPC.velocity.Y < 0f && NPC.oldVelocity.Y > 0f) && !NPC.justHit)
                NPC.netUpdate = true;
        }

        public override bool CheckActive() => false;

        public override void HitEffect(NPC.HitInfo hit) => ArmoredDiggerSegment.Dusts(NPC);

        // Le butin tombe du segment le plus proche du joueur, comme en 2022
        public override bool SpecialOnKill()
        {
            int closest = DropHelper.FindClosestWormSegment(NPC, Type, ModContent.NPCType<ArmoredDiggerBody>(), ModContent.NPCType<ArmoredDiggerTail>());
            NPC.position = Main.npc[closest].position;
            return false;
        }
    }

    /// <summary>Segment qui suit celui qui le précède (ai[1]), vie partagée avec la tête.</summary>
    public abstract class ArmoredDiggerSegment : ModNPC
    {
        public static void Dusts(NPC npc)
        {
            for (int k = 0; k < (npc.life <= 0 ? 13 : 3); k++)
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Torch);
        }

        public override void SetStaticDefaults() =>
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = new NPCID.Sets.NPCBestiaryDrawModifiers { Hide = true };

        public override void SetDefaults()
        {
            NPC.lifeMax = 20000;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.netAlways = true;
            NPC.dontCountMe = true;
            NPC.Calamity().VulnerableToElectricity = true;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;

        public override bool CheckActive() => false;

        public override void HitEffect(NPC.HitInfo hit) => Dusts(NPC);

        /// <summary>Le corps lance parfois des débris (Saucer Scrap) sur le joueur.</summary>
        protected virtual void Shoot() { }

        public override void AI()
        {
            if (NPC.ai[3] > 0f)
                NPC.realLife = (int)NPC.ai[3];
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();
            NPC ahead = NPC.ai[1] > 0f && NPC.ai[1] < Main.maxNPCs ? Main.npc[(int)NPC.ai[1]] : null;
            if (ahead == null || !ahead.active || ahead.life <= 0)
            {
                NPC.life = 0;
                NPC.HitEffect();
                NPC.checkDead();
                return;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
                Shoot();
            Vector2 toAhead = ahead.Center - NPC.Center;
            NPC.rotation = (float)Math.Atan2(toAhead.Y, toAhead.X) + MathHelper.PiOver2;
            float dist = toAhead.Length();
            int spacing = (int)(44f * NPC.scale);
            if (dist > 0f)
                NPC.position += toAhead * ((dist - spacing) / dist);
            NPC.velocity = Vector2.Zero;
        }
    }

    public class ArmoredDiggerBody : ArmoredDiggerSegment
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.damage = 70;
            NPC.width = 38;
            NPC.height = 38;
            NPC.defense = 40;
            NPC.DR_NERD(0.25f);
        }

        protected override void Shoot()
        {
            NPC.localAI[0] += Main.rand.Next(4);
            if (NPC.localAI[0] < Main.rand.Next(1800, 26000))
                return;
            NPC.localAI[0] = 0f;
            NPC.TargetClosest();
            Player target = Main.player[NPC.target];
            if (!Collision.CanHit(NPC.position, NPC.width, NPC.height, target.position, target.width, target.height))
                return;
            Vector2 aim = target.Center + new Vector2(Main.rand.Next(-20, 21), Main.rand.Next(-20, 21)) - NPC.Center;
            aim = aim.SafeNormalize(Vector2.UnitY) * 7f;
            // 30 x2 (projectiles hostiles) : 60 / 120 en Expert, comme l'infobox
            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + aim * 5f, aim, ProjectileID.SaucerScrap, 30, 0f, Main.myPlayer);
            NPC.netUpdate = true;
        }
    }

    public class ArmoredDiggerTail : ArmoredDiggerSegment
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.damage = 55;
            NPC.width = 34;
            NPC.height = 34;
            NPC.defense = 55;
            // Réduction de dégâts de la queue absente de l'infobox : celle du code de 2022
            NPC.DR_NERD(0.3f);
        }
    }

    /// <summary>
    /// Apparition (code de 2022, réglages de la page : « Notes ») : dans les cavernes après Plantera, hors donjon, temple,
    /// jungle, Sunken Sea, Abyss et enfer, sans boss et avec moins de 3 PNJ de ville proches ; 1 chance sur 100 000 par
    /// tick et par joueur, modifiée par les bougies et potions de la page ; puis 60 s sans nouvelle apparition.
    /// </summary>
    public class ArmoredDiggerSpawn : ModSystem
    {
        public const int Cooldown = 60 * 60;
        private static int cooldown;

        public override void ClearWorld() => cooldown = 0;

        public override void PostUpdateWorld()
        {
            if (cooldown > 0)
            {
                cooldown--;
                return;
            }
            if (Main.netMode == NetmodeID.MultiplayerClient || !NPC.downedPlantBoss || NPC.AnyNPCs(ModContent.NPCType<ArmoredDiggerHead>()))
                return;
            foreach (var player in Main.ActivePlayers)
            {
                var cal = player.Calamity();
                if (player.dead || !player.ZoneRockLayerHeight || player.ZoneUnderworldHeight || player.ZoneDungeon || player.ZoneJungle
                    || player.ZoneLihzhardTemple || cal.ZoneSunkenSea || cal.ZoneAbyss || CalamityPlayer.areThereAnyDamnBosses || player.townNPCs >= 3f)
                    continue;
                double rate = 100000;
                if (CalamityWorld.revenge)
                    rate /= 1.18;
                if (CalamityWorld.death)
                    rate /= 1.33;
                if (CalamityWorld.death && Main.bloodMoon)
                    rate /= 5;
                if (cal.zerg)
                    rate /= 2;
                if (cal.chaosCandle)
                    rate /= 1.67;
                if (player.enemySpawns)
                    rate /= 1.43;
                if (Main.SceneMetrics.WaterCandleCount > 0)
                    rate /= 1.25;
                if (cal.isNearbyBoss && CalamityMod.CalamityServerConfig.Instance.BossZen)
                    rate /= 0.02;
                if (cal.zen)
                    rate /= 0.5;
                if (cal.tranquilityCandle)
                    rate /= 0.6;
                if (player.calmed)
                    rate /= 0.7;
                if (Main.SceneMetrics.PeaceCandleCount > 0)
                    rate /= 0.8;
                if (!Main.rand.NextBool((int)rate))
                    continue;
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<ArmoredDiggerHead>());
                cooldown = Cooldown;
                return;
            }
        }
    }
}
