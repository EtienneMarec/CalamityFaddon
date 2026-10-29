// Adapté de Calamity Mod (objets remplacés avant 2.0.1.005 : Thermocline Blaster, Reality Rupture, The Old Reaper...), © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using System;
using CalamityFandom.Content.Legacy.Projectiles.Ranged;
using CalamityFandom.Content.Legacy.Projectiles.Rogue;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

// Objets que Calamity a remplacés (LegacyName) par un objet que le wiki ne connaît pas (décision du 30/10) : notre
// portage en Full Fandom, l'objet actuel sous le nom du wiki en Calamity++ (masquage par tools/gen_baseline.py).

namespace CalamityFandom.Content.Legacy.Items.Weapons.Ranged
{
    /// <summary>Wiki : Cursed Capper.txt — pistolet : toutes les balles deviennent des balles maudites (Cursed Inferno 4 s, vanilla).</summary>
    public class CursedCapper : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 35;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 52;
            Item.height = 32;
            Item.useTime = Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.25f;
            Item.value = Item.sellPrice(gold: 2, silver: 40);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item41;
            Item.autoReuse = true;
            Item.shootSpeed = 14f;
            Item.shoot = ProjectileID.CursedBullet;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
            => type = ProjectileID.CursedBullet;
    }
}

namespace CalamityFandom.Content.Legacy.Items.Weapons.Rogue
{
    /// <summary>
    /// Wiki : Luminous Striker.txt — javelot rogue sans gravité et qui ne traverse pas (page ; l'ancien code traversait
    /// 4 ennemis) ; laisse des éclats d'étoile qui montent (quart des dégâts) et éclate en éclats au contact. Attaque
    /// furtive : les éclats accompagnent le javelot au lieu de monter.
    /// </summary>
    public class LuminousStriker : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 86;
            Item.height = 102;
            Item.damage = 149;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useTime = Item.useAnimation = 29;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = Item.sellPrice(gold: 20);
            Item.rare = ItemRarityID.Red;
            Item.shoot = ModContent.ProjectileType<LuminousStrikerProj>();
            Item.shootSpeed = 20f;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            proj.Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }
    }

    /// <summary>
    /// Wiki : The Reaper.txt — faux rogue lâchée par les Reaper Sharks (10 %) : la faux cherche les ennemis et en touche un
    /// douze fois avant de disparaître (Crush Depth 10 s, infobox) ; attaque furtive : quatre faux. Faux de 2.0.1.005
    /// (celle de Calamity a été refaite pour The Old Reaper).
    /// </summary>
    public class TheReaper : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 106;
            Item.height = 104;
            Item.damage = 122;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useTime = Item.useAnimation = 21;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<LegacyReaperProjectile>();
            Item.shootSpeed = 20f;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.value = Item.sellPrice(gold: 26);
            Item.rare = ModContent.RarityType<CalamityMod.Rarities.PureGreen>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!player.Calamity().StealthStrikeAvailable())
                return true;
            int spread = 10;
            for (int i = 0; i < 4; i++)
            {
                Vector2 speed = (velocity + new Vector2(Main.rand.Next(-3, 4), Main.rand.Next(-3, 4))).RotatedBy(MathHelper.ToRadians(spread));
                var proj = Projectile.NewProjectileDirect(source, position, speed, type, (int)(damage * 0.8f) / 2, knockback, player.whoAmI);
                proj.Calamity().stealthStrike = true;
                spread -= Main.rand.Next(5, 8);
            }
            return false;
        }
    }
}

namespace CalamityFandom.Content.Legacy.Items.Accessories
{
    /// <summary>
    /// Wiki : Warbanner of the Sun.txt — covering des Profaned Guardians (10 %) : dégâts de mêlée, de vrai corps à corps et
    /// vitesse de mêlée en hausse plus on est près d'un ennemi, jusqu'à +20 % ; à partir de 30 blocs (480 px). Les deux
    /// bonus de dégâts se cumulent (+40 % en vrai corps à corps).
    /// </summary>
    [AutoloadEquip(EquipType.Balloon)]
    public class WarbanneroftheSun : ModItem
    {
        public const float MaxBonus = 0.2f;
        public const float MaxDistance = 480f;

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(6, 5));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 78;
            Item.value = Item.sellPrice(gold: 22);
            Item.rare = ItemRarityID.Purple;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            float bonus = Bonus(player);
            player.GetAttackSpeed<MeleeDamageClass>() += bonus;
            player.GetDamage<MeleeDamageClass>() += bonus;
            player.GetDamage<TrueMeleeDamageClass>() += bonus;
        }

        private static float Bonus(Player player)
        {
            float best = -1f;
            NPC closest = null;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.friendly || npc.dontTakeDamage || npc.damage <= 0 && !npc.boss)
                    continue;
                float d = Math.Abs(npc.Center.X - player.Center.X) + Math.Abs(npc.Center.Y - player.Center.Y);
                if (best < 0f || d < best)
                {
                    best = d;
                    closest = npc;
                }
            }
            if (closest == null)
                return 0f;
            float edge = Math.Max(0f, closest.Distance(player.Center) - Math.Max(closest.width, closest.height) / 2f);
            return edge < MaxDistance ? MathHelper.Lerp(0f, MaxBonus, 1f - edge / MaxDistance) : 0f;
        }
    }

    /// <summary>
    /// Wiki : Rusty Medallion.txt — covering pré-Hardmode : les armes à distance ont 20 % de chances de faire tomber du ciel
    /// 3 gouttes d'acide (celles du Flak Toxicannon de 2022) à 30 % des dégâts de base de l'arme, adoucies au-delà de 30
    /// (90 % moins efficaces), Irradiated 3 s. [choix] Gouttes tombant vers le curseur comme dans le code de 2022.
    /// TODO : coffres rouillés (« can be found in Rusty Chests »), génération du monde.
    /// </summary>
    public class RustyMedallion : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 32;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 20);
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual) => player.GetModPlayer<LegacyAccessoryPlayer>().RustyMedallion = true;
    }

    /// <summary>
    /// Wiki : Alchemical Flask.txt — covering : la page décrit un objet qui inflige la Plague au porteur, contre +2 % de
    /// dégâts (l'ancien code infligeait la Plague aux ennemis et lâchait des Plague Seekers) ; la page fait foi.
    /// </summary>
    public class AlchemicalFlask : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = Item.height = 26;
            Item.value = Item.sellPrice(gold: 12);
            Item.rare = ItemRarityID.Yellow;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddBuff(ModContent.BuffType<Plague>(), 2);
            player.GetDamage(DamageClass.Generic) += 0.02f;
        }
    }

    /// <summary>
    /// Wiki : Red Bow.txt — covering pêché dans le Dungeon avant Skeletron : +10 discrétion maximale, +3 défense, aggro
    /// -200. Recréé d'après la page (devenu le Ghost Bracelet, vanité, dans Calamity). [choix] Pêche : 1 chance sur 20
    /// par prise dans le Dungeon, avant Skeletron.
    /// </summary>
    public class RedBow : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 24;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(gold: 1);
            Item.defense = 3;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().rogueStealthMax += 0.1f;
            player.aggro -= 200;
        }
    }

    public class LegacyAccessoryPlayer : ModPlayer
    {
        public bool RustyMedallion;
        private int rustyCooldown;

        public override void ResetEffects() => RustyMedallion = false;

        public override void PostUpdate()
        {
            if (rustyCooldown > 0)
                rustyCooldown--;
        }

        public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!RustyMedallion || !item.CountsAsClass<RangedDamageClass>() || rustyCooldown > 0 || !Main.rand.NextBool(5))
                return true;
            rustyCooldown = 25;
            // 30 % des dégâts de base, adoucis au-delà de 30
            float raw = item.damage * 0.3f;
            int drop = (int)(raw <= 30f ? raw : 30f + (raw - 30f) * 0.1f);
            for (int i = 0; i < 3; i++)
            {
                Vector2 start = Main.MouseWorld - Vector2.UnitY.RotatedByRandom(0.4f) * 1250f;
                Vector2 dir = (Main.MouseWorld - start).SafeNormalize(Vector2.UnitY).RotatedByRandom(0.1f);
                var acid = Projectile.NewProjectileDirect(source, start, dir * 15f, ModContent.ProjectileType<ToxicannonDrop>(), drop, 0f, Player.whoAmI);
                acid.penetrate = 2;
                acid.DamageType = DamageClass.Generic;
            }
            return true;
        }

        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            if (attempt.inLava || attempt.inHoney || npcSpawn > 0 || !Player.ZoneDungeon || NPC.downedBoss3 || !Main.rand.NextBool(20))
                return;
            itemDrop = ModContent.ItemType<RedBow>();
        }
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.Ranged
{
    /// <summary>Goutte d'acide du Flak Toxicannon de 2022 (Rusty Medallion) : cherche les ennemis, traverse les blocs 2 s, Irradiated 3 s.</summary>
    public class ToxicannonDrop : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Environment/AcidDrop";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.timeLeft = 420;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 17;
        }

        public override void AI()
        {
            NPC target = Projectile.Center.ClosestNPCAt(600f, !Projectile.tileCollide);
            if (target != null)
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(target.Center) * Math.Max(5f, Projectile.velocity.Length()), 0.085f);
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            Projectile.tileCollide = Projectile.timeLeft <= 300;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (int i = 0; i < 4; i++)
            {
                var dust = Dust.NewDustDirect(Projectile.position - Projectile.velocity, 2, 2, 154, 0f, 0f, 38, new Color(112, 150, 42, 127), 2f);
                dust.velocity = dust.velocity * 0.1f - Projectile.velocity * 0.025f;
            }
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Irradiated>(), 180);

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], new Color(255, 255, 255, 127), 2);
            return false;
        }
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.Rogue
{
    /// <summary>Javelot de la Luminous Striker : laisse un éclat toutes les 4 frames ; éclate en 7 éclats au contact.</summary>
    public class LuminousStrikerProj : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Legacy/Items/Weapons/Rogue/LuminousStriker";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
        }

        public override void AI()
        {
            bool stealth = Projectile.Calamity().stealthStrike;
            if (Projectile.ai[0] == 0f && stealth)
            {
                Projectile.timeLeft = 600;
                Projectile.ai[0] = 1f;
            }
            if (Main.rand.NextBool(4))
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 176, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            if (Projectile.timeLeft % 4 != 0 || Projectile.owner != Main.myPlayer)
                return;
            int shard = ModContent.ProjectileType<LuminousShard>();
            if (stealth)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + new Vector2(Main.rand.NextFloat(-15f, 15f), Main.rand.NextFloat(-15f, 15f)), Projectile.velocity,
                    shard, (int)(Projectile.damage * 0.25f), Projectile.knockBack * 0.25f, Projectile.owner);
            else
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(0f, -2f), shard, (int)(Projectile.damage * 0.25f), Projectile.knockBack * 0.25f, Projectile.owner);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.owner != Main.myPlayer)
                return;
            for (int i = 0; i < 7; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Unit() * Main.rand.Next(30, 61) * 0.2f,
                    ModContent.ProjectileType<LuminousShard>(), (int)(Projectile.damage * 0.25f), Projectile.knockBack * 0.25f, Projectile.owner);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i <= 10; i++)
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 176, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
        }
    }

    /// <summary>Éclat d'étoile : monte en accélérant (ou suit le javelot), cherche les ennemis après 0,5 s, un ennemi.</summary>
    public class LuminousShard : ModProjectile
    {
        private bool rising;

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
        }

        public override bool? CanHitNPC(NPC target) => Projectile.timeLeft < 90 && target.CanBeChasedBy(Projectile) ? null : false;

        public override void AI()
        {
            if (Projectile.ai[0] == 0f && Projectile.velocity.X == 0f && Projectile.velocity.Y == -2f)
                rising = true;
            Projectile.ai[0] = 1f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (rising)
                Projectile.velocity.Y *= 1.05f;
            if (Projectile.timeLeft < 90)
                CalamityUtils.HomeInOnNPC(Projectile, true, 600f, 10f, 20f);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i <= 2; i++)
            {
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 176, Projectile.oldVelocity.X / 4, Projectile.oldVelocity.Y / 4, 0, default, 0.75f).noGravity = true;
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 177, Projectile.oldVelocity.X / 4, Projectile.oldVelocity.Y / 4, 0, default, 0.75f).noGravity = true;
            }
        }
    }

    /// <summary>
    /// Faux de The Reaper (ReaperProjectile de 2.0.1.005) : cherche un ennemi, puis le frappe à répétition en
    /// tournoyant ; Crush Depth 10 s (infobox ; 3 s dans le code). [choix] « hit an enemy twelve times before
    /// disappearing » : la faux disparaît à son 12e coup.
    /// </summary>
    public class LegacyReaperProjectile : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Legacy/Items/Weapons/Rogue/TheReaper";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 70;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 12;
            Projectile.extraUpdates = 3;
            Projectile.timeLeft = Projectile.MaxUpdates * 90;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 26;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
        }

        public override void AI()
        {
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 8;
                SoundEngine.PlaySound(SoundID.Item7, Projectile.position);
            }
            Projectile.rotation += 0.07f;
            if (Projectile.localAI[0] == 0f)
            {
                // Tête chercheuse jusqu'au premier coup (ou 60 mises à jour)
                if (++Projectile.ai[1] >= 60f || Projectile.numHits > 0)
                {
                    Projectile.localAI[0] = 1f;
                    Projectile.ai[1] = 0f;
                    Projectile.netUpdate = true;
                }
                else
                    CalamityUtils.HomeInOnNPC(Projectile, true, 250f, 12f, 14f);
                return;
            }
            // Ensuite : s'éloigne et revient sur la cible, par à-coups
            if (Projectile.ai[0] == 1f)
            {
                if (++Projectile.ai[1] > 40f)
                {
                    Projectile.ai[1] = 1f;
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                }
                return;
            }
            NPC target = null;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.CanBeChasedBy(Projectile))
                {
                    target = npc;
                    break;
                }
            }
            if (target != null)
            {
                Vector2 delta = target.Center - Projectile.Center;
                float distance = delta.Length();
                delta /= distance;
                Projectile.velocity = (Projectile.velocity * 40f + delta * (distance > 200f ? 11f : -3.6f)) / 41f;
            }
            if (Projectile.ai[1] > 0f)
                Projectile.ai[1] += Main.rand.Next(1, 4);
            if (Projectile.ai[1] > 40f)
            {
                Projectile.ai[1] = 0f;
                Projectile.netUpdate = true;
            }
            if (Projectile.ai[1] == 0f && target != null && Projectile.Distance(target.Center) < 500f)
            {
                Projectile.ai[1] = 1f;
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.ai[0] = 1f;
                    Projectile.velocity = Projectile.SafeDirectionTo(target.Center) * 8f;
                    Projectile.netUpdate = true;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<CrushDepth>(), 600);

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item21, Projectile.position);
            for (int i = 0; i < 8; i++)
            {
                var dust = Dust.NewDustDirect(Projectile.Center - new Vector2(50f), 100, 100, 33, 0f, 0f, 100, default, 3f);
                dust.noGravity = true;
                dust.velocity *= 5f;
            }
        }
    }
}
