"""Extrait le butin des infobox de PNJ du wiki vers WikiData/loot.json.

Formats rencontrés dans les infobox (paramètres sans nom, dans l'ordre) :
  | Objet | 7-14 | 100%                              -> objet, quantité, chance
  | custom | {{item|Objet|note=(Post-Golem)}} | 10%  -> objet avec condition écrite en note
  | group | One of the following 5 items... | 5     -> les 5 entrées suivantes forment un groupe
Quantités et chances en {{dv|normal|expert|...}} : valeur du mode normal.
L'application en jeu (Common/WikiData/WikiLoot.cs) est additive : seuls les objets que le PNJ ne lâche pas déjà sont
ajoutés, et les entrées avec une note de condition ne sont pas appliquées (listées dans le rapport).

Usage : python tools/gen_loot.py   (après wiki_index.py)
"""
import json
import os
import re
import sys

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from wiki_index import ROOT, WIKI, EDITS, EXTRA_MAIN, INFOBOX_RE, extract_template  # noqa: E402
from gen_recipes import split_all, clean_name  # noqa: E402

OUT = os.path.join(ROOT, "WikiData", "loot.json")
NOTE = "@@note:"  # marqueur d'une condition « note=... » placée entre deux paramètres
REPORT = os.path.join(ROOT, "reports", "loot.md")


def first_dv(raw):
    raw = re.sub(r"<!--.*?-->", "", raw or "", flags=re.S).strip()
    m = re.match(r"^\{\{\s*dv\s*\|([^{}]*)\}\}", raw, re.I)
    return m.group(1).split("|")[0].strip() if m else raw


def parse_qty(raw):
    v = first_dv(raw).replace(",", "")
    m = re.fullmatch(r"(\d+)\s*(?:[-–]\s*(\d+))?", v)
    if not m:
        return None
    lo = int(m.group(1))
    return lo, int(m.group(2) or lo)


def parse_chance(raw):
    v = first_dv(raw).replace(",", ".")
    m = re.fullmatch(r"(\d+(?:\.\d+)?)\s*%", v)
    if m:
        return min(float(m.group(1)) / 100, 1.0)
    m = re.fullmatch(r"1\s*/\s*(\d+)", v)
    return 1 / int(m.group(1)) if m else None


def parse_drops(positional):
    drops, problems = [], []
    i, group, group_left, group_id = 0, None, 0, 0
    while i < len(positional):
        tok = positional[i].strip()
        if tok.lower() == "group" and i + 2 < len(positional):
            group_id += 1
            group = {"id": group_id, "text": clean_name(positional[i + 1]), "always": "always" in positional[i + 1].lower()}
            try:
                group_left = int(re.sub(r"\D", "", positional[i + 2]) or 0)
            except ValueError:
                group_left = 0
            i += 3
            continue
        note = None
        if tok.lower() == "custom" and i + 2 < len(positional):
            m = re.search(r"\{\{\s*item\s*\|([^|}]*)(.*?)\}\}", positional[i + 1], re.I | re.S)
            name = clean_name(m.group(1)) if m else clean_name(positional[i + 1])
            if m:
                n = re.search(r"(?:note\s*=|@@note:)\s*([^|}]*)", m.group(2))
                note = clean_name(n.group(1)) if n else None
            qty, chance = (1, 1), parse_chance(positional[i + 2])
            i += 3
        elif i + 1 < len(positional) and positional[i + 1].startswith(NOTE) and i + 3 < len(positional):
            # « | Abyss Bar|note=(Post-Golem)|3-8|100% » : condition en ligne
            name, note = clean_name(tok), clean_name(positional[i + 1][len(NOTE):])
            qty, chance = parse_qty(positional[i + 2]) or (1, 1), parse_chance(positional[i + 3])
            i += 4
        elif i + 2 < len(positional) and parse_qty(positional[i + 1]) is not None:
            name, qty, chance = clean_name(tok), parse_qty(positional[i + 1]), parse_chance(positional[i + 2])
            i += 3
        else:
            problems.append(tok[:60])
            i += 1
            continue
        if not name or chance is None:
            problems.append(name or tok[:60])
            continue
        drop = {"item": name, "min": qty[0], "max": qty[1], "chance": round(chance, 4)}
        if note:
            drop["note"] = note
        if group and group_left > 0:
            drop["group"] = group["id"]
            drop["groupAlways"] = group["always"]
            group_left -= 1
        drops.append(drop)
    return drops, problems


def main():
    sources = {f: WIKI for f in os.listdir(WIKI) if f.endswith(".txt")}
    for folder in (EXTRA_MAIN, EDITS):
        if os.path.isdir(folder):
            sources.update({f: folder for f in os.listdir(folder) if f.endswith(".txt")})
    index = {p["file"]: p for p in json.load(open(os.path.join(ROOT, "data", "wiki_index.json"), encoding="utf-8")).values()}

    npcs, problems, conditional = {}, {}, []
    for fname in sorted(sources):
        page = index.get(fname)
        if not page or page.get("kind") != "content" or not page.get("infobox", "").startswith("npc"):
            continue
        text = open(os.path.join(sources[fname], fname), encoding="utf-8", errors="replace").read()
        m = INFOBOX_RE.search(text)
        if not m:
            continue
        # « |note=... » est un paramètre nommé pour MediaWiki, mais il qualifie le drop qui le précède : on le garde
        body = re.sub(r"\|\s*note\s*=", "|" + NOTE, extract_template(text, m.start()))
        positional, _ = split_all(body)
        drops, bad = parse_drops(positional)
        if bad:
            problems[page["title"]] = bad
        if drops:
            npcs[page["title"]] = {"page": fname, "boss": "boss" in (page.get("type") or "").lower(), "drops": drops}
            conditional += [(page["title"], d["item"], d["note"]) for d in drops if d.get("note")]

    # Alias de noms de PNJ (comme pour les éléments) : redirections, glossaire, nom de l'infobox
    elements = json.load(open(os.path.join(ROOT, "WikiData", "elements.json"), encoding="utf-8"))
    aliases = {t: a for t, a in elements.get("aliases", {}).items() if t in npcs}

    with open(OUT, "w", encoding="utf-8") as f:
        json.dump({"_generated_by": "tools/gen_loot.py", "npcs": npcs, "aliases": aliases}, f, ensure_ascii=False, indent=1)
    total = sum(len(v["drops"]) for v in npcs.values())
    with open(REPORT, "w", encoding="utf-8") as f:
        f.write("# Butin du wiki\n\n_Généré par `tools/gen_loot.py`._ Le bilan appliqué en jeu est dans `reports/loot_applied.md`.\n\n")
        f.write(f"- {len(npcs)} PNJ, {total} entrées de butin\n- {len(conditional)} entrées avec condition en note (non appliquées)\n")
        f.write(f"- {len(problems)} infobox avec des paramètres non compris\n\n## Entrées conditionnelles\n\n")
        f.write("| PNJ | Objet | Condition |\n|---|---|---|\n")
        for npc, item, note in conditional:
            f.write(f"| {npc} | {item} | {note} |\n")
        f.write("\n## Paramètres non compris\n\n")
        for npc, bad in sorted(problems.items()):
            f.write(f"- {npc} : {', '.join(bad)}\n")
    print(f"{len(npcs)} PNJ, {total} entrées ({len(conditional)} conditionnelles), {len(problems)} infobox avec restes")


if __name__ == "__main__":
    main()
