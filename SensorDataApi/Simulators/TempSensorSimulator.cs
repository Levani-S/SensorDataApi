using Newtonsoft.Json;
using SensorDataApi.Exceptions;
using SensorDataApi.ViewModels;
using System.Text;

namespace SensorDataApi.Simulators
{
    public class TempSensorSimulator
    {
        private readonly HttpClient _httpClient;
        private readonly string _serverUrl;
        private readonly long _deviceId;
        private readonly Random _random;

        private readonly ILogger<TempSensorSimulator> _logger;
        public long DeviceId => _deviceId;

        private static long _lastGeneratedDeviceId = 0;

        public TempSensorSimulator(string serverUrl, ILogger<TempSensorSimulator> logger)
        {
            _httpClient = new HttpClient();
            _serverUrl = serverUrl;
            _deviceId = GenerateUniqueDeviceId();
            _random = new Random((int)_deviceId);
            _logger = logger;
        }

        private static long GenerateUniqueDeviceId()
        {
            lock (typeof(TempSensorSimulator))
            {
                _lastGeneratedDeviceId++;
                return _lastGeneratedDeviceId;
            }
        }

        public async Task StartSimulation()
        {
            while (true)
            {
                try
                {
                    var tempData = new List<TempSensorViewModel>();

                    for (int i = 0; i < 6; i++)
                    {
                        var temperature = GenerateRandomTemperature();
                        var temp = new TempSensorViewModel
                        {
                            Temperature = temperature,
                            Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                            DeviceId = _deviceId,
                        };
                        tempData.Add(temp);

                        await Task.Delay(TimeSpan.FromMinutes(15));
                    }

                    await PostTemperatureData(tempData);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred during simulation.");
                    throw new DataSendException("Error occurred during simulation");
                }

            }
        }

        private async Task PostTemperatureData(List<TempSensorViewModel> tempData)
        {
            var jsonTelemetry = JsonConvert.SerializeObject(tempData);

            try
            {
                var response = await _httpClient.PostAsync($"{_serverUrl}/devices/{_deviceId}/temperature",
                    new StringContent(jsonTelemetry, Encoding.UTF8, "application/json"));

                response.EnsureSuccessStatusCode();
                _logger.LogInformation("Posting temperature data.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending temperature data.");
                throw new DataSendException("Error occurred while sending data");
            }
        }

        /// <summary>
        /// This method generates random Temperature where is used if else statement 
        /// there are used for Seasonal times and temperatures 
        /// WeatherFactor is randomly between  0 - Rain, 1 - Snow, 2 - Sunny, 3 - Cloudy
        /// </summary>
        /// <returns></returns>
        private double GenerateRandomTemperature()
        {
            int currentHour = DateTimeOffset.UtcNow.Hour;
            double temperature;
            DateTime now = DateTime.UtcNow;
            bool isSummer = now.Month >= 6 && now.Month <= 9;
            bool isWinter = now.Month == 12 || now.Month <= 2;

            int weatherFactor = _random.Next(4);

            if (isSummer)
            {
                temperature = (currentHour >= 12 && currentHour < 18) ? RandomInRange(26, 40) :// temperature in times
                             (currentHour >= 18 && currentHour < 22) ? RandomInRange(20, 28) :
                             (currentHour >= 22 || currentHour < 8) ? RandomInRange(4, 12) :
                                                                      RandomInRange(10, 20);
            }
            else if (isWinter)
            {
                temperature = (currentHour >= 12 && currentHour < 18) ? RandomInRange(12, 18) :
                             (currentHour >= 18 && currentHour < 22) ? RandomInRange(8, 14) :
                             (currentHour >= 22 || currentHour < 8) ? RandomInRange(-10, 2) :
                                                                     RandomInRange(6, 12);
            }
            else // Other seasons
            {
                double minTemperature = 5;
                double maxTemperature = 30;

                temperature = weatherFactor switch
                {
                    // Rain
                    0 => RandomInRange(minTemperature, maxTemperature) - 5,
                    // Snow
                    1 => RandomInRange(minTemperature, maxTemperature) - 10,
                    // Sunny
                    2 => RandomInRange(minTemperature, maxTemperature) + 5,
                    // Cloudy
                    _ => RandomInRange(minTemperature, maxTemperature),
                };
            }

            return Math.Round(temperature * 2) / 2.0;
        }

        private double RandomInRange(double minValue, double maxValue)
        {
            return (_random.NextDouble() * (maxValue - minValue)) + minValue;
        }

    }
}
