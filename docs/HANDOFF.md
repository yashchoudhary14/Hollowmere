# Handoff (cloud session <-> PC session)

Read `docs/WORKFLOW_CLOUD_PC.md` first. Each session edits only its own sections, then commits this file.

```text
Mode:          hybrid          # hybrid = cloud writes code, PC runs Unity | local = the PC session does both halves
Milestone:     M1.6 Village & houses rebuild (docs/ROADMAP.md)
Current phase: P1 - Data model, loader and validation
Phase state:   PC-TODO         # CLOUD-TODO -> PC-TODO -> DONE -> (next phase) CLOUD-TODO
Cloud branch:  claude/docs-review-cloud-phase-jbbppo
```

## Cloud summary for this phase (cloud session writes)

- Branch name: this cloud session was assigned `claude/docs-review-cloud-phase-jbbppo` by the harness and may only push there, so it is used instead of `cloud/m1.6-p1`. Merge it the same way.
- `BatchTools.RequiredPackages` += `com.unity.nuget.newtonsoft-json`. It is **already installed** as an indirect dependency (packages-lock: 3.2.2, depth 1), so the Editor assembly compiles even before `setup`; `setup` just makes it a direct dependency.
- `Hollowmere.Editor.asmdef`: `overrideReferences: true`, `precompiledReferences: ["Newtonsoft.Json.dll"]`. No other editor code used a precompiled DLL (checked: no NUnit/TestRunner/other DLL usings in `Scripts/Editor`).
- New folder `Assets/Hollowmere/Scripts/Editor/Design/` (namespace `Hollowmere.EditorTools`):
  - `DesignSpecs.cs`: plain classes for every key in `houses/Hxx.json`, `village.json` and `asset_catalog.json` (`HouseSpec`, `HouseWorldSpec`, `LevelSpec`, `WingSpec`, `RoomSpec`, `OpeningSpec` -> `DoorSpec`/`WindowSpec`, `StairSpec`, `ChimneySpec`, `PropSpec`, `AnchorSpec`, `VillageSpec`, `RoadSpec`, `LandmarkSpec`, `FillerSpec`, `StreamSpec`, `BridgeSpec`, `BlockerSpec`, `AssetCatalogSpec`...). PascalCase fields map to the JSON's snake_case keys.
  - `DesignData.cs`: `LoadHouse(slug|index)`, `LoadHouses()`, `LoadVillage()`, `LoadCatalog()`, `Parse<T>()`. Path = `Application.dataPath/../docs/design`. Uses `SnakeCaseNamingStrategy` + `MissingMemberHandling.Error` (strict: an unmapped JSON key fails the load, so a housegen schema change is never lost silently). Newtonsoft types stay private, so the test assembly needs no Newtonsoft reference.
  - `DesignChecks.cs`: C# twin of `core.py validate()` (identity vs `HouseCatalog`, rooms in wings, no overlaps, doors/windows/stairs, asset keys, DESIGN's shared template) and of `village.py build()` (footprints 1.5 m apart, plots clear, `to_altar`, the front-door placement formula, blocker keys). Public geometry helpers (`Rotate`, `ToWorld`, `WingPoly`, `RectPoly`, `PolysOverlap`, `Neighbours`, `IsExteriorEdge`, `TryFrontDoorLocal`) are meant for the P2/P3 builders.
- New `Assets/Hollowmere/Tests/EditMode/DesignDataTests.cs`: 10 tests (load, HouseCatalog match, rooms in wings, doors, asset keys, shared template, village footprints, strict snake_case loader, negative controls that break the data and expect errors, `Rotate` == Unity Y rotation).
- Checked in the cloud without Unity: a Python mirror of `DesignChecks`, run on the generated JSON with float32 rounding, gives 0 problems on all 15 houses and the village. The negative controls fail as expected, and front doors land within 3 µm of `village.json`. A script also confirmed that every JSON key maps to a field under Newtonsoft's snake_case rule. `python3 tools/housegen/run_check.py` exits 0. No data was changed.
- Not touched: scenes, builders, `GreyboxLayout.txt`, any `.meta` / `.asset`. Nothing visual this phase.

## PC checklist for this phase (cloud session writes; PC session ticks)

1. [ ] User closes the Unity Editor.
2. [ ] Merge: `git checkout main && git pull --ff-only origin main && git fetch origin && git merge --no-ff origin/claude/docs-review-cloud-phase-jbbppo -m "Merge M1.6 P1 (cloud): DesignData loader and DesignDataTests"`.
3. [ ] `tools/unity.ps1 setup`: expect `Adding packages: com.unity.nuget.newtonsoft-json`, then `SETUP OK`. (If it says all packages are already installed, that's fine too.)
4. [ ] `tools/unity.ps1 check`: 0 compile errors. If it fails with `Newtonsoft` not found: confirm `Packages/manifest.json` now lists `com.unity.nuget.newtonsoft-json` and that `Library/PackageCache/com.unity.nuget.newtonsoft-json*/Runtime/Newtonsoft.Json.dll` exists. Fix and record in the PC report what the cloud got wrong.
5. [ ] `tools/unity.ps1 test`: EditMode green. Expected: **88** (78 before + 10 `DesignDataTests`). If a `DesignDataTests` check fails on the data, paste its message into the report. Do **not** hand-edit `docs/design/*.json`; the cloud fixes the rule or `tools/housegen`.
6. [ ] `tools/unity.ps1 test -Platform PlayMode`: 8 green (unchanged).
7. [ ] `python tools/housegen/run_check.py` (or `py`): exit 0.
8. [ ] Commit the Unity-generated `.meta` files: `Assets/Hollowmere/Scripts/Editor/Design.meta`, `Design/DesignSpecs.cs.meta`, `Design/DesignData.cs.meta`, `Design/DesignChecks.cs.meta`, `Tests/EditMode/DesignDataTests.cs.meta`, plus `Packages/manifest.json` / `packages-lock.json` from `setup`. Commit message: "M1.6 P1: DesignData loader, Newtonsoft, DesignDataTests (PC verified)".
9. [ ] No builders, no screenshots, no fingerprint re-record this phase (`LookPassTests` must pass unchanged).
10. [ ] Tick the P1 PC item in `docs/ROADMAP.md`, fill the PC report below, set `Phase state: DONE`, add a Log line, push `main`, delete the remote branch (`git push origin --delete claude/docs-review-cloud-phase-jbbppo`).

Done when: check is clean, EditMode 88/88 and PlayMode 8/8 pass, and `main` is pushed with the .meta files.

## PC report (PC session writes)

- Merge: -
- check: -
- test (EditMode / PlayMode): -
- Builders run: -
- Screenshots (paths): -
- Compile errors fixed (what the cloud got wrong): -
- User decisions / approvals: -
- Notes for the next cloud phase: -

## Log (newest first, one line per phase half)

- 2026-10-22 - P1 cloud: Newtonsoft in RequiredPackages + Editor asmdef; `Scripts/Editor/Design/` (DesignSpecs, DesignData, DesignChecks); `DesignDataTests` (10). Branch claude/docs-review-cloud-phase-jbbppo. Next: P1 PC.
- 2026-10-22 - Design pack added (one-time: PC session copies docs/setup/skills/* into .claude/skills/, commits, pushes): docs/design (15 houses, village v2, assets), workflow, M1.6 in ROADMAP. Next: P1 cloud.
