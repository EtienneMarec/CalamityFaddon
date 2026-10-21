# Sprites à faire ou à revoir

Quand un contenu n'a pas de sprite sur le wiki, ou que l'image est inutilisable (photo, mauvais format), on le note
ici et l'utilisateur essaie de le faire. **Déposer le PNG dans `wiki_sprites_custom/` sous le titre exact de la page**
(ex. `wiki_sprites_custom/Mars Bars.png`), puis cocher : les outils le prennent en priorité sur l'image du wiki.
Format Terraria : pixel art, fond transparent ; animation = images empilées verticalement.

## Urgent : utilisés en jeu avec un sprite provisoire

- [ ] **Mars Bars** (matériau) — le wiki n'a qu'une photo de 360×280 ; réduite automatiquement en 90×70 en attendant.
      Attendu : sprite de matériau ~24×24 à 32×32. Fichier : `wiki_sprites_custom/Mars Bars.png`

## Optionnel : améliorations

- [x] **Yharim Bar au sol** (fait le 22/10 : détourage de l'image du wiki, réduit en 480×384 par `gen_items.py`) — dessiné aujourd'hui en agrandissant x16 le sprite de 30×24 ; l'image « The item in-game »
      du wiki (`Yharimbaringame.png`) montre une version géante plus détaillée, mais floue et sur un fond de décor,
      donc inutilisable telle quelle. Attendu : version détaillée ~120×96 (dessinée x4 en jeu) ou 480×384, fond
      transparent. Fichier : `wiki_sprites_custom/Yharim Bar (world).png` (le code à brancher est simple : texture
      séparée pour le dessin au sol)

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
