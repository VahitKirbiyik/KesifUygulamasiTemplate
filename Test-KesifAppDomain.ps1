# =========================================
# KesifApp.com Domain & Sayfa Test Script
# =========================================

$domainUrl = "https://kesifapp.com"
$pagesToTest = @(
    "$domainUrl/",
    "$domainUrl/legal.html",
    "$domainUrl/privacy-policy.html"
)

Write-Host "🔍 KesifApp.com Test Başlatılıyor..." -ForegroundColor Cyan

foreach ($page in $pagesToTest) {
    try {
        $resp = Invoke-WebRequest -Uri $page -UseBasicParsing -TimeoutSec 10
        if ($resp.StatusCode -eq 200) {
            Write-Host "✅ Sayfa Açılıyor: $page" -ForegroundColor Green
        } else {
            Write-Host "❌ Sayfa Hatası: $page (StatusCode: $(.StatusCode))" -ForegroundColor Red
        }
    }
    catch {
        Write-Host "❌ Sayfa Bulunamadı veya Erişilemiyor: $page" -ForegroundColor Red
    }
}

# README.md Lisans Badge Kontrolü
$readmeUrl = "https://raw.githubusercontent.com/VahitKirbiyik/KesifUygulamasiTemplate/main/README.md"
try {
    $readme = Invoke-WebRequest -Uri $readmeUrl -UseBasicParsing
    if ($readme.Content -match "https://kesifapp.com/legal.html") {
        Write-Host "✅ Lisans Badge Doğru Yönlendirme: https://kesifapp.com/legal.html" -ForegroundColor Green
    } else {
        Write-Host "❌ Lisans Badge Linki Yanlış veya Bulunamadı" -ForegroundColor Red
    }
}
catch {
    Write-Host "❌ README.md dosyası okunamadı veya erişilemiyor" -ForegroundColor Red
}

Write-Host "🔎 Test Tamamlandı!" -ForegroundColor Cyan\n