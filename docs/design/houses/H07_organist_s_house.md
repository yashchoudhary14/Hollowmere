# H07 - Organist's House (Venn)

> Machine-readable twin: `docs/design/houses/H07.json` - the builder reads THAT file; this page explains it. Every number here comes from the same data and is validated (no overlaps, door clearances, required items).

| | |
|---|---|
| **Archetype** | Tall gaunt Gothic stone house beside the chapel, with an organ bay |
| **Silhouette (must read at 50 m in fog)** | Narrow, tall and dark: steep 55 deg red-slate roof, THREE chimneys (both gables and the rear wing), lancet windows, a stone ORGAN BAY with a pointed window projecting toward the chapel, a red door under a hood mould, ivy over half the front. |
| **Ritual part** | Organ pipe |
| **M1 puzzle rig (code)** | `PuzzleKind.Sequence` (loud) |
| **Letter traits (HouseCatalog)** | door **Red**, **3** chimney(s), stopped clock **7:00**, family **Venn** |
| **World position** | x -42.0, z 82.0, floor level y +0.30, yaw 270 deg - Chapel Lane (east side) |
| **Front door (world)** | (-46.0, 82.0) - 54.9 m from the altar, 6.3 m from the lane |
| **Placement note** | organ bay (local +x) points north toward the chapel; rear wing backs onto Back Lane |

## Reference images

![plan](../ref/H07_plan.png)

![front-left](../ref/H07_massing_front.png) ![back-right](../ref/H07_massing_back.png)

Real-world references (look, don't copy): [Pump organ](https://en.wikipedia.org/wiki/Pump_organ), [Gothic Revival architecture](https://en.wikipedia.org/wiki/Gothic_Revival_architecture)

Mood-image prompt (Higgsfield / Gemini / ChatGPT image - save the result as `docs/design/ref/H07_mood.png`):

```text
tall gaunt dark grey Gothic stone house beside a village chapel, very steep red slate roof with three chimneys, lancet windows, a stone organ bay with a tall pointed window, a red pointed-arch door, ivy over half the front, photorealistic, late 1970s rural English village, overcast autumn dusk, damp, muted greens and greys, 35mm film still, Phasmophobia / Resident Evil Village mood, no people, eye-level, 16:9
```

## 1. Structure

### Levels
| Level | Floor y (m) | Clear height (m) |
|---|---|---|
| C - Cellar | -2.40 | 2.20 |
| G - Ground floor | +0.00 | 3.20 |
| F - First floor | +3.45 | 2.80 |

Floor build-up: 0.25 m between floors (joists + boards); ceilings use the room's `ceiling` material.

### Wings (massing)
| Wing | Rect x0,z0 -> x1,z1 | Size | Levels | Walls | Wall thickness | Roof |
|---|---|---|---|---|---|---|
| main | (-4.5, -4.0) -> (4.5, 4.0) | 9.0 x 8.0 | C, G, F | StoneBlock (dark grey, ivy on the west half), SandstoneBlocks hood moulds and lancet surrounds, BrickDark plinth | 0.5 | Steep gable roof 55 deg, ridge along x at 11.2 m, RoofRedSlate, cast-iron cresting on the ridge; gable chimneys W and E |
| rear | (-4.5, -9.0) -> (0.5, -4.0) | 5.0 x 5.0 | G, F | StoneBlock | 0.5 | Gable roof 50 deg, ridge along z, RoofRedSlate; chimney on the south gable |
| bay | (4.5, -1.6) -> (6.2, 1.6) | 1.7 x 3.2 | G | StoneBlock with a tall pointed window 1.4 x 3.0 | 0.5 | Stone-slate lean-to |

### Chimneys (count must equal HouseCatalog = 3)
| x | z | stack w x d | above ridge | pots | serves |
|---|---|---|---|---|---|
| -4.5 | 2.0 | 0.8 x 0.8 | 1.0 | 2 | parlour fire |
| 4.5 | -3.0 | 0.8 x 0.8 | 1.0 | 2 | music-room fire (cold) |
| -2.0 | -9.0 | 0.8 x 0.8 | 1.0 | 1 | kitchen range = THE HEARTH |

### Exterior details
- stone ORGAN BAY with a 3 m pointed window facing the chapel
- red pointed-arch door under a sandstone hood mould with carved head stops
- ivy over the west half of the front
- three chimneys (DESIGN): west gable, east gable, rear wing
- iron ridge cresting; a weathervane in the shape of a pipe

### Plot, garden and yard
- Front: narrow yew-hedged path, a lych-style gate onto Chapel Lane
- Back: a sunken overgrown garden, a stone bench, a door in the wall into the GRAVEYARD
- Plot boundary (world polygon): (-29.0,75.5), (-29.0,90.2), (-48.0,90.2), (-48.0,75.5). Boundary = DryStoneWall / FencePicket on the street side, FenceRail or hedges (Shrub1-4) elsewhere, a GardenGate on every path.

## 2. Rooms

| Room | Level | Rect (x0,z0,x1,z1) | Clear inside | Kind | Floor | Walls | Ceiling | Light |
|---|---|---|---|---|---|---|---|---|
| **hall** Hall | G | (-1.2, -4.0, 1.0, 4.0) | 2.08 x 7.00 | hall | FloorFlagstone | PanelDark full height | CeilingInterior | pendant |
| **parlour** Parlour | G | (-4.5, 0.0, -1.2, 4.0) | 2.74 x 3.44 | living | FloorDarkPlanks | Wallpaper2 (oxblood) | CeilingInterior | pendant |
| **study** Copying room | G | (-4.5, -4.0, -1.2, 0.0) | 2.74 x 3.88 | study | FloorDarkPlanks | PanelDark | CeilingInterior | pendant |
| **music** Music room (PUZZLE) | G | (1.0, -4.0, 4.5, 4.0) | 2.94 x 7.00 | music | FloorParquet | PanelDark to 1.5 m, Wallpaper2C | CeilingInterior | chandelier (Chandelier3) |
| **bay** Organ bay | G | (4.5, -1.6, 6.2, 1.6) | 1.14 x 2.20 | music-bay | FloorParquet | StoneBlock (exposed) | CeilingInterior | pendant |
| **kitchen** Kitchen (rear wing) | G | (-4.5, -9.0, 0.5, -4.0) | 4.00 x 4.44 | kitchen | FloorFlagstone | Whitewash | CeilingInterior | pendant |
| **bed1** Mr Venn's bedroom | F | (1.0, -4.0, 4.5, 4.0) | 2.94 x 7.00 | bedroom | FloorDarkPlanks | Wallpaper1 | CeilingInterior | pendant |
| **landing** Landing | F | (-1.2, -4.0, 1.0, 4.0) | 2.08 x 7.00 | landing | FloorDarkPlanks | PanelDark | CeilingInterior | pendant |
| **bed2** Mother's room (kept as it was) | F | (-4.5, 0.0, -1.2, 4.0) | 2.74 x 3.44 | bedroom | CarpetWorn | Wallpaper1B | CeilingInterior | pendant |
| **bath** Bathroom | F | (-4.5, -4.0, -1.2, 0.0) | 2.74 x 3.88 | bathroom | FloorBathTile | TileWallWhite (black border) | CeilingInterior | pendant |
| **store** Music store (rear wing) | F | (-4.5, -9.0, 0.5, -4.0) | 4.00 x 4.44 | storage | FloorOldWood2 | RawPlankWall | CeilingInterior | pendant |
| **cellar** Cellar (bellows chamber) | C | (-4.5, -4.0, 4.5, 4.0) | 8.00 x 7.00 | cellar | FloorFlagstone | Whitewash | CeilingInterior | pendant - the organ's old hand-pumped bellows reservoir sits under the music room |

Skirting: 0.15 m dark oak in every room except kitchens/sculleries/cellars (none) and bathrooms (tile). Every ground-floor room has one LightSwitch by its main door at 1.3 m; every room has the existing `Room` component.

### Doors (how the rooms connect)
| Door | Level | From -> to | Wall | Centre at | W x H | Leaf | Role |
|---|---|---|---|---|---|---|---|
| d_front | G | hall -> ext | N | 0.0 | 1.0 x 2.4 | pointed-arch oak door painted red, iron ring handle (Red) | front |
| d_back | G | kitchen -> ext | W | -6.5 | 0.85 x 2.05 | panel | back |
| d_par | G | hall -> parlour | W | 3.0 | 0.85 x 2.05 | panel |  |
| d_study | G | hall -> study | W | -3.0 | 0.85 x 2.05 | panel |  |
| d_music | G | hall -> music | E | 2.6 | 1.1 x 2.05 | double doors |  |
| d_kit | G | study -> kitchen | S | -2.8 | 0.85 x 2.05 | panel |  |
| d_bay | G | music -> bay | E | 0.0 | 2.2 x 2.8 | open pointed arch (no leaf) |  |
| d_b1 | F | landing -> bed1 | E | 2.6 | 0.85 x 2.05 | panel |  |
| d_b2 | F | landing -> bed2 | W | 2.6 | 0.85 x 2.05 | panel |  |
| d_bath | F | landing -> bath | W | -3.0 | 0.8 x 2.05 | panel |  |
| d_store | F | bath -> store | S | -1.8 | 0.8 x 2.05 | low door (the store is reached through the bathroom - Mr Venn's odd extension) |  |

### Windows
| Window | Room | Wall | Centre at | W x H | Sill | Style | Weak | Curtains / shutters |
|---|---|---|---|---|---|---|---|---|
| w_bay | bay | E | 0.0 | 1.4 x 3.0 | 0.5 | tall pointed window, plain leaded glass, faces the chapel |  |   |
| w_par | parlour | N | -2.9 | 0.9 x 2.0 | 0.9 | lancet pair |  |   |
| w_music | music | N | 2.8 | 0.9 x 2.0 | 0.9 | lancet pair |  |   |
| w_k | kitchen | E | -6.5 | 1.0 x 1.2 | 0.9 | sash, rotten | **WEAK** (break-in point) |   |
| w_study | study | W | -2.0 | 0.8 x 1.6 | 0.9 | lancet, cracked | **WEAK** (break-in point) |   |
| w_b1 | bed1 | N | 2.8 | 0.8 x 1.6 | 0.9 | lancet |  |   |
| w_b2 | bed2 | N | -2.9 | 0.8 x 1.6 | 0.9 | lancet |  |   |
| w_store | store | S | -2.0 | 0.6 x 0.8 | 0.9 | small |  |   |

### Stairs, ladders and voids
| Id | In room | Rect | Goes | Up toward | Type |
|---|---|---|---|---|---|
| s1 | hall | (0.05, -3.7, 0.95, -0.2) | G -> F | N | straight - stone stair, 17 risers, iron balusters |
| s1u | landing | (0.05, -3.7, 0.95, -0.2) | G -> F | N | void |
| s2 | hall | (-1.15, 0.0, -0.3, 2.5) | C -> G | S | cellar steps under a trapdoor in the hall flags (trapdoor stays open) |
| s2c | cellar | (-1.15, 0.0, -0.3, 2.5) | C -> G | S | arrives |

## 3. Every prop, room by room

Position = prop **pivot at the centre of its footprint, on the floor** (or on top of its parent for `on`, or the centre height `y` for wall-hung). Yaw = the way the prop's FRONT faces. Scale the asset so its bounds fit the listed size (see ASSETS.md 'Scaling rule').

### parlour - Parlour
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 11 | `Hearth` | BUILD (PropKit) | -3.62 | 0.00 | 2.00 | 90 | 1.8 x 0.7 x 1.4 | floor | hearth-decor cast-iron fireplace (decorative); THE HEARTH is the kitchen range |
| 12 | `MantelClock` | Poly Haven `mantel_clock_01` (1k) | -3.62 | 1.40 | 2.00 | 90 | 0.3 x 0.14 x 0.32 | on H07.p011 |  stopped at 7:00 |
| 13 | `SofaPainted` | Poly Haven `painted_wooden_sofa` (2k) | -2.90 | 0.00 | 0.44 | 0 | 1.6 x 0.7 x 0.9 | floor |   |
| 14 | `SideTable` | Poly Haven `side_table_01` (1k) | -2.60 | 0.00 | 3.22 | 180 | 0.5 x 0.5 x 0.6 | floor |   |
| 15 | `RotaryPhone` | Sketchfab `812e8f59f9e749fab483eabf4800dec2` (CC BY, sara El-sisi) | -2.60 | 0.60 | 3.22 | 180 | 0.22 x 0.25 x 0.14 | on H07.p014 | phone  |
| 16 | `CassettePlayer` | Poly Haven `cassette_player` (1k) | -2.75 | 0.60 | 3.22 | 180 | 0.35 x 0.22 x 0.12 | on H07.p014 | radio cassette recorder: tapes of children singing |
| 17 | `RockingChair` | Poly Haven `Rockingchair_01` (2k) | -2.40 | 0.00 | 1.60 | 270 | 0.6 x 0.95 x 1.05 | floor |   |

### study - Copying room
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 18 | `OfficeDesk` | Poly Haven `metal_office_desk` (2k) | -3.65 | 0.00 | -2.00 | 90 | 1.19 x 0.64 x 0.65 | floor |   |
| 19 | `Shelf` | Poly Haven `wooden_bookshelf_worn` (2k) | -3.40 | 0.00 | -0.27 | 180 | 1.0 x 0.35 x 1.9 | floor |   |
| 20 | `Chair` | Poly Haven `WoodenChair_01` (2k) | -3.00 | 0.00 | -2.00 | 270 | 0.45 x 0.5 x 0.95 | floor |   |

### music - Music room (PUZZLE)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 3 | `ChestWorn` | owned: Res_Sto_Chest_Wood_Worn_08 (Fab Saloon Interior) | 2.80 | 0.00 | -3.22 | 0 | 0.9 x 0.5 x 0.55 | floor |  PART CONTAINER: pipe chest (long box 0.9) |
| 4 | `Shelf` | Poly Haven `wooden_bookshelf_worn` (2k) | 1.26 | 0.00 | -2.60 | 90 | 1.0 x 0.35 x 1.9 | floor |  sheet music to the ceiling |
| 5 | `Shelf` | Poly Haven `wooden_bookshelf_worn` (2k) | 1.26 | 0.00 | -1.40 | 90 | 1.0 x 0.35 x 1.9 | floor |   |
| 6 | `ArmchairGreen` | Poly Haven `GreenChair_01` (2k) | 2.60 | 0.00 | 1.20 | 135 | 0.75 x 0.8 x 0.95 | floor |   |
| 7 | `Rug` | BUILD (PropKit) | 2.80 | 0.00 | 0.00 | 0 | 2.0 x 1.4 x 0.01 | floor |   |
| 8 | `FrameFancy2` | Poly Haven `fancy_picture_frame_02` (1k) | 1.08 | 1.62 | 0.50 | 90 | 0.45 x 0.05 x 0.55 | wall-hung | family-photo Venn with the chapel choir, 1958 |
| 9 | `TableRound2` | Poly Haven `round_wooden_table_02` (1k) | 2.20 | 0.00 | -1.60 | 0 | 0.9 x 0.9 x 0.75 | floor |   |
| 10 | `Candleholders` | Poly Haven `brass_candleholders` (1k) | 2.20 | 0.75 | -1.60 | 0 | 0.35 x 0.12 x 0.3 | on H07.p009 |   |

### bay - Organ bay
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 1 | `PumpOrgan` | BUILD (PropKit) | 5.25 | 0.00 | 0.00 | 270 | 1.25 x 0.6 x 1.9 | floor | puzzle THE PUMP ORGAN in the bay, keyboard facing west into the room |
| 2 | `StoolPainted` | Poly Haven `painted_wooden_stool` (1k) | 4.75 | 0.00 | 0.00 | 90 | 0.35 x 0.35 x 0.45 | floor |  organ stool |

### kitchen - Kitchen (rear wing)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 21 | `Hearth` | BUILD (PropKit) | -2.00 | 0.00 | -8.12 | 0 | 1.8 x 0.7 x 1.4 | floor | hearth black range in the south gable = THE HEARTH |
| 22 | `Dresser` | BUILD (PropKit) | -0.80 | 0.00 | -4.34 | 180 | 1.4 x 0.5 x 2.0 | floor |   |
| 23 | `TableWood2` | Poly Haven `WoodenTable_02` (1k) | -2.00 | 0.00 | -6.40 | 90 | 1.6 x 0.85 x 0.76 | floor |   |
| 24 | `KitchenSinkUnit` | BUILD (PropKit) | -3.67 | 0.00 | -5.00 | 90 | 1.0 x 0.6 x 0.9 | floor |   |
| 25 | `FuseBox` | Poly Haven `power_box_01` (1k) | 0.00 | 1.40 | -4.13 | 180 | 0.45 x 0.15 x 0.6 | wall-hung | fuse-box  |

### bed1 - Mr Venn's bedroom
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 26 | `Bed` | Poly Haven `GothicBed_01` (2k) | 2.92 | 0.00 | 1.80 | 270 | 1.6 x 2.1 x 1.4 | floor |   |
| 27 | `Nightstand` | Poly Haven `ClassicNightstand_01` (1k) | 3.77 | 0.00 | 0.70 | 270 | 0.5 x 0.4 x 0.6 | floor |   |
| 28 | `Nightstand` | Poly Haven `ClassicNightstand_01` (1k) | 3.77 | 0.00 | 2.90 | 270 | 0.5 x 0.4 x 0.6 | floor |   |
| 29 | `Wardrobe` | Poly Haven `GothicCabinet_01` (2k) | 2.80 | 0.00 | -3.17 | 0 | 1.2 x 0.6 x 2.1 | floor |   |

### landing - Landing
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 42 | `ShrineNiche` | BUILD (PropKit) | 0.60 | 0.90 | 3.38 | 180 | 0.6 x 0.25 x 0.9 | wall-hung | shrine  |

### bed2 - Mother's room (kept as it was)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 30 | `DayBed` | Poly Haven `vintage_day_bed` (2k) | -3.00 | 0.00 | 1.40 | 90 | 0.95 x 1.95 x 0.85 | floor |   |
| 31 | `NightstandPainted` | Poly Haven `painted_wooden_nightstand` (1k) | -3.78 | 0.00 | 0.65 | 90 | 0.45 x 0.38 x 0.6 | floor |   |
| 32 | `Commode` | Poly Haven `GothicCommode_01` (2k) | -2.90 | 0.00 | 0.34 | 0 | 1.1 x 0.5 x 0.9 | floor |  dressing table with her hairbrushes still on it |

### bath - Bathroom
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 33 | `BathtubPhoto` | Sketchfab `ba60ba7d0a4a4b6899021741f450395b` (CC BY, Igrium) | -2.90 | 0.00 | -0.48 | 180 | 1.7 x 0.78 x 0.65 | floor |   |
| 34 | `Sink` | Sketchfab `a278d63585494921906ab3500c421ff7` (CC BY, lagesnpiet) | -3.75 | 0.00 | -2.20 | 90 | 0.55 x 0.45 x 0.85 | floor |   |
| 35 | `Mirror` | Poly Haven `ornate_mirror_01` (1k) | -3.98 | 1.10 | -2.20 | 90 | 0.6 x 0.05 x 0.9 | wall-hung | mirror  |
| 36 | `Toilet` | Sketchfab `6ac515a1c4154db18b5b4bd0b46d6405` (CC BY, Allan-Jay Branscombe) | -3.65 | 0.00 | -3.40 | 90 | 0.4 x 0.65 x 0.8 | floor |   |

### store - Music store (rear wing)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 37 | `SteelShelves` | Poly Haven `steel_frame_shelves_01` (1k) | -0.26 | 0.00 | -6.50 | 270 | 1.2 x 0.45 x 1.9 | floor |  organ pipes of every length, labelled |
| 38 | `CellarCrates` | BUILD (PropKit) | -3.52 | 0.00 | -6.50 | 90 | 1.4 x 0.9 x 1.2 | floor |   |

### cellar - Cellar (bellows chamber)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 39 | `Barrel2` | Poly Haven `Barrel_02` (1k) | 3.67 | 0.00 | -2.00 | 270 | 0.6 x 0.6 x 0.9 | floor |  bellows reservoir (leather, cracked) - a big shape |
| 40 | `BarrelsWood` | Poly Haven `wooden_barrels_01` (1k) | 2.00 | 0.00 | 3.07 | 180 | 1.4 x 0.8 x 1.0 | floor |   |
| 41 | `SteelShelves2` | Poly Haven `steel_frame_shelves_02` (1k) | -3.77 | 0.00 | 2.00 | 90 | 1.0 x 0.4 x 1.8 | floor |   |

## 4. Gameplay anchors (empty GameObjects the builder must create)
| Kind | Room | x | y | z | What |
|---|---|---|---|---|---|
| puzzle | bay | 5.25 | 0.00 | 0.00 | SequencePuzzle reskinned: 6 marked keys (symbols painted on the key fronts); play the village lullaby (children hum it by day; notation on the hymn sheet in the copying room). EVERY KEY PRESS = NoiseRadius.LoudPuzzle |
| container | music | 2.80 | 0.00 | -3.22 | Organ pipe |
| item-spot | music | 2.20 | 0.75 | -1.60 | music-room table |
| item-spot | parlour | -3.62 | 1.40 | 2.00 | parlour mantel |
| note-spot | study | -3.65 | 0.65 | -1.90 | HYMN SHEET with the lullaby in shape-note symbols (note 1/3) |
| item-spot | kitchen | -2.00 | 1.40 | -8.12 | range shelf |
| note-spot | kitchen | -2.00 | 0.76 | -6.30 | letter from the vicar about 'the tune' (note 2/3) |
| hiding | bed1 | 2.92 | 0.00 | 1.80 | under the bed -> HidingSpot on H07.p026 (Bed) - crouched |
| hiding | bed1 | 2.80 | 0.00 | -3.17 | wardrobe -> HidingSpot on H07.p029 (Wardrobe) |
| note-spot | bed2 | -3.00 | 0.90 | 0.34 | Mrs Venn's lullaby verses (note 3/3) |
| hiding | bath | -2.90 | 0.00 | -0.48 | behind the bath curtain -> HidingSpot on H07.p033 (BathtubPhoto) |
| hiding | store | -3.52 | 0.00 | -6.50 | behind the pipe crates -> HidingSpot on H07.p038 (CellarCrates) - crouched |
| shrine | landing | 0.60 | 0.00 | 3.38 | Rite of Return point (active only if this house is a shrine this match) |

Mapping to existing code: `item-spot` -> `House.ItemSpots` (order as listed), `note-spot` -> the 3 `Readable` notes, `puzzle` -> `PuzzleRig` anchor, `container` -> `PartContainer`, `shrine` -> `House.ShrineMarker` (+ shrine niche inside), `hiding` -> `HidingSpot`.

## 5. Puzzle - Organ pipe
1. LOCK FIRST: the pump organ in the bay, music desk open, a long pipe chest at its feet.
2. The hymn sheet (copying room, note 1/3) writes the lullaby in shape notes (= the 6 symbols).
3. Children hum the lullaby by day (M6); the cassette recorder in the parlour plays a tape of it (partial).
4. Play 4-6 marked keys in order. EVERY KEY = NoiseRadius.LoudPuzzle (DESIGN 'Play the village lullaby... loudly').
5. Part: Organ pipe (0.9 m), in the pipe chest.

## 6. Night changes (DESIGN 'Night changes' - pick 2-3 per match)
- the organ plays one long low note when the Hollow is within 30 m
- the mother's dressing table mirror shows someone brushing her hair
- sheet music pages are spread on the floor in a trail to the cellar
- plus the shared pool: extra person in the family photo, a door opening onto a brick wall, moved furniture, an extra hallway door, all clocks stopped at the same time, writing on a wall in a player's name

## 7. Sound
- bells (chapel next door)
- organ: generated tones (in-world music only)
- haunting5 in the cellar
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
