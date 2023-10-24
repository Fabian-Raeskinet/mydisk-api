using MediatR;
using MyDisks.Contracts.Disks;

namespace MyDisks.Services.Disks;

public class CreateDiskCommandRequest : CreateDiskCommand, IRequest<Unit>
{
}