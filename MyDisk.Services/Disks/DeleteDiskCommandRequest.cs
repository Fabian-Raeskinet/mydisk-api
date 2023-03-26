using MediatR;
using MyDisk.Contracts.Disks;

namespace MyDisk.Services.Disks;

public class DeleteDiskCommandRequest : DeleteDiskCommand, IRequest<Unit>
{
}