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
    public class LightSensorController : ControllerBase
    {
        private readonly ILightSensorService _lightSensorService;

        public LightSensorController(ILightSensorService lightSensorService)
        {
            _lightSensorService = lightSensorService;
        }

        [HttpGet("statistics/{deviceid}")]
        public async Task<IActionResult> GetStatistics(long deviceid)
        {
            var statistics = await _lightSensorService.GetMaxIlluminanceForLastThirtyDaysAsync(deviceid);
            return Ok(statistics);
        }


        [AllowAnonymous]
        [HttpPost("{deviceId}/telemetry")]
        public async Task<IActionResult> PostTelemetry([FromBody] List<LightSensorViewModel> telemetryData)
        {
            await _lightSensorService.AddLightSensorDataAsync(telemetryData);

            return Ok("Telemetry data added successfully.");

        }
    }
}
