// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using System;
using CalamityFandom.Content.Legacy.Projectiles.Magic;
using CalamityFandom.Content.Legacy.Projectiles.Ranged;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

// Lot du 13/11 : Astral Blade, Coral Cannon (Sea King), Clamor Noctus. Code de 2.0.1.005, valeurs des infobox et du
// texte des pages ; recettes du wiki (WikiRecipes).

namespace CalamityFandom.Content.Legacy.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Astral Blade.txt — dégâts, recul et chance de critique multipliés par 1 à 2 selon la vie restante de la cible ;
    /// au-dessus de 50 % de vie, explosion de poussière (visuelle) ; Astral Infection 5 s ; seulement 25 % des bonus de
    /// vitesse de mêlée.
    /// </summary>
    public class AstralBlade : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 85;
            Item.DamageType = DamageClass.Melee;
            Item.width = Item.height = 80;
            Item.scale = 1.5f;
            Item.useTime = Item.useAnimation = 8;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4f;
            Item.crit = 25;
            Item.value = Item.sellPrice(gold: 19);
            Item.rare = ItemRarityID.Cyan;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        // Seulement un quart du bonus de vitesse de mêlée (page, Notes)
        public override float UseSpeedMultiplier(Player player)
        {
            float speed = player.GetTotalAttackSpeed(DamageClass.Melee);
            return speed <= 0f ? 1f : (1f + (speed - 1f) * 0.25f) / speed;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            for (int i = 0; i < 3; i++)
            {
                Dust d = CalamityUtils.MeleeDustHelper(player, Main.rand.NextBool() ? ModContent.DustType<AstralOrange>() : ModContent.DustType<AstralBlue>(),
                    0.7f, 55, 110, -0.07f, 0.07f);
                if (d != null)
                    d.customData = 0.03f;
            }
        }

        public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            float multiplier = MathHelper.Lerp(1f, 2f, target.life / (float)Math.Max(target.lifeMax, 1));
            modifiers.SourceDamage *= multiplier;
            modifiers.Knockback *= multiplier;
            // [choix] Critique : un second tirage avec la chance de base × (multiplicateur - 1)
            float crit = player.GetWeaponCrit(Item);
            if (Main.rand.NextFloat(100f) < crit * (multiplier - 1f))
                modifiers.SetCrit();
            if (multiplier > 1.5f && !Main.dedServ)
                AstralBurst(target.Center, multiplier);
        }

        private static void AstralBurst(Vector2 center, float multiplier)
        {
            SoundEngine.PlaySound(SoundID.Item105, center);
            bool blue = Main.rand.NextBool();
            float angleStart = Main.rand.NextFloat(0f, MathHelper.TwoPi);
            float step = 0.05f + (2f - multiplier);
            for (float angle = 0f; angle < MathHelper.TwoPi; angle += step)
            {
                blue = !blue;
                Vector2 velocity = angle.ToRotationVector2() * (2f + (float)(Math.Sin(angleStart + angle * 3f) + 1) * 2.5f) * Main.rand.NextFloat(0.95f, 1.05f);
                Dust d = Dust.NewDustPerfect(center, blue ? ModContent.DustType<AstralBlue>() : ModContent.DustType<AstralOrange>(), velocity);
                d.customData = 0.025f;
                d.scale = multiplier - 0.75f;
                d.noLight = false;
            }
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) =>
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 300);

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo) =>
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 300);
    }
}

namespace CalamityFandom.Content.Legacy.Items.Weapons.Ranged
{
    /// <summary>
    /// Wiki : Coral Cannon.txt — tire un petit corail sans munitions ; une chance sur cinq d'un gros corail (dégâts et recul
    /// doublés, un peu plus lent) qui étourdit (Eutrophication 0,25 s). Vendu 2 or par le Sea King, à tout moment.
    /// </summary>
    public class CoralCannon : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 52;
            Item.height = 40;
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7.5f;
            Item.crit = 10;
            Item.value = Item.buyPrice(gold: 2);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item61;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SmallCoralShot>();
            Item.shootSpeed = 10f;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // [choix] « un peu plus lent » : 90 % de la vitesse
            if (Main.rand.NextBool(5))
                Projectile.NewProjectile(source, position, velocity * 0.9f, ModContent.ProjectileType<BigCoral>(), damage * 2, knockback * 2f, player.whoAmI);
            else
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }
    }

    /// <summary>Le Sea King vend le Coral Cannon (page : 2 or, à tout moment).</summary>
    public class CoralCannonShop : GlobalNPC
    {
        public override void ModifyShop(NPCShop shop)
        {
            if (shop.NpcType == ModContent.NPCType<CalamityMod.NPCs.TownNPCs.SeaKing>())
                shop.Add<CoralCannon>();
        }
    }
}

namespace CalamityFandom.Content.Legacy.Items.Weapons.Magic
{
    /// <summary>
    /// Wiki : Clamor Noctus.txt — trois projectiles tombent du ciel vers le curseur : des étoiles (15 % des dégâts, Moon
    /// Burnt 3 s) qui se collent aux ennemis et aux blocs 2 s puis explosent (135 %), ou des Night Wyverns (375 %).
    /// [choix] Une chance sur dix d'un Night Wyvern par projectile (vieux code).
    /// </summary>
    public class ClamorNoctus : ModItem
    {
        public const float StarDamage = 0.15f, ExplosionDamage = 1.35f, WyvernDamage = 3.75f;

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 270;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 12;
            Item.width = 52;
            Item.height = 74;
            Item.useTime = Item.useAnimation = 21;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5.2f;
            Item.value = Item.sellPrice(gold: 28);
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.UseSound = SoundID.Item105;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ClamorNoctusStar>();
            Item.shootSpeed = 25f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 mouse = Main.MouseWorld;
            for (int i = 0; i < 3; i++)
            {
                bool wyvern = Main.rand.NextBool(10);
                Vector2 spawn = new(player.Center.X + player.width * 0.5f + Main.rand.Next(201) * -player.direction + (mouse.X - player.Center.X),
                    player.MountedCenter.Y - 600f);
                spawn.X = (spawn.X + player.Center.X) / 2f + Main.rand.Next(-200, 201);
                spawn.Y -= 100 * i;
                Vector2 toMouse = mouse - spawn;
                toMouse.Y = Math.Max(Math.Abs(toMouse.Y), 20f);
                Vector2 shot = toMouse.SafeNormalize(Vector2.UnitY) * Item.shootSpeed + Main.rand.NextVector2Square(-0.6f, 0.6f);
                Projectile.NewProjectile(source, spawn, shot, wyvern ? ModContent.ProjectileType<ClamorNoctusWyvern>() : type,
                    (int)(damage * (wyvern ? WyvernDamage : StarDamage)), knockback * (wyvern ? 1.5f : 1f), player.whoAmI);
            }
            return false;
        }
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.Ranged
{
    /// <summary>Petit corail du Coral Cannon (texture de Calamity, qui en a fait un projectile de spy).</summary>
    public class SmallCoralShot : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Rogue/SmallCoral";

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
        }

        public override void AI()
        {
            Projectile.velocity.X *= 0.9995f;
            Projectile.velocity.Y += 0.01f;
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.direction == -1 ? MathHelper.Pi : 0f);
        }

        public override void OnKill(int timeLeft) => CoralBurst(Projectile, 5, 1f);

        public static void CoralBurst(Projectile projectile, int count, float scale)
        {
            SoundEngine.PlaySound(SoundID.Item10, projectile.Center);
            for (int i = 0; i < count; i++)
            {
                Dust d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.SnowBlock,
                    projectile.oldVelocity.X / 4, projectile.oldVelocity.Y / 4, 0, new Color(234, 183, 100), scale);
                d.noGravity = true;
                d.velocity *= 3f;
            }
        }
    }

    /// <summary>Gros corail du Coral Cannon : étourdit (Eutrophication 0,25 s ; Calamity ne l'applique pas aux boss).</summary>
    public class BigCoral : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 34;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
        }

        public override void AI()
        {
            Projectile.velocity.X *= 0.999f;
            Projectile.velocity.Y += 0.025f;
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.direction == -1 ? MathHelper.Pi : 0f);
        }

        public override void OnKill(int timeLeft) => SmallCoralShot.CoralBurst(Projectile, 10, 1.5f);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Eutrophication>(), 15);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<Eutrophication>(), 15);
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.Magic
{
    /// <summary>
    /// Étoile de la Clamor Noctus : se colle à l'ennemi ou au bloc touché, explose au bout de 2 s (135 % des dégâts de
    /// l'arme, soit 9 fois ceux de l'étoile) ; Moon Burnt 3 s à l'impact et à l'explosion.
    /// </summary>
    public class ClamorNoctusStar : ModProjectile
    {
        private const int StickTime = 120;

        private ref float State => ref Projectile.ai[0];   // 0 en vol, 1 collée à un ennemi, 2 collée à un bloc, 3 explosion
        private ref float Target => ref Projectile.ai[1];
        private ref float Timer => ref Projectile.ai[2];
        private Vector2 offset;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 600;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void SendExtraAI(System.IO.BinaryWriter writer) => writer.WriteVector2(offset);

        public override void ReceiveExtraAI(System.IO.BinaryReader reader) => offset = reader.ReadVector2();

        public override void AI()
        {
            if (State == 3f)
                return;
            if (State == 0f)
            {
                Projectile.rotation += 0.3f * Math.Sign(Projectile.velocity.X == 0f ? 1f : Projectile.velocity.X);
                if (Main.rand.NextBool(3))
                    Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 20,
                        Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                return;
            }
            if (State == 1f)
            {
                NPC npc = Main.npc[(int)Target];
                if (!npc.active || npc.dontTakeDamage)
                {
                    Explode();
                    return;
                }
                Projectile.Center = npc.Center + offset;
            }
            Projectile.velocity = Vector2.Zero;
            if (++Timer >= StickTime * (Projectile.extraUpdates + 1))
                Explode();
        }

        private void Explode()
        {
            if (Projectile.owner != Main.myPlayer)
                return;
            // L'explosion : la même étoile, agrandie, frappe une fois chaque ennemi de la zone
            State = 3f;
            Projectile.damage = (int)(Projectile.damage * (Items.Weapons.Magic.ClamorNoctus.ExplosionDamage / Items.Weapons.Magic.ClamorNoctus.StarDamage));
            Projectile.Resize(80, 80);
            Projectile.tileCollide = false;
            Projectile.velocity = Vector2.Zero;
            Projectile.timeLeft = 3;
            for (int i = 0; i < Main.maxNPCs; i++)
                Projectile.localNPCImmunity[i] = 0;
            Projectile.netUpdate = true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (State == 0f)
            {
                State = 2f;
                Projectile.velocity = Vector2.Zero;
                Projectile.netUpdate = true;
            }
            return false;
        }

        public override bool? CanHitNPC(NPC target) => State == 0f || State == 3f ? null : false;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Nightwither>(), 180);
            if (State == 0f)
            {
                State = 1f;
                Target = target.whoAmI;
                offset = Projectile.Center - target.Center;
                Projectile.netUpdate = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            for (int i = 0; i < 30; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.Center - new Vector2(25f), 50, 50, 20, 0f, 0f, 100, default, i % 2 == 0 ? 1.7f : 1f);
                d.noGravity = i % 2 == 0;
                d.velocity *= i % 2 == 0 ? 5f : 2f;
            }
            if (!Main.dedServ)
                for (int i = 0; i < 3; i++)
                    Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity * 0.05f, Main.rand.Next(16, 18));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (State == 3f)
                return false;
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 2);
            return false;
        }
    }

    /// <summary>Night Wyvern de la Clamor Noctus : tombe droit, disparaît au premier ennemi ou bloc touché.</summary>
    public class ClamorNoctusWyvern : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Main.rand.NextBool(5))
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 126,
                    Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCHit7, Projectile.Center);
            for (int i = 0; i <= 10; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 126, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f,
                Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
