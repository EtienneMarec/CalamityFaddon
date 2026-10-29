// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using System;
using CalamityFandom.Content.Legacy.Projectiles.Melee;
using CalamityMod;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.Accessories
{
    /// <summary>
    /// Wiki : Mana Jelly.txt — covering retiré de Calamity : +20 mana max, régénération de mana accrue à l'arrêt.
    /// Rareté 1, vente 20 argent (infobox).
    /// </summary>
    public class ManaJelly : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 24;
            Item.value = Item.sellPrice(silver: 20);
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statManaMax2 += 20;
            if (Math.Abs(player.velocity.X) < 0.05f && Math.Abs(player.velocity.Y) < 0.05f && player.itemAnimation == 0)
                player.manaRegenBonus += 4;
        }
    }

    /// <summary>
    /// Wiki : Fungal Carapace.txt — covering retiré de Calamity : défense 6, explosion de spores de champignon quand on est
    /// touché (effet repris du vieux CalamityPlayer : 8 Truffle Spores, 70 dégâts selon la meilleure classe). Rareté 4,
    /// vente 2 or 40 argent.
    /// </summary>
    public class FungalCarapace : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.defense = 6;
            Item.width = 20;
            Item.height = 24;
            Item.value = Item.sellPrice(gold: 2, silver: 40);
            Item.rare = ItemRarityID.LightRed;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual) => player.GetModPlayer<LegacyCoveringPlayer>().FungalCarapace = Item;
    }

    /// <summary>
    /// Wiki : Amidias_ Spark.txt — covering retiré de Calamity : être touché libère une gerbe d'étincelles (effet repris du
    /// vieux CalamityPlayer : 8 étincelles, 6 dégâts selon la meilleure classe). Rareté 1, vente 20 argent.
    /// </summary>
    public class AmidiasSpark : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.value = Item.sellPrice(silver: 20);
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual) => player.GetModPlayer<LegacyCoveringPlayer>().AmidiasSpark = Item;
    }

    /// <summary>Effets « quand on est touché » des coverings portés (repris de CalamityPlayer.OnHurt en 2.0.1.005).</summary>
    public class LegacyCoveringPlayer : ModPlayer
    {
        public Item FungalCarapace, AmidiasSpark;

        public override void ResetEffects() => FungalCarapace = AmidiasSpark = null;

        public override void OnHurt(Player.HurtInfo info)
        {
            if (info.Damage <= 0 || Player.whoAmI != Main.myPlayer)
                return;
            const float Spread = 45f * 0.0174f;
            double startAngle = Math.Atan2(Player.velocity.X, Player.velocity.Y) - Spread / 2;
            double deltaAngle = Spread / 8f;
            if (FungalCarapace != null)
            {
                SoundEngine.PlaySound(SoundID.NPCHit45, Player.position);
                int damage = (int)Player.GetBestClassDamage().ApplyTo(70);
                for (int i = 0; i < 4; i++)
                {
                    var spawn = new Vector2(Player.Center.X + (Main.rand.NextBool() ? 100 : -100), Player.Center.Y + Main.rand.Next(-100, 101));
                    double angle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                    var velocity = new Vector2((float)Math.Sin(angle), (float)Math.Cos(angle)) * 5f;
                    foreach (var v in new[] { velocity, -velocity })
                        Projectile.NewProjectileDirect(Player.GetSource_Accessory(FungalCarapace), spawn, v, ProjectileID.TruffleSpore, damage, 1.25f, Player.whoAmI).timeLeft = 300;
                }
            }
            if (AmidiasSpark != null)
            {
                SoundEngine.PlaySound(SoundID.Item93, Player.position);
                int damage = (int)Player.GetBestClassDamage().ApplyTo(6);
                for (int i = 0; i < 4; i++)
                {
                    double angle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                    var velocity = new Vector2((float)Math.Sin(angle), (float)Math.Cos(angle)) * 5f;
                    foreach (var v in new[] { velocity, -velocity })
                    {
                        var spark = Projectile.NewProjectileDirect(Player.GetSource_Accessory(AmidiasSpark), Player.Center, v,
                            ModContent.ProjectileType<AquaticDischargeSpark>(), damage, 1.25f, Player.whoAmI);
                        spark.timeLeft = 120;
                        spark.DamageType = DamageClass.Generic;
                    }
                }
            }
        }
    }
}
