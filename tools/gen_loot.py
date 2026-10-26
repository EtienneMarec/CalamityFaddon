"""Extrait le butin des infobox de PNJ du wiki vers WikiData/loot.json.

Formats rencontrés dans les infobox (paramètres sans nom, dans l'ordre) :
  | Objet | 7-14 | 100%                              -> objet, quantité, chance
  | custom | {{item|Objet|note=(Post-Golem)}} | 10%  -> objet avec condition écrite en note
  | custom | {{item|Treasure Bag (Crabulon)|...}}     -> sans chance : objet seulement cité (« listed » : sac, relique),
                                                        pas ajouté mais pas retiré en Full Fandom
  | group | One of the following 5 items... | 5     -> les 5 entrées suivantes forment un groupe
Quantités et chances en {{dv|normal|expert|...}} : valeur du mode normal, et valeur Expert (minExpert, maxExpert,
chanceExpert) quand elle diffère. Page « Treasure Bag » : contenu de chaque sac (« bags »).
L'application en jeu (Common/WikiData/WikiLoot.cs) est additive : seuls les objets que le PNJ ne lâche pas déjà sont
ajoutés, et les entrées avec une note de condition ne sont pas appliquées (listées dans le rapport).

Usage : python tools/gen_loot.py   (après wiki_index.py)
"""
import json
import os
import re
import sys

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from wiki_index import ROOT, OUR_MAIN, INFOBOX_RE, extract_template, has_mode, pick_mode  # noqa: E402
from gen_recipes import split_all, clean_name, redirect_aliases  # noqa: E402

OUT = os.path.join(ROOT, "WikiData", "loot.json")
NOTE = "@@note:"  # marqueur d'une condition « note=... » placée entre deux paramètres
REPORT = os.path.join(ROOT, "reports", "loot.md")
# Page « Treasure Bag » : tableau d'un boss, objet, ligne, cellule
TABLE_RE = r'\{\|\s*class="terraria"\s*id="([^"]+)"(.*?)\n\|\}'
ITEM_RE = r"\{\{\s*item\s*\|([^|}]+)[^}]*\}\}"
ROW_RE = r"\n\|-[^\n]*"
CELL_RE = r"\|\||\n\|"


def first_dv(raw, expert=False):
    """Valeur du mode normal d'un {{dv|normal|expert|...}} ; avec expert=True, celle de l'Expert (vide = la même)."""
    raw = re.sub(r"<!--.*?-->", "", raw or "", flags=re.S).strip()
    m = re.match(r"^\{\{\s*dv\s*\|([^{}]*)\}\}", raw, re.I)
    if not m:
        return raw
    values = [v.strip() for v in m.group(1).split("|")]
    return values[1] if expert and len(values) > 1 and values[1] else values[0]


def parse_qty(raw, expert=False):
    v = first_dv(raw, expert).replace(",", "")
    m = re.fullmatch(r"(\d+)\s*(?:[-–]\s*(\d+))?", v)
    if not m:
        return None
    lo = int(m.group(1))
    return lo, int(m.group(2) or lo)


def parse_chance(raw, expert=False):
    v = first_dv(raw, expert).replace(",", ".")
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
        note = hidden_note = None
        if tok.lower() == "custom" and i + 1 < len(positional):
            m = re.search(r"\{\{\s*item\s*\|([^|}]*)(.*?)\}\}", positional[i + 1], re.I | re.S)
            name = clean_name(m.group(1)) if m else clean_name(positional[i + 1])
            if m:
                n = re.search(r"(?:note\s*=|@@note:)\s*([^|}]*)", m.group(2))
                note = clean_name(n.group(1)) if n else None
            chance = parse_chance(positional[i + 2]) if i + 2 < len(positional) else None
            if chance is None:
                # Ligne « custom » sans chance (sac, relique) : l'objet est cité, rien à tirer au sort
                drops.append({"item": name, "listed": True})
                i += 2
                continue
            # Quantité écrite après le modèle : « {{item|Depth Cells|...}} ({{dv|1-2|2-3}}) »
            after = positional[i + 1][m.end():] if m else ""
            qm = re.search(r"\((.*)\)", after)
            qty_raw, chance_raw = (qm.group(1).strip() if qm else "1"), positional[i + 2]
            i += 3
        elif i + 1 < len(positional) and positional[i + 1].startswith(NOTE) and i + 3 < len(positional):
            # « | Abyss Bar|note=(Post-Golem)|3-8|100% » : hors d'un {{item}}, « note= » est un paramètre nommé de
            # l'infobox, que le modèle n'affiche pas : sur le wiki affiché, il n'y a pas de condition (décision du 22/10)
            name = clean_name(tok)
            hidden_note = clean_name(positional[i + 1][len(NOTE):])
            qty_raw, chance_raw = positional[i + 2], positional[i + 3]
            i += 4
        elif i + 2 < len(positional) and parse_qty(positional[i + 1]) is not None:
            name, qty_raw, chance_raw = clean_name(tok), positional[i + 1], positional[i + 2]
            i += 3
        else:
            problems.append(tok[:60])
            i += 1
            continue
        qty, chance = parse_qty(qty_raw) or (1, 1), parse_chance(chance_raw)
        if not name or chance is None:
            problems.append(name or tok[:60])
            continue
        drop = {"item": name, "min": qty[0], "max": qty[1], "chance": round(chance, 4)}
        # Valeurs Expert du {{dv}} (sac ou drop direct en Expert, voir WikiLoot.cs), seulement si elles diffèrent
        qty_x, chance_x = parse_qty(qty_raw, True) or qty, parse_chance(chance_raw, True)
        if qty_x != qty:
            drop["minExpert"], drop["maxExpert"] = qty_x
        if chance_x is not None and abs(chance_x - chance) > 1e-6:
            drop["chanceExpert"] = round(chance_x, 4)
        if note:
            drop["note"] = note
            condition = parse_condition(note)
            if condition:
                drop["condition"] = condition
                drop["conditionText"] = condition_text(note)
        if hidden_note:
            drop["hiddenNote"] = hidden_note  # pour le rapport seulement
        if group and group_left > 0:
            drop["group"] = group["id"]
            drop["groupAlways"] = group["always"]
            group_left -= 1
        drops.append(drop)
    return drops, problems


# Conditions de butin écrites en note (« (After defeating Golem) ») que le jeu sait vérifier : boss vaincus (noms du
# wiki, en minuscules ; le C# les relie aux indicateurs du jeu), biome du joueur, premier kill. Une note dont une partie
# n'est pas comprise reste non appliquée (rapport).
BOSSES = ["wall of flesh", "skeletron", "plantera", "golem", "moon lord", "desert scourge", "cryogen", "calamitas",
          "astrum aureus", "astrum deus", "astral deus", "providence", "polterghast", "storm weaver", "ceaseless void",
          "signus", "devourer of gods"]
BIOMES = {"sunken sea": "sunkensea", "space": "space", "jungle": "jungle", "underworld": "underworld", "ice biome": "snow"}


def condition_text(note):
    """Note du wiki lisible en jeu (bestiaire) : « (After defeating<br/>[[Golem]]) » -> « After defeating Golem »."""
    t = re.sub(r"<br\s*/?>", " ", note)
    t = re.sub(r"\[\[(?:[^|\]]*\|)?([^\]]*)\]?\]?", r"\1", t)
    t = re.sub(r"\{\{.*", "", t)
    t = re.sub(r"[()\[\]]", "", t)
    return re.sub(r"\s+", " ", t).strip()


def parse_condition(note):
    """« (After defeating<br/>Golem) » -> {"downed": ["golem"]} ; None si la note n'est pas entièrement comprise."""
    t = re.sub(r"<br\s*/?>", " ", note)
    t = re.sub(r"\[\[(?:[^|\]]*\|)?([^\]]*)\]?\]?", r"\1", t)  # liens, même mal fermés
    t = re.sub(r"\{\{.*", "", t)  # modèles décoratifs en fin de note ({{fantag...)
    t = re.sub(r"[()\[\]]", " ", t).lower()
    t = re.sub(r"\s+", " ", t).strip()
    cond, rest = {}, t
    if "first kill" in rest:
        cond["firstKill"] = True
        rest = rest.replace("always on first kill", "")
    if "hardmode" in rest:
        cond.setdefault("downed", []).append("wall of flesh")
        rest = re.sub(r"hardmode(?:[- ]only)?", "", rest)
    if "godseeker mode" in rest:
        cond.setdefault("downed", []).append("moon lord")
        rest = rest.replace("godseeker mode", "")
    for name in sorted(BOSSES, key=len, reverse=True):
        if name in rest:
            cond.setdefault("downed", []).append(name)
            rest = rest.replace(name, "")
    for name, key in BIOMES.items():
        if name in rest:
            cond["biome"] = key
            rest = rest.replace(name, "")
    # Ce qui reste ne doit être que des mots de liaison (« after defeating », « when defeated in the », « post- »)
    leftover = re.sub(r"\b(?:after|defeating|once|is|are|has|have|been|defeated|post|the|and|if|when|in|only|during|"
                      r"profaned|goddess)\b|[-,]", " ", rest)
    if not cond or leftover.strip():
        return None
    return cond


def parse_bags(text):
    """Page « Treasure Bag » : un tableau par boss (id = titre du boss), en-tête = le sac, lignes = objet || chance ||
    quantité. Renvoie {boss: {"bag": nom du sac, "drops": [...]}}."""
    bags = {}
    for boss, body in re.findall(TABLE_RE, text, re.S):
        head = re.search(ITEM_RE, body)
        if not head:
            continue
        drops = []
        for row in re.split(ROW_RE, body)[1:]:
            m = re.search(ITEM_RE, row)
            if not m:
                continue
            cells = [c.strip() for c in re.split(CELL_RE, row[m.end():]) if c.strip()]
            chance = parse_chance(cells[0]) if cells else None
            if chance is None:
                continue
            qty = (parse_qty(cells[1]) if len(cells) > 1 else None) or (1, 1)
            drops.append({"item": clean_name(m.group(1)), "min": qty[0], "max": qty[1], "chance": round(chance, 4)})
        bags[boss] = {"bag": clean_name(head.group(1)), "drops": drops}
    return bags


def bags_by_mode(text):
    """Sacs avec {{mode}} : chaque objet qui diffère entre les deux modes est marqué de son mode (comme le butin)."""
    if not has_mode(text):
        return parse_bags(text)
    views = {mode: parse_bags(pick_mode(text, mode)) for mode in ("fandom", "plus")}
    bags = {}
    key = lambda d: json.dumps(d, sort_keys=True)
    for boss in set(views["fandom"]) | set(views["plus"]):
        f, p = views["fandom"].get(boss, {"drops": []}), views["plus"].get(boss, {"drops": []})
        fk, pk = {key(d) for d in f["drops"]}, {key(d) for d in p["drops"]}
        drops = [d if key(d) in fk else {**d, "modes": ["plus"]} for d in p["drops"]]
        drops += [{**d, "modes": ["fandom"]} for d in f["drops"] if key(d) not in pk]
        bags[boss] = {"bag": p.get("bag") or f.get("bag"), "drops": drops}
    return bags


def drops_by_mode(positional):
    """Butin d'une infobox ; avec {{mode|Full Fandom|Calamity++}} dans un paramètre, chaque drop qui n'est pas le même
    dans les deux modes est marqué de son mode (« modes »). Un {{mode|...|}} vide retire le paramètre dans ce mode."""
    if not any(has_mode(p) for p in positional):
        return parse_drops(positional)
    views = {}
    bad = []
    for mode in ("fandom", "plus"):
        tokens = [t for t in (pick_mode(p, mode) for p in positional) if t.strip()]
        views[mode], b = parse_drops(tokens)
        bad += b
    key = lambda d: json.dumps(d, sort_keys=True)
    fandom_keys = {key(d) for d in views["fandom"]}
    plus_keys = {key(d) for d in views["plus"]}
    drops = [d if key(d) in fandom_keys else {**d, "modes": ["plus"]} for d in views["plus"]]
    drops += [{**d, "modes": ["fandom"]} for d in views["fandom"] if key(d) not in plus_keys]
    return drops, sorted(set(bad))


ITEM_DROPS_RE = re.compile(r"\{\{\s*item drops infobox", re.I)


def add_item_page_drops(index, npcs, problems):
    """Infobox « item drops » des pages d'objets (« | [[Devil Fish]] (After Golem has been defeated) | 3-9 | 100% ») :
    ajoute au butin des PNJ ce que leur propre page ne cite pas. Un PNJ connu seulement par ces infobox (Wall of Flesh,
    Plantera...) est marqué « itemPagesOnly » : le Full Fandom ne lui retire rien (sa page ne liste pas tout son butin).
    Sources génériques (« any Jungle enemy », « Any Boss ») ignorées pour l'instant."""
    norm = lambda t: re.sub(r"[^a-z0-9]", "", t.lower())
    redirects = {norm(p["title"]): p.get("target", "").split("#")[0] for p in index.values() if p.get("kind") == "redirect"}
    by_norm = {norm(t): t for t in npcs}
    # Un objet peut avoir deux noms sur le wiki (« Scoria Ore » sur la page du Devil Fish, « Abyss Ore » sur sa propre
    # page) : on compare les noms ramenés au même objet (redirections, puis renommages du glossaire)
    renames = {norm(k): v for k, v in json.load(open(os.path.join(ROOT, "tools", "glossary.json"), encoding="utf-8"))["renames"].items()}

    def canon(name):
        target = redirects.get(norm(name)) or name
        return norm(renames.get(norm(target), target))

    known = {(norm(t), canon(d["item"])) for t, v in npcs.items() for d in v["drops"]}
    added, generic, unread = 0, 0, 0
    for page in sorted(index.values(), key=lambda p: p.get("title", "")):
        if page.get("kind") != "content" or not page.get("infobox", "").startswith("item"):
            continue
        text = open(os.path.join(OUR_MAIN, page["file"]), encoding="utf-8", errors="replace").read()
        m = ITEM_DROPS_RE.search(text)
        if not m:
            continue
        body = extract_template(text, m.start())
        if has_mode(body):
            unread += 1  # pas encore de {{mode}} dans ces infobox
            continue
        tokens = [t.strip() for t in split_all(body)[0]]
        item = page["title"]
        for i in range(0, len(tokens) - 2, 3):
            source, qty_raw, chance_raw = tokens[i], tokens[i + 1], tokens[i + 2]
            source = re.sub(r"\[\[(?:[^|\]]*\|)?([^\]]*)\]\]", r"\1", source)
            cm = re.match(r"^(.*?)\s*\((.*)\)\s*$", source)
            name, cond_raw = (cm.group(1), cm.group(2)) if cm else (source, None)
            name = clean_name(name)
            if not name or name.lower().startswith("any "):
                generic += 1
                continue
            # Le PNJ sous son titre du wiki (redirections suivies : « Plaguebringer Goliath » -> « The Plaguebringer Goliath »)
            title = by_norm.get(norm(name)) or by_norm.get(norm(redirects.get(norm(name), ""))) or name
            if (norm(title), canon(item)) in known:
                continue
            qty, chance = parse_qty(qty_raw), parse_chance(chance_raw)
            if qty is None or chance is None:
                problems.setdefault(item + " (page d'objet)", []).append(f"{name} | {qty_raw} | {chance_raw}"[:80])
                continue
            drop = {"item": item, "min": qty[0], "max": qty[1], "chance": round(chance, 4), "itemPage": page["file"]}
            qty_x, chance_x = parse_qty(qty_raw, True) or qty, parse_chance(chance_raw, True)
            if qty_x != qty:
                drop["minExpert"], drop["maxExpert"] = qty_x
            if chance_x is not None and abs(chance_x - chance) > 1e-6:
                drop["chanceExpert"] = round(chance_x, 4)
            if cond_raw:
                note = f"({cond_raw})"
                drop["note"] = note
                condition = parse_condition(note)
                if condition:
                    drop["condition"] = condition
                    drop["conditionText"] = condition_text(note)
            if title not in npcs:
                target = index.get(title + ".txt") or {}
                npcs[title] = {"page": None, "boss": "boss" in (target.get("type") or "").lower(), "drops": [],
                               "itemPagesOnly": True}
                by_norm[norm(title)] = title
            npcs[title]["drops"].append(drop)
            known.add((norm(title), canon(item)))
            added += 1
    return {"added": added, "generic": generic, "unread": unread}


def main():
    sources = {f: OUR_MAIN for f in os.listdir(OUR_MAIN) if f.endswith(".txt")}
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
        drops, bad = drops_by_mode(positional)
        if bad:
            problems[page["title"]] = bad
        if drops:
            npcs[page["title"]] = {"page": fname, "boss": "boss" in (page.get("type") or "").lower(), "drops": drops}
            conditional += [(page["title"], d["item"], d["note"]) for d in drops if d.get("note")]

    item_page_stats = add_item_page_drops(index, npcs, problems)
    conditional += [(t, d["item"], d["note"]) for t, v in npcs.items() for d in v["drops"] if d.get("note") and d.get("itemPage")]

    # Alias de noms de PNJ (comme pour les éléments) : redirections, glossaire, nom de l'infobox
    elements = json.load(open(os.path.join(ROOT, "WikiData", "elements.json"), encoding="utf-8"))
    aliases = {t: a for t, a in elements.get("aliases", {}).items() if t in npcs}

    bag_page = os.path.join(OUR_MAIN, "Treasure Bag.txt")
    bags = bags_by_mode(open(bag_page, encoding="utf-8").read()) if os.path.exists(bag_page) else {}

    # Noms d'objets du butin qui sont d'anciens noms (« Victory Shard » -> Pearl Shard) : redirections du wiki, puis
    # renommages du glossaire (Pearl Shard -> Conquest Fragment, le nom en jeu)
    names = ({d["item"] for v in npcs.values() for d in v["drops"]} | {d["item"] for b in bags.values() for d in b["drops"]}
             | {b["bag"] for b in bags.values() if b.get("bag")})
    renames = json.load(open(os.path.join(ROOT, "tools", "glossary.json"), encoding="utf-8"))["renames"]
    item_aliases = {k: renames.get(v, v) for k, v in redirect_aliases(names).items()}

    with open(OUT, "w", encoding="utf-8") as f:
        json.dump({"_generated_by": "tools/gen_loot.py", "npcs": npcs, "bags": bags, "aliases": aliases,
                   "itemAliases": item_aliases},
                  f, ensure_ascii=False, indent=1)
    total = sum(len(v["drops"]) for v in npcs.values())
    with open(REPORT, "w", encoding="utf-8") as f:
        f.write("# Butin du wiki\n\n_Généré par `tools/gen_loot.py`._ Le bilan appliqué en jeu est dans `reports/loot_applied.md`.\n\n")
        f.write(f"- {len(npcs)} PNJ, {total} entrées de butin\n- {len(conditional)} entrées avec condition en note "
                "(appliquées quand `parse_condition` la comprend, voir `reports/loot_applied.md`)\n")
        f.write(f"- Pages d'objets (infobox « item drops ») : {item_page_stats['added']} drops ajoutés aux PNJ, "
                f"{item_page_stats['generic']} sources génériques ignorées (« any Jungle enemy »...)\n")
        f.write(f"- {len(problems)} infobox avec des paramètres non compris\n\n## Entrées conditionnelles\n\n")
        f.write("| PNJ | Objet | Condition |\n|---|---|---|\n")
        for npc, item, note in conditional:
            f.write(f"| {npc} | {item} | {note} |\n")
        f.write("\n## Paramètres non compris\n\n")
        for npc, bad in sorted(problems.items()):
            f.write(f"- {npc} : {', '.join(bad)}\n")
    print(f"{len(npcs)} PNJ, {total} entrées ({len(conditional)} conditionnelles), {len(problems)} infobox avec restes ; "
          f"{len(bags)} sacs ({sum(len(b['drops']) for b in bags.values())} objets) ; pages d'objets : "
          f"{item_page_stats['added']} drops ajoutés, {item_page_stats['generic']} sources génériques ignorées")


if __name__ == "__main__":
    main()
