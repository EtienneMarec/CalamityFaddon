"""Télécharge depuis calamitymod.fandom.com ce qui manque au dump fandom_dump_txt/ :
- les pages du namespace principal écrasées sur Windows par une redirection de même nom à la casse près
  (ex. « Statigel Armor » -> redirection, « Statigel armor » -> contenu) ;
- les autres namespaces (Template, Guide, Category, User, Talk, Module...).

Résultat : fandom_dump_extra/<Namespace>/<titre>.txt + fandom_dump_extra/index.json (titre -> fichier).
Les noms de fichiers sont dédoublonnés sans tenir compte de la casse, donc rien n'est écrasé.

Usage : python tools/fetch_missing_pages.py
"""
import collections
import json
import os
import re
import time
import urllib.parse
import urllib.request

API = "https://calamitymod.fandom.com/api.php"
ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
DUMP = os.path.join(ROOT, "fandom_dump_txt")
OUT = os.path.join(ROOT, "fandom_dump_extra")
# Tous les namespaces sauf File (médias, déjà dans fandom_sprites/) et Special/Media
NAMESPACES = {0: "Main", 1: "Talk", 2: "User", 3: "User talk", 4: "Project", 5: "Project talk", 7: "File talk",
              8: "MediaWiki", 10: "Template", 11: "Template talk", 12: "Help", 14: "Category", 15: "Category talk",
              828: "Module", 10000: "Guide", 10001: "Guide talk"}


def api(**params):
    params["format"] = "json"
    req = urllib.request.Request(API + "?" + urllib.parse.urlencode(params),
                                 headers={"User-Agent": "CalamityFandom-dump/1.0"})
    for attempt in range(5):
        try:
            return json.load(urllib.request.urlopen(req, timeout=60))
        except Exception:
            time.sleep(2 + 3 * attempt)
    raise RuntimeError(params)


def all_titles(ns):
    titles, cont = [], {}
    while True:
        d = api(action="query", list="allpages", apnamespace=ns, aplimit=500, **cont)
        titles += [p["title"] for p in d["query"]["allpages"]]
        if "continue" not in d:
            return titles
        cont = {"apcontinue": d["continue"]["apcontinue"]}


def norm(s):
    return re.sub(r"[^0-9a-z]", "", s.lower())


def lost_main_pages():
    """Pages de contenu du namespace principal dont le fichier du dump est une redirection homonyme."""
    files = collections.defaultdict(list)
    for f in os.listdir(DUMP):
        files[norm(f[:-4])].append(f)
    groups = collections.defaultdict(list)
    for t in all_titles(0):
        groups[norm(t)].append(t)
    lost = []
    for key, titles in groups.items():
        if len(titles) <= len(files[key]):
            continue
        survivors = [open(os.path.join(DUMP, f), encoding="utf-8", errors="replace").read(200) for f in files[key]]
        if not all(s.lstrip().upper().startswith("#REDIRECT") for s in survivors):
            continue
        # le dump a gardé une redirection : on récupère les titres du groupe qui n'en sont pas
        for t in titles:
            lost.append(t)
    return lost


def fetch(titles):
    """titre -> wikitexte (None si absent), par lots de 50."""
    out = {}
    for i in range(0, len(titles), 50):
        batch = titles[i:i + 50]
        d = api(action="query", prop="revisions", rvprop="content", rvslots="main", titles="|".join(batch))
        norm_map = {n["to"]: n["from"] for n in d["query"].get("normalized", [])}
        for page in d["query"]["pages"].values():
            title = norm_map.get(page["title"], page["title"])
            revs = page.get("revisions")
            out[title] = revs[0]["slots"]["main"]["*"] if revs else None
    return out


def safe_name(title, used):
    name = re.sub(r'[<>:"/\\|?*]', "_", title).strip().rstrip(".")
    candidate, n = name, 2
    while candidate.lower() in used:
        candidate = f"{name} ({n})"
        n += 1
    used.add(candidate.lower())
    return candidate + ".txt"


def main():
    jobs = {"Main": lost_main_pages()}
    for ns, label in NAMESPACES.items():
        if ns != 0:
            jobs[label] = all_titles(ns)

    index = {}
    for label, titles in jobs.items():
        if not titles:
            continue
        folder = os.path.join(OUT, label)
        os.makedirs(folder, exist_ok=True)
        used = set()
        texts = fetch(titles)
        for title in titles:
            text = texts.get(title)
            if text is None:
                continue
            # pour Main, on ne garde que les vrais contenus (les redirections sont déjà dans le dump)
            if label == "Main" and text.lstrip().upper().startswith("#REDIRECT"):
                continue
            fname = safe_name(title, used)
            with open(os.path.join(folder, fname), "w", encoding="utf-8") as f:
                f.write(text)
            index[title] = f"{label}/{fname}"
        print(f"{label}: {len(titles)} pages")

    with open(os.path.join(OUT, "index.json"), "w", encoding="utf-8") as f:
        json.dump(index, f, ensure_ascii=False, indent=1)
    print(f"{len(index)} pages écrites dans fandom_dump_extra/")


if __name__ == "__main__":
    main()
