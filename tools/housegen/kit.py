"""Shared helpers for house definitions."""
from core import House

T_RENDER, T_BRICK, T_STONE, T_TIMBER = 0.35, 0.35, 0.5, 0.25


def levels(h, cellar=False, attic=False, g=2.8, f=2.6, first=True):
    if cellar:
        h.level("C", -2.4, 2.2, "Cellar")
    h.level("G", 0.0, g, "Ground floor")
    if first:
        h.level("F", g + 0.25, f, "First floor")
    if attic:
        h.level("A", g + 0.25 + f + 0.25 if first else g + 0.25, 2.3, "Attic (under the roof)")


def hide(h, p, label, crouch=False):
    q = (p["yaw"] // 90) % 4
    h.anchor("hiding", p["room"], p["x"], p["y"], p["z"],
             f"{label} -> HidingSpot on {p['id']} ({p['key']}){' - crouched' if crouch else ''}", p["yaw"])


def hide_at(h, room, x, z, label, yaw=0):
    h.anchor("hiding", room, x, 0.0, z, label, yaw)


def item(h, p, label, dx=0.0, dz=0.0):
    h.anchor_on("item-spot", p, dx, dz, label)


def note(h, p, label="", dx=-0.1, dz=0.0):
    h.anchor_on("note-spot", p, dx, dz, label or f"Readable note on {p['key']}")


def bed(h, room, side, key, at, stands=("Nightstand",), stand_side="both", hiding=True, label="bed"):
    """Bed with its head against a wall; nightstands either side."""
    p = h.wall(room, side, key, at=at)
    w = h.dims(key)[0]
    out = [p]
    for i, s in enumerate(stands):
        if not s:
            continue
        sw = h.dims(s)[0]
        sides = [-1, 1] if stand_side == "both" else ([-1] if stand_side == "left" else [1])
        for sg in sides:
            off = sg * (w / 2 + sw / 2 + 0.05)
            out.append(h.wall(room, side, s, at=at + off))
    if hiding:
        hide(h, p, f"under the {label}", crouch=True)
    return out


def shrine(h, room, side, at, note=""):
    p = h.hung(room, side, "ShrineNiche", at=at, y=1.35, role="shrine")
    h.anchor("shrine", room, p["x"], 0.0, p["z"], note or "Rite of Return point (active only if this house is a shrine this match)", p["yaw"])
    return p


def puzzle(h, p, text):
    h.anchor("puzzle", p["room"], p["x"], p["y"], p["z"], text, p["yaw"])


def container(h, p, text):
    h.anchor("container", p["room"], p["x"], p["y"], p["z"], text, p["yaw"])
