from core import House
from kit import *


# ============================================================================================ H10 Photographer's House
def h10():
    h = House(10, "H10", "Photographer's House", "Lisle", "Glass negative", "CodeLock", "Black", 2, 12)
    h.traits = dict(archetype="Edwardian red-brick double-fronted shop-house with a Victorian glasshouse studio behind",
                    silhouette="Symmetrical red brick with a recessed black door between two windows (left one a shop window "
                               "lettered 'LISLE - PORTRAITS'), a gable chimney at each end, and behind it a long glass-roofed "
                               "studio glinting over the garden wall.")
    levels(h, cellar=True, g=2.9, f=2.7)
    h.wing("main", (-5.0, -4.0, 5.0, 4.0), ["C", "G", "F"], "BrickRed (Flemish bond), BrickDark plinth, SandstoneBlocks lintels "
           "with keystones", T_BRICK, "Gable roof 40 deg, ridge along x at 8.6 m, RoofSlateGrey; chimneys on both gables")
    h.wing("studio", (-4.0, -10.0, 4.0, -4.0), ["G"], "Low brick wall 0.9 m (BrickRed) then timber-and-glass walls (half the "
           "panes painted white for diffusion, several broken)", 0.15, "Glass double-pitch roof on iron glazing bars, ridge 4.4 m; "
           "blinds on cords under the glass")
    G, F, C = "G", "F", "C"
    h.room("reception", "Reception (portrait shop)", G, (-5.0, 0.0, -1.0, 4.0), "shop", "FloorParquet", "Wallpaper2 (dark red) above PanelDark")
    h.room("hall", "Hall", G, (-1.0, -4.0, 1.0, 4.0), "hall", "FloorWornTile", "Wallpaper2C")
    h.room("parlour", "Parlour", G, (1.0, 0.0, 5.0, 4.0), "living", "Carpet70s", "WallpaperQuatrefoil")
    h.room("kitchen", "Kitchen", G, (-5.0, -4.0, -1.0, 0.0), "kitchen", "FloorQuarryTile", "Whitewash")
    h.room("print", "Print & retouching room", G, (1.0, -4.0, 5.0, 0.0), "workshop", "FloorOldWood2", "PlasterWorn, prints pinned everywhere")
    h.room("studio", "Glass studio", G, (-4.0, -10.0, 4.0, -4.0), "studio", "FloorOldWood1", "glass + painted backdrops", light="daylight; 2 StudioLamp")
    h.room("bed1", "Lisles' bedroom", F, (-5.0, 0.0, -1.0, 4.0), "bedroom", "CarpetWorn", "Wallpaper1")
    h.room("landing", "Landing", F, (-1.0, -4.0, 1.0, 4.0), "landing", "Carpet70sB", "Wallpaper2C")
    h.room("bed2", "Daughter's bedroom", F, (1.0, 0.0, 5.0, 4.0), "bedroom", "Carpet70sB", "Wallpaper1B")
    h.room("bath", "Bathroom", F, (-5.0, -4.0, -1.0, 0.0), "bathroom", "FloorBathTile", "TileWallWhite")
    h.room("store", "Negative store", F, (1.0, -4.0, 5.0, 0.0), "storage", "FloorOldWood2", "shelving floor to ceiling")
    h.room("cellar", "Darkroom cellar (PUZZLE)", C, (-5.0, -4.0, 5.0, 0.0), "cellar", "FloorConcrete", "painted matt black",
           light="RED SAFELIGHT only (Safelight); white bulb ruins the print")
    h.door("d_front", G, "hall", "N", -0.4, 0.9, role="front", kind="black door with etched glass 'LISLE', in a tiled recess", colour="Black")
    h.door("d_back", G, "kitchen", "W", -1.0, 0.85, role="back")
    h.door("d_rec", G, "hall", "W", 3.0, 0.85, room_b="reception")
    h.door("d_par", G, "hall", "E", 3.0, 0.85, room_b="parlour")
    h.door("d_kit", G, "hall", "W", -2.0, 0.85, room_b="kitchen")
    h.door("d_print", G, "hall", "E", -3.2, 0.85, room_b="print")
    h.door("d_studio", G, "hall", "S", -0.4, 0.9, room_b="studio", kind="glazed double door into the studio")
    h.door("d_b1", F, "landing", "W", 3.0, 0.8, room_b="bed1")
    h.door("d_b2", F, "landing", "E", 3.0, 0.8, room_b="bed2")
    h.door("d_bath", F, "landing", "W", -2.0, 0.75, room_b="bath")
    h.door("d_store", F, "landing", "E", -3.2, 0.75, room_b="store")
    h.window("w_shop", "reception", "N", -3.0, 2.4, 1.7, sill=0.6, style="shop window, gilt letters 'LISLE - PORTRAITS', framed portraits on easels")
    h.window("w_par", "parlour", "N", 3.0, 1.4, 1.5, style="sash", curtains="FabricTerlenka")
    h.window("w_k", "kitchen", "W", -3.0, 0.9, 1.0, weak=True, style="sash, cracked")
    h.window("w_print", "print", "E", -2.0, 1.0, 1.2, weak=True, style="sash, loose")
    h.window("w_st1", "studio", "W", -7.0, 3.0, 1.8, sill=0.9, style="glass wall bay (studio)")
    h.window("w_st2", "studio", "E", -7.0, 3.0, 1.8, sill=0.9, style="glass wall bay (studio), 3 panes broken")
    h.window("w_b1", "bed1", "N", -3.0, 1.2, 1.4, style="sash")
    h.window("w_b2", "bed2", "N", 3.0, 1.2, 1.4, style="sash")
    h.window("w_bath", "bath", "W", -2.0, 0.7, 0.8, sill=1.3, style="frosted")
    h.stair("s1", "hall", (0.0, -2.4, 0.9, 1.1), G, F, "S", note="15 risers, Carpet70sB runner")
    h.stair("s1u", "landing", (0.0, -2.4, 0.9, 1.1), G, F, "S", kind="void")
    h.stair("s2", "print", (3.7, -3.6, 4.6, -0.1), C, G, "N", kind="cellar stair along the print room's east wall, light-tight curtain at the top")
    h.stair("s2c", "cellar", (3.7, -3.6, 4.6, -0.1), C, G, "N", kind="arrives")
    h.chimney(-5.0, -2.6, serves="kitchen range = THE HEARTH", pots=2)
    h.chimney(5.0, 2.0, serves="parlour fire", pots=2)
    # reception
    h.wall("reception", "S", "Counter", at=-3.0, scale=0.75, note="small counter with an order book")
    h.on(h.props[-1], "VintageCamera", dx=0.5)
    note(h, h.props[-2], "order book: 'collect the Abney plate - do NOT print it' (note 1/3)")
    for i, z in enumerate((3.0, 1.0)):
        h.hung("reception", "W", "FrameFancy1" if i else "FrameFancy2", at=z, y=1.6, role="family-photo" if i == 0 else "",
               note="portrait of a village family" if i else "FAMILY PHOTO: the Lisles in the studio, 1970")
    h.at("reception", "ArmchairGreen", -2.0, 2.0, yaw=270)
    # parlour
    hr = h.wall("parlour", "E", "Hearth", at=2.0, role="hearth-decor", note="tiled fireplace (decorative)")
    h.on(hr, "MantelClock", note="stopped at 12:00")
    h.wall("parlour", "S", "Sofa3", at=3.0)
    tv = h.wall("parlour", "W", "Cabinet70s", at=1.2, note="radiogram")
    h.on(tv, "TV2", role="tv")
    pt = h.wall("parlour", "N", "SideTable", at=4.2)
    h.on(pt, "RotaryPhone", role="phone")
    h.at("parlour", "CoffeeTableRound", 3.0, 2.0)
    # kitchen - THE HEARTH
    rg = h.wall("kitchen", "W", "Hearth", at=-2.6, role="hearth", note="range in the west-gable flue = THE HEARTH")
    item(h, rg, "range mantel")
    kt = h.at("kitchen", "TablePainted", -2.6, -1.5, yaw=0)
    h.at("kitchen", "ChairPainted1", -2.6, -0.85, yaw=180)
    h.wall("kitchen", "S", "KitchenUnit", at=-2.0)
    h.wall("kitchen", "S", "KitchenSinkUnit", at=-3.2)
    h.hung("kitchen", "E", "FuseBox", at=-0.8, y=1.7, role="fuse-box")
    # print room
    h.wall("print", "N", "TableWood3", at=2.4, note="retouching desk with a light box")
    h.on(h.props[-1], "DeskLamp", dx=0.7); h.on(h.props[-2], "Magnifier", dx=-0.2)
    sf = h.wall("print", "W", "IronSafe", at=-1.8, role="puzzle container", note="NEGATIVE SAFE: 4 code dials")
    puzzle(h, sf, "CodeLockPuzzle: develop the hidden glass plate in the cellar darkroom (safelight on, white light off) - the print "
                  "shows the studio wall clock; the code is written on the blackboard behind the sitters")
    container(h, sf, "Glass negative in a paper sleeve")
    h.wall("print", "S", "DrawerCabinet", at=2.6, note="print drawers")
    item(h, h.props[-1], "print drawers")
    # studio
    h.wall("studio", "S", "Backdrop", at=0.0)
    h.at("studio", "Chair", 0.0, -8.6, yaw=0, note="posing chair")
    h.at("studio", "StudioLamp", -1.8, -7.2); h.at("studio", "StudioLamp", 1.8, -7.2)
    h.at("studio", "VintageCamera", 0.0, -5.6, y=1.3, role="allow-overlap", note="plate camera on a wooden tripod (tripod = 3 thin cylinders)")
    h.wall("studio", "W", "Commode", at=-5.0, note="props drawer: hats, gloves, a doll")
    h.wall("studio", "E", "CardboardBox", at=-5.0)
    h.wall("studio", "E", "BenchPainted", at=-9.0, note="family bench for group portraits")
    hide_at(h, "studio", -3.4, -9.6, "behind the rolled backdrops in the SW corner")
    # cellar darkroom
    h.wall("cellar", "N", "Enlarger", at=-3.5)
    tr = h.wall("cellar", "N", "DarkroomTrays", at=-1.5, note="developer / stop / fix trays + tongs + timer")
    h.hung("cellar", "N", "Safelight", at=-2.5, y=2.0)
    h.wall("cellar", "S", "SteelShelves", at=2.0, note="chemicals, boxes of paper (DO NOT OPEN IN LIGHT)")
    note(h, h.wall("cellar", "W", "TableSmall", at=-2.0), "Mr Lisle's darkroom rules card (note 2/3)")
    h.wall("cellar", "S", "CellarCrates", at=0.0); hide(h, h.props[-1], "behind the crates in the dark", crouch=True)
    # bed1
    bed(h, "bed1", "W", "Bed", 2.0, label="bed")
    w = h.wall("bed1", "S", "Wardrobe", at=-2.6); hide(h, w, "wardrobe")
    # bed2
    bed(h, "bed2", "E", "BedIron", 2.0, stands=("NightstandPainted",), stand_side="left", label="daughter's bed", hiding=False)
    note(h, h.wall("bed2", "S", "TableSmall", at=3.6), "daughter's photo album with captions (note 3/3)")
    # bath
    h.wall("bath", "S", "Bathtub", at=-3.2)
    hide(h, h.props[-1], "behind the bath curtain")
    h.wall("bath", "N", "Sink", at=-3.2)
    h.hung("bath", "N", "Mirror", at=-3.2, y=1.55, role="mirror")
    h.wall("bath", "W", "Toilet", at=-0.8)
    # store
    h.wall("store", "S", "SteelShelves", at=3.0, note="glass plates in grooved racks")
    h.wall("store", "N", "SteelShelves2", at=3.0)
    item(h, h.wall("store", "E", "CardboardBox", at=-2.0), "box of glass plates")
    shrine(h, "landing", "N", 0.4)
    return h


# ============================================================================================ H11 Priest's House
def h11():
    h = House(11, "H11", "Priest's House", "Ambrose", "Altar cloth", "CodeLock", "White", 3, 1)
    h.traits = dict(archetype="Stone presbytery with a private oratory wing projecting toward the lane",
                    silhouette="Grey stone, two storeys, THREE chimneys (west gable, a big external stack on the back wall, "
                               "and the front stack of the study); an oratory wing to the east with a steep roof, a tiny "
                               "bell-cote and a pointed window; white door under a stone canopy; a Calvary cross in the garden.")
    levels(h, attic=True, g=2.9, f=2.6)
    h.wing("main", (-6.0, -4.0, 2.0, 4.0), ["G", "F", "A"], "StoneBlock with SandstoneBlocks quoins; lime-pointed", T_STONE,
           "Gable roof 45 deg, ridge along x at 8.9 m, RoofSlateGrey")
    h.wing("oratory", (2.0, -1.0, 6.0, 6.0), ["G"], "StoneBlock, buttresses at the corners", T_STONE,
           "Steep 55 deg gable roof, ridge along z at 7.2 m, RoofSlateGrey; bell-cote (Bell scaled 0.35) at the north gable apex")
    G, F, A = "G", "F", "A"
    h.room("sitting", "Sitting room", G, (-6.0, 0.0, -2.5, 4.0), "living", "Carpet70sB", "Wallpaper2C (sage)")
    h.room("hall", "Hall", G, (-2.5, -4.0, -0.5, 4.0), "hall", "FloorFlagstone", "PlasterWorn")
    h.room("study", "Study", G, (-0.5, 0.0, 2.0, 4.0), "study", "FloorParquet", "PanelDark")
    h.room("kitchen", "Kitchen (housekeeper's)", G, (-6.0, -4.0, -2.5, 0.0), "kitchen", "FloorFlagstone", "Whitewash")
    h.room("scull", "Scullery & lavatory", G, (-0.5, -4.0, 2.0, 0.0), "utility", "FloorBrick", "Whitewash")
    h.room("oratory", "Oratory (PUZZLE)", G, (2.0, -1.0, 6.0, 6.0), "chapel", "FloorFlagstone", "Whitewash, stations of the cross",
           ceiling="open timber roof to 6 m", light="2 hanging lamps (WoodenLantern) + candles")
    h.room("bed1", "Father Ambrose's bedroom", F, (-6.0, 0.0, -2.5, 4.0), "bedroom", "FloorDarkPlanks", "Whitewash, crucifix")
    h.room("landing", "Landing", F, (-2.5, -4.0, -0.5, 4.0), "landing", "FloorDarkPlanks", "PlasterWorn")
    h.room("bed2", "Housekeeper's room", F, (-0.5, 0.0, 2.0, 4.0), "bedroom", "FloorOldWood1", "Wallpaper1B")
    h.room("bath", "Bathroom", F, (-6.0, -4.0, -2.5, 0.0), "bathroom", "FloorBathTile", "TileWallWhite")
    h.room("box", "Box room (vestments)", F, (-0.5, -4.0, 2.0, 0.0), "storage", "FloorOldWood2", "PlasterWorn")
    h.room("attic", "Attic", A, (-6.0, -4.0, 2.0, 4.0), "attic", "FloorOldWood2", "RawPlankWall")
    h.door("d_front", G, "hall", "N", -1.5, 1.0, role="front", kind="white boarded door with a stone canopy over it", colour="White")
    h.door("d_back", G, "hall", "S", -1.0, 0.85, role="back")
    h.door("d_sit", G, "hall", "W", 3.0, 0.85, room_b="sitting")
    h.door("d_kit", G, "hall", "W", -2.8, 0.85, room_b="kitchen")
    h.door("d_study", G, "hall", "E", 2.5, 0.85, room_b="study")
    h.door("d_scull", G, "hall", "E", -2.0, 0.8, room_b="scull")
    h.door("d_orat", G, "study", "E", 2.0, 0.9, room_b="oratory", kind="pointed oak door with a holy-water stoup beside it")
    h.door("d_orat_ext", G, "oratory", "N", 4.0, 1.0, role="side", kind="oratory west door to the lane (locked by day)")
    h.door("d_b1", F, "landing", "W", 3.0, 0.8, room_b="bed1")
    h.door("d_b2", F, "landing", "E", 2.5, 0.8, room_b="bed2")
    h.door("d_bath", F, "landing", "W", -2.8, 0.75, room_b="bath")
    h.door("d_box", F, "landing", "E", -2.0, 0.75, room_b="box")
    h.window("w_sit", "sitting", "N", -4.2, 1.2, 1.5, style="sash with stone mullion")
    h.window("w_study", "study", "N", 0.75, 0.9, 1.4, style="sash")
    h.window("w_k", "kitchen", "W", -2.0, 1.0, 1.1, weak=True, style="sash, rotten sill")
    h.window("w_sc", "scull", "S", 0.75, 0.8, 0.9, weak=True, style="small casement")
    h.window("w_or1", "oratory", "E", 2.5, 0.9, 2.4, sill=1.0, style="pointed lancet with plain coloured glass (red/blue)")
    h.window("w_or2", "oratory", "S", 4.0, 0.7, 1.6, sill=1.2, style="small lancet behind the altar")
    h.window("w_b1", "bed1", "N", -4.2, 1.0, 1.2, style="sash")
    h.window("w_b2", "bed2", "N", 0.75, 0.8, 1.1, style="sash")
    h.window("w_bath", "bath", "W", -2.0, 0.7, 0.8, sill=1.3, style="frosted")
    h.stair("s1", "hall", (-2.4, -1.5, -1.5, 2.0), G, F, "S", note="stone-and-oak stair, 15 risers")
    h.stair("s1u", "landing", (-2.4, -1.5, -1.5, 2.0), G, F, "S", kind="void")
    h.stair("s2", "landing", (-1.3, -3.4, -0.6, -2.6), F, A, "W", kind="loft ladder")
    h.chimney(-6.0, 2.0, serves="sitting-room fire", pots=1)
    h.chimney(-4.4, -4.0, w=1.2, d=0.8, serves="kitchen range = THE HEARTH (external stack on the back wall)", pots=2)
    h.chimney(0.75, 4.0, w=0.9, d=0.6, serves="study fire (external front stack)", pots=1)
    # oratory PUZZLE
    alt = h.wall("oratory", "S", "SmallAltar", at=4.0, note="altar with candlesticks; the frontal hides the cupboard")
    h.on(alt, "Candleholders", dx=-0.5); h.on(alt, "Candleholders", dx=0.5)
    item(h, alt, "altar (between the candlesticks)")
    hb = h.hung("oratory", "W", "HymnBoard", at=4.6, y=1.8, role="puzzle", note="HYMN BOARD: 3 rows of number cards")
    puzzle(h, hb, "CodeLockPuzzle reskinned: the hymn board shows 3 hymn numbers; the hymn book index (study) turns each hymn into "
                  "a page number; first digit of each + the psalm number on the altar card = 4-digit vestment-cupboard code")
    vc = h.wall("oratory", "E", "CabinetVintage", at=0.2, note="VESTMENT CUPBOARD with a 4-dial lock = PART CONTAINER")
    container(h, vc, "Altar cloth, folded, red embroidered")
    for z in (1.6, 3.2):
        h.at("oratory", "ChurchPew", 4.0, z, yaw=180, scale=0.75)
    h.at("oratory", "Lectern", 2.8, 0.6, yaw=90)
    h.wall("oratory", "W", "GothicStatue", at=5.2, scale=0.7, note="statue of the village saint, face worn smooth")
    hide_at(h, "oratory", 5.2, 5.0, "behind the curtain of the confessional corner (NE)")
    # study
    dk = h.at("study", "OfficeDesk", 0.75, 0.6, yaw=0, scale=0.8)
    h.on(dk, "BooksDeco", dx=-0.3); h.on(dk, "DeskLamp", dx=0.3)
    note(h, dk, "HYMN BOOK INDEX page, torn out (note 1/3)")
    h.at("study", "Chair", 0.75, 1.3, yaw=180)
    h.wall("study", "N", "Shelf", at=0.75)
    pt = h.hung("study", "W", "WallClock", at=1.0, y=2.0)
    # sitting
    hr = h.wall("sitting", "W", "Hearth", at=2.0, role="hearth-decor", note="stone fireplace")
    h.on(hr, "MantelClock", note="stopped at 1:00"); h.on(hr, "FrameStanding1", dx=-0.6, role="family-photo", note="Fr Ambrose's ordination, 1949")
    item(h, hr, "sitting-room mantel")
    h.wall("sitting", "S", "Sofa3", at=-4.2)
    h.at("sitting", "Armchair", -4.2, 2.2, yaw=200)
    st = h.at("sitting", "SideTable", -3.0, 1.4)
    h.on(st, "RotaryPhone", role="phone"); h.on(st, "Radio", dx=0.15, role="radio")
    # kitchen
    rg = h.wall("kitchen", "S", "Hearth", at=-4.4, role="hearth", note="range in the big back-wall stack = THE HEARTH")
    item(h, rg, "range shelf")
    h.wall("kitchen", "N", "Dresser", at=-4.2)
    kt = h.at("kitchen", "TableWood2", -4.2, -1.8, yaw=0)
    note(h, kt, "housekeeper's list: 'Sunday: 23, 104, 7' (note 2/3)")
    h.at("kitchen", "ChairDining", -4.2, -1.0, yaw=180)
    # scullery
    h.wall("scull", "S", "Copper", at=0.9)
    h.wall("scull", "N", "KitchenSinkUnit", at=0.3, note="stone sink")
    h.wall("scull", "E", "Toilet", at=-1.0)
    h.hung("scull", "E", "FuseBox", at=-2.6, y=1.7, role="fuse-box")
    # bed1
    bed(h, "bed1", "W", "BedIron", 2.0, stands=("Nightstand",), stand_side="right", label="narrow iron bed")
    w = h.wall("bed1", "S", "Wardrobe", at=-4.2); hide(h, w, "wardrobe")
    # bed2
    bed(h, "bed2", "S", "BedIron", 1.0, stands=("",), label="housekeeper's bed", hiding=False)
    # bath
    h.wall("bath", "S", "BathtubPhoto", at=-4.4); hide(h, h.props[-1], "behind the bath curtain")
    h.wall("bath", "N", "Sink", at=-4.6)
    h.hung("bath", "N", "Mirror", at=-4.6, y=1.55, role="mirror")
    # box room
    h.wall("box", "S", "CabinetPainted", at=0.75, note="cassocks on hooks"); hide(h, h.props[-1], "vestment cupboard (walk-in)")
    note(h, h.at("box", "TableSmall", 0.9, -1.0), "parish newsletter (note 3/3)")
    # attic
    h.wall("attic", "W", "ChestWorn", at=0.0)
    h.wall("attic", "N", "Suitcase", at=-3.0)
    shrine(h, "oratory", "E", 4.8)
    return h


# ============================================================================================ H12 Postmaster's House
def h12():
    h = House(12, "H12", "Postmaster's House", "Dunn", "Sealed letter", "Sequence", "Green", 2, 3)
    h.traits = dict(archetype="1920s Arts-and-Crafts brick house with a twin-gabled front and a flat-roofed sorting room",
                    silhouette="Two identical front gables side by side (an 'M' roof), red brick ground floor with clay "
                               "TILE-HUNG upper floor, a green door in a little gabled porch, one chimney per gable end, and a "
                               "flat-roofed sorting room on the east with a red pillar box and an enamel 'POST OFFICE' sign.")
    levels(h, attic=True, g=2.8, f=2.6)
    h.wing("main", (-5.0, -4.0, 5.0, 4.0), ["G", "F", "A"], "Ground floor BrickRed; first floor tile-hung (RoofClay2 used as "
           "vertical hanging tiles, with a band of fish-scale tiles)", T_BRICK,
           "M-roof: two parallel gables with ridges along z at x=-2.5 and x=+2.5, 50 deg, RoofClay2, central valley gutter")
    h.wing("sorting", (5.0, -4.0, 9.0, 1.0), ["G"], "BrickRed, enamel sign", 0.25, "Flat felt roof with a timber fascia")
    h.wing("porch", (-1.2, 4.0, 0.8, 5.2), ["G"], "Timber porch on a brick plinth", 0.1, "Small gabled porch roof, RoofClay2")
    G, F, A = "G", "F", "A"
    h.room("porch", "Porch", G, (-1.2, 4.0, 0.8, 5.2), "porch", "FloorWornTile", "open")
    h.room("front", "Front room", G, (-5.0, 0.0, -1.0, 4.0), "living", "Carpet70s", "WallpaperJacquard (tinted green)")
    h.room("hall", "Hall", G, (-1.0, -4.0, 1.0, 4.0), "hall", "FloorWornTile", "Wallpaper2")
    h.room("dining", "Dining room", G, (1.0, 0.0, 5.0, 4.0), "dining", "FloorParquet", "Wallpaper2C")
    h.room("kitchen", "Kitchen", G, (-5.0, -4.0, -1.0, 0.0), "kitchen", "FloorQuarryTile", "TileWallWhite to 1.2 m")
    h.room("office", "Postmaster's office", G, (1.0, -4.0, 5.0, 0.0), "study", "FloorOldWood1", "PanelDark")
    h.room("sorting", "Sorting room (PUZZLE)", G, (5.0, -4.0, 9.0, 1.0), "sorting", "FloorConcrete", "painted GPO cream/green")
    h.room("bedW", "Dunns' bedroom", F, (-5.0, 0.0, -1.0, 4.0), "bedroom", "CarpetWorn", "Wallpaper1")
    h.room("landing", "Landing", F, (-1.0, -4.0, 1.0, 4.0), "landing", "Carpet70sB", "Wallpaper2")
    h.room("bedE", "Spare bedroom", F, (1.0, 0.0, 5.0, 4.0), "bedroom", "FloorOldWood1", "Wallpaper1B")
    h.room("bath", "Bathroom", F, (-5.0, -4.0, -1.0, 0.0), "bathroom", "FloorBathTile", "TileWallWhite (pink border)")
    h.room("bed3", "Sewing room", F, (1.0, -4.0, 5.0, 0.0), "bedroom", "FloorOldWood2", "Wallpaper1")
    h.room("attic", "Attic (dead-letter store)", A, (-5.0, -4.0, 5.0, 4.0), "attic", "FloorOldWood2", "RawPlankWall")
    h.door("d_porch", G, "porch", "N", -0.2, 1.0, role="front", kind="porch opening", colour="Green")
    h.door("d_front", G, "porch", "S", -0.4, 0.9, room_b="hall", kind="green door with a brass letter flap and a bell push")
    h.door("d_back", G, "hall", "S", -0.4, 0.85, role="back")
    h.door("d_fr", G, "hall", "W", 3.0, 0.85, room_b="front")
    h.door("d_din", G, "hall", "E", 3.0, 0.85, room_b="dining")
    h.door("d_kit", G, "hall", "W", -2.0, 0.85, room_b="kitchen")
    h.door("d_off", G, "hall", "E", -3.2, 0.85, room_b="office")
    h.door("d_sort", G, "office", "E", -2.0, 0.85, room_b="sorting", kind="door with a brass 'STAFF ONLY' plate")
    h.door("d_post", G, "sorting", "N", 7.0, 1.0, role="post", kind="public counter door (glazed) under the 'POST OFFICE' sign")
    h.door("d_bW", F, "landing", "W", 3.0, 0.8, room_b="bedW")
    h.door("d_bE", F, "landing", "E", 3.0, 0.8, room_b="bedE")
    h.door("d_bath", F, "landing", "W", -2.0, 0.75, room_b="bath")
    h.door("d_b3", F, "landing", "E", -3.2, 0.75, room_b="bed3")
    h.window("w_fr", "front", "N", -3.0, 1.6, 1.4, style="leaded casement, 3 lights")
    h.window("w_din", "dining", "N", 3.0, 1.6, 1.4, style="leaded casement, 3 lights")
    h.window("w_k", "kitchen", "S", -3.0, 1.1, 1.1, weak=True, style="casement, cracked")
    h.window("w_off", "office", "S", 3.0, 1.0, 1.1, weak=True, style="casement")
    h.window("w_sort", "sorting", "E", -1.5, 1.4, 1.0, sill=1.2, style="high sorting-room window with wired glass")
    h.window("w_bW", "bedW", "N", -3.0, 1.2, 1.2, style="casement in the tile-hung gable")
    h.window("w_bE", "bedE", "N", 3.0, 1.2, 1.2, style="casement in the tile-hung gable")
    h.window("w_bath", "bath", "S", -3.0, 0.7, 0.8, sill=1.3, style="frosted")
    h.window("w_att", "attic", "N", -2.5, 0.5, 0.5, sill=1.0, style="small vent window in the west gable apex")
    h.stair("s1", "hall", (0.0, -2.4, 0.9, 1.1), G, F, "S", note="14 risers, oak")
    h.stair("s1u", "landing", (0.0, -2.4, 0.9, 1.1), G, F, "S", kind="void")
    h.stair("s2", "landing", (-0.9, -3.6, -0.2, -2.8), F, A, "N", kind="loft ladder")
    h.chimney(-5.0, -2.0, serves="kitchen range = THE HEARTH / front room", pots=2)
    h.chimney(5.0, 2.0, serves="dining-room fire", pots=2)
    # sorting PUZZLE
    ph = h.wall("sorting", "E", "Pigeonholes", at=-2.4, role="puzzle", note="SORTING FRAME: 6 x 5 pigeonholes; 6 labelled with symbols for the village streets")
    puzzle(h, ph, "SequencePuzzle reskinned: 6 undelivered letters lie on the sorting table; each address is a riddle (street + house "
                  "trait). Slot them in delivery-round order (round card on the office wall); the sealed letter drops from the "
                  "'DEAD LETTERS' box")
    st = h.at("sorting", "SortingTable", 6.6, -1.2, yaw=90)
    h.on(st, "LetterScale"); h.on(st, "Stapler", dx=0.3)
    note(h, st, "undelivered letters bundle (note 1/3)")
    h.wall("sorting", "S", "MailSacks", at=8.0); hide(h, h.props[-1], "behind the mail sacks", crouch=True)
    bx = h.wall("sorting", "W", "IronSafe", at=0.0, note="DEAD LETTERS box (iron, slot on top) = PART CONTAINER")
    container(h, bx, "The sealed letter (addressed to one of the players, in their own handwriting)")
    # office
    dk = h.wall("office", "N", "OfficeDesk", at=3.2, scale=0.85)
    h.on(dk, "Typewriter", dx=-0.2); h.on(dk, "RotaryPhone", dx=0.35, role="phone", note="the house/post-office phone - also the VILLAGE EXCHANGE board extension")
    item(h, dk, "office desk", dx=0.0)
    h.hung("office", "E", "FrameHanging3", at=-1.0, y=1.6, note="DELIVERY ROUND card in a frame (puzzle order)")
    h.wall("office", "W", "DrawerCabinet", at=-1.0)
    # front room
    hr = h.wall("front", "W", "Hearth", at=2.0, role="hearth-decor", note="brick fireplace")
    h.on(hr, "FrameStanding2", role="family-photo", note="Mr Dunn in GPO uniform, 1953"); h.on(hr, "MantelClock", dx=0.5, note="stopped at 3:00")
    item(h, hr, "front-room mantel")
    h.wall("front", "S", "Sofa2", at=-3.0)
    h.at("front", "Armchair", -2.6, 2.0, yaw=200)
    tv = h.wall("front", "E", "Cabinet70s", at=1.2)
    h.on(tv, "TV", role="tv")
    # dining
    dt = h.at("dining", "DiningTable", 3.0, 2.0, yaw=0)
    for dx in (-0.5, 0.5):
        h.at("dining", "ChairDining", 3.0 + dx, 1.25, yaw=0); h.at("dining", "ChairDining", 3.0 + dx, 2.75, yaw=180)
    # kitchen
    rg = h.wall("kitchen", "W", "Hearth", at=-2.0, role="hearth", note="cream Rayburn-style range = THE HEARTH")
    item(h, rg, "range rail")
    h.wall("kitchen", "N", "KitchenSinkUnit", at=-3.2)
    h.wall("kitchen", "N", "Fridge", at=-1.9)
    kt = h.at("kitchen", "TablePainted", -2.8, -2.2, yaw=0)
    h.at("kitchen", "ChairPainted2", -2.8, -2.95, yaw=0)
    h.hung("kitchen", "E", "FuseBox", at=-0.8, y=1.7, role="fuse-box")
    # bedrooms
    bed(h, "bedW", "W", "Bed", 2.0, label="double bed")
    w = h.wall("bedW", "S", "Wardrobe", at=-2.6); hide(h, w, "wardrobe")
    bed(h, "bedE", "E", "BedIron", 2.0, stands=("Nightstand",), stand_side="left", label="spare bed", hiding=False)
    h.wall("bed3", "S", "SewingMachine", at=3.0); h.wall("bed3", "N", "DayBed", at=3.0)
    note(h, h.wall("bed3", "E", "TableSmall", at=-2.0), "Mrs Dunn's note about 'the letter that came back' (note 2/3)")
    # bath
    h.wall("bath", "W", "Bathtub", at=-2.0); hide(h, h.props[-1], "behind the bath curtain")
    h.wall("bath", "N", "Sink", at=-2.0)
    h.hung("bath", "N", "Mirror", at=-2.0, y=1.55, role="mirror")
    h.wall("bath", "S", "Toilet", at=-1.6)
    # attic
    h.wall("attic", "W", "MailSacks", at=0.0)
    note(h, h.wall("attic", "E", "CardboardBox", at=0.0), "dead letter addressed to 'the guests' (note 3/3)")
    shrine(h, "landing", "N", 0.4)
    return h


# ============================================================================================ H13 Farmer's House
def h13():
    h = House(13, "H13", "Farmer's House", "Barrow", "Straw heart", "Sequence", "Green", 3, 6)
    h.traits = dict(archetype="Stone longhouse farmhouse with the barn under the same long roof and a dairy outshut",
                    silhouette="Very long (18 m) and low: farmhouse at the west end with a green door straight into the kitchen, "
                               "a huge barn at the east end with tall cart doors and a hayloft door, one continuous moss-covered "
                               "roof, THREE chimneys (west gable, mid, dairy), a muck heap and a rusty tractor in the yard.")
    levels(h, attic=False, g=2.7, f=2.4)
    h.wing("house", (-6.0, -4.0, 2.0, 4.0), ["G", "F"], "Rubble stone (Stone), whitewash patches, SandstoneBlocks lintels", 0.55,
           "Continuous gable roof 40 deg over house AND barn, ridge along x at 7.2 m, RoofRedSlate (house) / CorrugatedRust (barn end)")
    h.wing("barn", (2.0, -4.0, 12.0, 4.0), ["G", "F"], "Rubble stone + RawPlankWall gable cladding, ventilation slits", 0.45,
           "Same roof; cart doors 3.0 x 3.2 north, hayloft door above")
    h.wing("dairy", (-6.0, -7.0, -1.0, -4.0), ["G"], "Rubble, whitewashed", 0.35, "Catslide continuation of the main roof")
    G, F = "G", "F"
    h.room("kitchen", "Farm kitchen", G, (-6.0, -4.0, -1.5, 4.0), "kitchen", "FloorFlagstone", "Whitewash, hams on hooks", light="pendant + oil lamp")
    h.room("parlour", "Front parlour", G, (-1.5, 0.0, 2.0, 4.0), "living", "Carpet70sB", "Wallpaper2C")
    h.room("hall", "Back hall & stair", G, (-1.5, -4.0, 2.0, 0.0), "hall", "FloorFlagstone", "Whitewash")
    h.room("dairy", "Dairy", G, (-6.0, -7.0, -1.0, -4.0), "utility", "FloorBrick", "TileWallWhite (cracked), slate shelves")
    h.room("barn", "Barn (PUZZLE)", G, (2.0, -4.0, 12.0, 4.0), "barn", "FloorDirt", "rubble + RawPlankWall", light="2 CagedLight on chains")
    h.room("bed1", "Barrows' bedroom", F, (-6.0, 0.0, -1.5, 4.0), "bedroom", "FloorOldWood1", "Wallpaper1")
    h.room("bed2", "Lads' bedroom", F, (-6.0, -4.0, -3.0, 0.0), "bedroom", "FloorOldWood2", "PlasterWorn")
    h.room("landing", "Landing", F, (-3.0, -4.0, 2.0, 0.0), "landing", "FloorOldWood1", "PlasterWorn")
    h.room("bath", "Bathroom", F, (-1.5, 0.0, 2.0, 4.0), "bathroom", "FloorBathTile", "TileWallWhite")
    h.room("hayloft", "Hayloft (attic of the barn)", F, (2.0, -4.0, 12.0, 4.0), "attic", "FloorOldWood2", "RawPlankWall, hay")
    h.door("d_front", G, "kitchen", "N", -3.5, 0.95, role="front", kind="green stable door (top half open by day)", colour="Green")
    h.door("d_back", G, "hall", "S", 0.5, 0.9, role="back", note="to the yard")
    h.door("d_dairy", G, "kitchen", "S", -3.5, 0.85, room_b="dairy")
    h.door("d_hall", G, "kitchen", "E", -2.0, 0.85, room_b="hall")
    h.door("d_par", G, "hall", "N", -0.5, 0.85, room_b="parlour")
    h.door("d_barn", G, "parlour", "E", 2.0, 0.9, room_b="barn", kind="plank door into the barn (two steps down)")
    h.door("d_cart", G, "barn", "N", 7.0, 3.0, height=3.2, role="cart", kind="tall cart doors (barred on the inside)")
    h.door("d_b1", F, "landing", "N", -2.25, 0.8, room_b="bed1")
    h.door("d_b2", F, "landing", "W", -2.0, 0.8, room_b="bed2")
    h.door("d_bath", F, "landing", "N", 0.5, 0.75, room_b="bath")
    h.door("d_hay", F, "hayloft", "N", 9.5, 1.4, height=1.6, role="pitching", kind="hayloft pitching door with a hay hoist beam")
    h.window("w_k1", "kitchen", "N", -5.0, 0.9, 1.0, style="deep-set sash")
    h.window("w_k2", "kitchen", "W", -2.0, 0.9, 1.0, weak=True, style="deep-set sash, rotten")
    h.window("w_par", "parlour", "N", 0.5, 1.1, 1.1, style="sash")
    h.window("w_dairy", "dairy", "S", -3.5, 0.8, 0.5, sill=1.5, weak=True, style="slatted dairy vent (wooden louvres, loose)")
    h.window("w_barn", "barn", "S", 7.0, 0.3, 1.2, sill=1.0, style="ventilation slit (x3 along the south wall)")
    h.window("w_b1", "bed1", "N", -3.8, 0.9, 1.0, style="sash")
    h.window("w_b2", "bed2", "W", -2.0, 0.8, 0.9, style="small sash")
    h.window("w_bath", "bath", "N", 0.8, 0.6, 0.7, sill=1.3, style="frosted")
    h.stair("s1", "hall", (1.0, -3.4, 1.9, -0.1), G, F, "N", note="boxed stair, 12 risers")
    h.stair("s1u", "landing", (1.0, -3.4, 1.9, -0.1), G, F, "N", kind="void")
    h.stair("s2", "barn", (11.0, -3.5, 11.6, -2.3), G, F, "S", kind="fixed ladder to the hayloft")
    h.stair("s2u", "hayloft", (11.0, -3.5, 11.6, -2.3), G, F, "S", kind="hatch")
    h.chimney(-6.0, 0.0, w=1.4, d=1.2, serves="kitchen range = THE HEARTH (inglenook)", pots=2)
    h.chimney(-1.5, 2.0, serves="parlour fire (on the kitchen partition)", pots=1)
    h.chimney(-3.5, -7.0, serves="dairy copper", pots=1)
    # kitchen
    rg = h.wall("kitchen", "W", "Hearth", at=0.0, role="hearth", note="big inglenook range, bacon rack over, settle beside")
    h.on(rg, "MantelClock", note="stopped at 6:00"); h.on(rg, "FrameStanding1", dx=0.6, role="family-photo", note="the Barrows at harvest supper")
    item(h, rg, "inglenook shelf")
    kt = h.at("kitchen", "TableWood3", -3.6, 0.0, yaw=90)
    h.on(kt, "BowlWood2"); h.on(kt, "OilLamp", dx=0.3)
    note(h, kt, "farm diary: 'moved the guardians again' (note 1/3)")
    h.at("kitchen", "BenchPainted", -4.25, 0.0, yaw=90); h.at("kitchen", "BenchPainted", -2.95, 0.0, yaw=270)
    h.wall("kitchen", "S", "KitchenSinkUnit", at=-4.9)
    dz = h.wall("kitchen", "E", "Dresser", at=1.6)
    item(h, dz, "kitchen dresser")
    h.hung("kitchen", "E", "WallClock", at=-0.6, y=2.0)
    pt = h.wall("kitchen", "N", "SideTable", at=-2.1)
    h.on(pt, "RotaryPhone", role="phone"); h.on(pt, "Radio", dx=0.15, role="radio", note="farming programme by day")
    # parlour
    h.at("parlour", "Armchair", 0.5, 1.4, yaw=270)
    h.wall("parlour", "W", "Commode", at=2.8, note="best-china cabinet")
    h.at("parlour", "Rug", 0.3, 2.0)
    # hall
    h.hung("hall", "W", "FuseBox", at=-1.0, y=1.7, role="fuse-box")
    h.hung("hall", "S", "Mirror", at=-0.8, y=1.6, note="small mirror by the back door")
    h.wall("hall", "W", "RubberBoots", at=-3.2)
    # dairy
    h.wall("dairy", "W", "MilkChurns", at=-5.5)
    h.wall("dairy", "E", "CheesePress", at=-5.0)
    h.wall("dairy", "S", "Copper", at=-4.4)
    # barn PUZZLE
    bd = h.hung("barn", "W", "ScarecrowBoard", at=-2.0, y=1.5, role="puzzle", note="SCARECROW BOARD: a painted field map with 6 peg holes")
    puzzle(h, bd, "SequencePuzzle reskinned: the hayloft drawing shows where each of the 6 scarecrows stood the day the boy "
                  "vanished; compare with where they stand NOW in the fields; press the pegs of the ones that moved, in the "
                  "order of the drawing's numbers")
    cc = h.wall("barn", "S", "ChestWorn", at=3.2, note="CORN CHEST = PART CONTAINER")
    container(h, cc, "Straw heart (a corn dolly), tied with red thread")
    h.wall("barn", "E", "HayBales", at=1.5); hide(h, h.props[-1], "behind the hay bales", crouch=True)
    h.wall("barn", "E", "HayBales", at=0.0)
    h.at("barn", "Wheelbarrow", 6.0, -2.0, yaw=0)
    h.wall("barn", "S", "CartWheel", at=5.5)
    h.wall("barn", "S", "ToolChest", at=8.0)
    h.at("barn", "Scarecrow", 9.5, 1.5, yaw=200, note="a 7th scarecrow propped in the barn, facing the door")
    h.wall("barn", "N", "Spade", at=3.0)
    # hayloft
    h.wall("hayloft", "S", "HayBales", at=6.0); hide(h, h.props[-1], "in the hay, hayloft", crouch=True)
    h.wall("hayloft", "N", "HayBales", at=5.0)
    note(h, h.wall("hayloft", "W", "CardboardBox", at=-2.0), "THE BARN DRAWING: child's crayon picture of the fields with numbered scarecrows (note 2/3)")
    item(h, h.wall("hayloft", "S", "Wicker2", at=9.0), "basket in the hayloft")
    # bedrooms
    bed(h, "bed1", "W", "Bed", 2.0, label="double bed")
    w = h.wall("bed1", "E", "Wardrobe", at=2.4); hide(h, w, "wardrobe")
    bed(h, "bed2", "S", "BedIron", -4.6, stands=("",), label="lads' bed", hiding=True)
    note(h, h.wall("bed2", "N", "ChestOfDrawers", at=-4.6), "the lad's school essay 'Our scarecrows' (note 3/3)")
    # bath
    h.wall("bath", "E", "Bathtub", at=2.0)
    h.wall("bath", "W", "Sink", at=2.6)
    h.hung("bath", "W", "Mirror", at=2.6, y=1.55, role="mirror")
    h.wall("bath", "W", "Toilet", at=1.2)
    shrine(h, "landing", "S", -1.0)
    return h


# ============================================================================================ H14 Dollmaker's House
def h14():
    h = House(14, "H14", "Dollmaker's House", "Quill", "Porcelain hand", "Sequence", "Blue", 1, 9)
    h.traits = dict(archetype="Crooked 16th-century timber-framed house, the oldest in the village, with a jettied upper floor",
                    silhouette="Black oak frame with ochre infill panels, the upper floor OVERHANGS the street by 0.6 m, the whole "
                               "house leans ~2 deg to the east, steep mossy clay-tile roof with a single massive CENTRAL brick "
                               "chimney, a bow shop window full of dolls staring out, a small blue door beside the chimney.")
    levels(h, attic=True, g=2.5, f=2.4)
    h.wing("ground", (-4.5, -4.0, 4.5, 4.0), ["G"], "Oak frame (TimberBeam, black) with RenderOchre infill; BrickMixed plinth 0.4",
           T_TIMBER, "-")
    h.wing("upper", (-4.5, -4.0, 4.5, 4.6), ["F", "A"], "Jettied: oak frame + RenderOchre infill; carved bressumer along the "
           "jetty; whole frame leans 2 deg east", T_TIMBER,
           "Gable roof 55 deg, ridge along x at 9.0 m, RoofClay3 (mossy, sagging 0.15 m mid-ridge); central brick stack 1.6 x 2.0")
    G, F, A = "G", "F", "A"
    h.room("lobby", "Lobby entry", G, (-1.0, 1.5, 1.0, 4.0), "hall", "FloorFlagstone", "PlasterWorn")
    h.room("workshop", "Doll workshop & shop (PUZZLE)", G, (-4.5, -1.0, -1.0, 4.0), "workshop", "FloorOldWood2", "PlasterWorn between black studs", light="2 pendants + DeskLamp")
    h.room("parlour", "Hall-parlour", G, (1.0, -1.0, 4.5, 4.0), "living", "FloorDarkPlanks", "PlasterWorn between black studs")
    h.room("kitchen", "Kitchen", G, (-4.5, -4.0, 0.5, -1.0), "kitchen", "FloorBrick", "Whitewash")
    h.room("stairhall", "Stair hall", G, (0.5, -4.0, 4.5, -1.0), "hall", "FloorFlagstone", "PlasterWorn")
    h.room("bed1", "Miss Quill's chamber", F, (-4.5, -1.0, -1.0, 4.6), "bedroom", "FloorOldWood1", "Wallpaper1 (peeling)")
    h.room("closet", "Closet over the lobby", F, (-1.0, 1.5, 1.0, 4.6), "closet", "FloorOldWood1", "PlasterWorn")
    h.room("bed2", "Guest chamber", F, (1.0, -1.0, 4.5, 4.6), "bedroom", "FloorOldWood1", "Wallpaper1B")
    h.room("bath", "Bathroom", F, (-4.5, -4.0, 0.5, -1.0), "bathroom", "FloorBathTile", "TileWallWhite")
    h.room("landing", "Landing", F, (0.5, -4.0, 4.5, -1.0), "landing", "FloorOldWood2", "PlasterWorn")
    h.room("attic", "Attic (the doll store)", A, (-4.5, -4.0, 4.5, 4.6), "attic", "FloorOldWood2", "RawPlankWall", light="none")
    h.door("d_front", G, "lobby", "N", 0.0, 0.85, height=1.95, role="front", kind="low blue plank door with a doll-hand knocker", colour="Blue")
    h.door("d_back", G, "kitchen", "S", -2.0, 0.8, height=1.9, role="back")
    h.door("d_ws", G, "lobby", "W", 2.75, 0.8, room_b="workshop", height=1.9)
    h.door("d_par", G, "lobby", "E", 2.75, 0.8, room_b="parlour", height=1.9)
    h.door("d_kit", G, "workshop", "S", -2.5, 0.8, room_b="kitchen", height=1.9)
    h.door("d_sh", G, "parlour", "S", 3.5, 0.8, room_b="stairhall", height=1.9)
    h.door("d_ksh", G, "kitchen", "E", -2.5, 0.8, room_b="stairhall", height=1.9)
    h.door("d_b2", F, "landing", "N", 3.5, 0.8, room_b="bed2", height=1.85)
    h.door("d_cl", F, "bed2", "W", 3.0, 0.75, room_b="closet", height=1.85)
    h.door("d_b1", F, "closet", "W", 3.0, 0.75, room_b="bed1", height=1.85)
    h.door("d_bath", F, "landing", "W", -2.0, 0.75, room_b="bath", height=1.85)
    h.window("w_shop", "workshop", "N", -2.75, 2.2, 1.4, sill=0.7, style="bow shop window, small leaded panes, DOLLS STANDING IN IT facing the street")
    h.window("w_par", "parlour", "N", 2.75, 1.2, 1.1, style="leaded casement")
    h.window("w_k", "kitchen", "S", -3.6, 0.8, 0.9, weak=True, style="casement, crooked frame")
    h.window("w_sh", "stairhall", "E", -2.5, 0.7, 0.9, weak=True, style="casement, glass cracked")
    h.window("w_b1", "bed1", "N", -2.75, 1.4, 1.0, style="oriel window on the jetty")
    h.window("w_b2", "bed2", "N", 2.75, 1.2, 1.0, style="casement on the jetty")
    h.window("w_cl", "closet", "N", 0.0, 0.5, 0.6, sill=1.1, style="tiny window")
    h.window("w_bath", "bath", "W", -2.5, 0.6, 0.7, sill=1.2, style="frosted")
    h.window("w_att", "attic", "W", 0.0, 0.6, 0.6, sill=1.0, style="gable light")
    h.stair("s1", "stairhall", (1.4, -3.7, 4.2, -2.8), G, F, "E", note="steep winder stair, 13 risers, 3 winders at the top, every tread creaks")
    h.stair("s1u", "landing", (1.4, -3.7, 4.2, -2.8), G, F, "E", kind="void")
    h.stair("s2", "landing", (3.5, -2.6, 4.2, -1.9), F, A, "N", kind="loft ladder")
    h.chimney(0.0, 0.25, w=2.0, d=2.5, above_ridge=1.2, mat="BrickMixed", serves="central stack: parlour fire = THE HEARTH, workshop fire, kitchen range", pots=3)
    # parlour - THE HEARTH
    hr = h.wall("parlour", "W", "Hearth", at=0.25, role="hearth", note="wide open hearth in the central stack, dogs and a spit")
    h.on(hr, "MantelClock", note="stopped at 9:00"); h.on(hr, "FrameStanding2", dx=0.6, role="family-photo", note="Miss Quill as a girl with a doll identical to her")
    item(h, hr, "parlour mantel beam")
    h.at("parlour", "RockingChair", 3.2, 1.6, yaw=270)
    pt = h.wall("parlour", "E", "SideTable", at=1.0)
    h.on(pt, "RotaryPhone", role="phone"); h.on(pt, "Radio", dx=0.15, role="radio", note="radio that plays a music box tune at night")
    h.at("parlour", "Rug", 2.8, 1.5)
    # workshop PUZZLE
    dh = h.at("workshop", "Dollhouse", -2.75, 1.0, yaw=0, role="puzzle",
              note="THE DOLLHOUSE: an exact 1:12 replica of THIS house (generated from H14's own room data), front opens")
    puzzle(h, dh, "SequencePuzzle reskinned: 6 tiny dolls must stand in the dollhouse rooms where the people in the photograph on "
                  "the workbench are standing in the REAL house tonight (moving figures/mannequins placed per match); place them "
                  "room by room; wrong placement makes every doll in the shop window turn its head")
    bench = h.wall("workshop", "W", "TableWood3", at=1.5, scale=0.8, note="dollmaker's bench")
    h.on(bench, "DollParts", dx=-0.3); h.on(bench, "DeskLamp", dx=0.5); h.on(bench, "DollPorcelain", dx=0.1)
    note(h, bench, "the photograph of 'who stands where' (note 1/3)")
    item(h, bench, "dollmaker's bench", dx=-0.6)
    dc = h.wall("workshop", "S", "DollCabinet", at=-3.6, note="glass cabinet of dolls = PART CONTAINER (unlocks)")
    container(h, dc, "Porcelain hand, in a velvet glove")
    h.wall("workshop", "W", "SewingMachine", at=0.4)
    h.at("workshop", "StoolWood1", -2.75, 2.6, yaw=180)
    h.hung("workshop", "E", "FrameHanging2", at=3.0, y=1.6, note="framed doll patterns")
    for i, x in enumerate((-3.9, -1.6)):
        h.at("workshop", "DollAntique" if i else "DollPolly", x, 3.55, yaw=0, y=0.75, role="allow-overlap", note="doll in the shop window")
    # kitchen
    rg = h.wall("kitchen", "N", "Range", at=-0.25, note="range on the back face of the central stack")
    h.wall("kitchen", "W", "Dresser", at=-2.5)
    kt = h.at("kitchen", "TableRound2", -2.3, -2.5)
    item(h, kt, "kitchen table")
    h.wall("kitchen", "S", "KitchenSinkUnit", at=-0.6)
    # stair hall
    h.hung("stairhall", "N", "FuseBox", at=1.4, y=1.7, role="fuse-box")
    h.hung("stairhall", "N", "Mirror", at=2.6, y=1.6, note="old foxed mirror")
    # bed1
    bed(h, "bed1", "W", "Bed", 1.6, label="four-poster-ish bed")
    w = h.wall("bed1", "S", "Wardrobe", at=-3.6); hide(h, w, "wardrobe")
    h.at("bed1", "DollAntique", -2.0, 4.0, yaw=180, y=0.0, role="allow-overlap", note="a doll on the floor by the window, facing the bed")
    # closet
    note(h, h.at("closet", "TableSmall", 0.0, 2.1, yaw=0, scale=0.7), "Miss Quill's letter: 'they are all of us' (note 2/3)")
    # bed2
    bed(h, "bed2", "E", "BedIron", 1.6, stands=("NightstandPainted",), stand_side="right", label="guest bed")
    # bath
    h.wall("bath", "S", "Bathtub", at=-3.0, scale=0.85)
    h.wall("bath", "N", "Sink", at=-3.6)
    h.hung("bath", "N", "Mirror", at=-3.6, y=1.55, role="mirror")
    h.wall("bath", "S", "Toilet", at=-1.0)
    # attic
    h.wall("attic", "N", "DisplayShelves", at=-3.0, note="shelves of unfinished dolls")
    h.wall("attic", "N", "DisplayShelves", at=3.0)
    h.wall("attic", "S", "CellarCrates", at=-2.0); hide(h, h.props[-1], "among the doll crates", crouch=True)
    note(h, h.wall("attic", "W", "ChestWorn", at=-2.0), "list of names with a doll for each villager (note 3/3)")
    shrine(h, "landing", "S", 1.0)
    return h
