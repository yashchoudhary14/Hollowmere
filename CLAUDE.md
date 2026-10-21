# Hollowmere — instructions for Claude Code

Hollowmere is a 2–6 player co-op first-person horror game in Unity 6 (URP). Friends trapped in a walled village each get one house; every house hides one part of the ritual that opens the village gate. The full design is in `docs/DESIGN.md`.

You are the main programmer. The user owns the design, playtests, and handles anything that needs the Unity Editor window, accounts, or purchases.

## Read before working

- `docs/ROADMAP.md`: milestones, their checklists, and the **Status** line saying what's next. Always start here.
- `docs/DESIGN.md`: the design. It's the source of truth for rules and numbers. Don't invent mechanics that contradict it; if something is missing or unclear, add a question to its last section and ask the user.
- `docs/PLAYTEST_LOG.md`: the user's playtest feedback, newest first. Unresolved items take priority over new features.

## Environment

- Windows PC with an RTX 5080. Unity 6.3 LTS, project created from Unity Hub's **Universal 3D** template. This folder is the project root.
- You run in the **Code tab of the Claude desktop app**, in a Local session. The user isn't using a terminal, so give instructions in terms of the app and Unity Hub, never as commands for the user to type. The app's terminal pane (Ctrl+`) is there if they ever need one.
- Your shell is Git Bash if Git for Windows is installed, or PowerShell otherwise. `tools/unity.ps1` works from both (see below).
- You cannot see or click the Unity Editor. You work through code, editor scripts and the command line. The user presses Play and tells you what happened.
- **The Unity Editor must be closed** while you run batch commands. If a command reports that the Editor has the project open, ask the user to close it. Never kill Unity processes yourself.
- Only one session works on this project at a time. Don't create git worktrees for this repo: each one needs a full Unity import of its own.

## Unity command line: `tools/unity.ps1`

From Git Bash: `powershell.exe -NoProfile -ExecutionPolicy Bypass -File tools/unity.ps1 <command> [options]`
From PowerShell: `powershell -NoProfile -ExecutionPolicy Bypass -File tools/unity.ps1 <command> [options]`. This form works even when the machine's execution policy blocks scripts.

Commands can take several minutes. Wait for them to finish; don't start a second Unity command while one is running.

| Command | What it does | When |
| --- | --- | --- |
| `check` | Compiles all scripts | After every code change |
| `test` | EditMode tests (`-Platform PlayMode`, `-Filter Name`) | Before saying any task is done |
| `info` | Unity version, render pipeline, scenes, packages | Start of a session, after package changes |
| `setup` | Folders, player settings, missing packages | Once (M0), and when `RequiredPackages` changes |
| `run -Method Ns.Class.Method` | Runs a static editor method, e.g. a scene builder | Generating scenes, prefabs, assets |
| `shot -Scene <path> [-Camera <name>]` | Renders a camera to `Logs/claude/screenshot.png` | To look at a generated scene (read the PNG) |
| `build [-Out <exe>]` | Windows player build | End of milestones |
| `open` | Opens the Editor normally | Only when the user asks |
| `log` | Tail of the last Unity log | When the summary isn't enough |

- Each command prints Unity's exit code, the important log lines, and the path to the full log in `Logs/claude/`.
- `test` exit codes: 0 passed, 2 some failed, 3 couldn't run. `check` exits non-zero on compile errors.
- The first run on a fresh project imports everything and can take several minutes. That's normal.
- `shot` is new tooling. If the PNG comes out black or empty, say so instead of guessing what the scene looks like.

## How work gets built without the Editor window

- **Scenes, prefabs, materials and other assets come from editor scripts** ("builders") in `Assets/Hollowmere/Scripts/Editor/Builders/`. Each builder has a `[MenuItem("Hollowmere/...")]` so the user can run it from the Editor too, and a public static method you run with `run -Method`.
- Builders are idempotent: running one again rebuilds its output from scratch. Never hand-write `.unity`, `.prefab` or `.asset` YAML, and never edit `.meta` files or GUIDs.
- Placeholder art is primitives with URP Lit materials. Give builders clear prefab slots so real art can replace primitives later (milestone 7) without code changes.
- After generating a scene, take a `shot` and look at it before telling the user it's ready.

## Code layout

```text
Assets/Hollowmere/
  Scripts/Runtime/      Hollowmere.Runtime.asmdef, namespace Hollowmere
    Core/               match plan, catalog, noise, input, director
    Player/ World/ Puzzles/ Monster/ UI/ Net/   (create as milestones need them)
  Scripts/Editor/       Hollowmere.Editor.asmdef, namespace Hollowmere.EditorTools
    BatchTools.cs       entry points used by tools/unity.ps1
    Builders/           scene and asset generators
  Tests/EditMode/       Hollowmere.Tests.EditMode.asmdef, namespace Hollowmere.Tests
  Scenes/ Materials/ Prefabs/ Settings/ Art/ Audio/
```

## Conventions

- **Randomisation lives only in `MatchSetup`** (`Core/MatchPlan.cs`). It's pure C#, deterministic per seed, and unit-tested. Gameplay reads the resulting `MatchPlan`; it never rolls its own match-level randomness.
- **Input only through `GameInput`.** It supports both the Input System package and the legacy Input Manager.
- **Every sound the Hollow could hear goes through `NoiseSystem.Emit`**, using the radii in DESIGN.md "Player systems".
- **Tuning numbers** (speeds, timers, radii) come from DESIGN.md. Keep them as serialized fields or in one tuning ScriptableObject, with the design value as the default and a comment naming the DESIGN.md section.
- **Plan for multiplayer from the start.** Match state lives in a few authority objects (e.g. a `MatchDirector`); players send intents ("interact with X"); UI never holds game logic. Milestone 2 moves this onto Netcode for GameObjects with the host as authority, so avoid statics that hold per-match state.
- Unity 6 APIs: `FindObjectsByType` / `FindAnyObjectByType` (not `FindObjectOfType`). URP materials use shader `Universal Render Pipeline/Lit` and `_BaseColor`.
- NavMesh: use the AI Navigation package (`NavMeshSurface`), baked by the builder after geometry exists. Exclude door panels from the bake (`NavMeshModifier` with ignore-from-build) and give them carving `NavMeshObstacle`s. Those obstacles carve as soon as the NavMesh loads, in edit mode too, so a test that needs open doorways must disable them and reload the surface's data (see `GreyboxSceneTests`).
- Until the UI milestone, HUD and debug UI use `OnGUI`, which needs no extra assets. Avoid TextMeshPro until then; it needs an Essential Resources import.
- Pure logic gets EditMode tests. Behaviour that needs a running scene gets a PlayMode test when that's practical.
- Packages are added through `BatchTools.RequiredPackages` + `setup`, not by hand-editing versions in `Packages/manifest.json`.
- If git is set up, commit after each green `check` + `test`, with a clear message.

## Working loop

1. Read the ROADMAP Status line and pick the next unchecked item in the current milestone.
2. Implement in small steps, running `check` after each one.
3. Add or extend tests, then run `test`.
4. Tick the item in ROADMAP.md and update the Status line.
5. At the end of a milestone, run `build`, then give the user a short playtest script: open the project in Unity Hub, which scene to open, which keys to use, and what to look for. Remind them to close the Editor afterwards. Wait for their feedback, which they log with `/playtest`.

Ask the user, rather than guessing, about design decisions DESIGN.md doesn't settle, about buying or importing assets, and about accounts (Unity Cloud project for Relay and Vivox, Steamworks).
