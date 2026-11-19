using MediatR;
using MyDisks.Contracts.Disks;

namespace MyDisks.Services.Disks;

public class AttachAuthorCommandRequest : AttachAuthorCommand, ICommand
{
    public required DiskOwner Owner { get; init; }
}