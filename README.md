# Calamity Fandom

Addon for [Calamity Mod](https://steamcommunity.com/sharedfiles/filedetails/?id=2824688072) that makes the game match
the Calamity Fandom wiki, the one that kept living (and inventing) after the community moved to wiki.gg: renamed
items, changed stats, weapons, bosses and biomes that never existed.

## Development setup

Requirements: tModLoader (Steam), Calamity Mod subscribed on the Workshop, .NET 8 SDK, Python 3.11 + Pillow
(`python -m pip install --user pillow`).

```bash
python tools/extract_calamity_dll.py      # copies CalamityMod.dll from the Workshop to lib/
python tools/wiki_index.py                # analyzes the wiki and generates reports/
dotnet build CalamityFandom.csproj        # produces Mods/CalamityFandom.tmod
```

Then enable **Calamity Mod** and **Calamity Fandom** in game.

## Layout

```
CalamityFandom.cs, build.txt   tModLoader mod
Content/                       new content taken from the wiki
Common/                        systems and changes to Calamity
Localization/                  names and descriptions
tools/                         Python scripts (wiki indexing, sprite import, DLL)
reports/                       generated status: to do, gaps, renames
data/wiki_index.json           full index of the wiki (generated)
wiki/                          our wiki: copy of the Fandom, corrected and extended for the mod
docs/                          roadmap and decisions
fandom_dump_txt/, fandom_dump_extra/, fandom_sprites/, OtherMods/   reference material, never packaged
                                                                     (OtherMods/: sources of other mods, including CalamityModPublic/)
```

See [`reports/README.md`](reports/README.md) for progress and [`docs/ROADMAP.md`](docs/ROADMAP.md) for the plan.
