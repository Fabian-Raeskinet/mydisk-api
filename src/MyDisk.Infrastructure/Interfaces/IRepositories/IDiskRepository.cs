using MyDisk.Domain.Models;

namespace MyDisk.Infrastructure.Interfaces.IRepositories;

public interface IDiskRepository
{
    public List<Disk>? GetDisks();
    public Disk? GetDiskByFilter(Func<Disk, bool> predicate);
}