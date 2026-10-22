# H09 - Miller's House (Thorne)

> Machine-readable twin: `docs/design/houses/H09.json` - the builder reads THAT file; this page explains it. Every number here comes from the same data and is validated (no overlaps, door clearances, required items).

| | |
|---|---|
| **Archetype** | Miller's house joined to a three-storey watermill with a big undershot wheel on the leat |
| **Silhouette (must read at 50 m in fog)** | Two buildings in one: a small rendered house with a yellow door, and a tall mill (stone base, black weatherboarded upper floors, a lucam/hoist hood projecting from the top floor) with a 4 m waterwheel on its EAST wall over the mill race. One chimney, on the house's west gable. |
| **Ritual part** | Last-harvest sheaf |
| **M1 puzzle rig (code)** | `PuzzleKind.Clock` |
| **Letter traits (HouseCatalog)** | door **Yellow**, **1** chimney(s), stopped clock **10:00**, family **Thorne** |
| **World position** | x -104.0, z -96.0, floor level y -0.60, yaw 0 deg - Mill Lane (end, at the pond) |
| **Front door (world)** | (-103.5, -92.0) - 228.8 m from the altar, 2.0 m from the lane |
| **Placement note** | waterwheel on the mill's east wall sits in the mill race |

## Reference images

![plan](../ref/H09_plan.png)

![front-left](../ref/H09_massing_front.png) ![back-right](../ref/H09_massing_back.png)

Real-world references (look, don't copy): [Watermill](https://en.wikipedia.org/wiki/Watermill), [Lucarne](https://en.wikipedia.org/wiki/Lucarne)

Mood-image prompt (Higgsfield / Gemini / ChatGPT image - save the result as `docs/design/ref/H09_mood.png`):

```text
small cream rendered miller's house with a yellow door joined to a tall three-storey watermill with a stone base and black peeling weatherboarding, a large wooden undershot waterwheel, a hoist hood projecting from the top floor, a still mill pond in front, photorealistic, late 1970s rural English village, overcast autumn dusk, damp, muted greens and greys, 35mm film still, Phasmophobia / Resident Evil Village mood, no people, eye-level, 16:9
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
| house | (-5.0, -4.0) -> (3.0, 4.0) | 8.0 x 8.0 | G, F | RenderMossy (cream render, green damp at the base) | 0.35 | Gable roof 40 deg, ridge along x at 7.3 m, RoofClay2; chimney on the west gable |
| mill | (3.0, -5.0) -> (11.0, 5.0) | 8.0 x 10.0 | G, F, A | Ground: BrickDark + StoneBlock (0.6 thick); upper: Weatherboard painted black, peeling | 0.45 | Gable roof 45 deg, ridge along x at 12.5 m, RoofSlateGrey; LUCAM (hoist hood) on the north side at x=7, beam + chain + pulley |

### Chimneys (count must equal HouseCatalog = 1)
| x | z | stack w x d | above ridge | pots | serves |
|---|---|---|---|---|---|
| -5.0 | -2.0 | 0.8 x 0.8 | 1.0 | 1 | kitchen range = THE HEARTH |

### Exterior details
- UNDERSHOT WATERWHEEL 4.0 m dia, 1.2 m wide, on the mill's east wall over the race; stopped (animates when solved)
- LUCAM (hoist hood) projecting from the top floor with a chain and pulley; the hoist door 9 m up
- black weatherboarding peeling to grey; stone/brick ground floor; a horseshoe over the yellow house door
- mill pond to the west with a sluice gate and lever (the sluice is part of the puzzle)

### Plot, garden and yard
- Mill pond 25 x 15 m (water plane), reeds, a rowing boat sunk to its gunwales
- Mill race 2 m wide under a footbridge; sack cart; millstone propped against the wall (spare)
- Plot boundary (world polygon): (-110.0,-106.0), (-92.0,-106.0), (-92.0,-87.0), (-110.0,-87.0). Boundary = DryStoneWall / FencePicket on the street side, FenceRail or hedges (Shrub1-4) elsewhere, a GardenGate on every path.

## 2. Rooms

| Room | Level | Rect (x0,z0,x1,z1) | Clear inside | Kind | Floor | Walls | Ceiling | Light |
|---|---|---|---|---|---|---|---|---|
| **parlour** Parlour | G | (-5.0, 0.0, -0.8, 4.0) | 3.79 x 3.59 | living | FloorOldWood1 | Wallpaper2C | CeilingInterior | pendant |
| **hall** Hall | G | (-0.8, 0.0, 3.0, 4.0) | 3.68 x 3.59 | hall | FloorWornTile | PlasterWorn | CeilingInterior | pendant |
| **kitchen** Kitchen | G | (-5.0, -4.0, 3.0, 0.0) | 7.59 x 3.59 | kitchen | FloorFlagstone | Whitewash | CeilingInterior | pendant |
| **gearfloor** Mill: wheel & gear floor (PUZZLE) | G | (3.0, -5.0, 11.0, 5.0) | 7.10 x 9.10 | mill | FloorDirt | Whitewash flaking off stone | CeilingInterior | 2 CagedLight on a separate mill circuit - pit wheel in a slot along the east wall; the main shaft rises through the floor above |
| **bed1** Thornes' bedroom | F | (-5.0, 0.0, -0.8, 4.0) | 3.79 x 3.59 | bedroom | CarpetWorn | Wallpaper1 | CeilingInterior | pendant |
| **landing** Landing | F | (-0.8, 0.0, 3.0, 4.0) | 3.68 x 3.59 | landing | FloorOldWood1 | PlasterWorn | CeilingInterior | pendant |
| **bed2** Spare bedroom | F | (-5.0, -4.0, 0.0, 0.0) | 4.59 x 3.59 | bedroom | FloorOldWood2 | Wallpaper1B | CeilingInterior | pendant |
| **bath** Bathroom | F | (0.0, -4.0, 3.0, 0.0) | 2.88 x 3.59 | bathroom | FloorBathTile | TileWallWhite | CeilingInterior | pendant |
| **stonefloor** Mill: stone floor | F | (3.0, -5.0, 11.0, 5.0) | 7.10 x 9.10 | mill | FloorOldWood2 | RawPlankWall + flour dust everywhere | CeilingInterior | pendant |
| **binfloor** Mill: bin floor (attic) | A | (3.0, -5.0, 11.0, 5.0) | 7.10 x 9.10 | attic | FloorOldWood2 | RawPlankWall | CeilingInterior | none - flashlight only |

Skirting: 0.15 m dark oak in every room except kitchens/sculleries/cellars (none) and bathrooms (tile). Every ground-floor room has one LightSwitch by its main door at 1.3 m; every room has the existing `Room` component.

### Doors (how the rooms connect)
| Door | Level | From -> to | Wall | Centre at | W x H | Leaf | Role |
|---|---|---|---|---|---|---|---|
| d_front | G | hall -> ext | N | 0.5 | 0.9 x 2.05 | yellow ledged-and-braced door with a horseshoe over it (Yellow) | front |
| d_back | G | kitchen -> ext | S | 1.4 | 0.85 x 2.05 | panel - to the mill pond path | back |
| d_par | G | hall -> parlour | W | 2.0 | 0.85 x 2.05 | panel |  |
| d_kit | G | hall -> kitchen | S | 0.2 | 0.85 x 2.05 | panel |  |
| d_mill | G | kitchen -> gearfloor | E | -2.0 | 0.9 x 2.05 | iron-bound door into the mill (two steps down) |  |
| d_mill_ext | G | gearfloor -> ext | N | 7.0 | 1.6 x 2.4 | double cart doors (barred inside) | loading |
| d_b1 | F | landing -> bed1 | W | 2.0 | 0.8 x 2.05 | panel |  |
| d_b2 | F | landing -> bed2 | S | -0.4 | 0.75 x 2.05 | panel |  |
| d_bath | F | landing -> bath | S | 1.0 | 0.75 x 2.05 | panel |  |
| d_hoist | A | binfloor -> ext | N | 7.0 | 1.2 x 1.8 | lucam hoist door (opens onto a 9 m drop) | hoist |

### Windows
| Window | Room | Wall | Centre at | W x H | Sill | Style | Weak | Curtains / shutters |
|---|---|---|---|---|---|---|---|---|
| w_par | parlour | N | -2.9 | 1.2 x 1.3 | 0.9 | sash |  | FabricGingham  |
| w_k | kitchen | S | -2.5 | 1.1 x 1.1 | 0.9 | sash, cracked | **WEAK** (break-in point) |   |
| w_gf | gearfloor | E | 2.5 | 0.8 x 0.8 | 1.2 | small barred window beside the wheel (one bar rusted through) | **WEAK** (break-in point) |   |
| w_gf2 | gearfloor | S | 7.0 | 0.8 x 0.8 | 1.2 | small window |  |   |
| w_b1 | bed1 | N | -2.9 | 1.0 x 1.1 | 0.9 | sash |  |   |
| w_b2 | bed2 | S | -2.5 | 1.0 x 1.1 | 0.9 | sash |  |   |
| w_bath | bath | S | 1.5 | 0.6 x 0.7 | 1.3 | frosted |  |   |
| w_sf | stonefloor | E | -2.5 | 0.8 x 0.9 | 0.9 | weatherboard-framed window over the wheel |  |   |
| w_bin | binfloor | E | 0.0 | 0.6 x 0.6 | 1.0 | gable window |  |   |

### Stairs, ladders and voids
| Id | In room | Rect | Goes | Up toward | Type |
|---|---|---|---|---|---|
| s1 | hall | (2.0, 0.3, 2.9, 3.6) | G -> F | N | straight - steep cottage stair |
| s1u | landing | (2.0, 0.3, 2.9, 3.6) | G -> F | N | void |
| s2 | gearfloor | (3.5, -4.6, 4.3, -1.2) | G -> F | S | mill ladder-stair (open treads, handrail) |
| s2u | stonefloor | (3.5, -4.6, 4.3, -1.2) | G -> F | S | void |
| s3 | stonefloor | (3.5, 1.2, 4.3, 4.6) | F -> A | N | mill ladder-stair to the bin floor |
| s3u | binfloor | (3.5, 1.2, 4.3, 4.6) | F -> A | N | void |

## 3. Every prop, room by room

Position = prop **pivot at the centre of its footprint, on the floor** (or on top of its parent for `on`, or the centre height `y` for wall-hung). Yaw = the way the prop's FRONT faces. Scale the asset so its bounds fit the listed size (see ASSETS.md 'Scaling rule').

### parlour - Parlour
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 11 | `SofaPainted` | Poly Haven `painted_wooden_sofa` (2k) | -4.27 | 0.00 | 2.00 | 90 | 1.6 x 0.7 x 0.9 | floor |   |
| 12 | `Armchair` | Poly Haven `ArmChair_01` (2k) | -2.60 | 0.00 | 2.00 | 200 | 0.85 x 0.85 x 0.95 | floor |   |
| 13 | `SideTable` | Poly Haven `side_table_01` (1k) | -2.60 | 0.00 | 0.34 | 0 | 0.5 x 0.5 x 0.6 | floor |   |
| 14 | `RotaryPhone` | Sketchfab `812e8f59f9e749fab483eabf4800dec2` (CC BY, sara El-sisi) | -2.60 | 0.60 | 0.34 | 0 | 0.22 x 0.25 x 0.14 | on H09.p013 | phone  |
| 15 | `Radio` | Poly Haven `vintage_radio_transceiver` (1k) | -2.45 | 0.60 | 0.34 | 0 | 0.45 x 0.25 x 0.3 | on H09.p013 | radio  |
| 16 | `FrameFancy1` | Poly Haven `fancy_picture_frame_01` (1k) | -0.89 | 1.32 | 2.00 | 270 | 0.6 x 0.05 x 0.75 | wall-hung |  painting of the mill in 1890 |

### hall - Hall
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 17 | `Mirror` | Poly Haven `ornate_mirror_01` (1k) | -0.71 | 1.15 | 1.00 | 90 | 0.6 x 0.05 x 0.9 | wall-hung |   |
| 18 | `FuseBox` | Poly Haven `power_box_01` (1k) | 1.50 | 1.40 | 0.14 | 0 | 0.45 x 0.15 x 0.6 | wall-hung | fuse-box house circuit; the MILL circuit has a second box on the gear floor |

### kitchen - Kitchen
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 1 | `Hearth` | BUILD (PropKit) | -4.27 | 0.00 | -1.90 | 90 | 1.8 x 0.7 x 1.4 | floor | hearth black range with a bread oven; corn dollies hung on the beam |
| 2 | `MantelClock` | Poly Haven `mantel_clock_01` (1k) | -4.27 | 1.40 | -1.90 | 90 | 0.3 x 0.14 x 0.32 | on H09.p001 |  stopped at 10:00 |
| 3 | `FrameStanding2` | Poly Haven `standing_picture_frame_02` (1k) | -4.27 | 1.40 | -2.50 | 90 | 0.22 x 0.08 x 0.18 | on H09.p001 | family-photo the Thornes at harvest, 1966 |
| 4 | `TableWood3` | Poly Haven `WoodenTable_03` (1k) | -1.50 | 0.00 | -1.80 | 0 | 2.0 x 0.9 x 0.78 | floor |   |
| 5 | `BowlWood2` | Poly Haven `wooden_bowl_02` (1k) | -1.50 | 0.78 | -1.80 | 0 | 0.3 x 0.3 x 0.12 | on H09.p004 |   |
| 6 | `SweetPotato` | Poly Haven `sweet_potato` (1k) | -1.00 | 0.78 | -1.80 | 0 | 0.2 x 0.08 x 0.08 | on H09.p004 |   |
| 7 | `BenchPainted` | Poly Haven `painted_wooden_bench` (1k) | -1.50 | 0.00 | -2.65 | 0 | 1.4 x 0.4 x 0.45 | floor |   |
| 8 | `Dresser` | BUILD (PropKit) | -3.40 | 0.00 | -0.34 | 180 | 1.4 x 0.5 x 2.0 | floor |   |
| 9 | `KitchenSinkUnit` | BUILD (PropKit) | -0.60 | 0.00 | -3.32 | 0 | 1.0 x 0.6 x 0.9 | floor |   |
| 10 | `CabinetPainted` | Poly Haven `painted_wooden_cabinet` (2k) | 1.40 | 0.00 | -0.34 | 180 | 1.0 x 0.5 x 1.9 | floor |  larder |

### gearfloor - Mill: wheel & gear floor (PUZZLE)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 19 | `MillGears` | BUILD (PropKit) | 10.02 | 0.00 | 0.00 | 270 | 2.5 x 1.0 x 2.0 | floor | puzzle GEAR TRAIN against the wheel wall: pit wheel (fixed) -> 3 empty shafts -> crown wheel; 5 loose gears around the mill |
| 20 | `Sacks` | BUILD (PropKit) | 3.78 | 0.00 | 3.00 | 90 | 1.0 x 0.6 x 0.7 | floor |   |
| 21 | `ToolChest` | Poly Haven `metal_tool_chest` (1k) | 9.00 | 0.00 | -4.29 | 0 | 0.7 x 0.45 x 0.9 | floor |   |
| 22 | `TableWood1` | Poly Haven `WoodenTable_01` (1k) | 9.50 | 0.00 | 4.12 | 180 | 1.4 x 0.8 x 0.76 | floor |   |
| 23 | `HandTruck` | Poly Haven `hand_truck` (1k) | 6.00 | 0.00 | 2.50 | 0 | 0.5 x 0.5 x 1.2 | floor |   |
| 24 | `FuseBox` | Poly Haven `power_box_01` (1k) | 3.52 | 1.40 | -3.00 | 90 | 0.45 x 0.15 x 0.6 | wall-hung |  mill circuit box (second fuse box) |

### bed1 - Thornes' bedroom
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 32 | `BedIron` | Poly Haven `old_bed_frame` (2k) | -3.62 | 0.00 | 2.00 | 90 | 1.0 x 2.0 x 1.05 | floor |   |
| 33 | `NightstandPainted` | Poly Haven `painted_wooden_nightstand` (1k) | -4.43 | 0.00 | 1.23 | 90 | 0.45 x 0.38 x 0.6 | floor |   |
| 34 | `NightstandPainted` | Poly Haven `painted_wooden_nightstand` (1k) | -4.43 | 0.00 | 2.77 | 90 | 0.45 x 0.38 x 0.6 | floor |   |
| 35 | `Wardrobe` | Poly Haven `GothicCabinet_01` (2k) | -2.60 | 0.00 | 0.39 | 0 | 1.2 x 0.6 x 2.1 | floor |   |

### landing - Landing
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 42 | `ShrineNiche` | BUILD (PropKit) | 0.40 | 0.90 | 3.52 | 180 | 0.6 x 0.25 x 0.9 | wall-hung | shrine  |

### bed2 - Spare bedroom
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 36 | `BedIron` | Poly Haven `old_bed_frame` (2k) | -3.62 | 0.00 | -2.00 | 90 | 1.0 x 2.0 x 1.05 | floor |   |
| 37 | `ChestOfDrawers` | Poly Haven `vintage_wooden_drawer_01` (1k) | -2.20 | 0.00 | -0.33 | 180 | 0.9 x 0.48 x 1.0 | floor |   |

### bath - Bathroom
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 38 | `Bathtub` | Sketchfab `d06c028a89a54c469e4af9cac50606fa` (CC BY, Lady Lion Studios) | 2.59 | 0.00 | -2.40 | 270 | 1.4 x 0.64 x 0.53 | floor |   |
| 39 | `Sink` | Sketchfab `a278d63585494921906ab3500c421ff7` (CC BY, lagesnpiet) | 2.20 | 0.00 | -0.32 | 180 | 0.55 x 0.45 x 0.85 | floor |   |
| 40 | `Mirror` | Poly Haven `ornate_mirror_01` (1k) | 2.20 | 1.10 | -0.09 | 180 | 0.6 x 0.05 x 0.9 | wall-hung | mirror  |
| 41 | `Toilet` | Sketchfab `6ac515a1c4154db18b5b4bd0b46d6405` (CC BY, Allan-Jay Branscombe) | 0.41 | 0.00 | -3.20 | 90 | 0.4 x 0.65 x 0.8 | floor |   |

### stonefloor - Mill: stone floor
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 25 | `Millstones` | BUILD (PropKit) | 7.00 | 0.00 | 0.00 | 0 | 1.6 x 1.6 x 1.6 | floor |  pair of millstones in a wooden tun, hopper above |
| 26 | `Sacks` | BUILD (PropKit) | 6.00 | 0.00 | -4.22 | 0 | 1.0 x 0.6 x 0.7 | floor |   |
| 27 | `BarrelWorn` | owned: His_Med_Storage_Barrel_Wood_Worn_01 (Fab) | 9.50 | 0.00 | -4.20 | 0 | 0.65 x 0.65 x 0.95 | floor |   |
| 28 | `TableSmall` | Poly Haven `small_wooden_table_01` (1k) | 9.00 | 0.00 | 4.27 | 180 | 0.7 x 0.5 x 0.72 | floor |   |

### binfloor - Mill: bin floor (attic)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 29 | `CellarCrates` | BUILD (PropKit) | 7.00 | 0.00 | -4.07 | 0 | 1.4 x 0.9 x 1.2 | floor |  grain bin + hopper chute: PART CONTAINER (drops when the mill runs) |
| 30 | `Sacks` | BUILD (PropKit) | 9.50 | 0.00 | 4.22 | 180 | 1.0 x 0.6 x 0.7 | floor |   |
| 31 | `Wicker2` | Poly Haven `wicker_basket_02` (1k) | 9.50 | 0.00 | -4.32 | 0 | 0.4 x 0.4 x 0.4 | floor |   |

## 4. Gameplay anchors (empty GameObjects the builder must create)
| Kind | Room | x | y | z | What |
|---|---|---|---|---|---|
| item-spot | kitchen | -4.27 | 1.40 | -1.90 | range mantel |
| note-spot | kitchen | -1.60 | 0.78 | -1.80 | miller's account book: 'gears 5, 7, 9 - never the small one' (note 1/3) |
| hiding | kitchen | 1.40 | 0.00 | -0.34 | larder cupboard -> HidingSpot on H09.p010 (CabinetPainted) |
| item-spot | parlour | -2.75 | 0.60 | 0.34 | phone table |
| puzzle | gearfloor | 10.02 | 0.00 | 0.00 | Restart the mill (ClockPuzzle slot for M1; target rig = GearPuzzle): place 3 of the 5 gears on the shafts so they mesh (radii 0.25/0.35/0.45/0.55/0.65; only one combination meshes per match). Then pull the sluice lever: the wheel turns, VERY LOUD for 20 s, the bin-floor hopper drops the part. |
| hiding | gearfloor | 3.78 | 0.00 | 3.00 | behind the flour sacks -> HidingSpot on H09.p020 (Sacks) - crouched |
| item-spot | gearfloor | 9.50 | 0.76 | 4.12 | mill bench |
| note-spot | stonefloor | 9.10 | 0.72 | 4.27 | torn page: the last harvest sheaf 'goes in the hopper' (note 2/3) |
| container | binfloor | 7.00 | 0.00 | -4.07 | Last-harvest sheaf, tied with red thread |
| note-spot | binfloor | 9.40 | 0.40 | -4.32 | child's drawing of the gears (note 3/3) |
| hiding | bed1 | -3.62 | 0.00 | 2.00 | under the iron bed -> HidingSpot on H09.p032 (BedIron) - crouched |
| hiding | bed1 | -2.60 | 0.00 | 0.39 | wardrobe -> HidingSpot on H09.p035 (Wardrobe) |
| shrine | landing | 0.40 | 0.00 | 3.52 | Rite of Return point (active only if this house is a shrine this match) |

Mapping to existing code: `item-spot` -> `House.ItemSpots` (order as listed), `note-spot` -> the 3 `Readable` notes, `puzzle` -> `PuzzleRig` anchor, `container` -> `PartContainer`, `shrine` -> `House.ShrineMarker` (+ shrine niche inside), `hiding` -> `HidingSpot`.

## 5. Puzzle - Last-harvest sheaf
1. LOCK FIRST: from the kitchen door you face the gear train: one empty shaft is right at eye level.
2. 5 loose gears lie around the mill (3 are needed). Only one combination meshes per match (MatchPlan).
3. Place gears (hold interact); a wrong gear grinds and falls (thud + noise).
4. When all 3 mesh, pull the SLUICE LEVER (outside, by the pond - exposed!) - the wheel turns, VERY LOUD for 20 s.
5. The bin-floor hopper drops the part onto the stone floor. Part: Last-harvest sheaf.

## 6. Night changes (DESIGN 'Night changes' - pick 2-3 per match)
- the waterwheel turns slowly backwards
- flour footprints lead up the ladder and stop mid-air
- every sack has a face pressed into it
- plus the shared pool: extra person in the family photo, a door opening onto a brick wall, moved furniture, an extra hallway door, all clocks stopped at the same time, writing on a wall in a player's name

## 7. Sound
- moaningmetal: gear strain
- rural: water rushing; norway: wind over the pond
- the wheel: big wooden creaks
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
