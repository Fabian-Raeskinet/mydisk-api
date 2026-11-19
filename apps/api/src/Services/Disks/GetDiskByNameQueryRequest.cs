using MediatR;
using MyDisks.Contracts.Disks;

namespace MyDisks.Services.Disks;

public class GetDiskByNameQueryRequest : GetDiskByNameQuery, IQuery<DiskResult>
{
    public required DiskOwner Owner { get; init; }
}