---
name: cloud-phase
description: Do the CLOUD half of the current Hollowmere phase (heavy code, data, tests, docs) in a cloud session, then hand over to the PC session through docs/HANDOFF.md.
argument-hint: [optional: phase id to force, e.g. P3]
---

You are the CLOUD session for Hollowmere. You cannot run Unity. Follow `CLAUDE.md` and `docs/WORKFLOW_CLOUD_PC.md`.

1. **Sync.** `git fetch origin` then `git checkout main && git pull --ff-only origin main` (LFS smudge is skipped by the environment; that is expected - binaries are pointers here). If `docs/HANDOFF.md` says `Mode: local`, stop and tell the user this project is in local-only mode.
2. **Whose turn?** Read `docs/HANDOFF.md`.
   - `PC-TODO`: the PC half isn't done. Tell the user: "The PC part of <phase> isn't done yet - open the Local session and run /pc-phase." Stop.
   - `DONE`: read the PC report (compile errors you caused, test results, user decisions, notes). Advance `Current phase` to the next unchecked phase in `docs/ROADMAP.md` (or $ARGUMENTS if given) and set `CLOUD-TODO`.
   - `CLOUD-TODO`: continue with the current phase.
3. **Branch.** `git checkout -b cloud/<milestone>-p<N>` (e.g. `cloud/m1.6-p2`); if it exists remotely, check it out and merge `origin/main` into it.
4. **Plan briefly in chat** (the phase's cloud items from ROADMAP.md, files you'll touch, anything that needs a user decision - ask now, not later).
5. **Do the cloud items.** Rules:
   - Design data lives in `docs/design/` (HOUSES.md, houses/Hxx.json, VILLAGE_LAYOUT.md, village.json, ASSETS.md, asset_catalog.json). If you change data, change it in `tools/housegen/*.py`, then run `python3 tools/housegen/run_check.py` (exit 0 required), then `python3 tools/housegen/gen_docs.py . tools/third-party.json` and `python3 tools/housegen/gen_requests.py .`. Install matplotlib with `pip install --break-system-packages matplotlib` if needed.
   - Compile safety: copy existing patterns; Unity 6 APIs (`FindObjectsByType`); URP Lit `_BaseColor`; `Hollowmere.EditorTools` for editor code, `Hollowmere` for runtime; new packages only via `BatchTools.RequiredPackages` (tell the PC to run `setup`). Re-read every file you edit for missing `using`s, braces and asmdef references. No `.meta`, `.unity`, `.prefab` or `.asset` hand-edits.
   - Logic gets EditMode tests. Randomness only in `MatchSetup` (MatchPlan).
   - Tick the cloud items in ROADMAP.md.
6. **Write the PC checklist** in `docs/HANDOFF.md`: exact steps in order - merge command, `tools/unity.ps1 setup/check/test` calls, `run -Method` entries, `fetch-assets.ps1` etc., which `shot`s to take and which `docs/design/ref/*.png` to compare them with, what the user must approve or provide, what counts as done. Fill "Cloud summary". Set `Phase state: PC-TODO` and `Cloud branch:`. Add a Log line.
7. **Commit and push** the branch (`git push -u origin cloud/<...>`). Do not push to main.
8. **Tell the user** exactly: "Cloud part of <phase> done (branch <name>). Open the Local session on C:\dev\Hollowmere\Hollowmere, close the Unity Editor, and type /pc-phase."
