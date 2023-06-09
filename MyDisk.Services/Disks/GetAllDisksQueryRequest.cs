using MediatR;
using MyDisk.Contracts.Disks;

namespace MyDisk.Services.Disks;

public class GetAllDisksQueryRequest : GetAllDisksQuery, IRequest<IEnumerable<DiskResult>> { }