# H05 - Doctor's House (Crane)

> Machine-readable twin: `docs/design/houses/H05.json` - the builder reads THAT file; this page explains it. Every number here comes from the same data and is validated (no overlaps, door clearances, required items).

| | |
|---|---|
| **Archetype** | White stucco Edwardian villa with a single-storey surgery wing |
| **Silhouette (must read at 50 m in fog)** | Square, white and prim: hipped red-slate roof with ONE central-west chimney, a columned porch over the white door, a flat-roofed surgery wing on the east with its own door and a brass plate 'DR. A. CRANE  M.B.', iron railings, a monkey-puzzle tree in the front garden. |
| **Ritual part** | Glass eye |
| **M1 puzzle rig (code)** | `PuzzleKind.CodeLock` |
| **Letter traits (HouseCatalog)** | door **White**, **1** chimney(s), stopped clock **4:00**, family **Crane** |
| **World position** | x -62.0, z 48.0, floor level y +0.30, yaw 90 deg - Chapel Lane (west side) |
| **Front door (world)** | (-56.0, 48.0) - 85.0 m from the altar, 5.8 m from the lane |
| **Placement note** | gravel drive to the surgery door |

## Reference images

![plan](../ref/H05_plan.png)

![front-left](../ref/H05_massing_front.png) ![back-right](../ref/H05_massing_back.png)

Real-world references (look, don't copy): [Edwardian architecture](https://en.wikipedia.org/wiki/Edwardian_architecture), [Stucco](https://en.wikipedia.org/wiki/Stucco)

Mood-image prompt (Higgsfield / Gemini / ChatGPT image - save the result as `docs/design/ref/H05_mood.png`):

```text
white stucco Edwardian doctor's villa, hipped red slate roof with one chimney, a two-column porch over a white door, a flat-roofed single-storey surgery wing with a brass plate, iron railings, grimy streaks under the windows, photorealistic, late 1970s rural English village, overcast autumn dusk, damp, muted greens and greys, 35mm film still, Phasmophobia / Resident Evil Village mood, no people, eye-level, 16:9
```

## 1. Structure

### Levels
| Level | Floor y (m) | Clear height (m) |
|---|---|---|
| C - Cellar | -2.40 | 2.20 |
| G - Ground floor | +0.00 | 3.00 |
| F - First floor | +3.25 | 2.80 |

Floor build-up: 0.25 m between floors (joists + boards); ceilings use the room's `ceiling` material.

### Wings (massing)
| Wing | Rect x0,z0 -> x1,z1 | Size | Levels | Walls | Wall thickness | Roof |
|---|---|---|---|---|---|---|
| main | (-5.0, -4.5) -> (5.0, 4.5) | 10.0 x 9.0 | C, G, F | StuccoWhite (smooth, grimy streaks under sills), BrickDark plinth 0.6 m, painted stone quoins (pale grey) | 0.35 | Hipped roof 30 deg, RoofRedSlate, deep bracketed eaves 0.5 m; chimney on the spine wall at (-3.1, 0) |
| surgery | (5.0, -3.0) -> (10.0, 3.0) | 5.0 x 6.0 | G | StuccoWhite, same plinth | 0.35 | Flat roof hidden behind a 0.6 m parapet with stone coping; roof lantern (skylight) 1.2 x 1.2 over the consulting room |
| porch | (-1.4, 4.5) -> (1.4, 6.0) | 2.8 x 1.5 | G | Two Doric columns + pilasters, stone steps (3) | 0.2 | Flat lead porch roof |

### Chimneys (count must equal HouseCatalog = 1)
| x | z | stack w x d | above ridge | pots | serves |
|---|---|---|---|---|---|
| -3.1 | 0.0 | 1.0 x 0.8 | 1.0 | 4 | drawing-room fire = THE HEARTH, back-to-back with the kitchen range |

### Exterior details
- columned porch (2 Doric columns) with a flat lead roof; 3 stone steps
- brass plate by the SURGERY door: 'DR. A. CRANE  M.B.  SURGERY 9-11'
- bell pull at the surgery door
- monkey-puzzle tree in the front garden (use a FirTree, trimmed, as stand-in)
- railings + gate; gravel drive to the surgery door

### Plot, garden and yard
- Front: 8 m garden, gravel drive, lawn gone to hay, monkey-puzzle tree
- Back: walled garden, a greenhouse frame (empty), a stone bench, a child's swing hanging from one rope
- Plot boundary (world polygon): (-76.5,57.0), (-76.5,34.0), (-52.0,34.0), (-52.0,57.0). Boundary = DryStoneWall / FencePicket on the street side, FenceRail or hedges (Shrub1-4) elsewhere, a GardenGate on every path.

## 2. Rooms

| Room | Level | Rect (x0,z0,x1,z1) | Clear inside | Kind | Floor | Walls | Ceiling | Light |
|---|---|---|---|---|---|---|---|---|
| **porch** Porch | G | (-1.4, 4.5, 1.4, 6.0) | 2.40 x 1.24 | porch | FloorCheckTile | open | CeilingInterior | pendant |
| **hall** Hall | G | (-1.2, -4.5, 1.2, 4.5) | 2.28 x 8.59 | hall | FloorCheckTile | Wallpaper2 above PanelDark dado | CeilingInterior | pendant |
| **sitting** Drawing room | G | (-5.0, 0.0, -1.2, 4.5) | 3.39 x 4.09 | living | FloorParquet | Wallpaper2C (green damask) | CeilingInterior | pendant |
| **kitchen** Kitchen | G | (-5.0, -4.5, -1.2, 0.0) | 3.39 x 4.09 | kitchen | FloorQuarryTile | TileWallWhite to 1.5 m | CeilingInterior | pendant |
| **waiting** Waiting room | G | (1.2, 0.0, 5.0, 4.5) | 3.39 x 4.09 | waiting | FloorWornTile | PlasterWorn painted institutional cream | CeilingInterior | pendant |
| **disp** Dispensary (PUZZLE) | G | (1.2, -4.5, 5.0, 0.0) | 3.39 x 4.09 | dispensary | FloorWornTile | TileWallWhite to 2.0 m | CeilingInterior | pendant |
| **consult** Consulting room | G | (5.0, -3.0, 10.0, 3.0) | 4.59 x 5.30 | surgery | FloorParquet | PanelDark to 1.2 m, PlasterWorn | CeilingInterior | roof lantern + 2 pendants |
| **bed1** Doctor's bedroom | F | (-5.0, 0.0, -1.2, 4.5) | 3.39 x 4.09 | bedroom | CarpetWorn | Wallpaper1 | CeilingInterior | pendant |
| **landing** Landing | F | (-1.2, -4.5, 1.2, 4.5) | 2.28 x 8.30 | landing | Carpet70sB | Wallpaper2 | CeilingInterior | pendant |
| **bed2** Guest bedroom | F | (1.2, 0.0, 5.0, 4.5) | 3.39 x 4.09 | bedroom | FloorOldWood1 | Wallpaper1B | CeilingInterior | pendant |
| **bath** Bathroom | F | (-5.0, -4.5, -1.2, 0.0) | 3.39 x 4.09 | bathroom | FloorBathTile | TileWallWhite, radiator | CeilingInterior | pendant |
| **bed3** The boy's room (locked by day) | F | (1.2, -4.5, 5.0, 0.0) | 3.39 x 4.09 | bedroom | FloorOldWood2 | Wallpaper1B with a height chart pencilled on the door frame | CeilingInterior | pendant |
| **cellar** Cellar | C | (-5.0, -4.5, 1.2, 4.5) | 5.50 x 8.30 | cellar | FloorConcrete | Whitewash | joists | pendant |

Skirting: 0.15 m dark oak in every room except kitchens/sculleries/cellars (none) and bathrooms (tile). Every ground-floor room has one LightSwitch by its main door at 1.3 m; every room has the existing `Room` component.

### Doors (how the rooms connect)
| Door | Level | From -> to | Wall | Centre at | W x H | Leaf | Role |
|---|---|---|---|---|---|---|---|
| d_porch | G | porch -> ext | N | 0.0 | 1.2 x 2.05 | porch opening (White) | front |
| d_front | G | porch -> hall | S | 0.0 | 1.0 x 2.3 | white 4-panel door, brass knocker, coloured-glass fanlight |  |
| d_back | G | hall -> ext | S | 0.5 | 0.9 x 2.05 | half-glazed garden door | back |
| d_sit | G | hall -> sitting | W | 2.6 | 0.9 x 2.05 | panel |  |
| d_kit | G | hall -> kitchen | W | -3.0 | 0.85 x 2.05 | panel |  |
| d_wait | G | hall -> waiting | E | 2.6 | 0.9 x 2.05 | panel |  |
| d_disp | G | hall -> disp | E | -3.0 | 0.85 x 2.05 | door with a frosted 'PRIVATE' panel |  |
| d_cons1 | G | waiting -> consult | E | 1.5 | 0.9 x 2.05 | baize-lined door |  |
| d_cons2 | G | disp -> consult | E | -1.5 | 0.8 x 2.05 | panel |  |
| d_surg | G | consult -> ext | N | 7.5 | 0.95 x 2.05 | patients' entrance, brass plate, bell pull | surgery |
| d_b1 | F | landing -> bed1 | W | 2.6 | 0.85 x 2.05 | panel |  |
| d_b2 | F | landing -> bed2 | E | 2.6 | 0.85 x 2.05 | panel |  |
| d_bath | F | landing -> bath | W | -3.0 | 0.8 x 2.05 | panel |  |
| d_b3 | F | landing -> bed3 | E | -3.0 | 0.8 x 2.05 | 4-panel, key in the lock on the landing side |  |

### Windows
| Window | Room | Wall | Centre at | W x H | Sill | Style | Weak | Curtains / shutters |
|---|---|---|---|---|---|---|---|---|
| w_sit | sitting | N | -3.1 | 1.5 x 1.9 | 0.6 | tall sash 2-over-2 |  | FabricVelvet (bottle green)  |
| w_wait | waiting | N | 3.1 | 1.5 x 1.9 | 0.6 | tall sash |  | net only  |
| w_k | kitchen | S | -3.6 | 1.1 x 1.3 | 0.9 | sash, cracked | **WEAK** (break-in point) |   |
| w_disp | disp | S | 3.1 | 1.0 x 1.2 | 0.9 | sash with an iron grille (one bar missing) | **WEAK** (break-in point) |   |
| w_cons | consult | S | 7.5 | 1.4 x 1.6 | 0.8 | sash, frosted lower half |  |   |
| w_b1 | bed1 | N | -3.1 | 1.3 x 1.6 | 0.9 | sash |  | FabricLinen  |
| w_b2 | bed2 | N | 3.1 | 1.3 x 1.6 | 0.9 | sash |  |   |
| w_bath | bath | S | -3.1 | 0.8 x 0.9 | 1.3 | frosted |  |   |
| w_b3 | bed3 | S | 3.1 | 1.1 x 1.4 | 0.9 | sash - the boy's window, with paper stars stuck on it |  |   |
| w_land | landing | N | 0.0 | 1.0 x 1.0 | 1.2 | round-headed stair window over the porch |  |   |

### Stairs, ladders and voids
| Id | In room | Rect | Goes | Up toward | Type |
|---|---|---|---|---|---|
| s1 | hall | (-1.1, -1.5, -0.2, 2.0) | G -> F | S | straight - 18 risers 0.17, mahogany handrail, brass stair rods, Carpet70sB runner |
| s1u | landing | (-1.1, -1.5, -0.2, 2.0) | G -> F | S | void |
| s2 | kitchen | (-4.6, -4.1, -3.7, -0.6) | C -> G | N | cellar stair behind a ledged door |
| s2c | cellar | (-4.6, -4.1, -3.7, -0.6) | C -> G | N | arrives |

## 3. Every prop, room by room

Position = prop **pivot at the centre of its footprint, on the floor** (or on top of its parent for `on`, or the centre height `y` for wall-hung). Yaw = the way the prop's FRONT faces. Scale the asset so its bounds fit the listed size (see ASSETS.md 'Scaling rule').

### hall - Hall
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 21 | `HallConsole` | Poly Haven `ClassicConsole_01` (1k) | 0.91 | 0.00 | 0.80 | 270 | 1.1 x 0.4 x 0.82 | floor |   |
| 22 | `Mirror` | Poly Haven `ornate_mirror_01` (1k) | 1.11 | 1.25 | 0.80 | 270 | 0.6 x 0.05 x 0.9 | wall-hung |  hall mirror |
| 23 | `FuseBox` | Poly Haven `power_box_01` (1k) | -0.60 | 1.40 | -4.08 | 0 | 0.45 x 0.15 x 0.6 | wall-hung | fuse-box FUSE BOX + a 1930s meter |
| 24 | `CoatHooks` | BUILD (PropKit) | 0.98 | 0.00 | -1.60 | 270 | 0.9 x 0.25 x 1.7 | floor |   |

### sitting - Drawing room
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 1 | `Hearth` | BUILD (PropKit) | -3.10 | 0.00 | 0.44 | 0 | 1.8 x 0.7 x 1.4 | floor | hearth white marble fire surround, brass fender, coal scuttle |
| 2 | `MantelClock` | Poly Haven `mantel_clock_01` (1k) | -3.10 | 1.40 | 0.44 | 0 | 0.3 x 0.14 x 0.32 | on H05.p001 |  stopped at 4:00 |
| 3 | `FrameStanding1` | Poly Haven `standing_picture_frame_01` (1k) | -3.80 | 1.40 | 0.44 | 0 | 0.18 x 0.08 x 0.24 | on H05.p001 | family-photo Dr Crane, wife, the boy (age 8) |
| 4 | `VaseAntique` | Poly Haven `antique_ceramic_vase_01` (1k) | -2.40 | 1.40 | 0.44 | 0 | 0.25 x 0.25 x 0.45 | on H05.p001 |   |
| 5 | `Sofa` | Poly Haven `Sofa_01` (2k) | -4.17 | 0.00 | 2.20 | 90 | 2.0 x 0.9 x 0.85 | floor |   |
| 6 | `ArmchairGreen` | Poly Haven `GreenChair_01` (2k) | -2.40 | 0.00 | 1.20 | 270 | 0.75 x 0.8 x 0.95 | floor |   |
| 7 | `CoffeeTable` | Poly Haven `CoffeeTable_01` (1k) | -3.10 | 0.00 | 2.20 | 90 | 1.1 x 0.6 x 0.45 | floor |   |
| 8 | `Rug` | BUILD (PropKit) | -3.40 | 0.00 | 2.20 | 0 | 2.0 x 1.4 x 0.01 | floor |   |
| 9 | `SideTable` | Poly Haven `side_table_01` (1k) | -1.80 | 0.00 | 3.87 | 180 | 0.5 x 0.5 x 0.6 | floor |   |
| 10 | `RotaryPhone` | Sketchfab `812e8f59f9e749fab483eabf4800dec2` (CC BY, sara El-sisi) | -1.80 | 0.60 | 3.87 | 180 | 0.22 x 0.25 x 0.14 | on H05.p009 | phone cream bakelite phone, number card 'HOLLOWMERE 14' |
| 11 | `Radio` | Poly Haven `vintage_radio_transceiver` (1k) | -1.95 | 0.60 | 3.87 | 180 | 0.45 x 0.25 x 0.3 | on H05.p009 | radio walnut radio |
| 12 | `Radiator` | BUILD (PropKit) | -1.39 | 0.00 | 0.90 | 270 | 0.9 x 0.2 x 0.7 | floor |   |
| 13 | `FrameFancy1` | Poly Haven `fancy_picture_frame_01` (1k) | -4.62 | 1.43 | 0.80 | 90 | 0.6 x 0.05 x 0.75 | wall-hung |  oil painting of the chapel |
| 14 | `TableSmall` | Poly Haven `small_wooden_table_01` (1k) | -4.20 | 0.00 | 3.87 | 180 | 0.7 x 0.5 x 0.72 | floor |  card table with a chess set |

### kitchen - Kitchen
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 15 | `Range` | BUILD (PropKit) | -3.10 | 0.00 | -0.36 | 180 | 1.0 x 0.55 x 0.85 | floor |  cream enamel range in the back-to-back flue |
| 16 | `KitchenSinkUnit` | BUILD (PropKit) | -2.30 | 0.00 | -3.82 | 0 | 1.0 x 0.6 x 0.9 | floor |   |
| 17 | `Fridge` | Sketchfab `68d173f205344455933f861042b99fe3` (CC BY, dylanheyes) | -1.60 | 0.00 | -1.20 | 270 | 0.6 x 0.62 x 1.45 | floor |   |
| 18 | `TableWood1` | Poly Haven `WoodenTable_01` (1k) | -2.50 | 0.00 | -2.20 | 90 | 1.4 x 0.8 x 0.76 | floor |   |
| 19 | `TeaSet` | Poly Haven `tea_set_01` (1k) | -2.50 | 0.76 | -2.20 | 90 | 0.45 x 0.3 x 0.2 | on H05.p018 |   |
| 20 | `ChairDining` | Poly Haven `dining_chair_02` (1k) | -3.15 | 0.00 | -2.20 | 90 | 0.45 x 0.5 x 0.95 | floor |   |

### waiting - Waiting room
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 25 | `BenchPainted` | Poly Haven `painted_wooden_bench` (1k) | 3.60 | 0.00 | 3.92 | 180 | 1.4 x 0.4 x 0.45 | floor |  waiting bench under the window (low) |
| 26 | `BenchPainted` | Poly Haven `painted_wooden_bench` (1k) | 3.10 | 0.00 | 0.29 | 0 | 1.4 x 0.4 x 0.45 | floor |   |
| 27 | `TableSmall` | Poly Haven `small_wooden_table_01` (1k) | 1.54 | 0.00 | 1.00 | 90 | 0.7 x 0.5 x 0.72 | floor |  magazines from 1974 |
| 28 | `WallClock` | Poly Haven `wall_clock` (1k) | 1.29 | 1.93 | 3.60 | 90 | 0.35 x 0.06 x 0.35 | wall-hung |   |
| 29 | `PottedPlant2` | Poly Haven `potted_plant_02` (1k) | 4.45 | 0.00 | 2.40 | 270 | 0.35 x 0.35 x 0.6 | floor |   |

### disp - Dispensary (PUZZLE)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 30 | `MedicineCabinet` | BUILD (PropKit) | 3.40 | 0.90 | -0.21 | 180 | 0.9 x 0.3 x 1.2 | wall-hung | puzzle container GLASS-FRONTED MEDICINE CABINET with a 4-dial padlock - the PART CONTAINER |
| 31 | `Counter` | BUILD (PropKit) | 3.60 | 0.00 | -3.94 | 0 | 1.44 x 0.36 x 0.57 | floor |  dispensing bench 1.44 m |
| 32 | `MedicineBottles` | BUILD (PropKit) | 3.60 | 0.57 | -3.94 | 0 | 0.6 x 0.15 x 0.2 | on H05.p031 |  12 bottles, labels data-driven |
| 33 | `BunsenBurner` | Poly Haven `bunsen_burner` (1k) | 4.10 | 0.57 | -3.94 | 0 | 0.12 x 0.12 x 0.2 | on H05.p031 |   |
| 34 | `DrawerCabinet` | Poly Haven `drawer_cabinet` (1k) | 1.51 | 0.00 | -1.60 | 90 | 0.6 x 0.45 x 1.1 | floor |  PATIENT RECORDS index drawers (4 cards = clues) |
| 35 | `SteelShelves2` | Poly Haven `steel_frame_shelves_02` (1k) | 1.80 | 0.00 | -0.29 | 180 | 1.0 x 0.4 x 1.8 | floor |  shelves of brown bottles and pill boxes |

### consult - Consulting room
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 36 | `ExamCouch` | BUILD (PropKit) | 9.27 | 0.00 | 0.00 | 270 | 1.9 x 0.7 x 0.75 | floor |   |
| 37 | `OfficeDesk` | Poly Haven `metal_office_desk` (2k) | 8.50 | 0.00 | -2.25 | 0 | 1.4 x 0.75 x 0.76 | floor |   |
| 38 | `Microscope` | Poly Haven `vintage_microscope` (1k) | 8.90 | 0.76 | -2.25 | 0 | 0.2 x 0.25 x 0.35 | on H05.p037 |   |
| 39 | `Typewriter` | Sketchfab `1d6009449fe84fd79454c4081eb743bf` (CC BY, Kuba 'Volant' Wójcik) | 8.20 | 0.76 | -2.25 | 0 | 0.4 x 0.35 x 0.2 | on H05.p037 |   |
| 40 | `Chair` | Poly Haven `WoodenChair_01` (2k) | 8.50 | 0.00 | -1.50 | 180 | 0.45 x 0.5 x 0.95 | floor |   |
| 41 | `CabinetVintage` | Poly Haven `vintage_cabinet_01` (2k) | 6.00 | 0.00 | -2.40 | 0 | 1.0 x 0.45 x 1.8 | floor |  specimen cabinet: jars, a glass eye chart |
| 42 | `WashStand` | BUILD (PropKit) | 9.20 | 0.00 | 2.40 | 180 | 0.8 x 0.45 x 0.85 | floor |   |
| 43 | `Crutches` | Poly Haven `vintage_crutches_01` (1k) | 5.17 | 0.00 | 2.40 | 90 | 0.4 x 0.15 x 1.3 | floor |   |

### bed1 - Doctor's bedroom
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 48 | `Bed` | Poly Haven `GothicBed_01` (2k) | -3.57 | 0.00 | 2.20 | 90 | 1.6 x 2.1 x 1.4 | floor |   |
| 49 | `Nightstand` | Poly Haven `ClassicNightstand_01` (1k) | -4.42 | 0.00 | 1.10 | 90 | 0.5 x 0.4 x 0.6 | floor |   |
| 50 | `Nightstand` | Poly Haven `ClassicNightstand_01` (1k) | -4.42 | 0.00 | 3.30 | 90 | 0.5 x 0.4 x 0.6 | floor |   |
| 51 | `Wardrobe` | Poly Haven `GothicCabinet_01` (2k) | -2.00 | 0.00 | 0.39 | 0 | 1.2 x 0.6 x 2.1 | floor |   |
| 52 | `Commode` | Poly Haven `GothicCommode_01` (2k) | -1.90 | 0.00 | 3.87 | 180 | 1.1 x 0.5 x 0.9 | floor |   |

### landing - Landing
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 67 | `ShrineNiche` | BUILD (PropKit) | 1.01 | 0.90 | -0.80 | 270 | 0.6 x 0.25 x 0.9 | wall-hung | shrine  |

### bed2 - Guest bedroom
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 53 | `BedIron` | Poly Haven `old_bed_frame` (2k) | 3.62 | 0.00 | 2.20 | 270 | 1.0 x 2.0 x 1.05 | floor |   |
| 54 | `Nightstand` | Poly Haven `ClassicNightstand_01` (1k) | 4.42 | 0.00 | 1.40 | 270 | 0.5 x 0.4 x 0.6 | floor |   |
| 55 | `ChestOfDrawers` | Poly Haven `vintage_wooden_drawer_01` (1k) | 3.60 | 0.00 | 0.33 | 0 | 0.9 x 0.48 x 1.0 | floor |   |

### bath - Bathroom
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 56 | `BathtubPhoto` | Sketchfab `ba60ba7d0a4a4b6899021741f450395b` (CC BY, Igrium) | -4.23 | 0.00 | -2.40 | 90 | 1.7 x 0.78 x 0.65 | floor |   |
| 57 | `BathCurtainRail` | BUILD (PropKit) | -4.00 | 0.00 | -2.40 | 90 | 1.7 x 0.8 x 2.0 | floor | allow-overlap  |
| 58 | `Sink` | Sketchfab `a278d63585494921906ab3500c421ff7` (CC BY, lagesnpiet) | -2.60 | 0.00 | -0.32 | 180 | 0.55 x 0.45 x 0.85 | floor |   |
| 59 | `Mirror` | Poly Haven `ornate_mirror_01` (1k) | -2.60 | 1.10 | -0.09 | 180 | 0.6 x 0.05 x 0.9 | wall-hung | mirror  |
| 60 | `Toilet` | Sketchfab `6ac515a1c4154db18b5b4bd0b46d6405` (CC BY, Allan-Jay Branscombe) | -3.20 | 0.00 | -3.79 | 0 | 0.4 x 0.65 x 0.8 | floor |   |
| 61 | `Radiator` | BUILD (PropKit) | -1.39 | 0.00 | -1.00 | 270 | 0.9 x 0.2 x 0.7 | floor |   |

### bed3 - The boy's room (locked by day)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 62 | `BedIron` | Poly Haven `old_bed_frame` (2k) | 3.62 | 0.00 | -2.20 | 270 | 1.0 x 2.0 x 1.05 | floor |   |
| 63 | `NightstandPainted` | Poly Haven `painted_wooden_nightstand` (1k) | 4.43 | 0.00 | -1.43 | 270 | 0.45 x 0.38 x 0.6 | floor |   |
| 64 | `RockingHorse` | Sketchfab `5168ce9d45a3401ea6fe4d92f17bb332` (CC BY, Alex Petuhov) | 2.40 | 0.00 | -1.00 | 90 | 0.9 x 0.35 x 0.7 | floor |   |
| 65 | `Globe` | Sketchfab `8bb166a68f70426390eb113ff0fd72a0` (CC BY, Allan-Jay Branscombe) | 1.49 | 0.00 | -1.00 | 90 | 0.4 x 0.4 x 0.55 | floor |   |
| 66 | `Wicker2` | Poly Haven `wicker_basket_02` (1k) | 1.80 | 0.00 | -3.92 | 0 | 0.4 x 0.4 x 0.4 | floor |  toy basket |

### cellar - Cellar
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 44 | `SteelShelves` | Poly Haven `steel_frame_shelves_01` (1k) | 0.59 | 0.00 | 2.00 | 270 | 1.2 x 0.45 x 1.9 | floor |  wine racks + preserving jars |
| 45 | `CellarCrates` | BUILD (PropKit) | -2.00 | 0.00 | 3.67 | 180 | 1.4 x 0.9 x 1.2 | floor |   |
| 46 | `BarrelWorn` | owned: His_Med_Storage_Barrel_Wood_Worn_01 (Fab) | -0.50 | 0.00 | -3.79 | 0 | 0.65 x 0.65 x 0.95 | floor |   |
| 47 | `CardboardBox` | Poly Haven `cardboard_box_01` (1k) | 0.62 | 0.00 | -2.50 | 270 | 0.5 x 0.4 x 0.4 | floor |  boxes of old surgery files |

## 4. Gameplay anchors (empty GameObjects the builder must create)
| Kind | Room | x | y | z | What |
|---|---|---|---|---|---|
| item-spot | sitting | -3.10 | 1.40 | 0.44 | drawing-room mantel |
| note-spot | kitchen | -2.50 | 0.76 | -2.10 | housekeeper's note about the boy's medicine (note 1/3) |
| item-spot | hall | 0.91 | 0.82 | 0.80 | hall console |
| puzzle | disp | 3.40 | 0.90 | -0.21 | CodeLockPuzzle: match the 12 medicine bottles to the 4 patient record cards; each card's bottle number gives one digit |
| container | disp | 3.40 | 0.90 | -0.21 | Glass eye in a velvet box on the middle shelf |
| note-spot | disp | 1.51 | 1.10 | -1.60 | patient record cards (note 2/3) |
| item-spot | disp | 3.10 | 0.57 | -3.94 | dispensing bench |
| note-spot | consult | 8.40 | 0.76 | -2.25 | Dr Crane's case notes on 'the boy' (note 3/3) |
| hiding | cellar | -2.00 | 0.00 | 3.67 | behind the crates (cellar) -> HidingSpot on H05.p045 (CellarCrates) - crouched |
| hiding | bed1 | -3.57 | 0.00 | 2.20 | under the doctor's bed -> HidingSpot on H05.p048 (Bed) - crouched |
| hiding | bed1 | -2.00 | 0.00 | 0.39 | wardrobe -> HidingSpot on H05.p051 (Wardrobe) |
| hiding | bath | -4.23 | 0.00 | -2.40 | behind the bath curtain -> HidingSpot on H05.p056 (BathtubPhoto) |
| shrine | landing | 1.01 | 0.00 | -0.80 | Rite of Return point (active only if this house is a shrine this match) |

Mapping to existing code: `item-spot` -> `House.ItemSpots` (order as listed), `note-spot` -> the 3 `Readable` notes, `puzzle` -> `PuzzleRig` anchor, `container` -> `PartContainer`, `shrine` -> `House.ShrineMarker` (+ shrine niche inside), `hiding` -> `HidingSpot`.

## 5. Puzzle - Glass eye
1. LOCK FIRST: the glass medicine cabinet - you can SEE the glass eye in its velvet box through the glass.
2. 4 patient record cards (index drawers, note 2/3) each name a symptom; 12 bottles on the bench name cures.
3. Match each card to its bottle; each bottle's shelf number is a digit; card order = dial order.
4. Wrong code: a bottle falls and smashes (breaking glass noise radius).
5. Part: Glass eye.

## 6. Night changes (DESIGN 'Night changes' - pick 2-3 per match)
- the examination couch has a body-shaped dent and the sheet is warm
- the boy's room door (locked by day) is open
- every bottle label now reads the same name
- plus the shared pool: extra person in the family photo, a door opening onto a brick wall, moved furniture, an extra hallway door, all clocks stopped at the same time, writing on a wall in a player's name

## 7. Sound
- typewriter (consulting room types by itself)
- locks
- fridgehum: dispensary fridge
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
