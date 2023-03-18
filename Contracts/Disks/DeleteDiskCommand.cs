using MediatR;

namespace Contracts.Disks;

public class DeleteDiskCommand : IRequest<Unit>
{
    public DeleteDiskByPropertyEnum Property { get; set; }
    public string? Value { get; set; }
}