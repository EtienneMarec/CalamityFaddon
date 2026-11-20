using System;
using System.Collections.Generic;
using System.Linq;
using CalamityFandom.Common.Debug;
using CalamityFandom.Content.Legacy.Buffs.StatDebuffs;
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
using Terraria.Utilities;

// Lot BB (20/11) : les deux dernières armes du butin du wiki de la Plaguebringer Goliath.

namespace CalamityFandom.Content.Items.Weapons.Typeless
{
    /// <summary>
    /// Wiki : Hi-Scan Camera.txt — arme sans classe à deux modes (clic droit pour changer). Scan : un flash de 10 blocs qui
    /// prend les debuffs des ennemis et les garde (3 au plus) ; 200 % aux ennemis « organiques », tue les bestioles d'un
    /// coup. Snapshot : maintenu, un viseur ; relâché, les ennemis dans le viseur reçoivent les debuffs gardés (consommés),
    /// 300 % aux ennemis « spectraux ». Jamais de critique, pas de modificateurs.
    /// [choix] Flash centré sur le curseur ; « organique » = bruit de chair (NPCHit1), « spectral » = bruit de fantôme
    /// (NPCHit36, NPCHit54) ; « les debuffs que l'ennemi inflige » = ceux qu'il porte ; debuffs rendus 10 s ; viseur 20 x 12
    /// blocs.
    /// </summary>
    public class HiScanCamera : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 50;
            Item.DamageType = DamageClass.Default;
            Item.useTime = Item.useAnimation = 35;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1f;
            Item.autoReuse = false;
            Item.width = 48;
            Item.height = 40;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 5);
            Item.shoot = ModContent.ProjectileType<Projectiles.Typeless.ScanFlash>();
            Item.shootSpeed = 20f;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool? PrefixChance(int pre, UnifiedRandom rand) => false;

        public override void ModifyWeaponCrit(Player player, ref float crit) => crit = 0f;

        public override bool CanUseItem(Player player)
        {
            var cp = player.GetModPlayer<CameraPlayer>();
            if (player.altFunctionUse == 2)
            {
                Item.channel = false;
                Item.UseSound = SoundID.MenuTick;
                return true;
            }
            Item.channel = cp.Snapshot;
            Item.UseSound = cp.Snapshot ? null : SoundID.Item122;
            return !cp.Snapshot || player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Typeless.Viewfinder>()] <= 0;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var cp = player.GetModPlayer<CameraPlayer>();
            if (player.altFunctionUse == 2)
            {
                cp.Snapshot = !cp.Snapshot;
                CombatText.NewText(player.Hitbox, Color.LightGreen, cp.Snapshot ? "Snapshot" : "Scan");
                return false;
            }
            int proj = cp.Snapshot ? ModContent.ProjectileType<Projectiles.Typeless.Viewfinder>() : type;
            Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, proj, damage, knockback, player.whoAmI);
            return false;
        }
    }

    public class CameraPlayer : ModPlayer
    {
        public bool Snapshot;
        public readonly List<int> Stored = new();
    }
}

namespace CalamityFandom.Content.Items.Weapons.Magic
{
    /// <summary>
    /// Wiki : Plaguecom Tablet.txt — on dessine une ligne qui blesse les ennemis qui la touchent ; clic droit : change de
    /// forme. Offensive (vert et rouge) : Plague 5 s ; au relâchement (ou sans encre), les ennemis touchés éclatent en
    /// petites explosions à 135 %. Défensive (gris, cyan et vert) : bloque 10 tirs avant de se briser, 50 % de Glacial State
    /// 2 s, reste 30 s (les bouts clignotent en rouge à la fin). Encre : 6 % (offensive) ou 12 % (défensive) par 10 blocs
    /// tracés ; double appui sur Bas pour recharger, contre 100 mana + 50 % du mana max ; -75 % de régénération de mana tenue.
    /// [choix] Un segment toutes les 3 images ; dégâts de contact toutes les 10 images.
    /// </summary>
    public class PlaguecomTablet : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 150;
            Item.DamageType = DamageClass.Magic;
            Item.useTime = Item.useAnimation = 5;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = false;
            Item.channel = true;
            Item.knockBack = 0f;
            Item.autoReuse = false;
            Item.width = 58;
            Item.height = 62;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(gold: 10);
            Item.shoot = ModContent.ProjectileType<Projectiles.Magic.TabletPen>();
            Item.shootSpeed = 1f;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.channel = false;
                return true;
            }
            Item.channel = true;
            return player.GetModPlayer<TabletPlayer>().Ink > 0f && player.ownedProjectileCounts[Item.shoot] <= 0;
        }

        public override void ModifyWeaponKnockback(Player player, ref StatModifier knockback)
        {
            if (player.GetModPlayer<TabletPlayer>().Defensive)
                knockback.Flat += 5f;
        }

        public override void HoldItem(Player player) => player.GetModPlayer<TabletPlayer>().Holding = true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var tp = player.GetModPlayer<TabletPlayer>();
            if (player.altFunctionUse == 2)
            {
                tp.Defensive = !tp.Defensive;
                CombatText.NewText(player.Hitbox, tp.Defensive ? Color.Cyan : Color.LimeGreen, tp.Defensive ? "Defensive" : "Offensive");
                return false;
            }
            Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI, tp.Defensive ? 1f : 0f);
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var tp = Main.LocalPlayer.GetModPlayer<TabletPlayer>();
            tooltips.Add(new TooltipLine(Mod, "Ink", $"{(int)(tp.Ink * 100)}%") { OverrideColor = Color.LimeGreen });
        }
    }

    /// <summary>Encre de la tablette (0 à 1), forme choisie, rechargement par double appui sur Bas.</summary>
    public class TabletPlayer : ModPlayer
    {
        public float Ink = 1f;
        public bool Defensive, Holding;
        private int lastDown = -100;

        public override void ResetEffects() => Holding = false;

        public override void PostUpdateMiscEffects()
        {
            if (!Holding)
                return;
            // -75 % de régénération de mana
            Player.manaRegen = (int)(Player.manaRegen * 0.25f);
            if (Player.whoAmI != Main.myPlayer || !Player.controlDown || !Player.releaseDown)
                return;
            int now = (int)Main.GameUpdateCount;
            if (now - lastDown < 15 && Ink < 1f)
            {
                int cost = 100 + Player.statManaMax2 / 2;
                if (Player.CheckMana(cost, true, true))
                {
                    Ink = 1f;
                    SoundEngine.PlaySound(SoundID.Item37, Player.Center);
                    CombatText.NewText(Player.Hitbox, Color.LimeGreen, "100%");
                }
                lastDown = -100;
            }
            else
                lastDown = now;
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Typeless
{
    /// <summary>Flash du mode Scan : 10 blocs, garde les debuffs des ennemis, 200 % aux organiques, tue les bestioles.</summary>
    public class ScanFlash : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 160;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Default;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 6;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            if (Projectile.localAI[0]++ == 0)
            {
                for (int i = 0; i < 30; i++)
                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(80f, 80f), DustID.Electric, Vector2.Zero, 0, Color.White, 1.2f).noGravity = true;
                if (Projectile.owner == Main.myPlayer)
                    foreach (NPC n in Main.npc)
                        if (n.active && n.CountsAsACritter && n.Hitbox.Intersects(Projectile.Hitbox))
                            n.StrikeInstantKill();
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.DisableCrit();
            if (target.HitSound?.SoundPath == SoundID.NPCHit1.SoundPath)
                modifiers.SourceDamage *= 2f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            var stored = Main.player[Projectile.owner].GetModPlayer<Items.Weapons.Typeless.CameraPlayer>().Stored;
            for (int i = 0; i < NPC.maxBuffs && stored.Count < 3; i++)
            {
                int b = target.buffType[i];
                if (b > 0 && target.buffTime[i] > 0 && !stored.Contains(b))
                {
                    stored.Add(b);
                    target.DelBuff(i);
                    i--;
                }
            }
        }
    }

    /// <summary>Viseur du mode Snapshot : suit le curseur tant que le clic est maintenu ; relâché, déclenche la photo.</summary>
    public class Viewfinder : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 320;
            Projectile.height = 192;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            Projectile.timeLeft = 2;
            if (Projectile.owner != Main.myPlayer)
                return;
            Projectile.Center = Main.MouseWorld;
            owner.itemTime = owner.itemAnimation = 2;
            if (owner.channel && owner.HeldItem.type == ModContent.ItemType<Items.Weapons.Typeless.HiScanCamera>())
                return;
            // Photo : debuffs gardés sur tous les ennemis du viseur, 300 % aux spectraux
            var stored = owner.GetModPlayer<Items.Weapons.Typeless.CameraPlayer>().Stored;
            foreach (NPC n in Main.npc)
            {
                if (!n.active || n.friendly || n.dontTakeDamage || !n.Hitbox.Intersects(Projectile.Hitbox))
                    continue;
                foreach (int b in stored)
                    n.AddBuff(b, 600);
                bool spectral = n.HitSound?.SoundPath == SoundID.NPCHit36.SoundPath || n.HitSound?.SoundPath == SoundID.NPCHit54.SoundPath;
                n.SimpleStrikeNPC(spectral ? Projectile.damage * 3 : Projectile.damage, 0, false, 0f, DamageClass.Default);
            }
            stored.Clear();
            SoundEngine.PlaySound(SoundID.Camera, Projectile.Center);
            for (int i = 0; i < 40; i++)
                Dust.NewDustPerfect(Projectile.Center + new Vector2(Main.rand.NextFloat(-160f, 160f), Main.rand.NextFloat(-96f, 96f)), DustID.Electric, Vector2.Zero, 0, Color.White, 1f).noGravity = true;
            Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Cadre du viseur
            Vector2 tl = Projectile.TopLeft - Main.screenPosition;
            Color c = Color.White * 0.8f;
            Texture2D px = TextureAssets.MagicPixel.Value;
            Main.spriteBatch.Draw(px, new Rectangle((int)tl.X, (int)tl.Y, Projectile.width, 2), c);
            Main.spriteBatch.Draw(px, new Rectangle((int)tl.X, (int)tl.Y + Projectile.height - 2, Projectile.width, 2), c);
            Main.spriteBatch.Draw(px, new Rectangle((int)tl.X, (int)tl.Y, 2, Projectile.height), c);
            Main.spriteBatch.Draw(px, new Rectangle((int)tl.X + Projectile.width - 2, (int)tl.Y, 2, Projectile.height), c);
            return false;
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Magic
{
    /// <summary>Stylet de la tablette : suit le curseur et pose des segments d'encre tant que le clic est maintenu (ai[0] = 1 défensif).</summary>
    public class TabletPen : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private Vector2 last;
        private readonly List<int> segments = new();

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            Projectile.timeLeft = 2;
            if (Projectile.owner != Main.myPlayer)
                return;
            var tp = owner.GetModPlayer<Items.Weapons.Magic.TabletPlayer>();
            if (Projectile.localAI[0]++ == 0)
                last = Main.MouseWorld;
            owner.itemTime = owner.itemAnimation = 2;
            bool drawing = owner.channel && owner.HeldItem.type == ModContent.ItemType<Items.Weapons.Magic.PlaguecomTablet>() && tp.Ink > 0f;
            if (drawing && Projectile.localAI[0] % 3 == 0)
            {
                Vector2 now = Main.MouseWorld;
                float length = Vector2.Distance(last, now);
                if (length > 4f)
                {
                    tp.Ink = Math.Max(0f, tp.Ink - length / 160f * (Projectile.ai[0] == 1f ? 0.12f : 0.06f));
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), last, now - last, ModContent.ProjectileType<InkSegment>(), Projectile.damage, Projectile.knockBack,
                        Projectile.owner, Projectile.ai[0]);
                    segments.Add(p);
                    last = now;
                }
            }
            if (drawing)
                return;
            // Relâché ou plus d'encre : les ennemis touchés par la ligne offensive éclatent
            foreach (int s in segments)
            {
                Projectile seg = Main.projectile[s];
                if (seg.active && seg.type == ModContent.ProjectileType<InkSegment>() && seg.ai[0] == 0f)
                    seg.Kill();
            }
            Projectile.Kill();
        }
    }

    /// <summary>
    /// Segment d'encre : velocity = vecteur du segment. Offensif (ai[0] = 0) : Plague 5 s, les ennemis touchés éclatent à sa
    /// mort (135 %). Défensif : 30 s, bloque 10 tirs ennemis, 50 % de Glacial State 2 s.
    /// </summary>
    public class InkSegment : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private Vector2 span;
        private readonly HashSet<int> hit = new();

        private bool Defensive => Projectile.ai[0] == 1f;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1800;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (Projectile.localAI[0]++ == 0)
            {
                span = Projectile.velocity;
                Projectile.velocity = Vector2.Zero;
                if (!Defensive)
                    Projectile.timeLeft = 600;
            }
            if (!Defensive || Main.netMode == NetmodeID.MultiplayerClient)
                return;
            // Bloque les tirs ennemis qui le traversent
            foreach (Projectile p in Main.projectile)
            {
                if (!p.active || !p.hostile || p.damage <= 0)
                    continue;
                float _ = 0f;
                if (Collision.CheckAABBvLineCollision(p.position, p.Size, Projectile.Center, Projectile.Center + span, 8f, ref _))
                {
                    p.Kill();
                    if (++Projectile.ai[1] >= 10)
                    {
                        Projectile.Kill();
                        return;
                    }
                }
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + span, 8f, ref _);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Defensive)
            {
                if (Main.rand.NextBool())
                    target.AddBuff(ModContent.BuffType<GlacialState>(), 120);
                return;
            }
            target.AddBuff(ModContent.BuffType<Plague>(), 300);
            this.hit.Add(target.whoAmI);
        }

        public override void OnKill(int timeLeft)
        {
            if (Defensive || Projectile.owner != Main.myPlayer)
                return;
            foreach (int i in hit)
            {
                NPC n = Main.npc[i];
                if (!n.active)
                    continue;
                for (int k = 0; k < 3; k++)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), n.Center + Main.rand.NextVector2Circular(30f, 30f), Vector2.Zero, ModContent.ProjectileType<WikiDarts.DartBlast>(),
                        (int)(Projectile.damage * 1.35f / 3f), 0f, Projectile.owner, 32f, DustID.JungleSpore, 0f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (span == Vector2.Zero)
                return false;
            Color c;
            if (Defensive)
            {
                bool ending = Projectile.timeLeft < 120 && Projectile.timeLeft / 10 % 2 == 0;
                c = ending ? Color.Red : ((int)(Projectile.localAI[0] / 6) % 3) switch { 0 => Color.Gray, 1 => Color.Cyan, _ => Color.LimeGreen };
            }
            else
                c = (int)(Projectile.localAI[0] / 6) % 2 == 0 ? Color.LimeGreen : Color.Red;
            Main.EntitySpriteDraw(TextureAssets.MagicPixel.Value, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, 1, 1), c,
                span.ToRotation(), new Vector2(0f, 0.5f), new Vector2(span.Length() + 1f, 4f), SpriteEffects.None);
            return false;
        }
    }

    public class WikiWeapons1120dChecks : ModSystem
    {
        public override void Load()
        {
            SelfTest.ExtraChecks.Add(t =>
            {
                foreach (string name in new[] { "Hi-Scan Camera", "Plaguecom Tablet" })
                    t.Expect(Common.WikiData.WikiLoot.Added.Any(a => a.what.StartsWith(name)), $"butin du wiki : {name}");
            });
        }
    }
}
