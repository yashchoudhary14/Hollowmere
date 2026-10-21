---
name: milestone
description: Work through a roadmap milestone for Hollowmere, item by item, with compile and test checks after each step.
argument-hint: [milestone number, e.g. 1]
disable-model-invocation: true
---

Work on milestone $ARGUMENTS of `docs/ROADMAP.md`. If no number is given, use the one on the Status line.

1. Read `docs/ROADMAP.md`, the DESIGN.md sections that milestone names in brackets, and any `open` items in `docs/PLAYTEST_LOG.md`.
2. Write a short plan in chat: the order you'll do the unchecked items in, which files you'll create, and anything you need from the user. If an item needs a design decision that DESIGN.md doesn't settle, ask before building it.
3. For each item:
   - Implement it in small steps, running `tools/unity.ps1 check` after each step and fixing every error before moving on.
   - Add tests for logic, then run `tools/unity.ps1 test`.
   - If it generates a scene, run the builder with `run -Method`, take a `shot`, and look at the PNG.
   - Tick the item in ROADMAP.md, update the Status line, and commit if git is set up.
4. When every item is ticked, run `tools/unity.ps1 build`. Then give the user a playtest script: which scene to open, the controls, what to try, and what to look for. Phrase it for someone using Unity Hub and the Claude desktop app, not a terminal. Remind them to close the Editor afterwards and to report back with `/playtest`.
