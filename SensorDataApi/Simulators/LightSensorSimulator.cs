using Newtonsoft.Json;
using SensorDataApi.Exceptions;
using SensorDataApi.ViewModels;
using System.Text;

namespace SensorDataApi.Simulators
{
    public class LightSensorSimulator
    {
        private readonly HttpClient _httpClient;
        private readonly string _serverUrl;
        private readonly long _deviceId;
        private readonly Random _random;

        private readonly ILogger<LightSensorSimulator> _logger;
        public long DeviceId => _deviceId;

        private static long _lastGeneratedDeviceId = 0;

        public LightSensorSimulator(string serverUrl, ILogger<LightSensorSimulator> logger)
        {
            _httpClient = new HttpClient();
            _serverUrl = serverUrl;
            _deviceId = GenerateUniqueDeviceId();
            _random = new Random((int)_deviceId);
            _logger = logger;
        }

        private static long GenerateUniqueDeviceId()
        {
            lock (typeof(LightSensorSimulator))
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
                    var telemetryData = new List<LightSensorViewModel>();

                    for (int i = 0; i < 4; i++)
                    {
                        var illuminance = GenerateRandomIlluminance();
                        var telemetry = new LightSensorViewModel
                        {
                            Illuminance = illuminance,
                            Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                            DeviceId = _deviceId,
                        };
                        telemetryData.Add(telemetry);

                        await Task.Delay(TimeSpan.FromMinutes(15));
                    }

                    await PostTelemetryData(telemetryData);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred during simulation.");
                    throw new DataSendException("Error occurred during simulation");
                }

            }
        }

        private async Task PostTelemetryData(List<LightSensorViewModel> telemetryData)
        {
            var jsonTelemetry = JsonConvert.SerializeObject(telemetryData);

            try
            {
                var response = await _httpClient.PostAsync($"{_serverUrl}/devices/{_deviceId}/telemetry",
                    new StringContent(jsonTelemetry, Encoding.UTF8, "application/json"));

                response.EnsureSuccessStatusCode();
                _logger.LogInformation("Posting telemetry data.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending telemetry data.");
                throw new DataSendException("Error occurred while sending data");
            }
        }

        /// <summary>
        /// This method generates random illuminance where is used if else statement 
        /// there are used Min and Max which does not exceeds or belows it
        /// WeatherFactor is random between (0.6 , 1.0)
        /// </summary>
        /// <returns></returns>
        private double GenerateRandomIlluminance()
        {
            int currentHour = DateTimeOffset.UtcNow.Hour;
            double timeFactor;

            if (currentHour < 12) // First half of the day (First 12 Hours of the day)
            {
                timeFactor = 0.2 + (0.8 * currentHour / 12.0); // Increase
                timeFactor = Math.Min(timeFactor, 1.0);
            }
            else // Second half of the day
            {
                timeFactor = 1.0 - 0.8 * (currentHour - 12) / 12.0; // Decrease 
                timeFactor = Math.Max(timeFactor, 0.2);
            }

            double weatherFactor = (_random.NextDouble() * 0.4) + 0.6;

            double illuminance = weatherFactor * timeFactor * 1000.0;
            return Math.Round(illuminance * 2) / 2.0;
        }
    }
}
