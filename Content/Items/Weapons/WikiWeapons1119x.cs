using System;
using CalamityFandom.Content.Projectiles.Weapons1119x;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

// Lot AB (19/11) : Quintessence Cannon et Justice and Splendor (recettes du wiki), Divine Intervention (Meteor Head).

namespace CalamityFandom.Content.Items.Weapons.Magic
{
    /// <summary>
    /// Wiki : Quintessence Cannon.txt — comme le Last Prism : deux faisceaux de 6 rayons pastel, l'un serré, l'autre écarté ;
    /// tenus, ils s'inversent en 3 s ; plus un faisceau est serré, plus il fait mal ; à chaque fois qu'un faisceau se resserre
    /// complètement : explosions (350 %, debuffs exo) au bout du rayon, sur les ennemis touchés et sur l'arme ; aucun recul.
    /// [choix] Dégâts x0,5 (écarté) à x1,5 (serré) ; rayons de 1200 px ; l'explosion sur l'arme ne blesse pas le joueur.
    /// </summary>
    public class QuintessenceCannon : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 62;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 20;
            Item.useTime = Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = Item.noUseGraphic = true;
            Item.channel = true;
            Item.knockBack = 0f;
            Item.autoReuse = true;
            Item.width = 34;
            Item.height = 78;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
            Item.value = Item.sellPrice(gold: 30);
            Item.shoot = ModContent.ProjectileType<QuintessenceHoldout>();
            Item.shootSpeed = 30f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;
    }
}

namespace CalamityFandom.Content.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Justice and Splendor.txt — combo de 3 coups alternés (lame bleue, lame dorée), qui donne Holy Flame (+15 % de
    /// vitesse de mêlée, furtivité 5 % plus rapide) 5 s aux coups de vraie mêlée ; juste après le 3e coup, clic droit : ruée
    /// (300 %) qui, si elle touche, renforce les lames 10 s (+25 %) ; furtivité pleine : l'épée est lancée, cherche les
    /// ennemis et explose (500 %) ; renforcée, une copie spectrale reste en main pendant le lancer.
    /// [technique] Coups ordinaires (l'animation propre n'est pas faite) ; la copie spectrale = on peut frapper pendant le lancer.
    /// [choix] La ruée doit suivre le 3e coup de moins d'une seconde ; Holy Flame : la partie furtivité n'est pas faite.
    /// </summary>
    public class JusticeandSplendor : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 632;
            Item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 8.5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = 52;
            Item.height = 58;
            Item.crit = 10;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.value = Item.sellPrice(gold: 62);
            Item.shoot = ProjectileID.None;
            Item.shootSpeed = 16f;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            var mp = player.GetModPlayer<JusticePlayer>();
            Item.noUseGraphic = false;
            Item.noMelee = false;
            Item.shoot = ProjectileID.None;
            Item.useStyle = ItemUseStyleID.Swing;
            if (player.altFunctionUse == 2)
            {
                if (mp.Combo < 3 || mp.ComboTimer <= 0)
                    return false;
                mp.Combo = 0;
                mp.Dash = 15;
                mp.DashDir = Math.Sign(Main.MouseWorld.X - player.Center.X);
                if (mp.DashDir == 0)
                    mp.DashDir = player.direction;
                return true;
            }
            bool thrownOut = player.ownedProjectileCounts[ModContent.ProjectileType<ThrownJustice>()] > 0;
            if (thrownOut && mp.Empowered <= 0)
                return false;
            if (player.Calamity().StealthStrikeAvailable() && !thrownOut)
            {
                Item.shoot = ModContent.ProjectileType<ThrownJustice>();
                Item.noUseGraphic = true;
                Item.noMelee = true;
            }
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int p = Projectile.NewProjectile(source, position, velocity, type, damage * 5, knockback, player.whoAmI);
            if (Main.projectile.IndexInRange(p))
                Main.projectile[p].Calamity().stealthStrike = true;
            player.Calamity().ConsumeStealthByAttacking();
            return false;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            if (player.GetModPlayer<JusticePlayer>().Empowered > 0)
                damage += 0.25f;
        }

        public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (player.GetModPlayer<JusticePlayer>().Dash > 0)
                modifiers.SourceDamage *= 3f;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            var mp = player.GetModPlayer<JusticePlayer>();
            player.AddBuff(ModContent.BuffType<Buffs.HolyFlameBuff>(), 300);
            if (mp.Dash > 0)
            {
                mp.Empowered = 600;
                return;
            }
            if (player.itemAnimation == player.itemAnimationMax - 1 || mp.LastSwing != player.itemAnimationMax)
                mp.RegisterSwing();
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.itemAnimation == player.itemAnimationMax - 1)
                player.GetModPlayer<JusticePlayer>().RegisterSwing();
        }

        public override void ModifyItemScale(Player player, ref float scale)
        {
            if (player.GetModPlayer<JusticePlayer>().Empowered > 0)
                scale *= 1.15f;
        }
    }

    /// <summary>
    /// Wiki : Divine Intervention.txt — épée pré-Hardmode lâchée par les Meteor Heads (5 %, butin de la page) : chaque coup fait
    /// jaillir 3 à 5 étincelles soumises à la gravité (10 %) et appelle une tête de météore hors de l'écran qui cherche les
    /// ennemis (200 %) ; Holy Flame aux coups ; Hell Judgement (3 PV/s, -3 de défense) sur tout ce qu'elle touche ;
    /// furtivité pleine : l'épée est lancée vers le curseur.
    /// [choix] Une seule tête de météore à la fois par joueur ; l'épée lancée revient comme un boomerang (le wiki ne dit rien).
    /// </summary>
    public class DivineIntervention : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 25;
            Item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Item.useTime = Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = 70;
            Item.height = 60;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(silver: 10);
            Item.shoot = ProjectileID.None;
            Item.shootSpeed = 6f;
        }

        public override bool CanUseItem(Player player)
        {
            bool thrown = player.Calamity().StealthStrikeAvailable() && player.ownedProjectileCounts[ModContent.ProjectileType<ThrownDivineIntervention>()] == 0;
            Item.shoot = thrown ? ModContent.ProjectileType<ThrownDivineIntervention>() : ProjectileID.None;
            Item.noUseGraphic = Item.noMelee = thrown;
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int p = Projectile.NewProjectile(source, position, velocity * 2f, type, damage, knockback, player.whoAmI);
            if (Main.projectile.IndexInRange(p))
                Main.projectile[p].Calamity().stealthStrike = true;
            player.Calamity().ConsumeStealthByAttacking();
            return false;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
            => DivineHits.OnHit(player, target, player.GetSource_ItemUse(Item), player.GetWeaponDamage(Item));
    }

    /// <summary>Effets communs aux coups de la Divine Intervention (lame et lancer).</summary>
    public static class DivineHits
    {
        public static void OnHit(Player player, NPC target, IEntitySource source, int damage)
        {
            player.AddBuff(ModContent.BuffType<Buffs.HolyFlameBuff>(), 300);
            target.AddBuff(ModContent.BuffType<Buffs.HellJudgement>(), 300);
            if (player.whoAmI != Main.myPlayer)
                return;
            int sparks = Main.rand.Next(3, 6);
            for (int i = 0; i < sparks; i++)
                Projectile.NewProjectile(source, target.Center, new Vector2(Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-8f, -3f)),
                    ModContent.ProjectileType<FlamingShard>(), Math.Max(1, damage / 10), 0f, player.whoAmI);
            if (player.ownedProjectileCounts[ModContent.ProjectileType<DivineMeteorHead>()] < 1)
            {
                Vector2 from = player.Center + new Vector2(Main.rand.NextBool() ? -1100f : 1100f, -500f);
                Projectile.NewProjectile(source, from, (target.Center - from).SafeNormalize(Vector2.UnitY) * 10f,
                    ModContent.ProjectileType<DivineMeteorHead>(), damage * 2, 2f, player.whoAmI);
            }
        }
    }

    /// <summary>Combo, ruée et renforcement de Justice and Splendor.</summary>
    public class JusticePlayer : ModPlayer
    {
        public int Combo, ComboTimer, Dash, DashDir, Empowered, LastSwing;

        public void RegisterSwing()
        {
            Combo = Math.Min(Combo + 1, 3);
            ComboTimer = 60;
        }

        public override void PostUpdate()
        {
            if (ComboTimer > 0 && --ComboTimer == 0)
                Combo = 0;
            if (Empowered > 0)
            {
                Empowered--;
                if (!Main.dedServ && Main.rand.NextBool(4))
                    Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.GoldFlame, 0f, 0f, 100, default, 1.1f).noGravity = true;
            }
        }

        public override void PreUpdateMovement()
        {
            if (Dash <= 0)
                return;
            Dash--;
            Player.velocity.X = DashDir * 22f;
            Player.immune = true;
            Player.immuneTime = Math.Max(Player.immuneTime, 2);
        }
    }
}

namespace CalamityFandom.Content.Buffs
{
    /// <summary>Holy Flame (Justice and Splendor, Divine Intervention) : +15 % de vitesse de mêlée, furtivité 5 % plus rapide.</summary>
    public class HolyFlameBuff : ModBuff
    {
        public override string Texture => "Terraria/Images/Buff_" + BuffID.Inferno;

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetAttackSpeed(DamageClass.Melee) += 0.15f;
            player.Calamity().stealthGenStandstill += 0.05f;
            player.Calamity().stealthGenMoving += 0.05f;
        }
    }

    /// <summary>Hell Judgement (Divine Intervention) : 3 PV/s, -3 de défense.</summary>
    public class HellJudgement : ModBuff
    {
        public override string Texture => "Terraria/Images/Buff_" + BuffID.OnFire;

        public override void SetStaticDefaults() => Main.debuff[Type] = true;

        public override void Update(NPC npc, ref int buffIndex) => npc.GetGlobalNPC<HellJudgementNPC>().Judged = true;
    }

    public class HellJudgementNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public bool Judged;

        public override void ResetEffects(NPC npc) => Judged = false;

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (!Judged)
                return;
            if (npc.lifeRegen > 0)
                npc.lifeRegen = 0;
            npc.lifeRegen -= 6;
            damage = Math.Max(damage, 1);
        }

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (Judged)
                modifiers.Defense.Flat -= 3f;
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Weapons1119x
{
    /// <summary>Canon tenu de la Quintessence Cannon : deux faisceaux de 6 rayons qui s'inversent en 3 s.</summary>
    public class QuintessenceHoldout : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Magic/QuintessenceHoldout";

        public const float Length = 1200f;
        private bool[] wasConverged = new bool[2];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        /// <summary>Écart du faisceau (0 serré, 1 écarté) au temps donné.</summary>
        public float Spread(int set) => set == 0 ? (float)(0.5 + 0.5 * Math.Cos(Projectile.ai[0] * MathHelper.Pi / 180f)) : (float)(0.5 - 0.5 * Math.Cos(Projectile.ai[0] * MathHelper.Pi / 180f));

        public float BeamAngle(int set, int i) => Projectile.velocity.ToRotation() + (i - 2.5f) * 0.12f * Spread(set);

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            for (int set = 0; set < 2; set++)
                for (int i = 0; i < 6; i++)
                    if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + BeamAngle(set, i).ToRotationVector2() * Length, 10f, ref _))
                        return true;
            return false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // Plus le faisceau est serré, plus il fait mal (le plus serré des deux compte)
            float tight = 1f - Math.Min(Spread(0), Spread(1));
            modifiers.SourceDamage *= 0.5f + tight;
            modifiers.Knockback *= 0f;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!owner.channel || owner.dead || owner.noItems || owner.CCed)
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 2;
            owner.heldProj = Projectile.whoAmI;
            owner.itemTime = owner.itemAnimation = 2;
            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 aim = (Main.MouseWorld - owner.MountedCenter).SafeNormalize(Vector2.UnitX * owner.direction);
                if (aim != Projectile.velocity)
                    Projectile.netUpdate = true;
                Projectile.velocity = aim;
            }
            owner.ChangeDir(Projectile.velocity.X > 0 ? 1 : -1);
            owner.itemRotation = (Projectile.velocity * owner.direction).ToRotation();
            Projectile.Center = owner.MountedCenter + Projectile.velocity * 30f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.ai[0]++;
            if (Projectile.ai[0] % 10f == 0f && !owner.CheckMana(owner.HeldItem, -1, true))
            {
                Projectile.Kill();
                return;
            }
            for (int set = 0; set < 2; set++)
            {
                bool converged = Spread(set) < 0.02f;
                if (converged && !wasConverged[set] && Projectile.owner == Main.myPlayer && Projectile.ai[0] > 10f)
                    Burst(set);
                wasConverged[set] = converged;
            }
        }

        private void Burst(int set)
        {
            SoundEngine.PlaySound(SoundID.Item62 with { Volume = 0.6f }, Projectile.Center);
            int damage = (int)(Projectile.damage * 3.5f);
            var source = Projectile.GetSource_FromThis();
            Vector2 end = Projectile.Center + BeamAngle(set, 0).ToRotationVector2() * Length;
            Projectile.NewProjectile(source, end, Vector2.Zero, ModContent.ProjectileType<Weapons1119e.ExoBlast>(), damage, 0f, Projectile.owner);
            Projectile.NewProjectile(source, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<Weapons1119e.ExoBlast>(), damage, 0f, Projectile.owner);
            float _ = 0f;
            foreach (NPC npc in Main.ActiveNPCs)
                if (npc.CanBeChasedBy() && Collision.CheckAABBvLineCollision(npc.Hitbox.TopLeft(), npc.Hitbox.Size(), Projectile.Center, end, 10f, ref _))
                    Projectile.NewProjectile(source, npc.Center, Vector2.Zero, ModContent.ProjectileType<Weapons1119e.ExoBlast>(), damage, 0f, Projectile.owner);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D px = TextureAssets.MagicPixel.Value;
            Vector2 start = Projectile.Center - Main.screenPosition;
            for (int set = 0; set < 2; set++)
                for (int i = 0; i < 6; i++)
                {
                    Color c = Main.hslToRgb((i / 6f + Main.GlobalTimeWrappedHourly * 0.3f) % 1f, 0.7f, 0.8f) with { A = 0 };
                    Main.EntitySpriteDraw(px, start, new Rectangle(0, 0, 1, 1), c * 0.6f, BeamAngle(set, i), new Vector2(0f, 0.5f), new Vector2(Length, 6f), SpriteEffects.None);
                }
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, 1f, SpriteEffects.None);
            return false;
        }
    }

    /// <summary>Justice lancée (furtivité pleine) : cherche les ennemis et explose (dégâts déjà x5).</summary>
    public class ThrownJustice : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Melee/Justice";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            CalamityUtils.HomeInOnNPC(Projectile, true, 1000f, 20f, 10f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.GoldFlame, Vector2.Zero, 100, default, 1.4f).noGravity = true;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            Projectile.ExpandHitboxBy(160);
            Projectile.maxPenetrate = Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            for (int i = 0; i < Projectile.localNPCImmunity.Length; i++)
                Projectile.localNPCImmunity[i] = 0;
            Projectile.Damage();
            if (!Main.dedServ)
                for (int i = 0; i < 30; i++)
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, i % 2 == 0 ? DustID.GoldFlame : DustID.BlueTorch, 0f, 0f, 100, default, 1.8f).velocity *= 2.5f;
        }
    }

    /// <summary>Étincelle de la Divine Intervention : tombe, Hell Judgement.</summary>
    public class FlamingShard : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Melee/FlamingShard";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.25f, 12f);
            Projectile.rotation = Projectile.velocity.ToRotation();
            Lighting.AddLight(Projectile.Center, 0.6f, 0.4f, 0.1f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Buffs.HellJudgement>(), 300);
    }

    /// <summary>Tête de météore appelée par la Divine Intervention : arrive de hors de l'écran et cherche les ennemis.</summary>
    public class DivineMeteorHead : ModProjectile
    {
        public override string Texture => "Terraria/Images/NPC_" + NPCID.MeteorHead;

        public override void SetStaticDefaults() => Main.projFrames[Type] = 2;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            CalamityUtils.HomeInOnNPC(Projectile, true, 2000f, 14f, 15f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (++Projectile.frameCounter % 6 == 0)
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
            if (!Main.dedServ)
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1.5f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Buffs.HellJudgement>(), 300);
    }

    /// <summary>Divine Intervention lancée (furtivité pleine) : vole vers le curseur puis revient.</summary>
    public class ThrownDivineIntervention : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Melee/DivineIntervention";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Projectile.penetrate = -1;
            Projectile.timeLeft = 240;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            Projectile.rotation += 0.4f;
            if (++Projectile.ai[0] > 30f)
            {
                Vector2 back = owner.Center - Projectile.Center;
                if (back.Length() < 30f)
                {
                    Projectile.Kill();
                    return;
                }
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, back.SafeNormalize(Vector2.Zero) * 16f, 0.1f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
            => Items.Weapons.Melee.DivineHits.OnHit(Main.player[Projectile.owner], target, Projectile.GetSource_FromThis(), Projectile.damage);
    }
}
