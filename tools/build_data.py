"""Régénère toutes les données du wiki, dans l'ordre : pages d'éléments du wiki (depuis le code), index, textes, stats, recettes, éléments, objets simples, butin.
tools/apply_modes.py n'est pas lancé ici : il réécrit des pages, à lancer exprès (docs/WIKI.md).

Usage : python tools/build_data.py
Ensuite : dotnet build CalamityFandom.csproj, puis python tools/selftest.py (jeu fermé).
"""
import os
import subprocess
import sys

STEPS = ["test_modes.py", "gen_element_pages.py", "wiki_index.py", "gen_localization.py", "gen_stats.py", "gen_recipes.py", "gen_baseline.py", "gen_elements.py", "gen_items.py", "gen_loot.py"]


def main():
    here = os.path.dirname(os.path.abspath(__file__))
    env = dict(os.environ, PYTHONIOENCODING="utf-8")
    for step in STEPS:
        print(f"== {step}")
        result = subprocess.run([sys.executable, os.path.join(here, step)], env=env)
        if result.returncode != 0:
            sys.exit(f"Échec de {step}")


if __name__ == "__main__":
    main()
