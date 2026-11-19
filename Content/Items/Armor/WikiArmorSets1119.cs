using System;
using CalamityMod;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Dart = CalamityFandom.Content.Projectiles.WikiDarts.WikiDartProj;

// Lot AN (19/11) : ensembles d'armure du wiki (recettes du wiki). Le wiki n'a que les icônes : le rendu porté reprend une
// armure de Calamity (provisoire, listé dans docs/SPRITES.md) : Twisted Netherite = Fearmonger, Elderstone = Marnite
// Architect, Frozen = Daedalus (mêlée), Archangel = Aerospec (mêlée), Catarachedrae = Bloodflare (mêlée).

namespace CalamityFandom.Content.Items.Armor
{
    /// <summary>Pièce d'armure du wiki dont le rendu porté est la texture d'une armure de Calamity.</summary>
    public abstract class StandInArmor : ModItem
    {
        protected abstract EquipType Slot { get; }
        protected abstract string Worn { get; }
        protected abstract int Defense { get; }
        protected abstract int Rarity { get; }
        protected abstract int SellCopper { get; }
        protected virtual bool Vanity => false;

        public override void Load()
        {
            if (Main.dedServ)
                return;
            string suffix = Slot switch { EquipType.Head => "_Head", EquipType.Body => "_Body", _ => "_Legs" };
            EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Armor/" + Worn + suffix, Slot, this);
        }

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            if (Main.dedServ || Slot != EquipType.Body)
                return;
            int slot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
            ArmorIDs.Body.Sets.HidesTopSkin[slot] = true;
            ArmorIDs.Body.Sets.HidesArms[slot] = true;
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 18;
            Item.rare = Rarity;
            Item.value = SellCopper * 5;
            Item.defense = Defense;
            Item.vanity = Vanity;
            if (Main.dedServ)
                return;
            int slot = EquipLoader.GetEquipSlot(Mod, Name, Slot);
            switch (Slot)
            {
                case EquipType.Head: Item.headSlot = slot; break;
                case EquipType.Body: Item.bodySlot = slot; break;
                default: Item.legSlot = slot; break;
            }
        }
    }

    // ------------------------------------------------------------------ Twisted Netherite

    /// <summary>
    /// Wiki : Twisted Netherite armor.txt — 4 pièces (la 4e, des bottes, se porte en accessoire) : +50 % de dégâts et +12,5 %
    /// de réduction des dégâts chacune ; bottes +2 % de vitesse. Ensemble complet : toutes les attaques infligent Wither 10 s.
    /// « Les objets lâchés ne disparaissent plus » : la page dit que ça ne marche pas (bug) : non fait.
    /// </summary>
    public abstract class TwistedNetheritePiece : StandInArmor
    {
        protected override int Rarity => ModContent.RarityType<PureGreen>();
        protected override int SellCopper => 240000;

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.5f;
            player.endurance += 0.125f;
        }
    }

    public class TwistedNetheriteHelmet : TwistedNetheritePiece
    {
        protected override EquipType Slot => EquipType.Head;
        protected override string Worn => "Fearmonger/FearmongerGreathelm";
        protected override int Defense => 20;

        public override bool IsArmorSet(Item head, Item body, Item legs)
            => body.type == ModContent.ItemType<TwistedNetheriteChestplate>() && legs.type == ModContent.ItemType<TwistedNetheriteLeggings>();

        public override void UpdateArmorSet(Player player)
        {
            if (!player.GetModPlayer<WikiArmorPlayer>().NetheriteBoots)
                return;
            player.setBonus = Language.GetTextValue("Mods.CalamityFandom.WikiArmor.TwistedNetherite");
            player.GetModPlayer<WikiArmorPlayer>().NetheriteSet = true;
        }
    }

    public class TwistedNetheriteChestplate : TwistedNetheritePiece
    {
        protected override EquipType Slot => EquipType.Body;
        protected override string Worn => "Fearmonger/FearmongerPlateMail";
        protected override int Defense => 80;
    }

    public class TwistedNetheriteLeggings : TwistedNetheritePiece
    {
        protected override EquipType Slot => EquipType.Legs;
        protected override string Worn => "Fearmonger/FearmongerGreaves";
        protected override int Defense => 45;
    }

    /// <summary>Bottes en Twisted Netherite : accessoire (Terraria n'a pas d'emplacement de bottes), 30 défense.</summary>
    public class TwistedNetheriteBoots : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 10;
            Item.accessory = true;
            Item.defense = 30;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.value = 240000 * 5;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Generic) += 0.5f;
            player.endurance += 0.125f;
            player.moveSpeed += 0.02f;
            player.GetModPlayer<WikiArmorPlayer>().NetheriteBoots = true;
        }
    }

    // ------------------------------------------------------------------ Elderstone

    /// <summary>
    /// Wiki : Elderstone Armor.txt — ensemble multiclasse en pierre : Headplate 60 défense, +60 % de dégâts, -40 % de vitesse ;
    /// Chestplate 80, +20 %, -40 %, immunité au recul ; Legplate 60, +20 %, +30 % de vitesse. Ensemble : un clone de l'œil de
    /// Xeroc combat pour le joueur. [choix] L'œil (sans emplacement de sbire) tire un rayon (1000 dégâts d'invocation) toutes
    /// les 40 images sur l'ennemi le plus proche.
    /// </summary>
    public abstract class ElderstonePiece : StandInArmor
    {
        protected override int Rarity => ModContent.RarityType<BurnishedAuric>();
        protected override int SellCopper => 1000;
    }

    public class ElderstoneHeadplate : ElderstonePiece
    {
        protected override EquipType Slot => EquipType.Head;
        protected override string Worn => "MarniteArchitect/MarniteArchitectHeadgear";
        protected override int Defense => 60;

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.6f;
            player.moveSpeed -= 0.4f;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
            => body.type == ModContent.ItemType<ElderstoneChestplate>() && legs.type == ModContent.ItemType<ElderstoneLegplate>();

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = Language.GetTextValue("Mods.CalamityFandom.WikiArmor.Elderstone");
            player.GetModPlayer<WikiArmorPlayer>().ElderstoneSet = true;
        }
    }

    public class ElderstoneChestplate : ElderstonePiece
    {
        protected override EquipType Slot => EquipType.Body;
        protected override string Worn => "MarniteArchitect/MarniteArchitectToga";
        protected override int Defense => 80;

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.2f;
            player.moveSpeed -= 0.4f;
            player.noKnockback = true;
        }
    }

    public class ElderstoneLegplate : ElderstonePiece
    {
        protected override EquipType Slot => EquipType.Legs;
        protected override string Worn => "MarniteArchitect/MarniteArchitectToga";
        protected override int Defense => 60;

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.2f;
            player.moveSpeed += 0.3f;
        }
    }

    // ------------------------------------------------------------------ Frozen

    /// <summary>
    /// Wiki : Frozen Armor.txt — ensemble de mêlée : Visor 60 défense, +60 % de vitesse de mêlée, +25 PV max ; Chestplate 65,
    /// +20 % de taille en vraie mêlée, Frostburn sur les coups, les ennemis vous visent plus, +25 PV ; Greaves 75, immunité aux
    /// debuffs de froid, +25 PV. Ensemble : touche de bonus d'armure (celle de Calamity) : bouclier de glace qui pare ; parer
    /// un ennemi : 100 dégâts de mêlée et 1 s d'invulnérabilité ; parer un projectile : dégâts divisés par deux ; ne rien
    /// parer : le joueur gèle 1 s et le bouclier attend 10 s. Recharge de 5 s.
    /// [choix] Fenêtre de parade de 20 images ; parer un ennemi annule le coup ; « froid » = Frostburn, Frostbite, Chilled,
    /// Frozen, Glacial State ; « vous visent plus » = +400 d'aggro.
    /// </summary>
    public abstract class FrozenPiece : StandInArmor
    {
        protected override int Rarity => ModContent.RarityType<CosmicPurple>();
        protected override int SellCopper => 600000;

        public override void UpdateEquip(Player player) => player.statLifeMax2 += 25;
    }

    public class FrozenVisor : FrozenPiece
    {
        protected override EquipType Slot => EquipType.Head;
        protected override string Worn => "Daedalus/DaedalusHeadMelee";
        protected override int Defense => 60;

        public override void UpdateEquip(Player player)
        {
            base.UpdateEquip(player);
            player.GetAttackSpeed(DamageClass.Melee) += 0.6f;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
            => body.type == ModContent.ItemType<FrozenChestplate>() && legs.type == ModContent.ItemType<FrozenGreaves>();

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = Language.GetTextValue("Mods.CalamityFandom.WikiArmor.Frozen");
            player.GetModPlayer<WikiArmorPlayer>().FrozenSet = true;
        }

        public override void ArmorSetShadows(Player player)
        {
            if (Main.rand.NextBool(6))
                Dust.NewDustDirect(player.position, player.width, player.height, DustID.Snow, 0f, 1f, 100, default, 1f);
        }
    }

    public class FrozenChestplate : FrozenPiece
    {
        protected override EquipType Slot => EquipType.Body;
        protected override string Worn => "Daedalus/DaedalusBreastplate";
        protected override int Defense => 65;

        public override void UpdateEquip(Player player)
        {
            base.UpdateEquip(player);
            player.aggro += 400;
            player.GetModPlayer<WikiArmorPlayer>().FrozenChest = true;
        }
    }

    public class FrozenGreaves : FrozenPiece
    {
        protected override EquipType Slot => EquipType.Legs;
        protected override string Worn => "Daedalus/DaedalusLeggings";
        protected override int Defense => 75;

        public override void UpdateEquip(Player player)
        {
            base.UpdateEquip(player);
            player.buffImmune[BuffID.Frostburn] = true;
            player.buffImmune[BuffID.Frostburn2] = true;
            player.buffImmune[BuffID.Chilled] = true;
            player.buffImmune[BuffID.Frozen] = true;
            player.buffImmune[ModContent.BuffType<Legacy.Buffs.StatDebuffs.GlacialState>()] = true;
        }
    }

    // ------------------------------------------------------------------ Vanité

    /// <summary>Wiki : Archangel set.txt — tenue de vanité (Helm, Chestplate, Greaves), « No value ».</summary>
    public abstract class ArchangelPiece : StandInArmor
    {
        protected override int Defense => 0;
        protected override int Rarity => ItemRarityID.Blue;
        protected override int SellCopper => 0;
        protected override bool Vanity => true;
    }

    public class ArchangelHelm : ArchangelPiece
    {
        protected override EquipType Slot => EquipType.Head;
        protected override string Worn => "Aerospec/AerospecHeadMelee";
    }

    public class ArchangelChestplate : ArchangelPiece
    {
        protected override EquipType Slot => EquipType.Body;
        protected override string Worn => "Aerospec/AerospecBreastplate";
    }

    public class ArchangelGreaves : ArchangelPiece
    {
        protected override EquipType Slot => EquipType.Legs;
        protected override string Worn => "Aerospec/AerospecLeggings";
    }

    /// <summary>
    /// Wiki : Catarachedrae Vanity Outfit.txt — tenue de vanité (Headset 1 or, Dress 1 or, Boots 9 or).
    /// [technique] « Fabricable seulement en Revengeance » : les recettes du wiki n'ont pas de condition, non fait.
    /// </summary>
    public abstract class CatarachedraePiece : StandInArmor
    {
        protected override int Defense => 0;
        protected override int Rarity => ItemRarityID.Green;
        protected override bool Vanity => true;
    }

    public class CatarachedraeHeadset : CatarachedraePiece
    {
        protected override EquipType Slot => EquipType.Head;
        protected override string Worn => "Bloodflare/BloodflareHeadMelee";
        protected override int SellCopper => 10000;
    }

    public class CatarachedraeDress : CatarachedraePiece
    {
        protected override EquipType Slot => EquipType.Body;
        protected override string Worn => "Bloodflare/BloodflareBodyArmor";
        protected override int SellCopper => 10000;
    }

    public class CatarachedraeBoots : CatarachedraePiece
    {
        protected override EquipType Slot => EquipType.Legs;
        protected override string Worn => "Bloodflare/BloodflareCuisses";
        protected override int SellCopper => 90000;
    }

    // ------------------------------------------------------------------ effets

    public class WikiArmorPlayer : ModPlayer
    {
        public bool NetheriteBoots, NetheriteSet, ElderstoneSet, FrozenSet, FrozenChest;
        public int ParryWindow, ParryCooldown;

        public override void ResetEffects() => NetheriteBoots = NetheriteSet = ElderstoneSet = FrozenSet = FrozenChest = false;

        public override void PostUpdate()
        {
            if (ParryCooldown > 0)
                ParryCooldown--;
            if (ParryWindow > 0 && --ParryWindow == 0)
            {
                // Rien de paré : le joueur gèle 1 s et le bouclier attend 10 s
                Player.AddBuff(BuffID.Frozen, 60);
                ParryCooldown = 600;
            }
        }

        public override void PostUpdateEquips()
        {
            if (ElderstoneSet && Player.whoAmI == Main.myPlayer && Player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.ArmorSets1119.XerocEyeClone>()] < 1)
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.ArmorSets1119.XerocEyeClone>(),
                    (int)Player.GetTotalDamage(DamageClass.Summon).ApplyTo(1000), 0f, Player.whoAmI);
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (!FrozenSet || ParryCooldown > 0 || ParryWindow > 0 || CalamityMod.CalamityKeybinds.ArmorSetBonusHotKey?.JustPressed != true)
                return;
            ParryWindow = 20;
            SoundEngine.PlaySound(SoundID.Item30, Player.Center);
        }

        private bool TryParry()
        {
            if (ParryWindow <= 0)
                return false;
            ParryWindow = 0;
            ParryCooldown = 300;
            if (!Main.dedServ)
                for (int i = 0; i < 20; i++)
                    Dust.NewDustPerfect(Player.Center, DustID.IceTorch, Main.rand.NextVector2CircularEdge(5f, 5f), 100, default, 1.5f).noGravity = true;
            SoundEngine.PlaySound(SoundID.Item27, Player.Center);
            return true;
        }

        public override bool FreeDodge(Player.HurtInfo info)
        {
            // Parer un ennemi : coup annulé, 100 dégâts de mêlée, 1 s d'invulnérabilité
            if (info.DamageSource.SourceNPCIndex < 0 || !TryParry())
                return false;
            NPC npc = Main.npc[info.DamageSource.SourceNPCIndex];
            Player.ApplyDamageToNPC(npc, (int)Player.GetTotalDamage(DamageClass.Melee).ApplyTo(100), 6f, npc.Center.X > Player.Center.X ? 1 : -1, false, DamageClass.Melee);
            Player.SetImmuneTimeForAllTypes(60);
            return true;
        }

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            if (TryParry())
                modifiers.FinalDamage *= 0.5f;
        }

        public override void ModifyItemScale(Item item, ref float scale)
        {
            if (FrozenChest && item.CountsAsClass(DamageClass.Melee) && !item.noMelee)
                scale *= 1.2f;
        }

        private void OnAnyHit(NPC target, bool trueMelee)
        {
            if (NetheriteSet)
                target.AddBuff(ModContent.BuffType<Buffs.Wither>(), 600);
            if (FrozenChest && trueMelee)
                target.AddBuff(BuffID.Frostburn, 180);
        }

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone) => OnAnyHit(target, item.CountsAsClass(DamageClass.Melee));

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone) => OnAnyHit(target, proj.CountsAsClass<TrueMeleeDamageClass>());
    }
}

namespace CalamityFandom.Content.Projectiles.ArmorSets1119
{
    /// <summary>
    /// Clone de l'œil de Xeroc (Elderstone) : flotte au-dessus du joueur, tire un rayon toutes les 40 images.
    /// [technique] Le wiki n'a qu'une illustration de l'œil : texture d'un Demon Eye de Terraria teinté (docs/SPRITES.md).
    /// </summary>
    public class XerocEyeClone : ModProjectile
    {
        public override string Texture => "Terraria/Images/NPC_" + NPCID.DemonEye;

        public override void SetStaticDefaults() => Main.projFrames[Type] = 2;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.friendly = false;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 2;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.dead || !owner.GetModPlayer<Items.Armor.WikiArmorPlayer>().ElderstoneSet)
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 2;
            Vector2 home = owner.Center + new Vector2(0f, -80f);
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, (home - Projectile.Center) * 0.1f, 0.2f);
            NPC target = Dart.Closest(Projectile.Center, 900f);
            Projectile.rotation = target != null ? (target.Center - Projectile.Center).ToRotation() + MathHelper.Pi : 0f;
            if (++Projectile.frameCounter % 8 == 0)
                Projectile.frame = (Projectile.frame + 1) % 2;
            if (target != null && Projectile.owner == Main.myPlayer && ++Projectile.ai[0] % 40 == 0)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitX) * 20f,
                    ModContent.ProjectileType<XerocEyeBeam>(), Projectile.damage, 2f, Projectile.owner);
        }

        public override Color? GetAlpha(Color lightColor) => new Color(200, 160, 255, 220);
    }

    public class XerocEyeBeam : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults() => ProjectileID.Sets.MinionShot[Type] = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 60;
            Projectile.extraUpdates = 2;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (!Main.dedServ)
                Dust.NewDustPerfect(Projectile.Center, DustID.PurpleTorch, Vector2.Zero, 100, default, 1.3f).noGravity = true;
        }
    }
}
