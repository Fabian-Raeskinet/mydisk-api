using MediatR;

namespace Contracts.Disks;

public class UpdateDiskCommand : IRequest<DiskResponse>
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public DateTime? ReleaseDate { get; set; }
}