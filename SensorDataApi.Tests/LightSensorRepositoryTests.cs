using Microsoft.EntityFrameworkCore;

#pragma warning disable CS8625


namespace SensorDataApi.Tests
{
    [TestFixture]
    public class LightSensorRepositoryTests
    {
        private LightSensorRepository _lightSensorRepository;
        private SensorDataDbContext _dbContext;
        private ILogger<LightSensorRepository> _logger;

        [SetUp]
        public void Setup()
        {
            // Create a mock for DbContextOptions
            var options = new DbContextOptionsBuilder<SensorDataDbContext>()
                .UseInMemoryDatabase(databaseName: "LightSensorDatabase")
                .Options;

            // Create a mock DbContext and Logger
            _dbContext = new SensorDataDbContext(options);
            _logger = new Mock<ILogger<LightSensorRepository>>().Object;

            _lightSensorRepository = new LightSensorRepository(_dbContext, _logger);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }

        [Test]
        public async Task GetMaxIlluminanceForLastThirtyDaysAsync_ValidData_ReturnsStatistics()
        {
            // Arrange
            var deviceId = 1;

            // Add some sample LightSensor data to the in-memory database
            var testData = new List<LightSensor>
            {
                new LightSensor { Illuminance = 1000.0, Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), DeviceId = deviceId },
                new LightSensor { Illuminance = 900.0, Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), DeviceId = deviceId },
                new LightSensor { Illuminance = 1100.0, Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), DeviceId = deviceId },
            };
            _dbContext.LightSensor.AddRange(testData);
            _dbContext.SaveChanges();

            // Act
            var statistics = await _lightSensorRepository.GetMaxIlluminanceForLastThirtyDaysAsync(deviceId);

            // Assert
            Assert.That(statistics, Is.Not.Null);
            Assert.That(statistics, Has.Count.EqualTo(1));
            Assert.That(statistics[0].MaxIlluminance, Is.GreaterThanOrEqualTo(0));
        }

        [Test]
        public async Task AddLightSensorDataAsync_ValidData_AddsDataToDatabase()
        {
            // Arrange
            var testData = new List<LightSensor>
            {
                new LightSensor { Illuminance = 1000.0, Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), DeviceId = 1 },
                new LightSensor { Illuminance = 900.0, Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), DeviceId = 1 },
                new LightSensor { Illuminance = 1100.0, Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), DeviceId = 2 },
            };

            // Act
            await _lightSensorRepository.AddLightSensorDataAsync(testData);

            // Assert
            var savedData = _dbContext.LightSensor.ToList();
            Assert.That(savedData, Is.Not.Null);
            Assert.That(savedData, Has.Count.EqualTo(3));
        }

        [Test]
        public void AddLightSensorDataAsync_InvalidData_ThrowsDataInsertionException()
        {
            // Arrange
            var testData = new List<LightSensor>
            {
                null
            };

            // Act & Assert
            Assert.ThrowsAsync<DataInsertionException>(() =>
                _lightSensorRepository.AddLightSensorDataAsync(testData));
        }
    }
}
