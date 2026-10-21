"""Écrit dans wiki/ la documentation du système d'éléments telle qu'implémentée, à partir du code (source de vérité) :
Common/Elements/Element.cs (effets), Common/Elements/ElementStatus.cs (debuffs), Balance/elements.json (chances en
Calamity++), Localization (noms et descriptions).

  - une page par debuff d'élément (wiki/Main/<Nom>.txt) ;
  - la section « Status effects » de wiki/Main/Elemental System.txt (entre les marqueurs <!-- gen_element_pages -->).

À relancer après tout changement de ces valeurs. Usage : python tools/gen_element_pages.py
"""
import json
import os
import re
import sys

import hjson

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from wiki_index import OUR_MAIN, OUR_WIKI, ROOT  # noqa: E402

ELEMENTS = ["Cold", "Fire", "Wind", "Water", "Poison", "Acid", "Radioactive", "Electricity", "Stab", "Slash",
            "Smash", "Bomb", "Magic", "Dark", "Holy", "Unholy", "Machine"]
BEGIN, END = "<!-- gen_element_pages : début (généré, ne pas éditer) -->", "<!-- gen_element_pages : fin -->"
NL = "\r\n"  # les pages du wiki utilisent des fins de ligne Windows


def read_effects():
    text = open(os.path.join(ROOT, "Common", "Elements", "Element.cs"), encoding="utf-8-sig").read()
    block = text[text.index("Effects =") : text.index("};", text.index("Effects ="))]
    effects = {}
    for line in block.splitlines():
        m = re.search(r"(new\((.*)\)|null),?\s*//\s*(\w+)", line)
        if not m or m.group(3) not in ELEMENTS:
            continue
        args = m.group(2) or ""
        e = {}
        d = re.search(r"Dps:\s*new\[\]\s*\{\s*([\d,\s]+)\}", args)
        if d:
            e["dps"] = [int(x) for x in d.group(1).split(",")]
        for key in ("Slow", "DamageDown", "DefenseDown", "MissChance"):
            v = re.search(rf"{key}:\s*([\d.]+)f", args)
            if v:
                e[key] = float(v.group(1))
        e["Confuse"] = "Confuse: true" in args
        e["NoRegen"] = "NoRegen: true" in args
        effects[m.group(3)] = None if m.group(1) == "null" else e
    return effects


def read_statuses():
    text = open(os.path.join(ROOT, "Common", "Elements", "ElementStatus.cs"), encoding="utf-8-sig").read()
    return {m.group(2): m.group(1) for m in re.finditer(r"class (\w+) : ElementStatus \{ public override Element Element => Element\.(\w+);", text)}


def pct(v):
    return f"{v * 100:g}%"


def effect_lines(effect):
    lines = []
    if effect.get("dps"):
        a, b, c = effect["dps"]
        lines.append(f"Loses {a} / {b} / {c} life per second (pre-[[Hardmode]] / [[Hardmode]] / after the [[Moon Lord]])")
    if effect.get("Slow"):
        lines.append(f"Moves {pct(effect['Slow'])} slower (half as much on bosses)")
    if effect.get("DamageDown"):
        lines.append(f"Contact damage reduced by {pct(effect['DamageDown'])}")
    if effect.get("DefenseDown"):
        lines.append(f"Defense reduced by {pct(effect['DefenseDown'])}")
    if effect.get("Confuse"):
        lines.append("[[Confused]]: moves erratically")
    if effect.get("MissChance"):
        lines.append(f"{pct(effect['MissChance'])} chance to miss its contact attacks")
    if effect.get("NoRegen"):
        lines.append("Cannot regenerate life")
    return lines


def main():
    effects = read_effects()
    statuses = read_statuses()
    chances = json.load(open(os.path.join(ROOT, "Balance", "elements.json"), encoding="utf-8"))["procChance"]
    loc = hjson.load(open(os.path.join(ROOT, "Localization", "en-US", "Mods.CalamityFandom.hjson"), encoding="utf-8"))["Buffs"]
    index_path = os.path.join(OUR_WIKI, "index.json")
    index = json.load(open(index_path, encoding="utf-8"))

    rows = []
    for element in ELEMENTS:
        effect = effects.get(element)
        status = statuses.get(element)
        chance = f"{{{{mode|100%|{pct(chances.get(element, 1))}}}}}"
        if not effect or not status:
            special = {"Wind": "No lasting effect: hits push enemies away 50% harder.",
                       "Bomb": "No lasting effect: critical hits deal twice the usual bonus and are 50% more likely; "
                               "each enemy already hit by the same explosion that tick adds 10% damage (up to 2x)."}[element]
            rows.append(f"|-{NL}| [[File:{element} (element).png|link={element} (element)]] [[{element} (element)|{element}]] || — || {special} || —")
            continue
        name = loc[status]["DisplayName"]
        lines = effect_lines(effect)
        rows.append(f"|-{NL}| [[File:{element} (element).png|link={element} (element)]] [[{element} (element)|{element}]] || [[{name}]] || "
                    + "<br/>".join(lines) + f" || {chance}")

        page = NL.join([
            "{{buff infobox",
            "| type = Debuff",
            f"| image = {element} (element).png",
            f"| effects = " + "<br/>".join(lines),
            f"| tooltip = {loc[status]['Description']}",
            "}}",
            "",
            f"'''{name}''' is a [[debuff]] of the [[Elemental System]], inflicted on enemies by [[{element} (element)|{element}]] weapons. "
            f"It lasts 2 seconds and is refreshed by every new hit that inflicts it.",
            "",
            "== Effects ==",
            *[f"* {l}" for l in lines],
            "",
            "== Causes ==",
            f"* Any hit from a weapon with the {element} element (or from its projectiles), with a {chance} chance per hit "
            "(Full Fandom / Calamity++, see [[Fandom Accuracy]]).",
            f"* Enemies [[Elemental System#Elemental Attributes|resistant]] to {element} are immune.",
            "",
            "== Notes ==",
            "* Enemies affected by elemental debuffs show particles of the element's color instead of a debuff icon.",
            "* Effects of several elemental debuffs add up, capped at 50% for slowing, damage reduction, defense reduction and missing.",
            "",
            "{{Master Template Elements}}",
            "[[Category:Debuffs]]",
            "[[Category:Elemental debuffs]]",
            "",
        ])
        fname = f"{name}.txt"
        with open(os.path.join(OUR_MAIN, fname), "w", encoding="utf-8", newline="") as fh:
            fh.write(page)
        index[f"Main/{fname}"] = name

    section = NL.join([
        BEGIN,
        "== Status effects ==",
        "Every hit from an elemental weapon can inflict the lasting effect of each of its elements on the enemy, for 2 seconds. "
        "In '''Full Fandom''' every hit inflicts every effect; in '''Calamity++''' each effect has its own chance per hit "
        "(see [[Fandom Accuracy]]). Enemies resistant to an element are immune to its effect.",
        "",
        '{| class="terraria lined"',
        "! Element !! Debuff !! Effect !! Chance per hit",
        *rows,
        "|}",
        "",
        "=== Rules ===",
        "* A weapon with several elements multiplies their damage modifiers: a fire weapon against an enemy vulnerable to fire "
        "and resistant to poison deals normal damage if it is also a poison weapon. The total is kept between 25% and 400%.",
        "* Projectiles carry the elements of the weapon (or projectile) that fired them.",
        "* Attacks whose damage is changed by an affinity show '''Vulnerable!''' or '''Resisted''' above the enemy.",
        "* Machine weapons also increase the player's damage and critical strike chance by 5% while they are being used.",
        "* Enemies affected by elemental debuffs show particles of the element and take a slight tint of its color; "
        "no debuff icon is displayed.",
        END,
    ])
    path = os.path.join(OUR_MAIN, "Elemental System.txt")
    text = open(path, encoding="utf-8", newline="").read()
    if BEGIN in text:
        text = text[: text.index(BEGIN)] + section + text[text.index(END) + len(END):]
    else:
        anchor = "== Elemental Attributes =="
        text = text.replace(anchor, section + NL + NL + anchor, 1)
    with open(path, "w", encoding="utf-8", newline="") as fh:
        fh.write(text)

    with open(index_path, "w", encoding="utf-8") as fh:
        json.dump(index, fh, ensure_ascii=False, indent=0, sort_keys=True)
    print(f"{len(rows)} éléments documentés, {sum(1 for e in ELEMENTS if effects.get(e))} pages de debuff écrites")


if __name__ == "__main__":
    main()
