using SensorDataApi.Models;
using SensorDataApi.ViewModels;

namespace SensorDataApi.Data.Repositories
{
    public interface ITempSensorRepository
    {
        Task<List<MaxTemperatureViewModel>> GetMaxTemperatureForLastThirtyDaysAsync(long id);
        Task AddTempSensorDataAsync(List<TempSensor> tempSensorDataList);
    }
}
