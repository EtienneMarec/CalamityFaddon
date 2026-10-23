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
    }
}
