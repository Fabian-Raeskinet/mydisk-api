using MediatR;
using MyDisk.Contracts.Disks;

namespace MyDisk.Services.Disks;

public class CreateDiskCommandRequest : CreateDiskCommand, IRequest<Unit>
{
}