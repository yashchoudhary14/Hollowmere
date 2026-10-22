# Hollowmere - assets: every model, texture and sound, where it comes from, who downloads it

> Machine-readable: `docs/design/asset_catalog.json` (every key), `tools/third-party.json` (Poly Haven + ambientCG downloads, merged with what you already have), `tools/sketchfab.json` (CC-BY models). Asset KEYS are what the house/village JSON use.

## Who does what
| Source | Licence | Who downloads | How |
|---|---|---|---|
| Poly Haven (models, textures, HDRIs) | CC0 - no attribution needed, credit anyway in CREDITS.md | **Claude Code (PC)** | `tools/fetch-assets.ps1` (already exists) reads the merged `tools/third-party.json` |
| ambientCG (textures) | CC0 | **Claude Code (PC)** | same script |
| Sketchfab (24 CC-BY models: phone, bath, toilet, dolls, loom, enlarger...) | CC BY 4.0 - **must credit the author** (CREDITS.md is generated) | **Claude Code (PC)** after **you** give it a Sketchfab API token once | new `tools/fetch-sketchfab.ps1` + `tools/sketchfab.json`; fallback: you click 'Download 3D model' on each page (list below) |
| Fab - Quixel 'Saloon Interior' (FREE, already in `AssetDownloads/salooninterior.unitypackage`) | Fab Standard License (any engine) | done | `tools/import-assetstore.ps1` (already set up) |
| Sonniss GDC 2026 bundle (already in `AssetDownloads/`) | Sonniss GDC licence (royalty-free, commercial OK) | done | `tools/prepare-audio.ps1` + `tools/audio-sources.json` |
| Procedural (`BUILD`) | own work | **Claude Code** | PropKit/BuilderKit primitives + catalog materials - shapes in the table below |
| Paid upgrades (later) | store EULA | **you** buy, Claude Code imports | list at the bottom; buy only after the greybox+free pass plays well |

### You do this once (10 minutes)
1. Make a free Sketchfab account at https://sketchfab.com/signup.
2. Get your API token: Sketchfab -> Settings -> Password & API -> 'API Token' -> copy.
3. Save the token on your PC only (it must never be pasted into a chat or committed): open Notepad, paste the token, and save it as `%USERPROFILE%\.hollowmere\sketchfab_token.txt` (make the `.hollowmere` folder in your user folder; in Notepad's Save dialog choose 'All files' so it doesn't add .txt twice). Then tell Claude Code: `Run tools/fetch-sketchfab.ps1.`
4. If the script reports a 401/403 for any model (Sketchfab sometimes requires the OAuth flow), open the model's page from the table below, click **Download 3D model -> glTF (.glb)** and drop the file in `C:\dev\Hollowmere\AssetDownloads\Sketchfab\<key>.glb` (the key is the first column below; `<uid>.glb` also works). The script picks local files up automatically.
5. Unity needs glTF import for .glb: Claude Code adds `com.unity.cloud.gltfast` to `BatchTools.RequiredPackages` and runs `setup`.

## Scaling rule (all models)
Every model key has a target size (w x d x h, metres; w = along the wall it backs onto, d = depth from the wall, h = height). On import, Claude Code scales the model UNIFORMLY so it fits inside that box and is as large as possible, pivots it at the bottom centre of its bounds, rotates it so its front faces +z, and logs any axis that ends up >10% off the target. A per-house `scale` multiplies the box (e.g. 0.85 for a smaller bath).

## Models
| Key | Source | Asset id | Size w x d x h (m) | Download | Used in | Notes / paid upgrade |
|---|---|---|---|---|---|---|
| `Wardrobe` | polyhaven | [GothicCabinet_01](https://polyhaven.com/a/GothicCabinet_01) | 1.2 x 0.6 x 2.1 | owned | H00, H01, H02, H03, H04, H05, H06, H07, H08, H09, H10, H11, H12, H13, H14 |   |
| `Bed` | polyhaven | [GothicBed_01](https://polyhaven.com/a/GothicBed_01) | 1.6 x 2.1 x 1.4 | owned | H00, H01, H03, H04, H05, H06, H07, H08, H10, H12, H13, H14 |   |
| `Chair` | polyhaven | [WoodenChair_01](https://polyhaven.com/a/WoodenChair_01) | 0.45 x 0.5 x 0.95 | owned | H03, H05, H07, H10, H11 |   |
| `RockingChair` | polyhaven | [Rockingchair_01](https://polyhaven.com/a/Rockingchair_01) | 0.6 x 0.95 x 1.05 | owned | H01, H02, H04, H07, H14 |   |
| `Shelf` | polyhaven | [wooden_bookshelf_worn](https://polyhaven.com/a/wooden_bookshelf_worn) | 1.0 x 0.35 x 1.9 | owned | H03, H07, H11, village |   |
| `GrandfatherClock` | polyhaven | [vintage_grandfather_clock_01](https://polyhaven.com/a/vintage_grandfather_clock_01) | 0.55 x 0.32 x 2.05 | owned | H00 |   |
| `Barrel` | polyhaven | [wine_barrel_01](https://polyhaven.com/a/wine_barrel_01) | 0.6 x 0.6 x 0.85 | owned | village |   |
| `Crate` | polyhaven | [wooden_crate_01](https://polyhaven.com/a/wooden_crate_01) | 0.6 x 0.45 x 0.45 | owned | village |   |
| `Books` | polyhaven | [book_encyclopedia_set_01](https://polyhaven.com/a/book_encyclopedia_set_01) | 0.5 x 0.2 x 0.28 | owned | spare (not placed yet) |   |
| `MantelClock` | polyhaven | [mantel_clock_01](https://polyhaven.com/a/mantel_clock_01) | 0.3 x 0.14 x 0.32 | owned | H00, H01, H04, H05, H07, H08, H09, H10, H11, H12, H13, H14 |   |
| `OilLamp` | polyhaven | [vintage_oil_lamp](https://polyhaven.com/a/vintage_oil_lamp) | 0.18 x 0.18 x 0.45 | owned | H00, H02, H04, H13 |   |
| `Candleholders` | polyhaven | [brass_candleholders](https://polyhaven.com/a/brass_candleholders) | 0.35 x 0.12 x 0.3 | owned | H02, H07, H11 |   |
| `Candlestick` | polyhaven | [wooden_candlestick](https://polyhaven.com/a/wooden_candlestick) | 0.1 x 0.1 x 0.25 | owned | H00, H02 |   |
| `Lantern` | polyhaven | [Lantern_01](https://polyhaven.com/a/Lantern_01) | 0.2 x 0.2 x 0.4 | owned | village |   |
| `Bucket` | polyhaven | [wooden_bucket_01](https://polyhaven.com/a/wooden_bucket_01) | 0.32 x 0.32 x 0.3 | owned | village |   |
| `Stool` | polyhaven | [wooden_stool_02](https://polyhaven.com/a/wooden_stool_02) | 0.35 x 0.35 x 0.45 | owned | spare (not placed yet) |   |
| `Hammer` | polyhaven | [wooden_hammer_01](https://polyhaven.com/a/wooden_hammer_01) | 0.32 x 0.1 x 0.04 | owned | spare (not placed yet) |   |
| `Axe` | polyhaven | [wooden_axe_02](https://polyhaven.com/a/wooden_axe_02) | 0.2 x 0.06 x 0.75 | owned | village |   |
| `WateringCan` | polyhaven | [watering_can_metal_01](https://polyhaven.com/a/watering_can_metal_01) | 0.5 x 0.2 x 0.35 | owned | H01, village |   |
| `Stump` | polyhaven | [tree_stump_01](https://polyhaven.com/a/tree_stump_01) | 0.8 x 0.8 x 0.5 | owned | village |   |
| `DeadTrunk` | polyhaven | [dead_tree_trunk](https://polyhaven.com/a/dead_tree_trunk) | 4.0 x 0.7 x 0.7 | owned | village |   |
| `Branches` | polyhaven | [dry_branches_medium_01](https://polyhaven.com/a/dry_branches_medium_01) | 1.5 x 1.0 x 0.4 | owned | village |   |
| `Rocks` | polyhaven | [rock_moss_set_02](https://polyhaven.com/a/rock_moss_set_02) | 1.5 x 1.2 x 0.6 | owned | village |   |
| `Boulder` | polyhaven | [boulder_01](https://polyhaven.com/a/boulder_01) | 2.0 x 1.6 x 1.2 | owned | village |   |
| `CartWheel` | owned | His_Wil_Cart_Wheel_Wood_Old_01 (Fab Saloon Interior) | 1.1 x 0.12 x 1.1 | owned | H13, village |   |
| `ChestWorn` | owned | Res_Sto_Chest_Wood_Worn_08 (Fab Saloon Interior) | 0.9 x 0.5 x 0.55 | owned | H00, H02, H04, H06, H07, H11, H13, H14 |   |
| `MetalBucket` | owned | Res_Rur_Storage_Bucket_Metal_Worn_03 (Fab) | 0.3 x 0.3 x 0.3 | owned | village |   |
| `BarrelWorn` | owned | His_Med_Storage_Barrel_Wood_Worn_01 (Fab) | 0.65 x 0.65 x 0.95 | owned | H01, H04, H05, H08, H09, village |   |
| `OldChair` | owned | Res_Fur_Chair_Wood_Old_03 (Fab) | 0.45 x 0.5 x 0.95 | owned | spare (not placed yet) |   |
| `ShelfRustic` | owned | Res_Rur_Furniture_Shelf_Wood_Old_01 (Fab) | 1.0 x 0.3 x 1.0 | owned | H01 |   |
| `FloorLampOld` | owned | His_Sal_Furniture_Lamp_Wood_Old_02 (Fab) | 0.3 x 0.3 x 0.6 | owned | spare (not placed yet) |   |
| `Horseshoe` | owned | His_Wil_Equipment_Horseshoe_Metal_Rusted_01 (Fab) | 0.12 x 0.02 x 0.13 | owned | spare (not placed yet) |   |
| `PictureFrame` | owned | Urb_Dec_PictureFrame_Wood_Worn_01 (Fab) | 0.35 x 0.03 x 0.45 | owned | H00 |   |
| `CeilingBeam` | owned | Ind_Sup_Beam_Wood_Old_04 (Fab) | 4.0 x 0.2 x 0.22 | owned | spare (not placed yet) |   |
| `Jug` | owned | Res_Sto_Flask_Ceramic_Worn_01 (Fab) | 0.18 x 0.18 x 0.3 | owned | H04 |   |
| `StoolArmless` | owned | Res_Fur_Stool_Wood_Armless_01 (Fab) | 0.35 x 0.35 x 0.5 | owned | spare (not placed yet) |   |
| `Armchair` | polyhaven | [ArmChair_01](https://polyhaven.com/a/ArmChair_01) | 0.85 x 0.85 x 0.95 | auto | H00, H01, H02, H03, H08, H09, H11, H12, H13 |   |
| `ArmchairGreen` | polyhaven | [GreenChair_01](https://polyhaven.com/a/GreenChair_01) | 0.75 x 0.8 x 0.95 | auto | H05, H07, H10 |   |
| `LoungeChair70s` | polyhaven | [mid_century_lounge_chair](https://polyhaven.com/a/mid_century_lounge_chair) | 0.8 x 0.85 x 0.85 | auto | spare (not placed yet) |   |
| `Sofa` | polyhaven | [Sofa_01](https://polyhaven.com/a/Sofa_01) | 2.0 x 0.9 x 0.85 | auto | H03, H05 |   |
| `Sofa2` | polyhaven | [sofa_02](https://polyhaven.com/a/sofa_02) | 1.9 x 0.9 x 0.85 | auto | H01, H08, H12 |   |
| `Sofa3` | polyhaven | [sofa_03](https://polyhaven.com/a/sofa_03) | 1.8 x 0.85 x 0.8 | auto | H00, H06, H10, H11 |   |
| `SofaPainted` | polyhaven | [painted_wooden_sofa](https://polyhaven.com/a/painted_wooden_sofa) | 1.6 x 0.7 x 0.9 | auto | H04, H07, H09 |   |
| `Ottoman` | polyhaven | [Ottoman_01](https://polyhaven.com/a/Ottoman_01) | 0.6 x 0.6 x 0.42 | auto | spare (not placed yet) |   |
| `CoffeeTable` | polyhaven | [CoffeeTable_01](https://polyhaven.com/a/CoffeeTable_01) | 1.1 x 0.6 x 0.45 | auto | H01, H03, H05, H08 |   |
| `CoffeeTableRound` | polyhaven | [coffee_table_round_01](https://polyhaven.com/a/coffee_table_round_01) | 0.8 x 0.8 x 0.45 | auto | H00, H10 |   |
| `DiningTable` | polyhaven | [dining_table](https://polyhaven.com/a/dining_table) | 1.8 x 0.9 x 0.76 | auto | H01, H12 |   |
| `TableWood1` | polyhaven | [WoodenTable_01](https://polyhaven.com/a/WoodenTable_01) | 1.4 x 0.8 x 0.76 | auto | H00, H03, H05, H06, H08, H09 |   |
| `TableWood2` | polyhaven | [WoodenTable_02](https://polyhaven.com/a/WoodenTable_02) | 1.6 x 0.85 x 0.76 | auto | H02, H07, H11 |   |
| `TableWood3` | polyhaven | [WoodenTable_03](https://polyhaven.com/a/WoodenTable_03) | 2.0 x 0.9 x 0.78 | auto | H00, H09, H10, H13, H14 |   |
| `TableRound` | polyhaven | [round_wooden_table_01](https://polyhaven.com/a/round_wooden_table_01) | 1.0 x 1.0 x 0.75 | auto | H02 |   |
| `TableRound2` | polyhaven | [round_wooden_table_02](https://polyhaven.com/a/round_wooden_table_02) | 0.9 x 0.9 x 0.75 | auto | H07, H14 |   |
| `TablePainted` | polyhaven | [painted_wooden_table](https://polyhaven.com/a/painted_wooden_table) | 1.2 x 0.7 x 0.76 | auto | H01, H10, H12 |   |
| `TableSmall` | polyhaven | [small_wooden_table_01](https://polyhaven.com/a/small_wooden_table_01) | 0.7 x 0.5 x 0.72 | auto | H00, H04, H05, H06, H08, H09, H10, H11, H12, H14 |   |
| `SideTable` | polyhaven | [side_table_01](https://polyhaven.com/a/side_table_01) | 0.5 x 0.5 x 0.6 | auto | H01, H03, H04, H05, H06, H07, H08, H09, H10, H11, H13, H14 |   |
| `SideTableTall` | polyhaven | [side_table_tall_01](https://polyhaven.com/a/side_table_tall_01) | 0.45 x 0.45 x 0.85 | auto | H00 |   |
| `TableGallinera` | polyhaven | [gallinera_table](https://polyhaven.com/a/gallinera_table) | 1.5 x 0.8 x 0.76 | auto | H04 |   |
| `ChairDining` | polyhaven | [dining_chair_02](https://polyhaven.com/a/dining_chair_02) | 0.45 x 0.5 x 0.95 | auto | H00, H01, H03, H05, H08, H11, H12 |   |
| `ChairPainted1` | polyhaven | [painted_wooden_chair_01](https://polyhaven.com/a/painted_wooden_chair_01) | 0.45 x 0.48 x 0.9 | auto | H01, H10 |   |
| `ChairPainted2` | polyhaven | [painted_wooden_chair_02](https://polyhaven.com/a/painted_wooden_chair_02) | 0.45 x 0.48 x 0.9 | auto | H01, H12 |   |
| `ChairGallinera` | polyhaven | [gallinera_chair](https://polyhaven.com/a/gallinera_chair) | 0.45 x 0.5 x 0.9 | auto | H02, H04 |   |
| `SchoolChair` | polyhaven | [SchoolChair_01](https://polyhaven.com/a/SchoolChair_01) | 0.42 x 0.42 x 0.8 | auto | village |   |
| `SchoolDesk` | polyhaven | [SchoolDesk_01](https://polyhaven.com/a/SchoolDesk_01) | 0.65 x 0.5 x 0.75 | auto | H03, village |   |
| `StoolFolding` | polyhaven | [folding_wooden_stool](https://polyhaven.com/a/folding_wooden_stool) | 0.4 x 0.35 x 0.45 | auto | spare (not placed yet) |   |
| `StoolWood1` | polyhaven | [wooden_stool_01](https://polyhaven.com/a/wooden_stool_01) | 0.35 x 0.35 x 0.45 | auto | H00, H14 |   |
| `StoolPainted` | polyhaven | [painted_wooden_stool](https://polyhaven.com/a/painted_wooden_stool) | 0.35 x 0.35 x 0.45 | auto | H07 |   |
| `BenchPainted` | polyhaven | [painted_wooden_bench](https://polyhaven.com/a/painted_wooden_bench) | 1.4 x 0.4 x 0.45 | auto | H05, H09, H10, H13, village |   |
| `BedIron` | polyhaven | [old_bed_frame](https://polyhaven.com/a/old_bed_frame) | 1.0 x 2.0 x 1.05 | auto | H00, H01, H02, H03, H04, H05, H08, H09, H10, H11, H12, H13, H14 |   |
| `DayBed` | polyhaven | [vintage_day_bed](https://polyhaven.com/a/vintage_day_bed) | 0.95 x 1.95 x 0.85 | auto | H01, H07, H12 |   |
| `Nightstand` | polyhaven | [ClassicNightstand_01](https://polyhaven.com/a/ClassicNightstand_01) | 0.5 x 0.4 x 0.6 | auto | H00, H01, H03, H04, H05, H06, H07, H10, H11, H12, H13, H14 |   |
| `NightstandPainted` | polyhaven | [painted_wooden_nightstand](https://polyhaven.com/a/painted_wooden_nightstand) | 0.45 x 0.38 x 0.6 | auto | H00, H01, H02, H05, H07, H09, H10, H14 |   |
| `Commode` | polyhaven | [GothicCommode_01](https://polyhaven.com/a/GothicCommode_01) | 1.1 x 0.5 x 0.9 | auto | H01, H05, H07, H08, H10, H13 |   |
| `ChestOfDrawers` | polyhaven | [vintage_wooden_drawer_01](https://polyhaven.com/a/vintage_wooden_drawer_01) | 0.9 x 0.48 x 1.0 | auto | H00, H02, H03, H04, H05, H06, H09, H13 |   |
| `DrawerCabinet` | polyhaven | [drawer_cabinet](https://polyhaven.com/a/drawer_cabinet) | 0.6 x 0.45 x 1.1 | auto | H03, H05, H08, H10, H12 |   |
| `HallConsole` | polyhaven | [ClassicConsole_01](https://polyhaven.com/a/ClassicConsole_01) | 1.1 x 0.4 x 0.82 | auto | H00, H03, H05 |   |
| `CabinetVintage` | polyhaven | [vintage_cabinet_01](https://polyhaven.com/a/vintage_cabinet_01) | 1.0 x 0.45 x 1.8 | auto | H01, H03, H05, H11 |   |
| `Cabinet70s` | polyhaven | [modern_wooden_cabinet](https://polyhaven.com/a/modern_wooden_cabinet) | 1.2 x 0.45 x 0.8 | auto | H01, H06, H08, H10, H12 |   |
| `CabinetPainted` | polyhaven | [painted_wooden_cabinet](https://polyhaven.com/a/painted_wooden_cabinet) | 1.0 x 0.5 x 1.9 | auto | H01, H03, H09, H11 |   |
| `CabinetPainted2` | polyhaven | [painted_wooden_cabinet_02](https://polyhaven.com/a/painted_wooden_cabinet_02) | 0.9 x 0.45 x 1.0 | auto | spare (not placed yet) |   |
| `ShelvesPainted` | polyhaven | [painted_wooden_shelves](https://polyhaven.com/a/painted_wooden_shelves) | 0.9 x 0.3 x 1.6 | auto | H02, H04 |   |
| `DisplayShelves` | polyhaven | [wooden_display_shelves_01](https://polyhaven.com/a/wooden_display_shelves_01) | 1.2 x 0.4 x 1.8 | auto | H00, H14 |   |
| `SteelShelves` | polyhaven | [steel_frame_shelves_01](https://polyhaven.com/a/steel_frame_shelves_01) | 1.2 x 0.45 x 1.9 | auto | H01, H04, H05, H07, H08, H10 |   |
| `SteelShelves2` | polyhaven | [steel_frame_shelves_02](https://polyhaven.com/a/steel_frame_shelves_02) | 1.0 x 0.4 x 1.8 | auto | H01, H03, H05, H07, H08, H10 |   |
| `MetalRack` | polyhaven | [worn_metal_rack](https://polyhaven.com/a/worn_metal_rack) | 1.0 x 0.4 x 1.7 | auto | H00 |   |
| `OfficeDesk` | polyhaven | [metal_office_desk](https://polyhaven.com/a/metal_office_desk) | 1.4 x 0.75 x 0.76 | auto | H03, H05, H07, H11, H12 |   |
| `ToolCart` | polyhaven | [tool_cart](https://polyhaven.com/a/tool_cart) | 0.8 x 0.45 x 0.9 | auto | spare (not placed yet) |   |
| `PicnicTable` | polyhaven | [wooden_picnic_table](https://polyhaven.com/a/wooden_picnic_table) | 1.8 x 1.5 x 0.75 | auto | spare (not placed yet) |   |
| `Mirror` | polyhaven | [ornate_mirror_01](https://polyhaven.com/a/ornate_mirror_01) | 0.6 x 0.05 x 0.9 | auto | H00, H01, H02, H03, H04, H05, H06, H07, H08, H09, H10, H11, H12, H13, H14 |   |
| `FrameHanging1` | polyhaven | [hanging_picture_frame_01](https://polyhaven.com/a/hanging_picture_frame_01) | 0.4 x 0.03 x 0.5 | auto | H00, H02, H06 |   |
| `FrameHanging2` | polyhaven | [hanging_picture_frame_02](https://polyhaven.com/a/hanging_picture_frame_02) | 0.5 x 0.03 x 0.4 | auto | H00, H04, H14 |   |
| `FrameHanging3` | polyhaven | [hanging_picture_frame_03](https://polyhaven.com/a/hanging_picture_frame_03) | 0.6 x 0.03 x 0.45 | auto | H00, H01, H08, H12 |   |
| `FrameFancy1` | polyhaven | [fancy_picture_frame_01](https://polyhaven.com/a/fancy_picture_frame_01) | 0.6 x 0.05 x 0.75 | auto | H01, H05, H09, H10 |   |
| `FrameFancy2` | polyhaven | [fancy_picture_frame_02](https://polyhaven.com/a/fancy_picture_frame_02) | 0.45 x 0.05 x 0.55 | auto | H03, H07, H10 |   |
| `FrameStanding1` | polyhaven | [standing_picture_frame_01](https://polyhaven.com/a/standing_picture_frame_01) | 0.18 x 0.08 x 0.24 | auto | H01, H04, H05, H11, H13 |   |
| `FrameStanding2` | polyhaven | [standing_picture_frame_02](https://polyhaven.com/a/standing_picture_frame_02) | 0.22 x 0.08 x 0.18 | auto | H01, H02, H09, H12, H14 |   |
| `WallClock` | polyhaven | [wall_clock](https://polyhaven.com/a/wall_clock) | 0.35 x 0.06 x 0.35 | auto | H00, H02, H05, H06, H11, H13 |   |
| `AlarmClock` | polyhaven | [alarm_clock_01](https://polyhaven.com/a/alarm_clock_01) | 0.14 x 0.08 x 0.16 | auto | H00 |   |
| `TV` | polyhaven | [Television_01](https://polyhaven.com/a/Television_01) | 0.7 x 0.5 x 0.55 | auto | H01, H08, H12 |   |
| `TV2` | polyhaven | [television_02](https://polyhaven.com/a/television_02) | 0.6 x 0.45 x 0.5 | auto | H00, H06, H10 |   |
| `Radio` | polyhaven | [vintage_radio_transceiver](https://polyhaven.com/a/vintage_radio_transceiver) | 0.45 x 0.25 x 0.3 | auto | H00, H02, H03, H04, H05, H09, H11, H13, H14 |   |
| `CassettePlayer` | polyhaven | [cassette_player](https://polyhaven.com/a/cassette_player) | 0.35 x 0.22 x 0.12 | auto | H07 |   |
| `CassettePortable` | polyhaven | [portable_cassette_player](https://polyhaven.com/a/portable_cassette_player) | 0.3 x 0.1 x 0.18 | auto | spare (not placed yet) |   |
| `Boombox` | polyhaven | [boombox](https://polyhaven.com/a/boombox) | 0.55 x 0.2 x 0.3 | auto | spare (not placed yet) |   |
| `Kettle` | polyhaven | [vintage_electric_kettle](https://polyhaven.com/a/vintage_electric_kettle) | 0.25 x 0.18 x 0.25 | auto | H00 |   |
| `Cooker` | polyhaven | [electric_stove](https://polyhaven.com/a/electric_stove) | 0.6 x 0.6 x 0.9 | auto | H01, H04 |   |
| `MasonryHeater` | polyhaven | [scandinavian_masonry_heater](https://polyhaven.com/a/scandinavian_masonry_heater) | 1.2 x 0.9 x 2.2 | auto | spare (not placed yet) |   |
| `BarrelStove` | polyhaven | [barrel_stove](https://polyhaven.com/a/barrel_stove) | 0.6 x 0.9 x 1.6 | auto | spare (not placed yet) |   |
| `Chandelier1` | polyhaven | [Chandelier_01](https://polyhaven.com/a/Chandelier_01) | 0.7 x 0.7 x 0.7 | auto | spare (not placed yet) |   |
| `Chandelier3` | polyhaven | [Chandelier_03](https://polyhaven.com/a/Chandelier_03) | 0.6 x 0.6 x 0.6 | auto | spare (not placed yet) |   |
| `LanternChandelier` | polyhaven | [lantern_chandelier_01](https://polyhaven.com/a/lantern_chandelier_01) | 0.6 x 0.6 x 0.8 | auto | spare (not placed yet) |   |
| `PullChainSocket` | polyhaven | [pull_chain_light_socket](https://polyhaven.com/a/pull_chain_light_socket) | 0.1 x 0.1 x 0.25 | auto | spare (not placed yet) |   |
| `Lightbulb` | polyhaven | [lightbulb_01](https://polyhaven.com/a/lightbulb_01) | 0.07 x 0.07 x 0.12 | auto | spare (not placed yet) |   |
| `CagedLight` | polyhaven | [caged_hanging_light](https://polyhaven.com/a/caged_hanging_light) | 0.25 x 0.25 x 0.45 | auto | spare (not placed yet) |   |
| `WallLamp` | polyhaven | [industrial_wall_lamp](https://polyhaven.com/a/industrial_wall_lamp) | 0.2 x 0.3 x 0.3 | auto | spare (not placed yet) |   |
| `DeskLamp` | polyhaven | [desk_lamp_arm_01](https://polyhaven.com/a/desk_lamp_arm_01) | 0.25 x 0.4 x 0.5 | auto | H00, H03, H10, H11, H14 |   |
| `WoodenLantern` | polyhaven | [wooden_lantern_01](https://polyhaven.com/a/wooden_lantern_01) | 0.25 x 0.25 x 0.45 | auto | village |   |
| `StreetLamp` | polyhaven | [street_lamp_01](https://polyhaven.com/a/street_lamp_01) | 0.6 x 0.6 x 4.2 | auto | village |   |
| `StreetLamp2` | polyhaven | [street_lamp_02](https://polyhaven.com/a/street_lamp_02) | 0.6 x 0.6 x 4.0 | auto | village |   |
| `Flashlight` | polyhaven | [vintage_flashlight](https://polyhaven.com/a/vintage_flashlight) | 0.06 x 0.25 x 0.08 | auto | spare (not placed yet) |   |
| `PotEnamel` | polyhaven | [pot_enamel_01](https://polyhaven.com/a/pot_enamel_01) | 0.3 x 0.25 x 0.2 | auto | H01, H08 |   |
| `PanBrass` | polyhaven | [brass_pan_01](https://polyhaven.com/a/brass_pan_01) | 0.45 x 0.25 x 0.08 | auto | spare (not placed yet) |   |
| `PotBrass1` | polyhaven | [brass_pot_01](https://polyhaven.com/a/brass_pot_01) | 0.3 x 0.3 x 0.25 | auto | spare (not placed yet) |   |
| `PotBrass2` | polyhaven | [brass_pot_02](https://polyhaven.com/a/brass_pot_02) | 0.25 x 0.25 x 0.2 | auto | spare (not placed yet) |   |
| `MetalJug` | polyhaven | [metal_jug](https://polyhaven.com/a/metal_jug) | 0.15 x 0.15 x 0.25 | auto | spare (not placed yet) |   |
| `TeaSet` | polyhaven | [tea_set_01](https://polyhaven.com/a/tea_set_01) | 0.45 x 0.3 x 0.2 | auto | H00, H05 |   |
| `BowlWood` | polyhaven | [wooden_bowl_01](https://polyhaven.com/a/wooden_bowl_01) | 0.25 x 0.25 x 0.1 | auto | H02 |   |
| `BowlWood2` | polyhaven | [wooden_bowl_02](https://polyhaven.com/a/wooden_bowl_02) | 0.3 x 0.3 x 0.12 | auto | H09, H13 |   |
| `SpoonWood` | polyhaven | [wooden_spoon](https://polyhaven.com/a/wooden_spoon) | 0.3 x 0.06 x 0.03 | auto | spare (not placed yet) |   |
| `CuttingBoard` | polyhaven | [wooden_cutting_board](https://polyhaven.com/a/wooden_cutting_board) | 0.45 x 0.3 x 0.03 | auto | H08 |   |
| `CeramicPot` | polyhaven | [ceramic_pot](https://polyhaven.com/a/ceramic_pot) | 0.3 x 0.3 x 0.35 | auto | spare (not placed yet) |   |
| `WineBottles` | polyhaven | [wine_bottles_01](https://polyhaven.com/a/wine_bottles_01) | 0.3 x 0.2 x 0.32 | auto | spare (not placed yet) |   |
| `Apples` | polyhaven | [food_apple_01](https://polyhaven.com/a/food_apple_01) | 0.08 x 0.08 x 0.08 | auto | spare (not placed yet) |   |
| `Onions` | polyhaven | [yellow_onion](https://polyhaven.com/a/yellow_onion) | 0.09 x 0.09 x 0.09 | auto | H01 |   |
| `SweetPotato` | polyhaven | [sweet_potato](https://polyhaven.com/a/sweet_potato) | 0.2 x 0.08 x 0.08 | auto | H09 |   |
| `TinnedFood` | polyhaven | [russian_food_cans_01](https://polyhaven.com/a/russian_food_cans_01) | 0.3 x 0.2 x 0.12 | auto | H01 |   |
| `LongLifeFood` | polyhaven | [long_life_food](https://polyhaven.com/a/long_life_food) | 0.3 x 0.2 x 0.2 | auto | spare (not placed yet) |   |
| `CheeseBox` | polyhaven | [CheeseBox_01](https://polyhaven.com/a/CheeseBox_01) | 0.25 x 0.25 x 0.1 | auto | spare (not placed yet) |   |
| `CrateWood2` | polyhaven | [wooden_crate_02](https://polyhaven.com/a/wooden_crate_02) | 0.8 x 0.6 x 0.6 | auto | spare (not placed yet) |   |
| `Barrel2` | polyhaven | [Barrel_02](https://polyhaven.com/a/Barrel_02) | 0.6 x 0.6 x 0.9 | auto | H04, H07 |   |
| `BarrelsWood` | polyhaven | [wooden_barrels_01](https://polyhaven.com/a/wooden_barrels_01) | 1.4 x 0.8 x 1.0 | auto | H07, H08 |   |
| `BucketWood2` | polyhaven | [wooden_bucket_02](https://polyhaven.com/a/wooden_bucket_02) | 0.35 x 0.35 x 0.32 | auto | spare (not placed yet) |   |
| `Wicker1` | polyhaven | [wicker_basket_01](https://polyhaven.com/a/wicker_basket_01) | 0.45 x 0.35 x 0.3 | auto | H04 |   |
| `Wicker2` | polyhaven | [wicker_basket_02](https://polyhaven.com/a/wicker_basket_02) | 0.4 x 0.4 x 0.4 | auto | H04, H05, H09, H13 |   |
| `CardboardBox` | polyhaven | [cardboard_box_01](https://polyhaven.com/a/cardboard_box_01) | 0.5 x 0.4 x 0.4 | auto | H03, H04, H05, H06, H10, H12, H13 |   |
| `Suitcase` | polyhaven | [vintage_suitcase](https://polyhaven.com/a/vintage_suitcase) | 0.65 x 0.2 x 0.45 | auto | H01, H02, H04, H06, H11 |   |
| `Toolbox` | polyhaven | [metal_toolbox](https://polyhaven.com/a/metal_toolbox) | 0.5 x 0.22 x 0.25 | auto | H01 |   |
| `ToolChest` | polyhaven | [metal_tool_chest](https://polyhaven.com/a/metal_tool_chest) | 0.7 x 0.45 x 0.9 | auto | H01, H08, H09, H13 |   |
| `OilTin` | polyhaven | [oil_tin](https://polyhaven.com/a/oil_tin) | 0.2 x 0.15 x 0.25 | auto | village |   |
| `CanRusted` | polyhaven | [can_rusted](https://polyhaven.com/a/can_rusted) | 0.1 x 0.1 x 0.12 | auto | village |   |
| `Jerrycan` | polyhaven | [metal_jerrycan](https://polyhaven.com/a/metal_jerrycan) | 0.35 x 0.17 x 0.47 | auto | village |   |
| `TrashCan` | polyhaven | [metal_trash_can](https://polyhaven.com/a/metal_trash_can) | 0.5 x 0.5 x 0.75 | auto | village |   |
| `CompostBags` | polyhaven | [compost_bags](https://polyhaven.com/a/compost_bags) | 0.9 x 0.6 x 0.4 | auto | village |   |
| `CementBag` | polyhaven | [cement_bag](https://polyhaven.com/a/cement_bag) | 0.6 x 0.4 x 0.15 | auto | village |   |
| `PlanterBox1` | polyhaven | [planter_box_01](https://polyhaven.com/a/planter_box_01) | 1.0 x 0.4 x 0.4 | auto | H01, village |   |
| `PlanterBox2` | polyhaven | [planter_box_02](https://polyhaven.com/a/planter_box_02) | 0.8 x 0.4 x 0.4 | auto | village |   |
| `PlanterPot` | polyhaven | [planter_pot_clay](https://polyhaven.com/a/planter_pot_clay) | 0.35 x 0.35 x 0.3 | auto | H01, village |   |
| `SeedTray` | polyhaven | [seeding_tray_01](https://polyhaven.com/a/seeding_tray_01) | 0.5 x 0.3 x 0.08 | auto | H01 |   |
| `PottedPlant1` | polyhaven | [potted_plant_01](https://polyhaven.com/a/potted_plant_01) | 0.4 x 0.4 x 0.8 | auto | spare (not placed yet) |   |
| `PottedPlant2` | polyhaven | [potted_plant_02](https://polyhaven.com/a/potted_plant_02) | 0.35 x 0.35 x 0.6 | auto | H05 |   |
| `Vase1` | polyhaven | [ceramic_vase_01](https://polyhaven.com/a/ceramic_vase_01) | 0.2 x 0.2 x 0.35 | auto | spare (not placed yet) |   |
| `Vase2` | polyhaven | [ceramic_vase_02](https://polyhaven.com/a/ceramic_vase_02) | 0.18 x 0.18 x 0.3 | auto | H06 |   |
| `VaseAntique` | polyhaven | [antique_ceramic_vase_01](https://polyhaven.com/a/antique_ceramic_vase_01) | 0.25 x 0.25 x 0.45 | auto | H05 |   |
| `VaseBrass` | polyhaven | [brass_vase_01](https://polyhaven.com/a/brass_vase_01) | 0.15 x 0.15 x 0.3 | auto | spare (not placed yet) |   |
| `CarvedPlate` | polyhaven | [carved_wooden_plate](https://polyhaven.com/a/carved_wooden_plate) | 0.35 x 0.03 x 0.35 | auto | spare (not placed yet) |   |
| `BooksDeco` | polyhaven | [decorative_book_set_01](https://polyhaven.com/a/decorative_book_set_01) | 0.4 x 0.2 x 0.25 | auto | H11 |   |
| `Binder` | polyhaven | [binder_notebook](https://polyhaven.com/a/binder_notebook) | 0.25 x 0.3 x 0.05 | auto | spare (not placed yet) |   |
| `Clipboard` | polyhaven | [clipboard](https://polyhaven.com/a/clipboard) | 0.23 x 0.32 x 0.02 | auto | spare (not placed yet) |   |
| `Notepads` | polyhaven | [office_notepads](https://polyhaven.com/a/office_notepads) | 0.25 x 0.2 x 0.04 | auto | spare (not placed yet) |   |
| `Stationery` | polyhaven | [stationery_supplies](https://polyhaven.com/a/stationery_supplies) | 0.3 x 0.2 x 0.15 | auto | spare (not placed yet) |   |
| `Stapler` | polyhaven | [vintage_stapler](https://polyhaven.com/a/vintage_stapler) | 0.18 x 0.05 x 0.08 | auto | H12 |   |
| `ChessSet` | polyhaven | [chess_set](https://polyhaven.com/a/chess_set) | 0.45 x 0.45 x 0.1 | auto | spare (not placed yet) |   |
| `Dartboard` | polyhaven | [dartboard](https://polyhaven.com/a/dartboard) | 0.45 x 0.04 x 0.45 | auto | spare (not placed yet) |   |
| `Chalkboard` | polyhaven | [standing_chalkboard_01](https://polyhaven.com/a/standing_chalkboard_01) | 1.0 x 0.5 x 1.5 | auto | H01, H03, village |   |
| `PocketWatch` | polyhaven | [pocket_watch](https://polyhaven.com/a/pocket_watch) | 0.06 x 0.02 x 0.08 | auto | H00 |   |
| `Spectacles` | polyhaven | [round_spectacles](https://polyhaven.com/a/round_spectacles) | 0.13 x 0.12 x 0.04 | auto | spare (not placed yet) |   |
| `CigarettePack` | polyhaven | [cigarette_pack](https://polyhaven.com/a/cigarette_pack) | 0.06 x 0.02 x 0.09 | auto | spare (not placed yet) |   |
| `Lighter` | polyhaven | [vintage_lighter](https://polyhaven.com/a/vintage_lighter) | 0.04 x 0.02 x 0.06 | auto | H00 |   |
| `Binoculars` | polyhaven | [binoculars](https://polyhaven.com/a/binoculars) | 0.18 x 0.12 x 0.07 | auto | spare (not placed yet) |   |
| `Magnifier` | polyhaven | [magnifying_glass_01](https://polyhaven.com/a/magnifying_glass_01) | 0.25 x 0.1 x 0.03 | auto | H00, H10 |   |
| `Microscope` | polyhaven | [vintage_microscope](https://polyhaven.com/a/vintage_microscope) | 0.2 x 0.25 x 0.35 | auto | H05 |   |
| `ChemistrySet` | polyhaven | [chemistry_set](https://polyhaven.com/a/chemistry_set) | 0.5 x 0.3 x 0.35 | auto | spare (not placed yet) |   |
| `BunsenBurner` | polyhaven | [bunsen_burner](https://polyhaven.com/a/bunsen_burner) | 0.12 x 0.12 x 0.2 | auto | H05 |   |
| `MedicalBox` | polyhaven | [medical_box](https://polyhaven.com/a/medical_box) | 0.35 x 0.2 x 0.25 | auto | H00 |   |
| `MedicalTape` | polyhaven | [medical_tape](https://polyhaven.com/a/medical_tape) | 0.05 x 0.05 x 0.03 | auto | spare (not placed yet) |   |
| `Crutches` | polyhaven | [vintage_crutches_01](https://polyhaven.com/a/vintage_crutches_01) | 0.4 x 0.15 x 1.3 | auto | H05 |   |
| `GasMask` | polyhaven | [old_gas_mask](https://polyhaven.com/a/old_gas_mask) | 0.2 x 0.2 x 0.25 | auto | spare (not placed yet) |   |
| `RubberBoots` | polyhaven | [rubber_boots](https://polyhaven.com/a/rubber_boots) | 0.3 x 0.3 x 0.4 | auto | H13 |   |
| `FishermansHat` | polyhaven | [fishermans_hat](https://polyhaven.com/a/fishermans_hat) | 0.35 x 0.35 x 0.12 | auto | spare (not placed yet) |   |
| `SpinningWheel` | polyhaven | [spinning_wheel_01](https://polyhaven.com/a/spinning_wheel_01) | 0.9 x 0.5 x 0.95 | auto | H04 |   |
| `HandSaw` | polyhaven | [handsaw_wood](https://polyhaven.com/a/handsaw_wood) | 0.65 x 0.03 x 0.18 | auto | spare (not placed yet) |   |
| `HandPlane` | polyhaven | [hand_plane_no4](https://polyhaven.com/a/hand_plane_no4) | 0.25 x 0.07 x 0.15 | auto | spare (not placed yet) |   |
| `BenchVice` | polyhaven | [bench_vice_01](https://polyhaven.com/a/bench_vice_01) | 0.3 x 0.2 x 0.2 | auto | H00 |   |
| `HandDrill` | polyhaven | [vintage_hand_drill](https://polyhaven.com/a/vintage_hand_drill) | 0.12 x 0.08 x 0.4 | auto | spare (not placed yet) |   |
| `DrillPress` | polyhaven | [old_drill_press](https://polyhaven.com/a/old_drill_press) | 0.5 x 0.6 x 1.6 | auto | spare (not placed yet) |   |
| `Crowbar` | polyhaven | [crowbar_01](https://polyhaven.com/a/crowbar_01) | 0.75 x 0.04 x 0.08 | auto | spare (not placed yet) |   |
| `Sledgehammer` | polyhaven | [sledgehammer_01](https://polyhaven.com/a/sledgehammer_01) | 0.25 x 0.08 x 0.9 | auto | spare (not placed yet) |   |
| `Hatchet` | polyhaven | [hatchet](https://polyhaven.com/a/hatchet) | 0.35 x 0.05 x 0.12 | auto | village |   |
| `Spade` | polyhaven | [rusted_spade_01](https://polyhaven.com/a/rusted_spade_01) | 0.2 x 0.08 x 1.1 | auto | H13, village |   |
| `Pickaxe` | polyhaven | [picke_dirty_01](https://polyhaven.com/a/picke_dirty_01) | 0.6 x 0.05 x 0.9 | auto | village |   |
| `Trowel` | polyhaven | [trowel_01](https://polyhaven.com/a/trowel_01) | 0.3 x 0.08 x 0.05 | auto | spare (not placed yet) |   |
| `GardenGloves` | polyhaven | [garden_gloves_01](https://polyhaven.com/a/garden_gloves_01) | 0.2 x 0.1 x 0.05 | auto | spare (not placed yet) |   |
| `Broom` | polyhaven | [wooden_broom](https://polyhaven.com/a/wooden_broom) | 0.3 x 0.1 x 1.3 | auto | H00 |   |
| `Dustpan` | polyhaven | [dustpan](https://polyhaven.com/a/dustpan) | 0.3 x 0.3 x 0.1 | auto | spare (not placed yet) |   |
| `Ladder` | polyhaven | [wooden_ladder](https://polyhaven.com/a/wooden_ladder) | 0.5 x 0.1 x 2.5 | auto | village |   |
| `Ladder2` | polyhaven | [wooden_ladder_02](https://polyhaven.com/a/wooden_ladder_02) | 0.5 x 0.1 x 3.0 | auto | spare (not placed yet) |   |
| `Pliers` | polyhaven | [pliers](https://polyhaven.com/a/pliers) | 0.2 x 0.05 x 0.02 | auto | H00 |   |
| `Wrench` | polyhaven | [adjustable_wrench](https://polyhaven.com/a/adjustable_wrench) | 0.3 x 0.07 x 0.02 | auto | spare (not placed yet) |   |
| `PipeWrench` | polyhaven | [pipe_wrench](https://polyhaven.com/a/pipe_wrench) | 0.35 x 0.1 x 0.03 | auto | spare (not placed yet) |   |
| `SmallOilCan` | polyhaven | [small_oil_can_01](https://polyhaven.com/a/small_oil_can_01) | 0.12 x 0.12 x 0.2 | auto | spare (not placed yet) |   |
| `Toolbox2` | polyhaven | [metal_toolbox](https://polyhaven.com/a/metal_toolbox) | 0.5 x 0.22 x 0.25 | auto | spare (not placed yet) |   |
| `BoltCutters` | polyhaven | [bolt_cutters_01](https://polyhaven.com/a/bolt_cutters_01) | 0.6 x 0.15 x 0.04 | auto | spare (not placed yet) |   |
| `HandTruck` | polyhaven | [hand_truck](https://polyhaven.com/a/hand_truck) | 0.5 x 0.5 x 1.2 | auto | H09, village |   |
| `Generator` | polyhaven | [portable_generator](https://polyhaven.com/a/portable_generator) | 0.7 x 0.5 x 0.55 | auto | village |   |
| `PropaneTank` | polyhaven | [propane_tank](https://polyhaven.com/a/propane_tank) | 0.35 x 0.35 x 0.8 | auto | village |   |
| `FuseBox` | polyhaven | [power_box_01](https://polyhaven.com/a/power_box_01) | 0.45 x 0.15 x 0.6 | auto | H00, H01, H02, H03, H04, H05, H06, H07, H08, H09, H10, H11, H12, H13, H14 |   |
| `UtilityBox` | polyhaven | [utility_box_01](https://polyhaven.com/a/utility_box_01) | 0.6 x 0.3 x 1.0 | auto | village |   |
| `PowerPoles` | polyhaven | [modular_electricity_poles](https://polyhaven.com/a/modular_electricity_poles) | 0.5 x 0.5 x 8.0 | auto | village |   |
| `GateLatch` | polyhaven | [gate_latch_01](https://polyhaven.com/a/gate_latch_01) | 0.15 x 0.06 x 0.1 | auto | village |   |
| `IronGate` | polyhaven | [large_iron_gate](https://polyhaven.com/a/large_iron_gate) | 3.0 x 0.2 x 2.5 | auto | village |   |
| `GardenHose` | polyhaven | [garden_hose_wall_mounted_01](https://polyhaven.com/a/garden_hose_wall_mounted_01) | 0.45 x 0.25 x 0.5 | auto | village |   |
| `Sprinkler` | polyhaven | [garden_sprinkler_01](https://polyhaven.com/a/garden_sprinkler_01) | 0.2 x 0.2 x 0.3 | auto | village |   |
| `GardenGnome` | polyhaven | [garden_gnome](https://polyhaven.com/a/garden_gnome) | 0.2 x 0.2 x 0.35 | auto | village |   |
| `FirePit` | polyhaven | [stone_fire_pit](https://polyhaven.com/a/stone_fire_pit) | 1.2 x 1.2 x 0.4 | auto | village |   |
| `CoveredCar` | polyhaven | [covered_car](https://polyhaven.com/a/covered_car) | 4.2 x 1.8 x 1.5 | auto | village |   |
| `OldTyre` | polyhaven | [old_tyre](https://polyhaven.com/a/old_tyre) | 0.65 x 0.2 x 0.65 | auto | village |   |
| `WheelRim` | polyhaven | [rusted_wheel_rim_01](https://polyhaven.com/a/rusted_wheel_rim_01) | 0.45 x 0.2 x 0.45 | auto | village |   |
| `BullHead` | polyhaven | [bull_head](https://polyhaven.com/a/bull_head) | 0.6 x 0.4 x 0.5 | auto | spare (not placed yet) |   |
| `HorseHead` | polyhaven | [horse_head](https://polyhaven.com/a/horse_head) | 0.3 x 0.5 x 0.5 | auto | spare (not placed yet) |   |
| `GothicStatue` | polyhaven | [gothic_statue](https://polyhaven.com/a/gothic_statue) | 0.6 x 0.6 x 1.8 | auto | H11 |   |
| `ThrowPillows` | polyhaven | [throw_pillows_01](https://polyhaven.com/a/throw_pillows_01) | 0.5 x 0.4 x 0.2 | auto | spare (not placed yet) |   |
| `RubberDuck` | polyhaven | [rubber_duck_toy](https://polyhaven.com/a/rubber_duck_toy) | 0.1 x 0.08 x 0.09 | auto | spare (not placed yet) |   |
| `TreasureChest` | polyhaven | [treasure_chest](https://polyhaven.com/a/treasure_chest) | 0.8 x 0.5 x 0.55 | auto | spare (not placed yet) |   |
| `DeadTrunk2` | polyhaven | [dead_tree_trunk_02](https://polyhaven.com/a/dead_tree_trunk_02) | 4.5 x 0.8 x 0.8 | auto | village |   |
| `Stump2` | polyhaven | [tree_stump_02](https://polyhaven.com/a/tree_stump_02) | 0.7 x 0.7 x 0.45 | auto | village |   |
| `Shrub1` | polyhaven | [shrub_01](https://polyhaven.com/a/shrub_01) | 1.2 x 1.2 x 1.0 | auto | village |   |
| `Shrub2` | polyhaven | [shrub_02](https://polyhaven.com/a/shrub_02) | 1.0 x 1.0 x 0.9 | auto | village |   |
| `Shrub3` | polyhaven | [shrub_03](https://polyhaven.com/a/shrub_03) | 1.4 x 1.4 x 1.2 | auto | village |   |
| `Shrub4` | polyhaven | [shrub_04](https://polyhaven.com/a/shrub_04) | 0.9 x 0.9 x 0.7 | auto | village |   |
| `Nettles` | polyhaven | [nettle_plant](https://polyhaven.com/a/nettle_plant) | 0.6 x 0.6 x 0.8 | auto | village |   |
| `Fern` | polyhaven | [fern_02](https://polyhaven.com/a/fern_02) | 0.8 x 0.8 x 0.6 | auto | village |   |
| `GrassClump` | polyhaven | [grass_medium_01](https://polyhaven.com/a/grass_medium_01) | 0.6 x 0.6 x 0.4 | auto | village |   |
| `GrassClump2` | polyhaven | [grass_medium_02](https://polyhaven.com/a/grass_medium_02) | 0.6 x 0.6 x 0.45 | auto | village |   |
| `Moss` | polyhaven | [moss_01](https://polyhaven.com/a/moss_01) | 0.5 x 0.5 x 0.05 | auto | village |   |
| `Dandelion` | polyhaven | [dandelion_01](https://polyhaven.com/a/dandelion_01) | 0.2 x 0.2 x 0.3 | auto | village |   |
| `Periwinkle` | polyhaven | [periwinkle_plant](https://polyhaven.com/a/periwinkle_plant) | 0.4 x 0.4 x 0.2 | auto | village |   |
| `FirTree` | polyhaven | [fir_tree_01](https://polyhaven.com/a/fir_tree_01) | 5.0 x 5.0 x 14.0 | auto | village | heavy - use LOD/impostor  |
| `PineTree` | polyhaven | [pine_tree_01](https://polyhaven.com/a/pine_tree_01) | 5.0 x 5.0 x 15.0 | auto | village | heavy - use LOD/impostor  |
| `SmallTree` | polyhaven | [tree_small_02](https://polyhaven.com/a/tree_small_02) | 3.0 x 3.0 x 5.0 | auto | village |   |
| `FirSapling` | polyhaven | [fir_sapling_medium](https://polyhaven.com/a/fir_sapling_medium) | 1.2 x 1.2 x 2.0 | auto | village |   |
| `PineSapling` | polyhaven | [pine_sapling_medium](https://polyhaven.com/a/pine_sapling_medium) | 1.2 x 1.2 x 2.0 | auto | village |   |
| `RootCluster` | polyhaven | [root_cluster_01](https://polyhaven.com/a/root_cluster_01) | 1.2 x 1.0 x 0.5 | auto | village |   |
| `BarkDebris` | polyhaven | [bark_debris_01](https://polyhaven.com/a/bark_debris_01) | 0.8 x 0.8 x 0.1 | auto | village |   |
| `RotaryPhone` | sketchfab | [812e8f59f9e749fab483eabf4800dec2](https://sketchfab.com/3d-models/812e8f59f9e749fab483eabf4800dec2) | 0.22 x 0.25 x 0.14 | auto | H00, H01, H02, H03, H04, H05, H06, H07, H08, H09, H10, H11, H12, H13, H14 | sara El-sisi. 'Rotary Phone', 22.6k faces. Every house has one (living room). Upgrade: Rewind Memory PRO retro rooms (Asset Store, URP) |
| `Typewriter` | sketchfab | [1d6009449fe84fd79454c4081eb743bf](https://sketchfab.com/3d-models/1d6009449fe84fd79454c4081eb743bf) | 0.4 x 0.35 x 0.2 | auto | H05, H12 | Kuba 'Volant' Wójcik. '1930s style typewriter - w/o branding', 22k faces  |
| `SewingMachine` | sketchfab | [db6aead9ecf947bcbde1f78d3e095fbe](https://sketchfab.com/3d-models/db6aead9ecf947bcbde1f78d3e095fbe) | 0.45 x 0.25 x 0.32 | auto | H02, H12, H14 | Giravolt / MEMGA. Singer c.1905 scan, 249k faces - decimate to ~25k on import  |
| `Bathtub` | sketchfab | [d06c028a89a54c469e4af9cac50606fa](https://sketchfab.com/3d-models/d06c028a89a54c469e4af9cac50606fa) | 1.65 x 0.75 x 0.62 | auto | H00, H01, H02, H04, H06, H08, H09, H10, H12, H13, H14 | Lady Lion Studios. 'Vintage Metal Bathtub FREE Low Poly', 4.1k faces  |
| `BathtubPhoto` | sketchfab | [ba60ba7d0a4a4b6899021741f450395b](https://sketchfab.com/3d-models/ba60ba7d0a4a4b6899021741f450395b) | 1.7 x 0.78 x 0.65 | auto | H03, H05, H07, H11 | Igrium. 'Photorealistic Bathtub', 19.7k faces - hero bath for Doctor/Priest/Twins  |
| `Toilet` | sketchfab | [6ac515a1c4154db18b5b4bd0b46d6405](https://sketchfab.com/3d-models/6ac515a1c4154db18b5b4bd0b46d6405) | 0.4 x 0.65 x 0.8 | auto | H00, H01, H02, H03, H04, H05, H06, H07, H08, H09, H10, H11, H12, H13, H14 | Allan-Jay Branscombe. 'Game Ready - Dirty Old Toilet', 7.6k faces  |
| `Sink` | sketchfab | [a278d63585494921906ab3500c421ff7](https://sketchfab.com/3d-models/a278d63585494921906ab3500c421ff7) | 0.55 x 0.45 x 0.85 | auto | H00, H01, H02, H03, H04, H05, H06, H07, H08, H09, H10, H11, H12, H13, H14 | lagesnpiet. 'Sink', 4.1k faces - check it reads as a pedestal basin; else build  |
| `Fridge` | sketchfab | [68d173f205344455933f861042b99fe3](https://sketchfab.com/3d-models/68d173f205344455933f861042b99fe3) | 0.6 x 0.62 x 1.45 | auto | H01, H03, H05, H08, H12 | dylanheyes. 'Old Fridge', 32.6k faces  |
| `FridgeSmall` | sketchfab | [7d12f05773a6403c9ca259ed43604c69](https://sketchfab.com/3d-models/7d12f05773a6403c9ca259ed43604c69) | 0.55 x 0.58 x 1.2 | auto | H00 | Chernorot Bogdan. 'USSR Old refrigerator', 9.4k faces  |
| `DollPorcelain` | sketchfab | [9ce09a8a6b1b45ce90c4479b54b4e003](https://sketchfab.com/3d-models/9ce09a8a6b1b45ce90c4479b54b4e003) | 0.2 x 0.12 x 0.45 | auto | H14 | alicean. 'A porcelain doll', 23k faces  |
| `DollAntique` | sketchfab | [a0a494a8c3db4cc4ad10cb433125fe78](https://sketchfab.com/3d-models/a0a494a8c3db4cc4ad10cb433125fe78) | 0.18 x 0.12 x 0.4 | auto | H14 | nektar.green. '3D Antique Doll', 29.9k faces  |
| `DollPolly` | sketchfab | [9ef34eb03f0a42978b3a9d20f0f8f389](https://sketchfab.com/3d-models/9ef34eb03f0a42978b3a9d20f0f8f389) | 0.18 x 0.12 x 0.35 | auto | H14 | archmage40. 'Porcelain Polly', 20k faces  |
| `Enlarger` | sketchfab | [3174dc4d4f9d4b3e969ce9135ec6d423](https://sketchfab.com/3d-models/3174dc4d4f9d4b3e969ce9135ec6d423) | 0.45 x 0.55 x 1.1 | auto | H10 | MarcoFromIADE. 'Axomat 5a Photo Enlarger', 77k faces  |
| `LoomDobby` | sketchfab | [07ba2b15ffb7425cab23f700828fd7f3](https://sketchfab.com/3d-models/07ba2b15ffb7425cab23f700828fd7f3) | 1.6 x 1.5 x 1.6 | auto | H04 | Spiacente. 'Dobby Loom', 128k faces - decimate to ~30k; fallback: build LoomFrame  |
| `Scarecrow` | sketchfab | [38b41b2deeb24b1d877c2551526d5654](https://sketchfab.com/3d-models/38b41b2deeb24b1d877c2551526d5654) | 1.2 x 0.4 x 2.0 | auto | H13, village | Mehdi Shahsavan. 'Scarecrow', 32.6k faces (alt: dd66376c24f246f28b10a67f047b8d6a by Baardus, 880 faces)  |
| `HayBales` | sketchfab | [6b775049e57e4d9da87977905493ebfb](https://sketchfab.com/3d-models/6b775049e57e4d9da87977905493ebfb) | 1.2 x 0.5 x 0.45 | auto | H13, village | FrodoUndead. 'Hay Bales', 3.3k faces  |
| `Wheelbarrow` | sketchfab | [48314bafaffc4593ba1d7a5af31810c6](https://sketchfab.com/3d-models/48314bafaffc4593ba1d7a5af31810c6) | 1.4 x 0.6 x 0.6 | auto | H13, village | Peter Nox. 'Wooden Wheelbarrow', 1.5k faces  |
| `ChurchPew` | sketchfab | [9c2934828dfa4dc2b64102defa3969e7](https://sketchfab.com/3d-models/9c2934828dfa4dc2b64102defa3969e7) | 3.0 x 0.6 x 0.95 | auto | H11, village | Sam Constance. 'old church pew', 948 faces (hero alt: 0fdeca01a1214150aeae21f46cfe98d5 awramos 40.8k)  |
| `RockingHorse` | sketchfab | [5168ce9d45a3401ea6fe4d92f17bb332](https://sketchfab.com/3d-models/5168ce9d45a3401ea6fe4d92f17bb332) | 0.9 x 0.35 x 0.7 | auto | H00, H05, H06 | Alex Petuhov. 'Rocking Toy', 16.8k faces  |
| `BabyCrib` | sketchfab | [b945651a5fa841b7aef4ea4da81fcc39](https://sketchfab.com/3d-models/b945651a5fa841b7aef4ea4da81fcc39) | 1.2 x 0.65 x 1.0 | auto | H06 | Rukas. 'Old Baby Crib', 4.2k faces  |
| `Globe` | sketchfab | [8bb166a68f70426390eb113ff0fd72a0](https://sketchfab.com/3d-models/8bb166a68f70426390eb113ff0fd72a0) | 0.4 x 0.4 x 0.55 | auto | H03, H05 | Allan-Jay Branscombe. 'Game Ready - Old World Globe', 26.8k faces  |
| `Cleaver` | sketchfab | [6c3fc11e570b4d3ca4d8a862f3b6a916](https://sketchfab.com/3d-models/6c3fc11e570b4d3ca4d8a862f3b6a916) | 0.32 x 0.03 x 0.12 | auto | H08 | Barend19. 'Meat clever', 3.2k faces  |
| `Bell` | sketchfab | [b2e02b7b964d4ee0b0e7292ed102270d](https://sketchfab.com/3d-models/b2e02b7b964d4ee0b0e7292ed102270d) | 1.2 x 1.2 x 1.2 | auto | village | LukeModels75. 'Classic Bell 3D Model', 32.6k faces - bell tower + school cupola (scaled)  |
| `AngelStatues` | sketchfab | [8ac9f760bc7e4931a491d56a57986199](https://sketchfab.com/3d-models/8ac9f760bc7e4931a491d56a57986199) | 0.8 x 0.6 x 1.6 | auto | village | That's Cool Jack. 'Scanned Angel Statue Collection', 7.6k faces - graveyard  |
| `KitchenUnit` | procedural | box carcass 0.6 deep, WoodKitchen fronts, laminate top | 0.6 x 0.6 x 0.9 | build | H01, H10 |   |
| `KitchenSinkUnit` | procedural | unit + ceramic butler sink 0.6x0.45 + brass taps | 1.0 x 0.6 x 0.9 | build | H00, H01, H02, H03, H04, H05, H06, H07, H08, H09, H10, H11, H12, H13, H14 |   |
| `Dresser` | procedural | Welsh dresser: base cupboard 0.5 deep + plate rack 0.25 deep, WoodCabinetWorn | 1.4 x 0.5 x 2.0 | build | H00, H02, H03, H04, H06, H07, H08, H09, H11, H13, H14 |   |
| `Range` | procedural | cast-iron range 1.0 wide set into the hearth opening, MetalPlate black | 1.0 x 0.55 x 0.85 | build | H05, H14 |   |
| `Hearth` | procedural | existing BuildHearth (HearthStone surround, mantel, Fire) - size per house | 1.8 x 0.7 x 1.4 | build | H00, H01, H02, H03, H04, H05, H06, H07, H08, H09, H10, H11, H12, H13, H14 |   |
| `Counter` | procedural | shop counter, panelled front, glass top optional | 2.4 x 0.6 x 0.95 | build | H00, H05, H08, H10, village |   |
| `Pigeonholes` | procedural | sorting frame 6 cols x 5 rows, 0.18 cells, WoodCabinetWorn | 1.2 x 0.35 x 1.9 | build | H12 |   |
| `PumpOrgan` | procedural | reed organ: case 1.25w 0.55d 1.1h + top gallery to 1.9h, 2 pedals, 61 keys, stops | 1.25 x 0.6 x 1.9 | build | H07 |  Upgrade: paid model e.g. 'Vintage Old Pump Pipe Organ' (glenfx, Cubebrush) |
| `LoomFrame` | procedural | fallback floor loom: 4 posts, warp beam, 8 harness frames, bench | 1.6 x 1.5 x 1.6 | build | H04 |   |
| `BalanceScale` | procedural | butcher's beam scale: cast pillar, 1.0 beam, 2 pans 0.35, brass weights 1/2/5/10/20 lb | 1.1 x 0.4 x 0.9 | build | H08 |   |
| `MeatRail` | procedural | ceiling rail 2.4 long with 8 S-hooks, 2 carcass shapes (cloth-wrapped) | 2.4 x 0.2 x 0.6 | build | H08 |   |
| `ButcherBlock` | procedural | end-grain block on legs 0.7x0.7x0.85, worn hollow top | 0.7 x 0.7 x 0.85 | build | H08 |   |
| `DarkroomTrays` | procedural | 3 enamel trays 0.3x0.4 on a sink bench + tongs + timer | 1.4 x 0.5 x 0.9 | build | H10 |   |
| `Safelight` | procedural | red safelight lamp, wall mounted | 0.2 x 0.15 x 0.2 | build | H10 |   |
| `HymnBoard` | procedural | dark oak hymn board, 3 rows of slide-in number cards | 0.5 x 0.05 x 0.9 | build | H11 |   |
| `Slate` | procedural | school slate in wooden frame 0.3x0.22 | 0.3 x 0.02 x 0.22 | build | H03 |   |
| `ChalkLetters` | procedural | scattered wooden letter tiles 0.06 cubes | 0.06 x 0.06 x 0.06 | build | spare (not placed yet) |   |
| `Blackboard` | procedural | wall blackboard 2.4x1.2 with chalk ledge | 2.4 x 0.08 x 1.2 | build | village |   |
| `MillGears` | procedural | puzzle gear train: 3 shafts, 5 oak gears (radii 0.25/0.35/0.45/0.55/0.65 m), lantern pinion | 2.5 x 1.0 x 2.0 | build | H09 |   |
| `Millstones` | procedural | pair of 1.2 m millstones in a wooden tun + hopper + shoe | 1.6 x 1.6 x 1.6 | build | H09 |   |
| `WaterWheel` | procedural | undershot wheel 4.0 m dia, 1.2 wide, 24 paddles, oak + iron rims | 1.2 x 4.0 x 4.0 | build | spare (not placed yet) |   |
| `Sacks` | procedural | stack of 4 hessian flour sacks (FabricHessian) | 1.0 x 0.6 x 0.7 | build | H04, H08, H09 |   |
| `Dollhouse` | procedural | 1:12 replica of H14 generated from houses/H14 json (shell + rooms + 12 tiny doll slots) | 0.75 x 0.55 x 0.8 | build | H14 |   |
| `MedicineCabinet` | procedural | glass-fronted wall cabinet 0.9x0.3x1.2, 4 shelves, padlock hasp | 0.9 x 0.3 x 1.2 | build | H05 |   |
| `MedicineBottles` | procedural | set of 12 brown/blue/clear bottles with paper labels (labels data-driven) | 0.6 x 0.15 x 0.2 | build | H05 |   |
| `ExamCouch` | procedural | leather examination couch with step | 1.9 x 0.7 x 0.75 | build | H05 |   |
| `Lectern` | procedural | oak lectern | 0.6 x 0.5 x 1.15 | build | H11, village |   |
| `Altar` | procedural | existing altar block | 3.8 x 1.3 x 1.0 | build | spare (not placed yet) |   |
| `ShrineNiche` | procedural | carved wall niche 0.6x0.25x0.9 with candle + diamond symbol; hidden unless shrine | 0.6 x 0.25 x 0.9 | build | H00, H01, H02, H03, H04, H05, H06, H07, H08, H09, H10, H11, H12, H13, H14 |   |
| `PartContainer` | procedural | existing PartContainer chest - per-house skin noted in HOUSES.md | 1.0 x 0.6 x 0.5 | build | spare (not placed yet) |   |
| `CoatHooks` | procedural | wall rail with 5 hooks + 2 coats + 1 hat (cloth) | 0.9 x 0.25 x 1.7 | build | H00, H01, H05 |   |
| `Curtains` | procedural | pair of cloth panels on a pole, per window, fabric per room | 1.4 x 0.08 x 1.8 | build | spare (not placed yet) |   |
| `Rug` | procedural | flat rug quad, fabric per room | 2.0 x 1.4 x 0.01 | build | H00, H01, H02, H03, H05, H07, H13, H14 |   |
| `Radiator` | procedural | cast-iron column radiator (only Doctor, Priest) | 0.9 x 0.2 x 0.7 | build | H05 |   |
| `WashStand` | procedural | marble-top wash stand with jug & basin | 0.8 x 0.45 x 0.85 | build | H05 |   |
| `Mangle` | procedural | cast-iron mangle (Widow scullery) | 0.8 x 0.45 x 1.3 | build | H02 |   |
| `Copper` | procedural | brick copper boiler in scullery corner | 0.8 x 0.8 x 0.9 | build | H02, H11, H13 |   |
| `BathCurtainRail` | procedural | rail + plastic curtain around bath (hiding spot) | 1.7 x 0.8 x 2.0 | build | H00, H01, H03, H05 |   |
| `UnderStairCupboard` | procedural | door + void under a stair (hiding spot) | 1.0 x 0.9 x 1.6 | build | H00 |   |
| `WalkInCupboard` | procedural | panelled cupboard door + 0.8x0.8 void | 0.9 x 0.8 x 2.0 | build | spare (not placed yet) |   |
| `CellarCrates` | procedural | stack of crates with a crouch gap behind (hiding spot) | 1.4 x 0.9 x 1.2 | build | H00, H01, H02, H04, H05, H06, H07, H09, H10, H14 |   |
| `LightSwitch` | procedural | existing LightSwitch plate | 0.09 x 0.03 x 0.13 | build | spare (not placed yet) |   |
| `StoppedClockFront` | procedural | existing front StoppedClock (hour from HouseCatalog) | 0.62 x 0.05 x 0.62 | build | spare (not placed yet) |   |
| `Nameplate` | procedural | existing nameplate with family name | 1.1 x 0.04 x 0.3 | build | spare (not placed yet) |   |
| `WeakWindowBoards` | procedural | weak window: thin frame, cracked pane, rot (WeakWindow component) | 1.0 x 0.1 x 1.2 | build | spare (not placed yet) |   |
| `PostBoxPillar` | procedural | red pillar post box, 1.5 h | 0.5 x 0.5 x 1.5 | build | village |   |
| `PhoneKiosk` | procedural | red telephone kiosk 1.0x1.0x2.5, dead line | 1.0 x 1.0 x 2.5 | build | village |   |
| `BusShelter` | procedural | corrugated bus shelter | 3.0 x 1.5 x 2.4 | build | village |   |
| `WarMemorial` | procedural | stone cross on 3 steps | 1.8 x 1.8 x 4.0 | build | village |   |
| `Gravestone` | procedural | headstone variants (5 profiles) in Stone/SandstoneBlocks, moss decal | 0.7 x 0.15 x 1.0 | build | village |  Upgrade: Modular Dark Fantasy Graveyard (Hivemind, $49.99, URP) |
| `FencePicket` | procedural | picket fence 2 m bay, painted, gaps | 2.0 x 0.08 x 1.0 | build | village |   |
| `FenceWattle` | procedural | woven wattle hurdle 2 m | 2.0 x 0.1 x 1.1 | build | village |   |
| `FenceRail` | procedural | post-and-rail 2.5 m bay | 2.5 x 0.1 x 1.2 | build | village |   |
| `DryStoneWall` | procedural | dry-stone wall 1.2 h, 0.6 thick, Stone material, coping stones | 2.0 x 0.6 x 1.2 | build | village |   |
| `GardenGate` | procedural | ledged gate 1.0 w with GateLatch | 1.0 x 0.08 x 1.1 | build | village |   |
| `WashingLine` | procedural | 2 posts + line + 3-6 cloth quads that move in wind | 6.0 x 0.3 x 2.0 | build | H04, village |   |
| `VegBeds` | procedural | raised beds with cabbage/bean-cane/marrow meshes (simple) | 3.0 x 1.2 x 0.8 | build | village |   |
| `Beehive` | procedural | WBC beehive | 0.55 x 0.55 x 0.9 | build | village |   |
| `Well` | procedural | village well: 2.4 dia stone drum, oak roof, winch, bucket | 2.6 x 2.6 x 3.2 | build | village |   |
| `Signpost` | procedural | finger signpost (MAIN ST / MILL LANE / CHAPEL LANE...) | 1.0 x 0.2 x 2.6 | build | village |   |
| `VintageCamera` | polyhaven | [Camera_01](https://polyhaven.com/a/Camera_01) | 0.18 x 0.14 x 0.16 | auto | H10 |   |
| `FilmProjector` | polyhaven | [filmstrip_projector_8mm](https://polyhaven.com/a/filmstrip_projector_8mm) | 0.3 x 0.4 x 0.3 | auto | spare (not placed yet) |   |
| `Backdrop` | procedural | studio backdrop: painted canvas (cloudy grey) 3.0 x 2.8 on a timber stand | 3.0 x 0.4 x 2.8 | build | H10 |   |
| `StudioLamp` | procedural | studio flood lamp on a tripod stand, 1.8 h | 0.5 x 0.5 x 1.8 | build | H10 |   |
| `IronSafe` | procedural | black cast-iron safe 0.6x0.6x0.8, brass handle, 4 code dials (CodeLockPuzzle dial slots) | 0.6 x 0.6 x 0.8 | build | H10, H12 |   |
| `SmallAltar` | procedural | oratory altar 1.6x0.7x1.0, oak, with the altar frontal (ALTAR CLOTH is the part) | 1.6 x 0.7 x 1.0 | build | H11 |   |
| `SortingTable` | procedural | post sorting table 2.0x0.9x0.9 with a raised rim | 2.0 x 0.9 x 0.9 | build | H12 |   |
| `MailSacks` | procedural | 3 canvas GPO mail sacks (FabricHessian, red stencil) | 1.0 x 0.7 x 0.6 | build | H12 |   |
| `LetterScale` | procedural | brass letter scale | 0.25 x 0.15 x 0.2 | build | H12 |   |
| `ScarecrowBoard` | procedural | barn wall board 1.6x1.2: painted map of the fields with 6 peg holes and 6 symbol pegs | 1.6 x 0.1 x 1.2 | build | H13 |   |
| `MilkChurns` | procedural | 3 steel milk churns | 0.9 x 0.4 x 0.7 | build | H13 |   |
| `CheesePress` | procedural | cast-iron cheese press | 0.6 x 0.5 x 1.4 | build | H13 |   |
| `DollParts` | procedural | workbench bins of porcelain heads, hands and glass eyes (data: 40 small meshes) | 0.6 x 0.4 x 0.25 | build | H14 |   |
| `DollCabinet` | procedural | glass-fronted display cabinet 1.2x0.4x1.9 with 18 dolls on 4 shelves | 1.2 x 0.4 x 1.9 | build | H14 |   |
| `FallenTree` | procedural | night blocker: DeadTrunk2 + branches across a lane (DESIGN 'one random path blocked') | 6.0 x 1.2 x 1.2 | build | village |   |
| `BrokenCart` | procedural | night blocker: hay cart on its side + CartWheel | 3.5 x 1.6 x 1.5 | build | village |   |

## Textures / materials
All PBR sets: colour, OpenGL normal, packed ARM (or AO/roughness/metal). 2K unless noted. URP Lit, tiling in metres (BuilderKit UVs are metric): walls 2.0 m, floors 1.5 m, roofs 2.0 m, fabrics 0.5 m.

| Key | Source | Id | Used for | Download | Used in |
|---|---|---|---|---|---|
| `HouseWall` | polyhaven | [grey_plaster](https://polyhaven.com/a/grey_plaster) | weathered exterior render | owned | village |
| `InnerWall` | polyhaven | [white_plaster_02](https://polyhaven.com/a/white_plaster_02) | plain interior plaster | owned | spare |
| `Door` | polyhaven | [wood_peeling_paint_weathered](https://polyhaven.com/a/wood_peeling_paint_weathered) | painted doors, tinted per house | owned | spare |
| `Planks` | polyhaven | [brown_planks_03](https://polyhaven.com/a/brown_planks_03) | old planks: fences, sheds | owned | village |
| `Floor` | ambientcg | [WoodFloor043](https://ambientcg.com/view?id=WoodFloor043) | worn knotted floorboards | owned | spare |
| `Roof` | polyhaven | [ceramic_roof_01](https://polyhaven.com/a/ceramic_roof_01) | mossy clay pantiles | owned | H00, H01, H03, H04, H05, H06, H07, H08, H09, H10, H11, H12, H13, H14, village |
| `ChapelRoof` | polyhaven | [roof_slates_03](https://polyhaven.com/a/roof_slates_03) | dark slate | owned | village |
| `Stone` | polyhaven | [stone_wall](https://polyhaven.com/a/stone_wall) | dry-stone / rubble walls | owned | H02, H03, H04, H07, H09, H11, H13, village |
| `ChapelWall` | polyhaven | [castle_wall_slates](https://polyhaven.com/a/castle_wall_slates) | coursed slate-stone walls | owned | village |
| `StandingStone` | ambientcg | [Rock034](https://ambientcg.com/view?id=Rock034) | mossy rock | owned | village |
| `Ground` | polyhaven | [forrest_ground_01](https://polyhaven.com/a/forrest_ground_01) | dry grass/twig ground | owned | H09, H12, village |
| `Road` | polyhaven | [brown_mud_leaves_01](https://polyhaven.com/a/brown_mud_leaves_01) | muddy lane | owned | village |
| `Paving` | polyhaven | [mossy_cobblestone](https://polyhaven.com/a/mossy_cobblestone) | square cobbles | owned | village |
| `Metal` | polyhaven | [rusty_metal_04](https://polyhaven.com/a/rusty_metal_04) | rusty metal | owned | spare |
| `HearthStone` | polyhaven | [stone_brick_wall_001](https://polyhaven.com/a/stone_brick_wall_001) | hearth stone | owned | spare |
| `Chimney` | polyhaven | [castle_brick_02_red](https://polyhaven.com/a/castle_brick_02_red) | old brick chimneys | owned | spare |
| `BookLeather` | polyhaven | [brown_leather](https://polyhaven.com/a/brown_leather) | leather | owned | spare |
| `Cloth` | polyhaven | [fabric_pattern_05](https://polyhaven.com/a/fabric_pattern_05) | old patterned cloth | owned | spare |
| `Bark` | polyhaven | [bark_brown_02](https://polyhaven.com/a/bark_brown_02) | bark | owned | spare |
| `RenderCream` | polyhaven | [beige_wall_001](https://polyhaven.com/a/beige_wall_001) | cream lime render (Blue Door, Twins, Postmaster) | auto | spare |
| `RenderOchre` | polyhaven | [yellow_plaster_02](https://polyhaven.com/a/yellow_plaster_02) | ochre render (Dollmaker upper floor) | auto | H14 |
| `RenderMossy` | polyhaven | [worn_mossy_plasterwall](https://polyhaven.com/a/worn_mossy_plasterwall) | damp mossy render, base of walls | auto | H09 |
| `RenderCracked` | polyhaven | [worn_cracked_plaster](https://polyhaven.com/a/worn_cracked_plaster) | cracked render (Widow's House) | auto | H02 |
| `Pebbledash` | polyhaven | [rough_plaster_03](https://polyhaven.com/a/rough_plaster_03) | rough-cast pebbledash (Rao, Butcher rear) | auto | H01 |
| `StuccoWhite` | polyhaven | [white_stucco_02](https://polyhaven.com/a/white_stucco_02) | white stucco (Doctor's villa) | auto | H05 |
| `PlasterStone` | polyhaven | [plaster_stone_wall_01](https://polyhaven.com/a/plaster_stone_wall_01) | half-rendered rubble (Weaver, Miller) | auto | H04 |
| `BrickRed` | polyhaven | [red_brick](https://polyhaven.com/a/red_brick) | red brick (Butcher, Photographer, Postmaster) | auto | H00, H01, H06, H08, H10, H12 |
| `BrickMixed` | polyhaven | [mixed_brick_wall](https://polyhaven.com/a/mixed_brick_wall) | patched brick (Schoolmaster rear, sheds) | auto | H00, H03, H08, H14 |
| `BrickPaintedWorn` | polyhaven | [painted_worn_brick](https://polyhaven.com/a/painted_worn_brick) | limewashed brick (Twins' side walls, bakery) | auto | H02, H06, H08, village |
| `BrickDark` | polyhaven | [dark_brick_wall](https://polyhaven.com/a/dark_brick_wall) | dark engineering brick (mill base, Organist plinth) | auto | H05, H07, H09, H10 |
| `StoneBlock` | polyhaven | [rough_block_wall](https://polyhaven.com/a/rough_block_wall) | squared rubble (Priest, Schoolmaster, Organist) | auto | H03, H07, H09, H11 |
| `SandstoneBlocks` | polyhaven | [sandstone_blocks_08](https://polyhaven.com/a/sandstone_blocks_08) | dressed sandstone quoins and sills | auto | H00, H03, H04, H07, H10, H11, H13 |
| `Weatherboard` | polyhaven | [exterior_wall_cladding](https://polyhaven.com/a/exterior_wall_cladding) | painted timber weatherboarding (Farmer barn, boat-sheds) | auto | H09, village |
| `RawPlankWall` | polyhaven | [raw_plank_wall](https://polyhaven.com/a/raw_plank_wall) | rough sawn plank walls (barns, sheds, mill upper) | auto | H00, H02, H03, H04, H06, H07, H09, H11, H12, H13, H14, village |
| `TimberBeam` | polyhaven | [beam_wall_01](https://polyhaven.com/a/beam_wall_01) | exposed timber framing (Dollmaker jetty, Weaver) | auto | H14 |
| `RoofSlateGrey` | polyhaven | [roof_slates_02](https://polyhaven.com/a/roof_slates_02) | grey slate | auto | H00, H03, H09, H10, H11 |
| `RoofTilesGrey` | polyhaven | [grey_roof_tiles](https://polyhaven.com/a/grey_roof_tiles) | grey concrete tiles (1960s re-roof) | auto | H01, village |
| `RoofClay2` | polyhaven | [clay_roof_tiles_02](https://polyhaven.com/a/clay_roof_tiles_02) | red clay plain tiles | auto | H08, H09, H12 |
| `RoofClay3` | polyhaven | [clay_roof_tiles_03](https://polyhaven.com/a/clay_roof_tiles_03) | weathered clay tiles | auto | H06, H14 |
| `RoofRedSlate` | polyhaven | [red_slate_roof_tiles_01](https://polyhaven.com/a/red_slate_roof_tiles_01) | red-brown slate (Organist, Doctor) | auto | H04, H05, H07, H13 |
| `Thatch` | polyhaven | [thatch_roof_angled](https://polyhaven.com/a/thatch_roof_angled) | thatch (Weaver, Widow) | auto | H02 |
| `ThatchReed` | polyhaven | [reed_roof_04](https://polyhaven.com/a/reed_roof_04) | reed thatch ridge/variant | auto | H02 |
| `CorrugatedRust` | polyhaven | [rusty_corrugated_iron](https://polyhaven.com/a/rusty_corrugated_iron) | rusty corrugated iron (lean-tos, sheds, byre) | auto | H02, H08, H13, village |
| `FloorOldWood1` | polyhaven | [old_wooden_floor_01](https://polyhaven.com/a/old_wooden_floor_01) | scuffed boards | auto | H00, H01, H02, H03, H04, H05, H06, H08, H09, H10, H11, H12, H13, H14 |
| `FloorOldWood2` | polyhaven | [old_wooden_floor_02](https://polyhaven.com/a/old_wooden_floor_02) | grey boards | auto | H00, H01, H02, H03, H04, H05, H06, H07, H09, H10, H11, H12, H13, H14 |
| `FloorDarkPlanks` | polyhaven | [dark_wooden_planks](https://polyhaven.com/a/dark_wooden_planks) | dark varnished boards | auto | H00, H03, H07, H11, H14 |
| `FloorParquet` | polyhaven | [diagonal_parquet](https://polyhaven.com/a/diagonal_parquet) | parquet (Doctor, Priest study) | auto | H01, H03, H05, H07, H10, H11, H12 |
| `FloorQuarryTile` | polyhaven | [terracotta_floor_tiles](https://polyhaven.com/a/terracotta_floor_tiles) | quarry tiles (kitchens, sculleries) | auto | H00, H01, H03, H04, H05, H06, H08, H10, H12 |
| `FloorCheckTile` | polyhaven | [checkered_pavement_tiles](https://polyhaven.com/a/checkered_pavement_tiles) | black/white checker (Doctor hall, Butcher shop) | auto | H05, H08 |
| `FloorBathTile` | polyhaven | [dirty_tiles](https://polyhaven.com/a/dirty_tiles) | small grubby tiles (bathrooms) | auto | H00, H01, H02, H03, H04, H05, H06, H07, H08, H09, H10, H11, H12, H13, H14 |
| `FloorWornTile` | polyhaven | [worn_tile_floor](https://polyhaven.com/a/worn_tile_floor) | worn patterned tiles (halls) | auto | H00, H01, H03, H05, H06, H08, H09, H10, H12 |
| `FloorFlagstone` | polyhaven | [slate_floor_03](https://polyhaven.com/a/slate_floor_03) | flagstones (cellars, Weaver, Miller) | auto | H02, H04, H07, H09, H11, H13, H14 |
| `FloorConcrete` | polyhaven | [concrete_floor_worn_001](https://polyhaven.com/a/concrete_floor_worn_001) | cellar/shed concrete | auto | H01, H05, H10, H12 |
| `FloorBrick` | polyhaven | [brick_floor_003](https://polyhaven.com/a/brick_floor_003) | brick floors (scullery, byre, dairy) | auto | H01, H02, H03, H04, H08, H11, H13, H14 |
| `FloorDirt` | polyhaven | [dirt_floor](https://polyhaven.com/a/dirt_floor) | earth floors (barn, mill wheel pit) | auto | H09, H13 |
| `CarpetWorn` | polyhaven | [dirty_carpet](https://polyhaven.com/a/dirty_carpet) | stained fitted carpet (1970s bedrooms) | auto | H00, H01, H05, H07, H08, H09, H10, H12 |
| `Carpet70s` | ambientcg | [Carpet016](https://ambientcg.com/view?id=Carpet016) | patterned 1970s carpet | auto | H01, H06, H08, H10, H12 |
| `Carpet70sB` | ambientcg | [Carpet014](https://ambientcg.com/view?id=Carpet014) | second carpet pattern | auto | H00, H01, H03, H05, H06, H08, H10, H11, H12, H13 |
| `Wallpaper1` | ambientcg | [Wallpaper001A](https://ambientcg.com/view?id=Wallpaper001A) | floral wallpaper (bedrooms) | auto | H00, H01, H03, H04, H05, H06, H07, H08, H09, H10, H12, H13, H14 |
| `Wallpaper1B` | ambientcg | [Wallpaper001B](https://ambientcg.com/view?id=Wallpaper001B) | floral wallpaper, other colourway | auto | H00, H02, H03, H05, H06, H07, H08, H09, H10, H11, H12, H14 |
| `Wallpaper2` | ambientcg | [Wallpaper002A](https://ambientcg.com/view?id=Wallpaper002A) | stripe/damask wallpaper (parlours) | auto | H00, H01, H03, H05, H06, H07, H10, H12 |
| `Wallpaper2C` | ambientcg | [Wallpaper002C](https://ambientcg.com/view?id=Wallpaper002C) | damask, faded colourway | auto | H00, H01, H03, H05, H07, H08, H09, H10, H11, H12, H13 |
| `WallpaperJacquard` | polyhaven | [floral_jacquard](https://polyhaven.com/a/floral_jacquard) | used as 1970s floral wallpaper (tint) | auto | H00, H03, H06, H12 |
| `WallpaperQuatrefoil` | polyhaven | [quatrefoil_jacquard_fabric](https://polyhaven.com/a/quatrefoil_jacquard_fabric) | used as geometric 1970s wallpaper | auto | H01, H08, H10 |
| `PlasterWorn` | polyhaven | [worn_plaster_wall](https://polyhaven.com/a/worn_plaster_wall) | stained interior plaster | auto | H00, H01, H02, H04, H05, H08, H09, H11, H13, H14 |
| `PlasterDamaged` | polyhaven | [damaged_plaster](https://polyhaven.com/a/damaged_plaster) | plaster with lath showing (damp corners) | auto | spare |
| `PanelDark` | polyhaven | [dark_paneled_wood](https://polyhaven.com/a/dark_paneled_wood) | dark wood panelling (Priest, Organist, Doctor) | auto | H00, H03, H05, H07, H10, H11, H12 |
| `TileWallWhite` | polyhaven | [interior_tiles](https://polyhaven.com/a/interior_tiles) | white wall tiles (bath, butcher shop, darkroom) | auto | H00, H01, H02, H03, H04, H05, H06, H07, H08, H09, H10, H11, H12, H13, H14 |
| `Whitewash` | polyhaven | [white_rough_plaster](https://polyhaven.com/a/white_rough_plaster) | limewash (cellars, byre, mill) | auto | H00, H01, H02, H03, H04, H05, H06, H07, H08, H09, H10, H11, H13, H14 |
| `CeilingInterior` | polyhaven | [ceiling_interior](https://polyhaven.com/a/ceiling_interior) | plaster ceilings with cracks | auto | H00, H01, H02, H03, H04, H05, H06, H07, H08, H09, H10, H11, H12, H13, H14 |
| `FabricLinen` | polyhaven | [rough_linen](https://polyhaven.com/a/rough_linen) | bedding, curtains | auto | spare |
| `FabricTerlenka` | polyhaven | [terlenka](https://polyhaven.com/a/terlenka) | 1970s curtain fabric | auto | spare |
| `FabricGingham` | polyhaven | [gingham_check](https://polyhaven.com/a/gingham_check) | kitchen curtains/tablecloths | auto | spare |
| `FabricVelvet` | polyhaven | [velour_velvet](https://polyhaven.com/a/velour_velvet) | parlour curtains, sofa | auto | spare |
| `FabricHessian` | polyhaven | [hessian_230](https://polyhaven.com/a/hessian_230) | sacks, blackout cloth | auto | spare |
| `FabricCorduroy` | polyhaven | [ribbed_corduroy](https://polyhaven.com/a/ribbed_corduroy) | armchairs | auto | spare |
| `Leather` | polyhaven | [fabric_leather_02](https://polyhaven.com/a/fabric_leather_02) | doctor's couch, bags | auto | spare |
| `MetalPlate` | polyhaven | [metal_plate](https://polyhaven.com/a/metal_plate) | fuse boxes, tins | auto | spare |
| `RustyMetal2` | polyhaven | [rusty_metal_02](https://polyhaven.com/a/rusty_metal_02) | rusted fittings, mill hardware | auto | spare |
| `GreenRust` | polyhaven | [green_metal_rust](https://polyhaven.com/a/green_metal_rust) | painted rusty metal (pillar box, gates) | auto | H00 |
| `WoodKitchen` | polyhaven | [kitchen_wood](https://polyhaven.com/a/kitchen_wood) | kitchen units, worktops | auto | spare |
| `WoodCabinetWorn` | polyhaven | [wood_cabinet_worn_long](https://polyhaven.com/a/wood_cabinet_worn_long) | cupboards, counters, pigeonholes | auto | spare |
| `WoodRoughPine` | polyhaven | [rough_pine_door](https://polyhaven.com/a/rough_pine_door) | ledged plank doors | auto | spare |
| `WoodMedieval` | polyhaven | [medieval_wood](https://polyhaven.com/a/medieval_wood) | mill gears, loom frame, beams | auto | spare |
| `GroundLeaves` | polyhaven | [leaves_forest_ground](https://polyhaven.com/a/leaves_forest_ground) | forest floor inside the wall | auto | village |
| `GroundGrassPath` | polyhaven | [grass_path_2](https://polyhaven.com/a/grass_path_2) | worn grass footpaths | auto | village |
| `GroundGravel` | polyhaven | [gravel_road](https://polyhaven.com/a/gravel_road) | gravel drives and the square edges | auto | village |
| `GroundMud` | polyhaven | [brown_mud_02](https://polyhaven.com/a/brown_mud_02) | yard mud, farmyard | auto | village |
| `GroundPark` | polyhaven | [park_dirt](https://polyhaven.com/a/park_dirt) | village green bare patches | auto | village |
| `Cobbles2` | polyhaven | [cobblestone_floor_08](https://polyhaven.com/a/cobblestone_floor_08) | Main Street setts | auto | village |
| `RockFloor` | polyhaven | [rock_embedded_floor](https://polyhaven.com/a/rock_embedded_floor) | yard paving / forecourts | auto | village |
| `PavingSlabs` | polyhaven | [concrete_pavement_02](https://polyhaven.com/a/concrete_pavement_02) | 1960s paving slabs (post office, school yard) | auto | village |

## Audio (already on your PC - Sonniss GDC 2026)
Root: `C:/dev/Hollowmere/AssetDownloads/Sonniss.com-GDC2026-GameAudioBundle`. Claude Code picks 2-6 files per need, trims/normalises with `tools/prepare-audio.ps1`, and lists them in `tools/audio-sources.json` (existing flow).

| Need key | Sonniss folder | Used for |
|---|---|---|
| clocks | 344 Audio - Antique Clocks | Blue Door ticking/chimes (H00), every mantel clock, the 12 chimes when the gate key clock opens |
| books | 344 Audio - Antique Books | page turns, books falling (Schoolmaster, Priest, Postmaster) |
| typewriter | 344 Audio - Antique Typewriter | Postmaster/Doctor typewriter keys (night change: types by itself) |
| telephone | 344 Audio - Antique Telephone | every rotary phone: ring, dial, pick-up (DESIGN 'The phone') |
| smallmetal | 344 Audio - Antique Small Metals | keys, latches, weights on the scale (Butcher), coins |
| ghostly | 344 Audio - Ghostly Presences Vol. 1 | hollow-house ambience, sanity events |
| haunting3 | 344 Audio - Haunting Ambiences Vol. 3 | Act 2/3 beds |
| haunting5 | 344 Audio - Haunting Ambiences Vol. 5 | cellars/attics |
| winds | 344 Audio - Extreme Winds Vol. 1 | the Hollow's approach gusts, bell tower |
| bells | Ivo Vicic - Church Bells | bell tower tolls (dusk 12, midnight count, dawn) |
| campfire | Ivo Vicic - Campfire - Bonfire FX | hearth fire loops (all 15 houses) |
| chimney | InMotionAudio - Chimney Wind | cold hearths of hollow houses |
| sinister4 | InMotionAudio - Sinister Textures 4 | build-up beds before scares |
| sinister5 | InMotionAudio - Sinister Textures 5 | build-up beds before scares |
| whistle | InMotionAudio - The Death Whistle | Butcher's bone whistle part |
| musicbox | Sonic Bat - Music Boxes | Dollmaker music box; lullaby source |
| vintageradio | Sonic Bat - Vintage Radio | radios/TV static in every house |
| stormy | Sonic Bat - Stormy Night Ambience | Act 3 rain/wind |
| interiorwind | Jake Fielding - Interior Wind Rain and Storms | inside houses during storms |
| fridgehum | Jake Fielding - Fridge Hums Vol.2 | 1970s fridges, post office exchange hum |
| locks | Epic Stock Media - HD Lock And Mechanism Sound Design Kit | every puzzle lock, part containers, fuse boxes |
| hauntedhouse | Epic Stock Media - Halloween Game - Haunted House and Horror Audio Scare Kit | doors, creaks, scare stingers (bell only per DESIGN - use sparingly as foley) |
| creatures | Epic Stock Media - Humanoid Creatures Vol 4 - Monstrous and Undead Creature Vocalization Sound Sets | the Hollow's breathing/joint clicks |
| rural | Epic Stock Media - Public Spaces - Storms Lakes Parks and Rural Nature Exteriors | day ambience, mill pond, fields |
| norway | Just Sound Effects - Highlands of Norway | wind over fields, forest edge |
| moaningmetal | The Noisery - Moaning Metal | mill gears/waterwheel strain (H09), iron gate |
| paper | Cinematic Sound Design - Paper Foley | letters (Postmaster puzzle), notes, photos |
| washing | InMotionAudio - Washing Basket Foley | wicker baskets, Weaver's thread spools |

## Procedural props (Claude Code builds these)
| Key | Shape spec | Size |
|---|---|---|
| `KitchenUnit` | box carcass 0.6 deep, WoodKitchen fronts, laminate top | 0.6 x 0.6 x 0.9 |
| `KitchenSinkUnit` | unit + ceramic butler sink 0.6x0.45 + brass taps | 1.0 x 0.6 x 0.9 |
| `Dresser` | Welsh dresser: base cupboard 0.5 deep + plate rack 0.25 deep, WoodCabinetWorn | 1.4 x 0.5 x 2.0 |
| `Range` | cast-iron range 1.0 wide set into the hearth opening, MetalPlate black | 1.0 x 0.55 x 0.85 |
| `Hearth` | existing BuildHearth (HearthStone surround, mantel, Fire) - size per house | 1.8 x 0.7 x 1.4 |
| `Counter` | shop counter, panelled front, glass top optional | 2.4 x 0.6 x 0.95 |
| `Pigeonholes` | sorting frame 6 cols x 5 rows, 0.18 cells, WoodCabinetWorn | 1.2 x 0.35 x 1.9 |
| `PumpOrgan` | reed organ: case 1.25w 0.55d 1.1h + top gallery to 1.9h, 2 pedals, 61 keys, stops | 1.25 x 0.6 x 1.9 |
| `LoomFrame` | fallback floor loom: 4 posts, warp beam, 8 harness frames, bench | 1.6 x 1.5 x 1.6 |
| `BalanceScale` | butcher's beam scale: cast pillar, 1.0 beam, 2 pans 0.35, brass weights 1/2/5/10/20 lb | 1.1 x 0.4 x 0.9 |
| `MeatRail` | ceiling rail 2.4 long with 8 S-hooks, 2 carcass shapes (cloth-wrapped) | 2.4 x 0.2 x 0.6 |
| `ButcherBlock` | end-grain block on legs 0.7x0.7x0.85, worn hollow top | 0.7 x 0.7 x 0.85 |
| `DarkroomTrays` | 3 enamel trays 0.3x0.4 on a sink bench + tongs + timer | 1.4 x 0.5 x 0.9 |
| `Safelight` | red safelight lamp, wall mounted | 0.2 x 0.15 x 0.2 |
| `HymnBoard` | dark oak hymn board, 3 rows of slide-in number cards | 0.5 x 0.05 x 0.9 |
| `Slate` | school slate in wooden frame 0.3x0.22 | 0.3 x 0.02 x 0.22 |
| `ChalkLetters` | scattered wooden letter tiles 0.06 cubes | 0.06 x 0.06 x 0.06 |
| `Blackboard` | wall blackboard 2.4x1.2 with chalk ledge | 2.4 x 0.08 x 1.2 |
| `MillGears` | puzzle gear train: 3 shafts, 5 oak gears (radii 0.25/0.35/0.45/0.55/0.65 m), lantern pinion | 2.5 x 1.0 x 2.0 |
| `Millstones` | pair of 1.2 m millstones in a wooden tun + hopper + shoe | 1.6 x 1.6 x 1.6 |
| `WaterWheel` | undershot wheel 4.0 m dia, 1.2 wide, 24 paddles, oak + iron rims | 1.2 x 4.0 x 4.0 |
| `Sacks` | stack of 4 hessian flour sacks (FabricHessian) | 1.0 x 0.6 x 0.7 |
| `Dollhouse` | 1:12 replica of H14 generated from houses/H14 json (shell + rooms + 12 tiny doll slots) | 0.75 x 0.55 x 0.8 |
| `MedicineCabinet` | glass-fronted wall cabinet 0.9x0.3x1.2, 4 shelves, padlock hasp | 0.9 x 0.3 x 1.2 |
| `MedicineBottles` | set of 12 brown/blue/clear bottles with paper labels (labels data-driven) | 0.6 x 0.15 x 0.2 |
| `ExamCouch` | leather examination couch with step | 1.9 x 0.7 x 0.75 |
| `Lectern` | oak lectern | 0.6 x 0.5 x 1.15 |
| `Altar` | existing altar block | 3.8 x 1.3 x 1.0 |
| `ShrineNiche` | carved wall niche 0.6x0.25x0.9 with candle + diamond symbol; hidden unless shrine | 0.6 x 0.25 x 0.9 |
| `PartContainer` | existing PartContainer chest - per-house skin noted in HOUSES.md | 1.0 x 0.6 x 0.5 |
| `CoatHooks` | wall rail with 5 hooks + 2 coats + 1 hat (cloth) | 0.9 x 0.25 x 1.7 |
| `Curtains` | pair of cloth panels on a pole, per window, fabric per room | 1.4 x 0.08 x 1.8 |
| `Rug` | flat rug quad, fabric per room | 2.0 x 1.4 x 0.01 |
| `Radiator` | cast-iron column radiator (only Doctor, Priest) | 0.9 x 0.2 x 0.7 |
| `WashStand` | marble-top wash stand with jug & basin | 0.8 x 0.45 x 0.85 |
| `Mangle` | cast-iron mangle (Widow scullery) | 0.8 x 0.45 x 1.3 |
| `Copper` | brick copper boiler in scullery corner | 0.8 x 0.8 x 0.9 |
| `BathCurtainRail` | rail + plastic curtain around bath (hiding spot) | 1.7 x 0.8 x 2.0 |
| `UnderStairCupboard` | door + void under a stair (hiding spot) | 1.0 x 0.9 x 1.6 |
| `WalkInCupboard` | panelled cupboard door + 0.8x0.8 void | 0.9 x 0.8 x 2.0 |
| `CellarCrates` | stack of crates with a crouch gap behind (hiding spot) | 1.4 x 0.9 x 1.2 |
| `LightSwitch` | existing LightSwitch plate | 0.09 x 0.03 x 0.13 |
| `StoppedClockFront` | existing front StoppedClock (hour from HouseCatalog) | 0.62 x 0.05 x 0.62 |
| `Nameplate` | existing nameplate with family name | 1.1 x 0.04 x 0.3 |
| `WeakWindowBoards` | weak window: thin frame, cracked pane, rot (WeakWindow component) | 1.0 x 0.1 x 1.2 |
| `PostBoxPillar` | red pillar post box, 1.5 h | 0.5 x 0.5 x 1.5 |
| `PhoneKiosk` | red telephone kiosk 1.0x1.0x2.5, dead line | 1.0 x 1.0 x 2.5 |
| `BusShelter` | corrugated bus shelter | 3.0 x 1.5 x 2.4 |
| `WarMemorial` | stone cross on 3 steps | 1.8 x 1.8 x 4.0 |
| `Gravestone` | headstone variants (5 profiles) in Stone/SandstoneBlocks, moss decal | 0.7 x 0.15 x 1.0 |
| `FencePicket` | picket fence 2 m bay, painted, gaps | 2.0 x 0.08 x 1.0 |
| `FenceWattle` | woven wattle hurdle 2 m | 2.0 x 0.1 x 1.1 |
| `FenceRail` | post-and-rail 2.5 m bay | 2.5 x 0.1 x 1.2 |
| `DryStoneWall` | dry-stone wall 1.2 h, 0.6 thick, Stone material, coping stones | 2.0 x 0.6 x 1.2 |
| `GardenGate` | ledged gate 1.0 w with GateLatch | 1.0 x 0.08 x 1.1 |
| `WashingLine` | 2 posts + line + 3-6 cloth quads that move in wind | 6.0 x 0.3 x 2.0 |
| `VegBeds` | raised beds with cabbage/bean-cane/marrow meshes (simple) | 3.0 x 1.2 x 0.8 |
| `Beehive` | WBC beehive | 0.55 x 0.55 x 0.9 |
| `Well` | village well: 2.4 dia stone drum, oak roof, winch, bucket | 2.6 x 2.6 x 3.2 |
| `Signpost` | finger signpost (MAIN ST / MILL LANE / CHAPEL LANE...) | 1.0 x 0.2 x 2.6 |
| `Backdrop` | studio backdrop: painted canvas (cloudy grey) 3.0 x 2.8 on a timber stand | 3.0 x 0.4 x 2.8 |
| `StudioLamp` | studio flood lamp on a tripod stand, 1.8 h | 0.5 x 0.5 x 1.8 |
| `IronSafe` | black cast-iron safe 0.6x0.6x0.8, brass handle, 4 code dials (CodeLockPuzzle dial slots) | 0.6 x 0.6 x 0.8 |
| `SmallAltar` | oratory altar 1.6x0.7x1.0, oak, with the altar frontal (ALTAR CLOTH is the part) | 1.6 x 0.7 x 1.0 |
| `SortingTable` | post sorting table 2.0x0.9x0.9 with a raised rim | 2.0 x 0.9 x 0.9 |
| `MailSacks` | 3 canvas GPO mail sacks (FabricHessian, red stencil) | 1.0 x 0.7 x 0.6 |
| `LetterScale` | brass letter scale | 0.25 x 0.15 x 0.2 |
| `ScarecrowBoard` | barn wall board 1.6x1.2: painted map of the fields with 6 peg holes and 6 symbol pegs | 1.6 x 0.1 x 1.2 |
| `MilkChurns` | 3 steel milk churns | 0.9 x 0.4 x 0.7 |
| `CheesePress` | cast-iron cheese press | 0.6 x 0.5 x 1.4 |
| `DollParts` | workbench bins of porcelain heads, hands and glass eyes (data: 40 small meshes) | 0.6 x 0.4 x 0.25 |
| `DollCabinet` | glass-fronted display cabinet 1.2x0.4x1.9 with 18 dolls on 4 shelves | 1.2 x 0.4 x 1.9 |
| `FallenTree` | night blocker: DeadTrunk2 + branches across a lane (DESIGN 'one random path blocked') | 6.0 x 1.2 x 1.2 |
| `BrokenCart` | night blocker: hay cart on its side + CartWheel | 3.5 x 1.6 x 1.5 |

## Paid upgrades for later (researched, prices from the store pages; check URP support + screenshots before buying)
- **Rewind Memory PRO: 80s/90s Retro Rooms** (EmacEArt, $54, URP+HDRP, Jan 2026) - 1970s-80s interior dressing for every house. https://assetstore.unity.com/packages/3d/props/interior/1980-midpoly-retro-rooms-childroomupdate2021-part-iii-177436
- **Complete Home Interior Pack** (Nekobolt, $20, Built-in + URP) - kitchens, bathrooms, furniture. https://marketplace.unity.com/packages/3d/props/interior/31049
- **Modular Dark Fantasy Graveyard / Cemetery** (Hivemind, $49.99, URP+HDRP) - graveyard, chapel dressing. https://marketplace.unity.com/packages/3d/environments/fantasy/modular-dark-fantasy-graveyard-cemetery-horror-cemetery-horror-g-300192
- **Detailed Old Wooden Houses and Shacks Pack** (ANRUVAL, $24.99) - sheds, barns, boat-houses, filler. https://marketplace.unity.com/packages/3d/environments/historic/detailed-old-wooden-houses-and-shacks-pack-175718
- **Medieval Nordic Village Environment** (BEFOUR, $64.99, URP+HDRP) - only for the oldest buildings (H14, H02) and forest. https://marketplace.unity.com/packages/3d/environments/historic/medieval-nordic-village-environment-279881
- **Abandoned Horror Mansion Interior (Unity)** (Leartes, $89.99, 160 assets; pipeline not stated - ask the seller) - decayed interiors. https://leartesstudios.gumroad.com/l/horrormansion_unity
- **NatureManufacture forest / R.A.M. river packs** (Built-in/URP/HDRP) - the forest belt and the stream. https://blog.unity.com/en/games/create-high-fidelity-worlds-with-naturemanufacture
- **Fab premium Megascans collections** (e.g. 'Medieval Farm', 'European Broadleaf Forest') - photoscanned hero props; Fab Standard License allows any engine. https://quixel.com/news/quixel-on-fab-new-megascans-and-megaplants
- **A pump organ / harmonium hero model** (e.g. 'Vintage Old Pump Pipe Organ', Cubebrush) to replace the procedural PumpOrgan. https://cubebrush.co/glenfx/products/0ydxa/vintage-old-pump-pipe-organ
