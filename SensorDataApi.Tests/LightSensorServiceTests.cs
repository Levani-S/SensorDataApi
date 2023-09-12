namespace SensorDataApi.Tests
{
    [TestFixture]
    public class LightSensorServiceTests
    {
        [Test]
        public async Task TestGetMaxIlluminanceForLastThirtyDays()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var lightSensorRepositoryMock = new Mock<ILightSensorRepository>();
            var loggerMock = new Mock<ILogger<LightSensorService>>();

            lightSensorRepositoryMock.Setup(repo =>
                repo.GetMaxIlluminanceForLastThirtyDaysAsync(It.IsAny<long>()))
                .ReturnsAsync(new List<MaxIlluminanceViewModel>());

            unitOfWorkMock.SetupGet(uow => uow.LightSensors)
                .Returns(lightSensorRepositoryMock.Object);

            var lightSensorService = new LightSensorService(unitOfWorkMock.Object, loggerMock.Object);

            // Act
            var result = await lightSensorService.GetMaxIlluminanceForLastThirtyDaysAsync(123);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(0));
        }

        [Test]
        public async Task TestAddLightSensorDataAsync()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var lightSensorRepositoryMock = new Mock<ILightSensorRepository>();
            var loggerMock = new Mock<ILogger<LightSensorService>>();

            lightSensorRepositoryMock.Setup(repo =>
                repo.AddLightSensorDataAsync(It.IsAny<List<LightSensor>>()))
                .Returns(Task.CompletedTask);

            unitOfWorkMock.SetupGet(uow => uow.LightSensors)
                .Returns(lightSensorRepositoryMock.Object);

            var lightSensorService = new LightSensorService(unitOfWorkMock.Object, loggerMock.Object);

            var lightSensorDataList = new List<LightSensorViewModel>
            {
                new LightSensorViewModel
                {
                    Illuminance = 100,
                    Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    DeviceId = 1
                },
                new LightSensorViewModel
                {
                    Illuminance = 300,
                    Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    DeviceId = 1
                },
                new LightSensorViewModel
                {
                    Illuminance = 129,
                    Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    DeviceId = 2
                },
                new LightSensorViewModel
                {
                    Illuminance = 100,
                    Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    DeviceId = 4
                }
            };

            await lightSensorService.AddLightSensorDataAsync(lightSensorDataList);

            lightSensorRepositoryMock.Verify(repo =>
                repo.AddLightSensorDataAsync(It.IsAny<List<LightSensor>>()), Times.Once);
        }
        [Test]
        public Task TestExceptionHandling_GetMaxIlluminanceForLastThirtyDays()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var lightSensorRepositoryMock = new Mock<ILightSensorRepository>();
            var loggerMock = new Mock<ILogger<LightSensorService>>();

            lightSensorRepositoryMock.Setup(repo =>
                repo.GetMaxIlluminanceForLastThirtyDaysAsync(It.IsAny<long>()))
                .ThrowsAsync(new Exception("Simulated repository exception"));

            unitOfWorkMock.SetupGet(uow => uow.LightSensors)
                .Returns(lightSensorRepositoryMock.Object);

            var lightSensorService = new LightSensorService(unitOfWorkMock.Object, loggerMock.Object);

            // Act & Assert
            Assert.ThrowsAsync<DataInsertionException>(() =>
                lightSensorService.GetMaxIlluminanceForLastThirtyDaysAsync(123));

            return Task.CompletedTask;
        }

        [Test]
        public Task TestNegativeInputs_AddLightSensorDataAsync()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var lightSensorRepositoryMock = new Mock<ILightSensorRepository>();
            var loggerMock = new Mock<ILogger<LightSensorService>>();

            unitOfWorkMock.SetupGet(uow => uow.LightSensors)
                .Returns(lightSensorRepositoryMock.Object);

            var lightSensorService = new LightSensorService(unitOfWorkMock.Object, loggerMock.Object);

            // Act & Assert
            var nullDataException = Assert.ThrowsAsync<ArgumentNullException>(() =>
                lightSensorService.AddLightSensorDataAsync(null));
            Assert.That(nullDataException.ParamName, Is.EqualTo("lightSensorDataList"));

            var emptyListException = Assert.ThrowsAsync<ArgumentException>(() =>
                lightSensorService.AddLightSensorDataAsync(new List<LightSensorViewModel>()));
            Assert.That(emptyListException.ParamName, Is.EqualTo("lightSensorDataList"));

            return Task.CompletedTask;
        }



    }

}
