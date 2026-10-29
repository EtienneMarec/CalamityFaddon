"""Indexe notre wiki (wiki/, copie éditée du wiki Fandom) et le compare au code de Calamity.

Produit :
  data/wiki_index.json        une entrée par page (type, infobox, correspondance Calamity, écarts)
  reports/README.md           synthèse chiffrée
  reports/new_content.md      contenu absent de Calamity (à créer), groupé par type d'infobox
  reports/stat_drift.md       contenu officiel dont les stats du wiki diffèrent du code
  reports/addons.md           pages marquées {{Addon|...}}
  reports/renames.md          objets officiels renommés par le wiki (détectés via les redirections)

Une page est marquée « edited » si elle diffère de la page d'origine dans fandom_dump_txt/.

Usage : python tools/wiki_index.py
"""
import collections
import glob
import json
import os
import re

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
# fandom_dump_txt/ : le wiki Fandom d'origine, jamais modifié (référence du mode Full Fandom)
FANDOM = os.path.join(ROOT, "fandom_dump_txt")
WIKI = FANDOM  # ancien nom, gardé pour init_wiki.py
# wiki/ : notre wiki (copie du Fandom éditée au fil du projet, voir docs/WIKI.md) ; c'est lui que les outils lisent
OUR_WIKI = os.path.join(ROOT, "wiki")
OUR_MAIN = os.path.join(OUR_WIKI, "Main")
SPRITES = os.path.join(ROOT, "fandom_sprites")
# OtherMods/ : sources d'autres mods utiles au développement (ignoré par git, jamais compilé ni empaqueté)
OTHER_MODS = os.path.join(ROOT, "OtherMods")
CALAMITY = os.path.join(OTHER_MODS, "CalamityModPublic")
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

_TITLES = None


def file_to_title(stem):
    """Retrouve le vrai titre de la page à partir du nom de fichier du dump.

    Le dump remplace la ponctuation par « _ » (« : », « , », « ' », « ( »...) : on cherche le titre exact dans
    fandom_dump_extra/titles.json (liste de l'API), sinon on reconstitue au mieux.
    """
    global _TITLES
    if _TITLES is None:
        _TITLES = {}
        path = os.path.join(ROOT, "fandom_dump_extra", "titles.json")
        if os.path.exists(path):
            for title in json.load(open(path, encoding="utf-8")):
                _TITLES.setdefault(norm(title), []).append(title)
    candidates = _TITLES.get(norm(stem), [])
    if len(candidates) > 1:
        # Homonymes à la casse près : celui dont les lettres correspondent exactement
        letters = re.sub(r"[^0-9A-Za-z]", "", stem)
        candidates = [c for c in candidates if re.sub(r"[^0-9A-Za-z]", "", c) == letters] or candidates
    if candidates:
        return candidates[0]
    t = stem
    t = re.sub(r"_s\b", "'s", t)          # Arkhalis_s -> Arkhalis's
    t = re.sub(r" _([^_]+)_$", r" (\1)", t)  # Dragon Egg _pet_ -> Dragon Egg (pet)
    t = t.replace("_ ", ": ")              # Godseeker_ Yharim -> Godseeker: Yharim
    t = t.replace("_", "/")                # Recipes_Anvil -> Recipes/Anvil
    return t


MODE_START = re.compile(r"\{\{\s*mode\s*\|", re.I)


def _split_mode(raw, start):
    """(fin, [paramètres]) du {{mode|...}} qui commence à `start` ; les modèles imbriqués ({{dv}}...) sont respectés."""
    depth, i, params, cur = 0, start, [], []
    i = raw.index("|", start) + 1
    while i < len(raw):
        two = raw[i:i + 2]
        if two == "{{":
            depth += 1
            cur.append(two)
            i += 2
            continue
        if two == "}}":
            if depth == 0:
                params.append("".join(cur))
                return i + 2, params
            depth -= 1
            cur.append(two)
            i += 2
            continue
        if raw[i] == "|" and depth == 0:
            params.append("".join(cur))
            cur = []
        else:
            cur.append(raw[i])
        i += 1
    return len(raw), params + ["".join(cur)]


def pick_mode(raw, mode):
    """Résout {{mode|valeur Full Fandom|valeur Calamity++}} (wiki/Template/Template_Mode.txt) pour mode « fandom » ou
    « plus » ; un texte sans {{mode}} est renvoyé tel quel (même valeur dans les deux modes)."""
    if raw is None:
        return None
    out, pos = [], 0
    for m in MODE_START.finditer(raw):
        if m.start() < pos:
            continue
        end, params = _split_mode(raw, m.start())
        out.append(raw[pos:m.start()])
        value = params[0] if mode == "fandom" or len(params) < 2 else params[1]
        out.append(value.strip())
        pos = end
    out.append(raw[pos:])
    return "".join(out)


def has_mode(raw):
    return bool(raw and MODE_START.search(raw))


def norm(name):
    return re.sub(r"[^a-z0-9]", "", name.lower())


# --------------------------------------------------------------------------- Calamity

def calamity_display_names():
    """Liste de (nom affiché brut, catégorie de localisation, nom de classe), depuis les hjson de Calamity."""
    entries = []
    for path in glob.glob(os.path.join(LOCALIZATION, "Mods.CalamityMod.*.hjson")):
        category = os.path.basename(path)[len("Mods.CalamityMod."):-len(".hjson")]
        stack = []
        for line in open(path, encoding="utf-8-sig"):
            s = line.strip()
            m = re.match(r"^([\w.]+)\.DisplayName:\s*(.+)$", s)
            if m:
                entries.append((m.group(2).strip('"'), category, m.group(1).split(".")[-1]))
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
                entries.append((m.group(1).strip('"'), category, stack[-1].split(".")[-1]))
    return entries


def load_calamity_names():
    """DisplayName -> liste de (catégorie de localisation, nom de classe)."""
    names = collections.defaultdict(list)
    for display, category, cls in calamity_display_names():
        names[norm(display)].append((category, cls))
    return names


CLASS_DECL = re.compile(r"\bclass\s+(\w+)\s*(?:<[^>{]*>)?\s*:\s*([\w.]+)")


def resolve_mod_types(declarations):
    """{classe: (base, chemin)} -> {classe: (chemin, type tModLoader)} en suivant l'héritage jusqu'à un Mod*.

    Beaucoup d'objets de Calamity héritent d'une classe intermédiaire (RogueWeapon, BaseSwordHoldoutItem...) :
    on remonte la chaîne. Les classes abstraites de base sont gardées aussi, sans conséquence (pas de DisplayName).
    """
    resolved = {}
    for cls, (base, path) in declarations.items():
        seen, current = set(), base.split(".")[-1]
        while current in declarations and current not in seen:
            seen.add(current)
            current = declarations[current][0].split(".")[-1]
        if current.startswith("Mod") or current.startswith("Global"):
            resolved[cls] = (path, current)
    return resolved


def load_calamity_classes():
    """Nom de classe -> (chemin du .cs, type tModLoader : ModItem, ModNPC...), héritage indirect compris."""
    declarations = {}
    for path in glob.glob(os.path.join(CALAMITY, "**", "*.cs"), recursive=True):
        try:
            text = open(path, encoding="utf-8-sig", errors="replace").read()
        except OSError:
            continue
        for m in CLASS_DECL.finditer(text):
            declarations.setdefault(m.group(1), (m.group(2), os.path.relpath(path, ROOT)))
    return resolve_mod_types(declarations)


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
    # Objets générés sans classe dédiée (tools/gen_items.py)
    generated = os.path.join(ROOT, "WikiData", "items.json")
    if os.path.exists(generated):
        generated_data = json.load(open(generated, encoding="utf-8"))
        for entry in generated_data.get("materials", []) + generated_data.get("wings", []):
            done.setdefault(entry["page"], "WikiData/items.json")
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
    cal_classes = load_calamity_classes()
    # Noms restés dans la localisation de Calamity sans classe derrière (objets retirés, ex. Beetle Juice) : ignorés,
    # sinon la page passerait pour « officielle » alors que l'objet est à porter
    cal_names = {name: kept for name, matches in load_calamity_names().items()
                 if (kept := [(cat, cls) for cat, cls in matches if cls in cal_classes])}
    sprites = {norm(os.path.splitext(f)[0]): f for f in os.listdir(SPRITES)}
    implemented = load_implemented()

    # Notre wiki : wiki/Main ; titres exacts dans wiki/index.json. « edited » = différente de la page du Fandom.
    titles = json.load(open(os.path.join(OUR_WIKI, "index.json"), encoding="utf-8"))
    index = {}
    for fname in sorted(f for f in os.listdir(OUR_MAIN) if f.endswith(".txt")):
        stem = fname[:-4]
        text = open(os.path.join(OUR_MAIN, fname), encoding="utf-8", errors="replace").read()
        page = parse_page(text)
        title = titles.get(f"Main/{fname}") or file_to_title(stem)
        page["file"] = fname
        page["title"] = title
        original = os.path.join(FANDOM, fname)
        if not os.path.exists(original) or open(original, encoding="utf-8", errors="replace").read() != text:
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
            # Une page d'objet ne décrit qu'un objet, une page de PNJ qu'un PNJ (« Energy Orb » est aussi un projectile)
            box = page.get("infobox") or ""
            if box.startswith("item"):
                matches = [m for m in matches if m[0].startswith("Items")]
            elif box.startswith("npc"):
                matches = [m for m in matches if m[0].startswith("NPCs")]
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
    # Renommages sans redirection, vérifiés à la main (tools/glossary.json → renames, ex. The Etomer ->
    # Triboluminescent Etomer) : la page n'est pas du nouveau contenu
    glossary = json.load(open(os.path.join(ROOT, "tools", "glossary.json"), encoding="utf-8"))
    for old, new in glossary["renames"].items():
        matches = cal_names.get(norm(old), [])
        target = by_title.get(norm(new))
        if matches and target and target["status"] == "new":
            target["status"] = "renamed"
            target["renamed_from"] = [old]
            attach_calamity(target, matches, cal_classes)
    # Renommages de Calamity depuis 2.0.1.005 (historique git, tools/calamity_renames.py) : la page du wiki décrit
    # l'objet actuel sous son ancien nom (Essence of Chaos = Essence of Havoc)
    from calamity_renames import load as load_renames, wiki_known  # import tardif : calamity_renames importe ce module
    for r in wiki_known(load_renames(), {norm(p["title"]): p for p in index.values()}):
        target = by_title.get(norm(r["page"]))
        matches = [m for m in cal_names.get(norm(r["newName"]), []) if m[1] == r["new"]]
        if target and matches and target["status"] == "new":
            target["status"] = "renamed"
            target["renamed_from"] = [r["newName"]]
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
        f.write(f"| pages de wiki/ modifiées ou créées par rapport au Fandom | {sum(1 for p in pages if p.get('edited'))} |\n")
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
