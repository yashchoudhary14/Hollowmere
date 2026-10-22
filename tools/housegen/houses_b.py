from core import House
from kit import *


# ============================================================================================ H05 Doctor's House
def h05():
    h = House(5, "H05", "Doctor's House", "Crane", "Glass eye", "CodeLock", "White", 1, 4)
    h.traits = dict(archetype="White stucco Edwardian villa with a single-storey surgery wing",
                    silhouette="Square, white and prim: hipped red-slate roof with ONE central-west chimney, a columned porch "
                               "over the white door, a flat-roofed surgery wing on the east with its own door and a brass plate "
                               "'DR. A. CRANE  M.B.', iron railings, a monkey-puzzle tree in the front garden.")
    levels(h, cellar=True, g=3.0, f=2.8)
    h.wing("main", (-5.0, -4.5, 5.0, 4.5), ["C", "G", "F"], "StuccoWhite (smooth, grimy streaks under sills), BrickDark plinth "
           "0.6 m, painted stone quoins (pale grey)", T_RENDER,
           "Hipped roof 30 deg, RoofRedSlate, deep bracketed eaves 0.5 m; chimney on the spine wall at (-3.1, 0)")
    h.wing("surgery", (5.0, -3.0, 10.0, 3.0), ["G"], "StuccoWhite, same plinth", T_RENDER,
           "Flat roof hidden behind a 0.6 m parapet with stone coping; roof lantern (skylight) 1.2 x 1.2 over the consulting room")
    h.wing("porch", (-1.4, 4.5, 1.4, 6.0), ["G"], "Two Doric columns + pilasters, stone steps (3)", 0.2, "Flat lead porch roof")
    G, F, C = "G", "F", "C"
    h.room("porch", "Porch", G, (-1.4, 4.5, 1.4, 6.0), "porch", "FloorCheckTile", "open")
    h.room("hall", "Hall", G, (-1.2, -4.5, 1.2, 4.5), "hall", "FloorCheckTile", "Wallpaper2 above PanelDark dado")
    h.room("sitting", "Drawing room", G, (-5.0, 0.0, -1.2, 4.5), "living", "FloorParquet", "Wallpaper2C (green damask)")
    h.room("kitchen", "Kitchen", G, (-5.0, -4.5, -1.2, 0.0), "kitchen", "FloorQuarryTile", "TileWallWhite to 1.5 m")
    h.room("waiting", "Waiting room", G, (1.2, 0.0, 5.0, 4.5), "waiting", "FloorWornTile", "PlasterWorn painted institutional cream")
    h.room("disp", "Dispensary (PUZZLE)", G, (1.2, -4.5, 5.0, 0.0), "dispensary", "FloorWornTile", "TileWallWhite to 2.0 m")
    h.room("consult", "Consulting room", G, (5.0, -3.0, 10.0, 3.0), "surgery", "FloorParquet", "PanelDark to 1.2 m, PlasterWorn", light="roof lantern + 2 pendants")
    h.room("bed1", "Doctor's bedroom", F, (-5.0, 0.0, -1.2, 4.5), "bedroom", "CarpetWorn", "Wallpaper1")
    h.room("landing", "Landing", F, (-1.2, -4.5, 1.2, 4.5), "landing", "Carpet70sB", "Wallpaper2")
    h.room("bed2", "Guest bedroom", F, (1.2, 0.0, 5.0, 4.5), "bedroom", "FloorOldWood1", "Wallpaper1B")
    h.room("bath", "Bathroom", F, (-5.0, -4.5, -1.2, 0.0), "bathroom", "FloorBathTile", "TileWallWhite, radiator")
    h.room("bed3", "The boy's room (locked by day)", F, (1.2, -4.5, 5.0, 0.0), "bedroom", "FloorOldWood2", "Wallpaper1B with a height chart pencilled on the door frame")
    h.room("cellar", "Cellar", C, (-5.0, -4.5, 1.2, 4.5), "cellar", "FloorConcrete", "Whitewash", ceiling="joists")
    h.door("d_porch", G, "porch", "N", 0.0, 1.2, role="front", kind="porch opening", colour="White")
    h.door("d_front", G, "porch", "S", 0.0, 1.0, room_b="hall", kind="white 4-panel door, brass knocker, coloured-glass fanlight", height=2.3)
    h.door("d_back", G, "hall", "S", 0.5, 0.9, role="back", kind="half-glazed garden door")
    h.door("d_sit", G, "hall", "W", 2.6, 0.9, room_b="sitting")
    h.door("d_kit", G, "hall", "W", -3.0, 0.85, room_b="kitchen")
    h.door("d_wait", G, "hall", "E", 2.6, 0.9, room_b="waiting")
    h.door("d_disp", G, "hall", "E", -3.0, 0.85, room_b="disp", kind="door with a frosted 'PRIVATE' panel")
    h.door("d_cons1", G, "waiting", "E", 1.5, 0.9, room_b="consult", kind="baize-lined door")
    h.door("d_cons2", G, "disp", "E", -1.5, 0.8, room_b="consult")
    h.door("d_surg", G, "consult", "N", 7.5, 0.95, role="surgery", kind="patients' entrance, brass plate, bell pull")
    h.door("d_b1", F, "landing", "W", 2.6, 0.85, room_b="bed1")
    h.door("d_b2", F, "landing", "E", 2.6, 0.85, room_b="bed2")
    h.door("d_bath", F, "landing", "W", -3.0, 0.8, room_b="bath")
    h.door("d_b3", F, "landing", "E", -3.0, 0.8, room_b="bed3", kind="4-panel, key in the lock on the landing side")
    h.window("w_sit", "sitting", "N", -3.1, 1.5, 1.9, sill=0.6, style="tall sash 2-over-2", curtains="FabricVelvet (bottle green)")
    h.window("w_wait", "waiting", "N", 3.1, 1.5, 1.9, sill=0.6, style="tall sash", curtains="net only")
    h.window("w_k", "kitchen", "S", -3.6, 1.1, 1.3, weak=True, style="sash, cracked")
    h.window("w_disp", "disp", "S", 3.1, 1.0, 1.2, weak=True, style="sash with an iron grille (one bar missing)")
    h.window("w_cons", "consult", "S", 7.5, 1.4, 1.6, sill=0.8, style="sash, frosted lower half")
    h.window("w_b1", "bed1", "N", -3.1, 1.3, 1.6, style="sash", curtains="FabricLinen")
    h.window("w_b2", "bed2", "N", 3.1, 1.3, 1.6, style="sash")
    h.window("w_bath", "bath", "S", -3.1, 0.8, 0.9, sill=1.3, style="frosted")
    h.window("w_b3", "bed3", "S", 3.1, 1.1, 1.4, style="sash - the boy's window, with paper stars stuck on it")
    h.window("w_land", "landing", "N", 0.0, 1.0, 1.0, sill=1.2, style="round-headed stair window over the porch")
    h.stair("s1", "hall", (-1.1, -1.5, -0.2, 2.0), G, F, "S", note="18 risers 0.17, mahogany handrail, brass stair rods, Carpet70sB runner")
    h.stair("s1u", "landing", (-1.1, -1.5, -0.2, 2.0), G, F, "S", kind="void")
    h.stair("s2", "kitchen", (-4.6, -4.1, -3.7, -0.6), C, G, "N", kind="cellar stair behind a ledged door")
    h.stair("s2c", "cellar", (-4.6, -4.1, -3.7, -0.6), C, G, "N", kind="arrives")
    h.chimney(-3.1, 0.0, w=1.0, d=0.8, serves="drawing-room fire = THE HEARTH, back-to-back with the kitchen range", pots=4)
    # drawing room
    hr = h.wall("sitting", "S", "Hearth", at=-3.1, role="hearth", note="white marble fire surround, brass fender, coal scuttle")
    h.on(hr, "MantelClock", note="stopped at 4:00"); h.on(hr, "FrameStanding1", dx=-0.7, role="family-photo", note="Dr Crane, wife, the boy (age 8)")
    h.on(hr, "VaseAntique", dx=0.7)
    item(h, hr, "drawing-room mantel")
    h.wall("sitting", "W", "Sofa", at=2.2)
    h.at("sitting", "ArmchairGreen", -2.4, 1.2, yaw=270)
    h.at("sitting", "CoffeeTable", -3.1, 2.2, yaw=90)
    h.at("sitting", "Rug", -3.4, 2.2)
    pt = h.wall("sitting", "N", "SideTable", at=-1.8)
    h.on(pt, "RotaryPhone", role="phone", note="cream bakelite phone, number card 'HOLLOWMERE 14'")
    h.on(pt, "Radio", dx=0.15, role="radio", note="walnut radio")
    h.wall("sitting", "E", "Radiator", at=0.9)
    h.hung("sitting", "W", "FrameFancy1", at=0.8, y=1.8, note="oil painting of the chapel")
    h.wall("sitting", "N", "TableSmall", at=-4.2, note="card table with a chess set")
    # kitchen
    rg = h.wall("kitchen", "N", "Range", at=-3.1, note="cream enamel range in the back-to-back flue")
    h.wall("kitchen", "S", "KitchenSinkUnit", at=-2.3)
    h.wall("kitchen", "E", "Fridge", at=-1.2)
    kt = h.at("kitchen", "TableWood1", -2.5, -2.2, yaw=90)
    h.on(kt, "TeaSet"); note(h, kt, "housekeeper's note about the boy's medicine (note 1/3)")
    h.at("kitchen", "ChairDining", -3.15, -2.2, yaw=90)
    # hall
    hc = h.wall("hall", "E", "HallConsole", at=0.8)
    item(h, hc, "hall console")
    h.hung("hall", "E", "Mirror", at=0.8, y=1.7, note="hall mirror")
    h.hung("hall", "S", "FuseBox", at=-0.6, y=1.7, role="fuse-box", note="FUSE BOX + a 1930s meter")
    h.wall("hall", "E", "CoatHooks", at=-1.6)
    # waiting room
    h.wall("waiting", "N", "BenchPainted", at=3.6, note="waiting bench under the window (low)")
    h.wall("waiting", "S", "BenchPainted", at=3.1)
    h.wall("waiting", "W", "TableSmall", at=1.0, note="magazines from 1974")
    h.hung("waiting", "W", "WallClock", at=3.6, y=2.1)
    h.wall("waiting", "E", "PottedPlant2", at=2.4)
    # dispensary PUZZLE
    mc = h.hung("disp", "N", "MedicineCabinet", at=3.4, y=1.5, role="puzzle container",
                note="GLASS-FRONTED MEDICINE CABINET with a 4-dial padlock - the PART CONTAINER")
    puzzle(h, mc, "CodeLockPuzzle: match the 12 medicine bottles to the 4 patient record cards; each card's bottle number gives one digit")
    container(h, mc, "Glass eye in a velvet box on the middle shelf")
    ct = h.wall("disp", "S", "Counter", at=3.6, scale=0.6, note="dispensing bench 1.44 m")
    h.on(ct, "MedicineBottles", note="12 bottles, labels data-driven")
    h.on(ct, "BunsenBurner", dx=0.5)
    h.wall("disp", "W", "DrawerCabinet", at=-1.6, note="PATIENT RECORDS index drawers (4 cards = clues)")
    note(h, h.props[-1], "patient record cards (note 2/3)", dx=0)
    h.wall("disp", "N", "SteelShelves2", at=1.8, note="shelves of brown bottles and pill boxes")
    item(h, ct, "dispensing bench", dx=-0.5)
    # consulting room
    h.wall("consult", "E", "ExamCouch", at=0.0)
    dk = h.wall("consult", "S", "OfficeDesk", at=8.5)
    h.on(dk, "Microscope", dx=0.4); h.on(dk, "Typewriter", dx=-0.3)
    note(h, dk, "Dr Crane's case notes on 'the boy' (note 3/3)")
    h.at("consult", "Chair", 8.5, -1.5, yaw=180)
    h.wall("consult", "S", "CabinetVintage", at=6.0, note="specimen cabinet: jars, a glass eye chart")
    h.wall("consult", "N", "WashStand", at=9.2)
    h.wall("consult", "W", "Crutches", at=2.4)
    # cellar
    h.wall("cellar", "E", "SteelShelves", at=2.0, note="wine racks + preserving jars")
    h.wall("cellar", "N", "CellarCrates", at=-2.0); hide(h, h.props[-1], "behind the crates (cellar)", crouch=True)
    h.wall("cellar", "S", "BarrelWorn", at=-0.5)
    h.wall("cellar", "E", "CardboardBox", at=-2.5, note="boxes of old surgery files")
    # bed1
    bed(h, "bed1", "W", "Bed", 2.2, label="doctor's bed")
    w = h.wall("bed1", "S", "Wardrobe", at=-2.0); hide(h, w, "wardrobe")
    h.wall("bed1", "N", "Commode", at=-1.9)
    # bed2 guest
    bed(h, "bed2", "E", "BedIron", 2.2, stands=("Nightstand",), stand_side="left", label="guest bed", hiding=False)
    h.wall("bed2", "S", "ChestOfDrawers", at=3.6)
    # bath
    bt = h.wall("bath", "W", "BathtubPhoto", at=-2.4); hide(h, bt, "behind the bath curtain")
    h.at("bath", "BathCurtainRail", -4.0, -2.4, yaw=90, role="allow-overlap")
    h.wall("bath", "N", "Sink", at=-2.6)
    h.hung("bath", "N", "Mirror", at=-2.6, y=1.55, role="mirror")
    h.wall("bath", "S", "Toilet", at=-3.2)
    h.wall("bath", "E", "Radiator", at=-1.0)
    # bed3 the boy's
    bed(h, "bed3", "E", "BedIron", -2.2, stands=("NightstandPainted",), stand_side="right", label="the boy's bed", hiding=False)
    h.at("bed3", "RockingHorse", 2.4, -1.0, yaw=90)
    h.wall("bed3", "W", "Globe", at=-1.0)
    h.wall("bed3", "S", "Wicker2", at=1.8, note="toy basket")
    shrine(h, "landing", "E", -0.8)
    return h


# ============================================================================================ H06 Twins' House
def h06():
    h = House(6, "H06", "Twins' House", "Abney", "Silver locket", "Sequence", "Yellow", 2, 2)
    h.traits = dict(archetype="Two identical Victorian brick cottages knocked into one, perfectly mirrored",
                    silhouette="Wide and symmetrical: TWO front doors side by side (both yellow; the west one is screwed shut), two "
                               "identical gable chimneys, twin bay windows, twin dormers. Everything on the west is the mirror "
                               "image of the east... almost.")
    levels(h, attic=True, g=2.7, f=2.5)
    h.wing("main", (-7.0, -4.0, 7.0, 4.0), ["G", "F", "A"], "BrickPaintedWorn (cream limewash over red brick, worn through on "
           "the lower 1 m) with BrickRed window arches", T_BRICK,
           "Gable roof 38 deg, ridge along x at 7.9 m, RoofClay3; two front dormers at x=+-4.2 with RoofClay3 cheeks; "
           "gable chimneys at x=+-7.0")
    G, F, A = "G", "F", "A"
    h.room("hall", "Double hall (knocked through)", G, (-2.0, -4.0, 2.0, 4.0), "hall", "FloorWornTile", "Wallpaper2 (west) / Wallpaper2 (east)")
    h.room("parW", "West parlour (Ada's)", G, (-7.0, 0.0, -2.0, 4.0), "living", "Carpet70s", "WallpaperJacquard")
    h.room("parE", "East parlour (Ida's)", G, (2.0, 0.0, 7.0, 4.0), "living", "Carpet70s", "WallpaperJacquard")
    h.room("kitW", "West kitchen (cold)", G, (-7.0, -4.0, -2.0, 0.0), "kitchen", "FloorQuarryTile", "Whitewash")
    h.room("kitE", "East kitchen (lived in)", G, (2.0, -4.0, 7.0, 0.0), "kitchen", "FloorQuarryTile", "Whitewash")
    h.room("landing", "Double landing", F, (-2.0, -4.0, 2.0, 1.5), "landing", "FloorOldWood1", "Wallpaper2")
    h.room("bath", "Bathroom (between the halves)", F, (-2.0, 1.5, 2.0, 4.0), "bathroom", "FloorBathTile", "TileWallWhite")
    h.room("bedW", "West bedroom (Ada)", F, (-7.0, 0.0, -2.0, 4.0), "bedroom", "Carpet70sB", "Wallpaper1")
    h.room("bedE", "East bedroom (Ida)", F, (2.0, 0.0, 7.0, 4.0), "bedroom", "Carpet70sB", "Wallpaper1")
    h.room("nurW", "West back room (the cots)", F, (-7.0, -4.0, -2.0, 0.0), "bedroom", "FloorOldWood2", "Wallpaper1B")
    h.room("nurE", "East back room (the cots)", F, (2.0, -4.0, 7.0, 0.0), "bedroom", "FloorOldWood2", "Wallpaper1B")
    h.room("attic", "Long attic (PUZZLE end)", A, (-7.0, -4.0, 7.0, 4.0), "attic", "FloorOldWood2", "RawPlankWall")
    h.door("d_frontE", G, "hall", "N", 1.0, 0.9, role="front", kind="yellow 4-panel door, brass '7B'", colour="Yellow")
    h.door("d_frontW", G, "hall", "N", -1.0, 0.9, room_b="ext", role="sealed", kind="yellow 4-panel door '7A', screwed shut from inside (cannot open)", colour="Yellow")
    h.door("d_back", G, "hall", "S", 0.0, 0.9, role="back")
    for side, rp, rk, sx in (("W", "parW", "kitW", -1), ("E", "parE", "kitE", 1)):
        h.door(f"d_{rp}", G, "hall", side, 2.4, 0.85, room_b=rp)
        h.door(f"d_{rk}", G, "hall", side, -2.6, 0.85, room_b=rk)
    h.door("d_bedW", F, "landing", "W", 0.75, 0.8, room_b="bedW")
    h.door("d_bedE", F, "landing", "E", 0.75, 0.8, room_b="bedE")
    h.door("d_nurW", F, "landing", "W", -3.3, 0.8, room_b="nurW")
    h.door("d_nurE", F, "landing", "E", -3.3, 0.8, room_b="nurE")
    h.door("d_bath", F, "landing", "N", 0.0, 0.8, room_b="bath")
    for s, x in (("W", -4.5), ("E", 4.5)):
        h.window(f"w_bay{s}", "par" + s, "N", x, 1.8, 1.5, sill=0.7, style="twin canted bay (shown flat)", curtains="FabricTerlenka")
        h.window(f"w_bed{s}", "bed" + s, "N", x, 1.1, 1.2, style="dormer sash")
        h.window(f"w_nur{s}", "nur" + s, "S", x, 1.0, 1.1, style="sash")
    h.window("w_kW", "kitW", "S", -4.5, 1.0, 1.0, weak=True, style="sash (west)")
    h.window("w_kE", "kitE", "S", 4.5, 1.0, 1.0, weak=True, style="sash (east)")
    h.window("w_attic", "attic", "W", 0.0, 0.6, 0.6, sill=1.0, style="gable light (only on the WEST gable - the east one is bricked up: a difference)")
    for s, x0 in (("W", -1.9), ("E", 1.0)):
        h.stair(f"s{s}", "hall", (x0, -3.6, x0 + 0.9, -0.1), G, F, "N", note=f"{s} stair, mirror image of the other, 13 risers")
        h.stair(f"s{s}u", "landing", (x0, -3.6, x0 + 0.9, -0.1), G, F, "N", kind="void")
    h.stair("s3", "landing", (-0.45, -1.4, 0.45, -0.4), F, A, "S", kind="loft ladder between the two stair wells")
    h.chimney(-7.0, -1.5, serves="west kitchen range (cold, identical decoy hearth)", pots=2)
    h.chimney(7.0, -1.5, serves="east kitchen range = THE HEARTH", pots=2)
    # mirrored furnishing helper
    def mir(room_base, side_w, key, at, **kw):
        out = []
        for s in ("W", "E"):
            side = side_w if side_w in "NS" else ({"W": "E", "E": "W"}[side_w] if s == "E" else side_w)
            a = at if side_w in "EW" else (at if s == "W" else -at)
            out.append(h.wall(room_base + s, side, key, at=a, **kw))
        return out
    hw, he = mir("kit", "W", "Hearth", -1.5, role="hearth", note="cast-iron range; WEST is cold & swept, EAST is THE HEARTH")
    item(h, he, "east range mantel")
    mir("kit", "N", "Dresser", -4.0)
    mir("kit", "S", "KitchenSinkUnit", -5.6)
    tw, te = (h.at("kitW", "TableWood1", -4.0, -2.0, yaw=0), h.at("kitE", "TableWood1", 4.0, -2.0, yaw=0))
    note(h, te, "Ida's note: 'Ada moved the clock again' (note 1/3)")
    mir("par", "W", "Sofa3", 2.0)
    pw, pe = mir("par", "S", "SideTable", -3.0)
    h.on(pe, "RotaryPhone", role="phone", note="phone ONLY on the east side (west has the matching table, empty)")
    h.on(pw, "Vase2")
    tvw, tve = mir("par", "S", "Cabinet70s", -5.6)
    h.on(tvw, "TV2", role="tv"); h.on(tve, "TV2", role="tv")
    h.hung("parW", "E", "FrameHanging1", at=3.2, y=1.6, role="family-photo", note="the twins aged 6 (west)")
    h.hung("parE", "W", "FrameHanging1", at=3.2, y=1.6, role="family-photo", note="the twins aged 6 (east) - identical except one girl is missing")
    h.hung("parW", "E", "WallClock", at=1.0, y=2.1, note="clock (west) - DIFFERENCE candidate: hands at 2:00 vs 10:00 mirrored")
    h.hung("parE", "W", "WallClock", at=1.0, y=2.1, note="clock (east)")
    h.hung("hall", "S", "FuseBox", at=-1.2, y=1.7, role="fuse-box")
    h.hung("hall", "N", "Mirror", at=0.0, y=1.8, note="mirror between the two front doors")
    # bedrooms (mirrored)
    bw, *_ = bed(h, "bedW", "W", "Bed", 2.2, label="Ada's bed", hiding=True)
    be, *_ = bed(h, "bedE", "E", "Bed", 2.2, label="Ida's bed", hiding=False)
    ww, we = mir("bed", "S", "Wardrobe", -4.6)
    hide(h, we, "Ida's wardrobe")
    mir("nur", "S", "BabyCrib", -5.6)
    cw, ce = mir("nur", "N", "ChestOfDrawers", -5.6)
    note(h, cw, "midwife's letter, 1951 (note 2/3)")
    # bath
    bt = h.wall("bath", "N", "Bathtub", at=-0.6, scale=0.85); hide(h, bt, "behind the bath curtain")
    h.wall("bath", "E", "Toilet", at=3.0)
    h.wall("bath", "S", "Sink", at=1.4)
    h.hung("bath", "S", "Mirror", at=1.4, y=1.55, role="mirror", note="the mirror that shows the 'other' half of the house")
    # attic PUZZLE
    box = h.wall("attic", "E", "ChestWorn", at=0.0, role="puzzle container",
                 note="THE LOCKET BOX: a double music box with two keyholes; Sequence panel = 6 brass tabs on its lid")
    puzzle(h, box, "Spot the differences: 3-5 objects differ between the halves (picked per match from DIFFERENCE candidates). "
                   "Each difference shows a symbol when found; press the symbols on the box in the order the cot-room "
                   "rhyme gives. SequencePuzzle (6 symbols, 4 in the answer).")
    container(h, box, "Silver locket")
    h.wall("attic", "W", "Suitcase", at=0.0)
    h.wall("attic", "N", "CellarCrates", at=-4.0); hide(h, h.props[-1], "behind the trunks (attic, west end)", crouch=True)
    h.wall("attic", "S", "RockingHorse", at=4.0)
    note(h, h.wall("attic", "S", "TableSmall", at=-3.0), "the cot-room rhyme (note 3/3)")
    item(h, h.wall("attic", "N", "CardboardBox", at=2.5), "box of matching dresses")
    item(h, tw, "west kitchen table (the 'wrong' one)")
    shrine(h, "landing", "S", 1.0)
    return h


# ============================================================================================ H07 Organist's House
def h07():
    h = House(7, "H07", "Organist's House", "Venn", "Organ pipe", "Sequence", "Red", 3, 7, loud=True)
    h.traits = dict(archetype="Tall gaunt Gothic stone house beside the chapel, with an organ bay",
                    silhouette="Narrow, tall and dark: steep 55 deg red-slate roof, THREE chimneys (both gables and the rear wing), "
                               "lancet windows, a stone ORGAN BAY with a pointed window projecting toward the chapel, a red door "
                               "under a hood mould, ivy over half the front.")
    levels(h, cellar=True, g=3.2, f=2.8)
    h.wing("main", (-4.5, -4.0, 4.5, 4.0), ["C", "G", "F"], "StoneBlock (dark grey, ivy on the west half), SandstoneBlocks "
           "hood moulds and lancet surrounds, BrickDark plinth", T_STONE,
           "Steep gable roof 55 deg, ridge along x at 11.2 m, RoofRedSlate, cast-iron cresting on the ridge; gable chimneys W and E")
    h.wing("rear", (-4.5, -9.0, 0.5, -4.0), ["G", "F"], "StoneBlock", T_STONE, "Gable roof 50 deg, ridge along z, RoofRedSlate; "
           "chimney on the south gable")
    h.wing("bay", (4.5, -1.6, 6.2, 1.6), ["G"], "StoneBlock with a tall pointed window 1.4 x 3.0", T_STONE, "Stone-slate lean-to")
    G, F, C = "G", "F", "C"
    h.room("hall", "Hall", G, (-1.2, -4.0, 1.0, 4.0), "hall", "FloorFlagstone", "PanelDark full height")
    h.room("parlour", "Parlour", G, (-4.5, 0.0, -1.2, 4.0), "living", "FloorDarkPlanks", "Wallpaper2 (oxblood)")
    h.room("study", "Copying room", G, (-4.5, -4.0, -1.2, 0.0), "study", "FloorDarkPlanks", "PanelDark")
    h.room("music", "Music room (PUZZLE)", G, (1.0, -4.0, 4.5, 4.0), "music", "FloorParquet", "PanelDark to 1.5 m, Wallpaper2C", light="chandelier (Chandelier3)")
    h.room("bay", "Organ bay", G, (4.5, -1.6, 6.2, 1.6), "music-bay", "FloorParquet", "StoneBlock (exposed)")
    h.room("kitchen", "Kitchen (rear wing)", G, (-4.5, -9.0, 0.5, -4.0), "kitchen", "FloorFlagstone", "Whitewash")
    h.room("bed1", "Mr Venn's bedroom", F, (1.0, -4.0, 4.5, 4.0), "bedroom", "FloorDarkPlanks", "Wallpaper1")
    h.room("landing", "Landing", F, (-1.2, -4.0, 1.0, 4.0), "landing", "FloorDarkPlanks", "PanelDark")
    h.room("bed2", "Mother's room (kept as it was)", F, (-4.5, 0.0, -1.2, 4.0), "bedroom", "CarpetWorn", "Wallpaper1B")
    h.room("bath", "Bathroom", F, (-4.5, -4.0, -1.2, 0.0), "bathroom", "FloorBathTile", "TileWallWhite (black border)")
    h.room("store", "Music store (rear wing)", F, (-4.5, -9.0, 0.5, -4.0), "storage", "FloorOldWood2", "RawPlankWall")
    h.room("cellar", "Cellar (bellows chamber)", C, (-4.5, -4.0, 4.5, 4.0), "cellar", "FloorFlagstone", "Whitewash",
           note="the organ's old hand-pumped bellows reservoir sits under the music room")
    h.door("d_front", G, "hall", "N", 0.0, 1.0, height=2.4, role="front", kind="pointed-arch oak door painted red, iron ring handle", colour="Red")
    h.door("d_back", G, "kitchen", "W", -6.5, 0.85, role="back")
    h.door("d_par", G, "hall", "W", 3.0, 0.85, room_b="parlour")
    h.door("d_study", G, "hall", "W", -3.0, 0.85, room_b="study")
    h.door("d_music", G, "hall", "E", 2.6, 1.1, room_b="music", kind="double doors")
    h.door("d_kit", G, "study", "S", -2.8, 0.85, room_b="kitchen")
    h.door("d_bay", G, "music", "E", 0.0, 2.4, height=2.8, room_b="bay", kind="open pointed arch (no leaf)")
    h.door("d_b1", F, "landing", "E", 2.6, 0.85, room_b="bed1")
    h.door("d_b2", F, "landing", "W", 2.6, 0.85, room_b="bed2")
    h.door("d_bath", F, "landing", "W", -3.0, 0.8, room_b="bath")
    h.door("d_store", F, "bath", "S", -1.8, 0.8, room_b="store", kind="low door (the store is reached through the bathroom - Mr Venn's odd extension)")
    h.window("w_bay", "bay", "E", 0.0, 1.4, 3.0, sill=0.5, style="tall pointed window, plain leaded glass, faces the chapel")
    h.window("w_par", "parlour", "N", -2.9, 0.9, 2.0, style="lancet pair")
    h.window("w_music", "music", "N", 2.8, 0.9, 2.0, style="lancet pair")
    h.window("w_k", "kitchen", "E", -6.5, 1.0, 1.2, weak=True, style="sash, rotten")
    h.window("w_study", "study", "W", -2.0, 0.8, 1.6, weak=True, style="lancet, cracked")
    h.window("w_b1", "bed1", "N", 2.8, 0.8, 1.6, style="lancet")
    h.window("w_b2", "bed2", "N", -2.9, 0.8, 1.6, style="lancet")
    h.window("w_store", "store", "S", -2.0, 0.6, 0.8, style="small")
    h.stair("s1", "hall", (0.05, -3.7, 0.95, -0.2), G, F, "N", note="stone stair, 17 risers, iron balusters")
    h.stair("s1u", "landing", (0.05, -3.7, 0.95, -0.2), G, F, "N", kind="void")
    h.stair("s2", "hall", (-1.15, -3.7, -0.3, -1.2), C, G, "S", kind="cellar steps under a trapdoor in the hall flags (trapdoor stays open)")
    h.stair("s2c", "cellar", (-1.15, -3.7, -0.3, -1.2), C, G, "S", kind="arrives")
    h.chimney(-4.5, 2.0, serves="parlour fire", pots=2)
    h.chimney(4.5, -3.0, serves="music-room fire (cold)", pots=2)
    h.chimney(-2.0, -9.0, serves="kitchen range = THE HEARTH", pots=1)
    # music room PUZZLE
    org = h.at("bay", "PumpOrgan", 5.25, 0.0, yaw=270, role="puzzle", note="THE PUMP ORGAN in the bay, keyboard facing west into the room")
    puzzle(h, org, "SequencePuzzle reskinned: 6 marked keys (symbols painted on the key fronts); play the village lullaby "
                   "(children hum it by day; notation on the hymn sheet in the copying room). EVERY KEY PRESS = NoiseRadius.LoudPuzzle")
    h.at("bay", "StoolPainted", 4.75, 0.0, yaw=90, note="organ stool")
    ch = h.wall("music", "S", "ChestWorn", at=2.8, note="PART CONTAINER: pipe chest (long box 0.9)")
    container(h, ch, "Organ pipe")
    h.wall("music", "W", "Shelf", at=-2.6, note="sheet music to the ceiling")
    h.wall("music", "W", "Shelf", at=-1.4)
    h.at("music", "ArmchairGreen", 2.6, 1.2, yaw=135)
    h.at("music", "Rug", 2.8, 0.0)
    h.hung("music", "W", "FrameFancy2", at=0.5, y=1.9, role="family-photo", note="Venn with the chapel choir, 1958")
    mt = h.at("music", "TableRound2", 2.2, -1.6)
    h.on(mt, "Candleholders"); item(h, mt, "music-room table")
    # parlour
    hr = h.wall("parlour", "W", "Hearth", at=2.0, role="hearth", note="cast-iron fireplace (decorative); THE HEARTH is the kitchen range")
    h.on(hr, "MantelClock", note="stopped at 7:00")
    item(h, hr, "parlour mantel")
    h.wall("parlour", "S", "SofaPainted", at=-2.9)
    pt = h.wall("parlour", "N", "SideTable", at=-2.6)
    h.on(pt, "RotaryPhone", role="phone"); h.on(pt, "CassettePlayer", dx=0.15, role="radio", note="cassette recorder: tapes of children singing")
    h.at("parlour", "RockingChair", -2.4, 1.6, yaw=270)
    # copying room
    dk = h.wall("study", "W", "OfficeDesk", at=-2.0, scale=0.85)
    note(h, dk, "HYMN SHEET with the lullaby in shape-note symbols (note 1/3)")
    h.wall("study", "N", "Shelf", at=-3.4)
    h.at("study", "Chair", -3.0, -2.0, yaw=270)
    # kitchen
    rg = h.wall("kitchen", "S", "Hearth", at=-2.0, role="hearth", note="black range in the south gable = THE HEARTH")
    item(h, rg, "range shelf")
    h.wall("kitchen", "N", "Dresser", at=-0.8)
    kt = h.at("kitchen", "TableWood2", -2.0, -6.4, yaw=90)
    note(h, kt, "letter from the vicar about 'the tune' (note 2/3)")
    h.wall("kitchen", "W", "KitchenSinkUnit", at=-5.0)
    h.hung("kitchen", "N", "FuseBox", at=0.0, y=1.7, role="fuse-box")
    # bed1
    bed(h, "bed1", "E", "Bed", 1.8, label="bed")
    w = h.wall("bed1", "S", "Wardrobe", at=2.8); hide(h, w, "wardrobe")
    # bed2
    bed(h, "bed2", "W", "DayBed", 1.4, stands=("NightstandPainted",), stand_side="left", label="day-bed", hiding=False)
    cm = h.wall("bed2", "S", "Commode", at=-2.9, note="dressing table with her hairbrushes still on it")
    note(h, cm, "Mrs Venn's lullaby verses (note 3/3)")
    # bath
    bt = h.wall("bath", "N", "BathtubPhoto", at=-2.9); hide(h, bt, "behind the bath curtain")
    h.wall("bath", "W", "Sink", at=-2.2)
    h.hung("bath", "W", "Mirror", at=-2.2, y=1.55, role="mirror")
    h.wall("bath", "W", "Toilet", at=-3.4)
    # store
    h.wall("store", "E", "SteelShelves", at=-6.5, note="organ pipes of every length, labelled")
    h.wall("store", "W", "CellarCrates", at=-6.5); hide(h, h.props[-1], "behind the pipe crates", crouch=True)
    # cellar
    h.wall("cellar", "E", "Barrel2", at=-2.0, note="bellows reservoir (leather, cracked) - a big shape")
    h.wall("cellar", "N", "BarrelsWood", at=2.0)
    h.wall("cellar", "W", "SteelShelves2", at=2.0)
    shrine(h, "landing", "N", 0.6)
    return h


# ============================================================================================ H08 Butcher's House
def h08():
    h = House(8, "H08", "Butcher's House", "Grice", "Bone whistle", "CodeLock", "Red", 2, 5)
    h.traits = dict(archetype="Main Street butcher's shop with the family living above and a cold room behind",
                    silhouette="Shop front of green-and-cream glazed tiles with a big open serving window under a torn striped "
                               "canvas awning, 'J. GRICE - FAMILY BUTCHER' fascia, a red side door to the flat, brick upper "
                               "floor with two sashes, party-wall chimneys both sides, a long single-storey cold room behind.")
    levels(h, cellar=True, g=3.0, f=2.7)
    h.wing("main", (-4.0, -6.0, 4.0, 4.0), ["C", "G", "F"], "Shop front: green/cream tiles to 1.0 m then BrickRed; sides BrickMixed",
           T_BRICK, "Gable roof, ridge along x at 8.2 m, RoofClay2; chimneys on both side walls")
    h.wing("cold", (-4.0, -11.0, 0.5, -6.0), ["G"], "Whitewashed brick (Whitewash on BrickPaintedWorn), louvred vent", 0.3,
           "Mono-pitch CorrugatedRust roof")
    G, F, C = "G", "F", "C"
    h.room("shop", "Butcher's shop", G, (-4.0, 0.0, 4.0, 4.0), "shop", "FloorCheckTile", "TileWallWhite to ceiling (blood-spattered grout)", light="2 caged lights")
    h.room("hall", "Side passage & stair", G, (1.8, -6.0, 4.0, 0.0), "hall", "FloorWornTile", "PlasterWorn")
    h.room("kitchen", "Back kitchen", G, (-4.0, -6.0, 1.8, 0.0), "kitchen", "FloorQuarryTile", "Whitewash")
    h.room("cold", "Cold room (PUZZLE)", G, (-4.0, -11.0, 0.5, -6.0), "coldroom", "FloorBrick", "TileWallWhite, drain in the floor", light="one caged bulb")
    h.room("living", "Front room (upstairs)", F, (-4.0, 0.0, 4.0, 4.0), "living", "Carpet70s", "WallpaperQuatrefoil")
    h.room("landing", "Landing", F, (1.8, -6.0, 4.0, 0.0), "landing", "Carpet70sB", "Wallpaper2C")
    h.room("bed1", "Grices' bedroom", F, (-4.0, -2.5, 1.8, 0.0), "bedroom", "CarpetWorn", "Wallpaper1")
    h.room("pass", "Back passage", F, (-4.0, -3.4, 1.8, -2.5), "passage", "Carpet70sB", "Wallpaper2C")
    h.room("bed2", "Son's bedroom", F, (-4.0, -6.0, -0.6, -3.4), "bedroom", "FloorOldWood1", "Wallpaper1B (football wallpaper border)")
    h.room("bath", "Bathroom", F, (-0.6, -6.0, 1.8, -3.4), "bathroom", "FloorBathTile", "TileWallWhite")
    h.room("cellar", "Salting cellar", C, (-4.0, 0.0, 4.0, 4.0), "cellar", "FloorBrick", "Whitewash", note="salt troughs, hooks")
    h.door("d_shop", G, "shop", "N", -3.0, 1.0, role="shop", kind="shop door with a bell, 'CLOSED' sign")
    h.door("d_front", G, "hall", "E", -3.0, 0.9, role="front", kind="red side door to the flat, letterbox", colour="Red")
    h.door("d_back", G, "hall", "S", 2.9, 0.85, role="back", note="to the slaughter yard")
    h.door("d_shopk", G, "shop", "S", -1.2, 0.9, room_b="kitchen", kind="half-glazed door with a meat-hook rail over it")
    h.door("d_hallk", G, "hall", "W", -4.4, 0.85, room_b="kitchen")
    h.door("d_cold", G, "kitchen", "S", -1.8, 1.0, room_b="cold", kind="insulated cold-room door with a lever latch (heavy, loud)")
    h.door("d_liv", F, "landing", "N", 2.9, 0.85, room_b="living")
    h.door("d_b1", F, "landing", "W", -1.2, 0.8, room_b="bed1")
    h.door("d_pass", F, "landing", "W", -2.95, 0.8, room_b="pass")
    h.door("d_b2", F, "pass", "S", -1.2, 0.8, room_b="bed2")
    h.door("d_bath", F, "pass", "S", 0.4, 0.75, room_b="bath")
    h.window("w_shop", "shop", "N", 1.0, 3.6, 1.8, sill=0.9, style="open serving window with a marble slab sill and a sliding sash")
    h.window("w_k", "kitchen", "W", -3.0, 1.0, 1.1, weak=True, style="side window onto the alley, cracked")
    h.window("w_cold", "cold", "S", -1.75, 0.6, 0.4, sill=1.8, weak=True, style="louvred vent (pull-out)")
    h.window("w_l1", "living", "N", -2.0, 1.2, 1.5, style="sash", curtains="FabricVelvet")
    h.window("w_l2", "living", "N", 2.0, 1.2, 1.5, style="sash", curtains="FabricVelvet")
    h.window("w_b2", "bed2", "W", -4.8, 0.9, 1.1, style="sash")
    h.window("w_bath", "bath", "S", 0.4, 0.7, 0.8, sill=1.3, style="frosted")
    h.window("w_land", "landing", "E", -2.0, 0.7, 1.1, style="stair window")
    h.stair("s1", "hall", (2.6, -5.0, 3.5, -1.5), G, F, "S", note="steep straight stair, 15 risers, lino treads")
    h.stair("s1u", "landing", (2.6, -5.0, 3.5, -1.5), G, F, "S", kind="void")
    h.stair("s2", "shop", (2.6, 0.4, 3.5, 3.5), C, G, "N", kind="trapdoor + steps behind the counter to the salting cellar")
    h.stair("s2c", "cellar", (2.6, 0.4, 3.5, 3.5), C, G, "N", kind="arrives")
    h.chimney(-4.0, -3.0, serves="kitchen range = THE HEARTH", pots=2)
    h.chimney(4.0, 2.0, serves="upstairs front-room fire", pots=2)
    # shop
    ct = h.wall("shop", "N", "Counter", at=1.0, depth_off=0.4, note="marble-topped serving counter behind the window, 2.4 m")
    h.on(ct, "Cleaver", dx=0.6); h.on(ct, "CuttingBoard", dx=-0.4)
    note(h, ct, "price board (note 1/3)")
    h.at("shop", "MeatRail", 0.0, 2.2, yaw=0, y=2.2, note="ceiling rail with 8 hooks (2 cloth-wrapped carcass shapes)")
    blk = h.at("shop", "ButcherBlock", -0.6, 1.2)
    h.on(blk, "Cleaver")
    item(h, blk, "butcher's block", dx=0.2)
    h.wall("shop", "W", "Fridge", at=2.6, note="display fridge (humming)")
    h.wall("shop", "S", "DrawerCabinet", at=0.8, note="till cabinet")
    item(h, h.props[-1], "till cabinet")
    h.hung("shop", "S", "FrameHanging3", at=-3.0, y=1.8, role="family-photo", note="Grice & son outside the shop, 1969 - the son is holding the whistle")
    # kitchen
    hr = h.wall("kitchen", "W", "Hearth", at=-4.4, role="hearth", note="range in the west party-wall flue")
    h.on(hr, "MantelClock", note="stopped at 5:00")
    item(h, hr, "kitchen mantel")
    kt = h.at("kitchen", "TableWood1", -1.2, -3.0, yaw=0)
    h.on(kt, "PotEnamel")
    h.at("kitchen", "ChairDining", -1.2, -2.2, yaw=180)
    h.wall("kitchen", "N", "KitchenSinkUnit", at=-3.0)
    h.wall("kitchen", "E", "Dresser", at=-2.0)
    # hall
    h.hung("hall", "W", "FuseBox", at=-0.6, y=1.7, role="fuse-box")
    h.hung("hall", "W", "Mirror", at=-2.4, y=1.6, note="small mirror by the side door")
    # cold room PUZZLE
    sc = h.wall("cold", "E", "BalanceScale", at=-8.5, role="puzzle", note="BEAM SCALE on a cast pillar; brass weights 1/2/5/10/20 lb on the shelf")
    puzzle(h, sc, "CodeLockPuzzle reskinned: the LEDGER (upstairs) gives a total; find the cuts (hanging, each with a weight tag) "
                  "and weights that balance it; the 4-digit combination = the tag numbers of the chosen cuts")
    bx = h.wall("cold", "S", "ToolChest", at=-2.4, note="PART CONTAINER: steel cash box bolted to the bench")
    container(h, bx, "Bone whistle")
    h.at("cold", "MeatRail", -1.75, -8.5, yaw=90, y=2.0, note="hanging cuts with tags (puzzle clues)")
    h.wall("cold", "W", "SteelShelves2", at=-8.5)
    h.wall("cold", "W", "BarrelWorn", at=-10.2); hide(h, h.props[-1], "behind the brine barrel", crouch=True)
    # upstairs living
    hr2 = h.wall("living", "E", "Hearth", at=2.0, role="hearth-decor", note="tiled fireplace (decorative; not a protective hearth)")
    h.wall("living", "S", "Sofa2", at=-1.0)
    h.at("living", "Armchair", 1.0, 1.8, yaw=90)
    tv = h.wall("living", "W", "Cabinet70s", at=2.0)
    h.on(tv, "TV", role="tv")
    pt = h.wall("living", "S", "SideTable", at=-3.4)
    h.on(pt, "RotaryPhone", role="phone")
    h.at("living", "CoffeeTable", -1.0, 1.6)
    ld = h.wall("living", "N", "TableSmall", at=0.0, note="writing table")
    note(h, ld, "THE LEDGER: last entry 'weigh it all to 37 lb' (note 2/3)")
    # bed1
    bed(h, "bed1", "W", "Bed", -1.25, stands=("",), label="double bed")
    h.wall("bed1", "S", "Commode", at=-0.4)
    # bed2
    bed(h, "bed2", "W", "BedIron", -5.0, stands=("",), label="son's bed", hiding=False)
    w2 = h.wall("bed2", "E", "Wardrobe", at=-5.0); hide(h, w2, "son's wardrobe")
    note(h, h.wall("bed2", "W", "TableSmall", at=-3.9), "the son's diary (note 3/3)")
    # bath
    h.wall("bath", "E", "Bathtub", at=-4.8, scale=0.85); hide(h, h.props[-1], "behind the bath curtain")
    h.wall("bath", "W", "Toilet", at=-5.4)
    h.wall("bath", "N", "Sink", at=1.2)
    h.hung("bath", "N", "Mirror", at=1.2, y=1.55, role="mirror")
    # cellar
    h.wall("cellar", "W", "SteelShelves", at=2.0, note="salt sacks")
    h.wall("cellar", "N", "BarrelsWood", at=-1.0)
    h.wall("cellar", "S", "Sacks", at=-2.0)
    shrine(h, "landing", "E", -0.6)
    return h


# ============================================================================================ H09 Miller's House
def h09():
    h = House(9, "H09", "Miller's House", "Thorne", "Last-harvest sheaf", "Clock", "Yellow", 1, 10)
    h.traits = dict(archetype="Miller's house joined to a three-storey watermill with a big undershot wheel on the leat",
                    silhouette="Two buildings in one: a small rendered house with a yellow door, and a tall mill (stone base, "
                               "black weatherboarded upper floors, a lucam/hoist hood projecting from the top floor) with a 4 m "
                               "waterwheel on its EAST wall over the mill race. One chimney, on the house's west gable.")
    levels(h, attic=True, g=2.7, f=2.5)
    h.wing("house", (-5.0, -4.0, 3.0, 4.0), ["G", "F"], "RenderMossy (cream render, green damp at the base)", T_RENDER,
           "Gable roof 40 deg, ridge along x at 7.3 m, RoofClay2; chimney on the west gable")
    h.wing("mill", (3.0, -5.0, 11.0, 5.0), ["G", "F", "A"], "Ground: BrickDark + StoneBlock (0.6 thick); upper: Weatherboard "
           "painted black, peeling", 0.45,
           "Gable roof 45 deg, ridge along x at 12.5 m, RoofSlateGrey; LUCAM (hoist hood) on the north side at x=7, beam + chain + pulley")
    G, F, A = "G", "F", "A"
    h.room("parlour", "Parlour", G, (-5.0, 0.0, -0.8, 4.0), "living", "FloorOldWood1", "Wallpaper2C")
    h.room("hall", "Hall", G, (-0.8, 0.0, 3.0, 4.0), "hall", "FloorWornTile", "PlasterWorn")
    h.room("kitchen", "Kitchen", G, (-5.0, -4.0, 3.0, 0.0), "kitchen", "FloorFlagstone", "Whitewash")
    h.room("gearfloor", "Mill: wheel & gear floor (PUZZLE)", G, (3.0, -5.0, 11.0, 5.0), "mill", "FloorDirt", "Whitewash flaking off stone",
           light="2 CagedLight on a separate mill circuit", note="pit wheel in a slot along the east wall; the main shaft rises through the floor above")
    h.room("bed1", "Thornes' bedroom", F, (-5.0, 0.0, -0.8, 4.0), "bedroom", "CarpetWorn", "Wallpaper1")
    h.room("landing", "Landing", F, (-0.8, 0.0, 3.0, 4.0), "landing", "FloorOldWood1", "PlasterWorn")
    h.room("bed2", "Spare bedroom", F, (-5.0, -4.0, 0.0, 0.0), "bedroom", "FloorOldWood2", "Wallpaper1B")
    h.room("bath", "Bathroom", F, (0.0, -4.0, 3.0, 0.0), "bathroom", "FloorBathTile", "TileWallWhite")
    h.room("stonefloor", "Mill: stone floor", F, (3.0, -5.0, 11.0, 5.0), "mill", "FloorOldWood2", "RawPlankWall + flour dust everywhere")
    h.room("binfloor", "Mill: bin floor (attic)", A, (3.0, -5.0, 11.0, 5.0), "attic", "FloorOldWood2", "RawPlankWall", light="none - flashlight only")
    h.door("d_front", G, "hall", "N", 0.5, 0.9, role="front", kind="yellow ledged-and-braced door with a horseshoe over it", colour="Yellow")
    h.door("d_back", G, "kitchen", "S", 1.4, 0.85, role="back", note="to the mill pond path")
    h.door("d_par", G, "hall", "W", 2.0, 0.85, room_b="parlour")
    h.door("d_kit", G, "hall", "S", 0.2, 0.85, room_b="kitchen")
    h.door("d_mill", G, "kitchen", "E", -2.0, 0.9, room_b="gearfloor", kind="iron-bound door into the mill (two steps down)")
    h.door("d_mill_ext", G, "gearfloor", "N", 7.0, 1.6, height=2.4, role="loading", kind="double cart doors (barred inside)")
    h.door("d_b1", F, "landing", "W", 2.0, 0.8, room_b="bed1")
    h.door("d_b2", F, "landing", "S", -0.4, 0.75, room_b="bed2")
    h.door("d_bath", F, "landing", "S", 1.0, 0.75, room_b="bath")
    h.door("d_hoist", A, "binfloor", "N", 7.0, 1.2, height=1.8, role="hoist", kind="lucam hoist door (opens onto a 9 m drop)")
    h.window("w_par", "parlour", "N", -2.9, 1.2, 1.3, style="sash", curtains="FabricGingham")
    h.window("w_k", "kitchen", "S", -2.5, 1.1, 1.1, weak=True, style="sash, cracked")
    h.window("w_gf", "gearfloor", "E", 2.5, 0.8, 0.8, sill=1.2, weak=True, style="small barred window beside the wheel (one bar rusted through)")
    h.window("w_gf2", "gearfloor", "S", 7.0, 0.8, 0.8, sill=1.2, style="small window")
    h.window("w_b1", "bed1", "N", -2.9, 1.0, 1.1, style="sash")
    h.window("w_b2", "bed2", "S", -2.5, 1.0, 1.1, style="sash")
    h.window("w_bath", "bath", "S", 1.5, 0.6, 0.7, sill=1.3, style="frosted")
    h.window("w_sf", "stonefloor", "E", -2.5, 0.8, 0.9, style="weatherboard-framed window over the wheel")
    h.window("w_bin", "binfloor", "E", 0.0, 0.6, 0.6, sill=1.0, style="gable window")
    h.stair("s1", "hall", (2.0, 0.3, 2.9, 3.6), G, F, "N", note="steep cottage stair")
    h.stair("s1u", "landing", (2.0, 0.3, 2.9, 3.6), G, F, "N", kind="void")
    h.stair("s2", "gearfloor", (3.5, -4.6, 4.3, -1.2), G, F, "S", kind="mill ladder-stair (open treads, handrail)")
    h.stair("s2u", "stonefloor", (3.5, -4.6, 4.3, -1.2), G, F, "S", kind="void")
    h.stair("s3", "stonefloor", (3.5, 1.2, 4.3, 4.6), F, A, "N", kind="mill ladder-stair to the bin floor")
    h.stair("s3u", "binfloor", (3.5, 1.2, 4.3, 4.6), F, A, "N", kind="void")
    h.chimney(-5.0, -2.0, serves="kitchen range = THE HEARTH", pots=1)
    # kitchen
    hr = h.wall("kitchen", "W", "Hearth", at=-1.9, role="hearth", note="black range with a bread oven; corn dollies hung on the beam")
    h.on(hr, "MantelClock", note="stopped at 10:00"); h.on(hr, "FrameStanding2", dx=0.6, role="family-photo", note="the Thornes at harvest, 1966")
    item(h, hr, "range mantel")
    kt = h.at("kitchen", "TableWood3", -1.5, -1.8, yaw=0)
    h.on(kt, "BowlWood2"); h.on(kt, "SweetPotato", dx=0.5)
    note(h, kt, "miller's account book: 'gears 5, 7, 9 - never the small one' (note 1/3)")
    h.at("kitchen", "BenchPainted", -1.5, -2.65, yaw=0)
    h.wall("kitchen", "N", "Dresser", at=-3.4)
    h.wall("kitchen", "S", "KitchenSinkUnit", at=-0.6)
    h.wall("kitchen", "N", "CabinetPainted", at=1.4, note="larder"); hide(h, h.props[-1], "larder cupboard")
    # parlour
    h.wall("parlour", "W", "SofaPainted", at=2.0)
    h.at("parlour", "Armchair", -2.6, 2.0, yaw=200)
    pt = h.wall("parlour", "S", "SideTable", at=-2.6)
    h.on(pt, "RotaryPhone", role="phone"); h.on(pt, "Radio", dx=0.15, role="radio")
    item(h, pt, "phone table", dx=-0.15)
    h.hung("parlour", "E", "FrameFancy1", at=2.0, y=1.7, note="painting of the mill in 1890")
    # hall
    h.hung("hall", "W", "Mirror", at=1.0, y=1.6)
    h.hung("hall", "S", "FuseBox", at=1.5, y=1.7, role="fuse-box", note="house circuit; the MILL circuit has a second box on the gear floor")
    # mill gear floor PUZZLE
    gears = h.wall("gearfloor", "E", "MillGears", at=0.0, role="puzzle",
                   note="GEAR TRAIN against the wheel wall: pit wheel (fixed) -> 3 empty shafts -> crown wheel; 5 loose gears around the mill")
    puzzle(h, gears, "Restart the mill (ClockPuzzle slot for M1; target rig = GearPuzzle): place 3 of the 5 gears on the shafts so they "
                     "mesh (radii 0.25/0.35/0.45/0.55/0.65; only one combination meshes per match). Then pull the sluice lever: "
                     "the wheel turns, VERY LOUD for 20 s, the bin-floor hopper drops the part.")
    h.wall("gearfloor", "W", "Sacks", at=3.0); hide(h, h.props[-1], "behind the flour sacks", crouch=True)
    h.wall("gearfloor", "S", "ToolChest", at=9.0)
    item(h, h.wall("gearfloor", "N", "TableWood1", at=9.5), "mill bench")
    h.at("gearfloor", "HandTruck", 6.0, 2.5, yaw=0)
    h.hung("gearfloor", "W", "FuseBox", at=-3.0, y=1.7, note="mill circuit box (second fuse box)")
    # stone floor
    h.at("stonefloor", "Millstones", 7.0, 0.0, note="pair of millstones in a wooden tun, hopper above")
    h.wall("stonefloor", "S", "Sacks", at=6.0)
    h.wall("stonefloor", "S", "BarrelWorn", at=9.5)
    note(h, h.wall("stonefloor", "N", "TableSmall", at=9.0), "torn page: the last harvest sheaf 'goes in the hopper' (note 2/3)")
    # bin floor (attic) - container
    hop = h.wall("binfloor", "S", "CellarCrates", at=7.0, note="grain bin + hopper chute: PART CONTAINER (drops when the mill runs)")
    container(h, hop, "Last-harvest sheaf, tied with red thread")
    h.wall("binfloor", "N", "Sacks", at=9.5)
    note(h, h.wall("binfloor", "S", "Wicker2", at=9.5), "child's drawing of the gears (note 3/3)")
    # bed1
    bed(h, "bed1", "W", "BedIron", 2.0, stands=("NightstandPainted",), label="iron bed")
    w = h.wall("bed1", "S", "Wardrobe", at=-2.6); hide(h, w, "wardrobe")
    # bed2
    bed(h, "bed2", "W", "BedIron", -2.0, stands=("",), label="spare bed", hiding=False)
    h.wall("bed2", "N", "ChestOfDrawers", at=-2.2)
    # bath
    h.wall("bath", "E", "Bathtub", at=-2.4, scale=0.85)
    h.wall("bath", "N", "Sink", at=2.2)
    h.hung("bath", "N", "Mirror", at=2.2, y=1.55, role="mirror")
    h.wall("bath", "W", "Toilet", at=-3.2)
    shrine(h, "landing", "N", 0.4)
    return h
