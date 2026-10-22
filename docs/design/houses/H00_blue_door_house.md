# H00 - Blue Door House (Hale)

> Machine-readable twin: `docs/design/houses/H00.json` - the builder reads THAT file; this page explains it. Every number here comes from the same data and is validated (no overlaps, door clearances, required items).

| | |
|---|---|
| **Archetype** | Clockmaker's narrow Georgian brick town house, shop on the ground floor |
| **Silhouette (must read at 50 m in fog)** | Tall and narrow (7.6 m front, 3 storeys to the ridge), two chimney stacks side by side on the WEST gable, a big clock face in the front dormer, a shop window with a hanging clock sign. |
| **Ritual part** | Iron gate key |
| **M1 puzzle rig (code)** | `PuzzleKind.Clock` |
| **Letter traits (HouseCatalog)** | door **Blue**, **2** chimney(s), stopped clock **3:00**, family **Hale** |
| **World position** | x 10.6, z 52.0, floor level y +0.45, yaw 270 deg - Main Street (east side) |
| **Front door (world)** | (6.1, 54.4) - 58.0 m from the altar, 4.9 m from the lane |
| **Placement note** | pavement-line town house; floor +0.45 so 3 steps up to the blue door |

## Reference images

![plan](../ref/H00_plan.png)

![front-left](../ref/H00_massing_front.png) ![back-right](../ref/H00_massing_back.png)

Real-world references (look, don't copy): [Georgian architecture](https://en.wikipedia.org/wiki/Georgian_architecture), [Longcase clock](https://en.wikipedia.org/wiki/Longcase_clock)

Mood-image prompt (Higgsfield / Gemini / ChatGPT image - save the result as `docs/design/ref/H00_mood.png`):

```text
narrow three-storey red brick Georgian town house on a village street, a shop window with gilt lettering 'E. HALE - CLOCKS & WATCHES', a bright BLUE six-panel front door up three stone steps with black iron railings, a flat-roofed dormer holding a large stopped clock face, two chimney stacks side by side on the left gable, photorealistic, late 1970s rural English village, overcast autumn dusk, damp, muted greens and greys, 35mm film still, Phasmophobia / Resident Evil Village mood, no people, eye-level, 16:9
```

## 1. Structure

### Levels
| Level | Floor y (m) | Clear height (m) |
|---|---|---|
| G - Ground floor | +0.00 | 2.90 |
| F - First floor | +3.15 | 2.70 |
| A - Attic (under the roof) | +6.10 | 2.30 |

Floor build-up: 0.25 m between floors (joists + boards); ceilings use the room's `ceiling` material.

### Wings (massing)
| Wing | Rect x0,z0 -> x1,z1 | Size | Levels | Walls | Wall thickness | Roof |
|---|---|---|---|---|---|---|
| main | (-3.8, -5.5) -> (3.8, 4.5) | 7.6 x 10.0 | G, F, A | BrickRed front (Flemish bond, dark headers), BrickMixed sides/back; SandstoneBlocks sills, lintels and a 0.3 m plinth course | 0.35 | Gable roof, ridge along x at 9.6 m, 42 deg, RoofSlateGrey with lead ridge; front flat-roofed dormer 2.2 w x 1.6 h holding a 0.9 m clock face (stopped at 3:00, same as the street clock) with a small window below it; 0.25 m eaves overhang; cast-iron gutters (GreenRust) |

### Chimneys (count must equal HouseCatalog = 2)
| x | z | stack w x d | above ridge | pots | serves |
|---|---|---|---|---|---|
| -3.8 | 2.0 | 0.8 x 0.8 | 1.0 | 2 | shop fireplace (bricked up) / bed1 |
| -3.8 | -2.6 | 0.8 x 0.8 | 1.0 | 2 | kitchen range = THE HEARTH / bed2 |

### Exterior details
- 3 stone steps up to the door; black iron railings with spear finials both sides
- fanlight over the door (semicircular, radial glazing bars)
- hanging bracket sign: a gilt clock silhouette 0.6 m, creaks in wind
- the street clock (existing StoppedClock) on the wall right of the door, stopped at 3:00
- dormer clock face 0.9 m dia (cream with black numerals) - also stopped at 3:00
- cast-iron downpipes at both front corners; one has come away at the top and drips
- nameplate 'HALE' (existing Nameplate) left of the door

### Plot, garden and yard
- Front: no garden - house sits on the pavement line; 1 m strip of cobbles (Paving) with a boot scraper
- Back: walled yard 7.6 x 8 m (DryStoneWall 1.8 high), outside WC shed (BrickMixed, CorrugatedRust roof), dustbin (TrashCan), washing line with 3 grey sheets, a broken bicycle wheel (WheelRim), a gate to Back Lane
- Plot boundary (world polygon): (25.1,47.9), (25.1,56.1), (5.1,56.1), (5.1,47.9). Boundary = DryStoneWall / FencePicket on the street side, FenceRail or hedges (Shrub1-4) elsewhere, a GardenGate on every path.

## 2. Rooms

| Room | Level | Rect (x0,z0,x1,z1) | Clear inside | Kind | Floor | Walls | Ceiling | Light |
|---|---|---|---|---|---|---|---|---|
| **shop** Clock shop | G | (-3.8, 0, 1.8, 4.5) | 5.19 x 4.09 | shop | FloorDarkPlanks | PanelDark to 1.0 m, Wallpaper2C above | CeilingInterior | 2 brass pendants |
| **hall** Hall & stair | G | (1.8, -5.5, 3.8, 4.5) | 1.59 x 9.30 | hall | FloorWornTile | Wallpaper2 above PanelDark dado | CeilingInterior | 1 pendant + fanlight |
| **kitchen** Kitchen | G | (-3.8, -5.5, -0.6, 0) | 2.79 x 5.09 | kitchen | FloorQuarryTile | Whitewash | CeilingInterior | 1 pull-chain bulb (PullChainSocket) |
| **parlour** Back parlour | G | (-0.6, -5.5, 1.8, 0) | 2.28 x 5.09 | living | Carpet70sB | WallpaperJacquard (tinted mustard) | CeilingInterior | FloorLampOld + pendant |
| **bed1** Front bedroom (the Hales') | F | (-3.8, 0, 1.8, 4.5) | 5.19 x 4.09 | bedroom | CarpetWorn | Wallpaper1 | CeilingInterior | pendant |
| **landing** Landing | F | (1.8, -5.5, 3.8, 4.5) | 1.59 x 9.30 | landing | FloorOldWood1 | Wallpaper2 | CeilingInterior | pendant |
| **bed2** Child's bedroom | F | (-3.8, -5.5, -0.6, 0) | 2.79 x 5.09 | bedroom | FloorOldWood2 | Wallpaper1B (faded blue) | CeilingInterior | pendant |
| **lobby** Back lobby | F | (-0.6, -2.2, 1.8, 0) | 2.28 x 2.08 | lobby | FloorOldWood1 | Wallpaper2 | CeilingInterior | pendant |
| **bath** Bathroom | F | (-0.6, -5.5, 1.8, -2.2) | 2.28 x 2.89 | bathroom | FloorBathTile | TileWallWhite to 1.4 m, PlasterWorn above | CeilingInterior | pendant |
| **workshop** Clock workshop (attic) | A | (-3.8, -5.5, 3.8, 4.5) | 6.90 x 9.30 | attic | FloorOldWood2 | RawPlankWall (rafters exposed), knee walls 1.1 m at z=+-3.4 | exposed rafters + CeilingBeam | 2 CagedLight + DeskLamp on bench |

Skirting: 0.15 m dark oak in every room except kitchens/sculleries/cellars (none) and bathrooms (tile). Every ground-floor room has one LightSwitch by its main door at 1.3 m; every room has the existing `Room` component.

### Doors (how the rooms connect)
| Door | Level | From -> to | Wall | Centre at | W x H | Leaf | Role |
|---|---|---|---|---|---|---|---|
| d_front | G | hall -> ext | N | 2.35 | 0.95 x 2.15 | 6-panel door + fanlight (Blue) - brass knocker, letterbox, 3 stone steps up (house floor +0.45 above street), black iron railings each side | front |
| d_back | G | hall -> ext | S | 2.4 | 0.85 x 2.05 | ledged plank (WoodRoughPine) - to the back yard | back |
| d_shop | G | hall -> shop | W | 2.6 | 0.9 x 2.05 | glazed half-door with shop bell |  |
| d_parl | G | hall -> parlour | W | -1.8 | 0.85 x 2.05 | 4-panel |  |
| d_kit | G | parlour -> kitchen | W | -4.4 | 0.85 x 2.05 | ledged plank |  |
| d_bed1 | F | landing -> bed1 | W | 2.6 | 0.8 x 2.05 | panel |  |
| d_lobby | F | landing -> lobby | W | -1.1 | 0.8 x 2.05 | 4-panel |  |
| d_bed2 | F | lobby -> bed2 | W | -1.1 | 0.8 x 2.05 | 4-panel, painted pale blue |  |
| d_bath | F | lobby -> bath | S | 0.6 | 0.75 x 2.05 | 4-panel, bolt inside |  |

### Windows
| Window | Room | Wall | Centre at | W x H | Sill | Style | Weak | Curtains / shutters |
|---|---|---|---|---|---|---|---|---|
| w_shop | shop | N | -1.0 | 2.6 x 1.7 | 0.55 | shop window, 6 panes, gilt lettering 'E. HALE - CLOCKS & WATCHES' |  | none  |
| w_k | kitchen | S | -2.6 | 1.0 x 1.1 | 0.9 | sash, cracked lower pane, rotten frame | **WEAK** (break-in point) | FabricGingham cafe curtain  |
| w_p | parlour | S | 0.6 | 1.0 x 1.3 | 0.9 | sash, loose putty | **WEAK** (break-in point) | FabricVelvet (ochre)  |
| w_b1 | bed1 | N | -1.0 | 1.2 x 1.45 | 0.9 | sash 6-over-6 |  | FabricTerlenka (orange/brown 1970s)  |
| w_l | landing | N | 2.8 | 0.7 x 1.2 | 0.9 | narrow sash over the front door |  |   |
| w_b2 | bed2 | S | -2.2 | 1.0 x 1.2 | 0.9 | sash |  | FabricLinen  |
| w_bath | bath | S | 0.6 | 0.7 x 0.8 | 1.3 | frosted top-hung |  |   |
| w_dorm | workshop | N | -1.0 | 0.9 x 0.7 | 0.9 | small window under the dormer clock face |  |   |
| w_gable | workshop | E | 0.0 | 0.6 x 0.6 | 1.2 | round gable window (oculus) |  |   |

### Stairs, ladders and voids
| Id | In room | Rect | Goes | Up toward | Type |
|---|---|---|---|---|---|
| s1 | hall | (2.55, -0.3, 3.45, 3.2) | G -> F | S | straight - 15 risers 0.193, oak, turned balusters, carpet runner (Carpet70s) held by brass rods; void above in landing |
| s2 | landing | (2.55, -0.3, 3.45, 3.2) | G -> F | S | stairwell void (balustrade on W side) |
| s3 | landing | (1.9, -4.9, 2.7, -3.7) | F -> A | S | folding loft ladder through 0.7x1.0 hatch |

## 3. Every prop, room by room

Position = prop **pivot at the centre of its footprint, on the floor** (or on top of its parent for `on`, or the centre height `y` for wall-hung). Yaw = the way the prop's FRONT faces. Scale the asset so its bounds fit the listed size (see ASSETS.md 'Scaling rule').

### shop - Clock shop
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 1 | `Counter` | BUILD (PropKit) | -1.20 | 0.00 | 0.39 | 0 | 2.4 x 0.6 x 0.95 | floor |  mahogany shop counter, glass top showing 6 pocket watches |
| 2 | `PocketWatch` | Poly Haven `pocket_watch` (1k) | -1.70 | 0.95 | 0.39 | 0 | 0.06 x 0.02 x 0.08 | on H00.p001 |   |
| 3 | `PocketWatch` | Poly Haven `pocket_watch` (1k) | -1.40 | 0.95 | 0.39 | 0 | 0.06 x 0.02 x 0.08 | on H00.p001 |   |
| 4 | `Magnifier` | Poly Haven `magnifying_glass_01` (1k) | -0.90 | 0.95 | 0.39 | 0 | 0.25 x 0.1 x 0.03 | on H00.p001 |   |
| 5 | `Lighter` | Poly Haven `vintage_lighter` (1k) | -0.50 | 0.95 | 0.39 | 0 | 0.04 x 0.02 x 0.06 | on H00.p001 |  brass desk bell substitute |
| 6 | `WallClock` | Poly Haven `wall_clock` (1k) | -3.42 | 1.73 | 0.90 | 90 | 0.35 x 0.06 x 0.35 | wall-hung |  wall clock 1, each stopped at a different time |
| 7 | `WallClock` | Poly Haven `wall_clock` (1k) | -3.42 | 1.73 | 2.10 | 90 | 0.35 x 0.06 x 0.35 | wall-hung |  wall clock 2, each stopped at a different time |
| 8 | `WallClock` | Poly Haven `wall_clock` (1k) | -3.42 | 1.73 | 3.30 | 90 | 0.35 x 0.06 x 0.35 | wall-hung |  wall clock 3, each stopped at a different time |
| 9 | `GrandfatherClock` | Poly Haven `vintage_grandfather_clock_01` (2k) | -3.26 | 0.00 | 2.70 | 90 | 0.55 x 0.32 x 2.05 | floor |  display longcase clock (not the puzzle one), pendulum still |
| 10 | `DisplayShelves` | Poly Haven `wooden_display_shelves_01` (1k) | -3.22 | 0.00 | 1.00 | 90 | 1.2 x 0.4 x 1.8 | floor |  shelves of mantel clocks and alarm clocks |
| 11 | `MantelClock` | Poly Haven `mantel_clock_01` (1k) | -3.00 | 1.20 | 0.90 | 90 | 0.3 x 0.14 x 0.32 | raised |  on shelf |
| 12 | `AlarmClock` | Poly Haven `alarm_clock_01` (1k) | -3.00 | 0.75 | 1.20 | 90 | 0.14 x 0.08 x 0.16 | raised |  on shelf |
| 13 | `TableSmall` | Poly Haven `small_wooden_table_01` (1k) | -1.00 | 0.00 | 2.60 | 0 | 0.7 x 0.5 x 0.72 | floor |  window display table |
| 14 | `MantelClock` | Poly Haven `mantel_clock_01` (1k) | -1.00 | 0.72 | 2.60 | 0 | 0.3 x 0.14 x 0.32 | raised |  display clock on the table |
| 15 | `Rug` | BUILD (PropKit) | -1.00 | 0.00 | 2.00 | 0 | 2.0 x 1.4 x 0.01 | floor |  worn runner, Cloth |
| 16 | `PictureFrame` | owned: Urb_Dec_PictureFrame_Wood_Worn_01 (Fab) | 0.90 | 1.48 | 0.07 | 0 | 0.35 x 0.03 x 0.45 | wall-hung | family-photo FAMILY PHOTO: Edwin Hale, wife, small girl in front of this shop (count of people changes at night) |

### hall - Hall & stair
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 17 | `Mirror` | Poly Haven `ornate_mirror_01` (1k) | 1.89 | 1.15 | 3.60 | 90 | 0.6 x 0.05 x 0.9 | wall-hung |  hall mirror over coat hooks (late mirror event candidate) |
| 18 | `CoatHooks` | BUILD (PropKit) | 2.02 | 0.00 | 1.50 | 90 | 0.9 x 0.25 x 1.7 | floor |   |
| 19 | `HallConsole` | Poly Haven `ClassicConsole_01` (1k) | 2.09 | 0.00 | -3.60 | 90 | 1.1 x 0.4 x 0.82 | floor |   |
| 20 | `RotaryPhone` | Sketchfab `812e8f59f9e749fab483eabf4800dec2` (CC BY, sara El-sisi) | 2.09 | 0.82 | -3.40 | 90 | 0.22 x 0.25 x 0.14 | on H00.p019 | phone the house phone (DESIGN 'The phone') |
| 21 | `FuseBox` | Poly Haven `power_box_01` (1k) | 3.20 | 1.30 | -5.08 | 0 | 0.45 x 0.15 x 0.6 | wall-hung | fuse-box FUSE BOX by the back door |
| 22 | `UnderStairCupboard` | BUILD (PropKit) | 3.00 | 0.00 | 0.45 | 270 | 1.0 x 0.9 x 1.6 | floor | allow-overlap under-stair cupboard (on the stair footprint, below the flight) |

### kitchen - Kitchen
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 23 | `Hearth` | BUILD (PropKit) | -3.07 | 0.00 | -2.60 | 90 | 1.8 x 0.7 x 1.4 | floor | hearth black-leaded RANGE set in a 1.8 m brick opening; mantel shelf with tea caddies |
| 24 | `MantelClock` | Poly Haven `mantel_clock_01` (1k) | -3.07 | 1.40 | -3.00 | 90 | 0.3 x 0.14 x 0.32 | on H00.p023 |  stopped at 3:00 |
| 25 | `Candlestick` | Poly Haven `wooden_candlestick` (1k) | -3.07 | 1.40 | -2.00 | 90 | 0.1 x 0.1 x 0.25 | on H00.p023 |   |
| 26 | `Dresser` | BUILD (PropKit) | -2.00 | 0.00 | -0.34 | 180 | 1.4 x 0.5 x 2.0 | floor |  Welsh dresser with willow-pattern plates |
| 27 | `KitchenSinkUnit` | BUILD (PropKit) | -2.60 | 0.00 | -4.82 | 0 | 1.0 x 0.6 x 0.9 | floor |   |
| 28 | `Kettle` | Poly Haven `vintage_electric_kettle` (1k) | -2.30 | 0.90 | -4.82 | 0 | 0.25 x 0.18 x 0.25 | on H00.p027 |   |
| 29 | `TableWood1` | Poly Haven `WoodenTable_01` (1k) | -1.60 | 0.00 | -2.40 | 90 | 1.4 x 0.8 x 0.76 | floor |   |
| 30 | `TeaSet` | Poly Haven `tea_set_01` (1k) | -1.60 | 0.76 | -2.40 | 90 | 0.45 x 0.3 x 0.2 | on H00.p029 |   |
| 31 | `OilLamp` | Poly Haven `vintage_oil_lamp` (1k) | -1.60 | 0.76 | -2.70 | 90 | 0.18 x 0.18 x 0.45 | on H00.p029 |   |
| 32 | `ChairDining` | Poly Haven `dining_chair_02` (1k) | -2.30 | 0.00 | -2.40 | 90 | 0.45 x 0.5 x 0.95 | floor |   |
| 33 | `ChairDining` | Poly Haven `dining_chair_02` (1k) | -0.90 | 0.00 | -2.90 | 270 | 0.45 x 0.5 x 0.95 | floor |   |
| 34 | `FridgeSmall` | Sketchfab `7d12f05773a6403c9ca259ed43604c69` (CC BY, Chernorot Bogdan) | -0.98 | 0.00 | -0.36 | 270 | 0.55 x 0.58 x 1.2 | floor |   |
| 35 | `Broom` | Poly Haven `wooden_broom` (1k) | -0.85 | 0.00 | -5.05 | 0 | 0.3 x 0.1 x 1.3 | floor | allow-overlap  |

### parlour - Back parlour
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 36 | `Armchair` | Poly Haven `ArmChair_01` (2k) | 1.28 | 0.00 | -4.00 | 270 | 0.85 x 0.85 x 0.95 | floor |   |
| 37 | `Sofa3` | Poly Haven `sofa_03` (2k) | 0.60 | 0.00 | -0.52 | 180 | 1.8 x 0.85 x 0.8 | floor |  facing the TV |
| 38 | `TV2` | Poly Haven `television_02` (1k) | 0.60 | 0.00 | -4.89 | 0 | 0.6 x 0.45 x 0.5 | floor | tv TV with rabbit-ear aerial on the floor-standing cabinet |
| 39 | `CoffeeTableRound` | Poly Haven `coffee_table_round_01` (1k) | 0.50 | 0.00 | -2.80 | 0 | 0.8 x 0.8 x 0.45 | floor |   |
| 40 | `SideTableTall` | Poly Haven `side_table_tall_01` (1k) | 1.49 | 0.00 | -3.00 | 270 | 0.45 x 0.45 x 0.85 | floor |   |
| 41 | `Radio` | Poly Haven `vintage_radio_transceiver` (1k) | 1.49 | 0.85 | -3.00 | 270 | 0.45 x 0.25 x 0.3 | on H00.p040 | radio radio: static rises when the Hollow is near |
| 42 | `FrameHanging1` | Poly Haven `hanging_picture_frame_01` (1k) | -0.53 | 1.35 | -1.00 | 90 | 0.4 x 0.03 x 0.5 | wall-hung | family-photo wedding photograph |
| 43 | `WallClock` | Poly Haven `wall_clock` (1k) | 0.60 | 1.93 | -0.09 | 180 | 0.35 x 0.06 x 0.35 | wall-hung |   |

### bed1 - Front bedroom (the Hales')
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 44 | `Bed` | Poly Haven `GothicBed_01` (2k) | -2.37 | 0.00 | 2.20 | 90 | 1.6 x 2.1 x 1.4 | floor |   |
| 45 | `Nightstand` | Poly Haven `ClassicNightstand_01` (1k) | -3.22 | 0.00 | 1.10 | 90 | 0.5 x 0.4 x 0.6 | floor |   |
| 46 | `Nightstand` | Poly Haven `ClassicNightstand_01` (1k) | -3.22 | 0.00 | 3.30 | 90 | 0.5 x 0.4 x 0.6 | floor |   |
| 47 | `Wardrobe` | Poly Haven `GothicCabinet_01` (2k) | -0.60 | 0.00 | 0.39 | 0 | 1.2 x 0.6 x 2.1 | floor |   |
| 48 | `ChestOfDrawers` | Poly Haven `vintage_wooden_drawer_01` (1k) | 1.47 | 0.00 | 1.20 | 270 | 0.9 x 0.48 x 1.0 | floor |   |
| 49 | `Rug` | BUILD (PropKit) | -1.00 | 0.00 | 2.20 | 0 | 2.0 x 1.4 x 0.01 | floor |   |
| 50 | `FrameHanging2` | Poly Haven `hanging_picture_frame_02` (1k) | 1.73 | 1.40 | 3.30 | 270 | 0.5 x 0.03 x 0.4 | wall-hung | family-photo the daughter's school photo |

### landing - Landing
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 77 | `ShrineNiche` | BUILD (PropKit) | 3.33 | 0.90 | -4.20 | 270 | 0.6 x 0.25 x 0.9 | wall-hung | shrine  |

### bed2 - Child's bedroom
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 51 | `BedIron` | Poly Haven `old_bed_frame` (2k) | -2.42 | 0.00 | -3.00 | 90 | 1.0 x 2.0 x 1.05 | floor |   |
| 52 | `NightstandPainted` | Poly Haven `painted_wooden_nightstand` (1k) | -3.23 | 0.00 | -3.77 | 90 | 0.45 x 0.38 x 0.6 | floor |   |
| 53 | `NightstandPainted` | Poly Haven `painted_wooden_nightstand` (1k) | -3.23 | 0.00 | -2.23 | 90 | 0.45 x 0.38 x 0.6 | floor |   |
| 54 | `RockingHorse` | Sketchfab `5168ce9d45a3401ea6fe4d92f17bb332` (CC BY, Alex Petuhov) | -2.97 | 0.00 | -0.27 | 180 | 0.9 x 0.35 x 0.7 | floor |   |
| 55 | `ChestOfDrawers` | Poly Haven `vintage_wooden_drawer_01` (1k) | -3.18 | 0.00 | -1.50 | 90 | 0.9 x 0.48 x 1.0 | floor |  drawers lined with newspaper from 1971 |
| 56 | `TableSmall` | Poly Haven `small_wooden_table_01` (1k) | -1.60 | 0.00 | -4.60 | 0 | 0.7 x 0.5 x 0.72 | floor |   |

### lobby - Back lobby
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 63 | `FrameHanging3` | Poly Haven `hanging_picture_frame_03` (1k) | 0.60 | 1.38 | -0.07 | 180 | 0.6 x 0.03 x 0.45 | wall-hung |  framed clock advert |

### bath - Bathroom
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 57 | `Bathtub` | Sketchfab `d06c028a89a54c469e4af9cac50606fa` (CC BY, Lady Lion Studios) | 1.33 | 0.00 | -4.20 | 270 | 1.65 x 0.75 x 0.62 | floor |   |
| 58 | `BathCurtainRail` | BUILD (PropKit) | 1.30 | 0.00 | -4.20 | 270 | 1.7 x 0.8 x 2.0 | floor | allow-overlap  |
| 59 | `Sink` | Sketchfab `a278d63585494921906ab3500c421ff7` (CC BY, lagesnpiet) | -0.28 | 0.00 | -4.60 | 90 | 0.55 x 0.45 x 0.85 | floor |   |
| 60 | `Mirror` | Poly Haven `ornate_mirror_01` (1k) | -0.52 | 1.10 | -4.60 | 90 | 0.6 x 0.05 x 0.9 | wall-hung | mirror BATHROOM MIRROR (mirror events) |
| 61 | `Toilet` | Sketchfab `6ac515a1c4154db18b5b4bd0b46d6405` (CC BY, Allan-Jay Branscombe) | 0.35 | 0.00 | -4.79 | 0 | 0.4 x 0.65 x 0.8 | floor |   |
| 62 | `MedicalBox` | Poly Haven `medical_box` (1k) | -0.40 | 1.40 | -3.40 | 90 | 0.35 x 0.2 x 0.25 | raised | allow-overlap wall cabinet: bandage spawn |

### workshop - Clock workshop (attic)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 64 | `GrandfatherClock` | Poly Haven `vintage_grandfather_clock_01` (2k) | 0.00 | 0.00 | -4.96 | 0 | 0.55 x 0.32 x 2.05 | floor | puzzle THE PUZZLE CLOCK (existing ClockPuzzle rig): case 0.55x0.32x2.05; winding keyhole 1.38 m; part chest is INSIDE the trunk door |
| 65 | `TableWood3` | Poly Haven `WoodenTable_03` (1k) | -1.00 | 0.00 | 3.67 | 180 | 2.0 x 0.9 x 0.78 | floor |  clockmaker's bench under the dormer, 2.0 m |
| 66 | `DeskLamp` | Poly Haven `desk_lamp_arm_01` (1k) | -1.80 | 0.78 | 3.67 | 180 | 0.25 x 0.4 x 0.5 | on H00.p065 |   |
| 67 | `Magnifier` | Poly Haven `magnifying_glass_01` (1k) | -0.80 | 0.78 | 3.67 | 180 | 0.25 x 0.1 x 0.03 | on H00.p065 |   |
| 68 | `Pliers` | Poly Haven `pliers` (1k) | -1.30 | 0.78 | 3.67 | 180 | 0.2 x 0.05 x 0.02 | on H00.p065 |   |
| 69 | `BenchVice` | Poly Haven `bench_vice_01` (1k) | -0.20 | 0.78 | 3.67 | 180 | 0.3 x 0.2 x 0.2 | on H00.p065 |   |
| 70 | `StoolWood1` | Poly Haven `wooden_stool_01` (1k) | -1.00 | 0.00 | 2.80 | 0 | 0.35 x 0.35 x 0.45 | floor |   |
| 71 | `GrandfatherClock` | Poly Haven `vintage_grandfather_clock_01` (2k) | -3.26 | 0.00 | -2.00 | 90 | 0.55 x 0.32 x 2.05 | floor |  unfinished longcase case, no movement |
| 72 | `GrandfatherClock` | Poly Haven `vintage_grandfather_clock_01` (2k) | -3.26 | 0.00 | -1.30 | 90 | 0.55 x 0.32 x 2.05 | floor |  unfinished longcase case, no movement |
| 73 | `ChestWorn` | owned: Res_Sto_Chest_Wood_Worn_08 (Fab Saloon Interior) | 3.17 | 0.00 | 2.00 | 270 | 0.9 x 0.5 x 0.55 | floor |  PART CONTAINER skin: brass-bound clockmaker's chest |
| 74 | `WallClock` | Poly Haven `wall_clock` (1k) | -3.42 | 1.32 | 1.00 | 90 | 0.35 x 0.06 x 0.35 | wall-hung |   |
| 75 | `MetalRack` | Poly Haven `worn_metal_rack` (1k) | 3.22 | 0.00 | -2.20 | 270 | 1.0 x 0.4 x 1.7 | floor |  spare clock parts in trays |
| 76 | `CellarCrates` | BUILD (PropKit) | 2.97 | 0.00 | -4.40 | 270 | 1.4 x 0.9 x 1.2 | floor |   |

## 4. Gameplay anchors (empty GameObjects the builder must create)
| Kind | Room | x | y | z | What |
|---|---|---|---|---|---|
| note-spot | shop | -1.30 | 0.95 | 0.39 | price ledger page (note 1/3) |
| item-spot | hall | 2.09 | 0.82 | -3.60 | hall table (beside the phone) |
| hiding | hall | 3.00 | 0.00 | 0.30 | UnderStairCupboard: door at x=2.55 facing W, void under the top of the stair |
| item-spot | kitchen | -3.07 | 1.40 | -2.60 | kitchen mantel |
| note-spot | kitchen | -1.60 | 0.76 | -2.30 | Mrs Hale's shopping list (note 2/3) |
| hiding | bed1 | -2.37 | 0.00 | 2.20 | under the big bed -> HidingSpot on H00.p044 (Bed) - crouched |
| hiding | bed1 | -0.60 | 0.00 | 0.39 | wardrobe -> HidingSpot on H00.p047 (Wardrobe) |
| hiding | bed2 | -2.42 | 0.00 | -3.00 | under the iron child's bed -> HidingSpot on H00.p051 (BedIron) - crouched |
| note-spot | bed2 | -1.70 | 0.72 | -4.60 | CHILD'S DRAWING: blue door, clock showing the puzzle time (note 3/3, drives the Clock puzzle) |
| hiding | bath | 1.33 | 0.00 | -4.20 | behind the bath curtain -> HidingSpot on H00.p057 (Bathtub) |
| puzzle | workshop | 0.00 | 0.00 | -4.96 | ClockPuzzle: wind with the WindingKey (spawns on one ItemSpot), set the hour shown in the child's drawing |
| item-spot | workshop | -1.20 | 0.78 | 3.67 | workbench (between the tools) |
| container | workshop | 3.17 | 0.00 | 2.00 | PartContainer -> Iron gate key (or inside the clock trunk door: Claude Code picks the chest for M1 compatibility) |
| hiding | workshop | 2.97 | 0.00 | -4.40 | behind the crate stack under the eaves -> HidingSpot on H00.p076 (CellarCrates) - crouched |
| shrine | landing | 3.33 | 0.00 | -4.20 | shrine niche on the landing wall |

Mapping to existing code: `item-spot` -> `House.ItemSpots` (order as listed), `note-spot` -> the 3 `Readable` notes, `puzzle` -> `PuzzleRig` anchor, `container` -> `PartContainer`, `shrine` -> `House.ShrineMarker` (+ shrine niche inside), `hiding` -> `HidingSpot`.

## 5. Puzzle - Iron gate key
1. LOCK FIRST: the attic clock is the first thing you see when your head comes up through the hatch - lit by the dormer window.
2. Find the WINDING KEY: MatchPlan picks one of the 3 item spots (hall table, kitchen mantel, workbench).
3. Read the CHILD'S DRAWING (bed2, note 3/3): a blue door and a clock showing the match's hour (MatchPlan solution).
4. Insert key at the keyhole (1.38 m); each turn of the hour hand = ClockFace interact; a wrong hour + 'try' = clunk (NoiseRadius.PuzzleClunk).
5. Correct hour: the trunk door swings open; the clock CHIMES 12 TIMES (DESIGN twist) - NoiseRadius.LoudPuzzle; barricade first.
6. Part: Iron gate key, hung on a hook inside the trunk.

## 6. Night changes (DESIGN 'Night changes' - pick 2-3 per match)
- all 21 shop clocks stop at the same minute, then start ticking backwards one by one
- the rocking horse in bed2 is rocking when you enter
- a second, identical grandfather clock appears on the landing (gone when you look back)
- plus the shared pool: extra person in the family photo, a door opening onto a brick wall, moved furniture, an extra hallway door, all clocks stopped at the same time, writing on a wall in a player's name

## 7. Sound
- clocks (344 Audio - Antique Clocks): 3D ticking from every clock, slightly out of sync; silence when the Hollow is within 15 m
- locks: winding-key ratchet; trunk-door latch
- bells: the 12 chimes use clocks/chime samples pitched down 2 semitones
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
