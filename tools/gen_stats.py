"""Génère WikiData/stats.json : stats des infobox du wiki à appliquer aux objets et PNJ de Calamity.

- Objets : valeurs absolues (dégâts, temps d'utilisation, recul, critique, vitesse, mana, défense, rareté, prix).
- PNJ : ratios wiki / code en mode normal pour la vie et les dégâts (la montée en difficulté de Calamity est
  conservée), défense absolue.
Seules les valeurs nettes sont reprises (« 42 », « 7.5 », « 19% », « {{dv|normal|expert|...}} », « {{value|...}} ») ;
les infobox à paliers (« Tier 1 / Tier 2 », « pre-Astrum Aureus ») sont ignorées.

Modes (décidé avec l'utilisateur le 21/10, voir docs/decisions.md) :
- valeur identique au code de Calamity 2.0.1.005 -> Full Fandom seulement (ce n'est que l'ancien Calamity) ;
- valeur propre au wiki -> tous les modes ;
- écart extrême (> x5) propre au wiki -> selon tools/stats_decisions.json (everywhere / fullOnly) ; s'il n'y figure
  pas encore, Full Fandom seulement en attendant de le trancher avec l'utilisateur ;
- pages listées dans « code » de stats_decisions.json : ignorées ici, gérées en C#.

Usage : python tools/gen_stats.py   (après wiki_index.py)
"""
import json
import os
import re
import sys

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from wiki_index import ROOT, load_calamity_classes  # noqa: E402
from gen_baseline import BASELINE, BASELINE_NAME, classes_at, sources_at  # noqa: E402

OUT = os.path.join(ROOT, "WikiData", "stats.json")
REPORT = os.path.join(ROOT, "reports", "stats.md")
DECISIONS = os.path.join(ROOT, "tools", "stats_decisions.json")
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
    decisions = json.load(open(DECISIONS, encoding="utf-8"))
    everywhere, full_only, by_code = (set(decisions.get(k, [])) for k in ("everywhere", "fullOnly", "code"))

    # Code de Calamity à l'abandon du wiki : une valeur du wiki identique n'est que « l'ancien Calamity »
    old_classes = classes_at(BASELINE)
    old_texts = sources_at(BASELINE, [path for _, path in old_classes.values()])

    items, npcs, full_items, full_npcs = {}, {}, {}, {}
    era, pending, decided, unverified = [], [], [], []

    def place(cls, page, k, value, target, layer_all, layer_full):
        layer = layer_full if target == "full" else layer_all
        layer.setdefault(cls, {"page": page["file"]})[k] = value

    for page in sorted(index.values(), key=lambda p: p["title"]):
        if page.get("status") not in ("official", "renamed") or not page.get("fields"):
            continue
        if page["title"] in by_code:
            continue  # comportement géré dans le code C# (ex. Ravager / Meatmetal Sans)
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
            old_text = old_texts.get(old_classes.get(cls, (None, None))[1])

            def target_for(k, w, c, o):
                """« full » (Full Fandom seulement) ou « all » (tous les modes) : voir docs/decisions.md."""
                if o is not None and abs(o - w) < 1e-6:
                    era.append((page["title"], cls, k, w, c))
                    return "full"
                if k in ("damage", "useAnimation", "mana", "defense", "life") and extreme(w, c):
                    if page["title"] in everywhere:
                        decided.append((page["title"], cls, k, w, c, "tous les modes"))
                        return "all"
                    if page["title"] in full_only:
                        decided.append((page["title"], cls, k, w, c, "Full Fandom seulement"))
                    else:
                        pending.append((page["title"], cls, k, w, c))
                    return "full"
                return "all"

            if kind == "ModItem":
                code = item_code(text)
                old = item_code(old_text) if old_text else {}
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
                for k, w in wiki.items():
                    if w is None:
                        continue
                    c = code.get(k)
                    if c is not None and abs(c - w) < 1e-6:
                        continue  # identique au code : rien à faire
                    if c is None and k in ("damage", "useAnimation"):
                        unverified.append((page["title"], cls, k, w))
                    value = int(w) if k not in ("knockBack", "shootSpeed") else w
                    place(cls, page, k, value, target_for(k, w, c, old.get(k)), items, full_items)

            elif kind == "ModNPC":
                code = npc_code(text)
                old = npc_code(old_text) if old_text else {}
                for k in ("life", "damage"):
                    w, c = number(f.get(k)), code.get(k)
                    if w is None or c is None or c == 0 or abs(w - c) < 1e-6:
                        continue
                    place(cls, page, k + "Ratio", round(w / c, 4), target_for(k, w, c, old.get(k)), npcs, full_npcs)
                w, c = number(f.get("defense")), code.get("defense")
                if w is not None and (c is None or abs(w - c) > 1e-6):
                    place(cls, page, "defense", int(w), target_for("defense", w, c, old.get("defense")), npcs, full_npcs)

    os.makedirs(os.path.dirname(OUT), exist_ok=True)
    with open(OUT, "w", encoding="utf-8") as fh:
        json.dump({"_generated_by": "tools/gen_stats.py", "items": items, "npcs": npcs,
                   "fullItems": full_items, "fullNpcs": full_npcs}, fh, indent=1, ensure_ascii=False)

    def fmt(v):
        return "-" if v is None else f"{v:g}"

    def table(rows, extra=None):
        head = "| Page | Classe | Champ | Wiki | Code actuel |" + (f" {extra} |" if extra else "")
        lines = [head, "|" + "---|" * (head.count("|") - 1)]
        for r in rows:
            t, cls, k, w, c = r[:5]
            lines.append(f"| {t} | {cls} | {k} | {fmt(w)} | {fmt(c)} |" + (f" {r[5]} |" if extra else ""))
        return "\n".join(lines) + "\n"

    with open(REPORT, "w", encoding="utf-8") as fh:
        fh.write("# Stats du wiki appliquées\n\n_Généré par `tools/gen_stats.py`._ Règles : `docs/decisions.md`, section « Stats ».\n\n")
        fh.write(f"- Tous les modes : {len(items)} objets et {len(npcs)} PNJ (`items` / `npcs` de `WikiData/stats.json`).\n")
        fh.write(f"- Full Fandom en plus : {len(full_items)} objets et {len(full_npcs)} PNJ (`fullItems` / `fullNpcs`).\n")
        fh.write(f"- {len(era)} valeurs identiques à Calamity {BASELINE_NAME} : Full Fandom seulement (ce n'est que l'ancien Calamity).\n")
        fh.write(f"- {len(decided)} écarts extrêmes tranchés avec l'utilisateur (`tools/stats_decisions.json`).\n")
        fh.write(f"- **{len(pending)} écarts extrêmes (> x{MAX_RATIO:g}) à trancher** : Full Fandom seulement en attendant.\n")
        fh.write(f"- Gérés dans le code : {', '.join(sorted(by_code)) or 'aucun'}.\n\n")
        fh.write("## Écarts extrêmes à trancher\n\n" + (table(pending) if pending else "Aucun.\n"))
        fh.write("\n## Écarts extrêmes tranchés\n\n" + table(decided, "Où"))
        fh.write(f"\n## Valeurs de Calamity {BASELINE_NAME} (Full Fandom seulement)\n\n" + table(era))
        fh.write("\n## Appliqué sans comparaison possible\n\n| Page | Classe | Champ | Wiki |\n|---|---|---|---|\n")
        for t, cls, k, w in unverified:
            fh.write(f"| {t} | {cls} | {k} | {fmt(w)} |\n")
    print(f"tous modes : {len(items)} objets, {len(npcs)} PNJ ; Full en plus : {len(full_items)} objets, {len(full_npcs)} PNJ ; "
          f"{len(era)} valeurs 2022 ; {len(decided)} extrêmes tranchés ; {len(pending)} à trancher")


if __name__ == "__main__":
    main()
