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
- [x] Graydron (Murasama) : 50 % de critique ; dégâts **2200 en Calamity++** (validé 21/10), **3001 en Full Fandom**
      (3001 était sa valeur dans Calamity 2022 : Full Fandom seulement, page `{{mode|3001|2200}}`) — à vérifier en Full
- [x] Ark of the Cosmos : 2000 dégâts
- [x] Un objet modifié en vitesse (ex. Absolute Zero, `use` 20 → 19) frappe à la bonne cadence, sans bug d'animation
- [x] Une arme à plusieurs tirs par usage (ex. Clockwork Bow) tire toujours correctement
- [x] Un ennemi modifié (ex. Aero Slime, défense 10) : PV et défense cohérents avec le wiki (`/` Cheat Sheet → NPC)
- [x] Rien de cassé côté prix de vente (pas d'exemple précis testé, semble bon)
- [x] Une arme de spy (rogue) a aussi les stats du wiki : Garnet (ex-Malachite) 65 dégâts, Cinquedea 36 ; et une
      épée « tenue » : Galileo Gladius `use` 10 (ces objets étaient oubliés avant la correction du 21/10)

### 1.3 Recettes du wiki (`WikiData/recipes.json`, bilan dans `reports/recipes.md`)

Chaque recette changée est vérifiée par l'auto-test ; en jeu, contrôler surtout le comportement :
- [x] Aerospec Breastplate : 11 Aerialite Bar, 10 Cloud, 5 Rain Cloud, 2 Feather au **Sky Mill**
- [x] Abyss Candle : Lumenyl au lieu de Torch
- [x] Une recette avec groupe « A/B » accepte les deux objets. Exemple : **Crystal of the Wicked** (« Shadow Scale /
      Tissue Sample ») : le jeu n'affiche que l'objet représentatif du groupe (Shadow Scale), comme « Any Iron Bar ».
      Tester : avoir des **Tissue Sample** et aucune Shadow Scale (+ le reste) → la recette doit être fabricable ; le
      survol de l'ingrédient affiche « Shadow Scale / Tissue Sample »
- [x] **En Calamity++** : une recette d'origine absente du wiki est toujours là (ex. Acidwood depuis Acidwood Wall).
      (En Full Fandom elle disparaît : vérifié le 21/10.)
- [x] Les recettes de Calamity à condition gardent leur condition. Exemple : **Decryption Computer** (recette modifiée
      par le wiki : 10 Dubious Plating, 10 Any Copper Bar, 15 Glass à l'enclume) est verrouillée au palier 1 de
      l'arsenal de Draedon : dans un monde neuf, la recette affiche sa condition et n'est pas fabricable ; après
      avoir utilisé une **Schematic (Sunken Sea)** (Cheat Sheet), elle le devient
- [x] Shimmer : une transmutation du wiki fonctionne (Unpurified Gel) (liste dans `WikiData/recipes.json` → `shimmer`)
- [x] Le guide (« Guide → objets craftables ») ne plante pas et affiche les nouvelles recettes

### 1.4 Système d'éléments (`Common/Elements/`, `WikiData/elements.json`)

Données et immunités vérifiées par l'auto-test ; en jeu :
- [x] Infobulle de Graydron : ligne « Elements: Slash, Machine » colorée, sous les dégâts
- [x] Zweihandromeda : « Elements: Dark, Magic, Stab »
- [x] Frapper un Aero Slime (vulnérable Fire/Dark) avec une arme Fire : dégâts doublés, **chiffre de dégâts jaune**, plus
      de texte « Vulnerable! » (changé le 22/10)
- [x] Avec une arme Poison sur l'Aero Slime : **chiffre gris**, pas de texte « Resisted », pas de debuff Envenomed
- [x] Noms des debuffs d'éléments (Fandom Test Vial) : Numbed, Ablaze, Waterlogged, Envenomed, Corroded, Radiation Sickness,
      Shocked, Punctured, Lacerated, Discombobulated, Hexed, Incapacitated, Smitten, Damned, Malfunctioning
      (aucun effet sur le joueur : normal, le wiki ne décrit les effets que sur les ennemis)
- [x] Effets d'éléments sans icône au-dessus des ennemis : particules propres à l'élément (glace pour Cold, sang pour
      Stab/Slash, étincelles pour Electricity, fumée sombre pour Dark...) et légère teinte de l'ennemi
- [x] Full Fandom : chaque coup applique tous les effets ; Calamity++ : seulement de temps en temps (25 % / 15 %)
- [x] Un ennemi Chilled/Shocked est visiblement ralenti ; un boss l'est moins
- [x] Les projectiles d'une arme élémentaire (arc, bâton) appliquent aussi les effets
- [ ] Multijoueur (si possible) : les debuffs se voient chez les deux joueurs (non testé, accepté le 22/10)

### 1.5 Matériaux du wiki générés (218, `WikiData/items.json`)

- [x] Alloy Bar, Yharim Bar, Crystal of the Cyborg existent (Cheat Sheet), avec leur sprite et leur infobulle
      (pas encore posables : pas de tuiles, normal)
- [x] Un matériau animé (Essential Essence Bar, Galactica Singularity) s'anime dans l'inventaire et flotte au sol
- [x] Alloy Bar se fabrique (recette du wiki) et sert dans d'autres recettes (guide)
- [x] Le prix de vente et la rareté (couleur du nom) sont cohérents
- [x] Aucun sprite n'est énorme ou décalé dans l'inventaire
- [x] **Yharim Bar jeté au sol** : géant (x16, comme l'image de sa page), posé sur le sol, ramassable en touchant
      n'importe quelle partie ; taille normale dans l'inventaire (ajouté le 22/10, les deux modes)
- [x] Yharim Bar au sol : version **détaillée** (celle de l'image du wiki), plus le sprite agrandi

### 1.6 Butin du wiki (bilan dans `reports/loot_applied.md`)

- [x] Aero Slime peut lâcher Skyline Wings (3 %)
- [x] Desert Scourge (mode **normal**, monde Classic) lâche Parched Scale, Profaned Shard, Ocean Crest ; **pas** d'Abyss
      Bar (condition Post-Golem ignorée)
- [x] Desert Scourge en **Expert**, Full Fandom comme Calamity++ : lâche son **sac** (corrigé le 22/10 : le Full Fandom
      retirait le sac, absent de l'infobox du boss mais documenté par la page Treasure Bag)
- [x] Desert Scourge : apparaît avec toute sa vie (barre pleine), pas 5 000 sur 750 000 (corrigé le 22/10 : les
      segments du ver n'avaient pas le ratio de vie de la tête)
- [x] **Calamity++, Expert** : le sac du Desert Scourge contient en plus Parched Scale (10-16), Profaned Shard, Coral /
      Seashell / Starfish (7-11), Aero Stone... (drops de sa page + page Treasure Bag) ; rien ne tombe en double
- [x] **Full Fandom, Expert** : le Desert Scourge lâche **directement** Parched Scale (10-16), Profaned Shard, Ocean
      Crest, une arme... **et** son sac, qui ne contient que ce que liste la page Treasure Bag (Ocean Crest, masque,
      armes à 33 %, Aero Stone, Sand Cloak, Coral / Seashell / Starfish ; pas de Victory Shard, objet introuvable)
- [x] En Expert, un boss ne lâche pas en double ce qui est déjà dans son sac
- [x] Le bestiaire affiche les nouveaux drops (Skyline Wings sur l'Aero Slime, Parched Scale sur le Desert Scourge) et,
      en Full Fandom, plus ceux qui ont été retirés (corrigé le 22/10)
- [x] Un ver (Eater of Worlds) ne lâche son butin qu'une fois
- [x] Desert Scourge (mode normal) lâche aussi **3-8 Abyss Bar** (la condition « Post-Golem » n'est pas affichée sur
      le wiki : supprimée le 22/10) et un **Sandy Angling Kit** (porté le 22/10) ; Conquest Fragment : 25-30
- [x] Sandy Angling Kit : clic droit = objets de pêche (Tackle Box, Angler Hat, potions de pêche...), Golden Bug Net
      seulement après Skeletron ; sprite de 2022
- [ ] **Conditions de butin** (nuit du 22/10) : dans le bestiaire, des drops affichent leur condition (ex. Devil Fish,
      Gulper Eel : Lumenyl « After defeating Calamitas » ; Heat Spirit : Essence of Chaos « In Hardmode ») ; avant
      Calamitas, un Devil Fish ne lâche pas de Lumenyl de ce drop, après oui (Boss Checklist / Cheat Sheet pour marquer
      le boss vaincu)
- [ ] **Butin des pages d'objets** (nuit du 22/10) : le Wall of Flesh lâche aussi 3-5 Twisted Tendril, Plantera 4-8
      Essence of Babil, les Zombies parfois de la Disgusting Meat (visible dans le bestiaire) ; en Full Fandom, le Wall
      of Flesh garde tout son butin vanilla
- [ ] **Portages de la nuit du 22/10** (Cheat Sheet) : Bleached Angling Kit (clic droit, lâché par l'Aquatic Scourge),
      Seabound Staff (une étoile de mer sbire au curseur), Effluvium Bow (2 flèches, flèches en bois → Mist Arrow),
      Regenator (vie max divisée par 2, régénération rapide), Storm Spray (jet d'eau qui goutte), Aquatic Discharge
      (estoc, étincelles au sol quand un ennemi meurt)
- [ ] Second lot : Solar Flare (yoyo, explosions sacrées), Diseased Pike (lance, Plague Seekers), Crimslime Staff et
      Corroslime Staff (un slime sbire chacun, le buff disparaît quand le slime meurt)
- [ ] Troisième lot : Gehenna (deux boules de feu tombent du ciel toutes les 10 s, explosion rouge persistante),
      Shock Grenade (explose en éclairs ; furtive : aura électrique et éclairs à tête chercheuse ; se fabrique par 100)
- [ ] Desert Scourge (mode normal) : peut aussi lâcher Seabound Staff, Storm Spray ou Aquatic Discharge (25 % chacun)
- [ ] Conquest Fragment (Pearl Shard) : sprite du wiki (éclat bleu et doré) dans l'inventaire et au sol, **dans les
      deux modes** ; idem Victide Bar, Divine Bar, Parched Scale (liste : `reports/sprites.md`)
- [ ] **Full Fandom** : des objets au sprite refait depuis 2022 reprennent celui du wiki (ex.
      The Ballista, Purified Gel, Exoblade, Radiant Star) ; **Calamity++** : sprites actuels
- [ ] Aucun sprite remplacé n'est coupé, décalé ou énorme dans l'inventaire (en Full Fandom, parcourir Cheat Sheet)

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

### 2.3 Décision du 21/10 : contenu ancien cité nulle part

- [ ] Full Fandom : **The Old Reaper** (ancienne arme de spy, citée nulle part sur le wiki) n'a plus de recette ni de
      drop ; les **Astral / Sulphurous / Sunken Pylon** ne sont plus vendus (liste complète : `reports/baseline.md`,
      section « Antérieurs, cités nulle part »)
- [ ] Full Fandom : Acidwood Wall existe toujours (cité dans un changelog du wiki)
- [ ] Renommages trouvés au passage : The Etomer → **Triboluminescent Etomer**, Martian Distress Remote → **Martian
      Distress Beacon** (dans tous les modes)

---

## Décisions en attente (pas des tests)

- [x] Écarts de stats extrêmes : tous tranchés (`tools/stats_decisions.json`)
- [ ] Les chiffres du système d'éléments (`docs/decisions.md`) : à juger en jouant.
- [x] Full Fandom : anciens objets dont le wiki ne parle nulle part → masqués aussi (tranché le 21/10, tests en 2.3)
- [ ] Les addons (Tranquility, Community Remix...) : voir `docs/ROADMAP.md`, phase 5.
