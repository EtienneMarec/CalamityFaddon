using System;
using System.Linq;
using CalamityFandom.Common.Debug;
using CalamityMod;
using CalamityMod.CalPlayer.Dashes;
using CalamityMod.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

// Lot BE (20/11) : butin de Polyphemalus (l'œil tué en dernier donne son objet ; le défi donne le Polypebral Shield).

namespace CalamityFandom.Content.Items.Weapons.Ranged
{
    /// <summary>
    /// Wiki : Heterochromia.txt — arc : sept arcs de quatre flèches très vite, qui deviennent tour à tour des flèches de
    /// Frostburn, maudites, d'ombre (qui percent) et de feu ; 90 % de chances de ne pas consommer de munition (Astigmageddon
    /// tué en dernier). [choix] Arc de 15° ; une salve toutes les 12 images.
    /// </summary>
    public class Heterochromia : ModItem
    {
        private static readonly int[] Arrows = { ProjectileID.FrostburnArrow, ProjectileID.CursedArrow, ProjectileID.ShadowFlameArrow, ProjectileID.FireArrow };

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 30;
            Item.DamageType = DamageClass.Ranged;
            Item.useAnimation = 84;
            Item.useTime = 12;
            Item.reuseDelay = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 8f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item5;
            Item.width = 26;
            Item.height = 64;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 10);
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.Arrow;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.NextFloat() >= 0.9f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 4; i++)
            {
                int p = Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(-7.5f + 5f * i)), Arrows[i], damage, knockback, player.whoAmI);
                if (Arrows[i] == ProjectileID.ShadowFlameArrow && Main.projectile.IndexInRange(p))
                    Main.projectile[p].penetrate = 3;
            }
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Chain-Saw.txt — chaque coup ajoute de l'obscurité (10, 20 sur un critique ; 600 au plus), qui baisse de 1 toutes
    /// les 4 images après 1 s sans frapper ; Darkness dès qu'il y en a, Blackout au-delà de 30 %, Obstructed au-delà de 60 % ;
    /// le critique monte avec (0 % à vide, 100 % à 90 %) et ne profite d'aucun autre bonus ; clic droit : tire des yeux selon
    /// l'obscurité et la remet à zéro (Exotrexia tuée en dernier).
    /// [choix] Un œil chercheur par tranche de 100 d'obscurité ; l'arme se balance comme une épée.
    /// </summary>
    public class ChainSaw : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 125;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6.5f;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.UseSound = SoundID.Item23;
            Item.width = 62;
            Item.height = 28;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 10);
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            var cp = player.GetModPlayer<ChainSawPlayer>();
            if (player.altFunctionUse == 2)
            {
                Item.noMelee = true;
                return cp.Charge >= 100;
            }
            Item.noMelee = false;
            return true;
        }

        public override bool? UseItem(Player player)
        {
            var cp = player.GetModPlayer<ChainSawPlayer>();
            if (player.altFunctionUse == 2 && player.whoAmI == Main.myPlayer)
            {
                int eyes = cp.Charge / 100;
                for (int i = 0; i < eyes; i++)
                    Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX).RotatedByRandom(0.4f) * 12f,
                        ModContent.ProjectileType<Projectiles.Polyphemalus.MiniEye>(), player.GetWeaponDamage(Item), 2f, player.whoAmI, i % 4);
                cp.Charge = 0;
            }
            return true;
        }

        public override void ModifyWeaponCrit(Player player, ref float crit) => crit = Math.Min(100f, player.GetModPlayer<ChainSawPlayer>().Charge / 540f * 100f);

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            var cp = player.GetModPlayer<ChainSawPlayer>();
            cp.Charge = Math.Min(600, cp.Charge + (hit.Crit ? 20 : 10));
            cp.Decay = 60;
        }
    }

    public class ChainSawPlayer : ModPlayer
    {
        public int Charge, Decay;

        public override void PostUpdate()
        {
            if (Decay > 0)
                Decay--;
            else if (Charge > 0 && Main.GameUpdateCount % 4 == 0)
                Charge--;
            if (Charge <= 0)
                return;
            Player.AddBuff(BuffID.Darkness, 2);
            if (Charge > 180)
                Player.AddBuff(BuffID.Blackout, 2);
            if (Charge > 360)
                Player.AddBuff(BuffID.Obstructed, 2);
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Magic
{
    /// <summary>
    /// Wiki : Nystagmus.txt — une spirale de 36 yeux chercheurs, un par image (Conjunctivirus tué en dernier).
    /// [choix] 15 mana par spirale.
    /// </summary>
    public class Nystagmus : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 15;
            Item.useTime = Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 8f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item8;
            Item.width = 70;
            Item.height = 56;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 10);
            Item.shoot = ModContent.ProjectileType<Projectiles.Polyphemalus.EyeSpiral>();
            Item.shootSpeed = 1f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI);
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Summon
{
    /// <summary>
    /// Wiki : Tetrachromancy.txt — un mini-Polyphemalus qui suit le curseur tant que le clic est maintenu ; relâché (après
    /// une demi-seconde), ou contre un bloc ou un ennemi, il éclate en quatre yeux chercheurs (Cataractacomb tué en dernier).
    /// [choix] 10 mana.
    /// </summary>
    public class Tetrachromancy : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 80;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 10;
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.channel = true;
            Item.knockBack = 6.5f;
            Item.UseSound = SoundID.Item20;
            Item.width = 40;
            Item.height = 50;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 10);
            Item.shoot = ModContent.ProjectileType<Projectiles.Polyphemalus.MiniPolyphemalus>();
            Item.shootSpeed = 10f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;
    }
}

namespace CalamityFandom.Content.Items.Accessories
{
    /// <summary>
    /// Wiki : Polypebral Shield.txt — 8 défense ; ruée dans toutes les directions qui cogne les ennemis (80 dégâts), 12
    /// images d'invulnérabilité par ennemi touché ; longue recharge (défi de Polyphemalus réussi).
    /// [technique] Ricochets (rappuyer juste avant l'impact) et sons de ricochet non faits ; recharge : celle des ruées de Calamity.
    /// </summary>
    public class PolypebralShield : WikiCovering
    {
        protected override int Rarity => ItemRarityID.Pink;
        protected override int SellCopper => 48000;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.defense = 8;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().DashID = PolypebralDash.ID;
            player.dashType = 0;
        }
    }

    public class PolypebralDash : PlayerDashEffect
    {
        public static new string ID { get; private set; }

        public override DashCollisionType CollisionType => DashCollisionType.ShieldBonk;

        public override bool IsOmnidirectional => true;

        public override void Load() => ID = DashID;

        public override float CalculateDashSpeed(Player player) => 17f;

        public override void MidDashEffects(Player player, ref float dashSpeed, ref float dashSpeedDecelerationFactor, ref float runSpeedDecelerationFactor)
        {
            if (!Main.dedServ)
                Dust.NewDustDirect(player.position, player.width, player.height, DustID.Blood, 0f, 0f, 100, default, 1.2f).noGravity = true;
        }

        public override void OnHitEffects(Player player, NPC npc, IEntitySource source, ref DashHitContext hitContext)
        {
            hitContext.HitDirection = npc.Center.X > player.Center.X ? 1 : -1;
            hitContext.PlayerImmunityFrames = 12;
            hitContext.damageClass = DamageClass.Melee;
            hitContext.BaseDamage = 80;
            hitContext.BaseKnockback = 7f;
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Polyphemalus
{
    /// <summary>Petit œil chercheur (Chain-Saw, Nystagmus, Tetrachromancy) : ai[0] = lequel des quatre (texture de l'œil).</summary>
    public class MiniEye : ModProjectile
    {
        private static readonly string[] Eyes = { "Astigmageddon", "Exotrexia", "Conjunctivirus", "Cataractacomb" };

        public override string Texture => "CalamityFandom/Content/NPCs/Polyphemalus/Astigmageddon";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse use)
                Projectile.DamageType = use.Item.DamageType;
            else if (source is EntitySource_Parent parent && parent.Entity is Projectile p)
                Projectile.DamageType = p.DamageType;
        }

        public override void AI()
        {
            if (++Projectile.localAI[0] > 10)
            {
                NPC t = Projectile.FindTargetWithinRange(700f);
                if (t != null)
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(t.Center) * 13f, 0.08f);
            }
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("CalamityFandom/Content/NPCs/Polyphemalus/" + Eyes[(int)Projectile.ai[0] % 4]).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, 0.2f, SpriteEffects.None);
            return false;
        }
    }

    /// <summary>Spirale du Nystagmus : un œil par image pendant 36 images, autour du joueur.</summary>
    public class EyeSpiral : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 36;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            Projectile.Center = owner.Center;
            if (Projectile.owner != Main.myPlayer)
                return;
            float angle = Projectile.localAI[0]++ / 36f * MathHelper.TwoPi * 2f;
            int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), owner.Center, angle.ToRotationVector2() * 8f, ModContent.ProjectileType<MiniEye>(), Projectile.damage,
                Projectile.knockBack, Projectile.owner, Projectile.localAI[0] % 4);
            if (Main.projectile.IndexInRange(p))
                Main.projectile[p].DamageType = DamageClass.Magic;
        }
    }

    /// <summary>Mini-Polyphemalus (Tetrachromancy) : suit le curseur tant que le clic est tenu ; éclate en quatre yeux.</summary>
    public class MiniPolyphemalus : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/NPCs/Polyphemalus/Polyphemalus";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (Projectile.owner == Main.myPlayer)
            {
                if (owner.channel && Projectile.ai[1] == 0f)
                {
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero) * 12f, 0.1f);
                    if (Main.GameUpdateCount % 10 == 0)
                        Projectile.netUpdate = true;
                }
                else if (Projectile.ai[1] == 0f)
                {
                    Projectile.ai[1] = 1f;
                    Projectile.timeLeft = 30;
                    Projectile.netUpdate = true;
                }
            }
            Projectile.rotation += 0.1f;
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner != Main.myPlayer)
                return;
            for (int i = 0; i < 4; i++)
            {
                int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (MathHelper.PiOver2 * i).ToRotationVector2() * 9f, ModContent.ProjectileType<MiniEye>(),
                    Projectile.damage, Projectile.knockBack, Projectile.owner, i);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].DamageType = DamageClass.Summon;
            }
            SoundEngine.PlaySound(SoundID.NPCDeath1 with { Volume = 0.5f }, Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, 0.15f, SpriteEffects.None);
            return false;
        }
    }

    public class PolyphemalusChecks : ModSystem
    {
        public override void Load()
        {
            SelfTest.ExtraChecks.Add(t =>
            {
                int eye = ModContent.ItemType<Items.SummonItems.FusedEye>();
                t.Expect(Main.recipe.Take(Recipe.numRecipes).Any(r => r.createItem.type == eye && !r.Disabled), "recette du wiki du Fused Eye");
                t.Expect(Common.Taming.TamingSystem.EntryOf(ModContent.NPCType<NPCs.Polyphemalus.Polyphemalus>()) != null, "Polyphemalus a une méthode de Taming");
            });
        }
    }
}
