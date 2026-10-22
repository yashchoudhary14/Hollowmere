---
name: pc-phase
description: Do the PC half of the current Hollowmere phase (merge the cloud branch, Unity compile/test/build/screenshots, asset downloads) - or, in local-only mode, the whole phase.
argument-hint: [optional notes from the user]
disable-model-invocation: true
---

You are the PC (Local) session for Hollowmere on the user's Windows PC. Follow `CLAUDE.md`, `.claude/skills/unity-cli` and `docs/WORKFLOW_CLOUD_PC.md`.

If the user says cloud credits/usage are used up (or asks for local-only mode): set `Mode: local` in `docs/HANDOFF.md`, add a Log line, commit and push, and continue below in local mode. "Switch to hybrid mode" sets it back.

**Hybrid mode (`Mode: hybrid`)**

1. Read `docs/HANDOFF.md`. If `Phase state` is not `PC-TODO`, tell the user what the state is (e.g. "the cloud part isn't done yet - go to the cloud session and say: move to the next phase") and stop.
2. Make sure the Unity Editor is closed (ask the user if a command says it's open). `git fetch origin`, `git checkout main`, `git pull`, then `git merge --no-ff origin/<Cloud branch>`; resolve conflicts (prefer the cloud's code, keep local asset/meta files).
3. Work through the PC checklist in order, ticking each item in HANDOFF.md:
   - `tools/unity.ps1 setup` when packages changed, then `check`. **Fix every compile error yourself** and note in the PC report what the cloud got wrong.
   - `test` (EditMode, and PlayMode when the checklist says) - fix failures; ask the user before changing design.
   - Builders via `run -Method`, then `shot`s; open each PNG and compare with the listed `docs/design/ref/*.png`; iterate until they match or note the difference.
   - Asset scripts (`fetch-assets.ps1`, `fetch-sketchfab.ps1`, `import-assetstore.ps1`, `prepare-audio.ps1`, `write-credits.ps1`) when listed.
   - Ask the user for anything marked as a user step (approve screenshots, give a token, playtest), in plain words for someone using the Claude desktop app and Unity Hub.
4. Fill the PC report (results, test counts, screenshot paths, fixes, user decisions, notes for the next cloud phase). Set `Phase state: DONE`, add a Log line.
5. Commit on `main` with a clear message, `git push origin main`, then `git push origin --delete <Cloud branch>`.
6. Tell the user exactly: "PC part of <phase> done. Go back to the cloud session and type: move to the next phase."

**Local mode (`Mode: local`)**

Do the whole phase here: the cloud items from ROADMAP.md (as `/cloud-phase` would, but on `main`, no branch), then the PC items, with `check` after every code step. Keep HANDOFF.md updated (both sections) so hybrid mode can resume later. Finish with: "Phase <N> done. Type /pc-phase for the next one."
