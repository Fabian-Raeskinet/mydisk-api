using MediatR;
using MyDisk.Contracts.Disks;

namespace MyDisk.Services.Disks;

public class AttachAuthorCommandRequest : AttachAuthorCommand, IRequest<Unit>
{
}