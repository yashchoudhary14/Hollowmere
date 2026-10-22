# housegen - the source of the village & house design data

Python 3 (only `matplotlib` is needed, for the reference images). These scripts are the **source** of everything in
`docs/design/`; the Markdown/JSON/PNG files there are generated from them. Edit here, then regenerate - never hand-edit
`docs/design/houses/*.json` or `village.json`.

| File | What it holds |
|---|---|
| `catalog.py` | Every asset key: textures (MATERIALS), models (MODELS: Poly Haven, Fab, Sketchfab, procedural `BUILD`), audio. Dimensions, licence, who downloads it, paid upgrade. |
| `core.py` | `House`/`Room` data model, placement helpers (wall, corner, on, hung...), and `validate()` (overlaps, door clearance, stairs, DESIGN shared-template checks). |
| `kit.py` | Shorthands used by the house files: levels, hiding spots, items, notes, beds, shrines, puzzles, containers. |
| `houses_a.py`, `houses_b.py`, `houses_c.py` | `h00()`..`h14()`: the 15 houses, room by room, prop by prop. |
| `house_text.py` | Per-house exterior/garden text, puzzle steps, night changes, audio, references and image prompts. |
| `houses_colour.py` | Door colours for the renders. |
| `village.py` | Village v2: house placements, landmarks, filler buildings, roads, stream, bridges, blockers, scarecrows, lamps + checks. |
| `render_house.py`, `render_village.py` | Plan, massing and map PNGs in `docs/design/ref/`. |
| `gen_docs.py` | Writes all of `docs/design/`, and merges `tools/third-party.json` + `tools/sketchfab.json`. |
| `run_check.py` | Validates all houses and the village; exit code 1 on any error. |
| `gen_requests.py` | Writes `docs/design/ART_AND_LORE_REQUESTS.md` (images and texts for Gemini/ChatGPT, see `docs/AI_HELPERS.md`). |

From the repo root:

```bash
pip install --break-system-packages matplotlib     # cloud session, once (PC: pip install matplotlib)
python tools/housegen/run_check.py                 # must print 0 errors / 0 village problems
python tools/housegen/gen_docs.py . tools/third-party.json
python tools/housegen/gen_requests.py .
```

`gen_docs.py` is idempotent: re-running it with the already-merged `tools/third-party.json` changes nothing.
In the cloud (Linux) use `python3`; on Windows use `py` if `python` isn't on PATH.
