using MediatR;
using MyDisk.Contracts.Disks;

namespace MediatorExtension.Disks;

public class DeleteDiskCommandRequest : Request<DeleteDiskCommand, Unit>
{
}