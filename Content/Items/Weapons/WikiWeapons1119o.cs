using System;
using CalamityFandom.Content.Projectiles.Weapons1119o;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

// Lot S (19/11) : armes du wiki fabricables à la Cosmic Anvil (recettes du wiki), pages d'addon comprises (section 11).
// « Moon Burnt » = Nightwither de Calamity (glossaire).

namespace CalamityFandom.Content.Items.Weapons.Magic
{
    /// <summary>
    /// Wiki : First Crescent.txt — un petit croissant qui tire de petits fragments de lune dans tous les sens (50 %) ;
    /// Moon Burnt 5 s. [choix] Croissant de 2 s, un fragment toutes les 8 images.
    /// </summary>
    public class FirstCrescent : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 144;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 30;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7.5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item9;
            Item.width = Item.height = 74;
            Item.rare = ModContent.RarityType<CosmicPurple>();
            Item.value = Item.sellPrice(gold: 25);
            Item.shoot = ModContent.ProjectileType<LittleCrescent>();
            Item.shootSpeed = 16f;
        }
    }

    /// <summary>
    /// Wiki : Wrath of the Dragons.txt — une pulsation draconique au curseur lance régulièrement deux anneaux de 32 poussières
    /// de feu chercheuses (Dragonfire 3 s) et parfois deux Flare Tornadoes chercheuses (200 %) qui laissent une Flarenado
    /// (150 %, Dragonfire 4 s) au contact. Amélioration du Wrath of the Cosmos.
    /// [choix] Tornades toutes les 3 salves (la page dit 7, mais la pulsation n'en fait que 4).
    /// </summary>
    public class WrathoftheDragons : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 555;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 50;
            Item.useTime = Item.useAnimation = 33;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.knockBack = 6f;
            Item.UseSound = SoundID.Item117;
            Item.width = 38;
            Item.height = 44;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
            Item.value = Item.sellPrice(gold: 30);
            Item.shoot = ModContent.ProjectileType<Projectiles.Weapons1119j.WrathPulse>();
            Item.shootSpeed = 12f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI, 2f);
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Rivers of Frost.txt — fait toujours 1 dégât, sans critique ; chaque coup retire 1 % de la vie max de l'ennemi
    /// (en ignorant défense et réduction) ; clic droit : coûte 6,5 % de la vie max et tout le mana, imprègne l'épée 1 minute :
    /// les coups lancent une longue lame de givre (-0,5 % de vie) et les coups d'épée retirent 2 %. Providence : 0,5 % et
    /// immunisée aux lames (notes).
    /// </summary>
    public class RiversofFrost : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 1;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = 32;
            Item.height = 62;
            Item.rare = ModContent.RarityType<CosmicPurple>();
            Item.value = Item.sellPrice(gold: 36);
            Item.shoot = ProjectileID.None;
            Item.shootSpeed = 14f;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse != 2)
            {
                Item.shoot = player.GetModPlayer<RiversOfFrostPlayer>().Imbued > 0 ? ModContent.ProjectileType<RiversFrostBlade>() : ProjectileID.None;
                return true;
            }
            Item.shoot = ProjectileID.None;
            if (player.whoAmI == Main.myPlayer)
            {
                var hurt = PlayerDeathReason.ByCustomReason(Terraria.Localization.NetworkText.FromKey("Mods.CalamityFandom.Items.RiversofFrost.Death", player.name));
                player.Hurt(hurt, Math.Max(1, (int)(player.statLifeMax2 * 0.065f)), 0, dodgeable: false, armorPenetration: 9999);
                player.statMana = 0;
                player.GetModPlayer<RiversOfFrostPlayer>().Imbued = 3600;
                SoundEngine.PlaySound(SoundID.Item30, player.Center);
            }
            return true;
        }

        public override void ModifyWeaponCrit(Player player, ref float crit) => crit = 0f;

        public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.DisableCrit();
            float percent = player.GetModPlayer<RiversOfFrostPlayer>().Imbued > 0 ? 0.02f : 0.01f;
            if (target.type == ModContent.NPCType<CalamityMod.NPCs.Providence.Providence>())
                percent = 0.005f;
            RiversOfFrostPlayer.Drain(target, percent);
            modifiers.SetMaxDamage(1);
        }
    }

    public class RiversOfFrostPlayer : ModPlayer
    {
        public int Imbued;

        public override void PostUpdate()
        {
            if (Imbued > 0)
                Imbued--;
        }

        /// <summary>Retire un pourcentage de la vie max, sans tenir compte de la défense (jamais le coup final : le coup d'1 dégât s'en charge).</summary>
        public static void Drain(NPC target, float percent)
        {
            NPC real = target.realLife >= 0 ? Main.npc[target.realLife] : target;
            int amount = Math.Max(1, (int)(real.lifeMax * percent));
            real.life = Math.Max(1, real.life - amount);
            CombatText.NewText(real.Hitbox, new Color(150, 220, 255), amount);
            real.netUpdate = true;
        }
    }

    /// <summary>
    /// Wiki : The Otherside.txt — chaque coup laisse une déchirure de la réalité (environ 10 s, 5 au plus) qui blesse ; deux
    /// déchirures qui se touchent fusionnent, plus grandes et plus fortes : 25 % des dégâts pour une, +15 % par déchirure en
    /// plus, jusqu'à 100 %. [choix] Déchirure de 60 px (+30 par fusion) au bout de la lame, un coup toutes les 15 images.
    /// </summary>
    public class TheOtherside : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 980;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 10f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item71;
            Item.width = 50;
            Item.height = 74;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
            Item.value = Item.sellPrice(gold: 35);
            Item.shoot = ModContent.ProjectileType<RealityTear>();
            Item.shootSpeed = 1f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 at = player.Center + (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX * player.direction) * 70f;
            // 5 au plus : la plus ancienne disparaît
            int count = 0, oldest = -1, oldestTime = int.MaxValue;
            foreach (Projectile p in Main.ActiveProjectiles)
                if (p.owner == player.whoAmI && p.type == type)
                {
                    count++;
                    if (p.timeLeft < oldestTime)
                    {
                        oldestTime = p.timeLeft;
                        oldest = p.whoAmI;
                    }
                }
            if (count >= 5 && oldest >= 0)
                Main.projectile[oldest].Kill();
            Projectile.NewProjectile(source, at, Vector2.Zero, type, damage, knockback, player.whoAmI, 1f);
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Rogue
{
    /// <summary>
    /// Wiki : Ecological Collapse.txt — jusqu'à 10 mines lumineuses qui ralentissent jusqu'à l'arrêt, puis font une aura
    /// électrique de 20 blocs qui ignore l'invincibilité des ennemis ; au bout de 20 s, elles éclatent en 8 à 10 éclats de
    /// Lumenyl ; frappe furtive : une mine en plus du maximum, 20 à 30 éclats, mais seulement 8 s.
    /// [choix] Aura : 10 % des dégâts toutes les 10 images ; éclats à 50 %.
    /// </summary>
    public class EcologicalCollapse : RogueWeapon
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 2000;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.useTime = Item.useAnimation = 39;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = Item.noUseGraphic = true;
            Item.knockBack = 7f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = 40;
            Item.height = 38;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.value = Item.sellPrice(gold: 30);
            Item.shoot = ModContent.ProjectileType<LumenousMine>();
            Item.shootSpeed = 7f;
        }

        public override bool CanUseItem(Player player) =>
            player.Calamity().StealthStrikeAvailable() || player.ownedProjectileCounts[Item.shoot] < 10;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (player.Calamity().StealthStrikeAvailable() && Main.projectile.IndexInRange(p))
            {
                Main.projectile[p].Calamity().stealthStrike = true;
                Main.projectile[p].timeLeft = 480;
            }
            return false;
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Weapons1119o
{
    /// <summary>Petit croissant de la First Crescent : 2 s, un fragment de lune (50 %) toutes les 8 images dans un sens au hasard.</summary>
    public class LittleCrescent : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.97f;
            if (!Main.dedServ)
                for (int i = 0; i < 2; i++)
                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(10f, 10f), DustID.IceTorch, Vector2.Zero, 100, default, 1.2f).noGravity = true;
            if (Projectile.owner == Main.myPlayer && Projectile.timeLeft % 8 == 0)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2CircularEdge(10f, 10f), ModContent.ProjectileType<MoonFragment>(),
                    Projectile.damage / 2, 0f, Projectile.owner);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Nightwither>(), 300);
    }

    /// <summary>Fragment de lune : Moon Burnt (Nightwither) 5 s.</summary>
    public class MoonFragment : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.WhiteTorch, Vector2.Zero, 100, new Color(200, 220, 255), 1f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Nightwither>(), 300);
    }

    /// <summary>Lame de givre de la Rivers of Frost imprégnée : longue portée, retire 0,5 % de la vie max (pas Providence).</summary>
    public class RiversFrostBlade : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 50;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool? CanHitNPC(NPC target) => target.type == ModContent.NPCType<CalamityMod.NPCs.Providence.Providence>() ? false : null;

        public override void AI()
        {
            if (!Main.dedServ)
                for (int i = 0; i < 3; i++)
                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(14f, 14f), DustID.IceTorch, Vector2.Zero, 100, default, 1.4f).noGravity = true;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.DisableCrit();
            modifiers.SetMaxDamage(1);
            Items.Weapons.Melee.RiversOfFrostPlayer.Drain(target, 0.005f);
        }
    }

    /// <summary>
    /// Déchirure de The Otherside : ~10 s ; ai[0] = nombre de déchirures fusionnées ; dégâts 25 % + 15 % par fusion (100 % au
    /// plus) ; fusionne avec une autre déchirure du joueur qu'elle touche.
    /// </summary>
    public class RealityTear : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private int baseDamage;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void OnSpawn(IEntitySource source)
        {
            baseDamage = Projectile.damage;
            UpdateSize();
        }

        private void UpdateSize()
        {
            int merged = Math.Max(1, (int)Projectile.ai[0]);
            Projectile.Resize(30 + 30 * merged, 30 + 30 * merged);
            Projectile.damage = (int)(baseDamage * Math.Min(1f, 0.25f + 0.15f * (merged - 1)));
        }

        public override void AI()
        {
            if (!Main.dedServ)
                for (int i = 0; i < 3; i++)
                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 2f, Projectile.height / 2f), DustID.Shadowflame, Vector2.Zero, 100, default, 1.3f).noGravity = true;
            if (Projectile.owner != Main.myPlayer)
                return;
            foreach (Projectile p in Main.ActiveProjectiles)
                if (p.whoAmI != Projectile.whoAmI && p.type == Type && p.owner == Projectile.owner && p.Hitbox.Intersects(Projectile.Hitbox) && p.whoAmI > Projectile.whoAmI)
                {
                    Projectile.ai[0] += Math.Max(1f, p.ai[0]);
                    Projectile.timeLeft = Math.Max(Projectile.timeLeft, p.timeLeft);
                    p.Kill();
                    UpdateSize();
                    Projectile.netUpdate = true;
                    break;
                }
        }
    }

    /// <summary>
    /// Mine lumineuse : ralentit jusqu'à l'arrêt, puis aura électrique de 20 blocs (10 % toutes les 10 images, immunité
    /// locale : ignore l'invincibilité des ennemis) ; au bout de 20 s (8 en furtif), éclate en 8-10 éclats (20-30 en furtif).
    /// </summary>
    public class LumenousMine : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Rogue/EcologicalCollapse";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1200;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = Vector2.Zero;
            return false;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.95f;
            Projectile.rotation += Projectile.velocity.Length() * 0.05f;
            if (Projectile.velocity.Length() > 0.3f)
                return;
            Projectile.velocity = Vector2.Zero;
            Lighting.AddLight(Projectile.Center, 0.3f, 0.6f, 0.9f);
            if (!Main.dedServ && Main.rand.NextBool(2))
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(320f, 320f), DustID.Electric, Vector2.Zero, 100, default, 0.8f).noGravity = true;
            if (Projectile.owner == Main.myPlayer && Projectile.timeLeft % 10 == 0)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<LumenousAura>(), Projectile.damage / 10, 0f, Projectile.owner);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);
            if (Projectile.owner != Main.myPlayer)
                return;
            bool stealth = Projectile.Calamity().stealthStrike;
            int count = stealth ? Main.rand.Next(20, 31) : Main.rand.Next(8, 11);
            for (int i = 0; i < count; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2CircularEdge(10f, 10f) * Main.rand.NextFloat(0.6f, 1f),
                    ModContent.ProjectileType<LumenylShard>(), Projectile.damage / 2, 0f, Projectile.owner);
        }
    }

    /// <summary>Aura électrique d'une mine : 20 blocs, un coup, immunité locale.</summary>
    public class LumenousAura : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 640;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) =>
            Vector2.Distance(Projectile.Center, targetHitbox.ClosestPointInRect(Projectile.Center)) < 320f;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Electrified, 60);
    }

    /// <summary>Éclat de Lumenyl.</summary>
    public class LumenylShard : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.CrystalShard;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = 1;
            Projectile.timeLeft = 90;
            Projectile.tileCollide = false;
        }

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        public override Color? GetAlpha(Color lightColor) => new Color(120, 220, 255, 200);
    }
}
