using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyDisks.Domain.Authors;
using MyDisks.Domain.Disks;

namespace MyDisks.Data;

public class ApplicationDbContextInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ApplicationDbContextInitializer> _logger;

    public ApplicationDbContextInitializer(ILogger<ApplicationDbContextInitializer> logger,
        ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            if (_context.Database.IsSqlServer())
                await _context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private async Task TrySeedAsync()
    {
        // Default data
        // Seed, if necessary
        if (!_context.Authors.Any())
        {
            // var author = new Author(new Guid());
            // author.Pseudonyme = "Orelsan";
            // _context.Authors.Add(author);


            _context.Authors.Add(new Author { Pseudonym = new Pseudonym("Orelsan") });

            await _context.SaveChangesAsync();
        }

        if (!_context.Disks.Any())
        {
            // var author1 = new Author(new Guid(), "Lomepal");
            // var disk1 = new Disk(new Guid(), "Jeannine", new DateTime(2018, 8, 29), author1);
            // _context.Disks.Add(disk1);
            //
            // var author2 = new Author(new Guid(), "Roméo Elvis");
            // var disk2 = new Disk(new Guid(), "Chocolat", new DateTime(2016, 2, 14), author2);
            // _context.Disks.Add(disk2);

            _context.Disks.Add(new Disk
            {
                Name = new Name("Jeannine"), ReleaseDate = new DateTime(2018, 8, 29),
                Author = new Author { Pseudonym = new Pseudonym("Lomepal") }
            });
            _context.Disks.Add(
                new Disk
                {
                    Name = new Name("Chocolat"), ReleaseDate = new DateTime(2016, 2, 14),
                    Author = new Author { Pseudonym = new Pseudonym("Roméo Elvis") }
                }
            );

            await _context.SaveChangesAsync();
        }
    }
}