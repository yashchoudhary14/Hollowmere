import math, re, os
import matplotlib
matplotlib.use("Agg")
import matplotlib.pyplot as plt
from matplotlib.patches import Rectangle, Polygon, Arc, Circle, FancyArrow
from mpl_toolkits.mplot3d.art3d import Poly3DCollection
from catalog import ALL

KIND_COL = {"kitchen": "#7a5b3a", "living": "#5b4a6e", "living-bay": "#5b4a6e", "bedroom": "#3f5a6a", "bedroom-bay": "#3f5a6a",
            "bathroom": "#3c6a66", "hall": "#5a5a52", "landing": "#5a5a52", "lobby": "#5a5a52", "passage": "#5a5a52", "porch": "#4a4a44",
            "cellar": "#3a3a3a", "attic": "#4a4436", "utility": "#5a503e", "shop": "#6a4e3a", "study": "#4e5a3a", "dining": "#5e4a3a",
            "workshop": "#6a5a3a", "music": "#5b3a4e", "music-bay": "#5b3a4e", "chapel": "#4a4a6a", "dispensary": "#3a5a5a",
            "surgery": "#3a5a5a", "waiting": "#4a5a5a", "studio": "#5a6a6a", "greenhouse": "#4a6a4a", "coldroom": "#4a5a6a",
            "mill": "#5a4a3a", "barn": "#5a4a2a", "storage": "#4a4436", "closet": "#4a4436", "sorting": "#5a4a3a"}
ANCH = {"hiding": ("H", "#ff6a5a"), "item-spot": ("I", "#ffd24a"), "note-spot": ("N", "#9ae06a"), "puzzle": ("P", "#ff4af0"),
        "container": ("C", "#4af0ff"), "shrine": ("S", "#ffffff")}


def plan(h, out_path):
    lvls = list(h.levels.keys())
    n = len(lvls)
    x0, z0, x1, z1 = h.footprint()
    W, D = x1 - x0, z1 - z0
    fig, axs = plt.subplots(1, n, figsize=(max(6, W * 0.62) * n, max(6, D * 0.62) + 1.2), dpi=110)
    if n == 1:
        axs = [axs]
    fig.patch.set_facecolor("#141512")
    for ax, lv in zip(axs, lvls):
        ax.set_facecolor("#1d1f1a")
        for w in h.wings:
            if lv in w["levels"]:
                a = w["rect"]
                ax.add_patch(Rectangle((a[0], a[1]), a[2] - a[0], a[3] - a[1], fc="#2a2c26", ec="#d8d0b8", lw=2.2, zorder=1))
        for r in h.level_rooms(lv):
            ix0, iz0, ix1, iz1 = r.inner()
            ax.add_patch(Rectangle((ix0, iz0), ix1 - ix0, iz1 - iz0, fc=KIND_COL.get(r.kind, "#555"), ec="#cfc6ad", lw=0.8, alpha=0.9, zorder=2))
            ax.text(r.cx, r.rect[3] - 0.45, f"{r.name}\n{r.w:.1f} x {r.d:.1f} m", ha="center", va="top", fontsize=6.3, color="#f4ecd4", zorder=9,
                    bbox=dict(fc="#00000066", ec="none", pad=0.6))
        # doors
        for d in h.doors:
            if d["level"] != lv:
                continue
            r = h.rooms[d["room_a"]]
            x0r, z0r, x1r, z1r = r.rect
            w = d["width"]
            if d["side"] in "NS":
                z = z1r if d["side"] == "N" else z0r
                ax.plot([d["at"] - w / 2, d["at"] + w / 2], [z, z], color="#1d1f1a", lw=5, zorder=4)
                col = "#ffb347" if d["room_b"] == "ext" else "#e8e0c8"
                ax.plot([d["at"] - w / 2, d["at"] + w / 2], [z, z], color=col, lw=1.2, ls=":", zorder=5)
                ax.add_patch(Arc((d["at"] - w / 2, z), 2 * w, 2 * w, theta1=0 if d["side"] == "S" else 270, theta2=90 if d["side"] == "S" else 360,
                                 color=col, lw=0.6, zorder=5))
            else:
                x = x1r if d["side"] == "E" else x0r
                ax.plot([x, x], [d["at"] - w / 2, d["at"] + w / 2], color="#1d1f1a", lw=5, zorder=4)
                col = "#ffb347" if d["room_b"] == "ext" else "#e8e0c8"
                ax.plot([x, x], [d["at"] - w / 2, d["at"] + w / 2], color=col, lw=1.2, ls=":", zorder=5)
            if d["room_b"] == "ext":
                lx = d["at"] if d["side"] in "NS" else (x1r + 0.5 if d["side"] == "E" else x0r - 0.5)
                lz = (z1r + 0.5 if d["side"] == "N" else z0r - 0.5) if d["side"] in "NS" else d["at"]
                ax.text(lx, lz, d["role"].upper(), fontsize=6, color="#ffb347", ha="center", va="center", zorder=9)
        # windows
        for wn in h.windows:
            r = h.rooms[wn["room"]]
            if r.level != lv:
                continue
            x0r, z0r, x1r, z1r = r.rect
            c = "#ff5050" if wn["weak"] else "#7fd0ff"
            if wn["side"] in "NS":
                z = z1r if wn["side"] == "N" else z0r
                ax.plot([wn["at"] - wn["width"] / 2, wn["at"] + wn["width"] / 2], [z, z], color=c, lw=3.2, zorder=6)
            else:
                x = x1r if wn["side"] == "E" else x0r
                ax.plot([x, x], [wn["at"] - wn["width"] / 2, wn["at"] + wn["width"] / 2], color=c, lw=3.2, zorder=6)
        # stairs
        for st in h.stairs:
            if h.rooms[st["room"]].level != lv:
                continue
            a = st["rect"]
            ax.add_patch(Rectangle((a[0], a[1]), a[2] - a[0], a[3] - a[1], fc="#00000055", ec="#e8e0c8", lw=0.8, hatch="---", zorder=3))
            cx, cz = (a[0] + a[2]) / 2, (a[1] + a[3]) / 2
            dx, dz = {"N": (0, 1), "S": (0, -1), "E": (1, 0), "W": (-1, 0)}[st["ascent"]]
            L = 0.35 * ((a[3] - a[1]) if st["ascent"] in "NS" else (a[2] - a[0]))
            ax.add_patch(FancyArrow(cx - dx * L, cz - dz * L, 2 * dx * L, 2 * dz * L, width=0.04, head_width=0.25, color="#e8e0c8", zorder=7))
            ax.text(cx, cz, f"{st['frm']}->{st['to']}", fontsize=5.5, color="#ffffff", ha="center", va="center", zorder=8, rotation=90 if st["ascent"] in "NS" else 0)
        # props
        for p in h.props:
            if h.rooms[p["room"]].level != lv:
                continue
            f = h.fp(p)
            elev = p.get("on") or p.get("hung") or p["y"] > 0.3
            ax.add_patch(Rectangle((f[0], f[1]), f[2] - f[0], f[3] - f[1], fc="#d9c7a0" if not elev else "none",
                                   ec="#3a2a10" if not elev else "#d9c7a0", lw=0.5, alpha=0.85 if not elev else 0.9, zorder=6 if not elev else 7,
                                   ls="-" if not elev else "--"))
            q = int(round(p["yaw"] / 90)) % 4
            fx, fz = [(0, 1), (1, 0), (0, -1), (-1, 0)][q]
            if not elev:
                ax.plot([p["x"], p["x"] + fx * 0.25], [p["z"], p["z"] + fz * 0.25], color="#7a1010", lw=0.8, zorder=7)
            ax.text(p["x"], p["z"], str(p["n"]), fontsize=4.8, color="#101010" if not elev else "#f0e0c0", ha="center", va="center", zorder=8)
        for a in h.anchors:
            if h.rooms[a["room"]].level != lv:
                continue
            ch, col = ANCH[a["kind"]]
            ax.add_patch(Circle((a["x"] + 0.18, a["z"] + 0.18), 0.16, fc=col, ec="black", lw=0.4, zorder=10))
            ax.text(a["x"] + 0.18, a["z"] + 0.18, ch, fontsize=4.8, ha="center", va="center", color="black", zorder=11, weight="bold")
        for c in h.chimney_stacks:
            if lv in ("G", "F", "C"):
                ax.add_patch(Rectangle((c["x"] - c["w"] / 2, c["z"] - c["d"] / 2), c["w"], c["d"], fc="#7a3a2a", ec="#e8c0a0", lw=0.6, hatch="xxx", zorder=3))
        ax.set_xlim(x0 - 1.2, x1 + 1.2); ax.set_ylim(z0 - 1.2, z1 + 1.4); ax.set_aspect("equal")
        ax.tick_params(colors="#8a8a7a", labelsize=6); ax.grid(color="#33352e", lw=0.3)
        ax.set_title(f"{h.slug} {h.name} - {h.levels[lv]['label']} (y={h.levels[lv]['y']:+.2f}, h={h.levels[lv]['h']:.2f})",
                     color="#efe4c8", fontsize=9)
        ax.annotate("FRONT (+z)", xy=((x0 + x1) / 2, z1 + 1.15), color="#ffb347", fontsize=7, ha="center")
    fig.text(0.01, 0.005, "numbers = prop ids (HOUSES.md table)  solid = floor prop (red tick = facing)  dashed = on/hung prop  "
             "blue = window  red = WEAK window  dotted gap = door (orange = exterior)  hatched = stair (arrow = up)  "
             "H hiding  I item spot  N note  P puzzle  C part container  S shrine", color="#c9c0a8", fontsize=7)
    plt.tight_layout(rect=(0, 0.02, 1, 1))
    plt.savefig(out_path, facecolor=fig.get_facecolor()); plt.close()


def _roof_info(w):
    s = w["roof"].lower()
    m = re.search(r"(\d+)\s*deg", s)
    pitch = float(m.group(1)) if m else 40.0
    if "flat" in s and "gable" not in s.split("flat")[0][-30:]:
        kind = "flat"
    elif "hipped" in s:
        kind = "hip"
    elif "lean" in s or "mono" in s or "catslide" in s:
        kind = "shed"
    else:
        kind = "gable"
    a = w["rect"]
    axis = "x" if (a[2] - a[0]) >= (a[3] - a[1]) else "z"
    if "ridge along z" in s: axis = "z"
    if "ridge along x" in s: axis = "x"
    if "m-roof" in s: kind, axis = "m", "z"
    return kind, pitch, axis


def massing(h, out_path, elev=24, azim=-60):
    fig = plt.figure(figsize=(9, 7), dpi=110)
    fig.patch.set_facecolor("#141512")
    ax = fig.add_subplot(111, projection="3d")
    ax.set_facecolor("#141512")
    top_by_level = {k: v["y"] + v["h"] for k, v in h.levels.items()}
    polys, cols = [], []
    for w in h.wings:
        x0, z0, x1, z1 = w["rect"]
        lv_above = [l for l in w["levels"] if h.levels[l]["y"] >= 0]
        if not lv_above:
            continue
        hmax = max(top_by_level[l] for l in lv_above if l != "A") if any(l != "A" for l in lv_above) else top_by_level[lv_above[0]] - 1.5
        y0 = 0.0
        col = "#b8865a" if "Brick" in w["walls"] else ("#8a8a84" if "Stone" in w["walls"] else "#c9bfa6")
        if "Weatherboard" in w["walls"]: col = "#2c2a28"
        if "glass" in w["walls"].lower(): col = "#9fb8c0"
        if "ochre" in w["walls"].lower() or "Ochre" in w["walls"]: col = "#b9934a"
        # walls (box)
        P = lambda x, z, y: (x, z, y)
        faces = [[P(x0, z0, y0), P(x1, z0, y0), P(x1, z0, hmax), P(x0, z0, hmax)], [P(x1, z0, y0), P(x1, z1, y0), P(x1, z1, hmax), P(x1, z0, hmax)],
                 [P(x1, z1, y0), P(x0, z1, y0), P(x0, z1, hmax), P(x1, z1, hmax)], [P(x0, z1, y0), P(x0, z0, y0), P(x0, z0, hmax), P(x0, z1, hmax)]]
        polys += faces; cols += [col] * 4
        kind, pitch, axis = _roof_info(w)
        t = math.tan(math.radians(pitch))
        rc = "#5a3a2e" if "Clay" in w["roof"] or "clay" in w["roof"] or "RedSlate" in w["roof"] else "#3a3c40"
        if "thatch" in w["roof"].lower(): rc = "#8a7448"
        if "Corrugated" in w["roof"]: rc = "#7a4a2a"
        if "Glass" in w["roof"] or "glass" in w["roof"]: rc = "#bcd6dc"
        if kind == "flat":
            polys.append([P(x0, z0, hmax + 0.3), P(x1, z0, hmax + 0.3), P(x1, z1, hmax + 0.3), P(x0, z1, hmax + 0.3)]); cols.append(rc)
        elif kind == "shed":
            rise = min(x1 - x0, z1 - z0) * t * 0.5
            polys.append([P(x0, z0, hmax), P(x1, z0, hmax), P(x1, z1, hmax + rise), P(x0, z1, hmax + rise)]); cols.append(rc)
        elif kind in ("gable", "hip", "m"):
            if kind == "m":
                xm = (x0 + x1) / 2
                for a0, a1 in ((x0, xm), (xm, x1)):
                    r = (a1 - a0) / 2 * t; xr = (a0 + a1) / 2
                    polys += [[P(a0, z0, hmax), P(xr, z0, hmax + r), P(xr, z1, hmax + r), P(a0, z1, hmax)],
                              [P(a1, z0, hmax), P(xr, z0, hmax + r), P(xr, z1, hmax + r), P(a1, z1, hmax)]]
                    cols += [rc, rc]
                    polys += [[P(a0, z1, hmax), P(a1, z1, hmax), P(xr, z1, hmax + r)], [P(a0, z0, hmax), P(a1, z0, hmax), P(xr, z0, hmax + r)]]
                    cols += [col, col]
            elif axis == "x":
                r = (z1 - z0) / 2 * t; zr = (z0 + z1) / 2
                inset = (z1 - z0) / 2 if kind == "hip" else 0
                polys += [[P(x0, z0, hmax), P(x1, z0, hmax), P(x1 - inset, zr, hmax + r), P(x0 + inset, zr, hmax + r)],
                          [P(x0, z1, hmax), P(x1, z1, hmax), P(x1 - inset, zr, hmax + r), P(x0 + inset, zr, hmax + r)]]
                cols += [rc, rc]
                polys += [[P(x0, z0, hmax), P(x0, z1, hmax), P(x0 + inset, zr, hmax + r)], [P(x1, z0, hmax), P(x1, z1, hmax), P(x1 - inset, zr, hmax + r)]]
                cols += [rc if kind == "hip" else col] * 2
            else:
                r = (x1 - x0) / 2 * t; xr = (x0 + x1) / 2
                inset = (x1 - x0) / 2 if kind == "hip" else 0
                polys += [[P(x0, z0, hmax), P(x0, z1, hmax), P(xr, z1 - inset, hmax + r), P(xr, z0 + inset, hmax + r)],
                          [P(x1, z0, hmax), P(x1, z1, hmax), P(xr, z1 - inset, hmax + r), P(xr, z0 + inset, hmax + r)]]
                cols += [rc, rc]
                polys += [[P(x0, z0, hmax), P(x1, z0, hmax), P(xr, z0 + inset, hmax + r)], [P(x0, z1, hmax), P(x1, z1, hmax), P(xr, z1 - inset, hmax + r)]]
                cols += [rc if kind == "hip" else col] * 2
    # chimneys
    ridge = max(p[2] for poly in polys for p in poly)
    for c in h.chimney_stacks:
        x, z, w, d = c["x"], c["z"], c["w"], c["d"]
        top = ridge + c["above_ridge"]
        for f in ([(x - w / 2, z - d / 2), (x + w / 2, z - d / 2)], [(x + w / 2, z - d / 2), (x + w / 2, z + d / 2)],
                  [(x + w / 2, z + d / 2), (x - w / 2, z + d / 2)], [(x - w / 2, z + d / 2), (x - w / 2, z - d / 2)]):
            polys.append([(f[0][0], f[0][1], 2.0), (f[1][0], f[1][1], 2.0), (f[1][0], f[1][1], top), (f[0][0], f[0][1], top)]); cols.append("#8a3a2a")
    # windows + doors on exterior faces (just coloured quads)
    for wn in h.windows:
        r = h.rooms[wn["room"]]; y = h.levels[r.level]["y"]
        if y < 0:
            continue
        x0r, z0r, x1r, z1r = r.rect
        y0, y1 = y + wn["sill"], y + wn["sill"] + wn["height"]
        a, b = wn["at"] - wn["width"] / 2, wn["at"] + wn["width"] / 2
        e = 0.03
        if wn["side"] == "N": q = [(a, z1r + e, y0), (b, z1r + e, y0), (b, z1r + e, y1), (a, z1r + e, y1)]
        elif wn["side"] == "S": q = [(a, z0r - e, y0), (b, z0r - e, y0), (b, z0r - e, y1), (a, z0r - e, y1)]
        elif wn["side"] == "E": q = [(x1r + e, a, y0), (x1r + e, b, y0), (x1r + e, b, y1), (x1r + e, a, y1)]
        else: q = [(x0r - e, a, y0), (x0r - e, b, y0), (x0r - e, b, y1), (x0r - e, a, y1)]
        polys.append(q); cols.append("#ff6a5a" if wn["weak"] else "#1a2a33")
    from houses_colour import DOOR_RGB
    for d in h.doors:
        if d["room_b"] != "ext":
            continue
        r = h.rooms[d["room_a"]]; y = h.levels[r.level]["y"]
        x0r, z0r, x1r, z1r = r.rect
        a, b = d["at"] - d["width"] / 2, d["at"] + d["width"] / 2
        e = 0.04; y1 = y + d["height"]
        if d["side"] == "N": q = [(a, z1r + e, y), (b, z1r + e, y), (b, z1r + e, y1), (a, z1r + e, y1)]
        elif d["side"] == "S": q = [(a, z0r - e, y), (b, z0r - e, y), (b, z0r - e, y1), (a, z0r - e, y1)]
        elif d["side"] == "E": q = [(x1r + e, a, y), (x1r + e, b, y), (x1r + e, b, y1), (x1r + e, a, y1)]
        else: q = [(x0r - e, a, y), (x0r - e, b, y), (x0r - e, b, y1), (x0r - e, a, y1)]
        polys.append(q); cols.append(DOOR_RGB.get(d.get("colour") or "", "#4a3a2a"))
    pc = Poly3DCollection(polys, facecolors=cols, edgecolors="#111111", linewidths=0.4)
    ax.add_collection3d(pc)
    x0, z0, x1, z1 = h.footprint()
    span = max(x1 - x0, z1 - z0, ridge + 2)
    cx, cz = (x0 + x1) / 2, (z0 + z1) / 2
    ax.set_xlim(cx - span / 2, cx + span / 2); ax.set_ylim(cz - span / 2, cz + span / 2); ax.set_zlim(0, span)
    ax.view_init(elev=elev, azim=azim)
    ax.set_axis_off()
    ax.set_title(f"{h.slug} {h.name} - massing (front = +z, view from front-left)", color="#efe4c8", fontsize=10)
    plt.tight_layout(); plt.savefig(out_path, facecolor=fig.get_facecolor()); plt.close()
