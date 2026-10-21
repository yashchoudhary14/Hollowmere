<#
.SYNOPSIS
  Downloads the CC0 assets listed in tools/third-party.json (Poly Haven, ambientCG) into Assets/ThirdParty.

.DESCRIPTION
  One folder per asset: Assets/ThirdParty/PolyHaven/<id>/ and Assets/ThirdParty/AmbientCG/<id>/.
  Only the maps the game uses are kept: colour, OpenGL normal, packed AO/rough/metal (or the separate maps when
  there is no packed one) and alpha. No EXR, no displacement. Each folder gets a source.json (name, authors, URL,
  licence) that CREDITS.md is generated from. Files already present are skipped, so re-running is cheap.

.EXAMPLE
  powershell -NoProfile -ExecutionPolicy Bypass -File tools/fetch-assets.ps1
#>
param([switch]$Force)

$ErrorActionPreference = 'Stop'
$ProgressPreference = 'SilentlyContinue'
$Root = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$Manifest = Get-Content (Join-Path $PSScriptRoot 'third-party.json') -Raw | ConvertFrom-Json
$wc = New-Object System.Net.WebClient
$wc.Headers.Add('User-Agent', 'Hollowmere-fetch-assets')
$script:bytes = 0

function Get-File([string]$url, [string]$path) {
    if ((Test-Path $path) -and -not $Force) { return }
    New-Item -ItemType Directory -Force -Path (Split-Path $path) | Out-Null
    # Download to .part and rename when complete, so an interrupted run never leaves a file that looks finished.
    $part = "$path.part"
    for ($attempt = 1; ; $attempt++) {
        try {
            $wc.DownloadFile($url, $part)
            break
        } catch {
            if (Test-Path $part) { Remove-Item $part -Force }
            if ($attempt -ge 3) { throw }
            Write-Host "  retrying $(Split-Path $url -Leaf) ($($_.Exception.Message))"
            Start-Sleep -Seconds (5 * $attempt)
        }
    }
    Move-Item $part $path -Force
    $script:bytes += (Get-Item $path).Length
}

function Write-Source([string]$dir, $info) {
    $info | ConvertTo-Json -Depth 4 | Set-Content -Path (Join-Path $dir 'source.json') -Encoding utf8
}

# ------------------------------------------------------------ Poly Haven

# Map keys we keep, lower-cased: optional part prefix ("bark_"), then the map. Fabric patterns ship colour
# variants (col_01..03) instead of a diffuse map; the first one is the one their preview shows.
$KeepMap = '^(?<part>.*_)?(?<map>diff|diffuse|albedo|col_01|nor_gl|arm|rough|roughness|metal|metallic|ao|alpha|mask)$'

function Get-PolyHavenMaps($files, [string]$res, [string]$dir) {
    $keys = @($files.PSObject.Properties.Name)
    $lower = $keys | ForEach-Object { $_.ToLowerInvariant() }
    foreach ($key in $keys) {
        $k = $key.ToLowerInvariant()
        if ($k -notmatch $KeepMap) { continue }
        $part = $Matches['part']; $map = $Matches['map']
        # A packed AO/rough/metal map makes the separate ones redundant.
        if ($map -in 'rough', 'roughness', 'metal', 'metallic', 'ao' -and $lower -contains "${part}arm") { continue }
        $entry = $files.$key.$res
        if (-not $entry) { continue }
        $format = if ($entry.jpg) { 'jpg' } elseif ($entry.png) { 'png' } else { $null }
        if (-not $format) { continue }
        $url = $entry.$format.url
        Get-File $url (Join-Path $dir (Split-Path $url -Leaf))
    }
}

function Get-PolyHavenInfo([string]$id, [string]$kind, [string]$use, [string]$key) {
    $info = Invoke-RestMethod "https://api.polyhaven.com/info/$id"
    [ordered]@{
        id = $id; name = $info.name; kind = $kind; key = $key; use = $use
        authors = @($info.authors.PSObject.Properties.Name)
        source = 'Poly Haven'; url = "https://polyhaven.com/a/$id"; licence = 'CC0 1.0'
    }
}

function Get-PolyHaven($asset, [string]$kind) {
    $dir = Join-Path $Root "Assets/ThirdParty/PolyHaven/$($asset.id)"
    $files = Invoke-RestMethod "https://api.polyhaven.com/files/$($asset.id)"
    switch ($kind) {
        'hdri' {
            $url = $files.hdri.($asset.res).hdr.url
            Get-File $url (Join-Path $dir (Split-Path $url -Leaf))
        }
        'model' {
            $url = $files.fbx.($asset.res).fbx.url
            Get-File $url (Join-Path $dir (Split-Path $url -Leaf))
            Get-PolyHavenMaps $files $asset.res $dir
        }
        default { Get-PolyHavenMaps $files $asset.res $dir }
    }
    Write-Source $dir (Get-PolyHavenInfo $asset.id $kind $asset.use $asset.key)
}

# ------------------------------------------------------------ ambientCG

$KeepAcg = '_(Color|NormalGL|Roughness|AmbientOcclusion|Metalness|Opacity)\.(jpg|png)$'

function Get-AmbientCG($asset) {
    $dir = Join-Path $Root "Assets/ThirdParty/AmbientCG/$($asset.id)"
    $have = (Test-Path $dir) -and (Get-ChildItem $dir -Filter '*_Color.*' -ErrorAction SilentlyContinue)
    if (-not $have -or $Force) {
        $zip = Join-Path ([IO.Path]::GetTempPath()) "$($asset.id)_$($asset.res)-JPG.zip"
        $wc.DownloadFile("https://ambientcg.com/get?file=$($asset.id)_$($asset.res)-JPG.zip", $zip)
        $tmp = Join-Path ([IO.Path]::GetTempPath()) "acg_$($asset.id)"
        if (Test-Path $tmp) { Remove-Item $tmp -Recurse -Force }
        Expand-Archive -Path $zip -DestinationPath $tmp
        New-Item -ItemType Directory -Force -Path $dir | Out-Null
        Get-ChildItem $tmp -File | Where-Object { $_.Name -match $KeepAcg } | ForEach-Object {
            Copy-Item $_.FullName (Join-Path $dir $_.Name) -Force
            $script:bytes += $_.Length
        }
        Remove-Item $tmp -Recurse -Force
        Remove-Item $zip -Force
    }
    $r = Invoke-RestMethod "https://ambientcg.com/api/v2/full_json?id=$($asset.id)"
    $a = $r.foundAssets[0]
    Write-Source $dir ([ordered]@{
        id = $asset.id; name = $a.displayName; kind = 'texture'; key = $asset.key; use = $asset.use
        authors = @('ambientCG (Lennart Demes)')
        source = 'ambientCG'; url = "https://ambientcg.com/view?id=$($asset.id)"; licence = 'CC0 1.0'
    })
}

# ------------------------------------------------------------ run

foreach ($t in $Manifest.textures) {
    Write-Host "texture  $($t.source)/$($t.id)"
    if ($t.source -eq 'ambientcg') { Get-AmbientCG $t } else { Get-PolyHaven $t 'texture' }
}
foreach ($h in $Manifest.hdris) { Write-Host "hdri     $($h.id)"; Get-PolyHaven $h 'hdri' }
foreach ($m in $Manifest.models) { Write-Host "model    $($m.id)"; Get-PolyHaven $m 'model' }

$total = (Get-ChildItem (Join-Path $Root 'Assets/ThirdParty') -Recurse -File -Exclude '*.meta' | Measure-Object Length -Sum).Sum
Write-Host ("Downloaded {0:N1} MB this run; Assets/ThirdParty now holds {1:N1} MB." -f ($script:bytes / 1MB), ($total / 1MB))
