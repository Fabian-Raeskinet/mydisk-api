using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyDisk.Domain.Common;

namespace MyDisk.Domain.Entities;

public class Disk : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public string? ImageUrl { get; set; }
    public Guid? AuthorId { get; set; }
    [ForeignKey("AuthorId")]
    public virtual Author? Author { get; set; }
}