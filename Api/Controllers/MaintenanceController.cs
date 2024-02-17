using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyDisks.Api.Filters;
using MyDisks.Api.Maintenance;

namespace MyDisks.Api.Controllers;

public class MaintenanceController : ControllerBase
{
    public MaintenanceController(IOptions<MaintenanceSettings> maintenanceSettings)
    {
        MaintenanceSettings = maintenanceSettings;
    }

    public IOptions<MaintenanceSettings> MaintenanceSettings { get; }

    [HttpGet("maintenance")]
    [ProducesResponseType(typeof(MaintenanceStatusResult), StatusCodes.Status200OK)]
    [IgnoreMaintenanceFilter]
    public IActionResult IsMaintenance()
    {
        var status = MaintenanceHelper.IsMaintenance(MaintenanceSettings);
        return Ok(new MaintenanceStatusResult { Status = status });
    }
}