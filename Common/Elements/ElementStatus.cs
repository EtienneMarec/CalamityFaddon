using Terraria;
using Terraria.ModLoader;

namespace CalamityFandom.Common.Elements
{
    /// <summary>
    /// Statut infligé par une arme élémentaire. Un vrai debuff : synchronisé en multijoueur par le jeu, et les PNJ
    /// résistants à l'élément en sont immunisés (« They are also immune to its effect »).
    /// Les effets sont appliqués par <see cref="ElementGlobalNPC"/>.
    /// </summary>
    public abstract class ElementStatus : ModBuff
    {
        public abstract Element Element { get; }

        public override string Texture => $"CalamityFandom/Assets/Elements/{Element}";

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            ElementData.StatusBuffs[(int)Element] = Type;
            // Volontairement absent de l'affichage de debuffs de Calamity (décision du 21/10 : trop chargé à l'écran) ;
            // le retour visuel passe par les particules et la teinte (ElementGlobalNPC.DrawEffects)
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (ElementInfo.Effects[(int)Element].Confuse)
                npc.confused = true;
        }
    }

    public class Numbed : ElementStatus { public override Element Element => Element.Cold; } // Wiki : Numbed.txt
    public class Ablaze : ElementStatus { public override Element Element => Element.Fire; } // Wiki : Ablaze.txt
    public class Waterlogged : ElementStatus { public override Element Element => Element.Water; } // Wiki : Waterlogged.txt
    public class Envenomed : ElementStatus { public override Element Element => Element.Poison; } // Wiki : Envenomed.txt
    public class Corroded : ElementStatus { public override Element Element => Element.Acid; } // Wiki : Corroded.txt
    public class RadiationSickness : ElementStatus { public override Element Element => Element.Radioactive; } // Wiki : Radiation Sickness.txt
    public class Shocked : ElementStatus { public override Element Element => Element.Electricity; } // Wiki : Shocked.txt
    public class Punctured : ElementStatus { public override Element Element => Element.Stab; } // Wiki : Punctured.txt
    public class Lacerated : ElementStatus { public override Element Element => Element.Slash; } // Wiki : Lacerated.txt
    public class Discombobulated : ElementStatus { public override Element Element => Element.Smash; } // Wiki : Discombobulated.txt
    public class Hexed : ElementStatus { public override Element Element => Element.Magic; } // Wiki : Hexed.txt
    public class Incapacitated : ElementStatus { public override Element Element => Element.Dark; } // Wiki : Incapacitated.txt
    public class Smitten : ElementStatus { public override Element Element => Element.Holy; } // Wiki : Smitten.txt
    public class Damned : ElementStatus { public override Element Element => Element.Unholy; } // Wiki : Damned.txt
    public class Malfunctioning : ElementStatus { public override Element Element => Element.Machine; } // Wiki : Malfunctioning.txt
}
