Param()

$root = Split-Path -Parent $MyInvocation.MyCommand.Definition
Write-Host "Root: $root"

$aihelperPath = Join-Path $root 'KesifUygulamasi.AIHelper'
if (-Not (Test-Path $aihelperPath)) { Write-Host "AIHelper not found: $aihelperPath"; exit 0 }

$backupDir = Join-Path $aihelperPath 'backup-empty-xaml'
New-Item -ItemType Directory -Path $backupDir -Force | Out-Null

$files = Get-ChildItem -Path $aihelperPath -Filter *.xaml -Recurse -File
$moved = 0
foreach ($f in $files) {
    if ($f.Length -eq 0) {
        $dest = Join-Path $backupDir $f.Name
        Write-Host "Moving empty XAML: $($f.FullName) -> $dest"
        Move-Item -Path $f.FullName -Destination $dest -Force
        $moved++
    }
}
Write-Host "Moved $moved empty XAML files to $backupDir"
