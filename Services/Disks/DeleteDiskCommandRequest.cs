using MediatR;
using MyDisks.Contracts.Disks;

namespace MyDisks.Services.Disks;

public class DeleteDiskCommandRequest : DeleteDiskCommand, IRequest<Unit>
{
}