# H11 - Priest's House (Ambrose)

> Machine-readable twin: `docs/design/houses/H11.json` - the builder reads THAT file; this page explains it. Every number here comes from the same data and is validated (no overlaps, door clearances, required items).

| | |
|---|---|
| **Archetype** | Stone presbytery with a private oratory wing projecting toward the lane |
| **Silhouette (must read at 50 m in fog)** | Grey stone, two storeys, THREE chimneys (west gable, a big external stack on the back wall, and the front stack of the study); an oratory wing to the east with a steep roof, a tiny bell-cote and a pointed window; white door under a stone canopy; a Calvary cross in the garden. |
| **Ritual part** | Altar cloth |
| **M1 puzzle rig (code)** | `PuzzleKind.CodeLock` |
| **Letter traits (HouseCatalog)** | door **White**, **3** chimney(s), stopped clock **1:00**, family **Ambrose** |
| **World position** | x -62.0, z 80.0, floor level y +0.40, yaw 90 deg - Chapel Lane (west side) |
| **Front door (world)** | (-58.0, 81.5) - 65.5 m from the altar, 5.6 m from the lane |
| **Placement note** | oratory wing (local +x) points south; Calvary in the front garden; graveyard behind to the north-west |

## Reference images

![plan](../ref/H11_plan.png)

![front-left](../ref/H11_massing_front.png) ![back-right](../ref/H11_massing_back.png)

Real-world references (look, don't copy): [Clergy house](https://en.wikipedia.org/wiki/Clergy_house)

Mood-image prompt (Higgsfield / Gemini / ChatGPT image - save the result as `docs/design/ref/H11_mood.png`):

```text
grey stone two-storey presbytery with three chimneys, a white boarded door under a stone canopy, a small attached private chapel wing with a steep roof, a tiny bell-cote and a pointed stained-glass window, a stone Calvary cross in a box-hedged front garden, photorealistic, late 1970s rural English village, overcast autumn dusk, damp, muted greens and greys, 35mm film still, Phasmophobia / Resident Evil Village mood, no people, eye-level, 16:9
```

## 1. Structure

### Levels
| Level | Floor y (m) | Clear height (m) |
|---|---|---|
| G - Ground floor | +0.00 | 2.90 |
| F - First floor | +3.15 | 2.60 |
| A - Attic (under the roof) | +6.00 | 2.30 |

Floor build-up: 0.25 m between floors (joists + boards); ceilings use the room's `ceiling` material.

### Wings (massing)
| Wing | Rect x0,z0 -> x1,z1 | Size | Levels | Walls | Wall thickness | Roof |
|---|---|---|---|---|---|---|
| main | (-6.0, -4.0) -> (2.0, 4.0) | 8.0 x 8.0 | G, F, A | StoneBlock with SandstoneBlocks quoins; lime-pointed | 0.5 | Gable roof 45 deg, ridge along x at 8.9 m, RoofSlateGrey |
| oratory | (2.0, -1.0) -> (6.0, 6.0) | 4.0 x 7.0 | G | StoneBlock, buttresses at the corners | 0.5 | Steep 55 deg gable roof, ridge along z at 7.2 m, RoofSlateGrey; bell-cote (Bell scaled 0.35) at the north gable apex |

### Chimneys (count must equal HouseCatalog = 3)
| x | z | stack w x d | above ridge | pots | serves |
|---|---|---|---|---|---|
| -6.0 | 2.0 | 0.8 x 0.8 | 1.0 | 1 | sitting-room fire |
| -4.4 | -4.0 | 1.2 x 0.8 | 1.0 | 2 | kitchen range = THE HEARTH (external stack on the back wall) |
| 0.75 | 4.0 | 0.9 x 0.6 | 1.0 | 1 | study fire (external front stack) |

### Exterior details
- white boarded door under a stone canopy; a small brass crucifix above it
- oratory wing: steep roof, tiny bell-cote (no bell rope), a pointed lancet with red/blue glass
- three chimneys (DESIGN): west gable, big external stack on the back wall, front stack of the study
- a Calvary (stone cross) in the front garden, lichen-covered

### Plot, garden and yard
- Front: box-hedged garden with gravel paths in a cross pattern, the Calvary in the middle
- Back: vegetable plot gone wild, a gate onto the graveyard path
- Plot boundary (world polygon): (-72.0,88.0), (-72.0,72.0), (-52.0,72.0), (-52.0,88.0). Boundary = DryStoneWall / FencePicket on the street side, FenceRail or hedges (Shrub1-4) elsewhere, a GardenGate on every path.

## 2. Rooms

| Room | Level | Rect (x0,z0,x1,z1) | Clear inside | Kind | Floor | Walls | Ceiling | Light |
|---|---|---|---|---|---|---|---|---|
| **sitting** Sitting room | G | (-6.0, 0.0, -2.5, 4.0) | 2.94 x 3.44 | living | Carpet70sB | Wallpaper2C (sage) | CeilingInterior | pendant |
| **hall** Hall | G | (-2.5, -4.0, -0.5, 4.0) | 1.88 x 7.00 | hall | FloorFlagstone | PlasterWorn | CeilingInterior | pendant |
| **study** Study | G | (-0.5, 0.0, 2.0, 4.0) | 2.38 x 3.44 | study | FloorParquet | PanelDark | CeilingInterior | pendant |
| **kitchen** Kitchen (housekeeper's) | G | (-6.0, -4.0, -2.5, 0.0) | 2.94 x 3.44 | kitchen | FloorFlagstone | Whitewash | CeilingInterior | pendant |
| **scull** Scullery & lavatory | G | (-0.5, -4.0, 2.0, 0.0) | 1.94 x 3.44 | utility | FloorBrick | Whitewash | CeilingInterior | pendant |
| **oratory** Oratory (PUZZLE) | G | (2.0, -1.0, 6.0, 6.0) | 3.00 x 6.00 | chapel | FloorFlagstone | Whitewash, stations of the cross | open timber roof to 6 m | 2 hanging lamps (WoodenLantern) + candles |
| **bed1** Father Ambrose's bedroom | F | (-6.0, 0.0, -2.5, 4.0) | 2.94 x 3.44 | bedroom | FloorDarkPlanks | Whitewash, crucifix | CeilingInterior | pendant |
| **landing** Landing | F | (-2.5, -4.0, -0.5, 4.0) | 1.88 x 7.00 | landing | FloorDarkPlanks | PlasterWorn | CeilingInterior | pendant |
| **bed2** Housekeeper's room | F | (-0.5, 0.0, 2.0, 4.0) | 1.94 x 3.44 | bedroom | FloorOldWood1 | Wallpaper1B | CeilingInterior | pendant |
| **bath** Bathroom | F | (-6.0, -4.0, -2.5, 0.0) | 2.94 x 3.44 | bathroom | FloorBathTile | TileWallWhite | CeilingInterior | pendant |
| **box** Box room (vestments) | F | (-0.5, -4.0, 2.0, 0.0) | 1.94 x 3.44 | storage | FloorOldWood2 | PlasterWorn | CeilingInterior | pendant |
| **attic** Attic | A | (-6.0, -4.0, 2.0, 4.0) | 7.00 x 7.00 | attic | FloorOldWood2 | RawPlankWall | CeilingInterior | pendant |

Skirting: 0.15 m dark oak in every room except kitchens/sculleries/cellars (none) and bathrooms (tile). Every ground-floor room has one LightSwitch by its main door at 1.3 m; every room has the existing `Room` component.

### Doors (how the rooms connect)
| Door | Level | From -> to | Wall | Centre at | W x H | Leaf | Role |
|---|---|---|---|---|---|---|---|
| d_front | G | hall -> ext | N | -1.5 | 1.0 x 2.05 | white boarded door with a stone canopy over it (White) | front |
| d_back | G | hall -> ext | S | -1.0 | 0.85 x 2.05 | panel | back |
| d_sit | G | hall -> sitting | W | 3.0 | 0.85 x 2.05 | panel |  |
| d_kit | G | hall -> kitchen | W | -2.8 | 0.85 x 2.05 | panel |  |
| d_study | G | hall -> study | E | 2.5 | 0.85 x 2.05 | panel |  |
| d_scull | G | hall -> scull | E | -2.0 | 0.8 x 2.05 | panel |  |
| d_orat | G | study -> oratory | E | 2.0 | 0.9 x 2.05 | pointed oak door with a holy-water stoup beside it |  |
| d_orat_ext | G | oratory -> ext | N | 4.0 | 1.0 x 2.05 | oratory west door to the lane (locked by day) | side |
| d_b1 | F | landing -> bed1 | W | 3.0 | 0.8 x 2.05 | panel |  |
| d_b2 | F | landing -> bed2 | E | 2.5 | 0.8 x 2.05 | panel |  |
| d_bath | F | landing -> bath | W | -2.8 | 0.75 x 2.05 | panel |  |
| d_box | F | landing -> box | E | -2.0 | 0.75 x 2.05 | panel |  |

### Windows
| Window | Room | Wall | Centre at | W x H | Sill | Style | Weak | Curtains / shutters |
|---|---|---|---|---|---|---|---|---|
| w_sit | sitting | N | -4.2 | 1.2 x 1.5 | 0.9 | sash with stone mullion |  |   |
| w_study | study | N | 0.75 | 0.9 x 1.4 | 0.9 | sash |  |   |
| w_k | kitchen | W | -2.0 | 1.0 x 1.1 | 0.9 | sash, rotten sill | **WEAK** (break-in point) |   |
| w_sc | scull | S | 0.75 | 0.8 x 0.9 | 0.9 | small casement | **WEAK** (break-in point) |   |
| w_or1 | oratory | E | 2.5 | 0.9 x 2.4 | 1.0 | pointed lancet with plain coloured glass (red/blue) |  |   |
| w_or2 | oratory | S | 4.0 | 0.7 x 1.6 | 1.2 | small lancet behind the altar |  |   |
| w_b1 | bed1 | N | -4.2 | 1.0 x 1.2 | 0.9 | sash |  |   |
| w_b2 | bed2 | N | 0.75 | 0.8 x 1.1 | 0.9 | sash |  |   |
| w_bath | bath | W | -2.0 | 0.7 x 0.8 | 1.3 | frosted |  |   |

### Stairs, ladders and voids
| Id | In room | Rect | Goes | Up toward | Type |
|---|---|---|---|---|---|
| s1 | hall | (-2.4, -1.5, -1.5, 2.0) | G -> F | S | straight - stone-and-oak stair, 15 risers |
| s1u | landing | (-2.4, -1.5, -1.5, 2.0) | G -> F | S | void |
| s2 | landing | (-1.3, -3.4, -0.6, -2.6) | F -> A | W | loft ladder |

## 3. Every prop, room by room

Position = prop **pivot at the centre of its footprint, on the floor** (or on top of its parent for `on`, or the centre height `y` for wall-hung). Yaw = the way the prop's FRONT faces. Scale the asset so its bounds fit the listed size (see ASSETS.md 'Scaling rule').

### sitting - Sitting room
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 16 | `Hearth` | BUILD (PropKit) | -5.12 | 0.00 | 2.00 | 90 | 1.8 x 0.7 x 1.4 | floor | hearth-decor stone fireplace |
| 17 | `MantelClock` | Poly Haven `mantel_clock_01` (1k) | -5.12 | 1.40 | 2.00 | 90 | 0.3 x 0.14 x 0.32 | on H11.p016 |  stopped at 1:00 |
| 18 | `FrameStanding1` | Poly Haven `standing_picture_frame_01` (1k) | -5.12 | 1.40 | 2.60 | 90 | 0.18 x 0.08 x 0.24 | on H11.p016 | family-photo Fr Ambrose's ordination, 1949 |
| 19 | `Sofa3` | Poly Haven `sofa_03` (2k) | -4.20 | 0.00 | 0.52 | 0 | 1.8 x 0.85 x 0.8 | floor |   |
| 20 | `Armchair` | Poly Haven `ArmChair_01` (2k) | -4.20 | 0.00 | 2.20 | 200 | 0.85 x 0.85 x 0.95 | floor |   |
| 21 | `SideTable` | Poly Haven `side_table_01` (1k) | -3.00 | 0.00 | 1.40 | 0 | 0.5 x 0.5 x 0.6 | floor |   |
| 22 | `RotaryPhone` | Sketchfab `812e8f59f9e749fab483eabf4800dec2` (CC BY, sara El-sisi) | -3.00 | 0.60 | 1.40 | 0 | 0.22 x 0.25 x 0.14 | on H11.p021 | phone  |
| 23 | `Radio` | Poly Haven `vintage_radio_transceiver` (1k) | -2.85 | 0.60 | 1.40 | 0 | 0.45 x 0.25 x 0.3 | on H11.p021 | radio  |

### study - Study
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 10 | `OfficeDesk` | Poly Haven `metal_office_desk` (2k) | 0.75 | 0.00 | 0.60 | 0 | 1.12 x 0.6 x 0.61 | floor |   |
| 11 | `BooksDeco` | Poly Haven `decorative_book_set_01` (1k) | 0.45 | 0.61 | 0.60 | 0 | 0.4 x 0.2 x 0.25 | on H11.p010 |   |
| 12 | `DeskLamp` | Poly Haven `desk_lamp_arm_01` (1k) | 1.05 | 0.61 | 0.60 | 0 | 0.25 x 0.4 x 0.5 | on H11.p010 |   |
| 13 | `Chair` | Poly Haven `WoodenChair_01` (2k) | 0.75 | 0.00 | 1.30 | 180 | 0.45 x 0.5 x 0.95 | floor |   |
| 14 | `Shelf` | Poly Haven `wooden_bookshelf_worn` (2k) | 0.75 | 0.00 | 3.29 | 180 | 1.0 x 0.35 x 1.9 | floor |   |
| 15 | `WallClock` | Poly Haven `wall_clock` (1k) | -0.41 | 1.82 | 1.00 | 90 | 0.35 x 0.06 x 0.35 | wall-hung |   |

### kitchen - Kitchen (housekeeper's)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 24 | `Hearth` | BUILD (PropKit) | -4.40 | 0.00 | -3.12 | 0 | 1.8 x 0.7 x 1.4 | floor | hearth range in the big back-wall stack = THE HEARTH |
| 25 | `Dresser` | BUILD (PropKit) | -4.20 | 0.00 | -0.34 | 180 | 1.4 x 0.5 x 2.0 | floor |   |
| 26 | `TableWood2` | Poly Haven `WoodenTable_02` (1k) | -4.20 | 0.00 | -1.80 | 0 | 1.6 x 0.85 x 0.76 | floor |   |
| 27 | `ChairDining` | Poly Haven `dining_chair_02` (1k) | -4.20 | 0.00 | -1.00 | 180 | 0.45 x 0.5 x 0.95 | floor |   |

### scull - Scullery & lavatory
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 28 | `Copper` | BUILD (PropKit) | 0.90 | 0.00 | -3.07 | 0 | 0.8 x 0.8 x 0.9 | floor |   |
| 29 | `KitchenSinkUnit` | BUILD (PropKit) | 0.30 | 0.00 | -0.39 | 180 | 1.0 x 0.6 x 0.9 | floor |  stone sink |
| 30 | `Toilet` | Sketchfab `6ac515a1c4154db18b5b4bd0b46d6405` (CC BY, Allan-Jay Branscombe) | 1.15 | 0.00 | -1.00 | 270 | 0.4 x 0.65 x 0.8 | floor |   |
| 31 | `FuseBox` | Poly Haven `power_box_01` (1k) | 1.43 | 1.40 | -2.60 | 270 | 0.45 x 0.15 x 0.6 | wall-hung | fuse-box  |

### oratory - Oratory (PUZZLE)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 1 | `SmallAltar` | BUILD (PropKit) | 4.00 | 0.00 | -0.12 | 0 | 1.6 x 0.7 x 1.0 | floor |  altar with candlesticks; the frontal hides the cupboard |
| 2 | `Candleholders` | Poly Haven `brass_candleholders` (1k) | 3.50 | 1.00 | -0.12 | 0 | 0.35 x 0.12 x 0.3 | on H11.p001 |   |
| 3 | `Candleholders` | Poly Haven `brass_candleholders` (1k) | 4.50 | 1.00 | -0.12 | 0 | 0.35 x 0.12 x 0.3 | on H11.p001 |   |
| 4 | `HymnBoard` | BUILD (PropKit) | 2.52 | 1.35 | 4.60 | 90 | 0.5 x 0.05 x 0.9 | wall-hung | puzzle HYMN BOARD: 3 rows of number cards |
| 5 | `CabinetVintage` | Poly Haven `vintage_cabinet_01` (2k) | 5.25 | 0.00 | 0.20 | 270 | 1.0 x 0.45 x 1.8 | floor |  VESTMENT CUPBOARD with a 4-dial lock = PART CONTAINER |
| 6 | `ChurchPew` | Sketchfab `9c2934828dfa4dc2b64102defa3969e7` (CC BY, Sam Constance) | 4.00 | 0.00 | 1.60 | 180 | 2.25 x 0.45 x 0.71 | floor |   |
| 7 | `ChurchPew` | Sketchfab `9c2934828dfa4dc2b64102defa3969e7` (CC BY, Sam Constance) | 4.00 | 0.00 | 3.20 | 180 | 2.25 x 0.45 x 0.71 | floor |   |
| 8 | `Lectern` | BUILD (PropKit) | 2.80 | 0.00 | 0.60 | 90 | 0.6 x 0.5 x 1.15 | floor |   |
| 9 | `GothicStatue` | Poly Haven `gothic_statue` (2k) | 2.74 | 0.00 | 5.20 | 90 | 0.42 x 0.42 x 1.26 | floor |  statue of the village saint, face worn smooth |
| 43 | `ShrineNiche` | BUILD (PropKit) | 5.38 | 0.90 | 4.80 | 270 | 0.6 x 0.25 x 0.9 | wall-hung | shrine  |

### bed1 - Father Ambrose's bedroom
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 32 | `BedIron` | Poly Haven `old_bed_frame` (2k) | -4.47 | 0.00 | 2.00 | 90 | 1.0 x 2.0 x 1.05 | floor |   |
| 33 | `Nightstand` | Poly Haven `ClassicNightstand_01` (1k) | -5.27 | 0.00 | 2.80 | 90 | 0.5 x 0.4 x 0.6 | floor |   |
| 34 | `Wardrobe` | Poly Haven `GothicCabinet_01` (2k) | -4.20 | 0.00 | 0.39 | 0 | 1.2 x 0.6 x 2.1 | floor |   |

### bed2 - Housekeeper's room
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 35 | `BedIron` | Poly Haven `old_bed_frame` (2k) | 1.00 | 0.00 | 1.09 | 0 | 1.0 x 2.0 x 1.05 | floor |   |

### bath - Bathroom
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 36 | `BathtubPhoto` | Sketchfab `ba60ba7d0a4a4b6899021741f450395b` (CC BY, Igrium) | -4.40 | 0.00 | -3.08 | 0 | 1.7 x 0.78 x 0.65 | floor |   |
| 37 | `Sink` | Sketchfab `a278d63585494921906ab3500c421ff7` (CC BY, lagesnpiet) | -4.60 | 0.00 | -0.32 | 180 | 0.55 x 0.45 x 0.85 | floor |   |
| 38 | `Mirror` | Poly Haven `ornate_mirror_01` (1k) | -4.60 | 1.10 | -0.09 | 180 | 0.6 x 0.05 x 0.9 | wall-hung | mirror  |

### box - Box room (vestments)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 39 | `CabinetPainted` | Poly Haven `painted_wooden_cabinet` (2k) | 0.75 | 0.00 | -3.22 | 0 | 1.0 x 0.5 x 1.9 | floor |  cassocks on hooks |
| 40 | `TableSmall` | Poly Haven `small_wooden_table_01` (1k) | 0.90 | 0.00 | -1.00 | 0 | 0.7 x 0.5 x 0.72 | floor |   |

### attic - Attic
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 41 | `ChestWorn` | owned: Res_Sto_Chest_Wood_Worn_08 (Fab Saloon Interior) | -5.22 | 0.00 | 0.00 | 90 | 0.9 x 0.5 x 0.55 | floor |   |
| 42 | `Suitcase` | Poly Haven `vintage_suitcase` (1k) | -3.00 | 0.00 | 3.37 | 180 | 0.65 x 0.2 x 0.45 | floor |   |

## 4. Gameplay anchors (empty GameObjects the builder must create)
| Kind | Room | x | y | z | What |
|---|---|---|---|---|---|
| item-spot | oratory | 4.00 | 1.00 | -0.12 | altar (between the candlesticks) |
| puzzle | oratory | 2.52 | 1.35 | 4.60 | CodeLockPuzzle reskinned: the hymn board shows 3 hymn numbers; the hymn book index (study) turns each hymn into a page number; first digit of each + the psalm number on the altar card = 4-digit vestment-cupboard code |
| container | oratory | 5.25 | 0.00 | 0.20 | Altar cloth, folded, red embroidered |
| hiding | oratory | 5.20 | 0.00 | 5.00 | behind the curtain of the confessional corner (NE) |
| note-spot | study | 0.65 | 0.61 | 0.60 | HYMN BOOK INDEX page, torn out (note 1/3) |
| item-spot | sitting | -5.12 | 1.40 | 2.00 | sitting-room mantel |
| item-spot | kitchen | -4.40 | 1.40 | -3.12 | range shelf |
| note-spot | kitchen | -4.30 | 0.76 | -1.80 | housekeeper's list: 'Sunday: 23, 104, 7' (note 2/3) |
| hiding | bed1 | -4.47 | 0.00 | 2.00 | under the narrow iron bed -> HidingSpot on H11.p032 (BedIron) - crouched |
| hiding | bed1 | -4.20 | 0.00 | 0.39 | wardrobe -> HidingSpot on H11.p034 (Wardrobe) |
| hiding | bath | -4.40 | 0.00 | -3.08 | behind the bath curtain -> HidingSpot on H11.p036 (BathtubPhoto) |
| hiding | box | 0.75 | 0.00 | -3.22 | vestment cupboard (walk-in) -> HidingSpot on H11.p039 (CabinetPainted) |
| note-spot | box | 0.80 | 0.72 | -1.00 | parish newsletter (note 3/3) |
| shrine | oratory | 5.38 | 0.00 | 4.80 | Rite of Return point (active only if this house is a shrine this match) |

Mapping to existing code: `item-spot` -> `House.ItemSpots` (order as listed), `note-spot` -> the 3 `Readable` notes, `puzzle` -> `PuzzleRig` anchor, `container` -> `PartContainer`, `shrine` -> `House.ShrineMarker` (+ shrine niche inside), `hiding` -> `HidingSpot`.

## 5. Puzzle - Altar cloth
1. LOCK FIRST: entering the oratory, the hymn board is lit by the lancet and the vestment cupboard's dials glint.
2. The board shows 3 hymn numbers (randomised); the hymn-book index (study, note 1/3) gives each hymn's page.
3. The housekeeper's list (note 2/3) is a decoy from LAST Sunday (false-clue flavour).
4. Code = first digit of each page number + the psalm number on the altar card.
5. Wrong: the bell-cote bell rings once (no rope!) - noise. Part: Altar cloth.

## 6. Night changes (DESIGN 'Night changes' - pick 2-3 per match)
- the hymn numbers have changed to the players' count
- the oratory statue now faces the door
- candles on the altar are lit
- plus the shared pool: extra person in the family photo, a door opening onto a brick wall, moved furniture, an extra hallway door, all clocks stopped at the same time, writing on a wall in a player's name

## 7. Sound
- bells: one small bell
- books
- ghostly: Latin murmuring from the box room
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
