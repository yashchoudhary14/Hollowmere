<#
.SYNOPSIS
  Imports only the selected files from the user's Asset Store and Fab .unitypackage files (tools/assetstore-imports.json).

.DESCRIPTION
  A .unitypackage is a gzipped tar of <guid>/{asset, asset.meta, pathname}. This reads every item's pathname,
  extracts only the items the manifest selects, and writes them where the manifest says (AssetStore/ or Fab/):
  - FBX meshes and WAV clips are copied as they are.
  - Textures are written as JPGs of at most 2048 px in the ThirdParty naming scheme (diff, nor_gl, arm), so the
    ThirdParty import rules and material builder treat them like the Poly Haven sets:
      _BaseColor / _Normal                      -> <name>_diff_2k.jpg / <name>_nor_gl_2k.jpg
      HDRP _MaskMap (metal, AO, detail, smooth) -> <name>_arm_2k.jpg (AO, 1 - smoothness, metal)
  - Megascans packs ("megascans": true) are read through their materials instead: each selected M_*.mat names its
    _Albedo, _Normal and _DR textures. The Quixel shader uses only _DR's red channel, as roughness remapped from
    _MinRoughness to _MaxRoughness, and multiplies albedo by _AlbedoTint * _Brightness. Both are baked in here:
      _Albedo * tint * brightness -> <name>_diff_2k.jpg
      _Normal                     -> <name>_nor_gl_2k.jpg
      _DR                         -> <name>_arm_2k.jpg (AO 1, roughness lerp(min, max, R), metal 0)
  The vendor's .meta files, materials and prefabs (built for HDRP) are not imported: ThirdPartyImport sets every
  import option and builds URP materials. A source.json per folder feeds CREDITS.md.

.EXAMPLE
  powershell -NoProfile -ExecutionPolicy Bypass -File tools/import-assetstore.ps1 -Force
#>
param([switch]$Force)

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$Root = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$Downloads = [IO.Path]::GetFullPath((Join-Path $Root '..\AssetDownloads'))
$Manifest = Get-Content (Join-Path $PSScriptRoot 'assetstore-imports.json') -Raw | ConvertFrom-Json
$Tar = Join-Path $env:SystemRoot 'System32\tar.exe'
$Work = Join-Path ([IO.Path]::GetTempPath()) 'hollowmere-assetstore'
$Inv = [Globalization.CultureInfo]::InvariantCulture

Add-Type -ReferencedAssemblies System.Drawing -TypeDefinition @'
using System; using System.Drawing; using System.Drawing.Imaging; using System.Runtime.InteropServices;
public static class Channels
{
    // Calls f(r, g, b, a) for every pixel of src and writes the RGB it returns.
    delegate void Pixel(byte[] s, int si, byte[] d, int di);
    static Bitmap Map(Bitmap src, Pixel f)
    {
        var rect = new Rectangle(0, 0, src.Width, src.Height);
        var s = src.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        var dst = new Bitmap(src.Width, src.Height, PixelFormat.Format24bppRgb);
        var d = dst.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
        var sb = new byte[s.Stride * src.Height]; Marshal.Copy(s.Scan0, sb, 0, sb.Length);
        var db = new byte[d.Stride * src.Height];
        for (int y = 0; y < src.Height; y++)
            for (int x = 0; x < src.Width; x++)
                f(sb, y * s.Stride + x * 4, db, y * d.Stride + x * 3);   // BGRA in, BGR out
        Marshal.Copy(db, 0, d.Scan0, db.Length);
        src.UnlockBits(s); dst.UnlockBits(d);
        return dst;
    }
    static byte B(float v) { return (byte)Math.Max(0, Math.Min(255, (int)Math.Round(v))); }

    // HDRP mask map (R metal, G AO, B detail, A smoothness) to ARM (R AO, G roughness, B metal).
    public static Bitmap MaskToArm(Bitmap src)
    {
        return Map(src, (s, i, d, o) => { d[o + 2] = s[i + 1]; d[o + 1] = (byte)(255 - s[i + 3]); d[o] = s[i + 2]; });
    }
    // Megascans _DR (roughness in R, remapped min..max) to ARM with no occlusion and no metal.
    public static Bitmap RoughToArm(Bitmap src, float min, float max)
    {
        return Map(src, (s, i, d, o) => { d[o + 2] = 255; d[o + 1] = B(255f * Clamp01(min + (max - min) * s[i + 2] / 255f)); d[o] = 0; });
    }
    // Colour times a tint (linear multiply on the stored values, as the shader graph does).
    public static Bitmap Tint(Bitmap src, float r, float g, float b)
    {
        return Map(src, (s, i, d, o) => { d[o + 2] = B(s[i + 2] * r); d[o + 1] = B(s[i + 1] * g); d[o] = B(s[i] * b); });
    }
    static float Clamp01(float v) { return v < 0 ? 0 : v > 1 ? 1 : v; }
}
'@

function Save-Jpeg([System.Drawing.Bitmap]$bmp, [string]$path) {
    $max = 2048
    if ($bmp.Width -gt $max -or $bmp.Height -gt $max) {
        $scale = $max / [math]::Max($bmp.Width, $bmp.Height)
        $small = New-Object System.Drawing.Bitmap ([int]($bmp.Width * $scale)), ([int]($bmp.Height * $scale))
        $g = [System.Drawing.Graphics]::FromImage($small)
        $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
        $g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
        $g.DrawImage($bmp, 0, 0, $small.Width, $small.Height); $g.Dispose()
        $bmp = $small
    }
    $codec = [System.Drawing.Imaging.ImageCodecInfo]::GetImageEncoders() | Where-Object MimeType -eq 'image/jpeg'
    $params = New-Object System.Drawing.Imaging.EncoderParameters 1
    $params.Param[0] = New-Object System.Drawing.Imaging.EncoderParameter ([System.Drawing.Imaging.Encoder]::Quality), 92L
    New-Item -ItemType Directory -Force -Path (Split-Path $path) | Out-Null
    $bmp.Save($path, $codec, $params)
}

# Loads a PNG, transforms it and saves it as a JPG, unless the JPG exists (and -Force is off).
function Write-Map([string]$png, [string]$path, [scriptblock]$transform) {
    if ((Test-Path $path) -and -not $Force) { return }
    $bmp = [System.Drawing.Bitmap]::FromFile($png)
    try {
        $img = & $transform $bmp
        Save-Jpeg $img $path
        "   {0}  ({1}x{2})" -f (Split-Path $path -Leaf), $bmp.Width, $bmp.Height
    } finally { $bmp.Dispose() }
}

function Get-Name([string]$folder) { $folder.ToLowerInvariant() -replace '[^a-z0-9]+', '_' }

# Reads the texture GUIDs and the few scalar and colour properties the Quixel shader graph uses.
function Read-QuixelMaterial([string]$path) {
    $yaml = Get-Content $path -Raw
    $tex = { param($n) if ($yaml -match "- _${n}:\s*\r?\n\s*m_Texture: \{fileID: \d+, guid: ([0-9a-f]{32})") { $Matches[1] } }
    $num = { param($n, $def) if ($yaml -match "- _${n}: (-?[0-9.eE+-]+)") { [double]::Parse($Matches[1], $Inv) } else { $def } }
    $tint = if ($yaml -match '- _AlbedoTint: \{r: ([0-9.]+), g: ([0-9.]+), b: ([0-9.]+)') {
        @([double]::Parse($Matches[1], $Inv), [double]::Parse($Matches[2], $Inv), [double]::Parse($Matches[3], $Inv))
    } else { @(1.0, 1.0, 1.0) }
    [pscustomobject]@{
        Albedo = & $tex 'Albedo'; Normal = & $tex 'Normal'; Rough = & $tex 'DR'
        MinRough = & $num 'MinRoughness' 0.0; MaxRough = & $num 'MaxRoughness' 1.0
        Brightness = & $num 'Brightness' 1.0; Tint = $tint
    }
}

foreach ($pack in $Manifest.packs) {
    $pkg = Join-Path $Downloads $pack.file
    if (-not (Test-Path $pkg)) { Write-Warning "missing package: $pkg"; continue }
    Write-Host "== $($pack.name)"
    $tmp = Join-Path $Work ([IO.Path]::GetFileNameWithoutExtension($pack.file))
    if (Test-Path $tmp) { Remove-Item $tmp -Recurse -Force }
    New-Item -ItemType Directory -Force -Path $tmp | Out-Null

    # 1. Every item's path inside the package.
    & $Tar -xzf $pkg -C $tmp '*/pathname' 2>$null
    $all = @{}
    Get-ChildItem $tmp -Directory | ForEach-Object {
        $all[$_.Name] = (Get-Content (Join-Path $_.FullName 'pathname') -TotalCount 1).Trim()
    }
    $items = $all.GetEnumerator() | Where-Object { $_.Value -match $pack.select -and -not ($pack.skip -and $_.Value -match $pack.skip) } |
        ForEach-Object { [pscustomobject]@{ Guid = $_.Key; Path = $_.Value } }
    Write-Host "   selected $(@($items).Count) items"

    # 2. Extract only those (and, for Megascans, the textures their materials name).
    $extract = { param($guids)
        $list = Join-Path $tmp 'members.txt'
        $guids | ForEach-Object { "$_/asset" } | Set-Content -Path $list -Encoding ascii
        & $Tar -xzf $pkg -C $tmp -T $list
    }
    & $extract ($items | ForEach-Object Guid)
    $mats = @{}
    if ($pack.megascans) {
        foreach ($it in $items | Where-Object { $_.Path -like '*.mat' }) { $mats[$it.Path] = Read-QuixelMaterial (Join-Path $tmp "$($it.Guid)/asset") }
        & $extract ($mats.Values | ForEach-Object { $_.Albedo; $_.Normal; $_.Rough } | Where-Object { $_ } | Sort-Object -Unique)
    }

    # 3. Copy or convert into the project.
    foreach ($it in $items) {
        $asset = Join-Path $tmp "$($it.Guid)/asset"
        $rel = $it.Path.Substring($pack.strip.Length)
        $outDir = Join-Path $Root (Join-Path $pack.out (Split-Path $rel -Parent))
        $leaf = Split-Path $rel -Leaf
        $name = Get-Name ($rel.Split('/')[0])
        if ($leaf -match '\.(fbx|wav)$') {
            $dst = Join-Path $outDir $leaf
            if (-not (Test-Path $dst) -or $Force) { New-Item -ItemType Directory -Force -Path $outDir | Out-Null; Copy-Item $asset $dst -Force }
        } elseif ($leaf -match '\.mat$') {
            $m = $mats[$it.Path]
            $png = { param($guid) Join-Path $tmp "$guid/asset" }
            $t = $m.Tint; $k = $m.Brightness
            if ($m.Albedo) { Write-Map (& $png $m.Albedo) (Join-Path $outDir "${name}_diff_2k.jpg") { param($b) if ($t[0] * $k -eq 1 -and $t[1] * $k -eq 1 -and $t[2] * $k -eq 1) { $b } else { [Channels]::Tint($b, $t[0] * $k, $t[1] * $k, $t[2] * $k) } } }
            if ($m.Normal) { Write-Map (& $png $m.Normal) (Join-Path $outDir "${name}_nor_gl_2k.jpg") { param($b) $b } }
            $lo = $m.MinRough; $hi = $m.MaxRough
            if ($m.Rough) { Write-Map (& $png $m.Rough) (Join-Path $outDir "${name}_arm_2k.jpg") { param($b) [Channels]::RoughToArm($b, $lo, $hi) } }
        } elseif ($leaf -match '_BaseColor\.png$') {
            Write-Map $asset (Join-Path $outDir "${name}_diff_2k.jpg") { param($b) $b }
        } elseif ($leaf -match '_Normal\.png$') {
            Write-Map $asset (Join-Path $outDir "${name}_nor_gl_2k.jpg") { param($b) $b }
        } elseif ($leaf -match '_MaskMap\.png$') {
            Write-Map $asset (Join-Path $outDir "${name}_arm_2k.jpg") { param($b) [Channels]::MaskToArm($b) }
        }
    }

    # 4. Credits: one source.json per top-level folder.
    Get-ChildItem (Join-Path $Root $pack.out) -Directory | ForEach-Object {
        [ordered]@{ id = $_.Name; name = $_.Name; kind = 'package'; pack = $pack.name; use = $pack.use
                    source = $pack.source; url = $pack.url; licence = $pack.licence } |
            ConvertTo-Json | Set-Content (Join-Path $_.FullName 'source.json') -Encoding utf8
    }
    Remove-Item $tmp -Recurse -Force
}
$total = (Get-ChildItem (Join-Path $Root 'Assets/ThirdParty/AssetStore') -Recurse -File -Exclude '*.meta' | Measure-Object Length -Sum).Sum
Write-Host ("Assets/ThirdParty/AssetStore now holds {0:N1} MB." -f ($total / 1MB))
