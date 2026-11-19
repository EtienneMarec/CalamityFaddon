using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

// Lot O (19/11) : objets du wiki lâchés par des ennemis déjà dans le jeu (ennemis générés : Yggdrasil Ent, Diviner
// Valkyrie, Floating Biomass). Valeurs des pages.

namespace CalamityFandom.Content.Items.Accessories
{
    /// <summary>
    /// Wiki : Yggdrasil Core.txt — 10 défense et une aura de flammes sacrées autour du joueur (Holy Flames 2 s). Butin de
    /// l'Yggdrasil Ent. [choix] Aura de 12 blocs de rayon, vérifiée toutes les 30 images.
    /// </summary>
    public class YggdrasilCore : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 34;
            Item.accessory = true;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.sellPrice(gold: 50);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statDefense += 10;
            if (!hideVisual && Main.rand.NextBool(4))
                Dust.NewDustPerfect(player.Center + Main.rand.NextVector2CircularEdge(192f, 192f), DustID.GoldFlame, Vector2.Zero, 100, default, 1.2f).noGravity = true;
            if (player.whoAmI != Main.myPlayer || Main.GameUpdateCount % 30 != 0)
                return;
            foreach (NPC npc in Main.ActiveNPCs)
                if (!npc.friendly && npc.lifeMax > 5 && !npc.dontTakeDamage && npc.Distance(player.Center) < 192f)
                {
                    npc.AddBuff(ModContent.BuffType<HolyFlames>(), 120);
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        NetMessage.SendData(MessageID.AddNPCBuff, number: npc.whoAmI, number2: ModContent.BuffType<HolyFlames>(), number3: 120);
                }
        }
    }

    /// <summary>
    /// Wiki : Babil Destruction Core.txt — après chaque coup reçu, pendant 5 s : +100 % de vitesse de minage, +20 % de vitesse
    /// de déplacement, -20 % de réduction des dégâts. Butin des Floating Biomasses.
    /// </summary>
    public class BabilDestructionCore : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = Item.height = 32;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.sellPrice(gold: 10);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var mp = player.GetModPlayer<BabilCorePlayer>();
            mp.Equipped = true;
            if (mp.Timer > 0)
            {
                player.pickSpeed -= 0.5f;
                player.moveSpeed += 0.2f;
                player.endurance -= 0.2f;
            }
        }
    }

    public class BabilCorePlayer : ModPlayer
    {
        public bool Equipped;
        public int Timer;

        public override void ResetEffects() => Equipped = false;

        public override void PostUpdate()
        {
            if (Timer > 0)
                Timer--;
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            if (Equipped)
                Timer = 300;
        }
    }
}

namespace CalamityFandom.Content.Items.Weapons.Rogue
{
    /// <summary>
    /// Wiki : Spear of Liberty.txt — trois lances : celle du milieu comme la Cursed Spear, les deux autres très chercheuses ;
    /// chacune perce une fois (Cursed Inferno 2 s) ; frappe furtive : les trois cherchent, traversent les blocs et percent deux
    /// fois de plus. Recette du wiki, butin rare des ennemis de la Corruption (1,4 %) et de la Diviner Valkyrie.
    /// </summary>
    public class SpearofLiberty : RogueWeapon
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.damage = 26;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.useTime = Item.useAnimation = 19;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = Item.noUseGraphic = true;
            Item.knockBack = 2f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.width = Item.height = 48;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.sellPrice(gold: 7, silver: 20);
            Item.shoot = ModContent.ProjectileType<Projectiles.Weapons1119k.CursedSpearProj>();
            Item.shootSpeed = 20f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool stealth = player.Calamity().StealthStrikeAvailable();
            for (int i = -1; i <= 1; i++)
            {
                bool homing = stealth || i != 0;
                int p = Projectile.NewProjectile(source, position, velocity.RotatedBy(0.2f * i) * (i != 0 ? 1.3f : 1f), ModContent.ProjectileType<LibertySpear>(),
                    damage, knockback, player.whoAmI, homing ? 1f : 0f, stealth ? 1f : 0f);
            }
            return false;
        }
    }

    /// <summary>Lance de la Spear of Liberty : ai[0] = 1 chercheuse ; ai[1] = 1 (furtive) : traverse les blocs, perce deux fois de plus.</summary>
    public class LibertySpear : Projectiles.Weapons1119k.CursedSpearProj
    {
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Rogue/SpearofLiberty";

        public override void OnSpawn(IEntitySource source)
        {
            if (Projectile.ai[1] == 1f)
            {
                Projectile.penetrate = 4;
                Projectile.tileCollide = false;
            }
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 1f)
            {
                CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 700f, 18f, 10f);
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
                if (!Main.dedServ && Main.rand.NextBool(2))
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.CursedTorch, 0f, 0f, 100, default, 1.2f).noGravity = true;
                return;
            }
            base.AI();
        }
    }
}
