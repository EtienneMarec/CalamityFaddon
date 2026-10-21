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
        }
    }
}
