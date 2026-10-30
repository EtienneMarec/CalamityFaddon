"""Extrait le système d'éléments du wiki (page « Elemental System ») vers WikiData/elements.json.

- Armes : champs element1..n des infobox d'objets.
- PNJ : champs resistant1..n / vulnerable1..n des infobox de PNJ.
- Armures : champs resistance1..n / vulnerable1..n des pages d'ensembles (joueur qui porte l'ensemble complet).
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
from wiki_index import ROOT, has_mode, pick_mode  # noqa: E402

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
    items, npcs, full_items, full_npcs, armors = {}, {}, {}, {}, {}
    infobox_names = []  # (nom de l'infobox, titre) : alias quand le titre diffère
    rejected = collections.Counter()
    for page in index.values():
        if page.get("kind") != "content" or not page.get("fields"):
            continue
        box = page.get("infobox", "")
        name = page["fields"].get("name") or page["title"]
        name = re.sub(r"\s+", " ", re.sub(r"<[^>]*>|\[\[|\]\]", "", name)).strip()
        # {{mode|Full Fandom|Calamity++}} dans un champ : la vue Calamity++ va dans items/npcs, la vue Full Fandom dans
        # fullItems/fullNpcs quand elle diffère
        views = {mode: {k: pick_mode(v, mode) for k, v in page["fields"].items()} for mode in ("plus", "fandom")}
        moded = any(has_mode(v) for v in page["fields"].values())
        if box.startswith("item") and any(re.fullmatch(r"resistance\d*", k) for k in page["fields"]):
            # Armure (page d'ensemble) : résistances / vulnérabilités du joueur qui porte l'ensemble complet ; pièces
            # reconnues au préfixe de leur nom de classe (« Aerospec armor » -> AerospecHelm, AerospecBreastplate...)
            fields = views["plus"]
            resist, bad1 = values(fields, "resistance")
            vuln, bad2 = values(fields, "vulnerable")
            rejected.update(bad1 + bad2)
            both = set(resist) & set(vuln)
            prefix = re.sub(r"[^A-Za-z0-9]", "", re.sub(r"\s+(armou?r|coverings?)$", "", page["title"], flags=re.I))
            armors[page["title"]] = {"prefix": prefix, "resistant": [e for e in resist if e not in both],
                                     "vulnerable": [e for e in vuln if e not in both]}
        if box.startswith("item"):
            per_mode = {}
            for mode, fields in views.items():
                elements, bad = values(fields, "element")
                if mode == "plus":
                    rejected.update(bad)
                per_mode[mode] = elements
            if per_mode["plus"]:
                items[page["title"]] = per_mode["plus"]
            if moded and per_mode["fandom"] != per_mode["plus"]:
                full_items[page["title"]] = per_mode["fandom"]
        elif box.startswith("npc"):
            per_mode = {}
            for mode, fields in views.items():
                resist, bad1 = values(fields, "resistant")
                vuln, bad2 = values(fields, "vulnerable")
                if mode == "plus":
                    rejected.update(bad1 + bad2)
                both = set(resist) & set(vuln)  # contradiction : on ignore l'élément
                per_mode[mode] = {"resistant": [e for e in resist if e not in both],
                                  "vulnerable": [e for e in vuln if e not in both]}
            entry = per_mode["plus"]
            if entry["resistant"] or entry["vulnerable"]:
                npcs[page["title"]] = entry
            if moded and per_mode["fandom"] != entry:
                full_npcs[page["title"]] = per_mode["fandom"]
            if (page["title"] in npcs or page["title"] in full_npcs) and name != page["title"]:
                infobox_names.append((name, page["title"]))

    # Alias : le jeu nomme parfois autrement (« Signus » pour « Signus, Envoy of the Devourer ») ; on ajoute les
    # redirections du wiki et les alias manuels de tools/glossary.json, sans écraser une vraie page.
    aliases = [(p["title"], p["target"].split("#")[0].strip()) for p in index.values() if p["kind"] == "redirect"]
    glossary = json.load(open(os.path.join(ROOT, "tools", "glossary.json"), encoding="utf-8"))
    aliases += list(glossary.get("aliases", {}).items()) + infobox_names
    alias_map = {}
    for source, target in aliases:
        known = (target in items or target in npcs or target in full_items or target in full_npcs)
        if known and source not in items and source not in npcs:
            alias_map.setdefault(target, []).append(source)

    os.makedirs(os.path.dirname(OUT), exist_ok=True)
    with open(OUT, "w", encoding="utf-8") as f:
        out = {"_generated_by": "tools/gen_elements.py", "elements": ELEMENTS, "items": items, "npcs": npcs,
               "armors": armors, "aliases": alias_map}
        # Sections Full Fandom seulement si une page en a besoin (fichier inchangé sinon)
        if full_items:
            out["fullItems"] = full_items
        if full_npcs:
            out["fullNpcs"] = full_npcs
        json.dump(out, f, ensure_ascii=False, indent=1)

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
    print(f"{len(items)} objets, {len(npcs)} PNJ, {len(armors)} armures ; ignorés : {dict(rejected)}")


if __name__ == "__main__":
    main()
