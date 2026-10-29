// Adapté de Calamity Mod (objets retirés en 2022), © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using CalamityFandom.Content.Legacy.Buffs.StatDebuffs;
using CalamityMod;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

// Objets retirés de Calamity avant 2.0.1.005 (nuit du 29/10) : valeurs du wiki, recettes et butin par les systèmes du wiki.

namespace CalamityFandom.Content.Legacy.Items.SummonItems
{
    /// <summary>Wiki : Blood Relic.txt — la nuit, déclenche aussitôt une Blood Moon ; non consommable (infobulle).</summary>
    public class BloodRelic : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = Item.height = 20;
            Item.rare = ItemRarityID.Pink;
            Item.useAnimation = Item.useTime = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Roar;
        }

        public override bool CanUseItem(Player player) => !Main.bloodMoon && !Main.dayTime && !Main.pumpkinMoon && !Main.snowMoon;

        public override bool? UseItem(Player player)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return true;
            Main.bloodMoon = true;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);
            return true;
        }
    }
}

namespace CalamityFandom.Content.Legacy.Items.Tools
{
    /// <summary>Wiki : Wulfrum Axe.txt — hache de Wulfrum (60 %).</summary>
    public class WulfrumAxe : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 5;
            Item.knockBack = 5f;
            Item.useTime = 15;
            Item.useAnimation = 25;
            Item.axe = 60 / 5;
            Item.tileBoost += 1;
            Item.DamageType = DamageClass.Melee;
            Item.width = Item.height = 32;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.value = Item.sellPrice(silver: 20);
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = true;
        }
    }

    /// <summary>Wiki : Wulfrum Hammer.txt — marteau de Wulfrum (45 %).</summary>
    public class WulfrumHammer : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 7;
            Item.knockBack = 5.5f;
            Item.useTime = 11;
            Item.useAnimation = 28;
            Item.hammer = 45;
            Item.tileBoost += 1;
            Item.DamageType = DamageClass.Melee;
            Item.width = Item.height = 32;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.value = Item.sellPrice(silver: 20);
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = true;
        }
    }

    /// <summary>Wiki : Waraxe.txt — hache (85 %) ; un coup critique inflige War Cleave 15 s.</summary>
    public class Waraxe : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 26;
            Item.knockBack = 5.25f;
            Item.useTime = 18;
            Item.useAnimation = 21;
            Item.axe = 85 / 5;
            Item.DamageType = DamageClass.Melee;
            Item.width = Item.height = 32;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.value = Item.sellPrice(silver: 20);
            Item.rare = ItemRarityID.White;
            Item.autoReuse = true;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hit.Crit)
                target.AddBuff(ModContent.BuffType<WarCleave>(), 900);
        }
    }
}

namespace CalamityFandom.Content.Legacy.Items.Weapons.Melee
{
    /// <summary>Wiki : Warblade.txt — un coup critique inflige War Cleave 7,5 s.</summary>
    public class Warblade : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 27;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 18;
            Item.width = Item.height = 32;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.knockBack = 4.25f;
            Item.value = Item.sellPrice(silver: 20);
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = true;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hit.Crit)
                target.AddBuff(ModContent.BuffType<WarCleave>(), 450);
        }
    }

    /// <summary>Wiki : Corrupted Crusher Blade.txt — Cursed Inferno 2 s ; un coup critique retire 10 de défense à l'ennemi.</summary>
    public class CorruptedCrusherBlade : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 42;
            Item.DamageType = DamageClass.Melee;
            Item.scale = 1.25f;
            Item.useTime = Item.useAnimation = 25;
            Item.width = Item.height = 48;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.knockBack = 6.75f;
            Item.value = Item.sellPrice(silver: 40);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(7))
                Dust.NewDust(hitbox.TopLeft(), hitbox.Width, hitbox.Height, DustID.Demonite);
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.CursedInferno, 120);
            if (hit.Crit)
                target.Calamity().miscDefenseLoss = 10;
        }
    }

    /// <summary>Wiki : Crimson Crusher Blade.txt — Ichor 2 s ; un coup critique baisse de 5 les dégâts de contact (hors boss).</summary>
    public class CrimsonCrusherBlade : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 40;
            Item.DamageType = DamageClass.Melee;
            Item.scale = 1.25f;
            Item.useTime = Item.useAnimation = 27;
            Item.width = Item.height = 48;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.knockBack = 7f;
            Item.value = Item.sellPrice(silver: 40);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(7))
                Dust.NewDust(hitbox.TopLeft(), hitbox.Width, hitbox.Height, DustID.Blood);
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Ichor, 120);
            if (target.damage > 0 && hit.Crit && !CalamityPlayer.areThereAnyDamnBosses)
                target.damage = System.Math.Max(1, target.defDamage - 5);
        }
    }

    /// <summary>Wiki : Mandible Claws.txt — griffes très rapides et de courte portée.</summary>
    public class MandibleClaws : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 13;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 6;
            Item.width = Item.height = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.useTurn = true;
            Item.knockBack = 3.5f;
            Item.autoReuse = true;
            Item.value = Item.sellPrice(silver: 20);
            Item.rare = ItemRarityID.Blue;
        }
    }
}

namespace CalamityFandom.Content.Legacy.Items.Weapons.Ranged
{
    /// <summary>Wiki : Mandible Bow.txt — arc simple (flèches, vitesse 15).</summary>
    public class MandibleBow : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 14;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 25;
            Item.width = 20;
            Item.height = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item5;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = Item.sellPrice(silver: 20);
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 15f;
            Item.useAmmo = AmmoID.Arrow;
        }
    }
}
