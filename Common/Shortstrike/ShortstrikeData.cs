using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using CalamityFandom.Common.Config;
using CalamityFandom.Common.WikiData;
using CalamityMod.Projectiles.BaseProjectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Common.Shortstrike
{
    /// <summary>
    /// Wiki : Shortstrike.txt — données du Shortstrike (WikiData/shortstrike.json, généré par tools/gen_shortstrike.py) :
    /// règles de la jauge (Balance/shortstrike.json en Calamity++) et Shortflurry de chaque épée courte.
    /// Une épée courte = une arme du tableau de la page, ou toute arme maniée en estoc (useStyle Rapier, projectile
    /// d'épée courte de Calamity) : celles-là ont le Shortflurry générique.
    /// </summary>
    public class ShortstrikeData : ModSystem
    {
        public class Rules
        {
            public int MaxPoints { get; set; } = 100;
            public int DamagePerPoint { get; set; } = 10;
            public int PointPerHitSwing { get; set; } = 1;
            public int PointsPerDamagePercent { get; set; } = 5;
            public int PointsPerDefense { get; set; } = 3;
        }

        public class Effect
        {
            public string Type { get; set; }
            public float Value { get; set; }
            public string Name { get; set; }
            public float? Seconds { get; set; }
            public string Projectile { get; set; }
            /// <summary>Modes où l'effet s'applique (« fandom », « plus ») ; vide = les deux.</summary>
            public List<string> Modes { get; set; }
            /// <summary>Projectiles tirés : nombre, dégâts (part des dégâts du coup), vitesse, ai[0] ; Rain = projectile qui
            /// tombe du ciel quand ils touchent (Galileo Gladius en Full Fandom).</summary>
            public int Count { get; set; } = 1;
            public float DamageMult { get; set; } = 1f;
            public float Speed { get; set; } = 8f;
            public float Spread { get; set; } = 0.15f;
            public float Ai0 { get; set; }
            public string Rain { get; set; }

            /// <summary>Buff ou debuff résolu (Name), projectiles résolus (Projectile, Rain) ; -1 si introuvable.</summary>
            public int BuffType = -1, ProjectileType = -1, RainType = -1;
        }

        public class Attack
        {
            public string Weapon { get; set; }
            public float DamageBoost { get; set; }
            public int Hits { get; set; }
            public float Seconds { get; set; }
            public List<Effect> Effects { get; set; } = new();
            public List<string> Todo { get; set; } = new();
            public List<string> Modes { get; set; }
            public bool OffWiki { get; set; }
        }

        private class DataFile
        {
            public Rules Rules { get; set; } = new();
            public Attack Generic { get; set; } = new();
            public List<Attack> Attacks { get; set; } = new();
        }

        private class BalanceFile
        {
            public Dictionary<string, int> Rules { get; set; } = new();
        }

        public static Rules Current { get; private set; } = new();
        public static Attack Generic { get; private set; }
        /// <summary>Type d'objet -> Shortflurry (armes du tableau présentes en jeu).</summary>
        public static Dictionary<int, Attack> ByItem { get; } = new();
        public static List<string> MissingWeapons { get; } = new();
        public static List<string> MissingNames { get; } = new();

        private static DataFile data;

        public override void Load()
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            data = JsonSerializer.Deserialize<DataFile>(Mod.GetFileBytes("WikiData/shortstrike.json"), options);
            Current = data.Rules;
            Generic = data.Generic;
            if (FandomMode.IsPlus && Mod.FileExists("Balance/shortstrike.json"))
            {
                var balance = JsonSerializer.Deserialize<BalanceFile>(Mod.GetFileBytes("Balance/shortstrike.json"), options);
                foreach (var (key, value) in balance.Rules)
                {
                    var property = typeof(Rules).GetProperty(key, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    property?.SetValue(Current, value);
                }
            }
        }

        public override void Unload()
        {
            data = null;
            Generic = null;
            Current = new();
            ByItem.Clear();
            MissingWeapons.Clear();
            MissingNames.Clear();
        }

        // Les noms traduits ne sont prêts qu'après les recettes (comme le reste des données du wiki)
        public override void PostAddRecipes()
        {
            foreach (var attack in data.Attacks)
            {
                if (!FandomMode.Applies(attack.Modes))
                    continue;
                int item = WikiRecipes.ResolveItem(attack.Weapon);
                if (item <= 0)
                {
                    MissingWeapons.Add(attack.Weapon);
                    continue;
                }
                // Effets d'un seul mode (Galileo Gladius, Cosmic Shiv : version 2.0.1.005 en Full Fandom)
                attack.Effects = attack.Effects.Where(e => FandomMode.Applies(e.Modes)).ToList();
                foreach (var effect in attack.Effects)
                {
                    if (effect.Rain != null && (effect.RainType = ResolveProjectile(effect.Rain)) < 0)
                        MissingNames.Add($"{attack.Weapon} : projectile {effect.Rain}");
                    if (effect.Name != null && (effect.BuffType = ResolveBuff(effect.Name)) < 0)
                        MissingNames.Add($"{attack.Weapon} : buff {effect.Name}");
                    if (effect.Projectile != null && (effect.ProjectileType = ResolveProjectile(effect.Projectile)) < 0)
                        MissingNames.Add($"{attack.Weapon} : projectile {effect.Projectile}");
                }
                ByItem[item] = attack;
            }
        }

        /// <summary>Buff par nom affiché (vanilla, puis mods), sinon par nom de classe (« Nightwither », renommé Moon Burnt).</summary>
        private static int ResolveBuff(string name)
        {
            string wanted = WikiRecipes.Normalize(name.Replace(" (debuff)", ""));
            for (int type = 1; type < BuffLoader.BuffCount; type++)
                if (WikiRecipes.Normalize(Lang.GetBuffName(type)) == wanted)
                    return type;
            string className = name.Replace(" ", "").Replace("!", "");
            if (ModContent.TryFind<ModBuff>("CalamityMod", className, out var buff))
                return buff.Type;
            return -1;
        }

        /// <summary>Projectile de Calamity par nom de classe ; « CalamityFandom/Nom » pour un des nôtres (portages homonymes).</summary>
        private static int ResolveProjectile(string name)
        {
            if (name.Contains('/'))
                return ModContent.TryFind<ModProjectile>(name, out var own) ? own.Type : -1;
            return ModContent.TryFind<ModProjectile>("CalamityMod", name, out var proj) ? proj.Type : -1;
        }

        /// <summary>L'objet se manie-t-il comme une épée courte ?</summary>
        public static bool IsShortsword(Item item)
        {
            if (item == null || item.IsAir || item.damage <= 0)
                return false;
            if (ByItem.ContainsKey(item.type) || item.useStyle == ItemUseStyleID.Rapier)
                return true;
            return item.shoot > ProjectileID.None && ContentSamples.ProjectilesByType.TryGetValue(item.shoot, out var proj)
                && proj.ModProjectile is BaseShortswordProjectile;
        }

        /// <summary>Shortflurry de l'arme : la sienne (tableau du wiki) ou le générique.</summary>
        public static Attack For(Item item) => ByItem.TryGetValue(item.type, out var attack) ? attack : Generic;
    }
}
