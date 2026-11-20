using System;
using System.Linq;
using CalamityFandom.Common.Debug;
using CalamityFandom.Content.Projectiles.Weapons1119m;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

// Lot AZ (20/11) : objets que le butin du wiki donne selon la difficulté (« {{expert|5%}} », « {{revengeance|100%}} »...,
// lus par gen_loot.py depuis le 20/11). Ils tombent dès qu'ils existent.

namespace CalamityFandom.Content.Items.Accessories
{
    /// <summary>
    /// Wiki : Arcanoid Orb.txt — une orbe immobile qui blesse au contact (120) et rend Confused (3 s) ; touche « Arcanoid
    /// Orb » : l'orbe se replace au curseur pour 5 mana ; maintenue : elle suit le curseur en vidant le mana (Cosmic
    /// Elemental, Expert 5 %). [choix] 1 mana toutes les 3 images en suivant ; dégâts d'invocation.
    /// </summary>
    public class ArcanoidOrb : WikiCovering
    {
        public static ModKeybind Key;

        protected override int Rarity => ItemRarityID.Orange;
        protected override int SellCopper => 20000;

        public override void Load() => Key = KeybindLoader.RegisterKeybind(Mod, "ArcanoidOrb", "F");

        public override void Unload() => Key = null;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.expert = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
            => Weapons.Summon.WikiWeapons1120bPlayer.KeepMinion(player, Item, ModContent.ProjectileType<Projectiles.Summon.ArcanoidOrbMinion>(), ModContent.BuffType<Buffs.ArcanoidOrbBuff>(), 120);
    }

    /// <summary>
    /// Wiki : Astral Bulwark.txt — ancien objet Expert d'Astrum Deus : être touché fait tomber cinq étoiles astrales du ciel
    /// (Astral Infection 2 s) ; immunité à Astral Infection. [choix] Étoiles à 50 dégâts, vers l'ennemi le plus proche.
    /// </summary>
    public class AstralBulwark : WikiCovering
    {
        protected override int Rarity => ItemRarityID.Expert;
        protected override int SellCopper => 190000;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.expert = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[ModContent.BuffType<AstralInfectionDebuff>()] = true;
            player.GetModPlayer<Coverings1120dPlayer>().Bulwark = true;
        }
    }

    /// <summary>
    /// Wiki : Blood Element.txt — +12 % de dégâts magiques, +100 mana ; un coup magique fait tomber un morceau de viande
    /// gelée (Confused et Burning Blood 3 s aux ennemis touchés) ; le ramasser donne un morceau ; touche « Consume » : tous
    /// les morceaux renforcent la prochaine attaque magique (Dreadnautilus après Calamitas, Revengeance).
    /// [choix] 10 morceaux au plus, +15 % de dégâts par morceau ; un morceau tombe au plus toutes les 30 images.
    /// [technique] « Le reste du mana devient des contenants de viande » n'est pas fait.
    /// </summary>
    public class BloodElement : WikiCovering
    {
        public static ModKeybind Key;

        protected override int Rarity => ItemRarityID.Lime;
        protected override int SellCopper => 200000;

        public override void Load() => Key = KeybindLoader.RegisterKeybind(Mod, "BloodElementConsume", "C");

        public override void Unload() => Key = null;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<MagicDamageClass>() += 0.12f;
            player.statManaMax2 += 100;
            player.GetModPlayer<Coverings1120dPlayer>().BloodElement = true;
        }
    }

    /// <summary>
    /// Wiki : Burden Breaker.txt — ancien objet de Skeletron Prime : au-delà de 25 mph de vitesse horizontale, accélère de
    /// 2,5 % par image jusqu'à un mur (ou la vitesse maximale, qui remet à zéro) ; inactif pendant un boss.
    /// [choix] Vitesse maximale ramenée à 60 px par image (≈ 300 mph) pour que les blocs arrêtent encore le joueur.
    /// </summary>
    public class BurdenBreaker : WikiCovering
    {
        protected override int Rarity => ItemRarityID.Pink;
        protected override int SellCopper => 48000;

        public override void UpdateAccessory(Player player, bool hideVisual) => player.GetModPlayer<Coverings1120dPlayer>().BurdenBreaker = true;
    }

    /// <summary>
    /// Wiki : Dread Bone.txt — +10 % de dégâts d'invocation ; les sbires ont 20 % de chances d'un critique à 300 % ; après 5
    /// attaques ou un critique, le sbire explose en sang et revient quelques secondes plus tard (Blood Zombie après l'Eater
    /// of Worlds ou le Brain of Cthulhu, Expert 1 %). [choix] Absent 3 s (ni dégâts ni dessin).
    /// </summary>
    public class DreadBone : WikiCovering
    {
        protected override int Rarity => ItemRarityID.Green;
        protected override int SellCopper => 6000;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<SummonDamageClass>() += 0.1f;
            player.GetModPlayer<Coverings1120dPlayer>().DreadBone = true;
        }
    }

    /// <summary>
    /// Wiki : Fancy Flesh Skull.txt — l'Adrénaline met deux fois plus de temps à se remplir mais dure quatre fois plus
    /// (Wall of Flesh, Death 10 %).
    /// </summary>
    public class FancyFleshSkull : WikiCovering
    {
        protected override int Rarity => ItemRarityID.LightPurple;
        protected override int SellCopper => 2000;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var cal = player.Calamity();
            cal.AdrenalineChargeTime *= 2;
            cal.AdrenalineDuration *= 4;
        }
    }

    public class Coverings1120dPlayer : ModPlayer
    {
        public bool Bulwark, BloodElement, BurdenBreaker, DreadBone;
        public int MeatPieces, MeatCooldown;
        public bool MeatPrimed;

        public override void ResetEffects() => Bulwark = BloodElement = BurdenBreaker = DreadBone = false;

        public override void PostUpdateEquips()
        {
            if (MeatCooldown > 0)
                MeatCooldown--;
            if (BloodElement && Player.whoAmI == Main.myPlayer && global::CalamityFandom.Content.Items.Accessories.BloodElement.Key?.JustPressed == true && MeatPieces > 0)
            {
                MeatPrimed = true;
                SoundEngine.PlaySound(SoundID.Item2, Player.Center);
            }
        }

        public override void PostUpdate()
        {
            // Burden Breaker : accélération horizontale sans fin (hors combat de boss)
            if (!BurdenBreaker || Main.CurrentFrameFlags.AnyActiveBossNPC)
                return;
            float vx = Math.Abs(Player.velocity.X);
            if (vx > 4.9f)
            {
                Player.velocity.X *= 1.025f;
                if (Math.Abs(Player.velocity.X) > 60f)
                    Player.velocity.X = 0f;
            }
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            if (!Bulwark || Player.whoAmI != Main.myPlayer)
                return;
            for (int i = 0; i < 5; i++)
            {
                Vector2 from = Player.Center + new Vector2(Main.rand.NextFloat(-400f, 400f), -800f);
                NPC t = Main.npc.Where(n => n.CanBeChasedBy() && n.Distance(Player.Center) < 1000f).OrderBy(n => n.Distance(Player.Center)).FirstOrDefault();
                Vector2 to = t?.Center ?? Player.Center + new Vector2(Main.rand.NextFloat(-200f, 200f), 0f);
                Projectile.NewProjectile(Player.GetSource_OnHurt(info.DamageSource), from, (to - from).SafeNormalize(Vector2.UnitY) * 16f,
                    ModContent.ProjectileType<Projectiles.Typeless.BulwarkStar>(), 50, 2f, Player.whoAmI);
            }
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (BloodElement && MeatPrimed && proj.CountsAsClass<MagicDamageClass>())
            {
                modifiers.SourceDamage *= 1f + 0.15f * MeatPieces;
                MeatPieces = 0;
                MeatPrimed = false;
            }
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (BloodElement && proj.CountsAsClass<MagicDamageClass>() && MeatCooldown <= 0 && Player.whoAmI == Main.myPlayer
                && proj.type != ModContent.ProjectileType<Projectiles.Typeless.FrozenMeat>())
            {
                MeatCooldown = 30;
                Projectile.NewProjectile(Player.GetSource_OnHit(target), target.Top, new Vector2(Main.rand.NextFloat(-2f, 2f), -5f),
                    ModContent.ProjectileType<Projectiles.Typeless.FrozenMeat>(), Math.Max(1, damageDone / 4), 1f, Player.whoAmI);
            }
        }
    }

    /// <summary>Dread Bone : critiques des sbires, explosion après 5 attaques ou un critique, retour 3 s plus tard.</summary>
    public class DreadBoneMinion : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        private int attacks, dead;

        private static bool Owned(Projectile p) => p.minion && p.owner >= 0 && p.owner < Main.maxPlayers && Main.player[p.owner].active
            && Main.player[p.owner].GetModPlayer<Coverings1120dPlayer>().DreadBone;

        public override bool PreAI(Projectile projectile)
        {
            if (dead > 0)
            {
                dead--;
                projectile.timeLeft = Math.Max(projectile.timeLeft, 2);
                if (dead == 0)
                    for (int i = 0; i < 15; i++)
                        Dust.NewDustPerfect(projectile.Center, DustID.Blood, Main.rand.NextVector2Circular(3f, 3f), 0, default, 1.2f);
            }
            return true;
        }

        public override bool? CanDamage(Projectile projectile) => dead > 0 ? false : null;

        public override bool PreDraw(Projectile projectile, ref Color lightColor) => dead <= 0;

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Owned(projectile) && Main.rand.NextBool(5))
            {
                modifiers.SourceDamage *= 3f;
                modifiers.SetCrit();
                attacks = 5;
            }
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!Owned(projectile) || ++attacks < 5)
                return;
            attacks = 0;
            dead = 180;
            for (int i = 0; i < 25; i++)
                Dust.NewDustPerfect(projectile.Center, DustID.Blood, Main.rand.NextVector2Circular(6f, 6f), 0, default, 1.6f);
            SoundEngine.PlaySound(SoundID.NPCDeath1 with { Volume = 0.5f }, projectile.Center);
        }
    }

    public class WikiCoverings1120dChecks : ModSystem
    {
        public override void Load()
        {
            SelfTest.ExtraChecks.Add(t =>
            {
                foreach (string name in new[] { "Arcanoid Orb", "Astral Bulwark", "Blood Element", "Burden Breaker", "Dread Bone", "Fancy Flesh Skull", "Lead Wizard" })
                    t.Expect(Common.WikiData.WikiLoot.Added.Any(a => a.what.StartsWith(name)), $"butin du wiki : {name}");
            });
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Ranged
{
    /// <summary>
    /// Wiki : Lead Wizard.txt — ancien fusil de Golem : les balles deviennent des balles jaunes à grande vitesse qui partent
    /// au-dessus et en dessous du curseur (environ 30° entre les deux) ; 33 % de chances de ne pas consommer de munition.
    /// </summary>
    public class LeadWizard : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 58;
            Item.DamageType = DamageClass.Ranged;
            Item.crit = 30;
            Item.useTime = Item.useAnimation = 9;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item41;
            Item.width = 72;
            Item.height = 38;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 12);
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 20f;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.NextFloat() >= 0.33f;

        public override Vector2? HoldoutOffset() => new Vector2(-8f, 0f);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = -1; i <= 1; i += 2)
                Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(15f * i)), ProjectileID.BulletHighVelocity, damage, knockback, player.whoAmI);
            return false;
        }
    }
}

namespace CalamityFandom.Content.Buffs
{
    public class ArcanoidOrbBuff : WikiMinionBuff
    {
        public override string Texture => "Terraria/Images/Buff_" + BuffID.Clairvoyance;
        protected override int MinionType => ModContent.ProjectileType<Projectiles.Summon.ArcanoidOrbMinion>();
    }
}

namespace CalamityFandom.Content.Projectiles.Summon
{
    /// <summary>Orbe arcanoïde : reste où elle est ; touche Arcanoid Orb : au curseur (5 mana) ; maintenue : suit le curseur.</summary>
    public class ArcanoidOrbMinion : WikiMinion
    {
        public override string Texture => "CalamityFandom/Content/Items/Accessories/ArcanoidOrb";
        protected override int BuffType => ModContent.BuffType<Buffs.ArcanoidOrbBuff>();
        protected override float Slots => 0f;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.localNPCHitCooldown = 15;
        }

        public override void AI()
        {
            if (!KeepAlive())
                return;
            if (Projectile.ai[0] == 0f)
            {
                Projectile.ai[0] = 1f;
                Projectile.Center = Owner.Center + new Vector2(0f, -80f);
            }
            Projectile.velocity = Vector2.Zero;
            Projectile.rotation += 0.05f;
            Lighting.AddLight(Projectile.Center, 0.6f, 0.3f, 0.9f);
            if (Projectile.owner != Main.myPlayer || Items.Accessories.ArcanoidOrb.Key == null)
                return;
            var key = Items.Accessories.ArcanoidOrb.Key;
            if (key.JustPressed && Owner.CheckMana(5, true))
            {
                Projectile.Center = Main.MouseWorld;
                Projectile.netUpdate = true;
            }
            else if (key.Current && !key.JustPressed && (Main.GameUpdateCount % 3 != 0 || Owner.CheckMana(1, true)))
            {
                Projectile.Center = Vector2.Lerp(Projectile.Center, Main.MouseWorld, 0.15f);
                if (Main.GameUpdateCount % 10 == 0)
                    Projectile.netUpdate = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Confused, 180);
    }
}

namespace CalamityFandom.Content.Projectiles.Typeless
{
    /// <summary>Étoile astrale de l'Astral Bulwark : Astral Infection 2 s.</summary>
    public class BulwarkStar : ModProjectile
    {
        public override string Texture => "CalamityFandom/Content/Projectiles/Typeless/BulwarkStar";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Projectile.rotation += 0.2f;
            Lighting.AddLight(Projectile.Center, 0.8f, 0.4f, 0.6f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120);
    }

    /// <summary>Morceau de viande gelée (Blood Element) : tombe (Confused et Burning Blood 3 s) ; se ramasse (un morceau, 10 au plus).</summary>
    public class FrozenMeat : ModProjectile
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.MonsterLasagna;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.25f, 10f);
            Projectile.velocity.X *= 0.98f;
            Projectile.rotation += Projectile.velocity.X * 0.05f;
            Player owner = Main.player[Projectile.owner];
            if (Projectile.owner == Main.myPlayer && Projectile.timeLeft < 570 && owner.Hitbox.Intersects(Projectile.Hitbox))
            {
                var cp = owner.GetModPlayer<Items.Accessories.Coverings1120dPlayer>();
                cp.MeatPieces = Math.Min(10, cp.MeatPieces + 1);
                CombatText.NewText(owner.Hitbox, new Color(200, 60, 60), cp.MeatPieces);
                SoundEngine.PlaySound(SoundID.Grab, owner.Center);
                Projectile.Kill();
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = Vector2.Zero;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Confused, 180);
            target.AddBuff(ModContent.BuffType<BurningBlood>(), 180);
        }
    }
}
