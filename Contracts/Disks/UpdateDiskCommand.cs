using MediatR;

namespace Contracts.Disks;

public class UpdateDiskCommand : IRequest<DiskResponse>
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? ReleaseDate { get; set; }
}