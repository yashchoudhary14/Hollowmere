# H13 - Farmer's House (Barrow)

> Machine-readable twin: `docs/design/houses/H13.json` - the builder reads THAT file; this page explains it. Every number here comes from the same data and is validated (no overlaps, door clearances, required items).

| | |
|---|---|
| **Archetype** | Stone longhouse farmhouse with the barn under the same long roof and a dairy outshut |
| **Silhouette (must read at 50 m in fog)** | Very long (18 m) and low: farmhouse at the west end with a green door straight into the kitchen, a huge barn at the east end with tall cart doors and a hayloft door, one continuous moss-covered roof, THREE chimneys (west gable, mid, dairy), a muck heap and a rusty tractor in the yard. |
| **Ritual part** | Straw heart |
| **M1 puzzle rig (code)** | `PuzzleKind.Sequence` |
| **Letter traits (HouseCatalog)** | door **Green**, **3** chimney(s), stopped clock **6:00**, family **Barrow** |
| **World position** | x 86.0, z -34.0, floor level y +0.20, yaw 0 deg - East Road (south side) |
| **Front door (world)** | (82.5, -30.0) - 164.2 m from the altar, 7.9 m from the lane |
| **Placement note** | farmyard in front of the barn; fields to the south and east |

## Reference images

![plan](../ref/H13_plan.png)

![front-left](../ref/H13_massing_front.png) ![back-right](../ref/H13_massing_back.png)

Real-world references (look, don't copy): [Devon longhouse](https://en.wikipedia.org/wiki/Devon_longhouse), [Scarecrow](https://en.wikipedia.org/wiki/Scarecrow)

Mood-image prompt (Higgsfield / Gemini / ChatGPT image - save the result as `docs/design/ref/H13_mood.png`):

```text
long low stone farmhouse longhouse with an attached barn under one continuous mossy roof, a green stable door, tall barred cart doors, a hayloft door with a hoist beam, a muddy farmyard with a tarp-covered tractor, fields with scarecrows beyond, photorealistic, late 1970s rural English village, overcast autumn dusk, damp, muted greens and greys, 35mm film still, Phasmophobia / Resident Evil Village mood, no people, eye-level, 16:9
```

## 1. Structure

### Levels
| Level | Floor y (m) | Clear height (m) |
|---|---|---|
| G - Ground floor | +0.00 | 2.70 |
| F - First floor | +2.95 | 2.40 |

Floor build-up: 0.25 m between floors (joists + boards); ceilings use the room's `ceiling` material.

### Wings (massing)
| Wing | Rect x0,z0 -> x1,z1 | Size | Levels | Walls | Wall thickness | Roof |
|---|---|---|---|---|---|---|
| house | (-6.0, -4.0) -> (2.0, 4.0) | 8.0 x 8.0 | G, F | Rubble stone (Stone), whitewash patches, SandstoneBlocks lintels | 0.55 | Continuous gable roof 40 deg over house AND barn, ridge along x at 7.2 m, RoofRedSlate (house) / CorrugatedRust (barn end) |
| barn | (2.0, -4.0) -> (12.0, 4.0) | 10.0 x 8.0 | G, F | Rubble stone + RawPlankWall gable cladding, ventilation slits | 0.45 | Same roof; cart doors 3.0 x 3.2 north, hayloft door above |
| dairy | (-6.0, -7.0) -> (-1.0, -4.0) | 5.0 x 3.0 | G | Rubble, whitewashed | 0.35 | Catslide continuation of the main roof |

### Chimneys (count must equal HouseCatalog = 3)
| x | z | stack w x d | above ridge | pots | serves |
|---|---|---|---|---|---|
| -6.0 | 0.0 | 1.4 x 1.2 | 1.0 | 2 | kitchen range = THE HEARTH (inglenook) |
| -1.5 | 2.0 | 0.8 x 0.8 | 1.0 | 1 | parlour fire (on the kitchen partition) |
| -3.5 | -7.0 | 0.8 x 0.8 | 1.0 | 1 | dairy copper |

### Exterior details
- green STABLE door (top half open by day) straight into the kitchen
- barn cart doors 3.0 x 3.2, barred
- hayloft pitching door with a hoist beam
- rusty tractor in the yard (CoveredCar as a tarp-covered stand-in)
- muck heap; milk churn stand by the lane

### Plot, garden and yard
- Farmyard 20 x 14 m north of the barn: mud (GroundMud), puddles, a water trough, a muck heap, the covered tractor
- FIELDS beyond (village 'Fields and scarecrows' zone): 6 SCARECROWS used by the puzzle stand there
- Plot boundary (world polygon): (76.0,-49.0), (102.0,-49.0), (102.0,-24.0), (76.0,-24.0). Boundary = DryStoneWall / FencePicket on the street side, FenceRail or hedges (Shrub1-4) elsewhere, a GardenGate on every path.

## 2. Rooms

| Room | Level | Rect (x0,z0,x1,z1) | Clear inside | Kind | Floor | Walls | Ceiling | Light |
|---|---|---|---|---|---|---|---|---|
| **kitchen** Farm kitchen | G | (-6.0, -4.0, -1.5, 4.0) | 3.89 x 7.39 | kitchen | FloorFlagstone | Whitewash, hams on hooks | CeilingInterior | pendant + oil lamp |
| **parlour** Front parlour | G | (-1.5, 0.0, 2.0, 4.0) | 3.38 x 3.39 | living | Carpet70sB | Wallpaper2C | CeilingInterior | pendant |
| **hall** Back hall & stair | G | (-1.5, -4.0, 2.0, 0.0) | 3.38 x 3.39 | hall | FloorFlagstone | Whitewash | CeilingInterior | pendant |
| **dairy** Dairy | G | (-6.0, -7.0, -1.0, -4.0) | 4.30 x 2.59 | utility | FloorBrick | TileWallWhite (cracked), slate shelves | CeilingInterior | pendant |
| **barn** Barn (PUZZLE) | G | (2.0, -4.0, 12.0, 4.0) | 9.49 x 7.10 | barn | FloorDirt | rubble + RawPlankWall | CeilingInterior | 2 CagedLight on chains |
| **bed1** Barrows' bedroom | F | (-6.0, 0.0, -1.5, 4.0) | 3.89 x 3.39 | bedroom | FloorOldWood1 | Wallpaper1 | CeilingInterior | pendant |
| **bed2** Lads' bedroom | F | (-6.0, -4.0, -3.0, 0.0) | 2.39 x 3.39 | bedroom | FloorOldWood2 | PlasterWorn | CeilingInterior | pendant |
| **landing** Landing | F | (-3.0, -4.0, 2.0, 0.0) | 4.88 x 3.39 | landing | FloorOldWood1 | PlasterWorn | CeilingInterior | pendant |
| **bath** Bathroom | F | (-1.5, 0.0, 2.0, 4.0) | 3.38 x 3.39 | bathroom | FloorBathTile | TileWallWhite | CeilingInterior | pendant |
| **hayloft** Hayloft (attic of the barn) | F | (2.0, -4.0, 12.0, 4.0) | 9.49 x 7.10 | attic | FloorOldWood2 | RawPlankWall, hay | CeilingInterior | pendant |

Skirting: 0.15 m dark oak in every room except kitchens/sculleries/cellars (none) and bathrooms (tile). Every ground-floor room has one LightSwitch by its main door at 1.3 m; every room has the existing `Room` component.

### Doors (how the rooms connect)
| Door | Level | From -> to | Wall | Centre at | W x H | Leaf | Role |
|---|---|---|---|---|---|---|---|
| d_front | G | kitchen -> ext | N | -3.5 | 0.95 x 2.05 | green stable door (top half open by day) (Green) | front |
| d_back | G | hall -> ext | S | 0.5 | 0.9 x 2.05 | panel - to the yard | back |
| d_dairy | G | kitchen -> dairy | S | -3.5 | 0.85 x 2.05 | panel |  |
| d_hall | G | kitchen -> hall | E | -2.0 | 0.85 x 2.05 | panel |  |
| d_par | G | hall -> parlour | N | -0.5 | 0.85 x 2.05 | panel |  |
| d_barn | G | parlour -> barn | E | 2.0 | 0.9 x 2.05 | plank door into the barn (two steps down) |  |
| d_cart | G | barn -> ext | N | 7.0 | 3.0 x 3.2 | tall cart doors (barred on the inside) | cart |
| d_b1 | F | landing -> bed1 | N | -2.25 | 0.8 x 2.05 | panel |  |
| d_b2 | F | landing -> bed2 | W | -2.0 | 0.8 x 2.05 | panel |  |
| d_bath | F | landing -> bath | N | 0.5 | 0.75 x 2.05 | panel |  |
| d_hay | F | hayloft -> ext | N | 9.5 | 1.4 x 1.6 | hayloft pitching door with a hay hoist beam | pitching |

### Windows
| Window | Room | Wall | Centre at | W x H | Sill | Style | Weak | Curtains / shutters |
|---|---|---|---|---|---|---|---|---|
| w_k1 | kitchen | N | -5.0 | 0.9 x 1.0 | 0.9 | deep-set sash |  |   |
| w_k2 | kitchen | W | -2.0 | 0.9 x 1.0 | 0.9 | deep-set sash, rotten | **WEAK** (break-in point) |   |
| w_par | parlour | N | 0.5 | 1.1 x 1.1 | 0.9 | sash |  |   |
| w_dairy | dairy | S | -3.5 | 0.8 x 0.5 | 1.5 | slatted dairy vent (wooden louvres, loose) | **WEAK** (break-in point) |   |
| w_barn | barn | S | 7.0 | 0.3 x 1.2 | 1.0 | ventilation slit (x3 along the south wall) |  |   |
| w_b1 | bed1 | N | -3.8 | 0.9 x 1.0 | 0.9 | sash |  |   |
| w_b2 | bed2 | W | -2.0 | 0.8 x 0.9 | 0.9 | small sash |  |   |
| w_bath | bath | N | 0.8 | 0.6 x 0.7 | 1.3 | frosted |  |   |

### Stairs, ladders and voids
| Id | In room | Rect | Goes | Up toward | Type |
|---|---|---|---|---|---|
| s1 | hall | (1.0, -3.4, 1.9, -0.1) | G -> F | N | straight - boxed stair, 12 risers |
| s1u | landing | (1.0, -3.4, 1.9, -0.1) | G -> F | N | void |
| s2 | barn | (11.0, -3.5, 11.6, -2.3) | G -> F | S | fixed ladder to the hayloft |
| s2u | hayloft | (11.0, -3.5, 11.6, -2.3) | G -> F | S | hatch |

## 3. Every prop, room by room

Position = prop **pivot at the centre of its footprint, on the floor** (or on top of its parent for `on`, or the centre height `y` for wall-hung). Yaw = the way the prop's FRONT faces. Scale the asset so its bounds fit the listed size (see ASSETS.md 'Scaling rule').

### kitchen - Farm kitchen
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 1 | `Hearth` | BUILD (PropKit) | -5.07 | 0.00 | 0.00 | 90 | 1.8 x 0.7 x 1.4 | floor | hearth big inglenook range, bacon rack over, settle beside |
| 2 | `MantelClock` | Poly Haven `mantel_clock_01` (1k) | -5.07 | 1.40 | 0.00 | 90 | 0.3 x 0.14 x 0.32 | on H13.p001 |  stopped at 6:00 |
| 3 | `FrameStanding1` | Poly Haven `standing_picture_frame_01` (1k) | -5.07 | 1.40 | -0.60 | 90 | 0.18 x 0.08 x 0.24 | on H13.p001 | family-photo the Barrows at harvest supper |
| 4 | `TableWood3` | Poly Haven `WoodenTable_03` (1k) | -3.60 | 0.00 | 0.00 | 90 | 2.0 x 0.9 x 0.78 | floor |   |
| 5 | `BowlWood2` | Poly Haven `wooden_bowl_02` (1k) | -3.60 | 0.78 | 0.00 | 90 | 0.3 x 0.3 x 0.12 | on H13.p004 |   |
| 6 | `OilLamp` | Poly Haven `vintage_oil_lamp` (1k) | -3.60 | 0.78 | -0.30 | 90 | 0.18 x 0.18 x 0.45 | on H13.p004 |   |
| 7 | `BenchPainted` | Poly Haven `painted_wooden_bench` (1k) | -4.25 | 0.00 | 0.00 | 90 | 1.4 x 0.4 x 0.45 | floor |   |
| 8 | `BenchPainted` | Poly Haven `painted_wooden_bench` (1k) | -2.95 | 0.00 | 0.00 | 270 | 1.4 x 0.4 x 0.45 | floor |   |
| 9 | `KitchenSinkUnit` | BUILD (PropKit) | -4.90 | 0.00 | -3.61 | 0 | 1.0 x 0.6 x 0.9 | floor |   |
| 10 | `Dresser` | BUILD (PropKit) | -1.84 | 0.00 | 1.60 | 270 | 1.4 x 0.5 x 2.0 | floor |   |
| 11 | `WallClock` | Poly Haven `wall_clock` (1k) | -1.59 | 1.82 | -0.60 | 270 | 0.35 x 0.06 x 0.35 | wall-hung |   |
| 12 | `SideTable` | Poly Haven `side_table_01` (1k) | -2.10 | 0.00 | 3.17 | 180 | 0.5 x 0.5 x 0.6 | floor |   |
| 13 | `RotaryPhone` | Sketchfab `812e8f59f9e749fab483eabf4800dec2` (CC BY, sara El-sisi) | -2.10 | 0.60 | 3.17 | 180 | 0.22 x 0.25 x 0.14 | on H13.p012 | phone  |
| 14 | `Radio` | Poly Haven `vintage_radio_transceiver` (1k) | -2.25 | 0.60 | 3.17 | 180 | 0.45 x 0.25 x 0.3 | on H13.p012 | radio farming programme by day |

### parlour - Front parlour
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 15 | `Armchair` | Poly Haven `ArmChair_01` (2k) | 0.50 | 0.00 | 1.40 | 270 | 0.85 x 0.85 x 0.95 | floor |   |
| 16 | `Commode` | Poly Haven `GothicCommode_01` (2k) | -1.16 | 0.00 | 2.80 | 90 | 1.1 x 0.5 x 0.9 | floor |  best-china cabinet |
| 17 | `Rug` | BUILD (PropKit) | 0.30 | 0.00 | 2.00 | 0 | 2.0 x 1.4 x 0.01 | floor |   |

### hall - Back hall & stair
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 18 | `FuseBox` | Poly Haven `power_box_01` (1k) | -1.36 | 1.40 | -1.00 | 90 | 0.45 x 0.15 x 0.6 | wall-hung | fuse-box  |
| 19 | `Mirror` | Poly Haven `ornate_mirror_01` (1k) | -0.80 | 1.15 | -3.42 | 0 | 0.6 x 0.05 x 0.9 | wall-hung |  small mirror by the back door |
| 20 | `RubberBoots` | Poly Haven `rubber_boots` (1k) | -1.26 | 0.00 | -3.20 | 90 | 0.3 x 0.3 x 0.4 | floor |   |

### dairy - Dairy
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 21 | `MilkChurns` | BUILD (PropKit) | -5.42 | 0.00 | -5.50 | 90 | 0.9 x 0.4 x 0.7 | floor |   |
| 22 | `CheesePress` | BUILD (PropKit) | -1.63 | 0.00 | -5.00 | 270 | 0.6 x 0.5 x 1.4 | floor |   |
| 23 | `Copper` | BUILD (PropKit) | -4.40 | 0.00 | -6.22 | 0 | 0.8 x 0.8 x 0.9 | floor |   |

### barn - Barn (PUZZLE)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 24 | `ScarecrowBoard` | BUILD (PropKit) | 2.11 | 0.90 | -2.00 | 90 | 1.6 x 0.1 x 1.2 | wall-hung | puzzle SCARECROW BOARD: a painted field map with 6 peg holes |
| 25 | `ChestWorn` | owned: Res_Sto_Chest_Wood_Worn_08 (Fab Saloon Interior) | 3.20 | 0.00 | -3.27 | 0 | 0.9 x 0.5 x 0.55 | floor |  CORN CHEST = PART CONTAINER |
| 26 | `HayBales` | Sketchfab `6b775049e57e4d9da87977905493ebfb` (CC BY, FrodoUndead) | 11.27 | 0.00 | 1.50 | 270 | 1.2 x 0.5 x 0.45 | floor |   |
| 27 | `HayBales` | Sketchfab `6b775049e57e4d9da87977905493ebfb` (CC BY, FrodoUndead) | 11.27 | 0.00 | 0.00 | 270 | 1.2 x 0.5 x 0.45 | floor |   |
| 28 | `Wheelbarrow` | Sketchfab `48314bafaffc4593ba1d7a5af31810c6` (CC BY, Peter Nox) | 6.00 | 0.00 | -2.00 | 0 | 1.4 x 0.6 x 0.6 | floor |   |
| 29 | `CartWheel` | owned: His_Wil_Cart_Wheel_Wood_Old_01 (Fab Saloon Interior) | 5.50 | 0.00 | -3.46 | 0 | 1.1 x 0.12 x 1.1 | floor |   |
| 30 | `ToolChest` | Poly Haven `metal_tool_chest` (1k) | 8.00 | 0.00 | -3.29 | 0 | 0.7 x 0.45 x 0.9 | floor |   |
| 31 | `Scarecrow` | Sketchfab `38b41b2deeb24b1d877c2551526d5654` (CC BY, Mehdi Shahsavan) | 9.50 | 0.00 | 1.50 | 200 | 1.2 x 0.4 x 2.0 | floor |  a 7th scarecrow propped in the barn, facing the door |
| 32 | `Spade` | Poly Haven `rusted_spade_01` (1k) | 3.00 | 0.00 | 3.48 | 180 | 0.2 x 0.08 x 1.1 | floor |   |

### bed1 - Barrows' bedroom
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 37 | `Bed` | Poly Haven `GothicBed_01` (2k) | -4.37 | 0.00 | 2.00 | 90 | 1.6 x 2.1 x 1.4 | floor |   |
| 38 | `Nightstand` | Poly Haven `ClassicNightstand_01` (1k) | -5.22 | 0.00 | 0.90 | 90 | 0.5 x 0.4 x 0.6 | floor |   |
| 39 | `Nightstand` | Poly Haven `ClassicNightstand_01` (1k) | -5.22 | 0.00 | 3.10 | 90 | 0.5 x 0.4 x 0.6 | floor |   |
| 40 | `Wardrobe` | Poly Haven `GothicCabinet_01` (2k) | -1.89 | 0.00 | 2.40 | 270 | 1.2 x 0.6 x 2.1 | floor |   |

### bed2 - Lads' bedroom
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 41 | `BedIron` | Poly Haven `old_bed_frame` (2k) | -4.60 | 0.00 | -2.42 | 0 | 1.0 x 2.0 x 1.05 | floor |   |
| 42 | `ChestOfDrawers` | Poly Haven `vintage_wooden_drawer_01` (1k) | -4.60 | 0.00 | -0.33 | 180 | 0.9 x 0.48 x 1.0 | floor |   |

### landing - Landing
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 47 | `ShrineNiche` | BUILD (PropKit) | -1.00 | 0.90 | -3.33 | 0 | 0.6 x 0.25 x 0.9 | wall-hung | shrine  |

### bath - Bathroom
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 43 | `Bathtub` | Sketchfab `d06c028a89a54c469e4af9cac50606fa` (CC BY, Lady Lion Studios) | 1.53 | 0.00 | 2.00 | 270 | 1.65 x 0.75 x 0.62 | floor |   |
| 44 | `Sink` | Sketchfab `a278d63585494921906ab3500c421ff7` (CC BY, lagesnpiet) | -1.19 | 0.00 | 2.60 | 90 | 0.55 x 0.45 x 0.85 | floor |   |
| 45 | `Mirror` | Poly Haven `ornate_mirror_01` (1k) | -1.42 | 1.10 | 2.60 | 90 | 0.6 x 0.05 x 0.9 | wall-hung | mirror  |
| 46 | `Toilet` | Sketchfab `6ac515a1c4154db18b5b4bd0b46d6405` (CC BY, Allan-Jay Branscombe) | -1.08 | 0.00 | 1.20 | 90 | 0.4 x 0.65 x 0.8 | floor |   |

### hayloft - Hayloft (attic of the barn)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 33 | `HayBales` | Sketchfab `6b775049e57e4d9da87977905493ebfb` (CC BY, FrodoUndead) | 6.00 | 0.00 | -3.27 | 0 | 1.2 x 0.5 x 0.45 | floor |   |
| 34 | `HayBales` | Sketchfab `6b775049e57e4d9da87977905493ebfb` (CC BY, FrodoUndead) | 5.00 | 0.00 | 3.27 | 180 | 1.2 x 0.5 x 0.45 | floor |   |
| 35 | `CardboardBox` | Poly Haven `cardboard_box_01` (1k) | 2.29 | 0.00 | -2.00 | 90 | 0.5 x 0.4 x 0.4 | floor |   |
| 36 | `Wicker2` | Poly Haven `wicker_basket_02` (1k) | 9.00 | 0.00 | -3.32 | 0 | 0.4 x 0.4 x 0.4 | floor |   |

## 4. Gameplay anchors (empty GameObjects the builder must create)
| Kind | Room | x | y | z | What |
|---|---|---|---|---|---|
| item-spot | kitchen | -5.07 | 1.40 | 0.00 | inglenook shelf |
| note-spot | kitchen | -3.60 | 0.78 | 0.10 | farm diary: 'moved the guardians again' (note 1/3) |
| item-spot | kitchen | -1.84 | 2.00 | 1.60 | kitchen dresser |
| puzzle | barn | 2.11 | 0.90 | -2.00 | SequencePuzzle reskinned: the hayloft drawing shows where each of the 6 scarecrows stood the day the boy vanished; compare with where they stand NOW in the fields; press the pegs of the ones that moved, in the order of the drawing's numbers |
| container | barn | 3.20 | 0.00 | -3.27 | Straw heart (a corn dolly), tied with red thread |
| hiding | barn | 11.27 | 0.00 | 1.50 | behind the hay bales -> HidingSpot on H13.p026 (HayBales) - crouched |
| hiding | hayloft | 6.00 | 0.00 | -3.27 | in the hay, hayloft -> HidingSpot on H13.p033 (HayBales) - crouched |
| note-spot | hayloft | 2.29 | 0.40 | -1.90 | THE BARN DRAWING: child's crayon picture of the fields with numbered scarecrows (note 2/3) |
| item-spot | hayloft | 9.00 | 0.40 | -3.32 | basket in the hayloft |
| hiding | bed1 | -4.37 | 0.00 | 2.00 | under the double bed -> HidingSpot on H13.p037 (Bed) - crouched |
| hiding | bed1 | -1.89 | 0.00 | 2.40 | wardrobe -> HidingSpot on H13.p040 (Wardrobe) |
| hiding | bed2 | -4.60 | 0.00 | -2.42 | under the lads' bed -> HidingSpot on H13.p041 (BedIron) - crouched |
| note-spot | bed2 | -4.50 | 1.00 | -0.33 | the lad's school essay 'Our scarecrows' (note 3/3) |
| shrine | landing | -1.00 | 0.00 | -3.33 | Rite of Return point (active only if this house is a shrine this match) |

Mapping to existing code: `item-spot` -> `House.ItemSpots` (order as listed), `note-spot` -> the 3 `Readable` notes, `puzzle` -> `PuzzleRig` anchor, `container` -> `PartContainer`, `shrine` -> `House.ShrineMarker` (+ shrine niche inside), `hiding` -> `HidingSpot`.

## 5. Puzzle - Straw heart
1. LOCK FIRST: the scarecrow board on the barn wall - a painted field map with 6 empty peg holes.
2. The hayloft drawing (note 2/3) shows the 6 scarecrows numbered where they stood 'the day he went'.
3. In the fields NOW 2-4 scarecrows have moved (MatchPlan). Players go out (exposed) to compare.
4. Press the pegs of the moved scarecrows in the drawing's number order (SequencePuzzle).
5. Wrong: every scarecrow in the field turns to face the farm (DESIGN night behaviour). Part: Straw heart.

## 6. Night changes (DESIGN 'Night changes' - pick 2-3 per match)
- scarecrows turn to face the nearest player between acts (DESIGN)
- the 7th scarecrow in the barn is gone
- milk churns are full of warm milk
- plus the shared pool: extra person in the family photo, a door opening onto a brick wall, moved furniture, an extra hallway door, all clocks stopped at the same time, writing on a wall in a player's name

## 7. Sound
- norway/rural: wind over fields, crows
- barn: hay rustle, beam creaks
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
