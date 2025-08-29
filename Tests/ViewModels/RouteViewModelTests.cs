using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Devices.Sensors;
using Moq;
using KesifUygulamasiTemplate.ViewModels;
using KesifUygulamasiTemplate.Services.Interfaces;
using KesifUygulamasiTemplate.Models;

namespace KesifUygulamasi.Tests.ViewModels
{
    /// <summary>
    /// Unit tests for RouteViewModel using xUnit and Moq
    /// </summary>
    public class RouteViewModelTests
    {
        private readonly Mock<IRouteService> _mockRouteService;
        private readonly RouteViewModel _viewModel;

        public RouteViewModelTests()
        {
            _mockRouteService = new Mock<IRouteService>();
            _viewModel = new RouteViewModel(_mockRouteService.Object);
        }

        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            // Arrange & Act
            var viewModel = new RouteViewModel(_mockRouteService.Object);

            // Assert
            Assert.NotNull(viewModel.RoutePoints);
            Assert.Empty(viewModel.RoutePoints);
            Assert.NotNull(viewModel.GenerateRouteCommand);
            Assert.False(viewModel.IsBusy);
            Assert.Equal(string.Empty, viewModel.ErrorMessage);
        }

        [Fact]
        public void Constructor_WithNullService_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new RouteViewModel(null!));
        }

        [Fact]
        public async Task GenerateRouteCommand_WithValidLocations_ShouldPopulateRoutePoints()
        {
            // Arrange
            var startLocation = new LocationModel { Latitude = 39.9, Longitude = 32.8 };
            var endLocation = new LocationModel { Latitude = 41.0, Longitude = 29.0 };
            var expectedRoute = new List<LocationModel>
            {
                startLocation,
                new LocationModel { Latitude = 40.0, Longitude = 31.0 },
                new LocationModel { Latitude = 40.5, Longitude = 30.0 },
                endLocation
            };

            _mockRouteService
                .Setup(x => x.GetRouteAsync(It.IsAny<LocationModel>(), It.IsAny<LocationModel>()))
                .ReturnsAsync(expectedRoute);

            _viewModel.StartLocation = startLocation;
            _viewModel.EndLocation = endLocation;

            // Act
            _viewModel.GenerateRouteCommand.Execute(null);

            // Assert
            Assert.Equal(expectedRoute.Count, _viewModel.RoutePoints.Count);
            for (int i = 0; i < expectedRoute.Count; i++)
            {
                Assert.Equal(expectedRoute[i].Latitude, _viewModel.RoutePoints[i].Latitude);
                Assert.Equal(expectedRoute[i].Longitude, _viewModel.RoutePoints[i].Longitude);
            }
            Assert.False(_viewModel.IsBusy);
            Assert.Equal(string.Empty, _viewModel.ErrorMessage);
        }

        [Fact]
        public void GenerateRouteCommand_WithNullStartLocation_ShouldSetErrorMessage()
        {
            // Arrange
            _viewModel.StartLocation = null;
            _viewModel.EndLocation = new LocationModel { Latitude = 41.0, Longitude = 29.0 };

            // Act
            _viewModel.GenerateRouteCommand.Execute(null);

            // Assert
            Assert.Empty(_viewModel.RoutePoints);
            Assert.False(_viewModel.IsBusy);
            Assert.Equal("Start and End locations must be set.", _viewModel.ErrorMessage);
            _mockRouteService.Verify(x => x.GetRouteAsync(It.IsAny<LocationModel>(), It.IsAny<LocationModel>()), Times.Never);
        }

        [Fact]
        public void GenerateRouteCommand_WithNullEndLocation_ShouldSetErrorMessage()
        {
            // Arrange
            _viewModel.StartLocation = new LocationModel { Latitude = 39.9, Longitude = 32.8 };
            _viewModel.EndLocation = null;

            // Act
            _viewModel.GenerateRouteCommand.Execute(null);

            // Assert
            Assert.Empty(_viewModel.RoutePoints);
            Assert.False(_viewModel.IsBusy);
            Assert.Equal("Start and End locations must be set.", _viewModel.ErrorMessage);
            _mockRouteService.Verify(x => x.GetRouteAsync(It.IsAny<LocationModel>(), It.IsAny<LocationModel>()), Times.Never);
        }

        [Fact]
        public void GenerateRouteCommand_WhenServiceThrowsException_ShouldSetErrorMessage()
        {
            // Arrange
            var startLocation = new LocationModel { Latitude = 39.9, Longitude = 32.8 };
            var endLocation = new LocationModel { Latitude = 41.0, Longitude = 29.0 };
            var exceptionMessage = "Network error occurred";

            _mockRouteService
                .Setup(x => x.GetRouteAsync(It.IsAny<LocationModel>(), It.IsAny<LocationModel>()))
                .ThrowsAsync(new Exception(exceptionMessage));

            _viewModel.StartLocation = startLocation;
            _viewModel.EndLocation = endLocation;

            // Act
            _viewModel.GenerateRouteCommand.Execute(null);

            // Assert
            Assert.Empty(_viewModel.RoutePoints);
            Assert.False(_viewModel.IsBusy);
            Assert.Equal($"Failed to generate route: {exceptionMessage}", _viewModel.ErrorMessage);
        }

        [Fact]
        public void GenerateRouteCommand_ShouldSetIsBusyDuringExecution()
        {
            // Arrange
            var startLocation = new LocationModel { Latitude = 39.9, Longitude = 32.8 };
            var endLocation = new LocationModel { Latitude = 41.0, Longitude = 29.0 };
            var tcs = new TaskCompletionSource<List<LocationModel>>();
            var busyStates = new List<bool>();

            _mockRouteService
                .Setup(x => x.GetRouteAsync(It.IsAny<LocationModel>(), It.IsAny<LocationModel>()))
                .Returns(tcs.Task);

            _viewModel.StartLocation = startLocation;
            _viewModel.EndLocation = endLocation;

            // Subscribe to property changes to track IsBusy state
            _viewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(_viewModel.IsBusy))
                {
                    busyStates.Add(_viewModel.IsBusy);
                }
            };

            // Act
            var executeTask = Task.Run(() => _viewModel.GenerateRouteCommand.Execute(null));

            // Complete the mock service call
            tcs.SetResult(new List<LocationModel> { startLocation, endLocation });
            executeTask.Wait();

            // Assert
            Assert.Contains(true, busyStates); // IsBusy was set to true
            Assert.Contains(false, busyStates); // IsBusy was set to false
            Assert.False(_viewModel.IsBusy); // Final state should be false
        }

        [Fact]
        public void GenerateRouteCommand_WhenAlreadyBusy_ShouldNotExecute()
        {
            // Arrange
            var startLocation = new LocationModel { Latitude = 39.9, Longitude = 32.8 };
            var endLocation = new LocationModel { Latitude = 41.0, Longitude = 29.0 };

            _viewModel.StartLocation = startLocation;
            _viewModel.EndLocation = endLocation;

            // Manually set IsBusy to true to simulate ongoing operation
            typeof(RouteViewModel)
                .GetProperty(nameof(_viewModel.IsBusy))
                ?.SetValue(_viewModel, true);

            // Act
            _viewModel.GenerateRouteCommand.Execute(null);

            // Assert
            _mockRouteService.Verify(x => x.GetRouteAsync(It.IsAny<LocationModel>(), It.IsAny<LocationModel>()), Times.Never);
        }

        [Fact]
        public void GenerateRouteCommand_ShouldClearPreviousRoutePoints()
        {
            // Arrange
            var startLocation = new LocationModel { Latitude = 39.9, Longitude = 32.8 };
            var endLocation = new LocationModel { Latitude = 41.0, Longitude = 29.0 };
            var newRoute = new List<LocationModel> { startLocation, endLocation };

            // Add some initial points
            _viewModel.RoutePoints.Add(new LocationModel { Latitude = 1.0, Longitude = 1.0 });
            _viewModel.RoutePoints.Add(new LocationModel { Latitude = 2.0, Longitude = 2.0 });

            _mockRouteService
                .Setup(x => x.GetRouteAsync(It.IsAny<LocationModel>(), It.IsAny<LocationModel>()))
                .ReturnsAsync(newRoute);

            _viewModel.StartLocation = startLocation;
            _viewModel.EndLocation = endLocation;

            // Act
            _viewModel.GenerateRouteCommand.Execute(null);

            // Assert
            Assert.Equal(newRoute.Count, _viewModel.RoutePoints.Count);
            Assert.Equal(startLocation.Latitude, _viewModel.RoutePoints[0].Latitude);
            Assert.Equal(endLocation.Latitude, _viewModel.RoutePoints[1].Latitude);
        }

        [Fact]
        public void StartLocation_WhenSet_ShouldRaisePropertyChanged()
        {
            // Arrange
            var location = new LocationModel { Latitude = 39.9, Longitude = 32.8 };
            var propertyChangedRaised = false;

            _viewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(_viewModel.StartLocation))
                    propertyChangedRaised = true;
            };

            // Act
            _viewModel.StartLocation = location;

            // Assert
            Assert.True(propertyChangedRaised);
            Assert.Equal(location, _viewModel.StartLocation);
        }

        [Fact]
        public void EndLocation_WhenSet_ShouldRaisePropertyChanged()
        {
            // Arrange
            var location = new LocationModel { Latitude = 41.0, Longitude = 29.0 };
            var propertyChangedRaised = false;

            _viewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(_viewModel.EndLocation))
                    propertyChangedRaised = true;
            };

            // Act
            _viewModel.EndLocation = location;

            // Assert
            Assert.True(propertyChangedRaised);
            Assert.Equal(location, _viewModel.EndLocation);
        }
    }
}
