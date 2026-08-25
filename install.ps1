$ErrorActionPreference = "Stop"

$appName = "TodoList"
$publishDir = Join-Path $PSScriptRoot "bin\Release\net10.0-windows\win-x64\publish"
$exePath = Join-Path $publishDir "TodoList.exe"
$installDir = Join-Path $env:LocalAppData $appName
$targetExe = Join-Path $installDir "TodoList.exe"

if (-not (Test-Path $exePath)) {
    Write-Host "Publishing the application..."
    dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
}

Write-Host "Installing to $installDir..."
if (-not (Test-Path $installDir)) {
    New-Item -ItemType Directory -Force -Path $installDir | Out-Null
}

# Stop if it's already running
Get-Process -Name $appName -ErrorAction SilentlyContinue | Stop-Process -Force

Copy-Item -Path (Join-Path $publishDir "*") -Destination $installDir -Recurse -Force

# Create Desktop Shortcut
$wshShell = New-Object -ComObject WScript.Shell
$desktop = [Environment]::GetFolderPath("Desktop")
$shortcut = $wshShell.CreateShortcut((Join-Path $desktop "$appName.lnk"))
$shortcut.TargetPath = $targetExe
$shortcut.WorkingDirectory = $installDir
$shortcut.IconLocation = $targetExe
$shortcut.Save()
Write-Host "Created Desktop Shortcut."

# Create Start Menu Shortcut
$startMenu = [Environment]::GetFolderPath("StartMenu")
$programs = Join-Path $startMenu "Programs"
$shortcutStart = $wshShell.CreateShortcut((Join-Path $programs "$appName.lnk"))
$shortcutStart.TargetPath = $targetExe
$shortcutStart.WorkingDirectory = $installDir
$shortcutStart.IconLocation = $targetExe
$shortcutStart.Save()
Write-Host "Created Start Menu Shortcut."

Write-Host "Installation Complete! You can now launch TodoList from your Desktop or Start Menu."
