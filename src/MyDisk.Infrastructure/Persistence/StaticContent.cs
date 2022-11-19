using MyDisk.Domain.Models;

namespace MyDisk.Infrastructure.Persistence
{
    public static class StaticContent
    {
        public static List<Disk> ContextData = new List<Disk>
        {
            new Disk { Name = "Jeannine", Id = Guid.NewGuid(), ReleaseDate = new DateTime(2018, 8, 29), Author = "Lomepal", CreatedDateTime = DateTime.Now },
            new Disk { Name = "Chocolat", Id = Guid.NewGuid(), ReleaseDate = new DateTime(2016, 2, 14), Author = "Romeo Elvis", CreatedDateTime = DateTime.Now }
        };
    }
}
