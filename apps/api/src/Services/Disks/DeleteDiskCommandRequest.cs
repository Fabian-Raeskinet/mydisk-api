using MediatR;
using MyDisks.Contracts.Disks;

namespace MyDisks.Services.Disks;

public class DeleteDiskCommandRequest : DeleteDiskCommand, ICommand
{
    public required DiskOwner Owner { get; init; }
}