# H10 - Photographer's House (Lisle)

> Machine-readable twin: `docs/design/houses/H10.json` - the builder reads THAT file; this page explains it. Every number here comes from the same data and is validated (no overlaps, door clearances, required items).

| | |
|---|---|
| **Archetype** | Edwardian red-brick double-fronted shop-house with a Victorian glasshouse studio behind |
| **Silhouette (must read at 50 m in fog)** | Symmetrical red brick with a recessed black door between two windows (left one a shop window lettered 'LISLE - PORTRAITS'), a gable chimney at each end, and behind it a long glass-roofed studio glinting over the garden wall. |
| **Ritual part** | Glass negative |
| **M1 puzzle rig (code)** | `PuzzleKind.CodeLock` |
| **Letter traits (HouseCatalog)** | door **Black**, **2** chimney(s), stopped clock **12:00**, family **Lisle** |
| **World position** | x -8.0, z 50.0, floor level y +0.00, yaw 90 deg - Main Street (west side) |
| **Front door (world)** | (-4.0, 50.4) - 61.7 m from the altar, 5.0 m from the lane |
| **Placement note** | glass studio behind |

## Reference images

![plan](../ref/H10_plan.png)

![front-left](../ref/H10_massing_front.png) ![back-right](../ref/H10_massing_back.png)

Real-world references (look, don't copy): [Darkroom](https://en.wikipedia.org/wiki/Darkroom), [Photographic studio](https://en.wikipedia.org/wiki/Photographic_studio)

Mood-image prompt (Higgsfield / Gemini / ChatGPT image - save the result as `docs/design/ref/H10_mood.png`):

```text
Edwardian red brick double-fronted shop house with a shop window lettered 'LISLE - PORTRAITS', a black front door in a tiled recess, chimneys at both gable ends, a long Victorian glasshouse photography studio behind it with white-painted glass panes, photorealistic, late 1970s rural English village, overcast autumn dusk, damp, muted greens and greys, 35mm film still, Phasmophobia / Resident Evil Village mood, no people, eye-level, 16:9
```

## 1. Structure

### Levels
| Level | Floor y (m) | Clear height (m) |
|---|---|---|
| C - Cellar | -2.40 | 2.20 |
| G - Ground floor | +0.00 | 2.90 |
| F - First floor | +3.15 | 2.70 |

Floor build-up: 0.25 m between floors (joists + boards); ceilings use the room's `ceiling` material.

### Wings (massing)
| Wing | Rect x0,z0 -> x1,z1 | Size | Levels | Walls | Wall thickness | Roof |
|---|---|---|---|---|---|---|
| main | (-5.0, -4.0) -> (5.0, 4.0) | 10.0 x 8.0 | C, G, F | BrickRed (Flemish bond), BrickDark plinth, SandstoneBlocks lintels with keystones | 0.35 | Gable roof 40 deg, ridge along x at 8.6 m, RoofSlateGrey; chimneys on both gables |
| studio | (-4.0, -10.0) -> (4.0, -4.0) | 8.0 x 6.0 | G | Low brick wall 0.9 m (BrickRed) then timber-and-glass walls (half the panes painted white for diffusion, several broken) | 0.15 | Glass double-pitch roof on iron glazing bars, ridge 4.4 m; blinds on cords under the glass |

### Chimneys (count must equal HouseCatalog = 2)
| x | z | stack w x d | above ridge | pots | serves |
|---|---|---|---|---|---|
| -5.0 | -2.6 | 0.8 x 0.8 | 1.0 | 2 | kitchen range = THE HEARTH |
| 5.0 | 2.0 | 0.8 x 0.8 | 1.0 | 2 | parlour fire |

### Exterior details
- recessed tiled entrance (black and white) with a black door, etched glass 'LISLE'
- shop window: gilt 'LISLE - PORTRAITS', framed portraits on easels facing the street (all subjects' eyes scratched out at night)
- glasshouse studio visible over the side gate: white-painted panes, roller blinds, 3 broken panes

### Plot, garden and yard
- Front: pavement
- Back: narrow garden mostly taken by the studio; a path down the side; a dustbin of broken glass plates
- Plot boundary (world polygon): (-22.0,55.3), (-22.0,44.7), (-3.7,44.7), (-3.7,55.3). Boundary = DryStoneWall / FencePicket on the street side, FenceRail or hedges (Shrub1-4) elsewhere, a GardenGate on every path.

## 2. Rooms

| Room | Level | Rect (x0,z0,x1,z1) | Clear inside | Kind | Floor | Walls | Ceiling | Light |
|---|---|---|---|---|---|---|---|---|
| **reception** Reception (portrait shop) | G | (-5.0, 0.0, -1.0, 4.0) | 3.59 x 3.59 | shop | FloorParquet | Wallpaper2 (dark red) above PanelDark | CeilingInterior | pendant |
| **hall** Hall | G | (-1.0, -4.0, 1.0, 4.0) | 1.88 x 7.59 | hall | FloorWornTile | Wallpaper2C | CeilingInterior | pendant |
| **parlour** Parlour | G | (1.0, 0.0, 5.0, 4.0) | 3.59 x 3.59 | living | Carpet70s | WallpaperQuatrefoil | CeilingInterior | pendant |
| **kitchen** Kitchen | G | (-5.0, -4.0, -1.0, 0.0) | 3.59 x 3.59 | kitchen | FloorQuarryTile | Whitewash | CeilingInterior | pendant |
| **print** Print & retouching room | G | (1.0, -4.0, 5.0, 0.0) | 3.59 x 3.59 | workshop | FloorOldWood2 | PlasterWorn, prints pinned everywhere | CeilingInterior | pendant |
| **studio** Glass studio | G | (-4.0, -10.0, 4.0, -4.0) | 7.70 x 5.79 | studio | FloorOldWood1 | glass + painted backdrops | CeilingInterior | daylight; 2 StudioLamp |
| **bed1** Lisles' bedroom | F | (-5.0, 0.0, -1.0, 4.0) | 3.59 x 3.59 | bedroom | CarpetWorn | Wallpaper1 | CeilingInterior | pendant |
| **landing** Landing | F | (-1.0, -4.0, 1.0, 4.0) | 1.88 x 7.30 | landing | Carpet70sB | Wallpaper2C | CeilingInterior | pendant |
| **bed2** Daughter's bedroom | F | (1.0, 0.0, 5.0, 4.0) | 3.59 x 3.59 | bedroom | Carpet70sB | Wallpaper1B | CeilingInterior | pendant |
| **bath** Bathroom | F | (-5.0, -4.0, -1.0, 0.0) | 3.59 x 3.59 | bathroom | FloorBathTile | TileWallWhite | CeilingInterior | pendant |
| **store** Negative store | F | (1.0, -4.0, 5.0, 0.0) | 3.59 x 3.59 | storage | FloorOldWood2 | shelving floor to ceiling | CeilingInterior | pendant |
| **cellar** Darkroom cellar (PUZZLE) | C | (-5.0, -4.0, 5.0, 0.0) | 9.30 x 3.30 | cellar | FloorConcrete | painted matt black | CeilingInterior | RED SAFELIGHT only (Safelight); white bulb ruins the print |

Skirting: 0.15 m dark oak in every room except kitchens/sculleries/cellars (none) and bathrooms (tile). Every ground-floor room has one LightSwitch by its main door at 1.3 m; every room has the existing `Room` component.

### Doors (how the rooms connect)
| Door | Level | From -> to | Wall | Centre at | W x H | Leaf | Role |
|---|---|---|---|---|---|---|---|
| d_front | G | hall -> ext | N | -0.4 | 0.9 x 2.05 | black door with etched glass 'LISLE', in a tiled recess (Black) | front |
| d_back | G | kitchen -> ext | W | -1.0 | 0.85 x 2.05 | panel | back |
| d_rec | G | hall -> reception | W | 3.0 | 0.85 x 2.05 | panel |  |
| d_par | G | hall -> parlour | E | 3.0 | 0.85 x 2.05 | panel |  |
| d_kit | G | hall -> kitchen | W | -2.0 | 0.85 x 2.05 | panel |  |
| d_print | G | hall -> print | E | -3.2 | 0.85 x 2.05 | panel |  |
| d_studio | G | hall -> studio | S | -0.4 | 0.9 x 2.05 | glazed double door into the studio |  |
| d_b1 | F | landing -> bed1 | W | 3.0 | 0.8 x 2.05 | panel |  |
| d_b2 | F | landing -> bed2 | E | 3.0 | 0.8 x 2.05 | panel |  |
| d_bath | F | landing -> bath | W | -2.0 | 0.75 x 2.05 | panel |  |
| d_store | F | landing -> store | E | -3.2 | 0.75 x 2.05 | panel |  |

### Windows
| Window | Room | Wall | Centre at | W x H | Sill | Style | Weak | Curtains / shutters |
|---|---|---|---|---|---|---|---|---|
| w_shop | reception | N | -3.0 | 2.4 x 1.7 | 0.6 | shop window, gilt letters 'LISLE - PORTRAITS', framed portraits on easels |  |   |
| w_par | parlour | N | 3.0 | 1.4 x 1.5 | 0.9 | sash |  | FabricTerlenka  |
| w_k | kitchen | W | -3.0 | 0.9 x 1.0 | 0.9 | sash, cracked | **WEAK** (break-in point) |   |
| w_print | print | E | -2.0 | 1.0 x 1.2 | 0.9 | sash, loose | **WEAK** (break-in point) |   |
| w_st1 | studio | W | -7.0 | 3.0 x 1.8 | 0.9 | glass wall bay (studio) |  |   |
| w_st2 | studio | E | -7.0 | 3.0 x 1.8 | 0.9 | glass wall bay (studio), 3 panes broken |  |   |
| w_b1 | bed1 | N | -3.0 | 1.2 x 1.4 | 0.9 | sash |  |   |
| w_b2 | bed2 | N | 3.0 | 1.2 x 1.4 | 0.9 | sash |  |   |
| w_bath | bath | W | -2.0 | 0.7 x 0.8 | 1.3 | frosted |  |   |

### Stairs, ladders and voids
| Id | In room | Rect | Goes | Up toward | Type |
|---|---|---|---|---|---|
| s1 | hall | (0.0, -2.4, 0.9, 1.1) | G -> F | S | straight - 15 risers, Carpet70sB runner |
| s1u | landing | (0.0, -2.4, 0.9, 1.1) | G -> F | S | void |
| s2 | print | (3.7, -3.6, 4.6, -0.1) | C -> G | N | cellar stair along the print room's east wall, light-tight curtain at the top |
| s2c | cellar | (3.7, -3.6, 4.6, -0.1) | C -> G | N | arrives |

## 3. Every prop, room by room

Position = prop **pivot at the centre of its footprint, on the floor** (or on top of its parent for `on`, or the centre height `y` for wall-hung). Yaw = the way the prop's FRONT faces. Scale the asset so its bounds fit the listed size (see ASSETS.md 'Scaling rule').

### reception - Reception (portrait shop)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 1 | `Counter` | BUILD (PropKit) | -3.00 | 0.00 | 0.32 | 0 | 1.8 x 0.45 x 0.71 | floor |  small counter with an order book |
| 2 | `VintageCamera` | Poly Haven `Camera_01` (1k) | -2.50 | 0.71 | 0.32 | 0 | 0.18 x 0.14 x 0.16 | on H10.p001 |   |
| 3 | `FrameFancy2` | Poly Haven `fancy_picture_frame_02` (1k) | -4.62 | 1.32 | 3.00 | 90 | 0.45 x 0.05 x 0.55 | wall-hung | family-photo FAMILY PHOTO: the Lisles in the studio, 1970 |
| 4 | `FrameFancy1` | Poly Haven `fancy_picture_frame_01` (1k) | -4.62 | 1.23 | 1.00 | 90 | 0.6 x 0.05 x 0.75 | wall-hung |  portrait of a village family |
| 5 | `ArmchairGreen` | Poly Haven `GreenChair_01` (2k) | -2.00 | 0.00 | 2.00 | 270 | 0.75 x 0.8 x 0.95 | floor |   |

### parlour - Parlour
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 6 | `Hearth` | BUILD (PropKit) | 4.27 | 0.00 | 2.00 | 270 | 1.8 x 0.7 x 1.4 | floor | hearth-decor tiled fireplace (decorative) |
| 7 | `MantelClock` | Poly Haven `mantel_clock_01` (1k) | 4.27 | 1.40 | 2.00 | 270 | 0.3 x 0.14 x 0.32 | on H10.p006 |  stopped at 12:00 |
| 8 | `Sofa3` | Poly Haven `sofa_03` (2k) | 3.00 | 0.00 | 0.52 | 0 | 1.8 x 0.85 x 0.8 | floor |   |
| 9 | `Cabinet70s` | Poly Haven `modern_wooden_cabinet` (1k) | 1.31 | 0.00 | 1.20 | 90 | 1.2 x 0.45 x 0.8 | floor |  radiogram |
| 10 | `TV2` | Poly Haven `television_02` (1k) | 1.31 | 0.80 | 1.20 | 90 | 0.6 x 0.45 x 0.5 | on H10.p009 | tv  |
| 11 | `SideTable` | Poly Haven `side_table_01` (1k) | 4.20 | 0.00 | 3.37 | 180 | 0.5 x 0.5 x 0.6 | floor |   |
| 12 | `RotaryPhone` | Sketchfab `812e8f59f9e749fab483eabf4800dec2` (CC BY, sara El-sisi) | 4.20 | 0.60 | 3.37 | 180 | 0.22 x 0.25 x 0.14 | on H10.p011 | phone  |
| 13 | `CoffeeTableRound` | Poly Haven `coffee_table_round_01` (1k) | 3.00 | 0.00 | 2.00 | 0 | 0.8 x 0.8 x 0.45 | floor |   |

### kitchen - Kitchen
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 14 | `Hearth` | BUILD (PropKit) | -4.27 | 0.00 | -2.60 | 90 | 1.8 x 0.7 x 1.4 | floor | hearth range in the west-gable flue = THE HEARTH |
| 15 | `TablePainted` | Poly Haven `painted_wooden_table` (1k) | -2.60 | 0.00 | -1.50 | 0 | 1.2 x 0.7 x 0.76 | floor |   |
| 16 | `ChairPainted1` | Poly Haven `painted_wooden_chair_01` (1k) | -2.60 | 0.00 | -0.85 | 180 | 0.45 x 0.48 x 0.9 | floor |   |
| 17 | `KitchenUnit` | BUILD (PropKit) | -2.00 | 0.00 | -3.32 | 0 | 0.6 x 0.6 x 0.9 | floor |   |
| 18 | `KitchenSinkUnit` | BUILD (PropKit) | -3.20 | 0.00 | -3.32 | 0 | 1.0 x 0.6 x 0.9 | floor |   |
| 19 | `FuseBox` | Poly Haven `power_box_01` (1k) | -1.14 | 1.40 | -0.80 | 270 | 0.45 x 0.15 x 0.6 | wall-hung | fuse-box  |

### print - Print & retouching room
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 20 | `TableWood3` | Poly Haven `WoodenTable_03` (1k) | 2.40 | 0.00 | -0.54 | 180 | 2.0 x 0.9 x 0.78 | floor |  retouching desk with a light box |
| 21 | `DeskLamp` | Poly Haven `desk_lamp_arm_01` (1k) | 1.70 | 0.78 | -0.54 | 180 | 0.25 x 0.4 x 0.5 | on H10.p020 |   |
| 22 | `Magnifier` | Poly Haven `magnifying_glass_01` (1k) | 2.60 | 0.78 | -0.54 | 180 | 0.25 x 0.1 x 0.03 | on H10.p020 |   |
| 23 | `IronSafe` | BUILD (PropKit) | 1.39 | 0.00 | -1.80 | 90 | 0.6 x 0.6 x 0.8 | floor | puzzle container NEGATIVE SAFE: 4 code dials |
| 24 | `DrawerCabinet` | Poly Haven `drawer_cabinet` (1k) | 2.60 | 0.00 | -3.40 | 0 | 0.6 x 0.45 x 1.1 | floor |  print drawers |

### studio - Glass studio
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 25 | `Backdrop` | BUILD (PropKit) | 0.00 | 0.00 | -9.62 | 0 | 3.0 x 0.4 x 2.8 | floor |   |
| 26 | `Chair` | Poly Haven `WoodenChair_01` (2k) | 0.00 | 0.00 | -8.60 | 0 | 0.45 x 0.5 x 0.95 | floor |  posing chair |
| 27 | `StudioLamp` | BUILD (PropKit) | -1.80 | 0.00 | -7.20 | 0 | 0.5 x 0.5 x 1.8 | floor |   |
| 28 | `StudioLamp` | BUILD (PropKit) | 1.80 | 0.00 | -7.20 | 0 | 0.5 x 0.5 x 1.8 | floor |   |
| 29 | `VintageCamera` | Poly Haven `Camera_01` (1k) | 0.00 | 1.30 | -5.60 | 0 | 0.18 x 0.14 x 0.16 | raised | allow-overlap plate camera on a wooden tripod (tripod = 3 thin cylinders) |
| 30 | `Commode` | Poly Haven `GothicCommode_01` (2k) | -3.57 | 0.00 | -5.00 | 90 | 1.1 x 0.5 x 0.9 | floor |  props drawer: hats, gloves, a doll |
| 31 | `CardboardBox` | Poly Haven `cardboard_box_01` (1k) | 3.62 | 0.00 | -5.00 | 270 | 0.5 x 0.4 x 0.4 | floor |   |
| 32 | `BenchPainted` | Poly Haven `painted_wooden_bench` (1k) | 3.62 | 0.00 | -9.00 | 270 | 1.4 x 0.4 x 0.45 | floor |  family bench for group portraits |

### bed1 - Lisles' bedroom
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 39 | `Bed` | Poly Haven `GothicBed_01` (2k) | -3.57 | 0.00 | 2.00 | 90 | 1.6 x 2.1 x 1.4 | floor |   |
| 40 | `Nightstand` | Poly Haven `ClassicNightstand_01` (1k) | -4.42 | 0.00 | 0.90 | 90 | 0.5 x 0.4 x 0.6 | floor |   |
| 41 | `Nightstand` | Poly Haven `ClassicNightstand_01` (1k) | -4.42 | 0.00 | 3.10 | 90 | 0.5 x 0.4 x 0.6 | floor |   |
| 42 | `Wardrobe` | Poly Haven `GothicCabinet_01` (2k) | -2.60 | 0.00 | 0.39 | 0 | 1.2 x 0.6 x 2.1 | floor |   |

### landing - Landing
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 53 | `ShrineNiche` | BUILD (PropKit) | 0.40 | 0.90 | 3.52 | 180 | 0.6 x 0.25 x 0.9 | wall-hung | shrine  |

### bed2 - Daughter's bedroom
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 43 | `BedIron` | Poly Haven `old_bed_frame` (2k) | 3.62 | 0.00 | 2.00 | 270 | 1.0 x 2.0 x 1.05 | floor |   |
| 44 | `NightstandPainted` | Poly Haven `painted_wooden_nightstand` (1k) | 4.43 | 0.00 | 1.23 | 270 | 0.45 x 0.38 x 0.6 | floor |   |
| 45 | `TableSmall` | Poly Haven `small_wooden_table_01` (1k) | 3.60 | 0.00 | 0.34 | 0 | 0.7 x 0.5 x 0.72 | floor |   |

### bath - Bathroom
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 46 | `Bathtub` | Sketchfab `d06c028a89a54c469e4af9cac50606fa` (CC BY, Lady Lion Studios) | -3.20 | 0.00 | -3.25 | 0 | 1.65 x 0.75 x 0.62 | floor |   |
| 47 | `Sink` | Sketchfab `a278d63585494921906ab3500c421ff7` (CC BY, lagesnpiet) | -3.20 | 0.00 | -0.32 | 180 | 0.55 x 0.45 x 0.85 | floor |   |
| 48 | `Mirror` | Poly Haven `ornate_mirror_01` (1k) | -3.20 | 1.10 | -0.09 | 180 | 0.6 x 0.05 x 0.9 | wall-hung | mirror  |
| 49 | `Toilet` | Sketchfab `6ac515a1c4154db18b5b4bd0b46d6405` (CC BY, Allan-Jay Branscombe) | -4.29 | 0.00 | -0.80 | 90 | 0.4 x 0.65 x 0.8 | floor |   |

### store - Negative store
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 50 | `SteelShelves` | Poly Haven `steel_frame_shelves_01` (1k) | 3.00 | 0.00 | -3.40 | 0 | 1.2 x 0.45 x 1.9 | floor |  glass plates in grooved racks |
| 51 | `SteelShelves2` | Poly Haven `steel_frame_shelves_02` (1k) | 3.00 | 0.00 | -0.29 | 180 | 1.0 x 0.4 x 1.8 | floor |   |
| 52 | `CardboardBox` | Poly Haven `cardboard_box_01` (1k) | 4.42 | 0.00 | -2.00 | 270 | 0.5 x 0.4 x 0.4 | floor |   |

### cellar - Darkroom cellar (PUZZLE)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 33 | `Enlarger` | Sketchfab `3174dc4d4f9d4b3e969ce9135ec6d423` (CC BY, MarcoFromIADE) | -3.50 | 0.00 | -0.66 | 180 | 0.45 x 0.55 x 1.1 | floor |   |
| 34 | `DarkroomTrays` | BUILD (PropKit) | -1.50 | 0.00 | -0.63 | 180 | 1.4 x 0.5 x 0.9 | floor |  developer / stop / fix trays + tongs + timer |
| 35 | `Safelight` | BUILD (PropKit) | -2.50 | 1.90 | -0.42 | 180 | 0.2 x 0.15 x 0.2 | wall-hung |   |
| 36 | `SteelShelves` | Poly Haven `steel_frame_shelves_01` (1k) | 2.00 | 0.00 | -3.40 | 0 | 1.2 x 0.45 x 1.9 | floor |  chemicals, boxes of paper (DO NOT OPEN IN LIGHT) |
| 37 | `TableSmall` | Poly Haven `small_wooden_table_01` (1k) | -4.37 | 0.00 | -2.00 | 90 | 0.7 x 0.5 x 0.72 | floor |   |
| 38 | `CellarCrates` | BUILD (PropKit) | 0.00 | 0.00 | -3.17 | 0 | 1.4 x 0.9 x 1.2 | floor |   |

## 4. Gameplay anchors (empty GameObjects the builder must create)
| Kind | Room | x | y | z | What |
|---|---|---|---|---|---|
| note-spot | reception | -3.10 | 0.71 | 0.32 | order book: 'collect the Abney plate - do NOT print it' (note 1/3) |
| item-spot | kitchen | -4.27 | 1.40 | -2.60 | range mantel |
| puzzle | print | 1.39 | 0.00 | -1.80 | CodeLockPuzzle: develop the hidden glass plate in the cellar darkroom (safelight on, white light off) - the print shows the studio wall clock; the code is written on the blackboard behind the sitters |
| container | print | 1.39 | 0.00 | -1.80 | Glass negative in a paper sleeve |
| item-spot | print | 2.60 | 1.10 | -3.40 | print drawers |
| hiding | studio | -3.40 | 0.00 | -9.60 | behind the rolled backdrops in the SW corner |
| note-spot | cellar | -4.37 | 0.72 | -1.90 | Mr Lisle's darkroom rules card (note 2/3) |
| hiding | cellar | 0.00 | 0.00 | -3.17 | behind the crates in the dark -> HidingSpot on H10.p038 (CellarCrates) - crouched |
| hiding | bed1 | -3.57 | 0.00 | 2.00 | under the bed -> HidingSpot on H10.p039 (Bed) - crouched |
| hiding | bed1 | -2.60 | 0.00 | 0.39 | wardrobe -> HidingSpot on H10.p042 (Wardrobe) |
| note-spot | bed2 | 3.50 | 0.72 | 0.34 | daughter's photo album with captions (note 3/3) |
| hiding | bath | -3.20 | 0.00 | -3.25 | behind the bath curtain -> HidingSpot on H10.p046 (Bathtub) |
| item-spot | store | 4.42 | 0.40 | -2.00 | box of glass plates |
| shrine | landing | 0.40 | 0.00 | 3.52 | Rite of Return point (active only if this house is a shrine this match) |

Mapping to existing code: `item-spot` -> `House.ItemSpots` (order as listed), `note-spot` -> the 3 `Readable` notes, `puzzle` -> `PuzzleRig` anchor, `container` -> `PartContainer`, `shrine` -> `House.ShrineMarker` (+ shrine niche inside), `hiding` -> `HidingSpot`.

## 5. Puzzle - Glass negative
1. LOCK FIRST: the iron negative safe in the print room has 4 dials and a framed print above it with a blank space.
2. Find the undeveloped glass plate (one item spot). Go to the cellar darkroom, close the light-tight curtain.
3. Switch the white bulb OFF and the safelight ON (if white light is on, the plate is ruined - reset after 30 s).
4. Enlarger -> developer -> stop -> fix (3 trays, in order, each a hold-interact). Reading the print = the code on the blackboard behind the sitters (randomised).
5. Part: Glass negative.

## 6. Night changes (DESIGN 'Night changes' - pick 2-3 per match)
- a new portrait in the shop window shows the players standing in the studio
- the studio camera's cloth is draped over the posing chair as if over a person
- the safelight comes on by itself
- plus the shared pool: extra person in the family photo, a door opening onto a brick wall, moved furniture, an extra hallway door, all clocks stopped at the same time, writing on a wall in a player's name

## 7. Sound
- paper: print handling; locks: safe dials; a shutter click (camera) when someone enters the studio
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
