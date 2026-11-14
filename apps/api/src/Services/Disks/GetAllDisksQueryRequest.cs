using MediatR;
using MyDisks.Contracts.Disks;

namespace MyDisks.Services.Disks;

public class GetAllDisksQueryRequest : GetAllDisksQuery, IQuery<IEnumerable<DiskResult>>
{
}