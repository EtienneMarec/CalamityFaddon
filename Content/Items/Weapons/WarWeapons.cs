using CalamityFandom.Content.Legacy.Buffs.StatDebuffs;
using CalamityMod;
using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

// Ensemble « War » du wiki (modèle {{War}}) : armes lâchées par l'armée gobeline, qui infligent War Cleave (porté avec
// la Waraxe et la Warblade, Content/Legacy/). Stats des infobox ; comportements de la prose des pages.

namespace CalamityFandom.Content.Items.Ammo
{
    /// <summary>Wiki : War Arrow.txt — flèche qui inflige War Cleave 4 s (Goblin Archer).</summary>
    public class WarArrow : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 99;

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 36;
            Item.damage = 9;
            // [choix] L'infobox dit « spy » : une flèche tirée par un arc fait des dégâts à distance
            Item.DamageType = DamageClass.Ranged;
            Item.knockBack = 1.5f;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.ammo = AmmoID.Arrow;
            Item.shoot = ModContent.ProjectileType<Projectiles.Ranged.WarArrowProj>();
            Item.shootSpeed = 13f;
            Item.value = 1; // 1,5 cuivre à la vente sur le wiki : arrondi
            Item.rare = ItemRarityID.Blue;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Ranged
{
    /// <summary>Wiki : Warbow.txt — « Converts wooden arrows into war arrows which crumble enemy defenses » (War Cleave 10 s).</summary>
    public class Warbow : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 56;
            Item.damage = 26;
            Item.DamageType = DamageClass.Ranged;
            Item.crit = 19 - 4;
            Item.knockBack = 6f;
            Item.useTime = Item.useAnimation = 70;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item5;
            Item.useAmmo = AmmoID.Arrow;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            // « velocity = 1.5 » sur le wiki : vitesse de l'arc, qui s'ajoute à celle de la flèche
            Item.shootSpeed = 1.5f;
            Item.value = Item.sellPrice(silver: 40);
            Item.rare = ItemRarityID.Blue;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (type == ProjectileID.WoodenArrowFriendly)
                type = ModContent.ProjectileType<Projectiles.Ranged.WarArrowProj>();
        }

        public override Vector2? HoldoutOffset() => new Vector2(-4f, 0f);
    }
}

namespace CalamityFandom.Content.Items.Weapons.Magic
{
    /// <summary>Wiki : War Staff.txt — boule d'énergie grise, vitesse moyenne, War Cleave 10 s.</summary>
    public class WarStaff : ModItem
    {
        public override void SetStaticDefaults() => Item.staff[Type] = true;

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 40;
            Item.damage = 12;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.knockBack = 2.5f;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item8;
            Item.shoot = ModContent.ProjectileType<Projectiles.Magic.WarStaffBall>();
            Item.shootSpeed = 11f;
            Item.value = Item.sellPrice(silver: 20);
            Item.rare = ItemRarityID.Blue;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Summon
{
    /// <summary>
    /// Wiki : Warbell.txt — se manie comme une épée (dégâts d'invocation) ; un coup fait sonner une cloche, donne 10 % de
    /// dégâts aux sbires pendant 5 s et inflige War Cleave 5 s. [choix] Le bonus n'a pas d'icône de buff (la page n'en
    /// nomme pas).
    /// </summary>
    public class Warbell : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 42;
            Item.damage = 16;
            Item.DamageType = DamageClass.Summon;
            Item.knockBack = 4f;
            Item.useTime = Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.value = Item.sellPrice(silver: 40);
            Item.rare = ItemRarityID.Blue;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<WarCleave>(), 5 * 60);
            player.GetModPlayer<WarbellPlayer>().Boost = 5 * 60;
            SoundEngine.PlaySound(SoundID.Item35 with { Pitch = 0.2f }, target.Center);
        }
    }

    /// <summary>Bonus de la Warbell : +10 % de dégâts d'invocation pendant 5 s après un coup.</summary>
    public class WarbellPlayer : ModPlayer
    {
        public int Boost;

        public override void PostUpdateEquips()
        {
            if (Boost <= 0)
                return;
            Boost--;
            Player.GetDamage(DamageClass.Summon) += 0.1f;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Rogue
{
    /// <summary>
    /// Wiki : Warglaive.txt — glaive lancée, soumise à la gravité, qui traverse 3 ennemis et disparaît au 4e en infligeant
    /// War Cleave 5 s ; en frappe furtive, elle se plante dans les blocs et reste 10 s ou jusqu'à 4 coups.
    /// </summary>
    public class Warglaive : RogueWeapon
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 99;

        public override void SetDefaults()
        {
            Item.width = Item.height = 32;
            Item.damage = 15;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.knockBack = 2f;
            Item.useTime = Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.consumable = true;
            Item.maxStack = 9999;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<Projectiles.Rogue.WarglaiveProj>();
            Item.shootSpeed = 10f;
            Item.value = Item.sellPrice(copper: 4);
            Item.rare = ItemRarityID.Blue;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (player.Calamity().StealthStrikeAvailable() && p.WithinBounds(Main.maxProjectiles))
                Main.projectile[p].Calamity().stealthStrike = true;
            return false;
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Ranged
{
    /// <summary>War Arrow en vol : War Cleave 4 s (flèche du Warbow : 10 s, ai[2] = 1).</summary>
    public class WarArrowProj : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Ammo/WarArrow";

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.WoodenArrowFriendly);
            AIType = ProjectileID.WoodenArrowFriendly;
            Projectile.width = Projectile.height = 10;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse_WithAmmo use && use.Item.type == ModContent.ItemType<Items.Weapons.Ranged.Warbow>())
                Projectile.ai[2] = 1f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) =>
            target.AddBuff(ModContent.BuffType<WarCleave>(), Projectile.ai[2] == 1f ? 10 * 60 : 4 * 60);

        // Le sprite pointe vers le bas : tourné dans le sens du vol
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor),
                Projectile.velocity.ToRotation() - MathHelper.PiOver2, texture.Size() / 2f, 1f, SpriteEffects.None);
            return false;
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Magic
{
    /// <summary>Boule d'énergie grise du War Staff (traînée de poussière grise, pas de sprite sur le wiki).</summary>
    public class WarStaffBall : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.ShadowBeamFriendly;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 180;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            for (int i = 0; i < 2; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Smoke, Projectile.velocity * 0.1f, 80, Color.LightGray, 1.3f);
                dust.noGravity = true;
            }
            Lighting.AddLight(Projectile.Center, 0.25f, 0.25f, 0.28f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<WarCleave>(), 10 * 60);

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 80, Color.LightGray);
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Rogue
{
    /// <summary>Glaive lancée : gravité, 4 coups au plus ; frappe furtive : se plante dans les blocs (10 s ou 4 coups).</summary>
    public class WarglaiveProj : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Rogue/Warglaive";

        private bool Stuck => Projectile.ai[0] == 1f;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = 4;
            Projectile.timeLeft = 600;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            if (Stuck)
            {
                Projectile.velocity = Vector2.Zero;
                return;
            }
            Projectile.rotation += 0.35f * Projectile.direction;
            Projectile.velocity.Y = MathHelper.Min(Projectile.velocity.Y + 0.25f, 16f);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!Projectile.Calamity().stealthStrike)
                return true;
            // Plantée : reste 10 s ou jusqu'à 4 coups
            Projectile.ai[0] = 1f;
            Projectile.timeLeft = 10 * 60;
            Projectile.velocity = Vector2.Zero;
            Projectile.netUpdate = true;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<WarCleave>(), 5 * 60);
    }
}
