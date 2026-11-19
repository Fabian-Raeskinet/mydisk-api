using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyDisks.Domain.Disks;
using MyDisks.Domain.Reviews;

namespace MyDisks.Data;

public class DiskRepository : IDiskRepository
{
    public IApplicationDbContext Context { get; }

    public DiskRepository(IApplicationDbContext context)
    {
        Context = context;
    }

    public async Task<IEnumerable<Disk>?> GetDisksAsync(Expression<Func<Disk, bool>> predicate)
    {
        return await Context.Disks
            .Where(predicate)
            .Include(x => x.Author)
            .Include(x => x.Reviews.Where(r => r.Status == ReviewStatus.Published))
            .ToListAsync();
    }

    public async Task<Disk?> GetDiskByFilterAsync(Expression<Func<Disk, bool>> predicate)
    {
        return await Context.Disks
            .Include(x => x.Author)
            .Include(x => x.Reviews)
            .FirstOrDefaultAsync(predicate);
    }

    public async Task CreateDiskAsync(Disk disk)
    {
        await Context.Disks.AddAsync(disk);
    }

    public async Task DeleteDiskAsync(Disk disk)
    {
        Context.Disks.Remove(disk);
    }

    public Task UpdateDiskAsync(Disk disk)
    {
        if (Context.Disks.Entry(disk).State == EntityState.Detached)
            Context.Disks.Update(disk);

        return Task.CompletedTask;
    }
}
