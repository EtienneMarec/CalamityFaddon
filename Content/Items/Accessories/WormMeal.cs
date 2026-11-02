using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Items.Accessories
{
    /// <summary>
    /// Wiki : Worm Meal.txt — quand un sbire touche un ennemi, il lâche 1 ou 2 vers parasites (5 au plus) : ils courent au sol,
    /// sautent haut, s'accrochent à un ennemi et le mordent toutes les 0,66 s (12 dégâts d'invocation, Confused et Slow 2 s),
    /// puis explosent après 2 s de morsure, ou après 5 s sans cible. Recette du wiki (Gullet Worm, 3 Stinkfish).
    /// [choix] Vers dessinés avec le sprite du Gullet Worm (l'image de la page) ; ils comptent les attaques des sbires et
    /// celles de leurs projectiles ; leur « explosion » ne fait pas de dégâts (la page n'en donne pas).
    /// </summary>
    public class WormMeal : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = Item.height = 26;
            Item.accessory = true;
            Item.value = Item.sellPrice(gold: 8, silver: 50);
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual) => player.GetModPlayer<WormMealPlayer>().Active = true;
    }

    public class WormMealPlayer : ModPlayer
    {
        public const int MaxWorms = 5, Damage = 12;
        public bool Active;

        public override void ResetEffects() => Active = false;

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!Active || Player.whoAmI != Main.myPlayer || !(proj.minion || ProjectileID.Sets.MinionShot[proj.type])
                || proj.type == ModContent.ProjectileType<ParasiticWorm>())
                return;
            int worm = ModContent.ProjectileType<ParasiticWorm>();
            int count = Main.rand.Next(1, 3);
            for (int i = 0; i < count && Player.ownedProjectileCounts[worm] < MaxWorms; i++)
            {
                Projectile.NewProjectile(Player.GetSource_Accessory(Player.HeldItem), target.Center, new Vector2(Main.rand.NextFloat(-3f, 3f), -4f), worm,
                    (int)Player.GetTotalDamage(DamageClass.Summon).ApplyTo(Damage), 0f, Player.whoAmI);
                Player.ownedProjectileCounts[worm]++;
            }
        }
    }

    /// <summary>
    /// Ver parasite du Worm Meal : court au sol vers un ennemi (saute haut), s'y accroche et mord toutes les 40 ticks pendant
    /// 2 s, puis explose ; 5 s sans cible : explose aussi. ai[0] = ennemi accroché + 1 ; localAI[0] = temps accroché ou sans cible.
    /// </summary>
    public class ParasiticWorm : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Critters/GulletWorm";

        private const int BiteTicks = 40, LatchTicks = 120, SeekTicks = 300;

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 900;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = BiteTicks;
        }

        private NPC Latched => Projectile.ai[0] > 0 && Main.npc[(int)Projectile.ai[0] - 1].active ? Main.npc[(int)Projectile.ai[0] - 1] : null;

        public override void AI()
        {
            Projectile.localAI[0]++;
            NPC latched = Latched;
            if (latched != null)
            {
                Projectile.tileCollide = false;
                Projectile.Center = latched.Center + new Vector2(0f, latched.height * 0.25f);
                Projectile.velocity = Vector2.Zero;
                if (Projectile.localAI[0] >= LatchTicks)
                    Projectile.Kill();
                return;
            }
            Projectile.tileCollide = true;
            if (Projectile.localAI[0] >= SeekTicks)
            {
                Projectile.Kill();
                return;
            }
            NPC target = Projectile.FindTargetWithinRange(700f);
            Projectile.velocity.Y = MathHelper.Min(Projectile.velocity.Y + 0.4f, 12f);
            if (target != null)
            {
                float dir = System.Math.Sign(target.Center.X - Projectile.Center.X);
                Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, dir * 7f, 0.1f);
                Projectile.spriteDirection = Projectile.direction = (int)dir;
                // « Both high speed and jump height » : saute vers une cible plus haute ou contre un mur
                bool onGround = Projectile.velocity.Y == 0f || Collision.SolidCollision(Projectile.BottomLeft, Projectile.width, 2);
                if (onGround && (target.Center.Y < Projectile.Center.Y - 40f || Collision.SolidCollision(Projectile.position + new Vector2(dir * 8f, 0f), Projectile.width, Projectile.height)))
                    Projectile.velocity.Y = -11f;
            }
            else
                Projectile.velocity.X *= 0.95f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = 0f;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = 0f;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Confused, 120);
            target.AddBuff(BuffID.Slow, 120);
            if (Latched == null)
            {
                Projectile.ai[0] = target.whoAmI + 1;
                Projectile.localAI[0] = 0f;
                Projectile.netUpdate = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);
            for (int i = 0; i < 10; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood);
        }
    }
}
