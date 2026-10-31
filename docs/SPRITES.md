# Sprites à faire ou à revoir

Quand un contenu n'a pas de sprite sur le wiki, ou que l'image est inutilisable (photo, mauvais format), on le note
ici et l'utilisateur essaie de le faire. **Déposer le PNG dans `wiki_sprites_custom/` sous le titre exact de la page**
(ex. `wiki_sprites_custom/Mars Bars.png`), puis cocher : les outils le prennent en priorité sur l'image du wiki.
Format Terraria : pixel art, fond transparent ; animation = images empilées verticalement.

## Urgent : utilisés en jeu avec un sprite provisoire

- [ ] **Mars Bars** (matériau) — le wiki n'a qu'une photo de 360×280 ; réduite automatiquement en 90×70 en attendant.
      Attendu : sprite de matériau ~24×24 à 32×32. Fichier : `wiki_sprites_custom/Mars Bars.png`

- [ ] **Holographic Chessboard** (outil du Taming du Brain of Cthulhu et de XP-00 Hypnos) — sprite provisoire 32×32 dessiné
      par Claude (`Content/Items/Taming/HolographicChessboard.png`). Attendu : objet ~32×32, échiquier holographique.
      Fichier : `wiki_sprites_custom/Holographic Chessboard.png`
- [ ] **Pièces d'échecs du Holographic Chessboard** — dessinées pour l'instant comme des lettres (K Q R B N P). Attendu :
      une planche de 6 colonnes (pion, cavalier, fou, tour, dame, roi) sur 2 lignes (blanc, noir), cases de 32×32,
      style hologramme. Fichier : `wiki_sprites_custom/Holographic Chessboard (pieces).png` (le code est à brancher)

## Récompenses du Taming (pas de page ni d'image sur le wiki)

Même objet dans les deux modes (sans effet en Full Fandom). Effets proposés : `docs/decisions.md`, section Taming.
Fichiers : `wiki_sprites_custom/<Nom>.png`.

- [ ] **Bloated Gel** (King Slime, accessoire)
- [ ] **Hydrated Pendant** (Desert Scourge, accessoire)
- [ ] **Dizzy Lens** (Eye of Cthulhu, accessoire)
- [ ] **Blessed Fungus** (Crabulon, accessoire)
- [ ] **Worm Platter** (Eater of Worlds, nourriture)
- [ ] **En Passant** (Brain of Cthulhu, accessoire)
- [ ] **Blighted Tome** (The Hive Mind, livre de magie)
- [ ] **Ball of Worms** (The Perforators, objet qui invoque un familier) — le familier peut reprendre l'Estranged Son
- [ ] **Motherbrain** (Queen Bee, accessoire)
- [ ] **Skeletron's Fist** (Skeletron, gant de mêlée)
- [ ] **Fate** (Wall of Flesh, accessoire)
- [ ] **Melting Ice Cube** (Cryogen, accessoire)
- [ ] **Frozen Heater** (Pyrogen, accessoire ; boss inventé, plus tard)
- [ ] **Shiny Lipstick** (The Destroyer, accessoire)
- [ ] **Worship 101** (Lunatic Cultist, livre)
- [ ] **Wulfrum Hardhat** et **Wulfrum Lenses** (casques, avec leurs boss inventés ; pas d'image sur le wiki non plus)

- [ ] **The Food Chain (fouet en action)** — planche provisoire dessinée par Claude
      (`Content/Projectiles/Summon/TheFoodChainProj.png`, 10 × 92 : manche 26 px, 3 maillons de 16 px, pointe 18 px,
      format de l'ExampleWhip). Attendu : même format, chaîne d'anneaux de calamar comme l'icône du wiki.
      Fichier : `wiki_sprites_custom/The Food Chain (whip).png` (le code est à brancher)

## Optionnel : améliorations

- [x] **Yharim Bar au sol** (fait le 22/10 : détourage de l'image du wiki, réduit en 480×384 par `gen_items.py`) — dessiné aujourd'hui en agrandissant x16 le sprite de 30×24 ; l'image « The item in-game »
      du wiki (`Yharimbaringame.png`) montre une version géante plus détaillée, mais floue et sur un fond de décor,
      donc inutilisable telle quelle. Attendu : version détaillée ~120×96 (dessinée x4 en jeu) ou 480×384, fond
      transparent. Fichier : `wiki_sprites_custom/Yharim Bar (world).png` (le code à brancher est simple : texture
      séparée pour le dessin au sol)

## Ailes du wiki (images portées)

- [ ] **Bellwings** : pas d'image « (equipped) » sur le wiki : invisibles une fois portées. Attendu : planche verticale de 4
      images (battement), comme les ailes de Terraria. Fichier : `wiki_sprites_custom/Bellwings (equipped).png`
- [ ] Optionnel : les 10 autres ailes n'ont qu'une pose sur le wiki (pas de battement) ; une planche de 4 images par paire
      les animerait (`wiki_sprites_custom/<Nom> (equipped).png`, le code est à brancher)

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
