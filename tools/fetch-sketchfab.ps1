<#
.SYNOPSIS
  Downloads the CC-BY Sketchfab models listed in tools/sketchfab.json into Assets/ThirdParty/Sketchfab/<key>/.

.DESCRIPTION
  Sketchfab needs an account to download. Two ways:

  1. Automatic (recommended). Make a free account on sketchfab.com, open Settings -> Password & API, copy the
     "API token", and save it ONLY on this PC in a file outside the repo:
         %USERPROFILE%\.hollowmere\sketchfab_token.txt
     (or set the SKETCHFAB_TOKEN environment variable). Never commit the token.
     The script then calls GET https://api.sketchfab.com/v3/models/<uid>/download and saves the .glb
     (or the glTF zip when no .glb is offered).

  2. Manual. Open each model's URL (listed when the script runs without a token, and in docs/design/ASSETS.md),
     press Download -> "glTF Binary (.glb)" (or "glTF" zip), and save it as
         C:\dev\Hollowmere\AssetDownloads\Sketchfab\<key>.glb   (or <key>.zip)
     The script copies whatever it finds there.

  Every model folder gets a source.json (name, author, URL, licence CC BY 4.0) so tools/write-credits.ps1 credits it.
  Unity imports .glb/.gltf through the glTFast package (com.unity.cloud.gltfast, added by BatchTools.RequiredPackages).
  Raw downloads stay in AssetDownloads (never committed). Files already present are skipped unless -Force.

.EXAMPLE
  powershell -NoProfile -ExecutionPolicy Bypass -File tools/fetch-sketchfab.ps1
  powershell -NoProfile -ExecutionPolicy Bypass -File tools/fetch-sketchfab.ps1 -Only RotaryPhone,Bathtub
#>
param([switch]$Force, [string[]]$Only)

$ErrorActionPreference = 'Stop'
$ProgressPreference = 'SilentlyContinue'
$Root = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$Manifest = Get-Content (Join-Path $PSScriptRoot 'sketchfab.json') -Raw | ConvertFrom-Json
$Downloads = Join-Path (Split-Path $Root) 'AssetDownloads\Sketchfab'
if (-not (Test-Path (Split-Path $Downloads))) { $Downloads = Join-Path $Root 'AssetDownloads\Sketchfab' }
$Dest = Join-Path $Root 'Assets\ThirdParty\Sketchfab'
New-Item -ItemType Directory -Force -Path $Downloads, $Dest | Out-Null

$token = $env:SKETCHFAB_TOKEN
$tokenFile = Join-Path $env:USERPROFILE '.hollowmere\sketchfab_token.txt'
if (-not $token -and (Test-Path $tokenFile)) { $token = (Get-Content $tokenFile -Raw).Trim() }

function Save-Download([string]$url, [string]$path) {
    $part = "$path.part"
    for ($attempt = 1; ; $attempt++) {
        try { Invoke-WebRequest -Uri $url -OutFile $part -UseBasicParsing; break }
        catch {
            if (Test-Path $part) { Remove-Item $part -Force }
            if ($attempt -ge 3) { throw }
            Start-Sleep -Seconds (5 * $attempt)
        }
    }
    Move-Item $part $path -Force
}

$ok = 0; $missing = @()
foreach ($m in $Manifest.models) {
    if ($Only -and ($Only -notcontains $m.key)) { continue }
    $dir = Join-Path $Dest $m.key
    $glb = Join-Path $dir "$($m.key).glb"
    $gltfDir = Join-Path $dir 'gltf'
    if (-not $Force -and ((Test-Path $glb) -or (Test-Path $gltfDir))) { $ok++; continue }

    $rawGlb = Join-Path $Downloads "$($m.key).glb"
    $rawZip = Join-Path $Downloads "$($m.key).zip"
    foreach ($alt in @("$($m.uid).glb", "$($m.uid).zip")) {
        $p = Join-Path $Downloads $alt
        if (Test-Path $p) { if ($alt -like '*.glb') { $rawGlb = $p } else { $rawZip = $p } }
    }

    if (-not (Test-Path $rawGlb) -and -not (Test-Path $rawZip) -and $token) {
        Write-Host "Downloading $($m.key) ($($m.uid))..."
        try {
            $info = Invoke-RestMethod -Uri "https://api.sketchfab.com/v3/models/$($m.uid)/download" `
                                      -Headers @{ Authorization = "Token $token" }
            if ($info.glb) { Save-Download $info.glb.url $rawGlb }
            elseif ($info.gltf) { Save-Download $info.gltf.url $rawZip }
            else { Write-Warning "  $($m.key): no glb/gltf offered" }
        } catch {
            Write-Warning "  $($m.key): API download failed ($($_.Exception.Message)). Download it by hand: $($m.url)"
        }
    }

    if (Test-Path $rawGlb) {
        New-Item -ItemType Directory -Force -Path $dir | Out-Null
        Copy-Item $rawGlb $glb -Force
    } elseif (Test-Path $rawZip) {
        New-Item -ItemType Directory -Force -Path $gltfDir | Out-Null
        Expand-Archive -Path $rawZip -DestinationPath $gltfDir -Force
    } else {
        $missing += $m; continue
    }
    [ordered]@{
        id = $m.uid; name = $m.key; kind = 'model'; key = $m.key; use = $m.note
        authors = @($m.author); source = 'Sketchfab'; url = $m.url; licence = $m.licence
        target_size_m = $m.target_size
    } | ConvertTo-Json -Depth 4 | Set-Content -Path (Join-Path $dir 'source.json') -Encoding utf8
    $ok++
}

Write-Host "Sketchfab: $ok model(s) in place, $($missing.Count) missing."
if ($missing.Count) {
    if (-not $token) { Write-Host "No API token found (see the header of this script). Manual downloads:" }
    foreach ($m in $missing) { Write-Host ("  {0,-16} {1}  -> save as AssetDownloads\Sketchfab\{0}.glb" -f $m.key, $m.url) }
    exit 2
}
exit 0
