using MediatR;
using MyDisks.Contracts.Disks;

namespace MyDisks.Services.Disks;

public class GetDiskByNameQueryRequest : GetDiskByNameQuery, IRequest<DiskResult>
{
}