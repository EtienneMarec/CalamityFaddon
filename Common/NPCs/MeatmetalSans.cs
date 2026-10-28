using System;
using CalamityMod.NPCs.Ravager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityFandom.Common.NPCs
{
    /// <summary>
    /// Wiki : Ravager.txt — le Ravager est devenu « Meatmetal Sans » (parodie de Sans d'Undertale) : il esquive tout ;
    /// chaque attaque tentée fait avancer un compteur d'un cran (12 en phase 1, 23 au total) ; après chaque esquive, il
    /// projette le joueur au loin (« BDSM ») ; toutes ses attaques font 1 dégât et empoisonnent.
    /// Version minimale, tous les modes (décision du 21/10) : 23 PV = le compteur, un cran par coup, « MISS » à la place
    /// des dégâts, puis 1,5 s d'esquive ([choix], test du 28/10 : 0,5 s était trop court). Les dégâts sur la durée
    /// (debuffs) et les coups directs hors esquive ne comptent pas. Le vrai combat (os, Gaster Blasters, écran noir,
    /// Handheld Tank...) est dans docs/ROADMAP.md.
    /// </summary>
    public class MeatmetalSans : GlobalNPC
    {
        public const int HitsToKill = 23;
        public const int DodgeTicks = 90;
        /// <summary>[choix] Vitesse donnée au joueur projeté après une esquive.</summary>
        public const float LaunchSpeed = 12f;

        public override bool InstancePerEntity => true;

        private uint lastHit;
        /// <summary>Vie autorisée : ne baisse que par un coup compté.</summary>
        private int allowedLife = HitsToKill;

        public static bool IsSans(NPC npc) => npc.ModNPC is { Mod.Name: "CalamityMod", Name: "RavagerBody" };

        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => lateInstantiation && IsSans(entity);

        /// <summary>Phase 1 tant que moins de 12 coups ont été comptés (page du wiki).</summary>
        public const int PhaseOneHits = 12;
        /// <summary>Échelle des sprites (test du 28/10) : Sans presque à taille humaine (180 px de haut -> 63, le joueur en fait
        /// 42), le Ravager refait à x0,64 de l'image du wiki (668 px de large -> 428, deuxième réduction du 28/10).</summary>
        public const float PhaseOneScale = 0.35f, PhaseTwoScale = 0.64f;
        /// <summary>Hitbox de la phase 1 : le sprite de Sans réduit (120 x 180 x 0,35).</summary>
        private static readonly Point PhaseOneSize = new(40, 60);

        private Point phaseTwoSize;
        private bool swappedTextures;
        private Asset<Texture2D> savedLegLeft, savedLegRight, savedHead, savedGlow;

        public static bool InPhaseOne(NPC npc) => HitsToKill - npc.life < PhaseOneHits;

        public override void SetDefaults(NPC npc)
        {
            npc.lifeMax = HitsToKill;
            npc.defense = 0;
            // La barre « vanilla » de Calamity ajoute toujours la vie des 5 parties du Ravager (21 023 au test du 28/10) :
            // barre standard du jeu, qui ne compte que Sans
            npc.BossBar = null;
        }

        // Le jeu adapte la vie au mode et au nombre de joueurs : on garde exactement 23 coups
        public override void ApplyDifficultyAndPlayerScaling(NPC npc, int numPlayers, float balance, float bossAdjustment) =>
            npc.lifeMax = HitsToKill;

        // Le corps de Calamity crée tête, griffes et jambes à sa première IA (localAI[0] == 0) : on l'en empêche. Sinon la
        // barre de vie de Calamity les compte (21 023 PV affichés, test du 28/10) même retirées aussitôt
        public override bool PreAI(NPC npc)
        {
            if (npc.localAI[0] == 0f)
                npc.localAI[0] = 1f;
            return true;
        }

        public override void PostAI(NPC npc)
        {
            if (npc.life < allowedLife && npc.life > 0)
                npc.life = allowedLife;

            // Hitbox du sprite de la phase en cours, posée au même endroit au sol
            if (phaseTwoSize == Point.Zero)
                phaseTwoSize = new Point(npc.width, npc.height);
            Point size = InPhaseOne(npc) ? PhaseOneSize : phaseTwoSize;
            if (npc.width != size.X || npc.height != size.Y)
            {
                Vector2 bottom = npc.Bottom;
                npc.width = size.X;
                npc.height = size.Y;
                npc.Bottom = bottom;
            }
        }

        // Sprites du wiki (tous les modes, décision du 28/10) : Sans en phase 1, Ravager refait en phase 2. Le corps de
        // Calamity dessine aussi jambes, tête et lueur dans son PostDraw : leurs textures sont vidées le temps du dessin
        private static Asset<Texture2D> PhaseTexture(bool phaseOne) =>
            ModContent.Request<Texture2D>($"CalamityFandom/Assets/Bosses/MeatmetalSans_Phase{(phaseOne ? 1 : 2)}");

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (npc.IsABestiaryIconDummy)
                return true;
            var empty = ModContent.Request<Texture2D>("CalamityMod/Projectiles/InvisibleProj");
            int legLeft = ModContent.NPCType<RavagerLegLeft>(), legRight = ModContent.NPCType<RavagerLegRight>(), head = ModContent.NPCType<RavagerHead>();
            (savedLegLeft, savedLegRight, savedHead, savedGlow) = (TextureAssets.Npc[legLeft], TextureAssets.Npc[legRight], TextureAssets.Npc[head], RavagerBody.GlowTexture);
            TextureAssets.Npc[legLeft] = TextureAssets.Npc[legRight] = TextureAssets.Npc[head] = empty;
            RavagerBody.GlowTexture = empty;
            swappedTextures = true;

            bool phaseOne = InPhaseOne(npc);
            Texture2D texture = PhaseTexture(phaseOne).Value;
            spriteBatch.Draw(texture, npc.Bottom - screenPos + new Vector2(0f, npc.gfxOffY), null, npc.GetAlpha(drawColor), 0f,
                new Vector2(texture.Width / 2f, texture.Height), phaseOne ? PhaseOneScale : PhaseTwoScale, SpriteEffects.None, 0f);
            return false;
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!swappedTextures)
                return;
            TextureAssets.Npc[ModContent.NPCType<RavagerLegLeft>()] = savedLegLeft;
            TextureAssets.Npc[ModContent.NPCType<RavagerLegRight>()] = savedLegRight;
            TextureAssets.Npc[ModContent.NPCType<RavagerHead>()] = savedHead;
            RavagerBody.GlowTexture = savedGlow;
            swappedTextures = false;
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (npc.lifeRegen < 0)
                npc.lifeRegen = 0;
            damage = 0;
        }

        private bool Dodging => Main.GameUpdateCount - lastHit < DodgeTicks;

        public override bool? CanBeHitByItem(NPC npc, Player player, Item item) => Dodging ? false : null;

        public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile) => Dodging ? false : null;

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            modifiers.SetMaxDamage(1);
            modifiers.DisableCrit();
            modifiers.HideCombatText();
        }

        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone) => Dodge(npc, player);

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone) =>
            Dodge(npc, projectile.owner >= 0 && projectile.owner < Main.maxPlayers ? Main.player[projectile.owner] : null);

        /// <summary>Coup compté : « MISS », esquive, et le joueur est projeté loin de Sans.</summary>
        private void Dodge(NPC npc, Player attacker)
        {
            lastHit = Main.GameUpdateCount;
            allowedLife = npc.life;
            CombatText.NewText(npc.Hitbox, Color.LightGray, "MISS", true);
            // Fin de la phase 1 : Sans se téléporte plus loin du joueur (page du wiki) ; [choix] 600 px à l'opposé
            if (HitsToKill - npc.life == PhaseOneHits && Main.netMode != NetmodeID.MultiplayerClient)
            {
                float side = attacker != null && attacker.Center.X > npc.Center.X ? -1f : 1f;
                for (int i = 0; i < 30; i++)
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.BlueTorch, 0f, 0f, 100, default, 2f);
                npc.position.X += side * 600f;
                npc.netUpdate = true;
            }
            if (attacker is { active: true } && attacker.whoAmI == Main.myPlayer)
            {
                Vector2 away = (attacker.Center - npc.Center).SafeNormalize(Vector2.UnitX * attacker.direction);
                attacker.velocity = new Vector2(away.X * LaunchSpeed, Math.Min(away.Y * LaunchSpeed, -6f));
            }
        }

        public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers) => MeatmetalSansAttack.OneDamage(ref modifiers);

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo) => target.AddBuff(BuffID.Poisoned, 300);
    }

    /// <summary>Attaques de Meatmetal Sans : 1 dégât exactement (même avec la réduction de dégâts de Calamity, qui faisait
    /// tomber le coup à 0, test du 28/10) et Poisoned 5 s.</summary>
    public static class MeatmetalSansAttack
    {
        public static void OneDamage(ref Player.HurtModifiers modifiers) => modifiers.ModifyHurtInfo += (ref Player.HurtInfo info) => info.Damage = 1;

        public static bool FromSans(IEntitySource source) => source is EntitySource_Parent parent && parent.Entity switch
        {
            NPC npc => MeatmetalSans.IsSans(npc) || npc.TryGetGlobalNPC(out MeatmetalSansSummon summon) && summon.FromSans,
            _ => false,
        };
    }

    /// <summary>PNJ créés par Sans (piliers de roche et de flammes) : 1 dégât et poison aussi (200 dégâts au test du 28/10).</summary>
    public class MeatmetalSansSummon : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool FromSans;

        public override void OnSpawn(NPC npc, IEntitySource source) => FromSans = MeatmetalSansAttack.FromSans(source);

        public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
        {
            if (FromSans)
                MeatmetalSansAttack.OneDamage(ref modifiers);
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            if (FromSans)
                target.AddBuff(BuffID.Poisoned, 300);
        }
    }

    /// <summary>
    /// Tête, griffes et jambes du Ravager de Calamity : absentes du Meatmetal Sans du wiki (« he will stand in place ») ;
    /// Sans ne les crée plus (MeatmetalSans.PreAI), ceci retire celles qui apparaîtraient quand même, sans butin.
    /// </summary>
    public class MeatmetalSansParts : GlobalNPC
    {
        private static readonly string[] Parts = { "RavagerHead", "RavagerHead2", "RavagerClawLeft", "RavagerClawRight", "RavagerLegLeft", "RavagerLegRight" };

        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) =>
            lateInstantiation && entity.ModNPC is { Mod.Name: "CalamityMod" } modNPC && Array.IndexOf(Parts, modNPC.Name) >= 0;

        public override bool PreAI(NPC npc)
        {
            npc.active = false;
            npc.life = 0;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.SyncNPC, number: npc.whoAmI);
            return false;
        }
    }

    /// <summary>Les projectiles de Meatmetal Sans (et de ses piliers) font aussi 1 dégât et empoisonnent.</summary>
    public class MeatmetalSansProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        private bool fromSans;

        public override void OnSpawn(Projectile projectile, IEntitySource source) => fromSans = MeatmetalSansAttack.FromSans(source);

        public override void ModifyHitPlayer(Projectile projectile, Player target, ref Player.HurtModifiers modifiers)
        {
            if (fromSans)
                MeatmetalSansAttack.OneDamage(ref modifiers);
        }

        public override void OnHitPlayer(Projectile projectile, Player target, Player.HurtInfo info)
        {
            if (fromSans)
                target.AddBuff(BuffID.Poisoned, 300);
        }
    }
}
