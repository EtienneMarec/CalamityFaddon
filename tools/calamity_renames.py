"""Objets et PNJ de Calamity renommés depuis 2.0.1.005 (historique git de CalamityModPublic), dont le wiki a la page
sous l'ancien nom (décision du 23/10, docs/decisions.md) : Essence of Chaos -> Essence of Havoc, Crimulan Slime God ->
Crimulan Paladin, Night's Gaze -> Vega...

- wiki_index.py relie la page du wiki à la classe actuelle (statut « renamed ») ;
- gen_localization.py rend son nom du wiki à l'objet (le nom seul, pas une substitution dans tous les textes : beaucoup
  de nouveaux noms sont des mots courants, « Radiance », « Lightspeed ») ;
- gen_baseline.py ne le masque plus en Full Fandom (sauf s'il a été porté : la version de 2022 le remplace en Full
  Fandom, et le portage est masqué en Calamity++).

Résultat mis en cache dans data/calamity_renames.json (recalculé si le dépôt de Calamity a changé).

Usage : python tools/calamity_renames.py   (affiche la liste)
"""
import json
import os
import re
import subprocess
import sys

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from wiki_index import CALAMITY, ROOT, calamity_display_names, norm  # noqa: E402

CACHE = os.path.join(ROOT, "data", "calamity_renames.json")
BASELINE = "3ec8f553ed"


def git(*args):
    return subprocess.run(["git", "-C", CALAMITY, *args], capture_output=True, text=True, encoding="utf-8",
                          errors="replace", check=True).stdout


def compute():
    from gen_baseline import classes_at, legacy_display_names  # import tardif : gen_baseline importe wiki_index
    log = git("log", f"{BASELINE}..HEAD", "--reverse", "--diff-filter=R", "-M", "--name-status", "--format=", "--", "*.cs")
    final, score = {}, {}
    for line in log.splitlines():
        parts = line.split("\t")
        if len(parts) != 3 or not parts[0].startswith("R"):
            continue
        old, new = (os.path.splitext(os.path.basename(p))[0] for p in parts[1:])
        if old == new:
            continue
        s = int(parts[0][1:])
        # Chaînes A -> B -> C dans l'ordre des commits
        for k, v in list(final.items()):
            if v == old:
                final[k], score[k] = new, min(score[k], s)
        if old not in final:
            final[old], score[old] = new, s
    old_classes, new_classes = classes_at(BASELINE), classes_at("HEAD")
    old_names = legacy_display_names(BASELINE)
    current = {}
    for display, _, cls in calamity_display_names():
        current.setdefault(cls, display)
    renames = []
    for old, new in sorted(final.items()):
        if old not in old_classes or new not in new_classes or old in new_classes:
            continue
        kind, path = old_classes[old]
        if new_classes[new][0] != kind:
            continue
        renames.append({"old": old, "new": new, "kind": kind, "oldName": old_names.get(path) or old,
                        "newName": current.get(new, new), "similarity": score[old]})
    # Même classe, nom affiché changé (SilvaBasin : « Effulgent Manipulator » en 2022, « Silva Basin » aujourd'hui).
    # Seulement quand l'ancien nom est connu (DisplayName.SetDefault du vieux code).
    # Le vieux nom est lu par fichier : un fichier qui déclarait plusieurs objets (casque + plastron) ne compte pas.
    per_path = {}
    for kind, path in old_classes.values():
        per_path[path] = per_path.get(path, 0) + 1
    for cls in sorted(set(old_classes) & set(new_classes)):
        kind, path = old_classes[cls]
        before, now = old_names.get(path), current.get(cls)
        if not before or not now or now.startswith("{$") or per_path[path] != 1:
            continue
        if norm(before) != norm(now) and new_classes[cls][0] == kind:
            renames.append({"old": cls, "new": cls, "kind": kind, "oldName": before, "newName": now, "similarity": 100})
    return renames


def load():
    head = git("rev-parse", "HEAD").strip()
    if os.path.exists(CACHE):
        cached = json.load(open(CACHE, encoding="utf-8"))
        if cached.get("head") == head:
            return cached["renames"]
    renames = compute()
    os.makedirs(os.path.dirname(CACHE), exist_ok=True)
    with open(CACHE, "w", encoding="utf-8") as f:
        json.dump({"_generated_by": "tools/calamity_renames.py", "head": head, "renames": renames}, f, ensure_ascii=False, indent=1)
    return renames


def wiki_known(renames, pages):
    """Renommages dont l'ancien nom est une page du wiki du bon type (infobox d'objet pour un objet, de PNJ pour un PNJ),
    directement ou par redirection. pages : {titre normalisé: page de data/wiki_index.json} (contenu et redirections)."""
    known = []
    for r in renames:
        page = pages.get(norm(r["oldName"]))
        hops = 0
        while page and page.get("kind") == "redirect" and hops < 3:
            page = pages.get(norm(page.get("target", "").split("#")[0]))
            hops += 1
        if not page or page.get("kind") != "content":
            continue
        box = page.get("infobox", "") or ""
        if (r["kind"] == "ModItem") != box.startswith("item"):
            continue
        known.append({**r, "page": page["title"]})
    return known


def main():
    renames = load()
    index = json.load(open(os.path.join(ROOT, "data", "wiki_index.json"), encoding="utf-8"))
    pages = {norm(p["title"]): p for p in index.values()}
    known = wiki_known(renames, pages)
    print(f"{len(renames)} renommages depuis 2.0.1.005, dont {len(known)} connus du wiki sous l'ancien nom")
    for r in known:
        print(f"  {r['oldName']!r:42} -> {r['newName']!r} ({r['new']}, {r['similarity']} %)")


if __name__ == "__main__":
    main()
