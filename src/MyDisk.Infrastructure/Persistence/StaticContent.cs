using MyDisk.Domain.Models;

namespace MyDisk.Infrastructure.Persistence
{
    public static class StaticContent
    {
        public static List<Author> AuthorData = new List<Author>
        {
            new Author { Id = Guid.NewGuid(), Firstname = "Lomepal"},
            new Author { Id = Guid.NewGuid(), Firstname = "Roméo Elvis"},
            new Author { Id = Guid.NewGuid(), Firstname = "Orelsan"}
        };

        public static List<Disk> DiskData = new List<Disk>
        {
            new Disk { Name = "Jeannine", Id = Guid.NewGuid(), ReleaseDate = new DateTime(2018, 8, 29), Author = AuthorData[0], CreatedDateTime = DateTime.Now},
            new Disk { Name = "Chocolat", Id = Guid.NewGuid(), ReleaseDate = new DateTime(2016, 2, 14), Author = AuthorData[1], CreatedDateTime = DateTime.Now }
        };

    }
}
