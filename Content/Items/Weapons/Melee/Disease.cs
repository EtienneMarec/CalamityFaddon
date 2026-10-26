using CalamityMod;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Disease.txt — épée courte inventée par le wiki, lâchée par le Plaguebringer Goliath (WikiLoot) : les ennemis
    /// tués libèrent des Plague Seekers (projectile de Calamity) ; Shortflurry : une Plague Seeker à chaque coup (données de
    /// la page Shortstrike, WikiData/shortstrike.json). Coup automatique. Valeurs du wiki : 56 dégâts, recul 5,5, use 12,
    /// rareté 8, vente 6 or. [choix] 3 Plague Seekers à 60 % des dégâts par ennemi tué (comme l'Epidemic Shredder).
    /// </summary>
    public class Disease : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 38;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.damage = 56;
            Item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Item.useAnimation = Item.useTime = 12;
            Item.shoot = ModContent.ProjectileType<DiseaseProj>();
            Item.shootSpeed = 2f;
            Item.knockBack = 5.5f;
            Item.UseSound = SoundID.Item1;
            Item.value = Item.sellPrice(gold: 6);
            Item.rare = ItemRarityID.Yellow;
        }

        public override bool MeleePrefix() => true;
    }

    public class DiseaseProj : BaseShortswordProjectile
    {
        public override Terraria.Localization.LocalizedText DisplayName => ModContent.GetInstance<Disease>().DisplayName;
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Melee/Disease";

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(16);
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Projectile.ownerHitCheck = true;
            Projectile.timeLeft = 360;
            Projectile.hide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void SetVisualOffsets()
        {
            int halfProjWidth = Projectile.width / 2, halfProjHeight = Projectile.height / 2;
            DrawOriginOffsetX = 0;
            DrawOffsetX = -(30 / 2 - halfProjWidth);
            DrawOriginOffsetY = -(38 / 2 - halfProjHeight);
        }

        public override void ExtraBehavior()
        {
            if (Main.rand.NextBool(5))
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.GreenFairy);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.life > 0 || Projectile.owner != Main.myPlayer)
                return;
            for (int i = 0; i < 3; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Main.rand.NextVector2CircularEdge(4f, 4f),
                    ModContent.ProjectileType<PlagueSeeker>(), (int)(Projectile.damage * 0.6f), Projectile.knockBack * 0.6f, Projectile.owner);
        }
    }
}
