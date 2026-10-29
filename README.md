# Calamity Fandom

Addon pour [Calamity Mod](https://steamcommunity.com/sharedfiles/filedetails/?id=2824688072) qui fait correspondre le
jeu au wiki Fandom de Calamity, celui qui a continué sa vie (et ses inventions) après le passage de la communauté sur
wiki.gg : objets renommés, stats modifiées, armes, bosses et biomes qui n'ont jamais existé.

## Installation pour développer

Prérequis : tModLoader (Steam), Calamity Mod abonné sur le Workshop, .NET 8 SDK, Python 3.11 + Pillow
(`python -m pip install --user pillow`).

```bash
python tools/extract_calamity_dll.py      # copie CalamityMod.dll depuis le Workshop vers lib/
python tools/wiki_index.py                # analyse le wiki et génère reports/
dotnet build CalamityFandom.csproj        # produit Mods/CalamityFandom.tmod
```

Puis activer **Calamity Mod** et **Calamity Fandom** en jeu.

## Organisation

```
CalamityFandom.cs, build.txt   mod tModLoader
Content/                       nouveau contenu tiré du wiki
Common/                        systèmes et modifications de Calamity
Localization/                  noms et descriptions
tools/                         scripts Python (indexation du wiki, import de sprites, DLL)
reports/                       état des lieux généré : à faire, écarts, renommages
data/wiki_index.json           index complet du wiki (généré)
wiki_edits/                    pages du wiki corrigées ou créées par nous
docs/                          feuille de route et décisions
fandom_dump_txt/, fandom_dump_extra/, fandom_sprites/, OtherMods/   matériel de référence, jamais empaqueté
                                                                     (OtherMods/ : sources d'autres mods, dont CalamityModPublic/)
```

Voir [`reports/README.md`](reports/README.md) pour l'avancement et [`docs/ROADMAP.md`](docs/ROADMAP.md) pour le plan.
