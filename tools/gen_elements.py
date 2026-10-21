"""Extrait le système d'éléments du wiki (page « Elemental System ») vers WikiData/elements.json.

- Armes : champs element1..n des infobox d'objets.
- PNJ : champs resistant1..n / vulnerable1..n des infobox de PNJ.
Les données sont indexées par titre de page ; le jeu les résout par nom anglais (comme les recettes), ce qui couvre
Calamity, vanilla et nos propres objets. Les valeurs hors des 17 éléments (« My Heart is Cold »...) sont ignorées.

Usage : python tools/gen_elements.py   (après wiki_index.py)
"""
import collections
import json
import os
import re
import sys

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from wiki_index import ROOT  # noqa: E402

OUT = os.path.join(ROOT, "WikiData", "elements.json")
REPORT = os.path.join(ROOT, "reports", "elements.md")
ELEMENTS = ["Cold", "Fire", "Wind", "Water", "Poison", "Acid", "Radioactive", "Electricity", "Stab", "Slash",
            "Smash", "Bomb", "Magic", "Dark", "Holy", "Unholy", "Machine"]
BY_LOWER = {e.lower(): e for e in ELEMENTS}


def values(fields, prefix):
    found, rejected = [], []
    for key in sorted(fields, key=lambda k: int(re.sub(r"\D", "", k) or 0)):
        if not re.fullmatch(rf"{prefix}\d*", key):
            continue
        raw = re.sub(r"<!--.*?-->", "", fields[key], flags=re.S).strip()
        if not raw:
            continue
        element = BY_LOWER.get(raw.lower())
        if element is None:
            rejected.append(raw)
        elif element not in found:
            found.append(element)
    return found, rejected


def main():
    index = json.load(open(os.path.join(ROOT, "data", "wiki_index.json"), encoding="utf-8"))
    items, npcs = {}, {}
    infobox_names = []  # (nom de l'infobox, titre) : alias quand le titre diffère
    rejected = collections.Counter()
    for page in index.values():
        if page.get("kind") != "content" or not page.get("fields"):
            continue
        box = page.get("infobox", "")
        name = page["fields"].get("name") or page["title"]
        name = re.sub(r"\s+", " ", re.sub(r"<[^>]*>|\[\[|\]\]", "", name)).strip()
        if box.startswith("item"):
            elements, bad = values(page["fields"], "element")
            rejected.update(bad)
            if elements:
                items[page["title"]] = elements
        elif box.startswith("npc"):
            resist, bad1 = values(page["fields"], "resistant")
            vuln, bad2 = values(page["fields"], "vulnerable")
            rejected.update(bad1 + bad2)
            both = set(resist) & set(vuln)  # contradiction : on ignore l'élément
            resist = [e for e in resist if e not in both]
            vuln = [e for e in vuln if e not in both]
            if resist or vuln:
                entry = {"resistant": resist, "vulnerable": vuln}
                npcs[page["title"]] = entry
                if name != page["title"]:
                    infobox_names.append((name, page["title"]))

    # Alias : le jeu nomme parfois autrement (« Signus » pour « Signus, Envoy of the Devourer ») ; on ajoute les
    # redirections du wiki et les alias manuels de tools/glossary.json, sans écraser une vraie page.
    aliases = [(p["title"], p["target"].split("#")[0].strip()) for p in index.values() if p["kind"] == "redirect"]
    glossary = json.load(open(os.path.join(ROOT, "tools", "glossary.json"), encoding="utf-8"))
    aliases += list(glossary.get("aliases", {}).items()) + infobox_names
    alias_map = {}
    for source, target in aliases:
        if (target in items or target in npcs) and source not in items and source not in npcs:
            alias_map.setdefault(target, []).append(source)

    os.makedirs(os.path.dirname(OUT), exist_ok=True)
    with open(OUT, "w", encoding="utf-8") as f:
        json.dump({"_generated_by": "tools/gen_elements.py", "elements": ELEMENTS, "items": items, "npcs": npcs,
                   "aliases": alias_map}, f, ensure_ascii=False, indent=1)

    usage = collections.Counter(e for els in items.values() for e in els)
    with open(REPORT, "w", encoding="utf-8") as f:
        f.write("# Éléments du wiki\n\n_Généré par `tools/gen_elements.py`._\n\n")
        f.write(f"- {len(items)} objets avec éléments, {len(npcs)} PNJ avec résistances / vulnérabilités\n")
        f.write(f"- Valeurs ignorées (hors des 17 éléments) : {dict(rejected) or 'aucune'}\n\n")
        f.write("| Élément | Armes | PNJ résistants | PNJ vulnérables |\n|---|---|---|---|\n")
        for e in ELEMENTS:
            r = sum(e in v["resistant"] for v in npcs.values())
            v_ = sum(e in v["vulnerable"] for v in npcs.values())
            f.write(f"| {e} | {usage[e]} | {r} | {v_} |\n")
    print(f"{len(items)} objets, {len(npcs)} PNJ ; ignorés : {dict(rejected)}")


if __name__ == "__main__":
    main()
