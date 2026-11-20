using System;
using System.Collections.Generic;
using System.Linq;
using CalamityFandom.Common.Debug;
using CalamityFandom.Content.Projectiles.Weapons1119m;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Accessories;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

// Lot AY (20/11) : la chaîne des gelées : Nucleate Gello (Cragmaw Mire) → Assortegelatin → Slimelgamation → The Godfather.
// Chaque objet reprend les effets de ses ingrédients (UpdateAccessory appelés tels quels).

namespace CalamityFandom.Content.Items.Accessories
{
    /// <summary>
    /// Wiki : Nucleate Gello.txt — un Nucleate qui saute sur les ennemis et s'y colle (40 dégâts d'invocation, Sulphuric
    /// Poisoning 20 s, Irradiated 10 s) ; les slimes invoqués par King Slime, Queen Slime, The Slime God et Astrum Aureus ne
    /// vous touchent plus (Cragmaw Mire).
    /// </summary>
    public class NucleateGello : WikiCovering
    {
        protected override int Rarity => ItemRarityID.Pink;
        protected override int SellCopper => 48000;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            Weapons.Summon.WikiWeapons1120bPlayer.KeepMinion(player, Item, ModContent.ProjectileType<Projectiles.Summon.LatchSlime>(), ModContent.BuffType<Buffs.GelSlimeBuff>(), 40);
            player.GetModPlayer<GelPlayer>().Servants = true;
        }
    }

    /// <summary>
    /// Wiki : Assortegelatin.txt — effets du Royal Gel, de la Volatile Gelatin et du Nucleate Gello ; un slime invincible
    /// selon le biome qui saute sur les ennemis et s'y colle (400 dégâts d'invocation) ; tous les slimes sont pacifiés, même
    /// les boss. [choix] Couleur du slime selon le biome ; « pacifiés » = ils ne vous touchent plus. [technique] Les slimes
    /// sauvages qui se battent pour vous (x6 dégâts, x12 vie) ne sont pas faits.
    /// </summary>
    public class Assortegelatin : WikiCovering
    {
        protected override int Rarity => ItemRarityID.Red;
        protected override int SellCopper => 48000;

        public override void UpdateAccessory(Player player, bool hideVisual) => Apply(player, Item, hideVisual);

        public static void Apply(Player player, Item item, bool hideVisual)
        {
            player.ApplyEquipFunctional(ContentSamples.ItemsByType[ItemID.RoyalGel], hideVisual);
            player.ApplyEquipFunctional(ContentSamples.ItemsByType[ItemID.VolatileGelatin], hideVisual);
            player.GetModPlayer<GelPlayer>().Servants = true;
            player.GetModPlayer<GelPlayer>().AllSlimes = true;
            // Le slime du biome remplace le Nucleate (400 au lieu de 40)
            Weapons.Summon.WikiWeapons1120bPlayer.KeepMinion(player, item, ModContent.ProjectileType<Projectiles.Summon.LatchSlime>(), ModContent.BuffType<Buffs.GelSlimeBuff>(), 400);
            foreach (Projectile p in Main.projectile)
                if (p.active && p.owner == player.whoAmI && p.type == ModContent.ProjectileType<Projectiles.Summon.LatchSlime>() && p.originalDamage < 400)
                    p.Kill();
        }
    }

    /// <summary>
    /// Wiki : Slimelgamation.txt — effets de The Amalgam (durée des potions, debuffs et pluie quand on est touché, traînée de
    /// spores et de nuages...) et de l'Assortegelatin.
    /// </summary>
    public class Slimelgamation : WikiCovering
    {
        protected override int Rarity => ModContent.RarityType<BurnishedAuric>();
        protected override int SellCopper => 320000;

        public override void UpdateAccessory(Player player, bool hideVisual) => Apply(player, Item, hideVisual);

        public static void Apply(Player player, Item item, bool hideVisual)
        {
            ModContent.GetInstance<TheAmalgam>().UpdateAccessory(player, hideVisual);
            Assortegelatin.Apply(player, item, hideVisual);
        }
    }

    /// <summary>
    /// Wiki : The Godfather.txt — « All effects of ingredients » : Slimelgamation, The Sponge, Abyssal Diving Suit, Nebulous
    /// Core, Hide of Astrum Deus, Deep Diver, The Transformer, Luxor's Gift. [technique] Pas faits : la ruée de la touche
    /// Plague Pack et les mini-projectiles sur toutes les armes (déjà donnés par Luxor's Gift sur la plupart).
    /// </summary>
    public class TheGodfather : WikiCovering
    {
        protected override int Rarity => ModContent.RarityType<BurnishedAuric>();
        protected override int SellCopper => 320000;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            Slimelgamation.Apply(player, Item, hideVisual);
            ModContent.GetInstance<TheSponge>().UpdateAccessory(player, hideVisual);
            ModContent.GetInstance<AbyssalDivingSuit>().UpdateAccessory(player, hideVisual);
            ModContent.GetInstance<NebulousCore>().UpdateAccessory(player, hideVisual);
            ModContent.GetInstance<HideofAstrumDeus>().UpdateAccessory(player, hideVisual);
            ModContent.GetInstance<DeepDiver>().UpdateAccessory(player, hideVisual);
            ModContent.GetInstance<TheTransformer>().UpdateAccessory(player, hideVisual);
            ModContent.GetInstance<LuxorsGift>().UpdateAccessory(player, hideVisual);
        }
    }

    /// <summary>
    /// Butin de la page du Nucleate Gello (pas de celle du Cragmaw Mire) : 100 % en Acid Rain de niveau 2, 10 % au niveau 3.
    /// [choix] Niveau 3 = après Polterghast, comme dans Calamity.
    /// </summary>
    public class NucleateGelloDrop : GlobalNPC
    {
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == ModContent.NPCType<CalamityMod.NPCs.AcidRain.CragmawMire>();

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            var tier2 = new Terraria.GameContent.ItemDropRules.LeadingConditionRule(DropHelper.If(_ => !DownedBossSystem.downedPolterghast));
            tier2.OnSuccess(Terraria.GameContent.ItemDropRules.ItemDropRule.Common(ModContent.ItemType<NucleateGello>()));
            var tier3 = new Terraria.GameContent.ItemDropRules.LeadingConditionRule(DropHelper.If(_ => DownedBossSystem.downedPolterghast));
            tier3.OnSuccess(Terraria.GameContent.ItemDropRules.ItemDropRule.Common(ModContent.ItemType<NucleateGello>(), 10));
            npcLoot.Add(tier2);
            npcLoot.Add(tier3);
            Common.WikiData.WikiLoot.Keep.Add(tier2);
            Common.WikiData.WikiLoot.Keep.Add(tier3);
        }
    }

    /// <summary>Pacification des slimes : Servants (slimes des boss), AllSlimes (tous, même les boss).</summary>
    public class GelPlayer : ModPlayer
    {
        public bool Servants, AllSlimes;

        public override void ResetEffects() => Servants = AllSlimes = false;
    }

    public class GelNPC : GlobalNPC
    {
        private static HashSet<int> slimeBosses, servants;

        private static void Build()
        {
            if (slimeBosses != null)
                return;
            slimeBosses = new HashSet<int> { NPCID.KingSlime, NPCID.QueenSlimeBoss };
            servants = new HashSet<int> { NPCID.BlueSlime, NPCID.SlimeSpiked, NPCID.QueenSlimeMinionBlue, NPCID.QueenSlimeMinionPink, NPCID.QueenSlimeMinionPurple };
            foreach (string n in new[] { "SlimeGodCore", "CrimulanPaladin", "EbonianPaladin", "SplitCrimulanPaladin", "SplitEbonianPaladin" })
                if (ModContent.TryFind<ModNPC>("CalamityMod", n, out var m))
                    slimeBosses.Add(m.Type);
            foreach (string n in new[] { "CorruptSlimeSpawn", "CorruptSlimeSpawn2", "CrimsonSlimeSpawn", "CrimsonSlimeSpawn2", "AureusSpawn" })
                if (ModContent.TryFind<ModNPC>("CalamityMod", n, out var m))
                    servants.Add(m.Type);
        }

        public static bool IsSlime(NPC npc) => npc.aiStyle == NPCAIStyleID.Slime || slimeBosses.Contains(npc.type) || servants.Contains(npc.type);

        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            Build();
            var gp = target.GetModPlayer<GelPlayer>();
            if (gp.AllSlimes && IsSlime(npc))
                return false;
            // Slimes invoqués pendant un combat contre un des quatre boss
            if (gp.Servants && servants.Contains(npc.type) && (NPC.AnyNPCs(NPCID.KingSlime) || NPC.AnyNPCs(NPCID.QueenSlimeBoss)
                || slimeBosses.Any(NPC.AnyNPCs) || NPC.AnyNPCs(ModContent.NPCType<CalamityMod.NPCs.AstrumAureus.AstrumAureus>())))
                return false;
            return true;
        }
    }

    public class WikiCoverings1120cChecks : ModSystem
    {
        public override void Load()
        {
            SelfTest.ExtraChecks.Add(t =>
            {
                foreach (int type in new[] { ModContent.ItemType<Assortegelatin>(), ModContent.ItemType<Slimelgamation>(), ModContent.ItemType<TheGodfather>() })
                    t.Expect(Main.recipe.Take(Recipe.numRecipes).Any(r => r.createItem.type == type && !r.Disabled), $"recette du wiki de {Lang.GetItemNameValue(type)}");
                var rates = new List<Terraria.GameContent.ItemDropRules.DropRateInfo>();
                foreach (var r in Main.ItemDropsDB.GetRulesForNPCID(ModContent.NPCType<CalamityMod.NPCs.AcidRain.CragmawMire>(), false))
                    r.ReportDroprates(rates, new Terraria.GameContent.ItemDropRules.DropRateInfoChainFeed(1f));
                t.Expect(rates.Any(r => r.itemId == ModContent.ItemType<NucleateGello>()), "le Cragmaw Mire lâche le Nucleate Gello");
            });
        }
    }
}

namespace CalamityFandom.Content.Buffs
{
    public class GelSlimeBuff : WikiMinionBuff
    {
        public override string Texture => "Terraria/Images/Buff_" + BuffID.BabySlime;
        protected override int MinionType => ModContent.ProjectileType<Projectiles.Summon.LatchSlime>();
    }
}

namespace CalamityFandom.Content.Projectiles.Summon
{
    /// <summary>
    /// Slime d'accessoire (Nucleate Gello : 40, vert, Sulphuric Poisoning et Irradiated ; Assortegelatin : 400, couleur du
    /// biome) : saute sur l'ennemi le plus proche et s'y colle 2 s, invincible.
    /// </summary>
    public class LatchSlime : WikiMinion
    {
        public override string Texture => "Terraria/Images/NPC_" + NPCID.BlueSlime;
        protected override int BuffType => ModContent.BuffType<Buffs.GelSlimeBuff>();
        protected override float Slots => 0f;

        private bool Nucleate => Projectile.originalDamage < 400;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 24;
            Projectile.height = 18;
        }

        public override void AI()
        {
            if (!KeepAlive())
                return;
            if (++Projectile.frameCounter % 10 == 0)
                Projectile.frame ^= 1;
            // Collé à un ennemi
            if (Projectile.ai[1] > 0f)
            {
                NPC stuck = Main.npc[(int)Projectile.ai[1] - 1];
                if (!stuck.active || ++Projectile.localAI[0] > 120)
                {
                    Projectile.ai[1] = 0f;
                    Projectile.localAI[0] = 0f;
                    Projectile.velocity = new Vector2(Main.rand.NextFloat(-3f, 3f), -6f);
                }
                else
                {
                    Projectile.Center = stuck.Center;
                    Projectile.velocity = Vector2.Zero;
                    return;
                }
            }
            NPC target = Projectile.Center.MinionHoming(900f, Owner);
            if (target == null)
            {
                Idle();
                return;
            }
            if (++Projectile.ai[0] % 30 == 0)
                Projectile.velocity = Projectile.SafeDirectionTo(target.Center) * 12f + new Vector2(0f, -3f);
            else
                Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.25f, 10f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[1] == 0f)
            {
                Projectile.ai[1] = target.whoAmI + 1;
                Projectile.netUpdate = true;
            }
            if (Nucleate)
            {
                target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 1200);
                target.AddBuff(ModContent.BuffType<Irradiated>(), 600);
            }
        }

        private Color Tint()
        {
            if (Nucleate)
                return new Color(120, 220, 90);
            Player p = Owner;
            if (p.ZoneCorrupt) return new Color(150, 90, 200);
            if (p.ZoneCrimson) return new Color(220, 60, 60);
            if (p.ZoneHallow) return new Color(255, 150, 220);
            if (p.ZoneJungle) return new Color(90, 200, 60);
            if (p.ZoneSnow) return new Color(170, 220, 255);
            if (p.ZoneDesert) return new Color(230, 200, 110);
            if (p.ZoneUnderworldHeight) return new Color(255, 110, 40);
            if (p.Calamity().ZoneAstral) return new Color(120, 80, 220);
            return new Color(80, 140, 255);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Rectangle frame = tex.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Color c = Tint().MultiplyRGBA(lightColor) * 0.85f;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, frame, c, 0f, frame.Size() / 2f, 1f, SpriteEffects.None);
            return false;
        }
    }
}
