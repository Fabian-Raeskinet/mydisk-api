using MediatR;
using MyDisks.Contracts.Disks;

namespace MyDisks.Services.Disks;

public class UpdateDiskCommandRequest : UpdateDiskCommand, ICommand
{
    public required DiskOwner Owner { get; init; }
}