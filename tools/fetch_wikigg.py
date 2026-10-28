"""Télécharge le wiki officiel de Calamity (calamitymod.wiki.gg, à jour avec la version 2.2.2) dans wikigg_dump/ :
pages du namespace principal en wikitexte, même nommage de fichiers que fandom_dump_txt/, plus wikigg_dump/index.json
(titre -> fichier) et wikigg_dump/redirects.json (titre -> cible). Sert de comparaison avec le Fandom
(tools/compare_wikigg.py) ; jamais lu par le mod. Gardé en local (.gitignore), à relancer pour le rafraîchir.

Usage : python tools/fetch_wikigg.py
"""
import json
import os
import time
import urllib.parse
import urllib.request

API = "https://calamitymod.wiki.gg/api.php"
ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
OUT = os.path.join(ROOT, "wikigg_dump")


def api(**params):
    params["format"] = "json"
    req = urllib.request.Request(API + "?" + urllib.parse.urlencode(params), headers={"User-Agent": "CalamityFandom-compare/1.0"})
    for attempt in range(6):
        try:
            return json.load(urllib.request.urlopen(req, timeout=90))
        except Exception:
            time.sleep(3 + 5 * attempt)
    raise RuntimeError(params)


def all_titles(redirects):
    titles, cont = [], {}
    while True:
        d = api(action="query", list="allpages", apnamespace=0, aplimit=500,
                apfilterredir="redirects" if redirects else "nonredirects", **cont)
        titles += [p["title"] for p in d["query"]["allpages"]]
        if "continue" not in d:
            return titles
        cont = {"apcontinue": d["continue"]["apcontinue"]}


def file_name(title):
    # Même règle que le script d'import du dump Fandom : tout caractère hors lettres, chiffres, espace, _ et - devient _
    return "".join(c if c.isalnum() or c in (" ", "_", "-") else "_" for c in title) + ".txt"


def main():
    os.makedirs(OUT, exist_ok=True)
    titles = all_titles(False)
    print(f"{len(titles)} pages")
    index, used = {}, set()
    for i in range(0, len(titles), 50):
        batch = titles[i:i + 50]
        d = api(action="query", prop="revisions", rvprop="content", rvslots="main", titles="|".join(batch))
        for page in d["query"]["pages"].values():
            if "revisions" not in page:
                continue
            name = file_name(page["title"])
            # Windows ne distingue pas la casse : suffixe en cas de collision
            base, n = name, 2
            while name.lower() in used:
                name = base[:-4] + f"__{n}.txt"
                n += 1
            used.add(name.lower())
            with open(os.path.join(OUT, name), "w", encoding="utf-8") as f:
                f.write(page["revisions"][0]["slots"]["main"]["*"])
            index[page["title"]] = name
        time.sleep(0.5)
        print(f"  {min(i + 50, len(titles))}/{len(titles)}", flush=True)
    redirect_titles = all_titles(True)
    redirects = {}
    for i in range(0, len(redirect_titles), 50):
        d = api(action="query", redirects=1, titles="|".join(redirect_titles[i:i + 50]))
        for r in d["query"].get("redirects", []):
            redirects[r["from"]] = r["to"]
        time.sleep(0.3)
    json.dump(index, open(os.path.join(OUT, "index.json"), "w", encoding="utf-8"), ensure_ascii=False, indent=0)
    json.dump(redirects, open(os.path.join(OUT, "redirects.json"), "w", encoding="utf-8"), ensure_ascii=False, indent=0)
    print(f"{len(index)} pages, {len(redirects)} redirections -> {OUT}")


if __name__ == "__main__":
    main()
