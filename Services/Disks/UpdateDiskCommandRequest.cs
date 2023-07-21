using MediatR;
using MyDisk.Contracts.Disks;

namespace MyDisk.Services.Disks;

public class UpdateDiskCommandRequest : UpdateDiskCommand, IRequest<Unit> { }