# H14 - Dollmaker's House (Quill)

> Machine-readable twin: `docs/design/houses/H14.json` - the builder reads THAT file; this page explains it. Every number here comes from the same data and is validated (no overlaps, door clearances, required items).

| | |
|---|---|
| **Archetype** | Crooked 16th-century timber-framed house, the oldest in the village, with a jettied upper floor |
| **Silhouette (must read at 50 m in fog)** | Black oak frame with ochre infill panels, the upper floor OVERHANGS the street by 0.6 m, the whole house leans ~2 deg to the east, steep mossy clay-tile roof with a single massive CENTRAL brick chimney, a bow shop window full of dolls staring out, a small blue door beside the chimney. |
| **Ritual part** | Porcelain hand |
| **M1 puzzle rig (code)** | `PuzzleKind.Sequence` |
| **Letter traits (HouseCatalog)** | door **Blue**, **1** chimney(s), stopped clock **9:00**, family **Quill** |
| **World position** | x -29.0, z -4.0, floor level y +0.00, yaw 90 deg - The Green (west side) |
| **Front door (world)** | (-25.0, -4.0) - 118.7 m from the altar, 3.7 m from the lane |
| **Placement note** | oldest house; jetty overhangs the green path; leans 2 deg east |

## Reference images

![plan](../ref/H14_plan.png)

![front-left](../ref/H14_massing_front.png) ![back-right](../ref/H14_massing_back.png)

Real-world references (look, don't copy): [Jettying](https://en.wikipedia.org/wiki/Jettying), [Timber framing](https://en.wikipedia.org/wiki/Timber_framing), [Dollhouse](https://en.wikipedia.org/wiki/Dollhouse)

Mood-image prompt (Higgsfield / Gemini / ChatGPT image - save the result as `docs/design/ref/H14_mood.png`):

```text
crooked sixteenth-century timber-framed house with black oak beams and ochre plaster infill, upper floor overhanging the street, steep mossy clay tile roof with a massive central brick chimney, a bow shop window full of antique porcelain dolls staring out, a small blue door, leaning slightly, photorealistic, late 1970s rural English village, overcast autumn dusk, damp, muted greens and greys, 35mm film still, Phasmophobia / Resident Evil Village mood, no people, eye-level, 16:9
```

## 1. Structure

### Levels
| Level | Floor y (m) | Clear height (m) |
|---|---|---|
| G - Ground floor | +0.00 | 2.50 |
| F - First floor | +2.75 | 2.40 |
| A - Attic (under the roof) | +5.40 | 2.30 |

Floor build-up: 0.25 m between floors (joists + boards); ceilings use the room's `ceiling` material.

### Wings (massing)
| Wing | Rect x0,z0 -> x1,z1 | Size | Levels | Walls | Wall thickness | Roof |
|---|---|---|---|---|---|---|
| ground | (-4.5, -4.0) -> (4.5, 4.0) | 9.0 x 8.0 | G | Oak frame (TimberBeam, black) with RenderOchre infill; BrickMixed plinth 0.4 | 0.25 | - |
| upper | (-4.5, -4.0) -> (4.5, 4.6) | 9.0 x 8.6 | F, A | Jettied: oak frame + RenderOchre infill; carved bressumer along the jetty; whole frame leans 2 deg east | 0.25 | Gable roof 55 deg, ridge along x at 9.0 m, RoofClay3 (mossy, sagging 0.15 m mid-ridge); central brick stack 1.6 x 2.0 |

### Chimneys (count must equal HouseCatalog = 1)
| x | z | stack w x d | above ridge | pots | serves |
|---|---|---|---|---|---|
| 0.0 | 0.25 | 2.0 x 2.5 | 1.2 | 3 | central stack: parlour fire = THE HEARTH, workshop fire, kitchen range |

### Exterior details
- JETTIED upper floor overhanging 0.6 m on carved brackets; carved bressumer beam
- black oak frame with ochre infill; whole house leans ~2 deg east (apply to the shell root, NOT gameplay colliders)
- BOW SHOP WINDOW full of dolls facing the street (night: some face inward)
- massive central brick chimney with 3 pots
- a doll-hand knocker on the small blue door

### Plot, garden and yard
- Front: pavement under the jetty
- Back: tiny walled garden, a well with a wooden lid, a doll-sized grave with a cross
- Plot boundary (world polygon): (-39.0,1.5), (-39.0,-9.5), (-24.1,-9.5), (-24.1,1.5). Boundary = DryStoneWall / FencePicket on the street side, FenceRail or hedges (Shrub1-4) elsewhere, a GardenGate on every path.

## 2. Rooms

| Room | Level | Rect (x0,z0,x1,z1) | Clear inside | Kind | Floor | Walls | Ceiling | Light |
|---|---|---|---|---|---|---|---|---|
| **lobby** Lobby entry | G | (-1.0, 1.5, 1.0, 4.0) | 1.88 x 2.00 | hall | FloorFlagstone | PlasterWorn | CeilingInterior | pendant |
| **workshop** Doll workshop & shop (PUZZLE) | G | (-4.5, -1.0, -1.0, 4.0) | 3.00 x 4.69 | workshop | FloorOldWood2 | PlasterWorn between black studs | CeilingInterior | 2 pendants + DeskLamp |
| **parlour** Hall-parlour | G | (1.0, -1.0, 4.5, 4.0) | 3.00 x 4.69 | living | FloorDarkPlanks | PlasterWorn between black studs | CeilingInterior | pendant |
| **kitchen** Kitchen | G | (-4.5, -4.0, 0.5, -1.0) | 4.69 x 2.50 | kitchen | FloorBrick | Whitewash | CeilingInterior | pendant |
| **stairhall** Stair hall | G | (0.5, -4.0, 4.5, -1.0) | 3.69 x 2.50 | hall | FloorFlagstone | PlasterWorn | CeilingInterior | pendant |
| **bed1** Miss Quill's chamber | F | (-4.5, -1.0, -1.0, 4.6) | 3.00 x 5.29 | bedroom | FloorOldWood1 | Wallpaper1 (peeling) | CeilingInterior | pendant |
| **closet** Closet over the lobby | F | (-1.0, 1.5, 1.0, 4.6) | 1.88 x 2.60 | closet | FloorOldWood1 | PlasterWorn | CeilingInterior | pendant |
| **bed2** Guest chamber | F | (1.0, -1.0, 4.5, 4.6) | 3.00 x 5.29 | bedroom | FloorOldWood1 | Wallpaper1B | CeilingInterior | pendant |
| **bath** Bathroom | F | (-4.5, -4.0, 0.5, -1.0) | 4.69 x 2.50 | bathroom | FloorBathTile | TileWallWhite | CeilingInterior | pendant |
| **landing** Landing | F | (0.5, -4.0, 4.5, -1.0) | 3.69 x 2.50 | landing | FloorOldWood2 | PlasterWorn | CeilingInterior | pendant |
| **attic** Attic (the doll store) | A | (-4.5, -4.0, 4.5, 4.6) | 8.50 x 8.10 | attic | FloorOldWood2 | RawPlankWall | CeilingInterior | none |

Skirting: 0.15 m dark oak in every room except kitchens/sculleries/cellars (none) and bathrooms (tile). Every ground-floor room has one LightSwitch by its main door at 1.3 m; every room has the existing `Room` component.

### Doors (how the rooms connect)
| Door | Level | From -> to | Wall | Centre at | W x H | Leaf | Role |
|---|---|---|---|---|---|---|---|
| d_front | G | lobby -> ext | N | 0.0 | 0.85 x 1.95 | low blue plank door with a doll-hand knocker (Blue) | front |
| d_back | G | kitchen -> ext | S | -2.0 | 0.8 x 1.9 | panel | back |
| d_ws | G | lobby -> workshop | W | 2.75 | 0.8 x 1.9 | panel |  |
| d_par | G | lobby -> parlour | E | 2.75 | 0.8 x 1.9 | panel |  |
| d_kit | G | workshop -> kitchen | S | -2.5 | 0.8 x 1.9 | panel |  |
| d_sh | G | parlour -> stairhall | S | 3.5 | 0.8 x 1.9 | panel |  |
| d_ksh | G | kitchen -> stairhall | E | -2.5 | 0.8 x 1.9 | panel |  |
| d_b2 | F | landing -> bed2 | N | 3.5 | 0.8 x 1.85 | panel |  |
| d_cl | F | bed2 -> closet | W | 3.0 | 0.75 x 1.85 | panel |  |
| d_b1 | F | closet -> bed1 | W | 3.0 | 0.75 x 1.85 | panel |  |
| d_bath | F | landing -> bath | W | -2.0 | 0.75 x 1.85 | panel |  |

### Windows
| Window | Room | Wall | Centre at | W x H | Sill | Style | Weak | Curtains / shutters |
|---|---|---|---|---|---|---|---|---|
| w_shop | workshop | N | -2.75 | 2.2 x 1.4 | 0.7 | bow shop window, small leaded panes, DOLLS STANDING IN IT facing the street |  |   |
| w_par | parlour | N | 2.75 | 1.2 x 1.1 | 0.9 | leaded casement |  |   |
| w_k | kitchen | S | -3.6 | 0.8 x 0.9 | 0.9 | casement, crooked frame | **WEAK** (break-in point) |   |
| w_sh | stairhall | E | -2.5 | 0.7 x 0.9 | 0.9 | casement, glass cracked | **WEAK** (break-in point) |   |
| w_b1 | bed1 | N | -2.75 | 1.4 x 1.0 | 0.9 | oriel window on the jetty |  |   |
| w_b2 | bed2 | N | 2.75 | 1.2 x 1.0 | 0.9 | casement on the jetty |  |   |
| w_cl | closet | N | 0.0 | 0.5 x 0.6 | 1.1 | tiny window |  |   |
| w_bath | bath | W | -2.5 | 0.6 x 0.7 | 1.2 | frosted |  |   |
| w_att | attic | W | 0.0 | 0.6 x 0.6 | 1.0 | gable light |  |   |

### Stairs, ladders and voids
| Id | In room | Rect | Goes | Up toward | Type |
|---|---|---|---|---|---|
| s1 | stairhall | (1.4, -3.7, 4.2, -2.8) | G -> F | E | straight - steep winder stair, 13 risers, 3 winders at the top, every tread creaks |
| s1u | landing | (1.4, -3.7, 4.2, -2.8) | G -> F | E | void |
| s2 | landing | (3.5, -2.6, 4.2, -1.9) | F -> A | N | loft ladder |

## 3. Every prop, room by room

Position = prop **pivot at the centre of its footprint, on the floor** (or on top of its parent for `on`, or the centre height `y` for wall-hung). Yaw = the way the prop's FRONT faces. Scale the asset so its bounds fit the listed size (see ASSETS.md 'Scaling rule').

### workshop - Doll workshop & shop (PUZZLE)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 9 | `Dollhouse` | BUILD (PropKit) | -2.75 | 0.00 | 1.00 | 0 | 0.75 x 0.55 x 0.8 | floor | puzzle THE DOLLHOUSE: an exact 1:12 replica of THIS house (generated from H14's own room data), front opens |
| 10 | `TableWood3` | Poly Haven `WoodenTable_03` (1k) | -3.86 | 0.00 | 1.50 | 90 | 1.6 x 0.72 x 0.62 | floor |  dollmaker's bench |
| 11 | `DollParts` | BUILD (PropKit) | -3.86 | 0.62 | 1.80 | 90 | 0.6 x 0.4 x 0.25 | on H14.p010 |   |
| 12 | `DeskLamp` | Poly Haven `desk_lamp_arm_01` (1k) | -3.86 | 0.62 | 1.00 | 90 | 0.25 x 0.4 x 0.5 | on H14.p010 |   |
| 13 | `DollPorcelain` | Sketchfab `9ce09a8a6b1b45ce90c4479b54b4e003` (CC BY, alicean) | -3.86 | 0.62 | 1.40 | 90 | 0.2 x 0.12 x 0.45 | on H14.p010 |   |
| 14 | `DollCabinet` | BUILD (PropKit) | -3.60 | 0.00 | -0.71 | 0 | 1.2 x 0.4 x 1.9 | floor |  glass cabinet of dolls = PART CONTAINER (unlocks) |
| 15 | `SewingMachine` | Sketchfab `db6aead9ecf947bcbde1f78d3e095fbe` (CC BY, Giravolt / MEMGA) | -4.09 | 0.00 | 0.40 | 90 | 0.45 x 0.25 x 0.32 | floor |   |
| 16 | `StoolWood1` | Poly Haven `wooden_stool_01` (1k) | -2.75 | 0.00 | 2.60 | 180 | 0.35 x 0.35 x 0.45 | floor |   |
| 17 | `FrameHanging2` | Poly Haven `hanging_picture_frame_02` (1k) | -1.26 | 1.40 | 3.00 | 270 | 0.5 x 0.03 x 0.4 | wall-hung |  framed doll patterns |
| 18 | `DollPolly` | Sketchfab `9ef34eb03f0a42978b3a9d20f0f8f389` (CC BY, archmage40) | -3.90 | 0.75 | 3.55 | 0 | 0.18 x 0.12 x 0.35 | raised | allow-overlap doll in the shop window |
| 19 | `DollAntique` | Sketchfab `a0a494a8c3db4cc4ad10cb433125fe78` (CC BY, nektar.green) | -1.60 | 0.75 | 3.55 | 0 | 0.18 x 0.12 x 0.4 | raised | allow-overlap doll in the shop window |

### parlour - Hall-parlour
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 1 | `Hearth` | BUILD (PropKit) | 1.63 | 0.00 | 0.25 | 90 | 1.8 x 0.7 x 1.4 | floor | hearth wide open hearth in the central stack, dogs and a spit |
| 2 | `MantelClock` | Poly Haven `mantel_clock_01` (1k) | 1.63 | 1.40 | 0.25 | 90 | 0.3 x 0.14 x 0.32 | on H14.p001 |  stopped at 9:00 |
| 3 | `FrameStanding2` | Poly Haven `standing_picture_frame_02` (1k) | 1.63 | 1.40 | -0.35 | 90 | 0.22 x 0.08 x 0.18 | on H14.p001 | family-photo Miss Quill as a girl with a doll identical to her |
| 4 | `RockingChair` | Poly Haven `Rockingchair_01` (2k) | 3.20 | 0.00 | 1.60 | 270 | 0.6 x 0.95 x 1.05 | floor |   |
| 5 | `SideTable` | Poly Haven `side_table_01` (1k) | 3.97 | 0.00 | 1.00 | 270 | 0.5 x 0.5 x 0.6 | floor |   |
| 6 | `RotaryPhone` | Sketchfab `812e8f59f9e749fab483eabf4800dec2` (CC BY, sara El-sisi) | 3.97 | 0.60 | 1.00 | 270 | 0.22 x 0.25 x 0.14 | on H14.p005 | phone  |
| 7 | `Radio` | Poly Haven `vintage_radio_transceiver` (1k) | 3.97 | 0.60 | 1.15 | 270 | 0.45 x 0.25 x 0.3 | on H14.p005 | radio radio that plays a music box tune at night |
| 8 | `Rug` | BUILD (PropKit) | 2.80 | 0.00 | 1.50 | 0 | 2.0 x 1.4 x 0.01 | floor |   |

### kitchen - Kitchen
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 20 | `Range` | BUILD (PropKit) | -0.25 | 0.00 | -1.55 | 180 | 1.0 x 0.55 x 0.85 | floor |  range on the back face of the central stack |
| 21 | `Dresser` | BUILD (PropKit) | -3.97 | 0.00 | -2.50 | 90 | 1.4 x 0.5 x 2.0 | floor |   |
| 22 | `TableRound2` | Poly Haven `round_wooden_table_02` (1k) | -2.30 | 0.00 | -2.50 | 0 | 0.9 x 0.9 x 0.75 | floor |   |
| 23 | `KitchenSinkUnit` | BUILD (PropKit) | -0.60 | 0.00 | -3.42 | 0 | 1.0 x 0.6 x 0.9 | floor |   |

### stairhall - Stair hall
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 24 | `FuseBox` | Poly Haven `power_box_01` (1k) | 1.40 | 1.40 | -1.32 | 180 | 0.45 x 0.15 x 0.6 | wall-hung | fuse-box  |
| 25 | `Mirror` | Poly Haven `ornate_mirror_01` (1k) | 2.60 | 1.15 | -1.27 | 180 | 0.6 x 0.05 x 0.9 | wall-hung |  old foxed mirror |

### bed1 - Miss Quill's chamber
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 26 | `Bed` | Poly Haven `GothicBed_01` (2k) | -3.17 | 0.00 | 1.60 | 90 | 1.6 x 2.1 x 1.4 | floor |   |
| 27 | `Nightstand` | Poly Haven `ClassicNightstand_01` (1k) | -4.02 | 0.00 | 0.50 | 90 | 0.5 x 0.4 x 0.6 | floor |   |
| 28 | `Nightstand` | Poly Haven `ClassicNightstand_01` (1k) | -4.02 | 0.00 | 2.70 | 90 | 0.5 x 0.4 x 0.6 | floor |   |
| 29 | `Wardrobe` | Poly Haven `GothicCabinet_01` (2k) | -3.60 | 0.00 | -0.61 | 0 | 1.2 x 0.6 x 2.1 | floor |   |
| 30 | `DollAntique` | Sketchfab `a0a494a8c3db4cc4ad10cb433125fe78` (CC BY, nektar.green) | -2.00 | 0.00 | 4.00 | 180 | 0.18 x 0.12 x 0.4 | floor | allow-overlap a doll on the floor by the window, facing the bed |

### closet - Closet over the lobby
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 31 | `TableSmall` | Poly Haven `small_wooden_table_01` (1k) | 0.00 | 0.00 | 2.10 | 0 | 0.49 x 0.35 x 0.5 | floor |   |

### bed2 - Guest chamber
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 32 | `BedIron` | Poly Haven `old_bed_frame` (2k) | 3.22 | 0.00 | 1.60 | 270 | 1.0 x 2.0 x 1.05 | floor |   |
| 33 | `NightstandPainted` | Poly Haven `painted_wooden_nightstand` (1k) | 4.03 | 0.00 | 2.38 | 270 | 0.45 x 0.38 x 0.6 | floor |   |

### bath - Bathroom
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 34 | `Bathtub` | Sketchfab `d06c028a89a54c469e4af9cac50606fa` (CC BY, Lady Lion Studios) | -3.00 | 0.00 | -3.40 | 0 | 1.4 x 0.64 x 0.53 | floor |   |
| 35 | `Sink` | Sketchfab `a278d63585494921906ab3500c421ff7` (CC BY, lagesnpiet) | -3.60 | 0.00 | -1.50 | 180 | 0.55 x 0.45 x 0.85 | floor |   |
| 36 | `Mirror` | Poly Haven `ornate_mirror_01` (1k) | -3.60 | 1.10 | -1.27 | 180 | 0.6 x 0.05 x 0.9 | wall-hung | mirror  |
| 37 | `Toilet` | Sketchfab `6ac515a1c4154db18b5b4bd0b46d6405` (CC BY, Allan-Jay Branscombe) | -1.00 | 0.00 | -3.40 | 0 | 0.4 x 0.65 x 0.8 | floor |   |

### landing - Landing
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 42 | `ShrineNiche` | BUILD (PropKit) | 1.00 | 0.90 | -3.62 | 0 | 0.6 x 0.25 x 0.9 | wall-hung | shrine  |

### attic - Attic (the doll store)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 38 | `DisplayShelves` | Poly Haven `wooden_display_shelves_01` (1k) | -3.00 | 0.00 | 4.12 | 180 | 1.2 x 0.4 x 1.8 | floor |  shelves of unfinished dolls |
| 39 | `DisplayShelves` | Poly Haven `wooden_display_shelves_01` (1k) | 3.00 | 0.00 | 4.12 | 180 | 1.2 x 0.4 x 1.8 | floor |   |
| 40 | `CellarCrates` | BUILD (PropKit) | -2.00 | 0.00 | -3.27 | 0 | 1.4 x 0.9 x 1.2 | floor |   |
| 41 | `ChestWorn` | owned: Res_Sto_Chest_Wood_Worn_08 (Fab Saloon Interior) | -3.97 | 0.00 | -2.00 | 90 | 0.9 x 0.5 x 0.55 | floor |   |

## 4. Gameplay anchors (empty GameObjects the builder must create)
| Kind | Room | x | y | z | What |
|---|---|---|---|---|---|
| item-spot | parlour | 1.63 | 1.40 | 0.25 | parlour mantel beam |
| puzzle | workshop | -2.75 | 0.00 | 1.00 | SequencePuzzle reskinned: 6 tiny dolls must stand in the dollhouse rooms where the people in the photograph on the workbench are standing in the REAL house tonight (moving figures/mannequins placed per match); place them room by room; wrong placement makes every doll in the shop window turn its head |
| note-spot | workshop | -3.86 | 0.62 | 1.60 | the photograph of 'who stands where' (note 1/3) |
| item-spot | workshop | -3.86 | 0.62 | 2.10 | dollmaker's bench |
| container | workshop | -3.60 | 0.00 | -0.71 | Porcelain hand, in a velvet glove |
| item-spot | kitchen | -2.30 | 0.75 | -2.50 | kitchen table |
| hiding | bed1 | -3.17 | 0.00 | 1.60 | under the four-poster-ish bed -> HidingSpot on H14.p026 (Bed) - crouched |
| hiding | bed1 | -3.60 | 0.00 | -0.61 | wardrobe -> HidingSpot on H14.p029 (Wardrobe) |
| note-spot | closet | -0.10 | 0.50 | 2.10 | Miss Quill's letter: 'they are all of us' (note 2/3) |
| hiding | bed2 | 3.22 | 0.00 | 1.60 | under the guest bed -> HidingSpot on H14.p032 (BedIron) - crouched |
| hiding | attic | -2.00 | 0.00 | -3.27 | among the doll crates -> HidingSpot on H14.p040 (CellarCrates) - crouched |
| note-spot | attic | -3.97 | 0.55 | -1.90 | list of names with a doll for each villager (note 3/3) |
| shrine | landing | 1.00 | 0.00 | -3.62 | Rite of Return point (active only if this house is a shrine this match) |

Mapping to existing code: `item-spot` -> `House.ItemSpots` (order as listed), `note-spot` -> the 3 `Readable` notes, `puzzle` -> `PuzzleRig` anchor, `container` -> `PartContainer`, `shrine` -> `House.ShrineMarker` (+ shrine niche inside), `hiding` -> `HidingSpot`.

## 5. Puzzle - Porcelain hand
1. LOCK FIRST: the dollhouse on the workshop floor - a perfect miniature of THIS house, front hinged open.
2. A photograph on the bench (note 1/3) shows people standing in rooms - per match, mannequin figures stand in the REAL house's rooms in those spots tonight (MatchPlan).
3. Place 6 tiny dolls in the dollhouse rooms matching the real figures (SequencePuzzle: rooms = symbols).
4. Wrong: every doll in the shop window turns its head toward the player (sanity hit, no noise).
5. Part: Porcelain hand (doll cabinet unlocks).

## 6. Night changes (DESIGN 'Night changes' - pick 2-3 per match)
- the dollhouse contains a tiny doll for each PLAYER, in the rooms where they actually are
- the doll on bed1's floor is now in the bed
- the shop-window dolls face inward
- plus the shared pool: extra person in the family photo, a door opening onto a brick wall, moved furniture, an extra hallway door, all clocks stopped at the same time, writing on a wall in a player's name

## 7. Sound
- musicbox
- ghostly: tiny footsteps on the attic boards
- house creaks (HouseCreak) louder than other houses
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
