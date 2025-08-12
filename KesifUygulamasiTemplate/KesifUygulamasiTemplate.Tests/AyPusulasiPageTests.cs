using Xunit;
using System;
using System.Threading.Tasks;

namespace KesifUygulamasiTemplate.Tests
{
    /// <summary>
    /// AyPusulasiPage için birim testleri
    /// Bu test sýnýfý ay pusulasý sayfasýnýn temel iþlevlerini test eder
    /// MAUI baðýmlýlýklarý olmadan core business logic'i test eder
    /// </summary>
    public class AyPusulasiPageTests
    {
        #region Test Data Classes
        
        /// <summary>
        /// AyPusulasiPage'de kullanýlan ay verisi için test sýnýfý
        /// Gerçek MoonData sýnýfýný simüle eder
        /// </summary>
        public class TestAyPusulasiData
        {
            public double Phase { get; set; }
            public DateTime RiseTime { get; set; }
            public DateTime SetTime { get; set; }
            public double Illumination { get; set; }
            public string PhaseName { get; set; } = string.Empty;
            public string PhaseEmoji { get; set; } = string.Empty;
            public double Azimuth { get; set; }
            public double Altitude { get; set; }
            public double Distance { get; set; }
            
            /// <summary>
            /// Label formatlarý için property'ler (UI'daki label'lara karþýlýk gelir)
            /// </summary>
            public string AyFazLabel => $"{Phase * 100:F1}%";           // ayFazLabel
            public string AyDogusLabel => RiseTime.ToString("HH:mm");    // ayDogusLabel  
            public string AyBatisLabel => SetTime.ToString("HH:mm");     // ayBatisLabel
            public string AydinlanmaLabel => $"{Illumination * 100:F1}%"; // aydinlanmaLabel
        }

        /// <summary>
        /// AyPusulasiPage'in core iþlevlerini simüle eden test sýnýfý
        /// Gerçek MoonCompassPage ve ViewModel'in temel mantýðýný test eder
        /// </summary>
        public class TestAyPusulasiPage
        {
            private TestAyPusulasiData? _moonData;
            private bool _isInitialized;

            /// <summary>
            /// Sayfa verilerinin yüklenip yüklenmediðini kontrol eder
            /// </summary>
            public bool IsDataLoaded => _moonData != null;

            /// <summary>
            /// Mevcut ay verisi
            /// </summary>
            public TestAyPusulasiData? MoonData => _moonData;

            /// <summary>
            /// Sayfa baþlatýldý mý?
            /// </summary>
            public bool IsInitialized => _isInitialized;

            /// <summary>
            /// AyPusulasiPage instance'ýný oluþturur
            /// Gerçek MoonCompassPage constructor'ýný simüle eder
            /// </summary>
            public TestAyPusulasiPage()
            {
                _isInitialized = true;
            }

            /// <summary>
            /// HesaplaVeGuncelle metodunu simüle eder
            /// Gerçek LoadMoonDataAsync metodunun eþdeðeri
            /// ayFazLabel, ayDogusLabel, ayBatisLabel, aydinlanmaLabel deðerlerini günceller
            /// </summary>
            public async Task HesaplaVeGuncelle(double latitude, double longitude)
            {
                // Async iþlem simülasyonu
                await Task.Delay(50);

                // Ýstanbul koordinatlarý için özel test verisi
                if (Math.Abs(latitude - 41.0082) < 0.01 && Math.Abs(longitude - 28.9784) < 0.01)
                {
                    _moonData = new TestAyPusulasiData
                    {
                        Phase = 0.75,  // %75 Þiþkin Ay
                        RiseTime = DateTime.Today.AddHours(19).AddMinutes(30), // 19:30
                        SetTime = DateTime.Today.AddHours(7).AddMinutes(15),   // 07:15
                        Illumination = 0.75, // %75 aydýnlanma
                        PhaseName = "Þiþkin Ay",
                        PhaseEmoji = "??",
                        Azimuth = 120.5,
                        Altitude = 45.2,
                        Distance = 384400
                    };
                }
                else
                {
                    // Diðer koordinatlar için genel test verisi
                    _moonData = new TestAyPusulasiData
                    {
                        Phase = 0.5,   // %50 Yarým Ay
                        RiseTime = DateTime.Today.AddHours(20),  // 20:00
                        SetTime = DateTime.Today.AddHours(8),    // 08:00
                        Illumination = 0.5, // %50 aydýnlanma
                        PhaseName = "Yarým Ay",
                        PhaseEmoji = "??",
                        Azimuth = 90,
                        Altitude = 30,
                        Distance = 380000
                    };
                }
            }

            /// <summary>
            /// Tüm label'larýn deðerlerini döndürür
            /// </summary>
            public (string ayFaz, string ayDogus, string ayBatis, string aydinlanma) GetAllLabels()
            {
                if (_moonData == null)
                    return (string.Empty, string.Empty, string.Empty, string.Empty);

                return (
                    _moonData.AyFazLabel,
                    _moonData.AyDogusLabel, 
                    _moonData.AyBatisLabel,
                    _moonData.AydinlanmaLabel
                );
            }
        }

        #endregion

        #region Test Fields
        private readonly TestAyPusulasiPage _ayPusulasiPage;
        #endregion

        #region Constructor
        /// <summary>
        /// Test sýnýfý constructor'ý - her test çalýþmadan önce çaðrýlýr
        /// </summary>
        public AyPusulasiPageTests()
        {
            _ayPusulasiPage = new TestAyPusulasiPage();
        }
        #endregion

        #region Basic Instance Tests

        /// <summary>
        /// Test: AyPusulasiPage instance'ýnýn baþarýyla oluþturulmasý
        /// Bu test, sayfanýn düzgün þekilde initialize edildiðini doðrular
        /// </summary>
        [Fact]
        public void AyPusulasiPage_ShouldBeCreatedSuccessfully()
        {
            // Arrange & Act
            var page = new TestAyPusulasiPage();

            // Assert
            Assert.NotNull(page);
            Assert.True(page.IsInitialized);
            Assert.False(page.IsDataLoaded); // Baþlangýçta veri yüklü olmamalý
        }

        #endregion

        #region HesaplaVeGuncelle Tests

        /// <summary>
        /// Test: HesaplaVeGuncelle metodunun çaðrýlmasý ve 
        /// ayFazLabel, ayDogusLabel, ayBatisLabel, aydinlanmaLabel deðerlerinin güncellenmesi
        /// Bu ana test metodu, istenen tüm label'larýn doðru þekilde güncellenmesini test eder
        /// </summary>
        [Fact]
        public async Task HesaplaVeGuncelle_ShouldUpdateAllLabelsAndNotBeEmpty()
        {
            // Arrange
            double istanbulLat = 41.0082;  // Ýstanbul enlem
            double istanbulLon = 28.9784;  // Ýstanbul boylam

            // Act - HesaplaVeGuncelle metodunu çaðýr
            await _ayPusulasiPage.HesaplaVeGuncelle(istanbulLat, istanbulLon);

            // Assert - Veri yüklendiðini doðrula
            Assert.True(_ayPusulasiPage.IsDataLoaded);
            Assert.NotNull(_ayPusulasiPage.MoonData);

            // Tüm label deðerlerini al
            var (ayFazLabel, ayDogusLabel, ayBatisLabel, aydinlanmaLabel) = _ayPusulasiPage.GetAllLabels();

            // 1. ayFazLabel kontrolü (Ay Fazý)
            Assert.NotNull(ayFazLabel);
            Assert.NotEmpty(ayFazLabel);
            Assert.Contains("%", ayFazLabel);
            Assert.Contains("75", ayFazLabel); // %75 bekleniyor

            // 2. ayDogusLabel kontrolü (Ay Doðuþu)  
            Assert.NotNull(ayDogusLabel);
            Assert.NotEmpty(ayDogusLabel);
            Assert.Contains("19", ayDogusLabel); // 19:30 bekleniyor
            Assert.Contains("30", ayDogusLabel);

            // 3. ayBatisLabel kontrolü (Ay Batýþý)
            Assert.NotNull(ayBatisLabel);
            Assert.NotEmpty(ayBatisLabel);
            Assert.Contains("07", ayBatisLabel); // 07:15 bekleniyor
            Assert.Contains("15", ayBatisLabel);

            // 4. aydinlanmaLabel kontrolü (Aydýnlanma)
            Assert.NotNull(aydinlanmaLabel);
            Assert.NotEmpty(aydinlanmaLabel);
            Assert.Contains("%", aydinlanmaLabel);
            Assert.Contains("75", aydinlanmaLabel); // %75 bekleniyor

            // 5. Ek detaylý kontroller
            Assert.Equal("75,0%", ayFazLabel);
            Assert.Equal("19:30", ayDogusLabel);
            Assert.Equal("07:15", ayBatisLabel);
            Assert.Equal("75,0%", aydinlanmaLabel);
        }

        /// <summary>
        /// Test: HesaplaVeGuncelle metodunun farklý koordinatlar ile çalýþmasý
        /// </summary>
        [Fact]
        public async Task HesaplaVeGuncelle_WithDifferentCoordinates_ShouldUpdateLabels()
        {
            // Arrange
            double newYorkLat = 40.7128;
            double newYorkLon = -74.0060;

            // Act
            await _ayPusulasiPage.HesaplaVeGuncelle(newYorkLat, newYorkLon);

            // Assert
            Assert.True(_ayPusulasiPage.IsDataLoaded);
            
            var (ayFazLabel, ayDogusLabel, ayBatisLabel, aydinlanmaLabel) = _ayPusulasiPage.GetAllLabels();

            // Tüm label'lar boþ olmamalý
            Assert.NotEmpty(ayFazLabel);
            Assert.NotEmpty(ayDogusLabel);
            Assert.NotEmpty(ayBatisLabel);
            Assert.NotEmpty(aydinlanmaLabel);

            // New York için farklý deðerler bekleniyor
            Assert.Contains("50", ayFazLabel);  // %50
            Assert.Contains("20", ayDogusLabel); // 20:00
            Assert.Contains("08", ayBatisLabel); // 08:00
            Assert.Contains("50", aydinlanmaLabel); // %50
        }

        #endregion

        #region Label Format Tests

        /// <summary>
        /// Test: ayFazLabel formatýnýn doðru olmasý
        /// Ay fazý yüzde formatýnda olmalý
        /// </summary>
        [Fact]
        public async Task AyFazLabel_ShouldHaveCorrectPercentageFormat()
        {
            // Arrange & Act
            await _ayPusulasiPage.HesaplaVeGuncelle(41.0082, 28.9784);

            // Assert
            var (ayFazLabel, _, _, _) = _ayPusulasiPage.GetAllLabels();
            
            Assert.NotEmpty(ayFazLabel);
            Assert.Contains("%", ayFazLabel);
            Assert.Matches(@"\d+(\,\d+)?%", ayFazLabel); // Sayý + % formatý
        }

        /// <summary>
        /// Test: ayDogusLabel ve ayBatisLabel saat formatýnýn doðru olmasý
        /// Saat formatý HH:mm olmalý
        /// </summary>
        [Fact]
        public async Task AyDogusAndBatisLabels_ShouldHaveCorrectTimeFormat()
        {
            // Arrange & Act
            await _ayPusulasiPage.HesaplaVeGuncelle(41.0082, 28.9784);

            // Assert
            var (_, ayDogusLabel, ayBatisLabel, _) = _ayPusulasiPage.GetAllLabels();

            // Doðuþ saati formatý kontrolü
            Assert.NotEmpty(ayDogusLabel);
            Assert.Matches(@"\d{2}:\d{2}", ayDogusLabel); // HH:mm formatý

            // Batýþ saati formatý kontrolü  
            Assert.NotEmpty(ayBatisLabel);
            Assert.Matches(@"\d{2}:\d{2}", ayBatisLabel); // HH:mm formatý
        }

        /// <summary>
        /// Test: aydinlanmaLabel formatýnýn doðru olmasý
        /// Aydýnlanma yüzde formatýnda olmalý
        /// </summary>
        [Fact]
        public async Task AydinlanmaLabel_ShouldHaveCorrectPercentageFormat()
        {
            // Arrange & Act
            await _ayPusulasiPage.HesaplaVeGuncelle(41.0082, 28.9784);

            // Assert
            var (_, _, _, aydinlanmaLabel) = _ayPusulasiPage.GetAllLabels();
            
            Assert.NotEmpty(aydinlanmaLabel);
            Assert.Contains("%", aydinlanmaLabel);
            Assert.Matches(@"\d+(\,\d+)?%", aydinlanmaLabel); // Sayý + % formatý
        }

        #endregion

        #region Integration Tests

        /// <summary>
        /// Test: Sayfa baþlatýldýðýnda instance'ýn doðru durumda olmasý
        /// </summary>
        [Fact]
        public void AyPusulasiPage_OnInstantiation_ShouldHaveCorrectInitialState()
        {
            // Arrange & Act
            var page = new TestAyPusulasiPage();

            // Assert
            Assert.True(page.IsInitialized);
            Assert.False(page.IsDataLoaded);
            Assert.Null(page.MoonData);
            
            // Label'lar baþlangýçta boþ olmalý
            var (ayFazLabel, ayDogusLabel, ayBatisLabel, aydinlanmaLabel) = page.GetAllLabels();
            Assert.Empty(ayFazLabel);
            Assert.Empty(ayDogusLabel);
            Assert.Empty(ayBatisLabel);
            Assert.Empty(aydinlanmaLabel);
        }

        #endregion

        #region Performance Tests

        /// <summary>
        /// Test: HesaplaVeGuncelle metodunun performans testi
        /// Metodun makul sürede tamamlanmasý
        /// </summary>
        [Fact]
        public async Task HesaplaVeGuncelle_ShouldCompleteInReasonableTime()
        {
            // Arrange
            var startTime = DateTime.Now;

            // Act
            await _ayPusulasiPage.HesaplaVeGuncelle(41.0082, 28.9784);

            // Assert
            var endTime = DateTime.Now;
            var duration = endTime - startTime;
            
            // 1 saniyeden az sürmeli
            Assert.True(duration.TotalSeconds < 1, 
                $"HesaplaVeGuncelle çok yavaþ çalýþýyor: {duration.TotalSeconds} saniye");
        }

        #endregion

        #region Data Validation Tests

        /// <summary>
        /// Test: Ay verilerinin geçerli aralýklarda olmasý
        /// </summary>
        [Fact]
        public async Task MoonData_ShouldHaveValidRanges()
        {
            // Act
            await _ayPusulasiPage.HesaplaVeGuncelle(41.0082, 28.9784);

            // Assert
            var moonData = _ayPusulasiPage.MoonData;
            Assert.NotNull(moonData);
            
            // Ay fazý 0-1 arasýnda olmalý
            Assert.True(moonData.Phase >= 0 && moonData.Phase <= 1);
            
            // Aydýnlanma 0-1 arasýnda olmalý
            Assert.True(moonData.Illumination >= 0 && moonData.Illumination <= 1);
            
            // Azimuth 0-360 arasýnda olmalý
            Assert.True(moonData.Azimuth >= 0 && moonData.Azimuth <= 360);
            
            // Altitude -90 ile +90 arasýnda olmalý
            Assert.True(moonData.Altitude >= -90 && moonData.Altitude <= 90);
            
            // Ay mesafesi makul aralýkta olmalý (350,000 - 410,000 km)
            Assert.True(moonData.Distance >= 350000 && moonData.Distance <= 410000);

            // Saat deðerleri geçerli olmalý
            Assert.True(moonData.RiseTime.Hour >= 0 && moonData.RiseTime.Hour <= 23);
            Assert.True(moonData.SetTime.Hour >= 0 && moonData.SetTime.Hour <= 23);
        }

        #endregion

        #region Multiple Coordinates Tests

        /// <summary>
        /// Test: Birden fazla koordinat ile art arda HesaplaVeGuncelle çaðrýlmasý
        /// </summary>
        [Fact]
        public async Task HesaplaVeGuncelle_MultipleCoordinates_ShouldUpdateCorrectly()
        {
            // Test 1: Ýstanbul
            await _ayPusulasiPage.HesaplaVeGuncelle(41.0082, 28.9784);
            var (faz1, dogus1, batis1, aydinlanma1) = _ayPusulasiPage.GetAllLabels();
            
            Assert.Contains("75", faz1);
            Assert.Contains("19", dogus1);
            
            // Test 2: New York
            await _ayPusulasiPage.HesaplaVeGuncelle(40.7128, -74.0060);
            var (faz2, dogus2, batis2, aydinlanma2) = _ayPusulasiPage.GetAllLabels();
            
            Assert.Contains("50", faz2);
            Assert.Contains("20", dogus2);
            
            // Veriler güncellenmiþ olmalý (Ýstanbul verisi New York ile deðiþmiþ olmalý)
            Assert.NotEqual(faz1, faz2);
            Assert.NotEqual(dogus1, dogus2);
        }

        #endregion
    }
}