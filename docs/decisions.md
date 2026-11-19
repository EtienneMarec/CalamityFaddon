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

- **[validé 23/10]** **Objets renommés par Calamity depuis 2.0.1.005** (historique git, `tools/calamity_renames.py` :
  170 connus du wiki sous l'ancien nom, dont ~147 avec un nom affiché différent : Essence of Chaos → Essence of Havoc,
  Crimulan Slime God → Crimulan Paladin, sac/masque/trophée de Calamitas, Elemental/Terra X...) : même objet. Il reprend
  le nom de sa page **dans les deux modes** (le nom seul, pas une substitution dans tous les textes : beaucoup de
  nouveaux noms sont des mots courants) et n'est plus masqué en Full Fandom. Un renommage ne compte que si la page du
  wiki est du bon type (objet / PNJ). Un nouveau nom qui a sa propre page est gardé.
  Aussi les changements de **nom affiché** sans changement de classe (24, ex. Calamitas Clone → Calamitas, Cataclysmic
  Construct → Cataclysm, Signus → Signus, Envoy of the Devourer, journaux de Draedon) ; ancien nom lu dans le vieux code
  (un fichier qui déclarait plusieurs objets ne compte pas). **[choix]** Silva Basin → Effulgent Manipulator ajouté à la
  main au glossaire (le vieux code n'avait pas de nom déclaré) : débloque la station des meubles Silva (24 recettes).
  **[technique]** Le nom rendu est le titre de la page (redirections suivies : Fabsol's Vodka → Fluorescent Vodka), sauf
  redirection vers une page de groupe (Treasure Bag (Calamitas) → Treasure Bag : l'ancien nom reste).
- **[technique 26/10]** Corrections des renommages : (1) **chaîne** Caustic Edge → Tainted Blade et True Caustic Edge →
  Caustic Edge : la page « Caustic Edge » appartient à l'autre objet, elle ne protège pas le nouveau nom (avant, les deux
  s'appelaient « Caustic Edge ») ; (2) plusieurs objets envoyés vers **la même page** (les 5 Draedon's Logs, Crimulan /
  Ebonian Slime God vers The Slime God) : page de groupe, chacun garde son ancien nom (avant : tous « Draedon's Logs »,
  « The Slime God ») ; (3) **ancien nom non déclaré** dans le vieux code : c'est le nom de classe découpé que tModLoader
  affichait (« Eutrophic Bathtub », « Big Sightseer Banner » ; avant : « EutrophicBathtub », « Banners ») ; (4) **nom de
  classe repris** par un autre objet (NavystoneWallSafe → NavystoneWall) : pas un changement de nom de la même classe.
- **[choix 26/10]** Fruits de vie : **Hemagrape** = Blood Orange de 2022 = Sanguine Tangerine de Calamity, **Sambucus
  Fruit** = Elderberry = Tainted Cloudberry (infobulles identiques mot pour mot ; un changelog cite `[[Hemagrape|Blood
  Orange]]`). Renommages ajoutés au glossaire. La page « Elder Berry » (redirection d'Elderberry) décrit un autre objet,
  inventé (un dash).
- **[choix 26/10]** Renommages aussi lus dans les attributs **`[LegacyName]`** de Calamity (la sauvegarde garde l'objet),
  que l'historique git rate quand le fichier a été réécrit. Comme c'est souvent un objet différent (Gehenna → Slagsplitter
  Pauldron, Soul Edge → Void Edge, Holy Wrath Potion → Flask of Holy Flames), seulement quand le nom affiché de 2022
  (déclaré dans le vieux code) est le même au « The » près, et pour les **objets de lore**. Les autres sont listés par
  `python tools/calamity_renames.py` (« non retenus ») ; parmi eux, des renommages probables à trancher : Warbanner of the
  Sun → Warbanner of the Righteous, Urchin Flail → Urchin Mace, et des portages déjà faits dont Calamity a fait un autre
  objet (Gehenna, Aquatic Discharge, Soul Edge, Crimslime Staff : gardés dans les deux modes).
- **[choix 26/10]** **Objets de lore** (`tools/lore_items.py`) : le wiki les connaît par la page de groupe « Lore items »
  (pages « X (Lore) » = redirections) et les boss les lâchent sous ce titre. Nom affiché = celui du tableau, dans les deux
  modes, puis le glossaire (Meatmetal Sans) : The Desert Scourge, Old Duke, Jungle Dragon, Yharon... Quand Calamity a
  réécrit le lore sous un autre nom ([LegacyName]), l'objet reprend le nom de 2022 : The Archmage → Cryogen, Requiem → Moon
  Lord, Prelude → Lunatic Cultist, Azafure → Brimstone Crag, The Abyss → The Ocean, Blood Moon → The Red Moon, Ceaseless
  Void → The Sentinels of the Devourer (le lore commun des trois sentinelles en 2022). Le texte reste celui de Calamity.
  Objets de lore absents du tableau (Signus, Storm Weaver, Empress of Light, Queen Slime, Awakening, Cynosure) : masqués
  en Full Fandom. Les lignes vandalisées « The Eater of Ireland » / « The Destroyer of Ireland » (paroles de chanson à la
  place du texte) ne renomment rien : les redirections « The Eater of Worlds (Lore) » / « The Destroyer (Lore) » existent.
- **[technique]** Recettes : « Silver/Tungsten Bar » = Silver Bar ou Tungsten Bar (un mot seul reprend la fin du dernier
  nom). Alias ajoutés : Any Quivers / Any Mana Flowers → groupes de Calamity au singulier (le changelog 2.0.0.003 dit que
  le pluriel était une faute), Dirt → Dirt Block, Ancient Cultist Trophy → Lunatic Cultist Trophy (nom de vanilla 1.4).
- **[technique]** Alias de noms du butin et des recettes : le glossaire est aussi appliqué aux noms du wiki (« Treasure Bag
  (Astrum Deus) » → Treasure Bag (Astral Deus), « Rogue Emblem » → Spy Emblem), comme aux textes du jeu.
- **[technique]** Une page d'objet ne se relie qu'à un objet de Calamity, une page de PNJ qu'à un PNJ (Energy Orb,
  Shroomerang, Spectral Feather... étaient reliés à un projectile ou un buff homonyme ; Energy Orb est maintenant un
  matériau généré).
- **[validé 23/10]** Objets **portés** alors que Calamity les avait seulement renommés ou refaits (12 : Night's Gaze =
  Vega, Starfall = Star Shower, Solar Flare = Burning Revelation, Effluvium Bow = Hoarfrost Bow, Energy Staff = Sanctified
  Spark, Amidias' Spark = Ilmeris' Spark, Regenator = Regenerator, Corroslime Staff = Blighted Slime Staff ; refontes :
  Storm Spray = Sandstream Scepter, Seabound Staff = Brittle Star Staff, Shock Grenade = Doomsday Device, Mana Jelly =
  Cleansing Jelly) : **Full Fandom** = notre portage (2022, ce que décrit le wiki), l'objet actuel masqué ;
  **Calamity++** = l'objet actuel sous le nom du wiki, notre portage masqué (`plusHidden` de `WikiData/baseline.json`).
  **[technique]** La recherche par nom (recettes, butin...) passe l'objet masqué dans le mode en cours après son homonyme.
- **[validé 29/10, annule la fusion du 28/10]** Starfall, Seabound Staff et Effluvium Bow avaient été fusionnés le
  28/10 avec Star Shower, Brittle Star Staff et Hoarfrost Bow, présentés à tort comme identiques : Calamity les a refaits
  après 2.0.1.005 (`docs/DIFF_FANDOM.md`). Les portages sont rétablis et suivent la règle commune : **Full Fandom** = notre
  portage (version 2.0.1.005 décrite par le wiki), l'arme actuelle masquée ; **Calamity++** = l'arme actuelle sous le nom
  du wiki, le portage masqué. Tous les portages d'objets renommés sont donc en double : Night's Gaze, Starfall, Seabound
  Staff, Effluvium Bow, Solar Flare, Storm Spray, Shock Grenade, Energy Staff, Regenator, Mana Jelly, Amidias' Spark,
  Corroslime Staff.
- **[validé 30/10]** Seabound Staff : même sbire que le Brittle Star Staff actuel (seul le mode d'attaque a changé
  depuis), gardé tel quel. **[choix]** (30/10) L'Effluvium Bow tirait la Mist Arrow de Calamity 2.2.2, qui porte la
  refonte (division en brumes) : il tire maintenant la Mist Arrow de 2.0.1.005 (`LegacyMistArrow`, Frostburn 3 s et
  Glacial State 0,5 s de l'infobox). Le Hoarfrost Bow de Calamity++ garde la sienne.
- **[technique]** Les stats d'une page renommée s'appliquent à l'objet actuel ; 9 écarts extrêmes viennent d'objets
  refaits (Taser 998 dégâts au wiki contre 11, Deathhail Staff 328 contre 4100...) : Full Fandom seulement en attendant
  (`reports/stats.md`, à trancher).

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
- **[choix]** (test du 23/10) Meatmetal Sans : la tête, les griffes et les jambes du Ravager de Calamity sont retirées dès
  leur apparition (sans butin) : le wiki n'en parle pas (« he will stand in place ») et, vivantes, elles rendaient le corps
  invulnérable (barre à 210 023 PV). En attendant le vrai combat (feuille de route).
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
- **[technique]** Segments de ver et variantes qui reprennent le nom d'un autre PNJ (`GetText("NPCs.XHead.DisplayName")`) :
  même ratio de vie que lui. Sans ça, la tête du Desert Scourge s'aligne sur la vie du segment suivant (5 000 PV sur
  350 000 en jeu, passe 1.6). Concerne aussi Aquatic Scourge, Devourer of Gods, Storm Weaver (Full Fandom), Profaned Energy.
- **[choix]** Rareté du wiki : 0-11 vanilla, 12 Turquoise, 13 Pure Green, 14 Cosmic Purple, 15 Burnished Auric,
  16 Hot Pink, 17 Calamity Red ; rareté inventée (« C2 »...) → 0. Prix d'achat = prix de vente du wiki × 5.

## 4. Recettes (`tools/gen_recipes.py`, `Common/WikiData/WikiRecipes.cs`)

- **[choix]** Une recette existante qui diffère du wiki est **modifiée sur place** (ses conditions et effets spéciaux
  de Calamity sont conservés) ; on choisit la recette existante la plus proche (ingrédients ou station en commun).
- **[validé 21/10]** Recettes du jeu absentes du wiki : conservées en Calamity++, désactivées en Full Fandom.
- **[technique]** (24/10) Groupes « Any X » : ceux de la page « Alternative crafting ingredients » que le jeu n'a pas sous ce
  nom sont créés depuis la liste de la page (Any Evil Flesh = Shadow Scale / Tissue Sample, Any Hardmode Evil Component =
  Cursed Flame / Ichor...). Un groupe que le jeu connaît déjà n'est pas touché.
- **[choix]** Rien n'est changé si un ingrédient ou une station du wiki n'existe pas encore en jeu.
- **[choix]** Recettes vandalisées illisibles (« sqrt(-1) » Alloy Bars, « -1 ») : ignorées, le jeu garde la sienne.
- **[choix]** Recette en double sur deux pages du wiki : comptée une fois.
- **[choix]** « A/B » = au choix (groupe de recettes, réutilise un groupe existant s'il contient exactement ces objets).
- **[choix]** Stations : « Iron Anvil » = enclumes fer/plomb, « Mythril Anvil » = enclumes Hardmode, « Alchemy Table only »
  = table d'alchimie, « Placed Bottle » = bouteille posée, « By Hand » = sans station, « Water » = près de l'eau ;
  les autres par le nom de l'objet qui pose la station.
- **[validé 31/10]** **Objets de Terraria** : les recettes du wiki s'appliquent aussi, **dans les deux modes**. Pour celles
  qui cassent la progression ou sont trop pénibles, Calamity++ garde l'ancienne recette en plus, **au cas par cas**
  (`Balance/recipes.json`, avec la raison de chaque objet ; Minimal suit donc le wiki).
  **[choix]** Comment : une recette du wiki **remplace** la recette du jeu qui partage au moins un ingrédient avec elle
  (la plus proche, station comprise : Angel Wings 15 → 25 Soul of Light, potions de l'Alchemy Table en Blood Orb →
  Unholy Core) ; sans ingrédient commun, elle **s'ajoute** (Glass en Astral Clay à côté du Glass en sable, Feral Claws
  en Silk). Les recettes de base de Terraria absentes du wiki ne sont **jamais retirées**, même en Full Fandom : le wiki
  de Calamity ne liste que les recettes qu'il ajoute ou change, pas toutes celles du jeu.
  **[choix]** Exceptions Calamity++ proposées par Claude (47) : les 42 potions dont la recette en Blood Orb (avant le
  Hardmode) devient de l'Unholy Core (Hardmode) ; Starfury et Enchanted Sword (Essence of Law, Hardmode, pour des épées
  d'avant le Hardmode) ; Sandstorm in a Bottle (Desert Feather, Hardmode) ; Cloud in a Bottle (25 Feather, 12 Bottle,
  50 Fallen Star) ; Obsidian Rose (20 seaux de lave). Remplacés sans exception : ailes (25 âmes), forges (+15 Alloy
  Bar), Hallowed Bar (+5 Pixie Dust), Life Crystal, Life Fruit, Magic Mirror, armures Turtle, Lava Charm, Muramasa
  (+22 Alloy Bar), Terra Blade (sans Broken Hero Sword), etc. : liste complète dans `reports/recipes.md`.
- **[choix]** Shimmer : seules les pages « Shimmer » / « Shimmer Transmutation », et seulement si l'objet de départ ou
  d'arrivée vient de Calamity ou de ce mod. « Sanguine Shimmer » est une mécanique inventée, ignorée pour l'instant.

- **[technique]** Avant le 31/10 au soir, les 163 recettes du wiki pour des objets de Terraria étaient seulement comparées
  au jeu (63 recettes que le jeu n'a pas, 42 potions, 58 recettes changées).

- **[choix]** (02/11) Alias de recettes : « Fiery Greatsword » = le Volcano de Terraria (ancien nom ; Night's Edge et
  Hellkite du wiki), « Elemental Bars » = Elemental Bar (Augar of the Elements). Groupe « Any Zapinator » (Gray ou
  Orange Zapinator, recette de la Lazhar) ajouté à notre page « Alternative crafting ingredients ». Laissé : « Crab »
  (Baroclaw, Delicious Meat), un ennemi et non un objet.

## 5. Système d'éléments (`Common/Elements/`, `Balance/elements.json`)

Source : `Elemental System.txt` (17 éléments, une phrase d'effet chacun ; résistant = 50 % des dégâts et immunité à
l'effet, vulnérable = 200 %). Tout le reste est un choix, réglable dans `Common/Elements/Element.cs`.

- **[choix]** Nature des effets : un **debuff** de 2 s par élément (sauf Wind et Bomb, qui agissent au coup). Noms tirés
  des verbes de la page, descriptions écrites par Claude :

  | Élément | Debuff | Effet (pré-HM / HM / après Moon Lord pour les dégâts par seconde) |
  |---|---|---|
  | Cold | Numbed | ralenti 20 %, dégâts de contact -10 % |
  | Fire | Ablaze | 6 / 20 / 60 par s, défense -10 % |
  | Wind | — | recul x1,5 au coup ; à l'atterrissage (31/10), 25 % des dégâts du coup, jusqu'à 50 % selon la vitesse de chute (pas les boss, ni les ennemis sans gravité ou sans recul ; dégâts appliqués par le serveur : en multijoueur, TODO le coup est vu par le client qui frappe) |
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

- **[choix]** (31/10, phase 2) **Éléments des armures** : les pages d'ensembles (« Aerospec armor »...) ont des champs
  `resistance1..n` / `vulnerable1..n` ; `gen_elements.py` les écrit dans `armors` (WikiData/elements.json) avec le
  préfixe de classe des pièces (« Aerospec », « DesertProwler »...). Le joueur qui porte l'**ensemble complet** (casque,
  plastron et jambières au préfixe de l'ensemble) prend 50 % des dégâts des attaques d'un élément résisté et 200 % d'un
  élément vulnérable (produit borné à 25 %-400 %, comme pour les PNJ). **Élément d'une attaque ennemie** : le wiki n'en
  donne pas ; on prend les éléments auxquels le PNJ résiste (la page « Elemental System » : un ennemi de feu résiste au
  feu), pour son contact et ses projectiles. Mêmes règles dans les deux modes, à valider. 8 ensembles en jeu (Aerospec,
  Desert Prowler, Fearmonger, Snow Ruffian, Statigel, Sulphurous, Victide, Wulfrum) ; 5 sont des armures inventées pas
  encore créées (Banana Clown, Corpulent, Gel, Grarble, Horseslime). Les debuffs élémentaires du joueur ne changent pas.

## 5 bis. Proficiency (`tools/gen_proficiency.py`, `Common/Proficiency/`, pages Proficiency et Level Meters)

- **[choix]** (31/10, phase 2) Mécanique retirée de Calamity le 18/09/2022 (avant 2.0.1.005), décrite par le wiki :
  rétablie d'après la **page** et non l'ancien code. Tableau des 30 niveaux (seuils jusqu'à 50 000, bonus non cumulés)
  en données (`WikiData/proficiency.json`) ; l'ancien code plafonnait à 12 500 (15 niveaux) et bridait avant le
  Hardmode et avant le Moon Lord : la page n'en parle pas, pas de bride. Gain de 1 par coup de plus de 5 dégâts (ou sur
  un boss), pas sur les ennemis de statue ni les mannequins ; recharge commune de 2 s, moitié avec la Level Meter de la
  classe, qui donne aussi 10 % de chances de double gain si la valeur n'est pas un multiple de 100 ; valeur max 500 000.
  Spy = rogue. Bonus : dégâts et critique de la classe ; mêlée : vitesse ; distance : chance de ne pas consommer la
  munition ; magie : coût de mana réduit ; invocation : recul des sbires (+X) et emplacements (pas de critique, la page
  ne le liste pas pour l'invocation) ; spy : vitesse des projectiles. Montée de niveau : message (textes de l'ancien
  Calamity) et feu d'artifice. Gardée par personnage. Mêmes règles dans les deux modes, à valider.
- **[choix]** **Level Meters** (5, sprites de 2022) : vendues 5 or par le Goblin Tinkerer (« buy »), infobulle avec le
  niveau, la valeur, le prochain seuil et les bonus.

## 5 ter. Discrétion Spy et Rage/Adrenaline (`Common/Spy/`, `Content/Items/PermanentBoosters/`)

- **[validé 31/10]** Discrétion de la page « Spy » (section Stealth) **en Full Fandom seulement** : chaque
  point (1 % de la jauge) donne +0,5 % de dégâts Spy, +0,67 % par point jauge pleine, et -4 d'aggro par point (Calamity
  en retire déjà 3). Remplace le bonus de Calamity 2.2.2 (fonction de la vitesse de l'arme). Calamity++ garde le système
  actuel, mieux équilibré. Les frappes furtives, la vitesse de remplissage et l'Elephant Killer ne changent pas.
- **Metabolic Converter** (objet inventé, sac de The Inventors d'après la page Treasure Bag ; la page de l'objet
  écrit `[[Crabulon|The Inventors]]`) : ajouté aussi au sac de Crabulon dans notre page Treasure Bag tant que The Inventors
  (boss inventé, phase 4) n'existe pas, sinon Rage et Adrenaline seraient inaccessibles en Full Fandom. Objet
  consommable unique, gardé par personnage. **[validé 31/10] Dans les deux modes**, en Revengeance, Rage et Adrenaline
  restent verrouillées tant qu'il n'a pas été utilisé (les sources de Calamity qui les débloquent déjà, Shattered
  Community et Draedon's Heart, les laissent actives).
- **[choix]** Stamina : page de l'addon Catalyst, ignorée (voir section 11).

## 5 quater. Taming (`Common/Taming/`, page Taming)

- **[validé 31/10]** Actif **dans les deux modes**. Chaque nouveau boss du mod reçoit sa méthode (pas les superboss, en
  Full Fandom en tout cas). **Le jeu doit pouvoir se finir sans tuer un seul boss** : aucune méthode ne doit demander
  un objet qui ne tombe que d'un boss qu'il faut tuer avant (voir « Route sans tuer » plus bas).
- 43 boss sur 50 ont leur méthode (The Inventors le 18/11) ; les 7 autres attendent leur boss, inventé par le wiki (Wulfrum
  Excavator, Cumulomenace, Pyrogen, The Derellect, Polyphemalus, Yharim Worm, The Wall of Bronze ; XP-00 Hypnos a déjà
  sa méthode, les échecs, prête pour son boss). Liste en TODO dans `TamingSystem.PostSetupContent`.
- **[validé 31/10]** **Départ du boss apprivoisé** (`Common/Taming/TamingLeave.cs`) : au lieu de disparaître d'un coup, le
  boss et ses morceaux jouent leur animation d'abandon (celle du jeu ou de Calamity : le Eye of Cthulhu s'envole, les vers
  s'enfouissent...) : pendant leur mise à jour, ils croient tous les joueurs morts. Ils ne blessent plus et ne prennent
  plus de dégâts ; le butin est déjà tombé. **[choix]** Effacés en 1,5 s s'ils sont encore là au bout de 6 s.
  **[validé 31/10]** Plus de « ... has been defeated! » (annoncé par le jeu quand le butin tombe) : seulement « ... has
  been tamed! ».
  **[technique]** Le message « ... is no longer after you » vient du mod **Boss Checklist** (activé chez l'utilisateur),
  pour tout boss qui disparaît avec des PV : les PV du boss qui part sont mis à 0 au moment où il disparaît, le message
  ne sort plus.
- **Difficulté du Taming : propositions (31/10)**, tranchées plus bas. Remarque de l'utilisateur : apprivoiser doit rester
  difficile (battre Skeletron avec le seul bras de fer est trop facile). **[choix]** Principe proposé : Full Fandom suit
  la page telle qu'écrite ; Calamity++ durcit les méthodes trop faciles, par des données (`Balance/taming.json`).
  Levier commun proposé, le **calme final** : quand la méthode est réussie, le boss n'est pas apprivoisé tout de suite,
  il continue de se battre pendant 30 à 60 s (selon le moment du jeu) et il faut survivre sans le toucher ; les cœurs
  montent pendant ce temps. Il vise surtout les méthodes « instantanées ».

  | Boss | Méthode actuelle | Difficulté | Proposition Calamity++ |
  |---|---|---|---|
  | King Slime | Slimed 3 min | moyenne | garder |
  | Desert Scourge | 3 s à la plage | trop facile | 20 s cumulées à la plage |
  | Eye of Cthulhu | 30 tours | moyenne | garder |
  | Crabulon | Yharic Delight dans les 5 s (Traveling Merchant) | facile | + calme final 30 s |
  | Brain of Cthulhu | 3 mats en un, monde figé | facile si on sait jouer | 5 problèmes dont 2 mats en deux, ou une erreur relâche les Creepers 5 s |
  | Eater of Worlds | 3 aliments différents digérés | longue (15-30 min) | garder |
  | The Hive Mind | 100 livres posés | moyenne | garder |
  | The Perforators | 50 vers, Estranged Sons à protéger | dure | garder |
  | Queen Bee | 100 abeilles, apprivoisée à l'apparition | facile | elle apparaît non apprivoisée + calme final 45 s |
  | Skeletron | bras de fer, apprivoisé à l'apparition | trop facile | 3 manches de plus en plus dures (zone plus étroite, Old Man qui tire plus fort) ; entre deux manches, Skeletron attaque 30 s sans qu'on puisse le toucher |
  | The Slime God | 20 Blight Slimes chacun | moyenne | garder |
  | Wall of Flesh | mener au Guide (50 blocs) | presque automatique | garder le Guide en vie à moins de 50 blocs pendant 20 s |
  | Queen Slime | 100 ballons | moyenne | garder |
  | Cryogen | 3 s dans la Brimstone Crag | trop facile | 20 s cumulées |
  | Brimstone Elemental | 5 PNJ qui emménagent | dure (logistique) | garder |
  | The Twins | 1 min de regard chacun | dure | garder |
  | The Destroyer | 5 ruées dans la tête | moyenne | 10 ruées |
  | Skeletron Prime | Skull porté 1 min | facile | 3 min |
  | Aquatic Scourge | 3 s à la plage (il apparaît à côté) | trop facile | 30 s cumulées |
  | Leviathan and Anahita | 100 poissons rouges | moyenne à dure | garder |
  | Plantera | 3 min à la surface (enragée) | dure | garder |
  | Golem | 50 touches du ballon | moyenne | garder |
  | Calamitas | câlin de 1 s | facile | 5 s de contact cumulées |
  | The Plaguebringer Goliath | 200 Green Solution | moyenne | garder |
  | Duke Fishron | un Truffle Worm naturel | moyenne | garder |
  | Empress of Light | de l'aube au crépuscule | très dure | garder |
  | Ravager | chute dans le Shimmer | moyenne | garder |
  | Astrum Aureus | Draedon's Remote (3 boss mécaniques renforcés) | dure | garder |
  | Astrum Deus | une étoile filante | hasard | 3 étoiles |
  | The Dragonfolly | donner un Yharon Soul Fragment | facile | + calme final 45 s |
  | Lunatic Cultist | passer entre les dévots, apprivoisé à l'apparition | trop facile | + calme final 60 s |
  | Moon Lord | abysse + Colossal Squid | dure | garder |
  | Profaned Guardians | invoquer Providence pendant leur combat | dure | garder |
  | Providence | 4 coins du monde | très dure | garder |
  | Ceaseless Void | 50 rochers | moyenne | garder |
  | Storm Weaver | 10 Wyverns nourries | moyenne | garder |
  | Signus | 8 min sans dégâts | très dure | garder |
  | Polterghast | 100 tombes, apprivoisé à l'apparition | facile | + calme final 60 s |
  | The Devourer of Gods | 150 messages pendant le combat | dure | garder |
  | Jungle Dragon, Yharon | 5 min sans dégâts | très dure | garder |
  | The Fabricational Quartet | payer 1 platine au Tax Collector | trop facile | une taxe par Exo Mech (3), la suivante 60 s après la précédente |
  | Supreme Calamitas | 5 min jusqu'à 5 % puis alt-f4 | dure | garder |

  **[validé 31/10] Tranché par l'utilisateur** (Calamity++, `Balance/taming.json`, sauf mention) :
  - **Skeletron** : 3 mini-jeux différents avec 30 s de combat entre chacun : le bras de fer (avant, qui fait venir
    Skeletron), puis un **bonneteau** (l'Old Man cache une clé sous un des 3 crânes et les mélange 10 fois, de plus en plus
    vite ; un essai) et un **Simon** (4 dalles de couleur, une suite de 6 à refaire ; une erreur et c'est perdu). Un jeu
    perdu revient 30 s plus tard ; le monde est figé pendant chaque jeu ; toucher Skeletron fait échouer, comme avant.
    **[choix]** Les deux nouveaux jeux et leurs réglages (Claude) ; Échap pendant le combat = jeu perdu.
  - **Calme final** : Crabulon 60 s, The Hive Mind 40 s (proposition de l'utilisateur), The Dragonfolly 60 s, Lunatic
    Cultist 60 s, Polterghast 60 s ; The Inventors 60 s quand le boss existera. Message « Calming down... don't attack
    (x s) » toutes les 5 s.
  - **Fabricational Quartet** : 3 taxes, au moins 60 s entre deux (« Not so fast! ... » sinon, sans payer).
  - **Desert Scourge** 40 s à la plage, **Cryogen** 20 s dans la Crag, **Skeletron Prime** 2 min, **The Destroyer** 8
    ruées, **Astrum Deus** 3 étoiles (chaque étoile compte, dans les deux modes ; 1 en Full Fandom).
  - **Brain of Cthulhu** : 5 problèmes, les 2 derniers en mats en deux ; un mauvais coup retire un problème résolu.
  - **Wall of Flesh** : le Guide ne peut pas revenir pendant les 3 premières minutes (le mur avance : un peu plus d'un
    demi petit monde à traverser) ; ensuite, contrôle toutes les 5 s comme avant.
  - **Brimstone Elemental** (proposition de l'utilisateur) : un PNJ au plus toutes les 30 s (contrôle des arrivées toutes
    les 30 s au lieu de 5).
  - **Aquatic Scourge** (les deux modes, sens de la page) : la mer « classique », pas la Sulphurous Sea où il apparaît ;
    toujours 3 s. Même règle pour le Desert Scourge.
  - **Ravager** (les deux modes) : seulement un bassin naturel de l'Aether, pas un bassin fait à la main. **[choix]** au
    moins 30 blocs d'Aetherium à moins de 30 blocs.
  - **Gardés tels quels** : Calamitas (contact trop difficile à tenir vu ses déplacements), Aquatic Scourge 3 s, les autres
    marqués « garder ».
  - **Twins** (question de l'utilisateur) : il faut être tourné vers chaque jumeau, ligne de vue dégagée, 1 min chacun en
    même temps ; plus d'1 s hors de vue remet son compte à zéro.
  - **Queen Bee** : **40** abeilles en même temps, seulement les petites abeilles des ruches : blocs de ruche cassés (le
    jeu en libère 1 ou 2 une fois sur deux, quand le bloc ne se change pas en miel ; l'utilisateur avait raison, Claude
    l'avait raté) et ruches suspendues qui tombent (2 à 5). Ni celles du joueur (Beenade, Bee Gun...), ni celles de la
    Queen Bee. Les outils (pioches, haches, marteaux, foreuses) ne blessent pas ces abeilles, sinon on les tue en creusant.
- **Méthodes testées et validées en jeu par l'utilisateur** (31/10, dans l'ordre de ses tests) : King Slime, Desert
  Scourge, Eye of Cthulhu, Crabulon, Eater of Worlds, Brain of Cthulhu, The Hive Mind (« Place 100 books during the
  fight »), The Perforators, Queen Bee.
- **[validé 31/10, testé]** **Eater of Worlds** (« Consume 3 Food items... the Well Fed buff must run out ») : un aliment
  mangé pendant le combat compte **quand son Well Fed s'achève de lui-même** (il restait 1 ou 2 ticks au tick d'avant) ;
  retiré d'un clic droit, il ne compte pas (message). **Calamity++** (`Balance/taming.json`, `eaterDistinctFoods`) :
  3 aliments **différents** : un aliment déjà compté le dit dès qu'on le mange et à la fin de son Well Fed (validé 31/10). **[choix]** Remanger pendant un Well Fed : c'est le dernier
  aliment qui compte ; un aliment mangé avant le combat ne compte pas ; « X digested! » au-dessus du joueur.
- **[validé 31/10, testé]** **Crabulon** (« Consume a Yharic Delight while Crabulon is passive right after spawning ») : pendant
  les **5 premières secondes** du combat, vérifiées là où le combat est suivi (serveur ou solo). La phase calme de
  Calamity (`ai[0] == 0`, utilisée avant) ne dure que 2 s en normal, 1 s en Death, 15 ticks en Death collé au boss :
  trop court, et elle revenait après chaque série de sauts. **Calamity++** (`Balance/taming.json`, `hungaryDelight`) :
  le Yharic Delight (Yharim Bar, donc fin de jeu) s'obtient plus tôt en tapant exactement « Yo, I'm Hungary !! » dans le
  chat à moins de 10 blocs du Traveling Merchant, qui répond « Hi Hungary, I'm the Traveling Merchant... » et en donne
  **un seul par passage en ville** (tous joueurs confondus). **[choix]** Apostrophe typographique acceptée, espaces
  autour ignorés, majuscules exigées ; réponse du marchand écrite par Claude. À revoir avec le rework de Crabulon
  (`docs/ROADMAP.md` : sa page invente une phase passive au début du combat, qui remplacera les 5 s).
  **[choix]** Un Yharic Delight qui ne compte pas l'explique au joueur, en rose dans le chat : boss tué dans ce monde
  (« The Zoologist can soothe it »), blessé pendant ce combat, absent, ou trop tard (secondes écoulées). L'auto-test
  joue la scène (joueur factice, Crabulon, Yharic Delight mangé par le chemin du jeu, dans les 5 s puis après).
- **[validé 31/10]** **Queen Bee** (« Bring 100 bees into a hive biome ») : comptent les petites abeilles ennemies et
  celles du joueur (Bee Gun, Beenade, Bee's Knees, Hive Pack...). Dans une ruche (mur de ruche derrière l'abeille, ou
  joueur dans le biome), les abeilles du joueur rebondissent sur les blocs sans s'user (au lieu de mourir au 3e
  rebond). **[choix]** 100 abeilles **en même temps** à moins de 100 blocs d'un joueur dans une ruche (vérifié toutes
  les 30 ticks) ; elles vivent 15 s au lieu de 2,5 s dans une ruche, sinon 100 est hors d'atteinte ; compteur
  « x/100 bees » au-dessus du joueur, **[validé 31/10]** à chaque palier de 5 (5, 10, 15...). Sans Queen Bee : elle apparaît apprivoisée, **[validé 31/10]** sur le joueur
  (au milieu de la ruche et des abeilles : son butin y tombe, et non plus à l'écart). **[validé 31/10]** Ensuite, pas de
  nouvelle Queen Bee avant une demi-journée (12 minutes ; les abeilles restantes la refaisaient apparaître en boucle) ; le
  palier affiche alors « The Queen Bee is resting (x min) ». Pendant un combat où
  elle n'a pas été blessée (ses abeilles comptent) : apprivoisée aussitôt. (`Common/Taming/TamingBees.cs`)
- **[validé 31/10]** **Récompenses** : la page en nomme 17. Les 15 sans page : même objet dans les deux modes, sans effet
  en Full Fandom (infobulle « souvenir »), vrai effet en Calamity++. Pas encore créées : le boss apprivoisé laisse pour
  l'instant seulement son butin. Sprites : aucun sur le wiki (liste dans `docs/SPRITES.md`). Wulfrum Hardhat et Wulfrum
  Lenses ont une page (effets décrits, dans les deux modes) et viendront avec leurs boss (The Inventors, Wulfrum
  Excavator). **Propositions en Calamity++ (à trancher : 2 par récompense, A ou B ou autre)** :

  **Les 15 récompenses nommées par la page** (A = proposition du 31/10, B = nouvelle) :

  | Récompense | Boss | A | B |
  |---|---|---|---|
  | Bloated Gel | King Slime | accessoire : pas de dégâts de chute ; une chute de haut fait rebondir | accessoire : bouclier de gelée qui absorbe un coup toutes les 30 s et rend l'agresseur Slimed |
  | Hydrated Pendant | Desert Scourge | accessoire : souffle ×2 ; immunité à On Fire! et aux tempêtes de sable ; +10 % de vitesse dans le désert | accessoire : toujours Wet (immunité au feu) ; +1 PV/s dans l'eau ; les attaques mouillent (Wet) |
  | Dizzy Lens | Eye of Cthulhu | accessoire : les ennemis qui vous touchent sont Confused 3 s ; +3 % de critique | accessoire : tourner autour d'un ennemi (rester près de lui en bougeant) monte le critique jusqu'à +10 % |
  | Blessed Fungus | Crabulon | accessoire : +1 PV/s (+2 dans le biome champignon) | accessoire : les potions de soin rendent 15 % de plus et font pousser un champignon qui soigne 10 PV |
  | Worm Platter | Eater of Worlds | nourriture qui ne se consomme pas : Exquisitely Stuffed 30 min, recharge 30 min | nourriture : chaque aliment mangé ensuite allonge le buff au lieu de le remplacer (3 fois au plus) |
  | En Passant | Brain of Cthulhu | accessoire : esquive un coup toutes les 20 s par un pas de côté | accessoire : riposte, +10 % de dégâts contre un ennemi qui vous a touché dans les 5 dernières secondes |
  | Blighted Tome | The Hive Mind | arme magique : nuages de Shade (zone, Brain Rot), ~18 dégâts | accessoire : +5 % de dégâts magiques, les sorts ont 10 % de chances d'infliger Brain Rot |
  | Ball of Worms | The Perforators | familier : un petit Estranged Son suit le joueur | arme d'invocation : un Estranged Son sbire qui creuse vers les ennemis |
  | Motherbrain | Queen Bee | accessoire : les abeilles du joueur deviennent des Giant Bees ; +1 abeille par essaim | accessoire : les abeilles du joueur vivent 2 fois plus longtemps et infligent Confused |
  | Skeletron's Fist | Skeletron | arme de mêlée : gant qui lance une main osseuse qui revient | accessoire : +12 % de vitesse de mêlée et de recul ; vrai corps-à-corps +10 % de dégâts |
  | Fate | Wall of Flesh | accessoire : toutes les 3 min, survit à un coup mortel avec 1 PV | accessoire : sous 25 % de vie, +10 % de dégâts et 10 % d'esquive |
  | Melting Ice Cube | Cryogen | accessoire : immunité à Chilled, Frozen et Frostburn ; les attaques infligent Frostburn | accessoire : aura de froid qui ralentit de 15 % les ennemis proches ; +10 % de vitesse dans la chaleur (enfer, Crag) |
  | Frozen Heater | Pyrogen | accessoire : immunité à On Fire!, Burning et 2 s de lave ; les attaques infligent Brimstone Flames | accessoire : +2 PV/s dans la neige et immunité à Chilled ; dégâts de lave divisés par 2 |
  | Shiny Lipstick | The Destroyer | accessoire : aura, les ennemis à moins de 15 blocs font 10 % de dégâts en moins | accessoire : les dashs rendent invincible 0,5 s de plus ; les ennemis traversés sont Lovestruck |
  | Worship 101 | Lunatic Cultist | accessoire : +1 sbire et +10 % de dégâts d'invocation | accessoire : +1 tourelle et les sbires attaquent 15 % plus vite |

  **Les 34 boss dont la case est vide** (objet à inventer : nom, type, effet ; le nom rappelle la méthode) :

  | Boss | A | B |
  |---|---|---|
  | The Slime God | **Paladin's Truce** (accessoire) : les slimes ne vous attaquent plus (Royal Gel) ; tuer un slime soigne 5 PV | **Blight Slime Jar** (familier) : une petite Blight Slime suit le joueur |
  | Queen Slime | **Balloon Bouquet** (accessoire) : +1 saut, chute ralentie | **Sparkle Crown** (accessoire) : sauts 15 % plus hauts, les chutes font rebondir |
  | Cumulomenace | **Cloud Pillow** (accessoire) : pas de dégâts de chute ; +1 PV/s à l'arrêt | **Thundercloud in a Bottle** (accessoire) : double saut qui laisse un nuage électrique (Electrified) |
  | The Twins | **Staring Contest Trophy** (accessoire) : +10 % de critique contre l'ennemi le plus proche du curseur | **Twin Monocle** (accessoire) : vision nocturne et ennemis invisibles révélés |
  | Skeletron Prime | **Skull Polish** (accessoire) : +6 défense, immunité à Bleeding | **Brass Jaw** (casque d'apparence + accessoire) : +10 % de dégâts des sbires et tourelles |
  | Aquatic Scourge | **Seashell Necklace** (accessoire) : souffle ×2 et immunité à Irradiated dans la Sulphurous Sea | **Scourge Scale Scarf** (accessoire) : 5 % de réduction de dégâts dans l'eau |
  | Brimstone Elemental | **Housewarming Gift** (accessoire) : +1 PV/s et +5 % de dégâts près de 3 PNJ de la ville | **Brimstone Welcome Mat** (meuble) : posé, la Brimstone Crag compte comme un bon voisinage pour les PNJ |
  | Plantera | **Sunflower Crown** (accessoire) : à la surface le jour, +2 PV/s et +5 % de dégâts | **Potted Bulb** (familier) : un petit bulbe de Plantera suit le joueur |
  | Calamitas | **Warm Hug** (accessoire) : Brimstone Flames dure 2 fois moins ; +5 défense | **Calamitas' Scarf** (apparence + accessoire) : immunité à Brimstone Flames |
  | The Derellect | **Rusty Wrench** (arme de mêlée) : clé à molette de vrai corps-à-corps | **Salvaged Gear** (accessoire) : +15 % de vitesse de minage et de pose |
  | Polyphemalus | **Lap Counter** (accessoire) : la vitesse de course monte quand on tourne autour d'un ennemi | **Eye Patch** (accessoire) : +5 % de critique, -5 % de dégâts subis |
  | Leviathan and Anahita | **Goldfish Bowl** (familier) : un poisson rouge flotte autour du joueur | **Siren's Lullaby** (accessoire) : les ennemis aquatiques ne vous attaquent pas tant que vous ne les frappez pas |
  | Astrum Aureus | **Spare Remote** (outil) : Draedon's Remote réutilisable (une fois par nuit) | **Astral Shock Absorber** (accessoire) : pas de dégâts de chute, sauts plus hauts dans l'Astral |
  | Golem | **Beach Ball** (arme de lancer) : ballon qui rebondit sur les ennemis | **Sunbathing Towel** (accessoire) : le jour, +4 défense et +1 PV/s |
  | The Plaguebringer Goliath | **Clean Stinger** (accessoire) : immunité à Plague, +5 % de dégâts | **Green Solution Flask** (accessoire) : les ennemis atteints de Plague prennent 10 % de dégâts en plus |
  | Duke Fishron | **Truffle Worm Plush** (familier) | **Duke's Fishing Hat** (accessoire) : +15 de puissance de pêche |
  | Empress of Light | **Dawn-to-Dusk Charm** (accessoire) : le jour +8 % de dégâts, la nuit +4 défense | **Prismatic Parasol** (accessoire) : chute lente qui laisse un arc-en-ciel |
  | Ravager | **Shimmered Heart** (accessoire) : +20 PV max | **Flesh Golem Plush** (familier) |
  | The Dragonfolly | **Feather of Friendship** (accessoire) : chute lente, vol 10 % plus rapide | **Folly Nest** (familier) : un oisillon suit le joueur |
  | Astrum Deus | **Wishing Star** (accessoire) : plus d'étoiles filantes près du joueur ; +20 mana | **Starstruck Collar** (accessoire) : la nuit, régénération de mana +50 % |
  | Moon Lord | **Squid Ink Lens** (accessoire) : lumière et vision dans l'Abyss | **Eldritch Eye Patch** (accessoire) : Hunter et Spelunker |
  | Yharim Worm | **Ultisword Sheath** (accessoire) : +10 % de dégâts et de vitesse de mêlée | **Tyrant's Gift Ribbon** (accessoire) : +5 % à tous les dégâts |
  | Profaned Guardians | **Guardian's Escort Badge** (accessoire) : +6 défense quand un boss est en vie | **Profaned Bell** (familier lumineux) |
  | Providence, the Profaned Goddess | **Travel Mug of Holy Flames** (accessoire) : +15 % de vitesse, immunité à Holy Flames | **World Corner Compass** (outil) : téléporte au coin du monde le plus proche |
  | Ceaseless Void | **Void Vacuum** (accessoire) : ramasse les objets de très loin | **Boulder Collection** (accessoire) : les rochers ne vous blessent plus |
  | Storm Weaver | **Wyvern Treat Bag** (accessoire) : les Wyverns ne vous attaquent plus | **Weaver's Kite** (accessoire) : chute lente, +20 % de vitesse par vent fort |
  | Signus, Envoy of the Devourer | **Untouchable Cloak** (accessoire) : après 30 s sans dégâts, +10 % de dégâts | **Envoy's Mask** (accessoire) : esquive un coup toutes les 45 s |
  | Polterghast | **Gravestone Bouquet** (accessoire) : les fantômes du donjon ne vous attaquent plus | **Ectoplasm Lantern** (familier lumineux) |
  | The Devourer of Gods | **Fan Mail** (accessoire) : +6 % de dégâts et de vitesse | **Devourer's Pillow** (familier) : un petit Devourer of Gods |
  | Jungle Dragon, Yharon | **Fireproof Feather** (accessoire) : immunité à Dragonfire et Burning, +1 saut | **Dragon Egg Nightlight** (familier lumineux) |
  | The Wall of Bronze | **Bronze Odometer** (accessoire) : +15 % de vitesse de course | **World Map Scroll** (outil) : révèle la carte autour du joueur |
  | The Fabricational Quartet | **Tax Refund** (consommable) : rend 1 platine, 10 % de remise chez les marchands pendant 1 jour | **Draedon's Receipt** (accessoire) : 15 % de remise chez les marchands |
  | XP-00 Hypnos | **Grandmaster's Pin** (accessoire) : +5 % de critique et de vitesse d'attaque | **Hypnotic Pendulum** (accessoire) : 10 % de chances d'infliger Confused |
  | Supreme Calamitas | **Alt+F4 Key** (accessoire) : une fois par jour, survit à un coup mortel et revient au point d'apparition | **Brimstone Teacup** (nourriture) : 20 min, +2 PV/s et immunité à Vulnerability Hex |
- **[technique]** « Cannot harm the boss » : toute perte de PV d'un PNJ du combat (coups, debuffs) fait échouer la
  tentative jusqu'à ce que le boss disparaisse. Les projectiles des méthodes (Slime Gun, Sparkle Slime Balloon, Green
  Solution, étoile filante, rochers) ne touchent pas le boss. Apprivoisé : cœurs, émoticône, message ; le boss laisse son
  butin (**[validé 31/10]** puis repart avec sa propre animation d'abandon, voir `TamingLeave` plus bas), ce qui compte
  comme une victoire (drapeaux de progression, Hardmode après le Wall of Flesh).
  Tué, il devient inapprivoisable (liste sauvegardée avec le monde). Progrès affiché par quart (♥ 25 %...).
- **[choix]** Méthodes interprétées : « lead to » un biome = 3 s cumulées avec le joueur le plus proche dans le biome
  (Desert Scourge et Aquatic Scourge : océan ; Cryogen : Brimstone Crag) ; Eye of Cthulhu : tours comptés autour de
  l'œil (**[validé 31/10]** seul le mouvement du joueur compte, pas celui de l'œil ; « 3/30 » à chaque tour, « ↺ 0/30 »
  à la remise à zéro), un tour entier en arrière remet à zéro (**[validé 31/10]**, un quart de tour était trop
  sensible aux esquives des charges) ; Twins : regarder un jumeau = être tourné vers lui avec la ligne de
  vue ; Skeletron Prime : Skull équipé une minute d'affilée ; Plantera : 3 min d'affilée en surface ; Calamitas
  (le clone) : « hug » = 1 s au contact ; Wall of Flesh : le Guide à 50 blocs ou moins de la bouche ou d'un œil ;
  Duke Fishron : à 10 blocs d'un Truffle Worm non relâché par un joueur ; Empress of Light : en vie au lever du jour
  (ou invoquée dans la première heure) et jusqu'au coucher ; Dragonfolly : le Yharon Soul Fragment lâché au sol doit
  toucher l'oiseau ; Destroyer : une ruée (dashDelay < 0) qui touche la tête compte une fois ; Moon Lord : un Colossal
  Squid apparu pendant le combat et un joueur dans la 4e couche de l'Abyss ; Providence : coins = 250 blocs des bords,
  espace ou enfer ; Ceaseless Void : Boulder, Bouncy, Life Crystal, Moon et Mini Boulder ; Storm Weaver : une wyverne
  dont la tête touche celle du Storm Weaver est « mangée » ; Signus/Yharon : dégâts reçus = compteur à zéro ;
  Devourer of Gods : chaque message compte 1, +1 par mot de la liste, 5 s'il contient « providence » (l'enragement
  n'est pas fait) ; Polterghast : 100 tombes posées sur des murs de donjon (compteur du monde) ; Queen Slime : pas de
  passage forcé en phase 2 à 50 ballons. King Slime et Queen Slime grandissent jusqu'à **+50 %** (**[validé 31/10]**
  au lieu des +100 % de la page, trop gros à l'écran : King Slime est déjà dessiné jusqu'à ×1,5 par Calamity ; page
  de notre wiki corrigée) (synchronisé en
  multijoueur ; **[technique]** agrandissement appliqué au dessin et à la hitbox de King Slime, pas à `NPC.scale`, que
  l'IA de Calamity recalcule à chaque tick avant nos hooks ; Queen Slime : dessin seulement). Ordre vérifié dans
  tModLoader (31/10) : PreAI de Calamity (IA de King Slime, qui ne remet la hitbox à 98×92×scale que si scale change)
  → notre PostAI (hitbox agrandie par le bas) → collisions → PreDraw (dessin agrandi) → PostDraw (scale rendu).
- **King Slime** : **[validé 31/10]** 5 minutes en Full Fandom, **3 minutes en Calamity++** (`Balance/taming.json`, objectifs
  par boss, notés `{{mode|5|3}}` sur notre page Taming). Elles comptent le temps où il est Slimed, quand ce Slimed vient du Slime
  Gun (un Slimed d'une autre source ne compte pas tant qu'aucun jet ne l'a touché). **[choix]** Chaque jet qui le touche
  pose au moins 3 s de Slimed (le jeu garde la durée la plus longue si celle du Slime Gun l'est).
- **[choix]** « Ignore boss zen » : pendant ces combats, la réduction d'apparitions de Calamity est annulée pour le
  Colossal Squid (Abyss) et pour les apparitions vanilla de l'espace (wyvernes, harpies).
- **The Perforators** : « spawn 50 worms » = Worms (critters) relâchés par le joueur ou sortis d'une Worm Statue
  pendant la tentative ; 5 s plus tard chacun devient un **Estranged Son** (`Content/NPCs/Perforators/`, sans page) et
  compte 1. **[validé 31/10]** Les vers sont invincibles tant que les Perforators sont là (leurs attaques les tuaient
  avant la transformation). **[validé 31/10]** Sprites : resprite du Giant Worm par l'utilisateur, réduit de moitié (il
  paraissait deux fois trop gros ; hitbox 16, segments espacés de 20 px) ; **[technique]** un seul PNJ qui dessine ses
  segments (un PNJ par segment : 50 Estranged Sons = 400 PNJ, le jeu en accepte 200, les vers suivants n'apparaissaient
  plus) ; les segments blessent quand même au contact. Taille : le Giant Worm de Terraria fait 18×30 par segment, les
  sprites de l'utilisateur 54×72 (tête), 42×54 (corps), 42×66 (queue) en blocs de 2 : à moitié, encore ~1,5× plus
  larges ; **[validé 31/10]** on garde la moitié ; IA de ver fouisseur du Giant Worm,
  plus vive. **[choix]** Valeurs : tête + 6 segments + queue, 60 PV partagés, dégâts 14/10/8, défense 2/4/6, vitesse 8
  et accélération 0,1 (Giant Worm ≈ 6 et 0,07). Tuer un Estranged Son fait échouer la tentative ; ils disparaissent
  avec les Perforators apprivoisés.
- **Route sans tuer** (`Common/Taming/TamingProgression.cs`) : **[choix]** Slime Gun (King Slime) fabriqué à l'enclume :
  30 Gel + 4 Iron/Lead Bar. **[technique]** Wall of Flesh : le jeu de base fait revenir les PNJ pendant les combats de
  boss (seules les invasions et éclipses bloquent les arrivées, vérifié dans le code de tModLoader), mais au contrôle
  suivant (jusqu'à 2 min) : pendant une tentative (Wall of Flesh, et Brimstone Elemental plus tard), contrôle toutes
  les 5 s. Il faut une maison libre près du passage du Wall of Flesh pour que le Guide y revienne. **[choix]** The
  Dragonfolly demande un Yharon Soul Fragment, que seul Yharon lâche (invoqué avec les plumes du Dragonfolly) : comme
  les « Hellcaster Fragments » d'avant (Trivia de la page), les ennemis du Solar Pillar en lâchent un sur 20, dans les
  deux modes.
- **Lunatic Cultist** : le « Cultist set » (redirection vers le wiki de Terraria) = Lunar Cultist Hood + Lunar Cultist
  Robe du jeu de base (aussi lâchées par les Eidolists d'après le wiki), en armure ou en apparence. **[choix]** Passer
  entre les deux Lunatic Devotees (horizontalement, à 10 blocs de hauteur au plus) fait apparaître le Lunatic Cultist
  au-dessus d'eux, apprivoisé aussitôt : les piliers arrivent comme après une victoire.
- **[validé 31/10]** **Bras de fer (Skeletron)** : tout est figé pendant le bras de fer, comme aux échecs (joueurs,
  ennemis, projectiles, apparitions) ; un coup raté fait reculer de 3 (**[choix]** 3 dans les « -3/4 » demandés). Le monde
  repart dès le résultat (affiché encore 1,5 s).
- **Échecs** (Brain of Cthulhu, XP-00 Hypnos) : **[validé 31/10]** le **Holographic Chessboard** (sans page) lance des
  problèmes : 3 à réussir, tirés au hasard ; mats en un contre le Brain of Cthulhu, mats en deux (plus durs) contre
  Hypnos (boss à créer). **[choix]** 45 problèmes de chaque sorte, générés par `tools/chess_puzzles` (positions tirées
  au hasard, roi en défense abrité derrière ses pions, pas plus de pièces qu'au départ ; gardées si une seule solution ;
  mats en deux : coup clé unique, silencieux, au moins deux réponses), la moitié avec les Noirs au trait ; moteur
  vérifié par les comptes « perft » de référence, et l'auto-test revérifie chaque problème. Pas de problèmes repris
  d'un site (droits) : générés par notre code. Un mauvais coup fait perdre le problème (un autre est tiré, sans
  compter) ; promotion toujours en dame ; dans un mat en deux, l'adversaire répond au hasard. Le Brain of Cthulhu et
  ses Creepers attendent pendant la partie (ils jouent). Sans boss proche : entraînement (mats en deux après le Moon
  Lord). Fabriqué à l'établi : 10 Glass, 10 bois, 1 Fallen Star. Sprite et pièces provisoires (`docs/SPRITES.md`).
  **[validé 31/10, testé]** Fonctionnement de base. Puis, à la demande de l'utilisateur :
  - **Positions possibles** dans une vraie partie sans promotion (`ChessPosition.IsPlausible`, utilisé par le générateur
    et l'auto-test) : deux fous d'un camp sur des cases de couleurs différentes (10 des 90 anciens problèmes avaient deux
    fous sur la même couleur), pas plus de pièces qu'au départ, pas de pion sur les rangées 1 et 8, rois non adjacents,
    camp qui ne joue pas hors d'échec. Les 90 problèmes ont été régénérés.
  - **Tout s'arrête** pendant une partie contre un boss : mise à jour des PNJ (boss, Creepers, autres ennemis,
    animations comprises) et des projectiles sautée, joueurs sans contrôles, immobiles, et que rien ne touche (pas
    l'invulnérabilité du jeu, qui fait clignoter le joueur).
    **[choix]** Tout le monde est figé, aussi en multijoueur (l'état de chaque joueur est envoyé à tous) ; rien n'est
    figé à l'entraînement (sans boss).
  - **Table et chaises** (sens des chaises validé le 31/10) : une table et deux chaises en bois (textures des meubles de
    Terraria) dessinées sous les pieds du joueur, par-dessus le décor (projectile `ChessTable`, sans dégâts).
    **[validé 31/10]** Mise en scène dans l'ordre : clic ; la table, la chaise du joueur, puis celle de l'adversaire
    apparaissent une par une (bruit et poussière de bois) ; le joueur glisse jusqu'à sa chaise ; l'adversaire glisse
    jusqu'à la sienne, **reculée selon la largeur de son sprite** ; poignée de main (le Creeper le plus proche, « œil »
    du Brain of Cthulhu, vient à la main du joueur ; un autre adversaire sautille face au bras tendu) ; puis le plateau.
    **[choix]** Durées : un meuble toutes les 0,25 s, joueur 1 s, adversaire 0,83 s, poignée de main 1 s (3,6 s en
    tout). **[validé 31/10]** La scène se pose à **7 blocs devant le joueur** (5 à 10 demandés), sur le premier sol
    trouvé (sinon en l'air) ; joueur et adversaire sont posés à la même hauteur, sur l'assise (12 px) ; les ennemis en
    plusieurs morceaux (Wyvern, vers) se déplacent en entier (morceaux liés par `realLife`). Rouvrir le plateau face au même adversaire réutilise la table sans reposer les meubles.
  - **[validé 31/10]** **N'importe quel PNJ à moins de 10 blocs**, **PNJ de la ville compris**, mais **pas les boss** (sauf
    le Brain of Cthulhu et, plus tard, XP-00 Hypnos, pour leur Taming) ni les morceaux de boss du Taming : même mise en
    scène et monde figé, partie d'entraînement pour le plaisir (ne fait rien). La table disparaît à la fin de la partie
    (face au Brain of Cthulhu : à la fin du combat). **[choix]** Le plus proche ; bestioles comprises.
  - **[validé 31/10]** **Personne à portée** : même mise en scène, mais à la place de l'adversaire, « Sit at the
    chessboard and play with yourself » s'affiche au-dessus de la chaise vide ; pas de poignée de main.
  - **[validé 31/10]** Plateau inutilisable avant la fin de l'**apparition** du boss : Calamity joue une animation
    d'apparition (état 0 ou 1 de son IA, cri au bout de 4 s, fin vers 7 s) ; les 2 s d'abord choisies (son Roar_0 de
    1,98 s) ne suffisaient pas, elles restent seulement si l'IA de Calamity n'est pas active. **[validé 31/10]** Plus
    d'apparition d'ennemis pendant une partie (sinon une vague arrivait à la fin) ; phrase « play with yourself » en doré (le bleu se perdait dans le ciel).
  - **[validé 31/10]** Chaise du **Brain of Cthulhu** reculée de 7,5 blocs de plus que sa largeur (son aura rouge et rose
    cachait la scène). **[choix]** 7,5 blocs, dans la fourchette de 5 à 10 demandée.
  - **[technique]** Le joueur est placé sur sa chaise avant et après le mouvement du jeu, et le décalage de marche
    (`gfxOffY`, resté figé) est remis à zéro : il apparaissait parfois au-dessus ou en dessous de la chaise.
- **The Slime God** (« Have both slime gods absorb 20 of their respective Blight Slime. Note that blight slimes will
  ignore boss zen while the Slime God is alive ») : une Ebonian Blight Slime qui touche l'Ebonian Paladin (ou une
  Crimulan contre le Crimulan Paladin) disparaît et compte pour lui ; il en faut 20 de chaque ; butin du noyau.
  Pendant la tentative, la réduction d'apparitions de Calamity est annulée pour les Blight Slimes (Corruption ou
  Crimson). À vérifier en jeu : un monde n'a qu'une corruption, il faut créer un coin de l'autre (graines ou poudres
  de l'autre corruption) pour trouver l'autre Blight Slime.
- **Brimstone Elemental** (« Have five new Town NPCs move into the brimstone crag ») : un PNJ de ville qui prend une
  maison dans la Brimstone Crag pendant la tentative compte (une fois par PNJ, un PNJ mort qui revient compte de
  nouveau ; ceux qui y vivaient au début ne comptent pas) ; il en faut 5. **[technique]** « Dans la Crag » : maison
  en enfer avec plus de 500 blocs de Brimstone Slag ou d'Infernal Suevite dans 120 × 80 blocs autour (règle de
  Calamity pour le joueur). Arrivées de PNJ contrôlées toutes les 5 s pendant la tentative (comme pour le Wall of
  Flesh).
- **Astrum Aureus** (« Use Draedon's Remote while Astrum Aureus is alive ») : utiliser le Draedon's Remote (porté, voir
  section 8) pendant qu'il est en vie l'apprivoise ; **[choix]** le remote n'appelle alors pas les boss mécaniques.
- **Leviathan and Anahita** (« Spawn 100 goldfish and allow Leviathan to kill them. Spawning a goldfish during the fight
  will cause Leviathan to awaken early ») : un Goldfish (relâché, sorti d'une statue, ou qui marche sous la pluie) qui
  apparaît pendant la tentative fait appeler Leviathan par Anahita (projectile LeviathanSpawner de Calamity, comme à
  40 % de vie) ; **[choix]** un Goldfish touché par Leviathan meurt et compte ; à 100, le duo est apprivoisé.
- **Golem** (« Hit a beach ball 50 times while Golem is alive. Beachball hits by Golem will count ») : **[validé 31/10]**
  un coup = un Beach Ball qui entre en contact avec un joueur ou avec Golem (corps, tête, poings ; le ballon ignore
  les ennemis dans le jeu de base, le contact est donc détecté par notre code) ; pas de rebond ajouté. **[choix]** Au
  plus un coup par ballon toutes les demi-secondes (sinon un ballon posé contre Golem compterait en continu). Le
  ballon ne doit pas rester bloqué : pendant une tentative il ne disparaît pas, et immobile 3 s il est relancé en
  l'air. Il ne fait pas de dégâts dans le jeu de base.
- **Skeletron** (« Arm Wrestle the Old Man ») : **[validé 31/10]** mini-jeu de l'utilisateur : un cran fait des
  allers-retours sur une barre ; cliquer (ou Espace) quand il est dans la zone fait avancer le bras de fer de 5, de 10
  dans la zone ×2 (deux fois plus étroite, au milieu) ; l'Old Man tire de 1 toutes les demi-secondes. **[choix]** Gagné
  à +50, perdu à -50 (on peut recommencer) ; 1/3 s sans recliquer après chaque clic (sinon cliquer en continu gagne à
  coup sûr), un clic raté ne coûte rien d'autre ; après chaque coup réussi, la zone change de place et le cran accélère
  de 5 % (jusqu'à ×2) ; zone large de 20 % de la barre, cran qui la traverse en 1 s au départ. « Going down 3 options,
  permanently revealing it » : le bouton « Arm Wrestle » apparaît à la 3e conversation avec l'Old Man, pour toujours
  (par personnage). Gagner fait apparaître Skeletron comme la malédiction (l'Old Man disparaît), apprivoisé aussitôt :
  donjon ouvert, Clothier, à toute heure.
- **The Fabricational Quartet** (Exo Mechs) : **[validé 31/10]** « File Draedons Taxes » : pendant le combat seulement,
  le Tax Collector a un second bouton « File Draedon's taxes » qui fait payer 1 platine (texte de dialogue changé
  aussi) ; **[technique]** tModLoader ne permet de changer les boutons que des PNJ du mod : détour sur
  `NPCLoader.SetChatButtons`. Le butin tombe du mech qui se bat (Thanatos, Ares ou Apollo).
- **Supreme Calamitas** : **[validé 31/10]** si tous les autres boss apprivoisables sont apprivoisés dans ce monde, sa
  vie baisse d'elle-même tant qu'on ne la touche pas (**[choix]** de 100 % à 5 % en 5 min ; message « press Alt+F4 »
  à 5 %). Quitter le jeu à ce moment (alt-f4, ou Save & Exit) l'apprivoise : au chargement suivant du monde, elle
  apparaît et est apprivoisée aussitôt (butin, Brimstone Witch). **[technique]** Le départ est noté dans
  `CalamityFandom_SupremeCalamitasTamed.txt` (dossier de sauvegarde de Terraria, avec l'identifiant du monde) dès les
  5 %, effacé si le combat finit autrement (coup, mort, départ du boss). À tester : ses phases (frères, Sepulcher)
  quand sa vie baisse sans dégâts.
- **Soothing** : le Zoologist vend « Soothe: <boss> » pour chaque boss inapprivoisable, un par jour (remis à l'aube).
  **[validé 31/10]** Après un achat, toutes les offres disparaissent aussitôt de la boutique ouverte, jusqu'à l'aube.
  **[validé 31/10, à revoir en test]** Prix (la page n'en donne pas) : 2 or avant le Hardmode, 10 or en Hardmode,
  50 or après le Moon Lord.

## 6. Matériaux générés (`tools/gen_items.py`)

- **[choix]** Portée : pages absentes de Calamity de type Crafting material / Bar / Soul, plus les « Block » qui servent
  d'ingrédient dans une recette (les barres sont des blocs sur le wiki). 218 objets.
- **[choix]** Barres et blocs **non posables** pour l'instant (pas de tuiles).
- **[choix]** Pile = champ `stack` (9999 par défaut) ; objet de recherche (Journey) : 25.
- **[choix]** Animation : frames identiques successives fusionnées, vitesse tirée du GIF du wiki ; les matériaux animés
  flottent au sol comme les âmes.
- **[choix]** Un matériau dont le nom existe déjà en vanilla n'est pas créé.
- **[choix]** (24/10) Type « Energy Source » (Char Matter) traité comme un matériau simple.
- **[choix]** (24/10) **Ailes** du wiki générées (11, `WikiWings`) : stats de l'infobulle ; vitesse verticale Average/Good/
  Great/Excellent = réglages choisis ; image portée = la pose unique du wiki répétée (pas de battement) ; Bellwings sans image
  portée (invisible). Bonus d'armure de set et phrases d'ambiance non traduits.
- **[technique]** (28/10) Les images « (equipped) » du wiki montrent le personnage avec ses ailes : `gen_items.py` l'efface
  (palette d'un personnage de référence, moyenne des images du wiki, autour de sa position retrouvée) et recentre l'image
  sur le point d'accroche des ailes de Terraria (9 px derrière le centre du joueur, 19 px au-dessus du sol).
- **[choix]** (24/10) **Aliments** générés comme les matériaux : potion du wiki dont le buff est Well Fed / Plenty Satisfied /
  Exquisitely Stuffed, avec la durée de l'infobox (11 : Titan Finger, Clam Chowder, Cheese, Crimtato, Yharic Delight...).
  Leurs effets en plus écrits en prose (Yharic Delight « Instantly heals the user to full health ») ne sont pas encore faits.
- **[technique]** (28/10) Aliments : la ligne « Increases life regeneration » des infobulles du wiki est retirée, Terraria
  l'ajoute lui-même aux aliments en Expert (doublon vu en jeu sur Clam Chowder et Crimtato).
- **[validé 22/10]** **Yharim Bar géant au sol** (x16, d'après l'image « The item in-game » de sa page : ~480 px de large
  pour un sprite de 30), **dans les deux modes**, zone de ramassage comprise (hitbox x16). Taille normale dans
  l'inventaire. Données : `WORLD_SCALE` dans `tools/gen_items.py` → `worldScale` de `WikiData/items.json`.
  Au sol, version détaillée détourée par l'utilisateur dans l'image du wiki (`wiki_sprites_custom/Yharim Bar (world).png`,
  ramenée à 480×384) ; sans elle, le sprite normal agrandi.
  **[technique]** Un objet si grand qui tombe dans un tunnel étroit peut rester coincé dans les blocs.
- **[validé 21/10]** Sprite absent ou inutilisable → demandé à l'utilisateur (`docs/SPRITES.md`). Provisoire en attendant :
  « Mars Bars » (photo réduite à 90×70).

- **[choix]** (31/10) Un « Block » du wiki est aussi généré comme matériau s'il est dans le butin du wiki (Antlion Ore,
  Angelinite Ore, Sallowrack...), sauf s'il est seulement historique (Frigid Bar, « Historical content »).
- **[choix]** (31/10) **Matériaux qui se posent** (`Content/Tiles/WikiTile.cs`, `Assets/WikiTiles/`) : les matériaux
  « Block » dont le wiki montre le bloc posé (image « (placed) », 3 × 3 blocs) se posent : 47 (44 blocs, 3 barres :
  Flahium, Omnicidal Sludge, Verdantium). Texture = bloc du milieu de cette image ; les blocs sont pleins et
  n'ont pas encore de bordures (toutes les cases de la planche sont ce bloc) ; les barres sont des tuiles 1 × 1
  posées comme les barres de métal du jeu (brillantes). Minerais (« Ore », « Slag », « Cluster ») visibles au
  Spelunker et au détecteur de métaux ; pas de pioche minimale (rien n'est généré dans le monde). Les 22 autres
  « Block » (Alloy Bar, Yharim Bar...) n'ont pas d'image posée sur le wiki : ils restent des matériaux.

- **[choix]** (02/11) **Murs du wiki** (`Content/Tiles/WikiWall.cs`, `Assets/WikiWalls/`) : tout mur dont le wiki montre
  l'image « (placed) » devient un objet qui pose un mur (8 : Abandoned Bronze Wall, Aeroplate Wall, Chaotic Brick Wall, Mossy Gravel Wall,
  Reinforced Acidplate, Freezeplate, Noctiplate et Redplate Wall). La planche reprend le carré de 32 px du milieu de
  l'image dans toutes ses cases (pas de variantes de bord, comme les blocs). Mossy Gravel Wall n'a pas d'icône d'objet sur
  le wiki : carré de 24 px pris au milieu du mur posé.

## 7. Butin (`tools/gen_loot.py`, `Common/WikiData/WikiLoot.cs`)

- **[choix]** Additif : seuls les objets que le PNJ ne lâche pas déjà sont ajoutés ; chances existantes inchangées.
  **[validé 21/10]** En Full Fandom, les drops du jeu absents de la page sont en plus retirés.
- **[choix]** Chance et quantité du mode normal (Expert : voir les sacs ci-dessous).
- **[choix]** Conditions écrites en note (`{{item|X|note=(After defeating Golem)}}`) : appliquées quand le jeu sait les
  vérifier (`tools/gen_loot.py` → `parse_condition`, `Common/WikiData/WikiLootCondition.cs`) : boss vaincus (vanilla et
  Calamity ; « Hardmode » = Wall of Flesh, « Godseeker Mode » = Moon Lord, « Calamitas » = le clone), biome du joueur
  (Sunken Sea, Space, Jungle, Underworld, Ice), premier kill (compteur du bestiaire, qui compte le kill avant le butin).
  Les autres (boss inventés comme The Starborn Moth, « While supercharged », « Tiers 2 & 3 », « Pacifist Drop »...)
  restent non appliquées. Un drop à condition dans un groupe « One of... » : non appliqué.
  **[choix]** (retour de test du 22/10) Quand le jeu donne déjà l'objet avec une autre condition (Devil Fish : Depth
  Cells après Leviathan dans Calamity, après Calamitas sur le wiki ; Abyss Ore après Golem sans texte), la version du
  wiki remplace la règle du jeu, si cette règle ne donne que cet objet. Dans les deux modes (valeur propre au wiki).
  **[technique]** Lignes `custom` : la quantité écrite après le modèle (« {{item|Depth Cells|...}} (1-2) ») est lue ; un
  objet cité sous deux noms (Scoria Ore / Abyss Ore) n'est plus compté deux fois.
- **[technique]** Valeurs de dégâts du wiki entre 0 et 1 (Stormjaw Staff 0,1) arrondies à 1 : à 0, l'arme ne fait plus
  rien et l'infobulle se dérègle (test du 23/10).
- **[technique]** Premier kill et biome ne se vérifient pas à l'ouverture d'un sac : ces drops tombent directement en
  Expert, même en Calamity++.
- **[choix]** (remarque de l'utilisateur, 22/10) Une condition écrite `| Objet|note=(...)|...` **hors** d'un `{{item}}` est
  un paramètre nommé de l'infobox que le modèle n'affiche pas : sur le wiki affiché il n'y a pas de condition, le drop
  s'applique. Un seul cas : **Abyss Bar** du Desert Scourge (« Post-Golem » invisible), donc 3-8 Abyss Bar dès le
  premier Desert Scourge, dans les deux modes (valeur propre au wiki). Les 210 autres conditions sont dans des
  `{{item|...|note=...}}`, affichées, et restent non appliquées.
- **[technique]** Anciens noms d'objets dans le butin (« Victory Shard » sur la page Treasure Bag) : redirections du
  wiki puis renommages du glossaire (→ Conquest Fragment).
- **[validé 22/10]** Boss en Expert (le wiki se contredit : la page du boss donne des quantités Expert `{{dv}}` sans
  citer de sac, la page « Treasure Bag » donne un autre contenu) :
  - **Calamity++** : le sac reçoit les drops de la page du boss (quantités/chances Expert) et ce que liste la page
    Treasure Bag, en plus de son contenu actuel ;
  - **Full Fandom** : les drops de la page du boss tombent aussi **directement** en Expert (quantités Expert), et le sac
    reste. **[choix]** Son contenu = la section de la page Treasure Bag seulement (le reste du sac est retiré, même
    règle que le butin des PNJ).
  En mode normal, rien ne change : drops de la page du boss, directement.
- **[choix]** Infobox « item drops » des **pages d'objets** (« Twisted Tendril : Wall of Flesh 3-5 100 % ») : ajoutées au
  butin du PNJ quand sa page ne les cite pas (322 drops), même règles (conditions entre parenthèses, Expert, sacs).
  Un PNJ connu **seulement** par ces infobox (Wall of Flesh, Plantera, Zombie...) n'est jamais vidé en Full Fandom :
  ces pages ne listent pas tout son butin. Sources génériques (« any Jungle enemy », « Any Boss ») : pas encore
  appliquées (27). Un nom partagé par plusieurs PNJ (Zombie) : le drop va à une seule variante, celle qui a déjà le plus
  de butin.
- **[choix]** Boss sans sac (mini-boss) : les drops du wiki tombent directement en Expert aussi, dans les deux modes.
- **[technique]** « Déjà donné en normal / en Expert » se lit dans les conditions des règles du jeu (NotExpert / IsExpert).
  Le sac d'une section de la page Treasure Bag est trouvé par son nom, sinon par le boss de la section (« Treasure Coffer »).
- **[choix]** « One of the following N items » : un objet au hasard parmi le groupe, sauf si le jeu en donne déjà un.
- **[choix]** Un seul PNJ par page, celui qui a déjà le plus de butin (vers, alias) ; titres sans nom (« ??? ») ignorés.
- **[choix]** Full Fandom : le **sac au trésor** d'un boss n'est jamais retiré, même si l'infobox du boss ne le cite pas
  (Desert Scourge, Cryogen...) : la page « Treasure Bag » documente le sac de chaque boss et son contenu. Sans ça, un
  boss en Expert ne lâchait plus rien (passe 1.6).
- **[technique]** Lignes `custom` sans chance (sac, relique : « Crabulon Relic ») : objet seulement cité, ni ajouté ni
  retiré. Avant, elles étaient mal lues et avalaient la ligne suivante.
- **[technique]** Le bestiaire est recopié depuis la base de butin après nos changements (tModLoader le remplit avant
  les recettes, donc avant nos ajouts et retraits).

- **[validé 31/10]** **Abyss Bar du Desert Scourge** : la page écrit `note=(Post-Golem)`, paramètre que l'infobox
  n'affiche pas (donc pas de condition sur le wiki affiché, décision du 22/10). En Calamity++, la condition s'applique
  (« After defeating Golem ») : sinon le Taming (mener le Desert Scourge à l'océan, malgré sa vie immense) donnait des
  Abyss Bars (Scoria Bars, post-Golem) dès le début. Full Fandom inchangé. Écrit avec `{{mode}}` sur notre page.

- **[choix]** (31/10) **Sources génériques des pages d'objets** (« any Jungle enemy », « Any Boss ») : règles de butin
  globales, dans les deux modes (comme le reste du butin, additif). « Enemy » = PNJ hostile, ni boss, ni de statue, ni
  bestiole ; « Boss » = boss ; le lieu est le biome du joueur au moment du kill (Blood Moon : l'événement). 15
  appliquées (14 en jeu : le Cent Striker n'existe pas encore) : essences réactives (10 %), Cent Striker (surface,
  0,5 %), Conquest Fragment (un par boss : les anciens
  Victory Shards, qui servent aux objets de lore). Laissées de côté (`reports/loot.md`) : lieux et événements inventés
  (Exosphere, Pot of Pain, Sky Abyss, vagues, Hydrabots), « Any Boss in Revengeance Mode » pour Laudanum, Stress Pills
  et Heart of Darkness (déjà dans les sacs de Calamity, comme l'explique leur page), Catarachedrae (« Hive Mind ou
  Perforators », et objet absent). **[technique]** Pas affichées dans le bestiaire (chaque fiche les listerait).
  **[technique]** Correctif du 31/10 (4000 Noctic Reactive Essences et 1100 Conquest Fragments signalés) : les
  arguments de `ItemDropRule.ByCondition` étaient dans le mauvais ordre (le dénominateur vient avant les quantités) :
  chaque règle générique tombait à 100 %, en 1 à 1000 exemplaires (1 à 10 000 pour le Conquest Fragment). Arguments
  nommés désormais, et l'auto-test vérifie quantités et chances. Autre correctif du même jour : chaque segment d'un ver passe
  par la distribution du butin à sa mort, et Calamity marque « boss » les segments de ses vers boss (Desert Scourge,
  Devourer of Gods, Storm Weaver, Astrum Deus) : un Conquest Fragment par segment, et une chance d'essence réactive par
  segment des vers ennemis. Désormais : jamais les segments (`realLife` vers un autre PNJ) ; « Any Boss » une seule
  fois par combat, au dernier boss qui meurt (Twins, Gardiens, Exo Mechs : un seul), et pas en plus quand la fiche du
  boss donne déjà l'objet (Desert Scourge : un seul fragment) ; « any enemy » exclut aussi les morceaux et serviteurs
  de boss (rangés avec le boss dans le bestiaire, ou parties d'un boss du Taming : Creepers, Perforators...).

- **[choix]** (31/10) **Variantes d'un même nom** (Zombie, Demon Eye...) : le butin d'une page va à toutes les variantes
  qui portent ce nom (5 pages : Zombie 14 variantes, Demon Eye 8, Plague Charger, Necromancer, Crimson Slime Spawn),
  quand ce sont des PNJ
  indépendants : ni boss, ni segments (dontCountMe, IA de ver), et sans compter les parties cachées du bestiaire
  (segment du Bobbit Worm, lanterne du Profaned Energy, et par prudence les variantes cachées comme le Goldfish qui
  marche), qui feraient tomber le butin deux fois. Sinon, un seul PNJ
  (celui qui a déjà le plus de butin, puis pas un segment). Liste : `reports/loot_applied.md`.

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
- **[choix]** Version minimale : 23 PV (= 23 coups, 12 + 10 + 1 de la page), chaque coup retire 1 PV, défense 0, attaques
  (contact, projectiles, piliers) à 1 dégât exactement + Poisoned 5 s.
- **[choix 28/10]** Après chaque coup compté : « MISS » au lieu des dégâts, le joueur est projeté loin de Sans (vitesse
  12, le « BDSM » de la page), puis 1,5 s d'esquive (0,5 s était trop court au test). Les debuffs et les coups qui ne
  passent pas par une attaque (dégâts directs) ne comptent pas. Tête, griffes et jambes ne sont plus créées (la barre de
  vie de Calamity les comptait : 21 023). La barre « vanilla » propre au Ravager (qui ajoute toujours la vie des 5
  parties) est remplacée par la barre standard du jeu.
- **[validé 28/10]** Sprites du wiki dans **les deux modes** : Sans (`MeatmetalPhase1.png`) pour les 12 premiers coups,
  puis le Ravager refait (`Ravaj-removebg-preview.png`) pour les suivants (`Assets/Bosses/`). Taille (28/10) : Sans
  réduit à x0,35 (presque taille humaine, demandé), le Ravager refait à x0,64 (x0,8 puis encore x0,8, tests du 28/10). Hitbox de 40 x 60
  en phase 1 (celle du Ravager en phase 2) ; au 12e coup, Sans se téléporte 600 px à l'opposé du joueur (la page dit
  qu'il se téléporte pour s'éloigner, sans distance). [technique] Jambes, tête et lueur que le corps de Calamity dessine
  lui-même sont remplacées par une texture vide pendant son dessin. [technique] Le 1 dégât est fixé après la réduction de dégâts de Calamity, qui
  le faisait tomber à 0. Pas encore fait : ignorer
  l'invincibilité du joueur, os, Gaster Blasters, écran noir, Handheld Tank (`docs/ROADMAP.md`).

### Portages du 26/10 (`Content/Legacy/`, objets qui débloquent des recettes ou du butin)

- **[choix]** **Psychotic Amulet** : retiré de Calamity en juin 2022, avant 2.0.1.005 (porté depuis le commit qui le
  supprimait). Furtivité de la Shroomite armor de vanilla (même drapeau : ne se cumule pas avec elle, donne la partie
  ranged) + partie spy du vieux code (+20 % de dégâts, +10 % de critique) et +50 % de recul spy (totaux de la page).
  Le ralentissement du vieux code (vitesse divisée par deux pendant la furtivité) n'est pas porté : le wiki n'en parle pas.
  [technique] L'interdiction du bonus de la Vortex armor décrite par la page n'est pas codée (à vérifier en jeu).
- **[choix]** **Disseminator** : comportement du texte et du gif de la page (salve de l'arme + salves d'au-dessus et d'en
  dessous vers le curseur, qui traversent les blocs) plutôt que celui du code de 2.0.1.005 (jusqu'à 8 ennemis visés).
  4 plombs par salve et 70 % des dégâts pour les salves extérieures : chiffres du vieux code, le wiki n'en donne pas.
- **[choix]** **Lazhar** : le wiki dit que l'explosion inflige On Fire! 3 s ; l'explosion réutilisée de Calamity ne le fait
  pas, c'est donc le rayon qui l'inflige à l'ennemi touché.
- **[technique]** **Eutrophic Scimitar** : Eutrophication 0,5 s (infobox) au lieu de 1 s (vieux code). **Astral Bow** :
  classe AstralRepeater (nom de 2.0.1.005), les tirs à bout portant de Calamity n'existent plus.
- **[technique]** **Lead Core** : retiré de Calamity en avril 2022 (porté depuis le commit qui le supprimait).
- **[choix]** **Surgical Glove** et **Sulfuric Scale** : coverings inventés par le wiki, écrits d'après la page
  (`Content/Items/Accessories/`). Surgical Glove : « ne se cumule pas avec la Titan Glove et ses améliorations » = pas de
  bonus true melee quand l'effet de recul de la Titan Glove est actif ; Plague sur les attaques de mêlée et leurs
  projectiles (durées des Notes). Sulfuric Scale : « debuff qui fait perdre de la vie » = régénération négative ; sprite
  du wiki (`Sulfuric_Scale.png`).
- **[choix]** **Flask of Plague** : inventée par le wiki, faite comme les flasks de Calamity (un seul buff de flask à la
  fois) ; Plague 3 s sur les coups de mêlée, de fouet et spy (y compris leurs projectiles). Texte gardé tel quel
  (« Rogue », comme la page). Pas encore dans les caisses de jungle (Notes de la page).
- **[choix]** **Microxodonta** (inventé) : « en vol » = ailes en cours d'utilisation ; la Microbial Cluster amicale reprend
  la texture et le mouvement du PNJ de Calamity, dure 5 s, 300 dégâts fixes (ni bonus, ni critique, ni défense).
  **Plague Balloon** (inventé) : l'image portée est celle de l'objet, la nuée d'abeilles à la mort est de la poussière
  verte.
- **[technique]** **Sea Shell** : retiré de Calamity en juin 2022 (porté) ; +15 % de vitesse (wiki, 10 % dans le vieux code),
  bonus dans n'importe quel liquide (« submerged in liquid »).
- **[choix]** **Mirage Jelly (item)** (inventé) : -5 % de furtivité consommée par attaque furtive (réduction de perte de
  Calamity, qui se cumule avec la Coin of Deceit) ; le seuil de déclenchement ne change pas.
- **[choix]** **Inverted Gar** (inventé) : pêché dans l'Abysse à 5 % (remplace la prise) ; Gar Mana Boost 1 s quand un PNJ
  ou un projectile touche le joueur (pas les pics), mana gratuite pendant ce temps.
- **[choix]** **Glacial Crusher** (retiré en juin 2022, porté) et son debuff **Glacial State** (retiré depuis 2.0.1.005,
  porté) : mouvement du vieux code (x0,9 par tick), défense ignorée pendant l'effet (« defense is shattered », sans
  chiffre), aucun effet sur les boss (règle actuelle de Calamity pour les ralentissements).
- **[choix]** **Trash of Magnus** (inventé) : AverageDamageClass de Calamity (« scales with all your damage stats »,
  comme l'Eye of Magnus), aucun modificateur, éclairs de peste invisibles à traînée verte. **Blow Pipe** (inventée) :
  munition Dart (fléchettes et graines, comme le Blowgun), clic droit = dard de peste à vitesse moitié.
- **[choix]** **The Needler** (inventée) : code de la Needler de Calamity, Plague 1,5 s (durée du Venom de la Needler) ;
  classe ranged de l'infobox (le texte dit « magic gun »). **Disease** (inventée) : épée courte sur la base de celles
  de Calamity ; 3 Plague Seekers à 60 % des dégâts par ennemi tué (le wiki ne donne pas de nombre).
- **[choix]** **Phlegm** (inventé) : non consommable ; flaque de glaires toutes les 6 ticks (2 s, 30 % des dégâts) ;
  éternuement de l'attaque furtive = 6 glaires projetées. **Toxic Wisdom** (inventé) : hiéroglyphe et volute de Pigron
  alternés, faits de poussière (le wiki n'a pas leur image). **Reemergence** : Plague Arrow de Calamity (Malevolence).
- **[choix]** **Poison I-Bee** (inventé) : 3 épines par coup, une graine toutes les 8 ticks en furtif, vélocité 14 (absente
  de l'infobox).
- **[choix]** **Decomposition** (inventée) : ruche-tourelle qui tombe au sol, une abeille de la Plaguenade toutes les
  40 ticks vers un ennemi à moins de 600 px (Plague 3 s des abeilles de Calamity ; le wiki dit 1 s).
- **[choix]** **Metal Ball** (inventée) : boule tant que le bouton est tenu, chaîne de 160 px, pendule simple.
  **Sewer Brewer** (inventée) : charge complète en 1,5 s, foret de vitesse 8 à 20 et taille x1 à x2 ; modificateurs de
  taille interdits (Notes).
- **[technique]** **Terra-cotta** : retirée de Calamity en juin 2022 (portée) ; l'orbe de soin est réécrite (l'outil
  de Calamity est `internal`) : 9 PV au contact, attirée seulement avec Heartreach (Notes).
- **[choix]** (nuit du 29/10) Objets retirés de Calamity avant 2.0.1.005 : portés seulement si leur page les décrit comme
  présents ; une page `{{historical content}}` (30, dont Daylight, Crimsaber, Heart Rapier...) décrit un objet retiré,
  qui ne l'est pas (`legacy_extract.py --list` les signale). Les 9 matériaux historiques déjà générés (Ectoblood,
  Cloaking Gland...) restent : ils servent aux recettes du wiki.
- **[choix]** (nuit du 29/10) **Leeching Dagger**, **Bloody Rupture**, **Ancient Shiv**, **Eutrophic Shank** : épées courtes
  retirées avant 2.0.1.005 (code de 2022-06, `legacy_extract.py` sur la dernière version mensuelle), portées en estoc
  (Rapier + projectile, base commune `LegacyShortswordProj`) au lieu de la hitbox d'objet (Thrust) d'origine : jauge
  Shortstrike et vrais clones de Shortflurry. Wiki plutôt que vieux code : orbes de la Bloody Rupture et nuage de
  l'Ancient Shiv à la **mort** de l'ennemi (le code le faisait à chaque coup) ; orbes qui soignent de 2 PV (le Blood
  actuel de Calamity vole 7,5 % des dégâts : orbe porté, `LegacyHealingOrb`). Étincelle de l'Eutrophic Shank = celle de
  l'Aquatic Discharge (même code). Projectiles de soutien : moitié des dégâts de l'estoc.
- **[choix]** (nuit du 29/10) **Tomatus** = l'ancienne Flameburst Shortsword de Calamity : le Fandom l'a renommée Tomutus
  puis Tomatus (redirections ; le tableau Shortstrike dit « Tomutus »), nom affiché = titre de la page. Explosion = celle
  de Calamity (`FuckYou`), 75 % des dégâts. **Night's Stabber** : énergie sombre = l'orbe commun sans soin, pleins dégâts
  (vieux code) ; Shortflurry : « 5 tile long nightshade vines » = épines de 5 blocs (effet `vine`, mêmes images vertes que
  la Spore Knife) et Cursed Inferno 5 s (`tools/shortstrike_extra.json`).
- **[choix]** (nuit du 29/10) **Caliburn** et **True Caliburn** = les anciennes Excalibur Shortsword et True Excalibur
  Shortsword de Calamity (infobulle identique ; la page True Caliburn dit encore « True Excalibur Shortsword »), portées
  avec **True Night's Stabber** et **Terra Shiv** : rayons horizontaux tirés à chaque estoc (ancien `Shoot` : vitesse de
  l'objet dans la direction du joueur), rayons de lame vanilla (Light Beam, Terra Beam), 7 frames entre deux coups sur un
  même ennemi.
- **[choix]** (nuit du 29/10) Effets de Shortflurry de ces armes (phrases de la page Shortstrike, en données dans
  `tools/shortstrike_extra.json`) : « vertical beams of holy light » (Caliburn) = un rayon de la True Caliburn vers le
  haut et un vers le bas depuis l'ennemi touché, moitié des dégâts ; « Hits spawn a beam » (True Caliburn, True Night's
  Stabber, Terra Shiv) = rayon `ShortflurryBeam` qui part de l'ennemi touché dans le prolongement du coup, perce sans
  fin, à la longueur de la page (20, 15, 30 blocs), avec l'image du rayon de l'arme ; en route : colonnes de lumière
  tous les 4 blocs (True Caliburn, « as it travels » sans distance : [choix]), boules de feu maudites vanilla tous les
  5 blocs (True Night's Stabber) ; « heals nearby players » = soigne le joueur qui l'a lancé ; Terra Shiv : 3 rayons à
  ±25° au bout, moitié de la longueur, 20 % des dégâts, Ichor 20 s (durée de la page), +5 PV par coup. Projectiles
  lâchés : moitié des dégâts du rayon.
- **[choix]** (nuit du 29/10) Objets simples retirés avant 2.0.1.005, portés tels quels avec les valeurs du wiki : Blood
  Relic (non consommable comme le dit son infobulle, bien que l'infobox dise « consumable = yes »), Wulfrum Axe, Wulfrum
  Hammer, Waraxe, Warblade, Corrupted Crusher Blade, Crimson Crusher Blade, Mandible Claws, Mandible Bow (sans le tir à
  bout portant, mécanique retirée de Calamity et absente de la page). Debuff **War Cleave** porté : -15 défense, réduction
  de dégâts de Calamity x0,75 (compensée sur les dégâts finaux, `WarCleaveNPC`). `use` du wiki = useAnimation, useTime
  de l'ancien code pour les outils.
  **Raider's Glory** : « These arrows also cause enemies to drop more money » = Midas 10 s sur les flèches (l'ancien code
  ne tirait que des flèches d'Ichor accélérées ; le wiki fait foi). **Bittercold Staff** porté du code 1.3 (2022-02).
  **Shadowdrop Staff** : gouttes qui ne percent pas (« cannot pierce enemies », la page ; le code les faisait percer),
  tir réécrit plus simplement (3 gouttes au-dessus de l'écran, entre le joueur et le curseur). **Hell Burst** : pointes en
  chaîne (IA Vilethorn), immunité par type (8 frames) au lieu de l'ancien `immune = 8`.
- **[choix]** (29/10, lot 3) **Crystal Blade** : `use` 24 du wiki (25 dans le code). **Temporal Floe Sword** : la page
  décrit une horloge où l'aiguille des heures tourne de 15° par heure (cadran de 24 h, midi en haut) et celle des minutes
  de 6° par minute ; le code de 2022 utilisait un cadran de 12 h. Le wiki fait foi. Glacial State et Frostburn aux durées
  de l'infobox. **Sylvan Slasher** : boutique du Bandit de Calamity (`LegacyBanditShop`), 5 platine après Providence ;
  `CanUseItem` empêche deux entailles à la fois (arme canalisée) ; poussières et tir du code de 2022. **Marnite Spear** :
  lance de Calamity (`BaseSpearProjectile`) en `TrueMeleeDamageClass`.
- **[technique]** (29/10, lot 3) Mis de côté parce que **remplacés** dans le même commit de Calamity (renommage git du
  fichier, objet refait) : Seabow → Reed Blowgun, Redtide Sword → Urchin Mace, Seashell Boomerang → Fishbone Boomerang,
  Urchin Spear → Redtide Spear (lui-même retiré depuis), et Shroomerang **renommé** Infested Clawmerang. À traiter avec
  les remplacements `LegacyName` (décision en attente dans `docs/TESTS.md`).
- **[choix]** (29/10, lot 4) **Animus** : probabilités exactes de la page (l'ancien code tirait sur 195 valeurs, à peu
  près les mêmes) ; Brimstone Flames 5 s de l'infobox (le texte dit 100 s). Multiplicateur dans `AnimusPlayer` (le champ de
  CalamityPlayer n'existe plus). **Blade of Enmity** : « 25 % des bonus de vitesse de mêlée » par `UseSpeedMultiplier`.
  **Brimlash Buster** : l'infobulle (« 50% chance to do triple damage ») passe avant l'ancien code (x2 une fois sur 3,
  sur le coup suivant) ; le second tir du texte (« four brimstone bolts to the left, right, and diagonals ») n'a jamais
  existé dans le code de Calamity : ajouté tel quel (traits `Brimlash2` à gauche, à droite et en diagonale vers le haut).
  **Phoenix Blade** : flammes de soin de 3 s (page ; 1,75 s dans le code), explosion `FuckYou` de Calamity avec On Fire!
  3 s (`LegacyHitDebuff`, l'explosion de Calamity n'en inflige pas).
- **[choix]** (29/10, lot 4) Recettes ajoutées à **notre wiki** : la page de l'arme affiche une recette, mais aucun
  registre ne la contient (Brimlash Buster, Phoenix Blade), ou seul `Recipes_Historical_register` l'a, avec la station
  disparue Particle Accelerator (Blade of Enmity). On reprend la recette du code de 2022 (ingrédients et station), inscrite
  dans `Recipes_Hardmode Anvil_register` (Blade of Enmity, Phoenix Blade) et `Recipes_Ancient Manipulator_register`
  (Brimlash Buster). Le registre historique reste ignoré par `gen_recipes.py`. Même chose pour la **Fulguration Halberd**
  et la **Mariana** (lot 5, `Recipes_Hardmode Anvil_register` ; Lumenite = Lumenyl, son nom affiché en 2022).
- **[choix]** (29/10, lot 5) **Mariana** : la page passe avant le code (4 ou 5 bulles au lieu de 2 ou 3, 3 ennemis au lieu
  d'un, ~2 s au lieu de 3) ; la tête chercheuse de l'ancien code est retirée, la page n'en parle pas (« travel for a long
  distance »). **Aquatic Dissolution** : traverse les blocs pendant 1 s comme le dit la page (l'ancien code : 20 frames).
  **Trinity** : les 4 traits ont la même couleur, tirée au hasard (ancien code), en dégâts de mêlée. **Fulguration
  Halberd** : vrai corps à corps (`TrueMeleeDamageClass`) pour l'épée et la lance, immunité locale de 8 frames pour la lance.
- **[choix]** (29/10, lot 6) **Impaler** : « instantly kills vampires » = mort immédiate (`SetInstantKill`) ; explosion
  `FuckYou` avec On Fire! 3 s. **Spectre Rifle** : le zoom au clic droit est celui de la lunette de visée (`player.scope`).
  **Star Cannon EX** : l'étoile astrale de Calamity n'inflige pas Astral Infection quand elle fait des dégâts à distance ;
  on l'ajoute (2 s, infobox). Les tirs à bout portant de Calamity 2022 (`canFirePointBlankShots`) ne sont pas repris,
  la page n'en parle pas. Recette de l'Impaler ajoutée à notre wiki (code de 2022 ; Cruptix Bar = Scoria Bar, appelée
  Abyss Bar par le wiki). [technique] `LegacyHitDebuff` : debuff ajouté par le wiki à un projectile réutilisé qui n'en
  inflige pas (explosions, étoiles).
- **[choix]** (29/10, lot 7) **Hadopelagic Echo** : la page passe avant le code. L'onde va de 5 % à 100 % des dégâts et
  de la taille en 1 s (le code : de 0 à 100 % en 1/6 s) ; la recharge des échos est de 2 s (le code : 1 s, à cause des
  mises à jour doubles). Rareté 16 (Hot Pink) de l'infobox. **Bouncing Betty** : boutique du Bandit après un boss
  mécanique (`LegacyBanditShop`, prix de l'objet) ; les étincelles (`TotalityFire` de Calamity) gardent leur Hellfire et
  reçoivent On Fire! 3 à 6 s (infobox).
- **[choix]** (29/10, lot 8) **Latcher Mine** : explosion 3 s après s'être collée (page et infobulle ; le code : 2 s) ;
  6 flammes projetées comme le dit la page (le code : 4, ou 7 en attaque furtive), dont la moitié environ deviennent des
  éclats en attaque furtive ; On Fire! 10 s sur les flammes (infobox). Collage réécrit (la fonction de Calamity est
  interne). Vendue par le Bandit (`LegacyBanditShop`). **Quasar** : l'attraction vient des couteaux, comme dans le code
  (la page l'attribue à l'explosion) ; explosion `RadiantExplosion` de la Radiant Star actuelle, qui fait déjà pleuvoir
  les étoiles ; couteaux éclatés sans tête chercheuse (page).
- **[choix]** (29/10, lot 9) **Purified Jam** : Invincible = esquive de tous les coups (`FreeDodge`) et immunité aux
  debuffs du modèle « Immunity item debuffs » du wiki (« Distorted » n'existe plus ; « Oozed » = Ogre Spit).
  [technique] Nouvelle condition de butin « Once per player and world » (`gen_loot.py`, `WikiLootCondition`) : clé PNJ +
  note + nom du personnage, enregistrée avec le monde (`WikiLootOnceSystem`), pour que le serveur la vérifie.
  **Revivify Potion** : soin de 1/15 des dégâts après le coup (`OnHurt`), donc rien sur un coup mortel (Notes).
  **Marked Magnum** : `AverageDamageClass` de Calamity (« scales with all your damage stats »), critique forcé à 0.
- **[choix]** (30/10, retours de test) **Aftershock** : le rocher tombe vers l'ennemi touché (le code de 2022 visait le
  curseur ; la page dit seulement qu'il tombe du ciel). **Astral Bow** : taille 0,8 en main. **Trash of Magnus** : tenu
  comme un bâton (`Item.staff`), son sprite étant dessiné en diagonale. **Poison I-Bee** : 3 disques en vol au plus
  (« rapidly throws disks » ; sans limite, ils s'empilaient à l'infini). **Decomposition** : la ruche se pose sur les
  plateformes. **Plague Balloon** : la mort lâche une nuée d'abeilles vanilla teintées de vert, sans dégâts
  (`PlagueBalloonBee`), en plus de la poussière.
- **[validé 30/10]** Objets que Calamity a **remplacés ou renommés en les refaisant** (`LegacyName`, renommage git du
  fichier, avant comme après 2.0.1.005 : Seabow → Reed Blowgun, Lazinator, Frigid Bulwark → Frozen Shield, Warbanner of
  the Sun...) : **notre portage en Full Fandom, l'objet actuel de Calamity sous le nom du wiki en Calamity++** (règle des
  refontes du 23/10). Un simple renommage sans refonte reste l'objet de Calamity sous le nom du wiki, dans les deux modes.
- **[validé 30/10]** **Arcanum of the Void** : effet décrit par la page (barrière du vide, énergie sombre), pas celui de
  2022. **Exo Gladius** : porté avec sa page (suite à voir avec l'utilisateur). **Microcell Cut** : créée, d'après la
  Disease dont elle s'inspire.
- **[choix]** (30/10) **Arcanum of the Void** (effet de la page) : barrière = poussière violette autour du joueur
  quand elle est prête ; coup annulé (`FreeDodge`), explosion de 200 dégâts (160 px) et 4 énergies sombres de 25 dégâts
  qui touchent toutes les 0,5 s (4 × 50 = « ~200 DPS ») et cherchent les ennemis 4 s ; dégâts génériques ; recharge
  `GlobalDodge` de Calamity (90 s, partagée avec les esquives) ; ajouté au sac du Ceaseless Void sur la page Treasure Bag
  de notre wiki (sa page dit qu'il s'y trouve ; en Full Fandom, le contenu des sacs vient de cette page).
  **Exo Gladius** : **[validé 30/10]** pas d'attaque au clic droit (la parade de la page est retirée) ; le renvoi des
  projectiles ennemis à moins de 64 px vers le curseur se fait seulement pendant le Shortflurry (solo seulement : TODO
  multijoueur) ; les lignes de parade de l'infobulle sont remplacées par « Shortflurry attacks deflect projectiles ».
  Shortflurry : « every 5th strike... 8 healing exo comets » = nouveau paramètre `every` des effets `spawnOnStab`
  (compteur d'estocs remis à zéro à chaque Shortflurry) ; **[choix]** 8 comètes (`ExoGladComet` de 2022) à 50 % des
  dégâts, dispersion 0,35 rad, 2 PV rendus par comète qui touche. Explosion = dégâts de l'estoc, 96 px, image du wiki ;
  Miracle Blight 0,5 s. Sprite du wiki à sa taille (64×68 ; réduit de moitié par erreur le 30/10, il paraissait plus petit). **Microcell Cut** : créée d'après la Disease (estoc,
  3 étoiles à 75 % par ennemi tué) ; sprite du wiki à sa taille d'origine, recadré (30×40, taille de la Disease :
  réduit de moitié, il était trop petit) ; rareté « C4 » du wiki créée (`RarityC4`, violet de son image) ; debuff **Ancient
  Astral Blight** créé d'après sa page (Astrum Deus et ses armes ne l'infligent pas encore).
- **[choix]** (30/10) Précision de la règle des objets remplacés : quand le wiki a **aussi** une page pour l'objet qui
  remplace (Reed Blowgun, Urchin Mace, Fishbone Boomerang), il connaît les deux ; ancien et nouvel objet existent alors
  dans les deux modes (rien n'est masqué). La règle « portage en Full, objet actuel sous le nom du wiki en Calamity++ »
  ne vaut que si le nouvel objet n'a pas de page.
  Armes de l'océan d'avant la refonte de mai 2022, dans les deux modes : **Seabow** (gerbe
  d'eau à 40 % et à demi-vitesse de la flèche), **Redtide Sword** (tourbillon de 2022, `LegacyRedtideWhirlpool` :
  celui de Calamity a changé), **Seashell Boomerang** (coquillage de Calamity tiré toutes les 20 frames vers l'ennemi le
  plus proche en furtif ; la fonction d'origine est interne), **Urchin Spear** (Poisoned 10 s de l'infobox, le dard de
  Calamity reçoit Venom 3 s), **Urchin Flail** (Sea King, 2 or ; l'épine ne colle plus aux ennemis : fonction interne).
  Shroomerang = Infested Clawmerang et Aero Dynamite = Skynamite : même infobulle, simples renommages, rien à porter.
- **[technique]** (31/10) Remplacements que le wiki ne connaît pas : liste explicite `replacements` de
  `tools/glossary.json` (page du wiki → classe actuelle de Calamity). `gen_baseline.py` masque l'objet actuel en Full
  Fandom et notre portage en Calamity++ ; `gen_localization.py` donne le nom du wiki à l'objet actuel. La Marnite Spear,
  remplacée par le Marnite Deconstructor (sans page), y entre : elle suit désormais cette règle.
- **[choix]** (31/10) Portages de cette liste, page avant code : **Cursed Capper** 35 dégâts ; **Luminous Striker** ne
  traverse pas (« doesn't pierce » ; 4 ennemis dans le code), 149 dégâts ; **The Reaper** : faux de 2.0.1.005
  (`LegacyReaperProjectile`, celle de Calamity a été refaite), 12 coups puis disparaît, Crush Depth 10 s ;
  **Warbanner of the Sun** : code de 2022 (480 px) ; **Rusty Medallion** : 20 % de chances par tir de 3 gouttes à 30 %
  des dégâts de base, adoucies au-delà de 30, recharge de 25 frames du code, gouttes du Flak Toxicannon de 2022
  (`ToxicannonDrop`) ; TODO coffres rouillés ; **Alchemical Flask** : effet de la page (Plague sur le porteur, +2 %
  de dégâts), pas celui de 2022 ; **Red Bow** : absent du code de 2.0.1.005 (déjà le Ghost Bracelet), recréé d'après
  la page (+10 discrétion max, +3 défense, aggro -200, pêche dans le Dungeon avant Skeletron, 1 chance sur 20).
- **[choix]** (31/10) Objets retirés ou remplacés par un objet que le wiki connaît aussi, dans les deux modes :
  **Wulfrum Blade / Bow / Staff** (Bow 11 dégâts de la page), **Lazinator** (lasers vanilla, 3 contacts, le vert à
  95 % de vitesse), **Charged Dart Blaster** (trait de Calamity `ChargedBlast` ; clic droit porté, `ChargedBlast3`),
  **Acceleration Round** (accélération plafonnée à 20 mises à jour), **Cadance Potion** (spawn -56 % = intervalle
  ÷ 0,44, max ×0,56 ; Heartreach = portée des cœurs), **Crumbling Potion** (page existante, ingrédient de la Shattering)
  et **Shattering Potion** (Armor Crunch 2, 4 ou 6 s au hasard ; « Spy » = rogue), **Frigid Bulwark** (Paladin's Shield
  et Ice Barrier vanilla ; -5 % de plus sous 15 % de vie ; recette de 2022 ajoutée à notre wiki). **Marnite Bayonet** :
  dans `replacements` (le Marnite Repulsion Shield n'a pas de page).
- **[technique]** (29/10, lot 9) **Arcanum of the Void** mis de côté : la page décrit une barrière du vide qui explose
  en énergie sombre à tête chercheuse ; le code de 2022 renvoyait les projectiles. Effet inventé ou refait, à trancher.
- **[choix]** (29/10, lot 10) **Dualpoon**, **Triploon** : code de 2022 en une base commune (`MultiHarpoon`) ; recettes
  de 2022 ajoutées à notre wiki (`Recipes_Hardmode Anvil_register`). TODO : l'**Agent**, PNJ inventé par le wiki, les
  donne aussi (pas encore créé).
- **[choix]** (nuit du 29/10) **Wulfrum Leech Dagger** (objet inventé par le Fandom) : épée courte consommable en estoc,
  1 dégât (plafond), pas de critique, retrait de 0,5 % de la vie max par un second coup sans défense (`SimpleStrikeNPC`,
  comme la Coldheart Icicle de Calamity) ; pas sur les segments de ver ni les mannequins (page) ; Shortflurry : effet
  `unconsumable` 3 s. Sprite du wiki.
- **[à trancher]** Plague Hive : demande Alchemical Flask, que Calamity a remplacé par l'Alchemical Decanter
  (`[LegacyName]`) ; dépend de la décision sur les remplacements `[LegacyName]` (section 2).

### Portages du 31/10 (`Content/Legacy/Items/LegacyBatch1031b.cs`) : objets qui débloquent des recettes du wiki

- **[choix]** **Bloody Edge → True Bloody Edge → Terra Edge**, **Magnum → Lightning Hawk** (+ Magnum Rounds) et
  **Plague Hive** (l'Alchemical Flask dont il dépend est porté depuis le 30/10). Pages actuelles, recettes du wiki.
  Valeurs des pages : Bloody Edge soigne 2-3 PV à 50 % (le code : à chaque coup) ; True Bloody Edge 1-6 PV à 50 %
  (le code : 3-5) ; Terra Edge 2-3 PV à chaque coup, rayon à 75 % ; usages 22/19/16 du wiki.
- **[technique]** Magnum et Lightning Hawk : armes sans classe, 3 tirs par combat de boss (compteur de Calamity
  `timesUsed` disparu : refait sur l'objet, partagé par les exemplaires du même pistolet), coups critiques fixes 46 % et
  56 % sans bonus (page), +1,33 % des PV max de la cible par balle ; son du revolver du jeu (celui de Calamity a
  disparu). Magnum Rounds vendues par l'Arms Dealer quand on a l'un des deux pistolets (page).
- **[choix]** Plague Hive : effets de la page ; attaques qui infligent Plague et Plague Seekers par l'effet `alchFlask`
  de Calamity (celui de l'Alchemical Decanter) ; abeilles quand on est touché (Hive Pack) ; aura de 40 blocs de diamètre
  qui inflige Plague (« 20 damage per tick » lu comme les dégâts de Plague, posée toutes les demi-secondes) ; immunité
  à Plague du code de 2022 gardée.
- **[choix]** **Catastrophe Claymore → Devastation** (`LegacyBatch1031c.cs`) : éclairs d'énergie (Claymore) et décharges
  galactiques (Devastation : une depuis la lame, 4 salves de trois depuis le ciel à moitié des dégâts), invisibles
  (poussière), debuffs et durées des infobox ; usages 22 et 23 du wiki (23 et 24 dans le code).
- Laissé de côté : Holiday Halberd (page « historique »).
- **[choix]** **Alpha Virus** (`Content/Legacy/Items/Weapons/Rogue/AlphaVirus.cs`, porté le 31/10 au soir) : code de
  2022 (cellule à aura de 100 px qui touche à travers les blocs à 1/5 des dégâts sans recul, 6 seekers à la mort, frappe
  furtive qui ralentit et sème des seekers accrochés ; dégâts / 5 contre Storm Weaver, comme le vieux code), valeurs de
  l'infobox (242 dégâts, use 30, rareté 11, 24 or). Recette du wiki (Epidemic Shredder, 5 Luminite Bar, 25 Plague Cell
  Canister) : le Bloodstone Core n'était que dans la recette du vieux code, et l'avoir noté comme bloquant (31/10)
  était une erreur. Débloque la recette du wiki de Celestus.
- **[validé 31/10]** **The Insacred Texts** (`Content/Items/Weapons/Magic/TheInsacredTexts.cs`) : le clic droit n'ouvre
  pas de fanfiction AO3 classée « E » (AO3 n'est plus là pour Calamity, et l'explicite n'est pas le but) : il affiche en
  rose (HotPink) dans le chat, pour le joueur seul, une des 12 phrases écrites par Claude qui finissent par « ...then they
  kissed » (couples de boss et de PNJ de Calamity, `InsacredTexts.Fic0..11` dans la localisation) ; sans mana ni
  recharge. Page du wiki et infobulle modifiées en conséquence. Dans les deux modes (contenu nouveau).
  **[choix]** Clic gauche : vise les 3 ennemis les plus proches du curseur (à moins de 50 blocs du joueur) ; inutilisable
  sans cible (pas de mana perdue) ; le portail suit la cible 0,5 s, le bras la touche une fois (450 dégâts) puis la tire
  vers le portail 40 ticks (ni les boss, ni les ennemis sans recul) ; recharge de 25 s par joueur, affichée dans
  l'infobulle. Portail et bras : textures de Calamity teintées (`docs/SPRITES.md`).
- **[technique]** Les armes Rogue (`RogueWeapon` de Calamity) ont leurs textes sous `Items.Weapons.Rogue` : ceux de la
  Catastrophe Claycore et du Warglaive avaient été écrits sous `Items` et n'apparaissaient pas (infobulle vide). Corrigé,
  et l'auto-test vérifie qu'aucune arme Rogue du mod n'a d'infobulle vide.

### Armored Digger et Draedon's Remote (`Content/Legacy/`, 31/10, pour le Taming d'Astrum Aureus)

- **[choix]** **Armored Digger** porté (retiré de Calamity en 2022, code de 3ec8f553ed) : ver de 30 segments, mini-boss
  des cavernes après Plantera, IA de 2022. Stats des infobox du wiki (tête 90 dégâts / défense 15 / 15 % de réduction,
  corps 70 / 40 / 25 %, queue 55 / 55 / réduction de 2022 30 %, absente de l'infobox ; 20 000 PV partagés) ; débris
  (Saucer Scrap) du corps. Apparition de la page (« Notes ») : 1 sur 100 000 par tick et par joueur, multiplicateurs
  de la page (dont Death x1,33, absent du code) ; **60 s** de délai (page) au lieu des 10 min du code ; zones exclues du
  code (jungle, Sunken Sea, Abyss, enfer, donjon) plus le temple de la page. Butin de l'infobox (Draedon's Remote 1/3,
  Demonic Bone Ash, Mysterious Circuitry, Dubious Plating) ; les 3 armes du groupe n'ont pas de page : pas portées.
  Pas de bannière (celle de 2022 dépend de la table des bannières de Calamity). **[technique]** `WikiLoot` choisit,
  entre PNJ du même nom sans butin, celui qui n'est pas un segment (sinon le butin allait sur le corps).
- **[choix]** **Draedon's Remote** porté (avril 2022) : non consommé ; la nuit avant Plantera, appelle les trois boss
  mécaniques à la fois, renforcés d'après la page (Twins et Destroyer x1,8 PV, Destroyer x1,5 en taille, Probes x1,6
  PV et x1,2, tête de Prime x1,45, bras x0,8 ; +20 % de dégâts subis tant qu'un d'eux vit ; 8-16 Mysterious Circuitry
  et Dubious Plating chacun, le dernier des Twins seulement). **[technique]** Le « mode Mayhem » de Calamity n'existe
  plus : refait (`DraedonMayhem`) ; ce butin, donné par la page du remote et pas par celles des boss, est protégé du
  nettoyage du Full Fandom (`WikiLoot.Keep`). Rareté et usage du wiki (Orange, 45). TODO : de jour, la Mechanical
  Horde (événement inventé). Rien ne se passe si un boss mécanique est déjà là.

### Ensemble « War » (`Content/Items/Weapons/WarWeapons.cs`, 31/10)

- Créés d'après leurs pages (modèle {{War}}, armée gobeline ; Waraxe, Warblade et War Cleave étaient déjà portés) :
  **Warbow** (Goblin Archer : convertit les Wooden Arrows en War Arrows, War Cleave 10 s), **War Staff** (Goblin
  Sorcerer : boule d'énergie grise, War Cleave 10 s), **Warbell** (Goblin Peon : se manie comme une épée, dégâts
  d'invocation ; un coup fait sonner une cloche, +10 % de dégâts d'invocation 5 s et War Cleave 5 s), **Warglaive**
  (Goblin Thief : glaive lancée soumise à la gravité, 4 coups, War Cleave 5 s ; frappe furtive plantée dans les blocs
  10 s ou 4 coups), **War Arrow** (Goblin Archer : War Cleave 4 s). Stats des infobox, sprites du wiki.
- **[choix]** War Arrow : l'infobox dit « spy », mais une flèche tirée par un arc fait des dégâts à distance ; la
  vitesse 1,5 du Warbow est celle de l'arc (elle s'ajoute à celle de la flèche) ; le bonus de la Warbell n'a pas
  d'icône (la page ne nomme pas de buff) ; la boule du War Staff n'a pas de sprite sur le wiki (poussière grise).

### Autres objets du butin du wiki (31/10)

- **Porpoise Pistol** (Dolphins en Hardmode) : trait d'énergie bleu (« Proporse Bolt », sprite du wiki) qui rebondit
  et traverse 2 ennemis en ignorant une partie de leur invulnérabilité. **[choix]** Pas de munition (l'infobox n'en
  donne pas), 3 rebonds, 3 ennemis touchés, invulnérabilité locale de 10 ticks.
- **The Food Chain** (Colossal Squids) : fouet ; un coup donne Well Fed 10 s au joueur. **[choix]** Planche des
  segments provisoire (anneaux de calamar dessinés par Claude, `docs/SPRITES.md`) ; dessin et réglages de l'ExampleWhip
  de tModLoader (18 segments, portée x1,1, dégâts -20 % par ennemi traversé).
- **[technique]** `gen_loot.py` lit les quantités « 25-68/{{expert|37-120}} » (War Arrow, Warglaive et 6 autres).

### Frost Blade et Flare Blade (`Content/Items/Weapons/Melee/ElementBlades.cs`, 31/10)

- Épées du wiki qui servent à fabriquer la Flarefrost Blade : orbe à tête chercheuse (Frostburn ou On Fire! 2 s), la
  lame inflige le même debuff 3 s ; stats des infobox (la page de la Flare Blade recopie le texte de la Flarefrost
  Blade : infobox prise). **[choix]** Orbe inspirée du projectile Flarefrost de Calamity (poussière, 3 s, tête
  chercheuse à 250 px), réduite à un élément.

### The Princess (`Content/Items/Weapons/Magic/ThePrincess.cs`, 31/10)

- Doublon du wiki de The Prince (arme de Calamity, page séparée) avec ses propres valeurs (166 dégâts, 12 mana, Turquoise,
  24 or) et Moon Burnt en plus (3 s boule, 1 s flammes). **[choix]** Projectiles de The Prince réutilisés (comportement
  identique à la page), marqués quand ils viennent de The Princess pour ajouter Moon Burnt.

### Famille Catastrophe (`Content/Items/Weapons/CatastropheClayWeapons.cs`, 31/10)

- **Catastrophe Clayscore** (arc), **Clayspore** (baguette), **Claycore** (bombe rogue) : tirent les éclairs d'énergie
  de la Catastrophe Claymore portée (rouge, bleu ou jaune, mêmes debuffs), dans leur propre classe de dégâts.
  **[choix]** Clayscore sans munition (infobox) ; Claycore non consommée (pile de 1), bombe soumise à la gravité qui
  explose à l'impact en 3-4 éclairs (4-8 en frappe furtive) d'une même couleur, à 50 % des dégâts chacun (la page ne
  donne pas de valeur). Claybore et Claysiphonophore (autres membres) : pas encore faits.

### Elemental Claymore (`Content/Items/Weapons/Melee/ElementalClaymore.cs`, 31/10)

- Eon Beam d'une couleur au hasard (rose, vert, bleu, orange : invulnérabilité locale 6, 5, 4, 3 ticks), traverse 2
  ennemis et les blocs ; 4 orbes élémentaires peu précises vers le curseur (50 % des dégâts, tête chercheuse,
  traversent les blocs) ; Elemental Mix 2 s partout. **[choix]** Les orbes sont les Eon Bolts de Calamity (même
  comportement) ; le rayon reprend la forme du rayon de la Terra Blade, teinté de sa couleur.

### Outils du wiki (`Content/Items/Tools/WikiTools.cs`, 31/10)

- Créés d'après leurs infobox parce qu'ils débloquent une recette du wiki : Windslasher, Gusthammer, Lithos Pickaxe,
  Impact Adjuster, Crayfish Pickaxe (hache et marteau malgré son nom), Carapace Hammer, Astral Hammer, Astral Axe.
  « toolspeed » = vitesse d'utilisation (useTime), « use » = animation ; puissance de hache du wiki / 5 dans le jeu.
  **[choix]** Astral Hammer : portée +3 (« un bloc de moins que les Lunar Hamaxes », +4) ; Lithos Pickaxe et Impact
  Adjuster : On Fire! 3 s au contact (la page ne donne pas de durée). Large Father laissé de côté (lâché par The Wall
  of Bronze, boss inventé, et attaque spéciale à faire).

### Slime Puppet Staff (`Content/Projectiles/Summon/SlimePuppetBall.cs`, rework de la page, 31/10)

- **[choix]** **Full Fandom seulement** (Calamity++ garde les marionnettes de Calamity) : bouton maintenu, une boule de
  slime suit le curseur (12 px/tick, comme un Magic Missile) ; relâché, elle se fige ; 10 au plus (la plus ancienne
  disparaît), sans emplacement de sbire ; boules figées à 2,5 blocs ou moins (bord à bord) reliées par une bande de
  slime. Boules et liens : 0 dégât ; un ennemi qui marche (soumis aux blocs, pas un boss) qui y entre est arrêté
  (position horizontale d'avant). Elles disparaissent si le joueur meurt ou s'éloigne de plus de 190 blocs. Sprite du
  wiki (« Slime Puppet » : boule avec yeux quand elle suit, sans une fois figée). Dégâts 0 et mana 5 : infobox.

### Augar of the Elements (renommage, 02/11)

- **[technique]** « Augar of the Elements » (orthographe du wiki) est l'Augur of the Elements de Calamity, renommé
  Augur of the Void par le commit « Elemental weapon renames, EXCEPT Lance and Disk » : l'index ne le reliait pas à cause
  de l'orthographe. Ajouté aux renommages (`tools/glossary.json`) : il reprend le nom, les stats, le sprite et la recette du
  wiki comme les autres objets renommés.

### Exo Freeze et Gravitonomy Pike (02/11)

- **[choix]** **Exo Freeze** (`Content/Buffs/ExoFreeze.cs`) : debuff retiré de Calamity (son texte est resté). La cible
  ne peut plus bouger (sa vitesse est annulée après son IA) ; rien pendant la Boss Rush (page) ; les boss n'y sont pas
  sensibles (la page dit « some bosses » sans les nommer).
- **[choix]** **Gravitonomy Pike** (`Content/Items/Weapons/Melee/GravitonomyPike.cs`) : lance des valeurs de l'infobox
  (257 dégâts, use 18, rareté 15). Chaque coup tire un rayon qui traverse les murs et cherche faiblement ; à moins de 8 blocs
  d'un ennemi ou en le touchant, il se divise une fois en 3 rayons à 50 % qui cherchent les ennemis ; la lance attire les
  ennemis à moins de 25 blocs vers celui qu'elle touche (pas les boss). Les quatre debuffs de la page ; invulnérabilité
  propre à chaque projectile (« ignores immunity frames »). Recette du wiki (WikiRecipes).

### Accessoires du 02/11 (`Content/Items/Accessories/`)

- **[choix]** **Ferro Claws** : valeurs de l'infobulle ; les éclats (2, à tête chercheuse, 30 blocs, 25 % des dégâts) partent
  au début de chaque coup d'une arme de mêlée qui frappe elle-même (ni fouets, ni armes qui ne font que lancer un
  projectile), s'il y a un ennemi à moins de 30 blocs ; éclats sans sprite (fumée sombre). Obtenues dans le Shimmer avec
  un Mechanical Glove après le Moon Lord (code : la table de la page Shimmer n'est pas lue par le générateur).
- **[choix]** **Icicle Ring** : -15 PV/s tant que la vie dépasse le tiers de la vie maximale (arrondi au-dessus), mana à
  0, +50 % de dégâts magiques (page). Sans source tant que le Frost Imp n'existe pas (TODO).
- **[choix]** **Disorderful Soul Artifact** : réduction des dégâts mise à 0 après tous les autres effets ; vitesse
  (course, accélération, bottes) ×2 ; vitesse d'attaque +200 % pour toutes les classes. Recette du wiki.

### Gullet Worm (`Content/NPCs/Critters/GulletWorm.cs`, 02/11)

- **[choix]** Bestiole de la Blood Moon (page) : 10 % de chances en cassant un tas de cailloux ou de débris naturel (petit
  ou grand) pendant une Blood Moon (« Objects that have a chance to spawn critters » : interprété ainsi) ; IA du ver ; un
  seul dessin (celui de l'objet). Objet : appât de puissance 120, pile de 999 (page). « Higher chance to catch enemies
  during a Blood Moon » : 1 chance sur 6 de plus de remonter un ennemi de la Blood Moon du jeu (Zombie Merman, Wandering
  Eye Fish ; en Hardmode aussi Goblin Shark et Blood Eel). Cage (Gullet Worm Cage) pas encore faite.
- **[choix]** **Worm Meal** (`Content/Items/Accessories/WormMeal.cs`) : valeurs de la page ; les vers parasites (sprite du
  Gullet Worm, l'image de la page) sortent des coups des sbires et de leurs projectiles, courent et sautent, s'accrochent
  et mordent toutes les 40 ticks pendant 2 s puis « explosent » sans dégâts (la page n'en donne pas) ; 5 s sans cible :
  même fin. 5 vers au plus.

### Munitions portées (`Content/Legacy/Items/LegacyAmmo1102.cs`, 02/11)

- **[choix]** **Arctic Arrow**, **Enhanced Nano Round** (et ses Nanomachines), **Superball Bullet** : code de 2.0.1.005,
  valeurs des infobox (dégâts, pile de 9999, prix, rareté), recettes du wiki. **[technique]** Le bonus « point blank » du
  vieux code n'existe plus dans Calamity 2.2.2 : retiré. Laissées : Explosive Shotgun Shell (seulement pour l'ancienne
  Hydra, qui n'en utilise plus), Frostspark Bullet (page « historique »), Grenade Shells (pas dans les candidats).

### Flahmes, Flah Round, Flarrow (02/11)

- **[choix]** **Flahmes** (`Content/Buffs/Flahmes.cs`) : -500 PV/s (page) ; « damage reduction reduced by 35% » lu comme 35
  points de réduction en moins (jamais sous 0), appliqué par-dessus le calcul de Calamity.
- **[choix]** **Flah Round** (`Content/Items/Ammo/FlahAmmo.cs`) : explose et rebondit sur un ennemi ou un bloc, 9 fois ;
  petites explosions de 3 blocs à 33 % des dégâts, grande explosion finale de 8 blocs à 150 % avec Flahmes 3 s (page).
  **Flarrow** : cherche lentement 2 s puis fonce à 30 vers sa cible à travers les blocs ; sans cible, continue tout droit.
  Valeurs des infobox (rareté 16), recettes du wiki (4999 par Flahium Bar, Draedon's Forge).

### Potions portées (`Content/Legacy/Items/LegacyPotions1102.cs`, 02/11)

- **[choix]** **Holy Wrath**, **Penumbra**, **Profaned Rage**, **Titan Scale**, **Triumph** : effets du vieux CalamityPlayer
  (les buffs n'y mettaient qu'un drapeau), corrigés par la page quand elle diffère : Penumbra +15 % de discrétion en
  bougeant, +15 % à l'arrêt la nuit, 20 % pendant une éclipse (le vieux code : +10 % / +15 %) ; Profaned Rage +5 % de
  vitesse pendant le vol (absent du vieux code) ; Titan Scale : 10 s de bonus après un coup de vrai corps-à-corps (vieux
  code) ; Triumph : -15 % × la vie manquante de l'ennemi. Valeurs des infobox, recettes du wiki. Tesla Potion pas encore
  portée (aura et debuff Galvanic Corrosion).

### Armes portées (`Content/Legacy/Items/LegacyWeapons1102.cs`, 02/11)

- **[choix]** **Monkey Darts**, **Sky Stabber**, **Sticky Spiky Ball**, **Bouncy Spiky Ball**, **Verdant**, **Golden Gun** :
  code de 2.0.1.005, valeurs des infobox, recettes du wiki. D'après la page : Sticky Spiky Ball Slimed 5 s et sept balles en
  frappe furtive (vieux code : 2 s, quatre) ; Bouncy Spiky Ball : en frappe furtive un seul ballon au bonus plus fort à
  chaque ricochet (+25 % au lieu de +10 %, chiffre choisi) ; Golden Gun : Ichor 8 s (absent du vieux code) ; Monkey Darts
  vendues 4 argent par le Bandit après Plantera, une chance sur deux de casser (vieux code : trois sur quatre).
  **[technique]** Le comportement collant de Calamity (CommonProjectileAI) est interne : la Sticky Spiky Ball se fixe à
  l'ennemi touché ou s'arrête contre un bloc avec un code simple. Le Sky Stabber supprime ses boules au clic droit
  directement (le drapeau killSpikyBalls de Calamity n'existe plus).
- **[choix]** **Gauss Dagger**, **Gauss Pistol**, **Magna Striker** (`LegacyWeapons1102b.cs`) : code de 2.0.1.005, valeurs des
  infobox (Gauss Dagger use 23, Gauss Pistol 110 dégâts au lieu de 150), recettes du wiki. Les deux armes Gauss utilisent la
  charge de Calamity (50 et 85). **[technique]** Le compteur de flux de Calamity (GaussFluxTimer) n'existe plus : un compteur
  par ennemi le remplace (+50 par coup, -1 par tick, flux à 60 : deux coups en moins d'environ une seconde) ; le son Gauss
  n'existe plus non plus : Gauss Pistol avec le son de tir plasma de Calamity, Magna Striker avec celui de l'Opal Striker.
- **[choix]** **Astral Blade**, **Coral Cannon**, **Clamor Noctus** (`LegacyWeapons1113.cs`, 13/11) : code de 2.0.1.005,
  valeurs des infobox et du texte (Astral Blade use 8 au lieu de 12 ; Coral Cannon use 30 au lieu de 20, Eutrophication
  0,25 s au lieu de 0,5 s ; Clamor Noctus : étoiles à 15 %, explosion à 135 %, Night Wyvern à 375 % au lieu de 100 % /
  100 % / 2500 % dans le vieux code). Chiffres choisis : chance de critique de l'Astral Blade = second tirage avec
  chance × (multiplicateur - 1) ; gros corail à 90 % de la vitesse (« slightly slower velocity ») ; une chance sur dix
  d'un Night Wyvern par projectile (vieux code) ; rayon d'explosion de l'étoile 40 px. Coral Cannon vendu par le Sea
  King sans condition (page). **[technique]** Le petit corail de Calamity est devenu un projectile de spy : copie à
  distance (SmallCoralShot) avec sa texture ; les étoiles se collent avec un code simple (comme la Sticky Spiky Ball) ;
  les débris du Night Wyvern (gores de 2.0.1.005) n'existent plus, remplacés par de la poussière.
  Laissés de côté : **Bazooka** (le système d'armes à usage limité et les Grenade Shells n'existent plus, recette avec
  Essential Essence Bar introuvable) et **Alpha Ray** (demande Genisis et Galactica Singularity, retirés aussi).
- **[choix]** **Plague Keeper**, **Poison Pack**, **The Enforcer** (`LegacyWeapons1113b.cs`, 13/11) : code de 2.0.1.005,
  valeurs des infobox (Plague Keeper use 19 au lieu de 20 ; Poison Pack use 18 au lieu de 19 ; The Enforcer use 16 au
  lieu de 17, rareté 14 au lieu de Dark Blue). Ajouts d'après les pages : nuage de la Plague Keeper à -40 % une seule
  fois en s'effaçant (le vieux code retirait 40 % à chaque mise à jour) ; God Slayer Inferno 5 s par les flammes de The
  Enforcer (absent du vieux code). Chiffres choisis : cinq flammes (vieux code), apparues au-dessus de l'ennemi touché
  (jusqu'à 200 px de côté, 60 px plus haut chacune) au lieu de près du curseur, pour suivre « near the enemy ».
  **[technique]** Nuage et flammes avec les textures actuelles de Calamity (VirulentWave, EssenceFlame).
- **[choix]** **Tesla Potion**, **Yharim's Stimulants**, **Divine Retribution**, **Hell's Sun** (`LegacyWeapons1113c.cs`,
  13/11) : code de 2.0.1.005, valeurs des infobox (Hell's Sun 85 dégâts au lieu de 240, use 14 au lieu de 15, multiplicateur
  de frappe furtive x0,12 gardé). Yharim's Stimulants : les sept effets de la page (le vieux code en avait d'autres :
  +10 défense, +25 % recul, +7,5 % vitesse de mêlée...). **[technique]** Tesla Potion : le drapeau `tesla` du Tesla's
  Amulet de Calamity existe toujours et donne l'aura (12 dégâts de la meilleure classe, pas 15 sans classe comme la
  page) ; quand le joueur a le buff de la potion, ses coups donnent en plus Static Discharge 3 s (l'« Electrified » du
  wiki) et Galvanic Corrosion 0,5 s (l'amulette seule : 1,5 s et 0,1 s, inchangé).
- **[choix]** **Fish of Light**, **Fish of Night**, **Molecular Manipulator** (`LegacyItems1113d.cs`, 13/11) : pêche des
  poissons selon 2.0.1.005 (Hardmode, couche des cavernes, prise peu commune, une chance sur sept ; jamais à la place
  d'une caisse, d'un poisson de quête ou d'une créature), pile de 9999 (page). Molecular Manipulator : les dix effets de
  la page à chances égales ; poussée de 1,875 bloc par seconde ajoutée à la vitesse, vers le haut ou sur le côté (Notes
  de la page ; le vieux code fixait la vitesse à 30 px par image, vers le haut ou le bas) ; dégâts de contact ±20 sans
  la mise à l'échelle par difficulté que décrit la page (jamais sous 0). **[technique]** Les changements de taille, de
  dégâts et de défense ne sont faits que chez le tireur (comme le vieux code) : pas synchronisés en multijoueur.
- **[choix]** **Dodu's Handcannon**, **Yanmei's Knife** (`LegacyWeapons1113e.cs`, 13/11) : code de 2.0.1.005, valeurs des
  infobox (Handcannon : vitesse 24 au lieu de 13). **À trancher** : la recette du wiki de Dodu's Handcannon demande le
  Pumplerfaust, objet de l'addon Community Remix qui n'existe pas dans le mod : l'arme n'est pas fabricable pour
  l'instant (proposition : recette de 2022 en Calamity++ avec la Pearl God, ou créer le Pumplerfaust).
  Yanmei's Knife : tous les effets Kami ont disparu de Calamity et sont réécrits d'après la page : Kami Injection = +15 %
  de vitesse de déplacement (et de vitesse max) et +15 % de dégâts, Kami Flu = réduction des dégâts x0,8 (sauf
  Dragonfolly, Polterghast, Providence) et 125 PV/s, 10 s chacun ; 3 s d'attente après chaque coup ; son de l'Adrenaline
  Mode (page). Chiffres choisis : traînée et lueur vertes faites de poussière et de teinte (la page parle d'images
  rémanentes) ; cibles = boss sauf segments de vers, mini-boss de la barre de vie de Calamity et de l'Acid Rain, et tout
  ennemi pendant qu'un boss est vivant (« serviteurs » : Calamity n'a plus sa liste).
- **[choix]** **Gatling Laser** (`LegacyWeapons1113f.cs`, 13/11) : code de 2.0.1.005, valeurs de l'infobox (43 dégâts au
  lieu de 50 ; rareté « a » de la page illisible : Dark Orange comme le vieux code). **[technique]** Les trois sons du
  Gatling Laser (début, boucle, fin) n'existent plus dans Calamity : sons de laser vanilla (Item157 au début, Item91
  toutes les 6 images, Item158 à la fin).
- **[choix]** **Old Power Cell**, **Shroomite Visage** (`LegacyItems1113g.cs`, 13/11) : code de 2.0.1.005, valeurs des
  infobox (Old Power Cell use 45 au lieu de 10 ; Shroomite Visage vendu 6 or). Lance-flammes = armes à distance qui
  utilisent du gel (Calamity n'a plus sa liste de lance-flammes) ; texte du bonus d'ensemble : celui de la shroomite
  vanilla.
- **[choix]** **Acidic Rain Barrel**, **Nychthemeron** (`LegacyWeapons1113h.cs`), **Sea Urchin** (`SeaUrchin.cs`) (13/11) :
  code de 2.0.1.005, valeurs des infobox et des pages. Acidic Rain Barrel : use 21 au lieu de 22 ; gouttes avec les trois
  debuffs de la page (Irradiated 3 à 5 s ; Poisoned 1 à 3 s une fois sur deux ; Sulphuric Poisoning 1 à 2 s une fois sur
  huit ; le vieux code ne donnait qu'Irradiated 3 s). Nychthemeron : use 19 au lieu de 20 ; la boule casse aussi quand
  elle s'arrête (page) ; rappel par clic droit comme le vieux code (la page décrit un rappel lent en maintenant le clic,
  rapide en le relâchant : pas fait). Sea Urchin : Venom 2 s au contact (vieux code, absent de la page), insensible à
  Kami Flu (page). **[technique]** Pas de bannière pour le Sea Urchin (la tuile de bannières de Calamity a changé).
- **[choix]** **Cosmic Elemental** (`CosmicElemental.cs`, 13/11) : code de 2.0.1.005 (IA du Granite Elemental, une chance
  sur cent du taux des cavernes), butin de la page (Bone Sword 4 %, Starfury 2 %, Enchanted Sword 1 %, Arkhalis 0,5 % ; le
  vieux code donnait 5 %, 2 %, 2 % et un Terragrim), immunités de la page (Confused, Poisoned, Kami Flu). Cosmic Stone et
  Arcanoid Orb pas encore dans le mod (TODO). Pas de bannière, comme le Sea Urchin. Écartés : **Astral Ice** (existe
  encore dans Calamity, déplacé : faux positif de legacy_extract) et **Drunk Princess** (PNJ toujours là sous un autre nom).
- **[choix]** **Calamitas' Brew** (`LegacyPotions1113.cs`, 13/11) : potion de 2022-10, valeurs de la page (Lime, 15 min).
  Le buff Abyssal Weapon inflige **Abyssal Flames** (`Content/Buffs/AbyssalFlames.cs`), debuff créé d'après sa page (en
  2022, la potion donnait déjà les Brimstone Flames de Calamity, et Abyssal Flames n'existe plus) : pas de régénération,
  -6 défense, joueur -8 PV/s, ennemi -62,5 PV/s (78,5 avec Oiled), ennemi teinté de noir avec des particules rouge
  sang. Durées de la page : 6 s une fois sur quatre, 4 s ou 2 s sinon. Icônes des deux buffs : images du wiki. TODO :
  Supreme Calamitas, ses frères, Sepulcher et les Soul Seekers devraient l'infliger au joueur (page).
- **[choix]** **Aero Dynamite** = la Skynamite actuelle (même projectile AeroExplosive,
  renommée par Calamity) : renommage ajouté au glossaire, pas de portage.
- **[choix]** **Revengeance**, **Death** (`DifficultyItems.cs`, 13/11) : objets de 2022 portés dans les deux modes ; ils
  passent par le sélecteur de difficulté de Calamity (qui les a remplacés), donc l'icône et les messages restent
  cohérents. Désactiver Revengeance revient en Expert, désactiver Death revient en Revengeance. Recette « à un autel,
  sans ingrédient » codée à la main (WikiRecipes ne lit pas une recette vide) ; dans le Starter Bag en Expert (pages).
- **[choix]** **Alpha Ray** (`LegacyWeapons1113i.cs`, 13/11) : code de 2.0.1.005, valeurs de l'infobox et de la page (rareté
  14 au lieu de Dark Blue). Finalement portable : la « Genisis » du wiki est la Genesis renommée de Calamity et la
  Galactica Singularity est déjà un matériau généré (correction de la note du lot précédent).
  **[technique]** Alias de recette « Genisis » -> Genesis (glossaire) : l'arme de Calamity n'est pas renommée, car le wiki
  a aussi une page Genesis (objet de l'addon Catalyst).
- **[choix]** **Elemental Excalibur** (`LegacyWeapons1113j.cs`, 13/11) : code de 2.0.1.005, valeurs de l'infobox et de la
  page (use 13 au lieu de 14 ; soin de 10 à 19 PV au lieu de 10 à 12 ; rareté arc-en-ciel = celle des objets Expert sans
  être un objet Expert, comme le dit la page). Ajout d'après la page : 13e couleur, le **gris**, absent du vieux code
  (12 couleurs) : traverse blocs et ennemis ; chiffre choisi : une chance sur cent de lâcher une pièce de platine à
  l'impact. Greatsword of Blah : l'arme renommée de Calamity sert à la recette.

### Gel armor (`Content/Items/Armor/GelArmor.cs`, 18/11)

- **[choix]** Ensemble inventé par le wiki (page Gel Armor), dans les deux modes : valeurs des infobox de chaque pièce
  (le texte de la page dit 4 % de critique sur les casques, les infobulles 3 % : infobulles retenues), rareté 4, prix de
  vente des infobox. Bonus d'ensemble de la page : saut plus haut (+1/3 comme la Statigel, « 3 blocs »), +6 % de vitesse
  de saut, saut supplémentaire de la Statigel (particules de gel), 30 images d'invulnérabilité de plus après un coup de
  plus de 100 dégâts ; Helm +200 d'aggro ; Hood +6 % de dégâts des sbires, +1 sbire et un Baby Slime vanilla à 13
  dégâts ; Mask : furtivité de spy jusqu'à 50. Chiffre choisi : recul des sbires +50 % pour le Hood (« increased »).
  Mask : 15 % de chance de ne pas consommer une arme de spy consommable.
- **[technique]** Pas de planches portées sur le wiki : rendu des pièces Statigel de Calamity en attendant des sprites
  (`docs/SPRITES.md`, section 2).

### Bâtons écureuils (`Content/Items/Weapons/Summon/SquirrelStaves.cs`, 18/11)

- **[choix]** Sept tourelles inventées par le wiki (Knight, Baron, Viscount, Count, Duke, Prince, King), dans les deux
  modes, sprites du wiki (tourelles et bâtons, une image chacune, sans animation). Valeurs des infobox (dégâts, recul,
  mana 12, use 34, rareté ; la pile de 4 du Baron est ignorée). Attaques d'après les pages, avec des chiffres choisis :
  tir toutes les 40 images (Duke toutes les 8, King toutes les 50), portée 960 px comme le Squirrel Squire ; Knight :
  couteau qui perce une fois ; Baron : Prism Shard qui perce une fois puis 2 ou 3 éclats à moitié des dégâts ; Viscount :
  pieu qui perce sans fin ; Count : boule de feu chercheuse, explosion de 64 px, On Fire! 3 s ; Duke : bulles chercheuses
  à moitié des dégâts ; Prince : explosion de 160 px puis trois boules de feu du Count à moitié des dégâts, Holy Flames ;
  King : colonne de lumière de 480 px sur la cible, Holy Flames. Chute lente (2,5 px par image au plus) ; posées au
  curseur à 600 px au plus. **[technique]** Projectiles avec les textures vanilla (couteau, pieu, boule de feu, bulle,
  Inferno) et le Prism Shard de Calamity.

### Spy Emblem (renommage, 18/11)

- **[choix]** La **Spy Emblem** du wiki est la Rogue Emblem de Calamity (même effet, même butin du Wall of Flesh ; le
  vocabulaire « rogue → spy » l'affichait déjà ainsi) : renommage ajouté au glossaire pour que l'index la relie et que
  les valeurs de l'infobox s'appliquent (rareté 4, vente 2 or 40).

### Armes du wiki du 18/11 (`Content/Items/Weapons/WikiWeapons1118.cs`)

- **[choix]** **Terriblade**, **Sea Stone Sickle**, **Auric Sword**, **Raito**, **Majestic Gar**, **Sea Stone Shuriken**,
  **Dirt Clump**, **Sillyshark** : objets inventés par le wiki, dans les deux modes, sprites et valeurs des infobox.
  Chiffres choisis : Terriblade — l'effet est tiré à chaque coup (9 à chances égales) ; glaçon à moitié des dégâts ;
  éclair = colonne de 600 px à un endroit au hasard de la moitié basse de l'écran ; « Electrified » = Static Discharge de
  Calamity ; Bleeding vanilla (sans effet sur les ennemis) ; 8 éclats à 20 % ; la lame lancée revient au joueur
  (boomerang : se plante jusqu'à 5 s ; ricochet : 4 rebonds, rebondit sur l'eau) ; saut vers le haut à 14 px par
  image. Majestic Gar : « très grande portée » = taille x1,5, la baisse de défense est permanente pour l'ennemi.
  Raito : vraie mêlée, taille x1,4. Sea Stone Shuriken : une chance sur quatre de le récupérer. Dirt Clump : 5 fragments
  qui cherchent les ennemis à 200 px. Sillyshark : 3 à 5 crottes de nez (poussière verte, sans dégâts), Vaporfied 1,5 s
  par les balles, taille d'affichage x0,75 (sprite du wiki très grand).

- **[choix]** (suite, `WikiWeapons1118b.cs`) **Smoldering Seahorses**, **Aflaming Crystalliser**, **Moonstone Knives**,
  **Ethereal Knives**, **Heavenwing Staff**, **Eye of Light**, **Wooden Butterfly Staff**, **Crustalline** : sprites et
  valeurs des pages. Chiffres choisis : hippocampe chercheur à 500 px (On Fire! 2 s, élément Fire de la page) ; orbe
  d'améthyste qui tire un cristal toutes les 20 images (projectiles en poussière et cristaux vanilla teintés : pas de
  sprite sur le wiki) ; 3 éclats ricochants par cristal ; Ethereal Knives : 8 couteaux par utilisation (use 3, 24 images),
  explosion de 80 px à moitié des dégâts ; chauves-souris (texture de la Hellwing, teinte dorée) apparues 60 à 160 px
  au-dessus du curseur ; Eye of Light : une étoile toutes les 12 images, portée 800 px ; papillons : suivent le joueur,
  portée 600 px, s'éloignent après chaque charge ; Crustalline : un tour de 40 images à 70 px, se plante 2 s,
  5 fragments à moitié des dégâts en frappe furtive, bruit de verre brisé (le son de Providence n'est pas repris).
  **Wulfrum Shock Cutter** (ingrédient du Crustalline) : rafale de 3 (use 4, 12 images), clic droit maintenu = retour
  des cutters ; frappe furtive : 1,6 fois plus rapides, deux coups chacun ; pas encore obtenable : son butin vient de
  **The Inventors**, boss inventé pas encore dans le mod (le Crustalline ne se fabrique donc pas encore).

### The Inventors (`Content/NPCs/Inventors/`, `Content/Tiles/WulfrumSignalTransmitter.cs`, 18/11)

- **[choix]** Boss inventé par le wiki (page The Inventors), dans les deux modes : John et Jane Wulfrum, vie et dégâts
  par difficulté de la page (450/520/564/640 chacun, 10/16/20/25), défense 10 et 4, réduction des dégâts 5 %, sprites
  du wiki (une image chacun, pas d'animation). Invoqués par une Draedon Power Cell (consommée) sur le Wulfrum Signal
  Transmitter ; l'Energy Core (consommé) passe avant et lance un signal de 30 s (1 à 4 robots Wulfrum toutes les 4 s).
  Attaques de la page avec des chiffres choisis : marche 2,4 (3,6 à 75 %), ruée de 30 images à 12 px/image toutes les
  200/160/120 images (normal/Expert/Revengeance), 3 Wulfrum Bolts en éventail au début et à la fin des ruées (50 %),
  saut chargé 1 s puis retombée à 18 px/image toutes les 7 s (25 %) ; Jane : flèche toutes les 75/60/45 images (Death
  ÷1,3), éventail de 5 couteaux ou Blunderbuss (5 plombs) toutes les 210 images (150 en Revengeance), épée à moins de
  90 px. Death : une série d'attaques d'avance et vitesse x1,4. Bouclier : celui qui a passé plus de seuils que l'autre
  est invincible. Celui qui est le plus loin du joueur saute vers l'autre au bout de 3 s à plus de 900 px. Dégâts des
  projectiles ramenés à ceux de la page malgré le multiplicateur Expert du jeu.
  Butin : le sac (Expert) ou le butin normal tombe avec le second vaincu ; les noms « Egbert » de la page sont du
  vandalisme des objets Wulfrum : Wulfrum Metal Scrap 16 à 26, Energy Core 1 à 2, Energy Orb, Conquest Fragment, une
  arme parmi Wulfrum Blunderbuss, Wulfrum Prosthesis, Wulfrum Shock Cutter ; Lesser Healing Potion 8 à 14 ; Metabolic
  Converter dans le sac en Revengeance. Pas encore faits : masques, trophées, relique, lore, et l'« Aftermath » de la page (Energy Orbs des robots Wulfrum, minerais qui poussent, Desert Medallions des
  Cnidrions, Hive Mind invocable seulement après).
- **[choix]** **Wulfrum Hardhat** (`Content/Items/Armor/WulfrumHardhat.cs`) : récompense du Taming des Inventors (tombe
  quand ils sont apprivoisés), 2 défense, immunité aux pièges = projectiles « trap » du jeu et rochers ; planche portée
  fabriquée depuis l'image « Equipped » du wiki (à refaire : `docs/SPRITES.md`).
- **[choix]** Taming (page Taming : « Give the Inventors a Diamond Ring ») : la Diamond Ring s'utilise (tenue en l'air)
  à moins de 15 blocs de John ou de Jane, elle est consommée et ils sont apprivoisés ; mêmes règles que les autres
  (ne pas les blesser). Mêmes conditions dans les deux modes (à trancher : difficulté de Calamity++).

### Objets du wiki du 19/11 (`Content/Items/PermanentBoosters/WikiBoosters.cs`, `Content/Items/Tools/WikiTools1119.cs`)

- **[choix]** Bonus permanents **Bagel Nuts**, **Chez**, **Ham Pie**, **Juzie**, **KEX** : une fois par personnage (comme
  le Life Fruit), valeurs des pages. Chiffres choisis : Juzie « 40 % faster life regen ramping » = +0,4 au compteur de
  régénération par image ; Ham Pie : dégâts de défense de Calamity x0,5 (une Meld Blob une fois sur mille, page).
- **[choix]** **Draconic Elixir** : +20 % de temps de vol, +15 % de vitesse et d'accélération en vol, +16 défense ; la
  résurrection de Calamity (Silva, Auric) détectée au démarrage de son compte à rebours soigne à la moitié des PV, retire
  le buff, 1 min de Draconic Surge Cooldown et +30 s de Potion Sickness. **Potion of Triboluminescence** : Hunter,
  Spelunker, Dangersense et l'Omniscience de Calamity (la mise en valeur des projectiles amis n'est pas faite).
  **Stratus Beverage** : -33 % de dégâts ; chaque tir d'une arme à projectiles ajoute deux étoiles Stratus chercheuses
  (aux dégâts de l'arme, classe la plus forte du joueur) ; ne compte pas encore comme deux alcools.
- **[choix]** **Anti-Starspawn Ointment** : armes et accessoires lâchés par un ennemi supprimés si le joueur le plus proche
  est dans l'Astral Infection avec l'ointment en favori. **Rod of Misery** : symétrie en X, en Y ou les deux (un tiers
  chacun), pas de téléportation dans les blocs. **Wulfrum Mining Helmet** : trésors (Spelunker), immunité aux pièges du
  Hardhat, lumière bleue devant le joueur ; planche portée découpée dans l'image « Equipped » du wiki. **Jellyfish
  Cannonball** : la fabriquer donne un Cannonball Jellyfish au lieu de l'objet (page) ; TODO : munition du Cannon.
- **[choix]** Armes du 19/11 (`WikiWeapons1119.cs`, `1119b`, `1119c`), sprites et valeurs des infobox ; chiffres choisis :
  Fakenado : un requin toutes les 30 images, portée 700 px. Onyx Purge : 3 lasers + 1 éclat par tir (explosion 80 px).
  Interfacer Staff : 5 rebonds, un petit rayon (1/3 des dégâts) toutes les 12 images vers un ennemi à 300 px. Exif Fist
  / Hand of God : explosion en 4 à 6 étoiles à moitié des dégâts ; explosion finale du Hand of God réduite à 100 blocs
  (la page dit 300) à x3 dégâts. Celesticus : classe « moyenne » de Calamity (tous les bonus), pas de reforge, cristaux
  à 25 % des dégâts de base. Supercharged Dart Blaster : sans munition ; onde qui grandit 1,5 s puis 9 ondes à moitié.
  Twentloon / Twenty-Twloon : harpons d'affilée (un par image) avec le harpon du Triploon porté. Terra Bow : Lunar Bolts
  du Lunarian Bow de Calamity. Elemental Bow : 7 couleurs dans un ordre au hasard. Rods from God : soleil à x5 dégâts,
  perdu s'il est relâché avant 10 s ; explosion de 100 blocs. Grand Gar : explosion de 120 px, traits à 1/3 des dégâts,
  orbe de soin de 5 PV. Deeepscale Bomb : explosion de 200 px qui blesse aussi le joueur ; casse dans un rayon de 3
  blocs l'Abyss Gravel, la Voidstone, le Pyre Mantle et la Planty Mush. Stellar Kunai : chargés au bout de 60 images,
  explosion de 64 px. Catastrophe Claysiphonophore : éclairs de couleur de la Catastrophe Clay en dégâts d'invocation.
  Virid Vangar : un coup toutes les 20 images, Poisoned 4 s. Pastranion : filament toutes les 20 images (8 en frappe
  furtive), 320 px (640). Vermillion Angel : éclat tous les 12 pas, Vermillion Blast de 160 px (6 coups), hexagramme de
  12 éclats en frappe furtive. Breakthrough : « Frozen » = Glacial State 2 s ; ennemi gelé frappé = x3 et éclats de
  glace. Summit Breaker : arracher = x4 dégâts ; la traînée de plasma de son animation n'est pas faite.
  Pas encore faites : Garth, Justice and Splendor, Worst Prism (plus complexes).
- **[technique]** `gen_items.py` génère aussi les objets de type « Item » qui servent à une recette ou se disent matériau
  (chaîne de la Flah : Derelah, Molten Flah, Hemorraize, Meld Flah, Empress Flah, Washed Flah).

### Poissons-sacs du wiki (`Content/Items/Fishing/WikiFishBags.cs`, 19/11)

- **[choix]** **Front Gar**, **Rear Gar**, **Side Gar**, **Flahr** : 10 % des prises peu communes dans leur zone, après le
  boss de la page (pas à la place d'une caisse, d'un poisson de quête ou d'une créature) ; contenu des pages (Flah Seeds
  du Flahr : TODO, pas encore dans le mod). **Tar Gar** : « commonly » = une prise commune sur quatre dans la Corruption
  ou le Crimson la nuit ; clic droit : Slime Rain (refusé si elle est déjà en cours).

### Ennemis générés (`tools/gen_enemies.py`, `Content/NPCs/WikiEnemy.cs`, 19/11)

- **[choix]** Les ennemis inventés par le wiki (pages « new » à infobox de PNJ de type Enemy, Flying Enemy ou Slime, avec
  un sprite, hors vers, boss et pages historiques ; les pages d'addon comptent comme les autres, section 11 : 91) sont créés depuis leur infobox, dans les deux modes, sans
  classe dédiée : vie, dégâts de contact (premier nombre), défense, résistance au recul, argent, debuff au contact
  (trouvé par son nom affiché, ignoré s'il n'existe pas), sprite du wiki. IA vanilla la plus proche de la page :
  slime (« Slime AI » ou type Slime), chauve-souris (« Bat AI »), Demon Eye (yeux et volants), zombie sinon. Le butin
  vient de WikiLoot et les éléments d'ElementData, par le nom ; les comportements décrits seulement dans le texte
  (tirs, phases, Death Mode) ne sont pas faits : à donner à ceux qui le méritent une classe dédiée.
- **[choix]** Apparition : zones tirées de « environment » (Surface, Underground, Cavern, Snow, Desert, Jungle, Ocean,
  Underworld, Dungeon, Space, Hallow, Corruption, Crimson, Mushroom, Temple, Brimstone Crag, Sulphurous Sea, Abyss,
  Sunken Sea, Astral Infection), Hardmode / après le Moon Lord d'après l'infobox, la nuit si le texte le dit ; 5 % du
  taux normal (1 % au-delà de 5 000 PV), jamais en zone sûre ni pendant une invasion. Pas d'apparition naturelle pour
  les ennemis sans zone (41 : ennemis d'événements ou de boss inventés) ni pour ceux manifestement mal classés (plus de
  2 000 PV sans « hardmode », plus de 30 000 PV sans « postml », comme les ennemis Exo).

### Textures du wiki sur Calamity (`Assets/Overrides/`)
- **[validé 21/10]** Icône du wiki pour Moon Burnt (au lieu de celle de Nightwither), y compris dans l'affichage de
  debuffs de Calamity au-dessus des boss.
- **[validé 31/10]** **Objets animés** (77 sautés jusqu'ici par `gen_sprites.py`) : l'image du wiki est comparée à
  chaque image de la planche (recadrées). 25 en sont une : animation de Calamity gardée. 27 montrent un autre dessin
  (Abyss Bar, Astral Bar, Cosmilite Bar...) : image fixe du wiki, animation coupée en jeu (`Generated/static.json`,
  rétablie au déchargement). 22 aliments et boissons (Vodka, Baguette...) : leur planche de 3 images (inventaire, en
  main, consommation) est refaite avec l'image du wiki, sans rien couper. **[technique]** 3 objets qui découpent
  eux-mêmes leur planche ou rechargent leur texture par son chemin gardent leur sprite et leur animation : Graydron
  (Murasama), Telluric Glare, Auric Bar (sinon image invisible, ou plantage du dessin de la lueur). Mêmes règles de
  mode qu'avant : objets renommés (Abyss Bar, Phantoplasm, Astral Arcanum, Core of the Blood God) dans les deux modes,
  officiels en Full Fandom seulement. Liste complète : `reports/sprites.md`.
  **[technique]** Ceux qui se dessinent eux-mêmes dans l'inventaire avec un décalage réglé pour leur sprite (Abyss Bar
  +5/-12 px, Dynamo Stem Cells, Soul of Cryogen, The First Shadowflame, Shattered Community) : leur dessin propre est
  sauté quand l'image du wiki est appliquée, le jeu la dessine centrée (détour sur leur PreDrawInInventory).

- **[choix]** **Sandy Angling Kit** porté depuis 2.0.1.005 (retiré de Calamity, lâché par le Desert Scourge sur le
  wiki) : chances du vieux code, identiques à la page ; pile 9999 et sans valeur (infobox).

- **[validé 22/10]** **Sprites du wiki** pour les objets de Calamity dont l'image diffère (`tools/gen_sprites.py`,
  `reports/sprites.md`) : objets **renommés** (Conquest Fragment, Divine Bar, Victide Bar...) → sprite du wiki dans
  **tous les modes**, comme leur nom ; objets **officiels** au sprite refait par Calamity depuis 2022 → sprite du wiki en
  **Full Fandom** seulement (Calamity++ garde le sprite actuel).
- **[validé 30/10]** Nos propres objets (portages, créations) dont le wiki montre un autre sprite reçoivent le sprite
  du wiki **dans tous les modes** (« plus logique » : ces objets n'existent que par le wiki) :
  `gen_sprites.py` → `Generated/Items/`, `TextureOverrides` cherche la classe dans Calamity puis dans ce mod
  (une classe de Calamity du même nom est ignorée). 17 objets au 30/10, dont le Blood Clot Staff (retour de test).
  **[technique]** Les sprites de Terraria et de Calamity sont dessinés en blocs de 2×2 pixels : une image du wiki qui
  semble « agrandie x2 exactement » est en général à sa vraie taille ; ne la réduire que si elle dépasse 96 px (règle de
  `gen_sprites.py` et `gen_items.py`), jamais à la main avec `--scale 2` sans comparer à l'ancien sprite (erreur du 30/10 sur
  l'Exo Gladius et la Microcell Cut).
  **[technique]** Correction de la comparaison des images : `getbbox()` d'une image RGBA ne regarde que la transparence,
  et l'écart moyen incluait le fond transparent, si bien que des recolorations entières passaient pour « identiques ».
  Nouvelle règle : même image si même forme (≤ 2 % de pixels dont la transparence change) et ≤ 3 % des pixels visibles
  nettement différents (un canal à plus de 48). Résultat : ~120 objets de Calamity en plus reçoivent leur sprite du wiki.
  **[technique]** Icône seulement (la hitbox garde sa taille). Images « presque identiques » (compression WebP, x2)
  comptées comme identiques. Ignorés : objets animés de Calamity (76, dont Abyss Bar, Cruel Jewel, Graydron : le wiki
  n'a qu'une image fixe, et Graydron dessine ses propres textures), murs (le wiki montre le mur posé), illustrations
  de plus de 96 px. Une image faite à la main dans `Assets/Overrides/Items/` passe avant.

- **[choix]** Portages de la nuit du 22/10 (vieux code de 2.0.1.005, valeurs du wiki là où elles diffèrent) :
  **Bleached Angling Kit** (table de sa page), **Seabound Staff** (réutilise le sbire et le buff du Brittle Star
  Staff actuel), **Effluvium Bow** (Mist Arrow de Calamity ; l'option « tir à bout portant » n'existe plus),
  **Regenator** (effet disparu du code actuel : repris de 2.0.1.005, vie max x0,5, régénération +12), **Storm Spray**,
  **Aquatic Discharge** (use 12 ; étincelles quand le coup **tue**, comme le dit la page, au lieu de chaque coup).
  **[technique]** « Calamitas Mask » = l'actuel Calamitas Clone Mask : simple alias.
  Second lot : **Solar Flare** (yoyo de Providence), **Diseased Pike** (lance de la Plaguebringer Goliath),
  **Crimslime Staff** / **Corroslime Staff** (The Slime God ; use 29 du wiki ; leurs indicateurs du vieux
  CalamityPlayer remplacés par le buff du sbire). Valeurs identiques au vieux code sauf mention.
  Troisième lot : **Gehenna** (Brimstone Flames 10 s du wiki au lieu de 4 s ; compte à rebours dans un ModPlayer),
  **Shock Grenade** (use 17, pile 9999, recette du wiki : 2 Grenade, 1 Bomb, 5 Stratus Brick → 100).
  Quatrième lot : **Mana Jelly**, **Fungal Carapace**, **Amidias' Spark** (effets « quand on est touché » repris du
  vieux CalamityPlayer dans un ModPlayer), **Energy Staff** (remplacé par la Sanctified Spark en 2023 ; réutilise sa
  tourelle Profaned Energy ; remplacé le 22/10 par la tourelle **immobile** de 2.0.1.005, la version actuelle suit le
  joueur). **Beetle Juice** : matériau simple, généré par `gen_items.py` comme les autres.
  Cinquième lot : **Starfall** (50 dégâts du wiki au lieu de 57), **Storm Dragoon**, **Soul Edge** (use 22).
  **[choix]** Renommage : Starspawn Helix Staff → **Godspawn Helix Staff** (Calamity l'a seulement renommé).
  **[technique]** L'index ignore les noms restés dans la localisation de Calamity sans classe derrière (Beetle Juice,
  Cosmic Guardian, Exo Freeze passaient pour « officiels » alors qu'ils sont à porter).
- **[choix]** Groupe « One of the following N items » dont le jeu ne donne qu'une partie (Desert Scourge : Calamity a
  gardé Barinade et Scourge of the Desert, remplacé les autres) : les objets absents sont ajoutés un par un avec leur
  chance de la page (Seabound Staff, Storm Spray, Aquatic Discharge à 25 %), en plus du groupe du jeu.
- **À faire** : Gravistar Sabaton et Blazing Core (effets à recréer). Le Shortstrike est fait (section 8 bis).

- **[validé 28/10]** **Galileo Gladius** et **Cosmic Shiv** refaits par Calamity après 2.0.1.005 : en Full Fandom, ils
  retrouvent leur fonctionnement de 2.0.1.005, que décrit leur page (`Common/Legacy/FullFandomLegacyWeapons.cs`,
  projectiles portés dans `Content/Legacy/Projectiles/Melee/`) ; Calamity++ garde les armes actuelles. Cosmic Shiv : orbe
  cosmique ondulant puis à tête chercheuse, qui appelle 3 lames et 6 à 10 étoiles. **[choix]** Durées de debuff de la page
  (planètes : Moon Burnt 5 s au lieu de 4) ; un coup par estoc (le vieux code touchait à l'aller et au retour) ;
  l'explosion de l'orbe agrandit sa hitbox au lieu de frapper à la main.
- **[validé 28/10]** Galileo Gladius en Full Fandom : l'animation de l'arme actuelle de Calamity (épée tenue, lame
  lumineuse), réduite (portée 84 px au lieu de 192, lame lumineuse 0,65 au lieu de 1,55, arme plus avancée dans la main),
  avec les dégâts de 2.0.1.005 ; un coup direct fait tomber une planète (15 frames entre deux) sans croissant, un estoc
  dans le vide tire le croissant de lune (un tiers des dégâts) ; pas de lancer ni de Stratus Starburst (le compteur
  « Starburst » en haut à gauche n'apparaît plus en le tenant). **[choix]** Les
  chiffres de la réduction, à ajuster en jeu.
  Notre page Galileo Gladius décrit ce comportement sans `{{mode}}` : c'est le Full Fandom voulu, le texte du Fandom
  (croissant à chaque coup, pluie de planètes) reste dans `fandom_dump_txt/`.
- **[validé 28/10]** **Spore Knife** : la page décrit l'épée courte de mêlée que Calamity a retirée en juin 2022 (avant
  2.0.1.005) puis refaite en couteau de lancer rogue. Full Fandom : épée courte (estoc, true melee, nuage de spores vanilla
  au coup, moitié des dégâts de base) ; Calamity++ : l'arme rogue de Calamity, qui garde le Shortstrike (clin d'œil).

### Butin des ennemis du wiki (`Content/Items/Weapons/WikiWeapons1119d.cs`, 19/11)

Valeurs des infobox ; les objets tombent par WikiLoot (pages des ennemis).

- **[choix]** **Creature Capsule** : explosion (son et poussière) ; fantôme de 6 s, tire une Lost Soul alliée (un quart des
  dégâts) toutes les 30 images sur l'ennemi le plus proche à 600 px. « Capture » = mort instantanée d'un non-boss sur un
  critique de la capsule (les fantômes ne capturent pas) ; la capsule elle-même ne fait plus de critique normal.
- **[choix]** **Guardians Wrath** : clic gauche = estoc (IA du Trident) ; clic droit = trident lancé (gravité légère) qui
  se plante dans un bloc ou un ennemi ; clic droit à nouveau = éclair de 5 x les dégâts sur 160 px (Electrified 2 s) et le
  trident disparaît (« revient » : l'arme ne quitte jamais l'inventaire, on ne peut juste plus l'utiliser tant qu'il est lancé).
- **[choix]** **Hailberd** : cercle de 220 px autour du curseur, accélération x1,06 jusqu'à 26 px/image, 3 ennemis
  traversés, Frostburn 3 s.
- **[choix]** **Pizza Wheel** : une pizza (texture de la Pizza vanilla) part de chaque bord de l'écran à la hauteur du
  curseur, 14 px/image, frappe chaque ennemi une fois.
- **[choix]** **Sign of the Operator** : arme maintenue (canalisée) ; la page flotte à 50 px vers le curseur et lâche une
  tentacule toutes les 6 images (portée courte, 2 ennemis) qui désigne la cible des sbires comme un fouet.
  « Operator Sickness » n'existe pas : **Ichor 3 s** à la place (TODO si la page du debuff est faite).
- **[choix]** **Elder Berry** : bonus permanent (WikiBooster) ; touche « Elder Berry Dash » (F par défaut) : vitesse
  horizontale 14 dans la direction tenue, 1 s d'attente.
- **[choix]** **Metamorphic Stone** : sbire sans place (accessoire), sprite de l'Earth Elemental réduit à 35 % ; toutes les
  2 s une attaque à tour de rôle : ruée (contact pendant la ruée), 4 rochers du Staff of Earth (un tiers des dégâts),
  coup de bras à x6 sur 90 px (War Cleave et Armor Crunch 3 s).

### Lot H du 19/11 (`Content/Items/Weapons/WikiWeapons1119e.cs`)

Valeurs des infobox. Molten Piercer, Obsidigun et The Generator tombent de **Pyrogen** (boss fait plus tard le 19/11).

- **[choix]** **Axisdriver** : rafale = 3 tirs à 4 images puis 27 d'attente (39 en tout, la valeur `use`) ; balles
  exo à 4 mises à jour par image, 10 ennemis ; clic droit 39 images ; explosion de 130 px (moitié des dégâts de la balle
  chercheuse) avec l'image de l'explosion de prisme de Calamity ; éclairs élémentaires à un tiers des dégâts.
- **[choix]** **Elemental Saw Launcher** : charge jusqu'à 2 s (vitesse de lancer 18 à 30), lancée 45 images puis retour ;
  souffles à la moitié des dégâts ; explosion du rappel x3 ; Light Disc vanilla en guise de scie (pas de sprite).
- **[choix]** **Obsidigun** : vitesse 28 avec 2 mises à jour (le `velocity = 56` du wiki) ; 4 éclats à un tiers des dégâts.
- **[choix]** **Onyx Minigun** : tir d'onyx = Black Bolt vanilla (comme l'Onyxia de Calamity) à 170 %, 4 paires de balles. Sa seule recette est à l'Aquatic Hypercomputer ERM (page d'addon, station pas encore faite) : pas fabricable pour l'instant.
- **[choix]** **SDOMG** : 2 balles par tir ; dispersion de 0,3 à 0,02 rad en 600 images de tir (remise à zéro après 0,5 s
  sans tirer) ; « immobile » = vitesse < 0,1 ; rayon de 2000 px, x3 dégâts, une Plasma Grenade (Draedon's Arsenal de
  Calamity) au départ puis toutes les 5 s.
- **[choix]** **The Generator** : sceau à 8 px/image (le `velocity = 25` ferait traverser l'écran en une seconde), 2,5 s,
  attire les non-boss à 240 px.
- **[choix]** **Worst Prism** : 7 rayons (rouge On Fire!, orange Hellfire, jaune Ichor, vert Poisoned, cyan Frostburn,
  bleu Confused, violet Shadowflame ; Vaporfied 1,5 s pour tous), longueur 1600 px qui varie de ±240 px par image entre 0
  et 3200 ; mana prise toutes les 12 images.
- **[choix]** **Unending Void** : 8 orbes ; trou noir sur un coup = texture de l'Event Horizon de Calamity ; trou noir
  supermassif au curseur, 3 s, un orbe attiré toutes les 8 images depuis 600 px ; explosion sur 400 px, 12 orbes.
- **[choix]** **Garth** : soin = 5 % des dégâts, entre 1 et 68 ; 3 météores RGB à la moitié des dégâts depuis le haut de
  l'écran autour du curseur, éclatement après 1 s ; météores colorés au quart ; explosion rouge 100 px.
- **[choix]** **Molten Piercer** : vitesse 16 (au lieu de 28) ; 3 éclats au tiers ; frappe furtive : 3 s, un éclat
  toutes les 5 images dans la direction de la rotation.
- **[technique]** **Large Father** retiré (19/11, comme le 30/10) : sa recette du wiki remplace celle de la Crystyl
  Crusher de Calamity, qui ne serait plus fabricable tant que The Wall of Bronze (seule source) n'existe pas. Sprite gardé
  dans `Content/Items/Tools/LargeFather.png`. À refaire avec le boss (clic droit : coup qui mine 7 x 7 blocs).
- **[choix]** **Prehistoric Tablet** : comme le Metamorphic Stone (sbire d'accessoire sans place) ; nuages 2 s après le
  repérage, gouttes toutes les 10 images au tiers des dégâts ; 5 ruées à 30 images d'intervalle ; Sulphuric Poisoning
  12 s (gouttes et ruées).
- **[technique]** Recettes du wiki qui ne peuvent pas encore se faire, faute d'ingrédient : Onyx Purge (Onyx Gun Parts,
  sans page), Celesticus (Ogscule : son unique recette demande la bannière de l'Ogsculian Burrower, ennemi pas encore
  fait), Flaming Ice Bow (Ice Bow et Sharanga : ni page, ni objet de
  Terraria ou de Calamity) ; Pastranion à vérifier (Spaghetti). Le Wulfrum Mining Helmet se fait depuis le Wulfrum
  Excavator (Wulfrum Lenses). Retirées du contrôle « recette du wiki » de l'auto-test. Les autres ingrédients manquants (Bucket of
  Coal, Fracturing Fist, 22 Hornet Round, Flaming Ice Bow, Baow, Contaminated Dish) sont faits au lot I.
- **[choix]** **Pizza Wheel** (lot G) : infobulle inventée « Summons pizzas that roll across the screen » (pas sur le wiki).

### Pyrogen (`Content/NPCs/Pyrogen/`, 19/11)

Boss inventé par la page Pyrogen (dans Calamity, « Pyrogen » n'est que le nom de Cryogen sur la graine Zenith). Valeurs de
l'infobox : vie {{dv|44 000|52 000|66 600}}, contact {{dv|70|130|145|172}}, Magma Blast {{dv|46|80|96|108}}, éclats
{{dv|42|54|78|112}}, défense 24, DR 20 %, 20 or, immunités et debuffs de l'infobox ; butin par WikiLoot.

- **[choix]** Invocation : une Cryo Key **qui touche la lave** en Hardmode (n'importe où), consumée ; Pyrogen apparaît
  près du joueur le plus proche (`NPC.SpawnOnPlayer`). Pas de condition de zone.
- **[choix]** Bouclier : détruit les projectiles des joueurs à 110 px du centre (pas les sbires, sentinelles, lances et
  projectiles tenus : la mêlée le traverse, comme le dit la page) ; dessiné avec l'image du bouclier de Cryogen en
  obsidienne. Absent quand il est « séparé » (4 s après la ruée de la phase 1, 3 s à la fin de la phase 2), en phase 3
  (les morceaux montent du sol) et pendant le Hellstorm ; écarté en phase 4 (8 morceaux à 180 px qui blessent et
  arrêtent les projectiles qui les touchent, le centre est touchable).
- **[choix]** Rythme : vitesse x(1 + 0,5 x vie perdue), x1,1 en Expert, x1,15 en Revengeance, **x2 hors de l'Underworld
  et du Brimstone Crag** (« enrage »). Phase 1 : un tour en 3 s à 450 px, 13 éclats tous les 4 s, 4 tours ; phase 2 :
  2 salves de 10 éclats (plantés 1,5 s puis 6 éclats), 3 ruées (4 en Rev.), 12 Magma Blasts tous les 45 images pendant
  3 s ; phase 3 : 8 s, rangées de 9 morceaux tous les 170 px ; phase 4 : 10 s, Magma Blast toutes les 30 images,
  téléportation toutes les 5 s (endroit annoncé 2 s avant, devant la trajectoire du joueur). Death Mode : téléportations
  dans toutes les phases.
- **[choix]** Hellstorm : une fois, sous 44 % (55 % en Revengeance) ; 8 s ; attire le joueur (0,3 par image à moins de
  100 blocs) ; anneau de 16 éclats à 750 px toutes les 40 images ; amas final de 30 éclats. Le bouclier revient ensuite.
  Contact : {{dv||300|500|6666}} et Dragon Fire 30 min (valeurs de la page, voir « décisions en attente » de TESTS.md).
- **[choix]** Musique : celle de la Pumpkin Moon (la page : « If the Calamity Music add-on mod is disabled »). Icône de
  carte : `Pyrogen-IconT2.png` du wiki.
- **[choix]** Taming : « Lead Pyrogen into the Surface Snow biome » = neige **et** hauteur de surface ; 3 s cumulées en
  Full Fandom (comme Cryogen), 20 s en Calamity++ (`Balance/taming.json`, comme Cryogen). Récompense (Frozen Heater) : pas
  d'objet, comme les autres récompenses en attente.
- **[technique]** Pas de sac au trésor (pas de page) : en Expert, le butin tombe directement (règle de WikiLoot pour un
  boss sans sac). Pyrogen Mask, Trophy, Lore, Pyro Stone, Hell Flare, Heated Profanity : pas de page, pas d'objet.
- **[choix]** Soul of Pyrogen : lances = épines Sharp Tears vanilla en orange, 5 sous la cible, au plus une salve toutes
  les 20 images. Seared Shredder : faux = texture de la Death Sickle en orange, ralentit après 20 images. Hell's Bells :
  1 place de sbire, carillon de 160 px toutes les 50 images, icône du buff = sprite de l'arme.

### Lot I du 19/11 (`Content/Items/Weapons/WikiWeapons1119f.cs`)

Pages d'addon (Community Remix selon le Fandom), traitées comme les autres (section 11) : ingrédients des recettes de
l'Interfacer Staff, de l'Exif Fist, du Twenty-Twloon, de l'Elemental Bow et du Pastranion. Valeurs des infobox.

- **[choix]** **Bucket of Coal** : charbon = Coal vanilla réduit, lancé dans le sens de rotation du seau ; explosion
  noire de 60 px à la moitié des dégâts ; en frappe furtive, taille plafonnée à x4.
- **[choix]** **Fracturing Fist** : vitesse 5 avec 1 mise à jour de plus (valeurs de la page) ; explosion de 80 px aux
  dégâts du poing, immunité locale (« ignores immunity frames ») ; moitiés lancées en arrière à 40 % ; frappe furtive :
  3 Crystal Shards vanilla à 10 % par rebond, ennemi suivant cherché à 600 px.
- **[choix]** **22 Hornet Round** : ricochet vers l'ennemi le plus proche à 62 blocs pas encore touché ; tracé jaune.
- **[choix]** **Flaming Ice Bow** : -1 PV par image tenu (60 PV/s, rien ne l'empêche, mort « burned their hands »), sauf
  pendant l'invincibilité sans clignotement ; Frostburn 3 s sur les flèches tirées avec lui.
- **[choix]** **Baow** : la seconde flèche part du symétrique du joueur par rapport au curseur, vers le curseur.
- **[choix]** **Contaminated Dish** : 3 nuages de 50 px (1 s) à 65 % ; 5 éclats de verre (sans dégâts ; en frappe
  furtive 10 % et Bleeding 5 s) ; frappe furtive lancée x1,6.

### Aquatic Hypercomputer (`Content/Tiles/Hypercomputer/`, 19/11)

Page d'addon (« Tales of the Aquatic », inventé) traitée comme les autres (section 11) : 154 recettes du wiki se font à
ses 6 étapes. La station (recette : Iron Anvil) se pose, puis chaque module (recettes du wiki) s'utilise sur elle pour
passer à l'étape suivante : Static Hypermodule, Capsule Vault Module, Elemental Capsule Box, Enchanted Reconstruction
Module, Abyssblast Laser Module.

- **[technique]** Une tuile par étape (`AquaticHypercomputer`, `...Static`, `...Vault`, `...Capsules`, `...ERM`,
  `...Finished`) ; chaque étape compte aussi comme les précédentes, la dernière comme Draedon's Forge (et ce qu'elle
  remplace). Les pages des étapes redirigent toutes vers *Aquatic Hypercomputer* : WikiRecipes cherche d'abord une tuile
  de ce mod au nom de la station (lettres seulement), avant les redirections.
- **[technique]** Tuile de 4 x 3 (infobox) ; l'image de chaque étape (82 x 74, l'étape 1 animée en 6 images) est
  dessinée entière, posée sur le sol et centrée.
- **[choix]** Un module s'utilise en visant la station à portée de pose, à l'étape juste avant ; il est consommé. Casser
  la station rend la station et tous ses modules.
- **[choix]** Capsule Vault Module et après : laser de 20 dégâts (Slow 2 s) toutes les 40 images vers l'ennemi visible le
  plus proche à 25 blocs. Abyssblast Laser Module : Evercold 2 s chaque seconde aux ennemis dans le carré de 20 x 20 blocs
  centré sur la station.
- **[choix]** Prix de vente de la station : {{value|27|50}} de l'infobox (27 platine 50 or), gardé tel quel ; modules sans
  prix (le tableau n'en donne pas). Raretés du tableau.
- **[choix]** Evercold (page du debuff) : sur un ennemi, -200 PV/s, +20 % de dégâts reçus (« -20 % de réduction des
  dégâts ») et -20 % de vitesse (pas sur les boss).

### Wulfrum Excavator (`Content/NPCs/WulfrumExcavator/`, 19/11)

Boss pré-Hardmode inventé (ver). Valeurs de l'infobox : vie {{dv|2 000|3 500|5 000}}, dégâts tête {{dv|20|28|34|40}},
corps {{dv|10|16|20|25}}, queue {{dv|5|8|16|20}}, défense 0 / 4 / 6, DR 5 % (queue), 10 or, immunités ; butin WikiLoot.

- **[choix]** Invocation : frapper une **Wulfrum Pylon** (ennemi généré) avec une pioche ; il surgit 600 px sous elle et la
  mange. La page du boss demande en plus « an Ogscule in the player's inventory », celle de l'Ogscule dit l'inverse (« no
  Ogscule ») : condition ignorée (l'Ogscule n'existe pas encore). À trancher si l'Ogscule est fait.
- **[choix]** 8 segments de corps (d'après l'image du ver entier) ; écarts 38 px (corps) et 44 px (queue).
- **[choix]** « Cran de difficulté » : normal 0, Expert 1, Revengeance 2, Death 3 ; supercharged = +1 (« boosting its AI to
  the next difficulty stage »), 12 s (comme les ennemis de Calamity), à 495 px d'une Wulfrum Pylon ou d'un Wulfrum
  Amplifier ; reflet cyan sur les segments. Seuils de vie, lasers d'anneau ({{dv|6|8|10|12}}) et ennemis appelés aux
  changements de mouvement ({{dv|0|2|3|4}}) lus à ce cran.
- **[choix]** Mouvements : ver (fonce dans le sol, retombe hors des blocs) ; cercle de 160 px de rayon, hexagone de lasers
  toutes les 120 images ; rectangle de 1640 x 840 px autour du joueur (le serveur ne connaît pas l'écran), salve de 5 lasers
  à chaque coin ; lemniscate de 360 px, 300 px au-dessus du joueur. Rev. à 5 % : change de mouvement toutes les 5 s.
  Death : x1,4 de vitesse, lasers x1,2.
- **[choix]** Lasers du corps : chaque segment tous les 300 images (x 1 / 0,8 / 0,65 / 0,55 selon le cran), une chance sur
  3, s'il est à moins de 900 px ; dégâts {{dv|20|30|40|56}} (la page ne donne que Rev. et Death).
- **[choix]** Résistances de la page : grenades -80 % ; en Expert, projectiles qui percent : dégâts / nombre de percées
  (50 % pour l'infini), sauf sbires.
- **[choix]** Taming : la tête creuse tout filon de minerai qu'elle touche (cases du même minerai qui se touchent, 200 au
  plus ; les minerais tombent) ; 10 filons. Tout minerai compte (le jeu ne distingue pas un minerai naturel). Récompense :
  Wulfrum Lenses (« Pacifist Drop »).
- **[choix]** Aftermath : Energy Orb à 1 % sur les ennemis Wulfrum (Calamity et générés), Desert Medallion à 10 % sur les
  Cnidrions (chance non donnée). La pousse des minerais pré-Hardmode est en attente (TESTS.md). Wulfrum Lure seulement
  s'il meurt « supercharged ».
- **[choix]** Wulfrum Lenses : lumière bleue devant le joueur et effet Spelunker. **[technique]** Rendu porté provisoire
  (icône réduite), sprite demandé. Wulfrum Fusion Array : rafale de 3 tirs (use 10), portée ~25 blocs, 3 ennemis
  traversés, tag de 4 dégâts pendant 4 s pour sbires et sentinelles, 6 de pénétration d'armure.

### Lot J du 19/11 (`Content/Items/Weapons/WikiWeapons1119g.cs`)

Valeurs des infobox. Sources pas encore dans le jeu : Acidsighter (Acid Bow, Caustic Claw), Ceasing Void (Steel Blade),
Sigmund (Steel Kunai), Grand Thunder Cnidrion (Talon Burst), XB-10 Nortazzo (Titanslayer), le Collector (Swift Sword,
avec son Blueprint) : ces armes ne s'obtiennent pas encore (TODO avec eux). Butcher : recette du wiki.

- **[choix]** Acid Bow : 2 flèches à ±0,04 rad ; flèche sulfurique = Venom Arrow vanilla teintée en vert.
- **[choix]** Butcher : cadence x1 → x6 et dispersion 0,25 → 0 rad en 4 s de tir continu ; remise à zéro dès qu'on lâche.
- **[choix]** Talon Burst : vitesse 10 (la page n'en donne pas), Storm Bolts à vitesse x2, 2 coups (4 images d'écart).
- **[choix]** Titanslayer : 8 traits par utilisation (use 40 = 8 x 5 images), vitesse 14 ; une utilisation sur 3 :
  dégâts x6 (« +500 % »), taille x2, +20 % de critique, 4 âmes (Spectre Wrath vanilla) à 25 %.
- **[choix]** Aspid Blaster : 3 orbes à ±0,18 rad (Ball of Fire vanilla en orange), gravité 0,15.
- **[technique]** Caustic Claw : vrilles de la Shadowflame Hex Doll vanilla (comme dit la page), Irradiated 3 s en plus.
- **[choix]** Steel Blade : éclats = Throwing Knife vanilla, 2 ennemis, 1 s, déplacés avec le joueur ; bonus = dégâts de
  contact du dernier ennemi touché par la lame (gardé jusqu'au prochain coup).
- **[choix]** Swift Sword : bonus proportionnel à la vitesse, maximum à 20 px/image ; Swiftness vanilla 10 s par ennemi tué.
- **[technique]** Calamity Sword : « direct upgrade to the Swordsplosion », mais aucune recette sur le wiki : pas
  obtenable (décision en attente).
- **[choix]** Calamity Sword : x3 partout dans le Brimstone Crag (« up to ») ; 5 fragments, chercheurs après 40 images.
- **[choix]** Steel Kunai : 15 images de vol, légère attirance à 200 px ; frappe furtive : 2 s, chercheur, explosion de 70 px.

### Acidsighter (`Content/NPCs/Acidsighter/`, 19/11)

Boss pré-Hardmode inventé. Valeurs de l'infobox : vie {{dv|4 000|6 200|8 760}}, contact {{dv|25|30|35|40}} (+25 % en
phase 3), Acid Bomb {{dv|15|20|22|25}}, Acid Shot {{dv|20|23|30}}, Teeth Bomb et dents {{dv|30|33|36|40}}, défense 10,
20 or, immunités, Irradiated 3 s au contact ; Mutated Eye : vie {{dv|30|45|55}}, dégâts {{dv|14|28|36|44}}, défense 3.
Butin par WikiLoot ; sac en Expert (Treasure Bag (Acidsighter), sprite du wiki).

- **[choix]** Invocation : Poisoned Sclera (pas consommé) dans la Sulphurous Sea, la nuit.
- **[choix]** Attaques jouées à tour de rôle, dans l'ordre de la page, avec les seuils de phase du {{dv}} ; phase 1 sous
  {{dv|65|70|75}} % : un tour + charge toutes les 3 attaques. Charges : 0,5 s de visée, ruée à 16 (x1,3 en Death),
  freinage. Expert : charge avec un souffle d'Acid Venom (Venom vanilla {{dv||3|4}} s, 1 dégât) ajoutée à chaque phase.
- **[choix]** Fureur : x1,5 hors de la Sulphurous Sea et de l'Océan, x1,5 le jour (cumulés), x1,1 en Revengeance.
- **[choix]** Death : le nuage d'acide des Mutated Eyes n'est pas fait (seulement la vitesse des charges).
- **[choix]** Taming : **absent de la page Taming** ; proposition : le garder 20 s (cumulées) dans l'Océan propre (pas la
  Sulphurous Sea), dans les deux modes. À valider.
- **[choix]** Armes : Pungent Bomber (bombe posée sur l'ennemi frappé, inactive 20 images, 4 gouttes à 50 %), Corrosive
  Eye Staff (yeux en cercle de 70 px, une goutte toutes les 50 images à 500 px), Razor Teeth (rareté verte, 2 gouttes à
  50 % par coup ; furtif : 3 dents chercheuses plantées 2 s, Venom 2 s pour « Acid Venom »).

### Lot K du 19/11 (`Content/Items/Weapons/WikiWeapons1119h.cs`)

Armes qui sont des ingrédients de recettes du wiki (pages d'addon comprises). Valeurs des infobox.

- **[choix]** Arngren : dispersion de ±22,5° (« 45 degree random spread »).
- **[choix]** Flounder Mortar : jet d'acide en poussière verte, 2 ennemis, 3 mises à jour par image.
- **[choix]** Megaskeet : 1 tir sur 2 = jet de lumière (x1,3 de vitesse, perce tout), 1 sur 17 = Sunskater (sprite du
  wiki, x2 dégâts) qui vise la position future de l'ennemi le plus proche ; 2-3 éclats au tiers, Holy Flames 3 à 6 s ;
  ces tirs ne consomment rien, les autres 50 %.
- **[choix]** Confection : utilise des flèches mais tire toujours le gâteau (Slice of Cake vanilla) ; 5 globes au tiers
  (texture du Slime Gun), 10 s, 40 par joueur au plus.
- **[choix]** Dual Cane : hélice de 36 px d'amplitude, 2 ennemis par orbe ; orbes dessinés en poussière.
- **[choix]** Ichor Dagger : gravité 0,35 ; frappe furtive : un jet d'ichor (Golden Shower vanilla) toutes les 8 images à un tiers.
- **[choix]** Heaven Piercer : hache 180 % ; laser du clic droit : 40 images à 4 mises à jour (~ 40 blocs), casse ce qu'une
  pioche de 275 % casse, pas les coffres.

### Lot L du 19/11 (`Content/Items/Weapons/WikiWeapons1119i.cs`)

Ingrédients de recettes du wiki (pages d'addon comprises). Valeurs des infobox.

- **[choix]** Onyx Gunblade : avec des balles (cherchées comme une arme à feu, non consommées), chaque coup tire un Black
  Bolt (145 %, comme l'Onyx Blaster de Calamity) et 2 à 7 balles en éventail vers le curseur, en dégâts à distance.
- **[choix]** Onyx Staff : 3 traits à ±0,12 rad, explosion de 60 px à la moitié des dégâts.
- **[choix]** Thunder Bolt / Aero Bolt / Plasmaflash Bolt : même projectile ; 4 / 3 / 10 ennemis, 4 / 3 / 0 rebonds ;
  Plasmaflash à vitesse x2 (« fast-moving ») avec l'image du wiki ; Electrified 3 s / rien / 10 s.
- **[choix]** Rose Bow : éventail de ±0,06 rad ; flèche d'énergie = Chlorophyte Arrow rose ; 3 entailles à 8 images.
- **[choix]** Pumplerfaust : citrouille vanilla blanchie, pleine en 5 s ; explosion de 60 à 160 px, dégâts x1 à x2 ;
  pleine : brume de 200 px pendant 5 s à 30 %.
- **[choix]** Gourdler : une grenade de plus toutes les 36 images (8 au plus) ; grenades = citrouille verte ; 6 épines
  (Sharp Tears vanilla) à 30 % au contact d'un ennemi.
- **[choix]** Celestial Nuke : aura de 300 px pendant 3 s (55 %), 8 braises (40 %, God Slayer Inferno de Calamity 5 s pour
  « Embers of Duality »), 4 traits chercheurs (80 %) ; frappe furtive : dégâts x1,5, deux fois plus de braises et de traits.

### Lot M du 19/11 (`Content/Items/Weapons/WikiWeapons1119j.cs`)

Ingrédients de recettes du wiki. Valeurs des infobox.

- **[choix]** Dark Energy Staff : tourelle (image d'énergie noire de la Void Concentration de Calamity), 3 boules chercheuses
  toutes les 20 images à 700 px ; deux au plus (la plus ancienne disparaît).
- **[choix]** Jet Engine : poing (sprite du wiki) qui fonce à 18, rebondit au contact ; téléporté près de la cible au-delà
  de 1200 px (75 blocs).
- **[choix]** Profaned Nucleus : explosion de 80 px à 50 % par ennemi touché, Holy Flames 3 s ; 6 cristaux à 30 % ;
  frappe furtive : une flaque de magma (25 %, 3 s) toutes les 20 images.
- **[choix]** Unsealed Singularity : fioles qui éclatent après 40 images ; trou noir (moitié des dégâts, texture de l'Event
  Horizon) qui attire les non-boss à 300 px (+25 % par autre trou noir), matière violette toutes les 30 images ; frappe
  furtive : une fiole x2, trou noir x2,5 (600 px), matière x2.
- **[choix]** Twisted Netherite Scythe : dégâts sans classe ; 10 images d'invincibilité au coup.
- **[choix]** Wrath of the Eldritch / of the Cosmos : pulsation de 6 s (image du wiki), salves de 2 anneaux toutes les
  90 images (esprits au tiers / boules de God Slayer au tiers, God Slayer Inferno 2 s) ; toutes les 3 salves, rayon (Last
  Prism vanilla) vers l'ennemi le plus proche / deux Cosmic Guardians (grosses boules x2, 3 s).

### Grand Thunder Cnidrion (`Content/NPCs/GrandThunderCnidrion/`, 19/11)

Boss pré-Hardmode inventé, « alternative au Desert Scourge ». Valeurs de l'infobox : vie {{dv|1750|2300|2800}}, contact
{{dv|50|70|90}}, éclairs du ciel {{dv|24|48|72}}, tirs {{dv|30|60|90}}, défense 6, DR 5 %, 50 argent ; butin WikiLoot.

- **[technique]** Le champ `name` de l'infobox et une note des « Tips » sont du vandalisme offensant visant une personne
  réelle : ignorés (nom affiché = titre de la page, rien d'autre de fait). Notre page wiki/ est nettoyée (nom, nom
  de l'IA, « bags of fent » → « lightning bolts », note retirée).
- **[choix]** Invocation : le Desert Medallion de Calamity, **le jour**, dans le désert, invoque le Grand Thunder Cnidrion
  au lieu du Desert Scourge (page du médaillon : « Grand Thunder Cnidrion during the day and the Desert Scourge at
  night ») ; dans les deux modes. La nuit, rien ne change.
- **[choix]** Attaques en boucle : éclairs du ciel (12 en 2 s autour du joueur), charge horizontale (tremble 0,5 s), 3 Storm
  Cnidrilings, et sous 45 % des salves de 3 tirs ; vitesse x1,4 sous 45 %. Sans ligne de vue : un trait électrique qui
  traverse les blocs toutes les secondes (x2 dégâts de tir). Expert : chaque éclair du ciel fait naître deux tornades au sol.
- **[choix]** Storm Cnidriling (pas de page) : vie 60, dégâts 20, défense 2, fonce sur le joueur ; sprite du wiki.
- **[choix]** Taming : **absent de la page Taming** ; proposition : 15 s cumulées dans l'Océan (un Cnidrion craint l'eau,
  infobox : vulnérable à l'eau). À valider. Aftermath (Cactus Person) : PNJ pas encore fait.

### Lot N du 19/11 (`Content/Items/Weapons/WikiWeapons1119k.cs`)

Objets lâchés par des ennemis déjà dans le jeu (obtenables tout de suite par WikiLoot) : Infected Dagger Misses No Skeptics
et Varroa Hack (Plaguebringer Goliath), Cursed Spear (Clinger), Earthen Clicker (Earth Elemental), Nightform (Wall of
Flesh), Ghast Relic (Clam), Purgatory Blaster (Providence), Dragon Flame Barrage (Yharon). Valeurs des infobox. Les autres
armes du Plaguebringer Goliath (Plaguecom Tablet : dessin à l'écran ; Death-Needle Stinger : aiguilles enchaînées ;
Cirrhotipede : fouet ; Hi-Scan Camera : modes non décrits) attendent : systèmes trop longs pour cette passe.

- **[choix]** Infected Dagger : chercheuse à 600 px, perce à l'infini (30 images entre deux coups sur un même ennemi),
  repart vers l'ennemi le plus proche ; frappe furtive : 4 dagues en éventail, vitesse x1,4.
- **[choix]** Cursed Spear : vole droit 30 images puis tombe ; frappe furtive : une boule de feu maudite (vanilla) toutes les
  6 images au tiers.
- **[choix]** Varroa Hack : consommable, sans classe ; accrochée 3 s (un coup toutes les 15 images), puis explosion de 80 px
  à 150 %.
- **[technique]** Earthen Clicker : pas de classe « clicker » (autre mod) ; dégâts sans classe au curseur, à 30 blocs et
  en ligne de vue, un clic par utilisation. Infobulle inventée (la page n'en a pas) : « Click to damage the enemy under
  your cursor ».
- **[choix]** Ghast Relic : 10 % par coup de sbire, fantôme de 5 s (Spectre Wrath vanilla), 3 au plus, 20 dégâts par
  seconde (un coup de 20 par seconde).
- **[choix]** Purgatory Blaster : utilise les fléchettes ; 3 boules de feu à 50 % qui tournent de ±0,04 rad par image.
- **[choix]** Dragon Flame Barrage : 8 flammes depuis le haut de l'écran au curseur, explosion de 90 px 30 images après
  l'arrivée.

### Lot O du 19/11 (`Content/Items/Weapons/WikiWeapons1119l.cs`)

- **[choix]** Yggdrasil Core (Yggdrasil Ent) : 10 défense, aura de 12 blocs, Holy Flames 2 s toutes les 30 images.
- **[choix]** Babil Destruction Core (Floating Biomass) : 5 s après chaque coup reçu (« every time you take damage »).
- **[choix]** Spear of Liberty (recette du wiki, Diviner Valkyrie, Corruption) : lance du milieu = Cursed Spear ; les deux
  autres à vitesse x1,3, chercheuses à 700 px ; frappe furtive : toutes chercheuses, à travers les blocs, 4 ennemis.
- Pas faits : Devious Aestheticus (Moon Lord ; page trop vague : tirs, « split », mêlée non décrits), The Last Lightice
  (Snow Balla absent), Wakasagihime (Harpy ; système de « Karma » qui appelle des boss).

### Lot P du 19/11 (`Content/Items/Accessories/WikiCoverings1119.cs`)

« Coverings » fabricables (recettes du wiki, dont l'Aquatic Hypercomputer). Valeurs et effets des pages.

- **[choix]** Blazing Powder : une attaque à distance sur deux (compteur par joueur) lance une boule de feu astrale vers le
  curseur, 25 dégâts x bonus à distance, Hellfire 5 s.
- **[technique]** BFB : 3 sauts en plus (Cloud, Blizzard, Sandstorm in a Bottle) = quadruple saut ; ensuite 5 s de poussée
  vers le haut en tenant Saut (rendues au sol) au lieu d'ailes (pas d'image d'ailes) ; pas de dégâts de chute, sauts
  boostés, saut automatique.
- **[choix]** Magna Core : sphères de granite chercheuses (20 000 x bonus magiques), lancées par toute potion qui rend du
  mana ; 20 s d'attente.
- **[choix]** Quiver of Madness : munitions jamais consommées ; 4 portails à 90 px ; projectile à distance passé par un
  portail : dégâts x3 une fois, chercheur à 900 px. Les cibles-crânes (+500 %) ne sont pas faites.
- **[choix]** Socratic Sapphire : critique total et réduction des dégâts divisés par deux ; « coup sauvage » = dégâts
  critiques x5 (au lieu de x2), chance 10 % + (critique + réduction x 100) / 2, au plus 75 % ; debuffs 3,33 fois plus
  courts (sauf ceux que l'infirmière ne peut pas retirer).
- **[choix]** Cobalt Night Circuit : « flotter » = tenir Saut en tombant à moins de 5 blocs du sol ; aura de 5 blocs, 60
  dégâts toutes les 30 images (120/s). Palladium Light Circuit : stats de la page. Pas d'apparence de mech (pas d'image).
- **[technique]** Terraformance : la page recopie le texte de la Nightform ; +25 % de mana tout le temps (son infobulle ne
  parle pas de la nuit).
- **[choix]** Earth Destruction Core : comme la Babil Destruction Core (même compteur de 5 s), valeurs plus fortes.

### Lot Q du 19/11 (`Content/Items/Weapons/WikiWeapons1119m.cs`)

Armes fabricables à l'enclume (recettes du wiki). Valeurs des infobox, sprites des sbires du wiki.

- **[choix]** Fracture Staff : le Fractal Crawler bondit vers la cible toutes les 35 images (gravité entre deux) ; 3
  cristaux (Crystal Shard vanilla) à 20 % par coup.
- **[choix]** Holy Mackerel Staff : demi-place ; fonce à 16, frappe toutes les 10 images, Holy Flames 2 s ; en solo, ignore
  les cibles hors de l'écran (page) — en multijoueur l'écran du serveur n'existe pas, la règle est ignorée.
- **[choix]** Infernum Kris : 2 places, un seul ; couteau rouge (Throwing Knife vanilla) toutes les 12 images ; au-delà de
  600 px, téléportation 160 px au-dessus de la cible, 8 couteaux en cercle et un coup vers le bas x2.
- **[technique]** The First Fracturer : estoc à l'ancienne (Thrust) ; rayon bleu ; au contact, 3 à 5 rayons à 50 %
  arrivent de 900 px autour de l'ennemi (eux ne se multiplient pas).

### Lot R du 19/11 (`Content/Items/Weapons/WikiWeapons1119n.cs`)

Armes fabricables à l'Ancient Manipulator (recettes du wiki). Valeurs des infobox ; « Embers of Duality » = God Slayer
Inferno de Calamity.

- Unloxcalibur, Aspect of the End : épées sans effet (pages).
- **[choix]** Gamma Ray Burst : yoyo (texture du Kraken teintée), immunité locale de 4 images, portée 320 ; infobulle inventée
  (« Ignores enemy immunity frames », la page n'en a pas).
- **[choix]** Dark Harvest Staff : citrouille vanilla assombrie, rebonds (x0,8), 2 à 4 morceaux à 20 %.
- **[choix]** Prominence : 4 lasers droits et 2 lances chercheuses depuis 500 px autour du curseur.
- **[choix]** Chrononimbus : 5 lames en éventail ; 2 flammes chercheuses à 30 % par coup.
- **[choix]** Crius Prime : clic droit = charge jusqu'à 2 s (2 mana / 10 images), dégâts x1 à x5, 5 rebonds.
- **[choix]** Bolide Launcher : 2 Galactica Comets (sprite du wiki) à 250 % par tir.

### Lot S du 19/11 (`Content/Items/Weapons/WikiWeapons1119o.cs`)

Armes fabricables à la Cosmic Anvil (recettes du wiki). Valeurs des infobox ; « Moon Burnt » = Nightwither.

- **[choix]** First Crescent : croissant de 2 s (3 ennemis), un fragment à 50 % toutes les 8 images dans un sens au hasard.
- **[choix]** Wrath of the Dragons : même pulsation que les Wrath du lot M (teintée orange) ; 2 anneaux de 32 poussières
  de feu (tiers des dégâts, Dragonfire 3 s) ; deux Flare Tornadoes (x2) toutes les 3 salves (« seven rounds » sur la page,
  mais la pulsation n'en fait que 4) ; Flarenado de 3 s au contact (frappe toutes les 15 images).
- **[choix]** Rivers of Frost : coup = 1 dégât + 1 % de la vie max retiré directement (2 % imprégnée, 0,5 % sur
  Providence) ; le retrait ne tue jamais seul (le coup d'1 dégât s'en charge) ; clic droit : 6,5 % de la vie max (coup non
  esquivable), tout le mana, 1 min d'imprégnation ; lame de givre : 3 ennemis, 0,5 %, n'atteint pas Providence.
- **[choix]** The Otherside : déchirure de 60 px au bout de la lame à chaque coup, 10 s, 5 au plus ; fusion des déchirures
  qui se touchent (+30 px, +15 % par fusion, 100 % au plus).
- **[choix]** Ecological Collapse : mines (sprite de l'arme) ; aura de 320 px : 10 % des dégâts toutes les 10 images
  (immunité locale), Electrified 1 s ; éclats à 50 % ; frappe furtive : au-delà de 10 mines, 8 s, 20 à 30 éclats.

### Lot T du 19/11 (`Content/Items/Weapons/WikiWeapons1119p.cs`)

Objets fabricables aux premières étapes de l'Aquatic Hypercomputer (recettes du wiki). Valeurs des infobox.

- **[choix]** Microhavoc : utilise du gel (comme les lance-flammes), une consommation par utilisation ; cadence fixe (page) ;
  rayon qui perce les ennemis et s'arrête au 3e bloc traversé, Brimstone Flames 4 s.
- **[choix]** Frightshooter : salve de 3 tirs (4 images) puis 12 d'attente ; fléchettes à 2 mises à jour par image.
- **[choix]** Crystallic Coin Gun : dégâts des pièces, des explosions (50 px) et nombre d'éclats de la table de la page, avec
  les bonus à distance ; éclats à 20 dégâts fixes.
- **[choix]** Landscapist / Seascapist Mobilite Fragment : double appui gauche ou droite, ruée de 20 images à 20 px/image
  (~25 blocs), 80 images d'attente ; Landscapist au sol et hors des liquides, Seascapist sous l'eau ; traînée de 30 dégâts
  (Plague 1 s / Suffocation 2 s).
- **[choix]** Shatterspectre Tracers : vitesse de course max 6,75 (comme les Lightning Boots) ; Crush Depth 2 s sur tous les
  coups ; fée (25 dégâts, pétale toutes les 40 images). Debuffs d'eau et de glace renforcés : pas fait.

## 8 bis. Shortstrike (`tools/gen_shortstrike.py`, `Common/Shortstrike/`, page Shortstrike)

- **[validé 22/10]** Système créé d'après la page : jauge de 100 points (1 point tous les 10 dégâts d'épée courte et par
  coup qui touche), bonus en tenant une épée courte, Shortflurry au clic droit quand elle est pleine. Le tableau de la page
  est lu par le générateur ; chaque effet écrit en anglais devient une donnée (défense, réduction de dégâts, vitesses,
  critique, debuff au coup, soin par coup, vol de vie, buff, immunités, projectile au coup). 17 effets pas encore traduits
  (rayons, lianes, lunes, nuages... : `reports/shortstrike.md`), surtout pour des armes qui n'existent pas encore.
- **[validé 22/10]** Calamity++ : bonus de la jauge divisés par deux (`Balance/shortstrike.json` : +1 % tous les 10 points,
  +1 défense tous les 6) ; Shortflurry identiques au wiki.
- **[validé 22/10]** Armes du tableau absentes du jeu (Tomutus, Night's Stabber, Terra Shiv...) : créées plus tard, une
  par une ; leur ligne est prête.
- **[choix]** Épées courtes hors tableau : un Shortflurry générique (+10 %, 3 coups, 1 s) pour les autres mods ; un seul
  sur mesure, **Sahara Slicers** (Calamity++, l'arme est masquée en Full Fandom) : +25 %, 5 coups, 2 s, chaque coup relance
  ses éclats de sable (`tools/shortstrike_extra.json`). Toutes les épées courtes vanilla et de Calamity sont déjà dans le
  tableau, la liste n'a donc pas de doublon.
- **[choix]** « Épée courte » = arme du tableau, ou arme maniée en estoc (useStyle Rapier) ou qui tire un projectile
  d'épée courte de Calamity.
- **[choix]** Clones magiques : l'épée part du joueur vers le curseur (écart aléatoire de ±20°) et revient (sans teinte
  depuis le 28/10, voir plus bas) ; les coups sont répartis sur la durée ; un coup par clone et par ennemi. Les clones ne remplissent pas la
  jauge. Effets de stats actifs pendant la durée du Shortflurry ; buffs et immunités « for N seconds » à leur propre durée.
- **[choix]** « Attacks inflict X » sans durée : 5 s. « Hits drain 3% health » : 3 % de la vie restante de l'ennemi, au plus
  5 fois les dégâts du coup. Projectile « au coup » : moitié des dégâts du coup.
- **[validé 22/10]** Affichage : jauge dessinée par l'utilisateur (`Assets/UI/ShortstrikeMeter.png`, source x6 dans
  `wiki_sprites_custom/`), placée comme les jauges de Calamity (par défaut à droite de sa jauge de vol, à la même
  hauteur : sous l'adrénaline elle couvrait le texte des PNJ ; déplaçable
  sauf si Calamity verrouille ses jauges, position dans la config client du mod). **[choix]** Visible en tenant une épée
  courte ; les épées sortent quand elle devient pleine. **[validé 22/10]** Dessinée à 2× ; pendant un Shortflurry elle
  se vide sur la durée de l'attaque en tremblant (amplitude « RipperMeterShake » de Calamity), comme ses jauges. Aura orange pulsante autour du joueur. Points sauvegardés.
- **[choix]** (nuit du 24/10) Effets des phrases non traduites, pour les armes qui existent (`tools/shortstrike_extra.json` →
  `overrides`) : Galileo Gladius « Crescent hits spawn moons » = `GalileosMoon` de Calamity à chaque coup ; Cosmic Shiv « 20%
  chance to shoot a second beam » = son rayon `CosmicShivTrail`, tiré vers le curseur ; Submarine Shocker « stationary clouds »
  = nuage immobile de 4 blocs, 3 s, Sulphuric Poisoning, quart des dégâts ; Spore Knife « 4 tile long vines » = épines de 4
  blocs (voir plus bas). Reste Lucrecia (portée de son ADN) et les armes absentes.
- **[choix]** (28/10, retour de test) Les projectiles tirés par une épée courte, et leurs enfants, remplissent la jauge
  (« damage dealt using a shortsword ») : rayons du Cosmic Shiv, éclats des Sahara Slicers... Pas ceux des clones.
- **[choix]** (28/10) Cosmic Shiv : « a second identical beam » suppose un premier rayon ; chaque clone tire donc le rayon
  de l'arme (`CosmicShivTrail`) à chaque estoc, plus un second à 20 % (effet `spawnOnStab`, remplace le rayon « au coup »).
- **[choix]** (28/10) Nuages du Submarine Shocker dessinés avec ceux de la Toxic Flask vanilla ; liane de la Spore Knife
  = épine de jungle verticale qui sort du sol (images `ThornBase`/`ThornTip` de Calamity), à plat elle ne se voyait pas.
- **[validé 28/10]** La jauge pleine ne clignote plus ; son de départ du Shortflurry : Silva Activation (2,4 s) par défaut,
  les autres restent au choix dans la config client. Nuages du Submarine Shocker à 30 % d'opacité. Second rayon du Cosmic
  Shiv dévié de ±0,6 rad (il se superposait au premier). **[validé]** Effets du tableau plutôt que ceux des pages d'armes
  quand ils diffèrent (Galileo : +30 % de critique, pas +50 % de vitesse).
- **[validé 28/10]** Calamity++ : les épées courtes qui ont déjà une attaque au clic droit (Galileo Gladius, Sahara Slicers)
  ont un Shortflurry sur mesure, version renforcée de cette attaque ; jauge pleine ou Shortflurry en cours, le clic droit
  appartient au Shortflurry (`ShortstrikeAltBlock`). Galileo (**[validé]** au 2e retour) : grappin + traction gratuits
  (sans Stratus Starburst) en plus des 7 coups : un clone file vers le curseur, se plante dans le premier ennemi (3 étoiles
  Vega), tire le joueur jusqu'à lui, frappe une dernière fois et le joueur rebondit en arrière. **[choix]** Coup final x5,
  second éclat de 3 étoiles, traction 40 frames au plus, retour seul sans ennemi à 700 px. Sahara (**[validé]**) : tout le
  carquois (éclats dans le dos) part vers le curseur pendant la durée (l'éclat par estoc en plus : trop, retiré) ; les clones ont
  l'image de l'attaque (`SaharaSlicersBlade`/`Alt`) au lieu de l'icône. L'ancien « éclat au coup » laissait des éclats
  collés au dos du joueur (mode carquois) : retiré.
- **[choix]** (28/10) Full Fandom, effets des pages Galileo Gladius et Cosmic Shiv sur leur arme de 2.0.1.005 : chaque clone
  tire le projectile de l'arme (croissant de lune, un tiers des dégâts ; orbe cosmique, moitié des dégâts) ; les croissants
  des clones font pleuvoir des planètes (« crescent moons will drop down planetoids on enemy hits », même délai de 15
  frames) ; le Cosmic Shiv tire un second orbe à 20 %. Calamity++ : rayon `CosmicShivTrail`, et lunes au coup (tableau).
- **[validé 28/10]** Clones du Shortflurry = l'attaque elle-même (`Content/Shortstrike/ShortflurryEffects.cs`,
  `ShortflurryRealClone`) : pour une arme dont l'attaque est un projectile d'estoc (BaseShortswordProjectile) ou une épée
  tenue (BaseSwordHoldoutProjectile) de Calamity ou de ce mod, le clone est ce projectile, avec ses images, particules,
  lame lumineuse et effets au coup (étincelles du Submarine Shocker, rayon du Cosmic Shiv, spores de la Spore Knife...).
  **[technique]** Son AI est remplacé par une copie sans ce qui touche au joueur (arme tenue, direction, temps
  d'utilisation) ; dessiné comme un projectile normal ; un coup par ennemi ; dégâts figés à ceux du Shortflurry ;
  **[choix]** décalé de 40 px vers l'extérieur avec sa zone de coup (à la portée de l'arme, il restait collé au joueur). Les
  autres armes (épées courtes vanilla, Umbrella, Spore Knife rogue...) gardent le clone dessiné : l'objet, opaque, sans
  teinte, éclairé par le décor (l'attaque vanilla n'a pas de particules). Sahara Slicers : un de ses deux projectiles
  d'estoc au hasard (`cloneProjectile`). Cosmic Shiv : le premier rayon vient du clone réel, le second (20 %) reste un
  effet du Shortflurry ; Galileo (Full Fandom) : le croissant à chaque estoc reste un effet du Shortflurry (avec ses
  planètes), le croissant « estoc dans le vide » de l'arme est coupé pour les clones. **[choix]** Fil du grappin du
  Galileo en bleu ciel.
- **[validé 28/10]** Spore Knife : comme dit la page, les épines partent de l'ennemi touché dans une direction au hasard
  (4 blocs, touchent sur toute leur longueur). **[choix]** 1 à 3 par coup (nombre demandé par l'utilisateur), quart des
  dégâts du coup, Poisoned 2 s.
- **[technique]** Les points ne sont pas synchronisés en multijoueur : les autres joueurs ne voient pas l'aura (les clones,
  eux, sont synchronisés).

## 9. Outils de debug (`Content/Debug/`, sans recette)

- **[choix]** Fandom Test Vial : applique 10 s le debuff choisi au joueur et à l'ennemi le plus proche (Moon Burnt,
  Marked, 15 statuts d'éléments).
- **[choix]** Fandom Test Dummy : 1 000 000 PV (sans mise à l'échelle de difficulté), immobile, taille x2, aucun dégât,
  neutre aux éléments, revient à fond sous 10 % de ses PV, affiche les debuffs de Calamity.
- **[technique]** (28/10) Fandom Test Dummy : image dessinée à la taille de la hitbox (36×96) ; il ne force plus les coups
  (`CanBeHitBy...` à `true` passait outre l'immunité : un estoc touchait 4 fois par passage).
- **[choix]** Test Swords : 18 (une par élément + Moon Burnt), 10 dégâts, `use` 15, autoswing, couleur de l'élément ;
  suivent les règles du mode (chances en Calamity++).
- **[choix]** (31/10) Fandom Time Dial : clic gauche, lever du jour (4 h 30) ou tombée de la nuit (19 h 30) ; clic droit,
  une heure plus tard. Réglé en solo ou par le serveur (pas depuis un client).
- **[technique]** Sprites vanilla réutilisés (bouteille d'eau, mannequin, épée de cuivre, cadran solaire) : pas de
  sprite à faire.

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

## 11. Addons (« community modifications », `reports/addons.md`) et sources de `OtherMods/`

- **[validé 29/10]** Règle générale : ce que le Fandom attribue à un addon est **faux ou erroné**. Soit c'est inventé de
  toutes pièces, soit c'est du contenu qui n'appartient pas à l'addon nommé. Une page « addon » se traite donc comme les
  autres pages du Fandom. Elle ne vaut pas description d'un mod réel.
- **[validé 29/10]** État de chaque addon (selon l'utilisateur) :

  | Addon | Réalité | Source dans `OtherMods/` |
  |---|---|---|
  | Calamity: Community Remix | existe ; une partie du contenu du Fandom y a été ajoutée, modifiée pour être jouable ; beaucoup de pages décrivent du contenu qui n'y est pas. Contient aussi Hypnos et Polyphemalus | `CalRemix/` |
  | Calamity Hunt | existe : ajoute Goozma, en **une** version ; le Fandom en décrit plusieurs, nous en ferons **3** | `CalamityHunt/` |
  | Deimos, the Starborn Princess | = **Everquartz' Adventure** ; beaucoup de contenu du Fandom n'y est pas | `EverquartzAdventure/` |
  | Cataclysm | existe, ajoute une partie du contenu du Fandom ; beaucoup de contenu du Fandom n'y est pas | `CataclysmMod/` |
  | Infernum | existe ; l'essentiel de son contenu n'est pas sur le Fandom (référence au cas où) | `InfernumMode/` |
  | Catalyst | existe, sans dépôt public : version très modifiée d'Astrageldon, éloignée de ce que nous ferons (pour le boss du moins) | — |
  | Sloome | existe, sans dépôt public | — |
  | Apotheosis & Friends | existe, sans dépôt public | — |
  | Calamity: Tales of the Aquatic | **inventé** en totalité | — |
  | Calamity's Souls | **inventé** | — |
  | Calamity's Plantation | **inventé** (pas de page, seulement évoqué) | — |
  | Legendaries+ | **inventé** | — |
  | Emphatic Untamed Calamity | **inventé** | — |
  | Fargo's Souls DLC (Fargo Souls Mod x Calamity) | le DLC n'a pas de dépôt (simples liens avec d'autres mods) ; Fargo's Souls existe, idées reprises du Fandom inconnues | `FargosSoulsMod/` |
  | Wrath of the Gods | existe (Avatar of Emptiness, Nameless Deity) | `WrathOfTheGodsPublic/` |
  | Calamity Loot Swap | existe, mais ne fait qu'échanger du butin : n'ajoute rien | — |
  | Hypnos in Calamity, Polyphemalus, Calamity's Vanities | existent ; Hypnos et Polyphemalus ont une version dans CalRemix (dépôts d'origine disponibles chez l'utilisateur si besoin) | — |
  | Wakasagihime | selon le Fandom, mod séparé d'Artienia ; l'utilisateur pense qu'elle est dans CalRemix (introuvable dans `CalRemix/` le 29/10) | — |
  | Tranquility / Caeles Mod, Distorted Memories, Voidmaster's Threnody, Profaned Birb, Yharim Worm, Calamitous Absolution, Bloop's Items, Calamitous Journeys, Buh Deez, Add-on content, « terraria fan ideas wiki (not) », Wrath of the Mammoth | **inventés** | — |

  Variantes d'écriture de `reports/addons.md` ramenées au même addon : Sloome (« Sloome addon », « Calamity: Sloome
  Mod », « Sloome Mod »), Community Remix (« Calamity Community Remix », « Communiy Remix », « Community Content
  Remix »), Tales of the Aquatic, Catalyst, Infernum, Wrath of the Gods (« Wrath Of The God's »...), Fargo's (« Fargo's
  Soul Mod »). La page *Community Modifications* cite aussi Bad Apple (Goozma Boss), Unofficial Calamity Mod Music,
  Bags, Calamity Texture Pack et des traductions : ils ne sont rattachés à aucune page de contenu.
- **[choix]** Licences des sources de `OtherMods/` (lues le 29/10). Ce n'est pas un avis juridique :
  - **CalamityModPublic** : propriétaire (Azafure). La reprise de code est permise avec le crédit habituel ; elle
    n'autorise ni les sprites ni la redistribution.
  - **CataclysmMod** (MIT, © 2020 Stevie), **InfernumMode** (MIT, © 2025 Javyz) et **FargosSoulsMod** (MIT, © 2024
    Fargo Team) : code réutilisable en gardant la
    mention de copyright et la licence MIT dans le fichier repris. Vérifier les sprites un par un : les artistes ne
    sont pas forcément couverts.
  - **CalamityHunt** et **EverquartzAdventure** (GPL v3) : code réutilisable (décision ci-dessous).
  - **CalRemix** et **WrathOfTheGodsPublic** : aucune licence dans le dépôt, donc tous droits réservés par défaut :
    **inspiration seulement**.
- **[validé 29/10]** Licences du dépôt séparées fichier par fichier (`LICENSE.md`, `REUSE.toml`, `LICENSES/`) :
  - notre code : **GPL v3 ou ultérieure** ;
  - code repris : sa licence d'origine (GPL v3 pour Calamity Hunt et Everquartz' Adventure, MIT, ou
    `LicenseRef-Calamity` pour le code d'Azafure dans `Content/Legacy/`, avec en-têtes SPDX) ;
  - wiki : **CC BY-NC-SA 3.0**, d'après la page « Calamity Mod Wiki:Copyrights » du Fandom. Elle interdit l'usage
    commercial, ce qui ne pose pas de problème pour un mod gratuit.

  **À revoir plus tard** (reporté par l'utilisateur) :
  - le mod compilé réunit dans une seule DLL du code GPL et du code adapté de Calamity, que la GPL ne permet pas de
    combiner tel quel ;
  - les sprites de Calamity et du wiki, qu'aucune licence ne permet de redistribuer.
- **[validé 29/10]** Contenu sans page ou à page vide (objet seulement cité, page sans contenu) : en Full Fandom,
  probablement absent, ou un objet sans effet. En **Calamity++**, on crée du vrai contenu (« plus de contenu = plus
  cool »). C'est à décider au cas par cas avec l'utilisateur, selon `docs/MODES.md`.
- **[validé 29/10]** Des pages du wiki n'apparaissent dans aucune liste de la page principale (exemple : le biome
  **Aether**). Pour savoir ce que le wiki contient, ne jamais s'en tenir aux navbox ni aux listes de la page d'accueil :
  partir de l'index de toutes les pages (`data/wiki_index.json`).
