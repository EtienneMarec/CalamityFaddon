// Adapté de Calamity Mod (objets retirés en 2022), © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using CalamityFandom.Content.Legacy.Projectiles.Melee;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Trinity.txt — quatre traits de gemme (tous rubis, saphir ou améthyste) en petite gerbe, à 60 % des dégâts ;
    /// ils ne traversent ni les ennemis ni les blocs.
    /// </summary>
    public class Trinity : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 54;
            Item.damage = 50;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 17;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = Item.sellPrice(gold: 7, silver: 20);
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ProjectileID.RubyBolt;
            Item.shootSpeed = 11f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            type = Main.rand.Next(new[] { ProjectileID.RubyBolt, ProjectileID.SapphireBolt, ProjectileID.AmethystBolt });
            for (int i = 0; i < 4; i++)
            {
                Vector2 speed = velocity + new Vector2(Main.rand.Next(-30, 31), Main.rand.Next(-30, 31)) * 0.05f;
                var bolt = Projectile.NewProjectileDirect(source, position, speed, type, (int)(damage * 0.6f), knockback, player.whoAmI);
                bolt.DamageType = DamageClass.Melee;
                bolt.penetrate = 1;
            }
            return false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
                Dust.NewDust(hitbox.TopLeft(), hitbox.Width, hitbox.Height, 73);
        }
    }

    /// <summary>
    /// Wiki : Fulguration Halberd.txt — épée au clic gauche, lance au clic droit (vrai corps à corps) ; Burning Blood 5 s.
    /// </summary>
    public class FulgurationHalberd : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 60;
            Item.height = 64;
            Item.scale = 1.5f;
            Item.damage = 70;
            Item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Item.useTime = Item.useAnimation = 21;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.knockBack = 5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = Item.sellPrice(gold: 7, silver: 20);
            Item.rare = ItemRarityID.Pink;
            Item.shootSpeed = 8f;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            bool spear = player.altFunctionUse == 2;
            Item.noMelee = spear;
            Item.noUseGraphic = spear;
            Item.useStyle = spear ? ItemUseStyleID.Shoot : ItemUseStyleID.Swing;
            Item.shoot = spear ? ModContent.ProjectileType<FulgurationHalberdProj>() : ProjectileID.None;
            return !spear || player.ownedProjectileCounts[Item.shoot] <= 0;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<BurningBlood>(), 300);

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo) => target.AddBuff(ModContent.BuffType<BurningBlood>(), 300);
    }

    /// <summary>
    /// Wiki : Mariana.txt — un ennemi touché éclate en 4 ou 5 bulles (2 ou 3 dans l'ancien code) qui vont loin, rebondissent
    /// sans fin sur les blocs, traversent deux ennemis et éclatent au troisième ou après environ 2 s (page).
    /// </summary>
    public class Mariana : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 54;
            Item.height = 62;
            Item.scale = 1.5f;
            Item.damage = 90;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 23;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.knockBack = 6.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = Item.sellPrice(gold: 12);
            Item.rare = ItemRarityID.Lime;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) => Burst(player, target.Center, hit.Knockback);

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo) => Burst(player, target.Center, Item.knockBack);

        private void Burst(Player player, Vector2 at, float knockback)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                int count = Main.rand.Next(4, 6);
                for (int i = 0; i < count; i++)
                {
                    // « velocity = 16 » de l'infobox
                    Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(12f, 16f);
                    Projectile.NewProjectile(player.GetSource_ItemUse(Item), at, velocity, ModContent.ProjectileType<MarianaProjectile>(), player.GetWeaponDamage(Item), knockback, player.whoAmI);
                }
            }
            for (int i = 0; i < 30; i++)
            {
                var dust = Dust.NewDustDirect(player.position, player.width, player.height, 59, 0f, 0f, 100, default, 2f);
                dust.velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    dust.scale = 0.5f;
                    dust.fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
                Dust.NewDust(hitbox.TopLeft(), hitbox.Width, hitbox.Height, 59);
        }
    }

    /// <summary>
    /// Wiki : Aquatic Dissolution.txt — trois lances aquatiques tombent du ciel au-dessus du curseur ; elles traversent les
    /// blocs pendant une seconde, puis rebondissent deux fois et disparaissent à la troisième.
    /// </summary>
    public class AquaticDissolution : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 72;
            Item.damage = 125;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 15;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6f;
            Item.UseSound = SoundID.Item60;
            Item.autoReuse = true;
            Item.value = Item.sellPrice(gold: 24);
            Item.rare = ModContent.RarityType<CalamityMod.Rarities.Turquoise>();
            Item.shoot = ModContent.ProjectileType<OceanBeam>();
            Item.shootSpeed = 12f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 mouse = Main.MouseWorld;
            for (int i = 0; i < 3; i++)
                Projectile.NewProjectile(source, new Vector2(mouse.X + Main.rand.Next(-30, 31), mouse.Y - 600f), new Vector2(0f, 8f), type, damage, knockback, player.whoAmI);
            return false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (!Main.rand.NextBool(5))
                return;
            var dust = Dust.NewDustDirect(hitbox.TopLeft(), hitbox.Width, hitbox.Height, 33, player.direction * 2, 0f, 150, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1.3f);
            dust.velocity *= 0.2f;
            dust.noGravity = true;
        }
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.Melee
{
    /// <summary>Lance de la Fulguration Halberd (IA de lance de Calamity), Burning Blood 5 s.</summary>
    public class FulgurationHalberdProj : BaseSpearProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.aiStyle = ProjAIStyleID.Spear;
            Projectile.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Projectile.timeLeft = 90;
            Projectile.scale = 1.5f;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
        }

        public override float InitialSpeed => 3f;
        public override float ReelbackSpeed => 2.4f;
        public override float ForwardSpeed => 0.95f;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<BurningBlood>(), 300);
    }

    /// <summary>Bulle de la Mariana : rebondit sans fin, 3 ennemis, environ 2 s. [choix] Sans tête chercheuse (absente de la page).</summary>
    public class MarianaProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 120;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI() => Lighting.AddLight(Projectile.Center, 0f, 0.2f, 0.8f);

        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.timeLeft < 85)
            {
                byte b = (byte)(Projectile.timeLeft * 3);
                return new Color(b, b, b, (byte)(100f * b / 255f));
            }
            return new Color(255, 255, 255, 100);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 12; i++)
            {
                var dust = Dust.NewDustDirect(Projectile.Center, 0, 0, 221, 0f, 0f, 160, default, 1.1f);
                dust.noGravity = true;
                dust.velocity = Main.rand.NextVector2Unit() * 1.25f;
            }
        }
    }

    /// <summary>
    /// Lance aquatique de l'Aquatic Dissolution : traverse les blocs pendant 1 s (page ; l'ancien code ne comptait que 20
    /// frames de jeu), puis rebondit deux fois et disparaît à la troisième (penetrate partagé avec les ennemis, comme avant).
    /// </summary>
    public class OceanBeam : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (++Projectile.frameCounter > 24)
            {
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
                Projectile.frameCounter = 0;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            // Une seconde de jeu = 180 mises à jour avec extraUpdates = 2
            if (++Projectile.ai[1] >= 180f)
                Projectile.tileCollide = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (--Projectile.penetrate <= 0)
                return true;
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 56, Projectile.oldVelocity.X * 0.25f, Projectile.oldVelocity.Y * 0.25f);
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 245, Projectile.oldVelocity.X * 0.25f, Projectile.oldVelocity.Y * 0.25f);
            }
        }
    }
}
