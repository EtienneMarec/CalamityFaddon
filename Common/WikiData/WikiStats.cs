using System;
using System.Collections.Generic;
using System.Text.Json;
using CalamityFandom.Common.Config;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ModLoader;

namespace CalamityFandom.Common.WikiData
{
    /// <summary>
    /// Stats des infobox du wiki pour les objets et PNJ de Calamity (WikiData/stats.json, généré par tools/gen_stats.py).
    /// Chargé dans Load pour être disponible avant les premiers SetDefaults.
    /// </summary>
    public class WikiStats : ModSystem
    {
        public class ItemStats
        {
            public int? Damage { get; set; }
            public int? UseAnimation { get; set; }
            public float? KnockBack { get; set; }
            public int? Crit { get; set; }
            public float? ShootSpeed { get; set; }
            public int? Mana { get; set; }
            public int? Defense { get; set; }
            public int? Rare { get; set; }
            public int? Value { get; set; }
        }

        public class NPCStats
        {
            public float? LifeRatio { get; set; }
            public float? DamageRatio { get; set; }
            public int? Defense { get; set; }
        }

        private class StatsFile
        {
            public Dictionary<string, ItemStats> Items { get; set; } = new();
            public Dictionary<string, NPCStats> Npcs { get; set; } = new();
            /// <summary>Écarts extrêmes (> x5), appliqués seulement en mode Full Fandom.</summary>
            public Dictionary<string, ItemStats> FullItems { get; set; } = new();
            public Dictionary<string, NPCStats> FullNpcs { get; set; } = new();
        }

        public static Dictionary<string, ItemStats> Items { get; private set; } = new();
        public static Dictionary<string, NPCStats> NPCs { get; private set; } = new();

        /// <summary>
        /// Couches selon le mode : stats du wiki, puis écarts extrêmes (Full Fandom) ou équilibrage manuel de
        /// Balance/stats.json (Calamity++). Une valeur de la couche supérieure remplace celle du dessous.
        /// </summary>
        public override void Load()
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var file = JsonSerializer.Deserialize<StatsFile>(Mod.GetFileBytes("WikiData/stats.json"), options);
            Items = file.Items;
            NPCs = file.Npcs;
            if (FandomMode.IsFull)
                Overlay(file.FullItems, file.FullNpcs);
            if (FandomMode.IsPlus && Mod.FileExists("Balance/stats.json"))
            {
                var balance = JsonSerializer.Deserialize<StatsFile>(Mod.GetFileBytes("Balance/stats.json"), options);
                Overlay(balance.Items, balance.Npcs);
            }
        }

        private static void Overlay(Dictionary<string, ItemStats> items, Dictionary<string, NPCStats> npcs)
        {
            foreach (var (name, top) in items)
            {
                if (!Items.TryGetValue(name, out var s))
                    Items[name] = s = new ItemStats();
                s.Damage = top.Damage ?? s.Damage;
                s.UseAnimation = top.UseAnimation ?? s.UseAnimation;
                s.KnockBack = top.KnockBack ?? s.KnockBack;
                s.Crit = top.Crit ?? s.Crit;
                s.ShootSpeed = top.ShootSpeed ?? s.ShootSpeed;
                s.Mana = top.Mana ?? s.Mana;
                s.Defense = top.Defense ?? s.Defense;
                s.Rare = top.Rare ?? s.Rare;
                s.Value = top.Value ?? s.Value;
            }
            foreach (var (name, top) in npcs)
            {
                if (!NPCs.TryGetValue(name, out var s))
                    NPCs[name] = s = new NPCStats();
                s.LifeRatio = top.LifeRatio ?? s.LifeRatio;
                s.DamageRatio = top.DamageRatio ?? s.DamageRatio;
                s.Defense = top.Defense ?? s.Defense;
            }
        }

        public override void Unload()
        {
            Items = new();
            NPCs = new();
        }

        /// <summary>Rareté du wiki (numérotation Terraria étendue par Calamity) vers l'identifiant en jeu.</summary>
        public static int ResolveRarity(int wikiRare) => wikiRare switch
        {
            12 => ModContent.RarityType<Turquoise>(),
            13 => ModContent.RarityType<PureGreen>(),
            14 => ModContent.RarityType<CosmicPurple>(),
            15 => ModContent.RarityType<BurnishedAuric>(),
            16 => ModContent.RarityType<HotPink>(),
            17 => ModContent.RarityType<CalamityRed>(),
            _ => Math.Clamp(wikiRare, -1, 11),
        };

        public static void Apply(Item item, ItemStats stats)
        {
            if (stats.Damage is int damage)
                item.damage = damage;
            if (stats.UseAnimation is int useAnimation && useAnimation > 0)
            {
                // useTime suit useAnimation : égal s'il l'était, proportionnel sinon (armes à plusieurs tirs par usage)
                int old = item.useAnimation;
                if (item.useTime == old || old <= 0)
                    item.useTime = useAnimation;
                else
                    item.useTime = Math.Max(1, (int)Math.Round(item.useTime * useAnimation / (float)old));
                item.useAnimation = useAnimation;
            }
            if (stats.KnockBack is float knockBack)
                item.knockBack = knockBack;
            if (stats.Crit is int crit)
                item.crit = crit;
            if (stats.ShootSpeed is float shootSpeed)
                item.shootSpeed = shootSpeed;
            if (stats.Mana is int mana)
                item.mana = mana;
            if (stats.Defense is int defense)
                item.defense = defense;
            if (stats.Rare is int rare)
                item.rare = ResolveRarity(rare);
            if (stats.Value is int value)
                item.value = value;
        }

        public static void Apply(NPC npc, NPCStats stats)
        {
            if (stats.LifeRatio is float lifeRatio)
                npc.lifeMax = Math.Max(1, (int)Math.Round(npc.lifeMax * lifeRatio));
            if (stats.DamageRatio is float damageRatio)
                npc.damage = Math.Max(0, (int)Math.Round(npc.damage * damageRatio));
            if (stats.Defense is int defense)
                npc.defense = defense;
        }
    }

    /// <summary>Applique les stats du wiki aux objets de Calamity qui en ont.</summary>
    public class WikiStatsGlobalItem : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) =>
            lateInstantiation && entity.ModItem is { Mod.Name: "CalamityMod" } && WikiStats.Items.ContainsKey(entity.ModItem.Name);

        public override void SetDefaults(Item item) => WikiStats.Apply(item, WikiStats.Items[item.ModItem.Name]);
    }

    /// <summary>Applique les ratios de vie/dégâts et la défense du wiki aux PNJ de Calamity.</summary>
    public class WikiStatsGlobalNPC : GlobalNPC
    {
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) =>
            lateInstantiation && entity.ModNPC is { Mod.Name: "CalamityMod" } && WikiStats.NPCs.ContainsKey(entity.ModNPC.Name);

        public override void SetDefaults(NPC npc) => WikiStats.Apply(npc, WikiStats.NPCs[npc.ModNPC.Name]);
    }
}
