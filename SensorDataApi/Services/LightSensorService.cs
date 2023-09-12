using SensorDataApi.Data.Repositories;
using SensorDataApi.Data.UnitOfWork;
using SensorDataApi.Exceptions;
using SensorDataApi.Models;
using SensorDataApi.ViewModels;

namespace SensorDataApi.Services
{
    public class LightSensorService : ILightSensorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILightSensorRepository _lightSensorRepository;
        private readonly ILogger<LightSensorService> _logger;

        public LightSensorService(IUnitOfWork unitOfWork, ILogger<LightSensorService> logger)
        {
            _unitOfWork = unitOfWork;
            _lightSensorRepository = _unitOfWork.LightSensors;
            _logger = logger;
        }

        public async Task<List<MaxIlluminanceViewModel>> GetMaxIlluminanceForLastThirtyDaysAsync(long id)
        {
            try
            {
                return await _lightSensorRepository.GetMaxIlluminanceForLastThirtyDaysAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving max illuminance data.");
                throw new DataInsertionException("Error occurred during retrieving data");
            }
        }

        public async Task AddLightSensorDataAsync(List<LightSensorViewModel>? lightSensorDataList)
        {
            if (lightSensorDataList == null)
            {
                throw new ArgumentNullException(nameof(lightSensorDataList));
            }

            if (lightSensorDataList.Count == 0)
            {
                throw new ArgumentException("lightSensorDataList cannot be empty", nameof(lightSensorDataList));
            }
            try
            {
                var sensorDataList = new List<LightSensor>();

                foreach (var telemetryData in lightSensorDataList)
                {
                    var sensorData = new LightSensor
                    {
                        Illuminance = telemetryData.Illuminance,
                        Time = telemetryData.Time,
                        DeviceId = telemetryData.DeviceId,
                    };

                    sensorDataList.Add(sensorData);
                }
                _logger.LogInformation("Adding light sensor data.");

                await _lightSensorRepository.AddLightSensorDataAsync(sensorDataList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding light sensor data.");
                throw new DataInsertionException("Error occurred during data insertion.");
            }
        }

    }
}
