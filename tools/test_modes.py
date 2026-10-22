"""Tests du modèle {{mode|Full Fandom|Calamity++}} dans les générateurs (wikitexte fabriqué, pas de fichiers).

Usage : python tools/test_modes.py   (code de sortie 0 si tout passe)
"""
import os
import sys

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from wiki_index import extract_template, has_mode, pick_mode  # noqa: E402
from gen_recipes import recipes_by_mode, split_all  # noqa: E402
from gen_loot import drops_by_mode, parse_condition  # noqa: E402
from gen_elements import values  # noqa: E402
from gen_stats import item_value, number  # noqa: E402

failures = []


def check(condition, label):
    print(("OK    " if condition else "ÉCHEC ") + label)
    if not condition:
        failures.append(label)


def params(text):
    return split_all(extract_template(text, 0))


# --- pick_mode / has_mode
check(pick_mode("{{mode|68|3170}}", "fandom") == "68" and pick_mode("{{mode|68|3170}}", "plus") == "3170", "pick_mode simple")
check(pick_mode("{{mode|{{dv|1|2}}|5}}", "fandom") == "{{dv|1|2}}", "pick_mode avec modèle imbriqué")
check(pick_mode("42", "plus") == "42" and not has_mode("42"), "texte sans {{mode}} inchangé")
check(pick_mode("{{mode|Night's Gaze|}}", "plus") == "", "valeur vide = absent dans ce mode")

# --- stats
check(number(pick_mode("{{mode|{{dv|350,000|600,000}}|4000}}", "fandom")) == 350000, "stats : {{dv}} dans {{mode}}")
check(item_value("crit", pick_mode("{{mode|19%|10%}}", "plus")) == 6, "stats : critique en Calamity++ (10 % - 4)")

# --- recettes
same = "{{recipes/register | station = Cosmic Anvil | result = Thing | amount = 1 | Lumenyl | 25 }}"
r = recipes_by_mode(*params(same), "test", [])
check(len(r) == 1 and "modes" not in r[0], "recette sans {{mode}} : une seule, pour les deux modes")

qty = "{{recipes/register | station = Cosmic Anvil | result = Thing | amount = 1 | Lumenyl | {{mode|25|10}} }}"
r = recipes_by_mode(*params(qty), "test", [])
by = {x["modes"][0]: x for x in r}
check(by["fandom"]["ingredients"][0]["stack"] == 25 and by["plus"]["ingredients"][0]["stack"] == 10, "recette : quantité par mode")

drop = ("{{recipes/register | station = Cosmic Anvil | result = Zweihandromeda | amount = 1 | Lumenyl | 25"
        " | {{mode|Night's Gaze|}} | {{mode|1|}} }}")
r = recipes_by_mode(*params(drop), "test", [])
by = {x["modes"][0]: x for x in r}
check(len(by["fandom"]["ingredients"]) == 2 and len(by["plus"]["ingredients"]) == 1, "recette : ingrédient absent en Calamity++")

only = "{{recipes/register | station = Cosmic Anvil | result = {{mode|Thing|}} | amount = 1 | Lumenyl | 25 }}"
r = recipes_by_mode(*params(only), "test", [])
check(len(r) == 1 and r[0]["modes"] == ["fandom"], "recette : n'existe qu'en Full Fandom")

# --- butin
loot = "{{npc infobox | type = Enemy | Aerialite Ore|10-26| 100% | {{mode|Skyline Wings|}}|{{mode|1|}}|{{mode|3%|}} }}"
drops, _ = drops_by_mode(params(loot)[0])
plain = [d for d in drops if "modes" not in d]
check(len(plain) == 1 and plain[0]["item"] == "Aerialite Ore", "butin : drop commun aux deux modes")
check(any(d["item"] == "Skyline Wings" and d["modes"] == ["fandom"] for d in drops), "butin : drop en Full Fandom seulement")
chance = "{{npc infobox | type = Enemy | Aerialite Ore|10-26| {{mode|100%|50%}} }}"
drops, _ = drops_by_mode(params(chance)[0])
check({d["modes"][0]: d["chance"] for d in drops} == {"fandom": 1.0, "plus": 0.5}, "butin : chance par mode")

# --- conditions de butin (pas des {{mode}}, mais lues par le même générateur)
check(parse_condition("(After defeating<br/>Golem)") == {"downed": ["golem"]}, "condition : boss vaincu")
check(parse_condition("(When defeated in Space during Hardmode)") == {"downed": ["wall of flesh"], "biome": "space"},
      "condition : biome + Hardmode")
check(parse_condition("(Always on first kill)") == {"firstKill": True}, "condition : premier kill")
check(parse_condition("(After defeating The Starborn Moth)") is None, "condition : boss inconnu -> non appliquée")

# --- éléments
fields = {"element1": "Slash", "element2": "{{mode|Machine|}}"}
full = values({k: pick_mode(v, "fandom") for k, v in fields.items()}, "element")[0]
plus = values({k: pick_mode(v, "plus") for k, v in fields.items()}, "element")[0]
check(full == ["Slash", "Machine"] and plus == ["Slash"], "éléments : élément en Full Fandom seulement")

# --- localisation : un .hjson mal formé empêche tout le mod de se charger (22/10 : chaîne coupée par un vrai retour à la ligne)
import glob  # noqa: E402
import hjson  # noqa: E402
for path in glob.glob(os.path.join(os.path.dirname(os.path.dirname(os.path.abspath(__file__))), "Localization", "**", "*.hjson"), recursive=True):
    try:
        hjson.load(open(path, encoding="utf-8-sig"))
        readable = True
    except Exception:  # noqa: BLE001
        readable = False
    check(readable, f"localisation lisible : {os.path.basename(path)}")

print(f"\n{len(failures)} échec(s)")
sys.exit(1 if failures else 0)
