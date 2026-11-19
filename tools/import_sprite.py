"""Convertit un sprite du dump Fandom (WebP déguisé en .png/.gif) en vrai PNG utilisable par tModLoader.

Usage :
  python tools/import_sprite.py "Zweihandromeda.png" Content/Items/Weapons/Melee/Zweihandromeda.png
  python tools/import_sprite.py "Graydron Slash.gif" Content/Projectiles/GraydronSlash.png --strip

Le premier argument est le nom du fichier dans fandom_sprites/ (espaces ou underscores acceptés).
--strip : pour une image animée, empile toutes les frames verticalement (format attendu par
          Main.projFrames / DrawAnimationVertical). Sans --strip, seule la première frame est gardée.
--scale N : réduit l'image d'un facteur entier (certains sprites du wiki sont agrandis x2).
--tile LxH : image « (placed) » d'un meuble de L × H cases : complétée (centrée en largeur, posée en bas) jusqu'à
          L*16 × H*16 puis découpée en cases de 16 px espacées de 2 px (planche de tuile de tModLoader).
"""
import argparse
import os
import sys

from PIL import Image, ImageSequence

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
SPRITES = os.path.join(ROOT, "fandom_sprites")


def find_sprite(name):
    # Sprites faits à la main (docs/SPRITES.md) prioritaires sur ceux du wiki
    for c in (name, os.path.splitext(name)[0].replace("_", " ") + ".png"):
        custom = os.path.join(ROOT, "wiki_sprites_custom", c)
        if os.path.isfile(custom):
            return custom
    candidates = [name, name.replace(" ", "_")]
    for c in candidates:
        p = os.path.join(SPRITES, c)
        if os.path.isfile(p):
            return p
    lowered = {f.lower(): f for f in os.listdir(SPRITES)}
    for c in candidates:
        if c.lower() in lowered:
            return os.path.join(SPRITES, lowered[c.lower()])
    sys.exit(f"Sprite introuvable : {name}")


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("sprite")
    ap.add_argument("dest")
    ap.add_argument("--strip", action="store_true")
    ap.add_argument("--scale", type=int, default=1)
    ap.add_argument("--tile", default=None)
    args = ap.parse_args()

    src = Image.open(find_sprite(args.sprite))
    frames = [f.convert("RGBA") for f in ImageSequence.Iterator(src)] if args.strip else [src.convert("RGBA")]
    w, h = frames[0].size
    out = Image.new("RGBA", (w, h * len(frames)), (0, 0, 0, 0))
    for i, f in enumerate(frames):
        out.paste(f, (0, i * h))
    if args.scale > 1:
        out = out.resize((out.width // args.scale, out.height // args.scale), Image.NEAREST)

    if args.tile:
        tw, th = (int(v) for v in args.tile.lower().split("x"))
        if out.width > tw * 16 or out.height > th * 16:
            sys.exit(f"Image {out.width}x{out.height} plus grande que {tw * 16}x{th * 16} : utiliser --scale ?")
        full = Image.new("RGBA", (tw * 16, th * 16), (0, 0, 0, 0))
        full.paste(out, ((tw * 16 - out.width) // 2, th * 16 - out.height))
        out = Image.new("RGBA", (tw * 18 - 2, th * 18 - 2), (0, 0, 0, 0))
        for x in range(tw):
            for y in range(th):
                out.paste(full.crop((x * 16, y * 16, x * 16 + 16, y * 16 + 16)), (x * 18, y * 18))

    dest = os.path.join(ROOT, args.dest)
    os.makedirs(os.path.dirname(dest), exist_ok=True)
    out.save(dest, "PNG")
    print(f"{args.dest} : {out.width}x{out.height}, {len(frames)} frame(s)")


if __name__ == "__main__":
    main()
