using System;
using System.Collections.Generic;
using System.Linq;
using CalamityFandom.Common.Debug;
using CalamityFandom.Common.WikiData;
using CalamityMod;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Rarities;
using CalamityFandom.Content.Projectiles.Weapons1119m;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

// Lot AP (20/11) : armes du wiki dont la recette est faisable (recettes du wiki, éléments par le système d'éléments).

namespace CalamityFandom.Content.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Jake.txt — épée qui frappe en continu, sans projectile ni animation : seulement ses dégâts énormes. Tuer un
    /// ennemi fait apparaître au-dessus du joueur un des messages de la page, au hasard.
    /// </summary>
    public class Jake : ModItem
    {
        public const int Messages = 9;

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 19506;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 10f;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.UseSound = SoundID.Item1;
            Item.width = 126;
            Item.height = 132;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.value = Item.sellPrice(copper: 10);
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.life <= 0 && player.whoAmI == Main.myPlayer)
                CombatText.NewText(player.Hitbox, Main.DiscoColor,
                    Language.GetTextValue("Mods.CalamityFandom.WikiWeapons.JakeMessage" + Main.rand.Next(1, Messages + 1)), true);
        }
    }

    /// <summary>
    /// Wiki : RGB Graydron.txt — amélioration du Graydron (Murasama) : mêmes entailles électriques vers le curseur (montante,
    /// grande montante électrique, descendante), aux couleurs qui changent sans cesse ; 50 % de critique, vraie mêlée.
    /// [choix] Le projectile d'entaille de la Murasama de Calamity, recoloré en arc-en-ciel.
    /// </summary>
    public class RGBGraydron : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 56;
            Item.height = 126;
            Item.damage = 4116;
            Item.DamageType = ModContent.GetInstance<TrueMeleeNoSpeedDamageClass>();
            Item.crit = 46;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.useAnimation = 25;
            Item.useTime = 5;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6.5f;
            Item.autoReuse = false;
            Item.value = Item.sellPrice(platinum: 30);
            Item.rare = ModContent.RarityType<HotPink>();
            Item.shoot = ModContent.ProjectileType<Projectiles.Melee.RGBGraydronSlash>();
            Item.shootSpeed = 50f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override void HoldItem(Player player) => Lighting.AddLight(player.Center, Main.DiscoColor.ToVector3() * 0.8f);
    }

    /// <summary>
    /// Wiki : Totality Tides.txt — l'arme apparaît au curseur (boule d'énergie exo) et projette des tirs exo dans la direction
    /// boule - joueur : Exobeam (explose), Exo Arrow (chercheuse, éclair vers le bas), Exo Vortex (lent, tentacules
    /// chercheuses), Exo Rammer (trois charges, fléchettes chercheuses), Exo Scythe (8 faux en étoile au contact). Boule
    /// parfaitement centrée sur le joueur : +200 % de dégâts et tirs dans tous les sens.
    /// [choix] Un tir au hasard parmi les cinq à chaque utilisation (8 images) ; « centrée » = à moins de 24 px.
    /// </summary>
    public class TotalityTides : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 640;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 8;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.knockBack = 4f;
            Item.autoReuse = true;
            Item.width = Item.height = 150;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
            Item.value = Item.sellPrice(platinum: 1);
            Item.shoot = ModContent.ProjectileType<Projectiles.Melee.TotalityBall>();
            Item.shootSpeed = 1f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI);
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Typeless
{
    /// <summary>
    /// Wiki : Bazooka.txt — arme sans classe : une roquette qui tombe au bout d'une demi-seconde et explose au contact d'un
    /// ennemi ou d'un bloc ; 1 % de la vie de la cible s'ajoute aux dégâts ; tire des Grenade Shells ; deux tirs seulement
    /// par combat de boss ; jamais de critique.
    /// [choix] Vie actuelle de la cible ; explosion de 80 px de rayon.
    /// </summary>
    public class Bazooka : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 500;
            Item.DamageType = DamageClass.Default;
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 10f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item61;
            Item.width = 66;
            Item.height = 26;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(gold: 7, silver: 20);
            Item.shoot = ModContent.ProjectileType<Projectiles.Typeless.GrenadeRound>();
            Item.shootSpeed = 12f;
            Item.useAmmo = ModContent.ItemType<Ammo.GrenadeShells>();
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10f, 0f);

        public override void ModifyWeaponCrit(Player player, ref float crit) => crit = 0f;

        public override bool CanUseItem(Player player)
            => !Main.CurrentFrameFlags.AnyActiveBossNPC || player.GetModPlayer<BazookaPlayer>().BossShots < 2;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.CurrentFrameFlags.AnyActiveBossNPC)
                player.GetModPlayer<BazookaPlayer>().BossShots++;
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<Projectiles.Typeless.GrenadeRound>(), damage, knockback, player.whoAmI);
            return false;
        }
    }

    /// <summary>
    /// Butin de la page de l'Ogscule : Demon Eye, 10 %, « while a full set of Ebonwood Chair is equipped », et rien si un
    /// emplacement d'apparence est occupé (Notes). [choix] Des chaises ne s'équipent pas : au moins 3 Ebonwood Chairs dans
    /// l'inventaire et les 3 emplacements d'apparence d'armure vides. Sa recette du wiki demande la bannière d'un ennemi
    /// qui n'existe pas (Ogsculian Burrower) : seule cette source compte.
    /// </summary>
    public class OgsculeDrop : GlobalNPC
    {
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.DemonEye;

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            var rule = new Terraria.GameContent.ItemDropRules.LeadingConditionRule(DropHelper.If(info =>
                info.player.CountItem(ItemID.EbonwoodChair, 3) >= 3 && info.player.armor.Skip(10).Take(3).All(i => i.IsAir)));
            rule.OnSuccess(Terraria.GameContent.ItemDropRules.ItemDropRule.Common(ModContent.ItemType<Ogscule>(), 10));
            npcLoot.Add(rule);
            Common.WikiData.WikiLoot.Keep.Add(rule);
        }
    }

    /// <summary>Tirs de Bazooka pendant le combat de boss en cours (remis à zéro quand il n'y a plus de boss).</summary>
    public class BazookaPlayer : ModPlayer
    {
        public int BossShots;

        public override void PostUpdate()
        {
            if (!Main.CurrentFrameFlags.AnyActiveBossNPC)
                BossShots = 0;
        }
    }

    /// <summary>
    /// Wiki : Ogscule.txt — arme sans classe tenue sur le joueur ; maintenue, elle invoque des rayons ogsculiens dans des
    /// directions au hasard, qu'on peut un peu diriger. En pré-Hardmode, elle ne fait rien (« seems to do nothing? »,
    /// « A rival demonic entity is holding back its power »). Critique impossible à augmenter.
    /// [choix] Un rayon tous les 12 images, 40 images de vie, qui tourne vers le curseur (0,03 rad par image).
    /// </summary>
    public class Ogscule : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 204;
            Item.DamageType = DamageClass.Default;
            Item.useTime = 12;
            Item.useAnimation = 46;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.channel = true;
            Item.knockBack = 7f;
            Item.autoReuse = true;
            Item.width = 54;
            Item.height = 78;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.value = Item.sellPrice(gold: 1);
            Item.shoot = ModContent.ProjectileType<Projectiles.Typeless.OgsculianRay>();
            Item.shootSpeed = 1f;
        }

        public override void ModifyWeaponCrit(Player player, ref float crit) => crit = 1f;

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.hardMode)
                return;
            foreach (var line in tooltips.Where(l => l.Name == "Tooltip0"))
                line.Text = Language.GetTextValue("Mods.CalamityFandom.WikiWeapons.OgsculePreHardmode");
            tooltips.Add(new TooltipLine(Mod, "OgsculeRival", Language.GetTextValue("Mods.CalamityFandom.WikiWeapons.OgsculeRival")) { OverrideColor = new Color(0x93, 0x07, 0x07) });
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.hardMode)
                Projectile.NewProjectile(source, player.Center, Main.rand.NextVector2Unit(), type, damage, knockback, player.whoAmI);
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Ammo
{
    /// <summary>
    /// Wiki : Grenade Shells.txt — munitions réservées au Bazooka, vendues par l'Arms Dealer quand le joueur a un Bazooka.
    /// </summary>
    public class GrenadeShells : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 99;

        public override void SetDefaults()
        {
            Item.damage = 200;
            Item.DamageType = DamageClass.Default;
            Item.knockBack = 10f;
            Item.width = 34;
            Item.height = 18;
            Item.maxStack = 9;
            Item.consumable = true;
            Item.ammo = Type;
            Item.shoot = ModContent.ProjectileType<Projectiles.Typeless.GrenadeRound>();
            Item.shootSpeed = 12f;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(silver: 30);
        }
    }

    public class GrenadeShellsShop : GlobalNPC
    {
        public override void ModifyShop(NPCShop shop)
        {
            if (shop.NpcType == NPCID.ArmsDealer)
                shop.Add(ModContent.ItemType<GrenadeShells>(), new Condition(Language.GetText("Mods.CalamityFandom.Conditions.HasBazooka"),
                    () => Main.LocalPlayer.HasItem(ModContent.ItemType<Weapons.Typeless.Bazooka>())));
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Ranged
{
    /// <summary>
    /// Wiki : Ghoststreaker.txt — crée 4 lignes (900 dégâts par seconde) et tire des balles très rapides dessus (1100).
    /// [choix] Comme la Stringstreaker : lignes de 800 px à ±2° et ±6° vers le curseur pendant le tir (une frappe toutes
    /// les 6 images = 90 par frappe) ; une balle par ligne à chaque tir ; utilise des balles.
    /// </summary>
    public class Ghoststreaker : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 900;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 34;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item40;
            Item.width = 114;
            Item.height = 64;
            Item.rare = ModContent.RarityType<CalamityRed>();
            Item.value = Item.sellPrice(platinum: 2);
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 34f;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-20f, 0f);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int lines = ModContent.ProjectileType<Projectiles.Ranged.GhostLine>();
            foreach (float deg in new[] { -6f, -2f, 2f, 6f })
            {
                float angle = MathHelper.ToRadians(deg);
                if (player.ownedProjectileCounts[lines] < 4)
                    Projectile.NewProjectile(source, player.Center, velocity.SafeNormalize(Vector2.UnitX), lines, damage / 10, 0f, player.whoAmI, angle);
                Projectile.NewProjectile(source, position, velocity.RotatedBy(angle), ModContent.ProjectileType<Projectiles.Weapons1119ac.StringBullet>(), (int)(damage * 1.22f), knockback, player.whoAmI);
            }
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Magic
{
    /// <summary>
    /// Wiki : Wisdom of Survival.txt — livre de sorts (1200 dégâts, 120 mana, 90 images) sans description.
    /// [choix] Une grande orbe d'eau lente qui traverse tout et se divise en 6 gouttes chercheuses à sa disparition.
    /// </summary>
    public class WisdomofSurvival : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 1200;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 120;
            Item.useTime = Item.useAnimation = 90;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.knockBack = 1f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item84;
            Item.width = Item.height = 72;
            Item.rare = ModContent.RarityType<CalamityRed>();
            Item.value = Item.sellPrice(platinum: 2);
            Item.shoot = ModContent.ProjectileType<Projectiles.Magic.SurvivalOrb>();
            Item.shootSpeed = 6f;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Summon
{
    /// <summary>
    /// Wiki : Terreye Staff.txt — 4 places : un trio de Floating Biomass au-dessus du joueur. Près d'un ennemi, l'un fonce
    /// dessus en semant des chercheurs terra, le deuxième reste à distance et crache un flot de flammes terra, le troisième
    /// tourne vite autour en tirant des rayons terra vers l'intérieur. Clic droit maintenu 5 s sur un ennemi : un sigil
    /// grandit, puis les trois foncent ensemble sur lui (ils arrivent en même temps), 10 à 20 disques terra partent au
    /// hasard et deux cœurs terra tombent (10 PV au contact). Un seul trio à la fois.
    /// [choix] Les trois arrivent en 20 images ; impact à 500 % des dégâts du sbire.
    /// </summary>
    public class TerreyeStaff : WikiMinionStaff
    {
        protected override int Damage => 129;
        protected override int Rarity => ItemRarityID.Lime;
        protected override int SellCopper => 20000;
        protected override int BuffType => ModContent.BuffType<Buffs.TerreyesBuff>();
        protected override int MinionType => ModContent.ProjectileType<FloatingBiomass>();
        protected override float Knockback => 1f;
        protected override int UseTime => 29;

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.channel = true;
                Item.UseSound = null;
                return player.ownedProjectileCounts[MinionType] > 0 && player.ownedProjectileCounts[ModContent.ProjectileType<TerreyeSigil>()] <= 0;
            }
            Item.channel = false;
            Item.UseSound = SoundID.Item44;
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                NPC target = Main.npc.FirstOrDefault(n => n.CanBeChasedBy() && n.Hitbox.Contains(Main.MouseWorld.ToPoint()));
                if (target != null)
                    Projectile.NewProjectile(source, target.Center, Vector2.Zero, ModContent.ProjectileType<TerreyeSigil>(), damage, knockback, player.whoAmI, target.whoAmI);
                return false;
            }
            // Un seul trio : les anciens disparaissent
            foreach (Projectile p in Main.projectile)
                if (p.active && p.owner == player.whoAmI && p.type == type)
                    p.Kill();
            player.AddBuff(Item.buffType, 2);
            for (int role = 0; role < 3; role++)
            {
                int p = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI, role);
                if (Main.projectile.IndexInRange(p))
                {
                    Main.projectile[p].originalDamage = Item.damage;
                    Main.projectile[p].minionSlots = 4f / 3f;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// Wiki : The Dreaming Ghost.txt — 4 places par paire : un papillon violet qui fonce sur les ennemis et sème des éclats
    /// de cristal, un rose immobile qui tire une nuée de balles sakura. Chaque violet : +10 % de dégâts ; chaque rose : +2
    /// régénération.
    /// </summary>
    public class TheDreamingGhost : WikiMinionStaff
    {
        protected override int Damage => 468;
        protected override int Rarity => ModContent.RarityType<BurnishedAuric>();
        protected override int SellCopper => 400000;
        protected override int BuffType => ModContent.BuffType<Buffs.ResurrectionButterflyBuff>();
        protected override int MinionType => ModContent.ProjectileType<PurpleCrystallineButterfly>();
        protected override float Knockback => 1f;
        protected override int UseTime => 12;

        public override bool CanUseItem(Player player) => player.slotsMinions + 4f <= player.maxMinions;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            foreach (int t in new[] { type, ModContent.ProjectileType<PinkCrystallineButterfly>() })
            {
                int p = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, t, damage, knockback, player.whoAmI);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].originalDamage = Item.damage;
            }
            return false;
        }
    }

    /// <summary>Floating Biomass (Terreye Staff) : ai[0] = rôle (0 fonce, 1 flammes à distance, 2 tourne autour) ; ai[1] = impact du sigil.</summary>
    public class FloatingBiomass : WikiMinion
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Summon/FloatingBiomass";
        protected override int BuffType => ModContent.BuffType<Buffs.TerreyesBuff>();
        protected override float Slots => 4f / 3f;

        /// <summary>Cible du sigil et images restantes avant l'impact (posés par le sigil).</summary>
        public int ConvergeTarget = -1, ConvergeTime;

        public override void AI()
        {
            if (!KeepAlive())
                return;
            int role = (int)Projectile.ai[0];
            Projectile.localAI[0]++;
            if (ConvergeTarget >= 0)
            {
                NPC t = Main.npc[ConvergeTarget];
                if (!t.active || ConvergeTime <= 0)
                {
                    ConvergeTarget = -1;
                    return;
                }
                // Arrive pile au bout de ConvergeTime images, quelle que soit la distance
                Projectile.velocity = (t.Center - Projectile.Center) / ConvergeTime;
                Projectile.rotation = Projectile.velocity.ToRotation();
                if (--ConvergeTime == 0 && role == 0 && Projectile.owner == Main.myPlayer)
                    Converge(t);
                return;
            }
            NPC target = Projectile.Center.MinionHoming(1000f, Owner);
            if (target == null)
            {
                Vector2 home = Owner.Center + new Vector2((role - 1) * 40f, -70f);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, (home - Projectile.Center) * 0.1f, 0.2f);
                if (Projectile.Distance(Owner.Center) > 2000f)
                    Projectile.Center = Owner.Center;
                Projectile.rotation = Projectile.velocity.X * 0.05f;
                return;
            }
            var source = Projectile.GetSource_FromThis();
            bool mine = Projectile.owner == Main.myPlayer;
            switch (role)
            {
                case 0:
                    // Fonce sur la cible en semant des chercheurs terra
                    if (Projectile.localAI[0] % 30 == 0)
                        Projectile.velocity = Projectile.SafeDirectionTo(target.Center) * 18f;
                    if (mine && Projectile.localAI[0] % 12 == 0)
                        Projectile.NewProjectile(source, Projectile.Center, Main.rand.NextVector2Circular(4f, 4f), ModContent.ProjectileType<TerraSeeker>(), Projectile.damage / 3, 0f, Projectile.owner);
                    break;
                case 1:
                    // À distance, flot de flammes terra
                    Vector2 spot = target.Center + (Projectile.Center - target.Center).SafeNormalize(Vector2.UnitY) * 260f;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, (spot - Projectile.Center) * 0.08f, 0.2f);
                    if (mine && Projectile.localAI[0] % 5 == 0)
                        Projectile.NewProjectile(source, Projectile.Center, Projectile.SafeDirectionTo(target.Center).RotatedByRandom(0.15f) * 11f, ModContent.ProjectileType<TerraFlame>(), Projectile.damage / 4, 0f, Projectile.owner);
                    break;
                default:
                    // Tourne vite autour de la cible en tirant des rayons vers l'intérieur
                    float angle = Projectile.localAI[0] * 0.12f;
                    Vector2 orbit = target.Center + angle.ToRotationVector2() * 180f;
                    Projectile.velocity = (orbit - Projectile.Center) * 0.3f;
                    if (mine && Projectile.localAI[0] % 20 == 0)
                        Projectile.NewProjectile(source, Projectile.Center, Projectile.SafeDirectionTo(target.Center) * 14f, ProjectileID.TerraBeam, Projectile.damage / 2, 0f, Projectile.owner);
                    break;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        private void Converge(NPC t)
        {
            var source = Projectile.GetSource_FromThis();
            t.SimpleStrikeNPC(Projectile.damage * 5, 0, false, 0f, DamageClass.Summon);
            int disks = Main.rand.Next(10, 21);
            for (int i = 0; i < disks; i++)
                Projectile.NewProjectile(source, t.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(8f, 14f), ModContent.ProjectileType<TerraDisk>(), Projectile.damage / 2, 2f, Projectile.owner);
            for (int i = 0; i < 2; i++)
                Projectile.NewProjectile(source, t.Center, new Vector2(Main.rand.NextFloat(-3f, 3f), -5f), ModContent.ProjectileType<TarraHeart>(), 0, 0f, Projectile.owner);
            SoundEngine.PlaySound(SoundID.Item74, t.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, 1f,
                Math.Cos(Projectile.rotation) < 0 ? SpriteEffects.FlipVertically : SpriteEffects.None);
            return false;
        }
    }

    /// <summary>Sigil du Terreye Staff : grandit 5 s tant que le clic droit est maintenu sur l'ennemi, puis lance la convergence.</summary>
    public class TerreyeSigil : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Magic/TerraSigil";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 400;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            NPC t = Main.npc[(int)Projectile.ai[0]];
            if (!t.active || (Projectile.owner == Main.myPlayer && (!owner.channel || owner.HeldItem.type != ModContent.ItemType<TerreyeStaff>())))
            {
                Projectile.Kill();
                return;
            }
            owner.itemTime = owner.itemAnimation = 2;
            Projectile.Center = t.Center;
            Projectile.rotation += 0.05f;
            Projectile.scale = 0.2f + Projectile.ai[1] / 300f * 1.3f;
            if (++Projectile.ai[1] >= 300f)
            {
                foreach (Projectile p in Main.projectile)
                    if (p.active && p.owner == Projectile.owner && p.ModProjectile is FloatingBiomass b)
                    {
                        b.ConvergeTarget = t.whoAmI;
                        b.ConvergeTime = 20;
                    }
                Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, new Color(120, 255, 120, 0) * 0.8f, Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None);
            return false;
        }
    }

    /// <summary>Chercheur terra (Floating Biomass).</summary>
    public class TerraSeeker : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.TerraBeam;

        public override void SetStaticDefaults() => ProjectileID.Sets.MinionShot[Type] = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.scale = 0.5f;
        }

        public override void AI()
        {
            NPC t = Projectile.FindTargetWithinRange(600f);
            if (t != null)
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(t.Center) * 12f, 0.08f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            Lighting.AddLight(Projectile.Center, 0.2f, 0.6f, 0.2f);
        }
    }

    /// <summary>Flamme terra (Floating Biomass) : traverse 2 ennemis, poussière verte.</summary>
    public class TerraFlame : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults() => ProjectileID.Sets.MinionShot[Type] = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 40;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.TerraBlade, Projectile.velocity * 0.2f, 0, default, 1.4f);
            d.noGravity = true;
        }
    }

    /// <summary>Disque terra (convergence du Terreye Staff).</summary>
    public class TerraDisk : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.LightDisc;

        public override void SetStaticDefaults() => ProjectileID.Sets.MinionShot[Type] = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 90;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI() => Projectile.rotation += 0.4f;

        public override Color? GetAlpha(Color lightColor) => new Color(120, 255, 120, 200);
    }

    /// <summary>Cœur terra : tombe, rend 10 PV au joueur qui le touche.</summary>
    public class TarraHeart : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Materials/LivingShard";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.2f, 8f);
            Projectile.velocity.X *= 0.98f;
            foreach (Player p in Main.player)
                if (p.active && !p.dead && p.Hitbox.Intersects(Projectile.Hitbox))
                {
                    if (p.whoAmI == Main.myPlayer)
                        p.Heal(10);
                    Projectile.Kill();
                    return;
                }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity.X *= 0.5f;
            Projectile.velocity.Y = 0f;
            return false;
        }
    }

    /// <summary>Papillon violet (The Dreaming Ghost) : fonce sur les ennemis, éclats de cristal au contact.</summary>
    public class PurpleCrystallineButterfly : WikiMinion
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Summon/PurpleCrystallineButterfly";
        protected override int BuffType => ModContent.BuffType<Buffs.ResurrectionButterflyBuff>();
        protected override float Slots => 2f;

        public override void AI()
        {
            if (!KeepAlive())
                return;
            NPC target = Projectile.Center.MinionHoming(1200f, Owner);
            if (target == null)
                Idle();
            else if (++Projectile.ai[0] % 25 == 0)
            {
                Projectile.velocity = Projectile.SafeDirectionTo(target.Center) * 22f;
                Projectile.netUpdate = true;
            }
            else
                Projectile.velocity *= 0.97f;
            Projectile.spriteDirection = Projectile.velocity.X > 0 ? 1 : -1;
            Lighting.AddLight(Projectile.Center, 0.5f, 0.2f, 0.7f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.owner != Main.myPlayer)
                return;
            for (int i = 0; i < 3; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Main.rand.NextVector2Circular(7f, 7f), ProjectileID.CrystalShard, Projectile.damage / 4, 0f, Projectile.owner);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawFlipped(Color.White);
            return false;
        }
    }

    /// <summary>Papillon rose (The Dreaming Ghost) : reste sur place et tire une nuée de balles sakura.</summary>
    public class PinkCrystallineButterfly : WikiMinion
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Summon/PinkCrystallineButterfly";
        protected override int BuffType => ModContent.BuffType<Buffs.ResurrectionButterflyBuff>();
        protected override float Slots => 2f;

        public override void AI()
        {
            if (!KeepAlive())
                return;
            NPC target = Projectile.Center.MinionHoming(1200f, Owner);
            if (target == null)
            {
                Idle();
                return;
            }
            // Immobile pendant l'attaque
            Projectile.velocity *= 0.85f;
            if (Projectile.owner == Main.myPlayer && ++Projectile.ai[0] % 8 == 0)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.SafeDirectionTo(target.Center).RotatedByRandom(0.35f) * 12f,
                    ModContent.ProjectileType<SakuraBullet>(), Projectile.damage / 3, 0f, Projectile.owner);
            Projectile.spriteDirection = target.Center.X > Projectile.Center.X ? 1 : -1;
            Lighting.AddLight(Projectile.Center, 0.7f, 0.3f, 0.5f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawFlipped(Color.White);
            return false;
        }
    }

    /// <summary>Balle sakura (papillon rose) : chercheuse.</summary>
    public class SakuraBullet : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Summon/SakuraBullet";

        public override void SetStaticDefaults() => ProjectileID.Sets.MinionShot[Type] = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 150;
        }

        public override void AI()
        {
            NPC t = Projectile.FindTargetWithinRange(500f);
            if (t != null)
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(t.Center) * 12f, 0.05f);
            Projectile.rotation += 0.2f;
        }
    }
}

namespace CalamityFandom.Content.Buffs
{
    public class TerreyesBuff : WikiMinionBuff
    {
        public override string Texture => "CalamityFandom/Content/Buffs/Wiki/TerreyesBuff";
        protected override int MinionType => ModContent.ProjectileType<Items.Weapons.Summon.FloatingBiomass>();
    }

    /// <summary>Chaque papillon violet : +10 % de dégâts ; chaque rose : +2 régénération.</summary>
    public class ResurrectionButterflyBuff : WikiMinionBuff
    {
        public override string Texture => "CalamityFandom/Content/Buffs/Wiki/ResurrectionButterflyBuff";
        protected override int MinionType => ModContent.ProjectileType<Items.Weapons.Summon.PurpleCrystallineButterfly>();

        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            player.GetDamage<GenericDamageClass>() += 0.1f * player.ownedProjectileCounts[ModContent.ProjectileType<Items.Weapons.Summon.PurpleCrystallineButterfly>()];
            player.lifeRegen += 2 * player.ownedProjectileCounts[ModContent.ProjectileType<Items.Weapons.Summon.PinkCrystallineButterfly>()];
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Melee
{
    /// <summary>Entaille du RGB Graydron : celle de la Murasama de Calamity, en couleurs qui défilent.</summary>
    public class RGBGraydronSlash : MurasamaSlash
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/MurasamaSlash";
        public override LocalizedText DisplayName => Language.GetText("Mods.CalamityFandom.Items.RGBGraydron.DisplayName");

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.frameCounter <= 1)
                return false;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle frame = texture.Frame(verticalFrames: Main.projFrames[Type], frameY: Projectile.frame);
            SpriteEffects fx = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Color rgb = Main.hslToRgb((Main.GlobalTimeWrappedHourly * 1.5f + Projectile.frame * 0.07f) % 1f, 1f, 0.6f);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + Projectile.velocity * 0.3f + new Vector2(0, -32).RotatedBy(Projectile.rotation), frame,
                rgb, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, fx);
            return false;
        }

        public override void PostAI() => Lighting.AddLight(Projectile.Center, Main.hslToRgb(Main.GlobalTimeWrappedHourly * 1.5f % 1f, 1f, 0.6f).ToVector3() * 2f);
    }

    /// <summary>Boule exo de la Totality Tides : suit le curseur et projette les tirs exo dans la direction boule - joueur.</summary>
    public class TotalityBall : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Melee/TotalityTides";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.friendly = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 2;
            Projectile.scale = 0.5f;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (Projectile.owner == Main.myPlayer)
            {
                if (!owner.channel || owner.dead || owner.HeldItem.type != ModContent.ItemType<Items.Weapons.Melee.TotalityTides>())
                {
                    Projectile.Kill();
                    return;
                }
                if (Projectile.Center != Main.MouseWorld)
                    Projectile.netUpdate = Projectile.localAI[1]++ % 6 == 0;
                Projectile.Center = Main.MouseWorld;
            }
            Projectile.timeLeft = 2;
            owner.itemTime = owner.itemAnimation = 2;
            owner.ChangeDir(Projectile.Center.X > owner.Center.X ? 1 : -1);
            Projectile.rotation += 0.08f;
            Lighting.AddLight(Projectile.Center, Main.DiscoColor.ToVector3());
            if (Projectile.owner != Main.myPlayer || ++Projectile.ai[0] % owner.HeldItem.useTime != 0)
                return;
            Vector2 away = Projectile.Center - owner.Center;
            bool centered = away.Length() < 24f;
            Vector2 dir = centered ? Main.rand.NextVector2Unit() : away.SafeNormalize(Vector2.UnitX).RotatedByRandom(0.3f);
            int damage = centered ? Projectile.damage * 3 : Projectile.damage;
            int[] types =
            {
                ModContent.ProjectileType<TotalityExobeam>(), ModContent.ProjectileType<TotalityExoArrow>(), ModContent.ProjectileType<TotalityExoVortex>(),
                ModContent.ProjectileType<TotalityExoRammer>(), ModContent.ProjectileType<TotalityExoScythe>(),
            };
            int type = Main.rand.Next(types);
            float speed = type == types[2] ? 4f : 14f;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, dir * speed, type, damage, Projectile.knockBack, Projectile.owner);
            SoundEngine.PlaySound(SoundID.Item60 with { Volume = 0.4f }, Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None);
            return false;
        }
    }

    /// <summary>Base des tirs exo de la Totality Tides : mêlée, couleur exo.</summary>
    public abstract class TotalityShot : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        protected void Home(float range, float speed, float inertia)
        {
            NPC t = Projectile.FindTargetWithinRange(range);
            if (t != null)
                Projectile.velocity = (Projectile.velocity * (inertia - 1f) + Projectile.SafeDirectionTo(t.Center) * speed) / inertia;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;
    }

    /// <summary>Exobeam : tout droit, explosion exo au contact d'un ennemi ou d'un bloc.</summary>
    public class TotalityExobeam : TotalityShot
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/Exobeam";

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<Ranged.Wiki.DartBlastProxy>(), Projectile.damage / 2, 0f, Projectile.owner, 70f, DustID.RainbowMk2);
        }
    }

    /// <summary>Exo Arrow : cherche vite les ennemis ; au contact, un éclair exo tombe sur la cible.</summary>
    public class TotalityExoArrow : TotalityShot
    {
        public override string Texture => "CalamityMod/Projectiles/Ranged/ExoCrystalArrow";

        public override void AI()
        {
            Home(900f, 18f, 8f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center - new Vector2(0f, 600f), new Vector2(0f, 24f), ModContent.ProjectileType<TotalityLightning>(), Projectile.damage / 2, 0f, Projectile.owner);
        }
    }

    /// <summary>Éclair exo (Exo Arrow) : tombe tout droit, traverse.</summary>
    public class TotalityLightning : TotalityShot
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 30;
            Projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.RainbowMk2, Vector2.Zero, 0, Main.DiscoColor, 1.3f);
            d.noGravity = true;
        }
    }

    /// <summary>Exo Vortex : lent, cherche les ennemis, lance des tentacules exo chercheuses.</summary>
    public class TotalityExoVortex : TotalityShot
    {
        public override string Texture => "CalamityMod/Projectiles/Summon/CosmicEnergySpiral";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 60;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 240;
            Projectile.scale = 0.6f;
        }

        public override void AI()
        {
            Home(800f, 5f, 30f);
            Projectile.rotation += 0.15f;
            if (Projectile.owner == Main.myPlayer && ++Projectile.ai[0] % 20 == 0)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Unit() * 10f, ModContent.ProjectileType<TotalityTentacle>(), Projectile.damage / 3, 0f, Projectile.owner);
        }
    }

    /// <summary>Tentacule exo (Exo Vortex) / fléchette exo (Exo Rammer) : chercheuse, traînée de poussière.</summary>
    public class TotalityTentacle : TotalityShot
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void AI()
        {
            Home(700f, 14f, 15f);
            Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.RainbowMk2, Vector2.Zero, 0, Main.DiscoColor, 1f);
            d.noGravity = true;
        }
    }

    /// <summary>Exo Rammer : charge trois fois sur les ennemis en semant des fléchettes chercheuses, puis disparaît.</summary>
    public class TotalityExoRammer : TotalityShot
    {
        public override string Texture => "CalamityMod/Projectiles/Summon/CosmicEnergySpiral";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.scale = 0.35f;
        }

        public override void AI()
        {
            Projectile.rotation += 0.3f;
            if (++Projectile.ai[0] % 40 == 0)
            {
                NPC t = Projectile.FindTargetWithinRange(900f);
                if (t != null)
                    Projectile.velocity = Projectile.SafeDirectionTo(t.Center) * 22f;
                if (++Projectile.ai[1] > 3)
                    Projectile.Kill();
            }
            else
                Projectile.velocity *= 0.97f;
            if (Projectile.owner == Main.myPlayer && Projectile.ai[0] % 10 == 0)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Unit() * 8f, ModContent.ProjectileType<TotalityTentacle>(), Projectile.damage / 4, 0f, Projectile.owner);
        }
    }

    /// <summary>Exo Scythe : au contact, 8 autres faux exo partent en étoile.</summary>
    public class TotalityExoScythe : TotalityShot
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/BlazingPhantomBlade";

        public override void AI() => Projectile.rotation += 0.35f;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.owner != Main.myPlayer || Projectile.ai[0] == 1f)
                return;
            for (int i = 0; i < 8; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, (MathHelper.TwoPi * i / 8f).ToRotationVector2() * 12f, Type, Projectile.damage / 3, 0f, Projectile.owner, 1f);
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Ranged.Wiki
{
    /// <summary>Explosion générique (rayon ai[0], poussière ai[1]) : une frappe sur tous les ennemis du rayon.</summary>
    public class DartBlastProxy : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            if (Projectile.localAI[0]++ == 0)
            {
                Projectile.Resize((int)Projectile.ai[0] * 2, (int)Projectile.ai[0] * 2);
                for (int i = 0; i < 25; i++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, (int)Projectile.ai[1], Main.rand.NextVector2Circular(6f, 6f), 0, Main.DiscoColor, 1.4f);
                    d.noGravity = true;
                }
                SoundEngine.PlaySound(SoundID.Item14 with { Volume = 0.5f }, Projectile.Center);
            }
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Ranged
{
    /// <summary>Ligne de la Ghoststreaker (comme celle de la Stringstreaker, pour cette arme).</summary>
    public class GhostLine : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public const float Length = 800f;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 40;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.itemAnimation > 0 && owner.HeldItem.type == ModContent.ItemType<Items.Weapons.Ranged.Ghoststreaker>())
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
            Main.EntitySpriteDraw(TextureAssets.MagicPixel.Value, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, 1, 1), new Color(200, 220, 255, 0) * 0.7f,
                Projectile.velocity.ToRotation(), new Vector2(0f, 0.5f), new Vector2(Length, 2f), SpriteEffects.None);
            return false;
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Typeless
{
    /// <summary>Roquette du Bazooka : tombe après une demi-seconde, explose (80 px) ; +1 % de la vie de la cible ; jamais de critique.</summary>
    public class GrenadeRound : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Typeless/GrenadeRound";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Default;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            if (++Projectile.ai[0] > 30)
                Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.25f, 16f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Main.rand.NextBool(2))
                Dust.NewDustPerfect(Projectile.Center - Projectile.velocity * 0.5f, DustID.Smoke, Vector2.Zero, 100, default, 1.2f).noGravity = true;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.DisableCrit();
            modifiers.FlatBonusDamage += target.life * 0.01f;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            for (int i = 0; i < 30; i++)
                Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool() ? DustID.Torch : DustID.Smoke, Main.rand.NextVector2Circular(7f, 7f), 0, default, 1.8f).noGravity = true;
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.Resize(160, 160);
                Projectile.penetrate = -1;
                Projectile.Damage();
            }
        }
    }

    /// <summary>Rayon ogsculien (Ogscule) : part du joueur dans une direction au hasard et tourne doucement vers le curseur.</summary>
    public class OgsculianRay : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public const float Length = 900f;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Default;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 40;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            Projectile.Center = owner.MountedCenter;
            if (Projectile.owner == Main.myPlayer)
            {
                float want = (Main.MouseWorld - owner.MountedCenter).ToRotation();
                float now = Projectile.velocity.ToRotation();
                Projectile.velocity = now.AngleTowards(want, 0.03f).ToRotationVector2();
                if (Projectile.localAI[0]++ % 8 == 0)
                    Projectile.netUpdate = true;
            }
            Lighting.AddLight(Projectile.Center + Projectile.velocity * 200f, 0.8f, 0.2f, 0.5f);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => modifiers.DisableCrit();

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * Length, 14f, ref _);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float fade = Math.Min(1f, Projectile.timeLeft / 10f);
            Main.EntitySpriteDraw(TextureAssets.MagicPixel.Value, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, 1, 1), new Color(230, 60, 140, 0) * 0.8f * fade,
                Projectile.velocity.ToRotation(), new Vector2(0f, 0.5f), new Vector2(Length, 8f), SpriteEffects.None);
            return false;
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Magic
{
    /// <summary>Orbe de la Wisdom of Survival : lente, traverse tout ; se divise en 6 gouttes chercheuses à la fin.</summary>
    public class SurvivalOrb : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.WaterBolt;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
            Projectile.scale = 2.5f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void AI()
        {
            Projectile.rotation += 0.1f;
            Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(20f, 20f), DustID.Water, Vector2.Zero, 0, default, 1.5f);
            d.noGravity = true;
            Lighting.AddLight(Projectile.Center, 0.2f, 0.4f, 0.9f);
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner != Main.myPlayer)
                return;
            for (int i = 0; i < 6; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (MathHelper.TwoPi * i / 6f).ToRotationVector2() * 9f, ModContent.ProjectileType<SurvivalDrop>(), Projectile.damage / 4, 1f, Projectile.owner);
        }
    }

    public class SurvivalDrop : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.WaterBolt;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            NPC t = Projectile.FindTargetWithinRange(600f);
            if (t != null)
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(t.Center) * 12f, 0.07f);
            Projectile.rotation += 0.2f;
        }
    }
}

namespace CalamityFandom.Content.Items
{
    public class WikiWeapons1120Checks : ModSystem
    {
        public override void Load()
        {
            SelfTest.ExtraChecks.Add(t =>
            {
                foreach (int type in new[]
                {
                    ModContent.ItemType<Weapons.Melee.Jake>(), ModContent.ItemType<Weapons.Melee.RGBGraydron>(), ModContent.ItemType<Weapons.Melee.TotalityTides>(),
                    ModContent.ItemType<Weapons.Typeless.Bazooka>(), ModContent.ItemType<Weapons.Ranged.Ghoststreaker>(),
                    ModContent.ItemType<Weapons.Magic.WisdomofSurvival>(), ModContent.ItemType<Weapons.Summon.TerreyeStaff>(), ModContent.ItemType<Weapons.Summon.TheDreamingGhost>(),
                })
                    t.Expect(Main.recipe.Take(Recipe.numRecipes).Any(r => r.createItem.type == type && !r.Disabled), $"recette du wiki de {Lang.GetItemNameValue(type)}");
                var eyeRates = new List<Terraria.GameContent.ItemDropRules.DropRateInfo>();
                foreach (var r in Main.ItemDropsDB.GetRulesForNPCID(NPCID.DemonEye, false))
                    r.ReportDroprates(eyeRates, new Terraria.GameContent.ItemDropRules.DropRateInfoChainFeed(1f));
                t.Expect(eyeRates.Any(r => r.itemId == ModContent.ItemType<Weapons.Typeless.Ogscule>()), "le Demon Eye peut lâcher l'Ogscule");
                t.Expect(ContentSamples.ItemsByType[ModContent.ItemType<Weapons.Typeless.Bazooka>()].useAmmo == ModContent.ItemType<Ammo.GrenadeShells>(), "le Bazooka tire des Grenade Shells");
            });
        }
    }
}
