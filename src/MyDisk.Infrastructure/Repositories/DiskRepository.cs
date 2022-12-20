using MyDisk.Domain.Models;
using MyDisk.Infrastructure.Interfaces.IRepositories;
using MyDisk.Infrastructure.Persistence;

namespace MyDisk.Infrastructure.Repositories;

public class DiskRepository : IDiskRepository
{
    public List<Disk>? GetDisks() => StaticContent.DiskData;

    public Disk? GetDiskByFilter(Func<Disk, bool> predicate) => StaticContent.DiskData.FirstOrDefault(predicate);
}