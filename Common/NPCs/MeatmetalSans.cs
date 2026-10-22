using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Common.NPCs
{
    /// <summary>
    /// Wiki : Ravager.txt — le Ravager est devenu « Meatmetal Sans » (parodie de Sans d'Undertale) : 1 PV, mais il esquive
    /// et ne meurt qu'après 23 coups (12 en phase 1, 10 en phase 2) ; toutes ses attaques font 1 dégât et empoisonnent.
    /// Version minimale, tous les modes (décision du 21/10) : 23 PV, 1 dégât par coup, esquive 0,5 s après chaque coup.
    /// Le vrai combat (os, Gaster Blasters, écran noir, Handheld Tank...) est dans docs/ROADMAP.md.
    /// </summary>
    public class MeatmetalSans : GlobalNPC
    {
        public const int HitsToKill = 23;
        public const int DodgeTicks = 30;

        public override bool InstancePerEntity => true;

        private uint lastHit;

        public static bool IsSans(NPC npc) => npc.ModNPC is { Mod.Name: "CalamityMod", Name: "RavagerBody" };

        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => lateInstantiation && IsSans(entity);

        public override void SetDefaults(NPC npc)
        {
            npc.lifeMax = HitsToKill;
            npc.defense = 0;
        }

        // Le jeu adapte la vie au mode et au nombre de joueurs : on garde exactement 23 coups
        public override void ApplyDifficultyAndPlayerScaling(NPC npc, int numPlayers, float balance, float bossAdjustment) =>
            npc.lifeMax = HitsToKill;

        private bool Dodging => Main.GameUpdateCount - lastHit < DodgeTicks;

        public override bool? CanBeHitByItem(NPC npc, Player player, Item item) => Dodging ? false : null;

        public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile) => Dodging ? false : null;

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers) => modifiers.SetMaxDamage(1);

        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone) => lastHit = Main.GameUpdateCount;

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone) => lastHit = Main.GameUpdateCount;

        public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers) => modifiers.SetMaxDamage(1);

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo) => target.AddBuff(BuffID.Poisoned, 300);
    }

    /// <summary>
    /// Tête, griffes et jambes du Ravager de Calamity : absentes du Meatmetal Sans du wiki (« he will stand in place »),
    /// et tant qu'elles vivent le corps est invulnérable (barre de vie à 210 023 au lieu de 23, test du 22/10). Retirées
    /// dès leur apparition, sans butin ([choix], en attendant le vrai combat : voir docs/ROADMAP.md).
    /// </summary>
    public class MeatmetalSansParts : GlobalNPC
    {
        private static readonly string[] Parts = { "RavagerHead", "RavagerHead2", "RavagerClawLeft", "RavagerClawRight", "RavagerLegLeft", "RavagerLegRight" };

        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) =>
            lateInstantiation && entity.ModNPC is { Mod.Name: "CalamityMod" } modNPC && System.Array.IndexOf(Parts, modNPC.Name) >= 0;

        public override bool PreAI(NPC npc)
        {
            npc.active = false;
            npc.life = 0;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.SyncNPC, number: npc.whoAmI);
            return false;
        }
    }

    /// <summary>Les projectiles de Meatmetal Sans font aussi 1 dégât et empoisonnent.</summary>
    public class MeatmetalSansProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        private bool fromSans;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_Parent { Entity: NPC npc } && MeatmetalSans.IsSans(npc))
                fromSans = true;
        }

        public override void ModifyHitPlayer(Projectile projectile, Player target, ref Player.HurtModifiers modifiers)
        {
            if (fromSans)
                modifiers.SetMaxDamage(1);
        }

        public override void OnHitPlayer(Projectile projectile, Player target, Player.HurtInfo info)
        {
            if (fromSans)
                target.AddBuff(BuffID.Poisoned, 300);
        }
    }
}
