using MediatR;
using MyDisk.Services.Common.Enums;

namespace MyDisk.Services.Disks.Requests;

public class DeleteDiskRequest : IRequest<Unit>
{
    public DeleteDiskByPropertyEnum Property { get; set; }
    public string? Value { get; set; }
}