using MediatR;

namespace Contracts.Disks;

public class GetAllDisksQuery : IRequest<List<DiskEntity>> { }