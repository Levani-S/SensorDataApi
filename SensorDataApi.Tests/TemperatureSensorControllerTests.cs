using SensorDataApi.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SensorDataApi.Tests
{
    [TestFixture]
    public class TemperatureSensorControllerTests
    {
        private TemperatureSensorController _controller;
        private Mock<ITempSensorService> _tempSensorServiceMock;

        [SetUp]
        public void Setup()
        {
            _tempSensorServiceMock = new Mock<ITempSensorService>();
            _controller = new TemperatureSensorController(_tempSensorServiceMock.Object);
        }

        [Test]
        public async Task GetTemperatureStatistics_ReturnsOk()
        {
            // Arrange
            long deviceId = 1;
            var statistics = new List<MaxTemperatureViewModel>
            {
                new MaxTemperatureViewModel { Date = "2023-09-01", MaxTemperature = 28.5 },
                new MaxTemperatureViewModel { Date = "2023-09-02", MaxTemperature = 27.8 },
                new MaxTemperatureViewModel { Date = "2023-09-03", MaxTemperature = 29.2 },
                new MaxTemperatureViewModel { Date = "2023-09-04", MaxTemperature = 26.5 }
            };
            _tempSensorServiceMock.Setup(s => s.GetMaxTemperatureForLastThirtyDaysAsync(deviceId))
                .ReturnsAsync(statistics);

            // Act
            var result = await _controller.GetTemperatureStatistics(deviceId);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task PostTelemetry_ReturnsOk()
        {
            // Arrange
            var tempData = new List<TempSensorViewModel>
            {
                new TempSensorViewModel { Temperature = 25.5, Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), DeviceId = 1 },
                new TempSensorViewModel { Temperature = 24.0, Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), DeviceId = 3 },
                new TempSensorViewModel { Temperature = 26.2, Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), DeviceId = 2 },
                new TempSensorViewModel { Temperature = 23.8, Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), DeviceId = 4 }
            };
            _tempSensorServiceMock.Setup(s => s.AddTempSensorDataAsync(tempData))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.PostTelemetry(tempData);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
    }
}
