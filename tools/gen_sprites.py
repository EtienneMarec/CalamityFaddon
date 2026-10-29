"""Sprites du wiki pour les objets de Calamity dont l'image diffère (décision du 22/10).

- Objet renommé par le wiki (Graydron, Abyss Bar...) : sprite du wiki dans tous les modes, comme son nom
  -> Assets/Overrides/Generated/Items/<Classe>.png
- Objet officiel dont Calamity a refait le sprite depuis 2022 : sprite du wiki en Full Fandom seulement
  -> Assets/Overrides/Generated/Full/Items/<Classe>.png
- Objet porté ou créé par ce mod (page implémentée dans Content/) dont le wiki montre un autre sprite (resprite du
  Fandom) : sprite du wiki dans tous les modes -> Assets/Overrides/Generated/Items/ (décision du 30/10)
Appliqués par Common/WikiData/TextureOverrides.cs (icône seulement). Ignorés : objets animés de Calamity (planche
d'images), murs (le wiki montre le mur posé), illustrations de plus de 96 px.
Une image faite à la main dans Assets/Overrides/Items/ passe avant.

Les images du wiki sont du WebP parfois compressé avec perte, ou agrandies x2 : une image « presque identique » ou
identique une fois réduite compte comme la même.

Usage : python tools/gen_sprites.py   (après wiki_index.py)
"""
import json
import os
import re
import shutil
import sys

from PIL import Image, ImageChops, ImageSequence, ImageStat

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from wiki_index import ROOT, SPRITES  # noqa: E402

OUT_ALL = os.path.join(ROOT, "Assets", "Overrides", "Generated", "Items")
OUT_FULL = os.path.join(ROOT, "Assets", "Overrides", "Generated", "Full", "Items")
REPORT = os.path.join(ROOT, "reports", "sprites.md")
MAX_SPRITE = 96  # au-delà, c'est une illustration du wiki, pas une icône d'objet
BIG_DIFF = 48    # écart d'un canal (0-255) au-delà duquel un pixel est « nettement différent » (la compression WebP reste en dessous)


def ModClassClash(cls):
    """Une classe de Calamity du même nom recevrait l'image à notre place (TextureOverrides cherche d'abord Calamity)."""
    return os.path.exists(os.path.join(ROOT, "OtherMods", "CalamityModPublic", "Items")) and any(
        f == cls + ".cs" for _, _, files in os.walk(os.path.join(ROOT, "OtherMods", "CalamityModPublic", "Items")) for f in files)


def first_frame(path):
    img = Image.open(path)
    return next(ImageSequence.Iterator(img)).convert("RGBA")


def close(a, b):
    """Même image au bruit de compression près : même forme, et moins de 3 % des pixels visibles nettement différents.
    (Corrigé le 30/10 : getbbox() d'une image RGBA ne regarde que la transparence, et l'écart moyen sur toute l'image,
    fond transparent compris, laissait passer des recolorations entières comme celle du Blood Clot Staff.)"""
    if a.size != b.size:
        return False
    pa, pb = a.getdata(), b.getdata()
    shape = sum(1 for p, q in zip(pa, pb) if (p[3] > 128) != (q[3] > 128))
    both = [(p, q) for p, q in zip(pa, pb) if p[3] > 128 and q[3] > 128]
    if shape > 0.02 * a.width * a.height:
        return False
    big = sum(1 for p, q in both if max(abs(p[0] - q[0]), abs(p[1] - q[1]), abs(p[2] - q[2])) > BIG_DIFF)
    return big <= 0.03 * max(1, len(both))


def same_sprite(wiki, code):
    """Le sprite du wiki est-il celui du code (au besoin agrandi, ou première image d'une planche animée) ?"""
    candidates = [code]
    # Planche verticale (objet animé) : la première image
    if code.height > code.width and wiki.width == code.width and code.height % wiki.height == 0:
        candidates.append(code.crop((0, 0, code.width, wiki.height)))
    for c in candidates:
        if close(wiki, c):
            return True
        for k in (2, 3):
            if wiki.size == (c.width * k, c.height * k) and close(wiki.resize(c.size, Image.NEAREST), c):
                return True
    return False


def usable(wiki):
    """Ramène une image du wiki agrandie à sa taille d'icône ; None si c'est une illustration."""
    for k in (1, 2, 3, 4):
        w, h = wiki.width // k, wiki.height // k
        if wiki.width % k or wiki.height % k:
            continue
        small = wiki.resize((w, h), Image.NEAREST)
        # Agrandissement exact : réagrandir la version réduite redonne l'image
        if k > 1 and not close(small.resize(wiki.size, Image.NEAREST), wiki):
            continue
        if max(w, h) <= MAX_SPRITE:
            return small
    return None


def main():
    index = json.load(open(os.path.join(ROOT, "data", "wiki_index.json"), encoding="utf-8"))
    for folder in (OUT_ALL, OUT_FULL):
        shutil.rmtree(folder, ignore_errors=True)
        os.makedirs(folder)

    written = {"renamed": [], "official": []}
    same, skipped = 0, []
    done = set()
    for page in sorted(index.values(), key=lambda p: p["title"]):
        status = page.get("status")
        if page.get("kind") != "content" or status not in ("official", "renamed") or not page.get("sprite"):
            continue
        if not page.get("infobox", "").startswith("item"):
            continue
        for match in page.get("calamity", []):
            cls, file = match["class"], match.get("file")
            if not match["category"].startswith("Items") or not file or cls in done:
                continue
            png = os.path.join(ROOT, os.path.splitext(file)[0] + ".png")
            if not os.path.exists(png):
                continue
            done.add(cls)
            try:
                wiki = first_frame(os.path.join(SPRITES, page["sprite"]))
                code = Image.open(png).convert("RGBA")
            except OSError:
                skipped.append((page["title"], "image illisible"))
                continue
            if same_sprite(wiki, code):
                same += 1
                continue
            # Objet animé (planche de plusieurs images) : l'image du wiki n'en montre qu'une, elle casserait l'animation
            source = open(os.path.join(ROOT, file), encoding="utf-8-sig", errors="replace").read()
            if "RegisterItemAnimation" in source or "AnimatesAsSoul" in source:
                skipped.append((page["title"], "objet animé"))
                continue
            # Murs : le wiki montre souvent le mur posé, pas l'icône de l'objet
            if "wall" in (page.get("type") or "").lower() or page["title"].lower().endswith(" wall"):
                skipped.append((page["title"], "mur (image du mur posé)"))
                continue
            icon = usable(wiki)
            if icon is None:
                skipped.append((page["title"], f"illustration {wiki.width}x{wiki.height}"))
                continue
            folder = OUT_ALL if status == "renamed" else OUT_FULL
            icon.save(os.path.join(folder, cls + ".png"), "PNG")
            written[status].append((page["title"], cls))

    # Objets de ce mod : classe portant « /// Wiki : <page> », image à côté de son fichier
    written["ours"] = []
    for page in sorted(index.values(), key=lambda p: p["title"]):
        impl = page.get("implemented") or ""
        # Pages « new », et pages renommées dont notre portage est la version Full Fandom (Starfall, Seabound Staff...)
        if page.get("kind") != "content" or not page.get("sprite"):
            continue
        if not impl.startswith("Content/") or not impl.endswith(".cs") or not page.get("infobox", "").startswith("item"):
            continue
        source = open(os.path.join(ROOT, impl), encoding="utf-8-sig", errors="replace").read()
        m = re.search(r"Wiki : " + re.escape(page["file"]) + r".*?\bclass (\w+)", source, re.S)
        if not m:
            continue
        cls = m.group(1)
        if cls in done or ModClassClash(cls):
            continue
        png = os.path.join(ROOT, os.path.dirname(impl), cls + ".png")
        if not os.path.exists(png):
            continue
        done.add(cls)
        try:
            wiki = first_frame(os.path.join(SPRITES, page["sprite"]))
            code = Image.open(png).convert("RGBA")
        except OSError:
            skipped.append((page["title"], "image illisible"))
            continue
        if same_sprite(wiki, code):
            same += 1
            continue
        if "RegisterItemAnimation" in source or "AnimatesAsSoul" in source:
            skipped.append((page["title"], "objet animé"))
            continue
        icon = usable(wiki)
        if icon is None:
            skipped.append((page["title"], f"illustration {wiki.width}x{wiki.height}"))
            continue
        icon.save(os.path.join(OUT_ALL, cls + ".png"), "PNG")
        written["ours"].append((page["title"], cls))

    with open(REPORT, "w", encoding="utf-8") as f:
        f.write("# Sprites du wiki appliqués aux objets de Calamity\n\n_Généré par `tools/gen_sprites.py`._\n\n")
        f.write(f"- {same} objets ont déjà le sprite du wiki\n")
        f.write(f"- {len(written['renamed'])} objets renommés : sprite du wiki dans tous les modes\n")
        f.write(f"- {len(written['official'])} objets officiels : sprite du wiki en Full Fandom seulement\n")
        f.write(f"- {len(written['ours'])} objets de ce mod (portages, créations) : sprite du wiki dans tous les modes\n")
        f.write(f"- {len(skipped)} ignorés (image du wiki inutilisable)\n\n")
        for status, title in (("renamed", "Renommés (tous les modes)"), ("official", "Officiels (Full Fandom)"),
                              ("ours", "Objets de ce mod (tous les modes)")):
            f.write(f"## {title}\n\n" + ", ".join(f"{t} (`{c}`)" for t, c in written[status]) + "\n\n")
        f.write("## Ignorés\n\n" + "\n".join(f"- {t} : {why}" for t, why in skipped) + "\n")
    print(f"déjà identiques : {same} ; renommés : {len(written['renamed'])} ; officiels (Full) : {len(written['official'])} ; "
          f"ignorés : {len(skipped)}")


if __name__ == "__main__":
    main()
