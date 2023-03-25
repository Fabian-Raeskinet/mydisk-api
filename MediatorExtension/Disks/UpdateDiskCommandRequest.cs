using MediatR;
using MyDisk.Contracts.Disks;

namespace MediatorExtension.Disks;

public class UpdateDiskCommandRequest : Request<UpdateDiskCommand, Unit> { }