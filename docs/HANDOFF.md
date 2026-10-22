# Handoff (cloud session <-> PC session)

Read `docs/WORKFLOW_CLOUD_PC.md` first. Each session edits only its own sections, then commits this file.

```text
Mode:          hybrid          # hybrid = cloud writes code, PC runs Unity | local = the PC session does both halves
Milestone:     M1.6 Village & houses rebuild (docs/ROADMAP.md)
Current phase: P2 - Village v2 ground plan (greybox)
Phase state:   PC-TODO         # CLOUD-TODO -> PC-TODO -> DONE -> (next phase) CLOUD-TODO
Cloud branch:  claude/docs-review-cloud-phase-jbbppo
```

## Cloud summary for this phase (cloud session writes)

- **New scene, Greybox untouched.** The v2 ground plan is built into its own scene, `Assets/Hollowmere/Scenes/Village.unity`. The M1 gameplay houses (a fixed 12 x 9 m plan) don't fit the new plots (H00 is 7.6 m wide, H08 8 m), so rewriting `Greybox.unity` now would break the game and every M1 scene test until P3. `Greybox.unity`, its tests and `GreyboxLayout.txt` stay exactly as they are. P3 moves the gameplay onto the v2 scene and re-records the fingerprint once (ROADMAP P2/P3 updated to say so). `Village.unity` is not in the build settings yet.
- **`Builders/VillagePlan.cs`** (pure C#, no scene objects): the whole ground plan from `village.json` + the house files. It covers:
  - roads Catmull-Rom smoothed every 0.5 m
  - the stream channel (bed -1.2, steep banks) and the pond (-0.8 surface)
  - both bridges centred where their road crosses the stream and lengthened until both ends sit on flat ground (B2 becomes 6 m instead of 5 m)
  - invisible barriers along both banks and round the pond, with gaps only at the bridges
  - solid footprints (house wings, 16 filler buildings, landmark buildings, the M1 chapel)
  - plot boundaries from each house's "Front:"/"Back:" text (hedge, low wall, wall or post-and-rail fence), clipped off roads, water and a 3 m walk from every front door to its road
  - graveyard wall with the lychgate gap, schoolyard wall, field hedges, the round pinfold
  - the six night blockers across their lanes (road width + 3 m)
  - 58 roam points (road vertices + green + square) and 8 forest spawns on the belt's inner edge
  - about 516 belt trees (the M1 trunk + dead-limb prefab)
- **`Builders/GreyboxVillageBuilder.VillageV2.cs`**: `VillageBuilderV2.Build` turns the plan into the scene.
  - Reused unchanged from the M1 builders: ring wall, gate, fog road and outer forest, square, chapel, lights, grades and DressGrounds.
  - Ground: one mesh with a MeshCollider on 8 m tiles; flat tiles are single quads and tiles near water use a 1 m grid (about 12k vertices).
  - Streets: ribbon meshes in metres (setts with a kerb and pavements, lanes with gravel edges, tracks, paths), plus a flagged path from every front door.
  - Houses: footprint massing per wing from `Hxx.json`: box to the eaves, then a gable, M-roof, lean-to or flat roof read from the roof text. Each house also gets chimneys with pots at their JSON positions, a `FrontDoor` marker in the door colour, and steps for raised floors.
  - Filler buildings: closed boarded shells.
  - Landmarks:
    - store with its sign
    - school with its cupola, plus the schoolyard with swings
    - graveyard with gravestones, angels, yews and an open grave
    - green with the oak and benches
    - well, memorial, kiosk, bus shelter
    - fields, pinfold
  - Lamps, scarecrows and the six night blockers. The blockers are inactive, with a collider and a carving NavMeshObstacle.
  - NavMesh baked to `Village_NavMesh.asset`.
  - Cameras: a top-down orthographic camera plus 11 street and aerial cameras. `VillageBuilderV2.Shots` renders them all.
  - Wall, roof and street materials are named after catalog keys (`BrickRed`, `RoofSlateGrey`, `Cobbles2`...) with flat placeholder colours, so the P4 library entries dress them.
- **Small changes to existing code:**
  - `BuilderKit.Begin(meshAssetPath)`: each scene builder keeps its own metric-mesh cache, so building one scene never prunes meshes the other uses. Greybox still uses `BuilderMeshes.asset`; v2 uses `VillageV2BuilderMeshes.asset`.
  - `BakeNavMesh(assetPath)` takes the asset path.
  - Both default to the old behaviour.
- **Tests:**
  - `VillagePlanTests` (9, no scene): smoothing; roads follow their design lines; flat ground under roads and doors; bridges span the channel and no barrier cuts a deck; every front door has a clear walk to its road; boundaries off roads and water; blockers across their lanes; roam and spawn points clear; roof and boundary text parsing.
  - `VillageV2SceneTests` (6, skipped until the scene is built): houses at their JSON place; M1 landmarks unmoved; front-door distance to the altar within 2 m of `to_altar`; every front door reaches the square on the NavMesh, with no blocker and with each of the six on its own; each blocker carves its lane; roam and spawn points on the NavMesh.
- **What the cloud could check without Unity:**
  - Built a .NET harness in its scratchpad: the real `DesignSpecs`/`DesignData`/`DesignChecks`/`VillagePlan` sources compiled with Newtonsoft and Unity math stand-ins.
  - `VillagePlanTests` ran there: **9/9 pass**.
  - A 0.5 m grid route check with agent radius 0.5 (an approximation of the NavMesh): all 15 front doors, roam points and forest spawns reach the square, with no blocker and with each blocker alone.
  - The builder and both test files were **type-checked** against signature stubs of the Unity/project API they call: 0 errors.
  - Not yet run in Unity: the real compile, the bake, the scene tests and every screenshot.
- **For later phases:** H09 (-0.6), H02 (-0.3) and H04 (-0.2) have floors below the flat street (y 0). Their P2 boxes simply sink a little; P3 needs steps down or a lowered pad. Plot boundaries are simple runs; gates, wall caps and planting are P5 dressing.

## PC checklist for this phase (cloud session writes; PC session ticks)

1. [ ] User closes the Unity Editor.
2. [ ] Merge: `git checkout main && git pull --ff-only origin main && git fetch origin && git merge --no-ff origin/claude/docs-review-cloud-phase-jbbppo -m "Merge M1.6 P2 (cloud): village v2 ground plan"`.
3. [ ] `tools/unity.ps1 check`: 0 compile errors. Fix anything and record what the cloud got wrong below.
4. [ ] `tools/unity.ps1 test`: expect **97 passed, 6 ignored** (88 + 9 `VillagePlanTests`; the 6 `VillageV2SceneTests` are ignored until the scene exists). If a `VillagePlanTests` case fails, paste its message.
5. [ ] `tools/unity.ps1 run -Method Hollowmere.EditorTools.VillageBuilderV2.Build`: the log ends with `VILLAGE V2 BUILT: Assets/Hollowmere/Scenes/Village.unity (15 houses, 16 filler, ...)` and `NAVMESH BAKED`. Note how long it took.
6. [ ] `tools/unity.ps1 run -Method Hollowmere.EditorTools.VillageBuilderV2.Shots`: writes 12 PNGs to `Logs/claude/v2_*.png`. If any is black or empty, say so instead of guessing.
7. [ ] Compare `Logs/claude/v2_TopDown.png` (north up, white slug labels) with `docs/design/ref/village_map_v2.png`. Check that:
   - every house H00-H14 and filler F01-F16 sits where the map has it, turned the same way
   - the roads match the map, and the stream and pond with both bridges are there
   - the graveyard wall, schoolyard, fields and forest belt are there
   - the square, gate and chapel are where M1 had them
8. [ ] Look at the street shots: `v2_MainStreet`, `v2_MainStreetNorth`, `v2_Green`, `v2_MillLane` (required by the roadmap), plus `v2_Aerial`, `v2_ChapelLane`, `v2_SchoolLane`, `v2_EastRoad`, `v2_MillPond`, `v2_StreamBridge`, `v2_Fields`. Check that roads, kerbs and boundaries don't float or z-fight badly, house massing and roofs look sane, chimneys show, and no camera starts inside a wall.
9. [ ] **User approves the layout** from these shots (show them the top-down image and the four street shots). Record their changes, if any, as notes for the cloud. Don't change the data or `VillagePlan` by hand; the cloud does that.
10. [ ] `tools/unity.ps1 test`: now **103 passed** (the 6 `VillageV2SceneTests` run). If a route test fails, record the house slug and the blocker from the message.
11. [ ] `tools/unity.ps1 test -Platform PlayMode`: 8 passed (Greybox unchanged).
12. [ ] Do **not** re-record `GreyboxLayout.txt`: `LookPassTests` must pass as it is, because Greybox didn't change.
13. [ ] Commit and push to `main`:
    - the scene `Assets/Hollowmere/Scenes/Village.unity` and `Village_NavMesh.asset`
    - `Assets/Hollowmere/Art/Meshes/VillageV2Meshes.asset` and `VillageV2BuilderMeshes.asset`
    - the new materials in `Assets/Hollowmere/Materials/Greybox/`
    - the re-saved `Assets/Hollowmere/Prefabs/Trees/DeadLimbs_*.prefab`
    - the `.meta` files for the 4 new scripts and all of the above

    Note the scene's size in the report (Greybox.unity is about 20 MB). Commit message: "M1.6 P2: village v2 ground plan built, layout approved (PC verified)".
14. [ ] Tick the P2 PC item in `docs/ROADMAP.md`, fill the PC report below, set `Phase state: DONE`, add a Log line, push `main`, delete the remote branch (`git push origin --delete claude/docs-review-cloud-phase-jbbppo`).

Done when: check is clean, EditMode 103/103 and PlayMode 8/8 pass, the user has approved the layout (or listed changes), and `main` is pushed with the scene and assets.

## PC report (PC session writes)

- Merge: -
- check: -
- test (EditMode / PlayMode): -
- Builders run (and how long): -
- Screenshots (paths): -
- Compile errors fixed (what the cloud got wrong): -
- User decisions / approvals: -
- Notes for the next cloud phase: -

## Log (newest first, one line per phase half)

- 2026-10-22 - P2 cloud: village v2 ground plan in its own scene (`Village.unity`): `VillagePlan` (pure plan), `GreyboxVillageBuilder.VillageV2.cs` (+ `VillageBuilderV2.Build/Shots`), `VillagePlanTests` (9, passed offline) + `VillageV2SceneTests` (6); BuilderKit mesh cache per scene; Greybox untouched. Branch claude/docs-review-cloud-phase-jbbppo. Next: P2 PC.
- 2026-10-22 - P1 PC (via Cline): merged claude/docs-review-cloud-phase-jbbppo; `setup` added Newtonsoft 3.2.2 as a direct dependency; check clean (no fixes needed); EditMode 88/88, PlayMode 8/8; run_check.py exit 0; .meta files + manifest committed. Next: P2 cloud.
- 2026-10-22 - P1 cloud: Newtonsoft in RequiredPackages + Editor asmdef; `Scripts/Editor/Design/` (DesignSpecs, DesignData, DesignChecks); `DesignDataTests` (10). Branch claude/docs-review-cloud-phase-jbbppo. Next: P1 PC.
- 2026-10-22 - Design pack added (one-time: PC session copies docs/setup/skills/* into .claude/skills/, commits, pushes): docs/design (15 houses, village v2, assets), workflow, M1.6 in ROADMAP. Next: P1 cloud.
