# Hollowmere - the 15 houses (detailed build spec)

This is the index. Each house has its own page and a JSON twin in `docs/design/houses/`. Everything was generated from `tools/housegen/` (Python) and validated: rooms don't overlap, every door has 0.85 m clear in front of it on both sides, no prop blocks a door or stair, every prop sits inside its room, and each house has the DESIGN shared-template items (hearth, fuse box, phone, mirror, family photo, radio/TV, wardrobes/beds, 4-6 hiding spots, front + back door, exactly 2 weak windows, kitchen, living room, 2+ bedrooms, bathroom, cellar or attic, 3 notes, item spots, puzzle, container, shrine niche).

**Source of truth:** `docs/DESIGN.md` for rules; these files for geometry and dressing. HouseCatalog.cs keeps names, families, parts, door colours, chimney counts and clock hours - unchanged, so letters and MatchPlan still work.

## Coordinates (read once)
- Metres. Each house has its own local space: origin (0,0,0) = ground-floor level at the centre of its main wing.
- **+z = the FRONT** of the house (the street side), -z = back, +x / -x = the two sides (+x is on your LEFT when you stand in the street facing the front door).
- Room `rect` = (x0, z0, x1, z1) measured to the OUTER face of exterior walls and to the CENTRE line of interior walls. Exterior wall thickness is taken inside the rect (wing `t`); interior partitions are 0.12 m and straddle the shared edge. `inner` in the JSON is the clear floor.
- Doors/windows: `side` = which wall of `room_a` (N=+z, S=-z, E=+x, W=-x), `at` = centre along that wall (x for N/S, z for E/W).
- Prop `yaw` = Unity Y rotation: 0 -> the prop's front faces +z, 90 -> +x, 180 -> -z, 270 -> -x.
- World placement: `world.position` (x, y = floor level, z) and `world.yaw` (the way the FRONT faces) - see VILLAGE_LAYOUT.md.

## Summary
| # | House | Archetype | Footprint | Storeys | Roof | Puzzle (M1 rig) | Part | Street | To altar |
|---|---|---|---|---|---|---|---|---|---|
| H00 | [Blue Door House](houses/H00_blue_door_house.md) | Clockmaker's narrow Georgian brick town house, shop on the ground floor | 7.6 x 10.0 m | G+F+A | Gable roof | Clock | Iron gate key | Main Street (east side) | 58.0 m |
| H01 | [Rao House](houses/H01_rao_house.md) | 1930s pebbledash detached house that never opens its shutters, big walled vegetable garden | 9.2 x 11.6 m | C+G+F | Hipped roof 35 deg | CodeLock | Family seal ring | The Green (east side) | 110.8 m |
| H02 | [Widow's House](houses/H02_widow_s_house.md) | Low thatched cob cottage, 1.5 storeys, with a deep front porch | 12.0 x 10.0 m | G+F | Thatch 50 deg | Sequence | Hearth ember | Main Street south (east side, by the stream bridge) | 203.1 m |
| H03 | [Schoolmaster's House](houses/H03_schoolmaster_s_house.md) | Victorian Gothic stone house attached to the schoolyard wall | 10.0 x 14.0 m | G+F+A | Steep 55 deg gable facing the street (ridge along z) | CodeLock | Bell clapper | School Lane (south side, opposite the school) | 73.2 m |
| H04 | [Weaver's House](houses/H04_weaver_s_house.md) | Weaver's cottage: long stone house whose whole top floor is a loom shop lit by a continuous band of windows | 14.0 x 8.8 m | G+F+A | Gable roof 35 deg | Sequence | Spool of red thread | Mill Lane (north-west side) | 154.8 m |
| H05 | [Doctor's House](houses/H05_doctor_s_house.md) | White stucco Edwardian villa with a single-storey surgery wing | 15.0 x 10.5 m | C+G+F | Hipped roof 30 deg | CodeLock | Glass eye | Chapel Lane (west side) | 85.0 m |
| H06 | [Twins' House](houses/H06_twins_house.md) | Two identical Victorian brick cottages knocked into one, perfectly mirrored | 14.0 x 8.0 m | G+F+A | Gable roof 38 deg | Sequence | Silver locket | The Green (north side) | 90.5 m |
| H07 | [Organist's House](houses/H07_organist_s_house.md) | Tall gaunt Gothic stone house beside the chapel, with an organ bay | 10.7 x 13.0 m | C+G+F | Steep gable roof 55 deg | Sequence | Organ pipe | Chapel Lane (east side) | 54.9 m |
| H08 | [Butcher's House](houses/H08_butcher_s_house.md) | Main Street butcher's shop with the family living above and a cold room behind | 8.0 x 15.0 m | C+G+F | Gable roof | CodeLock | Bone whistle | Main Street (west side) | 50.8 m |
| H09 | [Miller's House](houses/H09_miller_s_house.md) | Miller's house joined to a three-storey watermill with a big undershot wheel on the leat | 16.0 x 10.0 m | G+F+A | Gable roof 40 deg | Clock | Last-harvest sheaf | Mill Lane (end, at the pond) | 228.8 m |
| H10 | [Photographer's House](houses/H10_photographer_s_house.md) | Edwardian red-brick double-fronted shop-house with a Victorian glasshouse studio behind | 10.0 x 14.0 m | C+G+F | Gable roof 40 deg | CodeLock | Glass negative | Main Street (west side) | 61.7 m |
| H11 | [Priest's House](houses/H11_priest_s_house.md) | Stone presbytery with a private oratory wing projecting toward the lane | 12.0 x 10.0 m | G+F+A | Gable roof 45 deg | CodeLock | Altar cloth | Chapel Lane (west side) | 65.5 m |
| H12 | [Postmaster's House](houses/H12_postmaster_s_house.md) | 1920s Arts-and-Crafts brick house with a twin-gabled front and a flat-roofed sorting room | 14.0 x 9.2 m | G+F+A | M-roof: two parallel gables with ridges along z at x=-2.5 and x=+2.5 | Sequence | Sealed letter | Main Street (east side, by the square) | 34.7 m |
| H13 | [Farmer's House](houses/H13_farmer_s_house.md) | Stone longhouse farmhouse with the barn under the same long roof and a dairy outshut | 18.0 x 11.0 m | G+F | Continuous gable roof 40 deg over house AND barn | Sequence | Straw heart | East Road (south side) | 164.2 m |
| H14 | [Dollmaker's House](houses/H14_dollmaker_s_house.md) | Crooked 16th-century timber-framed house, the oldest in the village, with a jettied upper floor | 9.0 x 8.6 m | G+F+A | Gable roof 55 deg | Sequence | Porcelain hand | The Green (west side) | 118.7 m |

![village](ref/village_map_v2.png)

## How the houses differ (variety rules met)
- No two share footprint size, wall material + roof combination, or storey count + chimney layout.
- Plans: double-pile town house (H00), 1930s hall house (H01), single-pile thatched cottage (H02), front-gabled Gothic + rear wing (H03), cross-passage longhouse with loom floor (H04), double-pile villa + surgery wing (H05), mirrored double house (H06), tall Gothic + bay + rear wing (H07), shop with flat over + cold room (H08), house + 3-storey mill (H09), shop-house + glasshouse studio (H10), L-plan presbytery + oratory (H11), twin-gabled Arts & Crafts + sorting wing (H12), longhouse + barn + dairy (H13), lobby-entry jettied timber frame (H14).
- Stairs: straight, boxed, winder, stone, mirrored pair, ladders, mill ladders, cellar trap stairs - each house has its own.

## Build order (see ROADMAP M1.6)
1. Data loader + validation (C#) for `houses/*.json` and `village.json`.
2. HouseBuilder: shells, openings, floors, stairs, roofs, chimneys (greybox primitives, real materials).
3. Props: asset-key -> prefab registry (PropKit), placement, hiding/puzzle/container/shrine wiring.
4. Puzzles: reskin the 3 M1 rigs per house, then bespoke rigs (GearPuzzle, ScalePuzzle, DarkroomPuzzle, DollhousePuzzle...).
