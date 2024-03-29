using Microsoft.Extensions.Options;

namespace MyDisks.Api.Maintenance;

public class MaintenanceHelper
{
    public static MaintenanceStatus IsMaintenance(IOptions<MaintenanceSettings> maintenanceSettings)
    {
        return maintenanceSettings.Value.Global ? MaintenanceStatus.Active : MaintenanceStatus.Inactive;
    }
}