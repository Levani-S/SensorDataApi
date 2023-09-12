namespace SensorDataApi.Tests
{

    [TestFixture]
    public class TempSensorServiceTests
    {
        [Test]
        public async Task TestGetMaxTemperatureForLastThirtyDays()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var tempSensorRepositoryMock = new Mock<ITempSensorRepository>();
            var loggerMock = new Mock<ILogger<TempSensorService>>();

            tempSensorRepositoryMock.Setup(repo =>
                repo.GetMaxTemperatureForLastThirtyDaysAsync(It.IsAny<long>()))
                .ReturnsAsync(new List<MaxTemperatureViewModel>());

            unitOfWorkMock.SetupGet(uow => uow.TempSensors)
                .Returns(tempSensorRepositoryMock.Object);

            var tempSensorService = new TempSensorService(unitOfWorkMock.Object, loggerMock.Object);

            // Act
            var result = await tempSensorService.GetMaxTemperatureForLastThirtyDaysAsync(871);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task TestAddTempSensorDataAsync()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var tempSensorRepositoryMock = new Mock<ITempSensorRepository>();
            var loggerMock = new Mock<ILogger<TempSensorService>>();

            tempSensorRepositoryMock.Setup(repo =>
                repo.AddTempSensorDataAsync(It.IsAny<List<TempSensor>>()))
                .Returns(Task.CompletedTask);

            unitOfWorkMock.SetupGet(uow => uow.TempSensors)
                .Returns(tempSensorRepositoryMock.Object);

            var tempSensorService = new TempSensorService(unitOfWorkMock.Object, loggerMock.Object);

            var tempSensorDataList = new List<TempSensorViewModel>
            {
                new TempSensorViewModel
                {
                    Temperature = 25,
                    Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    DeviceId = 456
                },
                new TempSensorViewModel
                {
                    Temperature = 30,
                    Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    DeviceId = 921
                }
            };


            await tempSensorService.AddTempSensorDataAsync(tempSensorDataList);

            tempSensorRepositoryMock.Verify(repo =>
                repo.AddTempSensorDataAsync(It.IsAny<List<TempSensor>>()), Times.Once);
        }
        [Test]
        public Task TestExceptionHandling_GetMaxTemperatureForLastThirtyDays()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var tempSensorRepositoryMock = new Mock<ITempSensorRepository>();
            var loggerMock = new Mock<ILogger<TempSensorService>>();

            tempSensorRepositoryMock.Setup(repo =>
                repo.GetMaxTemperatureForLastThirtyDaysAsync(It.IsAny<long>()))
                .ThrowsAsync(new Exception("Simulated repository exception"));

            unitOfWorkMock.SetupGet(uow => uow.TempSensors)
                .Returns(tempSensorRepositoryMock.Object);

            var tempSensorService = new TempSensorService(unitOfWorkMock.Object, loggerMock.Object);

            // Act & Assert
            Assert.ThrowsAsync<DataInsertionException>(() =>
                tempSensorService.GetMaxTemperatureForLastThirtyDaysAsync(456));

            return Task.CompletedTask;
        }

        [Test]
        public void TestNegativeInputs_AddTempSensorDataAsync()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var tempSensorRepositoryMock = new Mock<ITempSensorRepository>();
            var loggerMock = new Mock<ILogger<TempSensorService>>();

            // Mock the behavior of your repository and unit of work
            tempSensorRepositoryMock.Setup(repo =>
                repo.AddTempSensorDataAsync(It.IsAny<List<TempSensor>>()))
                .ThrowsAsync(new DataInsertionException("Simulated data insertion error"));

            unitOfWorkMock.SetupGet(uow => uow.TempSensors)
                .Returns(tempSensorRepositoryMock.Object);

            var tempSensorService = new TempSensorService(unitOfWorkMock.Object, loggerMock.Object);

            var tempSensorDataList = new List<TempSensorViewModel>();

            // Act and Assert
            Assert.ThrowsAsync<DataInsertionException>(async () =>
            {
                await tempSensorService.AddTempSensorDataAsync(tempSensorDataList);
            });
        }



    }
}
