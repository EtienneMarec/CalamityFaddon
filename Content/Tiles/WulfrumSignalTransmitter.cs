using System.IO;
using CalamityFandom.Content.NPCs.Inventors;
using CalamityMod.Items.DraedonMisc;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.NormalNPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityFandom.Content.Items.Placeables
{
    /// <summary>
    /// Wiki : Wulfrum Signal Transmitter.txt — meuble 2×2 ; clic droit avec un Energy Core : un signal de 30 s qui fait
    /// venir jusqu'à 4 robots Wulfrum toutes les 4 s ; avec une Draedon Power Cell (si pas d'Energy Core) : invoque The
    /// Inventors.
    /// </summary>
    public class WulfrumSignalTransmitter : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.WulfrumSignalTransmitterTile>());
            Item.width = 24;
            Item.height = 22;
            Item.rare = ItemRarityID.Blue;
            Item.value = 0;
        }
    }
}

namespace CalamityFandom.Content.Tiles
{
    public class WulfrumSignalTransmitterTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(194, 255, 67), Language.GetText("Mods.CalamityFandom.Items.WulfrumSignalTransmitter.DisplayName"));
            DustType = DustID.Iron;
            HitSound = SoundID.Tink;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = player.HasItem(ModContent.ItemType<EnergyCore>()) ? ModContent.ItemType<EnergyCore>() : ModContent.ItemType<DraedonPowerCell>();
        }

        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;
            Vector2 pos = new Vector2(i * 16 + 8, j * 16 + 8);
            // L'Energy Core passe avant la Draedon Power Cell (page, Notes)
            if (player.ConsumeItem(ModContent.ItemType<EnergyCore>()))
            {
                SoundEngine.PlaySound(SoundID.Item93, pos);
                WulfrumSignal.Start(pos);
                return true;
            }
            if (!NPC.AnyNPCs(ModContent.NPCType<JohnWulfrum>()) && !NPC.AnyNPCs(ModContent.NPCType<JaneWulfrum>())
                && player.ConsumeItem(ModContent.ItemType<DraedonPowerCell>()))
            {
                SoundEngine.PlaySound(SoundID.Item93, pos);
                SoundEngine.PlaySound(SoundID.Roar, pos);
                if (Main.netMode == NetmodeID.SinglePlayer)
                    NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<JohnWulfrum>());
                else
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: ModContent.NPCType<JohnWulfrum>());
                return true;
            }
            return false;
        }
    }

    /// <summary>Signal Wulfrum : 30 s, jusqu'à 4 robots toutes les 4 s autour du point d'émission (persiste si le meuble est cassé).</summary>
    public class WulfrumSignal : ModSystem
    {
        private static int timeLeft;
        private static Vector2 origin;

        public static void Start(Vector2 pos)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                var p = ModContent.GetInstance<CalamityFandom>().GetPacket();
                p.Write((byte)CalamityFandom.ExtraPacket.WulfrumSignal);
                p.WriteVector2(pos);
                p.Send();
                return;
            }
            origin = pos;
            timeLeft = 30 * 60;
        }

        public override void PostUpdateNPCs()
        {
            if (timeLeft <= 0 || Main.netMode == NetmodeID.MultiplayerClient)
                return;
            timeLeft--;
            if (timeLeft % 240 != 0)
                return;
            int[] types = { ModContent.NPCType<WulfrumDrone>(), ModContent.NPCType<WulfrumHovercraft>(), ModContent.NPCType<WulfrumGyrator>() };
            int count = Main.rand.Next(1, 5);
            for (int i = 0; i < count; i++)
            {
                Vector2 spot = origin + new Vector2(Main.rand.Next(-400, 401), Main.rand.Next(-300, -100));
                NPC.NewNPC(new EntitySource_WorldEvent(), (int)spot.X, (int)spot.Y, types[Main.rand.Next(types.Length)]);
            }
        }

        public static void Receive(BinaryReader reader)
        {
            origin = reader.ReadVector2();
            timeLeft = 30 * 60;
        }
    }
}
