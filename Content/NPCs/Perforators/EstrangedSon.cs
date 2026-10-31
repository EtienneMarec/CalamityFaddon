using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.NPCs.Perforators
{
    /// <summary>
    /// Estranged Son (page Taming, méthode des Perforators : « Each worm will transform into an Estranged Son after around 5
    /// seconds ») : pas de page. [validé 31/10] Resprite du Giant Worm par l'utilisateur ; IA de ver fouisseur du Giant
    /// Worm, plus rapide et qui frappe plus fort. Tête + 6 segments + queue. [validé 31/10] Sprites réduits de moitié
    /// (ils paraissaient deux fois trop gros) : pixels de 1 au lieu des blocs de 2 habituels.
    /// [technique] Un seul PNJ qui dessine lui-même ses segments (positions gardées en mémoire) : avec un PNJ par segment,
    /// 50 Estranged Sons en auraient demandé 400 (le jeu en accepte 200), et les vers suivants n'apparaissaient plus. Les
    /// segments blessent au contact comme la tête (ModifyCollisionData).
    /// </summary>
    public class EstrangedSonHead : ModNPC
    {
        public const int BodyCount = 6;
        /// <summary>Distance entre deux segments (sprites de 27 px de haut, qui se chevauchent).</summary>
        public const float Spacing = 20f;
        // Giant Worm : vitesse 6, accélération 0,07 environ ; [choix] Estranged Son un peu plus vif
        private const float MaxSpeed = 8f, Acceleration = 0.1f;

        /// <summary>Centres des segments, du premier corps à la queue (calculés partout à partir de la tête).</summary>
        private readonly Vector2[] segments = new Vector2[BodyCount + 1];
        private bool placed;

        public override void SetDefaults()
        {
            NPC.width = NPC.height = 16;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.behindTiles = true;
            NPC.knockBackResist = 0f;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.netAlways = true;
            NPC.lifeMax = 60;
            NPC.damage = 14;
            NPC.defense = 2;
            NPC.value = Item.buyPrice(silver: 1);
        }

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();
            Player target = Main.player[NPC.target];
            if (target.dead || !target.active || NPC.Distance(target.Center) > 5000f)
            {
                // Pas de cible : il s'enfonce et disparaît
                NPC.velocity.Y = MathHelper.Min(NPC.velocity.Y + 0.2f, 12f);
                NPC.EncourageDespawn(60);
            }
            else if (InTiles())
            {
                Vector2 desired = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * MaxSpeed;
                NPC.velocity.X = Approach(NPC.velocity.X, desired.X, Acceleration);
                NPC.velocity.Y = Approach(NPC.velocity.Y, desired.Y, Acceleration);
                if (NPC.soundDelay <= 0 && NPC.Distance(target.Center) < 1000f)
                {
                    NPC.soundDelay = (int)MathHelper.Clamp(NPC.Distance(target.Center) / 40f, 10f, 20f);
                    SoundEngine.PlaySound(SoundID.WormDig, NPC.Center);
                }
            }
            else
            {
                // Hors des blocs : il retombe, en gardant un peu d'élan horizontal
                NPC.velocity.Y = MathHelper.Min(NPC.velocity.Y + 0.11f, 12f);
                NPC.velocity.X *= 0.995f;
            }
            if (NPC.velocity.Length() > MaxSpeed * 1.5f)
                NPC.velocity = Vector2.Normalize(NPC.velocity) * MaxSpeed * 1.5f;
            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
            FollowHead();
        }

        /// <summary>Chaque segment suit celui qui le précède, à distance fixe (comme les segments d'un ver).</summary>
        private void FollowHead()
        {
            Vector2 ahead = NPC.Center;
            for (int i = 0; i < segments.Length; i++)
            {
                if (!placed)
                    segments[i] = NPC.Center;
                Vector2 toAhead = ahead - segments[i];
                float dist = toAhead.Length();
                if (dist > Spacing)
                    segments[i] += toAhead * ((dist - Spacing) / dist);
                ahead = segments[i];
            }
            placed = true;
        }

        private static float Approach(float value, float goal, float step) =>
            value < goal ? MathHelper.Min(value + step, goal) : MathHelper.Max(value - step, goal);

        private bool InTiles()
        {
            int x0 = (int)(NPC.position.X / 16f) - 1, x1 = (int)((NPC.position.X + NPC.width) / 16f) + 2;
            int y0 = (int)(NPC.position.Y / 16f) - 1, y1 = (int)((NPC.position.Y + NPC.height) / 16f) + 2;
            for (int x = x0; x < x1; x++)
            {
                for (int y = y0; y < y1; y++)
                {
                    if (!WorldGen.InWorld(x, y))
                        continue;
                    Tile tile = Main.tile[x, y];
                    if (tile.HasUnactuatedTile && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType] && tile.TileFrameY == 0)
                        || tile.LiquidAmount > 64)
                        return true;
                }
            }
            return false;
        }

        // Les segments touchent aussi : la zone de contact devient celle du segment qui touche la cible
        public override bool ModifyCollisionData(Rectangle victimHitbox, ref int immunityCooldownSlot, ref MultipliableFloat damageMultiplier, ref Rectangle npcHitbox)
        {
            if (npcHitbox.Intersects(victimHitbox) || !placed)
                return true;
            for (int i = 0; i < segments.Length; i++)
            {
                var box = new Rectangle((int)segments[i].X - NPC.width / 2, (int)segments[i].Y - NPC.height / 2, NPC.width, NPC.height);
                if (box.Intersects(victimHitbox))
                {
                    npcHitbox = box;
                    damageMultiplier *= i < BodyCount ? 10f / 14f : 8f / 14f; // dégâts des anciens segments : corps 10, queue 8
                    break;
                }
            }
            return true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects flip = NPC.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if (placed)
            {
                Texture2D body = ModContent.Request<Texture2D>("CalamityFandom/Content/NPCs/Perforators/EstrangedSonBody").Value;
                Texture2D tail = ModContent.Request<Texture2D>("CalamityFandom/Content/NPCs/Perforators/EstrangedSonTail").Value;
                // De la queue vers la tête, pour que chaque segment passe sous celui qui le précède
                for (int i = segments.Length - 1; i >= 0; i--)
                {
                    Vector2 ahead = i == 0 ? NPC.Center : segments[i - 1];
                    float rotation = (ahead - segments[i]).ToRotation() + MathHelper.PiOver2;
                    Texture2D tex = i == segments.Length - 1 ? tail : body;
                    Color light = NPC.GetAlpha(Lighting.GetColor(segments[i].ToTileCoordinates()));
                    spriteBatch.Draw(tex, segments[i] - screenPos, null, light, rotation, tex.Size() / 2f, NPC.scale, flip, 0f);
                }
            }
            Texture2D head = TextureAssets.Npc[Type].Value;
            spriteBatch.Draw(head, NPC.Center - screenPos, null, NPC.GetAlpha(drawColor), NPC.rotation, head.Size() / 2f, NPC.scale, flip, 0f);
            return false;
        }
    }
}
