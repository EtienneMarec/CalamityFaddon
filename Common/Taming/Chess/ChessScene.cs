using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Common.Taming.Chess
{
    /// <summary>
    /// Partie d'échecs face à un adversaire (Holographic Chessboard) : le Brain of Cthulhu (Taming), ou [validé 31/10]
    /// n'importe quel autre PNJ à moins de 10 blocs, PNJ de la ville compris, hors boss (ça ne fait rien, c'est pour le
    /// plaisir) ; personne à portée : même mise en scène, « Sit at the chessboard and play with yourself » à la place de
    /// l'adversaire. Tout s'arrête
    /// pendant la partie (joueurs, ennemis, boss et ses attaques, projectiles). [validé 31/10] Mise en scène, dans
    /// l'ordre : la table, la chaise du joueur puis celle de l'adversaire apparaissent une par une (dessinées par-dessus
    /// le décor) ; le joueur va s'asseoir ; l'adversaire va à sa chaise, reculée selon la largeur de son sprite ; ils se
    /// serrent la main (un Creeper, « œil » du Brain of Cthulhu, vient à la main du joueur ; un autre adversaire
    /// sautille) ; puis le plateau s'ouvre. La table reste jusqu'à la fin du combat contre un boss, et disparaît à la
    /// fin de la partie contre un autre PNJ. [choix] Durées ci-dessous ; tout le monde est figé, aussi en multijoueur.
    /// </summary>
    public class ChessScene : ModSystem
    {
        public const int TableAt = 0, PlayerChairAt = 15, OpponentChairAt = 30;
        public const int PlayerMoveAt = 45, OpponentMoveAt = 105, HandshakeAt = 155, IntroTicks = 215;
        /// <summary>[validé 31/10] La table apparaît à 7 blocs devant le joueur (5 à 10 demandés), posée sur le sol.</summary>
        public const float TableDistance = 7 * 16f;
        /// <summary>Hauteur de l'assise : joueur et adversaire sont posés dessus, à la même hauteur.</summary>
        public const float SeatHeight = 12f;
        /// <summary>Distance entre le centre de la table et la chaise du joueur.</summary>
        public const float ChairOffset = 40f;
        /// <summary>Portée pour un adversaire qui n'est pas un boss : 10 blocs.</summary>
        public const float MobRange = 160f;
        /// <summary>[validé 31/10] Chaise du Brain of Cthulhu reculée de 7,5 blocs en plus : son aura cachait la scène.</summary>
        public const float BrainExtraReach = 120f;

        /// <summary>Position de départ de chaque adversaire (par joueur), là où les PNJ sont gérés.</summary>
        private static readonly Dictionary<int, Vector2> opponentFrom = new();

        /// <summary>Un joueur joue aux échecs face à un adversaire : le monde est figé.</summary>
        public static bool Frozen
        {
            get
            {
                foreach (var p in Main.ActivePlayers)
                    if (!p.dead && p.GetModPlayer<ChessPlayer>().Open)
                        return true;
                return false;
            }
        }

        public override void Load()
        {
            On_NPC.UpdateNPC += (orig, self, i) =>
            {
                if (!Frozen)
                    orig(self, i);
            };
            On_Projectile.Update += (orig, self, i) =>
            {
                if (!Frozen || self.type == ModContent.ProjectileType<ChessTable>())
                    orig(self, i);
            };
        }

        public override void ClearWorld() => opponentFrom.Clear();

        /// <summary>
        /// [validé 31/10] Pas de partie contre un boss avant la fin de son apparition. Calamity donne au Brain of Cthulhu une
        /// animation d'apparition (tremblement, sortie du sol ou du ciel, cri au bout de 4 s, fin vers 7 s ; 2 s de moins
        /// invoqué par objet), dans l'état 0 ou 1 de son IA (ai[0]) : le plateau attend qu'elle soit finie. Sans cette IA :
        /// 2 s depuis son apparition (le cri Roar_0 dure 1,98 s, <see cref="ChessBossAge"/>).
        /// </summary>
        public const int RoarTicks = 120;

        public static bool BossStillRoaring(Player player)
        {
            foreach (var n in Main.ActiveNPCs)
            {
                if (n.type != NPCID.BrainofCthulhu || !n.WithinRange(player.Center, 3000f))
                    continue;
                bool calamityAI = n.TryGetGlobalNPC<CalamityMod.NPCs.CalamityVanillaAIOverrideNPC>(out var ai)
                    && ai.AIOverride is CalamityMod.NPCs.VanillaNPCAIOverrides.Bosses.BrainOfCthulhu.BrainOfCthulhuAI;
                if (calamityAI ? n.ai[0] <= 1f : n.GetGlobalNPC<ChessBossAge>().Age < RoarTicks)
                    return true;
            }
            return false;
        }

        /// <summary>Un PNJ peut jouer aux échecs pour le plaisir (PNJ de la ville compris) : pas un boss ni un morceau de boss du Taming.</summary>
        public static bool CanPlay(NPC npc) =>
            npc.active && !npc.boss && !NPCID.Sets.ProjectileNPC[npc.type] && npc.lifeMax > 5
            && (npc.realLife < 0 || npc.realLife == npc.whoAmI) && TamingSystem.EntryOf(npc.type) == null;

        /// <summary>Recul de la chaise de l'adversaire : la moitié de la largeur de son sprite, plus la moitié de la table.</summary>
        public static float ReachFor(NPC npc)
        {
            float width = npc.frame.Width > 0 ? npc.frame.Width * npc.scale : npc.width;
            return 24f + System.MathF.Max(width, npc.width) / 2f + (npc.type == NPCID.BrainofCthulhu ? BrainExtraReach : 0f);
        }

        /// <summary>
        /// Le joueur local commence la partie face à <paramref name="opponent"/> (appelé par le plateau) ; null : personne,
        /// la chaise d'en face reste vide.
        /// </summary>
        public static void Begin(NPC opponent)
        {
            Player player = Main.LocalPlayer;
            int opponentIndex = opponent?.whoAmI ?? -1;
            float reach = opponent != null ? ReachFor(opponent) : ChairOffset;
            Vector2 table = Ground(player.Bottom + new Vector2(player.direction * TableDistance, 0f));
            int preAge = 0;
            int tableType = ModContent.ProjectileType<ChessTable>();
            foreach (var p in Main.ActiveProjectiles)
            {
                if (p.type != tableType || p.owner != player.whoAmI)
                    continue;
                // Même adversaire : la table est déjà là (on saute la pose des meubles) ; sinon, on la range
                if ((int)p.ai[1] == opponentIndex)
                {
                    table = p.Center;
                    reach = p.ai[0];
                    preAge = PlayerMoveAt;
                }
                else
                    p.Kill();
            }
            if (preAge == 0)
                Projectile.NewProjectile(player.GetSource_Misc("ChessTable"), table, Vector2.Zero, tableType, 0, 0f, player.whoAmI, reach, opponentIndex);
            player.GetModPlayer<ChessPlayer>().From = player.Bottom;
            TamingSystem.ReportChess(true, table, preAge, opponentIndex, reach);
        }

        /// <summary>État d'un joueur (localement, ou reçu du réseau).</summary>
        public static void Set(int who, bool open, Vector2 table, int preAge, int opponent, float reach)
        {
            var chess = Main.player[who].GetModPlayer<ChessPlayer>();
            chess.Open = open;
            chess.Table = table;
            chess.Start = Main.GameUpdateCount - (uint)preAge;
            chess.Opponent = opponent;
            chess.Reach = reach;
            if (open && opponent >= 0 && opponent < Main.maxNPCs)
                opponentFrom[who] = Main.npc[opponent].Center;
        }

        /// <summary>Le plateau s'ouvre quand la mise en scène est finie.</summary>
        public static bool IntroDone(Player player)
        {
            var chess = player.GetModPlayer<ChessPlayer>();
            return !chess.Open || chess.Age >= IntroTicks;
        }

        /// <summary>Où l'adversaire s'assoit : centré sur sa chaise, le bas de son sprite sur l'assise.</summary>
        public static Vector2 OpponentSeat(ChessPlayer chess, NPC npc) =>
            new Vector2(chess.Table.X + chess.Reach, chess.Table.Y - SeatHeight - npc.height / 2f);

        public static Vector2 PlayerSeat(ChessPlayer chess) => chess.Table + new Vector2(-ChairOffset, -SeatHeight);

        /// <summary>
        /// Sol sous un point : premier bloc plein sous de l'air, cherché de 4 blocs au-dessus à 16 en dessous (sinon le
        /// point lui-même : la table peut flotter, ce n'est pas grave).
        /// </summary>
        private static Vector2 Ground(Vector2 point)
        {
            int x = (int)(point.X / 16f);
            bool air = false;
            for (int y = (int)(point.Y / 16f) - 4, end = y + 20; y < end; y++)
            {
                if (!WorldGen.InWorld(x, y, 2))
                    break;
                Tile tile = Main.tile[x, y];
                bool solid = tile.HasTile && !tile.IsActuated && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType]);
                if (solid && air)
                    return new Vector2(point.X, y * 16f);
                air = !solid;
            }
            return point;
        }

        /// <summary>Main du joueur pendant la poignée de main (même calcul partout).</summary>
        public static Vector2 HandPosition(Player player, uint age) =>
            player.MountedCenter + new Vector2(player.direction * 24f, 4f + 5f * System.MathF.Sin(age * 0.5f));

        private static float Smooth(uint age, int from, int to)
        {
            float t = MathHelper.Clamp((age - (float)from) / (to - from), 0f, 1f);
            return t * t * (3f - 2f * t);
        }

        // Déplacement de l'adversaire et poignée de main (les PNJ sont figés : on les place nous-mêmes)
        public override void PostUpdateNPCs()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            foreach (var p in Main.ActivePlayers)
            {
                var chess = p.GetModPlayer<ChessPlayer>();
                uint age = chess.Age;
                if (!chess.Open || age > IntroTicks || chess.Opponent < 0 || chess.Opponent >= Main.maxNPCs)
                    continue;
                NPC opponent = Main.npc[chess.Opponent];
                if (!opponent.active || age < OpponentMoveAt)
                    continue;
                Vector2 from = opponentFrom.TryGetValue(p.whoAmI, out var f) ? f : opponent.Center;
                Vector2 seat = OpponentSeat(chess, opponent);
                bool brain = opponent.type == NPCID.BrainofCthulhu;
                Vector2 before = opponent.Center;
                opponent.Center = Vector2.Lerp(from, seat, Smooth(age, OpponentMoveAt, HandshakeAt));
                if (age >= HandshakeAt && !brain)
                    opponent.Center += new Vector2(0f, 4f * System.MathF.Sin(age * 0.5f));
                opponent.velocity = Vector2.Zero;
                MoveParts(opponent, opponent.Center - before, age);
                if (brain && age >= HandshakeAt)
                    ShakeWithCreeper(p, age);
                if (age % 6 == 0 || age == IntroTicks)
                    opponent.netUpdate = true;
            }
        }

        /// <summary>
        /// [validé 31/10] Ennemis en plusieurs morceaux (Wyvern, vers) : les morceaux (realLife = l'adversaire) suivent la
        /// tête du même déplacement.
        /// </summary>
        private static void MoveParts(NPC opponent, Vector2 delta, uint age)
        {
            foreach (var n in Main.ActiveNPCs)
            {
                if (n.whoAmI == opponent.whoAmI || n.realLife != opponent.whoAmI)
                    continue;
                n.Center += delta;
                n.velocity = Vector2.Zero;
                if (age % 6 == 0 || age == IntroTicks)
                    n.netUpdate = true;
            }
        }

        /// <summary>Le Creeper le plus proche (un œil du Brain of Cthulhu) vient serrer la main du joueur.</summary>
        private static void ShakeWithCreeper(Player player, uint age)
        {
            NPC eye = null;
            foreach (var n in Main.ActiveNPCs)
                if (n.type == NPCID.Creeper && (eye == null || n.DistanceSQ(player.Center) < eye.DistanceSQ(player.Center)))
                    eye = n;
            if (eye == null)
                return;
            eye.Center = HandPosition(player, age) + new Vector2(player.direction * eye.width / 2f, 0f);
            eye.velocity = Vector2.Zero;
            if (age % 6 == 0 || age == IntroTicks)
                eye.netUpdate = true;
        }
    }

    /// <summary>
    /// [validé 31/10] Pas d'apparition d'ennemis pendant une partie : le monde est figé, mais le jeu continuait à en faire
    /// apparaître, et ils arrivaient tous d'un coup à la fin.
    /// </summary>
    public class ChessNoSpawns : GlobalNPC
    {
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            if (ChessScene.Frozen)
                maxSpawns = 0;
        }
    }

    /// <summary>Âge des boss qui jouent aux échecs (compté partout, serveur et clients).</summary>
    public class ChessBossAge : GlobalNPC
    {
        public int Age;

        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.BrainofCthulhu;

        public override void PostAI(NPC npc) => Age++;
    }

    /// <summary>État de la partie d'échecs d'un joueur : contrôles coupés, trajet jusqu'à sa chaise, bras tendu.</summary>
    public class ChessPlayer : ModPlayer
    {
        public bool Open;
        /// <summary>Début de la mise en scène (Main.GameUpdateCount).</summary>
        public uint Start;
        /// <summary>Milieu du bas de la table.</summary>
        public Vector2 Table;
        public int Opponent = -1;
        /// <summary>Distance du centre de la table à la chaise de l'adversaire.</summary>
        public float Reach;
        /// <summary>Où se tenait le joueur (local) avant d'aller s'asseoir.</summary>
        public Vector2 From;

        public uint Age => Main.GameUpdateCount - Start;

        public override void SetControls()
        {
            if (!ChessScene.Frozen)
                return;
            Player.controlLeft = Player.controlRight = Player.controlUp = Player.controlDown = false;
            Player.controlJump = Player.controlUseItem = Player.controlUseTile = Player.controlThrow = false;
            Player.controlHook = Player.controlMount = Player.controlQuickHeal = Player.controlQuickMana = false;
            Player.controlSmart = Player.controlTorch = false;
        }

        public override void PreUpdateMovement()
        {
            if (!ChessScene.Frozen)
                return;
            Player.velocity = Vector2.Zero;
            PlaceOnSeat();
        }

        /// <summary>
        /// Le joueur local va à sa chaise, tourné vers l'adversaire. Appelé avant et après le mouvement du jeu (les
        /// corrections de collision passent entre les deux), sans le décalage de marche (gfxOffY) resté figé.
        /// </summary>
        private void PlaceOnSeat()
        {
            if (!Open || Player.whoAmI != Main.myPlayer)
                return;
            float t = MathHelper.Clamp((Age - (float)ChessScene.PlayerMoveAt) / (ChessScene.OpponentMoveAt - ChessScene.PlayerMoveAt), 0f, 1f);
            Player.Bottom = Vector2.Lerp(From, ChessScene.PlayerSeat(this), t * t * (3f - 2f * t));
            Player.velocity = Vector2.Zero;
            Player.gfxOffY = 0f;
            Player.direction = 1;
        }

        // Pas de coups pendant la partie, sans l'invulnérabilité du jeu (qui fait clignoter le joueur)
        public override bool CanBeHitByNPC(NPC npc, ref int cooldownSlot) => !ChessScene.Frozen;

        public override bool CanBeHitByProjectile(Projectile proj) => !ChessScene.Frozen;

        public override void PostUpdate()
        {
            PlaceOnSeat();
            // Personne en face : le message apparaît au-dessus de la chaise vide, au moment où l'adversaire viendrait
            if (Open && Opponent < 0 && Age == ChessScene.OpponentMoveAt && Player.whoAmI == Main.myPlayer)
            {
                var chair = new Rectangle((int)(Table.X + Reach) - 40, (int)Table.Y - 80, 80, 40);
                CombatText.NewText(chair, Color.Gold, Terraria.Localization.Language.GetTextValue("Mods.CalamityFandom.Taming.Chess.Alone"), true);
            }
            // Poignée de main : le bras avant se tend vers l'œil (Brain of Cthulhu) ou vers l'adversaire
            if (!Open || Opponent < 0 || Age < ChessScene.HandshakeAt || Age > ChessScene.IntroTicks)
                return;
            Vector2 target = ChessScene.HandPosition(Player, Age);
            if (Opponent >= 0 && Opponent < Main.maxNPCs && Main.npc[Opponent].active && Main.npc[Opponent].type != NPCID.BrainofCthulhu)
                target = Main.npc[Opponent].Center;
            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (target - Player.MountedCenter).ToRotation() - MathHelper.PiOver2);
        }
    }

    /// <summary>
    /// Table et chaises en bois de la partie d'échecs (dessin seulement) : ai[0] = recul de la chaise de l'adversaire,
    /// ai[1] = adversaire. Les meubles apparaissent un par un ; retirés à la fin du combat (boss) ou de la partie.
    /// </summary>
    public class ChessTable : ModProjectile
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.WoodenTable;

        private ref float Age => ref Projectile.localAI[0];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hide = true;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Projectile.velocity = Vector2.Zero;
            Age++;
            // Pose de chaque meuble : bruit et poussière de bois
            float x = Age == ChessScene.TableAt + 1 ? 0f : Age == ChessScene.PlayerChairAt + 1 ? -ChessScene.ChairOffset
                : Age == ChessScene.OpponentChairAt + 1 ? Projectile.ai[0] : float.NaN;
            if (!float.IsNaN(x))
            {
                Vector2 spot = Projectile.Center + new Vector2(x, -16f);
                SoundEngine.PlaySound(SoundID.Dig, spot);
                for (int i = 0; i < 8; i++)
                    Dust.NewDust(spot - new Vector2(12f), 24, 24, DustID.WoodFurniture);
            }
            // Gardée tant que la partie dure ; face à un boss, jusqu'à la fin du combat
            int index = (int)Projectile.ai[1];
            Player owner = Main.player[Projectile.owner];
            var chess = owner.GetModPlayer<ChessPlayer>();
            bool playing = owner.active && chess.Open && chess.Opponent == index;
            bool keep = index < 0 ? playing
                : Main.npc[index].active && (Main.npc[index].boss && owner.active || playing);
            if (keep)
                Projectile.timeLeft = 2;
        }

        public override bool? CanDamage() => false;

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles,
            List<int> overPlayers, List<int> overWiresUI) => behindNPCs.Add(index);

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadTiles(TileID.Tables);
            Main.instance.LoadTiles(TileID.Chairs);
            Texture2D table = TextureAssets.Tile[TileID.Tables].Value;
            Texture2D chair = TextureAssets.Tile[TileID.Chairs].Value;
            // Projectile.Center = milieu du bas de la table
            Vector2 floor = Projectile.Center;
            // Table en bois : 3 × 2 cases (rangées de 16 et 18 px, cadres espacés de 18 px)
            if (Age > ChessScene.TableAt)
                for (int x = 0; x < 3; x++)
                    for (int y = 0; y < 2; y++)
                        DrawPiece(table, new Rectangle(x * 18, y * 18, 16, y == 0 ? 16 : 18), floor + new Vector2(-24 + x * 16, -32 + y * 16));
            // Chaises en bois : 1 × 2 cases (16 et 18 px) ; cadre 0 tourné vers la gauche, 18 vers la droite
            for (int y = 0; y < 2; y++)
            {
                if (Age > ChessScene.PlayerChairAt)
                    DrawPiece(chair, new Rectangle(18, y * 18, 16, y == 0 ? 16 : 18), floor + new Vector2(-ChessScene.ChairOffset - 8, -32 + y * 16));
                if (Age > ChessScene.OpponentChairAt)
                    DrawPiece(chair, new Rectangle(0, y * 18, 16, y == 0 ? 16 : 18), floor + new Vector2(Projectile.ai[0] - 8, -32 + y * 16));
            }
            return false;
        }

        private static void DrawPiece(Texture2D texture, Rectangle source, Vector2 topLeft)
        {
            Color light = Lighting.GetColor((topLeft + new Vector2(8f)).ToTileCoordinates());
            Main.EntitySpriteDraw(texture, topLeft - Main.screenPosition, source, light, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }
    }
}
