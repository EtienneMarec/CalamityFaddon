using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Items.Accessories
{
    /// <summary>
    /// Wiki : Surgical Glove.txt — covering inventé par le wiki, lâché par le Plaguebringer Goliath (WikiLoot) :
    /// +12 % de vitesse et de dégâts au corps à corps, +3 % de critique, +8 % de dégâts true melee (ne se cumule pas avec
    /// la Titan Glove et ses améliorations, Notes), immunité à la Plague ; les attaques de mêlée et leurs projectiles
    /// infligent la Plague (25 % : 6 s, 37,5 % : 4 s, 37,5 % : 2 s, Notes). Rareté 8, vente 12 or.
    /// </summary>
    public class SurgicalGlove : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 34;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 12);
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetAttackSpeed(DamageClass.Melee) += 0.12f;
            player.GetDamage(DamageClass.Melee) += 0.12f;
            player.GetCritChance(DamageClass.Melee) += 3;
            // [choix] « Titan Glove et ses améliorations » = l'effet de recul de la Titan Glove (kbGlove), que toutes
            // ses améliorations donnent
            if (!player.kbGlove)
                player.GetDamage(ModContent.GetInstance<TrueMeleeDamageClass>()) += 0.08f;
            player.buffImmune[ModContent.BuffType<Plague>()] = true;
            player.GetModPlayer<SurgicalGlovePlayer>().Equipped = true;
        }
    }

    public class SurgicalGlovePlayer : ModPlayer
    {
        public bool Equipped;

        public override void ResetEffects() => Equipped = false;

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone) => Infect(item.DamageType, target);

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone) => Infect(proj.DamageType, target);

        private void Infect(DamageClass type, NPC target)
        {
            if (!Equipped || !type.CountsAsClass(DamageClass.Melee))
                return;
            float roll = Main.rand.NextFloat();
            int seconds = roll < 0.25f ? 6 : roll < 0.625f ? 4 : 2;
            target.AddBuff(ModContent.BuffType<Plague>(), seconds * 60);
        }
    }
}
