using System;
using CalamityFandom.Content.Buffs;
using CalamityFandom.Content.Projectiles.Weapons1119ac;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Dart = CalamityFandom.Content.Projectiles.WikiDarts.WikiDartProj;
using Blast = CalamityFandom.Content.Projectiles.WikiDarts.DartBlast;

// Lot AL (19/11) : armes fabricables du wiki (recettes du wiki). Rareté « C3 » → 0.

namespace CalamityFandom.Content.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : H.S Trident.txt — lance qui se divise en deux : un rayon de lumière part du côté opposé du joueur ; perce
    /// beaucoup ; Crush Depth 2 s. [choix] Rayon à 100 %, perce à l'infini, 1 s.
    /// </summary>
    public class HSTrident : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.Spears[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 120;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 19;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = Item.noUseGraphic = true;
            Item.knockBack = 6f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = 110;
            Item.height = 108;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.value = Item.sellPrice(gold: 30);
            Item.shoot = ModContent.ProjectileType<HSTridentProj>();
            Item.shootSpeed = 5f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            Projectile.NewProjectile(source, player.Center, -velocity.SafeNormalize(Vector2.UnitX) * 16f, ModContent.ProjectileType<HSLightBeam>(), damage, knockback, player.whoAmI);
            return false;
        }
    }

    /// <summary>
    /// Wiki : XB-∞ Hekate (weapon).txt — épée plus grande que les autres de son niveau ; à chaque coup, Exo Freeze et un
    /// geyser de flammes exo au-dessus de l'ennemi. [choix] Geyser à 50 %, 1 s ; Exo Freeze 0,5 s.
    /// </summary>
    public class XBHekate : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 1060;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = 84;
            Item.height = 90;
            Item.scale = 1.3f;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
            Item.value = Item.sellPrice(gold: 50);
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ExoFreeze>(), 30);
            if (player.whoAmI == Main.myPlayer)
                Projectile.NewProjectile(player.GetSource_ItemUse(Item), target.Bottom, Vector2.Zero, ModContent.ProjectileType<ExoGeyser>(),
                    player.GetWeaponDamage(Item) / 2, 0f, player.whoAmI);
        }
    }

    /// <summary>
    /// Wiki : Gloomy Trident.txt — combo de 3 : deux coups (vers le haut puis vers le bas) qui lancent des perles noires
    /// (Crush Depth 3 s), puis un estoc qui lance un maelström (éclairs, Electrified 2 s) ; vraie mêlée : Crush Depth 8 s.
    /// En Revengeance, la Rage monte lentement tant qu'on la tient ; activer la Rage la remplace par le mirage de la
    /// Souveraine : un maelström bien plus fort (x2) qui aspire les ennemis, Terror of the Abyss (5 s) à tout l'écran ; la
    /// Rage retombe à 0 ; recharge de 60 s.
    /// [choix] 5 perles à 50 % par coup ; maelström de 2 s (un éclair à 50 % toutes les 15 images) ; la Rage monte de 1 % par
    /// seconde ; le mirage n'est pas dessiné (seul son maelström).
    /// </summary>
    public class GloomyTrident : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 447;
            Item.DamageType = DamageClass.Melee;
            Item.crit = 8;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = 88;
            Item.height = 100;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.value = Item.sellPrice(gold: 40);
            Item.shoot = ModContent.ProjectileType<BlackPearl>();
            Item.shootSpeed = 30f;
        }

        public override bool CanUseItem(Player player)
        {
            var mp = player.GetModPlayer<GloomyPlayer>();
            bool thrust = mp.Combo == 2;
            Item.useStyle = thrust ? ItemUseStyleID.Shoot : ItemUseStyleID.Swing;
            Item.noMelee = Item.noUseGraphic = thrust;
            Item.shoot = thrust ? ModContent.ProjectileType<GloomySpear>() : ModContent.ProjectileType<BlackPearl>();
            Item.shootSpeed = thrust ? 6f : 30f;
            return !thrust || player.ownedProjectileCounts[ModContent.ProjectileType<GloomySpear>()] < 1;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var mp = player.GetModPlayer<GloomyPlayer>();
            if (mp.Combo == 2)
            {
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                Projectile.NewProjectile(source, player.Center + velocity.SafeNormalize(Vector2.UnitX) * 120f, velocity * 0.5f, ModContent.ProjectileType<GloomMaelstrom>(),
                    damage, 0f, player.whoAmI, 0f);
            }
            else
                for (int i = 0; i < 5; i++)
                    Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.Lerp(-0.3f, 0.3f, i / 4f)) * Main.rand.NextFloat(0.4f, 0.6f), type, damage / 2, 2f, player.whoAmI);
            mp.Combo = (mp.Combo + 1) % 3;
            return false;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            // 2e coup : de haut en bas (sens inverse du 1er)
            if (player.GetModPlayer<GloomyPlayer>().Combo == 2 && player.itemAnimation > 0)
                player.itemRotation = -player.itemRotation;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<CrushDepth>(), 480);

        public override void HoldItem(Player player)
        {
            var cal = player.Calamity();
            var mp = player.GetModPlayer<GloomyPlayer>();
            if (cal.RageEnabled && !cal.rageModeActive && player.miscCounter % 60 == 0)
                cal.rage = Math.Min(cal.rageMax, cal.rage + cal.rageMax * 0.01f);
            if (cal.rageModeActive && mp.MirageCooldown <= 0 && player.whoAmI == Main.myPlayer)
            {
                mp.MirageCooldown = 3600;
                cal.rage = 0f;
                cal.rageModeActive = false;
                player.ClearBuff(ModContent.BuffType<CalamityMod.Buffs.StatBuffs.RageMode>());
                Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<GloomMaelstrom>(),
                    player.GetWeaponDamage(Item) * 2, 0f, player.whoAmI, 1f);
                foreach (NPC n in Main.ActiveNPCs)
                    if (!n.friendly && Vector2.Distance(n.Center, player.Center) < 1400f)
                        n.AddBuff(ModContent.BuffType<TerroroftheAbyss>(), 300);
                SoundEngine.PlaySound(SoundID.Zombie104, player.Center);
            }
        }
    }

    public class GloomyPlayer : ModPlayer
    {
        public int Combo, MirageCooldown;

        public override void PostUpdate()
        {
            if (MirageCooldown > 0)
                MirageCooldown--;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Ranged
{
    /// <summary>
    /// Wiki : Stringstreaker.txt — amélioration de l'Auralis : crée 2 lignes qui font 1200 dégâts par seconde et y tire
    /// régulièrement des balles très rapides (1800). [choix] Lignes de 800 px à ±4° vers le curseur pendant le tir (une
    /// frappe toutes les 6 images = 1200/s) ; une balle par ligne à chaque tir ; utilise des balles.
    /// </summary>
    public class Stringstreaker : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 1200;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item40;
            Item.width = 78;
            Item.height = 32;
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(platinum: 2);
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 34f;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int lines = ModContent.ProjectileType<StringLine>();
            for (int i = -1; i <= 1; i += 2)
            {
                float angle = i * MathHelper.ToRadians(4f);
                if (player.ownedProjectileCounts[lines] < 2)
                    Projectile.NewProjectile(source, player.Center, velocity.SafeNormalize(Vector2.UnitX), lines, damage / 10, 0f, player.whoAmI, angle);
                Projectile.NewProjectile(source, position, velocity.RotatedBy(angle), ModContent.ProjectileType<StringBullet>(), (int)(damage * 1.5f), knockback, player.whoAmI);
            }
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Magic
{
    /// <summary>
    /// Wiki : Wrath of the Gods.txt — une pulsation divine au curseur qui lance à intervalles un anneau de 48 boules de feu
    /// traîtresses chercheuses ; tous les 10 anneaux, elle explose en hexagramme d'orbes (75 %) et tire un rayon rouge
    /// chercheur (500 %), sans disparaître. Whispering Death et Brimstone Flames (3 s boules, 2 s orbes, 10 s rayon),
    /// Vulnerability Hex 4 s (rayon). [choix] Une pulsation à la fois (une nouvelle remplace l'ancienne), 20 s ; un anneau
    /// par seconde ; boules à 25 %.
    /// </summary>
    public class WrathoftheGods : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 999;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 75;
            Item.useTime = Item.useAnimation = 120;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.knockBack = 7f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item119;
            Item.width = 38;
            Item.height = 44;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.value = Item.sellPrice(gold: 40);
            Item.shoot = ModContent.ProjectileType<GodlyPulse>();
            Item.shootSpeed = 0f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            foreach (Projectile p in Main.ActiveProjectiles)
                if (p.type == type && p.owner == player.whoAmI)
                    p.Kill();
            Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI);
            return false;
        }
    }

    /// <summary>
    /// Wiki : Grand Purification.txt — baguette : boule de feu bleue rapide qui éclate en flammes persistantes (ignorent la
    /// moitié de l'invulnérabilité, dégâts qui montent, Moon Burnt 4 s) ; touche « Open » : Supernova sacrée (Betsy's Curse
    /// 10 s) ; recharge de 2 min, pendant laquelle la touche fait un pilier de lumière au curseur (75 %).
    /// [choix] « Open » = clic droit ; flammes de 1 s, dégâts x1 à x2,5 ; Supernova de 480 px à 500 %.
    /// </summary>
    public class GrandPurification : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 640;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 65;
            Item.useTime = Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item20;
            Item.width = Item.height = 116;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.value = Item.sellPrice(platinum: 22);
            Item.shoot = ModContent.ProjectileType<PurificationFireball>();
            Item.shootSpeed = 20f;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
                return true;
            var wp = player.GetModPlayer<WikiWeaponsACPlayer>();
            if (wp.SupernovaCooldown <= 0)
            {
                wp.SupernovaCooldown = 7200;
                var p = Projectile.NewProjectileDirect(source, Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<Blast>(), damage * 5, 10f, player.whoAmI, 480f, DustID.GoldFlame, BuffID.BetsysCurse);
                p.DamageType = DamageClass.Magic;
                SoundEngine.PlaySound(SoundID.Item122, Main.MouseWorld);
            }
            else
                Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<LightPillar>(), (int)(damage * 0.75f), 0f, player.whoAmI);
            return false;
        }
    }

    public class WikiWeaponsACPlayer : ModPlayer
    {
        public int SupernovaCooldown;

        public override void PostUpdate()
        {
            if (SupernovaCooldown > 0)
                SupernovaCooldown--;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Summon
{
    /// <summary>
    /// Wiki : Consequences of Power.txt — amélioration finale du Black Hawk Remote : un avion (PW-MK1) qui alterne missiles
    /// chercheurs en grand nombre, rafales de railgun qui percent, et ruées vers l'ennemi le plus proche en posant des mines
    /// à aura. [choix] 4 s par mode ; 4 missiles (50 %) toutes les 20 images ; railgun (150 %) 3 tirs toutes les 40 images ;
    /// une mine (aura de 120 px, 30 % toutes les 10 images, 3 s) toutes les 30 images de ruée ; 1 emplacement de sbire.
    /// </summary>
    public class ConsequencesofPower : WikiMinionStaff
    {
        protected override int Damage => 432;
        protected override int Rarity => ModContent.RarityType<HotPink>();
        protected override int SellCopper => 400000;
        protected override int BuffType => ModContent.BuffType<Buffs.PWMK1Buff>();
        protected override int MinionType => ModContent.ProjectileType<PWMK1>();
        protected override float Knockback => 20f;
        protected override int UseTime => 9;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.mana = 21;
            Item.width = 48;
            Item.height = 36;
        }
    }
}

namespace CalamityFandom.Content.Buffs
{
    /// <summary>Terror of the Abyss (Gloomy Trident) : -50 % de réduction des dégâts (Calamity), -30 % de dégâts, -60 défense.</summary>
    public class TerroroftheAbyss : ModBuff
    {
        public override string Texture => "Terraria/Images/Buff_" + BuffID.Obstructed;

        public override void SetStaticDefaults() => Main.debuff[Type] = true;

        public override void Update(NPC npc, ref int buffIndex) => npc.GetGlobalNPC<TerrorNPC>().Terror = true;
    }

    public class TerrorNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public bool Terror;

        public override void ResetEffects(NPC npc) => Terror = false;

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (!Terror)
                return;
            modifiers.Defense.Flat -= 60f;
            float dr = npc.Calamity().DR;
            if (dr > 0f && dr < 1f)
                modifiers.FinalDamage *= (1f - dr / 2f) / (1f - dr);
        }

        public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
        {
            if (Terror)
                modifiers.SourceDamage *= 0.7f;
        }
    }

    public class PWMK1Buff : WikiMinionBuff
    {
        public override string Texture => "CalamityFandom/Content/Buffs/Wiki/PWMK1Buff";

        protected override int MinionType => ModContent.ProjectileType<PWMK1>();
    }
}

namespace CalamityFandom.Content.Projectiles.Weapons1119ac
{
    public class HSTridentProj : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Melee/HSTridentSpear";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.aiStyle = ProjAIStyleID.Spear;
            AIType = ProjectileID.Trident;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.hide = true;
            Projectile.scale = 0.6f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<CrushDepth>(), 120);
    }

    /// <summary>Rayon de lumière du H.S Trident : part du côté opposé, perce à l'infini, 1 s ; Crush Depth 2 s.</summary>
    public class HSLightBeam : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.LightBeam;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

        public override Color? GetAlpha(Color lightColor) => new Color(200, 240, 255, 100);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<CrushDepth>(), 120);
    }

    /// <summary>Geyser de flammes exo (XB-∞ Hekate) : colonne de 160 px au-dessus de l'ennemi, 1 s.</summary>
    public class ExoGeyser : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 160;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void OnSpawn(IEntitySource source) => Projectile.position -= new Vector2(0f, Projectile.height / 2f);

        public override void AI()
        {
            if (Main.dedServ)
                return;
            for (int i = 0; i < 3; i++)
                Dust.NewDustPerfect(Projectile.Bottom + new Vector2(Main.rand.NextFloat(-15f, 15f), 0f), DustID.RainbowMk2, new Vector2(0f, -Main.rand.NextFloat(6f, 10f)), 0,
                    Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.6f), 1.3f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<ExoFreeze>(), 30);
    }

    /// <summary>Perle noire du Gloomy Trident (50 %) : ralentit, Crush Depth 3 s.</summary>
    public class BlackPearl : ModProjectile
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.BlackPearl;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 90;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.98f;
            Projectile.rotation += 0.2f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<CrushDepth>(), 180);
    }

    public class GloomySpear : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Melee/GloomyTrident";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.aiStyle = ProjAIStyleID.Spear;
            AIType = ProjectileID.Trident;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.hide = true;
            Projectile.scale = 0.8f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<CrushDepth>(), 480);
    }

    /// <summary>
    /// Maelström du Gloomy Trident : ai[0] 0 = normal (2 s, avance doucement), 1 = mirage (5 s, aspire les ennemis, plus grand) ;
    /// un éclair (50 %, Electrified 2 s) sur un ennemi proche toutes les 15 images (5 en mirage).
    /// </summary>
    public class GloomMaelstrom : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Melee/GloomMaelstrom";

        private bool Mirage => Projectile.ai[0] == 1f;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 160;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Mirage)
            {
                Projectile.timeLeft = 300;
                Projectile.Resize(400, 400);
            }
        }

        public override void AI()
        {
            Projectile.velocity *= 0.97f;
            Projectile.rotation += Mirage ? 0.15f : 0.1f;
            if (Mirage)
                foreach (NPC n in Main.ActiveNPCs)
                    if (!n.friendly && !n.boss && n.knockBackResist > 0f && Vector2.Distance(n.Center, Projectile.Center) < 600f)
                        n.velocity = Vector2.Lerp(n.velocity, (Projectile.Center - n.Center).SafeNormalize(Vector2.Zero) * 8f, 0.08f);
            if (Projectile.owner != Main.myPlayer || ++Projectile.localAI[0] % 15 != 0)
                return;
            for (int i = 0; i < (Mirage ? 5 : 1); i++)
            {
                NPC n = Dart.Closest(Projectile.Center + Main.rand.NextVector2Circular(100f, 100f), Mirage ? 700f : 400f);
                if (n == null)
                    break;
                var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), n.Center, Vector2.Zero, ModContent.ProjectileType<Blast>(), Projectile.damage / 2, 0f,
                    Projectile.owner, 40f, DustID.Electric, BuffID.Electrified);
                p.DamageType = DamageClass.Melee;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            float scale = (Mirage ? 1.2f : 0.5f) * Math.Min(1f, Projectile.timeLeft / 20f);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, new Color(60, 80, 160, 120), Projectile.rotation, tex.Size() / 2f, scale, SpriteEffects.None);
            return false;
        }
    }

    /// <summary>Ligne du Stringstreaker : du joueur vers le curseur (ai[0] = écart), tant qu'on tire ; frappe toutes les 6 images.</summary>
    public class StringLine : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public const float Length = 800f;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.itemAnimation > 0 && owner.HeldItem.type == ModContent.ItemType<Items.Weapons.Ranged.Stringstreaker>())
                Projectile.timeLeft = 2;
            Projectile.Center = owner.MountedCenter;
            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 dir = (Main.MouseWorld - owner.MountedCenter).SafeNormalize(Vector2.UnitX).RotatedBy(Projectile.ai[0]);
                if (dir != Projectile.velocity)
                    Projectile.netUpdate = Projectile.localAI[0]++ % 10 == 0;
                Projectile.velocity = dir;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * Length, 6f, ref _);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(TextureAssets.MagicPixel.Value, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, 1, 1), new Color(150, 255, 230, 0) * 0.8f,
                Projectile.velocity.ToRotation(), new Vector2(0f, 0.5f), new Vector2(Length, 2f), SpriteEffects.None);
            return false;
        }
    }

    /// <summary>Balle très rapide du Stringstreaker (150 %).</summary>
    public class StringBullet : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.BulletHighVelocity;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 60;
            Projectile.extraUpdates = 4;
        }

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
    }

    /// <summary>
    /// Pulsation divine (Wrath of the Gods) : 20 s ; un anneau de 48 boules de feu (25 %) par seconde ; tous les 10 anneaux,
    /// hexagramme d'orbes (75 %) et rayon rouge (500 %) vers l'ennemi le plus proche.
    /// </summary>
    public class GodlyPulse : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Magic/GodlyPulse";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.friendly = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1200;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Projectile.rotation += 0.05f;
            Lighting.AddLight(Projectile.Center, 0.9f, 0.4f, 0.2f);
            if (Projectile.owner != Main.myPlayer || ++Projectile.ai[0] % 60 != 0)
                return;
            var source = Projectile.GetSource_FromThis();
            for (int i = 0; i < 48; i++)
                Projectile.NewProjectile(source, Projectile.Center, (MathHelper.TwoPi * i / 48f).ToRotationVector2() * 8f, ModContent.ProjectileType<TraitorousFireball>(),
                    Projectile.damage / 4, 2f, Projectile.owner);
            SoundEngine.PlaySound(SoundID.Item117 with { Volume = 0.5f }, Projectile.Center);
            if (++Projectile.ai[1] % 10 != 0)
                return;
            // Hexagramme : deux triangles d'orbes
            for (int t = 0; t < 2; t++)
                for (int s = 0; s < 3; s++)
                {
                    Vector2 a = (MathHelper.TwoPi * s / 3f + t * MathHelper.Pi / 3f).ToRotationVector2() * 160f;
                    Vector2 b = (MathHelper.TwoPi * (s + 1) / 3f + t * MathHelper.Pi / 3f).ToRotationVector2() * 160f;
                    for (int k = 0; k < 6; k++)
                    {
                        Vector2 point = Vector2.Lerp(a, b, k / 6f);
                        Projectile.NewProjectile(source, Projectile.Center + point, point.SafeNormalize(Vector2.UnitX) * 6f, ModContent.ProjectileType<TraitorousOrb>(),
                            (int)(Projectile.damage * 0.75f), 2f, Projectile.owner);
                    }
                }
            NPC target = Dart.Closest(Projectile.Center, 1600f);
            if (target != null)
                Projectile.NewProjectile(source, Projectile.Center, (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitX), ModContent.ProjectileType<TraitorousDeathray>(),
                    Projectile.damage * 5, 0f, Projectile.owner, target.whoAmI);
            SoundEngine.PlaySound(SoundID.Item122, Projectile.Center);
        }

        public static void Debuffs(NPC target, int seconds)
        {
            target.AddBuff(ModContent.BuffType<WhisperingDeath>(), seconds * 60);
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), seconds * 60);
        }
    }

    public class TraitorousFireball : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Magic/TraitorousFireball";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (++Projectile.ai[0] > 20f)
                Dart.Home(Projectile, Dart.Closest(Projectile.Center, 900f), 12f, 20f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => GodlyPulse.Debuffs(target, 3);
    }

    public class TraitorousOrb : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Magic/TraitorousOrb";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => GodlyPulse.Debuffs(target, 2);
    }

    /// <summary>Rayon rouge (Wrath of the Gods, 500 %) : suit l'ennemi ai[0] pendant 1 s ; Vulnerability Hex 4 s.</summary>
    public class TraitorousDeathray : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public const float Length = 1600f;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            NPC target = Main.npc[(int)Projectile.ai[0]];
            if (target.active)
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, (target.Center - Projectile.Center).SafeNormalize(Projectile.velocity), 0.1f).SafeNormalize(Vector2.UnitX);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * Length, 24f, ref _);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            GodlyPulse.Debuffs(target, 10);
            target.AddBuff(ModContent.BuffType<VulnerabilityHex>(), 240);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float width = 20f * Math.Min(1f, Projectile.timeLeft / 15f);
            Main.EntitySpriteDraw(TextureAssets.MagicPixel.Value, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, 1, 1), new Color(255, 40, 40, 0),
                Projectile.velocity.ToRotation(), new Vector2(0f, 0.5f), new Vector2(Length, width), SpriteEffects.None);
            return false;
        }
    }

    /// <summary>Boule de feu bleue (Grand Purification) : éclate en flammes persistantes.</summary>
    public class PurificationFireball : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            if (!Main.dedServ)
                for (int i = 0; i < 2; i++)
                    Dust.NewDustPerfect(Projectile.Center, DustID.BlueTorch, Main.rand.NextVector2Circular(1f, 1f), 100, default, 1.8f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Nightwither>(), 240);

        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<PurificationFlames>(), Projectile.damage, 0f, Projectile.owner);
        }
    }

    /// <summary>Flammes persistantes (Grand Purification) : 1 s, frappe toutes les 5 images, dégâts x1 → x2,5 ; Moon Burnt 4 s.</summary>
    public class PurificationFlames : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 120;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => modifiers.SourceDamage *= 1f + 1.5f * (60 - Projectile.timeLeft) / 60f;

        public override void AI()
        {
            if (!Main.dedServ)
                for (int i = 0; i < 4; i++)
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.BlueTorch, 0f, -1f, 100, default, 1.6f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Nightwither>(), 240);
    }

    /// <summary>Pilier de lumière au curseur (Grand Purification, 75 %) : 1 s.</summary>
    public class LightPillar : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 600;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float a = Math.Min(1f, Projectile.timeLeft / 20f);
            Main.EntitySpriteDraw(TextureAssets.MagicPixel.Value, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, 1, 1), new Color(255, 250, 200, 0) * a,
                0f, new Vector2(0.5f, 0.5f), new Vector2(Projectile.width, Projectile.height), SpriteEffects.None);
            return false;
        }
    }

    /// <summary>
    /// PW-MK1 (Consequences of Power) : alterne toutes les 4 s missiles (4 à 50 % toutes les 20 images), railgun (3 tirs à
    /// 150 % toutes les 40 images) et ruée vers l'ennemi en posant des mines (toutes les 30 images).
    /// </summary>
    public class PWMK1 : Weapons1119m.WikiMinion
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Summon/PWMK1";

        protected override int BuffType => ModContent.BuffType<Buffs.PWMK1Buff>();

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 30;
            Projectile.height = 40;
        }

        private int Mode => (int)(Projectile.ai[0] / 240f) % 3;

        public override bool? CanDamage() => Mode == 2 ? null : false;

        public override void AI()
        {
            if (!KeepAlive())
                return;
            Projectile.ai[0]++;
            NPC target = Projectile.OwnerMinionAttackTargetNPC ?? Dart.Closest(Owner.Center, 1200f);
            if (target == null)
            {
                Idle();
                Projectile.rotation = Projectile.velocity.X * 0.05f;
                return;
            }
            var source = Projectile.GetSource_FromThis();
            if (Mode == 2)
            {
                Dart.Home(Projectile, target, 22f, 8f);
                if (Projectile.owner == Main.myPlayer && Projectile.ai[0] % 30 == 0)
                    Projectile.NewProjectile(source, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<AerialMine>(), (int)(Projectile.damage * 0.3f), 0f, Projectile.owner);
            }
            else
            {
                Vector2 hover = target.Center + new Vector2(-Math.Sign(target.Center.X - Owner.Center.X) * 300f, -200f);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, (hover - Projectile.Center) * 0.05f, 0.1f);
                if (Projectile.owner == Main.myPlayer)
                {
                    Vector2 aim = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitX);
                    if (Mode == 0 && Projectile.ai[0] % 20 == 0)
                        for (int i = 0; i < 4; i++)
                            Projectile.NewProjectile(source, Projectile.Center, aim.RotatedBy(MathHelper.Lerp(-0.8f, 0.8f, i / 3f)) * 10f, ModContent.ProjectileType<MiniMissile>(),
                                Projectile.damage / 2, 1f, Projectile.owner);
                    if (Mode == 1 && Projectile.ai[0] % 40 < 9 && Projectile.ai[0] % 3 == 0)
                        Projectile.NewProjectile(source, Projectile.Center, aim * 30f, ModContent.ProjectileType<RailgunShot>(), (int)(Projectile.damage * 1.5f), 4f, Projectile.owner);
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }
    }

    public class MiniMissile : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Summon/ScorchedEarthRocket";

        public override void SetStaticDefaults() => ProjectileID.Sets.MinionShot[Type] = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 150;
            Projectile.tileCollide = false;
            Projectile.scale = 0.4f;
        }

        public override void AI()
        {
            if (++Projectile.ai[0] > 10f)
                Dart.Home(Projectile, Dart.Closest(Projectile.Center, 900f), 16f, 12f);
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnKill(int timeLeft)
        {
            if (!Main.dedServ)
                for (int i = 0; i < 8; i++)
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1.4f).velocity *= 2f;
        }
    }

    public class RailgunShot : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.BulletHighVelocity;

        public override void SetStaticDefaults() => ProjectileID.Sets.MinionShot[Type] = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.extraUpdates = 3;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        public override Color? GetAlpha(Color lightColor) => new Color(255, 80, 80, 150);
    }

    /// <summary>Mine aérienne EM : 3 s, aura de 120 px (30 %, toutes les 10 images).</summary>
    public class AerialMine : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Summon/CrimsonMine";

        public override void SetStaticDefaults() => ProjectileID.Sets.MinionShot[Type] = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 120;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 150), 0f, tex.Size() / 2f, 0.55f, SpriteEffects.None);
            return false;
        }
    }
}
