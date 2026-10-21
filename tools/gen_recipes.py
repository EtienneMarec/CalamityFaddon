"""Extrait les recettes du wiki (pages Recipes_<station>_register) vers WikiData/recipes.json.

Format d'une recette dans le wikitexte :
  {{recipes/register | station = Mythril Anvil | result = Soul Flux | amount = 1 | Soul of Light | 5 | ... }}
Les paramètres sans nom vont par paires (ingrédient, quantité). « A/B » = au choix (groupe de recettes).
L'application en jeu (Common/WikiData/WikiRecipes.cs) résout les noms d'objets et de stations par leur nom anglais.

Usage : python tools/gen_recipes.py
"""
import collections
import glob
import json
import os
import re
import sys

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from wiki_index import OUR_MAIN, ROOT, extract_template, has_mode, pick_mode  # noqa: E402

OUT = os.path.join(ROOT, "WikiData", "recipes.json")
# Pages qui ne décrivent pas de vraies recettes d'artisanat
SKIP_PAGES = {"Recipes_Historical_register.txt"}


def split_all(body):
    """Paramètres d'un modèle : (positionnels, nommés), en ignorant les | imbriqués."""
    parts, depth, cur, i = [], 0, [], 0
    while i < len(body):
        two = body[i:i + 2]
        if two in ("{{", "[["):
            depth += 1
            cur.append(two)
            i += 2
            continue
        if two in ("}}", "]]"):
            depth -= 1
            cur.append(two)
            i += 2
            continue
        if body[i] == "|" and depth == 0:
            parts.append("".join(cur))
            cur = []
        else:
            cur.append(body[i])
        i += 1
    parts.append("".join(cur))
    positional, named = [], {}
    for p in parts[1:]:
        p = re.sub(r"<!--.*?-->", "", p, flags=re.S).strip()
        m = re.match(r"^([a-zA-Z][a-zA-Z0-9 _]*?)\s*=\s*(.*)$", p, re.S)
        if m:
            named[m.group(1).strip().lower()] = m.group(2).strip()
        elif p:
            positional.append(p)
    return positional, named


def clean_name(name):
    name = re.sub(r"\[\[(?:[^\]|]*\|)?([^\]]*)\]\]", r"\1", name)  # [[lien|texte]] -> texte
    name = re.sub(r"\{\{[^}]*\}\}", "", name)  # modèles décoratifs
    return re.sub(r"\s+", " ", name).strip()


def parse_qty(raw):
    m = re.match(r"^\s*(\d+)", raw or "")
    return int(m.group(1)) if m else None


def redirect_aliases(names):
    index = json.load(open(os.path.join(ROOT, "data", "wiki_index.json"), encoding="utf-8"))
    norm = lambda t: re.sub(r"\s+", " ", t.replace("_", " ")).strip().lower()
    redirects = {norm(p["title"]): p["target"].split("#")[0].strip() for p in index.values() if p["kind"] == "redirect"}
    aliases = {}
    for name in sorted(names):
        target, hops = name, 0
        while norm(target) in redirects and hops < 4:
            target = redirects[norm(target)]
            hops += 1
        target = re.sub(r"^(?:tgc|w:c:terraria):", "", target, flags=re.I)  # lien vers le wiki Terraria officiel
        if hops and norm(target) != norm(name):
            aliases[name] = target
    manual = json.load(open(os.path.join(ROOT, "tools", "glossary.json"), encoding="utf-8")).get("aliases", {})
    aliases.update({k: v for k, v in manual.items() if k in names and k != v})
    return aliases


MODES = ("fandom", "plus")


def build_recipe(positional, named, fname, problems):
    """Recette du wiki à partir des paramètres d'un {{recipes/register}}, ou None (vide, vandalisée)."""
    result = clean_name(named.get("result", ""))
    if not result:
        return None
    # Une paire (ingrédient, quantité) vide des deux côtés = absente dans ce mode ({{mode|Objet|}} | {{mode|1|}})
    pairs = [(n, q) for n, q in zip(positional[0::2], positional[1::2]) if n.strip() or q.strip()]
    if len(positional) % 2:
        problems.append((fname, result, "nombre impair de paramètres"))
    ingredients = []
    for name, qty in pairs:
        q = parse_qty(qty)
        alternatives = [clean_name(n) for n in name.split("/") if clean_name(n)]
        if q is None or q <= 0 or not alternatives:
            problems.append((fname, result, f"ingrédient illisible : {name} | {qty}"))
            return None  # recette vandalisée (ex. « sqrt(-1) » Alloy Bars) : on garde celle du jeu
        ingredients.append({"items": alternatives, "stack": q})
    stations = [clean_name(st) for st in re.split(r"\s*(?:/|<br\s*/?>|\band\b)\s*", named.get("station", "")) if clean_name(st)]
    recipe = {
        "result": result,
        "amount": parse_qty(named.get("amount")) or 1,
        "stations": stations,
        "ingredients": ingredients,
        "page": fname,
    }
    extra = {k: v for k, v in named.items() if k not in ("station", "result", "amount")}
    if extra:
        recipe["extra"] = extra
    return recipe


def recipes_by_mode(positional, named, fname, problems):
    """Recettes d'un {{recipes/register}} : une seule si la page est la même dans les deux modes, sinon une par mode
    (marquée « modes ») quand un paramètre contient {{mode|Full Fandom|Calamity++}}."""
    if not any(has_mode(v) for v in positional + list(named.values())):
        recipe = build_recipe(positional, named, fname, problems)
        return [recipe] if recipe else []
    views = {mode: build_recipe([pick_mode(v, mode) for v in positional],
                                {k: pick_mode(v, mode) for k, v in named.items()}, fname, problems)
             for mode in MODES}
    if views["fandom"] == views["plus"]:
        return [views["plus"]] if views["plus"] else []
    return [{**recipe, "modes": [mode]} for mode, recipe in views.items() if recipe]


def main():
    pages = {os.path.basename(p): p for p in glob.glob(os.path.join(OUR_MAIN, "Recipes_*egister.txt"))}

    recipes, problems = [], []
    named_keys = collections.Counter()
    for fname in sorted(pages):
        if fname in SKIP_PAGES:
            continue
        text = open(pages[fname], encoding="utf-8", errors="replace").read()
        for m in re.finditer(r"\{\{\s*[Rr]ecipes/register\s*\|", text):
            positional, named = split_all(extract_template(text, m.start()))
            named_keys.update(named.keys())
            recipes += recipes_by_mode(positional, named, fname, problems)

    # Transmutations du Shimmer : ce ne sont pas des recettes d'artisanat
    shimmer = [{"from": r["ingredients"][0]["items"][0], "to": r["result"], "station": r["stations"][0], "page": r["page"],
                **({"modes": r["modes"]} if "modes" in r else {})}
               for r in recipes if any("shimmer" in st.lower() for st in r["stations"]) and r["ingredients"]]
    recipes = [r for r in recipes if not any("shimmer" in st.lower() for st in r["stations"])]

    # Alias : le wiki utilise parfois d'anciens noms (« Sea Remains ») qui redirigent vers la page actuelle.
    # Le jeu essaie d'abord le nom tel quel, puis la cible de la redirection.
    aliases = redirect_aliases(
        {r["result"] for r in recipes}
        | {n for r in recipes for i in r["ingredients"] for n in i["items"]}
        | {st for r in recipes for st in r["stations"]}
        | {x for s in shimmer for x in (s["from"], s["to"])})

    os.makedirs(os.path.dirname(OUT), exist_ok=True)
    with open(OUT, "w", encoding="utf-8") as f:
        json.dump({"_generated_by": "tools/gen_recipes.py", "recipes": recipes, "shimmer": shimmer, "aliases": aliases},
                  f, ensure_ascii=False, indent=1)
    print(f"{len(recipes)} recettes, {len(shimmer)} transmutations Shimmer, {len(problems)} problèmes de lecture")
    print("paramètres nommés :", dict(named_keys))
    print("stations :", collections.Counter(s for r in recipes for s in r["stations"]).most_common())
    for p in problems[:15]:
        print("  ", p)


if __name__ == "__main__":
    main()
