using MediatR;
using MyDisks.Contracts.Disks;

namespace MyDisks.Services.Disks;

public class UpdateDiskCommandRequest : UpdateDiskCommand, IRequest<Unit> { }