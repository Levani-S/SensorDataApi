using SensorDataApi.Data.Repositories;

#pragma warning disable CA1816 //delete if using UnitOfWorks.Dispose Outside of repository

namespace SensorDataApi.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SensorDataDbContext _context;
        public ILightSensorRepository LightSensors { get; }
        public ITempSensorRepository TempSensors { get; }

        public UnitOfWork(SensorDataDbContext context, ILogger<LightSensorRepository> lightSensorLogger, ILogger<TempSensorRepository> tempSensorLogger)
        {
            _context = context;
            LightSensors = new LightSensorRepository(_context, lightSensorLogger); 
            TempSensors = new TempSensorRepository(_context, tempSensorLogger); 
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
