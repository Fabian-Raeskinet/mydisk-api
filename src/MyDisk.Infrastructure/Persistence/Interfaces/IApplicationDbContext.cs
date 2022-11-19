using MyDisk.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace MyDisk.Infrastructure.Persistence.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Disk> Disks { get; }
    }
}
