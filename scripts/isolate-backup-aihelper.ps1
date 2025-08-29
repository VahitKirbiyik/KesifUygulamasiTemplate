# Move backup-aihelper-* folders into .hidden-archive to avoid MSBuild picking them up
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Get-ChildItem -Path $root -Filter "backup-aihelper-*" -Directory | ForEach-Object {
    $dest = Join-Path $root ".hidden-archive"
    if (-not (Test-Path $dest)) { New-Item -ItemType Directory -Path $dest | Out-Null }
    $target = Join-Path $dest $_.Name
    Write-Host "Moving $($_.FullName) -> $target"
    Move-Item -Path $_.FullName -Destination $target -Force
}
Write-Host "Done."
