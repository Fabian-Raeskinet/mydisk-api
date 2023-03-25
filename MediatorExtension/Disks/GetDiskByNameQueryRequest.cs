using MediatR;
using MyDisk.Contracts.Disks;

namespace MediatorExtension.Disks;

public class GetDiskByNameQueryRequest : GetDiskByNameQuery, IRequest<DiskResponse>
{
}