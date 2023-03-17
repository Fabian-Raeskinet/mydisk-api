using MediatR;

namespace Contracts.Disks;

public class AttachAuthorCommand : IRequest<DiskResponse>
{
    public Guid AuthorId { get; set; }
    public Guid DiskId { get; set; }
}