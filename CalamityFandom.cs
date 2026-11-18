using System.IO;
using CalamityFandom.Common.Taming;
using CalamityFandom.Content.Debug;
using CalamityFandom.Content.Items;
using Terraria.ModLoader;

namespace CalamityFandom
{
    public class CalamityFandom : Mod
    {
        public override void Load()
        {
            // Objets générés depuis les données du wiki (pas de classe C# par objet)
            WikiMaterial.LoadAll(this);
            WikiWings.LoadAll(this);
            // Objets de debug pour les tests en jeu (docs/TESTS.md)
            TestSword.LoadAll(this);
        }

        /// <summary>Paquets hors Taming (numéros hauts pour ne pas croiser ceux du Taming).</summary>
        public enum ExtraPacket : byte { WulfrumSignal = 200 }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            long start = reader.BaseStream.Position;
            if (reader.ReadByte() == (byte)ExtraPacket.WulfrumSignal)
            {
                Content.Tiles.WulfrumSignal.Receive(reader);
                return;
            }
            reader.BaseStream.Position = start;
            TamingSystem.HandlePacket(reader, whoAmI);
        }
    }
}
