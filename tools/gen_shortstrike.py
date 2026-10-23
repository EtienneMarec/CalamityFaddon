"""Shortstrike (wiki/Main/Shortstrike.txt) -> WikiData/shortstrike.json : règles de la jauge et attaques Shortflurry.

Règles : chiffres du texte de la page (1 point par 10 dégâts et par coup porté, 100 max, +1 % de dégâts tous les
5 points, +1 défense tous les 3). Calamity++ les ajuste dans Balance/shortstrike.json.

Attaques : une ligne du tableau « Shortflurry attacks » par arme (bonus de dégâts, nombre de coups, durée, effets).
Chaque effet écrit en anglais est traduit en données (`effects`) quand il est reconnu ; sinon il reste dans `todo`
(rayons, lianes...) et le rapport reports/shortstrike.md le liste.

Attaques hors wiki (décision du 22/10, docs/decisions.md) : tools/shortstrike_extra.json (Shortflurry générique des
épées courtes des autres mods, Sahara Slicers en Calamity++).

Usage : python tools/gen_shortstrike.py   (lancé par build_data.py)
"""
import json
import os
import re
import sys

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from wiki_index import OUR_MAIN, ROOT  # noqa: E402

PAGE = os.path.join(OUR_MAIN, "Shortstrike.txt")
EXTRA = os.path.join(ROOT, "tools", "shortstrike_extra.json")
OUT = os.path.join(ROOT, "WikiData", "shortstrike.json")
REPORT = os.path.join(ROOT, "reports", "shortstrike.md")

RULES = {
    "maxPoints": 100,
    "damagePerPoint": 10,          # « For every 10 damage dealt using a shortsword »
    "pointPerHitSwing": 1,         # « and for every swing of a shortsword that hits an enemy »
    "pointsPerDamagePercent": 5,   # « 1% damage for every 5 points »
    "pointsPerDefense": 3,         # « and 1 defense for every 3 »
}

# Effet du wiki -> données. Groupes nommés : n (nombre), name (buff ou debuff).
PATTERNS = [
    (r"increases player defense by (?P<n>\d+)$", lambda m: [{"type": "defense", "value": int(m["n"])}]),
    (r"increases player damage reduction by (?P<n>\d+)%", lambda m: [{"type": "damageReduction", "value": int(m["n"]) / 100}]),
    (r"increases player movement speed by (?P<n>\d+)%", lambda m: [{"type": "moveSpeed", "value": int(m["n"]) / 100}]),
    (r"increases player melee speed by (?P<n>\d+)%", lambda m: [{"type": "meleeSpeed", "value": int(m["n"]) / 100}]),
    (r"increases player melee crit chance by (?P<n>\d+)%", lambda m: [{"type": "meleeCrit", "value": int(m["n"])}]),
    (r"increases player melee speed and damage reduction by (?P<n>\d+)%",
     lambda m: [{"type": "meleeSpeed", "value": int(m["n"]) / 100}, {"type": "damageReduction", "value": int(m["n"]) / 100}]),
    (r"increases player melee speed, defense, and damage reduction by (?P<n>\d+)%",
     lambda m: [{"type": "meleeSpeed", "value": int(m["n"]) / 100}, {"type": "defensePercent", "value": int(m["n"]) / 100},
                {"type": "damageReduction", "value": int(m["n"]) / 100}]),
    (r"attacks inflict \[\[(?:[^|\]]*\|)?(?P<name>[^\]]+)\]\](?: for (?P<n>[\d.]+) seconds?)?$",
     lambda m: [{"type": "inflict", "name": m["name"], "seconds": float(m["n"]) if m["n"] else None}]),
    (r"heals the player for (?P<n>\d+) health per hit", lambda m: [{"type": "healPerHit", "value": int(m["n"])}]),
    (r"hits drain (?P<n>\d+)% health", lambda m: [{"type": "drainPercent", "value": int(m["n"]) / 100}]),
    (r"provides (?P<n>\d+) life regeneration for (?P<s>\d+) seconds",
     lambda m: [{"type": "lifeRegen", "value": int(m["n"]), "seconds": int(m["s"])}]),
    (r"player becomes immune to \[\[(?P<name>[^\]]+)\]\] for (?P<n>\d+) seconds",
     lambda m: [{"type": "immune", "name": m["name"], "seconds": int(m["n"])}]),
    (r"player gains the \[\[(?P<name>[^\]]+)\]\] buff for (?P<n>\d+) seconds",
     lambda m: [{"type": "gainBuff", "name": m["name"], "seconds": int(m["n"])}]),
    (r"player gains immunity to most debuffs for (?P<n>\d+) seconds", lambda m: [{"type": "immuneAll", "seconds": int(m["n"])}]),
    (r"hits spawn homing plague seekers for each hit", lambda m: [{"type": "spawnOnHit", "projectile": "PlagueSeeker"}]),
    (r"hits spawn homing astral stars for each hit", lambda m: [{"type": "spawnOnHit", "projectile": "AstralStar"}]),
    (r"creates bloodsplosions on enemy hits", lambda m: [{"type": "spawnOnHit", "projectile": "BloodBombExplosion"}]),
]


def clean(cell):
    cell = re.sub(r"\{\{item\|([^|}]+)[^}]*\}\}", r"\1", cell)
    return cell.strip()


def parse_effects(text):
    effects, todo = [], []
    for part in re.split(r"<br\s*/?>", text):
        part = part.strip()
        if not part:
            continue
        low = part.lower().rstrip(".")
        for pattern, build in PATTERNS:
            m = re.search(pattern, low)
            if m:
                # Les noms de buff gardent leur casse d'origine
                groups = {k: (re.search(pattern, part.rstrip("."), re.I)[k] if k == "name" else v) for k, v in m.groupdict().items()}
                effects += build(groups)
                break
        else:
            todo.append(part)
    return effects, todo


def parse_table(text):
    table = text[text.index("{| class=\"terraria lined\""):]
    table = table[:table.index("\n|}")]
    attacks = []
    for row in table.split("\n|-")[1:]:
        cells = [c.strip() for c in re.split(r"\s*\|\|\s*", row.strip().lstrip("|").strip())]
        if len(cells) < 5 or not cells[0].startswith("{{item"):
            continue
        weapon, boost, hits, duration, effect_text = clean(cells[0]), cells[1], cells[2], cells[3], " || ".join(cells[4:])
        effects, todo = parse_effects(effect_text)
        attacks.append({
            "weapon": weapon,
            "damageBoost": int(boost.rstrip("%")) / 100,
            "hits": int(hits),
            "seconds": float(re.match(r"[\d.]+", duration)[0]),
            "effects": effects,
            "todo": todo,
            "text": effect_text,
        })
    return attacks


def main():
    text = open(PAGE, encoding="utf-8").read()
    attacks = parse_table(text)
    extra = json.load(open(EXTRA, encoding="utf-8"))
    for entry in extra["attacks"]:
        # « effects » écrits à la main : le texte n'est alors qu'une description
        effects, todo = parse_effects(entry.get("text", "")) if "effects" not in entry else ([], [])
        attacks.append({**entry, "effects": entry.get("effects", []) + effects, "todo": todo, "offWiki": True})
    # Effets écrits à la main pour des phrases de la page non traduites (projectile choisi : [choix], docs/decisions.md)
    for attack in attacks:
        override = extra.get("overrides", {}).get(attack["weapon"])
        if override:
            attack["effects"] = attack["effects"] + override["effects"]
            attack["todo"] = [t for t in attack["todo"] if t not in override["todo"]]
    with open(OUT, "w", encoding="utf-8") as f:
        json.dump({"_generated_by": "tools/gen_shortstrike.py", "rules": RULES, "generic": extra["generic"], "attacks": attacks},
                  f, ensure_ascii=False, indent=1)
    todo = [(a["weapon"], t) for a in attacks for t in a["todo"]]
    with open(REPORT, "w", encoding="utf-8") as f:
        f.write("# Shortstrike\n\n_Généré par `tools/gen_shortstrike.py` depuis la page Shortstrike._\n\n")
        f.write(f"- {len(attacks)} Shortflurry ({sum(1 for a in attacks if a.get('offWiki'))} hors wiki)\n")
        f.write(f"- {len(todo)} effets pas encore traduits en données (à faire à la main dans le C#)\n\n")
        f.write("## Effets à faire\n\n" + "\n".join(f"- **{w}** : {t}" for w, t in todo) + "\n")
    print(f"{len(attacks)} Shortflurry, {sum(len(a['effects']) for a in attacks)} effets en données, {len(todo)} à faire")


if __name__ == "__main__":
    main()
