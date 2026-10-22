"""Village layout v2 for Hollowmere, matching docs/DESIGN.md "The village map" and the existing Greybox builder:
  * 300 x 300 m inside the ring wall (x, z in [-150, 150]), gate at (0, 150) on the NORTH edge
  * square + altar at (0, 112), bell tower (26, 132), notice board (14, 96), chapel (-60, 112)  <- unchanged from the builder
  * fog road north of the gate to the bridge at z = 352                                     <- unchanged
World axes = Unity: +x east, +z north, y up. Yaw = Unity Y rotation (clockwise from +z when seen from above).
"""
import math
from houses_a import h00, h01, h02, h03, h04
from houses_b import h05, h06, h07, h08, h09
from houses_c import h10, h11, h12, h13, h14

HOUSE_FUNCS = [h00, h01, h02, h03, h04, h05, h06, h07, h08, h09, h10, h11, h12, h13, h14]

# --------------------------------------------------------------------------------------------- houses in the world
# origin = house-local (0,0,0); yaw = direction the FRONT (local +z) faces; street = the lane the front door opens onto
PLACEMENT = {
    "H00": dict(pos=(10.6, 52.0), yaw=270, street="Main Street (east side)", plot=dict(front=1.0, back=9.0, side=0.3), ground=0.45,
                note="pavement-line town house; floor +0.45 so 3 steps up to the blue door"),
    "H01": dict(pos=(36.0, 6.0), yaw=262, street="The Green (east side)", plot=dict(front=6.0, back=16.0, side=5.0), ground=0.1,
                note="walled vegetable garden behind (east)"),
    "H02": dict(pos=(25.5, -90.0), yaw=270, street="Main Street south (east side, by the stream bridge)", plot=dict(front=4.0, back=8.0, side=3.0), ground=-0.3,
                note="isolated thatched cottage; porch faces the lane"),
    "H03": dict(pos=(56.0, 58.0), yaw=0, street="School Lane (south side, opposite the school)", plot=dict(front=2.0, back=6.0, side=2.0), ground=0.2,
                note="faces the schoolhouse across School Lane"),
    "H04": dict(pos=(-53.8, -32.2), yaw=135, street="Mill Lane (north-west side)", plot=dict(front=1.0, back=10.0, side=2.0), ground=-0.2,
                note="long axis runs along the lane; drying green behind"),
    "H05": dict(pos=(-62.0, 48.0), yaw=90, street="Chapel Lane (west side)", plot=dict(front=4.0, back=10.0, side=4.0), ground=0.3,
                note="gravel drive to the surgery door"),
    "H06": dict(pos=(-24.0, 29.0), yaw=180, street="The Green (north side)", plot=dict(front=4.0, back=8.0, side=1.5), ground=0.0,
                note="two front gardens split by a wall, facing the green"),
    "H07": dict(pos=(-42.0, 82.0), yaw=270, street="Chapel Lane (east side)", plot=dict(front=2.0, back=4.0, side=2.0), ground=0.3,
                note="organ bay (local +x) points north toward the chapel; rear wing backs onto Back Lane"),
    "H08": dict(pos=(-7.75, 66.0), yaw=90, street="Main Street (west side)", plot=dict(front=0.3, back=10.0, side=0.3), ground=0.0,
                note="shop on the pavement; slaughter yard behind, opening onto Back Lane"),
    "H09": dict(pos=(-104.0, -96.0), yaw=0, street="Mill Lane (end, at the pond)", plot=dict(front=4.0, back=5.0, side=1.0), ground=-0.6,
                note="waterwheel on the mill's east wall sits in the mill race"),
    "H10": dict(pos=(-8.0, 50.0), yaw=90, street="Main Street (west side)", plot=dict(front=0.3, back=4.0, side=0.3), ground=0.0,
                note="glass studio behind"),
    "H11": dict(pos=(-62.0, 80.0), yaw=90, street="Chapel Lane (west side)", plot=dict(front=4.0, back=6.0, side=2.0), ground=0.4,
                note="oratory wing (local +x) points south; Calvary in the front garden; graveyard behind to the north-west"),
    "H12": dict(pos=(10.8, 78.0), yaw=270, street="Main Street (east side, by the square)", plot=dict(front=0.5, back=6.0, side=0.5), ground=0.1,
                note="IS the village post office: public counter in the sorting wing, which points north onto the square"),
    "H13": dict(pos=(86.0, -34.0), yaw=0, street="East Road (south side)", plot=dict(front=6.0, back=8.0, side=4.0), ground=0.2,
                note="farmyard in front of the barn; fields to the south and east"),
    "H14": dict(pos=(-29.0, -4.0), yaw=90, street="The Green (west side)", plot=dict(front=0.3, back=6.0, side=1.0), ground=0.0,
                note="oldest house; jetty overhangs the green path; leans 2 deg east"),
}

# --------------------------------------------------------------------------------------------- fixed landmarks (unchanged from the builder unless noted)
LANDMARKS = [
    dict(id="L_GATE", name="Village gate", pos=(0, 150), size=(10.4, 1.6), yaw=0, kind="gate", keep=True,
         note="existing BuildGate; 'DO NOT LOOK BACK' inscription; ring wall continues E and W"),
    dict(id="L_SQUARE", name="Town square (cobbled)", pos=(0, 117), size=(70, 60), yaw=0, kind="area", keep=True,
         note="existing Paving 70 x 60; add a 0.15 m kerb + 2 m pavement round its edge, 4 benches, 2 street lamps"),
    dict(id="L_ALTAR", name="Altar + stone circle", pos=(0, 112), size=(16, 16), yaw=0, kind="ritual", keep=True, note="existing"),
    dict(id="L_BELL", name="Bell tower", pos=(26, 132), size=(4, 4), yaw=0, kind="tower", keep=True,
         note="existing; dress with StoneBlock + a louvred belfry + clock face (stopped)"),
    dict(id="L_BOARD", name="Notice board", pos=(14, 96), size=(3.8, 0.6), yaw=180, kind="board", keep=True, note="existing"),
    dict(id="L_CHAPEL", name="Chapel", pos=(-60, 112), size=(12, 18), yaw=90, kind="chapel", keep=True,
         note="existing 12 x 18 shell; dress: ChapelWall, ChapelRoof, a west bellcote, 4 lancets per side, porch on the east door"),
    dict(id="L_GRAVE", name="Graveyard", pos=(-72, 104), size=(30, 34), yaw=0, kind="area",
         note="dry-stone wall 1.2 m, LYCHGATE at (-56,88) on Chapel Lane, ~60 Gravestone, 4 AngelStatues, 2 yews (SmallTree dark), "
              "1 open grave; Hollow night spawn stays inside the chapel"),
    dict(id="L_STORE", name="Bakery & General Store", pos=(44, 112), size=(12, 9), yaw=270, kind="shop",
         note="DESIGN zone: by day villagers, tools, batteries; by night locked and dark. Shop window 'HOLLOWMERE STORES - BAKER', "
              "bread oven chimney, a delivery van (CoveredCar) outside"),
    dict(id="L_SCHOOL", name="Schoolhouse", pos=(66, 80), size=(18, 9), yaw=180, kind="school",
         note="DESIGN zone: single tall room + bell cupola (Bell scaled 0.5), 12 SchoolDesk + SchoolChair, Blackboard, "
              "children's drawings on the walls; schoolyard 26 x 16 to the north with a wall (2.2) and swings"),
    dict(id="L_GREEN", name="Village green", pos=(0, 4), size=(44, 30), yaw=0, kind="area",
         note="grass oval with worn paths; the WELL at (-4, 4); WAR MEMORIAL at (10, 8); 2 benches; an old oak (SmallTree scaled 2.5)"),
    dict(id="L_WELL", name="Well", pos=(-4, 4), size=(2.6, 2.6), yaw=0, kind="well", note="DESIGN zone: sounds come up from below at night"),
    dict(id="L_MEMORIAL", name="War memorial", pos=(10, 8), size=(1.8, 1.8), yaw=180, kind="memorial", note="names on it match graveyard family names"),
    dict(id="L_KIOSK", name="Telephone kiosk", pos=(-11, 23), size=(1.0, 1.0), yaw=180, kind="kiosk", note="dead line; rings once at midnight"),
    dict(id="L_SHELTER", name="Bus shelter", pos=(7, 32), size=(3.0, 1.5), yaw=270, kind="shelter", note="timetable: last bus 1977"),
    dict(id="L_POND", name="Mill pond", pos=(-112, -76), size=(30, 16), yaw=0, kind="water", note="water plane at -0.8; reeds; sunk rowing boat"),
    dict(id="L_FIELDS", name="Fields & scarecrows", pos=(95, -105), size=(90, 70), yaw=0, kind="fields",
         note="tall crop (wheat stubble + standing maize strip 2.2 m for hiding), 6 SCARECROWS (puzzle H13), hedges, a field gate"),
    dict(id="L_PINFOLD", name="Pinfold (animal pound)", pos=(-46, -10), size=(8, 8), yaw=0, kind="pinfold", note="round dry-stone pen 1.5 m"),
]

FILLER = [  # non-allotted, mostly boarded/locked; give the village density. size = (w, d), yaw = front
    ("F01", "terrace pair, boarded (Main St)", (-7.8, 80), (10, 7), 90), ("F02", "cottage, boarded (Main St)", (10.0, 62), (8, 7), 270),
    ("F03", "cottage, walk-through ruin (Main St)", (8.6, 40), (8, 7), 270), ("F04", "cottage, boarded (Main St)", (-9.2, 36), (8, 7), 90),
    ("F05", "sexton's cottage", (-44, 62), (7, 6), 270), ("F06", "forge (open-sided)", (-46, 6), (8, 6), 90),
    ("F07", "cottage, boarded", (26, -20), (8, 6), 300), ("F08", "pair of cottages", (-8, -36), (12, 7), 90),
    ("F09", "cottage, boarded (Mill Lane)", (-64.3, -65.7), (8, 6), 315), ("F10", "garage + petrol pump (1960s)", (40, 30), (9, 7), 270),
    ("F11", "cottage, boarded", (64, 22), (8, 7), 0), ("F12", "Dutch barn (open)", (118, -36), (14, 8), 0),
    ("F13", "cottage, boarded", (2.5, -66), (8, 6), 90), ("F14", "allotment sheds x3", (66, -34), (9, 4), 0),
    ("F15", "pump house", (-84, -94), (4, 4), 0), ("F16", "cottage, boarded", (44, 92), (8, 7), 180),
]

ROADS = [  # (id, name, kind, width, points)
    ("R1", "Main Street", "street", 6.0, [(0, 87), (0.7, 72), (1.5, 60), (0.8, 45), (-1, 32), (0, 19)]),
    ("R2", "Main Street south", "street", 5.0, [(0, -11), (4, -30), (8, -55), (12, -78), (16, -100), (19, -114), (21, -124)]),
    ("R3", "Chapel Lane", "lane", 4.5, [(-35, 104), (-46, 100), (-52, 88), (-53, 70), (-51, 52), (-46, 32), (-36, 16), (-22, 8)]),
    ("R4", "School Lane", "lane", 4.5, [(1, 70), (20, 70), (40, 69), (58, 67.4), (80, 66)]),
    ("R5", "Mill Lane", "lane", 4.0, [(-20, -12), (-28, -20), (-40, -30), (-56, -46), (-72, -62), (-88, -78), (-96, -86)]),
    ("R6", "East Road", "lane", 4.5, [(22, -2), (44, -8), (62, -16), (82, -22), (110, -24), (128, -26)]),
    ("R7", "Back Lane", "track", 3.0, [(-24, 88), (-25, 70), (-24, 52), (-22, 40)]),
    ("R8", "Field Track", "track", 3.0, [(100, -24), (104, -50), (96, -76), (88, -100)]),
    ("R9", "Green loop", "path", 2.5, [(-22, 8), (-21, -8), (-8, -14), (10, -14), (21, -6), (22, 10), (10, 20), (-8, 20), (-22, 8)]),
    ("R10", "Pond path", "path", 2.0, [(-96, -86), (-103.5, -90)]),
    ("R13", "Butcher's Alley", "path", 2.0, [(1.4, 58.5), (-24.5, 58.5)]),
]

STREAM = [(-150, -62), (-132, -68), (-124, -74), (-112, -76), (-98, -80), (-91.5, -88), (-91.5, -100), (-80, -108), (-40, -112),
          (-5, -116), (19, -114), (50, -120), (90, -128), (150, -136)]
BRIDGES = [("B1", "Main Street stone bridge (2 arches)", (19, -114), 6.0, 10.0), ("B2", "Mill race footbridge", (-93.5, -84.5), 2.0, 5.0)]

BLOCKERS = [  # DESIGN: "A fallen tree or cart blocks one random path" - MatchPlan picks 1 of these per match
    ("BL1", "FallenTree", (-52.5, 76)), ("BL2", "BrokenCart", (0, 38)), ("BL3", "FallenTree", (-48, -38)),
    ("BL4", "BrokenCart", (52, -12)), ("BL5", "FallenTree", (9, -60)), ("BL6", "BrokenCart", (30, 69.6)),
]
SCARECROWS = [(70, -80), (88, -92), (106, -84), (118, -110), (96, -126), (74, -118)]
VILLAGE_KEYS = [  # asset keys the village dressing uses (besides the houses) - drives the download manifest
    "StreetLamp", "StreetLamp2", "Gravestone", "AngelStatues", "SmallTree", "Shrub1", "Shrub2", "Shrub3", "Shrub4", "Fern", "Nettles",
    "GrassClump", "GrassClump2", "DeadTrunk", "DeadTrunk2", "Stump", "Stump2", "Rocks", "Boulder", "Branches", "CoveredCar", "TrashCan",
    "WheelRim", "OldTyre", "Bell", "BenchPainted", "CompostBags", "VegBeds", "Beehive", "WashingLine", "FencePicket", "FenceWattle",
    "FenceRail", "DryStoneWall", "GardenGate", "GateLatch", "IronGate", "Well", "Signpost", "WarMemorial", "PhoneKiosk", "BusShelter",
    "PostBoxPillar", "FallenTree", "BrokenCart", "Scarecrow", "HayBales", "Wheelbarrow", "CartWheel", "Barrel", "BarrelWorn", "Crate",
    "Bucket", "MetalBucket", "WateringCan", "PlanterBox1", "PlanterBox2", "PlanterPot", "GardenGnome", "Sprinkler", "GardenHose", "FirePit",
    "PowerPoles", "UtilityBox", "Ladder", "Spade", "Pickaxe", "Axe", "Hatchet", "Moss", "Dandelion", "Periwinkle", "RootCluster", "BarkDebris",
    "FirSapling", "PineSapling", "FirTree", "PineTree", "PropaneTank", "Jerrycan", "Generator", "OilTin", "CanRusted", "CementBag", "HandTruck",
    "ChurchPew", "Lectern", "Chalkboard", "SchoolDesk", "SchoolChair", "Blackboard", "Counter", "Shelf", "Lantern", "WoodenLantern",
    "GroundLeaves", "GroundGrassPath", "GroundGravel", "GroundMud", "GroundPark", "Cobbles2", "RockFloor", "PavingSlabs", "Paving", "Road",
    "Ground", "Stone", "StandingStone", "ChapelWall", "ChapelRoof", "Roof", "HouseWall", "Planks", "BrickPaintedWorn", "RoofTilesGrey",
    "CorrugatedRust", "RawPlankWall", "Weatherboard",
]
for _k in VILLAGE_KEYS:
    from catalog import ALL as _ALL
    assert _k in _ALL, _k

STREET_LAMPS = [(-6, 90), (6, 92), (-4, 60), (5, 44), (-5, 22), (6, -18), (-26, 2), (-50, 56), (-48, 92), (24, 69), (48, 67), (14, -60), (-40, -34)]


# ============================================================================================= geometry helpers
def rot(x, z, yaw):
    a = math.radians(yaw)
    return (x * math.cos(a) + z * math.sin(a), -x * math.sin(a) + z * math.cos(a))


def to_world(pl, x, z):
    dx, dz = rot(x, z, pl["yaw"])
    return (pl["pos"][0] + dx, pl["pos"][1] + dz)


def rect_poly(cx, cz, w, d, yaw):
    pts = [(-w / 2, -d / 2), (w / 2, -d / 2), (w / 2, d / 2), (-w / 2, d / 2)]
    return [(cx + rot(x, z, yaw)[0], cz + rot(x, z, yaw)[1]) for x, z in pts]


def local_rect_poly(pl, r):
    x0, z0, x1, z1 = r
    return [to_world(pl, x, z) for x, z in ((x0, z0), (x1, z0), (x1, z1), (x0, z1))]


def sat_overlap(a, b, margin=0.0):
    for poly in (a, b):
        n = len(poly)
        for i in range(n):
            x1, z1 = poly[i]; x2, z2 = poly[(i + 1) % n]
            ax, az = -(z2 - z1), (x2 - x1)
            L = math.hypot(ax, az); ax /= L; az /= L
            pa = [p[0] * ax + p[1] * az for p in a]; pb = [p[0] * ax + p[1] * az for p in b]
            if max(pa) + margin <= min(pb) or max(pb) + margin <= min(pa):
                return False
    return True


def seg_dist(p, a, b):
    ax, az = a; bx, bz = b; px, pz = p
    dx, dz = bx - ax, bz - az
    L2 = dx * dx + dz * dz
    t = 0 if L2 == 0 else max(0, min(1, ((px - ax) * dx + (pz - az) * dz) / L2))
    return math.hypot(px - (ax + t * dx), pz - (az + t * dz))


def poly_contains(poly, p):
    x, z = p; inside = False
    n = len(poly)
    for i in range(n):
        x1, z1 = poly[i]; x2, z2 = poly[(i + 1) % n]
        if (z1 > z) != (z2 > z) and x < (x2 - x1) * (z - z1) / (z2 - z1) + x1:
            inside = not inside
    return inside


def road_poly_dist(road_pts, poly):
    """Min distance from a polyline to a polygon (0 if crossing)."""
    best = 1e9
    for i in range(len(road_pts) - 1):
        a, b = road_pts[i], road_pts[i + 1]
        for k in range(21):
            t = k / 20
            p = (a[0] + (b[0] - a[0]) * t, a[1] + (b[1] - a[1]) * t)
            if poly_contains(poly, p):
                return 0.0
        for j in range(len(poly)):
            best = min(best, seg_dist(poly[j], a, b))
        for j in range(len(poly)):
            c, d = poly[j], poly[(j + 1) % len(poly)]
            best = min(best, seg_dist(a, c, d), seg_dist(b, c, d))
    return best


def path_len(pts):
    return sum(math.hypot(pts[i + 1][0] - pts[i][0], pts[i + 1][1] - pts[i][1]) for i in range(len(pts) - 1))


# ============================================================================================= build + check
def build():
    houses = [f() for f in HOUSE_FUNCS]
    out = dict(houses=[], landmarks=LANDMARKS, filler=[], roads=[], problems=[])
    footprints = []
    for h in houses:
        pl = PLACEMENT[h.slug]
        wings = [local_rect_poly(pl, w["rect"]) for w in h.wings]
        x0, z0, x1, z1 = h.footprint()
        p = pl["plot"]
        plot = local_rect_poly(pl, (x0 - p["side"], z0 - p["back"], x1 + p["side"], z1 + p["front"]))
        front = [d for d in h.doors if d["role"] == "front"][0]
        fr = h.rooms[front["room_a"]].rect
        lx = front["at"] if front["side"] in "NS" else (fr[0] if front["side"] == "W" else fr[2])
        lz = (fr[3] if front["side"] == "N" else fr[1]) if front["side"] in "NS" else front["at"]
        door_w = to_world(pl, lx, lz)
        out["houses"].append(dict(slug=h.slug, name=h.name, pos=pl["pos"], yaw=pl["yaw"], ground=pl["ground"], street=pl["street"],
                                  note=pl["note"], wings=wings, plot=plot, front_door=door_w))
        footprints.append((h.slug, wings, plot))
    for fid, name, pos, size, yaw in FILLER:
        poly = rect_poly(pos[0], pos[1], size[0], size[1], yaw)
        out["filler"].append(dict(id=fid, name=name, pos=pos, size=size, yaw=yaw, poly=poly))
        footprints.append((fid, [poly], poly))
    lm_polys = []
    for lm in LANDMARKS:
        if lm["kind"] in ("area", "fields", "water", "ritual"):
            continue
        poly = rect_poly(lm["pos"][0], lm["pos"][1], lm["size"][0], lm["size"][1], lm["yaw"])
        lm_polys.append((lm["id"], poly))
        footprints.append((lm["id"], [poly], poly))
    # overlaps: building vs building (+1.5 m), plots vs other buildings
    for i in range(len(footprints)):
        for j in range(i + 1, len(footprints)):
            a, b = footprints[i], footprints[j]
            if any(sat_overlap(p, q, -1.5) for p in a[1] for q in b[1]):
                out["problems"].append(f"buildings too close: {a[0]} / {b[0]}")
            elif a[0].startswith("H") and any(sat_overlap(a[2], q) for q in b[1]):
                out["problems"].append(f"plot of {a[0]} overlaps building {b[0]}")
            elif b[0].startswith("H") and any(sat_overlap(b[2], q) for q in a[1]):
                out["problems"].append(f"plot of {b[0]} overlaps building {a[0]}")
    # roads vs buildings: centreline must be >= width/2 + 0.5 from every building
    for rid, name, kind, width, pts in ROADS:
        for fid, wings, plot in footprints:
            for w in wings:
                d = road_poly_dist(pts, w)
                if d < width / 2 + 0.5:
                    out["problems"].append(f"road {rid} {name} hits {fid} (clearance {d:.1f} m)")
        out["roads"].append(dict(id=rid, name=name, kind=kind, width=width, points=pts, length=round(path_len(pts), 1)))
    # front door must be within 14 m of a road and inside the ring wall incl. forest belt (|x|,|z| <= 128)
    for hh in out["houses"]:
        dmin = min(min(seg_dist(hh["front_door"], r[4][k], r[4][k + 1]) for k in range(len(r[4]) - 1)) for r in ROADS)
        hh["door_to_road"] = round(dmin, 1)
        if dmin > 14:
            out["problems"].append(f"{hh['slug']} front door {dmin:.1f} m from any road")
        for x, z in sum(hh["wings"], []):
            if abs(x) > 130 or abs(z) > 140:
                out["problems"].append(f"{hh['slug']} inside the forest belt / wall zone at ({x:.0f},{z:.0f})")
                break
        hh["to_altar"] = round(math.hypot(hh["front_door"][0] - 0, hh["front_door"][1] - 112), 1)
    return houses, out


if __name__ == "__main__":
    houses, out = build()
    for hh in out["houses"]:
        print(f"{hh['slug']} {hh['name']:<22} door ({hh['front_door'][0]:7.1f},{hh['front_door'][1]:7.1f})  to altar {hh['to_altar']:6.1f} m"
              f"  to road {hh['door_to_road']:4.1f} m")
    print("\nproblems:" if out["problems"] else "\nno problems")
    for p in out["problems"]:
        print(" -", p)
