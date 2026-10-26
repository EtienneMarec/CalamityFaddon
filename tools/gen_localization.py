"""Génère les surcharges de textes de Calamity d'après le vocabulaire du wiki (tools/glossary.json).

tModLoader charge les .hjson de chaque mod dans l'ordre de chargement : un fichier dont le préfixe est
Mods.CalamityMod, dans notre mod (chargé après Calamity), remplace les textes de Calamity.

Sortie :
  Localization/en-US/Mods.CalamityMod.hjson   textes modifiés uniquement (généré, ne pas éditer). tModLoader le
                                              reformate au chargement : il n'est réécrit que si le contenu change.
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
            # Un renommage décidé explicitement (« Ravager » -> « Meatmetal Sans ») passe avant la protection
            if any(o in page["title"] for o in olds) and page["title"] not in glossary["renames"]:
                protected.add(page["title"])
    protected.update(glossary.get("keep", []))
    return compiled, sorted(protected, key=lambda s: (-len(s), s))


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


def flatten_file(path):
    """Clés à plat d'un .hjson ; les clés pointées de tModLoader (« A.B: x ») sont dépliées pareil."""
    flat = {}

    def walk(obj, prefix):
        for k, v in obj.items():
            key = f"{prefix}.{k}" if prefix else k
            if isinstance(v, dict):
                walk(v, key)
            else:
                flat[key.replace(".$parentVal", "")] = v

    walk(hjson.load(open(path, encoding="utf-8-sig")), "")
    return flat


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

    # Objets renommés par Calamity depuis 2.0.1.005 (tools/calamity_renames.py, décision du 23/10) : ils reprennent le
    # nom de leur page. Le nom seul : beaucoup de nouveaux noms sont des mots courants (« Radiance ») qu'une substitution
    # dans tous les textes abîmerait. Un nouveau nom qui a sa propre page sur le wiki est gardé.
    from calamity_renames import load as load_renames, wiki_known
    pages = {re.sub(r"[^a-z0-9]", "", p["title"].lower()): p for p in index.values()}
    strings = load_calamity_strings()
    by_class = {}
    for key in strings:
        if key.endswith(".DisplayName"):
            by_class.setdefault(key.split(".")[-2], key)
    for r in wiki_known(load_renames(), pages):
        own_page = pages.get(re.sub(r"[^a-z0-9]", "", r["newName"].lower()))
        key = by_class.get(r["new"])
        if r["newName"] == r["oldName"] or not key or (own_page and own_page.get("kind") == "content"):
            continue
        # Titre de la page (redirections suivies : Fabsol's Vodka -> Fluorescent Vodka), sauf page de groupe (« Treasure
        # Bag (Calamitas) » -> « Treasure Bag ») où l'ancien nom reste ; sans parenthèse de désambiguïsation « (weapon) »
        target = pages.get(re.sub(r"[^a-z0-9]", "", r["page"].lower()), {})
        name = r["oldName"] if target.get("status") == "group" else re.sub(r"\s*\([a-z][^)]*\)$", "", r["page"])
        changed[key] = (strings[key], name)

    # Objets de lore : nom du tableau de la page « Lore items » (tools/lore_items.py), y compris quand Calamity a
    # réécrit le lore sous un autre nom (LoreArchmage -> « Cryogen »). Les renommages du glossaire passent ensuite
    # (lore du Ravager : « Meatmetal Sans », décision du 21/10)
    from lore_items import lore_map
    for cls, name in lore_map().items():
        name = transform(name, rules, protected)
        key = by_class.get(cls)
        if key and strings[key] != name:
            changed[key] = (strings[key], name)

    prefix ="Mods.CalamityMod."
    wanted = {k[len(prefix):]: v[1] for k, v in sorted(changed.items())}
    # tModLoader reformate ce fichier à son chargement : on ne le réécrit que si le contenu change vraiment
    if os.path.exists(OUT) and flatten_file(OUT) == wanted:
        print("Contenu inchangé : fichier laissé tel quel")
    else:
        os.makedirs(os.path.dirname(OUT), exist_ok=True)
        with open(OUT, "w", encoding="utf-8") as f:
            f.write(hjson.dumps(nest(wanted), indent="\t", ensure_ascii=False))
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
