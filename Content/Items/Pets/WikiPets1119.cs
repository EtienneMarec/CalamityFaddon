using System;
using CalamityMod;
using CalamityMod.Events;
using CalamityMod.NPCs.AstrumAureus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

// Lot AK (19/11) : familiers du wiki et Astralination (recettes du wiki).

namespace CalamityFandom.Content.Items.Pets
{
    /// <summary>Objet d'invocation d'un familier du wiki.</summary>
    public abstract class WikiPetItem : ModItem
    {
        protected abstract int PetType { get; }
        protected abstract int PetBuff { get; }
        protected abstract int Rarity { get; }
        protected abstract int SellCopper { get; }

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(PetType, PetBuff);
            Item.width = Item.height = 32;
            Item.rare = Rarity;
            Item.value = SellCopper * 5;
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
                player.AddBuff(Item.buffType, 3600);
            return true;
        }
    }

    /// <summary>Wiki : Batfly Bait.txt — un lézard vert presque apprivoisé qui suit le joueur (au sol).</summary>
    public class BatflyBait : WikiPetItem
    {
        protected override int PetType => ModContent.ProjectileType<Projectiles.Pets.GreenLizard>();
        protected override int PetBuff => ModContent.BuffType<Buffs.GreenLizardBuff>();
        protected override int Rarity => ItemRarityID.Lime;
        protected override int SellCopper => 120000;
    }

    /// <summary>Wiki : Cool Item.txt — InxieRatZey, qui suit le joueur sans voler mais saute et fonce. [choix] Ruée quand il est loin.</summary>
    public class CoolItem : WikiPetItem
    {
        protected override int PetType => ModContent.ProjectileType<Projectiles.Pets.InxieRatZey>();
        protected override int PetBuff => ModContent.BuffType<Buffs.InxieRatZeyBuff>();
        protected override int Rarity => ItemRarityID.Pink;
        protected override int SellCopper => 220000;
    }

    /// <summary>Wiki : Elemental Delight.txt — un serpent cynthien qui vole autour du joueur.</summary>
    public class ElementalDelight : WikiPetItem
    {
        protected override int PetType => ModContent.ProjectileType<Projectiles.Pets.CynthianSerpent>();
        protected override int PetBuff => ModContent.BuffType<Buffs.CynthianSerpentBuff>();
        protected override int Rarity => ItemRarityID.Red;
        protected override int SellCopper => 150000;
    }
}

namespace CalamityFandom.Content.Items.SummonItems
{
    /// <summary>
    /// Wiki : Astralination.txt — invoque Astrum Aureus dans l'Astral Infection la nuit ; non consommé.
    /// (Comme l'Astral Chunk de Calamity, avec la nuit en plus.)
    /// </summary>
    public class Astralination : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 15;
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 46;
            Item.useTime = Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
            Item.rare = ItemRarityID.Lime;
            Item.value = 0;
            Item.UseSound = SoundID.Roar;
        }

        public override bool CanUseItem(Player player)
            => player.Calamity().ZoneAstral && !Main.dayTime && !NPC.AnyNPCs(ModContent.NPCType<AstrumAureus>()) && !BossRushEvent.BossRushActive;

        public override bool? UseItem(Player player)
        {
            CalamityUtils.SpawnBossOnPosUsingItem(player, ModContent.NPCType<AstrumAureus>(), (int)(player.position.X + Main.rand.Next(-250, 251)), (int)(player.position.Y - 500f), Item.UseSound);
            return true;
        }
    }
}

namespace CalamityFandom.Content.Buffs
{
    /// <summary>Buff d'un familier du wiki : garde le familier tant qu'il est actif.</summary>
    public abstract class WikiPetBuff : ModBuff
    {
        protected abstract int PetType { get; }

        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;
            if (player.ownedProjectileCounts[PetType] <= 0 && player.whoAmI == Main.myPlayer)
                Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, PetType, 0, 0f, player.whoAmI);
        }
    }

    public class GreenLizardBuff : WikiPetBuff
    {
        public override string Texture => "CalamityFandom/Content/Buffs/Wiki/GreenLizardBuff";
        protected override int PetType => ModContent.ProjectileType<Projectiles.Pets.GreenLizard>();
    }

    /// <summary>[technique] Pas d'icône de buff sur le wiki : celle du lézard de compagnie de Terraria.</summary>
    public class InxieRatZeyBuff : WikiPetBuff
    {
        public override string Texture => "Terraria/Images/Buff_" + BuffID.PetLizard;
        protected override int PetType => ModContent.ProjectileType<Projectiles.Pets.InxieRatZey>();
    }

    public class CynthianSerpentBuff : WikiPetBuff
    {
        public override string Texture => "CalamityFandom/Content/Buffs/Wiki/CynthianSerpentBuff";
        protected override int PetType => ModContent.ProjectileType<Projectiles.Pets.CynthianSerpent>();
    }
}

namespace CalamityFandom.Content.Projectiles.Pets
{
    /// <summary>Familier du wiki (image fixe, retournée selon le sens) : au sol ou volant.</summary>
    public abstract class WikiPet : ModProjectile
    {
        protected abstract int PetBuff { get; }
        protected abstract bool Flying { get; }
        protected virtual bool Dashes => false;

        public override void SetStaticDefaults()
        {
            Main.projPet[Type] = true;
            ProjectileID.Sets.CharacterPreviewAnimations[Type] = ProjectileID.Sets.SimpleLoop(0, 1);
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            Projectile.tileCollide = !Flying;
            Projectile.ignoreWater = true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!owner.active || owner.dead || !owner.HasBuff(PetBuff))
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 2;
            Vector2 target = owner.Center + new Vector2(-owner.direction * 50f, Flying ? -50f : 0f);
            float dist = Vector2.Distance(Projectile.Center, owner.Center);
            if (dist > 1600f)
                Projectile.Center = owner.Center;
            // Au sol, il vole aussi quand il est trop loin (comme les familiers de Terraria)
            bool fly = Flying || dist > 500f || Projectile.ai[0] == 1f;
            Projectile.tileCollide = !fly;
            if (fly)
            {
                Projectile.ai[0] = !Flying && dist < 120f && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height) ? 0f : (Flying ? 0f : 1f);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, (target - Projectile.Center) * 0.08f, 0.15f);
                if (Projectile.velocity.Length() > 16f)
                    Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 16f;
            }
            else
            {
                float dx = target.X - Projectile.Center.X;
                float speed = Math.Abs(dx) > 40f ? Math.Min(8f, Math.Abs(dx) * 0.05f + 2f) : 0f;
                if (Dashes && Math.Abs(dx) > 250f && Projectile.localAI[0] <= 0f)
                {
                    Projectile.velocity.X = Math.Sign(dx) * 14f;
                    Projectile.localAI[0] = 90f;
                }
                else
                    Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, Math.Sign(dx) * speed, 0.1f);
                if (Projectile.localAI[0] > 0f)
                    Projectile.localAI[0]--;
                Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.4f, 10f);
                bool grounded = Collision.SolidCollision(Projectile.BottomLeft, Projectile.width, 4);
                bool blocked = Collision.SolidCollision(Projectile.position + new Vector2(Math.Sign(dx) * 8f, 0f), Projectile.width, Projectile.height - 4);
                if (grounded && (blocked || target.Y < Projectile.Center.Y - 60f))
                    Projectile.velocity.Y = -8f;
            }
            if (Math.Abs(Projectile.velocity.X) > 0.3f)
                Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
            Projectile.rotation = fly && !Flying ? Projectile.velocity.X * 0.05f : Flying ? Projectile.velocity.X * 0.03f : 0f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Vector2 pos = Projectile.Bottom - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Main.EntitySpriteDraw(tex, pos, null, lightColor, Projectile.rotation, new Vector2(tex.Width / 2f, tex.Height), Projectile.scale,
                Projectile.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            return false;
        }
    }

    public class GreenLizard : WikiPet
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Pets/GreenLizard";
        protected override int PetBuff => ModContent.BuffType<Buffs.GreenLizardBuff>();
        protected override bool Flying => false;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.scale = 0.6f;
        }
    }

    public class InxieRatZey : WikiPet
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Pets/InxieRatZey";
        protected override int PetBuff => ModContent.BuffType<Buffs.InxieRatZeyBuff>();
        protected override bool Flying => false;
        protected override bool Dashes => true;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 24;
            Projectile.height = 40;
        }
    }

    public class CynthianSerpent : WikiPet
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Pets/CynthianSerpent";
        protected override int PetBuff => ModContent.BuffType<Buffs.CynthianSerpentBuff>();
        protected override bool Flying => true;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.scale = 0.7f;
        }
    }
}
