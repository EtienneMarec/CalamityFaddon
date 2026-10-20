"""Extrait CalamityMod.dll (+ .xml/.pdb si présents) du .tmod installé via le Workshop vers lib/.

Usage : python tools/extract_calamity_dll.py [chemin/vers/CalamityMod.tmod]
Sans argument, prend la version la plus récente dans le dossier Workshop de Steam.
"""
import io
import os
import struct
import sys
import zlib

WORKSHOP = r"C:\Program Files (x86)\Steam\steamapps\workshop\content\1281930\2824688072"
ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
WANTED = {"CalamityMod.dll", "CalamityMod.xml", "CalamityMod.pdb"}


def read_string(f):
    # BinaryWriter.Write(string) : longueur encodée en 7 bits puis UTF-8
    length, shift = 0, 0
    while True:
        b = f.read(1)[0]
        length |= (b & 0x7F) << shift
        shift += 7
        if not b & 0x80:
            break
    return f.read(length).decode("utf-8")


def latest_tmod():
    def key(v):
        return tuple(int(p) for p in v.split(".") if p.isdigit())
    versions = sorted((d for d in os.listdir(WORKSHOP) if os.path.isdir(os.path.join(WORKSHOP, d))), key=key)
    return os.path.join(WORKSHOP, versions[-1], "CalamityMod.tmod")


def main():
    path = sys.argv[1] if len(sys.argv) > 1 else latest_tmod()
    with open(path, "rb") as f:
        if f.read(4) != b"TMOD":
            sys.exit("Pas un fichier .tmod")
        tml_version = read_string(f)
        f.read(20 + 256)  # hash + signature
        f.read(4)  # longueur des données
        name = read_string(f)
        version = read_string(f)
        count = struct.unpack("<i", f.read(4))[0]
        entries = []
        for _ in range(count):
            fname = read_string(f)
            length, compressed = struct.unpack("<ii", f.read(8))
            entries.append((fname, length, compressed))
        out_dir = os.path.join(ROOT, "lib")
        os.makedirs(out_dir, exist_ok=True)
        for fname, length, compressed in entries:
            data = f.read(compressed)
            if fname not in WANTED:
                continue
            if compressed != length:
                data = zlib.decompress(data, -15)
            with open(os.path.join(out_dir, fname), "wb") as o:
                o.write(data)
            print(f"{fname} ({length} octets) -> lib/")
    print(f"{name} v{version} (tML {tml_version}) depuis {path}")


if __name__ == "__main__":
    main()
