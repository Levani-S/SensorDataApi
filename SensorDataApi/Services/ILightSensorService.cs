using SensorDataApi.ViewModels;

namespace SensorDataApi.Services
{
    public interface ILightSensorService
    {
        Task<List<MaxIlluminanceViewModel>> GetMaxIlluminanceForLastThirtyDaysAsync(long id);
        Task AddLightSensorDataAsync(List<LightSensorViewModel> lightSensorDataList);
    }
}
