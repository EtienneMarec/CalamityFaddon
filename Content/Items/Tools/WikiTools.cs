using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Items.Tools
{
    /// <summary>
    /// Outils du wiki qui débloquent une recette (31/10) : stats des infobox (pick / axe / hammer, « toolspeed » = useTime,
    /// « use » = useAnimation), sprites du wiki. La puissance de hache du wiki est celle affichée (le jeu la stocke /5).
    /// </summary>
    public abstract class WikiTool : ModItem
    {
        protected virtual int Pick => 0;
        protected virtual int AxeShown => 0;
        protected virtual int Hammer => 0;
        protected abstract int Damage { get; }
        protected abstract int Use { get; }
        protected abstract int ToolSpeed { get; }
        protected abstract float Knockback { get; }
        protected abstract int Rare { get; }
        protected abstract int Sell { get; }
        protected virtual int TileBoost => 0;
        /// <summary>Poussière émise en frappant (-1 : aucune).</summary>
        protected virtual int SwingDust => -1;
        /// <summary>Debuff au contact (-1 : aucun).</summary>
        protected virtual int ContactDebuff => -1;

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = Damage;
            Item.DamageType = DamageClass.Melee;
            Item.pick = Pick;
            Item.axe = AxeShown / 5;
            Item.hammer = Hammer;
            Item.useAnimation = Use;
            Item.useTime = ToolSpeed;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.knockBack = Knockback;
            Item.tileBoost = TileBoost;
            Item.UseSound = SoundID.Item1;
            Item.rare = Rare;
            Item.value = Sell * 5;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (SwingDust >= 0 && Main.rand.NextBool(3))
                Dust.NewDust(hitbox.TopLeft(), hitbox.Width, hitbox.Height, SwingDust);
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (ContactDebuff >= 0)
                target.AddBuff(ContactDebuff, 180);
        }
    }

    /// <summary>Wiki : Windslasher.txt</summary>
    public class Windslasher : WikiTool
    {
        protected override int AxeShown => 125;
        protected override int Damage => 20;
        protected override int Use => 25;
        protected override int ToolSpeed => 16;
        protected override float Knockback => 7f;
        protected override int Rare => ItemRarityID.Orange;
        protected override int Sell => Item.sellPrice(silver: 80);
    }

    /// <summary>Wiki : Gusthammer.txt</summary>
    public class Gusthammer : WikiTool
    {
        protected override int Hammer => 70;
        protected override int Damage => 20;
        protected override int Use => 25;
        protected override int ToolSpeed => 16;
        protected override float Knockback => 7f;
        protected override int Rare => ItemRarityID.Orange;
        protected override int Sell => Item.sellPrice(silver: 80);
    }

    /// <summary>Wiki : Lithos Pickaxe.txt — mine les Lihzahrd Bricks ; particules de feu, On Fire! au contact.</summary>
    public class LithosPickaxe : WikiTool
    {
        protected override int Pick => 210;
        protected override int Damage => 58;
        protected override int Use => 14;
        protected override int ToolSpeed => 6;
        protected override float Knockback => 3.5f;
        protected override int Rare => ItemRarityID.Yellow;
        protected override int Sell => Item.sellPrice(gold: 16);
        protected override int SwingDust => DustID.Torch;
        protected override int ContactDebuff => BuffID.OnFire;
    }

    /// <summary>Wiki : Impact Adjuster.txt — particules de feu, On Fire! au contact.</summary>
    public class ImpactAdjuster : WikiTool
    {
        protected override int Hammer => 95;
        protected override int Damage => 58;
        protected override int Use => 14;
        protected override int ToolSpeed => 6;
        protected override float Knockback => 3.5f;
        protected override int Rare => ItemRarityID.Yellow;
        protected override int Sell => Item.sellPrice(gold: 16);
        protected override int SwingDust => DustID.Torch;
        protected override int ContactDebuff => BuffID.OnFire;
    }

    /// <summary>Wiki : Crayfish Pickaxe.txt — malgré son nom, hache et marteau (infobox).</summary>
    public class CrayfishPickaxe : WikiTool
    {
        protected override int AxeShown => 55;
        protected override int Hammer => 60;
        protected override int Damage => 15;
        protected override int Use => 28;
        protected override int ToolSpeed => 11;
        protected override float Knockback => 6f;
        protected override int Rare => ItemRarityID.Green;
        protected override int Sell => Item.sellPrice(silver: 40);
    }

    /// <summary>Wiki : Carapace Hammer.txt</summary>
    public class CarapaceHammer : WikiTool
    {
        protected override int Hammer => 60;
        protected override int Damage => 10;
        protected override int Use => 28;
        protected override int ToolSpeed => 11;
        protected override float Knockback => 6f;
        protected override int Rare => ItemRarityID.Green;
        protected override int Sell => Item.sellPrice(silver: 40);
    }

    /// <summary>Wiki : Astral Hammer.txt — un bloc de portée de moins que les Lunar Hamaxes ; particules violettes.</summary>
    public class AstralHammer : WikiTool
    {
        protected override int Hammer => 100;
        protected override int Damage => 70;
        protected override int Use => 19;
        protected override int ToolSpeed => 5;
        protected override float Knockback => 5f;
        protected override int Rare => ItemRarityID.Cyan;
        protected override int Sell => Item.sellPrice(gold: 19);
        protected override int TileBoost => 3;
        protected override int SwingDust => DustID.PurpleTorch;
    }

    /// <summary>Wiki : Astral Axe.txt — particules violettes.</summary>
    public class AstralAxe : WikiTool
    {
        protected override int AxeShown => 165;
        protected override int Damage => 60;
        protected override int Use => 19;
        protected override int ToolSpeed => 5;
        protected override float Knockback => 5f;
        protected override int Rare => ItemRarityID.Cyan;
        protected override int Sell => Item.sellPrice(gold: 19);
        protected override int SwingDust => DustID.PurpleTorch;
    }
}
