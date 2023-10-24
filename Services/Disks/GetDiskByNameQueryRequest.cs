using MediatR;
using MyDisk.Contracts.Disks;

namespace MyDisk.Services.Disks;

public class GetDiskByNameQueryRequest : GetDiskByNameQuery, IRequest<DiskResult>
{
}