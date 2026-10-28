# Fandom / wiki officiel : objets qui fonctionnent différemment

_Généré par `tools/compare_wikigg.py` (infobulles du Fandom, du wiki officiel et des versions du code depuis 2019). Revue à la main : `docs/DIFF_FANDOM.md`._

- inventée : 27
- mélange : 24
- ancienne version : 13
- refonte : 268
- modifiée : 268
- valeurs : 50
- mélange de versions : 33
- à vérifier : 0
- homonyme : 2
- addon : 1
- ambiance : 9
- reformulation : 2
- reformulée : 83
- formulation : 5

## Inventées (revue à la main) : le Fandom décrit un fonctionnement qui n'a jamais existé dans Calamity

### Alchemical Flask (Accessories)

Nom actuel : Alchemical Decanter. Version du code la plus proche : 2021-11 (ressemblance 0.45, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Inflicts the player with the Plague debuff / 2% increased damage
- Officiel : All attacks inflict the Plague and reduces damage taken from the Plague by 50% / Projectiles spawn plague seekers on enemy hits / Releases plague bees and douses you in Honey (buff) when damaged
- Lignes introuvables dans le code : Inflicts the player with the Plague debuff
- **Revue** : « Inflige la Plague au joueur, +2 % de dégâts » : aucune version. Depuis son ajout (1.1.6.15), les attaques infligent la Plague aux ennemis (seekers, immunité ou réduction de la Plague selon les versions) ; renommée Alchemical Decanter en 2.1.0.
- Note de version : [2.1.0] Renamed Alchemical Flask to Alchemical Decanter.
- « 2% increased damage » : ailleurs dans le code (2019-02, Items/Accessories/AmalgamatedBrain.cs : 12% increased damage) — **recopiée d'un autre objet**

### Baroclaw (Accessories)

Version du code la plus proche : None (ressemblance 0.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : 10 defense / Press [Baroclaw] to summon two large claws that drag the nearest enemy inwards / Enemies held by the claws remain trapped for 5 seconds
- Officiel : 10% increased damage and 5% increased damage reduction / Attackers take high damage and are inflicted with Armor Crunch and Crush Depth
- Lignes introuvables dans le code : Press [Baroclaw] to summon two large claws that drag the nearest enemy inwards / Enemies held by the claws remain trapped for 5 seconds
- **Revue** : Pinces invoquées par une touche qui attrapent un ennemi 5 s : jamais. Ajouté en 2.0.3.002 avec des dégâts renvoyés (thorns), Armor Crunch puis Crush Depth (2.0.3.003). « 10 defense » : jamais non plus (dégâts et réduction de dégâts).
- « 10 defense » : ailleurs dans le code (2021-01, Items/CalamityGlobalItem.cs : +10 defense)

### Clockwork Bow (Weapons.Ranged)

Version du code la plus proche : 2022-01 (ressemblance 0.33, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Launches cogs / Repeated attacks build up damage and fire an occasional precision blast
- Officiel : Hold [use] to load up to six precision bolts / The more precision bolts are loaded, the harder they hit
- Lignes introuvables dans le code : Launches cogs / Repeated attacks build up damage and fire an occasional precision blast
- **Revue** : Tire des rouages, cumule les dégâts et lance un « Precision Blast » après 50 coups : jamais. Avant 1.5.1.001 : 10 à 18 flèches tombées du ciel ; depuis : arc qui charge jusqu'à six carreaux.
- Note de version : [1.5.1.001] Reworked Clockwork Bow, it no longer fires 10 arrows from the sky which pass through blocks. It now holds out a bow which charges up to six homing bolts. Damage of the bolts increases based on the number of loaded bolts. Buffed damage from 66 to 808.

### Death Whistle (SummonItems)

Version du code la plus proche : 2019-02 (ressemblance 0.33, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Plays a melancholous tune that scares away enemies
- Officiel : Summons the Ravager / Not consumable / One of the few sounds the flesh golem still recognizes.
- Lignes introuvables dans le code : Plays a melancholous tune that scares away enemies
- **Revue** : Arme qui tire des notes de musique (Temporal Sadness, Confused) : jamais. Objet d'invocation du Ravager depuis toujours (ancien Ancient Medallion, renommé en 1.4.5.001).
- Note de version : [1.4.5.001] Renamed Ancient Medallion to Death Whistle.

### Dimensional Soul Artifact (Accessories)

Nom actuel : Crushing Ego. Version du code la plus proche : 2019-02 (ressemblance 0.38, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Power / Creates a cosmilite orbital
- Officiel : Multiplies all damage by 1.3x, but at the cost of taking 20% increased damage / Don the mask of arrogance, and dance by your lonesome. Once the ball has gone quiet, you'll be the last man standing.
- Lignes introuvables dans le code : Creates a cosmilite orbital
- **Revue** : « Crée un orbital de Cosmilite » (purement visuel) : jamais. L'objet a toujours augmenté les dégâts en contrepartie des dégâts subis (1.3.4.103, 1.4.1.001...) ; renommé Crushing Ego en 2.2.0.
- Note de version : [2.2.0] Renamed Dimensional Soul Artifact to Crushing Ego.

### Divine Intervention (Projectiles.Magic)

Version du code la plus proche : None (ressemblance 0.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Striking enemies spawns sparks and homing meteor heads and grants the holy flame buff / The holy flame buff increases melee speed and stealth regen / Can be thrown if your stealth is full / You're getting rusty, machine!
- Officiel : (pas de page)
- Lignes introuvables dans le code : Striking enemies spawns sparks and homing meteor heads and grants the holy flame buff / The holy flame buff increases melee speed and stealth regen / Can be thrown if your stealth is full / You're getting rusty, machine!
- **Revue** : Épée pré-Hardmode lâchée par les Meteor Heads (étincelles, têtes de météore, buff de flamme sacrée, lancer en furtif) : n'a jamais existé dans Calamity. L'index la relie à tort au projectile HolyFlame (statut « renamed ») : c'est un objet entièrement nouveau à créer.

### Eye of the Storm (Accessories)

Version du code la plus proche : 2018-12 (ressemblance 0.71, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Summons an aether valkyrie to fight for you
- Officiel : Summons a Cloud Elemental to fight for you / The elemental has reduced opacity if visibility is disabled
- Lignes introuvables dans le code : Summons an aether valkyrie to fight for you
- **Revue** : Invoque une Aether Valkyrie amie (et serait lâché par l'Aether Valkyrie, ennemi inventé) : jamais. Invoque un Cloud Elemental dans toutes les versions (arme d'invocation, puis accessoire depuis 1.1.7.701).
- Note de version : [1.1.7.701] Reworked Eye of the Storm, Pearl of Enthrallment, Rare Waifu in a Bottle, and Waifu in a Bottle into accessories that summon their respective elementals when equipped, instead of summon weapons.

### Forgotten Dragon Egg (Pets)

Version du code la plus proche : 2019-06 (ressemblance 0.31, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Delicious... don't think about the specifics, though
- Officiel : Calls Akato, son of Yharon, to your side
- Lignes introuvables dans le code : Delicious... don't think about the specifics, though
- **Revue** : Aliment qui donne Exquisitely Stuffed jusqu'à la mort : jamais ; c'est l'objet du familier Akato depuis son ajout (1.4.2.101).

### Genesis (Weapons.Magic)

Version du code la plus proche : None (ressemblance 0.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : The ritualistic artifact, having its true purpose revealed / Summons Purgatory Arbiter, Umbrus / During the battle, all wires and wired devices will be disabled and heart pickups will hurt instead of heal / Not consumable
- Officiel : Fire an array of lasers that decreases in spread for each shot / Every sixth shot fires a splitting Genesis Beam and resets the spread of the weapon / Hitting an enemy before the Genesis Beam splits deals increased damage
- Lignes introuvables dans le code : The ritualistic artifact, having its true purpose revealed / Summons Purgatory Arbiter, Umbrus / During the battle, all wires and wired devices will be disabled and heart pickups will hurt instead of heal
- **Revue** : Le Fandom en fait un objet d'invocation non consommable de « Purgatory Arbiter, Umbrus » (boss inventé), lâché par « Vision of the Tyrant » (inventé). Dans Calamity, Genesis est une arme à lasers (ancienne « Genisis », 2019 : rayon en Y et lasers).
- Note de version : [2.0.4.001] Reworked Genesis, it no longer fires splitting beams and a random spread of lasers. It now fires a large spread of lasers which gets tighter on successive shots. Every sixth shot replaces the spread of lasers with a powerful splitting beam, which resets the la
- « Not consumable » : ailleurs dans le code (2018-12, Items/DevourerMunsters/RuneofCos.cs : Not consumable) — **recopiée d'un autre objet**

### Geyser Shell (Pets)

Version du code la plus proche : 2020-04 (ressemblance 0.4, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Shoots out little flak hermits which propel themselves towards enemies / Little acid buggy
- Officiel : Summons a little flak hermit
- Lignes introuvables dans le code : Shoots out little flak hermits which propel themselves towards enemies
- **Revue** : Munition (balles) qui libère des bébés Flak Crab : jamais ; ajouté en 1.4.4.001 comme objet de familier (un petit flak hermit).
- « Little acid buggy » : ailleurs dans le code (2020-04, Buffs/Pets/FlakPetBuff.cs : Little acid buggy)

### Momentum Capacitor (Tools)

Version du code la plus proche : 2022-08 (ressemblance 0.32, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Greatly increased minion speed...
- Officiel : While using the Momentum Capacitor, / your top speed will continuously and uncontrollably increase / This effect only works if there are no bosses alive
- Lignes introuvables dans le code : Greatly increased minion speed...
- **Revue** : Augmente sans cesse la vitesse des serviteurs, vendu par le Bandit : jamais. Avant 2.0.1.001, accessoire qui dépensait la furtivité pour un champ qui accélère les projectiles rogue ; depuis, il augmente la vitesse du joueur.
- Note de version : [2.0.1.001] Reworked Momentum Capacitor, it is no longer an accessory that consumes stealth when pressing {{key|Momentum Capacitor Effect}} to generate a magnetic field that amplifies Rogue projectile velocities. Instead, it now rapidly increases the player's movement spe

### Profaned Core (SummonItems)

Version du code la plus proche : 2020-06 (ressemblance 0.62, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Cores of the unholy flame / Summons Providence when placed within 40 slots of your inventory simultaneously.
- Officiel : Summons Providence when used in the Hallow or Underworld / Not consumable / The heart of Her Guardians, Her wicked wrath shall burn upon whoever now wields it.
- Lignes introuvables dans le code : Summons Providence when placed within 40 slots of your inventory simultaneously.
- **Revue** : Condition inventée : un Core dans 40 cases différentes de l'inventaire pour invoquer Providence. Le vrai s'utilise dans le Hallow ou l'Underworld (consommable puis non consommable dès 1.2.3.001).

### Quiver of Nihility (Accessories)

Version du code la plus proche : 2022-08 (ressemblance 0.34, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Show me the night sky / 30% increased arrow damage / Arrows are trailed by homing shadow arrows and void particles
- Officiel : 5% increased ranged critical strike chance / Summons a ring of four void fields to orbit you / Arrows that pass through these fields gain a 75% damage boost and double speed / Filled with a substance darker than the night sky
- Lignes introuvables dans le code : Show me the night sky / 30% increased arrow damage / Arrows are trailed by homing shadow arrows and void particles
- **Revue** : Ajouté en 2.0.1.001 avec ses quatre champs de vide (code de 2022-08) ; flèches d'ombre à tête chercheuse, particules de vide et +30 % de dégâts des flèches : jamais. Phrase d'ambiance réécrite (« Show me the night sky »).

### Relic of Ruin (Weapons.Magic)

Version du code la plus proche : 2019-08 (ressemblance 0.58, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Casts a spread of Profaned Arrows which reduce enemy defense
- Officiel : Casts a spread of sand blades
- Lignes introuvables dans le code : Casts a spread of Profaned Arrows which reduce enemy defense
- **Revue** : Flèches profanées qui réduisent la défense : jamais. Éventail de lames de sable dans toutes les versions (16 puis 12 projectiles, tête chercheuse depuis 1.4.2.105).

### Seafood (SummonItems)

Version du code la plus proche : 2022-02 (ressemblance 0.25, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Minor improvements to all stats
- Officiel : Summons the Aquatic Scourge when used in the Sulphurous Sea / Enrages outside the Sulphurous Sea / Not consumable / Large bait for a large catch. Beware the monsters of the seas.
- **Revue** : Le Fandom en fait un aliment (Well Fed 15 min, ligne recopiée de la Delicious Meat de 2018). Objet d'invocation de l'Aquatic Scourge dans toutes les versions.
- « Minor improvements to all stats » : ailleurs dans le code (2018-12, Items/Permafrost/DeliciousMeat.cs : Minor improvements to all stats) — **recopiée d'un autre objet**

### Tracking Disk (Weapons.DraedonsArsenal)

Nom actuel : P-LSR: Aerial Tracker. Version du code la plus proche : 2022-04 (ressemblance 0.32, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : A weapon that, as it flies, processes calculations and breaks / Releases a flying disk that breaks / Stealth strikes allow the disk to break faster
- Officiel : Releases a flying disk that fires lasers at nearby enemies / Lasers do not directly damage enemies, but instead apply a stacking laser burn effect / Once hit by enough lasers or sufficient time has passed, laser burn will deal direct damage / The disk gains acceleration as it returns to the player / Stealth strikes toss two disks that can fire at multiple different targets from farther range
- Lignes introuvables dans le code : Releases a flying disk that breaks / Stealth strikes allow the disk to break faster
- **Revue** : Disque qui tourne autour d'un ennemi et se casse au bout de 4 coups : jamais. Disque qui tire des lasers depuis son ajout (1.4.5.001), renommé P-LSR: Aerial Tracker en 2.1.0.
- Note de version : [2.1.0] Renamed Tracking Disk to P-LSR: Aerial Tracker.

### Warloks' Moon Fist (Weapons.Summon)

Version du code la plus proche : 2022-10 (ressemblance 0.18, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Melee attacks and projectiles inflict nightwither and have a 10% chance to instantly kill normal enemies / Grants the user 60% increased true melee damage on true melee hits / 25% increased melee speed, damage, and 5% increased melee critical strike chance / 50% increased true melee damage / Temporary immunity to lava / Increased melee knockback
- Officiel : Summons a celestial fist to punch enemies for you
- Lignes introuvables dans le code : Melee attacks and projectiles inflict nightwither and have a 10% chance to instantly kill normal enemies / Grants the user 60% increased true melee damage on true melee hits
- **Revue** : Le Fandom en fait un accessoire de mêlée (Nightwither, 10 % de mort instantanée, +60 % de dégâts true melee...), assemblé à partir de lignes d'autres accessoires. Arme d'invocation (poing céleste) depuis son ajout (2.0.1.002).
- « 25% increased melee speed, damage, and 5% increased melee critical strike chance » : ailleurs dans le code (2019-02, Items/Accessories/ElementalGauntlet.cs : 15% increased melee speed, damage, and 5% increased melee critical strike chance) — **recopiée d'un autre objet**
- « 50% increased true melee damage » : ailleurs dans le code (2020-07, Items/Accessories/ElementalGauntlet.cs : 20% increased true melee damage) — **recopiée d'un autre objet**
- « Temporary immunity to lava » : ailleurs dans le code (2018-12, Items/Accessories/AngelTreads.cs : Temporary immunity to lava) — **recopiée d'un autre objet**
- « Increased melee knockback » : ailleurs dans le code (2018-12, Items/Accessories/ElementalGauntlet.cs : Increased melee knockback) — **recopiée d'un autre objet**

### Wulfrum Acrobatics Pack (Accessories)

Version du code la plus proche : 2022-09 (ressemblance 0.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : While powered, allows for infinite jumping / Each subsequent jump without landing requires an increasingly narrow / window for when the jump can occur
- Officiel : Provides a wulfrum slingshot grappling hook / This slingshot's stats scale with your equipped hook / If the hook has special effects, the slingshot also benefits from them / The slingshot allows for advanced rope-swinging physics / Hold [Up] or [Down] to retract or extend the slingshot's rope / Automatically grapples nearby walls if a lethal fall is detected / This safety feature won't activate if [Down] is held / 8% increased movement speed
- Lignes introuvables dans le code : While powered, allows for infinite jumping / Each subsequent jump without landing requires an increasingly narrow / window for when the jump can occur
- **Revue** : Saut infini alimenté par des Energy Orbs (objet inventé) avec une fenêtre de plus en plus étroite : jamais. Ajouté en 2.0.1.001 comme grappin-fronde, qu'il est toujours.

### Wulfrum Digging Turtle (Tools)

Version du code la plus proche : 2022-08 (ressemblance 0.34, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : BOOM! / Throws out a Wulfrum Turtle that tries to find enemies and jump into them, exploding / Right click to lay the turtle on the ground to cause enemies to aggro on it / Only 4 turtles can be active at once
- Officiel : Throws a rickety mining contraption to dig out a small tunnel / In case of an emergency, press [open] to instantly detonate all your digging turtles / The turtle scales with your pickaxe power
- Lignes introuvables dans le code : BOOM! / Throws out a Wulfrum Turtle that tries to find enemies and jump into them, exploding / Right click to lay the turtle on the ground to cause enemies to aggro on it / Only 4 turtles can be active at once
- **Revue** : Arme qui lance une tortue explosive contre les ennemis (clic droit : leurre au sol, 4 au plus) : jamais. Outil qui creuse un tunnel depuis son ajout (2.0.1.001).

### Wulfrum Lure (Placeables)

Version du code la plus proche : 2022-08 (ressemblance 0.14, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Shouldn't electricity deter fish?
- Officiel : Consumes Energy Core to emit a wulfrum signal that lures Wulfrum automatons out / The signal lasts for 30 seconds
- Lignes introuvables dans le code : Shouldn't electricity deter fish?
- **Revue** : Page vandalisée : « Wolf Ram Fishing Pole », canne à pêche rechargée par un « Blood orb ». Le vrai (2.0.1.001) consomme une batterie / Energy Core pour attirer les automates Wulfrum.

### Wulfrum Scaffold Kit (Tools)

Version du code la plus proche : 2022-08 (ressemblance 0.01, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Jet upwards like a samurai / Consumes one Wulfrum Metal Scrap to jet the player upwards / As you are moved upwards, up to 40 temporary blocks are placed
- Officiel : Places down temporary metal scaffolding. Uses up Wulfrum Metal Scrap for 40 tiles built / Scaffold needs to be adjacent to a solid tile to be placed down / For when you need something built fast and don't need it to last.
- Lignes introuvables dans le code : Jet upwards like a samurai / Consumes one Wulfrum Metal Scrap to jet the player upwards / As you are moved upwards, up to 40 temporary blocks are placed
- **Revue** : Propulse le joueur vers le haut en posant 40 blocs sous lui : jamais. Pose des échafaudages temporaires depuis son ajout (2.0.1.001), 40 blocs par ferraille.

### Wulfrum Screwdriver (Weapons.Melee)

Version du code la plus proche : 2022-08 (ressemblance 0.02, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Increases the amount of times projectiles pierce by 1 / Reduces life regen by 5
- Officiel : Can be used to rapidly and royally screw over foes / Striking an enemy sometimes gives you a wulfrum screw that you can throw with [open] / Hit a thrown screw with the screwdriver to launch it forward at high speed / Who makes flatheads this large?? The hell am I supposed to use it for? / This thing could take an eye out! / ...Ah.
- Lignes introuvables dans le code : Increases the amount of times projectiles pierce by 1
- **Revue** : Le Fandom en fait une potion (debuff : -5 régénération, +1 perforation à tous les projectiles). Dans Calamity, c'est une épée courte (l'ancienne Wulfrum Blade, renommée et refaite en 2.0.1.001), qui lance des vis.
- Note de version : [2.0.1.001] Renamed Wulfrum Blade to Wulfrum Screwdriver.
- Note de version : [2.0.1.001] Reworked Wulfrum Screwdriver, it no longer functions as a broadsword. It now functions as a shortsword, causing the player to stab in the general direction of their cursor with a screwdriver. Each time an enemy is damaged with the screwdriver, there is a 20% c
- « Reduces life regen by 5 » : ailleurs dans le code (2018-12, Items/FabsolStuff/EvergreenGin.cs : Reduces life regen by 3) — **recopiée d'un autre objet**

### Wulfrum Treasure Pinger (Tools)

Version du code la plus proche : 2022-08 (ressemblance 0.23, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Consumes Energy Orbs to send out radio waves that mark chests
- Officiel : Locates nearby metal that's hopefully more valuable than wulfrum / This contraption seems incredibly shoddy. It'll break sooner than later for sure
- Lignes introuvables dans le code : Consumes Energy Orbs to send out radio waves that mark chests
- **Revue** : Meuble qui consomme des Energy Orbs (objet inventé) pour marquer les coffres : jamais. Détecteur de minerais qui finit par casser depuis son ajout (2.0.1.001).

### Nanoblack Reaper (Weapons.Rogue)

Version du code la plus proche : 2019-08 (ressemblance 0.39, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.2

- Fandom : Annihilates anything it touches with one large swing / Takes 10 seconds to charge, each 10% of charge will release homing 75 energy blades / Holding it uncharged will rapidly heal you / Stealth strikes will release 575 energy blades on hit / She smothered them in Her hatred
- Officiel : Throws a scythe which rapidly forms a swarm of nanoblack tesselations from hard light / Nanoblack tesselations each emit an instantaneous zero-point energy strike / Zero-point energy strikes have 150 armor penetration and will target bosses whenever possible / Release [use] just as the scythe begins returning to carve through the enemy closest to your cursor at light speed / Light-speed carves have 150 armor penetration. If timed in a 4-frame window, they supercrit with +100% critical strike chance / [open] with sufficient stealth to massively increase attack speed for a short time / Scythes thrown during flurries emit supercharged energy strikes but cannot be returned manually / She smothered them in Her hatred. / Hold Shift for more'He who picks up the pen writes the script. / She has no intention of ever putting it down. / Her design eclipses your countable stars. / The gleam She exudes will put them all to shame.
- Lignes introuvables dans le code : Annihilates anything it touches with one large swing / Takes 10 seconds to charge, each 10% of charge will release homing 75 energy blades / Holding it uncharged will rapidly heal you / Stealth strikes will release 575 energy blades on hit
- **Revue** : Charge de 10 s qui lâche 75 lames par seconde, soin rapide quand elle n'est pas chargée, 575 lames en furtif : jamais. Avant 2.1.2, elle lançait des faux à tête chercheuse qui créaient des nanolames (clones en furtif depuis 1.5.1.001).
- Note de version : [2.1.2] Reworked Nanoblack Reaper, it no longer rapidly throws homing nanoblack scythes that spawn nanoblades, and stealth strikes no longer make scythes spawn afterimage clones. It now throws a scythe which rapidly spawns hardlight tesselations which fire an instanta

### Yateveo Bloom (Weapons.Melee)

Version du code la plus proche : 2019-09 (ressemblance 0.54, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.25

- Fandom : A synthesis of jungle flora / Creates blooming roses with each swing / Right click to throw a powerful rose spear / Attacks explode with numerous petals
- Officiel : A synthesis of jungle flora / Throws a powerful rose flail / Press [open] to stab with a flower spear
- Lignes introuvables dans le code : Creates blooming roses with each swing / Attacks explode with numerous petals
- **Revue** : Lance qui frappe le sol et fait pousser des roses qui explosent en pétales (Poisoned, Venom) : jamais. Fléau de roses et estoc de lance fleurie depuis son ajout (1.4.2.104).

### Abyss Shell Fossil (Pets)

Version du code la plus proche : None (ressemblance 0.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.33

- Fandom : A fossil of a prehistoric creature, long forgotten in the deep abyss / Summons a pet Escargidolon Snail to follow you / May attract unwanted attention in the abyss...
- Officiel : Summons a pet Eidolon Snail to follow you / Slightly reduces creatures' ability to detect you in the abyss while equipped
- Lignes introuvables dans le code : A fossil of a prehistoric creature, long forgotten in the deep abyss / May attract unwanted attention in the abyss...
- **Revue** : Familier escargot (réel, ajouté en 2.0.2.001) ; inventé : dans l'Abysse, il attire un Adult Eidolon Wyrm qui le dévore puis attaque le joueur. Le vrai réduit au contraire la détection par les créatures de l'Abysse.

### The Syringe (Weapons.Rogue)

Version du code la plus proche : 2019-12 (ressemblance 0.33, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Gives a powerful buff, with drawbacks included! / I'm pretty sure this isn't a good idea
- Officiel : Throws a high velocity syringe that increases damage as it travels / Shatters into glass and cinders on impact / Stealth strikes also shatter into Plague bees / I'm pretty sure this isn't healthy
- Lignes introuvables dans le code : Gives a powerful buff, with drawbacks included!
- **Revue** : Le Fandom en fait une potion (buff « Plagued Wrath » : dégâts, critique, vitesse, contreparties). Arme spy (seringue qui éclate en verre et cendres) depuis son ajout (1.4.2.106).

## Mélange (revue à la main) : en partie une ancienne version, en partie inventé

### Awakening (Lore)

Version du code la plus proche : None (ressemblance 0.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Whispers from on high dance in your ears... / Press "Left Shift" to listen closer
- Officiel : (pas de page)
- Lignes introuvables dans le code : Press "Left Shift" to listen closer
- **Revue** : Infobulle réelle (objet de lore du Starter Bag, 2.0.2.001). Inventé par la page : un clic droit dans l'inventaire invoque « The Calamity » (boss inventé), objet non renouvelable.
- « Whispers from on high dance in your ears... » : ailleurs dans le code (2.2.2, Localization/en-US/Mods.CalamityMod.Items.Lore.hjson : [c/E3AF40:Whispers from on high dance in your ears...])

### Elemental Shiv (Weapons.Melee)

Nom actuel : Lightspeed. Version du code la plus proche : 2022-07 (ressemblance 0.68, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Don't underestimate the power of shivs / Shoots a rainbow shiv that spawns additional shivs on hit / Shortflurry attacks split into more beams and increase melee speed, DR, and defense by 15%
- Officiel : Unleash a flurry of stabs which build Elemental Mastery on hit / Elemental Mastery is lost over time if it has not been gained for a few seconds / Press [open] with maximum Elemental Mastery to dash toward your cursor / Hitting an enemy during the dash refunds some Elemental Mastery
- Lignes introuvables dans le code : Shortflurry attacks split into more beams and increase melee speed, DR, and defense by 15%
- **Revue** : Ancienne version de l'arme. Ligne Shortflurry : mécanique inventée par le Fandom (page Shortstrike), déjà implémentée dans le mod.
- Note de version : [2.1.0] Renamed Elemental Shiv to Lightspeed.
- Note de version : [2.1.0] Reworked Lightspeed, it no longer fires homing rainbow shiv projectiles that spawn additional shiv projectiles on hit. It now rapidly thrusts in a flurry of stabs straight ahead. Hitting with the flurry of stabs builds Elemental Mastery; Elemental Mastery diss

### Exoblade (Weapons.Melee)

Version du code la plus proche : 2022-07 (ressemblance 0.31, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Ancient blade of Yharim's weapons and armors expert, Draedon / Fires a spread of exo beams with different effects based on the beam's color / Killing an enemy with true melee strikes causes a large explosion that freezes nearby enemies
- Officiel : Press [use] to release multiple energy beams that home in on enemies and slice them on hit / Press [open] to dash in the direction of the cursor with the blade / Enemy hits from the blade during the dash result in massive damage and a rebound / Press [use] immediately after a rebound to perform a powerful slash / Powerful slashes are larger, heal you, and create a large explosion / Struck enemies are torn apart by Miracle Blight
- Lignes introuvables dans le code : Fires a spread of exo beams with different effects based on the beam's color / Killing an enemy with true melee strikes causes a large explosion that freezes nearby enemies
- **Revue** : Explosion glacée quand un ennemi presque mort est touché : ancienne version (DraedonsExoblade, 2018 à 2022-07). Inventé : « une gerbe de rayons exo aux effets différents selon leur couleur » (rayons des composants, Terratome...) ; l'ancienne arme tirait un rayon exo et des comètes. Refaite en 2022-08 (rayons à tête chercheuse, ruée).
- Note de version : [2.0.1.001] Reworked Exoblade, it no longer fires an exo beam and summons exo comets on true melee hits to protect the player. It now has a custom swing animation, and when swung, fires out three exo energy beams that home onto enemies after a short duration. When an exo 

### Galileo Gladius (Weapons.Melee)

Version du code la plus proche : 2020-01 (ressemblance 0.66, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Don't underestimate the power of small space swords / Shoots a homing crescent moon / Spawns planetoids on enemy hits / Shortflurry attacks spawn planetoids on projectile hits, increase melee speed by 50%, and give immunity to Nightwither
- Officiel : Rapidly stabs in front of the player, generating Starbursts on hit / [open] to throw the gladius on a constellation chain, impaling foes and inflicting Voidfrost / When the blade is impaled, use [use] or [open] to free it / [use] pulls it free and deals high damage, consuming 10 Starbursts / [open] pulls yourself to the blade, allowing you to jump off the foe / Jumping off a foe consumes 20 Starbursts and provides invincibility / Figaro, magnifico
- Lignes introuvables dans le code : Shortflurry attacks spawn planetoids on projectile hits, increase melee speed by 50%, and give immunity to Nightwither
- **Revue** : Ancienne version (croissant de lune, planètes : 2020 à 2.0.1.005, portée en Full Fandom). Ligne Shortflurry : mécanique inventée par le Fandom (page Shortstrike), déjà implémentée dans le mod.
- Note de version : [2.1.0] Reworked Galileo Gladius, it no longer fires homing crescent projectiles or rains planets on true melee hits. It now rapidly stabs in front of the player with an enlarged slash hitbox. Hitting enemies generates Starburst. Starburst is an energy pool used by St

### Lucrecia (Weapons.Melee)

Version du code la plus proche : 2021-01 (ressemblance 0.66, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Finesse / Striking an enemy makes you immune for a short time / Fires a DNA chain / Shortflurry attacks greatly increase the chain's range, and increase life regen
- Officiel : Fires a spiraling darklight shard when swung / Hits with the blade itself build Darklight Energy / Darklight Energy is lost over time if it has not been gained for a few seconds / Press [open] with maximum energy to fire a double helix comet
- Lignes introuvables dans le code : Shortflurry attacks greatly increase the chain's range, and increase life regen
- **Revue** : Ancienne version (chaîne d'ADN, immunité en frappant : avant 2.0.4.001 ; aujourd'hui éclat de lumière noire et énergie). Ligne Shortflurry : mécanique inventée par le Fandom (page Shortstrike), déjà implémentée dans le mod.
- Note de version : [2.1.0] Reworked Lucrecia, it no longer thrusts to fire small DNA projectiles. It now has a custom swing animation, where the sword is swung back and forth. Each swing fires a darklight shard projectile. Hitting with the sword itself builds charge in the weapon; charg

### Sand Shark Tooth Necklace (Accessories)

Version du code la plus proche : 2021-08 (ressemblance 0.29, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : You're the alpha now / Increases all stats by 10% / Increases armor penetration by 10Summons a smart Sand Shark to protect you / It can be controlled directly with the cursor while a summoning weapon is held, and will benefit from all minion bonuses
- Officiel : Increases armor penetration by 10 / 10% increased damage
- Lignes introuvables dans le code : You're the alpha now / Increases all stats by 10% / Increases armor penetration by 10Summons a smart Sand Shark to protect you / It can be controlled directly with the cursor while a summoning weapon is held, and will benefit from all minion bonuses
- **Revue** : +10 de pénétration d'armure : réel (toutes versions). Inventés : le requin des sables invoqué, « Increases all stats by 10% » (6 % de dégâts jusqu'à 2.1.0, 10 % ensuite), la phrase « You're the alpha now ».

### Spore Knife (Weapons.Rogue)

Version du code la plus proche : 2021-02 (ressemblance 0.57, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Enemies release spore clouds on hit / Shortflurry attacks inflict Poisoned and spawn vines
- Officiel : Throws an overgrown dagger that bursts into a cloud of poisonous spores / Stealth strikes erupt into hungry homing buds that stick to enemies and gnaw them
- Lignes introuvables dans le code : Shortflurry attacks inflict Poisoned and spawn vines
- **Revue** : Épée courte de mêlée retirée en juin 2022 (nuages de spores), portée en Full Fandom ; refaite depuis en couteau de lancer rogue. Ligne Shortflurry : mécanique inventée par le Fandom (page Shortstrike), déjà implémentée dans le mod.

### Terminus (SummonItems)

Version du code la plus proche : 2019-10 (ressemblance 0.93, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : A ritualistic artifact, thought to have brought upon The End many millennia ago / Sealed away in the abyss, far from those that would seek to misuse it / Activates Boss Rush Mode, using it again will deactivate Boss Rush Mode / During the Boss Rush, all wires and wired devices will be disabled / Right click to summon the forgotten massacre
- Officiel : Hold [use] to start or end the Boss Rush
- Lignes introuvables dans le code : Right click to summon the forgotten massacre
- **Revue** : Lancement du Boss Rush : infobulle de 2022. Inventés : le menu à deux choix, le clic droit qui invoque « Vision of the Tyrant » (boss inventé), l'obtention en battant La Ruga (boss inventé).

### The Amalgam (Accessories)

Version du code la plus proche : 2021-01 (ressemblance 0.38, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Extends the duration of potion buffs by 100% and potion buffs remain active even after you die / 15% increased damage / Shade and brimstone flames rain down when you are hit / Nearby enemies receive a variety of debuffs when you are hit / Life drains over time in exchange for a significant damage boost as health decreases / You leave behind a trail of spores, toxic clouds, plague bees, and brine / Standing still generates an aura that slows enemies, and increases defense and life regen / All weapons have a chance to fire out mini birbs and miniature suns / 50% of the items stats are granted to team members
- Officiel : All attacks inflict a variety of debuffs, 200% increased elemental debuff damage / Most buffs last twice as long, and persist after dying / Taking damage confuses nearby enemies and spawns a dank creeper / The dank creeper flies above you, exploding into several shaderain clouds / Grants or improves the ability to dodge attacks / After dodging, summon damage and crit chance are boosted by 10% / You have no idea how it's staying together / Expert
- Lignes introuvables dans le code : Life drains over time in exchange for a significant damage boost as health decreases / You leave behind a trail of spores, toxic clouds, plague bees, and brine / Standing still generates an aura that slows enemies, and increases defense and life regen / All weapons have a chance to fire out mini birbs and miniature suns / 50% of the items stats are granted to team members
- **Revue** : Surtout l'ancienne version (avant 1.5.0.001 : effets hérités du Fungal Clump, de la Leviathan Ambergris et du Void of Extinction ; aura, durées de buffs). Introuvables dans toute version : « la vie draine contre un gros bonus de dégâts », la traînée de spores/nuages/abeilles/saumure, « 50 % des stats aux équipiers » (la page dit 7,5 % de dégâts à l'équipe).

### The Sponge (Accessories)

Version du code la plus proche : 2021-10 (ressemblance 0.78, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : 15% increased damage reduction / +30 max life and mana / 5% increased movement and jump speed / Standing still boosts life and mana regen / Increased defense, movement speed and damage reduction while submerged in liquid / Enemies take damage when they hit you / You emit a cloud of mushroom spores and sparks when you are hit / 2% reduced stealth costs / 6.25% of the damage from enemy attacks is absorbed and converted into healing / Increased healing as health lowers / Dying revives you inside of an encumbering shell for 3 seconds with a 1 minute cooldown / Generates an energy shield over 1 minute that takes one hit
- Officiel : Grants immunity to knockback / Gain a barrier with 120 health / The barrier grants 10% damage reduction while active / Halved Adrenaline loss if all damage is absorbed / The barrier will regenerate 8 seconds after being hit, taking 10 seconds to completely regenerate / Getting hit resets the timer
- Lignes introuvables dans le code : 2% reduced stealth costs / Increased healing as health lowers / Dying revives you inside of an encumbering shell for 3 seconds with a 1 minute cooldown / Generates an energy shield over 1 minute that takes one hit
- **Revue** : Ancienne version (stats de 1.5.0.001 : 20 défense, +30 vie et mana, 6,25 % de soin...). Inventés : la résurrection dans une coquille 3 s (1 min de recharge) et le bouclier d'énergie qui encaisse un coup (la barrière de 120 PV n'arrive qu'avec la refonte 2.0.3).
- Note de version : [2.0.3.003] | description = Reworked the Honey Dew and Crown Jewel lines and The Sponge. Multiple general rebalances.
- Note de version : [2.0.3.003] Reworked The Sponge, it no longer grants 20 defense, 30 max mana, or immunity to Armor Crunch. It now grants a shield with a durability of 180. This shield can recharge if no damage is taken for 9 seconds, taking 6 seconds to fully charge. While the shield is 

### Violence (Weapons.Melee)

Version du code la plus proche : 2021-07 (ressemblance 0.69, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Releases a blazing fork which stays near the mouse and shreds enemies, racking up Violence Points that can be used for strong attacks
- Officiel : A demonic pitchfork, cursed to cause endless bloodshed / [use] commands the trident to follow your cursor and shred enemies with devastating speed / [open] throws the trident, violently tearing through enemies with immense power and inflicting Vulnerability Hex
- Lignes introuvables dans le code : Releases a blazing fork which stays near the mouse and shreds enemies, racking up Violence Points that can be used for strong attacks
- **Revue** : Fourche qui reste près du curseur et déchiquette : ancienne version (1.5.0.001 à 2.1.0, avant le lancer au clic droit). Inventés : les « Violence Points » à dépenser en attaques puissantes.

### Dark God's Sheath (Accessories)

Nom actuel : Dark Matter Sheath. Version du code la plus proche : 2022-10 (ressemblance 0.28, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.2

- Fandom : The Dark God’s void / +20 maximum stealth / 20% increased rogue crit chance in space / Stealth strikes launch dark meteor blasts / rogue projectile velocity is doubled
- Officiel : +10 maximum stealth / Stealth generation is boosted while moving if not attacking / Stealth strikes only expend 50% of your max stealth / 6% increased rogue damage and rogue crit chance
- Lignes introuvables dans le code : The Dark God’s void / 20% increased rogue crit chance in space / Stealth strikes launch dark meteor blasts / rogue projectile velocity is doubled
- **Revue** : +20 furtivité max : ancienne version (1.4.3.002 à 2.0.1.002, 10 ensuite). Inventés : +20 % de critique spy dans l'espace, météores sombres sur les attaques furtives, vitesse des projectiles doublée. Renommé Dark Matter Sheath (2.0.3.003).
- Note de version : [2.0.3.003] Renamed Dark God's Sheath to Dark Matter Sheath.

### Cosmic Shiv (Weapons.Melee)

Version du code la plus proche : 2020-06 (ressemblance 0.75, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.25

- Fandom : Definitely don't underestimate the power of shivs / Fires a cosmic beam that homes in on enemies / Upon hitting an enemy, a barrage of offscreen objects home in on the enemy as well as raining stars / Shortflurry attacks have a chance to fire a second beam, grants the player Cosmic Freeze, and increase melee speed, DR, and defense
- Officiel : Fires a cosmic star that homes in on enemies / Upon hitting an enemy, a barrage of blades will persistently strike / Landing more stars causes more blades to strike
- Lignes introuvables dans le code : Shortflurry attacks have a chance to fire a second beam, grants the player Cosmic Freeze, and increase melee speed, DR, and defense
- **Revue** : Ancienne version (rayon cosmique, barrage et pluie d'étoiles : 2020 à 2.0.1.005, portée en Full Fandom). Ligne Shortflurry : mécanique inventée par le Fandom (page Shortstrike), déjà implémentée dans le mod.
- Note de version : [2.0.4.001] Reworked Cosmic Shiv, it no longer shoots a fast orb that spawns several blades from the sides and rains down several stars on hit. It now shoots homing stars that spawn an invisible aura over the hit enemy for 4 seconds. This aura continuously summons blade p

### The Absorber (Accessories)

Version du code la plus proche : 2021-10 (ressemblance 0.97, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.25

- Fandom : 5% increased movement and jump speed / +20 max life and mana / Standing still boosts life and mana regen / Increased defense, movement speed and damage reduction while submerged in liquid / Enemies take damage when they hit you / You emit a cloud of mushroom spores when you are hit / 10% increased damage reduction / 3% reduced stealth costs / 5% of the damage from enemy attacks is absorbed and converted into healing
- Officiel : Grants immunity to knockback / 12% increased movement and jump speed / Attackers take immense damage and are afflicted with Absorber Affliction / Using a healing potion will spawn a green aura that lasts for 30 seconds / Walking into the aura removes all debuffs once and grants 2 HP/s regen, 8% damage, and 5% damage reduction / 5% of the damage from enemy attacks is converted into healing
- Lignes introuvables dans le code : 3% reduced stealth costs
- **Revue** : Ancienne version (effets de ses composants, +20 vie et mana, régénération à l'arrêt, épines... avant la refonte de 2.0.3). Inventé : « 3 % reduced stealth costs », venu de la Mirage Jelly inventée.
- Note de version : [2.0.3.002] Reworked The Absorber, it no longer grants 7% damage reduction, 20 max health and mana, increases life and mana regeneration when standing still, emits a cloud of mushroom spores when hit, or grants immunity to Armor Crunch. Enemies that touch the player are n

### Grand Gelatin (Accessories)

Version du code la plus proche : 2021-09 (ressemblance 0.89, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.33

- Fandom : 10% increased movement and jump speed / 5% reduced stealth cost / +20 max life and mana / Standing still boosts life and mana regen
- Officiel : 12% increased movement speed and jump speed / Using a healing potion creates a cleansing aura which lasts for 30 seconds / Entering the aura removes all debuffs one time and grants +2 HP/s life regen
- Lignes introuvables dans le code : 5% reduced stealth cost
- **Revue** : Ancienne version (avant la refonte 2.0.3.002 : +20 vie et mana, régénération à l'arrêt). « 5 % reduced stealth cost » inventé : vient de la Mirage Jelly, ingrédient inventé par le Fandom.
- Note de version : [2.0.3.002] Reworked Grand Gelatin, it no longer boosts max health and mana by 20 or increases life and mana regeneration when standing still. It now spawns an aura when using a healing potion which last for 45 seconds. Entering the aura will cleanse all debuffs and remov

### Spectral Veil (Accessories)

Version du code la plus proche : 2021-12 (ressemblance 0.51, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.33

- Fandom : The inside of the cloak is full of teeth... / Allows you to dodge attacks / The dodge has a 70 second cooldown / This cooldown is shared with all other dodges and reflects / Provides 10% increased movement speed / Stealth generates 20% faster while moving
- Officiel : Pressing [Spectral Veil Teleport] consumes 25% of your maximum stealth to teleport, rendering you invulnerable / If you dodge something while invulnerable, you instantly gain full stealth / Teleportation is disabled while Chaos State is active / Stealth generates 15% faster / The inside of the cloak is full of teeth...
- Lignes introuvables dans le code : Allows you to dodge attacks
- **Revue** : +10 % de vitesse et furtivité +20 % en mouvement : ancienne version (1.5.0.001, vitesse retirée en 1.5.1.003). Inventée : l'esquive passive avec 70 s de recharge partagée ; l'objet a toujours été une téléportation sur touche qui rend invulnérable (1.4.2.109).
- « The dodge has a 70 second cooldown » : ailleurs dans le code (2021-04, Items/Accessories/StatisBeltOfCurses.cs : The dodge has a 60 second cooldown) — **recopiée d'un autre objet**
- « This cooldown is shared with all other dodges and reflects » : ailleurs dans le code (2021-04, Items/Accessories/AbyssalMirror.cs : This cooldown is shared with all other dodges and reflects) — **recopiée d'un autre objet**

### Deadshot Brooch (Accessories)

Version du code la plus proche : 2022-07 (ressemblance 0.76, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : 10% increased ranged damage, 5% increased ranged critical strike chance, and 20% reduced ammo usage / 5 increased defense, 2 increased life regen, and 15% increased pick speed
- Officiel : 12% increased ranged damage, 7% increased ranged critical strike chance and 20% reduced ammo usage / Pressing [Ammo Cycle Hotkey] will automatically swap the order of ammos in your ammo slots / The ammo type swapped depends on the ammo of the weapon you are holding
- **Revue** : +10 % de dégâts à distance, +5 % de critique, 20 % de munitions économisées : ancienne version. La ligne « 5 increased defense, 2 increased life regen, and 15% increased pick speed » est recopiée de la Daedalus Emblem (code de 2020).
- « 5 increased defense, 2 increased life regen, and 15% increased pick speed » : ailleurs dans le code (2020-05, Items/Accessories/DaedalusEmblem.cs : 5 increased defense, 2 increased life regen, and 15% increased pick speed) — **recopiée d'un autre objet**

### Marksman Bow (Weapons.Ranged)

Version du code la plus proche : 2021-03 (ressemblance 0.78, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Fires three arrows at once / Wooden arrows are converted into Marksman Arrows that can ricochet off coins
- Officiel : Fires three arrows at once / Wooden arrows are converted into Jester's arrows
- Lignes introuvables dans le code : Wooden arrows are converted into Marksman Arrows that can ricochet off coins
- **Revue** : Trois flèches à la fois : réel. Inventé : flèches en bois changées en « Marksman Arrows » qui ricochent sur des pièces ; elles deviennent des Jester's Arrows dans toutes les versions (ajout en 1.4.5.001).

### Reaper Tooth Necklace (Accessories)

Version du code la plus proche : 2022-02 (ressemblance 0.54, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : A grisly trophy from the ultimate predator / 15% increased damage / Increases armor penetration by 15Summons a smart Reaper Shark to protect you / It can be controlled directly with the cursor while a summoning weapon is held, and will benefit from all minion bonuses
- Officiel : 20% increased damage / Increases armor penetration by 30 / A grisly trophy from the ultimate predator
- Lignes introuvables dans le code : Increases armor penetration by 15Summons a smart Reaper Shark to protect you / It can be controlled directly with the cursor while a summoning weapon is held, and will benefit from all minion bonuses
- **Revue** : +15 % de dégâts et +15 de pénétration d'armure : ancienne version (1.5.1.001 à 2.1.0 ; 20 % et 30 aujourd'hui). Inventés : le requin Reaper invoqué, dirigeable au curseur, et « toutes les stats +15 % » de la page.

### Slime Puppet Staff (Weapons.Summon)

Version du code la plus proche : 2021-07 (ressemblance 0.51, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Summons a slime ball that follows your cursor / Slime balls will connect to eachother when close
- Officiel : Summons a slime ball that follows you / The ball flies toward nearby enemies and explodes into slime on enemy hits / Does not consume minion slots
- Lignes introuvables dans le code : Slime balls will connect to eachother when close
- **Revue** : Boules de slime qui suivent le curseur sans prendre de place : ancienne version. Inventé : « les boules se relient entre elles quand elles sont proches ».

### Submarine Shocker (Weapons.Melee)

Version du code la plus proche : 2018-12 (ressemblance 0.64, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Enemies release electric sparks on hit / Shortflurry attacks spawn poisonous clouds
- Officiel : Enemies release electric sparks on hit
- Lignes introuvables dans le code : Shortflurry attacks spawn poisonous clouds
- **Revue** : Ancienne version de l'arme. Ligne Shortflurry : mécanique inventée par le Fandom (page Shortstrike), déjà implémentée dans le mod.

### Calamity (Accessories)

Version du code la plus proche : 2021-12 (ressemblance 0.39, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.67

- Fandom : Lights your cursor ablaze with the Witch's flames, summoning a burning sigil around it / While equipped your FPS is reduced by 50% and makes the game run in matrix mode / Equip in a vanity slot to change the cursor without dealing damage
- Officiel : Lights your cursor ablaze with the Witch's flames, engulfing it in a blazing aura / Enemies touching the blaze take immense damage and are inflicted with Vulnerability Hex / Focusing an enemy for 2.5 seconds curses them, ascending Vulnerability Hex to its True form / Foes afflicted with True Vulnerability Hex take 1.15x damage from all sources / This effect is increased to 2.5x for the blazing cursor itself / Equip in a vanity slot to change the cursor without dealing damage / These changes work in conjunction with the Rainbow Cursor
- Lignes introuvables dans le code : While equipped your FPS is reduced by 50% and makes the game run in matrix mode
- **Revue** : « FPS réduits de 50 %, mode matrix » : blague inventée. Le reste (curseur enflammé) est l'infobulle de 2021.

### Exotic Pheromones (SummonItems)

Version du code la plus proche : 2022-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.67

- Fandom : Attracts the failed draconic experiment / Summon The Dragonfolly when used in the jungle / Not consumable
- Officiel : Summons Dragonfolly when used in the Jungle / Enrages outside the Jungle / Not consumable / Pheromones used for hunting Draconic Offshoots.
- **Revue** : Infobulle : ancienne version réelle (invoque la Dragonfolly dans la Jungle, non consommable). Inventé par la page : c'est une fusée éclairante à tirer avec un Flare Gun dans le Désert, et le boss s'appelle « The Magnificent Lightning Avian » (renommage du Fandom absent du glossaire).

### Bouncing Eyeball (Weapons.Rogue)

Version du code la plus proche : 2020-06 (ressemblance 0.9, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Throws an eyeball that bounces off of surfaces. / Critical chance is doubled during a blood moon / Stealth strikes cause the eyeballs to move much faster and bounce more energetically
- Officiel : Throws an eyeball that bounces off of surfaces / Stealth strikes cause the eyeballs to move much faster and bounce more energetically / Knockback is much stronger during a blood moon
- Lignes introuvables dans le code : Critical chance is doubled during a blood moon
- **Revue** : Œil qui rebondit, attaques furtives : réel. Inventé : « critique doublé pendant une Blood Moon » ; c'est le recul qui augmente.

## Anciennes versions confirmées à la main (notes de version du wiki officiel)

### Cosmolight (Tools)

Version du code la plus proche : 2021-05 (ressemblance 0.54, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Advances time immediately to the next dawn, noon, dusk or midnight / Does not work while a boss is alive
- Officiel : A lost artifact freshly rekindled by exposure to raw cosmic energies / Hold [use] to drastically accelerate the passage of day and night / [open] to freeze the passage of day and night indefinitely / The time freeze lasts forever and works during celestial anomalies
- Lignes introuvables dans le code : Advances time immediately to the next dawn, noon, dusk or midnight
- **Revue** : Avance immédiatement à l'aube, midi, crépuscule ou minuit (midi et minuit depuis 2.0.2.002), pas pendant un boss : fonctionnement d'avant la refonte 2.1.0 (accélération du temps tenue, gel du temps).
- Note de version : [2.1.0] Reworked Cosmolight, it no longer immediately sets time to the next dawn, noon, dusk, or midnight. It now makes time advance at 120x its usual rate when holding left-click. Right-clicking now toggles the ability to indefinitely freeze time.

### Death's Ascension (Weapons.Melee)

Version du code la plus proche : 2022-08 (ressemblance 0.58, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : You can feel the hands of Death himself onto yours / Left Click to perform a true melee slash that rips souls out of hit enemies / Right click to fire a spread of five deadly slashes
- Officiel : Performs powerful soul-infused swings that creates a rift orbited by scythes on enemy hits / Press [open] to launch a barrage of homing scythes / The rift will fire scythes along with you / It feels just right in your grip... / But the hands that wield it are not yours
- Lignes introuvables dans le code : Left Click to perform a true melee slash that rips souls out of hit enemies / Right click to fire a spread of five deadly slashes
- **Revue** : Version d'origine (2.0.1.001 à 2.0.1.005 : estoc qui arrache des âmes, clic droit = 5 projectiles en éventail, vérifié dans le code de 2.0.1.005). Refaite ensuite (faille entourée de faux).

### Fluorescent Vodka (Potions)

Nom actuel : Purple Haze. Version du code la plus proche : None (ressemblance 0.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Boosts all damage stats by 8% but lowers defense by 10% / Increases immune time after being struck
- Officiel : For 5 seconds after a stealth strike, all damage increased by 25%, but stealth strike damage reduced by 25% / Drinking more than 3 different alcohols might not end well with your liver
- **Revue** : Effets de l'ancienne Fabsol's Vodka (+8 % de dégâts, -10 % de défense, immunité prolongée), que le Fandom a renommée Fluorescent Vodka et Calamity Purple Haze, avant sa refonte (bonus après une attaque furtive). Le renommage du glossaire (Purple Haze -> Fluorescent Vodka) est donc juste.
- « Boosts all damage stats by 8% but lowers defense by 10% » : ailleurs dans le code (2021-01, Items/Potions/Alcohol/FabsolsVodka.cs : Boosts all damage stats by 8% but lowers defense by 10%) — **recopiée d'un autre objet**
- « Increases immune time after being struck » : ailleurs dans le code (2018-12, Items/FabsolStuff/FabsolsVodka.cs : Increases immune time after being struck) — **recopiée d'un autre objet**

### Heavenly Gale (Weapons.Ranged)

Version du code la plus proche : 2021-03 (ressemblance 0.14, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Converts wooden arrows into gilded exo arrows / Arrows rain down from above the cursor which rapidly gust sideways upon enemy hits / Upon gusting, a large slash is created, trailed by heavenly lightning.
- Officiel : Fires a rapid stream of supercharged exo-crystals / Holding the bow and waiting for some time before firing causes the crystals to become more powerful / If the crystals are sufficiently powerful enough they will summon torrents of exo-lightning above whatever target they hit / Enemies struck with Exo-lightning are atomized by Miracle Blight
- Lignes introuvables dans le code : Arrows rain down from above the cursor which rapidly gust sideways upon enemy hits / Upon gusting, a large slash is created, trailed by heavenly lightning.
- **Revue** : Version d'avant la refonte (flèches exo qui tombent au-dessus du curseur, tornades vertes à l'impact, 1.2.3.007 à 1.5.x), racontée autrement. Introuvable : « une grande entaille suivie d'éclairs célestes ».
- Note de version : [2.0.1.002] Reworked Heavenly Gale, it no longer fires ten laser arrows with different effects depending on their color. It now fires a burst of nine crystal arrows in different angles from its tip which home in on enemies. The player can stop attacking for up to five sec

### Infected Remote (Weapons.Summon)

Version du code la plus proche : 2020-04 (ressemblance 0.94, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : There’s a faded note written on it in green / Only the first line is readable: ‘She won’t afflict you, I promise!’ / Summons a Plague Elemental / Consumes all of the remaining minion slots on use / Must be used from the hotbar / Increased power based on the number of minion slots used
- Officiel : Summons a humanoid-hybrid Plaguebringer, codenamed 'Virili', to aid you in battle / Virili can only be summoned once / Virili uses a variety of attacks which she cycles through on a timer / All attacks spread the Plague, as was her original design intention / There's a single label on the back: / WARNING: Subject is extremely corrosive and contagious. Avoid direct contact.
- Lignes introuvables dans le code : Summons a Plague Elemental
- **Revue** : Infobulle de 2020 (consomme toutes les places de serviteur, s'utilise depuis la barre rapide) ; Virili y est appelée « Plague Elemental ». Refaite en 2.0.1.002 (3 places).
- Note de version : [2.0.1.002] Reworked Infected Remote. It now costs 3 minion slots instead of scaling based on minion slots used. Virili now charges in a cross-based pattern, and charges above the target during the rocket firing phase. Charges now have a motion blur visual. Bees and Baby 

### Mutated Truffle (Weapons.Summon)

Version du code la plus proche : 2020-04 (ressemblance 0.74, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Summons a gaseous Old Duke to fight for you / When below 50% life, it moves much faster and additionally spawns gassy clouds on hit
- Officiel : Summons a younger fishron to irradiate your foes / The fishron will cycle between charging, projectiles, and a typhoon / There can only be one minion
- Lignes introuvables dans le code : When below 50% life, it moves much faster and additionally spawns gassy clouds on hit
- **Revue** : Accessoire qui invoque un jeune Old Duke : ancienne version (refait en arme d'invocation en 2.0.3.005). « Sous 50 % de vie, plus rapide et nuages de gaz au coup » : introuvable dans le code ni dans les notes de version, invérifiable.
- Note de version : [2.0.3.005] Reworked Mutated Truffle, it is now a summon weapon instead of an accessory. Only one Younger Duke can be summoned, and it requires 3 minion slots to summon. The Younger Duke cycles between 3 attacks: charging rapidly at enemies, shooting tooth balls which exp

### Rusty Beacon Prototype (Weapons.Summon)

Version du code la plus proche : 2022-11 (ressemblance 0.68, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Summons a long-abandoned drone to support you / The drone slowly follows your cursor and inflicts Irradiated to nearby enemies
- Officiel : Summons a sentry / Summons a long-abandoned drone that passively emits Irradiated waves
- Lignes introuvables dans le code : The drone slowly follows your cursor and inflicts Irradiated to nearby enemies
- **Revue** : Drone qui inflige Irradiated : version 2.0.1.003 (Irradiated au lieu de Sulphuric Poisoning) ; « suit lentement le curseur » : invérifiable (avant 2.0.1.002 il planait au-dessus des ennemis puis éclatait en éclats ; depuis, tourelle à impulsions).
- Note de version : [2.0.1.002] Reworked Rusty Beacon Prototype, it no longer hovers over enemies and bursts into rusty shrapnel. It now releases a cloudy pulse every 2 seconds which inflicts Sulphuric Poisoning for 1.5 seconds. It now has a base damage of 2 and consumes sentry slots instead

### Stream Gouge (Weapons.Melee)

Version du code la plus proche : 2020-02 (ressemblance 0.69, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires an essence spear clone that turns into a portal to summon more clones / Ignores immunity frames
- Officiel : Summons a portal that the spear crosses through / Shortly after going through the portal, portals appear near the mouse that release copies of the spear's cutting edge / Enemies hit by the copies create lacerations in space, revealing a cosmic background
- Lignes introuvables dans le code : Fires an essence spear clone that turns into a portal to summon more clones
- **Revue** : Même mécanisme raconté autrement (clone de la lance qui devient un portail d'où sortent d'autres clones, tooltip de 1.4.3.001) ; « ignore les immunités » : réel à l'époque (1 frame d'immunité locale jusqu'à 1.5.0.001).
- Note de version : [2.0.0.001] Reworked Stream Gouge, it no longer fires essence spear clones. The spear now spins around the player and summons a portal to attack in random directions in the range of the cursor. Buffed damage from 112 to 300.

### Subsuming Vortex (Weapons.Magic)

Version du code la plus proche : 2020-09 (ressemblance 0.44, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Summons a colossal vortex around the player while held / Nearby enemies are sucked into the vortex and are attacked by a flurry of exo tentacles
- Officiel : Press [use] to release a barrage of vortices that race towards enemies / Press [open] to cast a gigantic vortex in front of you with a bias towards the mouse / When enemies are near the vortex, it sends multiple fast-moving smaller vortices towards them / Releasing [open] fires the vortex towards the mouse if enough time has passed / Damage scales based on how long the vortex was held out / Struck enemies are completely consumed by Miracle Blight
- Lignes introuvables dans le code : Summons a colossal vortex around the player while held / Nearby enemies are sucked into the vortex and are attacked by a flurry of exo tentacles
- **Revue** : Version de 2020 (vortex géant et lent, tentacules exo qui frappent les ennemis proches) racontée autrement ; « autour du joueur tant qu'on le tient » et l'aspiration des ennemis sont des ajouts de rédaction. Refait en 2022-09 puis 2022-11 (clic gauche / clic droit).
- Note de version : [1.4.5.001] Reworked Subsuming Vortex, it now summons massive vortices that damage enemies and spawn tentacles. Nerfed damage from 520 to 480.
- Note de version : [2.0.1.002] Reworked Subsuming Vortex. Primary fire now fires a spread of three high-speed small homing vortices. Secondary fire now fires a large semi-controllable swirling vortex. The vortex fires fast-moving homing small vortices at nearby enemies. After charging for 4

### The Hive (Weapons.Ranged)

Version du code la plus proche : 2019-08 (ressemblance 0.64, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Launches a variety of rockets
- Officiel : Launches a wide spread of 4 missiles that explode on contact with enemies and tiles / Charging the launcher decreases the spread and increases the velocity of the missile barrage / Fully charging the launcher will cause it to fire a Plague nuke to decimate a massive area on impact
- Lignes introuvables dans le code : Launches a variety of rockets
- **Revue** : Lance plusieurs sortes de roquettes au hasard : ancienne version (même chance pour chaque projectile depuis 1.1.7). Les « 51 types de roquettes » de la page sont invérifiables (probablement exagérés). Refait depuis (salve de 4 missiles, charge, nuke).
- Note de version : [2.0.4.001] Reworked The Hive, it no longer randomly fires one of 4 different rockets with various effects. It now fires a spread of plague missiles on use. Holding down left click charges up the weapon, decreasing the spread and increasing the velocity of the missiles. T

### Atlas Munitions Beacon (Weapons.Summon)

Version du code la plus proche : 2022-11 (ressemblance 0.93, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.12

- Fandom : Drops down a crate from the sky that opens up to reveal a mounted, stationary cannon / The cannon will fire at any potential enemies within its range, and enter overdrive mode if said enemy is close to the cannon When in overdrive mode the cannon uses three barrels that each collectively fire. The cannon also heats up the more it fires in overdrive mode / Players may right click to pick up the cannon and use it for themselves, if they are selecting an Atlas Munitions Beacon. When players fire the cannon, it automatically enters overdrive mode If the held cannon becomes hot due to overdrive mode, you are forced to drop it. Otherwise, right clicking allows you to drop it manually If the held cannon is dropped back onto the mount, it is attached again   It's surprisingly easy to pick up / Summons an Atlas Soldier to fight for you Calamity Community Remix / Takes up 3 minion slots and only one can exist
- Officiel : Summons a sentry / Brings forth an exo-powered, stationary cannon / The cannon will fire at enemies within its range / If an enemy is in close proximity, it will enter Overdrive / When in Overdrive the cannon uses its full power to bring down any foe / While in Overdrive, the cannon will build up heat, eventually overheating / Press [open] while holding this item to pick up the cannon and use it yourself / When held, the cannon will always be in Overdrive / If the cannon overheats while you hold it, you will be forced to drop it / Pressing [open] allows you to drop it manually / If dropped near the base, it will re-attach automatically / It's surprisingly easy to pick up
- Lignes introuvables dans le code : The cannon will fire at any potential enemies within its range, and enter overdrive mode if said enemy is close to the cannon When in overdrive mode the cannon uses three barrels that each collectively fire. The cannon also heats up the more it fires in overdrive mode / Players may right click to pick up the cannon and use it for themselves, if they are selecting an Atlas Munitions Beacon. When players fire the cannon, it automatically enters overdrive mode If the held cannon becomes hot due to overdrive mode, you are forced to drop it. Otherwise, right clicking allows you to drop it manually If the held cannon is dropped back onto the mount, it is attached again   It's surprisingly easy to pick up / Summons an Atlas Soldier to fight for you Calamity Community Remix / Takes up 3 minion slots and only one can exist
- **Revue** : Infobulle détaillée d'origine (2.0.1.002 : caisse lâchée du ciel, canon, mode Overdrive à trois canons, surchauffe), simplifiée en 2.0.3.005 ; seuls les retours à la ligne manquent.

### Blight Spewer

- **Revue** : Page sans infobulle : lance-flammes classique (flammes vertes qui infligent la Plague) = ancienne version ; refait en nuages de nanomachines collantes qui traquent les ennemis.

### The Bee (Accessories)

Version du code la plus proche : 2022-02 (ressemblance 0.99, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : When at full HP, your damage is increased based on your damage reductionDamage taken at full HP is halvedThis has a 10 second cooldown
- Officiel : When at full HP, your damage is increased based on your damage reduction / Damage taken at full HP is halved / Any energy shields must be fully charged for this effect to apply / This has a 6 second cooldown
- Lignes introuvables dans le code : When at full HP, your damage is increased based on your damage reductionDamage taken at full HP is halvedThis has a 10 second cooldown
- **Revue** : Infobulle de 2022 (recharge de 10 s, ramenée à 6 en 2.1.0) ; seuls les retours à la ligne manquent sur le Fandom.

## Refontes : le Fandom décrit une ancienne version, très différente de l'objet actuel

### Abaddon (Accessories)

Version du code la plus proche : 2021-09 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Reduces the damage caused by the Brimstone Flames debuff and provides immunity to Searing Lava
- Officiel : Critical strikes curse enemies with Bane that scales off of your critical strike chance / When critically hit, enemies spread all debuffs they are afflicted with that are weaker than your Bane to nearby enemies / Current Bane damage: [DAMAGE] DPS
- Note de version : [2.0.3.005] Reworked Abaddon, it no longer grants immunity to Brimstone Flames or Searing Lava. It now greatly reduces the damage over time of Brimstone Flames, and increases critical strike chance by 10%. Landing a critical strike creates an explosion that inflicts Brims
- Note de version : [2.2.0] Reworked Abaddon, it no longer reduces the damage of Brimstone Flames, grants 8% critical strike chance, or makes critical strikes create an explosion that inflicts Brimstone Flames. It now makes critical strikes inflict Bane. Bane deals a base of 20 damage pe

### Abyss Blade (Weapons.Melee)

Version du code la plus proche : 2020-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires short-range tridents / Hitting enemies will inflict the crush depth debuff / The lower the enemies' defense, the more damage they take from this debuff
- Officiel : Tosses a spinning abyssal sword that slices through enemies and releases water energy upon hitting a tile / The sword will fly towards enemies after a ricochet, dealing 2x damage
- Note de version : [2.0.4.001] Reworked Abyss Blade, it no longer fires a short-range trident projectile on swing. It now throws a spinning sword which can pierce through enemies. When the sword hits a tile, it spawns homing water energies that inflict Crush Depth for 2 seconds, and will la

### Abyssal Amulet (Accessories)

Nom actuel : Sea Spirit Amulet. Version du code la plus proche : 2021-09 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Attacks inflict the Crush Depths debuff / Grants immunity to the Crush Depth debuff / While in the abyss you gain 10% increased max life
- Officiel : Slowly accumulate up to eight spirit orbs that follow you / Spirits disperse when dashing / If below 50% life, they will heal you, otherwise they will attack nearby enemies / 35% increased Water debuff damage / Spirit orbs are transparent and silent if visibility is disabled
- Note de version : [2.1.0] Renamed Abyssal Amulet to Sea Spirit Amulet.
- Note de version : [2.1.0] Reworked Sea Spirit Amulet, it no longer increases max health while in the Abyss, makes attacks inflict Riptide, or grants immunity to Riptide. It now spawns spirit orbs every 1.08 seconds which follow the player, up to a maximum of 8. Spirit orbs disperse whe

### Acid Round (Ammo)

Nom actuel : Bubonic Round. Version du code la plus proche : 2019-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Explodes into acid that inflicts the plague / Does more damage the higher the target's defense
- Officiel : Spreads the Plague on hit and ignores 10% of enemy defense
- Note de version : [2.0.2.001] Renamed Acid Round to Bubonic Round.

### Aetherflux Cannon (Weapons.Magic)

Version du code la plus proche : 2021-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires a barrage of phased god rays that pass through terrain
- Officiel : Fires a focused photon helix that phases through everything

### Amalgamated Brain (Accessories)

Version du code la plus proche : 2020-12 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : 10% increased damage / Shade rains down when you are hit / You will confuse nearby enemies when you are struck
- Officiel : Taking damage confuses nearby enemies and spawns a dank creeper / The dank creeper flies above you, exploding into several shaderain clouds / Grants or improves the ability to dodge attacks / After dodging, summon damage and crit chance are boosted by 10%

### Ambrosial Ampoule (Accessories)

Version du code la plus proche : 2021-10 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : You emit light / 7% increased damage reduction and increased life regen / Freeze, chill, and frostburn immunity
- Officiel : +1 to +3 HP/s life regen based on missing health (Current: +[REGEN] HP/s) / Natural life regen is 75% more potent and returns 100% faster / You emit yellow light if visibility is enabled
- Note de version : [2.0.3.003] Reworked Ambrosial Ampoule, it no longer increases defense or damage reduction, or grants immunity to any debuffs. It now increases max health by 70, halves the duration of fire and sickness debuffs, and grants between 4 and 12 life regeneration depending on m

### Ancient Altar (Placeables)

Version du code la plus proche : 2019-04 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Used for special crafting
- Officiel : (pas de page)

### Ancient Fossil (Accessories)

Version du code la plus proche : 2019-06 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Increases pick speed by 15% while underground
- Officiel : Increases mining speed by 10% / Provides immunity to damage from falling blocks

### Anechoic Plating (Accessories)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Reduces creature's ability to detect you in the abyss / Reduces the defense reduction that the abyss causes
- Officiel : Abyss creatures are less likely to see you / Mitigates the defense reduction from the abyss' pressure

### Animosity (Weapons.Ranged)

Version du code la plus proche : 2019-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : 50% chance to not consume ammo / Fires a powerful sniper round / Right click to fire a burst of bullets
- Officiel : Fires a shotgun blast with a tight spread which is imbued with Brimstone Flames / Press [open] to fire a powerful brimstone round that causes enemies to become Marked for Death
- Note de version : [2.0.4.001] Reworked Animosity, it no longer fires a high velocity bullet that deals 580% base damage on left click, or a rapid burst of 3 bullets on right click. Left click now fires a tight shotgun spread of bullets; all bullets will inflict Brimstone Flames, no matter 

### Apoctosis Array (Weapons.Magic)

Version du code la plus proche : 2021-03 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires ion blasts that speed up and then explode / Damage scales with how full your mana is / Using Astral Injection reduces the effectiveness of the mana boost
- Officiel : Rapidly fires an array of ion blasts with heavy armor penetration that accelerate and explode / Press [open] to charge the blaster, restoring your mana until it reaches max / The more mana you restore, the stronger the arrow is when you release it / Charging for 90% of your max mana or more will reach max power / Holding this weapon disables the pickup of stars
- Note de version : [2.1.0] Reworked Apoctosis Array, it no longer fires blasts whose damage scales based on the percentage of max mana the player has. It now rapidly fires spreads of 3 blasts which explode on hit, dealing 30% base damage. All projectiles have an innate 40 armor penetrat

### Archaic Powder (Accessories)

Version du code la plus proche : 2021-10 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : 20% increased mining speed, 5% damage reduction, and +10 defense while underground or in the underworld
- Officiel : Increases mining speed by 25% / Provides immunity to damage from falling blocks and dart traps / 65% resistance to all damage from traps / Does not stack with downgrades

### Astral Injection (Potions)

Version du code la plus proche : 2019-06 (ressemblance 0.99, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Gives mana sickness and hurts you when used, but you regenerate mana extremely quickly even when moving or casting spells
- Officiel : Restores 60 mana per second / Inflicts Mana Sickness when used

### Auric Ore (Placeables)

Version du code la plus proche : 2021-10 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Laced with the dormant power of a deity
- Officiel : Infused with long lost draconic energies

### Aurora Blazer (Weapons.Ranged)

Version du code la plus proche : 2020-06 (ressemblance 0.99, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Spews astral flames that travel in star-shaped patterns / 60% chance to not consume gel
- Officiel : Spews a nebula stream which forms a star shaped pattern and spreads the Astral Infection (debuff) / Uses gel for ammo

### Barinade (Weapons.Ranged)

Version du code la plus proche : 2021-03 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Converts wooden arrows into electric bolt arrows that explode
- Officiel : Fires two converging high-velocity fin arrows that easily soar through the air
- Note de version : [2.0.3.007] Reworked Barinade, it no longer converts Wooden Arrows into electric arrows that explode on death. It now fires 2 arrows at once, and converts all arrows into extremely fast fin arrows that converge at the cursor. Nerfed damage from 9 to 4, but buffed use time

### Basher (Weapons.Melee)

Version du code la plus proche : 2020-04 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Inflicts irradiated on enemy hits
- Officiel : Swings a rotten club while knocking you back in the process
- Note de version : [2.1.0] Reworked Basher. It now has a custom swing animation, where the sword is rapidly swung back and forth. Holding right-click while swinging the sword causes it to deal 250% base damage and double knockback, but it will knock back the player on hits. Nerfed damag

### Bladecrest Oathsword (Weapons.Melee)

Version du code la plus proche : 2019-08 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Sword of an ancient demon lord / Fires a blood scythe
- Officiel : Throw a sharp blade that impales and slows enemies / Up to three blades can impale an enemy at a time / Consecutive hits past the first three will launch out the previous impaled blades / Hold Shift for moreA ritual blade used by Demons for sacrifices. / The more blood taints the edge, the sharper it becomes.
- Note de version : [2.0.0.001] Reworked Bladecrest Oathsword. It now has a custom swing animation and fires three scythes per swing instead of one. Buffed use time from 25 to 21.
- Note de version : [2.1.0] Reworked Bladecrest Oathsword, it no longer has a custom swing animation where it shoots scythes that home in on enemies. It now throws the blade when used, which impales into enemies. Up to 3 blades can be impaled in an enemy at a time; if another is impaled,

### Blazing Star (Weapons.Rogue)

Version du code la plus proche : 2021-10 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Stacks up to 4 / Stealth strikes release all stars at once with infinite piercing
- Officiel : Tosses a red-hot ricocheting glaive / After bouncing off of tiles, the glaive can re-hit the same enemy / Stealth strikes shred the first enemy they hit and provide rings of high-knockback orbiting glaives
- Note de version : [2.1.0] Reworked Blazing Star, it no longer throws a glaive which acts like a boomerang, and stealth strikes no longer throw 3 glaives with infinite pierce. It now throws a glaive which pierces and can bounce off tiles, ricocheting to the closest enemy when doing so. 

### Blissful Bombardier (Weapons.Ranged)

Version du code la plus proche : 2018-12 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires flare rockets
- Officiel : Launches a crystalline missile high into the sky, raining holy fire on the cursor / The missile relentlessly seeks out a target before bursting into a massive explosion
- Note de version : [2.1.0] Reworked Blissful Bombardier, it no longer fires homing rockets which spawn lingering flare dust as they travel and on hit. It now launches a holy missile straight upwards. This missile then rains a barrage of smaller fires around the cursor, before dropping t

### Blood Pact (Accessories)

Version du code la plus proche : 2020-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Doubles your max HP / Allows you to be critically hit 25% of the time / After a critical hit, you gain various buffs for ten seconds / Any healing potions consumed during this time period heal 50% more health
- Officiel : Increases max HP by 25% / Healing Potions are 33% more effective / Grants immunity to Bleeding, Burning Blood and Heavy Bleeding

### Bloodfire Arrow (Ammo)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Heals you a small amount on enemy hits
- Officiel : High-velocity arrows with a blood soaked tip / Firing these arrows will drain your health and boost the arrow damage by 20% / Accelerates your natural life regen and heals you on hit
- Note de version : [2.0.4.001] Reworked Bloodfire Arrow, it no longer has a chance to heal 1 health on hit. It now removes 1 health from the player for each arrow fired, but arrows increase natural life regeneration on hit and provide lifesteal which scales based on the player's missing hea

### Bloodflare Core (Accessories)

Version du code la plus proche : 2020-11 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : You lose up to half your defense after taking damage / Lost defense regenerates over time / You gain 1 health for every 1 defense gained as it regenerates
- Officiel : Grants immunity to knockback / Recovering defense damage heals you, each defense recovered gives 1 HP / Taking damage converts at least half of your total defense in defense damage
- Note de version : [2.0.3.007] Reworked Bloodflare Core, it no longer directly reduces defense when hit, then regenerates it to heal 1 health for each point regained. It now makes all hits deal at least half the player's total defense in defense damage, and makes recovering defense damage h

### Bloody Mary (Potions)

Version du code la plus proche : 2021-01 (ressemblance 0.93, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Boosts damage by 15%, movement speed by 10%, melee speed by 15% and crit chance by 7% during a Blood Moon / Reduces life regen by 2 and defense by 4%
- Officiel : Increases enemy spawn rates by 7x / Greatly reduces piercing weapon effectiveness / Drinking more than 3 different alcohols might not end well with your liver

### Bloody Worm Tooth (Accessories)

Version du code la plus proche : 2018-12 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : 5% increased damage reduction and increased melee stats / 10% increased damage reduction and melee stats when below 50% life
- Officiel : 10% increased melee damage

### Botanic Planter (Placeables)

Version du code la plus proche : 2019-04 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Used for special crafting
- Officiel : (pas de page)

### Bounding Potion (Potions)

Version du code la plus proche : 2021-04 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Grants 5% increased jump speed, 25 extra blocks of fall damage resistance, and increased jump height
- Officiel : 5% increased jump speed and 20% increased jump height

### Brackish Flask (Weapons.Rogue)

Nom actuel : Whitewater. Version du code la plus proche : 2020-06 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Explodes into poisonous seawater blasts / Stealth strikes summon a brackish spear spike
- Officiel : Throws a flask of magic white water at the cursor that glows after a short while / Flasks that hit enemies while glowing release four water spears that converge back on the flask's position / If the flask hits an enemy or gets too far, it will return to the owner / Stealth strikes create six water spears and summon a swirling whitewater vortex that heals players and knocks back enemies / Allies that enter the vortex will heal for twice as long
- Note de version : [2.1.0] Renamed Brackish Flask to Whitewater.
- Note de version : [2.1.0] Reworked Whitewater, it no longer throws a flask that shatters into seawater and a water blast on hit, and stealth strikes no longer additionally summon a water spear spike. It now throws a whitewater flask that follows the cursor. Hitting an enemy with the fl

### Burning Sea (Weapons.Magic)

Version du code la plus proche : 2019-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires a bouncing brimstone fireball that splits into homing fireballs upon collision with water
- Officiel : Conjures a massive fireball that hovers near you and incinerates foes with Brimstone Flames / Holding the fireball for too long causes it to burn out / This sets you alight with Brimstone Flames and gives a long delay before another can be cast
- Note de version : [2.1.0] Reworked Burning Sea, it no longer fires gravity-affected fireballs that burst into homing fires when touching water. Holding left-click now casts a fireball that grows in size over 4 seconds. When charged, the fireball will follow the cursor, but it cannot be

### Calcium Potion (Potions)

Version du code la plus proche : 2019-06 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Grants immunity to fall damage
- Officiel : Grants 50% knockback resistance

### Caribbean Rum (Potions)

Version du code la plus proche : 2021-02 (ressemblance 0.92, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Boosts life regen by 2, movement speed by 10% and wing flight time by 20% / Makes you floaty and reduces defense by 10%
- Officiel : Increases move speed by 25% / Reduces your gravity by 50% / Drinking more than 3 different alcohols might not end well with your liver

### Celestial Jewel (Accessories)

Nom actuel : Infected Jewel. Version du code la plus proche : 2018-12 (ressemblance 0.89, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Boosts life regen even while under the effects of a damaging debuff / While under the effects of a damaging debuff you will gain 11 defense / Press [Astral Teleport] to teleport to a random location while no bosses are alive
- Officiel : +1 HP/s life regen / Reduces damage over time taken by 6 HP/s / Jump-starts regeneration after a damaging debuff ends / A beautiful gemstone tarnished by spaceborne rot
- Note de version : [2.0.3.003] Renamed Celestial Jewel to Infected Jewel.
- Note de version : [2.0.3.003] Reworked Infected Jewel, it no longer allows the player to randomly teleport by pressing the {{key|Astral Teleport}} key. It now increases life regeneration by 2, with an additional 4 life regeneration while inflicted with a debuff. Defense boost while inflict

### Chaos Stone (Accessories)

Version du code la plus proche : 2021-10 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : One of the ancient relics / Mana Sickness from drinking mana potions is replaced by Mana Burn / Mana Burn deals damage over time relative to the intensity of the debuff / This debuff does not reduce your magic damage
- Officiel : Disables automatic potion consumption / Items that consume mana can be used even without the required mana amount / Mana usage can overflow into negative mana, causing you to both receive and inflict Mana Burn / The lower your mana, the more intense the manafire burns / Negative Mana is capped at your maximum mana amount / Current maximum intensity with your maximum mana: -[REGEN] HP/s, and burns enemies for [DAMAGE]% of dealt damage
- Note de version : [2.1.0] Reworked Chaos Stone, it no longer replaces Mana Sickness with Mana Burn, which deals scaling damage over time based on the debuff's duration. It now disables automatic mana potion consumption, and allows mana to overflow into negative values, up to the invers

### Chaotic Fish (Fishing)

Nom actuel : Fishing. Version du code la plus proche : 2020-01 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : The horns lay a curse on those who touch it / Right click to extract essence
- Officiel : (pas de page)
- Note de version : [2.0.2.001] Renamed Chaotic Fish to Havocfish.

### Cinquedea (Weapons.Rogue)

Version du code la plus proche : 2019-10 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Stealth strikes home in after hitting an enemy
- Officiel : Stealth strikes stick into enemies before launching forward, slicing everything

### Claret Cannon (Weapons.Typeless)

Version du code la plus proche : 2021-03 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Converts musket balls into strings of 3  bloody tears that drain enemy health
- Officiel : Fires a hypercondensed claret round / The round explodes upon striking an enemy and releases blood orbs / The explosion shreds enemies, dramatically reducing their defense / Picking up the blood orbs will reduce Potion Sickness when possible / If you do not have Potion Sickness, your natural regeneration will be enhanced / Can only fire once every 10 seconds / This weapon scales with the average of all your damage stats
- Note de version : [2.1.0] Reworked Claret Cannon, it no longer fires bursts of 3 bullets, or converts Musket Balls to bloody tears that heal for 2% of the damage dealt. It now deals typeless damage instead of ranged damage, and fires a powerful claret round on a 10 second cooldown. On 

### Cleansing Blaze (Weapons.Ranged)

Version du code la plus proche : 2018-12 (ressemblance 0.89, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : 90% chance to not use gel
- Officiel : Unleashes a torrent of God Slayer Inferno which can pass through tiles / Uses gel for ammo

### Cnidarian (Weapons.Summon)

Version du code la plus proche : 2020-12 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires a seashell when enemies are near / A very agile yoyo
- Officiel : Fishes up an electric jellyfish / Periodically sends out sparks to electrocute nearby enemies. / From looking at their less sapient brethren, the Old Kingdom's inhabitants adapted tools and weapons designed for underwater efficiency. / The electric properties of ghost bells remain useful even after death, having been commonly used as conduits.
- Note de version : [2.0.0.001] Reworked Cnidarian, it no longer functions as a yoyo. It now holds out an electric jellyfish hook attached to a string, which follows the mouse cursor. If an enemy is damaged by the hook, it causes active minions to target that enemy. Only one enemy can be tar

### Coldheart Icicle (Weapons.Typeless)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Drains a percentage of enemy health on hit / Cannot inflict critical hits
- Officiel : N(ice) work

### Contaminated Bile (Weapons.Rogue)

Version du code la plus proche : 2020-04 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Throws a flask of sickly green, irradiated bile which explodes on collision / Stealth strikes make the explosion much more violent and powerful
- Officiel : Throws a flask of Irradiated bile which shatters and lingers in a cloud / Stealth strikes cause sulphuric bubbles to quickly rise from the cloud
- Note de version : [2.1.0] Reworked Contaminated Bile, it no longer throws a bile flask which shatters and creates a small explosion on hit, and stealth strikes no longer make the explosion larger and last longer. It now throws a bile flask which shatters into a lingering irradiated clo

### Core of the Blood God (Accessories)

Nom actuel : Chalice of the Blood God. Version du code la plus proche : 2020-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : 8% increased damage and damage reduction / Boosts your max HP by 10% / Healing Potions are 15% more effective / Halves enemy contact damage / When you take contact damage this effect has a 20 second cooldown
- Officiel : Increases max HP by 25% / +2 HP/s life regen and reduces the cooldown of healing potions by 25% / Taking more than 5 damage converts the hit into a damage over time bleed effect / This bleeding does not interfere with life regeneration / Drinking a healing potion removes some of the remaining bleed, up to 50% of its heal value / Unequipping this accessory while bleeding inflicts the remaining damage instantly / Grants immunity to Bleeding, Burning Blood, Heavy Bleeding and Laceration / All that for a drop of blood...
- Note de version : [2.0.3.007] Renamed Core of the Blood God to Chalice of the Blood God.
- Note de version : [2.0.3.007] Reworked Chalice of the Blood God, it no longer halves contact damage on a 20 second cooldown or increases healing potion health boost. It now reduces all damage taken to at most 5 damage, but converts the rest of the damage taken into damage over time bleedin

### Corrupt Flask (Accessories)

Nom actuel : Unholy Tonic. Version du code la plus proche : 2018-12 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : 7% increased damage reduction and +3 defense while in the corruption
- Officiel : Increases defense by 3 while in the corruption / Grants immunity to Brain Rot
- Note de version : [2.1.0] Renamed Corrupt Flask to Unholy Tonic.

### Corruption Effigy (Placeables)

Version du code la plus proche : 2021-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : When placed down, nearby players have their movement speed and crit chance increased by 10% / Nearby players also suffer a 5% decrease to their damage reduction
- Officiel : [open] when placed to gain 10% increased movement speed and critical strike chance / However, this comes at a cost of 10% damage reduction / This cannot be stacked with Crimson Effigy

### Crackshot Colt (Weapons.Ranged)

Version du code la plus proche : 2022-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Right click to throw a coin in the air. Hitting the coin with a bullet redirects the shot into the nearest enemy / Coin throws consume copper coins
- Officiel : Press [open] to toss a Copper Coin in the air / Striking a coin with a bullet causes it to ricochet into the nearest enemy / Up to 4 coins can be tossed simultaneously, and shots will ricochet off multiple coins if possible / Coins guarantee a critical strike after a short delay

### Cranium Smasher (Weapons.Rogue)

Version du code la plus proche : 2020-06 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Throws disks that roll on the ground, occasionally launches an explosive disk / Stealth strikes launch an explosive disk that can pierce several enemies
- Officiel : Alternates between throwing piercing, rolling disks and explosive disks / Stealth strikes embed into the hit foe, exploding multiple times

### Crescent Moon (Weapons.Melee)

Version du code la plus proche : 2020-01 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : People wanted the moon, let's bring the moon to them. / Fires a whip sword that summons homing crescent moons
- Officiel : Throws a lunar flail that releases homing moons at the apex of its path / Holding [use] spins the flail around you, generating Starbursts / With 20 Starbursts, hold [use] again after throwing the flail to cause it to explode / Instead of releasing moons, this explosion summons Scorpius to do immense damage and inflict Voidfrost / Lunacy derived from genius
- Note de version : [2.1.0] Reworked Crescent Moon, it no longer functions as a whip sword which spawns homing crescent projectiles. It now functions as a flail; holding left-click spins the flail around the player, and releasing left-click launches it towards the cursor, creating homing

### Crimson Effigy (Placeables)

Version du code la plus proche : 2021-01 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : When placed down nearby players have their damage increased by 15% and defense by 10 / Nearby players also suffer a 10% decrease to their maximum health
- Officiel : [open] when placed to gain 10% damage and 5 defense / However, this comes at a cost of 10% maximum health / This cannot be stacked with Corruption Effigy

### Crimson Flask (Accessories)

Nom actuel : Vicious Tonic. Version du code la plus proche : 2018-12 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : 7% increased damage reduction and +3 defense while in the crimson
- Officiel : Increases defense by 3 while in the crimson / Grants immunity to Burning Blood
- Note de version : [2.1.0] Renamed Crimson Flask to Vicious Tonic.

### Crown Jewel (Accessories)

Version du code la plus proche : 2022-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Boosts life regen even while under the effects of a damaging debuff / While under the effects of a damaging debuff you will gain 8 defense
- Officiel : +0.5 HP/s life regen / Reduces damage over time taken by 2 HP/s
- Note de version : [2.0.3.003] | description = Reworked the Honey Dew and Crown Jewel lines and The Sponge. Multiple general rebalances.
- Note de version : [2.0.3.003] Reworked Crown Jewel, it now increases life regeneration by 2, with an additional 3 life regeneration and a boost to natural life regeneration while inflicted with a debuff. Defense boost while inflicted with a debuff now slowly fades away when no longer infli
- Note de version : [2.0.4.001] Renamed Crown Jewel to Ruby Jewel.

### Crystyl Crusher (Tools)

Version du code la plus proche : 2020-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Gotta dig faster, gotta go deeper / Right click to swing normally
- Officiel : Hold [open] to channel a beam that destroys just about anything / A primitive weapon from a bygone era...

### Deathhail Staff (Weapons.Magic)

Nom actuel : Hyperdeath Rift Scepter. Version du code la plus proche : 2020-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Rain death upon your foes! / Casts a storm of nebula lasers from the sky
- Officiel : Tears space in two with Hyperdeath Rift Ripper
- Note de version : [2.1.0] Renamed Deathhail Staff to Hyperdeath Rift Scepter.

### Deathwind (Weapons.Ranged)

Nom actuel : Thread of Eradication. Version du code la plus proche : 2021-03 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires a spread of 4 arrows / Wooden arrows are converted into nebula arrows
- Officiel : Charges up God's Bane Doombeam / Channelling the weapon causes it to charge up Hyperdeath Rift instead / While charging Hyperdeath Rift, the bow's turning speed is slowed down / Always critically strikes, with critical strike chance increasing critical strike damage
- Note de version : [2.1.0] Renamed Deathwind to Thread of Eradication.
- Note de version : [2.1.0] Reworked Thread of Eradication, it no longer fires a spread of arrows or converts Wooden Arrows into nebula shots that deal more damage. It now charges up a magenta laser beam which takes 1 second to fire. Continuing to hold left-click will cause a stronger bl

### Deep Diver (Accessories)

Version du code la plus proche : 2021-01 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : 15% increased damage, movement speed and +15 defense while underwater / While underwater you gain the ability to dash great distances
- Officiel : Allows the ability to dash with a shorter cooldown / This dash can ram through enemies without taking damage / Provides greatly improved water mobility
- Note de version : [2.0.4.001] Reworked Deep Diver, it no longer increases damage, defense, and movement speed, or grants a long dash while underwater. It now always grants a short ram dash that can be used in rapid succession, and gives full underwater mobility. It no longer gives 8 defens

### Deep Wounder (Weapons.Rogue)

Version du code la plus proche : 2020-01 (ressemblance 0.98, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Throws an abyssal hatchet that inflicts Armor Crunch and Marked for Death for Death to the enemies it hits / Stealth strikes cause the hatchet to be thrown faster and trail water, inflicting Crush Depth in addition to the other debuffs
- Officiel : Throws an abyssal hatchet / Stealth strikes cause the hatchet to stick to struck enemies, releasing a barrage of water projectiles / Water projectiles launch at the closest enemy after a brief delay

### Deific Amulet (Accessories)

Version du code la plus proche : 2022-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Causes stars to fall and grants increased immune time when damaged / Provides life regeneration and reduces the cooldown of healing potions
- Officiel : Releases a burst of homing star-bees, increases movement speed, and drenches you in Honey (buff) after taking damage / Increases the length of invincibility, with it increasing even more when at low health

### Depth Crusher (Weapons.Melee)

Version du code la plus proche : 2022-07 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Hitting enemies will inflict the crush depth debuff / The lower the enemies' defense, the more damage they take from this debuff / Right click to use without hammering down walls
- Officiel : Tosses a heavy abyssal crusher that releases water energy upon hitting a tile / The crusher will ricochet off of tiles and enemies
- Note de version : [2.0.4.001] Reworked Depth Crusher, it no longer functions like a standard broadsword or has 70% hammer power. It now throws a spinning sword on use, which bounces off enemies and tiles. Additionally, it creates homing water energies that inflict Riptide for 3 seconds on 

### Devil's Devastation (Weapons.Melee)

Version du code la plus proche : 2020-07 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires a spread of demonic scythes / Pitchforks rise from the underworld to skewer your foes / Critical hits cause shadowflame explosions
- Officiel : Throw a demonic blade that impales, immolates, and slows enemies / Up to five blades can impale an enemy at a time, with further hits launching the previous impaled blades / Press [open] to enter a stance where you can use [use] to perform a colossal, devastating slash / While in the stance, you gain life and flight time regeneration and your dashes have a shorter cooldown / The devastating slash must recharge after being used / Striking an impaled enemy with the devastating slash launches all blades and readies another slash / The final enemy hit with a devastating slash has a death mark applied to them / When you exit the stance, the death mark explodes for massive damage / Hold Shift for moreThe perfect blade of the Underworld, wielding the power of Brimstone directly. / Humans understand this force as Havoc, the inevitable sundering of all order in fire. / Perhaps this is why the Brimstone Elemental had strength unrivaled by her sisters.
- Note de version : [2.1.0] Reworked Devil's Devastation, it no longer fires a spread of accelerating scythes with pitchforks from below, or spawns shadowflame explosions when critically striking with a true melee hit. It now throws the blade when used, which impales into enemies and slo

### Downpour (Weapons.Magic)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires a spray of water that drips extra trails of water
- Officiel : Fires an explosive stream of sand that falls after hitting an enemy / The explosion only occurs when the projectile expires

### Dragon's Breath (Weapons.Ranged)

Version du code la plus proche : 2021-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Left click fires in two-shot bursts / The first shot is a spread of 6 normal bullets / The second shot is a tight spread of 6 Dragon's Breath rounds / Right click fires full auto and mixes the bullets randomly, but does 20% less damage / This weapon has no randomness to its spread pattern / 66% chance to not consume ammo
- Officiel : Fires intense blasts of Dragonfire at an accelerating pace / Upon firing for long enough, you will begin using a cartridge of energy / While using a cartridge you will shoot a draconic beam of pure Dragonfire that doesn't consume ammo / Once a cartridge has been ejected you will return to the original fire rate / 80% chance to save ammo
- Note de version : [2.0.3.007] Reworked Dragon's Breath, it no longer fires spreads of normal bullets along with Draconic Bullets, or fires random spreads of normal bullets and Draconic Bullets on right click. It now consumes Gel instead of bullets, and shoots a stream of fireballs whose fi

### Drew's Wings (Accessories.Wings)

Nom actuel : Wings of Rebirth. Version du code la plus proche : 2019-11 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Absolutely Fabulous
- Officiel : Allows flight and slow fall / Max horizontal speed: Excellent / Max vertical speed: Excellent / Horizontal acceleration: Excellent / Vertical acceleration: Great / Resplendent to the very end
- Note de version : [2.0.5] Renamed Drew's Wings to Wings of Rebirth.

### Duststorm in a Bottle (Weapons.Rogue)

Version du code la plus proche : 2020-06 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Explodes into a dust cloud / Stealth strikes form a more intense and longer lasting dust cloud
- Officiel : Disables normal stealth generation / Throws a bottle filled with high-pressure dust that shatters on impact with a target or after a short delay / Hold [use] to generate stealth, but cause the bottle to shatter sooner / Stealth strikes cause the bottle to leak dust clouds around it for a few seconds instead of shattering / Will shatter in your hand if charged for too long
- Note de version : [2.2.0] Reworked Duststorm in a Bottle, it no longer throws a bottle that shatters into a dust cloud on hit, and stealth strikes no longer create a larger and longer lasting dust cloud. It now throws a bottle that shatters into a dust cloud after a delay or when hitti

### Effulgent Manipulator (Placeables)

Nom actuel : Basins. Version du code la plus proche : 2019-04 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Used for special crafting
- Officiel : (pas de page)
- Note de version : [2.1.0] Renamed Effulgent Manipulator to Silva Basin.

### Elemental Eruption (Weapons.Ranged)

Nom actuel : Chromatic Eruption. Version du code la plus proche : 2019-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : 90% chance to not consume gel / Fires a spread of rainbow flames
- Officiel : Charges up to spew rainbow flames and pairs of homing chromatic flares / Uses gel for ammo / 50% chance to save ammo
- Note de version : [2.1.0] Renamed Elemental Eruption to Chromatic Eruption.

### Elephant Killer (Weapons.Rogue)

Version du code la plus proche : 2022-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Uses Magnum Rounds / Can be used thrice per boss battle
- Officiel : Can be tossed to gain 25% of max stealth on hit / [open] to spend 10% of max stealth and fire the gun with increased damage / All gun shots always critically strike, with critical strike chance increasing critical strike damage / Stealth strikes fire a Spectral Elephant from the gun / Any gun shots that pass through the Spectral Elephant are redirected and greatly empowered / Natural stealth generation and stealth damage is disabled when holding this weapon / Why's my gun called the Elephant Killer? / ... / Don't worry about it

### Energy Staff (Weapons.Summon)

Nom actuel : Sanctified Spark. Version du code la plus proche : 2019-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Summons a profaned energy turret to fight for you
- Officiel : Summons a sentry / Summons a friendly immolator that will float towards you / Immolators rapidly fire homing flares of Holy Flames at nearby enemies, before continuing to float near you again / After firing, Immolators will fire one healing bolt at you for every 4 summon slots you have / They might be a bit clingy, but at least they're warm
- Note de version : [2.0.2.001] Renamed Energy Staff to Sanctified Spark.
- Note de version : [2.1.0] Reworked Sanctified Spark, it no longer summons a stationary immolator sentry which rapidly fires flame bursts at enemies. It now summons an immolator sentry which follows the player. Sentries fire rapid bursts of flame bursts at enemies, with a brief pause be

### Equanimity (Weapons.Rogue)

Version du code la plus proche : 2019-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Throws a dark/light boomerang that confuses enemies / The boomerang will create light shards upon hitting enemies when thrown out, and will fire homing dark shards when returning / Stealth strikes cause the boomerang to create both dark and light shards whenever one type would be created
- Officiel : Throws a boomerang that ricochets between enemies and makes them Confused / Enemy hits alternate summoning homing light shards or piercing dark shards / Stealth strikes cause an alternating explosion of light or dark on every ricochet, empowering the opposite color of shard
- Note de version : [2.1.0] Reworked Equanimity, it no longer throws a boomerang that creates light shards when hitting enemies and fires homing dark shards when returning, and stealth strikes no longer fire both types of shards at once. It now has a custom throw animation, and throws a 

### Eutrophic Shelf (Placeables)

Version du code la plus proche : 2022-06 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Used for special crafting
- Officiel : (pas de page)

### Evergreen Gin (Potions)

Version du code la plus proche : 2022-02 (ressemblance 0.81, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Multiplies all sickness and water-related debuff damage by 1.5 and increases damage reduction by 5% / Reduces life regen by 1
- Officiel : 50% increased Water and Sickness debuff damage / 50% decreased Electric and Heat debuff damage / Drinking more than 3 different alcohols might not end well with your liver

### Excelsus (Weapons.Melee)

Nom actuel : Maw of Infinity. Version du code la plus proche : 2019-10 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires a spread of spinning blades / Summons laser fountains on hit
- Officiel : Swings a blade forged from the Devourer's maw in a 3-swing combo / The first swing only hits close range / The second swing additionally launches Faith Incinerator / The third swing unleashes Divinity Devourer at your cursor, exploding for massive damage
- Note de version : [2.1.0] Renamed Excelsus to Maw of Infinity.
- Note de version : [2.1.0] Reworked Maw of Infinity, it no longer fires spinning sword projectiles on swing or spawns laser fountains on true melee hits. It now has a custom swing animation, where the sword is swung back and forth. The sword attacks in a three-swing combo; the first swi

### Exorcism (Weapons.Rogue)

Version du code la plus proche : 2020-01 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Throws a hallowed cross which explodes into a flash of light that damages nearby enemies, closer enemies receiving more damage / As the cross travels downwards, the damage inflicted by both the cross and flash increases constantly / Stealth strikes cause the cross to be thrown with full damage immediately. Hallowed stars fall when the cross explodes
- Officiel : Throws a gigantic hallowed cross to the heavens, which will fall at your cursor a moment later / Enemies hit as it falls are impaled and repeatedly seared with holy light / Upon expiration a cross shaped explosion is created / Stealth strikes twirl and throw the cross, ending in a stronger explosion at your cursor
- Note de version : [2.1.0] Reworked Exorcism, it no longer throws a cross that creates a flash of light, and stealth strikes no longer create larger flashes and rain holy stars. It now has a custom throw animation, throwing a massive holy cross upwards, which falls at the cursor shortly

### Fallen Paladin's Hammer (Weapons.Melee)

Version du code la plus proche : 2022-07 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Explodes on enemy hits
- Officiel : Throws a heavy, gravity-affected hammer that will explode and return to you after striking an enemy / After returning three times, a homing spectral hammer is summoned with devastating impact
- Note de version : [2.0.3.001] Reworked Fallen Paladin's Hammer, it no longer throws a hammer that functions like a boomerang and explodes on hits. It now throws a fast hammer that slows down and returns to the player on enemy hits. After landing a hit with the hammer three times, an echo h

### Feather Crown (Accessories)

Version du code la plus proche : 2019-11 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : 15% increased rogue projectile velocity / Stealth strikes cause feathers to fall from the sky on enemy hits
- Officiel : Summons a Skyline Feather every 2 seconds, up to a maximum of 5 / Each Skyline Feather increases magic damage by 2% and decreases mana cost by 1% / With 5 feathers, increases magic critical strike chance by 5% and all magic attacks inflict Static Discharge / Running out of mana or drinking a mana potion sheds all feathers / Upon shedding a feather, no new feathers can be gained for 5 seconds / Feathers are transparent and silent if visibility is disabled / Hold Shift for moreYoung mages would don feathers and gaze into the starlit night, as if welcoming the breath of the heavens. / To commune with mana was to commune with the world, and further, its draconic masters.
- Note de version : [2.2.0] Reworked Feather Crown, it no longer grants 15% rogue velocity or makes stealth strikes rain feathers from the sky on hit. It now summons a feather that orbits the player every 2 seconds, up to a maximum of 5. Each feather orbiting the player grants 2% magic d

### Feralthorn Claymore (Weapons.Melee)

Version du code la plus proche : 2019-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Summons thorns on enemy hits
- Officiel : Sprouts thorns from enemies on hit

### Fetid Emesis (Weapons.Ranged)

Version du code la plus proche : 2022-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : 40% chance to not consume ammo / Has a chance to release rotten chunks instead of bullets
- Officiel : Fires an accelerating stream of bullets until the barrel overheats / When the barrel overheats, a spread of fetid chunks is vomited out / After firing, the gun must cool back down before it can be used again / 66% chance to save ammo
- Note de version : [2.1.0] Reworked Fetid Emesis, it no longer rapidly fires bullets while having a small chance to instead fire a rotten chunk that sticks to enemies. When used, it now fires bullets, with its fire rate dramatically increasing over 5 seconds. After 5 seconds, the gun fi

### Fireball (Potions)

Version du code la plus proche : 2022-02 (ressemblance 0.81, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Multiplies all fire-based debuff damage by 1.5 / Reduces life regen by 1
- Officiel : 50% increased Heat debuff damage / 50% decreased Sickness debuff damage / Drinking more than 3 different alcohols might not end well with your liver

### Firestorm Cannon (Weapons.Ranged)

Version du code la plus proche : 2019-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : 70% chance to not consume flares / Right click to fire a spread of flares
- Officiel : Fires a rapid barrage of flares that builds up a searing heat in the weapon / Firing for too long overheats the weapon, leaving it unusable for a short period and setting you On Fire! / Press [open] to put away the weapon, which preserves its heat / 33% chance to save ammo / What could go wrong?
- Note de version : [2.1.0] Reworked Firestorm Cannon, it no longer fires a constant stream of inaccurate flares, or has a right-click which fires a shotgun spread of non-piercing flares. It now fires a barrage of flares which increases in fire rate and inaccuracy over time. Firing the w

### Fish of Eleum (Fishing)

Nom actuel : Fishing. Version du code la plus proche : 2020-01 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Right click to extract essence
- Officiel : (pas de page)

### Flak Kraken (Weapons.Ranged)

Version du code la plus proche : 2019-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires an energy reticle that becomes more powerful over time
- Officiel : Fires pairs of abyssal flak shells that explode into wide cones of ink shrapnel / Abyssal flak shells burst when they reach the cursor's position / Direct hits with the flak shell will deal reduced shrapnel damage
- Note de version : [2.0.4.001] Reworked Flak Kraken, it no longer spawns an energy reticle which follows the cursor, and the gun itself no longer deals contact damage. It now consumes rockets as ammo, and fires bursts of 2 abyssal flak shells that burst into 5 ink shrapnel when hitting enem

### Flak Toxicannon (Weapons.Ranged)

Version du code la plus proche : 2020-04 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires angled shots in the direction of the cursor / Can only be shot in a cone direction above the player / High IQ required
- Officiel : Fires a toxic shell that explodes into a cone of radioactive sludge / Toxic shells burst when they reach the cursor's position / Direct hits with the toxic shell will reduce the damage dealt by the sludge / Uses rockets for ammo
- Note de version : [2.0.4.001] Reworked Flak Toxicannon, it no longer rapidly fires flak shots while aiming above the player that burst into toxic droplets. It now consumes rockets as ammo, and fires toxic flak shells that burst into 4 sludge projectiles when hitting enemies or when the pro

### Flame-Licked Shell (Accessories)

Version du code la plus proche : 2022-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : 35% decreased movement speed / Enemies take damage when they hit you / You move faster and lose 18 defense for 3 seconds if you take damage / Temporary immunity to lava
- Officiel : Grants immunity to knockback / Pressing [Activate Accessory Parry] will activate a parry, encasing you in rocks / Being hit shortly after encasement will implode the rocks / Upon implosion, a ring of brimstone hellfire is launched outwards and you receive 90% DR for that hit / After imploding, this damage reduction effect goes on cooldown / Damage parried whilst on cooldown summons weaker brimstone flames, and will not be reduced
- Note de version : [2.0.3.005] Reworked Flame-Licked Shell, it no longer decreases movement speed, deals damage to enemies and increases movement speed when hit, or grants immunity to Armor Crunch. It now grants immunity to Searing Lava, and allows the player to parry attacks by pressing th

### Flash Round (Ammo)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Gives off a concussive blast that confuses enemies in a large area for a short time
- Officiel : A ricocheting bullet that deals more damage with each bounce / The damage bonus is 1.13x or +2 flat damage per bounce, depending on which is stronger
- Note de version : [2.1.0] Reworked Flash Round, it no longer creates a medium-sized explosion on hit. It now bounces off enemies and tiles and creates a small flash which knocks away enemies. The bullet becomes affected by gravity after bouncing, and can bounce off tiles twice, gaining

### Flesh Totem (Accessories)

Version du code la plus proche : 2019-04 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Halves enemy contact damage / When you take contact damage this effect has a 20 second cooldown
- Officiel : 5% increased magic critical strike chance / Increases maximum mana by 20 / Summons an effigy above your head that stores mana as you use it, up to 600 mana / The effigy occasionally pulses, restoring 25 mana / Pulses appear more frequently the more mana is stored / Upon being struck, the effigy unleashes half of all stored up mana as vengeful souls that chase your enemies / The effigy is transparent and silent if visibility is disabled
- Note de version : [2.2.0] Reworked Flesh Totem, it no longer halves contact damage on a 20 second cooldown. It now grants 5% magic critical strike chance and 20 max mana, and summons a totem above the player's head. The totem "stores" mana the player uses, up to a maximum of 600. The t

### Forbidden Oathblade (Weapons.Melee)

Version du code la plus proche : 2019-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires a demonic scythe and critical hits cause shadowflame explosions
- Officiel : Throw a sharp blade that impales, burns, and slows enemies / Up to three blades can impale an enemy at a time, with further hits launching the previous impaled blades / Press [open] to enter a stance where you can use [use] to perform a powerful slash / The powerful slash must recharge after being used / Striking an impaled enemy with the powerful slash launches all blades and readies another slash / Hold Shift for moreA blade forged in the likeness of that granted to the Demon Lord. / The title was coveted by all demonkind. Only one may exist at a time. / Any claimant who seeks the throne has a simple goal: the head of the former Lord.
- Note de version : [2.1.0] Reworked Forbidden Oathblade, it no longer fires accelerating scythes, or spawns shadowflame explosions when critically striking with a true melee hit. It now throws the blade when used, which impales into enemies. Up to 3 blades can be impaled in an enemy at 

### Frequency Manipulator (Weapons.DraedonsArsenal)

Nom actuel : R-PLS: Pulse Grenade. Version du code la plus proche : 2020-11 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : A long device, used in the tuning of some rather... original machines. / Swings a spear around and then throws it / On collision, the spear releases a burst of homing energy / Stealth strikes release more energy and explode on collision
- Officiel : A regenerating grenade that will track enemies and burst into 8 pulse orbs / Pulse orbs will return to you and reform the grenade, hitting anything in their path / Stealth strikes throw the grenade at high speeds and burst into twice as many orbs
- Note de version : [2.1.0] Renamed Frequency Manipulator to R-PLS: Pulse Grenade.
- Note de version : [2.1.0] Reworked R-PLS: Pulse Grenade, it no longer throws a javelin which releases homing energies on hit, and stealth strikes no longer explode while releasing additional energies. It now has a custom throw animation, throwing a grenade which homes in on enemies. On

### Fungal Symbiote (Accessories)

Version du code la plus proche : 2020-07 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Various melee weapons emit mushrooms in true melee range / True melee strikes deal 15% more damage
- Officiel : You grow mushrooms on the grass beneath you / Consuming mushrooms provides Mushy, granting 3 defense and +1 HP/s life regen / 10% increased damage while you have Mushy
- Note de version : [2.0.2.001] Reworked Fungal Symbiote, it no longer grants 15% true melee damage or makes melee weapons spawn mushrooms. It now grants a 10% damage boost when using mushroom-based weapons. Mushrooms grow on grass beneath the player, and consuming Mushrooms grants the Mushy

### Galaxy Smasher (Weapons.Melee)

Version du code la plus proche : 2022-07 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Explodes and summons death lasers on enemy hits
- Officiel : Throws a heavy, gravity-affected hammer that will return to you after striking an enemy / Consecutive hits summon more and more cosmic shards / After hitting eight times, the hammer supercharges itself with galactic energies, / delivering an apocalyptic strike that reverberates through space and time
- Note de version : [2.0.3.001] Reworked Galaxy Smasher, it no longer throws a hammer that functions like a boomerang and spawns nebula lasers on hits. It now throws a fast hammer that spawns mini hammers and returns to the player on enemy hits. The more hits landed, the more mini hammers ar

### Gauss Rifle (Weapons.DraedonsArsenal)

Nom actuel : B-GSS: Nidhogg. Version du code la plus proche : 2022-04 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : A large and slow weapon, the concussive force of its projectiles do well in clearing large groups / Fires a devastating high velocity blast with absurd knockback
- Officiel : A magnet powered energy railgun that fires rapidly, creating lingering magnetic implosions that pull in weak enemies on hit / Only one implosion can exist at a time / Press [open] to begin charging a devastating energy beam that always crits and will fire once ready / This charge attack can not be interrupted once started and has a long cooldown / Many soldiers underestimate the destructive capabilities of magnetism. / This railgun has reliably pulled in skeptics of its crushing efficiency.
- Note de version : [2.1.0] Renamed Gauss Rifle to B-GSS: Nidhogg.
- Note de version : [2.1.0] Reworked B-GSS: Nidhogg, it no longer fires an instant-traveling beam which creates an explosion on hit. It now rapidly fires instant-traveling gauss shots with very slight inaccuracy. Gauss shots create a small lingering magnetic implosion which pulls in non-

### Giant Shell (Accessories)

Version du code la plus proche : 2022-02 (ressemblance 0.96, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : 15% reduced movement speed and 5% damage reduction / Taking a hit will make you move very fast for a short time
- Officiel : 10% reduced dash velocity / Taking a hit restores dash velocity and reduces defense for 3 seconds

### Giant Tortoise Shell (Accessories)

Version du code la plus proche : 2022-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : 10% decreased movement speed / Enemies take damage when they hit you
- Officiel : Grants immunity to knockback / 10% increased damage reduction and 10% reduced dash velocity / Taking a hit restores dash velocity and reduces defense and damage reduction for 3 seconds

### Gilded Proboscis (Weapons.Melee)

Version du code la plus proche : 2021-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Heals the player on hit
- Officiel : Steals life proportional to the damage dealt to foes / [use] to charge up a precise thrust / The thrust begins weak, but becomes more powerful the longer it's held / [open] to dash forward with a mighty upstrike / The distance you dash depends on if you are moving with or against the dash / Always critically strikes, with critical strike chance increasing critical strike damage
- Note de version : [2.1.0] Reworked Gilded Proboscis, it no longer functions like a standard spear. Holding left-click now charges up a thrust of the spear, which is performed when releasing left-click. The thrust takes 5 seconds to reach full charge, scaling in damage based on how long

### Gladiator's Locket (Accessories)

Version du code la plus proche : 2019-06 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Summons two spirit swords to protect you
- Officiel : Enemies drop a healing orb on kill / Gain an increase to your damage and movement speed the lower your health is, up to 20% at 50% life or below
- Note de version : [2.0.2.001] Reworked Gladiator's Locket, it no longer spawns a pair of sword minions that rotate around the player, dealing damage. It now makes enemies drop a life orb on death that heals the player for 10 health. The player is also granted a boost to damage and movement

### Glaive (Weapons.Rogue)

Version du code la plus proche : 2019-10 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Stacks up to 3 / Stealth strikes are super fast and pierce infinitely
- Officiel : Tosses a ricocheting glaive / Stealth strikes shred the first enemy they hit and provide a ring of high-knockback orbiting glaives
- Note de version : [2.1.0] Reworked Glaive, it no longer throws a glaive which acts as a boomerang, and stealth strikes no longer pierce infinitely. It now throws a glaive which pierces and can bounce off tiles, ricocheting to the closest enemy when doing so. Stealth strikes now make gl

### God's Bellows (Weapons.Ranged)

Nom actuel : Dead Sun's Wind. Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : 70% chance to not consume gel
- Officiel : Fires a concentrated blast of superheated cosmic plasma that explodes and can ricochet off blocks / Shots aggressively home in on enemies after being ricocheted / Consecutive hits cause larger explosions / Uses gel for ammo
- Note de version : [2.0.3.003] Renamed God's Bellows to Dead Sun's Wind.
- Note de version : [2.0.3.007] Reworked Dead Sun's Wind, it no longer functions like a standard flamethrower. It now fires a concentrated plasma blast which explodes on enemy hits, dealing 150% base damage. The size of the explosion scales on consecutive hits. If the plasma blast hits a til

### Godly Soul Artifact (Accessories)

Nom actuel : Faded Idolatry. Version du code la plus proche : 2022-03 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Loyalty / For each Fiery Draconid you have summoned, you gain 1 minion slot
- Officiel : Summoned Yharon's Kindle Staff Fiery Draconids cost one less minion slot / Once everything has crumbled to dust, what will be left of the world you created?

### Grand Dad (Weapons.Melee)

Version du code la plus proche : 2019-06 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Lowers enemy defense to 0 when they are struck / Yeets enemies across space and time / 7
- Officiel : An unfathomably heavy blade that rips through enemies using its sheer mass / Provides healing over time to you and all nearby allies on hit / Striking low health enemies will launch them with incredible force / Launched enemies are killed instantly on impact with walls and floors / How are you swinging something that has half the mass of a star?

### Gravistar Sabaton (Accessories)

Nom actuel : Interstellar Stompers. Version du code la plus proche : 2020-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Tap the DOWN key to increase your fall speed for 5 seconds / This has a 8 second cooldown / Striking the ground with increased fall speed will cause an astral explosion
- Officiel : 6% increased movement speed, 20% increased jump speed, and negates fall damage / Hold [Down] to increase your maximum fall speed / Pressing [Interstellar Stompers Hotkey] while midair charges up a superspeed slam over 0.33 seconds / While falling, you ram through enemies and don't take contact damage / Once you hit the ground, you create an explosion at your feet / The explosion scales in both damage and size the longer you slam for / Temporary 40% increased jump speed after slamming into the ground
- Note de version : [2.0.3.002] Reworked Gravistar Sabaton, it no longer makes pressing {{key|Down}} increase fall speed for 5 seconds on an 8 second cooldown or makes astral explosions when falling faster. It now increases jump speed by 20% and grants immunity to fall damage. Holding the {{
- Note de version : [2.1.0] Renamed Gravistar Sabaton to Interstellar Stompers.

### Greatsword of Judgement (Weapons.Melee)

Version du code la plus proche : 2020-03 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : A pale white sword from a forgotten land / You can hear faint yet comforting whispers emanating from the blade / No matter where you may be you are never alone / I shall always be at your side, my lord / Fires a white orb that emits white rain on death for a time
- Officiel : Fires a piercing energy wave that releases homing stars as it flies / Hits with the blade summon two stronger stars from behind you
- Note de version : [2.1.0] Reworked Greatsword of Judgement, it no longer fires a beam that spawns side projectiles as it travels, and a lingering aura on hit that continuously rains whiterain projectiles. It now has a custom swing animation, and fires a large piercing wave that gradual

### Greentide (Weapons.Melee)

Version du code la plus proche : 2019-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Summons teeth from the sky on hit
- Officiel : Launches a spread of four teeth with one stronger piercing tooth in the center / Hitting with the blade creates two jaws of teeth to chomp on the enemy closest to your cursor

### Hallowed Rune (Accessories)

Version du code la plus proche : 2021-05 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Whenever your minions hit an enemy you will gain a random buff, does not stack with downgrades / These buffs will either boost your defense, summon damage, or life regen for a while / If you have the offensive boost, enemies hit by minions will sometimes be hit by stars
- Officiel : Minion attacks provide various buffs / Does not stack with downgrades / Hold Shift for more  Hallowed Power grants 10% increased summon damage and rains stars on enemies / Hallowed Defense grants 5 defense / Hallowed Regen grants +1 HP/s life regen

### Harvest Staff (Weapons.Summon)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Casts flaming pumpkins
- Officiel : Summons a sentry / Grows a pumpkin patch that sprouts pumpkins nearby / Pumpkins will hop out when approached by nearby creatures or players / Once out, they will aggressively tackle enemies, exploding on hit / No thoughts behind those eyes
- Note de version : [2.0.4.001] Reworked Harvest Staff, it no longer fires bouncing pumpkin projectiles. It now deals summon damage instead of magic damage, and spawns a pumpkin patch sentry which grows pumpkins around it. Pumpkins will remain in the ground until an enemy or player passes ov

### Hellborn (Weapons.Ranged)

Version du code la plus proche : 2021-06 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires a spread of 3 bullets / Converts musket balls into explosive bullets / Enemies that touch the gun while it's being fired trigger a massive explosion / After the explosion, this gun gains a massive boost to damage, fire rate and knockback for 10 seconds / These stat bonuses slowly decay over time
- Officiel : An ornate firearm created during the Draconic Era by Azafure's finest forgemaster / Hold [use] to twirl the gun in your hand, rapidly hitting nearby enemies / Hits with the gun itself add rounds to the gun, up to a max of 12 / At the cost of one round, hold [open] to fire a giant energy sphere with potent recoil / Know the grace of a gun forged through beauteous inspiration
- Note de version : [2.1.0] Reworked Hellborn, it no longer consumes ammo, fires a spread of bullets, or grants the player large damage, attack speed, and knockback boosts when dealing damage with the gun itself. It now causes the player to twirl the gun in their hand, dealing 8% base da

### Hellkite (Weapons.Melee)

Version du code la plus proche : 2019-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Contains the power of an ancient drake / Summons flame geyser explosions on enemy hits
- Officiel : An ancient blade created during the Draconic Era by Azafure's finest forgemaster / Swings a slow and heavy blade with powerful knockback / Holding [open] will begin charging a strengthened swing / Releasing [open] while fully charged will unleash a devastating slash that always critically strikes / Feel the strength of a blade forged through passion
- Note de version : [2.1.0] Reworked Hellkite, it no longer functions like a broadsword that spawns explosions on hit. It now has a custom swing animation, where the sword is swung back and forth. Hitting an enemy launches them away from the player. Holding right-click will channel a str

### Hide of Astrum Deus (Accessories)

Version du code la plus proche : 2021-01 (ressemblance 0.93, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Taking damage, or inflicting a true melee strike, drops an immense amount of blight stars from the sky / Taking damage boosts true melee damage by 50% / Boost duration is based on the amount of damage you took, the higher the damage the longer the boost / Provides immunity to the astral infection, the astral blight, cursed inferno, on fire, and frostburn debuffs / Enemies take damage when they hit you and are inflicted with the astral blight debuff
- Officiel : True melee strikes drop astral stars from the sky / Releases an explosion and temporarily increases true melee damage by 30% when damaged / Boost duration scales with the amount of damage you took, up to 10 seconds / Nullifies the Astral Infection (debuff)

### Holy Fire Bullet (Ammo)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Explosive holy bullets
- Officiel : Profaned bullets that deal 1.15x damage and explode on impact / The explosion is larger and the bullet deals increased damage if impact occurs immediately after firing / The extra damage bonus caps at 1.15x

### Honey Dew (Accessories)

Version du code la plus proche : 2022-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : 5% increased damage reduction, plus 9 defense and increased life regen while in the Jungle / Poison and Venom immunity / Honey-like life regen with no speed penalty / Most bee/hornet enemies and projectiles do 75% damage to you
- Officiel : Natural life regen is 50% more potent
- Note de version : [2.0.3.003] | description = Reworked the Honey Dew and Crown Jewel lines and The Sponge. Multiple general rebalances.
- Note de version : [2.0.3.003] Reworked Honey Dew, it no longer increases defense, damage reduction, and life regeneration while in the Jungle, grants immunity to Poisoned and Acid Venom, or reduces damage from bee and hornet enemies. It now increases max health by 30, halves the duration o

### Hydra (Weapons.Ranged)

Version du code la plus proche : 2022-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Uses Explosive Shotgun Shells / Can be used once per boss battle
- Officiel : Fires a spread of 3 blood globs imbued with Acid Venom / Grows a gun-head every 3 seconds, up to a maximum of 3 heads / Press [use] to command any existing heads to fire / Press [open] to launch the heads for powerful explosive damage / Fusion of heavy machinery and Lernaean blood
- Note de version : [2.0.2.001] Reworked Hydra. It now uses Bullets as ammo and deals ranged damage instead of typeless. While holding the weapon, an extra gun grows every three seconds, up to a maximum of three extra guns. Left-clicking makes the weapon and any existing extra guns fire 4 no

### Hyperius Bullet (Ammo)

Version du code la plus proche : 2019-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Your enemies might have a bad time / Spawns additional bullets on enemy hits
- Officiel : Dazzling multicolor bullets that split into swirling lights on impact / Damage that would be dealt from the bullets is instead stored on the target / As more damage is stored, it will begin to overflow, slowly releasing and damaging the enemy / Attacks from sources other than Hyperius Bullets will release some of the charge / Releasing charge in this way is 20% more efficient
- Note de version : [2.0.4.001] Reworked Hyperius Bullet, it no longer spawns an additional bullet in a random nearby position and direction on hit. It now splits into 3 bullets on hit that swirl outwards from the point of impact and deal 15% base damage with an extra 10 flat damage. Enemy h
- Note de version : [2.1.0] Reworked Hyperius Bullet. Main and split bullets no longer directly deal damage, instead storing damage on the enemy based on how much damage it would have dealt. If the stored damage is greater than 7% of the enemy's max health, it will overflow and release s

### Icicle Arrow (Ammo)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Shatters into shards on impact
- Officiel : Once shot, arrows quickly freeze into falling icicles / Hitting enemies or tiles before the arrow fully freezes causes it to bounce upwards
- Note de version : [2.0.4.001] Reworked Icicle Arrow, it no longer spawns ice shards on hit. Arrows now pierce infinitely and ignore gravity, but slow down as they travel, eventually falling. If it hits an enemy, the arrow will rise upwards before falling. Falling arrows deal 135% damage, b

### Icy Bullet (Ammo)

Nom actuel : Hailstorm Bullet. Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Can hit up to three times / Breaks into ice shards on last impact
- Officiel : Once shot, bullets rapidly decelerate / Critical hits cause a flash freeze, dealing additional damage based on your crit chance and freezing enemies
- Note de version : [2.0.4.001] Renamed Icy Bullet to Hailstorm Bullet.
- Note de version : [2.0.4.001] Reworked Hailstorm Bullet, it no longer spawns ice shards on hit. It now slows down and disappears after traveling a medium distance, and inflicts Frostburn on hit. Critical strikes inflict Frostbite and create a flash freeze on the enemy that deals 48% base d

### Insidious Impaler (Weapons.Melee)

Version du code la plus proche : 2019-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires a harpoon that sticks to enemies and explodes
- Officiel : Throws self-propelling radioactive harpoons which chase foes / Harpoons ricochet at close range for additional hits
- Note de version : [2.1.0] Reworked Insidious Impaler, it no longer throws gravity-affected harpoons that stick to enemies and explode after a delay. It now throws harpoons that home in on enemies. On hit, harpoons briefly bounce off enemies before launching towards them again, dealing 

### Ion Blaster (Weapons.Magic)

Version du code la plus proche : 2021-03 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires ion blasts that speed up and then explode / Damage scales with how full your mana is / Using Astral Injection reduces the effectiveness of the mana boost
- Officiel : Rapidly fires ion blasts with heavy armor penetration that accelerate and explode / Press [open] to charge the blaster, restoring your mana until it reaches max / The more mana you restore, the stronger the burst is when you release it / Charging for 90% of your max mana or more will reach max power / Holding this weapon disables the pickup of stars
- Note de version : [2.1.0] Reworked Ion Blaster, it no longer fires blasts whose damage scales based on the percentage of max mana the player has. It now rapidly fires blasts which explode on hit, dealing 30% base damage. All projectiles have an innate 40 armor penetration. Holding righ

### Iron Boots (Accessories)

Version du code la plus proche : 2018-12 (ressemblance 0.92, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Allows you to fall faster in liquids
- Officiel : Hold [Down] to increase your maximum fall speed, even in water / Upon hitting the ground, a shockwave is created at your feet / The shockwave scales in both damage and size the longer you fall for / Negates fall damage
- Note de version : [2.1.0] Reworked Iron Boots, it no longer increases fall speed in liquids. It now grants immunity to fall damage, and makes holding DOWN massively increase the player's max fall speed. Hitting the ground while falling faster creates a shockwave that inflicts Armor Cru

### Lacerator (Weapons.Melee)

Version du code la plus proche : 2020-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Enemies that are hit by the yoyo will have their life drained / A very agile yoyo / Someone thought this was a viable weapon against DoG at one point lol
- Officiel : Reach: 35 tiles / Speed: 48 / Infinite throw duration / Striking enemies conjures a bloody saw to shred them / The saw does more damage the more often the yoyo hits, and fades away when not striking enemies / Enemies touching the edge of the sawblade get blood orbs torn out of them / Picking up these blood orbs reduces the duration of Potion Sickness / The direction of the saw's spin depends on which direction you originally threw the yoyo
- Note de version : [2.1.0] Reworked Lacerator, it no longer increases natural life regeneration on hit. Hitting enemies now inflicts Laceration for 3 seconds and spawns a blood saw on top of the yoyo which rapidly shreds enemies, dealing more damage the more hits are landed with the yoy

### Leviathan Ambergris (Accessories)

Version du code la plus proche : 2022-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : You leave behind poisonous seawater as you move / 30% increased movement speed, 10% increase to all damage, and plus 10 defense while submerged in liquid / If you are damaged while submerged in liquid you will gain a damaging aura for a short time / Being outside of liquid increases all damage by 5% and increases damage reduction by 5%
- Officiel : 100% increased initial dash velocity, 33% faster dash cooldown, and greatly increased dash deceleration / Your dashes are empowered by water, knocking back and damaging enemies with vicious currents / Visual effects are reduced if visibility is disabled
- Note de version : [2.0.3.005] Reworked Leviathan Ambergris, it no longer provides increased damage, defense, and movement speed or a damaging aura while in liquids, increases stats outside of liquids, or leaves behind a trail of poisonous seawater. It now increases dash velocity by 15%, an

### Leviatitan (Weapons.Ranged)

Version du code la plus proche : 2021-03 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Converts musket balls into toxic and normal water blasts
- Officiel : Fires high velocity aqua blasts / Releases homing aberrations that shred enemy defense every six shots / [open] to vomit a meteor that explodes on hit / The L in LMG stands for Leviathan
- Note de version : [2.1.0] Reworked Leviatitan, it no longer converts Musket Balls into toxic water blasts. It now rapidly fires aqua blasts on use that inflict Riptide. Every sixth shot of the weapon additionally spawns two homing aberrations which deal 130% base damage and inflict Arm

### Life Jelly (Accessories)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : +20 max life / Standing still boosts life regen
- Officiel : Using a healing potion spawns a pink aura that lasts for 30 seconds / The aura grants +2 HP/s life regen
- Note de version : [2.0.3.002] Reworked Life Jelly, it no longer boosts max health by 20 or increases life regeneration when standing still. It now spawns an aura when using a healing potion which lasts for 45 seconds. While inside the aura and for 5 seconds after leaving it, the player is 

### Living Dew (Accessories)

Version du code la plus proche : 2020-07 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : 5% increased damage reduction, +5 defense, and increased life regen while in the Jungle / Immunity to Poison
- Officiel : Natural life regen is 75% more potent and returns 100% faster
- Note de version : [2.0.3.003] Reworked Living Dew, it no longer increases defense, damage reduction, and life regeneration while in the Jungle, or grants immunity to Poisoned. It now increases max health by 50, halves the duration of fire and sickness debuffs, gives permanent honey life re

### Lumenous Amulet (Accessories)

Nom actuel : Diamond of the Deep. Version du code la plus proche : 2021-09 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Attacks inflict the Crush Depth debuff / Grants immunity to the Crush Depth debuff / While in the abyss you gain 25% increased max life / Provides a moderate amount of light in the abyss
- Officiel : 60% increased Water debuff damage / Slowly accrue up to three different abyssal spirits / Dash to disperse the spirits and have them attack enemies / When dispersed below 50% life, the spirits support you instead / The spirits appear in order and have different effects based on color: / Explodes on impact and inflicts Crush Depth / Heals for 8 HP / Pierces enemies and inflicts Hellfire / Increases defense by 10 temporarily / Hits multiple times and inflicts Hadopelagic Pressure / Heals 5% of max HP / Spirits are transparent and silent when visibility is toggled off
- Note de version : [2.1.0] Renamed Lumenous Amulet to Diamond of the Deep.
- Note de version : [2.1.0] Reworked Diamond of the Deep, it no longer increases max health or grants light while in the Abyss, makes attacks inflict Crush Depth, or grants immunity to Riptide and Crush Depth. It now spawns abyssal spirits every 2.5 seconds which follow the player, up to

### Luxor's Gift (Accessories)

Version du code la plus proche : 2019-06 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Weapons fire unique projectiles based on the damage type they have / Some weapons are unable to receive this bonus
- Officiel : Summons a scarab launcher that follows you closely / The scarab launcher fires different projectiles at enemies depending on what class the held weapon belongs to / Held item class is not updated when holding a tool / When not in use, the scarab will condense itself into a small orb of light
- Note de version : [2.1.0] Reworked Luxor's Gift, it no longer fires additional various projectiles when using weapons based on damage type. It now summons a large scarab launcher behind the player. The launcher fires various projectiles when dealing damage based on the held weapon's da

### Magna Cannon (Weapons.Ranged)

Version du code la plus proche : 2019-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires a concentrated blast of energy
- Officiel : Charges up a rapid fire burst of crystal bolts / Charging for longer increases the number of bolts fired / Fully charged shots fire an even longer and more rapid burst
- Note de version : [2.0.3.005] Reworked Magna Cannon, it no longer fires slow blasts of energy, and now deals ranged damage instead of magic. It is now charged up by holding left click to fire a rapid fire burst of crystal bolts. The longer it is charged up, the more bolts are fired. The we

### Mana Jelly (Accessories)

Nom actuel : Cleansing Jelly. Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : +20 max mana / Standing still boosts mana regen
- Officiel : Using a healing potion spawns a blue aura that lasts for 30 seconds / Walking into the aura removes all debuffs one time
- Note de version : [2.0.3.002] Renamed Mana Jelly to Cleansing Jelly.
- Note de version : [2.0.3.002] Reworked Cleansing Jelly, it no longer boosts max mana by 20 or increases mana regeneration when standing still. It now spawns an aura when using a healing potion which lasts for 45 seconds. Entering the aura will cleanse all debuffs and remove defense damage 

### Mana Polarizer (Accessories)

Version du code la plus proche : 2020-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Increases max mana by 50 and magic damage by 6% / Life regen lowered by 3 if mana is above 50% of its maximum / Grants spectre healing, the amount healed scales with your mana / The healing only works while holding a magic weapon / The more mana you have, the more you heal
- Officiel : Increases maximum mana by 40 / Provides life regeneration and magic damage that scales based on how much mana you have / Lower mana means more life regeneration, but less magic damage / These scale from -2 to +2 HP/s, and 15% to 5% damage / The provided regeneration is halved when under Mana Sickness / Magic projectiles steal enemy life

### Mantis Claws (Weapons.Melee)

Version du code la plus proche : 2019-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Explodes on enemy hits
- Officiel : Rapidly swing a mantis claw which slashes foes in short range / Press [open] to  form a pressurized bubble in front of you / Once fully formed, the bubble is instantly snapped down on, blasting foes with a water jet / It's just that shrimple
- Note de version : [2.1.0] Reworked Mantis Claws, it no longer functions like a broadsword that creates visual explosions on hit. It now spawns slash projectiles that curve outwards and linger for a short time, inflicting Heavy Bleeding for 3 seconds on hit. Using right-click channels a

### Matter Modulator (Weapons.DraedonsArsenal)

Nom actuel : R-PMA: Holofibre Immolator. Version du code la plus proche : 2022-04 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Using extra mass gained from collision with solid materials, it causes extra damage / Fires a burst of unstable matter that does significant damage after striking a tile / Before striking a tile, the matter pierces infinitely but deals little damage
- Officiel : Fires three gravity-affected plasma drops which dissolve through foes / Holding [use] quickly charges the bow, firing a high-speed plasma arrow / Plasma arrows will detonate after impact, creating stronger explosions if stuck into tiles / Holding [open] will repeatedly fire uncharged shots / The elegance of holofibre is unmatched. / However, mechanical tension may have produced more consistent results.
- Note de version : [2.1.0] Renamed Matter Modulator to R-PMA: Holofibre Immolator.
- Note de version : [2.1.0] Reworked R-PMA: Holofibre Immolator, it no longer fires an unstable matter projectile which deals less damage and pierces before hitting a tile. It now fires a spread of 3 gravity-affected plasma drops which can pierce 3 times. Holding left-click quickly charg

### Meowthrower (Weapons.Ranged)

Version du code la plus proche : 2019-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : 50% chance to not consume gel / Fires blue and pink flames that emit meows on enemy hits
- Officiel : Cooks your foes to purrfection with catty smoke / Occasionally launches homing feline entities / Uses gel for ammo

### Meteor Fist (Weapons.Rogue)

Version du code la plus proche : 2020-06 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires a fist that explodes / Stealth strikes make the fist ricochet between enemies up to 4 times
- Officiel : Fires a wired-guided rocket fist that seeks the cursor and accelerates over time / Stealth strikes immediately start accelerating and drop a meteorite on the struck enemy
- Note de version : [2.1.0] Reworked Meteor Fist, it no longer fires a gravity-affected fist that explodes on hit, and stealth strikes no longer ignore gravity and bounce between enemies. It now fires a wire-guided fist which accelerates over time and homes towards the cursor. Stealth st

### Midas Prime (Weapons.Ranged)

Version du code la plus proche : 2022-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Struck enemies drop extra coins / Right click to throw a coin in the air. Hitting the coin with a bullet redirects the shot into the nearest enemy / If you have multiple coins up in the air, bullets will first redirect towards other coins up to a maximum of 4 / Coin ricochets will increase the damage of the bullet, provided the coins have been in the air for long enough / Coin throws consume gold and silver coins
- Officiel : Press [open] to toss a Gold Coin or Silver Coin in the air / Striking a coin with a bullet causes it to ricochet into the nearest enemy / Up to 4 coins can be tossed simultaneously, and shots will ricochet off multiple coins if possible / Coins guarantee a critical strike after a short delay and bullets do bonus damage based on coins used

### Mirror Blade (Weapons.Melee)

Version du code la plus proche : 2019-09 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : The amount of contact damage an enemy does is added to this weapons' damage / You must hit an enemy with the blade to trigger this effect
- Officiel : Striking an enemy or a hostile projectile summons up to 10 orbiting mirror shards / Any shards above 10 instead seek out enemies / Holding [open] coalesces the orbiting mirror shards into a shield / Hold Shift for moreEach shard reduces the damage of a projectile that hit the shield by 10% / The shield shatters after one projectile, launching the shards at enemies with increased damage / With all 10 shards, you gain a brief period of invulnerability when shattering the shield'A blade taken from your own reflection within the Ceaseless Void
- Note de version : [2.1.0] Reworked Mirror Blade, it no longer fires mirror blasts or gains damage based on an enemy's contact damage when hitting with true melee. It now has a custom swing animation, but no longer fires projectiles. Hitting enemies spawns slashes that deal 50% base dam

### Molten Amputator (Weapons.Rogue)

Version du code la plus proche : 2020-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Throws a scythe that emits molten globs on enemy hits / Stealth strikes spawn molten globs periodically in flight and more on-hit
- Officiel : Throws a spinning scythe that lingers at your cursor / [open] to recall all existing scythes / Recalling a scythe while it's on an enemy rends them for massive damage / [open] with sufficient stealth to empower the next 30 scythes and greatly increase attack speed / You cannot recall scythes thrown while empowered
- Note de version : [2.1.0] Reworked Molten Amputator, it no longer fires a scythe that spawns molten globs on hit, and stealth strikes no longer spawn molten globs as they travel and even more on hit. It now throws a rapidly spinning scythe which lingers at the position of the cursor wh

### Monolith Amalgam (Placeables)

Version du code la plus proche : 2022-06 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Used for special crafting
- Officiel : (pas de page)

### Moonstone Crown (Accessories)

Version du code la plus proche : 2019-11 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : 15% increased rogue projectile velocity / Stealth strikes summon lunar flares on enemy hits / rogue projectiles very occasionally summon moon sigils behind them
- Officiel : Increases maximum mana by 40 / Summons a Lunar Sigil every 2 seconds, up to a maximum of 10 / Each Lunar Sigil increases magic damage by 2% and reduces mana cost by 1% / With 5 sigils, increases mana regeneration by +5 MP/s / With 10 sigils, increases magic critical strike chance by 10% and all magic attacks inflict Nightwither / Running out of mana or using a mana potion shatters all sigils / Upon shattering a sigil, no new sigils can appear for 5 seconds / Sigils are transparent and silent if visibility is disabled / Hold Shift for moreCountless tears were shed over Chrysadia's marring: when its eventide clouds gave way not to beloved stars, / or to dear patrons, but to a ravenous void and its unyielding dread.
- Note de version : [2.2.0] Reworked Moonstone Crown, it no longer grants 15% rogue velocity, makes rogue projectiles spawn trailing moon sigils, or makes stealth strikes rain lunar flares from the sky on hit. It now grants 40 max mana, and summons a lunar sigil that orbits the player ev

### Mythril Knife (Weapons.Rogue)

Version du code la plus proche : 2020-06 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Stealth strikes inflict a wide assortment of debuffs
- Officiel : Stealth strikes are coated in deadly toxins, inflicting Irradiated, Poisoned, and Acid Venom

### Neptune's Bounty (Weapons.Melee)

Version du code la plus proche : 2020-03 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires a trident that rains additional tridents as it travels / Hitting enemies will inflict the crush depth debuff / The lower the enemies' defense, the more damage they take from this debuff
- Officiel : Tosses a spinning abyssal sword which constantly sprays water energy / Once the blade begins falling, it will redirect itself towards enemies with increased velocity and 2x damage
- Note de version : [2.0.4.001] Reworked Neptune's Bounty, it no longer fires trident projectiles which spawn trailing, homing projectiles as they travel. It now throws a fast spinning sword which pierces and constantly shoots water blasts that inflict Crush Depth for 4 seconds towards nearb

### Nullification Pistol (Weapons.Ranged)

Version du code la plus proche : 2021-04 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Is it nullable or not? Let's find out! / Fires a fast null bullet that distorts NPC stats
- Officiel : Fires discrete bolts of polarized energy. [open] to swap modes / Positive energy splits into three piercing beams / Negative energy shoots rapidly and homes in on enemies / All shots blow away enemies at close range / Each mode weakens with continuous use and resets to full power when swapped / Weaponizing alien tech? Now that's what I like to hear!
- Note de version : [2.1.0] Reworked Nullification Pistol, it no longer fires null shots which cause random effects on hit enemies. It now fires one of two types of energy bolts, which can be swapped between using right-click. Positive energy bolt projectiles split into three, and negati

### Oblivion (Weapons.Melee)

Version du code la plus proche : 2022-07 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires brimstone lasers when enemies are near / A very agile yoyo
- Officiel : Reach: 32 tiles / Speed: 36 / Duration: 15 seconds / A highly unstable yoyo that slowly releases homing brimstone hellblasts / Enemy hits with the yoyo charge its power, greatly increasing the fire rate of the hellblasts / At maximum charge, the yoyo bursts into brimstone darts and resets its flight duration / Striking an enemy at maximum charge will overload the yoyo, causing it to release all stored up power and shatter / Shattering the yoyo deals immense damage but leaves it unable to be used for several seconds
- Note de version : [2.2.2] Reworked Oblivion, it no longer fires darts at nearby enemies. It now slowly releases homing hellfireballs while held out. Hitting enemies with the yoyo charges it up, increasing the frequency of hellfireballs. After 20 hits, the yoyo reaches full charge, whic

### Ocean Crest (Accessories)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Most ocean enemies become friendly and provides waterbreathing
- Officiel : You recover air while standing still underwater / 15% increased mining speed while wet
- Note de version : [2.0.3.007] Reworked Ocean Crest, it no longer provides water breathing or makes ocean enemies friendly. It now causes the player to recover air while standing still underwater, and increases mining speed by 15% while wet.

### Old Lord Oathsword (Weapons.Melee)

Nom actuel : Old Lord Claymore. Version du code la plus proche : 2019-08 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : A relic of the ancient underworld / Critical hits cause lava explosions
- Officiel : A slow but powerful blade that sets enemies ablaze and has substantial knockback / Hold Shift for moreA gift to outstanding Demons, a replica of the sword used by the greatest of all Demon Lords. / The prosperity he brought to their kind is unmatched even now that they alone rule the Underworld.
- Note de version : [2.0.0.001] Reworked Old Lord Oathsword. It now has a custom swing animation and no longer has an effect on critical strikes. The player can right click to hold the sword upwards while charging up. If fully charged, the player will spin in the direction of the cursor whil
- Note de version : [2.0.2.001] Renamed Old Lord Oathsword to Old Lord Claymore.
- Note de version : [2.1.0] Reworked Old Lord Claymore, it no longer performs a small slash with left-click or can be charged up with right-click to perform a lunge which can ram through enemies and spawn bombs. It now performs a very slow slash which deals heavy knockback and inflicts H

### Onyx Excavator Key (Mounts)

Version du code la plus proche : 2020-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Summons a drill to drill through the world so you can destroy all the neat world generation / with complete disregard for all the creatures that inhabit these lands. I am sure the EPA and PETA would like / to have a word with you afterwards. / The power of the destruction scales with the highest powered pickaxe in your inventory
- Officiel : Summons a rideable Onyx Drill mount / Hold Left Click to drill blocks while moving either horizontally or downwards / The drill's pickaxe power and mining speed copy the best pickaxe in your inventory / Irresponsibly destroying all the neat world generation and causing unchecked geological devastation? Splendid

### Opal Striker (Weapons.Ranged)

Version du code la plus proche : 2018-12 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires a string of opal strikes
- Officiel : Fires a chargeable heated energy bolt that pierces / Fully charged shots deal extra damage, move faster and inflict Hellfire
- Note de version : [2.0.3.005] Reworked Opal Striker, it no longer fires bursts of rapid fire opal strikes, and no longer consumes any ammo. It now fires a fast opal bolt which pierces once and inflicts On Fire! for 1 second. Holding left click will charge up the weapon for up to 1.3 second

### Ornate Shield (Accessories)

Version du code la plus proche : 2022-01 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Boosted damage reduction and health while wearing the Daedalus armor / Grants a frost dash
- Officiel : Allows the ability to dash / This dash can ram through enemies without taking damage

### Overloaded Blaster (Weapons.Ranged)

Version du code la plus proche : 2019-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : 33% chance to not consume gel / Fires a large spread of bouncing slime
- Officiel : Converts gel into a spread of energized slime pearls that overload when they come to a stop / Overloaded slime pearls deal 60% more damage but no longer pierce / Slime pearls bounce off of walls at very high speed / Uses gel for ammo / 25% chance to save ammo
- Note de version : [2.0.3.007] Reworked Overloaded Blaster, it no longer fires slow, piercing slime bolts. It now shoots piercing slime bolts which eventually slow down and stop after traveling a medium distance, becoming overloaded. When overloaded, slime bolts deal 200% base damage but no

### Pandemic (Weapons.Melee)

Version du code la plus proche : 2020-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires plague seekers when enemies are near / A very agile yoyo
- Officiel : Reach: 30 tiles / Speed: 24 / Duration: 15 seconds / Each unique enemy struck that is infected with Plague increases the yoyo's size, power, and airtime / Refreshes Plague duration if a target is already infected / Counterweights inflict Plague and move twice as fast
- Note de version : [2.2.0] Reworked Pandemic, it no longer spawns plague seekers when near enemies, or directly inflicts Plague on hit. Hitting an enemy that is inflicted with Plague now refreshes the duration to 5 seconds, increases the yoyo's size and damage based on the number of ene

### Pearl God (Weapons.Ranged)

Version du code la plus proche : 2021-05 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Your life is mine... / Fires shockblast rounds that emit massive explosions and steal enemy life as well as additional bullets / Every seventh shot fires a massive shockblast
- Officiel : Alternates between firing a bullet imbued with lifesteal and 3 bullets imbued with pearlescence / Pearlescent imbued bullets create a radiant flash on impact, dealing additional damage to the enemy they hit
- Note de version : [2.0.4.001] Reworked Pearl God, it no longer fires a spread of bullets and a shockblast round that lifesteals and makes large dust explosions. It now alternates between firing 1 lifestealing bullet and 3 pearlescent bullets. These effects are granted no matter the ammo ty

### Phantasmal Fury (Weapons.Magic)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Casts a phantasmal bolt that explodes into more bolts
- Officiel : Casts a vortex of homing souls around you that repels enemies and will fly towards your cursor after a few seconds
- Note de version : [2.1.0] Reworked Phantasmal Fury, it no longer casts homing phantasmal bolts that burst into additional homing bolts on hit. It now casts a swirling vortex of souls around the player. While swirling around the player, souls deal 30% base damage but have twice as much 

### Phantasmal Ruin (Weapons.Rogue)

Version du code la plus proche : 2021-03 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires an enormous ghost lance that emits lost souls as it flies / Explodes into tormented souls on enemy hits / Stealth strikes continuously leave spectral clones in their wake
- Officiel : Fires a large ghost lance that explodes into spirits on enemy hits / Stealth strikes pierce twice and summon many spirits from the depths on the first hit
- Note de version : [2.0.3.005] Reworked Phantasmal Ruin, it no longer fires a lance that emits spirits as it travels, and stealth strikes no longer emit spectral clones. It now fires a fast ghost lance that explodes into 4 homing souls on enemy hits. Stealth strikes now pierce twice, summon

### Plantation Staff (Weapons.Summon)

Version du code la plus proche : 2020-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Summons a miniature plantera to protect you / Fires seeds, spiky balls, and spore clouds from afar to poison targets / Enrages when you get under 75% health and begins ramming enemies / Occupies 3 minion slots and there can only be one
- Officiel : Summons a miniature Plantera to devastate enemies with floral fury / There can only be one Mini Plantera at a time
- Note de version : [2.0.3.005] Reworked Plantation Staff, it no longer randomly fires seeds, bouncing thorn balls, and spore clouds at enemies, and no longer enrages and charges at enemies when below 75% health. The Mini Plantera minion now cycles between 3 attacks. It first fires thorn bal

### Plasma Caster (Weapons.DraedonsArsenal)

Version du code la plus proche : 2022-04 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Industrial tool used to fuse metal together with super-heated plasma / Right click for turbo mode
- Officiel : Press [use] to fire a large, exploding plasma bolt / Press [open] to fire in turbo mode, increasing rate of fire but with hindered accuracy

### Plasma Rifle (Weapons.Magic)

Version du code la plus proche : 2019-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires a plasma blast that explodes / Right click to fire plasma bolts
- Officiel : Fires an explosive plasma blast with pinpoint accuracy / Press [open] to rapidly fire non-explosive blasts

### Primordial Ancient (Weapons.Magic)

Version du code la plus proche : 2019-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : An ancient relic from an ancient land / Casts a gigantic blast of dust
- Officiel : Casts five primordial sand blasts that converge at your cursor after a moment and create a giant explosion / Casting this spell at max mana while without mana sickness will cause it to give you and any player it hits the Aeolian Earth buff
- Note de version : [2.1.0] Reworked Primordial Ancient, it no longer casts a fast blast of dust. It now casts five enchanted sand blasts that converge at the cursor after a delay and create an explosion. Using the weapon while at maximum mana and while not inflicted with Mana Sickness w

### Primordial Earth (Weapons.Magic)

Version du code la plus proche : 2019-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Casts a large blast of dust
- Officiel : Casts two enchanted blasts of sand that converge with each other and explode / Casting this spell at max mana while without mana sickness will cause it to give you and any player it hits the Sand's Wind buff
- Note de version : [2.1.0] Reworked Primordial Earth, it no longer casts a blast of dust. It now casts two enchanted sand blasts that converge and explode shortly after casting. Using the weapon while at maximum mana and while not inflicted with Mana Sickness will grant the player and a

### Profaned Partisan (Weapons.Rogue)

Version du code la plus proche : 2019-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires an unholy spear that explodes on death / Stealth strikes spawn smaller spears to fly along side it
- Officiel : Throws a profaned spear that splits into miniature spears upon hitting an enemy / Stealth strikes remain impaled in an enemy for 10 seconds / While an enemy is impaled, all spears that strike them instead shatter into exploding stars
- Note de version : [2.2.0] Reworked Profaned Partisan, it no longer throws a javelin that explodes on hit, and stealth strikes no longer summon additional smaller spears as they travel. It now throws a javelin that splits into holy spears on hit. Holy spears have oscillating velocity an

### Pulse Rifle (Weapons.DraedonsArsenal)

Nom actuel : A-PLS: Pulse Rifle. Version du code la plus proche : 2022-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Draedon's former pulse rifle, used in emergencies for creations which turned against him / When the pulse hits a target it will arc to another nearby target
- Officiel : Fires an enhanced pulse beam that dissipates into 4 high-frequency pulse orbs on hit / Pulse orbs aggressively track nearby targets and track faster the more they hit / Pulse orb pierce will not decrease when killing enemies or dealing low damage / My own personal weapon, used to stamp out failed creations. / I have backed up the schematics in the event of the original's destruction.
- Note de version : [2.0.4.001] Reworked Pulse Rifle, it no longer fires a hitscan beam that instantly arcs between up to 4 enemies. It now fires a hitscan beam that dissipates into 4 pulse orbs on hit. Pulse orbs briefly pause before aggressively tracking nearby enemies, dealing 50% base da
- Note de version : [2.1.0] Renamed Pulse Rifle to A-PLS: Pulse Rifle.

### Purge Guzzler (Weapons.Magic)

Version du code la plus proche : 2021-03 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Emits three beams of holy energy in a tight spread
- Officiel : Fires a barrage of holy rays that converge into two massive sweeping beams / Baptism by holy fire
- Note de version : [2.1.0] Reworked Purge Guzzler, it no longer fires slightly inaccurate beams which create explosions on hit. It now fires a barrage of beams which gets faster but more inaccurate as the weapon is fired. After 2.5 seconds of firing, the barrage of beams is replaced wit

### Raider's Talisman (Accessories)

Version du code la plus proche : 2019-02 (ressemblance 0.99, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Whenever you crit an enemy with a rogue weapon your rogue damage increases / This effect can stack up to 150 times / Max rogue damage boost is 15%
- Officiel : Stealth strikes grant 15% critical strike chance to non-stealth strikes for 10 seconds
- Note de version : [1.3.2.001] Reworked Raider's Talisman, it now provides its buffs when a throwing weapon lands a critical strike on an enemy instead of when the player is damaged. The effect now stacks up to 50 times instead of 100, for a maximum throwing boost of 15% instead of 40%. Rem
- Note de version : [2.0.1.002] Reworked Raider's Talisman, it no longer grants a stacking rogue damage boost from landing critical strikes. It now increases non-stealth strike rogue critical strike chance by 12% after performing a stealth strike. This effect decays over 5 seconds. This effe

### Regenator (Accessories)

Nom actuel : Regenerator. Version du code la plus proche : 2019-06 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Reduces max HP by 50% but greatly improves life regeneration
- Officiel : Your health cannot exceed 50% of its maximum / Increases length of invincibility after taking damage / Provides powerful special life regen to heal you for 7.5 HP/s / All normal life regen is negated and converted into damage / Current damage boost: [DAMAGE]%
- Note de version : [2.1.0] Renamed Regenator to Regenerator.
- Note de version : [2.1.0] Reworked Regenerator, it no longer grants 6 HP/s life regeneration and greatly increased natural life regeneration. It now increases the immunity frames from taking damage. All life regeneration from the player is negated, but the player is given a separate re

### Relic of Convergence (Tools)

Version du code la plus proche : 2021-06 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Creates a profaned crystal that charges power / Holding out the crystal slows the player down / At the end of its life, the crystal heals the player for 70 HP
- Officiel : Creates a profaned crystal at the cursor that channels a healing aura before shattering / All players within the area of effect are healed for 50 HP / Allies gain 50% extra healing / Holding out the crystal reduces the damage of all summons, decreases defense, and increases incoming damage / Never once had I known kindness. Neither my peers or family had a shred of compassion for me. / I wished for someone I could confide. A presence that cleansed me of anxiety. / And when I saw Her radiant smile when we met, I knew I had finally found it.
- Note de version : [2.1.0] Reworked Relic of Convergence, it no longer holds out a crystal that slows movement for several seconds and heals the player for 70 health. It now holds out a crystal which creates an aura at the cursor's location. While the aura exists, the player has movemen

### Relic of Deliverance (Weapons.Typeless)

Version du code la plus proche : 2021-11 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Summons a spear that causes you to lunge towards the mouse position / The spear requires a charge-up. The longer the charge, the stronger the lunge / The spear disappears immediately if you are not holding this item while charging / If enough time has passed or the spear collides into a wall, the spear dies and the lunge ends
- Officiel : Using [use] mounts you on the spear; holding [use] revvs the spear, slowing you down / Release to launch yourself at high speeds, halved while a boss is alive / Losing too much speed while charging will bust the charge, resetting your speed / The dash allows you to ram through enemies and travel through tiles / Being inside tiles for too long will warp you back to where you entered / Press [open] to dismount / Holding out the spear heavily reduces the damage of all summons / This weapon scales with the average of all your damage stats / Vengeance fueled me, I had no thought other than exactment. / The dragon took everything from me, leaving my broken pieces strewn on the ground. / But when I danced by Her side, I knew there was something still worth fighting for.
- Note de version : [2.1.0] Reworked Relic of Deliverance, it no longer starts a lunging spear attack which can be redirected up to 3 times before dissipating. It now mounts the player on the spear, which can be freely turned. Right-clicking dismounts the spear. The spear allows the play

### Relic of Resilience (Tools)

Version du code la plus proche : 2020-06 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Summons a bulwark at the mouse position / The bulwark is killed by enemies and all projectiles. / On death, the bulwark explodes into a rotating burst of shards / If an enemy is in the area of the shards, its next attack is much weaker. This effect has a cooldown / After a bit of time, the shards come together to reform the original bulwark. / This reformation can only happen 3 times. / You gain a small cooldown when summoning a new bulwark. / If a bulwark already exists, using this item will relocate it
- Officiel : Holding this item creates orbiting rock shards around you and nearby allies / After 5 seconds, the relic will retain orbiting rocks even when not holding the item / Continuing to hold the item will steadily increase the number of rocks and defenses until reaching max power / While they orbit you, your minimum defense is set to 200 and minimum damage reduction to 10% / Taking a hit or pressing  while holding this item will pull in the shards, then blast them outward, crippling hit enemies / This effect has a 10 second cooldown after taking a hit, but recharges faster if holding the item / Holding out the shield heavily reduces the damage of all summons / I saw a massacre and did nothing to stop it. My vow broken, I was shunned. / My will to forge ahead vanished. My final wish was simply to not die alone. / But when She put faith in my shield, I would never again let it falter.
- Note de version : [2.1.0] Reworked Relic of Resilience, it no longer summons a bulwark at the cursor which is broken by hostile attacks and reforms, inflicting Profaned Weakness on nearby enemies while broken. It now summons an orbit of rocks around the player while held. While the roc

### Rock (Placeables)

Version du code la plus proche : 2019-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : The first object Xeroc ever created
- Officiel : When the battle was finally over, the dust having just began to settle / Where even the stone refused the course of time, there sat a monk / Who dared to question history and stare into fate / What is the true price of the past being ever eternal? / Why must our future forever haunt us?

### Rotten Brain (Accessories)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : 10% increased damage when below 75% life / 5% decreased movement speed when below 50% life / Shade rains down when you are hit
- Officiel : Taking damage spawns a dank creeper / The dank creeper flies above you, exploding into several shaderain clouds

### Rotten Dogtooth (Accessories)

Version du code la plus proche : 2021-07 (ressemblance 0.84, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Makes Stealth strikes inflict Armor Crunch, deal 10% more damage and cost 1 less unit of stealth.
- Officiel : Stealth strikes inflict Crumbling and deal 8% more damage

### Rover Drive (Accessories)

Version du code la plus proche : 2022-02 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Activates a protective shield that grants 15 defense for 10 seconds / The shield then dissipates and recharges for 20 seconds before being reactivated
- Officiel : Gain a barrier with 20 health / Halved Adrenaline loss if all damage is absorbed / The barrier will regenerate 10 seconds after being hit, taking 5 seconds to completely regenerate / Getting hit resets the timer / Can also be scrapped at an extractinator
- Note de version : [2.0.1.001] Reworked Rover Drive, it no longer periodically generates an energy shield around the player that increases their defense by 15 for 10 seconds. It now generates a protective matrix that absorbs 50 damage and increases the player's defense by 10. The protective
- Note de version : [2.0.3.003] Reworked Rover Drive, it now grants a shield with 20 durability instead of increasing max health by its current durability, up to 40. It still grants 10 defense while the shield is up, and takes 10 seconds for the shield to recharge.

### SHPC (Weapons.Magic)

Version du code la plus proche : 2021-05 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires plasma orbs that linger and emit massive explosions / Right click to fire powerful energy beams
- Officiel : Consumes Souls as ammo / Ejects orbs of unstable, concentrated soulpower that linger and explode / Orbs emit seeking souls and lingering pickup souls after collapse / Orbs have varying effects depending on Soul type used / [SOUL DESCRIPTION] / Press [open] to use the machine as a vacuum, pulling in lingering souls / Once released, a torrent of soul lasers is discharged scaling with the number of souls pulled in / Shift-[open] this item in your inventory to eject the currently loaded Soul / Tales of legends past radiate around you... / Hold Shift to listen closer'Though soul-powered machines were a near failure, potential remained in souls as a form of fuel. / To avoid my last experiments' main flaw, the Soul-Harnessing Plasma Cannon was designed to expel souls as projectiles before they exert control over the weapon. / Each soul type appears to react differently to the base plasma. Yet, lacking volunteers, obtaining potential ammunition is currently too difficult to undergo further testing. / A shame, for a weapon that could have practically unlimited ammunition in the battlefield. Perhaps some day, its potential can be truly tested.
- Note de version : [2.1.0] Reworked SHPC, it no longer fires a burst of exploding plasma orbs on left-click, or rapidly fires lasers on right-click. It now uses Souls as ammo; the weapon consumes one Soul to give 50 charges, and each use consumes one charge. Left-click now fires a singl

### SYNTAX ERROR (SummonItems)

Nom actuel : NO. Version du code la plus proche : 2018-12 (ressemblance 0.98, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : asdhgjhrew8943923203lajflfjndg89w34ut8340ofjaur823u3lr
- Officiel : U

### Sanguine Flare (Weapons.Magic)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires a blast of sanguine flares that drain enemy life
- Officiel : Charges up a sanguine flare that gains power over time / When striking an enemy, this flare causes a bloodsplosion and splits into blood orbs / Picking up these blood orbs reduces the duration of Potion Sickness
- Note de version : [2.1.0] Reworked Sanguine Flare, it no longer fires a burst of flares which heal for 2.5% of the damage dealt on hit. When used, it now charges up a sanguine flare whose damage increases as it charges, up to a maximum of 500% base damage. After 3 seconds of charging o

### Saros Possession (Weapons.Summon)

Version du code la plus proche : 2020-04 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Gain absolute control over light itself / Summons a radiant aura / Consumes all of the remaining minion slots on use / Must be used from the hotbar / Increased power based on the number of minion slots used
- Officiel : Summons a radiant aura harnessing the power of primordial light / Repeated summons increase the power of the aura / [use] with full summon slots to channel Totality Ray / Totality Ray decays while in use and recharges when not in use / A ballet of light and shadow, torn from the sky
- Note de version : [2.1.0] Reworked Saros Possession, it no longer summons a solar aura that takes up 8 minion slots and fires homing radiance orbs. It now summons a solar aura which fires homing light orbs that can bounce between enemies. Repeated summons with the weapon will instead i

### Scorpio (Weapons.Ranged)

Version du code la plus proche : 2019-08 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : BOOM / Right click to fire a nuke
- Officiel : Hold [use] to fire a barrage of homing nano missiles and charge energy / Maximum charge is reached after firing 3 barrages / [open] while holding [use] at max charge to launch a nano nuke / Nano nukes deal massive damage and reset the cooldown of missile barrages
- Note de version : [2.0.4.001] Reworked Scorpio, it no longer rapidly fires homing rockets on left click, or slowly fires homing rockets with a larger explosion radius on right click. Holding left click now fires bursts of nano missiles which pause before speeding up and homing in on enemie

### Scourge of the Desert (Weapons.Rogue)

Version du code la plus proche : 2020-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Gains velocity over time / Stealth strikes gain damage as they damage enemies
- Officiel : A javelin that will burrow through the ground and launch itself at enemies / Stealth strikes throw three javelins that will burrow multiple times
- Note de version : [2.0.3.007] Reworked Scourge of the Desert, it no longer gains speed as it travels, and stealth strikes no longer gain damage as they travel. It now throws a javelin that can burrow into tiles and launch itself at enemies. Stealth strikes now throw 3 javelins that burrow 

### Screwdriver (Potions)

Version du code la plus proche : 2021-06 (ressemblance 0.84, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : >Multiplies piercing projectile damage by 1.05 / Reduces life regen by 1
- Officiel : Greatly increases tile and wall placement range and speed / Greatly reduces damage dealt / Drinking more than 3 different alcohols might not end well with your liver

### Seadragon (Weapons.Ranged)

Version du code la plus proche : 2022-03 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : 50% chance to not consume ammo / Fires streams of water every other shot / Fires a homing rocket every 18 shots, which explodes into fire shards on death
- Officiel : Loads a 50 round magazine of alternating bullets and pressurized water jets / Water jets summon small fish that swim around you / When all bullets are fired, summoned fish swarm the cursor while you reload / 66% chance to save ammo
- Note de version : [2.2.0] Reworked Seadragon, it no longer rapidly fires bullet and water jets alongside a short-range water ring, or fires rockets and emits muzzle blasts. It now loads a magazine of 50 shots, alternating between firing bullets and water jets which inflict Riptide for 

### Sealed Singularity (Weapons.Rogue)

Version du code la plus proche : 2020-06 (ressemblance 0.99, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Shatters on impact, summoning a black hole that sucks in nearby enemies / Stealth strikes summon a black hole that lasts longer and sucks enemies with a stronger force
- Officiel : Disables normal stealth generation / Throws a contained singularity bomb that slows down and creates a damaging field after a short fuse / Hold [use] to generate stealth, but burn through the bomb's fuse / After the field collapses, the singularity seeks out the strongest target that it hit and explodes / Stealth strikes increase the size and duration of the damaging field / Will explode in your hand if you hold the fuse too long
- Note de version : [2.2.0] Reworked Sealed Singularity, it no longer throws a bomb that shatters into a black hole on hit that sucks in enemies, and stealth strikes no longer make the black hole last longer and have stronger sucking. It now throws a bomb that stops in place and creates 

### Shadecrystal Barrage (Weapons.Magic)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Summons rapid fire shadecrystals, can shoot two crystals at once
- Officiel : Summons bursts of shadecrystals that can home in on enemies

### Shaderain Staff (Weapons.Magic)

Version du code la plus proche : 2018-12 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires a shade storm cloud
- Officiel : Fires a spray of evil clouds and rain
- Note de version : [2.0.3.001] Reworked Shaderain Staff, it no longer summons a long-lasting rain cloud which drops damaging raindrops downwards similar to Nimbus Rod. It now fires a spread of 3 gravity-affected raindrops and a decelerating cloud ball which deals 125% base damage. Nerfed sh

### Shadethrower (Weapons.Ranged)

Version du code la plus proche : 2019-09 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : 66% chance to not consume gel
- Officiel : Spits clouds of shade which afflict enemies with Brain Rot / Uses gel for ammo

### Shadow Potion (Potions)

Version du code la plus proche : 2020-06 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Causes the player to disappear while not attacking / Holding different types of rogue weapons give the player boosts / Different types of rogue weapons spawn different projectiles on hit / Stealth generation is increased by 10%
- Officiel : 8% increased stealth generation / Turns the player into a shadow with glowing eyes / Visual effects can be disabled with the Stealth Invisibility config

### Shadowbolt Staff (Weapons.Magic)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : The more tiles and enemies the beam bounces off of or travels through the more damage the beam does
- Officiel : Fires a shadow beam that creates a platform to redirect itself back at enemies for increased damage
- Note de version : [2.1.0] Reworked Shadowbolt Staff, it no longer casts a bouncing shadowbolt which deals more damage for every enemy or tile it hits. It now casts a shadowbolt that can pass through tiles. After traveling far enough or hitting an enemy, the shadowbolt creates a platfor

### Shard of Antumbra (Weapons.Rogue)

Version du code la plus proche : 2022-07 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Stealth strikes leave homing stars in their wake
- Officiel : Throws a cosmic javelin that sticks into enemies and tiles, creating a small rift nearby on enemy hits / After hitting a target, it will charge with energy and teleport to the rift, aiming itself at the nearest target / Stealth strikes create a multitude of rifts to spawn additional javelins upon teleporting
- Note de version : [2.1.0] Reworked Shard of Antumbra, it no longer throws a javelin which pierces infinitely, and stealth strikes no longer spawn homing star projectiles as they travel. It now throws a javelin which sticks to enemies and tiles. After hitting an enemy, the javelin will 

### Shellfish Staff (Weapons.Summon)

Version du code la plus proche : 2019-04 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Summons a shellfish to fight for you / Takes up 2 minion slots
- Officiel : Summons a shellfish to latch onto enemies and inflict Shellfish Claps

### Shock Grenade (Weapons.Rogue)

Nom actuel : Doomsday Device. Version du code la plus proche : 2020-01 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Throws a grenade that explodes into a burst of lightning / Stealth strikes cause the grenade to leave an electrifying aura when it explodes
- Officiel : Direct hits from the weapon grant stealth / Non-stealth strikes do not consume stealth when fully charged / Holding a stealth strike charge beyond its limit rapidly drains stealth / Stealth strikes ricochet between multiple enemies, ending in a strong impact / A high-tech explosive with devastating power... If only you could figure out how to arm it / At least you can throw it really hard!
- Note de version : [2.1.0] Renamed Shock Grenade to Doomsday Device.
- Note de version : [2.1.0] Reworked Doomsday Device, it no longer throws a grenade that explodes and spawns several homing bolts on hit, and stealth strikes no longer leave a lingering electric aura. Holding left-click now quickly charges a throw, throwing the grenade when released. The

### Skyline Wings (Accessories.Wings)

Version du code la plus proche : 2021-09 (ressemblance 0.86, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : No vertical speed / 10% increased jump speed while wearing the Aerospec armor
- Officiel : Allows flight and slow fall / Max horizontal speed: Average / Max vertical speed: Average / Horizontal acceleration: Average / Vertical acceleration: Average
- « No vertical speed » : ailleurs dans le code (2021-05, Items/CalamityGlobalItemTooltip.cs : vertical speed)

### Slick Cane (Weapons.Rogue)

Nom actuel : Walking Cane. Version du code la plus proche : 2019-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Swipes a cane that steals money from enemies. / Stealth strikes gives a 1 in 15 chance for enemies to drop 1-3 gold coins when hit / Economy at its finest
- Officiel : Swipes a pointed walking cane / Stealth strikes drain the sanity of foes, causing shadow hands to be summoned when attacked / While held, increases movement speed by 20% / Is it walking me, or am I walking it?
- Note de version : [2.2.2] Renamed Slick Cane to Walking Cane.
- Note de version : [2.2.2] Reworked Walking Cane, it no longer makes enemy hits drop money, and stealth strikes no longer have scaling damage based on how much money the player is carrying. It now swipes a long walking cane towards the cursor. Stealth strikes trigger an insanity effect 

### Snake Eyes (Weapons.DraedonsArsenal)

Version du code la plus proche : 2022-04 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Surveillance drones equipped with a strong electric field which can be directed at enemies / Summons a mechanical watcher that zaps and flies around enemies
- Officiel : Summons a levitating drone which fires piercing, highly calibrated energy bolts / If a bolt hits, the drone will teleport into its path and reflect the bolt / Reflected bolts home in and explode on impact / Equipped with high-end sensors, little can evade them. Even less are able to escape their ever watchful eyes. / An excellent bridge between my experiments with the sight-based soul powered machines and my latest project.
- Note de version : [2.0.3.003] Reworked Snake Eyes, it no longer flies around enemies in a square pattern while zapping them with a hitscan beam. It now teleports to a random location near enemies and fires a fast, infinitely piercing energy bolt. Shortly after hitting an enemy, the minion 

### Soaring Potion (Potions)

Version du code la plus proche : 2020-05 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Increases flight time and horizontal flight speed by 10% / Restores a fraction of your wing flight time after a true melee strike / The amount of flight time restored scales with your melee stats and weapon swing speed
- Officiel : 10% increased flight time

### Solar Flare (Weapons.Melee)

Nom actuel : Burning Revelation. Version du code la plus proche : 2020-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Emits large holy explosions on hit / A very agile yoyo
- Officiel : Reach: 42.5 tiles / Speed: 54 / Infinite throw duration / An ever burning yoyo that constantly charges power / Direct hits greatly increase the charge and release profaned energy that scales with charge level / At maximum charge, the yoyo unleashes a volley of homing stars that gain damage the farther they travel / Stars with maximum damage let off an additional explosion on hit / It burns, but you hold it close to your heart. Clarity and purpose fill your soul
- Note de version : [2.0.4.001] Renamed Solar Flare to Burning Revelation.
- Note de version : [2.1.0] Reworked Burning Revelation, it no longer creates explosions on hit. The yoyo now constantly builds up charge as it is held out. Landing hits increases charge faster and creates explosions whose size scales with charge. When the yoyo reaches full charge, it sp

### Spark Spreader (Weapons.Ranged)

Version du code la plus proche : 2021-01 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : 70% chance to not consume gel
- Officiel : Ignites your foe with flaming sparks / Uses gel for ammo

### Spear of Destiny (Weapons.Rogue)

Version du code la plus proche : 2020-06 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Throws three spears with the outer two having homing capabilities / Stealth strikes cause all three spears to home in, ignore tiles, and pierce more
- Officiel : Cycles between throwing 3 small homing spears and throwing a single piercing lance / Stealth Strikes throw a high-velocity holy lance that explodes on hit
- Note de version : [2.0.3.005] Reworked Spear of Destiny, it no longer always throws 3 spears with the outer 2 having homing, and stealth strikes no longer make all spears home and pierce more. It now alternates between throwing 3 small homing spears and 1 piercing lance which deals 300% ba

### Speed Blaster (Weapons.Ranged)

Version du code la plus proche : 2022-07 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Shoots a string of paint blasts
- Officiel : Rapidly fires phosphorescent paintball shots / Press [open] to eject a powerful paint stream of a different color / Using this ability rapidly propels you in the direction you are moving / This ability has a 5 second cooldown / During this cooldown, you fire more rapidly with greater accuracy
- Note de version : [2.0.3.005] Reworked Speed Blaster, it now fires one paintball per shot instead of rapidly firing six shots in a burst. Paintball shots now have a shorter range and higher inaccuracy by default, and are visually adjusted to drop paint on death. Right clicking while holdin

### Spikecrag Staff (Weapons.Summon)

Version du code la plus proche : 2019-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Summons a spikecrag to protect you
- Officiel : Summons a sentry / Summons a bed of stalagmites to spray spikes at your enemies

### Spirit Glyph (Accessories)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Whenever your minions hit an enemy you will gain a random buff / These buffs will either boost your defense, summon damage, or life regen for a while
- Officiel : Minion attacks provide various buffs / Hold Shift for moreSpirit Power grants 10% increased summon damage / Spirit Defense grants 2 defense / Spirit Regen grants 0.5 HP/s life regeneration

### Staff of the Mechworm (Weapons.Summon)

Nom actuel : Void Eater Marionette. Version du code la plus proche : 2020-11 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Summons an aerial mechworm to fight for you
- Officiel : Contains the power to control a mini Devourer / Repeated summons increase the length and power of the Devourer / The Devourer attacks with Divinity Devourer, Faith Incinerator, and Ultracosmic Maelstrom / Can play fetch with dropped items / Shift-[open] this item in your inventory when the Devourer has been spawned to prioritize fetching
- Note de version : [2.1.0] Renamed Staff of the Mechworm to Void Eater Marionette.
- Note de version : [2.1.0] Reworked Void Eater Marionette, it no longer summons a mini Devourer minion which drifts on top of enemies, occasionally firing lasers and doing teleport charges. It now summons a mini Devourer minion whose damage scales based on the number of minion slots use

### Star Swallower Containment Unit (Buffs)

Nom actuel : P-PMA: Aqueous Hunter Drone. Version du code la plus proche : 2022-04 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Small novelties created to easily transport and fire plasma, strangely popular with humans / Summons a biomechanical frog that vomits plasma onto enemies
- Officiel : Summons a shrimp drone that fires bursts of homing plasma missiles / [open] while holding any summon weapon to command the shrimp to launch a vertical missile barrage / Vertical missiles aim towards your cursor and leave a lingering blast if they hit solid ground / Each shrimp uses 4 minion slots / Ignores 15 points of enemy Defense / When pursuing the adherents of gods, Yharim believed no stone should be left unturned. / Weaponry for underwater contingencies was an unusually heavy focus, but no models ever saw deployment

### Star of Destruction (Weapons.Rogue)

Version du code la plus proche : 2020-01 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires a huge destructive mine that explodes into destruction bolts / Amount of bolts scales with enemies hit, up to 16 / Stealth strikes always explode into the max amount of bolts
- Officiel : Throws a fan of cosmic bolts that meld together into a star of destruction / The explosive star will slowly follow your cursor until it expires or hits an enemy / Upon running out of time, the star will burst into a huge explosion that deals increased damage / Stealth strikes throw additional bolts that converge into a faster star that can pierce, exploding on every hit
- Note de version : [2.1.0] Reworked Star of Destruction, it no longer throws a bomb that explodes into a scaling amount of homing bolts based on enemy hits, and stealth strikes no longer always spawn the maximum number of bolts. It now throws a large spread of mini bolts which converge 

### Starmada (Weapons.Ranged)

Version du code la plus proche : 2019-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires a barrage of stars and plasma blasts
- Officiel : [use] fires a spread of starshots / [open] fires a concussive short range starburst / Starshots can be accelerated by firing when the starburst goes off / Perfect timing reduces spread, increases damage, and adds a stacking weapon power buff / Imperfect timing still increases damage, but heavily increases spread and reduces weapon power / The weapon power buff strengthens both starshots and the concussive starburst / Uses fallen stars, 50% chance to save ammo / Et toi, et moi
- Note de version : [2.1.0] Reworked Starmada, it no longer fires a barrage of various star and plasma blast projectiles which speeds up over time. Left-click now fires a spread of piercing star shots. Right-click now fires a short-range blast on a cooldown. The left-click and right-clic

### Static Refiner (Placeables)

Version du code la plus proche : 2019-04 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Used for special crafting
- Officiel : (pas de page)

### Statis' Ninja Belt (Accessories)

Version du code la plus proche : 2021-11 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : 6% increased jump speed and allows constant jumping / Increased fall damage resistance by 35 blocks / Can climb walls, dash, and dodge attacks / The dodge has a 90 second cooldown / This cooldown is shared with all other dodges and reflects
- Officiel : Provides a high-speed dash / 10% increased movement speed, 32% increased jump speed and negates fall damage / Increases movement acceleration and deceleration by 1.6x / Allows the user to swim, climb walls, and auto jump / Grants or improves the ability to dodge attacks

### Statis' Void Sash (Accessories)

Version du code la plus proche : 2021-11 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : 12% increased jump speed and allows constant jumping / Grants immunity to fall damage / Can climb walls, dash and dodge attacks / The dodge has a 90 second cooldown / This cooldown is shared with all other dodges and reflects / Dashes leave homing scythes in your wake
- Officiel : Provides a high-speed dash / 20% increased movement speed, 32% increased jump speed, and negates fall damage / Increases movement acceleration and deceleration by 1.85x / Allows the user to swim, climb walls, and auto jump / Grants or improves the ability to dodge attacks

### Storm Spray (Weapons.Magic)

Nom actuel : Sandstream Scepter. Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires a spray of water that drips extra trails of water
- Officiel : Fires an explosive stream of sand that falls after hitting an enemy / The explosion only occurs when the projectile expires
- Note de version : [2.0.3.007] Renamed Storm Spray to Sandstream Scepter.

### Stormfront Razor (Weapons.Rogue)

Version du code la plus proche : 2021-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Throws a throwing knife that leaves sparks as it travels. / Stealth strikes cause the knife to be faster and leave a huge shower of sparks as it travels
- Officiel : Throws a knife that summons lightning on hit / Stealth strikes cause the knife to be faster and summon multiple lightning bolts on hit / Strike and stem the flow
- Note de version : [2.0.2.001] Reworked Stormfront Razor, it no longer throws a piercing dagger that spawns sparks as it travels, and stealth satrikes no longer make sparks spawn more often. It now throws non-piercing daggers that summon lightning bolts from the sky on impact with an enemy 

### Stratus Sphere (Weapons.Typeless)

Version du code la plus proche : 2020-01 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires an energy orb containing the essence of our stratosphere / Up to six of these can be active at a time
- Officiel : Launches a singularity which exerts influence on all entities in a large area around it / Influenced players gain Starbursts over time and will consume Starbursts to fly when out of flight time / [open] to cause Starbursts to form a barrier, causing you to lose Starbursts instead of health / Launching the singularity deactivates the barrier, and creating the barrier deactivates the singularity / This weapon scales with the average of all your damage stats / Channeling souls into stars, an inane concept

### Sulphuric Acid Cannon (Weapons.Ranged)

Version du code la plus proche : 2022-03 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires an acidic shot that sticks to enemies and dissolves them
- Officiel : Fires homing Acid Rockets / Acid Rockets stick to enemies and explode into toxic mist after a short delay
- Note de version : [2.1.0] Reworked Sulphuric Acid Cannon, it no longer fires homing acid blasts which stick to enemies and constantly release acid mist. It now uses rockets as ammo, and slowly fires acid rockets which home in on enemies and stick to them. After sticking in enemies for 

### Sunbeam Fish (Fishing)

Nom actuel : Fishing. Version du code la plus proche : 2020-01 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Right click to extract essence
- Officiel : (pas de page)

### Supernova (Weapons.Rogue)

Version du code la plus proche : 2020-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Creates a massive explosion on impact / Explodes into spikes and homing energy / Stealth strikes release energy as they fly
- Officiel : Disables normal stealth generation / Hold [use] to generate stealth, or hold [open] to toss the bomb without generating stealth / Tosses a smart bomb that locks onto and tracks targets / If no target is found it will return to you / Each bomb creates a miniature Miracle Blight supernova that drags in weak enemies and chases strong enemies / Stealth strikes create a blast that shakes the foundations of reality itself
- Note de version : [2.0.4.001] Reworked Supernova, it no longer tosses a gravity-affected bomb that creates an explosion and shrapnel on hit, and stealth strikes no longer spawn energies as they travel. It now tosses out a bomb which automatically homes towards nearby enemies. On hit, bombs

### Svantechnical (Weapons.Ranged)

Version du code la plus proche : 2019-11 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires several barrages of bullets / Right click to zoom out
- Officiel : Fires a trinity of energy bolts that split into beams that track your cursor / Ignores 200 points of enemy Defense / 80% chance to save ammo / The age old balance of trading damage for frames
- Note de version : [2.0.4.003] Reworked Svantechnical, it no longer rapidly fires bullets from the gun alongside an inaccurate spread of bullets from the cursor, and no longer has a scope effect on right click. It now fires 3 bullets at once in a sine-wave pattern; left click converts bulle

### Tequila (Potions)

Version du code la plus proche : 2019-04 (ressemblance 0.88, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Boosts damage, damage reduction, and knockback by 3%, crit chance by 2%, and defense by 5 during daytime / Reduces life regen by 1
- Officiel : 50% increased Electric debuff damage / 50% decreased Cold debuff damage / Drinking more than 3 different alcohols might not end well with your liver

### Tequila Sunrise (Potions)

Version du code la plus proche : 2021-01 (ressemblance 0.86, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Boosts damage, damage reduction, and knockback by 7%, crit chance by 3%, and defense by 10 during daytime / Reduces life regen by 1
- Officiel : Greatly increases natural life regeneration after suffering a damaging debuff / 50% increased damage from damaging debuffs / Drinking more than 3 different alcohols might not end well with your liver

### Terra Arrow (Ammo)

Nom actuel : Sprouting Arrow. Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Travels incredibly quickly and explodes into more arrows when it hits a certain velocity
- Officiel : Fires an energy arrow that quickly splits into two arrows / These arrows ignore 8 points of defense
- Note de version : [2.0.4.001] Renamed Terra Arrow to Sprouting Arrow.
- Note de version : [2.0.4.001] Reworked Sprouting Arrow, it no longer splits into 2 arrows on hit that home. It now fires an arrow that splits into 2 arrows shortly after firing that continue in the same direction. These split arrows each deal 54% base damage. Main arrow now has 8 extra upd

### Terra Bullet (Ammo)

Nom actuel : Dryad's Tear. Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Explodes and splits into homing terra shards on death
- Officiel : An extremely powerful magic bullet blessed with nature's essence that becomes unstable when multiple are nearby / Carries a teardrop along with it which on impact will detach and crash down upon the hit enemy and burst / All bullet and teardrop hits have 18 armor penetration / Focus. Breathe. Just one more shot, and the violence will fade away...
- Note de version : [2.0.4.001] Renamed Terra Bullet to Dryad's Tear.
- Note de version : [2.2.0] Reworked Dryad's Tear, it no longer splits into homing shards on hit. It now has an innate 18 armor penetration, however having multiple of the bullet active at once reduces the damage of bullets and causes their trajectory to swerve off to the side. Each bull

### Terra Flameburster (Weapons.Ranged)

Nom actuel : Wildfire Bloom. Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : 80% chance to not consume gel
- Officiel : Charges up to emit a stream of energized flames and pairs of homing wildfire flares / Uses gel for ammo / 50% chance to save ammo
- Note de version : [2.0.4.001] Renamed Terra Flameburster to Wildfire Bloom.

### The First Shadowflame (Accessories)

Version du code la plus proche : 2021-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : It is said that in the past, Prometheus descended from the heavens to grant man fire. / If that were true, then it is surely the demons of hell that would have risen from below to do the same. / Increases max minions by 1 and minions inflict the Shadowflame debuff on enemies.
- Officiel : Increases your max number of minions by 1 / Minion attacks inflict Shadowflame, and grants immunity to Shadowflame

### The Mutilator (Weapons.Melee)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Striking an enemy below 20% life will trigger a bloodsplosion / Bloodsplosions cause hearts to drop that can be picked up to heal you
- Officiel : Striking enemies empowers The Mutilator, increasing its damage / At full charge, striking enemies causes a blood orb explosion / Picking up these blood orbs reduces the duration of Potion Sickness
- Note de version : [2.1.0] Reworked The Mutilator, it no longer functions like a standard broadsword which creates bloodsplosions that drop Hearts when hitting an enemy with below 20% health. It now has a custom swing animation. Hitting enemies inflicts Laceration for 1 second and adds 

### The Obliterator (Weapons.Melee)

Version du code la plus proche : 2020-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Ruins nearby enemies with death lasers / An exceptionally agile yoyo
- Officiel : Reach: 45 tiles / Speed: 54 / Infinite throw duration / Occasionally performs a miniature Dimensional Drive at nearby foes / Counterweight hits instantly begin another Dimensional Drive

### The Transformer (Accessories)

Version du code la plus proche : 2022-03 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Taking damage releases a blast of sparks / Immunity to Electrified and you resist all electrical projectile and enemy damage / Enemy bullets do half damage to you and are reflected back at the enemy for 800% their original damage
- Officiel : Generates an aura around you that creates morphing globs when projectiles pass through it or when enemies are killed / The globs encircle you and deal low damage and knockback to enemies they collide with / The circle can contain up to 30 globs at once / Press [Transformer Hot Key] to transform all globs into needles and launch them at your cursor / How did this come out of the junk you cobbled together? You're lucky it doesn't transform you
- Note de version : [2.1.0] Reworked The Transformer, it no longer increases electricity debuff damage, grants immunity to Electrified, releases a burst of sparks when hit, reduces damage from electric attacks and bullets, or reflects bullets. It now generates an aura around the player w

### Thief's Dime (Pets)

Version du code la plus proche : 2019-11 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Those scurvy dogs don’t know the first thing about making bank / Summons a coin that revolves around you and steals money from enemies
- Officiel : Summons Goldie the rogue coin to collect nearby coins and light your way / Provides a small amount of light in the abyss / Once worthless treasure, now a relic of a thief's companionship
- Note de version : [2.0.2.001] Reworked Thief's Dime. It is now a Light pet item. The rotating coin now collects coins for the player, and provides a small amount of light in the Abyss.

### Thunderstorm (Weapons.Magic)

Nom actuel : Volterion. Version du code la plus proche : 2019-06 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Make it rain
- Officiel : Discharges a high-voltage stream which bursts into 5 balls of lightning on hit / Each ball lightning emits two additional lightning bolts before exploding
- Note de version : [2.1.0] Renamed Thunderstorm to Volterion.
- Note de version : [2.1.0] Reworked Volterion, it no longer fires plasma bolts that rain additional bolts from the sky on hit. It now fires a high-voltage bolt which bursts into 5 electric orbs. Electric orbs fire lightning bolts at enemies briefly before exploding. Buffed damage from 1

### Titanium Railgun (Weapons.Ranged)

Version du code la plus proche : 2019-08 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires a laser
- Officiel : Hold down to charge up a decimating titanium blast / Longer hold time increases power, leading to more damage, size, and knockback / Power is capped when the cannon's sights have fully converged
- Note de version : [2.0.0.001] Reworked Titanium Railgun, it no longer instantly fires a beam on use. Holding down left click now charges up the weapon to fire an extremely fast beam when released. The damage of the beam scales with how long it has been charged up for, stopping when the sig

### Toxic Heart (Accessories)

Version du code la plus proche : 2021-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Summons a plague aura around you that damages nearby enemies / Makes you immune to the Plague debuff
- Officiel : Your heart beats with the Plague, pulsing outside of your body / Pulse frequency scale with regen, and pulses that kill enemies pulse again to heal you / 50% increased Sickness debuff damage / Effects are transparent and silent if visibility is disabled
- Note de version : [2.0.4.003] Reworked Toxic Heart, it no longer grants immunity to Plague or spawns a damaging aura around the player that inflicts Plague. It now causes plague pulses to be constantly spawned that damage enemies and inflict Plague. The frequency of pulses scales based on 

### Toxicant Twister (Weapons.Rogue)

Version du code la plus proche : 2020-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Throws a slow moving boomerang / After a few moments, the boomerang chooses a target and rapidly homes in / Stealth strikes home in faster and rapidly release sand
- Officiel : Throws a fan of four spinning disks that swirl around your cursor / Stealth strikes swirl in a wider range and create a vortex of raindrops that home in on enemies
- Note de version : [2.1.0] Reworked Toxicant Twister, it no longer throws a boomerang that homes in after a delay, and stealth strikes no longer home in faster and spawn trailing sand. It now throws 4 boomerangs that spin and cluster around the cursor. Stealth strikes now spin faster an

### Trinket of Chi (Accessories)

Version du code la plus proche : 2020-12 (ressemblance 0.98, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : After 1 second of standing still and not attacking you gain a buff / The buff boosts your damage by 50% and decreases damage taken by 15% / The buff deactivates if you move or attack once
- Officiel : Increases damage reduction by 20% if you avoid taking damage for 10 seconds / This buff expires on hit / +0.5 HP/s life regen for you and your teammates
- Note de version : [2.0.2.001] Reworked Trinket of Chi, it no longer makes standing still for 1 second grant a buff that increases damage by 50% and damage reduction by 15%. It now grants a buff that decreases damage taken by 10% after not being hit for 15 seconds, and grants 2 life regener

### True Forbidden Oathblade (Weapons.Melee)

Nom actuel : Exalted Oathblade. Version du code la plus proche : 2019-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires a spread of demonic scythes and critical hits cause shadowflame explosions
- Officiel : Throw a twisted blade that impales, immolates, and slows enemies / Up to four blades can impale an enemy at a time, with further hits launching the previous impaled blades / Press [open] to enter a stance where you can use [use] to perform a devastating slash / While in the stance, you gain life and flight time regeneration and your dashes have a shorter cooldown / The devastating slash must recharge after being used / Striking an impaled enemy with the devastating slash launches all blades and readies another slash / Hold Shift for moreA sword fashioned from the legend of the Last Demon Lord. / Crowned through his overwhelming might, his sword bathed in the blood of Demons and the fires of hell. / Even long after his passing, none have dared to take the seat. / Demonkind has not known unity since.
- Note de version : [2.0.2.001] Renamed True Forbidden Oathblade to Exalted Oathblade.
- Note de version : [2.1.0] Reworked Exalted Oathblade, it no longer fires a spread of accelerating scythes, or spawns shadowflame explosions when critically striking with a true melee hit. It now throws the blade when used, which impales into enemies and slows them. Up to 4 blades can b

### Tumbleweed (Weapons.Melee)

Version du code la plus proche : 2019-10 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Releases a rolling tumbleweed on hit
- Officiel : Throws a tumbleweed flail / Spinning tumbleweeds brew a localized dust storm / Launched tumbleweeds detach from their chain upon striking an enemy and roll forwards
- Note de version : [2.1.0] Reworked Tumbleweed, it no longer throws a flail which detaches and spawns a rolling tumbleweed on enemy hits. It now uses the same functionality as vanilla thrown flails; holding left-click spins the flail around the player, releasing left-click launches the 

### Unstable Granite Core (Accessories)

Version du code la plus proche : 2019-06 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Three sparks are released on critical hits
- Officiel : Gain an energy field that periodically zaps enemies with arcing energy / The arcing energy ignores a substantial amount of enemy defense
- Note de version : [2.0.2.001] Reworked Unstable Granite Core, it no longer makes critical strikes spawn sparks. It now automatically attacks nearby enemies with arcing sparks that can target up to 3 enemies, and bounce twice. The effect is active for 5 seconds every 10 seconds. The arcing 

### Ursa Sergeant (Fishing)

Version du code la plus proche : 2022-02 (ressemblance 0.96, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : 15% reduced movement speed / Immune to Astral Infection and Feral Bite / Increased regeneration at lower health
- Officiel : Attacks maul enemies with astral claw swipes that inflict Astral Infection (debuff) / Taking damage reduces the cooldown, but inflicts you with Astral Infection (debuff) / Enemy kills also reduce the cooldown / Swipes will be translucent and silent if visibility is disabled
- Note de version : [2.1.0] Reworked Ursa Sergeant, it no longer grants 20 defense, reduces movement speed, increases life regeneration as health lowers, or grants immunity to Astral Infection and Feral Bite. It now makes an astral claw swipe spawn when attacking enemies on a 5 second co

### Vanquisher Arrow (Ammo)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Pierces through tiles / Spawns extra homing arrows as it travels
- Officiel : Pierces through tiles and homes in on enemies / Tears a rift in space on enemy hits
- Note de version : [2.0.4.001] Reworked Vanquisher Arrow, it no longer spawns split arrows as it travels that home in on enemies. Arrows now pierce once, dealing 85% base damage, and gain homing when they pass close enough to an enemy. It additionally spawns slashes on death that deal 100% 

### Viscera (Weapons.Magic)

Version du code la plus proche : 2019-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires a blood beam that heals you on enemy hits / The more tiles and enemies the beam bounces off of or travels through the more healing the beam does
- Officiel : Fires a rapid barrage of piercing blood beams / The first enemy struck by each beam will burst into blood orbs / Picking up these blood orbs reduces the duration of Potion Sickness / Each successive beam in the barrage deals more damage / The last beam of the barrage instead explodes upon hitting an enemy
- Note de version : [2.0.4.001] Reworked Viscera, it no longer fires piercing blood beams that heal more health the more tiles and enemies they hit. It now fires a burst of 4 blood beams with a pause in between. Beams pierce 6 times, grant -1 local immunity frames, and bounce off tiles. Each

### Vitriolic Viper (Weapons.Magic)

Version du code la plus proche : 2020-04 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Releases a volley of venomous fangs and spit
- Officiel : Charges up and fires a slithering beam of radiation / If fully charged, instead fires a huge spread of short range fangs
- Note de version : [2.1.0] Reworked Vitriolic Viper, it no longer fires a spread of fangs and spit. It is now charged up to fire a spread of spit that inflicts Irradiated, whose spread gets tighter the longer it is charged. The weapon takes 1 second to fully charge, and when fully charg

### Void Concentration Staff (Weapons.Summon)

Version du code la plus proche : 2021-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Summons a foreboding aura that attacks by firing void orbs / Minion damage is increased by 5% while the aura is active / Requires three minion slots to use / Only one may exist / Right click to launch a black hole that grows in size
- Officiel : Surrounds you with dark energy, which makes all attacks deal 15% less damage / [open] to condense the energy onto a target, focusing your minions on them and creating homing beams / When the focused enemy takes damage from any source, they will be struck by additional dark energies / These dark energy are twice as strong when the strike was from a minion or sentry / After 10 seconds, the dark energy will resume orbiting you
- Note de version : [2.2.0] Reworked Void Concentration Staff, it no longer summons an aura which fires orbs or grants 5% summon damage, and no longer fires black holes on right-click. It now summons an aura which creates Dark Energies that orbit around the player; the aura requires 4 mi

### Void of Extinction (Accessories)

Nom actuel : Apollyon. Version du code la plus proche : 2019-04 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : No longer cursed / Drops brimstone fireballs from the sky occasionally / 15% increase to all damage / Brimstone fire rains down while invincibility is active / Temporary immunity to lava, greatly reduces lava burn damage, and 25% increased damage while in lava
- Officiel : Critical strikes curse enemies with Bane that greatly scales off of your critical strike chance / When critically hit, enemies spread all debuffs they are afflicted with that are weaker than your Bane to nearby enemies / Critical strikes deal an additional 3% damage per debuff on the enemy / Current Bane damage: [DAMAGE] DPS
- Note de version : [2.0.3.005] Reworked Void of Extinction, it no longer increases damage by 12%, grants lava immunity, immunity to several debuffs, makes projectiles rain from the sky occasionally and when hit, or makes attacks inflict Brimstone Flames. It now greatly reduces the damage ov
- Note de version : [2.2.0] Renamed Void of Extinction to Apollyon.
- Note de version : [2.2.0] Reworked Apollyon, it no longer reduces the damage of Brimstone Flames, grants 13% critical strike chance, or makes critical strikes create an explosion that inflicts Brimstone Flames. It now makes critical strikes inflict Bane. Bane deals a base of 20 damage 

### Whiskey (Potions)

Version du code la plus proche : 2021-01 (ressemblance 0.82, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Boosts damage and knockback by 4% and critical strike chance by 2% / Reduces defense by 5%
- Officiel : Provides between -15% and 15% damage to the held weapon's class / Each class tracks this boost individually / This boost lowers the longer you hold weapons of the same class, taking 10 seconds to fully decay / This boost recharges when holding weapons of a different class, taking 5 seconds to fully recharge / Drinking more than 3 different alcohols might not end well with your liver

### Wingman (Weapons.Magic)

Version du code la plus proche : 2019-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires a concentrated laser beam
- Officiel : Spawns two Wingmen which orbit you and fire lasers at your cursor / Press [open] while firing to order the Wingmen to launch grenades / Grenades have a cooldown which is shortened if the Wingmen are fully revved up
- Note de version : [2.0.4.001] Reworked Wingman, it no longer fires 3 laser beams on use. It now summons two Wingmen that orbit near the player, charging up to rapidly fire lasers towards the cursor. Right clicking while using the weapon will cause the Wingmen to instead fire lingering gren

### Yharim's Insignia (Accessories)

Nom actuel : Ascendant Insignia. Version du code la plus proche : 2021-01 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : 10% increased damage when under 50% life / 10% increased melee speed / 10% increased melee and true melee damage / Melee attacks and melee projectiles inflict holy fire / Temporary immunity to lava / Increased melee knockback
- Officiel : Grants 4 additional seconds of flight time / Increases movement speed by 15%, jump speed by 20%, and acceleration by 1.5x / Pressing [Accessory Ability] fully restores flight time and prevents it from decreasing for 4 seconds / This ability has a 40 second cooldown
- Note de version : [2.0.3.007] Renamed Yharim's Insignia to Ascendant Insignia.
- Note de version : [2.0.3.007] Reworked Ascendant Insignia, it no longer increases melee damage and true melee damage by 15%, increases damage by 10% when below 50% health, increases melee knockback, or makes melee attacks inflict Holy Flames. It now increases flight time by 30%, movement s

### Asgardian Aegis (Accessories)

Version du code la plus proche : 2022-02 (ressemblance 0.86, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.12

- Fandom : Grants immunity to knockback / Immune to most debuffs / +40 max life and increased life regeneration / Grants a supreme holy flame dash / Can be used to ram enemies / Press [Elysian Gar] to activate buffs to all damage, crit chance, and defense / Activating this buff will reduce your movement speed and increase enemy aggro / +20 defense while submerged in liquid
- Officiel : Grants immunity to knockback / Allows the ability to ram dash, empowered by a cosmic inferno / This dash can ram through enemies without taking damage

### Blazing Core (Accessories)

Nom actuel : Divine Providence. Version du code la plus proche : 2020-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.17

- Fandom : The searing core of the profaned goddess / 10% damage reduction / Being hit creates a miniature sun that lingers, dealing damage to nearby enemies / The sun will slowly drag enemies into it / Only one sun can be active at once / Provides a moderate amount of light in the Abyss
- Officiel : Grants immunity to knockback / The searing core of the Profaned Goddess / Pressing [Accessory Ability] will activate a parry, encasing you in luminous profaned crystal / Being hit shortly after encasement will shatter the crystal / Upon shattering, holy stars spiral outwards and damage taken is negated / After shattering, this damage reduction effect goes on cooldown / Damage parried whilst on cooldown summons additional healing stars, and will not be reduced
- Note de version : [2.0.3.002] Reworked Blazing Core, it no longer spawns a miniature sun when hit that drags in nearby enemies and deals damage. Pressing the {{key|Blazing Core Parry}} key will now briefly encase the player in a crystal. Being hit while encased will shatter the crystal, re
- Note de version : [2.2.2] Renamed Blazing Core to Divine Providence.

### Seraph Tracers (Accessories.Wings)

Version du code la plus proche : 2022-05 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.17

- Fandom : Ludicrous speed! / 24% increased running acceleration / Greater mobility on ice / Water and lava walking / Immunity to the On Fire! debuff / Temporary immunity to lava
- Officiel : Allows flight and slow fall / Max horizontal speed: Excellent / Max vertical speed: Excellent / Horizontal acceleration: Excellent / Vertical acceleration: Great / 50% increased running acceleration / Provides the effects of Angel Treads / Provides immunity to lava and damaging tiles, including Auric Ore / Taking speed EVEN FURTHER BEYOND!
- Note de version : [2.0.3.001] Renamed Celestial Tracers to Seraph Tracers.

### Asgard's Valor (Accessories)

Version du code la plus proche : 2022-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.2

- Fandom : Grants immunity to knockback / Immune to most debuffs and reduces the damage caused by the Brimstone Flames debuff / +16 defense while submerged in liquid / +20 max life / Grants a holy dash which can be used to ram enemies
- Officiel : Grants immunity to knockback / Allows the ability to ram dash / This dash can ram through enemies without taking damage

### Bloom Stone (Accessories)

Version du code la plus proche : 2021-10 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.2

- Fandom : One of the ancient relics / You quickly regenerate life while on the ground / This effect works best during daytime / Flowers grow if you are standing on grass / Random dye plants will grow while standing on grassless dirt
- Officiel : Healing Potions are 50% more effective, but provide their healing over time / Grappling hooks retract and pull the player 30 mph faster, and grow a flower at hookpoints / Touching a flower makes it burst into pollen which gives a temporary boost to potion healing over time / Potion healing slows drastically when at full health / One of the ancient relics
- Note de version : [2.1.0] Reworked Bloom Stone, it no longer grants scaling life regeneration and natural life regeneration boosts while grounded based on time of day, or grows flowers and dye plants. It now increases healing potion health restoration by 50%, but makes the heal be appl

### Evil Smasher (Weapons.Melee)

Version du code la plus proche : 2021-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.2

- Fandom : EViL! sMaSH eVIL! SmAsh...ER! / For every enemy you kill this hammer gains stat bonuses / These bonuses stack until a cap is reached / The bonus stacks will reset if you select a different item / The bonus stacks will be reduced by 1 every time you get hit
- Officiel : Channels a powerful hammer strike with the capability to crush all evil / Striking a tile with the hammer strike will cause an explosion and push you back / Striking enemies inflicts Smashed Evil on both the user and target / Charging strikes increases damage, explosion radius, and Smashed Evil duration / EViL! sMaSH eVIl! SmAsh... ER!
- Note de version : [2.2.0] Reworked Evil Smasher, it no longer functions like a standard broadsword which receives stacking damage, attack speed, and knockback boosts when hitting enemies, losing them when getting hit. It now deals typeless damage instead of melee damage. Holding left-c

### Greatsword of Blah (Weapons.Melee)

Nom actuel : Orderbringer. Version du code la plus proche : 2020-03 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.2

- Fandom : A pale white sword from a forgotten land / You can hear faint yet comforting whispers emanating from the blade / No matter where you may be you are never alone / I shall always be at your side, my lord / Fires a rainbow blade that emits rainbow rain on death for a time
- Officiel : Fires piercing energy waves and summons four homing stars near your cursor / Hits with the blade summon two beams from the sky / No matter where you may be you are never alone
- Note de version : [2.0.3.001] Renamed Greatsword of Blah to Orderbringer.
- Note de version : [2.1.0] Reworked Orderbringer, it no longer fires a beam that spawns side projectiles as it travels, or a lingering aura on hit which continuously rains rainbow rain. It now fires a large piercing wave alongside 4 homing stars. Waves slow down as they travel. Hitting 

### Profaned Soul Artifact (Accessories)

Version du code la plus proche : 2020-04 (ressemblance 0.96, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.2

- Fandom : Purity / Summons a healer guardian which heals for a certain amount of health every few seconds / Summons a defensive guardian if you have at least 10 minion slots, which boosts your movement speed and your damage resistance / Summons an offensive guardian if you are wearing the Uelibloom summoner set (or stronger), which boosts your summon damage and your minion slots / If you get hit, most of their effects will disappear for 5 seconds
- Officiel : Summons your own personal Guardians to fight alongside you / The offensive guardian grants an additional minion slot and fires a ring of spears while attacking / The defensive guardian creates a weak barrier of profaned rock / Halved Adrenaline loss if all damage is absorbed / Once depleted, the rocks are flung outward and will regenerate 5 seconds after being hit / Taking damage resets the timer / The healer guardian restores 10 health every few seconds / Purity
- Note de version : [2.0.3.003] Reworked Profaned Soul Artifact and Profaned Soul Crystal. Profaned Soul Artifact no longer provides any stat boosts other than the 1 minion slot. Offensive Guardian now occasionally fires spear bursts while attacking. Defensive Guardian no longer attacks targ

### The Evolution (Accessories)

Version du code la plus proche : 2021-11 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.2

- Fandom : You reflect projectiles when they hit you / Reflected projectiles deal no damage to you / This reflect has a 120 second cooldown which is shared with all other dodges and reflects / If this effect triggers you get a health regeneration boost for 5 seconds / If the same enemy projectile type hits you again you will resist its damage 15%
- Officiel : Allows you to dodge projectiles, reflecting highly damaging mirror shards back at enemies / Black Belt Attacks that deal less than 5% of your max life in damage will not be dodged / Black Belt This dodge has a maximum cooldown of 90 seconds, shortened depending on the dodged attack's damage / Black Belt Equipping multiple accessories that give this ability to dodge will reduce the cooldown by 15% each / Getting hit by the same enemy projectile type during cooldown will resist its damage by 25%

### Alluring Bait (Accessories)

Version du code la plus proche : 2019-12 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.25

- Fandom : 30 increased fishing power during the day / 45 increased fishing power during the night / 60 increased fishing power during a solar eclipse / Greatly increases chance of catching potion ingredient fish
- Officiel : Increases fishing power by 10 / Increases the yield of potion ingredient fish

### Astreal Defeat (Weapons.Ranged)

Version du code la plus proche : 2021-04 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.25

- Fandom : Ethereal bow of the tyrant king's mother / The mother strongly discouraged acts of violence throughout her life / Though she kept this bow close, to protect her family in times of great disaster / All arrows are converted to Astreal Arrows that emit flames as they travel
- Officiel : All arrows are converted to Astreal Arrows that emit flames as they travel

### Daawnlight Spirit Origin (Accessories)

Version du code la plus proche : 2021-12 (ressemblance 0.93, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.25

- Fandom : All nearby enemies and bosses are Marked for Death with bullseyes / Ranged attacks that strike a bullseye always critically strike and deal massive damage / When a bullseye is struck, it vanishes and a new one appears elswhere / Explosions or large projectiles cannot strike bullseyes / Converts all ranged critical strike chance boosts into extra critical strike damage / All ranged weapons will deal even more critical strike damage the slower they are / Summons a heroic spirit from another world if accessory visibility is enabled / The heroic spirit is also summoned when this accessory is placed in vanity slots / A strand of a lost cosmos remains, waiting for its master
- Officiel : All ranged weapons can supercrit when critical strike chance is over 100% / Marks enemies and bosses with bullseyes which boost critical strike chance when hit / Extra critical strike chance quickly fades over time / Summons a heroic spirit from another world if visibility is on or in vanity slots / A strand of a lost cosmos remains, waiting for its master
- Note de version : [2.1.0] Reworked Daawnlight Spirit Origin, it no longer converts ranged critical strike chance into extra critical strike damage, or places bullseyes on enemies which guarantee a critical strike when hit. It now places bullseyes on enemies which give stacking ranged c

### Laudanum (Accessories)

Version du code la plus proche : 2020-11 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.25

- Fandom : Converts certain debuffs into buffs and extends their durations / Debuffs affected: Darkness, Blackout, Confused, Slow, Weak, Broken Armor, / Armor Crunch, War Cleave, Chilled, Ichor and Obstructed / Revengeance drop
- Officiel : Converts certain debuffs into buffs and extends their durations / This also removes immunities to these debuffs if you have them

### Sirius (Weapons.Summon)

Version du code la plus proche : 2020-04 (ressemblance 0.93, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.25

- Fandom : Summons the brightest star in the night sky to shine upon your foes / Consumes all of the remaining minion slots on use / Must be used from the hotbar / Increased power based on the number of slots used
- Officiel : Summons the brightest star in the sky, Sirius, to protect you / Continue summoning to complete Canis Major / Repeated summons increase its power greatly, even after the constellation is complete / Sirius passively generates Starbursts when targeting enemies / When Starbursts are full, Sirius consumes them to launch a powerful piercing quasar that inflicts Voidfrost / A guide to a dream never realized
- Note de version : [2.1.0] Reworked Sirius, it no longer summons a star minion which uses 6 minion slots and hovers over the player, firing hitscan beams. It now summons a star which hovers over the player and fires homing fireballs at enemies. Repeated summons with the weapon will inst

### Soul of Cryogen (Accessories.Wings)

Version du code la plus proche : 2020-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.25

- Fandom : The magic of the ancient ice castle is yours / 7% increase to all damage / All melee attacks and projectiles inflict frostburn / Icicles rain down as you fly
- Officiel : Allows flight and slow fall / Max horizontal speed: Good / Max vertical speed: Average / Horizontal acceleration: Average / Vertical acceleration: Average / Icicles rain down as you fly

### Celestial Tracers (Accessories.Wings)

Nom actuel : Seraph Tracers. Version du code la plus proche : 2022-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.29

- Fandom : Talking speed EVEN FURTHER BEYOND! / 50% increased running acceleration / Greater mobility on ice / Water and lava walking / Immunity to the On Fire! debuff / Temporary immunity to lava / Being hit for over 200 damage will make you immune for an extended period of time
- Officiel : Allows flight and slow fall / Max horizontal speed: Excellent / Max vertical speed: Excellent / Horizontal acceleration: Excellent / Vertical acceleration: Great / 50% increased running acceleration / Provides the effects of Angel Treads / Provides immunity to lava and damaging tiles, including Auric Ore / Taking speed EVEN FURTHER BEYOND!
- Note de version : [2.0.3.001] Renamed Seraph Tracers to Celestial Tracers.
- Note de version : [2.1.0] Renamed Celestial Tracers to Moon Walkers. <!-- Is named Moon Boots when wings are disabled -->
- Note de version : [2.0.3.001] Renamed Celestial Tracers to Seraph Tracers.

### Dark Sun Ring (Accessories)

Version du code la plus proche : 2020-12 (ressemblance 0.99, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.29

- Fandom : Contains the power of the dark sun / 12% increase to damage and melee speed / +1 life regeneration, 15% increased pick speed, and +2 max minions / Increased minion knockback / During the day the player has +3 life regen / During the night the player has +15 defense / Both of these bonuses are granted during an eclipse
- Officiel : Grants immunity to knockback / 12% increased damage / +0.5 HP/s life regen and +2 max minions / Increased minion knockback / Grants various buffs as the day goes on / Contains the power of the dark sun

### Rampart of Deities (Accessories)

Version du code la plus proche : 2022-03 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.29

- Fandom : Causes stars to fall and gives increased immune time when damaged / Provides life regeneration and reduces the cooldown of healing potions / Absorbs 25% of damage done to players on your team / This effect is only active above 25% life / Grants immunity to knockback / Puts a shell around the owner when below 50% life that reduces damage / The shell becomes more powerful when below 15% life and reduces damage even further
- Officiel : Grants immunity to knockback / Releases a blast of homing star-bees, increases movement speed, and drenches you in Honey (buff) after taking damage / Increases the length of invincibility, with it increasing even more when at low health or taking high damage attacks / Reduces damage taken by 25% when below 50% life / Absorbs 25% of damage done to players on your team when you are above 25% life

### The Community (Accessories)

Version du code la plus proche : 2021-04 (ressemblance 0.92, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.29

- Fandom : The heart of (most of) the Terraria community / Starts off with weak buffs to all of your stats / The stat buffs become more powerful as you progress / Reduces the DoT effects of harmful debuffs inflicted on you / Thank you to all of my supporters that made this mod a reality
- Officiel : The heart of (most of) the Terraria community / Thank you to all of our supporters who made this mod a reality!

### Bloodfin (Fishing)

Version du code la plus proche : 2020-04 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.33

- Fandom : The wonders of angiogenesis / Grants a buff that boosts life regen for 10 seconds / The life regen boost is stronger if below 75% health
- Officiel : Grants a buff that restores 1 HP every 0.25 seconds / 10 second duration / The wonders of angiogenesis

### Corrosive Spine (Accessories)

Version du code la plus proche : 2020-05 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.33

- Fandom : 10% increased movement speed / All rogue weapons inflict venom and spawn clouds on enemy hits / You release a ton of clouds everywhere on hit
- Officiel : 5% increased movement speed / 50% increased Sickness and Water debuff damage / All attacks inflict Irradiated and reduces the severity of the sulphuric waters / You burst into radioactive clouds when struck

### Draedon's Heart (Accessories)

Version du code la plus proche : 2021-10 (ressemblance 0.82, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.33

- Fandom : Standing still regenerates your life quickly, reduces your damage by 50% and boosts your defense by 75% / Nanomachines, son
- Officiel : Grants immunity to knockback / 15% reduced contact damage from enemies / Replaces Adrenaline with the Nanomachines meter / Unlike Adrenaline, you lose no Nanomachines when you take damage, but they stop accumulating for 1 second / With full Nanomachines, press [Adrenaline Mode] to heal 180 health over 2 seconds / While healing you take [DR]% less damage / Nanomachines, son
- Note de version : [2.0.0.001] Reworked Draedon's Heart, no longer increases the player's life regeneration and damage reduction while standing still. It now adds the Nanomachines meter, which will gradually charge over time during boss encounters. When the meter is full, it can be activate

### Dynamic Pursuer (Weapons.Rogue)

Version du code la plus proche : 2022-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.33

- Fandom : A weapon that, as it flies, processes calculations and fires electricity / Releases a flying disk that fires electricity at nearby enemies / Stealth strikes allow the disk to ricochet multiple times and unleash an electric explosion, then fire inaccurate lasers while returning
- Officiel : Throws a calculation-processing disk that fires electricity at enemies / Stealth strikes allows the disk to ricochet to other targets, fire lasers, and unleash an explosion / If the disk cannot find a new target to ricochet to, it will pursue the original target instead

### Eclipse's Fall (Weapons.Rogue)

Version du code la plus proche : 2019-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.33

- Fandom : When the sun goes dark, you will know judgement / Summons spears from the sky on hit / Stealth strikes impale enemies and summon a constant barrage of spears over time
- Officiel : Throws a javelin that shatters into fragments of primordial light upon enemy hits / If more than 20 fragments exist, the oldest fragments will seek out enemies to attack / Stealth strikes coalesce fragments into Spear of Obscuration, gaining power based on the number of fragments / Spear of Obscuration will pierce though any enemy hit by Eclipse's Fall / When the sun goes dark, all will know terror
- Note de version : [2.2.0] Reworked Eclipse's Fall, it no longer throws a javelin that rains down mini spears on hit, and stealth strikes no longer impale enemies and constantly rain mini spears. It now throws a javelin that shatters into light fragments on hit. Light fragments linger i

### Eye of Desolation (SummonItems)

Version du code la plus proche : 2022-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.33

- Fandom : Tonight is going to be a horrific night... / Summons Calamitas when used during nighttime / Not consumable
- Officiel : Summons the Calamitas Clone / Creates a square arena in Revengeance Mode, with you at its bottom / Not consumable / She is here.

### God Slayer Slug (Ammo)

Version du code la plus proche : 2021-10 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.33

- Fandom : Heavy ammunition with unlimited piercing that tears through spacetime / After a slug lands a hit, if it strikes a wall or runs out of targets to pierce, / it warps backwards through space and supercharges, exploding on impact
- Officiel : Heavy ammunition with unlimited piercing that tears through spacetime / Slugs that strike a target will warp back and supercharge, shattering on hit

### Hadarian Wings (Accessories.Wings)

Version du code la plus proche : 2021-09 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.33

- Fandom : Powered by the Astral Infection / Hold down to hover during flight, greatly extending flight duration / 10% increased movement and jump speed while wearing the Astral Armor
- Officiel : Allows flight and slow fall / Max horizontal speed: Great / Max vertical speed: Great / Horizontal acceleration: Great / Vertical acceleration: Great / Press [Down] to toggle hover / Press [Up] to deactivate hover / Powered by the Astral Infection

### Jungle Dragon Egg (SummonItems)

Nom actuel : Blessed Phoenix Egg. Version du code la plus proche : 2022-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.33

- Fandom : An imitation of the egg that bore the loyal guardian of the tyrant king / Summons Jungle Dragon, Yharon when used in the jungle / Not consumable
- Officiel : Summons Yharon, Dragon of Rebirth / Enrages outside the fire walls / Not consumable / A symbol of the Dragon watching over and waiting for you.
- Note de version : [2.0.2.001] Renamed Jungle Dragon Egg to Blessed Phoenix Egg.

### Necklace of Vexation (Accessories)

Version du code la plus proche : 2020-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.33

- Fandom : Revenge / 20% increased damage when under 50% life / All attacks inflict Cursed Inferno and Venom while wearing Reaver armor
- Officiel : Up to 30% increased damage the lower your life is / Current boost: [DAMAGE]% / All attacks inflict Acid Venom / Revenge

### Photoviscerator (Weapons.Ranged)

Version du code la plus proche : 2020-07 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.33

- Fandom : 90% chance to not consume gel / Fires a stream of exo flames and light that explodes into homing sparks / Right click to fire homing flares which stick to enemies and incinerate them
- Officiel : Emits a continuous beam of Miracle Blight that disintegrates everything / Press [open] to fire homing exo flares which stick to and incinerate enemies / Uses gel for ammo / The beam has a 95% chance to save ammo

### Polaris Parrotfish (Weapons.Ranged)

Version du code la plus proche : 2019-11 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.33

- Fandom : It carries the mark of the Northern Star / Projectile hits grant buffs to the weapon and the player / Buffs are removed on hit
- Officiel : Rapidly fires an array of colorful star shots / Firing speed increases the closer you are to an enemy / Ignores 15 points of enemy Defense / It carries the mark of the Northern Star
- Note de version : [2.0.4.001] Reworked Polaris Parrotfish, it no longer fires lasers which grant the Polaris Boost buff or get stronger on successive hits. It now rapidly fires star shots which can bounce off tiles. However, only 2 projectiles can exist at once, requiring staying close to 

### Pulse Turret Remote (Weapons.DraedonsArsenal)

Version du code la plus proche : 2022-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.33

- Fandom : A device used to defend against the weaker, less cognizant rogue creations of Draedon / Summons a pulse turret which eradicates nearby foes with focused energy blasts / Only one pulse turret may exist at a time
- Officiel : Summons a sentry / Summons a turret to fire explosive pulse blasts at your enemies / Only one pulse turret may exist at a time

### Sand Cloak (Accessories)

Version du code la plus proche : 2020-01 (ressemblance 0.96, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.33

- Fandom : +1 defense and 5% increased movement speed / Press [Sand Cloak Effect] to consume 25% of your maximum stealth to create a protective dust veil which provides +6 defense and +2 life regen / This effect has a 30 second cooldown before it can be used again
- Officiel : Dashing creates a large protective sand veil which damages enemies inside it / The sand veil lasts 15 seconds, but dissipates early if you dash again / The veil slowly follows you, and any player inside it gains 3 defense and 75% acceleration / After dissipating, the sand veil has a 15 second cooldown before it can be used again

### True Tyrant's Ultisword (Weapons.Melee)

Nom actuel : Defiled Greatsword. Version du code la plus proche : 2019-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.33

- Fandom : Fires blazing, hyper, and sunlight blades / Gives the player the tyrant's fury buff on enemy hits / This buff increases melee damage by 30% and melee crit chance by 10%
- Officiel : Fires blazing, hyper, and sunlight blades
- Note de version : [2.0.3.001] Renamed True Tyrant's Ultisword to Defiled Greatsword.

### Tyrant Yharim's Ultisword (Weapons.Melee)

Nom actuel : Blighted Cleaver. Version du code la plus proche : 2019-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.33

- Fandom : Fires homing blazing blades / Gives the player the tyrant's fury buff on enemy hits / This buff increases melee damage by 30% and melee crit chance by 10%
- Officiel : Fires homing, blazing blades
- Note de version : [2.0.3.001] Renamed Tyrant Yharim's Ultisword to Blighted Cleaver.

### Void of Calamity (Accessories)

Version du code la plus proche : 2022-05 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.33

- Fandom : Cursed? Reduces damage reduction by 10% / 15% increase to all damage / Brimstone fire rains down while invincibility is active
- Officiel : 12% increased damage / Calamitous flames rain down while invincible

### Aquatic Heart (Accessories)

Version du code la plus proche : 2021-09 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.38

- Fandom : Transforms the holder into a water elemental / Going underwater gives you a buff / Greatly reduces breath loss and provides a small amount of light in the abyss / Enemies become frozen when they touch you / You have a layer of ice around you that absorbs 20% damage but breaks after one hit / After 30 seconds the ice shield will regenerate / Wow, you can swim now! / Most of these effects are only active after Skeletron has been defeated
- Officiel : Transforms the holder into a light-emitting water elemental / Wow, you can swim now! / Being underwater increases max speed and acceleration by 15% / Greatly reduces breath loss and reduces the darkness intensity in the abyss / Attackers are inflicted with Frozen / Gain a fragile ice barrier that grants 20% damage reduction for one hit / The ice shield regenerates after 30 seconds

## Modifiées : le Fandom décrit une ancienne version, en partie différente

### Adamantite Throwing Axe (Weapons.Rogue)

Version du code la plus proche : 2020-06 (ressemblance 0.99, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Stealth strikes summon lighting bolts on enemy hits
- Officiel : Stealth strikes ignore gravity and rain down lightning on enemy hits

### Aether's Whisper (Weapons.Magic)

Version du code la plus proche : 2022-07 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Inflicts long-lasting shadowflame and splits on tile hits / Projectiles gain damage as they travel
- Officiel : Fires void beams that gain damage as they travel and split on tile hits

### Alula Australis (Weapons.Magic)

Version du code la plus proche : 2020-06 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires a beautiful aurora trailed by a star shower
- Officiel : Fires a beautiful aurora that spreads the Astral Infection (debuff) and is trailed by a star shower

### Anahita's Arpeggio (Weapons.Magic)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Casts slow-moving treble clefs that confuse enemies
- Officiel : Holding [use] casts up to 6 treble clefs that orbit the player / Releasing [use] causes the treble clefs to converge at the cursor / After converging, treble clefs seek your cursor
- Note de version : [2.1.0] Reworked Anahita's Arpeggio, it no longer fires bouncing music notes towards the cursor. Holding left-click now spawns up to 6 music notes which orbit around the player. Releasing left-click causes the music notes to briefly fade away before reappearing around

### Arterial Assault (Weapons.Ranged)

Version du code la plus proche : 2021-03 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires a chain of 5 arrows from the sky / Wooden arrows are converted into homing bloodfire arrows
- Officiel : Fires a group of 5 homing blood arrows / These blood arrows burst into blood orbs upon striking an enemy / Picking up these blood orbs reduces the duration of Potion Sickness
- Note de version : [2.1.0] Reworked Arterial Assault, it no longer rains 5 arrows from the sky on each use that can home in on enemies. It now fires a burst of 5 blood arrows from the bow which can home in on enemies within a small range and spawn a blood orb on hit. Each blood orb coll

### Astral Pike (Weapons.Melee)

Version du code la plus proche : 2019-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Summons astral star swarms on critical hits
- Officiel : Summons astral star swarms on hit that spread the Astral Infection (debuff)

### Ball O' Fugu (Weapons.Melee)

Version du code la plus proche : 2019-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Throws a fish that spews homing spikes
- Officiel : Throws a flailing abyssal pufferfish / Builds up poisonous spikes as it spins / Spikes blow out of the fish as it retracts
- Note de version : [2.1.0] Reworked Ball O' Fugu, it no longer launches a flail which spawns homing spikes as it travels. It now uses the same functionality as vanilla thrown flails; holding left-click spins the flail around the player, releasing left-click launches the flail towards th

### Broken Water Filter (Tools)

Version du code la plus proche : 2020-11 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Favorite this item to disable natural Acid Rain spawns
- Officiel : Toggle this item to prevent Acid Rain from occurring / Press [open] to toggle / Spawns are currently [STATE]

### Calamari's Lament (Weapons.Summon)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Summons a squid to fight for you
- Officiel : Summons a miniature squid that viciously inks enemies / At long distance, the squid will fire aggressively homing ink volleys / At close range, the squid will latch onto enemies and deal increased damage / In the Abyss, all kinds of resources are scarce. / Every species is solitary. There simply is not enough to share.
- Note de version : [2.0.3.005] Reworked Calamari's Lament, it no longer hovers above enemies and fires piercing ink projectiles. Squid minions now hover nearby the player and fire homing ink projectiles from a large distance at enemies. Ink projectiles no longer pierce, but will only hit th

### Caustic Edge (Weapons.Melee)

Version du code la plus proche : 2019-06 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Inflicts poison and venom on enemy hits
- Officiel : Fires a bouncing caustic beam / Inflicts Acid Venom and slows down on enemy hits
- Note de version : [2.0.3.001] Renamed True Caustic Edge to Caustic Edge.

### Coral Spout (Weapons.Magic)

Version du code la plus proche : 2018-12 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Casts coral water spouts that lay on the ground and damage enemies
- Officiel : Casts a shotgun-like blast of coral shards / Keep the attack button held down to narrow the spread / Coral shards ignore 5 points of enemy defense / Fully charging the attack releases a single, bigger chunk of coral that sticks to enemies / Grabbing the chunk of coral after it falls from the enemy replenishes 150 mana / The mana-charged coral also provides the Coral Symbiosis buff / Knowledge is important, and the Old Sea Kingdom's many scribes knew it had to be preserved at any cost. / Their libraries were much larger than most, because of the water-proof stone tablets occupying them.
- Note de version : [2.0.0.001] Reworked Coral Spout, it no longer continuously fires out gravity-affected coral spike projectiles. It now fires out a spread of five pink coral spikes that travel a short distance before exploding. The weapon can be charged up to decrease the spread of the co

### Cursed Dagger (Weapons.Rogue)

Version du code la plus proche : 2020-05 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Throws bouncing daggers / Stealth strikes are showered in cursed fireballs
- Officiel : Throws a fast cursed dagger and two inaccurate weaker daggers / Stealth strikes throw a single dagger that bursts into a Cursed Inferno vortex
- Note de version : [2.0.3.005] Reworked Cursed Dagger, daggers no longer bounce, and stealth strike daggers no longer shower cursed fireballs. It now throws a fast dagger along with 2 slower, weaker daggers. Daggers pierce once, dealing 80% damage on the second hit. Stealth strikes now thro

### Darklight Greatsword (Weapons.Melee)

Version du code la plus proche : 2019-11 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires darklight blades that split on death
- Officiel : Fires darklight blades that slice through enemies, creating additional slashes

### Dazzling Stabber Staff (Weapons.Summon)

Version du code la plus proche : 2020-04 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Summons a holy blade to fight for you
- Officiel : Summons three holy daggers made of profaned materials to fight for you / Holy flame daggers gallantly turn all that oppose them to ashes / Crystalline daggers gracefully carve foes to ribbons / Profane daggers brutishly splinter and reform eternally / Ever onward, towards Her future
- Note de version : [2.1.0] Reworked Dazzling Stabber Staff, it now summons 3 daggers per use, taking up a combined 1 minion slot. Crystal daggers rapidly slash enemies before briefly pausing, dealing 50% base damage but having an innate 40 armor penetration. Flame daggers swipe at enemi

### Death Valley Duster (Weapons.Magic)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Casts a large blast of dust
- Officiel : Casts an enchanted blast of sand that passes through enemies and tiles / Casting this spell at max mana while without mana sickness will cause it to give you and any player it hits the Sand's Wind buff
- Note de version : [2.1.0] Reworked Death Valley Duster, it no longer casts a short blast of dust. It now casts an enchanted sand blast that lingers for a short time. Using the weapon while at maximum mana and while not inflicted with Mana Sickness will grant the player and any player t

### Depths Charm (Accessories)

Version du code la plus proche : 2019-06 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Reduces the damage caused by the pressure of the abyss while out of breath / Removes the bleed effect caused by the upper layers of the abyss
- Officiel : Reduces the damage caused by drowning in the abyss / Removes the increased breath loss of Crush Depth

### Dreadmine Staff (Weapons.Summon)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Summons a dreadmine turret to fight for you
- Officiel : Summons a sentry / Summons a turret that swarms your enemies with sea mines

### Eldritch Soul Artifact (Accessories)

Nom actuel : Fragments of Another World. Version du code la plus proche : 2020-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Knowledge / Boosts melee speed by 10%, ranged velocity by 25%, rogue damage by 15%, max minions by 2, and reduces mana cost by 15%
- Officiel : Grants immunity to Whispering Death / 20% increased melee speed / 1.15x specialist ranged damage / 25% reduced mana usage / +2 maximum sentries / 10% increased rogue attack speed / Don't speak of your desires to the shadows, they listen.
- Note de version : [2.2.0] Renamed Eldritch Soul Artifact to Fragments of Another World.

### Elemental Gauntlet (Accessories)

Version du code la plus proche : 2022-01 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Melee attacks and projectiles inflict on fire, frostburn and holy flames / 15% increased melee speed, damage, and 5% increased melee critical strike chance / 20% increased true melee damage / Temporary immunity to lava / Increased melee knockback
- Officiel : Melee attacks and projectiles inflict Elemental Mix / 15% increased melee damage and melee speed / 5% increased melee critical strike chance / 20% increased true melee damage and increased melee knockback / Enables auto swing for melee weapons and increases their size / Does not stack with downgrades

### Elysian Arrow (Ammo)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Summons meteors from the sky on death
- Officiel : On impact, summons purifying light from above and below at the nearest enemy / Purifying light ignores 15 points of enemy Defense
- Note de version : [2.0.4.001] Reworked Elysian Arrow, it no longer rains meteors from the sky on hit. It now creates two purifying light slashes on death that will always target a nearby enemy, even if the arrow misses. These slashes hit once and deal 40% base damage. Nerfed damage from 20

### Enchanted Pearl (Accessories)

Version du code la plus proche : 2019-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Increases fishing skill / Increases chance to catch crates
- Officiel : Increases fishing power by 10 / Increases chance to catch crates and loot-containing fish by 15% / Crate catches are 25% more likely to be higher quality, accompanied by visual pearls

### Entropic Claymore (Weapons.Melee)

Version du code la plus proche : 2019-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires a spread of homing plasma balls
- Officiel : Fires a spread of living plasma that swims through the air / Plasma will track your cursor or enemies near it, prioritizing which is closest / The plasma pierces, but will stop tracking after hitting the first enemy
- Note de version : [2.1.0] Reworked Entropic Claymore, it no longer fires a spread of homing, non-piercing projectiles on swing. It now has a custom swing animation, and fires a spread of 5 tapeworm projectiles on swing. Tapeworm projectiles have an innate 15 armor penetration, and will

### Eradicator (Weapons.Rogue)

Nom actuel : Dimension-Tearing Disk. Version du code la plus proche : 2021-01 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Throws a disk that fires lasers at nearby enemies / Stealth strikes stick to enemies and unleash a barrage of lasers in all directions
- Officiel : Throws a disk that tears space in the direction of the nearest enemy / Stealth strikes stick to enemies and unleash God's Bane Doombeam Array
- Note de version : [2.1.0] Renamed Eradicator to Dimension-Tearing Disk.

### Fish of Flight (Fishing)

Version du code la plus proche : 2019-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Right click to extract souls
- Officiel : Press [open] to extract Soul of Flight souls

### Frigidflash Bolt (Weapons.Magic)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Casts a slow-moving ball of flash-freezing magma
- Officiel : Casts a fast ball of flash-freezing magma that bounces off tiles / When hitting a tile, a flarefrost burst is produced / Press [open] to cast an empowered ball that creates a collapsing flarefrost vortex which explodes
- Note de version : [2.1.0] Reworked Frigidflash Bolt, it no longer casts a slow-moving ball of freezing magma which bounces off tiles. It now casts a ball of freezing magma which bounces off tiles, creating a frigidflash blast whenever it does so. Balls of freezing magma eventually slow

### Frost Bolt (Weapons.Magic)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Casts a slow-moving ball of frost
- Officiel : Casts a ball of frost that bounces off tiles / When hitting a tile, a small frost burst is produced / Frost projectiles will fall to the ground after enough time has passed
- Note de version : [2.1.0] Reworked Frost Bolt, it now casts a ball of frost which bounces off tiles, creating a frost blast whenever it does so. Balls of frost eventually slow down and gain gravity after traveling for 3.67 seconds, gaining gravity faster after hitting enemies. Buffed d

### Gelitic Blade (Weapons.Melee)

Version du code la plus proche : 2019-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires a gel wave that slows down on enemy hits
- Officiel : Fires a fast gel wave that speeds up on enemy hits and covers them with Slime (debuff)

### Genesis Pickaxe (Tools)

Version du code la plus proche : 2021-12 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Can mine Uelibloom Ore located in the Jungle
- Officiel : Can mine Uelibloom Ore
- Note de version : [1.4.5.001] Renamed Gallant Pickaxe to Genesis Pickaxe.

### Genisis (Weapons.Magic)

Nom actuel : Genesis. Version du code la plus proche : 2019-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires a Y-shaped beam of destructive energy and a spread of lasers
- Officiel : Fire an array of lasers that decreases in spread for each shot / Every sixth shot fires a splitting Genesis Beam and resets the spread of the weapon / Hitting an enemy before the Genesis Beam splits deals increased damage
- Note de version : [2.0.4.001] Renamed Genisis to Genesis.
- Note de version : [2.0.4.001] Reworked Genesis, it no longer fires splitting beams and a random spread of lasers. It now fires a large spread of lasers which gets tighter on successive shots. Every sixth shot replaces the spread of lasers with a powerful splitting beam, which resets the la

### Glimmering Gemfish (Fishing)

Version du code la plus proche : 2019-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Right click to extract gems
- Officiel : Press [open] to extract gems

### Godspawn Helix Staff (Weapons.Summon)

Nom actuel : Starspawn Helix Staff. Version du code la plus proche : 2020-01 (ressemblance 0.84, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Summons blighted probes to protect you
- Officiel : Summons astral probes that spread the Astral Infection (debuff)
- Note de version : [2.0.3.003] Renamed Godspawn Helix Staff to Starspawn Helix Staff.

### Greatbay Pickaxe (Tools)

Version du code la plus proche : 2021-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Can mine Demonite, Crimtane and Meteorite
- Officiel : Can mine Demonite, Crimtane, Meteorite, Sea Prisms and Sea Prism Crystals

### Heavy Laser Rifle (Weapons.DraedonsArsenal)

Nom actuel : W-LSR: Phalanx Surge. Version du code la plus proche : 2022-07 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Laser weapon used by heavy infantry units in Yharim's army
- Officiel : Fires a spread of lasers / Holding [open] charges the weapon, allowing it to thrust forward and imaple enemies with an energy spear / Holding [use] when firing the charge attack will launch you forward at high speeds and deal extra damage / Using the charged attack has a short cooldown / Though Godseeker Knights are often brutish, not all prefer close quarters melee combat. / As such, Yharim personally requested that I create heavy weaponry for them despite being few in number.
- Note de version : [2.1.0] Renamed Heavy Laser Rifle to W-LSR: Phalanx Surge.
- Note de version : [2.1.0] Reworked W-LSR: Phalanx Surge, it no longer fires a pair of instant-traveling laser beams. It now fires a wide spread of lasers. Holding right-click charges the weapon, taking 1 second to fully charge. Releasing right-click at full charge thrusts a laser lance

### Hive Pod (Weapons.Summon)

Version du code la plus proche : 2019-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Summons an astral hive to protect you
- Officiel : Summons a sentry / Summons an astraglomerate to spawn glomerings at your enemies / Glomerings spread the Astral Infection (debuff) on impact

### Holy Collider (Weapons.Melee)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Striking enemies will cause them to explode into holy fire
- Officiel : Swings a holy greatsword that can be charged for higher damage / Charged swings create four lingering flares that float around the point of impact / Striking a flare with an uncharged swing will push it around and extend its lifetime / Charged swings can empower flares, turning them into massive fireballs on hit and launching them to your cursor
- Note de version : [2.1.0] Reworked Holy Collider, it no longer functions as a standard broadsword which spawns homing holy fires on hit. It now has a custom swing animation, where the sword is swung back and forth. Holding left-click charges up a swing of the sword, which is performed 

### Ichor Spear (Weapons.Rogue)

Version du code la plus proche : 2020-05 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Throws an ichor tipped trident / Stealth strikes are showered in splashes of ichor
- Officiel : Throws a high-velocity Ichor (debuff) tipped javelin that pierces through enemies / Stealth strikes impale enemies and deal repeated damage
- Note de version : [2.0.3.005] Reworked Ichor Spear, stealth strikes no longer shower ichor shots. It now throws a fast ichor javelin which pierces 5 times and grants 50 local immunity frames. Stealth strikes now stick to enemies, dealing repeated damage. Buffed damage from 84 to 132 and kn

### Igneous Exaltation (Weapons.Summon)

Version du code la plus proche : 2020-04 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Summons an orbiting blade / Right click to launch all blades towards the cursor
- Officiel : Summons an Igneous blade that orbits around you / When your minion slots are full, [use] again to launch all blades towards the cursor / This ability has a two-second cooldown that requires holding this item / Shift-[open] this item in your inventory to change the rotation direction

### Jelly-Charged Battery (Accessories)

Version du code la plus proche : 2020-03 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : +1 max minions and 7% minion damage / Minion attacks spawn orbs of energy and inflict Electrified
- Officiel : Increases your max number of minions by 1, and summon damage by 7% / Minion attacks create charged orbs

### Napalm Arrow (Ammo)

Nom actuel : Cinder Arrow. Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Explodes into fire shards
- Officiel : Explosive arrows that burst into weak flaming Brimstone Shards
- Note de version : [2.0.4.001] Renamed Napalm Arrow to Cinder Arrow.
- Note de version : [2.0.4.001] Reworked Cinder Arrow, it no longer splits into piercing flames on hit. It now creates a small explosion that deals 50% base damage on hit, and launches 3 shrapnel projectiles upwards from the hit location that hit once, deal 10% base damage, and inflict Brims

### Night's Gaze (Weapons.Rogue)

Nom actuel : Vega. Version du code la plus proche : 2020-06 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Strike your foes with this spear of the night / Throws a spear that shatters when it hits an enemy / Stealth strikes cause the spear to summon homing stars as it flies
- Officiel : Throws a spear that shatters into star fragments on impact and generates Starbursts / Stealth strike conjure an image of Lyra to deal immense damage in an area / [open] to consume 20 Starbursts and immediately perform a stealth strike, regardless of your current stealth / This strike calls all star fragments to attack the damaged foe and inflicts Voidfrost / Consuming Starbursts with Vega allows using stealth for 10 seconds, even without rogue armor / The final song of a misunderstood genius... or just a madman
- Note de version : [2.1.0] Renamed Night's Gaze to Vega.
- Note de version : [2.1.0] Reworked Vega, it no longer throws a javelin which shatters into stars and sparks on hit, and stealth strikes no longer leave a trail of stars as they travel. It now throws a fast javelin which shatters into lingering stars and generates Starburst on hit. Star

### Norfleet (Weapons.Ranged)

Version du code la plus proche : 2021-05 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fire everything!
- Officiel : Fires a cluster of erratic energy spheres that each contain a different element / Every three shots the weapon must recharge its energy to reload / Blows up everything!
- Note de version : [2.0.4.001] Reworked Norfleet, it no longer fires bursts of star projectiles which are fired more often the longer the weapon is fired, and no longer consumes ammo. It now fires bursts of 3 erratic energy spheres. Each energy sphere explodes on hit and inflicts a differen

### Orthocera Shell (Weapons.Summon)

Version du code la plus proche : 2020-04 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Summons a flying orthocera sentry at the mouse position
- Officiel : Summons a sentry / Summons a flying orthocera to spit Irradiated and Acid Venom streams at your enemies

### Palladium Javelin (Weapons.Rogue)

Version du code la plus proche : 2020-06 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Stealth strikes split into more javelins
- Officiel : Stealth strikes split into more javelins and boost natural life regeneration on hits

### Pulse Pistol (Weapons.DraedonsArsenal)

Nom actuel : P-PLS: Pulse Pistol. Version du code la plus proche : 2022-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires a pulse that arcs to a new target on enemy hits
- Officiel : Fires a pulse orb that tracks nearby targets / After hitting an enemy, the pulse orb will split in two / Pulse orb pierce will increase when killing enemies
- Note de version : [2.1.0] Renamed Pulse Pistol to P-PLS: Pulse Pistol.
- Note de version : [2.1.0] Reworked P-PLS: Pulse Pistol, it no longer fires a fast beam which arcs between nearby enemies. It now fires a pulse orb which slows down before tracking the nearest enemy. After hitting an enemy, the pulse orb splits into two smaller pulse orbs which deal 25%

### Radiant Ooze (Accessories)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : You emit light and regen life more quickly at night
- Officiel : +0.5 to +2.5 HP/s life regen based on missing health / You emit yellow light if visibility is enabled / (Current regen: +[REGEN] HP/s)
- Note de version : [2.0.3.003] Reworked Radiant Ooze, it no longer grants 4 life regeneration at nighttime, and now grants between 1 and 3 life regeneration depending on missing health.

### Seabound Staff (Weapons.Summon)

Nom actuel : Brittle Star Staff. Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Summons a brittle star to fight for you
- Officiel : Summons a small brittle starfish that will dash through enemies until it shatters / Starfish will quickly regenerate after shattering / Holding [open] will swap starfish between dashing and circling you / When the starfish are circling, each one provides you with 3 defense
- Note de version : [2.0.3.007] Renamed Seabound Staff to Brittle Star Staff.
- Note de version : [2.0.3.007] Reworked Brittle Star Staff. Brittle stars now shatter after damaging an enemy 4 times, quickly regenerating afterwards. Right clicking causes all active brittle stars to stop attacking and orbit around the player. While orbiting, brittle stars still deal dama

### Seething Discharge (Weapons.Magic)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires a barrage of brimstone blasts
- Officiel : Fires an accelerating arc of brimstone hellblasts / Every third attack spawns a brimstone dart ring which chases after foes
- Note de version : [2.1.0] Reworked Seething Discharge, it no longer fires a random spread of 1 hellblast and 2 darts. It now fires an arc of 5 hellblasts that accelerate over time. Every third use of the weapon additionally spawns a ring of 8 brimstone darts that can home in on enemies

### Silencing Sheath (Accessories)

Version du code la plus proche : 2022-10 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : +10 maximum stealth
- Officiel : +10 maximum stealth, and stealth generates 4% faster

### Soma Prime (Weapons.Ranged)

Version du code la plus proche : 2021-03 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Converts musket balls into extremely powerful high velocity rounds that inflict a powerful bleed debuff
- Officiel : This weapon can supercrit if its crit chance is over 100% / All bullets fired have a 5% chance to inflict Shred, a stacking bleed debuff / Critical strikes deal 3x damage instead of 2x by default and have a 30% chance to inflict Shred / Shred deals 600 DPS per stack and scales with your ranged stats / Damage ticks of Shred can critically strike for 3x damage and can supercrit / Replaces musket balls with enhanced High Velocity Bullets / 80% chance to save ammo
- Note de version : [1.2.0.001] Renamed Soma Prime to THIS WEAPON IS A REFERENCE TO WARFRAME OH MAH GAWD.
- Note de version : [1.2.3.006] Renamed Gun to Soma Prime.
- Note de version : [1.5.1.001] Reworked Soma Prime, it now inflicts Shred regardless of bullet type. Shred now inflicts 75 damage per second instead of 750 but it is now affected by ranged damage boosts and can stack on top of each other. Additionally, critical strike chance above 100% can 

### Spear of Paleolith (Weapons.Rogue)

Version du code la plus proche : 2020-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Throws an ancient spear that shatters enemy armor / Spears rain fossil shards as they travel / Stealth strikes travel slower but further, raining more fossil shards
- Officiel : Throws an ancient spear that shatters into fossil shards upon hitting an enemy / Stealth strikes remain impaled in the enemy for 10 seconds / While an enemy is impaled, all spears that strike them erupt into even more fossil shards
- Note de version : [2.2.0] Reworked Spear of Paleolith, it no longer throws a javelin that rains fossil shards as it travels, and stealth strikes no longer travel slower but farther. It now throws a javelin that shatters into fossil shards on hit. Fossil shards linger on surfaces, pierc

### Spectralstorm Cannon (Weapons.Ranged)

Version du code la plus proche : 2020-09 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : 70% chance to not consume flares / Fires a storm of lost souls and flares
- Officiel : Fires a barrage of spectral Nightwither flares that builds up a searing heat in the weapon / While cooling down, heat-seeking phantoms are continuously released / Firing for too long overheats the weapon, leaving it unusable for a short period and inflicting you with Nightwither / However, triggering overheat releases a powerful burst of souls and resets heat / Press [open] to put away the weapon, which preserves its heat / 33% chance to save ammo / Ectoplasmic blast rated visor mandatory
- Note de version : [2.1.0] Reworked Spectralstorm Cannon, it no longer fires a constant stream of flares and homing souls. It now fires a barrage of flares which increases in fire rate over time. All flares are converted into spectral flares which inflict Nightwither. Firing the weapon 

### Squirrel Squire Staff (Weapons.Summon)

Version du code la plus proche : 2020-03 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Summons a squirrel squire to fight for you
- Officiel : Summons a sentry / Summons a squirrel to throw acorns at your enemies

### Starfall (Weapons.Magic)

Nom actuel : Star Shower. Version du code la plus proche : 2019-08 (ressemblance 0.93, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Casts a spread of astral blight stars at the mouse cursor
- Officiel : Casts a spread of astral stars from the sky that converge at the cursor and spread the Astral Infection (debuff)
- Note de version : [2.0.3.001] Renamed Starfall to Star Shower.

### Stellar Cannon (Weapons.Ranged)

Version du code la plus proche : 2019-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Launches an explosive astral crystal
- Officiel : Launches an explosive astral crystal that spreads the Astral Infection (debuff)

### Stellar Contempt (Weapons.Melee)

Version du code la plus proche : 2019-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Lunar flares rain down on enemy hits
- Officiel : Throws a heavy, gravity-affected hammer that will return to you after striking an enemy / Consecutive hits call down more and more lunar flares from the sky / After hitting five times, the hammer supercharges with lunar energy to pulverize your target
- Note de version : [2.0.3.001] Reworked Stellar Contempt, it no longer throws a hammer that functions like a boomerang and spawns lunar flares on hits. It now throws a fast hammer that rains lunar flares and returns to the player on enemy hits. The more hits landed, the more lunar flares ar

### Telluric Glare (Weapons.Ranged)

Version du code la plus proche : 2021-04 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Fires volleys of four colossal radiant arrows which can pass through walls
- Officiel : Rapidly fires volleys of arrows / Every other arrow is converted into radiant greatarrows which pierce but quicky disappear after striking / Wooden arrows are converted into more powerful greatarrows that deal increased damage

### Triactis' True Paladinian Mage-Hammer of Might (Weapons.Melee)

Version du code la plus proche : 2019-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Explodes on enemy hits
- Officiel : Throws a heavy, gravity-affected hammer that will return to you after striking an enemy / Enemy hits release prismatic light flares which orbit around the struck foe / Instead of returning, the fourth hit transforms flares into hardlight hammers / These hammers converge and form a universe-collapsing explosion that always critically strikes
- Note de version : [2.1.0] Reworked Triactis' True Paladinian Mage-Hammer of Might, it no longer throws a hammer that functions like a boomerang and causes explosions on hits. It now throws a large, gravity-affected hammer that returns to the player on enemy hits. Every hit of the hamme

### Tundra Leash (Mounts)

Version du code la plus proche : 2019-06 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Summons an angry dog mount
- Officiel : Summons a Rimehound mount

### Urchin Stinger (Weapons.Rogue)

Version du code la plus proche : 2020-01 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Stealth strikes stick to enemies while releasing sulfuric bubbles
- Officiel : Sticks into enemies and injects an extremely potent poison / Stealth strikes stick into and make enemies Poisoned for longer / Sticking seven stingers in an enemy with a stealth strike stuck in it spawns an irradiated blast
- Note de version : [2.1.0] Reworked Urchin Stinger, stealth strikes no longer stick to enemies and constantly release acid bubbles. It now throws a spread of 2 urchin stingers which stick into enemies, inflicting Poisoned which can bypass immunity. Stealth strikes now throw 1 urchin sti

### Vampiric Talisman (Accessories)

Version du code la plus proche : 2019-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : rogue projectiles give lifesteal on crits / 12% increased rogue damage
- Officiel : Rogue critical strikes steal enemy life / Stealth strikes grant 15% critical strike chance and increased lifesteal to non-stealth strikes for 10 seconds / Stealth strikes inflict Heavy Bleeding and Armor Crunch, and deal 8% more damage / Possessing the talisman makes you yearn for blood
- Note de version : [2.1.0] Reworked Vampiric Talisman, it no longer grants 12% rogue damage. It now makes stealth strikes deal 8% more damage, and inflict Armor Crunch for 2.5 seconds and Heavy Bleeding for 5 seconds. Stealth strikes also increase critical strike chance by 15% and multi

### Voltaic Jelly (Accessories)

Version du code la plus proche : 2020-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : +1 max minions / Minion attacks inflict Electrified
- Officiel : Increases your max number of minions by 1 / Minion attacks inflict Static Discharge

### Abyssal Diving Suit (Accessories)

Version du code la plus proche : 2021-01 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.14

- Fandom : Transforms the holder into an armored diver / Increases max movement speed and acceleration while underwater but you move slowly outside of water / The suits' armored plates reduce damage taken by 15% / The plates will only take damage if the damage taken is over 50 / After the suit has taken too much damage its armored plates will take 3 minutes to regenerate / Reduces the damage caused by the pressure of the abyss while out of breath / Removes the bleed effect caused by the abyss in all layers except the deepest one / Grants the ability to swim and greatly extends underwater breathing / Provides light underwater and extra mobility on ice / Provides a moderate amount of light in the abyss / Greatly reduces breath loss in the abyss / Reduces creature's ability to detect you in the abyss / Reduces the defense reduction that the abyss causes / Allows you to fall faster while in liquids
- Officiel : Transforms the holder into an armored diver: / Increases max movement speed and acceleration while underwater, but reduces it outside of water / Grants the ability to swim, extends underwater breathing, emits light underwater, and improves mobility on ice / Provides a moderate amount of light, reduces breath loss, and reduces damage caused by drowning in the abyss / Abyss creatures are less likely to see you, mitigates the abyss' defense reduction, and lets you sink faster in liquids / Grants immunity to the sulphurous waters and Crush Depth's increased breath loss / Grants resistance to the increased breath loss caused by Hadopelagic Pressure

### Angel Treads (Accessories)

Version du code la plus proche : 2022-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.14

- Fandom : Extreme Speed! / 36% increased running acceleration / Increased flight time / Greater mobility on ice / Water and lava walking / Immunity to the On Fire! debuff / Temporary immunity to lava
- Officiel : Allows flight, super fast running, and extra mobility on ice / 12% increased movement speed and 10% increased flight time / Provides the ability to walk on water, honey & lava / Grants immunity to fire blocks and 7 seconds of immunity to lava / Reduces damage from touching lava / Extreme speed!

### Corpus Avertor (Weapons.Rogue)

Version du code la plus proche : 2021-01 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.17

- Fandom : Seems like it has worn down over time / Attacks grant lifesteal based on damage dealt / The lower your HP the more damage this weapon does and heals the player on enemy hits / Stealth strikes throw a single rainbow outlined dagger / On enemy hits, this dagger boosts the damage and life regen of all members of your team / However, there is a small chance it will cut your health in half instead
- Officiel : Throws a spinning dagger that spawns a rebounding clone / Deals more damage the lower your health, and grants lifesteal / Stealth strikes throw a piercing rainbow dagger which spawns an extra slash in its wake, at a blood cost / This dagger grants players on your team 10% damage and +2 HP/s life regen on hit, and heals them for a portion of damage dealt / Seems like it has worn down over time

### Temporal Umbrella (Weapons.Summon)

Version du code la plus proche : 2020-04 (ressemblance 0.8, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.2

- Fandom : Surprisingly sturdy, I reckon this could defeat the Mafia in a single blow / Summons a magic hat to hover above your head / The hat will release a variety of objects to assault your foes / Requires 8 minion slots to use and there can only be one hat
- Officiel : Summons a variety of tools to assist you in battle: / - A vermilion arrow to pierce your adversaries / - A marmalade hammer to bonk enemies in style / - A jade axe to cleave monsters and occasionally fell a tree / - A cyan umbrella to beat down your foes with grace / - A lavender rifle to snipe those who stand in your way / Best of all, you get to wear a top hat / There can only be one set of tools at a time / Surprisingly sturdy, I reckon this could defeat the Mafia in a single blow
- Note de version : [2.0.2.001] Reworked Temporal Umbrella, it no longer summons a magic hat which spawns various projectiles. Five minions are now permanently active and attack enemies:
- Note de version : [2.0.3.005] Reworked Profaned Soul Crystal. It now has an empowered state that is triggered by having no other minions summoned, and is no longer nerfed when using Endogenesis or Temporal Umbrella. Its enraged state is now triggered at nighttime instead of when below 50% 

### Halley's Inferno (Weapons.Ranged)

Version du code la plus proche : 2019-11 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.25

- Fandom : Halley came sooner than expected / Fires a flaming comet / 50% chance to not consume gel / Right click to zoom out
- Officiel : Fires a burst of intensely concentrated lunar fire jets / [open] to rapid-fire Starbursts that inflict Voidfrost / Generates Starbursts when hitting enemies, depending on your accuracy / Uses gel for ammo / 50% chance to save ammo / Halley arrived sooner than expected

### Statis' Curse (Accessories)

Version du code la plus proche : 2021-07 (ressemblance 0.99, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.25

- Fandom : Increased max minions by 3, does not stack with downgrades / 10% increased minion damage / Increased minion knockback / Minions inflict holy flames and shadowflames on hit
- Officiel : Increases your max number of minions by 3, and summon damage by 12% / Increased minion knockback / Minion attacks inflict Shadowflame, and grants immunity to Shadowflame / Does not stack with downgrades

### Abyssal Mirror (Accessories)

Version du code la plus proche : 2021-11 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.33

- Fandom : Light does not reach the depths of the ocean / Significantly reduces enemy aggression, even in the abyss / Stealth generates 30% faster when standing still and 20% faster while moving / Grants the ability to evade attacks, releasing a cloud of lumenyl fluid which damages and stuns nearby enemies / Evading an attack grants a lot of stealth but has a 90 second cooldown / This cooldown is shared with all other dodges and reflects
- Officiel : Enemies are less likely to target you, even in the abyss / Generate stealth 25% faster when standing still and 12% faster while moving / Grants or improves the ability to dodge attacks / Dodging an attack restores stealth and releases a cloud that damages and stuns nearby enemies / Light does not reach the depths of the ocean

### Badge of Bravery (Accessories)

Version du code la plus proche : 2021-04 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.33

- Fandom : 15% increased melee speed / Increases melee damage and melee crit by 5% / +5 armor penetration
- Officiel : Increases melee damage and critical strike chance by 10% / Increases melee armor penetration by 5

### Blood Boiler (Weapons.Ranged)

Version du code la plus proche : 2021-04 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.33

- Fandom : Fires a stream of lifestealing bloodfire / Uses your health as ammo / 25% chance to not consume ammo
- Officiel : Fires twin streams of bloodfire that returns to you / Returning bloodfire reduces Potion Sickness if it struck an enemy / Uses your health as ammo / 20% chance to save health

### Bloodsoaked Crasher (Weapons.Rogue)

Version du code la plus proche : 2019-11 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.33

- Fandom : Slows down when hitting an enemy. Speeds up otherwise / Heals on enemy hits / Stealth strikes spawn homing blood on enemy hits
- Officiel : Throws a bloody axe that is affected by gravity and can bounce twice / The axe speeds up over time, but will slow down upon hitting an enemy / Spews blood orbs upon enemy hits / Picking up these blood orbs reduces the duration of Potion Sickness / Stealth strikes spawn homing blood bolts on hit

### Burning Strife (Weapons.Rogue)

Version du code la plus proche : 2020-01 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.33

- Fandom : Throws a shadowflame spiky ball that bursts into flames / Stealth Strikes make the ball linger and explode more violently / Definitely not pocket safe
- Officiel : Throws an enchanted spiky ball that explodes into Shadowflame / Stealth strikes make the ball erupt into tentacles on hit / Definitely not pocket safe

### Desert Medallion (SummonItems)

Version du code la plus proche : 2021-11 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.33

- Fandom : The desert sand stirs... / Summons the Desert Scourge / Not consumable
- Officiel : Summons the Desert Scourge when used in the Desert / Enrages outside the Desert / Not consumable / The Scourge seeks for another meal.

### Elemental Quiver (Accessories)

Nom actuel : Planebreaker's Pouch. Version du code la plus proche : 2021-04 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.33

- Fandom : 15% increased ranged damage, 5% increased ranged critical strike chance, and 20% reduced ammo usage / 5 increased defense, 2 increased life regen, and 15% increased pick speed / Greatly increases arrow speed and grants a 20% chance to not consume arrows
- Officiel : 15% increased ranged damage, 7% increased ranged critical strike chance and 20% reduced ammo usage / Grants a 20% chance to not consume arrows / Pressing [Ammo Cycle Hotkey] will automatically swap the ammos in your ammo slots / The ammo type swapped depends on the ammo of the weapon you are holding
- Note de version : [2.1.0] Renamed Elemental Quiver to Planebreaker's Pouch.

### Elysian Tracers (Accessories.Wings)

Nom actuel : Void Striders. Version du code la plus proche : 2022-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.33

- Fandom : Ludicrous speed! / 36% increased running acceleration / Greater mobility on ice / Water and lava walking / Immunity to the On Fire! debuff / Temporary immunity to lava
- Officiel : Allows flight and slow fall / Max horizontal speed: Excellent / Max vertical speed: Excellent / Horizontal acceleration: Excellent / Vertical acceleration: Great / 36% increased running acceleration / Provides the effects of Angel Treads / Provides immunity to lava / Ludicrous speed!
- Note de version : [2.1.0] Renamed Elysian Tracers to Void Striders. <!-- Is named Void Kicks when wings are disabled -->

### Eye of Magnus (Weapons.Typeless)

Version du code la plus proche : 2022-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.33

- Fandom : Fires powerful beams that reduce enemy protection / This weapon scales with all your damage stats at once / Heals mana and health on hit
- Officiel : Fires seeking cosmic masses that restore health and mana on hit / Explodes into a nebula cloud that causes enemies to become Marked for Death / This weapon scales with the average of all your damage stats

### Galaxia (Weapons.Melee)

Version du code la plus proche : 2022-04 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.33

- Fandom : FUNCTION_DESC / FUNCTION_PASSIVE / Upgrading the sword let it break free from its earthly boundaries. You now have access to every single attunement at all times! / Use RMB to cycle the sword's attunement forward or backwards depending on the position of your cursor / Active Attunement : None / Passive Blessing : None
- Officiel : FUNCTION_DESC / Freed from earthly shackles and coursing with cosmic power / Has access to all of the attunements at all times / Press [open] to cycle the sword's attunement depending on the cursor's position / Active Attunement : [ATTUNEMENT]

### Gel Dart (Weapons.Rogue)

Version du code la plus proche : 2020-05 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.33

- Fandom : Throws bouncing darts / Stealth strikes ignore gravity and bounce more vigorously / They additionally leak slime and cover enemies in dark sludge
- Officiel : Throws bouncing gel darts that cover enemies in Slime (debuff) / Stealth strikes ignore gravity, bounce more vigorously, and constantly leak slime

### Graydron (Weapons.Melee)

Nom actuel : Murasama. Version du code la plus proche : 2019-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.33

- Fandom : There will be blood! / ID and power-level locked / Prove your strength or have the correct user ID to wield this sword
- Officiel : A high-frequency blade that can cleanly slice through even the mightiest foe / Every third swing deals immense damage / Prove your worth by slaying The Devourer of Gods to unsheath this sword / There will be blood!

### Hadal Stew (Potions)

Version du code la plus proche : 2020-09 (ressemblance 0.85, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.33

- Fandom : Only gives 50 seconds of Potion Sickness / Grants Well Fed
- Officiel : Only gives 50 seconds of Potion Sickness / Major improvements to all stats
- Note de version : [1.4.2.103] Renamed Sunken Stew to Hadal Stew.

### Hardened Honeycomb (Weapons.Rogue)

Version du code la plus proche : 2020-01 (ressemblance 0.98, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.33

- Fandom : Fires a honeycomb that shatters into fragments / Grants the honey buff to the player it touches / Stealth strikes can bounce off walls and enemies
- Officiel : Fires a honeycomb that shatters into fragments / Stealth strikes grant the Honey (buff) buff on hit and spawn bees

### Ice Star (Weapons.Rogue)

Version du code la plus proche : 2020-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.33

- Fandom : Throws homing ice stars / Stealth strikes pierce infinitely and spawn ice shards on hit / Ice Stars are too brittle to be recovered after being thrown
- Officiel : Throws fast but brittle homing ice stars / Stealth strikes last longer and spawn smaller homing stars on first hits

### Pwnagehammer (Weapons.Melee)

Version du code la plus proche : 2020-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.33

- Fandom : Throws a heavy, gravity-affected hammer that creates a loud blast of hallowed energy when it hits something / There is a 20 percent chance for the hammer to home in on a target / Homing hammers summon an additional spectral hammer on hit and are guaranteed to land a critical hit
- Officiel : Throws a heavy, gravity-affected hammer that creates a loud blast of hallowed energy when it hits something / After hitting four times, a spectral hammer will be summoned to strike the enemy

### Scorched Earth (Weapons.Ranged)

Version du code la plus proche : 2022-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.33

- Fandom : Fires a burst of four fuel-air rockets which explode into cluster bombs / Each burst consumes two rockets each / Burns your targets to a fine crisp
- Officiel : Fires a burst of four heat seeking missiles that rapidly accelerate after a short delay / All rockets explode into napalm cluster bombs on impact / 50% chance to save ammo / Burns your targets to a fine crisp

### Sigil of Calamitas (Accessories)

Version du code la plus proche : 2021-10 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.33

- Fandom : 15% increased magic damage and 10% decreased mana usage / +100 max mana / Increases pickup range for mana stars
- Officiel : Increases maximum mana by 60 / 15% increased magic damage / Increases pickup range for mana stars

### Statis' Blessing (Accessories)

Version du code la plus proche : 2021-07 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.33

- Fandom : Increased max minions by 2 and 10% increased minion damage / Increased minion knockback / Minions inflict holy flames on hit
- Officiel : Increases your max number of minions by 2, and summon damage by 12% / Increased minion knockback

### The Microwave (Weapons.Melee)

Version du code la plus proche : 2020-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.33

- Fandom : Fries nearby enemies with radiation / A very agile yoyo / Cooking, Astral Infection style
- Officiel : Reach: 32 tiles / Speed: 54 / Infinite throw duration / Fries nearby enemies with radiation and spreads the Astral Infection (debuff) / Cooking, Astral Infection style

### Tyranny's End (Weapons.Ranged)

Version du code la plus proche : 2020-06 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.33

- Fandom : Pierce the heart of even the most heavily-armored foe / Fires a .70 caliber sniper round that bypasses enemy defense and DR / Rounds mark enemies for death and summon a swarm of additional bullets on crits
- Officiel : Converts musket balls into .70 caliber sniper rounds that deal devastating damage / Causes enemies to become Marked for Death and bypasses defense / Critical hits summon a swarm of 6 additional bullets / Pierce the heart of even the most heavily-armored foe

### Eclipse Mirror (Accessories)

Version du code la plus proche : 2021-11 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.36

- Fandom : Its reflection shows naught but darkness / +20 maximum stealth / 6% increased rogue damage, and 6% increased rogue crit chance / Vastly reduces enemy aggression, even in the abyss / Stealth generates 20% faster when standing still / Mobile stealth generation exponentially accelerates while not attacking / Stealth strikes have a 100% critical hit chance / Stealth strikes only expend 50% of your max stealth / Grants the ability to evade attacks in a blast of darksun light, which inflicts extreme damage in a wide area / Evading an attack grants full stealth but has a 90 second cooldown / This cooldown is shared with all other dodges and reflects
- Officiel : +10 maximum stealth / 6% increased rogue damage and rogue crit chance / Enemies are less likely to target you, even in the abyss / Stealth generates 25% faster when standing still and is boosted while moving if not attacking / Stealth strikes only expend 50% of your max stealth / Grants or improves the ability to dodge attacks / Dodging an attack restores stealth and releases a burst of highly damaging darksun light / Its reflection shows naught but darkness

### Counter Scarf (Accessories)

Version du code la plus proche : 2021-07 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.4

- Fandom : True melee strikes deal 10% more damage / Grants the ability to dash; dashing into an attack will cause you to dodge it / After a successful dodge you must wait 15 seconds before you can dodge again / This cooldown will be twice as long if you have Chaos State / While on cooldown, Chaos State will last twice as long
- Officiel : Grants the ability to dash; dashing into an attack will cause you to dodge it / After a successful dodge you must wait 30 seconds before you can dodge again

### Evasion Scarf (Accessories)

Version du code la plus proche : 2021-07 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.4

- Fandom : True melee strikes deal 15% more damage / Grants the ability to dash; dashing into an attack will cause you to dodge it / After a successful dodge you must wait 13 seconds before you can dodge again / This cooldown will be 50 percent longer if you have Chaos State / While on cooldown, Chaos State will be 50 percent longer
- Officiel : Grants the ability to dash; dashing into an attack will cause you to dodge it / After a successful dodge you must wait 30 seconds before you can dodge again / Has a shorter dash cooldown than most dashes

### Supreme Bait Tackle Box Fishing Station (Accessories)

Version du code la plus proche : 2019-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.4

- Fandom : The ultimate fishing accessory / Increases fishing skill by 80 / Fishing line will never break and decreases chances of bait consumption / Increases chance to catch crates / Sonar potion effect
- Officiel : Increases fishing power by 50 / Fishing line will never break and decreases chance of bait consumption / Allows fishing in lava

### Aero Stone (Accessories)

Version du code la plus proche : 2021-09 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : One of the ancient relics / Increases flight time, movement speed and jump speed by 10%
- Officiel : Grants 0.83 additional seconds of flight time / You slowly recover flight time while in the air while not flying or gliding / One of the ancient relics
- Note de version : [2.0.3.005] Reworked Aero Stone, it no longer increases movement speed, jump speed, and flight time by 10%. It now increases flight time by a flat 35 frames, and regenerates 1 frame of flight time every 5 frames while not flying or gliding. This flight recovery is disable

### Affliction (Accessories)

Version du code la plus proche : 2022-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Gives you and all other players on your team +1 life regen, / +10% max life, 7% damage reduction, 13 defense, and 10% increased damage
- Officiel : Gives all teammates +0.5 HP/s life regen, +10% max life, 7% damage reduction, 9 defense and 10% increased damage

### Air Spinner (Weapons.Melee)

Version du code la plus proche : 2020-03 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Fires feathers when enemies are near / A very agile yoyo
- Officiel : Reach: 20.625 tiles / Speed: 32 / Duration: 30 seconds / Fires feathers when enemies are near

### Anti-materiel Rifle (Weapons.Ranged)

Version du code la plus proche : 2021-03 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Converts musket balls into .50 caliber sniper rounds that rip apart enemy defense and DR / If you crit the target a second swarm of bullets will fire
- Officiel : Converts musket balls into .50 caliber sniper rounds that rip apart enemy defense and cause enemies to become Marked for Death / Unleashes a second swarm of bullets on hit, with additional bullets on critical strikes

### Aorta (Weapons.Melee)

Version du code la plus proche : 2020-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Fires homing blood when enemies are near / An exceptionally agile yoyo
- Officiel : Reach: 20.625 tiles / Speed: 25 / Duration: 24 seconds / Fires homing blood when enemies are near

### Aureate Booster (Accessories.Wings)

Version du code la plus proche : 2022-05 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Mach speed with style! / 10% increased movement speed while wearing the Reaver Armor
- Officiel : Allows flight and slow fall / Max horizontal speed: Great / Max vertical speed: Great / Horizontal acceleration: Good / Vertical acceleration: Good / Hold [up] to boost faster! / Mach speed with style!

### Axe of Purity (Tools)

Version du code la plus proche : 2022-07 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Left click to use as a tool / Right click to cleanse evil
- Officiel : [use] to use as a tool / [open] to cleanse evil

### Ballistic Poison Bomb (Weapons.Rogue)

Version du code la plus proche : 2020-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Throws a sticky bomb that explodes into spikes and poison clouds / Stealth strikes throw three at once
- Officiel : Throws a sticky bomb that explodes into spikes and venomous clouds / Stealth strikes erupt into more clouds with a longer lasting Acid Venom

### Bloodfire Bullet (Ammo)

Version du code la plus proche : 2020-10 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Accelerates your life regeneration on hit / Deals bonus damage based on your current life regeneration
- Officiel : Passes through tiles and accelerates your natural life regen on hit based on how powerful the hit was / Deals bonus damage based on your current natural life regen / This damage caps at 1.1x

### Caustic Staff (Weapons.Summon)

Version du code la plus proche : 2020-09 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Summons a mini dragon to fight for you / The dragon can inflict several debilitating debuffs if you hold a summon weapon or tool
- Officiel : Summons a mini dragon to fight for you / The dragon attacks faster and inflicts more debuffs the more are summoned / Hold Shift for moreAlways inflicts Hellfire and Acid Venom / Inflicts Cursed Inferno with 2 dragons summoned / Inflicts Ichor (debuff) with 3 dragons summoned / Inflicts Marked for Death with 4 dragons summoned

### Celestial Onion (Misc)

Version du code la plus proche : 2019-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Alien power pulses inside its layers / Consuming it does something that cannot be reversed
- Officiel : Permanently increases the number of accessory slots / This item does nothing in Master Mode / Alien power pulses inside its layers

### Chicken Cannon (Weapons.Ranged)

Version du code la plus proche : 2021-09 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Fires chicken flares which create enormous incendiary explosions / Right click to detonate all airborne rockets
- Officiel : Fires chicken flares which create enormous Dragonfire explosions

### Cosmic Immaterializer (Weapons.Summon)

Version du code la plus proche : 2021-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Summons a cosmic energy spiral to fight for you / The orb will fire swarms of homing energy bolts when enemies are detected by it / Requires 10 minion slots to use and there can only be one energy spiral / Without a summoner armor set bonus this minion will deal less damage
- Officiel : Summons a cosmic energy spiral to fight for you / The spiral will fire swarms of homing energy bolts when enemies are detected by it / Struck enemies are cleansed by Miracle Blight / There can only be one energy spiral

### Cosmic Kunai (Weapons.Rogue)

Version du code la plus proche : 2020-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Fires a stream of short-range kunai / Stealth strikes spawn 5 Cosmic Scythes which home and explode
- Officiel : Fires a rapid stream of short-range kunai / Stealth strikes make kunai pierce foes, travel farther, and spawn 9 homing scythes

### Cosmilamp (Weapons.Summon)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Summons a cosmic lantern to fight for you / Takes up 2 minion slots
- Officiel : Summons a cosmic lantern above your head to fight for you

### Craw Carapace (Accessories)

Version du code la plus proche : 2018-12 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : 5% increased damage reduction / Enemies take damage when they touch you
- Officiel : 7% increased damage / Attackers take damage and are inflicted with Crumbling

### Deep Sea Dumbbell (Weapons.Rogue)

Version du code la plus proche : 2019-09 (ressemblance 0.8, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Swings a dumbbell that hits and flings weights with each attack / Right click to flex
- Officiel : Throws a dumbbell that bounces and flings weights with each bounce / Press [open] to flex, increasing the damage of your next attack up to 5 times damage / Flexes can hit enemies directly
- Note de version : [1.5.0.001] Reworked Deep Sea Dumbbell, flex damage no longer boosts stealth strike damage and instead boosts the damage of the next dumbbell throw by 100%, which can stack up to 4 times. Hitting an enemy with a dumbbell no longer permanently decreases enemy defense by 15

### Dynamo Stem Cells (Accessories)

Version du code la plus proche : 2021-01 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : 10% increased movement speed / Ranged weapons have a chance to fire mini swarmers
- Officiel : 10% increased ranged damage and movement speed / Ranged weapons fire a powerful mini swarmer every 3 seconds / Reduces damage taken from Dragonfire

### Earthen Pike (Weapons.Melee)

Version du code la plus proche : 2019-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Crushes enemy defenses / Sprays fossil shards on use
- Officiel : Sprays fossil shards on swing

### Eidolic Wail (Weapons.Magic)

Version du code la plus proche : 2019-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Earrape / Fires a string of bouncing sound waves that become stronger as they travel
- Officiel : Fires a string of bouncing sound waves that slow down on enemy hits / RIP headphone users

### Elysian Wings (Accessories.Wings)

Version du code la plus proche : 2021-01 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Blessed by the Profaned Flame / Temporary immunity to lava and 15% increased movement speed
- Officiel : Allows flight and slow fall / Max horizontal speed: Excellent / Max vertical speed: Excellent / Horizontal acceleration: Excellent / Vertical acceleration: Great / Blessed by the Profaned Flame

### Entropy's Vigil (Weapons.Summon)

Version du code la plus proche : 2020-05 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Summons Calamitas and her brothers to protect you / One minion trio takes two minion slots
- Officiel : Summons a mini Calamitas Clone and her brothers to protect you

### Ethereal Extorter (Accessories)

Version du code la plus proche : 2021-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : rogue projectiles explode into homing souls on death / 10% increased rogue damage and +10 maximum stealth, however, life regen is reduced by 1
- Officiel : Rogue projectiles explode into homing souls on hit / 8% increased rogue damage and +5 maximum stealth

### Ethereal Subjugator (Weapons.Summon)

Version du code la plus proche : 2020-05 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Summons a phantom to protect you / Each phantom takes only half of a minion slot
- Officiel : Summons a phantom to protect you
- Note de version : [2.0.2.001] Reworked Ethereal Subjugator. Minions now take up one minion slot instead of half of a slot, and attack by releasing ghastly particles that home in towards the closest enemy after a short time. Buffed damage from 45 to 200.

### Exodus Wings (Accessories.Wings)

Version du code la plus proche : 2020-09 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Pulsing with an alien heartbeat / 5% increased rogue damage and critical strike chance while wearing the Empyrean Armor
- Officiel : Allows flight and slow fall / Max horizontal speed: Great / Max vertical speed: Excellent / Horizontal acceleration: Great / Vertical acceleration: Great / Grants permanent Empyrean Wrath while wearing the Empyrean armor / Pulsing with an alien heartbeat

### Fault Line (Weapons.Melee)

Version du code la plus proche : 2020-04 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Explodes on enemy hits / A very agile yoyo
- Officiel : Reach: 30 tiles / Speed: 40 / Infinite throw duration / Explodes on enemy hits

### Flowers of Mortality (Weapons.Summon)

Version du code la plus proche : 2021-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Summons five rainbow flowers over your head / The combined flowers consume two and a half minion slots
- Officiel : Summons five rainbow flowers over your head

### Frost Barrier (Accessories)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : You will freeze enemies near you when you are struck / You are immune to the chilled debuff
- Officiel : You will freeze nearby enemies when you are struck / Grants immunity to Chilled and Frostburn

### Frostcrush Valari (Weapons.Rogue)

Version du code la plus proche : 2020-06 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Fires a long ranged boomerang that explodes into icicles on hit / Stealth strikes throw three short ranged boomerangs along with a spread of icicles
- Officiel : Fires a long ranged frosty boomerang that chips off homing icicles on hit / Stealth strikes throw a boomerang which follows the cursor

### Frosty Flare (Weapons.Rogue)

Version du code la plus proche : 2020-06 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Do not insert in flare gun / Sticks to enemies / Generates a localized hailstorm / Stealth strikes trail snowflakes and summon phantom copies instead of ice shards
- Officiel : Throw a flare that sticks to enemies and rains a hailstorm on them / Stealth strikes summon phantom copies instead of ice shards / Do not insert in flare gun

### Galvanizing Glaive (Weapons.DraedonsArsenal)

Version du code la plus proche : 2022-04 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Its use as a tool is to quickly separate a single object into two / That is also its use as a weapon / Swings a spear which envelops struck foes in an energy field / When done swinging, the spear discharges an extra pulse of energy
- Officiel : Swings a spear which envelops struck foes in an energy field / When finished, the spear discharges an extra pulse of energy / A tool which quickly and neatly cleaves one object into two. / Its efficiency and reliability as a weapon is unmistakable.

### Giant Pearl (Accessories)

Version du code la plus proche : 2020-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : You have a light aura around you / Enemies within the aura are slowed down / Does not work while a boss is alive / Provides a small amount of light in the abyss
- Officiel : You have a light aura around you / Enemies within the aura are slowed down / Increases visibility range in the abyss

### Glove of Recklessness (Accessories)

Version du code la plus proche : 2021-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Increases rogue attack speed by 12% but decreases damage by 10% / Adds inaccuracy to rogue weapons
- Officiel : Increases rogue attack speed by 15%, but decreases damage by 10% and crit by 5% / 15% increased stealth regeneration / Makes rogue weapons slightly inaccurate

### God's Paranoia (Weapons.Rogue)

Nom actuel : Gods' Paranoia. Version du code la plus proche : 2020-06 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Shoots a speedy homing spiky ball. Stacks up to 10. / Attaches to enemies and summons a localized storm of god slayer kunai / Stealth strikes home in faster and summon kunai at a faster rate / Right click to delete all existing spiky balls
- Officiel : Hurls homing spiky balls which attach to enemies / Attached spiky balls summon a localized storm of god slayer kunai on each hit / Stealth strikes home in faster and hit at a faster rate
- Note de version : [2.0.3.005] Renamed God's Paranoia to Gods' Paranoia.

### Grax (Tools)

Version du code la plus proche : 2021-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Hitting an enemy will greatly boost your defense, melee damage, and melee crit for a short time / Right click to use without hammering down walls or chopping down trees
- Officiel : Hitting an enemy will increases your damage by 15% and defense by 15 for 15 seconds / Press [open] to use without hammering down walls or chopping down trees

### Hadal Mantle (Accessories.Wings)

Version du code la plus proche : 2022-05 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Fueled by the fury of the depths / 5% increased damage while wearing the Hydrothermic Armor
- Officiel : Allows flight and slow fall / Max horizontal speed: Great / Max vertical speed: Good / Horizontal acceleration: Great / Vertical acceleration: Good / Fueled by the fury of the depths

### Harpy Ring (Accessories)

Version du code la plus proche : 2022-03 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : 10% increased movement speed / Boosts your maximum flight time by 20%
- Officiel : 20% increased movement speed and flight time

### Herring Staff (Weapons.Summon)

Version du code la plus proche : 2020-05 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Summons a Herring to fight for you / Each herring takes only half of a minion slot
- Officiel : Summons a school of herrings to fight for you
- Note de version : [2.0.3.001] Reworked Herring Staff, it now summons a shoal of 3 herrings which consumes a single minion slot instead of summoning individual herrings which take up half a minion slot each. The 3 herrings are purely visual, and collision is dealt by an invisible projectile

### Hydraulic Volt Crasher (Weapons.DraedonsArsenal)

Version du code la plus proche : 2020-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Good for both stamping metal plates and instantly fusing them, as well as crushing enemies. / An electrically charged jackhammer which shocks all nearby foes on hit
- Officiel : An electrically charged jackhammer which shocks all nearby foes on hit / Press [open] to use without hammering down walls / A device made to electrically stamp metal plates, instantly fusing them. / Naturally, it can also function as a devastating weapon.

### Infernal Kris (Weapons.Rogue)

Version du code la plus proche : 2020-01 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Throws a burning dagger that starts spinning after travelling a short distance, inflicting additional damage while spinning / Stealth strikes cause the dagger to be engulfed in flames, exploding on contact with walls and enemies
- Officiel : Rapidly throws burning daggers / Stealth strikes engulf the dagger in flames, exploding when hitting walls and enemies

### Kylie (Weapons.Rogue)

Version du code la plus proche : 2019-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Stealth strikes throws three short ranged kylies instead of a single long range one / Also known as Dowak
- Officiel : Stealth strikes throw a kylie that weakly seeks out your cursor / Also known as Dowak

### Leviathan Teeth (Weapons.Rogue)

Version du code la plus proche : 2020-01 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Rapidly throws a variety of poisonous fangs that stick to enemies / Stealth strikes cause 3 very fast teeth to be thrown, ignoring gravity and inflicting extreme knockback
- Officiel : Rapidly throws a spread of teeth that stick to enemies / After sticking to an enemy, the teeth will gouge through them and shoot out the other side / Stealth strikes create a clamping jaw of teeth to maul your foes
- Note de version : [2.1.0] Reworked Leviathan Teeth, it no longer throws teeth that stick to enemies and inflict a stacking Acid Venom, and stealth strikes no longer throw more teeth that move faster and spawn water blasts on hit. It now throws a spread of 2 teeth that stick to enemies 

### Lionfish (Weapons.Rogue)

Version du code la plus proche : 2020-05 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Sticks to enemies and injects a potent toxin / Stealth strikes are trailed by homing urchin spikes
- Officiel : Sticks to enemies and injects a potent Acid Venom / The Acid Venom can stack when more lionfish are stuck in an enemy / Stealth strikes spawn homing urchin spikes as they travel

### Lunic Eye (Weapons.Typeless)

Version du code la plus proche : 2019-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Fires lunic beams that reduce enemy protection / This weapon scales with all your damage stats at once
- Officiel : Fires seeking energy bolts that cause enemies to become Marked for Death / This weapon scales with the average of all your damage stats

### Martian Distress Beacon (SummonItems)

Nom actuel : Martian Distress Remote. Version du code la plus proche : 2022-11 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Summons the Martian Madness / Not consumable
- Officiel : Summons the Martian Madness

### Megalodon (Weapons.Ranged)

Version du code la plus proche : 2022-03 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : 50% chance to not consume ammo / Fires streams of water every other shot
- Officiel : Loads a 30 round magazine of bullets and pressurized water jets that are rapidly unleashed / After firing, the gun unleashes a powerful homing rocket before reloading / Rocket damage is based on how many shots hit / 50% chance to save ammo
- Note de version : [2.2.0] Reworked Megalodon, it no longer rapidly fires bullets and water jets alongside a short-range water ring. It now loads a magazine of 30 shots, alternating between firing bullets and water jets which inflict Riptide for 1 second. After firing the entire magazin

### Mounted Scanner (Weapons.DraedonsArsenal)

Version du code la plus proche : 2022-04 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Laser technology used in this case for both targeting and defense / Summons a powerful weapon above your head that fires lasers at nearby enemies
- Officiel : Summons a powerful scanner module above your head that fires lasers at nearby enemies

### Nanotech (Accessories)

Version du code la plus proche : 2020-10 (ressemblance 0.96, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : rogue projectiles create nanoblades as they travel / Stealth strikes summon nanobeams and sparks on enemy hits / Stealth strikes have +20 armor penetration, deal 5% more damage, and heal for 1 HP / 12% increased rogue damage and 15% increased rogue velocity / Whenever you crit an enemy with a rogue weapon your rogue damage increases / This effect can stack up to 150 times / Max rogue damage boost is 15%
- Officiel : Rogue projectiles create nanoblades as they travel / Stealth strikes summon nanobeams and sparks on enemy hits / Stealth strikes have lifesteal, +15 armor penetration, deal 5% more damage / 15% increased rogue damage and 15% increased rogue velocity / Stealth strikes grant lifesteal and 10% critical strike chance to non-stealth strikes for 10 seconds
- Note de version : [2.0.1.002] Reworked Raider's Talisman, it no longer grants a stacking rogue damage boost from landing critical strikes. It now increases non-stealth strike rogue critical strike chance by 12% after performing a stealth strike. This effect decays over 5 seconds. This effe

### Nuclear Rod (Accessories)

Nom actuel : Nuclear Fuel Rod. Version du code la plus proche : 2020-04 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Minions release an irradiated aura on enemy hits / +1 max minion
- Officiel : Increases your max number of minions by 1 / Minions release an Irradiated aura on enemy hits / Grants immunity to Irradiated
- Note de version : [2.0.3.001] Renamed Nuclear Rod to Nuclear Fuel Rod.

### Old Die (Accessories)

Version du code la plus proche : 2020-03 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Lucky for you, the curse doesn't affect you. Mostly. / Increases the randomness of attack damage
- Officiel : Increases luck by 20% / Lucky for you, the curse doesn't affect you. Mostly
- Note de version : [2.0.0.001] Reworked Old Die, it no longer increases the damage deviation range, including rolling the deviation twice for rogue damage. It now increases the luck stat by 0.2 while equipped.

### Phaseslayer (Weapons.DraedonsArsenal)

Version du code la plus proche : 2022-04 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : A rough prototype of the Murasama blade, it is formed entirely from laser energy / Wield a colossal laser blade which is controlled by the cursor / Faster swings deal more damage and release sword beams / When at low charge, the blade is smaller and weaker
- Officiel : Projects a colossal laser blade which is controlled by the cursor / Move the cursor to swing the blade and rip and tear through enemies / Faster swings deal more damage and release additional sword beams

### Photosynthesis Potion (Potions)

Version du code la plus proche : 2019-06 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : You regen life quickly while not moving, this effect is five times as strong during daytime / Dropped hearts heal more HP
- Officiel : Dropped hearts heal 5 more HP

### Prismatic Breaker (Weapons.Melee)

Version du code la plus proche : 2020-06 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Seems to belong to a certain magical girl. Radiates with intense cosmic energy. / Fire to charge for a powerful rainbow laser / Right click to instead swing the sword and fire rainbow colored waves / The sword is boosted by both melee and ranged damage
- Officiel : Fires several bursts of magic stars as the weapon charges up / After charging up, a massive rainbow laser is unleashed to devastate your foes / This weapon is boosted by both melee and ranged damage / Seems to belong to a certain magical girl. Radiates with intense cosmic energy
- Note de version : [2.1.0] Reworked Prismatic Breaker, it no longer fires a small laser beam to the cursor on left-click, or swings to fire wave projectiles on right-click. It now charges up when holding left-click, firing bursts of stars from the tip of the sword. These bursts of stars

### Pristine Annihilation (Weapons.Ranged)

Nom actuel : Pristine Fury. Version du code la plus proche : 2021-05 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Fires an intense helix of flames that explode into a column of fire / Right click to fire a short ranged cloud of lingering flames
- Officiel : Fires an intense helix of Holy Flames that explode into a column of smoke / [open] to disperse a limited supply of flammable crystalline powder that restores over time / Powder can be ignited, causing it to explode for extra damage / Both helix flames and other nearby explosions can ignite the powder / Uses gel for ammo / Tales of legends past radiate around you... / Hold Shift to listen closer'I and many others lost everything in this world's darkest hour. / I saw a beast descend from the heavens. A hulking cosmic monstrosity that heralded countless angels of death. / My family was torn from me, my people forsake me. Even our supposed protectors did nothing to aid us. / And thus I swore to those I lost that I would do what is right, that my blaze's light would dwarf even that of Zeratros' Sun; / That one day, this cruel world would know nothing less than My perfect fury.

### Pulse Dragon (Weapons.DraedonsArsenal)

Version du code la plus proche : 2022-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Heavy duty flails, each containing a powerful generator which is activated upon launch / Throws two dragon heads that emit electrical fields
- Officiel : Throws two mechanical dragon heads that emit damaging electrical fields / Yharim specifically requested this weapon to be modelled after the Dragons of old. / How trite.

### Rare Elemental in a Bottle (Accessories)

Nom actuel : Oasis Elemental in a Bottle. Version du code la plus proche : 2019-04 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Summons a sand elemental to heal you / ;D
- Officiel : Summons an oasis elemental to assist you / The elemental fires healing orbs that restore 10 health while you are damaged / The elemental has reduced opacity if visibility is disabled
- Note de version : [2.2.0] Renamed Rare Elemental in a Bottle to Oasis Elemental in a Bottle.

### Rem's Revenge (Weapons.Melee)

Version du code la plus proche : 2021-10 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Wielded by the most powerful fighter. / Summons blood explosions and lowers enemy defense on hit
- Officiel : Summons blood explosions and inflicts Wither on hit / A deadly morning star befitting its demonic owner

### Riptide (Weapons.Melee)

Version du code la plus proche : 2022-07 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Sprays a spiral of aqua streams in random directions / A very agile yoyo
- Officiel : Reach: 18 tiles / Speed: 25 / Duration: 18 seconds / Sprays a spiral of aqua streams

### Rose Stone (Accessories)

Version du code la plus proche : 2021-09 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : One of the ancient relics / Summons a brimstone elemental to fight for you
- Officiel : Summons a Brimstone Elemental to fight for you / The elemental has reduced opacity if visibility is disabled

### Rubico Prime (Weapons.Ranged)

Version du code la plus proche : 2022-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Semi-automatic sniper that fires in 5 second bursts / Fires impact rounds that have an increased crit multiplier
- Officiel : A powerful sniper rifle that must reload after 5 shots / Fires impact rounds that deal increased critical damage

### Rune of Kos (SummonItems)

Nom actuel : Mark of Providence. Version du code la plus proche : 2022-07 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : A relic of the profaned flame / Contains the power hunted relentlessly by the sentinels of the cosmic devourer / When used in certain areas of the world, it will unleash them / Not consumable
- Officiel : An ornate sigil of the Profaned Flame / Hatred yet burns upon its brow / Such power attracts otherworldly beings / Not consumable
- Note de version : [2.1.0] Renamed Rune of Kos to Mark of Providence.

### Scourge of the Seas (Weapons.Rogue)

Version du code la plus proche : 2019-11 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Snaps apart into a venomous cloud upon striking an enemy / Stealth strikes are coated with vile toxins, afflicting enemies with a powerful debuff
- Officiel : Snaps apart into Irradiated clouds upon striking an enemy / Stealth strikes cause a longer Irradiated and are additionally coated with Acid Venom

### Seeking Scorcher (Weapons.Melee)

Version du code la plus proche : 2020-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : May your enemies burn in hell for the sins they have committed / Throws a holy boomerang that seeks out up to three enemies before returning to the player
- Officiel : Throws a massive crystalline axe which bounces to three additional targets after hitting / Crystallizes in place if all bounces have not been exhausted / Crystallized hatchets can be shattered by another axe / Doing so empowers the hatchet based on the amount shattered but prevents it from crystallizing / May your enemies burn in hell for the sins they have committed
- Note de version : [2.1.0] Reworked Seeking Scorcher, it now throws a crystalline hatchet which can bounce between up to four enemies, losing 20% damage per hit. If the hatchet does not reach its maximum bounces, it crystallizes in place. Crystallized hatchets are shattered when hit by 

### Septic Skewer (Weapons.Ranged)

Version du code la plus proche : 2019-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Launches a spiky harpoon infested with toxins / Releases bacteria when returning to the player
- Officiel : Launches a vile chained harpoon infested with toxins / Harpoons stick into enemies and inflict Sulphuric Poisoning for a long time / After being impaled for one second, harpoons rip themselves back out and spray damaging gores / Press [open] to manually rip out all harpoons, dealing bonus damage / Enemies close to death are dragged back with returning harpoons and executed when they reach you / Executions heal you for 25 health and launch an additional harpoon automatically
- Note de version : [2.1.0] Reworked Septic Skewer, it no longer fires harpoons that spawn bacteria projectiles when returning to the player. It now fires harpoons that stick into enemies. After being stuck for 1 second, harpoons return to the player, releasing gore projectiles from the 

### Shimmerspark (Weapons.Melee)

Version du code la plus proche : 2020-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Fires stars when enemies are near / A very agile yoyo
- Officiel : Reach: 28 tiles / Speed: 36 / Infinite throw duration / Fires stars when enemies are near

### Shinobi Blade (Weapons.Rogue)

Version du code la plus proche : 2020-01 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Throws a fast blade that spawns healing orbs when it kills an enemy / Stealth strikes cause 3 blades to be thrown at once
- Officiel : Throws a fast blade that spawns healing orbs when it kills an enemy / Stealth strikes repeatedly stab the struck enemy from random directions

### Silva Wings (Accessories.Wings)

Version du code la plus proche : 2020-04 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : The purest of nature / The Silva revive heals you to half health while wearing the Silva armor
- Officiel : Allows flight and slow fall / Max horizontal speed: Excellent / Max vertical speed: Excellent / Horizontal acceleration: Excellent / Vertical acceleration: Great / The purest of nature

### Snap Clam (Weapons.Rogue)

Version du code la plus proche : 2020-05 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Can latch on enemies and deal damage over time / Stealth strikes throw five clams at once that cause increased damage over time
- Officiel : Can latch on enemies and inflict Clammed / Stealth strikes throw five clams at once

### Starbuster Core (Accessories)

Version du code la plus proche : 2020-04 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Summons release an astral explosion on enemy hits / +1 max minion
- Officiel : Increases your max number of minions by 1 / Summons release an astral explosion on enemy hits

### Starlight Wings (Accessories.Wings)

Version du code la plus proche : 2019-11 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Wings of the Nightingale / 5% increased damage and critical strike chance while wearing the Daedalus Armor
- Officiel : Allows flight and slow fall / Max horizontal speed: Good / Max vertical speed: Average / Horizontal acceleration: Good / Vertical acceleration: Average / Wings of the Nightingale

### Sun God Staff (Weapons.Summon)

Nom actuel : Vengeful Sun Staff. Version du code la plus proche : 2021-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Summons a solar god spirit to protect you / There can only be one spirit
- Officiel : Summons a sun spirit to protect you / Repeated summons eclipse the spirit and increase its power
- Note de version : [2.0.3.003] Renamed Sun God Staff to Vengeful Sun Staff.
- Note de version : [2.1.0] Reworked Vengeful Sun Staff, it no longer fires instantaneous solar beams. It now fires medium speed solar beams that split into 2 homing light orbs on hit. Repeated summons with the weapon will instead increase the minion's damage and attack speed by 25% per 

### Sun Spirit Staff (Weapons.Summon)

Version du code la plus proche : 2021-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Summons a solar spirit to protect you / There can only be one spirit
- Officiel : Summons a solar spirit to protect you / Repeated summons increase its power
- Note de version : [2.1.0] Reworked Sun Spirit Staff, it no longer fires instantaneous solar beams. It now fires medium speed solar beams. Repeated summons with the weapon will instead increase the minion's attack speed by 100% per minion slot used. Buffed damage from 12 to 25.

### Tactician's Trump Card (Weapons.Magic)

Version du code la plus proche : 2022-01 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Faint memories of a Princess from the future cross your mind... / Fires a sword beam that electrifies enemies on hit
- Officiel : Fires an Electrified sword beam / Faint memories of a Princess from the future cross your mind...

### Tarragon Wings (Accessories.Wings)

Version du code la plus proche : 2019-11 (ressemblance 0.91, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Born of the jungle / +15 defense and +2 life regen while wearing the Uelibloom Armor
- Officiel : Allows flight and slow fall / Max horizontal speed: Excellent / Max vertical speed: Excellent / Horizontal acceleration: Great / Vertical acceleration: Great / Born of the jungle

### Taser (Weapons.DraedonsArsenal)

Nom actuel : P-TSL: Short Circuit. Version du code la plus proche : 2020-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : A slow, simple electric weapon, meant only for low ranking guards. / Shoots a hook that attaches to enemies and electrocutes them before returning
- Officiel : Press Left Click to rapidly fire stunning bolts with high knockback / Press Right Click to shoot a hook that attaches to enemies and rapidly electrocutes them / After electrocuting an enemy enough, the hook will create a shock explosion before returning to you / Successful hook attacks have a short cooldown that is applied unless the hook kills an enemy / Ignores 10 points of enemy Defense
- Note de version : [2.1.0] Renamed Taser to P-TSL: Short Circuit.
- Note de version : [2.1.0] Reworked P-TSL: Short Circuit, left-click no longer fires a hook that latches onto enemies briefly before returning. Left-click now rapidly fires electric shots with slight inaccuracy which pierce once. Right-click now launches the hook, which deals 120% base 

### Terra Ray (Weapons.Magic)

Nom actuel : Photosynthesis. Version du code la plus proche : 2021-07 (ressemblance 0.95, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Casts an energy ray that splits into energy on enemies hits / More energy is created the farther along the ray the enemy hit is
- Officiel : Casts a brilliant ray that dissolves into energy on enemy hits
- Note de version : [2.0.4.001] Renamed Terra Ray to Photosynthesis.

### The Camper (Accessories)

Version du code la plus proche : 2021-05 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : In rest may we find victory. / You deal 90% less damage unless stationary / Standing still grants buff(s) dependent on what weapon you're holding / Standing still provides a damaging aura around you / While moving, you regenerate health as if standing still / Provides a small amount of light in the Abyss
- Officiel : You deal 50% less damage unless stationary / Standing still grants buffs depending on held item, and creates a damaging aura around you / While moving, you regenerate health as if standing still / Increases visibility range in the abyss when standing still / In rest may we find victory

### The God's Gambit (Weapons.Melee)

Version du code la plus proche : 2020-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Fires a stream of slime when enemies are near / A very agile yoyo
- Officiel : Reach: 25 tiles / Speed: 32 / Infinite throw duration / Fires a stream of damaging slime when enemies are near

### Throwing Brick (Weapons.Rogue)

Version du code la plus proche : 2020-06 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Prove its resistance by throwing it upwards and catching it with your face / Throws a brick that shatters if stealth is full.
- Officiel : Stealth strikes cause the brick to shatter / Prove its resistance by throwing it upwards and catching it with your face

### Tome of Fates (Weapons.Magic)

Nom actuel : Apathanull. Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Casts cosmic tentacles to spear your enemies / Can randomly fire a brimstone tentacle for immense damage
- Officiel : Casts a spread of dark cosmic tentacles to skewer your enemies / Tentacles will aim at your cursor and lash out three times before disappearing
- Note de version : [2.1.0] Renamed Tome of Fates to Apathanull.
- Note de version : [2.1.0] Reworked Apathanull, it no longer fires tentacles from the player with a random chance to fire a brimstone tentacle that deals increased damage. It now fires cosmic tentacles which spawn near and target the cursor. Tentacles attack the cursor 3 times before di

### True Caustic Edge (Weapons.Melee)

Nom actuel : Caustic Edge. Version du code la plus proche : 2022-01 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Fires a bouncing caustic beam / Inflicts poison and venom
- Officiel : Fires a bouncing caustic beam / Inflicts Acid Venom and slows down on enemy hits
- Note de version : [2.0.3.001] Renamed True Caustic Edge to Caustic Edge.

### Ultima (Weapons.Ranged)

Version du code la plus proche : 2020-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Casts a continuous stream of plasma bolts / Over time the bolts are replaced with powerful lasers / Bolts power up into solid beams as you continue shooting / 90% chance to not consume ammo
- Officiel : Casts a continuous stream of plasma bolts / Over time bolts power up into solid beams as you continue shooting / 66% chance to save ammo

### Ultra Liquidator (Weapons.Magic)

Version du code la plus proche : 2022-01 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Summons liquidation blades that summon more blades on enemy hits / The blades inflict ichor, cursed inferno and brimstone flames
- Officiel : Summons liquidation blades that summon more blades on enemy hits

### Vehemence (Weapons.Magic)

Version du code la plus proche : 2021-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Casts intense bolts of hellish power that explode into skulls / Direct hits inflict Demon Flames for an extended period of time
- Officiel : Casts intense bolts of hellish power that explode into skulls

### Verium Bullet (Ammo)

Nom actuel : Verium Bolt. Version du code la plus proche : 2020-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : There is no escape! / Homes in after striking an enemy
- Officiel : Marks enemies on hit, dealing extra damage after a short time / This damage stacks based on how many bolts hit the enemy / There is no escape...
- Note de version : [2.0.4.001] Renamed Verium Bullet to Verium Bolt.
- Note de version : [2.0.4.001] Reworked Verium Bolt, it is no longer a bullet that pierces once and homes in after piercing. It is now an arrow that marks enemies on hit with sparkle effects. After 1.5 seconds, the enemy receives extra flat damage. The flat damage dealt scales with how many

### Wither Blossoms Staff (Weapons.Summon)

Version du code la plus proche : 2021-07 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Summons four decaying flowers over your head / The combined flowers consume two minion slots
- Officiel : Summons four decaying Plague flowers over your head

### Yharim's Gift (Accessories)

Version du code la plus proche : 2022-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : The power of a god pulses from within this artifact / Flaming meteors rain down while invincibility is active / Exploding dragon dust is left behind as you move / Damage and movement speed increased by 15%
- Officiel : Grants immunity to knockback / Increases damage by 10% and movement speed by 20% / All attacks inflict Auric Rebuke / Flaming meteors rain down while invincible / The power to kill a god pulses from within this artifact

### Yharon's Kindle Staff (Weapons.Summon)

Version du code la plus proche : 2022-07 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Summons a fiery draconid to fight for you / Requires 5 minion slots to use
- Officiel : Summons a fiery draconid to fight for you

### Profaned Soul Crystal (Accessories)

Version du code la plus proche : 2022-03 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.54

- Fandom : Transforms you into an emissary of the profaned goddess / Requires 10 minion slots to use in order to grant the following effects / All non-summon weapons are converted into powerful summon variations / Falling below 50% life will empower these attacks / Transforms Melee attacks into a barrage of spears / Transforms Magic attacks into a powerful splitting fireball / Transforms Ranged attacks into a flurry of fireballs and meteors / Transforms rogue attacks into a deadly crystalline spiral / Summons and empowers the profaned babs to fight alongside you / You are no longer affected by burn out when hit / Provides buffs depending on the time of day / Thinking back, it was a boring life / And so we burn it all in the name of purity
- Officiel : Transforms you into an emissary of the profaned goddess / Provides passive buffs depending on the time of day / Greatly boosts the power of the Guardian's relics / Grants additional effects if you have 10 unused minion slots / Summons your own personal Guardians to fight alongside you / All non-summon attacks are transformed, and are empowered if no minions are active / Melee attacks become a barrage of spears / Ranged attacks become a flurry of fireballs and meteors / Magic attacks become a giant splitting fireball, costing 100 mana / Whips become overtaken by profaned crystal / Rogue attacks become a deadly crystalline spiral / Thinking back, it was a boring life / And so we burn it all in the name of purity
- Note de version : [2.0.3.005] Reworked Profaned Soul Crystal. It now has an empowered state that is triggered by having no other minions summoned, and is no longer nerfed when using Endogenesis or Temporal Umbrella. Its enraged state is now triggered at nighttime instead of when below 50% 

### Occult Skull Crown (Accessories)

Version du code la plus proche : 2022-02 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.56

- Fandom : Only a fool would ever wear this... / You constantly gain rage over time and rage does not fade away when out of combat / Converts certain debuffs into buffs and extends their durations / Debuffs affected: Darkness, Blackout, Confused, Slow, Weak, Broken Armor, / Armor Crunch, War Cleave, Chilled, Ichor and Obstructed / Getting hit causes you to only lose half of your max adrenaline rather than all of it / Increases your max movement speed and acceleration by 5% / Can also be worn as a helmet / Revengeance item
- Officiel : You constantly gain rage over time, and rage does not fade when out of combat / Converts certain debuffs into buffs and extends their durations / This also removes immunities to these debuffs if you have them / Adrenaline charges 20% faster / Increases your max movement speed and acceleration by 5% / Only a fool would ever wear this... / Revengeance

### Auralis (Weapons.Ranged)

Version du code la plus proche : 2020-10 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.57

- Fandom : Fires a high speed glowing bullet that inflicts debilitating debuffs / Right click to zoom out / Standing still provides increasing damage bonuses up to 25% / Standing still for 5 or more seconds while using the scope ability will summon an aurora / The aurora reduces the damage of the next projectile hit by a flat 100 / This effect lasts up to 20 seconds and has a 30 second cooldown / 50% chance to not consume bullets
- Officiel : Fires a high speed glowing bullet that causes enemies to become Marked for Death / Press [open] to zoom out / Standing still provides increasing damage bonuses up to 25% / Standing still for 5 or more seconds while using the scope ability will summon an aurora / The aurora reduces the damage of the next projectile hit by 100 / This effect lasts up to 20 seconds and has a 30 second cooldown

### Roxcalibur (Weapons.Melee)

Version du code la plus proche : 2019-09 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.6

- Fandom : You couldn’t get it out of the rock, so you just brought the rock instead / A hellish entity of flesh holds the key to this weapon’s power / Left click to shoot several rock shards / Right click to dive downwards and bounce off enemies / Diving into blocks creates a shockwave
- Officiel : Hold [use] to charge up a strike / Release with the cursor above you to swing normally, releasing rock shards / Release with the cursor below you to dive downwards and bounce off enemies / Diving into blocks creates a shockwave / You couldn't get it out of the rock, so you just brought the rock instead
- Note de version : [2.0.3.007] Reworked Roxcalibur, it no longer performs a normal broadsword swing while firing rock shards on left click, or performs a slam on right click. It now has a custom swing animation, and holding down left click charges up an overhead swing of the sword. Releasin

### Abombination (SummonItems)

Version du code la plus proche : 2022-02 (ressemblance 0.97, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.67

- Fandom : Calls in the airborne jungle abomination / Summons the Plaguebringer Goliath when used in the jungle / Not consumable
- Officiel : Summons the Plaguebringer Goliath when used in the Jungle / Enrages outside the Jungle / Not consumable / Radio capable of emitting a frequency recognized by the Plague nanovirus.

### Altar of the Accursed (Placeables)

Version du code la plus proche : 2021-10 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.67

- Fandom : Offer Ashes of Calamity at this altar to summon the Witch / Doing so will create a square arena of blocks, with the altar at its center / During the battle, heart pickups only heal for half as much
- Officiel : Offer Ashes of Calamity at this altar to summon the Witch / Doing so will create a square arena of blocks, with the altar at its center / The Witch enrages while you are outside of the arena / Hovering over the altar while using ruler measurements will display a temporary arena box / Used for special crafting

### Angelic Shotgun (Weapons.Ranged)

Version du code la plus proche : 2021-03 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.67

- Fandom : Each shot casts a radiant beam of holy light from the sky / Converts musket balls into illuminated bullets / Fighting ‘til the war’s won
- Officiel : Each shot casts a radiant beam of Holy Flames from the sky / Musket balls convert into hallow-point rounds which also empower the beam / Fighting 'til the war's won

### Baguette (Potions)

Version du code la plus proche : 2020-04 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.67

- Fandom : Minor improvements to all stats / Boosts the effects of Red Wine / je suis Monte
- Officiel : Minor improvements to all stats / Goes well with Red Wine (debuff) / je suis Monte

### Borealis Bomber (Weapons.Summon)

Version du code la plus proche : 2020-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.67

- Fandom : Summons aureus bombers to fight for you / Aureus bombers explode on enemy impact / Does not consume minion slots
- Officiel : Summons aureus bombers to chase down your enemies / Aureus bombers explode violently on enemy impact, spreading the Astral Infection (debuff) / Does not consume minion slots

### Charred Idol (SummonItems)

Version du code la plus proche : 2022-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.67

- Fandom : Use at your own risk / Summons the Brimstone Elemental when used in the brimstone crags / Not consumable
- Officiel : Summons the Brimstone Elemental when used in the Brimstone Crags / Enrages outside the Brimstone Crags / Not consumable / Anything under her gaze shall be scorched beyond recognition.

### Condemnation (Weapons.Ranged)

Version du code la plus proche : 2021-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.67

- Fandom : Fires powerful scarlet bolts suffused with hateful magics / Hold left click to load up to eight bolts for powerful burst fire / Hold right click to use the repeater full auto
- Officiel : Fires powerful scarlet bolts suffused with hateful magics / Hold [use] to load up to nine bolts for powerful burst fire

### Dragonblood Disgorger (Weapons.Summon)

Version du code la plus proche : 2020-04 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.67

- Fandom : Summons a skeletal dragon and her two children / Requires 6 minion slots to be summoned / There can only be one family
- Officiel : Summons a skeletal dragon and her two children / Baby spits burst into blood orbs upon hitting an enemy / Picking up these blood orbs reduces the duration of Potion Sickness / There can only be one family at a time

### Fantasy Talisman (Weapons.Rogue)

Version du code la plus proche : 2019-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.67

- Fandom : Fires high velocity talismans that ignore gravity / Talismans attach to enemies, causing them to release lost souls / Stealth strikes release more souls and leave behind souls as they travel
- Officiel : Fires high velocity talismans that ignore gravity / Talismans attach to enemies, releasing vengeful souls that chase them down / Stealth strikes release more souls and erupt into lost souls on death / Good thing I didn't bet against you!

### Heavenfallen Stardisk (Weapons.Rogue)

Version du code la plus proche : 2020-06 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.67

- Fandom : Throws a stardisk upwards which then launches itself towards your mouse cursor, / explodes into several astral energy bolts if the thrower is moving vertically when throwing it and during its impact / Stealth strikes rain astral energy bolts from the sky
- Officiel : Throws a stardisk upwards which then launches itself towards your mouse cursor / Explodes into a star of astral bolts which spread the Astral Infection (debuff) / Stealth strikes rain astral energy bolts from the sky

### Hermit's Box of One Hundred Medicines (Pets)

Version du code la plus proche : 2022-07 (ressemblance 0.99, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.67

- Fandom : As the ice melts in the springs / And waves wash the old moss’ hair… / Thank you, Goodbye. / Summons the Third Sage / Use the item with right click in the hotbar to toggle the Third Sage's blessing. / With the blessing, the player will spawn with full health rather than half.
- Officiel : Summons the Third Sage / As the ice melts in the springs / And waves wash the old moss' hair... / Thank you, Goodbye

### Infinity (Weapons.Ranged)

Version du code la plus proche : 2022-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.67

- Fandom : Fires a barrage of energy bolts that split and bounce / Right click to fire a barrage of normal bullets / They say infinity is neverending, yet you hold it in your hands
- Officiel : Fires a twin stream of energy bolts that split / Ignores 20 points of enemy Defense / 80% chance to save ammo / They say infinity is neverending, yet you hold it in your hands

### Necroplasmic Beacon (SummonItems)

Version du code la plus proche : 2022-02 (ressemblance 0.97, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.67

- Fandom : It's spooky / Summons Polterghast when used in the dungeon / Consumable
- Officiel : Summons Polterghast when used in the Dungeon / Despawns on the surface / Not consumable / The wails of the spirits disturbed something far more terrifying.

### Overloaded Sludge (SummonItems)

Version du code la plus proche : 2021-11 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.67

- Fandom : It looks corrupted / Summons the Slime God / Not consumable
- Officiel : Summons the Slime God / Not consumable / Writhing and bloated with the pus of Gods long dead.

### Profaned Shard (SummonItems)

Version du code la plus proche : 2022-02 (ressemblance 0.93, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.67

- Fandom : A shard of the unholy flame / Summons the Profaned Guardians when used in the Underground during daytime / Not consumable
- Officiel : Summons the Profaned Guardians when used in the Hallow or Underworld / Not consumable / Her Guardians seek the holder of such vast stolen power.

### Recitation of the Beast (Weapons.Magic)

Version du code la plus proche : 2020-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.67

- Fandom : A thousand years sealed in the demon's realm will teach you a thing or two / Summons beast scythes around the player in a small circle, / before firing toward the cursor and home in to nearby enemies
- Officiel : Summons beast scythes around the player in a small circle / Beast scythes fire toward the cursor and home in to nearby enemies

### Refraction Rotor (Weapons.Rogue)

Version du code la plus proche : 2021-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.67

- Fandom : Fires a huge prismatic disk shuriken / The shuriken shatters moments after impact into homing rockets / Stealth strikes shatter into many more rockets
- Officiel : Throws a huge prismatic shuriken / The shuriken shatters moments after impact into homing blades / Stealth strikes throw 3 shurikens that rapidly home on enemies
- Note de version : [2.1.0] Reworked Refraction Rotor, it no longer throws a disk that shatters into homing rockets after hitting an enemy, and stealth strikes no longer cause more rockets to spawn. It now throws a disk that sticks to enemies, continuously damaging them before exploding 

### Rift Reeler (Fishing)

Version du code la plus proche : 2022-07 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.67

- Fandom : Fires three to five lines at once. Can fish in lava. / The battlefield is a scene of constant chaos. / The winner will be the one who controls that chaos, both the pole and the fish.
- Officiel : Fires four lines at once. Bobbers deal damage upon contact with enemies / Allows fishing in lava / The battlefield is a scene of constant chaos. / The winner will be the one who controls that chaos, both the pole and the fish.

### Shredder (Weapons.Ranged)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.67

- Fandom : The myth, the legend, the weapon that drops more frames than any other / Fires a barrage of energy bolts that split and bounce / Right click to fire a barrage of normal bullets
- Officiel : Fires a barrage of energy bolts that split and bounce / The myth, the legend, the weapon that drops more frames than any other

### Spiteful Candle (Placeables)

Version du code la plus proche : 2020-09 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.67

- Fandom : When placed, nearby enemies take 5% more damage. / This extra damage bypasses enemy damage reduction and defense / Its hateful glow flickers with ire
- Officiel : [open] when placed to deal 1.07x damage to all enemies / The extra 0.07x damage bypasses enemy defense and damage reduction / This cannot be stacked with other candle buffs / Its hateful glow flickers with ire

### Stress Pills (Accessories)

Version du code la plus proche : 2020-05 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.67

- Fandom : Boosts your defense by 4 and max movement speed and acceleration by 5% / Receiving a hit causes you to only lose half of your max adrenaline rather than all of it / Revengeance drop
- Officiel : Adrenaline charges 20% faster / Increases your max movement speed and acceleration by 5% / Revengeance

### Terratomere (Weapons.Melee)

Version du code la plus proche : 2021-04 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.67

- Fandom : Linked to the essence of Terraria / Heals the player on true melee hits / Fires a barrage of 4 homing beams that freeze enemies
- Officiel : Heals the player on true melee hits / Fires several piercing energy bolts that rapidly seek targets / On hit, explodes and repeatedly slashes the target / Linked to the essence of Terraria
- Note de version : [2.0.2.001] Reworked Terratomere, it no longer spawns 4 homing beams on swing. It now has a custom swing animation, and fires a single energy bolt that explodes on hit. The blade also shoots a short range slash projectile after every swing. Both the blade and the energy b

### The Sevens Striker (Weapons.Ranged)

Version du code la plus proche : 2022-09 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.67

- Fandom : Left click to consume 1 coin and play a slot machine / Quality of the outcome depends on the coin used / Right click to rapidly fire a barrage of coins / Right click has an 80% chance to not consume coins and will not fire platinum coins / A gun given to a great gunslinger / Forged by the arms of a man given no name
- Officiel : Press [use] to consume 1 coin and play a slot machine / The quality of the outcome depends on the coin used / A gun given to a great gunslinger / Forged by the arms of a man given no name / Hold Shift for more'LET'S GO GAMBLING!

### Vesuvius (Weapons.Magic)

Version du code la plus proche : 2021-05 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.67

- Fandom : Asteroids give the Molten buff on enemy hits / Calls down a swarm of molten asteroids / Right click to fire a spread of molten asteroids from the staff
- Officiel : Calls down a swarm of molten asteroids / Press [open] to fire a spread of molten asteroids from the staff

### Wave Pounder (Weapons.DraedonsArsenal)

Version du code la plus proche : 2022-04 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.67

- Fandom : It utilizes its power to send heavy shockwaves throughout the area, causing agonizing internal damage / Throws a bomb which explodes into a forceful shockwave / Stealth strikes emit absurdly powerful shockwaves
- Officiel : Throws a bomb which explodes into a agonizingly forceful shockwave / Stealth strikes emit absurdly powerful shockwaves

### Yin-Yo (Weapons.Melee)

Version du code la plus proche : 2020-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.67

- Fandom : Fires light or dark shards when enemies are near / Shards fly back and forth / A very agile yoyo
- Officiel : Reach: 25 tiles / Speed: 32 / Infinite throw duration / Fires light or dark shards when enemies are near / Shards fly back and forth

### Ares' Exoskeleton (Weapons.Summon)

Version du code la plus proche : 2022-08 (ressemblance 0.95, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.75

- Fandom : Creates a panel with four slots and four choices above it: Plasma, Tesla, Laser, and Gauss / Clicking one of the choices and then clicking one of the slots allows you to summon a cannon that stays close to you and attacks nearby enemies / Clicking on a slot that's already occupied destroys its associated cannon and clears the slot / Cannons take 3 minion slots each
- Officiel : Creates a panel with four slots and four choices above it: Plasma, Tesla, Laser, and Gauss / Clicking one of the choices and then clicking one of the slots summons a cannon of that type / Clicking on a slot that's already occupied destroys its associated cannon and clears the slot

### Ethereal Talisman (Accessories)

Version du code la plus proche : 2021-10 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.75

- Fandom : 15% increased magic damage, 5% increased magic critical strike chance, and 10% decreased mana usage / +150 max mana / Increases pickup range for mana stars / You automatically use mana potions when needed if visibility is on
- Officiel : Increases maximum mana by 60 and 8% reduced mana cost / 15% increased magic damage and 5% increased magic critical strike chance / Increases pickup range for mana stars / You automatically use mana potions when needed

### Nebulous Core (Accessories)

Version du code la plus proche : 2021-10 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.75

- Fandom : 10% increased damage / Summons floating nebula stars to protect you / You will survive an attack that would have killed you and be healed 100 HP / This effect has a 90 second cooldown
- Officiel : 12% increased damage / Summons floating nebula stars to protect you / Taking a fatal hit revives you to 300 HP / This effect has a 90 second cooldown

### Utensil Poker (Weapons.Rogue)

Version du code la plus proche : 2021-07 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.75

- Fandom : Space chickens, that is all. / Fires random utensils in bursts of three / Grants Well Fed on enemy hits / Stealth strikes replace any utensil with a powerful butcher knife
- Officiel : Fires utensils in bursts of three / Stealth strikes replace any utensil with a powerful butcher knife / Space chickens, that is all

### Venerated Locket (Accessories)

Version du code la plus proche : 2020-01 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.75

- Fandom : 10% increased rogue damage / Using a rogue weapon summons a copy of the projectile that falls from the sky / Stealth strikes cause a circular fan of seeking cosmilite knives to be thrown / You'll never be alone, no matter where you go
- Officiel : 10% increased rogue damage / Rogue projectiles summon a clone from above the cursor / Stealth strikes unleash a ring of seeking cosmilite knives / You'll never be alone, no matter where you go

### Shattered Community (Accessories)

Version du code la plus proche : 2021-12 (ressemblance 0.84, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.8

- Fandom : Ruined by unknowable hatred, it still contains (most of) the power of The Community... / You generate rage over time and rage does not fade away out of combat / Taking damage gives rage, this effect is not hindered by your defensive stats / While Rage Mode is active, taking damage gives only half as much rage / Deal damage with Rage Mode to further empower your wrath / Current level: [LEVEL] (+[BOOST]% Rage Mode damage) / Progress to next level: [PROGRESS RATIO]% / Total Rage Mode damage: [CUMULATIVE DAMAGE DEALT]
- Officiel : Ruined by unknowable hatred, it still contains (most of) the power of The Community... / Increases damage, max health, and movement speed by 10% / Increases defense by 10, and life regen by 1 HP/s / Increases critical strike chance and damage reduction by 5%, and flight time by 20% / You generate rage over time and rage does not fade out of combat / Gain rage after taking damage regardless of defensive stats / While Rage Mode is active, taking damage gives only half as much rage / Deal damage with Rage Mode to further empower your wrath / Current level: [LEVEL] (+[BOOST]% Rage Mode damage) / Progress to next level: [PROGRESS RATIO]% / Total Rage Mode damage: [CUMULATIVE DAMAGE DEALT]

### Ancient Ice Chunk (Weapons.Summon)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Summons an ice clasper to fight for you
- Officiel : Summons an ice clasper to fight for you / The clasper fires ice shards at distant enemies / While you are close to enemies, it will aggressively bash them with frozen fury
- Note de version : [2.0.3.001] Reworked Ancient Ice Chunk, it no longer continuously charges at enemies. It now stays close to the player and fires ice crystals at enemies from a distance. If the player gets close to enemies, the Ice Claspers will charge at them, similarly to old behavior.

### Anti-Cyst Ointment (Tools)

Version du code la plus proche : 2021-09 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Favorite this item to prevent perforator cysts from spawning near you
- Officiel : Toggle this item to prevent hive cysts from spawning near you / Press [open] to toggle / Spawns are currently [STATE]

### Anti-Tumor Ointment (Tools)

Version du code la plus proche : 2021-09 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Favorite this item to prevent hive cysts from spawning near you
- Officiel : Toggle this item to prevent hive tumors from spawning near you / Press [open] to toggle / Spawns are currently [STATE]

### Astral Staff (Weapons.Magic)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Summons a large crystal from the sky that has a large area of effect on impact.
- Officiel : Summons a large crystal from the sky that has a large area of effect on impact and spreads the Astral Infection (debuff)

### Baleful Harvester (Weapons.Melee)

Version du code la plus proche : 2021-06 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Summons flaming pumpkins and skulls that split into homing fire orbs on enemy hits
- Officiel : A flaming scythe which is rapidly swung around three times in succession / Enemy hits summon flaming skulls that split into homing fire orbs

### Ball and Chain (Tools)

Version du code la plus proche : 2020-10 (ressemblance 0.98, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : So heavy… / Favorite this item to disable any dashes granted by equipment.
- Officiel : Toggle this item to disable any dashes granted by equipment / [open] to toggle / Dashes are currently [STATE] / So heavy...

### Black Hawk Remote (Weapons.Summon)

Version du code la plus proche : 2019-11 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Summons a Black Hawk jet to fight for you
- Officiel : Summons a Black Hawk jet to fight for you / Jets will fire bullets from your inventory / 50% chance to save ammo

### Bleach Ball (Tools)

Version du code la plus proche : 2021-09 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Favorite this item to prevent the Aquatic Scourge from naturally spawning near you
- Officiel : Toggle this item to prevent Aquatic Scourge from spawning / Press [open] to toggle / Spawns are currently [STATE]

### Bloody Worm Food (SummonItems)

Version du code la plus proche : 2022-02 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Summons the Perforator Hive when used in the crimson / Not consumable
- Officiel : Summons the Perforator Hive when used in the Crimson / Despawns outside the Crimson / Not consumable / Do not play with your food, they won't play with theirs.

### Blossom Flux (Weapons.Ranged)

Version du code la plus proche : 2021-05 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Fires a stream of leaves / Right click to fire a spore orb that explodes into a cloud of spore gas
- Officiel : Fires a stream of leaves / Press [open] to fire a spore orb that explodes into a cloud of spore gas / Tales of legends past radiate around you... / Hold Shift to listen closer'...I see... So frightened by the Godseeker were my children they chose to defile death for a warden in my image. / I am sorry. I could not gaze unto your hearts. The despair I inflicted upon you... I should have forseen the consequences of taking away your guardian. / But you shan't be neglected any longer. / Heed me, floral beast, on my authority. Ravage this place no longer, stand guard of the home and memories of the people who once lived here. / Channel all the might of this canopy and of the dead within you, so that no Godseeker, lowly or high, will ever trespass into this settlement. / Defend it till the last rays of the sun, so my hero shall yet have a home.

### Ceremonial Urn (SummonItems)

Version du code la plus proche : 2022-07 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Use at the Altar of the Accursed to summon Supreme Calamitas / Not consumable
- Officiel : Use at the Altar of the Accursed to summon the Supreme Witch, Calamitas / Not consumable / The Witch will not allow it to happen once again.

### Conference Call (Weapons.Ranged)

Version du code la plus proche : 2019-06 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : @everyone
- Officiel : Fires a tight spread of bullets that call more bullets from the sky / @everyone

### Cosmic Viper Engine (Weapons.Summon)

Version du code la plus proche : 2020-04 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Summons a cosmic gunship to shoot down your foes
- Officiel : Summons a cosmic gunship to shoot down your foes / Gunships will fire bullets from your inventory / Alternates firing rockets and bullets / 50% chance to save ammo / Rockets do not consume ammo

### Cosmilite Bar (Materials)

Version du code la plus proche : 2018-12 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : A chunk of highly-resistant cosmic steel
- Officiel : A chunk of durable and highly resilient cosmic steel

### Decapodita Sprout (SummonItems)

Version du code la plus proche : 2022-02 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Summons Crabulon when used in the mushroom biome / Not consumable
- Officiel : Summons Crabulon when used in the Underground Mushroom biome / Not consumable / Bristling with the spores of a unique, parasitic strain.

### Devil's Sunrise (Weapons.Melee)

Version du code la plus proche : 2019-09 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Balls? Smalls.
- Officiel : Unleashes a devilish flurry of swipes / Hellish energy charges in the blades over time, which is accelerated by striking foes / Press [open] while swinging the blades with full energy to throw the spinning blades to your cursor / The spinning blades lodge into and repeatedly slice the first foe they meet, stealing their life / Balls? Smalls.
- Note de version : [2.1.0] Reworked Devil's Sunrise, it no longer spawns small swipes towards the cursor along with a cyclone that follows the cursor and disappears after 8 seconds, and its damage no longer scales over time. It now spawns a large barrage of swipes towards the player's c

### Eidolon Tablet (SummonItems)

Version du code la plus proche : 2022-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Summons the Lunatic Cultist / Not consumable
- Officiel : Summons the Lunatic Cultist / Not consumable / An aged artifact venerated by the Dragon Cult.

### Elemental in a Bottle (Accessories)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Summons a sand elemental to fight for you
- Officiel : Summons a sand elemental to fight for you / The elemental has reduced opacity if visibility is disabled

### Flare Bolt (Weapons.Magic)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Casts a slow-moving ball of flame
- Officiel : Casts a ball of flame / Balls of flame collapse into a swirling backdraft which pulls in enemies
- Note de version : [2.1.0] Reworked Flare Bolt, it no longer casts a slow-moving ball of fire which bounces off tiles. It now casts a ball of fire which slows down after reaching the cursor. The ball of fire bursts into a swirling flame vortex that pulls in enemies on hit. Buffed damage

### Garnet (Weapons.Rogue)

Nom actuel : Malachite. Version du code la plus proche : 2021-05 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Throws a stream of kunai that stick to enemies and explode / Right click to throw a single kunai that pierces, after piercing an enemy it emits a massive explosion on the next enemy hit / Stealth strikes fire three kunai that home in, stick to enemies, and explode
- Officiel : Throws a stream of kunai that stick to enemies and explode / Press [open] to throw a single kunai that pierces, after piercing an enemy it emits a massive explosion on the next enemy hit / Stealth strikes fire three kunai that home in, stick to enemies, and explode / Tales of legends past radiate around you... / Hold Shift to listen closer / > STATUS_ / Extensive hull damage. Repairs initiated. / Sensor arrays offline. Repairs scheduled T+2200. / > UPDATE_DIRECTIVE_ / Error 0x00017F80. Device sector unreadable. / Initializing search for external directive. / Searching... (383526 more) / Directive found. / New directive compiled. Rebooting... / > PLAGUE UNIT 194-QB-03d ONLINE. / Directive summary: / Continue propagation of unified frequency band and lead satellite units.

### Gaze of Crysthamyr (Mounts)

Version du code la plus proche : 2019-06 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Summons a shadow dragon
- Officiel : Summons a shadow dragon / Hold W to ascend faster

### Heart of the Elements (Accessories)

Version du code la plus proche : 2021-09 (ressemblance 0.97, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : The bart of the world / Summons all elementals to protect you
- Officiel : Summons all elementals to protect you / The elementals have reduced opacity if visibility is disabled / The heart of the world

### Icebreaker (Weapons.Rogue)

Version du code la plus proche : 2020-05 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Stealth strikes spawn a cosmic explosion and freeze nearby enemies on enemy hits
- Officiel : Throws an icy hammer that magically returns to the player / Stealth strikes spawn a cosmic explosion and freeze nearby enemies on hit

### Lashes of Chaos (Weapons.Magic)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Watch the world burn...
- Officiel : Alternates between exploding hellfireballs and piercing hellblasts / Watch the world burn...

### MOAB (Accessories.Wings)

Version du code la plus proche : 2021-09 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : The mother of all balloons / 10% increased jump speed and allows constant jumping / Grants the player cloud, blizzard, and sandstorm mid-air jumps
- Officiel : Allows flight and slow fall / Max horizontal speed: Average / Max vertical speed: Great / Horizontal acceleration: Average / Vertical acceleration: Good / Hold [up] to boost faster! / 32% increased jump speed and allows auto-jump / Grants the player cloud, blizzard, and sandstorm mid-air jumps / The Mother Of All Balloons

### Mineral Mortar (Weapons.Ranged)

Version du code la plus proche : 2019-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Shoots an onyx bomb that explodes into sand sharks on death
- Officiel : Shoots an onyx bomb that explodes into huge area damage / Uses rockets for ammo
- Note de version : [2.0.3.007] Reworked Mineral Mortar, it no longer fires slow onyx bombs that explode into homing sharks on death. It now charges up to fire a fast onyx bomb which is affected by gravity. These onyx bombs create a large explosion on death, and are affected by all rocket ty

### Minigun (Weapons.Ranged)

Nom actuel : Kingsbane. Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : 80% chance to not consume ammo
- Officiel : After revving up, rapidly fire bullets / When you stop firing you will release a barrage of auric bullets / Auric bullets aggressively home in on enemies, ignore 50 points of defense, and inflict Auric Rebuke / Constantly consumes ammo while in use / 90% chance to save ammo
- Note de version : [2.0.3.007] Renamed Minigun to Kingsbane.
- Note de version : [2.0.3.007] Reworked Kingsbane, it now takes 1.5 seconds to rev up before firing bullets. Releasing left click after the gun is revved up causes a stream of 50 Auric Bullets to be fired before the gun stops firing, causing recoil. Auric Bullets travel extremely fast, home

### Mycoroot (Weapons.Rogue)

Version du code la plus proche : 2020-05 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Fires a stream of short-range fungal roots / Stealth strikes spawn an explosion of fungi spores
- Officiel : Fires a stream of short-range fungal roots / Stealth strikes spawn an explosion of fungi spores / This grants you and nearby players on your team the Mushy buff for 15 seconds

### Nano Purge (Weapons.Magic)

Version du code la plus proche : 2022-07 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Fires a barrage of nano lasers
- Officiel : Fires a barrage of nano lasers / Bounces towards the nearest enemy after hitting a wall

### Odd Mushroom (Potions)

Version du code la plus proche : 2019-04 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Trippy
- Officiel : Causes you to see many fake, vibrant copies of all nearby entities / These visual effects may be nauseating or harmful to those with photosensitivity issues / Trippy

### P90 (Weapons.Ranged)

Version du code la plus proche : 2022-04 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : 50% chance to not consume ammo / It's a bullet hose
- Officiel : Unleashes a stream of armor-piercing rounds at an incredible rate / Armor-piercing rounds ignore 30 defense / 50% chance to save ammo / It's a bullet hose
- Note de version : [2.0.3.007] Reworked P90, it now converts all bullets into special P90 rounds. These rounds travel extremely fast and have an innate 15 armor penetration. Nerfed damage from 8 to 5, but buffed effective use time from 5 to 4.

### Resilient Candle (Placeables)

Version du code la plus proche : 2019-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : When placed, nearby players' defense blocks 5% more damage / Neither rain nor wind can snuff its undying flame
- Officiel : [open] when placed to make defense block 15% more damage / (65% in Classic, 90% in Expert, and 115% in Master) / This cannot be stacked with other candle buffs / Neither rain nor wind can snuff its undying flame

### Sirenproof Earmuffs (Tools)

Version du code la plus proche : 2021-09 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Favorite this item to prevent Anahita from spawning near you
- Officiel : Toggle this item to prevent Anahita from spawning / Press [open] to toggle / Spawns are currently [STATE]

### Snowstorm Staff (Weapons.Magic)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Fires a snowflake that follows the mouse cursor
- Officiel : Fires snowflakes that follows the mouse cursor / Snowflakes split into ice stars on enemy hits

### Spadefish (Fishing)

Version du code la plus proche : 2019-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : How can a fish be used to dig through the ground? / Some questions are best left unanswered.
- Officiel : Throws a spadefish at enemies / Stealth Strikes cause you to spin the spade before throwing, damaging nearby enemies / Shift-[open] this item in your inventory to [TOGGLE] breaking tiles / How can a fish be used to dig through the ground? / Some questions are best left unanswered...
- Note de version : [2.2.0] Reworked Spadefish, it is no longer a pickaxe with 34% pickaxe power. It is now a rogue weapon, which throws the spadefish as a projectile when used. Spadefish can hit up to 3 enemies, bouncing off on hit. Stealth strikes briefly spin the spadefish around the 

### Starfleet (Weapons.Ranged)

Version du code la plus proche : 2019-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Fires a spread of plasma blasts
- Officiel : [use] fires a spread of starshots / [open] fires a concussive short range starburst / Starshots can be accelerated by firing when the starburst goes off / Perfect timing reduces spread and increases damage / Imperfect timing still increases damage, but heavily increases spread / Uses fallen stars, 50% chance to save ammo / Etoile, et toi
- Note de version : [2.1.0] Reworked Starfleet, it no longer fires a spread of exploding plasma blasts. Left-click now fires a spread of piercing star shots. Right-click now fires a short-range blast on a cooldown. The left-click and right-click attacks can be used independently of each 

### Storm Surge (Weapons.Ranged)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Fear the storm / Does not consume ammo
- Officiel : Fires a blue shockwave that passes through blocks / Does not consume ammo / Fear the storm

### Swordsplosion (Weapons.Melee)

Version du code la plus proche : 2019-06 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Sword swarm
- Officiel : Fires multiple rainbow beam swords that rain from the sky / Sword swarm

### Teratoma (SummonItems)

Version du code la plus proche : 2022-02 (ressemblance 0.96, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Summons the Hive Mind when in the corruption / Not consumable
- Officiel : Summons the Hive Mind when used in the Corruption / Despawns outside the Corruption / Not consumable / An exquisitely hideous tumor for those diseased in the head.

### Terra Lance (Weapons.Melee)

Nom actuel : Botanic Piercer. Version du code la plus proche : 2019-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Fires a lance beam
- Officiel : Fires a botanic energy beam
- Note de version : [2.0.4.001] Renamed Terra Lance to Botanic Piercer.

### The Burning Sky (Weapons.Melee)

Version du code la plus proche : 2021-09 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Hold the blade to the sky, and witness Armageddon
- Officiel : Rains down a stream of Dragonfire at the cursor / Hold the blade to the sky, and witness Armageddon

### Thorn Blossom (Weapons.Magic)

Version du code la plus proche : 2019-06 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Every rose has its thorn
- Officiel : Casts a beaming flower that explodes into homing petals alongside a deadly thorn / Costs 3 HP to fire / Every rose has its thorn

### Typhon's Greed (Weapons.Melee)

Version du code la plus proche : 2019-08 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Summons water spirits while in use
- Officiel : Summons water spirits while in use / These water spirits will home in on enemies after traveling towards the player

### Urchin Mace (Weapons.Melee)

Version du code la plus proche : 2022-07 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Throws out a whirlpool when released at full charge
- Officiel : Throws out a whirlpool when released / The people of the sea were adept hunters, but they abhorred unnecessary violence. / Unfortunately, neighboring nations were brutish, so they fashioned their tools for war.

### Venusian Trident (Weapons.Magic)

Version du code la plus proche : 2020-10 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Casts an infernal trident that erupts into a gigantic explosion of fire and magma shards
- Officiel : Casts an infernal trident that erupts into a gigantic explosion of fire and magma shards / Always critically strikes, with critical strike chance increasing critical strike damage

### Void Vortex (Weapons.Magic)

Version du code la plus proche : 2021-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Conjures a swirling vortex of supercharged magnet spheres around the cursor
- Officiel : Conjures a swirling vortex of supercharged magnet spheres around the cursor that explodes into a storm of homing mini spheres

### Voodoo Demon Voodoo Doll (Tools)

Version du code la plus proche : 2021-09 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Favorite this item to prevent voodoo demons from spawning near you
- Officiel : Toggle this item to prevent voodoo demons from spawning near you / Press [open] to toggle / Spawns are currently [STATE]

### Winter's Fury (Weapons.Magic)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : The pages are freezing to the touch
- Officiel : Fires a stream of ice orbs and a barrage of ice shards towards the cursor / The pages are freezing to the touch

### Wulfrum Battery (Accessories)

Version du code la plus proche : 2020-02 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : 7% increased summon damage
- Officiel : Your summons emit light if visibility is on / 7% increased summon damage / Wulfrum enemies drop additional scrap / Can also be scrapped at an extractinator
- Note de version : [2.0.1.001] Reworked Wulfrum Battery, in addition to increasing summon damage by 7%, it now causes all minions summoned to emit light, and adds a 50% chance for wulfrum enemies to drop an extra Wulfrum Metal Scrap when killed. Additionally, it can now be used in the Extra

### Wulfrum Controller (Weapons.Summon)

Version du code la plus proche : 2020-01 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Summons a wulfrum droid to fight for you
- Officiel : Summons a wulfrum droid to fight for you / Hold [open] while holding the remote to switch all of your drones between regular and supercharge mode / Supercharged droids will stop attacking and focus wulfrum energy onto you / Supercharged droids provide +0.25 HP/s life regen and 2 defense per droid / Can also be used to heal other players by keeping your mouse cursor close enough to them
- Note de version : [2.0.1.001] Reworked Wulfrum Controller, the Wulfrum Droid minion now recoils after firing its green projectile at enemies, and can no longer fire at enemies through walls. Additionally, it now has a right-click function that causes it to become supercharged. While superc

### Wulfrum Fishing Pole (Fishing)

Version du code la plus proche : 2019-11 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : This barely works, but it's better than nothing.
- Officiel : Can fish up Wulfrum Metal Scrap and Wulfrum Energy Cores / This barely works, but it's better than nothing...

### Wulfrum Knife (Weapons.Rogue)

Version du code la plus proche : 2020-06 (ressemblance 1.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Stealth strikes make the knife fly further and hit several times at once
- Officiel : Fires wulfrum knives in bursts of 3 that stick in enemies / Stealth strikes make the knife fly further and hit several times at once / [open] to magnetize all stuck knives back to you, restoring stealth / Returning knives also temporarily allows using stealth strikes without rogue armor
- Note de version : [2.0.1.001] Reworked Wulfrum Knife, it now throws out knives in a burst of three when used. The second and third knives spread out slightly as they travel. Stealth strike now causes each knife to damage an enemy twice, for a total of six hits if all of the knives hit an e

## Valeurs seulement : même fonctionnement, anciens chiffres (pourcentages, bonus, durées)

### Avalanche (Weapons.Melee)

Version du code la plus proche : 2021-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Spawns ice bombs that explode after 3 seconds into ice shards on hit
- Officiel : Hitting enemies spawns orbiting ice bombs / After 2 seconds, ice bombs explode into gravity-affected ice shards

### Glove of Precision (Accessories)

Version du code la plus proche : 2019-11 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Decreases rogue attack speed by 20% but increases damage and crit by 12% and velocity by 25%
- Officiel : Decreases rogue attack speed by 15% / Increases rogue damage and critical strike chance by 10%, and velocity by 15%

### Aquatic Emblem (Accessories)

Version du code la plus proche : 2021-01 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Most ocean enemies become friendly and provides waterbreathing / Being underwater slowly boosts your defense over time but also slows movement speed / The defense boost and movement speed reduction slowly vanish while outside of water / Maximum defense boost is 50, maximum movement speed reduction is 10% / Provides a small amount of light in the abyss / Moderately reduces breath loss in the abyss
- Officiel : Most ocean enemies become friendly and allows breathing underwater / Being underwater gradually increases defense but slows movement speed over 10 seconds / This effect slowly vanishes outside of water and resets while mounted / Maximum defense boost is 15 and movement speed reduction is 10% / Provides a small amount of light and moderately reduces breath loss in the abyss

### Bloody Worm Scarf (Accessories)

Version du code la plus proche : 2022-07 (ressemblance 0.93, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : 7 defense / 7% increased damage reduction / 7% increased melee damage and speed
- Officiel : 10% increased damage reduction and melee damage
- « 7 defense » : ailleurs dans le code (2019-02, Items/CalamityGlobalItem.cs : 7 defense)

### Flarewing Bow (Weapons.Ranged)

Version du code la plus proche : 2021-03 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Shoots a spread of 5 arrows / Wooden arrows are converted into bouncing obsidian bats
- Officiel : Shoots a spread of 3 arrows / Wooden arrows are converted into obsidian bats which home in after piercing once

### SDFMG (Weapons.Ranged)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : It came from the edge of Terraria / 50% chance to not consume ammo
- Officiel : 66% chance to save ammo / It came from the edge of Terraria

### Permafrost's Concoction (Accessories)

Version du code la plus proche : 2020-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.6

- Fandom : Increases maximum mana by 50 and reduces mana cost by 15% / Increases life regen as life decreases / Increases life regen when afflicted with Poison, On Fire, or Brimstone Flames / You will survive fatal damage and revive with 30% life on a 3 minute cooldown / You are encased in an ice barrier for 3 seconds when revived
- Officiel : Increases maximum mana by 40 and reduces mana cost by 15% / +0.5 HP/s life regen below half health, +1 HP/s when below 25% health and +2 HP/s when below 10% health / +3 HP/s life regen while afflicted with any fire debuff / You will survive fatal damage and revive with 30% life on a 3 minute cooldown / You are encased in an ice barrier for 3 seconds when revived

### Azathoth (Weapons.Melee)

Nom actuel : Ozzathoth. Version du code la plus proche : 2020-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.67

- Fandom : Fires cosmic orbs that blast nearby enemies with lasers / A very agile yoyo / Destroy the universe in the blink of an eye
- Officiel : Reach: 55 tiles / Speed: 60 / Infinite throw duration / Fires cosmic orbs that blast nearby enemies with lasers / Destroy the universe in the blink of an eye
- Note de version : [2.0.5] Renamed Azathoth to Ozzathoth.

### Defective Sphere (Weapons.Rogue)

Version du code la plus proche : 2020-06 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.67

- Fandom : Fires a variety of deadly spheres with different effects / Stacks up to 5 / Stealth strikes launch all 4 sphere types at once
- Officiel : Fires a variety of deadly spheres with different effects / Up to five spheres can be thrown at a time / Stealth strikes launch all 4 sphere types at once

### Magnomaly Cannon (Weapons.Ranged)

Version du code la plus proche : 2020-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.67

- Fandom : Launches a powerful exo rocket to nuke anything and everything / Rockets are surrounded by an invisible damaging aura and split into damaging beams on hit / 66% chance to not consume rockets
- Officiel : Launches a powerful exo rocket to nuke anything and everything with Miracle Blight / Rockets are surrounded by an invisible damaging aura and split into damaging beams on hit / 50% chance to save ammo

### Sulphurous Grabber (Weapons.Melee)

Version du code la plus proche : 2020-04 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.67

- Fandom : Occasionally releases a ring of colored bubbles / The yoyo powers up after touching a green bubble / A very agile yoyo
- Officiel : Reach: 25 tiles / Speed: 32 / Infinite throw duration / Occasionally releases a ring of colored bubbles / The yoyo powers up after touching a green bubble

### Tactical Plague Engine (Weapons.Summon)

Version du code la plus proche : 2021-01 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.67

- Fandom : Summons a plague jet to pummel your enemies into submission / Jets will fire ammo from your inventory, 66% chance to not consume ammo / Sometimes shoots a missile instead of a bullet
- Officiel : Summons a Plague jet to pummel your enemies into submission / Jets will fire bullets from your inventory / Sometimes shoots a missile instead of a bullet / 50% chance to save ammo / Missiles do not consume ammo

### Metastasis (Weapons.Summon)

Version du code la plus proche : 2021-12 (ressemblance 0.83, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.75

- Fandom : Summons a sepulcher to fight for you / While the sepulcher is active brimstone hearts will begin to appear and orbit you / It will attempt to attack you more and more frequently depending on how many hearts are present / It takes up 4 minion slots and it can only be summoned once / A contemplated possible future of the cosmic serpent, / A gruesome warning for those blinded by the hunger for power.
- Officiel : Summons a Sepulcher to fight for you / While the Sepulcher is active, brimstone hearts will begin to appear and orbit you / When too many hearts are present, it will interrupt attacking to attack the hearts instead / This happens more frequently depending on how many hearts are present / Can only be summoned once

### Seraphim (Weapons.Rogue)

Version du code la plus proche : 2022-02 (ressemblance 0.99, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.75

- Fandom : Throws an extraordinarily fast dagger which slows down exponentially and dissipates into light / Once dissipation has ended, 6 fast, splitting blades that aim at and slice nearby targets are released, along / with a large laserbeam / Stealth strikes release a volley of 9 lights which explode into smaller laser beams along with the dagger
- Officiel : Throws an extraordinarily fast dagger which slows down exponentially and dissipates into light / Once dissipation has ended, 6 splitting blades that slice nearby targets are released, along with a large laserbeam / Stealth strikes release a volley of 8 lights which explode into smaller laser beams along with the dagger

### The Oracle (Weapons.Melee)

Version du code la plus proche : 2020-03 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.75

- Fandom : Gaze into the past, the present, the future... and the circumstances of your inevitable demise / Emits an aura of red lightning which charges up when hitting enemies / Fires auric orbs when supercharged / An exceptionally agile yoyo
- Officiel : Reach: 50 tiles / Speed: 60 / Infinite throw duration / Emits an aura of auric lightning which charges up when hitting enemies / Fires auric orbs when supercharged / Gaze into the past, the present, the future... and the circumstances of your inevitable demise

### Universal Genesis (Weapons.Ranged)

Version du code la plus proche : 2022-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.75

- Fandom : Let the starry sky fall upon your enemies / Fires a spread of bullets from the gun and a flurry of stars to rain down on the cursor / Converts musket balls into starcaller shots that summon additional stars on enemy hits / 50% chance to not consume ammo
- Officiel : Fires a spread of bullets from the gun and a flurry of stars to rain down on the cursor / Converts musket balls into starcaller shots that summon additional stars on enemy hits / Let the starry sky fall upon your enemies

### Endogenesis (Weapons.Summon)

Version du code la plus proche : 2020-02 (ressemblance 0.98, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.78

- Fandom : Summons an ascended ice construct to protect you / Changes attack modes by resummoning or reusing the staff / The first mode makes it shoot sweeping lasers aimed at the enemy / The second mode sacrifices its limbs to shoot out homing projectiles / The third mode allows it to aggressively tackle its enemies / The fourth mode makes the limbs function as endothermic flamethrowers / Requires 10 minion slots to use / There can only be one / Ice puns not included
- Officiel : Summons an ascended ice construct to protect you / Changes attack modes by resummoning or reusing the staff / The first mode makes it shoot sweeping lasers aimed at the enemy / The second mode sacrifices its limbs to shoot out homing projectiles / The third mode allows it to aggressively tackle its enemies / The fourth mode makes the limbs function as endothermic flamethrowers / There can only be one construct at a time / Ice puns not included
- Note de version : [2.0.3.005] Reworked Profaned Soul Crystal. It now has an empowered state that is triggered by having no other minions summoned, and is no longer nerfed when using Endogenesis or Temporal Umbrella. Its enraged state is now triggered at nighttime instead of when below 50% 

### Frost Flare (Accessories)

Version du code la plus proche : 2022-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.8

- Fandom : All melee attacks and projectiles inflict frostburn / Immunity to frostburn, chilled, and frozen / Being above 75% life grants 10% increased damage / Being below 25% life grants 20 defense and 15% increased max movement speed and acceleration / Grants resistance against cold attacks
- Officiel : All attacks and projectiles inflict Frostbite / Being above 50% life grants 10% increased damage / Being below 50% life grants 10 defense and 15% increased max movement speed and acceleration / Grants resistance against cold attacks, Frostburn, Chilled and Frozen

### Prideful Hunter's Planar Ripper (Weapons.Ranged)

Version du code la plus proche : 2021-03 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.86

- Fandom : Every fourth shot deals 135% damage / Converts musket balls into lightning bolts / Lightning bolts travel extremely fast and explode on enemy kills / Lightning bolt crits grant a stacking speed boost to the player / This stacks up to 20 percent bonus movement speed and acceleration / The boost will reset if the player holds a different item / 33% chance to not consume ammo
- Officiel : Every fourth shot deals 135% damage / Converts musket balls into Electrified lightning bolts / Lightning bolts travel extremely fast and explode on enemy kills / Lightning bolt crits grant a stacking speed boost to the player / This stacks up to 20 percent bonus movement speed and acceleration / The boost will reset if the player holds a different item / 50% chance to save ammo

### Arbalest (Weapons.Ranged)

Version du code la plus proche : 2021-05 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Fires a volley of 10 high-speed arrows / Arrows start off small and grow in size with continuous fire / Arrow damage, spread and knockback scale with arrow size
- Officiel : Fires a volley of 6 high-speed arrows / Arrows start off small and grow in size with continuous fire / Arrow damage, spread and knockback scale with arrow size

### Arcturus Astroidean (Fishing)

Version du code la plus proche : 2019-11 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Increases fishing power if used in the Astral Infection or Sulphurous Sea
- Officiel : Increases fishing power by 1.1x if used in the Astral Infection or Sulphurous Sea

### Bloodstained Glove (Accessories)

Version du code la plus proche : 2019-12 (ressemblance 0.99, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Stealth strikes have +10 armor peneration and heal for 1 HP
- Officiel : Stealth strikes have +8 armor penetration and heal for 5 HP

### Coin of Deceit (Accessories)

Version du code la plus proche : 2022-10 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Stealth strikes only expend 85% of your max stealth / 6% increased rogue crit chance
- Officiel : Stealth strikes only expend 90% of your max stealth / 6% increased rogue crit chance

### Comet Shard (Misc)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Permanently increases maximum mana by 50
- Officiel : Permanently increases maximum mana by 60

### Cosmic Discharge (Weapons.Melee)

Version du code la plus proche : 2021-05 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Striking an enemy with the whip causes glacial explosions and grants the player the cosmic freeze buff / This buff gives the player increased life regen while standing still and freezes enemies near the player
- Officiel : Striking an enemy with the whip causes glacial explosions that grant the player the Cosmic Freeze buff / Cosmic Freeze gives the player +2 HP/s life regen while standing still and freezes enemies near the player

### Cryo Stone (Accessories)

Version du code la plus proche : 2021-10 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : One of the ancient relics / Creates a rotating ice shield around you that damages and slows down enemies on contact
- Officiel : 50% increased Cold debuff damage / Creates a rotating ice shield around you that damages and slows down enemies on contact / One of the ancient relics

### Ectoheart (Misc)

Version du code la plus proche : 2020-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Permanently increases Adrenaline Mode damage by 15% and damage reduction by 5% / Revengeance drop
- Officiel : Permanently increases Adrenaline Mode damage by 20% and damage reduction by 5% / Revengeance drop

### Electrician's Glove (Accessories)

Version du code la plus proche : 2022-02 (ressemblance 0.98, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Stealth strikes summon sparks on enemy hits / Stealth strikes have +10 armor penetration, deal 10% more damage, and heal for 1 HP
- Officiel : Stealth strikes summon sparks on enemy hits and heal you for 10 HP / Stealth strikes also have +8 armor penetration and deal 8% more damage

### Electrolyte Gel Pack (Misc)

Version du code la plus proche : 2020-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Permanently increases Adrenaline Mode damage by 15% and damage reduction by 5% / Revengeance drop
- Officiel : Permanently increases Adrenaline Mode damage by 20% and damage reduction by 5% / Revengeance drop

### Ethereal Core (Misc)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Permanently increases maximum mana by 50
- Officiel : Permanently increases maximum mana by 60

### Gael's Greatsword (Weapons.Melee)

Version du code la plus proche : 2021-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Hand it over, that thing. Your dark soul. / First swing fires homing skulls / Second swing fires a giant, powerful skull / Third swing has no projectiles / Constantly generates rage when in use / Swings leave behind exploding blood trails when below 50% health / Replaces Rage Mode with an enormous barrage of skulls
- Officiel : First swing fires homing skulls / Second swing fires a giant, powerful skull / Third swing has no projectiles / Constantly generates rage when in use / Swings leave behind exploding blood trails when below 50% health / Occasionally summons a barrage of lightning bolts when below 50% health / Replaces Rage Mode with an enormous barrage of skulls / This attack has a 30 second cooldown / Hand it over, that thing. Your dark soul

### Gilded Dagger (Weapons.Rogue)

Version du code la plus proche : 2020-03 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Throws a shiny blade that ricochets towards another enemy on hit / Stealth strikes cause the blades to home in after ricocheting, with each ricochet dealing 20% more damage / Stealth strikes also have increased piercing
- Officiel : Throws a shiny blade that ricochets towards another enemy on hit / Stealth strikes cause the blade to home in after ricocheting, with each ricochet dealing 5% more damage / Stealth strikes also have increased piercing

### Gleaming Dagger (Weapons.Rogue)

Version du code la plus proche : 2020-03 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Throws a shiny blade that ricochets towards another enemy on hit / Stealth strikes cause the blades to home in after ricocheting, with each ricochet dealing 20% more damage / Stealth strikes also have increased piercing
- Officiel : Throws a shiny blade that ricochets towards another enemy on hit / Stealth strikes cause the blade to home in after ricocheting, with each ricochet dealing 5% more damage / Stealth strikes also have increased piercing

### Heron Rod (Fishing)

Version du code la plus proche : 2019-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Increased fishing power in space. / A silent head and beak lanced down and plucked it out by the head, / and the beak swallowed the little fish while its tail waved frantically.
- Officiel : Increased fishing power by 1.1x when in space / A silent head and beak lanced down and plucked it out by the head, / and the beak swallowed the little fish while its tail waved frantically.

### Ink Bomb (Accessories)

Version du code la plus proche : 2019-11 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Throws several ink bombs when hit that explode in a confusing cloud of ink / Gain a lot of stealth when struck / This effect has a 20s cooldown before it can occur again
- Officiel : Stealth generates 7% faster / Throws several ink bombs when hit that explode into ink and confuse enemies / Gain a lot of stealth when struck / This effect has a 20 second cooldown before it can occur again

### Iron Francisca (Weapons.Rogue)

Version du code la plus proche : 2020-06 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : The franciscas do more damage for a short time when initially thrown / Stealth strikes pierce infinitely
- Officiel : The franciscas do more damage for a short time when initially thrown / Stealth strikes pierce up to 6 times

### Lead Tomahawk (Weapons.Rogue)

Version du code la plus proche : 2020-06 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : The tomahawks have more damage for a short time when initially thrown / Stealth strikes pierce infinitely
- Officiel : The tomahawks have more damage for a short time when initially thrown / Stealth strikes pierce up to 6 times

### Mirage Mirror (Accessories)

Version du code la plus proche : 2020-03 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Bend light around you / Reduces enemy aggression outside of the abyss / Stealth generates 30% faster when standing still and 20% faster while moving
- Officiel : Reduces enemy aggression outside of the abyss / Stealth generates 25% faster when standing still and 12% faster while moving / Bend light around you

### Onyxia (Weapons.Ranged)

Version du code la plus proche : 2019-08 (ressemblance 0.99, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : 50% chance to not consume ammo / Fires a storm of bullets and onyx shards
- Officiel : Fires a storm of bullets and onyx shards / 50% chance to save ammo

### Phantom Heart (Misc)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Permanently increases maximum mana by 50
- Officiel : Permanently increases maximum mana by 60

### Plague Staff (Weapons.Magic)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Fires a spread of plague fangs
- Officiel : Fires a tight spread of 4 Plague fangs

### Realm Ravager (Weapons.Ranged)

Version du code la plus proche : 2021-03 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Shoots a burst of 3 to 4 bullets / Converts musket balls into explosive bullets
- Officiel : Shoots a burst of 4 bullets / Converts musket balls into explosive bullets

### Ruin Medallion (Accessories)

Version du code la plus proche : 2019-11 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Stealth strikes only expend 50% of your max stealth / 6% increased rogue damage, and 6% increased rogue crit chance
- Officiel : Stealth strikes only expend 75% of your max stealth / 6% increased rogue damage and rogue crit chance

### Shield of the Ocean (Accessories)

Version du code la plus proche : 2020-02 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Increased defense by 5 when submerged in liquid / Increases movement speed and life regen while wearing the Victide armor
- Officiel : Increased defense by 5 when submerged in liquid / 10% increased movement speed and +1 HP/s life regen while wearing Victide armor

### Spelunker's Amulet (Accessories)

Version du code la plus proche : 2022-01 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Spelunker effect and 15% increased mining speed
- Officiel : Spelunker effect and 10% increased mining speed

### Starlight Fuel Cell (Misc)

Version du code la plus proche : 2020-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Permanently increases Adrenaline Mode damage by 15% and damage reduction by 5% / Revengeance drop
- Officiel : Permanently increases Adrenaline Mode damage by 20% and damage reduction by 5% / Revengeance drop

### Super Dummy (Tools)

Version du code la plus proche : 2018-12 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Creates a super dummy / Regenerates 1 million life per second / Will not die when taking damage over time from debuffs / Right click to kill all super dummies
- Officiel : Creates a super dummy / Regenerates 1 million life per second / Will not die when taking damage over time from debuffs / [open] to kill all super dummies / Can only spawn 50 super dummies at once

### Universe Splitter (Weapons.Summon)

Version du code la plus proche : 2021-07 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Summons an energy field at the mouse cursor / After the field has been deployed, it begins to summon multiple small beams / After several seconds have passed, an enormous laser beam appears at the field's position / This effect has a cooldown / Attempting to use this item during the cooldown will cause it to short circuit and do damage to you. / An ancient artifact from a previous age, it waits for your command... / This is a terrible idea, but it isn't yours anyways... right?
- Officiel : Summons an energy field at the mouse cursor / After the field has been deployed, it begins to summon multiple small beams / After several seconds have passed, an enormous laser beam appears at the field's position / This effect has a 30 second cooldown / Attempting to use this item during the cooldown will cause it to short circuit and do damage to you / An ancient artifact from a previous age, it waits for your command. / This is a terrible idea, but it isn't yours anyways... right?

### Verstaltite Fishing Rod (Fishing)

Version du code la plus proche : 2022-01 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Increased fishing power when in the tundra / The ancient alloy's prismatic qualities are perfect for attracting fish
- Officiel : Increased fishing power by 1.1x when in the tundra / The ancient alloy's prismatic qualities are perfect for attracting fish

### Vital Jelly (Accessories)

Version du code la plus proche : 2021-09 (ressemblance 1.0, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : 10% increased movement and jump speed
- Officiel : 12% increased movement and jump speed

## Mélange : chaque ligne du Fandom vient d'une version du code, sans qu'une version les ait toutes

### Astral Arcanum (Accessories)

Nom actuel : Radiance. Version du code la plus proche : 2022-02 (ressemblance 0.79, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Provides immunity to the astral infection debuff / Boosts life regen even while under the effects of a damaging debuff / While under the effects of a damaging debuff you will gain 15 defense / Press [Astral Arcanum UI Toggle] to toggle teleportation UI while no bosses are alive
- Officiel : +1 to +4 HP/s life regen based on missing health (Current: +[REGEN] HP/s) / Reduces damage over time taken by 12 HP/s and jump-starts regeneration after a damaging debuff ends / Natural life regen is 75% more potent and returns 100% faster / You emit shining blue light if visibility is enabled / A once putrid crystal, now glimmering with immaculate purity
- Note de version : [2.0.3.003] Renamed Astral Arcanum to Purity.
- Note de version : [2.0.4.001] Renamed Purity to Radiance.

### Cadaverous Carrion (Weapons.Summon)

Version du code la plus proche : 2020-05 (ressemblance 0.74, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Summons a geyser on the ground
- Officiel : Summons a sentry / Summons a gross, shark-puking Old Duke head on the ground

### Cinnamon Roll (Potions)

Version du code la plus proche : 2022-02 (ressemblance 0.77, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Boosts mana regeneration rate and multiplies all fire-based debuff damage by 1.5 / Reduces defense by 10%
- Officiel : Greatly increases dash speed / Greatly reduces dash distance / Drinking more than 3 different alcohols might not end well with your liver

### Grape Beer (Potions)

Version du code la plus proche : 2021-01 (ressemblance 0.67, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Reduces defense by 3% and movement speed by 5%
- Officiel : Bullets, Arrows, Darts, and Rockets gain homing / 75% reduced critical strike damage / Drinking more than 3 different alcohols might not end well with your liver

### Moonshine (Potions)

Version du code la plus proche : 2019-04 (ressemblance 0.71, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Increases defense by 10 and damage reduction by 5% / Reduces life regen by 1
- Officiel : Increases maximum health by 25% / Attacks always take at least half of your defense worth in defense damage / Defense damage takes three times as long to recover / Drinking more than 3 different alcohols might not end well with your liver

### Moscow Mule (Potions)

Version du code la plus proche : 2019-04 (ressemblance 0.65, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Boosts damage and knockback by 9% and critical strike chance by 3% / Reduces life regen by 2
- Officiel : 50% increased knockback, all attacks inflict heavy knockback / Reduces the effectiveness of piercing weapons / Drinking more than 3 different alcohols might not end well with your liver

### Red Wine (Potions)

Version du code la plus proche : 2019-08 (ressemblance 0.69, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Reduces life regen by 1
- Officiel : 10% increased vertical speed / 25% decreased flight time / Drinking more than 3 different alcohols might not end well with your liver

### Rum (Potions)

Version du code la plus proche : 2021-01 (ressemblance 0.8, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Boosts life regen by 2 and movement speed by 10% / Reduces defense by 5%
- Officiel : 15% increased non-minion summon damage / 15% decreased minion damage / Drinking more than 3 different alcohols might not end well with your liver

### Star Beam Rye (Potions)

Version du code la plus proche : 2021-01 (ressemblance 0.7, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Boosts max mana by 50, magic damage by 8%, / and reduces mana usage by 10% / Reduces defense by 6% and life regen by 1
- Officiel : Increases Mana regeneration, even while using an item / Magic weapons deal 0.9x damage / Drinking more than 3 different alcohols might not end well with your liver
- Note de version : [2.1.1] Reworked Star Beam Rye, it no longer disables natural mana regeneration, makes mana recover at a fixed rate, or increases magic damage based on the weapon's mana consumption. It now grants 15 mana per second mana regeneration even while using weapons, but mult

### Vodka (Potions)

Version du code la plus proche : 2021-01 (ressemblance 0.77, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Boosts damage by 6% and critical strike chance by 2% / Reduces life regen by 1 and defense by 5%
- Officiel : 25% increased Typeless debuff damage / 50% decreased elemental debuff damage / Drinking more than 3 different alcohols might not end well with your liver

### Voidragon (Weapons.Ranged)

Version du code la plus proche : 2022-07 (ressemblance 0.76, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : 75% chance to not consume ammo / Fires void blasts that explode every other shot
- Officiel : Loads a 50 round magazine of alternating bullets and concentrated void blasts / When all bullets are fired, the gun unleashes an enormous jet of abyssal fire / The power of the abyssal fire scales with the amount of shots hit / 66% chance to save ammo / Fade to black
- Note de version : [1.4.0.001] Reworked Voidragon, it now shoots bullets accompanied by exploding void blasts instead of firing exploding laser blasts. It now uses bullets instead of Fallen Stars as ammo. Buffed use time from 7 to 5, and shot speed from 15 to 18. Changed its use sound.
- Note de version : [2.2.0] Reworked Voidragon, it no longer rapidly fires bullets, exploding void blasts, and tentacles. It now loads a magazine of 50 shots, alternating between firing bullets and void blasts that inflict Whispering Death for 3 seconds. After firing the entire magazine,

### White Wine (Potions)

Version du code la plus proche : 2021-01 (ressemblance 0.58, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Boosts magic damage by 10% / Reduces defense by 6% and life regen by 1
- Officiel : You regain flight time the closer you are to an enemy / 50% reduced flight time / Drinking more than 3 different alcohols might not end well with your liver

### Elysian Aegis (Accessories)

Version du code la plus proche : 2020-03 (ressemblance 0.74, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.14

- Fandom : Blessed by the Profaned Flame / Grants immunity to knockback, and the Burning, On Fire! and Holy Flames debuffs / +30 max life / Grants a supreme holy flame dash / Can be used to ram enemies / Press [Elysian Gar] to activate buffs to all damage, crit chance, and defense / Activating this buff will reduce your movement speed and increase enemy aggro
- Officiel : Grants immunity to knockback / Allows the ability to dash, empowered by holy fire / This dash can ram through enemies without taking damage / Blessed by the Profaned Flame

### Delicious Meat (Potions)

Version du code la plus proche : 2018-12 (ressemblance 0.72, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.33

- Fandom : Medium improvements to all stats / So very delicious / Increases life regeneration
- Officiel : Medium improvements to all stats / Not consumable, but takes 15 minutes to regenerate / A blessed strip of pork that never seems to be entirely consumed...
- « Increases life regeneration » : ailleurs dans le code (2021-06, Items/CalamityGlobalItem.cs : Increases your life regeneration)

### Gruesome Eminence (Weapons.Magic)

Version du code la plus proche : 2021-11 (ressemblance 0.8, refait après 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.4

- Fandom : Summons a gaseous conglomeration of spirits near the cursor / At first, the spirits will fly wildly. This can hurt enemies and players / However, over time they will begin to accumulate to create a single, controllable monstrosity / The spirits of the amalgam could never pass on to their desired afterlife, / Tainted and melded by rage as they were.
- Officiel : Summons a gaseous conglomeration of spirits near the cursor that fly around wildly / Over time, the spirits accumulate to create a single, controllable monstrosity

### Everclear (Potions)

Version du code la plus proche : 2021-01 (ressemblance 0.7, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Boosts damage by 25% / Reduces life regen by 10 and defense by 30%
- Officiel : 25% increased damage / Reduces life regen by 5 HP/s and defense by 30% / Counts as 2 alcohols for Alcohol Poisoning / Drinking more than 3 different alcohols might not end well with your liver

### Margarita (Potions)

Version du code la plus proche : 2021-01 (ressemblance 0.67, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Reduces the duration of most debuffs / Reduces defense by 6% and life regen by 1
- Officiel : Halves the duration of most harmful debuffs / Reduces damage of debuffs you inflict by 50% / Drinking more than 3 different alcohols might not end well with your liver / 3 minute duration

### The Maelstrom (Weapons.Ranged)

Version du code la plus proche : 2022-05 (ressemblance 0.65, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.5

- Fandom : Fires charged Reaper Sharks that explode into water / Does extra damage to enemies shot at point-blank range
- Officiel : Fires charged Reaper Sharks that cause water explosions
- « Does extra damage to enemies shot at point-blank range » : ailleurs dans le code (2021-08, Items/CalamityGlobalItemTooltip.cs : Does extra damage to enemies shot at point-blank range)

### Ark of the Elements (Weapons.Melee)

Version du code la plus proche : 2018-12 (ressemblance 0.12, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.56

- Fandom : Performs a combo of swings, throwing the blade out every 5 swings / Releasing the mouse while the blade is out will throw the second half towards it, making the scissors snap / Snapping the scissors together increase their damage and empower your next two swings / Using RMB will snip out the scissor blades in front of you / Hitting an enemy with it will parry them, granting you a small window of invulnerability / You can also parry projectiles and temporarily make them deal 200 less damage / Parrying will empower the next 10 swings of the sword, letting you use both blades at once / Using RMB and pressing up while the Ark is empowered will throw the blades in front of you to provoke a Big Rip in spacetime, using up all your charges in the process / A heavenly pair of blades infused with the essence of Terraria, powerful enough to cut through the fabric of reality
- Officiel : [use] to perform a 5-swing combo that ends by throwing the blade / Releasing while the blade is out will deal extra damage / [open] to snip out the scissor blades in front of you / Hitting an enemy with it will parry them, granting you brief invulnerability / You can parry projectiles to make them deal 60 less damage for a short period / Parrying will empower the next 10 swings of the sword, letting you use both blades at once / A heavenly pair of blades infused with the essence of Terraria, powerful enough to cut through the fabric of reality
- Note de version : [1.5.1.001] Reworked Ark of the Elements, it now has a custom swing animation and no longer summons projectiles on each swing. Instead, stars and beams are fired on the second and fourth swing of a five-attack sequence. The fifth attack launches the sword which spins in p

### Ark of the Ancients (Weapons.Melee)

Version du code la plus proche : 2022-03 (ressemblance 0.22, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.6

- Fandom : Using RMB will extend the Ark out in front of you. Hitting an enemy with it will parry them, granting you a small window of invulnerability / You can also parry projectiles and temporarily make them deal 160 less damage / Parrying will empower the next 10 swings of the sword, boosting their damage and letting them throw stronger projectiles / Using RMB and pressing up while the Ark is empowered will release all the charges in a powerful burst of energy / A heavenly blade forged to vanquish all evil
- Officiel : [open] to extend the Ark out in front of you / Hitting an enemy with it will parry them, granting you brief invulnerability / You can also parry projectiles to make them deal 30 less damage for a short period / Parrying will empower the next 10 swings of the sword, letting it throw stronger projectiles / A heavenly blade forged to vanquish all evil
- Note de version : [1.5.1.001] Renamed Ark of the Ancients to Fractured Ark.
- Note de version : [1.5.1.001] Reworked Ark of the Ancients, it now has a custom swing animation and no longer summons projectiles on each swing. Instead, stars are fired every other swing. The player can right click to parry an enemy or projectile, granting the player immunity frames, redu

### Cinders of Lament (Weapons.Summon)

Version du code la plus proche : 2021-11 (ressemblance 0.76, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.6

- Fandom : Summons either Cataclysm or Catastrophe at the mouse position / They will look at you for a moment before charging at you / They can do damage to both you and enemies / The Witch, a sinner of her own making, / Within her mind her demon lies, ever patient, until the end of time.
- Officiel : Summons either Cataclysm or Catastrophe at the mouse position / They will look at you for a moment before charging at you / They can do damage to both you and enemies

### Cosmic Worm (SummonItems)

Version du code la plus proche : 2020-08 (ressemblance 0.46, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.67

- Fandom : Summons the Devourer of Gods / WARNING! Some sentinels have not been truly defeated yet and will spawn at full power during this fight! (If all Sentinels have not yet been defeated) / Not consumable
- Officiel : Summons the Devourer of Gods / Not consumable / The Nameless Serpent has set out for you.

### Exo Box (Mounts)

Version du code la plus proche : 2021-12 (ressemblance 0.67, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.75

- Fandom : Materializes a quite cozy and extremely nimble flying Exo throne / Hold [Exo Chair Speed Up] while sitting in the throne to move much faster / And hold [Exo Chair Slow Down] to move much slower / A comfortable gamer is a dangerous gamer
- Officiel : Materializes a quite cozy and extremely nimble flying Exo throne / Hold [Exo Chair Slow Down] while sitting in the throne to move slower for more precision / A comfortable gamer is a dangerous gamer

### Fractured Ark (Weapons.Melee)

Version du code la plus proche : 2022-07 (ressemblance 0.42, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.75

- Fandom : Using RMB will extend the Ark out in front of you. Hitting an enemy with it will parry them, granting you a small window of invulnerability / You can also parry projectiles and temporarily make them deal 100 less damage / Parrying will empower the next 10 swings of the sword, boosting their damage and letting them throw projectiles out / A worn down and rusty blade once wielded against the evil of this world, ready to be of use once more
- Officiel : [open] to extend the Ark out in front of you / Hitting an enemy with it will parry them, granting you a small window of invulnerability / Parrying empowers the next 10 swings of the sword, boosting damage and letting them throw projectiles / A worn down and rusty blade once wielded against the evil of this world, ready to be of use once more
- Note de version : [1.5.1.001] Reworked Fractured Ark, it now has a custom swing animation and no longer summons projectiles on each swing. The player can right click to parry an enemy or projectile, granting the player immunity frames, reducing damage taken from said attack, and causing th

### Abyss Torch (Placeables)

Nom actuel : Torches. Version du code la plus proche : None (ressemblance 0.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Can be placed in water
- Officiel : (pas de page)
- Note de version : [2.1.0] Renamed Abyss Torch to Void Torch.

### Biome Blade (Weapons.Melee)

Version du code la plus proche : 2022-02 (ressemblance 0.59, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Does nothing..yet / Repairing the blade seems to have improved its attuning capacities / Hold down RMB while standing still on flat ground to attune the weapon to the powers of the surrounding biome / Using RMB otherwise switches between the current attunement and an extra stored one / Main Attumenent : [None] / Secondary Attumenent : [None]
- Officiel : Does nothing..yet / Repairing the blade seems to have improved its attuning capacities / Hold down [open] while standing still on flat ground to attune the weapon to the powers of the surrounding biome / Pressing [open] otherwise switches between the current attunement and an extra stored one / Main Attunement : [None] / Secondary Attunement : [None]
- Note de version : [1.5.1.001] Renamed Biome Blade to Broken Biome Blade.
- Note de version : [1.5.1.001] Reworked Biome Blade, it no longer fires projectiles in its base form. The player can right click to attune to the surrounding biome, allowing the weapon to perform special attacks exclusive to the biome. The player can use attunements outside of the native bi

### Blunder Booster (Accessories)

Version du code la plus proche : 2021-01 (ressemblance 0.64, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : 12% increased rogue damage and 15% increased rogue projectile velocity / Stealth generates 10% faster / Summons a red lightning aura to surround the player and electrify nearby enemies / Press [Booster Dash] to consume 25% of your maximum stealth to perform a swift upwards/diagonal dash which leaves a trail of lightning bolts
- Officiel : 12% increased rogue damage and 15% increased rogue projectile velocity / Stealth generates 10% faster / Summons a red lightning aura around the player that electrifies nearby enemies with Vermillion Flux / The aura has reduced opacity if visibility is disabled / Press [Booster Dash] to consume 25% of your maximum stealth to perform a swift upwards diagonal dash / This dash leaves a trail of red lightning bolts / This effect has a 1 second cooldown before it can be used again

### Broken Biome Blade (Weapons.Melee)

Version du code la plus proche : 2022-02 (ressemblance 0.67, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Does nothing.. yet / Hold down RMB while standing still on flat ground to attune the weapon to the powers of the surrounding biome / Using RMB otherwise switches between the current attunement and an extra stored one / Main Attumenent : [None] / Secondary Attumenent : [None]
- Officiel : Does nothing.. yet / Hold down [open] while standing still on flat ground to attune the weapon to the powers of the surrounding biome / Pressing [open] otherwise switches between the current attunement and an extra stored one / Main Attunement : [None] / Secondary Attunement : [None]
- Note de version : [1.5.1.001] Reworked Broken Biome Blade, it no longer fires projectiles in its base form. The player can right click to attune to the surrounding biome, allowing the weapon to perform special attacks exclusive to the biome. The player can use attunements outside of the na

### Hemagrape (Misc)

Nom actuel : Sanguine Tangerine. Version du code la plus proche : None (ressemblance 0.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : It has a distinctly sweet flavor and a strong aroma / Permanently increases maximum life by 25 / Can only be used if the max amount of life fruit has been consumed
- Officiel : Permanently increases maximum life by 25 / Can only be used once and requires the max amount of Life Fruits consumed / It has a distinctly sweet flavor and a strong aroma

### Normality Relocator (Tools)

Version du code la plus proche : 2021-05 (ressemblance 0.79, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : I'll be there in the blink of an eye / Press [Normality Relocator] to teleport to the position of the mouse / Fall speed is doubled for 30 frames after teleporting / Teleportation is disabled while Chaos State is active / Works while in the inventory
- Officiel : Press [Normality Relocator] to teleport to the position of the mouse / Fall speed is doubled for 0.5 seconds after teleporting / Teleportation is disabled while Chaos State is active / Works while in the inventory / I'll be there in the blink of an eye

### Plagued Fuel Pack (Accessories)

Version du code la plus proche : 2020-04 (ressemblance 0.75, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : 5% increased rogue damage and 15% increased rogue projectile velocity / Stealth generates 10% faster / Press [Booster Dash] to consume 25% of your maximum stealth to perform a swift upwards/diagonal dash which leaves a trail of plagued clouds
- Officiel : 8% increased rogue damage and 15% increased rogue projectile velocity / Stealth generates 10% faster / Press [Booster Dash] to consume 25% of your maximum stealth to perform a swift upwards diagonal dash / The dash leaves a lingering trail of Plague clouds / This effect has a 1 second cooldown before it can be used again

### Sambucus Fruit (Misc)

Nom actuel : Tainted Cloudberry. Version du code la plus proche : None (ressemblance 0.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : A tangy, tart flavor with a somewhat earthen touch / Permanently increases maximum life by 25 / Can only be used if the max amount of life fruit has been consumed
- Officiel : Permanently increases maximum life by 25 / Can only be used once and requires the max amount of Life Fruits consumed / A tangy, tart flavor with a somewhat earthen touch

### True Biome Blade (Weapons.Melee)

Version du code la plus proche : 2018-12 (ressemblance 0.21, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 1.0

- Fandom : Does nothing.. yet / It seems that upgrading the blade expanded the scope of the previous attunements / Your secondary attunement can now provide passive bonuses / Holding down RMB for 2 seconds attunes the weapon to the powers of the surrounding biome / Using RMB for a shorter period of time switches your active and passive attunements around / Active Attumenent : [None] / Passive Attumenent : [None]
- Officiel : Does nothing.. yet / It seems that upgrading the blade expanded the scope of the previous attunements / Your secondary attunement can now provide passive bonuses / Holding down [open] for 2 seconds attunes the weapon to the powers of the surrounding biome / Pressing [open] for a shorter period of time switches your active and passive attunements around / Active Attunement : [None] / Passive Attunement : [None]
- Note de version : [1.5.1.001] Renamed True Biome Blade to Biome Blade.
- Note de version : [1.5.1.001] Reworked True Biome Blade, it no longer fires a spread of projectiles in its base form. The player can right click to attune to the surrounding biome, allowing the weapon to perform special attacks exclusive to the biome, in addition to secondary effects while

## À vérifier : lignes du Fandom absentes de toutes les versions du code (inventées, ou ajoutées par les rédacteurs)

## Homonymes (revue à la main) : objet inventé par le Fandom ; Calamity a créé plus tard un objet différent du même nom

### Disgusting Meat (Tools)

Version du code la plus proche : None (ressemblance 0.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Mmm... a delicacy, to be sure! / Best served cold
- Officiel : [use] will remove health and mana upgrades, use with no Calamity upgrades to remove vanilla upgrades / [open] will remove Rage and Adrenaline upgrades, plus other miscellaneous boosters such as Demon Heart / Removed boosters will be vomited out of you as items
- Lignes introuvables dans le code : Mmm... a delicacy, to be sure! / Best served cold
- **Revue** : Le Fandom décrit un matériau inventé (lâché par les zombies, l'Eater of Worlds, la Dragonfolly...) qui sert aux « Tainted Potions » inventées. Calamity a ajouté en 2.1.0 un objet du même nom qui retire les améliorations du joueur : en Full Fandom, ce devrait être le matériau du wiki, pas l'objet de Calamity.

### Ha-pu Fruit (Accessories)

Version du code la plus proche : None (ressemblance 0.0, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.0

- Fandom : Permanently increases Rage Mode damage by 5% / Revengeance drop
- Officiel : A fruit that calms your nerves (may have unintended side effects)
- Lignes introuvables dans le code : Permanently increases Rage Mode damage by 5%
- **Revue** : Le Fandom en fait une amélioration permanente (+5 % de dégâts de la Rage) obtenue en jetant dans le Shimmer des objets inventés (Mushroom Plasma Root, Infernal Blood, Red Lightning Container). Calamity a ajouté en 2.0.3.003 un Ha-pu Fruit différent (« A fruit that calms your nerves »).
- « Revengeance drop » : ailleurs dans le code (2018-12, Items/Accessories/CounterScarf.cs : Revengeance drop) — **recopiée d'un autre objet**

## Addons (revue à la main) : le Fandom décrit l'effet d'un autre mod

### Eidolon Staff (Weapons.Magic)

Version du code la plus proche : 2022-07 (ressemblance 0.52, déjà différent en 2.0.1.005) ; lignes encore présentes aujourd'hui : 0.17

- Fandom : The power of an ancient cultist resonates within this staff / Fires a spread of ancient light and has a chance to fire a spinning ice cluster / Summons a Juvenile Eidolon Wyrm to fight for you which takes up 4 minion slots / Right click to fire out Ice Clusters / When an Ice Cluster intersects with the Wyrm, it explodes into homing icicles / Only one wyrm can exist at a time Calamity: Community Remix
- Officiel : Fires a spread of ancient light and a spinning ice cluster
- Lignes introuvables dans le code : Summons a Juvenile Eidolon Wyrm to fight for you which takes up 4 minion slots / Right click to fire out Ice Clusters / When an Ice Cluster intersects with the Wyrm, it explodes into homing icicles / Only one wyrm can exist at a time Calamity: Community Remix
- **Revue** : Tir de lumière ancienne et d'amas de glace : ancienne version (2019). Le Juvenile Eidolon Wyrm invoqué (4 emplacements, amas de glace au clic droit) vient de Calamity: Community Remix, cité dans l'infobulle.
- Note de version : [1.4.0.001] Renamed Stardust Staff to Eidolon Staff.

## Ambiance (revue à la main) : seule une phrase d'ambiance diffère, même fonctionnement

Yharon Soul Fragment, Blood Bath, The Dance of Light, Yharim's Crystal, Ark of the Cosmos, Phosphorescent Gauntlet, Storm Ruler, Halibut Cannon, Metal Monstrosity

## Reformulation (revue à la main) : même fonctionnement

Havoc's Breath, Angelic Alliance

## Reformulées seulement (même fonctionnement, ancien texte)

Abyssal Diving Gear, Dragon Scales, Magic Scarf and Hat, Nucleogenesis, Scuttler's Jewel, Star-Tainted Generator, Mortar Round, Dragoon Drizzlefish, Enchanted Starfish, Stuffed Fish, Xerocodile, Titan Heart, Dragonfruit, Miracle Fruit, Enchanted Butterfly, Little Light, Radiating Crystal, Aureus Cell, Astral Chunk, Cryo Key, Portabulb, Starcore, Arid Artifact, Plasma Grenade, Pole Warper, Arctic Bear Paw, Astralachnea Staff, Cosmic Rainbow, Event Horizon, Face Melter, Helium Flash, Keelhaul, Mad Alchemist's Cocktail Glove, Phoenix Flame Barrage, Rancor, The Prince, Vivid Clarity, Wyvern's Call, Absolute Zero, Astral Scythe, Bonebreaker, Brimlance, Brimstone Sword, Dragon Rage, Elemental Lance, Galactus Blade, Illustrious Knives, Nebulash, Red Sun, Titan Arm, Alluvion, Astral Blaster, Bladedge Railbow, Bullet-Filled Shotgun, Clamor Rifle, Drataliornus, Effluvium Bow, Eternal Blizzard, Goobow, Onyx Chain Blaster, Pumpler, Shellshooter, Apoctolith, Ashen Stalactite, Celestus, Consecrated Water, Epidemic Shredder, Fishbone Boomerang, Grave Grimreaver, Jaws of Oblivion, Leonid Progenitor, Regulus Riot, Spent Fuel Container, The Final Dawn, Time Bolt, Abandoned Slime Staff, Cryogenic Staff, Dormant Brimseeker, Eye of Night, Flamsteed Ring, Guidelight of Oblivion, Resurrection Butterfly, Aestheticus

## Formulation seulement (le Fandom suit le code actuel)

Serpentine, Quagmire, Archerfish, Daemon's Flame, Phangasm

## Bonus de set d'armure (lignes du Fandom qui ne sont plus vraies)

### Aerospec armor (ancienne version)

- ancienne version (2021-12) : Helmet:' 11% increased minion damage and +1 max minion

### Auric Tesla armor (ancienne version)

- introuvable dans le code : Uelibloom, Bloodflare, God Slayer, and Silva armor effects
- introuvable dans le code : Space Helmet': +6 max minions and 75% increased minion damage
- **Revue** : Bonus de set d'avant la refonte (effets des sets Tarragon, Bloodflare, God Slayer et Silva réunis), avec les noms Tarragon/Uelibloom échangés par le Fandom ; valeurs du casque d'invocateur de l'époque.

### Bloodflare armor (ancienne version)

- ancienne version (2018-12) : Enemies killed during a Blood Moon have a much higher chance to drop Blood Orbs
- ancienne version (2018-12) : True melee strikes will heal you
- ancienne version (2021-07) : Wyvern Helm': 50% increased minion damage and +3 max minions
- ancienne version (2019-02) : At 90% life and above you gain 10% increased minion damage
- ancienne version (2019-02) : At 50% life and below you gain 20 defense and 2 life regen
- ancienne version (2019-02) : Imp Mask': Being over 80% life boosts your defense by 30 and rogue crit by 5%
- ancienne version (2019-02) : Being below 80% life boosts your rogue damage by 10%
- ancienne version (2019-02) : rogue critical strikes have a 50% chance to heal you

### Brimflame armor (ancienne version)

- ancienne version (2020-05) : Grants an additional 15% increased damage and magic crit
- ancienne version (2020-04) : While under this effect, your damage is significantly boosted
- ancienne version (2020-04) : However, this comes at the cost of rapid life loss and no mana regeneration
- ancienne version (2020-04) : This can be toggled off, however, a brimstone frenzy has a 30 second cooldown
- introuvable dans le code : Press [Armor Set Bonus] to trigger a brimflame frenzy effect
- **Revue** : Frénésie de Brimflame sur touche (2020) : ancienne version ; la ligne « Press [Armor Set Bonus]... » est le raccourci ajouté par le code.

### Daedalus armor (ancienne version)

- ancienne version (2021-04) : Reflected projectiles deal 50% less damage to you
- ancienne version (2018-12) : You have a 10% chance absorb physical attacks and projectiles when hit
- ancienne version (2018-12) : If you absorb an attack you are healed for 1/2 of that attack's damage
- ancienne version (2021-01) : Mask': +20% increased minion damage and +2 max minions
- introuvable dans le code : Headgear': +5% ranged damage
- **Revue** : Absorption des attaques, réflexion, bonus par casque : ancienne version (2018-2021). « Headgear: +5% ranged damage » : invérifiable (valeur du casque de l'époque).

### Demonshade armor (ancienne version)

- ancienne version (2021-01) : 100% increased minion damage and +10 max minions
- ancienne version (2018-12) : All attacks inflict the demon flame debuff
- ancienne version (2018-12) : A friendly red devil follows you around
- ancienne version (2019-04) : This makes them do 25% more damage but they also take 125% more damage

### Desert Prowler armor (ancienne version)

- introuvable dans le code : Ranged attacks deal an additional 1 flat damage
- introuvable dans le code : Double tap down to activate Sandsmoke Bomb.
- **Revue** : Ancien bonus (dégâts fixes en plus pour les attaques à distance, bombe de fumée). Formulations invérifiables : « +1 flat damage », « Double tap down to activate Sandsmoke Bomb » (la bombe se déclenche aujourd'hui par une touche).

### Empyrean armor (ancienne version)

- ancienne version (2018-12) : Imbued with cosmic wrath and rage when you are damaged

### Fathom Swarmer armor (ancienne version)

- ancienne version (2018-12) : 10% increased minion damage and +2 max minions

### Fearmonger armor (ancienne version)

- ancienne version (2020-06) : 20% increased minion damage and +1 max minions
- ancienne version (2020-08) : The minion damage nerf while wielding weaponry is reduced
- introuvable dans le code : All minion attacks grant regeneration
- **Revue** : +20 % dégâts des serviteurs et +1 serviteur, pénalité réduite avec une arme (2020) : ancienne version. « All minion attacks grant regeneration » : invérifiable.

### Forbidden Circlet (ancienne version)

- ancienne version (2020-05) : rogue and summon attacks will scale off of the stat with a higher boost

### God Slayer armor (ancienne version)

- ancienne version (2021-07) : Allows you to dash for an immense distance in 8 directions
- ancienne version (2018-12) : Taking over 80 damage in one hit will cause you to release a swarm of high-damage god killer darts
- ancienne version (2018-12) : Enemies take a lot of damage when they hit you
- ancienne version (2018-12) : An attack that would deal 80 damage or less will have its damage reduced to 1
- ancienne version (2021-04) : Helmet': You fire a god killer shrapnel round while firing ranged weapons every 2.5 seconds
- ancienne version (2019-02) : Mask': While at full HP all of your rogue stats are boosted by 10%
- ancienne version (2018-12) : If you take over 80 damage in one hit you will be given extra immunity frames
- introuvable dans le code : Press [God Slayer Dash] while holding down the movement keys in the direction you want to dash
- **Revue** : Ruée en 8 directions, fléchettes, dégâts réduits à 1 sous 80, bonus par casque : ancienne version (2018-2021) ; « Press [God Slayer Dash]... » est le raccourci ajouté par le code.

### Hydrothermic Armor (ancienne version)

- ancienne version (2021-01) : Mask': 5% increased magic damage and 15% reduced mana usage
- ancienne version (2021-01) : Helmet': 40% increased minion damage and +2 max minions

### Mollusk armor (ancienne version)

- ancienne version (2019-04) : Two shellfishes aid you in combat

### Omega Blue armor (ancienne version)

- introuvable dans le code : Press [Armor Set Bonus] to activate abyssal madness for 5 seconds
- **Revue** : « Press [Armor Set Bonus] to activate abyssal madness for 5 seconds » : raccourci du code, ancienne version du bonus.

### Plague Reaper armor (ancienne version)

- ancienne version (2020-01) : Enemies receive 10% more damage from ranged projectiles when afflicted by the Plague
- ancienne version (2020-01) : Getting hit causes the plague cinders to rain from above
- ancienne version (2020-01) : Press the [Armor Set Bonus] key to blind yourself for 5 seconds but massively boost your ranged damage

### Plaguebringer armor (ancienne version)

- ancienne version (2020-06) : Grants a plague dash to ram enemies and afflict them with the plague
- ancienne version (2020-06) : Summons a lil’ plaguebringer to protect you and empower nearby minions
- ancienne version (2019-02) : +3 max minions

### Prismatic armor (ancienne version)

- ancienne version (2020-06) : +40 max mana and 15% reduced mana cost

### Reaver armor (ancienne version)

- ancienne version (2021-01) : Helm': +10 defense and +3 life regen
- ancienne version (2021-01) : All attacks have a small chance to steal life and speed up the rate of life regen
- ancienne version (2021-01) : Enemy damage is reflected and summons a thorn spike
- ancienne version (2021-01) : Reaver Rage has a 25% chance to activate when you are damaged
- ancienne version (2021-01) : Headgear': Causes nearby treasure to sparkle
- ancienne version (2021-01) : Increased item grab range and block placement range
- ancienne version (2021-01) : Mining tiles restores breath while underwater
- ancienne version (2021-01) : Summons a reaver orb to light up the area around you

### Tarragon armor (ancienne version)

- ancienne version (2018-12) : Amount healed is based on projectile damage
- ancienne version (2021-07) : Horned Helm': 50% increased minion damage and +3 max minions
- ancienne version (2019-02) : While under effects of a debuff you gain 10% increased rogue damage

### Umbraphile armor (ancienne version)

- ancienne version (2019-12) : Penumbra potions always build stealth at max effectiveness

### Gem Tech armor (reformulation)

- introuvable dans le code : A grand total of six exquisitely crafted gem fragments, each one an embodiment of one of the many diverse and distinct classes within the realm of magical and combative arts, along with a singularly unique and multifaceted base gem, gracefully and effortlessly orbit around you in an entrancing and mesmerizing display of arcane prowess and mysticism. These gems are far from being mere ornamental decorations; they are imbued with potent and formidable abilities and powers, each specific to their respective classes. However, be forewarned, for there exists a perilous risk associated with these precious gems. Should you, in the heat of battle, sustain a significant and grievous amount of damage—specifically, more than 100 damage from a single, devastating hit—you will face the unfortunate consequence of losing one of these invaluable gems. The particular gem that will be lost corresponds directly and unequivocally to the class of the most recent ability, skill, or weapon that you had employed prior to the impact. In the event that the specific class gem that ought to be lost has already been forfeited in a previous encounter or mishap, the highly valuable and versatile base gem will be sacrificed in its stead. Upon the unfortunate loss of a gem, it does not simply vanish into the ether, dissipating into nothingness, but rather dramatically and forcefully breaks off from its orbit around you, and with a relentless and aggressive determination, homes in on the nearest enemy or fearsome boss, provided one is within the vicinity. This homing mechanism ensures that the gem seeks out and targets the closest threat with unerring accuracy. When this gem strikes its designated target, it delivers a catastrophic and devastating blow, inflicting a base damage of a staggering and formidable 40,000, leaving a lasting impact on the foe. Once a gem is lost, it is not gone forever into the void but instead enters a temporary state of dormancy, a period of absence, reappearing and rejoining the orbit around you after a precisely measured and calculated duration of 30 seconds. Each gem carries its own unique set of enhancements and abilities, tailored to augment and bolster the capabilities of its respective class. For instance, the vibrant and fiery red gem bestows an additional 130 maximum stealth, significantly bolsters rogue-related statistics and attributes, and ensures that stealth is only expended through the use of rogue-specific weapons, thereby enhancing the efficacy and potency of rogue actions. The radiant and luminous yellow gem, on the other hand, serves to enhance melee capabilities, providing increased melee statistics and causing melee attacks to release harmful and damaging shards upon hitting their targets. This effect comes with a cooldown period, a brief interval before it can be activated again. Notably, this cooldown is significantly shorter for pure melee attacks, thus favoring direct and unadulterated melee combat. The verdant and vibrant green gem is designed to augment ranged combat capabilities. It provides increased ranged statistics and triggers swift-flying flechettes that hone in on and damage enemies when they are struck by ranged projectiles. This effect ensures that enemies are continually harassed and damaged by additional projectiles, enhancing the overall effectiveness of ranged attacks. The serene and tranquil blue gem is a boon for summoners, granting an additional four maximum minions, thereby expanding the summoner's arsenal, increasing minion damage, and mitigating the penalty typically associated with summoner items when wielding a non-summoner weapon. This gem thus enhances the versatility and adaptability of summoners in combat. The mystical and enigmatic violet gem is tailored to enhance magical prowess. It provides an extra 100 maximum mana, boosting magic-related statistics and attributes, and enables rapid mana regeneration when holding a non-magical weapon. This effect ensures that mages can quickly replenish their mana reserves, maintaining their ability to cast powerful spells. Lastly, the delicate and multifaceted pink base gem offers a comprehensive suite of benefits, including an additional 75 defense, enhanced damage reduction, increased movement speed, improved jump speed, and a bonus of 2 life regeneration. This gem serves as a foundation, providing overall enhancements that benefit the wielder in various aspects of combat and survival. In the extraordinary and rare event that all six gems are present, active, and orbiting around you simultaneously, a special and potent effect is triggered. Striking a target with any weapon will grant you an additional 2 life regeneration for a duration of 8 seconds. This beneficial effect is further amplified to 3 life regeneration if, during this 8-second window, you utilize a weapon from a different class, for a period of 2.5 seconds. This synergistic effect encourages versatility and adaptability in combat, rewarding those who skillfully switch between different weapon classes.
- **Revue** : Le setbonus est réécrit en un long pavé grandiloquent (vandalisme de forme), mais décrit le vrai bonus : six gemmes de classe et une gemme de base, perte d'une gemme au-delà de 100 dégâts d'un coup, gemme qui frappe pour 40 000, retour après 30 s, bonus par gemme.

## Objets retirés de Calamity (pages « new » qui portent le nom d'un ancien objet)

Infobulle du Fandom comparée aux versions de l'objet retiré (date de retrait = dernière version mensuelle où il existait ; 2.0.1.005 = encore présent à l'abandon du wiki).

### mélange (10)

- **Ancient Shiv** (AncientShiv, retiré après 2022-06) : version la plus proche 2018-12 (0.64) ; introuvable : Shortflurry attacks increase melee speed by 5% ; **Revue** : Ancien objet de Calamity décrit fidèlement (épée courte retirée), sauf la ligne Shortflurry : mécanique inventée par le Fandom (page Shortstrike, déjà implémentée).
- **Aquatic Discharge** [fait] (AquaticDischarge, retiré après 2.0.1.005) : version la plus proche 2018-12 (0.68) ; introuvable : Shortflurry attacks paralyze enemies ; **Revue** : Ancien objet de Calamity décrit fidèlement (épée courte retirée), sauf la ligne Shortflurry : mécanique inventée par le Fandom (page Shortstrike, déjà implémentée).
- **Arcanum of the Void** (ArcanumoftheVoid, retiré après 2022-01) : version la plus proche 2021-11 (0.6) ; introuvable : Spawns A Void Barrier around you that shields you from A hit / You release Dark Energy when you get hit and the barrier breaks ; **Revue** : Ancien objet (retiré en 2022) : recharge de 90 s partagée avec les esquives, réelle (2021). Inventés : la « Void Barrier » qui encaisse un coup et l'énergie sombre libérée ; l'objet renvoyait les projectiles (5 à 10 % de chance en 2018-2019, puis à coup sûr avec recharge).
- **Bloody Rupture** (BloodyRupture, retiré après 2022-06) : version la plus proche 2018-12 (0.6) ; introuvable : Shortflurry attacks create bloodsplosions on enemy hits ; **Revue** : Ancien objet de Calamity décrit fidèlement (épée courte retirée), sauf la ligne Shortflurry : mécanique inventée par le Fandom (page Shortstrike, déjà implémentée).
- **Eutrophic Shank** (EutrophicShank, retiré après 2022-06) : version la plus proche 2019-04 (0.51) ; introuvable : Shortflurry attacks inflict Eutrophication ; **Revue** : Ancien objet de Calamity décrit fidèlement (épée courte retirée), sauf la ligne Shortflurry : mécanique inventée par le Fandom (page Shortstrike, déjà implémentée).
- **Exo Gladius** (ExoGladius, retiré après 2022-06) : version la plus proche 2020-02 (0.38) ; introuvable : Striking an enemy with the blade causes an explosion / Right-clicking with it activates a parry that can be used to deflect projectiles / The player becomes temporarily immune when using the parry / The parry is stronger, but has a longer cooldown ; **Revue** : Ancien objet (retiré en 2022) : immunité brève en frappant, réelle (2020). Inventés : l'explosion au coup et la parade au clic droit qui renvoie les projectiles ; l'arme tirait un orbe arc-en-ciel qui appelait des rayons et des comètes.
- **Leeching Dagger** (LeechingDagger, retiré après 2022-06) : version la plus proche 2018-12 (0.59) ; introuvable : Shortflurry strikes heal the player for 5 health per hit ; **Revue** : Ancien objet de Calamity décrit fidèlement (épée courte retirée), sauf la ligne Shortflurry : mécanique inventée par le Fandom (page Shortstrike, déjà implémentée).
- **Night's Stabber** (NightsStabber, retiré après 2022-06) : version la plus proche 2021-02 (0.63) ; introuvable : Shortflurry attacks heal the player, spawn night vines, / Inflict Cursed Inferno, and increase melee speed ; **Revue** : Ancien objet de Calamity décrit fidèlement (épée courte retirée), sauf la ligne Shortflurry : mécanique inventée par le Fandom (page Shortstrike, déjà implémentée).
- **Terra Shiv** (TerraShiv, retiré après 2022-06) : version la plus proche 2018-12 (0.39) ; introuvable : Shortflurry attacks shoot out splitting beams that heal and inflict ichor / Melee speed and DR are also boosted by 15% ; **Revue** : Ancien objet de Calamity décrit fidèlement (épée courte retirée), sauf la ligne Shortflurry : mécanique inventée par le Fandom (page Shortstrike, déjà implémentée).
- **True Night's Stabber** (TrueNightsStabber, retiré après 2022-06) : version la plus proche 2018-12 (0.48) ; introuvable : Shortflurry attacks spawn healing beams that shoot cursed fireballs, and increases melee speed by 10% ; **Revue** : Ancien objet de Calamity décrit fidèlement (épée courte retirée), sauf la ligne Shortflurry : mécanique inventée par le Fandom (page Shortstrike, déjà implémentée).

### ancien objet (201)

- **Acceleration Round** (AccelerationBullet, AccelerationRound, retiré après 2.0.1.005) : version la plus proche 2018-12 (1.0)
- **Acidic Rain Barrel** (AcidicRainBarrel, retiré après 2.0.1.005) : version la plus proche 2020-04 (1.0)
- **Aeries** (Aeries, retiré après 2022-01) : version la plus proche 2021-03 (1.0)
- **Aero Dynamite** (AeroDynamite, retiré après 2022-06) : version la plus proche 2020-09 (1.0)
- **Aftershock** [fait] (Aftershock, retiré après 2.0.1.005) : version la plus proche 2018-12 (1.0)
- **Alpha Ray** (AlphaRay, retiré après 2.0.1.005) : version la plus proche 2019-08 (1.0)
- **Alpha Virus** (AlphaVirus, retiré après 2.0.1.005) : version la plus proche 2022-02 (1.0)
- **Ancient Crusher** (AncientCrusher, retiré après 2022-01) : version la plus proche 2018-12 (1.0)
- **Animus** (Animus, retiré après 2022-06) : version la plus proche 2019-08 (1.0)
- **Aquatic Dissolution** (AquaticDissolution, retiré après 2022-06) : version la plus proche 2019-12 (1.0)
- **Arctic Arrow** (ArcticArrow, retiré après 2.0.1.005) : version la plus proche 2018-12 (1.0)
- **Armageddon** (Armageddon, retiré après 2022-01) : version la plus proche 2021-12 (1.0)
- **Asteroid Staff** [fait] (AsteroidStaff, retiré après 2.0.1.005) : version la plus proche 2019-08 (1.0)
- **Astral Blade** (AstralBlade, retiré après 2.0.1.005) : version la plus proche 2020-07 (1.0)
- **Astral Bow** [fait] (AstralRepeater, retiré après 2.0.1.005) : version la plus proche 2019-08 (1.0)
- **Astral Bulwark** (AstralBulwark, retiré après 2022-01) : version la plus proche 2019-09 (1.0)
- **Baby Flak Crab** [fait] (BabyFlakCrabItem, BabyFlakHermit, retiré après 2.0.1.005) : version la plus proche 2020-04 (1.0)
- **Bazooka** (Bazooka, retiré après 2.0.1.005) : version la plus proche 2022-02 (1.0)
- **Beetle Juice** [fait] (BeetleJuice, retiré après 2.0.1.005) : version la plus proche 2018-12 (1.0)
- **Bittercold Staff** (BittercoldStaff, retiré après 2022-02) : version la plus proche 2018-12 (1.0)
- **Blade of Enmity** (BladeofEnmity, retiré après 2022-06) : version la plus proche 2018-12 (1.0)
- **Bleached Angling Kit** [fait] (BleachedAnglingKit, retiré après 2.0.1.005) : version la plus proche 2021-12 (1.0)
- **Blighted Lens** [fait] (BlightedLens, retiré après 2022-04) : version la plus proche 2018-12 (1.0)
- **Blood Clot Staff** [fait] (BloodClotStaff, retiré après 2.0.1.005) : version la plus proche 2018-12 (1.0)
- **Blood Relic** (BloodIdol, retiré après 2022-04) : version la plus proche 2022-01 (1.0)
- **Blood Sample** [fait] (BloodSample, retiré après 2.0.1.005) : version la plus proche 2018-12 (1.0)
- **Bloodletting Essence** [fait] (BloodlettingEssence, retiré après 2020-11) : version la plus proche 2018-12 (1.0)
- **Bloodworm** (BloodwormItem, retiré après 2.0.1.005) : version la plus proche 2020-04 (1.0)
- **Bloody Edge** (BloodyEdge, retiré après 2.0.1.005) : version la plus proche 2021-02 (1.0)
- **Bouncing Betty** (BouncingBetty, retiré après 2022-06) : version la plus proche 2019-12 (1.0)
- **Bouncy Spiky Ball** (BouncySpikyBall, retiré après 2.0.1.005) : version la plus proche 2020-01 (1.0)
- **Brimlash Buster** (BrimlashBuster, retiré après 2022-06) : version la plus proche 2019-08 (1.0)
- **Brimstone Fish** [fait] (BrimstoneFish, retiré après 2.0.1.005) : version la plus proche 2019-12 (1.0)
- **Brimstone Flameblaster** (BrimstoneFlameblaster, retiré après 2022-06) : version la plus proche 2019-08 (1.0)
- **Burden Breaker** (GoldBurdenBreaker, retiré après 2022-01) : version la plus proche 2021-05 (1.0)
- **Cadance Potion** (CadancePotion, CadencePotion, retiré après 2.0.1.005) : version la plus proche 2020-08 (0.95)
- **Calamitas' Brew** (CalamitasBrew, retiré après 2022-10) : version la plus proche 2022-04 (1.0)
- **Carnage Ray** [fait] (CarnageRay, retiré après 2.0.1.005) : version la plus proche 2021-07 (1.0)
- **Catastrophe Claymore** (CatastropheClaymore, retiré après 2.0.1.005) : version la plus proche 2019-08 (1.0)
- **Chaotic Brick Wall** (ChaoticBrickWall, retiré après 2022-09) : version la plus proche 2022-07 (1.0)
- **Charged Dart Blaster** (ChargedDartRifle, retiré après 2022-06) : version la plus proche 2020-04 (1.0)
- **Charger Test Item** (ChargerTestItem, retiré après 2020-10) : version la plus proche 2020-09 (0.99)
- **Clamor Noctus** (ClamorNoctus, retiré après 2.0.1.005) : version la plus proche 2020-01 (1.0)
- **Cloaking Gland** [fait] (CloakingGland, retiré après 2020-11) : version la plus proche 2018-12 (1.0)
- **Conclave Crossfire** (ConferenceCall, retiré après 2.0.1.005) : version la plus proche 2019-08 (1.0)
- **Coral Cannon** (CoralCannon, retiré après 2.0.1.005) : version la plus proche 2019-04 (1.0)
- **Core of Chaos** [fait] (CoreofChaos, retiré après 2.0.1.005) : version la plus proche 2018-12 (1.0)
- **Core of Eleum** [fait] (CoreofEleum, retiré après 2.0.1.005) : version la plus proche 2018-12 (1.0)
- **Core of Sunlight** [fait] (CoreofCinder, CoreofSunlight, retiré après 2.0.1.005) : version la plus proche 2018-12 (1.0)
- **Corrupted Crusher Blade** (CorruptedCrusherBlade, retiré après 2022-06) : version la plus proche 2021-08 (1.0)
- **Crimsaber** (Crimsaber, retiré après 2019-04) : version la plus proche 2018-12 (1.0)
- **Crimslime Staff** [fait] (CrimslimeStaff, retiré après 2.0.1.005) : version la plus proche 2018-12 (1.0)
- **Crimson Crusher Blade** (CrimsonCrusherBlade, retiré après 2022-06) : version la plus proche 2019-08 (1.0)
- **Crumbling Potion** (CrumblingPotion, retiré après 2.0.1.005) : version la plus proche 2019-06 (1.0)
- **Crystal Blade** (CrystalBlade, retiré après 2022-06) : version la plus proche 2019-08 (1.0)
- **Crystal Flare Staff** (CrystalFlareStaff, retiré après 2022-06) : version la plus proche 2018-12 (1.0)
- **Cursed Capper** (CursedCapper, retiré après 2.0.1.005) : version la plus proche 2019-08 (1.0)
- **Daylight** (Daylight, retiré après 2021-03) : version la plus proche 2018-12 (1.0)
- **Death** (Death, retiré après 2022-06) : version la plus proche 2021-11 (0.94)
- **Defiled Feather** (DefiledRune, retiré après 2022-01) : version la plus proche 2021-03 (1.0)
- **Demon Trophy** (MLGRune, retiré après 2022-01) : version la plus proche None (0.0)
- **Demonic Bone Ash** [fait] (DemonicBoneAsh, retiré après 2.0.1.005) : version la plus proche 2018-12 (1.0)
- **Demonic Pitchfork** (DemonicPitchfork, retiré après 2022-01) : version la plus proche 2018-12 (1.0)
- **Desert Feather** [fait] (DesertFeather, retiré après 2.0.1.005) : version la plus proche 2018-12 (1.0)
- **Determination Breaker** (BurdenBreaker, retiré après 2021-11) : version la plus proche 2018-12 (1.0)
- **Devastation** (Devastation, retiré après 2.0.1.005) : version la plus proche 2019-08 (1.0)
- **Diseased Pike** [fait] (DiseasedPike, retiré après 2.0.1.005) : version la plus proche 2019-10 (1.0)
- **Disseminator** [fait] (Disseminator, Interfacer, retiré après 2.0.1.005) : version la plus proche 2019-08 (1.0)
- **Divine Retribution** (DivineRetribution, retiré après 2.0.1.005) : version la plus proche 2019-02 (1.0)
- **Dodu's Handcannon** (DodusHandcannon, retiré après 2.0.1.005) : version la plus proche 2021-03 (1.0)
- **Draconic Elixir** (DraconicElixir, retiré après 2.0.1.005) : version la plus proche 2021-07 (1.0)
- **Draedon's Remote** (DraedonsRemote, retiré après 2022-04) : version la plus proche 2021-11 (1.0)
- **Dualpoon** (Dualpoon, retiré après 2022-06) : version la plus proche 2019-08 (1.0)
- **Ectoblood** [fait] (Ectoblood, retiré après 2021-12) : version la plus proche 2018-12 (1.0)
- **Elemental Excalibur** (ElementalExcalibur, retiré après 2.0.1.005) : version la plus proche 2019-11 (1.0)
- **Enchanted Metal** [fait] (EnchantedMetal, retiré après 2021-07) : version la plus proche 2018-12 (1.0)
- **Enhanced Nano Round** (EnhancedNanoRound, retiré après 2.0.1.005) : version la plus proche 2018-12 (1.0)
- **Eutrophic Scimitar** [fait] (EutrophicScimitar, retiré après 2.0.1.005) : version la plus proche 2019-04 (1.0)
- **Explosive Shotgun Shell** (ExplosiveShells, retiré après 2.0.1.005) : version la plus proche 2019-04 (1.0)
- **Fetid Essence** [fait] (FetidEssence, retiré après 2020-11) : version la plus proche 2018-12 (1.0)
- **Fish of Light** (FishofLight, retiré après 2.0.1.005) : version la plus proche 2019-12 (1.0)
- **Fish of Night** (FishofNight, retiré après 2.0.1.005) : version la plus proche 2019-12 (1.0)
- **Frigid Bar** (CryoBar, retiré après 2021-03) : version la plus proche 2018-12 (1.0)
- **Frigid Bulwark** (FrigidBulwark, retiré après 2022-04) : version la plus proche 2018-12 (1.0)
- **Frostspark Bullet** (FrostsparkBullet, retiré après 2021-12) : version la plus proche 2018-12 (1.0)
- **Fulguration Halberd** (FulgurationHalberd, retiré après 2022-06) : version la plus proche 2019-08 (1.0)
- **Fungal Carapace** [fait] (FungalCarapace, retiré après 2.0.1.005) : version la plus proche 2018-12 (1.0)
- **Galactica Singularity** [fait] (GalacticaSingularity, retiré après 2.0.1.005) : version la plus proche 2018-12 (1.0)
- **Gatling Laser** (GatlingLaser, retiré après 2.0.1.005) : version la plus proche 2022-04 (1.0)
- **Gauss Dagger** (GaussDagger, retiré après 2.0.1.005) : version la plus proche 2020-08 (1.0)
- **Gauss Pistol** (GaussPistol, retiré après 2.0.1.005) : version la plus proche 2020-08 (1.0)
- **Gehenna** [fait] (Gehenna, retiré après 2.0.1.005) : version la plus proche 2018-12 (1.0)
- **Glacial Crusher** [fait] (GlacialCrusher, retiré après 2022-06) : version la plus proche 2022-01 (1.0)
- **Golden Gun** (GoldenGun, retiré après 2.0.1.005) : version la plus proche 2019-08 (1.0)
- **Grenade Shells** (GrenadeRounds, retiré après 2.0.1.005) : version la plus proche 2019-02 (1.0)
- **Gypsy Powder** [fait] (GypsyPowder, retiré après 2020-11) : version la plus proche 2018-12 (1.0)
- **Hadarian Membrane** [fait] (HadarianMembrane, retiré après 2022-06) : version la plus proche 2019-04 (1.0)
- **Hadopelagic Echo** (HadopelagicEcho, retiré après 2022-06) : version la plus proche 2022-02 (1.0)
- **Heart Rapier** (HeartRapier, retiré après 2020-05) : version la plus proche 2019-08 (1.0)
- **Hell Burst** (HellBurst, retiré après 2022-06) : version la plus proche 2018-12 (1.0)
- **Hell's Sun** (HellsSun, retiré après 2.0.1.005) : version la plus proche 2019-12 (1.0)
- **Holiday Halberd** (HolidayHalberd, retiré après 2021-08) : version la plus proche 2019-12 (1.0)
- **Holy Wrath Potion** (HolyWrathPotion, retiré après 2.0.1.005) : version la plus proche 2022-02 (1.0)
- **IOU an item** [fait] (IOU, retiré après 2019-11) : version la plus proche 2018-12 (1.0)
- **Impaler** (Impaler, retiré après 2022-06) : version la plus proche 2019-10 (1.0)
- **Iron Heart** (IronHeart, retiré après 2022-01) : version la plus proche 2020-12 (1.0)
- **Latcher Mine** (LatcherMine, retiré après 2022-06) : version la plus proche 2020-01 (1.0)
- **Lazhar** [fait] (Lazhar, retiré après 2.0.1.005) : version la plus proche 2019-08 (1.0)
- **Lazinator** (Zapper, retiré après 2022-06) : version la plus proche 2018-12 (1.0)
- **Lead Core** [fait] (LeadCore, retiré après 2022-04) : version la plus proche 2018-12 (1.0)
- **Lead Wizard** (LeadWizard, retiré après 2022-01) : version la plus proche 2019-06 (1.0)
- **Lightning Hawk** (LightningHawk, retiré après 2.0.1.005) : version la plus proche 2022-02 (1.0)
- **Lul** (CheatTestThing, retiré après 2022-06) : version la plus proche 2022-02 (0.91) ; introuvable : Equipable ; **Revue** : Objet de test de Calamity (CheatTestThing) ; « Equipable » est une étiquette du wiki.
- **Luminous Striker** (LuminousStriker, retiré après 2.0.1.005) : version la plus proche 2020-04 (1.0)
- **Magna Striker** (MagnaStriker, retiré après 2.0.1.005) : version la plus proche 2018-12 (1.0)
- **Magnum** (Magnum, retiré après 2.0.1.005) : version la plus proche 2022-02 (1.0)
- **Magnum Rounds** (MagnumRounds, retiré après 2.0.1.005) : version la plus proche 2019-02 (1.0)
- **Mandible Bow** (MandibleBow, retiré après 2022-06) : version la plus proche 2018-12 (1.0)
- **Mandible Claws** (MandibleClaws, retiré après 2022-06) : version la plus proche 2018-12 (1.0)
- **Maneater Bulb** [fait] (ManeaterBulb, retiré après 2020-11) : version la plus proche 2018-12 (1.0)
- **Mariana** (Mariana, retiré après 2022-06) : version la plus proche 2020-12 (1.0)
- **Marked Magnum** (MarkedMagnum, retiré après 2022-06) : version la plus proche 2019-08 (1.0)
- **Marnite Bayonet** (MarniteBayonet, MarniteRifleSpear, retiré après 2022-07) : version la plus proche 2022-07 (1.0)
- **Marnite Spear** (MarniteSpear, retiré après 2022-07) : version la plus proche 2018-12 (1.0)
- **Meld Bar** [fait] (MeldiateBar, retiré après 2022-06) : version la plus proche 2018-12 (1.0)
- **Molecular Manipulator** (MolecularManipulator, retiré après 2.0.1.005) : version la plus proche 2021-04 (1.0)
- **Monkey Darts** (MonkeyDarts, retiré après 2.0.1.005) : version la plus proche 2020-10 (1.0)
- **Moonlight** (Moonlight, retiré après 2021-03) : version la plus proche 2018-12 (1.0)
- **Murky Paste** [fait] (MurkyPaste, retiré après 2.0.1.005) : version la plus proche 2018-12 (1.0)
- **Murky Sludge** [fait] (MurkySludge, retiré après 2022-02) : version la plus proche 2018-12 (1.0)
- **Nychthemeron** (Nychthemeron, retiré après 2.0.1.005) : version la plus proche 2020-01 (0.95)
- **Old Power Cell** (OldPowerCell, retiré après 2.0.1.005) : version la plus proche 2021-11 (1.0)
- **Penumbra Potion** (PenumbraPotion, retiré après 2.0.1.005) : version la plus proche 2020-03 (0.99) ; introuvable : rogue stealth generates 15% faster while movingAt night, stealth also generates 15% faster while standing stillDuring an eclipse both boosts increase to 20% ; **Revue** : Infobulle de 2020 dont les retours à la ligne manquent sur le Fandom.
- **Phantom Lance** [fait] (PhantomLance, retiré après 2.0.1.005) : version la plus proche 2020-01 (1.0)
- **Phoenix Blade** (PhoenixBlade, retiré après 2022-06) : version la plus proche 2019-04 (1.0)
- **Piggy** [fait] (PiggyItem, retiré après 2.0.1.005) : version la plus proche 2020-02 (1.0)
- **Plague Hive** (PlagueHive, retiré après 2.0.1.005) : version la plus proche 2022-02 (1.0)
- **Plague Keeper** (PlagueKeeper, retiré après 2.0.1.005) : version la plus proche 2019-08 (1.0)
- **Poison Pack** (PoisonPack, retiré après 2.0.1.005) : version la plus proche 2020-01 (1.0)
- **Profaned Rage Potion** (ProfanedRagePotion, retiré après 2.0.1.005) : version la plus proche 2022-02 (1.0)
- **Psychotic Amulet** [fait] (PsychoticAmulet, retiré après 2022-06) : version la plus proche 2019-06 (1.0)
- **Purified Jam** (PurifiedJam, retiré après 2022-06) : version la plus proche 2021-09 (1.0)
- **Quasar** (Quasar, retiré après 2022-01) : version la plus proche 2020-06 (1.0)
- **Raider's Glory** (RaidersGlory, retiré après 2022-06) : version la plus proche 2020-04 (1.0)
- **Reaver Helmet** (ReaverHelmet, retiré après 2020-12) : version la plus proche 2019-02 (1.0)
- **Reaver Mask** (ReaverMask, retiré après 2020-12) : version la plus proche 2019-08 (0.99)
- **Redtide Spear** (RedtideSpear, retiré après 2.0.1.005) : version la plus proche 2022-07 (1.0)
- **Redtide Sword** (RedtideSword, retiré après 2022-06) : version la plus proche 2019-06 (1.0)
- **Revengeance** (Revenge, retiré après 2022-06) : version la plus proche 2022-03 (0.76)
- **Revivify Potion** (RevivifyPotion, retiré après 2022-06) : version la plus proche 2019-11 (1.0)
- **Rotten Matter** [fait] (RottenMatter, TrueShadowScale, retiré après 2.0.1.005) : version la plus proche 2018-12 (1.0)
- **Rusty Medallion** (RustyMedallion, retiré après 2.0.1.005) : version la plus proche 2020-05 (1.0)
- **Sandslasher** [fait] (Sandslasher, retiré après 2.0.1.005) : version la plus proche 2020-06 (1.0)
- **Sandy Angling Kit** [fait] (SandyAnglingKit, retiré après 2.0.1.005) : version la plus proche 2021-12 (1.0)
- **Scarred Angelfish** [fait] (ScarredAngelfish, retiré après 2022-07) : version la plus proche 2019-12 (1.0)
- **Sea Minnow** (SeaMinnowItem, retiré après 2.0.1.005) : version la plus proche 2020-02 (1.0)
- **Sea Shell** [fait] (SeaShell, retiré après 2022-06) : version la plus proche 2018-12 (1.0)
- **Seabow** (Seabow, retiré après 2022-06) : version la plus proche 2019-08 (1.0)
- **Seashell Boomerang** (SeashellBoomerang, SeashellBoomerangMelee, retiré après 2022-06) : version la plus proche 2021-01 (1.0)
- **Shadowdrop Staff** (ShadowdropStaff, retiré après 2022-06) : version la plus proche 2018-12 (1.0)
- **Shattering Potion** (ShatteringPotion, retiré après 2022-10) : version la plus proche 2019-08 (1.0)
- **Shroomerang** (Shroomerang, retiré après 2022-06) : version la plus proche 2019-12 (1.0)
- **Shroomite Visage** (ShroomiteVisage, retiré après 2.0.1.005) : version la plus proche 2018-12 (1.0)
- **Skullmasher** (Skullmasher, retiré après 2022-01) : version la plus proche 2021-05 (1.0)
- **Sky Stabber** (SkyStabber, retiré après 2.0.1.005) : version la plus proche 2020-06 (1.0)
- **Soul Edge** [fait] (SoulEdge, retiré après 2.0.1.005) : version la plus proche 2018-12 (1.0)
- **Spectre Rifle** (SpectreRifle, retiré après 2022-10) : version la plus proche 2021-03 (1.0)
- **Star Cannon EX** (StarCannonEX, retiré après 2022-06) : version la plus proche 2019-08 (1.0)
- **Sticky Spiky Ball** (StickySpikyBall, retiré après 2.0.1.005) : version la plus proche 2019-12 (1.0)
- **Storm Dragoon** [fait] (StormDragoon, retiré après 2.0.1.005) : version la plus proche 2020-05 (1.0)
- **Superball Bullet** (SuperballBullet, retiré après 2.0.1.005) : version la plus proche 2018-12 (1.0)
- **Sylvan Slasher** (SylvanSlasher, retiré après 2022-06) : version la plus proche 2020-01 (1.0)
- **Temporal Floe Sword** (TemporalFloeSword, retiré après 2022-06) : version la plus proche 2020-05 (1.0)
- **Tenebris** [fait] (Tenebris, retiré après 2.0.1.005) : version la plus proche 2018-12 (1.0)
- **Terra Edge** (TerraEdge, retiré après 2.0.1.005) : version la plus proche 2019-08 (1.0)
- **Terra-cotta** [fait] (Terracotta, retiré après 2022-06) : version la plus proche 2019-06 (1.0)
- **The Enforcer** (TheEnforcer, retiré après 2.0.1.005) : version la plus proche 2021-03 (1.0)
- **The Reaper** (TheReaper, retiré après 2022-10) : version la plus proche 2020-02 (1.0)
- **Titan Scale Potion** (TitanScalePotion, retiré après 2.0.1.005) : version la plus proche 2020-05 (1.0)
- **Tormented Mind of Malice** (MaliceModeItem, retiré après 2022-07) : version la plus proche 2022-07 (0.99) ; introuvable : Expert ; **Revue** : Objet du mode Malice (MaliceModeItem, retiré en 2022) ; « Expert » est une étiquette du wiki.
- **Trapper Bulb** [fait] (TrapperBulb, retiré après 2.0.1.005) : version la plus proche 2018-12 (1.0)
- **Trinity** (Trinity, retiré après 2022-06) : version la plus proche 2019-08 (1.0)
- **Triploon** (Triploon, retiré après 2022-06) : version la plus proche 2019-08 (1.0)
- **Triumph Potion** (TriumphPotion, retiré après 2.0.1.005) : version la plus proche 2019-06 (1.0)
- **True Bloody Edge** (TrueBloodyEdge, retiré après 2.0.1.005) : version la plus proche 2021-02 (1.0)
- **Twinkler** (TwinklerItem, retiré après 2.0.1.005) : version la plus proche 2019-11 (1.0)
- **Urchin Flail** (UrchinFlail, retiré après 2.0.1.005) : version la plus proche 2019-04 (1.0)
- **Urchin Spear** (UrchinSpear, retiré après 2022-06) : version la plus proche 2019-08 (1.0)
- **Verdant** (Verdant, retiré après 2.0.1.005) : version la plus proche 2020-03 (1.0)
- **Waraxe** (Waraxe, retiré après 2022-06) : version la plus proche 2019-06 (1.0)
- **Warbanner of the Sun** (SamuraiBadge, WarbanneroftheSun, retiré après 2.0.1.005) : version la plus proche 2022-05 (1.0)
- **Warblade** (Warblade, retiré après 2022-06) : version la plus proche 2019-06 (1.0)
- **Wrath of the Ancients** [fait] (WrathoftheAncients, retiré après 2.0.1.005) : version la plus proche 2020-10 (1.0)
- **Wulfrum Axe** (WulfrumAxe, retiré après 2022-07) : version la plus proche 2018-12 (1.0)
- **Wulfrum Blade** (WulfrumBlade, retiré après 2022-07) : version la plus proche 2018-12 (1.0)
- **Wulfrum Bow** (WulfrumBow, retiré après 2022-07) : version la plus proche 2018-12 (1.0)
- **Wulfrum Hammer** (WulfrumHammer, retiré après 2022-07) : version la plus proche 2018-12 (1.0)
- **Wulfrum Staff** (WulfrumStaff, retiré après 2022-07) : version la plus proche 2019-08 (1.0)
- **Wulfrum armor** (WulfrumArmor, retiré après 2022-07) : version la plus proche None (0.0)
- **Yanmei's Knife** (YanmeisKnife, retiré après 2.0.1.005) : version la plus proche 2020-09 (1.0)
- **Yharim's Stimulants** (YharimsStimulants, retiré après 2.0.1.005) : version la plus proche 2018-12 (1.0)
