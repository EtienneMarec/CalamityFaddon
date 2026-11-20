using System;
using System.IO;
using System.Linq;
using CalamityFandom.Common.Taming;
using CalamityMod;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Dv = CalamityFandom.Content.NPCs.Inventors.InventorBase;

// Lot BE (20/11) : Polyphemalus, boss Hardmode à quatre yeux (page Polyphemalus), son objet d'invocation et son butin.

namespace CalamityFandom.Content.NPCs.Polyphemalus
{
    /// <summary>
    /// Wiki : Polyphemalus.txt — boss Hardmode (Fused Eye la nuit, ou comme l'Eye of Cthulhu). Phase 1 : les quatre yeux
    /// soudés attaquent tour à tour (lasers rouges prédictifs, gerbes de glace, flèches en arc, boules d'ombre chercheuses),
    /// puis 4 charges (la dernière prédictive, avec une explosion de lasers et de glace), puis un « bullet hell » (en orbite
    /// en tirant vers le joueur, ou en tournant sur place dans toutes les directions). Vide, il se sépare en quatre yeux
    /// (Astigmageddon, Exotrexia, Conjunctivirus, Cataractacomb) ; le dernier tué donne son objet. Mode défi : avec le Shield
    /// of Cthulhu comme ruée, ses coups font 1000 (critique) ; Polyphemalus ne se sépare pas et donne le Polypebral Shield ;
    /// le défi s'arrête si on le blesse autrement. Pas de dégâts de contact juste après son apparition. 35 % de réduction,
    /// 10 défense (80 furieux). Cursed Inferno 4 s au contact.
    /// [choix] Furieux le jour (défense 80, 50 % plus rapide) ; « ? » de l'infobox en Expert et plus : vie et dégâts du mode
    /// normal (la vie suit l'échelle de difficulté du jeu).
    /// </summary>
    [AutoloadBossHead]
    public class Polyphemalus : ModNPC
    {
        public const string TamingKey = "Polyphemalus";

        private ref float Attack => ref NPC.ai[1];
        private ref float Timer => ref NPC.ai[2];
        private ref float Counter => ref NPC.ai[3];

        public bool Challenge;
        private bool started;
        private Vector2 dash;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

        public override void SetDefaults()
        {
            NPC.boss = true;
            NPC.width = 200;
            NPC.height = 200;
            NPC.lifeMax = 50000;
            NPC.damage = 100;
            NPC.defense = 10;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = Item.buyPrice(gold: 25);
            NPC.npcSlots = 10f;
            NPC.Calamity().DR = 0.35f;
            if (!Main.dedServ)
                Music = MusicID.Boss3;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment) => NPC.lifeMax = (int)(50000 * balance);

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Challenge);
            writer.Write(started);
            writer.WriteVector2(dash);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Challenge = reader.ReadBoolean();
            started = reader.ReadBoolean();
            dash = reader.ReadVector2();
        }

        private Player Target => Main.player[NPC.target];
        private bool Server => Main.netMode != NetmodeID.MultiplayerClient;
        private float Speed => Main.dayTime ? 1.5f : 1f;

        public static int ProjDamage => Dv.ProjDamage(100, 120, 140, 160) / 2;

        public override void AI()
        {
            NPC.TargetClosest();
            if (!started)
            {
                started = true;
                // Mode défi : le Shield of Cthulhu est la ruée active
                Challenge = Target.dashType == 2;
                NPC.localAI[0] = 120;
                NPC.netUpdate = true;
            }
            if (NPC.localAI[0] > 0)
                NPC.localAI[0]--;
            NPC.defense = Main.dayTime ? 80 : 10;
            if (!Target.active || Target.dead)
            {
                NPC.velocity.Y -= 0.4f;
                if (NPC.timeLeft > 60)
                    NPC.timeLeft = 60;
                return;
            }
            Lighting.AddLight(NPC.Center, 0.6f, 0.2f, 0.2f);
            Timer++;
            switch ((int)Attack)
            {
                case 0: Shoot(15, 4, () => Lasers()); break;
                case 1: Shoot(30, 3, () => Ice()); break;
                case 2: Shoot(20, 3, () => Arrows()); break;
                case 3: Shoot(40, 3, () => Shadowballs()); break;
                case 4: Charges(); break;
                default: BulletHell(); break;
            }
            NPC.rotation = NPC.velocity.X * 0.02f;
        }

        private void Hover(float height = -280f)
        {
            Vector2 spot = Target.Center + new Vector2(0f, height);
            NPC.velocity = Vector2.Lerp(NPC.velocity, (spot - NPC.Center) * 0.05f * Speed, 0.1f);
        }

        private void Next()
        {
            Attack = (Attack + 1) % 6;
            Timer = Counter = 0;
            NPC.netUpdate = true;
        }

        /// <summary>Attaque à tirs répétés : une salve toutes les <paramref name="every"/> images, <paramref name="times"/> fois.</summary>
        private void Shoot(int every, int times, Action salvo)
        {
            Hover();
            if (Timer % (int)(every / Speed) == 0 && Server)
            {
                salvo();
                if (++Counter >= times)
                    Next();
            }
        }

        private Vector2 Aim(Vector2 from, float speed, bool predict)
        {
            Vector2 to = predict ? Target.Center + Target.velocity * 20f : Target.Center;
            return (to - from).SafeNormalize(Vector2.UnitY) * speed;
        }

        private void Lasers()
        {
            for (int i = -1; i <= 1; i++)
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Aim(NPC.Center, 11f, true).RotatedBy(i * 0.12f), ModContent.ProjectileType<PolyLaser>(), ProjDamage, 0f);
            SoundEngine.PlaySound(SoundID.Item33, NPC.Center);
        }

        private void Ice()
        {
            Vector2 dir = Aim(NPC.Center, 9f, false);
            foreach (float a in new[] { -0.15f, 0f, 0.15f, -0.7f, -0.95f, 0.7f, 0.95f })
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, dir.RotatedBy(a), ModContent.ProjectileType<PolyIce>(), ProjDamage, 0f);
            SoundEngine.PlaySound(SoundID.Item28, NPC.Center);
        }

        private void Arrows()
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2 v = Aim(NPC.Center, 9f, false) + new Vector2(Main.rand.NextFloat(-3f, 3f), -6f - Main.rand.NextFloat(3f));
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, v, ProjectileID.WoodenArrowHostile, ProjDamage, 0f);
            }
            SoundEngine.PlaySound(SoundID.Item5, NPC.Center);
        }

        private void Shadowballs()
        {
            for (int i = -2; i <= 2; i++)
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Aim(NPC.Center, 7f, false).RotatedBy(i * 0.3f), ModContent.ProjectileType<PolyShadowball>(), ProjDamage * 3 / 4, 0f);
            SoundEngine.PlaySound(SoundID.Item104, NPC.Center);
        }

        /// <summary>4 charges : 30 images d'élan, 40 de charge ; la dernière vise où va le joueur et finit en explosion.</summary>
        private void Charges()
        {
            int t = (int)Timer;
            if (t <= 30)
            {
                NPC.velocity *= 0.9f;
                NPC.rotation += 0.3f;
                if (t == 30)
                {
                    bool last = Counter >= 3;
                    dash = Aim(NPC.Center, 22f * Speed, last);
                    SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                    NPC.netUpdate = true;
                }
                return;
            }
            NPC.velocity = dash;
            if (t < 70)
                return;
            if (Counter >= 3 && Server)
            {
                for (int i = 0; i < 12; i++)
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, (MathHelper.TwoPi * i / 12f).ToRotationVector2() * 10f, ModContent.ProjectileType<PolyLaser>(), ProjDamage, 0f);
                for (int i = 0; i < 8; i++)
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, (MathHelper.TwoPi * (i + 0.5f) / 8f).ToRotationVector2() * 7f, ModContent.ProjectileType<PolyIce>(), ProjDamage, 0f);
                SoundEngine.PlaySound(SoundID.Item14, NPC.Center);
            }
            if (++Counter >= 4)
                Next();
            else
            {
                Timer = 0;
                NPC.netUpdate = true;
            }
        }

        /// <summary>Bullet hell : en orbite en tirant vers le joueur, ou en tournant sur place (une fois sur deux).</summary>
        private void BulletHell()
        {
            bool orbit = NPC.localAI[1] % 2 == 0;
            if (orbit)
            {
                float angle = Timer * 0.025f;
                Vector2 spot = Target.Center + angle.ToRotationVector2() * 420f;
                NPC.velocity = (spot - NPC.Center) * 0.12f;
                if (Timer % 10 == 0 && Server)
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Aim(NPC.Center, 9f, false), ModContent.ProjectileType<PolyLaser>(), ProjDamage, 0f);
            }
            else
            {
                NPC.velocity *= 0.9f;
                if (Timer % 5 == 0 && Server)
                    for (int i = 0; i < 4; i++)
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, (Timer * 0.08f + MathHelper.PiOver2 * i).ToRotationVector2() * 7f,
                            i % 2 == 0 ? ModContent.ProjectileType<PolyShadowball>() : ModContent.ProjectileType<PolyIce>(), ProjDamage * 3 / 4, 0f);
            }
            if (Timer >= 240)
            {
                NPC.localAI[1]++;
                Next();
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => NPC.localAI[0] <= 0;

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo) => target.AddBuff(BuffID.CursedInferno, 240);

        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            // Défi : les coups du Shield of Cthulhu font 1000, toujours critiques
            if (Challenge && Main.player.Any(p => p.active && p.eocDash > 0 && p.Distance(NPC.Center) < 400f))
            {
                modifiers.SourceDamage *= 1000f / 30f;
                modifiers.SetCrit();
            }
        }

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone) => EndChallenge();

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone) => EndChallenge();

        private void EndChallenge()
        {
            if (!Challenge)
                return;
            Challenge = false;
            NPC.netUpdate = true;
        }

        public override bool CheckDead()
        {
            if (Challenge || TamingSystem.TamingNow)
                return true;
            // Vide : il se sépare en quatre yeux (sans butin)
            if (Server)
            {
                int[] eyes = { ModContent.NPCType<Astigmageddon>(), ModContent.NPCType<Exotrexia>(), ModContent.NPCType<Conjunctivirus>(), ModContent.NPCType<Cataractacomb>() };
                for (int i = 0; i < 4; i++)
                {
                    Vector2 at = NPC.Center + (MathHelper.PiOver2 * i).ToRotationVector2() * 60f;
                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)at.X, (int)at.Y, eyes[i], 0, i, 0f, 0f, 0f, NPC.target);
                }
            }
            SoundEngine.PlaySound(SoundID.NPCDeath1, NPC.Center);
            for (int i = 0; i < 40; i++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-6f, 6f), 0, default, 1.6f);
            NPC.life = 0;
            NPC.active = false;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, NPC.whoAmI);
            return false;
        }

        public override void OnKill()
        {
            // Défi réussi : Polypebral Shield ; apprivoisé : un des quatre objets au hasard [choix]
            if (Challenge)
                Item.NewItem(NPC.GetSource_Loot(), NPC.Hitbox, ModContent.ItemType<Items.Accessories.PolypebralShield>());
            else if (TamingSystem.TamingNow)
                Item.NewItem(NPC.GetSource_Loot(), NPC.Hitbox, Main.rand.Next(PolyEye.Drops()));
            if (!TamingSystem.TamingNow)
                PolyphemalusSystem.Downed = true;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);
        }

        public override void BossLoot(ref string name, ref int potionType) => potionType = ItemID.GreaterHealingPotion;

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (Main.dedServ)
                return;
            for (int i = 0; i < 4; i++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection * 2f, -1f, 100, default, 1.3f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = TextureAssets.Npc[Type].Value;
            spriteBatch.Draw(tex, NPC.Center - screenPos, null, NPC.GetAlpha(drawColor), NPC.rotation, tex.Size() / 2f, NPC.scale * 0.8f, SpriteEffects.None, 0f);
            return false;
        }
    }

    /// <summary>
    /// Œil séparé (phase 2) : d'abord une orbite commune en tirant vers le joueur (3 s), puis son propre cycle. ai[0] = place
    /// dans l'orbite. Pas de dégâts de contact (sauf Conjunctivirus en ruée). Le dernier tué donne son objet et le masque.
    /// </summary>
    public abstract class PolyEye : ModNPC
    {
        protected abstract int Life { get; }
        protected abstract int DropItem { get; }
        protected abstract Color Glow { get; }

        protected ref float Timer => ref NPC.ai[1];

        public static int[] Drops() => new[]
        {
            ModContent.ItemType<Items.Weapons.Ranged.Heterochromia>(), ModContent.ItemType<Items.Weapons.Melee.ChainSaw>(),
            ModContent.ItemType<Items.Weapons.Magic.Nystagmus>(), ModContent.ItemType<Items.Weapons.Summon.Tetrachromancy>(),
        };

        public static bool IsEye(int type) => type == ModContent.NPCType<Astigmageddon>() || type == ModContent.NPCType<Exotrexia>()
            || type == ModContent.NPCType<Conjunctivirus>() || type == ModContent.NPCType<Cataractacomb>();

        public override void SetStaticDefaults()
        {
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

        public override void SetDefaults()
        {
            NPC.boss = true;
            NPC.width = NPC.height = 90;
            NPC.lifeMax = Life;
            NPC.damage = 0;
            NPC.defense = 10;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = Item.buyPrice(gold: 5);
            NPC.Calamity().DR = 0.35f;
            if (!Main.dedServ)
                Music = MusicID.Boss3;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment) => NPC.lifeMax = (int)(Life * balance);

        protected Player Target => Main.player[NPC.target];
        protected bool Server => Main.netMode != NetmodeID.MultiplayerClient;
        protected static int Damage => Polyphemalus.ProjDamage;

        protected void Orbit(float radius, float speed)
        {
            float angle = NPC.ai[0] * MathHelper.PiOver2 + Main.GlobalTimeWrappedHourly * speed * 60f;
            Vector2 spot = Target.Center + angle.ToRotationVector2() * radius;
            NPC.velocity = (spot - NPC.Center) * 0.1f;
        }

        protected void Fire(int type, Vector2 velocity, int damage = -1)
        {
            if (Server)
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, type, damage < 0 ? Damage : damage, 0f);
        }

        protected Vector2 ToTarget(float speed) => (Target.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * speed;

        public override void AI()
        {
            NPC.TargetClosest();
            NPC.defense = Main.dayTime ? 80 : 10;
            if (!Target.active || Target.dead)
            {
                NPC.velocity.Y -= 0.4f;
                if (NPC.timeLeft > 60)
                    NPC.timeLeft = 60;
                return;
            }
            Lighting.AddLight(NPC.Center, Glow.ToVector3() * 0.6f);
            NPC.rotation = (Target.Center - NPC.Center).ToRotation() - MathHelper.PiOver2;
            // Orbite commune du début de la phase 2
            if (++NPC.localAI[0] < 180)
            {
                Orbit(380f, 0.02f);
                if (NPC.localAI[0] % 25 == 0)
                    Fire(ModContent.ProjectileType<PolyLaser>(), ToTarget(8f));
                return;
            }
            Timer = (Timer + 1) % 600;
            Pattern((int)Timer);
        }

        protected abstract void Pattern(int t);

        public override void OnKill()
        {
            if (Main.npc.Any(n => n.active && n.whoAmI != NPC.whoAmI && IsEye(n.type)))
                return;
            // Dernier œil : son objet, le masque de l'infobox, fin du combat
            Item.NewItem(NPC.GetSource_Loot(), NPC.Hitbox, DropItem);
            Item.NewItem(NPC.GetSource_Loot(), NPC.Hitbox, ItemID.EyeMask);
            PolyphemalusSystem.Downed = true;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (Main.dedServ)
                return;
            for (int i = 0; i < (NPC.life <= 0 ? 25 : 3); i++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection * 2f, -1f, 100, default, 1.2f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = TextureAssets.Npc[Type].Value;
            spriteBatch.Draw(tex, NPC.Center - screenPos, null, NPC.GetAlpha(drawColor), NPC.rotation, tex.Size() / 2f, NPC.scale, SpriteEffects.None, 0f);
            return false;
        }
    }

    /// <summary>Astigmageddon (rouge) : orbite et lasers ; puis orbite plus large et plus rapide, plus de lasers ; puis repos.</summary>
    public class Astigmageddon : PolyEye
    {
        protected override int Life => 17000;
        protected override int DropItem => ModContent.ItemType<Items.Weapons.Ranged.Heterochromia>();
        protected override Color Glow => Color.Red;

        protected override void Pattern(int t)
        {
            if (t < 300)
            {
                Orbit(320f, 0.03f);
                if (t % 30 == 0)
                    Fire(ModContent.ProjectileType<PolyLaser>(), ToTarget(10f));
            }
            else if (t < 480)
            {
                Orbit(460f, 0.06f);
                if (t % 12 == 0)
                    Fire(ModContent.ProjectileType<PolyLaser>(), ToTarget(11f));
            }
            else
                NPC.velocity *= 0.95f;
        }
    }

    /// <summary>Exotrexia (bleu) : avance avec beaucoup d'inertie en lançant des vagues de glace ; puis tourne en semant des boules de glace.</summary>
    public class Exotrexia : PolyEye
    {
        protected override int Life => 17000;
        protected override int DropItem => ModContent.ItemType<Items.Weapons.Melee.ChainSaw>();
        protected override Color Glow => Color.Cyan;

        protected override void Pattern(int t)
        {
            if (t < 360)
            {
                NPC.velocity = Vector2.Lerp(NPC.velocity, ToTarget(10f), 0.02f);
                if (t % 40 == 0)
                    Fire(ModContent.ProjectileType<PolyIce>(), ToTarget(9f));
            }
            else
            {
                Orbit(300f, 0.04f);
                if (t % 20 == 0)
                    Fire(ModContent.ProjectileType<FrostMine>(), Vector2.Zero, Damage / 3);
            }
        }
    }

    /// <summary>Conjunctivirus (vert) : flammes maudites en s'approchant lentement ; puis ruées répétées (seul contact des yeux).</summary>
    public class Conjunctivirus : PolyEye
    {
        protected override int Life => 17000;
        protected override int DropItem => ModContent.ItemType<Items.Weapons.Magic.Nystagmus>();
        protected override Color Glow => Color.LimeGreen;

        private bool Dashing => Timer >= 360;

        protected override void Pattern(int t)
        {
            if (t < 360)
            {
                NPC.velocity = ToTarget(2.5f);
                if (t % 5 == 0)
                    Fire(ProjectileID.EyeFire, ToTarget(8f).RotatedByRandom(0.1f), Damage / 3);
            }
            else if ((t - 360) % 50 == 0)
            {
                NPC.velocity = ToTarget(20f);
                SoundEngine.PlaySound(SoundID.ForceRoar, NPC.Center);
                NPC.netUpdate = true;
            }
            else
                NPC.velocity *= 0.98f;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => Dashing;

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers) => modifiers.SourceDamage.Base += Dv.Dv(150, 180, 210, 240);

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo) => target.AddBuff(BuffID.CursedInferno, 240);
    }

    /// <summary>Cataractacomb (gris) : plane loin au-dessus et tire des boules d'ombre chercheuses ; puis se fige et tire en éventail.</summary>
    public class Cataractacomb : PolyEye
    {
        protected override int Life => 25000;
        protected override int DropItem => ModContent.ItemType<Items.Weapons.Summon.Tetrachromancy>();
        protected override Color Glow => Color.MediumPurple;

        protected override void Pattern(int t)
        {
            if (t < 360)
            {
                Vector2 spot = Target.Center + new Vector2(0f, -380f);
                NPC.velocity = Vector2.Lerp(NPC.velocity, (spot - NPC.Center) * 0.06f, 0.1f);
                if (t % 45 == 0)
                    Fire(ModContent.ProjectileType<PolyShadowball>(), ToTarget(7f));
            }
            else
            {
                NPC.velocity = Vector2.Zero;
                if (t % 60 == 0)
                    for (int i = -3; i <= 3; i++)
                        Fire(ModContent.ProjectileType<PolyShadowball>(), ToTarget(7f).RotatedBy(i * 0.2f));
            }
        }
    }

    /// <summary>Laser rouge des yeux.</summary>
    public class PolyLaser : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.EyeLaser;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
        }

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        public override Color? GetAlpha(Color lightColor) => new Color(255, 80, 80, 0);
    }

    /// <summary>Vague de glace.</summary>
    public class PolyIce : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.FrostWave;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
        }

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.Frostburn, 180);
    }

    /// <summary>Boule d'ombre : cherche un peu le joueur, Shadowflame.</summary>
    public class PolyShadowball : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 22;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Player p = Main.player[Player.FindClosest(Projectile.position, Projectile.width, Projectile.height)];
            if (Projectile.timeLeft > 180)
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, (p.Center - Projectile.Center).SafeNormalize(Vector2.UnitY) * 7f, 0.025f);
            for (int i = 0; i < 2; i++)
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8f, 8f), DustID.Shadowflame, Vector2.Zero, 100, default, 1.5f).noGravity = true;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.ShadowFlame, 180);
    }

    /// <summary>Boule de glace d'Exotrexia : reste en place 4 s et gêne le passage.</summary>
    public class FrostMine : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.IceSpike;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 240;
            Projectile.tileCollide = false;
        }

        public override void AI() => Projectile.rotation += 0.05f;

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.Chilled, 180);
    }

    public class PolyphemalusSystem : ModSystem
    {
        public static bool Downed;
        private static bool wasDay = true;

        public override void ClearWorld() => Downed = false;

        public override void SaveWorldData(TagCompound tag)
        {
            if (Downed)
                tag["downedPolyphemalus"] = true;
        }

        public override void LoadWorldData(TagCompound tag) => Downed = tag.ContainsKey("downedPolyphemalus");

        public override void NetSend(BinaryWriter writer) => writer.Write(Downed);

        public override void NetReceive(BinaryReader reader) => Downed = reader.ReadBoolean();

        /// <summary>
        /// Apparition naturelle « comme l'Eye of Cthulhu » : au début de la nuit, en Hardmode après un boss mécanique, pas
        /// encore vaincu. [choix] Une nuit sur 10.
        /// </summary>
        public override void PostUpdateWorld()
        {
            bool nightStart = wasDay && !Main.dayTime;
            wasDay = Main.dayTime;
            if (!nightStart || Main.netMode == NetmodeID.MultiplayerClient || !Main.hardMode || !NPC.downedMechBossAny || Downed || !Main.rand.NextBool(10))
                return;
            var players = Main.player.Where(p => p.active && !p.dead && p.statLifeMax2 >= 400).ToList();
            if (players.Count == 0)
                return;
            ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Mods.CalamityFandom.NPCs.Polyphemalus.Watching"), new Color(175, 75, 255));
            NPC.SpawnOnPlayer(Main.rand.Next(players).whoAmI, ModContent.NPCType<Polyphemalus>());
        }
    }
}

namespace CalamityFandom.Content.Items.SummonItems
{
    /// <summary>Wiki : Fused Eye.txt — invoque Polyphemalus la nuit ; non consommé.</summary>
    public class FusedEye : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 44;
            Item.useTime = Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
            Item.rare = ItemRarityID.Yellow;
            Item.value = 0;
        }

        public override bool CanUseItem(Player player)
            => !Main.dayTime && !NPC.AnyNPCs(ModContent.NPCType<NPCs.Polyphemalus.Polyphemalus>()) && !Main.npc.Any(n => n.active && NPCs.Polyphemalus.PolyEye.IsEye(n.type));

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI != Main.myPlayer)
                return true;
            SoundEngine.PlaySound(SoundID.Roar, player.Center);
            int type = ModContent.NPCType<NPCs.Polyphemalus.Polyphemalus>();
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.SpawnOnPlayer(player.whoAmI, type);
            else
                NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
            return true;
        }
    }
}
