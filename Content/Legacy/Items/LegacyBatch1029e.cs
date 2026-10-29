// Adapté de Calamity Mod (objets retirés en 2022), © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using CalamityFandom.Content.Legacy.Projectiles.Melee;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Enums;
using CalamityMod.Dusts;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Animus.txt — épée Shadowspec : à chaque coup, les dégâts de base des coups suivants sont tirés au hasard
    /// (25 % 800, 25 % 1 200, 20 % 1 800, 15 % 3 000, 10 % 6 000, 5 % 10 000, la liste de la page) ; Brimstone Flames 5 s
    /// (infobox ; le texte dit 100 s, contredit par l'infobox et l'ancien code).
    /// </summary>
    public class Animus : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 82;
            Item.height = 84;
            Item.scale = 1.5f;
            Item.damage = 800;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.knockBack = 20f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = Item.sellPrice(platinum: 1);
            Item.rare = ModContent.RarityType<CalamityMod.Rarities.HotPink>();
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage) => damage *= player.GetModPlayer<AnimusPlayer>().Boost;

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 300);
            player.GetModPlayer<AnimusPlayer>().Roll();
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 300);
            player.GetModPlayer<AnimusPlayer>().Roll();
        }
    }

    /// <summary>Multiplicateur de dégâts de l'Animus, tiré à chaque coup (remplace l'ancien CalamityPlayer.animusBoost).</summary>
    public class AnimusPlayer : ModPlayer
    {
        public float Boost = 1f;

        public void Roll()
        {
            int roll = Main.rand.Next(100);
            Boost = roll switch
            {
                < 25 => 1f,
                < 50 => 1.5f,
                < 70 => 2.25f,
                < 85 => 3.75f,
                < 95 => 7.5f,
                _ => 12.5f,
            };
        }
    }

    /// <summary>
    /// Wiki : Blade of Enmity.txt — épée rapide sans projectile, Brimstone Flames 5 s ; « only benefits from 25% of melee
    /// swing speed bonuses » (Notes).
    /// </summary>
    public class BladeofEnmity : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 60;
            Item.height = 64;
            Item.scale = 1.5f;
            Item.damage = 190;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 9;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.knockBack = 8f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = Item.sellPrice(gold: 16);
            Item.rare = ItemRarityID.Yellow;
        }

        // Ne garde que 25 % du bonus de vitesse de mêlée : Terraria applique déjà la vitesse totale, on la ramène
        public override float UseSpeedMultiplier(Player player)
        {
            float speed = player.GetTotalAttackSpeed(DamageClass.Melee);
            return speed > 0f ? (1f + 0.25f * (speed - 1f)) / speed : 1f;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 300);

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo) => target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 300);
    }

    /// <summary>
    /// Wiki : Brimlash Buster.txt — « 50% chance to do triple damage on enemy hits » (infobulle ; l'ancien code doublait
    /// à 1 chance sur 3) ; à chaque coup, au hasard : le grand trait de la Brimlash vers le curseur (il se divise à sa
    /// mort) ou quatre traits autour du joueur (gauche, droite et diagonales) ; Brimstone Flames 5 s au contact, 3 s par
    /// les traits (ceux de Calamity).
    /// </summary>
    public class BrimlashBuster : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = Item.height = 72;
            Item.damage = 126;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 24;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 8f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = Item.sellPrice(gold: 20);
            Item.rare = ItemRarityID.Red;
            Item.shoot = ModContent.ProjectileType<BrimlashProj>();
            Item.shootSpeed = 18f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.rand.NextBool())
                return true;
            // [choix] « four brimstone bolts to the left, right, and diagonals » : gauche, droite et les deux diagonales hautes
            float speed = velocity.Length();
            foreach (Vector2 dir in new[] { -Vector2.UnitX, Vector2.UnitX, new Vector2(-1f, -1f), new Vector2(1f, -1f) })
                Projectile.NewProjectile(source, player.Center, Vector2.Normalize(dir) * speed, ModContent.ProjectileType<Brimlash2>(), damage, knockback, player.whoAmI);
            return false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
                Dust.NewDust(hitbox.TopLeft(), hitbox.Width, hitbox.Height, (int)CalamityDusts.Brimstone);
        }

        public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Main.rand.NextBool())
                modifiers.SourceDamage *= 3f;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 300);

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo) => target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 300);
    }

    /// <summary>
    /// Wiki : Phoenix Blade.txt — un ennemi tué explose (On Fire! 3 s) et laisse deux flammes de soin qui rendent 10 PV
    /// au contact et disparaissent après environ 3 s ou sur un bloc.
    /// </summary>
    public class PhoenixBlade : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = Item.height = 106;
            Item.damage = 160;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 26;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.knockBack = 8f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = Item.sellPrice(gold: 9, silver: 60);
            Item.rare = ItemRarityID.LightPurple;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.life <= 0)
                Explode(player, target.Center, hit.Knockback, hit.Crit ? hit.Damage / 2 : hit.Damage);
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            if (target.statLife <= 0)
                Explode(player, target.Center, Item.knockBack, hurtInfo.Damage);
        }

        private void Explode(Player player, Vector2 at, float knockback, int damage)
        {
            if (player.whoAmI != Main.myPlayer)
                return;
            var source = player.GetSource_ItemUse(Item);
            var boom = Projectile.NewProjectileDirect(source, at, Vector2.Zero, ModContent.ProjectileType<FuckYou>(), damage, knockback, player.whoAmI, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
            boom.DamageType = DamageClass.Melee;
            boom.GetGlobalProjectile<PhoenixExplosion>().Ignite = true;
            float speedX = Main.rand.Next(5);
            float speedY = Main.rand.Next(3, 7);
            int heal = ModContent.ProjectileType<PhoenixHeal>();
            Projectile.NewProjectile(source, at, new Vector2(-speedX, -speedY), heal, 0, 0f, player.whoAmI);
            Projectile.NewProjectile(source, at, new Vector2(speedX, -speedY), heal, 0, 0f, player.whoAmI);
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(4))
                Dust.NewDust(hitbox.TopLeft(), hitbox.Width, hitbox.Height, 244);
        }
    }

    /// <summary>Explosion de la Phoenix Blade (celle de Calamity) : On Fire! 3 s aux ennemis touchés (infobox).</summary>
    public class PhoenixExplosion : GlobalProjectile
    {
        public bool Ignite;

        public override bool InstancePerEntity => true;

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Ignite)
                target.AddBuff(BuffID.OnFire, 180);
        }
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.Melee
{
    /// <summary>
    /// Flamme de soin de la Phoenix Blade : ralentit, rend 10 PV au joueur qui la touche, disparaît au bout d'environ 3 s
    /// (page ; 1,75 s dans l'ancien code) ou sur un bloc ; attirée par le Heartreach dans sa seconde moitié (ancien code).
    /// </summary>
    public class PhoenixHeal : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.friendly = false;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 3;
            Projectile.timeLeft = 720;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.95f;
            Player player = Main.player[Projectile.owner];
            if (Projectile.Hitbox.Intersects(player.Hitbox))
            {
                if (Projectile.owner == Main.myPlayer && !player.moonLeech)
                {
                    player.HealPlayer(10, HealTextType.Local);
                    NetMessage.SendData(MessageID.SpiritHeal, -1, -1, null, player.whoAmI, 10);
                }
                Projectile.Kill();
                return;
            }
            if (player.lifeMagnet && Projectile.timeLeft < 360)
            {
                Vector2 toPlayer = (player.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 6f;
                Projectile.velocity = (Projectile.velocity * 15f + toPlayer) / 16f;
            }
            var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 244, 0f, 0f, 100);
            dust.noGravity = true;
            dust.position -= Projectile.velocity * 0.2f;
        }
    }
}
