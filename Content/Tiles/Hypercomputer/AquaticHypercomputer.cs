using System.Collections.Generic;
using CalamityFandom.Content.Items.Placeables;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

// Wiki : Aquatic Hypercomputer.txt — station de fabrication en 6 étapes (page d'addon « Tales of the Aquatic », traitée
// comme les autres, décisions section 11). Chaque module se pose sur la station déjà placée et la fait passer à l'étape
// suivante ; chaque étape a ses recettes (pages Recipes/Aquatic Hypercomputer <étape>) et garde celles d'avant.

namespace CalamityFandom.Content.Tiles.Hypercomputer
{
    /// <summary>
    /// Étape de l'Aquatic Hypercomputer (1 = la station seule ... 6 = Abyssblast Laser Module). Tuile de 4 x 3 (infobox) ;
    /// [technique] l'image du wiki (82 x 74) ne tombe pas sur la grille : elle est dessinée en entier depuis la case du coin,
    /// posée sur le sol et centrée. Le nom de la classe = nom de la station dans les recettes (WikiRecipes, sans espaces).
    /// </summary>
    public abstract class HypercomputerTile : ModTile
    {
        /// <summary>Étape (1 à 6).</summary>
        public abstract int Stage { get; }

        public override string Texture => "CalamityFandom/Content/Tiles/Hypercomputer/HypercomputerBlank";

        /// <summary>Tuiles des étapes, de 1 à 6.</summary>
        public static int[] StageTiles => new[]
        {
            ModContent.TileType<AquaticHypercomputer>(), ModContent.TileType<AquaticHypercomputerStatic>(), ModContent.TileType<AquaticHypercomputerVault>(),
            ModContent.TileType<AquaticHypercomputerCapsules>(), ModContent.TileType<AquaticHypercomputerERM>(), ModContent.TileType<AquaticHypercomputerFinished>(),
        };

        /// <summary>Objets qui ont mené à chaque étape (la station, puis les 5 modules) : rendus quand on la casse.</summary>
        public static int[] StageItems => new[]
        {
            ModContent.ItemType<Items.Placeables.AquaticHypercomputer>(), ModContent.ItemType<StaticHypermodule>(), ModContent.ItemType<CapsuleVaultModule>(),
            ModContent.ItemType<ElementalCapsuleBox>(), ModContent.ItemType<EnchantedReconstructionModule>(), ModContent.ItemType<AbyssblastLaserModule>(),
        };

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileLighted[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16 };
            TileObjectData.newTile.Origin = new Point16(1, 2);
            TileObjectData.newTile.HookPostPlaceMyPlayer = ModContent.GetInstance<HypercomputerEntity>().Generic_HookPostPlaceMyPlayer;
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(51, 51, 170), Language.GetText("Mods.CalamityFandom.Items.AquaticHypercomputer.DisplayName"));
            DustType = DustID.Electric;
            // Les étapes d'avant restent utilisables ; la dernière fait aussi Draedon's Forge (page)
            var adj = new List<int>();
            for (int s = 1; s < Stage; s++)
                adj.Add(StageTiles[s - 1]);
            if (Stage == 6)
            {
                adj.Add(ModContent.TileType<DraedonsForge>());
                adj.AddRange(new[] { TileID.WorkBenches, TileID.Chairs, TileID.Tables, TileID.Anvils, TileID.MythrilAnvil, ModContent.TileType<CosmicAnvil>(),
                    TileID.Furnaces, TileID.Hellforge, TileID.AdamantiteForge, TileID.TinkerersWorkbench, TileID.LunarCraftingStation, TileID.DemonAltar });
            }
            AdjTiles = adj.ToArray();
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.1f;
            g = 0.25f;
            b = 0.5f;
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            // La station et les modules posés
            for (int s = 0; s < Stage; s++)
                yield return new Item(StageItems[s]);
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY) => ModContent.GetInstance<HypercomputerEntity>().Kill(i, j);

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameX != 0 || tile.TileFrameY != 0)
                return false;
            string path = "CalamityFandom/Content/Tiles/Hypercomputer/" + GetType().Name + "_Placed";
            Texture2D tex = ModContent.Request<Texture2D>(path).Value;
            int frames = Stage == 1 ? 6 : 1;
            int frameHeight = tex.Height / frames;
            int frame = frames > 1 ? (int)(Main.GameUpdateCount / 8 % frames) : 0;
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            // Posée sur le sol (bas des 3 cases), centrée sur les 4 cases
            Vector2 pos = new Vector2(i * 16 + 32 - tex.Width / 2f, j * 16 + 48 - frameHeight) - Main.screenPosition + zero;
            spriteBatch.Draw(tex, pos, new Rectangle(0, frame * frameHeight, tex.Width, frameHeight), Lighting.GetColor(i + 1, j + 1), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            return false;
        }

        /// <summary>Coin haut gauche de la station qui contient la case (i, j), ou null.</summary>
        public static Point? TopLeft(int i, int j)
        {
            if (!WorldGen.InWorld(i, j))
                return null;
            Tile tile = Main.tile[i, j];
            if (!tile.HasTile || TileLoader.GetTile(tile.TileType) is not HypercomputerTile)
                return null;
            return new Point(i - tile.TileFrameX / 18, j - tile.TileFrameY / 18);
        }

        public static int StageAt(Point topLeft) =>
            TileLoader.GetTile(Main.tile[topLeft.X, topLeft.Y].TileType) is HypercomputerTile h ? h.Stage : 0;

        /// <summary>Remplace les 12 cases par l'étape suivante (les images des cases ne changent pas).</summary>
        public static void Upgrade(Point topLeft, int newStage)
        {
            int type = StageTiles[newStage - 1];
            for (int x = 0; x < 4; x++)
                for (int y = 0; y < 3; y++)
                    Main.tile[topLeft.X + x, topLeft.Y + y].TileType = (ushort)type;
            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendTileSquare(-1, topLeft.X, topLeft.Y, 4, 3);
            SoundEngine.PlaySound(SoundID.Item37, new Vector2(topLeft.X * 16 + 32, topLeft.Y * 16 + 24));
            for (int d = 0; d < 30; d++)
                Dust.NewDust(new Vector2(topLeft.X * 16, topLeft.Y * 16), 64, 48, DustID.Electric, 0f, -1f, 100, default, 1.2f);
        }
    }

    public class AquaticHypercomputer : HypercomputerTile
    {
        public override int Stage => 1;
    }

    /// <summary>Static Hypermodule : objets de fin de pré-Hardmode et de début de Hardmode.</summary>
    public class AquaticHypercomputerStatic : HypercomputerTile
    {
        public override int Stage => 2;
    }

    /// <summary>Capsule Vault Module : tire un laser faible qui ralentit les ennemis à 25 blocs.</summary>
    public class AquaticHypercomputerVault : HypercomputerTile
    {
        public override int Stage => 3;
    }

    /// <summary>Elemental Capsule Box : objets d'après le Moon Lord.</summary>
    public class AquaticHypercomputerCapsules : HypercomputerTile
    {
        public override int Stage => 4;
    }

    /// <summary>Enchanted Reconstruction Module : objets d'après le Devourer of Gods.</summary>
    public class AquaticHypercomputerERM : HypercomputerTile
    {
        public override int Stage => 5;
    }

    /// <summary>Abyssblast Laser Module : objets d'après l'Abyssal Jewel, aura d'Evercold de 20 x 20 blocs, fait Draedon's Forge.</summary>
    public class AquaticHypercomputerFinished : HypercomputerTile
    {
        public override int Stage => 6;
    }

    /// <summary>
    /// Défenses de l'Hypercomputer (serveur ou solo) : à partir du Capsule Vault Module, un laser faible qui ralentit vers
    /// l'ennemi le plus proche à 25 blocs (toutes les 40 images) ; avec l'Abyssblast Laser Module, Evercold aux ennemis
    /// dans un carré de 20 x 20 blocs. [choix] Laser de 20 dégâts, Slow 2 s ; Evercold 2 s renouvelé chaque seconde.
    /// </summary>
    public class HypercomputerEntity : ModTileEntity
    {
        private int timer;

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && TileLoader.GetTile(tile.TileType) is HypercomputerTile && tile.TileFrameX == 0 && tile.TileFrameY == 0;
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            // i, j : case d'origine (1, 2) ; l'entité va au coin haut gauche
            int x = i - 1, y = j - 2;
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(Main.myPlayer, x, y, 4, 3);
                NetMessage.SendData(MessageID.TileEntityPlacement, number: x, number2: y, number3: Type);
                return -1;
            }
            return Place(x, y);
        }

        public override void Update()
        {
            int stage = HypercomputerTile.StageAt(new Point(Position.X, Position.Y));
            if (stage < 3)
                return;
            Vector2 center = new(Position.X * 16 + 32, Position.Y * 16 + 8);
            timer++;
            if (stage >= 6 && timer % 60 == 0)
            {
                var area = new Rectangle((int)center.X - 160, (int)center.Y + 16 - 160, 320, 320);
                foreach (NPC npc in Main.ActiveNPCs)
                    if (!npc.friendly && !npc.dontTakeDamage && npc.lifeMax > 5 && npc.Hitbox.Intersects(area))
                        npc.AddBuff(ModContent.BuffType<Buffs.Evercold>(), 120);
            }
            if (timer % 40 != 0)
                return;
            NPC target = null;
            float best = 400f;
            foreach (NPC npc in Main.ActiveNPCs)
                if (npc.CanBeChasedBy() && npc.Distance(center) < best && Collision.CanHitLine(center, 1, 1, npc.Center, 1, 1))
                {
                    best = npc.Distance(center);
                    target = npc;
                }
            if (target != null)
                Projectile.NewProjectile(new EntitySource_TileEntity(this), center, (target.Center - center).SafeNormalize(Vector2.UnitY) * 10f,
                    ModContent.ProjectileType<HypercomputerLaser>(), 20, 0f, Main.myPlayer);
        }
    }

    /// <summary>Laser de l'Hypercomputer : faible, ralentit (Slow 2 s).</summary>
    public class HypercomputerLaser : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.GreenLaser;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.extraUpdates = 2;
        }

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        public override Color? GetAlpha(Color lightColor) => new Color(90, 160, 255, 0);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Slow, 120);
    }
}

namespace CalamityFandom.Content.Items.Placeables
{
    /// <summary>
    /// Wiki : Aquatic Hypercomputer.txt — la station (étape 1). [choix] Prix de vente de l'infobox ({{value|27|50}} :
    /// 27 platine 50 or) gardé tel quel.
    /// </summary>
    public class AquaticHypercomputer : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Hypercomputer.AquaticHypercomputer>());
            Item.width = 42;
            Item.height = 28;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(platinum: 27, gold: 50);
        }
    }

    /// <summary>
    /// Module de l'Aquatic Hypercomputer : utilisé sur une station placée à l'étape précédente (à portée de pose), il
    /// est consommé et la station passe à l'étape <see cref="Stage"/>. [choix] Pas de prix de vente (le tableau n'en donne pas).
    /// </summary>
    public abstract class HypercomputerModule : ModItem
    {
        public abstract int Stage { get; }
        protected abstract int Rarity { get; }

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = Item.height = 24;
            Item.maxStack = Item.CommonMaxStack;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = Item.useAnimation = 20;
            Item.useTurn = true;
            Item.autoReuse = false;
            Item.consumable = true;
            Item.rare = Rarity;
        }

        private Point? Target(Player player)
        {
            Point mouse = Main.MouseWorld.ToTileCoordinates();
            if (player.Distance(Main.MouseWorld) > 16f * (Player.tileRangeX + 2))
                return null;
            Point? topLeft = Tiles.Hypercomputer.HypercomputerTile.TopLeft(mouse.X, mouse.Y);
            return topLeft is Point p && Tiles.Hypercomputer.HypercomputerTile.StageAt(p) == Stage - 1 ? topLeft : null;
        }

        public override bool CanUseItem(Player player) => player.whoAmI != Main.myPlayer || Target(player) != null;

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI != Main.myPlayer || Target(player) is not Point topLeft)
                return false;
            Tiles.Hypercomputer.HypercomputerTile.Upgrade(topLeft, Stage);
            return true;
        }
    }

    /// <summary>Wiki : Aquatic Hypercomputer.txt (Static Hypermodule) — étape 2.</summary>
    public class StaticHypermodule : HypercomputerModule
    {
        public override int Stage => 2;
        protected override int Rarity => ItemRarityID.Orange;
    }

    /// <summary>Wiki : Aquatic Hypercomputer.txt (Capsule Vault Module) — étape 3 : laser qui ralentit.</summary>
    public class CapsuleVaultModule : HypercomputerModule
    {
        public override int Stage => 3;
        protected override int Rarity => ItemRarityID.Pink;
    }

    /// <summary>Wiki : Aquatic Hypercomputer.txt (Elemental Capsule Box) — étape 4.</summary>
    public class ElementalCapsuleBox : HypercomputerModule
    {
        public override int Stage => 4;
        protected override int Rarity => ItemRarityID.Red;
    }

    /// <summary>Wiki : Aquatic Hypercomputer.txt (Enchanted Reconstruction Module) — étape 5.</summary>
    public class EnchantedReconstructionModule : HypercomputerModule
    {
        public override int Stage => 5;
        protected override int Rarity => ModContent.RarityType<CalamityMod.Rarities.CosmicPurple>();
    }

    /// <summary>Wiki : Aquatic Hypercomputer.txt (Abyssblast Laser Module) — étape 6 : aura d'Evercold, fait Draedon's Forge.</summary>
    public class AbyssblastLaserModule : HypercomputerModule
    {
        public override int Stage => 6;
        protected override int Rarity => ModContent.RarityType<CalamityMod.Rarities.BurnishedAuric>();
    }
}

namespace CalamityFandom.Content.Buffs
{
    /// <summary>
    /// Wiki : Evercold.txt — joueur : pas de régénération, -30 PV/s, -20 défense, -20 % de réduction des dégâts, -40 % de
    /// vitesse ; ennemi : -200 PV/s, -20 % de réduction des dégâts (Calamity) et -20 % de vitesse.
    /// </summary>
    public class Evercold : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.lifeRegen > 0)
                player.lifeRegen = 0;
            player.lifeRegenTime = 0;
            player.lifeRegen -= 60;
            player.statDefense -= 20;
            player.endurance -= 0.2f;
            player.moveSpeed -= 0.4f;
        }

        public override void Update(NPC npc, ref int buffIndex) => npc.GetGlobalNPC<EvercoldNPC>().Active = true;
    }

    public class EvercoldNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public bool Active;

        public override void ResetEffects(NPC npc) => Active = false;

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (!Active)
                return;
            if (npc.lifeRegen > 0)
                npc.lifeRegen = 0;
            npc.lifeRegen -= 400;
            if (damage < 40)
                damage = 40;
        }

        public override void PostAI(NPC npc)
        {
            if (Active && !npc.boss)
                npc.position -= npc.velocity * 0.2f;
        }

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            // [choix] « -20 % de réduction des dégâts » : chaque coup fait 20 % de plus
            if (Active)
                modifiers.FinalDamage *= 1.2f;
        }
    }
}
