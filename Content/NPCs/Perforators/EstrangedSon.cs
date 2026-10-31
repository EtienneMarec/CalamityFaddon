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
    /// Worm, plus rapide et qui frappe plus fort. Tête + 6 segments + queue, vie partagée. [validé 31/10] Sprites réduits
    /// de moitié (ils paraissaient deux fois trop gros) : pixels de 1 au lieu des blocs de 2 habituels.
    /// </summary>
    public abstract class EstrangedSonSegment : ModNPC
    {
        public const int BodyCount = 6;
        /// <summary>Distance entre deux segments (sprites de 27 px de haut, qui se chevauchent).</summary>
        public const float Spacing = 20f;

        public override void SetStaticDefaults() =>
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = new NPCID.Sets.NPCBestiaryDrawModifiers { Hide = this is not EstrangedSonHead };

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
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = TextureAssets.Npc[Type].Value;
            spriteBatch.Draw(tex, NPC.Center - screenPos, null, NPC.GetAlpha(drawColor), NPC.rotation, tex.Size() / 2f, NPC.scale,
                NPC.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
            return false;
        }
    }

    /// <summary>Tête : creuse vers le joueur dans les blocs, retombe hors des blocs (comme le Giant Worm).</summary>
    public class EstrangedSonHead : EstrangedSonSegment
    {
        // Giant Worm : vitesse 6, accélération 0,07 environ ; [choix] Estranged Son un peu plus vif
        private const float MaxSpeed = 8f, Acceleration = 0.1f;

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.lifeMax = 60;
            NPC.damage = 14;
            NPC.defense = 2;
            NPC.value = Item.buyPrice(silver: 1);
        }

        public override void AI()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient && NPC.localAI[0] == 0f)
            {
                NPC.localAI[0] = 1f;
                int previous = NPC.whoAmI;
                for (int i = 0; i <= BodyCount; i++)
                {
                    int type = i < BodyCount ? ModContent.NPCType<EstrangedSonBody>() : ModContent.NPCType<EstrangedSonTail>();
                    int seg = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, type, NPC.whoAmI, ai1: previous);
                    Main.npc[seg].realLife = NPC.whoAmI;
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.SyncNPC, number: seg);
                    previous = seg;
                }
            }

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
    }

    /// <summary>Segment qui suit celui qui le précède (ai[1]).</summary>
    public abstract class EstrangedSonFollower : EstrangedSonSegment
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.lifeMax = 60;
            NPC.dontCountMe = true;
        }

        public override void AI()
        {
            NPC ahead = Main.npc[(int)NPC.ai[1]];
            NPC head = NPC.realLife >= 0 ? Main.npc[NPC.realLife] : null;
            if (!ahead.active || head == null || !head.active || head.type != ModContent.NPCType<EstrangedSonHead>())
            {
                NPC.life = 0;
                NPC.active = false;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                return;
            }
            NPC.target = head.target;
            Vector2 toAhead = ahead.Center - NPC.Center;
            float dist = toAhead.Length();
            NPC.rotation = toAhead.ToRotation() + MathHelper.PiOver2;
            if (dist > 0f)
                NPC.Center += toAhead * ((dist - Spacing) / dist);
            NPC.velocity = Vector2.Zero;
        }

        public override bool CheckActive() => false;
    }

    public class EstrangedSonBody : EstrangedSonFollower
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.damage = 10;
            NPC.defense = 4;
        }
    }

    public class EstrangedSonTail : EstrangedSonFollower
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.damage = 8;
            NPC.defense = 6;
        }
    }
}
