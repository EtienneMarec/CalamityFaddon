using System;
using System.Linq;
using CalamityFandom.Common.Debug;
using CalamityFandom.Content.Projectiles.Weapons1119;
using CalamityMod;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.SummonItems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

// Lot AT (20/11) : suite du butin du wiki de PNJ existants (Skeletron, Moon Lord, Harpy, Glitch, Stiriatic Walkie).

namespace CalamityFandom.Content.Items.SummonItems
{
    /// <summary>
    /// Wiki : Kos Fragment.txt — relique des os lâchée par Skeletron (100 %), non consommée : dans l'espace, le Donjon ou les
    /// Enfers, elle appelle chacun des trois boss (Cloud Pusher, Ceasing Void, Sigmund : la Storm Weaver, le Ceaseless Void
    /// et Signus de Calamity). [choix] Mêmes règles que la Mark of Providence (Rune of Kos), dont elle reprend l'usage, mais
    /// dès le pré-Hardmode comme le dit la page.
    /// </summary>
    public class KosFragment : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 17;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 30;
            Item.useAnimation = Item.useTime = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
            Item.rare = ModContent.RarityType<CalamityMod.Rarities.Turquoise>();
            Item.value = Item.sellPrice(gold: 1);
        }

        public override bool CanUseItem(Player player) => ModContent.GetInstance<MarkofProvidence>().CanUseItem(player);

        public override bool? UseItem(Player player) => ModContent.GetInstance<MarkofProvidence>().UseItem(player);
    }
}

namespace CalamityFandom.Content.Items.Pets
{
    /// <summary>Wiki : Everflute.txt — un drôle d'oiseau qui suit le joueur et vole pour le rattraper (Glitch, 5 %).</summary>
    public class Everflute : WikiPetItem
    {
        protected override int PetType => ModContent.ProjectileType<Projectiles.Pets.WeirdBird>();
        protected override int PetBuff => ModContent.BuffType<Buffs.WeirdBirdBuff>();
        protected override int Rarity => ModContent.RarityType<CalamityMod.Rarities.CosmicPurple>();
        protected override int SellCopper => 150000;
    }

    /// <summary>Wiki : Crystallised Stiriatic Chunk.txt — un Stiriatic Walkie qui suit le joueur (Stiriatic Walkie, 10 %). Rareté « C2 » → 0.</summary>
    public class CrystallisedStiriaticChunk : WikiPetItem
    {
        protected override int PetType => ModContent.ProjectileType<Projectiles.Pets.StiriaticWalkie>();
        protected override int PetBuff => ModContent.BuffType<Buffs.StiriaticWalkieBuff>();
        protected override int Rarity => ItemRarityID.White;
        protected override int SellCopper => 15000000;
    }
}

namespace CalamityFandom.Content.Buffs
{
    public class WeirdBirdBuff : WikiPetBuff
    {
        public override string Texture => "CalamityFandom/Content/Buffs/Wiki/WeirdBirdBuff";
        protected override int PetType => ModContent.ProjectileType<Projectiles.Pets.WeirdBird>();
    }

    public class StiriaticWalkieBuff : WikiPetBuff
    {
        public override string Texture => "CalamityFandom/Content/Buffs/Wiki/StiriaticWalkieBuff";
        protected override int PetType => ModContent.ProjectileType<Projectiles.Pets.StiriaticWalkie>();
    }
}

namespace CalamityFandom.Content.Projectiles.Pets
{
    public class WeirdBird : WikiPet
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Pets/WeirdBird";
        protected override int PetBuff => ModContent.BuffType<Buffs.WeirdBirdBuff>();
        protected override bool Flying => false;
    }

    public class StiriaticWalkie : WikiPet
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Pets/StiriaticWalkie";
        protected override int PetBuff => ModContent.BuffType<Buffs.StiriaticWalkieBuff>();
        protected override bool Flying => false;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 40;
            Projectile.height = 28;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Typeless
{
    /// <summary>
    /// Wiki : Devious Aestheticus.txt — arme sans classe (Moon Lord, 5 %) : une gerbe de cristaux dont le nombre suit les
    /// places de sbire vides (8 au plus) ; Vaporfied 7 s (tir principal), 4 s (éclats, coup de l'arme).
    /// [choix] 20 images ; un cristal + un par place vide, en éventail de 10° par cristal ; le cristal principal éclate en
    /// 3 cristaux chercheurs (ceux de l'Aestheticus, 1/3 des dégâts) ; l'arme frappe aussi au contact.
    /// </summary>
    public class DeviousAestheticus : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 266;
            Item.DamageType = ModContent.GetInstance<AverageDamageClass>();
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 1f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item109;
            Item.width = Item.height = 72;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.sellPrice(gold: 1);
            Item.shoot = ModContent.ProjectileType<Projectiles.Typeless.DeviousCrystal>();
            Item.shootSpeed = 12f;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Vaporfied>(), 240);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int empty = (int)Math.Clamp(player.maxMinions - player.slotsMinions, 0f, 8f);
            int count = 1 + empty;
            float spread = MathHelper.ToRadians(10f);
            for (int i = 0; i < count; i++)
                Projectile.NewProjectile(source, position, velocity.RotatedBy(spread * (i - (count - 1) / 2f)), type, damage, knockback, player.whoAmI);
            return false;
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Typeless
{
    /// <summary>Cristal principal du Devious Aestheticus : Vaporfied 7 s, éclate en 3 cristaux chercheurs (ceux de l'Aestheticus).</summary>
    public class DeviousCrystal : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Typeless/AestheticusCrystal";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<AverageDamageClass>();
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.scale = 1.4f;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Lighting.AddLight(Projectile.Center, 0.8f, 0.4f, 0.9f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Vaporfied>(), 420);
            if (Projectile.owner != Main.myPlayer)
                return;
            for (int i = 0; i < 3; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Main.rand.NextVector2Unit() * 7f, ModContent.ProjectileType<AestheticusCrystal>(),
                    Math.Max(1, Projectile.damage / 3), 0f, Projectile.owner);
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Wakasagihime.txt — épée (Harpies, 5 %, après Duke Fishron) : un ennemi touché (pas un boss) perd de la défense et
    /// de la résistance au recul ; projeté contre un bloc, il prend des dégâts selon sa vitesse horizontale.
    /// [choix] -2 défense par coup (jusqu'à 0), +10 % de recul subi par coup ; choc = 12 x vitesse, dans la seconde qui suit
    /// le coup. [technique] Le « Karma » (boss appelés en tuant dans l'océan) n'est pas fait.
    /// </summary>
    public class Wakasagihime : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 84;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 35f;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.UseSound = SoundID.Item1;
            Item.width = 94;
            Item.height = 96;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 9, silver: 99, copper: 99);
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.boss)
                return;
            target.defense = Math.Max(0, target.defense - 2);
            target.knockBackResist = Math.Min(1f, target.knockBackResist * 1.1f + 0.02f);
            target.GetGlobalNPC<WakasagihimeNPC>().SlamTime = 60;
        }
    }

    public class WakasagihimeNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public int SlamTime;
        private float lastSpeed;

        public override void PostAI(NPC npc)
        {
            if (SlamTime <= 0)
                return;
            SlamTime--;
            // Choc contre un mur : la vitesse horizontale tombe d'un coup
            if (npc.collideX && Math.Abs(lastSpeed) > 4f && Math.Abs(npc.velocity.X) < 1f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.SimpleStrikeNPC((int)(Math.Abs(lastSpeed) * 12f), Math.Sign(lastSpeed));
                SlamTime = 0;
            }
            lastSpeed = npc.velocity.X;
        }
    }

    public class WikiItems1120bChecks : ModSystem
    {
        public override void Load()
        {
            SelfTest.ExtraChecks.Add(t =>
            {
                foreach (string name in new[] { "Kos Fragment", "Everflute", "Crystallised Stiriatic Chunk", "Devious Aestheticus", "Wakasagihime" })
                    t.Expect(Common.WikiData.WikiLoot.Added.Any(a => a.what.StartsWith(name)), $"butin du wiki : {name}");
            });
        }
    }
}
