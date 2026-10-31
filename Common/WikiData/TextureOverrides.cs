using System;
using System.Collections.Generic;
using CalamityFandom.Common.Config;
using CalamityMod.NPCs;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Common.WikiData
{
    /// <summary>
    /// Remplace des textures de Calamity par les sprites du wiki : toute image
    /// Assets/Overrides/{Buffs|Items|NPCs|Projectiles}/&lt;ClasseCalamity&gt;.png est appliquée au chargement (client seulement)
    /// et l'originale est restaurée au déchargement. Ajouter une image suffit, sans code.
    /// Images générées par tools/gen_sprites.py : Generated/Items/ (objets renommés, tous les modes) et
    /// Generated/Full/Items/ (sprites refaits par Calamity depuis 2022, Full Fandom seulement) ; nos propres objets reçoivent leur
    /// resprite du wiki dans Generated/Items/ (tous les modes). Une image faite à la main passe avant une image générée.
    /// Objets animés : une image générée ne remplace la planche que si l'objet est listé dans Generated/static.json (le
    /// wiki montre un autre dessin, décision du 31/10) ; l'animation est alors coupée (rétablie au déchargement). Les
    /// aliments (« food ») gardent leurs 3 images : leur planche générée en a déjà 3.
    /// </summary>
    public class TextureOverrides : ModSystem
    {
        private const string Root = "Assets/Overrides/";
        private readonly List<(Asset<Texture2D>[] array, int index, Asset<Texture2D> original)> replaced = new();
        private readonly List<(int type, DrawAnimation animation, bool soul)> stopped = new();

        // Objets de Calamity qui se dessinent eux-mêmes dans l'inventaire (décalage et échelle réglés pour leur sprite :
        // Abyss Bar +5/-12 px...) : quand on leur a donné l'image du wiki, le jeu de base la dessine à la place, centrée
        private delegate bool PreDrawInventoryOrig(ModItem self, SpriteBatch spriteBatch, Microsoft.Xna.Framework.Vector2 position,
            Microsoft.Xna.Framework.Rectangle frame, Microsoft.Xna.Framework.Color drawColor, Microsoft.Xna.Framework.Color itemColor,
            Microsoft.Xna.Framework.Vector2 origin, float scale);
        private delegate bool PreDrawInventoryHook(PreDrawInventoryOrig orig, ModItem self, SpriteBatch spriteBatch,
            Microsoft.Xna.Framework.Vector2 position, Microsoft.Xna.Framework.Rectangle frame, Microsoft.Xna.Framework.Color drawColor,
            Microsoft.Xna.Framework.Color itemColor, Microsoft.Xna.Framework.Vector2 origin, float scale);
        private static readonly HashSet<int> vanillaInventoryDraw = new();
        private readonly List<MonoMod.RuntimeDetour.Hook> drawHooks = new();

        private static bool SkipOwnInventoryDraw(PreDrawInventoryOrig orig, ModItem self, SpriteBatch spriteBatch,
            Microsoft.Xna.Framework.Vector2 position, Microsoft.Xna.Framework.Rectangle frame, Microsoft.Xna.Framework.Color drawColor,
            Microsoft.Xna.Framework.Color itemColor, Microsoft.Xna.Framework.Vector2 origin, float scale) =>
            vanillaInventoryDraw.Contains(self.Type) || orig(self, spriteBatch, position, frame, drawColor, itemColor, origin, scale);

        /// <summary>Le dessin d'inventaire propre à l'objet est sauté (image du wiki dessinée par le jeu, centrée).</summary>
        private void DrawInInventoryAsVanilla(int type)
        {
            var modItem = ItemLoader.GetItem(type);
            var method = modItem?.GetType().GetMethod(nameof(ModItem.PreDrawInInventory));
            if (method == null || method.DeclaringType == typeof(ModItem) || !vanillaInventoryDraw.Add(type))
                return;
            if (drawHooks.Exists(h => h.Source == method))
                return;
            try
            {
                drawHooks.Add(new MonoMod.RuntimeDetour.Hook(method, (PreDrawInventoryHook)SkipOwnInventoryDraw));
            }
            catch (Exception e)
            {
                vanillaInventoryDraw.Remove(type);
                Mod.Logger.Warn($"Dessin d'inventaire de {modItem.Name} non remplacé : {e.Message}");
            }
        }

        private class StaticFile
        {
            public List<string> Static { get; set; } = new();
            /// <summary>Aliments : planche de 3 images refaite avec l'image du wiki, à appliquer telle quelle.</summary>
            public List<string> Food { get; set; } = new();
        }

        public static int Applied { get; private set; }

        public override void PostSetupContent()
        {
            Applied = 0;
            if (Main.dedServ)
                return;
            var staticItems = new HashSet<string>();
            var foodSheets = new HashSet<string>();
            if (Mod.FileExists(Root + "Generated/static.json"))
            {
                var data = System.Text.Json.JsonSerializer.Deserialize<StaticFile>(Mod.GetFileBytes(Root + "Generated/static.json"),
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                staticItems.UnionWith(data.Static);
                foodSheets.UnionWith(data.Food);
            }
            // (type, classe) -> (dossier, priorité) : à la main 2, généré Full Fandom 1, généré 0
            var chosen = new Dictionary<(string kind, string cls), (string folder, int priority)>();
            foreach (string file in Mod.GetFileNames())
            {
                if (!file.StartsWith(Root) || !(file.EndsWith(".rawimg") || file.EndsWith(".png")))
                    continue;
                string[] parts = file[Root.Length..].Split('/');
                (string folder, int priority) source = parts.Length switch
                {
                    2 => (parts[0], 2),
                    3 when parts[0] == "Generated" => ("Generated/" + parts[1], 0),
                    4 when parts[0] == "Generated" && parts[1] == "Full" && FandomMode.IsFull => ("Generated/Full/" + parts[2], 1),
                    _ => (null, -1),
                };
                if (source.folder == null)
                    continue;
                var key = (parts[^2], System.IO.Path.GetFileNameWithoutExtension(parts[^1]));
                if (!chosen.TryGetValue(key, out var current) || current.priority < source.priority)
                    chosen[key] = source;
            }

            foreach (var ((kind, cls), (folder, priority)) in chosen)
            {
                (Asset<Texture2D>[] array, int index) target = kind switch
                {
                    "Buffs" when ModContent.TryFind<ModBuff>("CalamityMod", cls, out var b) => (TextureAssets.Buff, b.Type),
                    "Items" when ModContent.TryFind<ModItem>("CalamityMod", cls, out var i) => (TextureAssets.Item, i.Type),
                    "NPCs" when ModContent.TryFind<ModNPC>("CalamityMod", cls, out var n) => (TextureAssets.Npc, n.Type),
                    "Projectiles" when ModContent.TryFind<ModProjectile>("CalamityMod", cls, out var p) => (TextureAssets.Projectile, p.Type),
                    // Objets de ce mod (portages, créations) : resprites du wiki, tous les modes (décision du 30/10)
                    "Items" when Mod.TryFind<ModItem>(cls, out var own) => (TextureAssets.Item, own.Type),
                    _ => (null, -1),
                };
                if (target.array == null)
                {
                    Mod.Logger.Warn($"Texture de remplacement sans cible : {folder}/{cls}");
                    continue;
                }
                // Objet animé (planche d'images) : une image fixe générée casserait l'animation, sauf quand le wiki montre
                // un autre dessin (static.json) : on coupe alors l'animation
                if (priority < 2 && kind == "Items" && Main.itemAnimations[target.index] != null && !foodSheets.Contains(cls))
                {
                    if (!staticItems.Contains(cls))
                        continue;
                    stopped.Add((target.index, Main.itemAnimations[target.index], ItemID.Sets.AnimatesAsSoul[target.index]));
                    DrawInInventoryAsVanilla(target.index);
                    Main.itemAnimations[target.index] = null;
                    ItemID.Sets.AnimatesAsSoul[target.index] = false;
                }
                string path = $"CalamityFandom/{Root}{folder}/{cls}";
                replaced.Add((target.array, target.index, target.array[target.index]));
                target.array[target.index] = ModContent.Request<Texture2D>(path);
                if (kind == "Buffs")
                    ReplaceInDebuffDisplay(ModContent.Find<ModBuff>("CalamityMod", cls).Texture, path);
                Applied++;
            }
        }

        /// <summary>
        /// Calamity dessine les debuffs au-dessus des boss avec sa propre liste de chemins de textures
        /// (CalamityGlobalNPC.moddedDebuffTextureList), sans passer par TextureAssets.Buff : on y remplace aussi le chemin.
        /// </summary>
        private void ReplaceInDebuffDisplay(string originalPath, string newPath)
        {
            var list = CalamityGlobalNPC.moddedDebuffTextureList;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Item1 == originalPath)
                {
                    displayReplaced.Add((i, list[i]));
                    list[i] = (newPath, list[i].Item2);
                }
            }
        }

        private readonly List<(int index, (string, Predicate<NPC>) original)> displayReplaced = new();

        public override void Unload()
        {
            foreach (var (array, index, original) in replaced)
                if (index < array.Length)
                    array[index] = original;
            replaced.Clear();
            foreach (var (type, animation, soul) in stopped)
            {
                if (type < Main.itemAnimations.Length)
                    Main.itemAnimations[type] = animation;
                if (type < ItemID.Sets.AnimatesAsSoul.Length)
                    ItemID.Sets.AnimatesAsSoul[type] = soul;
            }
            stopped.Clear();
            foreach (var hook in drawHooks)
                hook.Dispose();
            drawHooks.Clear();
            vanillaInventoryDraw.Clear();
            var list = CalamityGlobalNPC.moddedDebuffTextureList;
            foreach (var (index, original) in displayReplaced)
                if (index < list.Count)
                    list[index] = original;
            displayReplaced.Clear();
        }
    }
}
