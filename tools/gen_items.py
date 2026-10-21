"""Génère les objets « simples » du wiki (matériaux d'artisanat) sans écrire de classe C# pour chacun.

Pour chaque page de matériau absente de Calamity :
  - sprite converti en PNG dans Assets/WikiItems/<Nom>.png (animé : frames empilées verticalement) ;
  - entrée dans WikiData/items.json (rareté, prix, pile, animation) ;
  - nom et infobulle dans Localization/en-US/Mods.CalamityFandom.WikiItems.hjson.
Content/Items/WikiMaterial.cs crée un objet par entrée au chargement du mod. Le système de recettes les trouve ensuite
par leur nom comme n'importe quel objet.

Usage : python tools/gen_items.py   (après wiki_index.py)
"""
import json
import os
import re
import sys

import hjson
from PIL import Image, ImageSequence

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from wiki_index import ROOT, SPRITES  # noqa: E402

CUSTOM = os.path.join(ROOT, "wiki_sprites_custom")
from gen_stats import number, sell_copper  # noqa: E402
from gen_localization import flatten_file  # noqa: E402

OUT = os.path.join(ROOT, "WikiData", "items.json")
ASSETS = os.path.join(ROOT, "Assets", "WikiItems")
LOC = os.path.join(ROOT, "Localization", "en-US", "Mods.CalamityFandom.WikiItems.hjson")
MATERIAL_TYPES = {"crafting material", "crafting materials", "bar", "soul"}
MAX_SPRITE = 96  # au-delà, réduction par un facteur entier (illustrations du wiki, ex. « Mars Bars »)


def internal_name(title, used):
    words = re.findall(r"[A-Za-z0-9]+", title.replace("'", ""))
    name = "".join(w[:1].upper() + w[1:] for w in words) or "Item"
    if name[0].isdigit():
        name = "W" + name
    base, n = name, 2
    while name.lower() in used:
        name = f"{base}{n}"
        n += 1
    used.add(name.lower())
    return name


def clean_text(raw):
    t = raw or ""
    t = re.sub(r"<!--.*?-->", "", t, flags=re.S)
    t = re.sub(r"<br\s*/?>", "\n", t, flags=re.I)
    t = re.sub(r"\{\{\s*color\s*\|([^|}]*)\|\s*#?([0-9A-Fa-f]{6})\s*\}\}", r"[c/\2:\1]", t)  # couleur conservée
    t = re.sub(r"\{\{\s*color\s*\|([^|}]*)\|[^}]*\}\}", r"\1", t)
    t = re.sub(r"\[\[(?:[^\]|]*\|)?([^\]]*)\]\]", r"\1", t)
    t = re.sub(r"\{\{[^}]*\}\}", "", t)
    t = re.sub(r"'''?", "", t)
    t = re.sub(r"<[^>]+>", "", t)
    return "\n".join(line.strip() for line in t.splitlines() if line.strip())


def sprite_file(page):
    """Chemin du sprite : d'abord wiki_sprites_custom/<titre>.png (sprites faits par l'utilisateur, voir
    docs/SPRITES.md), puis l'image indiquée par l'infobox, puis celle trouvée par wiki_index.py."""
    for title in (page["title"], re.sub(r"\s*\([^)]*\)$", "", page["title"])):
        custom = os.path.join(CUSTOM, title + ".png")
        if os.path.exists(custom):
            return custom
    image = (page["fields"].get("image") or "").strip()
    if image:
        candidate = image.replace("[[File:", "").split("|")[0].strip(" ]").replace(" ", "_")
        if os.path.exists(os.path.join(SPRITES, candidate)):
            return os.path.join(SPRITES, candidate)
    return os.path.join(SPRITES, page["sprite"]) if page.get("sprite") else None


def convert(src, dest):
    """WebP (fixe ou animé) -> PNG ; renvoie (largeur, hauteur d'une frame, nombre de frames, ticks par frame)."""
    img = Image.open(src)
    frames = [f.convert("RGBA") for f in ImageSequence.Iterator(img)]
    # Les frames identiques successives (GIF ralenti) ne servent à rien
    unique = [frames[0]] + [f for prev, f in zip(frames, frames[1:]) if f.tobytes() != prev.tobytes()]
    w, h = unique[0].size
    factor = 1
    while max(w, h) / factor > MAX_SPRITE:
        factor += 1
    if factor > 1:
        w, h = w // factor, h // factor
        unique = [f.resize((w, h), Image.NEAREST) for f in unique]
    sheet = Image.new("RGBA", (w, h * len(unique)), (0, 0, 0, 0))
    for i, f in enumerate(unique):
        sheet.paste(f, (0, i * h))
    sheet.save(dest, "PNG")
    duration = img.info.get("duration") or 80
    ticks = max(1, round(duration / (1000 / 60)))
    return w, h, len(unique), ticks


def main():
    index = json.load(open(os.path.join(ROOT, "data", "wiki_index.json"), encoding="utf-8"))
    os.makedirs(ASSETS, exist_ok=True)
    used = {"zweihandromeda", "stratusstar", "zweihandromedaswing", "wikimaterial"}
    # Les barres sont des « Block » sur le wiki (elles se posent) : prises comme matériaux si une recette les utilise
    recipes = json.load(open(os.path.join(ROOT, "WikiData", "recipes.json"), encoding="utf-8"))["recipes"]
    ingredients = {n.lower() for r in recipes for i in r["ingredients"] for n in i["items"]}
    items, loc = [], {}
    for page in sorted(index.values(), key=lambda p: p["title"]):
        if page.get("status") != "new" or page.get("kind") != "content" or not page.get("infobox", "").startswith("item"):
            continue
        kind = (page.get("type") or "").strip().lower().rstrip(".")
        if kind not in MATERIAL_TYPES and not (kind == "block" and page["title"].lower() in ingredients):
            continue
        sprite = sprite_file(page)
        if not sprite:
            continue
        f = page["fields"]
        title = re.sub(r"\s*\([^)]*\)$", "", page["title"])  # « Stardust (material) » -> « Stardust »
        name = internal_name(title, used)
        w, h, frames, ticks = convert(sprite, os.path.join(ASSETS, name + ".png"))
        rare = number(f.get("rare"))
        stack = number(f.get("stack"))
        copper = sell_copper(f.get("sell"))
        items.append({
            "name": name,
            "displayName": title,
            "page": page["file"],
            "width": w, "height": h, "frames": frames, "ticksPerFrame": ticks,
            "rare": int(rare) if rare is not None and -1 <= rare <= 17 else 0,
            "maxStack": int(min(max(stack or 9999, 1), 9999)),
            "value": copper * 5 if copper is not None else 0,
        })
        entry = {"DisplayName": title}
        tooltip = clean_text(f.get("tooltip"))
        entry["Tooltip"] = tooltip if tooltip else ""
        loc[name] = entry

    with open(OUT, "w", encoding="utf-8") as fh:
        json.dump({"_generated_by": "tools/gen_items.py", "materials": items}, fh, ensure_ascii=False, indent=1)
    # tModLoader reformate ce fichier à son chargement : on ne le réécrit que si le contenu change vraiment
    if not os.path.exists(LOC) or flatten_file(LOC) != {f"{k}.{f}": v for k, e in loc.items() for f, v in e.items()}:
        with open(LOC, "w", encoding="utf-8") as fh:
            fh.write(hjson.dumps(loc, indent="\t", ensure_ascii=False) + "\n")
    # Sprites d'objets qui ne sont plus générés
    names = {i["name"] + ".png" for i in items}
    for stale in set(os.listdir(ASSETS)) - names:
        os.remove(os.path.join(ASSETS, stale))
    print(f"{len(items)} matériaux générés ({sum(i['frames'] > 1 for i in items)} animés)")


if __name__ == "__main__":
    main()
