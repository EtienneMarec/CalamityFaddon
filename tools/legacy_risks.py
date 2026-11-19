"""Portages de l'ancien Calamity à tester en détail : ceux qui réutilisent une classe de Calamity modifiée depuis.

Le code porté a fonctionné dans sa version d'origine ; ce qui peut casser, ce sont les classes de Calamity 2.2.2 qu'il
réutilise telles quelles (projectiles, buffs, PNJ, poussières, classes de base) : si Calamity les a refaites depuis la
version d'où vient le portage, le comportement n'est plus celui d'origine (cas de l'Effluvium Bow et de la Mist Arrow).

Pour chaque objet porté (classe avec « /// Wiki : » dans un fichier qui a l'en-tête « Adapté de Calamity ») : classes de
Calamity atteintes depuis l'objet (en suivant nos propres classes), puis lignes changées dans leur fichier entre la
version d'origine (commit de l'en-tête, sinon 2.0.1.005) et 2.2.2, sans le bruit (textes, using, commentaires).

Usage : python tools/legacy_risks.py   ->  reports/legacy_risks.md
"""
import glob
import os
import re
import subprocess
import sys

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from wiki_index import ROOT, CALAMITY, CLASS_DECL, load_calamity_classes  # noqa: E402
from gen_baseline import BASELINE  # noqa: E402

REPORT = os.path.join(ROOT, "reports", "legacy_risks.md")
THRESHOLD = 8  # lignes significatives changées au-delà desquelles un portage est « à risque »
# Seuls les projectiles comptent (et les classes de base) : les debuffs et les PNJ de Calamity sont voulus dans leur
# version actuelle (même debuff partout dans le jeu ; un PNJ cité n'est qu'une condition), les poussières sont visuelles.
CONTENT_KINDS = {"ModProjectile"}
DEPTH = 2  # objet -> ses projectiles -> les leurs ; au-delà, on traverse des aides communes sans rapport
# Classes de Calamity que tout le code touche (joueur, utilitaires...) : modifiées à chaque version, sans rapport
# avec un objet précis. Leurs changements d'API cassent la compilation, pas le comportement en silence.
IGNORED = {"CalamityUtils", "CalamityPlayer", "CalamityGlobalNPC", "CalamityGlobalItem", "CalamityGlobalProjectile",
           "CalamityMod", "CalamityKeybinds", "CalamityConfig", "CalamityWorld", "DownedBossSystem", "BossRushEvent",
           "CommonCalamitySounds", "CalamitySounds", "ModdedRarity"}
NOISE = re.compile(r"^\s*(//|///|using |\[|$|[{}();]+$|.*(SetDefault|DisplayName|Tooltip|SacrificeTotal|ResearchUnlockCount"
                   r"|LocalizationCategory|Language\.GetText))")
HEADER = re.compile(r"^// Adapté de Calamity Mod(.*)$", re.M)
HASH = re.compile(r"\b([0-9a-f]{10,40})\b")
WIKI = re.compile(r"///.*Wiki : ([^.\n]+)\.txt")
IDENT = re.compile(r"(?<![.\w])[A-Z]\w+\b")  # pas après un point : DustID.Blood n'est pas la classe Blood
STRIP = re.compile(r'//[^\n]*|/\*.*?\*/|@?"(?:\\.|[^"\\])*"', re.S)  # commentaires et chaînes


def git(*args):
    return subprocess.run(["git", "-C", CALAMITY, *args], capture_output=True, text=True,
                          encoding="utf-8", errors="replace").stdout


def blocks(text):
    """[(classe, base, code)] des classes du fichier (du « class » jusqu'à la classe suivante)."""
    found = [(m.start(), m.group(1), m.group(2)) for m in CLASS_DECL.finditer(text)]
    out = []
    for i, (start, name, base) in enumerate(found):
        end = found[i + 1][0] if i + 1 < len(found) else len(text)
        # le commentaire /// juste au-dessus fait partie de la classe
        head = text.rfind("\n\n", 0, start)
        doc = text[head if head >= 0 else 0:start]
        out.append((name, base, doc + text[start:end]))
    return out


def source_commit(header):
    m = HASH.search(header)
    if m:
        commit = m.group(1)
        # « retiré le ..., commit X » : la dernière version où l'objet existait est le parent
        if "retiré" in header:
            commit += "^"
        return commit
    return BASELINE


def main():
    calamity = load_calamity_classes()  # classe -> (chemin relatif à ROOT, type)
    # notre code : classe -> (code, fichier) ; nos classes masquent celles de Calamity du même nom
    ours, ported = {}, []
    for path in glob.glob(os.path.join(ROOT, "Content", "**", "*.cs"), recursive=True) + \
            glob.glob(os.path.join(ROOT, "Common", "**", "*.cs"), recursive=True):
        text = open(path, encoding="utf-8-sig", errors="replace").read()
        rel = os.path.relpath(path, ROOT).replace("\\", "/")
        header = HEADER.search(text)
        for name, base, code in blocks(text):
            ours[name] = (STRIP.sub("", code), base)
            wiki = WIKI.search(code)
            if header and wiki:
                ported.append((name, wiki.group(1), rel, source_commit(header.group(1))))

    def calamity_refs(root):
        """Classes de Calamity atteintes depuis une de nos classes : {classe: chemin}."""
        seen, todo, refs = {root}, [(root, 0)], {}
        while todo:
            current, depth = todo.pop()
            code, base = ours[current]
            for ident in set(IDENT.findall(code)) | {base.split(".")[-1]}:
                if ident in ours:
                    if ident not in seen and depth < DEPTH:
                        seen.add(ident)
                        todo.append((ident, depth + 1))
                elif ident in calamity and ident not in IGNORED:
                    path, kind = calamity[ident]
                    if kind in CONTENT_KINDS or ident == base.split(".")[-1]:
                        refs[ident] = os.path.relpath(os.path.join(ROOT, path), CALAMITY).replace("\\", "/")
        return refs

    old_paths = {}  # (commit, classe) -> chemin à ce commit

    def path_at(commit, cls, current):
        key = (commit, cls)
        if key not in old_paths:
            if subprocess.run(["git", "-C", CALAMITY, "cat-file", "-e", f"{commit}:{current}"],
                              capture_output=True).returncode == 0:
                old_paths[key] = current
            else:
                hit = git("grep", "-l", "-E", rf"class {cls}\b", commit, "--", "*.cs").splitlines()
                old_paths[key] = hit[0].split(":", 1)[1] if hit else None
        return old_paths[key]

    changes = {}  # (commit, chemin actuel) -> lignes significatives changées (None : classe absente à l'origine)

    def changed(commit, cls, current):
        key = (commit, current)
        if key not in changes:
            old = path_at(commit, cls, current)
            if old is None:
                changes[key] = None
            else:
                diff = git("diff", "-w", "-U0", f"{commit}:{old}", f"HEAD:{current}")
                changes[key] = sum(1 for line in diff.splitlines()
                                   if line[:1] in "+-" and not line.startswith(("+++", "---")) and not NOISE.match(line[1:]))
        return changes[key]

    by_page = {}  # page -> (fichier, commit, {classe: raison}) ; l'objet et ses projectiles citent la même page
    for name, page, rel, commit in ported:
        page = re.sub(r"_(?= |$)", "'", re.sub(r"_s\b", "'s", page))  # nom de fichier du dump -> titre
        entry = by_page.setdefault(page, (rel, commit, {}))
        for cls, path in calamity_refs(name).items():
            n = changed(commit, cls, path)
            if n is None:
                entry[2][cls] = f"`{cls}` (n'existait pas dans la version d'origine)"
            elif n >= THRESHOLD:
                entry[2][cls] = f"`{cls}` ({n} lignes changées)"
    risky, safe = [], []
    for page in sorted(by_page, key=str.lower):
        rel, commit, reasons = by_page[page]
        (risky if reasons else safe).append((page, rel, commit, [reasons[c] for c in sorted(reasons)]))

    lines = ["# Portages de l'ancien Calamity : lesquels tester en détail", "",
             "Généré par `tools/legacy_risks.py`, ne pas éditer. Un portage est **à risque** quand il réutilise une classe "
             "de Calamity 2.2.2 (projectile, buff, PNJ, poussière, classe de base) qui a changé de plus de "
             f"{THRESHOLD} lignes significatives depuis la version d'où il vient, ou qui n'existait pas alors. "
             "Les autres n'ont besoin que du test rapide (`docs/TESTS.md`).", "",
             "Limite : sans commit dans l'en-tête du fichier, la version d'origine est 2.0.1.005 ; un portage plus ancien "
             "peut donc avoir des changements de 2022 non vus.", "",
             f"## À risque ({len(risky)})", ""]
    for page, rel, commit, reasons in risky:
        lines.append(f"- [ ] **{page}** (`{rel}`, depuis `{commit}`) : " + ", ".join(reasons))
    lines += ["", f"## Test rapide seulement ({len(safe)})", ""]
    lines.append(", ".join(page for page, *_ in safe))
    os.makedirs(os.path.dirname(REPORT), exist_ok=True)
    open(REPORT, "w", encoding="utf-8", newline="\n").write("\n".join(lines) + "\n")
    print(f"{len(risky)} à risque, {len(safe)} test rapide -> {os.path.relpath(REPORT, ROOT)}")


if __name__ == "__main__":
    main()
