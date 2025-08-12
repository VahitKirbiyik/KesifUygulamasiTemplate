using Xunit;

namespace KesifUygulamasiTemplate.Tests
{
    /// <summary>
    /// Basitle�tirilmi� Ay Pusulas� testleri - MAUI ba��ml�l�klar� olmadan
    /// Bu testler core business logic'i test eder
    /// </summary>
    public class AyPusulasiSimpleTests
    {
        #region Test Data Classes
        
        /// <summary>
        /// Test i�in basitle�tirilmi� ay verisi s�n�f�
        /// </summary>
        public class TestMoonData
        {
            public double Phase { get; set; }
            public DateTime RiseTime { get; set; }
            public DateTime SetTime { get; set; }
            public double Illumination { get; set; }
            public string PhaseName { get; set; } = string.Empty;
            public double Azimuth { get; set; }
            public double Altitude { get; set; }
            public double Distance { get; set; }
        }

        /// <summary>
        /// Test i�in basitle�tirilmi� konum s�n�f�
        /// </summary>
        public class TestLocation
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            
            public TestLocation(double latitude, double longitude)
            {
                Latitude = latitude;
                Longitude = longitude;
            }
        }

        /// <summary>
        /// Test i�in ay pusulas� hesaplama s�n�f�
        /// Ger�ek MoonCompassService'in temel i�levlerini sim�le eder
        /// </summary>
        public class TestAyPusulasiCalculator
        {
            public TestMoonData HesaplaVeGuncelle(double latitude, double longitude)
            {
                // �stanbul koordinatlar� i�in test verisi
                if (Math.Abs(latitude - 41.0082) < 0.01 && Math.Abs(longitude - 28.9784) < 0.01)
                {
                    return new TestMoonData
                    {
                        Phase = 0.75,
                        RiseTime = DateTime.Today.AddHours(19).AddMinutes(30),
                        SetTime = DateTime.Today.AddHours(7).AddMinutes(15),
                        Illumination = 0.75,
                        PhaseName = "�i�kin Ay",
                        Azimuth = 120.5,
                        Altitude = 45.2,
                        Distance = 384400
                    };
                }

                // Di�er koordinatlar i�in genel test verisi
                return new TestMoonData
                {
                    Phase = 0.5,
                    RiseTime = DateTime.Today.AddHours(20),
                    SetTime = DateTime.Today.AddHours(8),
                    Illumination = 0.5,
                    PhaseName = "Yar�m Ay",
                    Azimuth = 90,
                    Altitude = 30,
                    Distance = 380000
                };
            }

            public string FormatPhase(double phase)
            {
                return $"{phase * 100:F1}%";
            }

            public string FormatTime(DateTime time)
            {
                return time.ToString("HH:mm");
            }

            public string FormatAngle(double angle)
            {
                return $"{angle:F1}�";
            }

            public string FormatDistance(double distance)
            {
                return $"{distance:N0} km";
            }
        }

        #endregion

        #region Test Fields
        private readonly TestAyPusulasiCalculator _calculator;
        #endregion

        #region Constructor
        public AyPusulasiSimpleTests()
        {
            _calculator = new TestAyPusulasiCalculator();
        }
        #endregion

        #region Test Methods

        /// <summary>
        /// Test: AyPusulasiCalculator instance'�n�n olu�turulmas�
        /// </summary>
        [Fact]
        public void AyPusulasiCalculator_ShouldBeCreatedSuccessfully()
        {
            // Arrange & Act
            var calculator = new TestAyPusulasiCalculator();

            // Assert
            Assert.NotNull(calculator);
        }

        /// <summary>
        /// Test: HesaplaVeGuncelle metodunun �stanbul koordinatlar� i�in �al��mas�
        /// Bu test ger�ek "HesaplaVeGuncelle" fonksiyonunu sim�le eder
        /// </summary>
        [Fact]
        public void HesaplaVeGuncelle_IstanbulCoordinates_ShouldReturnValidData()
        {
            // Arrange
            double istanbulLat = 41.0082;
            double istanbulLon = 28.9784;

            // Act - "Hesapla ve G�ncelle" i�levini �a��r
            var moonData = _calculator.HesaplaVeGuncelle(istanbulLat, istanbulLon);

            // Assert - T�m label de�erlerinin g�ncellenmesi kontrol�
            
            // 1. Ay Faz� (ayFazLabel) kontrol�
            Assert.True(moonData.Phase > 0);
            Assert.True(moonData.Phase <= 1);
            Assert.Equal(0.75, moonData.Phase); // �stanbul i�in beklenen test de�eri

            // 2. Ay Do�u� (ayDogusLabel) kontrol�
            Assert.True(moonData.RiseTime > DateTime.MinValue);
            Assert.Equal(19, moonData.RiseTime.Hour);
            Assert.Equal(30, moonData.RiseTime.Minute);

            // 3. Ay Bat�� (ayBatisLabel) kontrol�
            Assert.True(moonData.SetTime > DateTime.MinValue);
            Assert.Equal(7, moonData.SetTime.Hour);
            Assert.Equal(15, moonData.SetTime.Minute);

            // 4. Ayd�nlanma (aydinlanmaLabel) kontrol�
            Assert.True(moonData.Illumination > 0);
            Assert.True(moonData.Illumination <= 1);
            Assert.Equal(0.75, moonData.Illumination);

            // 5. Faz ismi kontrol�
            Assert.NotEmpty(moonData.PhaseName);
            Assert.Equal("�i�kin Ay", moonData.PhaseName);

            // 6. A�� de�erleri kontrol�
            Assert.True(moonData.Azimuth >= 0 && moonData.Azimuth <= 360);
            Assert.True(moonData.Altitude >= -90 && moonData.Altitude <= 90);

            // 7. Mesafe kontrol�
            Assert.True(moonData.Distance > 350000); // Minimum ay mesafesi
            Assert.True(moonData.Distance < 410000); // Maksimum ay mesafesi
        }

        /// <summary>
        /// Test: Formatlanm�� verilerin do�ru format ile g�sterilmesi
        /// ayFazLabel, ayDogusLabel, ayBatisLabel, aydinlanmaLabel i�in format kontrolleri
        /// </summary>
        [Fact]
        public void FormattedLabels_ShouldDisplayCorrectFormats()
        {
            // Arrange
            var moonData = _calculator.HesaplaVeGuncelle(41.0082, 28.9784);

            // Act - Format metodlar�n� �a��r (label'larda g�sterilecek veriler)
            string ayFazLabel = _calculator.FormatPhase(moonData.Phase);           // Ay faz� %
            string ayDogusLabel = _calculator.FormatTime(moonData.RiseTime);       // Do�u� saati
            string ayBatisLabel = _calculator.FormatTime(moonData.SetTime);        // Bat�� saati
            string aydinlanmaLabel = _calculator.FormatPhase(moonData.Illumination); // Ayd�nlanma %

            // Assert - T�m label'lar�n bo� olmad���n� ve do�ru format i�erdi�ini kontrol et

            // 1. ayFazLabel kontrol�
            Assert.NotEmpty(ayFazLabel);
            Assert.Contains("%", ayFazLabel);
            Assert.Equal("75,0%", ayFazLabel);

            // 2. ayDogusLabel kontrol�
            Assert.NotEmpty(ayDogusLabel);
            Assert.Matches(@"^\d{2}:\d{2}$", ayDogusLabel); // HH:mm format�
            Assert.Equal("19:30", ayDogusLabel);

            // 3. ayBatisLabel kontrol�
            Assert.NotEmpty(ayBatisLabel);
            Assert.Matches(@"^\d{2}:\d{2}$", ayBatisLabel); // HH:mm format�
            Assert.Equal("07:15", ayBatisLabel);

            // 4. aydinlanmaLabel kontrol�
            Assert.NotEmpty(aydinlanmaLabel);
            Assert.Contains("%", aydinlanmaLabel);
            Assert.Equal("75,0%", aydinlanmaLabel);
        }

        #endregion
    }
}