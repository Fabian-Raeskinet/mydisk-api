using MyDisk.Services.Authors.DTOs;

namespace MyDisk.Services.Disks.DTOs;

public class DiskResponse
{
    public string? Name { get; set; }
    public AuthorResponse? Author { get; set; }
    public DateTime? ReleaseDate { get; set; }
}