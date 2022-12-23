using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyDisk.Domain.Models;

public class Disk
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public string? ImageUrl { get; set; }
    public Guid? AuthorId { get; set; }
    [ForeignKey("AuthorId")]
    public virtual Author? Author { get; set; }
}