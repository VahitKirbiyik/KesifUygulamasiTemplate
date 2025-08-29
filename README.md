# KesifUygulamasiTemplate

[![GitHub Pages](https://img.shields.io/badge/GitHub%20Pages-Active-brightgreen)](https://kesifapp.com/)
[![CI](https://github.com/VahitKirbiyik/KesifUygulamasiTemplate/actions/workflows/ci.yml/badge.svg)](https://github.com/VahitKirbiyik/KesifUygulamasiTemplate/actions/workflows/ci.yml)
[![Code Coverage](https://img.shields.io/badge/coverage-85%25-brightgreen)](https://github.com/VahitKirbiyik/KesifUygulamasiTemplate/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://kesifapp.com/legal.html)
[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/)
[![Platforms](https://img.shields.io/badge/Platforms-iOS%20%7C%20Android%20%7C%20Windows-lightgrey.svg)](https://dotnet.microsoft.com/en-us/apps/maui)
[![GitHub stars](https://img.shields.io/github/stars/VahitKirbiyik/KesifUygulamasiTemplate)](https://github.com/VahitKirbiyik/KesifUygulamasiTemplate/stargazers)
[![GitHub issues](https://img.shields.io/github/issues/VahitKirbiyik/KesifUygulamasiTemplate)](https://github.com/VahitKirbiyik/KesifUygulamasiTemplate/issues)
[![GitHub last commit](https://img.shields.io/github/last-commit/VahitKirbiyik/KesifUygulamasiTemplate)](https://github.com/VahitKirbiyik/KesifUygulamasiTemplate/commits/main)

.NET MAUI tabanlı gelişmiş keşif uygulaması template projesi. Modern navigasyon, pusula özellikleri ve kapsamlı harita desteği ile donatılmıştır.

## 🚀 Özellikler

### 🗺️ Navigasyon & Harita
- **Çoklu Rota API Desteği**: Google Maps & Mapbox Directions API
- **Offline Harita**: 500MB önbellek ile çevrimdışı harita desteği
- **Gerçek Zamanlı Trafik**: Trafik durumu ve alternatif rotalar
- **Sesli Yönlendirme**: Text-to-Speech ile navigasyon talimatları
- **POI Yönetimi**: İlgi çekici noktalar ve favori yerler

### 🧭 Pusula Özellikleri
- **Ay Pusulası**: Ayın konumunu ve fazını gerçek zamanlı hesaplama
- **Kıble Pusulası**: İslam'ın kutsal yönü için hassas pusula
- **Pusula Kalibrasyonu**: Otomatik kalibrasyon sistemi
- **Astronomik Hesaplamalar**: Güneş, ay ve yıldız pozisyonları

### 🌐 Çoklu Dil Desteği
- **40+ Dil**: Arapça, Almanca, Fransızca, İspanyolca, Çince, Japonca vb.
- **RTL Desteği**: Arapça ve İbranice için sağdan sola yazım
- **Dinamik Dil Değişimi**: Çalışma zamanında dil değiştirme
- **Kültürel Yerelleştirme**: Tarih, sayı ve para birimi formatları

### 🔔 Akıllı Bildirimler
- **Navigasyon Bildirimleri**: Varış noktası hatırlatmaları
- **Hava Durumu Uyarıları**: Hava durumu değişiklikleri
- **Acil Durum Noktaları**: Yakındaki hastane, polis merkezi bildirimleri
- **Favori Yer Hatırlatmaları**: Kaydedilmiş yerler için hatırlatmalar
- **Ay Pusulası Bildirimleri**: Ay olayları ve faz değişiklikleri

### 💳 Bağış Sistemi
- **PayPal Entegrasyonu**: Güvenli PayPal bağış sistemi
- **Stripe Entegrasyonu**: Kredi kartı ile bağış desteği
- **Çoklu Ödeme Yöntemi**: Alternatif bağış seçenekleri
- **Teşekkür Bildirimleri**: Bağış sonrası kullanıcı geri bildirimi

### 🔧 Teknik Özellikler
- **MVVM Mimarisi**: Temiz ve test edilebilir kod yapısı
- **Dependency Injection**: Service locator pattern
- **Global Exception Handling**: Merkezi hata yönetimi
- **SQLite Veritabanı**: Yerel veri depolama
- **Offline Senkronizasyon**: Çevrimdışı veri senkronizasyonu

## 🛠️ Teknoloji Stack

- **Framework**: .NET 8, .NET MAUI
- **UI**: XAML, CommunityToolkit.Maui
- **Architecture**: MVVM Pattern
- **Database**: SQLite
- **Maps**: Google Maps API, Mapbox API
- **Weather**: OpenWeatherMap API
- **Analytics**: Microsoft App Center
- **Testing**: xUnit, Moq, Coverlet
- **CI/CD**: GitHub Actions
- **Dependency Injection**: Microsoft.Extensions.DependencyInjection

## 📊 Kalite Metrikleri

- ✅ **Test Coverage**: %85+ hedef (RouteService, PushNotificationService, MapDataService testleri mevcut)
- ✅ **Code Quality**: StyleCop, Roslyn analizörleri
- ✅ **Build Status**: Tüm platformlarda başarılı
- ✅ **Exception Handling**: Global hata yakalama
- ✅ **API Configuration**: Yapılandırılabilir API anahtarları
- ✅ **Documentation**: Kapsamlı README ve kod yorumları

## 🏃‍♂️ Kurulum ve Çalıştırma

### Ön Gereksinimler
- .NET 8 SDK
- Visual Studio 2022 (17.4+)
- Android SDK (Android geliştirme için)
- Xcode (iOS geliştirme için)

### Hızlı Başlangıç

```bash
# Projeyi klonlayın
git clone https://github.com/your-username/KesifUygulamasiTemplate.git
cd KesifUygulamasiTemplate

# Bağımlılıkları yükleyin
dotnet restore

# API anahtarlarını yapılandırın
cp appsettings.json appsettings.local.json
# appsettings.local.json dosyasını düzenleyin ve API anahtarlarınızı ekleyin

# Uygulamayı çalıştırın
dotnet build
dotnet run
```

### API Anahtarları Yapılandırması

`appsettings.json` dosyasında aşağıdaki API anahtarlarını yapılandırın:

```json
{
  "ApiKeys": {
    "GoogleMaps": {
      "ApiKey": "YOUR_GOOGLE_MAPS_API_KEY"
    },
    "Mapbox": {
      "ApiKey": "YOUR_MAPBOX_API_KEY"
    },
    "OpenWeatherMap": {
      "ApiKey": "YOUR_OPENWEATHER_API_KEY"
    }
  }
}
```

## 🧪 Test Çalıştırma

```bash
# Tüm unit testleri çalıştırın
dotnet test

# Code coverage ile test çalıştırın
dotnet test --collect:"XPlat Code Coverage"

# Belirli bir test projesi çalıştırın
dotnet test KesifUygulamasiTemplate.Tests/KesifUygulamasiTemplate.Tests.csproj
```

### Test Kapsamı
- **Services**: RouteService, PushNotificationService, MapDataService
- **ViewModels**: SettingsViewModel, MoonCompassViewModel
- **Coverage Hedef**: %85+

## 📱 Platform Özellikleri

### Android
- Google Play Services entegrasyonu
- Android Auto desteği
- Wear OS companion app
- Background location services

### iOS
- Apple Maps entegrasyonu
- CarPlay desteği
- iOS Widget desteği
- Background location services

### Windows
- WinUI 3 desteği
- Windows Maps entegrasyonu
- Live tiles
- Background tasks

## 🔧 Gelişmiş Yapılandırma

### Harita Ayarları
```json
{
  "MapSettings": {
    "DefaultZoomLevel": 15,
    "MaxZoomLevel": 20,
    "MinZoomLevel": 3,
    "CacheSizeMB": 500,
    "CacheExpiryDays": 30
  }
}
```

### Bildirim Ayarları
```json
{
  "NotificationSettings": {
    "NavigationEnabled": true,
    "WeatherAlertsEnabled": true,
    "EmergencyPointsEnabled": true,
    "MoonCompassEnabled": true
  }
}
```

## 📈 CI/CD Pipeline

Proje aşağıdaki otomatik süreçleri içerir:

1. **Build & Test**: Tüm platformlarda paralel build
2. **Code Analysis**: Stil ve kalite kontrolü
3. **Test Coverage**: Kapsam raporu oluşturma (%85 hedef)
4. **Security Scan**: Güvenlik açığı taraması
5. **Artifact Upload**: Build çıktıları ve raporlar
6. **Release Build**: Android .aab ve iOS .ipa oluşturma

## 🛡️ Güvenlik ve Gizlilik

- **API Key Security**: Güvenli anahtar yönetimi
- **Location Privacy**: Konum izni yönetimi
- **Data Encryption**: Hassas veri şifreleme
- **Crash Reporting**: Güvenli crash log'ları
- **Privacy Controls**: Kullanıcı gizlilik ayarları

## 🤝 Katkıda Bulunma

1. Fork edin
2. Feature branch oluşturun (`git checkout -b feature/AmazingFeature`)
3. Commit edin (`git commit -m 'Add some AmazingFeature'`)
4. Push edin (`git push origin feature/AmazingFeature`)
5. Pull Request açın

### Geliştirme Standartları
- MVVM pattern kullanımı
- Unit test yazımı
- Code documentation
- StyleCop kurallarına uyum
- Türkçe dil hassasiyeti

## 📝 Lisans

Bu proje MIT lisansı altında lisanslanmıştır. Detaylar için [LICENSE](LICENSE) dosyasına bakın.

## 🙏 Teşekkürler

Bu projeye katkıda bulunan tüm geliştiricilere teşekkür ederiz. Özellikle:

- .NET MAUI topluluğu
- OpenStreetMap contributors
- Google Maps Platform
- Mapbox
- OpenWeatherMap

---

**⭐ Bu proje faydalı olduysa yıldız vermeyi unutmayın!**
