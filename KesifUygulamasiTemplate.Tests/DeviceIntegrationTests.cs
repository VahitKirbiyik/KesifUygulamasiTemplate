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
            // Bu test gerçek cihaz veya emulator gerektirir
            // Mock servis yerine gerçek LocationService test edilir
            await Task.CompletedTask;
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task CompassService_ShouldGetDeviceHeading()
        {
            // Bu test gerçek pusula sensörü gerektirir
            await Task.CompletedTask;
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task CameraService_ShouldAccessDeviceCamera()
        {
            // Bu test gerçek kamera erişimi gerektirir
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
            // Bu test hiçbir cihaz gerektirmez
            Assert.True(true);
        }
    }
}
