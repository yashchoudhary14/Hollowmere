# Handoff (cloud session <-> PC session)

Read `docs/WORKFLOW_CLOUD_PC.md` first. Each session edits only its own sections, then commits this file.

```text
Mode:          hybrid          # hybrid = cloud writes code, PC runs Unity | local = the PC session does both halves
Milestone:     M1.6 Village & houses rebuild (docs/ROADMAP.md)
Current phase: P3 - House shells
Phase state:   PC-TODO         # CLOUD-TODO -> PC-TODO -> DONE -> (next phase) CLOUD-TODO
Cloud branch:  claude/docs-review-cloud-phase-jbbppo
```

## Cloud summary for this phase (cloud session writes)

- **P3 was split** (ROADMAP updated). P3 now builds the 15 real house shells into `Village.unity` in place of the P2 footprint boxes. **P3b** (next) moves the gameplay layer (player, Hollow, MatchDirector, HUD, hearth, fuse box, wardrobe, notes, puzzles) onto `Village.unity` and re-records the fingerprint. Doing both at once would have meant one huge unreviewable phase. `Greybox.unity`, its tests and `GreyboxLayout.txt` are still untouched.
- **`Builders/HousePlan.cs`** (pure C#, no scene objects, like `VillagePlan`): each `Hxx.json` as shell geometry in the house's own space.
  - Exterior walls at the wing's thickness (`t`) and first wall material key, partitions 0.12 m, one per pair of rooms.
  - Every door and window cut at its design width and height (sills, lintels); an open bay with no door has no partition.
  - A floor per room with holes over the flights, a slab over each wing's top storey (out to the eaves, which closes the P2 "white line under the eaves"), holes in the village ground over cellar stairs.
  - Flights: stairs get a hidden walkable ramp under drawn treads (all at most 48 degrees); anything steeper than 55 degrees is a ladder. Foot and head landings are found inside the rooms, clear of walls.
  - A clear probe point in every room (tests now; the Hollow's search points in P3b).
  - Floors never sit at or below the street: every house floor is at least 0.03 m up. H02, H04 and H09 (below 0 in the JSON) and H06, H08, H10, H14 (at 0) get 0.03 m, so the village ground under a house never z-fights its floor; raised houses keep their height and get front steps.
- **`Builders/GreyboxVillageBuilder.HousesV2.cs`** builds the plans into the scene, under `Village/Houses/Hxx Name/`:
  - `Shell/Walls`, `Shell/Floors`, `Shell/Roofs` (the P2 roof shapes, now with MeshColliders and a Not Walkable NavMesh area), chimneys.
  - `Openings`: glass in every window (`WeakWindow_*` / `Window_*`), usable outside doors as the M1 `Door` (pivot on the hinge, carving obstacle while shut), fixed leaves for sealed doors and doors high up a wall, steps up to raised floors. With an open porch, the porch's inner door is the front door.
  - `Stairs`: ramp + treads + stringer; ladders as rails and rungs with a `Ladder` at each end (new runtime component: hold E 0.6 s to climb, noise 5 m like walking) and a `NavMeshLink` so the Hollow can follow.
  - `Rooms`: a `Room` per JSON room in JSON order, with `Id`, oriented bounds, a ceiling light and the hearth room (the one prop with role `hearth`).
  - `House`: `Rooms`, `Door`, `HearthRoom`, new `Volumes` (each wing from the street or cellar to the eaves or attic ceiling, in house space) and `Front` (the `FrontDoor` marker).
  - `Room.Contains` and `House.Contains` use the oriented volumes when present; M1 houses (no volumes, `Oriented` off) behave exactly as before.
- **NavMesh for houses.** Doors are 0.7-1.0 m wide, so the Humanoid agent becomes **radius 0.25 m, max slope 50 degrees** (`SetAgentForHouses`, stored in `ProjectSettings/NavMeshAreas.asset`, which the PC commits), and `Village.unity` bakes at a 0.08 m voxel. Greybox keeps its baked NavMesh; only a re-bake of Greybox would pick up the new radius.
- **Design data fixes.** Building the shells showed door and stair mistakes in the pack. All fixed in `tools/housegen`, with new `validate()` rules and C# twins in `DesignChecks`: no door opens onto a flight except at its foot or head; doors clear the partitions and the walls at the ends of their wall in both rooms; windows and outside doors don't open onto a neighbouring room; exactly one prop per house has role `hearth`.
  - H00: stair 0.8 m wide (hall walkway).
  - H01: dining door at the stair foot, bed2 door at the stair head, stair 0.2 m shorter, cellar stair along the back lobby's south wall, crates and fuse box moved.
  - H02: scullery 0.4 m wider for its door, sink moved; kitchen window moved clear of the porch.
  - H03: bed2 door clear of the stairwell; the sitting-room stair moved off the outside wall (now 0.9 m clear instead of 0.5) and 3.2 m long so the kitchen door doesn't open onto it; side table and armchair moved.
  - H04: the back door was half on the outshut's wall: moved to x -0.55 (in line with the front door at -0.5) and 0.85 m wide; the passage stair shortened to 3.2 m so it opens clear of it; the broken loom moved off the stairwell.
  - H06: kitchens through the parlours, nurseries through the bedrooms (the mirrored stairs fill the hall walls); the west range is now `hearth-decor` (the east one is THE hearth).
  - H07: cellar trapdoor stair moved north; bay arch 2.2 m (the bay's inner width); parlour fireplace `hearth-decor`.
  - H08: side front door clear of the stair; passage door 0.75 m.
  - H09: mill stair clear of the mill door; bed2/bath partition moved.
  - `docs/design` regenerated: JSON + Markdown of the changed houses, their plan PNGs (H01, H02, H03, H04, H07) and H04's massing.
- **P2 visual notes** (from the P2 PC report): (1) eaves line: house walls now meet the top slab and filler/store/school roofs get a soffit; (2) green and pinfold are now metre-UV meshes, not stretched cylinders; (3) water darker and less glossy; (4) the oak is a 9 m dead trunk with the forest's dead limbs; (5) **the dark strip by B2: not found from the code**, please name the object (select it in the Scene view, read its name in the Hierarchy) in the report.
- **Tests (11 new, 114 EditMode in all):**
  - `HousePlanTests` (7, no scene): every door/window cut once at its design size (attic gable lights and H09's lucam come with the roof dressing later); no wall piece stands in an opening; stairs at most 48 degrees and 0.6 m wide; landings inside their rooms and off the walls; a clear probe per room; a stairwell over every flight and a ground hole per cellar stair; `RectMinus`/`Subtract`.
  - `VillageV2HouseTests` (4, run once the scene is rebuilt): rooms with the JSON ids and one hearth room, probes inside house and room; every room reachable on the NavMesh from outside the front door with the doors open (stairs walked, ladders counted as climbed); every stair walked up and down by its own flight; every ladder has two `Ladder` ends with the right exits and a `NavMeshLink`.
  - `VillageV2SceneTests`: the house-position test now expects the raised floor (0.03 m) for houses at or below the street.
- **What the cloud could check without Unity:**
  - `run_check.py`: 0 errors, village problems 0. `DesignChecks` (C#) on all 15 houses: 0 problems.
  - `HousePlanTests` + `VillagePlanTests` ran in the scratchpad .NET harness: **16/16 pass**.
  - A 0.1 m grid reachability check over each `HousePlan` (agent 0.3 m, stairs and ladders as links): every room of every house reachable from outside its front door.
  - The builders, runtime changes and both new test files were **type-checked** against stubs of the Unity/project API: 0 errors.
  - Not yet run in Unity: the real compile, the build and bake, the scene tests, every screenshot.
- **For P3b:** the Hollow's NavMeshAgent radius should be at most 0.25 m to use the house NavMesh well; `Ladder.ClimbSeconds` and `NoiseRadius.Ladder` are not in DESIGN.md yet (question added to its last section).

## PC checklist for this phase (cloud session writes; PC session ticks)

1. [ ] User closes the Unity Editor.
2. [ ] Merge: `git checkout main && git pull --ff-only origin main && git fetch origin && git merge --no-ff origin/claude/docs-review-cloud-phase-jbbppo -m "Merge M1.6 P3 (cloud): house shells"`.
3. [ ] `tools/unity.ps1 check`: 0 compile errors. Fix anything and record what the cloud got wrong below.
4. [ ] `tools/unity.ps1 test` **before** rebuilding: 114 tests. Expect exactly 5 failures, all because `Village.unity` is still the P2 scene: `VillageV2SceneTests.Scene_HasEveryHouseAtItsPlace_AndTheV2Furniture` (old floor heights) and the 4 `VillageV2HouseTests` (no house shells yet). Everything else passes, in particular the 7 `HousePlanTests` and `DesignDataTests`. If a `HousePlanTests` case fails, paste its message.
5. [ ] `tools/unity.ps1 run -Method Hollowmere.EditorTools.VillageBuilderV2.Build`. The log shows `NavMesh agent (Humanoid): radius 0.25, max slope 50`, `NAVMESH BAKED` and `VILLAGE V2 BUILT: Assets/Hollowmere/Scenes/Village.unity (15 houses, ...)`. Note how long it took (P2: 31-76 s; the bake is finer now) and paste any error lines. If it reports it couldn't reach the NavMesh agent settings, set them by hand as the message says and record it.
6. [ ] `tools/unity.ps1 run -Method Hollowmere.EditorTools.VillageBuilderV2.Shots`: 12 PNGs `Logs/claude/v2_*.png`, now with real houses.
7. [ ] `tools/unity.ps1 run -Method Hollowmere.EditorTools.VillageBuilderV2.HouseShots`: 73 PNGs, `Logs/claude/house_Hxx_front.png`, `_back.png` and `_plan_<level>.png` (C, G, F, A as the house has them; the cutaways have the roofs off). If any is black or empty, say so instead of guessing.
8. [ ] For each house, compare:
   - `house_Hxx_front.png` / `_back.png` with `docs/design/ref/Hxx_massing_front.png` / `_back.png` (wings, roof shapes, chimneys; walls are flat placeholder colours until P4/P5)
   - each `house_Hxx_plan_<level>.png` with that level in `docs/design/ref/Hxx_plan.png` (house +z is up in both): rooms and walls in the right places, every door a gap, stairs where the plan has them, windows as glass in the outer walls

   List differences per house as notes for the cloud; don't change the data or the builders by hand.
9. [ ] Look at the street shots (`v2_MainStreet`, `v2_MainStreetNorth`, `v2_Green`, `v2_MillLane`, `v2_StreamBridge`): the white eaves line, the green's grass, the pond and the oak (P2 notes 1-4) should be fixed. For note 5, open `Village.unity`, click the dark strip left of the B2 bridge in the Scene view and record the selected object's name.
10. [ ] Ask the user to open `Village.unity` and walk through two or three houses in the Scene view (or fly the camera inside). **User approves the house shells**, or lists changes. Record them for the cloud.
11. [ ] `tools/unity.ps1 test`: now **114 passed**. If a `VillageV2HouseTests` case fails, paste its whole message (it lists every house and room that failed).
12. [ ] `tools/unity.ps1 test -Platform PlayMode`: 8 passed (Greybox unchanged).
13. [ ] Do **not** re-record `GreyboxLayout.txt`: `LookPassTests` must pass as it is (Greybox didn't change). That happens in P3b.
14. [ ] Commit and push to `main`:
    - `Assets/Hollowmere/Scenes/Village.unity` and `Village_NavMesh.asset`
    - `Assets/Hollowmere/Art/Meshes/VillageV2Meshes.asset` and `VillageV2BuilderMeshes.asset`
    - `ProjectSettings/NavMeshAreas.asset` (agent radius and slope)
    - new materials in `Assets/Hollowmere/Materials/Greybox/`, if any
    - the `.meta` files for the 5 new scripts (`HousePlan.cs`, `GreyboxVillageBuilder.HousesV2.cs`, `Ladder.cs`, `HousePlanTests.cs`, `VillageV2HouseTests.cs`) and all of the above

    Note the scene's size in the report. Commit message: "M1.6 P3: house shells built, user approved (PC verified)".
15. [ ] Tick the P3 PC item in `docs/ROADMAP.md`, fill the PC report below, set `Phase state: DONE`, add a Log line, push `main`, delete the remote branch (`git push origin --delete claude/docs-review-cloud-phase-jbbppo`).

Done when: check is clean, EditMode 114/114 and PlayMode 8/8 pass, the user has approved the shells (or listed changes), and `main` is pushed with the scene, NavMesh settings and assets.

## PC report (PC session writes)

- Merge:
- check:
- test (EditMode / PlayMode):
- Builders run (and how long):
- Screenshots (paths) and what they showed:
- Compile errors fixed (what the cloud got wrong):
- User decisions / approvals:
- Notes for the next cloud phase:

## Log (newest first, one line per phase half)

- 2026-10-22 - P3 cloud: house shells in `Village.unity` (`HousePlan` + `GreyboxVillageBuilder.HousesV2.cs`: walls, openings, floors, stairs, ladders with `Ladder` + NavMeshLink, roofs, `Room`s with JSON ids); NavMesh agent 0.25 m / 50 degrees; design data fixes in 9 houses + 4 new validate rules (C# twins); `HousePlanTests` (7, passed offline) + `VillageV2HouseTests` (4); `VillageBuilderV2.HouseShots`; P3 split (P3b moves the game). Branch claude/docs-review-cloud-phase-jbbppo. Next: P3 PC.
- 2026-10-22 - P2 PC (via Cline): merged claude/docs-review-cloud-phase-jbbppo; check clean (no fixes needed); `VillageBuilderV2.Build` (31 s) + `Shots` (12 PNGs); top-down matches `village_map_v2.png`; user approved the layout (no changes); EditMode 103/103, PlayMode 8/8; GreyboxLayout.txt not re-recorded; `Village.unity` (13.4 MB), NavMesh, mesh assets, Greybox materials, DeadLimbs prefabs and .meta files committed; visual + asset-hygiene notes for the cloud in the PC report. Next: P3 cloud.
- 2026-10-22 - P2 cloud: village v2 ground plan in its own scene (`Village.unity`): `VillagePlan` (pure plan), `GreyboxVillageBuilder.VillageV2.cs` (+ `VillageBuilderV2.Build/Shots`), `VillagePlanTests` (9, passed offline) + `VillageV2SceneTests` (6); BuilderKit mesh cache per scene; Greybox untouched. Branch claude/docs-review-cloud-phase-jbbppo. Next: P2 PC.
- 2026-10-22 - P1 PC (via Cline): merged claude/docs-review-cloud-phase-jbbppo; `setup` added Newtonsoft 3.2.2 as a direct dependency; check clean (no fixes needed); EditMode 88/88, PlayMode 8/8; run_check.py exit 0; .meta files + manifest committed. Next: P2 cloud.
- 2026-10-22 - P1 cloud: Newtonsoft in RequiredPackages + Editor asmdef; `Scripts/Editor/Design/` (DesignSpecs, DesignData, DesignChecks); `DesignDataTests` (10). Branch claude/docs-review-cloud-phase-jbbppo. Next: P1 PC.
- 2026-10-22 - Design pack added (one-time: PC session copies docs/setup/skills/* into .claude/skills/, commits, pushes): docs/design (15 houses, village v2, assets), workflow, M1.6 in ROADMAP. Next: P1 cloud.
