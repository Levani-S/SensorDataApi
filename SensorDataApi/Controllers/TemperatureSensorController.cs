using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using SensorDataApi.Services;
using SensorDataApi.ViewModels;

namespace SensorDataApi.Controllers
{
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    [Authorize]
    [ApiController]
    [Route("devices")]
    public class TemperatureSensorController : ControllerBase
    {
        private readonly ITempSensorService _tempSensorService;

        public TemperatureSensorController(ITempSensorService tempSensorService)
        {
            _tempSensorService = tempSensorService;
        }

        [HttpGet("tempStatistics/{deviceid}")]
        public async Task<IActionResult> GetTemperatureStatistics(long deviceid)
        {
            var statistics = await _tempSensorService.GetMaxTemperatureForLastThirtyDaysAsync(deviceid);
            return Ok(statistics);
        }


        [AllowAnonymous]
        [HttpPost("{deviceId}/temperature")]
        public async Task<IActionResult> PostTelemetry([FromBody] List<TempSensorViewModel> tempData)
        {
            await _tempSensorService.AddTempSensorDataAsync(tempData);

            return Ok("Temperature data added successfully.");

        }
    }
}
