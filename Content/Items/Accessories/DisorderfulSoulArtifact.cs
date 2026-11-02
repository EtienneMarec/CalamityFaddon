using CalamityMod.Rarities;
using Terraria;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Items.Accessories
{
    /// <summary>
    /// Wiki : Disorderful Soul Artifact.txt — réduction des dégâts ramenée à 0, vitesse de déplacement doublée, vitesse
    /// d'attaque triplée. Recette du wiki (WikiRecipes). [choix] Vitesse : course maximale, accélération et vitesse des bottes ;
    /// attaque : +200 % à toutes les classes ; la réduction est annulée après tous les autres effets.
    /// </summary>
    public class DisorderfulSoulArtifact : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 26;
            Item.accessory = true;
            Item.value = Item.sellPrice(gold: 20);
            Item.rare = ModContent.RarityType<Turquoise>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetAttackSpeed(DamageClass.Generic) += 2f;
            player.GetModPlayer<DisorderfulPlayer>().Active = true;
        }
    }

    public class DisorderfulPlayer : ModPlayer
    {
        public bool Active;

        public override void ResetEffects() => Active = false;

        public override void PostUpdateRunSpeeds()
        {
            if (!Active)
                return;
            Player.maxRunSpeed *= 2f;
            Player.accRunSpeed *= 2f;
            Player.runAcceleration *= 2f;
        }

        public override void PostUpdateMiscEffects()
        {
            if (Active)
                Player.endurance = 0f;
        }
    }
}
