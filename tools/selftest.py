"""Lance un serveur tModLoader sans interface avec CALFANDOM_SELFTEST=1 et affiche le résultat de l'auto-test
(Common/Debug/SelfTest.cs).
Écrit aussi les bilans calculés en jeu dans reports/ (ex. reports/recipes.md). Le mod doit avoir été construit (dotnet build) et le jeu fermé.

Usage : python tools/selftest.py [--mode Minimal|FullFandom|CalamityPlus|all]   (Minimal par défaut)
Les rapports de reports/ ne sont écrits qu'en mode Minimal (le mode par défaut du mod).
Code de sortie : 0 si tout passe, 1 sinon.
"""
import os
import subprocess
import sys

TML = r"C:\Program Files (x86)\Steam\steamapps\common\tModLoader"
WORKSHOP = r"C:\Program Files (x86)\Steam\steamapps\workshop"


MODES = ["Minimal", "FullFandom", "CalamityPlus"]


def main():
    mode = sys.argv[sys.argv.index("--mode") + 1] if "--mode" in sys.argv else "Minimal"
    results = [run(m) for m in (MODES if mode == "all" else [mode])]
    sys.exit(0 if all(results) else 1)


def run(mode):
    print(f"===== mode {mode}")
    reports = os.path.join(os.path.dirname(os.path.dirname(os.path.abspath(__file__))), "reports")
    env = dict(os.environ, CALFANDOM_SELFTEST="1", CALFANDOM_REPORTS=reports, CALFANDOM_MODE=mode)
    proc = subprocess.run(
        ["dotnet", "tModLoader.dll", "-server", "-steamworkshopfolder", WORKSHOP],
        cwd=TML, env=env, stdin=subprocess.DEVNULL, capture_output=True, text=True,
        encoding="utf-8", errors="replace", timeout=600)
    lines = [l for l in proc.stdout.splitlines() if "[SELFTEST]" in l]
    if not lines:
        print(proc.stdout[-3000:])
        print("Aucune sortie d'auto-test : le mod n'a pas fini de charger (voir tModLoader-Logs/server.log)")
        return False
    for l in lines:
        print(l)
    return any("RESULT PASS" in l for l in lines)


if __name__ == "__main__":
    main()
