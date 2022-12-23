using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyDisk.Domain.Models;

namespace MyDisk.Infrastructure.Persistence;

public class ApplicationDbContextInitializer
{
    private ILogger<ApplicationDbContextInitializer> _logger;
    private readonly ApplicationDbContext _context;

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
            {
                await _context.Database.MigrateAsync();
            }
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
            _context.Authors.Add(new Author { Pseudonyme = "Orelsan" });
            
            await _context.SaveChangesAsync();
        }

        if (!_context.Disks.Any())
        {
            _context.Disks.Add(new Disk
            {
                Name = "Jeannine", Id = Guid.NewGuid(), ReleaseDate = new DateTime(2018, 8, 29), Author = new Author { Pseudonyme = "Lomepal" },
                CreatedDateTime = DateTime.Now
            });
            _context.Disks.Add(
                new Disk()
                {
                    Name = "Chocolat", Id = Guid.NewGuid(), ReleaseDate = new DateTime(2016, 2, 14),
                    Author = new Author { Pseudonyme = "Roméo Elvis" }, CreatedDateTime = DateTime.Now
                }
            );

            await _context.SaveChangesAsync();
        }
    }
}