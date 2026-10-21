# Notre wiki (`wiki/`)

Décision du 21/10 : on garde une **copie complète du wiki Fandom** dans `wiki/`, éditée au fil du projet. Elle décrit
le mod tel qu'il est (les deux modes) : le jour où le mod est publié, son wiki (wiki.gg) est déjà écrit.

## Les deux sources

| Dossier | Rôle | Modifiable |
|---|---|---|
| `fandom_dump_txt/` (+ `fandom_dump_extra/`) | Le wiki Fandom d'origine : référence du **mode Full Fandom** | **jamais** |
| `wiki/` | Notre wiki : décrit le mod (Full Fandom **et** Calamity++) ; c'est lui que lisent les outils | oui |

- `wiki/` a été commité **identique** au Fandom (commit `c27190c`, sauf les 18 pages que Windows avait écrasées) :
  `git diff c27190c -- wiki/` montre donc tout ce qu'on a changé. Git ne stocke pas deux fois un fichier identique.
- `wiki/Main/` : pages (mêmes noms de fichiers que le dump) ; `wiki/Template/`, `Guide/`, `Category/`, `Project/`,
  `Module/`, `MediaWiki/`, `Help/` : les autres espaces. `wiki/index.json` : fichier → titre exact.
- Les pages de discussion et d'utilisateurs ne sont pas copiées (restent dans `fandom_dump_extra/`).

## Quand les deux modes diffèrent : `{{mode}}`

```
| damage = {{mode|68|3170}}
```

Première valeur : Full Fandom ; seconde : Calamity++ (page du modèle : `wiki/Template/Template_Mode.txt`).
Les outils (`tools/gen_stats.py` pour l'instant) lisent ce modèle directement : c'est une **décision explicite**, qui
passe avant les règles automatiques (« valeur 2022 → Full seulement », écarts extrêmes...). Un champ sans `{{mode}}`
vaut pour les deux modes.

## Pages générées

- `tools/gen_element_pages.py` : section « Status effects » d'« Elemental System » (entre marqueurs) et pages des
  debuffs d'éléments, depuis `Common/Elements/` et `Balance/elements.json`. Ne pas les éditer à la main.
- `tools/apply_modes.py` : écrit en `{{mode}}` les stats que les règles réservent au Full Fandom. À relancer à la main
  si de nouvelles pages ou règles en produisent (`reports/stats.md` → « valeurs 2022 »), puis vérifier le diff.

## Éditer le wiki

**Règle** : les outils ne lisent que `wiki/`, pour les deux modes. Une correction qui ne doit **pas** toucher le
Full Fandom (« le wiki à la lettre ») s'écrit `{{mode|valeur d'origine|nouvelle valeur}}` ; une modification sans
`{{mode}}` vaut pour les deux modes (réservé à ce qui est propre au mod : pages nouvelles, descriptions de mécaniques).

1. Modifier ou créer la page dans `wiki/` (jamais dans `fandom_dump_txt/`). Nouvelle page : l'ajouter à `wiki/index.json`.
2. Le **pourquoi** reste dans `docs/decisions.md` ; la page dit le **quoi** (valeurs, comportement, noms).
3. `python tools/build_data.py`, puis `dotnet build` et `python tools/selftest.py --mode all`.
4. `reports/README.md` compte les pages modifiées ou créées par rapport au Fandom.

## Étapes

- [x] 1. Copie exacte du Fandom dans `wiki/`, commitée seule (`c27190c`)
- [x] 2. Les outils lisent `wiki/` ; `{{mode}}` pris en charge pour les stats ; `wiki_edits/` supprimé
- [x] 3. Décisions reportées sur les pages :
  - 830 stats « Full Fandom seulement » en `{{mode}}` (`tools/apply_modes.py`, données identiques avant/après) ; 30
    restent gérées par la règle (valeur actuelle calculée dans le code, ou page partagée avec une classe décidée
    autrement, comme Polterghast) ;
  - « Elemental System » (section « Status effects ») et une page par debuff d'élément, **générées depuis le code**
    (`tools/gen_element_pages.py`, lancé par `build_data.py`) ;
  - Meatmetal Sans (page Ravager) et Zweihandromeda : comportement tel qu'implémenté ;
  - nouvelle page « Fandom Accuracy » (les deux modes) et `Template:Mode`.
  - Pas sur le wiki : les outils de debug.
- [ ] 4. `{{mode}}` lu aussi par les recettes, le butin et les éléments
- [ ] 5. Le jour venu : export vers wiki.gg (les pages sont déjà en wikitexte MediaWiki)
