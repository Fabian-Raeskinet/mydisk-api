namespace MyDisk.Domain.Models
{
    public class Disk
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string? ImageUrl { get; set; }
        public string? Author { get; set; }
    }
}
