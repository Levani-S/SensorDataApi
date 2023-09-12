using Microsoft.EntityFrameworkCore;
using SensorDataApi.Exceptions;
using SensorDataApi.Models;
using SensorDataApi.ViewModels;

namespace SensorDataApi.Data.Repositories
{
    public class LightSensorRepository : ILightSensorRepository
    {
        private readonly SensorDataDbContext _dbContext;
        private readonly ILogger<LightSensorRepository> _logger;

        public LightSensorRepository(SensorDataDbContext dbContext, ILogger<LightSensorRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<List<MaxIlluminanceViewModel>> GetMaxIlluminanceForLastThirtyDaysAsync(long id)
        {
            try
            {
                var maxIlluminance = new List<MaxIlluminanceViewModel>();
                var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

                var thirtyDaysAgoTimestamp = (long)(thirtyDaysAgo - new DateTime(1970, 1, 1)).TotalSeconds;// Because of Unix I do 1970 january 1

                var result = await _dbContext.LightSensor
                    .Where(data => data.DeviceId == id && data.Time >= thirtyDaysAgoTimestamp)
                    .ToListAsync();

                // Group data by date and calculate maximum illuminance for each day
                var groupedData = result
                    .GroupBy(data => DateTimeOffset.FromUnixTimeSeconds(data.Time).Date);

                foreach (var group in groupedData)
                {
                    var maxIlluminanceForDay = group.Max(data => data.Illuminance);
                    maxIlluminance.Add(new MaxIlluminanceViewModel
                    {
                        Date = group.Key.ToString("yyyy-MM-dd"),
                        MaxIlluminance = maxIlluminanceForDay
                    });
                }

                return maxIlluminance;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving max illuminance data.");
                throw new DataInsertionException("Error occurred during retrieving data");
            }
        }

        public async Task AddLightSensorDataAsync(List<LightSensor> lightSensorDataList)
        {
            try
            {
                if (lightSensorDataList != null)
                {
                    _dbContext.LightSensor.AddRange(lightSensorDataList);
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    throw new ArgumentNullException(nameof(lightSensorDataList), "Light Sensor data is null");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding light sensor data.");
                throw new DataInsertionException("Error occurred during data insertion.");
            }
        }
    }
}
