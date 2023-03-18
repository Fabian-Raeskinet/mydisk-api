using MediatR;

namespace Contracts.Disks;

public class AttachAuthorCommand : IRequest<DiskResponse>
{
    public string AuthorId { get; set; }
    public string DiskId { get; set; }
}