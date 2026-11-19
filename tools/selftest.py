"""Lance un serveur tModLoader sans interface avec CALFANDOM_SELFTEST=1 et affiche le résultat de l'auto-test
(Common/Debug/SelfTest.cs).
Écrit aussi les bilans calculés en jeu dans reports/ (ex. reports/recipes.md). Le mod doit avoir été construit (dotnet build) et le jeu fermé.

Usage : python tools/selftest.py [--mode Minimal|FullFandom|CalamityPlus|all]   (CalamityPlus par défaut ; all = les trois couches)
Les rapports de reports/ ne sont écrits qu'en mode CalamityPlus (le mode par défaut du mod).
Code de sortie : 0 si tout passe, 1 sinon.
"""
import os
import subprocess
import time
import sys

TML = r"C:\Program Files (x86)\Steam\steamapps\common\tModLoader"
WORKSHOP = r"C:\Program Files (x86)\Steam\steamapps\workshop"


MODES = ["Minimal", "FullFandom", "CalamityPlus"]


def stale_tmod():
    """Vrai si le .tmod est plus ancien qu'un fichier source : le build a échoué (souvent TML003, jeu ouvert) et
    l'auto-test testerait l'ancienne version du mod."""
    root = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
    tmod = os.path.join(os.path.dirname(os.path.dirname(root)), "Mods", "CalamityFandom.tmod")
    if not os.path.exists(tmod):
        return True
    built = os.path.getmtime(tmod)
    for folder in ("Common", "Content", "WikiData", "Localization", "Assets", "Balance"):
        for dirpath, _, files in os.walk(os.path.join(root, folder)):
            if any(os.path.getmtime(os.path.join(dirpath, f)) > built for f in files):
                return True
    return False


def broken_localization():
    """Premier fichier hjson qui ne se lit pas (message d'erreur), ou None. tModLoader refuse alors de charger le mod."""
    import hjson
    root = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
    folder = os.path.join(root, "Localization", "en-US")
    for name in sorted(os.listdir(folder)):
        if name.endswith(".hjson"):
            try:
                hjson.load(open(os.path.join(folder, name), encoding="utf-8"))
            except Exception as e:  # noqa: BLE001 — toute erreur de syntaxe
                return f"{name} : {e}"
    return None


def main():
    enabled = os.path.join(os.path.dirname(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))), "Mods", "enabled.json")
    if os.path.exists(enabled) and "CalamityFandom" not in open(enabled, encoding="utf-8").read():
        sys.exit("CalamityFandom n'est pas activé (Mods/enabled.json) : tModLoader le désactive après un plantage au chargement.")
    if stale_tmod():
        sys.exit("Le .tmod est plus ancien que les sources : relancer dotnet build avec le jeu fermé (erreur TML003 sinon).")
    bad = broken_localization()
    if bad:
        sys.exit("Fichier de localisation invalide (le mod ne se chargerait pas, le serveur attendrait 10 min) : " + bad)
    mode = sys.argv[sys.argv.index("--mode") + 1] if "--mode" in sys.argv else "CalamityPlus"
    results = [run(m) for m in (MODES if mode == "all" else [mode])]
    sys.exit(0 if all(results) else 1)


def run(mode):
    print(f"===== mode {mode}")
    reports = os.path.join(os.path.dirname(os.path.dirname(os.path.abspath(__file__))), "reports")
    env = dict(os.environ, CALFANDOM_SELFTEST="1", CALFANDOM_REPORTS=reports, CALFANDOM_MODE=mode)
    # Lecture ligne à ligne : on arrête le serveur dès que le chargement du mod échoue (sinon il attendrait 10 minutes)
    proc = subprocess.Popen(
        ["dotnet", "tModLoader.dll", "-server", "-steamworkshopfolder", WORKSHOP],
        cwd=TML, env=env, stdin=subprocess.DEVNULL, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, text=True,
        encoding="utf-8", errors="replace")
    output, failed = [], None
    deadline = time.time() + 600
    for line in proc.stdout:
        output.append(line.rstrip("\n"))
        if "An error occurred while loading" in line or "Cannot add two" in line:
            failed = line.strip()
        if failed and len(output) > 400:
            break
        if failed and "error was caused by" in line:
            failed += " / " + line.strip()
            break
        if time.time() > deadline:
            failed = "délai de 10 minutes dépassé"
            break
    if proc.poll() is None:
        proc.kill()
    proc.wait()
    lines = [l for l in output if "[SELFTEST]" in l]
    if failed or not lines:
        print("\n".join(output[-40:]))
        print("Le mod n'a pas chargé : " + (failed or "aucune sortie d'auto-test") + " (voir tModLoader-Logs/server.log)")
        return False
    for l in lines:
        print(l)
    return any("RESULT PASS" in l for l in lines)


if __name__ == "__main__":
    main()
