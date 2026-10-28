"""Prépare le portage d'un contenu retiré de Calamity : extrait de la version 2.0.1.005 (git de CalamityModPublic)
le code d'une classe, ses textures, et récursivement les classes qu'elle utilise et qui n'existent plus aujourd'hui.

Usage :
  python tools/legacy_extract.py NightsGaze [AutreClasse ...]
  python tools/legacy_extract.py --list            liste des candidats au portage (reports/baseline.md, puis les objets
                                                   retirés avant 2.0.1.005 dont le Fandom a une page)

Un objet retiré avant 2.0.1.005 (Spore Knife, Aftershock...) est extrait de la dernière version mensuelle où il existait
(data/old_tooltips.json, tools/old_tooltips.py).

Sortie : legacy_src/<Classe>/ (ignoré par git) avec les .cs et .png d'origine et un PORTAGE.md qui liste, pour chaque
classe référencée, si elle existe encore dans Calamity actuel (à réutiliser telle quelle) ou s'il faut la porter.
Le code porté va dans Content/Legacy/ ; voir la section « Portage » de CLAUDE.md.
"""
import json
import os
import re
import subprocess
import sys

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from wiki_index import ROOT, CALAMITY, load_calamity_classes  # noqa: E402
from gen_baseline import BASELINE, classes_at, git  # noqa: E402

OUT = os.path.join(ROOT, "legacy_src")
REF_RE = re.compile(r"(?:ModContent\.(?:Projectile|Item|Buff|NPC|Dust|Tile)Type|AddIngredient|NewProjectile\w*)<(\w+)>")
TYPE_RE = re.compile(r"(?:Type|Ingredient)<(\w+)>")


def all_declarations(commit):
    """{classe: chemin} de toutes les classes à un commit (pas seulement ModItem/ModNPC)."""
    found = {}
    for line in git("grep", "-n", "-E", r"class \w+", commit, "--", "*.cs").splitlines():
        _, path, _, content = line.split(":", 3)
        m = re.search(r"\bclass\s+(\w+)", content)
        if m:
            found.setdefault(m.group(1), path)
    return found


def show(commit, path):
    return subprocess.run(["git", "-C", CALAMITY, "show", f"{commit}:{path}"], capture_output=True).stdout


def textures_for(commit, path, cls):
    folder = os.path.dirname(path)
    names = git("ls-tree", "--name-only", commit, folder + "/").splitlines()
    return [n for n in names if n.endswith(".png") and os.path.basename(n).startswith(cls)]


def last_commit_with(cls):
    """(commit, date) du dernier instantané mensuel où la classe existait (objets retirés avant 2.0.1.005), ou None."""
    path = os.path.join(ROOT, "data", "old_tooltips.json")
    if not os.path.exists(path):
        return None
    old = json.load(open(path, encoding="utf-8"))
    if cls not in old.get("seen", {}):
        return None
    snap = old["snapshots"][old["seen"][cls][1]]
    return snap["commit"], snap["date"]


def pre_baseline_candidates():
    """Pages « new » du Fandom qui portent le nom d'un objet retiré avant 2.0.1.005 : (classe, nom, date de retrait, fait)."""
    old = json.load(open(os.path.join(ROOT, "data", "old_tooltips.json"), encoding="utf-8"))
    index = json.load(open(os.path.join(ROOT, "data", "wiki_index.json"), encoding="utf-8"))
    baseline = len(old["snapshots"]) - 1
    by_name = {}
    for cls, history in old["items"].items():
        for _, name, _ in history:
            if name and old["seen"][cls][1] < baseline:
                by_name.setdefault(re.sub(r"[^a-z0-9]", "", name.lower()), cls)
    found = []
    for entry in index.values():
        if entry.get("kind") == "content" and entry.get("status") == "new":
            cls = by_name.get(re.sub(r"[^a-z0-9]", "", entry["title"].lower()))
            if cls:
                found.append((cls, entry["title"], old["snapshots"][old["seen"][cls][1]]["date"], bool(entry.get("implemented"))))
    return sorted(found, key=lambda f: f[1])


def extract(root_cls, old_decl, current, commit=BASELINE):
    target = os.path.join(OUT, root_cls)
    os.makedirs(target, exist_ok=True)
    todo, done, summary = [root_cls], set(), []
    while todo:
        cls = todo.pop()
        if cls in done or cls not in old_decl:
            continue
        done.add(cls)
        path = old_decl[cls]
        source = show(commit, path)
        dest = os.path.join(target, path)
        os.makedirs(os.path.dirname(dest), exist_ok=True)
        open(dest, "wb").write(source)
        for tex in textures_for(commit, path, cls):
            open(os.path.join(target, tex), "wb").write(show(commit, tex))
        text = source.decode("utf-8", errors="replace")
        for ref in sorted(set(TYPE_RE.findall(text))):
            if ref in current:
                summary.append((cls, ref, "existe encore dans Calamity : réutiliser"))
            elif ref in old_decl:
                summary.append((cls, ref, "retiré : à porter aussi (extrait)"))
                todo.append(ref)
            else:
                summary.append((cls, ref, "vanilla ou introuvable"))
    with open(os.path.join(target, "PORTAGE.md"), "w", encoding="utf-8") as f:
        f.write(f"# Portage de {root_cls} (Calamity {commit})\n\n")
        f.write("Classes extraites : " + ", ".join(sorted(done)) + "\n\n| Utilisée par | Référence | Statut |\n|---|---|---|\n")
        for cls, ref, status in summary:
            f.write(f"| {cls} | {ref} | {status} |\n")
    print(f"{root_cls} : {len(done)} classe(s) extraite(s) dans {os.path.relpath(target, ROOT)}")


def main():
    if "--list" in sys.argv or len(sys.argv) < 2:
        baseline = json.load(open(os.path.join(ROOT, "WikiData", "baseline.json"), encoding="utf-8"))
        for r in baseline["removed"]:
            if r["wikiPage"]:
                print(f"{r['class']:40} {r['name']}")
        print("\nRetirés avant 2.0.1.005 (dernière version mensuelle où ils existaient) :")
        for cls, name, date, done in pre_baseline_candidates():
            print(f"{cls:40} {name} ({date}){' [fait]' if done else ''}")
        return
    old_decl = all_declarations(BASELINE)
    current = set(load_calamity_classes())
    for cls in sys.argv[1:]:
        if cls in old_decl:
            extract(cls, old_decl, current)
            continue
        # Retiré avant 2.0.1.005 : dernière version où il existait
        last = last_commit_with(cls)
        if not last:
            print(f"{cls} : introuvable en 2.0.1.005 et dans l'historique (lancer tools/old_tooltips.py)")
            continue
        print(f"{cls} : retiré avant 2.0.1.005, extrait de la version {last[1]} ({last[0][:10]})")
        extract(cls, all_declarations(last[0]), current, last[0])


if __name__ == "__main__":
    main()
