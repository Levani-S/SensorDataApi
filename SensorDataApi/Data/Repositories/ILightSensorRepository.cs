using SensorDataApi.Models;
using SensorDataApi.ViewModels;

namespace SensorDataApi.Data.Repositories
{
    public interface ILightSensorRepository
    {
        Task<List<MaxIlluminanceViewModel>> GetMaxIlluminanceForLastThirtyDaysAsync(long id);
        Task AddLightSensorDataAsync(List<LightSensor> lightSensorDataList);
    }
}
