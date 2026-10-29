using CalamityFandom.Content.Buffs;
using CalamityMod;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Microcell Cut.txt — épée courte inventée (attribuée à l'addon inventé Tales of the Aquatic), d'après la
    /// Disease (décision du 30/10) : chaque ennemi tué libère 3 étoiles à tête chercheuse à 75 % des dégâts qui infligent
    /// Ancient Astral Blight 4 s. Shortflurry : une étoile astrale à chaque coup (données de la page Shortstrike).
    /// Valeurs du wiki : 1222 dégâts, use 4, recul 5,5, vélocité 9, rareté C4, vente 5 platine.
    /// Recette du wiki : 10 Lethal Disease Canister + Disease à l'Aquatic Hypercomputer (station inventée, pas encore
    /// créée : TODO, la recette s'appliquera d'elle-même une fois la station et l'ingrédient faits).
    /// </summary>
    public class MicrocellCut : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = Item.height = 36;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.damage = 1222;
            Item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Item.useAnimation = Item.useTime = 4;
            Item.shoot = ModContent.ProjectileType<MicrocellCutProj>();
            Item.shootSpeed = 2f;
            Item.knockBack = 5.5f;
            Item.UseSound = SoundID.Item1;
            Item.value = Item.sellPrice(platinum: 5);
            Item.rare = ModContent.RarityType<Rarities.RarityC4>();
        }

        public override bool MeleePrefix() => true;
    }

    public class MicrocellCutProj : BaseShortswordProjectile
    {
        public override Terraria.Localization.LocalizedText DisplayName => ModContent.GetInstance<MicrocellCut>().DisplayName;
        public override string Texture => "CalamityFandom/Content/Items/Weapons/Melee/MicrocellCut";

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(16);
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Projectile.ownerHitCheck = true;
            Projectile.timeLeft = 360;
            Projectile.hide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void SetVisualOffsets()
        {
            int halfProjWidth = Projectile.width / 2, halfProjHeight = Projectile.height / 2;
            DrawOriginOffsetX = 0;
            DrawOffsetX = -(36 / 2 - halfProjWidth);
            DrawOriginOffsetY = -(36 / 2 - halfProjHeight);
        }

        public override void ExtraBehavior()
        {
            if (Main.rand.NextBool(5))
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.PurpleTorch);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<AncientAstralBlight>(), 240);
            if (target.life > 0 || Projectile.owner != Main.myPlayer)
                return;
            for (int i = 0; i < 3; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Main.rand.NextVector2CircularEdge(9f, 9f),
                    ModContent.ProjectileType<MicrocellStar>(), (int)(Projectile.damage * 0.75f), Projectile.knockBack * 0.5f, Projectile.owner);
        }
    }

    /// <summary>Étoile de la Microcell Cut (sprite de la Hallow Star vanilla, comme l'image de la page) : cherche les ennemis.</summary>
    public class MicrocellStar : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.HallowStar;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 240;
            Projectile.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
        }

        public override void AI()
        {
            Projectile.rotation += 0.25f * Projectile.direction;
            Lighting.AddLight(Projectile.Center, 0.6f, 0.3f, 0.8f);
            if (Main.rand.NextBool(3))
                Dust.NewDustPerfect(Projectile.Center, DustID.PurpleTorch, Vector2.Zero, 100, default, 1.2f).noGravity = true;
            if (Projectile.timeLeft < 225)
                CalamityUtils.HomeInOnNPC(Projectile, true, 900f, 14f, 20f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<AncientAstralBlight>(), 240);
    }
}

namespace CalamityFandom.Content.Rarities
{
    /// <summary>Rareté « C4 » du wiki (fantag, page Rarity) : violet (216, 0, 255) de son image.</summary>
    public class RarityC4 : ModRarity
    {
        public override Color RarityColor => new(216, 0, 255);
    }
}

namespace CalamityFandom.Content.Buffs
{
    /// <summary>
    /// Wiki : Ancient Astral Blight.txt — debuff : pas de régénération ; ennemi -52 PV/s et -12 défense ; joueur
    /// -14 PV/s et -12 défense ; particules jaunes et violettes. [choix] Infligé pour l'instant par la Microcell Cut ;
    /// Astrum Deus et ses armes (page) restent à faire.
    /// </summary>
    public class AncientAstralBlight : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<AncientAstralBlightPlayer>().Active = true;
            player.statDefense -= 12;
            Particles(player.position, player.width, player.height);
        }

        public override void Update(NPC npc, ref int buffIndex) => Particles(npc.position, npc.width, npc.height);

        internal static void Particles(Vector2 position, int width, int height)
        {
            if (Main.dedServ || !Main.rand.NextBool(3))
                return;
            var dust = Dust.NewDustDirect(position, width, height, Main.rand.NextBool() ? DustID.YellowTorch : DustID.PurpleTorch, 0f, -1f, 100, default, 1.3f);
            dust.noGravity = true;
        }
    }

    public class AncientAstralBlightPlayer : ModPlayer
    {
        public bool Active;

        public override void ResetEffects() => Active = false;

        public override void UpdateBadLifeRegen()
        {
            if (!Active)
                return;
            if (Player.lifeRegen > 0)
                Player.lifeRegen = 0;
            Player.lifeRegenTime = 0;
            Player.lifeRegen -= 28; // -14 PV/s (lifeRegen compte des demi-PV par seconde)
        }
    }

    public class AncientAstralBlightNPC : GlobalNPC
    {
        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (!npc.HasBuff(ModContent.BuffType<AncientAstralBlight>()))
                return;
            if (npc.lifeRegen > 0)
                npc.lifeRegen = 0;
            npc.lifeRegen -= 104; // -52 PV/s
            if (damage < 13)
                damage = 13;
        }

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (npc.HasBuff(ModContent.BuffType<AncientAstralBlight>()))
                modifiers.Defense.Flat -= 12;
        }
    }
}
