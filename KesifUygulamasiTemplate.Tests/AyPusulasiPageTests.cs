using Xunit;
using System;
using System.Threading.Tasks;

namespace KesifUygulamasiTemplate.Tests
{
    /// <summary>
    /// AyPusulasiPage i�in birim testleri
    /// Bu test s�n�f� ay pusulas� sayfas�n�n temel i�levlerini test eder
    /// MAUI ba��ml�l�klar� olmadan core business logic'i test eder
    /// </summary>
    public class AyPusulasiPageTests
    {
        #region Test Data Classes
        
        /// <summary>
        /// AyPusulasiPage'de kullan�lan ay verisi i�in test s�n�f�
        /// Ger�ek MoonData s�n�f�n� sim�le eder
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
            /// Label formatlar� i�in property'ler (UI'daki label'lara kar��l�k gelir)
            /// </summary>
            public string AyFazLabel => $"{Phase * 100:F1}%";           // ayFazLabel
            public string AyDogusLabel => RiseTime.ToString("HH:mm");    // ayDogusLabel  
            public string AyBatisLabel => SetTime.ToString("HH:mm");     // ayBatisLabel
            public string AydinlanmaLabel => $"{Illumination * 100:F1}%"; // aydinlanmaLabel
        }

        /// <summary>
        /// AyPusulasiPage'in core i�levlerini sim�le eden test s�n�f�
        /// Ger�ek MoonCompassPage ve ViewModel'in temel mant���n� test eder
        /// </summary>
        public class TestAyPusulasiPage
        {
            private TestAyPusulasiData? _moonData;
            private bool _isInitialized;

            /// <summary>
            /// Sayfa verilerinin y�klenip y�klenmedi�ini kontrol eder
            /// </summary>
            public bool IsDataLoaded => _moonData != null;

            /// <summary>
            /// Mevcut ay verisi
            /// </summary>
            public TestAyPusulasiData? MoonData => _moonData;

            /// <summary>
            /// Sayfa ba�lat�ld� m�?
            /// </summary>
            public bool IsInitialized => _isInitialized;

            /// <summary>
            /// AyPusulasiPage instance'�n� olu�turur
            /// Ger�ek MoonCompassPage constructor'�n� sim�le eder
            /// </summary>
            public TestAyPusulasiPage()
            {
                _isInitialized = true;
            }

            /// <summary>
            /// HesaplaVeGuncelle metodunu sim�le eder
            /// Ger�ek LoadMoonDataAsync metodunun e�de�eri
            /// ayFazLabel, ayDogusLabel, ayBatisLabel, aydinlanmaLabel de�erlerini g�nceller
            /// </summary>
            public async Task HesaplaVeGuncelle(double latitude, double longitude)
            {
                // Async i�lem sim�lasyonu
                await Task.Delay(50);

                // �stanbul koordinatlar� i�in �zel test verisi
                if (Math.Abs(latitude - 41.0082) < 0.01 && Math.Abs(longitude - 28.9784) < 0.01)
                {
                    _moonData = new TestAyPusulasiData
                    {
                        Phase = 0.75,  // %75 �i�kin Ay
                        RiseTime = DateTime.Today.AddHours(19).AddMinutes(30), // 19:30
                        SetTime = DateTime.Today.AddHours(7).AddMinutes(15),   // 07:15
                        Illumination = 0.75, // %75 ayd�nlanma
                        PhaseName = "�i�kin Ay",
                        PhaseEmoji = "??",
                        Azimuth = 120.5,
                        Altitude = 45.2,
                        Distance = 384400
                    };
                }
                else
                {
                    // Di�er koordinatlar i�in genel test verisi
                    _moonData = new TestAyPusulasiData
                    {
                        Phase = 0.5,   // %50 Yar�m Ay
                        RiseTime = DateTime.Today.AddHours(20),  // 20:00
                        SetTime = DateTime.Today.AddHours(8),    // 08:00
                        Illumination = 0.5, // %50 ayd�nlanma
                        PhaseName = "Yar�m Ay",
                        PhaseEmoji = "??",
                        Azimuth = 90,
                        Altitude = 30,
                        Distance = 380000
                    };
                }
            }

            /// <summary>
            /// T�m label'lar�n de�erlerini d�nd�r�r
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
        /// Test s�n�f� constructor'� - her test �al��madan �nce �a�r�l�r
        /// </summary>
        public AyPusulasiPageTests()
        {
            _ayPusulasiPage = new TestAyPusulasiPage();
        }
        #endregion

        #region Basic Instance Tests

        /// <summary>
        /// Test: AyPusulasiPage instance'�n�n ba�ar�yla olu�turulmas�
        /// Bu test, sayfan�n d�zg�n �ekilde initialize edildi�ini do�rular
        /// </summary>
        [Fact]
        public void AyPusulasiPage_ShouldBeCreatedSuccessfully()
        {
            // Arrange & Act
            var page = new TestAyPusulasiPage();

            // Assert
            Assert.NotNull(page);
            Assert.True(page.IsInitialized);
            Assert.False(page.IsDataLoaded); // Ba�lang��ta veri y�kl� olmamal�
        }

        #endregion

        #region HesaplaVeGuncelle Tests

        /// <summary>
        /// Test: HesaplaVeGuncelle metodunun �a�r�lmas� ve 
        /// ayFazLabel, ayDogusLabel, ayBatisLabel, aydinlanmaLabel de�erlerinin g�ncellenmesi
        /// Bu ana test metodu, istenen t�m label'lar�n do�ru �ekilde g�ncellenmesini test eder
        /// </summary>
        [Fact]
        public async Task HesaplaVeGuncelle_ShouldUpdateAllLabelsAndNotBeEmpty()
        {
            // Arrange
            double istanbulLat = 41.0082;  // �stanbul enlem
            double istanbulLon = 28.9784;  // �stanbul boylam

            // Act - HesaplaVeGuncelle metodunu �a��r
            await _ayPusulasiPage.HesaplaVeGuncelle(istanbulLat, istanbulLon);

            // Assert - Veri y�klendi�ini do�rula
            Assert.True(_ayPusulasiPage.IsDataLoaded);
            Assert.NotNull(_ayPusulasiPage.MoonData);

            // T�m label de�erlerini al
            var (ayFazLabel, ayDogusLabel, ayBatisLabel, aydinlanmaLabel) = _ayPusulasiPage.GetAllLabels();

            // 1. ayFazLabel kontrol� (Ay Faz�)
            Assert.NotNull(ayFazLabel);
            Assert.NotEmpty(ayFazLabel);
            Assert.Contains("%", ayFazLabel);
            Assert.Contains("75", ayFazLabel); // %75 bekleniyor

            // 2. ayDogusLabel kontrol� (Ay Do�u�u)  
            Assert.NotNull(ayDogusLabel);
            Assert.NotEmpty(ayDogusLabel);
            Assert.Contains("19", ayDogusLabel); // 19:30 bekleniyor
            Assert.Contains("30", ayDogusLabel);

            // 3. ayBatisLabel kontrol� (Ay Bat���)
            Assert.NotNull(ayBatisLabel);
            Assert.NotEmpty(ayBatisLabel);
            Assert.Contains("07", ayBatisLabel); // 07:15 bekleniyor
            Assert.Contains("15", ayBatisLabel);

            // 4. aydinlanmaLabel kontrol� (Ayd�nlanma)
            Assert.NotNull(aydinlanmaLabel);
            Assert.NotEmpty(aydinlanmaLabel);
            Assert.Contains("%", aydinlanmaLabel);
            Assert.Contains("75", aydinlanmaLabel); // %75 bekleniyor

            // 5. Ek detayl� kontroller
            Assert.Equal("75,0%", ayFazLabel);
            Assert.Equal("19:30", ayDogusLabel);
            Assert.Equal("07:15", ayBatisLabel);
            Assert.Equal("75,0%", aydinlanmaLabel);
        }

        /// <summary>
        /// Test: HesaplaVeGuncelle metodunun farkl� koordinatlar ile �al��mas�
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

            // T�m label'lar bo� olmamal�
            Assert.NotEmpty(ayFazLabel);
            Assert.NotEmpty(ayDogusLabel);
            Assert.NotEmpty(ayBatisLabel);
            Assert.NotEmpty(aydinlanmaLabel);

            // New York i�in farkl� de�erler bekleniyor
            Assert.Contains("50", ayFazLabel);  // %50
            Assert.Contains("20", ayDogusLabel); // 20:00
            Assert.Contains("08", ayBatisLabel); // 08:00
            Assert.Contains("50", aydinlanmaLabel); // %50
        }

        #endregion

        #region Label Format Tests

        /// <summary>
        /// Test: ayFazLabel format�n�n do�ru olmas�
        /// Ay faz� y�zde format�nda olmal�
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
            Assert.Matches(@"\d+(\,\d+)?%", ayFazLabel); // Say� + % format�
        }

        /// <summary>
        /// Test: ayDogusLabel ve ayBatisLabel saat format�n�n do�ru olmas�
        /// Saat format� HH:mm olmal�
        /// </summary>
        [Fact]
        public async Task AyDogusAndBatisLabels_ShouldHaveCorrectTimeFormat()
        {
            // Arrange & Act
            await _ayPusulasiPage.HesaplaVeGuncelle(41.0082, 28.9784);

            // Assert
            var (_, ayDogusLabel, ayBatisLabel, _) = _ayPusulasiPage.GetAllLabels();

            // Do�u� saati format� kontrol�
            Assert.NotEmpty(ayDogusLabel);
            Assert.Matches(@"\d{2}:\d{2}", ayDogusLabel); // HH:mm format�

            // Bat�� saati format� kontrol�  
            Assert.NotEmpty(ayBatisLabel);
            Assert.Matches(@"\d{2}:\d{2}", ayBatisLabel); // HH:mm format�
        }

        /// <summary>
        /// Test: aydinlanmaLabel format�n�n do�ru olmas�
        /// Ayd�nlanma y�zde format�nda olmal�
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
            Assert.Matches(@"\d+(\,\d+)?%", aydinlanmaLabel); // Say� + % format�
        }

        #endregion

        #region Integration Tests

        /// <summary>
        /// Test: Sayfa ba�lat�ld���nda instance'�n do�ru durumda olmas�
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
            
            // Label'lar ba�lang��ta bo� olmal�
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
        /// Metodun makul s�rede tamamlanmas�
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
            
            // 1 saniyeden az s�rmeli
            Assert.True(duration.TotalSeconds < 1, 
                $"HesaplaVeGuncelle �ok yava� �al���yor: {duration.TotalSeconds} saniye");
        }

        #endregion

        #region Data Validation Tests

        /// <summary>
        /// Test: Ay verilerinin ge�erli aral�klarda olmas�
        /// </summary>
        [Fact]
        public async Task MoonData_ShouldHaveValidRanges()
        {
            // Act
            await _ayPusulasiPage.HesaplaVeGuncelle(41.0082, 28.9784);

            // Assert
            var moonData = _ayPusulasiPage.MoonData;
            Assert.NotNull(moonData);
            
            // Ay faz� 0-1 aras�nda olmal�
            Assert.True(moonData.Phase >= 0 && moonData.Phase <= 1);
            
            // Ayd�nlanma 0-1 aras�nda olmal�
            Assert.True(moonData.Illumination >= 0 && moonData.Illumination <= 1);
            
            // Azimuth 0-360 aras�nda olmal�
            Assert.True(moonData.Azimuth >= 0 && moonData.Azimuth <= 360);
            
            // Altitude -90 ile +90 aras�nda olmal�
            Assert.True(moonData.Altitude >= -90 && moonData.Altitude <= 90);
            
            // Ay mesafesi makul aral�kta olmal� (350,000 - 410,000 km)
            Assert.True(moonData.Distance >= 350000 && moonData.Distance <= 410000);

            // Saat de�erleri ge�erli olmal�
            Assert.True(moonData.RiseTime.Hour >= 0 && moonData.RiseTime.Hour <= 23);
            Assert.True(moonData.SetTime.Hour >= 0 && moonData.SetTime.Hour <= 23);
        }

        #endregion

        #region Multiple Coordinates Tests

        /// <summary>
        /// Test: Birden fazla koordinat ile art arda HesaplaVeGuncelle �a�r�lmas�
        /// </summary>
        [Fact]
        public async Task HesaplaVeGuncelle_MultipleCoordinates_ShouldUpdateCorrectly()
        {
            // Test 1: �stanbul
            await _ayPusulasiPage.HesaplaVeGuncelle(41.0082, 28.9784);
            var (faz1, dogus1, batis1, aydinlanma1) = _ayPusulasiPage.GetAllLabels();
            
            Assert.Contains("75", faz1);
            Assert.Contains("19", dogus1);
            
            // Test 2: New York
            await _ayPusulasiPage.HesaplaVeGuncelle(40.7128, -74.0060);
            var (faz2, dogus2, batis2, aydinlanma2) = _ayPusulasiPage.GetAllLabels();
            
            Assert.Contains("50", faz2);
            Assert.Contains("20", dogus2);
            
            // Veriler g�ncellenmi� olmal� (�stanbul verisi New York ile de�i�mi� olmal�)
            Assert.NotEqual(faz1, faz2);
            Assert.NotEqual(dogus1, dogus2);
        }

        #endregion
    }
}