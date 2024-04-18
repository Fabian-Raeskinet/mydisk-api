namespace MyDisks.Api.Maintenance;

public class MaintenanceStatusResult
{
    public MaintenanceStatus Status { get; init; }
}

public enum MaintenanceStatus
{
    Active = 1,
    Inactive
}