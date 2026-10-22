# H01 - Rao House (Rao)

> Machine-readable twin: `docs/design/houses/H01.json` - the builder reads THAT file; this page explains it. Every number here comes from the same data and is validated (no overlaps, door clearances, required items).

| | |
|---|---|
| **Archetype** | 1930s pebbledash detached house that never opens its shutters, big walled vegetable garden |
| **Silhouette (must read at 50 m in fog)** | Hipped roof with one fat central chimney, a two-storey canted bay on the left, ALL windows shuttered (green shutters, closed), 'RAO' painted on the garden gate, a lean-to greenhouse against the back wall. |
| **Ritual part** | Family seal ring |
| **M1 puzzle rig (code)** | `PuzzleKind.CodeLock` |
| **Letter traits (HouseCatalog)** | door **Green**, **1** chimney(s), stopped clock **9:00**, family **Rao** |
| **World position** | x 36.0, z 6.0, floor level y +0.10, yaw 262 deg - The Green (east side) |
| **Front door (world)** | (31.6, 5.8) - 110.8 m from the altar, 9.8 m from the lane |
| **Placement note** | walled vegetable garden behind (east) |

## Reference images

![plan](../ref/H01_plan.png)

![front-left](../ref/H01_massing_front.png) ![back-right](../ref/H01_massing_back.png)

Real-world references (look, don't copy): [Roughcast](https://en.wikipedia.org/wiki/Roughcast), [Bay window](https://en.wikipedia.org/wiki/Bay_window)

Mood-image prompt (Higgsfield / Gemini / ChatGPT image - save the result as `docs/design/ref/H01_mood.png`):

```text
1930s detached English house finished in grey-brown pebbledash, hipped grey tile roof with one fat central chimney, two-storey canted bay window on the left, every window closed with green wooden shutters, a green garden gate with 'RAO' painted in white, overgrown walled vegetable garden, photorealistic, late 1970s rural English village, overcast autumn dusk, damp, muted greens and greys, 35mm film still, Phasmophobia / Resident Evil Village mood, no people, eye-level, 16:9
```

## 1. Structure

### Levels
| Level | Floor y (m) | Clear height (m) |
|---|---|---|
| C - Cellar | -2.40 | 2.20 |
| G - Ground floor | +0.00 | 2.70 |
| F - First floor | +2.95 | 2.50 |

Floor build-up: 0.25 m between floors (joists + boards); ceilings use the room's `ceiling` material.

### Wings (massing)
| Wing | Rect x0,z0 -> x1,z1 | Size | Levels | Walls | Wall thickness | Roof |
|---|---|---|---|---|---|---|
| main | (-4.6, -4.4) -> (4.6, 4.4) | 9.2 x 8.8 | C, G, F | Pebbledash (grey-brown) over brick; BrickRed plinth 0.5 m; white-painted window surrounds | 0.35 | Hipped roof 35 deg, RoofTilesGrey (1960s re-roof), hip ridge tiles; central chimney at (−0.6, 0.2) rising 1.2 m; 0.4 m boxed eaves with white fascia |
| bay | (-4.0, 4.4) -> (-1.0, 5.2) | 3.0 x 0.8 | G, F | Pebbledash, 3-face canted bay (shown as rect here; facets at 45 deg on the front corners) | 0.35 | Lead-covered flat top at 5.5 m |
| greenhouse | (-2.0, -6.4) -> (3.0, -4.4) | 5.0 x 2.0 | G | Lean-to greenhouse: brick base 0.6 m, timber + glass (half the panes broken) | 0.12 | Glass lean-to at 25 deg |

### Chimneys (count must equal HouseCatalog = 1)
| x | z | stack w x d | above ridge | pots | serves |
|---|---|---|---|---|---|
| -0.6 | 0.0 | 1.0 x 0.9 | 1.0 | 3 | front-room fire = THE HEARTH (back-to-back with kitchen boiler) |

### Exterior details
- all front and side windows have green timber shutters CLOSED and nailed (visible nail heads)
- 'RAO' painted in white capitals on the green garden gate (gate 1.2 wide in a 1.4 m brick wall)
- tiled porch step with a cracked boot scraper; milk bottles (3, empty) on the step
- TV aerial bent on the chimney; telephone wire from a pole to the eaves
- lean-to greenhouse on the back wall with half the panes broken

### Plot, garden and yard
- Front: 6 m garden, crazy-paving path, overgrown privet hedge 1.6 m both sides
- Back & side: WALLED VEGETABLE GARDEN 14 x 12 m (BrickRed wall 1.8 m): raised beds (VegBeds) of cabbages, bean canes, marrows gone to seed; a scarecrow-like coat on a pole; compost bins (CompostBags); wheelbarrow; a cold frame
- Plot boundary (world polygon): (59.5,-0.4), (56.8,18.6), (23.6,13.9), (26.2,-5.1). Boundary = DryStoneWall / FencePicket on the street side, FenceRail or hedges (Shrub1-4) elsewhere, a GardenGate on every path.

## 2. Rooms

| Room | Level | Rect (x0,z0,x1,z1) | Clear inside | Kind | Floor | Walls | Ceiling | Light |
|---|---|---|---|---|---|---|---|---|
| **front** Front room (sitting room) | G | (-4.6, 0.0, -0.6, 4.4) | 3.59 x 3.99 | living | Carpet70s | WallpaperQuatrefoil (brown/orange) | CeilingInterior | pendant with fringed shade |
| **bayG** Bay (front room) | G | (-4.0, 4.4, -1.0, 5.2) | 2.30 x 0.39 | living-bay | Carpet70s | WallpaperQuatrefoil | CeilingInterior | pendant |
| **hall** Hall | G | (-0.6, 0.0, 1.4, 4.4) | 1.88 x 3.99 | hall | FloorWornTile | Wallpaper2C | CeilingInterior | pendant |
| **dining** Dining room | G | (1.4, 0.0, 4.6, 4.4) | 2.79 x 3.99 | dining | FloorParquet | Wallpaper2 | CeilingInterior | pendant |
| **kitchen** Kitchen | G | (-4.6, -4.4, 1.4, 0.0) | 5.59 x 3.99 | kitchen | FloorQuarryTile | TileWallWhite to 1.2 m + PlasterWorn | CeilingInterior | pendant |
| **back** Back lobby & cellar stair | G | (1.4, -4.4, 4.6, 0.0) | 2.79 x 3.99 | utility | FloorBrick | Whitewash | CeilingInterior | pendant |
| **glass** Greenhouse | G | (-2.0, -6.4, 3.0, -4.4) | 4.76 x 1.82 | greenhouse | FloorBrick | glass | CeilingInterior | pendant |
| **bed1** Parents' bedroom | F | (-4.6, 0.0, -0.6, 4.4) | 3.59 x 3.99 | bedroom | CarpetWorn | Wallpaper1 | CeilingInterior | pendant |
| **bayF** Bay (bedroom) | F | (-4.0, 4.4, -1.0, 5.2) | 2.30 x 0.39 | bedroom-bay | CarpetWorn | Wallpaper1 | CeilingInterior | pendant |
| **landing** Landing | F | (-0.6, 0.0, 1.4, 4.4) | 1.88 x 3.99 | landing | Carpet70sB | Wallpaper2C | CeilingInterior | pendant |
| **bed2** Grandmother's bedroom | F | (1.4, 0.0, 4.6, 4.4) | 2.79 x 3.99 | bedroom | FloorOldWood1 | Wallpaper2 | CeilingInterior | pendant |
| **bed3** Box room (shrine room) | F | (1.4, -4.4, 4.6, 0.0) | 2.79 x 3.99 | bedroom | FloorOldWood2 | PlasterWorn | CeilingInterior | pendant |
| **bath** Bathroom | F | (-4.6, -4.4, 1.4, 0.0) | 5.59 x 3.99 | bathroom | FloorBathTile | TileWallWhite (avocado border tiles) | CeilingInterior | pendant |
| **cellar** Cellar | C | (-4.6, -4.4, 4.6, 4.4) | 8.50 x 8.10 | cellar | FloorConcrete | Whitewash flaking to brick | joists | one bulb on a cord, dead until the fuse box is reset |

Skirting: 0.15 m dark oak in every room except kitchens/sculleries/cellars (none) and bathrooms (tile). Every ground-floor room has one LightSwitch by its main door at 1.3 m; every room has the existing `Room` component.

### Doors (how the rooms connect)
| Door | Level | From -> to | Wall | Centre at | W x H | Leaf | Role |
|---|---|---|---|---|---|---|---|
| d_front | G | hall -> ext | N | 0.4 | 0.9 x 2.05 | half-glazed door, sunrise pattern glass (Green) - tiled porch step; garden gate 6 m in front painted 'RAO' | front |
| d_back | G | back -> ext | E | -2.2 | 0.85 x 2.05 | ledged plank, two bolts - opens to the side path | back |
| d_front_r | G | hall -> front | W | 3.4 | 0.85 x 2.05 | 4-panel |  |
| d_din | G | hall -> dining | E | 0.9 | 0.85 x 2.05 | 4-panel |  |
| d_kit | G | hall -> kitchen | S | -0.1 | 0.85 x 2.05 | 4-panel |  |
| d_backl | G | kitchen -> back | E | -2.2 | 0.85 x 2.05 | ledged |  |
| d_glass | G | kitchen -> glass | S | -0.6 | 0.8 x 2.05 | glazed door |  |
| d_b1 | F | landing -> bed1 | W | 2.6 | 0.8 x 2.05 | panel |  |
| d_b2 | F | landing -> bed2 | E | 0.8 | 0.8 x 2.05 | panel |  |
| d_bath | F | landing -> bath | S | -0.1 | 0.75 x 2.05 | panel |  |
| d_b3 | F | bed2 -> bed3 | S | 3.6 | 0.75 x 2.05 | connecting door, wallpapered over (visible seam) |  |

### Windows
| Window | Room | Wall | Centre at | W x H | Sill | Style | Weak | Curtains / shutters |
|---|---|---|---|---|---|---|---|---|
| w_bay | bayG | N | -2.5 | 2.0 x 1.5 | 0.85 | 1930s casement, leaded top-lights |  | FabricTerlenka net + heavy curtain / shutters: outside, green, CLOSED and nailed (DESIGN Rao signature) |
| w_din | dining | N | 3.0 | 1.4 x 1.3 | 0.85 | 1930s casement, leaded top-lights |  | FabricTerlenka net + heavy curtain / shutters: outside, green, CLOSED and nailed (DESIGN Rao signature) |
| w_k | kitchen | W | -2.2 | 1.1 x 1.1 | 0.85 | 1930s casement, leaded top-lights | **WEAK** (break-in point) | FabricTerlenka net + heavy curtain / shutters: outside, green, CLOSED and nailed (DESIGN Rao signature) |
| w_back | back | S | 3.4 | 0.7 x 0.9 | 0.85 | 1930s casement, leaded top-lights | **WEAK** (break-in point) | FabricTerlenka net + heavy curtain  |
| w_bayF | bayF | N | -2.5 | 2.0 x 1.3 | 0.85 | 1930s casement, leaded top-lights |  | FabricTerlenka net + heavy curtain / shutters: outside, green, CLOSED and nailed (DESIGN Rao signature) |
| w_b2 | bed2 | N | 3.0 | 1.3 x 1.2 | 0.85 | 1930s casement, leaded top-lights |  | FabricTerlenka net + heavy curtain / shutters: outside, green, CLOSED and nailed (DESIGN Rao signature) |
| w_bath | bath | S | -3.6 | 0.8 x 0.9 | 1.3 | 1930s casement, leaded top-lights |  | FabricTerlenka net + heavy curtain  |
| w_b3 | bed3 | E | -2.2 | 0.8 x 1.0 | 0.85 | 1930s casement, leaded top-lights |  | FabricTerlenka net + heavy curtain / shutters: outside, green, CLOSED and nailed (DESIGN Rao signature) |

### Stairs, ladders and voids
| Id | In room | Rect | Goes | Up toward | Type |
|---|---|---|---|---|---|
| s1 | hall | (0.4, 0.5, 1.3, 3.8) | G -> F | S | straight - 14 risers, painted balusters, Carpet70s runner |
| s1u | landing | (0.4, 0.5, 1.3, 3.8) | G -> F | S | void |
| s2 | back | (3.6, -4.0, 4.5, -0.6) | G -> C | S | steep cellar stair, 11 risers, open treads, handrail rope |
| s2c | cellar | (3.6, -4.0, 4.5, -0.6) | G -> C | S | arrives in the cellar |

## 3. Every prop, room by room

Position = prop **pivot at the centre of its footprint, on the floor** (or on top of its parent for `on`, or the centre height `y` for wall-hung). Yaw = the way the prop's FRONT faces. Scale the asset so its bounds fit the listed size (see ASSETS.md 'Scaling rule').

### front - Front room (sitting room)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 1 | `Hearth` | BUILD (PropKit) | -1.80 | 0.00 | 0.44 | 0 | 1.8 x 0.7 x 1.4 | floor | hearth tiled 1930s fire surround (beige tiles) around the open grate |
| 2 | `FrameStanding1` | Poly Haven `standing_picture_frame_01` (1k) | -2.40 | 1.40 | 0.44 | 0 | 0.18 x 0.08 x 0.24 | on H01.p001 | family-photo FAMILY PHOTO 1 (counted by the code) |
| 3 | `FrameStanding2` | Poly Haven `standing_picture_frame_02` (1k) | -1.20 | 1.40 | 0.44 | 0 | 0.22 x 0.08 x 0.18 | on H01.p001 | family-photo FAMILY PHOTO 2 (counted) |
| 4 | `MantelClock` | Poly Haven `mantel_clock_01` (1k) | -1.80 | 1.40 | 0.44 | 0 | 0.3 x 0.14 x 0.32 | on H01.p001 |  stopped at 9:00 |
| 5 | `Sofa2` | Poly Haven `sofa_02` (2k) | -3.77 | 0.00 | 2.00 | 90 | 1.9 x 0.9 x 0.85 | floor |   |
| 6 | `Armchair` | Poly Haven `ArmChair_01` (2k) | -1.60 | 0.00 | 2.20 | 225 | 0.85 x 0.85 x 0.95 | floor |   |
| 7 | `Cabinet70s` | Poly Haven `modern_wooden_cabinet` (1k) | -0.92 | 0.00 | 1.60 | 270 | 1.2 x 0.45 x 0.8 | floor |  teak sideboard |
| 8 | `TV` | Poly Haven `Television_01` (1k) | -0.92 | 0.80 | 1.60 | 270 | 0.7 x 0.5 x 0.55 | on H01.p007 | tv  |
| 9 | `CoffeeTable` | Poly Haven `CoffeeTable_01` (1k) | -2.90 | 0.00 | 2.30 | 90 | 1.1 x 0.6 x 0.45 | floor |   |
| 10 | `Rug` | BUILD (PropKit) | -2.90 | 0.00 | 2.30 | 0 | 2.0 x 1.4 x 0.01 | floor |   |
| 11 | `SideTable` | Poly Haven `side_table_01` (1k) | -4.00 | 0.00 | 0.70 | 0 | 0.5 x 0.5 x 0.6 | floor |   |
| 12 | `RotaryPhone` | Sketchfab `812e8f59f9e749fab483eabf4800dec2` (CC BY, sara El-sisi) | -4.00 | 0.60 | 0.70 | 0 | 0.22 x 0.25 x 0.14 | on H01.p011 | phone  |

### bayG - Bay (front room)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 13 | `PlanterPot` | Poly Haven `planter_pot_clay` (1k) | -2.50 | 0.00 | 4.66 | 0 | 0.35 x 0.35 x 0.3 | floor |  dead aspidistra in the bay |

### hall - Hall
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 14 | `Mirror` | Poly Haven `ornate_mirror_01` (1k) | -0.52 | 1.15 | 1.00 | 90 | 0.6 x 0.05 x 0.9 | wall-hung |  hall mirror |
| 15 | `FrameHanging3` | Poly Haven `hanging_picture_frame_03` (1k) | 1.32 | 1.38 | 1.00 | 270 | 0.6 x 0.03 x 0.45 | wall-hung | family-photo FAMILY PHOTO 3: three generations on the steps (counted) |

### dining - Dining room
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 16 | `DiningTable` | Poly Haven `dining_table` (2k) | 3.00 | 0.00 | 2.20 | 90 | 1.8 x 0.9 x 0.76 | floor |   |
| 17 | `ChairDining` | Poly Haven `dining_chair_02` (1k) | 2.25 | 0.00 | 1.70 | 90 | 0.45 x 0.5 x 0.95 | floor |   |
| 18 | `ChairDining` | Poly Haven `dining_chair_02` (1k) | 3.75 | 0.00 | 1.70 | 270 | 0.45 x 0.5 x 0.95 | floor |   |
| 19 | `ChairDining` | Poly Haven `dining_chair_02` (1k) | 2.25 | 0.00 | 2.70 | 90 | 0.45 x 0.5 x 0.95 | floor |   |
| 20 | `ChairDining` | Poly Haven `dining_chair_02` (1k) | 3.75 | 0.00 | 2.70 | 270 | 0.45 x 0.5 x 0.95 | floor |   |
| 21 | `CabinetVintage` | Poly Haven `vintage_cabinet_01` (2k) | 4.00 | 0.00 | 0.80 | 270 | 1.0 x 0.45 x 1.8 | floor |  glass-fronted china cabinet |
| 22 | `FrameFancy1` | Poly Haven `fancy_picture_frame_01` (1k) | 3.00 | 1.23 | 0.09 | 0 | 0.6 x 0.05 x 0.75 | wall-hung | family-photo FAMILY PHOTO 4: wedding (counted) |

### kitchen - Kitchen
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 23 | `Cooker` | Poly Haven `electric_stove` (2k) | -3.20 | 0.00 | -0.39 | 180 | 0.6 x 0.6 x 0.9 | floor |   |
| 24 | `KitchenSinkUnit` | BUILD (PropKit) | -2.30 | 0.00 | -0.39 | 180 | 1.0 x 0.6 x 0.9 | floor |   |
| 25 | `KitchenUnit` | BUILD (PropKit) | -1.40 | 0.00 | -0.39 | 180 | 0.6 x 0.6 x 0.9 | floor |   |
| 26 | `TablePainted` | Poly Haven `painted_wooden_table` (1k) | -1.80 | 0.00 | -2.40 | 0 | 1.2 x 0.7 x 0.76 | floor |   |
| 27 | `PotEnamel` | Poly Haven `pot_enamel_01` (1k) | -1.80 | 0.76 | -2.40 | 0 | 0.3 x 0.25 x 0.2 | on H01.p026 |   |
| 28 | `Onions` | Poly Haven `yellow_onion` (1k) | -1.50 | 0.76 | -2.40 | 0 | 0.09 x 0.09 x 0.09 | on H01.p026 |   |
| 29 | `ChairPainted1` | Poly Haven `painted_wooden_chair_01` (1k) | -1.80 | 0.00 | -3.15 | 0 | 0.45 x 0.48 x 0.9 | floor |   |
| 30 | `ChairPainted2` | Poly Haven `painted_wooden_chair_02` (1k) | -1.80 | 0.00 | -1.65 | 180 | 0.45 x 0.48 x 0.9 | floor |   |
| 31 | `Fridge` | Sketchfab `68d173f205344455933f861042b99fe3` (CC BY, dylanheyes) | 0.60 | 0.00 | -3.71 | 0 | 0.6 x 0.62 x 1.45 | floor |   |
| 32 | `CabinetPainted` | Poly Haven `painted_wooden_cabinet` (2k) | -3.97 | 0.00 | -0.80 | 90 | 1.0 x 0.5 x 1.9 | floor |  larder cupboard - walk-in hiding |

### back - Back lobby & cellar stair
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 33 | `CoatHooks` | BUILD (PropKit) | 1.58 | 0.00 | -1.00 | 90 | 0.9 x 0.25 x 1.7 | wall-hung |   |
| 34 | `Toolbox` | Poly Haven `metal_toolbox` (1k) | 2.40 | 0.00 | -0.20 | 180 | 0.5 x 0.22 x 0.25 | floor |   |

### glass - Greenhouse
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 35 | `PlanterBox1` | Poly Haven `planter_box_01` (1k) | -1.20 | 0.00 | -6.05 | 0 | 1.0 x 0.4 x 0.4 | floor |   |
| 36 | `PlanterBox1` | Poly Haven `planter_box_01` (1k) | 0.40 | 0.00 | -6.05 | 0 | 1.0 x 0.4 x 0.4 | floor |   |
| 37 | `SteelShelves2` | Poly Haven `steel_frame_shelves_02` (1k) | 2.65 | 0.00 | -5.40 | 270 | 1.0 x 0.4 x 1.8 | floor |   |
| 38 | `SeedTray` | Poly Haven `seeding_tray_01` (1k) | 2.20 | 0.90 | -5.40 | 0 | 0.5 x 0.3 x 0.08 | raised | allow-overlap  |
| 39 | `WateringCan` | Poly Haven `watering_can_metal_01` (1k) | 1.50 | 0.00 | -4.80 | 0 | 0.5 x 0.2 x 0.35 | floor |   |

### bed1 - Parents' bedroom
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 40 | `Bed` | Poly Haven `GothicBed_01` (2k) | -2.60 | 0.00 | 1.14 | 0 | 1.6 x 2.1 x 1.4 | floor |   |
| 41 | `Nightstand` | Poly Haven `ClassicNightstand_01` (1k) | -3.70 | 0.00 | 0.29 | 0 | 0.5 x 0.4 x 0.6 | floor |   |
| 42 | `Nightstand` | Poly Haven `ClassicNightstand_01` (1k) | -1.50 | 0.00 | 0.29 | 0 | 0.5 x 0.4 x 0.6 | floor |   |
| 43 | `Wardrobe` | Poly Haven `GothicCabinet_01` (2k) | -3.92 | 0.00 | 2.60 | 90 | 1.2 x 0.6 x 2.1 | floor |   |
| 44 | `Commode` | Poly Haven `GothicCommode_01` (2k) | -0.94 | 0.00 | 1.00 | 270 | 1.1 x 0.5 x 0.9 | floor |  dressing table with triple mirror |

### bed2 - Grandmother's bedroom
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 45 | `BedIron` | Poly Haven `old_bed_frame` (2k) | 3.22 | 0.00 | 2.00 | 270 | 1.0 x 2.0 x 1.05 | floor |   |
| 46 | `NightstandPainted` | Poly Haven `painted_wooden_nightstand` (1k) | 4.03 | 0.00 | 1.23 | 270 | 0.45 x 0.38 x 0.6 | floor |   |
| 47 | `SideTable` | Poly Haven `side_table_01` (1k) | 2.65 | 0.00 | 0.34 | 0 | 0.5 x 0.5 x 0.6 | floor |  washstand-height side table with a jug |
| 48 | `RockingChair` | Poly Haven `Rockingchair_01` (2k) | 2.00 | 0.00 | 3.20 | 180 | 0.6 x 0.95 x 1.05 | floor |   |

### bed3 - Box room (shrine room)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 49 | `DayBed` | Poly Haven `vintage_day_bed` (2k) | 3.00 | 0.00 | -3.04 | 0 | 0.95 x 1.95 x 0.85 | floor |   |
| 50 | `ShrineNiche` | BUILD (PropKit) | 4.12 | 0.90 | -1.00 | 270 | 0.6 x 0.25 x 0.9 | wall-hung | shrine  |
| 51 | `Suitcase` | Poly Haven `vintage_suitcase` (1k) | 2.40 | 0.00 | -0.19 | 180 | 0.65 x 0.2 x 0.45 | floor |   |

### bath - Bathroom
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 52 | `Bathtub` | Sketchfab `d06c028a89a54c469e4af9cac50606fa` (CC BY, Lady Lion Studios) | -3.85 | 0.00 | -2.20 | 90 | 1.65 x 0.75 x 0.62 | floor |   |
| 53 | `BathCurtainRail` | BUILD (PropKit) | -3.70 | 0.00 | -2.20 | 90 | 1.7 x 0.8 x 2.0 | floor | allow-overlap  |
| 54 | `Sink` | Sketchfab `a278d63585494921906ab3500c421ff7` (CC BY, lagesnpiet) | -1.00 | 0.00 | -0.32 | 180 | 0.55 x 0.45 x 0.85 | floor |   |
| 55 | `Mirror` | Poly Haven `ornate_mirror_01` (1k) | -1.00 | 1.10 | -0.09 | 180 | 0.6 x 0.05 x 0.9 | wall-hung | mirror  |
| 56 | `Toilet` | Sketchfab `6ac515a1c4154db18b5b4bd0b46d6405` (CC BY, Allan-Jay Branscombe) | 0.98 | 0.00 | -3.40 | 270 | 0.4 x 0.65 x 0.8 | floor |   |

### cellar - Cellar
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 57 | `FuseBox` | Poly Haven `power_box_01` (1k) | 2.00 | 1.20 | -3.98 | 0 | 0.45 x 0.15 x 0.6 | wall-hung | fuse-box FUSE BOX - must be reset to light the cellar (DESIGN Rao) |
| 58 | `ToolChest` | Poly Haven `metal_tool_chest` (1k) | -2.00 | 0.00 | 3.79 | 180 | 0.7 x 0.45 x 0.9 | floor | puzzle LOCKBOX: steel box bolted to the bench, 4 code dials (CodeLockPuzzle) |
| 59 | `SteelShelves` | Poly Haven `steel_frame_shelves_01` (1k) | -4.00 | 0.00 | 0.00 | 90 | 1.2 x 0.45 x 1.9 | floor |  jars of pickled lime, tins |
| 60 | `BarrelWorn` | owned: His_Med_Storage_Barrel_Wood_Worn_01 (Fab) | -3.80 | 0.00 | -3.60 | 0 | 0.65 x 0.65 x 0.95 | floor |   |
| 61 | `CellarCrates` | BUILD (PropKit) | 1.20 | 0.00 | -3.50 | 0 | 1.4 x 0.9 x 1.2 | floor |  crates with a crouch gap behind |
| 62 | `TinnedFood` | Poly Haven `russian_food_cans_01` (1k) | -1.00 | 0.00 | -3.80 | 0 | 0.3 x 0.2 x 0.12 | floor |   |
| 63 | `Chalkboard` | Poly Haven `standing_chalkboard_01` (1k) | 3.97 | 0.00 | 1.50 | 270 | 1.0 x 0.5 x 1.5 | floor |  WRITING ON THE WALL (chalk) explaining the code: which photos count |
| 64 | `ShelfRustic` | owned: Res_Rur_Furniture_Shelf_Wood_Old_01 (Fab) | 1.50 | 0.00 | 3.87 | 180 | 1.0 x 0.3 x 1.0 | floor |   |

## 4. Gameplay anchors (empty GameObjects the builder must create)
| Kind | Room | x | y | z | What |
|---|---|---|---|---|---|
| item-spot | front | -4.00 | 0.60 | 0.70 | phone table |
| note-spot | dining | 3.00 | 0.76 | 2.30 | Grandmother's letter about the cellar (note 1/3) |
| item-spot | kitchen | -1.80 | 0.76 | -2.40 | kitchen table |
| hiding | kitchen | -3.97 | 0.00 | -0.80 | larder cupboard -> HidingSpot on H01.p032 (CabinetPainted) |
| hiding | bed1 | -2.60 | 0.00 | 1.14 | under the double bed -> HidingSpot on H01.p040 (Bed) - crouched |
| hiding | bed1 | -3.92 | 0.00 | 2.60 | wardrobe (bay side) -> HidingSpot on H01.p043 (Wardrobe) |
| note-spot | bed1 | -0.94 | 0.90 | 0.90 | Mr Rao's diary: 'count only those who were here before the autumn' (note 2/3) |
| shrine | bed3 | 4.12 | 0.00 | -1.00 | shrine niche over the day-bed |
| hiding | bath | -3.85 | 0.00 | -2.20 | behind the avocado bath curtain -> HidingSpot on H01.p052 (Bathtub) |
| puzzle | cellar | -2.00 | 0.00 | 3.79 | CodeLockPuzzle: digits = people counted in 4 named family photos AS THEY WERE BY DAY |
| container | cellar | -2.00 | 0.00 | 3.79 | Lockbox holds the family seal ring |
| hiding | cellar | -3.80 | 0.00 | -3.60 | behind the barrels -> HidingSpot on H01.p060 (BarrelWorn) - crouched |
| hiding | cellar | 1.20 | 0.00 | -3.50 | behind the crates -> HidingSpot on H01.p061 (CellarCrates) - crouched |
| note-spot | cellar | 3.97 | 1.50 | 1.50 | chalk writing (note 3/3) |
| item-spot | cellar | 1.50 | 1.00 | 3.87 | cellar shelf |

Mapping to existing code: `item-spot` -> `House.ItemSpots` (order as listed), `note-spot` -> the 3 `Readable` notes, `puzzle` -> `PuzzleRig` anchor, `container` -> `PartContainer`, `shrine` -> `House.ShrineMarker` (+ shrine niche inside), `hiding` -> `HidingSpot`.

## 5. Puzzle - Family seal ring
1. LOCK FIRST: going down the cellar stair the beam of the flashlight lands on the steel lockbox with 4 dials.
2. The cellar is DARK: reset the fuse box (hold interact 2 s, small noise) - DESIGN Rao.
3. Read the chalk WRITING ON THE WALL: which photos count ('the ones on the mantel, the stairs and the wedding, in that order').
4. Each digit = number of people in that photo AS IT WAS BY DAY. At night the photos have extra people (night change) - players must remember, or have photographed them by day (DESIGN Rao twist).
5. Wrong code = clunk; 3 wrong codes in a row trips the fuse again.
6. Part: Family seal ring, in a velvet pouch in the lockbox.

## 6. Night changes (DESIGN 'Night changes' - pick 2-3 per match)
- every family photo gains one extra person (the same pale man) - DESIGN Rao twist
- the shutters are OPEN from the inside on one window
- a pot of dal is warm on the cooker
- plus the shared pool: extra person in the family photo, a door opening onto a brick wall, moved furniture, an extra hallway door, all clocks stopped at the same time, writing on a wall in a player's name

## 7. Sound
- vintageradio: radio tuned to static with a faint sitar sample bleeding through (stays in-world)
- locks: lockbox dials; fridgehum: kitchen fridge
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
