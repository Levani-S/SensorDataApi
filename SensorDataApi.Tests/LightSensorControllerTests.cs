using SensorDataApi.Controllers;
using Microsoft.AspNetCore.Mvc;


namespace SensorDataApi.Tests
{
    [TestFixture]
    public class LightSensorControllerTests
    {
        private LightSensorController _controller;
        private Mock<ILightSensorService> _lightSensorServiceMock;

        [SetUp]
        public void Setup()
        {
            _lightSensorServiceMock = new Mock<ILightSensorService>();
            _controller = new LightSensorController(_lightSensorServiceMock.Object);
        }

        [Test]
        public async Task GetStatistics_ReturnsOk()
        {
            // Arrange
            long deviceId = 1;
            var statistics = new List<MaxIlluminanceViewModel>
            {
                new MaxIlluminanceViewModel { Date = "2023-09-01", MaxIlluminance = 1200 },
                new MaxIlluminanceViewModel { Date = "2023-09-02", MaxIlluminance = 1100 },
                new MaxIlluminanceViewModel { Date = "2023-09-03", MaxIlluminance = 1300 },
                new MaxIlluminanceViewModel { Date = "2023-09-04", MaxIlluminance = 1250 }
            };
            _lightSensorServiceMock.Setup(s => s.GetMaxIlluminanceForLastThirtyDaysAsync(deviceId))
                .ReturnsAsync(statistics);

            // Act
            var result = await _controller.GetStatistics(deviceId);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task PostTelemetry_ReturnsOk()
        {
            // Arrange
            var telemetryData = new List<LightSensorViewModel>
            {

                new LightSensorViewModel { Illuminance = 500, Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), DeviceId = 1 },
                new LightSensorViewModel { Illuminance = 600, Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), DeviceId = 2 },
                new LightSensorViewModel { Illuminance = 700, Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), DeviceId = 3 },
                new LightSensorViewModel { Illuminance = 800, Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), DeviceId = 4 }
            };
            _lightSensorServiceMock.Setup(s => s.AddLightSensorDataAsync(telemetryData))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.PostTelemetry(telemetryData);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
    }
}
