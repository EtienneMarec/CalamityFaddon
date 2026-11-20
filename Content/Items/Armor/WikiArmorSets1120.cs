using System;
using System.Linq;
using CalamityFandom.Common.Debug;
using CalamityFandom.Content.Projectiles.Weapons1119f;
using CalamityMod;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

// Lot AW (20/11) : Hornet Round armor (recettes du wiki : 22 Hornet Rounds, Klepticoins, Soul of Plight, Scoria Bars).

namespace CalamityFandom.Content.Items.Armor
{
    /// <summary>
    /// Wiki : Hornet Round Armor.txt — 122 défense ; casque : +100 % distance, +80 vie et mana, toutes les balles deviennent
    /// des 22 Hornet Rounds ; plastron : +100 % invocation, +80 vie et mana, +6 sbires ; jambières : +60 vie et mana, +50 %
    /// de vitesse. Ensemble : +2 de régénération à l'arrêt, 22 nains auriques (les petits foncent, les grands tirent des 22
    /// Hornet Rounds) ; touche de bonus d'armure : on devient un frelon qui vole sans fin, très vite, et les nains partent.
    /// [technique] Rendu porté provisoire : l'armure Auric Tesla de Calamity (le wiki n'a que les icônes, docs/SPRITES.md).
    /// [choix] Nains : 300 dégâts d'invocation de base, sans place ; frelon : 20 px par image dans la direction des touches.
    /// [technique] La forme de frelon n'est pas synchronisée en multijoueur (les autres voient le joueur normal).
    /// </summary>
    public abstract class HornetRoundPiece : StandInArmor
    {
        protected override int Rarity => ModContent.RarityType<BurnishedAuric>();
        protected override int SellCopper => 220000;
    }

    public class HornetRoundHelm : HornetRoundPiece
    {
        protected override EquipType Slot => EquipType.Head;
        protected override string Worn => "Auric/AuricTeslaHeadRanged";
        protected override int Defense => 40;

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<RangedDamageClass>() += 1f;
            player.statLifeMax2 += 80;
            player.statManaMax2 += 80;
            player.GetModPlayer<HornetRoundPlayer>().Helm = true;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
            => body.type == ModContent.ItemType<HornetRoundMantleplate>() && legs.type == ModContent.ItemType<HornetRoundLeggings>();

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = Language.GetTextValue("Mods.CalamityFandom.WikiArmor.HornetRound");
            player.GetModPlayer<HornetRoundPlayer>().Set = true;
        }
    }

    public class HornetRoundMantleplate : HornetRoundPiece
    {
        protected override EquipType Slot => EquipType.Body;
        protected override string Worn => "Auric/AuricTeslaBodyArmor";
        protected override int Defense => 60;

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<SummonDamageClass>() += 1f;
            player.statLifeMax2 += 80;
            player.statManaMax2 += 80;
            player.maxMinions += 6;
        }
    }

    public class HornetRoundLeggings : HornetRoundPiece
    {
        protected override EquipType Slot => EquipType.Legs;
        protected override string Worn => "Auric/AuricTeslaCuisses";
        protected override int Defense => 22;

        public override void UpdateEquip(Player player)
        {
            player.statLifeMax2 += 60;
            player.statManaMax2 += 60;
            player.moveSpeed += 0.5f;
        }
    }

    public class HornetRoundPlayer : ModPlayer
    {
        public bool Helm, Set, Hornet;

        public override void ResetEffects() => Helm = Set = false;

        public override void PostUpdateEquips()
        {
            if (!Set)
            {
                Hornet = false;
                return;
            }
            if (Player.whoAmI == Main.myPlayer && CalamityKeybinds.ArmorSetBonusHotKey?.JustPressed == true)
            {
                Hornet = !Hornet;
                SoundEngine.PlaySound(Hornet ? SoundID.Item97 : SoundID.Item8, Player.Center);
            }
            if (Player.velocity.Length() < 0.5f)
                Player.lifeRegen += 2;
            if (Hornet)
            {
                Player.noFallDmg = true;
                return;
            }
            // 22 nains : 11 petits qui foncent, 11 grands qui tirent
            if (Player.whoAmI != Main.myPlayer)
                return;
            Player.AddBuff(ModContent.BuffType<Buffs.AuricDwarfsBuff>(), 2);
            int dwarf = ModContent.ProjectileType<Projectiles.Summon.AuricDwarf>();
            int count = Player.ownedProjectileCounts[dwarf];
            for (int i = count; i < 22; i++)
            {
                int damage = (int)Player.GetTotalDamage<SummonDamageClass>().ApplyTo(300);
                int p = Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, dwarf, damage, 2f, Player.whoAmI, i % 2, i);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].originalDamage = 300;
            }
            Player.ownedProjectileCounts[dwarf] = 22;
        }

        public override void PreUpdateMovement()
        {
            if (!Hornet)
                return;
            // Frelon : vol sans fin, rapide, dans la direction des touches
            Vector2 input = new((Player.controlRight ? 1 : 0) - (Player.controlLeft ? 1 : 0), (Player.controlDown ? 1 : 0) - (Player.controlUp ? 1 : 0));
            Vector2 want = input == Vector2.Zero ? Vector2.Zero : input.SafeNormalize(Vector2.Zero) * 20f;
            Player.velocity = Vector2.Lerp(Player.velocity, want, 0.2f);
            Player.fallStart = (int)(Player.position.Y / 16f);
            if (input.X != 0)
                Player.direction = Math.Sign(input.X);
        }

        public override void PostUpdateRunSpeeds()
        {
            if (Hornet)
            {
                Player.gravity = 0f;
                Player.maxFallSpeed = 20f;
            }
        }

        public override void ModifyShootStats(Item item, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (Helm && item.useAmmo == AmmoID.Bullet)
                type = ModContent.ProjectileType<HornetRoundBullet>();
        }

        public override void HideDrawLayers(PlayerDrawSet drawInfo)
        {
            if (!Hornet)
                return;
            foreach (var layer in PlayerDrawLayerLoader.Layers)
                if (layer is not HornetFormLayer)
                    layer.Hide();
        }
    }

    /// <summary>Le joueur en frelon (image « Hornet » de la page).</summary>
    public class HornetFormLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Wings);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.GetModPlayer<HornetRoundPlayer>().Hornet;

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player p = drawInfo.drawPlayer;
            Texture2D tex = ModContent.Request<Texture2D>("CalamityFandom/Content/Projectiles/Summon/HornetForm").Value;
            Vector2 pos = p.Center - Main.screenPosition + new Vector2(0f, p.gfxOffY);
            drawInfo.DrawDataCache.Add(new DrawData(tex, pos.Floor(), null, Lighting.GetColor(p.Center.ToTileCoordinates()), p.velocity.X * 0.03f, tex.Size() / 2f, 1f,
                p.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally));
        }
    }

    public class WikiArmorSets1120Checks : ModSystem
    {
        public override void Load()
        {
            SelfTest.ExtraChecks.Add(t =>
            {
                foreach (int type in new[] { ModContent.ItemType<HornetRoundHelm>(), ModContent.ItemType<HornetRoundMantleplate>(), ModContent.ItemType<HornetRoundLeggings>() })
                    t.Expect(Main.recipe.Take(Recipe.numRecipes).Any(r => r.createItem.type == type && !r.Disabled), $"recette du wiki de {Lang.GetItemNameValue(type)}");
            });
        }
    }
}

namespace CalamityFandom.Content.Buffs
{
    public class AuricDwarfsBuff : ModBuff
    {
        public override string Texture => "CalamityFandom/Content/Buffs/Wiki/AuricDwarfsBuff";

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Summon
{
    /// <summary>Nain aurique (Hornet Round armor) : ai[0] = 0 petit (fonce), 1 grand (tire des 22 Hornet Rounds) ; ai[1] = place dans le cercle.</summary>
    public class AuricDwarf : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Summon/AuricDwarfShort";

        private bool Tall => Projectile.ai[0] == 1f;

        public override void SetStaticDefaults()
        {
            Main.projPet[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.scale = 0.6f;
        }

        public override bool? CanDamage() => Tall ? false : null;

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            var hp = owner.GetModPlayer<Items.Armor.HornetRoundPlayer>();
            if (!owner.active || owner.dead || !hp.Set || hp.Hornet)
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 2;
            NPC target = Projectile.Center.MinionHoming(1000f, owner);
            float angle = Projectile.ai[1] / 22f * MathHelper.TwoPi + Main.GlobalTimeWrappedHourly;
            Vector2 home = owner.Center + angle.ToRotationVector2() * (Tall ? 140f : 90f);
            if (target == null || Tall)
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, (home - Projectile.Center) * 0.15f, 0.2f);
            else if (++Projectile.localAI[0] % 30 == Projectile.ai[1] % 30)
                Projectile.velocity = Projectile.SafeDirectionTo(target.Center) * 18f;
            else
                Projectile.velocity *= 0.97f;
            if (Tall && target != null && Projectile.owner == Main.myPlayer && ++Projectile.localAI[1] % 45 == Projectile.ai[1] % 45)
            {
                int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.SafeDirectionTo(target.Center) * 16f,
                    ModContent.ProjectileType<HornetRoundBullet>(), Projectile.damage, 1f, Projectile.owner);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].DamageType = DamageClass.Summon;
            }
            if (Projectile.Distance(owner.Center) > 2000f)
                Projectile.Center = owner.Center;
            Projectile.spriteDirection = (target?.Center.X ?? owner.Center.X) > Projectile.Center.X ? 1 : -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Tall ? "CalamityFandom/Content/Projectiles/Summon/AuricDwarfTall" : Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor, 0f, tex.Size() / 2f, Projectile.scale,
                Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
            return false;
        }
    }
}
