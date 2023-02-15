using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyDisk.Domain.Common;

namespace MyDisk.Domain.Entities;

public class Author : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public string? Pseudonyme { get; set; }
}