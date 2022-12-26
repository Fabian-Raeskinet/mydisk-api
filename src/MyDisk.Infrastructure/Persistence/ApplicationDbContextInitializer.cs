using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyDisk.Domain.Entities;
using MyDisk.Infrastructure.Persistence.Identity;

namespace MyDisk.Infrastructure.Persistence;

public class ApplicationDbContextInitializer
{
    private ILogger<ApplicationDbContextInitializer> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ApplicationDbContextInitializer(ILogger<ApplicationDbContextInitializer> logger,
        ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
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
        // Default roles
        var administratorRole = new IdentityRole("Administrator");

        if (_roleManager.Roles.All(r => r.Name != administratorRole.Name))
        {
            await _roleManager.CreateAsync(administratorRole);
        }

        // Default users
        var administrator = new ApplicationUser { UserName = "administrator@localhost", Email = "administrator@localhost" };

        if (_userManager.Users.All(u => u.UserName != administrator.UserName))
        {
            await _userManager.CreateAsync(administrator, "Administrator1!");
            if (!string.IsNullOrWhiteSpace(administratorRole.Name))
            {
                await _userManager.AddToRolesAsync(administrator, new [] { administratorRole.Name });
            }
        }

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
                Name = "Jeannine", Id = Guid.NewGuid(), ReleaseDate = new DateTime(2018, 8, 29),
                Author = new Author { Pseudonyme = "Lomepal" },
            });
            _context.Disks.Add(
                new Disk()
                {
                    Name = "Chocolat", Id = Guid.NewGuid(), ReleaseDate = new DateTime(2016, 2, 14),
                    Author = new Author { Pseudonyme = "Roméo Elvis" }
                }
            );

            await _context.SaveChangesAsync();
        }
    }
}