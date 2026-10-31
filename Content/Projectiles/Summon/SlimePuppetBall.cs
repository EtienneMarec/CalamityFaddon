using System.Collections.Generic;
using CalamityFandom.Common.Config;
using CalamityMod.Items.Weapons.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Projectiles.Summon
{
    /// <summary>
    /// Rework de la page « Slime Puppet Staff » (objet de Calamity) en Full Fandom ([choix] 31/10, Calamity++ garde l'arme de Calamity) : « Summons a
    /// slime ball that follows your cursor. Slime balls will connect to eachother when close ». Bouton maintenu : la boule
    /// suit le curseur (comme le Magic Missile) ; relâché : elle se fige. 10 boules au plus (la plus ancienne disparaît),
    /// sans emplacement de sbire. Les boules figées à environ 2,5 blocs l'une de l'autre sont reliées ; boules et liens
    /// ne font pas de dégâts mais arrêtent les ennemis qui marchent (SlimePuppetBarrier).
    /// </summary>
    public class SlimePuppetBall : ModProjectile
    {
        public const int MaxBalls = 10;
        /// <summary>« Approximately 2.5 blocks » entre deux boules (bords), soit environ 3,9 blocs entre centres.</summary>
        public const float LinkDistance = 2.5f * 16f + 22f;
        private const float FollowSpeed = 12f;

        /// <summary>ai[0] : 0 tant qu'elle suit le curseur, 1 une fois figée.</summary>
        public bool Stuck => Projectile.ai[0] == 1f;

        public override void SetStaticDefaults() => Main.projFrames[Type] = 2;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            Projectile.netImportant = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            // La plus ancienne disparaît au-delà de 10
            var mine = new List<Projectile>();
            foreach (var p in Main.ActiveProjectiles)
                if (p.type == Type && p.owner == Projectile.owner && p.whoAmI != Projectile.whoAmI)
                    mine.Add(p);
            if (mine.Count >= MaxBalls)
            {
                mine.Sort((a, b) => a.identity.CompareTo(b.identity));
                mine[0].Kill();
            }
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!owner.active || owner.dead || owner.HeldItem.type != ModContent.ItemType<SlimePuppetStaff>() && !Stuck)
            {
                Projectile.Kill();
                return;
            }
            // Reste tant que le joueur est là ; disparaît s'il change de mode (rechargement) ou s'il s'éloigne beaucoup
            if (FandomMode.IsFull && Projectile.Distance(owner.Center) < 3000f)
                Projectile.timeLeft = 2;
            Projectile.frame = Stuck ? 1 : 0;
            if (Stuck)
            {
                Projectile.velocity = Vector2.Zero;
                return;
            }
            if (Projectile.owner != Main.myPlayer)
                return;
            if (!owner.channel)
            {
                Projectile.ai[0] = 1f;
                Projectile.velocity = Vector2.Zero;
                Projectile.netUpdate = true;
                return;
            }
            Vector2 toMouse = Main.MouseWorld - Projectile.Center;
            Vector2 wanted = toMouse.Length() < FollowSpeed ? toMouse : Vector2.Normalize(toMouse) * FollowSpeed;
            if (Vector2.Distance(Projectile.velocity, wanted) > 0.5f)
                Projectile.netUpdate = true;
            Projectile.velocity = wanted;
        }

        /// <summary>Liens entre les boules figées d'un même joueur.</summary>
        public static IEnumerable<(Vector2 a, Vector2 b)> Links()
        {
            int type = ModContent.ProjectileType<SlimePuppetBall>();
            var balls = new List<Projectile>();
            // (méthode itérateur : pas de Main.ActiveProjectiles ici)
            for (int k = 0; k < Main.maxProjectiles; k++)
                if (Main.projectile[k].active && Main.projectile[k].type == type && Main.projectile[k].ai[0] == 1f)
                    balls.Add(Main.projectile[k]);
            for (int i = 0; i < balls.Count; i++)
                for (int j = i + 1; j < balls.Count; j++)
                    if (balls[i].owner == balls[j].owner && balls[i].Distance(balls[j].Center) <= LinkDistance)
                        yield return (balls[i].Center, balls[j].Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            // Les liens sont dessinés par la boule la plus ancienne de chaque paire (une seule fois)
            if (Stuck)
            {
                foreach (var p in Main.ActiveProjectiles)
                {
                    if (p.type != Type || p.ai[0] != 1f || p.owner != Projectile.owner || p.identity <= Projectile.identity
                        || p.Distance(Projectile.Center) > LinkDistance)
                        continue;
                    DrawLink(Projectile.Center, p.Center, lightColor);
                }
            }
            Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame, 0, -2);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, lightColor, 0f, frame.Size() / 2f, 1f, SpriteEffects.None);
            return false;
        }

        /// <summary>Lien : bande de slime rose (couleur du sprite), un peu translucide.</summary>
        private static void DrawLink(Vector2 a, Vector2 b, Color light)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            Vector2 diff = b - a;
            float length = diff.Length();
            Color color = new Color(230, 110, 180).MultiplyRGBA(light) * 0.85f;
            Main.EntitySpriteDraw(pixel, a - Main.screenPosition, new Rectangle(0, 0, 1, 1), color, diff.ToRotation(),
                new Vector2(0f, 0.5f), new Vector2(length, 6f), SpriteEffects.None);
        }
    }

    /// <summary>Le Slime Puppet Staff du wiki (Full Fandom) : pose des boules au lieu d'invoquer les marionnettes de Calamity.</summary>
    public class SlimePuppetStaffFandom : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ModContent.ItemType<SlimePuppetStaff>();

        public override void SetDefaults(Item item)
        {
            if (!FandomMode.IsFull)
                return;
            item.channel = true;
            item.autoReuse = false;
            item.shoot = ModContent.ProjectileType<SlimePuppetBall>();
            item.shootSpeed = 0f;
        }

        public override bool Shoot(Item item, Player player, Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!FandomMode.IsFull)
                return true;
            Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<SlimePuppetBall>(), 0, 0f, player.whoAmI);
            return false;
        }
    }

    /// <summary>
    /// « Slime balls and supports do not deal damage, but will prevent enemies from walking through them » : un ennemi qui
    /// marche (soumis aux blocs, pas un boss) et qui entre dans une boule ou un lien est remis à sa position d'avant.
    /// </summary>
    public class SlimePuppetBarrier : GlobalNPC
    {
        public override void PostAI(NPC npc)
        {
            if (!FandomMode.IsFull || npc.friendly || npc.townNPC || npc.boss || npc.noTileCollide || npc.lifeMax <= 5)
                return;
            int type = ModContent.ProjectileType<SlimePuppetBall>();
            bool blocked = false;
            foreach (var p in Main.ActiveProjectiles)
            {
                if (p.type == type && p.ai[0] == 1f && p.Hitbox.Intersects(npc.Hitbox))
                {
                    blocked = true;
                    break;
                }
            }
            if (!blocked)
            {
                float point = 0f;
                foreach (var (a, b) in SlimePuppetBall.Links())
                {
                    if (Collision.CheckAABBvLineCollision(npc.position, npc.Size, a, b, 6f, ref point))
                    {
                        blocked = true;
                        break;
                    }
                }
            }
            if (!blocked)
                return;
            npc.position.X = npc.oldPosition.X;
            npc.velocity.X = 0f;
        }
    }
}
