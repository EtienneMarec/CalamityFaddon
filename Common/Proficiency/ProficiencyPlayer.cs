using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using CalamityMod;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityFandom.Common.Proficiency
{
    /// <summary>Les cinq proficiencies du wiki (page « Proficiency ») ; pas de proficiency pour le lancer ni le sans-classe.</summary>
    public enum ProficiencyClass { Melee, Ranged, Magic, Summon, Spy }

    /// <summary>Tableau des niveaux et règles (WikiData/proficiency.json, généré par tools/gen_proficiency.py).</summary>
    public class ProficiencyData : ModSystem
    {
        public class Rules
        {
            public int MaxValue { get; set; } = 500000;
            public int MinDamage { get; set; } = 5;
            public int CooldownTicks { get; set; } = 120;
            public float MeterDoubleChance { get; set; } = 0.1f;
        }

        public class Level
        {
            public int Required { get; set; }
            public float Damage { get; set; }
            public float Crit { get; set; }
            public float MeleeSpeed { get; set; }
            public float AmmoConsumption { get; set; }
            public float ManaCost { get; set; }
            public float MinionKnockback { get; set; }
            public int MinionSlots { get; set; }
            public float ProjectileSpeed { get; set; }
        }

        private class DataFile
        {
            public Rules Rules { get; set; } = new();
            public List<Level> Levels { get; set; } = new();
        }

        public static Rules Rule { get; private set; } = new();
        public static Level[] Levels { get; private set; } = Array.Empty<Level>();

        public override void Load()
        {
            var data = JsonSerializer.Deserialize<DataFile>(Mod.GetFileBytes("WikiData/proficiency.json"),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Rule = data.Rules;
            Levels = data.Levels.OrderBy(l => l.Required).ToArray();
        }

        public override void Unload()
        {
            Rule = new();
            Levels = Array.Empty<Level>();
        }

        /// <summary>Niveau atteint (0 = aucun) pour une valeur de proficiency.</summary>
        public static int LevelOf(int value)
        {
            int level = 0;
            while (level < Levels.Length && value >= Levels[level].Required)
                level++;
            return level;
        }

        /// <summary>Bonus du niveau (non cumulés : ceux de la ligne atteinte), ou null au niveau 0.</summary>
        public static Level BonusOf(int level) => level > 0 && level <= Levels.Length ? Levels[level - 1] : null;
    }

    /// <summary>
    /// Proficiency du joueur (page « Proficiency », mécanique retirée de Calamity en septembre 2022, décrite par le wiki) :
    /// +1 par coup de plus de 5 dégâts sur un ennemi (ou tout coup sur un boss), sauf ennemis de statue et mannequins ;
    /// 2 s de recharge commune entre deux gains (moitié avec la Level Meter de la classe) ; 10 % de chances de double gain
    /// avec la Level Meter si la valeur n'est pas un multiple de 100. Gardée par personnage, d'un monde à l'autre.
    /// </summary>
    public class ProficiencyPlayer : ModPlayer
    {
        public static readonly int ClassCount = Enum.GetValues<ProficiencyClass>().Length;

        public readonly int[] Value = new int[ClassCount];
        public readonly int[] Level = new int[ClassCount];
        /// <summary>Level Meter de la classe équipée ce tick.</summary>
        public readonly bool[] Meter = new bool[ClassCount];
        private int cooldown;

        private static readonly string[] Keys = { "melee", "ranged", "magic", "summon", "spy" };
        private static readonly Color[] MessageColors = { Color.Orange, Color.GreenYellow, Color.DodgerBlue, Color.Violet, Color.Gold };

        public override void ResetEffects() => Array.Clear(Meter);

        public override void SaveData(TagCompound tag)
        {
            for (int i = 0; i < ClassCount; i++)
                tag["proficiency_" + Keys[i]] = Value[i];
        }

        public override void LoadData(TagCompound tag)
        {
            for (int i = 0; i < ClassCount; i++)
            {
                Value[i] = tag.GetInt("proficiency_" + Keys[i]);
                Level[i] = ProficiencyData.LevelOf(Value[i]);
            }
        }

        public override void PostUpdateMiscEffects()
        {
            if (cooldown > 0)
                cooldown--;
        }

        /// <summary>Classe de proficiency d'un type de dégâts (le rogue est le « Spy » du wiki).</summary>
        public static ProficiencyClass? ClassOf(DamageClass type)
        {
            if (type == null)
                return null;
            if (type.CountsAsClass<RogueDamageClass>())
                return ProficiencyClass.Spy;
            if (type.CountsAsClass(DamageClass.Melee))
                return ProficiencyClass.Melee;
            if (type.CountsAsClass(DamageClass.Ranged))
                return ProficiencyClass.Ranged;
            if (type.CountsAsClass(DamageClass.Magic))
                return ProficiencyClass.Magic;
            if (type.CountsAsClass(DamageClass.Summon))
                return ProficiencyClass.Summon;
            return null;
        }

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone) => Gain(item.DamageType, target, damageDone);

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone) => Gain(proj.DamageType, target, damageDone);

        private void Gain(DamageClass type, NPC target, int damageDone)
        {
            if (Player.whoAmI != Main.myPlayer || cooldown > 0 || ClassOf(type) is not ProficiencyClass cls)
                return;
            if (target.SpawnedFromStatue || target.type == NPCID.TargetDummy || target.immortal || !(target.boss || damageDone > ProficiencyData.Rule.MinDamage))
                return;
            if (ModContent.TryFind<ModNPC>("CalamityMod", "SuperDummyNPC", out var dummy) && target.type == dummy.Type)
                return;
            int i = (int)cls;
            int gain = Meter[i] && Value[i] % 100 != 0 && Main.rand.NextFloat() < ProficiencyData.Rule.MeterDoubleChance ? 2 : 1;
            Value[i] = Math.Min(ProficiencyData.Rule.MaxValue, Value[i] + gain);
            cooldown = Meter[i] ? ProficiencyData.Rule.CooldownTicks / 2 : ProficiencyData.Rule.CooldownTicks;
            int level = ProficiencyData.LevelOf(Value[i]);
            if (level > Level[i])
            {
                Level[i] = level;
                LevelUp(i, level == ProficiencyData.Levels.Length);
            }
        }

        /// <summary>Message dans le chat et feu d'artifice (« a random firework is shot out from them »).</summary>
        private void LevelUp(int cls, bool final)
        {
            Main.NewText(Language.GetTextValue($"Mods.CalamityFandom.Proficiency.{(final ? "Max" : "LevelUp")}.{Keys[cls]}"), MessageColors[cls]);
            int firework = final ? ProjectileID.RocketFireworkRed + Main.rand.Next(4) : ProjectileID.RocketFireworksBoxRed + Main.rand.Next(4);
            Projectile.NewProjectile(Player.GetSource_FromThis("Proficiency"), Player.Center, new Vector2(0f, -5f), firework, 0, 0f, Player.whoAmI);
        }

        public override void PostUpdateEquips()
        {
            var melee = ProficiencyData.BonusOf(Level[(int)ProficiencyClass.Melee]);
            if (melee != null)
            {
                Player.GetDamage(DamageClass.Melee) += melee.Damage;
                Player.GetCritChance(DamageClass.Melee) += melee.Crit * 100f;
                Player.GetAttackSpeed(DamageClass.Melee) += melee.MeleeSpeed;
            }
            var ranged = ProficiencyData.BonusOf(Level[(int)ProficiencyClass.Ranged]);
            if (ranged != null)
            {
                Player.GetDamage(DamageClass.Ranged) += ranged.Damage;
                Player.GetCritChance(DamageClass.Ranged) += ranged.Crit * 100f;
            }
            var magic = ProficiencyData.BonusOf(Level[(int)ProficiencyClass.Magic]);
            if (magic != null)
            {
                Player.GetDamage(DamageClass.Magic) += magic.Damage;
                Player.GetCritChance(DamageClass.Magic) += magic.Crit * 100f;
                Player.manaCost -= magic.ManaCost;
            }
            var summon = ProficiencyData.BonusOf(Level[(int)ProficiencyClass.Summon]);
            if (summon != null)
            {
                Player.GetDamage(DamageClass.Summon) += summon.Damage;
                Player.GetKnockback(DamageClass.Summon).Base += summon.MinionKnockback;
                Player.maxMinions += summon.MinionSlots;
            }
            var spy = ProficiencyData.BonusOf(Level[(int)ProficiencyClass.Spy]);
            if (spy != null)
            {
                Player.GetDamage<RogueDamageClass>() += spy.Damage;
                Player.GetCritChance<RogueDamageClass>() += spy.Crit * 100f;
            }
        }

        // Ranged : « Ammo Consumption » = chance de ne pas consommer la munition
        public override bool CanConsumeAmmo(Item weapon, Item ammo)
        {
            var ranged = ProficiencyData.BonusOf(Level[(int)ProficiencyClass.Ranged]);
            return ranged == null || Main.rand.NextFloat() >= ranged.AmmoConsumption;
        }

        // Spy : « Projectile Speed »
        public override void ModifyShootStats(Item item, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            var spy = ProficiencyData.BonusOf(Level[(int)ProficiencyClass.Spy]);
            if (spy != null && item.CountsAsClass<RogueDamageClass>())
                velocity *= 1f + spy.ProjectileSpeed;
        }
    }
}
