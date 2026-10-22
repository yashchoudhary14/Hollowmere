"""Tiny house-description engine: rooms, openings, stairs and prop placement with validation.

Conventions (match GreyboxVillageBuilder):
  * metres, house-local space, origin = centre of the main wing at ground-floor level (y = 0)
  * +z = the FRONT of the house (front door wall), +x = right when standing in the street looking at the front
    door... no: +x is the house's local +x; the village JSON gives each house a world position and yaw.
  * Unity yaw (degrees, clockwise seen from above): 0 -> prop front faces +z, 90 -> +x, 180 -> -z, 270 -> -x
  * room rect = (x0, z0, x1, z1). Rects reach the OUTER face of exterior walls and the CENTRE line of interior
    walls; exterior wall thickness is taken inside the rect, interior walls straddle shared edges.
"""
import math
from catalog import ALL

INT_T = 0.12          # interior partition thickness
DOOR_CLEAR = 0.85     # free depth in front of every door (both sides)


class Room:
    def __init__(s, house, id, name, level, rect, kind, floor, walls, ceiling="CeilingInterior", light="pendant",
                 note="", skirting="dark oak 0.15", wall_lower=None):
        s.house, s.id, s.name, s.level, s.rect, s.kind = house, id, name, level, tuple(rect), kind
        s.floor, s.walls, s.ceiling, s.light, s.note, s.skirting, s.wall_lower = floor, walls, ceiling, light, note, skirting, wall_lower

    @property
    def w(s): return s.rect[2] - s.rect[0]
    @property
    def d(s): return s.rect[3] - s.rect[1]
    @property
    def cx(s): return (s.rect[0] + s.rect[2]) / 2
    @property
    def cz(s): return (s.rect[1] + s.rect[3]) / 2

    def inset(s, side):
        return s.house.edge_inset(s, side)

    def inner(s):
        x0, z0, x1, z1 = s.rect
        return (x0 + s.inset("W"), z0 + s.inset("S"), x1 - s.inset("E"), z1 - s.inset("N"))


class House:
    def __init__(s, idx, slug, name, family, part, puzzle_kind, door_colour, chimneys, clock_hour, loud=False):
        s.idx, s.slug, s.name, s.family, s.part = idx, slug, name, family, part
        s.puzzle_kind, s.door_colour, s.chimneys, s.clock_hour, s.loud = puzzle_kind, door_colour, chimneys, clock_hour, loud
        s.levels = {}          # name -> dict(y=, h=, label=)
        s.wings = []
        s.rooms = {}
        s.doors, s.windows, s.stairs, s.props = [], [], [], []
        s.chimney_stacks = []
        s.exterior = []        # free-form exterior features (porch, bay, dormers, sign, gutters...)
        s.plot = []            # garden / yard features (local coords)
        s.anchors = []         # gameplay anchors: item spots, note spots, puzzle, container, shrine, hiding
        s.traits = {}
        s.text = {}
        s._pid = 0

    # ------------------------------------------------------------------ structure
    def level(s, name, y, h, label):
        s.levels[name] = dict(y=y, h=h, label=label)

    def wing(s, id, rect, levels, walls, t, roof, note=""):
        s.wings.append(dict(id=id, rect=tuple(rect), levels=levels, walls=walls, t=t, roof=roof, note=note))

    def room(s, id, name, level, rect, kind, floor, walls, **kw):
        assert id not in s.rooms, id
        r = Room(s, id, name, level, rect, kind, floor, walls, **kw)
        s.rooms[id] = r
        return r

    def wing_of(s, room):
        x0, z0, x1, z1 = room.rect
        for w in s.wings:
            a = w["rect"]
            if room.level in w["levels"] and a[0] - 1e-6 <= x0 and a[1] - 1e-6 <= z0 and x1 <= a[2] + 1e-6 and z1 <= a[3] + 1e-6:
                return w
        raise ValueError(f"{s.slug}: room {room.id} not inside any wing on level {room.level}")

    def neighbours(s, room, side):
        """Rooms on the same level sharing (part of) the given edge."""
        x0, z0, x1, z1 = room.rect
        out = []
        for o in s.rooms.values():
            if o is room or o.level != room.level:
                continue
            a = o.rect
            if side == "N" and abs(a[1] - z1) < 1e-6 and min(x1, a[2]) - max(x0, a[0]) > 1e-3: out.append(o)
            if side == "S" and abs(a[3] - z0) < 1e-6 and min(x1, a[2]) - max(x0, a[0]) > 1e-3: out.append(o)
            if side == "E" and abs(a[0] - x1) < 1e-6 and min(z1, a[3]) - max(z0, a[1]) > 1e-3: out.append(o)
            if side == "W" and abs(a[2] - x0) < 1e-6 and min(z1, a[3]) - max(z0, a[1]) > 1e-3: out.append(o)
        return out

    def edge_is_exterior(s, room, side):
        x0, z0, x1, z1 = room.rect
        span = (x1 - x0) if side in "NS" else (z1 - z0)
        covered = 0.0
        for o in s.neighbours(room, side):
            a = o.rect
            covered += (min(x1, a[2]) - max(x0, a[0])) if side in "NS" else (min(z1, a[3]) - max(z0, a[1]))
        return covered < span - 1e-3

    def edge_inset(s, room, side):
        if s.edge_is_exterior(room, side):
            return s.wing_of(room)["t"]
        return INT_T / 2

    def door(s, id, level, room_a, side, at, width=0.9, height=2.05, room_b="ext", kind="panel", mat="Door", colour=None,
             role="", note="", swing="in"):
        s.doors.append(dict(id=id, level=level, room_a=room_a, room_b=room_b, side=side, at=at, width=width, height=height,
                            kind=kind, mat=mat, colour=colour, role=role, note=note, swing=swing))

    def window(s, id, room, side, at, width, height, sill=0.9, style="sash", weak=False, curtains=None, shutters=None, note=""):
        s.windows.append(dict(id=id, room=room, side=side, at=at, width=width, height=height, sill=sill, style=style,
                              weak=weak, curtains=curtains, shutters=shutters, note=note))

    def stair(s, id, room, rect, frm, to, ascent, kind="straight", mat="FloorDarkPlanks", note=""):
        s.stairs.append(dict(id=id, room=room, rect=tuple(rect), frm=frm, to=to, ascent=ascent, kind=kind, mat=mat, note=note))

    def chimney(s, x, z, w=0.8, d=0.8, above_ridge=1.0, mat="Chimney", serves="", pots=1):
        s.chimney_stacks.append(dict(x=x, z=z, w=w, d=d, above_ridge=above_ridge, mat=mat, serves=serves, pots=pots))

    def ext(s, text): s.exterior.append(text)
    def garden(s, text): s.plot.append(text)

    # ------------------------------------------------------------------ props
    def _new(s, room, key, x, y, z, yaw, role="", note="", on=None, mat=None, scale=None):
        assert key in ALL, f"{s.slug}: unknown asset key {key}"
        assert room in s.rooms, f"{s.slug}: unknown room {room}"
        s._pid += 1
        p = dict(id=f"{s.slug}.p{s._pid:03d}", n=s._pid, room=room, key=key, x=round(x, 3), y=round(y, 3), z=round(z, 3),
                 yaw=yaw % 360, role=role, note=note, on=on, mat=mat, scale=scale)
        s.props.append(p)
        return p

    def dims(s, key):
        d = ALL[key].get("dims")
        return d if d else (0.3, 0.3, 0.3)

    def fp(s, p):
        """Axis-aligned footprint (x0,z0,x1,z1) of a prop (yaw rounded to 90)."""
        w, d, h = s.dims(p["key"])
        if p.get("scale"):
            w, d, h = w * p["scale"], d * p["scale"], h * p["scale"]
        q = int(round(p["yaw"] / 90.0)) % 4
        if q % 2 == 1:
            w, d = d, w
        return (p["x"] - w / 2, p["z"] - d / 2, p["x"] + w / 2, p["z"] + d / 2)

    def top(s, p):
        h = s.dims(p["key"])[2] * (p.get("scale") or 1)
        return p["y"] + h

    def wall(s, room, side, key, at=None, gap=0.03, y=0.0, role="", note="", mat=None, yaw_extra=0, scale=None, depth_off=0.0):
        """Place a prop with its back against a wall of a room. `at` = absolute coordinate along the wall
        (x for N/S walls, z for E/W walls); default = wall centre."""
        r = s.rooms[room]
        ix0, iz0, ix1, iz1 = r.inner()
        w, d, h = s.dims(key)
        if scale: w, d, h = w * scale, d * scale, h * scale
        if side == "N":
            z = iz1 - d / 2 - gap - depth_off; x = r.cx if at is None else at; yaw = 180
        elif side == "S":
            z = iz0 + d / 2 + gap + depth_off; x = r.cx if at is None else at; yaw = 0
        elif side == "E":
            x = ix1 - d / 2 - gap - depth_off; z = r.cz if at is None else at; yaw = 270
        else:
            x = ix0 + d / 2 + gap + depth_off; z = r.cz if at is None else at; yaw = 90
        return s._new(room, key, x, y, z, yaw + yaw_extra, role, note, mat=mat, scale=scale)

    def corner(s, room, corner, key, gap=0.03, face=None, role="", note="", mat=None, scale=None):
        """Place in a corner ('NE','NW','SE','SW'), back against the first letter's wall unless face given."""
        r = s.rooms[room]
        ix0, iz0, ix1, iz1 = r.inner()
        w, d, h = s.dims(key)
        if scale: w, d, h = w * scale, d * scale, h * scale
        back = face or corner[0]
        if back in "NS":
            x = ix1 - w / 2 - gap if "E" in corner else ix0 + w / 2 + gap
            z = iz1 - d / 2 - gap if back == "N" else iz0 + d / 2 + gap
            yaw = 180 if back == "N" else 0
        else:
            z = iz1 - w / 2 - gap if "N" in corner else iz0 + w / 2 + gap
            x = ix1 - d / 2 - gap if back == "E" else ix0 + d / 2 + gap
            yaw = 270 if back == "E" else 90
        return s._new(room, key, x, 0.0, z, yaw, role, note, mat=mat, scale=scale)

    def at(s, room, key, x, z, yaw=0, y=0.0, role="", note="", mat=None, scale=None):
        return s._new(room, key, x, y, z, yaw, role, note, mat=mat, scale=scale)

    def on(s, parent, key, dx=0.0, dz=0.0, yaw=None, role="", note="", mat=None, scale=None):
        """Put a small prop on top of another (dx/dz in the parent's facing frame: +dx = parent's right)."""
        q = int(round(parent["yaw"] / 90.0)) % 4
        # parent local -> house: yaw 0: (dx, dz); 90: (dz, -dx); 180: (-dx, -dz); 270: (-dz, dx)
        ox, oz = [(dx, dz), (dz, -dx), (-dx, -dz), (-dz, dx)][q]
        p = s._new(parent["room"], key, parent["x"] + ox, s.top(parent), parent["z"] + oz,
                   parent["yaw"] if yaw is None else yaw, role, note, on=parent["id"], mat=mat, scale=scale)
        return p

    def hung(s, room, side, key, at=None, y=1.5, role="", note="", mat=None):
        """Wall-hung item (frames, mirror, clock, fuse box, switch): y = centre height above floor."""
        p = s.wall(room, side, key, at=at, gap=0.0, role=role, note=note, mat=mat)
        h = s.dims(key)[2]
        p["y"] = round(y - h / 2, 3)
        p["hung"] = True
        return p

    def anchor(s, kind, room, x, y, z, note="", yaw=0):
        s.anchors.append(dict(kind=kind, room=room, x=round(x, 3), y=round(y, 3), z=round(z, 3), yaw=yaw, note=note))

    def anchor_on(s, kind, parent, dx=0, dz=0, note=""):
        q = int(round(parent["yaw"] / 90.0)) % 4
        ox, oz = [(dx, dz), (dz, -dx), (-dx, -dz), (-dz, dx)][q]
        s.anchor(kind, parent["room"], parent["x"] + ox, s.top(parent), parent["z"] + oz, note, parent["yaw"])

    # ------------------------------------------------------------------ queries
    def level_rooms(s, lv):
        return [r for r in s.rooms.values() if r.level == lv]

    def footprint(s):
        xs0 = min(w["rect"][0] for w in s.wings); zs0 = min(w["rect"][1] for w in s.wings)
        xs1 = max(w["rect"][2] for w in s.wings); zs1 = max(w["rect"][3] for w in s.wings)
        return (xs0, zs0, xs1, zs1)


# ---------------------------------------------------------------------- validation

def _overlap(a, b, tol=0.005):
    return min(a[2], b[2]) - max(a[0], b[0]) > tol and min(a[3], b[3]) - max(a[1], b[1]) > tol


STAIR_END = 0.6       # a door may open onto the first/last 0.6 m of a flight (its foot below, its head above)


def stair_end(rect, d, depth):
    """The strip of a stair rect at the end the unit axis direction d points to."""
    x0, z0, x1, z1 = rect
    if d == (0, 1): return (x0, z1 - depth, x1, z1)
    if d == (0, -1): return (x0, z0, x1, z0 + depth)
    if d == (1, 0): return (x1 - depth, z0, x1, z1)
    return (x0, z0, x0 + depth, z1)


def _clip(a, b):
    return (max(a[0], b[0]), max(a[1], b[1]), min(a[2], b[2]), min(a[3], b[3]))


def _inside(a, b, tol=0.01):
    return a[0] >= b[0] - tol and a[1] >= b[1] - tol and a[2] <= b[2] + tol and a[3] <= b[3] + tol


def door_zone(h, dr, room_id):
    """Clearance box in front of a door, on the side of room_id."""
    r = h.rooms[room_id]
    side = dr["side"] if dr["room_a"] == room_id else {"N": "S", "S": "N", "E": "W", "W": "E"}[dr["side"]]
    w = dr["width"] + 0.2
    x0, z0, x1, z1 = r.rect
    if side == "N": return (dr["at"] - w / 2, z1 - DOOR_CLEAR, dr["at"] + w / 2, z1)
    if side == "S": return (dr["at"] - w / 2, z0, dr["at"] + w / 2, z0 + DOOR_CLEAR)
    if side == "E": return (x1 - DOOR_CLEAR, dr["at"] - w / 2, x1, dr["at"] + w / 2)
    return (x0, dr["at"] - w / 2, x0 + DOOR_CLEAR, dr["at"] + w / 2)


def validate(h):
    errs, warns = [], []
    # rooms inside wings, no overlaps
    for r in h.rooms.values():
        try: h.wing_of(r)
        except ValueError as e: errs.append(str(e))
    for lv in h.levels:
        rs = h.level_rooms(lv)
        for i, a in enumerate(rs):
            for b in rs[i + 1:]:
                if _overlap(a.rect, b.rect):
                    errs.append(f"rooms overlap {a.id}/{b.id}")
    # doors
    for dr in h.doors:
        ra = h.rooms[dr["room_a"]]
        x0, z0, x1, z1 = ra.rect
        lo, hi = (x0, x1) if dr["side"] in "NS" else (z0, z1)
        if not (lo + dr["width"] / 2 - 1e-6 <= dr["at"] <= hi - dr["width"] / 2 + 1e-6):
            errs.append(f"door {dr['id']} outside wall span of {ra.id}")
        if dr["room_b"] == "ext":
            if not h.edge_is_exterior(ra, dr["side"]):
                errs.append(f"door {dr['id']} marked exterior on an interior edge")
        else:
            rb = h.rooms[dr["room_b"]]
            if rb not in h.neighbours(ra, dr["side"]):
                errs.append(f"door {dr['id']}: {rb.id} is not across {ra.id}'s {dr['side']} wall")
            else:
                a = rb.rect
                lo2, hi2 = (a[0], a[2]) if dr["side"] in "NS" else (a[1], a[3])
                if not (max(lo, lo2) + dr["width"] / 2 - 1e-6 <= dr["at"] <= min(hi, hi2) - dr["width"] / 2 + 1e-6):
                    errs.append(f"door {dr['id']} not within shared span")
                elif not (max(lo, lo2) + dr["width"] / 2 + INT_T / 2 - 1e-6 <= dr["at"] <= min(hi, hi2) - dr["width"] / 2 - INT_T / 2 + 1e-6):
                    errs.append(f"door {dr['id']} runs into the partition at the end of its wall (clear opening narrower than {dr['width']})")
    # windows
    for wn in h.windows:
        r = h.rooms[wn["room"]]
        if not h.edge_is_exterior(r, wn["side"]):
            errs.append(f"window {wn['id']} on interior edge of {r.id}")
        x0, z0, x1, z1 = r.rect
        lo, hi = (x0, x1) if wn["side"] in "NS" else (z0, z1)
        if not (lo + wn["width"] / 2 <= wn["at"] <= hi - wn["width"] / 2):
            errs.append(f"window {wn['id']} outside span")
        for dr in h.doors:
            if dr["room_a"] == r.id and dr["side"] == wn["side"] and abs(dr["at"] - wn["at"]) < (dr["width"] + wn["width"]) / 2 + 0.1:
                errs.append(f"window {wn['id']} collides with door {dr['id']}")
    # props
    floor_props = {}
    for p in h.props:
        if p.get("on") or p.get("hung") or p["y"] > 0.3 or "allow-overlap" in (p.get("role") or ""):
            continue
        floor_props.setdefault(p["room"], []).append(p)
    for rid, ps in floor_props.items():
        r = h.rooms[rid]
        inner = r.inner()
        for p in ps:
            f = h.fp(p)
            if "flat" in ALL[p["key"]].get("tags", []) or p["key"] in ("Rug",):
                continue
            if f[0] < inner[0] - 0.02 or f[1] < inner[1] - 0.02 or f[2] > inner[2] + 0.02 or f[3] > inner[3] + 0.02:
                errs.append(f"prop {p['id']} {p['key']} outside room {rid} {tuple(round(v,2) for v in f)} vs {tuple(round(v,2) for v in inner)}")
        solid = [p for p in ps if p["key"] not in ("Rug",)]
        for i, a in enumerate(solid):
            for b in solid[i + 1:]:
                if _overlap(h.fp(a), h.fp(b)) and not ({"allow-overlap"} & set((a.get("role") or "").split()+(b.get("role") or "").split())):
                    errs.append(f"props overlap {a['id']} {a['key']} / {b['id']} {b['key']} in {rid}")
        # door clearance
        for dr in h.doors:
            if "arch" in dr["kind"]:
                continue
            for side_room in (dr["room_a"], dr["room_b"]):
                if side_room != rid:
                    continue
                z = door_zone(h, dr, rid)
                for p in solid:
                    if _overlap(h.fp(p), z):
                        errs.append(f"prop {p['id']} {p['key']} blocks door {dr['id']} in {rid}")
        # stairs
        for st in h.stairs:
            if st["room"] == rid or any(s2 == rid for s2 in st.get("also", [])):
                for p in solid:
                    if _overlap(h.fp(p), st["rect"]):
                        errs.append(f"prop {p['id']} {p['key']} on stair {st['id']}")
        # tall props in front of windows
        for wn in h.windows:
            if wn["room"] != rid:
                continue
            for p in solid:
                if h.top(p) <= wn["sill"] + 0.02:
                    continue
                f = h.fp(p)
                inner = r.inner()
                near = {"N": f[3] > inner[3] - 0.6, "S": f[1] < inner[1] + 0.6, "E": f[2] > inner[2] - 0.6, "W": f[0] < inner[0] + 0.6}[wn["side"]]
                lo, hi = (f[0], f[2]) if wn["side"] in "NS" else (f[1], f[3])
                if near and min(hi, wn["at"] + wn["width"] / 2) - max(lo, wn["at"] - wn["width"] / 2) > 0.05:
                    warns.append(f"tall prop {p['id']} {p['key']} in front of window {wn['id']}")
    # no door opens onto a stair flight or a stairwell: only onto its foot (lower floor) or its head (upper floor).
    # ascent = direction of travel from frm to to.
    for st in h.stairs:
        lv = h.rooms[st["room"]].level
        lower = st["frm"] if h.levels[st["frm"]]["y"] <= h.levels[st["to"]]["y"] else st["to"]
        dx, dz = {"N": (0, 1), "S": (0, -1), "E": (1, 0), "W": (-1, 0)}[st["ascent"]]
        if st["frm"] != lower:
            dx, dz = -dx, -dz                                   # (dx, dz) now points up the flight
        allowed = stair_end(st["rect"], (-dx, -dz) if lv == lower else (dx, dz), STAIR_END)
        for dr in h.doors:
            for side_room in (dr["room_a"], dr["room_b"]):
                if side_room == "ext" or h.rooms[side_room].level != lv:
                    continue
                z = door_zone(h, dr, side_room)
                if _overlap(z, st["rect"], tol=0.05) and not _inside(_clip(z, st["rect"]), allowed):
                    errs.append(f"door {dr['id']} opens onto stair {st['id']} ({st['kind'][:20]}) in {side_room}")
    # stair rect inside its room
    for st in h.stairs:
        r = h.rooms[st["room"]]
        a = st["rect"]; b = r.rect
        if not (a[0] >= b[0] - 1e-6 and a[1] >= b[1] - 1e-6 and a[2] <= b[2] + 1e-6 and a[3] <= b[3] + 1e-6):
            errs.append(f"stair {st['id']} outside room {r.id}")
    # requirements (docs/DESIGN.md "Houses and puzzles" - shared requirements)
    kinds = [r.kind for r in h.rooms.values()]
    tags = []
    for p in h.props:
        tags += ALL[p["key"]].get("tags", [])
        tags += (p.get("role") or "").split()
    need = {"hearth": "hearth", "fuse-box": "fuse box", "phone": "phone", "mirror": "mirror", "family-photo": "family photo",
            "tv|radio": "radio or TV"}
    for k, label in need.items():
        if not any(t in tags for t in k.split("|")):
            errs.append(f"missing required {label}")
    hiding = [a for a in h.anchors if a["kind"] == "hiding"]
    if not 4 <= len(hiding) <= 6:
        errs.append(f"hiding spots {len(hiding)} (need 4-6)")
    ext_doors = [d for d in h.doors if d["room_b"] == "ext"]
    if not any(d["role"] == "front" for d in ext_doors) or not any(d["role"] == "back" for d in ext_doors):
        errs.append("needs front and back doors")
    n_win = len(h.windows)
    weak = [w for w in h.windows if w["weak"]]
    if not 6 <= n_win <= 10: warns.append(f"{n_win} windows (design says 6-8)")
    if len(weak) != 2: errs.append(f"{len(weak)} weak windows (need exactly 2)")
    for k in ("kitchen", "living", "bedroom", "bathroom"):
        if k not in kinds: errs.append(f"missing room kind {k}")
    if kinds.count("bedroom") < 2: errs.append("needs two bedrooms")
    if not any(k in kinds for k in ("cellar", "attic")): errs.append("needs cellar or attic")
    for k in ("item-spot", "note-spot", "puzzle", "container", "shrine"):
        n = len([a for a in h.anchors if a["kind"] == k])
        if n == 0: errs.append(f"missing anchor {k}")
    if len([a for a in h.anchors if a["kind"] == "item-spot"]) < 3: errs.append("needs >=3 item spots")
    if len([a for a in h.anchors if a["kind"] == "note-spot"]) != 3: errs.append("needs exactly 3 note spots")
    if len(h.chimney_stacks) != h.chimneys: errs.append(f"chimney stacks {len(h.chimney_stacks)} != catalog {h.chimneys}")
    return errs, warns
