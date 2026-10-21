"""Crée wiki/ : notre copie du wiki Fandom, qui deviendra le wiki du mod (décision du 21/10, docs/WIKI.md).

À lancer une seule fois : refuse si wiki/ existe déjà. La copie est exacte (sauf les 18 pages que Windows avait
écrasées dans le dump, reprises de fandom_dump_extra/Main/) ; elle est commitée telle quelle, pour que `git diff`
montre ensuite tout ce qu'on a modifié par rapport au Fandom. fandom_dump_txt/ reste la référence intacte (Full Fandom).

Sortie :
  wiki/Main/<fichier du dump>.txt     pages (mêmes noms de fichiers que fandom_dump_txt/)
  wiki/<Espace>/<fichier>.txt         Template, Guide, Category, Project, Module, MediaWiki, Help
  wiki/index.json                     chemin -> titre exact de la page

Usage : python tools/init_wiki.py
"""
import json
import os
import shutil
import sys

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from wiki_index import ROOT, WIKI, file_to_title, norm  # noqa: E402

OUT = os.path.join(ROOT, "wiki")
EXTRA = os.path.join(ROOT, "fandom_dump_extra")
# Espaces utiles au futur wiki (pas les discussions ni les pages utilisateur)
NAMESPACES = ["Template", "Guide", "Category", "Project", "Module", "MediaWiki", "Help"]


def main():
    if os.path.exists(OUT):
        sys.exit("wiki/ existe déjà : ce script ne sert qu'à la création initiale.")
    extra_index = json.load(open(os.path.join(EXTRA, "index.json"), encoding="utf-8"))
    by_file = {v: k for k, v in extra_index.items()}  # « Main/Statigel armor.txt » -> titre
    index = {}

    main_dir = os.path.join(OUT, "Main")
    os.makedirs(main_dir)
    dump_files = sorted(f for f in os.listdir(WIKI) if f.endswith(".txt"))
    lower_to_file = {}
    for f in dump_files:
        shutil.copyfile(os.path.join(WIKI, f), os.path.join(main_dir, f))
        index[f"Main/{f}"] = file_to_title(f[:-4])
        lower_to_file.setdefault(norm(f[:-4]), f)

    # Pages écrasées dans le dump par une redirection homonyme à la casse près : le contenu remplace la redirection
    for f in sorted(os.listdir(os.path.join(EXTRA, "Main"))):
        title = by_file.get(f"Main/{f}")
        target = lower_to_file.get(norm(f[:-4]), f)
        shutil.copyfile(os.path.join(EXTRA, "Main", f), os.path.join(main_dir, target))
        index[f"Main/{target}"] = title or f[:-4]

    for ns in NAMESPACES:
        src = os.path.join(EXTRA, ns)
        if not os.path.isdir(src):
            continue
        os.makedirs(os.path.join(OUT, ns))
        for f in sorted(os.listdir(src)):
            shutil.copyfile(os.path.join(src, f), os.path.join(OUT, ns, f))
            index[f"{ns}/{f}"] = by_file.get(f"{ns}/{f}", f[:-4])

    with open(os.path.join(OUT, "index.json"), "w", encoding="utf-8") as fh:
        json.dump(index, fh, ensure_ascii=False, indent=0, sort_keys=True)
    print(f"{len(index)} pages copiées dans wiki/")


if __name__ == "__main__":
    main()
