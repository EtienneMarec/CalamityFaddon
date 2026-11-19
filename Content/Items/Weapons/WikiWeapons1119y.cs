using System;
using CalamityFandom.Content.Buffs;
using CalamityFandom.Content.Projectiles.Weapons1119y;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
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

// Lot AD (19/11) : armes fabricables du wiki (recettes du wiki) et debuffs Noctactricity / Nightclear (pages du wiki).
// Raretés « C2 »/« C3 »/« C4 » → 0 (règle des raretés inventées).

namespace CalamityFandom.Content.Items.Weapons.Ranged
{
    /// <summary>
    /// Wiki : Ace's Low.txt — avec des balles de mousquet : une file de 3 piques (cartes) avec un léger délai, qui percent 3
    /// ennemis et disparaissent au 4e ; On Fire! 2 s. [choix] Autres munitions : tir normal, 3 balles en rafale.
    /// </summary>
    public class AcesLow : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 25;
            Item.DamageType = DamageClass.Ranged;
            Item.useAnimation = 24;
            Item.useTime = 8;
            Item.reuseDelay = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item11;
            Item.width = 52;
            Item.height = 28;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(silver: 20);
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.Bullet;
        }

        // Une seule munition par file de 3
        public override bool CanConsumeAmmo(Item ammo, Player player) => player.itemAnimation >= player.itemAnimationMax - 2;

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (type == ProjectileID.Bullet)
                type = ModContent.ProjectileType<Spades>();
        }

        public override Vector2? HoldoutOffset() => new Vector2(-4f, 0f);
    }

    /// <summary>
    /// Wiki : Skull Fracture.txt — lance des javelots lents qui accélèrent vite et ignorent l'invulnérabilité des ennemis ;
    /// avec assez de mana, tire aussi 3 à 6 lames de lumière en éventail irrégulier. [choix] 8 mana par tir de lames ; lames à
    /// 50 % ; javelot : frappe toutes les 4 images, perce 5 fois.
    /// </summary>
    public class SkullFracture : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 56;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = 86;
            Item.height = 22;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 2);
            Item.shoot = ModContent.ProjectileType<SkullJavelin>();
            Item.shootSpeed = 8f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity * 0.3f, type, damage, knockback, player.whoAmI);
            if (player.CheckMana(8, true))
            {
                int n = Main.rand.Next(3, 7);
                for (int i = 0; i < n; i++)
                    Projectile.NewProjectile(source, position, velocity.RotatedBy(Main.rand.NextFloat(-0.35f, 0.35f)) * Main.rand.NextFloat(1.2f, 1.8f),
                        ModContent.ProjectileType<LightBlade>(), damage / 2, 1f, player.whoAmI);
            }
            return false;
        }
    }

    /// <summary>
    /// Wiki : Sky Clearer.txt — tire lentement des balles chercheuses qui éclatent en 3 autres en touchant quoi que ce soit ;
    /// Nightclear, Holy Flames et Evercold 5 s. [choix] Éclats à 33 % qui ne se divisent plus ; la balle de l'arme remplace
    /// la munition.
    /// </summary>
    public class SkyClearer : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 3400;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 48;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 10f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item40;
            Item.width = 112;
            Item.height = 50;
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(platinum: 50);
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 30f;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
            => type = ModContent.ProjectileType<SkyClearerBullet>();

        public override Vector2? HoldoutOffset() => new Vector2(-20f, 0f);
    }

    /// <summary>
    /// Wiki : Arkflammator.txt — lance-flammes : faux violettes qui percent à l'infini ennemis et blocs et créent des arcs
    /// (Noctactricity 5 s) ; consomme toujours du gel. Pendant de l'Ifflamator.
    /// [choix] Cadence de 2 images comme l'Ifflamator (le wiki dit 1) ; un arc vers un ennemi proche (50 %) toutes les 20 images
    /// par faux ; faux de 1,5 s.
    /// </summary>
    public class Arkflammator : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 240;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 2;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item34;
            Item.width = 98;
            Item.height = 40;
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(platinum: 10);
            Item.shoot = ModContent.ProjectileType<ArkflammatorScythe>();
            Item.shootSpeed = 50f;
            Item.useAmmo = AmmoID.Gel;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<ArkflammatorScythe>();
            velocity *= 0.4f;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-16f, 0f);
    }
}

namespace CalamityFandom.Content.Items.Weapons.Summon
{
    /// <summary>
    /// Wiki : Angel's Thesis.txt — tourelle : une statue animée immobile qui tire toutes les 3 s des motifs de fragments de
    /// pierre noirs et blancs autour d'elle. [choix] 12 fragments en anneau, noirs et blancs en alternance, l'anneau tourne
    /// d'un demi-pas à chaque salve. [technique] Le buff « Animated Statue » (« Perfectly balanced ») n'est pas fait : les
    /// tourelles n'ont pas de buff.
    /// </summary>
    public class AngelsThesis : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.GamepadWholeScreenUseRange[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 37;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 10;
            Item.useTime = Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 7f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item44;
            Item.width = Item.height = 58;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(gold: 2, silver: 40);
            Item.sentry = true;
            Item.shoot = ModContent.ProjectileType<AnimatedStatue>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var p = Projectile.NewProjectileDirect(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI);
            p.originalDamage = Item.damage;
            player.UpdateMaxTurrets();
            return false;
        }
    }

    /// <summary>
    /// Wiki : Cored Line.txt — fouet : crée un noyau au bout, qui tire 4 lignes dans des directions au hasard ; Evercold 5 s.
    /// [choix] Le noyau dure 1 s ; lignes de 400 px à 50 %, tirées toutes ensemble à 0,25 s.
    /// </summary>
    public class CoredLine : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.DefaultToWhip(ModContent.ProjectileType<CoredLineWhip>(), 480, 12f, 4f, 12);
            Item.width = 46;
            Item.height = 36;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(gold: 75);
        }

        public override bool MeleePrefix() => true;
    }
}

namespace CalamityFandom.Content.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Driftorcher.txt — avec des torches dans l'inventaire, chaque coup en consomme une et lance 5 étincelles soumises
    /// à la gravité qui cherchent les ennemis (10 %, percent une fois, 1,5 s), toujours aux mêmes angles (de tout droit
    /// devant à tout droit en haut) ; le debuff dépend de la torche (On Fire! 6 s par défaut).
    /// [choix] Debuffs : torche maudite Cursed Inferno, ichor Ichor, glace Frostburn, démoniaque Shadowflame, os/ultrabrillante
    /// Confused, sinon On Fire!.
    /// </summary>
    public class Driftorcher : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 65;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 19;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 2f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = 68;
            Item.height = 66;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(silver: 40);
            Item.shoot = ModContent.ProjectileType<DriftorcherSpark>();
            Item.shootSpeed = 10f;
        }

        public override bool CanShoot(Player player) => FindTorch(player) >= 0;

        private static int FindTorch(Player player)
        {
            for (int i = 0; i < 58; i++)
                if (!player.inventory[i].IsAir && ItemID.Sets.Torches[player.inventory[i].type])
                    return i;
            return -1;
        }

        public static int DebuffFor(int torch) => torch switch
        {
            ItemID.CursedTorch => BuffID.CursedInferno,
            ItemID.IchorTorch => BuffID.Ichor,
            ItemID.IceTorch => BuffID.Frostburn,
            ItemID.DemonTorch => BuffID.ShadowFlame,
            ItemID.BoneTorch or ItemID.UltrabrightTorch => BuffID.Confused,
            _ => BuffID.OnFire,
        };

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int slot = FindTorch(player);
            if (slot < 0)
                return false;
            int torch = player.inventory[slot].type;
            if (--player.inventory[slot].stack <= 0)
                player.inventory[slot].TurnToAir();
            for (int i = 0; i < 5; i++)
            {
                // De tout droit devant (0) à tout droit en haut (90°)
                float angle = MathHelper.PiOver2 * i / 4f;
                Vector2 v = new Vector2(player.direction * (float)Math.Cos(angle), -(float)Math.Sin(angle)) * Item.shootSpeed;
                Projectile.NewProjectile(source, player.Center, v, type, Math.Max(1, damage / 10), 0f, player.whoAmI, DebuffFor(torch));
            }
            return false;
        }
    }

    /// <summary>Wiki : Grarble Spear.txt — lance pré-Hardmode à frappe automatique.</summary>
    public class GrarbleSpear : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.Spears[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 26;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 21;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = Item.noUseGraphic = true;
            Item.knockBack = 5.25f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = Item.height = 48;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 20);
            Item.shoot = ModContent.ProjectileType<GrarbleSpearProj>();
            Item.shootSpeed = 5f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;
    }

    /// <summary>
    /// Wiki : Gluelic Blade.txt — clic gauche : coup d'épée qui laisse des étoiles à 4 branches (3 s, 80 %, Evercold) ; clic
    /// droit (2 fois plus lent) : l'épée pointe vers le curseur et lance une onde de choc (200 %) ; l'épée fait alors 130 %.
    /// Evercold 5 s. [choix] Une étoile toutes les 4 images du coup ; estoc = projectile court devant le joueur.
    /// </summary>
    public class GluelicBlade : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 2400;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6.5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = Item.height = 66;
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(gold: 75);
            Item.shoot = ProjectileID.None;
            Item.shootSpeed = 50f;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            bool stab = player.altFunctionUse == 2;
            Item.useStyle = stab ? ItemUseStyleID.Rapier : ItemUseStyleID.Swing;
            Item.useTime = Item.useAnimation = stab ? 40 : 20;
            Item.noMelee = stab;
            Item.shoot = stab ? ModContent.ProjectileType<GluelicShockwave>() : ProjectileID.None;
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 dir = velocity.SafeNormalize(Vector2.UnitX * player.direction);
            Projectile.NewProjectile(source, position, dir * 20f, type, damage * 2, knockback, player.whoAmI);
            Projectile.NewProjectile(source, player.Center + dir * 50f, dir, ModContent.ProjectileType<GluelicThrust>(), (int)(damage * 1.3f), knockback, player.whoAmI);
            return false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (player.altFunctionUse != 2 && player.whoAmI == Main.myPlayer && player.itemAnimation % 4 == 0)
                Projectile.NewProjectile(player.GetSource_ItemUse(Item), hitbox.Center.ToVector2(), Vector2.Zero, ModContent.ProjectileType<GluelicStar>(),
                    (int)(player.GetWeaponDamage(Item) * 0.8f), 0f, player.whoAmI);
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Evercold>(), 300);
    }

    /// <summary>
    /// Wiki : Exterminator.txt — yoyo à portée et durée infinies, entouré d'anneaux (80 %) ; un anneau qui touche un ennemi
    /// crée des arcs électriques (50 %) qui sautent sur d'autres ennemis ; Noctactricity 5 s. [choix] 3 sauts de 300 px au
    /// plus ; « infinie » = 4000 px de portée.
    /// </summary>
    public class Exterminator : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.Yoyo[Type] = true;
            ItemID.Sets.GamepadExtraRange[Type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 666;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.useTime = Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = Item.noUseGraphic = true;
            Item.channel = true;
            Item.knockBack = 10f;
            Item.UseSound = SoundID.Item1;
            Item.width = 32;
            Item.height = 48;
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(platinum: 10);
            Item.shoot = ModContent.ProjectileType<ExterminatorYoyo>();
            Item.shootSpeed = 16f;
        }
    }

    /// <summary>
    /// Wiki : Chain Spear.txt — lance (Abyssal Flames 3 s, Burning Blood 2 s) ; clic droit : la lance est lancée au bout d'une
    /// chaîne et ramène l'ennemi touché ; clic gauche quand un ennemi ramené est à moins de 10 blocs : parade, explosion de
    /// sang et l'ennemi ne fait plus de dégâts de contact un moment.
    /// [choix] Explosion de sang à 150 % sur 120 px ; 2 s sans dégâts de contact ; un boss n'est pas ramené (la lance revient).
    /// </summary>
    public class ChainSpear : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.Spears[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 124;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = Item.noUseGraphic = true;
            Item.knockBack = 3f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = Item.height = 68;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.value = Item.sellPrice(silver: 40);
            Item.shoot = ModContent.ProjectileType<ChainSpearProj>();
            Item.shootSpeed = 10f;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            int chain = ModContent.ProjectileType<ChainSpearChain>();
            if (player.altFunctionUse == 2)
            {
                Item.shoot = chain;
                return player.ownedProjectileCounts[chain] < 1 && player.ownedProjectileCounts[ModContent.ProjectileType<ChainSpearProj>()] < 1;
            }
            Item.shoot = ModContent.ProjectileType<ChainSpearProj>();
            // Parade : un ennemi ramené à moins de 10 blocs
            foreach (Projectile p in Main.ActiveProjectiles)
                if (p.type == chain && p.owner == player.whoAmI && p.ai[0] >= 0f && Main.npc[(int)p.ai[0]].active
                    && Vector2.Distance(Main.npc[(int)p.ai[0]].Center, player.Center) < 160f)
                {
                    NPC npc = Main.npc[(int)p.ai[0]];
                    if (player.whoAmI == Main.myPlayer)
                        Projectile.NewProjectile(player.GetSource_ItemUse(Item), npc.Center, Vector2.Zero, ModContent.ProjectileType<BloodBurst>(),
                            (int)(player.GetWeaponDamage(Item) * 1.5f), 6f, player.whoAmI, npc.whoAmI);
                    p.Kill();
                    break;
                }
            return player.ownedProjectileCounts[Item.shoot] < 1 && player.ownedProjectileCounts[chain] < 1;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Rogue
{
    /// <summary>
    /// Wiki : Dissfusion Disk.txt — amélioration de l'Eradicator : un projectile rapide qui ralentit fort, puis se sépare en
    /// deux glaives qui tournent autour d'un noyau instable, qui finit par exploser (gros dégâts).
    /// [choix] Arrêt au bout de ~0,5 s ; glaives (50 %) qui tournent 1,5 s à 80 px ; explosion de 200 px à 300 % ; frappe
    /// furtive : 4 glaives et explosion de 320 px.
    /// </summary>
    public class DissfusionDisk : RogueWeapon
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = Item.height = 96;
            Item.damage = 1200;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.knockBack = 0f;
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<DissfusionCore>();
            Item.shootSpeed = 21f;
            Item.value = Item.sellPrice(platinum: 2);
            Item.rare = ItemRarityID.White;
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

namespace CalamityFandom.Content.Buffs
{
    /// <summary>
    /// Wiki : Noctactricity.txt — pas de régénération, -100 % de vitesse, défense et réduction des dégâts divisées par deux.
    /// [choix] Les boss ne sont pas immobilisés (vitesse divisée par deux).
    /// </summary>
    public class Noctactricity : ModBuff
    {
        public override string Texture => "CalamityFandom/Content/Buffs/Wiki/Noctactricity";

        public override void SetStaticDefaults() => Main.debuff[Type] = true;

        public override void Update(NPC npc, ref int buffIndex) => npc.GetGlobalNPC<WikiDebuffsNPC>().Noctactricity = true;
    }

    /// <summary>
    /// Wiki : Nightclear.txt — Confused et Obstructed ; peut rendre confus les ennemis qui y sont immunisés.
    /// [technique] Sur un ennemi, Obstructed n'a pas d'effet : seul Confused est appliqué (immunités ignorées).
    /// </summary>
    public class Nightclear : ModBuff
    {
        public override string Texture => "CalamityFandom/Content/Buffs/Wiki/Nightclear";

        public override void SetStaticDefaults() => Main.debuff[Type] = true;

        public override void Update(NPC npc, ref int buffIndex) => npc.confused = true;
    }

    public class WikiDebuffsNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public bool Noctactricity;
        public int NoContact;

        public override void ResetEffects(NPC npc)
        {
            Noctactricity = false;
            if (NoContact > 0)
                NoContact--;
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (Noctactricity && npc.lifeRegen > 0)
                npc.lifeRegen = 0;
        }

        public override void PostAI(NPC npc)
        {
            if (!Noctactricity)
                return;
            if (npc.boss)
                npc.position -= npc.velocity * 0.5f;
            else
            {
                npc.position -= npc.velocity;
                npc.velocity *= 0f;
            }
        }

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (!Noctactricity)
                return;
            modifiers.Defense *= 0.5f;
            float dr = npc.Calamity().DR;
            if (dr > 0f && dr < 1f)
                modifiers.FinalDamage *= (1f - dr / 2f) / (1f - dr);
        }

        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot) => NoContact <= 0;
    }
}

namespace CalamityFandom.Content.Projectiles.Weapons1119y
{
    /// <summary>Pique de l'Ace's Low : perce 3 ennemis, On Fire! 2 s.</summary>
    public class Spades : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Ranged/Spades";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 4;
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation();

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.OnFire, 120);
    }

    /// <summary>Statue animée (tourelle de l'Angel's Thesis) : 12 fragments en anneau toutes les 3 s.</summary>
    public class AnimatedStatue : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Summon/AnimatedStatue";

        public override void SetStaticDefaults() => ProjectileID.Sets.MinionTargettingFeature[Type] = true;

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 43;
            Projectile.sentry = true;
            Projectile.friendly = false;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.timeLeft = Projectile.SentryLifeTime;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
        }

        public override bool? CanDamage() => false;

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override void AI()
        {
            Projectile.velocity.X = 0f;
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.4f, 10f);
            if (Projectile.owner != Main.myPlayer || ++Projectile.ai[0] < 180f)
                return;
            Projectile.ai[0] = 0f;
            Projectile.ai[1] = 1f - Projectile.ai[1];
            SoundEngine.PlaySound(SoundID.Item69 with { Volume = 0.5f }, Projectile.Center);
            for (int i = 0; i < 12; i++)
            {
                float angle = MathHelper.TwoPi * (i + Projectile.ai[1] * 0.5f) / 12f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, angle.ToRotationVector2() * 9f, ModContent.ProjectileType<StatueFragment>(),
                    Projectile.damage, Projectile.knockBack, Projectile.owner, i % 2);
            }
        }
    }

    /// <summary>Fragment de pierre de la statue : ai[0] 0 noir, 1 blanc.</summary>
    public class StatueFragment : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.BoulderStaffOfEarth;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 90;
            Projectile.scale = 0.45f;
        }

        public override void AI() => Projectile.rotation += 0.25f;

        public override Color? GetAlpha(Color lightColor) => Projectile.ai[0] == 0f ? new Color(40, 40, 40) : new Color(240, 240, 240);
    }

    /// <summary>Javelot de la Skull Fracture : part lentement, accélère fort, frappe toutes les 4 images.</summary>
    public class SkullJavelin : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Ranged/SkullFracture";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 6;
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 4;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            if (Projectile.velocity.Length() < 22f)
                Projectile.velocity *= 1.06f;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, 0.8f, SpriteEffects.None);
            return false;
        }
    }

    /// <summary>Lame de lumière de la Skull Fracture.</summary>
    public class LightBlade : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.LightBeam;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 60;
            Projectile.alpha = 60;
        }

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
    }

    /// <summary>Balle de la Sky Clearer : cherche les ennemis, éclate en 3 (ai[0] = 1 : éclat).</summary>
    public class SkyClearerBullet : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.BulletHighVelocity;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Projectiles.WikiDarts.WikiDartProj.Home(Projectile, Projectiles.WikiDarts.WikiDartProj.Closest(Projectile.Center, 800f), Projectile.ai[0] == 0f ? 14f : 16f, 12f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (!Main.dedServ && Main.rand.NextBool(2))
                Dust.NewDustPerfect(Projectile.Center, DustID.WhiteTorch, Vector2.Zero, 100, new Color(200, 230, 255), 1f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Nightclear>(), 300);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 300);
            target.AddBuff(ModContent.BuffType<Evercold>(), 300);
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.ai[0] != 0f || Projectile.owner != Main.myPlayer)
                return;
            for (int i = 0; i < 3; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center - Projectile.velocity, Main.rand.NextVector2CircularEdge(8f, 8f),
                    Type, Projectile.damage / 3, Projectile.knockBack, Projectile.owner, 1f);
        }
    }

    /// <summary>Faux de l'Arkflammator : perce tout, 1,5 s ; un arc (50 %) vers un ennemi proche toutes les 20 images.</summary>
    public class ArkflammatorScythe : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Ranged/ArkflammatorScythe";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 90;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Projectile.rotation += 0.35f;
            Projectile.velocity *= 0.97f;
            Projectile.alpha = (int)MathHelper.Lerp(255, 0, Math.Min(1f, Projectile.timeLeft / 30f));
            Lighting.AddLight(Projectile.Center, 0.5f, 0.1f, 0.7f);
            if (Projectile.owner == Main.myPlayer && ++Projectile.ai[0] % 20 == 0)
            {
                NPC n = Projectiles.WikiDarts.WikiDartProj.Closest(Projectile.Center, 240f);
                if (n != null)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<NoctArc>(),
                        Projectile.damage / 2, 0f, Projectile.owner, n.whoAmI, 0f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Noctactricity>(), 300);
    }

    /// <summary>
    /// Arc électrique (Arkflammator, Exterminator) : frappe l'ennemi ai[0] et saute jusqu'à 3 fois (ai[1] = sauts faits) vers
    /// un ennemi à moins de 300 px ; Noctactricity 5 s. Dessiné comme une ligne brisée depuis son point de départ.
    /// </summary>
    public class NoctArc : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 12;
            Projectile.tileCollide = false;
        }

        public override bool? CanHitNPC(NPC target) => target.whoAmI == (int)Projectile.ai[0] ? null : false;

        public override void AI()
        {
            NPC target = Main.npc[(int)Projectile.ai[0]];
            if (!target.active)
            {
                Projectile.Kill();
                return;
            }
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                Vector2 from = Projectile.Center, to = target.Center;
                if (!Main.dedServ)
                    for (float t = 0f; t < 1f; t += 0.05f)
                        Dust.NewDustPerfect(Vector2.Lerp(from, to, t) + Main.rand.NextVector2Circular(6f, 6f), DustID.PurpleTorch, Vector2.Zero, 100, default, 1.3f).noGravity = true;
                Projectile.Center = to;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Noctactricity>(), 300);
            if (Projectile.owner != Main.myPlayer || Projectile.ai[1] >= 3f)
                return;
            NPC next = Projectiles.WikiDarts.WikiDartProj.Closest(target.Center, 300f, target.whoAmI);
            if (next != null)
            {
                var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, Type, Projectile.damage, 0f, Projectile.owner, next.whoAmI, Projectile.ai[1] + 1f);
                p.DamageType = Projectile.DamageType;
            }
        }
    }

    /// <summary>Fouet du Cored Line : crée un noyau au bout quand il est tendu ; Evercold 5 s.</summary>
    public class CoredLineWhip : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults() => ProjectileID.Sets.IsAWhip[Type] = true;

        public override void SetDefaults()
        {
            Projectile.DefaultToWhip();
            Projectile.WhipSettings.Segments = 20;
            Projectile.WhipSettings.RangeMultiplier = 1.3f;
        }

        public override void AI()
        {
            // Appelé avant l'IA de fouet de Terraria : ai[0] compte les images
            Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out int _, out float _);
            if (Projectile.owner == Main.myPlayer && Projectile.localAI[1] == 0f && Projectile.ai[0] >= timeToFlyOut * 0.5f)
            {
                Projectile.localAI[1] = 1f;
                var points = new System.Collections.Generic.List<Vector2>();
                Projectile.FillWhipControlPoints(Projectile, points);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), points[^1], Vector2.Zero, ModContent.ProjectileType<JewelCore>(),
                    Projectile.damage / 2, 0f, Projectile.owner);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
            target.AddBuff(ModContent.BuffType<Evercold>(), 300);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var points = new System.Collections.Generic.List<Vector2>();
            Projectile.FillWhipControlPoints(Projectile, points);
            Texture2D px = TextureAssets.MagicPixel.Value;
            for (int i = 0; i < points.Count - 1; i++)
            {
                Vector2 diff = points[i + 1] - points[i];
                Main.EntitySpriteDraw(px, points[i] - Main.screenPosition, new Rectangle(0, 0, 1, 1), new Color(120, 220, 255), diff.ToRotation(), new Vector2(0f, 0.5f),
                    new Vector2(diff.Length() + 1f, 3f), SpriteEffects.None);
            }
            return false;
        }
    }

    /// <summary>Noyau du Cored Line : 1 s ; à 0,25 s, 4 lignes de 400 px au hasard (50 % du fouet, Evercold).</summary>
    public class JewelCore : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Summon/JewelCore";

        private float[] angles;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.SummonMeleeSpeed;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.scale = 0.6f;
        }

        public override void AI()
        {
            Projectile.rotation += 0.1f;
            if (angles == null)
            {
                var rand = new Random(Projectile.identity * 7919 + (int)Main.GameUpdateCount);
                angles = new float[4];
                for (int i = 0; i < 4; i++)
                    angles[i] = (float)(rand.NextDouble() * MathHelper.TwoPi);
            }
        }

        private bool Firing => Projectile.timeLeft <= 45 && Projectile.timeLeft > 35;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (!Firing || angles == null)
                return false;
            float _ = 0f;
            foreach (float a in angles)
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + a.ToRotationVector2() * 400f, 8f, ref _))
                    return true;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Evercold>(), 300);

        public override bool PreDraw(ref Color lightColor)
        {
            if (Firing && angles != null)
            {
                Texture2D px = TextureAssets.MagicPixel.Value;
                foreach (float a in angles)
                    Main.EntitySpriteDraw(px, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, 1, 1), new Color(140, 230, 255, 0), a, new Vector2(0f, 0.5f),
                        new Vector2(400f, 5f), SpriteEffects.None);
            }
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None);
            return false;
        }
    }

    /// <summary>Étincelle du Driftorcher : gravité, cherche les ennemis, perce une fois, 1,5 s ; ai[0] = debuff (6 s).</summary>
    public class DriftorcherSpark : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 90;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.2f;
            NPC n = Projectiles.WikiDarts.WikiDartProj.Closest(Projectile.Center, 400f);
            if (n != null)
                Projectile.velocity += (n.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 0.5f;
            if (Projectile.velocity.Length() > 12f)
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 12f;
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.Torch, Vector2.Zero, 100, default, 1.3f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff((int)Projectile.ai[0], 360);
    }

    public class GrarbleSpearProj : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Melee/GrarbleSpear";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 18;
            Projectile.aiStyle = ProjAIStyleID.Spear;
            AIType = ProjectileID.Trident;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.hide = true;
        }
    }

    /// <summary>Étoile à 4 branches de la Gluelic Blade : immobile 3 s, frappe toutes les 30 images, Evercold 5 s.</summary>
    public class GluelicStar : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.FairyQueenMagicItemShot;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        public override void AI()
        {
            Projectile.rotation += 0.05f;
            Projectile.alpha = Projectile.timeLeft < 30 ? (int)MathHelper.Lerp(255, 0, Projectile.timeLeft / 30f) : 0;
            Lighting.AddLight(Projectile.Center, 0.3f, 0.6f, 0.9f);
        }

        public override Color? GetAlpha(Color lightColor) => new Color(150, 230, 255, 0) * (1f - Projectile.alpha / 255f);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Evercold>(), 300);
    }

    /// <summary>Onde de choc de la Gluelic Blade (200 %).</summary>
    public class GluelicShockwave : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.IceSickle;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.scale = 1f + (60 - Projectile.timeLeft) / 60f;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(150, 230, 255, 100);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Evercold>(), 300);
    }

    /// <summary>Estoc de la Gluelic Blade (130 %) : zone invisible devant le joueur pendant l'estoc.</summary>
    public class GluelicThrust : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 15;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            Projectile.Center = owner.Center + Projectile.velocity * 50f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Evercold>(), 300);
    }

    /// <summary>Yoyo de l'Exterminator : portée « infinie », anneaux autour de lui.</summary>
    public class ExterminatorYoyo : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Melee/ExterminatorYoyo";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Type] = -1f;
            ProjectileID.Sets.YoyosMaximumRange[Type] = 4000f;
            ProjectileID.Sets.YoyosTopSpeed[Type] = 20f;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.aiStyle = ProjAIStyleID.Yoyo;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = -1;
        }

        public override void PostAI()
        {
            int rings = ModContent.ProjectileType<ExterminatorRings>();
            if (Projectile.owner == Main.myPlayer && Main.player[Projectile.owner].ownedProjectileCounts[rings] < 1)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, rings, (int)(Projectile.damage * 0.8f), 0f, Projectile.owner, Projectile.whoAmI);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Noctactricity>(), 300);
    }

    /// <summary>Anneaux de l'Exterminator (80 %) : suivent le yoyo ai[0] ; au contact, un arc (50 % du yoyo) vers un autre ennemi.</summary>
    public class ExterminatorRings : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Melee/ExterminatorRings";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 10;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            Projectile yoyo = Main.projectile[(int)Projectile.ai[0]];
            if (!yoyo.active || yoyo.type != ModContent.ProjectileType<ExterminatorYoyo>())
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 2;
            Projectile.Center = yoyo.Center;
            Projectile.rotation += 0.08f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Noctactricity>(), 300);
            if (Projectile.owner != Main.myPlayer)
                return;
            NPC next = Projectiles.WikiDarts.WikiDartProj.Closest(target.Center, 300f, target.whoAmI);
            if (next != null)
            {
                var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<NoctArc>(),
                    (int)(Projectile.damage * 0.625f), 0f, Projectile.owner, next.whoAmI, 0f);
                p.DamageType = DamageClass.MeleeNoSpeed;
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 120);
    }

    /// <summary>Coup de lance de la Chain Spear : Abyssal Flames 3 s, Burning Blood 2 s.</summary>
    public class ChainSpearProj : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Melee/ChainSpear";

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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => ChainSpearChain.Debuffs(target);
    }

    /// <summary>
    /// Lance lancée au bout d'une chaîne (clic droit) : ai[0] = ennemi accroché (-1 aucun), ai[1] = 1 au retour. Ramène
    /// l'ennemi vers le joueur ; revient seule sans cible au bout de 25 images.
    /// </summary>
    public class ChainSpearChain : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Melee/ChainSpear";

        public static void Debuffs(NPC target)
        {
            target.AddBuff(ModContent.BuffType<AbyssalFlames>(), 180);
            target.AddBuff(ModContent.BuffType<BurningBlood>(), 120);
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 240;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.extraUpdates = 1;
            Projectile.scale = 0.8f;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.ai[0] = -1f;
            Projectile.velocity *= 2f;
        }

        public override bool? CanHitNPC(NPC target) => Projectile.ai[0] >= 0f || Projectile.ai[1] == 1f ? false : null;

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.dead || !owner.active)
            {
                Projectile.Kill();
                return;
            }
            Vector2 toOwner = owner.Center - Projectile.Center;
            if (Projectile.ai[0] >= 0f)
            {
                NPC npc = Main.npc[(int)Projectile.ai[0]];
                if (!npc.active || toOwner.Length() < 40f)
                {
                    Projectile.Kill();
                    return;
                }
                Projectile.velocity = toOwner.SafeNormalize(Vector2.Zero) * 8f;
                npc.Center = Projectile.Center;
                npc.velocity = Vector2.Zero;
            }
            else
            {
                if (++Projectile.localAI[0] > 50f)
                    Projectile.ai[1] = 1f;
                if (Projectile.ai[1] == 1f)
                {
                    Projectile.velocity = toOwner.SafeNormalize(Vector2.Zero) * 12f;
                    if (toOwner.Length() < 30f)
                        Projectile.Kill();
                }
            }
            Projectile.rotation = (-toOwner).ToRotation() + MathHelper.PiOver4;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Debuffs(target);
            if (target.boss || target.realLife >= 0 || target.knockBackResist == 0f)
                Projectile.ai[1] = 1f;
            else
                Projectile.ai[0] = target.whoAmI;
            Projectile.netUpdate = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player owner = Main.player[Projectile.owner];
            Vector2 from = owner.MountedCenter, to = Projectile.Center;
            Texture2D chain = TextureAssets.Chain.Value;
            float rot = (to - from).ToRotation() - MathHelper.PiOver2;
            for (float d = 0f; d < Vector2.Distance(from, to); d += chain.Height)
            {
                Vector2 pos = Vector2.Lerp(from, to, d / Vector2.Distance(from, to));
                Main.EntitySpriteDraw(chain, pos - Main.screenPosition, null, Lighting.GetColor(pos.ToTileCoordinates(), new Color(200, 40, 40)), rot, chain.Size() / 2f, 1f, SpriteEffects.None);
            }
            return true;
        }
    }

    /// <summary>Explosion de sang de la parade de la Chain Spear (150 %, 120 px) ; l'ennemi ai[0] ne fait plus de dégâts de contact 2 s.</summary>
    public class BloodBurst : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 120;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
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
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (npc.active)
                npc.GetGlobalNPC<WikiDebuffsNPC>().NoContact = 120;
            SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);
            if (!Main.dedServ)
                for (int i = 0; i < 40; i++)
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0f, 0f, 0, default, 1.8f).velocity *= 3f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => ChainSpearChain.Debuffs(target);
    }

    /// <summary>
    /// Noyau du Dissfusion Disk : ralentit vite ; arrêté, deux glaives (quatre en frappe furtive) tournent autour 1,5 s, puis
    /// le noyau explose (300 %).
    /// </summary>
    public class DissfusionCore : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Rogue/DissfusionDisk";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = -1;
            Projectile.timeLeft = 150;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.scale = 0.5f;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.9f;
            Projectile.rotation += 0.3f;
            if (Projectile.timeLeft == 120 && Projectile.owner == Main.myPlayer)
            {
                int n = Projectile.Calamity().stealthStrike ? 4 : 2;
                for (int i = 0; i < n; i++)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<DissfusionGlaive>(),
                        Projectile.damage / 2, 0f, Projectile.owner, Projectile.whoAmI, MathHelper.TwoPi * i / n);
            }
            if (!Main.dedServ && Main.rand.NextBool(2))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, 0f, 0f, 100, default, 1f).noGravity = true;
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner != Main.myPlayer)
                return;
            float size = Projectile.Calamity().stealthStrike ? 320f : 200f;
            var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.WikiDarts.DartBlast>(),
                Projectile.damage * 3, 8f, Projectile.owner, size, DustID.Electric, 0f);
            p.DamageType = Projectile.DamageType;
        }
    }

    /// <summary>Glaive du Dissfusion Disk : tourne à 80 px autour du noyau ai[0], angle de départ ai[1].</summary>
    public class DissfusionGlaive : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Rogue/DissfusionDisk";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.scale = 0.35f;
        }

        public override void AI()
        {
            Projectile core = Main.projectile[(int)Projectile.ai[0]];
            if (!core.active || core.type != ModContent.ProjectileType<DissfusionCore>())
            {
                Projectile.Kill();
                return;
            }
            Projectile.ai[1] += 0.2f;
            Projectile.Center = core.Center + Projectile.ai[1].ToRotationVector2() * 80f;
            Projectile.rotation += 0.5f;
        }
    }
}
