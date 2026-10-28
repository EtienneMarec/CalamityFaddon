"""Corpus de toutes les phrases écrites dans le code de Calamity depuis décembre 2018 (chaînes de tous les fichiers .cs et
de la localisation, un instantané par mois, plus 2.0.1.005 et la version actuelle), avec la première date où chacune
apparaît. Sert à tools/compare_wikigg.py : une ligne d'infobulle du Fandom introuvable ici n'a jamais existé dans le jeu
(du moins depuis la mise en ligne du dépôt, fin 2018).

Résultat : data/old_strings.json {"phrase": "première date"}

Usage : python tools/old_strings.py   (après tools/old_tooltips.py, dont il reprend les instantanés)
"""
import io
import json
import os
import re
import tarfile

from old_tooltips import OUT as TOOLTIPS, STRING_RE, git

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
OUT = os.path.join(ROOT, "data", "old_strings.json")
HJSON_LINE = re.compile(r"^\s*[\w.]+\s*:\s*(.+)$")


def phrases(text, hjson):
    found = set()
    if hjson:
        for line in text.splitlines():
            m = HJSON_LINE.match(line)
            value = m.group(1) if m else line.strip()
            if " " in value and len(value) > 8:
                found.add(value.strip().strip('"'))
    else:
        for verbatim, normal in STRING_RE.findall(text):
            value = verbatim or normal
            if " " in value and len(value) > 8:
                found.update(part for part in value.replace("\\n", "\n").split("\n") if len(part) > 8)
    return found


def main():
    snaps = json.load(open(TOOLTIPS, encoding="utf-8"))["snapshots"] + [{"commit": "HEAD", "date": "2.2.2"}]
    first = {}
    for snap in snaps:
        data = git("archive", "--format=tar", snap["commit"])
        count = 0
        with tarfile.open(fileobj=io.BytesIO(data)) as tar:
            for member in tar:
                hjson = member.name.endswith(".hjson")
                if not (member.name.endswith(".cs") or hjson):
                    continue
                text = tar.extractfile(member).read().decode("utf-8", errors="replace")
                for phrase in phrases(text, hjson):
                    if phrase not in first:
                        first[phrase] = snap["date"]
                        count += 1
        print(f"{snap['date']} : {count} nouvelles phrases ({len(first)} en tout)", flush=True)
    json.dump(first, open(OUT, "w", encoding="utf-8"), ensure_ascii=False, indent=0)


if __name__ == "__main__":
    main()
