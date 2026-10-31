// Adapté de Calamity Mod (versions mensuelles de 2022, legacy_extract.py), © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

// Lot du 31/10 (suite) : Catastrophe Claymore → Devastation, qui débloque une recette du wiki. Les trois projectiles de
// chaque arme ne diffèrent que par leur couleur et leur debuff : une classe de base par famille. Numéros de poussière du
// code de 2022 (73 rouge, 56 bleu, 57 jaune).

namespace CalamityFandom.Content.Legacy.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Catastrophe Claymore.txt — tire un de trois éclairs d'énergie explosifs (rouge : On Fire! 2 s, bleu :
    /// Frostburn 1,5 s, jaune : Ichor 1 s) ; la lame a 1 chance sur 3 d'infliger les trois (3 s, 2 s, 1 s).
    /// </summary>
    public class CatastropheClaymore : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = Item.height = 56;
            Item.damage = 98;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 22;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6.25f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.Melee.CalamityAura>();
            Item.shootSpeed = 11f;
            Item.value = Item.sellPrice(gold: 9, silver: 60);
            Item.rare = ItemRarityID.LightPurple;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            type = Main.rand.Next(3) switch
            {
                0 => ModContent.ProjectileType<Projectiles.Melee.CalamityAura>(),
                1 => ModContent.ProjectileType<Projectiles.Melee.CalamityAuraType2>(),
                _ => ModContent.ProjectileType<Projectiles.Melee.CalamityAuraType3>(),
            };
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
                Dust.NewDust(hitbox.TopLeft(), hitbox.Width, hitbox.Height, 73);
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!Main.rand.NextBool(3))
                return;
            target.AddBuff(BuffID.Ichor, 60);
            target.AddBuff(BuffID.OnFire, 180);
            target.AddBuff(BuffID.Frostburn, 120);
        }
    }

    /// <summary>
    /// Wiki : Devastation.txt — tire une décharge galactique depuis la lame et 4 salves de trois depuis le ciel vers le
    /// curseur (moitié des dégâts) ; elles explosent près d'un ennemi (rouge : On Fire! 3 s, bleue : Frostburn 2 s,
    /// jaune : Ichor 1 s). La lame inflige les trois debuffs à chaque coup.
    /// </summary>
    public class Devastation : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = Item.height = 72;
            Item.damage = 132;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 23;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4.25f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.Melee.GalaxyBlast>();
            Item.shootSpeed = 16f;
            Item.value = Item.sellPrice(gold: 20);
            Item.rare = ItemRarityID.Red;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int red = ModContent.ProjectileType<Projectiles.Melee.GalaxyBlast>();
            int blue = ModContent.ProjectileType<Projectiles.Melee.GalaxyBlastType2>();
            int yellow = ModContent.ProjectileType<Projectiles.Melee.GalaxyBlastType3>();
            // Depuis la lame : une décharge au hasard (la rouge plus souvent, comme dans le code de 2022)
            type = Main.rand.Next(6) switch { 2 => blue, 3 => yellow, _ => red };
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, -2f);
            if (player.whoAmI != Main.myPlayer)
                return false;
            // Depuis le ciel : 4 salves au-dessus du joueur, vers le curseur, à moitié des dégâts
            Vector2 mouse = Main.MouseWorld;
            for (int i = 0; i < 4; i++)
            {
                var from = new Vector2(player.Center.X + Main.rand.Next(201) * -player.direction + (mouse.X - player.position.X), player.MountedCenter.Y - 600f);
                from.X = (from.X + player.Center.X) / 2f + Main.rand.Next(-200, 201);
                from.Y -= 100 * i;
                Vector2 dir = mouse - from;
                if (dir.Y < 20f)
                    dir.Y = 20f;
                dir = Vector2.Normalize(dir) * Item.shootSpeed + new Vector2(Main.rand.Next(-40, 41) * 0.02f, Main.rand.Next(-40, 41) * 0.02f);
                // ai[1] : hauteur à partir de laquelle la décharge heurte les blocs (elle traverse le plafond)
                float floor = mouse.Y;
                Projectile.NewProjectile(source, from, dir, red, damage / 2, knockback, player.whoAmI, 0f, floor);
                Projectile.NewProjectile(source, from, dir, blue, damage / 2, knockback, player.whoAmI, 0f, floor);
                Projectile.NewProjectile(source, from, dir, yellow, damage / 2, knockback, player.whoAmI, 0f, floor);
            }
            return false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
                Dust.NewDust(hitbox.TopLeft(), hitbox.Width, hitbox.Height, 73);
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Ichor, 60);
            target.AddBuff(BuffID.OnFire, 180);
            target.AddBuff(BuffID.Frostburn, 120);
        }
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.Melee
{
    /// <summary>Éclair d'énergie de la Catastrophe Claymore (invisible, fait de poussière) : explose au contact.</summary>
    public abstract class CalamityAuraBase : ModProjectile
    {
        protected abstract int DustType { get; }
        protected abstract int Debuff { get; }
        protected abstract int DebuffTicks { get; }

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 80;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void AI()
        {
            for (int i = 0; i < 3; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width - 28, Projectile.height - 28, DustType, 0f, 0f, 100, default, 1.35f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity = Main.dust[d].velocity * 0.1f + Projectile.velocity * 0.5f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(Debuff, DebuffTicks);

        // Même éclair pour toute la famille Catastrophe (Claymore, Claycore, Clayscore, Clayspore) : classe de dégâts de
        // l'arme qui le tire, ou du projectile dont il sort (bombe de la Claycore)
        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse use)
                Projectile.DamageType = use.Item.DamageType;
            else if (source is EntitySource_Parent { Entity: Projectile parent })
                Projectile.DamageType = parent.DamageType;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            // Explosion de 40 px qui touche tout ce qui est autour
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 40;
            Projectile.Center = Projectile.position;
            Projectile.maxPenetrate = Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();
            Vector2 push = Projectile.velocity * Projectile.MaxUpdates;
            for (int i = 0; i < 40; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType, 0f, 0f, 200, default, 1.7f);
                Main.dust[d].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * Main.rand.NextFloat() * Projectile.width / 2f;
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity = Main.dust[d].velocity * 3f + push * Main.rand.NextFloat();
            }
        }
    }

    public class CalamityAura : CalamityAuraBase
    {
        protected override int DustType => 73;
        protected override int Debuff => BuffID.OnFire;
        protected override int DebuffTicks => 120;
    }

    public class CalamityAuraType2 : CalamityAuraBase
    {
        protected override int DustType => 56;
        protected override int Debuff => BuffID.Frostburn;
        protected override int DebuffTicks => 90;
    }

    public class CalamityAuraType3 : CalamityAuraBase
    {
        protected override int DustType => 57;
        protected override int Debuff => BuffID.Ichor;
        protected override int DebuffTicks => 60;
    }

    /// <summary>
    /// Décharge galactique de la Devastation (invisible, poussière) : file vers sa cible et explose (140 px) quand un ennemi
    /// est à moins de 60 px ; ai[1] = hauteur sous laquelle elle heurte les blocs (-2 : tirée de la lame).
    /// </summary>
    public abstract class GalaxyBlastBase : ModProjectile
    {
        protected abstract int DustType { get; }
        protected abstract int Debuff { get; }
        protected abstract int DebuffTicks { get; }
        protected virtual int Penetrate => 1;

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private bool Exploding => Projectile.ai[1] == -1f;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.penetrate = Penetrate;
            Projectile.extraUpdates = 5;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void AI()
        {
            if (Projectile.ai[1] == -2f || Projectile.ai[1] >= 0f && Projectile.position.Y > Projectile.ai[1])
                Projectile.tileCollide = true;
            Dust dust = Dust.NewDustPerfect(Projectile.Center, DustType, Vector2.Zero);
            dust.noGravity = true;
            if (Exploding)
            {
                Projectile.ai[0]++;
                Projectile.velocity = Vector2.Zero;
                Projectile.tileCollide = false;
                Projectile.penetrate = -1;
                Projectile.position = Projectile.Center;
                Projectile.width = Projectile.height = 140;
                Projectile.Center = Projectile.position;
                if (Projectile.ai[0] >= Projectile.MaxUpdates * 3)
                    Projectile.Kill();
                return;
            }
            if (Projectile.numUpdates != 0)
                return;
            foreach (var npc in Main.ActiveNPCs)
            {
                if (npc.CanBeChasedBy(Projectile) && Projectile.Distance(npc.Center) < 60f
                    && Collision.CanHitLine(Projectile.Center, 0, 0, npc.Center, 0, 0))
                {
                    Projectile.ai[0] = 0f;
                    Projectile.ai[1] = -1f;
                    Projectile.netUpdate = true;
                    break;
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            for (int i = 0; i < 8; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType, 0f, 0f, 0, default, 2.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 3f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(Debuff, DebuffTicks);
    }

    public class GalaxyBlast : GalaxyBlastBase
    {
        protected override int DustType => 73;
        protected override int Debuff => BuffID.OnFire;
        protected override int DebuffTicks => 180;
        protected override int Penetrate => -1;
    }

    public class GalaxyBlastType2 : GalaxyBlastBase
    {
        protected override int DustType => 56;
        protected override int Debuff => BuffID.Frostburn;
        protected override int DebuffTicks => 120;
    }

    public class GalaxyBlastType3 : GalaxyBlastBase
    {
        protected override int DustType => 57;
        protected override int Debuff => BuffID.Ichor;
        protected override int DebuffTicks => 60;
    }
}
