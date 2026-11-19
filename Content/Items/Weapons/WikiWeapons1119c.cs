using System;
using CalamityFandom.Content.Legacy.Projectiles.Melee;
using CalamityFandom.Content.Projectiles.Weapons1119c;
using CalamityMod;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

// Armes inventées par le wiki (19/11, suite) : sprites et valeurs des pages, recettes du wiki (WikiRecipes).

namespace CalamityFandom.Content.Items.Weapons.Summon
{
    /// <summary>
    /// Wiki : Catastrophe Claysiphonophore.txt — lance un des trois siphonophores d'énergie (rouge On Fire! 2 s, bleu
    /// Frostburn 1,5 s, jaune Ichor 1 s) qui ne prennent pas de place de sbire mais disparaissent avec le temps.
    /// [technique] Les éclairs de couleur de la Catastrophe Clay (mêmes projectiles que la Clayspore), en dégâts d'invocation.
    /// </summary>
    public class CatastropheClaysiphonophore : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 47;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 8;
            Item.knockBack = 4f;
            Item.useTime = Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item44;
            Item.width = 24;
            Item.height = 58;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.sellPrice(gold: 9, silver: 60);
            Item.shoot = ModContent.ProjectileType<CalamityAura>();
            Item.shootSpeed = 7f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int p = Projectile.NewProjectile(source, position, velocity, CatastropheBolts.Random(), damage, knockback, player.whoAmI);
            if (Main.projectile.IndexInRange(p))
                Main.projectile[p].DamageType = DamageClass.Summon;
            return false;
        }
    }

    /// <summary>
    /// Wiki : Virid Vangar.txt — un gar collé au dos du joueur qui taillade les ennemis à moins de 10 blocs devant lui
    /// (Poisoned). [choix] Un coup toutes les 20 images, Poisoned 4 s.
    /// </summary>
    public class ViridVangar : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.GamepadWholeScreenUseRange[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 280;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 10;
            Item.useTime = Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.UseSound = SoundID.Item44;
            Item.width = 42;
            Item.height = 60;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.value = Item.sellPrice(gold: 24);
            Item.buffType = ModContent.BuffType<Buffs.ViridVangarBuff>();
            Item.shoot = ModContent.ProjectileType<VangarBlades>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            // Un seul gar (page : « a singular gar »)
            if (player.ownedProjectileCounts[type] > 0)
                return false;
            int p = Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI);
            if (Main.projectile.IndexInRange(p))
                Main.projectile[p].originalDamage = Item.damage;
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Rogue
{
    /// <summary>
    /// Wiki : Pastranion.txt — lance un bol d'Argentum d'où partent des filaments d'énergie qui fouettent les ennemis à moins
    /// de 20 blocs ; frappe furtive : filaments bien plus longs et plus rapides.
    /// </summary>
    public class Pastranion : RogueWeapon
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 413;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.useTime = Item.useAnimation = 28;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = Item.noUseGraphic = true;
            Item.knockBack = 5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = 56;
            Item.height = 50;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
            Item.value = Item.sellPrice(gold: 30);
            Item.shoot = ModContent.ProjectileType<PastranionBowl>();
            Item.shootSpeed = 18f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (player.Calamity().StealthStrikeAvailable() && Main.projectile.IndexInRange(p))
                Main.projectile[p].Calamity().stealthStrike = true;
            return false;
        }
    }

    /// <summary>
    /// Wiki : Vermillion Angel.txt — lance très rapide qui traverse les blocs et sème des projectiles qui cherchent les
    /// ennemis ; au premier ennemi, une Vermillion Blast qui frappe tout autour plusieurs fois ; frappe furtive : un
    /// hexagramme de projectiles en plus. 24 % de critique.
    /// </summary>
    public class VermillionAngel : RogueWeapon
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 800;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.useTime = Item.useAnimation = 80;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = Item.noUseGraphic = true;
            Item.knockBack = 9f;
            Item.crit = 20;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = Item.height = 54;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.value = Item.sellPrice(gold: 40);
            Item.shoot = ModContent.ProjectileType<VermillionSpear>();
            Item.shootSpeed = 30f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (player.Calamity().StealthStrikeAvailable() && Main.projectile.IndexInRange(p))
                Main.projectile[p].Calamity().stealthStrike = true;
            return false;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Breakthrough.txt — lance ; clic droit : souffle d'air glacé qui gèle les ennemis (Frozen) ; frapper un ennemi
    /// gelé le libère, lui inflige de lourds dégâts (x3) et fait une petite explosion de glace.
    /// [choix] « Frozen » = Glacial State porté ; dégâts x3.
    /// </summary>
    public class Breakthrough : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
            ItemID.Sets.Spears[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 400;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = Item.noUseGraphic = true;
            Item.knockBack = 4f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = 88;
            Item.height = 86;
            Item.rare = ModContent.RarityType<CosmicPurple>();
            Item.value = Item.sellPrice(gold: 50);
            Item.shoot = ModContent.ProjectileType<BreakthroughSpear>();
            Item.shootSpeed = 12f;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                SoundEngine.PlaySound(SoundID.Item34, position);
                for (int i = 0; i < 3; i++)
                    Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.2f) * 0.9f, ModContent.ProjectileType<FrigidGust>(), damage / 4, 0f, player.whoAmI);
                return false;
            }
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }
    }

    /// <summary>
    /// Wiki : Summit Breaker.txt — épée ; clic droit : la lancer, elle se plante dans ce qu'elle touche ; clic droit près
    /// d'elle : l'arracher (gros dégâts si elle était plantée dans un ennemi), puis 20 s avant de pouvoir la relancer.
    /// [choix] Arracher = x4 dégâts ; la traînée de plasma de l'animation n'est pas faite.
    /// </summary>
    public class SummitBreaker : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 650;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 20f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = 80;
            Item.height = 70;
            Item.rare = ModContent.RarityType<CosmicPurple>();
            Item.value = Item.sellPrice(gold: 50);
            Item.shootSpeed = 20f;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            int type = ModContent.ProjectileType<SummitBreakerThrown>();
            if (player.altFunctionUse != 2)
            {
                Item.useStyle = ItemUseStyleID.Swing;
                Item.noMelee = Item.noUseGraphic = false;
                Item.shoot = ProjectileID.None;
                return player.ownedProjectileCounts[type] == 0;
            }
            // Clic droit : arracher l'épée plantée si on est près d'elle, sinon la lancer (après les 20 s d'attente)
            foreach (var p in Main.ActiveProjectiles)
                if (p.owner == player.whoAmI && p.type == type)
                {
                    if (p.ai[0] != 0f && player.Distance(p.Center) < 160f && player.whoAmI == Main.myPlayer)
                        ((SummitBreakerThrown)p.ModProjectile).Rend();
                    return false;
                }
            if (player.HasBuff(BuffID.ChaosState) || player.GetModPlayer<SummitPlayer>().Cooldown > 0)
                return false;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = Item.noUseGraphic = true;
            Item.shoot = type;
            return true;
        }
    }

    public class SummitPlayer : ModPlayer
    {
        public int Cooldown;

        public override void PostUpdate()
        {
            if (Cooldown > 0)
                Cooldown--;
        }
    }
}

namespace CalamityFandom.Content.Buffs
{
    public class ViridVangarBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<VangarBlades>()] > 0)
                player.buffTime[buffIndex] = 18000;
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Weapons1119c
{
    /// <summary>Gar du Virid Vangar : collé au dos du joueur, taillade (Poisoned 4 s) un ennemi à moins de 10 blocs devant lui.</summary>
    public class VangarBlades : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Summon/VangarBlades";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
            Main.projPet[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 1f;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 18000;
            Projectile.scale = 0.5f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!owner.active || owner.dead || !owner.HasBuff<Buffs.ViridVangarBuff>())
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 2;
            NPC target = null;
            foreach (var n in Main.ActiveNPCs)
                if (n.CanBeChasedBy(Projectile) && owner.Distance(n.Center) < 160f && Math.Sign(n.Center.X - owner.Center.X) == owner.direction)
                {
                    target = n;
                    break;
                }
            Vector2 back = owner.Center + new Vector2(-owner.direction * 16f, -6f);
            if (target != null)
            {
                // Taillade : file vers l'ennemi puis revient au dos
                Projectile.ai[0]++;
                float t = Projectile.ai[0] % 20f / 20f;
                Projectile.Center = Vector2.Lerp(back, target.Center, (float)Math.Sin(t * Math.PI));
                Projectile.rotation += 0.6f * owner.direction;
            }
            else
            {
                Projectile.ai[0] = 0f;
                Projectile.Center = back;
                Projectile.rotation = owner.direction * 0.4f;
            }
            Projectile.spriteDirection = owner.direction;
        }

        public override bool? CanDamage() => Projectile.ai[0] > 0f ? null : false;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Poisoned, 240);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            SpriteEffects fx = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, fx);
            return false;
        }
    }

    /// <summary>Bol du Pastranion : ralentit, fouette toutes les 20 images (8 en frappe furtive) un ennemi à moins de 20 blocs (40 en frappe furtive).</summary>
    public class PastranionBowl : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Rogue/Pastranion";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.scale = 0.6f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.96f;
            Projectile.rotation += 0.15f;
            bool stealth = Projectile.Calamity().stealthStrike;
            if (Projectile.owner != Main.myPlayer || ++Projectile.ai[0] < (stealth ? 8f : 20f))
                return;
            NPC target = Projectile.Center.ClosestNPCAt(stealth ? 640f : 320f);
            if (target == null)
                return;
            Projectile.ai[0] = 0f;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<PastranionStrand>(),
                Projectile.damage / 2, 0f, Projectile.owner, target.whoAmI);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = Vector2.Zero;
            return false;
        }
    }

    /// <summary>Filament du Pastranion : relie le bol à l'ennemi pendant 8 images et le frappe.</summary>
    public class PastranionStrand : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 8;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            NPC target = Main.npc[(int)Projectile.ai[0]];
            if (!target.active)
            {
                Projectile.Kill();
                return;
            }
            Vector2 from = Projectile.Center;
            Projectile.Center = target.Center;
            if (Main.dedServ || Projectile.localAI[0] != 0f)
                return;
            Projectile.localAI[0] = 1f;
            for (float f = 0f; f < 1f; f += 0.04f)
                Dust.NewDustPerfect(Vector2.Lerp(from, target.Center, f), DustID.Clentaminator_Cyan, Vector2.Zero, 100, default, 0.9f).noGravity = true;
        }
    }

    /// <summary>Lance de la Vermillion Angel : traverse les blocs, sème des éclats chercheurs, Vermillion Blast au premier ennemi.</summary>
    public class VermillionSpear : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Rogue/VermillionAngel";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 120;
            Projectile.extraUpdates = 2;
            Projectile.scale = 0.6f;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            if (Projectile.owner == Main.myPlayer && ++Projectile.ai[0] % 12f == 0f)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Circular(3f, 3f),
                    ModContent.ProjectileType<VermillionShard>(), Projectile.damage / 5, 0f, Projectile.owner);
            Dust.NewDustPerfect(Projectile.Center, DustID.CrimsonTorch, Vector2.Zero, 100, default, 1.4f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.owner != Main.myPlayer)
                return;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<VermillionBlast>(), Projectile.damage / 4, 0f, Projectile.owner);
            if (Projectile.Calamity().stealthStrike)
                // Hexagramme : 12 éclats en deux triangles... en étoile
                for (int i = 0; i < 12; i++)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center + (MathHelper.TwoPi * i / 12f).ToRotationVector2() * 160f,
                        Vector2.Zero, ModContent.ProjectileType<VermillionShard>(), Projectile.damage / 3, 0f, Projectile.owner);
        }
    }

    /// <summary>Éclat vermillon : immobile un instant puis cherche les ennemis, traverse les blocs, un seul coup.</summary>
    public class VermillionShard : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.CrystalShard;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 150;
        }

        public override void AI()
        {
            if (++Projectile.ai[0] > 20f)
                CalamityUtils.HomeInOnNPC(Projectile, true, 700f, 16f, 15f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 60, 60, 220);
    }

    /// <summary>Vermillion Blast : zone de 160 px qui frappe tous les 6 images pendant 30 images.</summary>
    public class VermillionBlast : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 160;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 30;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
        }

        public override void AI()
        {
            if (Main.dedServ)
                return;
            for (int i = 0; i < 6; i++)
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(80f, 80f), DustID.CrimsonTorch, Vector2.Zero, 100, default, 1.8f).noGravity = true;
        }
    }

    /// <summary>Lance de Breakthrough tenue (comme une lance vanilla) ; frappe x3 un ennemi gelé, le dégèle et fait une explosion de glace.</summary>
    public class BreakthroughSpear : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Melee/BreakthroughSpear";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.aiStyle = ProjAIStyleID.Spear;
            AIType = ProjectileID.Trident;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.hide = true;
            Projectile.scale = 1.1f;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.HasBuff<Legacy.Buffs.StatDebuffs.GlacialState>())
                modifiers.SourceDamage *= 3f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int frozen = target.FindBuffIndex(ModContent.BuffType<Legacy.Buffs.StatDebuffs.GlacialState>());
            if (frozen < 0)
                return;
            target.DelBuff(frozen);
            SoundEngine.PlaySound(SoundID.Item27, target.Center);
            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<IceShatter>(), hit.Damage / 4, 0f, Projectile.owner);
        }
    }

    /// <summary>Souffle glacé de Breakthrough : Glacial State 2 s.</summary>
    public class FrigidGust : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 50;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.97f;
            if (!Main.dedServ)
                for (int i = 0; i < 2; i++)
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.IceTorch, Projectile.velocity.X * 0.3f, Projectile.velocity.Y * 0.3f, 100, default, 1.4f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Legacy.Buffs.StatDebuffs.GlacialState>(), 120);
    }

    /// <summary>Petite explosion de glace (ennemi gelé brisé).</summary>
    public class IceShatter : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 90;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 5;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Main.dedServ)
                return;
            for (int i = 0; i < 25; i++)
                Dust.NewDustPerfect(Projectile.Center, DustID.Ice, Main.rand.NextVector2Circular(6f, 6f), 100, default, 1.4f);
        }
    }

    /// <summary>
    /// Summit Breaker lancée : se plante dans le premier ennemi ou bloc touché (ai[0] = 1 bloc, 2 + index ennemi) ;
    /// <see cref="Rend"/> l'arrache (x4 dégâts si dans un ennemi) et lance 20 s d'attente.
    /// </summary>
    public class SummitBreakerThrown : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Melee/SummitBreaker";

        private Vector2 offset;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 3600;
            Projectile.scale = 0.7f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0f)
            {
                Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.15f, 16f);
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
                return;
            }
            Projectile.velocity = Vector2.Zero;
            if (Projectile.ai[0] >= 2f)
            {
                NPC npc = Main.npc[(int)Projectile.ai[0] - 2];
                if (!npc.active)
                    Projectile.ai[0] = 1f;
                else
                    Projectile.Center = npc.Center + offset;
            }
            if (Main.rand.NextBool(6))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleTorch, 0f, 0f, 100, default, 0.9f).noGravity = true;
        }

        public override bool? CanDamage() => Projectile.ai[0] == 0f ? null : false;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[0] != 0f)
                return;
            Projectile.ai[0] = 2f + target.whoAmI;
            offset = Projectile.Center - target.Center;
            Projectile.netUpdate = true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.ai[0] == 0f)
            {
                Projectile.ai[0] = 1f;
                SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            }
            return false;
        }

        public void Rend()
        {
            Player owner = Main.player[Projectile.owner];
            if (Projectile.ai[0] >= 2f)
            {
                NPC npc = Main.npc[(int)Projectile.ai[0] - 2];
                if (npc.active)
                {
                    SoundEngine.PlaySound(SoundID.Item71, npc.Center);
                    owner.ApplyDamageToNPC(npc, Projectile.damage * 4, 10f, owner.direction, false, DamageClass.Melee);
                }
            }
            owner.GetModPlayer<Items.Weapons.Melee.SummitPlayer>().Cooldown = 1200;
            Projectile.Kill();
        }
    }
}
