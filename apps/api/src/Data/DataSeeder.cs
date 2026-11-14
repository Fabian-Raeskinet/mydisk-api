using Microsoft.EntityFrameworkCore;
using MyDisks.Domain.Authors;
using MyDisks.Domain.Disks;

namespace MyDisks.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context, CancellationToken cancellationToken = default)
    {
        if (!await context.Authors.AnyAsync(cancellationToken))
        {
            context.Authors.Add(new Author { Pseudonym = new Pseudonym("Orelsan") });

            await context.SaveChangesAsync(cancellationToken);
        }

        if (!await context.Disks.AnyAsync(cancellationToken))
        {
            var lomepal = new Author { Pseudonym = new Pseudonym("Lomepal") };
            var jeannine = new Disk { Name = new Name("Jeannine"), ReleaseDate = new DateTime(2018, 8, 29) };
            jeannine.AttachAuthor(lomepal);
            context.Disks.Add(jeannine);

            var romeoElvis = new Author { Pseudonym = new Pseudonym("Roméo Elvis") };
            var chocolat = new Disk { Name = new Name("Chocolat"), ReleaseDate = new DateTime(2016, 2, 14) };
            chocolat.AttachAuthor(romeoElvis);
            context.Disks.Add(chocolat);

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
