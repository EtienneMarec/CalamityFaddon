"""Compare le fonctionnement des objets entre le Fandom (fandom_dump_txt/, la spécification du Full Fandom) et le wiki
officiel (wikigg_dump/, Calamity 2.2.2), à partir de leurs infobulles, et classe chaque écart :

- « ancienne version » : l'infobulle du Fandom correspond à celle d'une version passée du code (data/old_tooltips.json,
  tools/old_tooltips.py) : le Fandom décrit l'objet d'avant sa refonte (Galileo Gladius, Cosmic Shiv...) ;
- « à vérifier » : elle ne correspond à aucune version du code : inventée (vandalisme) ou simple reformulation, à lire ;
- les écarts de formulation seule (le Fandom correspond au code actuel) ne sont que comptés.

Les objets officiels et renommés de data/wiki_index.json sont comparés ; la page du wiki officiel est trouvée par le nom
actuel de l'objet dans Calamity (localisation), puis par le titre du Fandom.
Résultats : reports/wikigg_diff.md (rapport) et data/wikigg_diff.json (pour la revue à la main, docs/DIFF_FANDOM.md).

Usage : python tools/old_tooltips.py ; python tools/fetch_wikigg.py ; python tools/compare_wikigg.py
"""
import difflib
import json
import os
import re
import sys

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from wiki_index import ROOT, FANDOM, extract_template, split_params, INFOBOX_RE  # noqa: E402
from gen_localization import load_calamity_strings  # noqa: E402

WIKIGG = os.path.join(ROOT, "wikigg_dump")
EXTRA = os.path.join(ROOT, "fandom_dump_extra", "Main")
INDEX = os.path.join(ROOT, "data", "wiki_index.json")
OLD = os.path.join(ROOT, "data", "old_tooltips.json")
OUT_JSON = os.path.join(ROOT, "data", "wikigg_diff.json")
OUT_MD = os.path.join(ROOT, "reports", "wikigg_diff.md")

# Seuils (ratio de difflib sur les textes normalisés)
SAME = 0.80        # au-dessus : même fonctionnement
MATCH_OLD = 0.80   # au-dessus : le Fandom reprend cette version du code

# Ampleur du changement : part des lignes de la version décrite par le Fandom qu'on retrouve aujourd'hui (code ou wiki
# officiel)
REWORK = 0.40      # en dessous : refonte
MODIFIED = 0.85    # en dessous : modifiée ; au-dessus : reformulée seulement (ou lignes ajoutées depuis)
LINE_KNOWN = 0.72  # une ligne du Fandom est « connue » si une ligne d'une version du code lui ressemble à ce point

# Lignes sans rapport avec le fonctionnement
NOISE = [
    r"^(?:counts as wings|horizontal speed|acceleration multiplier|flight time|(?:average|good|great|excellent|poor) vertical speed)",
    r"receives \d+% benefit from melee speed",
    r"^\[?c/[0-9a-f]{6}:",            # couleurs de texte
    r"^this is a (?:donor|dedicated)",
    r"^(?:dedicated|donated) (?:to|by)",
]


def page_text(path):
    try:
        return open(path, encoding="utf-8", errors="replace").read()
    except OSError:
        return None


def infobox(text):
    if not text:
        return None
    m = INFOBOX_RE.search(text)
    if not m:
        return None
    return split_params(extract_template(text, m.start()))[1]


def strip_markup(raw):
    """Wikitexte ou texte de localisation -> texte brut, une ligne par ligne d'infobulle."""
    s = raw or ""
    s = re.sub(r"<br\s*/?>", "\n", s, flags=re.I)
    s = re.sub(r"<[^>]+>", "", s)
    s = re.sub(r"\{\{\s*key\s*\|\s*([^}|]+)[^}]*\}\}", r"[\1]", s, flags=re.I)
    # {{modèle|texte|...}} -> texte (cbuff, item, mode : 1re valeur...)
    for _ in range(4):
        s = re.sub(r"\{\{\s*(?:mode)\s*\|([^{}|]*)\|[^{}]*\}\}", r"\1", s, flags=re.I)
        s = re.sub(r"\{\{\s*[\w ]+\s*\|\s*([^{}|]*)(?:\|[^{}]*)?\}\}", r"\1", s)
        s = re.sub(r"\{\{[^{}]*\}\}", "", s)
    s = re.sub(r"\[\[(?:[^\]|]*\|)?([^\]]*)\]\]", r"\1", s)
    s = re.sub(r"\[c/[0-9A-Fa-f]{6}:([^\]]*)\]", r"\1", s)
    s = re.sub(r"\{[0-9$@:\w.]+\}", "#", s)   # paramètres de localisation {0}, {$Mods...}
    s = s.replace("''", "")
    lines = []
    for line in s.split("\n"):
        line = line.strip().strip("'\"").strip()
        if not line or any(re.search(p, line.lower()) for p in NOISE):
            continue
        lines.append(line)
    return lines


def norm(lines):
    text = " ".join(lines).lower()
    text = re.sub(r"[^a-z0-9%+\-. ]", " ", text)
    return re.sub(r"\s+", " ", text).strip()


def ratio(a, b):
    if not a and not b:
        return 1.0
    return difflib.SequenceMatcher(None, a, b).ratio()


def numbers(text):
    return sorted(set(re.findall(r"\d+(?:\.\d+)?%?", text)))


def load_wikigg():
    index = json.load(open(os.path.join(WIKIGG, "index.json"), encoding="utf-8"))
    redirects = json.load(open(os.path.join(WIKIGG, "redirects.json"), encoding="utf-8"))
    lower = {t.lower(): t for t in index}

    def find(title):
        for _ in range(3):
            if title in index:
                return title
            if title in redirects:
                title = redirects[title]
                continue
            if title.lower() in lower:
                return lower[title.lower()]
            return None
        return None
    return index, find


def history_section(text):
    m = re.search(r"==\s*History\s*==(.*?)(?:\n==[^=]|\Z)", text or "", re.S)
    return m.group(1).strip() if m else ""


def main():
    index = json.load(open(INDEX, encoding="utf-8"))
    old = json.load(open(OLD, encoding="utf-8"))
    snaps = old["snapshots"]
    strings = load_calamity_strings()
    gg_index, gg_find = load_wikigg()

    results = []
    for entry in index.values():
        if entry.get("kind") != "content" or entry.get("status") not in ("official", "renamed") or not entry.get("calamity"):
            continue
        if not (entry.get("infobox") or "").startswith(("item", "armor")):
            continue
        fandom_text = page_text(os.path.join(FANDOM, entry["file"])) or page_text(os.path.join(EXTRA, entry["file"]))
        fandom_box = infobox(fandom_text) or {}
        fandom_tip = strip_markup(fandom_box.get("tooltip"))
        cal = entry["calamity"][0]
        cls, category = cal["class"], cal.get("category", "")
        key = f"Mods.CalamityMod.{category}.{cls}"
        current_name = strings.get(key + ".DisplayName")
        current_tip = strip_markup(strings.get(key + ".Tooltip"))
        gg_title = (current_name and gg_find(current_name)) or gg_find(entry["title"])
        gg_text = page_text(os.path.join(WIKIGG, gg_index[gg_title])) if gg_title else None
        gg_tip = strip_markup((infobox(gg_text) or {}).get("tooltip"))
        if not fandom_tip:
            continue
        f, g, c = norm(fandom_tip), norm(gg_tip), norm(current_tip)
        reference = g or c
        same = ratio(f, reference)
        if same >= SAME and numbers(f) == numbers(reference):
            continue
        # Versions passées du code
        best, best_date, best_text, best_lines = 0.0, None, "", []
        versions = [strip_markup(t) for _, _, t in old["items"].get(cls, [])] + [current_tip]
        for snap_index, _, tooltip in old["items"].get(cls, []):
            r = ratio(f, norm(strip_markup(tooltip)))
            if r > best:
                best, best_date, best_text, best_lines = r, snaps[snap_index]["date"], norm(strip_markup(tooltip)), strip_markup(tooltip)
        current = ratio(f, c)
        # Lignes du Fandom qu'aucune version du code ne contient (ni le wiki officiel)
        known = [norm([line]) for version in versions + [gg_tip] for line in version]
        unknown = [line for line in fandom_tip
                   if max((ratio(norm([line]), k) for k in known), default=0.0) < LINE_KNOWN]
        # Lignes de l'ancienne version encore présentes aujourd'hui (code actuel ou wiki officiel)
        today = [norm([line]) for line in current_tip + gg_tip]
        old_lines = [norm([line]) for line in (best_lines if best >= MATCH_OLD else fandom_tip)]
        kept = [line for line in old_lines if max((ratio(line, t) for t in today), default=0.0) >= LINE_KNOWN]
        change = round(len(kept) / len(old_lines), 2) if old_lines else None
        if current >= SAME and current >= best:
            verdict = "formulation"
        elif unknown:
            verdict = "à vérifier"
        elif best >= MATCH_OLD:
            verdict = "refonte" if change is not None and change < REWORK else "modifiée" if change is not None and change < MODIFIED else "reformulée"
        else:
            verdict = "mélange de versions"
        results.append({
            "title": entry["title"], "class": cls, "category": category, "wikigg": gg_title, "verdict": verdict,
            "similarity": round(same, 2), "bestOld": round(best, 2), "bestOldDate": best_date, "current": round(current, 2),
            "change": round(change, 2) if change is not None else None, "unknownLines": unknown,
            "fandom": fandom_tip, "wikiggTooltip": gg_tip, "code": current_tip,
            "history": history_section(gg_text)[:1500],
        })

    results.sort(key=lambda r: (r["verdict"], r["category"], r["title"]))
    os.makedirs(os.path.dirname(OUT_JSON), exist_ok=True)
    json.dump(results, open(OUT_JSON, "w", encoding="utf-8"), ensure_ascii=False, indent=1)
    write_report(results)


VERDICTS = [
    ("refonte", "Refontes : le Fandom décrit une ancienne version, très différente de l'objet actuel"),
    ("modifiée", "Modifiées : le Fandom décrit une ancienne version, en partie différente"),
    ("mélange de versions", "Mélange : chaque ligne du Fandom vient d'une version du code, sans qu'une version les ait toutes"),
    ("à vérifier", "À vérifier : lignes du Fandom absentes de toutes les versions du code (inventées, ou ajoutées par les rédacteurs)"),
    ("reformulée", "Reformulées seulement (même fonctionnement, ancien texte)"),
    ("formulation", "Formulation seulement (le Fandom suit le code actuel)"),
]


def write_report(results):
    by = {}
    for r in results:
        by.setdefault(r["verdict"], []).append(r)
    with open(OUT_MD, "w", encoding="utf-8") as f:
        f.write("# Fandom / wiki officiel : objets qui fonctionnent différemment\n\n")
        f.write("_Généré par `tools/compare_wikigg.py` (infobulles du Fandom, du wiki officiel et des versions du code depuis "
                "2019). Revue à la main : `docs/DIFF_FANDOM.md`._\n\n")
        for verdict, title in VERDICTS:
            f.write(f"- {verdict} : {len(by.get(verdict, []))}\n")
        for verdict, title in VERDICTS[:4]:
            f.write(f"\n## {title}\n")
            for r in sorted(by.get(verdict, []), key=lambda r: (r["change"] if r["change"] is not None else 1, r["title"])):
                f.write(f"\n### {r['title']} ({r['category'].replace('Items.', '')})\n\n")
                if r["wikigg"] and r["wikigg"] != r["title"]:
                    f.write(f"Nom actuel : {r['wikigg']}. ")
                f.write(f"Version du code la plus proche : {r['bestOldDate']} (ressemblance {r['bestOld']}) ; lignes encore présentes aujourd'hui : {r['change']}\n\n")
                f.write("- Fandom : " + " / ".join(r["fandom"]) + "\n")
                f.write("- Officiel : " + (" / ".join(r["wikiggTooltip"]) or "(pas de page)") + "\n")
                if r["unknownLines"]:
                    f.write("- Lignes introuvables dans le code : " + " / ".join(r["unknownLines"]) + "\n")
        for verdict, title in VERDICTS[4:]:
            names = ", ".join(r["title"] for r in by.get(verdict, []))
            f.write(f"\n## {title}\n\n{names}\n")
    print(f"{len(results)} écarts : " + ", ".join(f"{k} {len(v)}" for k, v in by.items()))


if __name__ == "__main__":
    main()
