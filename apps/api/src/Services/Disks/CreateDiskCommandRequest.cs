using MyDisks.Contracts.Disks;

namespace MyDisks.Services.Disks;

public class CreateDiskCommandRequest : CreateDiskCommand, ICommand
{
    public required string CreatedByIssuer { get; init; }
    public required string CreatedBySubject { get; init; }
}
