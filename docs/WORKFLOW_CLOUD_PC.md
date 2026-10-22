# Hybrid workflow: heavy work in a cloud session, Unity work on the PC

Hollowmere is built by two Claude Code sessions that take turns, one phase at a time:

| | Cloud session | PC session (Local, Claude desktop app) |
|---|---|---|
| Runs on | Anthropic's cloud VM (Linux), keeps going with the laptop closed | Your PC, `C:\dev\Hollowmere\Hollowmere` |
| Can | read/write all code, docs, data, tests; run Python (`tools/housegen`), git push to a branch | everything, plus **Unity**: `tools/unity.ps1 check / test / run / shot / build`, asset downloads, the Editor |
| Cannot | run Unity (no Windows, no licence), download multi-GB assets usefully, see the Editor | (nothing missing - it is just slower for big code jobs and uses your local session) |
| Does per phase | the heavy part: design-to-code, builders, data loaders, puzzle logic + EditMode tests, scripts, docs | the Unity part: merge, compile, fix compile errors, run tests, run builders, screenshots, asset fetch/import, builds |

The baton is **`docs/HANDOFF.md`**. Its `Phase state` line says whose turn it is:

```text
CLOUD-TODO  ->  (cloud works, writes the PC checklist, pushes branch cloud/<milestone>-p<N>)  ->  PC-TODO
PC-TODO     ->  (PC merges the branch, runs Unity, fills the PC report, pushes main)           ->  DONE
DONE        ->  (cloud starts the next phase from ROADMAP.md)                                   ->  CLOUD-TODO
```

Both sessions follow `CLAUDE.md` and the project skills `/cloud-phase` and `/pc-phase` (in `.claude/skills/`).

---

## One-time setup (about 15 minutes, you do this)

1. **Give Claude's cloud access to the private repo.** Open https://claude.ai/code in a browser (same account as the desktop app). Follow the prompt to connect GitHub and **install the Claude GitHub App**; when GitHub asks which repositories, pick `yashchoudhary14/Hollowmere` (or "All repositories"). Without this, cloud sessions can't clone the private repo.
2. **Create a cloud environment for Hollowmere** (claude.ai/code -> the environment selector -> add/edit environment):
   - Name: `Hollowmere`
   - Network access: **Trusted** (default) - enough for git, pip and npm. The cloud doesn't download game assets; the PC does.
   - Environment variables (one per line):
     ```text
     GIT_LFS_SKIP_SMUDGE=1
     ```
     This makes the cloud clone only LFS *pointers*, not the ~gigabytes of textures/models/audio. Your GitHub account has 10 GiB of free LFS bandwidth per month; without this line every cloud session would spend part of it.
   - Setup script: leave empty (Python 3 is pre-installed; the session installs `matplotlib` itself if it needs to re-render reference images).
3. **Install the two skills and push everything.** In the Claude desktop app, open a **Local** Code session on `C:\dev\Hollowmere\Hollowmere` and say:
   ```text
   Copy docs/setup/skills/cloud-phase and docs/setup/skills/pc-phase into .claude/skills/, run python tools/housegen/run_check.py, then commit and push everything on main.
   ```
   (The skills are delivered under `docs/setup/` because only a session on your PC may write into `.claude/`. Cloud sessions clone from GitHub, not from your PC, so everything must be pushed.)

## The loop (what you type)

| Step | Where | You type |
|---|---|---|
| 1 | Desktop app -> Code tab -> **Cloud** -> environment `Hollowmere` -> repo `yashchoudhary14/Hollowmere` | `/cloud-phase` (first time: `Read CLAUDE.md, docs/WORKFLOW_CLOUD_PC.md and docs/HANDOFF.md, then /cloud-phase`) |
| 2 | wait until it says **"Cloud part done"** (you can close the laptop meanwhile) | - |
| 3 | Desktop app -> Code tab -> **Local** -> folder `C:\dev\Hollowmere\Hollowmere`; **close the Unity Editor** | `/pc-phase` |
| 4 | it may ask you to look at screenshots, approve the new layout, paste a token, or playtest | answer it |
| 5 | wait until it says **"PC part done"** | - |
| 6 | go back to the **same cloud session** (sidebar) | `move to the next phase` (or `/cloud-phase`) |

Repeat. Each phase is listed in `docs/ROADMAP.md` (M1.6 has P1-P8; later milestones are split the same way when they start).

Tips:
- Keep **one** cloud session per milestone. If it gets long, it auto-compacts; if it says the environment expired, just reopen it - the conversation comes back and it re-clones.
- Never run the cloud and PC parts at the same time on the same phase.
- If the PC session finds the cloud code doesn't compile, it fixes it and writes what went wrong into the PC report, so the cloud session learns.

## When cloud usage runs out -> local only

Cloud sessions use your Claude plan's usage (they share limits with everything else; there is no separate charge for the VM). When you can't or don't want to use cloud any more, open the **Local** session and type:

```text
Cloud credits are used up. Switch to local-only mode.
```

The PC session sets `Mode: local` in `docs/HANDOFF.md` and commits. From then on `/pc-phase` does **both** halves of each phase on your PC (code + Unity), straight on `main`, no branches. To go back to hybrid later: `Switch to hybrid mode.`

## Using Cline / Gemini / ChatGPT too

See `docs/AI_HELPERS.md`: Cline (your $40 credit with Claude Opus 5.5 / Claude Fable 5.1) can take over the PC part when your Claude limits are reached; Gemini and ChatGPT produce the mood images, 2D in-game art and lore text, which saves Claude usage.

## Rules both sessions follow

- **Branches:** cloud works on `cloud/<milestone>-p<N>` (e.g. `cloud/m1.6-p2`) and never pushes to `main`. PC merges it into `main` with a normal merge commit, then deletes the remote branch.
- **The PC checklist must be executable:** exact commands, which builder `-Method`, which screenshots to take and what to compare them with, which tests must pass, which user decisions are needed. No "verify things work".
- **Compile safety in the cloud:** the cloud can't compile Unity. It follows existing code patterns, uses only packages already in `BatchTools.RequiredPackages` (or adds them there + tells the PC to run `setup`), keeps editor code in `Hollowmere.EditorTools` and runtime code in `Hollowmere`, and writes EditMode tests for logic. Pure data checks run in the cloud with `python tools/housegen/run_check.py`.
- **Layout fingerprint:** only the PC session re-records `Tests/EditMode/GreyboxLayout.txt`, and only after the user approves the new layout from screenshots.
- **Assets:** downloads, imports and LFS commits happen on the PC only.
