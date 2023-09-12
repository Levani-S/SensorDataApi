using SensorDataApi.Data.Repositories;
using SensorDataApi.Data.UnitOfWork;
using SensorDataApi.Exceptions;
using SensorDataApi.Models;
using SensorDataApi.ViewModels;

namespace SensorDataApi.Services
{
    public class TempSensorService : ITempSensorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITempSensorRepository _tempSensorRepository;
        private readonly ILogger<TempSensorService> _logger;

        public TempSensorService(IUnitOfWork unitOfWork, ILogger<TempSensorService> logger)
        {
            _unitOfWork = unitOfWork;
            _tempSensorRepository = _unitOfWork.TempSensors;
            _logger = logger;
        }

        public async Task<List<MaxTemperatureViewModel>> GetMaxTemperatureForLastThirtyDaysAsync(long id)
        {
            try
            {
                return await _tempSensorRepository.GetMaxTemperatureForLastThirtyDaysAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving max temperature data.");
                throw new DataInsertionException("Error occurred during retrieving data");
            }
        }

        public async Task AddTempSensorDataAsync(List<TempSensorViewModel> tempSensorDataList)
        {
            if (tempSensorDataList == null)
            {
                throw new ArgumentNullException(nameof(tempSensorDataList));
            }

            try
            {
                var sensorDataList = new List<TempSensor>();

                foreach (var tempData in tempSensorDataList)
                {
                    var sensorData = new TempSensor
                    {
                        Temperature = tempData.Temperature,
                        Time = tempData.Time,
                        DeviceId = tempData.DeviceId,
                    };

                    sensorDataList.Add(sensorData);
                }
                _logger.LogInformation("Adding temp sensor data.");

                await _tempSensorRepository.AddTempSensorDataAsync(sensorDataList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding temp sensor data.");
                throw new DataInsertionException("Error occurred during data insertion.");
            }
        }


    }
}
