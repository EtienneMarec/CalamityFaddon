using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Items.Weapons.Magic
{
    /// <summary>
    /// Wiki : The Princess.txt — même arme que The Prince de Calamity (boule de feu sacrée qui éclate en 6 flammes à tête
    /// chercheuse), avec les valeurs de sa propre page ; inflige en plus Moon Burnt (Nightwither) : 3 s par la boule,
    /// 1 s par les flammes. [choix] Projectiles de The Prince réutilisés (même comportement que la page).
    /// </summary>
    public class ThePrincess : ModItem
    {
        public override void SetStaticDefaults() => Item.staff[Type] = true;

        public override void SetDefaults()
        {
            Item.width = 102;
            Item.height = 112;
            Item.damage = 166;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 12;
            Item.knockBack = 4.25f;
            Item.useTime = Item.useAnimation = 21;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.UseSound = SoundID.DD2_FlameburstTowerShot;
            Item.shoot = ModContent.ProjectileType<PrinceFlameLarge>();
            Item.shootSpeed = 23.5f;
            Item.value = Item.sellPrice(gold: 24);
            Item.rare = ModContent.RarityType<CalamityMod.Rarities.Turquoise>();
        }
    }

    /// <summary>Flammes tirées par The Princess (et celles qui en sortent) : Moon Burnt en plus des Holy Flames de Calamity.</summary>
    public class PrincessFlames : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        private bool princess;

        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) =>
            entity.type == ModContent.ProjectileType<PrinceFlameLarge>() || entity.type == ModContent.ProjectileType<PrinceFlameSmall>();

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            princess = source is EntitySource_ItemUse use && use.Item.type == ModContent.ItemType<ThePrincess>()
                || source is EntitySource_Parent { Entity: Projectile parent } && parent.TryGetGlobalProjectile<PrincessFlames>(out var p) && p.princess;
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (princess)
                target.AddBuff(ModContent.BuffType<Nightwither>(), projectile.type == ModContent.ProjectileType<PrinceFlameLarge>() ? 180 : 60);
        }
    }
}
