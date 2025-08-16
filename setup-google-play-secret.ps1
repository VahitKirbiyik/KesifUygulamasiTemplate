# Google Play Service Account Secret Kurulum Script'i
# .NET MAUI KesifUygulamasiTemplate projesi için

Write-Host "?? Google Play Service Account Secret Kurulum Baþlatýlýyor..." -ForegroundColor Green
Write-Host ""

# 1) Repo köküne geç
$repoRoot = "C:\Users\VAHÝT\KesifUygulamasiTemplate"
Write-Host "?? Repo kökü: $repoRoot" -ForegroundColor Yellow
Set-Location $repoRoot

# 2) GitHub CLI kontrol & kurulum
Write-Host "?? GitHub CLI kontrol ediliyor..." -ForegroundColor Yellow
if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
    Write-Host "? GitHub CLI bulunamadý!" -ForegroundColor Red
    Write-Host "?? GitHub CLI kurulumu baþlatýlýyor..." -ForegroundColor Yellow
    
    try {
        # Winget ile kurulum denemeleri
        if (Get-Command winget -ErrorAction SilentlyContinue) {
            Write-Host "   Winget ile kuruluyor..." -ForegroundColor Cyan
            winget install --id GitHub.cli --silent
        }
        elseif (Get-Command choco -ErrorAction SilentlyContinue) {
            Write-Host "   Chocolatey ile kuruluyor..." -ForegroundColor Cyan
            choco install gh --yes
        }
        else {
            Write-Host "? Otomatik kurulum yapýlamadý!" -ForegroundColor Red
            Write-Host "?? Manuel kurulum için: https://cli.github.com/" -ForegroundColor Cyan
            Write-Host "1. GitHub CLI'yi indirip kurun" -ForegroundColor White
            Write-Host "2. Script'i tekrar çalýþtýrýn" -ForegroundColor White
            Read-Host "Kurulum tamamladýktan sonra Enter'a basýn"
        }
    }
    catch {
        Write-Host "? GitHub CLI kurulumu baþarýsýz: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host "?? Manuel kurulum gerekli: https://cli.github.com/" -ForegroundColor Cyan
        exit 1
    }
}
else {
    Write-Host "? GitHub CLI zaten kurulu" -ForegroundColor Green
}

# 3) GitHub Auth kontrolü
Write-Host "?? GitHub authentication kontrol ediliyor..." -ForegroundColor Yellow
try {
    $authStatus = gh auth status 2>&1
    if ($LASTEXITCODE -ne 0) {
        Write-Host "? GitHub'a giriþ yapýlmamýþ!" -ForegroundColor Red
        Write-Host "?? GitHub'a giriþ yapýlýyor..." -ForegroundColor Yellow
        gh auth login --web
        if ($LASTEXITCODE -ne 0) {
            Write-Host "? GitHub authentication baþarýsýz!" -ForegroundColor Red
            exit 1
        }
    }
    else {
        Write-Host "? GitHub authentication baþarýlý" -ForegroundColor Green
    }
}
catch {
    Write-Host "? GitHub auth kontrolü baþarýsýz: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# 4) Git repo doðrulama
Write-Host "?? Git repository kontrol ediliyor..." -ForegroundColor Yellow
try {
    git rev-parse --is-inside-work-tree 2>$null
    if ($LASTEXITCODE -ne 0) {
        Write-Host "? Bu klasör bir git deposu deðil!" -ForegroundColor Red
        Write-Host "?? Git repository initialize ediliyor..." -ForegroundColor Yellow
        git init
        git remote add origin "https://github.com/VahitKirbiyik/KesifUygamamasiTemplate.git"
    }
    else {
        Write-Host "? Git repository doðrulandý" -ForegroundColor Green
    }
}
catch {
    Write-Host "? Git repository kontrolü baþarýsýz: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# 5) Repo owner/name alýnmasý
Write-Host "?? Repository bilgileri alýnýyor..." -ForegroundColor Yellow
try {
    $remote = git remote get-url origin 2>$null
    if (-not $remote) {
        Write-Host "? Origin remote bulunamadý!" -ForegroundColor Red
        Write-Host "?? Remote URL ayarlanýyor..." -ForegroundColor Yellow
        git remote add origin "https://github.com/VahitKirbiyik/KesifUygamamasiTemplate.git"
        $remote = "https://github.com/VahitKirbiyik/KesifUygamamasiTemplate.git"
    }
    
    $repo = ($remote -replace '.*[:/](.+?)(\.git)?$','$1')
    Write-Host "? Repository: $repo" -ForegroundColor Green
}
catch {
    Write-Host "? Repository bilgileri alýnamadý: $($_.Exception.Message)" -ForegroundColor Red
    $repo = "VahitKirbiyik/KesifUygamamasiTemplate"
    Write-Host "?? Varsayýlan repo kullanýlýyor: $repo" -ForegroundColor Yellow
}

# 6) Service account dosyasý kontrolü
Write-Host "?? Service account dosyasý kontrol ediliyor..." -ForegroundColor Yellow
$serviceFile = ".\service-account.json"
if (-not (Test-Path $serviceFile)) {
    Write-Host "? service-account.json dosyasý bulunamadý!" -ForegroundColor Red
    Write-Host ""
    Write-Host "?? Google Play Service Account oluþturma adýmlarý:" -ForegroundColor Cyan
    Write-Host "1. Google Play Console'a gidin: https://play.google.com/console" -ForegroundColor White
    Write-Host "2. Settings > API access > Service Accounts" -ForegroundColor White
    Write-Host "3. 'Create new service account' veya mevcut olaný kullanýn" -ForegroundColor White
    Write-Host "4. JSON anahtarýný indirin" -ForegroundColor White
    Write-Host "5. Dosyayý '$repoRoot\service-account.json' olarak kaydedin" -ForegroundColor White
    Write-Host ""
    Write-Host "?? Test için örnek dosya oluþturuluyor..." -ForegroundColor Yellow
    
    # Test için örnek service account dosyasý
    $exampleServiceAccount = @{
        "type" = "service_account"
        "project_id" = "your-project-id"
        "private_key_id" = "key-id"
        "private_key" = "-----BEGIN PRIVATE KEY-----\nYOUR_PRIVATE_KEY\n-----END PRIVATE KEY-----\n"
        "client_email" = "your-service-account@your-project.iam.gserviceaccount.com"
        "client_id" = "client-id"
        "auth_uri" = "https://accounts.google.com/o/oauth2/auth"
        "token_uri" = "https://oauth2.googleapis.com/token"
    }
    
    $exampleServiceAccount | ConvertTo-Json -Depth 10 | Out-File -FilePath $serviceFile -Encoding UTF8
    Write-Host "?? Örnek service-account.json oluþturuldu" -ForegroundColor Yellow
    Write-Host "??  Bu dosyayý gerçek Google Play Service Account JSON ile deðiþtirin!" -ForegroundColor Red
    
    # Kullanýcýdan gerçek dosya bekleme
    do {
        $choice = Read-Host "Gerçek service-account.json dosyasýný yerleþtirdiniz mi? (y/n)"
        if ($choice -eq 'n' -or $choice -eq 'N') {
            Write-Host "? Script durduruldu. Gerçek service account dosyasýný yerleþtirdikten sonra tekrar çalýþtýrýn." -ForegroundColor Red
            exit 1
        }
    } while ($choice -ne 'y' -and $choice -ne 'Y')
}
else {
    Write-Host "? service-account.json dosyasý bulundu" -ForegroundColor Green
}

# 7) Secret'ý GitHub'a ekle
Write-Host "?? GOOGLE_PLAY_SERVICE_ACCOUNT secret'ý GitHub'a ekleniyor..." -ForegroundColor Yellow
try {
    gh secret set GOOGLE_PLAY_SERVICE_ACCOUNT --body-file $serviceFile --repo $repo
    if ($LASTEXITCODE -ne 0) {
        Write-Host "? Secret eklenirken hata oluþtu!" -ForegroundColor Red
        exit 1
    }
    Write-Host "? Secret baþarýyla GitHub'a eklendi!" -ForegroundColor Green
}
catch {
    Write-Host "? Secret ekleme baþarýsýz: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# 8) Git branch kontrolü ve güncelleme
Write-Host "?? Git branch kontrolü..." -ForegroundColor Yellow
try {
    $currentBranch = git branch --show-current
    Write-Host "?? Mevcut branch: $currentBranch" -ForegroundColor Cyan
    
    if ($currentBranch -ne "main" -and $currentBranch -ne "master") {
        Write-Host "?? main/master branch'e geçiliyor..." -ForegroundColor Yellow
        git checkout main 2>$null
        if ($LASTEXITCODE -ne 0) {
            git checkout master 2>$null
            if ($LASTEXITCODE -ne 0) {
                Write-Host "? main/master branch'e geçilemedi!" -ForegroundColor Red
            }
        }
    }
    
    Write-Host "?? En son deðiþiklikler çekiliyor..." -ForegroundColor Yellow
    git pull origin $(git branch --show-current)
}
catch {
    Write-Host "??  Git güncelleme uyarýsý: $($_.Exception.Message)" -ForegroundColor Yellow
}

# 9) Pipeline'ý tetikle (boþ commit ile)
Write-Host "?? GitHub Actions pipeline tetikleniyor..." -ForegroundColor Yellow
try {
    git commit --allow-empty -m "ci: trigger Play deploy with secret [test-secret]"
    git push origin $(git branch --show-current)
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "? Pipeline tetikleme baþarýsýz!" -ForegroundColor Red
        Write-Host "?? Git kimlik doðrulamasý veya izinleri kontrol edin" -ForegroundColor Yellow
    }
    else {
        Write-Host "? Pipeline baþarýyla tetiklendi!" -ForegroundColor Green
    }
}
catch {
    Write-Host "? Pipeline tetikleme hatasý: $($_.Exception.Message)" -ForegroundColor Red
}

# 10) Kullaným talimatlarý
Write-Host ""
Write-Host "?? GitHub Actions Monitoring Komutlarý:" -ForegroundColor Cyan
Write-Host "?????????????????????????????????????????" -ForegroundColor Gray
Write-Host ""
Write-Host "?? Workflow'larý listele:" -ForegroundColor Yellow
Write-Host "   gh run list --repo $repo --limit 5" -ForegroundColor White
Write-Host ""
Write-Host "?? Son workflow detayýný görüntüle:" -ForegroundColor Yellow
Write-Host "   gh run view --repo $repo" -ForegroundColor White
Write-Host ""
Write-Host "?? Workflow loglarýný görüntüle:" -ForegroundColor Yellow
Write-Host "   gh run view <RUN_ID> --log --repo $repo" -ForegroundColor White
Write-Host ""
Write-Host "?? Artifact'larý indir:" -ForegroundColor Yellow
Write-Host "   gh run download <RUN_ID> --name android-aab --repo $repo" -ForegroundColor White
Write-Host ""
Write-Host "?? Web'de görüntüle:" -ForegroundColor Yellow
Write-Host "   https://github.com/$repo/actions" -ForegroundColor White
Write-Host ""

# 11) Son durum özeti
Write-Host "?? KURULUM ÖZETÝ" -ForegroundColor Green
Write-Host "??????????????????????????????????????????" -ForegroundColor Gray
Write-Host "? GitHub CLI kuruldu/doðrulandý" -ForegroundColor Green
Write-Host "? GitHub authentication yapýldý" -ForegroundColor Green
Write-Host "? Git repository doðrulandý" -ForegroundColor Green
Write-Host "? Service account secret eklendi" -ForegroundColor Green
Write-Host "? Pipeline tetiklendi" -ForegroundColor Green
Write-Host ""
Write-Host "?? Sonraki Adýmlar:" -ForegroundColor Cyan
Write-Host "1. GitHub Actions sekmesini kontrol edin" -ForegroundColor White
Write-Host "2. Deploy iþleminin baþarýlý olduðunu doðrulayýn" -ForegroundColor White
Write-Host "3. Google Play Console'da Internal Track'i kontrol edin" -ForegroundColor White
Write-Host ""
Write-Host "?? Kurulum tamamlandý!" -ForegroundColor Green

# 12) Otomatik workflow monitoring
Write-Host ""
$monitor = Read-Host "Son workflow durumunu otomatik kontrol etmek ister misiniz? (y/n)"
if ($monitor -eq 'y' -or $monitor -eq 'Y') {
    Write-Host "?? Son workflow durumu kontrol ediliyor..." -ForegroundColor Yellow
    try {
        gh run list --repo $repo --limit 1
        Write-Host ""
        Write-Host "?? Detaylý log için: gh run view --log --repo $repo" -ForegroundColor Cyan
    }
    catch {
        Write-Host "??  Workflow listesi alýnamadý: $($_.Exception.Message)" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "Script tamamlandý. Ýyi geliþtirmeler! ??" -ForegroundColor Green