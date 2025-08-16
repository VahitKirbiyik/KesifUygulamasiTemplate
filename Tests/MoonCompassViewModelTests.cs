using System;
using System.Threading.Tasks;
using KesifUygulamasiTemplate.Services;
using KesifUygulamasiTemplate.ViewModels;
using KesifUygulamasiTemplate.Models;
using Moq;
using Xunit;

namespace KesifUygulamasiTemplate.Tests
{
    public class MoonCompassViewModelTests
    {
        private readonly Mock<IMoonCompassService> _moonServiceMock;
        private readonly Mock<LocationService> _locationServiceMock;
        private readonly MoonCompassViewModel _viewModel;
        
        public MoonCompassViewModelTests()
        {
            // Arrange - Setup mocks
            _moonServiceMock = new Mock<IMoonCompassService>();
            _locationServiceMock = new Mock<LocationService>();
            
            // Mock moon data
            var mockMoonData = new MoonData
            {
                Phase = 0.5,
                RiseTime = DateTime.Today.AddHours(18),
                SetTime = DateTime.Today.AddHours(6)
            };
            
            // Setup mock methods
            _moonServiceMock.Setup(x => x.GetMoonDataAsync(It.IsAny<double>(), It.IsAny<double>()))
                .ReturnsAsync(mockMoonData);
            
            // Create ViewModel with mocks
            _viewModel = new MoonCompassViewModel(_moonServiceMock.Object, _locationServiceMock.Object);
        }
        
        [Fact]
        public async Task LoadMoonDataAsync_ShouldUpdateMoonData()
        {
            // Act
            await _viewModel.LoadMoonDataAsync(40.7128, -74.0060);
            
            // Assert
            Assert.NotNull(_viewModel.MoonData);
            Assert.Equal(0.5, _viewModel.MoonData.Phase);
            Assert.Equal(DateTime.Today.AddHours(18).Hour, _viewModel.MoonData.RiseTime.Hour);
            Assert.Equal(DateTime.Today.AddHours(6).Hour, _viewModel.MoonData.SetTime.Hour);
            
            // Verify method was called with correct parameters
            _moonServiceMock.Verify(x => x.GetMoonDataAsync(40.7128, -74.0060), Times.Once);
        }
        
        [Fact]
        public async Task LoadMoonDataAsync_ShouldHandleExceptions()
        {
            // Arrange - Setup mock to throw exception
            _moonServiceMock.Setup(x => x.GetMoonDataAsync(It.IsAny<double>(), It.IsAny<double>()))
                .ThrowsAsync(new Exception("Test exception"));
                
            // Act
            await _viewModel.LoadMoonDataAsync(40.7128, -74.0060);
            
            // Assert
            Assert.False(string.IsNullOrEmpty(_viewModel.ErrorMessage));
            Assert.Contains("Test exception", _viewModel.ErrorMessage);
        }
    }
}