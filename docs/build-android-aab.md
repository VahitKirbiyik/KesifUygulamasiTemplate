# Android AAB Production Build Documentation

## Genel Bakýþ

Bu script, .NET MAUI projesi için production-ready Android App Bundle (AAB) dosyalarý oluþturur. Google Play Store'a yüklenmeye hazýr, imzalý AAB dosyalarý üretir.

## Özellikler

### ? **Tam Ýmzalama Desteði**
- Keystore dosyasý doðrulama
- Otomatik key alias tespiti
- Keystore bütünlük kontrolü
- Çoklu imzalama algoritmasý desteði

### ? **Otomatik Versiyonlama**
- Git tag'lerinden versiyon çýkarma
- GitHub Actions entegrasyonu
- Commit count tabanlý versiyon kodlarý
- Fallback versiyonlama stratejisi

### ? **Build Optimizasyonlarý**
- R8 code shrinking
- ProGuard obfuscation
- LLVM optimizations
- MultiDex support
- AOT compilation
- IL stripping

### ? **Geliþmiþ Hata Kontrolü**
- Kapsamlý input validation
- Build environment checks
- AAB integrity verification
- Signature validation
- Comprehensive logging

### ? **CI/CD Entegrasyonu**
- GitHub Actions output variables
- Environment variable support
- Artifact management
- Build reporting
- Log archiving

## Kullaným

### Lokal Development

```bash
# Temel kullaným (unsigned)
./scripts/build-android-aab.sh

# Signed build için environment variables
export ANDROID_KEYSTORE_PATH="/path/to/your/keystore.jks"
export ANDROID_SIGNING_PASSWORD="your_password"
export ANDROID_KEY_ALIAS="your_key_alias"
./scripts/build-android-aab.sh
```

### GitHub Actions

```yaml
- name: Build Android AAB
  env:
    ANDROID_KEYSTORE_PATH: ${{ env.ANDROID_KEYSTORE_PATH }}
    ANDROID_SIGNING_PASSWORD: ${{ secrets.ANDROID_SIGNING_PASSWORD }}
    ANDROID_KEY_ALIAS: ${{ secrets.ANDROID_KEY_ALIAS }}
  run: |
    chmod +x ./scripts/build-android-aab.sh
    ./scripts/build-android-aab.sh
```

## Environment Variables

### Gerekli (Ýmzalý build için)
- `ANDROID_KEYSTORE_PATH`: Keystore dosyasýnýn tam yolu
- `ANDROID_SIGNING_PASSWORD`: Keystore ve key þifresi

### Opsiyonel
- `ANDROID_KEY_ALIAS`: Key alias (varsayýlan: "androiddebugkey")

### GitHub Actions Otomatik
- `GITHUB_REF`: Git tag referansý
- `GITHUB_OUTPUT`: Actions output file
- `GITHUB_ENV`: Actions environment file

## Çýktýlar

### Dosyalar
- `./artifacts/*.aab`: Oluþturulan AAB dosyalarý
- `./build-logs/build-*.log.gz`: Kompresli build loglarý

### GitHub Actions Outputs
- `aab_count`: Oluþturulan AAB sayýsý
- `primary_aab`: Ana AAB dosyasý yolu
- `version_name`: Uygulamanýn versiyon adý
- `version_code`: Uygulamanýn versiyon kodu
- `signed`: Ýmzalama durumu (true/false)
- `total_size`: Toplam artifact boyutu

## Versiyonlama Stratejisi

### Git Tag Versiyonlama
```bash
# v1.2.3 tag'i ? Version Name: 1.2.3, Version Code: commit count
git tag v1.2.3
git push origin v1.2.3
```

### Otomatik Versiyon Kodu
- Commit count kullanýlarak otomatik artan versiyon kodu
- Her commit yeni bir versiyon kodu oluþturur

## Build Optimizasyonlarý

### R8 Code Shrinking
```xml
<AndroidLinkTool>r8</AndroidLinkTool>
<AndroidLinkMode>SdkOnly</AndroidLinkMode>
```

### LLVM Optimizations
```xml
<EnableLLVM>true</EnableLLVM>
<AndroidStripILAfterAOT>true</AndroidStripILAfterAOT>
```

### Publishing Options
```xml
<PublishTrimmed>true</PublishTrimmed>
<AndroidCreatePackagePerAbi>false</AndroidCreatePackagePerAbi>
```

## Hata Giderme

### Build Hatalarý
- Build log dosyasýný kontrol edin: `./build-logs/build-*.log.gz`
- Verbose output için `-verbosity:detailed` ekleyin

### Ýmzalama Hatalarý
- Keystore dosyasý yolunu kontrol edin
- Key alias'ýn doðru olduðundan emin olun
- Þifrelerin eþleþtiðini doðrulayýn

### Versiyon Hatalarý
- Git repository'sinde olduðunuzdan emin olun
- Tag'lerin `v` prefix'i ile baþladýðýndan emin olun

## Güvenlik

### Keystore Güvenliði
- Keystore dosyalarýný asla repository'ye commit etmeyin
- Þifreleri environment variables olarak saklayýn
- GitHub Secrets kullanýn

### Build Güvenliði
- Script her zaman `set -euo pipefail` ile çalýþýr
- Input validation yapýlýr
- Temporary file'lar temizlenir

## Ýleri Seviye Kullaným

### Custom Build Parameters
Script içerisindeki `build_params` array'ini modifiye edebilirsiniz:

```bash
# Ek build parametreleri
build_params+=(
    "-p:AndroidEnableProfiledAot=true"
    "-p:AndroidR8JarPath=/custom/r8.jar"
)
```

### Custom Validation
`verify_aab_integrity` fonksiyonunu geniþletebilirsiniz:

```bash
# Custom AAB validation
verify_custom_aab() {
    local aab_file="$1"
    # Your custom validation logic
}
```

## Versiyon Geçmiþi

### v2.0.0 (Current)
- ? Tam production-ready script
- ? Comprehensive error handling
- ? Advanced logging system
- ? AAB integrity verification
- ? CI/CD integration

### v1.0.0 (Previous)
- ? Basic AAB build functionality
- ? Simple signing support
- ?? Limited error handling
- ?? Basic versioning

## Lisans

Bu script MIT lisansý altýnda daðýtýlmaktadýr.