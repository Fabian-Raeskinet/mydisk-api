using MediatR;
using MyDisk.Contracts.Disks;

namespace MediatorExtension.Disks;

public class GetAllDisksQueryRequest : GetAllDisksQuery, IRequest<List<DiskResponse>> { }