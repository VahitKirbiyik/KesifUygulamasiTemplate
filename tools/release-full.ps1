# ================================================================
# TOPLU OLUŞTURMA + KONTROL + ÇALIŞTIRMA BLOĞU
# - Bu blok: klasörleri oluşturur, üç dosyayı yazar (release-full-check.ps1, release-full.ps1, workflow)
# - Sonra release-full-check.ps1'i çalıştırır (kontroller -> eğer geçerse release-full.ps1 çalıştırılır)
# ================================================================

# 1) Gerekli klasörleri oluştur
New-Item -ItemType Directory -Force -Path ".\tools" | Out-Null
New-Item -ItemType Directory -Force -Path ".\.github\workflows" | Out-Null
New-Item -ItemType Directory -Force -Path ".\Keys" | Out-Null
New-Item -ItemType Directory -Force -Path ".\certs" | Out-Null

# 2) release-full.ps1 (güvenli, güncel sürüm) - yaz
@'
# =====================================
# release-full.ps1
# Tam otomatik + güvenli CI/CD / beta & release script
# =====================================

# Fail fast
$ErrorActionPreference = 'Stop'

try {
    # ------------ LOG / TRANSCRIPT ------------
    $gitRoot = (& git rev-parse --show-toplevel 2>$null).Trim()
    if (-not $gitRoot) {
        # Git yoksa bulunduğumuz dizini kullan
        $gitRoot = (Resolve-Path ".").Path
        Write-Warning "Git repository bulunamadı. Bazı adımlar (changelog, git commit) atlanabilir."
    }
    Set-Location $gitRoot

    $time = (Get-Date).ToString('yyyyMMdd-HHmmss')
    $logFile = Join-Path $gitRoot ("release-full-$time.log")
    Write-Host "[INFO] Transcript başlatılıyor: $logFile"
    Start-Transcript -Path $logFile -Force

    # ------------ PATH & FILES ------------
    $versionFile = Join-Path $gitRoot "VERSION.txt"
    $androidManifest = Join-Path $gitRoot "Android\Properties\AndroidManifest.xml"
    $iosInfoPlist = Join-Path $gitRoot "iOS\Info.plist"
    $outAndroid = Join-Path $gitRoot "out\Android"
    $outiOS = Join-Path $gitRoot "out\iOS"
    $keystorePath = Join-Path $gitRoot "Keys\release.keystore"
    $appleCertPath = Join-Path $gitRoot "certs\apple_certificate.p12"
    $googleKeyPath = Join-Path $gitRoot "certs\google_play_service_account.json"

    # ------------ PRE-HOOKS ------------
    Write-Host "[INFO] Hook kontrolleri (pre-commit, pre-push, commit-msg) başlatılıyor..."
    if (Test-Path (Join-Path $gitRoot "pre-commit.ps1")) { & (Join-Path $gitRoot "pre-commit.ps1") }
    if (Test-Path (Join-Path $gitRoot "pre-push.ps1")) { & (Join-Path $gitRoot "pre-push.ps1") }

    # ------------ RESX CHECK ------------
    Write-Host "[INFO] .resx anahtar doğrulaması (varsa) çalıştırılıyor..."
    if (Test-Path (Join-Path $gitRoot "Tools\ResxKeyChecker")) {
        try {
            dotnet run --project (Join-Path $gitRoot "Tools\ResxKeyChecker") --verify-all >> (Join-Path $gitRoot "pre-commit.log") 2>&1
        } catch {
            Write-Warning "[WARN] Resx doğrulayıcı çalıştırılırken hata: $($_.Exception.Message). pre-commit.log'u kontrol edin."
        }
    } else {
        Write-Host "[INFO] Resx checker aracı bulunamadı, bu adım atlandı."
    }

    # ------------ FEATURE TESTS ------------
    Write-Host "[INFO] Tüm özellik testleri çalıştırılıyor..."
    $testSucceeded = $true
    try {
        dotnet test --filter "Category=FeatureTest" --logger trx --results-directory (Join-Path $gitRoot "TestResults") >> (Join-Path $gitRoot "pre-commit.log") 2>&1
        if ($LASTEXITCODE -ne 0) { $testSucceeded = $false }
    } catch {
        $testSucceeded = $false
    }
    if (-not $testSucceeded) {
        Write-Error "[ERROR] Feature testleri başarısız. pre-commit.log'u inceleyin."
        exit 1
    }

    # ------------ VERSION FILE & BUMP ------------
    Write-Host "[INFO] Sürüm ve build numarası işleniyor..."
    if (-not (Test-Path $versionFile)) {
        Write-Warning "[WARN] VERSION.txt bulunamadı; oluşturuluyor (1.0.0)."
        "1.0.0" | Out-File -FilePath $versionFile -Encoding UTF8
    }
    $versionRaw = (Get-Content $versionFile -Raw).Trim()
    if (-not ($versionRaw -match '^\d+\.\d+\.\d+$')) {
        Write-Warning "[WARN] VERSION.txt formatı beklenenden farklı. Varsayılan 1.0.0 kullanılıyor."
        $versionRaw = "1.0.0"
    }
    $versionParts = $versionRaw.Split('.')
    if ($versionParts.Length -lt 3) {
        $versionParts = @($versionParts[0], ($versionParts[1] -as [int] -as [string]), "0")
    }
    $patch = [int]$versionParts[2] + 1
    $newVersion = "$($versionParts[0]).$($versionParts[1]).$patch"
    Set-Content -Path $versionFile -Value $newVersion -Encoding UTF8
    Write-Host "[INFO] VERSION.txt güncellendi -> $newVersion"

    # ------------ ANDROID / iOS VERSION UPDATE (güvenli) ------------
    if (Test-Path $androidManifest) {
        try {
            $manifestRaw = Get-Content $androidManifest -Raw
            $manifestRaw = $manifestRaw -replace 'android:versionCode="\d+"', ("android:versionCode=`"$patch`"")
            $manifestRaw = $manifestRaw -replace 'android:versionName=".*?"', ("android:versionName=`"$newVersion`"")
            $manifestRaw | Set-Content -Path $androidManifest -Encoding UTF8
            Write-Host "[INFO] AndroidManifest.xml sürüm güncellendi."
        } catch {
            Write-Warning "[WARN] AndroidManifest güncellemesi sırasında hata: $($_.Exception.Message)"
        }
    } else {
        Write-Warning "[WARN] AndroidManifest.xml bulunamadı; android versiyon güncellemesi atlandı."
    }

    if (-not $IsWindows) {
        if (Test-Path $iosInfoPlist) {
            try {
                $plistRaw = Get-Content $iosInfoPlist -Raw
                $plistRaw = $plistRaw -replace '<key>CFBundleShortVersionString</key>\s*<string>.*?</string>', "<key>CFBundleShortVersionString</key><string>$newVersion</string>"
                $plistRaw = $plistRaw -replace '<key>CFBundleVersion</key>\s*<string>.*?</string>', "<key>CFBundleVersion</key><string>$patch</string>"
                $plistRaw | Set-Content -Path $iosInfoPlist -Encoding UTF8
                Write-Host "[INFO] Info.plist sürüm güncellendi."
            } catch {
                Write-Warning "[WARN] Info.plist güncellemesi sırasında hata: $($_.Exception.Message)"
            }
        } else {
            Write-Warning "[WARN] Info.plist bulunamadı; iOS versiyon güncellemesi atlandı."
        }
    } else {
        Write-Host "[INFO] iOS sürüm güncellemesi atlandı (Windows ortamında)."
    }

    # ------------ CHANGELOG ------------
    Write-Host "[INFO] Changelog oluşturuluyor..."
    try {
        if (& git rev-parse --is-inside-work-tree 2>$null) {
            $tag = (& git describe --tags --abbrev=0 2>$null).Trim()
            if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrEmpty($tag)) {
                & git log --pretty=format:"- %s" HEAD > (Join-Path $gitRoot "CHANGELOG.md")
            } else {
                & git log --pretty=format:"- %s" "$tag..HEAD" > (Join-Path $gitRoot "CHANGELOG.md")
            }
            Write-Host "[INFO] CHANGELOG.md oluşturuldu."
        } else {
            Write-Warning "[WARN] Git workspace değil; changelog oluşturulmadı."
        }
    } catch {
        Write-Warning "[WARN] Changelog oluşturulurken hata: $($_.Exception.Message)"
    }

    # ------------ BUILD (Android) ------------
    Write-Host "[INFO] Android build başlatılıyor..."
    $androidPublishOk = $false
    try {
        & dotnet publish -c Release -f net8.0-android -o $outAndroid 2>&1 | Tee-Object -FilePath (Join-Path $gitRoot "pre-commit.log")
        if ($LASTEXITCODE -eq 0) { $androidPublishOk = $true; Write-Host "[INFO] Android build başarılı." } else { throw "dotnet publish android exit code $LASTEXITCODE" }
    } catch {
        Write-Error "[ERROR] Android build başarısız: $($_.Exception.Message)"
        exit 1
    }

    # ------------ BUILD (iOS) - only on non-Windows runners ------------
    if (-not $IsWindows) {
        Write-Host "[INFO] iOS build başlatılıyor..."
        try {
            & dotnet publish -c Release -f net8.0-ios -o $outiOS 2>&1 | Tee-Object -FilePath (Join-Path $gitRoot "pre-commit.log")
            if ($LASTEXITCODE -ne 0) { throw "dotnet publish ios exit code $LASTEXITCODE" }
            Write-Host "[INFO] iOS build başarılı."
        } catch {
            Write-Error "[ERROR] iOS build başarısız: $($_.Exception.Message)"
            exit 1
        }
    } else {
        Write-Host "[INFO] iOS build atlandı (Windows ortamı)."
    }

    # ------------ SIGNING (Android) - prefer apksigner ------------
    if ($androidPublishOk) {
        $apk = Get-ChildItem -Path $outAndroid -Filter *.apk -Recurse -ErrorAction SilentlyContinue | Select-Object -First 1
        if ($apk) {
            $apksigner = $null
            if ($env:ANDROID_SDK_ROOT) {
                $buildTools = Get-ChildItem -Path (Join-Path $env:ANDROID_SDK_ROOT "build-tools") -ErrorAction SilentlyContinue | Sort-Object Name -Descending | Select-Object -First 1
                if ($buildTools) { $apksigner = Join-Path $buildTools.FullName "apksigner.bat" }
            }
            if (-not $apksigner -and (Get-Command "apksigner" -ErrorAction SilentlyContinue)) {
                $apksigner = (Get-Command "apksigner").Source
            }

            if ($apksigner -and (Test-Path $keystorePath)) {
                Write-Host "[INFO] APK imzalama başlıyor..."
                $ksPassArg = if ($env:KEYSTORE_PASS) { "--ks-pass pass:$env:KEYSTORE_PASS" } else { "" }
                & $apksigner sign --ks $keystorePath $ksPassArg --ks-key-alias $env:KEYSTORE_ALIAS $apk.FullName
                if ($LASTEXITCODE -ne 0) { Write-Error "[ERROR] APK imzalama başarısız"; exit 1 }
                Write-Host "[INFO] APK imzalama tamamlandı."
            } else {
                Write-Warning "[WARN] apksigner bulunamadı veya keystore yok. APK imzalama atlandı."
            }
        } else {
            Write-Warning "[WARN] out/Android dizininde APK bulunamadı; imzalama atlandı."
        }
    }

    # ------------ FASTLANE / iOS DISTRIBUTION ------------
    if (Get-Command "fastlane" -ErrorAction SilentlyContinue) {
        if (-not $IsWindows) {
            if (Test-Path (Join-Path $gitRoot "ios")) {
                Push-Location (Join-Path $gitRoot "ios")
                try {
                    Write-Host "[INFO] fastlane match çalıştırılıyor..."
                    & fastlane match appstore
                    if ($LASTEXITCODE -ne 0) { throw "fastlane match failed" }
                    Write-Host "[INFO] fastlane ios beta çalıştırılıyor..."
                    & fastlane ios beta
                    if ($LASTEXITCODE -ne 0) { throw "fastlane ios beta failed" }
                } finally {
                    Pop-Location
                }
            } else {
                Write-Warning "[WARN] ios klasörü bulunamadı; fastlane atlandı."
            }
        } else {
            Write-Host "[INFO] fastlane atlandı (Windows). iOS dağıtımını CI macOS runner'da çalıştırın."
        }
    } else {
        Write-Warning "[WARN] fastlane yüklü değil; iOS dağıtımı atlandı."
    }

    # ------------ PRE-COMMIT LOG ANALİZİ ------------
    $preCommitLog = Join-Path $gitRoot "pre-commit.log"
    if (Test-Path $preCommitLog) {
        $matches = Select-String -Path $preCommitLog -Pattern "error|fail|exception" -SimpleMatch -CaseSensitive:$false -Quiet
        if ($matches) {
            Write-Error "[ERROR] pre-commit.log içinde hata/uyarı bulundu. Lütfen $preCommitLog dosyasını inceleyin."
            exit 1
        } else {
            Write-Host "[INFO] pre-commit.log temiz."
        }
    }

    # ------------ GIT COMMIT (sadece değişiklik varsa) ------------
    try {
        if (& git rev-parse --is-inside-work-tree 2>$null) {
            $porcelain = (& git status --porcelain).Trim()
            if (-not [string]::IsNullOrEmpty($porcelain)) {
                Write-Host "[INFO] Değişiklikler bulundu, commit atılıyor..."
                & git add .
                & git commit -m "All features tested, version updated to $newVersion, changelog generated, builds ready, beta prepared"
                Write-Host "[INFO] Commit başarıyla tamamlandı."
            } else {
                Write-Host "[INFO] Commit için değişiklik yok. Atlanıyor."
            }
        } else {
            Write-Warning "[WARN] Git repository değil; commit atlanıyor."
        }
    } catch {
        Write-Warning "[WARN] Git commit sırasında hata: $($_.Exception.Message)"
    }

    # ------------ YARI-OTOMATİK BETA/RELEASE REHBERİ ------------
    Write-Host ""
    Write-Host "==============================="
    Write-Host "YARI-OTOMATİK BETA/RELEASE ADIMLARI"
    Write-Host "==============================="
    if (-not (Test-Path $appleCertPath)) { Write-Warning "[WARN] Apple sertifikası bulunamadı: $appleCertPath" } else { Write-Host "[INFO] Apple sertifikası bulundu." }
    if (-not (Test-Path $googleKeyPath)) { Write-Warning "[WARN] Google Play API key bulunamadı: $googleKeyPath" } else { Write-Host "[INFO] Google Play API key bulundu." }
    Write-Host "[INFO] TestFlight beta yüklemek için Fastlane komutlarını CI macOS runner'da çalıştırın:"
    Write-Host "       fastlane match appstore"
    Write-Host "       fastlane ios beta"
    Write-Host "[INFO] Play Store Beta için Google Play Console veya Fastlane supply kullanın (PLAYSTORE_JSON secret)."
    Write-Host "[INFO] Android/iOS beta cihazlarında uygulamayı test edin."
    Write-Host "[INFO] App Store / Play Store metadata, ikon, ekran görüntülerini kontrol edin."

    Write-Host "[INFO] İşlem başarılı. Artefactler $outAndroid ve $outiOS içinde (varsa). Log: $logFile"
}
catch {
    Write-Error "[FATAL] İşlem sırasında hata: $($_.Exception.Message)"
    exit 1
}
finally {
    try {
        Stop-Transcript | Out-Null
    } catch {}
}
'@ | Set-Content -Path ".\tools\release-full.ps1" -Encoding UTF8

# 3) release-full-check.ps1 (kontrol + otomatik küçük düzeltmeler) - yaz
@'
# =================================================================================
# release-full-check.ps1
# release-full.ps1 çalıştırmadan önce eksik dosya, araç ve sertifika kontrollerini yapar.
# =================================================================================

$ErrorActionPreference = 'Stop'

# Repo root tespiti (git varsa git kökünü kullan)
$gitRoot = (& git rev-parse --show-toplevel 2>$null).Trim()
if (-not $gitRoot) { $gitRoot = (Resolve-Path ".").Path }

Write-Host "[INFO] Workspace kökü: $gitRoot"

$scriptPath = Join-Path $gitRoot "tools\release-full.ps1"
$versionFile = Join-Path $gitRoot "VERSION.txt"
$androidManifest = Join-Path $gitRoot "Android\Properties\AndroidManifest.xml"
$iosInfoPlist = Join-Path $gitRoot "iOS\Info.plist"
$gitFolder = Join-Path $gitRoot ".git"

$errors = @()

if (-not (Test-Path $scriptPath)) { $errors += "❌ release-full.ps1 dosyası bulunamadı: $scriptPath" }
if (-not (Test-Path $versionFile)) {
    $errors += "⚠ VERSION.txt bulunamadı, otomatik oluşturuluyor."
    "1.0.0" | Out-File -FilePath $versionFile -Encoding UTF8
}
if (-not (Test-Path $androidManifest)) { $errors += "⚠ AndroidManifest.xml bulunamadı: $androidManifest" }
if (-not (Test-Path $iosInfoPlist)) { $errors += "⚠ Info.plist bulunamadı: $iosInfoPlist" }
if (-not (Test-Path $gitFolder)) { $errors += "⚠ .git klasörü bulunamadı; git ile çalışan adımlar atlanacaktır." }

# Araç kontrolleri
if (-not (Get-Command "apksigner" -ErrorAction SilentlyContinue) -and -not (Get-Command "jarsigner" -ErrorAction SilentlyContinue)) {
    $errors += "⚠ apksigner/jarsigner bulunamadı. Android imzalama için Android SDK veya JDK gereklidir."
}
if (-not (Get-Command "fastlane" -ErrorAction SilentlyContinue)) { $errors += "⚠ fastlane bulunamadı. iOS dağıtımı için fastlane gereklidir (CI macOS runner)." }

# Sertifika/anahtar kontrolü
$appleCertPath = Join-Path $gitRoot "certs\apple_certificate.p12"
$googleKeyPath = Join-Path $gitRoot "certs\google_play_service_account.json"
if (-not (Test-Path $appleCertPath)) { $errors += "⚠ Apple sertifikası bulunamadı: $appleCertPath" }
if (-not (Test-Path $googleKeyPath)) { $errors += "⚠ Google Play API key bulunamadı: $googleKeyPath" }

if ($errors.Count -gt 0) {
    Write-Host "======================= KONTROL RAPORU ======================="
    $errors | ForEach-Object { Write-Host $_ }
    Write-Host "Lütfen eksikleri tamamladıktan sonra scripti tekrar çalıştırın."
    Write-Host "=============================================================="
    exit 1
}

Write-Host "[INFO] Tüm ön kontroller geçti. release-full.ps1 çalıştırılıyor..."
# release-full.ps1 çalıştır
& $scriptPath
'@ | Set-Content -Path ".\tools\release-full-check.ps1" -Encoding UTF8

# 4) GitHub Actions workflow - yaz
@'
name: Full Release Pipeline

on:
  push:
    branches:
      - main
  workflow_dispatch:

jobs:
  build_windows_android:
    name: Build & Release (Windows - Android)
    runs-on: windows-latest
    env:
      KEYSTORE_PASS: ${{ secrets.KEYSTORE_PASS }}
      KEYSTORE_ALIAS: ${{ secrets.KEYSTORE_ALIAS }}
      PLAYSTORE_JSON: ${{ secrets.PLAYSTORE_JSON }}
    steps:
      - name: Checkout code
        uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'

      - name: Run release-full-check and release (Windows)
        shell: pwsh
        run: |
          Set-ExecutionPolicy Bypass -Scope Process -Force
          .\tools\release-full-check.ps1

      - name: Upload Artifacts
        if: success()
        uses: actions/upload-artifact@v3
        with:
          name: BuildArtifacts
          path: out/

  build_macos_ios:
    name: Build & Release (macOS - iOS)
    runs-on: macos-latest
    needs: build_windows_android
    env:
      APP_STORE_CONNECT_API_KEY: ${{ secrets.APP_STORE_CONNECT_API_KEY }}
      FASTLANE_PASSWORD: ${{ secrets.FASTLANE_PASSWORD }}
    steps:
      - name: Checkout code
        uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'

      - name: Run release-full-check and release (macOS)
        shell: bash
        run: |
          sudo gem install fastlane -v 2.0 --no-document || true
          pwsh -c './tools/release-full-check.ps1'
'@ | Set-Content -Path ".\.github\workflows\release-full.yml" -Encoding UTF8

# 5) İzinler (Linux/macOS runner'lar için) - Windows'ta gerekli değil
if ($IsLinux -or $IsMacOS) {
    chmod +x .\tools\release-full.ps1 2>$null
    chmod +x .\tools\release-full-check.ps1 2>$null
}

# 6) Son adım: kontrol scriptini çalıştır
Write-Host "==============================="
Write-Host "ÖN KONTROL: release-full-check.ps1 çalıştırılıyor..."
Write-Host "Eğer eksik bir şey varsa script açıkça bildirecek."
Write-Host "==============================="
powershell -NoProfile -ExecutionPolicy Bypass -File ".\tools\release-full-check.ps1"
