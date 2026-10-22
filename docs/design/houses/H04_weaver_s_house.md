# H04 - Weaver's House (Ashdown)

> Machine-readable twin: `docs/design/houses/H04.json` - the builder reads THAT file; this page explains it. Every number here comes from the same data and is validated (no overlaps, door clearances, required items).

| | |
|---|---|
| **Archetype** | Weaver's cottage: long stone house whose whole top floor is a loom shop lit by a continuous band of windows |
| **Silhouette (must read at 50 m in fog)** | Long and low (14 m) with a RIBBON of 8 small windows across the upper floor front, stone-slate roof, one chimney at the WEST end, an outside stone stair to a loading door at the east gable. |
| **Ritual part** | Spool of red thread |
| **M1 puzzle rig (code)** | `PuzzleKind.Sequence` |
| **Letter traits (HouseCatalog)** | door **Red**, **1** chimney(s), stopped clock **11:00**, family **Ashdown** |
| **World position** | x -53.8, z -32.2, floor level y -0.20, yaw 135 deg - Mill Lane (north-west side) |
| **Front door (world)** | (-51.2, -34.1) - 154.8 m from the altar, 5.0 m from the lane |
| **Placement note** | long axis runs along the lane; drying green behind |

## Reference images

![plan](../ref/H04_plan.png)

![front-left](../ref/H04_massing_front.png) ![back-right](../ref/H04_massing_back.png)

Real-world references (look, don't copy): [Weavers%27 cottage](https://en.wikipedia.org/wiki/Weavers%27_cottage)

Mood-image prompt (Higgsfield / Gemini / ChatGPT image - save the result as `docs/design/ref/H04_mood.png`):

```text
long low stone weaver's cottage with a continuous band of small mullioned windows across the whole upper floor, heavy stone-slate roof, an outside stone stair to a loading door on the gable, a red front door, red dyed wool hanging on washing lines behind, photorealistic, late 1970s rural English village, overcast autumn dusk, damp, muted greens and greys, 35mm film still, Phasmophobia / Resident Evil Village mood, no people, eye-level, 16:9
```

## 1. Structure

### Levels
| Level | Floor y (m) | Clear height (m) |
|---|---|---|
| G - Ground floor | +0.00 | 2.70 |
| F - First floor | +2.95 | 2.60 |
| A - Attic (under the roof) | +5.80 | 2.30 |

Floor build-up: 0.25 m between floors (joists + boards); ceilings use the room's `ceiling` material.

### Wings (massing)
| Wing | Rect x0,z0 -> x1,z1 | Size | Levels | Walls | Wall thickness | Roof |
|---|---|---|---|---|---|---|
| main | (-7.0, -3.2) -> (7.0, 3.2) | 14.0 x 6.4 | G, F, A | PlasterStone (half-rendered rubble, render falling off), SandstoneBlocks quoins + long stone mullions in the upper window band | 0.5 | Gable roof 35 deg, ridge along x at 7.9 m, RoofRedSlate (heavy stone slates, graded courses), moss |
| outshut | (-7.0, -5.6) -> (-1.0, -3.2) | 6.0 x 2.4 | G | Rubble | 0.45 | Catslide continuation of the main roof down to 2.2 m |

### Chimneys (count must equal HouseCatalog = 1)
| x | z | stack w x d | above ridge | pots | serves |
|---|---|---|---|---|---|
| -7.0 | 0.0 | 1.2 x 1.0 | 1.0 | 1 | living fire = THE HEARTH |

### Exterior details
- upper-floor WEAVER'S WINDOW BAND: 7 lights with stone mullions running almost the full front
- outside stone stair (8 steps, no rail) up the east gable to the loading door
- red door in the middle of the front (cross-passage plan)
- heavy stone-slate roof with graded courses, moss, a sagging ridge
- red dye stains running down the wall under the parlour window

### Plot, garden and yard
- Front: no garden; a 1 m flagged strip
- Back: DRYING GREEN 10 x 8 m with 3 washing lines of RED skeins; 2 dye barrels; a well-house cover; a broken spinning wheel
- Plot boundary (world polygon): (-58.5,-14.8), (-71.2,-27.5), (-57.2,-41.5), (-44.5,-28.8). Boundary = DryStoneWall / FencePicket on the street side, FenceRail or hedges (Shrub1-4) elsewhere, a GardenGate on every path.

## 2. Rooms

| Room | Level | Rect (x0,z0,x1,z1) | Clear inside | Kind | Floor | Walls | Ceiling | Light |
|---|---|---|---|---|---|---|---|---|
| **living** House-place (living) | G | (-7.0, -3.2, -1.5, 3.2) | 4.94 x 5.84 | living | FloorFlagstone | Whitewash | CeilingInterior | pendant + oil lamp |
| **hall** Cross passage | G | (-1.5, -3.2, 0.5, 3.2) | 1.88 x 5.40 | hall | FloorFlagstone | Whitewash | CeilingInterior | pendant |
| **parlour** Parlour / dye room | G | (0.5, -3.2, 7.0, 3.2) | 5.94 x 5.40 | utility | FloorBrick | PlasterWorn with dye splashes | CeilingInterior | pendant |
| **kitchen** Back kitchen (outshut) | G | (-7.0, -5.6, -3.6, -3.2) | 2.89 x 1.89 | kitchen | FloorQuarryTile | Whitewash | CeilingInterior | pendant |
| **bath** Bathroom (outshut) | G | (-3.6, -5.6, -1.0, -3.2) | 2.09 x 1.89 | bathroom | FloorBathTile | TileWallWhite | CeilingInterior | pendant |
| **garret** Garret (attic over the bedrooms) | A | (-7.0, -3.2, -1.5, 3.2) | 4.50 x 5.40 | attic | FloorOldWood2 | RawPlankWall | CeilingInterior | pendant - reached by a ladder from bed1; 1.9 m headroom at the ridge only |
| **loomshop** LOOM SHOP (PUZZLE) | F | (-1.5, -3.2, 7.0, 3.2) | 7.94 x 5.40 | workshop | FloorOldWood2 | Whitewash, warp threads strung on pegs | CeilingInterior | daylight band; 2 CagedLight at night |
| **bed1** Bedroom (Ashdowns) | F | (-7.0, 0.0, -1.5, 3.2) | 4.94 x 2.64 | bedroom | FloorOldWood1 | Wallpaper1 | CeilingInterior | pendant |
| **bed2** Bedroom (apprentice) | F | (-7.0, -3.2, -1.5, 0.0) | 4.94 x 2.64 | bedroom | FloorOldWood1 | PlasterWorn | CeilingInterior | pendant |

Skirting: 0.15 m dark oak in every room except kitchens/sculleries/cellars (none) and bathrooms (tile). Every ground-floor room has one LightSwitch by its main door at 1.3 m; every room has the existing `Room` component.

### Doors (how the rooms connect)
| Door | Level | From -> to | Wall | Centre at | W x H | Leaf | Role |
|---|---|---|---|---|---|---|---|
| d_front | G | hall -> ext | N | -0.5 | 0.95 x 2.05 | boarded red door, cross-passage (Red) | front |
| d_back | G | hall -> ext | S | -1.0 | 0.9 x 2.05 | cross-passage back door (nearly aligned with the front) | back |
| d_liv | G | hall -> living | W | 1.4 | 0.85 x 2.05 | panel |  |
| d_par | G | hall -> parlour | E | 1.4 | 0.85 x 2.05 | panel |  |
| d_kit | G | living -> kitchen | S | -5.0 | 0.8 x 2.05 | panel |  |
| d_bath | G | kitchen -> bath | E | -3.9 | 0.75 x 2.05 | panel |  |
| d_loading | F | loomshop -> ext | E | 1.0 | 1.1 x 2.05 | loading door 1.1 x 1.8 on the gable, reached by the outside stone stair | loading |
| d_b1 | F | loomshop -> bed1 | W | 1.6 | 0.8 x 2.05 | panel |  |
| d_b2 | F | loomshop -> bed2 | W | -0.6 | 0.8 x 2.05 | panel |  |

### Windows
| Window | Room | Wall | Centre at | W x H | Sill | Style | Weak | Curtains / shutters |
|---|---|---|---|---|---|---|---|---|
| w_band0 | loomshop | N | -0.6 | 0.8 x 0.9 | 1.0 | weaver's light (one of the window band), stone mullions 0.15 |  |   |
| w_band1 | loomshop | N | 0.6 | 0.8 x 0.9 | 1.0 | weaver's light (one of the window band), stone mullions 0.15 |  |   |
| w_band2 | loomshop | N | 1.8 | 0.8 x 0.9 | 1.0 | weaver's light (one of the window band), stone mullions 0.15 |  |   |
| w_band3 | loomshop | N | 3.0 | 0.8 x 0.9 | 1.0 | weaver's light (one of the window band), stone mullions 0.15 |  |   |
| w_band4 | loomshop | N | 4.2 | 0.8 x 0.9 | 1.0 | weaver's light (one of the window band), stone mullions 0.15 |  |   |
| w_band5 | loomshop | N | 5.4 | 0.8 x 0.9 | 1.0 | weaver's light (one of the window band), stone mullions 0.15 |  |   |
| w_band_b1 | bed1 | N | -4.2 | 0.8 x 0.9 | 1.0 | window band continues |  |   |
| w_liv | living | N | -4.2 | 1.1 x 1.1 | 0.9 | casement, sagging | **WEAK** (break-in point) |   |
| w_par | parlour | N | 3.8 | 1.1 x 1.1 | 0.9 | casement | **WEAK** (break-in point) |   |
| w_par2 | parlour | S | 3.8 | 1.0 x 1.0 | 0.9 | casement |  |   |
| w_k | kitchen | W | -4.4 | 0.8 x 0.8 | 0.9 | small |  |   |
| w_garret | garret | W | 0.0 | 0.5 x 0.5 | 0.9 | tiny gable light |  |   |

### Stairs, ladders and voids
| Id | In room | Rect | Goes | Up toward | Type |
|---|---|---|---|---|---|
| s1 | hall | (-0.45, -2.8, 0.45, 0.6) | G -> F | S | straight stone-and-timber stair in the passage |
| s1u | loomshop | (-0.45, -2.8, 0.45, 0.6) | G -> F | S | void |
| s2 | bed1 | (-2.4, 0.1, -1.7, 1.0) | F -> A | W | ladder to the garret hatch |

## 3. Every prop, room by room

Position = prop **pivot at the centre of its footprint, on the floor** (or on top of its parent for `on`, or the centre height `y` for wall-hung). Yaw = the way the prop's FRONT faces. Scale the asset so its bounds fit the listed size (see ASSETS.md 'Scaling rule').

### living - House-place (living)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 1 | `Hearth` | BUILD (PropKit) | -6.12 | 0.00 | 0.00 | 90 | 1.8 x 0.7 x 1.4 | floor | hearth open fire with a crane and kettle, stone surround |
| 2 | `MantelClock` | Poly Haven `mantel_clock_01` (1k) | -6.12 | 1.40 | 0.00 | 90 | 0.3 x 0.14 x 0.32 | on H04.p001 |  stopped at 11:00 |
| 3 | `FrameStanding1` | Poly Haven `standing_picture_frame_01` (1k) | -6.12 | 1.40 | -0.60 | 90 | 0.18 x 0.08 x 0.24 | on H04.p001 | family-photo  |
| 4 | `RockingChair` | Poly Haven `Rockingchair_01` (2k) | -5.00 | 0.00 | 1.20 | 240 | 0.6 x 0.95 x 1.05 | floor |   |
| 5 | `SofaPainted` | Poly Haven `painted_wooden_sofa` (2k) | -2.60 | 0.00 | 2.32 | 180 | 1.6 x 0.7 x 0.9 | floor |   |
| 6 | `TableGallinera` | Poly Haven `gallinera_table` (1k) | -3.60 | 0.00 | -1.00 | 0 | 1.5 x 0.8 x 0.76 | floor |   |
| 7 | `OilLamp` | Poly Haven `vintage_oil_lamp` (1k) | -3.60 | 0.76 | -1.00 | 0 | 0.18 x 0.18 x 0.45 | on H04.p006 |   |
| 8 | `ChairGallinera` | Poly Haven `gallinera_chair` (1k) | -3.60 | 0.00 | -1.75 | 0 | 0.45 x 0.5 x 0.9 | floor |   |
| 9 | `SideTable` | Poly Haven `side_table_01` (1k) | -2.40 | 0.00 | -2.86 | 0 | 0.5 x 0.5 x 0.6 | floor |   |
| 10 | `RotaryPhone` | Sketchfab `812e8f59f9e749fab483eabf4800dec2` (CC BY, sara El-sisi) | -2.40 | 0.60 | -2.86 | 0 | 0.22 x 0.25 x 0.14 | on H04.p009 | phone  |
| 11 | `Radio` | Poly Haven `vintage_radio_transceiver` (1k) | -2.20 | 0.60 | -2.86 | 0 | 0.45 x 0.25 x 0.3 | on H04.p009 | radio  |
| 12 | `Dresser` | BUILD (PropKit) | -3.60 | 0.00 | -2.86 | 0 | 1.4 x 0.5 x 2.0 | floor |   |
| 13 | `SpinningWheel` | Poly Haven `spinning_wheel_01` (2k) | -6.00 | 0.00 | -2.10 | 45 | 0.9 x 0.5 x 0.95 | floor |   |

### hall - Cross passage
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 14 | `Mirror` | Poly Haven `ornate_mirror_01` (1k) | 0.41 | 1.15 | 2.60 | 270 | 0.6 x 0.05 x 0.9 | wall-hung |  small mirror |
| 15 | `FuseBox` | Poly Haven `power_box_01` (1k) | -1.36 | 1.40 | -2.60 | 90 | 0.45 x 0.15 x 0.6 | wall-hung | fuse-box  |

### parlour - Parlour / dye room
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 16 | `Barrel2` | Poly Haven `Barrel_02` (1k) | 1.60 | 0.00 | -2.20 | 0 | 0.6 x 0.6 x 0.9 | floor |  dye vat (red stain) |
| 17 | `Barrel2` | Poly Haven `Barrel_02` (1k) | 3.00 | 0.00 | -2.20 | 0 | 0.6 x 0.6 x 0.9 | floor |  dye vat (red stain) |
| 18 | `SteelShelves` | Poly Haven `steel_frame_shelves_01` (1k) | 6.25 | 0.00 | 0.00 | 270 | 1.2 x 0.45 x 1.9 | floor |  dye jars, mordant tins |
| 19 | `Wicker2` | Poly Haven `wicker_basket_02` (1k) | 1.40 | 0.00 | 2.47 | 180 | 0.4 x 0.4 x 0.4 | floor |   |
| 20 | `Wicker1` | Poly Haven `wicker_basket_01` (1k) | 5.60 | 0.00 | 2.50 | 180 | 0.45 x 0.35 x 0.3 | floor |   |
| 21 | `WashingLine` | BUILD (PropKit) | 3.80 | 0.60 | 0.40 | 0 | 6.0 x 0.3 x 2.0 | raised | allow-overlap indoor drying lines with red skeins at 2.0 m |
| 22 | `BarrelWorn` | owned: His_Med_Storage_Barrel_Wood_Worn_01 (Fab) | 5.90 | 0.00 | -2.35 | 0 | 0.65 x 0.65 x 0.95 | floor |   |

### kitchen - Back kitchen (outshut)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 23 | `Cooker` | Poly Haven `electric_stove` (2k) | -6.10 | 0.00 | -4.82 | 0 | 0.6 x 0.6 x 0.9 | floor |   |
| 24 | `KitchenSinkUnit` | BUILD (PropKit) | -4.80 | 0.00 | -4.82 | 0 | 1.0 x 0.6 x 0.9 | floor |   |
| 25 | `Jug` | owned: Res_Sto_Flask_Ceramic_Worn_01 (Fab) | -4.50 | 0.90 | -4.82 | 0 | 0.18 x 0.18 x 0.3 | on H04.p024 |   |

### bath - Bathroom (outshut)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 26 | `Bathtub` | Sketchfab `d06c028a89a54c469e4af9cac50606fa` (CC BY, Lady Lion Studios) | -2.50 | 0.00 | -4.80 | 0 | 1.4 x 0.64 x 0.53 | floor |   |
| 27 | `Toilet` | Sketchfab `6ac515a1c4154db18b5b4bd0b46d6405` (CC BY, Allan-Jay Branscombe) | -1.80 | 0.00 | -4.20 | 270 | 0.4 x 0.65 x 0.8 | floor |   |
| 28 | `Mirror` | Poly Haven `ornate_mirror_01` (1k) | -1.90 | 1.10 | -3.29 | 180 | 0.6 x 0.05 x 0.9 | wall-hung | mirror  |
| 29 | `Sink` | Sketchfab `a278d63585494921906ab3500c421ff7` (CC BY, lagesnpiet) | -1.90 | 0.00 | -3.52 | 180 | 0.55 x 0.45 x 0.85 | floor |   |

### garret - Garret (attic over the bedrooms)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 46 | `CellarCrates` | BUILD (PropKit) | -6.02 | 0.00 | 0.00 | 90 | 1.4 x 0.9 x 1.2 | floor |   |
| 47 | `Suitcase` | Poly Haven `vintage_suitcase` (1k) | -4.00 | 0.00 | 2.57 | 180 | 0.65 x 0.2 x 0.45 | floor |   |

### loomshop - LOOM SHOP (PUZZLE)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 30 | `LoomDobby` | Sketchfab `07ba2b15ffb7425cab23f700828fd7f3` (CC BY, Spiacente) | 3.20 | 0.00 | 0.00 | 180 | 1.6 x 1.5 x 1.6 | floor | puzzle THE LOOM, centre of the room: rethread it to match the hidden pattern (SequencePuzzle 6 symbols = 6 thread colours) |
| 31 | `LoomFrame` | BUILD (PropKit) | 0.80 | 0.00 | 1.60 | 180 | 1.6 x 1.5 x 1.6 | floor |  second, broken loom (dressing) |
| 32 | `ChestWorn` | owned: Res_Sto_Chest_Wood_Worn_08 (Fab Saloon Interior) | 6.22 | 0.00 | -1.80 | 270 | 0.9 x 0.5 x 0.55 | floor |  PART CONTAINER: thread chest |
| 33 | `ShelvesPainted` | Poly Haven `painted_wooden_shelves` (1k) | 5.50 | 0.00 | -2.52 | 0 | 0.9 x 0.3 x 1.6 | floor |  bobbins in pigeon rows |
| 34 | `Wicker1` | Poly Haven `wicker_basket_01` (1k) | 2.00 | 0.00 | -2.50 | 0 | 0.45 x 0.35 x 0.3 | floor |   |
| 35 | `TableSmall` | Poly Haven `small_wooden_table_01` (1k) | 4.20 | 0.00 | -2.42 | 0 | 0.7 x 0.5 x 0.72 | floor |   |
| 36 | `Sacks` | BUILD (PropKit) | 1.00 | 0.00 | -2.37 | 0 | 1.0 x 0.6 x 0.7 | floor |   |
| 37 | `ShrineNiche` | BUILD (PropKit) | 6.30 | 0.90 | -2.58 | 0 | 0.6 x 0.25 x 0.9 | wall-hung | shrine  |
| 45 | `CardboardBox` | Poly Haven `cardboard_box_01` (1k) | 3.20 | 0.00 | -2.47 | 0 | 0.5 x 0.4 x 0.4 | floor |   |

### bed1 - Bedroom (Ashdowns)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 38 | `Bed` | Poly Haven `GothicBed_01` (2k) | -5.40 | 0.00 | 1.14 | 0 | 1.6 x 2.1 x 1.4 | floor |   |
| 39 | `Nightstand` | Poly Haven `ClassicNightstand_01` (1k) | -4.30 | 0.00 | 0.29 | 0 | 0.5 x 0.4 x 0.6 | floor |   |
| 40 | `ChestOfDrawers` | Poly Haven `vintage_wooden_drawer_01` (1k) | -3.00 | 0.00 | 2.43 | 180 | 0.9 x 0.48 x 1.0 | floor |   |
| 41 | `FrameHanging2` | Poly Haven `hanging_picture_frame_02` (1k) | -2.60 | 1.40 | 2.69 | 180 | 0.5 x 0.03 x 0.4 | wall-hung | family-photo embroidered sampler: pattern piece 1 |

### bed2 - Bedroom (apprentice)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 42 | `Wardrobe` | Poly Haven `GothicCabinet_01` (2k) | -5.60 | 0.00 | -2.37 | 0 | 1.2 x 0.6 x 2.1 | floor |   |
| 43 | `BedIron` | Poly Haven `old_bed_frame` (2k) | -5.47 | 0.00 | -1.20 | 90 | 1.0 x 2.0 x 1.05 | floor |   |
| 44 | `TableSmall` | Poly Haven `small_wooden_table_01` (1k) | -1.84 | 0.00 | -2.20 | 270 | 0.7 x 0.5 x 0.72 | floor |   |

## 4. Gameplay anchors (empty GameObjects the builder must create)
| Kind | Room | x | y | z | What |
|---|---|---|---|---|---|
| item-spot | living | -6.12 | 1.40 | 0.00 | mantel |
| note-spot | living | -3.70 | 0.76 | -1.00 | pattern draft (note 1/3) |
| item-spot | living | -3.60 | 2.00 | -2.86 | dresser |
| hiding | parlour | 5.90 | 0.00 | -2.35 | inside the empty dye barrel -> HidingSpot on H04.p022 (BarrelWorn) - crouched |
| puzzle | loomshop | 3.20 | 0.00 | 0.00 | Rethread the loom: pattern pieces hidden in 3 places (bed1 sampler, parlour dye book, living mantel photo) |
| container | loomshop | 6.22 | 0.00 | -1.80 | Spool of red thread |
| item-spot | loomshop | 4.20 | 0.72 | -2.42 | winding table |
| hiding | loomshop | 1.00 | 0.00 | -2.37 | behind the wool sacks -> HidingSpot on H04.p036 (Sacks) - crouched |
| shrine | loomshop | 6.30 | 0.00 | -2.58 | Rite of Return point (active only if this house is a shrine this match) |
| hiding | bed1 | -5.40 | 0.00 | 1.14 | under the bed -> HidingSpot on H04.p038 (Bed) - crouched |
| hiding | bed2 | -5.60 | 0.00 | -2.37 | wardrobe -> HidingSpot on H04.p042 (Wardrobe) |
| note-spot | bed2 | -1.84 | 0.72 | -2.30 | apprentice's notebook (note 2/3) |
| note-spot | loomshop | 3.10 | 0.40 | -2.47 | order book (note 3/3) |
| hiding | garret | -6.02 | 0.00 | 0.00 | behind the trunks in the garret -> HidingSpot on H04.p046 (CellarCrates) - crouched |

Mapping to existing code: `item-spot` -> `House.ItemSpots` (order as listed), `note-spot` -> the 3 `Readable` notes, `puzzle` -> `PuzzleRig` anchor, `container` -> `PartContainer`, `shrine` -> `House.ShrineMarker` (+ shrine niche inside), `hiding` -> `HidingSpot`.

## 5. Puzzle - Spool of red thread
1. LOCK FIRST: the loom, centre of the loom shop, half-threaded; the thread chest beside it is locked to the loom's beam.
2. Pattern pieces (3): the embroidered sampler (bed1), the dye book (parlour), the mantel photo of the finished cloth.
3. Rethread: 6 thread colours (= the 6 SequenceSymbols) - press them on the loom heddles in the pattern order (4 of 6).
4. Wrong: the shuttle flies off the loom (thud + noise) and the threads reset.
5. Part: Spool of red thread.

## 6. Night changes (DESIGN 'Night changes' - pick 2-3 per match)
- the loom is weaving by itself, slowly, and stops when you enter
- the red skeins on the drying lines are now hanging inside the parlour
- a pattern on the floor in red thread spells a house name
- plus the shared pool: extra person in the family photo, a door opening onto a brick wall, moved furniture, an extra hallway door, all clocks stopped at the same time, writing on a wall in a player's name

## 7. Sound
- washing: thread/basket foley
- the loom: wooden clacks (Antique Small Metals + wood knocks)
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
