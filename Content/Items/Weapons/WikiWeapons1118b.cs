using System;
using CalamityFandom.Content.Projectiles.Weapons1118b;
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

// Armes inventées par le wiki (18/11, suite) : sprites et valeurs des pages, recettes du wiki (WikiRecipes).

namespace CalamityFandom.Content.Items.Weapons.Magic
{
    /// <summary>
    /// Wiki : Smoldering Seahorses.txt — grimoire qui lance des hippocampes enflammés à tête chercheuse ; sans ennemi
    /// proche, ils tombent et disparaissent.
    /// </summary>
    public class SmolderingSeahorses : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 27;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 5;
            Item.useTime = Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item20;
            Item.width = 26;
            Item.height = 34;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(silver: 15);
            Item.shoot = ModContent.ProjectileType<CinderSeahorse>();
            Item.shootSpeed = 3f;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) =>
            velocity = velocity.RotatedByRandom(0.5f);
    }

    /// <summary>
    /// Wiki : Aflaming Crystalliser.txt — orbes d'améthyste enflammées (On Fire! 8 s) qui ondulent et tirent des cristaux
    /// (70 % des dégâts) ; les cristaux éclatent en éclats qui ricochent (30 %).
    /// </summary>
    public class AflamingCrystalliser : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 40;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 14;
            Item.useTime = Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item43;
            Item.width = 54;
            Item.height = 78;
            Item.scale = 0.7f;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(silver: 80);
            Item.shoot = ModContent.ProjectileType<AmethystOrb>();
            Item.shootSpeed = 10f;
        }
    }

    /// <summary>
    /// Wiki : Moonstone Knives.txt — lance 4 ou 5 couteaux d'affilée qui rendent 3 mana chacun au contact et se brisent
    /// après une courte distance.
    /// </summary>
    public class MoonstoneKnives : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 3;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 5;
            Item.useTime = Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = Item.noUseGraphic = true;
            Item.knockBack = 2f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = Item.height = 24;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(silver: 40);
            Item.shoot = ModContent.ProjectileType<MoonstoneKnife>();
            Item.shootSpeed = 13f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int count = Main.rand.Next(4, 6);
            for (int i = 0; i < count; i++)
                Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.12f) * Main.rand.NextFloat(0.85f, 1.1f), type, damage, knockback, player.whoAmI);
            return false;
        }
    }

    /// <summary>
    /// Wiki : Ethereal Knives.txt — flot de 8 couteaux d'affilée qui rendent 3 mana chacun au contact et explosent.
    /// </summary>
    public class EtherealKnives : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 190;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.useTime = 3;
            Item.useAnimation = 24;
            Item.reuseDelay = 6;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = Item.noUseGraphic = true;
            Item.knockBack = 1f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item39;
            Item.width = Item.height = 52;
            Item.rare = ModContent.RarityType<CosmicPurple>();
            Item.value = Item.sellPrice(gold: 28);
            Item.shoot = ModContent.ProjectileType<EtherealKnife>();
            Item.shootSpeed = 15f;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) =>
            velocity = velocity.RotatedByRandom(0.08f);
    }

    /// <summary>
    /// Wiki : Heavenwing Staff.txt — cinq chauves-souris de feu sacré près du curseur, lancées à peu près vers le bas,
    /// à travers les blocs, sans percer ; Holy Flames 4 s.
    /// </summary>
    public class HeavenwingStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 33;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 18;
            Item.useTime = Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item8;
            Item.width = 52;
            Item.height = 56;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(silver: 80);
            Item.shoot = ModContent.ProjectileType<HeavenwingBat>();
            Item.shootSpeed = 9f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 5; i++)
            {
                Vector2 spawn = Main.MouseWorld + new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-160, -60));
                Vector2 v = Vector2.UnitY.RotatedByRandom(0.7f) * Item.shootSpeed * Main.rand.NextFloat(0.8f, 1.2f);
                Projectile.NewProjectile(source, spawn, v, type, damage, knockback, player.whoAmI);
            }
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Summon
{
    /// <summary>
    /// Wiki : Eye of Light.txt — petit œil qui flotte au-dessus du joueur ; près d'un ennemi, il s'y accroche et fait
    /// tomber des étoiles sacrées sur lui (pleins dégâts, Holy Flames 3 s).
    /// </summary>
    public class EyeofLight : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.GamepadWholeScreenUseRange[Type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 24;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 10;
            Item.useTime = Item.useAnimation = 29;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 1f;
            Item.UseSound = SoundID.Item44;
            Item.width = Item.height = 42;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(silver: 80);
            Item.buffType = ModContent.BuffType<Buffs.EyeofLightBuff>();
            Item.shoot = ModContent.ProjectileType<EyeofLightMinion>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            int p = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI);
            if (Main.projectile.IndexInRange(p))
                Main.projectile[p].originalDamage = Item.damage;
            return false;
        }
    }

    /// <summary>
    /// Wiki : Wooden Butterfly Staff.txt — papillons qui ne prennent pas de place de sbire (3 au plus), suivent le joueur et
    /// chargent les ennemis trois fois, puis disparaissent.
    /// </summary>
    public class WoodenButterflyStaff : ModItem
    {
        public const int MaxButterflies = 3;

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 12;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 4;
            Item.useTime = Item.useAnimation = 26;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 0f;
            Item.UseSound = SoundID.Item44;
            Item.width = 42;
            Item.height = 40;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 15);
            Item.shoot = ModContent.ProjectileType<ButterflyStaffMinion>();
            Item.shootSpeed = 10f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Trois au plus : le plus ancien disparaît
            int count = 0, oldest = -1;
            foreach (var p in Main.ActiveProjectiles)
                if (p.owner == player.whoAmI && p.type == type)
                {
                    count++;
                    if (oldest < 0 || p.timeLeft < Main.projectile[oldest].timeLeft)
                        oldest = p.whoAmI;
                }
            if (count >= MaxButterflies && oldest >= 0)
                Main.projectile[oldest].Kill();
            int b = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (Main.projectile.IndexInRange(b))
                Main.projectile[b].originalDamage = Item.damage;
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Rogue
{
    /// <summary>
    /// Wiki : Crustalline.txt — dague qui fait un tour autour du joueur puis tombe et se plante dans les blocs (bruit de
    /// cristal) ; Vaporfied 1,5 s ; frappe furtive : la dague éclate en fragments au bout de 30 images.
    /// </summary>
    public class Crustalline : RogueWeapon
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 21;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.useTime = Item.useAnimation = 19;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = Item.noUseGraphic = true;
            Item.knockBack = 2f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = 74;
            Item.height = 68;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(gold: 36);
            Item.shoot = ModContent.ProjectileType<CrustallineDagger>();
            Item.shootSpeed = 12f;
        }

        // 8 % de critique (4 de base + 4)
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 4;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (player.Calamity().StealthStrikeAvailable() && Main.projectile.IndexInRange(p))
                Main.projectile[p].Calamity().stealthStrike = true;
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Rogue
{
    /// <summary>
    /// Wiki : Wulfrum Shock Cutter.txt — rafales de 3 cutters soumis à la gravité, sans percer ; clic droit maintenu : les
    /// cutters en vol reviennent ; frappe furtive : cutters lumineux, plus rapides, qui frappent deux fois (80 % chacun).
    /// TODO : butin de The Inventors (boss inventé par le wiki, pas encore dans le mod).
    /// </summary>
    public class WulfrumShockCutter : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 9;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.useTime = 4;
            Item.useAnimation = 12;
            Item.reuseDelay = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = Item.noUseGraphic = true;
            Item.knockBack = 1f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = Item.height = 24;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 20);
            Item.shoot = ModContent.ProjectileType<ShockCutter>();
            Item.shootSpeed = 4f;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                // Aimante les cutters en vol vers le joueur
                foreach (var p in Main.ActiveProjectiles)
                    if (p.owner == player.whoAmI && p.type == type)
                    {
                        p.ai[1] = 1f;
                        p.netUpdate = true;
                    }
                return false;
            }
            bool stealth = player.Calamity().StealthStrikeAvailable();
            int c = Projectile.NewProjectile(source, position, velocity * (stealth ? 1.6f : 1f), type, stealth ? (int)(damage * 0.8f) : damage, knockback, player.whoAmI);
            if (stealth && Main.projectile.IndexInRange(c))
                Main.projectile[c].Calamity().stealthStrike = true;
            return false;
        }
    }
}

namespace CalamityFandom.Content.Buffs
{
    public class EyeofLightBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<EyeofLightMinion>()] > 0)
                player.buffTime[buffIndex] = 18000;
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Weapons1118b
{
    /// <summary>Hippocampe enflammé : cherche les ennemis à 500 px ; sinon tombe et disparaît.</summary>
    public class CinderSeahorse : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Magic/CinderSeahorse";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 240;
            Projectile.scale = 0.6f;
        }

        public override void AI()
        {
            NPC target = Projectile.Center.ClosestNPCAt(500f);
            if (target != null)
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(target.Center) * 9f, 0.08f);
            else
            {
                Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.2f, 12f);
                Projectile.alpha = Math.Min(Projectile.alpha + 3, 255);
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }
            Projectile.spriteDirection = Projectile.velocity.X > 0 ? 1 : -1;
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f);
            if (Main.rand.NextBool(2))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1.2f).noGravity = true;
            Lighting.AddLight(Projectile.Center, 0.6f, 0.3f, 0.1f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.OnFire, 120);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            SpriteEffects fx = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation, tex.Size() / 2f, Projectile.scale, fx);
            return false;
        }
    }

    /// <summary>Orbe d'améthyste enflammée : ondule, tire un cristal toutes les 20 images ; On Fire! 8 s.</summary>
    public class AmethystOrb : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            // Trajet en vague autour de la direction de départ
            Projectile.ai[0]++;
            Vector2 forward = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            Projectile.position += forward.RotatedBy(MathHelper.PiOver2) * (float)Math.Cos(Projectile.ai[0] * 0.2f) * 3f;
            if (Projectile.ai[0] % 20f == 0f && Projectile.owner == Main.myPlayer)
            {
                NPC target = Projectile.Center.ClosestNPCAt(400f);
                Vector2 v = (target != null ? Projectile.SafeDirectionTo(target.Center) : forward) * 9f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, v, ModContent.ProjectileType<OrbCrystal>(), (int)(Projectile.damage * 0.7f), Projectile.knockBack * 0.5f, Projectile.owner);
            }
            for (int i = 0; i < 2; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, i == 0 ? DustID.GemAmethyst : DustID.Torch, 0f, 0f, 100, default, 1.4f);
                d.noGravity = true;
                d.velocity *= 0.3f;
            }
            Lighting.AddLight(Projectile.Center, 0.6f, 0.2f, 0.7f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.OnFire, 480);
    }

    /// <summary>Cristal de l'orbe : éclate au contact en 3 éclats qui ricochent (30 % des dégâts de l'arme).</summary>
    public class OrbCrystal : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.CrystalShard;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 90;
        }

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        public override Color? GetAlpha(Color lightColor) => new Color(200, 120, 255, 200);

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27 with { Volume = 0.4f }, Projectile.Center);
            if (Projectile.owner != Main.myPlayer || Projectile.ai[0] == 1f)
                return;
            // 30 % / 70 % des dégâts du cristal
            for (int i = 0; i < 3; i++)
            {
                int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2CircularEdge(6f, 6f), Type,
                    (int)(Projectile.damage * 0.3f / 0.7f), 0f, Projectile.owner, 1f);
                if (Main.projectile.IndexInRange(p))
                {
                    Main.projectile[p].scale = 0.6f;
                    Main.projectile[p].timeLeft = 45;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // Les éclats ricochent
            if (Projectile.ai[0] != 1f)
                return true;
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }
    }

    /// <summary>Couteau de pierre de lune : rend 3 mana au contact, se brise vite.</summary>
    public class MoonstoneKnife : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Magic/MoonstoneKnife";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 25;
        }

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        public static void RestoreMana(Projectile projectile, int amount)
        {
            Player owner = Main.player[projectile.owner];
            if (projectile.owner != Main.myPlayer)
                return;
            owner.statMana = Math.Min(owner.statMana + amount, owner.statManaMax2);
            owner.ManaEffect(amount);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => RestoreMana(Projectile, 3);

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 4; i++)
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GemDiamond).noGravity = true;
        }
    }

    /// <summary>Couteau éthéré : rend 3 mana et explose au contact (80 px).</summary>
    public class EtherealKnife : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Magic/EtherealKnife";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 90;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Lighting.AddLight(Projectile.Center, 0.4f, 0.2f, 0.6f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => MoonstoneKnife.RestoreMana(Projectile, 3);

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14 with { Volume = 0.4f }, Projectile.Center);
            Projectile.ExpandHitboxBy(80);
            Projectile.maxPenetrate = Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.damage /= 2;
            Projectile.Damage();
            for (int i = 0; i < 20; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleTorch, 0f, 0f, 100, default, 1.6f);
                d.noGravity = true;
                d.velocity *= 2.5f;
            }
        }
    }

    /// <summary>Chauve-souris de feu sacré : traverse les blocs, ne perce pas, Holy Flames 4 s (texture de la Hellwing).</summary>
    public class HeavenwingBat : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.Hellwing;

        public override void SetStaticDefaults() => Main.projFrames[Type] = 3;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            Projectile.spriteDirection = Projectile.velocity.X > 0 ? 1 : -1;
            Projectile.rotation = Projectile.velocity.X * 0.05f;
            if (++Projectile.frameCounter > 4)
            {
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
                Projectile.frameCounter = 0;
            }
            if (Main.rand.NextBool(2))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GoldFlame, 0f, 0f, 100, default, 1.3f).noGravity = true;
            Lighting.AddLight(Projectile.Center, 0.9f, 0.8f, 0.4f);
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 230, 150, 200);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<HolyFlames>(), 240);
    }

    /// <summary>Œil de lumière : flotte au-dessus du joueur, s'accroche à l'ennemi le plus proche et fait tomber des étoiles.</summary>
    public class EyeofLightMinion : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Summon/EyeofLightMinion";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            Main.projPet[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 1f;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 18000;
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!owner.active || owner.dead || !owner.HasBuff<Buffs.EyeofLightBuff>())
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 2;
            NPC target = Projectile.Center.MinionHoming(800f, owner);
            Vector2 goal = target != null ? target.Top - Vector2.UnitY * 10f : owner.Center - new Vector2(0f, 60f + 30f * Projectile.minionPos);
            Projectile.Center = Vector2.Lerp(Projectile.Center, goal, target != null ? 0.25f : 0.1f);
            Projectile.rotation = 0f;
            Projectile.spriteDirection = owner.direction;
            if (target == null || Projectile.owner != Main.myPlayer || ++Projectile.ai[0] < 12f)
                return;
            // Étoile qui tombe sur la cible (pleins dégâts)
            Projectile.ai[0] = 0f;
            Vector2 sky = target.Center + new Vector2(Main.rand.Next(-80, 81), -500f);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), sky, (target.Center - sky).SafeNormalize(Vector2.UnitY) * 18f,
                ModContent.ProjectileType<LightStar>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }

    /// <summary>Étoile de l'Eye of Light (texture de l'étoile du Hallow) : Holy Flames 3 s.</summary>
    public class LightStar : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.HallowStar;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
        }

        public override void AI()
        {
            Projectile.rotation += 0.3f;
            if (Main.rand.NextBool(2))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.YellowStarDust).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
    }

    /// <summary>Papillon du Wooden Butterfly Staff : suit le joueur, charge les ennemis trois fois puis disparaît (pas de place de sbire).</summary>
    public class ButterflyStaffMinion : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Summon/ButterflyStaffMinion";

        public override void SetStaticDefaults() => ProjectileID.Sets.MinionTargettingFeature[Type] = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 3;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 36000;
            Projectile.scale = 0.6f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!owner.active || owner.dead)
            {
                Projectile.Kill();
                return;
            }
            NPC target = Projectile.Center.MinionHoming(600f, owner);
            Vector2 goal = target?.Center ?? owner.Center + new Vector2(-owner.direction * 40f, -40f) + new Vector2((float)Math.Sin(Projectile.timeLeft * 0.05f) * 20f, 0f);
            float speed = target != null ? 11f : 7f;
            Vector2 toGoal = goal - Projectile.Center;
            if (toGoal.Length() > 20f || target != null)
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, toGoal.SafeNormalize(Vector2.Zero) * speed, target != null ? 0.1f : 0.05f);
            if (Vector2.Distance(Projectile.Center, owner.Center) > 1500f)
                Projectile.Center = owner.Center;
            Projectile.spriteDirection = Projectile.velocity.X > 0 ? 1 : -1;
            Projectile.rotation = Projectile.velocity.X * 0.04f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Recule pour recharger
            Projectile.velocity = -Projectile.velocity * 0.8f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            SpriteEffects fx = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, fx);
            return false;
        }
    }

    /// <summary>Cutter Wulfrum : 5 mises à jour par image, gravité, ne perce pas (deux coups en frappe furtive), revient sur ai[1] = 1.</summary>
    public class ShockCutter : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Rogue/WulfrumShockCutter";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 5;
            Projectile.timeLeft = 600;
            Projectile.scale = 0.6f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Projectile.Calamity().stealthStrike)
                Projectile.penetrate = 2;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (Projectile.ai[1] == 1f)
            {
                Projectile.tileCollide = false;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(owner.Center) * 4f, 0.05f);
                if (Projectile.Hitbox.Intersects(owner.Hitbox))
                    Projectile.Kill();
            }
            else
                Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.012f, 3f);
            Projectile.rotation += 0.06f * Math.Sign(Projectile.velocity.X == 0f ? 1f : Projectile.velocity.X);
            if (Projectile.Calamity().stealthStrike && Main.rand.NextBool(6))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, 0f, 0f, 100, default, 0.7f).noGravity = true;
        }

        public override Color? GetAlpha(Color lightColor) => Projectile.Calamity().stealthStrike ? new Color(200, 255, 230, 200) : null;
    }

    /// <summary>Dague du Crustalline : un tour autour du joueur, puis tombe et se plante dans les blocs ; éclate en frappe furtive.</summary>
    public class CrustallineDagger : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Rogue/Crustalline";

        private bool stuck;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = 3;
            Projectile.timeLeft = 300;
            Projectile.scale = 0.5f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (Projectile.Calamity().stealthStrike && ++Projectile.localAI[1] == 30f)
            {
                Shatter();
                return;
            }
            if (stuck)
                return;
            if (++Projectile.ai[0] <= 40f)
            {
                // Un tour autour du joueur (40 images), à 70 px
                float angle = Projectile.velocity.ToRotation() + MathHelper.TwoPi * Projectile.ai[0] / 40f;
                Vector2 pos = owner.Center + angle.ToRotationVector2() * 70f;
                Projectile.rotation = angle + MathHelper.PiOver2 + MathHelper.PiOver4;
                Projectile.Center = pos;
                if (Projectile.ai[0] == 40f)
                    Projectile.velocity = (angle + MathHelper.PiOver2).ToRotationVector2() * 10f;
                else
                    Projectile.position -= Projectile.velocity;
                return;
            }
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.3f, 16f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!stuck)
            {
                stuck = true;
                Projectile.velocity = Vector2.Zero;
                Projectile.timeLeft = 120;
                SoundEngine.PlaySound(SoundID.Shatter with { Volume = 0.5f, Pitch = 0.4f }, Projectile.Center);
            }
            return false;
        }

        private void Shatter()
        {
            SoundEngine.PlaySound(SoundID.Shatter with { Volume = 0.6f }, Projectile.Center);
            if (Projectile.owner == Main.myPlayer)
                for (int i = 0; i < 5; i++)
                {
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2CircularEdge(7f, 7f),
                        ModContent.ProjectileType<OrbCrystal>(), Projectile.damage / 2, 0f, Projectile.owner, 1f);
                    if (Main.projectile.IndexInRange(p))
                    {
                        Main.projectile[p].DamageType = ModContent.GetInstance<RogueDamageClass>();
                        Main.projectile[p].timeLeft = 40;
                    }
                }
            Projectile.Kill();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Vaporfied>(), 90);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None);
            return false;
        }
    }
}
