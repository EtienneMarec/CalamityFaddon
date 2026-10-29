// Sprite et nom repris de Calamity Mod (objet retiré en 2022), © Azafure, LLC — https://github.com/CalamityTeam/CalamityModPublic
// L'effet est celui de la page du wiki (décision du 30/10), écrit pour ce mod.
using System;
using CalamityMod;
using CalamityMod.Cooldowns;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Legacy.Items.Accessories
{
    /// <summary>
    /// Wiki : Arcanum of the Void.txt — covering Expert du sac du Ceaseless Void : une barrière du vide entoure le joueur
    /// et bloque un coup ; elle se brise alors en une explosion et libère des énergies sombres à tête chercheuse (4 s,
    /// ~200 DPS au total). 90 s de recharge partagée avec les autres esquives (recharge GlobalDodge de Calamity).
    /// [choix] Chiffres : explosion 200 dégâts sur 160 px ; 4 énergies de 25 dégâts qui touchent toutes les 0,5 s
    /// (4 × 50 = 200 DPS) ; dégâts génériques (bonus de toutes les classes).
    /// </summary>
    public class ArcanumoftheVoid : ModItem
    {
        public const int Cooldown = 90 * 60;

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 30;
            Item.accessory = true;
            Item.expert = true;
            Item.rare = ItemRarityID.Expert;
            Item.value = Item.sellPrice(gold: 24);
        }

        public override void UpdateAccessory(Player player, bool hideVisual) => player.GetModPlayer<ArcanumPlayer>().Equipped = true;
    }

    public class ArcanumPlayer : ModPlayer
    {
        public bool Equipped;

        private bool Ready => Equipped && !Player.HasCooldown(GlobalDodge.ID) && !Player.Calamity().disableAllDodges;

        public override void ResetEffects() => Equipped = false;

        public override void PostUpdateEquips()
        {
            // Barrière visible tant qu'elle est prête
            if (!Ready || Main.dedServ || !Main.rand.NextBool(2))
                return;
            Vector2 offset = Main.rand.NextVector2CircularEdge(34f, 40f);
            var dust = Dust.NewDustPerfect(Player.Center + offset, DustID.Shadowflame, -offset * 0.02f, 150, default, 1.1f);
            dust.noGravity = true;
        }

        public override bool FreeDodge(Player.HurtInfo info)
        {
            if (!Ready)
                return false;
            Player.AddCooldown(GlobalDodge.ID, ArcanumoftheVoid.Cooldown);
            Player.SetImmuneTimeForAllTypes(Player.longInvince ? 100 : 60);
            SoundEngine.PlaySound(SoundID.Item14, Player.Center);
            for (int i = 0; i < 40; i++)
            {
                var dust = Dust.NewDustPerfect(Player.Center, DustID.Shadowflame, Main.rand.NextVector2Circular(8f, 8f), 100, default, 1.6f);
                dust.noGravity = true;
            }
            if (Player.whoAmI == Main.myPlayer)
            {
                var source = Player.GetSource_Accessory(new Item(ModContent.ItemType<ArcanumoftheVoid>()));
                int boom = (int)Player.GetTotalDamage(DamageClass.Generic).ApplyTo(200);
                Projectile.NewProjectile(source, Player.Center, Vector2.Zero, ModContent.ProjectileType<VoidBarrierBurst>(), boom, 6f, Player.whoAmI);
                int energy = (int)Player.GetTotalDamage(DamageClass.Generic).ApplyTo(25);
                for (int i = 0; i < 4; i++)
                    Projectile.NewProjectile(source, Player.Center, (MathHelper.PiOver2 * i + MathHelper.PiOver4).ToRotationVector2() * 8f,
                        ModContent.ProjectileType<VoidDarkEnergy>(), energy, 2f, Player.whoAmI);
            }
            return true;
        }
    }

    /// <summary>Le sac du Ceaseless Void contient l'Arcanum of the Void (page de l'objet : Expert, 100 %).</summary>
    public class ArcanumBagLoot : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
            => ModContent.TryFind<ModItem>("CalamityMod", "CeaselessVoidBag", out var bag) && entity.type == bag.Type;

        public override void ModifyItemLoot(Item item, ItemLoot itemLoot) => itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<ArcanumoftheVoid>()));
    }

    /// <summary>Explosion de la barrière (160 px, invisible, un coup par ennemi).</summary>
    public class VoidBarrierBurst : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 160;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 6;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
    }

    /// <summary>Énergie sombre de l'Arcanum of the Void : cherche les ennemis pendant 4 s, touche toutes les 0,5 s.</summary>
    public class VoidDarkEnergy : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 240;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.4f, 0f, 0.6f);
            for (int i = 0; i < 2; i++)
            {
                var dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(6f, 6f), DustID.Shadowflame, Vector2.Zero, 100, default, 1.3f);
                dust.noGravity = true;
            }
            if (Projectile.timeLeft < 225)
                CalamityUtils.HomeInOnNPC(Projectile, true, 800f, 12f, 20f);
        }
    }
}
