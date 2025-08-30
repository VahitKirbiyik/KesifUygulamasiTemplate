$#!/usr/bin/env pwsh
# Use the repository root (parent of the scripts folder)
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$root = Resolve-Path (Join-Path $scriptDir '..') | Select-Object -ExpandProperty Path
$hidden = Join-Path $root '.hidden-archive'
if (-not (Test-Path $hidden)) { New-Item -ItemType Directory -Path $hidden | Out-Null }
Get-ChildItem -Path $root -Filter 'backup-aihelper-*' -Directory | ForEach-Object {
    $dest = Join-Path $hidden $_.Name
    Write-Host "Moving '$($_.FullName)' to '$dest'"
    Move-Item -Path $_.FullName -Destination $dest -Force
}
