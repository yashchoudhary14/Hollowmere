import matplotlib
matplotlib.use("Agg")
import matplotlib.pyplot as plt
from matplotlib.patches import Polygon, Rectangle, Circle, Ellipse
from village import build, ROADS, STREAM, BRIDGES, BLOCKERS, SCARECROWS, STREET_LAMPS, LANDMARKS, rect_poly


def render(path="village_map_v2.png", zoom=None, dpi=120):
    houses, out = build()
    fig, ax = plt.subplots(figsize=(14, 14), dpi=dpi)
    bg = "#1c1e1a"; fig.patch.set_facecolor(bg); ax.set_facecolor("#33392c")
    # forest belt + wall
    ax.add_patch(Rectangle((-150, -150), 300, 300, fc="#1d2a1c", ec="#8a8a80", lw=3))
    ax.add_patch(Rectangle((-128, -140), 256, 268, fc="#33392c", ec="none"))
    ax.text(-146, 0, "FOREST BELT (20-25 m) + RING WALL 5 m", rotation=90, color="#7f9a7a", fontsize=8, va="center")
    for lm in LANDMARKS:
        x, z = lm["pos"]; w, d = lm["size"]
        if lm["kind"] == "area":
            col = {"L_SQUARE": "#5b5a55", "L_GREEN": "#3f5733", "L_GRAVE": "#3a3f37"}.get(lm["id"], "#444")
            ax.add_patch(Polygon(rect_poly(x, z, w, d, lm["yaw"]), fc=col, ec="#777", lw=0.6, zorder=1))
        elif lm["kind"] == "water":
            ax.add_patch(Ellipse((x, z), w, d, fc="#22343f", ec="#4a6a7a", zorder=1))
        elif lm["kind"] == "fields":
            ax.add_patch(Polygon(rect_poly(x, z, w, d, 0), fc="#4c4a2c", ec="#6a6844", lw=0.6, zorder=1))
    xs, zs = zip(*STREAM)
    ax.plot(xs, zs, color="#4a6a7a", lw=4, zorder=2)
    for rid, name, kind, width, pts in ROADS:
        xs, zs = zip(*pts)
        col = {"street": "#8c7b5e", "lane": "#7d6e55", "track": "#6a5d48", "path": "#6f6656"}[kind]
        ax.plot(xs, zs, color=col, lw=width * 1.15, solid_capstyle="round", solid_joinstyle="round", zorder=3)
        mid = pts[len(pts) // 2]
        ax.text(mid[0], mid[1] + 2.5, name, color="#e8dcb8", fontsize=6.5, ha="center", zorder=9, alpha=0.8)
    for b in BRIDGES:
        ax.add_patch(Circle(b[2], 2.5, fc="#8c7b5e", ec="white", lw=0.6, zorder=4))
    for hh in out["houses"]:
        ax.add_patch(Polygon(hh["plot"], fc="none", ec="#b9a77a", lw=0.7, ls="--", zorder=4))
        for w in hh["wings"]:
            ax.add_patch(Polygon(w, fc="#c48a3a", ec="#f2e6c8", lw=0.9, zorder=6))
        fx, fz = hh["front_door"]
        ax.add_patch(Circle((fx, fz), 0.9, fc="#3fa7ff", ec="white", lw=0.4, zorder=7))
        cx = sum(p[0] for p in hh["wings"][0]) / 4; cz = sum(p[1] for p in hh["wings"][0]) / 4
        ax.text(cx, cz, hh["slug"], color="black", fontsize=7, ha="center", va="center", weight="bold", zorder=8)
        ax.text(cx, cz - 7.5, hh["name"].replace("'s House", "").replace(" House", ""), color="#ffe7b0", fontsize=6.3, ha="center", zorder=8)
    for f in out["filler"]:
        ax.add_patch(Polygon(f["poly"], fc="#6a655a", ec="#9a958a", lw=0.5, zorder=5))
    for lm in LANDMARKS:
        if lm["kind"] in ("area", "fields", "water", "ritual"):
            if lm["kind"] == "ritual":
                ax.add_patch(Circle(lm["pos"], 7, fc="none", ec="#d0c0ff", lw=1.2, zorder=6))
            continue
        x, z = lm["pos"]
        ax.add_patch(Polygon(rect_poly(x, z, lm["size"][0], lm["size"][1], lm["yaw"]), fc="#7b6a8a", ec="#e0d6ff", lw=0.8, zorder=6))
        ax.text(x, z - lm["size"][1] / 2 - 3, lm["name"], color="#e0d6ff", fontsize=6.3, ha="center", zorder=8)
    for bid, kind, p in BLOCKERS:
        ax.plot(*p, marker="X", color="#ff5a4a", ms=7, zorder=8)
    for p in SCARECROWS:
        ax.plot(*p, marker="P", color="#e8c060", ms=7, zorder=8)
    for p in STREET_LAMPS:
        ax.plot(*p, marker="o", color="#ffd27a", ms=3, zorder=8)
    ax.plot([0, 0], [150, 175], color="#8c7b5e", lw=7, zorder=3)
    ax.text(3, 165, "fog road -> bridge (z=352)", color="#e8dcb8", fontsize=7)
    lim = zoom or (-155, 155, -155, 178)
    ax.set_xlim(lim[0], lim[1]); ax.set_ylim(lim[2], lim[3]); ax.set_aspect("equal")
    ax.grid(color="#40453a", lw=0.4); ax.tick_params(colors="#9a9a8a", labelsize=7)
    ax.set_xlabel("x (east, m)", color="#9a9a8a"); ax.set_ylabel("z (north, m)", color="#9a9a8a")
    ax.set_title("HOLLOWMERE village layout v2 - top-down, north up (Unity world metres)", color="#efe4c8", fontsize=13)
    ax.text(-150, -153, "orange = 15 allotted houses (blue dot = front door)   purple = landmarks   grey = filler (boarded)   "
            "dashed = plot boundary   X = night blocker candidates   + = scarecrows   dots = street lamps",
            color="#c9c0a8", fontsize=7.5)
    plt.tight_layout(); plt.savefig(path, facecolor=bg); plt.close()
    return out


if __name__ == "__main__":
    out = render()
    print(len(out["problems"]), "problems")
