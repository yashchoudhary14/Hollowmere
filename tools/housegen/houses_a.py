from core import House
from kit import *


# ============================================================================================ H00 Blue Door House
def h00():
    h = House(0, "H00", "Blue Door House", "Hale", "Iron gate key", "Clock", "Blue", 2, 3)
    h.traits = dict(archetype="Clockmaker's narrow Georgian brick town house, shop on the ground floor",
                    silhouette="Tall and narrow (7.6 m front, 3 storeys to the ridge), two chimney stacks side by side on the WEST gable, "
                               "a big clock face in the front dormer, a shop window with a hanging clock sign.")
    levels(h, attic=True, g=2.9, f=2.7)
    h.wing("main", (-3.8, -5.5, 3.8, 4.5), ["G", "F", "A"], "BrickRed front (Flemish bond, dark headers), BrickMixed sides/back; "
           "SandstoneBlocks sills, lintels and a 0.3 m plinth course", T_BRICK,
           "Gable roof, ridge along x at 9.6 m, 42 deg, RoofSlateGrey with lead ridge; front flat-roofed dormer 2.2 w x 1.6 h holding a "
           "0.9 m clock face (stopped at 3:00, same as the street clock) with a small window below it; 0.25 m eaves overhang; "
           "cast-iron gutters (GreenRust)")
    G, F, A = "G", "F", "A"
    h.room("shop", "Clock shop", G, (-3.8, 0, 1.8, 4.5), "shop", "FloorDarkPlanks", "PanelDark to 1.0 m, Wallpaper2C above", light="2 brass pendants")
    h.room("hall", "Hall & stair", G, (1.8, -5.5, 3.8, 4.5), "hall", "FloorWornTile", "Wallpaper2 above PanelDark dado", light="1 pendant + fanlight")
    h.room("kitchen", "Kitchen", G, (-3.8, -5.5, -0.6, 0), "kitchen", "FloorQuarryTile", "Whitewash", light="1 pull-chain bulb (PullChainSocket)")
    h.room("parlour", "Back parlour", G, (-0.6, -5.5, 1.8, 0), "living", "Carpet70sB", "WallpaperJacquard (tinted mustard)", light="FloorLampOld + pendant")
    h.room("bed1", "Front bedroom (the Hales')", F, (-3.8, 0, 1.8, 4.5), "bedroom", "CarpetWorn", "Wallpaper1")
    h.room("landing", "Landing", F, (1.8, -5.5, 3.8, 4.5), "landing", "FloorOldWood1", "Wallpaper2")
    h.room("bed2", "Child's bedroom", F, (-3.8, -5.5, -0.6, 0), "bedroom", "FloorOldWood2", "Wallpaper1B (faded blue)")
    h.room("lobby", "Back lobby", F, (-0.6, -2.2, 1.8, 0), "lobby", "FloorOldWood1", "Wallpaper2")
    h.room("bath", "Bathroom", F, (-0.6, -5.5, 1.8, -2.2), "bathroom", "FloorBathTile", "TileWallWhite to 1.4 m, PlasterWorn above")
    h.room("workshop", "Clock workshop (attic)", A, (-3.8, -5.5, 3.8, 4.5), "attic", "FloorOldWood2", "RawPlankWall (rafters exposed), knee walls 1.1 m at z=+-3.4",
           ceiling="exposed rafters + CeilingBeam", light="2 CagedLight + DeskLamp on bench")

    # doors
    h.door("d_front", G, "hall", "N", 2.35, 0.95, 2.15, role="front", kind="6-panel door + fanlight", colour="Blue",
           note="brass knocker, letterbox, 3 stone steps up (house floor +0.45 above street), black iron railings each side")
    h.door("d_back", G, "hall", "S", 2.4, 0.85, role="back", kind="ledged plank (WoodRoughPine)", note="to the back yard")
    h.door("d_shop", G, "hall", "W", 2.6, 0.9, room_b="shop", kind="glazed half-door with shop bell")
    h.door("d_parl", G, "hall", "W", -1.8, 0.85, room_b="parlour", kind="4-panel")
    h.door("d_kit", G, "parlour", "W", -4.4, 0.85, room_b="kitchen", kind="ledged plank")
    h.door("d_bed1", F, "landing", "W", 2.6, 0.8, room_b="bed1")
    h.door("d_lobby", F, "landing", "W", -1.1, 0.8, room_b="lobby", kind="4-panel")
    h.door("d_bed2", F, "lobby", "W", -1.1, 0.8, room_b="bed2", kind="4-panel, painted pale blue")
    h.door("d_bath", F, "lobby", "S", 0.6, 0.75, room_b="bath", kind="4-panel, bolt inside")
    # windows
    h.window("w_shop", "shop", "N", -1.0, 2.6, 1.7, sill=0.55, style="shop window, 6 panes, gilt lettering 'E. HALE - CLOCKS & WATCHES'", curtains="none")
    h.window("w_k", "kitchen", "S", -2.6, 1.0, 1.1, weak=True, style="sash, cracked lower pane, rotten frame", curtains="FabricGingham cafe curtain")
    h.window("w_p", "parlour", "S", 0.6, 1.0, 1.3, weak=True, style="sash, loose putty", curtains="FabricVelvet (ochre)")
    h.window("w_b1", "bed1", "N", -1.0, 1.2, 1.45, style="sash 6-over-6", curtains="FabricTerlenka (orange/brown 1970s)")
    h.window("w_l", "landing", "N", 2.8, 0.7, 1.2, style="narrow sash over the front door")
    h.window("w_b2", "bed2", "S", -2.2, 1.0, 1.2, style="sash", curtains="FabricLinen")
    h.window("w_bath", "bath", "S", 0.6, 0.7, 0.8, sill=1.3, style="frosted top-hung")
    h.window("w_dorm", "workshop", "N", -1.0, 0.9, 0.7, sill=0.9, style="small window under the dormer clock face")
    h.window("w_gable", "workshop", "E", 0.0, 0.6, 0.6, sill=1.2, style="round gable window (oculus)")
    # stairs
    h.stair("s1", "hall", (2.65, -0.3, 3.45, 3.2), G, F, "S", note="15 risers 0.193, oak, turned balusters, carpet runner (Carpet70s) held by brass rods; void above in landing")
    h.stair("s2", "landing", (2.65, -0.3, 3.45, 3.2), G, F, "S", kind="stairwell void (balustrade on W side)")
    h.stair("s3", "landing", (1.9, -4.9, 2.7, -3.7), F, A, "S", kind="folding loft ladder through 0.7x1.0 hatch")
    h.chimney(-3.8, 2.0, serves="shop fireplace (bricked up) / bed1", pots=2)
    h.chimney(-3.8, -2.6, serves="kitchen range = THE HEARTH / bed2", pots=2)

    # ---- shop
    c = h.wall("shop", "S", "Counter", at=-1.2, note="mahogany shop counter, glass top showing 6 pocket watches")
    h.on(c, "PocketWatch", dx=-0.5); h.on(c, "PocketWatch", dx=-0.2); h.on(c, "Magnifier", dx=0.3); h.on(c, "Lighter", dx=0.7, note="brass desk bell substitute")
    for i, z in enumerate((0.9, 2.1, 3.3)):
        h.hung("shop", "W", "WallClock", at=z, y=1.9, note=f"wall clock {i + 1}, each stopped at a different time")
    h.wall("shop", "W", "GrandfatherClock", at=2.7, note="display longcase clock (not the puzzle one), pendulum still")
    h.wall("shop", "W", "DisplayShelves", at=1.0, note="shelves of mantel clocks and alarm clocks")
    h.at("shop", "MantelClock", -3.0, 0.9, yaw=90, y=1.2, note="on shelf")
    h.at("shop", "AlarmClock", -3.0, 1.2, yaw=90, y=0.75, note="on shelf")
    h.at("shop", "TableSmall", -1.0, 2.6, yaw=0, note="window display table")
    h.at("shop", "MantelClock", -1.0, 2.6, yaw=0, y=0.72, note="display clock on the table")
    h.at("shop", "Rug", -1.0, 2.0, note="worn runner, Cloth")
    h.hung("shop", "S", "PictureFrame", at=0.9, y=1.7, role="family-photo",
           note="FAMILY PHOTO: Edwin Hale, wife, small girl in front of this shop (count of people changes at night)")
    note(h, c, "price ledger page (note 1/3)")

    # ---- hall
    h.hung("hall", "W", "Mirror", at=3.6, y=1.6, note="hall mirror over coat hooks (late mirror event candidate)")
    h.wall("hall", "W", "CoatHooks", at=1.5)
    hc = h.wall("hall", "W", "HallConsole", at=-3.6)
    h.on(hc, "RotaryPhone", dx=-0.2, role="phone", note="the house phone (DESIGN 'The phone')")
    item(h, hc, "hall table (beside the phone)")
    h.hung("hall", "S", "FuseBox", at=3.2, y=1.6, role="fuse-box", note="FUSE BOX by the back door")
    hide_at(h, "hall", 3.0, 0.3, "UnderStairCupboard: door at x=2.55 facing W, void under the top of the stair")
    h.at("hall", "UnderStairCupboard", 3.0, 0.45, yaw=270, role="allow-overlap", note="under-stair cupboard (on the stair footprint, below the flight)")

    # ---- kitchen (THE hearth)
    hr = h.wall("kitchen", "W", "Hearth", at=-2.6, role="hearth", note="black-leaded RANGE set in a 1.8 m brick opening; mantel shelf with tea caddies")
    h.on(hr, "MantelClock", dx=0.4, note="stopped at 3:00")
    h.on(hr, "Candlestick", dx=-0.6)
    item(h, hr, "kitchen mantel", dx=0.0)
    h.wall("kitchen", "N", "Dresser", at=-2.0, note="Welsh dresser with willow-pattern plates")
    s = h.wall("kitchen", "S", "KitchenSinkUnit", at=-2.6)
    h.on(s, "Kettle", dx=0.3)
    t = h.at("kitchen", "TableWood1", -1.6, -2.4, yaw=90)
    h.on(t, "TeaSet"); h.on(t, "OilLamp", dx=0.3)
    note(h, t, "Mrs Hale's shopping list (note 2/3)")
    h.at("kitchen", "ChairDining", -2.3, -2.4, yaw=90); h.at("kitchen", "ChairDining", -0.9, -2.9, yaw=270)
    h.corner("kitchen", "NE", "FridgeSmall", face="E")
    h.at("kitchen", "Broom", -0.85, -5.05, yaw=0, role="allow-overlap")

    # ---- parlour
    h.wall("parlour", "E", "Armchair", at=-4.0)
    h.wall("parlour", "N", "Sofa3", at=0.6, note="facing the TV")
    tv = h.wall("parlour", "S", "TV2", at=0.6, role="tv", note="TV with rabbit-ear aerial on the floor-standing cabinet")
    h.at("parlour", "CoffeeTableRound", 0.5, -2.8)
    r = h.wall("parlour", "E", "SideTableTall", at=-3.0)
    h.on(r, "Radio", role="radio", note="radio: static rises when the Hollow is near")
    h.hung("parlour", "W", "FrameHanging1", at=-1.0, y=1.6, role="family-photo", note="wedding photograph")
    h.hung("parlour", "N", "WallClock", at=0.6, y=2.1)

    # ---- bed1
    bed(h, "bed1", "W", "Bed", 2.2, stands=("Nightstand",), stand_side="both", label="big bed")
    w1 = h.wall("bed1", "S", "Wardrobe", at=-0.6)
    hide(h, w1, "wardrobe")
    h.wall("bed1", "E", "ChestOfDrawers", at=1.2)
    h.at("bed1", "Rug", -1.0, 2.2)
    h.hung("bed1", "E", "FrameHanging2", at=3.3, y=1.6, role="family-photo", note="the daughter's school photo")

    # ---- bed2 (child) - the child's drawing lives here
    bed(h, "bed2", "W", "BedIron", -3.0, stands=("NightstandPainted",), stand_side="both", label="iron child's bed")
    rh = h.corner("bed2", "NW", "RockingHorse", face="N")
    h.wall("bed2", "W", "ChestOfDrawers", at=-1.5, note="drawers lined with newspaper from 1971")
    dk = h.at("bed2", "TableSmall", -1.6, -4.6, yaw=0)
    note(h, dk, "CHILD'S DRAWING: blue door, clock showing the puzzle time (note 3/3, drives the Clock puzzle)")

    # ---- bath
    bt = h.wall("bath", "E", "Bathtub", at=-4.2)
    h.at("bath", "BathCurtainRail", 1.3, -4.2, yaw=270, role="allow-overlap")
    hide(h, bt, "behind the bath curtain")
    h.wall("bath", "W", "Sink", at=-4.6)
    h.hung("bath", "W", "Mirror", at=-4.6, y=1.55, role="mirror", note="BATHROOM MIRROR (mirror events)")
    h.wall("bath", "S", "Toilet", at=0.35)
    h.at("bath", "MedicalBox", -0.4, -3.4, yaw=90, y=1.4, role="allow-overlap", note="wall cabinet: bandage spawn")
    h.hung("lobby", "N", "FrameHanging3", at=0.6, y=1.6, note="framed clock advert")

    # ---- attic workshop (PUZZLE)
    pz = h.wall("workshop", "S", "GrandfatherClock", at=0.0, role="puzzle",
                note="THE PUZZLE CLOCK (existing ClockPuzzle rig): case 0.55x0.32x2.05; winding keyhole 1.38 m; part chest is INSIDE the trunk door")
    puzzle(h, pz, "ClockPuzzle: wind with the WindingKey (spawns on one ItemSpot), set the hour shown in the child's drawing")
    bench = h.wall("workshop", "N", "TableWood3", at=-1.0, note="clockmaker's bench under the dormer, 2.0 m")
    h.on(bench, "DeskLamp", dx=0.8); h.on(bench, "Magnifier", dx=-0.2); h.on(bench, "Pliers", dx=0.3); h.on(bench, "BenchVice", dx=-0.8)
    item(h, bench, "workbench (between the tools)", dx=0.2)
    h.at("workshop", "StoolWood1", -1.0, 2.8, yaw=0)
    for i in range(2):
        h.wall("workshop", "W", "GrandfatherClock", at=-2.0 + i * 0.7, note="unfinished longcase case, no movement")
    ch = h.wall("workshop", "E", "ChestWorn", at=2.0, note="PART CONTAINER skin: brass-bound clockmaker's chest")
    container(h, ch, "PartContainer -> Iron gate key (or inside the clock trunk door: Claude Code picks the chest for M1 compatibility)")
    h.hung("workshop", "W", "WallClock", at=1.0, y=1.5)
    h.wall("workshop", "E", "MetalRack", at=-2.2, note="spare clock parts in trays")
    hide(h, h.wall("workshop", "E", "CellarCrates", at=-4.4), "behind the crate stack under the eaves", crouch=True)
    shrine(h, "landing", "E", -4.2, "shrine niche on the landing wall")
    return h


# ============================================================================================ H01 Rao House
def h01():
    h = House(1, "H01", "Rao House", "Rao", "Family seal ring", "CodeLock", "Green", 1, 9)
    h.traits = dict(archetype="1930s pebbledash detached house that never opens its shutters, big walled vegetable garden",
                    silhouette="Hipped roof with one fat central chimney, a two-storey canted bay on the left, ALL windows shuttered "
                               "(green shutters, closed), 'RAO' painted on the garden gate, a lean-to greenhouse against the back wall.")
    levels(h, cellar=True, g=2.7, f=2.5)
    h.wing("main", (-4.6, -4.4, 4.6, 4.4), ["C", "G", "F"], "Pebbledash (grey-brown) over brick; BrickRed plinth 0.5 m; "
           "white-painted window surrounds", T_RENDER,
           "Hipped roof 35 deg, RoofTilesGrey (1960s re-roof), hip ridge tiles; central chimney at (−0.6, 0.2) rising 1.2 m; "
           "0.4 m boxed eaves with white fascia")
    h.wing("bay", (-4.0, 4.4, -1.0, 5.2), ["G", "F"], "Pebbledash, 3-face canted bay (shown as rect here; facets at 45 deg on the "
           "front corners)", T_RENDER, "Lead-covered flat top at 5.5 m")
    h.wing("greenhouse", (-2.0, -6.4, 3.0, -4.4), ["G"], "Lean-to greenhouse: brick base 0.6 m, timber + glass (half the panes broken)",
           0.12, "Glass lean-to at 25 deg")
    h.room("front", "Front room (sitting room)", "G", (-4.6, 0.0, -0.6, 4.4), "living", "Carpet70s", "WallpaperQuatrefoil (brown/orange)", light="pendant with fringed shade")
    h.room("bayG", "Bay (front room)", "G", (-4.0, 4.4, -1.0, 5.2), "living-bay", "Carpet70s", "WallpaperQuatrefoil")
    h.room("hall", "Hall", "G", (-0.6, 0.0, 1.4, 4.4), "hall", "FloorWornTile", "Wallpaper2C")
    h.room("dining", "Dining room", "G", (1.4, 0.0, 4.6, 4.4), "dining", "FloorParquet", "Wallpaper2")
    h.room("kitchen", "Kitchen", "G", (-4.6, -4.4, 1.4, 0.0), "kitchen", "FloorQuarryTile", "TileWallWhite to 1.2 m + PlasterWorn")
    h.room("back", "Back lobby & cellar stair", "G", (1.4, -4.4, 4.6, 0.0), "utility", "FloorBrick", "Whitewash")
    h.room("glass", "Greenhouse", "G", (-2.0, -6.4, 3.0, -4.4), "greenhouse", "FloorBrick", "glass")
    h.room("bed1", "Parents' bedroom", "F", (-4.6, 0.0, -0.6, 4.4), "bedroom", "CarpetWorn", "Wallpaper1")
    h.room("bayF", "Bay (bedroom)", "F", (-4.0, 4.4, -1.0, 5.2), "bedroom-bay", "CarpetWorn", "Wallpaper1")
    h.room("landing", "Landing", "F", (-0.6, 0.0, 1.4, 4.4), "landing", "Carpet70sB", "Wallpaper2C")
    h.room("bed2", "Grandmother's bedroom", "F", (1.4, 0.0, 4.6, 4.4), "bedroom", "FloorOldWood1", "Wallpaper2")
    h.room("bed3", "Box room (shrine room)", "F", (1.4, -4.4, 4.6, 0.0), "bedroom", "FloorOldWood2", "PlasterWorn")
    h.room("bath", "Bathroom", "F", (-4.6, -4.4, 1.4, 0.0), "bathroom", "FloorBathTile", "TileWallWhite (avocado border tiles)")
    h.room("cellar", "Cellar", "C", (-4.6, -4.4, 4.6, 4.4), "cellar", "FloorConcrete", "Whitewash flaking to brick", ceiling="joists",
           light="one bulb on a cord, dead until the fuse box is reset")
    h.door("d_front", "G", "hall", "N", 0.4, 0.9, role="front", kind="half-glazed door, sunrise pattern glass", colour="Green",
           note="tiled porch step; garden gate 6 m in front painted 'RAO'")
    h.door("d_back", "G", "back", "E", -2.2, 0.85, role="back", kind="ledged plank, two bolts", note="opens to the side path")
    h.door("d_front_r", "G", "hall", "W", 3.4, 0.85, room_b="front", kind="4-panel")
    h.door("d_din", "G", "hall", "E", 3.9, 0.85, room_b="dining", kind="4-panel")
    h.door("d_kit", "G", "hall", "S", -0.1, 0.85, room_b="kitchen", kind="4-panel")
    h.door("d_backl", "G", "kitchen", "E", -2.2, 0.85, room_b="back", kind="ledged")
    h.door("d_glass", "G", "kitchen", "S", -0.6, 0.8, room_b="glass", kind="glazed door")
    h.door("d_b1", "F", "landing", "W", 2.6, 0.8, room_b="bed1")
    h.door("d_b2", "F", "landing", "E", 0.5, 0.8, room_b="bed2")
    h.door("d_bath", "F", "landing", "S", -0.1, 0.75, room_b="bath")
    h.door("d_b3", "F", "bed2", "S", 3.6, 0.75, room_b="bed3", kind="connecting door, wallpapered over (visible seam)")
    for wid, room, side, at, wd, ht, weak in (("w_bay", "bayG", "N", -2.5, 2.0, 1.5, False), ("w_din", "dining", "N", 3.0, 1.4, 1.3, False),
                                             ("w_k", "kitchen", "W", -2.2, 1.1, 1.1, True), ("w_back", "back", "S", 3.4, 0.7, 0.9, True),
                                             ("w_bayF", "bayF", "N", -2.5, 2.0, 1.3, False), ("w_b2", "bed2", "N", 3.0, 1.3, 1.2, False),
                                             ("w_bath", "bath", "S", -3.6, 0.8, 0.9, False), ("w_b3", "bed3", "E", -2.2, 0.8, 1.0, False)):
        h.window(wid, room, side, at, wd, ht, weak=weak, sill=1.3 if wid == "w_bath" else 0.85, style="1930s casement, leaded top-lights",
                 shutters="outside, green, CLOSED and nailed (DESIGN Rao signature)" if side in "NEW" else None,
                 curtains="FabricTerlenka net + heavy curtain")
    h.stair("s1", "hall", (0.4, 0.5, 1.3, 3.8), "G", "F", "S", note="14 risers, painted balusters, Carpet70s runner")
    h.stair("s1u", "landing", (0.4, 0.5, 1.3, 3.8), "G", "F", "S", kind="void")
    h.stair("s2", "back", (1.6, -4.0, 4.4, -3.1), "G", "C", "E", kind="steep cellar stair, 11 risers, open treads, handrail rope")
    h.stair("s2c", "cellar", (1.6, -4.0, 4.4, -3.1), "G", "C", "E", kind="arrives in the cellar")
    h.chimney(-0.6, 0.0, w=1.0, d=0.9, serves="front-room fire = THE HEARTH (back-to-back with kitchen boiler)", pots=3)
    # front room
    hr = h.wall("front", "S", "Hearth", at=-1.8, role="hearth", note="tiled 1930s fire surround (beige tiles) around the open grate")
    h.on(hr, "FrameStanding1", dx=-0.6, role="family-photo", note="FAMILY PHOTO 1 (counted by the code)")
    h.on(hr, "FrameStanding2", dx=0.6, role="family-photo", note="FAMILY PHOTO 2 (counted)")
    h.on(hr, "MantelClock", dx=0.0, note="stopped at 9:00")
    h.wall("front", "W", "Sofa2", at=2.0)
    h.at("front", "Armchair", -1.6, 2.2, yaw=225)
    tv = h.wall("front", "E", "Cabinet70s", at=1.6, note="teak sideboard")
    h.on(tv, "TV", role="tv")
    h.at("front", "CoffeeTable", -2.9, 2.3, yaw=90)
    h.at("front", "Rug", -2.9, 2.3)
    st = h.at("front", "SideTable", -4.0, 0.7)
    h.on(st, "RotaryPhone", role="phone")
    item(h, st, "phone table")
    h.at("bayG", "PlanterPot", -2.5, 4.66, note="dead aspidistra in the bay")
    # hall
    h.hung("hall", "W", "Mirror", at=1.0, y=1.6, note="hall mirror")
    h.hung("hall", "E", "FrameHanging3", at=1.0, y=1.6, role="family-photo", note="FAMILY PHOTO 3: three generations on the steps (counted)")
    # dining
    dt = h.at("dining", "DiningTable", 3.0, 2.2, yaw=90)
    note(h, dt, "Grandmother's letter about the cellar (note 1/3)")
    for dz in (-0.5, 0.5):
        h.at("dining", "ChairDining", 2.25, 2.2 + dz, yaw=90); h.at("dining", "ChairDining", 3.75, 2.2 + dz, yaw=270)
    h.wall("dining", "E", "CabinetVintage", at=0.8, note="glass-fronted china cabinet")
    h.hung("dining", "S", "FrameFancy1", at=3.0, y=1.6, role="family-photo", note="FAMILY PHOTO 4: wedding (counted)")
    # kitchen
    h.wall("kitchen", "N", "Cooker", at=-3.2)
    ks = h.wall("kitchen", "N", "KitchenSinkUnit", at=-2.3)
    h.wall("kitchen", "N", "KitchenUnit", at=-1.4)
    kt = h.at("kitchen", "TablePainted", -1.8, -2.4, yaw=0)
    h.on(kt, "PotEnamel"); h.on(kt, "Onions", dx=0.3)
    item(h, kt, "kitchen table")
    h.at("kitchen", "ChairPainted1", -1.8, -3.15, yaw=0); h.at("kitchen", "ChairPainted2", -1.8, -1.65, yaw=180)
    h.wall("kitchen", "S", "Fridge", at=0.6)
    h.wall("kitchen", "W", "CabinetPainted", at=-0.8, note="larder cupboard - walk-in hiding")
    hide(h, h.props[-1], "larder cupboard")
    # back lobby
    h.hung("back", "W", "CoatHooks", at=-1.0, y=0.85)
    h.wall("back", "N", "Toolbox", at=2.4)
    # greenhouse
    for x in (-1.2, 0.4):
        h.wall("glass", "S", "PlanterBox1", at=x)
    h.wall("glass", "E", "SteelShelves2", at=-5.4)
    h.at("glass", "SeedTray", 2.2, -5.4, y=0.9, role="allow-overlap")
    h.at("glass", "WateringCan", 1.5, -4.8)
    # bed1
    bed(h, "bed1", "S", "Bed", -2.6, label="double bed")
    w = h.wall("bed1", "W", "Wardrobe", at=2.6); hide(h, w, "wardrobe (bay side)")
    dr = h.wall("bed1", "E", "Commode", at=1.0, note="dressing table with triple mirror")
    note(h, dr, "Mr Rao's diary: 'count only those who were here before the autumn' (note 2/3)")
    # bed2
    bed(h, "bed2", "E", "BedIron", 2.0, stands=("NightstandPainted",), stand_side="left", label="iron bed", hiding=False)
    h.wall("bed2", "S", "SideTable", at=2.65, note="washstand-height side table with a jug")
    h.at("bed2", "RockingChair", 2.0, 3.2, yaw=180)
    # bed3 box room
    h.wall("bed3", "S", "DayBed", at=3.0)
    shrine(h, "bed3", "E", -1.0, "shrine niche over the day-bed")
    h.wall("bed3", "N", "Suitcase", at=2.4)
    # bath
    bt = h.wall("bath", "W", "Bathtub", at=-2.2); hide(h, bt, "behind the avocado bath curtain")
    h.at("bath", "BathCurtainRail", -3.7, -2.2, yaw=90, role="allow-overlap")
    h.wall("bath", "N", "Sink", at=-1.0)
    h.hung("bath", "N", "Mirror", at=-1.0, y=1.55, role="mirror")
    h.wall("bath", "E", "Toilet", at=-3.4)
    # cellar (PUZZLE)
    fb = h.hung("cellar", "S", "FuseBox", at=0.6, y=1.5, role="fuse-box", note="FUSE BOX - must be reset to light the cellar (DESIGN Rao)")
    safe = h.wall("cellar", "N", "ToolChest", at=-2.0, role="puzzle",
                  note="LOCKBOX: steel box bolted to the bench, 4 code dials (CodeLockPuzzle)")
    puzzle(h, safe, "CodeLockPuzzle: digits = people counted in 4 named family photos AS THEY WERE BY DAY")
    container(h, safe, "Lockbox holds the family seal ring")
    h.wall("cellar", "W", "SteelShelves", at=0.0, note="jars of pickled lime, tins")
    h.at("cellar", "BarrelWorn", -3.8, -3.6); hide(h, h.props[-1], "behind the barrels", crouch=True)
    h.at("cellar", "CellarCrates", -2.2, -3.5, yaw=0, note="crates with a crouch gap behind"); hide(h, h.props[-1], "behind the crates", crouch=True)
    h.at("cellar", "TinnedFood", -1.0, -3.8, yaw=0)
    wr = h.wall("cellar", "E", "Chalkboard", at=1.5, note="WRITING ON THE WALL (chalk) explaining the code: which photos count")
    note(h, wr, "chalk writing (note 3/3)", dx=0, dz=0)
    item(h, h.wall("cellar", "N", "ShelfRustic", at=1.5), "cellar shelf")
    return h


# ============================================================================================ H02 Widow's House
def h02():
    h = House(2, "H02", "Widow's House", "Marsh", "Hearth ember", "Sequence", "Black", 1, 6)
    h.traits = dict(archetype="Low thatched cob cottage, 1.5 storeys, with a deep front porch",
                    silhouette="Long, low and lumpy: thatch sweeping to 2.2 m, two eyebrow dormers, one huge stone chimney at the "
                               "EAST end, a timber porch with a rocking chair, black ribbon on the black door, ONE lit window.")
    levels(h, attic=False, g=2.4, f=2.2)
    h.wing("main", (-6.0, -3.0, 6.0, 3.0), ["G", "F"], "Cob (thick, rounded corners) limewashed off-white (RenderCracked), "
           "tarred base 0.4 m", 0.6,
           "Thatch 50 deg, 0.35 m thick, eaves at 2.6 m, ridge 6.3 m with a patterned block-cut ridge (ThatchReed); two eyebrow "
           "dormers over the bedrooms; huge external stone stack on the east gable")
    h.wing("porch", (-1.6, 3.0, 1.6, 4.8), ["G"], "Open timber porch on 4 posts, low wall each side (Stone 0.5 h)", 0.1,
           "Mono-pitch thatch continuation")
    h.wing("scullery", (-6.0, -5.2, -1.6, -3.0), ["G"], "Brick lean-to (BrickPaintedWorn)", 0.25, "CorrugatedRust lean-to roof")
    G, F = "G", "F"
    h.room("porch", "Porch", G, (-1.6, 3.0, 1.6, 4.8), "porch", "FloorFlagstone", "open")
    h.room("living", "Living room (the hearth room)", G, (0.8, -3.0, 6.0, 3.0), "living", "FloorFlagstone", "Whitewash with smoke staining, "
           "CeilingBeam every 0.9 m", ceiling="low beamed ceiling 2.3 m", light="NO electric light: oil lamps only (Room.Light = lamp)")
    h.room("kitchen", "Kitchen", G, (-2.8, -3.0, 0.8, 3.0), "kitchen", "FloorBrick", "Whitewash")
    h.room("bedG", "Downstairs bedroom (the widow's)", G, (-6.0, -0.2, -2.8, 3.0), "bedroom", "FloorOldWood1", "Wallpaper1B (faded)")
    h.room("bathG", "Bathroom (converted pantry)", G, (-6.0, -3.0, -2.8, -0.2), "bathroom", "FloorBathTile", "TileWallWhite")
    h.room("scull", "Scullery", G, (-6.0, -5.2, -1.6, -3.0), "utility", "FloorBrick", "BrickPaintedWorn")
    h.room("loft", "Loft bedroom (son's, unused)", F, (-3.4, -3.0, 6.0, 3.0), "bedroom", "FloorOldWood2", "PlasterWorn on sloping ceiling",
           note="usable width shrinks to 3.6 m under the thatch")
    h.room("store", "Loft store (attic)", F, (-6.0, -3.0, -3.4, 3.0), "attic", "FloorOldWood2", "RawPlankWall")
    h.door("d_front", G, "porch", "S", 0.0, 0.85, room_b="kitchen", role="front-inner", kind="ledged black door, BLACK RIBBON bow nailed at 1.6 m")
    h.door("d_porch", G, "porch", "N", 0.0, 1.2, role="front", kind="open porch entrance (no leaf)", colour="Black", note="the porch is the threshold; the ribboned door is the real front door")
    h.door("d_back", G, "scull", "S", -4.0, 0.8, role="back", kind="ledged plank, latch")
    h.door("d_liv", G, "kitchen", "E", 1.2, 0.85, room_b="living", kind="ledged, 1.85 m high (duck!)", height=1.85)
    h.door("d_bed", G, "kitchen", "W", 0.3, 0.8, room_b="bedG", height=1.9)
    h.door("d_bath", G, "kitchen", "W", -1.6, 0.75, room_b="bathG", height=1.9)
    h.door("d_scull", G, "kitchen", "S", -2.2, 0.8, room_b="scull", height=1.9)
    h.door("d_store", F, "loft", "W", 1.5, 0.7, room_b="store", height=1.6, kind="low plank door")
    h.window("w_lit", "living", "N", 3.4, 1.0, 0.9, sill=0.8, style="small casement - THE SINGLE LIT WINDOW (candle on the sill)", curtains="FabricLinen half-drawn")
    h.window("w_k", "kitchen", "N", -1.6, 0.9, 0.9, weak=True, style="casement, rotten frame")
    h.window("w_liv2", "living", "S", 3.4, 0.9, 0.9, weak=True, style="casement, cracked pane")
    h.window("w_bed", "bedG", "N", -4.7, 0.8, 0.9, style="casement", curtains="FabricLinen")
    h.window("w_liv3", "living", "E", -1.5, 0.6, 0.6, sill=1.0, style="tiny window beside the chimney breast")
    h.window("w_dorm1", "loft", "N", 2.5, 0.9, 0.6, sill=0.4, style="eyebrow dormer (in thatch)")
    h.window("w_dorm2", "loft", "S", -0.8, 0.9, 0.6, sill=0.4, style="eyebrow dormer (back)")
    h.window("w_sc", "scull", "W", -4.1, 0.7, 0.7, style="scullery window")
    h.stair("s1", "kitchen", (-0.1, -2.9, 0.75, 0.2), G, F, "S", kind="steep boxed winder-less stair behind a ledged door", note="12 risers 0.22 - it creaks loudly (NoiseSystem on use)")
    h.stair("s1u", "loft", (-0.1, -2.9, 0.75, 0.2), G, F, "S", kind="void")
    h.chimney(6.0, 0.0, w=1.6, d=1.4, above_ridge=0.8, mat="Stone", serves="inglenook = THE HEARTH", pots=1)
    # living / inglenook - the PUZZLE is the hearth itself
    hr = h.wall("living", "E", "Hearth", at=0.0, role="hearth puzzle",
                note="INGLENOOK 2.4 w: stone surround, bressumer beam, bread oven door on the left, fire burns LOW and GREY (puzzle state)")
    puzzle(h, hr, "Widow's puzzle (SequencePuzzle until the bespoke rig exists): feed the right keepsakes to the fire in order; "
                  "final item = the widow's wedding photograph -> fire turns orange -> ember forms")
    container(h, hr, "The ember forms in the grate; it must be carried out in the oil lantern (no chest)")
    h.on(hr, "Candleholders", dx=-0.6); h.on(hr, "FrameStanding2", dx=0.6, role="family-photo", note="photo of Mr Marsh (died last autumn)")
    item(h, hr, "mantel beam")
    h.at("living", "RockingChair", 4.2, 1.4, yaw=250)
    h.at("living", "Armchair", 3.4, -1.0, yaw=300)
    h.wall("living", "N", "ShelvesPainted", at=1.6, note="shelf of Bibles and jam jars")
    t = h.at("living", "TableRound", 2.4, -0.4)
    h.on(t, "OilLamp", role="lamp"); h.on(t, "Radio", dx=0.3, role="radio", note="valve radio, plays the shipping forecast by day")
    h.on(t, "RotaryPhone", dx=-0.3, role="phone", note="black bakelite phone - the widow never answers it")
    dz = h.wall("living", "S", "Dresser", at=1.6, note="oak dresser with lustre jugs")
    item(h, dz, "dresser shelf")
    note(h, t, "the widow's last letter: which keepsakes 'he loved' (note 1/3)")
    h.wall("living", "W", "SewingMachine", at=-1.4)
    h.at("living", "Rug", 3.6, 0.0)
    h.hung("living", "W", "WallClock", at=-2.2, y=1.7, note="stopped at 6:00")
    # kitchen
    kt = h.at("kitchen", "TableWood2", -1.4, -0.6, yaw=90)
    h.on(kt, "BowlWood"); h.on(kt, "Candlestick", dx=0.4)
    note(h, kt, "recipe card with the black-ribbon pattern (note 2/3)")
    h.at("kitchen", "ChairGallinera", -2.15, -0.6, yaw=90)
    h.wall("kitchen", "S", "KitchenSinkUnit", at=-0.9)
    # bedroom
    bed(h, "bedG", "W", "BedIron", 1.4, stands=("NightstandPainted",), stand_side="right", label="iron bed")
    w = h.wall("bedG", "S", "Wardrobe", at=-4.7); hide(h, w, "wardrobe")
    h.hung("bedG", "E", "FrameHanging1", at=2.4, y=1.6, role="family-photo", note="WEDDING PHOTOGRAPH (the one the fire wants; in the cross-house variant it is MISSING and lives in another house)")
    item(h, h.at("bedG", "Suitcase", -4.2, 0.6, yaw=0), "suitcase under the window")
    # bath
    h.wall("bathG", "S", "Bathtub", at=-4.7, scale=0.85)
    hide(h, h.props[-1], "behind the bath (narrow)", crouch=True)
    h.wall("bathG", "N", "Sink", at=-4.9)
    h.hung("bathG", "N", "Mirror", at=-4.9, y=1.5, role="mirror")
    h.wall("bathG", "W", "Toilet", at=-1.2)
    # scullery
    h.wall("scull", "S", "Copper", at=-5.2)
    h.wall("scull", "E", "Mangle", at=-4.4)
    h.hung("scull", "N", "FuseBox", at=-5.3, y=1.6, role="fuse-box", note="FUSE BOX (the only electric circuit: kitchen + bedroom)")
    # loft
    bed(h, "loft", "S", "BedIron", 3.4, stands=("NightstandPainted",), stand_side="right", label="son's bed")
    h.wall("loft", "N", "ChestOfDrawers", at=4.0)
    h.at("loft", "Suitcase", 1.4, -1.0)
    shrine(h, "loft", "E", 0.8, "niche in the chimney breast (loft)")
    # store (attic)
    h.wall("store", "W", "CellarCrates", at=0.0); hide(h, h.props[-1], "behind the stacked trunks", crouch=True)
    h.wall("store", "N", "ChestWorn", at=-4.8, note="his sea chest: keepsakes for the puzzle")
    note(h, h.props[-1], "his letters (note 3/3)")
    return h


# ============================================================================================ H03 Schoolmaster's House
def h03():
    h = House(3, "H03", "Schoolmaster's House", "Penrose", "Bell clapper", "CodeLock", "White", 2, 8)
    h.traits = dict(archetype="Victorian Gothic stone house attached to the schoolyard wall",
                    silhouette="Steep front-facing GABLE with carved bargeboards and a finial, pointed-arch windows, two tall "
                               "diagonal-set brick chimneys, a little wooden bell-cote on the rear wing ridge, white door in a stone arch.")
    levels(h, cellar=False, attic=True, g=3.0, f=2.7)
    h.wing("main", (-3.5, -3.0, 3.5, 5.0), ["G", "F", "A"], "StoneBlock (grey squared rubble) with SandstoneBlocks quoins and "
           "pointed-arch dressings", T_STONE,
           "Steep 55 deg gable facing the street (ridge along z), RoofSlateGrey, carved bargeboards (dark green), finial at the apex")
    h.wing("rear", (-6.5, -9.0, 3.5, -3.0), ["G", "F"], "StoneBlock front, BrickMixed back wall", T_STONE,
           "Gable roof, ridge along x at 8.3 m, RoofSlateGrey; small timber bell-cote (Bell scaled to 0.4 m) on the ridge at x=-4")
    G, F, A = "G", "F", "A"
    h.room("hall", "Hall", G, (-3.5, 1.4, 0.3, 5.0), "hall", "FloorWornTile", "Wallpaper2 over PanelDark dado")
    h.room("study", "Study (the PUZZLE room)", G, (0.3, -3.0, 3.5, 5.0), "study", "FloorParquet", "PanelDark to 2.0 m, Wallpaper2C", light="green-shaded DeskLamp + pendant")
    h.room("sitting", "Sitting room", G, (-3.5, -3.0, 0.3, 1.4), "living", "Carpet70sB", "WallpaperJacquard")
    h.room("kitchen", "Kitchen", G, (-6.5, -9.0, -1.5, -3.0), "kitchen", "FloorQuarryTile", "Whitewash")
    h.room("pantry", "Pantry & back passage", G, (-1.5, -9.0, 3.5, -3.0), "utility", "FloorBrick", "Whitewash")
    h.room("bed1", "Schoolmaster's bedroom", F, (0.3, -3.0, 3.5, 5.0), "bedroom", "FloorDarkPlanks", "Wallpaper1")
    h.room("landing", "Landing", F, (-3.5, -3.0, 0.3, 5.0), "landing", "FloorOldWood1", "Wallpaper2")
    h.room("bed2", "Spare bedroom (lodger)", F, (-6.5, -9.0, -1.5, -3.0), "bedroom", "FloorOldWood2", "Wallpaper1B")
    h.room("bath", "Bathroom", F, (-1.5, -9.0, 3.5, -3.0), "bathroom", "FloorBathTile", "TileWallWhite")
    h.room("attic", "Attic (old schoolroom stores)", A, (-3.5, -3.0, 3.5, 5.0), "attic", "FloorOldWood2", "RawPlankWall")
    h.door("d_front", G, "hall", "N", -1.6, 1.0, height=2.3, role="front", kind="pointed-arch boarded door with iron strap hinges", colour="White")
    h.door("d_back", G, "pantry", "S", 1.8, 0.85, role="back", note="to the schoolyard gate")
    h.door("d_study", G, "hall", "E", 3.6, 0.9, room_b="study")
    h.door("d_sit", G, "hall", "S", -0.6, 0.9, room_b="sitting")
    h.door("d_kit", G, "sitting", "S", -2.0, 0.85, room_b="kitchen")
    h.door("d_pan", G, "kitchen", "E", -6.0, 0.85, room_b="pantry")
    h.door("d_study2", G, "study", "S", 0.9, 0.85, room_b="pantry", kind="baize-covered door")
    h.door("d_b1", F, "landing", "E", 3.6, 0.85, room_b="bed1")
    h.door("d_b2", F, "landing", "S", -2.0, 0.8, room_b="bed2")
    h.door("d_bath", F, "landing", "S", -0.5, 0.75, room_b="bath")
    h.window("w_study", "study", "N", 1.9, 1.3, 2.0, sill=0.8, style="tall pointed-arch, 2 lights with a quatrefoil")
    h.window("w_hall", "hall", "W", 3.2, 0.7, 1.4, style="lancet")
    h.window("w_sit", "sitting", "W", -0.8, 1.2, 1.5, weak=True, style="sash, cracked")
    h.window("w_study2", "study", "E", -1.0, 1.0, 1.6, style="lancet pair")
    h.window("w_k", "kitchen", "S", -4.0, 1.1, 1.2, weak=True, style="sash, rotten")
    h.window("w_b1", "bed1", "N", 1.9, 1.0, 1.5, style="pointed arch")
    h.window("w_b2", "bed2", "W", -6.0, 1.0, 1.2, style="sash")
    h.window("w_bath", "bath", "S", 1.0, 0.7, 0.8, sill=1.3, style="frosted")
    h.window("w_att", "attic", "N", 0.0, 0.6, 0.9, sill=0.6, style="slit window in the gable apex")
    h.stair("s1", "landing", (-3.4, -2.8, -2.5, 0.8), G, F, "S", kind="void (balustrade)")
    h.stair("s1g", "sitting", (-3.4, -2.8, -2.5, 0.8), G, F, "S", kind="open-string stair along the sitting room W wall", note="14 risers, pitch pine")
    h.stair("s2", "landing", (-0.6, 3.6, 0.2, 4.8), F, A, "N", kind="ladder to the attic hatch")
    h.chimney(-6.5, -6.0, w=0.7, d=0.7, above_ridge=1.6, serves="kitchen range = THE HEARTH (diagonal-set shaft)", pots=1)
    h.chimney(3.5, 0.5, w=0.7, d=0.7, above_ridge=1.6, serves="study fireplace (cold)", pots=1)
    # study PUZZLE
    desk = h.wall("study", "E", "OfficeDesk", at=0.5, note="partner's desk; drawer locked")
    h.on(desk, "DeskLamp", dx=0.5); h.on(desk, "Globe", dx=-0.5)
    sl = h.on(desk, "Slate", dx=0.0, role="puzzle", note="THE SLATE: 5-letter wooden frame; chalk letters scattered round the house")
    puzzle(h, sl, "CodeLockPuzzle reskinned: spell the hidden word on the slate from chalk letters (letters -> digits via the "
                  "'alphabet cipher' poster); 4-dial cupboard opens")
    for z in (2.2, 1.1, 0.0):
        h.wall("study", "W", "Shelf", at=z, note="floor-to-ceiling books")
    h.wall("study", "E", "DrawerCabinet", at=3.0)
    cab = h.wall("study", "S", "CabinetVintage", at=2.4, note="glass-fronted cupboard: the PART CONTAINER (4-dial lock)")
    container(h, cab, "Bell clapper inside, wrapped in a school flag")
    h.at("study", "Armchair", 2.0, 3.3, yaw=160)
    h.at("study", "Rug", 1.9, 1.0)
    h.hung("study", "W", "FrameFancy2", at=-1.5, y=1.7, role="family-photo", note="FAMILY PHOTO: class of 1952 with Mr Penrose")
    item(h, h.at("study", "SideTable", 2.6, 4.1), "study side table")
    note(h, desk, "school log book (note 1/3)")
    # hall
    h.hung("hall", "S", "Mirror", at=-1.6, y=1.6, note="hall mirror")
    hc = h.wall("hall", "S", "HallConsole", at=-2.4)
    h.on(hc, "RotaryPhone", role="phone")
    h.hung("hall", "N", "FuseBox", at=-3.0, y=1.8, role="fuse-box", note="FUSE BOX over the front door")
    # sitting
    h.wall("sitting", "E", "Sofa", at=-0.8)
    h.at("sitting", "Armchair", -1.6, 0.4, yaw=110)
    r = h.wall("sitting", "N", "SideTable", at=-2.3)
    h.on(r, "Radio", role="radio")
    h.at("sitting", "CoffeeTable", -1.0, -0.9, yaw=90)
    # kitchen
    hr = h.wall("kitchen", "W", "Hearth", at=-6.0, role="hearth", note="cast-iron range in an arched recess")
    item(h, hr, "range mantel")
    h.wall("kitchen", "N", "Dresser", at=-4.2)
    kt = h.at("kitchen", "TableWood1", -4.0, -6.6, yaw=90)
    note(h, kt, "letter from a pupil's mother (note 2/3)")
    h.at("kitchen", "ChairDining", -4.7, -6.6, yaw=90)
    h.wall("kitchen", "S", "KitchenSinkUnit", at=-2.4)
    h.wall("kitchen", "E", "Fridge", at=-4.5)
    # pantry/back passage
    h.wall("pantry", "E", "SteelShelves2", at=-5.0)
    h.wall("pantry", "N", "CabinetPainted", at=2.45, note="walk-in coat cupboard"); hide(h, h.props[-1], "coat cupboard")
    # bed1
    bed(h, "bed1", "E", "Bed", 1.6, label="bed")
    w = h.wall("bed1", "S", "Wardrobe", at=1.9); hide(h, w, "wardrobe")
    h.at("bed1", "Chair", 1.0, -1.6, yaw=45)
    # bed2
    bed(h, "bed2", "S", "BedIron", -4.0, stands=("Nightstand",), stand_side="left", label="lodger's bed")
    h.wall("bed2", "E", "ChestOfDrawers", at=-4.5)
    shrine(h, "landing", "E", -2.0)
    # bath
    bt = h.wall("bath", "E", "BathtubPhoto", at=-6.0); hide(h, bt, "behind the bath curtain")
    h.at("bath", "BathCurtainRail", 2.8, -6.0, yaw=270, role="allow-overlap")
    h.wall("bath", "N", "Sink", at=1.0)
    h.hung("bath", "N", "Mirror", at=1.0, y=1.55, role="mirror")
    h.wall("bath", "W", "Toilet", at=-8.0)
    # attic
    h.wall("attic", "S", "SchoolDesk", at=-1.5); h.wall("attic", "S", "SchoolDesk", at=0.0); h.wall("attic", "S", "SchoolDesk", at=1.5)
    h.wall("attic", "W", "Chalkboard", at=1.0, note="'alphabet cipher' poster pinned to an old easel board (A=1 ... drives the code)")
    note(h, h.props[-1], "cipher poster (note 3/3)")
    item(h, h.wall("attic", "E", "CardboardBox", at=0.0), "box of old exercise books")
    return h


# ============================================================================================ H04 Weaver's House
def h04():
    h = House(4, "H04", "Weaver's House", "Ashdown", "Spool of red thread", "Sequence", "Red", 1, 11)
    h.traits = dict(archetype="Weaver's cottage: long stone house whose whole top floor is a loom shop lit by a continuous band of windows",
                    silhouette="Long and low (14 m) with a RIBBON of 8 small windows across the upper floor front, stone-slate roof, "
                               "one chimney at the WEST end, an outside stone stair to a loading door at the east gable.")
    levels(h, attic=True, g=2.7, f=2.6)
    h.wing("main", (-7.0, -3.2, 7.0, 3.2), ["G", "F", "A"], "PlasterStone (half-rendered rubble, render falling off), "
           "SandstoneBlocks quoins + long stone mullions in the upper window band", T_STONE,
           "Gable roof 35 deg, ridge along x at 7.9 m, RoofRedSlate (heavy stone slates, graded courses), moss")
    h.wing("outshut", (-7.0, -5.6, -1.0, -3.2), ["G"], "Rubble", 0.45, "Catslide continuation of the main roof down to 2.2 m")
    G, F = "G", "F"
    h.room("living", "House-place (living)", G, (-7.0, -3.2, -1.5, 3.2), "living", "FloorFlagstone", "Whitewash", light="pendant + oil lamp")
    h.room("hall", "Cross passage", G, (-1.5, -3.2, 0.5, 3.2), "hall", "FloorFlagstone", "Whitewash")
    h.room("parlour", "Parlour / dye room", G, (0.5, -3.2, 7.0, 3.2), "utility", "FloorBrick", "PlasterWorn with dye splashes")
    h.room("kitchen", "Back kitchen (outshut)", G, (-7.0, -5.6, -3.6, -3.2), "kitchen", "FloorQuarryTile", "Whitewash")
    h.room("bath", "Bathroom (outshut)", G, (-3.6, -5.6, -1.0, -3.2), "bathroom", "FloorBathTile", "TileWallWhite")
    h.room("garret", "Garret (attic over the bedrooms)", "A", (-7.0, -3.2, -1.5, 3.2), "attic", "FloorOldWood2", "RawPlankWall",
           note="reached by a ladder from bed1; 1.9 m headroom at the ridge only")
    h.room("loomshop", "LOOM SHOP (PUZZLE)", F, (-1.5, -3.2, 7.0, 3.2), "workshop", "FloorOldWood2", "Whitewash, warp threads strung on pegs",
           light="daylight band; 2 CagedLight at night")
    h.room("bed1", "Bedroom (Ashdowns)", F, (-7.0, 0.0, -1.5, 3.2), "bedroom", "FloorOldWood1", "Wallpaper1")
    h.room("bed2", "Bedroom (apprentice)", F, (-7.0, -3.2, -1.5, 0.0), "bedroom", "FloorOldWood1", "PlasterWorn")
    h.door("d_front", G, "hall", "N", -0.5, 0.95, role="front", kind="boarded red door, cross-passage", colour="Red")
    h.door("d_back", G, "hall", "S", -1.0, 0.9, role="back", kind="cross-passage back door (nearly aligned with the front)")
    h.door("d_liv", G, "hall", "W", 1.4, 0.85, room_b="living")
    h.door("d_par", G, "hall", "E", 1.4, 0.85, room_b="parlour")
    h.door("d_kit", G, "living", "S", -5.0, 0.8, room_b="kitchen")
    h.door("d_bath", G, "kitchen", "E", -3.9, 0.75, room_b="bath")
    h.door("d_loading", F, "loomshop", "E", 1.0, 1.1, role="loading", kind="loading door 1.1 x 1.8 on the gable, reached by the outside stone stair")
    h.door("d_b1", F, "loomshop", "W", 1.6, 0.8, room_b="bed1")
    h.door("d_b2", F, "loomshop", "W", -0.6, 0.8, room_b="bed2")
    for i, x in enumerate((-0.6, 0.6, 1.8, 3.0, 4.2, 5.4)):
        h.window(f"w_band{i}", "loomshop", "N", x, 0.8, 0.9, sill=1.0, style="weaver's light (one of the window band), stone mullions 0.15")
    h.window("w_band_b1", "bed1", "N", -4.2, 0.8, 0.9, sill=1.0, style="window band continues")
    h.window("w_liv", "living", "N", -4.2, 1.1, 1.1, weak=True, style="casement, sagging")
    h.window("w_par", "parlour", "N", 3.8, 1.1, 1.1, weak=True, style="casement")
    h.window("w_par2", "parlour", "S", 3.8, 1.0, 1.0, style="casement")
    h.window("w_k", "kitchen", "W", -4.4, 0.8, 0.8, style="small")
    h.stair("s1", "hall", (-0.45, -2.8, 0.45, 0.6), G, F, "S", kind="straight stone-and-timber stair in the passage")
    h.stair("s1u", "loomshop", (-0.45, -2.8, 0.45, 0.6), G, F, "S", kind="void")
    h.stair("s2", "bed1", (-2.4, 0.1, -1.7, 1.0), F, "A", "W", kind="ladder to the garret hatch")
    h.chimney(-7.0, 0.0, w=1.2, d=1.0, serves="living fire = THE HEARTH", pots=1)
    # living
    hr = h.wall("living", "W", "Hearth", at=0.0, role="hearth", note="open fire with a crane and kettle, stone surround")
    h.on(hr, "MantelClock", note="stopped at 11:00"); h.on(hr, "FrameStanding1", dx=0.6, role="family-photo")
    item(h, hr, "mantel")
    h.at("living", "RockingChair", -5.0, 1.2, yaw=240)
    h.wall("living", "N", "SofaPainted", at=-2.6)
    lt = h.at("living", "TableGallinera", -3.6, -1.0, yaw=0)
    h.on(lt, "OilLamp"); note(h, lt, "pattern draft (note 1/3)")
    h.at("living", "ChairGallinera", -3.6, -1.75, yaw=0)
    rt = h.wall("living", "S", "SideTable", at=-2.4)
    h.on(rt, "RotaryPhone", role="phone"); h.on(rt, "Radio", dx=0.2, role="radio")
    dz = h.wall("living", "S", "Dresser", at=-3.6)
    item(h, dz, "dresser")
    h.at("living", "SpinningWheel", -6.0, -2.1, yaw=45)
    # hall
    h.hung("hall", "E", "Mirror", at=2.6, y=1.6, note="small mirror")
    h.hung("hall", "W", "FuseBox", at=-2.6, y=1.7, role="fuse-box")
    # parlour / dye room
    for x in (1.6, 3.0):
        h.at("parlour", "Barrel2", x, -2.2, note="dye vat (red stain)")
    h.wall("parlour", "E", "SteelShelves", at=0.0, note="dye jars, mordant tins")
    h.wall("parlour", "N", "Wicker2", at=1.4); h.wall("parlour", "N", "Wicker1", at=5.6)
    h.at("parlour", "WashingLine", 3.8, 0.4, yaw=0, y=0.6, role="allow-overlap", note="indoor drying lines with red skeins at 2.0 m")
    h.wall("parlour", "S", "BarrelWorn", at=5.9); hide(h, h.props[-1], "inside the empty dye barrel", crouch=True)
    # kitchen + bath
    h.wall("kitchen", "S", "Cooker", at=-6.1)
    ks = h.wall("kitchen", "S", "KitchenSinkUnit", at=-4.8)
    h.on(ks, "Jug", dx=0.3)
    h.wall("bath", "S", "Bathtub", at=-2.5, scale=0.85)
    h.wall("bath", "E", "Toilet", at=-4.2)
    h.hung("bath", "N", "Mirror", at=-1.9, y=1.55, role="mirror")
    h.wall("bath", "N", "Sink", at=-1.9)
    # loom shop PUZZLE
    loom = h.at("loomshop", "LoomDobby", 3.2, 0.0, yaw=180, role="puzzle",
                note="THE LOOM, centre of the room: rethread it to match the hidden pattern (SequencePuzzle 6 symbols = 6 thread colours)")
    puzzle(h, loom, "Rethread the loom: pattern pieces hidden in 3 places (bed1 sampler, parlour dye book, living mantel photo)")
    h.at("loomshop", "LoomFrame", 0.8, 1.6, yaw=180, note="second, broken loom (dressing)")
    ch = h.wall("loomshop", "E", "ChestWorn", at=-1.8, note="PART CONTAINER: thread chest")
    container(h, ch, "Spool of red thread")
    h.wall("loomshop", "S", "ShelvesPainted", at=5.5, note="bobbins in pigeon rows")
    h.wall("loomshop", "S", "Wicker1", at=2.0)
    item(h, h.wall("loomshop", "S", "TableSmall", at=4.2), "winding table")
    h.wall("loomshop", "S", "Sacks", at=1.0); hide(h, h.props[-1], "behind the wool sacks", crouch=True)
    shrine(h, "loomshop", "S", 6.3)
    # bedrooms
    bed(h, "bed1", "S", "Bed", -5.4, stand_side="right", label="bed")
    h.wall("bed1", "N", "ChestOfDrawers", at=-3.0)
    s = h.hung("bed1", "N", "FrameHanging2", at=-2.6, y=1.6, role="family-photo", note="embroidered sampler: pattern piece 1")
    w = h.wall("bed2", "S", "Wardrobe", at=-5.6); hide(h, w, "wardrobe")
    bed(h, "bed2", "W", "BedIron", -1.2, stands=("",), label="apprentice's bed", hiding=False)
    note(h, h.wall("bed2", "E", "TableSmall", at=-2.2), "apprentice's notebook (note 2/3)")
    note(h, h.wall("loomshop", "S", "CardboardBox", at=3.2), "order book (note 3/3)")
    h.wall("garret", "W", "CellarCrates", at=0.0); hide(h, h.props[-1], "behind the trunks in the garret", crouch=True)
    h.wall("garret", "N", "Suitcase", at=-4.0)
    h.window("w_garret", "garret", "W", 0.0, 0.5, 0.5, sill=0.9, style="tiny gable light")
    return h
