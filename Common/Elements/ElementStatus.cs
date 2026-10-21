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
            // Affiché au-dessus des boss avec les debuffs de Calamity (option « Debuff Display » de Calamity)
            int type = Type;
            CalamityMod.NPCs.CalamityGlobalNPC.moddedDebuffTextureList.Add((Texture, npc => npc.HasBuff(type)));
        }

        public override void Unload() =>
            CalamityMod.NPCs.CalamityGlobalNPC.moddedDebuffTextureList.RemoveAll(entry => entry.Item1 == Texture);

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (ElementInfo.Effects[(int)Element].Confuse)
                npc.confused = true;
        }
    }

    public class Chilled : ElementStatus { public override Element Element => Element.Cold; }
    public class Ablaze : ElementStatus { public override Element Element => Element.Fire; }
    public class Drowning : ElementStatus { public override Element Element => Element.Water; }
    public class Envenomed : ElementStatus { public override Element Element => Element.Poison; }
    public class Corroded : ElementStatus { public override Element Element => Element.Acid; }
    public class Irradiated : ElementStatus { public override Element Element => Element.Radioactive; }
    public class Shocked : ElementStatus { public override Element Element => Element.Electricity; }
    public class Punctured : ElementStatus { public override Element Element => Element.Stab; }
    public class Lacerated : ElementStatus { public override Element Element => Element.Slash; }
    public class Discombobulated : ElementStatus { public override Element Element => Element.Smash; }
    public class Hexed : ElementStatus { public override Element Element => Element.Magic; }
    public class Incapacitated : ElementStatus { public override Element Element => Element.Dark; }
    public class Smitten : ElementStatus { public override Element Element => Element.Holy; }
    public class Cursed : ElementStatus { public override Element Element => Element.Unholy; }
    public class Malfunctioning : ElementStatus { public override Element Element => Element.Machine; }
}
