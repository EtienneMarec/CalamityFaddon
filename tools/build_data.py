"""Régénère toutes les données du wiki, dans l'ordre : index, textes, stats, recettes, éléments, objets simples, butin.

Usage : python tools/build_data.py
Ensuite : dotnet build CalamityFandom.csproj, puis python tools/selftest.py (jeu fermé).
"""
import os
import subprocess
import sys

STEPS = ["wiki_index.py", "gen_localization.py", "gen_stats.py", "gen_recipes.py", "gen_elements.py", "gen_items.py", "gen_loot.py"]


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
