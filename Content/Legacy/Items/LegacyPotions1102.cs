// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using CalamityFandom.Content.Legacy.Buffs.Potions;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

// Potions retirées de Calamity après 2.0.1.005 et documentées sur le wiki (02/11). Effets du vieux CalamityPlayer (les
// buffs n'y mettaient qu'un drapeau), valeurs des infobox (rareté, durée, prix) ; recettes du wiki (WikiRecipes).
// La Tesla Potion (aura électrique, debuff Galvanic Corrosion) n'est pas encore portée.

namespace CalamityFandom.Content.Legacy.Items.Potions
{
    public abstract class LegacyBuffPotion : ModItem
    {
        protected abstract int Buff { get; }
        protected abstract int Seconds { get; }
        protected abstract int Rarity { get; }
        protected abstract (int width, int height) Size { get; }

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 20;

        public override void SetDefaults()
        {
            (Item.width, Item.height) = Size;
            Item.useTurn = true;
            Item.maxStack = 9999;
            Item.rare = Rarity;
            Item.useTime = Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.UseSound = SoundID.Item3;
            Item.consumable = true;
            Item.buffType = Buff;
            Item.buffTime = Seconds * 60;
            Item.value = Item.sellPrice(silver: 40);
        }
    }

    /// <summary>Wiki : Holy Wrath Potion.txt — +12 % de dégâts, Holy Flames 3 s à chaque coup ; retire le buff Wrath.</summary>
    public class HolyWrathPotion : LegacyBuffPotion
    {
        protected override int Buff => ModContent.BuffType<HolyWrathBuff>();
        protected override int Seconds => 300;
        protected override int Rarity => ItemRarityID.Purple;
        protected override (int, int) Size => (44, 36);
    }

    /// <summary>
    /// Wiki : Penumbra Potion.txt — discrétion Spy +15 % en bougeant ; la nuit, +15 % aussi à l'arrêt ; pendant une éclipse,
    /// +20 % dans les deux cas (valeurs de la page ; le vieux code donnait +10 % en bougeant, +15 % à l'arrêt).
    /// </summary>
    public class PenumbraPotion : LegacyBuffPotion
    {
        protected override int Buff => ModContent.BuffType<PenumbraBuff>();
        protected override int Seconds => 480;
        protected override int Rarity => ItemRarityID.Lime;
        protected override (int, int) Size => (26, 30);
    }

    /// <summary>
    /// Wiki : Profaned Rage Potion.txt — +12 % de critique, +5 % de vitesse de vol horizontale ; retire le buff Rage.
    /// [choix] Vitesse de vol : +5 % de vitesse de déplacement pendant le vol (le vieux code n'en donnait pas).
    /// </summary>
    public class ProfanedRagePotion : LegacyBuffPotion
    {
        protected override int Buff => ModContent.BuffType<ProfanedRageBuff>();
        protected override int Seconds => 300;
        protected override int Rarity => ItemRarityID.Purple;
        protected override (int, int) Size => (34, 42);
    }

    /// <summary>
    /// Wiki : Titan Scale Potion.txt — recul augmenté, +5 défense, +5 % de réduction des dégâts ; pendant 10 s après un coup
    /// de vrai corps-à-corps : +25 défense et +10 % au total (10 s : vieux code).
    /// </summary>
    public class TitanScalePotion : LegacyBuffPotion
    {
        protected override int Buff => ModContent.BuffType<TitanScale>();
        protected override int Seconds => 480;
        protected override int Rarity => ItemRarityID.Yellow;
        protected override (int, int) Size => (24, 34);
    }

    /// <summary>
    /// Wiki : Triumph Potion.txt — dégâts de contact des ennemis réduits de 15 % × la part de vie qui leur manque (réduction
    /// de contact de Calamity).
    /// </summary>
    public class TriumphPotion : LegacyBuffPotion
    {
        protected override int Buff => ModContent.BuffType<TriumphBuff>();
        protected override int Seconds => 240;
        protected override int Rarity => ItemRarityID.Green;
        protected override (int, int) Size => (28, 18);
    }
}

namespace CalamityFandom.Content.Legacy.Buffs.Potions
{
    public abstract class LegacyPotionBuff : ModBuff
    {
        public override void SetStaticDefaults() => Main.pvpBuff[Type] = true;
    }

    public class HolyWrathBuff : LegacyPotionBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.buffImmune[BuffID.Wrath] = true;
            player.GetDamage<GenericDamageClass>() += 0.12f;
            player.GetModPlayer<PotionEffectsPlayer1102>().HolyWrath = true;
        }
    }

    public class PenumbraBuff : LegacyPotionBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            var cal = player.Calamity();
            float boost = Main.eclipse ? 0.2f : 0.15f;
            cal.stealthGenMoving += boost;
            if (!Main.dayTime || Main.eclipse)
                cal.stealthGenStandstill += boost;
        }
    }

    public class ProfanedRageBuff : LegacyPotionBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.buffImmune[BuffID.Rage] = true;
            player.GetCritChance<GenericDamageClass>() += 12;
            if (player.wingTime > 0f && player.velocity.Y != 0f)
                player.moveSpeed += 0.05f;
        }
    }

    public class TitanScale : LegacyPotionBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.endurance += 0.05f;
            player.statDefense += 5;
            player.kbBuff = true;
            if (player.GetModPlayer<PotionEffectsPlayer1102>().TitanBoost > 0)
            {
                player.statDefense += 20;
                player.endurance += 0.05f;
            }
        }
    }

    public class TriumphBuff : LegacyPotionBuff
    {
        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<PotionEffectsPlayer1102>().Triumph = true;
    }

    /// <summary>Effets des potions portées qui dépendent des coups donnés ou reçus.</summary>
    public class PotionEffectsPlayer1102 : ModPlayer
    {
        public bool HolyWrath, Triumph;
        public int TitanBoost;

        public override void ResetEffects()
        {
            HolyWrath = Triumph = false;
            if (TitanBoost > 0)
                TitanBoost--;
            if (!Player.HasBuff<TitanScale>())
                TitanBoost = 0;
        }

        private void OnAnyHit(NPC target, bool trueMelee)
        {
            if (HolyWrath)
                target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
            if (trueMelee && Player.HasBuff<TitanScale>())
                TitanBoost = 600;
        }

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone) =>
            OnAnyHit(target, item.CountsAsClass(DamageClass.Melee));

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone) =>
            OnAnyHit(target, proj.CountsAsClass<TrueMeleeDamageClass>() || proj.CountsAsClass<TrueMeleeNoSpeedDamageClass>());

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            // Triumph : 15 % × la vie manquante de l'ennemi
            if (Triumph && npc.lifeMax > 0)
                modifiers.FinalDamage *= 1f - 0.15f * (1f - npc.life / (float)npc.lifeMax);
        }
    }
}
