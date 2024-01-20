using Microsoft.EntityFrameworkCore;
using MyDisks.Domain.Authors;
using MyDisks.Domain.Disks;

namespace MyDisks.Data;

public interface IApplicationDbContext
{
    DbSet<Disk> Disks { get; }
    DbSet<Author> Authors { get; }
    Task<int> SaveChangesAsync();
}