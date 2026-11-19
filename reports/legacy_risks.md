# Portages de l'ancien Calamity : lesquels tester en détail

Généré par `tools/legacy_risks.py`, ne pas éditer. Un portage est **à risque** quand il réutilise une classe de Calamity 2.2.2 (projectile, buff, PNJ, poussière, classe de base) qui a changé de plus de 8 lignes significatives depuis la version d'où il vient, ou qui n'existait pas alors. Les autres n'ont besoin que du test rapide (`docs/TESTS.md`).

Limite : sans commit dans l'en-tête du fichier, la version d'origine est 2.0.1.005 ; un portage plus ancien peut donc avoir des changements de 2022 non vus.

## À risque (36)

- [ ] **Ancient Shiv** (`Content/Legacy/Items/Weapons/Melee/LegacyShortswords.cs`, depuis `3ec8f553ed`) : `BaseShortswordProjectile` (10 lignes changées)
- [ ] **Aquatic Discharge** (`Content/Legacy/Items/Weapons/Melee/AquaticDischarge.cs`, depuis `3ec8f553ed`) : `BaseShortswordProjectile` (10 lignes changées)
- [ ] **Bloody Rupture** (`Content/Legacy/Items/Weapons/Melee/LegacyShortswords.cs`, depuis `3ec8f553ed`) : `BaseShortswordProjectile` (10 lignes changées)
- [ ] **Bouncing Betty** (`Content/Legacy/Items/LegacyBatch1029h.cs`, depuis `3ec8f553ed`) : `TotalityFire` (25 lignes changées)
- [ ] **Brimlash Buster** (`Content/Legacy/Items/LegacyBatch1029e.cs`, depuis `3ec8f553ed`) : `Brimlash2` (42 lignes changées), `BrimlashProj` (56 lignes changées)
- [ ] **Caliburn** (`Content/Legacy/Items/Weapons/Melee/LegacyShortswordBeams.cs`, depuis `3ec8f553ed`) : `BaseShortswordProjectile` (10 lignes changées)
- [ ] **Charged Dart Blaster** (`Content/Legacy/Items/LegacyReplaced1031.cs`, depuis `3ec8f553ed`) : `ChargedBlast` (76 lignes changées)
- [ ] **Cosmic Shiv** (`Content/Legacy/Projectiles/Melee/CosmicShivLegacy.cs`, depuis `3ec8f553ed`) : `BaseShortswordProjectile` (10 lignes changées)
- [ ] **Diseased Pike** (`Content/Legacy/Items/Weapons/Melee/DiseasedPike.cs`, depuis `3ec8f553ed`) : `BaseSpearProjectile` (40 lignes changées), `PlagueSeeker` (25 lignes changées)
- [ ] **Divine Retribution** (`Content/Legacy/Items/LegacyWeapons1113c.cs`, depuis `3ec8f553ed`) : `TeslaAura` (58 lignes changées)
- [ ] **Energy Staff** (`Content/Legacy/Items/Weapons/Summon/EnergyStaff.cs`, depuis `3ec8f553ed`) : `FlameBlast` (66 lignes changées), `FlameBurst` (48 lignes changées)
- [ ] **Eutrophic Shank** (`Content/Legacy/Items/Weapons/Melee/LegacyShortswords.cs`, depuis `3ec8f553ed`) : `BaseShortswordProjectile` (10 lignes changées)
- [ ] **Fulguration Halberd** (`Content/Legacy/Items/LegacyBatch1029f.cs`, depuis `3ec8f553ed`) : `BaseSpearProjectile` (40 lignes changées)
- [ ] **Galileo Gladius** (`Content/Legacy/Projectiles/Melee/GalileoGladiusLegacy.cs`, depuis `3ec8f553ed`) : `BaseSwordHoldoutProjectile` (n'existait pas dans la version d'origine)
- [ ] **Gauss Dagger** (`Content/Legacy/Items/LegacyWeapons1102b.cs`, depuis `3ec8f553ed`) : `GaussFlux` (9 lignes changées)
- [ ] **Impaler** (`Content/Legacy/Items/LegacyBatch1029g.cs`, depuis `3ec8f553ed`) : `FuckYou` (30 lignes changées)
- [ ] **Latcher Mine** (`Content/Legacy/Items/LegacyBatch1029i.cs`, depuis `3ec8f553ed`) : `BarrelShrapnel` (13 lignes changées), `TotalityFire` (25 lignes changées)
- [ ] **Lazhar** (`Content/Legacy/Items/Weapons/Magic/Lazhar.cs`, depuis `3ec8f553ed`) : `FuckYou` (30 lignes changées)
- [ ] **Leeching Dagger** (`Content/Legacy/Items/Weapons/Melee/LegacyShortswords.cs`, depuis `3ec8f553ed`) : `BaseShortswordProjectile` (10 lignes changées)
- [ ] **Magna Striker** (`Content/Legacy/Items/LegacyWeapons1102b.cs`, depuis `3ec8f553ed`) : `OpalStrike` (41 lignes changées)
- [ ] **Marnite Spear** (`Content/Legacy/Items/LegacyBatch1029d.cs`, depuis `3ec8f553ed`) : `BaseSpearProjectile` (40 lignes changées)
- [ ] **Night's Stabber** (`Content/Legacy/Items/Weapons/Melee/LegacyShortswords.cs`, depuis `3ec8f553ed`) : `BaseShortswordProjectile` (10 lignes changées)
- [ ] **Phoenix Blade** (`Content/Legacy/Items/LegacyBatch1029e.cs`, depuis `3ec8f553ed`) : `FuckYou` (30 lignes changées)
- [ ] **Quasar** (`Content/Legacy/Items/LegacyBatch1029i.cs`, depuis `3ec8f553ed`) : `RadiantExplosion` (58 lignes changées)
- [ ] **Sandslasher** (`Content/Legacy/Items/Weapons/Rogue/Sandslasher.cs`, depuis `3ec8f553ed`) : `DuststormCloud` (56 lignes changées)
- [ ] **Seabound Staff** (`Content/Legacy/Items/Weapons/Summon/SeaboundStaff.cs`, depuis `3ec8f553ed`) : `BrittleStarMinion` (151 lignes changées)
- [ ] **Shock Grenade** (`Content/Legacy/Items/Weapons/Rogue/ShockGrenade.cs`, depuis `3ec8f553ed`) : `ShockTeslaAura` (26 lignes changées)
- [ ] **Sky Stabber** (`Content/Legacy/Items/LegacyWeapons1102.cs`, depuis `3ec8f553ed`) : `StickyFeatherAero` (34 lignes changées)
- [ ] **Solar Flare** (`Content/Legacy/Items/Weapons/Melee/SolarFlare.cs`, depuis `3ec8f553ed`) : `HolyExplosionSupreme` (41 lignes changées)
- [ ] **Soul Edge** (`Content/Legacy/Items/Weapons/Melee/SoulEdge.cs`, depuis `3ec8f553ed`) : `GhastlySoulLarge` (104 lignes changées), `GhastlySoulMedium` (104 lignes changées), `GhastlySoulSmall` (103 lignes changées)
- [ ] **Spore Knife** (`Content/Legacy/Projectiles/Melee/SporeKnifeShortsword.cs`, depuis `3ec8f553ed`) : `BaseShortswordProjectile` (10 lignes changées)
- [ ] **Star Cannon EX** (`Content/Legacy/Items/LegacyBatch1029g.cs`, depuis `3ec8f553ed`) : `AstralStar` (91 lignes changées)
- [ ] **Starfall** (`Content/Legacy/Items/Weapons/Magic/Starfall.cs`, depuis `3ec8f553ed`) : `AstralStarMagic` (122 lignes changées)
- [ ] **Tomatus** (`Content/Legacy/Items/Weapons/Melee/LegacyShortswords.cs`, depuis `3ec8f553ed`) : `BaseShortswordProjectile` (10 lignes changées)
- [ ] **Urchin Spear** (`Content/Legacy/Items/LegacyOceanWeapons.cs`, depuis `3ec8f553ed`) : `BaseSpearProjectile` (40 lignes changées), `UrchinSpikeFugu` (73 lignes changées)
- [ ] **Wulfrum Staff** (`Content/Legacy/Items/LegacyReplaced1031.cs`, depuis `3ec8f553ed`) : `WulfrumBolt` (29 lignes changées)

## Test rapide seulement (135)

Acceleration Round, Acidic Rain Barrel, Aftershock, Alchemical Flask, Alpha Ray, Alpha Virus, Amidias' Spark, Animus, Aquatic Dissolution, Arctic Arrow, Armored Digger, Asteroid Staff, Astral Blade, Astral Bow, Bittercold Staff, Blade of Enmity, Bleached Angling Kit, Blood Clot Staff, Blood Relic, Bloody Edge, Bouncy Spiky Ball, Brimstone Flameblaster, Cadance Potion, Calamitas' Brew, Carnage Ray, Catastrophe Claymore, Clamor Noctus, Coral Cannon, Corroslime Staff, Corrupted Crusher Blade, Cosmic Elemental, Crimslime Staff, Crimson Crusher Blade, Crumbling Potion, Crystal Blade, Crystal Flare Staff, Cursed Capper, Death, Devastation, Disseminator, Dodu's Handcannon, Draedon's Remote, Dualpoon, Effluvium Bow, Elemental Excalibur, Enhanced Nano Round, Eutrophic Scimitar, Fish of Light, Fish of Night, Frigid Bulwark, Fungal Carapace, Gatling Laser, Gauss Pistol, Gehenna, Glacial Crusher, Glacial State, Golden Gun, Hadopelagic Echo, Hell Burst, Hell's Sun, Holy Wrath Potion, Lazinator, Lead Core, Lightning Hawk, Luminous Striker, Magnum, Magnum Rounds, Mana Jelly, Mandible Bow, Mandible Claws, Mariana, Marked Magnum, Marnite Bayonet, Molecular Manipulator, Monkey Darts, Night's Gaze, Nychthemeron, Old Power Cell, Penumbra Potion, Phantom Lance, Plague Hive, Plague Keeper, Poison Pack, Profaned Rage Potion, Psychotic Amulet, Purified Jam, Raider's Glory, Red Bow, Redtide Sword, Regenator, Revengeance, Revivify Potion, Rusty Medallion, Sandy Angling Kit, Sea Shell, Sea Urchin, Seabow, Seashell Boomerang, Shadowdrop Staff, Shattering Potion, Shroomite Visage, Spectre Rifle, Sticky Spiky Ball, Storm Dragoon, Storm Spray, Superball Bullet, Sylvan Slasher, Temporal Floe Sword, Terra Edge, Terra Shiv, Terra-cotta, Tesla Potion, The Enforcer, The Reaper, Titan Scale Potion, Trinity, Triploon, Triumph Potion, True Bloody Edge, True Caliburn, True Night's Stabber, Urchin Flail, Verdant, War Cleave, Waraxe, Warbanner of the Sun, Warblade, Wrath of the Ancients, Wulfrum armor, Wulfrum Axe, Wulfrum Blade, Wulfrum Bow, Wulfrum Hammer, Yanmei's Knife, Yharim's Stimulants
