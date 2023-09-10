using SensorDataApi.Models;

namespace SensorDataApi.Data.SeedData
{
    /// <summary>
    /// //It's just a test method which helps to test 30 day
    /// api get method use it once only to seed first 30 day
    /// data check program class and uncomment seeding
    /// </summary>
    public class DbInitializer
    {
        private readonly SensorDataDbContext _dbContext;

        public DbInitializer(SensorDataDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task InitializeAsync()
        {
            var currentDate = DateTime.UtcNow;
            var thirtyDaysAgo = currentDate.AddDays(-30);

            var lightSensorData = new List<LightSensor>();

            var random = new Random();

            for (var date = thirtyDaysAgo; date <= currentDate; date = date.AddDays(1))
            {
                var illuminance = random.Next(400, 850);

                var lightSensorEntry = new LightSensor
                {
                    Illuminance = illuminance,
                    Time = (long)(date - new DateTime(1970, 1, 1)).TotalSeconds,//because of unix timestamp i use that
                    DeviceId = random.Next(1, 2)
                };

                lightSensorData.Add(lightSensorEntry);
            }

            _dbContext.LightSensor.AddRange(lightSensorData);

            await _dbContext.SaveChangesAsync();

        }
    }
}

