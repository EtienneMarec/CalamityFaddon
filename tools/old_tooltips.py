"""Infobulles des objets de Calamity à travers ses versions (historique git de OtherMods/CalamityModPublic/), pour savoir si une
page du Fandom décrit une ancienne version d'un objet ou invente.

Instantanés : un commit par mois de décembre 2018 (début du dépôt) à fin 2022, plus 2.0.1.005 (3ec8f553ed, référence « abandon du
wiki »). Pour chacun, chaque classe d'objet (Items/**.cs) -> nom affiché et infobulle (DisplayName/Tooltip.SetDefault,
chaînes concaténées). Résultat : data/old_tooltips.json
  {"snapshots": [{"commit", "date"}...], "items": {Classe: [[index d'instantané, nom, infobulle], ...]},
   "strings": {Classe: [phrases du fichier, toutes versions]}, "seen": {Classe: [premier, dernier instantané]}}
(une entrée seulement quand le texte change d'un instantané à l'autre ; « strings » rattrape les lignes ajoutées par
ModifyTooltips).

Usage : python tools/old_tooltips.py
"""
import io
import json
import os
import re
import subprocess
import tarfile

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
REPO = os.path.join(ROOT, "OtherMods", "CalamityModPublic")
OUT = os.path.join(ROOT, "data", "old_tooltips.json")
BASELINE = "3ec8f553ed"

CLASS_RE = re.compile(r"\bclass\s+(\w+)\s*:\s*[\w.<>]+")
STRING_RE = re.compile(r'@"((?:[^"]|"")*)"|"((?:[^"\\]|\\.)*)"')


def git(*args):
    return subprocess.run(["git", "-C", REPO, *args], capture_output=True, check=True).stdout


def snapshots():
    commits = []
    for year in range(2018, 2023):
        for month in range(1, 13):
            if (year, month) < (2018, 12):
                continue
            sha = git("rev-list", "--first-parent", "-1", f"--before={year}-{month:02d}-01", "HEAD").decode().strip()
            if sha and sha not in [c["commit"] for c in commits]:
                commits.append({"commit": sha, "date": f"{year}-{month:02d}"})
    commits.append({"commit": git("rev-parse", BASELINE).decode().strip(), "date": "2.0.1.005"})
    return commits


def call_strings(text, method):
    """Chaînes concaténées passées à <method>.SetDefault(...) (première occurrence)."""
    m = re.search(re.escape(method) + r"\.SetDefault\s*\(", text)
    if not m:
        return None
    depth, i = 1, m.end()
    while i < len(text) and depth:
        c = text[i]
        if c == '"':
            # sauter la chaîne
            j = i + 1
            while j < len(text) and text[j] != '"':
                j += 2 if text[j] == "\\" else 1
            i = j
        elif c == "(":
            depth += 1
        elif c == ")":
            depth -= 1
        i += 1
    body = text[m.end():i - 1]
    parts = []
    for verbatim, normal in STRING_RE.findall(body):
        if verbatim:
            parts.append(verbatim.replace('""', '"'))
        else:
            parts.append(normal.encode("utf-8").decode("unicode_escape", errors="ignore") if "\\" in normal else normal)
    return "".join(parts)


def tooltips_at(commit):
    data = git("archive", "--format=tar", commit, "Items")
    found = {}
    with tarfile.open(fileobj=io.BytesIO(data)) as tar:
        for member in tar:
            if not member.name.endswith(".cs"):
                continue
            text = tar.extractfile(member).read().decode("utf-8", errors="replace")
            m = CLASS_RE.search(text)
            if not m:
                continue
            tooltip = call_strings(text, "Tooltip")
            name = call_strings(text, "DisplayName")
            if tooltip is None and name is None:
                continue
            # Toutes les phrases du fichier : lignes ajoutées par ModifyTooltips (touches, charge, bonus d'armure...)
            literals = {a or b for a, b in STRING_RE.findall(text) if " " in (a or b) and len(a or b) > 8}
            found[m.group(1)] = (name or "", tooltip or "", literals)
    return found


def main():
    snaps = snapshots()
    items, strings, seen = {}, {}, {}
    for index, snap in enumerate(snaps):
        found = tooltips_at(snap["commit"])
        print(f"{snap['date']} {snap['commit'][:10]} : {len(found)} objets", flush=True)
        for cls, (name, tooltip, literals) in found.items():
            history = items.setdefault(cls, [])
            if not history or history[-1][1:] != [name, tooltip]:
                history.append([index, name, tooltip])
            strings.setdefault(cls, set()).update(literals)
            seen.setdefault(cls, [index, index])[1] = index
    os.makedirs(os.path.dirname(OUT), exist_ok=True)
    json.dump({"snapshots": snaps, "items": items, "strings": {k: sorted(v) for k, v in strings.items()}, "seen": seen},
              open(OUT, "w", encoding="utf-8"), ensure_ascii=False, indent=0)
    print(f"{len(items)} classes -> {OUT}")


if __name__ == "__main__":
    main()
