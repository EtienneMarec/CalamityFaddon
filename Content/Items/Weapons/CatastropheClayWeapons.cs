using CalamityFandom.Content.Legacy.Projectiles.Melee;
using CalamityMod;
using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

// Famille « Catastrophe » du wiki (31/10) : comme la Catastrophe Claymore (portée), elles tirent un des trois éclairs
// d'énergie explosifs (rouge : On Fire! 2 s, bleu : Frostburn 1,5 s, jaune : Ichor 1 s), dans leur classe de dégâts.

namespace CalamityFandom.Content.Items.Weapons
{
    internal static class CatastropheBolts
    {
        public static int Random() => Main.rand.Next(3) switch
        {
            0 => ModContent.ProjectileType<CalamityAura>(),
            1 => ModContent.ProjectileType<CalamityAuraType2>(),
            _ => ModContent.ProjectileType<CalamityAuraType3>(),
        };
    }
}

namespace CalamityFandom.Content.Items.Weapons.Ranged
{
    /// <summary>Wiki : Catastrophe Clayscore.txt — arc qui tire un éclair d'énergie explosif. [choix] Sans munition (l'infobox n'en donne pas).</summary>
    public class CatastropheClayscore : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 56;
            Item.damage = 54;
            Item.DamageType = DamageClass.Ranged;
            Item.knockBack = 2f;
            Item.useTime = Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item5;
            Item.shoot = ModContent.ProjectileType<CalamityAura>();
            Item.shootSpeed = 14f;
            Item.value = Item.sellPrice(gold: 9, silver: 60);
            Item.rare = ItemRarityID.LightPurple;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) =>
            type = CatastropheBolts.Random();
    }
}

namespace CalamityFandom.Content.Items.Weapons.Magic
{
    /// <summary>Wiki : Catastrophe Clayspore.txt — baguette qui tire un éclair d'énergie explosif.</summary>
    public class CatastropheClayspore : ModItem
    {
        public override void SetStaticDefaults() => Item.staff[Type] = true;

        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 60;
            Item.damage = 68;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 8;
            Item.knockBack = 4f;
            Item.useTime = Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item8;
            Item.shoot = ModContent.ProjectileType<CalamityAura>();
            Item.shootSpeed = 7f;
            Item.value = Item.sellPrice(gold: 9, silver: 60);
            Item.rare = ItemRarityID.LightPurple;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) =>
            type = CatastropheBolts.Random();
    }
}

namespace CalamityFandom.Content.Items.Weapons.Rogue
{
    /// <summary>
    /// Wiki : Catastrophe Claycore.txt — bombe lancée qui explose à l'impact en 3 ou 4 éclairs d'une même couleur ; en
    /// frappe furtive, 4 à 8. [choix] Bombe non consommée (pile de 1 dans l'infobox), soumise à la gravité.
    /// </summary>
    public class CatastropheClaycore : RogueWeapon
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = Item.height = 32;
            Item.damage = 59;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.knockBack = 6f;
            Item.useTime = Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<Projectiles.Rogue.ClaycoreBomb>();
            Item.shootSpeed = 8f;
            Item.value = Item.sellPrice(gold: 9, silver: 60);
            Item.rare = ItemRarityID.LightPurple;
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

namespace CalamityFandom.Content.Projectiles.Rogue
{
    /// <summary>Bombe de la Catastrophe Claycore : explose à l'impact (bloc ou ennemi) en éclairs d'une couleur.</summary>
    public class ClaycoreBomb : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Rogue/CatastropheClaycore";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            Projectile.rotation += 0.2f * Projectile.direction;
            Projectile.velocity.Y = MathHelper.Min(Projectile.velocity.Y + 0.2f, 14f);
        }

        public override void OnKill(int timeLeft)
        {
            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            if (Projectile.owner != Main.myPlayer)
                return;
            int type = Items.Weapons.CatastropheBolts.Random();
            int count = Projectile.Calamity().stealthStrike ? Main.rand.Next(4, 9) : Main.rand.Next(3, 5);
            for (int i = 0; i < count; i++)
            {
                Vector2 velocity = Main.rand.NextVector2CircularEdge(8f, 8f) * Main.rand.NextFloat(0.7f, 1f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, type, Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
            }
        }
    }
}
