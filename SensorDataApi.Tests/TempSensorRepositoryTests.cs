using Microsoft.EntityFrameworkCore;
#pragma warning disable CS8625

namespace SensorDataApi.Tests
{
    [TestFixture]
    public class TempSensorRepositoryTests
    {
        private TempSensorRepository _tempSensorRepository;
        private SensorDataDbContext _dbContext;
        private ILogger<TempSensorRepository> _logger;

        [SetUp]
        public void Setup()
        {
            //  mock for DbContextOptions
            var options = new DbContextOptionsBuilder<SensorDataDbContext>()
                .UseInMemoryDatabase(databaseName: "TempSensorDatabase")
                .Options;

            // mock DbContext and Logger
            _dbContext = new SensorDataDbContext(options);
            _logger = new Mock<ILogger<TempSensorRepository>>().Object;

            _tempSensorRepository = new TempSensorRepository(_dbContext, _logger);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }

        [Test]
        public async Task GetMaxTemperatureForLastThirtyDaysAsync_ValidData_ReturnsStatistics()
        {
            // Arrange
            var deviceId = 1;

            var testData = new List<TempSensor>
            {
                new TempSensor { Temperature = 25.5, Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), DeviceId = deviceId },
                new TempSensor { Temperature = 26.0, Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), DeviceId = deviceId },
                new TempSensor { Temperature = 24.5, Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), DeviceId = deviceId },
            };
            _dbContext.TempSensor.AddRange(testData);
            _dbContext.SaveChanges();

            // Act
            var statistics = await _tempSensorRepository.GetMaxTemperatureForLastThirtyDaysAsync(deviceId);

            // Assert
            Assert.That(statistics, Is.Not.Null);
            Assert.That(statistics, Has.Count.EqualTo(1));
            Assert.That(statistics[0].MaxTemperature, Is.GreaterThanOrEqualTo(0));
        }

        [Test]
        public async Task AddTempSensorDataAsync_ValidData_AddsDataToDatabase()
        {
            // Arrange
            var testData = new List<TempSensor>
            {
                new TempSensor { Temperature = 25.5, Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), DeviceId = 1 },
                new TempSensor { Temperature = 26.0, Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), DeviceId = 1 },
                new TempSensor { Temperature = 24.5, Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), DeviceId = 2 },
            };

            // Act
            await _tempSensorRepository.AddTempSensorDataAsync(testData);

            // Assert
            var savedData = _dbContext.TempSensor.ToList();
            Assert.That(savedData, Is.Not.Null);
            Assert.That(savedData, Has.Count.EqualTo(3));
        }

        [Test]
        public void AddTempSensorDataAsync_InvalidData_ThrowsDataInsertionException()
        {
            // Arrange
            var testData = new List<TempSensor>
            {
                null
            };

            // Act & Assert
            Assert.ThrowsAsync<DataInsertionException>(() =>
                _tempSensorRepository.AddTempSensorDataAsync(testData));
        }
    }
}
