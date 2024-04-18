using Microsoft.EntityFrameworkCore;
using MyDisks.Domain.Authors;
using MyDisks.Domain.Disks;
using MyDisks.Domain.Reviews;

namespace MyDisks.Data;

public interface IApplicationDbContext
{
    DbSet<Disk> Disks { get; }
    DbSet<Author> Authors { get; }
    DbSet<Review> Reviews { get; }
    Task<int> SaveChangesAsync();
}