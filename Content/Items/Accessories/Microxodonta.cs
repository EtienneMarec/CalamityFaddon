using CalamityMod.Dusts;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Items.Accessories
{
    /// <summary>
    /// Wiki : Microxodonta.txt — covering inventé par le wiki, lâché par le Nuclear Terror (WikiLoot) : +92 % de vitesse de
    /// déplacement ; en vol, une Microbial Cluster toutes les 0,5 s, qui inflige 300 dégâts fixes aux ennemis qui la
    /// touchent. Rareté 13, vente 9 or 60 argent ; éléments par les données du wiki.
    /// [choix] « En vol » = ailes en cours d'utilisation (saut tenu en l'air, temps de vol restant) ; la grappe reprend la
    /// texture et le mouvement du PNJ Microbial Cluster de Calamity et disparaît au bout de 5 s.
    /// </summary>
    public class Microxodonta : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 26;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.value = Item.sellPrice(gold: 9, silver: 60);
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.moveSpeed += 0.92f;
            player.GetModPlayer<MicroxodontaPlayer>().Equipped = Item;
        }
    }

    public class MicroxodontaPlayer : ModPlayer
    {
        public Item Equipped;
        private int timer;

        public override void ResetEffects() => Equipped = null;

        public override void PostUpdateEquips()
        {
            bool flying = Player.wingsLogic > 0 && Player.controlJump && Player.wingTime > 0f && Player.velocity.Y != 0f;
            if (Equipped == null || !flying)
                return;
            if (++timer >= 30)
            {
                timer = 0;
                if (Main.myPlayer == Player.whoAmI)
                    Projectile.NewProjectile(Player.GetSource_Accessory(Equipped), Player.Bottom, Main.rand.NextVector2Circular(2f, 2f),
                        ModContent.ProjectileType<MicroxodontaCluster>(), MicroxodontaCluster.FlatDamage, 0f, Player.whoAmI);
            }
        }
    }

    /// <summary>Microbial Cluster amicale : 300 dégâts fixes, dérive en changeant de direction toutes les 60 ticks.</summary>
    public class MicroxodontaCluster : ModProjectile
    {
        public const int FlatDamage = 300;

        public override string Texture => "CalamityMod/NPCs/SulphurousSea/MicrobialCluster";

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.DamageType = DamageClass.Default;
        }

        // Dégâts fixes : pas de bonus de dégâts, de critique ni de défense
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage *= 0f;
            modifiers.FlatBonusDamage += FlatDamage;
            modifiers.DisableCrit();
            modifiers.DefenseEffectiveness *= 0f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = -oldVelocity;
            return false;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.GreenYellow.ToVector3() * 0.5f);
            if (++Projectile.ai[0] % 60f == 59f)
                Projectile.velocity = Projectile.velocity.SafeNormalize(-Vector2.UnitY).RotatedByRandom(MathHelper.PiOver4) * 4f;
            else
                Projectile.velocity *= 0.98f;
            if (Projectile.ai[0] % 32f == 31f)
            {
                var dust = Dust.NewDustPerfect(Projectile.Center, (int)CalamityDusts.SulphurousSeaAcid);
                dust.velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(1f, 2f);
                dust.noGravity = true;
                dust.scale = 1.6f;
            }
        }
    }
}
