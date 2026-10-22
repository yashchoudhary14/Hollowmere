# Other AI helpers: Cline, Gemini Pro, ChatGPT Go

Claude Code (cloud + PC sessions, `docs/WORKFLOW_CLOUD_PC.md`) stays the main programmer. These helpers take load off it:

| Helper | Best used for | Not for |
|---|---|---|
| **Cline** in VS Code, paid from your ~$40 Cline credit, running **Claude Opus 5.5** (default) or **Claude Fable 5.1** (hard problems) | Doing a **PC half** (or a whole phase in local mode) when your Claude plan limit is reached and you don't want to wait | Running at the same time as a Claude Code session on this repo |
| **Gemini** (Google AI Pro): the Gemini app, optionally Gemini CLI | Images (mood images, family photos, papers, signs), lore text, a second-opinion code review | Editing C# or running Unity |
| **ChatGPT Go** | Images and image *edits* (the day -> night family photos), lore text, short questions | Repo work (Go's Codex access is limited) |

What to make with Gemini/ChatGPT is listed, file by file with ready prompts, in **`docs/design/ART_AND_LORE_REQUESTS.md`**.

---

## 1. Cline with Opus 5.5 / Fable 5.1 (step by step)

Prices on the API (Cline bills your credit at these rates; check the model picker for the current numbers): **Claude Opus 5.5 $4 / $20** per million input / output tokens, **Claude Fable 5.1 $10 / $50** (both 1M-token context). An agent task re-reads files a lot, so input dominates. Rough guide: a PC phase is usually **$2-8 with Opus 5.5** and about 2.5x that with Fable 5.1, so $40 is roughly 5-15 phase halves on Opus. Prompt caching (on by default for Anthropic models in Cline) cuts repeat reads a lot.

### One-time setup (10 minutes)

1. Install **VS Code** from https://code.visualstudio.com (Windows, User installer, default options).
2. Open VS Code -> Extensions (the four-squares icon on the left, or Ctrl+Shift+X) -> search **Cline** -> Install (publisher: Cline / saoudrizwan). A Cline icon appears in the left bar.
3. Click the Cline icon -> **Sign in** (or the settings gear -> **API Provider: Cline**) and sign in with the account that holds your credit. Your balance is shown in Cline's account view and at https://app.cline.bot (dashboard).
4. In Cline's settings, open the **Model** dropdown and search `opus`. Pick **Claude Opus 5.5** (shown as `anthropic/claude-opus-5-5` or similar). If you only see older models, update the Cline extension (Extensions -> Cline -> Update); if Opus 5.5 still isn't listed, pick the newest Claude Opus shown.
   - Optional: Cline lets you choose a different model for **Plan** and **Act** mode. Good split: Plan = Fable 5.1 (thinking), Act = Opus 5.5 (doing). Or Opus 5.5 for both to save credit.
5. File -> Open Folder -> `C:\dev\Hollowmere\Hollowmere`. Cline reads `.clinerules` in the project automatically; it points Cline to `CLAUDE.md` and this workflow.
6. In Cline's settings, turn **Auto-approve** on for *read files* only. Leave *edit files* and *execute commands* on manual at first, so you see every change; once you trust it, allow commands too (Unity commands take minutes, keep the command timeout long).

### Using it for a phase

1. Make sure no Claude Code session is working on the project, and the Unity Editor is closed.
2. In Cline, start a **new task** (the + button) in **Plan** mode and type:
   ```text
   Read .clinerules, CLAUDE.md, docs/WORKFLOW_CLOUD_PC.md and docs/HANDOFF.md.
   Do the PC half of the current phase exactly as .claude/skills/pc-phase/SKILL.md describes. Make a plan first.
   ```
   (For a whole phase in local mode: `Do the current phase in local mode as .claude/skills/pc-phase/SKILL.md describes.`)
3. Read the plan. If it looks right, switch to **Act** mode and say `Go ahead.` Approve file edits and commands as they come (Cline shows a diff for each edit).
4. When it finishes, check that it filled the **PC report** in `docs/HANDOFF.md`, ticked ROADMAP and committed. Then carry on as usual (cloud session: `move to the next phase`).
5. Keep tasks short: one phase half per task. Start a new task for the next one; long tasks re-send a growing history and cost more.

### Choosing the model

- **Opus 5.5** (default): compile fixes, running builders, comparing screenshots, asset scripts, most C#.
- **Fable 5.1**: when Opus has failed twice on the same problem (a puzzle rig's logic, a NavMesh bug, a nasty merge). Switch back afterwards.
- Watch the cost counter at the top of the task. If a single task passes ~$5 on Opus without progress, stop it and ask for a summary of what's blocking.

---

## 2. Gemini (Google AI Pro) - images, text, review

### a) Images in the Gemini app (no setup)

1. Open https://gemini.google.com (signed in with the Google account that has AI Pro).
2. Open `docs/design/ART_AND_LORE_REQUESTS.md` (in VS Code, or on GitHub). Start with section **A** (mood images): for each house, paste the prompt from `docs/design/mood_prompts.txt` and add `16:9, photoreal`.
3. Generate 2-4 versions, download the best, and save it with the exact file name into the folder given (`docs/design/ref/` for mood images, `Assets/Hollowmere/Art/Incoming/...` for game images). Create missing folders in File Explorer.
4. Section **C** (papers, drawings, signs): one chat per file, prompt as given. Check spelling on signs.
5. Section **B** (family photos): make the day photo, then in the **same chat** ask for the edit to the night version, so the people stay the same. (ChatGPT is also good at this; use whichever keeps faces more consistent.)
6. When a batch is saved, tell Claude Code on the PC: `New images are in Assets/Hollowmere/Art/Incoming. Import them as the requests file says.`

### b) Lore text in the Gemini app

1. New chat. Upload the house page, e.g. `docs/design/houses/H07_organist_s_house.md`, and `docs/design/ART_AND_LORE_REQUESTS.md`.
2. Type: `Write the 3 notes for H07 following section D of the requests file. Use the format shown there. Put puzzle variables in braces, never the answer.`
3. Read it, ask for changes if needed, then copy the result into Notepad and save as `docs/lore/incoming/H07_notes.md` (Save as type: All files, encoding UTF-8).
4. Repeat for all 15 houses (one chat per house keeps quality up). Then tell Claude Code: `Lore notes are in docs/lore/incoming. Wire them to the note spots.`

### c) Optional: Gemini CLI for the same work without copy-paste

Needs Node.js (https://nodejs.org, LTS installer). Then in VS Code: Terminal -> New Terminal, and type `npm install -g @google/gemini-cli`, then `cd C:\dev\Hollowmere\Hollowmere` and `gemini`. Sign in with Google when asked (AI Pro gives it higher limits). Useful prompts:
- `Read docs/design/ART_AND_LORE_REQUESTS.md section D and docs/design/houses/H03_*.md. Write docs/lore/incoming/H03_notes.md. Only create files under docs/lore/incoming. Do not run git.`
- `Review the last commit (git show HEAD) for bugs. Do not change any files; just list problems with file and line.` Paste anything real into the Claude Code session.

Rules for Gemini CLI: only writes under `docs/lore/incoming/` or `Assets/Hollowmere/Art/Incoming/`, never commits, never runs Unity, never at the same time as a Claude Code or Cline session that is merging.

---

## 3. ChatGPT Go - images and text

1. https://chatgpt.com, signed in to your Go account.
2. Images: same as Gemini (section 2a). ChatGPT's image *editing* is strong for the day -> night family photo pairs: upload or generate the day photo, then `Edit this image: add one extra person...` (exact text in the requests file).
3. Text: same as 2b - upload the house page and the requests file, ask for that house's notes, save into `docs/lore/incoming/`.
4. Codex on Go is limited: don't use it on this repo.

Splitting the work so neither runs out: Gemini does sections A and C and half of D; ChatGPT does section B (photos + edits) and the other half of D.

---

## What each helper must never do

- Commit, push, merge or touch `main` (only Claude Code sessions and Cline running the `/pc-phase` steps do).
- Edit `.meta`, `.unity`, `.prefab` or `.asset` files, or `docs/design/houses/*.json` (those come from `tools/housegen`).
- Put real people, brand logos or copyrighted characters into images.
- Receive secrets: never paste a token (Sketchfab, GitHub, Unity) into any chat.
