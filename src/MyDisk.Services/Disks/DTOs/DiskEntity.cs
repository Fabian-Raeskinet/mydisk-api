using MyDisk.Domain.Entities;

namespace MyDisk.Services.Disks.DTOs;

public class DiskEntity
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public string? ImageUrl { get; set; }
    public Author? Author { get; set; }
}