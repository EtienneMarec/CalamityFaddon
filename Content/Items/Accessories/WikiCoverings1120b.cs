using System;
using System.Linq;
using CalamityFandom.Common.Debug;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Wings;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

// Lot AX (20/11) : coverings qui reprennent tous les effets de leurs ingrédients (UpdateAccessory des objets appelés tels
// quels) et Earthshooter.

namespace CalamityFandom.Content.Items.Accessories
{
    /// <summary>
    /// Wiki : Shadowspectre Tracers.txt — amélioration des Celestial Tracers (Seraph Tracers de Calamity) et des Lunarspectre
    /// Tracers : compte comme des ailes (vitesse 15, accélération x3, vol infini), 15 défense, saut automatique, marche sur
    /// les liquides, fées entropiques en courant...
    /// [choix] Effets des deux ingrédients (fées et Moon Burnt des Lunarspectre) ; ailes : planche des Seraph Tracers en
    /// attendant un sprite (docs/SPRITES.md) ; « Umbrus Fumes » = Umbric Fumes, 3 s. [technique] Pas faits : plongeon (bas
    /// deux fois), ruée en ombre, debuffs renforcés.
    /// </summary>
    public class ShadowspectreTracers : WikiCovering
    {
        protected override int Rarity => ModContent.RarityType<HotPink>();
        protected override int SellCopper => 5000000;

        public override void Load()
        {
            if (!Main.dedServ)
                EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Accessories/Wings/SeraphTracers_Wings", EquipType.Wings, this);
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            if (!Main.dedServ)
                ArmorIDs.Wing.Sets.Stats[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Wings)] = new WingStats(1000, 15f, 3f);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 52;
            Item.height = 40;
            Item.defense = 15;
            if (!Main.dedServ)
                Item.wingSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Wings);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            ModContent.GetInstance<SeraphTracers>().UpdateAccessory(player, hideVisual);
            ModContent.GetInstance<LunarspectreTracers>().UpdateAccessory(player, hideVisual);
            player.lifeRegen += 2;
            player.runAcceleration *= 1.12f;
            player.runSlowdown *= 1.12f;
            // Vol infini
            player.wingTime = player.wingTimeMax;
            player.GetModPlayer<Coverings1120bPlayer>().Shadowspectre = true;
        }
    }

    /// <summary>
    /// Wiki : Spy's Lootbox.txt — tous les effets de ses douze ingrédients (Nanotech, Vampiric Talisman, Eclipse Mirror,
    /// Blunder Booster, Dragon Scales, Ethereal Extorter, Venerated Locket, Gloves of Precision et of Recklessness, Old Die,
    /// Sand Cloak, Scuttler's Jewel), plus : +30 de furtivité max, +12 % de dégâts et de critique de spy, -700 d'aggro,
    /// furtivité 50 % plus rapide à l'arrêt, frappes furtives à moitié prix et toujours critiques ; à chaque tir de spy, une
    /// copie tombe du ciel vers le curseur (30 % des dégâts).
    /// [technique] Pas faits : éclats au bout de la copie, tornades du soleil noir, aura de la touche Spectral Veil,
    /// explosion de lumière quand on est touché.
    /// </summary>
    public class SpysLootbox : WikiCovering
    {
        protected override int Rarity => ModContent.RarityType<BurnishedAuric>();
        protected override int SellCopper => 280000;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            ModContent.GetInstance<Nanotech>().UpdateAccessory(player, hideVisual);
            ModContent.GetInstance<VampiricTalisman>().UpdateAccessory(player, hideVisual);
            ModContent.GetInstance<EclipseMirror>().UpdateAccessory(player, hideVisual);
            ModContent.GetInstance<BlunderBooster>().UpdateAccessory(player, hideVisual);
            ModContent.GetInstance<DragonScales>().UpdateAccessory(player, hideVisual);
            ModContent.GetInstance<EtherealExtorter>().UpdateAccessory(player, hideVisual);
            ModContent.GetInstance<VeneratedLocket>().UpdateAccessory(player, hideVisual);
            ModContent.GetInstance<GloveOfPrecision>().UpdateAccessory(player, hideVisual);
            ModContent.GetInstance<GloveOfRecklessness>().UpdateAccessory(player, hideVisual);
            ModContent.GetInstance<OldDie>().UpdateAccessory(player, hideVisual);
            ModContent.GetInstance<SandCloak>().UpdateAccessory(player, hideVisual);
            ModContent.GetInstance<ScuttlersJewel>().UpdateAccessory(player, hideVisual);
            var cal = player.Calamity();
            var rogue = ModContent.GetInstance<RogueDamageClass>();
            cal.rogueStealthMax += 0.3f;
            player.GetDamage(rogue) += 0.12f;
            player.GetCritChance(rogue) += 12;
            player.aggro -= 700;
            cal.stealthGenStandstill += 0.5f;
            cal.stealthStrikeHalfCost = true;
            cal.wearingRogueArmor = true;
            player.GetModPlayer<Coverings1120bPlayer>().Lootbox = true;
        }
    }

    /// <summary>
    /// Wiki : Universal Stone.txt — « le vrai potentiel d'un élémentaire mineur » : effets des six pierres (Aero, Cryo,
    /// Metamorphic, Chaos, Bloom, Cosmic), un Cosmic Conflict Construct (4002 dégâts d'invocation), un bouclier cosmique qui
    /// inflige God Slayer Inferno, immunité à God Slayer Inferno, +50 % de temps de vol.
    /// [choix] Le construct = le Cosmic Elemental de la Cosmic Stone, à 4002 dégâts (pas de sprite du construct sur le wiki) ;
    /// bouclier de 120 px : God Slayer Inferno 3 s toutes les 30 images. [technique] Pas faits : Cosmichid (+1 % de dégâts),
    /// Mana Burn.
    /// </summary>
    public class UniversalStone : WikiCovering
    {
        protected override int Rarity => ModContent.RarityType<CosmicPurple>();
        protected override int SellCopper => 200000;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            ModContent.GetInstance<AeroStone>().UpdateAccessory(player, hideVisual);
            ModContent.GetInstance<CryoStone>().UpdateAccessory(player, hideVisual);
            ModContent.GetInstance<ChaosStone>().UpdateAccessory(player, hideVisual);
            ModContent.GetInstance<BloomStone>().UpdateAccessory(player, hideVisual);
            ModContent.GetInstance<MetamorphicStone>().UpdateAccessory(player, hideVisual);
            Weapons.Summon.WikiWeapons1120bPlayer.KeepMinion(player, Item, ModContent.ProjectileType<Weapons.Summon.CosmicElementalMinion>(),
                ModContent.BuffType<Buffs.CosmicElementalBuff>(), 4002);
            player.buffImmune[ModContent.BuffType<GodSlayerInferno>()] = true;
            player.GetModPlayer<Coverings1120bPlayer>().Universal = true;
        }
    }

    public class Coverings1120bPlayer : ModPlayer
    {
        public bool Lootbox, Universal, Shadowspectre;

        public override void ResetEffects() => Lootbox = Universal = Shadowspectre = false;

        // Shadowspectre Tracers : toutes les attaques infligent Umbric Fumes (« Umbrus Fumes » de la page) [choix] 3 s
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Shadowspectre)
                target.AddBuff(ModContent.BuffType<Buffs.UmbricFumes>(), 180);
        }

        public override void PostUpdateEquips()
        {
            if (Universal)
            {
                Player.wingTimeMax = (int)(Player.wingTimeMax * 1.5f);
                if (Main.GameUpdateCount % 30 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                    foreach (NPC n in Main.npc)
                        if (n.active && !n.friendly && n.Distance(Player.Center) < 120f)
                            n.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 180);
                if (Main.rand.NextBool(3))
                {
                    Dust d = Dust.NewDustPerfect(Player.Center + Main.rand.NextVector2CircularEdge(120f, 120f), DustID.PurpleTorch, Vector2.Zero, 0, default, 1.1f);
                    d.noGravity = true;
                }
            }
        }

        public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Spy's Lootbox : une copie du tir tombe du ciel vers le curseur
            if (Lootbox && item.CountsAsClass(ModContent.GetInstance<RogueDamageClass>()) && Player.whoAmI == Main.myPlayer)
            {
                Vector2 from = Main.MouseWorld + new Vector2(Main.rand.NextFloat(-200f, 200f), -800f);
                Projectile.NewProjectile(source, from, (Main.MouseWorld - from).SafeNormalize(Vector2.UnitY) * Math.Max(velocity.Length(), 12f), type,
                    (int)(damage * 0.3f), knockback, Player.whoAmI);
            }
            return true;
        }
    }

    public class Coverings1120bChecks : ModSystem
    {
        public override void Load()
        {
            SelfTest.ExtraChecks.Add(t =>
            {
                foreach (int type in new[] { ModContent.ItemType<ShadowspectreTracers>(), ModContent.ItemType<SpysLootbox>(), ModContent.ItemType<UniversalStone>(),
                    ModContent.ItemType<Weapons.Ranged.Earthshooter>() })
                    t.Expect(Main.recipe.Take(Recipe.numRecipes).Any(r => r.createItem.type == type && !r.Disabled), $"recette du wiki de {Lang.GetItemNameValue(type)}");
            });
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Ranged
{
    /// <summary>Wiki : Earthshooter.txt — amélioration de la Palladium Dartshooter : deux fléchettes chercheuses. [choix] Recherche à 500 px.</summary>
    public class Earthshooter : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 60;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item98;
            Item.width = 88;
            Item.height = 28;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 5);
            Item.shoot = ProjectileID.PoisonDartBlowgun;
            Item.shootSpeed = 32f;
            Item.useAmmo = AmmoID.Dart;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-14f, 0f);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = -1; i <= 1; i += 2)
            {
                int p = Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(3f * i)), type, damage, knockback, player.whoAmI);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].GetGlobalProjectile<EarthshooterDart>().Homing = true;
            }
            return false;
        }
    }

    public class EarthshooterDart : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public bool Homing;

        public override void PostAI(Projectile projectile)
        {
            if (!Homing)
                return;
            NPC t = projectile.FindTargetWithinRange(500f);
            if (t != null)
            {
                float speed = Math.Max(projectile.velocity.Length(), 10f);
                projectile.velocity = Vector2.Lerp(projectile.velocity, projectile.SafeDirectionTo(t.Center) * speed, 0.1f);
            }
        }
    }
}
