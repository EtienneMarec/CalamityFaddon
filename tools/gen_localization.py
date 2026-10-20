"""Génère les surcharges de textes de Calamity d'après le vocabulaire du wiki (tools/glossary.json).

tModLoader charge les .hjson de chaque mod dans l'ordre de chargement : un fichier dont le préfixe est
Mods.CalamityMod, dans notre mod (chargé après Calamity), remplace les textes de Calamity.

Sortie :
  Localization/en-US/Mods.CalamityMod.hjson   textes modifiés uniquement (généré, ne pas éditer)
  reports/localization.md                     noms changés + nombre de textes touchés

Usage : python tools/gen_localization.py   (nécessite data/wiki_index.json : lancer wiki_index.py avant)
"""
import glob
import json
import os
import re

import hjson

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
CAL_LOC = os.path.join(ROOT, "CalamityModPublic", "Localization", "en-US")
OUT = os.path.join(ROOT, "Localization", "en-US", "Mods.CalamityMod.hjson")
REPORT = os.path.join(ROOT, "reports", "localization.md")

# Balises de chat à ne jamais modifier : [i:...], [cbuff:...], {0}, {$Mods...}. Pour [c/RRGGBB:texte],
# seul le préfixe est protégé, le texte coloré reste traduisible.
PROTECTED_TAG = re.compile(r"\[c/[0-9A-Fa-f]{6}:|\[(?!c/)[^\]]*\]|\{[^}]*\}")


def load_calamity_strings():
    values = {}

    def flatten(obj, path):
        if isinstance(obj, dict):
            for k, v in obj.items():
                flatten(v, f"{path}.{k}" if path else k)
        elif isinstance(obj, str):
            values[path] = obj

    for path in sorted(glob.glob(os.path.join(CAL_LOC, "Mods.CalamityMod*.hjson"))):
        prefix = os.path.basename(path)[:-len(".hjson")]
        flatten(hjson.load(open(path, encoding="utf-8-sig")), prefix)
    return values


def case_variants(old, new):
    yield old, new
    yield old[0].upper() + old[1:], new[0].upper() + new[1:]
    yield old.upper(), new.upper()


def build_rules(glossary, index):
    rules = []
    for old, new in glossary["renames"].items():
        rules.append((old, new))
    for old, new in glossary["terms"].items():
        rules.extend(case_variants(old, new))
    # Les plus longs d'abord (« rogues » avant « rogue »)
    rules.sort(key=lambda r: -len(r[0]))
    compiled = [(re.compile(rf"(?<![\w-]){re.escape(old)}(?=(?:s|es)?\b)"), new) for old, new in rules]

    # Noms officiels qui ont encore leur propre page sur le wiki : intouchables
    protected = set()
    olds = [old for old, _ in rules]
    for page in index.values():
        if page.get("kind") == "content" and page.get("status") == "official":
            if any(o in page["title"] for o in olds):
                protected.add(page["title"])
    protected.update(glossary.get("keep", []))
    return compiled, sorted(protected, key=len, reverse=True)


def transform(text, rules, protected):
    stash = []

    def keep(m):
        stash.append(m.group(0))
        return f"\x00{len(stash) - 1}\x00"

    text = PROTECTED_TAG.sub(keep, text)
    for name in protected:
        text = re.sub(rf"\b{re.escape(name)}\b", keep, text)
    for pattern, new in rules:
        text = pattern.sub(new, text)
    return re.sub(r"\x00(\d+)\x00", lambda m: stash[int(m.group(1))], text)


def nest(flat):
    root = {}
    for key, value in flat.items():
        parts = key.split(".")
        node = root
        for p in parts[:-1]:
            node = node.setdefault(p, {})
        node[parts[-1]] = value
    return root


def main():
    glossary = json.load(open(os.path.join(ROOT, "tools", "glossary.json"), encoding="utf-8"))
    index = json.load(open(os.path.join(ROOT, "data", "wiki_index.json"), encoding="utf-8"))
    rules, protected = build_rules(glossary, index)

    changed = {}
    for key, value in load_calamity_strings().items():
        new = transform(value, rules, protected)
        if new != value:
            changed[key] = (value, new)

    prefix = "Mods.CalamityMod."
    body = nest({k[len(prefix):]: v[1] for k, v in sorted(changed.items())})
    os.makedirs(os.path.dirname(OUT), exist_ok=True)
    with open(OUT, "w", encoding="utf-8") as f:
        f.write("# Généré par tools/gen_localization.py depuis tools/glossary.json : ne pas éditer à la main.\n")
        f.write(hjson.dumps(body, indent="\t", ensure_ascii=False))
        f.write("\n")

    names = {k: v for k, v in changed.items() if k.endswith(".DisplayName")}
    with open(REPORT, "w", encoding="utf-8") as f:
        f.write("# Textes de Calamity réécrits\n\n_Généré par `tools/gen_localization.py`._\n\n")
        f.write(f"{len(changed)} textes modifiés, dont {len(names)} noms.\n\n")
        f.write("Noms officiels conservés car ils ont encore leur page sur le wiki : "
                + (", ".join(protected) or "aucun") + ".\n\n")
        f.write("| Clé | Avant | Après |\n|---|---|---|\n")
        for k, (old, new) in sorted(names.items(), key=lambda kv: kv[1][0]):
            f.write(f"| `{k[len(prefix):]}` | {old} | {new} |\n")
    print(f"{len(changed)} textes modifiés ({len(names)} noms) -> {os.path.relpath(OUT, ROOT)}")


if __name__ == "__main__":
    main()
