# Hollowmere - village layout v2 (streets, plots, landmarks)

> Machine-readable twin: `docs/design/village.json`. Matches DESIGN.md 'The village map': 300 x 300 m inside the wall, square and gate on the north edge. The square, gate, altar, stone circle, bell tower, notice board, chapel and fog road stay EXACTLY where `GreyboxVillageBuilder` puts them today; only the houses (were a 3 x 5 grid), streets and dressing change.

![village map](ref/village_map_v2.png)

## Why the old layout read wrong, and the rules now
- Houses sat on a grid, all facing north. Now every house faces the street its front door opens onto (yaw per house).
- No street hierarchy. Now: Main Street (6 m, setts, pavements) -> lanes (4-4.5 m, mud + gravel) -> tracks (3 m) -> paths (2-2.5 m).
- Empty gaps. Now 16 boarded/ruined filler buildings, plots with walls/fences/gardens, lamps, a green with a well and memorial.
- Main Street is a continuous built frontage from the square to the green (H12, F01, H08, H10, F04 / F02, H00, F03) - the 'tight' part of the village; the green is the open heart; lanes radiate out to looser, more isolated houses (Mill, Widow, Farm).
- Routes: every house is 35-230 m from the altar (front door straight line). The far three (Miller 229 m, Widow 203 m, Farmer 164 m) are the high-risk runs; Main Street houses are the short, safe-feeling ones.
- Landmarks for orientation: bell tower (seen from everywhere), chapel, the green with its oak and well, the mill wheel, the farm barn.

## Houses
| House | World x | Floor y | World z | Yaw (front faces) | Street | Front door | To altar |
|---|---|---|---|---|---|---|---|
| H00 Blue Door House | 10.6 | +0.45 | 52.0 | 270 | Main Street (east side) | (6.1, 54.4) | 58.0 m |
| H01 Rao House | 36.0 | +0.10 | 6.0 | 262 | The Green (east side) | (31.6, 5.8) | 110.8 m |
| H02 Widow's House | 25.5 | -0.30 | -90.0 | 270 | Main Street south (east side, by the stream bridge) | (20.7, -90.0) | 203.1 m |
| H03 Schoolmaster's House | 56.0 | +0.20 | 58.0 | 0 | School Lane (south side, opposite the school) | (54.4, 63.0) | 73.2 m |
| H04 Weaver's House | -53.8 | -0.20 | -32.2 | 135 | Mill Lane (north-west side) | (-51.2, -34.1) | 154.8 m |
| H05 Doctor's House | -62.0 | +0.30 | 48.0 | 90 | Chapel Lane (west side) | (-56.0, 48.0) | 85.0 m |
| H06 Twins' House | -24.0 | +0.00 | 29.0 | 180 | The Green (north side) | (-25.0, 25.0) | 90.5 m |
| H07 Organist's House | -42.0 | +0.30 | 82.0 | 270 | Chapel Lane (east side) | (-46.0, 82.0) | 54.9 m |
| H08 Butcher's House | -7.75 | +0.00 | 66.0 | 90 | Main Street (west side) | (-8.8, 62.0) | 50.8 m |
| H09 Miller's House | -104.0 | -0.60 | -96.0 | 0 | Mill Lane (end, at the pond) | (-103.5, -92.0) | 228.8 m |
| H10 Photographer's House | -8.0 | +0.00 | 50.0 | 90 | Main Street (west side) | (-4.0, 50.4) | 61.7 m |
| H11 Priest's House | -62.0 | +0.40 | 80.0 | 90 | Chapel Lane (west side) | (-58.0, 81.5) | 65.5 m |
| H12 Postmaster's House | 10.8 | +0.10 | 78.0 | 270 | Main Street (east side, by the square) | (5.6, 77.8) | 34.7 m |
| H13 Farmer's House | 86.0 | +0.20 | -34.0 | 0 | East Road (south side) | (82.5, -30.0) | 164.2 m |
| H14 Dollmaker's House | -29.0 | +0.00 | -4.0 | 90 | The Green (west side) | (-25.0, -4.0) | 118.7 m |

Placement formula: world = position + R(yaw) * local, with R(yaw): x' = x cos(yaw) + z sin(yaw), z' = -x sin(yaw) + z cos(yaw) (Unity `Quaternion.Euler(0, yaw, 0) * local`).

## Streets
| Id | Name | Kind | Width | Surface | Points (x, z) | Length |
|---|---|---|---|---|---|---|
| R1 | Main Street | street | 6.0 m | Cobbles2 setts + 1.5 m Paving pavements with a 0.12 m kerb | (0,87), (0.7,72), (1.5,60), (0.8,45), (-1,32), (0,19) | 68.2 m |
| R2 | Main Street south | street | 5.0 m | Cobbles2 setts + 1.5 m Paving pavements with a 0.12 m kerb | (0,-11), (4,-30), (8,-55), (12,-78), (16,-100), (19,-114), (21,-124) | 115.0 m |
| R3 | Chapel Lane | lane | 4.5 m | Road (mud+leaves) centre, GroundGravel edges | (-35,104), (-46,100), (-52,88), (-53,70), (-51,52), (-46,32), (-36,16), (-22,8) | 116.9 m |
| R4 | School Lane | lane | 4.5 m | Road (mud+leaves) centre, GroundGravel edges | (1,70), (20,70), (40,69), (58,67.4), (80,66) | 79.1 m |
| R5 | Mill Lane | lane | 4.0 m | Road (mud+leaves) centre, GroundGravel edges | (-20,-12), (-28,-20), (-40,-30), (-56,-46), (-72,-62), (-88,-78), (-96,-86) | 106.1 m |
| R6 | East Road | lane | 4.5 m | Road (mud+leaves) centre, GroundGravel edges | (22,-2), (44,-8), (62,-16), (82,-22), (110,-24), (128,-26) | 109.6 m |
| R7 | Back Lane | track | 3.0 m | GroundMud with a grass centre strip (GroundGrassPath) | (-24,88), (-25,70), (-24,52), (-22,40) | 48.2 m |
| R8 | Field Track | track | 3.0 m | GroundMud with a grass centre strip (GroundGrassPath) | (100,-24), (104,-50), (96,-76), (88,-100) | 78.8 m |
| R9 | Green loop | path | 2.5 m | GroundGrassPath / stepping stones | (-22,8), (-21,-8), (-8,-14), (10,-14), (21,-6), (22,10), (10,20), (-8,20), (-22,8) | 130.0 m |
| R10 | Pond path | path | 2.0 m | GroundGrassPath / stepping stones | (-96,-86), (-103.5,-90) | 8.5 m |
| R13 | Butcher's Alley | path | 2.0 m | GroundGrassPath / stepping stones | (1.4,58.5), (-24.5,58.5) | 25.9 m |

Build streets as meshes from these polylines (Catmull-Rom smoothed, 0.5 m segments), 0.02 m above ground, with a 0.3 m blended verge decal. The existing fog road (0,150)->(0,352) is unchanged.

## Landmarks and village buildings
| Id | Name | Centre (x, z) | Size | Yaw | Notes |
|---|---|---|---|---|---|
| L_GATE | Village gate (existing) | (0, 150) | 10.4 x 1.6 | 0 | existing BuildGate; 'DO NOT LOOK BACK' inscription; ring wall continues E and W |
| L_SQUARE | Town square (cobbled) (existing) | (0, 117) | 70 x 60 | 0 | existing Paving 70 x 60; add a 0.15 m kerb + 2 m pavement round its edge, 4 benches, 2 street lamps |
| L_ALTAR | Altar + stone circle (existing) | (0, 112) | 16 x 16 | 0 | existing |
| L_BELL | Bell tower (existing) | (26, 132) | 4 x 4 | 0 | existing; dress with StoneBlock + a louvred belfry + clock face (stopped) |
| L_BOARD | Notice board (existing) | (14, 96) | 3.8 x 0.6 | 180 | existing |
| L_CHAPEL | Chapel (existing) | (-60, 112) | 12 x 18 | 90 | existing 12 x 18 shell; dress: ChapelWall, ChapelRoof, a west bellcote, 4 lancets per side, porch on the east door |
| L_GRAVE | Graveyard | (-72, 104) | 30 x 34 | 0 | dry-stone wall 1.2 m, LYCHGATE at (-56,88) on Chapel Lane, ~60 Gravestone, 4 AngelStatues, 2 yews (SmallTree dark), 1 open grave; Hollow night spawn stays inside the chapel |
| L_STORE | Bakery & General Store | (44, 112) | 12 x 9 | 270 | DESIGN zone: by day villagers, tools, batteries; by night locked and dark. Shop window 'HOLLOWMERE STORES - BAKER', bread oven chimney, a delivery van (CoveredCar) outside |
| L_SCHOOL | Schoolhouse | (66, 80) | 18 x 9 | 180 | DESIGN zone: single tall room + bell cupola (Bell scaled 0.5), 12 SchoolDesk + SchoolChair, Blackboard, children's drawings on the walls; schoolyard 26 x 16 to the north with a wall (2.2) and swings |
| L_GREEN | Village green | (0, 4) | 44 x 30 | 0 | grass oval with worn paths; the WELL at (-4, 4); WAR MEMORIAL at (10, 8); 2 benches; an old oak (SmallTree scaled 2.5) |
| L_WELL | Well | (-4, 4) | 2.6 x 2.6 | 0 | DESIGN zone: sounds come up from below at night |
| L_MEMORIAL | War memorial | (10, 8) | 1.8 x 1.8 | 180 | names on it match graveyard family names |
| L_KIOSK | Telephone kiosk | (-11, 23) | 1.0 x 1.0 | 180 | dead line; rings once at midnight |
| L_SHELTER | Bus shelter | (7, 32) | 3.0 x 1.5 | 270 | timetable: last bus 1977 |
| L_POND | Mill pond | (-112, -76) | 30 x 16 | 0 | water plane at -0.8; reeds; sunk rowing boat |
| L_FIELDS | Fields & scarecrows | (95, -105) | 90 x 70 | 0 | tall crop (wheat stubble + standing maize strip 2.2 m for hiding), 6 SCARECROWS (puzzle H13), hedges, a field gate |
| L_PINFOLD | Pinfold (animal pound) | (-46, -10) | 8 x 8 | 0 | round dry-stone pen 1.5 m |

## Filler buildings (non-allotted)
All locked; boarded windows (Planks over WeakWindowBoards) you can peek through; F03 is a walk-through ruin (roof gone, 1 hiding spot), F06 an open forge (hiding behind the bellows). Use 3 shell variants: cottage (gable, Roof), terrace (2 doors), outbuilding (CorrugatedRust).

| Id | What | Centre (x, z) | Size | Yaw |
|---|---|---|---|---|
| F01 | terrace pair, boarded (Main St) | (-7.8, 80) | 10 x 7 | 90 |
| F02 | cottage, boarded (Main St) | (10.0, 62) | 8 x 7 | 270 |
| F03 | cottage, walk-through ruin (Main St) | (8.6, 40) | 8 x 7 | 270 |
| F04 | cottage, boarded (Main St) | (-9.2, 36) | 8 x 7 | 90 |
| F05 | sexton's cottage | (-44, 62) | 7 x 6 | 270 |
| F06 | forge (open-sided) | (-46, 6) | 8 x 6 | 90 |
| F07 | cottage, boarded | (26, -20) | 8 x 6 | 300 |
| F08 | pair of cottages | (-8, -36) | 12 x 7 | 90 |
| F09 | cottage, boarded (Mill Lane) | (-64.3, -65.7) | 8 x 6 | 315 |
| F10 | garage + petrol pump (1960s) | (40, 30) | 9 x 7 | 270 |
| F11 | cottage, boarded | (64, 22) | 8 x 7 | 0 |
| F12 | Dutch barn (open) | (118, -36) | 14 x 8 | 0 |
| F13 | cottage, boarded | (2.5, -66) | 8 x 6 | 90 |
| F14 | allotment sheds x3 | (66, -34) | 9 x 4 | 0 |
| F15 | pump house | (-84, -94) | 4 x 4 | 0 |
| F16 | cottage, boarded | (44, 92) | 8 x 7 | 180 |

## Water, bridges, night blockers, scarecrows, lamps
- Stream (3 m wide, bed y -1.2): (-150,-62), (-132,-68), (-124,-74), (-112,-76), (-98,-80), (-91.5,-88), (-91.5,-100), (-80,-108), (-40,-112), (-5,-116), (19,-114), (50,-120), (90,-128), (150,-136). Mill race = the segment along x = -91.5 past the mill wheel.
- Bridge B1: Main Street stone bridge (2 arches) at (19, -114), 6.0 m wide, 10.0 m long.
- Bridge B2: Mill race footbridge at (-93.5, -84.5), 2.0 m wide, 5.0 m long.
- Night blockers (MatchPlan picks one per match - DESIGN 'A fallen tree or cart blocks one random path'): BL1 FallenTree at (-52.5, 76), BL2 BrokenCart at (0, 38), BL3 FallenTree at (-48, -38), BL4 BrokenCart at (52, -12), BL5 FallenTree at (9, -60), BL6 BrokenCart at (30, 69.6). Each sits across a lane and carves the NavMesh.
- Scarecrows (H13 puzzle; DESIGN: turn to face the nearest player between acts): [(70, -80), (88, -92), (106, -84), (118, -110), (96, -126), (74, -118)].
- Street lamps (StreetLamp, 1 in 3 working by day, all dead at night except during the chant): [(-6, 90), (6, 92), (-4, 60), (5, 44), (-5, 22), (6, -18), (-26, 2), (-50, 56), (-48, 92), (24, 69), (48, 67), (14, -60), (-40, -34)].
- Forest belt: 20-25 m deep inside the ring wall (FirTree/PineTree impostors + existing DeadLimbs prefabs + Shrub/Fern/Nettles), dense enough to block sight; the Hollow's ForestSpawns move to its inner edge.

## Gameplay hooks to update (VillageLayout.cs)
- `RoamPoints`: replace the old 6 x 5 grid with the road polyline vertices + the green + the square (the builder can derive them from village.json).
- `ForestSpawns`: 8 points on the forest belt's inner edge (one per compass direction).
- `PlayerSpawn`, `HollowNightSpawn` (chapel), `SquareCenter`, `GateLineZ`, `BridgeZ`: unchanged.
- Re-record `GreyboxLayout.txt` (LayoutFingerprint) ONCE the user has approved the new layout.
