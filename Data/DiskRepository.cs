using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyDisks.Domain;
using MyDisks.Domain.Entities;

namespace MyDisks.Data;

public class DiskRepository : IDiskRepository
{
    private readonly IApplicationDbContext _context;

    public DiskRepository(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Disk>?> GetDisksAsync()
    {
        return await _context.Disks.Include(x => x.Author).ToListAsync();
    }

    public async Task<Disk?> GetDiskByFilterAsync(Expression<Func<Disk, bool>> predicate)
    {
        return await _context.Disks.Include(x => x.Author).FirstOrDefaultAsync(predicate);
    }

    public async Task<Guid> CreateDiskAsync(Disk disk)
    {
        var result = await _context.Disks.AddAsync(disk);
        await _context.SaveChangesAsync();
        return result.Entity.Id;
    }

    public async Task<bool> DeleteDiskAsync(Disk disk)
    {
        _context.Disks.Remove(disk);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task UpdateDiskAsync(Disk disk)
    {
        _context.Disks.Update(disk);
        await _context.SaveChangesAsync();
    }
}