using MediatR;

namespace Contracts.Disks;

public class DeleteDiskCommand : IRequest<Unit>
{
    public DeleteDiskByProperty Property { get; set; }
    public string? Value { get; set; }
}