"""Écrit sur les pages de wiki/ les stats « Full Fandom seulement » sous forme {{mode|valeur du wiki|valeur Calamity++}}.

Les règles automatiques de tools/gen_stats.py (valeur identique à Calamity 2.0.1.005, décisions « fullOnly » de
tools/stats_decisions.json) deviennent ainsi visibles sur le wiki du mod. La valeur Calamity++ est la valeur actuelle de
Calamity. Ne touche pas un champ :
  - dont la valeur actuelle n'est pas une constante lisible dans le code de Calamity ;
  - partagé par plusieurs classes aux valeurs actuelles différentes (segments de ver...) ;
  - qui contient déjà un {{mode}}.
Après coup, les données générées doivent être identiques (les {{mode}} passent avant les règles).

Usage : python tools/gen_stats.py && python tools/apply_modes.py   (puis tools/build_data.py)
"""
import collections
import json
import os
import re
import sys

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from wiki_index import OUR_MAIN, ROOT, INFOBOX_RE, extract_template, has_mode, load_calamity_classes  # noqa: E402
from gen_stats import ITEM_WIKI_FIELD  # noqa: E402

NPC_WIKI_FIELD = {"life": "life", "damage": "damage", "defense": "defense"}


def plus_text(entry):
    """Valeur actuelle de Calamity au format du wiki."""
    code = entry["code"]
    if entry["kind"] == "ModItem" and entry["field"] == "crit":
        return f"{code + 4:g}%"  # le wiki affiche la chance totale (4 % de base)
    return f"{code:g}"


def main():
    entries = json.load(open(os.path.join(ROOT, "data", "stats_full_only.json"), encoding="utf-8"))
    groups = collections.defaultdict(list)
    for e in entries:
        groups[(e["file"], e["kind"], e["field"])].append(e)

    # Classes de même type rattachées à chaque page : un champ n'est converti que si la règle vaut pour toutes
    classes = load_calamity_classes()
    index = {p["file"]: p for p in json.load(open(os.path.join(ROOT, "data", "wiki_index.json"), encoding="utf-8")).values()}

    def page_classes(fname, kind):
        page = index.get(fname, {})
        return {m["class"] for m in page.get("calamity", []) if classes.get(m["class"], (None, None))[1] == kind}

    stats = collections.Counter()
    by_file = collections.defaultdict(dict)
    for (fname, kind, field), group in groups.items():
        if any(e["code"] is None for e in group):
            stats["valeur actuelle non lisible"] += 1
            continue
        if len({e["code"] for e in group}) > 1:
            stats["classes aux valeurs différentes"] += 1
            continue
        if {e["class"] for e in group} != page_classes(fname, kind):
            stats["page partagée avec une classe hors règle"] += 1
            continue
        wiki_field = (ITEM_WIKI_FIELD if kind == "ModItem" else NPC_WIKI_FIELD)[field]
        by_file[fname][wiki_field] = plus_text(group[0])

    for fname, fields in sorted(by_file.items()):
        path = os.path.join(OUR_MAIN, fname)
        with open(path, encoding="utf-8", newline="") as fh:
            text = fh.read()
        m = INFOBOX_RE.search(text)
        if not m:
            stats["infobox introuvable"] += len(fields)
            continue
        start = m.start()
        end = start + len(extract_template(text, start)) + 4
        box = text[start:end]
        for wiki_field, plus in fields.items():
            line = re.compile(rf"(^\|\s*{re.escape(wiki_field)}\s*=[ \t]*)([^\r\n]*?)([ \t]*)(\r?\n)", re.M | re.I)
            lm = line.search(box)
            if not lm or not lm.group(2).strip():
                stats["champ introuvable"] += 1
                continue
            if has_mode(lm.group(2)):
                stats["déjà en {{mode}}"] += 1
                continue
            value = f"{{{{mode|{lm.group(2).strip()}|{plus}}}}}"
            box = box[:lm.start()] + lm.group(1) + value + lm.group(3) + lm.group(4) + box[lm.end():]
            stats["converti"] += 1
        text = text[:start] + box + text[end:]
        with open(path, "w", encoding="utf-8", newline="") as fh:
            fh.write(text)

    for k, v in stats.most_common():
        print(f"{k} : {v}")


if __name__ == "__main__":
    main()
