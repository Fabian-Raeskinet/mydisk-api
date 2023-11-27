using System.Linq.Expressions;
using MyDisks.Domain.Entities;

namespace MyDisks.Domain;

public interface IDiskRepository
{
    public Task<IEnumerable<Disk>?> GetDisksAsync();
    public Task<Disk?> GetDiskByFilterAsync(Expression<Func<Disk, bool>> predicate);
    public Task<Guid> CreateDiskAsync(Disk disk);
    public Task<bool> DeleteDiskAsync(Disk disk);
    public Task UpdateDiskAsync(Disk disk);
}
