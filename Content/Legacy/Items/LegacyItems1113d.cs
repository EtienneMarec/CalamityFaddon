// Adapté de Calamity Mod 2.0.1.005, © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// SPDX-FileCopyrightText: Azafure, LLC
// SPDX-FileCopyrightText: 2026 Etienne Marec (adaptation)
// SPDX-License-Identifier: LicenseRef-Calamity
using CalamityFandom.Content.Legacy.Items.Fishing;
using CalamityFandom.Content.Legacy.Projectiles.Ranged;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

// Lot du 13/11 (suite) : Fish of Light, Fish of Night (poissons à âmes), Molecular Manipulator. Code de 2.0.1.005,
// valeurs des infobox et du texte des pages.

namespace CalamityFandom.Content.Legacy.Items.Fishing
{
    /// <summary>Poisson à ouvrir (clic droit) : 2 à 5 âmes.</summary>
    public abstract class SoulFish : ModItem
    {
        protected abstract int Soul { get; }

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 10;
            ItemID.Sets.CanBePlacedOnWeaponRacks[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.width = Item.height = 34;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup) =>
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.GoodieBags;

        public override bool CanRightClick() => true;

        public override void ModifyItemLoot(ItemLoot itemLoot) => itemLoot.Add(ItemDropRule.Common(Soul, 1, 2, 5));
    }

    /// <summary>Wiki : Fish of Light.txt — pêché dans le Hallow souterrain en Hardmode ; 2 à 5 Souls of Light.</summary>
    public class FishofLight : SoulFish
    {
        protected override int Soul => ItemID.SoulofLight;
    }

    /// <summary>Wiki : Fish of Night.txt — pêché dans la Corruption ou le Crimson souterrains en Hardmode ; 2 à 5 Souls of Night.</summary>
    public class FishofNight : SoulFish
    {
        protected override int Soul => ItemID.SoulofNight;
    }

    /// <summary>
    /// Pêche des deux poissons (règle de 2.0.1.005) : prise peu commune, une chance sur sept, dans l'eau, jamais à la place
    /// d'une caisse, d'un poisson de quête ou d'une créature.
    /// </summary>
    public class SoulFishCatch : ModPlayer
    {
        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            if (attempt.inLava || attempt.inHoney || attempt.crate || npcSpawn > 0 || itemDrop == attempt.questFish
                || !Main.hardMode || !Player.ZoneRockLayerHeight || !attempt.uncommon || !Main.rand.NextBool(7))
                return;
            if (Player.ZoneHallow)
                itemDrop = ModContent.ItemType<FishofLight>();
            else if (Player.ZoneCorrupt || Player.ZoneCrimson)
                itemDrop = ModContent.ItemType<FishofNight>();
        }
    }
}

namespace CalamityFandom.Content.Legacy.Items.Weapons.Ranged
{
    /// <summary>
    /// Wiki : Molecular Manipulator.txt — balle nulle rapide qui change au hasard une caractéristique de l'ennemi touché
    /// (sauf boss) : taille x5 ou x0,1, dégâts de contact ±20, recul nul ou total, défense ±10, poussée vers le haut ou
    /// sur le côté.
    /// </summary>
    public class MolecularManipulator : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 580;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 56;
            Item.height = 34;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 8f;
            Item.value = Item.sellPrice(gold: 24);
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.UseSound = SoundID.Item33;
            Item.autoReuse = true;
            Item.shootSpeed = 25f;
            Item.shoot = ModContent.ProjectileType<NullShot2>();
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.Ranged
{
    public class NullShot2 : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        // Poussée de la page : 1,875 bloc par seconde (0,5 px par image), ajoutée à la vitesse
        private const float Push = 0.5f;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            if (++Projectile.localAI[0] <= 3f || Main.dedServ)
                return;
            for (int i = 0; i < 10; i++)
            {
                Vector2 pos = Projectile.position - Projectile.velocity / 10f * i;
                Dust d = Dust.NewDustDirect(pos, 1, 1, DustID.RainbowTorch, 0f, 0f, 0, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB));
                d.position = pos;
                d.velocity = Vector2.Zero;
                d.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.boss || target.type == NPCID.TargetDummy)
                return;
            switch (Main.rand.Next(10))
            {
                case 0: target.scale *= 5f; break;
                case 1: target.scale *= 0.1f; break;
                case 2: target.damage += 20; break;
                case 3: target.damage = System.Math.Max(target.damage - 20, 0); break;
                case 4: target.knockBackResist = 0f; break;
                case 5: target.knockBackResist = 1f; break;
                case 6: target.defense += 10; break;
                case 7: target.defense -= 10; break;
                case 8: target.velocity.Y -= Push; break;
                default: target.velocity.X += Main.rand.NextBool() ? Push : -Push; break;
            }
            target.netUpdate = true;
        }
    }
}
