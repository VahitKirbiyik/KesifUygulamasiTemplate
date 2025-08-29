using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using KesifUygulamasiTemplate.Models;
using KesifUygulamasiTemplate.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace RouteServiceTestProject
{
    public class RouteServiceTests
    {
        private readonly Mock<HttpClient> _mockHttpClient;
        private readonly Mock<ConnectivityService> _mockConnectivityService;
        private readonly RouteService _routeService;

        public RouteServiceTests()
        {
            _mockHttpClient = new Mock<HttpClient>();
            _mockConnectivityService = new Mock<ConnectivityService>();
            _routeService = new RouteService(_mockHttpClient.Object, _mockConnectivityService.Object);
        }

        [Fact]
        public async Task GetRouteAsync_WithValidLocations_ReturnsRoute()
        {
            // Arrange
            var start = new LocationModel { Latitude = 41.0082, Longitude = 28.9784 }; // İstanbul
            var end = new LocationModel { Latitude = 39.9334, Longitude = 32.8597 }; // Ankara

            _mockConnectivityService.Setup(c => c.IsConnected).Returns(true);

            // Act
            var result = await _routeService.GetRouteAsync(start, end);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Count >= 2);
        }

        [Fact]
        public async Task GetRouteAsync_WhenOffline_ReturnsOfflineRoute()
        {
            // Arrange
            var start = new LocationModel { Latitude = 41.0082, Longitude = 28.9784 };
            var end = new LocationModel { Latitude = 39.9334, Longitude = 32.8597 };

            _mockConnectivityService.Setup(c => c.IsConnected).Returns(false);

            // Act
            var result = await _routeService.GetRouteAsync(start, end);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Count >= 2);
        }

        [Fact]
        public async Task GetRouteAsync_WithSameStartEnd_ReturnsDirectRoute()
        {
            // Arrange
            var location = new LocationModel { Latitude = 41.0082, Longitude = 28.9784 };
            _mockConnectivityService.Setup(c => c.IsConnected).Returns(true);

            // Act
            var result = await _routeService.GetRouteAsync(location, location);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Count >= 1);
        }
    }
}
