using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Debug
{
    /// <summary>
    /// Mannequin de test : énorme réserve de PV, immobile, inoffensif, neutre face à tous les éléments. Il se régénère
    /// avant de mourir et affiche les debuffs de Calamity au-dessus de lui comme un boss. À poser avec FandomDummySpawner.
    /// </summary>
    public class FandomTestDummy : ModNPC
    {
        public const int Life = 1_000_000;

        public override string Texture => $"Terraria/Images/NPC_{NPCID.TargetDummy}";

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.TargetDummy];
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = new NPCID.Sets.NPCBestiaryDrawModifiers { Hide = true };
            CalamityNPCSets.ForceDrawDebuffDisplay[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 36;
            NPC.height = 96;
            NPC.lifeMax = Life;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit15;
            NPC.value = 0f;
            NPC.npcSlots = 0f;
        }

        // Pas de mise à l'échelle Expert/Revengeance : toujours 1 million de PV
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment) => NPC.lifeMax = Life;

        public override void AI()
        {
            NPC.velocity = Vector2.Zero;
            NPC.timeLeft = 2;
            // Se régénère avant de mourir : c'est un sac de PV, pas une cible à abattre
            if (NPC.life < Life / 10)
            {
                NPC.life = NPC.lifeMax;
                NPC.HealEffect(NPC.lifeMax);
                NPC.netUpdate = true;
            }
        }

        public override bool CheckActive() => false;

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

        // Pas de « true » forcé ici : il passerait outre l'immunité après un coup (un estoc touchait 4 fois par passage)

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            // Image vanilla étirée sur la hitbox, posée au sol : l'agrandissement par NPC.scale la laissait deux fois
            // plus petite que la hitbox
            Texture2D texture = TextureAssets.Npc[Type].Value;
            Rectangle frame = NPC.frame.Height > 0 ? NPC.frame : texture.Frame(1, Main.npcFrameCount[Type]);
            float scale = NPC.height / (float)frame.Height;
            Vector2 origin = new(frame.Width / 2f, frame.Height);
            spriteBatch.Draw(texture, NPC.Bottom - screenPos + new Vector2(0f, NPC.gfxOffY), frame, NPC.GetAlpha(drawColor), 0f, origin,
                scale, NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
            return false;
        }
    }

    /// <summary>Objet de debug : clic gauche = pose un mannequin au curseur, clic droit = retire tous les mannequins.</summary>
    public class FandomDummySpawner : ModItem
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.TargetDummy}";

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 36;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useAnimation = Item.useTime = 20;
            Item.rare = ItemRarityID.Quest;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool? UseItem(Player player)
        {
            // Création/suppression de PNJ : seulement là où le monde est simulé (solo ou serveur)
            if (player.whoAmI != Main.myPlayer || Main.netMode == NetmodeID.MultiplayerClient)
                return true;
            int type = ModContent.NPCType<FandomTestDummy>();
            if (player.altFunctionUse == 2)
            {
                foreach (var npc in Main.ActiveNPCs)
                    if (npc.type == type)
                        npc.active = false;
                return true;
            }
            NPC.NewNPC(player.GetSource_ItemUse(Item), (int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, type);
            return true;
        }
    }
}
