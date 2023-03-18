using MediatR;

namespace Contracts.Disks;

public class GetDiskByNameQuery : IRequest<DiskResponse>
{
    public string? Name { get; set; }
}

