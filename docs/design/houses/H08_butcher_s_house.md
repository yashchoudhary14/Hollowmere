# H08 - Butcher's House (Grice)

> Machine-readable twin: `docs/design/houses/H08.json` - the builder reads THAT file; this page explains it. Every number here comes from the same data and is validated (no overlaps, door clearances, required items).

| | |
|---|---|
| **Archetype** | Main Street butcher's shop with the family living above and a cold room behind |
| **Silhouette (must read at 50 m in fog)** | Shop front of green-and-cream glazed tiles with a big open serving window under a torn striped canvas awning, 'J. GRICE - FAMILY BUTCHER' fascia, a red side door to the flat, brick upper floor with two sashes, party-wall chimneys both sides, a long single-storey cold room behind. |
| **Ritual part** | Bone whistle |
| **M1 puzzle rig (code)** | `PuzzleKind.CodeLock` |
| **Letter traits (HouseCatalog)** | door **Red**, **2** chimney(s), stopped clock **5:00**, family **Grice** |
| **World position** | x -7.75, z 66.0, floor level y +0.00, yaw 90 deg - Main Street (west side) |
| **Front door (world)** | (-10.8, 62.0) - 51.1 m from the altar, 3.5 m from the lane |
| **Placement note** | shop on the pavement; slaughter yard behind, opening onto Back Lane |

## Reference images

![plan](../ref/H08_plan.png)

![front-left](../ref/H08_massing_front.png) ![back-right](../ref/H08_massing_back.png)

Real-world references (look, don't copy): [Butcher](https://en.wikipedia.org/wiki/Butcher)

Mood-image prompt (Higgsfield / Gemini / ChatGPT image - save the result as `docs/design/ref/H08_mood.png`):

```text
village butcher's shop front with green and cream glazed tiles, open serving window with a marble sill, torn red and white striped awning, gilt fascia 'J. GRICE - FAMILY BUTCHER', red brick upper floor with two sash windows, red side door, photorealistic, late 1970s rural English village, overcast autumn dusk, damp, muted greens and greys, 35mm film still, Phasmophobia / Resident Evil Village mood, no people, eye-level, 16:9
```

## 1. Structure

### Levels
| Level | Floor y (m) | Clear height (m) |
|---|---|---|
| C - Cellar | -2.40 | 2.20 |
| G - Ground floor | +0.00 | 3.00 |
| F - First floor | +3.25 | 2.70 |

Floor build-up: 0.25 m between floors (joists + boards); ceilings use the room's `ceiling` material.

### Wings (massing)
| Wing | Rect x0,z0 -> x1,z1 | Size | Levels | Walls | Wall thickness | Roof |
|---|---|---|---|---|---|---|
| main | (-4.0, -6.0) -> (4.0, 4.0) | 8.0 x 10.0 | C, G, F | Shop front: green/cream tiles to 1.0 m then BrickRed; sides BrickMixed | 0.35 | Gable roof, ridge along x at 8.2 m, RoofClay2; chimneys on both side walls |
| cold | (-4.0, -11.0) -> (0.5, -6.0) | 4.5 x 5.0 | G | Whitewashed brick (Whitewash on BrickPaintedWorn), louvred vent | 0.3 | Mono-pitch CorrugatedRust roof |

### Chimneys (count must equal HouseCatalog = 2)
| x | z | stack w x d | above ridge | pots | serves |
|---|---|---|---|---|---|
| -4.0 | -3.0 | 0.8 x 0.8 | 1.0 | 2 | kitchen range = THE HEARTH |
| 4.0 | 2.0 | 0.8 x 0.8 | 1.0 | 2 | upstairs front-room fire |

### Exterior details
- shop front: green/cream glazed tiles to 1.0 m, marble slab sill, big open serving window with a sliding sash
- torn red-and-white striped canvas awning on iron arms
- fascia 'J. GRICE - FAMILY BUTCHER' (black, gilt)
- meat hooks rail inside the window; price tickets
- red side door to the flat with its own letterbox

### Plot, garden and yard
- Front: pavement + a stone mounting block
- Back: SLAUGHTER YARD 8 x 10 m: concrete floor with a drain, a hose, a tin bath, a block and tackle on a beam, a muck cart; a gate to Back Lane
- Plot boundary (world polygon): (-28.8,70.3), (-28.8,61.7), (-3.5,61.7), (-3.5,70.3). Boundary = DryStoneWall / FencePicket on the street side, FenceRail or hedges (Shrub1-4) elsewhere, a GardenGate on every path.

## 2. Rooms

| Room | Level | Rect (x0,z0,x1,z1) | Clear inside | Kind | Floor | Walls | Ceiling | Light |
|---|---|---|---|---|---|---|---|---|
| **shop** Butcher's shop | G | (-4.0, 0.0, 4.0, 4.0) | 7.30 x 3.59 | shop | FloorCheckTile | TileWallWhite to ceiling (blood-spattered grout) | CeilingInterior | 2 caged lights |
| **hall** Side passage & stair | G | (1.8, -6.0, 4.0, 0.0) | 1.79 x 5.59 | hall | FloorWornTile | PlasterWorn | CeilingInterior | pendant |
| **kitchen** Back kitchen | G | (-4.0, -6.0, 1.8, 0.0) | 5.39 x 5.59 | kitchen | FloorQuarryTile | Whitewash | CeilingInterior | pendant |
| **cold** Cold room (PUZZLE) | G | (-4.0, -11.0, 0.5, -6.0) | 3.90 x 4.64 | coldroom | FloorBrick | TileWallWhite, drain in the floor | CeilingInterior | one caged bulb |
| **living** Front room (upstairs) | F | (-4.0, 0.0, 4.0, 4.0) | 7.30 x 3.59 | living | Carpet70s | WallpaperQuatrefoil | CeilingInterior | pendant |
| **landing** Landing | F | (1.8, -6.0, 4.0, 0.0) | 1.79 x 5.59 | landing | Carpet70sB | Wallpaper2C | CeilingInterior | pendant |
| **bed1** Grices' bedroom | F | (-4.0, -2.5, 1.8, 0.0) | 5.39 x 2.38 | bedroom | CarpetWorn | Wallpaper1 | CeilingInterior | pendant |
| **pass** Back passage | F | (-4.0, -3.4, 1.8, -2.5) | 5.39 x 0.78 | passage | Carpet70sB | Wallpaper2C | CeilingInterior | pendant |
| **bed2** Son's bedroom | F | (-4.0, -6.0, -0.6, -3.4) | 2.99 x 2.19 | bedroom | FloorOldWood1 | Wallpaper1B (football wallpaper border) | CeilingInterior | pendant |
| **bath** Bathroom | F | (-0.6, -6.0, 1.8, -3.4) | 2.28 x 2.19 | bathroom | FloorBathTile | TileWallWhite | CeilingInterior | pendant |
| **cellar** Salting cellar | C | (-4.0, 0.0, 4.0, 4.0) | 7.30 x 3.30 | cellar | FloorBrick | Whitewash | CeilingInterior | pendant - salt troughs, hooks |

Skirting: 0.15 m dark oak in every room except kitchens/sculleries/cellars (none) and bathrooms (tile). Every ground-floor room has one LightSwitch by its main door at 1.3 m; every room has the existing `Room` component.

### Doors (how the rooms connect)
| Door | Level | From -> to | Wall | Centre at | W x H | Leaf | Role |
|---|---|---|---|---|---|---|---|
| d_shop | G | shop -> ext | N | -3.0 | 1.0 x 2.05 | shop door with a bell, 'CLOSED' sign | shop |
| d_front | G | hall -> ext | E | -3.0 | 0.9 x 2.05 | red side door to the flat, letterbox (Red) | front |
| d_back | G | hall -> ext | S | 2.9 | 0.85 x 2.05 | panel - to the slaughter yard | back |
| d_shopk | G | shop -> kitchen | S | -1.2 | 0.9 x 2.05 | half-glazed door with a meat-hook rail over it |  |
| d_hallk | G | hall -> kitchen | W | -4.4 | 0.85 x 2.05 | panel |  |
| d_cold | G | kitchen -> cold | S | -1.8 | 1.0 x 2.05 | insulated cold-room door with a lever latch (heavy, loud) |  |
| d_liv | F | landing -> living | N | 2.9 | 0.85 x 2.05 | panel |  |
| d_b1 | F | landing -> bed1 | W | -1.2 | 0.8 x 2.05 | panel |  |
| d_pass | F | landing -> pass | W | -2.95 | 0.8 x 2.05 | panel |  |
| d_b2 | F | pass -> bed2 | S | -1.2 | 0.8 x 2.05 | panel |  |
| d_bath | F | pass -> bath | S | 0.4 | 0.75 x 2.05 | panel |  |

### Windows
| Window | Room | Wall | Centre at | W x H | Sill | Style | Weak | Curtains / shutters |
|---|---|---|---|---|---|---|---|---|
| w_shop | shop | N | 1.0 | 3.6 x 1.8 | 0.9 | open serving window with a marble slab sill and a sliding sash |  |   |
| w_k | kitchen | W | -3.0 | 1.0 x 1.1 | 0.9 | side window onto the alley, cracked | **WEAK** (break-in point) |   |
| w_cold | cold | S | -1.75 | 0.6 x 0.4 | 1.8 | louvred vent (pull-out) | **WEAK** (break-in point) |   |
| w_l1 | living | N | -2.0 | 1.2 x 1.5 | 0.9 | sash |  | FabricVelvet  |
| w_l2 | living | N | 2.0 | 1.2 x 1.5 | 0.9 | sash |  | FabricVelvet  |
| w_b2 | bed2 | W | -4.8 | 0.9 x 1.1 | 0.9 | sash |  |   |
| w_bath | bath | S | 0.4 | 0.7 x 0.8 | 1.3 | frosted |  |   |
| w_land | landing | E | -2.0 | 0.7 x 1.1 | 0.9 | stair window |  |   |

### Stairs, ladders and voids
| Id | In room | Rect | Goes | Up toward | Type |
|---|---|---|---|---|---|
| s1 | hall | (2.6, -5.0, 3.5, -1.5) | G -> F | S | straight - steep straight stair, 15 risers, lino treads |
| s1u | landing | (2.6, -5.0, 3.5, -1.5) | G -> F | S | void |
| s2 | shop | (2.6, 0.4, 3.5, 3.5) | C -> G | N | trapdoor + steps behind the counter to the salting cellar |
| s2c | cellar | (2.6, 0.4, 3.5, 3.5) | C -> G | N | arrives |

## 3. Every prop, room by room

Position = prop **pivot at the centre of its footprint, on the floor** (or on top of its parent for `on`, or the centre height `y` for wall-hung). Yaw = the way the prop's FRONT faces. Scale the asset so its bounds fit the listed size (see ASSETS.md 'Scaling rule').

### shop - Butcher's shop
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 1 | `Counter` | BUILD (PropKit) | 1.00 | 0.00 | 2.92 | 180 | 2.4 x 0.6 x 0.95 | floor |  marble-topped serving counter behind the window, 2.4 m |
| 2 | `Cleaver` | Sketchfab `6c3fc11e570b4d3ca4d8a862f3b6a916` (CC BY, Barend19) | 0.40 | 0.95 | 2.92 | 180 | 0.32 x 0.03 x 0.12 | on H08.p001 |   |
| 3 | `CuttingBoard` | Poly Haven `wooden_cutting_board` (1k) | 1.40 | 0.95 | 2.92 | 180 | 0.45 x 0.3 x 0.03 | on H08.p001 |   |
| 4 | `MeatRail` | BUILD (PropKit) | 0.00 | 2.20 | 2.20 | 0 | 2.4 x 0.2 x 0.6 | raised |  ceiling rail with 8 hooks (2 cloth-wrapped carcass shapes) |
| 5 | `ButcherBlock` | BUILD (PropKit) | -0.60 | 0.00 | 1.20 | 0 | 0.7 x 0.7 x 0.85 | floor |   |
| 6 | `Cleaver` | Sketchfab `6c3fc11e570b4d3ca4d8a862f3b6a916` (CC BY, Barend19) | -0.60 | 0.85 | 1.20 | 0 | 0.32 x 0.03 x 0.12 | on H08.p005 |   |
| 7 | `Fridge` | Sketchfab `68d173f205344455933f861042b99fe3` (CC BY, dylanheyes) | -3.31 | 0.00 | 2.60 | 90 | 0.6 x 0.62 x 1.45 | floor |  display fridge (humming) |
| 8 | `DrawerCabinet` | Poly Haven `drawer_cabinet` (1k) | 0.80 | 0.00 | 0.32 | 0 | 0.6 x 0.45 x 1.1 | floor |  till cabinet |
| 9 | `FrameHanging3` | Poly Haven `hanging_picture_frame_03` (1k) | -3.00 | 1.57 | 0.07 | 0 | 0.6 x 0.03 x 0.45 | wall-hung | family-photo Grice & son outside the shop, 1969 - the son is holding the whistle |

### hall - Side passage & stair
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 17 | `FuseBox` | Poly Haven `power_box_01` (1k) | 1.94 | 1.40 | -0.60 | 90 | 0.45 x 0.15 x 0.6 | wall-hung | fuse-box  |
| 18 | `Mirror` | Poly Haven `ornate_mirror_01` (1k) | 1.89 | 1.15 | -2.40 | 90 | 0.6 x 0.05 x 0.9 | wall-hung |  small mirror by the side door |

### kitchen - Back kitchen
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 10 | `Hearth` | BUILD (PropKit) | -3.27 | 0.00 | -4.40 | 90 | 1.8 x 0.7 x 1.4 | floor | hearth range in the west party-wall flue |
| 11 | `MantelClock` | Poly Haven `mantel_clock_01` (1k) | -3.27 | 1.40 | -4.40 | 90 | 0.3 x 0.14 x 0.32 | on H08.p010 |  stopped at 5:00 |
| 12 | `TableWood1` | Poly Haven `WoodenTable_01` (1k) | -1.20 | 0.00 | -3.00 | 0 | 1.4 x 0.8 x 0.76 | floor |   |
| 13 | `PotEnamel` | Poly Haven `pot_enamel_01` (1k) | -1.20 | 0.76 | -3.00 | 0 | 0.3 x 0.25 x 0.2 | on H08.p012 |   |
| 14 | `ChairDining` | Poly Haven `dining_chair_02` (1k) | -1.20 | 0.00 | -2.20 | 180 | 0.45 x 0.5 x 0.95 | floor |   |
| 15 | `KitchenSinkUnit` | BUILD (PropKit) | -3.00 | 0.00 | -0.39 | 180 | 1.0 x 0.6 x 0.9 | floor |   |
| 16 | `Dresser` | BUILD (PropKit) | 1.46 | 0.00 | -2.00 | 270 | 1.4 x 0.5 x 2.0 | floor |   |

### cold - Cold room (PUZZLE)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 19 | `BalanceScale` | BUILD (PropKit) | -0.03 | 0.00 | -8.50 | 270 | 1.1 x 0.4 x 0.9 | floor | puzzle BEAM SCALE on a cast pillar; brass weights 1/2/5/10/20 lb on the shelf |
| 20 | `ToolChest` | Poly Haven `metal_tool_chest` (1k) | -2.40 | 0.00 | -10.45 | 0 | 0.7 x 0.45 x 0.9 | floor |  PART CONTAINER: steel cash box bolted to the bench |
| 21 | `MeatRail` | BUILD (PropKit) | -1.75 | 2.00 | -8.50 | 90 | 2.4 x 0.2 x 0.6 | raised |  hanging cuts with tags (puzzle clues) |
| 22 | `SteelShelves2` | Poly Haven `steel_frame_shelves_02` (1k) | -3.47 | 0.00 | -8.50 | 90 | 1.0 x 0.4 x 1.8 | floor |   |
| 23 | `BarrelWorn` | owned: His_Med_Storage_Barrel_Wood_Worn_01 (Fab) | -3.35 | 0.00 | -10.20 | 90 | 0.65 x 0.65 x 0.95 | floor |   |

### living - Front room (upstairs)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 24 | `Hearth` | BUILD (PropKit) | 3.27 | 0.00 | 2.00 | 270 | 1.8 x 0.7 x 1.4 | floor | hearth-decor tiled fireplace (decorative; not a protective hearth) |
| 25 | `Sofa2` | Poly Haven `sofa_02` (2k) | -1.00 | 0.00 | 0.54 | 0 | 1.9 x 0.9 x 0.85 | floor |   |
| 26 | `Armchair` | Poly Haven `ArmChair_01` (2k) | 1.00 | 0.00 | 1.80 | 90 | 0.85 x 0.85 x 0.95 | floor |   |
| 27 | `Cabinet70s` | Poly Haven `modern_wooden_cabinet` (1k) | -3.40 | 0.00 | 2.00 | 90 | 1.2 x 0.45 x 0.8 | floor |   |
| 28 | `TV` | Poly Haven `Television_01` (1k) | -3.40 | 0.80 | 2.00 | 90 | 0.7 x 0.5 x 0.55 | on H08.p027 | tv  |
| 29 | `SideTable` | Poly Haven `side_table_01` (1k) | -3.40 | 0.00 | 0.34 | 0 | 0.5 x 0.5 x 0.6 | floor |   |
| 30 | `RotaryPhone` | Sketchfab `812e8f59f9e749fab483eabf4800dec2` (CC BY, sara El-sisi) | -3.40 | 0.60 | 0.34 | 0 | 0.22 x 0.25 x 0.14 | on H08.p029 | phone  |
| 31 | `CoffeeTable` | Poly Haven `CoffeeTable_01` (1k) | -1.00 | 0.00 | 1.60 | 0 | 1.1 x 0.6 x 0.45 | floor |   |
| 32 | `TableSmall` | Poly Haven `small_wooden_table_01` (1k) | 0.00 | 0.00 | 3.37 | 180 | 0.7 x 0.5 x 0.72 | floor |  writing table |

### landing - Landing
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 45 | `ShrineNiche` | BUILD (PropKit) | 3.52 | 0.90 | -0.60 | 270 | 0.6 x 0.25 x 0.9 | wall-hung | shrine  |

### bed1 - Grices' bedroom
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 33 | `Bed` | Poly Haven `GothicBed_01` (2k) | -2.57 | 0.00 | -1.25 | 90 | 1.6 x 2.1 x 1.4 | floor |   |
| 34 | `Commode` | Poly Haven `GothicCommode_01` (2k) | -0.40 | 0.00 | -2.16 | 0 | 1.1 x 0.5 x 0.9 | floor |   |

### bed2 - Son's bedroom
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 35 | `BedIron` | Poly Haven `old_bed_frame` (2k) | -2.62 | 0.00 | -5.00 | 90 | 1.0 x 2.0 x 1.05 | floor |   |
| 36 | `Wardrobe` | Poly Haven `GothicCabinet_01` (2k) | -0.99 | 0.00 | -5.00 | 270 | 1.2 x 0.6 x 2.1 | floor |   |
| 37 | `TableSmall` | Poly Haven `small_wooden_table_01` (1k) | -3.37 | 0.00 | -3.90 | 90 | 0.7 x 0.5 x 0.72 | floor |   |

### bath - Bathroom
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 38 | `Bathtub` | Sketchfab `d06c028a89a54c469e4af9cac50606fa` (CC BY, Lady Lion Studios) | 1.39 | 0.00 | -4.80 | 270 | 1.4 x 0.64 x 0.53 | floor |   |
| 39 | `Toilet` | Sketchfab `6ac515a1c4154db18b5b4bd0b46d6405` (CC BY, Allan-Jay Branscombe) | -0.18 | 0.00 | -5.40 | 90 | 0.4 x 0.65 x 0.8 | floor |   |
| 40 | `Sink` | Sketchfab `a278d63585494921906ab3500c421ff7` (CC BY, lagesnpiet) | 1.20 | 0.00 | -3.71 | 180 | 0.55 x 0.45 x 0.85 | floor |   |
| 41 | `Mirror` | Poly Haven `ornate_mirror_01` (1k) | 1.20 | 1.10 | -3.48 | 180 | 0.6 x 0.05 x 0.9 | wall-hung | mirror  |

### cellar - Salting cellar
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 42 | `SteelShelves` | Poly Haven `steel_frame_shelves_01` (1k) | -3.40 | 0.00 | 2.00 | 90 | 1.2 x 0.45 x 1.9 | floor |  salt sacks |
| 43 | `BarrelsWood` | Poly Haven `wooden_barrels_01` (1k) | -1.00 | 0.00 | 3.22 | 180 | 1.4 x 0.8 x 1.0 | floor |   |
| 44 | `Sacks` | BUILD (PropKit) | -2.00 | 0.00 | 0.68 | 0 | 1.0 x 0.6 x 0.7 | floor |   |

## 4. Gameplay anchors (empty GameObjects the builder must create)
| Kind | Room | x | y | z | What |
|---|---|---|---|---|---|
| note-spot | shop | 1.10 | 0.95 | 2.92 | price board (note 1/3) |
| item-spot | shop | -0.40 | 0.85 | 1.20 | butcher's block |
| item-spot | shop | 0.80 | 1.10 | 0.32 | till cabinet |
| item-spot | kitchen | -3.27 | 1.40 | -4.40 | kitchen mantel |
| puzzle | cold | -0.03 | 0.00 | -8.50 | CodeLockPuzzle reskinned: the LEDGER (upstairs) gives a total; find the cuts (hanging, each with a weight tag) and weights that balance it; the 4-digit combination = the tag numbers of the chosen cuts |
| container | cold | -2.40 | 0.00 | -10.45 | Bone whistle |
| hiding | cold | -3.35 | 0.00 | -10.20 | behind the brine barrel -> HidingSpot on H08.p023 (BarrelWorn) - crouched |
| note-spot | living | 0.10 | 0.72 | 3.37 | THE LEDGER: last entry 'weigh it all to 37 lb' (note 2/3) |
| hiding | bed1 | -2.57 | 0.00 | -1.25 | under the double bed -> HidingSpot on H08.p033 (Bed) - crouched |
| hiding | bed2 | -0.99 | 0.00 | -5.00 | son's wardrobe -> HidingSpot on H08.p036 (Wardrobe) |
| note-spot | bed2 | -3.37 | 0.72 | -3.80 | the son's diary (note 3/3) |
| hiding | bath | 1.39 | 0.00 | -4.80 | behind the bath curtain -> HidingSpot on H08.p038 (Bathtub) |
| shrine | landing | 3.52 | 0.00 | -0.60 | Rite of Return point (active only if this house is a shrine this match) |

Mapping to existing code: `item-spot` -> `House.ItemSpots` (order as listed), `note-spot` -> the 3 `Readable` notes, `puzzle` -> `PuzzleRig` anchor, `container` -> `PartContainer`, `shrine` -> `House.ShrineMarker` (+ shrine niche inside), `hiding` -> `HidingSpot`.

## 5. Puzzle - Bone whistle
1. LOCK FIRST: steel cash box bolted to the cold-room bench; the beam scale next to it.
2. The LEDGER (upstairs, note 2/3) says the total weight (randomised: 17-49 lb).
3. Cuts hanging on the rail carry weight tags; brass weights 1/2/5/10/20 lb.
4. Balance: total on one pan, choose cuts on the other; the tag NUMBERS of the chosen cuts form the code.
5. Dropping a weight = clunk noise. Part: Bone whistle (blowing it = DESIGN item? no: carried only).

## 6. Night changes (DESIGN 'Night changes' - pick 2-3 per match)
- the meat rail sways; one hook has a coat on it (a player's coat)
- the display fridge light is on with nothing inside but a doll's hand
- the cleaver is embedded in the kitchen door frame
- plus the shared pool: extra person in the family photo, a door opening onto a brick wall, moved furniture, an extra hallway door, all clocks stopped at the same time, writing on a wall in a player's name

## 7. Sound
- whistle: The Death Whistle (when the part is picked up)
- smallmetal: weights
- fridgehum
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
