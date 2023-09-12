using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SensorDataApi.BackgroundServices;
using SensorDataApi.Simulators;

namespace SensorDataApi.Tests
{
    [TestFixture]
    public class SimulatorsBackgroundServiceTests
    {
        [Test]
        public async Task TestSimulatorsBackgroundService()
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
                .AddLogging(builder => builder.AddConsole())
                .BuildServiceProvider();

            var logger = serviceProvider.GetService<ILogger<SimulatorsBackgroundService>>();
            var tempSimulatorLogger = serviceProvider.GetService<ILogger<TempSensorSimulator>>();
            var lightSimulatorLogger = serviceProvider.GetService<ILogger<LightSensorSimulator>>();

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var mockHttpClient = new Mock<HttpClient>();

            // Mock the logger instances if they are null
            if (logger == null)
            {
                var loggerMock = new Mock<ILogger<SimulatorsBackgroundService>>();
                logger = loggerMock.Object;
            }

            if (tempSimulatorLogger == null)
            {
                var tempSimulatorLoggerMock = new Mock<ILogger<TempSensorSimulator>>();
                tempSimulatorLogger = tempSimulatorLoggerMock.Object;
            }

            if (lightSimulatorLogger == null)
            {
                var lightSimulatorLoggerMock = new Mock<ILogger<LightSensorSimulator>>();
                lightSimulatorLogger = lightSimulatorLoggerMock.Object;
            }

            var service = new SimulatorsBackgroundService(configuration, logger, tempSimulatorLogger, lightSimulatorLogger);

            // Act
            var cancellationTokenSource = new CancellationTokenSource();
            var task = service.StartAsync(cancellationTokenSource.Token);

            await Task.Delay(TimeSpan.FromSeconds(10)); // Simulate 10 seconds of work

            // Stop the service
            cancellationTokenSource.Cancel();
            await task;
        }
    }
}
