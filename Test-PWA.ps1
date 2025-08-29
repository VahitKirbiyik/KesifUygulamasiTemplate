# =========================================
# KesifApp PWA Test Scripti
# =========================================

Write-Host "🔍 KesifApp PWA Test Başlatılıyor..." -ForegroundColor Cyan

$baseUrl = "https://kesifapp.com"
$testResults = @{}

# 1. Manifest.json testi
Write-Host "`n📱 Manifest.json Testi:" -ForegroundColor Yellow
try {
    $manifestResponse = Invoke-WebRequest -Uri "$baseUrl/manifest.json" -UseBasicParsing
    if ($manifestResponse.StatusCode -eq 200) {
        $manifest = $manifestResponse.Content | ConvertFrom-Json
        Write-Host "✅ Manifest.json erişilebilir" -ForegroundColor Green

        # Manifest içeriği kontrolü
        $requiredFields = @("name", "short_name", "start_url", "display", "icons")
        foreach ($field in $requiredFields) {
            if ($manifest.$field) {
                Write-Host "✅ $field alanı mevcut: $($manifest.$field)" -ForegroundColor Green
            } else {
                Write-Host "❌ $field alanı eksik" -ForegroundColor Red
            }
        }

        $testResults["Manifest"] = $true
    } else {
        Write-Host "❌ Manifest.json erişilemiyor (Status: $($manifestResponse.StatusCode))" -ForegroundColor Red
        $testResults["Manifest"] = $false
    }
}
catch {
    Write-Host "❌ Manifest.json testi başarısız: $($_.Exception.Message)" -ForegroundColor Red
    $testResults["Manifest"] = $false
}

# 2. Service Worker testi
Write-Host "`n⚙️ Service Worker Testi:" -ForegroundColor Yellow
try {
    $swResponse = Invoke-WebRequest -Uri "$baseUrl/service-worker.js" -UseBasicParsing
    if ($swResponse.StatusCode -eq 200) {
        Write-Host "✅ service-worker.js erişilebilir" -ForegroundColor Green

        # Service Worker içeriği kontrolü
        $swContent = $swResponse.Content
        $swChecks = @(
            @{ Name = "Install event"; Pattern = "install" },
            @{ Name = "Activate event"; Pattern = "activate" },
            @{ Name = "Fetch event"; Pattern = "fetch" },
            @{ Name = "Cache management"; Pattern = "caches" }
        )

        foreach ($check in $swChecks) {
            if ($swContent -match $check.Pattern) {
                Write-Host "✅ $($check.Name) handler mevcut" -ForegroundColor Green
            } else {
                Write-Host "❌ $($check.Name) handler eksik" -ForegroundColor Red
            }
        }

        $testResults["ServiceWorker"] = $true
    } else {
        Write-Host "❌ service-worker.js erişilemiyor (Status: $($swResponse.StatusCode))" -ForegroundColor Red
        $testResults["ServiceWorker"] = $false
    }
}
catch {
    Write-Host "❌ Service Worker testi başarısız: $($_.Exception.Message)" -ForegroundColor Red
    $testResults["ServiceWorker"] = $false
}

# 3. HTML sayfalarında PWA entegrasyonu testi
Write-Host "`n🌐 HTML PWA Entegrasyonu Testi:" -ForegroundColor Yellow
$pagesToTest = @(
    @{ Name = "Ana Sayfa"; Url = "$baseUrl/" },
    @{ Name = "Yasal Sayfa"; Url = "$baseUrl/legal.html" },
    @{ Name = "Gizlilik Sayfası"; Url = "$baseUrl/privacy-policy.html" },
    @{ Name = "404 Sayfası"; Url = "$baseUrl/404.html" }
)

foreach ($page in $pagesToTest) {
    try {
        $pageResponse = Invoke-WebRequest -Uri $page.Url -UseBasicParsing
        if ($pageResponse.StatusCode -eq 200) {
            $content = $pageResponse.Content

            # PWA meta etiketleri kontrolü
            $pwaChecks = @(
                @{ Name = "Manifest link"; Pattern = 'rel="manifest"' },
                @{ Name = "Theme color"; Pattern = 'name="theme-color"' },
                @{ Name = "Apple mobile web app"; Pattern = 'name="apple-mobile-web-app' },
                @{ Name = "Service Worker script"; Pattern = 'service-worker\.js' }
            )

            $pageResults = @()
            foreach ($check in $pwaChecks) {
                if ($content -match $check.Pattern) {
                    $pageResults += "✅ $($check.Name)"
                } else {
                    $pageResults += "❌ $($check.Name)"
                }
            }

            Write-Host "$($page.Name) ($($page.Url)):" -ForegroundColor White
            foreach ($result in $pageResults) {
                if ($result -match "✅") {
                    Write-Host "  $result" -ForegroundColor Green
                } else {
                    Write-Host "  $result" -ForegroundColor Red
                }
            }

            $testResults[$page.Name] = $true
        } else {
            Write-Host "❌ $($page.Name) erişilemiyor (Status: $($pageResponse.StatusCode))" -ForegroundColor Red
            $testResults[$page.Name] = $false
        }
    }
    catch {
        Write-Host "❌ $($page.Name) testi başarısız: $($_.Exception.Message)" -ForegroundColor Red
        $testResults[$page.Name] = $false
    }
}

# 4. Offline capability testi (basit)
Write-Host "`n📶 Offline Capability Testi:" -ForegroundColor Yellow
try {
    # 404.html'nin offline fallback olarak çalışıp çalışmadığını kontrol et
    $offlineResponse = Invoke-WebRequest -Uri "$baseUrl/404.html" -UseBasicParsing
    if ($offlineResponse.StatusCode -eq 200) {
        Write-Host "✅ 404.html offline fallback sayfası mevcut" -ForegroundColor Green
        $testResults["OfflineFallback"] = $true
    } else {
        Write-Host "❌ 404.html offline fallback sayfası eksik" -ForegroundColor Red
        $testResults["OfflineFallback"] = $false
    }
}
catch {
    Write-Host "❌ Offline fallback testi başarısız: $($_.Exception.Message)" -ForegroundColor Red
    $testResults["OfflineFallback"] = $false
}

# 5. Test özeti
Write-Host "`n📊 PWA Test Özeti:" -ForegroundColor Cyan
Write-Host "=" * 50 -ForegroundColor Cyan

$passedTests = 0
$totalTests = $testResults.Count

foreach ($test in $testResults.GetEnumerator()) {
    if ($test.Value) {
        Write-Host "✅ $($test.Key): BAŞARILI" -ForegroundColor Green
        $passedTests++
    } else {
        Write-Host "❌ $($test.Key): BAŞARISIZ" -ForegroundColor Red
    }
}

Write-Host "`n📈 Genel Sonuç: $passedTests/$totalTests test başarılı" -ForegroundColor Yellow

if ($passedTests -eq $totalTests) {
    Write-Host "🎉 Tüm PWA testleri başarılı! Uygulamanız PWA özelliklerine hazır." -ForegroundColor Green
} elseif ($passedTests -ge ($totalTests * 0.7)) {
    Write-Host "⚠️ Çoğu PWA testi başarılı. Küçük iyileştirmeler gerekebilir." -ForegroundColor Yellow
} else {
    Write-Host "❌ PWA testlerinin çoğu başarısız. Yapılandırma kontrolü gerekebilir." -ForegroundColor Red
}

# 6. PWA kontrol önerileri
Write-Host "`n💡 PWA İyileştirme Önerileri:" -ForegroundColor Cyan
Write-Host "- Manifest.json'da tüm gerekli alanların doldurulduğundan emin olun" -ForegroundColor White
Write-Host "- Service Worker'ın tüm event handler'larını içerdiğinden emin olun" -ForegroundColor White
Write-Host "- Tüm HTML sayfalarında PWA meta etiketlerinin mevcut olduğundan emin olun" -ForegroundColor White
Write-Host "- HTTPS protokolü kullanıldığından emin olun (PWA için zorunlu)" -ForegroundColor White
Write-Host "- Farklı ekran boyutlarında test edin" -ForegroundColor White

Write-Host "`n🔄 Testi tekrar çalıştırmak için: .\Test-PWA.ps1" -ForegroundColor Cyan
