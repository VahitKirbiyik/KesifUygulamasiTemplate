param()

$root = (Get-Location).Path
$timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
$backupDir = Join-Path $root "backup-aihelper-$timestamp"
New-Item -ItemType Directory -Path $backupDir -Force | Out-Null

Write-Host "Backup dir: $backupDir"

# Move KesifUygulamasi.AIHelper folder if present
$aihelper = Join-Path $root 'KesifUygulamasi.AIHelper'
if (Test-Path $aihelper) {
    Write-Host "Moving AIHelper folder..."
    Move-Item -Path $aihelper -Destination $backupDir -Force
    Write-Host "Moved: $aihelper -> $backupDir"
} else {
    Write-Host "No KesifUygulamasi.AIHelper folder found."
}

# Move Ollama/OllamaPanel related files from Views if present
$views = Join-Path $root 'Views'
if (Test-Path $views) {
    $ollamaFiles = Get-ChildItem -Path $views -Recurse -File -Include '*Ollama*' -ErrorAction SilentlyContinue
    foreach ($f in $ollamaFiles) {
        $rel = $f.FullName.Substring($root.Length).TrimStart('\')
        Write-Host "Moving: $rel"
        Move-Item -Path $f.FullName -Destination $backupDir -Force
    }
} else {
    Write-Host "Views folder not found."
}

# Also move any top-level files that reference AIHelper namespaces (simple heuristic)
$aihelperMatches = Get-ChildItem -Path $root -Recurse -File -Include *.xaml,*.cs -ErrorAction SilentlyContinue | Where-Object {
    Select-String -Path $_.FullName -Pattern 'KesifUygulamasi.AIHelper' -Quiet
}
foreach ($m in $aihelperMatches) {
    Write-Host "Archiving file referencing AIHelper: $($m.FullName)"
    Move-Item -Path $m.FullName -Destination $backupDir -Force
}

# Clean obj and bin folders to remove generated references from previous builds
Write-Host "Cleaning obj/bin folders..."
Get-ChildItem -Path $root -Recurse -Directory -Force -ErrorAction SilentlyContinue | Where-Object { $_.Name -in @('obj','bin') } | ForEach-Object {
    try {
        Remove-Item -LiteralPath $_.FullName -Recurse -Force -ErrorAction Stop
        Write-Host "Removed: $($_.FullName)"
    } catch {
        Write-Host "Failed to remove: $($_.FullName) - $($_.Exception.Message)"
    }
}

Write-Host "AIHelper backup and cleanup complete. Backup location: $backupDir"
