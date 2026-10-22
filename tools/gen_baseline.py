"""Compare Calamity à l'abandon du wiki Fandom (2.0.1.005) avec la version actuelle, via le git de CalamityModPublic.

Pourquoi 2.0.1.005 : des 29 objets ajoutés juste avant, 19 ont une page wiki ; des 77 ajoutés par 2.0.2.001
(février 2023), un seul. Le wiki a cessé de suivre Calamity entre les deux.

Sortie :
  WikiData/baseline.json   hidden : objets/PNJ masqués en mode Full Fandom :
                             - ajoutés depuis et sans page wiki ;
                             - plus anciens, sans page et cités nulle part dans le texte du wiki (décision du 21/10)
                           unmentioned : la seconde catégorie seule (pour le rapport)
                           removed : objets/PNJ retirés depuis (à porter depuis le vieux code), avec leur page wiki
  reports/baseline.md      les mêmes listes, lisibles

Usage : python tools/gen_baseline.py   (après wiki_index.py)
"""
import collections
import json
import os
import re
import subprocess
import sys

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from wiki_index import (ROOT, CALAMITY, CLASS_DECL, OUR_WIKI, calamity_display_names, load_calamity_names,  # noqa: E402
                        norm, resolve_mod_types)  # noqa: E402

BASELINE = "3ec8f553ed"  # « Version bump 2.0.1.005 », 30/12/2022
BASELINE_NAME = "2.0.1.005"
OUT = os.path.join(ROOT, "WikiData", "baseline.json")
REPORT = os.path.join(ROOT, "reports", "baseline.md")


def git(*args):
    return subprocess.run(["git", "-C", CALAMITY, *args], capture_output=True, text=True,
                          encoding="utf-8", errors="replace", check=True).stdout


def classes_at(commit):
    """{classe: (type, chemin)} des ModItem/ModNPC à un commit, héritage indirect compris (RogueWeapon...)."""
    declarations = {}
    for line in git("grep", "-n", "-E", r"class \w+.*:", commit, "--", "*.cs").splitlines():
        # format : commit:chemin:ligne:contenu
        _, path, _, content = line.split(":", 3)
        m = CLASS_DECL.search(content)
        if m:
            declarations.setdefault(m.group(1), (m.group(2), path))
    return {cls: (kind, path) for cls, (path, kind) in resolve_mod_types(declarations).items()
            if kind in ("ModItem", "ModNPC")}


def sources_at(commit, paths):
    """{chemin: texte} des fichiers à un commit, lus en un seul appel git (git cat-file --batch)."""
    paths = sorted(set(p for p in paths if p))
    request = "".join(f"{commit}:{p}\n" for p in paths).encode("utf-8")
    out = subprocess.run(["git", "-C", CALAMITY, "cat-file", "--batch"], input=request, capture_output=True, check=True).stdout
    texts, pos = {}, 0
    for p in paths:
        header_end = out.index(b"\n", pos)
        header = out[pos:header_end].split()
        pos = header_end + 1
        if len(header) < 3 or header[1] == b"missing":
            continue
        size = int(header[2])
        texts[p] = out[pos:pos + size].decode("utf-8", errors="replace")
        pos += size + 1
    return texts


def legacy_display_names(commit):
    """Noms affichés à l'ancienne version (DisplayName.SetDefault dans le code, avant la localisation hjson)."""
    names = {}
    for line in git("grep", "-n", "-E", r'DisplayName\.SetDefault\("', commit, "--", "*.cs").splitlines():
        _, path, _, content = line.split(":", 3)
        m = re.search(r'DisplayName\.SetDefault\("([^"]+)"\)', content)
        if m:
            names.setdefault(path, m.group(1))
    return names


def ported_classes():
    """Classes de notre mod portées depuis 2.0.1.005 (Content/Legacy)."""
    classes = set()
    for folder, _, files in os.walk(os.path.join(ROOT, "Content", "Legacy")):
        for f in files:
            if f.endswith(".cs"):
                classes |= set(re.findall(r"\bclass (\w+)\b", open(os.path.join(folder, f), encoding="utf-8-sig").read()))
    return classes


def words(text):
    """Texte en minuscules, mots séparés par une seule espace (pour chercher un nom sans tenir compte de la ponctuation)."""
    return " " + re.sub(r"[^a-z0-9]+", " ", text.lower()).strip() + " "


def wiki_corpus():
    """Tout le texte de notre wiki (tous les espaces de noms), mis en mots : les références internes ne sont pas
    toujours des liens [[...]], on cherche donc les noms dans le texte brut."""
    parts = []
    for folder, _, files in os.walk(OUR_WIKI):
        for f in files:
            if f.endswith(".txt"):
                parts.append(words(open(os.path.join(folder, f), encoding="utf-8", errors="replace").read()))
    return "".join(parts)


def main():
    old = classes_at(BASELINE)
    new = classes_at("HEAD")
    old_names = legacy_display_names(BASELINE)

    index = json.load(open(os.path.join(ROOT, "data", "wiki_index.json"), encoding="utf-8"))
    titles = {norm(p["title"]) for p in index.values() if p["kind"] == "content"}
    redirects = {norm(p["title"]) for p in index.values() if p["kind"] == "redirect"}
    documented_classes = {m["class"] for p in index.values() for m in p.get("calamity", [])}

    # Nom affiché actuel de chaque classe (localisation hjson)
    current_names = {}
    for display, matches in load_calamity_names().items():
        for _, cls in matches:
            current_names.setdefault(cls, display)

    def old_display(cls):
        kind, path = old[cls]
        return old_names.get(path) or re.sub(r"(?<!^)(?=[A-Z])", " ", cls)

    current_by_display = collections.defaultdict(list)
    for cls in new:
        if cls in current_names:
            current_by_display[current_names[cls]].append(cls)

    # Le wiki connaît aussi un objet par une redirection vers une page de groupe (« Aerospec Hat » -> « Aerospec
    # armor ») ou par une recette qui le produit
    recipes_path = os.path.join(ROOT, "WikiData", "recipes.json")
    recipe_results = set()
    if os.path.exists(recipes_path):
        recipe_results = {norm(r["result"]) for r in json.load(open(recipes_path, encoding="utf-8"))["recipes"]}
    known = titles | redirects | recipe_results

    # Classes renommées depuis (historique git, décision du 23/10) : même objet, que le wiki connaît sous l'ancien nom.
    # S'il a été porté (Content/Legacy, classe au nom de la page), le portage le remplace en Full Fandom, et c'est le
    # portage qui est masqué en Calamity++.
    from calamity_renames import load as load_renames, wiki_known
    all_renames = load_renames()
    renamed_old = {r["old"] for r in all_renames}
    wiki_renames = {r["new"]: r for r in wiki_known(all_renames, {norm(p["title"]): p for p in index.values()})}
    ported = ported_classes()
    plus_hidden = {"items": [], "npcs": []}

    # Ajoutés depuis : masqués en Full Fandom sauf si le wiki les connaît
    hidden = {"items": [], "npcs": []}
    for cls in sorted(set(new) - set(old)):
        kind, _ = new[cls]
        key = "items" if kind == "ModItem" else "npcs"
        rename = wiki_renames.get(cls)
        port = re.sub(r"[^A-Za-z0-9]", "", rename["oldName"]) if rename else None
        if port in ported:
            hidden[key].append(cls)
            plus_hidden[key].append(port)
            continue
        has_page = cls in documented_classes or norm(current_names.get(cls, "")) in known or rename is not None
        if not has_page:
            hidden[key].append(cls)

    # Plus anciens (déjà là à l'abandon du wiki), sans page et cités nulle part : masqués aussi (décision du 21/10).
    # Cités en texte seulement (changelog, liste...) : gardés, le wiki les connaît.
    corpus = wiki_corpus()
    # Noms sous lesquels le wiki peut connaître la classe : nom actuel, nom de 2.0.1.005, renommage du glossaire,
    # alias du glossaire (nom du wiki -> nom en jeu)
    glossary = json.load(open(os.path.join(ROOT, "tools", "glossary.json"), encoding="utf-8"))
    wiki_aliases = collections.defaultdict(set)
    for wiki_name, game_name in glossary.get("aliases", {}).items():
        wiki_aliases[game_name].add(wiki_name)
    raw_names = collections.defaultdict(set)
    for display, _, cls in calamity_display_names():
        raw_names[cls] |= {display, glossary["renames"].get(display, display)} | wiki_aliases[display]
    unmentioned = {"items": [], "npcs": []}
    for cls in sorted(set(new) & set(old)):
        kind, path = new[cls]
        # Classes de base abstraites (BaseDye, HeldOnlyItem) : pas des objets
        if re.search(rf"\babstract\s+class\s+{cls}\b", open(os.path.join(CALAMITY, path), encoding="utf-8-sig").read()):
            continue
        names = raw_names.get(cls, set()) | {old_display(cls)}
        if cls in documented_classes or any(norm(n) in known for n in names):
            continue
        if not any(words(n).strip() and words(n) in corpus for n in names):
            unmentioned["items" if kind == "ModItem" else "npcs"].append(cls)
    for key in hidden:
        hidden[key] = sorted(set(hidden[key]) | set(unmentioned[key]))

    # Retirés depuis : sauf simple renommage de classe (même nom affiché qu'un objet actuel)
    removed, renamed = [], []
    for cls in sorted(set(old) - set(new)):
        kind, path = old[cls]
        display = old_display(cls)
        if norm(display) in current_by_display or cls.endswith("Banner") or cls in renamed_old:
            renamed.append((cls, display))
            continue
        page = norm(display) in titles or norm(display) in redirects
        removed.append({"class": cls, "kind": kind, "name": display, "path": path, "wikiPage": page})

    with open(OUT, "w", encoding="utf-8") as f:
        json.dump({"_generated_by": "tools/gen_baseline.py", "baseline": BASELINE_NAME, "commit": BASELINE,
                   "hidden": hidden, "unmentioned": unmentioned, "plusHidden": plus_hidden, "removed": removed},
                  f, ensure_ascii=False, indent=1)

    with_page = [r for r in removed if r["wikiPage"]]
    with open(REPORT, "w", encoding="utf-8") as f:
        f.write(f"# Calamity {BASELINE_NAME} (abandon du wiki) → version actuelle\n\n_Généré par `tools/gen_baseline.py`._\n\n")
        f.write(f"- **Masqués en Full Fandom** : {len(hidden['items'])} objets, {len(hidden['npcs'])} PNJ, dont "
                f"{len(unmentioned['items'])} objets et {len(unmentioned['npcs'])} PNJ antérieurs à {BASELINE_NAME} "
                "cités nulle part dans le wiki\n")
        f.write(f"- **Retirés depuis** : {len(removed)} ({len(with_page)} avec une page wiki → à porter en priorité)\n")
        f.write(f"- **Renommés depuis** (même objet, nom du wiki rendu en jeu) : {len(wiki_renames)} connus du wiki ; "
                f"portages masqués en Calamity++ (l'objet renommé de Calamity les remplace) : "
                f"{', '.join(plus_hidden['items'] + plus_hidden['npcs']) or 'aucun'}\n")
        f.write(f"- Simples renommages de classe ou bannières ignorés : {len(renamed)}\n\n")
        f.write("## Retirés depuis, avec page wiki (à porter)\n\n| Classe | Nom | Ancien fichier |\n|---|---|---|\n")
        for r in with_page:
            f.write(f"| {r['class']} | {r['name']} | `{r['path']}` |\n")
        f.write("\n## Retirés depuis, sans page wiki\n\n" + ", ".join(r["name"] for r in removed if not r["wikiPage"]) + "\n")
        f.write("\n## Antérieurs, cités nulle part dans le wiki (masqués en Full Fandom)\n\n### Objets\n\n"
                + ", ".join(unmentioned["items"]) + "\n\n### PNJ\n\n" + ", ".join(unmentioned["npcs"]) + "\n")
        f.write("\n## Tous les masqués en Full Fandom\n\n### Objets\n\n" + ", ".join(hidden["items"]) + "\n")
        f.write("\n### PNJ\n\n" + ", ".join(hidden["npcs"]) + "\n")
    print(f"masqués : {len(hidden['items'])} objets, {len(hidden['npcs'])} PNJ (dont cités nulle part : "
          f"{len(unmentioned['items'])} objets, {len(unmentioned['npcs'])} PNJ) ; retirés : {len(removed)} "
          f"({len(with_page)} avec page) ; renommages ignorés : {len(renamed)}")


if __name__ == "__main__":
    main()
