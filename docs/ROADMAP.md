# Feuille de route

Ordre pensé pour avoir vite un résultat jouable et pour que chaque phase serve aux suivantes.
Chiffres : `reports/README.md`, `reports/recipes.md`, `reports/loot_applied.md` (à régénérer).

## Phase 0 — Fondations techniques (fait)

- [x] Squelette du mod, référence à `CalamityMod.dll`, build propre (référence exclue du `.tmod`)
- [x] Indexeur du wiki : statut par page, renommages, écarts de stats, suivi d'avancement, vrais titres via l'API
- [x] Dump complété (`fandom_dump_extra/` : pages écrasées par Windows, modèles, guides...)
- [x] Conversion des sprites WebP → PNG
- [x] Auto-test en serveur sans interface (`tools/selftest.py`) + checklist en jeu (`docs/TESTS.md`)
- [x] Démo : Zweihandromeda (épée tenue à coups alternés)

## Phase 1 — « Ça ressemble au wiki » (fait, nuit du 21/10)

- [x] Renommages et vocabulaire (Graydron, Tarragon, Spy, Coverings...) — `tools/glossary.json`
- [x] Stats des infobox sur 1033 objets et 67 PNJ — écarts extrêmes tranchés (`tools/stats_decisions.json`)
- [x] Recettes du wiki : 900 objets (Calamity + matériaux du wiki), Shimmer
- [x] Matériaux du wiki générés automatiquement (218)
- [x] Butin du wiki ajouté (additif)

## Phase 2 — Systèmes inventés par le wiki

- [x] Elemental System (17 éléments, statuts, résistances/vulnérabilités de 127 PNJ, 688 armes)
- [x] Éléments des armures → résistances du joueur (31/10 ; ensemble complet, élément d'un ennemi = ses résistances, à valider)
- [ ] Classe Spy : proficiency, Level Meters, stealth tel que décrit (pages `Spy.txt`, `Level Meters`)
- [ ] Wind : dégâts à l'atterrissage
- [x] **Shortstrike / Shortflurry** (22/10) — reste : 17 effets spéciaux (`reports/shortstrike.md`) et les armes manquantes
- [ ] Autres pages « Game mechanics » (catégorie dans l'index)

## Phase 3 — Compléter les fondations de données

- [x] **Conditions de butin** en note : 177 notes comprises (boss vaincus, biome, premier kill), 19 drops encore en
      attente (boss inventés, « While supercharged »...) — nuit du 22/10
- [x] **Sacs de trésor** (Expert) : page « Treasure Bag » + drops des boss selon le mode (22/10)
- [x] Infobox « item drops » des pages d'objets : 322 drops ajoutés (nuit du 22/10)
- [ ] Sources génériques de ces infobox (« Any Boss », « any Jungle enemy » : 27) et variantes d'un même nom (Zombie)
- [ ] **Barres posables** : tuiles pour les matériaux « Block » (Alloy Bar...), puis blocs/murs/meubles du wiki
- [ ] Recettes vanilla modifiées par le wiki (exclues pour l'instant)
- [ ] Infobulles du wiki sur les objets officiels (quand le comportement correspond)

## Reworks demandés par les pages (comportement différent de Calamity)

- [ ] **Meatmetal Sans** (page Ravager) : vrai combat façon Sans (os, Gaster Blasters, écran noir, Handheld Tank),
      2 phases, apparence ; version minimale déjà en place
- [ ] **Slime Puppet Staff** : boules de slime qui servent de barrières reliées entre elles (0 dégât, 10 max)

## Portage du contenu retiré de Calamity (en cours)

91 objets portés au 30/10 (`Content/Legacy/`, choix dans `docs/decisions.md`). Les retraits « nets » (fichier supprimé
sans remplaçant) sont épuisés. Restent : les objets que Calamity a **remplacés ou renommés** (`LegacyName`, renommages
git : Seabow → Reed Blowgun, Lazinator, Frigid Bulwark...), en attente de décision (`docs/TESTS.md`), Arcanum of the
Void (effet de la page inventé ?), Exo Gladius, Microcell Cut, et l'Agent (PNJ inventé qui donne Dualpoon/Triploon).
Base commune quand plusieurs objets se ressemblent (kits de pêche, épées courtes, harpons). Effets disparus du code
actuel : repris du vieux CalamityPlayer dans un ModPlayer.

## Phase 4 — Nouveau contenu (`reports/new_content.md`)

Prioriser avec `reports/recipes.md` → « Noms introuvables » et `reports/loot_applied.md` → « Objets introuvables » :
ce qui débloque le plus de recettes et de drops d'abord. Ensuite : debuffs → ennemis → armes/coverings → boss → biomes.
Chaque boss du wiki entraîne butin, sac, musique, invocation. Les biomes (Exosphere, Abyssphere...) sont les plus
coûteux (génération de monde).

## Phase 5 — Les cas « addon »

778 pages attribuent leur contenu à des addons (`reports/addons.md`) : Tranquility, Community Remix, Tales of the
Aquatic, Fargo's DLC... dont certains existent vraiment. État de chaque addon, licences des sources de `OtherMods/`
et règle générale (le Fandom se trompe sur les addons) : `docs/decisions.md`, section 11. Reste à trancher : comment
intégrer leurs pages (ici, ou en sous-mods), et les addons pas encore classés.

## Décisions en attente (pour l'utilisateur)

Voir aussi `docs/TESTS.md` → « Décisions en attente » et les marqueurs **[choix]** de `docs/decisions.md`.

1. ~~Les écarts de stats extrêmes~~ : tous tranchés (`tools/stats_decisions.json`).
2. Les choix d'équilibrage du système d'éléments (`docs/decisions.md`) : à valider en jeu.
3. Le sort des addons (phase 5).
