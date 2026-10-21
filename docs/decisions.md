# Décisions

Journal des choix faits quand le wiki est contradictoire, irréaliste ou incomplet.
Une entrée par décision : page(s), problème, choix, et la page corrigée dans `wiki_edits/` s'il y en a une.

## Zweihandromeda — recette sans Night's Gaze

- **Problème** : la recette (`Recipes_Cosmic Anvil_register.txt`) demande Night's Gaze, un javelot qui n'existe plus
  dans Calamity 2.2.2 mais qui a sa page sur le wiki.
- **Choix** : recette provisoire sans Night's Gaze (TODO dans le code), à compléter quand Night's Gaze sera recréé.
- **Choix** : « swings in a back and forth pattern » = projectile tenu qui alterne un coup vers le bas et le même
  coup vers le haut, orienté vers le curseur (arc de ~220°). Le swing vanilla est inutilisable avec un sprite de
  232×230 : il place l'arme loin de la main.
- **Choix** : les étoiles tombent à chaque ennemi touché (2 par touche, demi-dégâts), quantité non précisée par le wiki.

## Elemental System — chiffres et règles non précisés par le wiki

Source : `Elemental System.txt` (17 éléments, une phrase d'effet chacun ; résistant = 50 % des dégâts et immunité à
l'effet, vulnérable = 200 %). Tout le reste est un choix, réglable dans `Common/Elements/Element.cs` :

- **Plusieurs éléments sur une arme** : multiplicateurs multipliés entre eux (feu vulnérable + poison résistant =
  neutre), bornés entre x0,25 et x4.
- **Statut** : chaque coup applique 2 s du statut de chaque élément de l'arme (sauf PNJ résistant, immunisé).
  Wind et Bomb n'ont pas de statut durable : leurs effets s'appliquent au coup.
- **Dégâts sur la durée** par palier de progression (pré-Hardmode / Hardmode / après Moon Lord) :
  Holy, Unholy 10/30/100 ; Radioactive, Slash 8/25/80 ; Fire, Electricity 6/20/60 ; Poison 5/15/45 ;
  Stab, Magic 4/12/40 ; Water, Acid 3/10/30.
- **Ralentissement** : Cold 20 %, Electricity 15 %, Water / Acid / Stab 10 % (moitié sur les boss, total plafonné à 50 %).
- **Affaiblissement des dégâts de contact** : Cold, Poison, Acid, Magic, Machine 10 %.
- **Défense** : Radioactive, Smash 15 % ; Fire, Acid 10 %. Radioactive bloque aussi la régénération.
- **Smash** : confusion vanilla. **Dark** : 20 % de chances de rater le contact (« making them miss »).
- **Wind** : recul x1,5. « Hurting them when they land » pas encore fait.
- **Bomb** : critiques x2 en dégâts, x1,5 en chance ; +10 % par ennemi déjà touché dans le même tick (max x2).
- **Machine** : +5 % de dégâts et +5 % de critique pendant l'utilisation de l'arme.
- Les projectiles héritent des éléments de l'arme (ou du projectile parent) ; les éléments des armures
  (« Elements » dans le tableau des Coverings) ne sont pas encore gérés.
- Noms des statuts tirés des verbes de la page : Chilled, Ablaze, Drowning, Envenomed, Corroded, Irradiated, Shocked,
  Punctured, Lacerated, Discombobulated, Hexed, Incapacitated, Smitten, Cursed, Malfunctioning.

## Matériaux générés automatiquement (`tools/gen_items.py`)

- **Portée** : pages d'objets absentes de Calamity de type Crafting material / Bar / Soul, plus les « Block » qui
  servent d'ingrédient dans une recette du wiki (les barres sont des blocs sur le wiki). 218 objets.
- **Les barres et blocs ne se posent pas encore** : ce sont des matériaux simples en attendant un système de tuiles.
- Rareté inventée (« C2 »...) → rareté 0. Prix = prix de vente du wiki x5. Pile = champ `stack` (9999 par défaut).
- Sprites de plus de 96 px réduits par un facteur entier (ex. « Mars Bars », une photo).
- Un matériau dont le nom existe déjà en vanilla n'est pas créé (ex. page wiki d'un objet vanilla).
- Pour donner un comportement à l'un d'eux (se poser, effet...), lui écrire sa propre classe et l'exclure du générateur.

## Butin du wiki (`tools/gen_loot.py`, `Common/WikiData/WikiLoot.cs`)

- **Additif** : on ajoute seulement les objets que le PNJ ne lâche pas déjà ; les chances des drops existants ne sont
  pas modifiées et rien n'est retiré.
- Chance et quantité du mode normal (premier terme de `{{dv}}`).
- Drops avec une condition en note (« Post-Golem », « Tier 3 », « Always on first kill »...) : **non appliqués**,
  listés dans `reports/loot.md`. À traduire en conditions de drop plus tard.
- Boss : ajouts limités au mode normal ; les sacs de trésor (Expert) ne sont pas encore modifiés.
- « One of the following N items » : un objet au hasard parmi le groupe, sauf si le jeu en donne déjà un.
- Un seul PNJ par page (celui qui a déjà le plus de butin) : évite de multiplier le butin des vers et des alias.

## Modes de fidélité (voir `docs/MODES.md`)

- **Référence « abandon du wiki » = Calamity 2.0.1.005** (commit `3ec8f553ed`, 30/12/2022) : des 29 objets ajoutés
  juste avant, 19 ont une page ; des 77 ajoutés par 2.0.2.001 (février 2023), un seul. Dernier changelog du Fandom : 2.0.1.002.
- Choix de l'utilisateur (21/10) : config serveur + monde verrouillé ; un seul mod à 3 modes ; contenu retiré porté
  depuis le vieux code ; Minimal par défaut.
- Un objet ajouté après 2.0.1.005 n'est pas masqué si le wiki le connaît d'une façon ou d'une autre : page, redirection
  (« Aerospec Hat » → « Aerospec armor ») ou recette du wiki qui le produit.
