# =========================================
# KesifApp.com DNS Test Scripti
# =========================================

Write-Host "🔍 DNS Yapılandırma Testi Başlatılıyor..." -ForegroundColor Cyan

$domain = "kesifapp.com"
$githubPages = "vahitkirbiyik.github.io"

# DNS çözümlemesi testi
Write-Host "`n📡 DNS Çözümlemesi Testi:" -ForegroundColor Yellow
try {
    $dnsResult = Resolve-DnsName $domain -Type CNAME -ErrorAction Stop
    Write-Host "✅ CNAME Kaydı Bulundu: $($dnsResult.NameHost)" -ForegroundColor Green
    if ($dnsResult.NameHost -eq $githubPages) {
        Write-Host "✅ CNAME Kaydı Doğru: $githubPages" -ForegroundColor Green
    } else {
        Write-Host "❌ CNAME Kaydı Yanlış: $($dnsResult.NameHost) (Beklenen: $githubPages)" -ForegroundColor Red
    }
}
catch {
    Write-Host "❌ CNAME Kaydı Bulunamadı" -ForegroundColor Red

    # A kayıtlarını kontrol et
    try {
        $aRecords = Resolve-DnsName $domain -Type A -ErrorAction Stop
        $githubIPs = @("185.199.108.153", "185.199.109.153", "185.199.110.153", "185.199.111.153")
        Write-Host "ℹ️ A Kayıtları Kontrol Ediliyor..." -ForegroundColor Yellow

        foreach ($record in $aRecords) {
            if ($githubIPs -contains $record.IPAddress) {
                Write-Host "✅ Geçerli A Kaydı: $($record.IPAddress)" -ForegroundColor Green
            } else {
                Write-Host "❌ Geçersiz A Kaydı: $($record.IPAddress)" -ForegroundColor Red
            }
        }
    }
    catch {
        Write-Host "❌ A Kayıtları da Bulunamadı" -ForegroundColor Red
    }
}

# Web sitesi erişim testi
Write-Host "`n🌐 Web Sitesi Erişim Testi:" -ForegroundColor Yellow
$testUrls = @(
    "https://$domain/",
    "https://www.$domain/",
    "https://$githubPages/"
)

foreach ($url in $testUrls) {
    try {
        $response = Invoke-WebRequest -Uri $url -UseBasicParsing -TimeoutSec 10
        if ($response.StatusCode -eq 200) {
            Write-Host "✅ Erişilebilir: $url" -ForegroundColor Green
        } else {
            Write-Host "❌ HTTP Hatası: $url (Status: $($response.StatusCode))" -ForegroundColor Red
        }
    }
    catch {
        Write-Host "❌ Erişilemiyor: $url" -ForegroundColor Red
    }
}

Write-Host "`n🔧 DNS Yapılandırma Önerileri:" -ForegroundColor Cyan
Write-Host "1. CNAME Kaydı (Önerilen):" -ForegroundColor White
Write-Host "   Type: CNAME" -ForegroundColor White
Write-Host "   Name: kesifapp.com" -ForegroundColor White
Write-Host "   Value: vahitkirbiyik.github.io" -ForegroundColor White
Write-Host "" -ForegroundColor White
Write-Host "2. A Kayıtları (Alternatif):" -ForegroundColor White
Write-Host "   Type: A" -ForegroundColor White
Write-Host "   Name: kesifapp.com" -ForegroundColor White
Write-Host "   Value: 185.199.108.153" -ForegroundColor White
Write-Host "   Value: 185.199.109.153" -ForegroundColor White
Write-Host "   Value: 185.199.110.153" -ForegroundColor White
Write-Host "   Value: 185.199.111.153" -ForegroundColor White

Write-Host "`n⏱️ DNS değişikliklerinin yayılması 24-48 saat sürebilir." -ForegroundColor Yellow
Write-Host "🔄 Testi tekrar çalıştırmak için: .\Test-DNS.ps1" -ForegroundColor Cyan
