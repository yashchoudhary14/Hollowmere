# Hollowmere starter kit

This kit turns a fresh Unity project into one that Claude Code can build. It includes the design, a milestone roadmap, a command-line wrapper for Unity, the first game code with tests, and instructions that Claude Code reads automatically.

## What's in it

| Path | What it is |
| --- | --- |
| `CLAUDE.md` | Instructions Claude Code loads in every session |
| `docs/DESIGN.md` | The game design (v0.2) |
| `docs/ROADMAP.md` | Milestones M0–M8 with checklists and a Status line |
| `docs/PLAYTEST_LOG.md` | Where your playtest feedback gets recorded |
| `tools/unity.ps1` | Runs Unity in batch mode: compile, test, build, screenshot |
| `.claude/skills/` | `/milestone`, `/playtest` and the Unity command-line guide |
| `Assets/Hollowmere/` | Starting code (match setup, house catalogue, input, noise), editor tools and tests |

## Setup (about 30 minutes, mostly downloads)

1. **Install Unity.** Install Unity Hub, sign in, and install **Unity 6.3 LTS**. Add *Windows Build Support (IL2CPP)* if the Hub offers it.
2. **Create the project.** In Unity Hub, choose New project → **Universal 3D** template, name it `Hollowmere`, and save it somewhere that isn't synced by OneDrive, for example `C:\Dev\Hollowmere`. Wait for it to open, then **close the Editor**.
3. **Add the kit.** Before unzipping, right-click the zip → Properties → tick Unblock → OK. Then copy everything inside the kit folder into the project folder, next to `Assets`, `Packages` and `ProjectSettings`. `Assets/Hollowmere` should end up inside the project's `Assets` folder.
4. **Install Git for Windows** (recommended): https://git-scm.com/downloads/win. Claude Code uses it for its Bash shell and for version history. Without it, Claude Code falls back to PowerShell, which also works with this kit.
5. **Open the project in the Claude desktop app.**
   - Click the **Code** tab at the top.
   - Choose **Local**, not Cloud. Unity is on your PC, so the session has to run there too.
   - Click **Select folder** and pick the project folder, e.g. `C:\Dev\Hollowmere`.
   - Pick a permission mode next to the send button. **Accept edits** is a good start: file changes go through automatically, and Claude still asks before running commands. Once you trust the routine, **Auto** stops it asking about the Unity commands.
6. **Send the first message:** `Read CLAUDE.md, then complete M0 in docs/ROADMAP.md: run setup, check, test and info, fix anything that fails, and initialise git.`
7. Once M0 is green, type `/` in the prompt box and choose **milestone**, or just type `/milestone 1`, to start building the greybox game.

## Day-to-day

- **Keep the Unity Editor closed while Claude Code works.** Batch mode can't open a project that the Editor already has open, and the script will tell Claude to ask you.
- **To playtest:** open the project from Unity Hub, open the scene Claude names (e.g. `Assets/Hollowmere/Scenes/Greybox.unity`) and press Play. Close the Editor when you're done, then tell Claude what happened with `/playtest the monster got stuck on the chapel steps…`.
- **One session at a time on this project.** Unity can only run once per project folder. Don't start a second Code session on it, and don't use separate worktrees for sessions: each copy would need its own full Unity import, which takes a long time.
- **Reviewing changes:** click the `+12 -1` indicator after Claude edits files to see the diff, and comment on any line to ask for changes.
- **To change the design:** edit `docs/DESIGN.md`, or just tell Claude Code what you want changed. It will update the doc and the code together.
- **For the microphone mechanic:** allow desktop apps to use the mic in Windows Settings → Privacy & security → Microphone.

## Running the Unity tools yourself

Open the desktop app's terminal pane with **Ctrl+`**, or use any PowerShell window in the project folder:

```powershell
./tools/unity.ps1 check      # compile everything
./tools/unity.ps1 test       # run the tests
./tools/unity.ps1 info       # Unity version, packages, scenes
./tools/unity.ps1 open       # open the Editor
```

If PowerShell blocks the script, use `powershell -ExecutionPolicy Bypass -File tools/unity.ps1 check`.
