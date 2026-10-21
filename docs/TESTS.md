# Checklist de test en jeu

À dérouler après chaque passe. Une section par passe ; on coche `[x]` ce qui est validé, on note `KO :` + description
sous une ligne qui ne va pas. Les tests restés ouverts des passes précédentes restent à faire.

**Périmètre** : ce fichier couvre **tout ce qui est implémenté**. Ce qui n'est pas encore fait (portage des 264 autres
contenus retirés, équilibrage Calamity++, sacs de trésor, barres posables...) est dans `docs/ROADMAP.md` et n'a rien à
tester ici. Les décisions qui t'attendent sont en bas de ce fichier.

Astuces : Cheat Sheet / Hero's Mod pour faire apparaître objets et PNJ ; `/calfandom` n'existe pas (encore).

---

## Passe 0 — Démo : Zweihandromeda

- [x] L'objet apparaît, nom « Zweihandromeda », sprite correct dans l'inventaire
- [x] L'épée est tenue par la poignée, dans la main
- [x] Coups alternés : un vers le bas, le même vers le haut, orientés vers le curseur
- [x] Des étoiles (Stratus Star) tombent du ciel sur l'ennemi touché
- [x] L'ennemi touché reçoit le debuff Moon Burnt (= Nightwither de Calamity), 3 s, particules violettes
- [x] L'icône du debuff Moon Burnt est celle du wiki (flamme bleue), plus celle de Calamity (`Assets/Overrides/`)
      KO (21/10) : l'icône au-dessus des boss venait de l'affichage de Calamity, qui ne passait pas par le remplacement.
      Corrigé et validé le 21/10
- [x] Recette au Cosmic Anvil : Stratus Sphere, Galileo Gladius, Crescent Moon, **Night's Gaze**, 25 Lumenyl,
      37 Depth Cells, 4 Ascendant Spirit Essence, 8 Auric Bar (recette du wiki, voir 2.1)

---

## Passe 1 — Fondations (nuit du 21/10)

### 1.1 Vocabulaire et renommages du wiki (`Localization/en-US/Mods.CalamityMod.hjson`)

- [x] Le mod se charge sans erreur de localisation (pas de fichier renommé en `.legacy` dans `Localization/`)
- [x] Murasama s'appelle **Graydron**
- [x] Uelibloom Bar/Ore → **Tarragon Bar/Ore** ; Uelibloom Brick garde son nom
- [x] Astrum Deus (boss, barre de vie, sac, trophée, masque) → **Astral Deus** ; Hide of Astrum Deus garde son nom
- [x] Nightwither → **Moon Burnt** : le jeu n'affiche pas les debuffs d'un ennemi au survol. Prendre la
      **Fandom Test Vial** (Cheat Sheet, objet de debug) : clic droit pour choisir « Moon Burnt », clic gauche pour
      se l'appliquer → nom « Moon Burnt », icône bleue du wiki et description dans ta barre de buffs
- [x] Rogue Emblem → **Spy Emblem** ; infobulles des armes rogue : « spy damage », « Spy » au lieu de « Rogue »
- [x] Contrôles : « Covering Ability 1..7 » au lieu de « Accessory Ability »
- [x] Draconic Swarmer (invoqué par la Dragonfolly) → **Tempest Toddler**
- [x] Les textes réécrits gardent leurs mises en forme : survoler **Chalice of the Blood God** → la ligne
      « Grants immunity to Bleeding, Burning Blood... » montre les noms de debuffs en couleur (pas de texte brut
      du genre `[cbuffplr:Bleeding]`), et la phrase dit « this covering » au lieu de « this accessory »

### 1.2 Stats du wiki (`WikiData/stats.json`, 1204 objets, 67 PNJ)

Contrôlé automatiquement par `tools/selftest.py` (toutes les valeurs du JSON). En jeu, vérifier surtout le ressenti :
- [ ] Graydron (Murasama) : 3001 dégâts, 50 % de critique dans l'infobulle
- [ ] Ark of the Cosmos : 2000 dégâts
- [ ] Un objet modifié en vitesse (ex. Absolute Zero, `use` 19) frappe à la bonne cadence, sans bug d'animation
- [ ] Une arme à plusieurs tirs par usage (ex. Clockwork Bow) tire toujours correctement
- [ ] Un ennemi modifié (ex. Aero Slime, défense 10) : PV et défense cohérents avec le wiki (`/` Cheat Sheet → NPC)
- [ ] Rien de cassé côté prix de vente (vendre un objet modifié à un PNJ)
- [ ] Une arme de spy (rogue) a aussi les stats du wiki : Garnet (ex-Malachite) 65 dégâts, Cinquedea 36 ; et une
      épée « tenue » : Galileo Gladius `use` 10 (ces objets étaient oubliés avant la correction du 21/10)

### 1.3 Recettes du wiki (`WikiData/recipes.json`, bilan dans `reports/recipes.md`)

Chaque recette changée est vérifiée par l'auto-test ; en jeu, contrôler surtout le comportement :
- [ ] Aerospec Breastplate : 11 Aerialite Bar, 10 Cloud, 5 Rain Cloud, 2 Feather au **Sky Mill**
- [ ] Abyss Candle : Lumenyl au lieu de Torch
- [ ] Une recette avec groupe « A/B » du wiki (chercher « / » dans `reports/recipes.md`) accepte bien les deux objets
- [ ] Une recette d'origine absente du wiki est toujours là (ex. Acidwood depuis Acidwood Wall)
- [ ] Les recettes de Calamity à condition (ex. objets Draedon, Revengeance) gardent leurs conditions
- [ ] Shimmer : une transmutation du wiki fonctionne (liste dans `WikiData/recipes.json` → `shimmer`)
- [ ] Le guide (« Guide → objets craftables ») ne plante pas et affiche les nouvelles recettes

### 1.4 Système d'éléments (`Common/Elements/`, `WikiData/elements.json`)

Données et immunités vérifiées par l'auto-test ; en jeu :
- [ ] Infobulle de Graydron : ligne « Elements: Slash, Machine » colorée, sous les dégâts
- [ ] Zweihandromeda : « Elements: Dark, Magic, Stab »
- [ ] Frapper un Aero Slime (vulnérable Fire/Dark) avec une arme Fire : texte « Vulnerable! », dégâts doublés
- [ ] Avec une arme Poison sur l'Aero Slime : texte « Resisted », pas de debuff Envenomed
- [ ] Noms des debuffs d'éléments (Fandom Test Vial) : Numbed, Ablaze, Waterlogged, Envenomed, Corroded, Radiation Sickness,
      Shocked, Punctured, Lacerated, Discombobulated, Hexed, Incapacitated, Smitten, Damned, Malfunctioning
- [ ] Effets d'éléments sans icône au-dessus des ennemis : particules propres à l'élément (glace pour Cold, sang pour
      Stab/Slash, étincelles pour Electricity, fumée sombre pour Dark...) et légère teinte de l'ennemi
- [ ] Full Fandom : chaque coup applique tous les effets ; Calamity++ : seulement de temps en temps (25 % / 15 %)
- [ ] Un ennemi Chilled/Shocked est visiblement ralenti ; un boss l'est moins
- [ ] Les projectiles d'une arme élémentaire (arc, bâton) appliquent aussi les effets
- [ ] Multijoueur (si possible) : les debuffs se voient chez les deux joueurs

### 1.5 Matériaux du wiki générés (218, `WikiData/items.json`)

- [ ] Alloy Bar, Yharim Bar, Crystal of the Cyborg existent (Cheat Sheet), avec leur sprite et leur infobulle
- [ ] Un matériau animé (Essential Essence Bar, Galactica Singularity) s'anime dans l'inventaire et flotte au sol
- [ ] Alloy Bar se fabrique (recette du wiki) et sert dans d'autres recettes (guide)
- [ ] Le prix de vente et la rareté (couleur du nom) sont cohérents
- [ ] Aucun sprite n'est énorme ou décalé dans l'inventaire

### 1.6 Butin du wiki (bilan dans `reports/loot_applied.md`)

- [ ] Aero Slime peut lâcher Skyline Wings (3 %) — voir le bestiaire
- [ ] Desert Scourge (mode normal) lâche Parched Scale, Profaned Shard, Ocean Crest ; **pas** d'Abyss Bar (condition Post-Golem ignorée)
- [ ] En Expert, un boss ne lâche pas en double ce qui est déjà dans son sac
- [ ] Les entrées du bestiaire affichent les nouveaux drops sans planter
- [ ] Un ver (Aquatic Scourge) ne lâche son butin qu'une fois

---

## Passe 2 — Modes de fidélité (`docs/MODES.md`)

Contrôlé par `python tools/selftest.py --mode all`. En jeu :
- [ ] Config « Calamity Fandom » : « Fandom accuracy » à 2 choix (Full Fandom / Calamity++, Calamity++ par défaut) +
      option de debug « Disable Calamity++ balancing » ; changer demande un rechargement
- [ ] Monde créé en Calamity++ puis ouvert en Full Fandom : message orange à l'entrée (pas de message entre
      Calamity++ et son option de debug)
- [ ] Full Fandom : un objet récent de Calamity (voir `reports/baseline.md`, section « masqués ») n'a plus de recette
- [ ] Desert Scourge : 350 000 PV du wiki (ratio x87,5) **dans tous les modes** (validé le 21/10)
- [ ] Full Fandom : Acidwood ne se fabrique plus depuis Acidwood Wall (recette absente du wiki)
- [ ] Full Fandom : un PNJ récent masqué (liste « PNJ » de `reports/baseline.md`) n'apparaît plus naturellement
- [ ] Full Fandom : les marchands ne vendent plus d'objet masqué ; un objet masqué déjà dans l'inventaire reste utilisable
- [ ] Full Fandom : les boss récents ne s'invoquent plus (leur objet d'invocation n'a plus de recette ni de drop)
- [ ] Full Fandom : Starfleet à 68 dégâts (valeur de 2022) ; en Calamity++ elle garde ses dégâts actuels
- [ ] Retour en Calamity++ : tout revient comme avant

### 2.1 Premier contenu porté : Night's Gaze

- [ ] Night's Gaze existe (Cheat Sheet), sprite + glowmask, infobulle du wiki, « Elements: Dark, Magic, Stab »
- [ ] Lancer : la lance part, se brise sur un ennemi en étoiles et étincelles, Moon Burnt sur la cible
- [ ] Attaque furtive : la lance sème des étoiles à tête chercheuse
- [ ] Recette à l'Ancient Manipulator : Daybreak, 7 Lumenyl, 4 Ruinous Soul, 12 Exodium Cluster
- [x] Zweihandromeda : la recette demande maintenant aussi Night's Gaze

### 2.2 Décisions du 21/10 : stats par mode, Meatmetal Sans, outils de test

**Outils de debug** (Cheat Sheet, sans recette) : **Fandom Dummy Spawner** (clic gauche : pose au curseur un mannequin
géant immobile, 1 million de PV, ne meurt jamais, neutre à tous les éléments ; clic droit : les retire) et 18
**Test Swords** (une par élément + Moon Burnt, de la couleur de l'élément, 10 dégâts).

- [ ] Le mannequin se pose, ne bouge pas, ne fait pas de dégâts, se régénère quand il descend sous 10 % de ses PV
- [ ] Test Sword: Moon Burnt sur le mannequin → icône bleue du wiki au-dessus de lui
- [ ] Chaque Test Sword d'élément (Cold, Fire, Water, Poison, Acid, Radioactive, Electricity, Stab, Slash, Smash, Magic,
      Dark, Holy, Unholy, Machine) : particules de l'élément et teinte sur le mannequin, pas d'icône ; infobulle
      « Elements: X » ; en Full Fandom à chaque coup, en Calamity++ seulement de temps en temps
- [ ] Test Sword: Wind repousse plus fort (sans effet sur le mannequin, qui ne recule pas : tester sur un ennemi normal)
- [ ] Test Sword: Bomb : coups critiques plus fréquents et plus forts

- [x] **Fandom Test Vial** (Cheat Sheet) : clic droit = debuff suivant (nom affiché), clic gauche = appliqué à toi et à
      l'ennemi le plus proche (message dans le chat). Liste : Moon Burnt, Marked, puis les 15 statuts d'éléments
- [ ] Sur un boss : Moon Burnt garde son icône du wiki au-dessus de lui, mais **pas** d'icône d'élément

- [ ] Ravager (Meatmetal Sans) : il faut exactement 23 coups pour le tuer, chaque coup ne retire qu'1 PV
- [ ] Il « esquive » : les coups rapprochés (moins de 0,5 s) ne comptent pas
- [ ] Ses attaques (contact et projectiles) ne font qu'1 dégât mais empoisonnent
- [ ] Temporal Umbrella : coûte 520 mana, utilisable une fois le mana max assez boosté
- [ ] Relic of Ruin et Stormjaw Staff gardent leurs dégâts en Calamity++ (1 et 0 en Full Fandom)
- [ ] Le Ravager s'appelle **Meatmetal Sans** (boss, masque, trophée, relique, sac)
- [ ] Ceaseless Void : 0 de défense

---

## Décisions en attente (pas des tests)

- [x] Écarts de stats extrêmes : tous tranchés (`tools/stats_decisions.json`)
- [ ] Les chiffres du système d'éléments (`docs/decisions.md`) : à juger en jouant.
- [ ] Les addons (Tranquility, Community Remix...) : voir `docs/ROADMAP.md`, phase 5.
