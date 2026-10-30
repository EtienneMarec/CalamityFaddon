using System;
using System.Linq;
using CalamityMod;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Entry = CalamityFandom.Common.Taming.TamingSystem.Entry;
using Fight = CalamityFandom.Common.Taming.TamingSystem.Fight;

namespace CalamityFandom.Common.Taming
{
    /// <summary>
    /// Méthodes de la page Taming vérifiées à chaque tick (serveur ou solo). Les méthodes vues par un client (nourriture,
    /// livres, ruées, chat, dégâts reçus) sont dans <see cref="TamingPlayer"/> et <see cref="TamingGlobalTile"/>.
    /// </summary>
    public static class TamingMethods
    {
        /// <summary>Projectile tiré par l'objet (repli sur les ID connus si l'échantillon n'en a pas).</summary>
        public static int Shoot(int item)
        {
            int shoot = ContentSamples.ItemsByType[item].shoot;
            if (shoot > ProjectileID.None)
                return shoot;
            return item switch
            {
                ItemID.SlimeGun => ProjectileID.SlimeGun,
                ItemID.GelBalloon => ProjectileID.GelBalloon,
                ItemID.GreenSolution => ProjectileID.PureSpray,
                _ => ProjectileID.None,
            };
        }

        public static readonly int[] Boulders =
            { ProjectileID.Boulder, ProjectileID.BouncyBoulder, ProjectileID.LifeCrystalBoulder, ProjectileID.MoonBoulder, ProjectileID.MiniBoulder };

        /// <summary>Projectiles d'une méthode qui ne doivent pas blesser ce boss (sinon la tentative échoue).</summary>
        public static bool IsMethodProjectile(int projType, int npcType)
        {
            var e = TamingSystem.EntryOf(npcType);
            return e?.Key switch
            {
                "King Slime" => projType == Shoot(ItemID.SlimeGun),
                "Queen Slime" => projType == Shoot(ItemID.GelBalloon),
                "The Plaguebringer Goliath" => projType == Shoot(ItemID.GreenSolution),
                "Astrum Deus" => projType == ProjectileID.FallingStar,
                "Ceaseless Void" => Boulders.Contains(projType),
                _ => false,
            };
        }

        public static void Tick(Entry e, Fight f, NPC npc)
        {
            Player p = TamingSystem.Nearest(npc);
            switch (e.Key)
            {
                // « Spray King Slime with the Slime Gun for a combined total of 5 minutes »
                case "King Slime":
                    if (Touching(npc, Shoot(ItemID.SlimeGun), countOnce: false, kill: false) > 0)
                        f.Progress++;
                    break;
                // « Lead ... to the Ocean »
                case "Desert Scourge":
                case "Aquatic Scourge":
                    if (p != null && p.ZoneBeach)
                        f.Progress++;
                    break;
                case "Cryogen":
                    if (p != null && p.Calamity().ZoneCalamity)
                        f.Progress++;
                    break;
                // « Make 30 circles around the Eye of Cthulhu. All circles must be in the same direction »
                case "Eye of Cthulhu":
                    if (p == null)
                        break;
                    float angle = (p.Center - npc.Center).ToRotation();
                    if (f.Flag)
                    {
                        f.B += MathHelper.WrapAngle(angle - f.A);
                        f.C = Math.Max(f.C, Math.Abs(f.B));
                        // Demi-tour (un quart de tour en arrière) : le compteur repart de zéro
                        if (Math.Abs(f.B) < f.C - MathHelper.PiOver2)
                            f.B = f.C = 0f;
                        f.Progress = (float)Math.Floor(Math.Abs(f.B) / MathHelper.TwoPi);
                    }
                    f.A = angle;
                    f.Flag = true;
                    break;
                // « Lead the Wall of Flesh to the Guide ... at most 50 blocks from each other ... the eyes and/or the mouth »
                case "Wall of Flesh":
                    foreach (var guide in Main.ActiveNPCs)
                        if (guide.type == NPCID.Guide && Main.npc.Any(w => w.active && e.Types.Contains(w.type) && w.Distance(guide.Center) <= 50 * 16))
                            f.Progress = e.Goal;
                    break;
                // « Throw 100 Sparkle Slime Balloons at Queen Slime »
                case "Queen Slime":
                    f.Progress += Touching(npc, Shoot(ItemID.GelBalloon), countOnce: true, kill: true);
                    break;
                // « Stare at each twin for at least 1 minute. You cannot break line of sight for more then 1 second »
                case "The Twins":
                    if (p == null)
                        break;
                    for (int i = 0; i < 2; i++)
                    {
                        NPC twin = Main.npc.FirstOrDefault(n => n.active && n.type == e.Types[i]);
                        bool sees = twin != null && p.direction == Math.Sign(twin.Center.X - p.Center.X)
                            && Collision.CanHitLine(p.position, p.width, p.height, twin.position, twin.width, twin.height);
                        if (sees)
                        {
                            f.Ints[i] = Math.Min(f.Ints[i] + 1, (int)e.Goal);
                            f.Ints[i + 2] = 0;
                        }
                        else if (++f.Ints[i + 2] > 60)
                            f.Ints[i] = 0;
                    }
                    f.Progress = Math.Min(f.Ints[0], f.Ints[1]);
                    break;
                // « Have the Skull equipped » (pendant une minute d'affilée)
                case "Skeletron Prime":
                    if (Main.player.Any(pl => pl.active && !pl.dead && pl.Distance(npc.Center) < 1600f && HasSkull(pl)))
                        f.Progress++;
                    else
                        f.Progress = 0;
                    break;
                // « Survive on the surface with Plantera for 3 minutes »
                case "Plantera":
                    if (p != null && (p.ZoneOverworldHeight || p.ZoneSkyHeight))
                        f.Progress++;
                    else
                        f.Progress = 0;
                    break;
                // « Give Calamitas a hug » : une seconde au contact
                case "Calamitas":
                    if (p != null && p.Hitbox.Intersects(npc.Hitbox))
                        f.Progress++;
                    else
                        f.Progress = 0;
                    break;
                // « Use 200 Green Solution with the Clentaminator on the Plaguebringer Goliath »
                case "The Plaguebringer Goliath":
                    f.Progress += Touching(npc, Shoot(ItemID.GreenSolution), countOnce: true, kill: false);
                    break;
                // « Bring Duke Fishron to one naturally spawning Truffle Worm »
                case "Duke Fishron":
                    foreach (var worm in Main.ActiveNPCs)
                        if ((worm.type == NPCID.TruffleWorm || worm.type == NPCID.TruffleWormDigger) && worm.releaseOwner == 255
                            && worm.Distance(npc.Center) < 160f)
                            f.Progress = e.Goal;
                    break;
                // « Survive from dawn to dusk »
                case "Empress of Light":
                    if (Main.dayTime && !f.WasDay)
                        f.Flag = true;
                    if (!Main.dayTime && f.WasDay && f.Flag)
                        f.Progress = e.Goal;
                    f.WasDay = Main.dayTime;
                    break;
                // « Cause Ravager to fall into a pool of Shimmer »
                case "Ravager":
                    if (npc.shimmerWet)
                        f.Progress = e.Goal;
                    break;
                // « Have Astrum Deus be hit by a fallen star »
                case "Astrum Deus":
                    foreach (var seg in Main.ActiveNPCs)
                        if (e.Types.Contains(seg.type) && Touching(seg, ProjectileID.FallingStar, countOnce: true, kill: true) > 0)
                            f.Progress = e.Goal;
                    break;
                // « Give the Dragonfolly a Yharon Soul Fragment » : le fragment lâché au sol doit toucher l'oiseau
                case "The Dragonfolly":
                    if (Give(npc, ModContent.ItemType<YharonSoulFragment>()))
                        f.Progress = e.Goal;
                    break;
                // « Bring Moon Lord to the 4th layer of the abyss, and wait for a Colossal Squid to spawn » (f.Flag :
                // calmar apparu pendant le combat, TamingGlobalNPC.OnSpawn)
                case "Moon Lord":
                    if (f.Flag && Main.player.Any(pl => pl.active && !pl.dead && pl.Calamity().ZoneAbyssLayer4 && pl.Distance(npc.Center) < 4000f))
                        f.Progress = e.Goal;
                    break;
                // « Take Providence to all 4 corners of the world »
                case "Providence, the Profaned Goddess":
                    int corner = Corner(npc.Center);
                    if (corner >= 0)
                        f.Ints[0] |= 1 << corner;
                    f.Progress = System.Numerics.BitOperations.PopCount((uint)f.Ints[0]);
                    break;
                // « Allow Ceaseless Void to absorb 50 rolling Boulders »
                case "Ceaseless Void":
                    foreach (int boulder in Boulders)
                        f.Progress += Touching(npc, boulder, countOnce: true, kill: true);
                    break;
                // « Feed 10 Wyverns »
                case "Storm Weaver":
                    foreach (var wyvern in Main.ActiveNPCs)
                    {
                        if (wyvern.type != NPCID.WyvernHead || !wyvern.Hitbox.Intersects(npc.Hitbox))
                            continue;
                        foreach (var seg in Main.ActiveNPCs)
                            if (seg.realLife == wyvern.whoAmI && seg.whoAmI != wyvern.whoAmI)
                                Despawn(seg);
                        Despawn(wyvern);
                        f.Progress++;
                    }
                    break;
                case TamingSystem.SupremeCalamitasKey:
                    SupremeCalamitasQuit.Tick(f, npc);
                    break;
                // « Survive for 8 (Signus) / 5 (Yharon) minutes without taking damage » : TamingSystem.ReportHurt remet à zéro
                case "Signus, Envoy of the Devourer":
                case "Jungle Dragon, Yharon":
                    if (p != null)
                        f.Progress++;
                    break;
            }
        }

        private static bool HasSkull(Player p)
        {
            for (int i = 3; i < 10; i++)
                if (p.armor[i].type == ItemID.Skull || p.armor[i + 10].type == ItemID.Skull)
                    return true;
            return false;
        }

        /// <summary>Coin du monde (0 haut gauche, 1 haut droit, 2 bas gauche, 3 bas droit) où se trouve la position, ou -1.</summary>
        public static int Corner(Vector2 pos)
        {
            int x = (int)(pos.X / 16f), y = (int)(pos.Y / 16f);
            const int edge = 250;
            int col = x < edge ? 0 : x > Main.maxTilesX - edge ? 1 : -1;
            int row = y < Main.worldSurface * 0.35 ? 0 : y > Main.maxTilesY - 200 ? 1 : -1;
            return col < 0 || row < 0 ? -1 : row * 2 + col;
        }

        /// <summary>Projectiles de ce type qui touchent le PNJ ; countOnce : chacun ne compte qu'une fois.</summary>
        private static int Touching(NPC npc, int projType, bool countOnce, bool kill)
        {
            if (projType <= 0)
                return 0;
            int count = 0;
            foreach (var proj in Main.ActiveProjectiles)
            {
                if (proj.type != projType || !proj.Hitbox.Intersects(npc.Hitbox))
                    continue;
                var g = proj.GetGlobalProjectile<TamingGlobalProjectile>();
                if (countOnce && g.Counted)
                    continue;
                g.Counted = true;
                count++;
                if (kill)
                {
                    proj.Kill();
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.KillProjectile, number: proj.identity, number2: proj.owner);
                }
            }
            return count;
        }

        /// <summary>Un objet de ce type, au sol, touche le PNJ : il est consommé (un exemplaire).</summary>
        private static bool Give(NPC npc, int itemType)
        {
            for (int i = 0; i < Main.maxItems; i++)
            {
                Item item = Main.item[i];
                if (!item.active || item.type != itemType || !item.Hitbox.Intersects(npc.Hitbox))
                    continue;
                if (--item.stack <= 0)
                    item.TurnToAir();
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.SyncItem, number: i);
                return true;
            }
            return false;
        }

        private static void Despawn(NPC n)
        {
            n.life = 0;
            n.active = false;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.SyncNPC, number: n.whoAmI);
        }
    }
}
