# H02 - Widow's House (Marsh)

> Machine-readable twin: `docs/design/houses/H02.json` - the builder reads THAT file; this page explains it. Every number here comes from the same data and is validated (no overlaps, door clearances, required items).

| | |
|---|---|
| **Archetype** | Low thatched cob cottage, 1.5 storeys, with a deep front porch |
| **Silhouette (must read at 50 m in fog)** | Long, low and lumpy: thatch sweeping to 2.2 m, two eyebrow dormers, one huge stone chimney at the EAST end, a timber porch with a rocking chair, black ribbon on the black door, ONE lit window. |
| **Ritual part** | Hearth ember |
| **M1 puzzle rig (code)** | `PuzzleKind.Sequence` |
| **Letter traits (HouseCatalog)** | door **Black**, **1** chimney(s), stopped clock **6:00**, family **Marsh** |
| **World position** | x 25.5, z -90.0, floor level y -0.30, yaw 270 deg - Main Street south (east side, by the stream bridge) |
| **Front door (world)** | (20.7, -90.0) - 203.1 m from the altar, 6.4 m from the lane |
| **Placement note** | isolated thatched cottage; porch faces the lane |

## Reference images

![plan](../ref/H02_plan.png)

![front-left](../ref/H02_massing_front.png) ![back-right](../ref/H02_massing_back.png)

Real-world references (look, don't copy): [Cob (material)](https://en.wikipedia.org/wiki/Cob_(material)), [Thatching](https://en.wikipedia.org/wiki/Thatching), [Inglenook](https://en.wikipedia.org/wiki/Inglenook)

Mood-image prompt (Higgsfield / Gemini / ChatGPT image - save the result as `docs/design/ref/H02_mood.png`):

```text
small low thatched cob cottage with lumpy whitewashed walls, deep thatch sweeping almost to the ground, two eyebrow dormers, a huge stone chimney on the end gable, a wooden porch with an empty rocking chair, a black door with a black mourning ribbon, a single candle-lit window, photorealistic, late 1970s rural English village, overcast autumn dusk, damp, muted greens and greys, 35mm film still, Phasmophobia / Resident Evil Village mood, no people, eye-level, 16:9
```

## 1. Structure

### Levels
| Level | Floor y (m) | Clear height (m) |
|---|---|---|
| G - Ground floor | +0.00 | 2.40 |
| F - First floor | +2.65 | 2.20 |

Floor build-up: 0.25 m between floors (joists + boards); ceilings use the room's `ceiling` material.

### Wings (massing)
| Wing | Rect x0,z0 -> x1,z1 | Size | Levels | Walls | Wall thickness | Roof |
|---|---|---|---|---|---|---|
| main | (-6.0, -3.0) -> (6.0, 3.0) | 12.0 x 6.0 | G, F | Cob (thick, rounded corners) limewashed off-white (RenderCracked), tarred base 0.4 m | 0.6 | Thatch 50 deg, 0.35 m thick, eaves at 2.6 m, ridge 6.3 m with a patterned block-cut ridge (ThatchReed); two eyebrow dormers over the bedrooms; huge external stone stack on the east gable |
| porch | (-1.6, 3.0) -> (1.6, 4.8) | 3.2 x 1.8 | G | Open timber porch on 4 posts, low wall each side (Stone 0.5 h) | 0.1 | Mono-pitch thatch continuation |
| scullery | (-6.0, -5.2) -> (-1.6, -3.0) | 4.4 x 2.2 | G | Brick lean-to (BrickPaintedWorn) | 0.25 | CorrugatedRust lean-to roof |

### Chimneys (count must equal HouseCatalog = 1)
| x | z | stack w x d | above ridge | pots | serves |
|---|---|---|---|---|---|
| 6.0 | 0.0 | 1.6 x 1.4 | 0.8 | 1 | inglenook = THE HEARTH |

### Exterior details
- black RIBBON bow nailed to the door at 1.6 m (DESIGN signature)
- ROCKING CHAIR on the porch, facing the lane (DESIGN signature)
- ONE LIT WINDOW: candle on the living-room sill (w_lit), even by day
- thatch: 0.35 m thick, mossy, a rope net over the ridge, chicken-wire patch on the north slope
- cob walls bulge; limewash flaking to brown earth at the base; tarred plinth

### Plot, garden and yard
- Front: cottage garden 5 m deep, rosemary and lavender gone woody, a crooked gate, a water butt
- Back: a woodpile (DeadTrunk slices + Axe in a stump), a privy shed, hens' coop (empty), a path to the stream
- Plot boundary (world polygon): (38.7,-99.0), (38.7,-81.0), (16.7,-81.0), (16.7,-99.0). Boundary = DryStoneWall / FencePicket on the street side, FenceRail or hedges (Shrub1-4) elsewhere, a GardenGate on every path.

## 2. Rooms

| Room | Level | Rect (x0,z0,x1,z1) | Clear inside | Kind | Floor | Walls | Ceiling | Light |
|---|---|---|---|---|---|---|---|---|
| **porch** Porch | G | (-1.6, 3.0, 1.6, 4.8) | 3.00 x 1.64 | porch | FloorFlagstone | open | CeilingInterior | pendant |
| **living** Living room (the hearth room) | G | (0.8, -3.0, 6.0, 3.0) | 4.54 x 4.80 | living | FloorFlagstone | Whitewash with smoke staining, CeilingBeam every 0.9 m | low beamed ceiling 2.3 m | NO electric light: oil lamps only (Room.Light = lamp) |
| **kitchen** Kitchen | G | (-2.8, -3.0, 0.8, 3.0) | 3.48 x 4.80 | kitchen | FloorBrick | Whitewash | CeilingInterior | pendant |
| **bedG** Downstairs bedroom (the widow's) | G | (-6.0, -0.2, -2.8, 3.0) | 2.54 x 2.54 | bedroom | FloorOldWood1 | Wallpaper1B (faded) | CeilingInterior | pendant |
| **bathG** Bathroom (converted pantry) | G | (-6.0, -3.0, -2.8, -0.2) | 2.54 x 2.68 | bathroom | FloorBathTile | TileWallWhite | CeilingInterior | pendant |
| **scull** Scullery | G | (-6.0, -5.2, -1.6, -3.0) | 3.90 x 1.89 | utility | FloorBrick | BrickPaintedWorn | CeilingInterior | pendant |
| **loft** Loft bedroom (son's, unused) | F | (-3.4, -3.0, 6.0, 3.0) | 8.74 x 4.80 | bedroom | FloorOldWood2 | PlasterWorn on sloping ceiling | CeilingInterior | pendant - usable width shrinks to 3.6 m under the thatch |
| **store** Loft store (attic) | F | (-6.0, -3.0, -3.4, 3.0) | 1.94 x 4.80 | attic | FloorOldWood2 | RawPlankWall | CeilingInterior | pendant |

Skirting: 0.15 m dark oak in every room except kitchens/sculleries/cellars (none) and bathrooms (tile). Every ground-floor room has one LightSwitch by its main door at 1.3 m; every room has the existing `Room` component.

### Doors (how the rooms connect)
| Door | Level | From -> to | Wall | Centre at | W x H | Leaf | Role |
|---|---|---|---|---|---|---|---|
| d_front | G | porch -> kitchen | S | 0.0 | 0.85 x 2.05 | ledged black door, BLACK RIBBON bow nailed at 1.6 m | front-inner |
| d_porch | G | porch -> ext | N | 0.0 | 1.2 x 2.05 | open porch entrance (no leaf) (Black) - the porch is the threshold; the ribboned door is the real front door | front |
| d_back | G | scull -> ext | S | -4.0 | 0.8 x 2.05 | ledged plank, latch | back |
| d_liv | G | kitchen -> living | E | 1.2 | 0.85 x 1.85 | ledged, 1.85 m high (duck!) |  |
| d_bed | G | kitchen -> bedG | W | 0.3 | 0.8 x 1.9 | panel |  |
| d_bath | G | kitchen -> bathG | W | -1.6 | 0.75 x 1.9 | panel |  |
| d_scull | G | kitchen -> scull | S | -2.3 | 0.8 x 1.9 | panel |  |
| d_store | F | loft -> store | W | 1.5 | 0.7 x 1.6 | low plank door |  |

### Windows
| Window | Room | Wall | Centre at | W x H | Sill | Style | Weak | Curtains / shutters |
|---|---|---|---|---|---|---|---|---|
| w_lit | living | N | 3.4 | 1.0 x 0.9 | 0.8 | small casement - THE SINGLE LIT WINDOW (candle on the sill) |  | FabricLinen half-drawn  |
| w_k | kitchen | N | -2.2 | 0.9 x 0.9 | 0.9 | casement, rotten frame | **WEAK** (break-in point) |   |
| w_liv2 | living | S | 3.4 | 0.9 x 0.9 | 0.9 | casement, cracked pane | **WEAK** (break-in point) |   |
| w_bed | bedG | N | -4.7 | 0.8 x 0.9 | 0.9 | casement |  | FabricLinen  |
| w_liv3 | living | E | -1.5 | 0.6 x 0.6 | 1.0 | tiny window beside the chimney breast |  |   |
| w_dorm1 | loft | N | 2.5 | 0.9 x 0.6 | 0.4 | eyebrow dormer (in thatch) |  |   |
| w_dorm2 | loft | S | -0.8 | 0.9 x 0.6 | 0.4 | eyebrow dormer (back) |  |   |
| w_sc | scull | W | -4.1 | 0.7 x 0.7 | 0.9 | scullery window |  |   |

### Stairs, ladders and voids
| Id | In room | Rect | Goes | Up toward | Type |
|---|---|---|---|---|---|
| s1 | kitchen | (-0.1, -2.9, 0.75, 0.2) | G -> F | S | steep boxed winder-less stair behind a ledged door - 12 risers 0.22 - it creaks loudly (NoiseSystem on use) |
| s1u | loft | (-0.1, -2.9, 0.75, 0.2) | G -> F | S | void |

## 3. Every prop, room by room

Position = prop **pivot at the centre of its footprint, on the floor** (or on top of its parent for `on`, or the centre height `y` for wall-hung). Yaw = the way the prop's FRONT faces. Scale the asset so its bounds fit the listed size (see ASSETS.md 'Scaling rule').

### living - Living room (the hearth room)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 1 | `Hearth` | BUILD (PropKit) | 5.02 | 0.00 | 0.00 | 270 | 1.8 x 0.7 x 1.4 | floor | hearth puzzle INGLENOOK 2.4 w: stone surround, bressumer beam, bread oven door on the left, fire burns LOW and GREY (puzzle state) |
| 2 | `Candleholders` | Poly Haven `brass_candleholders` (1k) | 5.02 | 1.40 | -0.60 | 270 | 0.35 x 0.12 x 0.3 | on H02.p001 |   |
| 3 | `FrameStanding2` | Poly Haven `standing_picture_frame_02` (1k) | 5.02 | 1.40 | 0.60 | 270 | 0.22 x 0.08 x 0.18 | on H02.p001 | family-photo photo of Mr Marsh (died last autumn) |
| 4 | `RockingChair` | Poly Haven `Rockingchair_01` (2k) | 4.20 | 0.00 | 1.40 | 250 | 0.6 x 0.95 x 1.05 | floor |   |
| 5 | `Armchair` | Poly Haven `ArmChair_01` (2k) | 3.40 | 0.00 | -1.00 | 300 | 0.85 x 0.85 x 0.95 | floor |   |
| 6 | `ShelvesPainted` | Poly Haven `painted_wooden_shelves` (1k) | 1.60 | 0.00 | 2.22 | 180 | 0.9 x 0.3 x 1.6 | floor |  shelf of Bibles and jam jars |
| 7 | `TableRound` | Poly Haven `round_wooden_table_01` (1k) | 2.40 | 0.00 | -0.40 | 0 | 1.0 x 1.0 x 0.75 | floor |   |
| 8 | `OilLamp` | Poly Haven `vintage_oil_lamp` (1k) | 2.40 | 0.75 | -0.40 | 0 | 0.18 x 0.18 x 0.45 | on H02.p007 | lamp  |
| 9 | `Radio` | Poly Haven `vintage_radio_transceiver` (1k) | 2.70 | 0.75 | -0.40 | 0 | 0.45 x 0.25 x 0.3 | on H02.p007 | radio valve radio, plays the shipping forecast by day |
| 10 | `RotaryPhone` | Sketchfab `812e8f59f9e749fab483eabf4800dec2` (CC BY, sara El-sisi) | 2.10 | 0.75 | -0.40 | 0 | 0.22 x 0.25 x 0.14 | on H02.p007 | phone black bakelite phone - the widow never answers it |
| 11 | `Dresser` | BUILD (PropKit) | 1.60 | 0.00 | -2.12 | 0 | 1.4 x 0.5 x 2.0 | floor |  oak dresser with lustre jugs |
| 12 | `SewingMachine` | Sketchfab `db6aead9ecf947bcbde1f78d3e095fbe` (CC BY, Giravolt / MEMGA) | 1.01 | 0.00 | -1.40 | 90 | 0.45 x 0.25 x 0.32 | floor |   |
| 13 | `Rug` | BUILD (PropKit) | 3.60 | 0.00 | 0.00 | 0 | 2.0 x 1.4 x 0.01 | floor |   |
| 14 | `WallClock` | Poly Haven `wall_clock` (1k) | 0.89 | 1.52 | -2.20 | 90 | 0.35 x 0.06 x 0.35 | wall-hung |  stopped at 6:00 |

### kitchen - Kitchen
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 15 | `TableWood2` | Poly Haven `WoodenTable_02` (1k) | -1.40 | 0.00 | -0.60 | 90 | 1.6 x 0.85 x 0.76 | floor |   |
| 16 | `BowlWood` | Poly Haven `wooden_bowl_01` (1k) | -1.40 | 0.76 | -0.60 | 90 | 0.25 x 0.25 x 0.1 | on H02.p015 |   |
| 17 | `Candlestick` | Poly Haven `wooden_candlestick` (1k) | -1.40 | 0.76 | -1.00 | 90 | 0.1 x 0.1 x 0.25 | on H02.p015 |   |
| 18 | `ChairGallinera` | Poly Haven `gallinera_chair` (1k) | -2.15 | 0.00 | -0.60 | 90 | 0.45 x 0.5 x 0.9 | floor |   |
| 19 | `KitchenSinkUnit` | BUILD (PropKit) | -0.90 | 0.00 | -2.07 | 0 | 1.0 x 0.6 x 0.9 | floor |   |

### bedG - Downstairs bedroom (the widow's)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 20 | `BedIron` | Poly Haven `old_bed_frame` (2k) | -4.37 | 0.00 | 1.40 | 90 | 1.0 x 2.0 x 1.05 | floor |   |
| 21 | `NightstandPainted` | Poly Haven `painted_wooden_nightstand` (1k) | -5.18 | 0.00 | 2.17 | 90 | 0.45 x 0.38 x 0.6 | floor |   |
| 22 | `Wardrobe` | Poly Haven `GothicCabinet_01` (2k) | -4.70 | 0.00 | 0.19 | 0 | 1.2 x 0.6 x 2.1 | floor |   |
| 23 | `FrameHanging1` | Poly Haven `hanging_picture_frame_01` (1k) | -2.88 | 1.35 | 2.40 | 270 | 0.4 x 0.03 x 0.5 | wall-hung | family-photo WEDDING PHOTOGRAPH (the one the fire wants; in the cross-house variant it is MISSING and lives in another house) |
| 24 | `Suitcase` | Poly Haven `vintage_suitcase` (1k) | -4.20 | 0.00 | 0.60 | 0 | 0.65 x 0.2 x 0.45 | floor |   |

### bathG - Bathroom (converted pantry)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 25 | `Bathtub` | Sketchfab `d06c028a89a54c469e4af9cac50606fa` (CC BY, Lady Lion Studios) | -4.70 | 0.00 | -2.59 | 0 | 1.4 x 0.64 x 0.53 | floor |   |
| 26 | `Sink` | Sketchfab `a278d63585494921906ab3500c421ff7` (CC BY, lagesnpiet) | -4.90 | 0.00 | -0.52 | 180 | 0.55 x 0.45 x 0.85 | floor |   |
| 27 | `Mirror` | Poly Haven `ornate_mirror_01` (1k) | -4.90 | 1.05 | -0.28 | 180 | 0.6 x 0.05 x 0.9 | wall-hung | mirror  |
| 28 | `Toilet` | Sketchfab `6ac515a1c4154db18b5b4bd0b46d6405` (CC BY, Allan-Jay Branscombe) | -5.04 | 0.00 | -1.20 | 90 | 0.4 x 0.65 x 0.8 | floor |   |

### scull - Scullery
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 29 | `Copper` | BUILD (PropKit) | -5.20 | 0.00 | -4.52 | 0 | 0.8 x 0.8 x 0.9 | floor |   |
| 30 | `Mangle` | BUILD (PropKit) | -2.10 | 0.00 | -4.40 | 270 | 0.8 x 0.45 x 1.3 | floor |   |
| 31 | `FuseBox` | Poly Haven `power_box_01` (1k) | -5.30 | 1.30 | -3.13 | 180 | 0.45 x 0.15 x 0.6 | wall-hung | fuse-box FUSE BOX (the only electric circuit: kitchen + bedroom) |

### loft - Loft bedroom (son's, unused)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 32 | `BedIron` | Poly Haven `old_bed_frame` (2k) | 3.40 | 0.00 | -1.37 | 0 | 1.0 x 2.0 x 1.05 | floor |   |
| 33 | `NightstandPainted` | Poly Haven `painted_wooden_nightstand` (1k) | 4.17 | 0.00 | -2.18 | 0 | 0.45 x 0.38 x 0.6 | floor |   |
| 34 | `ChestOfDrawers` | Poly Haven `vintage_wooden_drawer_01` (1k) | 4.00 | 0.00 | 2.13 | 180 | 0.9 x 0.48 x 1.0 | floor |   |
| 35 | `Suitcase` | Poly Haven `vintage_suitcase` (1k) | 1.40 | 0.00 | -1.00 | 0 | 0.65 x 0.2 x 0.45 | floor |   |
| 36 | `ShrineNiche` | BUILD (PropKit) | 5.28 | 0.90 | 0.80 | 270 | 0.6 x 0.25 x 0.9 | wall-hung | shrine  |

### store - Loft store (attic)
| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |
|---|---|---|---|---|---|---|---|---|---|
| 37 | `CellarCrates` | BUILD (PropKit) | -4.92 | 0.00 | 0.00 | 90 | 1.4 x 0.9 x 1.2 | floor |   |
| 38 | `ChestWorn` | owned: Res_Sto_Chest_Wood_Worn_08 (Fab Saloon Interior) | -4.80 | 0.00 | 2.12 | 180 | 0.9 x 0.5 x 0.55 | floor |  his sea chest: keepsakes for the puzzle |

## 4. Gameplay anchors (empty GameObjects the builder must create)
| Kind | Room | x | y | z | What |
|---|---|---|---|---|---|
| puzzle | living | 5.02 | 0.00 | 0.00 | Widow's puzzle (SequencePuzzle until the bespoke rig exists): feed the right keepsakes to the fire in order; final item = the widow's wedding photograph -> fire turns orange -> ember forms |
| container | living | 5.02 | 0.00 | 0.00 | The ember forms in the grate; it must be carried out in the oil lantern (no chest) |
| item-spot | living | 5.02 | 1.40 | 0.00 | mantel beam |
| item-spot | living | 1.60 | 2.00 | -2.12 | dresser shelf |
| note-spot | living | 2.30 | 0.75 | -0.40 | the widow's last letter: which keepsakes 'he loved' (note 1/3) |
| note-spot | kitchen | -1.40 | 0.76 | -0.50 | recipe card with the black-ribbon pattern (note 2/3) |
| hiding | bedG | -4.37 | 0.00 | 1.40 | under the iron bed -> HidingSpot on H02.p020 (BedIron) - crouched |
| hiding | bedG | -4.70 | 0.00 | 0.19 | wardrobe -> HidingSpot on H02.p022 (Wardrobe) |
| item-spot | bedG | -4.20 | 0.45 | 0.60 | suitcase under the window |
| hiding | bathG | -4.70 | 0.00 | -2.59 | behind the bath (narrow) -> HidingSpot on H02.p025 (Bathtub) - crouched |
| hiding | loft | 3.40 | 0.00 | -1.37 | under the son's bed -> HidingSpot on H02.p032 (BedIron) - crouched |
| shrine | loft | 5.28 | 0.00 | 0.80 | niche in the chimney breast (loft) |
| hiding | store | -4.92 | 0.00 | 0.00 | behind the stacked trunks -> HidingSpot on H02.p037 (CellarCrates) - crouched |
| note-spot | store | -4.70 | 0.55 | 2.12 | his letters (note 3/3) |

Mapping to existing code: `item-spot` -> `House.ItemSpots` (order as listed), `note-spot` -> the 3 `Readable` notes, `puzzle` -> `PuzzleRig` anchor, `container` -> `PartContainer`, `shrine` -> `House.ShrineMarker` (+ shrine niche inside), `hiding` -> `HidingSpot`.

## 5. Puzzle - Hearth ember
1. LOCK FIRST: the inglenook fire burns LOW and GREY - the room is cold even with the fire 'lit'.
2. The widow's letter (note 1/3) lists keepsakes 'he loved': his pipe, his medal, a seashell, his letters (sea chest, attic store).
3. Feed the keepsakes to the fire in the order of the letter (SequencePuzzle: 6 symbols, 4 correct).
4. The last item must be the WEDDING PHOTOGRAPH (bedroom wall). Cross-house variant: the photo is in another house (DESIGN).
5. Correct: the fire roars orange, an EMBER forms; carry it out in the OIL LANTERN (store item) - no chest.
6. TWIST (DESIGN): taking the ember lets the fire die - the Widow's House loses hearth protection for the rest of the night.

## 6. Night changes (DESIGN 'Night changes' - pick 2-3 per match)
- the rocking chair on the porch rocks when nobody is near it
- the black ribbon is on the INSIDE of the door
- a second place is set at the kitchen table
- plus the shared pool: extra person in the family photo, a door opening onto a brick wall, moved furniture, an extra hallway door, all clocks stopped at the same time, writing on a wall in a player's name

## 7. Sound
- campfire: low crackle; becomes a roar when solved
- chimney: wind moaning in the big stack; washing: no
- ghostly: a woman humming the lullaby in the loft
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
