# Checklist de test en jeu

À dérouler après chaque passe. Une section par passe ; on coche `[x]` ce qui est validé, on note `KO :` + description
sous une ligne qui ne va pas. Les tests restés ouverts des passes précédentes restent à faire.

Astuces : Cheat Sheet / Hero's Mod pour faire apparaître objets et PNJ ; `/calfandom` n'existe pas (encore).

---

## Passe 0 — Démo : Zweihandromeda

- [x] L'objet apparaît, nom « Zweihandromeda », sprite correct dans l'inventaire
- [x] L'épée est tenue par la poignée, dans la main
- [x] Coups alternés : un vers le bas, le même vers le haut, orientés vers le curseur
- [x] Des étoiles (Stratus Star) tombent du ciel sur l'ennemi touché
- [ ] L'ennemi touché reçoit le debuff Moon Burnt (= Nightwither de Calamity), 3 s, particules violettes
- [ ] Recette au Cosmic Anvil : Stratus Sphere, Galileo Gladius, Crescent Moon, 25 Lumenyl, 37 Depth Cells,
      4 Ascendant Spirit Essence, 8 Auric Bar (Night's Gaze manquant volontairement)

---

## Passe 1 — Fondations (nuit du 21/10)

### 1.1 Vocabulaire et renommages du wiki (`Localization/en-US/Mods.CalamityMod.hjson`)

- [ ] Le mod se charge sans erreur de localisation (pas de fichier renommé en `.legacy` dans `Localization/`)
- [ ] Murasama s'appelle **Graydron**
- [ ] Uelibloom Bar/Ore → **Tarragon Bar/Ore** ; Uelibloom Brick garde son nom
- [ ] Astrum Deus (boss, barre de vie, sac, trophée, masque) → **Astral Deus** ; Hide of Astrum Deus garde son nom
- [ ] Nightwither → **Moon Burnt** (survol du debuff sur un ennemi)
- [ ] Rogue Emblem → **Spy Emblem** ; infobulles des armes rogue : « spy damage », « Spy » au lieu de « Rogue »
- [ ] Contrôles : « Covering Ability 1..7 » au lieu de « Accessory Ability »
- [ ] Draconic Swarmer (invoqué par la Dragonfolly) → **Tempest Toddler**
- [ ] Les balises dans les infobulles s'affichent toujours (icônes `[i:...]`, buffs colorés `[cbuff:...]`)

### 1.2 Stats du wiki (`WikiData/stats.json`, 1033 objets, 67 PNJ)

Contrôlé automatiquement par `tools/selftest.py` (toutes les valeurs du JSON). En jeu, vérifier surtout le ressenti :
- [ ] Graydron (Murasama) : 3001 dégâts, 50 % de critique dans l'infobulle
- [ ] Ark of the Cosmos : 2000 dégâts
- [ ] Un objet modifié en vitesse (ex. Absolute Zero, `use` 19) frappe à la bonne cadence, sans bug d'animation
- [ ] Une arme à plusieurs tirs par usage (ex. Clockwork Bow) tire toujours correctement
- [ ] Un ennemi modifié (ex. Aero Slime, défense 10) : PV et défense cohérents avec le wiki (`/` Cheat Sheet → NPC)
- [ ] Rien de cassé côté prix de vente (vendre un objet modifié à un PNJ)
- [ ] **À décider** : les 38 écarts extrêmes de `reports/stats.md` (Desert Scourge 350 000 PV, Ravager 1 PV...)

### 1.3 Recettes du wiki (`WikiData/recipes.json`, bilan dans `reports/recipes.md`)

Chaque recette changée est vérifiée par l'auto-test ; en jeu, contrôler surtout le comportement :
- [ ] Aerospec Breastplate : 11 Aerialite Bar, 10 Cloud, 5 Rain Cloud, 2 Feather au **Sky Mill**
- [ ] Abyss Candle : Lumenyl au lieu de Torch
- [ ] Une recette avec groupe « A/B » du wiki (chercher « / » dans `reports/recipes.md`) accepte bien les deux objets
- [ ] Une recette d'origine absente du wiki est toujours là (ex. Acidwood depuis Acidwood Wall)
- [ ] Les recettes de Calamity à condition (ex. objets Draedon, Revengeance) gardent leurs conditions
- [ ] Shimmer : une transmutation du wiki fonctionne (liste dans `WikiData/recipes.json` → `shimmer`)
- [ ] Le guide (« Guide → objets craftables ») ne plante pas et affiche les nouvelles recettes
