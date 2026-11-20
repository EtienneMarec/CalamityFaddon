using System;
using System.Collections.Generic;
using System.Linq;
using CalamityFandom.Common.Debug;
using CalamityMod;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

// Lot BA (20/11) : The Everquartz (butin du wiki de l'Everquartz).

namespace CalamityFandom.Content.Items.Accessories
{
    /// <summary>
    /// Wiki : The Everquartz.txt — 50 défense, plus de dégâts de défense, -25 % de dégâts de contact ; à l'arrêt : régénération
    /// rapide et +75 % de défense ; barre de piézoélectricité qui se remplit en 30 s : touche « Piezoelectricity Mode » quand
    /// elle est pleine, 500 PV rendus en 2 s et -75 % de dégâts reçus pendant ce temps (Everquartz, 1 %).
    /// [choix] « Régénération rapide » = +10 ; barre dessinée sous le joueur (image de la page).
    /// </summary>
    public class TheEverquartz : WikiCovering
    {
        public static ModKeybind Key;

        protected override int Rarity => ModContent.RarityType<Turquoise>();
        protected override int SellCopper => 500000;

        public override void Load() => Key = KeybindLoader.RegisterKeybind(Mod, "PiezoelectricityMode", "V");

        public override void Unload() => Key = null;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 14;
            Item.height = 28;
            Item.defense = 50;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().externalDefenseDamageImmunity = true;
            player.GetModPlayer<EverquartzPlayer>().Equipped = true;
        }
    }

    public class EverquartzPlayer : ModPlayer
    {
        public const int ChargeTime = 1800, HealTime = 120;
        public bool Equipped;
        public int Charge, Healing;

        public override void ResetEffects() => Equipped = false;

        public override void PostUpdateEquips()
        {
            if (!Equipped)
            {
                Charge = 0;
                return;
            }
            if (Player.velocity.Length() < 0.1f)
            {
                Player.lifeRegen += 10;
                Player.statDefense += Player.statDefense.Positive * 3 / 4;
            }
            if (Healing > 0)
            {
                Healing--;
                if (Healing % 6 == 0 && Player.whoAmI == Main.myPlayer)
                    Player.Heal(25);
                Player.endurance += 0.75f;
                return;
            }
            if (Charge < ChargeTime)
                Charge++;
            else if (Player.whoAmI == Main.myPlayer && TheEverquartz.Key?.JustPressed == true)
            {
                Charge = 0;
                Healing = HealTime;
                SoundEngine.PlaySound(SoundID.Item29, Player.Center);
            }
        }

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            if (Equipped)
                modifiers.FinalDamage *= 0.75f;
        }
    }

    /// <summary>Barre de piézoélectricité sous le joueur (image de la page).</summary>
    public class EverquartzBar : ModSystem
    {
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(l => l.Name == "Vanilla: Resource Bars");
            if (index < 0)
                return;
            layers.Insert(index, new LegacyGameInterfaceLayer("CalamityFandom: Piezoelectricity", () =>
            {
                Player p = Main.LocalPlayer;
                var ep = p.GetModPlayer<EverquartzPlayer>();
                if (!ep.Equipped || p.dead)
                    return true;
                Texture2D tex = ModContent.Request<Texture2D>("CalamityFandom/Content/Items/Accessories/PiezoelectricityBar").Value;
                Vector2 pos = p.Bottom - Main.screenPosition + new Vector2(-tex.Width / 2f, 12f);
                float fill = ep.Healing > 0 ? ep.Healing / (float)EverquartzPlayer.HealTime : ep.Charge / (float)EverquartzPlayer.ChargeTime;
                Main.spriteBatch.Draw(tex, pos, Color.White * 0.5f);
                Main.spriteBatch.Draw(tex, pos, new Rectangle(0, 0, (int)(tex.Width * fill), tex.Height), Color.White);
                return true;
            }, InterfaceScaleType.Game));
        }

        public override void Load()
        {
            SelfTest.ExtraChecks.Add(t => t.Expect(Common.WikiData.WikiLoot.Added.Any(a => a.what.StartsWith("The Everquartz")), "butin du wiki : The Everquartz"));
        }
    }
}
