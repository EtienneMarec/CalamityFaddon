using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityFandom.Common.WikiData
{
    /// <summary>
    /// Remplace des textures de Calamity par les sprites du wiki : toute image
    /// Assets/Overrides/{Buffs|Items|NPCs|Projectiles}/&lt;ClasseCalamity&gt;.png est appliquée au chargement (client seulement)
    /// et l'originale est restaurée au déchargement. Ajouter une image suffit, sans code.
    /// </summary>
    public class TextureOverrides : ModSystem
    {
        private const string Root = "Assets/Overrides/";
        private readonly List<(Asset<Texture2D>[] array, int index, Asset<Texture2D> original)> replaced = new();

        public static int Applied { get; private set; }

        public override void PostSetupContent()
        {
            Applied = 0;
            if (Main.dedServ)
                return;
            foreach (string file in Mod.GetFileNames())
            {
                if (!file.StartsWith(Root) || !(file.EndsWith(".rawimg") || file.EndsWith(".png")))
                    continue;
                string[] parts = file[Root.Length..].Split('/');
                if (parts.Length != 2)
                    continue;
                string kind = parts[0], cls = System.IO.Path.GetFileNameWithoutExtension(parts[1]);
                (Asset<Texture2D>[] array, int index) target = kind switch
                {
                    "Buffs" when ModContent.TryFind<ModBuff>("CalamityMod", cls, out var b) => (TextureAssets.Buff, b.Type),
                    "Items" when ModContent.TryFind<ModItem>("CalamityMod", cls, out var i) => (TextureAssets.Item, i.Type),
                    "NPCs" when ModContent.TryFind<ModNPC>("CalamityMod", cls, out var n) => (TextureAssets.Npc, n.Type),
                    "Projectiles" when ModContent.TryFind<ModProjectile>("CalamityMod", cls, out var p) => (TextureAssets.Projectile, p.Type),
                    _ => (null, -1),
                };
                if (target.array == null)
                {
                    Mod.Logger.Warn($"Texture de remplacement sans cible dans Calamity : {file}");
                    continue;
                }
                replaced.Add((target.array, target.index, target.array[target.index]));
                target.array[target.index] = ModContent.Request<Texture2D>($"CalamityFandom/{Root}{kind}/{cls}");
                Applied++;
            }
        }

        public override void Unload()
        {
            foreach (var (array, index, original) in replaced)
                if (index < array.Length)
                    array[index] = original;
            replaced.Clear();
        }
    }
}
