using SensorDataApi.ViewModels;

namespace SensorDataApi.Services
{
    public interface ITempSensorService
    {
        Task<List<MaxTemperatureViewModel>> GetMaxTemperatureForLastThirtyDaysAsync(long id);
        Task AddTempSensorDataAsync(List<TempSensorViewModel> tempSensorDataList);
    }
}
