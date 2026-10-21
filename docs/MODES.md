# Niveaux de fidélité au Fandom

Un seul mod, trois modes, choisis dans la config serveur **Calamity Fandom → Fandom accuracy** (rechargement du mod
obligatoire : noms, recettes et existence des objets se fixent au chargement). Chaque monde mémorise le mode avec
lequel il a été créé et prévient si on le charge dans un autre (`Common/Config/WorldModeLock.cs`).
Mode par défaut : **Minimal**.

Dans le code : `FandomMode.Current`, `FandomMode.IsFull`, `FandomMode.IsPlus` (`Common/Config/FandomConfig.cs`).
La variable d'environnement `CALFANDOM_MODE` force un mode (auto-test : `python tools/selftest.py --mode all`).

## Calamity est terminé

D'après l'utilisateur (21/10/2026), le développement de Calamity s'est arrêté et l'équipe s'est séparée : **2.2.2 est
la version finale**. « Calamity actuel » (modes Minimal et Calamity++) désigne donc 2.2.2, une cible figée : plus de
mise à jour à suivre (`tools/extract_calamity_dll.py` ne devrait plus servir). Les droits restent à Azafure, LLC, ce qui
ne change rien à la question des assets pour une publication, mais trouver un interlocuteur sera peut-être plus difficile.

## Les trois modes

| | Full Fandom | Minimal | Calamity++ |
|---|---|---|---|
| Idée | Uniquement ce qui est écrit sur le wiki | Calamity actuel + ajouts du wiki + correctifs | Minimal + vrai équilibrage, jouable |
| Référence Calamity | 2.0.1.005 (abandon du wiki) | version actuelle | version actuelle |
| Contenu Calamity ajouté depuis 2.0.1.005 sans trace sur le wiki | masqué (recettes, butin, apparitions, ventes) | présent | présent |
| Contenu retiré de Calamity depuis 2.0.1.005, documenté sur le wiki | porté depuis le vieux code | porté | porté |
| Stats du wiki | toutes, y compris les valeurs de 2022 | propres au wiki (extrêmes : au cas par cas) | Minimal + `Balance/stats.json` |
| Recettes du jeu absentes du wiki | désactivées | conservées | conservées |
| Butin du jeu absent de la page du PNJ | retiré | conservé | conservé |
| Pages incohérentes (`wiki_edits/`) | non (le wiki tel quel) | oui | oui |
| Noms, vocabulaire, éléments, matériaux, butin ajouté | oui | oui | oui (équilibrage possible) |

« Masqué » ne détruit rien : un objet masqué présent dans un inventaire reste là, il n'est simplement plus obtenable.
Les PNJ créés en combat ne sont jamais supprimés (un boss retravaillé peut dépendre d'un sbire récent).

## État

- [x] Config + verrou par monde, auto-test des trois modes
- [x] Full Fandom : masquage (`WikiData/baseline.json`, `tools/gen_baseline.py`), stats extrêmes, recettes et butin hors wiki retirés
- [x] Calamity++ : couche `Balance/stats.json` (vide : l'équilibrage reste à faire)
- [ ] `wiki_edits/` ignoré en Full Fandom (aucune page corrigée pour l'instant, donc sans effet)
- [ ] Portage du contenu retiré : 265 objets/PNJ avec page wiki (`reports/baseline.md`) — outil `tools/legacy_extract.py`
      prêt, 1 fait (Night's Gaze), méthode dans CLAUDE.md
- [ ] Calamity++ : recettes, butin, éléments équilibrés (mêmes principes de couches)

## Portage du vieux code de Calamity

Licence de Calamity (`CalamityModPublic/LICENSE.md`) : reprendre du **code** est permis à condition de créditer
« Azafure, LLC » avec un lien vers le dépôt. Les **assets** (sprites, sons) restent la propriété d'Azafure et Calamity
ne peut pas être redistribué : pour une publication, il faudra soit l'accord d'Azafure, soit des sprites refaits.
En local, aucun problème.

## Et le futur wiki.gg

Toutes les données appliquées en jeu sont générées (`WikiData/`, `Balance/`) : on pourra générer les pages du wiki
depuis ces fichiers, avec un modèle `{{mode|fandom|minimal|plus}}` sur le principe de `{{dv}}` pour les difficultés.
Règle à tenir dès maintenant : **toute différence entre modes passe par des données** (pas de valeur codée en dur
selon le mode), pour qu'elle puisse apparaître sur le wiki.
