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
- [x] Stats des infobox sur 1033 objets et 67 PNJ — **38 écarts extrêmes à trancher** (`reports/stats.md`)
- [x] Recettes du wiki : 900 objets (Calamity + matériaux du wiki), Shimmer
- [x] Matériaux du wiki générés automatiquement (218)
- [x] Butin du wiki ajouté (additif)

## Phase 2 — Systèmes inventés par le wiki

- [x] Elemental System (17 éléments, statuts, résistances/vulnérabilités de 127 PNJ, 688 armes)
- [ ] Éléments des armures (« Elements » des Coverings) → résistances du joueur
- [ ] Classe Spy : proficiency, Level Meters, stealth tel que décrit (pages `Spy.txt`, `Level Meters`)
- [ ] Wind : dégâts à l'atterrissage
- [ ] Autres pages « Game mechanics » (catégorie dans l'index)

## Phase 3 — Compléter les fondations de données

- [ ] **Conditions de butin** en note (« Post-Golem », « Tier 3 », « first kill ») : 127 drops en attente
- [ ] **Sacs de trésor** (Expert) : pages « Treasure Bag (...) »
- [ ] **Barres posables** : tuiles pour les matériaux « Block » (Alloy Bar...), puis blocs/murs/meubles du wiki
- [ ] Recettes vanilla modifiées par le wiki (exclues pour l'instant)
- [ ] Infobulles du wiki sur les objets officiels (quand le comportement correspond)

## Reworks demandés par les pages (comportement différent de Calamity)

- [ ] **Meatmetal Sans** (page Ravager) : vrai combat façon Sans (os, Gaster Blasters, écran noir, Handheld Tank),
      2 phases, apparence ; version minimale déjà en place
- [ ] **Slime Puppet Staff** : boules de slime qui servent de barrières reliées entre elles (0 dégât, 10 max)

## Phase 4 — Nouveau contenu (`reports/new_content.md`)

Prioriser avec `reports/recipes.md` → « Noms introuvables » et `reports/loot_applied.md` → « Objets introuvables » :
ce qui débloque le plus de recettes et de drops d'abord. Ensuite : debuffs → ennemis → armes/coverings → boss → biomes.
Chaque boss du wiki entraîne butin, sac, musique, invocation. Les biomes (Exosphere, Abyssphere...) sont les plus
coûteux (génération de monde).

## Phase 5 — Les cas « addon »

778 pages attribuent leur contenu à des addons (`reports/addons.md`) : Tranquility, Community Remix, Tales of the
Aquatic, Fargo's DLC... dont certains existent vraiment. À trancher : tout intégrer ici, découper en sous-mods, ou
laisser de côté ceux qui existent réellement sur le Workshop.

## Décisions en attente (pour l'utilisateur)

1. Les 38 écarts de stats extrêmes (`reports/stats.md`) : appliquer, ignorer, ou corriger la page.
2. Les choix d'équilibrage du système d'éléments (`docs/decisions.md`) : à valider en jeu.
3. Le sort des addons (phase 5).
