---
name: unity-cli
description: Run Unity from the command line for this project - compile check, tests, package setup, Windows build, scene screenshots, or any editor method. Use after every C# change and before reporting work as done.
---

# Unity command line

All Unity work goes through `tools/unity.ps1`, which runs the editor in batch mode and summarises the log.

From Git Bash:

```bash
powershell.exe -NoProfile -ExecutionPolicy Bypass -File tools/unity.ps1 check
powershell.exe -NoProfile -ExecutionPolicy Bypass -File tools/unity.ps1 test
powershell.exe -NoProfile -ExecutionPolicy Bypass -File tools/unity.ps1 test -Filter MatchSetupTests
powershell.exe -NoProfile -ExecutionPolicy Bypass -File tools/unity.ps1 run -Method Hollowmere.EditorTools.GreyboxBuilder.Build   # example: a builder from M1
powershell.exe -NoProfile -ExecutionPolicy Bypass -File tools/unity.ps1 shot -Scene Assets/Hollowmere/Scenes/Greybox.unity
```

From a PowerShell shell, use `powershell -NoProfile -ExecutionPolicy Bypass -File tools/unity.ps1 check`, which works even when the execution policy blocks scripts.

## Reading results

- The script prints Unity's exit code, matching log lines (`error CS…`, exceptions, `[Hollowmere]` messages) and the full log path in `Logs/claude/`.
- `check`: exit 0 means everything compiled. Fix every `error CS` line before doing anything else. Also fix `warning CS0618` (obsolete API) when it's in our code.
- `test`: prints totals and each failed test with its message. Exit 0 means all passed, 2 means some failed, 3 means the run itself failed (usually a compile error).
- `shot`: writes a PNG (default `Logs/claude/screenshot.png`). Open it with the Read tool and look at it. Don't describe a scene you haven't seen.

## Common problems

| Message | Fix |
| --- | --- |
| `The Unity Editor has this project open` | Ask the user to close the Editor. Don't kill Unity yourself. |
| `Unity.exe not found` | Ask the user which Unity version is installed, or have them set `UNITY_EXE`. |
| `No valid Unity Editor license` | The user opens Unity Hub and signs in or activates a Personal licence. |
| Timeout after 40 min | Usually the first import or a hang. Check `log`, then rerun with `-TimeoutMinutes 60` if it was only importing. |
| `Assembly has reference to non-existent assembly` | A package is missing. Add it to `BatchTools.RequiredPackages`, run `setup`, then `check`. |

## Writing new editor entry points

- Put them in `Assets/Hollowmere/Scripts/Editor/` (builders go in `Builders/`), namespace `Hollowmere.EditorTools`, as `public static void` methods.
- A method run with `run -Method` gets `-quit` added, so it must finish synchronously. Asynchronous work (package requests, waiting on imports) needs its own entry in `tools/unity.ps1` that ends with `EditorApplication.Exit(code)`, like `BatchTools.Setup`.
- Log progress with the `[Hollowmere]` prefix so it shows in the summary.
- Read custom arguments with `BatchTools.Arg("-hmSomething")`.
