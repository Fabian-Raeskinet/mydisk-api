using MediatR;
using MyDisk.Contracts.Disks;

namespace MediatorExtension.Disks;

public class AttachAuthorCommandRequest : Request<AttachAuthorCommand, Unit>
{
}