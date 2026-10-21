<#
.SYNOPSIS
  Runs the Unity Editor from the command line (batch mode) for this project.

.DESCRIPTION
  Commands:
    info    Print Unity version, render pipeline, scenes and packages.
    check   Compile every script. Exit 0 = compiles.
    test    Run tests (-Platform EditMode|PlayMode, optional -Filter).
    setup   Create folders, set player settings, add missing packages.
    build   Build a Windows player (-Out to change the .exe path).
    shot    Render a scene's main camera to a PNG (-Scene, -Out, -Camera).
    run     Run any static editor method (-Method Namespace.Class.Method).
    open    Open the project in the normal Unity Editor (for playtesting).
    log     Show the end of the most recent log.

  The Unity Editor GUI must be closed for every command except 'open' and 'log'.

.EXAMPLE
  powershell -NoProfile -ExecutionPolicy Bypass -File tools/unity.ps1 check
.EXAMPLE
  powershell -NoProfile -ExecutionPolicy Bypass -File tools/unity.ps1 test -Filter MatchSetupTests
.EXAMPLE
  powershell -NoProfile -ExecutionPolicy Bypass -File tools/unity.ps1 shot -Scene Assets/Hollowmere/Scenes/Greybox.unity
#>
param(
    [Parameter(Position = 0)]
    [ValidateSet('info', 'check', 'test', 'setup', 'build', 'shot', 'run', 'open', 'log')]
    [string]$Command = 'check',
    [string]$Method = '',
    [ValidateSet('EditMode', 'PlayMode')]
    [string]$Platform = 'EditMode',
    [string]$Filter = '',
    [string]$Scene = '',
    [string]$Camera = '',
    [string]$Out = '',
    [int]$TimeoutMinutes = 40
)

$ErrorActionPreference = 'Stop'
$ProjectPath = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$LogDir = Join-Path $ProjectPath 'Logs\claude'
New-Item -ItemType Directory -Force -Path $LogDir | Out-Null

function Quote([string]$s) { return '"' + $s + '"' }

function Find-UnityExe {
    if ($env:UNITY_EXE -and (Test-Path $env:UNITY_EXE)) { return $env:UNITY_EXE }

    $roots = @()
    $roots += (Join-Path $env:ProgramFiles 'Unity\Hub\Editor')
    $secondary = Join-Path $env:APPDATA 'UnityHub\secondaryInstallPath.json'
    if (Test-Path $secondary) {
        # Unity Hub stores a custom editor install folder as a JSON string, e.g. "D:\\Unity".
        try { $raw = Get-Content $secondary -Raw | ConvertFrom-Json }
        catch { $raw = (Get-Content $secondary -Raw).Trim().Trim('"') }
        if (($raw -is [string]) -and $raw) { $roots += $raw }
    }

    # Prefer the exact version this project was last opened with.
    $versionFile = Join-Path $ProjectPath 'ProjectSettings\ProjectVersion.txt'
    if (Test-Path $versionFile) {
        $line = Get-Content $versionFile | Where-Object { $_ -like 'm_EditorVersion:*' } | Select-Object -First 1
        if ($line) {
            $version = ($line -split ':', 2)[1].Trim()
            foreach ($root in $roots) {
                $exe = Join-Path $root "$version\Editor\Unity.exe"
                if (Test-Path $exe) { return $exe }
            }
            Write-Host "WARNING: Unity $version (from ProjectVersion.txt) is not installed in the usual places. Falling back to the newest editor found."
        }
    }

    foreach ($root in $roots) {
        if (-not (Test-Path $root)) { continue }
        $dirs = Get-ChildItem $root -Directory | Sort-Object {
            # Sort 6000.3.10f1 above 6000.3.9f1.
            ($_.Name -replace '[a-z]\d*$', '') -as [version]
        } -Descending
        foreach ($d in $dirs) {
            $exe = Join-Path $d.FullName 'Editor\Unity.exe'
            if (Test-Path $exe) { return $exe }
        }
    }
    throw 'Unity.exe not found. Install Unity 6.3 LTS with Unity Hub, or set the UNITY_EXE environment variable to the full path of Unity.exe.'
}

function Test-EditorOpen {
    $lock = Join-Path $ProjectPath 'Temp\UnityLockfile'
    if (-not (Test-Path $lock)) { return $false }
    try {
        $fs = [System.IO.File]::Open($lock, 'Open', 'ReadWrite', 'None')
        $fs.Close()
        return $false
    } catch {
        return $true
    }
}

function Show-LogSummary([string]$log) {
    if (-not (Test-Path $log)) { Write-Host '(Unity wrote no log file)'; return }
    $lines = Get-Content $log
    $pattern = 'error CS\d+|warning CS0618|Scripts have compiler errors|Aborting batchmode|Exception:|NullReferenceException|\[Hollowmere\]|Error building|BuildFailedException|No valid Unity Editor license|another Unity instance is running|Assembly has reference to non-existent assembly'
    $hits = $lines | Select-String -Pattern $pattern | ForEach-Object { $_.Line.Trim() } | Select-Object -Unique
    if ($hits) {
        Write-Host '---- Unity log highlights ----'
        $hits | Select-Object -First 80 | ForEach-Object { Write-Host $_ }
        $count = @($hits).Count
        if ($count -gt 80) { Write-Host "... ($count lines matched, showing 80)" }
    } else {
        Write-Host '---- Last 25 log lines ----'
        $lines | Select-Object -Last 25 | ForEach-Object { Write-Host $_ }
    }
    Write-Host "Full log: $log"
}

function Invoke-Unity([string[]]$UnityArgs, [string]$LogName) {
    if (Test-EditorOpen) {
        Write-Host 'ERROR: The Unity Editor has this project open. Ask the user to close the Editor, then run this again.'
        exit 10
    }
    $unity = Find-UnityExe
    $log = Join-Path $LogDir "$LogName.log"
    if (Test-Path $log) { Remove-Item $log -Force }
    Set-Content -Path (Join-Path $LogDir 'last.txt') -Value $log

    $all = @('-batchmode', '-projectPath', (Quote $ProjectPath), '-logFile', (Quote $log)) + $UnityArgs
    Write-Host "Unity: $unity"
    Write-Host "Running '$LogName' (first run on a fresh project imports everything and can take several minutes)..."
    $started = Get-Date
    $p = Start-Process -FilePath $unity -ArgumentList $all -PassThru -NoNewWindow
    $null = $p.Handle  # keeps ExitCode available after the process ends
    if (-not $p.WaitForExit($TimeoutMinutes * 60 * 1000)) {
        try { $p.Kill() } catch { }
        Write-Host "ERROR: Unity did not finish within $TimeoutMinutes minutes and was stopped."
        Show-LogSummary $log
        exit 11
    }
    $p.WaitForExit()
    $code = $p.ExitCode
    $secs = [int]((Get-Date) - $started).TotalSeconds
    Write-Host "Unity exited with code $code after $secs s."
    Show-LogSummary $log
    return $code
}

switch ($Command) {
    'info' {
        $code = Invoke-Unity @('-nographics', '-executeMethod', 'Hollowmere.EditorTools.BatchTools.Info') 'info'
        $info = Join-Path $LogDir 'info.txt'
        if (Test-Path $info) { Get-Content $info | ForEach-Object { Write-Host $_ } }
        exit $code
    }
    'check' {
        $code = Invoke-Unity @('-nographics', '-executeMethod', 'Hollowmere.EditorTools.BatchTools.CompileCheck') 'check'
        if ($code -eq 0) { Write-Host 'RESULT: compile OK' } else { Write-Host 'RESULT: compile FAILED (see errors above)' }
        exit $code
    }
    'setup' {
        $code = Invoke-Unity @('-nographics', '-executeMethod', 'Hollowmere.EditorTools.BatchTools.Setup') 'setup'
        exit $code
    }
    'test' {
        $results = Join-Path $LogDir "test-results-$Platform.xml"
        if (Test-Path $results) { Remove-Item $results -Force }
        $unityArgs = @('-runTests', '-testPlatform', $Platform, '-testResults', (Quote $results))
        if ($Filter) { $unityArgs += @('-testFilter', (Quote $Filter)) }
        if ($Platform -eq 'EditMode') { $unityArgs += '-nographics' }
        $code = Invoke-Unity $unityArgs "test-$Platform"
        if (Test-Path $results) {
            [xml]$xml = Get-Content $results -Raw
            $run = $xml.SelectSingleNode('/test-run')
            $total = $run.GetAttribute('total'); $passed = $run.GetAttribute('passed')
            $failed = $run.GetAttribute('failed'); $skipped = $run.GetAttribute('skipped')
            Write-Host "TESTS ($Platform): total=$total passed=$passed failed=$failed skipped=$skipped"
            foreach ($case in $xml.SelectNodes("//test-case[@result='Failed']")) {
                Write-Host ('FAILED: ' + $case.GetAttribute('fullname'))
                $msg = $case.SelectSingleNode('failure/message')
                if ($msg) { Write-Host ('  ' + $msg.InnerText.Trim()) }
                $stack = $case.SelectSingleNode('failure/stack-trace')
                if ($stack) {
                    ($stack.InnerText -split "`n") | Select-Object -First 4 | ForEach-Object { Write-Host ('    ' + $_.Trim()) }
                }
            }
        } else {
            Write-Host 'No test results file. Tests did not run (usually a compile error, see above).'
        }
        # Unity: 0 = all passed, 2 = some failed, 3 = run error
        exit $code
    }
    'build' {
        $unityArgs = @('-executeMethod', 'Hollowmere.EditorTools.BatchTools.BuildWindows')
        if ($Out) { $unityArgs += @('-hmOut', (Quote $Out)) }
        exit (Invoke-Unity $unityArgs 'build')
    }
    'shot' {
        # No -nographics: rendering needs the GPU.
        $unityArgs = @('-executeMethod', 'Hollowmere.EditorTools.BatchTools.CaptureScreenshot')
        if ($Scene) { $unityArgs += @('-hmScene', (Quote $Scene)) }
        if ($Camera) { $unityArgs += @('-hmCamera', (Quote $Camera)) }
        $png = if ($Out) { $Out } else { Join-Path $LogDir 'screenshot.png' }
        $unityArgs += @('-hmOut', (Quote $png))
        $code = Invoke-Unity $unityArgs 'shot'
        if ($code -eq 0) { Write-Host "Screenshot: $png" }
        exit $code
    }
    'run' {
        if (-not $Method) { Write-Host 'ERROR: pass -Method Namespace.Class.Method'; exit 2 }
        $safeName = 'run-' + ($Method -replace '[^A-Za-z0-9_.-]', '_')
        exit (Invoke-Unity @('-executeMethod', $Method, '-quit') $safeName)
    }
    'open' {
        $unity = Find-UnityExe
        Start-Process -FilePath $unity -ArgumentList @('-projectPath', (Quote $ProjectPath))
        Write-Host 'Opening the Unity Editor. Close it again before running batch commands.'
        exit 0
    }
    'log' {
        $lastFile = Join-Path $LogDir 'last.txt'
        if (-not (Test-Path $lastFile)) { Write-Host 'No log yet.'; exit 0 }
        $last = (Get-Content $lastFile -Raw).Trim()
        if (Test-Path $last) { Get-Content $last | Select-Object -Last 120 | ForEach-Object { Write-Host $_ } }
        exit 0
    }
}
