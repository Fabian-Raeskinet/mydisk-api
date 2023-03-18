using System.Linq.Expressions;
using MyDisk.Domain.Entities;

namespace MyDisk.Domain;

public interface IDiskRepository
{
    public Task<List<Disk>?> GetDisksAsync();
    public Task<Disk?> GetDiskByFilterAsync(Expression<Func<Disk, bool>> predicate);
    public Task<Guid> CreateDiskAsync(Disk disk);
    public Task<bool> DeleteDiskAsync(Disk disk);
    public Task UpdateDiskAsync(Disk disk);
}