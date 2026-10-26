// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.Weapons.Ranged
{
    /// <summary>
    /// Wiki : Disseminator.txt — fusil à pompe, amélioration de la Conference Call : trois salves, une de l'arme, une
    /// tombée d'au-dessus et une montée d'en dessous, toutes deux vers le curseur, qui traversent les blocs et sont moins
    /// précises (texte et gif de la page ; le code de 2.0.1.005 visait jusqu'à 8 ennemis, ce que le wiki ne décrit pas).
    /// 50 % de chances de ne pas consommer de munition. Valeurs du wiki : 52 dégâts, recul 4,5, vitesse 23, vélocité 13,
    /// rareté 10, vente 20 or. Retiré de Calamity après 2.0.1.005 ; recette par les données du wiki.
    /// </summary>
    public class Disseminator : ModItem
    {
        /// <summary>Plombs par salve et part des dégâts des salves d'au-dessus et d'en dessous (code de 2.0.1.005).</summary>
        private const int BulletsPerBlast = 4;
        private const float ExtraDamage = 0.7f;

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 52;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 66;
            Item.height = 24;
            Item.useTime = Item.useAnimation = 23;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4.5f;
            Item.value = Item.sellPrice(gold: 20);
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item38;
            Item.autoReuse = true;
            Item.shootSpeed = 13f;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.NextBool();

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < BulletsPerBlast; i++)
            {
                Vector2 spread = velocity + new Vector2(Main.rand.Next(-15, 16), Main.rand.Next(-15, 16)) * 0.05f;
                Main.projectile[Projectile.NewProjectile(source, position, spread, type, damage, knockback, player.whoAmI)].extraUpdates += 2;
            }

            // Salves d'au-dessus et d'en dessous, vers le curseur
            Vector2 cursor = Main.MouseWorld;
            int extraDamage = (int)(damage * ExtraDamage);
            foreach (float side in new[] { -1f, 1f })
            {
                Vector2 origin = new Vector2((cursor.X + player.Center.X) / 2f + Main.rand.Next(-200, 201), player.MountedCenter.Y + side * 600f);
                for (int i = 0; i < BulletsPerBlast; i++)
                {
                    Vector2 aim = Vector2.Normalize(cursor - origin) * Item.shootSpeed
                        + new Vector2(Main.rand.Next(-25, 26), Main.rand.Next(-25, 26)) * 0.05f;
                    var proj = Main.projectile[Projectile.NewProjectile(source, origin, aim, type, extraDamage, knockback, player.whoAmI)];
                    proj.extraUpdates += 2;
                    proj.tileCollide = false;
                    proj.timeLeft /= 2;
                }
            }
            return false;
        }
    }
}
