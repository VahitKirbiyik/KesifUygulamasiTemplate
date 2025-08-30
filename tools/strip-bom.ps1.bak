param(
    [Parameter(Mandatory=$false)][string]$Root = "."
)

$utf8NoBom = New-Object System.Text.UTF8Encoding($false)
$files = Get-ChildItem -Path $Root -Recurse -File -Include *.cs,*.xaml,*.csproj,*.props,*.targets,*.json,*.md,*.yml,*.yaml,*.ps1,*.xml

foreach ($f in $files) {
    $bytes = [System.IO.File]::ReadAllBytes($f.FullName)
    if ($bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF) {
        $content = [System.Text.Encoding]::UTF8.GetString($bytes, 3, $bytes.Length - 3)
        [System.IO.File]::WriteAllText($f.FullName, $content, $utf8NoBom)
        Write-Host "BOM removed: $($f.FullName)"
    }
}
