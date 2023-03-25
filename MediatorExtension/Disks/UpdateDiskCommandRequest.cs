using MediatR;
using MyDisk.Contracts.Disks;

namespace MediatorExtension.Disks;

public class UpdateDiskCommandRequest : UpdateDiskCommand, IRequest<Unit> { }