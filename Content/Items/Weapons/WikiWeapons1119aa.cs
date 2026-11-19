using System;
using CalamityFandom.Content.Buffs;
using CalamityFandom.Content.Projectiles.Weapons1119aa;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Weapons.Rogue;
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

// Lot AF (19/11) : armes fabricables du wiki (recettes du wiki) et debuffs Wither et Shred. Raretés « C2 » → 0.

namespace CalamityFandom.Content.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Meridian.txt — une lance qui tourne et va vers le curseur en lâchant des étoiles (20 %) ; à chaque coup, des
    /// essences du vide jaillissent de l'ennemi et une rangée d'épées tombe du ciel. [choix] Lance tenue (clic maintenu), une
    /// étoile toutes les 10 images ; 4 essences à 30 % ; 5 épées à 50 % (une fois toutes les 20 images au plus).
    /// </summary>
    public class Meridian : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 3600;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = Item.noUseGraphic = true;
            Item.channel = true;
            Item.knockBack = 6f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = Item.height = 146;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.value = Item.sellPrice(gold: 40);
            Item.shoot = ModContent.ProjectileType<MeridianSpear>();
            Item.shootSpeed = 13f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;
    }

    /// <summary>
    /// Wiki : Stratoctrine Spear.txt — lance : chaque coup crée 2 lames volantes (3 s) qui foncent sur les ennemis ; Moon
    /// Burnt (Nightwither) 3 s. [choix] Lames à 50 %.
    /// </summary>
    public class StratoctrineSpear : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.Spears[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 340;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = Item.noUseGraphic = true;
            Item.knockBack = 12f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = 72;
            Item.height = 78;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.sellPrice(gold: 30);
            Item.shoot = ModContent.ProjectileType<StratoctrineSpearProj>();
            Item.shootSpeed = 14f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            for (int i = -1; i <= 1; i += 2)
                Projectile.NewProjectile(source, position, velocity.RotatedBy(i * 0.6f), ModContent.ProjectileType<StctrineBlade>(), damage / 2, 2f, player.whoAmI);
            return false;
        }
    }

    /// <summary>
    /// Wiki : Tendon Tides.txt — lance des disques qui percent 10 fois puis éclatent (débris sans dégâts) ; Shred 6 s (10 %),
    /// qui se cumule à chaque coup critique, jusqu'à 64 fois. [choix] Shred : 5 PV/s par cumul ; un coup critique ajoute
    /// toujours un cumul.
    /// </summary>
    public class TendonTides : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 240;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = Item.noUseGraphic = true;
            Item.knockBack = 5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = 110;
            Item.height = 108;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.value = Item.sellPrice(gold: 48);
            Item.shoot = ModContent.ProjectileType<TendonDisk>();
            Item.shootSpeed = 9f;
        }
    }

    /// <summary>
    /// Wiki : Twisted Netherite Sword.txt — épée : un ennemi qui n'est pas un boss, touché alors qu'aucun boss n'est en vie,
    /// tombe en cendres et meurt sur le coup (avec son butin). [choix] « use 1.6 » = 1,6 coup par seconde comme l'épée en
    /// netherite de Minecraft (38 images) ; les segments de ver et les PNJ amicaux ne sont pas concernés.
    /// </summary>
    public class TwistedNetheriteSword : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 2031;

        public override void SetDefaults()
        {
            Item.damage = 8;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 38;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 1f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = 46;
            Item.height = 60;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.value = Item.sellPrice(gold: 3);
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.boss || target.realLife >= 0 || target.friendly || target.life <= 0 || CalamityMod.CalPlayer.CalamityPlayer.areThereAnyDamnBosses)
                return;
            if (!Main.dedServ)
                for (int i = 0; i < 30; i++)
                    Dust.NewDustDirect(target.position, target.width, target.height, DustID.Ash, 0f, -1f, 0, default, 1.5f);
            player.ApplyDamageToNPC(target, target.life + target.defense * 2 + 10, 0f, 0, false);
        }
    }

    /// <summary>
    /// Wiki : Wind Turbine Blade.txt — estoc vers le curseur ; après 3 estocs, 1 s d'attente ; un clic droit pendant
    /// l'attente : grand coup (lame +200 %) et une tornade chercheuse qui ignore l'invulnérabilité. [choix] Tornade de 3 s
    /// à 100 %, qui frappe toutes les 5 images.
    /// </summary>
    public class WindTurbineBlade : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 90;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = Item.noUseGraphic = true;
            Item.knockBack = 2f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = 102;
            Item.height = 100;
            Item.rare = ItemRarityID.Cyan;
            Item.value = Item.sellPrice(gold: 10);
            Item.shoot = ModContent.ProjectileType<WindTurbineThrust>();
            Item.shootSpeed = 5f;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            var mp = player.GetModPlayer<WikiWeaponsAAPlayer>();
            if (player.altFunctionUse == 2)
            {
                if (mp.TurbineCooldown <= 0)
                    return false;
                mp.TurbineCooldown = 0;
                mp.TurbineStabs = 0;
                Item.useStyle = ItemUseStyleID.Swing;
                Item.noMelee = Item.noUseGraphic = false;
                Item.useTime = Item.useAnimation = 24;
                Item.shoot = ModContent.ProjectileType<TurbineTornado>();
                return true;
            }
            if (mp.TurbineCooldown > 0)
                return false;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = Item.noUseGraphic = true;
            Item.useTime = Item.useAnimation = 10;
            Item.shoot = ModContent.ProjectileType<WindTurbineThrust>();
            if (++mp.TurbineStabs >= 3)
            {
                mp.TurbineStabs = 0;
                mp.TurbineCooldown = 60 + Item.useAnimation;
            }
            return true;
        }

        public override void ModifyItemScale(Player player, ref float scale)
        {
            if (player.altFunctionUse == 2 && player.itemAnimation > 0)
                scale *= 3f;
        }
    }

    /// <summary>
    /// Wiki : Xenon Cutlass.txt — épée qui lance un flot d'entailles bleu clair (~40 blocs), qui traversent les blocs et
    /// percent à l'infini ; Holy Flames 3 s. [choix] Entailles à 100 %.
    /// </summary>
    public class XenonCutlass : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 112;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 2.25f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = 50;
            Item.height = 120;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.value = Item.sellPrice(gold: 20);
            Item.shoot = ModContent.ProjectileType<XenonSlash>();
            Item.shootSpeed = 13f;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
    }
}

namespace CalamityFandom.Content.Items.Weapons.Ranged
{
    /// <summary>
    /// Wiki : Overloaded Combatant Blaster.txt — balles à très grande vitesse, 15° d'écart ; un tir d'onyx toutes les 10
    /// balles ; Nightclear, Holy Flames, Evercold 5 s. [choix] Cadence de 2 images (le wiki dit 1) ; tir d'onyx à 300 %,
    /// explosion de 96 px.
    /// </summary>
    public class OverloadedCombatantBlaster : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 200;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 2;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item11 with { Volume = 0.4f };
            Item.width = 112;
            Item.height = 50;
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(platinum: 50);
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 30f;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var mp = player.GetModPlayer<Melee.WikiWeaponsAAPlayer>();
            Projectile.NewProjectile(source, position, velocity.RotatedByRandom(MathHelper.ToRadians(7.5f)), ModContent.ProjectileType<OverloadedBullet>(), damage, knockback, player.whoAmI);
            if (++mp.BlasterShots >= 10)
            {
                mp.BlasterShots = 0;
                Projectile.NewProjectile(source, position, velocity * 0.6f, ModContent.ProjectileType<OnyxBlast>(), damage * 3, knockback, player.whoAmI);
            }
            return false;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-20f, 0f);
    }

    /// <summary>
    /// Wiki : Revolvin' Mc'Coy.txt — 6 tirs puis il faut recharger ; tirs rapprochés de plus en plus forts (x1,2 ; 1,4 ; 1,8 ;
    /// 2,2 ; 2,8 ; 3,5) ; les balles deviennent des balles-cactus qui éclatent en épines (45 %) ; les 2 dernières sont en
    /// pierre : Armor Crunch 5 s, éclatent en rochers (150 %). [choix] Rechargement : clic droit ou automatique 1,5 s après
    /// le 6e tir ; « rapprochés » = moins d'1 s entre deux tirs, sinon la série repart de x1,2 ; 4 épines / 3 rochers.
    /// </summary>
    public class RevolvinMcCoy : ModItem
    {
        private static readonly float[] Multipliers = { 1.2f, 1.4f, 1.8f, 2.2f, 2.8f, 3.5f };

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 865;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item41;
            Item.width = 56;
            Item.height = 36;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.value = Item.sellPrice(platinum: 10);
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 20f;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            var mp = player.GetModPlayer<Melee.WikiWeaponsAAPlayer>();
            if (player.altFunctionUse == 2)
            {
                if (mp.McCoyShots > 0 && mp.McCoyReload <= 0)
                {
                    mp.McCoyReload = 60;
                    SoundEngine.PlaySound(SoundID.Item149, player.Center);
                }
                return false;
            }
            return mp.McCoyReload <= 0 && mp.McCoyShots < 6;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var mp = player.GetModPlayer<Melee.WikiWeaponsAAPlayer>();
            if (mp.McCoyChain <= 0)
                mp.McCoyStreak = 0;
            float mult = Multipliers[Math.Min(mp.McCoyStreak, 5)];
            bool stone = mp.McCoyShots >= 4;
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<CactusBullet>(), (int)(damage * mult), knockback, player.whoAmI, stone ? 1f : 0f);
            mp.McCoyShots++;
            mp.McCoyStreak++;
            mp.McCoyChain = 60;
            if (mp.McCoyShots >= 6)
                mp.McCoyReload = 90;
            return false;
        }
    }

    /// <summary>
    /// Wiki : Sister's Toy Rifle.txt — balles en mousse qui gardent l'effet de la balle utilisée ; recharge après 6 tirs ;
    /// après une recharge, les 3 premiers tirs rebondissent sur 5 ennemis au plus ; 30 % de chances de ne pas consommer la
    /// munition. [technique] La balle tirée est la vraie munition (pour garder son effet), dessinée en mousse.
    /// [choix] Recharge de 1,5 s.
    /// </summary>
    public class SistersToyRifle : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 48;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item11;
            Item.width = 136;
            Item.height = 64;
            Item.scale = 0.6f;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 1, silver: 50);
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 10f;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player) => !Main.rand.NextBool(10, 33);

        public override bool CanUseItem(Player player) => player.GetModPlayer<Melee.WikiWeaponsAAPlayer>().ToyReload <= 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var mp = player.GetModPlayer<Melee.WikiWeaponsAAPlayer>();
            var p = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            var foam = p.GetGlobalProjectile<ToyRifleFoam>();
            foam.Foam = true;
            if (mp.ToyShots < 3)
            {
                foam.Bounces = 5;
                p.penetrate = Math.Max(p.penetrate, 1) + 5;
                p.usesLocalNPCImmunity = true;
                p.localNPCHitCooldown = -1;
            }
            if (++mp.ToyShots >= 6)
            {
                mp.ToyShots = 0;
                mp.ToyReload = 90;
            }
            return false;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-24f, 0f);
    }
}

namespace CalamityFandom.Content.Items.Weapons.Magic
{
    /// <summary>
    /// Wiki : Ram's Revenge.txt — baguette : un trait magique ; sur un ennemi, Wither 4 s (-20 défense) ; sur un ennemi ou un
    /// bloc, une explosion de sang (Burning Blood 1 s). [choix] Explosion de 80 px à 50 %.
    /// </summary>
    public class RamsRevenge : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 375;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 12;
            Item.useTime = Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 10f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item72;
            Item.width = Item.height = 48;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(gold: 20);
            Item.shoot = ModContent.ProjectileType<RamBolt>();
            Item.shootSpeed = 12f;
        }
    }

    /// <summary>
    /// Wiki : Shading Manual.txt — crée un orbe qui fait le tour du joueur en 2 s en tirant des rayons sur les ennemis, puis
    /// il faut relancer ; Nightclear, Holy Flames, Evercold 5 s. [choix] Un orbe à la fois ; un rayon toutes les 12 images.
    /// </summary>
    public class ShadingManual : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 440;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 35;
            Item.useTime = Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.knockBack = 0f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item8;
            Item.width = 40;
            Item.height = 56;
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(platinum: 50);
            Item.shoot = ModContent.ProjectileType<ShadingOrb>();
            Item.shootSpeed = 0f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;
    }
}

namespace CalamityFandom.Content.Items.Weapons.Rogue
{
    /// <summary>
    /// Wiki : Shadow's Descent.txt — boules d'ombre (5 au plus) qui ignorent la gravité et les blocs, ralentissent puis
    /// s'arrêtent ; arrêtées, elles lâchent des esprits chercheurs et couvrent une zone d'ombre aux dégâts croissants ;
    /// disparaissent après 50 coups ou longtemps ; clic droit : efface toutes les boules ; Shadowflame 10 s. Frappe furtive :
    /// la boule émet aussi des pulsations (Demon Flames 5 s, God Slayer Inferno 10 s, Nightwither 20 s, Shadowflame 30 s).
    /// [choix] 20 s de vie ; esprit (50 %) toutes les 30 images ; zone de 160 px à 10 % puis +10 % par seconde (jusqu'à 100 %) ;
    /// pulsation (100 %, 240 px) toutes les 60 images.
    /// </summary>
    public class ShadowsDescent : RogueWeapon
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = Item.height = 52;
            Item.damage = 212;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.knockBack = 4f;
            Item.useTime = Item.useAnimation = 19;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<ShadowBall>();
            Item.shootSpeed = 5f;
            Item.value = Item.sellPrice(gold: 40);
            Item.rare = ModContent.RarityType<HotPink>();
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                foreach (Projectile p in Main.ActiveProjectiles)
                    if (p.type == Item.shoot && p.owner == player.whoAmI)
                        p.Kill();
                return false;
            }
            return player.ownedProjectileCounts[Item.shoot] < 5;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (player.Calamity().StealthStrikeAvailable() && p.WithinBounds(Main.maxProjectiles))
                Main.projectile[p].Calamity().stealthStrike = true;
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Melee
{
    public class WikiWeaponsAAPlayer : ModPlayer
    {
        public int TurbineStabs, TurbineCooldown, BlasterShots, McCoyShots, McCoyStreak, McCoyChain, McCoyReload, ToyShots, ToyReload;

        public override void PostUpdate()
        {
            if (TurbineCooldown > 0)
                TurbineCooldown--;
            if (McCoyChain > 0)
                McCoyChain--;
            if (McCoyReload > 0 && --McCoyReload == 0)
                McCoyShots = 0;
            if (ToyReload > 0)
                ToyReload--;
        }
    }
}

namespace CalamityFandom.Content.Buffs
{
    /// <summary>
    /// Wither (Ram's Revenge ; la page « Wither » du wiki redirige ailleurs) : d'après les infobulles « Withered... Defense
    /// reduced by 20 » et « Losing life... » : -20 défense et 4 PV/s. [choix]
    /// </summary>
    public class Wither : ModBuff
    {
        public override string Texture => "CalamityFandom/Content/Buffs/Wiki/Wither";

        public override void SetStaticDefaults() => Main.debuff[Type] = true;

        public override void Update(NPC npc, ref int buffIndex) => npc.GetGlobalNPC<WikiDebuffsAANPC>().Wither = true;
    }

    /// <summary>Shred (Tendon Tides) : 5 PV/s par cumul, 64 cumuls au plus ; les cumuls tombent avec le debuff.</summary>
    public class TendonShred : ModBuff
    {
        public override string Texture => "CalamityFandom/Content/Buffs/Wiki/TendonShred";

        public override void SetStaticDefaults() => Main.debuff[Type] = true;

        public override void Update(NPC npc, ref int buffIndex)
        {
            var g = npc.GetGlobalNPC<WikiDebuffsAANPC>();
            g.Shredded = true;
            if (npc.buffTime[buffIndex] <= 1)
                g.ShredStacks = 0;
        }
    }

    public class WikiDebuffsAANPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public bool Wither, Shredded;
        public int ShredStacks;

        public override void ResetEffects(NPC npc) => Wither = Shredded = false;

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            int loss = (Wither ? 8 : 0) + (Shredded ? 10 * Math.Max(1, ShredStacks) : 0);
            if (loss == 0)
                return;
            if (npc.lifeRegen > 0)
                npc.lifeRegen = 0;
            npc.lifeRegen -= loss;
            damage = Math.Max(damage, loss / 8);
        }

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (Wither)
                modifiers.Defense.Flat -= 20f;
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Weapons1119aa
{
    /// <summary>Lance de Meridian : tenue, va vers le curseur en tournant, lâche des étoiles ; coups : essences et épées.</summary>
    public class MeridianSpear : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Melee/MeridianSpear";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 90;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
            Projectile.scale = 0.7f;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!owner.channel || owner.dead || owner.noItems || owner.CCed)
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 2;
            owner.itemTime = owner.itemAnimation = 2;
            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 want = (Main.MouseWorld - Projectile.Center) * 0.15f;
                if (want.Length() > 26f)
                    want = want.SafeNormalize(Vector2.Zero) * 26f;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, want, 0.3f);
                Projectile.netUpdate = Projectile.ai[1]++ % 10 == 0;
            }
            Projectile.rotation += 0.45f;
            owner.ChangeDir(Projectile.Center.X > owner.Center.X ? 1 : -1);
            if (Projectile.owner == Main.myPlayer && ++Projectile.ai[0] % 10 == 0)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2CircularEdge(6f, 6f), ModContent.ProjectileType<GalaxianStar>(),
                    Projectile.damage / 5, 1f, Projectile.owner);
            if (Projectile.localAI[0] > 0f)
                Projectile.localAI[0]--;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.owner != Main.myPlayer)
                return;
            var source = Projectile.GetSource_FromThis();
            for (int i = 0; i < 4; i++)
                Projectile.NewProjectile(source, target.Center, Main.rand.NextVector2CircularEdge(14f, 14f), ModContent.ProjectileType<VoidEssence>(), (int)(Projectile.damage * 0.3f), 1f, Projectile.owner);
            if (Projectile.localAI[0] > 0f)
                return;
            Projectile.localAI[0] = 20f;
            for (int i = -2; i <= 2; i++)
                Projectile.NewProjectile(source, target.Center + new Vector2(i * 70f, -600f), Vector2.UnitY * 24f, ModContent.ProjectileType<MeridianSword>(), Projectile.damage / 2, 2f, Projectile.owner);
        }
    }

    /// <summary>Étoile galaxienne de Meridian (20 %) : ralentit et cherche les ennemis.</summary>
    public class GalaxianStar : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Melee/GalaxianStar";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Dart.Home(Projectile, Dart.Closest(Projectile.Center, 600f), 14f, 18f);
            Projectile.rotation += 0.2f;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 150);
    }

    /// <summary>Essence du vide de Meridian (30 %) : file droit, perce 2 fois.</summary>
    public class VoidEssence : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Melee/VoidEssence";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.scale = 0.7f;
        }

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        public override Color? GetAlpha(Color lightColor) => new Color(200, 150, 255, 120);
    }

    /// <summary>Épée qui tombe du ciel (Meridian, 50 %) : traverse les blocs.</summary>
    public class MeridianSword : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Melee/Meridian";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.scale = 0.4f;
            Projectile.alpha = 80;
        }

        public override void AI() => Projectile.rotation = MathHelper.PiOver4 * 3f;
    }

    public class StratoctrineSpearProj : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Melee/StratoctrineSpear";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 22;
            Projectile.aiStyle = ProjAIStyleID.Spear;
            AIType = ProjectileID.Trident;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.hide = true;
            Projectile.scale = 0.8f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Nightwither>(), 180);
    }

    /// <summary>Lame volante de la Stratoctrine Spear : 3 s, fonce sur les ennemis, Nightwither 3 s.</summary>
    public class StctrineBlade : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Melee/StctrineBlade";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            if (++Projectile.ai[0] > 10f)
                Dart.Home(Projectile, Dart.Closest(Projectile.Center, 700f), 16f, 12f);
            Projectile.rotation += 0.3f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Nightwither>(), 180);
    }

    /// <summary>Disque de Tendon Tides : perce 10 fois puis éclate ; Shred 6 s (10 %, ou à chaque coup critique, qui cumule).</summary>
    public class TendonDisk : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Melee/TendonTides";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 10;
            Projectile.timeLeft = 180;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.scale = 0.5f;
        }

        public override void AI() => Projectile.rotation += 0.4f;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            var g = target.GetGlobalNPC<WikiDebuffsAANPC>();
            if (hit.Crit || Main.rand.NextBool(10))
            {
                target.AddBuff(ModContent.BuffType<TendonShred>(), 360);
                g.ShredStacks = Math.Min(64, g.ShredStacks + 1);
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (!Main.dedServ)
                for (int i = 0; i < 15; i++)
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0f, 0f, 0, default, 1.5f).velocity *= 2f;
        }
    }

    /// <summary>Estoc de la Wind Turbine Blade : la lame part vers le curseur et revient (10 images).</summary>
    public class WindTurbineThrust : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Melee/WindTurbineBlade";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 10;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ownerHitCheck = true;
            Projectile.scale = 0.6f;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            float t = 1f - Math.Abs(Projectile.timeLeft - 5f) / 5f;
            Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.UnitX * owner.direction);
            Projectile.Center = owner.MountedCenter + dir * (20f + 60f * t);
            Projectile.rotation = dir.ToRotation() + MathHelper.PiOver4;
            owner.ChangeDir(dir.X > 0 ? 1 : -1);
            owner.heldProj = Projectile.whoAmI;
        }
    }

    /// <summary>Tornade de la Wind Turbine Blade : 3 s, cherche les ennemis, frappe toutes les 5 images.</summary>
    public class TurbineTornado : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 80;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
        }

        public override void AI()
        {
            Dart.Home(Projectile, Dart.Closest(Projectile.Center, 700f), 8f, 20f);
            if (!Main.dedServ)
                for (int i = 0; i < 3; i++)
                {
                    float h = Main.rand.NextFloat();
                    Vector2 pos = Projectile.Bottom - new Vector2((float)Math.Sin(Main.GlobalTimeWrappedHourly * 12f + h * 10f) * 20f * h, h * 80f);
                    Dust.NewDustPerfect(pos, DustID.Cloud, Vector2.Zero, 100, default, 1.2f).noGravity = true;
                }
        }
    }

    /// <summary>Entaille du Xenon Cutlass : traverse les blocs, perce à l'infini, ~40 blocs ; Holy Flames 3 s.</summary>
    public class XenonSlash : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.LightBeam;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 50;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

        public override Color? GetAlpha(Color lightColor) => new Color(120, 220, 255, 100);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
    }

    public class OverloadedBullet : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.BulletHighVelocity;

        public static void Debuffs(NPC target)
        {
            target.AddBuff(ModContent.BuffType<Nightclear>(), 300);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 300);
            target.AddBuff(ModContent.BuffType<Evercold>(), 300);
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.extraUpdates = 2;
        }

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => Debuffs(target);
    }

    /// <summary>Tir d'onyx de l'Overloaded Combatant Blaster (300 %) : explose sur 96 px.</summary>
    public class OnyxBlast : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.Shadowflame, Vector2.Zero, 100, default, 1.6f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => OverloadedBullet.Debuffs(target);

        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner != Main.myPlayer)
                return;
            var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<Blast>(), Projectile.damage / 2, 0f,
                Projectile.owner, 96f, DustID.Shadowflame, ModContent.BuffType<Nightclear>());
            p.DamageType = DamageClass.Ranged;
        }
    }

    /// <summary>Balle-cactus du Revolvin' Mc'Coy : ai[0] 1 = pierre (Armor Crunch 5 s, 3 rochers à 150 %), sinon 4 épines à 45 %.</summary>
    public class CactusBullet : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.BulletHighVelocity;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.extraUpdates = 1;
        }

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        public override Color? GetAlpha(Color lightColor) => Projectile.ai[0] == 1f ? new Color(150, 150, 150) : new Color(100, 200, 80);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[0] == 1f)
                target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 300);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14 with { Volume = 0.5f }, Projectile.Center);
            if (Projectile.owner != Main.myPlayer)
                return;
            bool stone = Projectile.ai[0] == 1f;
            int n = stone ? 3 : 4;
            // Dégâts de l'arme (sans le multiplicateur de série) : le projectile porte déjà ce multiplicateur
            int baseDamage = Projectile.damage;
            for (int i = 0; i < n; i++)
            {
                var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2CircularEdge(8f, 8f),
                    ModContent.ProjectileType<CactusSpine>(), (int)(baseDamage * (stone ? 1.5f : 0.45f)), 2f, Projectile.owner, stone ? 1f : 0f);
                p.scale = stone ? 1.6f : 1f;
            }
        }
    }

    /// <summary>Épine (ai[0] = 0) ou rocher pointu (ai[0] = 1, dessiné gris et plus gros) du Revolvin' Mc'Coy.</summary>
    public class CactusSpine : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.PineNeedleFriendly;

        public override Color? GetAlpha(Color lightColor) => Projectile.ai[0] == 1f ? new Color(140, 140, 140) : null;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 60;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.15f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }
    }

    /// <summary>Balles du Sister's Toy Rifle : dessinées en mousse ; les 3 premières après une recharge rebondissent vers un autre ennemi.</summary>
    public class ToyRifleFoam : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public bool Foam;
        public int Bounces;

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Bounces <= 0)
                return;
            Bounces--;
            NPC next = Dart.Closest(projectile.Center, 500f, target.whoAmI);
            if (next != null)
                projectile.velocity = (next.Center - projectile.Center).SafeNormalize(Vector2.UnitX) * projectile.velocity.Length();
        }

        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            if (!Foam)
                return true;
            Texture2D tex = ModContent.Request<Texture2D>("CalamityFandom/Content/Projectiles/Ranged/FoamDart").Value;
            Main.EntitySpriteDraw(tex, projectile.Center - Main.screenPosition, null, lightColor, projectile.velocity.ToRotation(), tex.Size() / 2f, 1f, SpriteEffects.None);
            return false;
        }
    }

    /// <summary>Trait de Ram's Revenge : Wither 4 s ; explosion de sang (80 px, 50 %, Burning Blood 1 s) sur un ennemi ou un bloc.</summary>
    public class RamBolt : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.Blood, Vector2.Zero, 0, default, 1.4f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Wither>(), 240);

        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner != Main.myPlayer)
                return;
            var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<Blast>(), Projectile.damage / 2, 0f,
                Projectile.owner, 80f, DustID.Blood, ModContent.BuffType<BurningBlood>());
            p.DamageType = DamageClass.Magic;
        }
    }

    /// <summary>Orbe du Shading Manual : un tour du joueur en 2 s, un rayon toutes les 12 images.</summary>
    public class ShadingOrb : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Magic/ShadingOrb";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.friendly = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            float angle = MathHelper.TwoPi * (120 - Projectile.timeLeft) / 120f;
            Projectile.Center = owner.Center + angle.ToRotationVector2() * 90f;
            Lighting.AddLight(Projectile.Center, 0.4f, 0.3f, 0.7f);
            if (Projectile.owner != Main.myPlayer || Projectile.timeLeft % 12 != 0)
                return;
            NPC target = Dart.Closest(Projectile.Center, 900f);
            if (target != null)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitX) * 18f,
                    ModContent.ProjectileType<ShadingBeam>(), Projectile.damage, 0f, Projectile.owner);
        }
    }

    public class ShadingBeam : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 60;
            Projectile.extraUpdates = 3;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.PurpleTorch, Vector2.Zero, 100, default, 1.1f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => OverloadedBullet.Debuffs(target);
    }

    /// <summary>
    /// Boule d'ombre du Shadow's Descent : ralentit et s'arrête ; arrêtée : esprits (50 %) toutes les 30 images, zone de
    /// 160 px aux dégâts croissants, pulsations en frappe furtive ; 50 coups ou 20 s. Shadowflame 10 s.
    /// </summary>
    public class ShadowBall : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.ShadowFlame;

        private int hits;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1200;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
            => Projectile.ai[0] >= 60f ? Vector2.Distance(targetHitbox.Center.ToVector2(), Projectile.Center) < 80f : projHitbox.Intersects(targetHitbox);

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.ai[0] >= 60f)
                modifiers.SourceDamage *= Math.Min(1f, 0.1f + 0.1f * (Projectile.ai[0] - 60f) / 60f);
        }

        public override void AI()
        {
            Projectile.ai[0]++;
            if (Projectile.ai[0] < 60f)
                Projectile.velocity *= 0.96f;
            else
                Projectile.velocity = Vector2.Zero;
            if (!Main.dedServ)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.Shadowflame, Vector2.Zero, 100, default, 1.4f).noGravity = true;
                if (Projectile.ai[0] >= 60f)
                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(80f, 80f), DustID.Shadowflame, Vector2.Zero, 150, default, 1f).noGravity = true;
            }
            if (Projectile.ai[0] < 60f || Projectile.owner != Main.myPlayer)
                return;
            var source = Projectile.GetSource_FromThis();
            if (Projectile.ai[0] % 30 == 0)
                Projectile.NewProjectile(source, Projectile.Center, Main.rand.NextVector2CircularEdge(6f, 6f), ModContent.ProjectileType<ShadowSpirit>(), Projectile.damage / 2, 1f, Projectile.owner);
            if (Projectile.Calamity().stealthStrike && Projectile.ai[0] % 60 == 0)
                Projectile.NewProjectile(source, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ShadowPulse>(), Projectile.damage, 0f, Projectile.owner);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.ShadowFlame, 600);
            if (++hits >= 50)
                Projectile.Kill();
        }
    }

    /// <summary>Esprit d'ombre (50 %) : cherche les ennemis, Shadowflame 10 s.</summary>
    public class ShadowSpirit : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = 1;
            Projectile.timeLeft = 150;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Dart.Home(Projectile, Dart.Closest(Projectile.Center, 700f), 11f, 14f);
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.Shadowflame, Vector2.Zero, 100, default, 1.2f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.ShadowFlame, 600);
    }

    /// <summary>Pulsation sombre (frappe furtive) : 240 px ; Demon Flames 5 s, God Slayer Inferno 10 s, Nightwither 20 s, Shadowflame 30 s.</summary>
    public class ShadowPulse : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 240;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = -1;
            Projectile.timeLeft = 10;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            if (Projectile.localAI[0]++ == 0f && !Main.dedServ)
                for (int i = 0; i < 40; i++)
                    Dust.NewDustPerfect(Projectile.Center, DustID.Shadowflame, Main.rand.NextVector2CircularEdge(10f, 10f), 100, default, 1.6f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<DemonicFlames>(), 300);
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 600);
            target.AddBuff(ModContent.BuffType<Nightwither>(), 1200);
            target.AddBuff(BuffID.ShadowFlame, 1800);
        }
    }
}
