using CalamityFandom.Common.Shortstrike;
using CalamityFandom.Content.Legacy.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Content.Items.Weapons.Melee
{
    /// <summary>
    /// Wiki : Wulfrum Leech Dagger.txt — épée courte consommable inventée par le Fandom (pile de 999) : toujours 1 dégât, pas de
    /// critique, et chaque coup retire 0,5 % de la vie max de l'ennemi sans défense ni réduction (comme la Coldheart Icicle
    /// de Calamity) ; seulement la tête des vers, pas les mannequins. Shortflurry : non consommable 3 s (effet
    /// « unconsumable » de la page Shortstrike).
    /// </summary>
    public class WulfrumLeechDagger : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 99;

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = ModContent.GetInstance<CalamityMod.TrueMeleeDamageClass>();
            Item.useAnimation = Item.useTime = 56;
            Item.width = Item.height = 24;
            Item.damage = 1;
            Item.knockBack = 0f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.consumable = true;
            Item.maxStack = 999;
            Item.shoot = ModContent.ProjectileType<WulfrumLeechDaggerProj>();
            Item.shootSpeed = 2f;
            Item.value = Item.sellPrice(copper: 10);
            Item.rare = ItemRarityID.Blue;
        }

        // « Shortflurry attacks make the weapon unconsumable for 3 seconds »
        public override bool ConsumeItem(Player player) => !player.GetModPlayer<ShortstrikePlayer>().Unconsumable;

        // « does not benefit from increased damage or critical strike chance »
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit = 0f;
    }
}

namespace CalamityFandom.Content.Legacy.Projectiles.Melee
{
    using global::CalamityFandom.Content.Items.Weapons.Melee;

    /// <summary>Estoc de la Wulfrum Leech Dagger : 1 dégât, puis 0,5 % de la vie max retiré à part.</summary>
    public class WulfrumLeechDaggerProj : LegacyShortswordProj
    {
        protected override int ItemType => ModContent.ItemType<WulfrumLeechDagger>();
        protected override int DustType => DustID.Iron;

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.DisableCrit();
            modifiers.SetMaxDamage(1);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Tête des vers seulement ; ni les mannequins ni ce qui ne peut pas prendre de dégâts
            bool segment = target.realLife >= 0 && target.realLife != target.whoAmI;
            if (Projectile.owner != Main.myPlayer || segment || target.immortal || target.dontTakeDamage || target.type == NPCID.TargetDummy || !target.active)
                return;
            target.SimpleStrikeNPC(System.Math.Max(1, target.lifeMax / 200), Main.player[Projectile.owner].direction);
        }
    }
}
