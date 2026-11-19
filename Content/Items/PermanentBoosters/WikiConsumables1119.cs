using CalamityMod;
using CalamityMod.CalPlayer;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

// Lot AC (19/11) : bonus permanents de Rage et d'Adrénaline, potions du wiki (recettes du wiki).

namespace CalamityFandom.Content.Items.PermanentBoosters
{
    /// <summary>
    /// Wiki : JOOZ.txt — Adrénaline qui se charge 10 % plus vite ; en combat de boss, les coups ne font perdre que la moitié
    /// de l'Adrénaline qu'ils retireraient.
    /// </summary>
    public class JOOZ : WikiBooster
    {
        protected override int Rarity => ItemRarityID.Green;
        protected override int SellCopper => 300000;
    }

    /// <summary>Wiki : SAUS.txt — +10 % de dégâts, pour toujours.</summary>
    public class SAUS : WikiBooster
    {
        protected override int Rarity => ItemRarityID.Red;
        protected override int SellCopper => 1000000;
    }

    /// <summary>Wiki : Potato Cat.txt — +45 % de dégâts du mode Rage, pour toujours (rareté « a » → 0).</summary>
    public class PotatoCat : WikiBooster
    {
        protected override int Rarity => ItemRarityID.White;
        protected override int SellCopper => 300000;
    }

    /// <summary>
    /// Wiki : YashBrowns.txt — Rage +2 s, dégâts du mode Adrénaline +25 %, réduction des dégâts de l'Adrénaline pleine +7 %.
    /// [technique] « +250 d'endurance (stamina) » : Calamity n'a pas d'endurance, non fait.
    /// </summary>
    public class YashBrowns : WikiBooster
    {
        protected override int Rarity => ItemRarityID.Yellow;
        protected override int SellCopper => 300000;
    }

    /// <summary>Wiki : Adrenaline of the Gods.txt — dégâts du mode Adrénaline +45 %, réduction des dégâts de l'Adrénaline pleine +15 %.</summary>
    public class AdrenalineoftheGods : WikiBooster
    {
        protected override int Rarity => ModContent.RarityType<BurnishedAuric>();
        protected override int SellCopper => 0;
    }

    /// <summary>Wiki : Rage of the Cosmos.txt — mode Rage +3 s.</summary>
    public class RageoftheCosmos : WikiBooster
    {
        protected override int Rarity => ModContent.RarityType<CosmicPurple>();
        protected override int SellCopper => 0;
    }

    /// <summary>Effets des bonus de Rage et d'Adrénaline du wiki, ajoutés à ceux de Calamity.</summary>
    public class RipperBoosterPlayer : ModPlayer
    {
        private float lastAdrenaline, adrenalineBeforeHit;

        private bool Has(string name) => Player.GetModPlayer<WikiBoosterPlayer>().Used.Contains(name);

        private float ExtraAdrenalineDamage => (Has(nameof(YashBrowns)) ? 0.25f : 0f) + (Has(nameof(AdrenalineoftheGods)) ? 0.45f : 0f);
        private float ExtraAdrenalineDR => (Has(nameof(YashBrowns)) ? 0.07f : 0f) + (Has(nameof(AdrenalineoftheGods)) ? 0.15f : 0f);

        public override void PostUpdateEquips()
        {
            var cal = Player.Calamity();
            if (Has(nameof(SAUS)))
                Player.GetDamage<GenericDamageClass>() += 0.1f;
            // Remis à zéro par Calamity à chaque image (ResetEffects), lus après (UpdateRippers)
            if (Has(nameof(PotatoCat)))
                cal.RageDamageBoost += 0.45f;
            if (Has(nameof(YashBrowns)))
                cal.RageDuration += 120;
            if (Has(nameof(RageoftheCosmos)))
                cal.RageDuration += 180;
        }

        public override void PostUpdateMiscEffects()
        {
            var cal = Player.Calamity();
            // JOOZ : 10 % de charge en plus (Calamity charge l'Adrénaline juste avant, dans le même crochet)
            if (Has(nameof(JOOZ)) && !cal.adrenalineModeActive && cal.adrenaline > lastAdrenaline)
                cal.adrenaline = System.Math.Min(cal.adrenalineMax, cal.adrenaline + (cal.adrenaline - lastAdrenaline) * 0.1f);
            lastAdrenaline = cal.adrenaline;
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            var cal = Player.Calamity();
            adrenalineBeforeHit = cal.adrenaline;
            // Même condition que la réduction des dégâts de l'Adrénaline pleine de Calamity
            float extra = ExtraAdrenalineDR;
            if (extra > 0f && cal.AdrenalineEnabled && !cal.draedonsHeart && cal.adrenaline == cal.adrenalineMax && !cal.adrenalineModeActive
                && cal.TotalEnergyShielding <= 0)
            {
                float dr = cal.GetAdrenalineDR();
                modifiers.IncomingDamageMultiplier *= System.Math.Max(0.05f, 1f - dr - extra) / System.Math.Max(0.05f, 1f - dr);
            }
        }

        public override void PostHurt(Player.HurtInfo info)
        {
            var cal = Player.Calamity();
            if (Has(nameof(JOOZ)) && CalamityPlayer.areThereAnyDamnBosses && cal.adrenaline < adrenalineBeforeHit)
                cal.adrenaline += (adrenalineBeforeHit - cal.adrenaline) / 2f;
            lastAdrenaline = cal.adrenaline;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            var cal = Player.Calamity();
            float extra = ExtraAdrenalineDamage;
            if (extra > 0f && cal.adrenalineModeActive && !cal.draedonsHeart)
                modifiers.SourceDamage *= 1f + extra / (1f + cal.GetAdrenalineDamage());
        }
    }
}

namespace CalamityFandom.Content.Items.Potions
{
    /// <summary>Wiki : Active Healing Potion.txt — soigne 600 PV ; Potion Sickness 40 s.</summary>
    public class ActiveHealingPotion : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 30;

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 32;
            Item.useTime = Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.UseSound = SoundID.Item3;
            Item.useTurn = true;
            Item.consumable = true;
            Item.maxStack = 9999;
            Item.potion = true;
            Item.healLife = 600;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
            Item.value = Item.sellPrice(gold: 20);
        }

        public override bool? UseItem(Player player)
        {
            player.GetModPlayer<ShortSicknessPlayer>().Pending = 40 * 60;
            return null;
        }
    }

    /// <summary>Raccourcit la Potion Sickness d'une potion du wiki (appliquée par Terraria après l'utilisation).</summary>
    public class ShortSicknessPlayer : ModPlayer
    {
        public int Pending;

        public override void PostUpdate()
        {
            if (Pending <= 0 || Player.potionDelay <= 0)
                return;
            if (Player.potionDelay > Pending)
            {
                Player.potionDelay = Pending;
                int i = Player.FindBuffIndex(BuffID.PotionSickness);
                if (i >= 0)
                    Player.buffTime[i] = Pending;
            }
            Pending = 0;
        }
    }

    /// <summary>Wiki : Cobalt Skin Potion.txt — Cobalt Skin 8 min : +14 défense.</summary>
    public class CobaltSkinPotion : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 20;

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 30;
            Item.useTime = Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.UseSound = SoundID.Item3;
            Item.useTurn = true;
            Item.consumable = true;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(silver: 20);
            Item.buffType = ModContent.BuffType<Buffs.CobaltSkin>();
            Item.buffTime = 8 * 60 * 60;
        }
    }

    /// <summary>
    /// Wiki : Sugars of Calamity.txt — Calamitous Euphoria 10 min (+30 % de vitesse, +50 % de dégâts, -30 % de critique,
    /// -10 défense) ; à la fin : Calamitous Dysphoria 45 min (-10 % de vitesse, -20 % de dégâts, -20 % de critique, -40 % de
    /// vitesse de minage, -10 défense). Plus de 2 prises d'affilée (pendant l'Euphoria ou la Dysphoria) : Calamitous Anxiety
    /// 20 min (+15 % de vitesse, +5 défense, -50 % de dégâts, -90 % de critique). En reprendre retire la Dysphoria, qui revient
    /// entière à la fin de l'Euphoria ou de l'Anxiety.
    /// [choix] Critique en points ; l'Anxiety remplace l'Euphoria de la prise qui la déclenche.
    /// </summary>
    public class SugarsofCalamity : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 20;

        public override void SetDefaults()
        {
            Item.width = Item.height = 32;
            Item.useTime = Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.UseSound = SoundID.Item2;
            Item.useTurn = true;
            Item.consumable = true;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(gold: 50);
        }

        public override bool? UseItem(Player player)
        {
            var mp = player.GetModPlayer<SugarsPlayer>();
            int euphoria = ModContent.BuffType<Buffs.CalamitousEuphoria>(), dysphoria = ModContent.BuffType<Buffs.CalamitousDysphoria>();
            mp.Streak = player.HasBuff(euphoria) || player.HasBuff(dysphoria) ? mp.Streak + 1 : 1;
            player.ClearBuff(dysphoria);
            if (mp.Streak > 2)
            {
                player.ClearBuff(euphoria);
                player.AddBuff(ModContent.BuffType<Buffs.CalamitousAnxiety>(), 20 * 60 * 60);
            }
            else
                player.AddBuff(euphoria, 10 * 60 * 60);
            return true;
        }
    }

    public class SugarsPlayer : ModPlayer
    {
        public int Streak;

        public override void PostUpdateBuffs()
        {
            if (!Player.HasBuff(ModContent.BuffType<Buffs.CalamitousEuphoria>()) && !Player.HasBuff(ModContent.BuffType<Buffs.CalamitousDysphoria>())
                && !Player.HasBuff(ModContent.BuffType<Buffs.CalamitousAnxiety>()))
                Streak = 0;
        }
    }
}

namespace CalamityFandom.Content.Buffs
{
    public class CobaltSkin : ModBuff
    {
        public override string Texture => "CalamityFandom/Content/Buffs/Wiki/CobaltSkin";

        public override void Update(Player player, ref int buffIndex) => player.statDefense += 14;
    }

    /// <summary>Euphoria et Anxiety donnent la Dysphoria quand elles se terminent.</summary>
    public abstract class SugarsBuff : ModBuff
    {
        public override string Texture => "CalamityFandom/Content/Buffs/Wiki/" + Name;

        protected abstract void Apply(Player player);

        protected virtual bool EndsInDysphoria => true;

        public override void Update(Player player, ref int buffIndex)
        {
            Apply(player);
            if (EndsInDysphoria && player.buffTime[buffIndex] <= 1 && !player.dead)
                player.AddBuff(ModContent.BuffType<CalamitousDysphoria>(), 45 * 60 * 60);
        }
    }

    public class CalamitousEuphoria : SugarsBuff
    {
        protected override void Apply(Player player)
        {
            player.moveSpeed += 0.3f;
            player.GetDamage<GenericDamageClass>() += 0.5f;
            player.GetCritChance<GenericDamageClass>() -= 30;
            player.statDefense -= 10;
        }
    }

    public class CalamitousDysphoria : SugarsBuff
    {
        protected override bool EndsInDysphoria => false;

        public override void SetStaticDefaults() => Main.debuff[Type] = true;

        protected override void Apply(Player player)
        {
            player.moveSpeed -= 0.1f;
            player.GetDamage<GenericDamageClass>() -= 0.2f;
            player.GetCritChance<GenericDamageClass>() -= 20;
            player.pickSpeed += 0.4f;
            player.statDefense -= 10;
        }
    }

    public class CalamitousAnxiety : SugarsBuff
    {
        public override void SetStaticDefaults() => Main.debuff[Type] = true;

        protected override void Apply(Player player)
        {
            player.moveSpeed += 0.15f;
            player.statDefense += 5;
            player.GetDamage<GenericDamageClass>() -= 0.5f;
            player.GetCritChance<GenericDamageClass>() -= 90;
        }
    }
}
