# Différences de fonctionnement : Fandom / Calamity actuel

Revue de la nuit du 28 au 29/10 : pour chaque objet officiel ou renommé, le fonctionnement décrit par le Fandom
(`fandom_dump_txt/`, la spécification du Full Fandom) est comparé à celui du wiki officiel (calamitymod.wiki.gg,
Calamity 2.2.2), puis à toutes les versions passées du code de Calamity, pour savoir si un écart est **inventé** par le
Fandom ou si c'est simplement **l'ancienne version** de l'objet.

Détail objet par objet (infobulle Fandom, infobulle officielle, version du code reconnue, preuves) :
`reports/wikigg_diff.md`. Ce document-ci en tire les conclusions pour la suite du développement.

## Méthode

- `tools/fetch_wikigg.py` : télécharge le wiki officiel dans `wikigg_dump/` (2 886 pages, 6 517 redirections, local).
- `tools/old_tooltips.py` : infobulles de chaque objet dans l'historique git de Calamity, un instantané par mois de
  décembre 2018 (début du dépôt) à 2.0.1.005 ; `tools/old_strings.py` : toutes les phrases du code (tous fichiers,
  21 840), avec leur date d'apparition et leur fichier.
- `tools/compare_wikigg.py` : pour les 1 324 pages d'objets officiels ou renommés, compare les infobulles ligne par
  ligne (vocabulaire du Fandom ramené à celui de Calamity : Spy, Covering, Moon Burnt...), les bonus de set d'armure
  et le type d'objet. Une ligne du Fandom est :
  - **actuelle** si le code actuel ou le wiki officiel la contient ;
  - **ancienne** si une version passée de l'objet la contenait (avec la date) ;
  - **introuvable** sinon, ou **recopiée** si elle vient du fichier d'un autre objet.
- `tools/diff_review.json` : revue à la main de chaque cas introuvable ou douteux, avec les preuves (notes de version
  du wiki officiel `wikigg_dump/<version>.txt`, code de 2.0.1.005). Les 66 cas « à vérifier » et les 22 sets d'armure
  sont tous tranchés.

Relancer : `python tools/old_tooltips.py`, `python tools/old_strings.py`, `python tools/fetch_wikigg.py` (réseau),
`python tools/compare_wikigg.py`.

## Chiffres

| Verdict | Objets | Sens |
|---|---|---|
| Refonte | 268 | le Fandom décrit une ancienne version, très différente de l'objet actuel |
| Modifiée | 280 | ancienne version, en partie différente |
| Reformulée / formulation | 126 | même fonctionnement, ancien texte |
| Mélange de versions | 33 | chaque ligne vient d'une version passée, sans qu'une seule les ait toutes (surtout les alcools d'avant leur refonte) |
| Ancienne version (revue) | 13 | confirmée à la main |
| **Inventée** | **27** | le Fandom décrit un fonctionnement qui n'a jamais existé |
| **En partie inventée** | **24** | ancienne version + lignes inventées |
| Homonyme | 2 | objet inventé, Calamity a créé plus tard un objet différent du même nom |
| Addon | 1 | effet d'un autre mod (Calamity: Community Remix) |
| Ambiance | 9 | seule une phrase d'ambiance change (blagues, paroles de chansons) |
| Sets d'armure | 22 | 21 anciennes versions, 1 reformulation (Gem Tech, réécrit en pavé), aucune invention |

**Conclusion principale** : le Fandom invente peu (51 objets sur 785 écarts). L'essentiel des écarts de fonctionnement
(≈ 594) vient de ce que Calamity a refait ses objets, et dans 483 cas **après** 2.0.1.005 : le Fandom décrit alors exactement la version
2.0.1.005, qu'on peut porter en Full Fandom comme le Galileo Gladius et le Cosmic Shiv (`tools/legacy_extract.py`,
`Common/Legacy/FullFandomLegacyWeapons.cs`).

## À traiter (découvertes de la revue)

1. **Fusion du 28/10 à revoir** : Starfall, Seabound Staff et Effluvium Bow n'étaient pas identiques aux armes
   actuelles. Calamity les a renommées (Star Shower, Brittle Star Staff, Hoarfrost Bow) **puis refaites** après
   2.0.1.005 (Brittle Star Staff : étoiles qui se brisent, orbite qui donne de la défense ; Star Shower : étoiles
   tombées du ciel ; Hoarfrost Bow : brumes qui se divisent). Le Fandom décrit les versions 2.0.1.005, que nos portages
   supprimés reproduisaient. En Full Fandom, il faudrait soit les rétablir, soit appliquer l'ancien fonctionnement à
   l'arme de Calamity (comme le Galileo).
2. **Disgusting Meat** et **Ha-pu Fruit** : objets inventés par le Fandom (un matériau des Tainted Potions ; une
   amélioration permanente de la Rage), que l'index prend pour des objets officiels parce que Calamity a créé plus tard
   des objets différents du même nom. En Full Fandom, ce devraient être les objets du Fandom.
3. **Divine Intervention** : épée entièrement inventée (lâchée par les Meteor Heads), que l'index relie à tort au
   projectile HolyFlame (statut « renamed ») : elle ne sera jamais créée tant que ce statut reste.
4. **Dragonfolly** : le Fandom l'a renommée « The Magnificent Lightning Avian » (page du boss, objets « Magnificent
   Lightning Avian Mask/Trophy », Exotic Pheromones) ; ni le glossaire ni `reports/renames.md` ne le connaissent.
5. **Inventions qui changent la nature d'un objet** (arme ↔ potion ↔ accessoire...) : The Syringe et Wulfrum
   Screwdriver (potions), Death Whistle (arme), Seafood et Forgotten Dragon Egg (aliments), Geyser Shell (munition),
   Warloks' Moon Fist, Baroclaw, Momentum Capacitor (autres effets)... En Full Fandom, ce sont des objets à refaire
   selon la page (liste complète ci-dessous).
6. **Déjà faits** : Galileo Gladius, Cosmic Shiv et Spore Knife (anciennes versions en Full Fandom), Shortstrike
   (mécanique inventée, présente dans 6 infobulles).

## Buffs et debuffs officiels (33 pages)

Comparés à la main (champ `effects` du Fandom, `playereffects` / `enemyeffects` du wiki officiel) : 13 identiques
(Alcohol Poisoning, Blood Frenzy, Daybroken, Eutrophication, NO U, Silva Immunity, Tarragon Cloak...) ; les autres
diffèrent surtout par les **valeurs** (dégâts par seconde, défense, vitesse) : Calamity a rééquilibré ses debuffs après
l'abandon du wiki (en particulier en 2.1.0). Les écarts de nature sont aussi d'anciennes versions, confirmées par les
notes de version : Irradiated (donnait +5 % de dégâts et du recul aux serviteurs avant son rééquilibrage), Burning
Blood (« now only deals damage to the player and no longer has any other effects »), Extreme Gravity (réduction du temps
de vol modifiée plusieurs fois), Plague, God Slayer Inferno, Vulnerability Hex, Whispering Death, Marked (le Marked for
Death actuel multiplie les dégâts reçus). **Aucune invention** parmi les buffs officiels. Les valeurs du Fandom
seraient à appliquer en Full Fandom si le mod règle un jour les debuffs (aujourd'hui : seulement ceux des éléments).

## Pages « new » : anciens objets de Calamity ou inventions ?

Même question pour les 1 179 pages d'objets que l'index classe « new » (absents de Calamity 2.2.2) : **212 portent le nom
d'un ancien objet de Calamity** (historique depuis décembre 2018), ce ne sont donc pas des inventions mais des objets
retirés que le Fandom a gardés :

- 95 existaient encore en 2.0.1.005 (`python tools/legacy_extract.py --list`, première partie) ; 31 sont déjà portés ;
- **117 avaient été retirés avant 2.0.1.005** (beaucoup pendant le passage à Terraria 1.4, en 2022) ; 22 sont déjà
  portés (Aftershock, Terra-cotta, Spore Knife...). `legacy_extract.py` les connaît maintenant : `--list` les affiche
  (seconde partie, avec la date de retrait) et `legacy_extract.py Classe` extrait le code de la dernière version
  mensuelle où l'objet existait.

Leur infobulle a été comparée de la même façon aux versions de l'objet retiré (`reports/wikigg_diff.md`, section
« Objets retirés de Calamity ») : 201 sur 211 décrivent fidèlement l'ancien objet ; 10 y ajoutent des lignes inventées
(8 lignes Shortflurry, la parade de l'Exo Gladius, la « Void Barrier » de l'Arcanum of the Void).

Les ≈ 970 autres pages d'objets « new » sont des inventions du Fandom (ou des objets d'addons, ou d'anciens objets
renommés avant leur retrait, que la recherche par nom ne voit pas).

## Limites

- La comparaison passe par les infobulles et les bonus de set : un comportement inventé seulement dans le texte d'une
  page (sans ligne d'infobulle) peut avoir échappé, sauf s'il change le type de l'objet (contrôlé aussi).
- Le dépôt de Calamity commence en novembre 2018 : une version plus ancienne n'est vérifiable que par les notes de
  version du wiki officiel.
- Les PNJ et les boss (IA, PV, attaques) ne sont pas couverts : ici, uniquement les objets.
- Une ligne « invérifiable » dans les notes de revue n'est pas forcément inventée : rien ne la confirme ni ne l'infirme.

## Objets inventés ou en partie inventés

### Inventés (27)

| Objet | Constat |
|---|---|
| **Abyss Shell Fossil** | Familier escargot (réel, ajouté en 2.0.2.001) ; inventé : dans l'Abysse, il attire un Adult Eidolon Wyrm qui le dévore puis attaque le joueur. Le vrai réduit au contraire la détection par les créatures de l'Abysse. |
| **Alchemical Flask** | « Inflige la Plague au joueur, +2 % de dégâts » : aucune version. Depuis son ajout (1.1.6.15), les attaques infligent la Plague aux ennemis (seekers, immunité ou réduction de la Plague selon les versions) ; renommée Alchemical Decanter en 2.1.0. |
| **Baroclaw** | Pinces invoquées par une touche qui attrapent un ennemi 5 s : jamais. Ajouté en 2.0.3.002 avec des dégâts renvoyés (thorns), Armor Crunch puis Crush Depth (2.0.3.003). « 10 defense » : jamais non plus (dégâts et réduction de dégâts). |
| **Clockwork Bow** | Tire des rouages, cumule les dégâts et lance un « Precision Blast » après 50 coups : jamais. Avant 1.5.1.001 : 10 à 18 flèches tombées du ciel ; depuis : arc qui charge jusqu'à six carreaux. |
| **Death Whistle** | Arme qui tire des notes de musique (Temporal Sadness, Confused) : jamais. Objet d'invocation du Ravager depuis toujours (ancien Ancient Medallion, renommé en 1.4.5.001). |
| **Dimensional Soul Artifact** | « Crée un orbital de Cosmilite » (purement visuel) : jamais. L'objet a toujours augmenté les dégâts en contrepartie des dégâts subis (1.3.4.103, 1.4.1.001...) ; renommé Crushing Ego en 2.2.0. |
| **Divine Intervention** | Épée pré-Hardmode lâchée par les Meteor Heads (étincelles, têtes de météore, buff de flamme sacrée, lancer en furtif) : n'a jamais existé dans Calamity. L'index la relie à tort au projectile HolyFlame (statut « renamed ») : c'est un objet entièrement nouveau à créer. |
| **Eye of the Storm** | Invoque une Aether Valkyrie amie (et serait lâché par l'Aether Valkyrie, ennemi inventé) : jamais. Invoque un Cloud Elemental dans toutes les versions (arme d'invocation, puis accessoire depuis 1.1.7.701). |
| **Forgotten Dragon Egg** | Aliment qui donne Exquisitely Stuffed jusqu'à la mort : jamais ; c'est l'objet du familier Akato depuis son ajout (1.4.2.101). |
| **Genesis** | Le Fandom en fait un objet d'invocation non consommable de « Purgatory Arbiter, Umbrus » (boss inventé), lâché par « Vision of the Tyrant » (inventé). Dans Calamity, Genesis est une arme à lasers (ancienne « Genisis », 2019 : rayon en Y et lasers). |
| **Geyser Shell** | Munition (balles) qui libère des bébés Flak Crab : jamais ; ajouté en 1.4.4.001 comme objet de familier (un petit flak hermit). |
| **Momentum Capacitor** | Augmente sans cesse la vitesse des serviteurs, vendu par le Bandit : jamais. Avant 2.0.1.001, accessoire qui dépensait la furtivité pour un champ qui accélère les projectiles rogue ; depuis, il augmente la vitesse du joueur. |
| **Nanoblack Reaper** | Charge de 10 s qui lâche 75 lames par seconde, soin rapide quand elle n'est pas chargée, 575 lames en furtif : jamais. Avant 2.1.2, elle lançait des faux à tête chercheuse qui créaient des nanolames (clones en furtif depuis 1.5.1.001). |
| **Profaned Core** | Condition inventée : un Core dans 40 cases différentes de l'inventaire pour invoquer Providence. Le vrai s'utilise dans le Hallow ou l'Underworld (consommable puis non consommable dès 1.2.3.001). |
| **Quiver of Nihility** | Ajouté en 2.0.1.001 avec ses quatre champs de vide (code de 2022-08) ; flèches d'ombre à tête chercheuse, particules de vide et +30 % de dégâts des flèches : jamais. Phrase d'ambiance réécrite (« Show me the night sky »). |
| **Relic of Ruin** | Flèches profanées qui réduisent la défense : jamais. Éventail de lames de sable dans toutes les versions (16 puis 12 projectiles, tête chercheuse depuis 1.4.2.105). |
| **Seafood** | Le Fandom en fait un aliment (Well Fed 15 min, ligne recopiée de la Delicious Meat de 2018). Objet d'invocation de l'Aquatic Scourge dans toutes les versions. |
| **The Syringe** | Le Fandom en fait une potion (buff « Plagued Wrath » : dégâts, critique, vitesse, contreparties). Arme spy (seringue qui éclate en verre et cendres) depuis son ajout (1.4.2.106). |
| **Tracking Disk** | Disque qui tourne autour d'un ennemi et se casse au bout de 4 coups : jamais. Disque qui tire des lasers depuis son ajout (1.4.5.001), renommé P-LSR: Aerial Tracker en 2.1.0. |
| **Warloks' Moon Fist** | Le Fandom en fait un accessoire de mêlée (Nightwither, 10 % de mort instantanée, +60 % de dégâts true melee...), assemblé à partir de lignes d'autres accessoires. Arme d'invocation (poing céleste) depuis son ajout (2.0.1.002). |
| **Wulfrum Acrobatics Pack** | Saut infini alimenté par des Energy Orbs (objet inventé) avec une fenêtre de plus en plus étroite : jamais. Ajouté en 2.0.1.001 comme grappin-fronde, qu'il est toujours. |
| **Wulfrum Digging Turtle** | Arme qui lance une tortue explosive contre les ennemis (clic droit : leurre au sol, 4 au plus) : jamais. Outil qui creuse un tunnel depuis son ajout (2.0.1.001). |
| **Wulfrum Lure** | Page vandalisée : « Wolf Ram Fishing Pole », canne à pêche rechargée par un « Blood orb ». Le vrai (2.0.1.001) consomme une batterie / Energy Core pour attirer les automates Wulfrum. |
| **Wulfrum Scaffold Kit** | Propulse le joueur vers le haut en posant 40 blocs sous lui : jamais. Pose des échafaudages temporaires depuis son ajout (2.0.1.001), 40 blocs par ferraille. |
| **Wulfrum Screwdriver** | Le Fandom en fait une potion (debuff : -5 régénération, +1 perforation à tous les projectiles). Dans Calamity, c'est une épée courte (l'ancienne Wulfrum Blade, renommée et refaite en 2.0.1.001), qui lance des vis. |
| **Wulfrum Treasure Pinger** | Meuble qui consomme des Energy Orbs (objet inventé) pour marquer les coffres : jamais. Détecteur de minerais qui finit par casser depuis son ajout (2.0.1.001). |
| **Yateveo Bloom** | Lance qui frappe le sol et fait pousser des roses qui explosent en pétales (Poisoned, Venom) : jamais. Fléau de roses et estoc de lance fleurie depuis son ajout (1.4.2.104). |

### En partie inventés (24)

| Objet | Constat |
|---|---|
| **Awakening** | Infobulle réelle (objet de lore du Starter Bag, 2.0.2.001). Inventé par la page : un clic droit dans l'inventaire invoque « The Calamity » (boss inventé), objet non renouvelable. |
| **Bouncing Eyeball** | Œil qui rebondit, attaques furtives : réel. Inventé : « critique doublé pendant une Blood Moon » ; c'est le recul qui augmente. |
| **Calamity** | « FPS réduits de 50 %, mode matrix » : blague inventée. Le reste (curseur enflammé) est l'infobulle de 2021. |
| **Cosmic Shiv** | Ancienne version (rayon cosmique, barrage et pluie d'étoiles : 2020 à 2.0.1.005, portée en Full Fandom). Ligne Shortflurry : mécanique inventée par le Fandom (page Shortstrike), déjà implémentée dans le mod. |
| **Dark God's Sheath** | +20 furtivité max : ancienne version (1.4.3.002 à 2.0.1.002, 10 ensuite). Inventés : +20 % de critique spy dans l'espace, météores sombres sur les attaques furtives, vitesse des projectiles doublée. Renommé Dark Matter Sheath (2.0.3.003). |
| **Deadshot Brooch** | +10 % de dégâts à distance, +5 % de critique, 20 % de munitions économisées : ancienne version. La ligne « 5 increased defense, 2 increased life regen, and 15% increased pick speed » est recopiée de la Daedalus Emblem (code de 2020). |
| **Elemental Shiv** | Ancienne version de l'arme. Ligne Shortflurry : mécanique inventée par le Fandom (page Shortstrike), déjà implémentée dans le mod. |
| **Exoblade** | Explosion glacée quand un ennemi presque mort est touché : ancienne version (DraedonsExoblade, 2018 à 2022-07). Inventé : « une gerbe de rayons exo aux effets différents selon leur couleur » (rayons des composants, Terratome...) ; l'ancienne arme tirait un rayon exo et des comètes. Refaite en 2022-08 (rayons à tête chercheuse, ruée). |
| **Exotic Pheromones** | Infobulle : ancienne version réelle (invoque la Dragonfolly dans la Jungle, non consommable). Inventé par la page : c'est une fusée éclairante à tirer avec un Flare Gun dans le Désert, et le boss s'appelle « The Magnificent Lightning Avian » (renommage du Fandom absent du glossaire). |
| **Galileo Gladius** | Ancienne version (croissant de lune, planètes : 2020 à 2.0.1.005, portée en Full Fandom). Ligne Shortflurry : mécanique inventée par le Fandom (page Shortstrike), déjà implémentée dans le mod. |
| **Grand Gelatin** | Ancienne version (avant la refonte 2.0.3.002 : +20 vie et mana, régénération à l'arrêt). « 5 % reduced stealth cost » inventé : vient de la Mirage Jelly, ingrédient inventé par le Fandom. |
| **Lucrecia** | Ancienne version (chaîne d'ADN, immunité en frappant : avant 2.0.4.001 ; aujourd'hui éclat de lumière noire et énergie). Ligne Shortflurry : mécanique inventée par le Fandom (page Shortstrike), déjà implémentée dans le mod. |
| **Marksman Bow** | Trois flèches à la fois : réel. Inventé : flèches en bois changées en « Marksman Arrows » qui ricochent sur des pièces ; elles deviennent des Jester's Arrows dans toutes les versions (ajout en 1.4.5.001). |
| **Reaper Tooth Necklace** | +15 % de dégâts et +15 de pénétration d'armure : ancienne version (1.5.1.001 à 2.1.0 ; 20 % et 30 aujourd'hui). Inventés : le requin Reaper invoqué, dirigeable au curseur, et « toutes les stats +15 % » de la page. |
| **Sand Shark Tooth Necklace** | +10 de pénétration d'armure : réel (toutes versions). Inventés : le requin des sables invoqué, « Increases all stats by 10% » (6 % de dégâts jusqu'à 2.1.0, 10 % ensuite), la phrase « You're the alpha now ». |
| **Slime Puppet Staff** | Boules de slime qui suivent le curseur sans prendre de place : ancienne version. Inventé : « les boules se relient entre elles quand elles sont proches ». |
| **Spectral Veil** | +10 % de vitesse et furtivité +20 % en mouvement : ancienne version (1.5.0.001, vitesse retirée en 1.5.1.003). Inventée : l'esquive passive avec 70 s de recharge partagée ; l'objet a toujours été une téléportation sur touche qui rend invulnérable (1.4.2.109). |
| **Spore Knife** | Épée courte de mêlée retirée en juin 2022 (nuages de spores), portée en Full Fandom ; refaite depuis en couteau de lancer rogue. Ligne Shortflurry : mécanique inventée par le Fandom (page Shortstrike), déjà implémentée dans le mod. |
| **Submarine Shocker** | Ancienne version de l'arme. Ligne Shortflurry : mécanique inventée par le Fandom (page Shortstrike), déjà implémentée dans le mod. |
| **Terminus** | Lancement du Boss Rush : infobulle de 2022. Inventés : le menu à deux choix, le clic droit qui invoque « Vision of the Tyrant » (boss inventé), l'obtention en battant La Ruga (boss inventé). |
| **The Absorber** | Ancienne version (effets de ses composants, +20 vie et mana, régénération à l'arrêt, épines... avant la refonte de 2.0.3). Inventé : « 3 % reduced stealth costs », venu de la Mirage Jelly inventée. |
| **The Amalgam** | Surtout l'ancienne version (avant 1.5.0.001 : effets hérités du Fungal Clump, de la Leviathan Ambergris et du Void of Extinction ; aura, durées de buffs). Introuvables dans toute version : « la vie draine contre un gros bonus de dégâts », la traînée de spores/nuages/abeilles/saumure, « 50 % des stats aux équipiers » (la page dit 7,5 % de dégâts à l'équipe). |
| **The Sponge** | Ancienne version (stats de 1.5.0.001 : 20 défense, +30 vie et mana, 6,25 % de soin...). Inventés : la résurrection dans une coquille 3 s (1 min de recharge) et le bouclier d'énergie qui encaisse un coup (la barrière de 120 PV n'arrive qu'avec la refonte 2.0.3). |
| **Violence** | Fourche qui reste près du curseur et déchiquette : ancienne version (1.5.0.001 à 2.1.0, avant le lancer au clic droit). Inventés : les « Violence Points » à dépenser en attaques puissantes. |

### Homonymes (2)

| Objet | Constat |
|---|---|
| **Disgusting Meat** | Le Fandom décrit un matériau inventé (lâché par les zombies, l'Eater of Worlds, la Dragonfolly...) qui sert aux « Tainted Potions » inventées. Calamity a ajouté en 2.1.0 un objet du même nom qui retire les améliorations du joueur : en Full Fandom, ce devrait être le matériau du wiki, pas l'objet de Calamity. |
| **Ha-pu Fruit** | Le Fandom en fait une amélioration permanente (+5 % de dégâts de la Rage) obtenue en jetant dans le Shimmer des objets inventés (Mushroom Plasma Root, Infernal Blood, Red Lightning Container). Calamity a ajouté en 2.0.3.003 un Ha-pu Fruit différent (« A fruit that calms your nerves »). |

### Addon (1)

| Objet | Constat |
|---|---|
| **Eidolon Staff** | Tir de lumière ancienne et d'amas de glace : ancienne version (2019). Le Juvenile Eidolon Wyrm invoqué (4 emplacements, amas de glace au clic droit) vient de Calamity: Community Remix, cité dans l'infobulle. |

## Anciennes versions (refontes et modifications), par catégorie

Candidats au portage en Full Fandom (comme le Galileo Gladius). Liste complète avec les infobulles et les notes de version :
`reports/wikigg_diff.md`.


### Refontes, refaites après 2.0.1.005 (le Fandom décrit la version 2.0.1.005, portable avec legacy_extract.py) : 235

- **Accessoires** (57) : Abyssal Amulet, Ancient Fossil, Anechoic Plating, Aquatic Heart, Archaic Powder, Asgard's Valor, Asgardian Aegis, Blazing Core, Blood Pact, Bloodflare Core, Bloom Stone, Chaos Stone, Core of the Blood God, Corrosive Spine, Crown Jewel, Dark Sun Ring, Deep Diver, Deific Amulet, Feather Crown, Flame-Licked Shell, Flesh Totem, Fungal Symbiote, Giant Shell, Giant Tortoise Shell, Gladiator's Locket, Godly Soul Artifact, Gravistar Sabaton, Hallowed Rune, Hide of Astrum Deus, Honey Dew, Iron Boots, Laudanum, Leviathan Ambergris, Life Jelly, Living Dew, Lumenous Amulet, Luxor's Gift, Mana Jelly, Mana Polarizer, Moonstone Crown, Necklace of Vexation, Ocean Crest, Ornate Shield, Rampart of Deities, Regenator, Rotten Brain, Spirit Glyph, Statis' Ninja Belt, Statis' Void Sash, The Community, The Evolution, The First Shadowflame, The Transformer, Toxic Heart, Trinket of Chi, Unstable Granite Core, Yharim's Insignia
- **Distance** (33) : Animosity, Astreal Defeat, Aurora Blazer, Barinade, Cleansing Blaze, Crackshot Colt, Deathwind, Dragon's Breath, Elemental Eruption, Fetid Emesis, Firestorm Cannon, Flak Kraken, Flak Toxicannon, God's Bellows, Hellborn, Hydra, Leviatitan, Magna Cannon, Meowthrower, Midas Prime, Nullification Pistol, Overloaded Blaster, Pearl God, Photoviscerator, Polaris Parrotfish, Seadragon, Shadethrower, Spark Spreader, Speed Blaster, Starmada, Sulphuric Acid Cannon, Svantechnical, Terra Flameburster
- **Mêlée** (31) : Abyss Blade, Basher, Crescent Moon, Depth Crusher, Devil's Devastation, Evil Smasher, Excelsus, Fallen Paladin's Hammer, Feralthorn Claymore, Forbidden Oathblade, Galaxy Smasher, Gilded Proboscis, Grand Dad, Greatsword of Blah, Greatsword of Judgement, Greentide, Hellkite, Insidious Impaler, Lacerator, Mantis Claws, Mirror Blade, Neptune's Bounty, Oblivion, Pandemic, Solar Flare, The Mutilator, The Obliterator, True Forbidden Oathblade, True Tyrant's Ultisword, Tumbleweed, Tyrant Yharim's Ultisword
- **Rogue (Spy)** (27) : Blazing Star, Brackish Flask, Cinquedea, Contaminated Bile, Cranium Smasher, Deep Wounder, Duststorm in a Bottle, Dynamic Pursuer, Eclipse's Fall, Elephant Killer, Equanimity, Exorcism, Glaive, Meteor Fist, Molten Amputator, Phantasmal Ruin, Profaned Partisan, Scourge of the Desert, Sealed Singularity, Shard of Antumbra, Shock Grenade, Slick Cane, Spear of Destiny, Star of Destruction, Stormfront Razor, Supernova, Toxicant Twister
- **Magie** (20) : Aetherflux Cannon, Apoctosis Array, Burning Sea, Deathhail Staff, Downpour, Ion Blaster, Phantasmal Fury, Plasma Rifle, Primordial Ancient, Primordial Earth, Purge Guzzler, SHPC, Sanguine Flare, Shadecrystal Barrage, Shadowbolt Staff, Storm Spray, Thunderstorm, Viscera, Vitriolic Viper, Wingman
- **Munitions** (11) : Acid Round, Bloodfire Arrow, Flash Round, God Slayer Slug, Holy Fire Bullet, Hyperius Bullet, Icicle Arrow, Icy Bullet, Terra Arrow, Terra Bullet, Vanquisher Arrow
- **Objets à poser** (10) : Ancient Altar, Auric Ore, Botanic Planter, Corruption Effigy, Crimson Effigy, Effulgent Manipulator, Eutrophic Shelf, Monolith Amalgam, Rock, Static Refiner
- **Potions** (9) : Astral Injection, Bloody Mary, Bounding Potion, Calcium Potion, Caribbean Rum, Fireball, Screwdriver, Soaring Potion, Tequila
- **Invocation** (9) : Energy Staff, Harvest Staff, Plantation Staff, Saros Possession, Shellfish Staff, Sirius, Spikecrag Staff, Staff of the Mechworm, Void Concentration Staff
- **Arsenal de Draedon** (7) : Frequency Manipulator, Gauss Rifle, Matter Modulator, Plasma Caster, Pulse Rifle, Pulse Turret Remote, Snake Eyes
- **Pêche** (5) : Bloodfin, Chaotic Fish, Fish of Eleum, Sunbeam Fish, Ursa Sergeant
- **Ailes** (4) : Celestial Tracers, Drew's Wings, Seraph Tracers, Soul of Cryogen
- **Invocation de boss** (3) : Eye of Desolation, Jungle Dragon Egg, SYNTAX ERROR
- **Outils** (3) : Crystyl Crusher, Relic of Convergence, Relic of Resilience
- **Sans classe** (3) : Coldheart Icicle, Relic of Deliverance, Stratus Sphere
- **Buffs** (1) : Star Swallower Containment Unit
- **Montures** (1) : Onyx Excavator Key
- **Familiers** (1) : Thief's Dime

### Refontes, déjà différentes en 2.0.1.005 (le Fandom décrit une version plus ancienne, date entre parenthèses) : 33

- **Accessoires** (17) : Abaddon (2021-09), Alluring Bait (2019-12), Amalgamated Brain (2020-12), Ambrosial Ampoule (2021-10), Bloody Worm Tooth (2018-12), Celestial Jewel (2018-12), Corrupt Flask (2018-12), Crimson Flask (2018-12), Daawnlight Spirit Origin (2021-12), Draedon's Heart (2021-10), Profaned Soul Artifact (2020-04), Raider's Talisman (2019-02), Rotten Dogtooth (2021-07), Rover Drive (2022-02), Sand Cloak (2020-01), Void of Calamity (2020-12), Void of Extinction (2019-04)
- **Potions** (4) : Evergreen Gin (2022-02), Shadow Potion (2020-06), Tequila Sunrise (2021-01), Whiskey (2021-01)
- **Distance** (4) : Blissful Bombardier (2018-12), Opal Striker (2018-12), Scorpio (2019-08), Titanium Railgun (2019-08)
- **Ailes** (2) : Hadarian Wings (2021-09), Skyline Wings (2021-09)
- **Mêlée** (2) : Bladecrest Oathsword (2019-08), Old Lord Oathsword (2019-08)
- **Magie** (1) : Shaderain Staff (2018-12)
- **Rogue (Spy)** (1) : Mythril Knife (2020-06)
- **Invocation** (1) : Cnidarian (2020-12)
- **Sans classe** (1) : Claret Cannon (2021-03)

### Modifiées en partie, refaites après 2.0.1.005 (le Fandom décrit la version 2.0.1.005, portable avec legacy_extract.py) : 248

- **Accessoires** (39) : Abyssal Diving Suit, Abyssal Mirror, Aero Stone, Affliction, Angel Treads, Badge of Bravery, Bloody Worm Scarf, Depths Charm, Eclipse Mirror, Eldritch Soul Artifact, Elemental Gauntlet, Elemental in a Bottle, Enchanted Pearl, Ethereal Extorter, Ethereal Talisman, Frost Barrier, Giant Pearl, Glove of Precision, Glove of Recklessness, Harpy Ring, Heart of the Elements, Jelly-Charged Battery, Nebulous Core, Nuclear Rod, Radiant Ooze, Rare Elemental in a Bottle, Rose Stone, Shattered Community, Sigil of Calamitas, Silencing Sheath, Starbuster Core, Statis' Blessing, Statis' Curse, Supreme Bait Tackle Box Fishing Station, The Camper, Vampiric Talisman, Venerated Locket, Voltaic Jelly, Yharim's Gift
- **Mêlée** (38) : Air Spinner, Aorta, Astral Pike, Avalanche, Baleful Harvester, Ball O' Fugu, Brimlance, Brimstone Sword, Caustic Edge, Darklight Greatsword, Devil's Sunrise, Dragon Rage, Earthen Pike, Entropic Claymore, Fault Line, Galaxia, Gelitic Blade, Graydron, Holy Collider, Prismatic Breaker, Pwnagehammer, Rem's Revenge, Riptide, Roxcalibur, Seeking Scorcher, Shimmerspark, Stellar Contempt, Swordsplosion, Terra Lance, Terratomere, The Burning Sky, The God's Gambit, The Microwave, Triactis' True Paladinian Mage-Hammer of Might, True Caustic Edge, Typhon's Greed, Urchin Mace, Yin-Yo
- **Rogue (Spy)** (33) : Adamantite Throwing Axe, Ballistic Poison Bomb, Bloodsoaked Crasher, Burning Strife, Corpus Avertor, Cosmic Kunai, Cursed Dagger, Eradicator, Fantasy Talisman, Frostcrush Valari, Frosty Flare, Gel Dart, God's Paranoia, Hardened Honeycomb, Heavenfallen Stardisk, Ice Star, Icebreaker, Ichor Spear, Infernal Kris, Kylie, Leviathan Teeth, Lionfish, Night's Gaze, Palladium Javelin, Refraction Rotor, Scourge of the Seas, Shinobi Blade, Snap Clam, Spear of Paleolith, Throwing Brick, Time Bolt, Urchin Stinger, Utensil Poker
- **Distance** (30) : Angelic Shotgun, Anti-materiel Rifle, Arterial Assault, Astral Blaster, Auralis, Blood Boiler, Chicken Cannon, Condemnation, Flarewing Bow, Halley's Inferno, Infinity, Megalodon, Mineral Mortar, Minigun, Norfleet, P90, Pristine Annihilation, Rubico Prime, SDFMG, Scorched Earth, Septic Skewer, Shredder, Spectralstorm Cannon, Starfleet, Stellar Cannon, Storm Surge, Telluric Glare, The Sevens Striker, Tyranny's End, Ultima
- **Magie** (28) : Aether's Whisper, Alula Australis, Anahita's Arpeggio, Astral Staff, Death Valley Duster, Eidolic Wail, Flare Bolt, Frigidflash Bolt, Frost Bolt, Genisis, Lashes of Chaos, Nano Purge, Phoenix Flame Barrage, Plague Staff, Recitation of the Beast, Seething Discharge, Snowstorm Staff, Starfall, Tactician's Trump Card, Terra Ray, Thorn Blossom, Tome of Fates, Ultra Liquidator, Vehemence, Venusian Trident, Vesuvius, Void Vortex, Winter's Fury
- **Invocation** (24) : Ancient Ice Chunk, Ares' Exoskeleton, Borealis Bomber, Calamari's Lament, Caustic Staff, Cosmic Immaterializer, Cosmilamp, Dazzling Stabber Staff, Dragonblood Disgorger, Dreadmine Staff, Entropy's Vigil, Ethereal Subjugator, Flowers of Mortality, Godspawn Helix Staff, Herring Staff, Hive Pod, Igneous Exaltation, Orthocera Shell, Seabound Staff, Squirrel Squire Staff, Sun God Staff, Sun Spirit Staff, Wither Blossoms Staff, Yharon's Kindle Staff
- **Ailes** (9) : Aureate Booster, Elysian Tracers, Elysian Wings, Exodus Wings, Hadal Mantle, MOAB, Silva Wings, Starlight Wings, Tarragon Wings
- **Invocation de boss** (9) : Abombination, Ceremonial Urn, Charred Idol, Cryo Key, Eidolon Tablet, Martian Distress Beacon, Necroplasmic Beacon, Overloaded Sludge, Rune of Kos
- **Arsenal de Draedon** (9) : Galvanizing Glaive, Heavy Laser Rifle, Hydraulic Volt Crasher, Mounted Scanner, Phaseslayer, Pulse Dragon, Pulse Pistol, Taser, Wave Pounder
- **Pêche** (7) : Enchanted Starfish, Fish of Flight, Glimmering Gemfish, Rift Reeler, Spadefish, Wulfrum Fishing Pole, Xerocodile
- **Munitions** (4) : Bloodfire Bullet, Elysian Arrow, Napalm Arrow, Verium Bullet
- **Divers** (4) : Celestial Onion, Comet Shard, Ethereal Core, Phantom Heart
- **Outils** (4) : Axe of Purity, Broken Water Filter, Grax, Greatbay Pickaxe
- **Objets à poser** (3) : Altar of the Accursed, Resilient Candle, Spiteful Candle
- **Sans classe** (3) : Aestheticus, Eye of Magnus, Lunic Eye
- **Potions** (2) : Aureus Cell, Photosynthesis Potion
- **Montures** (1) : Gaze of Crysthamyr
- **Familiers** (1) : Hermit's Box of One Hundred Medicines

### Modifiées en partie, déjà différentes en 2.0.1.005 (le Fandom décrit une version plus ancienne, date entre parenthèses) : 32

- **Accessoires** (11) : Counter Scarf (2021-07), Craw Carapace (2018-12), Dynamo Stem Cells (2021-01), Elemental Quiver (2021-04), Evasion Scarf (2021-07), Nanotech (2020-10), Occult Skull Crown (2022-02), Old Die (2020-03), Profaned Soul Crystal (2022-03), Stress Pills (2020-05), Wulfrum Battery (2020-02)
- **Invocation de boss** (5) : Bloody Worm Food (2022-02), Decapodita Sprout (2022-02), Desert Medallion (2021-11), Profaned Shard (2022-02), Teratoma (2022-02)
- **Invocation** (4) : Black Hawk Remote (2019-11), Cosmic Viper Engine (2020-04), Temporal Umbrella (2020-04), Wulfrum Controller (2020-01)
- **Potions** (3) : Baguette (2020-04), Hadal Stew (2020-09), Odd Mushroom (2019-04)
- **Rogue (Spy)** (3) : Deep Sea Dumbbell (2019-09), Mycoroot (2020-05), Wulfrum Knife (2020-06)
- **Distance** (2) : Conference Call (2019-06), Soma Prime (2021-03)
- **Matériaux** (1) : Cosmilite Bar (2018-12)
- **Montures** (1) : Tundra Leash (2019-06)
- **Outils** (1) : Genesis Pickaxe (2021-12)
- **Magie** (1) : Coral Spout (2018-12)
