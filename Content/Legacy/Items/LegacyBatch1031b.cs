// Adapté de Calamity Mod (versions mensuelles de 2022, legacy_extract.py), © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

// Lot du 31/10 : objets retirés qui débloquent des recettes du wiki (Terra Edge, Lightning Hawk, Plague Hive) et leurs
// prédécesseurs. Valeurs des pages là où elles diffèrent du code ; recettes du wiki (WikiRecipes), pas d'AddRecipes.

namespace CalamityFandom.Content.Legacy.Items.Weapons.Melee
{
    /// <summary>Soin au toucher des épées sanglantes (pas sur les mannequins ni avec Moon Bite).</summary>
    internal static class SwordLifesteal
    {
        public static void Heal(Player player, NPC target, int min, int max, float chance)
        {
            if (!target.canGhostHeal || player.moonLeech || Main.rand.NextFloat() >= chance)
                return;
            int amount = Main.rand.Next(min, max + 1);
            player.statLife += amount;
            player.HealEffect(amount);
        }
    }

    /// <summary>Wiki : Bloody Edge.txt — Burning Blood 1 s ; 50 % de chances de rendre 2 à 3 PV (page ; le code soignait à chaque coup).</summary>
    public class BloodyEdge : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 60;
            Item.scale = 1.25f;
            Item.damage = 70;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.knockBack = 5.25f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = Item.sellPrice(silver: 80);
            Item.rare = ItemRarityID.Orange;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
                Dust.NewDust(hitbox.TopLeft(), hitbox.Width, hitbox.Height, DustID.Blood);
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<BurningBlood>(), 60);
            SwordLifesteal.Heal(player, target, 2, 3, 0.5f);
        }
    }

    /// <summary>
    /// Wiki : True Bloody Edge.txt — lame jaune qui traverse 2 ennemis ; l'épée et la lame infligent Ichor et Burning Blood
    /// 1 s ; l'épée a 50 % de chances de rendre 1 à 6 PV (page ; 3 à 5 dans le code).
    /// </summary>
    public class TrueBloodyEdge : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 64;
            Item.scale = 1.5f;
            Item.damage = 75;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 19;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.Melee.BloodyBlade>();
            Item.shootSpeed = 11f;
            Item.value = Item.sellPrice(gold: 16);
            Item.rare = ItemRarityID.Yellow;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
                Dust.NewDust(hitbox.TopLeft(), hitbox.Width, hitbox.Height, DustID.Blood);
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Ichor, 60);
            target.AddBuff(ModContent.BuffType<BurningBlood>(), 60);
            SwordLifesteal.Heal(player, target, 1, 6, 0.5f);
        }
    }

    /// <summary>
    /// Wiki : Terra Edge.txt — rayon à 75 % des dégâts (Ichor 0,75 s) ; l'épée rend 2 à 3 PV et inflige Ichor 1,5 s.
    /// </summary>
    public class TerraEdge : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = Item.height = 58;
            Item.scale = 1.5f;
            Item.damage = 112;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.knockBack = 6.25f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.Melee.TerraEdgeBeam>();
            Item.shootSpeed = 12f;
            Item.value = Item.sellPrice(gold: 16);
            Item.rare = ItemRarityID.Yellow;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) =>
            damage = (int)(damage * 0.75f);

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
                Dust.NewDust(hitbox.TopLeft(), hitbox.Width, hitbox.Height, DustID.TerraBlade);
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Ichor, 90);
            SwordLifesteal.Heal(player, target, 2, 3, 1f);
        }
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.Melee
{
    /// <summary>Lame jaune de la True Bloody Edge : traverse 2 ennemis, Ichor et Burning Blood 1 s.</summary>
    public class BloodyBlade : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 48;
            Projectile.aiStyle = ProjAIStyleID.Sickle;
            AIType = ProjectileID.DeathSickle;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 300;
        }

        public override void AI() =>
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.3f / 255f, (255 - Projectile.alpha) * 0.3f / 255f, 0f);

        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.timeLeft < 85)
            {
                byte b = (byte)(Projectile.timeLeft * 3);
                return new Color(b, b, b, (byte)(100f * b / 255f));
            }
            return new Color(255, 255, 255, 100);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 2);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.owner] = 9;
            target.AddBuff(ModContent.BuffType<BurningBlood>(), 60);
            target.AddBuff(BuffID.Ichor, 60);
        }
    }

    /// <summary>Rayon de la Terra Edge : Ichor 0,75 s, traverse 2 ennemis.</summary>
    public class TerraEdgeBeam : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.aiStyle = ProjAIStyleID.Beam;
            AIType = ProjectileID.TerraBeam;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 600;
        }

        public override void AI() => Lighting.AddLight(Projectile.Center, 0f, (255 - Projectile.alpha) * 1.3f / 255f, 0f);

        public override Color? GetAlpha(Color lightColor) => new Color(0, 200, 0, 0);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Ichor, 45);

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int i = 4; i < 31; i++)
            {
                Vector2 back = Projectile.oldVelocity * (30f / i);
                int d = Dust.NewDust(Projectile.oldPosition - back, 8, 8, DustID.TerraBlade, Projectile.oldVelocity.X, Projectile.oldVelocity.Y, 100, default, 1.8f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 0.5f;
                d = Dust.NewDust(Projectile.oldPosition - back, 8, 8, DustID.TerraBlade, Projectile.oldVelocity.X, Projectile.oldVelocity.Y, 100, default, 1.4f);
                Main.dust[d].velocity *= 0.05f;
            }
        }
    }
}

namespace CalamityFandom.Content.Legacy.Items.Weapons.Typeless
{
    /// <summary>
    /// Magnum et Lightning Hawk (pages) : armes sans classe qui tirent des Magnum Rounds (balle rapide, ne traverse pas,
    /// +1,33 % des PV max de la cible) ; une fois un boss apparu, seulement 3 tirs jusqu'à sa défaite ; coups critiques
    /// fixes, sans bonus des coverings ni des modificateurs. [technique] Le compteur de Calamity (timesUsed) et le son
    /// du Magnum n'existent plus : compteur refait ici, son du revolver du jeu.
    /// </summary>
    public abstract class FiniteUseGun : ModItem
    {
        public const int UsesPerBoss = 3;
        protected abstract int Crit { get; }
        public int TimesUsed;

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Default;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 8f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item41;
            Item.shootSpeed = 12f;
            Item.shoot = ModContent.ProjectileType<Projectiles.Typeless.MagnumRound>();
            Item.useAmmo = ModContent.ItemType<Ammo.MagnumRounds>();
        }

        public override void ModifyWeaponCrit(Player player, ref float crit) => crit = Crit;

        public override bool CanUseItem(Player player) => !CalamityPlayer.areThereAnyDamnBosses || TimesUsed < UsesPerBoss;

        public override void UpdateInventory(Player player)
        {
            if (!CalamityPlayer.areThereAnyDamnBosses)
                TimesUsed = 0;
        }

        public override bool Shoot(Player player, Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (CalamityPlayer.areThereAnyDamnBosses)
            {
                // Tous les exemplaires du même pistolet dans l'inventaire comptent (pas de contournement en changeant d'arme)
                foreach (Item item in player.inventory)
                    if (item.type == Type && item.ModItem is FiniteUseGun gun)
                        gun.TimesUsed++;
            }
            return true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5f, 0f);
    }

    /// <summary>Wiki : Magnum.txt</summary>
    public class Magnum : FiniteUseGun
    {
        protected override int Crit => 46;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 52;
            Item.height = 28;
            Item.damage = 80;
            Item.useTime = Item.useAnimation = 25;
            Item.value = Item.sellPrice(silver: 40);
            Item.rare = ItemRarityID.Green;
        }
    }

    /// <summary>Wiki : Lightning Hawk.txt</summary>
    public class LightningHawk : FiniteUseGun
    {
        protected override int Crit => 56;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 50;
            Item.height = 28;
            Item.damage = 400;
            Item.useTime = Item.useAnimation = 21;
            Item.value = Item.sellPrice(gold: 9, silver: 60);
            Item.rare = ItemRarityID.LightPurple;
        }
    }

    /// <summary>
    /// Wiki : Magnum Rounds.txt — munition du Magnum et de la Lightning Hawk (pile de 12), vendue par l'Arms Dealer quand
    /// le joueur a l'un des deux pistolets dans son inventaire.
    /// </summary>
    public class MagnumRoundsShop : GlobalNPC
    {
        public override void ModifyShop(NPCShop shop)
        {
            if (shop.NpcType != NPCID.ArmsDealer)
                return;
            var hasGun = new Condition("Mods.CalamityFandom.Conditions.HasMagnum",
                () => Main.LocalPlayer.HasItem(ModContent.ItemType<Magnum>()) || Main.LocalPlayer.HasItem(ModContent.ItemType<LightningHawk>()));
            shop.Add<Ammo.MagnumRounds>(hasGun);
        }
    }
}

namespace CalamityFandom.Content.Legacy.Items.Ammo
{
    /// <summary>Wiki : Magnum Rounds.txt</summary>
    public class MagnumRounds : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 20;

        public override void SetDefaults()
        {
            Item.width = Item.height = 18;
            Item.damage = 80;
            Item.knockBack = 8f;
            Item.maxStack = 12;
            Item.consumable = true;
            Item.value = Item.sellPrice(silver: 20);
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = ModContent.ProjectileType<Projectiles.Typeless.MagnumRound>();
            Item.shootSpeed = 12f;
            Item.ammo = Type;
        }
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.Typeless
{
    /// <summary>Balle des Magnum Rounds : comme une High Velocity Bullet, ne traverse pas, +1,33 % des PV max de la cible.</summary>
    public class MagnumRound : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.light = 0.5f;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 10;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            AIType = ProjectileID.BulletHighVelocity;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => modifiers.FlatBonusDamage += target.lifeMax / 75;
    }
}

namespace CalamityFandom.Content.Legacy.Items.Accessories
{
    /// <summary>
    /// Wiki : Plague Hive.txt — toutes les attaques infligent Plague et font naître des Plague Seekers (effet de
    /// l'Alchemical Decanter de Calamity, alchFlask) ; des abeilles sortent quand on est touché (Hive Pack : abeilles
    /// plus fortes) ; aura de Plague d'environ 40 blocs de diamètre. [choix] « 20 damage per tick » de l'aura : lu comme
    /// les dégâts de Plague (l'aura l'inflige toutes les demi-secondes) ; immunité à Plague du code de 2022 gardée.
    /// </summary>
    public class PlagueHive : ModItem
    {
        public const float AuraRadius = 20f * 16f;

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 48;
            Item.value = Item.sellPrice(gold: 16);
            Item.rare = ItemRarityID.Cyan;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().alchFlask = true;
            player.strongBees = true;
            player.honeyCombItem = Item;
            player.buffImmune[ModContent.BuffType<Plague>()] = true;
            if (player.whoAmI != Main.myPlayer || Main.GameUpdateCount % 30 != 0)
                return;
            foreach (var npc in Main.ActiveNPCs)
                if (!npc.friendly && !npc.dontTakeDamage && npc.Distance(player.Center) < AuraRadius)
                    npc.AddBuff(ModContent.BuffType<Plague>(), 60);
        }
    }
}
