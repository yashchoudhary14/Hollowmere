# H12 - Postmaster's House (Dunn)

> Machine-readable twin: `docs/design/houses/H12.json` - the builder reads THAT file; this page explains it. Every number here comes from the same data and is validated (no overlaps, door clearances, required items).

| | |
|---|---|
| **Archetype** | 1920s Arts-and-Crafts brick house with a twin-gabled front and a flat-roofed sorting room |
| **Silhouette (must read at 50 m in fog)** | Two identical front gables side by side (an 'M' roof), red brick ground floor with clay TILE-HUNG upper floor, a green door in a little gabled porch, one chimney per gable end, and a flat-roofed sorting room on the east with a red pillar box and an enamel 'POST OFFICE' sign. |
| **Ritual part** | Sealed letter |
| **M1 puzzle rig (code)** | `PuzzleKind.Sequence` |
| **Letter traits (HouseCatalog)** | door **Green**, **2** chimney(s), stopped clock **3:00**, family **Dunn** |
| **World position** | x 10.8, z 78.0, floor level y +0.10, yaw 270 deg - Main Street (east side, by the square) |
| **Front door (world)** | (5.6, 77.8) - 34.7 m from the altar, 5.2 m from the lane |
| **Placement note** | IS the village post office: public counter in the sorting wing, which points north onto the square |

## Reference images

![plan](../ref/H12_plan.png)

![front-left](../ref/H12_massing_front.png) ![back-right](../ref/H12_massing_back.png)

Real-world references (look, don't copy): [Arts and Crafts movement](https://en.wikipedia.org/wiki/Arts_and_Crafts_movement), [Roof tiles](https://en.wikipedia.org/wiki/Roof_tiles)

Mood-image prompt (Higgsfield / Gemini / ChatGPT image - save the result as `docs/design/ref/H12_mood.png`):

```text
1920s Arts and Crafts brick house with two identical front gables side by side, clay tile-hung upper floor, a small timber porch with a green door, a red pillar box at the gate and an enamel POST OFFICE sign on a flat-roofed side wing, photorealistic, late 1970s rural English village, overcast autumn dusk, damp, muted greens and greys, 35mm film still, Phasmophobia / Resident Evil Village mood, no people, eye-level, 16:9
```

## 1. Structure

### Levels
| Level | Floor y (m) | Clear height (m) |
|---|---|---|
| G - Ground floor | +0.00 | 2.80 |
| F - First floor | +3.05 | 2.60 |
| A - Attic (under the roof) | +5.90 | 2.30 |

Floor build-up: 0.25 m between floors (joists + boards); ceilings use the room's `ceiling` material.

### Wings (massing)
| Wing | Rect x0,z0 -> x1,z1 | Size | Levels | Walls | Wall thickness | Roof |
|---|---|---|---|---|---|---|
| main | (-5.0, -4.0) -> (5.0, 4.0) | 10.0 x 8.0 | G, F, A | Ground floor BrickRed; first floor tile-hung (RoofClay2 used as vertical hanging tiles, with a band of fish-scale tiles) | 0.35 | M-roof: two parallel gables with ridges along z at x=-2.5 and x=+2.5, 50 deg, RoofClay2, central valley gutter |
| sorting | (5.0, -4.0) -> (9.0, 1.0) | 4.0 x 5.0 | G | BrickRed, enamel sign | 0.25 | Flat felt roof with a timber fascia |
| porch | (-1.2, 4.0) -> (0.8, 5.2) | 2.0 x 1.2 | G | Timber porch on a brick plinth | 0.1 | Small gabled porch roof, RoofClay2 |

### Chimneys (count must equal HouseCatalog = 2)
| x | z | stack w x d | above ridge | pots | serves |
|---|---|---|---|---|---|
| -5.0 | -2.0 | 0.8 x 0.8 | 1.0 | 2 | kitchen range = THE HEARTH / front room |
| 5.0 | 2.0 | 0.8 x 0.8 | 1.0 | 2 | dining-room fire |

### Exterior details
- twin front gables ('M' roof), tile-hung upper floor with a band of fish-scale tiles
- small gabled timber porch over a GREEN door with a brass letter flap
- red PILLAR BOX at the front gate; enamel 'POST OFFICE' sign over the sorting-room door
- a telegraph pole with wires to the eaves (the village exchange line)

### Plot, garden and yard
- Front: low brick wall, the pillar box, a bicycle with a postbag on the handlebars
- Back: tidy lawn gone wild, a pigeon loft (empty, door swinging)
- Plot boundary (world polygon): (20.8,72.5), (20.8,87.5), (5.1,87.5), (5.1,72.5). Boundary = DryStoneWall / FencePicket on the street side, FenceRail or hedges (Shrub1-4) elsewhere, a GardenGate on every path.

## 2. Rooms

| Room | Level | Rect (x0,z0,x1,z1) | Clear inside | Kind | Floor | Walls | Ceiling | Light |
|---|---|---|---|---|---|---|---|---|
| **porch** Porch | G | (-1.2, 4.0, 0.8, 5.2) | 1.80 x 1.04 | porch | FloorWornTile | open | CeilingInterior | pendant |
| **front** Front room | G | (-5.0, 0.0, -1.0, 4.0) | 3.59 x 3.59 | living | Carpet70s | WallpaperJacquard (tinted green) | CeilingInterior | pendant |
| **hall** Hall | G | (-1.0, -4.0, 1.0, 4.0) | 1.88 x 7.30 | hall | FloorWornTile | Wallpaper2 | CeilingInterior | pendant |
| **dining** Dining room | G | (1.0, 0.0, 5.0, 4.0) | 3.59 x 3.59 | dining | FloorParquet | Wallpaper2C | CeilingInterior | pendant |
| **kitchen** Kitchen | G | (-5.0, -4.0, -1.0, 0.0) | 3.59 x 3.59 | kitchen | FloorQuarryTile | TileWallWhite to 1.2 m | CeilingInterior | pendant |
| **office** Postmaster's office | G | (1.0, -4.0, 5.0, 0.0) | 3.88 x 3.59 | study | FloorOldWood1 | PanelDark | CeilingInterior | pendant |
| **sorting** Sorting room (PUZZLE) | G | (5.0, -4.0, 9.0, 1.0) | 3.69 x 4.50 | sorting | FloorConcrete | painted GPO cream/green | CeilingInterior | pendant |
| **bedW** Dunns' bedroom | F | (-5.0, 0.0, -1.0, 4.0) | 3.59 x 3.59 | bedroom | CarpetWorn | Wallpaper1 | CeilingInterior | pendant |
| **landing** Landing | F | (-1.0, -4.0, 1.0, 4.0) | 1.88 x 7.30 | landing | Carpet70sB | Wallpaper2 | CeilingInterior | pendant |
| **bedE** Spare bedroom | F | (1.0, 0.0, 5.0, 4.0) | 3.59 x 3.59 | bedroom | FloorOldWood1 | Wallpaper1B | CeilingInterior | pendant |
| **bath** Bathroom | F | (-5.0, -4.0, -1.0, 0.0) | 3.59 x 3.59 | bathroom | FloorBathTile | TileWallWhite (pink border) | CeilingInterior | pendant |
| **bed3** Sewing room | F | (1.0, -4.0, 5.0, 0.0) | 3.59 x 3.59 | bedroom | FloorOldWood2 | Wallpaper1 | CeilingInterior | pendant |
| **attic** Attic (dead-letter store) | A | (-5.0, -4.0, 5.0, 4.0) | 9.30 x 7.30 | attic | FloorOldWood2 | RawPlankWall | CeilingInterior | pendant |

Skirting: 0.15 m dark oak in every room except kitchens/sculleries/cellars (none) and bathrooms (tile). Every ground-floor room has one LightSwitch by its main door at 1.3 m; every room has the existing `Room` component.

### Doors (how the rooms connect)
| Door | Level | From -> to | Wall | Centre at | W x H | Leaf | Role |
|---|---|---|---|---|---|---|---|
| d_porch | G | porch -> ext | N | -0.2 | 1.0 x 2.05 | porch opening (Green) | front |
| d_front | G | porch -> hall | S | -0.4 | 0.9 x 2.05 | green door with a brass letter flap and a bell push |  |
| d_back | G | hall -> ext | S | -0.4 | 0.85 x 2.05 | panel | back |
| d_fr | G | hall -> front | W | 3.0 | 0.85 x 2.05 | panel |  |
| d_din | G | hall -> dining | E | 3.0 | 0.85 x 2.05 | panel |  |
| d_kit | G | hall -> kitchen | W | -2.0 | 0.85 x 2.05 | panel |  |
| d_off | G | hall -> office | E | -3.2 | 0.85 x 2.05 | panel |  |
| d_sort | G | office -> sorting | E | -2.0 | 0.85 x 2.05 | door with a brass 'STAFF ONLY' plate |  |
| d_post | G | sorting -> ext | N | 7.0 | 1.0 x 2.05 | public counter door (glazed) under the 'POST OFFICE' sign | post |
| d_bW | F | landing -> bedW | W | 3.0 | 0.8 x 2.05 | panel |  |
| d_bE | F | landing -> bedE | E | 3.0 | 0.8 x 2.05 | panel |  |
| d_bath | F | landing -> bath | W | -2.0 | 0.75 x 2.05 | panel |  |
| d_b3 | F | landing -> bed3 | E | -3.2 | 0.75 x 2.05 | panel |  |

### Windows
| Window | Room | Wall | Centre at | W x H | Sill | Style | Weak | Curtains / shutters |
|---|---|---|---|---|---|---|---|---|
| w_fr | front | N | -3.0 | 1.6 x 1.4 | 0.9 | leaded casement, 3 lights |  |   |
| w_din | dining | N | 3.0 | 1.6 x 1.4 | 0.9 | leaded casement, 3 lights |  |   |
| w_k | kitchen | S | -3.0 | 1.1 x 1.1 | 0.9 | casement, cracked | **WEAK** (break-in point) |   |
| w_off | office | S | 3.0 | 1.0 x 1.1 | 0.9 | casement | **WEAK** (break-in point) |   |
| w_sort | sorting | E | -1.5 | 1.4 x 1.0 | 1.2 | high sorting-room window with wired glass |  |   |
| w_bW | bedW | N | -3.0 | 1.2 x 1.2 | 0.9 | casement in the tile-hung gable |  |   |
| w_bE | bedE | N | 3.0 | 1.2 x 1.2 | 0.9 | casement in the tile-hung gable |  |   |
| w_bath | bath | S | -3.0 | 0.7 x 0.8 | 1.3 | frosted |  |   |
| w_att | attic | N | -2.5 | 0.5 x 0.5 | 1.0 | small vent window in the west gable apex |  |   |

### Stairs, ladders and voids
| Id | In room | Rect | Goes | Up toward | Type |
|---|---|---|---|---|---|
| s1 | hall | (0.0, -2.4, 0.9, 1.1) | G -> F | S | straight - 14 risers, oak |
| s1u | landing | (0.0, -2.4, 0.9, 1.1) | G -> F | S | void |
| s2 | landing | (-0.9, -3.6, -0.2, -2.8) | F -> A | N | loft ladder |

## 3. Every prop, room by room

Position = prop **pivot at the centre of its footprint, on the floor** (or on top of its parent for `on`, or the centre height `y` for wall-hung). Yaw = the way the prop's FRONT faces. Scale the asset so its bounds fit the listed size (see ASSETS.md 'Scaling rule').

### front - Front room
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 12 | `Hearth` | BUILD (PropKit) | -4.27 | 0.00 | 2.00 | 90 | 1.8 x 0.7 x 1.4 | floor | hearth-decor brick fireplace |
| 13 | `FrameStanding2` | Poly Haven `standing_picture_frame_02` (1k) | -4.27 | 1.40 | 2.00 | 90 | 0.22 x 0.08 x 0.18 | on H12.p012 | family-photo Mr Dunn in GPO uniform, 1953 |
| 14 | `MantelClock` | Poly Haven `mantel_clock_01` (1k) | -4.27 | 1.40 | 1.50 | 90 | 0.3 x 0.14 x 0.32 | on H12.p012 |  stopped at 3:00 |
| 15 | `Sofa2` | Poly Haven `sofa_02` (2k) | -3.00 | 0.00 | 0.54 | 0 | 1.9 x 0.9 x 0.85 | floor |   |
| 16 | `Armchair` | Poly Haven `ArmChair_01` (2k) | -2.60 | 0.00 | 2.00 | 200 | 0.85 x 0.85 x 0.95 | floor |   |
| 17 | `Cabinet70s` | Poly Haven `modern_wooden_cabinet` (1k) | -1.31 | 0.00 | 1.20 | 270 | 1.2 x 0.45 x 0.8 | floor |   |
| 18 | `TV` | Poly Haven `Television_01` (1k) | -1.31 | 0.80 | 1.20 | 270 | 0.7 x 0.5 x 0.55 | on H12.p017 | tv  |

### dining - Dining room
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 19 | `DiningTable` | Poly Haven `dining_table` (2k) | 3.00 | 0.00 | 2.00 | 0 | 1.8 x 0.9 x 0.76 | floor |   |
| 20 | `ChairDining` | Poly Haven `dining_chair_02` (1k) | 2.50 | 0.00 | 1.25 | 0 | 0.45 x 0.5 x 0.95 | floor |   |
| 21 | `ChairDining` | Poly Haven `dining_chair_02` (1k) | 2.50 | 0.00 | 2.75 | 180 | 0.45 x 0.5 x 0.95 | floor |   |
| 22 | `ChairDining` | Poly Haven `dining_chair_02` (1k) | 3.50 | 0.00 | 1.25 | 0 | 0.45 x 0.5 x 0.95 | floor |   |
| 23 | `ChairDining` | Poly Haven `dining_chair_02` (1k) | 3.50 | 0.00 | 2.75 | 180 | 0.45 x 0.5 x 0.95 | floor |   |

### kitchen - Kitchen
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 24 | `Hearth` | BUILD (PropKit) | -4.27 | 0.00 | -2.00 | 90 | 1.8 x 0.7 x 1.4 | floor | hearth cream Rayburn-style range = THE HEARTH |
| 25 | `KitchenSinkUnit` | BUILD (PropKit) | -3.20 | 0.00 | -0.39 | 180 | 1.0 x 0.6 x 0.9 | floor |   |
| 26 | `Fridge` | Sketchfab `68d173f205344455933f861042b99fe3` (CC BY, dylanheyes) | -1.90 | 0.00 | -0.40 | 180 | 0.6 x 0.62 x 1.45 | floor |   |
| 27 | `TablePainted` | Poly Haven `painted_wooden_table` (1k) | -2.80 | 0.00 | -2.20 | 0 | 1.2 x 0.7 x 0.76 | floor |   |
| 28 | `ChairPainted2` | Poly Haven `painted_wooden_chair_02` (1k) | -2.80 | 0.00 | -2.95 | 0 | 0.45 x 0.48 x 0.9 | floor |   |
| 29 | `FuseBox` | Poly Haven `power_box_01` (1k) | -1.14 | 1.40 | -0.80 | 270 | 0.45 x 0.15 x 0.6 | wall-hung | fuse-box  |

### office - Postmaster's office
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 7 | `OfficeDesk` | Poly Haven `metal_office_desk` (2k) | 3.20 | 0.00 | -0.41 | 180 | 1.19 x 0.64 x 0.65 | floor |   |
| 8 | `Typewriter` | Sketchfab `1d6009449fe84fd79454c4081eb743bf` (CC BY, Kuba 'Volant' Wójcik) | 3.40 | 0.65 | -0.41 | 180 | 0.4 x 0.35 x 0.2 | on H12.p007 |   |
| 9 | `RotaryPhone` | Sketchfab `812e8f59f9e749fab483eabf4800dec2` (CC BY, sara El-sisi) | 2.85 | 0.65 | -0.41 | 180 | 0.22 x 0.25 x 0.14 | on H12.p007 | phone the house/post-office phone - also the VILLAGE EXCHANGE board extension |
| 10 | `FrameHanging3` | Poly Haven `hanging_picture_frame_03` (1k) | 4.92 | 1.38 | -1.00 | 270 | 0.6 x 0.03 x 0.45 | wall-hung |  DELIVERY ROUND card in a frame (puzzle order) |
| 11 | `DrawerCabinet` | Poly Haven `drawer_cabinet` (1k) | 1.31 | 0.00 | -1.00 | 90 | 0.6 x 0.45 x 1.1 | floor |   |

### sorting - Sorting room (PUZZLE)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 1 | `Pigeonholes` | BUILD (PropKit) | 8.54 | 0.00 | -2.40 | 270 | 1.2 x 0.35 x 1.9 | floor | puzzle SORTING FRAME: 6 x 5 pigeonholes; 6 labelled with symbols for the village streets |
| 2 | `SortingTable` | BUILD (PropKit) | 6.60 | 0.00 | -1.20 | 90 | 2.0 x 0.9 x 0.9 | floor |   |
| 3 | `LetterScale` | BUILD (PropKit) | 6.60 | 0.90 | -1.20 | 90 | 0.25 x 0.15 x 0.2 | on H12.p002 |   |
| 4 | `Stapler` | Poly Haven `vintage_stapler` (1k) | 6.60 | 0.90 | -1.50 | 90 | 0.18 x 0.05 x 0.08 | on H12.p002 |   |
| 5 | `MailSacks` | BUILD (PropKit) | 8.00 | 0.00 | -3.37 | 0 | 1.0 x 0.7 x 0.6 | floor |   |
| 6 | `IronSafe` | BUILD (PropKit) | 5.39 | 0.00 | 0.00 | 90 | 0.6 x 0.6 x 0.8 | floor |  DEAD LETTERS box (iron, slot on top) = PART CONTAINER |

### bedW - Dunns' bedroom
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 30 | `Bed` | Poly Haven `GothicBed_01` (2k) | -3.57 | 0.00 | 2.00 | 90 | 1.6 x 2.1 x 1.4 | floor |   |
| 31 | `Nightstand` | Poly Haven `ClassicNightstand_01` (1k) | -4.42 | 0.00 | 0.90 | 90 | 0.5 x 0.4 x 0.6 | floor |   |
| 32 | `Nightstand` | Poly Haven `ClassicNightstand_01` (1k) | -4.42 | 0.00 | 3.10 | 90 | 0.5 x 0.4 x 0.6 | floor |   |
| 33 | `Wardrobe` | Poly Haven `GothicCabinet_01` (2k) | -2.60 | 0.00 | 0.39 | 0 | 1.2 x 0.6 x 2.1 | floor |   |

### landing - Landing
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 45 | `ShrineNiche` | BUILD (PropKit) | 0.40 | 0.90 | 3.52 | 180 | 0.6 x 0.25 x 0.9 | wall-hung | shrine  |

### bedE - Spare bedroom
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 34 | `BedIron` | Poly Haven `old_bed_frame` (2k) | 3.62 | 0.00 | 2.00 | 270 | 1.0 x 2.0 x 1.05 | floor |   |
| 35 | `Nightstand` | Poly Haven `ClassicNightstand_01` (1k) | 4.42 | 0.00 | 1.20 | 270 | 0.5 x 0.4 x 0.6 | floor |   |

### bath - Bathroom
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 39 | `Bathtub` | Sketchfab `d06c028a89a54c469e4af9cac50606fa` (CC BY, Lady Lion Studios) | -4.25 | 0.00 | -2.00 | 90 | 1.65 x 0.75 x 0.62 | floor |   |
| 40 | `Sink` | Sketchfab `a278d63585494921906ab3500c421ff7` (CC BY, lagesnpiet) | -2.00 | 0.00 | -0.32 | 180 | 0.55 x 0.45 x 0.85 | floor |   |
| 41 | `Mirror` | Poly Haven `ornate_mirror_01` (1k) | -2.00 | 1.10 | -0.09 | 180 | 0.6 x 0.05 x 0.9 | wall-hung | mirror  |
| 42 | `Toilet` | Sketchfab `6ac515a1c4154db18b5b4bd0b46d6405` (CC BY, Allan-Jay Branscombe) | -1.60 | 0.00 | -3.29 | 0 | 0.4 x 0.65 x 0.8 | floor |   |

### bed3 - Sewing room
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 36 | `SewingMachine` | Sketchfab `db6aead9ecf947bcbde1f78d3e095fbe` (CC BY, Giravolt / MEMGA) | 3.00 | 0.00 | -3.50 | 0 | 0.45 x 0.25 x 0.32 | floor |   |
| 37 | `DayBed` | Poly Haven `vintage_day_bed` (2k) | 3.00 | 0.00 | -1.06 | 180 | 0.95 x 1.95 x 0.85 | floor |   |
| 38 | `TableSmall` | Poly Haven `small_wooden_table_01` (1k) | 4.37 | 0.00 | -2.00 | 270 | 0.7 x 0.5 x 0.72 | floor |   |

### attic - Attic (dead-letter store)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 43 | `MailSacks` | BUILD (PropKit) | -4.27 | 0.00 | 0.00 | 90 | 1.0 x 0.7 x 0.6 | floor |   |
| 44 | `CardboardBox` | Poly Haven `cardboard_box_01` (1k) | 4.42 | 0.00 | 0.00 | 270 | 0.5 x 0.4 x 0.4 | floor |   |

## 4. Gameplay anchors (empty GameObjects the builder must create)
| Kind | Room | x | y | z | What |
|---|---|---|---|---|---|
| puzzle | sorting | 8.54 | 0.00 | -2.40 | SequencePuzzle reskinned: 6 undelivered letters lie on the sorting table; each address is a riddle (street + house trait). Slot them in delivery-round order (round card on the office wall); the sealed letter drops from the 'DEAD LETTERS' box |
| note-spot | sorting | 6.60 | 0.90 | -1.10 | undelivered letters bundle (note 1/3) |
| hiding | sorting | 8.00 | 0.00 | -3.37 | behind the mail sacks -> HidingSpot on H12.p005 (MailSacks) - crouched |
| container | sorting | 5.39 | 0.00 | 0.00 | The sealed letter (addressed to one of the players, in their own handwriting) |
| item-spot | office | 3.20 | 0.65 | -0.41 | office desk |
| item-spot | front | -4.27 | 1.40 | 2.00 | front-room mantel |
| item-spot | kitchen | -4.27 | 1.40 | -2.00 | range rail |
| hiding | bedW | -3.57 | 0.00 | 2.00 | under the double bed -> HidingSpot on H12.p030 (Bed) - crouched |
| hiding | bedW | -2.60 | 0.00 | 0.39 | wardrobe -> HidingSpot on H12.p033 (Wardrobe) |
| note-spot | bed3 | 4.37 | 0.72 | -2.10 | Mrs Dunn's note about 'the letter that came back' (note 2/3) |
| hiding | bath | -4.25 | 0.00 | -2.00 | behind the bath curtain -> HidingSpot on H12.p039 (Bathtub) |
| note-spot | attic | 4.42 | 0.40 | -0.10 | dead letter addressed to 'the guests' (note 3/3) |
| shrine | landing | 0.40 | 0.00 | 3.52 | Rite of Return point (active only if this house is a shrine this match) |

Mapping to existing code: `item-spot` -> `House.ItemSpots` (order as listed), `note-spot` -> the 3 `Readable` notes, `puzzle` -> `PuzzleRig` anchor, `container` -> `PartContainer`, `shrine` -> `House.ShrineMarker` (+ shrine niche inside), `hiding` -> `HidingSpot`.

## 5. Puzzle - Sealed letter
1. LOCK FIRST: the DEAD LETTERS box on the sorting-room wall has a slot and a padlock with 6 symbols.
2. 6 letters on the sorting table; each address is a riddle about a real village house (street + door colour/chimneys).
3. The delivery round card (office) gives the street order; slot letters into the 6 street pigeonholes in round order.
4. Wrong slot: the letter is 'returned' (falls out, paper noise). Part: the sealed letter.

## 6. Night changes (DESIGN 'Night changes' - pick 2-3 per match)
- one pigeonhole holds a letter addressed to a player, in their handwriting
- the exchange board in the office lights up a house number that is ringing
- the pillar box door is open and full of black feathers
- plus the shared pool: extra person in the family photo, a door opening onto a brick wall, moved furniture, an extra hallway door, all clocks stopped at the same time, writing on a wall in a player's name

## 7. Sound
- telephone: exchange buzz, rings
- paper
- typewriter
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
