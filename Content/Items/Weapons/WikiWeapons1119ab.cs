using System;
using System.Collections.Generic;
using CalamityFandom.Content.Projectiles.Weapons1119ab;
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
using Dart = CalamityFandom.Content.Projectiles.WikiDarts.WikiDartProj;

// Lot AJ (19/11) : armes, munition, potion et matériaux du wiki (recettes du wiki). Rareté « C1 » → 0.

namespace CalamityFandom.Content.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Titan Tides.txt — tenue : une aura cosmique qui tourne autour du joueur, grandit (jusqu'à x3) et tourne plus
    /// vite (jusqu'à x5) ; ses dégâts montent aussi ; les ennemis touchés reçoivent une volée de lames de God Slayer ; Shred
    /// (10 %, 6 s) à partir de ~4,5 s. [choix] Pleine puissance en 6 s ; dégâts x1 à x3 ; 3 lames à 20 % par coup.
    /// </summary>
    public class TitanTides : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 320;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 2;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = Item.noUseGraphic = true;
            Item.channel = true;
            Item.knockBack = 2f;
            Item.autoReuse = true;
            Item.width = 150;
            Item.height = 148;
            Item.rare = ModContent.RarityType<CosmicPurple>();
            Item.value = Item.sellPrice(gold: 64);
            Item.shoot = ModContent.ProjectileType<TitanAura>();
            Item.shootSpeed = 0f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;
    }
}

namespace CalamityFandom.Content.Items.Weapons.Rogue
{
    /// <summary>
    /// Wiki : Gearwork Shield.txt — clic droit : photo de l'état de tous les ennemis ; clic gauche : ils reviennent à la
    /// photo (recharge 1 min) ; pendant la recharge, le bouclier se lance comme un boomerang, et chaque coup retire 1 s de
    /// recharge. Frappe furtive : les ennemis remis en arrière prennent 20 fois les dégâts de l'arme.
    /// [choix] La photo garde la position, la vitesse, les PV et les ai[] ; frappe furtive = la furtivité pleine au moment du
    /// retour en arrière. [technique] En multijoueur, seul le serveur peut remettre les ennemis en arrière (pas fait).
    /// </summary>
    public class GearworkShield : RogueWeapon
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 66;
            Item.height = 64;
            Item.damage = 167;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.knockBack = 4.25f;
            Item.useTime = Item.useAnimation = 23;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<GearworkShieldProj>();
            Item.shootSpeed = 13f;
            Item.value = Item.sellPrice(gold: 20);
            Item.rare = ModContent.RarityType<PureGreen>();
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            var mp = player.GetModPlayer<GearworkPlayer>();
            if (player.altFunctionUse == 2)
            {
                mp.TakeSnapshot();
                return false;
            }
            if (mp.Cooldown <= 0 && mp.Snapshot != null)
            {
                bool stealth = player.Calamity().StealthStrikeAvailable();
                mp.Rewind(stealth ? player.GetWeaponDamage(Item) * 20 : 0);
                if (stealth)
                    player.Calamity().ConsumeStealthByAttacking();
                return false;
            }
            return mp.Cooldown > 0 && player.ownedProjectileCounts[Item.shoot] < 1;
        }
    }

    public class GearworkPlayer : ModPlayer
    {
        public int Cooldown;
        public List<(int whoAmI, int type, Vector2 position, Vector2 velocity, int life, float[] ai)> Snapshot;

        public override void PostUpdate()
        {
            if (Cooldown > 0)
                Cooldown--;
        }

        public void TakeSnapshot()
        {
            Snapshot = new();
            foreach (NPC n in Main.ActiveNPCs)
                if (n.CanBeChasedBy())
                    Snapshot.Add((n.whoAmI, n.type, n.position, n.velocity, n.life, (float[])n.ai.Clone()));
            SoundEngine.PlaySound(SoundID.Item4 with { Pitch = 0.5f }, Player.Center);
        }

        public void Rewind(int stealthDamage)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            foreach (var s in Snapshot)
            {
                NPC n = Main.npc[s.whoAmI];
                if (!n.active || n.type != s.type)
                    continue;
                n.position = s.position;
                n.velocity = s.velocity;
                n.life = Math.Min(n.lifeMax, s.life);
                Array.Copy(s.ai, n.ai, Math.Min(s.ai.Length, n.ai.Length));
                n.netUpdate = true;
                if (!Main.dedServ)
                    for (int i = 0; i < 10; i++)
                        Dust.NewDustDirect(n.position, n.width, n.height, DustID.GoldCoin, 0f, 0f, 100, default, 1.2f).noGravity = true;
                if (stealthDamage > 0)
                    Player.ApplyDamageToNPC(n, stealthDamage, 0f, 0, false, ModContent.GetInstance<RogueDamageClass>());
            }
            Snapshot = null;
            Cooldown = 3600;
            SoundEngine.PlaySound(SoundID.Item117, Player.Center);
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Typeless
{
    /// <summary>
    /// Wiki : Tyrant Shield.txt — tenu : le cadavre de Yharim devant le joueur ; un ennemi qui fonce dessus à l'horizontale
    /// prend 5980 dégâts (sans classe, avec les bonus de dégâts) ; un projectile qui arrive à l'horizontale est renvoyé,
    /// devient ami et fait 5980 dégâts ; après un renvoi, plus utilisable pendant 1 min. Les rayons ne sont pas renvoyés.
    /// [choix] « Rayon » = projectile immobile ou de plus de 64 px.
    /// </summary>
    public class TyrantShield : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 5980;
            Item.DamageType = DamageClass.Generic;
            Item.useTime = Item.useAnimation = 2;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = Item.noUseGraphic = true;
            Item.channel = true;
            Item.autoReuse = true;
            Item.width = 50;
            Item.height = 68;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
            Item.value = Item.sellPrice(gold: 90);
            Item.shoot = ModContent.ProjectileType<TyrantCorpse>();
            Item.shootSpeed = 1f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1
            && player.GetModPlayer<Ranged.OnesStrikerPlayer>().TyrantCooldown <= 0;
    }
}

namespace CalamityFandom.Content.Items.Weapons.Ranged
{
    /// <summary>
    /// Wiki : The Ones Striker.txt — tire des chaînes de pièces (comme un Coin Gun), y compris des pièces que les Coin Guns
    /// ne prennent pas (Cosmilite Coin). Par pièce : cuivre 60 x3, argent 100 x4, or 180 x5, platine 260 x6, Cosmilite 380
    /// x8 ; Midas : argent 1 s, or 10 s, platine 100 s, Cosmilite 1000 s. [technique] Klepticoin et Miracle Coin n'existent
    /// pas encore. [choix] Les pièces d'une chaîne se suivent à 0,05 s d'écart ; dégâts de distance.
    /// </summary>
    public class TheOnesStriker : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 1;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 9f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item11;
            Item.width = 234;
            Item.height = 104;
            Item.scale = 0.5f;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.value = Item.sellPrice(gold: 30);
            Item.shoot = ModContent.ProjectileType<StrikerCoin>();
            Item.shootSpeed = 24f;
            Item.useAmmo = AmmoID.Coin;
        }

        public override bool? CanChooseAmmo(Item ammo, Player player) => ammo.type == ModContent.ItemType<Ammo.CosmiliteCoin>() || ammo.type == ModContent.ItemType<Ammo.MiracleCoin>() ? true : null;

        public static (int tier, int damage, int count) Coin(int ammo) => ammo switch
        {
            ItemID.CopperCoin => (0, 60, 3),
            ItemID.SilverCoin => (1, 100, 4),
            ItemID.GoldCoin => (2, 180, 5),
            ItemID.PlatinumCoin => (3, 260, 6),
            _ when ammo == ModContent.ItemType<Ammo.Klepticoin>() => (5, 560, 10),
            _ when ammo == ModContent.ItemType<Ammo.MiracleCoin>() => (6, 9999, 14),
            _ => (4, 380, 8),
        };

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var (tier, coinDamage, count) = Coin(source.AmmoItemIdUsed);
            int dmg = (int)player.GetTotalDamage(DamageClass.Ranged).ApplyTo(coinDamage);
            for (int i = 0; i < count; i++)
                Projectile.NewProjectile(source, position - velocity * i * 0.15f, velocity, ModContent.ProjectileType<StrikerCoin>(), dmg, knockback, player.whoAmI, tier);
            return false;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-30f, 0f);
    }

    public class OnesStrikerPlayer : ModPlayer
    {
        public int TyrantCooldown;

        public override void PostUpdate()
        {
            if (TyrantCooldown > 0)
                TyrantCooldown--;
        }
    }
}

namespace CalamityFandom.Content.Items.Ammo
{
    /// <summary>
    /// Wiki : Cosmilite Coin.txt — pièce qui vaut 100 Platinum Coins ; munition de The Ones Striker seulement (la page le dit
    /// avec l'addon Tales of the Aquatic, traité comme le reste du wiki). [choix] Pas d'échange automatique avec les pièces
    /// de platine (la recette du wiki suffit).
    /// </summary>
    public class CosmiliteCoin : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 100;

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 18;
            Item.maxStack = 999;
            Item.consumable = true;
            Item.ammo = Type;
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(platinum: 100);
        }
    }
}

namespace CalamityFandom.Content.Items.Potions
{
    /// <summary>Wiki : Esseric Skin Potion.txt — Esseric Skin 8 min : +30 défense ; rareté « C1 » → 0.</summary>
    public class EssericSkinPotion : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 20;

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 36;
            Item.useTime = Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.UseSound = SoundID.Item3;
            Item.useTurn = true;
            Item.consumable = true;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(gold: 20);
            Item.buffType = ModContent.BuffType<Buffs.EssericSkin>();
            Item.buffTime = 8 * 60 * 60;
        }
    }
}

namespace CalamityFandom.Content.Items.Materials
{
    /// <summary>Matériau du wiki sans effet (pages d'objets « Item »).</summary>
    public abstract class WikiPlainItem : ModItem
    {
        protected abstract int Rarity { get; }
        protected abstract int SellCopper { get; }
        protected virtual int MaxStack => 9999;

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 25;

        public override void SetDefaults()
        {
            Item.width = Item.height = 20;
            Item.maxStack = MaxStack;
            Item.rare = Rarity;
            Item.value = SellCopper * 5;
        }
    }

    /// <summary>Wiki : Lunar Flah.txt — obtenue en fabriquant avec la Meld Flah ; « No value ».</summary>
    public class LunarFlah : WikiPlainItem
    {
        protected override int Rarity => ItemRarityID.Red;
        protected override int SellCopper => 0;
    }

    /// <summary>
    /// Wiki : Merge Flah.txt — obtenue avec la Cosmic Flah ; jetée dans la lave avec un Energy Core, un Yharon Soul Fragment,
    /// une Alloy Bar et une Meld Bar, elle donne une Desolate Flah (DesolateFlahLava, lot BD).
    /// </summary>
    public class MergeFlah : WikiPlainItem
    {
        protected override int Rarity => ModContent.RarityType<CosmicPurple>();
        protected override int SellCopper => 150000;
    }

    /// <summary>
    /// Wiki : Uncharged Flahium.txt — après 60 minutes de jeu sans servir, devient une Flahium Bar. [choix] 60 minutes de
    /// jeu = 60 s réelles, comptées dans l'inventaire ; tout le paquet se transforme d'un coup.
    /// </summary>
    public class UnchargedFlahium : WikiPlainItem
    {
        protected override int Rarity => ModContent.RarityType<HotPink>();
        protected override int SellCopper => 0;

        private int timer;

        public override void UpdateInventory(Player player)
        {
            if (++timer < 3600)
                return;
            int bar = Common.WikiData.WikiRecipes.ResolveItem("Flahium Bar");
            if (bar <= 0)
                return;
            int stack = Item.stack;
            Item.SetDefaults(bar);
            Item.stack = stack;
        }
    }

    /// <summary>
    /// Wiki : Soul Container.txt — sert à garder l'âme d'un élémentaire et à l'emporter dans un autre monde.
    /// [technique] Les âmes d'élémentaires n'existent pas encore : aucun effet.
    /// </summary>
    public class SoulContainer : WikiPlainItem
    {
        protected override int Rarity => ItemRarityID.White;
        protected override int SellCopper => 0;
        protected override int MaxStack => 1;
    }
}

namespace CalamityFandom.Content.Buffs
{
    public class EssericSkin : ModBuff
    {
        public override string Texture => "CalamityFandom/Content/Buffs/Wiki/EssericSkin";

        public override void Update(Player player, ref int buffIndex) => player.statDefense += 30;
    }
}

namespace CalamityFandom.Content.Projectiles.Weapons1119ab
{
    /// <summary>Aura de Titan Tides : grandit (x1 → x3) et tourne plus vite (x1 → x5) en 6 s ; dégâts x1 → x3.</summary>
    public class TitanAura : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Melee/TitanTides";

        private float Power => Math.Min(1f, Projectile.ai[0] / 360f);

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
            => Vector2.Distance(targetHitbox.Center.ToVector2(), Projectile.Center) < 60f * (1f + 2f * Power);

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
            owner.heldProj = Projectile.whoAmI;
            Projectile.Center = owner.MountedCenter;
            Projectile.ai[0]++;
            Projectile.rotation += 0.1f * (1f + 4f * Power);
            Lighting.AddLight(Projectile.Center, 0.5f, 0.2f, 0.8f);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => modifiers.SourceDamage *= 1f + 2f * Power;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[0] >= 270f && Main.rand.NextBool(10))
            {
                target.AddBuff(ModContent.BuffType<Buffs.TendonShred>(), 360);
                var g = target.GetGlobalNPC<Buffs.WikiDebuffsAANPC>();
                g.ShredStacks = Math.Min(64, g.ShredStacks + 1);
            }
            if (Projectile.owner != Main.myPlayer)
                return;
            for (int i = 0; i < 3; i++)
            {
                Vector2 from = target.Center + Main.rand.NextVector2CircularEdge(200f, 200f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), from, (target.Center - from).SafeNormalize(Vector2.UnitY) * 18f,
                    ModContent.ProjectileType<GodSlayerBlade>(), Projectile.damage / 5, 0f, Projectile.owner);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            float scale = 0.8f * (1f + 2f * Power);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, new Color(200, 120, 255, 120), Projectile.rotation, tex.Size() / 2f, scale, SpriteEffects.None);
            return false;
        }
    }

    /// <summary>Lame de God Slayer de Titan Tides (20 %) : file vers l'ennemi et le traverse.</summary>
    public class GodSlayerBlade : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.LightBeam;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 30;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

        public override Color? GetAlpha(Color lightColor) => new Color(255, 80, 220, 120);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 60);
    }

    /// <summary>Gearwork Shield lancé (pendant la recharge) : boomerang ; chaque coup retire 1 s de recharge.</summary>
    public class GearworkShieldProj : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Rogue/GearworkShield";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.scale = 0.6f;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            Projectile.rotation += 0.4f;
            if (++Projectile.ai[0] > 30f)
            {
                Vector2 back = owner.Center - Projectile.Center;
                if (back.Length() < 30f)
                {
                    Projectile.Kill();
                    return;
                }
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, back.SafeNormalize(Vector2.Zero) * 16f, 0.1f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            var mp = Main.player[Projectile.owner].GetModPlayer<Items.Weapons.Rogue.GearworkPlayer>();
            mp.Cooldown = Math.Max(0, mp.Cooldown - 60);
        }
    }

    /// <summary>
    /// Cadavre de Yharim (Tyrant Shield) : devant le joueur tant qu'on tient le clic ; frappe un ennemi qui fonce dessus ou
    /// renvoie un projectile qui arrive à l'horizontale (5980 dégâts) ; ensuite 1 min sans pouvoir le tenir.
    /// </summary>
    public class TyrantCorpse : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Typeless/TyrantShield";

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 60;
            Projectile.friendly = false;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            var mp = owner.GetModPlayer<Items.Weapons.Ranged.OnesStrikerPlayer>();
            if (!owner.channel || owner.dead || owner.noItems || owner.CCed || mp.TyrantCooldown > 0)
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 2;
            owner.itemTime = owner.itemAnimation = 2;
            owner.heldProj = Projectile.whoAmI;
            if (Projectile.owner == Main.myPlayer)
                owner.ChangeDir(Main.MouseWorld.X > owner.Center.X ? 1 : -1);
            int dir = owner.direction;
            Projectile.Center = owner.MountedCenter + new Vector2(dir * 30f, 0f);
            Projectile.spriteDirection = dir;
            if (Projectile.owner != Main.myPlayer)
                return;
            int damage = (int)owner.GetTotalDamage(DamageClass.Generic).ApplyTo(Projectile.originalDamage > 0 ? Projectile.originalDamage : Projectile.damage);
            foreach (NPC n in Main.ActiveNPCs)
                if (!n.friendly && !n.dontTakeDamage && n.Hitbox.Intersects(Projectile.Hitbox) && Math.Sign(-n.velocity.X) == dir && Math.Abs(n.velocity.X) > 0.5f)
                {
                    owner.ApplyDamageToNPC(n, damage, 8f, dir, false, DamageClass.Generic);
                    Deflected(mp);
                    return;
                }
            foreach (Projectile p in Main.ActiveProjectiles)
                if (p.hostile && !p.friendly && p.Hitbox.Intersects(Projectile.Hitbox) && Math.Sign(-p.velocity.X) == dir && Math.Abs(p.velocity.X) > 0.5f
                    && p.width <= 64 && p.height <= 64)
                {
                    p.hostile = false;
                    p.friendly = true;
                    p.owner = owner.whoAmI;
                    p.velocity.X = -p.velocity.X * 1.5f;
                    p.damage = damage;
                    p.DamageType = DamageClass.Generic;
                    p.netUpdate = true;
                    Deflected(mp);
                    return;
                }
        }

        private void Deflected(Items.Weapons.Ranged.OnesStrikerPlayer mp)
        {
            mp.TyrantCooldown = 3600;
            SoundEngine.PlaySound(SoundID.NPCHit4, Projectile.Center);
            Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor, 0f, tex.Size() / 2f, 1f,
                Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            return false;
        }
    }

    /// <summary>Pièce de The Ones Striker : ai[0] 0 cuivre … 3 platine, 4 Cosmilite, 5 Klepticoin, 6 Miracle Coin ; Midas selon la pièce (sans fin pour 5 et 6).</summary>
    public class StrikerCoin : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.GoldCoin;

        private static readonly int[] MidasTime = { 0, 60, 600, 6000, 60000 };
        private static readonly Color[] Tints = { new(220, 120, 60), new(210, 210, 220), new(255, 220, 80), new(200, 230, 255), Color.White };

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 150;
            Projectile.extraUpdates = 1;
        }

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int t = Projectile.ai[0] >= 5f ? int.MaxValue / 2 : MidasTime[(int)Projectile.ai[0] % 5];
            if (t > 0)
                target.AddBuff(BuffID.Midas, t);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            string shot = Projectile.ai[0] switch { 4f => "CsShot", 5f => "KShot", 6f => "MShot", _ => null };
            Texture2D tex = shot != null ? ModContent.Request<Texture2D>("CalamityFandom/Content/Projectiles/Ranged/" + shot).Value : TextureAssets.Projectile[Type].Value;
            Color c = shot != null ? Color.White : Tints[(int)Projectile.ai[0] % 5];
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, c, Projectile.rotation, tex.Size() / 2f, 1f, SpriteEffects.None);
            return false;
        }
    }
}
