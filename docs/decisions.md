# Registre des décisions hors wiki

Tout ce qui, dans le mod, ne vient pas directement d'une page du wiki : choix faits quand le wiki est muet,
incohérent ou irréaliste, et contraintes techniques. **Toute nouvelle décision de ce genre doit être ajoutée ici.**

Marqueurs :
- **[validé]** : tranché ou validé avec l'utilisateur (date) ;
- **[choix]** : le wiki ne dit rien ou se contredit, décision prise par Claude, **à valider** en jeu ;
- **[technique]** : imposé par l'implémentation (tModLoader, Calamity), sans effet de design voulu.

Une page corrigée ou créée l'est directement dans `wiki/` (notre wiki, `docs/WIKI.md`) ; ce registre garde le pourquoi.

---

## 1. Modes de fidélité (`docs/MODES.md`)

- **[validé 21/10]** Un seul mod. Deux modes joueurs : **Full Fandom** et **Calamity++** (défaut). **Minimal** = Calamity++
  sans `Balance/`, option de debug seulement (« ni fidèle ni équilibré »).
- **[validé 21/10]** Réglage dans la config serveur, rechargement obligatoire ; chaque monde mémorise son mode.
- **[choix — écart à confirmer]** « Monde verrouillé » est implémenté comme un **avertissement** à l'entrée (message orange),
  pas comme un blocage : charger un monde dans l'autre mode reste possible. Seul le passage Full Fandom ↔ Calamity++
  avertit (l'équilibrage ne change que des nombres).
- **[validé 21/10]** Référence « abandon du wiki » = **Calamity 2.0.1.005** (commit `3ec8f553ed`, 30/12/2022) : des 29
  objets ajoutés juste avant, 19 ont une page ; des 77 ajoutés par 2.0.2.001, un seul.
- **[validé 21/10]** En Calamity++, on garde ce dont le wiki ne parle pas : recettes et butin du jeu absents du wiki,
  contenu Calamity d'après 2022. Seul Full Fandom les retire.
- **[validé 21/10]** Contenu retiré de Calamity depuis 2.0.1.005 et documenté sur le wiki : porté depuis le vieux code,
  présent dans tous les modes.
- **[choix]** Full Fandom « masque » le contenu récent : plus de recette, de butin, d'apparition naturelle, de vente.
  Un objet masqué déjà dans un inventaire reste utilisable. Une règle de butin n'est retirée que si **tout** ce qu'elle
  donne est masqué.
- **[choix]** Les PNJ masqués créés en combat (sbires de boss) ne sont jamais supprimés : un boss retravaillé peut en
  dépendre.
- **[choix]** Un objet récent n'est pas masqué si le wiki le connaît d'une façon ou d'une autre : page, redirection
  (« Aerospec Hat » → « Aerospec armor ») ou recette du wiki qui le produit (cas des objets de lore).
- **[validé 21/10]** Full Fandom masque aussi le contenu **plus ancien** (déjà là en 2.0.1.005) quand le wiki n'en parle
  **nulle part** : ni page, ni redirection, ni recette, ni simple mention dans le texte d'une page (tous espaces de noms,
  changelogs compris ; les références internes ne sont pas toujours des liens). Cité seulement en texte (ex. Acidwood
  Wall, dans un changelog) → gardé. Noms cherchés : nom actuel, nom de 2.0.1.005, renommages et alias du glossaire.
  Liste : `reports/baseline.md`, section « Antérieurs, cités nulle part ».
- **[technique]** Classes de base abstraites (BaseDye, HeldOnlyItem) ignorées : ce ne sont pas des objets.
- **[validé 21/10]** Calamity 2.2.2 est la version finale (développement arrêté) : cible figée.

## 2. Noms et vocabulaire (`tools/glossary.json`, `Localization/en-US/Mods.CalamityMod.hjson`)

- **[choix]** Renommages retenus après lecture de chaque page cible (`reports/renames.md`) : Scoria → Abyss Bar/Ore,
  Astrum Deus → Astral Deus, Brimstone Crags → Brimstone Crag, Pearl Shard → Conquest Fragment, Dark Spark → Cruel Jewel,
  Divine Geode → Divine Bar, Malachite → Garnet, Murasama → Graydron, Marked for Death → Marked, Nightwither → Moon Burnt,
  Grand Scale → Parched Scale, Pristine Fury → Pristine Annihilation, Uelibloom → Tarragon, Draconic Swarmer → Tempest
  Toddler, Sea Remains → Victide Bar, Wulfrum Metal Scrap → Wulfrum Bar.
- **[choix]** Faux renommages écartés (la redirection pointe vers une autre chose ou un regroupement) : Holy Flame →
  Divine Intervention, Hiveling → Hive, Star Swallower, Cosmilite Sconce → Sconce, Rage/Adrenaline Mode → Meter,
  Astral Chest → Biome Chest, Danny Devito → Trash Can, et les pages de regroupement (Beds, Chairs, armures...).
- **[choix]** Renommages trouvés sans redirection (le wiki a une page « nouvelle » qui décrit un objet officiel :
  même infobulle, même usage) : The Etomer → Triboluminescent Etomer, Martian Distress Remote → Martian Distress
  Beacon. Le sac de la Leviathan est connu du wiki sous « Treasure Bag (The Leviathan) » : simple alias (le renommage
  en jeu ne passe pas, le nom finit par une parenthèse).
- **[validé 21/10]** Ravager → **Meatmetal Sans** partout (boss, masque, trophée, relique, sac, lore). Un renommage décidé
  passe avant la protection ci-dessous.
- **[choix]** Vocabulaire appliqué à tous les textes de Calamity : rogue → spy, accessory → covering, post-Moon Lord →
  Godseeker Mode (mot entier, casse respectée).
- **[choix]** Un nom officiel qui a encore sa propre page n'est pas modifié : Hide of Astrum Deus, Uelibloom Brick (Wall),
  Realm Ravager. Idiomes gardés : « going rogue » et variantes.
- **[choix]** Les infobulles des objets officiels restent celles de Calamity (avec le vocabulaire ci-dessus) : les
  infobulles du wiki décrivent souvent un comportement de 2022 que le jeu n'a plus.
- **[choix]** Alias de noms quand le jeu et le wiki diffèrent : Sand → Sand Block, Vertebrae → Vertebra,
  Supreme Witch, Calamitas → Supreme Calamitas, Yharon, Dragon of Rebirth / Resplendent Phoenix → Jungle Dragon, Yharon.
  Sinon, les redirections du wiki servent d'alias.

## 3. Stats (`tools/gen_stats.py`, `tools/stats_decisions.json`)

- **[validé 21/10]** Valeur du wiki identique au code de Calamity 2.0.1.005 → **Full Fandom seulement** (ce n'est que
  l'ancien Calamity : Starfleet 68 → 3170 depuis). 858 valeurs.
- **[validé 21/10]** Valeur propre au wiki → tous les modes.
- **[validé 21/10]** Écarts extrêmes (> x5) propres au wiki, au cas par cas :
  - tous les modes : Desert Scourge (350 000 PV, « impossible avant le Hardmode, dans l'esprit du Fandom »),
    Anti-materiel Rifle, Clockwork Bow, Coral Spout, Halibut Cannon, Soma Prime, Voidragon, Grand Dad, Nanoblack Reaper,
    Shadecrystal Barrage, Venusian Trident, Cannonball Jellyfish, Great Sand Shark, Polterghast, THE LORDE,
    Ceaseless Void (0 de défense : wiki.gg aussi), Rusty Beacon Prototype et Slime Puppet Staff (0 dégât : normal
    d'après leurs pages), Temporal Umbrella (520 mana : atteignable, pas de plafond à 400 dans tModLoader) ;
  - Full Fandom seulement : Relic of Ruin (garder ses dégâts), Stormjaw Staff (0,1 rendrait ses étincelles inutiles),
    Exorcism (`use` 19).
  - Tout nouvel écart extrême reste en Full Fandom seulement et apparaît « à trancher » dans `reports/stats.md`.
- **[choix]** Seules les valeurs nettes sont lues ; les infobox à paliers (« Tier 1 / Tier 2 », « pre-Astrum Aureus »)
  sont ignorées. `{{dv}}` → valeur du mode normal.
- **[choix]** `critical` du wiki = `Item.crit` + 4 (le wiki affiche la chance totale, avec les 4 % de base ; vérifié sur
  27 objets). `use` = `useAnimation` ; `useTime` suit (égal s'il l'était, proportionnel sinon).
- **[choix]** Stats de combat (dégâts, vitesse, recul, critique, mana, vitesse de tir) seulement pour les types Weapon,
  Tool, Ammunition : les accessoires et blocs ne reçoivent que défense, rareté et prix.
- **[choix]** PNJ : vie et dégâts appliqués en **ratio** wiki / code (mode normal), pour garder la montée en difficulté
  de Calamity (Expert, Revengeance, Death) ; défense en valeur absolue.
- **[choix]** Rareté du wiki : 0-11 vanilla, 12 Turquoise, 13 Pure Green, 14 Cosmic Purple, 15 Burnished Auric,
  16 Hot Pink, 17 Calamity Red ; rareté inventée (« C2 »...) → 0. Prix d'achat = prix de vente du wiki × 5.

## 4. Recettes (`tools/gen_recipes.py`, `Common/WikiData/WikiRecipes.cs`)

- **[choix]** Une recette existante qui diffère du wiki est **modifiée sur place** (ses conditions et effets spéciaux
  de Calamity sont conservés) ; on choisit la recette existante la plus proche (ingrédients ou station en commun).
- **[validé 21/10]** Recettes du jeu absentes du wiki : conservées en Calamity++, désactivées en Full Fandom.
- **[choix]** Rien n'est changé si un ingrédient ou une station du wiki n'existe pas encore en jeu.
- **[choix]** Recettes vandalisées illisibles (« sqrt(-1) » Alloy Bars, « -1 ») : ignorées, le jeu garde la sienne.
- **[choix]** Recette en double sur deux pages du wiki : comptée une fois.
- **[choix]** « A/B » = au choix (groupe de recettes, réutilise un groupe existant s'il contient exactement ces objets).
- **[choix]** Stations : « Iron Anvil » = enclumes fer/plomb, « Mythril Anvil » = enclumes Hardmode, « Alchemy Table only »
  = table d'alchimie, « Placed Bottle » = bouteille posée, « By Hand » = sans station, « Water » = près de l'eau ;
  les autres par le nom de l'objet qui pose la station.
- **[choix]** Résultats vanilla exclus pour l'instant (le wiki ne liste souvent qu'une partie de leurs recettes).
- **[choix]** Shimmer : seules les pages « Shimmer » / « Shimmer Transmutation », et seulement si l'objet de départ ou
  d'arrivée vient de Calamity ou de ce mod. « Sanguine Shimmer » est une mécanique inventée, ignorée pour l'instant.

## 5. Système d'éléments (`Common/Elements/`, `Balance/elements.json`)

Source : `Elemental System.txt` (17 éléments, une phrase d'effet chacun ; résistant = 50 % des dégâts et immunité à
l'effet, vulnérable = 200 %). Tout le reste est un choix, réglable dans `Common/Elements/Element.cs`.

- **[choix]** Nature des effets : un **debuff** de 2 s par élément (sauf Wind et Bomb, qui agissent au coup). Noms tirés
  des verbes de la page, descriptions écrites par Claude :

  | Élément | Debuff | Effet (pré-HM / HM / après Moon Lord pour les dégâts par seconde) |
  |---|---|---|
  | Cold | Numbed | ralenti 20 %, dégâts de contact -10 % |
  | Fire | Ablaze | 6 / 20 / 60 par s, défense -10 % |
  | Wind | — | recul x1,5 au coup (« hurting them when they land » pas encore fait) |
  | Water | Waterlogged | 3 / 10 / 30 par s, ralenti 10 % |
  | Poison | Envenomed | 5 / 15 / 45 par s, dégâts de contact -10 % |
  | Acid | Corroded | 3 / 10 / 30 par s, ralenti 10 %, dégâts -10 %, défense -10 % |
  | Radioactive | Radiation Sickness | 8 / 25 / 80 par s, défense -15 %, pas de régénération |
  | Electricity | Shocked | 6 / 20 / 60 par s, ralenti 15 % |
  | Stab | Punctured | 4 / 12 / 40 par s, ralenti 10 % |
  | Slash | Lacerated | 8 / 25 / 80 par s |
  | Smash | Discombobulated | confusion vanilla, défense -15 % |
  | Bomb | — | critiques x2 en dégâts et x1,5 en chance ; +10 % par ennemi déjà touché dans le même tick (max x2) |
  | Magic | Hexed | 4 / 12 / 40 par s, dégâts de contact -10 % |
  | Dark | Incapacitated | 20 % de chances de rater le contact |
  | Holy | Smitten | 10 / 30 / 100 par s |
  | Unholy | Damned | 10 / 30 / 100 par s |
  | Machine | Malfunctioning | dégâts de contact -10 % ; et pour le joueur : +5 % dégâts et critique pendant l'usage |

- **[choix]** Debuffs d'éléments renommés (21/10) pour ne pas porter le nom d'un autre debuff : Chilled → **Numbed**
  (vanilla), Drowning → **Waterlogged** (mécanique vanilla), Irradiated → **Radiation Sickness** (Calamity),
  Cursed → **Damned** (vanilla, et l'effet est tout autre). À valider.
- **[choix]** Effets cumulés plafonnés : ralentissement 50 % (moitié sur les boss), affaiblissement 50 %, défense 50 %,
  ratés 50 %.
- **[choix]** Plusieurs éléments sur une arme : multiplicateurs multipliés (feu vulnérable + poison résistant = neutre),
  bornés entre x0,25 et x4.
- **[validé 21/10]** Full Fandom : tous les effets à chaque coup. Calamity++ : une chance par coup, **25 %** pour les
  dégâts sur la durée, **15 %** pour les contrôles (Cold, Smash, Dark, Machine) — valeurs proposées par Claude, à ajuster
  après les tests.
- **[validé 21/10]** Pas d'icône d'élément au-dessus des ennemis (trop chargé) ; **[choix]** à la place : particules propres
  à l'élément (glace, flammes, eau, poison, acide, vert radioactif, électricité, sang pour Stab/Slash, pierre pour
  Smash, rose pour Magic, ombre pour Dark, or pour Holy, rouge sombre pour Unholy, métal pour Machine) et teinte de
  l'ennemi de 35 % vers la couleur de l'élément (mélangée si plusieurs).
- **[validé 22/10]** Pas de texte en plus quand l'attaque est multipliée (« Vulnerable! »/« Resisted » jugé trop lourd à
  l'écran) : le chiffre de dégâts du coup change de couleur. **[validé 22/10]** jaune (255, 235, 70) = vulnérable, gris =
  résisté (l'orange se confondrait avec la couleur normale des dégâts).
  **[technique]** Le chiffre est retrouvé parmi les textes de combat tout juste créés (même nombre, sur l'ennemi) : rien
  ne change chez les autres joueurs, qui voient la couleur normale.
- **[choix]** Infobulle « Elements: ... » colorée, placée sous la ligne de dégâts.
- **[choix]** Les projectiles héritent des éléments de l'arme (ou du projectile parent).
- **[choix]** Affinités contradictoires (même élément résistant et vulnérable) ignorées ; valeurs hors des 17 éléments
  (« My Heart is Cold », « Better Magic », « Colder ») ignorées.
- **[choix]** Les éléments des armures (« Elements » des Coverings) ne sont pas encore gérés.

## 6. Matériaux générés (`tools/gen_items.py`)

- **[choix]** Portée : pages absentes de Calamity de type Crafting material / Bar / Soul, plus les « Block » qui servent
  d'ingrédient dans une recette (les barres sont des blocs sur le wiki). 218 objets.
- **[choix]** Barres et blocs **non posables** pour l'instant (pas de tuiles).
- **[choix]** Pile = champ `stack` (9999 par défaut) ; objet de recherche (Journey) : 25.
- **[choix]** Animation : frames identiques successives fusionnées, vitesse tirée du GIF du wiki ; les matériaux animés
  flottent au sol comme les âmes.
- **[choix]** Un matériau dont le nom existe déjà en vanilla n'est pas créé.
- **[validé 22/10]** **Yharim Bar géant au sol** (x16, d'après l'image « The item in-game » de sa page : ~480 px de large
  pour un sprite de 30), **dans les deux modes**, zone de ramassage comprise (hitbox x16). Taille normale dans
  l'inventaire. Données : `WORLD_SCALE` dans `tools/gen_items.py` → `worldScale` de `WikiData/items.json`.
  **[technique]** Un objet si grand qui tombe dans un tunnel étroit peut rester coincé dans les blocs.
- **[validé 21/10]** Sprite absent ou inutilisable → demandé à l'utilisateur (`docs/SPRITES.md`). Provisoire en attendant :
  « Mars Bars » (photo réduite à 90×70).

## 7. Butin (`tools/gen_loot.py`, `Common/WikiData/WikiLoot.cs`)

- **[choix]** Additif : seuls les objets que le PNJ ne lâche pas déjà sont ajoutés ; chances existantes inchangées.
  **[validé 21/10]** En Full Fandom, les drops du jeu absents de la page sont en plus retirés.
- **[choix]** Chance et quantité du mode normal. Drops avec une condition en note (« Post-Golem », « Tier 3 »,
  « Always on first kill ») : non appliqués pour l'instant (`reports/loot.md`).
- **[choix]** Boss : ajouts en mode normal seulement (en Expert, le butin passe par le sac, pas encore géré).
- **[choix]** « One of the following N items » : un objet au hasard parmi le groupe, sauf si le jeu en donne déjà un.
- **[choix]** Un seul PNJ par page, celui qui a déjà le plus de butin (vers, alias) ; titres sans nom (« ??? ») ignorés.

## 8. Contenu créé ou porté

### Zweihandromeda (`Content/Items/Weapons/Melee/`)
- **[validé 21/10]** « Back and forth pattern » : un coup vers le bas, le même vers le haut, en alternance.
- **[choix]** Coup orienté vers le curseur, arc de ~220° (±1,9 rad), accélération puis ralentissement ; épée tenue par le
  bas de la poignée (point (14, 216) du sprite), validé visuellement ; zone de dégâts = toute la lame (~306 px, 40 px
  de large) ; un ennemi n'est touché qu'une fois par coup ; traînée bleue.
- **[choix]** « Summons fallen stars from the sky on a successful attack » : 2 **Stratus Stars** par ennemi touché,
  demi-dégâts, qui tombent de 800 px au-dessus de la cible (±200 px), vitesse 22, traversent les blocs, Moon Burnt 3 s.
- **[technique]** Le swing vanilla place un sprite de 232×230 loin de la main : projectile tenu à la place.

### Night's Gaze (`Content/Legacy/`)
- **[choix]** Code de Calamity 2.0.1.005, valeurs du wiki là où elles diffèrent : `use` 19, vente 26 or, Moon Burnt 5 s
  sur la lance (l'ancien code utilisait le temps de vie restant du projectile).

### Meatmetal Sans (page « Ravager », `Common/NPCs/MeatmetalSans.cs`)
- **[validé 21/10]** 1 PV « à la Sans » dans tous les modes, avec sa mécanique.
- **[choix]** Version minimale : 23 PV (= 23 coups, 12 + 10 + 1 de la page), chaque coup retire 1 PV, esquive 0,5 s
  après chaque coup, défense 0, attaques (contact et projectiles) à 1 dégât + Poisoned 5 s. Pas encore fait : ignorer
  l'invincibilité du joueur, os, Gaster Blasters, écran noir, Handheld Tank (`docs/ROADMAP.md`).

### Textures du wiki sur Calamity (`Assets/Overrides/`)
- **[validé 21/10]** Icône du wiki pour Moon Burnt (au lieu de celle de Nightwither), y compris dans l'affichage de
  debuffs de Calamity au-dessus des boss.

## 9. Outils de debug (`Content/Debug/`, sans recette)

- **[choix]** Fandom Test Vial : applique 10 s le debuff choisi au joueur et à l'ennemi le plus proche (Moon Burnt,
  Marked, 15 statuts d'éléments).
- **[choix]** Fandom Test Dummy : 1 000 000 PV (sans mise à l'échelle de difficulté), immobile, taille x2, aucun dégât,
  neutre aux éléments, revient à fond sous 10 % de ses PV, affiche les debuffs de Calamity.
- **[choix]** Test Swords : 18 (une par élément + Moon Burnt), 10 dégâts, `use` 15, autoswing, couleur de l'élément ;
  suivent les règles du mode (chances en Calamity++).
- **[technique]** Sprites vanilla réutilisés (bouteille d'eau, mannequin, épée de cuivre) : pas de sprite à faire.

## 10. Notre wiki (`wiki/`, `docs/WIKI.md`)

- **[validé 21/10]** Copie complète du wiki Fandom dans `wiki/`, éditée au fil du projet ; le dump d'origine reste la
  référence du Full Fandom. Remplace `wiki_edits/`.
- **[validé 21/10]** Notre wiki décrit les deux modes, avec `{{mode|Full Fandom|Calamity++}}` quand ils diffèrent.
- **[choix]** Pas copiés : discussions et pages utilisateur. Les 18 pages écrasées par Windows remplacent leur
  redirection homonyme (la redirection à la casse près est perdue).
- **[choix]** Un `{{mode}}` sur une page passe avant les règles automatiques des stats.
- **[choix]** Les pages des debuffs d'éléments et la section « Status effects » sont générées depuis le code ; les
  autres pages de notre wiki s'éditent à la main.
- **[choix]** Une stat partagée par plusieurs classes d'une même page n'est écrite en `{{mode}}` que si la règle vaut
  pour toutes ces classes (sinon la règle continue de s'appliquer, cas de Polterghast).
- **[choix]** Dans `{{mode}}`, une valeur vide veut dire « absent dans ce mode » (ingrédient, recette entière, drop,
  élément). Une page dont les deux vues sont identiques produit une seule entrée de données, valable dans les deux modes.
