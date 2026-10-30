"""Tableau de la page « Proficiency » du wiki -> WikiData/proficiency.json (niveaux, seuils, bonus par niveau).

Les bonus ne se cumulent pas d'un niveau à l'autre (« The boosts listed below are not cumulative ») : chaque ligne
donne les bonus du niveau atteint. Règles de la page (valeur max, recharge, seuil de dégâts, Level Meters) dans `rules`.

Usage : python tools/gen_proficiency.py   (lancé par build_data.py)
"""
import json
import os
import re
import sys

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from wiki_index import OUR_MAIN, ROOT  # noqa: E402

OUT = os.path.join(ROOT, "WikiData", "proficiency.json")
COLUMNS = ["level", "required", "damage", "crit", "meleeSpeed", "ammoConsumption", "manaCost", "minionKnockback",
           "minionSlots", "projectileSpeed"]


def number(cell):
    cell = cell.strip().replace("%", "").replace("+", "")
    return float(cell) if cell else 0.0


def main():
    text = open(os.path.join(OUR_MAIN, "Proficiency.txt"), encoding="utf-8").read()
    levels = []
    for line in text.splitlines():
        m = re.match(r"^\|\s*(\d+)\s*\|\|(.*)$", line)
        if not m:
            continue
        cells = [m.group(1)] + m.group(2).split("||")
        if len(cells) != len(COLUMNS):
            continue
        row = dict(zip(COLUMNS, (number(c) for c in cells)))
        row["level"], row["required"], row["minionSlots"] = int(row["level"]), int(row["required"]), int(row["minionSlots"])
        for key in ("damage", "crit", "meleeSpeed", "ammoConsumption", "manaCost", "projectileSpeed"):
            row[key] = round(row[key] / 100, 4)
        levels.append(row)
    levels.sort(key=lambda r: r["required"])
    rules = {
        # « Each proficiency is represented by a number between 0 and 500000 »
        "maxValue": 500000,
        # « more than 5 damage to a non statue-spawned enemy, or any amount of damage to a boss »
        "minDamage": 5,
        # « There is a two second cooldown between increases of any proficiency » ; Level Meter : « cooldown is halved »
        "cooldownTicks": 120,
        # Level Meters : « 10% chance to earn double proficiency if their current proficiency is not divisible by 100 »
        "meterDoubleChance": 0.1,
    }
    os.makedirs(os.path.dirname(OUT), exist_ok=True)
    with open(OUT, "w", encoding="utf-8") as f:
        json.dump({"_generated_by": "tools/gen_proficiency.py", "rules": rules, "levels": levels}, f, ensure_ascii=False, indent=1)
    print(f"{len(levels)} niveaux, dernier à {levels[-1]['required']} points")


if __name__ == "__main__":
    main()
