param()
$ErrorActionPreference = 'Stop'

function Write-Log {
    param([string]$msg)
    Add-Content -Path "tools/release-full-check.log" -Value ((Get-Date -Format o) + " $msg")
}

Write-Log "release-full-check.ps1 başladı."
Write-Host "release-full-check.ps1 başladı."

# Dotnet CLI kontrolü
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Log "dotnet CLI bulunamadı."
    Write-Host "[HATA] dotnet CLI bulunamadı. Lütfen .NET 8 SDK yükleyin."
    exit 1
}

# signtool kontrolü
if (-not (Get-Command signtool -ErrorAction SilentlyContinue)) {
    Write-Log "signtool bulunamadı."
    Write-Host "[HATA] signtool bulunamadı. Lütfen Windows SDK yükleyin."
    exit 1
}

# NuGet CLI kontrolü
if (-not (Get-Command nuget -ErrorAction SilentlyContinue)) {
    Write-Log "NuGet CLI bulunamadı."
    Write-Host "[HATA] NuGet CLI bulunamadı. Lütfen nuget.exe ekleyin."
    exit 1
}

# Sertifika dosyası kontrolü
$certFiles = @("tools/codesign.pfx", "tools/codesign.cer", "tools/codesign.key")
$certFound = $false
foreach ($cert in $certFiles) {
    if (Test-Path $cert) { $certFound = $true }
}
if (-not $certFound) {
    Write-Log "Sertifika dosyası eksik."
    Write-Host "[HATA] Sertifika dosyası eksik. tools klasörüne .pfx/.cer/.key ekleyin."
    exit 1
}

# .resx encoding kontrolü (UTF-8)
$resxFiles = Get-ChildItem -Path "Resources/Strings" -Filter *.resx -Recurse
foreach ($file in $resxFiles) {
    $bytes = [System.IO.File]::ReadAllBytes($file.FullName)
    if ($bytes.Length -gt 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF) {
        Write-Log "$($file.Name) UTF-8 BOM ile kaydedilmiş."
    } else {
        Write-Log "$($file.Name) UTF-8 BOM'suz veya farklı encoding."
        Write-Host "[UYARI] $($file.Name) dosyası UTF-8 BOM ile kaydedilmeli."
    }
}

Write-Log "release-full-check.ps1 tamamlandı."
Write-Host "release-full-check.ps1 tamamlandı."
exit 0
