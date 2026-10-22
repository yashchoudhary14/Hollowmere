# H06 - Twins' House (Abney)

> Machine-readable twin: `docs/design/houses/H06.json` - the builder reads THAT file; this page explains it. Every number here comes from the same data and is validated (no overlaps, door clearances, required items).

| | |
|---|---|
| **Archetype** | Two identical Victorian brick cottages knocked into one, perfectly mirrored |
| **Silhouette (must read at 50 m in fog)** | Wide and symmetrical: TWO front doors side by side (both yellow; the west one is screwed shut), two identical gable chimneys, twin bay windows, twin dormers. Everything on the west is the mirror image of the east... almost. |
| **Ritual part** | Silver locket |
| **M1 puzzle rig (code)** | `PuzzleKind.Sequence` |
| **Letter traits (HouseCatalog)** | door **Yellow**, **2** chimney(s), stopped clock **2:00**, family **Abney** |
| **World position** | x -24.0, z 29.0, floor level y +0.00, yaw 180 deg - The Green (north side) |
| **Front door (world)** | (-25.0, 25.0) - 90.5 m from the altar, 13.3 m from the lane |
| **Placement note** | two front gardens split by a wall, facing the green |

## Reference images

![plan](../ref/H06_plan.png)

![front-left](../ref/H06_massing_front.png) ![back-right](../ref/H06_massing_back.png)

Real-world references (look, don't copy): [Semi-detached](https://en.wikipedia.org/wiki/Semi-detached)

Mood-image prompt (Higgsfield / Gemini / ChatGPT image - save the result as `docs/design/ref/H06_mood.png`):

```text
two identical Victorian cream-limewashed brick cottages joined as one perfectly symmetrical house, two yellow front doors side by side, twin bay windows, twin dormers, twin gable chimneys, eerie symmetry, photorealistic, late 1970s rural English village, overcast autumn dusk, damp, muted greens and greys, 35mm film still, Phasmophobia / Resident Evil Village mood, no people, eye-level, 16:9
```

## 1. Structure

### Levels
| Level | Floor y (m) | Clear height (m) |
|---|---|---|
| G - Ground floor | +0.00 | 2.70 |
| F - First floor | +2.95 | 2.50 |
| A - Attic (under the roof) | +5.70 | 2.30 |

Floor build-up: 0.25 m between floors (joists + boards); ceilings use the room's `ceiling` material.

### Wings (massing)
| Wing | Rect x0,z0 -> x1,z1 | Size | Levels | Walls | Wall thickness | Roof |
|---|---|---|---|---|---|---|
| main | (-7.0, -4.0) -> (7.0, 4.0) | 14.0 x 8.0 | G, F, A | BrickPaintedWorn (cream limewash over red brick, worn through on the lower 1 m) with BrickRed window arches | 0.35 | Gable roof 38 deg, ridge along x at 7.9 m, RoofClay3; two front dormers at x=+-4.2 with RoofClay3 cheeks; gable chimneys at x=+-7.0 |

### Chimneys (count must equal HouseCatalog = 2)
| x | z | stack w x d | above ridge | pots | serves |
|---|---|---|---|---|---|
| -7.0 | -1.5 | 0.8 x 0.8 | 1.0 | 2 | west kitchen range (cold, identical decoy hearth) |
| 7.0 | -1.5 | 0.8 x 0.8 | 1.0 | 2 | east kitchen range = THE HEARTH |

### Exterior details
- TWO yellow front doors side by side: '7A' (west) screwed shut from inside, '7B' (east) is the real door
- twin bays, twin dormers, twin chimneys - perfectly mirrored
- DIFFERENCE (exterior, always on): the east gable window is bricked up, the west one is not
- two milk crates, two boot scrapers, two doorbells - one doorbell works

### Plot, garden and yard
- Front: two identical 4 m gardens split by a low wall, each with one standard rose (one dead)
- Back: one shared garden, two identical sheds, two washing lines (one with a child's dress, one empty)
- Plot boundary (world polygon): (-15.5,41.0), (-32.5,41.0), (-32.5,21.0), (-15.5,21.0). Boundary = DryStoneWall / FencePicket on the street side, FenceRail or hedges (Shrub1-4) elsewhere, a GardenGate on every path.

## 2. Rooms

| Room | Level | Rect (x0,z0,x1,z1) | Clear inside | Kind | Floor | Walls | Ceiling | Light |
|---|---|---|---|---|---|---|---|---|
| **hall** Double hall (knocked through) | G | (-2.0, -4.0, 2.0, 4.0) | 3.88 x 7.30 | hall | FloorWornTile | Wallpaper2 (west) / Wallpaper2 (east) | CeilingInterior | pendant |
| **parW** West parlour (Ada's) | G | (-7.0, 0.0, -2.0, 4.0) | 4.59 x 3.59 | living | Carpet70s | WallpaperJacquard | CeilingInterior | pendant |
| **parE** East parlour (Ida's) | G | (2.0, 0.0, 7.0, 4.0) | 4.59 x 3.59 | living | Carpet70s | WallpaperJacquard | CeilingInterior | pendant |
| **kitW** West kitchen (cold) | G | (-7.0, -4.0, -2.0, 0.0) | 4.59 x 3.59 | kitchen | FloorQuarryTile | Whitewash | CeilingInterior | pendant |
| **kitE** East kitchen (lived in) | G | (2.0, -4.0, 7.0, 0.0) | 4.59 x 3.59 | kitchen | FloorQuarryTile | Whitewash | CeilingInterior | pendant |
| **landing** Double landing | F | (-2.0, -4.0, 2.0, 1.5) | 3.88 x 5.09 | landing | FloorOldWood1 | Wallpaper2 | CeilingInterior | pendant |
| **bath** Bathroom (between the halves) | F | (-2.0, 1.5, 2.0, 4.0) | 3.88 x 2.09 | bathroom | FloorBathTile | TileWallWhite | CeilingInterior | pendant |
| **bedW** West bedroom (Ada) | F | (-7.0, 0.0, -2.0, 4.0) | 4.59 x 3.59 | bedroom | Carpet70sB | Wallpaper1 | CeilingInterior | pendant |
| **bedE** East bedroom (Ida) | F | (2.0, 0.0, 7.0, 4.0) | 4.59 x 3.59 | bedroom | Carpet70sB | Wallpaper1 | CeilingInterior | pendant |
| **nurW** West back room (the cots) | F | (-7.0, -4.0, -2.0, 0.0) | 4.59 x 3.59 | bedroom | FloorOldWood2 | Wallpaper1B | CeilingInterior | pendant |
| **nurE** East back room (the cots) | F | (2.0, -4.0, 7.0, 0.0) | 4.59 x 3.59 | bedroom | FloorOldWood2 | Wallpaper1B | CeilingInterior | pendant |
| **attic** Long attic (PUZZLE end) | A | (-7.0, -4.0, 7.0, 4.0) | 13.30 x 7.30 | attic | FloorOldWood2 | RawPlankWall | CeilingInterior | pendant |

Skirting: 0.15 m dark oak in every room except kitchens/sculleries/cellars (none) and bathrooms (tile). Every ground-floor room has one LightSwitch by its main door at 1.3 m; every room has the existing `Room` component.

### Doors (how the rooms connect)
| Door | Level | From -> to | Wall | Centre at | W x H | Leaf | Role |
|---|---|---|---|---|---|---|---|
| d_frontE | G | hall -> ext | N | 1.0 | 0.9 x 2.05 | yellow 4-panel door, brass '7B' (Yellow) | front |
| d_frontW | G | hall -> ext | N | -1.0 | 0.9 x 2.05 | yellow 4-panel door '7A', screwed shut from inside (cannot open) (Yellow) | sealed |
| d_back | G | hall -> ext | S | 0.0 | 0.9 x 2.05 | panel | back |
| d_parW | G | hall -> parW | W | 2.4 | 0.85 x 2.05 | panel |  |
| d_kitW | G | parW -> kitW | S | -4.3 | 0.85 x 2.05 | panel |  |
| d_parE | G | hall -> parE | E | 2.4 | 0.85 x 2.05 | panel |  |
| d_kitE | G | parE -> kitE | S | 4.3 | 0.85 x 2.05 | panel |  |
| d_bedW | F | landing -> bedW | W | 0.75 | 0.8 x 2.05 | panel |  |
| d_bedE | F | landing -> bedE | E | 0.75 | 0.8 x 2.05 | panel |  |
| d_nurW | F | bedW -> nurW | S | -3.2 | 0.8 x 2.05 | panel |  |
| d_nurE | F | bedE -> nurE | S | 3.2 | 0.8 x 2.05 | panel |  |
| d_bath | F | landing -> bath | N | 0.0 | 0.8 x 2.05 | panel |  |

### Windows
| Window | Room | Wall | Centre at | W x H | Sill | Style | Weak | Curtains / shutters |
|---|---|---|---|---|---|---|---|---|
| w_bayW | parW | N | -4.5 | 1.8 x 1.5 | 0.7 | twin canted bay (shown flat) |  | FabricTerlenka  |
| w_bedW | bedW | N | -4.5 | 1.1 x 1.2 | 0.9 | dormer sash |  |   |
| w_nurW | nurW | S | -4.5 | 1.0 x 1.1 | 0.9 | sash |  |   |
| w_bayE | parE | N | 4.5 | 1.8 x 1.5 | 0.7 | twin canted bay (shown flat) |  | FabricTerlenka  |
| w_bedE | bedE | N | 4.5 | 1.1 x 1.2 | 0.9 | dormer sash |  |   |
| w_nurE | nurE | S | 4.5 | 1.0 x 1.1 | 0.9 | sash |  |   |
| w_kW | kitW | S | -4.5 | 1.0 x 1.0 | 0.9 | sash (west) | **WEAK** (break-in point) |   |
| w_kE | kitE | S | 4.5 | 1.0 x 1.0 | 0.9 | sash (east) | **WEAK** (break-in point) |   |
| w_attic | attic | W | 0.0 | 0.6 x 0.6 | 1.0 | gable light (only on the WEST gable - the east one is bricked up: a difference) |  |   |

### Stairs, ladders and voids
| Id | In room | Rect | Goes | Up toward | Type |
|---|---|---|---|---|---|
| sW | hall | (-1.9, -3.6, -0.9999999999999999, -0.1) | G -> F | N | straight - W stair, mirror image of the other, 13 risers |
| sWu | landing | (-1.9, -3.6, -0.9999999999999999, -0.1) | G -> F | N | void |
| sE | hall | (1.0, -3.6, 1.9, -0.1) | G -> F | N | straight - E stair, mirror image of the other, 13 risers |
| sEu | landing | (1.0, -3.6, 1.9, -0.1) | G -> F | N | void |
| s3 | landing | (-0.45, -1.4, 0.45, -0.4) | F -> A | S | loft ladder between the two stair wells |

## 3. Every prop, room by room

Position = prop **pivot at the centre of its footprint, on the floor** (or on top of its parent for `on`, or the centre height `y` for wall-hung). Yaw = the way the prop's FRONT faces. Scale the asset so its bounds fit the listed size (see ASSETS.md 'Scaling rule').

### hall - Double hall (knocked through)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 23 | `FuseBox` | Poly Haven `power_box_01` (1k) | -1.20 | 1.40 | -3.58 | 0 | 0.45 x 0.15 x 0.6 | wall-hung | fuse-box  |
| 24 | `Mirror` | Poly Haven `ornate_mirror_01` (1k) | 0.00 | 1.35 | 3.62 | 180 | 0.6 x 0.05 x 0.9 | wall-hung |  mirror between the two front doors |

### parW - West parlour (Ada's)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 9 | `Sofa3` | Poly Haven `sofa_03` (2k) | -6.20 | 0.00 | 2.00 | 90 | 1.8 x 0.85 x 0.8 | floor |   |
| 11 | `SideTable` | Poly Haven `side_table_01` (1k) | -3.00 | 0.00 | 0.34 | 0 | 0.5 x 0.5 x 0.6 | floor |   |
| 14 | `Vase2` | Poly Haven `ceramic_vase_02` (1k) | -3.00 | 0.60 | 0.34 | 0 | 0.18 x 0.18 x 0.3 | on H06.p011 |   |
| 15 | `Cabinet70s` | Poly Haven `modern_wooden_cabinet` (1k) | -5.60 | 0.00 | 0.32 | 0 | 1.2 x 0.45 x 0.8 | floor |   |
| 17 | `TV2` | Poly Haven `television_02` (1k) | -5.60 | 0.80 | 0.32 | 0 | 0.6 x 0.45 x 0.5 | on H06.p015 | tv  |
| 19 | `FrameHanging1` | Poly Haven `hanging_picture_frame_01` (1k) | -2.08 | 1.35 | 3.20 | 270 | 0.4 x 0.03 x 0.5 | wall-hung | family-photo the twins aged 6 (west) |
| 21 | `WallClock` | Poly Haven `wall_clock` (1k) | -2.09 | 1.93 | 1.00 | 270 | 0.35 x 0.06 x 0.35 | wall-hung |  clock (west) - DIFFERENCE candidate: hands at 2:00 vs 10:00 mirrored |

### parE - East parlour (Ida's)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 10 | `Sofa3` | Poly Haven `sofa_03` (2k) | 6.20 | 0.00 | 2.00 | 270 | 1.8 x 0.85 x 0.8 | floor |   |
| 12 | `SideTable` | Poly Haven `side_table_01` (1k) | 3.00 | 0.00 | 0.34 | 0 | 0.5 x 0.5 x 0.6 | floor |   |
| 13 | `RotaryPhone` | Sketchfab `812e8f59f9e749fab483eabf4800dec2` (CC BY, sara El-sisi) | 3.00 | 0.60 | 0.34 | 0 | 0.22 x 0.25 x 0.14 | on H06.p012 | phone phone ONLY on the east side (west has the matching table, empty) |
| 16 | `Cabinet70s` | Poly Haven `modern_wooden_cabinet` (1k) | 5.60 | 0.00 | 0.32 | 0 | 1.2 x 0.45 x 0.8 | floor |   |
| 18 | `TV2` | Poly Haven `television_02` (1k) | 5.60 | 0.80 | 0.32 | 0 | 0.6 x 0.45 x 0.5 | on H06.p016 | tv  |
| 20 | `FrameHanging1` | Poly Haven `hanging_picture_frame_01` (1k) | 2.08 | 1.35 | 3.20 | 90 | 0.4 x 0.03 x 0.5 | wall-hung | family-photo the twins aged 6 (east) - identical except one girl is missing |
| 22 | `WallClock` | Poly Haven `wall_clock` (1k) | 2.09 | 1.93 | 1.00 | 90 | 0.35 x 0.06 x 0.35 | wall-hung |  clock (east) |

### kitW - West kitchen (cold)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 1 | `Hearth` | BUILD (PropKit) | -6.27 | 0.00 | -1.50 | 90 | 1.8 x 0.7 x 1.4 | floor | hearth cast-iron range; WEST is cold & swept, EAST is THE HEARTH |
| 3 | `Dresser` | BUILD (PropKit) | -2.80 | 0.00 | -0.34 | 180 | 1.4 x 0.5 x 2.0 | floor |   |
| 5 | `KitchenSinkUnit` | BUILD (PropKit) | -5.60 | 0.00 | -3.32 | 0 | 1.0 x 0.6 x 0.9 | floor |   |
| 7 | `TableWood1` | Poly Haven `WoodenTable_01` (1k) | -4.00 | 0.00 | -2.00 | 0 | 1.4 x 0.8 x 0.76 | floor |   |

### kitE - East kitchen (lived in)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 2 | `Hearth` | BUILD (PropKit) | 6.27 | 0.00 | -1.50 | 270 | 1.8 x 0.7 x 1.4 | floor | hearth cast-iron range; WEST is cold & swept, EAST is THE HEARTH |
| 4 | `Dresser` | BUILD (PropKit) | 2.80 | 0.00 | -0.34 | 180 | 1.4 x 0.5 x 2.0 | floor |   |
| 6 | `KitchenSinkUnit` | BUILD (PropKit) | 5.60 | 0.00 | -3.32 | 0 | 1.0 x 0.6 x 0.9 | floor |   |
| 8 | `TableWood1` | Poly Haven `WoodenTable_01` (1k) | 4.00 | 0.00 | -2.00 | 0 | 1.4 x 0.8 x 0.76 | floor |   |

### landing - Double landing
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 47 | `ShrineNiche` | BUILD (PropKit) | 1.00 | 0.90 | -3.52 | 0 | 0.6 x 0.25 x 0.9 | wall-hung | shrine  |

### bath - Bathroom (between the halves)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 37 | `Bathtub` | Sketchfab `d06c028a89a54c469e4af9cac50606fa` (CC BY, Lady Lion Studios) | -0.60 | 0.00 | 3.30 | 180 | 1.4 x 0.64 x 0.53 | floor |   |
| 38 | `Toilet` | Sketchfab `6ac515a1c4154db18b5b4bd0b46d6405` (CC BY, Allan-Jay Branscombe) | 1.58 | 0.00 | 3.00 | 270 | 0.4 x 0.65 x 0.8 | floor |   |
| 39 | `Sink` | Sketchfab `a278d63585494921906ab3500c421ff7` (CC BY, lagesnpiet) | 1.40 | 0.00 | 1.81 | 0 | 0.55 x 0.45 x 0.85 | floor |   |
| 40 | `Mirror` | Poly Haven `ornate_mirror_01` (1k) | 1.40 | 1.10 | 1.58 | 0 | 0.6 x 0.05 x 0.9 | wall-hung | mirror the mirror that shows the 'other' half of the house |

### bedW - West bedroom (Ada)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 25 | `Bed` | Poly Haven `GothicBed_01` (2k) | -5.57 | 0.00 | 2.20 | 90 | 1.6 x 2.1 x 1.4 | floor |   |
| 26 | `Nightstand` | Poly Haven `ClassicNightstand_01` (1k) | -6.42 | 0.00 | 1.10 | 90 | 0.5 x 0.4 x 0.6 | floor |   |
| 27 | `Nightstand` | Poly Haven `ClassicNightstand_01` (1k) | -6.42 | 0.00 | 3.30 | 90 | 0.5 x 0.4 x 0.6 | floor |   |
| 31 | `Wardrobe` | Poly Haven `GothicCabinet_01` (2k) | -4.60 | 0.00 | 0.39 | 0 | 1.2 x 0.6 x 2.1 | floor |   |

### bedE - East bedroom (Ida)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 28 | `Bed` | Poly Haven `GothicBed_01` (2k) | 5.57 | 0.00 | 2.20 | 270 | 1.6 x 2.1 x 1.4 | floor |   |
| 29 | `Nightstand` | Poly Haven `ClassicNightstand_01` (1k) | 6.42 | 0.00 | 1.10 | 270 | 0.5 x 0.4 x 0.6 | floor |   |
| 30 | `Nightstand` | Poly Haven `ClassicNightstand_01` (1k) | 6.42 | 0.00 | 3.30 | 270 | 0.5 x 0.4 x 0.6 | floor |   |
| 32 | `Wardrobe` | Poly Haven `GothicCabinet_01` (2k) | 4.60 | 0.00 | 0.39 | 0 | 1.2 x 0.6 x 2.1 | floor |   |

### nurW - West back room (the cots)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 33 | `BabyCrib` | Sketchfab `b945651a5fa841b7aef4ea4da81fcc39` (CC BY, Rukas) | -5.60 | 0.00 | -3.29 | 0 | 1.2 x 0.65 x 1.0 | floor |   |
| 35 | `ChestOfDrawers` | Poly Haven `vintage_wooden_drawer_01` (1k) | -5.60 | 0.00 | -0.33 | 180 | 0.9 x 0.48 x 1.0 | floor |   |

### nurE - East back room (the cots)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 34 | `BabyCrib` | Sketchfab `b945651a5fa841b7aef4ea4da81fcc39` (CC BY, Rukas) | 5.60 | 0.00 | -3.29 | 0 | 1.2 x 0.65 x 1.0 | floor |   |
| 36 | `ChestOfDrawers` | Poly Haven `vintage_wooden_drawer_01` (1k) | 5.60 | 0.00 | -0.33 | 180 | 0.9 x 0.48 x 1.0 | floor |   |

### attic - Long attic (PUZZLE end)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 41 | `ChestWorn` | owned: Res_Sto_Chest_Wood_Worn_08 (Fab Saloon Interior) | 6.37 | 0.00 | 0.00 | 270 | 0.9 x 0.5 x 0.55 | floor | puzzle container THE LOCKET BOX: a double music box with two keyholes; Sequence panel = 6 brass tabs on its lid |
| 42 | `Suitcase` | Poly Haven `vintage_suitcase` (1k) | -6.52 | 0.00 | 0.00 | 90 | 0.65 x 0.2 x 0.45 | floor |   |
| 43 | `CellarCrates` | BUILD (PropKit) | -4.00 | 0.00 | 3.17 | 180 | 1.4 x 0.9 x 1.2 | floor |   |
| 44 | `RockingHorse` | Sketchfab `5168ce9d45a3401ea6fe4d92f17bb332` (CC BY, Alex Petuhov) | 4.00 | 0.00 | -3.44 | 0 | 0.9 x 0.35 x 0.7 | floor |   |
| 45 | `TableSmall` | Poly Haven `small_wooden_table_01` (1k) | -3.00 | 0.00 | -3.37 | 0 | 0.7 x 0.5 x 0.72 | floor |   |
| 46 | `CardboardBox` | Poly Haven `cardboard_box_01` (1k) | 2.50 | 0.00 | 3.42 | 180 | 0.5 x 0.4 x 0.4 | floor |   |

## 4. Gameplay anchors (empty GameObjects the builder must create)
| Kind | Room | x | y | z | What |
|---|---|---|---|---|---|
| item-spot | kitE | 6.27 | 1.40 | -1.50 | east range mantel |
| note-spot | kitE | 3.90 | 0.76 | -2.00 | Ida's note: 'Ada moved the clock again' (note 1/3) |
| hiding | bedW | -5.57 | 0.00 | 2.20 | under the Ada's bed -> HidingSpot on H06.p025 (Bed) - crouched |
| hiding | bedE | 4.60 | 0.00 | 0.39 | Ida's wardrobe -> HidingSpot on H06.p032 (Wardrobe) |
| note-spot | nurW | -5.50 | 1.00 | -0.33 | midwife's letter, 1951 (note 2/3) |
| hiding | bath | -0.60 | 0.00 | 3.30 | behind the bath curtain -> HidingSpot on H06.p037 (Bathtub) |
| puzzle | attic | 6.37 | 0.00 | 0.00 | Spot the differences: 3-5 objects differ between the halves (picked per match from DIFFERENCE candidates). Each difference shows a symbol when found; press the symbols on the box in the order the cot-room rhyme gives. SequencePuzzle (6 symbols, 4 in the answer). |
| container | attic | 6.37 | 0.00 | 0.00 | Silver locket |
| hiding | attic | -4.00 | 0.00 | 3.17 | behind the trunks (attic, west end) -> HidingSpot on H06.p043 (CellarCrates) - crouched |
| note-spot | attic | -3.10 | 0.72 | -3.37 | the cot-room rhyme (note 3/3) |
| item-spot | attic | 2.50 | 0.40 | 3.42 | box of matching dresses |
| item-spot | kitW | -4.00 | 0.76 | -2.00 | west kitchen table (the 'wrong' one) |
| shrine | landing | 1.00 | 0.00 | -3.52 | Rite of Return point (active only if this house is a shrine this match) |

Mapping to existing code: `item-spot` -> `House.ItemSpots` (order as listed), `note-spot` -> the 3 `Readable` notes, `puzzle` -> `PuzzleRig` anchor, `container` -> `PartContainer`, `shrine` -> `House.ShrineMarker` (+ shrine niche inside), `hiding` -> `HidingSpot`.

## 5. Puzzle - Silver locket
1. LOCK FIRST: in the long attic the double music box sits on a trunk with 6 brass symbol tabs on its lid.
2. Per match pick 3-5 DIFFERENCE candidates (marked 'DIFFERENCE candidate' in props + a pool of 12): clock hands, a missing photo figure, the phone, a cot blanket colour, a drawer left open, a mirror that doesn't reflect, etc.
3. When a player looks at a difference for 2 s it shows a faint symbol (one of the 6).
4. The cot-room rhyme (note 3/3) gives the order; press the tabs on the box (SequencePuzzle, 4 steps).
5. Wrong: both halves' lights flicker and every clock in the house strikes 2.

## 6. Night changes (DESIGN 'Night changes' - pick 2-3 per match)
- the west kitchen hearth (decoy) is LIT and the east (real) one is cold for 30 s
- the twins' cots swap sides
- the screwed-shut door 7A is open a crack
- plus the shared pool: extra person in the family photo, a door opening onto a brick wall, moved furniture, an extra hallway door, all clocks stopped at the same time, writing on a wall in a player's name

## 7. Sound
- musicbox: the double music box plays two lullabies at once, slightly out of tune
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
