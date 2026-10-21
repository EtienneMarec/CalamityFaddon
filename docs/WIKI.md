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
- [ ] 3. Reporter les décisions sur les pages : chiffres et debuffs d'éléments (page « Elemental System » + une page par
      debuff : Chilled, Ablaze...), Meatmetal Sans tel qu'implémenté, valeurs 2022 en `{{mode}}` (génération
      automatique pour les ~860 stats), Zweihandromeda, matériaux sans comportement, nouveaux objets de test exclus
- [ ] 4. `{{mode}}` lu aussi par les recettes, le butin et les éléments
- [ ] 5. Le jour venu : export vers wiki.gg (les pages sont déjà en wikitexte MediaWiki)
