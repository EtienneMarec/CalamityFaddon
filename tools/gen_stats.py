"""Génère WikiData/stats.json : stats des infobox du wiki à appliquer aux objets et PNJ de Calamity.

- Objets : valeurs absolues (dégâts, temps d'utilisation, recul, critique, vitesse, mana, défense, rareté, prix).
- PNJ : ratios wiki / code en mode normal pour la vie et les dégâts (la montée en difficulté de Calamity est
  conservée), défense absolue.
Seules les valeurs nettes sont reprises (« 42 », « 7.5 », « 19% », « {{dv|normal|expert|...}} », « {{value|...}} ») ;
les infobox à paliers (« Tier 1 / Tier 2 », « pre-Astrum Aureus ») sont ignorées.

Garde-fou : un écart supérieur à x5 (ou inférieur à /5) par rapport au code n'est appliqué qu'en mode Full Fandom
(sections fullItems / fullNpcs) ; il est listé dans reports/stats.md. Pour l'appliquer dans tous les modes :
l'ajouter à tools/stats_force.json ; pour le corriger : copier la page dans wiki_edits/.

Usage : python tools/gen_stats.py   (après wiki_index.py)
"""
import json
import os
import re
import sys

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from wiki_index import ROOT, load_calamity_classes  # noqa: E402

OUT = os.path.join(ROOT, "WikiData", "stats.json")
REPORT = os.path.join(ROOT, "reports", "stats.md")
FORCE = os.path.join(ROOT, "tools", "stats_force.json")
MAX_RATIO = 5.0

PLAIN = re.compile(r"^-?[\d,]+(?:\.\d+)?%?$")
DV = re.compile(r"^\{\{\s*dv\s*\|([^{}]*)\}\}$", re.I)
VALUE = re.compile(r"^\{\{\s*value\s*\|([^{}]*)\}\}$", re.I)


def clean(raw):
    return re.sub(r"<!--.*?-->", "", raw or "", flags=re.S).strip()


def number(raw):
    """Valeur nette (mode normal pour {{dv}}), sinon None."""
    v = clean(raw)
    m = DV.match(v)
    if m:
        v = m.group(1).split("|")[0].strip()
    if not PLAIN.match(v):
        return None
    return float(v.replace(",", "").rstrip("%"))


def sell_copper(raw):
    m = VALUE.match(clean(raw))
    if not m:
        return None
    parts = [p.strip() or "0" for p in m.group(1).split("|")]
    if not all(re.fullmatch(r"\d+", p) for p in parts):
        return None
    parts = (parts + ["0"] * 4)[:4]
    p, g, s, c = (int(x) for x in parts)
    return p * 1_000_000 + g * 10_000 + s * 100 + c


def literal(text, pattern):
    m = re.search(pattern, text)
    return float(m.group(1)) if m else None


def item_code(text):
    return {
        "damage": literal(text, r"\bItem\.damage\s*=\s*(\d+)\s*;"),
        "useAnimation": literal(text, r"\bItem\.useAnimation\s*=\s*(?:Item\.useTime\s*=\s*)?(\d+)\s*;")
                        or literal(text, r"\bItem\.useTime\s*=\s*Item\.useAnimation\s*=\s*(\d+)\s*;"),
        "knockBack": literal(text, r"\bItem\.knockBack\s*=\s*(\d+(?:\.\d+)?)f?\s*;"),
        "crit": literal(text, r"\bItem\.crit\s*=\s*(\d+)\s*;"),
        "shootSpeed": literal(text, r"\bItem\.shootSpeed\s*=\s*(\d+(?:\.\d+)?)f?\s*;"),
        "mana": literal(text, r"\bItem\.mana\s*=\s*(\d+)\s*;"),
        "defense": literal(text, r"\bItem\.defense\s*=\s*(\d+)\s*;"),
    }


def npc_code(text):
    life = literal(text, r"LifeMaxNERB\(\s*(\d+)") or literal(text, r"\bNPC\.lifeMax\s*=\s*(\d+)\s*;")
    return {
        "life": life,
        "damage": literal(text, r"\bNPC\.damage\s*=\s*(\d+)\s*;"),
        "defense": literal(text, r"\bNPC\.defense\s*=\s*(\d+)\s*;"),
    }


def extreme(wiki, code):
    if code is None or wiki is None:
        return False
    if code == 0 or wiki == 0:
        return code != wiki and max(code, wiki) > 5
    r = wiki / code
    return r > MAX_RATIO or r < 1 / MAX_RATIO


def main():
    index = json.load(open(os.path.join(ROOT, "data", "wiki_index.json"), encoding="utf-8"))
    classes = load_calamity_classes()
    force = set(json.load(open(FORCE, encoding="utf-8"))) if os.path.exists(FORCE) else set()

    items, npcs = {}, {}
    # Écarts extrêmes : appliqués seulement en mode Full Fandom (« uniquement ce qui est écrit »)
    full_items, full_npcs = {}, {}
    review, unverified = [], []

    for page in sorted(index.values(), key=lambda p: p["title"]):
        if page.get("status") not in ("official", "renamed") or not page.get("fields"):
            continue
        f = page["fields"]
        for match in page.get("calamity", []):
            cls = match["class"]
            if cls not in classes:
                continue
            path, kind = classes[cls]
            # Une infobox de PNJ ne s'applique qu'à un ModNPC, une infobox d'objet qu'à un ModItem
            box = page.get("infobox", "")
            if (kind == "ModItem") != box.startswith("item"):
                continue
            text = open(os.path.join(ROOT, path), encoding="utf-8-sig", errors="replace").read()
            forced = page["title"] in force

            if kind == "ModItem":
                code = item_code(text)
                wiki = {
                    "damage": number(f.get("damage")),
                    "useAnimation": number(f.get("use")),
                    "knockBack": number(f.get("knockback")),
                    "crit": (lambda c: None if c is None else max(0.0, c - 4))(number(f.get("critical"))),
                    "shootSpeed": number(f.get("velocity")),
                    "mana": number(f.get("mana")),
                    "defense": number(f.get("defense")),
                    "rare": number(f.get("rare")),
                }
                # Stats de combat seulement pour ce qui se manie (les blocs ont un « use » sans intérêt)
                combat = (page.get("type") or "").strip().lower() in ("weapon", "tool", "ammunition")
                if not combat:
                    for k in ("damage", "useAnimation", "knockBack", "crit", "shootSpeed", "mana"):
                        wiki[k] = None
                copper = sell_copper(f.get("sell"))
                if copper is not None:
                    wiki["value"] = copper * 5
                entry = {}
                for k, w in wiki.items():
                    if w is None:
                        continue
                    c = code.get(k)
                    if c is not None and abs(c - w) < 1e-6:
                        continue  # identique au code : rien à faire
                    if k in ("damage", "useAnimation", "mana", "defense") and extreme(w, c) and not forced:
                        review.append((page["title"], cls, k, w, c))
                        full_items.setdefault(cls, {"page": page["file"]})[k] = int(w)
                        continue
                    if c is None and k in ("damage", "useAnimation"):
                        unverified.append((page["title"], cls, k, w))
                    entry[k] = int(w) if k not in ("knockBack", "shootSpeed") else w
                if entry:
                    items[cls] = {"page": page["file"], **entry}

            elif kind == "ModNPC":
                code = npc_code(text)
                entry = {}
                for k in ("life", "damage"):
                    w, c = number(f.get(k)), code.get(k)
                    if w is None or c is None or c == 0 or abs(w - c) < 1e-6:
                        continue
                    if extreme(w, c) and not forced:
                        review.append((page["title"], cls, k, w, c))
                        full_npcs.setdefault(cls, {"page": page["file"]})[k + "Ratio"] = round(w / c, 4)
                        continue
                    entry[k + "Ratio"] = round(w / c, 4)
                w, c = number(f.get("defense")), code.get("defense")
                if w is not None and (c is None or abs(w - c) > 1e-6):
                    entry["defense"] = int(w)
                if entry:
                    npcs[cls] = {"page": page["file"], **entry}

    os.makedirs(os.path.dirname(OUT), exist_ok=True)
    with open(OUT, "w", encoding="utf-8") as fh:
        json.dump({"_generated_by": "tools/gen_stats.py", "items": items, "npcs": npcs,
                   "fullItems": full_items, "fullNpcs": full_npcs}, fh, indent=1, ensure_ascii=False)

    with open(REPORT, "w", encoding="utf-8") as fh:
        fh.write("# Stats du wiki appliquées\n\n_Généré par `tools/gen_stats.py`._\n\n")
        fh.write(f"- {len(items)} objets et {len(npcs)} PNJ de Calamity modifiés (`WikiData/stats.json`).\n")
        fh.write(f"- {len(review)} écarts extrêmes (> x{MAX_RATIO:g}) : appliqués **seulement en mode Full Fandom**, ci-dessous.\n")
        fh.write(f"- {len(unverified)} valeurs appliquées sans valeur littérale dans le code pour comparer.\n\n")
        fh.write("## Écarts extrêmes à trancher\n\nPour appliquer quand même : ajouter le titre de la page dans "
                 "`tools/stats_force.json`. Pour corriger le wiki : copier la page dans `wiki_edits/`.\n\n")
        fh.write("| Page | Classe | Champ | Wiki | Code | Ratio |\n|---|---|---|---|---|---|\n")
        for t, cls, k, w, c in review:
            ratio = f"x{w / c:.2f}" if c else "-"
            fh.write(f"| {t} | {cls} | {k} | {w:g} | {c:g} | {ratio} |\n")
        fh.write("\n## Appliqué sans comparaison possible\n\n| Page | Classe | Champ | Wiki |\n|---|---|---|---|\n")
        for t, cls, k, w in unverified:
            fh.write(f"| {t} | {cls} | {k} | {w:g} |\n")
    print(f"{len(items)} objets, {len(npcs)} PNJ ; {len(review)} écarts extrêmes en attente ; {len(unverified)} non vérifiés")


if __name__ == "__main__":
    main()
