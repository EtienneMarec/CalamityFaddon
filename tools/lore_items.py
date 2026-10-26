"""Objets de lore : nom du wiki de chaque objet de lore de Calamity.

Sur le wiki, chaque objet de lore a une page « X (Lore) » qui redirige vers la page de groupe « Lore items » (tableau
nom / texte / boss) ; les infobox de boss le citent sous ce titre (« The Desert Scourge (Lore) »). Le nom affiché est
celui du tableau (« The Desert Scourge »). Correspondance avec Calamity :
- par le nom actuel ou celui de 2.0.1.005, au « The » près (Desert Scourge, The Dragonfolly) ;
- par les [LegacyName] de Calamity (tools/calamity_renames.py) quand le lore a été réécrit sous un autre nom :
  LoreArchmage était KnowledgeCryogen (« Cryogen »), LoreRequiem KnowledgeMoonLord, LoreAzafure KnowledgeBrimstoneCrag...
Les titres « X (Lore) » absents du tableau (Old Duke) comptent aussi, avec le nom X.

Utilisé par gen_localization.py (nom du wiki) et gen_baseline.py (objets de lore inconnus du wiki : masqués en Full
Fandom). Usage : python tools/lore_items.py (affiche la correspondance).
"""
import json
import os
import re
import sys

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from wiki_index import CALAMITY, OUR_MAIN, ROOT  # noqa: E402

LORE_PAGE = os.path.join(OUR_MAIN, "Lore items.txt")
LORE_HJSON = os.path.join(CALAMITY, "Localization", "en-US", "Mods.CalamityMod.Items.Lore.hjson")


def simple(name):
    return re.sub(r"[^a-z0-9]", "", re.sub(r"^the\s+", "", name.strip().lower()))


def wiki_lore_names():
    """{titre « X (Lore) »: nom affiché} : tableau de la page « Lore items », puis redirections « X (Lore) »."""
    names = {}
    if os.path.exists(LORE_PAGE):
        text = open(LORE_PAGE, encoding="utf-8").read()
        for title, display in re.findall(r"\{\{item\|([^|}]+?\(Lore\))\|([^|}]+)\}?\}", text):
            names.setdefault(re.sub(r"\s+", " ", title).strip(), display.strip())
    index = json.load(open(os.path.join(ROOT, "data", "wiki_index.json"), encoding="utf-8"))
    for page in index.values():
        title = page["title"]
        if title.endswith(" (Lore)") and title not in names:
            names[title] = title[:-len(" (Lore)")]
    return names


def calamity_lore():
    """{classe: nom affiché actuel} des objets de lore de Calamity."""
    text = open(LORE_HJSON, encoding="utf-8-sig").read()
    return dict(re.findall(r"^(Lore\w+): \{\s*\n\s*DisplayName: (.+?)\s*$", text, re.M))


def lore_map():
    """{classe de Calamity: nom du wiki} ; les objets de lore absents du résultat sont inconnus du wiki."""
    from calamity_renames import load as load_renames
    current = calamity_lore()
    # Noms sous lesquels chaque classe a existé : actuel, 2.0.1.005 (renommage git ou [LegacyName]), nom de la classe
    # (LoreExoMechs : « The Exo Mechs (Lore) »)
    known_as = {cls: {name, cls[len("Lore"):]} for cls, name in current.items()}
    for r in load_renames():
        if r["new"] in known_as:
            known_as[r["new"]].add(r["oldName"])
    by_name = {}
    for cls, names in sorted(known_as.items()):
        for n in names:
            by_name.setdefault(simple(n), cls)
    result = {}
    # Le tableau d'abord : il donne le nom exact ; les redirections ensuite
    for title, display in wiki_lore_names().items():
        cls = by_name.get(simple(display))
        if cls and cls not in result:
            result[cls] = display
    return result


def main():
    current = calamity_lore()
    mapping = lore_map()
    for cls in sorted(current):
        wiki = mapping.get(cls)
        mark = "  " if wiki == current[cls] else ("->" if wiki else "xx")
        print(f"{mark} {cls:28} {current[cls]!r:34} {wiki!r}")
    unmatched = sorted(t for t, d in wiki_lore_names().items() if d not in mapping.values())
    print(f"\nPages du wiki sans objet dans Calamity ({len(unmatched)}) : {', '.join(unmatched)}")


if __name__ == "__main__":
    main()
