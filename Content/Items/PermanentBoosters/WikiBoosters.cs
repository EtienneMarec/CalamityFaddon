using System;
using CalamityMod;
using CalamityMod.Buffs.Potions;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

// Bonus permanents et potions inventés par le wiki (19/11) : sprites et valeurs des pages, recettes du wiki.

namespace CalamityFandom.Content.Items.PermanentBoosters
{
    /// <summary>Bonus permanent à usage unique (comme le Life Fruit) : un drapeau sauvegardé par personnage.</summary>
    public abstract class WikiBooster : ModItem
    {
        protected abstract int Rarity { get; }
        protected abstract int SellCopper { get; }

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = Item.height = 28;
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.UseSound = SoundID.Item2;
            Item.consumable = true;
            Item.maxStack = 9999;
            Item.rare = Rarity;
            Item.value = SellCopper * 5;
        }

        public override bool CanUseItem(Player player) => !player.GetModPlayer<WikiBoosterPlayer>().Used.Contains(Name);

        public override bool? UseItem(Player player)
        {
            player.GetModPlayer<WikiBoosterPlayer>().Used.Add(Name);
            OnConsume(player);
            return true;
        }

        protected virtual void OnConsume(Player player)
        {
        }
    }

    /// <summary>Wiki : Bagel Nuts.txt — +25 de pénétration d'armure, pour toujours.</summary>
    public class BagelNuts : WikiBooster
    {
        protected override int Rarity => ItemRarityID.Yellow;
        protected override int SellCopper => 1000000;
    }

    /// <summary>Wiki : Chez.txt — +7 % de chance et de dégâts critiques, pour toujours.</summary>
    public class Chez : WikiBooster
    {
        protected override int Rarity => ItemRarityID.Yellow;
        protected override int SellCopper => 1000000;
    }

    /// <summary>
    /// Wiki : Ham Pie.txt — +8 défense, +8 % de réduction des dégâts, dégâts de défense divisés par deux, pour toujours ;
    /// une chance sur mille de donner une Meld Blob en le mangeant.
    /// </summary>
    public class HamPie : WikiBooster
    {
        protected override int Rarity => ItemRarityID.Orange;
        protected override int SellCopper => 1000000;

        protected override void OnConsume(Player player)
        {
            if (Main.rand.NextBool(1000))
                player.QuickSpawnItem(player.GetSource_ItemUse(Item), ModContent.ItemType<MeldBlob>());
        }
    }

    /// <summary>Wiki : Juzie.txt — +8 de régénération de vie et régénération qui monte 40 % plus vite, pour toujours.</summary>
    public class Juzie : WikiBooster
    {
        protected override int Rarity => ItemRarityID.LightRed;
        protected override int SellCopper => 750000;
    }

    /// <summary>Wiki : KEX.txt — +15 % de vitesse d'attaque, de déplacement et d'accélération, pour toujours.</summary>
    public class KEX : WikiBooster
    {
        protected override int Rarity => ItemRarityID.Orange;
        protected override int SellCopper => 500000;
    }

    public class WikiBoosterPlayer : ModPlayer
    {
        public System.Collections.Generic.HashSet<string> Used = new();

        private bool Has(string name) => Used.Contains(name);

        public override void SaveData(TagCompound tag) => tag["wikiBoosters"] = new System.Collections.Generic.List<string>(Used);

        public override void LoadData(TagCompound tag) => Used = new(tag.GetList<string>("wikiBoosters"));

        public override void CopyClientState(ModPlayer targetCopy) => ((WikiBoosterPlayer)targetCopy).Used = new(Used);

        // Synchronisation : les autres joueurs voient les effets (Elder Berry, bonus de dégâts) du joueur
        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            if (!((WikiBoosterPlayer)clientPlayer).Used.SetEquals(Used))
                SyncPlayer(-1, Main.myPlayer, false);
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)CalamityFandom.ExtraPacket.WikiBoosters);
            packet.Write((byte)Player.whoAmI);
            packet.Write((byte)Used.Count);
            foreach (string name in Used)
                packet.Write(name);
            packet.Send(toWho, fromWho);
        }

        /// <summary>Paquet <see cref="CalamityFandom.ExtraPacket.WikiBoosters"/> : relayé par le serveur aux autres clients.</summary>
        public static void Receive(System.IO.BinaryReader reader, int whoAmI)
        {
            int player = reader.ReadByte();
            int count = reader.ReadByte();
            var used = new System.Collections.Generic.HashSet<string>();
            for (int i = 0; i < count; i++)
                used.Add(reader.ReadString());
            var mp = Main.player[player].GetModPlayer<WikiBoosterPlayer>();
            mp.Used = used;
            if (Main.netMode == NetmodeID.Server)
                mp.SyncPlayer(-1, whoAmI, false);
        }

        public override void PostUpdateEquips()
        {
            if (Has(nameof(BagelNuts)))
                Player.GetArmorPenetration<GenericDamageClass>() += 25;
            if (Has(nameof(Chez)))
                Player.GetCritChance<GenericDamageClass>() += 7;
            if (Has(nameof(HamPie)))
            {
                Player.statDefense += 8;
                Player.endurance += 0.08f;
                Player.Calamity().defenseDamageRatio *= 0.5;
            }
            if (Has(nameof(KEX)))
            {
                Player.GetAttackSpeed<GenericDamageClass>() += 0.15f;
                Player.moveSpeed += 0.15f;
                Player.runAcceleration *= 1.15f;
            }
        }

        public override void UpdateLifeRegen()
        {
            if (!Has(nameof(Juzie)))
                return;
            Player.lifeRegen += 8;
            // Montée de la régénération 40 % plus rapide
            Player.lifeRegenTime += 0.4f;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Has(nameof(Chez)))
                modifiers.CritDamage += 0.07f;
        }
    }
}

namespace CalamityFandom.Content.Items.Potions
{
    /// <summary>
    /// Wiki : Draconic Elixir.txt — Draconic Surge 8 min : +20 % de temps de vol, +15 % de vitesse et d'accélération en vol,
    /// +16 défense ; avec l'armure Silva ou Auric, la résurrection de Calamity soigne aussi à la moitié des PV, retire le buff,
    /// donne Draconic Surge Cooldown 60 s (pas de nouvel élixir) et 30 s de Potion Sickness.
    /// </summary>
    public class DraconicElixir : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 20;

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 44;
            Item.scale = 0.7f;
            Item.useTime = Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.UseSound = SoundID.Item3;
            Item.useTurn = true;
            Item.consumable = true;
            Item.maxStack = 9999;
            Item.rare = ModContent.RarityType<CalamityMod.Rarities.BurnishedAuric>();
            Item.value = Item.sellPrice(silver: 40);
            Item.buffType = ModContent.BuffType<Buffs.DraconicSurge>();
            Item.buffTime = 8 * 3600;
        }

        public override bool CanUseItem(Player player) => !player.HasBuff<Buffs.DraconicSurgeCooldown>();
    }

    /// <summary>
    /// Wiki : Potion of Triboluminescence.txt — Triboluminescence 15 min : effets des potions Hunter, Spelunker, Dangersense
    /// et Omniscience (Calamity).
    /// </summary>
    public class PotionofTriboluminescence : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 20;

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 38;
            Item.useTime = Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.UseSound = SoundID.Item3;
            Item.useTurn = true;
            Item.consumable = true;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(silver: 80);
            Item.buffType = ModContent.BuffType<Buffs.Triboluminescence>();
            Item.buffTime = 15 * 3600;
        }
    }

    /// <summary>
    /// Wiki : Stratus Beverage.txt — alcool : 2 min de Stratus Beverage, -33 % de dégâts mais les armes à projectiles tirent
    /// en plus deux étoiles Stratus chercheuses (dégâts de l'arme, classe la plus forte du joueur).
    /// </summary>
    public class StratusBeverage : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 20;

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 34;
            Item.useTime = Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.UseSound = SoundID.Item3;
            Item.useTurn = true;
            Item.consumable = true;
            Item.maxStack = 9999;
            Item.rare = ModContent.RarityType<CalamityMod.Rarities.PureGreen>();
            Item.value = Item.sellPrice(gold: 10);
            Item.buffType = ModContent.BuffType<Buffs.StratusBeverageDebuff>();
            Item.buffTime = 2 * 3600;
        }
    }
}

namespace CalamityFandom.Content.Buffs
{
    public class DraconicSurge : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.statDefense += 16;
            player.GetModPlayer<DraconicPlayer>().Surge = true;
        }
    }

    public class DraconicSurgeCooldown : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
    }

    public class DraconicPlayer : ModPlayer
    {
        public bool Surge;
        private bool wasReviving;

        public override void ResetEffects() => Surge = false;

        public override void PostUpdateEquips()
        {
            if (!Surge)
                return;
            Player.wingTimeMax = (int)(Player.wingTimeMax * 1.2f);
        }

        public override void PostUpdateRunSpeeds()
        {
            if (!Surge || Player.velocity.Y == 0f || Player.wingTime <= 0f)
                return;
            Player.maxRunSpeed *= 1.15f;
            Player.runAcceleration *= 1.15f;
        }

        public override void PostUpdate()
        {
            // Résurrection Silva / Auric de Calamity : son compte à rebours démarre
            var cal = Player.Calamity();
            bool reviving = (cal.silvaSet || cal.auricSet) && cal.silvaCountdown < CalamityMod.Items.Armor.Silva.SilvaArmor.ReviveDuration && cal.silvaCountdown > 0;
            if (reviving && !wasReviving && Surge && Player.whoAmI == Main.myPlayer)
            {
                int heal = Player.statLifeMax2 / 2 - Player.statLife;
                if (heal > 0)
                {
                    Player.statLife += heal;
                    Player.HealEffect(heal);
                }
                Player.ClearBuff(ModContent.BuffType<DraconicSurge>());
                Player.AddBuff(ModContent.BuffType<DraconicSurgeCooldown>(), 3600);
                int sick = Player.FindBuffIndex(BuffID.PotionSickness);
                if (sick >= 0)
                    Player.buffTime[sick] += 1800;
                else
                    Player.AddBuff(BuffID.PotionSickness, 1800);
            }
            wasReviving = reviving;
        }
    }

    public class Triboluminescence : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.detectCreature = true;
            player.findTreasure = true;
            player.dangerSense = true;
            ModContent.GetInstance<Omniscience>().Update(player, ref buffIndex);
        }
    }

    public class StratusBeverageDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage<GenericDamageClass>() -= 0.33f;
            player.GetModPlayer<StratusDrinkPlayer>().Active = true;
        }
    }

    public class StratusDrinkPlayer : ModPlayer
    {
        public bool Active;

        public override void ResetEffects() => Active = false;
    }

    /// <summary>Deux étoiles Stratus en plus pour chaque arme qui tire des projectiles (Stratus Beverage).</summary>
    public class StratusDrinkShots : GlobalItem
    {
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!player.GetModPlayer<StratusDrinkPlayer>().Active || item.shoot <= ProjectileID.None || item.damage <= 0)
                return true;
            for (int i = -1; i <= 1; i += 2)
            {
                int p = Projectile.NewProjectile(source, position, velocity.RotatedBy(0.2f * i).SafeNormalize(Vector2.UnitX) * 10f,
                    ModContent.ProjectileType<StratusDrinkStar>(), damage, knockback, player.whoAmI);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].DamageType = player.GetBestClass();
            }
            return true;
        }
    }

    /// <summary>Étoile Stratus de la Stratus Beverage : celle de la Zweihandromeda, qui cherche les ennemis.</summary>
    public class StratusDrinkStar : Projectiles.Melee.StratusStar
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Melee/StratusStar";

        public override void AI()
        {
            base.AI();
            CalamityUtils.HomeInOnNPC(Projectile, true, 600f, 12f, 20f);
        }
    }
}
