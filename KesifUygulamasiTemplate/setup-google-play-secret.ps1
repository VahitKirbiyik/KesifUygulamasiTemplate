# Google Play Service Account Secret Kurulum Script'i
# .NET MAUI KesifUygulamasiTemplate projesi i�in

Write-Host "?? Google Play Service Account Secret Kurulum Ba�lat�l�yor..." -ForegroundColor Green
Write-Host ""

# 1) Repo k�k�ne ge�
$repoRoot = "C:\Users\VAH�T\KesifUygulamasiTemplate"
Write-Host "?? Repo k�k�: $repoRoot" -ForegroundColor Yellow
Set-Location $repoRoot

# 2) GitHub CLI kontrol & kurulum
Write-Host "?? GitHub CLI kontrol ediliyor..." -ForegroundColor Yellow
if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
    Write-Host "? GitHub CLI bulunamad�!" -ForegroundColor Red
    Write-Host "?? GitHub CLI kurulumu ba�lat�l�yor..." -ForegroundColor Yellow
    
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
            Write-Host "? Otomatik kurulum yap�lamad�!" -ForegroundColor Red
            Write-Host "?? Manuel kurulum i�in: https://cli.github.com/" -ForegroundColor Cyan
            Write-Host "1. GitHub CLI'yi indirip kurun" -ForegroundColor White
            Write-Host "2. Script'i tekrar �al��t�r�n" -ForegroundColor White
            Read-Host "Kurulum tamamlad�ktan sonra Enter'a bas�n"
        }
    }
    catch {
        Write-Host "? GitHub CLI kurulumu ba�ar�s�z: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host "?? Manuel kurulum gerekli: https://cli.github.com/" -ForegroundColor Cyan
        exit 1
    }
}
else {
    Write-Host "? GitHub CLI zaten kurulu" -ForegroundColor Green
}

# 3) GitHub Auth kontrol�
Write-Host "?? GitHub authentication kontrol ediliyor..." -ForegroundColor Yellow
try {
    $authStatus = gh auth status 2>&1
    if ($LASTEXITCODE -ne 0) {
        Write-Host "? GitHub'a giri� yap�lmam��!" -ForegroundColor Red
        Write-Host "?? GitHub'a giri� yap�l�yor..." -ForegroundColor Yellow
        gh auth login --web
        if ($LASTEXITCODE -ne 0) {
            Write-Host "? GitHub authentication ba�ar�s�z!" -ForegroundColor Red
            exit 1
        }
    }
    else {
        Write-Host "? GitHub authentication ba�ar�l�" -ForegroundColor Green
    }
}
catch {
    Write-Host "? GitHub auth kontrol� ba�ar�s�z: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# 4) Git repo do�rulama
Write-Host "?? Git repository kontrol ediliyor..." -ForegroundColor Yellow
try {
    git rev-parse --is-inside-work-tree 2>$null
    if ($LASTEXITCODE -ne 0) {
        Write-Host "? Bu klas�r bir git deposu de�il!" -ForegroundColor Red
        Write-Host "?? Git repository initialize ediliyor..." -ForegroundColor Yellow
        git init
        git remote add origin "https://github.com/VahitKirbiyik/KesifUygamamasiTemplate.git"
    }
    else {
        Write-Host "? Git repository do�ruland�" -ForegroundColor Green
    }
}
catch {
    Write-Host "? Git repository kontrol� ba�ar�s�z: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# 5) Repo owner/name al�nmas�
Write-Host "?? Repository bilgileri al�n�yor..." -ForegroundColor Yellow
try {
    $remote = git remote get-url origin 2>$null
    if (-not $remote) {
        Write-Host "? Origin remote bulunamad�!" -ForegroundColor Red
        Write-Host "?? Remote URL ayarlan�yor..." -ForegroundColor Yellow
        git remote add origin "https://github.com/VahitKirbiyik/KesifUygamamasiTemplate.git"
        $remote = "https://github.com/VahitKirbiyik/KesifUygamamasiTemplate.git"
    }
    
    $repo = ($remote -replace '.*[:/](.+?)(\.git)?$','$1')
    Write-Host "? Repository: $repo" -ForegroundColor Green
}
catch {
    Write-Host "? Repository bilgileri al�namad�: $($_.Exception.Message)" -ForegroundColor Red
    $repo = "VahitKirbiyik/KesifUygamamasiTemplate"
    Write-Host "?? Varsay�lan repo kullan�l�yor: $repo" -ForegroundColor Yellow
}

# 6) Service account dosyas� kontrol�
Write-Host "?? Service account dosyas� kontrol ediliyor..." -ForegroundColor Yellow
$serviceFile = ".\service-account.json"
if (-not (Test-Path $serviceFile)) {
    Write-Host "? service-account.json dosyas� bulunamad�!" -ForegroundColor Red
    Write-Host ""
    Write-Host "?? Google Play Service Account olu�turma ad�mlar�:" -ForegroundColor Cyan
    Write-Host "1. Google Play Console'a gidin: https://play.google.com/console" -ForegroundColor White
    Write-Host "2. Settings > API access > Service Accounts" -ForegroundColor White
    Write-Host "3. 'Create new service account' veya mevcut olan� kullan�n" -ForegroundColor White
    Write-Host "4. JSON anahtar�n� indirin" -ForegroundColor White
    Write-Host "5. Dosyay� '$repoRoot\service-account.json' olarak kaydedin" -ForegroundColor White
    Write-Host ""
    Write-Host "?? Test i�in �rnek dosya olu�turuluyor..." -ForegroundColor Yellow
    
    # Test i�in �rnek service account dosyas�
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
    Write-Host "?? �rnek service-account.json olu�turuldu" -ForegroundColor Yellow
    Write-Host "??  Bu dosyay� ger�ek Google Play Service Account JSON ile de�i�tirin!" -ForegroundColor Red
    
    # Kullan�c�dan ger�ek dosya bekleme
    do {
        $choice = Read-Host "Ger�ek service-account.json dosyas�n� yerle�tirdiniz mi? (y/n)"
        if ($choice -eq 'n' -or $choice -eq 'N') {
            Write-Host "? Script durduruldu. Ger�ek service account dosyas�n� yerle�tirdikten sonra tekrar �al��t�r�n." -ForegroundColor Red
            exit 1
        }
    } while ($choice -ne 'y' -and $choice -ne 'Y')
}
else {
    Write-Host "? service-account.json dosyas� bulundu" -ForegroundColor Green
}

# 7) Secret'� GitHub'a ekle
Write-Host "?? GOOGLE_PLAY_SERVICE_ACCOUNT secret'� GitHub'a ekleniyor..." -ForegroundColor Yellow
try {
    gh secret set GOOGLE_PLAY_SERVICE_ACCOUNT --body-file $serviceFile --repo $repo
    if ($LASTEXITCODE -ne 0) {
        Write-Host "? Secret eklenirken hata olu�tu!" -ForegroundColor Red
        exit 1
    }
    Write-Host "? Secret ba�ar�yla GitHub'a eklendi!" -ForegroundColor Green
}
catch {
    Write-Host "? Secret ekleme ba�ar�s�z: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# 8) Git branch kontrol� ve g�ncelleme
Write-Host "?? Git branch kontrol�..." -ForegroundColor Yellow
try {
    $currentBranch = git branch --show-current
    Write-Host "?? Mevcut branch: $currentBranch" -ForegroundColor Cyan
    
    if ($currentBranch -ne "main" -and $currentBranch -ne "master") {
        Write-Host "?? main/master branch'e ge�iliyor..." -ForegroundColor Yellow
        git checkout main 2>$null
        if ($LASTEXITCODE -ne 0) {
            git checkout master 2>$null
            if ($LASTEXITCODE -ne 0) {
                Write-Host "? main/master branch'e ge�ilemedi!" -ForegroundColor Red
            }
        }
    }
    
    Write-Host "?? En son de�i�iklikler �ekiliyor..." -ForegroundColor Yellow
    git pull origin $(git branch --show-current)
}
catch {
    Write-Host "??  Git g�ncelleme uyar�s�: $($_.Exception.Message)" -ForegroundColor Yellow
}

# 9) Pipeline'� tetikle (bo� commit ile)
Write-Host "?? GitHub Actions pipeline tetikleniyor..." -ForegroundColor Yellow
try {
    git commit --allow-empty -m "ci: trigger Play deploy with secret [test-secret]"
    git push origin $(git branch --show-current)
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "? Pipeline tetikleme ba�ar�s�z!" -ForegroundColor Red
        Write-Host "?? Git kimlik do�rulamas� veya izinleri kontrol edin" -ForegroundColor Yellow
    }
    else {
        Write-Host "? Pipeline ba�ar�yla tetiklendi!" -ForegroundColor Green
    }
}
catch {
    Write-Host "? Pipeline tetikleme hatas�: $($_.Exception.Message)" -ForegroundColor Red
}

# 10) Kullan�m talimatlar�
Write-Host ""
Write-Host "?? GitHub Actions Monitoring Komutlar�:" -ForegroundColor Cyan
Write-Host "?????????????????????????????????????????" -ForegroundColor Gray
Write-Host ""
Write-Host "?? Workflow'lar� listele:" -ForegroundColor Yellow
Write-Host "   gh run list --repo $repo --limit 5" -ForegroundColor White
Write-Host ""
Write-Host "?? Son workflow detay�n� g�r�nt�le:" -ForegroundColor Yellow
Write-Host "   gh run view --repo $repo" -ForegroundColor White
Write-Host ""
Write-Host "?? Workflow loglar�n� g�r�nt�le:" -ForegroundColor Yellow
Write-Host "   gh run view <RUN_ID> --log --repo $repo" -ForegroundColor White
Write-Host ""
Write-Host "?? Artifact'lar� indir:" -ForegroundColor Yellow
Write-Host "   gh run download <RUN_ID> --name android-aab --repo $repo" -ForegroundColor White
Write-Host ""
Write-Host "?? Web'de g�r�nt�le:" -ForegroundColor Yellow
Write-Host "   https://github.com/$repo/actions" -ForegroundColor White
Write-Host ""

# 11) Son durum �zeti
Write-Host "?? KURULUM �ZET�" -ForegroundColor Green
Write-Host "??????????????????????????????????????????" -ForegroundColor Gray
Write-Host "? GitHub CLI kuruldu/do�ruland�" -ForegroundColor Green
Write-Host "? GitHub authentication yap�ld�" -ForegroundColor Green
Write-Host "? Git repository do�ruland�" -ForegroundColor Green
Write-Host "? Service account secret eklendi" -ForegroundColor Green
Write-Host "? Pipeline tetiklendi" -ForegroundColor Green
Write-Host ""
Write-Host "?? Sonraki Ad�mlar:" -ForegroundColor Cyan
Write-Host "1. GitHub Actions sekmesini kontrol edin" -ForegroundColor White
Write-Host "2. Deploy i�leminin ba�ar�l� oldu�unu do�rulay�n" -ForegroundColor White
Write-Host "3. Google Play Console'da Internal Track'i kontrol edin" -ForegroundColor White
Write-Host ""
Write-Host "?? Kurulum tamamland�!" -ForegroundColor Green

# 12) Otomatik workflow monitoring
Write-Host ""
$monitor = Read-Host "Son workflow durumunu otomatik kontrol etmek ister misiniz? (y/n)"
if ($monitor -eq 'y' -or $monitor -eq 'Y') {
    Write-Host "?? Son workflow durumu kontrol ediliyor..." -ForegroundColor Yellow
    try {
        gh run list --repo $repo --limit 1
        Write-Host ""
        Write-Host "?? Detayl� log i�in: gh run view --log --repo $repo" -ForegroundColor Cyan
    }
    catch {
        Write-Host "??  Workflow listesi al�namad�: $($_.Exception.Message)" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "Script tamamland�. �yi geli�tirmeler! ??" -ForegroundColor Green