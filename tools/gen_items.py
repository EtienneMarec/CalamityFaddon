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
import shutil
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
# Aliments (24/10) : potions du wiki dont le buff est un buff de nourriture vanilla -> objet à manger généré
FOOD_BUFFS = {"well fed": "WellFed", "plenty satisfied": "WellFed2", "exquisitely stuffed": "WellFed3"}
DURATION_UNITS = {"second": 1, "minute": 60, "hour": 3600, "day": 86400}


def duration_seconds(raw):
    """« 4 minutes », « 4 hours », « 22 days » -> secondes (None si illisible)."""
    m = re.match(r"\s*([\d.]+)\s*(second|minute|hour|day)s?", raw or "", re.I)
    return int(float(m.group(1)) * DURATION_UNITS[m.group(2).lower()]) if m else None


MATERIAL_TYPES = {"crafting material", "crafting materials", "bar", "soul", "energy source"}  # Char Matter : énergie de Draedon
MAX_SPRITE = 96  # au-delà, réduction par un facteur entier (illustrations du wiki, ex. « Mars Bars »)
# Objets dessinés (et ramassables) en plus grand au sol, d'après une image du wiki (décision du 22/10, docs/decisions.md).
# Version détaillée facultative : wiki_sprites_custom/<titre> (world).png -> Assets/WikiItems/<Nom>_World.png
WORLD_SCALE = {
    "Yharim Bar": 16,  # « Yharimbaringame.png » (« The item in-game ») : ~480 px de large pour un sprite de 30
}


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


TILES = os.path.join(ROOT, "Assets", "WikiTiles")


def placed_sprite(title):
    """Image « <titre> (placed) » du wiki : 3 × 3 blocs posés (48 × 48), ou None."""
    path = os.path.join(SPRITES, title.replace(" ", "_") + "_(placed).png")
    return path if os.path.exists(path) else None


def build_tile(src, dest, bar):
    """Texture de tuile tirée du bloc du milieu de l'image « (placed) » (sans bordures : décision du 31/10, à affiner).
    Barre : une image 16 × 16 (tuile 1 × 1 comme les barres de métal). Bloc : planche standard 288 × 270 (cases de 18 px)
    dont chaque case reprend ce bloc. Renvoie la couleur moyenne (carte)."""
    img = Image.open(src)
    img = next(ImageSequence.Iterator(img)).convert("RGBA")
    tw, th = img.width // 3, img.height // 3
    center = img.crop((tw, th, 2 * tw, 2 * th)).resize((16, 16), Image.NEAREST)
    if bar:
        center.save(dest, "PNG")
    else:
        sheet = Image.new("RGBA", (288, 270), (0, 0, 0, 0))
        for x in range(16):
            for y in range(15):
                sheet.paste(center, (x * 18, y * 18))
        sheet.save(dest, "PNG")
    pixels = [p for p in center.getdata() if p[3] > 128] or [(128, 128, 128, 255)]
    return [sum(p[i] for p in pixels) // len(pixels) for i in range(3)]


def convert_world(src, dest, size):
    """Version géante dessinée au sol (wiki_sprites_custom/<titre> (world).png, détourée par l'utilisateur) : recadrée
    sur sa partie visible et mise à la taille de la hitbox au sol."""
    img = Image.open(src).convert("RGBA")
    img = img.crop(img.getchannel("A").getbbox())
    img.resize(size, Image.LANCZOS).save(dest, "PNG")


def main():
    index = json.load(open(os.path.join(ROOT, "data", "wiki_index.json"), encoding="utf-8"))
    os.makedirs(ASSETS, exist_ok=True)
    shutil.rmtree(TILES, ignore_errors=True)
    os.makedirs(TILES)
    used ={"zweihandromeda", "stratusstar", "zweihandromedaswing", "wikimaterial"}
    # Les barres sont des « Block » sur le wiki (elles se posent) : prises comme matériaux si une recette les utilise
    recipes = json.load(open(os.path.join(ROOT, "WikiData", "recipes.json"), encoding="utf-8"))["recipes"]
    ingredients = {n.lower() for r in recipes for i in r["ingredients"] for n in i["items"]}
    items, loc = [], {}
    for page in sorted(index.values(), key=lambda p: p["title"]):
        if page.get("status") != "new" or page.get("kind") != "content" or not page.get("infobox", "").startswith("item"):
            continue
        kind = (page.get("type") or "").strip().lower().rstrip(".")
        food = FOOD_BUFFS.get(clean_text(page["fields"].get("buff")).lower()) if kind == "potion" else None
        seconds = duration_seconds(page["fields"].get("duration")) if food else None
        if food and not seconds:
            food = None
        if kind not in MATERIAL_TYPES and not food and not (kind == "block" and page["title"].lower() in ingredients):
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
        # Bloc ou barre qui se pose (« Block » sur le wiki) : tuile générée si le wiki montre le bloc posé
        types = f"{kind} {(f.get('type2') or '').lower()}"
        placed = placed_sprite(title) if "block" in types else None
        if placed:
            bar = "bar" in types or title.lower().endswith(" bar")
            items[-1]["tile"] = "bar" if bar else "block"
            items[-1]["mapColor"] = build_tile(placed, os.path.join(TILES, name + ".png"), bar)
        if food:
            use = number(f.get("use"))
            items[-1].update({"foodBuff": food, "foodSeconds": seconds, "useTime": int(use) if use else 17})
        if title in WORLD_SCALE:
            items[-1]["worldScale"] = WORLD_SCALE[title]
            world = os.path.join(CUSTOM, title + " (world).png")
            if os.path.exists(world):
                scale = WORLD_SCALE[title]
                convert_world(world, os.path.join(ASSETS, name + "_World.png"), (w * scale, h * scale))
                items[-1]["worldTexture"] = True
        entry = {"DisplayName": title}
        tooltip = clean_text(f.get("tooltip"))
        if food and tooltip:
            # Terraria ajoute lui-même cette ligne aux aliments en Expert (le wiki l'a recopiée d'une capture Expert)
            tooltip = "\n".join(l for l in tooltip.split("\n") if l.strip().lower() != "increases life regeneration")
        entry["Tooltip"] = tooltip if tooltip else ""
        loc[name] = entry

    wings = build_wings(index, used, loc)

    with open(OUT, "w", encoding="utf-8") as fh:
        json.dump({"_generated_by": "tools/gen_items.py", "materials": items, "wings": wings}, fh, ensure_ascii=False, indent=1)
    # tModLoader reformate ce fichier à son chargement : on ne le réécrit que si le contenu change vraiment
    if not os.path.exists(LOC) or flatten_file(LOC) != {f"{k}.{f}": v for k, e in loc.items() for f, v in e.items()}:
        with open(LOC, "w", encoding="utf-8") as fh:
            fh.write(hjson.dumps(loc, indent="\t", ensure_ascii=False) + "\n")
    # Sprites d'objets qui ne sont plus générés
    names = {i["name"] + ".png" for i in items} | {i["name"] + "_World.png" for i in items if i.get("worldTexture")}
    names |= {w["name"] + ".png" for w in wings} | {w["name"] + "_Wings.png" for w in wings}
    for stale in set(os.listdir(ASSETS)) - names:
        os.remove(os.path.join(ASSETS, stale))
    print(f"{len(items)} matériaux générés ({sum(i['frames'] > 1 for i in items)} animés, {sum(1 for i in items if i.get('tile'))} posables), {len(wings)} ailes "
          f"({sum(1 for w in wings if not w['equipSprite'])} sans image portée)")


# Ailes (24/10) : coverings du wiki dont l'infobulle donne les stats de vol. Vitesse verticale : mots du wiki traduits en
# réglages de VerticalWingSpeeds dans Content/Items/WikiWings.cs ([choix], docs/decisions.md).
WING_STAT = {
    "horizontal": r"horizontal speed:\s*([\d.]+)",
    "acceleration": r"acceleration multiplier:\s*([\d.]+)",
    "flight": r"flight time:\s*([\d.]+|\{\{infinity\}\})",
    "vertical": r"(average|good|great|excellent) vertical speed",
}
# Lignes de stats en plus, traduites quand elles sont simples ; les autres restent dans « todo »
WING_EXTRAS = [
    (r"^\+?(\d+)% (?:increased )?movement speed$", "moveSpeed"),
    (r"^(\d+)% increased jump speed$", "jumpSpeed"),
    (r"^(\d+)% increased damage$", "damage"),
    (r"^(\d+) defense$", "defense"),
]


# Ailes portées : les images « (equipped) » du wiki montrent le personnage avec ses ailes. On l'efface (sa palette, autour
# de sa position trouvée par comparaison avec un personnage de référence) et on recentre l'image sur le point où Terraria
# accroche les ailes : 9 px derrière le centre du joueur, 19 px au-dessus du sol (PlayerDrawLayers.DrawPlayer_09_Wings).
_player_ref = None


def player_reference():
    """Personnage seul (30×48) : pixel le plus fréquent des images « (equipped) » de cette taille, et sa palette."""
    global _player_ref
    if _player_ref is None:
        import collections
        import glob
        images = []
        for path in glob.glob(os.path.join(SPRITES, "*_(equipped).png")):
            try:
                images.append(Image.open(path).convert("RGBA"))
            except OSError:
                continue
        same = [im for im in images if im.size == (30, 48)]
        ref = Image.new("RGBA", (30, 48), (0, 0, 0, 0))
        for y in range(48):
            for x in range(30):
                color, count = collections.Counter(im.getpixel((x, y)) for im in same).most_common(1)[0]
                if count >= len(same) // 2:
                    ref.putpixel((x, y), color)
        palette = {c for c in ref.getdata() if c[3]}
        seen = collections.Counter()
        for im in images:
            seen.update({c for c in im.getdata() if c[3]})
        palette |= {c for c, k in seen.items() if k >= 15}
        _player_ref = (ref, palette)
    return _player_ref


# Point d'accroche des ailes dans l'image de référence : centre du joueur x = 16, pieds sur la ligne 47
WING_ANCHOR = (16 - 9, 48 - 19)


def strip_player(frame):
    """Efface le personnage d'une image d'ailes portées et la recentre sur le point d'accroche des ailes."""
    ref, palette = player_reference()
    R, P = ref.load(), frame.load()
    W, H = frame.size
    best = (-1, 0, 0)
    for dy in range(-10, H - 38):
        for dx in range(-10, W - 20):
            match = sum(1 for y in range(0, 48, 2) for x in range(0, 30, 2)
                        if R[x, y][3] and 0 <= x + dx < W and 0 <= y + dy < H and P[x + dx, y + dy] == R[x, y])
            if match > best[0]:
                best = (match, dx, dy)
    _, dx, dy = best
    out = frame.copy()
    O = out.load()
    for y in range(max(0, dy - 4), min(H, dy + 52)):
        for x in range(max(0, dx - 4), min(W, dx + 34)):
            if P[x, y][3] and P[x, y] in palette:
                O[x, y] = (0, 0, 0, 0)
    cx, cy = dx + WING_ANCHOR[0], dy + WING_ANCHOR[1]
    half_w, half_h = max(cx, W - cx), max(cy, H - cy)
    centered = Image.new("RGBA", (2 * half_w, 2 * half_h), (0, 0, 0, 0))
    centered.paste(out, (half_w - cx, half_h - cy))
    return centered


def build_wings(index, used, loc):
    wings = []
    for page in sorted(index.values(), key=lambda p: p["title"]):
        f = page.get("fields") or {}
        tooltip = f.get("tooltip") or ""
        if page.get("status") != "new" or not page.get("infobox", "").startswith("item") or "flight time" not in tooltip.lower():
            continue
        stats = {k: re.search(p, tooltip, re.I) for k, p in WING_STAT.items()}
        if not (stats["horizontal"] and stats["acceleration"] and stats["flight"]):
            continue
        sprite = sprite_file(page)
        if not sprite:
            continue
        title = page["title"]
        name = internal_name(title, used)
        w, h, _, _ = convert(sprite, os.path.join(ASSETS, name + ".png"))
        # Image portée (une seule pose sur le wiki, personnage effacé) : planche de 4 images identiques, comme les ailes de Terraria
        base = os.path.splitext(os.path.basename(sprite))[0]
        equipped = os.path.join(SPRITES, base + "_(equipped).png")
        equip_sprite = os.path.exists(equipped)
        frame = strip_player(Image.open(equipped).convert("RGBA")) if equip_sprite else Image.new("RGBA", (2, 2), (0, 0, 0, 0))
        sheet = Image.new("RGBA", (frame.width, frame.height * 4), (0, 0, 0, 0))
        for i in range(4):
            sheet.paste(frame, (0, i * frame.height))
        sheet.save(os.path.join(ASSETS, name + "_Wings.png"), "PNG")
        flight = stats["flight"].group(1)
        extras, todo = [], []
        for line in re.split(r"<br\s*/?>", tooltip):
            line = clean_text(line).strip()
            low = line.lower()
            if not line or any(re.search(p, line, re.I) for p in WING_STAT.values()):
                continue
            for pattern, kind in WING_EXTRAS:
                m = re.match(pattern, low)
                if m:
                    extras.append({"type": kind, "value": int(m.group(1))})
                    break
            else:
                todo.append(line)
        rare, copper = number(f.get("rare")), sell_copper(f.get("sell"))
        wings.append({
            "name": name, "displayName": title, "page": page["file"], "width": w, "height": h,
            "rare": int(rare) if rare is not None and -1 <= rare <= 17 else 0,
            "value": copper * 5 if copper is not None else 0,
            "horizontal": float(stats["horizontal"].group(1)), "acceleration": float(stats["acceleration"].group(1)),
            "flightTime": -1 if "infinity" in flight else int(float(flight)),
            "vertical": stats["vertical"].group(1).lower() if stats["vertical"] else "average",
            "extras": extras, "todo": todo, "equipSprite": equip_sprite,
        })
        loc[name] = {"DisplayName": title, "Tooltip": clean_text(tooltip) or ""}
    return wings


if __name__ == "__main__":
    main()
