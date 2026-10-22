# H03 - Schoolmaster's House (Penrose)

> Machine-readable twin: `docs/design/houses/H03.json` - the builder reads THAT file; this page explains it. Every number here comes from the same data and is validated (no overlaps, door clearances, required items).

| | |
|---|---|
| **Archetype** | Victorian Gothic stone house attached to the schoolyard wall |
| **Silhouette (must read at 50 m in fog)** | Steep front-facing GABLE with carved bargeboards and a finial, pointed-arch windows, two tall diagonal-set brick chimneys, a little wooden bell-cote on the rear wing ridge, white door in a stone arch. |
| **Ritual part** | Bell clapper |
| **M1 puzzle rig (code)** | `PuzzleKind.CodeLock` |
| **Letter traits (HouseCatalog)** | door **White**, **2** chimney(s), stopped clock **8:00**, family **Penrose** |
| **World position** | x 56.0, z 58.0, floor level y +0.20, yaw 0 deg - School Lane (south side, opposite the school) |
| **Front door (world)** | (54.4, 63.0) - 73.2 m from the altar, 4.7 m from the lane |
| **Placement note** | faces the schoolhouse across School Lane |

## Reference images

![plan](../ref/H03_plan.png)

![front-left](../ref/H03_massing_front.png) ![back-right](../ref/H03_massing_back.png)

Real-world references (look, don't copy): [Gothic Revival architecture](https://en.wikipedia.org/wiki/Gothic_Revival_architecture), [Bargeboard](https://en.wikipedia.org/wiki/Bargeboard)

Mood-image prompt (Higgsfield / Gemini / ChatGPT image - save the result as `docs/design/ref/H03_mood.png`):

```text
Victorian Gothic stone schoolmaster's house, steep front-facing gable with carved green bargeboards and a finial, pointed-arch windows, a white boarded door with iron strap hinges, two tall diagonal brick chimneys, next to a small village schoolyard wall, photorealistic, late 1970s rural English village, overcast autumn dusk, damp, muted greens and greys, 35mm film still, Phasmophobia / Resident Evil Village mood, no people, eye-level, 16:9
```

## 1. Structure

### Levels
| Level | Floor y (m) | Clear height (m) |
|---|---|---|
| G - Ground floor | +0.00 | 3.00 |
| F - First floor | +3.25 | 2.70 |
| A - Attic (under the roof) | +6.20 | 2.30 |

Floor build-up: 0.25 m between floors (joists + boards); ceilings use the room's `ceiling` material.

### Wings (massing)
| Wing | Rect x0,z0 -> x1,z1 | Size | Levels | Walls | Wall thickness | Roof |
|---|---|---|---|---|---|---|
| main | (-3.5, -3.0) -> (3.5, 5.0) | 7.0 x 8.0 | G, F, A | StoneBlock (grey squared rubble) with SandstoneBlocks quoins and pointed-arch dressings | 0.5 | Steep 55 deg gable facing the street (ridge along z), RoofSlateGrey, carved bargeboards (dark green), finial at the apex |
| rear | (-6.5, -9.0) -> (3.5, -3.0) | 10.0 x 6.0 | G, F | StoneBlock front, BrickMixed back wall | 0.5 | Gable roof, ridge along x at 8.3 m, RoofSlateGrey; small timber bell-cote (Bell scaled to 0.4 m) on the ridge at x=-4 |

### Chimneys (count must equal HouseCatalog = 2)
| x | z | stack w x d | above ridge | pots | serves |
|---|---|---|---|---|---|
| -6.5 | -6.0 | 0.7 x 0.7 | 1.6 | 1 | kitchen range = THE HEARTH (diagonal-set shaft) |
| 3.5 | 0.5 | 0.7 x 0.7 | 1.6 | 1 | study fireplace (cold) |

### Exterior details
- steep front gable with carved dark-green bargeboards and a finial; trefoil cut-outs
- pointed-arch white door with iron strap hinges in a sandstone arch
- two tall diagonal-set brick chimney shafts (DESIGN: 2 chimneys)
- small wooden bell-cote on the rear wing ridge - the bell is GONE (its clapper is the ritual part)
- a gate in the back wall straight into the SCHOOLYARD

### Plot, garden and yard
- Front: 4 m garden, holly bush, iron railings on a low wall
- Back: yard shared with the schoolyard wall (2.2 m), outside coal store, a bicycle (rusty), a sundial with the gnomon snapped
- Plot boundary (world polygon): (47.5,43.0), (61.5,43.0), (61.5,65.0), (47.5,65.0). Boundary = DryStoneWall / FencePicket on the street side, FenceRail or hedges (Shrub1-4) elsewhere, a GardenGate on every path.

## 2. Rooms

| Room | Level | Rect (x0,z0,x1,z1) | Clear inside | Kind | Floor | Walls | Ceiling | Light |
|---|---|---|---|---|---|---|---|---|
| **hall** Hall | G | (-3.5, 1.4, 0.3, 5.0) | 3.24 x 3.04 | hall | FloorWornTile | Wallpaper2 over PanelDark dado | CeilingInterior | pendant |
| **study** Study (the PUZZLE room) | G | (0.3, -3.0, 3.5, 5.0) | 2.64 x 7.44 | study | FloorParquet | PanelDark to 2.0 m, Wallpaper2C | CeilingInterior | green-shaded DeskLamp + pendant |
| **sitting** Sitting room | G | (-3.5, -3.0, 0.3, 1.4) | 3.24 x 4.28 | living | Carpet70sB | WallpaperJacquard | CeilingInterior | pendant |
| **kitchen** Kitchen | G | (-6.5, -9.0, -1.5, -3.0) | 4.44 x 5.00 | kitchen | FloorQuarryTile | Whitewash | CeilingInterior | pendant |
| **pantry** Pantry & back passage | G | (-1.5, -9.0, 3.5, -3.0) | 4.44 x 5.44 | utility | FloorBrick | Whitewash | CeilingInterior | pendant |
| **bed1** Schoolmaster's bedroom | F | (0.3, -3.0, 3.5, 5.0) | 2.64 x 7.44 | bedroom | FloorDarkPlanks | Wallpaper1 | CeilingInterior | pendant |
| **landing** Landing | F | (-3.5, -3.0, 0.3, 5.0) | 3.24 x 7.44 | landing | FloorOldWood1 | Wallpaper2 | CeilingInterior | pendant |
| **bed2** Spare bedroom (lodger) | F | (-6.5, -9.0, -1.5, -3.0) | 4.44 x 5.00 | bedroom | FloorOldWood2 | Wallpaper1B | CeilingInterior | pendant |
| **bath** Bathroom | F | (-1.5, -9.0, 3.5, -3.0) | 4.44 x 5.44 | bathroom | FloorBathTile | TileWallWhite | CeilingInterior | pendant |
| **attic** Attic (old schoolroom stores) | A | (-3.5, -3.0, 3.5, 5.0) | 6.00 x 7.00 | attic | FloorOldWood2 | RawPlankWall | CeilingInterior | pendant |

Skirting: 0.15 m dark oak in every room except kitchens/sculleries/cellars (none) and bathrooms (tile). Every ground-floor room has one LightSwitch by its main door at 1.3 m; every room has the existing `Room` component.

### Doors (how the rooms connect)
| Door | Level | From -> to | Wall | Centre at | W x H | Leaf | Role |
|---|---|---|---|---|---|---|---|
| d_front | G | hall -> ext | N | -1.6 | 1.0 x 2.3 | pointed-arch boarded door with iron strap hinges (White) | front |
| d_back | G | pantry -> ext | S | 1.8 | 0.85 x 2.05 | panel - to the schoolyard gate | back |
| d_study | G | hall -> study | E | 3.6 | 0.9 x 2.05 | panel |  |
| d_sit | G | hall -> sitting | S | -0.6 | 0.9 x 2.05 | panel |  |
| d_kit | G | sitting -> kitchen | S | -2.0 | 0.85 x 2.05 | panel |  |
| d_pan | G | kitchen -> pantry | E | -6.0 | 0.85 x 2.05 | panel |  |
| d_study2 | G | study -> pantry | S | 0.9 | 0.85 x 2.05 | baize-covered door |  |
| d_b1 | F | landing -> bed1 | E | 3.6 | 0.85 x 2.05 | panel |  |
| d_b2 | F | landing -> bed2 | S | -2.0 | 0.8 x 2.05 | panel |  |
| d_bath | F | landing -> bath | S | -0.5 | 0.75 x 2.05 | panel |  |

### Windows
| Window | Room | Wall | Centre at | W x H | Sill | Style | Weak | Curtains / shutters |
|---|---|---|---|---|---|---|---|---|
| w_study | study | N | 1.9 | 1.3 x 2.0 | 0.8 | tall pointed-arch, 2 lights with a quatrefoil |  |   |
| w_hall | hall | W | 3.2 | 0.7 x 1.4 | 0.9 | lancet |  |   |
| w_sit | sitting | W | -0.8 | 1.2 x 1.5 | 0.9 | sash, cracked | **WEAK** (break-in point) |   |
| w_study2 | study | E | -1.0 | 1.0 x 1.6 | 0.9 | lancet pair |  |   |
| w_k | kitchen | S | -4.0 | 1.1 x 1.2 | 0.9 | sash, rotten | **WEAK** (break-in point) |   |
| w_b1 | bed1 | N | 1.9 | 1.0 x 1.5 | 0.9 | pointed arch |  |   |
| w_b2 | bed2 | W | -6.0 | 1.0 x 1.2 | 0.9 | sash |  |   |
| w_bath | bath | S | 1.0 | 0.7 x 0.8 | 1.3 | frosted |  |   |
| w_att | attic | N | 0.0 | 0.6 x 0.9 | 0.6 | slit window in the gable apex |  |   |

### Stairs, ladders and voids
| Id | In room | Rect | Goes | Up toward | Type |
|---|---|---|---|---|---|
| s1 | landing | (-3.0, -2.1, -2.1, 1.1) | G -> F | S | void (balustrade) |
| s1g | sitting | (-3.0, -2.1, -2.1, 1.1) | G -> F | S | open-string stair along the sitting room W wall - 14 risers, pitch pine |
| s2 | landing | (-0.6, 3.6, 0.2, 4.8) | F -> A | N | ladder to the attic hatch |

## 3. Every prop, room by room

Position = prop **pivot at the centre of its footprint, on the floor** (or on top of its parent for `on`, or the centre height `y` for wall-hung). Yaw = the way the prop's FRONT faces. Scale the asset so its bounds fit the listed size (see ASSETS.md 'Scaling rule').

### hall - Hall
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 14 | `Mirror` | Poly Haven `ornate_mirror_01` (1k) | -1.60 | 1.15 | 1.49 | 0 | 0.6 x 0.05 x 0.9 | wall-hung |  hall mirror |
| 15 | `HallConsole` | Poly Haven `ClassicConsole_01` (1k) | -2.40 | 0.00 | 1.69 | 0 | 1.1 x 0.4 x 0.82 | floor |   |
| 16 | `RotaryPhone` | Sketchfab `812e8f59f9e749fab483eabf4800dec2` (CC BY, sara El-sisi) | -2.40 | 0.82 | 1.69 | 0 | 0.22 x 0.25 x 0.14 | on H03.p015 | phone  |
| 17 | `FuseBox` | Poly Haven `power_box_01` (1k) | -3.00 | 1.50 | 4.42 | 180 | 0.45 x 0.15 x 0.6 | wall-hung | fuse-box FUSE BOX over the front door |

### study - Study (the PUZZLE room)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 1 | `OfficeDesk` | Poly Haven `metal_office_desk` (2k) | 2.60 | 0.00 | 0.50 | 270 | 1.4 x 0.75 x 0.76 | floor |  partner's desk; drawer locked |
| 2 | `DeskLamp` | Poly Haven `desk_lamp_arm_01` (1k) | 2.60 | 0.76 | 1.00 | 270 | 0.25 x 0.4 x 0.5 | on H03.p001 |   |
| 3 | `Globe` | Sketchfab `8bb166a68f70426390eb113ff0fd72a0` (CC BY, Allan-Jay Branscombe) | 2.60 | 0.76 | 0.00 | 270 | 0.4 x 0.4 x 0.55 | on H03.p001 |   |
| 4 | `Slate` | BUILD (PropKit) | 2.60 | 0.76 | 0.50 | 270 | 0.3 x 0.02 x 0.22 | on H03.p001 | puzzle THE SLATE: 5-letter wooden frame; chalk letters scattered round the house |
| 5 | `Shelf` | Poly Haven `wooden_bookshelf_worn` (2k) | 0.56 | 0.00 | 2.20 | 90 | 1.0 x 0.35 x 1.9 | floor |  floor-to-ceiling books |
| 6 | `Shelf` | Poly Haven `wooden_bookshelf_worn` (2k) | 0.56 | 0.00 | 1.10 | 90 | 1.0 x 0.35 x 1.9 | floor |  floor-to-ceiling books |
| 7 | `Shelf` | Poly Haven `wooden_bookshelf_worn` (2k) | 0.56 | 0.00 | 0.00 | 90 | 1.0 x 0.35 x 1.9 | floor |  floor-to-ceiling books |
| 8 | `DrawerCabinet` | Poly Haven `drawer_cabinet` (1k) | 2.75 | 0.00 | 3.00 | 270 | 0.6 x 0.45 x 1.1 | floor |   |
| 9 | `CabinetVintage` | Poly Haven `vintage_cabinet_01` (2k) | 2.40 | 0.00 | -2.69 | 0 | 1.0 x 0.45 x 1.8 | floor |  glass-fronted cupboard: the PART CONTAINER (4-dial lock) |
| 10 | `Armchair` | Poly Haven `ArmChair_01` (2k) | 2.00 | 0.00 | 3.30 | 160 | 0.85 x 0.85 x 0.95 | floor |   |
| 11 | `Rug` | BUILD (PropKit) | 1.90 | 0.00 | 1.00 | 0 | 2.0 x 1.4 x 0.01 | floor |   |
| 12 | `FrameFancy2` | Poly Haven `fancy_picture_frame_02` (1k) | 0.39 | 1.43 | -1.50 | 90 | 0.45 x 0.05 x 0.55 | wall-hung | family-photo FAMILY PHOTO: class of 1952 with Mr Penrose |
| 13 | `SideTable` | Poly Haven `side_table_01` (1k) | 2.60 | 0.00 | 4.10 | 0 | 0.5 x 0.5 x 0.6 | floor |   |

### sitting - Sitting room
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 18 | `Sofa` | Poly Haven `Sofa_01` (2k) | -0.24 | 0.00 | -0.80 | 270 | 2.0 x 0.9 x 0.85 | floor |   |
| 19 | `Armchair` | Poly Haven `ArmChair_01` (2k) | -1.30 | 0.00 | 0.10 | 110 | 0.85 x 0.85 x 0.95 | floor |   |
| 20 | `SideTable` | Poly Haven `side_table_01` (1k) | -1.70 | 0.00 | 1.06 | 180 | 0.5 x 0.5 x 0.6 | floor |   |
| 21 | `Radio` | Poly Haven `vintage_radio_transceiver` (1k) | -1.70 | 0.60 | 1.06 | 180 | 0.45 x 0.25 x 0.3 | on H03.p020 | radio  |
| 22 | `CoffeeTable` | Poly Haven `CoffeeTable_01` (1k) | -1.00 | 0.00 | -0.90 | 90 | 1.1 x 0.6 x 0.45 | floor |   |

### kitchen - Kitchen
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 23 | `Hearth` | BUILD (PropKit) | -5.62 | 0.00 | -6.00 | 90 | 1.8 x 0.7 x 1.4 | floor | hearth cast-iron range in an arched recess |
| 24 | `Dresser` | BUILD (PropKit) | -4.20 | 0.00 | -3.78 | 180 | 1.4 x 0.5 x 2.0 | floor |   |
| 25 | `TableWood1` | Poly Haven `WoodenTable_01` (1k) | -4.00 | 0.00 | -6.60 | 90 | 1.4 x 0.8 x 0.76 | floor |   |
| 26 | `ChairDining` | Poly Haven `dining_chair_02` (1k) | -4.70 | 0.00 | -6.60 | 90 | 0.45 x 0.5 x 0.95 | floor |   |
| 27 | `KitchenSinkUnit` | BUILD (PropKit) | -2.40 | 0.00 | -8.17 | 0 | 1.0 x 0.6 x 0.9 | floor |   |
| 28 | `Fridge` | Sketchfab `68d173f205344455933f861042b99fe3` (CC BY, dylanheyes) | -1.90 | 0.00 | -4.50 | 270 | 0.6 x 0.62 x 1.45 | floor |   |

### pantry - Pantry & back passage
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 29 | `SteelShelves2` | Poly Haven `steel_frame_shelves_02` (1k) | 2.77 | 0.00 | -5.00 | 270 | 1.0 x 0.4 x 1.8 | floor |   |
| 30 | `CabinetPainted` | Poly Haven `painted_wooden_cabinet` (2k) | 2.45 | 0.00 | -3.34 | 180 | 1.0 x 0.5 x 1.9 | floor |  walk-in coat cupboard |

### bed1 - Schoolmaster's bedroom
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 31 | `Bed` | Poly Haven `GothicBed_01` (2k) | 1.92 | 0.00 | 1.60 | 270 | 1.6 x 2.1 x 1.4 | floor |   |
| 32 | `Nightstand` | Poly Haven `ClassicNightstand_01` (1k) | 2.77 | 0.00 | 0.50 | 270 | 0.5 x 0.4 x 0.6 | floor |   |
| 33 | `Nightstand` | Poly Haven `ClassicNightstand_01` (1k) | 2.77 | 0.00 | 2.70 | 270 | 0.5 x 0.4 x 0.6 | floor |   |
| 34 | `Wardrobe` | Poly Haven `GothicCabinet_01` (2k) | 1.90 | 0.00 | -2.61 | 0 | 1.2 x 0.6 x 2.1 | floor |   |
| 35 | `Chair` | Poly Haven `WoodenChair_01` (2k) | 1.00 | 0.00 | -1.60 | 45 | 0.45 x 0.5 x 0.95 | floor |   |

### landing - Landing
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 39 | `ShrineNiche` | BUILD (PropKit) | 0.12 | 0.90 | -2.00 | 270 | 0.6 x 0.25 x 0.9 | wall-hung | shrine  |

### bed2 - Spare bedroom (lodger)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 36 | `BedIron` | Poly Haven `old_bed_frame` (2k) | -4.00 | 0.00 | -7.47 | 0 | 1.0 x 2.0 x 1.05 | floor |   |
| 37 | `Nightstand` | Poly Haven `ClassicNightstand_01` (1k) | -4.80 | 0.00 | -8.27 | 0 | 0.5 x 0.4 x 0.6 | floor |   |
| 38 | `ChestOfDrawers` | Poly Haven `vintage_wooden_drawer_01` (1k) | -1.83 | 0.00 | -4.50 | 270 | 0.9 x 0.48 x 1.0 | floor |   |

### bath - Bathroom
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 40 | `BathtubPhoto` | Sketchfab `ba60ba7d0a4a4b6899021741f450395b` (CC BY, Igrium) | 2.58 | 0.00 | -6.00 | 270 | 1.7 x 0.78 x 0.65 | floor |   |
| 41 | `BathCurtainRail` | BUILD (PropKit) | 2.80 | 0.00 | -6.00 | 270 | 1.7 x 0.8 x 2.0 | floor | allow-overlap  |
| 42 | `Sink` | Sketchfab `a278d63585494921906ab3500c421ff7` (CC BY, lagesnpiet) | 1.00 | 0.00 | -3.31 | 180 | 0.55 x 0.45 x 0.85 | floor |   |
| 43 | `Mirror` | Poly Haven `ornate_mirror_01` (1k) | 1.00 | 1.10 | -3.08 | 180 | 0.6 x 0.05 x 0.9 | wall-hung | mirror  |
| 44 | `Toilet` | Sketchfab `6ac515a1c4154db18b5b4bd0b46d6405` (CC BY, Allan-Jay Branscombe) | -1.08 | 0.00 | -8.00 | 90 | 0.4 x 0.65 x 0.8 | floor |   |

### attic - Attic (old schoolroom stores)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 45 | `SchoolDesk` | Poly Haven `SchoolDesk_01` (1k) | -1.50 | 0.00 | -2.22 | 0 | 0.65 x 0.5 x 0.75 | floor |   |
| 46 | `SchoolDesk` | Poly Haven `SchoolDesk_01` (1k) | 0.00 | 0.00 | -2.22 | 0 | 0.65 x 0.5 x 0.75 | floor |   |
| 47 | `SchoolDesk` | Poly Haven `SchoolDesk_01` (1k) | 1.50 | 0.00 | -2.22 | 0 | 0.65 x 0.5 x 0.75 | floor |   |
| 48 | `Chalkboard` | Poly Haven `standing_chalkboard_01` (1k) | -2.72 | 0.00 | 1.00 | 90 | 1.0 x 0.5 x 1.5 | floor |  'alphabet cipher' poster pinned to an old easel board (A=1 ... drives the code) |
| 49 | `CardboardBox` | Poly Haven `cardboard_box_01` (1k) | 2.77 | 0.00 | 0.00 | 270 | 0.5 x 0.4 x 0.4 | floor |   |

## 4. Gameplay anchors (empty GameObjects the builder must create)
| Kind | Room | x | y | z | What |
|---|---|---|---|---|---|
| puzzle | study | 2.60 | 0.76 | 0.50 | CodeLockPuzzle reskinned: spell the hidden word on the slate from chalk letters (letters -> digits via the 'alphabet cipher' poster); 4-dial cupboard opens |
| container | study | 2.40 | 0.00 | -2.69 | Bell clapper inside, wrapped in a school flag |
| item-spot | study | 2.60 | 0.60 | 4.10 | study side table |
| note-spot | study | 2.60 | 0.76 | 0.40 | school log book (note 1/3) |
| item-spot | kitchen | -5.62 | 1.40 | -6.00 | range mantel |
| note-spot | kitchen | -4.00 | 0.76 | -6.50 | letter from a pupil's mother (note 2/3) |
| hiding | pantry | 2.45 | 0.00 | -3.34 | coat cupboard -> HidingSpot on H03.p030 (CabinetPainted) |
| hiding | bed1 | 1.92 | 0.00 | 1.60 | under the bed -> HidingSpot on H03.p031 (Bed) - crouched |
| hiding | bed1 | 1.90 | 0.00 | -2.61 | wardrobe -> HidingSpot on H03.p034 (Wardrobe) |
| hiding | bed2 | -4.00 | 0.00 | -7.47 | under the lodger's bed -> HidingSpot on H03.p036 (BedIron) - crouched |
| shrine | landing | 0.12 | 0.00 | -2.00 | Rite of Return point (active only if this house is a shrine this match) |
| hiding | bath | 2.58 | 0.00 | -6.00 | behind the bath curtain -> HidingSpot on H03.p040 (BathtubPhoto) |
| note-spot | attic | -2.72 | 1.50 | 1.10 | cipher poster (note 3/3) |
| item-spot | attic | 2.77 | 0.40 | 0.00 | box of old exercise books |

Mapping to existing code: `item-spot` -> `House.ItemSpots` (order as listed), `note-spot` -> the 3 `Readable` notes, `puzzle` -> `PuzzleRig` anchor, `container` -> `PartContainer`, `shrine` -> `House.ShrineMarker` (+ shrine niche inside), `hiding` -> `HidingSpot`.

## 5. Puzzle - Bell clapper
1. LOCK FIRST: the glass-fronted study cupboard with the clapper visible inside, 4 dials.
2. A SLATE on the desk has 5 empty letter slots; 5 chalk letter tiles are scattered (item spots + 2 fixed places).
3. The word is randomised from a 5-letter pool (BELLS, CHALK, HYMNS, GRAVE, CHILD...). Clue: the school log book (note 1/3).
4. The attic 'alphabet cipher' poster maps letters to numbers; the cupboard code = numbers of letters 1, 2, 4, 5 (mod 10).
5. Wrong code: a ruler slaps the desk (clunk noise).

## 6. Night changes (DESIGN 'Night changes' - pick 2-3 per match)
- all three attic desks have been turned to face the hatch
- the study clock strikes 8 at the wrong time
- chalk writing on the hall wall: the player's name, spelled wrong
- plus the shared pool: extra person in the family photo, a door opening onto a brick wall, moved furniture, an extra hallway door, all clocks stopped at the same time, writing on a wall in a player's name

## 7. Sound
- books: page turns, books falling (study)
- a child's voice reciting the alphabet through the floor (ghostly)
- Room tone everywhere: existing `Audio/Ambience/RoomTone.wav`; hearth: `HearthFire.wav`; Sonniss sources in `C:/dev/Hollowmere/AssetDownloads/Sonniss.com-GDC2026-GameAudioBundle` (see ASSETS.md).

## 8. Build checklist for this house
- [ ] Shell from wings + rooms (walls with door/window openings at the exact positions above), floors per level, roofs + chimneys.
- [ ] Every door: `Door` component on exterior doors (front/back/side); interior doors are hinged leaves (NavMesh: ignore leaf, carving obstacle).
- [ ] Weak windows: the two marked **WEAK** get the weak-window component (DESIGN break-in points).
- [ ] All props from the tables: correct asset key, position, yaw, scale; hiding/puzzle/container/shrine anchors wired to components.
- [ ] Hearth = the prop with role `hearth` (exactly one); `hearth-decor` props are cold fireplaces with no Hearth component.
- [ ] Exactly one fuse box with role `fuse-box` wired to `FuseBox` (extra boxes are dressing).
- [ ] Phone, mirror, family photo(s), TV/radio present (DESIGN shared template).
- [ ] Run `LayoutFingerprint.Record` only after the user approves the new layout (CLAUDE.md).
- [ ] Take `shot` images: front, back, every room from its main door; compare with the plan + massing images here.
