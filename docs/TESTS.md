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
- [x] **Conditions de butin** (nuit du 22/10) : dans le bestiaire, des drops affichent leur condition (ex. Devil Fish,
      Gulper Eel : Lumenyl « After defeating Calamitas » ; Heat Spirit : Essence of Chaos « In Hardmode ») ; avant
      Calamitas, un Devil Fish ne lâche pas de Lumenyl de ce drop, après oui (Boss Checklist / Cheat Sheet pour marquer
      le boss vaincu)
- [x] **Butin des pages d'objets** (nuit du 22/10) : le Wall of Flesh lâche aussi 3-5 Twisted Tendril, Plantera 4-8
      Essence of Babil, les Zombies parfois de la Disgusting Meat (visible dans le bestiaire) ; en Full Fandom, le Wall
      of Flesh garde tout son butin vanilla
- [x] **Portages de la nuit du 22/10** (Cheat Sheet) : Bleached Angling Kit (clic droit, lâché par l'Aquatic Scourge),
      Seabound Staff (une étoile de mer sbire au curseur), Effluvium Bow (2 flèches, flèches en bois → Mist Arrow),
      Regenator (vie max divisée par 2, régénération rapide), Storm Spray (jet d'eau qui goutte), Aquatic Discharge
      (estoc, étincelles au sol quand un ennemi meurt)
- [x] Second lot : Solar Flare (yoyo, explosions sacrées), Diseased Pike (lance, Plague Seekers), Crimslime Staff et
      Corroslime Staff (un slime sbire chacun, le buff disparaît quand le slime meurt)
- [x] Troisième lot : Gehenna (deux boules de feu tombent du ciel toutes les 10 s, explosion rouge persistante),
      Shock Grenade (explose en éclairs ; furtive : aura électrique et éclairs à tête chercheuse ; se fabrique par 100)
- [x] Quatrième lot : Mana Jelly (+20 mana, régénération à l'arrêt), Fungal Carapace (spores quand on est touché),
      Amidias' Spark (étincelles quand on est touché), Energy Staff (tourelle), Beetle Juice ; le Starspawn Helix Staff
      s'appelle **Godspawn Helix Staff**
- [x] Cinquième lot : Starfall (5 étoiles tombent sur le curseur), Storm Dragoon (rafale, consomme peu de balles),
      Soul Edge (âmes fantomatiques, Crush Depth)
- [x] Devil Fish (bestiaire) : Depth Cells « After defeating Calamitas » (1-2), Abyss Ore « After defeating Golem » (3-9),
      une seule ligne d'Abyss Ore (corrigé le 22/10)
- [x] Diseased Pike : la lance pointe dans la bonne direction (corrigé le 22/10)
- [x] Energy Staff : la tourelle reste là où on la pose (corrigé le 22/10 : ancienne tourelle immobile)
- [x] Desert Scourge (mode normal) : peut aussi lâcher Seabound Staff, Storm Spray ou Aquatic Discharge (25 % chacun)
- [x] Conquest Fragment (Pearl Shard) : sprite du wiki (éclat bleu et doré) dans l'inventaire et au sol, **dans les
      deux modes** ; idem Victide Bar, Divine Bar, Parched Scale (liste : `reports/sprites.md`)
- [x] **Full Fandom** : des objets au sprite refait depuis 2022 reprennent celui du wiki (ex.
      The Ballista, Purified Gel, Exoblade, Radiant Star) ; **Calamity++** : sprites actuels
- [x] Aucun sprite remplacé n'est coupé, décalé ou énorme dans l'inventaire (en Full Fandom, parcourir Cheat Sheet)

---

## Passe 2 — Modes de fidélité (`docs/MODES.md`)

Contrôlé par `python tools/selftest.py --mode all`. En jeu :
- [x] Config « Calamity Fandom » : « Fandom accuracy » à 2 choix (Full Fandom / Calamity++, Calamity++ par défaut) +
      option de debug « Disable Calamity++ balancing » ; changer demande un rechargement
- [x] Monde créé en Calamity++ puis ouvert en Full Fandom : message orange à l'entrée (pas de message entre
      Calamity++ et son option de debug)
- [x] Full Fandom : un objet récent de Calamity n'a plus de recette. Exemple : **Acidwood Sword** (établi, Acidwood) ;
      en Calamity++ elle se fabrique
- [x] Desert Scourge : 350 000 PV du wiki (ratio x87,5) **dans tous les modes** (validé le 21/10)
- [x] Full Fandom : Acidwood ne se fabrique plus depuis Acidwood Wall (recette absente du wiki)
- [x] Full Fandom : un PNJ récent masqué n'apparaît plus naturellement. Exemple : **Slab Crab** (couches de l'Abysse)
- [x] Full Fandom : les marchands ne vendent plus d'objet masqué. Exemple : la **Dryad** ne vend plus d'**Astral Grass
      Seeds** en Hardmode (Calamity++ : oui) ; un objet masqué déjà dans l'inventaire reste utilisable
- [x] (28/10) Les objets masqués ne laissent plus de **case vide** au milieu du magasin : les suivants remontent
- [x] Full Fandom : les boss récents ne s'invoquent plus — sans objet : le seul boss récent sans page est le Horrible
      Hog, qui naît d'un cochon ; les autres « nouveaux » boss sont des renommages (23/10)
- [x] Full Fandom : Starfleet à 68 dégâts (valeur de 2022) ; en Calamity++ elle garde ses dégâts actuels
- [x] Retour en Calamity++ : tout revient comme avant

### 2.1 Premier contenu porté : Night's Gaze

- [x] Night's Gaze existe (Cheat Sheet), sprite + glowmask, infobulle du wiki, « Elements: Dark, Magic, Stab »
- [x] Lancer : la lance part, se brise sur un ennemi en étoiles et étincelles, Moon Burnt sur la cible
- [x] Attaque furtive : la lance sème des étoiles à tête chercheuse
- [x] Recette à l'Ancient Manipulator : Daybreak, 7 Lumenyl, 4 Ruinous Soul, 12 Exodium Cluster
- [x] Zweihandromeda : la recette demande maintenant aussi Night's Gaze

### 2.2 Décisions du 21/10 : stats par mode, Meatmetal Sans, outils de test

**Outils de debug** (Cheat Sheet, sans recette) : **Fandom Dummy Spawner** (clic gauche : pose au curseur un mannequin
géant immobile, 1 million de PV, ne meurt jamais, neutre à tous les éléments ; clic droit : les retire) et 18
**Test Swords** (une par élément + Moon Burnt, de la couleur de l'élément, 10 dégâts).

- [x] Le mannequin se pose, ne bouge pas, ne fait pas de dégâts, se régénère quand il descend sous 10 % de ses PV
- [x] Test Sword: Moon Burnt sur le mannequin → icône bleue du wiki au-dessus de lui
- [x] Chaque Test Sword d'élément (Cold, Fire, Water, Poison, Acid, Radioactive, Electricity, Stab, Slash, Smash, Magic,
      Dark, Holy, Unholy, Machine) : particules de l'élément et teinte sur le mannequin, pas d'icône ; infobulle
      « Elements: X » ; en Full Fandom à chaque coup, en Calamity++ seulement de temps en temps
- [x] Test Sword: Wind repousse plus fort (sans effet sur le mannequin, qui ne recule pas : tester sur un ennemi normal)
- [x] Test Sword: Bomb : coups critiques plus fréquents et plus forts

- [x] **Fandom Test Vial** (Cheat Sheet) : clic droit = debuff suivant (nom affiché), clic gauche = appliqué à toi et à
      l'ennemi le plus proche (message dans le chat). Liste : Moon Burnt, Marked, puis les 15 statuts d'éléments
- [x] Sur un boss : Moon Burnt garde son icône du wiki au-dessus de lui, mais **pas** d'icône d'élément

- [x] Ravager (Meatmetal Sans) : barre de vie à **23** (deuxième correction du 28/10 : c'était la barre propre au Ravager
      de Calamity) ; exactement 23 coups pour le tuer
- [x] (28/10) Sprite de **Sans** (squelette en sweat bleu) pendant les 12 premiers coups, sans jambes ni tête du Ravager
      autour, et les coups ne touchent que lui ; au 12e coup il se téléporte (poussière bleue) et prend le sprite du
      Ravager refait du wiki pour les 11 derniers ; dans les deux modes
- [x] (28/10, 2e passe) Sans à peu près à taille humaine (un peu plus grand que le joueur), touchable seulement sur son
      sprite
- [x] (28/10, 3e passe) Ravager refait de la phase 2 réduit encore (x0,64 de l'image du wiki)
- [x] (28/10) « MISS » entre les coups, 1 PV par coup ; attaques et piliers : 1 dégât + poison
- [x] (28/10) Chaque coup compté affiche « **MISS** », projette le joueur loin de Sans, puis 1,5 s pendant lesquelles les
      coups passent à travers ; les debuffs (feu, poison...) ne lui retirent rien
- [x] (28/10) Ses attaques (contact, projectiles) **et ses piliers** font exactement 1 dégât et empoisonnent (avant : 0 dégât
      à cause de la réduction de dégâts de Calamity, et 200 pour les piliers)
- [x] Temporal Umbrella : fonctionne (test du 28/10). 520 mana + 8 emplacements de sbires (le wiki en demande 8) en même
      temps : armure Demonshade (plastron +200 mana ; casque +2 et bonus d'ensemble +8 emplacements) + 200 des Mana
      Crystals + Comet Shard, Ethereal Core et Phantom Heart (+60 chacun) = 580 mana et 11 emplacements, sans aucun
      accessoire. Avec l'Ethereal Talisman (+60, -8 % de coût) il reste de la marge
- [x] Relic of Ruin (validée) et Stormjaw Staff gardent leurs dégâts en Calamity++ ; en Full Fandom **1** (0,1 du wiki arrondi
      à 1 le 23/10) : les étincelles du sbire font 1 dégât, l'infobulle est dans le bon ordre
- [x] Le Ravager s'appelle **Meatmetal Sans** (boss, masque, trophée, relique, sac)
- [x] Ceaseless Void : 0 de défense (plus les 50 % de réduction de dégâts de Calamity, cohérents
      avec la page : 9 dégâts → 4)

### 2.3 Décision du 21/10 : contenu ancien cité nulle part

- [x] Full Fandom : **The Old Reaper** (ancienne arme de spy, citée nulle part sur le wiki) n'a plus de recette ni de
      drop ; les **Astral / Sulphurous / Sunken Pylon** ne sont plus vendus (liste complète : `reports/baseline.md`,
      section « Antérieurs, cités nulle part »)
- [x] Full Fandom : Acidwood Wall existe toujours (cité dans un changelog du wiki)
- [x] Renommages trouvés au passage : The Etomer → **Triboluminescent Etomer**, Martian Distress Remote → **Martian
      Distress Beacon** (dans tous les modes)

### 2.5 Renommages de Calamity (23/10)

- [x] **Essence of Havoc** s'appelle **Essence of Chaos** (deux modes) ; en Full Fandom elle existe toujours (recettes, butin)
- [x] Le sac de **Calamitas** (Calamitas Clone) s'appelle « Treasure Bag (Calamitas) » et tombe en Expert en Full Fandom
- [x] Calamity++ : la **Vega** s'appelle **Night's Gaze** (c'est elle qui est utilisée) ; notre ancienne version n'a plus de
      recette ; Full Fandom : c'est notre Night's Gaze de 2022, la Vega est masquée. Idem Star Shower → Starfall, Burning
      Revelation → Solar Flare, Sandstream Scepter → Storm Spray, Doomsday Device → Shock Grenade...
- [ ] (28/10) **Starfall**, **Seabound Staff** et **Effluvium Bow** n'existent plus qu'une fois (l'objet de Calamity sous ce
      nom, dans les deux modes) ; en Full Fandom ils ont les stats du wiki ; le Desert Scourge lâche toujours le Seabound
      Staff. (Une ancienne version déjà dans l'inventaire devient un objet déchargé.)
- [x] Crimulan Paladin / Ebonian Paladin s'appellent Crimulan / Ebonian Slime God
- [x] Calamitas Clone s'appelle **Calamitas**, Cataclysmic Construct **Cataclysm**, Signus **Signus, Envoy of the Devourer**
- [x] La Silva Basin s'appelle **Effulgent Manipulator** et fabrique les meubles Silva
- [x] La Purple Haze s'appelle **Fluorescent Vodka** ; **Energy Orb** existe (matériau, Cheat Sheet)

### 2.4 Shortstrike (page Shortstrike, 22/10)

- [x] Avec une **Copper Shortsword** : la jauge dessinée apparaît à droite de la jauge de vol, sans couvrir le texte des
      PNJ ; elle se remplit en frappant
      (survol : « Shortstrike: N/100 »), les épées sortent quand elle est pleine ; on peut la déplacer si les jauges de
      Calamity ne sont pas verrouillées ; taille comparable aux jauges de Calamity (2×) ; pendant le Shortflurry, elle se
      vide en tremblant ; changer d'arme la garde ; elle reste après avoir quitté et relancé le personnage
- [ ] En tenant une épée courte : dégâts et défense augmentent avec la jauge (Full Fandom : +20 % / +33 à 100 points ;
      Calamity++ : +10 % / +16) ; avec une autre arme, plus de bonus
- [ ] Jauge pleine : son, aura orange pulsante ; **clic droit** = Shortflurry : 3 clones orange frappent en 1 s, la
      jauge retombe à 0, buff « Shortflurry », +5 défense pendant la durée (Copper)
- [ ] **Aquatic Discharge** : Shortflurry de 5 coups, les ennemis touchés sont Electrified
- [ ] **Galileo Gladius** : 7 coups, immunité à Moon Burnt 20 s, +30 % de critique au corps à corps pendant 3 s
- [ ] **Cosmic Shiv** : 20 coups en 6 s, buff Cosmic Freeze 20 s
- [ ] Calamity++ : **Sahara Slicers** ont leur Shortflurry (éclats de sable) ; Full Fandom : l'arme est masquée
- [ ] Une Copper Broadsword n'affiche pas la barre et ne déclenche rien au clic droit
- [ ] Aliments du wiki (Cheat Sheet) : **Titan Finger** se mange, donne Plenty Satisfied 4 minutes ; Cheese, Clam Chowder,
      Crimtato... ; les ennemis astraux lâchent parfois des Titan Fingers (24/10)
- [ ] **Ailes du wiki** (Cheat Sheet : Red Wings, Archangel Wings, Holy Mantle...) : s'équipent, se voient (une seule pose,
      pas de battement), Red Wings rapides (vitesse 15) et +20 % de déplacement ; Holy Mantle vole sans fin ; Bellwings
      invisibles une fois portées (pas d'image)
- [ ] Shortflurry du **Galileo Gladius** : des lunes apparaissent sur les ennemis touchés ; **Cosmic Shiv** : parfois un
      rayon en plus vers le curseur ; **Submarine Shocker** : petits nuages verts immobiles (Sulphuric Poisoning) ;
      **Spore Knife** : liane verte au sol sous l'ennemi (24/10)
- [ ] **Objets de lore** (26/10, Cheat Sheet, recherche « Lore » ou par nom) : le lore de Cryogen s'appelle « Cryogen »
      (texte de l'Archmage), celui du Desert Scourge « The Desert Scourge », de Yharon « Jungle Dragon, Yharon », du
      Ravager « Meatmetal Sans », du Moon Lord « Moon Lord » (texte de Requiem) ; Full Fandom : pas de lore de Signus,
      Storm Weaver, Queen Slime ni Empress of Light, mais celui de Cryogen et de Meatmetal Sans existent
- [ ] Premier kill de Cryogen : lâche le lore « Cryogen » (une seule fois, pas de doublon)
- [x] Noms corrigés (26/10) : **Caustic Edge** (l'ancienne, pré-Hardmode) et **True Caustic Edge** sont deux objets
      distincts ; les 5 journaux de Draedon ont chacun leur nom (« Draedon's Log - Hell »...) ; meubles **Eutrophic**
      (Eutrophic Chair, Bathtub...) au lieu de « EutrophicChair » ; bannières Big/Small Sightseer, Flounder, Catfish...
- [ ] Fruits de vie : la Sanguine Tangerine s'appelle **Hemagrape**, la Tainted Cloudberry **Sambucus Fruit** (+25 PV max
      chacune, message en cas de deuxième usage avec le nouveau nom) ; la recette de Keg Fireball / Screwdriver demande la
      Hemagrape
- [ ] Le sac d'Astral Deus et celui de Meatmetal Sans reçoivent leur contenu du wiki (page Treasure Bag) : trophée à 10 %
      dans le sac d'Astral Deus, Greater Healing Potion...
- [ ] Portages du 26/10 (Cheat Sheet) :
  - [ ] **Psychotic Amulet** : +5 % dégâts/critique spy et ranged ; immobile, on devient invisible (comme la Shroomite) et
        les bonus montent ; se fabrique dans les casques Auric Tesla ; lâché par la Possessed Armor
  - [ ] **Disseminator** : salve de l'arme + une salve tombée d'au-dessus et une montée d'en dessous vers le curseur, qui
        traversent les blocs ; une munition sur deux n'est pas consommée
  - [ ] **Wrath of the Ancients** : pose une pulsation au curseur qui libère des esprits à tête chercheuse ; une seule à la
        fois ; lâché par les Necromancer
  - [ ] **Astral Bow** (rafale de 3 flèches), **Lazhar** (rayons qui rebondissent et explosent, On Fire!), **Carnage Ray**
        (rayon de sang), **Phantom Lance** (javelots qui sèment des âmes, se fabriquent par 100), **Eutrophic Scimitar**
        (deux décharges qui étourdissent, lâché par le Blinded Angler)
  - [ ] **Aftershock** (un rocher tombe du ciel à chaque coup, Armor Crunch ; lâché par les Earth Elemental),
        **Sandslasher** (shuriken qui accélère à l'horizontale ; furtif : nuages de sable), **Asteroid Staff** (3 astéroïdes
        verts vers le curseur, qui explosent)
  - [ ] **Lead Core** (immunité Irradiated, Irradiated/Gamma Slime), **Surgical Glove** (+12 % vitesse et dégâts de
        mêlée, ennemis touchés en mêlée : Plague ; lâché par le Plaguebringer Goliath), **Sulfuric Scale** (immunité
        Poisoned/Venom/Sulphuric Poisoning ; dégâts en hausse quand on perd de la vie ou sous 75/50/25 % de vie ; Nuclear
        Terror)
  - [ ] **Flask of Plague** (lâchée par le Plaguebringer Goliath) : buff « Weapon Imbue: Plague » 20 min, remplace les
        autres flasks ; un coup d'épée, de fouet ou d'arme spy inflige la Plague
  - [ ] **Plague Balloon** (vanité, Plaguebringer Goliath) : le ballon se voit sur le joueur (vérifier sa position) ; à la
        mort, nuée verte
  - [ ] **Microxodonta** (Nuclear Terror) : course bien plus rapide ; en volant avec des ailes, des grappes vertes
        restent derrière et infligent 300 aux ennemis
  - [ ] **Sea Shell** (Sea Snail) : +3 défense, dans l'eau +3 de plus et on nage librement ; **Mirage Jelly** (covering,
        lâché par les Mirage Jelly) : une attaque furtive vide un peu moins la jauge ; **Inverted Gar** (pêche dans
        l'Abysse) : touché par un ennemi, la magie ne coûte rien pendant 1 s (buff Gar Mana Boost)
  - [ ] **Glacial Crusher** (Cryogen) : iceberg lent qui gèle (ennemi bleuté, presque immobile) ; un coup d'épée sur un
        ennemi gelé fait double dégâts
  - [ ] **Trash of Magnus** et **Blow Pipe** (Plaguebringer Goliath) : éclairs verts qui infligent la Plague, pas de
        reforge possible ; sarbacane avec graines/fléchettes, clic droit plus lent qui inflige la Plague
  - [ ] **The Needler** (Plaguebringer Goliath) : comme la Needler, mais les aiguilles infligent la Plague
  - [ ] **Disease** (épée courte, Plaguebringer Goliath) : un ennemi tué libère 3 Plague Seekers ; elle a sa jauge
        Shortstrike, et sa Shortflurry lance une Plague Seeker à chaque coup
  - [ ] **Reemergence** (Plaguebringer Goliath) : avec des flèches en bois, flèches de peste qui éclatent en abeilles
  - [ ] **Phlegm** (javelot du Plaguebringer) : tête chercheuse, traînée verte qui ralentit (Slow) et inflige la Plague ;
        attaque furtive : éternuement (glaires projetées devant) ; **Toxic Wisdom** : tirs verts et roses alternés
  - [ ] **Poison I-Bee** (Plaguebringer) : disque boomerang, épines au contact ; en furtif, graines en tous sens
  - [ ] **Decomposition** (tourelle, Plaguebringer) : ruche posée au curseur, tombe au sol, lâche des abeilles de peste
        vers les ennemis proches
  - [ ] **Metal Ball** (Plaguebringer) : bouton tenu, boule au bout d'une chaîne qui pend et suit les mouvements (courir en
        rond la fait tourner) ; **Sewer Brewer** : bouton tenu = charge (son à pleine charge), relâcher = foret plus gros
        et plus fort selon la charge, qui cherche les ennemis à pleine charge
  - [ ] **Terra-cotta** (Dune Splicer après Plantera) : chaque coup fait une petite explosion dorée ; un ennemi tué
        libère 3 orbes qui soignent de 9 PV quand on les touche (attirées avec Heartreach)
  - [ ] **Blood Clot Staff** : caillot de sang qui fonce sur les ennemis ; buff « Blood Clot »
- [ ] Recettes : l'Elemental Quiver accepte n'importe quel carquois (« Any Quivers »), l'Ethereal Talisman n'importe quelle
      Mana Flower ; les recettes « Silver/Tungsten Bar » acceptent les deux barres

---

## Décisions en attente (pas des tests)

- [x] Écarts de stats extrêmes : tous tranchés (`tools/stats_decisions.json`)
- [ ] Les chiffres du système d'éléments (`docs/decisions.md`) : à juger en jouant.
- [x] Full Fandom : anciens objets dont le wiki ne parle nulle part → masqués aussi (tranché le 21/10, tests en 2.3)
- [ ] Les addons (Tranquility, Community Remix...) : voir `docs/ROADMAP.md`, phase 5.
- [ ] (26/10) Objets que Calamity a **remplacés** par un autre (`[LegacyName]`, `python tools/calamity_renames.py`) :
      les traiter comme les refontes du 23/10 (portage en Full Fandom, objet actuel sous le nom du wiki en Calamity++) ?
      Probables : Warbanner of the Sun → Warbanner of the Righteous, Urchin Flail → Urchin Mace, Rusty Medallion → Scions
      Curio, Luminous Striker → Reality Rupture, Cursed Capper → Thermocline Blaster, Alchemical Flask → Alchemical
      Decanter (bloque Plague Hive). Déjà portés dans les deux modes : Gehenna (→ Slagsplitter Pauldron), Aquatic
      Discharge (→ Sahara Slicers), Soul Edge (→ Void Edge), Crimslime Staff (→ Blighted Slime Staff). Fusions, à garder
      à part : Holy Wrath / Profaned Rage Potion → Flask of Holy Flames, Bloodstone Core → Bloodstone.
- [ ] (26/10) Renommages/refontes de Calamity **antérieurs à 2.0.1.005** dont le wiki a encore la page sous l'ancien nom
      (`[LegacyName]`) : appliquer la même règle (nom du wiki partout, refonte = portage en Full) ? Même objet renommé :
      The Reaper = The Old Reaper (aujourd'hui masqué en Full !), Calamitas' Brew = Flask of Brimstone, Chaotic Brick
      (Wall) = Scoria Brick (Wall), Aero Dynamite = Skynamite, Seashell Boomerang = Fishbone Boomerang, Conclave Crossfire
      = Conference Call. Refontes : Redtide Sword → Urchin Mace, Red Bow → Ghost Bracelet, Moonlight → Bakidon, Aeries →
      Arietes 41, Seabow → Reed Blowgun, Marnite Spear/Bayonet, Wulfrum Blade/Bow/Staff/armor → nouveaux objets Wulfrum.
      Liste complète : `python tools/calamity_renames.py` (dernière section).
