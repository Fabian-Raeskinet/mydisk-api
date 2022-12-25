using MediatR;
using MyDisk.Services.Disks.DTOs;

namespace MyDisk.Services.Disks.Requests;

public class UpdateDiskRequest : IRequest<DiskResponse>
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public DateTime? ReleaseDate { get; set; }
}