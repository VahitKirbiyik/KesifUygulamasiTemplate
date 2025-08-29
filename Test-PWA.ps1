# =========================================
# KesifApp PWA Test Scripti
# =========================================
# -*- coding: utf-8 -*-
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

param(
    [string]$BaseUrl = "https://kesifapp.com",
    [switch]$Verbose,
    [switch]$SkipNetworkTests
)

Write-Host "🔍 KesifApp PWA Test Başlatılıyor..." -ForegroundColor Cyan
if ($Verbose) {
    Write-Host "📋 Ayrıntılı mod aktif" -ForegroundColor Yellow
}

$testResults = @{}
$startTime = Get-Date

# Test sonuçlarını logla
function Write-TestLog {
    param([string]$Message, [string]$Level = "INFO")
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $logMessage = "[$timestamp] [$Level] $Message"
    if ($Verbose) {
        Write-Host $logMessage -ForegroundColor Gray
    }
}

# 1. Manifest.json testi
Write-Host "`n📱 Manifest.json Testi:" -ForegroundColor Yellow
Write-TestLog "Manifest.json testi başlatılıyor"

try {
    if (-not $SkipNetworkTests) {
        $manifestResponse = Invoke-WebRequest -Uri "$BaseUrl/manifest.json" -UseBasicParsing -TimeoutSec 30
        Write-TestLog "Manifest.json HTTP yanıtı alındı: $($manifestResponse.StatusCode)"

        if ($manifestResponse.StatusCode -eq 200) {
            try {
                $manifest = $manifestResponse.Content | ConvertFrom-Json
                Write-Host "✅ Manifest.json erişilebilir" -ForegroundColor Green
                Write-TestLog "Manifest.json başarıyla parse edildi"

                # Manifest içeriği kontrolü
                $requiredFields = @("name", "short_name", "start_url", "display", "icons")
                $optionalFields = @("description", "theme_color", "background_color", "lang", "scope")

                foreach ($field in $requiredFields) {
                    if ($manifest.PSObject.Properties.Name -contains $field) {
                        $value = $manifest.$field
                        if ($value) {
                            Write-Host "✅ $field alanı mevcut: $value" -ForegroundColor Green
                            Write-TestLog "$field alanı doğrulandı: $value"
                        } else {
                            Write-Host "⚠️ $field alanı boş" -ForegroundColor Yellow
                        }
                    } else {
                        Write-Host "❌ $field alanı eksik" -ForegroundColor Red
                        Write-TestLog "$field alanı bulunamadı" "WARN"
                    }
                }

                # İsteğe bağlı alanları kontrol et
                foreach ($field in $optionalFields) {
                    if ($manifest.PSObject.Properties.Name -contains $field) {
                        Write-Host "ℹ️ $field alanı mevcut: $($manifest.$field)" -ForegroundColor Cyan
                    }
                }

                $testResults["Manifest"] = $true
            }
            catch {
                Write-Host "❌ Manifest.json parse edilemedi: $($_.Exception.Message)" -ForegroundColor Red
                Write-TestLog "JSON parse hatası: $($_.Exception.Message)" "ERROR"
                $testResults["Manifest"] = $false
            }
        } else {
            Write-Host "❌ Manifest.json erişilemiyor (Status: $($manifestResponse.StatusCode))" -ForegroundColor Red
            $testResults["Manifest"] = $false
        }
    } else {
        Write-Host "⏭️ Ağ testleri atlandı" -ForegroundColor Yellow
        $testResults["Manifest"] = $null
    }
}
catch {
    Write-Host "❌ Manifest.json testi başarısız: $($_.Exception.Message)" -ForegroundColor Red
    Write-TestLog "Manifest testi hatası: $($_.Exception.Message)" "ERROR"
    $testResults["Manifest"] = $false
}

# 2. Service Worker testi
Write-Host "`n⚙️ Service Worker Testi:" -ForegroundColor Yellow
Write-TestLog "Service Worker testi başlatılıyor"

try {
    if (-not $SkipNetworkTests) {
        $swResponse = Invoke-WebRequest -Uri "$BaseUrl/service-worker.js" -UseBasicParsing -TimeoutSec 30
        Write-TestLog "Service Worker HTTP yanıtı alındı: $($swResponse.StatusCode)"

        if ($swResponse.StatusCode -eq 200) {
            Write-Host "✅ service-worker.js erişilebilir" -ForegroundColor Green

            # Service Worker içeriği kontrolü
            $swContent = $swResponse.Content
            $swChecks = @(
                @{ Name = "Install event"; Pattern = "install"; Required = $true },
                @{ Name = "Activate event"; Pattern = "activate"; Required = $true },
                @{ Name = "Fetch event"; Pattern = "fetch"; Required = $true },
                @{ Name = "Cache management"; Pattern = "caches"; Required = $true },
                @{ Name = "Error handling"; Pattern = "addEventListener.*error"; Required = $false },
                @{ Name = "Push notifications"; Pattern = "push"; Required = $false },
                @{ Name = "Background sync"; Pattern = "sync"; Required = $false }
            )

            $swScore = 0
            $totalChecks = $swChecks.Count

            foreach ($check in $swChecks) {
                if ($swContent -match $check.Pattern) {
                    $symbol = if ($check.Required) { "✅" } else { "ℹ️" }
                    $color = if ($check.Required) { "Green" } else { "Cyan" }
                    Write-Host "$symbol $($check.Name) handler mevcut" -ForegroundColor $color
                    if ($check.Required) { $swScore++ }
                    Write-TestLog "$($check.Name) handler bulundu"
                } else {
                    $symbol = if ($check.Required) { "❌" } else { "⚪" }
                    $color = if ($check.Required) { "Red" } else { "Gray" }
                    Write-Host "$symbol $($check.Name) handler eksik" -ForegroundColor $color
                    if ($check.Required) {
                        Write-TestLog "$($check.Name) handler eksik" "WARN"
                    }
                }
            }

            $swPercentage = [math]::Round(($swScore / $swChecks.Where({$_.Required}).Count) * 100, 1)
            Write-Host "📊 Service Worker kapsamı: $swPercentage%" -ForegroundColor Yellow

            $testResults["ServiceWorker"] = $swPercentage -ge 75
        } else {
            Write-Host "❌ service-worker.js erişilemiyor (Status: $($swResponse.StatusCode))" -ForegroundColor Red
            $testResults["ServiceWorker"] = $false
        }
    } else {
        Write-Host "⏭️ Ağ testleri atlandı" -ForegroundColor Yellow
        $testResults["ServiceWorker"] = $null
    }
}
catch {
    Write-Host "❌ Service Worker testi başarısız: $($_.Exception.Message)" -ForegroundColor Red
    Write-TestLog "Service Worker testi hatası: $($_.Exception.Message)" "ERROR"
    $testResults["ServiceWorker"] = $false
}

# 3. HTML sayfalarında PWA entegrasyonu testi
Write-Host "`n🌐 HTML PWA Entegrasyonu Testi:" -ForegroundColor Yellow
Write-TestLog "HTML PWA entegrasyonu testi başlatılıyor"

$pagesToTest = @(
    @{ Name = "Ana Sayfa"; Url = "$BaseUrl/"; File = "index.html" },
    @{ Name = "Yasal Sayfa"; Url = "$BaseUrl/legal.html"; File = "legal.html" },
    @{ Name = "Gizlilik Sayfası"; Url = "$BaseUrl/privacy-policy.html"; File = "privacy-policy.html" },
    @{ Name = "404 Sayfası"; Url = "$BaseUrl/404.html"; File = "404.html" }
)

foreach ($page in $pagesToTest) {
    Write-TestLog "$($page.Name) testi başlatılıyor"

    try {
        if (-not $SkipNetworkTests) {
            $pageResponse = Invoke-WebRequest -Uri $page.Url -UseBasicParsing -TimeoutSec 30
            Write-TestLog "$($page.Name) HTTP yanıtı: $($pageResponse.StatusCode)"

            if ($pageResponse.StatusCode -eq 200) {
                $content = $pageResponse.Content

                # PWA meta etiketleri kontrolü
                $pwaChecks = @(
                    @{ Name = "Manifest link"; Pattern = 'rel="manifest"'; Required = $true },
                    @{ Name = "Theme color"; Pattern = 'name="theme-color"'; Required = $true },
                    @{ Name = "Apple mobile web app"; Pattern = 'name="apple-mobile-web-app'; Required = $false },
                    @{ Name = "Service Worker script"; Pattern = 'service-worker\.js'; Required = $true },
                    @{ Name = "Viewport meta"; Pattern = 'name="viewport"'; Required = $true },
                    @{ Name = "Charset UTF-8"; Pattern = 'charset=utf-8'; Required = $true }
                )

                $pageResults = @()
                $pageScore = 0
                $requiredChecks = $pwaChecks.Where({$_.Required}).Count

                foreach ($check in $pwaChecks) {
                    if ($content -match $check.Pattern) {
                        $symbol = if ($check.Required) { "✅" } else { "ℹ️" }
                        $pageResults += "$symbol $($check.Name)"
                        if ($check.Required) { $pageScore++ }
                        Write-TestLog "$($page.Name) - $($check.Name) bulundu"
                    } else {
                        $symbol = if ($check.Required) { "❌" } else { "⚪" }
                        $pageResults += "$symbol $($check.Name)"
                        if ($check.Required) {
                            Write-TestLog "$($page.Name) - $($check.Name) eksik" "WARN"
                        }
                    }
                }

                Write-Host "$($page.Name) ($($page.Url)):" -ForegroundColor White
                foreach ($result in $pageResults) {
                    if ($result -match "✅") {
                        Write-Host "  $result" -ForegroundColor Green
                    } elseif ($result -match "ℹ️") {
                        Write-Host "  $result" -ForegroundColor Cyan
                    } else {
                        Write-Host "  $result" -ForegroundColor Red
                    }
                }

                $pagePercentage = [math]::Round(($pageScore / $requiredChecks) * 100, 1)
                Write-Host "  📊 PWA kapsamı: $pagePercentage%" -ForegroundColor Yellow

                $testResults[$page.Name] = $pagePercentage -ge 80
            } else {
                Write-Host "❌ $($page.Name) erişilemiyor (Status: $($pageResponse.StatusCode))" -ForegroundColor Red
                $testResults[$page.Name] = $false
            }
        } else {
            Write-Host "⏭️ $($page.Name) - Ağ testleri atlandı" -ForegroundColor Yellow
            $testResults[$page.Name] = $null
        }
    }
    catch {
        Write-Host "❌ $($page.Name) testi başarısız: $($_.Exception.Message)" -ForegroundColor Red
        Write-TestLog "$($page.Name) testi hatası: $($_.Exception.Message)" "ERROR"
        $testResults[$page.Name] = $false
    }
}

# 4. Offline capability testi (basit)
Write-Host "`n📶 Offline Capability Testi:" -ForegroundColor Yellow
Write-TestLog "Offline capability testi başlatılıyor"

try {
    if (-not $SkipNetworkTests) {
        $offlineResponse = Invoke-WebRequest -Uri "$BaseUrl/404.html" -UseBasicParsing -TimeoutSec 30
        Write-TestLog "404.html HTTP yanıtı: $($offlineResponse.StatusCode)"

        if ($offlineResponse.StatusCode -eq 200) {
            $offlineContent = $offlineResponse.Content

            # 404 sayfasında PWA entegrasyonu kontrolü
            $offlineChecks = @(
                @{ Name = "Service Worker"; Pattern = 'service-worker\.js' },
                @{ Name = "Manifest link"; Pattern = 'rel="manifest"' },
                @{ Name = "Offline mesaj"; Pattern = 'offline|çevrimdışı|bağlantı yok' }
            )

            foreach ($check in $offlineChecks) {
                if ($offlineContent -match $check.Pattern) {
                    Write-Host "✅ $($check.Name) mevcut" -ForegroundColor Green
                } else {
                    Write-Host "⚠️ $($check.Name) eksik" -ForegroundColor Yellow
                }
            }

            Write-Host "✅ 404.html offline fallback sayfası mevcut" -ForegroundColor Green
            $testResults["OfflineFallback"] = $true
        } else {
            Write-Host "❌ 404.html offline fallback sayfası eksik" -ForegroundColor Red
            $testResults["OfflineFallback"] = $false
        }
    } else {
        Write-Host "⏭️ Offline testi atlandı" -ForegroundColor Yellow
        $testResults["OfflineFallback"] = $null
    }
}
catch {
    Write-Host "❌ Offline fallback testi başarısız: $($_.Exception.Message)" -ForegroundColor Red
    Write-TestLog "Offline testi hatası: $($_.Exception.Message)" "ERROR"
    $testResults["OfflineFallback"] = $false
}

# 5. Test özeti
$endTime = Get-Date
$duration = $endTime - $startTime

Write-Host "`n📊 PWA Test Özeti:" -ForegroundColor Cyan
Write-Host "=" * 60 -ForegroundColor Cyan
Write-Host "⏱️ Test süresi: $($duration.TotalSeconds.ToString("F2")) saniye" -ForegroundColor Gray

$validResults = $testResults.Values.Where({ $_ -ne $null })
$passedTests = ($validResults | Where-Object { $_ -eq $true }).Count
$totalValidTests = $validResults.Count
$skippedTests = $testResults.Count - $totalValidTests

if ($skippedTests -gt 0) {
    Write-Host "⏭️ Atlanan testler: $skippedTests" -ForegroundColor Yellow
}

Write-Host "`n📈 Test Sonuçları:" -ForegroundColor White
foreach ($test in $testResults.GetEnumerator()) {
    $status = switch ($test.Value) {
        $true { "✅ BAŞARILI" }
        $false { "❌ BAŞARISIZ" }
        $null { "⏭️ ATLADI" }
        default { "❓ BİLİNMİYOR" }
    }

    $color = switch ($test.Value) {
        $true { "Green" }
        $false { "Red" }
        $null { "Yellow" }
        default { "Gray" }
    }

    Write-Host "  $($test.Key): $status" -ForegroundColor $color
}

Write-Host "`n📈 Genel Sonuç: $passedTests/$totalValidTests test başarılı" -ForegroundColor Yellow

if ($passedTests -eq $totalValidTests) {
    Write-Host "🎉 Tüm PWA testleri başarılı! Uygulamanız PWA özelliklerine hazır." -ForegroundColor Green
    Write-TestLog "Tüm testler başarılı" "SUCCESS"
} elseif ($passedTests -ge ($totalValidTests * 0.7)) {
    Write-Host "⚠️ Çoğu PWA testi başarılı. Küçük iyileştirmeler gerekebilir." -ForegroundColor Yellow
    Write-TestLog "Çoğu test başarılı, iyileştirme gerekebilir" "WARN"
} else {
    Write-Host "❌ PWA testlerinin çoğu başarısız. Yapılandırma kontrolü gerekebilir." -ForegroundColor Red
    Write-TestLog "Çoğu test başarısız" "ERROR"
}

# 6. PWA kontrol önerileri
Write-Host "`n💡 PWA İyileştirme Önerileri:" -ForegroundColor Cyan
Write-Host "- Manifest.json'da tüm gerekli alanların doldurulduğundan emin olun" -ForegroundColor White
Write-Host "- Service Worker'ın tüm event handler'larını içerdiğinden emin olun" -ForegroundColor White
Write-Host "- Tüm HTML sayfalarında PWA meta etiketlerinin mevcut olduğundan emin olun" -ForegroundColor White
Write-Host "- HTTPS protokolü kullanıldığından emin olun (PWA için zorunlu)" -ForegroundColor White
Write-Host "- Farklı ekran boyutlarında test edin" -ForegroundColor White
Write-Host "- Lighthouse PWA audit'ini çalıştırın" -ForegroundColor White

# 7. Performans istatistikleri
if ($Verbose) {
    Write-Host "`n📈 Performans İstatistikleri:" -ForegroundColor Cyan
    Write-Host "Başlangıç zamanı: $($startTime.ToString('yyyy-MM-dd HH:mm:ss'))" -ForegroundColor Gray
    Write-Host "Bitiş zamanı: $($endTime.ToString('yyyy-MM-dd HH:mm:ss'))" -ForegroundColor Gray
    Write-Host "Toplam süre: $($duration.TotalSeconds.ToString('F2')) saniye" -ForegroundColor Gray
}

Write-Host "`n🔄 Testi tekrar çalıştırmak için:" -ForegroundColor Cyan
Write-Host "  .\Test-PWA.ps1" -ForegroundColor White
Write-Host "  .\Test-PWA.ps1 -Verbose" -ForegroundColor White
Write-Host "  .\Test-PWA.ps1 -SkipNetworkTests" -ForegroundColor White

Write-TestLog "PWA testi tamamlandı. Süre: $($duration.TotalSeconds.ToString('F2')) saniye"
