"""Ennemis inventés par le wiki -> WikiData/enemies.json (+ sprites dans Assets/WikiEnemies/).

Chaque page « new » avec une infobox de PNJ de type Enemy, Flying Enemy ou Slime (et un sprite) devient un ennemi
instancié par Content/NPCs/WikiEnemy.cs, sans classe dédiée : vie, dégâts de contact, défense, résistance au recul,
argent, debuff au contact, IA vanilla la plus proche (slime, zombie, Demon Eye, chauve-souris), zone d'apparition
d'après « environment » et les drapeaux hardmode / postml. Le butin et les éléments viennent de loot.json et
elements.json par le nom. Les vers (Burrowing Enemy), boss et PNJ ne sont pas traités ici.

Usage : python tools/gen_enemies.py
"""
import hjson
import json
import os
import re
import sys

from PIL import Image, ImageSequence

sys.path.insert(0, os.path.dirname(__file__))
from wiki_index import ROOT, SPRITES  # noqa: E402
from gen_stats import number  # noqa: E402
from gen_items import internal_name, sprite_file, clean_text  # noqa: E402

ASSETS = os.path.join(ROOT, "Assets", "WikiEnemies")
OUT = os.path.join(ROOT, "WikiData", "enemies.json")
LOC = os.path.join(ROOT, "Localization", "en-US", "Mods.CalamityFandom.WikiEnemies.hjson")
TYPES = {"enemy": "fighter", "flying enemy": "flier", "slime": "slime"}

# Zones d'apparition : mots de « environment » -> clés lues par WikiEnemy.cs
ZONES = [
    ("brimstone", "crags"), ("sulphur", "sulphur"), ("abyss", "abyss"), ("sunken sea", "sunkensea"), ("astral", "astral"),
    ("corruption", "corruption"), ("crimson", "crimson"), ("hallow", "hallow"), ("jungle", "jungle"), ("desert", "desert"),
    ("snow", "snow"), ("tundra", "snow"), ("ice", "snow"), ("ocean", "ocean"), ("underworld", "hell"), ("hell", "hell"),
    ("dungeon", "dungeon"), ("space", "space"), ("sky", "space"), ("cavern", "cavern"), ("underground", "underground"),
    ("surface", "surface"), ("forest", "surface"), ("mushroom", "mushroom"), ("temple", "temple"), ("aether", "aether"),
]


def ai_of(kind, ai):
    """IA vanilla : slime, zombie (fighter), Demon Eye, chauve-souris, ou d'après le type."""
    ai = (ai or "").lower()
    if "slime" in ai:
        return "slime"
    if "bat" in ai:
        return "bat"
    if "eye" in ai or "flying" in ai or "hover" in ai:
        return "flier"
    if "fighter" in ai or "zombie" in ai or "walk" in ai:
        return "fighter"
    return TYPES[kind]


def zones_of(env):
    env = clean_text(env).lower()
    found = []
    for word, zone in ZONES:
        if word in env and zone not in found:
            found.append(zone)
    return found


def first_number(raw):
    """Premier nombre (mode normal du {{dv}}) d'un champ comme « {{dv|90|180}} (Contact)<br/>... »."""
    if not raw:
        return None
    raw = raw.split("<br")[0]
    m = re.search(r"\{\{dv\|([^|}]+)", raw)
    if m:
        raw = m.group(1)
    m = re.search(r"[\d,]+(\.\d+)?", raw.replace("{{element|", " "))
    return float(m.group(0).replace(",", "")) if m else None


def money_copper(raw):
    raw = raw or ""
    total = 0
    for tag, mult in (("pc", 1000000), ("gc", 10000), ("sc", 100), ("cc", 1)):
        for m in re.finditer(r"\{\{" + tag + r"\|([\d.]+)", raw):
            total += float(m.group(1)) * mult
    m = re.search(r"\{\{value\|([^}]*)\}\}", raw)
    if m:
        parts = [p.strip() for p in m.group(1).split("|")]
        for p, mult in zip(parts, (1000000, 10000, 100, 1)):
            if p:
                try:
                    total += float(p) * mult
                except ValueError:
                    pass
    return int(total)


def seconds(raw):
    m = re.search(r"([\d.]+)\s*(second|s\b|minute)", clean_text(raw or "").lower())
    if not m:
        return 0
    v = float(m.group(1))
    return int(v * 60 if m.group(2).startswith("minute") else v)


def convert(src, dest):
    """WebP/GIF -> planche PNG verticale ; renvoie (largeur, hauteur d'une image, nombre d'images)."""
    img = Image.open(src)
    frames = [f.convert("RGBA") for f in ImageSequence.Iterator(img)]
    unique = [frames[0]] + [f for prev, f in zip(frames, frames[1:]) if f.tobytes() != prev.tobytes()]
    w, h = unique[0].size
    sheet = Image.new("RGBA", (w, h * len(unique)), (0, 0, 0, 0))
    for i, f in enumerate(unique):
        sheet.paste(f, (0, i * h))
    sheet.save(dest, "PNG")
    return w, h, len(unique)


def main():
    index = json.load(open(os.path.join(ROOT, "data", "wiki_index.json"), encoding="utf-8"))
    pages = index if isinstance(index, dict) else {p["title"]: p for p in index}
    os.makedirs(ASSETS, exist_ok=True)
    used, enemies = set(), []
    for page in sorted(pages.values(), key=lambda p: p["title"]):
        if page.get("status") != "new" or page.get("kind") != "content" or not page.get("infobox", "").lower().startswith("npc"):
            continue
        if page.get("implemented") and page.get("implemented") != "WikiData/enemies.json":
            continue
        kind = (page.get("type") or "").strip().lower()
        if kind not in TYPES:
            continue
        f = page["fields"]
        life = first_number(f.get("life"))
        sprite = sprite_file(page)
        if not life or not sprite or not os.path.exists(sprite):
            continue
        if "{{historical" in open(os.path.join(ROOT, "wiki", "Main", page["file"]), encoding="utf-8", errors="replace").read().lower():
            continue
        name = internal_name(page["title"], used)
        w, h, frames = convert(sprite, os.path.join(ASSETS, name + ".png"))
        kb_value = first_number(f.get("knockback") or "")
        resist = 0.0
        if kb_value is not None:
            # « 100 » ou « 35% » : résistance au recul en %
            resist = min(1.0, kb_value / 100.0)
        text = clean_text(open(os.path.join(ROOT, "wiki", "Main", page["file"]), encoding="utf-8", errors="replace").read()).lower()
        enemies.append({
            "name": name,
            "displayName": page["title"],
            "page": page["file"],
            "width": w, "height": h, "frames": frames,
            "ai": ai_of(kind, f.get("ai")),
            "life": int(life),
            "damage": int(first_number(f.get("damage")) or 0),
            "defense": int(first_number(f.get("defense")) or 0),
            "knockBackResist": round(1.0 - resist, 3),
            "money": money_copper(f.get("money")),
            "debuff": clean_text(f.get("debuff") or "").replace("(debuff)", "").strip(),
            "debuffSeconds": seconds(f.get("duration")),
            "zones": zones_of(f.get("environment") or ""),
            "hardmode": (f.get("hardmode") or "").strip().lower() == "yes",
            "postml": (f.get("postml") or "").strip().lower() == "yes",
            "night": " at night" in text or "during the night" in text,
        })
        e = enemies[-1]
        # [choix] Pas d'apparition naturelle pour les ennemis manifestement mal classés : plus de 2000 PV sans « hardmode »,
        # plus de 30 000 PV sans « postml » (ennemis d'événements ou de boss inventés)
        if (e["life"] > 2000 and not e["hardmode"]) or (e["life"] > 30000 and not e["postml"]):
            e["zones"] = []
    json.dump({"_doc": "Généré par tools/gen_enemies.py depuis wiki/, ne pas éditer.", "enemies": enemies},
              open(OUT, "w", encoding="utf-8", newline="\n"), ensure_ascii=False, indent=1)
    loc = {e["name"]: {"DisplayName": e["displayName"]} for e in enemies}
    with open(LOC, "w", encoding="utf-8", newline="\n") as fh:
        fh.write(hjson.dumps(loc, indent="\t", ensure_ascii=False) + "\n")
    print(f"{len(enemies)} ennemis générés ({sum(1 for e in enemies if not e['zones'])} sans zone d'apparition)")


if __name__ == "__main__":
    main()
