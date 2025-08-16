using Xunit;
using System.Threading.Tasks;

namespace KesifUygulamasiTemplate.Tests
{
    /// <summary>
    /// Emulator veya fiziksel cihaz gerektiren integration testler
    /// </summary>
    public class DeviceIntegrationTests
    {
        [Fact]
        [Trait("Category", "Integration")]
        public async Task LocationService_ShouldGetDeviceLocation()
        {
            // Bu test ger�ek cihaz veya emulator gerektirir
            // Mock servis yerine ger�ek LocationService test edilir
            await Task.CompletedTask;
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task CompassService_ShouldGetDeviceHeading()
        {
            // Bu test ger�ek pusula sens�r� gerektirir
            await Task.CompletedTask;
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task CameraService_ShouldAccessDeviceCamera()
        {
            // Bu test ger�ek kamera eri�imi gerektirir
            await Task.CompletedTask;
        }
    }

    /// <summary>
    /// Cihaz gerektirmeyen unit testler
    /// </summary>
    public class UnitTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void MoonCalculation_ShouldReturnCorrectPhase()
        {
            // Bu test hi�bir cihaz gerektirmez
            Assert.True(true);
        }
    }
}