using System;
using CalamityFandom.Content.Legacy.Buffs.StatDebuffs;
using CalamityFandom.Content.Projectiles.WikiDarts;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

// Lot AC (19/11) : fléchettes du wiki (recettes du wiki). Valeurs des infobox ; comportements de la prose des pages.

namespace CalamityFandom.Content.Items.Ammo
{
    /// <summary>Fléchette du wiki : munition de type Dart, son projectile porte le même nom suivi de « Proj ».</summary>
    public abstract class WikiDart : ModItem
    {
        protected abstract int Damage { get; }
        protected abstract int Rarity { get; }
        protected abstract float Velocity { get; }
        protected abstract int SellSilver { get; }
        protected abstract int Projectile { get; }

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 99;

        public override void SetDefaults()
        {
            Item.damage = Damage;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 18;
            Item.height = 28;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.knockBack = 1.5f;
            Item.value = Item.sellPrice(silver: SellSilver);
            Item.rare = Rarity;
            Item.shoot = Projectile;
            Item.shootSpeed = Velocity;
            Item.ammo = AmmoID.Dart;
        }
    }

    /// <summary>
    /// Wiki : Exterminative Dart.txt — traverse les blocs, cherche les ennemis, God Slayer Inferno 2 s ; à l'impact :
    /// explosion (30 %) et 4 fléchettes planétaires (10 %, percent une fois, cherchent les ennemis, 1,5 s) qui affaiblissent la
    /// cible : solaire -1 % de réduction des dégâts, vortex -2 % de vitesse, nébuleuse -1 % de dégâts, poussière d'étoile
    /// 50 PV/s ; 4 cumuls au plus chacun, un cumul perdu toutes les 2 s.
    /// </summary>
    public class ExterminativeDart : WikiDart
    {
        protected override int Damage => 30;
        protected override int Rarity => ModContent.RarityType<CosmicPurple>();
        protected override float Velocity => 8f;
        protected override int SellSilver => 30;
        protected override int Projectile => ModContent.ProjectileType<ExterminativeDartProj>();
    }

    /// <summary>
    /// Wiki : Nighterrial Dart.txt — avec un ennemi à moins de 12 blocs, la fléchette s'arrête et lance un crochet spectral qui
    /// l'agrippe (-30 % de vitesse), 150 % toutes les 2 s, Moon Burnt (= Nightwither) 3 s ; 4 s plus tard le crochet disparaît
    /// et la fléchette tombe. 3 crochets par fléchette (pas deux sur le même ennemi), 3 par ennemi.
    /// </summary>
    public class NighterrialDart : WikiDart
    {
        protected override int Damage => 15;
        protected override int Rarity => ModContent.RarityType<PureGreen>();
        protected override float Velocity => 5f;
        protected override int SellSilver => 20;
        protected override int Projectile => ModContent.ProjectileType<NighterrialDartProj>();
    }

    /// <summary>
    /// Wiki : Profaned Crystal Dart.txt — ricoche 6 fois sur les blocs et les ennemis (en les perçant) ; à chaque ricochet, une
    /// petite roche profanée qui cherche l'ennemi le plus proche (10 %, Holy Flames 2 s). La fléchette inflige Holy Flames 2 s.
    /// </summary>
    public class ProfanedCrystalDart : WikiDart
    {
        protected override int Damage => 20;
        protected override int Rarity => ItemRarityID.Red;
        protected override float Velocity => 12f;
        protected override int SellSilver => 15;
        protected override int Projectile => ModContent.ProjectileType<ProfanedCrystalDartProj>();
    }

    /// <summary>
    /// Wiki : Profaned Fire Dart.txt — perce à l'infini sans chercher ; après 7 blocs, se divise en 2 à 4 boules de feu (50 %,
    /// cherchent fort, percent 3 fois) qui laissent des étincelles toutes les 0,33 s (5 %, cherchent un peu). Holy Flames 2 s
    /// (fléchette, boules), 1 s (étincelles).
    /// </summary>
    public class ProfanedFireDart : WikiDart
    {
        protected override int Damage => 20;
        protected override int Rarity => ItemRarityID.Red;
        protected override float Velocity => 6f;
        protected override int SellSilver => 15;
        protected override int Projectile => ModContent.ProjectileType<ProfanedFireDartProj>();
    }

    /// <summary>
    /// Wiki : Terrestrial Dart.txt — se plante dans l'ennemi : 10 % toutes les 0,5 s, tous les 3 coups une énergie de vie
    /// (30 %) ; disparaît au bout de 3 s. [choix] L'énergie de vie cherche l'ennemi le plus proche.
    /// </summary>
    public class TerrestrialDart : WikiDart
    {
        protected override int Damage => 15;
        protected override int Rarity => ItemRarityID.Yellow;
        protected override float Velocity => 4f;
        protected override int SellSilver => 5;
        protected override int Projectile => ModContent.ProjectileType<TerrestrialDartProj>();
    }

    /// <summary>
    /// Wiki : Vortex Dart.txt — sans ennemi à moins de 10 blocs, tourne autour du joueur ; sinon fonce dessus. 20 au plus (la
    /// plus ancienne disparaît). [choix] 10 s de vie.
    /// </summary>
    public class VortexDart : WikiDart
    {
        protected override int Damage => 25;
        protected override int Rarity => ItemRarityID.Cyan;
        protected override float Velocity => 6f;
        protected override int SellSilver => 10;
        protected override int Projectile => ModContent.ProjectileType<VortexDartProj>();
    }

    /// <summary>
    /// Wiki : Waterfrost Dart.txt — ralentit jusqu'à s'arrêter (~16 blocs), puis bondit toutes les 1,5 s (~12 blocs) vers
    /// l'endroit où sera l'ennemi le plus proche, en laissant une stalactite ; à l'impact : explosion de glace (30 %,
    /// Glacial State 2 s) et ses stalactites sont lancées sur l'ennemi touché (10 %) ; après 5 bonds, explose de même (les
    /// stalactites visent l'ennemi le plus proche). Traverse les blocs, pas les stalactites. Rareté « C1 » → 0.
    /// [technique] Reactive Flames (sans page) non fait.
    /// </summary>
    public class WaterfrostDart : WikiDart
    {
        protected override int Damage => 40;
        protected override int Rarity => ItemRarityID.White;
        protected override float Velocity => 20f;
        protected override int SellSilver => 50;
        protected override int Projectile => ModContent.ProjectileType<WaterfrostDartProj>();
    }
}

namespace CalamityFandom.Content.Projectiles.WikiDarts
{
    /// <summary>Fléchette du wiki : texture de l'objet, pointe vers l'avant (vers le haut sur le sprite).</summary>
    public abstract class WikiDartProj : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Ammo/" + Name.Replace("Proj", "");

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
        }

        protected void FaceVelocity() => Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        public static NPC Closest(Vector2 from, float range, int except = -1)
        {
            NPC best = null;
            float dist = range;
            foreach (NPC n in Main.ActiveNPCs)
                if (n.whoAmI != except && n.CanBeChasedBy() && Vector2.Distance(from, n.Center) < dist)
                {
                    dist = Vector2.Distance(from, n.Center);
                    best = n;
                }
            return best;
        }

        public static void Home(Projectile p, NPC target, float speed, float inertia)
        {
            if (target == null)
                return;
            Vector2 want = (target.Center - p.Center).SafeNormalize(Vector2.Zero) * speed;
            p.velocity = (p.velocity * (inertia - 1f) + want) / inertia;
        }
    }

    /// <summary>Explosion des fléchettes : ai[0] rayon, ai[1] poussière, ai[2] debuff (0 aucun), 2 s.</summary>
    public class DartBlast : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 5;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            if (Projectile.localAI[0]++ != 0f)
                return;
            Projectile.Resize((int)Projectile.ai[0], (int)Projectile.ai[0]);
            SoundEngine.PlaySound(SoundID.Item14 with { Volume = 0.5f }, Projectile.Center);
            if (!Main.dedServ)
                for (int i = 0; i < 20; i++)
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, (int)Projectile.ai[1], 0f, 0f, 100, default, 1.6f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[2] > 0f)
                target.AddBuff((int)Projectile.ai[2], 120);
        }
    }

    public class ExterminativeDartProj : WikiDartProj
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Home(Projectile, Closest(Projectile.Center, 600f), 14f, 15f);
            FaceVelocity();
            Lighting.AddLight(Projectile.Center, 0.5f, 0.3f, 0.8f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 120);
            if (Projectile.owner != Main.myPlayer)
                return;
            var source = Projectile.GetSource_FromThis();
            Projectile.NewProjectile(source, target.Center, Vector2.Zero, ModContent.ProjectileType<DartBlast>(), (int)(Projectile.damage * 0.3f), 0f,
                Projectile.owner, 80f, DustID.PurpleTorch, ModContent.BuffType<GodSlayerInferno>());
            for (int i = 0; i < 4; i++)
            {
                Vector2 dir = (MathHelper.PiOver2 * i).ToRotationVector2();
                Projectile.NewProjectile(source, target.Center + dir * 160f, -dir * 10f, ModContent.ProjectileType<PlanetaryDart>(),
                    Math.Max(1, Projectile.damage / 10), 0f, Projectile.owner, i);
            }
        }
    }

    /// <summary>Fléchette planétaire : ai[0] 0 solaire, 1 vortex, 2 nébuleuse, 3 poussière d'étoile.</summary>
    public class PlanetaryDart : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Ammo/ExterminativeDart";

        private static readonly Color[] Colors = { new(255, 150, 50), new(80, 255, 200), new(255, 80, 220), new(80, 160, 255) };

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 90;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.scale = 0.7f;
        }

        public override void AI()
        {
            WikiDartProj.Home(Projectile, WikiDartProj.Closest(Projectile.Center, 500f), 12f, 10f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.GetGlobalNPC<ExterminativeNPC>().Add((int)Projectile.ai[0]);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Colors[(int)Projectile.ai[0] % 4] with { A = 100 },
                Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None);
            return false;
        }
    }

    /// <summary>Cumuls des fléchettes planétaires (4 au plus chacun, un perdu toutes les 2 s).</summary>
    public class ExterminativeNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public int[] Stacks = new int[4];
        private int decay;

        public void Add(int kind)
        {
            Stacks[kind] = Math.Min(4, Stacks[kind] + 1);
            decay = 120;
        }

        public override void PostAI(NPC npc)
        {
            if (Stacks[0] + Stacks[1] + Stacks[2] + Stacks[3] == 0)
                return;
            if (--decay <= 0)
            {
                decay = 120;
                for (int i = 0; i < 4; i++)
                    Stacks[i] = Math.Max(0, Stacks[i] - 1);
            }
            // Vortex : -2 % de vitesse par cumul
            if (Stacks[1] > 0 && !npc.boss)
                npc.position -= npc.velocity * 0.02f * Stacks[1];
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (Stacks[3] <= 0)
                return;
            if (npc.lifeRegen > 0)
                npc.lifeRegen = 0;
            npc.lifeRegen -= 100 * Stacks[3];
            damage = Math.Max(damage, 10 * Stacks[3]);
        }

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (Stacks[0] > 0)
                modifiers.FinalDamage *= 1f + 0.01f * Stacks[0];
        }

        public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
        {
            if (Stacks[2] > 0)
                modifiers.SourceDamage *= 1f - 0.01f * Stacks[2];
        }
    }

    public class NighterrialDartProj : WikiDartProj
    {
        // ai[0] : crochets lancés ; ai[1] : images d'arrêt restantes
        public override void AI()
        {
            if (Projectile.ai[1] > 0f)
            {
                Projectile.velocity *= 0.8f;
                if (--Projectile.ai[1] <= 0f)
                    Projectile.velocity = new Vector2(0f, 2f);
            }
            else if (Projectile.ai[0] > 0f)
                Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.3f, 14f);
            if (Projectile.owner == Main.myPlayer && Projectile.ai[0] < 3f && Projectile.localAI[0]++ % 10 == 0)
            {
                int hookType = ModContent.ProjectileType<Polterhook>();
                foreach (NPC n in Main.ActiveNPCs)
                {
                    if (!n.CanBeChasedBy() || Vector2.Distance(n.Center, Projectile.Center) > 192f)
                        continue;
                    int onNpc = 0;
                    bool mine = false;
                    foreach (Projectile p in Main.ActiveProjectiles)
                        if (p.type == hookType && (int)p.ai[0] == n.whoAmI)
                        {
                            onNpc++;
                            mine |= (int)p.ai[1] == Projectile.identity;
                        }
                    if (onNpc >= 3 || mine)
                        continue;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (n.Center - Projectile.Center).SafeNormalize(Vector2.UnitX) * 20f,
                        hookType, (int)(Projectile.damage * 1.5f), 0f, Projectile.owner, n.whoAmI, Projectile.identity);
                    Projectile.ai[0]++;
                    Projectile.ai[1] = 240f;
                    Projectile.netUpdate = true;
                    break;
                }
            }
            FaceVelocity();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Nightwither>(), 180);
    }

    /// <summary>Crochet spectral : ai[0] ennemi visé, ai[1] fléchette d'origine. Agrippe 4 s, 150 % toutes les 2 s.</summary>
    public class Polterhook : ModProjectile
    {
        public override string Texture => "CalamityMod/NPCs/Polterghast/PolterghastHook";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 120;
            Projectile.scale = 0.6f;
        }

        public override bool? CanHitNPC(NPC target) => target.whoAmI == (int)Projectile.ai[0] ? null : false;

        public override void AI()
        {
            NPC target = Main.npc[(int)Projectile.ai[0]];
            if (!target.active || target.dontTakeDamage)
            {
                Projectile.Kill();
                return;
            }
            if (Projectile.localAI[0] == 0f)
            {
                WikiDartProj.Home(Projectile, target, 24f, 4f);
                if (Projectile.Hitbox.Intersects(target.Hitbox))
                {
                    Projectile.localAI[0] = 1f;
                    Projectile.timeLeft = 240;
                }
            }
            else
            {
                Projectile.Center = target.Center;
                Projectile.velocity = Vector2.Zero;
                if (!target.boss)
                    target.position -= target.velocity * 0.3f;
            }
            Projectile.rotation += 0.1f;
            if (!Main.dedServ && Main.rand.NextBool(3))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.DungeonSpirit, 0f, 0f, 100, default, 1f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Nightwither>(), 180);
    }

    public class ProfanedCrystalDartProj : WikiDartProj
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.penetrate = 7;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI() => FaceVelocity();

        private void Ricochet()
        {
            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Circular(4f, 4f),
                    ModContent.ProjectileType<ProfanedDartRock>(), Math.Max(1, Projectile.damage / 10), 0f, Projectile.owner);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (--Projectile.penetrate <= 0)
                return true;
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            Ricochet();
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 120);
            Ricochet();
            NPC next = Closest(Projectile.Center, 500f, target.whoAmI);
            float speed = Projectile.velocity.Length();
            Projectile.velocity = next != null ? (next.Center - Projectile.Center).SafeNormalize(Vector2.UnitX) * speed : -Projectile.velocity;
        }
    }

    /// <summary>Petite roche profanée : cherche l'ennemi le plus proche, Holy Flames 2 s.</summary>
    public class ProfanedDartRock : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.BoulderStaffOfEarth;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.scale = 0.4f;
        }

        public override void AI()
        {
            WikiDartProj.Home(Projectile, WikiDartProj.Closest(Projectile.Center, 500f), 10f, 12f);
            Projectile.rotation += 0.2f;
            if (!Main.dedServ && Main.rand.NextBool(2))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GoldFlame, 0f, 0f, 100, default, 0.9f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<HolyFlames>(), 120);
    }

    public class ProfanedFireDartProj : WikiDartProj
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            FaceVelocity();
            Projectile.localAI[0] += Projectile.velocity.Length();
            if (Projectile.localAI[0] < 112f)
                return;
            if (Projectile.owner == Main.myPlayer)
            {
                int n = Main.rand.Next(2, 5);
                for (int i = 0; i < n; i++)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.RotatedByRandom(0.8f) * 1.5f,
                        ModContent.ProjectileType<ProfanedFireball>(), Projectile.damage / 2, 0f, Projectile.owner);
            }
            Projectile.Kill();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<HolyFlames>(), 120);
    }

    /// <summary>Boule de feu profanée : cherche fort, perce 3 fois, laisse des étincelles toutes les 20 images.</summary>
    public class ProfanedFireball : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 180;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void AI()
        {
            WikiDartProj.Home(Projectile, WikiDartProj.Closest(Projectile.Center, 700f), 15f, 8f);
            if (!Main.dedServ)
                for (int i = 0; i < 2; i++)
                    Dust.NewDustPerfect(Projectile.Center, DustID.Torch, Main.rand.NextVector2Circular(1f, 1f), 100, default, 1.6f).noGravity = true;
            if (Projectile.owner == Main.myPlayer && ++Projectile.localAI[0] % 20 == 0)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Circular(3f, 3f),
                    ModContent.ProjectileType<ProfanedSpark>(), Math.Max(1, Projectile.damage / 10), 0f, Projectile.owner);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<HolyFlames>(), 120);
    }

    /// <summary>Étincelle profanée (5 % de la fléchette) : cherche un peu, Holy Flames 1 s.</summary>
    public class ProfanedSpark : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 90;
        }

        public override void AI()
        {
            WikiDartProj.Home(Projectile, WikiDartProj.Closest(Projectile.Center, 400f), 8f, 25f);
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.GoldFlame, Vector2.Zero, 100, default, 0.9f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<HolyFlames>(), 60);
    }

    public class TerrestrialDartProj : WikiDartProj
    {
        // ai[0] : 1 = plantée ; ai[1] : ennemi ; localAI[0] : coups ; localAI[1] : dégâts de base
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        public override bool? CanHitNPC(NPC target) => Projectile.ai[0] == 1f && target.whoAmI != (int)Projectile.ai[1] ? false : null;

        public override void AI()
        {
            if (Projectile.ai[0] != 1f)
            {
                FaceVelocity();
                return;
            }
            NPC target = Main.npc[(int)Projectile.ai[1]];
            if (!target.active)
            {
                Projectile.Kill();
                return;
            }
            Projectile.Center = target.Center + new Vector2(Projectile.localAI[1], Projectile.localAI[2]);
            Projectile.velocity = Vector2.Zero;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[0] != 1f)
            {
                Projectile.ai[0] = 1f;
                Projectile.ai[1] = target.whoAmI;
                Vector2 offset = (Projectile.Center - target.Center) * 0.6f;
                Projectile.localAI[1] = offset.X;
                Projectile.localAI[2] = offset.Y;
                Projectile.timeLeft = 180;
                Projectile.tileCollide = false;
                Projectile.originalDamage = Projectile.damage;
                Projectile.damage = Math.Max(1, Projectile.damage / 10);
                Projectile.netUpdate = true;
                return;
            }
            if (++Projectile.localAI[0] % 3 == 0 && Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2CircularEdge(6f, 6f),
                    ModContent.ProjectileType<LifeEnergy>(), (int)(Projectile.originalDamage * 0.3f), 0f, Projectile.owner);
        }
    }

    /// <summary>Énergie de vie de la Terrestrial Dart (30 %).</summary>
    public class LifeEnergy : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            WikiDartProj.Home(Projectile, WikiDartProj.Closest(Projectile.Center, 600f), 10f, 14f);
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.GreenFairy, Vector2.Zero, 100, default, 1.2f).noGravity = true;
        }
    }

    public class VortexDartProj : WikiDartProj
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.tileCollide = false;
        }

        public override void OnSpawn(Terraria.DataStructures.IEntitySource source)
        {
            // 20 au plus : la plus ancienne disparaît
            int count = 0;
            Projectile oldest = null;
            foreach (Projectile p in Main.ActiveProjectiles)
                if (p.type == Type && p.owner == Projectile.owner && p.whoAmI != Projectile.whoAmI)
                {
                    count++;
                    if (oldest == null || p.timeLeft < oldest.timeLeft)
                        oldest = p;
                }
            if (count >= 20)
                oldest?.Kill();
        }

        public override void AI()
        {
            NPC target = Closest(Projectile.Center, 160f);
            if (target != null)
                Home(Projectile, target, 18f, 6f);
            else if (++Projectile.ai[0] > 20f)
            {
                Player owner = Main.player[Projectile.owner];
                float angle = (Projectile.ai[0] * 0.06f) + Projectile.identity;
                Vector2 spot = owner.Center + angle.ToRotationVector2() * 100f;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, (spot - Projectile.Center) * 0.15f, 0.2f);
            }
            FaceVelocity();
            Lighting.AddLight(Projectile.Center, 0.2f, 0.6f, 0.5f);
        }
    }

    public class WaterfrostDartProj : WikiDartProj
    {
        // ai[0] : bonds faits ; ai[1] : compte à rebours du prochain bond ; ai[2] : ennemi touché (-1 aucun)
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.tileCollide = false;
            Projectile.timeLeft = 900;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.92f;
            if (Projectile.velocity.LengthSquared() > 1f)
                FaceVelocity();
            if (Projectile.velocity.LengthSquared() > 0.25f)
                return;
            if (++Projectile.ai[1] < 90f)
                return;
            Projectile.ai[1] = 0f;
            if (Projectile.ai[0]++ >= 5f)
            {
                Projectile.ai[2] = -1f;
                Projectile.Kill();
                return;
            }
            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<WaterfrostIcicle>(),
                    Math.Max(1, Projectile.damage / 10), 0f, Projectile.owner, Projectile.identity);
            NPC target = Closest(Projectile.Center, 800f);
            Vector2 aim = target != null ? target.Center + target.velocity * 20f : Projectile.Center + Projectile.rotation.ToRotationVector2().RotatedBy(-MathHelper.PiOver2) * 100f;
            Projectile.velocity = (aim - Projectile.Center).SafeNormalize(Vector2.UnitX) * 15.4f;
            Projectile.netUpdate = true;
            SoundEngine.PlaySound(SoundID.Item28 with { Volume = 0.4f }, Projectile.Center);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => Projectile.ai[2] = target.whoAmI + 1;

        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner != Main.myPlayer)
                return;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<DartBlast>(), (int)(Projectile.damage * 0.3f), 0f,
                Projectile.owner, 64f, DustID.IceTorch, ModContent.BuffType<GlacialState>());
            NPC target = Projectile.ai[2] >= 1f ? Main.npc[(int)Projectile.ai[2] - 1] : Closest(Projectile.Center, 800f);
            foreach (Projectile p in Main.ActiveProjectiles)
                if (p.type == ModContent.ProjectileType<WaterfrostIcicle>() && p.owner == Projectile.owner && (int)p.ai[0] == Projectile.identity && p.ai[1] == 0f)
                {
                    p.ai[1] = 1f;
                    Vector2 aim = target != null && target.active ? target.Center + target.velocity * 10f : p.Center + Vector2.UnitY * 100f;
                    p.velocity = (aim - p.Center).SafeNormalize(Vector2.UnitY) * 18f;
                    p.timeLeft = 120;
                    p.netUpdate = true;
                }
        }
    }

    /// <summary>Stalactite de la Waterfrost Dart : ai[0] fléchette, ai[1] 1 = lancée (10 %, s'arrête sur les blocs).</summary>
    public class WaterfrostIcicle : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.FrostShard;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 900;
            Projectile.tileCollide = false;
        }

        public override bool? CanDamage() => Projectile.ai[1] == 1f ? null : false;

        public override void AI()
        {
            if (Projectile.ai[1] == 1f)
            {
                Projectile.tileCollide = true;
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
        }
    }
}
