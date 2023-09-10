using Microsoft.EntityFrameworkCore;
using SensorDataApi.Exceptions;
using SensorDataApi.Models;
using SensorDataApi.ViewModels;

namespace SensorDataApi.Data.Repositories
{
    public class TempSensorRepository : ITempSensorRepository
    {
        private readonly SensorDataDbContext _dbContext;
        private readonly ILogger<TempSensorRepository> _logger;

        public TempSensorRepository(SensorDataDbContext dbContext, ILogger<TempSensorRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<List<MaxTemperatureViewModel>> GetMaxTemperatureForLastThirtyDaysAsync(long id)
        {
            try
            {
                var maxTemperature = new List<MaxTemperatureViewModel>();
                var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

                var thirtyDaysAgoTimestamp = (long)(thirtyDaysAgo - new DateTime(1970, 1, 1)).TotalSeconds;// Because of Unix I do 1970 january 1

                var result = await _dbContext.TempSensor
                    .Where(data => data.DeviceId == id && data.Time >= thirtyDaysAgoTimestamp)
                    .ToListAsync();

                // Group data by date and calculate maximum temperature for each day
                var groupedData = result
                    .GroupBy(data => DateTimeOffset.FromUnixTimeSeconds(data.Time).Date);

                foreach (var group in groupedData)
                {
                    var maxTemperatureForDay = group.Max(data => data.Temperature);
                    maxTemperature.Add(new MaxTemperatureViewModel
                    {
                        Date = group.Key.ToString("yyyy-MM-dd"),
                        MaxTemperature = maxTemperatureForDay
                    });
                }

                return maxTemperature;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving max temperature data.");
                throw new DataInsertionException("Error occurred during retrieving data");
            }
        }

        public async Task AddTempSensorDataAsync(List<TempSensor> tempSensorDataList)
        {
            try
            {
                _dbContext.TempSensor.AddRange(tempSensorDataList);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding temp sensor data.");
                throw new DataInsertionException("Error occurred during data insertion.");
            }
        }
    }
}
