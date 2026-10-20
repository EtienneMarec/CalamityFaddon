"""Indexe le dump du wiki Fandom et le compare au code de Calamity.

Produit :
  data/wiki_index.json        une entrée par page (type, infobox, correspondance Calamity, écarts)
  reports/README.md           synthèse chiffrée
  reports/new_content.md      contenu absent de Calamity (à créer), groupé par type d'infobox
  reports/stat_drift.md       contenu officiel dont les stats du wiki diffèrent du code
  reports/addons.md           pages marquées {{Addon|...}}
  reports/renames.md          objets officiels renommés par le wiki (détectés via les redirections)

Les pages de fandom_dump_extra/Main/ (pages écrasées dans le dump, cf. fetch_missing_pages.py) s'ajoutent au dump,
et celles de wiki_edits/ (même nommage) remplacent tout le reste.

Usage : python -I tools/wiki_index.py
"""
import collections
import glob
import json
import os
import re

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
WIKI = os.path.join(ROOT, "fandom_dump_txt")
EXTRA_MAIN = os.path.join(ROOT, "fandom_dump_extra", "Main")
EDITS = os.path.join(ROOT, "wiki_edits")
SPRITES = os.path.join(ROOT, "fandom_sprites")
CALAMITY = os.path.join(ROOT, "CalamityModPublic")
LOCALIZATION = os.path.join(CALAMITY, "Localization", "en-US")

INFOBOX_RE = re.compile(r"\{\{\s*(item|npc|enemy|boss|buff|tile|armor) infobox", re.I)

# Champs d'infobox comparés au code : champ wiki -> (champ C#, type de classe)
ITEM_FIELDS = {
    "damage": "damage",
    "use": "useAnimation",
    "knockback": "knockBack",
    "critical": "crit",
    "defense": "defense",
    "rare": "rare",
    "velocity": "shootSpeed",
    "mana": "mana",
}
NPC_FIELDS = {
    "life": "lifeMax",
    "damage": "damage",
    "defense": "defense",
}


# --------------------------------------------------------------------------- titres

def file_to_title(stem):
    """Reconstitue le titre de la page à partir du nom de fichier du dump."""
    t = stem
    t = re.sub(r"_s\b", "'s", t)          # Arkhalis_s -> Arkhalis's
    t = re.sub(r" _([^_]+)_$", r" (\1)", t)  # Dragon Egg _pet_ -> Dragon Egg (pet)
    t = t.replace("_ ", ": ")              # Godseeker_ Yharim -> Godseeker: Yharim
    t = t.replace("_", "/")                # Recipes_Anvil -> Recipes/Anvil
    return t


def norm(name):
    return re.sub(r"[^a-z0-9]", "", name.lower())


# --------------------------------------------------------------------------- Calamity

def load_calamity_names():
    """DisplayName -> liste de (catégorie de localisation, nom de classe)."""
    names = collections.defaultdict(list)
    for path in glob.glob(os.path.join(LOCALIZATION, "Mods.CalamityMod.*.hjson")):
        category = os.path.basename(path)[len("Mods.CalamityMod."):-len(".hjson")]
        stack = []
        for line in open(path, encoding="utf-8-sig"):
            s = line.strip()
            m = re.match(r"^([\w.]+)\.DisplayName:\s*(.+)$", s)
            if m:
                names[norm(m.group(2).strip('"'))].append((category, m.group(1).split(".")[-1]))
                continue
            m = re.match(r"^([\w.]+):\s*\{\s*$", s)
            if m:
                stack.append(m.group(1))
                continue
            if s.startswith("}") and stack:
                stack.pop()
                continue
            m = re.match(r"^DisplayName:\s*(.+)$", s)
            if m and stack:
                names[norm(m.group(1).strip('"'))].append((category, stack[-1].split(".")[-1]))
    return names


def load_calamity_classes():
    """Nom de classe -> chemin du .cs pour les ModItem / ModNPC / ModBuff / ModTile."""
    classes = {}
    pattern = re.compile(r"\bclass\s+(\w+)\s*:\s*(Mod\w+)")
    for path in glob.glob(os.path.join(CALAMITY, "**", "*.cs"), recursive=True):
        try:
            text = open(path, encoding="utf-8-sig", errors="replace").read()
        except OSError:
            continue
        for m in pattern.finditer(text):
            classes.setdefault(m.group(1), (os.path.relpath(path, ROOT), m.group(2)))
    return classes


def code_stats(path, kind):
    text = open(os.path.join(ROOT, path), encoding="utf-8-sig", errors="replace").read()
    prefix = "NPC" if kind == "ModNPC" else "Item"
    fields = NPC_FIELDS if kind == "ModNPC" else ITEM_FIELDS
    stats = {}
    for wiki_field, cs_field in fields.items():
        m = re.search(rf"\b{prefix}\.{cs_field}\s*=\s*(-?\d+(?:\.\d+)?)f?\s*;", text)
        if m:
            stats[wiki_field] = float(m.group(1))
    if kind == "ModNPC":
        m = re.search(r"LifeMaxNERB\(\s*(\d+)", text)
        if m:
            stats.setdefault("life", float(m.group(1)))
    return stats


# --------------------------------------------------------------------------- wikitexte

def extract_template(text, start):
    """Renvoie le contenu du modèle {{...}} qui commence à `start` (accolades équilibrées)."""
    depth, i = 0, start
    while i < len(text) - 1:
        pair = text[i:i + 2]
        if pair == "{{":
            depth += 1
            i += 2
            continue
        if pair == "}}":
            depth -= 1
            i += 2
            if depth == 0:
                return text[start + 2:i - 2]
            continue
        i += 1
    return text[start + 2:]


def split_params(body):
    """Découpe les paramètres d'un modèle au premier niveau (ignore les | imbriqués)."""
    parts, depth, cur, i = [], 0, [], 0
    while i < len(body):
        two = body[i:i + 2]
        if two in ("{{", "[["):
            depth += 1
            cur.append(two)
            i += 2
            continue
        if two in ("}}", "]]"):
            depth -= 1
            cur.append(two)
            i += 2
            continue
        if body[i] == "|" and depth == 0:
            parts.append("".join(cur))
            cur = []
        else:
            cur.append(body[i])
        i += 1
    parts.append("".join(cur))
    params = {}
    for p in parts[1:]:
        if "=" in p:
            k, v = p.split("=", 1)
            params[k.strip().lower()] = v.strip()
    return parts[0].strip(), params


def first_number(value):
    if value is None:
        return None
    m = re.search(r"-?\d[\d,]*(?:\.\d+)?", value)
    if not m:
        return None
    return float(m.group(0).replace(",", ""))


def parse_page(text):
    info = {}
    m = re.match(r"\s*#redirect\s*\[\[([^\]|#]+)", text, re.I)
    if m:
        return {"kind": "redirect", "target": m.group(1).strip()}
    info["kind"] = "content"
    m = INFOBOX_RE.search(text)
    if m:
        name, params = split_params(extract_template(text, m.start()))
        info["infobox"] = name.lower()
        info["type"] = params.get("type", "").strip() or None
        info["fields"] = {k: v for k, v in params.items() if len(v) < 300}
    info["addons"] = sorted({a.strip() for a in re.findall(r"\{\{\s*addon\s*\|([^}|]+)", text, re.I)})
    info["fantags"] = sorted({a.strip() for a in re.findall(r"\{\{\s*fantag\s*\|([^}|]+)", text, re.I)})
    info["categories"] = sorted({c.strip() for c in re.findall(r"\[\[Category:([^\]|]+)", text)})
    info["stub"] = bool(re.search(r"\{\{\s*stub", text, re.I))
    info["length"] = len(text)
    return info


def load_implemented():
    """Pages déjà traitées : tout .cs du mod contenant « Wiki : <fichier>.txt » dans un commentaire."""
    done = {}
    paths = glob.glob(os.path.join(ROOT, "Content", "**", "*.cs"), recursive=True) + \
        glob.glob(os.path.join(ROOT, "Common", "**", "*.cs"), recursive=True)
    for path in paths:
        for m in re.finditer(r"Wiki\s*:\s*([^\n]+?\.txt)", open(path, encoding="utf-8-sig").read()):
            done.setdefault(m.group(1).strip(), os.path.relpath(path, ROOT).replace(os.sep, "/"))
    return done


def attach_calamity(page, matches, cal_classes):
    page["calamity"] = [{"category": c, "class": k, "file": cal_classes.get(k, (None,))[0]} for c, k in matches]
    # Écarts de stats : on compare avec la première classe trouvée
    cls = next((cal_classes[k] for _, k in matches if k in cal_classes), None)
    if not cls or not page.get("fields"):
        return
    code = code_stats(cls[0], cls[1])
    fields = NPC_FIELDS if cls[1] == "ModNPC" else ITEM_FIELDS
    drift = {}
    for f in fields:
        w = first_number(page["fields"].get(f))
        c = code.get(f)
        if w is not None and c is not None and abs(w - c) > 1e-6:
            drift[f] = {"wiki": w, "code": c}
    if drift:
        page["drift"] = drift


# --------------------------------------------------------------------------- main

def main():
    cal_names = load_calamity_names()
    cal_classes = load_calamity_classes()
    sprites = {norm(os.path.splitext(f)[0]): f for f in os.listdir(SPRITES)}
    implemented = load_implemented()

    # Priorité : wiki_edits/ (nos corrections) > fandom_dump_extra/Main (pages perdues du dump) > dump d'origine
    sources = {f: WIKI for f in os.listdir(WIKI) if f.endswith(".txt")}
    if os.path.isdir(EXTRA_MAIN):
        sources.update({f: EXTRA_MAIN for f in os.listdir(EXTRA_MAIN) if f.endswith(".txt")})
    if os.path.isdir(EDITS):
        sources.update({f: EDITS for f in os.listdir(EDITS) if f.endswith(".txt")})

    index = {}
    for fname in sorted(sources):
        stem = fname[:-4]
        text = open(os.path.join(sources[fname], fname), encoding="utf-8", errors="replace").read()
        page = parse_page(text)
        title = file_to_title(stem)
        page["file"] = fname
        page["title"] = title
        if sources[fname] == EDITS:
            page["edited"] = True
        if fname in implemented:
            page["implemented"] = implemented[fname]
        if re.match(r"^\d+(_\d+)+", stem):
            page["kind"] = "changelog" if page["kind"] == "content" else page["kind"]
        if page["kind"] == "content":
            sprite = sprites.get(norm(title))
            if sprite:
                page["sprite"] = sprite
            matches = cal_names.get(norm(title), [])
            if matches:
                attach_calamity(page, matches, cal_classes)
            page["status"] = "official" if matches else "new"
        index[stem] = page

    # Renommages : une redirection dont la source est un nom officiel et dont la cible
    # est une page de contenu inconnue de Calamity (ex. Murasama -> Graydron).
    by_title = {norm(p["title"]): p for p in index.values() if p["kind"] == "content"}
    for page in list(index.values()):
        if page["kind"] != "redirect":
            continue
        matches = cal_names.get(norm(page["title"]), [])
        target = by_title.get(norm(page["target"]))
        if matches and target and target["status"] in ("new", "renamed", "group"):
            target["status"] = "renamed"
            target.setdefault("renamed_from", []).append(page["title"])
            if "calamity" not in target:
                attach_calamity(target, matches, cal_classes)
    # Plusieurs noms officiels vers une même page = page de regroupement (Beds, Aerospec armor...)
    for page in index.values():
        if page.get("status") == "renamed" and len(page["renamed_from"]) > 1:
            page["status"] = "group"

    os.makedirs(os.path.join(ROOT, "data"), exist_ok=True)
    with open(os.path.join(ROOT, "data", "wiki_index.json"), "w", encoding="utf-8") as f:
        json.dump(index, f, ensure_ascii=False, indent=1)

    write_reports(index)


def write_reports(index):
    out = os.path.join(ROOT, "reports")
    os.makedirs(out, exist_ok=True)
    pages = list(index.values())
    content = [p for p in pages if p["kind"] == "content"]
    new = [p for p in content if p["status"] == "new" and p.get("infobox")]
    official = [p for p in content if p["status"] == "official"]
    renamed = [p for p in content if p["status"] == "renamed"]
    drift = [p for p in official + renamed if p.get("drift")]
    addons = [p for p in content if p["addons"] or p["fantags"]]

    kinds = collections.Counter(p["kind"] for p in pages)
    with open(os.path.join(out, "README.md"), "w", encoding="utf-8") as f:
        f.write("# Synthèse du dump Fandom\n\n_Généré par `tools/wiki_index.py`, ne pas éditer à la main._\n\n")
        f.write("| Catégorie | Pages |\n|---|---|\n")
        for k, v in kinds.most_common():
            f.write(f"| {k} | {v} |\n")
        f.write(f"| contenu correspondant à Calamity | {len(official)} |\n")
        f.write(f"| contenu officiel renommé (via redirection) | {len(renamed)} |\n")
        f.write(f"| pages regroupant plusieurs objets officiels | {sum(1 for p in content if p['status'] == 'group')} |\n")
        f.write(f"| officiel ou renommé avec stats divergentes | {len(drift)} |\n")
        f.write(f"| contenu inconnu de Calamity avec infobox | {len(new)} |\n")
        f.write(f"| dont avec sprite trouvé | {sum(1 for p in new if p.get('sprite'))} |\n")
        f.write(f"| pages marquées addon / fantag | {len(addons)} |\n")
        f.write(f"| pages corrigées/créées dans wiki_edits/ | {sum(1 for p in pages if p.get('edited'))} |\n")
        f.write(f"| **pages implémentées dans le mod** | **{sum(1 for p in pages if p.get('implemented'))}** |\n\n")
        f.write("## Types d'infobox (contenu nouveau)\n\n| Type | Nombre |\n|---|---|\n")
        for t, n in collections.Counter((p.get("type") or "?") for p in new).most_common():
            f.write(f"| {t} | {n} |\n")

    with open(os.path.join(out, "new_content.md"), "w", encoding="utf-8") as f:
        f.write("# Contenu absent de Calamity\n\n_Généré par `tools/wiki_index.py`._ Pages avec infobox dont le titre ne correspond à aucun DisplayName de Calamity. "
                "Attention : des objets vanilla ou des pages renommées peuvent s'y glisser.\n\n")
        groups = collections.defaultdict(list)
        for p in new:
            groups[p.get("type") or "?"].append(p)
        for t in sorted(groups, key=lambda k: -len(groups[k])):
            f.write(f"## {t} ({len(groups[t])})\n\n")
            for p in sorted(groups[t], key=lambda p: p["title"]):
                flags = []
                if not p.get("sprite"):
                    flags.append("pas de sprite")
                if p["stub"]:
                    flags.append("stub")
                if p["addons"] or p["fantags"]:
                    flags.append("addon: " + ", ".join(p["addons"] + p["fantags"]))
                extra = f" — {'; '.join(flags)}" if flags else ""
                box = "x" if p.get("implemented") else " "
                f.write(f"- [{box}] {p['title']}{extra}\n")
            f.write("\n")

    with open(os.path.join(out, "renames.md"), "w", encoding="utf-8") as f:
        f.write("# Contenu officiel renommé par le wiki\n\n_Généré par `tools/wiki_index.py`._ Détecté via les redirections nom officiel -> nouvelle page.\n\n")
        f.write("| Nouveau nom (wiki) | Ancien nom | Classe Calamity |\n|---|---|---|\n")
        for p in sorted(renamed, key=lambda p: p["title"]):
            f.write(f"| {p['title']} | {', '.join(p['renamed_from'])} | {p['calamity'][0]['class']} |\n")

    with open(os.path.join(out, "stat_drift.md"), "w", encoding="utf-8") as f:
        f.write("# Contenu officiel modifié par le wiki\n\n_Généré par `tools/wiki_index.py`._ Comparaison des champs d'infobox avec les valeurs littérales de `SetDefaults`. "
                "Les valeurs calculées dans le code (multiplicateurs, mode de difficulté) ne sont pas détectées : vérifier avant d'appliquer.\n\n")
        f.write("| Page | Classe | Champ | Wiki | Code |\n|---|---|---|---|---|\n")
        for p in sorted(drift, key=lambda p: p["title"]):
            cls = p["calamity"][0]["class"]
            for field, d in p["drift"].items():
                f.write(f"| {p['title']} | {cls} | {field} | {d['wiki']:g} | {d['code']:g} |\n")

    with open(os.path.join(out, "addons.md"), "w", encoding="utf-8") as f:
        f.write("# Pages marquées addon\n\n_Généré par `tools/wiki_index.py`._\n\n")
        groups = collections.defaultdict(list)
        for p in addons:
            for a in p["addons"] + p["fantags"]:
                groups[a].append(p["title"])
        for a in sorted(groups, key=lambda k: -len(groups[k])):
            f.write(f"## {a} ({len(groups[a])})\n\n" + "".join(f"- {t}\n" for t in sorted(set(groups[a]))) + "\n")

    print(f"{len(pages)} pages indexées : {len(official)} officielles ({len(drift)} divergentes), {len(new)} nouvelles avec infobox, {len(addons)} addon.")


if __name__ == "__main__":
    main()
