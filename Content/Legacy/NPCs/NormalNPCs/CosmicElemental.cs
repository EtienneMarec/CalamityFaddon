// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using CalamityFandom.Content.Legacy.Buffs.StatDebuffs;
using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace CalamityFandom.Content.Legacy.NPCs.NormalNPCs
{
    /// <summary>
    /// Wiki : Cosmic Elemental.txt — ennemi rare des cavernes avant Hardmode qui se comporte comme un Granite Elemental
    /// (traverse les murs, se roule en boule quand on le frappe en Expert) ; Confused 3 s au contact ; épées rares en butin
    /// (Bone Sword 4 %, Starfury 2 %, Enchanted Sword 1 %, Arkhalis 0,5 %).
    /// TODO : Cosmic Stone et Arcanoid Orb (pas encore dans le mod ; WikiLoot les ajoutera depuis l'infobox).
    /// </summary>
    public class CosmicElemental : ModNPC
    {
        public override void SetStaticDefaults() => Main.npcFrameCount[Type] = 11;

        public override void SetDefaults()
        {
            NPC.npcSlots = 0.5f;
            NPC.aiStyle = NPCAIStyleID.GraniteElemental;
            NPC.damage = 20;
            NPC.width = NPC.height = 30;
            NPC.defense = 10;
            NPC.lifeMax = 25;
            NPC.knockBackResist = 0.5f;
            NPC.value = Item.buyPrice(silver: 3);
            NPC.HitSound = SoundID.NPCHit7;
            NPC.DeathSound = SoundID.NPCDeath6;
            NPC.buffImmune[BuffID.Confused] = true;
            NPC.buffImmune[BuffID.Poisoned] = true;
            NPC.buffImmune[ModContent.BuffType<KamiFlu>()] = true;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,
                new FlavorTextBestiaryInfoElement("Mods.CalamityFandom.NPCs.CosmicElemental.Bestiary"),
            });
        }

        public override void FindFrame(int frameHeight)
        {
            if (++NPC.frameCounter > 6)
            {
                NPC.frame.Y += frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.ai[0] == -1f)
            {
                // En boule
                if (NPC.frame.Y >= frameHeight * 11)
                    NPC.frame.Y = frameHeight * 10;
                else if (NPC.frame.Y <= frameHeight * 5)
                    NPC.frame.Y = frameHeight * 6;
                NPC.rotation += NPC.velocity.X * 0.2f;
            }
            else
            {
                if (NPC.frame.Y >= frameHeight * 6)
                    NPC.frame.Y = 0;
                NPC.rotation = NPC.velocity.X * 0.1f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[Type].Value;
            int height = texture.Height / Main.npcFrameCount[Type];
            SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(texture, NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY), NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation,
                new Vector2(texture.Width / 2f, height / 2f), NPC.scale, effects, 0f);
            return false;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            var cal = spawnInfo.Player.Calamity();
            if (spawnInfo.PlayerSafe || cal.ZoneAbyss || cal.ZoneSunkenSea)
                return 0f;
            return SpawnCondition.Cavern.Chance * 0.01f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(BuffID.Confused, 180, true);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < (NPC.life <= 0 ? 25 : 5); k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 70, hit.HitDirection, -1f);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.BoneSword, 25));
            npcLoot.Add(ItemDropRule.Common(ItemID.Starfury, 50));
            npcLoot.Add(ItemDropRule.Common(ItemID.EnchantedSword, 100));
            npcLoot.Add(ItemDropRule.Common(ItemID.Arkhalis, 200));
        }
    }
}
