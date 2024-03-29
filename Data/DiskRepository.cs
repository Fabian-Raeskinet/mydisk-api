﻿using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyDisks.Domain;
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

    public async Task<IEnumerable<Disk>?> GetDisksAsync()
    {
        return await Context.Disks
            .Include(x => x.Author)
            .Include(x => x.Reviews.Where(r => r.Status != ReviewStatus.Archived))
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
        await SaveChanges();
    }

    public async Task DeleteDiskAsync(Disk disk)
    {
        Context.Disks.Remove(disk);
        await SaveChanges();
    }

    public async Task UpdateDiskAsync(Disk disk)
    {
        Context.Disks.Update(disk);
        await SaveChanges();
    }

    private async Task SaveChanges()
    {
        await Context.SaveChangesAsync();
    }
}