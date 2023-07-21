using Microsoft.EntityFrameworkCore;
using MyDisk.Domain.Entities;

namespace MyDisks.Data;

public interface IApplicationDbContext
{
    DbSet<Disk> Disks { get; }
    DbSet<Author> Authors { get; }
    Task<int> SaveChangesAsync();
}