# Sprites à faire ou à revoir

Quand un contenu n'a pas de sprite sur le wiki, ou que l'image est inutilisable (photo, mauvais format), on le note
ici et l'utilisateur essaie de le faire. **Déposer le PNG dans `wiki_sprites_custom/` sous le titre exact de la page**
(ex. `wiki_sprites_custom/Mars Bars.png`), puis cocher : les outils le prennent en priorité sur l'image du wiki.
Format Terraria : pixel art en blocs de 2×2 pixels, fond transparent ; animation = images empilées verticalement.

Trois listes : **1. inexistants** (rien sur le wiki : provisoire de Claude ou rien du tout), **2. repris ailleurs**
(on affiche la texture d'autre chose), **3. améliorables** (le sprite du wiki est utilisé mais il manque quelque chose).
Mise à jour : 31/10.

## 1. Inexistants

### En jeu avec un provisoire (priorité)

- [ ] **Holographic Chessboard** (Taming du Brain of Cthulhu et de XP-00 Hypnos) — provisoire 32×32 de Claude
      (`Content/Items/Taming/HolographicChessboard.png`). Attendu : objet ~32×32, échiquier holographique.
      Fichier : `wiki_sprites_custom/Holographic Chessboard.png`
- [ ] **Pièces d'échecs du Holographic Chessboard** — dessinées comme des lettres (K Q R B N P). Attendu : planche de 6
      colonnes (pion, cavalier, fou, tour, dame, roi) sur 2 lignes (blanc, noir), cases de 32×32, style hologramme.
      Fichier : `wiki_sprites_custom/Holographic Chessboard (pieces).png` (le code est à brancher)
- [ ] **The Food Chain (fouet en action)** — planche provisoire de Claude (`Content/Projectiles/Summon/TheFoodChainProj.png`,
      10 × 92 : manche 26 px, 3 maillons de 16 px, pointe 18 px, format de l'ExampleWhip). Attendu : même format,
      chaîne d'anneaux de calamar comme l'icône du wiki. Fichier : `wiki_sprites_custom/The Food Chain (whip).png`
- [ ] **Mars Bars** (matériau) — le wiki n'a qu'une photo de 360×280, réduite automatiquement en 90×70. Attendu :
      sprite de matériau ~24×24 à 32×32. Fichier : `wiki_sprites_custom/Mars Bars.png`
- [ ] **Bellwings (portées)** — pas d'image « (equipped) » sur le wiki : invisibles une fois portées. Attendu : planche
      verticale de 4 images (battement), comme les ailes de Terraria. Fichier : `wiki_sprites_custom/Bellwings (equipped).png`

### Projectiles sans image sur le wiki (en poussière pour l'instant)

Rendus avec des particules : jouables tels quels, mais un sprite serait plus lisible.

- [ ] **Boule du War Staff** (« Gray Energy », poussière grise) — `wiki_sprites_custom/War Staff (projectile).png`, ~16×16
- [ ] **Orbes de la Frost Blade et de la Flare Blade** (poussière bleue / orange) — ~14×14 chacune
- [ ] **Éclairs de la famille Catastrophe** (Claymore, Clayscore, Clayspore, Claycore : rouge, bleu, jaune) — comme le vieux
      code de Calamity, en poussière ; ~20×20 par couleur
- [ ] **Décharges galactiques de la Devastation** (3 couleurs, poussière)

### Récompenses du Taming (pas de page ni d'image sur le wiki)

Même objet dans les deux modes (sans effet en Full Fandom). Propositions de noms et d'effets : `docs/decisions.md`,
section Taming. Fichiers : `wiki_sprites_custom/<Nom>.png`.

- [ ] Les 15 nommées par la page : **Bloated Gel** (King Slime), **Hydrated Pendant** (Desert Scourge), **Dizzy Lens**
      (Eye of Cthulhu), **Blessed Fungus** (Crabulon), **Worm Platter** (Eater of Worlds, nourriture), **En Passant**
      (Brain of Cthulhu), **Blighted Tome** (Hive Mind), **Ball of Worms** (Perforators ; le familier peut reprendre
      l'Estranged Son), **Motherbrain** (Queen Bee), **Skeletron's Fist** (Skeletron), **Fate** (Wall of Flesh),
      **Melting Ice Cube** (Cryogen), **Frozen Heater** (Pyrogen, boss inventé), **Shiny Lipstick** (Destroyer),
      **Worship 101** (Lunatic Cultist)
- [ ] Les ~33 boss dont la case est vide sur la page : objet à choisir d'abord (propositions dans `docs/decisions.md`)
- [ ] **Wulfrum Hardhat** et **Wulfrum Lenses** (casques ; ont une page mais pas d'image ; viendront avec leurs boss inventés)

### Contenu bloqué par un sprite

- [ ] **Gel Armor** (3 pièces) — icônes sur le wiki, mais pas d'image portée : il faut les planches Head / Body / Legs
      (format des armures Terraria). Fichiers : `wiki_sprites_custom/Gel Armor (head|body|legs).png`

## 2. Repris ailleurs (texture d'autre chose)

### Choisis par nous (pas de sprite sur le wiki)

| Contenu | Texture utilisée | Attendu |
|---|---|---|
| The Insacred Texts : portail | portail démoniaque de Calamity (`DemonPortal`), teinté rouge et aplati | grand portail rouge vu de côté, ~96×32 |
| The Insacred Texts : bras | main du Sepulcher de Supreme Calamitas, teintée et étirée | bras qui sort du portail, ~26×80 (étiré en jeu) |
| Elemental Claymore : Eon Beams | rayon de la Terra Blade vanilla, teinté en 4 couleurs | rayon d'épée en 4 couleurs (rose, vert, bleu, orange) |
| Elemental Claymore : orbes | Eon Bolts de Calamity | à garder (même effet sur la page) |
| The Princess : flammes | flammes du Prince de Calamity (`PrinceFlameLarge/Small`) | à garder ou à refaire en version « sainte » |
| Microxodonta : nuées | ennemi Microbial Cluster de Calamity | à garder (la page parle de Microbial Clusters) |
| Plague Balloon : abeilles | abeille vanilla | abeille de la peste (verte) ~10×10 |
| Blow Pipe : dards | dard vanilla | dard de peste |
| Microcell Cut : étoiles | Hallow Star vanilla | à garder (c'est l'image de la page) |
| Glacial Crusher : icebergs | Ice Bomb de Cryogen | iceberg ~30×30 |
| Profaned Energy Turret | ennemi Impious Immolator de Calamity | tourelle profanée |
| Holographic Chessboard : pièces | lettres | voir partie 1 |
| Outils de debug (mannequin, fiole, cadran, épée) | sprites vanilla | rien à faire |

### Repris du vieux code de Calamity (portages)

Ces armes retirées de Calamity utilisaient déjà la texture d'autre chose en 2022 ; à refaire seulement si on veut mieux
que l'original : pics de glace de la Bittercold Staff (`IceRain` de Cryogen), ondes de la Hadopelagic Echo (`Eidolic
Wail`), tirs de la Marked Magnum et du Charged Dart Blaster (`LaserProj`), gouttes du Flak Toxicannon (`AcidDrop`), lunes
de la Galileo Gladius (`CrescentMoonProj`), lames de Cosmic Shiv, Galileo Gladius et Spore Knife (icône de l'objet).

## 3. Améliorables

- [ ] **Blocs posés sans bords** (49 matériaux du wiki qui se posent : 44 blocs, 3 barres, minerais...,
      `Assets/WikiTiles/`) : texture = carré tiré de l'image « (placed) » du wiki, répété sans bords ni jonctions.
      Attendu : planche de tuile Terraria (288×270, format des blocs vanilla) par bloc. Fichiers :
      `wiki_sprites_custom/<Nom> (tile).png`. Liste : Abandoned Bronze Plating, Aeroplate, Angelinite Ore, Apocalyptic
      Ore, Arean Conduit Plating, Arsenic Ore, Ashplate, Bloodplate, Bloodstone Ore, Catastrophe Clay Ore, Chaotic Brick,
      Compressed Mana, Cosmilite Slag, Crystalplate, Curseplate, Debris Ore, Dustplate, Elemental Ore, Endoplate, Esseric
      Ore, Flahium Bar, Flahium Slag, Frozen Hellstone, Fuelplate, Haunted Ore, Ichorplate, Lawplate, Life Ore, Meld Ore,
      Mystplate, Natrisodic Cluster, Omnicidal Sludge Bar, Proviplate, Sallowrack, Shadowspec Ore, Shatterite Ore, Silicon
      Ore, Skylac Sand, Skylac Stone, Smooth Skylac Stone, Stiriatic Ore, Stratostone, Stratus Star Brick, Tenebris,
      Thorium Ore, Verdantium Bar, Verdantium Ore, Victide Ore, Zotplate
- [ ] **Icônes de murs** (14 : Abyss Gravel Wall, Aerialite Brick Wall, Astral Stone Wall, Cosmilite Brick Wall, Eutrophic
      Sand Wall, Navystone Wall, Smooth Abyss Gravel Wall, Smooth Navystone Wall, Smooth Voidstone Wall, Statigel Wall,
      Sulphurous Sand Wall, Unsafe Eutrophic Sand Wall, Unsafe Navystone Wall, Voidstone Wall) — le wiki montre le mur
      posé, pas l'objet : ils gardent l'icône de Calamity. Attendu : icône 32×32.
- [ ] **Illustrations trop grandes** (4 : Adamantite Particle Accelerator 142×85, Cosmic Immaterializer 108×108, Dragon's
      Breath 149×72, The Maelstrom 109×153) — gardent le sprite de Calamity ; une version à taille d'objet ferait le lien.
- [ ] **Animation coupée** (27 objets animés chez Calamity dont le wiki montre un autre dessin fixe : Abyss Bar, Astral
      Bar, Blood Orb, Cosmilite Bar, Phantoplasm, Unholy Essence...) — liste dans `reports/sprites.md`. Une planche
      animée dans le style du wiki leur rendrait l'animation. Fichiers : `wiki_sprites_custom/<Nom>.png` (images empilées).
- [ ] **Aliments** (22 boissons et plats : planche de 3 images refaite automatiquement avec l'image du wiki, qui ne montre
      qu'une pose) — liste dans `reports/sprites.md` ; les poses « tenu » et « posé » sont des copies de l'icône.
- [ ] **Ailes du wiki** (10 paires) : une seule pose sur le wiki (pas de battement) ; une planche de 4 images par paire
      les animerait (`wiki_sprites_custom/<Nom> (equipped).png`, le code est à brancher)
- [x] **Yharim Bar au sol** (fait le 22/10 : détourage de l'image du wiki, réduit en 480×384 par `gen_items.py`)

## Images du wiki impossibles à récupérer

- [ ] `AAMG "Disperse"`, `HPST "Malign"`, `OPFT "Sublimate"` (refusées par le CDN de Fandom) et `Moai.jpg` (sans fichier).
      Pas encore utilisées : à faire si on implémente les pages qui les montrent.

## Contenu du wiki sans sprite (82 pages, à faire quand on l'implémentera)

Certaines sont des pages d'objets ou debuffs vanilla (On Fire, Poisoned...) qui ont déjà leur sprite en jeu : rien à faire pour celles-là.

- **Debuff** (15) : Bleeding, Chaos State, Confused, Cursed Inferno, Darkness, Deadlock, Electrified, Frostburn, Ichor (debuff), Oiled, On Fire, Poisoned, Slow, Venom, Webbed
- **Boss** (14) : Artificial Sentience System, AX-P03, Birdest Up, Kulu-Yharon-Ku, Cyrogen, Dihidrogen Monoxide, Fatal Circuit, Goozmageldon, Horseslimes of the Goopocalypse, Mechanical Horde, Necron, Pandora, the Hypnotized Witch, The Scrapyard Ensemble, The Wall of Gifts, Void Hydra, XB-10 HuY
- **?** (7) : Charge, Mud Cannon, Sanity, Spy, Stamina Meter, Stress, Vanilla changes
- **Weapon** (7) : AAMG "Disperse", Bloodinator 1, Deck of Souls, HPST "Malign", OPFT "Sublimate", UserGrahkon/Sandbox, Vehicle-Borne Improvised Explosive Device
- **Block** (6) : Arena Block, Bars, Compressed Stone Blocks, Invincible Titanic Snow, Ores, Pew
- **Furniture** (6) : Banners, Effigies, Portative Forging Station, Pots, Tables (CTotA), Work Benches (CTotA)
- **NPC** (5) : Soul of the Eternal Intellect of Infinite Verboten Knowledge, The Observer, Town NPCs, Town Slimes, Youkai Miko
- **Covering** (4) : Deteriorating Lens, Level Meters, Music Boxes, Wings
- **Tool** (3) : Fishing Poles, Protective Shield, Sigil of the Nightmare
- **Enemy** (2) : Crazed Mechanic, Puff enemies
- **Potion** (2) : Fluorescent Vodka, Sacred Meat
- **Superboss** (2) : Nihil, the Fathomless Seeker, Poobis Prime
- **Wall** (2) : Dungeon Wall Unsafe, Mossy Gravel Wall
- **Aquatic** (1) : Sealed One
- **Armor** (1) : Horseslime armor
- **Challenge** (1) : Future bosses
- **Grab bag** (1) : Crates
- **Miscellaneous** (1) : Draedon's Logs
- **Undead Enemy** (1) : Irate Bones
- **basic Enemy** (1) : The Melter
