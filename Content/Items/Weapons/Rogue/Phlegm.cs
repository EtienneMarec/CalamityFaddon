using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Items.Weapons.Rogue
{
    /// <summary>
    /// Wiki : Phlegm.txt — javelot spy inventé par le wiki, lâché par le Plaguebringer Goliath (WikiLoot) : laisse une
    /// traînée de glaires qui inflige Plague (2 s) et Slow aux ennemis en dessous ; tête chercheuse très forte (Notes) ;
    /// attaque furtive : le joueur éternue. Valeurs du wiki : 26 dégâts, recul 2, vélocité 20, use 19, rareté 8, vente
    /// 7 or 20 argent. [choix] Non consommable (pas de pile dans l'infobox) ; l'éternuement projette 6 glaires devant le
    /// joueur ; traînée : une flaque toutes les 6 ticks, 2 s, 30 % des dégâts.
    /// </summary>
    public class Phlegm : RogueWeapon
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 66;
            Item.height = 66;
            Item.damage = 26;
            Item.knockBack = 2f;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = Item.useAnimation = 19;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<PhlegmProj>();
            Item.shootSpeed = 20f;
            Item.value = Item.sellPrice(gold: 7, silver: 20);
            Item.rare = ItemRarityID.Yellow;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (player.Calamity().StealthStrikeAvailable() && p.WithinBounds(Main.maxProjectiles))
            {
                Main.projectile[p].Calamity().stealthStrike = true;
                // Éternuement
                SoundEngine.PlaySound(SoundID.NPCDeath13 with { Pitch = 0.6f }, player.Center);
                for (int i = 0; i < 6; i++)
                    Projectile.NewProjectile(source, player.Center, velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.2f, 0.4f),
                        ModContent.ProjectileType<PhlegmTrail>(), (int)(damage * 0.3f), 0f, player.whoAmI, 1f);
            }
            return false;
        }
    }

    /// <summary>Javelot du Phlegm : tête chercheuse forte, perce une fois, sème des flaques de glaires.</summary>
    public class PhlegmProj : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Rogue/Phlegm";

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 240;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 600f, 20f, 10f);
            if (++Projectile.ai[0] % 6f == 0f && Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<PhlegmTrail>(),
                    (int)(Projectile.damage * 0.3f), 0f, Projectile.owner);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Plague>(), 120);
    }

    /// <summary>Flaque de glaires : immobile (ou projetée si ai[0] = 1, éternuement), 2 s, Plague 2 s et Slow.</summary>
    public class PhlegmTrail : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 1f)
                Projectile.velocity *= 0.95f;
            if (Main.rand.NextBool(2))
            {
                var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GreenMoss, 0f, 0f, 100, default, 1.1f);
                dust.noGravity = true;
                dust.velocity *= 0.2f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 120);
            target.AddBuff(BuffID.Slow, 120);
        }
    }
}
