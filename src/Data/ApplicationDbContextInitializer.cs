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
            _context.Authors.Add(new Author { Pseudonym = new Pseudonym("Orelsan") });

            await _context.SaveChangesAsync();
        }

        if (!_context.Disks.Any())
        {
            var lomepal = new Author { Pseudonym = new Pseudonym("Lomepal") };
            var jeannine = new Disk { Name = new Name("Jeannine"), ReleaseDate = new DateTime(2018, 8, 29) };
            jeannine.AttachAuthor(lomepal);

            _context.Disks.Add(jeannine);

            var author = new Author { Pseudonym = new Pseudonym("Roméo Elvis") };
            var disk = new Disk { Name = new Name("Chocolat"), ReleaseDate = new DateTime(2016, 2, 14), };
            disk.AttachAuthor(author);
            _context.Disks.Add(disk);

            await _context.SaveChangesAsync();
        }
    }
}