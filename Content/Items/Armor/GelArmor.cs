using CalamityMod;
using CalamityMod.ExtraJumps;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Items.Armor
{
    /// <summary>
    /// Pièce de la Gel armor (wiki : Gel Armor.txt) : ensemble d'avant Hardmode en Unpurified Gel, amélioré en Statigel.
    /// [technique] Le wiki n'a que les icônes : le rendu porté reprend les sprites Statigel de Calamity (Helm = mêlée,
    /// Headgear = distance, Cap = magie, Hood = invocation, Mask = spy).
    /// </summary>
    public abstract class GelArmorPiece : ModItem
    {
        protected abstract EquipType Slot { get; }
        protected abstract string StatigelTexture { get; }
        protected abstract int Defense { get; }
        protected abstract int SellCopper { get; }

        public override void Load()
        {
            if (Main.dedServ)
                return;
            string suffix = Slot switch { EquipType.Head => "_Head", EquipType.Body => "_Body", _ => "_Legs" };
            EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Armor/Statigel/" + StatigelTexture + suffix, Slot, this);
        }

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            if (Main.dedServ || Slot != EquipType.Body)
                return;
            int slot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
            ArmorIDs.Body.Sets.HidesTopSkin[slot] = true;
            ArmorIDs.Body.Sets.HidesArms[slot] = true;
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 18;
            Item.rare = ItemRarityID.LightRed;
            Item.value = SellCopper * 5;
            Item.defense = Defense;
            if (Main.dedServ)
                return;
            int slot = EquipLoader.GetEquipSlot(Mod, Name, Slot);
            switch (Slot)
            {
                case EquipType.Head: Item.headSlot = slot; break;
                case EquipType.Body: Item.bodySlot = slot; break;
                default: Item.legSlot = slot; break;
            }
        }
    }

    /// <summary>Casque de la Gel armor : bonus d'ensemble commun (saut, immunité après un gros coup) et le sien.</summary>
    public abstract class GelHead : GelArmorPiece
    {
        protected override EquipType Slot => EquipType.Head;
        protected override int SellCopper => 10000;

        public override bool IsArmorSet(Item head, Item body, Item legs) =>
            body.type == ModContent.ItemType<GelArmor>() && legs.type == ModContent.ItemType<GelGreaves>();

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = Language.GetTextValue("Mods.CalamityFandom.GelArmor.SetBonus") + SetBonusText();
            player.GetModPlayer<GelArmorPlayer>().SetBonus = true;
            // Saut plus haut (+1/3, comme la Statigel : environ 3 blocs), +6 % de vitesse de saut (0,3 × 20 dans
            // l'échelle de Calamity), saut supplémentaire en gel (celui de la Statigel)
            Player.jumpHeight += 5;
            player.jumpSpeedBoost += 0.3f;
            player.GetJumpState<StatigelJump>().Enable();
            SetBonus(player);
        }

        protected virtual string SetBonusText() => "";

        protected virtual void SetBonus(Player player)
        {
        }
    }

    /// <summary>Wiki : Gel Armor.txt — 7 défense, +3 % de chance de critique.</summary>
    public class GelArmor : GelArmorPiece
    {
        protected override EquipType Slot => EquipType.Body;
        protected override string StatigelTexture => "StatigelArmor";
        protected override int Defense => 7;
        protected override int SellCopper => 8000;

        public override void UpdateEquip(Player player) => player.GetCritChance<GenericDamageClass>() += 3;
    }

    /// <summary>Gel Greaves (page Gel Armor) : 5 défense, +3 % de dégâts, +5 % de vitesse de déplacement.</summary>
    public class GelGreaves : GelArmorPiece
    {
        protected override EquipType Slot => EquipType.Legs;
        protected override string StatigelTexture => "StatigelGreaves";
        protected override int Defense => 5;
        protected override int SellCopper => 6000;

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.03f;
            player.moveSpeed += 0.05f;
        }
    }

    /// <summary>Gel Helm : 4 défense, +5 % de dégâts et de vitesse de mêlée, +3 % de critique ; ensemble : +200 d'aggro.</summary>
    public class GelHelm : GelHead
    {
        protected override string StatigelTexture => "StatigelHeadMelee";
        protected override int Defense => 4;

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<MeleeDamageClass>() += 0.05f;
            player.GetAttackSpeed<MeleeDamageClass>() += 0.05f;
            player.GetCritChance<MeleeDamageClass>() += 3;
        }

        protected override string SetBonusText() => "\n" + Language.GetTextValue("Mods.CalamityFandom.GelArmor.Helm");

        protected override void SetBonus(Player player) => player.aggro += 200;
    }

    /// <summary>Gel Headgear : 3 défense, +5 % de dégâts et +3 % de critique à distance.</summary>
    public class GelHeadgear : GelHead
    {
        protected override string StatigelTexture => "StatigelHeadRanged";
        protected override int Defense => 3;

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<RangedDamageClass>() += 0.05f;
            player.GetCritChance<RangedDamageClass>() += 3;
        }
    }

    /// <summary>Gel Cap : 1 défense, +5 % de dégâts magiques, -5 % de mana, +3 % de critique, +30 mana max.</summary>
    public class GelCap : GelHead
    {
        protected override string StatigelTexture => "StatigelHeadMagic";
        protected override int Defense => 1;

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<MagicDamageClass>() += 0.05f;
            player.manaCost -= 0.05f;
            player.GetCritChance<MagicDamageClass>() += 3;
            player.statManaMax2 += 30;
        }
    }

    /// <summary>
    /// Gel Hood : 1 défense, recul des sbires augmenté ; ensemble : +6 % de dégâts des sbires, +1 sbire, un Baby Slime.
    /// [choix] Recul des sbires +50 % (la page ne donne pas de chiffre) ; Baby Slime vanilla à 13 dégâts (page).
    /// </summary>
    public class GelHood : GelHead
    {
        protected override string StatigelTexture => "StatigelHeadSummon";
        protected override int Defense => 1;

        public override void UpdateEquip(Player player) => player.GetKnockback<SummonDamageClass>() += 0.5f;

        protected override string SetBonusText() => "\n" + Language.GetTextValue("Mods.CalamityFandom.GelArmor.Hood");

        protected override void SetBonus(Player player)
        {
            player.GetDamage<SummonDamageClass>() += 0.06f;
            player.maxMinions += 1;
            if (player.whoAmI != Main.myPlayer)
                return;
            if (!player.HasBuff(BuffID.BabySlime))
                player.AddBuff(BuffID.BabySlime, 3600, true);
            if (player.ownedProjectileCounts[ProjectileID.BabySlime] < 1)
            {
                int damage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(13);
                int p = Projectile.NewProjectile(player.GetSource_FromThis("GelHood"), player.Center, -Microsoft.Xna.Framework.Vector2.UnitY, ProjectileID.BabySlime, damage, 0f, player.whoAmI);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].originalDamage = 13;
            }
        }
    }

    /// <summary>
    /// Gel Mask : 2 défense, +5 % de dégâts et +3 % de critique de spy, +3 % de vitesse, 15 % de chance de ne pas consommer
    /// les objets lancés ; ensemble : furtivité de spy jusqu'à 50.
    /// </summary>
    public class GelMask : GelHead
    {
        protected override string StatigelTexture => "StatigelHeadRogue";
        protected override int Defense => 2;

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<RogueDamageClass>() += 0.05f;
            player.GetCritChance<RogueDamageClass>() += 3;
            player.moveSpeed += 0.03f;
            player.GetModPlayer<GelArmorPlayer>().ThrowSave = true;
        }

        protected override string SetBonusText() => "\n" + Language.GetTextValue("Mods.CalamityFandom.GelArmor.Mask");

        protected override void SetBonus(Player player)
        {
            var cal = player.Calamity();
            cal.rogueStealthMax += 0.5f;
            cal.wearingRogueArmor = true;
        }
    }

    /// <summary>Effets de la Gel armor : 30 images d'invulnérabilité de plus après un coup de plus de 100 dégâts ; objets lancés économisés.</summary>
    public class GelArmorPlayer : ModPlayer
    {
        public bool SetBonus, ThrowSave;

        public override void ResetEffects() => SetBonus = ThrowSave = false;

        public override void PostHurt(Player.HurtInfo info)
        {
            if (SetBonus && info.Damage > 100)
                Player.immuneTime += 30;
        }
    }

    public class GelMaskThrowSave : GlobalItem
    {
        public override bool ConsumeItem(Item item, Player player) =>
            !(player.GetModPlayer<GelArmorPlayer>().ThrowSave && item.consumable && item.CountsAsClass<RogueDamageClass>() && Main.rand.NextFloat() < 0.15f);
    }
}
