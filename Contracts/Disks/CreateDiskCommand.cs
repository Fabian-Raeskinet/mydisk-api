using MediatR;

namespace Contracts.Disks;

public class CreateDiskCommand : IRequest<Guid>
{
    public string? Name { get; set; }
    public string? ReleaseDate { get; set; }
}