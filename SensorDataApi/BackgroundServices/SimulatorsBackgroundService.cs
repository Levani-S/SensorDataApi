using SensorDataApi.Simulators;

namespace SensorDataApi.BackgroundServices
{
    public class SimulatorsBackgroundService : IHostedService
    {
        private readonly string _serverUrl;
        private readonly ILogger<SimulatorsBackgroundService> _logger;
        private readonly ILogger<TempSensorSimulator> _tempSimulatorLogger;
        private readonly ILogger<LightSensorSimulator> _lightSimulatorLogger;

        public SimulatorsBackgroundService(IConfiguration configuration, ILogger<SimulatorsBackgroundService> logger, ILogger<TempSensorSimulator> tempSimulatorLogger, ILogger<LightSensorSimulator> lightSimulatorLogger)
        {
            _serverUrl = configuration.GetValue<string>("ServerSettings:ServerUrl");
            _logger = logger;
            _tempSimulatorLogger = tempSimulatorLogger;
            _lightSimulatorLogger = lightSimulatorLogger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            //two Instances of tempsimulator
            var tempSimulator1 = new TempSensorSimulator(_serverUrl, _tempSimulatorLogger);
            var tempSimulationTask1 = tempSimulator1.StartSimulation();

            var tempSimulator2 = new TempSensorSimulator(_serverUrl, _tempSimulatorLogger);
            var tempSimulationTask2 = tempSimulator2.StartSimulation();

            //two instances of lightsimulator
            var lightSimulator1 = new LightSensorSimulator(_serverUrl, _lightSimulatorLogger);
            var lightSimulationTask1 = lightSimulator1.StartSimulation();

            var lightSimulator2 = new LightSensorSimulator(_serverUrl, _lightSimulatorLogger);
            var lightSimulationTask2 = lightSimulator2.StartSimulation();

            Task.WhenAll(tempSimulationTask1, tempSimulationTask2, lightSimulationTask1, lightSimulationTask2).ContinueWith(task =>
            {
                if (task.Exception != null)
                {
                    _logger.LogError(task.Exception, "Sensor Simulator encountered an error.");
                }
            }, cancellationToken);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
