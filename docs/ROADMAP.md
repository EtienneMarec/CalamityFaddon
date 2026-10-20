# Feuille de route

Ordre pensé pour avoir vite un résultat jouable et pour que chaque phase serve aux suivantes.
Chiffres issus de `reports/README.md` (à régénérer).

## Phase 0 — Fondations (fait)

- [x] Squelette du mod, référence à `CalamityMod.dll`, build propre (référence exclue du `.tmod`)
- [x] Indexeur du wiki : statut par page, renommages, écarts de stats, suivi d'avancement
- [x] Conversion des sprites WebP → PNG
- [x] Objet de démonstration : Zweihandromeda (+ projectile Stratus Star)

## Phase 1 — « Ça ressemble au wiki » (gros effet, peu de code)

- [ ] **Renommages** (`reports/renames.md`, ~31) : remplacer les textes localisés de Calamity
      (Murasama → Graydron, Uelibloom → Tarragon, Nightwither → Moon Burnt...). Système unique dans `Common/`.
- [ ] **Terminologie** : Rogue → Spy, accessoires → Coverings, post-Moon Lord → Godseeker Mode
      dans les infobulles et l'interface.
- [ ] **Stats** (`reports/stat_drift.md`, ~600 objets/PNJ) : table de surcharges appliquée par un GlobalItem/GlobalNPC.
      Les valeurs viennent des infobox, donc on peut la générer automatiquement, puis la relire.
- [ ] **Recettes** : parser `Recipes_*_register.txt`, comparer avec les recettes de Calamity, générer les ajouts/retraits.

## Phase 2 — Systèmes inventés par le wiki

À faire avant le contenu qui en dépend :
- [ ] Elemental System (`element1..n` dans ~1100 infobox, faiblesses/résistances, effets par élément)
- [ ] Classe Spy : proficiency, Level Meters, stealth tel que décrit
- [ ] Ce que les pages « Game mechanics » décrivent d'autre (voir la catégorie dans l'index)

## Phase 3 — Nouveau contenu (`reports/new_content.md`, ~1570 pages)

Par dépendances : matériaux et minerais → debuffs → ennemis → armes/coverings → bosses → biomes.
Chaque boss du wiki entraîne son butin, sa musique, ses invocations. Les biomes (Exosphere, Abyssphere...) sont
les plus coûteux (génération de monde).

## Phase 4 — Les cas « addon »

775 pages attribuent leur contenu à des addons (`reports/addons.md`) : Tranquility, Community Remix, Tales of the
Aquatic, Fargo's DLC... dont certains existent vraiment. À trancher : on les intègre tous dans ce mod, ou on
découpe en sous-mods, ou on laisse de côté ceux qui existent réellement sur le Workshop.
