// Adapté de Calamity Mod (Spore Knife de mêlée, retirée en juin 2022 puis refaite en arme rogue), © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
using CalamityMod;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Projectiles.Melee
{
    /// <summary>
    /// Wiki : Spore Knife.txt — estoc de la Spore Knife en Full Fandom (Common/Legacy/FullFandomLegacyWeapons) : l'épée courte
    /// de la page, pas le couteau de lancer de Calamity. Un ennemi touché libère un nuage de spores (Spore Gas vanilla,
    /// moitié des dégâts de base), comme l'ancien code.
    /// </summary>
    public class SporeKnifeShortsword : BaseShortswordProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/SporeKnife";

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
            const int HalfSpriteWidth = 32 / 2;
            const int HalfSpriteHeight = 32 / 2;
            DrawOriginOffsetX = 0;
            DrawOffsetX = -(HalfSpriteWidth - Projectile.width / 2);
            DrawOriginOffsetY = -(HalfSpriteHeight - Projectile.height / 2);
        }

        public override void ExtraBehavior()
        {
            if (Main.rand.NextBool(5))
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Grass);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // « Enemies release spore clouds on hit »
            if (Projectile.owner != Main.myPlayer)
                return;
            Player owner = Main.player[Projectile.owner];
            int sporeDamage = (int)owner.GetTotalDamage<MeleeDamageClass>().ApplyTo(owner.HeldItem.OriginalDamage * 0.5f);
            var spore = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero,
                Main.rand.Next(ProjectileID.SporeGas, ProjectileID.SporeGas3 + 1), sporeDamage, hit.Knockback, Projectile.owner);
            spore.DamageType = DamageClass.Melee;
        }
    }
}
