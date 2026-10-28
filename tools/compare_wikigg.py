"""Compare le fonctionnement des objets entre le Fandom (fandom_dump_txt/, la spécification du Full Fandom) et le wiki
officiel (wikigg_dump/, Calamity 2.2.2), à partir de leurs infobulles, et classe chaque écart :

- « ancienne version » : l'infobulle du Fandom correspond à celle d'une version passée du code (data/old_tooltips.json,
  tools/old_tooltips.py) : le Fandom décrit l'objet d'avant sa refonte (Galileo Gladius, Cosmic Shiv...) ;
- « à vérifier » : elle ne correspond à aucune version du code : inventée (vandalisme) ou simple reformulation, à lire ;
- les écarts de formulation seule (le Fandom correspond au code actuel) ne sont que comptés.

Les objets officiels et renommés de data/wiki_index.json sont comparés ; la page du wiki officiel est trouvée par le nom
actuel de l'objet dans Calamity (localisation), puis par le titre du Fandom.
Résultats : reports/wikigg_diff.md (rapport) et data/wikigg_diff.json (pour la revue à la main, docs/DIFF_FANDOM.md).

Usage : python tools/old_tooltips.py ; python tools/old_strings.py ; python tools/fetch_wikigg.py ;
        python tools/compare_wikigg.py
"""
import difflib
import json
import os
import re
import sys

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from wiki_index import ROOT, FANDOM, extract_template, split_params, INFOBOX_RE  # noqa: E402
from gen_localization import load_calamity_strings  # noqa: E402

# tools/old_strings.py : corpus de toutes les phrases du code (facultatif, pour les lignes introuvables)

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
LINE_ELSEWHERE = 0.88  # même chose pour une phrase d'un autre fichier : presque identique (« Summons a X to fight for you »)

# Lignes sans rapport avec le fonctionnement
NOISE = [
    r"^(?:counts as wings|horizontal speed|acceleration multiplier|flight time|(?:average|good|great|excellent|poor) vertical speed)",
    r"receives \d+% benefit from melee speed",
    r"^\[?c/[0-9a-f]{6}:",            # couleurs de texte
    r"^this is a (?:donor|dedicated)",
    r"^current charge",                # ligne de charge ajoutée par le code (Draedon's Arsenal)
    r"crit level",                     # niveau de critique des objets de Nanotech, ajouté par le code
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


def unvandalize(raw, glossary):
    """Vocabulaire renommé par le Fandom (glossaire : Spy, Moon Burnt, Godseeker Mode...) -> termes de Calamity, pour que
    ces renommages, déjà connus, ne passent pas pour des changements de fonctionnement."""
    s = raw or ""
    for official, fandom in list(glossary.get("terms", {}).items()) + list(glossary.get("renames", {}).items()):
        s = re.sub(r"\b" + re.escape(fandom) + r"\b", official, s, flags=re.I)
    return s


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


class Corpus:
    """Toutes les phrases écrites dans le code de Calamity (data/old_strings.json), avec recherche approximative."""

    def __init__(self, first=None):
        """first : {phrase: [date, fichier]} ; par défaut data/old_strings.json."""
        if first is None:
            path = os.path.join(ROOT, "data", "old_strings.json")
            first = json.load(open(path, encoding="utf-8")) if os.path.exists(path) else {}
        # [date, fichier] (ancien format : date seule)
        self.phrases = [(norm(strip_markup(p)), v[0] if isinstance(v, list) else v, p, v[1] if isinstance(v, list) else "")
                        for p, v in first.items()]
        self.words = {}
        for i, (text, _, _, _) in enumerate(self.phrases):
            for w in set(text.split()):
                if len(w) > 3:
                    self.words.setdefault(w, []).append(i)

    def find(self, line, threshold=None):
        """(date, phrase, fichier) de la phrase du code la plus proche de la ligne, ou None."""
        threshold = LINE_ELSEWHERE if threshold is None else threshold
        text = norm([line])
        words = {w for w in text.split() if len(w) > 3}
        counts = {}
        for w in words:
            ids = self.words.get(w, [])
            if len(ids) > 2000:
                continue
            for i in ids:
                counts[i] = counts.get(i, 0) + 1
        best, found = 0.0, None
        for i, _ in sorted(counts.items(), key=lambda kv: -kv[1])[:300]:
            r = ratio(text, self.phrases[i][0])
            if r > best:
                best, found = r, self.phrases[i]
        return (found[1], found[2], found[3]) if found and best >= threshold else None


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
    glossary = json.load(open(os.path.join(ROOT, "tools", "glossary.json"), encoding="utf-8"))
    corpus = Corpus()
    gg_index, gg_find = load_wikigg()
    # Nom affiché passé -> classes qui l'ont porté (objets dont la classe a changé depuis : Astral Arcanum -> Radiance)
    by_old_name = {}
    for cls_name, history in old["items"].items():
        for _, name, _ in history:
            by_old_name.setdefault(norm([name]), set()).add(cls_name)

    results = []
    for entry in index.values():
        if entry.get("kind") != "content" or entry.get("status") not in ("official", "renamed") or not entry.get("calamity"):
            continue
        if not (entry.get("infobox") or "").startswith(("item", "armor")):
            continue
        fandom_text = page_text(os.path.join(FANDOM, entry["file"])) or page_text(os.path.join(EXTRA, entry["file"]))
        fandom_box = infobox(fandom_text) or {}
        fandom_tip = strip_markup(unvandalize(fandom_box.get("tooltip"), glossary))
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
        history = [h for c_name in {cls} | by_old_name.get(norm([entry["title"]]), set()) for h in old["items"].get(c_name, [])]
        versions = [strip_markup(t) for _, _, t in history] + [current_tip]
        for snap_index, _, tooltip in history:
            r = ratio(f, norm(strip_markup(tooltip)))
            if r > best:
                best, best_date, best_text, best_lines = r, snaps[snap_index]["date"], norm(strip_markup(tooltip)), strip_markup(tooltip)
        current = ratio(f, c)
        # Lignes du Fandom qu'aucune version du code ne contient (ni le wiki officiel)
        literals = [lit for c_name in {cls} | by_old_name.get(norm([entry["title"]]), set()) for lit in old.get("strings", {}).get(c_name, [])]
        known = [norm([line]) for version in versions + [gg_tip] + [strip_markup(lit) for lit in literals] for line in version]
        unknown = [line for line in fandom_tip
                   if max((ratio(norm([line]), k) for k in known), default=0.0) < LINE_KNOWN]
        # Ailleurs dans le code (autre objet, projectile, joueur...) à une date donnée ?
        elsewhere = {}
        for line in unknown:
            hit = corpus.find(line)
            if hit:
                elsewhere[line] = hit
        unknown = [line for line in unknown if line not in elsewhere]
        # Phrase prise dans le fichier d'un autre objet : recopiée sur la mauvaise page (Deadshot Brooch...)
        own = {c_name.lower() for c_name in {cls} | by_old_name.get(norm([entry["title"]]), set())}
        copied = {line: hit for line, hit in elsewhere.items()
                  if hit[2].startswith("Items/") and not os.path.basename(hit[2]).startswith("CalamityGlobalItem")
                  and os.path.splitext(os.path.basename(hit[2]))[0].lower() not in own}
        # Lignes de l'ancienne version encore présentes aujourd'hui (code actuel ou wiki officiel)
        today = [norm([line]) for line in current_tip + gg_tip]
        old_lines = [norm([line]) for line in (best_lines if best >= MATCH_OLD else fandom_tip)]
        kept = [line for line in old_lines if max((ratio(line, t) for t in today), default=0.0) >= LINE_KNOWN]
        change = round(len(kept) / len(old_lines), 2) if old_lines else None
        # Version 2.0.1.005 (abandon du wiki) : le Fandom la décrit-il ? (refait après l'abandon -> portable depuis 2.0.1.005)
        baseline = len(snaps) - 1
        at_baseline = [t for i, _, t in history if i <= baseline]
        r005 = ratio(f, norm(strip_markup(at_baseline[-1]))) if at_baseline else 0.0
        if current >= SAME and current >= best:
            verdict = "formulation"
        elif unknown or copied:
            verdict = "à vérifier"
        elif best >= MATCH_OLD:
            verdict = "refonte" if change is not None and change < REWORK else "modifiée" if change is not None and change < MODIFIED else "reformulée"
        else:
            verdict = "mélange de versions"
        results.append({
            "title": entry["title"], "class": cls, "category": category, "wikigg": gg_title, "verdict": verdict,
            "similarity": round(same, 2), "bestOld": round(best, 2), "bestOldDate": best_date, "current": round(current, 2),
            "change": round(change, 2) if change is not None else None, "unknownLines": unknown, "at005": round(r005, 2),
            "elsewhere": {line: f"{date}, {path} : {phrase}" for line, (date, phrase, path) in elsewhere.items()},
            "copied": sorted(copied),
            "fandom": fandom_tip, "wikiggTooltip": gg_tip, "code": current_tip,
            "history": history_section(gg_text)[:1500],
        })

    # Revue à la main (tools/diff_review.json) : verdict tranché et preuves
    review = json.load(open(os.path.join(ROOT, "tools", "diff_review.json"), encoding="utf-8"))
    armor = compare_set_bonuses(index, glossary, corpus, strings, gg_index, gg_find, review)
    for r in results:
        if r["title"] in review:
            r["auto"] = r["verdict"]
            r["verdict"] = review[r["title"]]["verdict"]
            r["note"] = review[r["title"]].get("note", "")
    # Pages revues à la main sans écart d'infobulle détecté (pas d'infobulle sur le Fandom, page qui invente ailleurs)
    compared = {r["title"] for r in results}
    for title, item in review.items():
        if title.startswith("_") or title in compared or any(a["title"] == title for a in armor):
            continue
        results.append({"title": title, "class": "", "category": "", "wikigg": None, "verdict": item["verdict"],
                        "similarity": None, "bestOld": None, "bestOldDate": None, "current": None, "change": None,
                        "unknownLines": [], "elsewhere": {}, "copied": [], "fandom": [], "wikiggTooltip": [], "code": [],
                        "history": "", "note": item.get("note", ""), "auto": "page"})
    results.sort(key=lambda r: (r["verdict"], r["category"], r["title"]))
    os.makedirs(os.path.dirname(OUT_JSON), exist_ok=True)
    json.dump(results, open(OUT_JSON, "w", encoding="utf-8"), ensure_ascii=False, indent=1)
    write_report(results, armor)


def compare_set_bonuses(index, glossary, corpus, strings, gg_index, gg_find, review):
    """Bonus de set des pages d'armure (champ setbonus) : chaque ligne du Fandom est-elle encore vraie (wiki officiel,
    localisation actuelle), d'une version passée (corpus du code) ou introuvable ?"""
    # Textes actuels de Calamity (localisation 2.2.2), cherchés comme le corpus
    today_corpus = Corpus({line: ["2.2.2", key] for key, value in strings.items() if isinstance(value, str)
                           for line in value.split("\n") if line.strip()})
    sets = []
    for entry in index.values():
        if entry.get("kind") != "content" or entry.get("status") not in ("official", "renamed", "group"):
            continue
        text = page_text(os.path.join(FANDOM, entry["file"])) or page_text(os.path.join(EXTRA, entry["file"]))
        bonus = (infobox(text) or {}).get("setbonus")
        if not bonus:
            continue
        gg_title = gg_find(entry["title"])
        gg_bonus = (infobox(page_text(os.path.join(WIKIGG, gg_index[gg_title]))) or {}).get("setbonus") if gg_title else None
        today = [norm([line]) for line in strip_markup(gg_bonus)]
        lines = [line for line in strip_markup(unvandalize(bonus, glossary)) if not re.match(r"^[\w' ]+:$", line)]
        old_lines, unknown = [], []
        for line in lines:
            n = norm([line])
            if max((ratio(n, t) for t in today), default=0.0) >= LINE_KNOWN or today_corpus.find(line, LINE_KNOWN):
                continue
            hit = corpus.find(line)
            if hit:
                old_lines.append((line, hit[0]))
            else:
                unknown.append(line)
        if not old_lines and not unknown:
            continue
        verdict = review.get(entry["title"], {}).get("verdict") or ("à vérifier" if unknown else "ancienne version")
        sets.append({"title": entry["title"], "wikigg": gg_title, "verdict": verdict, "old": old_lines, "unknown": unknown,
                     "note": review.get(entry["title"], {}).get("note", "")})
    return sorted(sets, key=lambda r: (r["verdict"], r["title"]))


VERDICTS = [
    ("inventée", "Inventées (revue à la main) : le Fandom décrit un fonctionnement qui n'a jamais existé dans Calamity"),
    ("mélange", "Mélange (revue à la main) : en partie une ancienne version, en partie inventé"),
    ("ancienne version", "Anciennes versions confirmées à la main (notes de version du wiki officiel)"),
    ("refonte", "Refontes : le Fandom décrit une ancienne version, très différente de l'objet actuel"),
    ("modifiée", "Modifiées : le Fandom décrit une ancienne version, en partie différente"),
    ("mélange de versions", "Mélange : chaque ligne du Fandom vient d'une version du code, sans qu'une version les ait toutes"),
    ("à vérifier", "À vérifier : lignes du Fandom absentes de toutes les versions du code (inventées, ou ajoutées par les rédacteurs)"),
    ("homonyme", "Homonymes (revue à la main) : objet inventé par le Fandom ; Calamity a créé plus tard un objet différent du même nom"),
    ("addon", "Addons (revue à la main) : le Fandom décrit l'effet d'un autre mod"),
    ("ambiance", "Ambiance (revue à la main) : seule une phrase d'ambiance diffère, même fonctionnement"),
    ("reformulation", "Reformulation (revue à la main) : même fonctionnement"),
    ("reformulée", "Reformulées seulement (même fonctionnement, ancien texte)"),
    ("formulation", "Formulation seulement (le Fandom suit le code actuel)"),
]


def write_report(results, armor):
    by = {}
    for r in results:
        by.setdefault(r["verdict"], []).append(r)
    with open(OUT_MD, "w", encoding="utf-8") as f:
        f.write("# Fandom / wiki officiel : objets qui fonctionnent différemment\n\n")
        f.write("_Généré par `tools/compare_wikigg.py` (infobulles du Fandom, du wiki officiel et des versions du code depuis "
                "2019). Revue à la main : `docs/DIFF_FANDOM.md`._\n\n")
        for verdict, title in VERDICTS:
            f.write(f"- {verdict} : {len(by.get(verdict, []))}\n")
        for verdict, title in VERDICTS[:9]:
            f.write(f"\n## {title}\n")
            for r in sorted(by.get(verdict, []), key=lambda r: (r["change"] if r["change"] is not None else 1, r["title"])):
                if r.get("auto") == "page":
                    f.write(f"\n### {r['title']}\n\n- **Revue** : {r['note']}\n")
                    continue
                f.write(f"\n### {r['title']} ({r['category'].replace('Items.', '')})\n\n")
                if r["wikigg"] and r["wikigg"] != r["title"]:
                    f.write(f"Nom actuel : {r['wikigg']}. ")
                when = "refait après 2.0.1.005" if (r.get("at005") or 0) >= MATCH_OLD else "déjà différent en 2.0.1.005"
                f.write(f"Version du code la plus proche : {r['bestOldDate']} (ressemblance {r['bestOld']}, {when}) ; "
                        f"lignes encore présentes aujourd'hui : {r['change']}\n\n")
                f.write("- Fandom : " + " / ".join(r["fandom"]) + "\n")
                f.write("- Officiel : " + (" / ".join(r["wikiggTooltip"]) or "(pas de page)") + "\n")
                if r["unknownLines"]:
                    f.write("- Lignes introuvables dans le code : " + " / ".join(r["unknownLines"]) + "\n")
                if r.get("note"):
                    f.write(f"- **Revue** : {r['note']}\n")
                for line, where in r.get("elsewhere", {}).items():
                    copied = " — **recopiée d'un autre objet**" if line in r.get("copied", []) else ""
                    f.write(f"- « {line} » : ailleurs dans le code ({where}){copied}\n")
        for verdict, title in VERDICTS[9:]:
            names = ", ".join(r["title"] for r in by.get(verdict, []))
            f.write(f"\n## {title}\n\n{names}\n")
        f.write("\n## Bonus de set d'armure (lignes du Fandom qui ne sont plus vraies)\n")
        for r in armor:
            f.write(f"\n### {r['title']} ({r['verdict']})\n\n")
            for line, date in r["old"]:
                f.write(f"- ancienne version ({date}) : {line}\n")
            for line in r["unknown"]:
                f.write(f"- introuvable dans le code : {line}\n")
            if r["note"]:
                f.write(f"- **Revue** : {r['note']}\n")
    print(f"{len(results)} écarts : " + ", ".join(f"{k} {len(v)}" for k, v in by.items()))
    print(f"{len(armor)} sets d'armure : " + ", ".join(f"{v} {sum(1 for r in armor if r['verdict'] == v)}" for v in sorted({r['verdict'] for r in armor})))


if __name__ == "__main__":
    main()
