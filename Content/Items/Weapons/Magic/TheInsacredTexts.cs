using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Items.Weapons.Magic
{
    /// <summary>
    /// Wiki : The Insacred Texts.txt — un grand portail rouge s'ouvre sous 3 ennemis au plus ; après une demi-seconde, un
    /// bras en sort, les attrape et les entraîne dedans (450 dégâts). Recharge de 25 s. [validé 31/10] Le clic droit
    /// n'ouvre pas de fanfiction (AO3) : il affiche en rose dans le chat une phrase au hasard qui finit par
    /// « ...then they kissed ». Lâché par les Zombies et les Demon Eyes (5 %, WikiLoot).
    /// </summary>
    public class TheInsacredTexts : ModItem
    {
        public const int Cooldown = 25 * 60;
        public const int FicCount = 12;
        private const float TargetRange = 800f;

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.damage = 450;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 100;
            Item.useTime = Item.useAnimation = 19;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.knockBack = 0f;
            Item.UseSound = SoundID.Item8;
            Item.shoot = ModContent.ProjectileType<Projectiles.Magic.InsacredPortal>();
            Item.shootSpeed = 14f;
            Item.value = Item.sellPrice(silver: 20);
            Item.rare = ItemRarityID.Blue;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player) =>
            player.altFunctionUse == 2 || player.GetModPlayer<InsacredPlayer>().Cooldown <= 0 && Targets(player).Any();

        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            if (player.altFunctionUse == 2)
                mult = 0f;
        }

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2 && player.whoAmI == Main.myPlayer && player.ItemAnimationJustStarted)
                Main.NewText(Language.GetTextValue("Mods.CalamityFandom.InsacredTexts.Fic" + Main.rand.Next(FicCount)), Color.HotPink);
            return null;
        }

        /// <summary>Les 3 ennemis les plus proches du curseur (à portée de vue du joueur).</summary>
        private static IEnumerable<NPC> Targets(Player player)
        {
            Vector2 mouse = player.whoAmI == Main.myPlayer ? Main.MouseWorld : player.Center;
            return Main.npc.Where(n => n.active && n.CanBeChasedBy() && Vector2.Distance(n.Center, player.Center) < TargetRange)
                .OrderBy(n => Vector2.DistanceSquared(n.Center, mouse)).Take(3);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
                return false;
            foreach (NPC target in Targets(player))
                Projectile.NewProjectile(source, target.Bottom, Vector2.Zero, type, damage, knockback, player.whoAmI, target.whoAmI);
            player.GetModPlayer<InsacredPlayer>().Cooldown = Cooldown;
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int left = Main.LocalPlayer.GetModPlayer<InsacredPlayer>().Cooldown;
            if (left > 0)
                tooltips.Add(new TooltipLine(Mod, "InsacredCooldown", Language.GetTextValue("Mods.CalamityFandom.InsacredTexts.Cooldown", (left + 59) / 60))
                { OverrideColor = Color.Gray });
        }
    }

    /// <summary>Recharge de 25 s des Insacred Texts (par joueur).</summary>
    public class InsacredPlayer : ModPlayer
    {
        public int Cooldown;

        public override void PostUpdate()
        {
            if (Cooldown > 0)
                Cooldown--;
        }
    }
}

namespace CalamityFandom.Content.Projectiles.Magic
{
    /// <summary>
    /// Portail des Insacred Texts (ai[0] = cible) : s'ouvre 30 ticks sous la cible, le bras monte et l'attrape (touche une
    /// fois, à 30 ticks), puis l'entraîne vers le portail pendant 40 ticks (pas les boss ni les ennemis sans recul).
    /// [choix] Portail et bras : textures de Calamity (DemonPortal, main du Sepulcher) teintées en rouge.
    /// </summary>
    public class InsacredPortal : ModProjectile
    {
        private const int GrabTime = 30;
        private const int DragTime = 40;
        private const int Life = 100;

        public override string Texture => "CalamityMod/NPCs/Other/DemonPortal";

        private NPC Target => Main.npc[(int)Projectile.ai[0]];
        private ref float Timer => ref Projectile.localAI[0];
        /// <summary>Hauteur du bras au-dessus du portail.</summary>
        private ref float Reach => ref Projectile.localAI[1];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = Life;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Timer++;
            NPC target = Target;
            bool alive = target.active && !target.dontTakeDamage;
            if (Timer <= GrabTime && alive)
                Projectile.Center = target.Bottom;
            if (Timer == 1)
                SoundEngine.PlaySound(SoundID.Item113, Projectile.Center);
            // Le bras monte jusqu'au centre de la cible, puis redescend en la tirant
            float height = alive ? MathHelper.Max(Projectile.Center.Y - target.Center.Y, 0f) : 0f;
            if (Timer <= GrabTime)
                Reach = height * (Timer / GrabTime);
            else if (Timer <= GrabTime + DragTime)
            {
                Reach = height;
                if (alive && Main.netMode != NetmodeID.MultiplayerClient && !target.boss && target.knockBackResist > 0f)
                {
                    target.velocity = (Projectile.Center - target.Bottom) * 0.15f;
                    target.netUpdate = true;
                }
            }
            else
                Reach = MathHelper.Max(Reach - 6f, 0f);
            if (Main.rand.NextBool(2))
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(30f, 4f), 60, 8, DustID.Blood, 0f, -1f, 100, default, 1.2f);
                dust.noGravity = true;
            }
            Lighting.AddLight(Projectile.Center, 0.5f, 0.05f, 0.05f);
        }

        public override bool? CanHitNPC(NPC target) =>
            target.whoAmI == (int)Projectile.ai[0] && Timer >= GrabTime ? null : false;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => true;

        public override bool PreDraw(ref Color lightColor)
        {
            float open = MathHelper.Clamp(MathHelper.Min(Timer, Projectile.timeLeft) / 15f, 0f, 1f);
            Vector2 center = Projectile.Center - Main.screenPosition;
            Texture2D arm = ModContent.Request<Texture2D>("CalamityMod/Gores/SupremeCalamitas/SepulcherHand_Gore1").Value;
            if (Reach > 4f)
            {
                // Bras étiré du portail jusqu'à la cible
                var scale = new Vector2(1.2f, Reach / arm.Height);
                Main.EntitySpriteDraw(arm, center, null, new Color(255, 80, 80), 0f, new Vector2(arm.Width / 2f, arm.Height), scale, SpriteEffects.None, 0);
            }
            Texture2D portal = ModContent.Request<Texture2D>(Texture).Value;
            var portalScale = new Vector2(1.6f, 0.45f) * open;
            Main.EntitySpriteDraw(portal, center, null, new Color(255, 40, 40, 200), 0f, portal.Size() / 2f, portalScale, SpriteEffects.None, 0);
            return false;
        }
    }
}
