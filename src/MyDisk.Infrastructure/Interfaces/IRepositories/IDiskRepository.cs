using System.Linq.Expressions;
using MyDisk.Domain.Models;

namespace MyDisk.Infrastructure.Interfaces.IRepositories;

public interface IDiskRepository
{
    public Task<List<Disk>?> GetDisksAsync();
    public Task<Disk?> GetDiskByFilterAsync(Expression<Func<Disk, bool>> predicate);
    public Task<Guid> CreateDiskAsync(Disk disk);
}