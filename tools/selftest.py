"""Lance un serveur tModLoader sans interface avec CALFANDOM_SELFTEST=1 et affiche le résultat de l'auto-test
(Common/Debug/SelfTest.cs). Le mod doit avoir été construit (dotnet build) et le jeu fermé.

Usage : python tools/selftest.py
Code de sortie : 0 si tout passe, 1 sinon.
"""
import os
import subprocess
import sys

TML = r"C:\Program Files (x86)\Steam\steamapps\common\tModLoader"
WORKSHOP = r"C:\Program Files (x86)\Steam\steamapps\workshop"


def main():
    env = dict(os.environ, CALFANDOM_SELFTEST="1")
    proc = subprocess.run(
        ["dotnet", "tModLoader.dll", "-server", "-steamworkshopfolder", WORKSHOP],
        cwd=TML, env=env, stdin=subprocess.DEVNULL, capture_output=True, text=True,
        encoding="utf-8", errors="replace", timeout=600)
    lines = [l for l in proc.stdout.splitlines() if "[SELFTEST]" in l]
    if not lines:
        print(proc.stdout[-3000:])
        print("Aucune sortie d'auto-test : le mod n'a pas fini de charger (voir tModLoader-Logs/server.log)")
        sys.exit(1)
    for l in lines:
        print(l)
    sys.exit(0 if any("RESULT PASS" in l for l in lines) else 1)


if __name__ == "__main__":
    main()
