// Adapté de Calamity Mod (objets retirés en 2022), © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using System;
using CalamityFandom.Content.Legacy.Buffs.Potions;
using CalamityFandom.Content.Legacy.Projectiles.Typeless;
using CalamityMod;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.Weapons.Typeless
{
    /// <summary>
    /// Wiki : Marked Magnum.txt — arme sans classe (tous les bonus de dégâts à la fois), sans munitions ni mana, jamais de
    /// critique ; ses tirs infligent Marked 8 s.
    /// </summary>
    public class MarkedMagnum : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.DamageType = ModContent.GetInstance<AverageDamageClass>();
            Item.damage = 4;
            Item.width = 54;
            Item.height = 20;
            Item.useTime = Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.value = Item.sellPrice(silver: 40);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item33;
            Item.shootSpeed = 12f;
            Item.shoot = ModContent.ProjectileType<MarkRound>();
        }

        // « does not have any critical strike chance and cannot gain any »
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit = 0f;

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);
    }
}

namespace CalamityFandom.Content.Legacy.Items.Potions
{
    /// <summary>
    /// Wiki : Purified Jam.txt — buff Invincible 10 s (5 s en Death Mode et pendant le Boss Rush), Potion Sickness 30 s
    /// (25 s avec la Philosopher's Stone) ; inutilisable sous Potion Sickness. Lâchée par The Slime God (butin du wiki).
    /// </summary>
    public class PurifiedJam : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 20;

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.useTurn = true;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.Orange;
            Item.useTime = Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.UseSound = SoundID.Item3;
            Item.consumable = true;
            Item.value = Item.sellPrice(silver: 40);
        }

        private static bool Short => CalamityWorld.death || BossRushEvent.BossRushActive;

        public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
        {
            if (!Short)
                return;
            var line = tooltips.Find(x => x.Mod == "Terraria" && x.Name == "Tooltip0");
            if (line != null)
                line.Text = line.Text.Replace("10 seconds", "5 seconds");
        }

        public override bool CanUseItem(Player player) => !player.HasBuff(BuffID.PotionSickness);

        public override bool? UseItem(Player player)
        {
            player.AddBuff(ModContent.BuffType<Invincible>(), Short ? 300 : 600);
            player.AddBuff(BuffID.PotionSickness, player.pStone ? 1500 : 1800);
            return true;
        }
    }

    /// <summary>
    /// Wiki : Revivify Potion.txt — buff Revivify 3 min : les attaques ennemies soignent du 1/15 de leurs dégâts, après
    /// coup (sans effet sur un coup mortel).
    /// </summary>
    public class RevivifyPotion : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 20;

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 36;
            Item.useTurn = true;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.LightRed;
            Item.useTime = Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.UseSound = SoundID.Item3;
            Item.value = Item.sellPrice(silver: 40);
            Item.consumable = true;
            Item.buffType = ModContent.BuffType<Revivify>();
            Item.buffTime = 180 * 60;
        }
    }
}

namespace CalamityFandom.Content.Legacy.Buffs.Potions
{
    /// <summary>
    /// Invincible (Purified Jam) : aucun dégât et immunité à la plupart des debuffs. [choix] « most debuffs » = la liste
    /// du wiki (modèle « Immunity item debuffs », partagée avec la Margarita) ; « Distorted » n'existe plus dans
    /// Calamity, « Oozed » = OgreSpit.
    /// </summary>
    public class Invincible : ModBuff
    {
        private static readonly int[] VanillaImmunities =
        {
            BuffID.BetsysCurse, BuffID.Blackout, BuffID.Bleeding, BuffID.BrokenArmor, BuffID.Burning, BuffID.Chilled,
            BuffID.Confused, BuffID.Cursed, BuffID.CursedInferno, BuffID.Darkness, BuffID.Dazed, BuffID.Electrified,
            BuffID.Rabies, BuffID.Frostburn, BuffID.Frozen, BuffID.Ichor, BuffID.OnFire, BuffID.OgreSpit, BuffID.Poisoned,
            BuffID.ShadowFlame, BuffID.Silenced, BuffID.Slow, BuffID.Stoned, BuffID.Suffocation, BuffID.Venom,
            BuffID.Weak, BuffID.Webbed,
        };

        private static readonly string[] CalamityImmunities =
        {
            "AstralInfectionDebuff", "BrimstoneFlames", "BurningBlood", "CrushDepth", "GodSlayerInferno", "HolyFlames",
            "Irradiated", "MarkedforDeath", "Plague", "SulphuricPoisoning",
        };

        private static int[] immunities;

        public static int[] Immunities => immunities ??= Resolve();

        private static int[] Resolve()
        {
            var list = new System.Collections.Generic.List<int>(VanillaImmunities) { ModContent.BuffType<StatDebuffs.GlacialState>() };
            foreach (string name in CalamityImmunities)
                if (ModContent.TryFind<ModBuff>("CalamityMod", name, out var buff))
                    list.Add(buff.Type);
            return list.ToArray();
        }

        public override void SetStaticDefaults() => Main.pvpBuff[Type] = true;

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<LegacyPotionPlayer>().Invincible = true;
            foreach (int buff in Immunities)
                player.buffImmune[buff] = true;
        }
    }

    /// <summary>Revivify (Revivify Potion) : soigne de 1/15 des dégâts reçus.</summary>
    public class Revivify : ModBuff
    {
        public override void SetStaticDefaults() => Main.pvpBuff[Type] = true;

        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<LegacyPotionPlayer>().Revivify = true;
    }

    public class LegacyPotionPlayer : ModPlayer
    {
        public bool Invincible;
        public bool Revivify;

        public override void ResetEffects() => Invincible = Revivify = false;

        public override bool FreeDodge(Player.HurtInfo info) => Invincible;

        public override void OnHurt(Player.HurtInfo info)
        {
            if (!Revivify || Player.whoAmI != Main.myPlayer || Player.statLife <= 0)
                return;
            int heal = info.Damage / 15;
            if (heal > 0)
                Player.Heal(heal);
        }
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.Typeless
{
    /// <summary>Tir de la Marked Magnum (rayon violet de Calamity), Marked 8 s.</summary>
    public class MarkRound : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/LaserProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 5;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 2;
            Projectile.timeLeft = 300;
            Projectile.DamageType = ModContent.GetInstance<AverageDamageClass>();
        }

        public override void AI()
        {
            Projectile.alpha = Math.Max(0, Projectile.alpha - 25);
            Lighting.AddLight(Projectile.Center, 0.5f, 0f, 0.7f);
            if (Projectile.ai[1] == 0f)
                Projectile.localAI[0] = Math.Min(30f, Projectile.localAI[0] + 2f);
            else if ((Projectile.localAI[0] -= 2f) <= 0f)
                Projectile.Kill();
        }

        public override Color? GetAlpha(Color lightColor) => new Color(200, 0, 250, 0);

        public override bool PreDraw(ref Color lightColor) => Projectile.DrawBeam(30f, 2f, lightColor);

        public override void OnKill(int timeLeft)
        {
            int count = Main.rand.Next(3, 7);
            for (int i = 0; i < count; i++)
            {
                var dust = Dust.NewDustDirect(Projectile.Center - Projectile.velocity / 2f, 0, 0, 173, 0f, 0f, 100, default, 2.1f);
                dust.velocity *= 2f;
                dust.noGravity = true;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => modifiers.DisableCrit();

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 480);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 480);
    }
}
