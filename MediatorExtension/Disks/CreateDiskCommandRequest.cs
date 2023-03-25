using MediatR;
using MyDisk.Contracts.Disks;

namespace MediatorExtension.Disks;

public class CreateDiskCommandRequest : Request<CreateDiskCommand, Unit>
{
}