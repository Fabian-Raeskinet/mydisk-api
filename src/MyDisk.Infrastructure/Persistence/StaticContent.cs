using MyDisk.Domain.Entities;

namespace MyDisk.Infrastructure.Persistence;

public static class StaticContent
{
    public static readonly List<Author> AuthorData = new List<Author>
    {
        new() { Id = Guid.NewGuid(), Pseudonyme = "Lomepal"},
        new() { Id = Guid.NewGuid(), Pseudonyme = "Roméo Elvis"},
        new() { Id = Guid.NewGuid(), Pseudonyme = "Orelsan"}
    };

    public static readonly List<Disk> DiskData = new List<Disk>
    {
        new() { Name = "Jeannine", Id = Guid.NewGuid(), ReleaseDate = new DateTime(2018, 8, 29), Author = AuthorData[0], CreatedDateTime = DateTime.Now},
        new() { Name = "Chocolat", Id = Guid.NewGuid(), ReleaseDate = new DateTime(2016, 2, 14), Author = AuthorData[1], CreatedDateTime = DateTime.Now }
    };
}