using System;
using System.IO;
using CalamityFandom.Common.Taming;
using CalamityMod;
using CalamityMod.Items.SummonItems;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Dv = CalamityFandom.Content.NPCs.Inventors.InventorBase;

namespace CalamityFandom.Content.NPCs.GrandThunderCnidrion
{
    /// <summary>
    /// Wiki : Grand Thunder Cnidrion.txt — boss pré-Hardmode du désert, alternative au Desert Scourge, invoqué par le Desert
    /// Medallion le jour (<see cref="DesertMedallionByDay"/>). Plane quelques blocs au-dessus du joueur en allant de gauche à
    /// droite (plus vite sous 45 %) ; attaques : éclairs du ciel, charge horizontale (il tremble avant), 3 Storm Cnidrilings,
    /// et sous 45 % des salves de 3 tirs ; sans ligne de vue, des traits électriques qui traversent les blocs ; en Expert,
    /// des éclairs qui font naître des tornades au sol. Butin par WikiLoot (infobox).
    /// [technique] Le nom de l'infobox (« name = ... ») et une anecdote des « Notes » sont du vandalisme offensant visant une
    /// personne réelle : ignorés, le nom affiché est le titre de la page.
    /// </summary>
    [AutoloadBossHead]
    public class GrandThunderCnidrion : ModNPC
    {
        public const string TamingKey = "Grand Thunder Cnidrion";

        private ref float Attack => ref NPC.ai[1];
        private ref float Timer => ref NPC.ai[2];
        private ref float Side => ref NPC.ai[3];

        private int losTimer;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

        public override void SetDefaults()
        {
            NPC.boss = true;
            NPC.width = 150;
            NPC.height = 80;
            NPC.lifeMax = 1750;
            NPC.damage = 50;
            NPC.defense = 6;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit12;
            NPC.DeathSound = SoundID.NPCDeath18;
            NPC.value = Item.buyPrice(silver: 50) / 5;
            NPC.npcSlots = 5f;
            NPC.Calamity().DR = 0.05f;
            NPC.Calamity().VulnerableToWater = true;
            if (!Main.dedServ)
                Music = MusicID.Boss1;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(Dv.Dv(1750, 2300, 2800, 2800) * balance);
            NPC.damage = Dv.Dv(50, 70, 90, 90);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) =>
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Desert,
                new FlavorTextBestiaryInfoElement("Mods.CalamityFandom.NPCs.GrandThunderCnidrion.Bestiary"),
            });

        public override void SendExtraAI(BinaryWriter writer) => writer.Write(losTimer);

        public override void ReceiveExtraAI(BinaryReader reader) => losTimer = reader.ReadInt32();

        private Player Target => Main.player[NPC.target];
        private bool Server => Main.netMode != NetmodeID.MultiplayerClient;
        private bool Enraged => NPC.life < NPC.lifeMax * 0.45f;

        public static int BarrageDamage => Dv.ProjDamage(24, 48, 72, 72);
        public static int ShotDamage => Dv.ProjDamage(30, 60, 90, 90);

        public override void AI()
        {
            NPC.TargetClosest(false);
            if (Target.dead || !Target.active || NPC.Distance(Target.Center) > 4000f)
            {
                NPC.velocity.Y -= 0.3f;
                NPC.EncourageDespawn(60);
                return;
            }
            Timer++;
            // [choix] Sans ligne de vue : un trait électrique qui traverse les blocs toutes les 60 images
            if (!Collision.CanHitLine(NPC.Center, 1, 1, Target.Center, 1, 1))
            {
                if (++losTimer % 60 == 0 && Server)
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.SafeDirectionTo(Target.Center) * 14f, ModContent.ProjectileType<ElectricStreak>(), ShotDamage * 2, 0f, Main.myPlayer);
            }
            switch ((int)Attack)
            {
                case 0: // Éclairs du ciel, avec un cri
                    Hover();
                    if (Timer == 1f)
                        SoundEngine.PlaySound(SoundID.Item122 with { Pitch = 0.4f }, NPC.Center);
                    if (Timer % 10 == 0f && Timer < 120f && Server)
                    {
                        Vector2 at = Target.Center + new Vector2(Main.rand.Next(-500, 501), -600f);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), at, new Vector2(0f, 12f), ModContent.ProjectileType<LightningBolt>(), BarrageDamage, 0f, Main.myPlayer, Main.expertMode ? 1f : 0f);
                    }
                    if (Timer > 150f)
                        Next();
                    break;
                case 1: // Charge horizontale : se met à la hauteur du joueur, tremble, fonce
                    if (Timer < 70f)
                    {
                        float side = Math.Sign(NPC.Center.X - Target.Center.X);
                        if (side == 0f)
                            side = 1f;
                        Fly(Target.Center + new Vector2(side * 450f, 0f), 12f);
                        if (Timer > 40f)
                            NPC.position += Main.rand.NextVector2Circular(3f, 3f);
                    }
                    else if (Timer == 70f)
                    {
                        NPC.velocity = new Vector2(Math.Sign(Target.Center.X - NPC.Center.X) * (Enraged ? 22f : 18f), 0f);
                        SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                    }
                    else if (Timer > 120f)
                        NPC.velocity *= 0.92f;
                    if (Timer > 150f)
                        Next();
                    break;
                case 2: // 3 Storm Cnidrilings
                    Hover();
                    if (Timer == 30f && Server)
                        for (int i = 0; i < 3; i++)
                        {
                            int n = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y + 30, ModContent.NPCType<StormCnidriling>());
                            if (Main.npc.IndexInRange(n))
                                Main.npc[n].velocity = new Vector2(Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-3f, 1f));
                        }
                    if (Timer > 90f)
                        Next();
                    break;
                default: // Sous 45 % : salves de 3 tirs
                    Hover();
                    if (Timer % 30 == 0f && Timer <= 90f)
                    {
                        SoundEngine.PlaySound(SoundID.Item21, NPC.Center);
                        if (Server)
                            for (int i = -1; i <= 1; i++)
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.SafeDirectionTo(Target.Center).RotatedBy(0.15f * i) * 10f,
                                    ModContent.ProjectileType<LightningShot>(), ShotDamage, 0f, Main.myPlayer);
                    }
                    if (Timer > 120f)
                        Next();
                    break;
            }
            NPC.spriteDirection = NPC.velocity.X > 0.5f ? 1 : NPC.velocity.X < -0.5f ? -1 : NPC.spriteDirection;
            NPC.rotation = NPC.velocity.X * 0.02f;
        }

        private void Next()
        {
            int count = Enraged ? 4 : 3;
            Attack = (Attack + 1) % count;
            Timer = 0f;
            NPC.netUpdate = true;
        }

        /// <summary>Plane quelques blocs au-dessus du joueur en allant d'un côté à l'autre (plus vite sous 45 %).</summary>
        private void Hover()
        {
            if (Side == 0f)
                Side = 1f;
            Vector2 dest = Target.Center + new Vector2(Side * 350f, -220f);
            Fly(dest, Enraged ? 11f : 8f);
            if (NPC.Distance(dest) < 60f)
            {
                Side = -Side;
                NPC.netUpdate = true;
            }
        }

        private void Fly(Vector2 dest, float speed)
        {
            Vector2 want = (dest - NPC.Center).SafeNormalize(Vector2.Zero) * Math.Min(speed, NPC.Distance(dest) / 10f + 1f);
            NPC.velocity = (NPC.velocity * 14f + want) / 15f;
        }

        public override void OnKill()
        {
            if (!TamingSystem.TamingNow)
                ThunderCnidrionSystem.Downed = true;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);
        }

        public override void BossLoot(ref string name, ref int potionType) => potionType = ItemID.LesserHealingPotion;

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (!Main.dedServ)
                for (int i = 0; i < (NPC.life <= 0 ? 40 : 4); i++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Electric, hit.HitDirection, -1f, 100, default, 1.1f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = TextureAssets.Npc[Type].Value;
            SpriteEffects fx = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(tex, NPC.Center - screenPos, null, NPC.GetAlpha(drawColor), NPC.rotation, tex.Size() / 2f, NPC.scale, fx, 0f);
            return false;
        }
    }

    /// <summary>[choix] Storm Cnidriling (pas de page) : petit Cnidrion volant qui fonce sur le joueur ; vie 60, dégâts 20.</summary>
    public class StormCnidriling : ModNPC
    {
        public override void SetDefaults()
        {
            NPC.width = 70;
            NPC.height = 40;
            NPC.lifeMax = 60;
            NPC.damage = 20;
            NPC.defense = 2;
            NPC.knockBackResist = 0.6f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit12;
            NPC.DeathSound = SoundID.NPCDeath18;
        }

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player p = Main.player[NPC.target];
            NPC.velocity = (NPC.velocity * 40f + NPC.SafeDirectionTo(p.Center) * 7f) / 41f;
            NPC.spriteDirection = NPC.velocity.X > 0 ? 1 : -1;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = TextureAssets.Npc[Type].Value;
            SpriteEffects fx = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(tex, NPC.Center - screenPos, null, NPC.GetAlpha(drawColor), 0f, tex.Size() / 2f, NPC.scale, fx, 0f);
            return false;
        }
    }

    public class ThunderCnidrionSystem : ModSystem
    {
        public static bool Downed;

        public override void ClearWorld() => Downed = false;

        public override void SaveWorldData(TagCompound tag)
        {
            if (Downed)
                tag["downedGrandThunderCnidrion"] = true;
        }

        public override void LoadWorldData(TagCompound tag) => Downed = tag.ContainsKey("downedGrandThunderCnidrion");

        public override void NetSend(BinaryWriter writer) => writer.Write(Downed);

        public override void NetReceive(BinaryReader reader) => Downed = reader.ReadBoolean();
    }

    /// <summary>
    /// Desert Medallion (page) : « summon the Grand Thunder Cnidrion during the day and the Desert Scourge at night ». Le jour,
    /// le médaillon de Calamity est intercepté : le Grand Thunder Cnidrion apparaît à la place du Desert Scourge.
    /// [choix] Dans les deux modes (au plus près du wiki).
    /// </summary>
    public class DesertMedallionByDay : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ModContent.ItemType<DesertMedallion>();

        public override bool CanUseItem(Item item, Player player)
        {
            if (!Main.dayTime)
                return true;
            int type = ModContent.NPCType<GrandThunderCnidrion>();
            if (player.whoAmI == Main.myPlayer && player.ZoneDesert && !NPC.AnyNPCs(type) && !NPC.AnyNPCs(ModContent.NPCType<DesertScourgeHead>()))
            {
                SoundEngine.PlaySound(SoundID.Roar, player.Center);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.SpawnOnPlayer(player.whoAmI, type);
                else
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
            }
            return false;
        }
    }

    /// <summary>Éclair qui tombe du ciel ; ai[0] = 1 (Expert) : fait naître deux tornades au sol.</summary>
    public class LightningBolt : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            // Traverse les blocs du haut, s'arrête au premier sol sous le joueur visé
            if (Projectile.timeLeft < 150)
                Projectile.tileCollide = true;
            if (!Main.dedServ)
                for (int i = 0; i < 2; i++)
                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(4f, 4f), DustID.Electric, Vector2.Zero, 100, default, 1.2f).noGravity = true;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item94 with { Volume = 0.5f }, Projectile.Center);
            if (Projectile.ai[0] != 1f || Main.netMode == NetmodeID.MultiplayerClient)
                return;
            for (int i = -1; i <= 1; i += 2)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center - new Vector2(0f, 30f), new Vector2(i * 4f, 0f), ModContent.ProjectileType<SandTornado>(),
                    Projectile.damage, 0f, Main.myPlayer);
        }
    }

    /// <summary>Tir d'éclair (salves de 3).</summary>
    public class LightningShot : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.Electric, Vector2.Zero, 100, default, 1.3f).noGravity = true;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.Electrified, 60);
    }

    /// <summary>Trait électrique quand le joueur se cache : traverse les blocs, fait mal.</summary>
    public class ElectricStreak : LightningShot
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 2;
        }
    }

    /// <summary>Tornade (Expert) : file à l'horizontale le long du sol pendant 3 s.</summary>
    public class SandTornado : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.SandnadoHostile;

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 80;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Projectile.rotation = 0f;
            if (!Main.dedServ)
                for (int i = 0; i < 2; i++)
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Sand, 0f, -2f, 100, default, 1.2f).noGravity = true;
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }
}
