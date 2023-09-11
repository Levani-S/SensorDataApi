using SensorDataApi.Data.Repositories;

namespace SensorDataApi.Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        ILightSensorRepository LightSensors { get; }
        ITempSensorRepository TempSensors { get; }
        int Complete();
    }
}
