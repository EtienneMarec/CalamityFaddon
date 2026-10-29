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

### Textures du wiki sur Calamity (`Assets/Overrides/`)
- **[validé 21/10]** Icône du wiki pour Moon Burnt (au lieu de celle de Nightwither), y compris dans l'affichage de
  debuffs de Calamity au-dessus des boss.

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
