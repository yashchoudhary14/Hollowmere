"""Generates every Hollowmere design deliverable from the data modules:
   docs/design/HOUSES.md, docs/design/houses/Hxx_*.md + .json, docs/design/VILLAGE_LAYOUT.md + village.json,
   docs/design/ASSETS.md, docs/design/asset_catalog.json, tools/third-party.json (merged), tools/sketchfab.json,
   docs/design/REFERENCE_IMAGES.md and docs/design/ref/*.png
Run:  python tools/housegen/gen_docs.py <repo-root>
"""
import json, os, sys, re, shutil, math
from collections import defaultdict
from core import validate
from catalog import ALL, MATERIALS, MODELS, AUDIO, SONNISS
from village import build, VILLAGE_KEYS, PLACEMENT, ROADS, STREAM, BRIDGES, BLOCKERS, SCARECROWS, STREET_LAMPS, LANDMARKS, FILLER
from house_text import TEXT, STYLE
import render_house, render_village

ROOT = sys.argv[1] if len(sys.argv) > 1 else "."
D = os.path.join(ROOT, "docs", "design")
os.makedirs(os.path.join(D, "houses"), exist_ok=True)
os.makedirs(os.path.join(D, "ref"), exist_ok=True)
os.makedirs(os.path.join(ROOT, "tools"), exist_ok=True)

houses, vill = build()
VH = {v["slug"]: v for v in vill["houses"]}


def slugname(h):
    return f"{h.slug}_{re.sub('[^a-z0-9]+', '_', h.name.lower()).strip('_')}"


def src_cell(key):
    e = ALL[key]
    s = e["source"]
    if s == "polyhaven":
        return f"Poly Haven `{e['id']}` ({e['res']})"
    if s == "ambientcg":
        return f"ambientCG `{e['id']}` ({e['res']})"
    if s == "sketchfab":
        return f"Sketchfab `{e['id']}` (CC BY, {e.get('author')})"
    if s == "procedural":
        return "BUILD (PropKit)"
    return f"owned: {e['id']}"


def f2(v):
    return f"{v:.2f}"


# ===================================================================================================== per-house JSON
def house_json(h):
    v = VH[h.slug]
    return dict(
        schema="hollowmere.house/1",
        slug=h.slug, index=h.idx, name=h.name, family=h.family, part=h.part, puzzle_kind_m1=h.puzzle_kind,
        door_colour=h.door_colour, chimneys=h.chimneys, clock_hour=h.clock_hour, loud_puzzle=h.loud,
        archetype=h.traits["archetype"], silhouette=h.traits["silhouette"],
        world=dict(position=[v["pos"][0], v["ground"], v["pos"][1]], yaw=v["yaw"], street=v["street"], plot_poly=v["plot"], note=v["note"]),
        conventions="metres; house-local; origin = (0,0,0) at ground-floor level, centre of the main wing; +z = FRONT (street side); "
                    "+x = on your LEFT when you stand in the street facing the front door; yaw = Unity Y rotation, 0 = the prop's front "
                    "faces +z, 90 = +x. See HOUSES.md 'Coordinates'.",
        levels=h.levels,
        wings=h.wings,
        rooms=[dict(id=r.id, name=r.name, level=r.level, rect=r.rect, inner=[round(c, 3) for c in r.inner()], kind=r.kind,
                    floor=r.floor, walls=r.walls, ceiling=r.ceiling, light=r.light, skirting=r.skirting, note=r.note) for r in h.rooms.values()],
        doors=h.doors, windows=h.windows, stairs=h.stairs, chimneys_detail=h.chimney_stacks,
        props=[dict(p, asset=ALL[p["key"]]["id"], source=ALL[p["key"]]["source"], dims=ALL[p["key"]].get("dims")) for p in h.props],
        anchors=h.anchors, exterior=TEXT[h.slug]["exterior"] + h.exterior, plot=TEXT[h.slug]["plot"],
        puzzle=TEXT[h.slug]["puzzle"], night_changes=TEXT[h.slug]["night"], audio=TEXT[h.slug]["audio"],
        references=TEXT[h.slug]["refs"], image_prompt=f"{TEXT[h.slug]['prompt']}, {STYLE}")


# ===================================================================================================== per-house markdown
def house_md(h):
    v = VH[h.slug]; T = TEXT[h.slug]; sn = slugname(h)
    L = []
    a = L.append
    a(f"# {h.slug} - {h.name} ({h.family})")
    a("")
    a(f"> Machine-readable twin: `docs/design/houses/{h.slug}.json` - the builder reads THAT file; this page explains it. "
      f"Every number here comes from the same data and is validated (no overlaps, door clearances, required items).")
    a("")
    a("| | |\n|---|---|")
    for k, val in (("Archetype", h.traits["archetype"]), ("Silhouette (must read at 50 m in fog)", h.traits["silhouette"]),
                   ("Ritual part", h.part), ("M1 puzzle rig (code)", f"`PuzzleKind.{h.puzzle_kind}`" + (" (loud)" if h.loud else "")),
                   ("Letter traits (HouseCatalog)", f"door **{h.door_colour}**, **{h.chimneys}** chimney(s), stopped clock **{h.clock_hour}:00**, family **{h.family}**"),
                   ("World position", f"x {v['pos'][0]}, z {v['pos'][1]}, floor level y {v['ground']:+.2f}, yaw {v['yaw']} deg - {v['street']}"),
                   ("Front door (world)", f"({v['front_door'][0]:.1f}, {v['front_door'][1]:.1f}) - {v['to_altar']} m from the altar, {v['door_to_road']} m from the lane"),
                   ("Placement note", v["note"])):
        a(f"| **{k}** | {val} |")
    a("")
    a("## Reference images")
    a("")
    a(f"![plan](../ref/{h.slug}_plan.png)")
    a("")
    a(f"![front-left](../ref/{h.slug}_massing_front.png) ![back-right](../ref/{h.slug}_massing_back.png)")
    a("")
    a("Real-world references (look, don't copy): " + ", ".join(f"[{u.rsplit('/', 1)[-1].replace('_', ' ')}]({u})" for u in T["refs"]))
    a("")
    a("Mood-image prompt (Higgsfield / Gemini / ChatGPT image - save the result as "
      f"`docs/design/ref/{h.slug}_mood.png`):")
    a("")
    a(f"```text\n{T['prompt']}, {STYLE}\n```")
    a("")
    a("## 1. Structure")
    a("")
    a("### Levels")
    a("| Level | Floor y (m) | Clear height (m) |\n|---|---|---|")
    for k, lv in h.levels.items():
        a(f"| {k} - {lv['label']} | {lv['y']:+.2f} | {lv['h']:.2f} |")
    a("")
    a("Floor build-up: 0.25 m between floors (joists + boards); ceilings use the room's `ceiling` material.")
    a("")
    a("### Wings (massing)")
    a("| Wing | Rect x0,z0 -> x1,z1 | Size | Levels | Walls | Wall thickness | Roof |\n|---|---|---|---|---|---|---|")
    for w in h.wings:
        r = w["rect"]
        a(f"| {w['id']} | ({r[0]}, {r[1]}) -> ({r[2]}, {r[3]}) | {r[2]-r[0]:.1f} x {r[3]-r[1]:.1f} | {', '.join(w['levels'])} | {w['walls']} | {w['t']} | {w['roof']} |")
    a("")
    a("### Chimneys (count must equal HouseCatalog = " + str(h.chimneys) + ")")
    a("| x | z | stack w x d | above ridge | pots | serves |\n|---|---|---|---|---|---|")
    for c in h.chimney_stacks:
        a(f"| {c['x']} | {c['z']} | {c['w']} x {c['d']} | {c['above_ridge']} | {c['pots']} | {c['serves']} |")
    a("")
    a("### Exterior details")
    for e in T["exterior"]:
        a(f"- {e}")
    a("")
    a("### Plot, garden and yard")
    for e in T["plot"]:
        a(f"- {e}")
    a(f"- Plot boundary (world polygon): {', '.join(f'({x:.1f},{z:.1f})' for x, z in v['plot'])}. Boundary = DryStoneWall / FencePicket "
      "on the street side, FenceRail or hedges (Shrub1-4) elsewhere, a GardenGate on every path.")
    a("")
    a("## 2. Rooms")
    a("")
    a("| Room | Level | Rect (x0,z0,x1,z1) | Clear inside | Kind | Floor | Walls | Ceiling | Light |\n|---|---|---|---|---|---|---|---|---|")
    for r in h.rooms.values():
        i = r.inner()
        a(f"| **{r.id}** {r.name} | {r.level} | ({r.rect[0]}, {r.rect[1]}, {r.rect[2]}, {r.rect[3]}) | {i[2]-i[0]:.2f} x {i[3]-i[1]:.2f} | {r.kind} | "
          f"{r.floor} | {r.walls} | {r.ceiling} | {r.light}{(' - ' + r.note) if r.note else ''} |")
    a("")
    a("Skirting: 0.15 m dark oak in every room except kitchens/sculleries/cellars (none) and bathrooms (tile). "
      "Every ground-floor room has one LightSwitch by its main door at 1.3 m; every room has the existing `Room` component.")
    a("")
    a("### Doors (how the rooms connect)")
    a("| Door | Level | From -> to | Wall | Centre at | W x H | Leaf | Role |\n|---|---|---|---|---|---|---|---|")
    for d in h.doors:
        a(f"| {d['id']} | {d['level']} | {d['room_a']} -> {d['room_b']} | {d['side']} | {d['at']} | {d['width']} x {d['height']} | "
          f"{d['kind']}{(' (' + d['colour'] + ')') if d.get('colour') else ''}{(' - ' + d['note']) if d.get('note') else ''} | {d['role']} |")
    a("")
    a("### Windows")
    a("| Window | Room | Wall | Centre at | W x H | Sill | Style | Weak | Curtains / shutters |\n|---|---|---|---|---|---|---|---|---|")
    for w in h.windows:
        a(f"| {w['id']} | {w['room']} | {w['side']} | {w['at']} | {w['width']} x {w['height']} | {w['sill']} | {w['style']} | "
          f"{'**WEAK** (break-in point)' if w['weak'] else ''} | {w.get('curtains') or ''} {('/ shutters: ' + w['shutters']) if w.get('shutters') else ''} |")
    a("")
    a("### Stairs, ladders and voids")
    a("| Id | In room | Rect | Goes | Up toward | Type |\n|---|---|---|---|---|---|")
    for s in h.stairs:
        a(f"| {s['id']} | {s['room']} | {tuple(s['rect'])} | {s['frm']} -> {s['to']} | {s['ascent']} | {s['kind']}{(' - ' + s['note']) if s.get('note') else ''} |")
    a("")
    a("## 3. Every prop, room by room")
    a("")
    a("Position = prop **pivot at the centre of its footprint, on the floor** (or on top of its parent for `on`, or the centre height `y` for wall-hung). "
      "Yaw = the way the prop's FRONT faces. Scale the asset so its bounds fit the listed size (see ASSETS.md 'Scaling rule').")
    a("")
    by_room = defaultdict(list)
    for p in h.props:
        by_room[p["room"]].append(p)
    for r in h.rooms.values():
        ps = by_room.get(r.id, [])
        if not ps:
            continue
        a(f"### {r.id} - {r.name}")
        a("| # | Asset key | Source | x | y | z | Yaw | Size (w x d x h) | Placement | Role / note |\n|---|---|---|---|---|---|---|---|---|---|")
        for p in ps:
            dims = ALL[p["key"]].get("dims") or ("", "", "")
            if p.get("scale"):
                dims = tuple(round(x * p["scale"], 2) for x in dims)
            how = "on " + p["on"] if p.get("on") else ("wall-hung" if p.get("hung") else ("raised" if p["y"] > 0.3 else "floor"))
            a(f"| {p['n']} | `{p['key']}` | {src_cell(p['key'])} | {f2(p['x'])} | {f2(p['y'])} | {f2(p['z'])} | {p['yaw']} | "
              f"{' x '.join(str(x) for x in dims)} | {how} | {p.get('role') or ''} {p.get('note') or ''} |")
        a("")
    a("## 4. Gameplay anchors (empty GameObjects the builder must create)")
    a("| Kind | Room | x | y | z | What |\n|---|---|---|---|---|---|")
    for an in h.anchors:
        a(f"| {an['kind']} | {an['room']} | {f2(an['x'])} | {f2(an['y'])} | {f2(an['z'])} | {an['note']} |")
    a("")
    a("Mapping to existing code: `item-spot` -> `House.ItemSpots` (order as listed), `note-spot` -> the 3 `Readable` notes, `puzzle` -> "
      "`PuzzleRig` anchor, `container` -> `PartContainer`, `shrine` -> `House.ShrineMarker` (+ shrine niche inside), `hiding` -> `HidingSpot`.")
    a("")
    a("## 5. Puzzle - " + h.part)
    for i, step in enumerate(T["puzzle"], 1):
        a(f"{i}. {step}")
    a("")
    a("## 6. Night changes (DESIGN 'Night changes' - pick 2-3 per match)")
    for n in T["night"]:
        a(f"- {n}")
    a("- plus the shared pool: extra person in the family photo, a door opening onto a brick wall, moved furniture, an extra hallway door, "
      "all clocks stopped at the same time, writing on a wall in a player's name")
    a("")
    a("## 7. Sound")
    for s in T["audio"]:
        a(f"- {s}")
    a(f"- Room tone everywhere: existing `Audio/Ambience/RoomTone.wav`; hearth: `HearthFire.wav`; Sonniss sources in `{SONNISS}` (see ASSETS.md).")
    a("")
    a("## 8. Build checklist for this house")
    for c in (f"Shell from wings + rooms (walls with door/window openings at the exact positions above), floors per level, roofs + chimneys.",
              "Every door: `Door` component on exterior doors (front/back/side); interior doors are hinged leaves (NavMesh: ignore leaf, carving obstacle).",
              "Weak windows: the two marked **WEAK** get the weak-window component (DESIGN break-in points).",
              "All props from the tables: correct asset key, position, yaw, scale; hiding/puzzle/container/shrine anchors wired to components.",
              "Hearth = the prop with role `hearth` (exactly one); `hearth-decor` props are cold fireplaces with no Hearth component.",
              "Exactly one fuse box with role `fuse-box` wired to `FuseBox` (extra boxes are dressing).",
              "Phone, mirror, family photo(s), TV/radio present (DESIGN shared template).",
              "Run `LayoutFingerprint.Record` only after the user approves the new layout (CLAUDE.md).",
              "Take `shot` images: front, back, every room from its main door; compare with the plan + massing images here."):
        a(f"- [ ] {c}")
    a("")
    return "\n".join(L)


# ===================================================================================================== HOUSES.md (index)
def houses_index():
    L = []; a = L.append
    a("# Hollowmere - the 15 houses (detailed build spec)")
    a("")
    a("This is the index. Each house has its own page and a JSON twin in `docs/design/houses/`. Everything was generated from "
      "`tools/housegen/` (Python) and validated: rooms don't overlap, every door has 0.85 m clear in front of it on both sides, "
      "no prop blocks a door or stair, every prop sits inside its room, and each house has the DESIGN shared-template items "
      "(hearth, fuse box, phone, mirror, family photo, radio/TV, wardrobes/beds, 4-6 hiding spots, front + back door, exactly 2 weak "
      "windows, kitchen, living room, 2+ bedrooms, bathroom, cellar or attic, 3 notes, item spots, puzzle, container, shrine niche).")
    a("")
    a("**Source of truth:** `docs/DESIGN.md` for rules; these files for geometry and dressing. HouseCatalog.cs keeps names, families, "
      "parts, door colours, chimney counts and clock hours - unchanged, so letters and MatchPlan still work.")
    a("")
    a("## Coordinates (read once)")
    a("- Metres. Each house has its own local space: origin (0,0,0) = ground-floor level at the centre of its main wing.")
    a("- **+z = the FRONT** of the house (the street side), -z = back, +x / -x = the two sides (+x is on your LEFT when you stand in the "
      "street facing the front door).")
    a("- Room `rect` = (x0, z0, x1, z1) measured to the OUTER face of exterior walls and to the CENTRE line of interior walls. Exterior wall "
      "thickness is taken inside the rect (wing `t`); interior partitions are 0.12 m and straddle the shared edge. `inner` in the JSON is the "
      "clear floor.")
    a("- Doors/windows: `side` = which wall of `room_a` (N=+z, S=-z, E=+x, W=-x), `at` = centre along that wall (x for N/S, z for E/W).")
    a("- Prop `yaw` = Unity Y rotation: 0 -> the prop's front faces +z, 90 -> +x, 180 -> -z, 270 -> -x.")
    a("- World placement: `world.position` (x, y = floor level, z) and `world.yaw` (the way the FRONT faces) - see VILLAGE_LAYOUT.md.")
    a("")
    a("## Summary")
    a("| # | House | Archetype | Footprint | Storeys | Roof | Puzzle (M1 rig) | Part | Street | To altar |\n|---|---|---|---|---|---|---|---|---|---|")
    for h in houses:
        x0, z0, x1, z1 = h.footprint(); v = VH[h.slug]
        lv = "+".join(k for k in h.levels)
        a(f"| {h.slug} | [{h.name}](houses/{slugname(h)}.md) | {h.traits['archetype']} | {x1-x0:.1f} x {z1-z0:.1f} m | {lv} | "
          f"{next((w['roof'] for w in h.wings if w['roof'] not in ('-','')), '-').split(',')[0].split(';')[0]} | {h.puzzle_kind} | {h.part} | {v['street']} | {v['to_altar']} m |")
    a("")
    a("![village](ref/village_map_v2.png)")
    a("")
    a("## How the houses differ (variety rules met)")
    a("- No two share footprint size, wall material + roof combination, or storey count + chimney layout.")
    a("- Plans: double-pile town house (H00), 1930s hall house (H01), single-pile thatched cottage (H02), front-gabled Gothic + rear wing (H03), "
      "cross-passage longhouse with loom floor (H04), double-pile villa + surgery wing (H05), mirrored double house (H06), tall Gothic + bay + rear "
      "wing (H07), shop with flat over + cold room (H08), house + 3-storey mill (H09), shop-house + glasshouse studio (H10), L-plan presbytery + oratory "
      "(H11), twin-gabled Arts & Crafts + sorting wing (H12), longhouse + barn + dairy (H13), lobby-entry jettied timber frame (H14).")
    a("- Stairs: straight, boxed, winder, stone, mirrored pair, ladders, mill ladders, cellar trap stairs - each house has its own.")
    a("")
    a("## Build order (see ROADMAP M1.6)")
    a("1. Data loader + validation (C#) for `houses/*.json` and `village.json`.")
    a("2. HouseBuilder: shells, openings, floors, stairs, roofs, chimneys (greybox primitives, real materials).")
    a("3. Props: asset-key -> prefab registry (PropKit), placement, hiding/puzzle/container/shrine wiring.")
    a("4. Puzzles: reskin the 3 M1 rigs per house, then bespoke rigs (GearPuzzle, ScalePuzzle, DarkroomPuzzle, DollhousePuzzle...).")
    a("")
    return "\n".join(L)


# ===================================================================================================== village
def village_md_json():
    vj = dict(schema="hollowmere.village/1", conventions="Unity world metres: +x east, +z north, y up; yaw = Unity Y rotation (front faces)",
              bounds=dict(inside_wall=[-150, -150, 150, 150], forest_belt_inner=[-128, -140, 128, 128], wall_height=5.0),
              terrain=dict(note="keep the playable ground FLAT at y=0 under streets and house pads (NavMesh + LayoutFingerprint); "
                                "undulate +-1.5 m only in gardens/fields/forest with a Unity Terrain; square stays at 0; pond surface -0.8; "
                                "stream bed -1.2 (banks 1:2)",
                           height_points=[dict(name="square", x=0, z=117, y=0.0), dict(name="green", x=0, z=4, y=0.0),
                                          dict(name="chapel hill", x=-70, z=104, y=0.6), dict(name="mill pond", x=-112, z=-76, y=-0.8),
                                          dict(name="fields", x=95, z=-105, y=0.4), dict(name="stream", x=0, z=-115, y=-1.2)]),
              houses=[dict(slug=v["slug"], name=v["name"], position=[v["pos"][0], v["ground"], v["pos"][1]], yaw=v["yaw"], street=v["street"],
                           front_door=v["front_door"], plot_poly=v["plot"], to_altar=v["to_altar"]) for v in vill["houses"]],
              landmarks=LANDMARKS, filler=[dict(id=f["id"], name=f["name"], pos=f["pos"], size=f["size"], yaw=f["yaw"]) for f in vill["filler"]],
              roads=vill["roads"], stream=dict(points=STREAM, width=3.0, bed_y=-1.2), bridges=[dict(id=b[0], name=b[1], pos=b[2], width=b[3], length=b[4]) for b in BRIDGES],
              night_blockers=[dict(id=b[0], prop=b[1], pos=b[2]) for b in BLOCKERS], scarecrows=SCARECROWS, street_lamps=STREET_LAMPS)
    json.dump(vj, open(os.path.join(D, "village.json"), "w"), indent=1)
    L = []; a = L.append
    a("# Hollowmere - village layout v2 (streets, plots, landmarks)")
    a("")
    a("> Machine-readable twin: `docs/design/village.json`. Matches DESIGN.md 'The village map': 300 x 300 m inside the wall, square and gate "
      "on the north edge. The square, gate, altar, stone circle, bell tower, notice board, chapel and fog road stay EXACTLY where "
      "`GreyboxVillageBuilder` puts them today; only the houses (were a 3 x 5 grid), streets and dressing change.")
    a("")
    a("![village map](ref/village_map_v2.png)")
    a("")
    a("## Why the old layout read wrong, and the rules now")
    for t in ("Houses sat on a grid, all facing north. Now every house faces the street its front door opens onto (yaw per house).",
              "No street hierarchy. Now: Main Street (6 m, setts, pavements) -> lanes (4-4.5 m, mud + gravel) -> tracks (3 m) -> paths (2-2.5 m).",
              "Empty gaps. Now 16 boarded/ruined filler buildings, plots with walls/fences/gardens, lamps, a green with a well and memorial.",
              "Main Street is a continuous built frontage from the square to the green (H12, F01, H08, H10, F04 / F02, H00, F03) - the "
              "'tight' part of the village; the green is the open heart; lanes radiate out to looser, more isolated houses (Mill, Widow, Farm).",
              "Routes: every house is 35-230 m from the altar (front door straight line). The far three (Miller 229 m, Widow 203 m, Farmer 164 m) "
              "are the high-risk runs; Main Street houses are the short, safe-feeling ones.",
              "Landmarks for orientation: bell tower (seen from everywhere), chapel, the green with its oak and well, the mill wheel, the farm barn."):
        a(f"- {t}")
    a("")
    a("## Houses")
    a("| House | World x | Floor y | World z | Yaw (front faces) | Street | Front door | To altar |\n|---|---|---|---|---|---|---|---|")
    for v in vill["houses"]:
        a(f"| {v['slug']} {v['name']} | {v['pos'][0]} | {v['ground']:+.2f} | {v['pos'][1]} | {v['yaw']} | {v['street']} | "
          f"({v['front_door'][0]:.1f}, {v['front_door'][1]:.1f}) | {v['to_altar']} m |")
    a("")
    a("Placement formula: world = position + R(yaw) * local, with R(yaw): x' = x cos(yaw) + z sin(yaw), z' = -x sin(yaw) + z cos(yaw) "
      "(Unity `Quaternion.Euler(0, yaw, 0) * local`).")
    a("")
    a("## Streets")
    a("| Id | Name | Kind | Width | Surface | Points (x, z) | Length |\n|---|---|---|---|---|---|---|")
    surf = {"street": "Cobbles2 setts + 1.5 m Paving pavements with a 0.12 m kerb", "lane": "Road (mud+leaves) centre, GroundGravel edges",
            "track": "GroundMud with a grass centre strip (GroundGrassPath)", "path": "GroundGrassPath / stepping stones"}
    for r in vill["roads"]:
        a(f"| {r['id']} | {r['name']} | {r['kind']} | {r['width']} m | {surf[r['kind']]} | {', '.join(f'({x},{z})' for x, z in r['points'])} | {r['length']} m |")
    a("")
    a("Build streets as meshes from these polylines (Catmull-Rom smoothed, 0.5 m segments), 0.02 m above ground, with a 0.3 m blended "
      "verge decal. The existing fog road (0,150)->(0,352) is unchanged.")
    a("")
    a("## Landmarks and village buildings")
    a("| Id | Name | Centre (x, z) | Size | Yaw | Notes |\n|---|---|---|---|---|---|")
    for lm in LANDMARKS:
        a(f"| {lm['id']} | {lm['name']}{' (existing)' if lm.get('keep') else ''} | {lm['pos']} | {lm['size'][0]} x {lm['size'][1]} | {lm['yaw']} | {lm['note']} |")
    a("")
    a("## Filler buildings (non-allotted)")
    a("All locked; boarded windows (Planks over WeakWindowBoards) you can peek through; F03 is a walk-through ruin (roof gone, 1 hiding spot), "
      "F06 an open forge (hiding behind the bellows). Use 3 shell variants: cottage (gable, Roof), terrace (2 doors), outbuilding (CorrugatedRust).")
    a("")
    a("| Id | What | Centre (x, z) | Size | Yaw |\n|---|---|---|---|---|")
    for f in vill["filler"]:
        a(f"| {f['id']} | {f['name']} | {f['pos']} | {f['size'][0]} x {f['size'][1]} | {f['yaw']} |")
    a("")
    a("## Water, bridges, night blockers, scarecrows, lamps")
    a(f"- Stream (3 m wide, bed y -1.2): {', '.join(f'({x},{z})' for x, z in STREAM)}. Mill race = the segment along x = -91.5 past the mill wheel.")
    for b in BRIDGES:
        a(f"- Bridge {b[0]}: {b[1]} at {b[2]}, {b[3]} m wide, {b[4]} m long.")
    a(f"- Night blockers (MatchPlan picks one per match - DESIGN 'A fallen tree or cart blocks one random path'): "
      + ", ".join(f"{b[0]} {b[1]} at {b[2]}" for b in BLOCKERS) + ". Each sits across a lane and carves the NavMesh.")
    a(f"- Scarecrows (H13 puzzle; DESIGN: turn to face the nearest player between acts): {SCARECROWS}.")
    a(f"- Street lamps (StreetLamp, 1 in 3 working by day, all dead at night except during the chant): {STREET_LAMPS}.")
    a("- Forest belt: 20-25 m deep inside the ring wall (FirTree/PineTree impostors + existing DeadLimbs prefabs + Shrub/Fern/Nettles), "
      "dense enough to block sight; the Hollow's ForestSpawns move to its inner edge.")
    a("")
    a("## Gameplay hooks to update (VillageLayout.cs)")
    a("- `RoamPoints`: replace the old 6 x 5 grid with the road polyline vertices + the green + the square (the builder can derive them from village.json).")
    a("- `ForestSpawns`: 8 points on the forest belt's inner edge (one per compass direction).")
    a("- `PlayerSpawn`, `HollowNightSpawn` (chapel), `SquareCenter`, `GateLineZ`, `BridgeZ`: unchanged.")
    a("- Re-record `GreyboxLayout.txt` (LayoutFingerprint) ONCE the user has approved the new layout.")
    a("")
    open(os.path.join(D, "VILLAGE_LAYOUT.md"), "w").write("\n".join(L))


# ===================================================================================================== assets
def assets_md():
    use = defaultdict(list)
    for h in houses:
        for p in h.props:
            use[p["key"]].append(h.slug)
        for r in h.rooms.values():
            for k in ALL:
                if ALL[k]["kind"] == "texture" and (k in r.floor.split() or k in r.walls.split() or k == r.ceiling):
                    use[k].append(h.slug)
        for w in h.wings:
            for k in ALL:
                if ALL[k]["kind"] == "texture" and (k in w["walls"] or k in w["roof"]):
                    use[k].append(h.slug)
    cat = dict(materials=MATERIALS, models=MODELS, audio=[dict(key=a, folder=b, use=c) for a, b, c in AUDIO], sonniss_root=SONNISS)
    json.dump(cat, open(os.path.join(D, "asset_catalog.json"), "w"), indent=1)
    L = []; a = L.append
    a("# Hollowmere - assets: every model, texture and sound, where it comes from, who downloads it")
    a("")
    a("> Machine-readable: `docs/design/asset_catalog.json` (every key), `tools/third-party.json` (Poly Haven + ambientCG downloads, "
      "merged with what you already have), `tools/sketchfab.json` (CC-BY models). Asset KEYS are what the house/village JSON use.")
    a("")
    a("## Who does what")
    a("| Source | Licence | Who downloads | How |\n|---|---|---|---|")
    a("| Poly Haven (models, textures, HDRIs) | CC0 - no attribution needed, credit anyway in CREDITS.md | **Claude Code (PC)** | `tools/fetch-assets.ps1` (already exists) reads the merged `tools/third-party.json` |")
    a("| ambientCG (textures) | CC0 | **Claude Code (PC)** | same script |")
    a(f"| Sketchfab ({len([m for m in MODELS if m['source']=='sketchfab'])} CC-BY models: phone, bath, toilet, dolls, loom, enlarger...) | CC BY 4.0 - **must credit the author** (CREDITS.md is generated) | **Claude Code (PC)** after **you** give it a Sketchfab API token once | new `tools/fetch-sketchfab.ps1` + `tools/sketchfab.json`; fallback: you click 'Download 3D model' on each page (list below) |")
    a("| Fab - Quixel 'Saloon Interior' (FREE, already in `AssetDownloads/salooninterior.unitypackage`) | Fab Standard License (any engine) | done | `tools/import-assetstore.ps1` (already set up) |")
    a("| Sonniss GDC 2026 bundle (already in `AssetDownloads/`) | Sonniss GDC licence (royalty-free, commercial OK) | done | `tools/prepare-audio.ps1` + `tools/audio-sources.json` |")
    a("| Procedural (`BUILD`) | own work | **Claude Code** | PropKit/BuilderKit primitives + catalog materials - shapes in the table below |")
    a("| Paid upgrades (later) | store EULA | **you** buy, Claude Code imports | list at the bottom; buy only after the greybox+free pass plays well |")
    a("")
    a("### You do this once (10 minutes)")
    a("1. Make a free Sketchfab account at https://sketchfab.com/signup.")
    a("2. Get your API token: Sketchfab -> Settings -> Password & API -> 'API Token' -> copy.")
    a("3. Save the token on your PC only (it must never be pasted into a chat or committed): open Notepad, paste the token, "
      "and save it as `%USERPROFILE%\\.hollowmere\\sketchfab_token.txt` (make the `.hollowmere` folder in your user folder; "
      "in Notepad's Save dialog choose 'All files' so it doesn't add .txt twice). Then tell Claude Code: `Run tools/fetch-sketchfab.ps1.`")
    a("4. If the script reports a 401/403 for any model (Sketchfab sometimes requires the OAuth flow), open the model's page from the "
      "table below, click **Download 3D model -> glTF (.glb)** and drop the file in `C:\\dev\\Hollowmere\\AssetDownloads\\Sketchfab\\<key>.glb` (the key is the first column below; `<uid>.glb` also works). "
      "The script picks local files up automatically.")
    a("5. Unity needs glTF import for .glb: Claude Code adds `com.unity.cloud.gltfast` to `BatchTools.RequiredPackages` and runs `setup`.")
    a("")
    a("## Scaling rule (all models)")
    a("Every model key has a target size (w x d x h, metres; w = along the wall it backs onto, d = depth from the wall, h = height). "
      "On import, Claude Code scales the model UNIFORMLY so it fits inside that box and is as large as possible, pivots it at the bottom centre "
      "of its bounds, rotates it so its front faces +z, and logs any axis that ends up >10% off the target. A per-house `scale` multiplies "
      "the box (e.g. 0.85 for a smaller bath).")
    a("")
    a("## Models")
    a("| Key | Source | Asset id | Size w x d x h (m) | Download | Used in | Notes / paid upgrade |\n|---|---|---|---|---|---|---|")
    for m in MODELS:
        if m["source"] == "sketchfab":
            sid = f"[{m['id']}](https://sketchfab.com/3d-models/{m['id']})"
        elif m["source"] == "polyhaven":
            sid = f"[{m['id']}](https://polyhaven.com/a/{m['id']})"
        else:
            sid = m["id"]
        dims = " x ".join(str(x) for x in m["dims"]) if m.get("dims") else ""
        u = sorted(set(use.get(m["key"], []))) + (["village"] if m["key"] in VILLAGE_KEYS else [])
        a(f"| `{m['key']}` | {m['source']} | {sid} | {dims} | {m['download']} | {', '.join(u) if u else 'spare (not placed yet)'} | "
          f"{(m.get('author') + '. ') if m.get('author') else ''}{m.get('note', '')} {('Upgrade: ' + m['upgrade']) if m.get('upgrade') else ''} |")
    a("")
    a("## Textures / materials")
    a("All PBR sets: colour, OpenGL normal, packed ARM (or AO/roughness/metal). 2K unless noted. URP Lit, tiling in metres "
      "(BuilderKit UVs are metric): walls 2.0 m, floors 1.5 m, roofs 2.0 m, fabrics 0.5 m.")
    a("")
    a("| Key | Source | Id | Used for | Download | Used in |\n|---|---|---|---|---|---|")
    for t in MATERIALS:
        link = f"https://polyhaven.com/a/{t['id']}" if t["source"] == "polyhaven" else f"https://ambientcg.com/view?id={t['id']}"
        u = sorted(set(use.get(t["key"], []))) + (["village"] if t["key"] in VILLAGE_KEYS else [])
        a(f"| `{t['key']}` | {t['source']} | [{t['id']}]({link}) | {t['use']} | {t['download']} | {', '.join(u) if u else 'spare'} |")
    a("")
    a("## Audio (already on your PC - Sonniss GDC 2026)")
    a(f"Root: `{SONNISS}`. Claude Code picks 2-6 files per need, trims/normalises with `tools/prepare-audio.ps1`, and lists them in "
      "`tools/audio-sources.json` (existing flow).")
    a("")
    a("| Need key | Sonniss folder | Used for |\n|---|---|---|")
    for k, folder, u in AUDIO:
        a(f"| {k} | {folder} | {u} |")
    a("")
    a("## Procedural props (Claude Code builds these)")
    a("| Key | Shape spec | Size |\n|---|---|---|")
    for m in MODELS:
        if m["source"] == "procedural":
            a(f"| `{m['key']}` | {m['id']} | {' x '.join(str(x) for x in m['dims'])} |")
    a("")
    a("## Paid upgrades for later (researched, prices from the store pages; check URP support + screenshots before buying)")
    for t in ("**Rewind Memory PRO: 80s/90s Retro Rooms** (EmacEArt, $54, URP+HDRP, Jan 2026) - 1970s-80s interior dressing for every house. https://assetstore.unity.com/packages/3d/props/interior/1980-midpoly-retro-rooms-childroomupdate2021-part-iii-177436",
              "**Complete Home Interior Pack** (Nekobolt, $20, Built-in + URP) - kitchens, bathrooms, furniture. https://marketplace.unity.com/packages/3d/props/interior/31049",
              "**Modular Dark Fantasy Graveyard / Cemetery** (Hivemind, $49.99, URP+HDRP) - graveyard, chapel dressing. https://marketplace.unity.com/packages/3d/environments/fantasy/modular-dark-fantasy-graveyard-cemetery-horror-cemetery-horror-g-300192",
              "**Detailed Old Wooden Houses and Shacks Pack** (ANRUVAL, $24.99) - sheds, barns, boat-houses, filler. https://marketplace.unity.com/packages/3d/environments/historic/detailed-old-wooden-houses-and-shacks-pack-175718",
              "**Medieval Nordic Village Environment** (BEFOUR, $64.99, URP+HDRP) - only for the oldest buildings (H14, H02) and forest. https://marketplace.unity.com/packages/3d/environments/historic/medieval-nordic-village-environment-279881",
              "**Abandoned Horror Mansion Interior (Unity)** (Leartes, $89.99, 160 assets; pipeline not stated - ask the seller) - decayed interiors. https://leartesstudios.gumroad.com/l/horrormansion_unity",
              "**NatureManufacture forest / R.A.M. river packs** (Built-in/URP/HDRP) - the forest belt and the stream. https://blog.unity.com/en/games/create-high-fidelity-worlds-with-naturemanufacture",
              "**Fab premium Megascans collections** (e.g. 'Medieval Farm', 'European Broadleaf Forest') - photoscanned hero props; Fab Standard License allows any engine. https://quixel.com/news/quixel-on-fab-new-megascans-and-megaplants",
              "**A pump organ / harmonium hero model** (e.g. 'Vintage Old Pump Pipe Organ', Cubebrush) to replace the procedural PumpOrgan. https://cubebrush.co/glenfx/products/0ydxa/vintage-old-pump-pipe-organ"):
        a(f"- {t}")
    a("")
    return "\n".join(L)


def used_keys():
    u = set(VILLAGE_KEYS)
    for h in houses:
        u |= {p["key"] for p in h.props}
        texts = [r.floor + " " + r.walls + " " + r.ceiling for r in h.rooms.values()] + [w["walls"] + " " + w["roof"] for w in h.wings]
        for k, e in ALL.items():
            if e["kind"] == "texture" and any(re.search(r"\b" + k + r"\b", t) for t in texts):
                u.add(k)
    return u


def tool_manifests(existing_tp_path):
    U = used_keys()
    tp = json.load(open(existing_tp_path)) if existing_tp_path and os.path.exists(existing_tp_path) else dict(textures=[], hdris=[], models=[])
    have_t = {t["id"] for t in tp["textures"]}; have_m = {m["id"] for m in tp["models"]}
    for t in MATERIALS:
        if t["download"] == "auto" and t["id"] not in have_t and t["key"] in U:
            tp["textures"].append(dict(source=t["source"], id=t["id"], res=t["res"], key=t["key"], use=t["use"]))
    for m in MODELS:
        if m["source"] == "polyhaven" and m["download"] == "auto" and m["id"] not in have_m and "heavy" not in m.get("note", "") and m["key"] in U:
            have_m.add(m["id"])
            tp["models"].append(dict(source="polyhaven", id=m["id"], res=m["res"], key=m["key"], use=f"house/village prop ({m['key']})"))
    if "v2 (village & houses rebuild)" not in tp.get("comment", ""):
        tp["comment"] = (tp.get("comment", "") + " | v2 (village & houses rebuild): entries appended from docs/design/asset_catalog.json; "
                         "keys match the house JSON asset keys.").strip(" |")
    json.dump(tp, open(os.path.join(ROOT, "tools", "third-party.json"), "w"), indent=2)
    sk = dict(comment="CC BY 4.0 models from Sketchfab. tools/fetch-sketchfab.ps1 downloads the GLB with the user's Sketchfab API token (%USERPROFILE%/.hollowmere/sketchfab_token.txt or SKETCHFAB_TOKEN, never committed) into "
                      "Assets/ThirdParty/Sketchfab/<key>/ and writes source.json for CREDITS.md. If a download is refused, the user downloads "
                      "the .glb by hand into AssetDownloads/Sketchfab/<key>.glb (or <uid>.glb) and the script copies it.",
              models=[dict(key=m["key"], uid=m["id"], author=m.get("author"), licence="CC BY 4.0",
                           url=f"https://sketchfab.com/3d-models/{m['id']}", target_size=m["dims"], note=m.get("note", ""))
                      for m in MODELS if m["source"] == "sketchfab" and m["key"] in U])
    json.dump(sk, open(os.path.join(ROOT, "tools", "sketchfab.json"), "w"), indent=2)
    return len(tp["textures"]), len(tp["models"]), len(sk["models"])


def reference_md():
    L = []; a = L.append
    a("# Reference images")
    a("")
    a("Two kinds of reference, both in `docs/design/ref/`:")
    a("")
    a("1. **Technical (exact) - generated from the same data the builder uses.** Use these to check your build: floor plans with every "
      "prop numbered (`Hxx_plan.png`), front-left and back-right massing views (`Hxx_massing_front.png`, `Hxx_massing_back.png`) and the village "
      "map (`village_map_v2.png`). After building, render the same views with `tools/unity.ps1 shot` and compare side by side.")
    a("2. **Mood (look & feel) - AI-generated or photos.** For each house, `Hxx_mood.png` (optional). Prompts are on each house page and in "
      "`docs/design/mood_prompts.txt`. Generate them in Higgsfield, Gemini or ChatGPT (see `docs/AI_HELPERS.md`) and save them with that name; "
      "Claude Code reads PNGs with its Read tool.")
    a("")
    a("Mood images are for colour, material, wear and atmosphere ONLY - when they disagree with the plan/massing, the plan wins.")
    a("")
    for h in houses:
        a(f"## {h.slug} {h.name}")
        a(f"![plan](ref/{h.slug}_plan.png)")
        a(f"![front](ref/{h.slug}_massing_front.png) ![back](ref/{h.slug}_massing_back.png)")
        a("")
    open(os.path.join(D, "REFERENCE_IMAGES.md"), "w").write("\n".join(L))
    with open(os.path.join(D, "mood_prompts.txt"), "w") as f:
        for h in houses:
            f.write(f"{h.slug}_mood.png\n{TEXT[h.slug]['prompt']}, {STYLE}\n\n")
        f.write("village_square_mood.png\ncobbled village square at dusk with a stone circle around an altar, a bell tower, a notice board, "
                f"a closed wooden village gate in a stone wall, {STYLE}\n\n")
        f.write("main_street_mood.png\nnarrow English village main street lined with brick and rendered shop-houses, a butcher's with a torn "
                f"awning, a photographer's shop window, a post office with a red pillar box, wet setts, {STYLE}\n\n")


if __name__ == "__main__":
    existing_tp = sys.argv[2] if len(sys.argv) > 2 else None
    errs = 0
    for h in houses:
        e, w = validate(h)
        errs += len(e)
        json.dump(house_json(h), open(os.path.join(D, "houses", f"{h.slug}.json"), "w"), indent=1)
        open(os.path.join(D, "houses", f"{slugname(h)}.md"), "w").write(house_md(h))
        render_house.plan(h, os.path.join(D, "ref", f"{h.slug}_plan.png"))
        render_house.massing(h, os.path.join(D, "ref", f"{h.slug}_massing_front.png"), elev=22, azim=120)
        render_house.massing(h, os.path.join(D, "ref", f"{h.slug}_massing_back.png"), elev=26, azim=-60)
    render_village.render(os.path.join(D, "ref", "village_map_v2.png"))
    open(os.path.join(D, "HOUSES.md"), "w").write(houses_index())
    village_md_json()
    open(os.path.join(D, "ASSETS.md"), "w").write(assets_md())
    print("manifest counts (textures, PH models, sketchfab):", tool_manifests(existing_tp))
    reference_md()
    print("validation errors:", errs, "| village problems:", len(vill["problems"]))
