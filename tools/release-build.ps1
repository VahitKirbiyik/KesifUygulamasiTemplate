# release-build.ps1
# Tüm test, build, signing ve artefact işlemlerini otomatikleştirir

# 1️⃣ .resx anahtar kontrolü
Write-Host "🔹 Tüm .resx dosyaları ve anahtarlar kontrol ediliyor..."
dotnet run --project Tools/ResxKeyChecker --verify-all >> pre-commit.log 2>&1


# 2️⃣ Özellik testleri
Write-Host "🔹 Uygulama özellikleri test ediliyor..."
dotnet test --filter "Category=FeatureTest" --logger trx --results-directory TestResults >> pre-commit.log 2>&1

# 1️⃣ Hook kontrolleri
Write-Host "🔹 pre-commit, pre-push ve commit-msg kontrolleri..."
if (Test-Path ".\pre-commit.ps1") { & ".\pre-commit.ps1" }
if (Test-Path ".\pre-push.ps1") { & ".\pre-push.ps1" }

# 2️⃣ .resx ve anahtar doğrulama
Write-Host "🔹 26 dil .resx kontrolleri..."
dotnet run --project Tools/ResxKeyChecker --verify-all >> pre-commit.log 2>&1

# 3️⃣ Özellik testleri
Write-Host "🔹 Tüm özellik testleri çalıştırılıyor..."
dotnet test --filter "Category=FeatureTest" --logger trx --results-directory TestResults >> pre-commit.log 2>&1

# 4️⃣ Otomatik sürüm ve build
Write-Host "🔹 Sürüm ve build numarası güncelleniyor..."
$versionFile = "VERSION.txt"
$version = Get-Content $versionFile
$versionParts = $version -split "\."
$versionParts[2] = [int]$versionParts[2] + 1
$newVersion = ($versionParts -join ".")
Set-Content $versionFile $newVersion

# Android ve iOS versiyon güncelle
(Get-Content "Android/Properties/AndroidManifest.xml") -replace 'android:versionCode="\d+"', "android:versionCode=\"$($versionParts[2])\"" | Set-Content "Android/Properties/AndroidManifest.xml"
(Get-Content "Android/Properties/AndroidManifest.xml") -replace 'android:versionName=".*"', "android:versionName=\"$newVersion\"" | Set-Content "Android/Properties/AndroidManifest.xml"

(Get-Content "iOS/Info.plist") -replace '<key>CFBundleShortVersionString</key>\s*<string>.*</string>', "<key>CFBundleShortVersionString</key><string>$newVersion</string>" | Set-Content "iOS/Info.plist"
(Get-Content "iOS/Info.plist") -replace '<key>CFBundleVersion</key>\s*<string>.*</string>', "<key>CFBundleVersion</key><string>$($versionParts[2])</string>" | Set-Content "iOS/Info.plist"

# 5️⃣ Changelog üretimi
Write-Host "🔹 Changelog oluşturuluyor..."
git log --pretty=format:"- %s" $(git describe --tags --abbrev=0)..HEAD > CHANGELOG.md

# 6️⃣ Build işlemleri
Write-Host "🔹 Android build..."
dotnet publish -c Release -f net8.0-android -o out/Android >> pre-commit.log 2>&1
Write-Host "🔹 iOS build..."
dotnet publish -c Release -f net8.0-ios -o out/iOS >> pre-commit.log 2>&1

# 7️⃣ Signing ve artefact yönetimi
$keystorePath = ".\Keys\release.keystore"
$keystorePass = $env:KEYSTORE_PASS
$keystoreAlias = $env:KEYSTORE_ALIAS
& "jarsigner" -verbose -sigalg SHA1withRSA -digestalg SHA1 -keystore $keystorePath out/Android/*.apk $keystoreAlias

cd ios
fastlane match appstore
fastlane ios beta
cd ..

# 8️⃣ Log ve hata kontrolü
if (Test-Path ".\pre-commit.log") {
    $errors = Get-Content ".\pre-commit.log" | Select-String "error|fail"
    if ($errors) {
        Write-Host "❌ Hata bulundu. İşlem durduruldu."
        exit 1
    } else {
        Write-Host "✅ Tüm test, build ve release işlemleri başarılı."
    }
}

# 9️⃣ Git işlemleri
git add .
git commit -m "All features tested, version updated, changelog generated, builds ready, beta prepared"
Write-Host "✅ Commit tamamlandı. Push yapılabilir."

# 1️⃣0️⃣ GitHub Actions workflow entegrasyonu
# .github/workflows/ci-cd.yml dosyası release-build.ps1’i çalıştıracak
# git add .

# PowerShell script to build the project, run tests, generate coverage report, and clean up directories

# Step 1: Build the project
Write-Host "Building the project..."
dotnet build "..\KesifUygulamasiTemplate.csproj" --configuration Release

# Step 2: Run tests and generate coverage report
Write-Host "Running tests and generating coverage report..."
dotnet test "..\KesifUygulamasiTemplate.csproj" --collect:"XPlat Code Coverage"

# Step 3: Clean up unnecessary directories
$directoriesToClean = @(
    "..\bin",
    "..\obj",
    "..\.vs",
    "..\TestResults"
)

foreach ($dir in $directoriesToClean) {
    if (Test-Path $dir) {
        Write-Host "Cleaning up $dir..."
        Remove-Item -Recurse -Force $dir
    }
}

Write-Host "Build, test, and cleanup process completed."

# ✅ Versiyon format doğrulama
$versionFile = "VERSION.txt"
$version = Get-Content $versionFile
if ($version -notmatch '^\d+\.\d+\.\d+$') {
    Write-Host "❌ VERSION.txt formatı geçersiz. x.y.z şeklinde olmalı."
    exit 1
}

# 🔄 Versiyon artırma
$versionParts = $version -split "\."
$versionParts[2] = [int]$versionParts[2] + 1
$newVersion = ($versionParts -join ".")
Set-Content $versionFile $newVersion
$versionCode = $versionParts[2]

# 📱 AndroidManifest.xml güncelleme
(Get-Content "Android/Properties/AndroidManifest.xml") -replace 'android:versionCode="\d+"', "android:versionCode=`"$versionCode`"" | Set-Content "Android/Properties/AndroidManifest.xml"
(Get-Content "Android/Properties/AndroidManifest.xml") -replace 'android:versionName=".*"', "android:versionName=`"$newVersion`"" | Set-Content "Android/Properties/AndroidManifest.xml"

# 🍎 iOS Info.plist güncelleme
(Get-Content "iOS/Info.plist") -replace '<key>CFBundleShortVersionString</key>\s*<string>.*</string>', "<key>CFBundleShortVersionString</key><string>$newVersion</string>" | Set-Content "iOS/Info.plist"
(Get-Content "iOS/Info.plist") -replace '<key>CFBundleVersion</key>\s*<string>.*</string>', "<key>CFBundleVersion</key><string>$versionCode</string>" | Set-Content "iOS/Info.plist"

# 📝 Changelog üretimi
git log --pretty=format:"- %s" $(git describe --tags --abbrev=0)..HEAD > CHANGELOG.md

# 🧪 Test sonuçlarında hata kontrolü
dotnet test --filter "Category=FeatureTest" --logger trx --results-directory TestResults >> pre-commit.log 2>&1
if (Test-Path ".\TestResults") {
    $testErrors = Get-ChildItem ".\TestResults" -Recurse | Select-String "Failed|Error"
    if ($testErrors) {
        Write-Host "❌ Test sonuçlarında hata bulundu. Build durduruldu."
        exit 1
    }
}

# 📦 Artefact boyut kontrolü
$apkPath = "out/Android/app-release.apk"
if (Test-Path $apkPath) {
    $apkSizeMB = (Get-Item $apkPath).Length / 1MB
    if ($apkSizeMB -gt 100) {
        Write-Host "⚠️ APK boyutu $([math]::Round($apkSizeMB,2)) MB. Limit aşıldı!"
    }
    jarsigner -verify $apkPath
}

# 🧹 Log temizliği
if (Test-Path ".\pre-commit.log") {
    $cleanLog = Get-Content ".\pre-commit.log" | Where-Object { $_ -notmatch "warning|deprecated" }
    $cleanLog | Set-Content ".\pre-commit-clean.log"
    Write-Host "📄 Log temizlendi. pre-commit-clean.log dosyasına yazıldı."
}

# 🧬 Git işlemleri ve CI/CD tetikleme
git add .
git commit -m "v$newVersion: Yayın öncesi tüm işlemler tamamlandı"
git tag "v$newVersion"
git push origin main --tags

# 🎉 Kutlama mesajı
Write-Host "✅ v$newVersion sürümü başarıyla oluşturuldu, testler geçti, CI/CD tetiklendi. Play Store yayınına hazırsın!"
